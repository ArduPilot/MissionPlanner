using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ApprovalOptionalReference<T>
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
        public T Data
        {
            get;
            set;
        }
    }
}
