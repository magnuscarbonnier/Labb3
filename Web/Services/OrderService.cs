using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Models;
using Newtonsoft.Json;

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
        public async Task<Order> PlaceOrder(string userId, Order order, ISession session)
        {
            if (userId != null && order != null && order.UserId == userId)
            {
                var orderDTO = CartToOrder(order);
                var JSON = JsonConvert.SerializeObject(orderDTO);
                var orderContent = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiaddress, orderContent);
                var orderResponse = await response.Content.ReadAsStringAsync();
                session.Remove(Lib.SessionKeyCart);
                var ordersaved = OrderDTOToOrder(JsonConvert.DeserializeObject<OrderDTO>(orderResponse));
                return ordersaved;
            }

            return null;
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

        public OrderDTO CartToOrder(Order order)
        {
            return new OrderDTO
            {
                OrderItems = order.OrderItems.Select(x => new OrderItem
                {
                    ProductId = x.Product.Id,
                    Description = x.Product.Description,
                    ImgSrc = x.Product.ImgSrc,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    Quantity = x.Quantity
                }).ToList(),
                Address = order.Address,
                City = order.City,
                Email = order.Email,
                FirstName = order.FirstName,
                LastName = order.LastName,
                OrderDate = DateTime.Now,
                Phone = order.Phone,
                Status = Lib.Status.Beställd,
                UserId = order.UserId,
                ZipCode = order.ZipCode
            };
        }

        public Order OrderDTOToOrder(OrderDTO order)
        {
            List<Item> items = new List<Item>();
            foreach(var item in order.OrderItems)
            {
                Item orderitem = new Item();
                orderitem.Product.Description = item.Description;
                orderitem.Product.Id = item.ProductId;
                orderitem.Product.ImgSrc = item.ImgSrc;
                orderitem.Product.Name = item.Name;
                orderitem.Product.Price = item.Price;
                orderitem.Quantity = item.Quantity;
                items.Add(orderitem);
            }
            return new Order
            {
                OrderItems = items,
                Address = order.Address,
                City = order.City,
                Email = order.Email,
                FirstName = order.FirstName,
                LastName = order.LastName,
                OrderDate = order.OrderDate,
                Phone = order.Phone,
                Status = order.Status,
                UserId = order.UserId,
                ZipCode = order.ZipCode,
                Id=order.Id
            };
        }
    }
}
