using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using telegram_lotte_bot.Domain.Telegram;
using telegram_lotte_bot.Application.Interfaces;
using telegram_lotte_bot.Domain.Lotte;

namespace telegram_lotte_bot.Application.Telegram
{
    public class CommandService : ICommandService
    {
        private readonly ILogger<CommandService> _logger;
        private readonly ITelegramSender _telegramSender;
        private readonly ILotteClient _lotteClient;

        private static readonly int SKU_ID_LENGHT = 13;
        private static readonly char CURRENCY_SYMBOL = '₫';

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

        private Dictionary<string, int>? ParseItemsQuantityList(string message)
        {
            string idAndQuantityPattern = @$"(?<=-)\d{{{SKU_ID_LENGHT}}}(?=-)|((?<=\s)\d)";

            MatchCollection idAndQuantityMatches = Regex.Matches(message, idAndQuantityPattern);

            if (idAndQuantityMatches[0].Groups.Count < 2 || idAndQuantityMatches.Count < 1) // Матчи делятся на 2 группы: айдишники и количество
            {
                return null;
            }

            Dictionary<string, int> items = new();
            for (int i = 0, j = 0; i < idAndQuantityMatches.Count; i++)
            {
                if (i != 0) j++;

                string itemId = idAndQuantityMatches[i].Groups[0].Value;

                string? itemQuantityRaw = idAndQuantityMatches.Count > i + 1 ? idAndQuantityMatches[i + 1].Groups[1].Value : null;

                // Если следующий match на количество отпарсился - пропускается из следующей итерации
                if (int.TryParse(itemQuantityRaw, out int itemQuantity))
                {
                    i++;
                }
                else itemQuantity = 1;

                items.Add(itemId, itemQuantity);
            }

            return items;
        }

        private async Task HandleAddListCommand(Message message)
        {
            if (message.ReplyTo == null)
            {
                await _telegramSender.SendMessage(message.Chat.Id, "Use command with reply on list message.", message.Id);
                return;
            }

            string messageText = message.ReplyTo.Text;

            //string urlKeysPattern = @"(?<=product\/)[^\s]+";
            //MatchCollection urlKeysMatches = Regex.Matches(messageText, urlKeysPattern);

            Dictionary<string, int>? itemsQuantity = ParseItemsQuantityList(messageText);
            List<string>? itemUrls = ParseItemUrls(messageText);
            
            if (itemsQuantity == null || itemUrls == null || itemsQuantity.Count != itemUrls.Count)
            {
                await _telegramSender.SendMessage(message.Chat.Id, "Failed to process the message.", message.Id);
                return;
            }

            int successItemsUnique = 0, succesItemsTotal = 0;
            int failedItemsUnique = 0, faileditemsTotal = 0;
            //int totalPrice = 0;
            List<string> failedItems = new();

            Message? inProcessMessage = await _telegramSender.SendMessage(message.Chat.Id, "Adding...", message.Id);

            List<Task> requests = new();

            for (int i = 0; i < itemsQuantity.Count; i++)
            {
                string itemId = itemsQuantity.ElementAt(i).Key;
                int itemQuantity = itemsQuantity.ElementAt(i).Value;

                async Task addToCart(int currentIndex)
                {
                    bool success = await _lotteClient.AddToCart(itemId, itemQuantity);

                    if (success)
                    {
                        successItemsUnique++;
                        succesItemsTotal += itemQuantity;

                        //ItemInfo? itemInfo = await _lotteClient.GetItemInfo(urlKeysMatches[j].Value);
                        //if (itemInfo != null)
                        //{
                        //    totalPrice += itemInfo.Price.Vnd.CurrentPrice * itemQuantity;
                        //}
                    }
                    else
                    {
                        failedItems.Add(itemUrls[currentIndex]);
                        failedItemsUnique++;
                        faileditemsTotal += itemQuantity;
                    }
                }

                requests.Add(addToCart(i));
            }

            await Task.WhenAll(requests);

            string resultMessage = $"Successfully added {successItemsUnique} items({succesItemsTotal} in total).\n";
            //resultMessage += $"Total price is {totalPrice:N0} {CURRENCY_SYMBOL}.\n";
            if (failedItemsUnique > 0) 
            {
                resultMessage += $"Failed to add {failedItemsUnique} items({faileditemsTotal} in total): \n";
                resultMessage += string.Join('\n', failedItems);
            }
                
            if (inProcessMessage != null)
            {
                await _telegramSender.EditMessage(inProcessMessage.Chat.Id, inProcessMessage.Id, resultMessage);
            }
            else
            {
                await _telegramSender.SendMessage(message.Chat.Id, resultMessage, message.Id);
            }
        }

        private List<string>? ParseItemUrls(string message)
        {
            string itemUrlPattern = @$"http\S+";

            MatchCollection itemUrlsMatches = Regex.Matches(message, itemUrlPattern);

            if (itemUrlsMatches.Count < 1)
            {
                return null;
            }

            List<string> urls = itemUrlsMatches.Select(m => m.Value).ToList();

            return urls;
        }

        private async Task HandleAddItemCommand(Message message)
        {
            string messageText = message.Text;

            string itemId;
            int itemQuantity;

            bool isUrl = messageText.Contains("http");

            if (isUrl)
            {
                string pattern = @"-\d*-";

                Match match = Regex.Match(messageText, pattern);

                if (!match.Success) goto invalidFormat; // Нестареющая классика

                itemId = match.Value.Replace("-", null);

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

                itemId = matches[0].Value;

                if (!int.TryParse(matches[1].Value, out itemQuantity)) goto invalidFormat;
            }

            bool postStatus = await _lotteClient.AddToCart(itemId, itemQuantity);

            if (postStatus)
            {
                await _telegramSender.SendMessage(message.Chat.Id, "Added to cart.", message.Id);
                return;
            }
            else
            {
                await _telegramSender.SendMessage(message.Chat.Id, "Failed to add to cart.", message.Id);
                return;
            }

        invalidFormat:
            await _telegramSender.SendMessage(message.Chat.Id, "Invalid command format.", message.Id);
            return;
        }
    }
}
