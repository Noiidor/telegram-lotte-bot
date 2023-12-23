using telegram_lotte_bot.Domain.Telegram;

namespace telegram_lotte_bot.Application.Interfaces
{
    public interface IUpdateHandler
    {
        Task<List<MessageUpdate>> GetUpdates(long offset);
    }
}
