using Newtonsoft.Json.Linq;
using NodaTime;

namespace AltitudeAngelWings.ApiClient.Models.Strategic
{
    public class CreateStrategicPlanPartRequest
    {
        /// <summary>
        /// Gets or sets an identifier that uniquely identifies this part within this flight.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a Polygon or LineString describing the planned operating area or route.
        /// </summary>
        public JObject Geography { get; set; }

        /// <summary>
        /// Gets or sets the instant this flight part is expected to start.
        /// </summary>
        public Instant? Start { get; set; }

        /// <summary>
        /// Gets or sets the instant this flight part is expected to be completed by.
        /// </summary>
        public Instant? End { get; set; }

        /// <summary>
        /// Gets or sets the maximum altitude that the drone will achieve during the flightPart.
        /// </summary>
        public Altitude MaxAltitude { get; set; }
    }
}
