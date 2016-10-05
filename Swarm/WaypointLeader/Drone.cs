using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.HIL;
using MissionPlanner.Utilities;
using ZedGraph;

namespace MissionPlanner.Swarm.WaypointLeader
{
    public class Drone
    {
        public MAVState MavState;

        public PointLatLngAlt Location;
        public PointLatLngAlt TargetLocation;

        public Vector3 Velocity;
        public Vector3 TargetVelocity;

        public double Heading { get { return Math.Atan2(Velocity.y, Velocity.x) * (180 / Math.PI); } }

        public int PathIndex = 0;

        /// <summary>
        /// Position in 1 second
        /// </summary>
        public PointLatLngAlt ProjectedLocation
        {
            get { return Location.newpos(Heading, Velocity.length()); }
        }

        public void SendPositionVelocity(PointLatLngAlt pos, Vector3 vel)
        {
            MavState.parent.setPositionTargetGlobalInt(MavState.sysid, MavState.compid, true, true, false,
                MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT, pos.Lat, pos.Lng, pos.Alt, vel.x, vel.y, -vel.z);
        }

        public void SendVelocity(Vector3 vel)
        {
            MavState.parent.setPositionTargetGlobalInt(MavState.sysid, MavState.compid, false, true, false,
                MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT, 0, 0, 0, vel.x, vel.y, -vel.z);
        }
    }
}
