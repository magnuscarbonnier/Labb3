using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Services
{
    public interface IProductService
    {
        Task<Product> GetById(Guid Id);
        Task<List<Product>> GetAll();
    }
}
