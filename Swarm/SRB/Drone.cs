using System;
using MissionPlanner.HIL;
using MissionPlanner.Utilities;

namespace MissionPlanner.Swarm.SRB
{
    public class Drone
    {
        public PointLatLngAlt Location;
        public MAVState MavState;
        public PointLatLngAlt TargetLocation;
        public Vector3 TargetVelocity;

        public Vector3 Velocity;
        public bool takeoffdone;

        public double Heading => Math.Atan2(Velocity.y, Velocity.x) * (180 / Math.PI);

        /// <summary>
        ///     Position in 1 second
        /// </summary>
        public PointLatLngAlt ProjectedLocation => Location.newpos(Heading, Velocity.length());

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

        public void SendYaw(float heading)
        {
            MavState.parent.doCommand(MavState.sysid, MavState.compid, MAVLink.MAV_CMD.CONDITION_YAW, heading,
                100.0f, 0, 0, 0, 0, 0, false);
        }
    }
}