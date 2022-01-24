using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class Detail
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
