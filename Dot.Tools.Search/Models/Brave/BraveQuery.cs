using Newtonsoft.Json;

namespace Dot.Tools.Search.Models.Brave
{
    public class BraveQuery
    {
        [JsonProperty("original")]
        public string Original { get; set; }

        [JsonProperty("show_strict_warning")]
        public bool ShowStrictWarning { get; set; }

        [JsonProperty("is_navigational")]
        public bool IsNavigational { get; set; }

        [JsonProperty("is_news_breaking")]
        public bool IsNewsBreaking { get; set; }

        [JsonProperty("spellcheck_off")]
        public bool SpellcheckOff { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("bad_results")]
        public bool BadResults { get; set; }

        [JsonProperty("should_fallback")]
        public bool ShouldFallback { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("header_country")]
        public string HeaderCountry { get; set; }

        [JsonProperty("more_results_available")]
        public bool MoreResultsAvailable { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

}
