using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrdersAPI.Tests
{
    public class JsonHandler
    {
        public static async Task<T> Deserialize<T>(HttpResponseMessage response)
        {
            var JSON = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(JSON, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}
