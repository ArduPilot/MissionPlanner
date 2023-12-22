using System.Text.Json.Serialization;
using AltitudeAngelWings.Clients.Flight.Model;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class SensorLocation
    {
        /// <summary>
        /// The position of the sensor providing the positions.
        /// </summary>
        [JsonPropertyName("position")]
        public GeographicPosition Position { get; set; }

        /// <summary>
        /// The altitude of the sensor. For ground-based sensors, altitude datum should be MSL.
        /// </summary>
        [JsonPropertyName("altitude")]
        public Altitude Altitude { get; set; }

        /// <summary>
        /// For directed sensors, the direction it is facing in degrees from north.
        /// </summary>
        [JsonPropertyName("heading")]
        public double Heading { get; set; }

        /// <summary>
        /// For directed sensors, the angle of the sensor in degrees from horizontal.
        /// </summary>
        [JsonPropertyName("angle")]
        public double Angle { get; set; }

    }
}