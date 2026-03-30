using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Models;

namespace WorkerService.Repository
{
    public interface IOrderDetailRepository : IRepositoryBase<OrderDetail>
    {
    }

    public class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(AppDbContext db) : base(db)
        {
        }
    }
}
