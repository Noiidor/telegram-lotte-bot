using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_lotte_bot
{
    internal static class LoggerConfiguration
    {
        public static ILogger CreateLogger()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            return loggerFactory.CreateLogger("MainLogger");
        }
    }
}
