using System.Text.Json.Serialization;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class BearingRange
    {
        /// <summary>
        ///  The minimum number of decimal degrees horizontal bearing where the detection was seen. 0 is North.
        /// </summary>
        [JsonPropertyName("min")]
        public double Min { get; set; }

        /// <summary>
        ///  The maximum number of decimal degrees horizontal bearing where the detection was seen. 0 is North.
        /// </summary>
        [JsonPropertyName("max")]
        public double Max { get; set; }

        /// <summary>
        ///  The rate at which the detection was seen moving horizontally, in decimal degrees per second. Clockwise is positive, anti-clockwise is negative.
        /// </summary>
        [JsonPropertyName("rate")]
        public double Rate { get; set; }
    }
}