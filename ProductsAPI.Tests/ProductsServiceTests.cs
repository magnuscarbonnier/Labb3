using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ProductsAPI.Tests
{
    public class ProductsServiceTests : IClassFixture<ProductFixture>
    {
        ProductFixture _fixture;

        public ProductsServiceTests(ProductFixture fixture)
        {
            _fixture = fixture;

        }
        [Fact]
        public async Task GetAll_Returns_OK()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.GetAsync("/api/products");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetEmptyGuid_Returns_NOTFOUND()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.GetAsync($"/api/products/{Guid.Empty}");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetRandomGuid_Returns_NOTFOUND()
        {
            using (var client = new TestClientProvider().Client)
            {
                Guid guid = Guid.NewGuid();
                var response = await client.GetAsync($"/api/products/{guid}");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetNotAGUID_Returns_NOTFOUND()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.GetAsync($"/api/products/1");
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetProductById_Returns_Product()
        {
            using (var client = new TestClientProvider().Client)
            {
                var productsResponse = await client.GetAsync($"/api/products/{_fixture.product.Id}");

                using(var responseStream=await productsResponse.Content.ReadAsStreamAsync())
                {
                    var product = await System.Text.Json.JsonSerializer.DeserializeAsync<Product>(responseStream,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    Assert.Equal(_fixture.product.Id, product.Id);
                }
            }
        }
    }
}