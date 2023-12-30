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

        private static readonly int SKU_ID_LENGHT = 13;

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
                await _telegramSender.SendMessage(message.Chat.Id, "Используйте команду ответом на сообщение со списком.", message.Id);
                return;
            }

            string messageText = message.ReplyTo.Text;

            string pattern = @$"(?<=-)\d{{{SKU_ID_LENGHT}}}(?=-)|((?<=\s)\d)";

            MatchCollection matches = Regex.Matches(messageText, pattern);

            if (matches[0].Groups.Count < 2 || matches.Count < 1) // Матчи делятся на 2 группы: айдишники и количество
            {
                await _telegramSender.SendMessage(message.Chat.Id, "Не удалось обработать сообщение.", message.Id);
                return;
            }

            int successItemsUnique = 0, succesItemsTotal = 0;
            int failedItemsUnique = 0, faileditemsTotal = 0;

            Message? inProcessMessage = await _telegramSender.SendMessage(message.Chat.Id, "Добавляю...", message.Id);

            List<Task> requests = new();

            for (int i = 0; i < matches.Count; i++)
            {
                string itemIdRaw = matches[i].Groups[0].Value;
                if (long.TryParse(itemIdRaw, out long itemId))
                {
                    string? itemQuantityRaw = matches.Count > i + 1 ? matches[i + 1].Groups[1].Value : null;

                    // Если следующий match на количество отпарсился - пропускается из следующей итерации
                    if (int.TryParse(itemQuantityRaw, out int itemQuantity)) i++;
                    else itemQuantity = 1;

                    Task addRequest = Task.Run(async () =>
                    {
                        bool success = await _lotteClient.AddToCart(itemId, itemQuantity);

                        if (success)
                        {
                            successItemsUnique++;
                            succesItemsTotal += itemQuantity;
                        }
                        else
                        {
                            failedItemsUnique++;
                            faileditemsTotal += itemQuantity;
                        }
                    });
                    requests.Add(addRequest);
                }
            }

            await Task.WhenAll(requests);

            string resultMessage = $"Успешно добавлено {successItemsUnique} товаров({succesItemsTotal} позиций).";
            resultMessage += $"Не удалось добавить {failedItemsUnique} товаров({faileditemsTotal} позиций).";

            if (inProcessMessage != null)
            {
                await _telegramSender.EditMessage(inProcessMessage.Chat.Id, inProcessMessage.Id, resultMessage);
            }
            else
            {
                await _telegramSender.SendMessage(message.Chat.Id, resultMessage, message.Id);
            }
        }

        private async Task HandleAddItemCommand(Message message)
        {
            string messageText = message.Text;

            long itemId;
            int itemQuantity;

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
