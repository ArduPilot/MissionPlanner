using System.Text.Json.Serialization;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class GeographicPosition
    {
        /// <summary>
        /// The Latitude.
        /// </summary>
        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        /// <summary>
        /// The Longitude.
        /// </summary>
        [JsonPropertyName("lon")]
        public double? Lon { get; set; }

        /// <summary>
        /// Accuracy of the position in meters. If not specified, accuracy is assumed to be the accuracy of the lat/long provided.
        /// </summary>
        [JsonPropertyName("accuracy")]
        public double? Accuracy { get; set; }

        /// <summary>
        /// Indicates the source of the position data (e.g GPS).
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