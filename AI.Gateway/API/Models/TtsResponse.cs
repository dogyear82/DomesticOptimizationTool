using Newtonsoft.Json;

namespace AI.Gateway.API.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]

    public class TtsResponse
    {
        [JsonProperty("aiRecord")]
        public AiRecord AiRecord { get; set; }
    }
}
