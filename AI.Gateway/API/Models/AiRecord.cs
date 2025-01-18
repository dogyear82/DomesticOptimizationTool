using Newtonsoft.Json;

namespace AI.Gateway.API.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AiRecord
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("teamId")]
        public string TeamId { get; set; }

        [JsonProperty("teamUser")]
        public TeamUser TeamUser { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("metadata")]
        public object Metadata { get; set; }

        [JsonProperty("rating")]
        public object Rating { get; set; }

        [JsonProperty("feedback")]
        public object Feedback { get; set; }

        [JsonProperty("conversationId")]
        public object ConversationId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("aiRecordDetail")]
        public AiRecordDetail AiRecordDetail { get; set; }

        [JsonProperty("additionalData")]
        public object AdditionalData { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AiRecordDetail
    {
        [JsonProperty("promptObject")]
        public TtsPrompt PromptObject { get; set; }

        [JsonProperty("resultObject")]
        public string[] ResultObject { get; set; }
    }
}
