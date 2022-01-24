using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using NodaTime;
using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class FlightPart
    {
        [JsonProperty("geography")]
        public Feature Geography
        {
            get; set;
        }

        [JsonProperty("start")]
        public Instant? Start
        {
            get; set;
        }

        [JsonProperty("end")]
        public Instant? End
        {
            get; set;
        }

        [JsonProperty("timeZone")]
        public DateTimeZone TimeZone
        {
            get; set;
        }

        [JsonProperty("maxAltitude")]
        public Altitude MaxAltitude
        {
            get; set;
        }

        [JsonProperty("approvalOptions")]
        public List<ApprovalOptionReference> ApprovalOptions
        {
            get;
            set;
        }
    }
}
