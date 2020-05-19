using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService
{
  
        public class ProductsContext : DbContext
        {
            public ProductsContext(DbContextOptions<ProductsContext> options)
               : base(options)
            {
            }

            public DbSet<Product> Products { get; set; }

        }

    
}
