using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_lotte_bot.Domain.Telegram;

namespace telegram_lotte_bot.Application.Interfaces
{
    public interface ICommandService
    {
        Task CheckUpdates(List<MessageUpdate> updates);
    }
}
