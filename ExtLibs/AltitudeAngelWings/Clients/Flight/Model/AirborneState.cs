using System.Runtime.Serialization;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public enum AirborneState
    {
        /// <summary>
        /// The state of the aircraft is unknown
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        /// <summary>
        /// The aircraft is reported as being airborne
        /// </summary>
        [EnumMember(Value = "airborne")]
        Airborne = 1,

        /// <summary>
        /// The aircraft is reported as being on the ground
        /// </summary>
        [EnumMember(Value = "grounded")]
        Grounded = 2
    }
}