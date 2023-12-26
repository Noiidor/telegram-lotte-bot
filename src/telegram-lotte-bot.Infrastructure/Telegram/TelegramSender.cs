using Microsoft.Extensions.Logging;
using telegram_lotte_bot.Application.Interfaces;

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

        public async Task EditMessage(long chatId, int messageId, string text)
        {

        }
    }
}
