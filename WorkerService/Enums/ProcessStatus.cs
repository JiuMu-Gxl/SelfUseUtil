using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Enums
{
    /// <summary>
    /// 临时表处理状态
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>待处理</summary>
        Pending = 0,

        /// <summary>锁定中</summary>
        Locking = 1,

        /// <summary>锁定成功</summary>
        LockSuccess = 2,

        /// <summary>创建中</summary>
        Creating = 3,

        /// <summary>完成</summary>
        Completed = 4,

        /// <summary>失败</summary>
        Failed = 5
    }
}
