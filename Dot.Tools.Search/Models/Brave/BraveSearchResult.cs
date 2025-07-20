using Newtonsoft.Json;
namespace Dot.Tools.Search.Models.Brave
{
    public class BraveSearchResult
    {
        [JsonProperty("search")]
        public BraveQuery Query { get; set; }

        [JsonProperty("mixed")]
        public BraveMixed Mixed { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("videos")]
        public BraveMixed Videos { get; set; }

        [JsonProperty("web")]
        public BraveWeb Web { get; set; }
    }
}
