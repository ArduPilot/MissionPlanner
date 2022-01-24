using System;
using System.Collections.Generic;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2
{
    public class StartFlightResponse
    {
        /// <summary>
        ///The ID of the newly started flight.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The ID of the flight plan.
        /// </summary>
        public string FlightPlanId { get; set; }

        /// <summary>
        /// The date and time the flight was started, if it started successfully
        /// </summary>
        public DateTimeOffset? Started { get; set; }

        /// <summary>
        /// A list of individual responses for each <see cref="FlightServiceRequest"/> issued
        /// </summary>
        public List<TacticalDeconflictionFlightServiceResponse> ServiceResponses { get; set; }
    }
}
