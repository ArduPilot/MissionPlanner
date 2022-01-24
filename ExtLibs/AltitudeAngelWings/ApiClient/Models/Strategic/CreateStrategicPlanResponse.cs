using System;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models.Strategic
{
    public class CreateStrategicPlanResponse
    {
        /// <summary>
        /// The flight plan id.
        /// </summary>
        [JsonProperty("flightPlanId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? FlightPlanId 
        {
            get;
            set;
        }

        public Guid OperationId 
        {
            get;
            set;
        }

        public int OutcomeCode => (int)this.Outcome;

        public StrategicSeverity Outcome 
        {
            get;
            set;
        }
    }
}
