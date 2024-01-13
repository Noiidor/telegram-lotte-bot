using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_lotte_bot.Domain.Lotte;

namespace telegram_lotte_bot.Application.Interfaces
{
    public interface ILotteClient
    {
        Task<bool> AddToCart(string itemId, int quantity);
        Task<ItemInfo?> GetItemInfo(string urlKey);
    }
}
