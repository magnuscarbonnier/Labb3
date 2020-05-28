using Newtonsoft.Json;
using OrdersAPI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
                var response = await client.GetAsync("/api/orders");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetEmptyGuid_Returns_NOTFOUND()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.GetAsync($"/api/orders/{Guid.Empty}");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetRandomGuid_Returns_NOTFOUND()
        {
            using (var client = new TestClientProvider().Client)
            {
                Guid guid = Guid.NewGuid();
                var response = await client.GetAsync($"/api/orders/{guid}");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetNotAGUID_Returns_BadRequest()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.GetAsync($"/api/orders/1");
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetOrderById_Returns_Order()
        {
            using (var client = new TestClientProvider().Client)
            {
                var orderResponse = await client.GetAsync($"/api/orders/{_fixture.order.Id}");

                using (var responseStream = await orderResponse.Content.ReadAsStreamAsync())
                {
                    var order = await System.Text.Json.JsonSerializer.DeserializeAsync<Order>(responseStream,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    Assert.Equal(_fixture.order.Id, order.Id);
                }
            }
        }

        //[Fact]
        //public async Task PutOrder_returns_updatedOrder()
        //{
           
        //    using (var client = new TestClientProvider().Client)
        //    {
        //        var order = _fixture.order;
        //        order.Status = Status.Bekräftad;
                
        //        var payload = JsonConvert.SerializeObject(order);
        //        HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                
        //        //put order
        //        var putresponse = await client.PutAsync($"/api/orders/{order.Id}",content);

        //        using (var responseStream = await putresponse.Content.ReadAsStreamAsync())
        //        {
        //            var updatedorder = await System.Text.Json.JsonSerializer.DeserializeAsync<Order>(responseStream,
        //                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        //            var removeresponse = await client.DeleteAsync($"/api/orders/{updatedorder.Id}");
        //            Assert.Equal(order.Status,updatedorder.Status);
        //        }
        //    }
        //}
    }
}