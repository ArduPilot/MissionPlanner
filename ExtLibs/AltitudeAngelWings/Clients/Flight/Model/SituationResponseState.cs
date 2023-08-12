using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class SituationResponseState
    {
        /// <summary>
        /// The aircraft priority response
        /// </summary>
        [JsonProperty("inResponse")]
        public bool InResponse { get; set; } = false;

        /// <summary>
        /// The aircraft response reason eg: emergency, no fuel etc
        /// </summary>
        [JsonProperty("responseReason")]
        public string ResponseReason { get; set; }
    }
}