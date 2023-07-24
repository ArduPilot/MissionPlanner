using AltitudeAngelWings.Clients.Flight.Model;
using MissionPlanner;

namespace AltitudeAngelWings.Plugin
{
    public static class MavStateExtensions
    {
        public static FlightCapability ToFlightCapability(this MAVState state)
        {
            switch (state.aptype)
            {
                case MAVLink.MAV_TYPE.FIXED_WING:
                    return FlightCapability.FixedWing;

                case MAVLink.MAV_TYPE.QUADROTOR:
                case MAVLink.MAV_TYPE.COAXIAL:
                case MAVLink.MAV_TYPE.HEXAROTOR:
                case MAVLink.MAV_TYPE.OCTOROTOR:
                case MAVLink.MAV_TYPE.TRICOPTER:
                case MAVLink.MAV_TYPE.DODECAROTOR:
                case MAVLink.MAV_TYPE.HELICOPTER:
                    return FlightCapability.Rotary;

                case MAVLink.MAV_TYPE.VTOL_DUOROTOR:
                case MAVLink.MAV_TYPE.VTOL_QUADROTOR:
                case MAVLink.MAV_TYPE.VTOL_TILTROTOR:
                case MAVLink.MAV_TYPE.VTOL_RESERVED2:
                case MAVLink.MAV_TYPE.VTOL_RESERVED3:
                case MAVLink.MAV_TYPE.VTOL_RESERVED4:
                case MAVLink.MAV_TYPE.VTOL_RESERVED5:
                    return FlightCapability.VTOL;

                default:
                    return FlightCapability.Unspecified;
            }
        }
    }
}