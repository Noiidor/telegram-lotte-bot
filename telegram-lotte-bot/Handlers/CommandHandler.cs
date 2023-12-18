using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using telegram_lotte_bot.DTO;

namespace telegram_lotte_bot.Handlers
{
    public class CommandHandler
    {
        private readonly TelegramCredentials _credentials;
        private readonly ILogger _logger;

        private static readonly HttpClient _httpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromHours(1)
        })
        {
            BaseAddress = new Uri(@$"https://api.telegram.org")
        };

        public CommandHandler(TelegramCredentials credentials, ILogger logger)
        {
            _credentials = credentials;
            _logger = logger;
        }

        public async Task SendMessage(long chatId, string text)
        {
            string apiEndpoint = $"/bot{_credentials.GetBotToken()}/sendMessage";

            var content = new StringContent($"chat_id={chatId}&text={Uri.EscapeDataString(text)}", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            _logger.LogInformation("Sending message.");
            HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Message send succesful.");
            }
            else
            {
                _logger.LogInformation("Message send error.");
            }
        }

        public async Task Reply(long chatId, long id, string text)
        {
            string apiEndpoint = $"/bot{_credentials.GetBotToken()}/sendMessage";

            var content = new StringContent($"reply_to_message_id={id}&chat_id={chatId}&text={Uri.EscapeDataString(text)}", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            _logger.LogInformation("Sending reply.");
            HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);

            string answer = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {


                _logger.LogInformation("Message reply send succesful.");
            }
            else
            {
                _logger.LogInformation("Message reply error.");
            }
        }

        public async Task HandleUpdates(List<Update> updates)
        {

        }
    }
}
