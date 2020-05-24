using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Services
{
    public interface IOrderService
    {
        List<Order> GetOrders(string userId, ISession session);
        Task<Guid> PlaceOrder(string userId, Order order, ISession session);
        Task<Order> GetOrderById(Guid orderId);
        Task<IEnumerable<Order>> GetUserOrders(string userid);
    }
}
