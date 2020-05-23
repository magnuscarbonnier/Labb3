using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Models;
using Newtonsoft.Json;
using Web.ViewModels;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        public async Task<Guid> PlaceOrder(string userId, Order order, ISession session)
        {
            if (userId != null && order != null && order.UserId == userId)
            {
                var JSON = JsonConvert.SerializeObject(order);
                var orderContent = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiaddress, orderContent);
                var orderResponse = await response.Content.ReadAsStringAsync();
                var ordersaved = JsonConvert.DeserializeObject<Order>(orderResponse);
                if(ordersaved!=null)
                {
                    session.Remove(Lib.SessionKeyCart);
                }
                
                return ordersaved.Id;
            }
            return Guid.Empty;
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            var response = await _httpClient.GetAsync(apiaddress + "/" + orderId);
            response.EnsureSuccessStatusCode();
            var orderresponse = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<Order>(orderresponse);
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

        public async Task<IEnumerable<Order>> GetUserOrders(string userid)
        {
            var response = await _httpClient.GetAsync(apiaddress + "/users/" + userid);
            response.EnsureSuccessStatusCode();
            var ordersresponse = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(ordersresponse);
            return orders;
        }
    }
}
