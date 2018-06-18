using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class BoundingLatLong
    {
        [JsonProperty(PropertyName = "sw")]
        public LatLong SouthWest { get; set; }

        [JsonProperty(PropertyName = "ne")]
        public LatLong NorthEast { get; set; }
    }
}
