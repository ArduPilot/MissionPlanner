using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ApprovalOptionReference
    {
        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }

        [JsonProperty("approverId")]
        public string ApproverId
        {
            get;
            set;
        }

        [JsonProperty("data")]
        public JObject Data
        {
            get;
            set;
        }
    }
}
