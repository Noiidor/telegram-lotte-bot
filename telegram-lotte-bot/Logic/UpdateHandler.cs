using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using telegram_lotte_bot.DTO;

namespace telegram_lotte_bot.Logic
{
    public class UpdateHandler
    {
        private readonly TelegramCredentials _credentials;
        private readonly ILogger _logger;
        private readonly CommandHandler _commandHandler;
        private readonly HttpClient _httpClient;

        int LONG_POOLING_TIMEOUT = 60;

        public UpdateHandler(TelegramCredentials credentials, ILogger logger, HttpClient httpClient, CommandHandler commandHandler)
        {
            _credentials = credentials;
            _logger = logger;
            _commandHandler = commandHandler;
            _httpClient = httpClient;
        }

        public async Task StartHandlingUpdates(CancellationToken cancellationToken)
        {
            long offset = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                List<MessageUpdate> updates = await GetUpdates(offset);

                updates.ForEach(e => offset = e.Id + 1);

                await _commandHandler.CheckUpdates(updates);
            }
        }

        private async Task<List<MessageUpdate>> GetUpdates(long offset)
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
                _logger.LogInformation("Updates response failed.");
            }
            return new();
        }
    }
}
