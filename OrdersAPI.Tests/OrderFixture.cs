using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OrdersAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrdersAPI.Tests
{
    public class OrderFixture : IDisposable
    {
        public Order order { get; private set; }
        public string token;

        public OrderFixture()
        {
            token = GenerateJSONWebToken();
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
                    ZipCode = "29281",
                    Status = Status.Beställd,
                    UserId = Guid.NewGuid().ToString(),
                    OrderItems = new List<OrderItem> { new OrderItem { Name = "Testprodukt", Price = 98.00M, ProductId = Guid.NewGuid(), Quantity = 3 } }
                };
                var payload = new StringContent(JsonConvert.SerializeObject(testOrder), Encoding.UTF8, "application/json");
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = payload,
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:44328/api/orders/")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);

                var responseOrder = await response.Content.ReadAsStringAsync();

                var createdProduct = JsonConvert.DeserializeObject<Order>(responseOrder);
                return createdProduct;
            }
        }

        public async void Dispose()
        {
            using (var client = new TestClientProvider().Client)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri("https://localhost:44328/api/orders/" + order.Id)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var deleteResponse = await client.SendAsync(request);
                deleteResponse.EnsureSuccessStatusCode();
            }
        }
        private string GenerateJSONWebToken()
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, "test@test.test")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperdupersecretPassword2000"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:44344/",
                audience: "https://localhost:44344/",
                expires: DateTime.Now.AddMinutes(3),
                signingCredentials: credentials,
                claims: claims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
