using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class FilterInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("property")]
        public string Property { get; set; }

        [JsonProperty("parentName")]
        public string ParentName { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}