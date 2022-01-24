using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class CreateFlightPlanRequest
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("flightOperationMode")]
        public FlightOperationMode FlightOperationMode { get; set; }

        [JsonProperty("flightCapability")]
        public FlightCapability FlightCapability { get; set; }

        [JsonProperty("parts")]
        public List<CreateFlightPartRequest> Parts { get; set; }

        [JsonProperty("dataFields")]
        public JObject DataFields { get; set; }

        [JsonProperty("droneSerialNumber")]
        public string DroneSerialNumber { get; set; }
    }
}
