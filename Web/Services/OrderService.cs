using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Services
{
    public class OrderService : IOrderService
    {
        private const string apiaddress = "https://localhost:44328/api/orders";
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> PlaceOrder(string userId, Order order, ISession session)
        {
            //var existingOrders = session.Get<List<Order>>(Lib.SessionKeyOrderList);
            //var orders = new List<Order>();
            string message = Lib.OrderNotAdded;
            if (userId != null && order != null && order.UserId == userId)
            {
                order.Status = Lib.Status.Beställd;
                order.OrderDate = DateTime.Now;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };
                var response = await _httpClient.PostAsync(
                    apiaddress, new StringContent(JsonSerializer.Serialize(order, options), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                session.Remove(Lib.SessionKeyCart);
            }
            return message;
        }

        public Order GetOrder(string userId, ISession session)
        {
            var existingOrder= session.Get<Order>(Lib.SessionKeyOrder);
            var order = new Order();
            if (existingOrder != null && userId != null && existingOrder.UserId == userId)
            {
                order = existingOrder;
            }
            return order;
        }

        public List<Order> GetOrders(string userId, ISession session)
        {
            var existingOrders = session.Get<List<Order>>(Lib.SessionKeyOrderList);
            var orders = new List<Order>();

            if (existingOrders != null && userId != null && existingOrders.Any(c=>c.UserId==userId))
            {
                orders = existingOrders;
            }

            return orders;
        }

        public string AddOrder(string userId, Order order, ISession session)
        {
            var message = Lib.OrderNotAdded;
            if (order != null && order.UserId == userId && order.OrderItems!= null)
            {
                session.Set<Order>(Lib.SessionKeyOrder, order);
                message = Lib.OrderAdd;
            }
            return message;
        }
    }
}
