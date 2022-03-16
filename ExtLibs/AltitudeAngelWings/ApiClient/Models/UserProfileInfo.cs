using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class UserProfileInfo
    {
        [JsonProperty("aa_id")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string EmailAddress { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("is_pro", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPro { get; set; }

        [JsonProperty("unit_of_measure", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof (StringEnumConverter))]
        public UnitOfMeasure UnitOfMeasure { get; set; }
    }
}
