using System.Text.Json.Serialization;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class GeographicVector
    {
        /// <summary>
        /// East-West part of the vector.
        /// </summary>
        [JsonPropertyName("x")]
        public double? X { get; set; }

        /// <summary>
        /// North-South part of the vector.
        /// </summary>
        [JsonPropertyName("y")]
        public double? Y { get; set; }

        /// <summary>
        /// Up-Down part of the vector.
        /// </summary>
        [JsonPropertyName("z")]
        public double? Z { get; set; }

        /// <summary>
        /// Accuracy of x/y part.
        /// </summary>
        [JsonPropertyName("horizontalAccuracy")]
        public double? HorizontalAccuracy { get; set; }

        /// <summary>
        /// Accuracy of z part.
        /// </summary>
        [JsonPropertyName("verticalAccuracy")]
        public double? VerticalAccuracy { get; set; }

        /// <summary>
        /// The source of the vector information.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        /// <summary>
        /// The age in seconds since this data was acquired.
        /// </summary>
        [JsonPropertyName("age")]
        public double? Age { get; set; }
    }
}