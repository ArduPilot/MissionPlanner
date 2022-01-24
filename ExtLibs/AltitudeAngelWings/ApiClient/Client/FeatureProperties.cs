using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class FeatureProperties
    {
        [JsonProperty("detailedCategory")]
        public string DetailedCategory { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("fromUtc")]
        public string FromUtc { get; set; }

        [JsonProperty("toUtc")]
        public string ToUtc { get; set; }

        [JsonProperty("fromLocal")]
        public string FromLocal { get; set; }

        [JsonProperty("toLocal")]
        public string ToLocal { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }

        [JsonProperty("flightType")]
        public string FlightType { get; set; }

        [JsonProperty("radiusMeters")]
        public string RadiusMeters { get; set; }

        [JsonProperty("isOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("alertSummary")]
        public string AlertSummary { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("hazardFactor")]
        public string HazardFactor { get; set; }

        [JsonProperty("hazardFactorName")]
        public string HazardFactorName { get; set; }

        [JsonProperty("fillColor")]
        public string FillColor { get; set; }

        [JsonProperty("strokeColor")]
        public string StrokeColor { get; set; }

        [JsonProperty("fillOpacity")]
        public string FillOpacity { get; set; }

        [JsonProperty("strokeOpacity")]
        public string StrokeOpacity { get; set; }

        [JsonProperty("strokeWidth")]
        public string StrokeWidth { get; set; }

        [JsonProperty("borderColor")]
        public string BorderColor { get; set; }

        [JsonProperty("borderOpacity")]
        public string BorderOpacity { get; set; }

        [JsonProperty("borderWidth")]
        public string BorderWidth { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("radius")]
        public string Radius { get; set; }

        [JsonProperty("filters")]
        public IList<FilterInfo> Filters { get; set; }

        [JsonProperty("display")]
        public DisplayInfo DisplayInfo { get; set; }
    }
}