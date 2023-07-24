using System.Text.Json.Serialization;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class ElevationRange
    {
        /// <summary>
        ///  The minimum number of decimal degrees veritcal azimuth where the detection was seen. 0 is the horizontal tangent to the WGS84 ellipsoid at the sensor location.
        /// </summary>
        [JsonPropertyName("min")]
        public double Min { get; set; }

        /// <summary>
        ///  The maximum number of decimal degrees veritcal azimuth where the detection was seen. Zero is the horizontal tangent to the WGS84 ellipsoid at the sensor location.
        /// </summary>
        [JsonPropertyName("max")]
        public double Max { get; set; }

        /// <summary>
        ///  The rate at which the detection was seen moving vertically in decimal degrees per second. Upward is positive, downward is negative.
        /// </summary>
        [JsonPropertyName("rate")]
        public double Rate { get; set; }
    }
}