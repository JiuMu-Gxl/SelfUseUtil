using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Dtos;
using WorkerService.Models;
using WorkerService.Service;

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

        public OrderLockWorker(IOptionsMonitor<WorkerOptions> options, IWebClient api, IServiceScopeFactory scopeFactory)
        {
            _options = options.CurrentValue;
            _api = api;
            _scopeFactory = scopeFactory;
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
                var _redis = scope.ServiceProvider.GetRequiredService<IRedisQueueService>();
                var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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
            // 超时判断
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Log.Debug($"订单时间:{now - orderInfo.CreateTime}");
            if (Math.Abs(now - orderInfo.CreateTime) > _options.LockTimeoutSeconds)
            {
                Log.Warning($"订单 {orderInfo.OrderNo} 锁单超时");
                return;
            }
            // 查询第三方是否锁单
            var result = await _api.Query(orderInfo.OrderNo);

            if (!result.Success)
            {
                Log.Warning($"订单 {orderInfo.OrderNo} 锁单失败：{result.Message}");
                return;
            }

        }
    }
}
