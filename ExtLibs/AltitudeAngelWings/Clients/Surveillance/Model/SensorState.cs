using System.Text.Json.Serialization;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class SensorState
    {
        /// <summary>
        /// The identifier of the sensor providing the positions.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Current pressure in mb at the sensor location.
        /// </summary>
        [JsonPropertyName("pressure")]
        public double? Pressure { get; set; }

        /// <summary>
        /// Provides up to date sensor position information if the sensor can move/rotate/etc.
        /// </summary>
        [JsonPropertyName("location")]
        public SensorLocation Location { get; set; }

        /// <summary>
        /// Provides any additional information about the current state of the sensor.
        /// </summary>
        [JsonPropertyName("additionalInfo")]
        public object AdditionalInfo { get; set; }
    }
}