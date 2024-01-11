using Microsoft.Extensions.Configuration;
using telegram_lotte_bot.Application.Interfaces;

namespace telegram_lotte_bot.Application
{
    public class UserSecretsHelper : ICredentialsManager
    {
        private readonly IConfiguration configuration;

        public UserSecretsHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetBotToken()
        {
//#if DEBUG
            return configuration.GetSection("BotToken").Value;
//#else
            //ret
//#endif
        }

        public string GetAuthString()
        {
            return configuration.GetSection("LotteAuth").Value;
        }
    }
}
