using Newtonsoft.Json;

namespace Dot.Tools.Search.Models.Brave
{

    public class BraveWeb
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("results")]
        public List<BraveWebResult> Results { get; set; }

        [JsonProperty("family_friendly")]
        public bool FamilyFriendly { get; set; }
    }

    public class BraveWebResult
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("is_source_local")]
        public bool IsSourceLocal { get; set; }

        [JsonProperty("is_source_both")]
        public bool IsSourceBoth { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("page_age")]
        public string PageAge { get; set; }

        [JsonProperty("profile")]
        public BraveWebProfile Profile { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("family_friendly")]
        public bool FamilyFriendly { get; set; }

        [JsonProperty("type")]
        public string ResultType { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("is_live")]
        public bool IsLive { get; set; }

        [JsonProperty("meta_url")]
        public BraveMetaUrl MetaUrl { get; set; }

        [JsonProperty("thumbnail")]
        public BraveThumbnail Thumbnail { get; set; }

        [JsonProperty("age")]
        public string Age { get; set; }
    }

    public class BraveWebProfile
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("long_name")]
        public string LongName { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }
    }

}
