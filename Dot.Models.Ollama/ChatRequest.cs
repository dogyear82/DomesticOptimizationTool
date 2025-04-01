using Microsoft.Extensions.AI;
using Newtonsoft.Json;

namespace Dot.Models.Ollama
{
    public class ChatRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("stream")]
        public bool Stream { get; set; }

        [JsonProperty("messages")]
        public List<Message> Messages { get; set; }
    }

    public class Message
    {
        public Message(ChatMessage message)
        {
            Role = message.Role.ToString().ToLower();
            Content = message.Text;
        }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
