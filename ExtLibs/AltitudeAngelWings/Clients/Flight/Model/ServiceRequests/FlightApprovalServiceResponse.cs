using System.Collections.Generic;

namespace AltitudeAngelWings.Clients.Flight.Model.ServiceRequests
{
    public class FlightApprovalServiceResponse : IFlightServiceResponse<FlightApprovalServiceResponseProperties>
    {
        /// <inheritdoc />
        public string Name => FlightApprovalServiceRequest.ServiceName;

        /// <inheritdoc />
        public FlightApprovalServiceResponseProperties Properties { get; set; } = new FlightApprovalServiceResponseProperties();

        /// <inheritdoc />
        public List<object> Conditions { get; set; } = new List<object>();

        /// <inheritdoc />
        public List<object> Errors { get; set; } = new List<object>();
    }
}