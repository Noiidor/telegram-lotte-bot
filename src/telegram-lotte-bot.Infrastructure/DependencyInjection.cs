using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using telegram_lotte_bot.Application.Interfaces;
using telegram_lotte_bot.Infrastructure.Lotte;
using telegram_lotte_bot.Infrastructure.Telegram;

namespace telegram_lotte_bot.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ILotteClient, LotteClient>();
            services.AddScoped<IUpdateHandler, UpdateHandler>();
            services.AddScoped<ITelegramSender, TelegramSender>();

            services.AddHttpClient("LotteClient", (serviceProvider, client) =>
            {
                client.DefaultRequestHeaders.Add("Authorization", serviceProvider.GetRequiredService<ICredentialsManager>().GetAuthString());
                client.BaseAddress = new Uri("https://www.lottemart.vn");
            });

            services.AddHttpClient("TelegramClient", client =>
            {
                client.BaseAddress = new Uri("https://api.telegram.org");
            });

            return services;
        }
    }
}
