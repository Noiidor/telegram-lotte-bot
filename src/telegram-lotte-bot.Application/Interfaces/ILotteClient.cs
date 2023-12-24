﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_lotte_bot.Application.Interfaces
{
    public interface ILotteClient
    {
        Task<bool> AddToCart(long itemId, int quantity);
    }
}