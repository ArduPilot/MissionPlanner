using System;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class RateDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("version")]
        public DateTimeOffset Version { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("rate")]
        public double Rate { get; set; }
        [JsonProperty("ordinal")]
        public int Ordinal { get; set; }
    }
}