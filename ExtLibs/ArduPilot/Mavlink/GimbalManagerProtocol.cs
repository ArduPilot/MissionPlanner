using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot.Mavlink
{
    public class GimbalManagerProtocol
    {
        CurrentState cs;

        // Stores the last GIMBAL_MANAGER_INFORMATION message for each gimbal device/component ID.
        // This index will be 1-6, or MAVLink component IDs 154, 171-175.
        // Index 0 is used to store the message of the first (lowest) gimbal ID.
        public ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_manager_information_t> ManagerInfo =
            new ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_manager_information_t>();

        // Stores the GIMBAL_MANAGER_STATUS message for each gimbal device/component ID.
        // This index will be 1-6, or MAVLink component IDs 154, 171-175.
        // Index 0 is used to store the message of the first (lowest) gimbal ID.
        public ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_manager_status_t> ManagerStatus =
            new ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_manager_status_t>();

        // Stores the GIMBAL_DEVICE_ATTITUDE_STATUS message for each gimbal device/component ID.
        // This index will be 1-6, or MAVLink component IDs 154, 171-175.
        // Index 0 is used to store the message of the first (lowest) gimbal ID.
        public ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_device_attitude_status_t> GimbalStatus =
            new ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_device_attitude_status_t>();

        private readonly MAVLinkInterface mavint;

        public GimbalManagerProtocol(MAVLinkInterface mavint, CurrentState cs)
        {
            this.mavint = mavint;
            this.cs = cs;
        }

        private bool first_discover = true;
        public void Discover()
        {
            if (first_discover)
            {
                first_discover = false;
                mavint.OnPacketReceived += MessagesHandler;
            }

            mavint.doCommand(0, 0, MAVLink.MAV_CMD.REQUEST_MESSAGE,
                (float)MAVLink.MAVLINK_MSG_ID.GIMBAL_MANAGER_INFORMATION,
                0, 0, 0, 0, 0, 0, false);
        }

        private void MessagesHandler(object sender, MAVLink.MAVLinkMessage message)
        {
            if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.GIMBAL_MANAGER_INFORMATION)
            {
                var gmi = (MAVLink.mavlink_gimbal_manager_information_t)message.data;

                ManagerInfo[gmi.gimbal_device_id] = gmi;
                if (!ManagerInfo.ContainsKey(0) || gmi.gimbal_device_id <= ManagerInfo[0].gimbal_device_id)
                {
                    ManagerInfo[0] = gmi;
                }
            }

            if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.GIMBAL_MANAGER_STATUS)
            {
                var gms = (MAVLink.mavlink_gimbal_manager_status_t)message.data;
                ManagerStatus[gms.gimbal_device_id] = gms;
                if (!ManagerStatus.ContainsKey(0) || gms.gimbal_device_id <= ManagerStatus[0].gimbal_device_id)
                {
                    ManagerStatus[0] = gms;
                }
            }

            if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.GIMBAL_DEVICE_ATTITUDE_STATUS)
            {
                var gds = (MAVLink.mavlink_gimbal_device_attitude_status_t)message.data;
                GimbalStatus[gds.gimbal_device_id] = gds;
                if (!GimbalStatus.ContainsKey(0) || gds.gimbal_device_id <= GimbalStatus[0].gimbal_device_id)
                {
                    GimbalStatus[0] = gds;
                }
            }
        }

        public bool HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS flags, byte gimbal_device_id = 0)
        {
            return ManagerInfo.TryGetValue(gimbal_device_id, out var info) && ((info.cap_flags & (uint)flags) != 0);
        }

        public bool HasAllCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS flags, byte gimbal_device_id = 0)
        {
            return ManagerInfo.TryGetValue(gimbal_device_id, out var info) && ((info.cap_flags & (uint)flags) == (uint)flags);
        }

        public bool HasStatusFlag(MAVLink.GIMBAL_DEVICE_FLAGS flags, byte gimbal_device_id = 0)
        {
            return ManagerStatus.TryGetValue(gimbal_device_id, out var status) && ((status.flags & (uint)flags) != 0);
        }

        public bool YawInVehicleFrame(byte gimbal_device_id = 0)
        {
            bool yaw_in_earth_frame = HasStatusFlag(MAVLink.GIMBAL_DEVICE_FLAGS.YAW_IN_EARTH_FRAME, gimbal_device_id);
            bool yaw_in_vehicle_frame = HasStatusFlag(MAVLink.GIMBAL_DEVICE_FLAGS.YAW_IN_VEHICLE_FRAME, gimbal_device_id);

            // Some older protocols don't set YAW_IN_EARTH_FRAME or YAW_IN_VEHICLE_FRAME flags,
            // with those, we have to infer it from whether YAW_LOCK is set.
            if (!yaw_in_earth_frame && !yaw_in_vehicle_frame)
            {
                bool yaw_lock = HasStatusFlag(MAVLink.GIMBAL_DEVICE_FLAGS.YAW_LOCK, gimbal_device_id);
                yaw_in_vehicle_frame = !yaw_lock;
            }

            return yaw_in_vehicle_frame;
        }

        /// <summary>
        /// Get the reported attitude of the gimbal. Yaw always reported relative to the earth frame.
        /// </summary>
        /// <param name="gimbal_device_id">Device ID of the gimbal. 0 means all gimbals</param>
        /// <returns></returns>
        public Quaternion GetAttitude(byte gimbal_device_id = 0)
        {
            if (!GimbalStatus.TryGetValue(gimbal_device_id, out var status))
            {
                return null;
            }

            var q = new Quaternion(status.q[0], status.q[1], status.q[2], status.q[3]);

            if (YawInVehicleFrame(gimbal_device_id))
            {
                q = Quaternion.from_euler(0, 0, cs.yaw * MathHelper.deg2rad) * q;
            }

            return q;
        }

        public Task<bool> RetractAsync(byte gimbal_device_id = 0)
        {
            if (!HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_RETRACT))
            {
                return Task.FromResult(false);
            }
            return mavint.doCommandAsync(
                (byte)mavint.sysidcurrent,
                (byte)mavint.compidcurrent,
                MAVLink.MAV_CMD.DO_GIMBAL_MANAGER_PITCHYAW,
                float.NaN, // pitch angle
                float.NaN, // yaw angle
                float.NaN, // pitch rate
                float.NaN, // yaw rate
                (float)MAVLink.GIMBAL_MANAGER_FLAGS.RETRACT,
                0, // unused
                gimbal_device_id);
        }

        public Task<bool> NeutralAsync(byte gimbal_device_id = 0)
        {
            if (!HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_NEUTRAL))
            {
                return Task.FromResult(false);
            }
            return mavint.doCommandAsync(
                (byte)mavint.sysidcurrent,
                (byte)mavint.compidcurrent,
                MAVLink.MAV_CMD.DO_GIMBAL_MANAGER_PITCHYAW,
                float.NaN, // pitch angle
                float.NaN, // yaw angle
                float.NaN, // pitch rate
                float.NaN, // yaw rate
                (float)MAVLink.GIMBAL_MANAGER_FLAGS.NEUTRAL,
                0, // unused
                gimbal_device_id);
        }

        public Task<bool> SetRCYawLockAsync(bool yaw_lock, byte gimbal_device_id = 0)
        {
            if ((yaw_lock && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_LOCK)) ||
                (!yaw_lock && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_FOLLOW)))
            {
                return Task.FromResult(false);
            }

            return mavint.doCommandAsync(
                (byte)mavint.sysidcurrent,
                (byte)mavint.compidcurrent,
                MAVLink.MAV_CMD.DO_GIMBAL_MANAGER_PITCHYAW,
                float.NaN, // pitch angle
                float.NaN, // yaw angle
                float.NaN, // pitch rate
                float.NaN, // yaw rate
                yaw_lock ? (float)MAVLink.GIMBAL_MANAGER_FLAGS.YAW_LOCK : 0,
                0, // unused
                gimbal_device_id);
        }

        /// <summary>
        /// Set the attitude of the gimbal with a quaternion. Yaw always reported relative to the earth frame.
        /// </summary>
        /// <param name="q">Gimbal attitude quaternion</param>
        /// <param name="yaw_lock">True if the gimbal should continue to point in this orientation. False if it should follow the yaw of the vehicle.</param>
        /// <param name="gimbal_device_id">Device ID of the gimbal. 0 means all gimbals</param>
        /// <returns></returns>
        public Task<bool> SetAttitudeAsync(Quaternion q, bool yaw_lock, byte gimbal_device_id = 0)
        {
            var pitch = q.get_euler_pitch() * MathHelper.rad2deg;
            var yaw = q.get_euler_yaw() * MathHelper.rad2deg;

            if (!yaw_lock)
            {
                yaw -= cs.yaw;
            }

            Console.WriteLine("SetAttitudeAsync: pitch={0}, yaw={1}, yaw_lock={2}", pitch, yaw < 0 ? yaw + 360 : yaw, yaw_lock);
            return SetAnglesCommandAsync(pitch, yaw, yaw_lock, gimbal_device_id);
            //return Task.FromResult(true);
        }

        private double wrap_180(double angle)
        {
            while (angle > 180)
            {
                angle -= 360;
            }
            while (angle < -180)
            {
                angle += 360;
            }
            return angle;
        }

        public Task<bool> SetAnglesCommandAsync(double pitch, double yaw, bool yaw_lock, byte gimbal_device_id = 0)
        {
            if (!HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.CAN_POINT_LOCATION_LOCAL) ||
                (pitch != 0 && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_PITCH_AXIS)) ||
                (yaw != 0 && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_AXIS)) ||
                (yaw != 0 && yaw_lock && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_LOCK)) ||
                (yaw != 0 && !yaw_lock && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_FOLLOW)))
            {
                return Task.FromResult(false);
            }

            return mavint.doCommandAsync(
                (byte)mavint.sysidcurrent,
                (byte)mavint.compidcurrent,
                MAVLink.MAV_CMD.DO_GIMBAL_MANAGER_PITCHYAW,
                (float)wrap_180(pitch),
                (float)wrap_180(yaw),
                float.NaN, // pitch rate
                float.NaN, // yaw rate
                yaw_lock ? (float)MAVLink.GIMBAL_MANAGER_FLAGS.YAW_LOCK : 0, // flags
                0, // unused
                gimbal_device_id);
        }

        public void SetAnglesStream(float pitch, float yaw, bool yaw_in_earth_frame, byte gimbal_device_id = 0)
        {
            MAVLink.mavlink_gimbal_manager_set_pitchyaw_t set = new MAVLink.mavlink_gimbal_manager_set_pitchyaw_t()
            {
                target_system = (byte)mavint.sysidcurrent,
                target_component = (byte)mavint.compidcurrent,
                gimbal_device_id = gimbal_device_id,
                pitch = pitch,
                yaw = yaw,
                pitch_rate = float.NaN,
                yaw_rate = float.NaN,
                flags = yaw_in_earth_frame ? (uint)MAVLink.GIMBAL_MANAGER_FLAGS.YAW_LOCK : 0
            };
            mavint.sendPacket(set, mavint.sysidcurrent, mavint.compidcurrent);
        }

        public Task<bool> SetRatesCommandAsync(float pitchRate, float yawRate, bool yaw_in_earth_frame, byte gimbal_device_id = 0)
        {
            if ((pitchRate != 0 && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_PITCH_AXIS)) ||
                (yawRate != 0 && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_AXIS)) ||
                (yawRate != 0 && yaw_in_earth_frame && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_LOCK)) ||
                (yawRate != 0 && !yaw_in_earth_frame && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_FOLLOW)))
            {
                return Task.FromResult(false);
            }

            return mavint.doCommandAsync(
                (byte)mavint.sysidcurrent,
                (byte)mavint.compidcurrent,
                MAVLink.MAV_CMD.DO_GIMBAL_MANAGER_PITCHYAW,
                float.NaN, // pitch angle
                float.NaN, // yaw angle
                pitchRate,
                yawRate,
                yaw_in_earth_frame ? (float)MAVLink.GIMBAL_MANAGER_FLAGS.YAW_LOCK : 0, // flags
                0, // unused
                gimbal_device_id);
        }

        public void SetRatesStream(float pitchRate, float yawRate, bool yaw_in_earth_frame, byte gimbal_device_id = 0)
        {
            MAVLink.mavlink_gimbal_manager_set_pitchyaw_t set = new MAVLink.mavlink_gimbal_manager_set_pitchyaw_t()
            {
                target_system = (byte)mavint.sysidcurrent,
                target_component = (byte)mavint.compidcurrent,
                gimbal_device_id = gimbal_device_id,
                pitch = float.NaN,
                yaw = float.NaN,
                pitch_rate = pitchRate,
                yaw_rate = yawRate,
                flags = yaw_in_earth_frame ? (uint)MAVLink.GIMBAL_MANAGER_FLAGS.YAW_LOCK : 0
            };
            mavint.sendPacket(set, mavint.sysidcurrent, mavint.compidcurrent);
        }

        public Task<bool> SetROILocationAsync(double lat, double lon, double alt = 0, byte gimbal_device_id = 0, MAVLink.MAV_FRAME frame = MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT)
        {
            if (!HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.CAN_POINT_LOCATION_GLOBAL))
            {
                return Task.FromResult(false);
            }

            return mavint.doCommandIntAsync(
                (byte)mavint.sysidcurrent,
                (byte)mavint.compidcurrent,
                MAVLink.MAV_CMD.DO_SET_ROI_LOCATION,
                gimbal_device_id,
                0, 0, 0, // unused
                (int)(lat * 1e7),
                (int)(lon * 1e7),
                (float)alt,
                frame: frame);
        }

        public Task<bool> SetROINoneAsync(byte gimbal_device_id = 0)
        {
            return mavint.doCommandAsync(
                (byte)mavint.sysidcurrent,
                (byte)mavint.compidcurrent,
                MAVLink.MAV_CMD.DO_SET_ROI_NONE,
                gimbal_device_id,
                0, 0, 0, 0, 0, 0);
        }

        public Task<bool> SetROISysIDAsync(byte sysid, byte gimbal_device_id = 0)
        {
            if (!HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.CAN_POINT_LOCATION_GLOBAL))
            {
                return Task.FromResult(false);
            }

            return mavint.doCommandAsync(
                (byte)mavint.sysidcurrent,
                (byte)mavint.compidcurrent,
                MAVLink.MAV_CMD.DO_SET_ROI_SYSID,
                sysid,
                gimbal_device_id,
                0, 0, 0, 0, 0);
        }
    }
}
