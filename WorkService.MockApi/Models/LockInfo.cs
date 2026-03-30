using WorkService.MockApi.Enums;
using System;

namespace WorkService.MockApi.Models
{
    public class LockInfo
    {
        public string OrderNo { get; set; }

        public int Status { get; set; } // 0处理中 1成功 2失败

        public DateTime CreateTime { get; set; }

        public int DelaySeconds { get; set; } // 随机延迟

        public int FinalStatus { get; set; } // 最终结果（提前决定）
        public bool NeverFinish { get; set; } // 模拟卡死 永不回传
    }

    public class LockResult
    {
        public string No { get; set; }
        public string Mode { get; set; }
        public int DelaySeconds { get; set; } 
    }
    public class SetRuleRequest
    {
        /// <summary>订单号</summary>
        public string OrderNo { get; set; }

        /// <summary>模拟模式</summary>
        public MockMode Mode { get; set; }
    }
}
