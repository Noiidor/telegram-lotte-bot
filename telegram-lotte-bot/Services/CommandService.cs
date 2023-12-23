using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using telegram_lotte_bot.DTO.Telegram;
using System.Text.RegularExpressions;
using telegram_lotte_bot.Logic;

namespace telegram_lotte_bot.Services
{
    public class CommandService : ICommandService
    {
        private readonly ILogger _logger;
        private readonly BotInteractionManager _interactionManager;
        private readonly LotteApiService _lotteService;

        public CommandService(ILogger logger, BotInteractionManager interactionManager, LotteApiService lotteService)
        {
            _logger = logger;
            _interactionManager = interactionManager;
            _lotteService = lotteService;
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

                    case string text when text.Contains("/addtocart"):
                        await HandleAddItemCommand(update.Message.Chat.Id, update.Message.Id, text);
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

        private async Task HandleAddItemCommand(long chatId, long replyId, string text)
        {
            long itemId = 0;
            int itemQuantity = 0;

            if (text.Contains("http"))
            {
                string pattern = "\\d+";

                MatchCollection? matches = Regex.Matches(text, pattern);

                if (matches == null || matches.Count < 2) goto invalidFormat; // Нестареющая классика

                if (!long.TryParse(matches[0].Value, out itemId)) goto invalidFormat;
                if (!int.TryParse(matches[1].Value, out itemQuantity)) goto invalidFormat;
            }
            else
            {

            }

            bool postStatus = await _lotteService.AddToCart(itemId, itemQuantity);

            if (postStatus)
            {
                await _interactionManager.SendMessage(chatId, "Добавлено в корзину.", replyId);
                return;
            }
            else
            {
                await _interactionManager.SendMessage(chatId, "Не удалось добавить в корзину.", replyId);
                return;
            }

        invalidFormat:
            await _interactionManager.SendMessage(chatId, "Неправильный формат команды.", replyId);
            return;
        }
    }
}
