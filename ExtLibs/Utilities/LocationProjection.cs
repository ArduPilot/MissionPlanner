using System;
using System.Collections.Generic;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class LocationProjection
    {
        public static PointLatLngAlt Project(PointLatLngAlt location, Vector3 velocity, DateTime locationtime, DateTime now)
        {
            var deltatime = now - locationtime;
            var bearing = Math.Atan2(velocity.y, velocity.x) * (180 / Math.PI);
            var dist = velocity * deltatime.TotalSeconds;
            var newpos = location.newpos(bearing, dist.length());
            newpos.Alt += dist.z;
            return newpos;
        }
    }
}
