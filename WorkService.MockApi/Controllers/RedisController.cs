using WorkService.MockApi.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkService.MockApi.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkService.MockApi.Models.Mock;

namespace WorkService.MockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        /// <summary>
        /// 测试：推送单个订单到锁定队列（LPush queue:order:lock）
        /// </summary>
        [HttpPost("push-lock-queue")]
        public async Task<bool> PushLockQueue([FromBody] OrderInRedisDto order)
        {
            var json = JsonConvert.SerializeObject(order);
            // 推入 Redis 队列
            var result = await RedisHelper.LPushAsync("queue:order:lock", json);
            return result > 0;
        }

        /// <summary>
        /// 测试：批量推送订单到锁定队列
        /// </summary>
        [HttpPost("push-lock-queue-batch")]
        public async Task<bool> PushLockQueueBatch([FromBody] List<OrderInRedisDto> orders)
        {
            if (orders.Count == 0) return false;
            var jsons = new List<string>();
            foreach (var o in orders) jsons.Add(JsonConvert.SerializeObject(o));
            var result = await RedisHelper.LPushAsync("queue:order:lock", jsons.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 测试：从锁定队列阻塞获取订单（BLPop）
        /// </summary>
        [HttpGet("blpop-lock-queue")]
        public async Task<OrderInRedisDto> BLPopLockQueue()
        {
            try
            {
                // 阻塞 10 秒
                var json = RedisHelper.BLPop(10, "queue:order:lock");
                if (string.IsNullOrEmpty(json)) return null;
                return JsonConvert.DeserializeObject<OrderInRedisDto>(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 测试：从锁定队列非阻塞获取订单（RPop）
        /// </summary>
        [HttpGet("pop-lock-queue")]
        public async Task<OrderInRedisDto> PopLockQueue()
        {
            var json = await RedisHelper.RPopAsync("queue:order:lock");
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<OrderInRedisDto>(json);
        }

        /// <summary>
        /// 测试：推送单个订单到创建队列
        /// </summary>
        [HttpPost("push-create-queue")]
        public async Task<bool> PushCreateQueue([FromBody] OrderInRedisDto order)
        {
            var json = JsonConvert.SerializeObject(order);
            var result = await RedisHelper.LPushAsync("queue:order:create", json);
            return result > 0;
        }

        /// <summary>
        /// 测试：从创建队列获取订单（非阻塞）
        /// </summary>
        [HttpGet("pop-create-queue")]
        public async Task<OrderInRedisDto> PopCreateQueue()
        {
            var json = await RedisHelper.RPopAsync("queue:order:create");
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<OrderInRedisDto>(json);
        }

        /// <summary>
        /// 测试：推送延迟订单任务
        /// </summary>
        [HttpPost("push-delay")]
        public async Task<bool> PushDelay([FromQuery] string orderNo, [FromQuery] int seconds)
        {
            var score = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + seconds;
            await RedisHelper.ZAddAsync("delay:order:check", (score, orderNo));
            return true;
        }
    }
}
