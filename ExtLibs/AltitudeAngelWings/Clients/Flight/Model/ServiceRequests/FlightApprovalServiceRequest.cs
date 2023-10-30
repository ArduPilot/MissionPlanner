namespace AltitudeAngelWings.Clients.Flight.Model.ServiceRequests
{
    public class FlightApprovalServiceRequest : IFlightServiceRequest<FlightApprovalServiceRequestProperties>
    {
        public const string ServiceName = "flight_approval";

        /// <inheritdoc />
        public string Name => ServiceName;

        /// <inheritdoc />
        public FlightApprovalServiceRequestProperties Properties { get; set; }
    }
}