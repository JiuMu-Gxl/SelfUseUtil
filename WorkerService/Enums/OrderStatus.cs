using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Enums
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>待处理</summary>
        Pending = 0,

        /// <summary>成功</summary>
        Success = 1,

        /// <summary>失败</summary>
        Failed = 2
    }
}
