using Newtonsoft.Json;
using System;

namespace telegram_lotte_bot.DTO
{
    public record Message
    {
        [JsonProperty("message_id")]
        public long Id { get; set; }

        [JsonProperty("from")]
        public MessageFrom? From { get; set; }

        [JsonProperty("chat")]
        public required Chat Chat { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; } = null!;

        [JsonProperty("text")]
        public required string Text { get; set; }
    }

    public record Chat
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; } = null!;

        [JsonProperty("type")]
        public string Type { get; set; } = null!;
    }

    public record MessageFrom
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; } = null!;

        [JsonProperty("username")]
        public string Username { get; set; } = null!;
    }
}