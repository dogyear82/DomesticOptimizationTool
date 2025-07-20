using Newtonsoft.Json;

namespace Dot.Tools.Search.Models.Brave
{
    public class BraveMixed
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("main")]
        public List<MixedItem> Main { get; set; }

        [JsonProperty("top")]
        public List<object> Top { get; set; }

        [JsonProperty("side")]
        public List<object> Side { get; set; }
    }

    public class MixedItem
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("all")]
        public bool All { get; set; }
    }
}
