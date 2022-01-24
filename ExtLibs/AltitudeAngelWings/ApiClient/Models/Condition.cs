using Newtonsoft.Json;
using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class Condition
    {
        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }

        [JsonProperty("text")]
        public string Text
        {
            get;
            set;
        }

        [JsonProperty("data")]
        public List<ConditionInfo> Data
        {
            get;
            set;
        } = new List<ConditionInfo>();
    }
}
