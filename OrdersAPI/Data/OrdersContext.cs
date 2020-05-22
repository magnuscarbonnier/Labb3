using Microsoft.EntityFrameworkCore;
using OrdersAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Data
{
    public class OrdersContext : DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options)
           : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
    }
}
