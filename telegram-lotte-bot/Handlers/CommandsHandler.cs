
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace telegram_lotte_bot.Handlers
{
    public class CommandsHandler
    {
        // Чисто сахар что бы каждый раз не писать _credentials.GetBotToken()
        private string BotToken { get { return _credentials.GetBotToken(); } }

        private string ChatId { get { return _credentials.GetChatId(); } } // Поменять на динамически-получаемый id

        private readonly TelegramCredentials _credentials;

        private static readonly HttpClient _httpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromHours(1)
        })
        {
            BaseAddress = new Uri(@$"https://api.telegram.org")
        };

        public CommandsHandler(TelegramCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task SendMessage(string text)
        {
            string apiEndpoint = $"/bot{BotToken}/sendMessage";

            var content = new StringContent($"chat_id={ChatId}&text={Uri.EscapeDataString(text)}", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);

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
