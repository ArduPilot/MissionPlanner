using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot.Mavlink
{
    public class GimbalManagerProtocol
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CurrentState cs;
        private readonly MAVLinkInterface mavint;

        // Stores the last GIMBAL_MANAGER_INFORMATION message for each gimbal device/component ID.
        // This index will be 1-6, or MAVLink component IDs 154, 171-175.
        // Index 0 is used to store the message of the first (lowest) gimbal ID.
        public ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_manager_information_t> ManagerInfo =
            new ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_manager_information_t>();

        // Stores the GIMBAL_DEVICE_ATTITUDE_STATUS message for each gimbal device/component ID.
        // This index will be 1-6, or MAVLink component IDs 154, 171-175.
        // Index 0 is used to store the message of the first (lowest) gimbal ID.
        public ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_device_attitude_status_t> GimbalStatus =
            new ConcurrentDictionary<byte, MAVLink.mavlink_gimbal_device_attitude_status_t>();

        private bool have_gimbal_manager_information = false;
        private int _started;

        // Uncomment the Console.WriteLine line to enable debug output.
        private static void DebugConsoleWrite(string format, params object[] args)
        {
            // Console.WriteLine(format, args);
        }

        public GimbalManagerProtocol(MAVLinkInterface mavint, CurrentState cs)
        {
            this.mavint = mavint;
            this.cs = cs;
        }

        /// <summary>
        /// Initializes the gimbal manager protocol and begins GIMBAL_MANAGER_INFORMATION discovery.
        /// </summary>
        /// <remarks>
        /// Sends fire-and-forget SET_MESSAGE_INTERVAL + GET_MESSAGE_INTERVAL for
        /// GIMBAL_MANAGER_INFORMATION, then watches the MESSAGE_INTERVAL reply to confirm.
        /// Retries up to 3 times; stops on success, interval_us == 0 (not supported),
        /// or if GIMBAL_MANAGER_INFORMATION arrives in the meantime.
        /// </remarks>
        /// <param name="sysid">Target system ID.</param>
        /// <param name="compid">Target component ID.</param>
        public async Task StartID(byte sysid, byte compid)
        {
            if (Interlocked.Exchange(ref _started, 1) != 0)
                return;

            mavint.OnPacketReceived += MessagesHandler;

            const ushort msgId = (ushort)MAVLink.MAVLINK_MSG_ID.GIMBAL_MANAGER_INFORMATION;
            const float intervalUs = 30_000_000; // 30 s

            bool confirmed = false;
            var sub = mavint.SubscribeToPacketType(
                MAVLink.MAVLINK_MSG_ID.MESSAGE_INTERVAL,
                msg =>
                {
                    var data = msg.ToStructure<MAVLink.mavlink_message_interval_t>();
                    if (data.message_id != msgId)
                        return true;

                    if (data.interval_us == 0)
                        log.Info("GimbalManager: GIMBAL_MANAGER_INFORMATION not supported (interval_us=0)");
                    else
                        log.InfoFormat("GimbalManager: GIMBAL_MANAGER_INFORMATION interval confirmed at {0} us", data.interval_us);

                    confirmed = true;
                    return true;
                },
                sysid, compid);

            try
            {
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    if (have_gimbal_manager_information || confirmed)
                        break;

                    try
                    {
                        _ = mavint.doCommandAsync(sysid, compid,
                            MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                            msgId, intervalUs,
                            0, 0, 0, 0, 0, false);

                        _ = mavint.doCommandAsync(sysid, compid,
                            MAVLink.MAV_CMD.GET_MESSAGE_INTERVAL,
                            msgId,
                            0, 0, 0, 0, 0, 0, false);
                    }
                    catch (Exception ex)
                    {
                        log.Debug("GimbalManager: SET/GET_MESSAGE_INTERVAL failed: " + ex.Message);
                    }

                    await Task.Delay(5000).ConfigureAwait(false);
                }
            }
            finally
            {
                mavint.UnSubscribeToPacketType(sub);
            }
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

                have_gimbal_manager_information = true;
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

        /// <summary>
        /// Gets the reported attitude of the gimbal in the earth frame.
        /// </summary>
        /// <param name="gimbal_device_id">Device ID of the gimbal. 0 means the first gimbal.</param>
        /// <returns>The gimbal attitude quaternion, or <c>null</c> if no status has been received.</returns>
        public Quaternion GetAttitude(byte gimbal_device_id = 0)
        {
            if (!GimbalStatus.TryGetValue(gimbal_device_id, out var status))
            {
                return null;
            }

            var q = new Quaternion(status.q[0], status.q[1], status.q[2], status.q[3]);

            // Rotate from vehicle frame to earth frame if needed.
            var flags = (MAVLink.GIMBAL_DEVICE_FLAGS)status.flags;
            bool yaw_in_earth = (flags & MAVLink.GIMBAL_DEVICE_FLAGS.YAW_IN_EARTH_FRAME) != 0;
            bool yaw_in_vehicle = (flags & MAVLink.GIMBAL_DEVICE_FLAGS.YAW_IN_VEHICLE_FRAME) != 0;

            // Older protocols may set neither flag; infer from YAW_LOCK.
            if (!yaw_in_earth && !yaw_in_vehicle)
                yaw_in_vehicle = (flags & MAVLink.GIMBAL_DEVICE_FLAGS.YAW_LOCK) == 0;

            if (yaw_in_vehicle)
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
        /// Commands the gimbal to a given attitude.
        /// </summary>
        /// <param name="q">Desired attitude quaternion in the earth frame.</param>
        /// <param name="yaw_lock">
        /// <c>true</c> to hold the earth-frame yaw; <c>false</c> to follow vehicle yaw.
        /// </param>
        /// <param name="gimbal_device_id">Device ID of the gimbal. 0 means the first gimbal.</param>
        /// <returns><c>true</c> if the command was acknowledged; otherwise, <c>false</c>.</returns>
        public Task<bool> SetAttitudeAsync(Quaternion q, bool yaw_lock, byte gimbal_device_id = 0)
        {
            var pitch = q.get_euler_pitch() * MathHelper.rad2deg;
            var yaw = q.get_euler_yaw() * MathHelper.rad2deg;

            if (!yaw_lock)
            {
                yaw -= cs.yaw;
            }

            DebugConsoleWrite("SetAttitudeAsync: pitch={0}, yaw={1}, yaw_lock={2}", pitch, yaw < 0 ? yaw + 360 : yaw, yaw_lock);
            return SetAnglesCommandAsync(pitch, yaw, yaw_lock, gimbal_device_id);
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
