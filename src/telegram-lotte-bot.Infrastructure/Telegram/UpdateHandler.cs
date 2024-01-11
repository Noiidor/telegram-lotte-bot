using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using telegram_lotte_bot.Application.Interfaces;
using telegram_lotte_bot.Domain.Telegram;

namespace telegram_lotte_bot.Infrastructure.Telegram
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ICredentialsManager _credentials;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly HttpClient _httpClient;

        private const int LONG_POOLING_TIMEOUT = 60;

        public UpdateHandler(ILogger<UpdateHandler> logger, ICredentialsManager credentials, IHttpClientFactory httpClientFactory)
        {
            _credentials = credentials;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("TelegramClient");
        }

        public async Task<List<MessageUpdate>> GetUpdates(long offset)
        {
            string apiEndpoint = $"/bot{_credentials.GetBotToken()}/getUpdates?timeout={LONG_POOLING_TIMEOUT}&offset={offset}";

            _logger.LogInformation("Receiving updates...");

            var response = await _httpClient.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Updates received.");

                string stringContent = await response.Content.ReadAsStringAsync();

                JObject jsonObject = JObject.Parse(stringContent);

                string? resultString = jsonObject["result"]?.ToString();

                if (string.IsNullOrEmpty(resultString)) return new();

                return JsonConvert.DeserializeObject<List<MessageUpdate>>(resultString) ?? new();
            }
            else
            {
                _logger.LogWarning(await response.Content.ReadAsStringAsync());

                _logger.LogWarning("Updates response failed.");
            }
            return new();
        }
    }
}
