using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Models;

namespace WorkerService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        /// <summary>订单表</summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>订单临时表</summary>
        public DbSet<OrderTemp> OrderTemps { get; set; }

        /// <summary>订单明细</summary>
        public DbSet<OrderDetail> OrderDetails { get; set; }

        /// <summary>订单明细临时表</summary>
        public DbSet<OrderDetailTemp> OrderDetailTemps { get; set; }

        /// <summary>
        /// 用 Fluent API
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 统一 schema
            modelBuilder.HasDefaultSchema("Order");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderTemp>().ToTable("OrderTemp");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetail");
            modelBuilder.Entity<OrderDetailTemp>().ToTable("OrderDetailTemp");
        }
    }
}
