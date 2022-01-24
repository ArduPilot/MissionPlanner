using System.Collections.Generic;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using NodaTime;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class CreateFlightPartRequest
    {
        [JsonProperty("id")]
        public string Id
        {
            get; set;
        }

        [JsonProperty("geography")]
        public Feature Geography
        {
            get; set;
        }

        [JsonProperty("start")]
        public Instant Start
        {
            get; set;
        }

        [JsonProperty("end")]
        public Instant End
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
