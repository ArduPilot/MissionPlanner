using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models.Strategic
{
    public class StrategicContactDetails
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("allowSmsContact")]
        public bool AllowSmsContact { get; set; }
    }
}
