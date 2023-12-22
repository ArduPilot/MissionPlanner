using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class ExcludedDataDetail
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
    }
}