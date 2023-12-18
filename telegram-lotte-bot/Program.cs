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

            CommandHandler commandsHandler = new(credentials, logger);

            UpdateHandler updateHandler = new(credentials, logger, commandsHandler);

            CancellationToken cancellationToken = new CancellationToken();

            await updateHandler.StartHandlingUpdates(cancellationToken);

        }
    }
}