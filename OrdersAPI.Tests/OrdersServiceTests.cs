using Newtonsoft.Json;
using OrdersAPI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace OrdersAPI.Tests
{
    public class OrdersServiceTests : IClassFixture<OrderFixture>
    {
        OrderFixture _fixture;

        public OrdersServiceTests(OrderFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAll_Returns_OK()
        {
            using (var client = new TestClientProvider().Client)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://localhost:44328/api/orders/")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);

                var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task GetAll_Returns_orderitems()
        {
            using (var client = new TestClientProvider().Client)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://localhost:44328/api/orders/")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var response = client.SendAsync(request);
                var orderResponse = await response.Result.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(orderResponse);

                foreach (var order in orders)
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        Assert.NotNull(orderItem);
                    }
                }
            }
        }

        [Fact]
        public async Task GetEmptyGuid_Returns_NOTFOUND()
        {
            using (var client = new TestClientProvider().Client)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://localhost:44328/api/orders/" + Guid.Empty)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task GetRandomGuid_Returns_NOTFOUND()
        {
            using (var client = new TestClientProvider().Client)
            {

                Guid guid = Guid.NewGuid();
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://localhost:44328/api/orders/" + guid)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task GetNotAGUID_Returns_BadRequest()
        {
            using (var client = new TestClientProvider().Client)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://localhost:44328/api/orders/1")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }


        [Fact]
        public async Task GetUserOrders_Returns_Order()
        {
            using (var client = new TestClientProvider().Client)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/user/{_fixture.order.UserId}")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var orderResponse = await client.SendAsync(request);

                using (var responseStream = await orderResponse.Content.ReadAsStreamAsync())
                {
                    var orders = await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<Order>>(responseStream,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    foreach (var order in orders)
                    {
                        Assert.Equal(_fixture.order.UserId, order.UserId);
                    }

                }
            }
        }

        [Fact]
        public async Task GetUserOrders_Returns_OK()
        {
            using (var client = new TestClientProvider().Client)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/user/{_fixture.order.UserId}")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var orderResponse = await client.SendAsync(request);

                Assert.Equal(HttpStatusCode.OK, orderResponse.StatusCode);

            }
        }

        [Fact]
        public async Task GetOrderById_Returns_Order()
        {
            using (var client = new TestClientProvider().Client)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/{_fixture.order.Id}")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var orderResponse = await client.SendAsync(request);

                using (var responseStream = await orderResponse.Content.ReadAsStreamAsync())
                {
                    var order = await System.Text.Json.JsonSerializer.DeserializeAsync<Order>(responseStream,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    Assert.Equal(_fixture.order.Id, order.Id);
                }
            }
        }

        [Fact]
        public async Task PutOrder_returns_updatedOrder()
        {

            using (var client = new TestClientProvider().Client)
            {
                var order = _fixture.order;
                order.Status = Status.Bekräftad;

                var payload = JsonConvert.SerializeObject(order);
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                //put order
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = content,
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/{_fixture.order.Id}")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var putResponse = await client.SendAsync(request);

                var orderResponse = await putResponse.Content.ReadAsStringAsync();
                var changedOrder = JsonConvert.DeserializeObject<Order>(orderResponse);

                Assert.Equal(order.Status, changedOrder.Status);
            }
        }

        [Fact]
        public async Task Put_Empty_Order_WithexistingGuid_returns_BadRequest()
        {
            //existing guid
            var guid = _fixture.order.Id;
            //empty order
            var order = new Order();

            using (var client = new TestClientProvider().Client)
            {
                var payload = JsonConvert.SerializeObject(order);
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = content,
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/{_fixture.order.Id}")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var putResponse = await client.SendAsync(request);


                Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            }
        }

        [Fact]
        public async Task PutOrder_returns_Status_OK()
        {
            using (var client = new TestClientProvider().Client)
            {
                var order = _fixture.order;
                order.Phone = "123456";

                var payload = JsonConvert.SerializeObject(order);
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                //put order
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = content,
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/{_fixture.order.Id}")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var putResponse = await client.SendAsync(request);


                Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Delete_Order_returns_Id()
        {
            var testOrder = new Order
            {
                Address = "Testadress",
                City = "Teststad",
                Email = "test@test.test",
                FirstName = "Testförnamn",
                LastName = "Testefternamn",
                OrderDate = DateTime.Now,
                Phone = "123",
                ZipCode = "123",
                Status = Status.Packas,
                UserId = Guid.NewGuid().ToString(),
                OrderItems = new List<OrderItem> { new OrderItem { Name = "Testprodukt2", Price = 48.00M, ProductId = Guid.NewGuid(), Quantity = 2 } }
            };
            using (var client = new TestClientProvider().Client)
            {

                var payload = JsonConvert.SerializeObject(testOrder);
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                //post testorder
                HttpRequestMessage postrequest = new HttpRequestMessage
                {
                    Content = content,
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/")
                };
                postrequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var postResponse = await client.SendAsync(postrequest);

                var orderResponse = await postResponse.Content.ReadAsStringAsync();

                var order = JsonConvert.DeserializeObject<Order>(orderResponse);
                //delete testorder
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/{order.Id}")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var deleteResponse = await client.SendAsync(request);
                var deletedOrderResponse = await deleteResponse.Content.ReadAsStringAsync();
                var deletedOrderId = JsonConvert.DeserializeObject<Guid>(deletedOrderResponse);

                Assert.Equal(order.Id, deletedOrderId);
            }
        }

        [Fact]
        public async Task Delete_Order_EmptyGuid_returns_NotFound()
        {
            var emptyGuid = Guid.Empty;

            using (var client = new TestClientProvider().Client)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/{emptyGuid}")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var deleteResponse = await client.SendAsync(request);

                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Post_Order_returns_Created()
        {
            var testOrder = new Order
            {
                Address = "Testadress",
                City = "Teststad",
                Email = "test@test.test",
                FirstName = "Testförnamn",
                LastName = "Testefternamn",
                OrderDate = DateTime.Now,
                Phone = "12345",
                ZipCode = "12345",
                Status = Status.Skickad,
                UserId = Guid.NewGuid().ToString(),
                OrderItems = new List<OrderItem> { new OrderItem { Name = "Testprodukt3", Price = 8.00M, ProductId = Guid.NewGuid(), Quantity = 4 } }
            };
            using (var client = new TestClientProvider().Client)
            {

                var payload = JsonConvert.SerializeObject(testOrder);
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                //post testorder
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = content,
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var postresponse = await client.SendAsync(request);

                var orderResponse = await postresponse.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(orderResponse);
                //delete testorder
                HttpRequestMessage deleterequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"https://localhost:44328/api/orders/{order.Id}")
                };
                deleterequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.token);
                var deleteResponse = await client.SendAsync(deleterequest);

                Assert.Equal(HttpStatusCode.Created, postresponse.StatusCode);
            }
        }
    }
}