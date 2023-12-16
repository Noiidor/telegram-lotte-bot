using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace telegram_lotte_bot
{
    public class TelegramCredentials
    {
        private IConfiguration configuration;

        public TelegramCredentials()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets("bf684e1b-a247-4665-b47f-2e87843a7b49");

            configuration = builder.Build();
        }

        public string GetBotToken()
        {
            return configuration.GetSection("BotToken").Value;
        }

        public string GetChatId()
        {
            return configuration.GetSection("ChatId").Value;
        }
    }
}
