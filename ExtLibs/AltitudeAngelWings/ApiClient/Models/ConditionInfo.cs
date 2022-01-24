using Newtonsoft.Json;
using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ConditionInfo
    {
        [JsonProperty("label")]
        public string Label
        {
            get;
            set;
        }

        [JsonProperty("data")]
        public Dictionary<string, string> Data
        {
            get;
            set;
        } = new Dictionary<string, string>();

        [JsonIgnore]
        public string Type
        {

            get => Data[WellKnownDataKeys.Type];
            set => Data[WellKnownDataKeys.Type] = value;
        }
    }
}
