using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using telegram_lotte_bot.Domain.Telegram;
using telegram_lotte_bot.Application.Interfaces;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace telegram_lotte_bot.Application.Telegram
{
    public class CommandService : ICommandService
    {
        private readonly ILogger<CommandService> _logger;
        private readonly ITelegramSender _telegramSender;
        private readonly ILotteClient _lotteClient;

        private readonly Dictionary<string, Func<Message, Task>> _commands;

        public CommandService(ILogger<CommandService> logger, ITelegramSender telegramSender, ILotteClient lotteClient)
        {
            _logger = logger;
            _telegramSender = telegramSender;
            _lotteClient = lotteClient;

            _commands = new Dictionary<string, Func<Message, Task>>()
            {
                {"/hi", HandleHiCommand },
                {"/add", HandleAddItemCommand },
                {"/addlist", HandleAddListCommand }
            };
        }

        public async Task CheckUpdates(List<MessageUpdate> updates)
        {
            foreach (var update in updates)
            {
                var message = update.Message;
                if (message == null) continue;

                if (message.Entities.Count == 0 || message.Entities[0].Type != "bot_command") continue;

                string command = Regex.Replace(message.Text, "@.*$", string.Empty);

                foreach (var commandKeyPair in _commands)
                {
                    if (commandKeyPair.Key.Contains(command))
                    {
                        await commandKeyPair.Value.Invoke(message);
                    }
                }
            }
        }


        private async Task HandleHiCommand(Message message)
        {
            await _telegramSender.SendMessage(message.Chat.Id, "Hi", message.Id);
        }

        private async Task HandleAddListCommand(Message message)
        {
            if (message.ReplyTo == null)
            {
                await _telegramSender.SendMessage(message.Chat.Id, "Используйте команду ответом на список.", message.Id);
                return;
            }

            string messageText = message.ReplyTo.Text;

            string pattern = @"(?<=-)\d+(?=-)|((?<=\s)\d)";

            MatchCollection matches = Regex.Matches(messageText, pattern);

            if (matches[0].Groups.Count < 2) // Матчи делятся на 2 группы: айдишники и количество
            {
                await _telegramSender.SendMessage(message.Chat.Id, "Не удалось обработать сообщение.", message.Id);
                return;
            }

            for (int i = 0; i < matches.Count; i++)
            {
                Match matchItemId = matches[i];
                if (long.TryParse(matchItemId.Groups[0].Value, out long itemId))
                {
                    int itemQuantity = 1;

                    string? itemQuantityRaw = matches.Count > i + 1 ? matches[i + 1].Groups[1].Value : null;

                    // Если следующий match на количество отпарсился - пропускается из следующей итерации
                    if (int.TryParse(itemQuantityRaw, out itemQuantity)) i++;

                    await _lotteClient.AddToCart(itemId, itemQuantity);
                }
            }

            await _telegramSender.SendMessage(message.Chat.Id, "Список успешно добавлен в корзину.", message.Id);
        }

        private async Task HandleAddItemCommand(Message message)
        {
            string messageText = message.Text;

            long itemId = 0;
            int itemQuantity = 0;

            bool isUrl = messageText.Contains("http");

            if (isUrl)
            {
                string pattern = @"-\d*-";

                Match match = Regex.Match(messageText, pattern);

                if (!match.Success) goto invalidFormat; // Нестареющая классика

                string idStr = match.Value.Replace("-", null);

                if (!long.TryParse(idStr, out itemId)) goto invalidFormat;

                pattern = @"\s\d";

                match = Regex.Match(messageText, pattern);

                if (!match.Success) goto invalidFormat;

                if (!int.TryParse(match.Value, out itemQuantity)) goto invalidFormat;
            }
            else
            {
                string pattern = "\\d+";

                MatchCollection? matches = Regex.Matches(messageText, pattern);

                if (matches == null || matches.Count < 2) goto invalidFormat; 

                if (!long.TryParse(matches[0].Value, out itemId)) goto invalidFormat;
                if (!int.TryParse(matches[1].Value, out itemQuantity)) goto invalidFormat;
            }

            bool postStatus = await _lotteClient.AddToCart(itemId, itemQuantity);

            if (postStatus)
            {
                await _telegramSender.SendMessage(message.Chat.Id, "Добавлено в корзину.", message.Id);
                return;
            }
            else
            {
                await _telegramSender.SendMessage(message.Chat.Id, "Не удалось добавить в корзину.", message.Id);
                return;
            }

        invalidFormat:
            await _telegramSender.SendMessage(message.Chat.Id, "Неправильный формат команды.", message.Id);
            return;
        }
    }
}
