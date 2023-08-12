using System.Collections.Generic;
using AltitudeAngelWings.Clients.Flight.Model.ServiceRequests;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class StartFlightRequest
    {
        public string FlightPlanId { get; set; }

        public List<IFlightServiceRequest> ServiceRequests { get; set; } = new List<IFlightServiceRequest>();
    }
}
