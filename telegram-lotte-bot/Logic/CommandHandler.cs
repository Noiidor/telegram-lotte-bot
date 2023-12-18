using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using telegram_lotte_bot.DTO;

namespace telegram_lotte_bot.Logic
{
    public class CommandHandler
    {
        private readonly TelegramCredentials _credentials;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public CommandHandler(TelegramCredentials credentials, ILogger logger, HttpClient httpClient)
        {
            _credentials = credentials;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task HandleUpdates(List<Update> updates)
        {

        }
    }
}
