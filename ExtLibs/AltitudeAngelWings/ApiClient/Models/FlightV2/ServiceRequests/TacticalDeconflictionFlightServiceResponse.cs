using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests
{
    public class TacticalDeconflictionFlightServiceResponse : IFlightServiceResponse<TacticalDeconflictionResponseProperties>
    {
        /// <inheritdoc />
        public string Name => TacticalDeconflictionFlightServiceRequest.ServiceName;

        /// <inheritdoc />
        public TacticalDeconflictionResponseProperties Properties { get; set; }

        /// <inheritdoc />
        public List<object> Conditions { get; set; } = new List<object>();

        /// <inheritdoc />
        public List<object> Errors { get; set; } = new List<object>();
    }
}
