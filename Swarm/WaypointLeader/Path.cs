using System.Collections.Generic;
using MissionPlanner.Utilities;

namespace MissionPlanner.Swarm.WaypointLeader
{
    public class Path
    {
        public static List<PointLatLngAlt> GeneratePath(MAVState MAV)
        {
            List<PointLatLngAlt> result = new List<PointLatLngAlt>();

            MAVLink.mavlink_mission_item_t? prevwp = null;

            int a = -1;

            foreach (var wp in MAV.wps.Values)
            {
                a++;
                if (!prevwp.HasValue)
                {
                    // change firstwp/aka home to valid alt
                    prevwp = new MAVLink.mavlink_mission_item_t?(new MAVLink.mavlink_mission_item_t() { x=wp.x,y = wp.y, z = 0});
                    continue;
                }

                if (wp.command != (ushort)MAVLink.MAV_CMD.WAYPOINT && wp.command != (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT)
                    continue;

                var wp1 = new PointLatLngAlt(prevwp.Value);
                var wp2 = new PointLatLngAlt(wp);
                var bearing = wp1.GetBearing(wp2);
                var distwps = wp1.GetDistance(wp2);
                var startalt = wp1.Alt;
                var endalt = wp2.Alt;

                if (startalt == 0)
                {
                    startalt = endalt;
                }

                if(distwps > 5000)
                    continue;

                for (double d = 0.1; d < distwps; d += 0.1)
                {
                    var pnt = wp1.newpos(bearing, d);
                    pnt.Alt = startalt + (d/distwps) * (endalt-startalt);
                    result.Add(pnt);
                }

                prevwp = wp;
            }

            return result;
        }

        public static List<PointLatLngAlt> GenerateOffsetPath(MAVState MAV, Vector3 offset)
        {
            List<PointLatLngAlt> result = GeneratePath(MAV);



            return result;
        }
    }
}
