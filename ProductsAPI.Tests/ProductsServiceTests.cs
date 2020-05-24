using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProductsAPI.Tests
{
    public class ProductsServiceTests
    {
        [Fact]
        public async Task GetAll_ReturnOK()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.GetAsync("/api/products");
                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetProductWithEmptyGuidReturnsBadRequest()
        {
            using (var client = new TestClientProvider().Client)
            {
                Guid guid = Guid.NewGuid();
                var response = await client.GetAsync($"/api/products/{guid}");
                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetSingleNotAGuid_Return400()
        {
            using (var client = new TestClientProvider().Client)
            {
                Guid g = Guid.NewGuid();
                var response = await client.GetAsync($"/api/product/123");
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }
}