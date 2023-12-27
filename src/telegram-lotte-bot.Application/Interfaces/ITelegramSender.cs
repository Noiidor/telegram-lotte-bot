using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_lotte_bot.Domain.Telegram;

namespace telegram_lotte_bot.Application.Interfaces
{
    public interface ITelegramSender
    {
        Task<Message?> SendMessage(long chatId, string text, long? replyToId);
        Task<Message?> EditMessage(long chatId, long messageId, string text);
    }
}
