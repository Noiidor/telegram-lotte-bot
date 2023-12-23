using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using telegram_lotte_bot.Domain.Telegram;
using telegram_lotte_bot.Application.Interfaces;

namespace telegram_lotte_bot.Application.Telegram
{
    public class CommandService : ICommandService
    {
        private readonly ILogger<CommandService> _logger;
        private readonly ITelegramSender _telegramSender;
        private readonly ILotteClient _lotteClient;

        public CommandService(ILogger<CommandService> logger, ITelegramSender telegramSender, ILotteClient lotteClient)
        {
            _logger = logger;
            _telegramSender = telegramSender;
            _lotteClient = lotteClient;
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
            await _telegramSender.SendMessage(chatId, "Hi", replyId);
        }

        private async Task HandleCountCharactersCommand(long chatId, long replyId, string text)
        {

            await _telegramSender.SendMessage(chatId, $"Количество символов: {text.Length}", replyId);

        }

        private async Task HandleAddItemCommand(long chatId, long replyId, string text)
        {
            long itemId = 0;
            int itemQuantity = 0;

            bool isUrl = text.Contains("http");

            if (isUrl)
            {
                string pattern = @"-\d*-";

                Match match = Regex.Match(text, pattern);

                if (!match.Success) goto invalidFormat; // Нестареющая классика

                string idStr = match.Value.Replace("-", null);

                if (!long.TryParse(idStr, out itemId)) goto invalidFormat;

                pattern = @"\s\d";

                match = Regex.Match(text, pattern);

                if (!match.Success) goto invalidFormat;

                if (!int.TryParse(match.Value, out itemQuantity)) goto invalidFormat;
            }
            else
            {
                string pattern = "\\d+";

                MatchCollection? matches = Regex.Matches(text, pattern);

                if (matches == null || matches.Count < 2) goto invalidFormat; 

                if (!long.TryParse(matches[0].Value, out itemId)) goto invalidFormat;
                if (!int.TryParse(matches[1].Value, out itemQuantity)) goto invalidFormat;
            }

            bool postStatus = await _lotteClient.AddToCart(itemId, itemQuantity);

            if (postStatus)
            {
                await _telegramSender.SendMessage(chatId, "Добавлено в корзину.", replyId);
                return;
            }
            else
            {
                await _telegramSender.SendMessage(chatId, "Не удалось добавить в корзину.", replyId);
                return;
            }

        invalidFormat:
            await _telegramSender.SendMessage(chatId, "Неправильный формат команды.", replyId);
            return;
        }
    }
}
