using CSRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RedisDemo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecutionScheduleController : ControllerBase
    {
        private Task _task;
        private string _redisKey = "GetRunSchedule";

        [HttpGet]
        public async Task<int> Get() {
            if (_task != null && _task.IsCompleted)
            {
                _task = null;
                return 100;
            }
            var lockRedis = RedisHelper.Instance.TryLock(_redisKey, 60 * 60);
            var runSchedule = RedisHelper.Instance.Get(_redisKey);
            if (lockRedis == null) {
                if (runSchedule == null)
                {
                    _task = Task.Run(async () =>
                    {
                        // 模拟执行任务的过程
                        for (int i = 1; i <= 100; i++)
                        {
                            // 等待一段时间
                            await Task.Delay(1000);
                            RedisHelper.Instance.Set(_redisKey, i);
                        }
                        lockRedis.Unlock();
                    });
                }
                if (int.Parse(runSchedule ?? "0") == 100)
                {
                    RedisHelper.Instance.Del(_redisKey);
                }
            }

            return int.Parse(runSchedule ?? "0");
        }
    }
}
