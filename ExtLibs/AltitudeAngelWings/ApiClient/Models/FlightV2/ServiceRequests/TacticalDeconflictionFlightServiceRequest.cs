namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests
{
    public class TacticalDeconflictionFlightServiceRequest : IFlightServiceRequest<TacticalDeconflictionRequestProperties>
    {
        public const string ServiceName = "tactical_deconfliction";

        /// <inheritdoc />
        public string Name => TacticalDeconflictionFlightServiceRequest.ServiceName;

        /// <inheritdoc />
        public TacticalDeconflictionRequestProperties Properties { get; set; }
    }
}
