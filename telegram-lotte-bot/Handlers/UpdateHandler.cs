using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using telegram_lotte_bot.DTO;

namespace telegram_lotte_bot.Handlers
{
    public class UpdateHandler
    {
        private readonly TelegramCredentials _credentials;
        private readonly ILogger _logger;
        private readonly CommandHandler _commandHandler;

        private static readonly HttpClient _httpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromHours(1)
        })
        {
            BaseAddress = new Uri(@$"https://api.telegram.org")
        };

        int LONG_POOLING_TIMEOUT = 60;

        public UpdateHandler(TelegramCredentials credentials, ILogger logger, CommandHandler commandHandler)
        {
            _credentials = credentials;
            _logger = logger;
            _commandHandler = commandHandler;
        }

        public async Task StartHandlingUpdates(CancellationToken cancellationToken)
        {
            long offset = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                List<Update> updates = await GetUpdates(offset);
                updates.Select(e => offset = e.Id + 1);

                await _commandHandler.HandleUpdates(updates);
            }
        }

        private async Task<List<Update>> GetUpdates(long offset)
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

                return JsonConvert.DeserializeObject<List<Update>>(resultString) ?? new();
            }
            else
            {
                _logger.LogInformation("Updates response failed.");
            }
            return new();
        }
    }
}
