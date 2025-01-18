
using Newtonsoft.Json;

namespace AI.Gateway.API.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TtsRequest
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("promptObject")]
        public TtsPrompt Prompt { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TtsPrompt
    {
        [JsonProperty("audioEncoding")]
        public string AudioEncoding { get; set; }

        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pitch")]
        public int Pitch { get; set; }

        [JsonProperty("speakingRate")]
        public double SpeakingRate { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("volumeGainDb")]
        public int VolumeGainDb { get; set; }
    }

}
