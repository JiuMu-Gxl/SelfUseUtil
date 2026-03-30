using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Models
{
    /// <summary>
    /// 订单明细表
    /// </summary>
    public class OrderDetail
    {
        /// <summary>主键</summary>
        public long Id { get; set; }

        /// <summary>订单号</summary>
        public string OrderNo { get; set; }

        /// <summary>商品ID</summary>
        public string ProductId { get; set; }

        /// <summary>商品名称</summary>
        public string? ProductName { get; set; }

        /// <summary>数量</summary>
        public int Quantity { get; set; }

        /// <summary>单价</summary>
        public decimal Price { get; set; }

        /// <summary>创建时间</summary>
        public DateTime CreateTime { get; set; }
    }
}
