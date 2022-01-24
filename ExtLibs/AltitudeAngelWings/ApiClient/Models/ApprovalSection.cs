using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ApprovalSection
    {
        [JsonProperty("geog")]
        public FeatureCollection Geog
        {
            get;
            set;
        }

        [JsonProperty("sectionOptions")]
        public List<ApprovalSectionOption> SectionOptions
        {
            get;
            set;
        }
    }
}
