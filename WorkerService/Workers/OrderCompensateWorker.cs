using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using WorkerService.Repository;
using WorkerService.Service;

namespace WorkerService.Workers
{
    /// <summary>
    /// 订单补偿服务
    /// </summary>
    public class OrderCompensateWorker : BackgroundService
    {
        private readonly WorkerOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public OrderCompensateWorker(
            IOptionsMonitor<WorkerOptions> options,
            IServiceScopeFactory scopeFactory,
            IMapper mapper)
        {
            _options = options.CurrentValue;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("订单补偿服务已启动");
            return;
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var _redis = scope.ServiceProvider.GetRequiredService<IRedisQueueService>();
                var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    Log.Information("开始执行订单补偿扫描");
                    int pageIndex = 1;
                    while (true)
                    {
                        Log.Information($"扫描第{pageIndex}页订单补偿数据页");
                        // 分页查询订单
                        var ordersPage = await _db.OrderTemps
                            .Where(x => x.ProcessStatus <= ProcessStatus.LockSuccess)
                            .OrderBy(x => x.CreateTime)
                            .Skip((pageIndex - 1) * _options.PageSize)
                            .Take(_options.PageSize)
                            .ToListAsync(stoppingToken);

                        Log.Information($"补偿订单数量: {ordersPage.Count}");
                        if (ordersPage.Count == 0)
                            break;

                        // 批量统计 DetailCount
                        var orderNos = ordersPage.Select(o => o.OrderNo).ToList();
                        var detailCounts = await _db.OrderDetailTemps
                            .Where(d => orderNos.Contains(d.OrderNo))
                            .GroupBy(d => d.OrderNo)
                            .Select(g => new { OrderNo = g.Key, Count = g.Count() })
                            .ToListAsync(stoppingToken);

                        var detailDict = detailCounts.ToDictionary(d => d.OrderNo, d => d.Count);

                        // 映射 DTO 并赋值 DetailCount
                        var orderDtos = ordersPage.Select(o =>
                        {
                            var dto = _mapper.Map<OrderTempWorkerDto>(o);
                            dto.DetailCount = detailDict.TryGetValue(o.OrderNo, out var count) ? count : 0;
                            return dto;
                        }).ToList();

                        // 构建 Redis DTO
                        var lockOrders = orderDtos.Select(order => new OrderInRedisDto
                        {
                            OrderNo = order.OrderNo,
                            DetailCount = order.DetailCount,
                            RetryCount = 0,
                            CreateTime = order.CreateTime.HasValue
                                ? new DateTimeOffset(order.CreateTime.Value).ToUnixTimeMilliseconds()
                                : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                        }).ToList();

                        await _redis.PushLockQueueBatch(lockOrders);
                        pageIndex++;
                    }
                    Log.Information("订单补偿执行完成");
                }
                catch (Exception ex)
                {
                    // 记录日志
                    Log.Error($"订单补偿服务异常：{ex.Message}");
                }
                finally
                {
                    await Sleep(stoppingToken);
                }
            }
            Log.Information("订单补偿服务已停止");
        }

        private async Task Sleep(CancellationToken stoppingToken)
        {
            Log.Information($"订单补偿服务休眠 {_options.CompensateIntervalSeconds} 秒");
            await Task.Delay(
                TimeSpan.FromSeconds(_options.CompensateIntervalSeconds),
                stoppingToken);
        }
    }
}
