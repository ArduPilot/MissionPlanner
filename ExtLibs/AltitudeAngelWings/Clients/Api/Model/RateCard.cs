using System;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class RateCard
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("billableEvent")]
        public string BillableEvent { get; set; }
        [JsonProperty("appliesFrom")]
        public DateTimeOffset? AppliesFrom { get; set; }
        [JsonProperty("appliesTo")]
        public DateTimeOffset? AppliesTo { get; set; }
    }
}