using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using telegram_lotte_bot.DTO.Lotte;

namespace telegram_lotte_bot.Services
{
    public class LotteApiService
    {
        private readonly ILogger _logger;
        private readonly UserSecretsHelper _credentials;
        private static readonly HttpClient _httpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromHours(1)
        })
        {
            BaseAddress = new Uri(@$"https://www.lottemart.vn")
        };

        public LotteApiService(ILogger logger, UserSecretsHelper credentials)
        {
            _logger = logger;
            _credentials = credentials;

            _httpClient.DefaultRequestHeaders.Add("Authorization", _credentials.GetAuthString());
        }

        public async Task GetCartItems()
        {
            string apiEndpoint = "/v1/p/mart/bos/en_ntg/V1/carts/mine/totals/custom?cart_type=mart";


        }

        public async Task<bool> AddToCart(long itemId, int quantity)
        {
            string apiEndpoint = "/v1/p/mart/bos/en_ntg/V1/carts-later/mine/items";

            CartPost cart = new();

            CartPostItem cartItem = new()
            {
                Id = itemId,
                Quantity = quantity
            };

            cart.CartItems.Add(cartItem);

            StringContent jsonContent = new(JsonConvert.SerializeObject(cart), Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending POST request to add cart item...");
            HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, jsonContent);

            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Item added succesfully.");
                return true;
            }
            else
            {
                _logger.LogInformation($"Error status code while adding item to cart: {response.StatusCode}.");
                return false;
            }
        }
    }
}
