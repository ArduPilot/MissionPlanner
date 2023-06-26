using System;
using System.Collections.Generic;
using System.Text;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot.Mavlink
{
    public class GimbalProtocol
    {
        //Multiple component IDs are reserved for gimbal devices: MAV_COMP_ID_GIMBAL, MAV_COMP_ID_GIMBAL2, MAV_COMP_ID_GIMBAL3, MAV_COMP_ID_GIMBAL4, MAV_COMP_ID_GIMBAL5, MAV_COMP_ID_GIMBAL6

        //gsdk - gimbal
        //MAVLINK_MSG_ID_PARAM_VALUE
        //MAVLINK_MSG_ID_COMMAND_ACK
        //MAVLINK_MSG_ID_GIMBAL_DEVICE_INFORMATION
        //MAVLINK_MSG_ID_RAW_IMU
        //MAVLINK_MSG_ID_GIMBAL_DEVICE_ATTITUDE_STATUS
        //MAVLINK_MSG_ID_MOUNT_ORIENTATION
        //MAVLINK_MSG_ID_MOUNT_STATUS
        //MAVLINK_MSG_ID_SYS_STATUS
        //MAVLINK_MSG_ID_HEARTBEAT

        public static Dictionary<ulong, MAVLink.MAVLinkMessage> List =
            new Dictionary<ulong, MAVLink.MAVLinkMessage>();

        public void Discover(MAVLinkInterface mint)
        {
            mint.doCommand(0, 0, MAVLink.MAV_CMD.REQUEST_MESSAGE,
                (float)MAVLink.MAVLINK_MSG_ID.GIMBAL_DEVICE_INFORMATION,
                0, 0, 0, 0, 0, 0, false);

            mint.OnPacketReceived += (sender, message) =>
            {
                if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.GIMBAL_DEVICE_INFORMATION)
                {
                    var gi = (MAVLink.mavlink_gimbal_device_information_t)message.data;

                    lock (List)
                    {
                        List[gi.uid] = message;
                    }
                }
            };
        }

        public bool Reboot(MAVLinkInterface mint, byte sysid, byte compid)
        {
            return mint.doCommand(sysid, compid, MAVLink.MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN, 0, 0, 0, 1, 0, 0, 0);
        }

        public bool SetRCMode(MAVLinkInterface mint, byte sysid, byte compid)
        {
            return mint.doCommand(sysid, compid, MAVLink.MAV_CMD.DO_MOUNT_CONFIGURE,
                (int)MAVLink.MAV_MOUNT_MODE.RC_TARGETING, 0, 0,
                0, 0, 0, 0);
        }

        public bool SetMotorState(MAVLinkInterface mint, byte sysid, byte compid, control_motor_t type)
        {
            return mint.doCommand(sysid, compid, MAVLink.MAV_CMD.USER_1, 0, 0, 0,
                0, 0, 0, (byte)type);
        }

        public void GetGimbalMode(MAVLinkInterface mint, byte sysid, byte compid)
        {
            var ss = mint.MAVlist[sysid, compid].getPacketLast((uint)MAVLink.MAVLINK_MSG_ID.SYS_STATUS);
            if (ss != null)
            {
                var sc = (MAVLink.mavlink_sys_status_t)ss.data;
                /* Check gimbal's motor */
                if ((sc.errors_count1 & (ushort)status1_t.STATUS1_MOTORS) > 0)
                {
                    _status.state = (byte)operation_state_t.GIMBAL_STATE_ON;

                    /* Check gimbal is follow mode*/
                    if ((sc.errors_count1 & (ushort)status1_t.STATUS1_MODE_FOLLOW_LOCK) > 0)
                    {
                        _status.mode = (byte)control_mode_t.GIMBAL_FOLLOW_MODE;

                    }
                    else
                    {
                        _status.mode = (byte)control_mode_t.GIMBAL_LOCK_MODE;
                    }
                }
                else
                {
                    _status.state = (byte)operation_state_t.GIMBAL_STATE_OFF;
                }
            }
        }

        public void SetResetMode(MAVLinkInterface mint, byte sysid, byte compid, gimbal_reset_mode_t reset_mode)
        {
            float pitch = 0; //_attitude.pitch;
            float roll = 0; //_attitude.roll;
            float yaw = 0; //_attitude.yaw;

            switch (reset_mode)
            {
                case gimbal_reset_mode_t.GIMBAL_RESET_MODE_YAW:
                    yaw = 0f;
                    break;

                case gimbal_reset_mode_t.GIMBAL_RESET_MODE_PITCH_AND_YAW:
                    pitch = 0f;
                    roll = 0f;
                    yaw = 0f;
                    break;

                case gimbal_reset_mode_t.GIMBAL_RESET_MODE_PITCH_DOWNWARD_UPWARD_AND_YAW:
                    pitch = -90f;
                    roll = 0f;
                    yaw = 0f;
                    break;

                case gimbal_reset_mode_t.GIMBAL_RESET_MODE_PITCH_DOWNWARD_UPWARD:
                    pitch = -90f;
                    break;

                case gimbal_reset_mode_t.GIMBAL_RESET_MODE_PITCH_MAPPING:
                    pitch = -90f;
                    break;

                default:
                    break;
            }

            /* Switch to follow */
            _control_mode = control_mode_t.GIMBAL_FOLLOW_MODE;
            SetOrientation(mint, sysid, compid, pitch, roll, yaw, input_mode_t.INPUT_ANGLE);
        }

        public void SetOrientation(MAVLinkInterface mint, byte sysid, byte compid, float pitch, float roll, float yaw,
            input_mode_t mode)
        {
            MAVLink.mavlink_gimbal_device_set_attitude_t attitude;
            attitude.target_system = sysid;
            attitude.target_component = compid;
            attitude.flags = (ushort)(MAVLink.GIMBAL_DEVICE_FLAGS.ROLL_LOCK | MAVLink.GIMBAL_DEVICE_FLAGS.PITCH_LOCK |
                                      ((_control_mode == control_mode_t.GIMBAL_LOCK_MODE)
                                          ? MAVLink.GIMBAL_DEVICE_FLAGS.YAW_LOCK
                                          : (MAVLink.GIMBAL_DEVICE_FLAGS)0));

            if (mode == input_mode_t.INPUT_ANGLE)
            {
                attitude.q = new float[4];
                /* Convert target to quaternion */
                mavlink_euler_to_quaternion((roll) * MathHelper.deg2radf, (pitch) * MathHelper.deg2radf,
                    (yaw) * MathHelper.deg2radf, attitude.q);
                attitude.angular_velocity_x = float.NaN;
                attitude.angular_velocity_y = float.NaN;
                attitude.angular_velocity_z = float.NaN;

            }
            else
            {
                attitude.angular_velocity_x = roll * MathHelper.deg2radf;
                attitude.angular_velocity_y = pitch * MathHelper.deg2radf;
                attitude.angular_velocity_z = yaw * MathHelper.deg2radf;
                attitude.q = new float[4];
                attitude.q[0] = float.NaN;
                attitude.q[1] = float.NaN;
                attitude.q[2] = float.NaN;
                attitude.q[3] = float.NaN;
            }
        }

        /**
 * Converts euler angles to a quaternion
 *
 * @param roll the roll angle in radians
 * @param pitch the pitch angle in radians
 * @param yaw the yaw angle in radians
 * @param quaternion a [w, x, y, z] ordered quaternion (null-rotation being 1 0 0 0)
 */
        public void mavlink_euler_to_quaternion(float roll, float pitch, float yaw, float[] quaternion)
        {
            float cosPhi_2 = Utils.cosf(roll / 2);
            float sinPhi_2 = Utils.sinf(roll / 2);
            float cosTheta_2 = Utils.cosf(pitch / 2);
            float sinTheta_2 = Utils.sinf(pitch / 2);
            float cosPsi_2 = Utils.cosf(yaw / 2);
            float sinPsi_2 = Utils.sinf(yaw / 2);
            quaternion[0] = (cosPhi_2 * cosTheta_2 * cosPsi_2 +
                             sinPhi_2 * sinTheta_2 * sinPsi_2);
            quaternion[1] = (sinPhi_2 * cosTheta_2 * cosPsi_2 -
                             cosPhi_2 * sinTheta_2 * sinPsi_2);
            quaternion[2] = (cosPhi_2 * sinTheta_2 * cosPsi_2 +
                             sinPhi_2 * cosTheta_2 * sinPsi_2);
            quaternion[3] = (cosPhi_2 * cosTheta_2 * sinPsi_2 -
                             sinPhi_2 * sinTheta_2 * cosPsi_2);
        }

        /**
 * @brief Input mode control gimbal
 * 
 */
        public enum input_mode_t
        {
            INPUT_ANGLE = 1,
            INPUT_SPEED = 2
        };

        /**
   * @brief Reset mode of gimbal.
   */
        public enum gimbal_reset_mode_t
        {
            /*! Only reset yaw axis of gimbal. Reset angle of yaw axis to the sum of yaw axis angle of aircraft and fine tune angle
            * of yaw axis of gimbal. */
            GIMBAL_RESET_MODE_YAW = 1,

            /*! Reset yaw axis and pitch axis of gimbal. Reset angle of yaw axis to sum of yaw axis angle of aircraft and fine tune 
            * angle of yaw axis of gimbal, and reset pitch axis angle to the fine tune angle. */
            GIMBAL_RESET_MODE_PITCH_AND_YAW = 3,

            /*! Reset yaw axis and pitch axis of gimbal. Reset angle of yaw axis to sum of yaw axis angle of aircraft and fine tune
            * angle of yaw axis of gimbal, and reset pitch axis angle to sum of -90 degree and fine tune angle if gimbal
            * downward, sum of 90 degree and fine tune angle if upward. */
            GIMBAL_RESET_MODE_PITCH_DOWNWARD_UPWARD_AND_YAW = 11,

            /*! Reset pitch axis of gimbal. Reset pitch axis angle to sum of -90 degree and fine tune angle if gimbal downward,
            * sum of 90 degree and fine tune angle if upward. */
            GIMBAL_RESET_MODE_PITCH_DOWNWARD_UPWARD = 12,

            /*! Reset pitch axis of gimbal. Reset pitch axis angle to mapping angle */
            GIMBAL_RESET_MODE_PITCH_MAPPING = 13,
        };

        /**
         * @brief Control gimbal Mode
         * @details Mode LOCK/FOLLOW/RESET MODE
         */
        public enum control_mode_t
        {
            GIMBAL_OFF = 0x00,
            GIMBAL_LOCK_MODE = 0x01,
            GIMBAL_FOLLOW_MODE = 0x02,
            GIMBAL_RESET_MODE = 0x04
        };

        gimbal_status_t _status;
        private control_mode_t _control_mode;

        /**
         * @brief Gimbal Status
         */
        public struct gimbal_status_t
        {
            public ushort
                load; /*< [ms] Maximum usage the mainloop time. Values: [0-1000] - should always be below 1000*/

            public ushort voltage_battery; /*< [V] Battery voltage*/
            public byte sensor; /*< Specific sensor occur error (encorder, imu) refer sensor_state_t*/
            public byte state; /* System state of gimbal. Refer interface_state_t*/
            public byte mode;
        };

        // Gimbal status 1
        [Flags]
        public enum status1_t : ushort
        {
            STATUS1_ERROR_NONE = 0x00,
            STATUS1_MODE_FOLLOW_LOCK = 0x01,
            STATUS1_MISS_STEP = 0x02,
            STATUS1_SENSOR_ERROR = 0x04,
            STATUS1_BATT_LOW = 0x08,
            STATUS1_MOTORS = 0x10,

            /// motors on = 1, motor off = 0 (fimware 1.3.4)*/
            STATUS1_INIT_MOTOR = 0x20,
            STATUS1_AUTO_TUNER = 0x40,

            /// 0b0100 0000
            STATUS1_CANLINK = 0x80,

            /// 0b1000 0000 ket noi can link.
            STATUS1_SEARCH_HOME = 0x100,

            /// search home
            STATUS1_SET_HOME = 0x200,

            /// set home
            STATUS1_SENSOR_CALIB = 0x400,

            /// calib sensor gom accel va gyro
            STATUS1_STARTUP = 0x800,
            STATUS1_REMOTE = 0x1000,
            STATUS1_INVERTED = 0x2000,
            STATUS1_MOTOR_PHASE_ERROR = 0x4000,
            STATUS1_MOTOR_ANGLE_ERROR = 0x8000,
        };

        // Gimbal status 2
        [Flags]
        public enum status2_t : ushort
        {
            STATUS2_ERROR_NONE = 0x00,
            STATUS2_IMU_ERROR = 0x01,
            STATUS2_MOTOR_TILT_ERROR = 0x02,
            STATUS2_MOTOR_ROLL_ERROR = 0x04,
            STATUS2_MOTOR_PAN_ERROR = 0x08,
            STATUS2_JOYSTICK_ERROR = 0x10,
            STATUS2_INVERTED_ERROR = 0x20,
            STATUS2_PAN_SEARCH_HOME_ERROR = 0x40,

            STATUS2_ANGLE_TILT_ERROR = 0x80,
            STATUS2_ANGLE_ROLL_ERROR = 0x100,
            STATUS2_ANGLE_PAN_ERROR = 0x200,

            STATUS2_MOVE_TILT_ERROR = 0x400,
            STATUS2_MOVE_ROLL_ERROR = 0x800,
            STATUS2_MOVE_PAN_ERROR = 0x1000,
        };

        /**
         * @brief Control direction
         */
        public enum control_direction_t
        {
            DIR_CW = 0x00,
            DIR_CCW = 0x01
        };

        /**
         * @brief Control motor mode ON/OFF
         */
        public enum control_motor_t
        {
            TURN_OFF = 0x00,
            TURN_ON = 0x01
        };

        /**
         * @brief Gimbal Operation status
         */
        public enum operation_state_t : byte
        {
            GIMBAL_STATE_OFF = 0x00, /*< Gimbal is off*/
            GIMBAL_STATE_INIT = 0x01, /*< Gimbal is initializing*/
            GIMBAL_STATE_ON = 0x02, /*< Gimbal is on */
            GIMBAL_STATE_LOCK_MODE = 0x04,
            GIMBAL_STATE_FOLLOW_MODE = 0x08,
            GIMBAL_STATE_SEARCH_HOME = 0x10,
            GIMBAL_STATE_SET_HOME = 0x20,
            GIMBAL_STATE_ERROR = 0x40
        };
    }
}