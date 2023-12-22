using System;
using System.Text.Json.Serialization;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class DetectionData
    {
        /// <summary>
        ///  The ID of this detection report.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        ///  The target ID of this detection report.
        /// </summary>
        [JsonPropertyName("targetId")]
        public string TargetId { get; set; }

        /// <summary>
        ///  When the detection was made, in UTC.
        /// </summary>
        [JsonPropertyName("sourceTimeStamp")]
        public DateTime SourceTimestamp { get; set; }

        /// <summary>
        ///  The bearing of the detection, relative to the sensor.
        /// </summary>
        [JsonPropertyName("bearing")]
        public BearingRange Bearing { get; set; }

        /// <summary>
        ///  The distance between the detection and the sensor.
        /// </summary>
        [JsonPropertyName("distance")]
        public MetersRange Distance { get; set; }

        /// <summary>
        ///  The elevation of the detection, relative to the sensor.
        /// </summary>
        [JsonPropertyName("elevation")]
        public ElevationRange Elevation { get; set; }

        /// <summary>
        ///  Additional information about the detection.
        /// </summary>
        [JsonPropertyName("additionalInfo")]
        public object AdditionalInfo { get; set; }
    }
}