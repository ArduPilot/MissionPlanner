using Newtonsoft.Json;
using NodaTime;
using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ApprovalSectionOption
    {
        [JsonProperty("approvalRequired")]
        public ApprovalRequired ApprovalRequired
        {
            get;
            set;
        }

        [JsonProperty("approvalRequiredReason")]
        public string ApprovalRequiredReason
        {
            get;
            set;
        }

        [JsonProperty("maxAlt")]
        public Altitude MaxAlt
        {
            get;
            set;
        }

        [JsonProperty("start")]
        public Instant Start
        {
            get;
            set;
        }

        [JsonProperty("end")]
        public Instant End
        {
            get;
            set;
        }

        [JsonProperty("approvalType")]
        public ApprovalType ApprovalType
        {
            get;
            set;
        }

        [JsonProperty("conditions")]
        public List<Condition> Conditions
        {
            get;
            set;
        } = new List<Condition>();
    }
}
