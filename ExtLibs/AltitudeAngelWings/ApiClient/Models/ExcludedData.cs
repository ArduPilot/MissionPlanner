using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ExcludedData
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("detail")]
        public Detail Detail { get; set; }

        [JsonProperty("errorReason")]
        public string ErrorReason { get; set; }
    }
}
