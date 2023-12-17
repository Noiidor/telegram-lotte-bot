using Microsoft.Extensions.Logging;
using System.Net;

namespace telegram_lotte_bot.Handlers
{
    public class CommandsHandler
    {
        // Чисто сахар что бы каждый раз не писать _credentials.GetBotToken()
        private string BotToken { get { return _credentials.GetBotToken(); } }

        private string ChatId { get { return _credentials.GetChatId(); } } // Поменять на динамически-получаемый id

        private readonly TelegramCredentials _credentials;
        private readonly ILogger _logger;

        private static readonly HttpClient _httpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromHours(1)
        })
        {
            BaseAddress = new Uri(@$"https://api.telegram.org")
        };

        public CommandsHandler(TelegramCredentials credentials, ILogger logger)
        {
            _credentials = credentials;
            _logger = logger;
        }

        public async Task SendMessage(string text)
        {
            string apiEndpoint = $"/bot{BotToken}/sendMessage";

            var content = new StringContent($"chat_id={ChatId}&text={Uri.EscapeDataString(text)}", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);

            _logger.LogInformation("test");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                await Console.Out.WriteLineAsync(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.Out.WriteLine("Нет ответа.");
            }

        }
    }
}
