using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using telegram_lotte_bot.Application.Interfaces;
using telegram_lotte_bot.Domain.Lotte;

namespace telegram_lotte_bot.Infrastructure.Lotte
{
    public class LotteClient : ILotteClient
    {
        private readonly ILogger<LotteClient> _logger;
        private readonly ICredentialsManager _credentials;
        private readonly HttpClient _httpClient;

        public LotteClient(ILogger<LotteClient> logger, ICredentialsManager credentials, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _credentials = credentials;
            _httpClient = httpClientFactory.CreateClient("LotteClient");
        }

        public async Task GetCartItems()
        {
            string apiEndpoint = "/v1/p/mart/bos/en_ntg/V1/carts/mine/totals/custom?cart_type=mart";


        }

        public async Task<bool> AddToCart(string itemId, int quantity)
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
