using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MissionPlanner.ArduPilot.Mavlink
{
    public class GimbalManagerProtocol
    {
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

        public GimbalManagerProtocol(MAVLinkInterface mavint)
        {
            this.mavint = mavint;
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

        public Task<bool> SetAnglesCommandAsync(float pitch, float yaw, bool yaw_in_earth_frame, byte gimbal_device_id = 0)
        {
            if (!HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.CAN_POINT_LOCATION_LOCAL) ||
                (pitch != 0 && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_PITCH_AXIS)) ||
                (yaw != 0 && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_AXIS)) ||
                (yaw != 0 && yaw_in_earth_frame && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_LOCK)) ||
                (yaw != 0 && !yaw_in_earth_frame && !HasCapability(MAVLink.GIMBAL_MANAGER_CAP_FLAGS.HAS_YAW_FOLLOW)))
            {
                return Task.FromResult(false);
            }

            return mavint.doCommandAsync(
                (byte)mavint.sysidcurrent,
                (byte)mavint.compidcurrent,
                MAVLink.MAV_CMD.DO_GIMBAL_MANAGER_PITCHYAW,
                pitch,
                yaw,
                float.NaN, // pitch rate
                float.NaN, // yaw rate
                yaw_in_earth_frame ? (float)MAVLink.GIMBAL_MANAGER_FLAGS.YAW_LOCK : 0, // flags
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
