﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace telegram_lotte_bot.DTO
{
    public record Update
    {
        [JsonProperty("update_id")]
        public long Id { get; set; }

        [JsonProperty("message")]
        public Message? Message { get; set; }
    }
}
