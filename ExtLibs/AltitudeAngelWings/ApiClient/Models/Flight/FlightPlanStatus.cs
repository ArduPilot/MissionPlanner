using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AltitudeAngelWings.ApiClient.Models.Flight
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FlightPlanStatus
    {
        [EnumMember(Value = "submitted")]
        Submitted = 1,
        [EnumMember(Value = "active")]
        Active = 2,
        [EnumMember(Value = "complete")]
        Complete = 3,
        [EnumMember(Value = "cancelled")]
        Cancelled = 4
    }
}