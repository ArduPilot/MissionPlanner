using Newtonsoft.Json;
using NodaTime;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class LaancApprovalReferenceProperties
    {
        [JsonProperty("expiresAt")]
        public Instant ExpiresAt
        {
            get;
            set;
        }

        [JsonProperty("signature")]
        public string Signature
        {
            get;
            set;
        }

        [JsonProperty("unitId")]
        public string UnitId
        {
            get; set;
        }
    }
}
