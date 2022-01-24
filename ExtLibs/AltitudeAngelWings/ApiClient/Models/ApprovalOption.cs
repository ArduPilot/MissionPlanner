using Newtonsoft.Json;
using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ApprovalOption
    {
        [JsonProperty("sections")]
        public List<ApprovalSection> Sections { get; set; }

        [JsonProperty("conditions")]
        public List<Condition> Conditions
        {
            get;
            set;
        } = new List<Condition>();
    }
}
