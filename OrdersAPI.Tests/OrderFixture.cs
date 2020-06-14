using Newtonsoft.Json;
using OrdersAPI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrdersAPI.Tests
{
    public class OrderFixture : IDisposable
    {
        public Order order { get; private set; }

        public OrderFixture()
        {
            order = Initialize().Result;
        }

        private async Task<Order> Initialize()
        {


            using (var client = new TestClientProvider().Client)
            {
                var testOrder = new Order
                {
                    Address = "Testadress",
                    City = "Teststad",
                    Email = "test@test.test",
                    FirstName = "Testförnamn",
                    LastName = "Testefternamn",
                    OrderDate = DateTime.Now,
                    Phone = "000011111",
                    ZipCode="29281",
                    Status = Status.Beställd,
                    UserId = Guid.NewGuid().ToString(),
                    OrderItems=new List<OrderItem> {new OrderItem { Name = "Testprodukt", Price = 98.00M, ProductId=Guid.NewGuid(), Quantity=3} }
                };

                var payload = new StringContent(JsonConvert.SerializeObject(testOrder), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"/api/orders", payload);
                var responseOrder = await response.Content.ReadAsStringAsync();

                var createdProduct = JsonConvert.DeserializeObject<Order>(responseOrder);
                return createdProduct;
            }
        }

        public async void Dispose()
        {
            using (var client = new TestClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync($"/api/orders/{order.Id}");
                deleteResponse.EnsureSuccessStatusCode();
            }
        }
    }
}
