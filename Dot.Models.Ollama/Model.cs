using Newtonsoft.Json;

namespace Dot.Models.Ollama
{
    public class TagResponse
    {
        [JsonProperty("models")]
        public List<Model> Models { get; set; }
    }

    public class Model
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("modified_at")]
        public DateTime ModifiedAt { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }
}
