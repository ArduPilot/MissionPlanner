using System;
using MissionPlanner.Utilities;

namespace MissionPlanner.Swarm
{
    public class DroneBase
    {
        public PointLatLngAlt Location;
        public MAVState MavState;
        public PointLatLngAlt TargetLocation;
        public Vector3 TargetVelocity;
        public Vector3 Velocity;
        public double Heading => Math.Atan2(Velocity.y, Velocity.x) * (180 / Math.PI);

        /// <summary>
        ///     Position in 1 second
        /// </summary>
        public PointLatLngAlt ProjectedLocation => Location.newpos(Heading, Velocity.length());

        public PointLatLngAlt ProjectedLocation2 => Location.newpos(Heading, Velocity.length() * 2);

        public void SendPositionVelocityYaw(PointLatLngAlt pos, Vector3 vel, double yaw, double yawrate=100)
        {
            if (pos == null || vel == null)
                return;

            MavState.parent.setPositionTargetGlobalInt(MavState.sysid, MavState.compid, true, true, false, true,
                MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT, pos.Lat, pos.Lng, pos.Alt, vel.x, vel.y, vel.z, yaw * MathHelper.deg2rad,
                yawrate);
        }

        public void SendPositionVelocity(PointLatLngAlt pos, Vector3 vel)
        {
            if (pos == null || vel == null)
                return;

            MavState.parent.setPositionTargetGlobalInt(MavState.sysid, MavState.compid, true, true, false, false,
                MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT, pos.Lat, pos.Lng, pos.Alt, vel.x, vel.y, vel.z, 0, 0);
        }

        public void SendVelocity(Vector3 vel)
        {
            if (vel == null)
                return;

            MavState.parent.setPositionTargetGlobalInt(MavState.sysid, MavState.compid, false, true, false, false,
                MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT, 0, 0, 0, vel.x, vel.y, vel.z, 0, 0);
        }

        public void SendYaw(float heading)
        {
            if ((Math.Abs(MavState.cs.yaw - heading) - 3) > 0)
            {
                MavState.parent.doCommand(MavState.sysid, MavState.compid, MAVLink.MAV_CMD.CONDITION_YAW, heading,
                    100.0f, 0, 0, 0, 0, 0, false);
            }
        }
    }
}