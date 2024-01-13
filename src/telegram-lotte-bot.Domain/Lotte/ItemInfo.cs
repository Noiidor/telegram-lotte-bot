using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_lotte_bot.Domain.Lotte
{
    public record ItemInfo
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("sku")]
        public required string Sku { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("price")]
        public required Price Price { get; set; }

        public ItemInfo()
        { }
    }

    public record Price
    {
        [JsonProperty("VND")]
        public required PriceVnd Vnd { get; set; }
    }

    public record PriceVnd
    {
        [JsonProperty("default")]
        public int CurrentPrice { get; set; }

        [JsonProperty("price")]
        public int PriceOriginal { get; set; }
    }
}
