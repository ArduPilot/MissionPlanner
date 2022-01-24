using System.Runtime.Serialization;

namespace AltitudeAngelWings.ApiClient.Models.Strategic
{
    public enum StrategicConflictResolutionScope
    {
        [EnumMember(Value = "local")]
        Local = 1,
        [EnumMember(Value = "global")]
        Global = 2
    }
}
