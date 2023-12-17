using telegram_lotte_bot.Handlers;

namespace telegram_lotte_bot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Started.");

            TelegramCredentials credentials = new();
            CommandsHandler commandsHandler = new(credentials);

            await commandsHandler.SendMessage("test");
        }
    }
}