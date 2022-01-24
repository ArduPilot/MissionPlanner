using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class DisplaySection
    {
        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("displayTitle")]
        public string DisplayTitle { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
        
        [JsonProperty("disclaimer")]
        public string Disclaimer { get; set; }
    }
}