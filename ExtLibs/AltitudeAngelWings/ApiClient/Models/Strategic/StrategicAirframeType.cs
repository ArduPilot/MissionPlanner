using System.Runtime.Serialization;

namespace AltitudeAngelWings.ApiClient.Models.Strategic
{
    public enum StrategicAirframeType
    {
        [EnumMember(Value = "fixedWing")]
        FixedWing = 1,
        [EnumMember(Value = "rotary")]
        Rotary = 2
    }
}
