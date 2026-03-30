using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Models;

namespace WorkerService.Repository
{
    public interface IOrderTempRepository : IRepositoryBase<OrderTemp>
    {
    }

    public class OrderTempRepository : RepositoryBase<OrderTemp>, IOrderTempRepository
    {
        public OrderTempRepository(AppDbContext db) : base(db)
        {
        }
    }
}
