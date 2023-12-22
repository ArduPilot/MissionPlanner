using System;
using System.Collections.Generic;
using AltitudeAngelWings.Clients.Flight.Model.ServiceRequests;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class StartFlightResponse
    {
        /// <summary>
        ///     The ID of the newly started flight.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     The ID of the flight plan.
        /// </summary>
        [JsonProperty("flightPlanId")]
        public string FlightPlanId { get; set; }

        /// <summary>
        ///     The version of the flight plan that was started
        /// </summary>
        [JsonProperty("flightPlanVersion")]
        public string FlightPlanVersion { get; set; }

        /// <summary>
        ///     The date and time the flight was started, if it started successfully
        /// </summary>
        [JsonProperty("started")]
        public DateTimeOffset? Started { get; set; }

        /// <summary>
        ///     A list of individual responses for each <see cref="FlightServiceRequest" /> issued
        /// </summary>
        [JsonProperty("serviceResponses")]
        public List<IFlightServiceResponse> ServiceResponses { get; set; }

        /// <summary>
        ///     Contains various indicators of the state of the flight
        /// </summary>
        [JsonProperty("flightState")]
        public FlightStates FlightState { get; set; }

        /// <summary>
        /// The priority level of the flight
        /// </summary>       
        [JsonProperty("priorityLevel")]
        public FlightPriorityLevel PriorityLevel { get; set; }
    }
}
