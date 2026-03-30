using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Enums;

namespace WorkerService.Models
{
    /// <summary>
    /// 订单临时表（用于异步处理）
    /// </summary>
    public class OrderTemp
    {
        /// <summary>主键</summary>
        public long Id { get; set; }

        /// <summary>订单号</summary>
        public string OrderNo { get; set; }

        /// <summary>处理状态</summary>
        public ProcessStatus ProcessStatus { get; set; }

        /// <summary>重试次数</summary>
        public int RetryCount { get; set; }

        /// <summary>下次执行时间</summary>
        public DateTime? NextExecuteTime { get; set; }

        /// <summary>第三方锁单号</summary>
        public string? ThirdLockNo { get; set; }

        /// <summary>创建时间</summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>更新时间</summary>
        public DateTime? UpdateTime { get; set; }
    }
}
