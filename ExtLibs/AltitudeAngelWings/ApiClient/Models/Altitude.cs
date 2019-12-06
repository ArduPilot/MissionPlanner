using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class Altitude
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public AltitudeDatum Datum { get; set; }
        public double Meters { get; set; }

    }
}
