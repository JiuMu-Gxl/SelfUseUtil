using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Enums;

namespace WorkerService.Models
{
    /// <summary>
    /// 订单表
    /// </summary>
    public class Order
    {
        /// <summary>主键</summary>
        public long Id { get; set; }

        /// <summary>订单号</summary>
        public string OrderNo { get; set; }

        /// <summary>用户ID</summary>
        public string UserId { get; set; }

        /// <summary>订单总金额</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>订单状态</summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>锁单状态</summary>
        public LockStatus LockStatus { get; set; }

        /// <summary>支付状态</summary>
        public PayStatus PayStatus { get; set; }

        /// <summary>第三方订单号</summary>
        public string? ThirdOrderNo { get; set; }

        /// <summary>创建时间</summary>
        public DateTime CreateTime { get; set; }

        /// <summary>更新时间</summary>
        public DateTime? UpdateTime { get; set; }
    }
}
