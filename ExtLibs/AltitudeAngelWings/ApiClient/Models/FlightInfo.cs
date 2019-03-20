using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class FlightInfo
    {
        /// <summary>
        ///     The take off position. Either this or the bounds must be specified.
        /// </summary>
        [JsonProperty(PropertyName = "pos")]
        public LatLong Position { get; set; }

        /// <summary>
        ///     The expected area of the flight. Either this or the position must be specified.
        /// </summary>
        [JsonProperty(PropertyName = "bounds")]
        public BoundingLatLong Bounds { get; set; }

        /// <summary>
        ///     The altitude of the initial flight request with datum.
        /// </summary>
        [JsonProperty(PropertyName = "alt")]
        public Altitude Altitude { get; set; }

        /// <summary>
        ///     Is this flight intending to flight beyond visual range.
        /// </summary>
        public bool FlyingBvr { get; set; }

        /// <summary>
        ///     Is This a commercial flight.
        /// </summary>
        public bool IsCommercial { get; set; }

        /// <summary>
        ///     (optional) User id of the pilot, can only be specified if delegating.
        /// </summary>
        public string PilotId { get; set; }
    }
}
