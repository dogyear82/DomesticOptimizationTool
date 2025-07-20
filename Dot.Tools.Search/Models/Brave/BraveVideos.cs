using Newtonsoft.Json;

namespace Dot.Tools.Search.Models.Brave
{
    public class BraveVideos
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("results")]
        public List<BraveVideoResult> Results { get; set; }

        [JsonProperty("mutated_by_goggles")]
        public bool MutatedByGoggles { get; set; }
    }

    public class BraveVideoResult
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("age")]
        public string Age { get; set; }

        [JsonProperty("page_age")]
        public string PageAge { get; set; }

        [JsonProperty("video")]
        public BraveVideoDetails Video { get; set; }

        [JsonProperty("meta_url")]
        public BraveMetaUrl MetaUrl { get; set; }

        [JsonProperty("thumbnail")]
        public BraveThumbnail Thumbnail { get; set; }
    }

    public class BraveVideoDetails
    {
        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("views")]
        public int Views { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }
    }

    public class BraveMetaUrl
    {
        [JsonProperty("scheme")]
        public string Scheme { get; set; }

        [JsonProperty("netloc")]
        public string Netloc { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("favicon")]
        public string Favicon { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }

    public class BraveThumbnail
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("original")]
        public string Original { get; set; }
    }
}
