using Newtonsoft.Json;
using ProductService.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductsAPI.Tests
{
    public class ProductFixture : IDisposable
    {
        public Product product { get; private set; }

        public ProductFixture()
        {
            product = Initialize().Result;
        }

        private async Task<Product> Initialize()
        {


            using (var client = new TestClientProvider().Client)
            {
                var testProduct = new Product
                {
                    Name = "Testprodukt",
                    ImgSrc = "https://www.w3schools.com/images/colorpicker.gif",
                    Description = "Testbeskrivning",
                    Price = 98.00M
                };

                var payload = new StringContent(JsonConvert.SerializeObject(testProduct), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"/api/products", payload);

                var createdProduct = await JsonHandler.Deserialize<Product>(response);
                return createdProduct;
            }
        }

        public async void Dispose()
        {
            using (var client = new TestClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync($"/api/products/{product.Id}");
                deleteResponse.EnsureSuccessStatusCode();
            }
        }
    }
}
