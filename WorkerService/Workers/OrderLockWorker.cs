using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Dtos;
using WorkerService.Enums;
using WorkerService.Models;
using WorkerService.Service;
using static System.Formats.Asn1.AsnWriter;

namespace WorkerService.Workers
{
    /// <summary>
    /// 锁单服务
    /// </summary>
    public class OrderLockWorker : BackgroundService
    {
        private readonly WorkerOptions _options;
        private readonly IWebClient _api;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRedisQueueService _redis;

        public OrderLockWorker(IOptionsMonitor<WorkerOptions> options, IWebClient api, IServiceScopeFactory scopeFactory, IRedisQueueService redisQueueService)
        {
            _options = options.CurrentValue;
            _api = api;
            _scopeFactory = scopeFactory;
            _redis = redisQueueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("订单锁定服务已启动");
            var tasks = new List<Task>();

            for (int i = 0; i < _options.LockWorkerThreads; i++)
            {
                tasks.Add(Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken));
            }

            await Task.WhenAll(tasks);
            Log.Information("订单锁定服务已停止");
        }

        private async Task ConsumeAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                OrderInRedisDto orderInfo = null;
                try
                {
                    Log.Information($"线程 {Environment.CurrentManagedThreadId}-开始执行订单锁定扫描");
                    // 无需延时，Redis BLPop 已经是阻塞式获取订单号
                    // 只有当有订单号时才会继续执行后续逻辑
                    orderInfo = await _redis.BLPopLockQueue();
                    if (orderInfo == null)
                        continue;

                    Log.Debug($"获取订单 {orderInfo.OrderNo} 时间差（毫秒）：{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - orderInfo.CreateTime}");

                    Log.Information($"线程 {Environment.CurrentManagedThreadId}-获取订单 {orderInfo.OrderNo}");

                    // 防重复消费
                    if (!await _redis.PushLockOrder(orderInfo, _options.RedisLockSeconds))
                        continue;

                    await ProcessOrder(orderInfo);
                }
                catch (Exception ex)
                {
                    Log.Error($"线程 {Environment.CurrentManagedThreadId}-订单锁定线程异常 {ex}");
                }
                finally
                {
                    if (orderInfo != null)
                    {
                        Log.Warning($"线程 {Environment.CurrentManagedThreadId}-释放订单 {orderInfo.OrderNo}");
                        await _redis.ReleaseLock(orderInfo.OrderNo);
                    }
                }
            }
        }

        private async Task ProcessOrder(OrderInRedisDto orderInfo)
        {
            using var scope = _scopeFactory.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //// 超时判断
            //var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            //Log.Debug($"订单时间:{now - orderInfo.CreateTime}");
            //if (Math.Abs(now - orderInfo.CreateTime) > _options.LockTimeoutSeconds)
            //{
            //    Log.Warning($"订单 {orderInfo.OrderNo} 锁单超时");
            //    return;
            //}

            // 查询第三方是否锁单
            var result = await _api.Query(orderInfo.OrderNo);
            if (result.Result == 2)
            {
                Log.Warning($"订单 {orderInfo.OrderNo} 锁单失败，{result.Message}");
                await LockFail(_db, orderInfo);
                return;
            }

            var order = await _db.OrderTemps.AsQueryable().FirstAsync(x => x.OrderNo == orderInfo.OrderNo);

            if (result.Result == 1)
            {
                await _db.OrderTemps
                    .Where(x => x.OrderNo == orderInfo.OrderNo)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.ProcessStatus, ProcessStatus.LockSuccess)
                        .SetProperty(x => x.UpdateTime, DateTime.UtcNow)
                    );
                Log.Information($"订单 {orderInfo.OrderNo} 已锁定");
                return;
            }
            // 锁定中
            if (order.ProcessStatus == ProcessStatus.Locking && result.Result == 0)
            {
                await RetryOrder(orderInfo, _db);
                return;
            }
            var details = await _db.OrderDetailTemps.AsQueryable()
              .Where(x => x.OrderNo == order.OrderNo)
              .ToListAsync();
            var orderdetails = await _db.OrderDetails.AsQueryable()
            .Where(x => x.OrderNo == order.OrderNo)
            .ToListAsync();

            var lockResult = await _api.Lock(orderInfo.OrderNo);

            if (!result.Success)
            {
                await RetryOrder(orderInfo, _db);
                Log.Warning($"订单 {orderInfo.OrderNo} 创建锁单失败：{result.Message}");
                return;
            }
            await _db.OrderTemps
                .Where(x => x.OrderNo == orderInfo.OrderNo)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ProcessStatus, ProcessStatus.Locking)
                );
            await _redis.PushLockQueue(orderInfo);
            Log.Information($"线程 {Environment.CurrentManagedThreadId}-提交锁单成功，修改状态为锁定中，订单 {orderInfo.OrderNo}");
        }

        /// <summary>
        /// 重试逻辑
        /// </summary>
        private async Task RetryOrder(OrderInRedisDto orderInfo, AppDbContext db)
        {
            orderInfo.RetryCount++;

            if (orderInfo.RetryCount <= _options.MaxRetryCount)
            {
                await _redis.PushLockQueue(orderInfo);
                Log.Information($"订单 {orderInfo.OrderNo} 重试锁单 {orderInfo.RetryCount}");
            }
            else
            {
                Log.Warning($"订单 {orderInfo.OrderNo} 锁单失败，超过最大重试次数");
                await LockFail(db, orderInfo);
            }
        }

        private async Task LockFail(AppDbContext db, OrderInRedisDto orderInfo)
        {
            await db.OrderTemps
                .Where(x => x.OrderNo == orderInfo.OrderNo)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ProcessStatus, ProcessStatus.Failed)
                    .SetProperty(x => x.UpdateTime, DateTime.UtcNow)
                );

            await Unlock(orderInfo);
        }

        private async Task Unlock(OrderInRedisDto orderInfo)
        {
            var result = await _api.Unlock(orderInfo.OrderNo);
            if (!result.Success)
            {
                Log.Warning($"订单 {orderInfo.OrderNo} 解锁失败：{result.Message}");
                return;
            }

            Log.Warning($"订单 {orderInfo.OrderNo} 解锁成功");
        }
    }
}
