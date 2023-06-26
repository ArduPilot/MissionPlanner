using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class FilterInfoDisplay : FilterInfo
    {
        [JsonProperty("visible")]
        public bool Visible { get; set; }
    }
}