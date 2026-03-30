using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Models;

namespace WorkerService.Dtos
{
    public class OrderTempWorkerDto : OrderTemp
    {
        public int DetailCount { get; set; } = 0;
    }
}
