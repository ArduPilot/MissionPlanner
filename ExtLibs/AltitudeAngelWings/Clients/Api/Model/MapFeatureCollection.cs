using System.Collections.Generic;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class MapFeatureCollection : FeatureCollection
    {
        [JsonProperty(PropertyName = "isCompleteData", Required = Required.Always)]
        public bool IsCompleteData { get; set; }

        [JsonProperty(PropertyName = "excludedData")]
        public List<ExcludedData> ExcludedData { get; set; } = new List<ExcludedData>();
    }
}
