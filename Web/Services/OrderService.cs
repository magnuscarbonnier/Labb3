﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        [Authorize]
        public async Task<Guid> PlaceOrder(string userId, Order order, ISession session, string token)
        {
            if (userId != null && order != null && order.UserId == userId)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var JSON = JsonConvert.SerializeObject(order);
                var orderContent = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiaddress, orderContent);
                var orderResponse = await response.Content.ReadAsStringAsync();
                var ordersaved = JsonConvert.DeserializeObject<Order>(orderResponse);
                if (ordersaved != null)
                {
                    session.Remove(Lib.SessionKeyCart);
                }

                return ordersaved.Id;
            }
            return Guid.Empty;
        }

        public async Task<Order> GetOrderById(Guid orderId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(apiaddress + $"/{orderId}");
            response.EnsureSuccessStatusCode();
            var orderresponse = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<Order>(orderresponse);
            return order;
        }

        public List<Order> GetOrders(string userId, ISession session, string token)
        {
            var existingOrders = session.Get<List<Order>>(Lib.SessionKeyOrderList);
            var orders = new List<Order>();

            if (existingOrders != null && userId != null && existingOrders.Any(c => c.UserId == userId))
            {
                orders = existingOrders;
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetUserOrders(string userid, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(apiaddress + $"/user/{userid}");
            response.EnsureSuccessStatusCode();
            var ordersresponse = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(ordersresponse);
            return orders;
        }
    }
}
