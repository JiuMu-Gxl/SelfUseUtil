using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Dtos
{
    public class OrderInRedisDto
    {
        public string OrderNo { get; set; }
        public int DetailCount { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// 创建时间（Unix）
        /// </summary>
        public long CreateTime { get; set; }
    }
}
