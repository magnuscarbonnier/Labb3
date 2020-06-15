using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Services
{
    public class ProductService : IProductService
    {
        private const string apiaddress = "https://localhost:44307/api/products";

        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<Product>> GetAll()
        {
            List<Product> products = new List<Product>();
            var request = new HttpRequestMessage(HttpMethod.Get, apiaddress);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var content = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                products = JsonSerializer.Deserialize<List<Product>>(content, options);
            }
            return products;
        }

        public async Task<Product> GetById(Guid id)
        {
            Product product = new Product();
            var request = new HttpRequestMessage(HttpMethod.Get, apiaddress + $"/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var content = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                product = JsonSerializer.Deserialize<Product>(content, options);
            }
            return product;
        }
    }
}
