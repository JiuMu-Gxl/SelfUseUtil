using CSRedis;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Dtos;

namespace WorkerService.Service
{
    public interface IRedisQueueService {
        Task PushLockQueueBatch(List<OrderInRedisDto> orders);
        Task<OrderInRedisDto> BLPopLockQueue();
        Task<bool> PushLockOrder(OrderInRedisDto order, int expireSeconds = -1);
        Task ReleaseLock(string orderNo);
    }

    public class RedisQueueService : IRedisQueueService
    {
        /// <summary>
        /// 批量推送订单到库存锁定队列
        /// 用于订单创建时进入库存锁定流程
        /// </summary>
        /// <param name="orders">订单信息</param>
        public async Task PushLockQueueBatch(List<OrderInRedisDto> orders)
        {
            if (orders.Count == 0) return;

            await RedisHelper.LPushAsync("queue:order:lock", orders.ToArray());
        }

        /// <summary>
        /// 从库存锁定队列阻塞获取订单
        /// </summary>
        /// <returns>订单号</returns>
        public async Task<OrderInRedisDto> BLPopLockQueue()
        {
            try
            {
                // 10秒阻塞，没数据返回 null
                var json = RedisHelper.BLPop(1, "queue:order:lock");
                if (json == null)
                {
                    Log.Warning($"Redis BLPop 为获取到内容");
                    return null;
                }
                // 反序列化成对象
                return JsonConvert.DeserializeObject<OrderInRedisDto>(json);
            }
            catch (Exception ex)
            {
                Log.Error($"Redis BLPop 阻塞失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 推送订单到库存锁定等待队列
        /// 用于锁定库存时进入锁定等待查询流程
        /// </summary>
        /// <param name="order"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        public async Task<bool> PushLockOrder(OrderInRedisDto order, int expireSeconds = -1)
        {
            return await RedisHelper.SetAsync($"lock:order:{order.OrderNo}", order, expireSeconds, RedisExistence.Nx);
        }

        /// <summary>
        /// 释放订单锁
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public async Task ReleaseLock(string orderNo)
        {
            await RedisHelper.DelAsync($"lock:order:{orderNo}");
        }
    }
}
