using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Models
{
    public class WorkerOptions
    {
        /// <summary>
        /// 补偿任务执行间隔(秒)
        /// </summary>
        public int CompensateIntervalSeconds { get; set; }

        /// <summary>
        /// 补偿任务扫描订单分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 锁定订单的工作线程数，线程越多，处理订单的速度越快，但也可能增加系统负载和锁冲突的风险
        /// </summary>
        public int LockWorkerThreads { get; set; }

        /// <summary>
        /// 订单锁定结果的 Redis 键过期时间(秒)，过期后会自动删除，避免缓存雪崩和占用过多内存
        /// </summary>
        public int RedisLockSeconds { get; set; }

        /// <summary>
        /// 订单锁定超时时间(秒)，超过这个时间未完成的订单会被认为锁定失败
        /// </summary>
        public int LockTimeoutSeconds { get; set; }

        /// <summary>
        /// 锁定失败重试次数，超过这个次数的订单会被认为锁定失败
        /// </summary>
        public int MaxRetryCount { get; set; }
    }
}
