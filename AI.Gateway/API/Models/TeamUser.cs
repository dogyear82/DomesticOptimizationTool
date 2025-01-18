using Newtonsoft.Json;

namespace AI.Gateway.API.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]    public class TeamUser
    {
        [JsonProperty("teamId")]
        public string TeamId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("userAvatar")]
        public object UserAvatar { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("creditLimit")]
        public long CreditLimit { get; set; }

        [JsonProperty("usedCredit")]
        public int UsedCredit { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }
    }
}
