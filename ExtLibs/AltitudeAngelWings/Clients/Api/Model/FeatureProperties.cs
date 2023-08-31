using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class FeatureProperties
    {
        [JsonProperty("detailedCategory")]
        public string DetailedCategory { get; set; }

        [JsonProperty("isOwner")]
        public bool IsOwner { get; set; }

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

        [JsonProperty("radius")]
        public string Radius { get; set; }

        [JsonProperty("display")]
        public DisplayInfo DisplayInfo { get; set; }

        [JsonProperty("altitudeFloor")]
        public AltitudeProperty AltitudeFloor { get; set; }

        [JsonProperty("utmStatus")]
        public UtmStatus UtmStatus { get; set; }

        [JsonProperty("contact")]
        public Contact Contact { get; set; }
    }
}