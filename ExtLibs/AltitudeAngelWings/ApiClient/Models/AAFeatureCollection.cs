using System.Collections.Generic;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class AAFeatureCollection : FeatureCollection
    {
        [JsonProperty(PropertyName = "isCompleteData", Required = Required.Always)]
        public bool IsCompleteData { get; set; }

        [JsonProperty(PropertyName = "excludedData")]
        public List<JObject> ExcludedData { get; set; } = new List<JObject>();
    }
}
