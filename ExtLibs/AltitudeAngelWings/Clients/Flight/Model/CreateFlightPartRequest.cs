using System.Collections.Generic;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using NodaTime;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class CreateFlightPartRequest
    {
        /// <summary>
        /// Gets or sets an identifier that uniquely identifies this part within this flight.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a Polygon or LineString describing the planned operating area or route.
        /// </summary>
        [JsonProperty("geography")]
        public Feature Geography { get; set; }

        /// <summary>
        /// Gets or sets the instant this flight part is expected to start.
        /// </summary>
        [JsonProperty("start")]
        public Instant? Start { get; set; }

        /// <summary>
        /// Gets or sets the instant this flight part is expected to be completed by.
        /// </summary>
        [JsonProperty("end")]
        public Instant? End { get; set; }

        /// <summary>
        /// Gets or sets the preferred time zone for orchestrating the flight part.
        /// </summary>
        [JsonProperty("timeZone")]
        public DateTimeZone TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the maximum altitude that the drone will achieve during the flightPart.
        /// </summary>
        [JsonProperty("maxAltitude")]
        public Altitude MaxAltitude { get; set; }

        /// <summary>
        /// Gets or sets the maximum altitude that the drone will achieve during the flightPart.
        /// </summary>
        [JsonProperty("minAltitude")]
        public Altitude MinAltitude { get; set; }

        /// <summary>
        /// Gets or sets the list of approval options.
        /// </summary>
        [JsonProperty("approvalOptions")]
        public List<ApprovalOptionReference> ApprovalOptions { get; set; }

        /// <summary>
        /// Gets or sets whether Take-Off and landing is at same location or at different locations.
        /// </summary>
        [JsonProperty("takeoffAndLandingIsAtSameLocation")]
        public bool? TakeoffAndLandingIsAtSameLocation { get; set; }
    }
}
