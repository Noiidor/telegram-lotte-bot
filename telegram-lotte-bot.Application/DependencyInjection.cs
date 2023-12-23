using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_lotte_bot.Application.Interfaces;
using telegram_lotte_bot.Application.Telegram;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace telegram_lotte_bot.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, ILoggingBuilder logging)
        {
            services.AddScoped<ICommandService, CommandService>();
            services.AddScoped<ICredentialsManager, UserSecretsHelper>();
            services.AddScoped<IUpdateService, UpdateService>();
            logging.AddConsole(options =>
            {
                options.TimestampFormat = "[HH:mm:ss]\n";
            });

            return services;
        }
    }
}
