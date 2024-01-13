using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using telegram_lotte_bot.Application.Interfaces;
using telegram_lotte_bot.Domain.Telegram;

namespace telegram_lotte_bot.Infrastructure.Telegram
{
    public class TelegramSender : ITelegramSender
    {
        private readonly ICredentialsManager _credentials;
        private readonly ILogger<TelegramSender> _logger;
        private readonly HttpClient _httpClient;

        private const int TRIM_LOG_MESSAGE_LENGHT = 30;

        public TelegramSender(ICredentialsManager credentials, ILogger<TelegramSender> logger, IHttpClientFactory httpClientFactory)
        {
            _credentials = credentials;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("TelegramClient");
        }

        // Явный boilerplate, надо решить
        public async Task<Message?> SendMessage(long chatId, string text, long? replyToId)
        {
            string apiEndpoint = $"/bot{_credentials.GetBotToken()}/sendMessage";

            string rawContext = $"chat_id={chatId}&text={Uri.EscapeDataString(text)}";
            if (replyToId.HasValue) rawContext += $"&reply_to_message_id={replyToId}";

            var reqContent = new StringContent(rawContext, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            if (text.Length >= TRIM_LOG_MESSAGE_LENGHT) text = text.Remove(TRIM_LOG_MESSAGE_LENGHT) + "...";
            _logger.LogInformation($"Chat: {chatId}\n\tSending {(replyToId.HasValue ? "reply" : "message")}... \"{text}\"");
            HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, reqContent);

            if (response.IsSuccessStatusCode)
            {
                string resContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MessageResult>(resContent)!.Message;
            }
            else
            {
                _logger.LogInformation($"Message sent responded with code {response.StatusCode}.");
                return null;
            }
        }

        public async Task<Message?> EditMessage(long chatId, long messageId, string text)
        {
            string apiEndpoint = $"/bot{_credentials.GetBotToken()}/editMessageText";

            string rawContext = $"chat_id={chatId}&message_id={messageId}&text={Uri.EscapeDataString(text)}";

            var reqContent = new StringContent(rawContext, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            if (text.Length >= TRIM_LOG_MESSAGE_LENGHT) text = text.Remove(TRIM_LOG_MESSAGE_LENGHT) + "...";
            _logger.LogInformation($"Chat: {chatId}\n\tEditing message... \"{text}\"");

            HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, reqContent);

            if (response.IsSuccessStatusCode)
            {
                string resContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MessageResult>(resContent)!.Message;
            }
            else
            {
                _logger.LogInformation($"Message edit responded with code {response.StatusCode}.");
                return null;
            }
        }
    }
}
