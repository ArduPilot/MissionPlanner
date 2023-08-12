using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class CreateFlightPlanResponse
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public ApprovalStatus Status { get; set; }

        [JsonProperty("flightPlanStatus", NullValueHandling = NullValueHandling.Ignore)]
        public FlightPlanStatus FlightPlanStatus { get; set; } = FlightPlanStatus.Submitted;

        [JsonProperty("reasons", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<string>> ApproverToReasons { get; set; }

        [JsonProperty("flightAlertSubscriptionId", NullValueHandling = NullValueHandling.Ignore)]
        public string FlightAlertSubscriptionId { get; set; }
    }
}