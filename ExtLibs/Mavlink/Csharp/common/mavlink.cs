using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public partial class MAVLink
{
        public const string MAVLINK_BUILD_DATE = "Thu Jun 18 07:16:50 2015";
        public const string MAVLINK_WIRE_PROTOCOL_VERSION = "1.0";
        public const int MAVLINK_MAX_DIALECT_PAYLOAD_SIZE = 255;

        public const int MAVLINK_LITTLE_ENDIAN = 1;
        public const int MAVLINK_BIG_ENDIAN = 0;

        public const byte MAVLINK_STX = 254;

        public const byte MAVLINK_ENDIAN = MAVLINK_LITTLE_ENDIAN;

        public const bool MAVLINK_ALIGNED_FIELDS = (1 == 1);

        public const byte MAVLINK_CRC_EXTRA = 1;
        
        public const bool MAVLINK_NEED_BYTE_SWAP = (MAVLINK_ENDIAN == MAVLINK_LITTLE_ENDIAN);
        
        public static readonly byte[] MAVLINK_MESSAGE_LENGTHS = new byte[] {9, 31, 12, 0, 14, 28, 3, 32, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 20, 2, 25, 23, 30, 101, 22, 26, 16, 14, 28, 32, 28, 28, 22, 22, 21, 6, 6, 37, 4, 4, 2, 2, 4, 2, 2, 3, 13, 12, 37, 0, 0, 0, 27, 25, 0, 0, 0, 0, 0, 68, 26, 185, 181, 42, 6, 4, 0, 11, 18, 0, 0, 37, 20, 35, 33, 3, 0, 0, 0, 22, 39, 37, 53, 51, 53, 51, 0, 28, 56, 42, 33, 0, 0, 0, 0, 0, 0, 0, 26, 32, 32, 20, 32, 62, 44, 64, 84, 9, 254, 16, 0, 36, 44, 64, 22, 6, 14, 12, 97, 2, 2, 113, 35, 6, 79, 35, 35, 22, 13, 255, 14, 18, 43, 8, 22, 14, 36, 43, 41, 0, 0, 0, 0, 0, 0, 36, 60, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 0, 0, 0, 254, 36, 30, 18, 18, 51, 9, 0};

        public static readonly byte[] MAVLINK_MESSAGE_CRCS = new byte[] {50, 124, 137, 0, 237, 217, 104, 119, 0, 0, 0, 89, 0, 0, 0, 0, 0, 0, 0, 0, 214, 159, 220, 168, 24, 23, 170, 144, 67, 115, 39, 246, 185, 104, 237, 244, 222, 212, 9, 254, 230, 28, 28, 132, 221, 232, 11, 153, 41, 39, 78, 0, 0, 0, 15, 3, 0, 0, 0, 0, 0, 153, 183, 51, 82, 118, 148, 21, 0, 243, 124, 0, 0, 38, 20, 158, 152, 143, 0, 0, 0, 106, 49, 22, 143, 140, 5, 150, 0, 231, 183, 63, 54, 0, 0, 0, 0, 0, 0, 0, 175, 102, 158, 208, 56, 93, 138, 108, 32, 185, 84, 34, 0, 124, 237, 4, 76, 128, 56, 116, 134, 237, 203, 250, 87, 203, 220, 25, 226, 46, 29, 223, 85, 6, 229, 203, 1, 195, 109, 168, 181, 0, 0, 0, 0, 0, 0, 154, 178, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 90, 0, 0, 0, 0, 0, 0, 8, 204, 49, 170, 44, 83, 46, 0};

        public static readonly Type[] MAVLINK_MESSAGE_INFO = new Type[] {typeof( mavlink_heartbeat_t ), typeof( mavlink_sys_status_t ), typeof( mavlink_system_time_t ), null, typeof( mavlink_ping_t ), typeof( mavlink_change_operator_control_t ), typeof( mavlink_change_operator_control_ack_t ), typeof( mavlink_auth_key_t ), null, null, null, typeof( mavlink_set_mode_t ), null, null, null, null, null, null, null, null, typeof( mavlink_param_request_read_t ), typeof( mavlink_param_request_list_t ), typeof( mavlink_param_value_t ), typeof( mavlink_param_set_t ), typeof( mavlink_gps_raw_int_t ), typeof( mavlink_gps_status_t ), typeof( mavlink_scaled_imu_t ), typeof( mavlink_raw_imu_t ), typeof( mavlink_raw_pressure_t ), typeof( mavlink_scaled_pressure_t ), typeof( mavlink_attitude_t ), typeof( mavlink_attitude_quaternion_t ), typeof( mavlink_local_position_ned_t ), typeof( mavlink_global_position_int_t ), typeof( mavlink_rc_channels_scaled_t ), typeof( mavlink_rc_channels_raw_t ), typeof( mavlink_servo_output_raw_t ), typeof( mavlink_mission_request_partial_list_t ), typeof( mavlink_mission_write_partial_list_t ), typeof( mavlink_mission_item_t ), typeof( mavlink_mission_request_t ), typeof( mavlink_mission_set_current_t ), typeof( mavlink_mission_current_t ), typeof( mavlink_mission_request_list_t ), typeof( mavlink_mission_count_t ), typeof( mavlink_mission_clear_all_t ), typeof( mavlink_mission_item_reached_t ), typeof( mavlink_mission_ack_t ), typeof( mavlink_set_gps_global_origin_t ), typeof( mavlink_gps_global_origin_t ), typeof( mavlink_param_map_rc_t ), null, null, null, typeof( mavlink_safety_set_allowed_area_t ), typeof( mavlink_safety_allowed_area_t ), null, null, null, null, null, typeof( mavlink_attitude_quaternion_cov_t ), typeof( mavlink_nav_controller_output_t ), typeof( mavlink_global_position_int_cov_t ), typeof( mavlink_local_position_ned_cov_t ), typeof( mavlink_rc_channels_t ), typeof( mavlink_request_data_stream_t ), typeof( mavlink_data_stream_t ), null, typeof( mavlink_manual_control_t ), typeof( mavlink_rc_channels_override_t ), null, null, typeof( mavlink_mission_item_int_t ), typeof( mavlink_vfr_hud_t ), typeof( mavlink_command_int_t ), typeof( mavlink_command_long_t ), typeof( mavlink_command_ack_t ), null, null, null, typeof( mavlink_manual_setpoint_t ), typeof( mavlink_set_attitude_target_t ), typeof( mavlink_attitude_target_t ), typeof( mavlink_set_position_target_local_ned_t ), typeof( mavlink_position_target_local_ned_t ), typeof( mavlink_set_position_target_global_int_t ), typeof( mavlink_position_target_global_int_t ), null, typeof( mavlink_local_position_ned_system_global_offset_t ), typeof( mavlink_hil_state_t ), typeof( mavlink_hil_controls_t ), typeof( mavlink_hil_rc_inputs_raw_t ), null, null, null, null, null, null, null, typeof( mavlink_optical_flow_t ), typeof( mavlink_global_vision_position_estimate_t ), typeof( mavlink_vision_position_estimate_t ), typeof( mavlink_vision_speed_estimate_t ), typeof( mavlink_vicon_position_estimate_t ), typeof( mavlink_highres_imu_t ), typeof( mavlink_optical_flow_rad_t ), typeof( mavlink_hil_sensor_t ), typeof( mavlink_sim_state_t ), typeof( mavlink_radio_status_t ), typeof( mavlink_file_transfer_protocol_t ), typeof( mavlink_timesync_t ), null, typeof( mavlink_hil_gps_t ), typeof( mavlink_hil_optical_flow_t ), typeof( mavlink_hil_state_quaternion_t ), typeof( mavlink_scaled_imu2_t ), typeof( mavlink_log_request_list_t ), typeof( mavlink_log_entry_t ), typeof( mavlink_log_request_data_t ), typeof( mavlink_log_data_t ), typeof( mavlink_log_erase_t ), typeof( mavlink_log_request_end_t ), typeof( mavlink_gps_inject_data_t ), typeof( mavlink_gps2_raw_t ), typeof( mavlink_power_status_t ), typeof( mavlink_serial_control_t ), typeof( mavlink_gps_rtk_t ), typeof( mavlink_gps2_rtk_t ), typeof( mavlink_scaled_imu3_t ), typeof( mavlink_data_transmission_handshake_t ), typeof( mavlink_encapsulated_data_t ), typeof( mavlink_distance_sensor_t ), typeof( mavlink_terrain_request_t ), typeof( mavlink_terrain_data_t ), typeof( mavlink_terrain_check_t ), typeof( mavlink_terrain_report_t ), typeof( mavlink_scaled_pressure2_t ), typeof( mavlink_att_pos_mocap_t ), typeof( mavlink_set_actuator_control_target_t ), typeof( mavlink_actuator_control_target_t ), null, null, null, null, null, null, typeof( mavlink_battery_status_t ), typeof( mavlink_autopilot_version_t ), typeof( mavlink_landing_target_t ), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, typeof( mavlink_vibration_t ), null, null, null, null, null, null, typeof( mavlink_v2_extension_t ), typeof( mavlink_memory_vect_t ), typeof( mavlink_debug_vect_t ), typeof( mavlink_named_value_float_t ), typeof( mavlink_named_value_int_t ), typeof( mavlink_statustext_t ), typeof( mavlink_debug_t ), null};

		public static readonly string[] MAVLINK_NAMES = new string[] {"HEARTBEAT", "SYS_STATUS", "SYSTEM_TIME", null, "PING", "CHANGE_OPERATOR_CONTROL", "CHANGE_OPERATOR_CONTROL_ACK", "AUTH_KEY", null, null, null, "SET_MODE", null, null, null, null, null, null, null, null, "PARAM_REQUEST_READ", "PARAM_REQUEST_LIST", "PARAM_VALUE", "PARAM_SET", "GPS_RAW_INT", "GPS_STATUS", "SCALED_IMU", "RAW_IMU", "RAW_PRESSURE", "SCALED_PRESSURE", "ATTITUDE", "ATTITUDE_QUATERNION", "LOCAL_POSITION_NED", "GLOBAL_POSITION_INT", "RC_CHANNELS_SCALED", "RC_CHANNELS_RAW", "SERVO_OUTPUT_RAW", "MISSION_REQUEST_PARTIAL_LIST", "MISSION_WRITE_PARTIAL_LIST", "MISSION_ITEM", "MISSION_REQUEST", "MISSION_SET_CURRENT", "MISSION_CURRENT", "MISSION_REQUEST_LIST", "MISSION_COUNT", "MISSION_CLEAR_ALL", "MISSION_ITEM_REACHED", "MISSION_ACK", "SET_GPS_GLOBAL_ORIGIN", "GPS_GLOBAL_ORIGIN", "PARAM_MAP_RC", null, null, null, "SAFETY_SET_ALLOWED_AREA", "SAFETY_ALLOWED_AREA", null, null, null, null, null, "ATTITUDE_QUATERNION_COV", "NAV_CONTROLLER_OUTPUT", "GLOBAL_POSITION_INT_COV", "LOCAL_POSITION_NED_COV", "RC_CHANNELS", "REQUEST_DATA_STREAM", "DATA_STREAM", null, "MANUAL_CONTROL", "RC_CHANNELS_OVERRIDE", null, null, "MISSION_ITEM_INT", "VFR_HUD", "COMMAND_INT", "COMMAND_LONG", "COMMAND_ACK", null, null, null, "MANUAL_SETPOINT", "SET_ATTITUDE_TARGET", "ATTITUDE_TARGET", "SET_POSITION_TARGET_LOCAL_NED", "POSITION_TARGET_LOCAL_NED", "SET_POSITION_TARGET_GLOBAL_INT", "POSITION_TARGET_GLOBAL_INT", null, "LOCAL_POSITION_NED_SYSTEM_GLOBAL_OFFSET", "HIL_STATE", "HIL_CONTROLS", "HIL_RC_INPUTS_RAW", null, null, null, null, null, null, null, "OPTICAL_FLOW", "GLOBAL_VISION_POSITION_ESTIMATE", "VISION_POSITION_ESTIMATE", "VISION_SPEED_ESTIMATE", "VICON_POSITION_ESTIMATE", "HIGHRES_IMU", "OPTICAL_FLOW_RAD", "HIL_SENSOR", "SIM_STATE", "RADIO_STATUS", "FILE_TRANSFER_PROTOCOL", "TIMESYNC", null, "HIL_GPS", "HIL_OPTICAL_FLOW", "HIL_STATE_QUATERNION", "SCALED_IMU2", "LOG_REQUEST_LIST", "LOG_ENTRY", "LOG_REQUEST_DATA", "LOG_DATA", "LOG_ERASE", "LOG_REQUEST_END", "GPS_INJECT_DATA", "GPS2_RAW", "POWER_STATUS", "SERIAL_CONTROL", "GPS_RTK", "GPS2_RTK", "SCALED_IMU3", "DATA_TRANSMISSION_HANDSHAKE", "ENCAPSULATED_DATA", "DISTANCE_SENSOR", "TERRAIN_REQUEST", "TERRAIN_DATA", "TERRAIN_CHECK", "TERRAIN_REPORT", "SCALED_PRESSURE2", "ATT_POS_MOCAP", "SET_ACTUATOR_CONTROL_TARGET", "ACTUATOR_CONTROL_TARGET", null, null, null, null, null, null, "BATTERY_STATUS", "AUTOPILOT_VERSION", "LANDING_TARGET", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "VIBRATION", null, null, null, null, null, null, "V2_EXTENSION", "MEMORY_VECT", "DEBUG_VECT", "NAMED_VALUE_FLOAT", "NAMED_VALUE_INT", "STATUSTEXT", "DEBUG", null};

        public const byte MAVLINK_VERSION = 3;

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
PARAM_MAP_RC = 50,
SAFETY_SET_ALLOWED_AREA = 54,
SAFETY_ALLOWED_AREA = 55,
ATTITUDE_QUATERNION_COV = 61,
NAV_CONTROLLER_OUTPUT = 62,
GLOBAL_POSITION_INT_COV = 63,
LOCAL_POSITION_NED_COV = 64,
RC_CHANNELS = 65,
REQUEST_DATA_STREAM = 66,
DATA_STREAM = 67,
MANUAL_CONTROL = 69,
RC_CHANNELS_OVERRIDE = 70,
MISSION_ITEM_INT = 73,
VFR_HUD = 74,
COMMAND_INT = 75,
COMMAND_LONG = 76,
COMMAND_ACK = 77,
MANUAL_SETPOINT = 81,
SET_ATTITUDE_TARGET = 82,
ATTITUDE_TARGET = 83,
SET_POSITION_TARGET_LOCAL_NED = 84,
POSITION_TARGET_LOCAL_NED = 85,
SET_POSITION_TARGET_GLOBAL_INT = 86,
POSITION_TARGET_GLOBAL_INT = 87,
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
OPTICAL_FLOW_RAD = 106,
HIL_SENSOR = 107,
SIM_STATE = 108,
RADIO_STATUS = 109,
FILE_TRANSFER_PROTOCOL = 110,
TIMESYNC = 111,
HIL_GPS = 113,
HIL_OPTICAL_FLOW = 114,
HIL_STATE_QUATERNION = 115,
SCALED_IMU2 = 116,
LOG_REQUEST_LIST = 117,
LOG_ENTRY = 118,
LOG_REQUEST_DATA = 119,
LOG_DATA = 120,
LOG_ERASE = 121,
LOG_REQUEST_END = 122,
GPS_INJECT_DATA = 123,
GPS2_RAW = 124,
POWER_STATUS = 125,
SERIAL_CONTROL = 126,
GPS_RTK = 127,
GPS2_RTK = 128,
SCALED_IMU3 = 129,
DATA_TRANSMISSION_HANDSHAKE = 130,
ENCAPSULATED_DATA = 131,
DISTANCE_SENSOR = 132,
TERRAIN_REQUEST = 133,
TERRAIN_DATA = 134,
TERRAIN_CHECK = 135,
TERRAIN_REPORT = 136,
SCALED_PRESSURE2 = 137,
ATT_POS_MOCAP = 138,
SET_ACTUATOR_CONTROL_TARGET = 139,
ACTUATOR_CONTROL_TARGET = 140,
BATTERY_STATUS = 147,
AUTOPILOT_VERSION = 148,
LANDING_TARGET = 149,
VIBRATION = 241,
V2_EXTENSION = 248,
MEMORY_VECT = 249,
DEBUG_VECT = 250,
NAMED_VALUE_FLOAT = 251,
NAMED_VALUE_INT = 252,
STATUSTEXT = 253,
DEBUG = 254,

		}

    
        
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
        	///<summary> ASLUAV autopilot -- http://www.asl.ethz.ch | </summary>
            ASLUAV=17, 
        	///<summary>  | </summary>
            ENUM_END=18, 
        
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
        	///<summary> Onboard companion controller | </summary>
            ONBOARD_CONTROLLER=18, 
        	///<summary> Two-rotor VTOL using control surfaces in vertical operation in addition. Tailsitter. | </summary>
            VTOL_DUOROTOR=19, 
        	///<summary> Quad-rotor VTOL using a V-shaped quad config in vertical operation. Tailsitter. | </summary>
            VTOL_QUADROTOR=20, 
        	///<summary> VTOL reserved 1 | </summary>
            VTOL_RESERVED1=21, 
        	///<summary> VTOL reserved 2 | </summary>
            VTOL_RESERVED2=22, 
        	///<summary> VTOL reserved 3 | </summary>
            VTOL_RESERVED3=23, 
        	///<summary> VTOL reserved 4 | </summary>
            VTOL_RESERVED4=24, 
        	///<summary> VTOL reserved 5 | </summary>
            VTOL_RESERVED5=25, 
        	///<summary> Onboard gimbal | </summary>
            GIMBAL=26, 
        	///<summary>  | </summary>
            ENUM_END=27, 
        
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
            MAV_COMP_ID_GIMBAL=154, 
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
        	///<summary> 0x20000 2nd 3D gyro | </summary>
            _3D_GYRO2=131072, 
        	///<summary> 0x40000 2nd 3D accelerometer | </summary>
            _3D_ACCEL2=262144, 
        	///<summary> 0x80000 2nd 3D magnetometer | </summary>
            _3D_MAG2=524288, 
        	///<summary> 0x100000 geofence | </summary>
            MAV_SYS_STATUS_GEOFENCE=1048576, 
        	///<summary> 0x200000 AHRS subsystem health | </summary>
            MAV_SYS_STATUS_AHRS=2097152, 
        	///<summary> 0x400000 Terrain subsystem health | </summary>
            MAV_SYS_STATUS_TERRAIN=4194304, 
        	///<summary>  | </summary>
            ENUM_END=4194305, 
        
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
        	///<summary> Global coordinate frame, WGS84 coordinate system. First value / x: latitude in degrees*1.0e-7, second value / y: longitude in degrees*1.0e-7, third value / z: positive altitude over mean sea level (MSL) | </summary>
            GLOBAL_INT=5, 
        	///<summary> Global coordinate frame, WGS84 coordinate system, relative altitude over ground with respect to the home position. First value / x: latitude in degrees*10e-7, second value / y: longitude in degrees*10e-7, third value / z: positive altitude with 0 being at the altitude of the home location. | </summary>
            GLOBAL_RELATIVE_ALT_INT=6, 
        	///<summary> Offset to the current local frame. Anything expressed in this frame should be added to the current local frame position. | </summary>
            LOCAL_OFFSET_NED=7, 
        	///<summary> Setpoint in body NED frame. This makes sense if all position control is externalized - e.g. useful to command 2 m/s^2 acceleration to the right. | </summary>
            BODY_NED=8, 
        	///<summary> Offset in body NED frame. This makes sense if adding setpoints to the current flight path, to avoid an obstacle - e.g. useful to command 2 m/s^2 acceleration to the east. | </summary>
            BODY_OFFSET_NED=9, 
        	///<summary> Global coordinate frame with above terrain level altitude. WGS84 coordinate system, relative altitude over terrain with respect to the waypoint coordinate. First value / x: latitude in degrees, second value / y: longitude in degrees, third value / z: positive altitude in meters with 0 being at ground level in terrain model. | </summary>
            GLOBAL_TERRAIN_ALT=10, 
        	///<summary> Global coordinate frame with above terrain level altitude. WGS84 coordinate system, relative altitude over terrain with respect to the waypoint coordinate. First value / x: latitude in degrees*10e-7, second value / y: longitude in degrees*10e-7, third value / z: positive altitude in meters with 0 being at ground level in terrain model. | </summary>
            GLOBAL_TERRAIN_ALT_INT=11, 
        	///<summary>  | </summary>
            ENUM_END=12, 
        
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
        
        ///<summary> Enumeration of possible mount operation modes </summary>
        public enum MAV_MOUNT_MODE
        {
    	///<summary> Load and keep safe position (Roll,Pitch,Yaw) from permant memory and stop stabilization | </summary>
            RETRACT=0, 
        	///<summary> Load and keep neutral position (Roll,Pitch,Yaw) from permanent memory. | </summary>
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
        
        ///<summary> Commands to be executed by the MAV. They can be executed on user request, or as part of a mission script. If the action is used in a mission, the parameter mapping to the waypoint/mission message is as follows: Param 1, Param 2, Param 3, Param 4, X: Param 5, Y:Param 6, Z:Param 7. This command list is similar what ARINC 424 is for commercial aircraft: A data format how to interpret waypoint/mission data. </summary>
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
        	///<summary> Continue on the current course and climb/descend to specified altitude.  When the altitude is reached continue to the next command (i.e., don't proceed to the next command until the desired altitude is reached. |Empty| Empty| Empty| Empty| Empty| Empty| Desired altitude in meters|  </summary>
            CONTINUE_AND_CHANGE_ALT=30, 
        	///<summary> Begin loiter at the specified Latitude and Longitude.  If Lat=Lon=0, then loiter at the current position.  Don't consider the navigation command complete (don't leave loiter) until the altitude has been reached.  Additionally, if the Heading Required parameter is non-zero the  aircraft will not leave the loiter until heading toward the next waypoint.  |Heading Required (0 = False)| Radius in meters. If positive loiter clockwise, negative counter-clockwise, 0 means no change to standard loiter.| Empty| Empty| Latitude| Longitude| Altitude|  </summary>
            LOITER_TO_ALT=31, 
        	///<summary> Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Region of intereset mode. (see MAV_ROI enum)| MISSION index/ target ID. (see MAV_ROI enum)| ROI index (allows a vehicle to manage multiple ROI's)| Empty| x the location of the fixed ROI (see MAV_FRAME)| y| z|  </summary>
            ROI=80, 
        	///<summary> Control autonomous path planning on the MAV. |0: Disable local obstacle avoidance / local path planning (without resetting map), 1: Enable local path planning, 2: Enable and reset local path planning| 0: Disable full path planning (without resetting map), 1: Enable, 2: Enable and reset map/occupancy grid, 3: Enable and reset planned route, but not occupancy grid| Empty| Yaw angle at goal, in compass degrees, [0..360]| Latitude/X of goal| Longitude/Y of goal| Altitude/Z of goal|  </summary>
            PATHPLANNING=81, 
        	///<summary> Navigate to MISSION using a spline path. |Hold time in decimal seconds. (ignored by fixed wing, time to stay at MISSION for rotary wing)| Empty| Empty| Empty| Latitude/X of goal| Longitude/Y of goal| Altitude/Z of goal|  </summary>
            SPLINE_WAYPOINT=82, 
        	///<summary> hand control over to an external controller |On / Off (> 0.5f on)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            GUIDED_ENABLE=92, 
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
        	///<summary> Set system mode. |Mode, as defined by ENUM MAV_MODE| Custom mode - this is system specific, please refer to the individual autopilot specifications for details.| Empty| Empty| Empty| Empty| Empty|  </summary>
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
        	///<summary> Terminate flight immediately |Flight termination activated if > 0.5| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_FLIGHTTERMINATION=185, 
        	///<summary> Mission command to perform a landing. This is used as a marker in a mission to tell the autopilot where a sequence of mission items that represents a landing starts. It may also be sent via a COMMAND_LONG to trigger a landing, in which case the nearest (geographically) landing sequence in the mission will be used. The Latitude/Longitude is optional, and may be set to 0/0 if not needed. If specified then it will be used to help find the closest landing sequence. |Empty| Empty| Empty| Empty| Latitude| Longitude| Empty|  </summary>
            DO_LAND_START=189, 
        	///<summary> Mission command to perform a landing from a rally point. |Break altitude (meters)| Landing speed (m/s)| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_RALLY_LAND=190, 
        	///<summary> Mission command to safely abort an autonmous landing. |Altitude (meters)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_GO_AROUND=191, 
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
        	///<summary> Mission command to control a camera or antenna mount |pitch or lat in degrees, depending on mount mode.| roll or lon in degrees depending on mount mode| yaw or alt (in meters) depending on mount mode| reserved| reserved| reserved| MAV_MOUNT_MODE enum value|  </summary>
            DO_MOUNT_CONTROL=205, 
        	///<summary> Mission command to set CAM_TRIGG_DIST for this flight |Camera trigger distance (meters)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_SET_CAM_TRIGG_DIST=206, 
        	///<summary> Mission command to enable the geofence |enable? (0=disable, 1=enable)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_FENCE_ENABLE=207, 
        	///<summary> Mission command to trigger a parachute |action (0=disable, 1=enable, 2=release, for some systems see PARACHUTE_ACTION enum, not in general message set.)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_PARACHUTE=208, 
        	///<summary> Change to/from inverted flight |inverted (0=normal, 1=inverted)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_INVERTED_FLIGHT=210, 
        	///<summary> Mission command to control a camera or antenna mount, using a quaternion as reference. |q1 - quaternion param #1, w (1 in null-rotation)| q2 - quaternion param #2, x (0 in null-rotation)| q3 - quaternion param #3, y (0 in null-rotation)| q4 - quaternion param #4, z (0 in null-rotation)| Empty| Empty| Empty|  </summary>
            DO_MOUNT_CONTROL_QUAT=220, 
        	///<summary> set id of master controller |System ID| Component ID| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_GUIDED_MASTER=221, 
        	///<summary> set limits for external control |timeout - maximum time (in seconds) that external controller will be allowed to control vehicle. 0 means no timeout| absolute altitude min (in meters, AMSL) - if vehicle moves below this alt, the command will be aborted and the mission will continue.  0 means no lower altitude limit| absolute altitude max (in meters)- if vehicle moves above this alt, the command will be aborted and the mission will continue.  0 means no upper altitude limit| horizontal move limit (in meters, AMSL) - if vehicle moves more than this distance from it's location at the moment the command was executed, the command will be aborted and the mission will continue. 0 means no horizontal altitude limit| Empty| Empty| Empty|  </summary>
            DO_GUIDED_LIMITS=222, 
        	///<summary> NOP - This command is only used to mark the upper limit of the DO commands in the enumeration |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
            DO_LAST=240, 
        	///<summary> Trigger calibration. This command will be only accepted if in pre-flight mode. |Gyro calibration: 0: no, 1: yes| Magnetometer calibration: 0: no, 1: yes| Ground pressure: 0: no, 1: yes| Radio calibration: 0: no, 1: yes| Accelerometer calibration: 0: no, 1: yes| Compass/Motor interference calibration: 0: no, 1: yes| Empty|  </summary>
            PREFLIGHT_CALIBRATION=241, 
        	///<summary> Set sensor offsets. This command will be only accepted if in pre-flight mode. |Sensor to adjust the offsets for: 0: gyros, 1: accelerometer, 2: magnetometer, 3: barometer, 4: optical flow, 5: second magnetometer| X axis offset (or generic dimension 1), in the sensor's raw units| Y axis offset (or generic dimension 2), in the sensor's raw units| Z axis offset (or generic dimension 3), in the sensor's raw units| Generic dimension 4, in the sensor's raw units| Generic dimension 5, in the sensor's raw units| Generic dimension 6, in the sensor's raw units|  </summary>
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
        	///<summary> Request autopilot capabilities |1: Request autopilot version| Reserved (all remaining params)|  </summary>
            REQUEST_AUTOPILOT_CAPABILITIES=520, 
        	///<summary> Start image capture sequence |Duration between two consecutive pictures (in seconds)| Number of images to capture total - 0 for unlimited capture| Resolution in megapixels (0.3 for 640x480, 1.3 for 1280x720, etc)|  </summary>
            IMAGE_START_CAPTURE=2000, 
        	///<summary> Stop image capture sequence |Reserved| Reserved|  </summary>
            IMAGE_STOP_CAPTURE=2001, 
        	///<summary> Starts video capture |Camera ID (0 for all cameras), 1 for first, 2 for second, etc.| Frames per second| Resolution in megapixels (0.3 for 640x480, 1.3 for 1280x720, etc)|  </summary>
            VIDEO_START_CAPTURE=2500, 
        	///<summary> Stop the current video capture |Reserved| Reserved|  </summary>
            VIDEO_STOP_CAPTURE=2501, 
        	///<summary> Create a panorama at the current position |Viewing angle horizontal of the panorama (in degrees, +- 0.5 the total angle)| Viewing angle vertical of panorama (in degrees)| Speed of the horizontal rotation (in degrees per second)| Speed of the vertical rotation (in degrees per second)|  </summary>
            PANORAMA_CREATE=2800, 
        	///<summary> Deploy payload on a Lat / Lon / Alt position. This includes the navigation to reach the required release position and velocity. |Operation mode. 0: prepare single payload deploy (overwriting previous requests), but do not execute it. 1: execute payload deploy immediately (rejecting further deploy commands during execution, but allowing abort). 2: add payload deploy to existing deployment list.| Desired approach vector in degrees compass heading (0..360). A negative value indicates the system can define the approach vector at will.| Desired ground speed at release time. This can be overriden by the airframe in case it needs to meet minimum airspeed. A negative value indicates the system can define the ground speed at will.| Minimum altitude clearance to the release position in meters. A negative value indicates the system can define the clearance at will.| Latitude unscaled for MISSION_ITEM or in 1e7 degrees for MISSION_ITEM_INT| Longitude unscaled for MISSION_ITEM or in 1e7 degrees for MISSION_ITEM_INT| Altitude, in meters AMSL|  </summary>
            PAYLOAD_PREPARE_DEPLOY=30001, 
        	///<summary> Control the payload deployment. |Operation mode. 0: Abort deployment, continue normal mission. 1: switch to payload deploment mode. 100: delete first payload deployment request. 101: delete all payload deployment requests.| Reserved| Reserved| Reserved| Reserved| Reserved| Reserved|  </summary>
            PAYLOAD_CONTROL_DEPLOY=30002, 
        	///<summary>  | </summary>
            ENUM_END=30003, 
        
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
        
        ///<summary> Power supply status flags (bitmask) </summary>
        public enum MAV_POWER_STATUS
        {
    	///<summary> main brick power supply valid | </summary>
            BRICK_VALID=1, 
        	///<summary> main servo power supply valid for FMU | </summary>
            SERVO_VALID=2, 
        	///<summary> USB power is connected | </summary>
            USB_CONNECTED=4, 
        	///<summary> peripheral supply is in over-current state | </summary>
            PERIPH_OVERCURRENT=8, 
        	///<summary> hi-power peripheral supply is in over-current state | </summary>
            PERIPH_HIPOWER_OVERCURRENT=16, 
        	///<summary> Power status has changed since boot | </summary>
            CHANGED=32, 
        	///<summary>  | </summary>
            ENUM_END=33, 
        
        };
        
        ///<summary> SERIAL_CONTROL device types </summary>
        public enum SERIAL_CONTROL_DEV
        {
    	///<summary> First telemetry port | </summary>
            TELEM1=0, 
        	///<summary> Second telemetry port | </summary>
            TELEM2=1, 
        	///<summary> First GPS port | </summary>
            GPS1=2, 
        	///<summary> Second GPS port | </summary>
            GPS2=3, 
        	///<summary> system shell | </summary>
            SHELL=10, 
        	///<summary>  | </summary>
            ENUM_END=11, 
        
        };
        
        ///<summary> SERIAL_CONTROL flags (bitmask) </summary>
        public enum SERIAL_CONTROL_FLAG
        {
    	///<summary> Set if this is a reply | </summary>
            REPLY=1, 
        	///<summary> Set if the sender wants the receiver to send a response as another SERIAL_CONTROL message | </summary>
            RESPOND=2, 
        	///<summary> Set if access to the serial port should be removed from whatever driver is currently using it, giving exclusive access to the SERIAL_CONTROL protocol. The port can be handed back by sending a request without this flag set | </summary>
            EXCLUSIVE=4, 
        	///<summary> Block on writes to the serial port | </summary>
            BLOCKING=8, 
        	///<summary> Send multiple replies until port is drained | </summary>
            MULTI=16, 
        	///<summary>  | </summary>
            ENUM_END=17, 
        
        };
        
        ///<summary> Enumeration of distance sensor types </summary>
        public enum MAV_DISTANCE_SENSOR
        {
    	///<summary> Laser altimeter, e.g. LightWare SF02/F or PulsedLight units | </summary>
            LASER=0, 
        	///<summary> Ultrasound altimeter, e.g. MaxBotix units | </summary>
            ULTRASOUND=1, 
        	///<summary>  | </summary>
            ENUM_END=2, 
        
        };
        
        ///<summary> Bitmask of (optional) autopilot capabilities (64 bit). If a bit is set, the autopilot supports this capability. </summary>
        public enum MAV_PROTOCOL_CAPABILITY
        {
    	///<summary> Autopilot supports MISSION float message type. | </summary>
            MISSION_FLOAT=1, 
        	///<summary> Autopilot supports the new param float message type. | </summary>
            PARAM_FLOAT=2, 
        	///<summary> Autopilot supports MISSION_INT scaled integer message type. | </summary>
            MISSION_INT=4, 
        	///<summary> Autopilot supports COMMAND_INT scaled integer message type. | </summary>
            COMMAND_INT=8, 
        	///<summary> Autopilot supports the new param union message type. | </summary>
            PARAM_UNION=16, 
        	///<summary> Autopilot supports the new param union message type. | </summary>
            FTP=32, 
        	///<summary> Autopilot supports commanding attitude offboard. | </summary>
            SET_ATTITUDE_TARGET=64, 
        	///<summary> Autopilot supports commanding position and velocity targets in local NED frame. | </summary>
            SET_POSITION_TARGET_LOCAL_NED=128, 
        	///<summary> Autopilot supports commanding position and velocity targets in global scaled integers. | </summary>
            SET_POSITION_TARGET_GLOBAL_INT=256, 
        	///<summary> Autopilot supports terrain protocol / data handling. | </summary>
            TERRAIN=512, 
        	///<summary> Autopilot supports direct actuator control. | </summary>
            SET_ACTUATOR_TARGET=1024, 
        	///<summary>  | </summary>
            ENUM_END=1025, 
        
        };
        
        ///<summary> Enumeration of estimator types </summary>
        public enum MAV_ESTIMATOR_TYPE
        {
    	///<summary> This is a naive estimator without any real covariance feedback. | </summary>
            NAIVE=1, 
        	///<summary> Computer vision based estimate. Might be up to scale. | </summary>
            VISION=2, 
        	///<summary> Visual-inertial estimate. | </summary>
            VIO=3, 
        	///<summary> Plain GPS estimate. | </summary>
            GPS=4, 
        	///<summary> Estimator integrating GPS and inertial sensing. | </summary>
            GPS_INS=5, 
        	///<summary>  | </summary>
            ENUM_END=6, 
        
        };
        
        ///<summary> Enumeration of battery types </summary>
        public enum MAV_BATTERY_TYPE
        {
    	///<summary> Not specified. | </summary>
            UNKNOWN=0, 
        	///<summary> Lithium polymere battery | </summary>
            LIPO=1, 
        	///<summary> Lithium ferrite battery | </summary>
            LIFE=2, 
        	///<summary> Lithium-ION battery | </summary>
            LION=3, 
        	///<summary> Nickel metal hydride battery | </summary>
            NIMH=4, 
        	///<summary>  | </summary>
            ENUM_END=5, 
        
        };
        
        ///<summary> Enumeration of battery functions </summary>
        public enum MAV_BATTERY_FUNCTION
        {
    	///<summary> Lithium polymere battery | </summary>
            UNKNOWN=0, 
        	///<summary> Battery supports all flight systems | </summary>
            ALL=1, 
        	///<summary> Battery for the propulsion system | </summary>
            PROPULSION=2, 
        	///<summary> Avionics battery | </summary>
            AVIONICS=3, 
        	///<summary> Payload battery | </summary>
            MAV_BATTERY_TYPE_PAYLOAD=4, 
        	///<summary>  | </summary>
            ENUM_END=5, 
        
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
            /// <summary> System mode bitfield, see MAV_MODE_FLAG ENUM in mavlink/include/mavlink_types.h </summary>
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
        /// <summary> Unix timestamp in microseconds or since system boot if smaller than MAVLink epoch (1.1.2009) </summary>
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
            /// <summary> Altitude (AMSL, NOT WGS84), in meters * 1000 (positive for up). Note that virtually all GPS modules provide the AMSL altitude in addition to the WGS84 altitude. </summary>
        public  Int32 alt;
            /// <summary> GPS HDOP horizontal dilution of position in cm (m*100). If unknown, set to: UINT16_MAX </summary>
        public  UInt16 eph;
            /// <summary> GPS VDOP vertical dilution of position in cm (m*100). If unknown, set to: UINT16_MAX </summary>
        public  UInt16 epv;
            /// <summary> GPS ground speed (m/s * 100). If unknown, set to: UINT16_MAX </summary>
        public  UInt16 vel;
            /// <summary> Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX </summary>
        public  UInt16 cog;
            /// <summary> 0-1: no fix, 2: 2D fix, 3: 3D fix, 4: DGPS, 5: RTK. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix. </summary>
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
            /// <summary> Quaternion component 1, w (1 in null-rotation) </summary>
        public  Single q1;
            /// <summary> Quaternion component 2, x (0 in null-rotation) </summary>
        public  Single q2;
            /// <summary> Quaternion component 3, y (0 in null-rotation) </summary>
        public  Single q3;
            /// <summary> Quaternion component 4, z (0 in null-rotation) </summary>
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
            /// <summary> Altitude in meters, expressed as * 1000 (millimeters), AMSL (not WGS84 - note that virtually all GPS modules provide the AMSL as well) </summary>
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
        /// <summary> PARAM1, see MAV_CMD enum </summary>
        public  Single param1;
            /// <summary> PARAM2, see MAV_CMD enum </summary>
        public  Single param2;
            /// <summary> PARAM3, see MAV_CMD enum </summary>
        public  Single param3;
            /// <summary> PARAM4, see MAV_CMD enum </summary>
        public  Single param4;
            /// <summary> PARAM5 / local: x position, global: latitude </summary>
        public  Single x;
            /// <summary> PARAM6 / y position: global: longitude </summary>
        public  Single y;
            /// <summary> PARAM7 / z position: global: altitude (relative or absolute, depending on frame. </summary>
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
            /// <summary> Altitude (AMSL), in meters * 1000 (positive for up) </summary>
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
            /// <summary> Altitude (AMSL), in meters * 1000 (positive for up) </summary>
        public  Int32 altitude;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    public struct mavlink_param_map_rc_t
    {
        /// <summary> Initial parameter value </summary>
        public  Single param_value0;
            /// <summary> Scale, maps the RC range [-1, 1] to a parameter value </summary>
        public  Single scale;
            /// <summary> Minimum param value. The protocol does not define if this overwrites an onboard minimum value. (Depends on implementation) </summary>
        public  Single param_value_min;
            /// <summary> Maximum param value. The protocol does not define if this overwrites an onboard maximum value. (Depends on implementation) </summary>
        public  Single param_value_max;
            /// <summary> Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored), send -2 to disable any existing map for this rc_channel_index. </summary>
        public  Int16 param_index;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
            /// <summary> Index of parameter RC channel. Not equal to the RC channel id. Typically correpsonds to a potentiometer-knob on the RC. </summary>
        public  byte parameter_rc_channel_index;
    
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=68)]
    public struct mavlink_attitude_quaternion_cov_t
    {
        /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float q;
            /// <summary> Roll angular speed (rad/s) </summary>
        public  Single rollspeed;
            /// <summary> Pitch angular speed (rad/s) </summary>
        public  Single pitchspeed;
            /// <summary> Yaw angular speed (rad/s) </summary>
        public  Single yawspeed;
            /// <summary> Attitude covariance </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)]
		public float covariance;
    
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=185)]
    public struct mavlink_global_position_int_cov_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch) in UTC. 0 for unknown. Commonly filled by the precision time source of a GPS receiver. </summary>
        public  UInt64 time_utc;
            /// <summary> Timestamp (milliseconds since system boot) </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Latitude, expressed as degrees * 1E7 </summary>
        public  Int32 lat;
            /// <summary> Longitude, expressed as degrees * 1E7 </summary>
        public  Int32 lon;
            /// <summary> Altitude in meters, expressed as * 1000 (millimeters), above MSL </summary>
        public  Int32 alt;
            /// <summary> Altitude above ground in meters, expressed as * 1000 (millimeters) </summary>
        public  Int32 relative_alt;
            /// <summary> Ground X Speed (Latitude), expressed as m/s </summary>
        public  Single vx;
            /// <summary> Ground Y Speed (Longitude), expressed as m/s </summary>
        public  Single vy;
            /// <summary> Ground Z Speed (Altitude), expressed as m/s </summary>
        public  Single vz;
            /// <summary> Covariance matrix (first six entries are the first ROW, next six entries are the second row, etc.) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)]
		public float covariance;
            /// <summary> Class id of the estimator this estimate originated from. </summary>
        public  byte estimator_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=181)]
    public struct mavlink_local_position_ned_cov_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch) in UTC. 0 for unknown. Commonly filled by the precision time source of a GPS receiver. </summary>
        public  UInt64 time_utc;
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
            /// <summary> Covariance matrix (first six entries are the first ROW, next six entries are the second row, etc.) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)]
		public float covariance;
            /// <summary> Class id of the estimator this estimate originated from. </summary>
        public  byte estimator_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    public struct mavlink_rc_channels_t
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
            /// <summary> RC channel 9 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan9_raw;
            /// <summary> RC channel 10 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan10_raw;
            /// <summary> RC channel 11 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan11_raw;
            /// <summary> RC channel 12 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan12_raw;
            /// <summary> RC channel 13 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan13_raw;
            /// <summary> RC channel 14 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan14_raw;
            /// <summary> RC channel 15 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan15_raw;
            /// <summary> RC channel 16 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan16_raw;
            /// <summary> RC channel 17 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan17_raw;
            /// <summary> RC channel 18 value, in microseconds. A value of UINT16_MAX implies the channel is unused. </summary>
        public  UInt16 chan18_raw;
            /// <summary> Total number of RC channels being received. This can be larger than 18, indicating that more channels are available but not given in this message. This value should be 0 when no RC channels are available. </summary>
        public  byte chancount;
            /// <summary> Receive signal strength indicator, 0: 0%, 100: 100%, 255: invalid/unknown. </summary>
        public  byte rssi;
    
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    public struct mavlink_mission_item_int_t
    {
        /// <summary> PARAM1, see MAV_CMD enum </summary>
        public  Single param1;
            /// <summary> PARAM2, see MAV_CMD enum </summary>
        public  Single param2;
            /// <summary> PARAM3, see MAV_CMD enum </summary>
        public  Single param3;
            /// <summary> PARAM4, see MAV_CMD enum </summary>
        public  Single param4;
            /// <summary> PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7 </summary>
        public  Int32 x;
            /// <summary> PARAM6 / y position: local: x position in meters * 1e4, global: longitude in degrees *10^7 </summary>
        public  Int32 y;
            /// <summary> PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame. </summary>
        public  Single z;
            /// <summary> Waypoint ID (sequence number). Starts at zero. Increases monotonically for each waypoint, no gaps in the sequence (0,1,2,3,4). </summary>
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    public struct mavlink_command_int_t
    {
        /// <summary> PARAM1, see MAV_CMD enum </summary>
        public  Single param1;
            /// <summary> PARAM2, see MAV_CMD enum </summary>
        public  Single param2;
            /// <summary> PARAM3, see MAV_CMD enum </summary>
        public  Single param3;
            /// <summary> PARAM4, see MAV_CMD enum </summary>
        public  Single param4;
            /// <summary> PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7 </summary>
        public  Int32 x;
            /// <summary> PARAM6 / local: y position in meters * 1e4, global: longitude in degrees * 10^7 </summary>
        public  Int32 y;
            /// <summary> PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame. </summary>
        public  Single z;
            /// <summary> The scheduled action for the mission item. see MAV_CMD in common.xml MAVLink specs </summary>
        public  UInt16 command;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> The coordinate system of the COMMAND. see MAV_FRAME in mavlink_types.h </summary>
        public  byte frame;
            /// <summary> false:0, true:1 </summary>
        public  byte current;
            /// <summary> autocontinue to next wp </summary>
        public  byte autocontinue;
    
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=39)]
    public struct mavlink_set_attitude_target_t
    {
        /// <summary> Timestamp in milliseconds since system boot </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float q;
            /// <summary> Body roll rate in radians per second </summary>
        public  Single body_roll_rate;
            /// <summary> Body roll rate in radians per second </summary>
        public  Single body_pitch_rate;
            /// <summary> Body roll rate in radians per second </summary>
        public  Single body_yaw_rate;
            /// <summary> Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust) </summary>
        public  Single thrust;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 6: reserved, bit 7: throttle, bit 8: attitude </summary>
        public  byte type_mask;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    public struct mavlink_attitude_target_t
    {
        /// <summary> Timestamp in milliseconds since system boot </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float q;
            /// <summary> Body roll rate in radians per second </summary>
        public  Single body_roll_rate;
            /// <summary> Body roll rate in radians per second </summary>
        public  Single body_pitch_rate;
            /// <summary> Body roll rate in radians per second </summary>
        public  Single body_yaw_rate;
            /// <summary> Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust) </summary>
        public  Single thrust;
            /// <summary> Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 7: reserved, bit 8: attitude </summary>
        public  byte type_mask;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=53)]
    public struct mavlink_set_position_target_local_ned_t
    {
        /// <summary> Timestamp in milliseconds since system boot </summary>
        public  UInt32 time_boot_ms;
            /// <summary> X Position in NED frame in meters </summary>
        public  Single x;
            /// <summary> Y Position in NED frame in meters </summary>
        public  Single y;
            /// <summary> Z Position in NED frame in meters (note, altitude is negative in NED) </summary>
        public  Single z;
            /// <summary> X velocity in NED frame in meter / s </summary>
        public  Single vx;
            /// <summary> Y velocity in NED frame in meter / s </summary>
        public  Single vy;
            /// <summary> Z velocity in NED frame in meter / s </summary>
        public  Single vz;
            /// <summary> X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afx;
            /// <summary> Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afy;
            /// <summary> Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afz;
            /// <summary> yaw setpoint in rad </summary>
        public  Single yaw;
            /// <summary> yaw rate setpoint in rad/s </summary>
        public  Single yaw_rate;
            /// <summary> Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate </summary>
        public  UInt16 type_mask;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9 </summary>
        public  byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    public struct mavlink_position_target_local_ned_t
    {
        /// <summary> Timestamp in milliseconds since system boot </summary>
        public  UInt32 time_boot_ms;
            /// <summary> X Position in NED frame in meters </summary>
        public  Single x;
            /// <summary> Y Position in NED frame in meters </summary>
        public  Single y;
            /// <summary> Z Position in NED frame in meters (note, altitude is negative in NED) </summary>
        public  Single z;
            /// <summary> X velocity in NED frame in meter / s </summary>
        public  Single vx;
            /// <summary> Y velocity in NED frame in meter / s </summary>
        public  Single vy;
            /// <summary> Z velocity in NED frame in meter / s </summary>
        public  Single vz;
            /// <summary> X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afx;
            /// <summary> Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afy;
            /// <summary> Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afz;
            /// <summary> yaw setpoint in rad </summary>
        public  Single yaw;
            /// <summary> yaw rate setpoint in rad/s </summary>
        public  Single yaw_rate;
            /// <summary> Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate </summary>
        public  UInt16 type_mask;
            /// <summary> Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9 </summary>
        public  byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=53)]
    public struct mavlink_set_position_target_global_int_t
    {
        /// <summary> Timestamp in milliseconds since system boot. The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency. </summary>
        public  UInt32 time_boot_ms;
            /// <summary> X Position in WGS84 frame in 1e7 * meters </summary>
        public  Int32 lat_int;
            /// <summary> Y Position in WGS84 frame in 1e7 * meters </summary>
        public  Int32 lon_int;
            /// <summary> Altitude in meters in AMSL altitude, not WGS84 if absolute or relative, above terrain if GLOBAL_TERRAIN_ALT_INT </summary>
        public  Single alt;
            /// <summary> X velocity in NED frame in meter / s </summary>
        public  Single vx;
            /// <summary> Y velocity in NED frame in meter / s </summary>
        public  Single vy;
            /// <summary> Z velocity in NED frame in meter / s </summary>
        public  Single vz;
            /// <summary> X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afx;
            /// <summary> Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afy;
            /// <summary> Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afz;
            /// <summary> yaw setpoint in rad </summary>
        public  Single yaw;
            /// <summary> yaw rate setpoint in rad/s </summary>
        public  Single yaw_rate;
            /// <summary> Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate </summary>
        public  UInt16 type_mask;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11 </summary>
        public  byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    public struct mavlink_position_target_global_int_t
    {
        /// <summary> Timestamp in milliseconds since system boot. The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency. </summary>
        public  UInt32 time_boot_ms;
            /// <summary> X Position in WGS84 frame in 1e7 * meters </summary>
        public  Int32 lat_int;
            /// <summary> Y Position in WGS84 frame in 1e7 * meters </summary>
        public  Int32 lon_int;
            /// <summary> Altitude in meters in AMSL altitude, not WGS84 if absolute or relative, above terrain if GLOBAL_TERRAIN_ALT_INT </summary>
        public  Single alt;
            /// <summary> X velocity in NED frame in meter / s </summary>
        public  Single vx;
            /// <summary> Y velocity in NED frame in meter / s </summary>
        public  Single vy;
            /// <summary> Z velocity in NED frame in meter / s </summary>
        public  Single vz;
            /// <summary> X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afx;
            /// <summary> Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afy;
            /// <summary> Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N </summary>
        public  Single afz;
            /// <summary> yaw setpoint in rad </summary>
        public  Single yaw;
            /// <summary> yaw rate setpoint in rad/s </summary>
        public  Single yaw_rate;
            /// <summary> Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate </summary>
        public  UInt16 type_mask;
            /// <summary> Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11 </summary>
        public  byte coordinate_frame;
    
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    public struct mavlink_optical_flow_rad_t
    {
        /// <summary> Timestamp (microseconds, synced to UNIX time or since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Integration time in microseconds. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the. </summary>
        public  UInt32 integration_time_us;
            /// <summary> Flow in radians around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.) </summary>
        public  Single integrated_x;
            /// <summary> Flow in radians around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.) </summary>
        public  Single integrated_y;
            /// <summary> RH rotation around X axis (rad) </summary>
        public  Single integrated_xgyro;
            /// <summary> RH rotation around Y axis (rad) </summary>
        public  Single integrated_ygyro;
            /// <summary> RH rotation around Z axis (rad) </summary>
        public  Single integrated_zgyro;
            /// <summary> Time in microseconds since the distance was sampled. </summary>
        public  UInt32 time_delta_distance_us;
            /// <summary> Distance to the center of the flow field in meters. Positive value (including zero): distance known. Negative value: Unknown distance. </summary>
        public  Single distance;
            /// <summary> Temperature * 100 in centi-degrees Celsius </summary>
        public  Int16 temperature;
            /// <summary> Sensor ID </summary>
        public  byte sensor_id;
            /// <summary> Optical flow quality / confidence. 0: no valid flow, 255: maximum quality </summary>
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
        /// <summary> True attitude quaternion component 1, w (1 in null-rotation) </summary>
        public  Single q1;
            /// <summary> True attitude quaternion component 2, x (0 in null-rotation) </summary>
        public  Single q2;
            /// <summary> True attitude quaternion component 3, y (0 in null-rotation) </summary>
        public  Single q3;
            /// <summary> True attitude quaternion component 4, z (0 in null-rotation) </summary>
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
        /// <summary> Receive errors </summary>
        public  UInt16 rxerrors;
            /// <summary> Count of error corrected packets </summary>
        public  UInt16 @fixed;
            /// <summary> Local signal strength </summary>
        public  byte rssi;
            /// <summary> Remote signal strength </summary>
        public  byte remrssi;
            /// <summary> Remaining free buffer space in percent. </summary>
        public  byte txbuf;
            /// <summary> Background noise level </summary>
        public  byte noise;
            /// <summary> Remote background noise level </summary>
        public  byte remnoise;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=254)]
    public struct mavlink_file_transfer_protocol_t
    {
        /// <summary> Network ID (0 for broadcast) </summary>
        public  byte target_network;
            /// <summary> System ID (0 for broadcast) </summary>
        public  byte target_system;
            /// <summary> Component ID (0 for broadcast) </summary>
        public  byte target_component;
            /// <summary> Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification. </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=251)]
		public byte[] payload;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=16)]
    public struct mavlink_timesync_t
    {
        /// <summary> Time sync timestamp 1 </summary>
        public  Int64 tc1;
            /// <summary> Time sync timestamp 2 </summary>
        public  Int64 ts1;
    
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
            /// <summary> Altitude (AMSL, not WGS84), in meters * 1000 (positive for up) </summary>
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    public struct mavlink_hil_optical_flow_t
    {
        /// <summary> Timestamp (microseconds, synced to UNIX time or since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Integration time in microseconds. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the. </summary>
        public  UInt32 integration_time_us;
            /// <summary> Flow in radians around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.) </summary>
        public  Single integrated_x;
            /// <summary> Flow in radians around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.) </summary>
        public  Single integrated_y;
            /// <summary> RH rotation around X axis (rad) </summary>
        public  Single integrated_xgyro;
            /// <summary> RH rotation around Y axis (rad) </summary>
        public  Single integrated_ygyro;
            /// <summary> RH rotation around Z axis (rad) </summary>
        public  Single integrated_zgyro;
            /// <summary> Time in microseconds since the distance was sampled. </summary>
        public  UInt32 time_delta_distance_us;
            /// <summary> Distance to the center of the flow field in meters. Positive value (including zero): distance known. Negative value: Unknown distance. </summary>
        public  Single distance;
            /// <summary> Temperature * 100 in centi-degrees Celsius </summary>
        public  Int16 temperature;
            /// <summary> Sensor ID </summary>
        public  byte sensor_id;
            /// <summary> Optical flow quality / confidence. 0: no valid flow, 255: maximum quality </summary>
        public  byte quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=64)]
    public struct mavlink_hil_state_quaternion_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Vehicle attitude expressed as normalized quaternion in w, x, y, z order (with 1 0 0 0 being the null-rotation) </summary>
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    public struct mavlink_log_request_list_t
    {
        /// <summary> First log id (0 for first available) </summary>
        public  UInt16 start;
            /// <summary> Last log id (0xffff for last available) </summary>
        public  UInt16 end;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    public struct mavlink_log_entry_t
    {
        /// <summary> UTC timestamp of log in seconds since 1970, or 0 if not available </summary>
        public  UInt32 time_utc;
            /// <summary> Size of the log (may be approximate) in bytes </summary>
        public  UInt32 size;
            /// <summary> Log id </summary>
        public  UInt16 id;
            /// <summary> Total number of logs </summary>
        public  UInt16 num_logs;
            /// <summary> High log number </summary>
        public  UInt16 last_log_num;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    public struct mavlink_log_request_data_t
    {
        /// <summary> Offset into the log </summary>
        public  UInt32 ofs;
            /// <summary> Number of bytes </summary>
        public  UInt32 count;
            /// <summary> Log id (from LOG_ENTRY reply) </summary>
        public  UInt16 id;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=97)]
    public struct mavlink_log_data_t
    {
        /// <summary> Offset into the log </summary>
        public  UInt32 ofs;
            /// <summary> Log id (from LOG_ENTRY reply) </summary>
        public  UInt16 id;
            /// <summary> Number of bytes (zero for end of log) </summary>
        public  byte count;
            /// <summary> log data </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=90)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    public struct mavlink_log_erase_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    public struct mavlink_log_request_end_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=113)]
    public struct mavlink_gps_inject_data_t
    {
        /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
            /// <summary> data length </summary>
        public  byte len;
            /// <summary> raw data (110 is enough for 12 satellites of RTCMv2) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=110)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    public struct mavlink_gps2_raw_t
    {
        /// <summary> Timestamp (microseconds since UNIX epoch or microseconds since system boot) </summary>
        public  UInt64 time_usec;
            /// <summary> Latitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 lat;
            /// <summary> Longitude (WGS84), in degrees * 1E7 </summary>
        public  Int32 lon;
            /// <summary> Altitude (AMSL, not WGS84), in meters * 1000 (positive for up) </summary>
        public  Int32 alt;
            /// <summary> Age of DGPS info </summary>
        public  UInt32 dgps_age;
            /// <summary> GPS HDOP horizontal dilution of position in cm (m*100). If unknown, set to: UINT16_MAX </summary>
        public  UInt16 eph;
            /// <summary> GPS VDOP vertical dilution of position in cm (m*100). If unknown, set to: UINT16_MAX </summary>
        public  UInt16 epv;
            /// <summary> GPS ground speed (m/s * 100). If unknown, set to: UINT16_MAX </summary>
        public  UInt16 vel;
            /// <summary> Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX </summary>
        public  UInt16 cog;
            /// <summary> 0-1: no fix, 2: 2D fix, 3: 3D fix, 4: DGPS fix, 5: RTK Fix. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix. </summary>
        public  byte fix_type;
            /// <summary> Number of satellites visible. If unknown, set to 255 </summary>
        public  byte satellites_visible;
            /// <summary> Number of DGPS satellites </summary>
        public  byte dgps_numch;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    public struct mavlink_power_status_t
    {
        /// <summary> 5V rail voltage in millivolts </summary>
        public  UInt16 Vcc;
            /// <summary> servo rail voltage in millivolts </summary>
        public  UInt16 Vservo;
            /// <summary> power supply status flags (see MAV_POWER_STATUS enum) </summary>
        public  UInt16 flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=79)]
    public struct mavlink_serial_control_t
    {
        /// <summary> Baudrate of transfer. Zero means no change. </summary>
        public  UInt32 baudrate;
            /// <summary> Timeout for reply data in milliseconds </summary>
        public  UInt16 timeout;
            /// <summary> See SERIAL_CONTROL_DEV enum </summary>
        public  byte device;
            /// <summary> See SERIAL_CONTROL_FLAG enum </summary>
        public  byte flags;
            /// <summary> how many bytes in this transfer </summary>
        public  byte count;
            /// <summary> serial data </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=70)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    public struct mavlink_gps_rtk_t
    {
        /// <summary> Time since boot of last baseline message received in ms. </summary>
        public  UInt32 time_last_baseline_ms;
            /// <summary> GPS Time of Week of last baseline </summary>
        public  UInt32 tow;
            /// <summary> Current baseline in ECEF x or NED north component in mm. </summary>
        public  Int32 baseline_a_mm;
            /// <summary> Current baseline in ECEF y or NED east component in mm. </summary>
        public  Int32 baseline_b_mm;
            /// <summary> Current baseline in ECEF z or NED down component in mm. </summary>
        public  Int32 baseline_c_mm;
            /// <summary> Current estimate of baseline accuracy. </summary>
        public  UInt32 accuracy;
            /// <summary> Current number of integer ambiguity hypotheses. </summary>
        public  Int32 iar_num_hypotheses;
            /// <summary> GPS Week Number of last baseline </summary>
        public  UInt16 wn;
            /// <summary> Identification of connected RTK receiver. </summary>
        public  byte rtk_receiver_id;
            /// <summary> GPS-specific health report for RTK data. </summary>
        public  byte rtk_health;
            /// <summary> Rate of baseline messages being received by GPS, in HZ </summary>
        public  byte rtk_rate;
            /// <summary> Current number of sats used for RTK calculation. </summary>
        public  byte nsats;
            /// <summary> Coordinate system of baseline. 0 == ECEF, 1 == NED </summary>
        public  byte baseline_coords_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    public struct mavlink_gps2_rtk_t
    {
        /// <summary> Time since boot of last baseline message received in ms. </summary>
        public  UInt32 time_last_baseline_ms;
            /// <summary> GPS Time of Week of last baseline </summary>
        public  UInt32 tow;
            /// <summary> Current baseline in ECEF x or NED north component in mm. </summary>
        public  Int32 baseline_a_mm;
            /// <summary> Current baseline in ECEF y or NED east component in mm. </summary>
        public  Int32 baseline_b_mm;
            /// <summary> Current baseline in ECEF z or NED down component in mm. </summary>
        public  Int32 baseline_c_mm;
            /// <summary> Current estimate of baseline accuracy. </summary>
        public  UInt32 accuracy;
            /// <summary> Current number of integer ambiguity hypotheses. </summary>
        public  Int32 iar_num_hypotheses;
            /// <summary> GPS Week Number of last baseline </summary>
        public  UInt16 wn;
            /// <summary> Identification of connected RTK receiver. </summary>
        public  byte rtk_receiver_id;
            /// <summary> GPS-specific health report for RTK data. </summary>
        public  byte rtk_health;
            /// <summary> Rate of baseline messages being received by GPS, in HZ </summary>
        public  byte rtk_rate;
            /// <summary> Current number of sats used for RTK calculation. </summary>
        public  byte nsats;
            /// <summary> Coordinate system of baseline. 0 == ECEF, 1 == NED </summary>
        public  byte baseline_coords_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    public struct mavlink_scaled_imu3_t
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=13)]
    public struct mavlink_data_transmission_handshake_t
    {
        /// <summary> total data size in bytes (set on ACK only) </summary>
        public  UInt32 size;
            /// <summary> Width of a matrix or image </summary>
        public  UInt16 width;
            /// <summary> Height of a matrix or image </summary>
        public  UInt16 height;
            /// <summary> number of packets beeing sent (set on ACK only) </summary>
        public  UInt16 packets;
            /// <summary> type of requested/acknowledged data (as defined in ENUM DATA_TYPES in mavlink/include/mavlink_types.h) </summary>
        public  byte type;
            /// <summary> payload size per packet (normally 253 byte, see DATA field size in message ENCAPSULATED_DATA) (set on ACK only) </summary>
        public  byte payload;
            /// <summary> JPEG quality out of [1,100] </summary>
        public  byte jpg_quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    public struct mavlink_encapsulated_data_t
    {
        /// <summary> sequence number (starting with 0 on every transmission) </summary>
        public  UInt16 seqnr;
            /// <summary> image data bytes </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=253)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    public struct mavlink_distance_sensor_t
    {
        /// <summary> Time since system boot </summary>
        public  UInt32 time_boot_ms;
            /// <summary> Minimum distance the sensor can measure in centimeters </summary>
        public  UInt16 min_distance;
            /// <summary> Maximum distance the sensor can measure in centimeters </summary>
        public  UInt16 max_distance;
            /// <summary> Current distance reading </summary>
        public  UInt16 current_distance;
            /// <summary> Type from MAV_DISTANCE_SENSOR enum. </summary>
        public  byte type;
            /// <summary> Onboard ID of the sensor </summary>
        public  byte id;
            /// <summary> Direction the sensor faces from FIXME enum. </summary>
        public  byte orientation;
            /// <summary> Measurement covariance in centimeters, 0 for unknown / invalid readings </summary>
        public  byte covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    public struct mavlink_terrain_request_t
    {
        /// <summary> Bitmask of requested 4x4 grids (row major 8x7 array of grids, 56 bits) </summary>
        public  UInt64 mask;
            /// <summary> Latitude of SW corner of first grid (degrees *10^7) </summary>
        public  Int32 lat;
            /// <summary> Longitude of SW corner of first grid (in degrees *10^7) </summary>
        public  Int32 lon;
            /// <summary> Grid spacing in meters </summary>
        public  UInt16 grid_spacing;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=43)]
    public struct mavlink_terrain_data_t
    {
        /// <summary> Latitude of SW corner of first grid (degrees *10^7) </summary>
        public  Int32 lat;
            /// <summary> Longitude of SW corner of first grid (in degrees *10^7) </summary>
        public  Int32 lon;
            /// <summary> Grid spacing in meters </summary>
        public  UInt16 grid_spacing;
            /// <summary> Terrain data in meters AMSL </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public Int16[] data;
            /// <summary> bit within the terrain request mask </summary>
        public  byte gridbit;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    public struct mavlink_terrain_check_t
    {
        /// <summary> Latitude (degrees *10^7) </summary>
        public  Int32 lat;
            /// <summary> Longitude (degrees *10^7) </summary>
        public  Int32 lon;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    public struct mavlink_terrain_report_t
    {
        /// <summary> Latitude (degrees *10^7) </summary>
        public  Int32 lat;
            /// <summary> Longitude (degrees *10^7) </summary>
        public  Int32 lon;
            /// <summary> Terrain height in meters AMSL </summary>
        public  Single terrain_height;
            /// <summary> Current vehicle height above lat/lon terrain height (meters) </summary>
        public  Single current_height;
            /// <summary> grid spacing (zero if terrain at this location unavailable) </summary>
        public  UInt16 spacing;
            /// <summary> Number of 4x4 terrain blocks waiting to be received or read from disk </summary>
        public  UInt16 pending;
            /// <summary> Number of 4x4 terrain blocks in memory </summary>
        public  UInt16 loaded;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    public struct mavlink_scaled_pressure2_t
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


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=36)]
    public struct mavlink_att_pos_mocap_t
    {
        /// <summary> Timestamp (micros since boot or Unix epoch) </summary>
        public  UInt64 time_usec;
            /// <summary> Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float q;
            /// <summary> X position in meters (NED) </summary>
        public  Single x;
            /// <summary> Y position in meters (NED) </summary>
        public  Single y;
            /// <summary> Z position in meters (NED) </summary>
        public  Single z;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=43)]
    public struct mavlink_set_actuator_control_target_t
    {
        /// <summary> Timestamp (micros since boot or Unix epoch) </summary>
        public  UInt64 time_usec;
            /// <summary> Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs. </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public float controls;
            /// <summary> Actuator group. The "_mlx" indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances. </summary>
        public  byte group_mlx;
            /// <summary> System ID </summary>
        public  byte target_system;
            /// <summary> Component ID </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=41)]
    public struct mavlink_actuator_control_target_t
    {
        /// <summary> Timestamp (micros since boot or Unix epoch) </summary>
        public  UInt64 time_usec;
            /// <summary> Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs. </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public float controls;
            /// <summary> Actuator group. The "_mlx" indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances. </summary>
        public  byte group_mlx;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=36)]
    public struct mavlink_battery_status_t
    {
        /// <summary> Consumed charge, in milliampere hours (1 = 1 mAh), -1: autopilot does not provide mAh consumption estimate </summary>
        public  Int32 current_consumed;
            /// <summary> Consumed energy, in 100*Joules (intergrated U*I*dt)  (1 = 100 Joule), -1: autopilot does not provide energy consumption estimate </summary>
        public  Int32 energy_consumed;
            /// <summary> Temperature of the battery in centi-degrees celsius. INT16_MAX for unknown temperature. </summary>
        public  Int16 temperature;
            /// <summary> Battery voltage of cells, in millivolts (1 = 1 millivolt) </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public UInt16[] voltages;
            /// <summary> Battery current, in 10*milliamperes (1 = 10 milliampere), -1: autopilot does not measure the current </summary>
        public  Int16 current_battery;
            /// <summary> Battery ID </summary>
        public  byte id;
            /// <summary> Function of the battery </summary>
        public  byte battery_function;
            /// <summary> Type (chemistry) of the battery </summary>
        public  byte type;
            /// <summary> Remaining battery energy: (0%: 0, 100%: 100), -1: autopilot does not estimate the remaining battery </summary>
        public  byte battery_remaining;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=60)]
    public struct mavlink_autopilot_version_t
    {
        /// <summary> bitmask of capabilities (see MAV_PROTOCOL_CAPABILITY enum) </summary>
        public  UInt64 capabilities;
            /// <summary> UID if provided by hardware </summary>
        public  UInt64 uid;
            /// <summary> Firmware version number </summary>
        public  UInt32 flight_sw_version;
            /// <summary> Middleware version number </summary>
        public  UInt32 middleware_sw_version;
            /// <summary> Operating system version number </summary>
        public  UInt32 os_sw_version;
            /// <summary> HW / board version (last 8 bytes should be silicon ID, if any) </summary>
        public  UInt32 board_version;
            /// <summary> ID of the board vendor </summary>
        public  UInt16 vendor_id;
            /// <summary> ID of the product </summary>
        public  UInt16 product_id;
            /// <summary> Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases. </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] flight_custom_version;
            /// <summary> Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases. </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] middleware_custom_version;
            /// <summary> Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases. </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] os_custom_version;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    public struct mavlink_landing_target_t
    {
        /// <summary> X-axis angular offset (in radians) of the target from the center of the image </summary>
        public  Single angle_x;
            /// <summary> Y-axis angular offset (in radians) of the target from the center of the image </summary>
        public  Single angle_y;
            /// <summary> Distance to the target from the vehicle in meters </summary>
        public  Single distance;
            /// <summary> The ID of the target if multiple targets are present </summary>
        public  byte target_num;
            /// <summary> MAV_FRAME enum specifying the whether the following feilds are earth-frame, body-frame, etc. </summary>
        public  byte frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    public struct mavlink_vibration_t
    {
        /// <summary> Timestamp (micros since boot or Unix epoch) </summary>
        public  UInt64 time_usec;
            /// <summary> Vibration levels on X-axis </summary>
        public  Single vibration_x;
            /// <summary> Vibration levels on Y-axis </summary>
        public  Single vibration_y;
            /// <summary> Vibration levels on Z-axis </summary>
        public  Single vibration_z;
            /// <summary> first accelerometer clipping count </summary>
        public  UInt32 clipping_0;
            /// <summary> second accelerometer clipping count </summary>
        public  UInt32 clipping_1;
            /// <summary> third accelerometer clipping count </summary>
        public  UInt32 clipping_2;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=254)]
    public struct mavlink_v2_extension_t
    {
        /// <summary> A code that identifies the software component that understands this message (analogous to usb device classes or mime type strings).  If this code is less than 32768, it is considered a 'registered' protocol extension and the corresponding entry should be added to https://github.com/mavlink/mavlink/extension-message-ids.xml.  Software creators can register blocks of message IDs as needed (useful for GCS specific metadata, etc...). Message_types greater than 32767 are considered local experiments and should not be checked in to any widely distributed codebase. </summary>
        public  UInt16 message_type;
            /// <summary> Network ID (0 for broadcast) </summary>
        public  byte target_network;
            /// <summary> System ID (0 for broadcast) </summary>
        public  byte target_system;
            /// <summary> Component ID (0 for broadcast) </summary>
        public  byte target_component;
            /// <summary> Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification. </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=249)]
		public byte[] payload;
    
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
