using Microsoft.Extensions.Hosting;
using telegram_lotte_bot.Application;
using telegram_lotte_bot.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using telegram_lotte_bot.Application.Telegram;
using Microsoft.Extensions.DependencyInjection;
using telegram_lotte_bot.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace telegram_lotte_bot.CLI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.AddUserSecrets("bf684e1b-a247-4665-b47f-2e87843a7b49");

            builder.Services.AddApplicationServices(builder.Logging);
            builder.Services.AddInfrastructureServices();

            var host = builder.Build();

            await Console.Out.WriteLineAsync("Started.");

            CancellationToken cancellationToken = new();
            var update = host.Services.GetRequiredService<IUpdateService>();
            await update.StartReceivingUpdates(cancellationToken);
        }
    }
}