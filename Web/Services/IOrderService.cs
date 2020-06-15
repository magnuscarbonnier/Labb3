using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Services
{
    public interface IOrderService
    {
        List<Order> GetOrders(string userId, ISession session, string token);
        Task<Guid> PlaceOrder(string userId, Order order, ISession session, string token);
        Task<Order> GetOrderById(Guid orderId, string token);
        Task<IEnumerable<Order>> GetUserOrders(string userid, string token);
    }
}
