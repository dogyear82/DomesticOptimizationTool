using Newtonsoft.Json;

namespace Dot.Models.LocalAPI
{
    public class LocalChatRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("messages")]
        public List<LocalChatMessage> Messages { get; set; }
    }

    public class LocalChatMessage
    {
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class LocalChatResponseChunk
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("message")]
        public LocalChatMessage Message { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }
    }
}
