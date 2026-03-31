using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mono.TextTemplating;
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
    public class OrderNewLockWorker : BackgroundService
    {
        private readonly WorkerOptions _options;
        private readonly IWebClient _api;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRedisQueueService _redis;

        public OrderNewLockWorker(
            IOptionsMonitor<WorkerOptions> options, 
            IWebClient api, 
            IServiceScopeFactory scopeFactory, 
            IRedisQueueService redisQueueService)
        {
            _options = options.CurrentValue;
            _api = api;
            _scopeFactory = scopeFactory;
            _redis = redisQueueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("订单锁定服务已启动");

            var tasks = Enumerable.Range(0, _options.LockWorkerThreads)
                .Select(_ => Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken));

            await Task.WhenAll(tasks);
            Log.Information("订单锁定服务已停止");
        }

        /// <summary>
        /// 消费队列
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task ConsumeAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                OrderInRedisDto orderInfo = null;

                try
                {
                    orderInfo = await GetOrder();

                    if (orderInfo == null)
                    {
                        await Task.Delay(50, stoppingToken);
                        continue;
                    }

                    // 防重复消费（分布式锁）
                    if (!await _redis.PushLockOrder(orderInfo, _options.RedisLockSeconds))
                        continue;

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    await ProcessOrder(orderInfo, db);
                }
                catch (Exception ex)
                {
                    Log.Error($"订单线程异常 {ex}");
                }
                finally
                {
                    if (orderInfo != null)
                        await _redis.ReleaseLock(orderInfo.OrderNo);
                }
            }
        }

        private async Task<OrderInRedisDto> GetOrder()
        {
            //// 先取重试队列（最快）
            //var order = await _redis.PopRetryQueue();
            //if (order != null) return order;

            //// 再取普通队列
            //order = await _redis.BLPopLockQueue();
            //if (order != null) return order;

            //// 最后取延迟队列
            //order = await _redis.PopDelayQueue();
            //return order;

            // 阻塞队列（核心）
            var order = await _redis.BLPopLockQueue();
            if (order != null) return order;

            // 重试队列
            order = await _redis.PopRetryQueue();
            if (order != null) return order;

            // 延迟队列
            return await _redis.PopDelayQueue();
        }

        private async Task ProcessOrder(OrderInRedisDto orderInfo, AppDbContext db)
        {
            //// 超时判断
            //var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            //Log.Debug($"订单时间:{now - orderInfo.CreateTime}");
            //if (Math.Abs(now - orderInfo.CreateTime) > _options.LockTimeoutSeconds)
            //{
            //    Log.Warning($"订单 {orderInfo.OrderNo} 锁单超时");
            //    return;
            //}

            var order = await db.OrderTemps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrderNo == orderInfo.OrderNo);

            if (order == null)
            {
                Log.Warning("订单不存在 {OrderNo}", orderInfo.OrderNo);
                return;
            }

            switch (order.ProcessStatus)
            {
                case ProcessStatus.Pending:
                    await HandlePending(orderInfo, db);
                    break;

                case ProcessStatus.Locking:
                    await HandleLocking(orderInfo, db);
                    break;

                case ProcessStatus.LockSuccess:
                    await HandleLockSuccess(orderInfo, db);
                    break;

                case ProcessStatus.Creating:
                    // 后续扩展（创建正式订单）
                    Log.Information("订单创建中 {OrderNo}", orderInfo.OrderNo);
                    break;

                case ProcessStatus.Completed:
                    // 终态
                    Log.Information("订单已完成 {OrderNo}", orderInfo.OrderNo);
                    break;

                case ProcessStatus.Failed:
                    // 终态
                    Log.Warning("订单已失败 {OrderNo}", orderInfo.OrderNo);
                    break;

                default:
                    Log.Warning("未知状态 {Status} - {OrderNo}", order.ProcessStatus, orderInfo.OrderNo);
                    break;
            }
        }

        private async Task HandlePending(OrderInRedisDto orderInfo, AppDbContext db)
        {
            // 防重复执行
            var updated = await db.OrderTemps
                .Where(x => x.OrderNo == orderInfo.OrderNo && x.ProcessStatus == ProcessStatus.Pending)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.ProcessStatus, ProcessStatus.Locking)
                    .SetProperty(p => p.UpdateTime, DateTime.UtcNow)
                );

            if (updated == 0)
                return;

            var lockResult = await _api.Lock(orderInfo.OrderNo);

            if (!lockResult.Success)
            {
                Log.Warning("订单 {OrderNo} 锁单失败：{Msg}", orderInfo.OrderNo, lockResult.Message);
                await RetryOrder(orderInfo, db);
                return;
            }

            Log.Information("订单 {OrderNo} 已发起锁单", orderInfo.OrderNo);

            await RetryOrder(orderInfo, db);
        }

        private async Task HandleLocking(OrderInRedisDto orderInfo, AppDbContext db)
        {
            var result = await _api.Query(orderInfo.OrderNo);

            // 成功
            if (result.Result == 1)
            {
                await db.OrderTemps
                    .Where(x => x.OrderNo == orderInfo.OrderNo && x.ProcessStatus == ProcessStatus.Locking)
                    .ExecuteUpdateAsync(x => x
                        .SetProperty(p => p.ProcessStatus, ProcessStatus.LockSuccess)
                        .SetProperty(p => p.UpdateTime, DateTime.UtcNow)
                    );

                Log.Information("订单锁定成功 {OrderNo}", orderInfo.OrderNo);
                return;
            }

            // 失败
            if (result.Result == 2)
            {
                await LockFail(db, orderInfo);
                return;
            }

            // 继续轮询
            await RetryOrder(orderInfo, db);
        }

        private async Task HandleLockSuccess(OrderInRedisDto orderInfo, AppDbContext db)
        {
            // 更新为创建中（防止重复创建）
            var updated = await db.OrderTemps
                .Where(x => x.OrderNo == orderInfo.OrderNo && x.ProcessStatus == ProcessStatus.LockSuccess)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.ProcessStatus, ProcessStatus.Creating)
                    .SetProperty(p => p.UpdateTime, DateTime.UtcNow)
                );

            if (updated == 0)
                return;

            // TODO：创建正式订单（你后面加）
            Log.Information("订单进入创建阶段 {OrderNo}", orderInfo.OrderNo);

            // 模拟创建成功
            await db.OrderTemps
                .Where(x => x.OrderNo == orderInfo.OrderNo)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.ProcessStatus, ProcessStatus.Completed)
                    .SetProperty(p => p.UpdateTime, DateTime.UtcNow)
                );
        }

        /// <summary>
        /// 重试逻辑
        /// </summary>
        private async Task RetryOrder(OrderInRedisDto orderInfo, AppDbContext db)
        {
            orderInfo.RetryCount++;

            if (orderInfo.RetryCount > _options.MaxRetryCount)
            {
                Log.Warning($"订单 {orderInfo.OrderNo} 超过最大重试次数");
                await LockFail(db, orderInfo);
                return;
            }

            // 分级策略
            if (orderInfo.RetryCount <= 3)
            {
                // 极速重试（无延迟）
                await _redis.PushRetryQueue(orderInfo);
            }
            else if (orderInfo.RetryCount <= 6)
            {
                await _redis.PushDelayQueue(orderInfo, 1);
            }
            else if (orderInfo.RetryCount <= 10)
            {
                await _redis.PushDelayQueue(orderInfo, 3);
            }
            else
            {
                await _redis.PushDelayQueue(orderInfo, 5);
            }

            Log.Information($"订单 {orderInfo.OrderNo} 第 {orderInfo.RetryCount} 次重试");
        }

        /// <summary>
        /// 锁单失败处理
        /// </summary>
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

        /// <summary>
        /// 解锁库存
        /// </summary>
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
