using Newtonsoft.Json;
using OllamaSharp.Models.Chat;

namespace Dot.Models
{
    public class Prompt
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("messages")]
        public List<ChatMessage> Messages { get; set; }
    }

    public class ChatMessage
    {
        [JsonProperty("conversation_id")]
        public string ConversationId { get; set; }

        [JsonProperty("role")]
        public ChatRole Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class LlmResponseChunk
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("message")]
        public ChatMessage Message { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }
    }
}
