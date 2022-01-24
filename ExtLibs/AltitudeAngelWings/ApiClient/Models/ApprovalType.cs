using Newtonsoft.Json;
using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ApprovalType
    {
        [JsonProperty("typeName")]
        public string TypeName
        {
            get;
            set;
        }

        [JsonProperty("authority")]
        public string Authority
        {
            get;
            set;
        }

        [JsonProperty("unit")]
        public string Unit
        {
            get;
            set;
        }

        [JsonProperty("unitId")]
        public string UnitId
        {
            get;
            set;
        }

        [JsonProperty("unitAirspaceClass")]
        public string UnitAirspaceClass
        {
            get;
            set;
        }

        [JsonProperty("responseTime")]
        public object ResponseTime => null;

        [JsonProperty("dataFields")]
        public IEnumerable<string> DataFields
        {
            get;
            set;
        }

        [JsonProperty("reference")]
        public ApprovalOptionalReference<LaancApprovalReferenceProperties> Reference
        {
            get;
            set;
        }
    }
}
