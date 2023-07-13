using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class UtmDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("externalUrl")]
        public string ExternalUrl { get; set; }
    }
}