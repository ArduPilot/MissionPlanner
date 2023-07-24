using System;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2
{
    public class FlightState<T>
    {
        public FlightState() : this(default)
        {
        }

        public FlightState(T value)
        {
            Value = value;
        }

        /// <summary>
        /// The state value
        /// </summary>
        [JsonProperty("value")]
        public T Value { get; set; }

        /// <summary>
        /// When the value was last set in UTC. Null if never set.
        /// </summary>
        [JsonProperty("lastUpdatedUtc")]
        public DateTime? LastUpdatedUtc { get; set; }
    }
}