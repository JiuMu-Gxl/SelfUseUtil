using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Models;

namespace WorkerService.Repository
{
    public interface IOrderDetailTempRepository : IRepositoryBase<OrderDetailTemp>
    {
    }

    public class OrderDetailTempRepository : RepositoryBase<OrderDetailTemp>, IOrderDetailTempRepository
    {
        public OrderDetailTempRepository(AppDbContext db) : base(db)
        {
        }
    }
}
