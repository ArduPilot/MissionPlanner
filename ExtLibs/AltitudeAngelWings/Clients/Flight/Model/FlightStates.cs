using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class FlightStates
    {
        /// <summary>
        /// The airborne state of the aircraft involved in the flight.
        /// </summary>
        [JsonProperty("airborneState")]
        public FlightState<AirborneState> AirborneState { get; set; }

        /// <summary>
        /// Whether the aircraft is in an emergency
        /// </summary>
        [JsonProperty("emergencyState")]
        public FlightState<EmergencyState> EmergencyState { get; set; }

        /// <summary>
        /// Aircraft situation response 
        /// </summary>
        [JsonProperty("situationResponseState")]
        public FlightState<SituationResponseState> SituationResponseState { get; set; }
    }
}