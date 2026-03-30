using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Enums
{
    /// <summary>
    /// 锁单状态
    /// </summary>
    public enum LockStatus
    {
        /// <summary>未锁</summary>
        None = 0,

        /// <summary>锁定中</summary>
        Locking = 1,

        /// <summary>成功</summary>
        Success = 2,

        /// <summary>失败</summary>
        Failed = 3
    }
}
