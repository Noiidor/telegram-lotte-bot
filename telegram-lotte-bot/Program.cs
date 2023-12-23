using Microsoft.Extensions.Logging;
using telegram_lotte_bot.Logic;
using telegram_lotte_bot.Services;

namespace telegram_lotte_bot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ILogger logger = LoggerConfiguration.CreateLogger();

            UserSecretsHelper credentials = new();

            HttpClient httpClient = new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromHours(1)
            })
            {
                BaseAddress = new Uri(@$"https://api.telegram.org")
            };

            LotteApiService lotteService = new(logger, credentials);

            BotInteractionManager interactionManager = new(credentials, logger, httpClient);

            CommandService commandsHandler = new(logger, interactionManager, lotteService);

            UpdateHandler updateHandler = new(credentials, logger, httpClient, commandsHandler);

            logger.LogInformation("Started.");

            CancellationToken cancellationToken = new CancellationToken();
            await updateHandler.StartHandlingUpdates(cancellationToken);
        }
    }
}