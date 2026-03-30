using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Workers
{
    /// <summary>
    /// 订单创建服务
    /// </summary>
    public class OrderCreateWorker : BackgroundService
    {

        public OrderCreateWorker()
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }
    }
}
