using System.Runtime.Serialization;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public enum AirFrameType
    {
        [EnumMember(Value = "fixedWing")]
        FixedWing = 1,
        [EnumMember(Value = "rotary")]
        Rotary = 2,
        [EnumMember(Value = "vtol")]
        VTOL = 3,
        [EnumMember(Value = "tethered")]
        Tethered = 4
    }
}