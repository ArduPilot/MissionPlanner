using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class UtmStatus
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("utm")]
        public UtmDetails UtmDetails { get; set; }
        [JsonProperty("rateCards")]
        public IDictionary<string, IReadOnlyList<RateCard>> RateTypes { get; set; }
    }
}