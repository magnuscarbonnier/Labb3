using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public async Task GetProductById_Returns_ProductId()
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

        [Fact]
        public async Task Add_New_Product_Returns_CreatedProduct()
        {
            // Create testproduct
            var product = new Product { Description = "Testbeksriv", ImgSrc = "imgsrc", Name = "Test", Price = 2.00M };
            
            using (var client = new TestClientProvider().Client)
            {
                var json = JsonHandler.Serialize<Product>(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/products/", content);
                var newProduct = await JsonHandler.Deserialize<Product>(response);
                
                Assert.NotNull(newProduct);
                Assert.NotEqual(Guid.Empty, newProduct.Id);
                
                //cleanup
                var deleteresponse=await client.DeleteAsync($"/api/products/{newProduct.Id}");
            }
        }

        [Fact]
        public async Task Change_product_returns_updated_productid()
        {
            // Create testproduct
            var product = new Product { Description = "Testbeksriv", ImgSrc = "imgsrc", Name = "Test", Price = 2.00M };

            using (var client = new TestClientProvider().Client)
            {
                var json = JsonHandler.Serialize<Product>(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/products/", content);
                var newProduct = await JsonHandler.Deserialize<Product>(response);

                newProduct.Name = "changednametest";
                json = JsonHandler.Serialize<Product>(newProduct);
                content = new StringContent(json, Encoding.UTF8, "application/json");

                response = await client.PutAsync($"/api/products/{newProduct.Id}", content);
                var updatedProduct = await JsonHandler.Deserialize<Product>(response);


                Assert.NotNull(updatedProduct);
                Assert.Equal(newProduct.Name, updatedProduct.Name);

                //cleanup
                var deleteresponse = await client.DeleteAsync($"/api/products/{newProduct.Id}");
            }
        }

        [Fact]
        public async Task Delete_Product_ReturnId()
        {
            // Create testproduct
            var product = new Product { Description = "Testbeksriv", ImgSrc = "imgsrc", Name = "Test", Price = 2.00M };

            using (var client = new TestClientProvider().Client)
            {
                var json = JsonHandler.Serialize<Product>(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/products/", content);
                var newProduct = await JsonHandler.Deserialize<Product>(response);
                var deleteresponse = await client.DeleteAsync($"/api/products/{newProduct.Id}");
   
                
                var deletedProduct = await JsonHandler.Deserialize<Product>(deleteresponse);


                Assert.NotEqual(Guid.Empty,deletedProduct.Id);
                Assert.Equal(newProduct.Id, deletedProduct.Id);

                
            }
            
           
        }
    }
}