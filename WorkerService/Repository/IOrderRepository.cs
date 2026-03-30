using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Models;

namespace WorkerService.Repository
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
    }

    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext db) : base(db)
        {
        }
    }
}
