using System;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests
{
    public class FlightServiceResponseConverter : JsonCreationConverter<IFlightServiceResponse>
    {
        /// <inheritdoc />
        protected override IFlightServiceResponse Create(Type objectType, JObject jObject)
        {
            switch (jObject.GetValue("name", StringComparison.InvariantCultureIgnoreCase)?.Value<string>())
            {
                case TacticalDeconflictionFlightServiceRequest.ServiceName:
                    return new TacticalDeconflictionFlightServiceResponse();
                case FlightApprovalServiceRequest.ServiceName:
                    return new FlightApprovalServiceResponse();
                default:
                    throw new ArgumentOutOfRangeException("wow");
            }
        }
    }
}