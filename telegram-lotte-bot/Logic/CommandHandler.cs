using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using telegram_lotte_bot.DTO;

namespace telegram_lotte_bot.Logic
{
    public class CommandHandler
    {
        private readonly ILogger _logger;
        private readonly BotInteractionManager _interactionManager;

        public CommandHandler(ILogger logger, BotInteractionManager interactionManager)
        {
            _logger = logger;
            _interactionManager = interactionManager;
        }

        public async Task CheckUpdates(List<MessageUpdate> updates)
        {
            foreach (var update in updates)
            {
                switch (update.Message?.Text)
                {
                    case string text when text.Contains("/hi"):
                        await HandleHiCommand(update.Message.Chat.Id, update.Message.Id);
                        break;

                    case string text when text.Contains("/countchars"):
                        await HandleCountCharactersCommand(update.Message.Chat.Id, update.Message.Id, text);
                        break;

                    default:
                        break;
                }
            }
        }


        private async Task HandleHiCommand(long chatId, long replyId)
        {
            await _interactionManager.SendMessage(chatId, "Hi", replyId);
        }

        private async Task HandleCountCharactersCommand(long chatId, long replyId, string text)
        {

            await _interactionManager.SendMessage(chatId, $"Количество символов: {text.Length}", replyId);

        }
    }
}
