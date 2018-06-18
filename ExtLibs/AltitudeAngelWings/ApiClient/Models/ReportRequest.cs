using System.Collections.Generic;
using DotNetOpenAuth.Messaging;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ReportRequest
    {
        [JsonProperty(PropertyName = "types")]
        public IList<string> RequestedReportTypes { get; set; } = new List<string>();
        [JsonProperty(PropertyName = "flight")]
        public FlightInfo FlightInfo { get; set; }

        [JsonProperty(PropertyName = "aircraft")]
        public AircraftInfo AircraftInfo { get; set; }

        public ReportRequest(AircraftInfo aircraftInfo, FlightInfo flightInfo, params string[] reportTypes)
        {
            AircraftInfo = aircraftInfo;
            FlightInfo = flightInfo;
            RequestedReportTypes.AddRange(reportTypes);
        }
    }
}
