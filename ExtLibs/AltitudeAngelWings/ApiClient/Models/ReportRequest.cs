using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ReportRequest
    {
        [JsonProperty(PropertyName = "types")]
        public IList<string> RequestedReportTypes { get; set; }

        [JsonProperty(PropertyName = "flight")]
        public FlightInfo FlightInfo { get; set; }

        [JsonProperty(PropertyName = "aircraft")]
        public AircraftInfo AircraftInfo { get; set; }

        public ReportRequest(AircraftInfo aircraftInfo, FlightInfo flightInfo, params string[] reportTypes)
        {
            AircraftInfo = aircraftInfo;
            FlightInfo = flightInfo;
            RequestedReportTypes = new List<string>(reportTypes);
        }
    }
}
