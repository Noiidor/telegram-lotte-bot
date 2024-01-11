using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_lotte_bot.Domain.Telegram
{
    public record MessageResult
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("result")]
        public required Message Message { get; set; }

        public MessageResult()
        { }
    }
}
