using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
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