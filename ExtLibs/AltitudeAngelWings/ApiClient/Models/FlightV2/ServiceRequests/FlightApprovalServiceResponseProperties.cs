using AltitudeAngelWings.ApiClient.Models.Flight;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests
{
    public class FlightApprovalServiceResponseProperties
    {
        /// <summary>
        /// Current overall approval status 
        /// </summary>
        public ApprovalStatus FlightStatus { get; set; }
    }
}