using Microsoft.Extensions.Logging;
using telegram_lotte_bot.Handlers;

namespace telegram_lotte_bot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ILogger logger = LoggerConfiguration.CreateLogger();

            logger.LogInformation("Started.");

            TelegramCredentials credentials = new();

            CommandsHandler commandsHandler = new(credentials, logger);

            await commandsHandler.SendMessage("test");
        }
    }
}