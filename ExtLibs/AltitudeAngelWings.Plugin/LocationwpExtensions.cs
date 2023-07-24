using System.Collections.Generic;
using System.Linq;
using AltitudeAngelWings.Model;
using MissionPlanner.Utilities;

namespace AltitudeAngelWings.Plugin
{
    public static class LocationwpExtensions
    {
        public static FlightPlanWaypoint ToWaypoint(this Locationwp location)
            => new FlightPlanWaypoint
            {
                Latitude = location.lat,
                Longitude = location.lng,
                Altitude = location.alt
            };

        public static IEnumerable<FlightPlanWaypoint> ToWaypoints(this IEnumerable<Locationwp> locations)
            => locations.Where(IsValidWaypoint).Select(l => l.ToWaypoint());

        public static bool IsValidWaypoint(this Locationwp location)
        {
            // Command that can contain latitude and longitude
            var cmd = (MAVLink.MAV_CMD)location.id;
            if (cmd != MAVLink.MAV_CMD.WAYPOINT &&
                cmd != MAVLink.MAV_CMD.LOITER_UNLIM &&
                cmd != MAVLink.MAV_CMD.LOITER_TURNS &&
                cmd != MAVLink.MAV_CMD.LOITER_TIME &&
                cmd != MAVLink.MAV_CMD.LAND &&
                cmd != MAVLink.MAV_CMD.TAKEOFF &&
                cmd != MAVLink.MAV_CMD.FOLLOW &&
                cmd != MAVLink.MAV_CMD.LOITER_TO_ALT &&
                cmd != MAVLink.MAV_CMD.PATHPLANNING &&
                cmd != MAVLink.MAV_CMD.SPLINE_WAYPOINT &&
                cmd != MAVLink.MAV_CMD.VTOL_TAKEOFF &&
                cmd != MAVLink.MAV_CMD.VTOL_LAND &&
                cmd != MAVLink.MAV_CMD.PAYLOAD_PLACE &&
                cmd != MAVLink.MAV_CMD.DO_SET_HOME &&
                cmd != MAVLink.MAV_CMD.DO_LAND_START &&
                cmd != MAVLink.MAV_CMD.DO_REPOSITION &&
                cmd != MAVLink.MAV_CMD.DO_SET_ROI_LOCATION &&
                cmd != MAVLink.MAV_CMD.DO_MOUNT_CONTROL &&
                cmd != MAVLink.MAV_CMD.OVERRIDE_GOTO &&
                cmd != MAVLink.MAV_CMD.SET_GUIDED_SUBMODE_CIRCLE &&
                cmd != MAVLink.MAV_CMD.FENCE_RETURN_POINT &&
                cmd != MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION &&
                cmd != MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION &&
                cmd != MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION &&
                cmd != MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION &&
                cmd != MAVLink.MAV_CMD.RALLY_POINT &&
                cmd != MAVLink.MAV_CMD.PAYLOAD_PREPARE_DEPLOY &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_1 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_2 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_3 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_4 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_5 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_1 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_2 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_3 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_4 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_5)
            {
                return false;
            }

            // Exclude "empty" latitude and longitude
            return location.lat != 0 && location.lng != 0;
        }
    }
}