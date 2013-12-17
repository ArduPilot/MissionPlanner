using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public partial class MAVLink
{
        public const string MAVLINK_BUILD_DATE = "Wed Dec 11 06:37:07 2013";
        public const string MAVLINK_WIRE_PROTOCOL_VERSION = "1.0";
        public const int MAVLINK_MAX_DIALECT_PAYLOAD_SIZE = 254;

        public const int MAVLINK_LITTLE_ENDIAN = 1;
        public const int MAVLINK_BIG_ENDIAN = 0;

        public const byte MAVLINK_STX = 254;

        public const byte MAVLINK_ENDIAN = MAVLINK_LITTLE_ENDIAN;

        public const bool MAVLINK_ALIGNED_FIELDS = (1 == 1);

        public const byte MAVLINK_CRC_EXTRA = 1;
        
        public const bool MAVLINK_NEED_BYTE_SWAP = (MAVLINK_ENDIAN == MAVLINK_LITTLE_ENDIAN);
        
        public static readonly byte[] MAVLINK_MESSAGE_LENGTHS = new byte[] {9, 31, 12, 0, 14, 28, 3, 32, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 20, 2, 25, 23, 30, 101, 22, 26, 16, 14, 28, 32, 28, 28, 22, 22, 21, 6, 6, 37, 4, 4, 2, 2, 4, 2, 2, 3, 13, 12, 19, 17, 15, 15, 27, 25, 18, 18, 20, 20, 9, 34, 26, 46, 36, 0, 6, 4, 0, 11, 18, 0, 0, 0, 20, 0, 33, 3, 0, 0, 20, 22, 0, 0, 0, 0, 0, 0, 0, 28, 56, 42, 33, 0, 0, 0, 0, 0, 0, 0, 26, 32, 32, 20, 32, 62, 54, 64, 84, 9, 254, 249, 9, 36, 26, 64, 22, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 24, 33, 25, 42, 8, 4, 12, 15, 13, 6, 15, 14, 0, 12, 3, 8, 28, 44, 3, 9, 22, 12, 18, 34, 66, 98, 8, 48, 19, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 36, 30, 18, 18, 51, 9, 0};

        public static readonly byte[] MAVLINK_MESSAGE_CRCS = new byte[] {50, 124, 137, 0, 237, 217, 104, 119, 0, 0, 0, 89, 0, 0, 0, 0, 0, 0, 0, 0, 214, 159, 220, 168, 24, 23, 170, 144, 67, 115, 39, 246, 185, 104, 237, 244, 222, 212, 9, 254, 230, 28, 28, 132, 221, 232, 11, 153, 41, 39, 214, 223, 141, 33, 15, 3, 100, 24, 239, 238, 30, 240, 183, 130, 130, 0, 148, 21, 0, 243, 124, 0, 0, 0, 20, 0, 152, 143, 0, 0, 127, 106, 0, 0, 0, 0, 0, 0, 0, 231, 183, 63, 54, 0, 0, 0, 0, 0, 0, 0, 175, 102, 158, 208, 56, 93, 211, 108, 32, 185, 235, 93, 124, 124, 119, 4, 76, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 241, 15, 134, 219, 208, 188, 84, 22, 19, 21, 134, 0, 78, 68, 189, 127, 154, 21, 21, 144, 1, 234, 73, 181, 22, 83, 167, 138, 234, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 204, 49, 170, 44, 83, 46, 0};

        public static readonly Type[] MAVLINK_MESSAGE_INFO = new Type[] {typeof( mavlink_heartbeat_t ), typeof( mavlink_sys_status_t ), typeof( mavlink_system_time_t ), null, typeof( mavlink_ping_t ), typeof( mavlink_change_operator_control_t ), typeof( mavlink_change_operator_control_ack_t ), typeof( mavlink_auth_key_t ), null, null, null, typeof( mavlink_set_mode_t ), null, null, null, null, null, null, null, null, typeof( mavlink_param_request_read_t ), typeof( mavlink_param_request_list_t ), typeof( mavlink_param_value_t ), typeof( mavlink_param_set_t ), typeof( mavlink_gps_raw_int_t ), typeof( mavlink_gps_status_t ), typeof( mavlink_scaled_imu_t ), typeof( mavlink_raw_imu_t ), typeof( mavlink_raw_pressure_t ), typeof( mavlink_scaled_pressure_t ), typeof( mavlink_attitude_t ), typeof( mavlink_attitude_quaternion_t ), typeof( mavlink_local_position_ned_t ), typeof( mavlink_global_position_int_t ), typeof( mavlink_rc_channels_scaled_t ), typeof( mavlink_rc_channels_raw_t ), typeof( mavlink_servo_output_raw_t ), typeof( mavlink_mission_request_partial_list_t ), typeof( mavlink_mission_write_partial_list_t ), typeof( mavlink_mission_item_t ), typeof( mavlink_mission_request_t ), typeof( mavlink_mission_set_current_t ), typeof( mavlink_mission_current_t ), typeof( mavlink_mission_request_list_t ), typeof( mavlink_mission_count_t ), typeof( mavlink_mission_clear_all_t ), typeof( mavlink_mission_item_reached_t ), typeof( mavlink_mission_ack_t ), typeof( mavlink_set_gps_global_origin_t ), typeof( mavlink_gps_global_origin_t ), typeof( mavlink_set_local_position_setpoint_t ), typeof( mavlink_local_position_setpoint_t ), typeof( mavlink_global_position_setpoint_int_t ), typeof( mavlink_set_global_position_setpoint_int_t ), typeof( mavlink_safety_set_allowed_area_t ), typeof( mavlink_safety_allowed_area_t ), typeof( mavlink_set_roll_pitch_yaw_thrust_t ), typeof( mavlink_set_roll_pitch_yaw_speed_thrust_t ), typeof( mavlink_roll_pitch_yaw_thrust_setpoint_t ), typeof( mavlink_roll_pitch_yaw_speed_thrust_setpoint_t ), typeof( mavlink_set_quad_motors_setpoint_t ), typeof( mavlink_set_quad_swarm_roll_pitch_yaw_thrust_t ), typeof( mavlink_nav_controller_output_t ), typeof( mavlink_set_quad_swarm_led_roll_pitch_yaw_thrust_t ), typeof( mavlink_state_correction_t ), null, typeof( mavlink_request_data_stream_t ), typeof( mavlink_data_stream_t ), null, typeof( mavlink_manual_control_t ), typeof( mavlink_rc_channels_override_t ), null, null, null, typeof( mavlink_vfr_hud_t ), null, typeof( mavlink_command_long_t ), typeof( mavlink_command_ack_t ), null, null, typeof( mavlink_roll_pitch_yaw_rates_thrust_setpoint_t ), typeof( mavlink_manual_setpoint_t ), null, null, null, null, null, null, null, typeof( mavlink_local_position_ned_system_global_offset_t ), typeof( mavlink_hil_state_t ), typeof( mavlink_hil_controls_t ), typeof( mavlink_hil_rc_inputs_raw_t ), null, null, null, null, null, null, null, typeof( mavlink_optical_flow_t ), typeof( mavlink_global_vision_position_estimate_t ), typeof( mavlink_vision_position_estimate_t ), typeof( mavlink_vision_speed_estimate_t ), typeof( mavlink_vicon_position_estimate_t ), typeof( mavlink_highres_imu_t ), typeof( mavlink_omnidirectional_flow_t ), typeof( mavlink_hil_sensor_t ), typeof( mavlink_sim_state_t ), typeof( mavlink_radio_status_t ), typeof( mavlink_file_transfer_start_t ), typeof( mavlink_file_transfer_dir_list_t ), typeof( mavlink_file_transfer_res_t ), typeof( mavlink_hil_gps_t ), typeof( mavlink_hil_optical_flow_t ), typeof( mavlink_hil_state_quaternion_t ), typeof( mavlink_scaled_imu2_t ), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, typeof( mavlink_battery_status_t ), typeof( mavlink_setpoint_8dof_t ), typeof( mavlink_setpoint_6dof_t ), typeof( mavlink_sensor_offsets_t ), typeof( mavlink_set_mag_offsets_t ), typeof( mavlink_meminfo_t ), typeof( mavlink_ap_adc_t ), typeof( mavlink_digicam_configure_t ), typeof( mavlink_digicam_control_t ), typeof( mavlink_mount_configure_t ), typeof( mavlink_mount_control_t ), typeof( mavlink_mount_status_t ), null, typeof( mavlink_fence_point_t ), typeof( mavlink_fence_fetch_point_t ), typeof( mavlink_fence_status_t ), typeof( mavlink_ahrs_t ), typeof( mavlink_simstate_t ), typeof( mavlink_hwstatus_t ), typeof( mavlink_radio_t ), typeof( mavlink_limits_status_t ), typeof( mavlink_wind_t ), typeof( mavlink_data16_t ), typeof( mavlink_data32_t ), typeof( mavlink_data64_t ), typeof( mavlink_data96_t ), typeof( mavlink_rangefinder_t ), typeof( mavlink_airspeed_autocal_t ), typeof( mavlink_rally_point_t ), typeof( mavlink_rally_fetch_point_t ), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, typeof( mavlink_memory_vect_t ), typeof( mavlink_debug_vect_t ), typeof( mavlink_named_value_float_t ), typeof( mavlink_named_value_int_t ), typeof( mavlink_statustext_t ), typeof( mavlink_debug_t ), null};

        public const byte MAVLINK_VERSION = 2;

		public enum MAVLINK_MSG_ID 
		{
			HEARTBEAT = 0,
SYS_STATUS = 1,
SYSTEM_TIME = 2,
PING = 4,
CHANGE_OPERATOR_CONTROL = 5,
CHANGE_OPERATOR_CONTROL_ACK = 6,
AUTH_KEY = 7,
SET_MODE = 11,
PARAM_REQUEST_READ = 20,
PARAM_REQUEST_LIST = 21,
PARAM_VALUE = 22,
PARAM_SET = 23,
GPS_RAW_INT = 24,
GPS_STATUS = 25,
SCALED_IMU = 26,
RAW_IMU = 27,
RAW_PRESSURE = 28,
SCALED_PRESSURE = 29,
ATTITUDE = 30,
ATTITUDE_QUATERNION = 31,
LOCAL_POSITION_NED = 32,
GLOBAL_POSITION_INT = 33,
RC_CHANNELS_SCALED = 34,
RC_CHANNELS_RAW = 35,
SERVO_OUTPUT_RAW = 36,
MISSION_REQUEST_PARTIAL_LIST = 37,
MISSION_WRITE_PARTIAL_LIST = 38,
MISSION_ITEM = 39,
MISSION_REQUEST = 40,
MISSION_SET_CURRENT = 41,
MISSION_CURRENT = 42,
MISSION_REQUEST_LIST = 43,
MISSION_COUNT = 44,
MISSION_CLEAR_ALL = 45,
MISSION_ITEM_REACHED = 46,
MISSION_ACK = 47,
SET_GPS_GLOBAL_ORIGIN = 48,
GPS_GLOBAL_ORIGIN = 49,
SET_LOCAL_POSITION_SETPOINT = 50,
LOCAL_POSITION_SETPOINT = 51,
GLOBAL_POSITION_SETPOINT_INT = 52,
SET_GLOBAL_POSITION_SETPOINT_INT = 53,
SAFETY_SET_ALLOWED_AREA = 54,
SAFETY_ALLOWED_AREA = 55,
SET_ROLL_PITCH_YAW_THRUST = 56,
SET_ROLL_PITCH_YAW_SPEED_THRUST = 57,
ROLL_PITCH_YAW_THRUST_SETPOINT = 58,
ROLL_PITCH_YAW_SPEED_THRUST_SETPOINT = 59,
SET_QUAD_MOTORS_SETPOINT = 60,
SET_QUAD_SWARM_ROLL_PITCH_YAW_THRUST = 61,
NAV_CONTROLLER_OUTPUT = 62,
SET_QUAD_SWARM_LED_ROLL_PITCH_YAW_THRUST = 63,
STATE_CORRECTION = 64,
REQUEST_DATA_STREAM = 66,
DATA_STREAM = 67,
MANUAL_CONTROL = 69,
RC_CHANNELS_OVERRIDE = 70,
VFR_HUD = 74,
COMMAND_LONG = 76,
COMMAND_ACK = 77,
ROLL_PITCH_YAW_RATES_THRUST_SETPOINT = 80,
MANUAL_SETPOINT = 81,
LOCAL_POSITION_NED_SYSTEM_GLOBAL_OFFSET = 89,
HIL_STATE = 90,
HIL_CONTROLS = 91,
HIL_RC_INPUTS_RAW = 92,
OPTICAL_FLOW = 100,
GLOBAL_VISION_POSITION_ESTIMATE = 101,
VISION_POSITION_ESTIMATE = 102,
VISION_SPEED_ESTIMATE = 103,
VICON_POSITION_ESTIMATE = 104,
HIGHRES_IMU = 105,
OMNIDIRECTIONAL_FLOW = 106,
HIL_SENSOR = 107,
SIM_STATE = 108,
RADIO_STATUS = 109,
FILE_TRANSFER_START = 110,
FILE_TRANSFER_DIR_LIST = 111,
FILE_TRANSFER_RES = 112,
HIL_GPS = 113,
HIL_OPTICAL_FLOW = 114,
HIL_STATE_QUATERNION = 115,
SCALED_IMU2 = 116,
BATTERY_STATUS = 147,
SETPOINT_8DOF = 148,
SETPOINT_6DOF = 149,
SENSOR_OFFSETS = 150,
SET_MAG_OFFSETS = 151,
MEMINFO = 152,
AP_ADC = 153,
DIGICAM_CONFIGURE = 154,
DIGICAM_CONTROL = 155,
MOUNT_CONFIGURE = 156,
MOUNT_CONTROL = 157,
MOUNT_STATUS = 158,
FENCE_POINT = 160,
FENCE_FETCH_POINT = 161,
FENCE_STATUS = 162,
AHRS = 163,
SIMSTATE = 164,
HWSTATUS = 165,
RADIO = 166,
LIMITS_STATUS = 167,
WIND = 168,
DATA16 = 169,
DATA32 = 170,
DATA64 = 171,
DATA96 = 172,
RANGEFINDER = 173,
AIRSPEED_AUTOCAL = 174,
RALLY_POINT = 175,
RALLY_FETCH_POINT = 176,
MEMORY_VECT = 249,
DEBUG_VECT = 250,
NAMED_VALUE_FLOAT = 251,
NAMED_VALUE_INT = 252,
STATUSTEXT = 253,
DEBUG = 254,

		}

    
        
        ///<summary> Enumeration of possible mount operation modes </summary>
        public enum MAV_MOUNT_MODE
        {
    	///<summary> Load and keep safe position (Roll,Pitch,Yaw) from EEPROM and stop stabilization | </summary>
            RETRACT=0, 
        	///<summary> Load and keep neutral position (Roll,Pitch,Yaw) from EEPROM. | </summary>
            NEUTRAL=1, 
        	///<summary> Load neutral position and start MAVLink Roll,Pitch,Yaw control with stabilization | </summary>
            MAVLINK_TARGETING=2, 
        	///<summary> Load neutral position and start RC Roll,Pitch,Yaw control with stabilization | </summary>
            RC_TARGETING=3, 
        	///<summary> Load neutral position and start to point to Lat,Lon,Alt | </summary>
            GPS_POINT=4, 
        	///<summary>  | </summary>
            ENUM_END=5, 
        
        };
        
        ///<summary>  </summary>
        public enum MAV_CMD
        {
    	///<summary> Navigate to MISSION. |Hold time in decimal seconds. (ignored by fixed wing, time to stay at MISSION for rotary wing)| Acceptance radius in meters (if the sphere with this radius is hit, the MISSION counts as reached)| 0 to pass through the WP, if > 0 radius in meters to pass by WP. Positive value for clockwise orbit, negative value for counter-clockwise orbit. Allows trajectory control.| Desired yaw angle at MISSION (rotary wing)| Latitude| Longitude| Altitude|  </summary>
            WAYPOINT=16, 
        	///<summary> Loiter around this MISSION an unlimited amount of time |Empty| Empty| Radius around MISSION, in meters. If positive loiter clockwise, else counter-clockwise| Desired yaw angle.| Latitude| Longitude| Altitude|  </summary>
            LOITER_UNLIM=17, 
        	///<summary> Loiter around this MISSION for X turns |Turns| Empty| Radius around MISSION, in meters. If positive loiter clockwise, else counter-clockwise| Desired yaw angle.| Latitude| Longitude| Altitude|  </summary>
            LOITER_TURNS=18, 
        	///<summary> Loiter around this MISSION for X seconds |Seconds (decimal)| Empty| Radius around MISSION, in meters. If positive loiter clockwise, else counter-clockwise| Desired yaw angle.| Latitude| Longitude| Altitude|  </summary>
            LOITER_TIME=19, 
        	///<summary> Return to launch location |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            RETURN_TO_LAUNCH=20, 
        	///<summary> Land at location |Empty| Empty| Empty| Desired yaw angle.| Latitude| Longitude| Altitude|  </summary>
            LAND=21, 
        	///<summary> Takeoff from ground / hand |Minimum pitch (if airspeed sensor present), desired pitch without sensor| Empty| Empty| Yaw angle (if magnetometer present), ignored without magnetometer| Latitude| Longitude| Altitude|  </summary>
            TAKEOFF=22, 
        	///<summary> Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Region of intereset mode. (see MAV_ROI enum)| MISSION index/ target ID. (see MAV_ROI enum)| ROI index (allows a vehicle to manage multiple ROI's)| Empty| x the location of the fixed ROI (see MAV_FRAME)| y| z|  </summary>
            ROI=80, 
        	///<summary> Control autonomous path planning on the MAV. |0: Disable local obstacle avoidance / local path planning (without resetting map), 1: Enable local path planning, 2: Enable and reset local path planning| 0: Disable full path planning (without resetting map), 1: Enable, 2: Enable and reset map/occupancy grid, 3: Enable and reset planned route, but not occupancy grid| Empty| Yaw angle at goal, in compass degrees, [0..360]| Latitude/X of goal| Longitude/Y of goal| Altitude/Z of goal|  </summary>
            PATHPLANNING=81, 
        	///<summary> NOP - This command is only used to mark the upper limit of the NAV/ACTION commands in the enumeration |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            LAST=95, 
        	///<summary> Delay mission state machine. |Delay in seconds (decimal)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            CONDITION_DELAY=112, 
        	///<summary> Ascend/descend at rate.  Delay mission state machine until desired altitude reached. |Descent / Ascend rate (m/s)| Empty| Empty| Empty| Empty| Empty| Finish Altitude|  </summary>
            CONDITION_CHANGE_ALT=113, 
        	///<summary> Delay mission state machine until within desired distance of next NAV point. |Distance (meters)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            CONDITION_DISTANCE=114, 
        	///<summary> Reach a certain target angle. |target angle: [0-360], 0 is north| speed during yaw change:[deg per second]| direction: negative: counter clockwise, positive: clockwise [-1,1]| relative offset or absolute angle: [ 1,0]| Empty| Empty| Empty|  </summary>
            CONDITION_YAW=115, 
        	///<summary> NOP - This command is only used to mark the upper limit of the CONDITION commands in the enumeration |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            CONDITION_LAST=159, 
        	///<summary> Set system mode. |Mode, as defined by ENUM MAV_MODE| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_SET_MODE=176, 
        	///<summary> Jump to the desired command in the mission list.  Repeat this action only the specified number of times |Sequence number| Repeat count| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_JUMP=177, 
        	///<summary> Change speed and/or throttle set points. |Speed type (0=Airspeed, 1=Ground Speed)| Speed  (m/s, -1 indicates no change)| Throttle  ( Percent, -1 indicates no change)| Empty| Empty| Empty| Empty|  </summary>
            DO_CHANGE_SPEED=178, 
        	///<summary> Changes the home location either to the current location or a specified location. |Use current (1=use current location, 0=use specified location)| Empty| Empty| Empty| Latitude| Longitude| Altitude|  </summary>
            DO_SET_HOME=179, 
        	///<summary> Set a system parameter.  Caution!  Use of this command requires knowledge of the numeric enumeration value of the parameter. |Parameter number| Parameter value| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_SET_PARAMETER=180, 
        	///<summary> Set a relay to a condition. |Relay number| Setting (1=on, 0=off, others possible depending on system hardware)| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_SET_RELAY=181, 
        	///<summary> Cycle a relay on and off for a desired number of cyles with a desired period. |Relay number| Cycle count| Cycle time (seconds, decimal)| Empty| Empty| Empty| Empty|  </summary>
            DO_REPEAT_RELAY=182, 
        	///<summary> Set a servo to a desired PWM value. |Servo number| PWM (microseconds, 1000 to 2000 typical)| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_SET_SERVO=183, 
        	///<summary> Cycle a between its nominal setting and a desired PWM for a desired number of cycles with a desired period. |Servo number| PWM (microseconds, 1000 to 2000 typical)| Cycle count| Cycle time (seconds)| Empty| Empty| Empty|  </summary>
            DO_REPEAT_SERVO=184, 
        	///<summary> Control onboard camera system. |Camera ID (-1 for all)| Transmission: 0: disabled, 1: enabled compressed, 2: enabled raw| Transmission mode: 0: video stream, >0: single images every n seconds (decimal)| Recording: 0: disabled, 1: enabled compressed, 2: enabled raw| Empty| Empty| Empty|  </summary>
            DO_CONTROL_VIDEO=200, 
        	///<summary> Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Region of intereset mode. (see MAV_ROI enum)| MISSION index/ target ID. (see MAV_ROI enum)| ROI index (allows a vehicle to manage multiple ROI's)| Empty| x the location of the fixed ROI (see MAV_FRAME)| y| z|  </summary>
            DO_SET_ROI=201, 
        	///<summary> Mission command to configure an on-board camera controller system. |Modes: P, TV, AV, M, Etc| Shutter speed: Divisor number for one second| Aperture: F stop number| ISO number e.g. 80, 100, 200, Etc| Exposure type enumerator| Command Identity| Main engine cut-off time before camera trigger in seconds/10 (0 means no cut-off)|  </summary>
            DO_DIGICAM_CONFIGURE=202, 
        	///<summary> Mission command to control an on-board camera controller system. |Session control e.g. show/hide lens| Zoom's absolute position| Zooming step value to offset zoom from the current position| Focus Locking, Unlocking or Re-locking| Shooting Command| Command Identity| Empty|  </summary>
            DO_DIGICAM_CONTROL=203, 
        	///<summary> Mission command to configure a camera or antenna mount |Mount operation mode (see MAV_MOUNT_MODE enum)| stabilize roll? (1 = yes, 0 = no)| stabilize pitch? (1 = yes, 0 = no)| stabilize yaw? (1 = yes, 0 = no)| Empty| Empty| Empty|  </summary>
            DO_MOUNT_CONFIGURE=204, 
        	///<summary> Mission command to control a camera or antenna mount |pitch(deg*100) or lat, depending on mount mode.| roll(deg*100) or lon depending on mount mode| yaw(deg*100) or alt (in cm) depending on mount mode| Empty| Empty| Empty| Empty|  </summary>
            DO_MOUNT_CONTROL=205, 
        	///<summary> Mission command to set CAM_TRIGG_DIST for this flight |Camera trigger distance (meters)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_SET_CAM_TRIGG_DIST=206, 
        	///<summary> NOP - This command is only used to mark the upper limit of the DO commands in the enumeration |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_LAST=240, 
        	///<summary> Trigger calibration. This command will be only accepted if in pre-flight mode. |Gyro calibration: 0: no, 1: yes| Magnetometer calibration: 0: no, 1: yes| Ground pressure: 0: no, 1: yes| Radio calibration: 0: no, 1: yes| Accelerometer calibration: 0: no, 1: yes| Empty| Empty|  </summary>
            PREFLIGHT_CALIBRATION=241, 
        	///<summary> Set sensor offsets. This command will be only accepted if in pre-flight mode. |Sensor to adjust the offsets for: 0: gyros, 1: accelerometer, 2: magnetometer, 3: barometer, 4: optical flow| X axis offset (or generic dimension 1), in the sensor's raw units| Y axis offset (or generic dimension 2), in the sensor's raw units| Z axis offset (or generic dimension 3), in the sensor's raw units| Generic dimension 4, in the sensor's raw units| Generic dimension 5, in the sensor's raw units| Generic dimension 6, in the sensor's raw units|  </summary>
            PREFLIGHT_SET_SENSOR_OFFSETS=242, 
        	///<summary> Request storage of different parameter values and logs. This command will be only accepted if in pre-flight mode. |Parameter storage: 0: READ FROM FLASH/EEPROM, 1: WRITE CURRENT TO FLASH/EEPROM| Mission storage: 0: READ FROM FLASH/EEPROM, 1: WRITE CURRENT TO FLASH/EEPROM| Reserved| Reserved| Empty| Empty| Empty|  </summary>
            PREFLIGHT_STORAGE=245, 
        	///<summary> Request the reboot or shutdown of system components. |0: Do nothing for autopilot, 1: Reboot autopilot, 2: Shutdown autopilot.| 0: Do nothing for onboard computer, 1: Reboot onboard computer, 2: Shutdown onboard computer.| Reserved| Reserved| Empty| Empty| Empty|  </summary>
            PREFLIGHT_REBOOT_SHUTDOWN=246, 
        	///<summary> Hold / continue the current action |MAV_GOTO_DO_HOLD: hold MAV_GOTO_DO_CONTINUE: continue with next item in mission plan| MAV_GOTO_HOLD_AT_CURRENT_POSITION: Hold at current position MAV_GOTO_HOLD_AT_SPECIFIED_POSITION: hold at specified position| MAV_FRAME coordinate frame of hold point| Desired yaw angle in degrees| Latitude / X position| Longitude / Y position| Altitude / Z position|  </summary>
            OVERRIDE_GOTO=252, 
        	///<summary> start running a mission |first_item: the first mission item to run| last_item:  the last mission item to run (after this item is run, the mission ends)|  </summary>
            MISSION_START=300, 
        	///<summary> Arms / Disarms a component |1 to arm, 0 to disarm|  </summary>
            COMPONENT_ARM_DISARM=400, 
        	///<summary> Starts receiver pairing |0:Spektrum| 0:Spektrum DSM2, 1:Spektrum DSMX|  </summary>
            START_RX_PAIR=500, 
        	///<summary>  | </summary>
            ENUM_END=501, 
        
        };
        
        ///<summary>  </summary>
        public enum FENCE_ACTION
        {
    	///<summary> Disable fenced mode | </summary>
            NONE=0, 
        	///<summary> Switched to guided mode to return point (fence point 0) | </summary>
            GUIDED=1, 
        	///<summary> Report fence breach, but don't take action | </summary>
            REPORT=2, 
        	///<summary> Switched to guided mode to return point (fence point 0) with manual throttle control | </summary>
            GUIDED_THR_PASS=3, 
        	///<summary>  | </summary>
            ENUM_END=4, 
        
        };
        
        ///<summary>  </summary>
        public enum FENCE_BREACH
        {
    	///<summary> No last fence breach | </summary>
            NONE=0, 
        	///<summary> Breached minimum altitude | </summary>
            MINALT=1, 
        	///<summary> Breached maximum altitude | </summary>
            MAXALT=2, 
        	///<summary> Breached fence boundary | </summary>
            BOUNDARY=3, 
        	///<summary>  | </summary>
            ENUM_END=4, 
        
        };
        
        ///<summary>  </summary>
        public enum LIMITS_STATE
        {
    	///<summary>  pre-initialization | </summary>
            LIMITS_INIT=0, 
        	///<summary>  disabled | </summary>
            LIMITS_DISABLED=1, 
        	///<summary>  checking limits | </summary>
            LIMITS_ENABLED=2, 
        	///<summary>  a limit has been breached | </summary>
            LIMITS_TRIGGERED=3, 
        	///<summary>  taking action eg. RTL | </summary>
            LIMITS_RECOVERING=4, 
        	///<summary>  we're no longer in breach of a limit | </summary>
            LIMITS_RECOVERED=5, 
        	///<summary>  | </summary>
            ENUM_END=6, 
        
        };
        
        ///<summary>  </summary>
        public enum LIMIT_MODULE
        {
    	///<summary>  pre-initialization | </summary>
            LIMIT_GPSLOCK=1, 
        	///<summary>  disabled | </summary>
            LIMIT_GEOFENCE=2, 
        	///<summary>  checking limits | </summary>
            LIMIT_ALTITUDE=4, 
        	///<summary>  | </summary>
            ENUM_END=5, 
        
        };
        
        ///<summary> Flags in RALLY_POINT message </summary>
        public enum RALLY_FLAGS
        {
    	///<summary> Flag set when requiring favorable winds for landing.  | </summary>
            FAVORABLE_WIND=1, 
        	///<summary> Flag set when plane is to immediately descend to break altitude and land without GCS intervention.  Flag not set when plane is to loiter at Rally point until commanded to land. | </summary>
            LAND_IMMEDIATELY=2, 
        	///<summary>  | </summary>
            ENUM_END=3, 
        
        };
        
    
        
        ///<summary> Micro air vehicle / autopilot classes. This identifies the individual model. </summary>
        public enum MAV_AUTOPILOT
        {
    	///<summary> Generic autopilot, full support for everything | </summary>
            GENERIC=0, 
        	///<summary> PIXHAWK autopilot, http://pixhawk.ethz.ch | </summary>
            PIXHAWK=1, 
        	///<summary> SLUGS autopilot, http://slugsuav.soe.ucsc.edu | </summary>
            SLUGS=2, 
        	///<summary> ArduPilotMega / ArduCopter, http://diydrones.com | </summary>
            ARDUPILOTMEGA=3, 
        	///<summary> OpenPilot, http://openpilot.org | </summary>
            OPENPILOT=4, 
        	///<summary> Generic autopilot only supporting simple waypoints | </summary>
            GENERIC_WAYPOINTS_ONLY=5, 
        	///<summary> Generic autopilot supporting waypoints and other simple navigation commands | </summary>
            GENERIC_WAYPOINTS_AND_SIMPLE_NAVIGATION_ONLY=6, 
        	///<summary> Generic autopilot supporting the full mission command set | </summary>
            GENERIC_MISSION_FULL=7, 
        	///<summary> No valid autopilot, e.g. a GCS or other MAVLink component | </summary>
            INVALID=8, 
        	///<summary> PPZ UAV - http://nongnu.org/paparazzi | </summary>
            PPZ=9, 
        	///<summary> UAV Dev Board | </summary>
            UDB=10, 
        	///<summary> FlexiPilot | </summary>
            FP=11, 
        	///<summary> PX4 Autopilot - http://pixhawk.ethz.ch/px4/ | </summary>
            PX4=12, 
        	///<summary> SMACCMPilot - http://smaccmpilot.org | </summary>
            SMACCMPILOT=13, 
        	///<summary> AutoQuad -- http://autoquad.org | </summary>
            AUTOQUAD=14, 
        	///<summary> Armazila -- http://armazila.com | </summary>
            ARMAZILA=15, 
        	///<summary> Aerob -- http://aerob.ru | </summary>
            AEROB=16, 
        	///<summary>  | </summary>
            ENUM_END=17, 
        
        };
        
        ///<summary>  </summary>
        public enum MAV_TYPE
        {
    	///<summary> Generic micro air vehicle. | </summary>
            GENERIC=0, 
        	///<summary> Fixed wing aircraft. | </summary>
            FIXED_WING=1, 
        	///<summary> Quadrotor | </summary>
            QUADROTOR=2, 
        	///<summary> Coaxial helicopter | </summary>
            COAXIAL=3, 
        	///<summary> Normal helicopter with tail rotor. | </summary>
            HELICOPTER=4, 
        	///<summary> Ground installation | </summary>
            ANTENNA_TRACKER=5, 
        	///<summary> Operator control unit / ground control station | </summary>
            GCS=6, 
        	///<summary> Airship, controlled | </summary>
            AIRSHIP=7, 
        	///<summary> Free balloon, uncontrolled | </summary>
            FREE_BALLOON=8, 
        	///<summary> Rocket | </summary>
            ROCKET=9, 
        	///<summary> Ground rover | </summary>
            GROUND_ROVER=10, 
        	///<summary> Surface vessel, boat, ship | </summary>
            SURFACE_BOAT=11, 
        	///<summary> Submarine | </summary>
            SUBMARINE=12, 
        	///<summary> Hexarotor | </summary>
            HEXAROTOR=13, 
        	///<summary> Octorotor | </summary>
            OCTOROTOR=14, 
        	///<summary> Octorotor | </summary>
            TRICOPTER=15, 
        	///<summary> Flapping wing | </summary>
            FLAPPING_WING=16, 
        	///<summary> Flapping wing | </summary>
            KITE=17, 
        	///<summary>  | </summary>
            ENUM_END=18, 
        
        };
        
        ///<summary> These flags encode the MAV mode. </summary>
        public enum MAV_MODE_FLAG
        {
    	///<summary> 0b00000001 Reserved for future use. | </summary>
            CUSTOM_MODE_ENABLED=1, 
        	///<summary> 0b00000010 system has a test mode enabled. This flag is intended for temporary system tests and should not be used for stable implementations. | </summary>
            TEST_ENABLED=2, 
        	///<summary> 0b00000100 autonomous mode enabled, system finds its own goal positions. Guided flag can be set or not, depends on the actual implementation. | </summary>
            AUTO_ENABLED=4, 
        	///<summary> 0b00001000 guided mode enabled, system flies MISSIONs / mission items. | </summary>
            GUIDED_ENABLED=8, 
        	///<summary> 0b00010000 system stabilizes electronically its attitude (and optionally position). It needs however further control inputs to move around. | </summary>
            STABILIZE_ENABLED=16, 
        	///<summary> 0b00100000 hardware in the loop simulation. All motors / actuators are blocked, but internal software is full operational. | </summary>
            HIL_ENABLED=32, 
        	///<summary> 0b01000000 remote control input is enabled. | </summary>
            MANUAL_INPUT_ENABLED=64, 
        	///<summary> 0b10000000 MAV safety set to armed. Motors are enabled / running / can start. Ready to fly. | </summary>
            SAFETY_ARMED=128, 
        	///<summary>  | </summary>
            ENUM_END=129, 
        
        };
        
        ///<summary> These values encode the bit positions of the decode position. These values can be used to read the value of a flag bit by combining the base_mode variable with AND with the flag position value. The result will be either 0 or 1, depending on if the flag is set or not. </summary>
        public enum MAV_MODE_FLAG_DECODE_POSITION
        {
    	///<summary> Eighth bit: 00000001 | </summary>
            CUSTOM_MODE=1, 
        	///<summary> Seventh bit: 00000010 | </summary>
            TEST=2, 
        	///<summary> Sixt bit:   00000100 | </summary>
            AUTO=4, 
        	///<summary> Fifth bit:  00001000 | </summary>
            GUIDED=8, 
        	///<summary> Fourth bit: 00010000 | </summary>
            STABILIZE=16, 
        	///<summary> Third bit:  00100000 | </summary>
            HIL=32, 
        	///<summary> Second bit: 01000000 | </summary>
            MANUAL=64, 
        	///<summary> First bit:  10000000 | </summary>
            SAFETY=128, 
        	///<summary>  | </summary>
            ENUM_END=129, 
        
        };
        
        ///<summary> Override command, pauses current mission execution and moves immediately to a position </summary>
        public enum MAV_GOTO
        {
    	///<summary> Hold at the current position. | </summary>
            DO_HOLD=0, 
        	///<summary> Continue with the next item in mission execution. | </summary>
            DO_CONTINUE=1, 
        	///<summary> Hold at the current position of the system | </summary>
            HOLD_AT_CURRENT_POSITION=2, 
        	///<summary> Hold at the position specified in the parameters of the DO_HOLD action | </summary>
            HOLD_AT_SPECIFIED_POSITION=3, 
        	///<summary>  | </summary>
            ENUM_END=4, 
        
        };
        
        ///<summary> These defines are predefined OR-combined mode flags. There is no need to use values from this enum, but it                simplifies the use of the mode flags. Note that manual input is enabled in all modes as a safety override. </summary>
        public enum MAV_MODE
        {
    	///<summary> System is not ready to fly, booting, calibrating, etc. No flag is set. | </summary>
            PREFLIGHT=0, 
        	///<summary> System is allowed to be active, under manual (RC) control, no stabilization | </summary>
            MANUAL_DISARMED=64, 
        	///<summary> UNDEFINED mode. This solely depends on the autopilot - use with caution, intended for developers only. | </summary>
            TEST_DISARMED=66, 
        	///<summary> System is allowed to be active, under assisted RC control. | </summary>
            STABILIZE_DISARMED=80, 
        	///<summary> System is allowed to be active, under autonomous control, manual setpoint | </summary>
            GUIDED_DISARMED=88, 
        	///<summary> System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by MISSIONs) | </summary>
            AUTO_DISARMED=92, 
        	///<summary> System is allowed to be active, under manual (RC) control, no stabilization | </summary>
            MANUAL_ARMED=192, 
        	///<summary> UNDEFINED mode. This solely depends on the autopilot - use with caution, intended for developers only. | </summary>
            TEST_ARMED=194, 
        	///<summary> System is allowed to be active, under assisted RC control. | </summary>
            STABILIZE_ARMED=208, 
        	///<summary> System is allowed to be active, under autonomous control, manual setpoint | </summary>
            GUIDED_ARMED=216, 
        	///<summary> System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by MISSIONs) | </summary>
            AUTO_ARMED=220, 
        	///<summary>  | </summary>
            ENUM_END=221, 
        
        };
        
        ///<summary>  </summary>
        public enum MAV_STATE
        {
    	///<summary> Uninitialized system, state is unknown. | </summary>
            UNINIT=0, 
        	///<summary> System is booting up. | </summary>
            BOOT=1, 
        	///<summary> System is calibrating and not flight-ready. | </summary>
            CALIBRATING=2, 
        	///<summary> System is grounded and on standby. It can be launched any time. | </summary>
            STANDBY=3, 
        	///<summary> System is active and might be already airborne. Motors are engaged. | </summary>
            ACTIVE=4, 
        	///<summary> System is in a non-normal flight mode. It can however still navigate. | </summary>
            CRITICAL=5, 
        	///<summary> System is in a non-normal flight mode. It lost control over parts or over the whole airframe. It is in mayday and going down. | </summary>
            EMERGENCY=6, 
        	///<summary> System just initialized its power-down sequence, will shut down now. | </summary>
            POWEROFF=7, 
        	///<summary>  | </summary>
            ENUM_END=8, 
        
        };
        
        ///<summary>  </summary>
        public enum MAV_COMPONENT
        {
    	///<summary>  | </summary>
            MAV_COMP_ID_ALL=0, 
        	///<summary>  | </summary>
            MAV_COMP_ID_CAMERA=100, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO1=140, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO2=141, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO3=142, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO4=143, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO5=144, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO6=145, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO7=146, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO8=147, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO9=148, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO10=149, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO11=150, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO12=151, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO13=152, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SERVO14=153, 
        	///<summary>  | </summary>
            MAV_COMP_ID_MAPPER=180, 
        	///<summary>  | </summary>
            MAV_COMP_ID_MISSIONPLANNER=190, 
        	///<summary>  | </summary>
            MAV_COMP_ID_PATHPLANNER=195, 
        	///<summary>  | </summary>
            MAV_COMP_ID_IMU=200, 
        	///<summary>  | </summary>
            MAV_COMP_ID_IMU_2=201, 
        	///<summary>  | </summary>
            MAV_COMP_ID_IMU_3=202, 
        	///<summary>  | </summary>
            MAV_COMP_ID_GPS=220, 
        	///<summary>  | </summary>
            MAV_COMP_ID_UDP_BRIDGE=240, 
        	///<summary>  | </summary>
            MAV_COMP_ID_UART_BRIDGE=241, 
        	///<summary>  | </summary>
            MAV_COMP_ID_SYSTEM_CONTROL=250, 
        	///<summary>  | </summary>
            ENUM_END=251, 
        
        };
        
        ///<summary> These encode the sensors whose status is sent as part of the SYS_STATUS message. </summary>
        public enum MAV_SYS_STATUS_SENSOR
        {
    	///<summary> 0x01 3D gyro | </summary>
            _3D_GYRO=1, 
        	///<summary> 0x02 3D accelerometer | </summary>
            _3D_ACCEL=2, 
        	///<summary> 0x04 3D magnetometer | </summary>
            _3D_MAG=4, 
        	///<summary> 0x08 absolute pressure | </summary>
            ABSOLUTE_PRESSURE=8, 
        	///<summary> 0x10 differential pressure | </summary>
            DIFFERENTIAL_PRESSURE=16, 
        	///<summary> 0x20 GPS | </summary>
            GPS=32, 
        	///<summary> 0x40 optical flow | </summary>
            OPTICAL_FLOW=64, 
        	///<summary> 0x80 computer vision position | </summary>
            VISION_POSITION=128, 
        	///<summary> 0x100 laser based position | </summary>
            LASER_POSITION=256, 
        	///<summary> 0x200 external ground truth (Vicon or Leica) | </summary>
            EXTERNAL_GROUND_TRUTH=512, 
        	///<summary> 0x400 3D angular rate control | </summary>
            ANGULAR_RATE_CONTROL=1024, 
        	///<summary> 0x800 attitude stabilization | </summary>
            ATTITUDE_STABILIZATION=2048, 
        	///<summary> 0x1000 yaw position | </summary>
            YAW_POSITION=4096, 
        	///<summary> 0x2000 z/altitude control | </summary>
            Z_ALTITUDE_CONTROL=8192, 
        	///<summary> 0x4000 x/y position control | </summary>
            XY_POSITION_CONTROL=16384, 
        	///<summary> 0x8000 motor outputs / control | </summary>
            MOTOR_OUTPUTS=32768, 
        	///<summary> 0x10000 rc receiver | </summary>
            RC_RECEIVER=65536, 
        	///<summary>  | </summary>
            ENUM_END=65537, 
        
        };
        
        ///<summary>  </summary>
        public enum MAV_FRAME
        {
    	///<summary> Global coordinate frame, WGS84 coordinate system. First value / x: latitude, second value / y: longitude, third value / z: positive altitude over mean sea level (MSL) | </summary>
            GLOBAL=0, 
        	///<summary> Local coordinate frame, Z-up (x: north, y: east, z: down). | </summary>
            LOCAL_NED=1, 
        	///<summary> NOT a coordinate frame, indicates a mission command. | </summary>
            MISSION=2, 
        	///<summary> Global coordinate frame, WGS84 coordinate system, relative altitude over ground with respect to the home position. First value / x: latitude, second value / y: longitude, third value / z: positive altitude with 0 being at the altitude of the home location. | </summary>
            GLOBAL_RELATIVE_ALT=3, 
        	///<summary> Local coordinate frame, Z-down (x: east, y: north, z: up) | </summary>
            LOCAL_ENU=4, 
        	///<summary>  | </summary>
            ENUM_END=5, 
        
        };
        
        ///<summary>  </summary>
        public enum MAVLINK_DATA_STREAM_TYPE
        {
    	///<summary>  | </summary>
            MAVLINK_DATA_STREAM_IMG_JPEG=1, 
        	///<summary>  | </summary>
            MAVLINK_DATA_STREAM_IMG_BMP=2, 
        	///<summary>  | </summary>
            MAVLINK_DATA_STREAM_IMG_RAW8U=3, 
        	///<summary>  | </summary>
            MAVLINK_DATA_STREAM_IMG_RAW32U=4, 
        	///<summary>  | </summary>
            MAVLINK_DATA_STREAM_IMG_PGM=5, 
        	///<summary>  | </summary>
            MAVLINK_DATA_STREAM_IMG_PNG=6, 
        	///<summary>  | </summary>
            ENUM_END=7, 
        
        };
        
        ///<summary> Data stream IDs. A data stream is not a fixed set of messages, but rather a      recommendation to the autopilot software. Individual autopilots may or may not obey      the recommended messages. </summary>
        public enum MAV_DATA_STREAM
        {
    	///<summary> Enable all data streams | </summary>
            ALL=0, 
        	///<summary> Enable IMU_RAW, GPS_RAW, GPS_STATUS packets. | </summary>
            RAW_SENSORS=1, 
        	///<summary> Enable GPS_STATUS, CONTROL_STATUS, AUX_STATUS | </summary>
            EXTENDED_STATUS=2, 
        	///<summary> Enable RC_CHANNELS_SCALED, RC_CHANNELS_RAW, SERVO_OUTPUT_RAW | </summary>
            RC_CHANNELS=3, 
        	///<summary> Enable ATTITUDE_CONTROLLER_OUTPUT, POSITION_CONTROLLER_OUTPUT, NAV_CONTROLLER_OUTPUT. | </summary>
            RAW_CONTROLLER=4, 
        	///<summary> Enable LOCAL_POSITION, GLOBAL_POSITION/GLOBAL_POSITION_INT messages. | </summary>
            POSITION=6, 
        	///<summary> Dependent on the autopilot | </summary>
            EXTRA1=10, 
        	///<summary> Dependent on the autopilot | </summary>
            EXTRA2=11, 
        	///<summary> Dependent on the autopilot | </summary>
            EXTRA3=12, 
        	///<summary>  | </summary>
            ENUM_END=13, 
        
        };
        
        ///<summary>  The ROI (region of interest) for the vehicle. This can be                 be used by the vehicle for camera/vehicle attitude alignment (see                 MAV_CMD_NAV_ROI). </summary>
        public enum MAV_ROI
        {
    	///<summary> No region of interest. | </summary>
            NONE=0, 
        	///<summary> Point toward next MISSION. | </summary>
            WPNEXT=1, 
        	///<summary> Point toward given MISSION. | </summary>
            WPINDEX=2, 
        	///<summary> Point toward fixed location. | </summary>
            LOCATION=3, 
        	///<summary> Point toward of given id. | </summary>
            TARGET=4, 
        	///<summary>  | </summary>
            ENUM_END=5, 
        
        };
        
        ///<summary> ACK / NACK / ERROR values as a result of MAV_CMDs and for mission item transmission. </summary>
        public enum MAV_CMD_ACK
        {
    	///<summary> Command / mission item is ok. | </summary>
            OK=1, 
        	///<summary> Generic error message if none of the other reasons fails or if no detailed error reporting is implemented. | </summary>
            ERR_FAIL=2, 
        	///<summary> The system is refusing to accept this command from this source / communication partner. | </summary>
            ERR_ACCESS_DENIED=3, 
        	///<summary> Command or mission item is not supported, other commands would be accepted. | </summary>
            ERR_NOT_SUPPORTED=4, 
        	///<summary> The coordinate frame of this command / mission item is not supported. | </summary>
            ERR_COORDINATE_FRAME_NOT_SUPPORTED=5, 
        	///<summary> The coordinate frame of this command is ok, but he coordinate values exceed the safety limits of this system. This is a generic error, please use the more specific error messages below if possible. | </summary>
            ERR_COORDINATES_OUT_OF_RANGE=6, 
        	///<summary> The X or latitude value is out of range. | </summary>
            ERR_X_LAT_OUT_OF_RANGE=7, 
        	///<summary> The Y or longitude value is out of range. | </summary>
            ERR_Y_LON_OUT_OF_RANGE=8, 
        	///<summary> The Z or altitude value is out of range. | </summary>
            ERR_Z_ALT_OUT_OF_RANGE=9, 
        	///<summary>  | </summary>
            ENUM_END=10, 
        
        };
        
        ///<summary> Specifies the datatype of a MAVLink parameter. </summary>
        public enum MAV_PARAM_TYPE
        {
    	///<summary> 8-bit unsigned integer | </summary>
            UINT8=1, 
        	///<summary> 8-bit signed integer | </summary>
            INT8=2, 
        	///<summary> 16-bit unsigned integer | </summary>
            UINT16=3, 
        	///<summary> 16-bit signed integer | </summary>
            INT16=4, 
        	///<summary> 32-bit unsigned integer | </summary>
            UINT32=5, 
        	///<summary> 32-bit signed integer | </summary>
            INT32=6, 
        	///<summary> 64-bit unsigned integer | </summary>
            UINT64=7, 
        	///<summary> 64-bit signed integer | </summary>
            INT64=8, 
        	///<summary> 32-bit floating-point | </summary>
            REAL32=9, 
        	///<summary> 64-bit floating-point | </summary>
            REAL64=10, 
        	///<summary>  | </summary>
            ENUM_END=11, 
        
        };
        
        ///<summary> result from a mavlink command </summary>
        public enum MAV_RESULT
        {
    	///<summary> Command ACCEPTED and EXECUTED | </summary>
            ACCEPTED=0, 
        	///<summary> Command TEMPORARY REJECTED/DENIED | </summary>
            TEMPORARILY_REJECTED=1, 
        	///<summary> Command PERMANENTLY DENIED | </summary>
            DENIED=2, 
        	///<summary> Command UNKNOWN/UNSUPPORTED | </summary>
            UNSUPPORTED=3, 
        	///<summary> Command executed, but failed | </summary>
            FAILED=4, 
        	///<summary>  | </summary>
            ENUM_END=5, 
        
        };
        
        ///<summary> result in a mavlink mission ack </summary>
        public enum MAV_MISSION_RESULT
        {
    	///<summary> mission accepted OK | </summary>
            MAV_MISSION_ACCEPTED=0, 
        	///<summary> generic error / not accepting mission commands at all right now | </summary>
            MAV_MISSION_ERROR=1, 
        	///<summary> coordinate frame is not supported | </summary>
            MAV_MISSION_UNSUPPORTED_FRAME=2, 
        	///<summary> command is not supported | </summary>
            MAV_MISSION_UNSUPPORTED=3, 
        	///<summary> mission item exceeds storage space | </summary>
            MAV_MISSION_NO_SPACE=4, 
        	///<summary> one of the parameters has an invalid value | </summary>
            MAV_MISSION_INVALID=5, 
        	///<summary> param1 has an invalid value | </summary>
            MAV_MISSION_INVALID_PARAM1=6, 
        	///<summary> param2 has an invalid value | </summary>
            MAV_MISSION_INVALID_PARAM2=7, 
        	///<summary> param3 has an invalid value | </summary>
            MAV_MISSION_INVALID_PARAM3=8, 
        	///<summary> param4 has an invalid value | </summary>
            MAV_MISSION_INVALID_PARAM4=9, 
        	///<summary> x/param5 has an invalid value | </summary>
            MAV_MISSION_INVALID_PARAM5_X=10, 
        	///<summary> y/param6 has an invalid value | </summary>
            MAV_MISSION_INVALID_PARAM6_Y=11, 
        	///<summary> param7 has an invalid value | </summary>
            MAV_MISSION_INVALID_PARAM7=12, 
        	///<summary> received waypoint out of sequence | </summary>
            MAV_MISSION_INVALID_SEQUENCE=13, 
        	///<summary> not accepting any mission commands from this communication partner | </summary>
            MAV_MISSION_DENIED=14, 
        	///<summary>  | </summary>
            ENUM_END=15, 
        
        };
        
        ///<summary> Indicates the severity level, generally used for status messages to indicate their relative urgency. Based on RFC-5424 using expanded definitions at: http://www.kiwisyslog.com/kb/info:-syslog-message-levels/. </summary>
        public enum MAV_SEVERITY
        {
    	///<summary> System is unusable. This is a "panic" condition. | </summary>
            EMERGENCY=0, 
        	///<summary> Action should be taken immediately. Indicates error in non-critical systems. | </summary>
            ALERT=1, 
        	///<summary> Action must be taken immediately. Indicates failure in a primary system. | </summary>
            CRITICAL=2, 
        	///<summary> Indicates an error in secondary/redundant systems. | </summary>
            ERROR=3, 
        	///<summary> Indicates about a possible future error if this is not resolved within a given timeframe. Example would be a low battery warning. | </summary>
            WARNING=4, 
        	///<summary> An unusual event has occured, though not an error condition. This should be investigated for the root cause. | </summary>
            NOTICE=5, 
        	///<summary> Normal operational messages. Useful for logging. No action is required for these messages. | </summary>
            INFO=6, 
        	///<summary> Useful non-operational messages that can assist in debugging. These should not occur during normal operation. | </summary>
            DEBUG=7, 
        	///<summary>  | </summary>
            ENUM_END=8, 
        
        };
        

    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    public struct mavlink_sensor_offsets_t
    {
        /// <summary> magnetic declination (radians) </summary>
        public  Single mag_declination;
            /// <summary> raw pressure from barometer </summary>
        public  Int32 raw_press;
            /// <summary> raw temperature from barometer </summary>
        public  Int32 raw_temp;
            /// <summary> gyro X calibration </summary>
        public  Single gyro_cal_x;
            /// <summary> gyro Y calibration </summary>
        public  Single gyro_cal_y;
            /// <summary> gyro Z calibration </summary>
        public  Single gyro_cal_z;
            /// <summary> accel X calibration </summary>
        public  Single accel_cal_x;
            /// <summary> accel Y calibration </summary>
        public  Single accel_cal_y;
            /// <summary> accel Z calibration </summary>
        public  Single accel_cal_z;
            /// <summary> magnetometer X offset </summary>
        public  Int16 mag_ofs_x;
            /// <summary> magnetometer Y offset </summary>
        public  Int16 mag_ofs_y;
            /// <summary> magnetometer Z offset </summary>
        public  Int16 mag_ofs_z;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    public struct mavlink_set_mag_offsets_t
    {
        /// <summary> magnetometer X offset </summary>
        public  Int16 mag_ofs_x;
            /// <summary> magnetometer Y offset </summary>
        public  Int16 mag_ofs_y;
            /// <summary> magnetometer Z offset </summary>
        public  Int16 mag_ofs_z;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    public struct mavlink_meminfo_t
    {
        /// <summary> heap top </summary>
        public  UInt16 brkval;
            /// <summary> free memory </summary>
        public  UInt16 freemem;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    public struct mavlink_ap_adc_t
    {
        /// <summary> ADC output 1 </summary>
        public  UInt16 adc1;
            /// <summary> ADC output 2 </summary>
        public  UInt16 adc2;
            /// <summary> ADC output 3 </summary>
        public  UInt16 adc3;
            /// <summary> ADC output 4 </summary>
        public  UInt16 adc4;
            /// <summary> ADC output 5 </summary>
        public  UInt16 adc5;
            /// <summary> ADC output 6 </summary>
        public  UInt16 adc6;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=15)]
    public struct mavlink_digicam_configure_t
    {
        /// <summary> Correspondent value to given extra_param </summary>
        public  Single extra_value;
            /// <summary> Divisor number //e.g. 1000 means 1/1000 (0 means ignore) </summary>
        public  UInt16 shutter_speed;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> Mode enumeration from 1 to N //P, TV, AV, M, Etc (0 means ignore) </summary>
        public  byte mode;
            /// <summary> F stop number x 10 //e.g. 28 means 2.8 (0 means ignore) </summary>
        public  byte aperture;
            /// <summary> ISO enumeration from 1 to N //e.g. 80, 100, 200, Etc (0 means ignore) </summary>
        public  byte iso;
            /// <summary> Exposure type enumeration from 1 to N (0 means ignore) </summary>
        public  byte exposure_type;
            /// <summary> Command Identity (incremental loop: 0 to 255)//A command sent multiple times will be executed or pooled just once </summary>
        public  byte command_id;
            /// <summary> Main engine cut-off time before camera trigger in seconds/10 (0 means no cut-off) </summary>
        public  byte engine_cut_off;
            /// <summary> Extra parameters enumeration (0 means ignore) </summary>
        public  byte extra_param;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=13)]
    public struct mavlink_digicam_control_t
    {
        /// <summary> Correspondent value to given extra_param </summary>
        public  Single extra_value;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> 0: stop, 1: start or keep it up //Session control e.g. show/hide lens </summary>
        public  byte session;
            /// <summary> 1 to N //Zoom's absolute position (0 means ignore) </summary>
        public  byte zoom_pos;
            /// <summary> -100 to 100 //Zooming step value to offset zoom from the current position </summary>
        public  byte zoom_step;
            /// <summary> 0: unlock focus or keep unlocked, 1: lock focus or keep locked, 3: re-lock focus </summary>
        public  byte focus_lock;
            /// <summary> 0: ignore, 1: shot or start filming </summary>
        public  byte shot;
            /// <summary> Command Identity (incremental loop: 0 to 255)//A command sent multiple times will be executed or pooled just once </summary>
        public  byte command_id;
            /// <summary> Extra parameters enumeration (0 means ignore) </summary>
        public  byte extra_param;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    public struct mavlink_mount_configure_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> mount operating mode (see MAV_MOUNT_MODE enum) </summary>
        public  byte mount_mode;
            /// <summary> (1 = yes, 0 = no) </summary>
        public  byte stab_roll;
            /// <summary> (1 = yes, 0 = no) </summary>
        public  byte stab_pitch;
            /// <summary> (1 = yes, 0 = no) </summary>
        public  byte stab_yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=15)]
    public struct mavlink_mount_control_t
    {
        /// <summary> pitch(deg*100) or lat, depending on mount mode </summary>
        public  Int32 input_a;
            /// <summary> roll(deg*100) or lon depending on mount mode </summary>
        public  Int32 input_b;
            /// <summary> yaw(deg*100) or alt (in cm) depending on mount mode </summary>
        public  Int32 input_c;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> if "1" it will save current trimmed position on EEPROM (just valid for NEUTRAL and LANDING) </summary>
        public  byte save_position;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    public struct mavlink_mount_status_t
    {
        /// <summary> pitch(deg*100) or lat, depending on mount mode </summary>
        public  Int32 pointing_a;
            /// <summary> roll(deg*100) or lon depending on mount mode </summary>
        public  Int32 pointing_b;
            /// <summary> yaw(deg*100) or alt (in cm) depending on mount mode </summary>
        public  Int32 pointing_c;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    public struct mavlink_fence_point_t
    {
        /// <summary> Latitude of point </summary>
        public  Single lat;
            /// <summary> Longitude of point </summary>
        public  Single lng;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> point index (first point is 1, 0 is for return point) </summary>
        public  byte idx;
            /// <summary> total number of points (for sanity checking) </summary>
        public  byte count;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    public struct mavlink_fence_fetch_point_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> point index (first point is 1, 0 is for return point) </summary>
        public  byte idx;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    public struct mavlink_fence_status_t
    {
        /// <summary> time of last breach in milliseconds since boot </summary>
        public  UInt32 breach_time;
            /// <summary> number of fence breaches </summary>
        public  UInt16 breach_count;
            /// <summary> 0 if currently inside fence, 1 if outside </summary>
        public  byte breach_status;
            /// <summary> last breach type (see FENCE_BREACH_* enum) </summary>
        public  byte breach_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    public struct mavlink_ahrs_t
    {
        /// <summary> X gyro drift estimate rad/s </summary>
        public  Single omegaIx;
            /// <summary> Y gyro drift estimate rad/s </summary>
        public  Single omegaIy;
            /// <summary> Z gyro drift estimate rad/s </summary>
        public  Single omegaIz;
            /// <summary> average accel_weight </summary>
        public  Single accel_weight;
            /// <summary> average renormalisation value </summary>
        public  Single renorm_val;
            /// <summary> average error_roll_pitch value </summary>
        public  Single error_rp;
            /// <summary> average error_yaw value </summary>
        public  Single error_yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    public struct mavlink_simstate_t
    {
        /// <summary> Roll angle (rad) </summary>
        public  Single roll;
            /// <summary> Pitch angle (rad) </summary>
        public  Single pitch;
            /// <summary> Yaw angle (rad) </summary>
        public  Single yaw;
            /// <summary> X acceleration m/s/s </summary>
        public  Single xacc;
            /// <summary> Y acceleration m/s/s </summary>
        public  Single yacc;
            /// <summary> Z acceleration m/s/s </summary>
        public  Single zacc;
            /// <summary> Angular speed around X axis rad/s </summary>
        public  Single xgyro;
            /// <summary> Angular speed around Y axis rad/s </summary>
        public  Single ygyro;
            /// <summary> Angular speed around Z axis rad/s </summary>
        public  Single zgyro;
            /// <summary> Latitude in degrees * 1E7 </summary>
        public  Int32 lat;
            /// <summary> Longitude in degrees * 1E7 </summary>
        public  Int32 lng;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    public struct mavlink_hwstatus_t
    {
        /// <summary> board voltage (mV) </summary>
        public  UInt16 Vcc;
            /// <summary> I2C error count </summary>
        public  byte I2Cerr;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    public struct mavlink_radio_t
    {
        /// <summary> receive errors </summary>
        public  UInt16 rxerrors;
            /// <summary> count of error corrected packets </summary>
        public  UInt16 @fixed;
            /// <summary> local signal strength </summary>
        public  byte rssi;
            /// <summary> remote signal strength </summary>
        public  byte remrssi;
            /// <summary> how full the tx buffer is as a percentage </summary>
        public  byte txbuf;
            /// <summary> background noise level </summary>
        public  byte noise;
            /// <summary> remote background noise level </summary>
        public  byte remnoise;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    public struct mavlink_limits_status_t
    {
        /// <summary> time of last breach in milliseconds since boot </summary>
        public  UInt32 last_trigger;
            /// <summary> time of last recovery action in milliseconds since boot </summary>
        public  UInt32 last_action;
            /// <summary> time of last successful recovery in milliseconds since boot </summary>
        public  UInt32 last_recovery;
            /// <summary> time of last all-clear in milliseconds since boot </summary>
        public  UInt32 last_clear;
            /// <summary> number of fence breaches </summary>
        public  UInt16 breach_count;
            /// <summary> state of AP_Limits, (see enum LimitState, LIMITS_STATE) </summary>
        public  byte limits_state;
            /// <summary> AP_Limit_Module bitfield of enabled modules, (see enum moduleid or LIMIT_MODULE) </summary>
        public  byte mods_enabled;
            /// <summary> AP_Limit_Module bitfield of required modules, (see enum moduleid or LIMIT_MODULE) </summary>
        public  byte mods_required;
            /// <summary> AP_Limit_Module bitfield of triggered modules, (see enum moduleid or LIMIT_MODULE) </summary>
        public  byte mods_triggered;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    public struct mavlink_wind_t
    {
        /// <summary> wind direction that wind is coming from (degrees) </summary>
        public  Single direction;
            /// <summary> wind speed in ground plane (m/s) </summary>
        public  Single speed;
            /// <summary> vertical wind speed (m/s) </summary>
        public  Single speed_z;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    public struct mavlink_data16_t
    {
        /// <summary> data type </summary>
        public  byte type;
            /// <summary> data length </summary>
        public  byte len;
            /// <summary> raw data </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=34)]
    public struct mavlink_data32_t
    {
        /// <summary> data type </summary>
        public  byte type;
            /// <summary> data length </summary>
        public  byte len;
            /// <summary> raw data </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=66)]
    public struct mavlink_data64_t
    {
        /// <summary> data type </summary>
        public  byte type;
            /// <summary> data length </summary>
        public  byte len;
            /// <summary> raw data </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=64)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=98)]
    public struct mavlink_data96_t
    {
        /// <summary> data type </summary>
        public  byte type;
            /// <summary> data length </summary>
        public  byte len;
            /// <summary> raw data </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=96)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    public struct mavlink_rangefinder_t
    {
        /// <summary> distance in meters </summary>
        public  Single distance;
            /// <summary> raw voltage if available, zero otherwise </summary>
        public  Single voltage;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=48)]
    public struct mavlink_airspeed_autocal_t
    {
        /// <summary> GPS velocity north m/s </summary>
        public  Single vx;
            /// <summary> GPS velocity east m/s </summary>
        public  Single vy;
            /// <summary> GPS velocity down m/s </summary>
        public  Single vz;
            /// <summary> Differential pressure pascals </summary>
        public  Single diff_pressure;
            /// <summary> Estimated to true airspeed ratio </summary>
        public  Single EAS2TAS;
            /// <summary> Airspeed ratio </summary>
        public  Single ratio;
            /// <summary> EKF state x </summary>
        public  Single state_x;
            /// <summary> EKF state y </summary>
        public  Single state_y;
            /// <summary> EKF state z </summary>
        public  Single state_z;
            /// <summary> EKF Pax </summary>
        public  Single Pax;
            /// <summary> EKF Pby </summary>
        public  Single Pby;
            /// <summary> EKF Pcz </summary>
        public  Single Pcz;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=19)]
    public struct mavlink_rally_point_t
    {
        /// <summary> Latitude of point in degrees * 1E7 </summary>
        public  Int32 lat;
            /// <summary> Longitude of point in degrees * 1E7 </summary>
        public  Int32 lng;
            /// <summary> Transit / loiter altitude in meters relative to home </summary>
        public  Int16 alt;
            /// <summary> Break altitude in meters relative to home </summary>
        public  Int16 break_alt;
            /// <summary> Heading to aim for when landing. In centi-degrees. </summary>
        public  UInt16 land_dir;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> point index (first point is 0) </summary>
        public  byte idx;
            /// <summary> total number of points (for sanity checking) </summary>
        public  byte count;
            /// <summary> See RALLY_FLAGS enum for definition of the bitmask. </summary>
        public  byte flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    public struct mavlink_rally_fetch_point_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> point index (first point is 0) </summary>
        public  byte idx;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    public struct mavlink_heartbeat_t
    {
        /// <summary> A bitfield for use for autopilot-specific flags. </summary>
        public  UInt32 custom_mode;
            /// <summary> Type of the MAV (quadrotor, helicopter, etc., up to 15 types, defined in MAV_TYPE ENUM) </summary>
        public  byte type;
            /// <summary> Autopilot type / class. defined in MAV_AUTOPILOT ENUM </summary>
        public  byte autopilot;
            /// <summary> System mode bitfield, see MAV_MODE_FLAGS ENUM in mavlink/include/mavlink_types.h </summary>
        public  byte base_mode;
            /// <summary> System status flag, see MAV_STATE ENUM </summary>
        public  byte system_status;
            /// <summary> MAVLink version, not writable by user, gets added by protocol because of magic data type: uint8_t_mavlink_version </summary>
        public  byte mavlink_version;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=31)]
    public struct mavlink_sys_status_t
    {
        /// <summary> Bitmask showing which onboard controllers and sensors are present. Value of 0: not present. Value of 1: present. Indices defined by ENUM MAV_SYS_STATUS_SENSOR </summary>
        public  UInt32 onboard_control_sensors_present;
            /// <summary> Bitmask showing which onboard controllers and sensors are enabled:  Value of 0: not enabled. Value of 1: enabled. Indices defined by ENUM MAV_SYS_STATUS_SENSOR </summary>
        public  UInt32 onboard_control_sensors_enabled;
            /// <summary> Bitmask showing which onboard controllers and sensors are operational or have an error:  Value of 0: not enabled. Value of 1: enabled. Indices defined by ENUM MAV_SYS_STATUS_SENSOR </summary>
        public  UInt32 onboard_control_sensors_health;
            /// <summary> Maximum usage in percent of the mainloop time, (0%: 0, 100%: 1000) should be always below 1000 </summary>
        public  UInt16 load;
            /// <summary> Battery voltage, in millivolts (1 = 1 millivolt) </summary>
        public  UInt16 voltage_battery;
            /// <summary> Battery current, in 10*milliamperes (1 = 10 milliampere), -1: autopilot does not measure the current </summary>
        public  Int16 current_battery;
            /// <summary> Communication drops in percent, (0%: 0, 100%: 10'000), (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV) </summary>
        public  UInt16 drop_rate_comm;
            /// <summary> Communication errors (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV) </summary>
        public  UInt16 errors_comm;
            /// <summary> Autopilot-specific errors </summary>
        public  UInt16 errors_count1;
            /// <summary> Autopilot-specific errors </summary>
        public  UInt16 errors_count2;
            /// <summary> Autopilot-specific errors </summary>
        public  UInt16 errors_count3;
            /// <summary> Autopilot-specific errors </summary>
        public  UInt16 errors_count4;
            /// <summary> Remaining battery energy: (0%: 0, 100%: 100), -1: autopilot estimate the remaining battery </summary>
        public  byte battery_remaining;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    public struct mavlink_system_time_t
    {
        /// <summary> Timestamp of the master clock in microseconds since UNIX epoch. </summary>
        public  UInt64 time_unix_usec;
            /// <summary> Timestamp of the component clock since boot time in milliseconds. </summary>
        public  UInt32 time_boot_ms;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    public struct mavlink_ping_t
    {
        /// <summary> Unix timestamp in microseconds </summary>
        public  UInt64 time_usec;
            /// <summary> PING sequence </summary>
        public  UInt32 seq;
            /// <summary> 0: request ping from all receiving systems, if greater than 0: message is a ping response and number is the system id of the requesting system </summary>
        public  byte target_system;
            /// <summary> 0: request ping from all receiving components, if greater than 0: message is a ping response and number is the system id of the requesting system </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    public struct mavlink_change_operator_control_t
    {
        /// <summary> System the GCS requests control for </summary>
        public  byte target_system;
            /// <summary> 0: request control of this MAV, 1: Release control of this MAV </summary>
        public  byte control_request;
            /// <summary> 0: key as plaintext, 1-255: future, different hashing/encryption variants. The GCS should in general use the safest mode possible initially and then gradually move down the encryption level if it gets a NACK message indicating an encryption mismatch. </summary>
        public  byte version;
            /// <summary> Password / Key, depending on version plaintext or encrypted. 25 or less characters, NULL terminated. The characters may involve A-Z, a-z, 0-9, and "!?,.-" </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=25)]
		public byte[] passkey;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    public struct mavlink_change_operator_control_ack_t
    {
        /// <summary> ID of the GCS this message  </summary>
        public  byte gcs_system_id;
            /// <summary> 0: request control of this MAV, 1: Release control of this MAV </summary>
        public  byte control_request;
            /// <summary> 0: ACK, 1: NACK: Wrong passkey, 2: NACK: Unsupported passkey encryption method, 3: NACK: Already under control </summary>
        public  byte ack;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    public struct mavlink_auth_key_t
    {
        /// <summary> key </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] key;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    public struct mavlink_set_mode_t
    {
        /// <summary> The new autopilot-specific mode. This field can be ignored by an autopilot. </summary>
        public  UInt32 custom_mode;
            /// <summary> The system setting the mode </summary>
        public  byte target_system;
            /// <summary> The new base mode </summary>
        public  byte base_mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    public struct mavlink_param_request_read_t
    {
        /// <summary> Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored) </summary>
        public  Int16 param_index;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    public struct mavlink_param_request_list_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=25)]
    public struct mavlink_param_value_t
    {
        /// <summary> Onboard parameter value </summary>
        public  Single param_value;
            /// <summary> Total number of onboard parameters </summary>
        public  UInt16 param_count;
            /// <summary> Index of this onboard parameter </summary>
        public  UInt16 param_index;
            /// <summary> Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
            /// <summary> Onboard parameter type: see the MAV_PARAM_TYPE enum for supported data types. </summary>
        public  byte param_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=23)]
    public struct mavlink_param_set_t
    {
        /// <summary> Onboard parameter value </summary>
        public  Single param_value;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
            /// <summary> Onboard parameter type: see the MAV_PARAM_TYPE enum for supported data types. </summary>
        public  byte param_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=30)]
    public struct mavlink_gps_raw_int_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Latitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 lat;
            /// <summary> Longitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 lon;
            /// <summary> Altitude (WGS84), in meters * 1000 (positive for up) </summary>
        public  Int32 alt;
            /// <summary> GPS HDOP horizontal dilution of position in cm (m*100). If unknown, set to: UINT16_MAX </summary>
        public  UInt16 eph;
            /// <summary> GPS VDOP vertical dilution of position in cm (m*100). If unknown, set to: UINT16_MAX </summary>
        public  UInt16 epv;
            /// <summary> GPS ground speed (m/s * 100). If unknown, set to: UINT16_MAX </summary>
        public  UInt16 vel;
            /// <summary> Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX </summary>
        public  UInt16 cog;
            /// <summary> 0-1: no fix, 2: 2D fix, 3: 3D fix. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix. </summary>
        public  byte fix_type;
            /// <summary> Number of satellites visible. If unknown, set to 255 </summary>
        public  byte satellites_visible;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=101)]
    public struct mavlink_gps_status_t
    {
        /// <summary> Number of satellites visible </summary>
        public  byte satellites_visible;
            /// <summary> Global satellite ID </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_prn;
            /// <summary> 0: Satellite not used, 1: used for localization </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_used;
            /// <summary> Elevation (0: right on top of receiver, 90: on the horizon) of satellite </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_elevation;
            /// <summary> Direction of satellite, 0: 0 deg, 255: 360 deg. </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_azimuth;
            /// <summary> Signal to noise ratio of satellite </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_snr;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    public struct mavlink_scaled_imu_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> X acceleration (mg) </summary>
        public  Int16 xacc;
            /// <summary> Y acceleration (mg) </summary>
        public  Int16 yacc;
            /// <summary> Z acceleration (mg) </summary>
        public  Int16 zacc;
            /// <summary> Angular speed around X axis (millirad /sec) </summary>
        public  Int16 xgyro;
            /// <summary> Angular speed around Y axis (millirad /sec) </summary>
        public  Int16 ygyro;
            /// <summary> Angular speed around Z axis (millirad /sec) </summary>
        public  Int16 zgyro;
            /// <summary> X Magnetic field (milli tesla) </summary>
        public  Int16 xmag;
            /// <summary> Y Magnetic field (milli tesla) </summary>
        public  Int16 ymag;
            /// <summary> Z Magnetic field (milli tesla) </summary>
        public  Int16 zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    public struct mavlink_raw_imu_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> X acceleration (raw) </summary>
        public  Int16 xacc;
            /// <summary> Y acceleration (raw) </summary>
        public  Int16 yacc;
            /// <summary> Z acceleration (raw) </summary>
        public  Int16 zacc;
            /// <summary> Angular speed around X axis (raw) </summary>
        public  Int16 xgyro;
            /// <summary> Angular speed around Y axis (raw) </summary>
        public  Int16 ygyro;
            /// <summary> Angular speed around Z axis (raw) </summary>
        public  Int16 zgyro;
            /// <summary> X Magnetic field (raw) </summary>
        public  Int16 xmag;
            /// <summary> Y Magnetic field (raw) </summary>
        public  Int16 ymag;
            /// <summary> Z Magnetic field (raw) </summary>
        public  Int16 zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=16)]
    public struct mavlink_raw_pressure_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Absolute pressure (raw) </summary>
        public  Int16 press_abs;
            /// <summary> Differential pressure 1 (raw) </summary>
        public  Int16 press_diff1;
            /// <summary> Differential pressure 2 (raw) </summary>
        public  Int16 press_diff2;
            /// <summary> Raw Temperature measurement (raw) </summary>
        public  Int16 temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    public struct mavlink_scaled_pressure_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Absolute pressure (hectopascal) </summary>
        public  Single press_abs;
            /// <summary> Differential pressure 1 (hectopascal) </summary>
        public  Single press_diff;
            /// <summary> Temperature measurement (0.01 degrees celsius) </summary>
        public  Int16 temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    public struct mavlink_attitude_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Roll angle (rad, -pi..+pi) </summary>
        public  Single roll;
            /// <summary> Pitch angle (rad, -pi..+pi) </summary>
        public  Single pitch;
            /// <summary> Yaw angle (rad, -pi..+pi) </summary>
        public  Single yaw;
            /// <summary> Roll angular speed (rad/s) </summary>
        public  Single rollspeed;
            /// <summary> Pitch angular speed (rad/s) </summary>
        public  Single pitchspeed;
            /// <summary> Yaw angular speed (rad/s) </summary>
        public  Single yawspeed;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    public struct mavlink_attitude_quaternion_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Quaternion component 1 </summary>
        public  Single q1;
            /// <summary> Quaternion component 2 </summary>
        public  Single q2;
            /// <summary> Quaternion component 3 </summary>
        public  Single q3;
            /// <summary> Quaternion component 4 </summary>
        public  Single q4;
            /// <summary> Roll angular speed (rad/s) </summary>
        public  Single rollspeed;
            /// <summary> Pitch angular speed (rad/s) </summary>
        public  Single pitchspeed;
            /// <summary> Yaw angular speed (rad/s) </summary>
        public  Single yawspeed;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    public struct mavlink_local_position_ned_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> X Position </summary>
        public  Single x;
            /// <summary> Y Position </summary>
        public  Single y;
            /// <summary> Z Position </summary>
        public  Single z;
            /// <summary> X Speed </summary>
        public  Single vx;
            /// <summary> Y Speed </summary>
        public  Single vy;
            /// <summary> Z Speed </summary>
        public  Single vz;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    public struct mavlink_global_position_int_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Latitude, expressed as * 1E7 </summary>
        public  Int32 lat;
            /// <summary> Longitude, expressed as * 1E7 </summary>
        public  Int32 lon;
            /// <summary> Altitude in meters, expressed as * 1000 (millimeters), above MSL </summary>
        public  Int32 alt;
            /// <summary> Altitude above ground in meters, expressed as * 1000 (millimeters) </summary>
        public  Int32 relative_alt;
            /// <summary> Ground X Speed (Latitude), expressed as m/s * 100 </summary>
        public  Int16 vx;
            /// <summary> Ground Y Speed (Longitude), expressed as m/s * 100 </summary>
        public  Int16 vy;
            /// <summary> Ground Z Speed (Altitude), expressed as m/s * 100 </summary>
        public  Int16 vz;
            /// <summary> Compass heading in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX </summary>
        public  UInt16 hdg;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    public struct mavlink_rc_channels_scaled_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> RC channel 1 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. </summary>
        public  Int16 chan1_scaled;
            /// <summary> RC channel 2 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. </summary>
        public  Int16 chan2_scaled;
            /// <summary> RC channel 3 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. </summary>
        public  Int16 chan3_scaled;
            /// <summary> RC channel 4 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. </summary>
        public  Int16 chan4_scaled;
            /// <summary> RC channel 5 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. </summary>
        public  Int16 chan5_scaled;
            /// <summary> RC channel 6 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. </summary>
        public  Int16 chan6_scaled;
            /// <summary> RC channel 7 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. </summary>
        public  Int16 chan7_scaled;
            /// <summary> RC channel 8 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. </summary>
        public  Int16 chan8_scaled;
            /// <summary> Servo output port (set of 8 outputs = 1 port). Most MAVs will just use one, but this allows for more than 8 servos. </summary>
        public  byte port;
            /// <summary> Receive signal strength indicator, 0: 0%, 100: 100%, 255: invalid/unknown. </summary>
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    public struct mavlink_rc_channels_raw_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> RC channel 1 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan1_raw;
            /// <summary> RC channel 2 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan2_raw;
            /// <summary> RC channel 3 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan3_raw;
            /// <summary> RC channel 4 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan4_raw;
            /// <summary> RC channel 5 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan5_raw;
            /// <summary> RC channel 6 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan6_raw;
            /// <summary> RC channel 7 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan7_raw;
            /// <summary> RC channel 8 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan8_raw;
            /// <summary> Servo output port (set of 8 outputs = 1 port). Most MAVs will just use one, but this allows for more than 8 servos. </summary>
        public  byte port;
            /// <summary> Receive signal strength indicator, 0: 0%, 100: 100%, 255: invalid/unknown. </summary>
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=21)]
    public struct mavlink_servo_output_raw_t
    {
        /// <summary> Timestamp (microseconds since system boot) </summary>
        public  UInt32 time_usec;
            /// <summary> Servo output 1 value, in microseconds </summary>
        public  UInt16 servo1_raw;
            /// <summary> Servo output 2 value, in microseconds </summary>
        public  UInt16 servo2_raw;
            /// <summary> Servo output 3 value, in microseconds </summary>
        public  UInt16 servo3_raw;
            /// <summary> Servo output 4 value, in microseconds </summary>
        public  UInt16 servo4_raw;
            /// <summary> Servo output 5 value, in microseconds </summary>
        public  UInt16 servo5_raw;
            /// <summary> Servo output 6 value, in microseconds </summary>
        public  UInt16 servo6_raw;
            /// <summary> Servo output 7 value, in microseconds </summary>
        public  UInt16 servo7_raw;
            /// <summary> Servo output 8 value, in microseconds </summary>
        public  UInt16 servo8_raw;
            /// <summary> Servo output port (set of 8 outputs = 1 port). Most MAVs will just use one, but this allows to encode more than 8 servos. </summary>
        public  byte port;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    public struct mavlink_mission_request_partial_list_t
    {
        /// <summary> Start index, 0 by default </summary>
        public  Int16 start_index;
            /// <summary> End index, -1 by default (-1: send list to end). Else a valid index of the list </summary>
        public  Int16 end_index;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    public struct mavlink_mission_write_partial_list_t
    {
        /// <summary> Start index, 0 by default and smaller / equal to the largest index of the current onboard list. </summary>
        public  Int16 start_index;
            /// <summary> End index, equal or greater than start index. </summary>
        public  Int16 end_index;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    public struct mavlink_mission_item_t
    {
        /// <summary> PARAM1 / For NAV command MISSIONs: Radius in which the MISSION is accepted as reached, in meters </summary>
        public  Single param1;
            /// <summary> PARAM2 / For NAV command MISSIONs: Time that the MAV should stay inside the PARAM1 radius before advancing, in milliseconds </summary>
        public  Single param2;
            /// <summary> PARAM3 / For LOITER command MISSIONs: Orbit to circle around the MISSION, in meters. If positive the orbit direction should be clockwise, if negative the orbit direction should be counter-clockwise. </summary>
        public  Single param3;
            /// <summary> PARAM4 / For NAV and LOITER command MISSIONs: Yaw orientation in degrees, [0..360] 0 = NORTH </summary>
        public  Single param4;
            /// <summary> PARAM5 / local: x position, global: latitude </summary>
        public  Single x;
            /// <summary> PARAM6 / y position: global: longitude </summary>
        public  Single y;
            /// <summary> PARAM7 / z position: global: altitude </summary>
        public  Single z;
            /// <summary> Sequence </summary>
        public  UInt16 seq;
            /// <summary> The scheduled action for the MISSION. see MAV_CMD in common.xml MAVLink specs </summary>
        public  UInt16 command;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> The coordinate system of the MISSION. see MAV_FRAME in mavlink_types.h </summary>
        public  byte frame;
            /// <summary> false:0, true:1 </summary>
        public  byte current;
            /// <summary> autocontinue to next wp </summary>
        public  byte autocontinue;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    public struct mavlink_mission_request_t
    {
        /// <summary> Sequence </summary>
        public  UInt16 seq;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    public struct mavlink_mission_set_current_t
    {
        /// <summary> Sequence </summary>
        public  UInt16 seq;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    public struct mavlink_mission_current_t
    {
        /// <summary> Sequence </summary>
        public  UInt16 seq;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    public struct mavlink_mission_request_list_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    public struct mavlink_mission_count_t
    {
        /// <summary> Number of mission items in the sequence </summary>
        public  UInt16 count;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    public struct mavlink_mission_clear_all_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    public struct mavlink_mission_item_reached_t
    {
        /// <summary> Sequence </summary>
        public  UInt16 seq;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    public struct mavlink_mission_ack_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> See MAV_MISSION_RESULT enum </summary>
        public  byte type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=13)]
    public struct mavlink_set_gps_global_origin_t
    {
        /// <summary> Latitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 latitude;
            /// <summary> Longitude (WGS84, in degrees * 1E7 </summary>
        public  Int32 longitude;
            /// <summary> Altitude (WGS84), in meters * 1000 (positive for up) </summary>
        public  Int32 altitude;
            /// <summary> System ID </summary>
        public  byte target_system;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    public struct mavlink_gps_global_origin_t
    {
        /// <summary> Latitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 latitude;
            /// <summary> Longitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 longitude;
            /// <summary> Altitude (WGS84), in meters * 1000 (positive for up) </summary>
        public  Int32 altitude;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=19)]
    public struct mavlink_set_local_position_setpoint_t
    {
        /// <summary> x position </summary>
        public  Single x;
            /// <summary> y position </summary>
        public  Single y;
            /// <summary> z position </summary>
        public  Single z;
            /// <summary> Desired yaw angle </summary>
        public  Single yaw;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> Coordinate frame - valid values are only MAV_FRAME_LOCAL_NED or MAV_FRAME_LOCAL_ENU </summary>
        public  byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=17)]
    public struct mavlink_local_position_setpoint_t
    {
        /// <summary> x position </summary>
        public  Single x;
            /// <summary> y position </summary>
        public  Single y;
            /// <summary> z position </summary>
        public  Single z;
            /// <summary> Desired yaw angle </summary>
        public  Single yaw;
            /// <summary> Coordinate frame - valid values are only MAV_FRAME_LOCAL_NED or MAV_FRAME_LOCAL_ENU </summary>
        public  byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=15)]
    public struct mavlink_global_position_setpoint_int_t
    {
        /// <summary> Latitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 latitude;
            /// <summary> Longitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 longitude;
            /// <summary> Altitude (WGS84), in meters * 1000 (positive for up) </summary>
        public  Int32 altitude;
            /// <summary> Desired yaw angle in degrees * 100 </summary>
        public  Int16 yaw;
            /// <summary> Coordinate frame - valid values are only MAV_FRAME_GLOBAL or MAV_FRAME_GLOBAL_RELATIVE_ALT </summary>
        public  byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=15)]
    public struct mavlink_set_global_position_setpoint_int_t
    {
        /// <summary> Latitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 latitude;
            /// <summary> Longitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 longitude;
            /// <summary> Altitude (WGS84), in meters * 1000 (positive for up) </summary>
        public  Int32 altitude;
            /// <summary> Desired yaw angle in degrees * 100 </summary>
        public  Int16 yaw;
            /// <summary> Coordinate frame - valid values are only MAV_FRAME_GLOBAL or MAV_FRAME_GLOBAL_RELATIVE_ALT </summary>
        public  byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=27)]
    public struct mavlink_safety_set_allowed_area_t
    {
        /// <summary> x position 1 / Latitude 1 </summary>
        public  Single p1x;
            /// <summary> y position 1 / Longitude 1 </summary>
        public  Single p1y;
            /// <summary> z position 1 / Altitude 1 </summary>
        public  Single p1z;
            /// <summary> x position 2 / Latitude 2 </summary>
        public  Single p2x;
            /// <summary> y position 2 / Longitude 2 </summary>
        public  Single p2y;
            /// <summary> z position 2 / Altitude 2 </summary>
        public  Single p2z;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> Coordinate frame, as defined by MAV_FRAME enum in mavlink_types.h. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down. </summary>
        public  byte frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=25)]
    public struct mavlink_safety_allowed_area_t
    {
        /// <summary> x position 1 / Latitude 1 </summary>
        public  Single p1x;
            /// <summary> y position 1 / Longitude 1 </summary>
        public  Single p1y;
            /// <summary> z position 1 / Altitude 1 </summary>
        public  Single p1z;
            /// <summary> x position 2 / Latitude 2 </summary>
        public  Single p2x;
            /// <summary> y position 2 / Longitude 2 </summary>
        public  Single p2y;
            /// <summary> z position 2 / Altitude 2 </summary>
        public  Single p2z;
            /// <summary> Coordinate frame, as defined by MAV_FRAME enum in mavlink_types.h. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down. </summary>
        public  byte frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    public struct mavlink_set_roll_pitch_yaw_thrust_t
    {
        /// <summary> Desired roll angle in radians </summary>
        public  Single roll;
            /// <summary> Desired pitch angle in radians </summary>
        public  Single pitch;
            /// <summary> Desired yaw angle in radians </summary>
        public  Single yaw;
            /// <summary> Collective thrust, normalized to 0 .. 1 </summary>
        public  Single thrust;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    public struct mavlink_set_roll_pitch_yaw_speed_thrust_t
    {
        /// <summary> Desired roll angular speed in rad/s </summary>
        public  Single roll_speed;
            /// <summary> Desired pitch angular speed in rad/s </summary>
        public  Single pitch_speed;
            /// <summary> Desired yaw angular speed in rad/s </summary>
        public  Single yaw_speed;
            /// <summary> Collective thrust, normalized to 0 .. 1 </summary>
        public  Single thrust;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    public struct mavlink_roll_pitch_yaw_thrust_setpoint_t
    {
        /// <summary> Timestamp in milliseconds since system boot </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Desired roll angle in radians </summary>
        public  Single roll;
            /// <summary> Desired pitch angle in radians </summary>
        public  Single pitch;
            /// <summary> Desired yaw angle in radians </summary>
        public  Single yaw;
            /// <summary> Collective thrust, normalized to 0 .. 1 </summary>
        public  Single thrust;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    public struct mavlink_roll_pitch_yaw_speed_thrust_setpoint_t
    {
        /// <summary> Timestamp in milliseconds since system boot </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Desired roll angular speed in rad/s </summary>
        public  Single roll_speed;
            /// <summary> Desired pitch angular speed in rad/s </summary>
        public  Single pitch_speed;
            /// <summary> Desired yaw angular speed in rad/s </summary>
        public  Single yaw_speed;
            /// <summary> Collective thrust, normalized to 0 .. 1 </summary>
        public  Single thrust;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    public struct mavlink_set_quad_motors_setpoint_t
    {
        /// <summary> Front motor in + configuration, front left motor in x configuration </summary>
        public  UInt16 motor_front_nw;
            /// <summary> Right motor in + configuration, front right motor in x configuration </summary>
        public  UInt16 motor_right_ne;
            /// <summary> Back motor in + configuration, back right motor in x configuration </summary>
        public  UInt16 motor_back_se;
            /// <summary> Left motor in + configuration, back left motor in x configuration </summary>
        public  UInt16 motor_left_sw;
            /// <summary> System ID of the system that should set these motor commands </summary>
        public  byte target_system;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=34)]
    public struct mavlink_set_quad_swarm_roll_pitch_yaw_thrust_t
    {
        /// <summary> Desired roll angle in radians +-PI (+-INT16_MAX) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public Int16[] roll;
            /// <summary> Desired pitch angle in radians +-PI (+-INT16_MAX) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public Int16[] pitch;
            /// <summary> Desired yaw angle in radians, scaled to int16 +-PI (+-INT16_MAX) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public Int16[] yaw;
            /// <summary> Collective thrust, scaled to uint16 (0..UINT16_MAX) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] thrust;
            /// <summary> ID of the quadrotor group (0 - 255, up to 256 groups supported) </summary>
        public  byte group;
            /// <summary> ID of the flight mode (0 - 255, up to 256 modes supported) </summary>
        public  byte mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    public struct mavlink_nav_controller_output_t
    {
        /// <summary> Current desired roll in degrees </summary>
        public  Single nav_roll;
            /// <summary> Current desired pitch in degrees </summary>
        public  Single nav_pitch;
            /// <summary> Current altitude error in meters </summary>
        public  Single alt_error;
            /// <summary> Current airspeed error in meters/second </summary>
        public  Single aspd_error;
            /// <summary> Current crosstrack error on x-y plane in meters </summary>
        public  Single xtrack_error;
            /// <summary> Current desired heading in degrees </summary>
        public  Int16 nav_bearing;
            /// <summary> Bearing to current MISSION/target in degrees </summary>
        public  Int16 target_bearing;
            /// <summary> Distance to active MISSION in meters </summary>
        public  UInt16 wp_dist;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=46)]
    public struct mavlink_set_quad_swarm_led_roll_pitch_yaw_thrust_t
    {
        /// <summary> Desired roll angle in radians +-PI (+-INT16_MAX) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public Int16[] roll;
            /// <summary> Desired pitch angle in radians +-PI (+-INT16_MAX) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public Int16[] pitch;
            /// <summary> Desired yaw angle in radians, scaled to int16 +-PI (+-INT16_MAX) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public Int16[] yaw;
            /// <summary> Collective thrust, scaled to uint16 (0..UINT16_MAX) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] thrust;
            /// <summary> ID of the quadrotor group (0 - 255, up to 256 groups supported) </summary>
        public  byte group;
            /// <summary> ID of the flight mode (0 - 255, up to 256 modes supported) </summary>
        public  byte mode;
            /// <summary> RGB red channel (0-255) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] led_red;
            /// <summary> RGB green channel (0-255) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] led_blue;
            /// <summary> RGB blue channel (0-255) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] led_green;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=36)]
    public struct mavlink_state_correction_t
    {
        /// <summary> x position error </summary>
        public  Single xErr;
            /// <summary> y position error </summary>
        public  Single yErr;
            /// <summary> z position error </summary>
        public  Single zErr;
            /// <summary> roll error (radians) </summary>
        public  Single rollErr;
            /// <summary> pitch error (radians) </summary>
        public  Single pitchErr;
            /// <summary> yaw error (radians) </summary>
        public  Single yawErr;
            /// <summary> x velocity </summary>
        public  Single vxErr;
            /// <summary> y velocity </summary>
        public  Single vyErr;
            /// <summary> z velocity </summary>
        public  Single vzErr;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    public struct mavlink_request_data_stream_t
    {
        /// <summary> The requested interval between two messages of this type </summary>
        public  UInt16 req_message_rate;
            /// <summary> The target requested to send the message stream. </summary>
        public  byte target_system;
            /// <summary> The target requested to send the message stream. </summary>
        public  byte target_component;
            /// <summary> The ID of the requested data stream </summary>
        public  byte req_stream_id;
            /// <summary> 1 to start sending, 0 to stop sending. </summary>
        public  byte start_stop;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    public struct mavlink_data_stream_t
    {
        /// <summary> The requested interval between two messages of this type </summary>
        public  UInt16 message_rate;
            /// <summary> The ID of the requested data stream </summary>
        public  byte stream_id;
            /// <summary> 1 stream is enabled, 0 stream is stopped. </summary>
        public  byte on_off;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=11)]
    public struct mavlink_manual_control_t
    {
        /// <summary> X-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to forward(1000)-backward(-1000) movement on a joystick and the pitch of a vehicle. </summary>
        public  Int16 x;
            /// <summary> Y-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to left(-1000)-right(1000) movement on a joystick and the roll of a vehicle. </summary>
        public  Int16 y;
            /// <summary> Z-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a separate slider movement with maximum being 1000 and minimum being -1000 on a joystick and the thrust of a vehicle. </summary>
        public  Int16 z;
            /// <summary> R-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a twisting of the joystick, with counter-clockwise being 1000 and clockwise being -1000, and the yaw of a vehicle. </summary>
        public  Int16 r;
            /// <summary> A bitfield corresponding to the joystick buttons' current state, 1 for pressed, 0 for released. The lowest bit corresponds to Button 1. </summary>
        public  UInt16 buttons;
            /// <summary> The system to be controlled. </summary>
        public  byte target;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    public struct mavlink_rc_channels_override_t
    {
        /// <summary> RC channel 1 value, in microseconds. A value of UINT16_MAX means to ignore this field. </summary>
        public  UInt16 chan1_raw;
            /// <summary> RC channel 2 value, in microseconds. A value of UINT16_MAX means to ignore this field. </summary>
        public  UInt16 chan2_raw;
            /// <summary> RC channel 3 value, in microseconds. A value of UINT16_MAX means to ignore this field. </summary>
        public  UInt16 chan3_raw;
            /// <summary> RC channel 4 value, in microseconds. A value of UINT16_MAX means to ignore this field. </summary>
        public  UInt16 chan4_raw;
            /// <summary> RC channel 5 value, in microseconds. A value of UINT16_MAX means to ignore this field. </summary>
        public  UInt16 chan5_raw;
            /// <summary> RC channel 6 value, in microseconds. A value of UINT16_MAX means to ignore this field. </summary>
        public  UInt16 chan6_raw;
            /// <summary> RC channel 7 value, in microseconds. A value of UINT16_MAX means to ignore this field. </summary>
        public  UInt16 chan7_raw;
            /// <summary> RC channel 8 value, in microseconds. A value of UINT16_MAX means to ignore this field. </summary>
        public  UInt16 chan8_raw;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    public struct mavlink_vfr_hud_t
    {
        /// <summary> Current airspeed in m/s </summary>
        public  Single airspeed;
            /// <summary> Current ground speed in m/s </summary>
        public  Single groundspeed;
            /// <summary> Current altitude (MSL), in meters </summary>
        public  Single alt;
            /// <summary> Current climb rate in meters/second </summary>
        public  Single climb;
            /// <summary> Current heading in degrees, in compass units (0..360, 0=north) </summary>
        public  Int16 heading;
            /// <summary> Current throttle setting in integer percent, 0 to 100 </summary>
        public  UInt16 throttle;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=33)]
    public struct mavlink_command_long_t
    {
        /// <summary> Parameter 1, as defined by MAV_CMD enum. </summary>
        public  Single param1;
            /// <summary> Parameter 2, as defined by MAV_CMD enum. </summary>
        public  Single param2;
            /// <summary> Parameter 3, as defined by MAV_CMD enum. </summary>
        public  Single param3;
            /// <summary> Parameter 4, as defined by MAV_CMD enum. </summary>
        public  Single param4;
            /// <summary> Parameter 5, as defined by MAV_CMD enum. </summary>
        public  Single param5;
            /// <summary> Parameter 6, as defined by MAV_CMD enum. </summary>
        public  Single param6;
            /// <summary> Parameter 7, as defined by MAV_CMD enum. </summary>
        public  Single param7;
            /// <summary> Command ID, as defined by MAV_CMD enum. </summary>
        public  UInt16 command;
            /// <summary> System which should execute the command </summary>
        public  byte target_system;
            /// <summary> Component which should execute the command, 0 for all components </summary>
        public  byte target_component;
            /// <summary> 0: First transmission of this command. 1-255: Confirmation transmissions (e.g. for kill command) </summary>
        public  byte confirmation;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    public struct mavlink_command_ack_t
    {
        /// <summary> Command ID, as defined by MAV_CMD enum. </summary>
        public  UInt16 command;
            /// <summary> See MAV_RESULT enum </summary>
        public  byte result;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    public struct mavlink_roll_pitch_yaw_rates_thrust_setpoint_t
    {
        /// <summary> Timestamp in milliseconds since system boot </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Desired roll rate in radians per second </summary>
        public  Single roll_rate;
            /// <summary> Desired pitch rate in radians per second </summary>
        public  Single pitch_rate;
            /// <summary> Desired yaw rate in radians per second </summary>
        public  Single yaw_rate;
            /// <summary> Collective thrust, normalized to 0 .. 1 </summary>
        public  Single thrust;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    public struct mavlink_manual_setpoint_t
    {
        /// <summary> Timestamp in milliseconds since system boot </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Desired roll rate in radians per second </summary>
        public  Single roll;
            /// <summary> Desired pitch rate in radians per second </summary>
        public  Single pitch;
            /// <summary> Desired yaw rate in radians per second </summary>
        public  Single yaw;
            /// <summary> Collective thrust, normalized to 0 .. 1 </summary>
        public  Single thrust;
            /// <summary> Flight mode switch position, 0.. 255 </summary>
        public  byte mode_switch;
            /// <summary> Override mode switch position, 0.. 255 </summary>
        public  byte manual_override_switch;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    public struct mavlink_local_position_ned_system_global_offset_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> X Position </summary>
        public  Single x;
            /// <summary> Y Position </summary>
        public  Single y;
            /// <summary> Z Position </summary>
        public  Single z;
            /// <summary> Roll </summary>
        public  Single roll;
            /// <summary> Pitch </summary>
        public  Single pitch;
            /// <summary> Yaw </summary>
        public  Single yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=56)]
    public struct mavlink_hil_state_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Roll angle (rad) </summary>
        public  Single roll;
            /// <summary> Pitch angle (rad) </summary>
        public  Single pitch;
            /// <summary> Yaw angle (rad) </summary>
        public  Single yaw;
            /// <summary> Body frame roll / phi angular speed (rad/s) </summary>
        public  Single rollspeed;
            /// <summary> Body frame pitch / theta angular speed (rad/s) </summary>
        public  Single pitchspeed;
            /// <summary> Body frame yaw / psi angular speed (rad/s) </summary>
        public  Single yawspeed;
            /// <summary> Latitude, expressed as * 1E7 </summary>
        public  Int32 lat;
            /// <summary> Longitude, expressed as * 1E7 </summary>
        public  Int32 lon;
            /// <summary> Altitude in meters, expressed as * 1000 (millimeters) </summary>
        public  Int32 alt;
            /// <summary> Ground X Speed (Latitude), expressed as m/s * 100 </summary>
        public  Int16 vx;
            /// <summary> Ground Y Speed (Longitude), expressed as m/s * 100 </summary>
        public  Int16 vy;
            /// <summary> Ground Z Speed (Altitude), expressed as m/s * 100 </summary>
        public  Int16 vz;
            /// <summary> X acceleration (mg) </summary>
        public  Int16 xacc;
            /// <summary> Y acceleration (mg) </summary>
        public  Int16 yacc;
            /// <summary> Z acceleration (mg) </summary>
        public  Int16 zacc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    public struct mavlink_hil_controls_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Control output -1 .. 1 </summary>
        public  Single roll_ailerons;
            /// <summary> Control output -1 .. 1 </summary>
        public  Single pitch_elevator;
            /// <summary> Control output -1 .. 1 </summary>
        public  Single yaw_rudder;
            /// <summary> Throttle 0 .. 1 </summary>
        public  Single throttle;
            /// <summary> Aux 1, -1 .. 1 </summary>
        public  Single aux1;
            /// <summary> Aux 2, -1 .. 1 </summary>
        public  Single aux2;
            /// <summary> Aux 3, -1 .. 1 </summary>
        public  Single aux3;
            /// <summary> Aux 4, -1 .. 1 </summary>
        public  Single aux4;
            /// <summary> System mode (MAV_MODE) </summary>
        public  byte mode;
            /// <summary> Navigation mode (MAV_NAV_MODE) </summary>
        public  byte nav_mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=33)]
    public struct mavlink_hil_rc_inputs_raw_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> RC channel 1 value, in microseconds </summary>
        public  UInt16 chan1_raw;
            /// <summary> RC channel 2 value, in microseconds </summary>
        public  UInt16 chan2_raw;
            /// <summary> RC channel 3 value, in microseconds </summary>
        public  UInt16 chan3_raw;
            /// <summary> RC channel 4 value, in microseconds </summary>
        public  UInt16 chan4_raw;
            /// <summary> RC channel 5 value, in microseconds </summary>
        public  UInt16 chan5_raw;
            /// <summary> RC channel 6 value, in microseconds </summary>
        public  UInt16 chan6_raw;
            /// <summary> RC channel 7 value, in microseconds </summary>
        public  UInt16 chan7_raw;
            /// <summary> RC channel 8 value, in microseconds </summary>
        public  UInt16 chan8_raw;
            /// <summary> RC channel 9 value, in microseconds </summary>
        public  UInt16 chan9_raw;
            /// <summary> RC channel 10 value, in microseconds </summary>
        public  UInt16 chan10_raw;
            /// <summary> RC channel 11 value, in microseconds </summary>
        public  UInt16 chan11_raw;
            /// <summary> RC channel 12 value, in microseconds </summary>
        public  UInt16 chan12_raw;
            /// <summary> Receive signal strength indicator, 0: 0%, 255: 100% </summary>
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    public struct mavlink_optical_flow_t
    {
        /// <summary> Timestamp (UNIX) </summary>
        public  UInt64 time_usec;
            /// <summary> Flow in meters in x-sensor direction, angular-speed compensated </summary>
        public  Single flow_comp_m_x;
            /// <summary> Flow in meters in y-sensor direction, angular-speed compensated </summary>
        public  Single flow_comp_m_y;
            /// <summary> Ground distance in meters. Positive value: distance known. Negative value: Unknown distance </summary>
        public  Single ground_distance;
            /// <summary> Flow in pixels * 10 in x-sensor direction (dezi-pixels) </summary>
        public  Int16 flow_x;
            /// <summary> Flow in pixels * 10 in y-sensor direction (dezi-pixels) </summary>
        public  Int16 flow_y;
            /// <summary> Sensor ID </summary>
        public  byte sensor_id;
            /// <summary> Optical flow quality / confidence. 0: bad, 255: maximum quality </summary>
        public  byte quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    public struct mavlink_global_vision_position_estimate_t
    {
        /// <summary> Timestamp (microseconds, synced to UNIX time or since system boot) </summary>
        public  UInt64 usec;
            /// <summary> Global X position </summary>
        public  Single x;
            /// <summary> Global Y position </summary>
        public  Single y;
            /// <summary> Global Z position </summary>
        public  Single z;
            /// <summary> Roll angle in rad </summary>
        public  Single roll;
            /// <summary> Pitch angle in rad </summary>
        public  Single pitch;
            /// <summary> Yaw angle in rad </summary>
        public  Single yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    public struct mavlink_vision_position_estimate_t
    {
        /// <summary> Timestamp (microseconds, synced to UNIX time or since system boot) </summary>
        public  UInt64 usec;
            /// <summary> Global X position </summary>
        public  Single x;
            /// <summary> Global Y position </summary>
        public  Single y;
            /// <summary> Global Z position </summary>
        public  Single z;
            /// <summary> Roll angle in rad </summary>
        public  Single roll;
            /// <summary> Pitch angle in rad </summary>
        public  Single pitch;
            /// <summary> Yaw angle in rad </summary>
        public  Single yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    public struct mavlink_vision_speed_estimate_t
    {
        /// <summary> Timestamp (microseconds, synced to UNIX time or since system boot) </summary>
        public  UInt64 usec;
            /// <summary> Global X speed </summary>
        public  Single x;
            /// <summary> Global Y speed </summary>
        public  Single y;
            /// <summary> Global Z speed </summary>
        public  Single z;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    public struct mavlink_vicon_position_estimate_t
    {
        /// <summary> Timestamp (microseconds, synced to UNIX time or since system boot) </summary>
        public  UInt64 usec;
            /// <summary> Global X position </summary>
        public  Single x;
            /// <summary> Global Y position </summary>
        public  Single y;
            /// <summary> Global Z position </summary>
        public  Single z;
            /// <summary> Roll angle in rad </summary>
        public  Single roll;
            /// <summary> Pitch angle in rad </summary>
        public  Single pitch;
            /// <summary> Yaw angle in rad </summary>
        public  Single yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=62)]
    public struct mavlink_highres_imu_t
    {
        /// <summary> Timestamp (microseconds, synced to UNIX time or since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> X acceleration (m/s^2) </summary>
        public  Single xacc;
            /// <summary> Y acceleration (m/s^2) </summary>
        public  Single yacc;
            /// <summary> Z acceleration (m/s^2) </summary>
        public  Single zacc;
            /// <summary> Angular speed around X axis (rad / sec) </summary>
        public  Single xgyro;
            /// <summary> Angular speed around Y axis (rad / sec) </summary>
        public  Single ygyro;
            /// <summary> Angular speed around Z axis (rad / sec) </summary>
        public  Single zgyro;
            /// <summary> X Magnetic field (Gauss) </summary>
        public  Single xmag;
            /// <summary> Y Magnetic field (Gauss) </summary>
        public  Single ymag;
            /// <summary> Z Magnetic field (Gauss) </summary>
        public  Single zmag;
            /// <summary> Absolute pressure in millibar </summary>
        public  Single abs_pressure;
            /// <summary> Differential pressure in millibar </summary>
        public  Single diff_pressure;
            /// <summary> Altitude calculated from pressure </summary>
        public  Single pressure_alt;
            /// <summary> Temperature in degrees celsius </summary>
        public  Single temperature;
            /// <summary> Bitmask for fields that have updated since last message, bit 0 = xacc, bit 12: temperature </summary>
        public  UInt16 fields_updated;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=54)]
    public struct mavlink_omnidirectional_flow_t
    {
        /// <summary> Timestamp (microseconds, synced to UNIX time or since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Front distance in meters. Positive value (including zero): distance known. Negative value: Unknown distance </summary>
        public  Single front_distance_m;
            /// <summary> Flow in deci pixels (1 = 0.1 pixel) on left hemisphere </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public Int16[] left;
            /// <summary> Flow in deci pixels (1 = 0.1 pixel) on right hemisphere </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public Int16[] right;
            /// <summary> Sensor ID </summary>
        public  byte sensor_id;
            /// <summary> Optical flow quality / confidence. 0: bad, 255: maximum quality </summary>
        public  byte quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=64)]
    public struct mavlink_hil_sensor_t
    {
        /// <summary> Timestamp (microseconds, synced to UNIX time or since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> X acceleration (m/s^2) </summary>
        public  Single xacc;
            /// <summary> Y acceleration (m/s^2) </summary>
        public  Single yacc;
            /// <summary> Z acceleration (m/s^2) </summary>
        public  Single zacc;
            /// <summary> Angular speed around X axis in body frame (rad / sec) </summary>
        public  Single xgyro;
            /// <summary> Angular speed around Y axis in body frame (rad / sec) </summary>
        public  Single ygyro;
            /// <summary> Angular speed around Z axis in body frame (rad / sec) </summary>
        public  Single zgyro;
            /// <summary> X Magnetic field (Gauss) </summary>
        public  Single xmag;
            /// <summary> Y Magnetic field (Gauss) </summary>
        public  Single ymag;
            /// <summary> Z Magnetic field (Gauss) </summary>
        public  Single zmag;
            /// <summary> Absolute pressure in millibar </summary>
        public  Single abs_pressure;
            /// <summary> Differential pressure (airspeed) in millibar </summary>
        public  Single diff_pressure;
            /// <summary> Altitude calculated from pressure </summary>
        public  Single pressure_alt;
            /// <summary> Temperature in degrees celsius </summary>
        public  Single temperature;
            /// <summary> Bitmask for fields that have updated since last message, bit 0 = xacc, bit 12: temperature </summary>
        public  UInt32 fields_updated;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=84)]
    public struct mavlink_sim_state_t
    {
        /// <summary> True attitude quaternion component 1 </summary>
        public  Single q1;
            /// <summary> True attitude quaternion component 2 </summary>
        public  Single q2;
            /// <summary> True attitude quaternion component 3 </summary>
        public  Single q3;
            /// <summary> True attitude quaternion component 4 </summary>
        public  Single q4;
            /// <summary> Attitude roll expressed as Euler angles, not recommended except for human-readable outputs </summary>
        public  Single roll;
            /// <summary> Attitude pitch expressed as Euler angles, not recommended except for human-readable outputs </summary>
        public  Single pitch;
            /// <summary> Attitude yaw expressed as Euler angles, not recommended except for human-readable outputs </summary>
        public  Single yaw;
            /// <summary> X acceleration m/s/s </summary>
        public  Single xacc;
            /// <summary> Y acceleration m/s/s </summary>
        public  Single yacc;
            /// <summary> Z acceleration m/s/s </summary>
        public  Single zacc;
            /// <summary> Angular speed around X axis rad/s </summary>
        public  Single xgyro;
            /// <summary> Angular speed around Y axis rad/s </summary>
        public  Single ygyro;
            /// <summary> Angular speed around Z axis rad/s </summary>
        public  Single zgyro;
            /// <summary> Latitude in degrees </summary>
        public  Single lat;
            /// <summary> Longitude in degrees </summary>
        public  Single lon;
            /// <summary> Altitude in meters </summary>
        public  Single alt;
            /// <summary> Horizontal position standard deviation </summary>
        public  Single std_dev_horz;
            /// <summary> Vertical position standard deviation </summary>
        public  Single std_dev_vert;
            /// <summary> True velocity in m/s in NORTH direction in earth-fixed NED frame </summary>
        public  Single vn;
            /// <summary> True velocity in m/s in EAST direction in earth-fixed NED frame </summary>
        public  Single ve;
            /// <summary> True velocity in m/s in DOWN direction in earth-fixed NED frame </summary>
        public  Single vd;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    public struct mavlink_radio_status_t
    {
        /// <summary> receive errors </summary>
        public  UInt16 rxerrors;
            /// <summary> count of error corrected packets </summary>
        public  UInt16 @fixed;
            /// <summary> local signal strength </summary>
        public  byte rssi;
            /// <summary> remote signal strength </summary>
        public  byte remrssi;
            /// <summary> how full the tx buffer is as a percentage </summary>
        public  byte txbuf;
            /// <summary> background noise level </summary>
        public  byte noise;
            /// <summary> remote background noise level </summary>
        public  byte remnoise;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=254)]
    public struct mavlink_file_transfer_start_t
    {
        /// <summary> Unique transfer ID </summary>
        public  UInt64 transfer_uid;
            /// <summary> File size in bytes </summary>
        public  UInt32 file_size;
            /// <summary> Destination path </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=240)]
		public byte[] dest_path;
            /// <summary> Transfer direction: 0: from requester, 1: to requester </summary>
        public  byte direction;
            /// <summary> RESERVED </summary>
        public  byte flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=249)]
    public struct mavlink_file_transfer_dir_list_t
    {
        /// <summary> Unique transfer ID </summary>
        public  UInt64 transfer_uid;
            /// <summary> Directory path to list </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=240)]
		public byte[] dir_path;
            /// <summary> RESERVED </summary>
        public  byte flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    public struct mavlink_file_transfer_res_t
    {
        /// <summary> Unique transfer ID </summary>
        public  UInt64 transfer_uid;
            /// <summary> 0: OK, 1: not permitted, 2: bad path / file name, 3: no space left on device </summary>
        public  byte result;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=36)]
    public struct mavlink_hil_gps_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Latitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 lat;
            /// <summary> Longitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 lon;
            /// <summary> Altitude (WGS84), in meters * 1000 (positive for up) </summary>
        public  Int32 alt;
            /// <summary> GPS HDOP horizontal dilution of position in cm (m*100). If unknown, set to: 65535 </summary>
        public  UInt16 eph;
            /// <summary> GPS VDOP vertical dilution of position in cm (m*100). If unknown, set to: 65535 </summary>
        public  UInt16 epv;
            /// <summary> GPS ground speed (m/s * 100). If unknown, set to: 65535 </summary>
        public  UInt16 vel;
            /// <summary> GPS velocity in cm/s in NORTH direction in earth-fixed NED frame </summary>
        public  Int16 vn;
            /// <summary> GPS velocity in cm/s in EAST direction in earth-fixed NED frame </summary>
        public  Int16 ve;
            /// <summary> GPS velocity in cm/s in DOWN direction in earth-fixed NED frame </summary>
        public  Int16 vd;
            /// <summary> Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: 65535 </summary>
        public  UInt16 cog;
            /// <summary> 0-1: no fix, 2: 2D fix, 3: 3D fix. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix. </summary>
        public  byte fix_type;
            /// <summary> Number of satellites visible. If unknown, set to 255 </summary>
        public  byte satellites_visible;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    public struct mavlink_hil_optical_flow_t
    {
        /// <summary> Timestamp (UNIX) </summary>
        public  UInt64 time_usec;
            /// <summary> Flow in meters in x-sensor direction, angular-speed compensated </summary>
        public  Single flow_comp_m_x;
            /// <summary> Flow in meters in y-sensor direction, angular-speed compensated </summary>
        public  Single flow_comp_m_y;
            /// <summary> Ground distance in meters. Positive value: distance known. Negative value: Unknown distance </summary>
        public  Single ground_distance;
            /// <summary> Flow in pixels in x-sensor direction </summary>
        public  Int16 flow_x;
            /// <summary> Flow in pixels in y-sensor direction </summary>
        public  Int16 flow_y;
            /// <summary> Sensor ID </summary>
        public  byte sensor_id;
            /// <summary> Optical flow quality / confidence. 0: bad, 255: maximum quality </summary>
        public  byte quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=64)]
    public struct mavlink_hil_state_quaternion_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Vehicle attitude expressed as normalized quaternion </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float attitude_quaternion;
            /// <summary> Body frame roll / phi angular speed (rad/s) </summary>
        public  Single rollspeed;
            /// <summary> Body frame pitch / theta angular speed (rad/s) </summary>
        public  Single pitchspeed;
            /// <summary> Body frame yaw / psi angular speed (rad/s) </summary>
        public  Single yawspeed;
            /// <summary> Latitude, expressed as * 1E7 </summary>
        public  Int32 lat;
            /// <summary> Longitude, expressed as * 1E7 </summary>
        public  Int32 lon;
            /// <summary> Altitude in meters, expressed as * 1000 (millimeters) </summary>
        public  Int32 alt;
            /// <summary> Ground X Speed (Latitude), expressed as m/s * 100 </summary>
        public  Int16 vx;
            /// <summary> Ground Y Speed (Longitude), expressed as m/s * 100 </summary>
        public  Int16 vy;
            /// <summary> Ground Z Speed (Altitude), expressed as m/s * 100 </summary>
        public  Int16 vz;
            /// <summary> Indicated airspeed, expressed as m/s * 100 </summary>
        public  UInt16 ind_airspeed;
            /// <summary> True airspeed, expressed as m/s * 100 </summary>
        public  UInt16 true_airspeed;
            /// <summary> X acceleration (mg) </summary>
        public  Int16 xacc;
            /// <summary> Y acceleration (mg) </summary>
        public  Int16 yacc;
            /// <summary> Z acceleration (mg) </summary>
        public  Int16 zacc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    public struct mavlink_scaled_imu2_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> X acceleration (mg) </summary>
        public  Int16 xacc;
            /// <summary> Y acceleration (mg) </summary>
        public  Int16 yacc;
            /// <summary> Z acceleration (mg) </summary>
        public  Int16 zacc;
            /// <summary> Angular speed around X axis (millirad /sec) </summary>
        public  Int16 xgyro;
            /// <summary> Angular speed around Y axis (millirad /sec) </summary>
        public  Int16 ygyro;
            /// <summary> Angular speed around Z axis (millirad /sec) </summary>
        public  Int16 zgyro;
            /// <summary> X Magnetic field (milli tesla) </summary>
        public  Int16 xmag;
            /// <summary> Y Magnetic field (milli tesla) </summary>
        public  Int16 ymag;
            /// <summary> Z Magnetic field (milli tesla) </summary>
        public  Int16 zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=24)]
    public struct mavlink_battery_status_t
    {
        /// <summary> Consumed charge, in milliampere hours (1 = 1 mAh), -1: autopilot does not provide mAh consumption estimate </summary>
        public  Int32 current_consumed;
            /// <summary> Consumed energy, in 100*Joules (intergrated U*I*dt)  (1 = 100 Joule), -1: autopilot does not provide energy consumption estimate </summary>
        public  Int32 energy_consumed;
            /// <summary> Battery voltage of cell 1, in millivolts (1 = 1 millivolt) </summary>
        public  UInt16 voltage_cell_1;
            /// <summary> Battery voltage of cell 2, in millivolts (1 = 1 millivolt), -1: no cell </summary>
        public  UInt16 voltage_cell_2;
            /// <summary> Battery voltage of cell 3, in millivolts (1 = 1 millivolt), -1: no cell </summary>
        public  UInt16 voltage_cell_3;
            /// <summary> Battery voltage of cell 4, in millivolts (1 = 1 millivolt), -1: no cell </summary>
        public  UInt16 voltage_cell_4;
            /// <summary> Battery voltage of cell 5, in millivolts (1 = 1 millivolt), -1: no cell </summary>
        public  UInt16 voltage_cell_5;
            /// <summary> Battery voltage of cell 6, in millivolts (1 = 1 millivolt), -1: no cell </summary>
        public  UInt16 voltage_cell_6;
            /// <summary> Battery current, in 10*milliamperes (1 = 10 milliampere), -1: autopilot does not measure the current </summary>
        public  Int16 current_battery;
            /// <summary> Accupack ID </summary>
        public  byte accu_id;
            /// <summary> Remaining battery energy: (0%: 0, 100%: 100), -1: autopilot does not estimate the remaining battery </summary>
        public  byte battery_remaining;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=33)]
    public struct mavlink_setpoint_8dof_t
    {
        /// <summary> Value 1 </summary>
        public  Single val1;
            /// <summary> Value 2 </summary>
        public  Single val2;
            /// <summary> Value 3 </summary>
        public  Single val3;
            /// <summary> Value 4 </summary>
        public  Single val4;
            /// <summary> Value 5 </summary>
        public  Single val5;
            /// <summary> Value 6 </summary>
        public  Single val6;
            /// <summary> Value 7 </summary>
        public  Single val7;
            /// <summary> Value 8 </summary>
        public  Single val8;
            /// <summary> System ID </summary>
        public  byte target_system;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=25)]
    public struct mavlink_setpoint_6dof_t
    {
        /// <summary> Translational Component in x </summary>
        public  Single trans_x;
            /// <summary> Translational Component in y </summary>
        public  Single trans_y;
            /// <summary> Translational Component in z </summary>
        public  Single trans_z;
            /// <summary> Rotational Component in x </summary>
        public  Single rot_x;
            /// <summary> Rotational Component in y </summary>
        public  Single rot_y;
            /// <summary> Rotational Component in z </summary>
        public  Single rot_z;
            /// <summary> System ID </summary>
        public  byte target_system;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=36)]
    public struct mavlink_memory_vect_t
    {
        /// <summary> Starting address of the debug variables </summary>
        public  UInt16 address;
            /// <summary> Version code of the type variable. 0=unknown, type ignored and assumed int16_t. 1=as below </summary>
        public  byte ver;
            /// <summary> Type code of the memory variables. for ver = 1: 0=16 x int16_t, 1=16 x uint16_t, 2=16 x Q15, 3=16 x 1Q14 </summary>
        public  byte type;
            /// <summary> Memory contents at specified address </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] value;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=30)]
    public struct mavlink_debug_vect_t
    {
        /// <summary> Timestamp </summary>
        public  UInt64 time_usec;
            /// <summary> x </summary>
        public  Single x;
            /// <summary> y </summary>
        public  Single y;
            /// <summary> z </summary>
        public  Single z;
            /// <summary> Name </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    public struct mavlink_named_value_float_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Floating point value </summary>
        public  Single value;
            /// <summary> Name of the debug variable </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    public struct mavlink_named_value_int_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Signed integer value </summary>
        public  Int32 value;
            /// <summary> Name of the debug variable </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    public struct mavlink_statustext_t
    {
        /// <summary> Severity of status. Relies on the definitions within RFC-5424. See enum MAV_SEVERITY. </summary>
        public  byte severity;
            /// <summary> Status text message, without null termination character </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=50)]
		public byte[] text;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    public struct mavlink_debug_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> DEBUG value </summary>
        public  Single value;
            /// <summary> index of debug variable </summary>
        public  byte ind;
    
    };

}
