using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class Altitude
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("datum")]
        public AltitudeDatum Datum { get; set; }

        [JsonProperty("meters")]
        public double Meters { get; set; }

    }
}
