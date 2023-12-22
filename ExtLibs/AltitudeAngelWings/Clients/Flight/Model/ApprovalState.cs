using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApprovalState
    {
        [EnumMember(Value = "pending")]
        Pending = 1,
        [EnumMember(Value = "approved")]
        Approved = 2,
        [EnumMember(Value = "rejected")]
        Rejected = 3,
        [EnumMember(Value = "cancelled")]
        Cancelled = 4,
        [EnumMember(Value = "preliminaryApproved")]
        PreliminaryApproved = 5,
        [EnumMember(Value = "notRequired")]
        NotRequired = 6,
        [EnumMember(Value = "rescinded")]
        Rescinded = 7,
        [EnumMember(Value = "invalid")]
        Invalid = 8,
        [EnumMember(Value = "expired")]
        Expired = 9
    }
}