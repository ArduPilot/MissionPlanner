using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class DisplayInfo
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("detailedCategory")]
        public string DetailedCategory { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("sections")]
        public IList<DisplaySection> Sections { get; set; } = new List<DisplaySection>();
    }
}