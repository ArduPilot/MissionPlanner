using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class RateCardDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("version")]
        public DateTimeOffset Version { get; set; }
        [JsonProperty("billableEvent")]
        public string BillableEvent { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("rates")]
        public IList<RateDetail> Rates { get; set; }
        [JsonProperty("tenant")]
        public string Tenant { get; set; }
        [JsonProperty("appliesFrom")]
        public DateTimeOffset AppliesFrom { get; set; }
        [JsonProperty("appliesTo")]
        public DateTimeOffset? AppliesTo { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("taxRate")]
        public double TaxRate { get; set; }
        [JsonProperty("standingCharge")]
        public double StandingCharge { get; set; }
        [JsonProperty("rateCardTerms")]
        public string RateCardTerms { get; set; }
        [JsonProperty("rateCardTermsVersion")]
        public DateTimeOffset RateCardTermsVersion { get; set; }
        [JsonProperty("aaTerms")]
        public string AaTerms { get; set; }
        [JsonProperty("costUnitId")]
        public string CostUnitId { get; set; }
        [JsonProperty("costUnitVersion")]
        public DateTimeOffset CostUnitVersion { get; set; }
        [JsonProperty("hasCostUnit")]
        public bool HasCostUnit { get; set; }
        [JsonProperty("explanatoryText")]
        public string ExplanatoryText { get; set; }
    }
}