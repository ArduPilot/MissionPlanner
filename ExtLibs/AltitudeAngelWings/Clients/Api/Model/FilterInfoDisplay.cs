using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class FilterInfoDisplay : FilterInfo
    {
        [JsonProperty("visible")]
        public bool Visible { get; set; }
    }
}