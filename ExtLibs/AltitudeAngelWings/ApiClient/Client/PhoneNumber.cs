using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class PhoneNumber
    {
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("extension")]
        public string Extension { get; set; }
    }
}