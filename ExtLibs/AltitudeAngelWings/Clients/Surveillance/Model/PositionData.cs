using System;
using System.Text.Json.Serialization;
using AltitudeAngelWings.Clients.Flight.Model;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class PositionData
    {
        /// <summary>
        /// Uniquely identifies this specific position report by the sender.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Indicates when the position report was sent, according to the sensor clock, in UTC. (IAT should be accounted for by the sensor)
        /// </summary>
        [JsonPropertyName("sourceTimeStamp")]
        public DateTime? SourceTimeStamp { get; set; }

        /// <summary>
        /// Contains identification details of the detected object.
        /// </summary>
        [JsonPropertyName("target")]
        public Target Target { get; set; }

        /// <summary>
        /// Contains the position of the detected object.
        /// </summary>
        [JsonPropertyName("position")]
        public GeographicPosition Position { get; set; }

        /// <summary>
        /// Contains one or more detected altitudes for the object.
        /// </summary>
        [JsonPropertyName("altitudes")]
        public Altitude[] Altitudes { get; set; }

        /// <summary>
        /// True if the sensor considers the object to be on the ground.
        /// </summary>
        [JsonPropertyName("onGround")]
        public bool OnGround { get; set; }

        /// <summary>
        /// Contains data for the speed and track of the object.
        /// </summary>
        [JsonPropertyName("groundVelocity")]
        public GeographicVector GroundVelocity { get; set; }

        /// <summary>
        /// Contains information about the true airspeed (TAS) of the object.
        /// </summary>
        [JsonPropertyName("trueAirspeed")]
        public GeographicVector TrueAirSpeed { get; set; }

        /// <summary>
        /// Contains acceleration information for the object.
        /// </summary>
        [JsonPropertyName("acceleration")]
        public GeographicVector Acceleration { get; set; }

        /// <summary>
        /// Aircraft heading in degrees.
        /// </summary>
        [JsonPropertyName("heading")]
        public double Heading { get; set; }

        /// <summary>
        /// Provides any additional sensor-specific information.
        /// </summary>
        [JsonPropertyName("additionalInfo")]
        public object AdditionalInfo { get; set; }
    }
}