using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class AltitudeProperty
    {
        [JsonProperty("datum")]
        public string Datum { get; set; }
        [JsonProperty("meters")]
        public float Meters { get; set; }
        // Ignore feet
    }
}