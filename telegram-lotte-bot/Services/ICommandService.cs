using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_lotte_bot.DTO.Telegram;

namespace telegram_lotte_bot.Services
{
    public interface ICommandService
    {
        Task CheckUpdates(List<MessageUpdate> updates);
    }
}
