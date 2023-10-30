using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class ExcludedData
    {
        [JsonProperty(PropertyName = "errorReason")]
        public string ErrorReason { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "detail")]
        public ExcludedDataDetail Detail { get; set; }
    }
}