using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            _logger.LogInformation("POST item to cart...");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(apiEndpoint, jsonContent);
            }

            catch (TaskCanceledException)
            {
                _logger.LogWarning("POST item request timed out.");
                return false;
            }

            if (response.IsSuccessStatusCode) return true;
            else
            {
                string errorMessage = $"{response.StatusCode}";

                string? message = JObject.Parse(await response.Content.ReadAsStringAsync())?.SelectToken("message")?.ToString();
                if (message != null) errorMessage += $"\n\t{message}";

                _logger.LogWarning($"Item POST to cart error: {errorMessage}.");

                return false;
            }
        }

        public async Task<ItemInfo?> GetItemInfo(string urlKey)
        {
            string apiEndpoint = "/_next/data/-f_sRVlNCx0_DRruvtyOK/en-ntg/product/";

            apiEndpoint += urlKey + ".json";

            _logger.LogInformation("GET Item...");
            HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);


            if (response.IsSuccessStatusCode)
            {
                JObject jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());

                var itemContent = jsonObject.SelectToken("pageProps.content")?.ToObject<ItemInfo>();

                if (itemContent == null)
                {
                    _logger.LogError($"Item GET could not parse item content.");
                    return null;
                }

                return itemContent;

            }
            else
            {
                _logger.LogWarning($"Item GET error: {response.StatusCode}.");
                return null;
            }
        }
    }
}
