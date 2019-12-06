using System;
using System.Collections.Generic;
using System.Text;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner;
using MissionPlanner.ArduPilot;
using MissionPlanner.Maps;

namespace SvgNet.SvgGdi
{
    class shim
    {
    }


}

namespace GMap.NET.Drawing.Properties
{
    internal class Resources : Xamarin.Properties.Resources
    {

    }
}

namespace MissionPlanner.Maps
{
    internal class Resources: Xamarin.Properties.Resources
    {

    }
}

namespace Xamarin
{
    public class Common
    {
        public static GMapMarker getMAVMarker(MAVState MAV)
        {
            PointLatLng portlocation = new PointLatLng(MAV.cs.lat, MAV.cs.lng);

            if (MAV.aptype == MAVLink.MAV_TYPE.FIXED_WING)
            {
                // colorise map marker/s based on their sysid, for common sysid/s used 1-6, 11-16, and 101-106
                // its rare for ArduPilot to be used to fly more than 6 planes at a time from one console.
                int which = 0; // default 0=red for other sysids
                if ((MAV.sysid >= 1) && (MAV.sysid <= 6)) { which = MAV.sysid - 1; }  //1=black, 2=blue, 3=green,4=yellow,5=orange,6=red
                if ((MAV.sysid >= 11) && (MAV.sysid <= 16)) { which = MAV.sysid - 11; }  //1=black, 2=blue, 3=green,4=yellow,5=orange,6=red
                if ((MAV.sysid >= 101) && (MAV.sysid <= 106)) { which = MAV.sysid - 101; }  //1=black, 2=blue, 3=green,4=yellow,5=orange,6=red

                return (new GMapMarkerPlane(which, portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing,
                    MAV.cs.radius * CurrentState.multiplierdist)
                {
                    ToolTipText = MAV.cs.alt.ToString("0") + CurrentState.AltUnit + " | " + (int)MAV.cs.airspeed +
                                  CurrentState.SpeedUnit + " | id:" + (int)MAV.sysid,
                    ToolTipMode = MarkerTooltipMode.Always
                });
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.GROUND_ROVER)
            {
                return (new GMapMarkerRover(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing)
                {
                    ToolTipText = MAV.cs.alt.ToString("0") + "\n" + MAV.sysid.ToString("sysid: 0"),
                    ToolTipMode = MarkerTooltipMode.Always
                });
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SURFACE_BOAT)
            {
                return (new GMapMarkerBoat(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing));
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SUBMARINE)
            {
                return (new GMapMarkerSub(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing));
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER)
            {
                return (new GMapMarkerHeli(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing));
            }
            else if (MAV.cs.firmware == Firmwares.ArduTracker)
            {
                return (new GMapMarkerAntennaTracker(portlocation, MAV.cs.yaw,
                    MAV.cs.target_bearing));
            }
            else if (MAV.cs.firmware == Firmwares.ArduCopter2 || MAV.aptype == MAVLink.MAV_TYPE.QUADROTOR)
            {
                if (MAV.param.ContainsKey("AVD_W_DIST_XY") && MAV.param.ContainsKey("AVD_F_DIST_XY"))
                {
                    var w = MAV.param["AVD_W_DIST_XY"].Value;
                    var f = MAV.param["AVD_F_DIST_XY"].Value;
                    return (new GMapMarkerQuad(portlocation, MAV.cs.yaw,
                        MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid)
                    {
                        danger = (int)f,
                        warn = (int)w
                    });
                }

                return (new GMapMarkerQuad(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid));
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.COAXIAL)
            {
                return (new GMapMarkerSingle(portlocation, MAV.cs.yaw,
                   MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid));
            }
            else
            {
                // unknown type
                return (new GMarkerGoogle(portlocation, GMarkerGoogleType.green_dot));
            }
        }
    }

}