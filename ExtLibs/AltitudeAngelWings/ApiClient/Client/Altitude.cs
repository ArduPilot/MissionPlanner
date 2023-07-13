using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
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