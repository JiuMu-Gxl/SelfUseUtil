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
        // 原有
        Task PushLockQueue(OrderInRedisDto order);
        Task PushLockQueueBatch(List<OrderInRedisDto> orders);
        Task<OrderInRedisDto> BLPopLockQueue();
        Task<bool> PushLockOrder(OrderInRedisDto order, int expireSeconds = -1);
        Task ReleaseLock(string orderNo);

        // 新增：重试队列（高优先级）
        Task PushRetryQueue(OrderInRedisDto order);
        Task<OrderInRedisDto> PopRetryQueue();

        // 新增：延迟队列（ZSet）
        Task PushDelayQueue(OrderInRedisDto order, int delaySeconds);
        Task<OrderInRedisDto> PopDelayQueue();
    }

    public class RedisQueueService : IRedisQueueService
    {
        private const string LOCK_QUEUE = "queue:order:lock";
        private const string RETRY_QUEUE = "queue:order:retry";
        private const string DELAY_QUEUE = "queue:order:delay";

        #region 原有队列

        public async Task PushLockQueue(OrderInRedisDto order)
        {
            await RedisHelper.LPushAsync(LOCK_QUEUE, order);
        }

        public async Task PushLockQueueBatch(List<OrderInRedisDto> orders)
        {
            if (orders.Count == 0) return;
            await RedisHelper.LPushAsync(LOCK_QUEUE, orders.ToArray());
        }

        public async Task<OrderInRedisDto> BLPopLockQueue()
        {
            try
            {
                var json = RedisHelper.BLPop(1, LOCK_QUEUE);
                if (json == null) return null;

                return JsonConvert.DeserializeObject<OrderInRedisDto>(json);
            }
            catch (Exception ex)
            {
                Log.Error($"BLPop失败: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region 重试队列（高优先级）

        public async Task PushRetryQueue(OrderInRedisDto order)
        {
            await RedisHelper.LPushAsync(RETRY_QUEUE, order);
        }

        public async Task<OrderInRedisDto> PopRetryQueue()
        {
            var json = await RedisHelper.LPopAsync(RETRY_QUEUE);
            if (json == null) return null;

            return JsonConvert.DeserializeObject<OrderInRedisDto>(json);
        }

        #endregion

        #region 延迟队列（ZSet）

        public async Task PushDelayQueue(OrderInRedisDto order, int delaySeconds)
        {
            var score = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + delaySeconds * 1000;
            var json = JsonConvert.SerializeObject(order);

            await RedisHelper.ZAddAsync(
                DELAY_QUEUE,
                (score, json)
            );
        }

        public async Task<OrderInRedisDto> PopDelayQueue()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // 获取到期任务
            var values = await RedisHelper.ZRangeByScoreAsync(DELAY_QUEUE, 0, now, 1);
            if (values == null || values.Length == 0)
                return null;

            var json = values[0];

            // 删除（防重复消费）
            await RedisHelper.ZRemAsync(DELAY_QUEUE, json);

            return JsonConvert.DeserializeObject<OrderInRedisDto>(json);
        }

        #endregion

        #region 分布式锁

        public async Task<bool> PushLockOrder(OrderInRedisDto order, int expireSeconds = -1)
        {
            return await RedisHelper.SetAsync(
                $"lock:order:{order.OrderNo}",
                "1",
                expireSeconds,
                RedisExistence.Nx);
        }

        public async Task ReleaseLock(string orderNo)
        {
            await RedisHelper.DelAsync($"lock:order:{orderNo}");
        }

        #endregion
    }
}
