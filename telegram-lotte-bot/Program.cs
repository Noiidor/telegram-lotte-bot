using Microsoft.Extensions.Logging;
using telegram_lotte_bot.Logic;

namespace telegram_lotte_bot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ILogger logger = LoggerConfiguration.CreateLogger();

            TelegramCredentials credentials = new();

            HttpClient httpClient = new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromHours(1)
            })
            {
                BaseAddress = new Uri(@$"https://api.telegram.org")
            };

            CommandHandler commandsHandler = new(credentials, logger, httpClient);

            UpdateHandler updateHandler = new(credentials, logger, httpClient, commandsHandler);

            CancellationToken cancellationToken = new CancellationToken();
            await updateHandler.StartHandlingUpdates(cancellationToken);

            logger.LogInformation("Started.");
        }
    }
}