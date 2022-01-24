using System.Collections.Generic;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2
{
    public class StartFlightRequest
    {
        public string FlightPlanId { get; set; }

        public List<IFlightServiceRequest> ServiceRequests { get; set; } = new List<IFlightServiceRequest>();
    }
}
