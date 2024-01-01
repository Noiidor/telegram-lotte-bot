using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_lotte_bot.Domain.Lotte
{
    public record CartPost
    {
        [JsonProperty("cart_items")]
        public List<CartPostItem> CartItems { get; set; } = new();

        [JsonProperty("cart_type")]
        public string CartType { get; set; } = "mart";
    }

    public record CartPostItem
    {
        [JsonProperty("sku")]
        public required string Id { get; set; }

        [JsonProperty("qty")]
        public int Quantity { get; set; }

        [JsonProperty("is_ready_checkout")]
        public int ReadyCheckout { get; set; } = 1;
    }
}
