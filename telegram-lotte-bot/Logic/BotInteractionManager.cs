using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace telegram_lotte_bot.Logic
{
    public class BotInteractionManager
    {
        private readonly TelegramCredentials _credentials;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public BotInteractionManager(TelegramCredentials credentials, ILogger logger, HttpClient httpClient)
        {
            _credentials = credentials;
            _logger = logger;
            _httpClient = httpClient;
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
    }
}
