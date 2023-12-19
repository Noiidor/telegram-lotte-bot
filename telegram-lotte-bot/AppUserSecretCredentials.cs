using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace telegram_lotte_bot
{
    public class AppUserSecretCredentials
    {
        private IConfiguration configuration;

        public AppUserSecretCredentials()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets("bf684e1b-a247-4665-b47f-2e87843a7b49");

            configuration = builder.Build();
        }

        public string GetBotToken()
        {
            return configuration.GetSection("BotToken").Value;
        }

        public string GetAuthString()
        {
            return configuration.GetSection("LotteAuth").Value;
        }
    }
}
