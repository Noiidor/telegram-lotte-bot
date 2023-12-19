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

        private const int TRIM_LOG_MESSAGE_LENGHT = 30;

        public BotInteractionManager(TelegramCredentials credentials, ILogger logger, HttpClient httpClient)
        {
            _credentials = credentials;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task SendMessage(long chatId, string text, long? replyToId)
        {
            string apiEndpoint = $"/bot{_credentials.GetBotToken()}/sendMessage";

            string rawContext = $"chat_id={chatId}&text={Uri.EscapeDataString(text)}";
            if (replyToId.HasValue) rawContext += $"&reply_to_message_id={replyToId}";

            var content = new StringContent(rawContext, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            if (text.Length >= TRIM_LOG_MESSAGE_LENGHT) text = text.Remove(TRIM_LOG_MESSAGE_LENGHT) + "...";
            _logger.LogInformation($"Chat: {chatId}\n\tSending {(replyToId.HasValue ? "reply" : "message")}... \"{text}\""); // лол
            HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Message send succesfully.");
            }
            else
            {
                _logger.LogInformation($"Message sent responded with code {response.StatusCode}.");
            }
        }
    }
}
