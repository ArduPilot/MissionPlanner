using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MavlinkMessagePlugin
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
    ///<summary> Metrics typically displayed on a HUD for fixed wing aircraft. </summary>
    public struct mavlink_vfr_hud_EDIT_t
    {
        /// <summary>Current indicated airspeed (IAS).  [m/s] </summary>
        [MAVLink.Units("[m/s]")]
        [MAVLink.Description("Current indicated airspeed (IAS).")]
        public float airspeed;
        /// <summary>Current ground speed.  [m/s] </summary>
        [MAVLink.Units("[m/s]")]
        [MAVLink.Description("Current ground speed.")]
        public float groundspeed;
        /// <summary>Current altitude (MSL).  [m] </summary>
        [MAVLink.Units("[m]")]
        [MAVLink.Description("Current altitude (MSL).")]
        public float alt;
        /// <summary>Current climb rate.  [m/s] </summary>
        [MAVLink.Units("[m/s]")]
        [MAVLink.Description("Current climb rate.")]
        public float climb;
        /// <summary>Current heading in compass units (0-360, 0=north).  [deg] </summary>
        [MAVLink.Units("[deg]")]
        [MAVLink.Description("Current heading in compass units (0-360, 0=north).")]
        public short heading;
        /// <summary>Current throttle setting (0 to 100).  [%] </summary>
        [MAVLink.Units("[%]")]
        [MAVLink.Description("Current throttle setting (0 to 100).")]
        public ushort throttle;

    };
}
