using Newtonsoft.Json;

namespace Dot.Models.Ollama
{
    public class OllamaResponse
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("message")]
        public MessageContent Message { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }
    }

    public class MessageContent
    {
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
