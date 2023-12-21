using System.Text.Json.Serialization;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class SurveillanceReport
    {
        /// <summary>
        ///     Provides information about the sensor’s current state.
        /// </summary>
        [JsonPropertyName("sensor")]
        public SensorState Sensor { get; set; }

        /// <summary>
        ///     List of all aircraft positions.
        /// </summary>
        [JsonPropertyName("positions")]
        public PositionData[] Positions { get; set; }

        /// <summary>
        ///     List of all aircraft detections.
        /// </summary>
        [JsonPropertyName("detections")]
        public DetectionData[] Detections { get; set; }

        /// <summary>
        ///     Tags that apply to this position report.
        /// </summary>
        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }
    }
}
