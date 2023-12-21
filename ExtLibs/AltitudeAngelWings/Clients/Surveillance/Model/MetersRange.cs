using System.Text.Json.Serialization;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class MetersRange
    {
        /// <summary>
        ///  The minimum number of metres where the detection was seen.
        /// </summary>
        [JsonPropertyName("min")]
        public double Min { get; set; }

        /// <summary>
        ///  The maximum number of metres where the detection was seen.
        /// </summary>
        [JsonPropertyName("max")]
        public double Max { get; set; }

        /// <summary>
        ///  The rate at which the detection was seen moving in metres per second. Towards the sensor is negative, away is postive.
        /// </summary>
        [JsonPropertyName("rate")]
        public double Rate { get; set; }
    }
}