using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public partial class MAVLink
{
    public const string MAVLINK_BUILD_DATE = "Sun Aug 19 2018";
    public const string MAVLINK_WIRE_PROTOCOL_VERSION = "2.0";
    public const int MAVLINK_MAX_PAYLOAD_LEN = 255;

    public const byte MAVLINK_CORE_HEADER_LEN = 9;///< Length of core header (of the comm. layer)
    public const byte MAVLINK_CORE_HEADER_MAVLINK1_LEN = 5;///< Length of MAVLink1 core header (of the comm. layer)
    public const byte MAVLINK_NUM_HEADER_BYTES = (MAVLINK_CORE_HEADER_LEN + 1);///< Length of all header bytes, including core and stx
    public const byte MAVLINK_NUM_CHECKSUM_BYTES = 2;
    public const byte MAVLINK_NUM_NON_PAYLOAD_BYTES = (MAVLINK_NUM_HEADER_BYTES + MAVLINK_NUM_CHECKSUM_BYTES);

    public const int MAVLINK_MAX_PACKET_LEN = (MAVLINK_MAX_PAYLOAD_LEN + MAVLINK_NUM_NON_PAYLOAD_BYTES + MAVLINK_SIGNATURE_BLOCK_LEN);///< Maximum packet length
    public const byte MAVLINK_SIGNATURE_BLOCK_LEN = 13;

    public const int MAVLINK_LITTLE_ENDIAN = 1;
    public const int MAVLINK_BIG_ENDIAN = 0;

    public const byte MAVLINK_STX = 253;

	public const byte MAVLINK_STX_MAVLINK1 = 0xFE;

    public const byte MAVLINK_ENDIAN = MAVLINK_LITTLE_ENDIAN;

    public const bool MAVLINK_ALIGNED_FIELDS = (1 == 1);

    public const byte MAVLINK_CRC_EXTRA = 1;
    
    public const byte MAVLINK_COMMAND_24BIT = 1;
        
    public const bool MAVLINK_NEED_BYTE_SWAP = (MAVLINK_ENDIAN == MAVLINK_LITTLE_ENDIAN);
        
    // msgid, name, crc, length, type
    public static readonly message_info[] MAVLINK_MESSAGE_INFOS = new message_info[] {
		new message_info(0, "HEARTBEAT", 50, 9, 9, typeof( mavlink_heartbeat_t )),
		new message_info(1, "SYS_STATUS", 124, 31, 31, typeof( mavlink_sys_status_t )),
		new message_info(2, "SYSTEM_TIME", 137, 12, 12, typeof( mavlink_system_time_t )),
		new message_info(4, "PING", 237, 14, 14, typeof( mavlink_ping_t )),
		new message_info(5, "CHANGE_OPERATOR_CONTROL", 217, 28, 28, typeof( mavlink_change_operator_control_t )),
		new message_info(6, "CHANGE_OPERATOR_CONTROL_ACK", 104, 3, 3, typeof( mavlink_change_operator_control_ack_t )),
		new message_info(7, "AUTH_KEY", 119, 32, 32, typeof( mavlink_auth_key_t )),
		new message_info(11, "SET_MODE", 89, 6, 6, typeof( mavlink_set_mode_t )),
		new message_info(20, "PARAM_REQUEST_READ", 214, 20, 20, typeof( mavlink_param_request_read_t )),
		new message_info(21, "PARAM_REQUEST_LIST", 159, 2, 2, typeof( mavlink_param_request_list_t )),
		new message_info(22, "PARAM_VALUE", 220, 25, 25, typeof( mavlink_param_value_t )),
		new message_info(23, "PARAM_SET", 168, 23, 23, typeof( mavlink_param_set_t )),
		new message_info(24, "GPS_RAW_INT", 24, 30, 50, typeof( mavlink_gps_raw_int_t )),
		new message_info(25, "GPS_STATUS", 23, 101, 101, typeof( mavlink_gps_status_t )),
		new message_info(26, "SCALED_IMU", 170, 22, 22, typeof( mavlink_scaled_imu_t )),
		new message_info(27, "RAW_IMU", 144, 26, 26, typeof( mavlink_raw_imu_t )),
		new message_info(28, "RAW_PRESSURE", 67, 16, 16, typeof( mavlink_raw_pressure_t )),
		new message_info(29, "SCALED_PRESSURE", 115, 14, 14, typeof( mavlink_scaled_pressure_t )),
		new message_info(30, "ATTITUDE", 39, 28, 28, typeof( mavlink_attitude_t )),
		new message_info(31, "ATTITUDE_QUATERNION", 246, 32, 32, typeof( mavlink_attitude_quaternion_t )),
		new message_info(32, "LOCAL_POSITION_NED", 185, 28, 28, typeof( mavlink_local_position_ned_t )),
		new message_info(33, "GLOBAL_POSITION_INT", 104, 28, 28, typeof( mavlink_global_position_int_t )),
		new message_info(34, "RC_CHANNELS_SCALED", 237, 22, 22, typeof( mavlink_rc_channels_scaled_t )),
		new message_info(35, "RC_CHANNELS_RAW", 244, 22, 22, typeof( mavlink_rc_channels_raw_t )),
		new message_info(36, "SERVO_OUTPUT_RAW", 222, 21, 37, typeof( mavlink_servo_output_raw_t )),
		new message_info(37, "MISSION_REQUEST_PARTIAL_LIST", 212, 6, 7, typeof( mavlink_mission_request_partial_list_t )),
		new message_info(38, "MISSION_WRITE_PARTIAL_LIST", 9, 6, 7, typeof( mavlink_mission_write_partial_list_t )),
		new message_info(39, "MISSION_ITEM", 254, 37, 38, typeof( mavlink_mission_item_t )),
		new message_info(40, "MISSION_REQUEST", 230, 4, 5, typeof( mavlink_mission_request_t )),
		new message_info(41, "MISSION_SET_CURRENT", 28, 4, 4, typeof( mavlink_mission_set_current_t )),
		new message_info(42, "MISSION_CURRENT", 28, 2, 2, typeof( mavlink_mission_current_t )),
		new message_info(43, "MISSION_REQUEST_LIST", 132, 2, 3, typeof( mavlink_mission_request_list_t )),
		new message_info(44, "MISSION_COUNT", 221, 4, 5, typeof( mavlink_mission_count_t )),
		new message_info(45, "MISSION_CLEAR_ALL", 232, 2, 3, typeof( mavlink_mission_clear_all_t )),
		new message_info(46, "MISSION_ITEM_REACHED", 11, 2, 2, typeof( mavlink_mission_item_reached_t )),
		new message_info(47, "MISSION_ACK", 153, 3, 4, typeof( mavlink_mission_ack_t )),
		new message_info(48, "SET_GPS_GLOBAL_ORIGIN", 41, 13, 21, typeof( mavlink_set_gps_global_origin_t )),
		new message_info(49, "GPS_GLOBAL_ORIGIN", 39, 12, 20, typeof( mavlink_gps_global_origin_t )),
		new message_info(50, "PARAM_MAP_RC", 78, 37, 37, typeof( mavlink_param_map_rc_t )),
		new message_info(51, "MISSION_REQUEST_INT", 196, 4, 5, typeof( mavlink_mission_request_int_t )),
		new message_info(54, "SAFETY_SET_ALLOWED_AREA", 15, 27, 27, typeof( mavlink_safety_set_allowed_area_t )),
		new message_info(55, "SAFETY_ALLOWED_AREA", 3, 25, 25, typeof( mavlink_safety_allowed_area_t )),
		new message_info(61, "ATTITUDE_QUATERNION_COV", 167, 72, 72, typeof( mavlink_attitude_quaternion_cov_t )),
		new message_info(62, "NAV_CONTROLLER_OUTPUT", 183, 26, 26, typeof( mavlink_nav_controller_output_t )),
		new message_info(63, "GLOBAL_POSITION_INT_COV", 119, 181, 181, typeof( mavlink_global_position_int_cov_t )),
		new message_info(64, "LOCAL_POSITION_NED_COV", 191, 225, 225, typeof( mavlink_local_position_ned_cov_t )),
		new message_info(65, "RC_CHANNELS", 118, 42, 42, typeof( mavlink_rc_channels_t )),
		new message_info(66, "REQUEST_DATA_STREAM", 148, 6, 6, typeof( mavlink_request_data_stream_t )),
		new message_info(67, "DATA_STREAM", 21, 4, 4, typeof( mavlink_data_stream_t )),
		new message_info(69, "MANUAL_CONTROL", 243, 11, 11, typeof( mavlink_manual_control_t )),
		new message_info(70, "RC_CHANNELS_OVERRIDE", 124, 18, 38, typeof( mavlink_rc_channels_override_t )),
		new message_info(73, "MISSION_ITEM_INT", 38, 37, 38, typeof( mavlink_mission_item_int_t )),
		new message_info(74, "VFR_HUD", 20, 20, 20, typeof( mavlink_vfr_hud_t )),
		new message_info(75, "COMMAND_INT", 158, 35, 35, typeof( mavlink_command_int_t )),
		new message_info(76, "COMMAND_LONG", 152, 33, 33, typeof( mavlink_command_long_t )),
		new message_info(77, "COMMAND_ACK", 143, 3, 3, typeof( mavlink_command_ack_t )),
		new message_info(81, "MANUAL_SETPOINT", 106, 22, 22, typeof( mavlink_manual_setpoint_t )),
		new message_info(82, "SET_ATTITUDE_TARGET", 49, 39, 39, typeof( mavlink_set_attitude_target_t )),
		new message_info(83, "ATTITUDE_TARGET", 22, 37, 37, typeof( mavlink_attitude_target_t )),
		new message_info(84, "SET_POSITION_TARGET_LOCAL_NED", 143, 53, 53, typeof( mavlink_set_position_target_local_ned_t )),
		new message_info(85, "POSITION_TARGET_LOCAL_NED", 140, 51, 51, typeof( mavlink_position_target_local_ned_t )),
		new message_info(86, "SET_POSITION_TARGET_GLOBAL_INT", 5, 53, 53, typeof( mavlink_set_position_target_global_int_t )),
		new message_info(87, "POSITION_TARGET_GLOBAL_INT", 150, 51, 51, typeof( mavlink_position_target_global_int_t )),
		new message_info(89, "LOCAL_POSITION_NED_SYSTEM_GLOBAL_OFFSET", 231, 28, 28, typeof( mavlink_local_position_ned_system_global_offset_t )),
		new message_info(90, "HIL_STATE", 183, 56, 56, typeof( mavlink_hil_state_t )),
		new message_info(91, "HIL_CONTROLS", 63, 42, 42, typeof( mavlink_hil_controls_t )),
		new message_info(92, "HIL_RC_INPUTS_RAW", 54, 33, 33, typeof( mavlink_hil_rc_inputs_raw_t )),
		new message_info(93, "HIL_ACTUATOR_CONTROLS", 47, 81, 81, typeof( mavlink_hil_actuator_controls_t )),
		new message_info(100, "OPTICAL_FLOW", 175, 26, 34, typeof( mavlink_optical_flow_t )),
		new message_info(101, "GLOBAL_VISION_POSITION_ESTIMATE", 102, 32, 116, typeof( mavlink_global_vision_position_estimate_t )),
		new message_info(102, "VISION_POSITION_ESTIMATE", 158, 32, 116, typeof( mavlink_vision_position_estimate_t )),
		new message_info(103, "VISION_SPEED_ESTIMATE", 208, 20, 56, typeof( mavlink_vision_speed_estimate_t )),
		new message_info(104, "VICON_POSITION_ESTIMATE", 56, 32, 116, typeof( mavlink_vicon_position_estimate_t )),
		new message_info(105, "HIGHRES_IMU", 93, 62, 62, typeof( mavlink_highres_imu_t )),
		new message_info(106, "OPTICAL_FLOW_RAD", 138, 44, 44, typeof( mavlink_optical_flow_rad_t )),
		new message_info(107, "HIL_SENSOR", 108, 64, 64, typeof( mavlink_hil_sensor_t )),
		new message_info(108, "SIM_STATE", 32, 84, 84, typeof( mavlink_sim_state_t )),
		new message_info(109, "RADIO_STATUS", 185, 9, 9, typeof( mavlink_radio_status_t )),
		new message_info(110, "FILE_TRANSFER_PROTOCOL", 84, 254, 254, typeof( mavlink_file_transfer_protocol_t )),
		new message_info(111, "TIMESYNC", 34, 16, 16, typeof( mavlink_timesync_t )),
		new message_info(112, "CAMERA_TRIGGER", 174, 12, 12, typeof( mavlink_camera_trigger_t )),
		new message_info(113, "HIL_GPS", 124, 36, 36, typeof( mavlink_hil_gps_t )),
		new message_info(114, "HIL_OPTICAL_FLOW", 237, 44, 44, typeof( mavlink_hil_optical_flow_t )),
		new message_info(115, "HIL_STATE_QUATERNION", 4, 64, 64, typeof( mavlink_hil_state_quaternion_t )),
		new message_info(116, "SCALED_IMU2", 76, 22, 22, typeof( mavlink_scaled_imu2_t )),
		new message_info(117, "LOG_REQUEST_LIST", 128, 6, 6, typeof( mavlink_log_request_list_t )),
		new message_info(118, "LOG_ENTRY", 56, 14, 14, typeof( mavlink_log_entry_t )),
		new message_info(119, "LOG_REQUEST_DATA", 116, 12, 12, typeof( mavlink_log_request_data_t )),
		new message_info(120, "LOG_DATA", 134, 97, 97, typeof( mavlink_log_data_t )),
		new message_info(121, "LOG_ERASE", 237, 2, 2, typeof( mavlink_log_erase_t )),
		new message_info(122, "LOG_REQUEST_END", 203, 2, 2, typeof( mavlink_log_request_end_t )),
		new message_info(123, "GPS_INJECT_DATA", 250, 113, 113, typeof( mavlink_gps_inject_data_t )),
		new message_info(124, "GPS2_RAW", 87, 35, 35, typeof( mavlink_gps2_raw_t )),
		new message_info(125, "POWER_STATUS", 203, 6, 6, typeof( mavlink_power_status_t )),
		new message_info(126, "SERIAL_CONTROL", 220, 79, 79, typeof( mavlink_serial_control_t )),
		new message_info(127, "GPS_RTK", 25, 35, 35, typeof( mavlink_gps_rtk_t )),
		new message_info(128, "GPS2_RTK", 226, 35, 35, typeof( mavlink_gps2_rtk_t )),
		new message_info(129, "SCALED_IMU3", 46, 22, 22, typeof( mavlink_scaled_imu3_t )),
		new message_info(130, "DATA_TRANSMISSION_HANDSHAKE", 29, 13, 13, typeof( mavlink_data_transmission_handshake_t )),
		new message_info(131, "ENCAPSULATED_DATA", 223, 255, 255, typeof( mavlink_encapsulated_data_t )),
		new message_info(132, "DISTANCE_SENSOR", 85, 14, 14, typeof( mavlink_distance_sensor_t )),
		new message_info(133, "TERRAIN_REQUEST", 6, 18, 18, typeof( mavlink_terrain_request_t )),
		new message_info(134, "TERRAIN_DATA", 229, 43, 43, typeof( mavlink_terrain_data_t )),
		new message_info(135, "TERRAIN_CHECK", 203, 8, 8, typeof( mavlink_terrain_check_t )),
		new message_info(136, "TERRAIN_REPORT", 1, 22, 22, typeof( mavlink_terrain_report_t )),
		new message_info(137, "SCALED_PRESSURE2", 195, 14, 14, typeof( mavlink_scaled_pressure2_t )),
		new message_info(138, "ATT_POS_MOCAP", 109, 36, 120, typeof( mavlink_att_pos_mocap_t )),
		new message_info(139, "SET_ACTUATOR_CONTROL_TARGET", 168, 43, 43, typeof( mavlink_set_actuator_control_target_t )),
		new message_info(140, "ACTUATOR_CONTROL_TARGET", 181, 41, 41, typeof( mavlink_actuator_control_target_t )),
		new message_info(141, "ALTITUDE", 47, 32, 32, typeof( mavlink_altitude_t )),
		new message_info(142, "RESOURCE_REQUEST", 72, 243, 243, typeof( mavlink_resource_request_t )),
		new message_info(143, "SCALED_PRESSURE3", 131, 14, 14, typeof( mavlink_scaled_pressure3_t )),
		new message_info(144, "FOLLOW_TARGET", 127, 93, 93, typeof( mavlink_follow_target_t )),
		new message_info(146, "CONTROL_SYSTEM_STATE", 103, 100, 100, typeof( mavlink_control_system_state_t )),
		new message_info(147, "BATTERY_STATUS", 154, 36, 41, typeof( mavlink_battery_status_t )),
		new message_info(148, "AUTOPILOT_VERSION", 178, 60, 78, typeof( mavlink_autopilot_version_t )),
		new message_info(149, "LANDING_TARGET", 200, 30, 60, typeof( mavlink_landing_target_t )),
		new message_info(150, "SENSOR_OFFSETS", 134, 42, 42, typeof( mavlink_sensor_offsets_t )),
		new message_info(151, "SET_MAG_OFFSETS", 219, 8, 8, typeof( mavlink_set_mag_offsets_t )),
		new message_info(152, "MEMINFO", 208, 4, 8, typeof( mavlink_meminfo_t )),
		new message_info(153, "AP_ADC", 188, 12, 12, typeof( mavlink_ap_adc_t )),
		new message_info(154, "DIGICAM_CONFIGURE", 84, 15, 15, typeof( mavlink_digicam_configure_t )),
		new message_info(155, "DIGICAM_CONTROL", 22, 13, 13, typeof( mavlink_digicam_control_t )),
		new message_info(156, "MOUNT_CONFIGURE", 19, 6, 6, typeof( mavlink_mount_configure_t )),
		new message_info(157, "MOUNT_CONTROL", 21, 15, 15, typeof( mavlink_mount_control_t )),
		new message_info(158, "MOUNT_STATUS", 134, 14, 14, typeof( mavlink_mount_status_t )),
		new message_info(160, "FENCE_POINT", 78, 12, 12, typeof( mavlink_fence_point_t )),
		new message_info(161, "FENCE_FETCH_POINT", 68, 3, 3, typeof( mavlink_fence_fetch_point_t )),
		new message_info(162, "FENCE_STATUS", 189, 8, 8, typeof( mavlink_fence_status_t )),
		new message_info(163, "AHRS", 127, 28, 28, typeof( mavlink_ahrs_t )),
		new message_info(164, "SIMSTATE", 154, 44, 44, typeof( mavlink_simstate_t )),
		new message_info(165, "HWSTATUS", 21, 3, 3, typeof( mavlink_hwstatus_t )),
		new message_info(166, "RADIO", 21, 9, 9, typeof( mavlink_radio_t )),
		new message_info(167, "LIMITS_STATUS", 144, 22, 22, typeof( mavlink_limits_status_t )),
		new message_info(168, "WIND", 1, 12, 12, typeof( mavlink_wind_t )),
		new message_info(169, "DATA16", 234, 18, 18, typeof( mavlink_data16_t )),
		new message_info(170, "DATA32", 73, 34, 34, typeof( mavlink_data32_t )),
		new message_info(171, "DATA64", 181, 66, 66, typeof( mavlink_data64_t )),
		new message_info(172, "DATA96", 22, 98, 98, typeof( mavlink_data96_t )),
		new message_info(173, "RANGEFINDER", 83, 8, 8, typeof( mavlink_rangefinder_t )),
		new message_info(174, "AIRSPEED_AUTOCAL", 167, 48, 48, typeof( mavlink_airspeed_autocal_t )),
		new message_info(175, "RALLY_POINT", 138, 19, 19, typeof( mavlink_rally_point_t )),
		new message_info(176, "RALLY_FETCH_POINT", 234, 3, 3, typeof( mavlink_rally_fetch_point_t )),
		new message_info(177, "COMPASSMOT_STATUS", 240, 20, 20, typeof( mavlink_compassmot_status_t )),
		new message_info(178, "AHRS2", 47, 24, 24, typeof( mavlink_ahrs2_t )),
		new message_info(179, "CAMERA_STATUS", 189, 29, 29, typeof( mavlink_camera_status_t )),
		new message_info(180, "CAMERA_FEEDBACK", 52, 45, 47, typeof( mavlink_camera_feedback_t )),
		new message_info(181, "BATTERY2", 174, 4, 4, typeof( mavlink_battery2_t )),
		new message_info(182, "AHRS3", 229, 40, 40, typeof( mavlink_ahrs3_t )),
		new message_info(183, "AUTOPILOT_VERSION_REQUEST", 85, 2, 2, typeof( mavlink_autopilot_version_request_t )),
		new message_info(184, "REMOTE_LOG_DATA_BLOCK", 159, 206, 206, typeof( mavlink_remote_log_data_block_t )),
		new message_info(185, "REMOTE_LOG_BLOCK_STATUS", 186, 7, 7, typeof( mavlink_remote_log_block_status_t )),
		new message_info(186, "LED_CONTROL", 72, 29, 29, typeof( mavlink_led_control_t )),
		new message_info(191, "MAG_CAL_PROGRESS", 92, 27, 27, typeof( mavlink_mag_cal_progress_t )),
		new message_info(192, "MAG_CAL_REPORT", 36, 44, 50, typeof( mavlink_mag_cal_report_t )),
		new message_info(193, "EKF_STATUS_REPORT", 71, 22, 26, typeof( mavlink_ekf_status_report_t )),
		new message_info(194, "PID_TUNING", 98, 25, 25, typeof( mavlink_pid_tuning_t )),
		new message_info(195, "DEEPSTALL", 120, 37, 37, typeof( mavlink_deepstall_t )),
		new message_info(200, "GIMBAL_REPORT", 134, 42, 42, typeof( mavlink_gimbal_report_t )),
		new message_info(201, "GIMBAL_CONTROL", 205, 14, 14, typeof( mavlink_gimbal_control_t )),
		new message_info(214, "GIMBAL_TORQUE_CMD_REPORT", 69, 8, 8, typeof( mavlink_gimbal_torque_cmd_report_t )),
		new message_info(215, "GOPRO_HEARTBEAT", 101, 3, 3, typeof( mavlink_gopro_heartbeat_t )),
		new message_info(216, "GOPRO_GET_REQUEST", 50, 3, 3, typeof( mavlink_gopro_get_request_t )),
		new message_info(217, "GOPRO_GET_RESPONSE", 202, 6, 6, typeof( mavlink_gopro_get_response_t )),
		new message_info(218, "GOPRO_SET_REQUEST", 17, 7, 7, typeof( mavlink_gopro_set_request_t )),
		new message_info(219, "GOPRO_SET_RESPONSE", 162, 2, 2, typeof( mavlink_gopro_set_response_t )),
		new message_info(226, "RPM", 207, 8, 8, typeof( mavlink_rpm_t )),
		new message_info(230, "ESTIMATOR_STATUS", 163, 42, 42, typeof( mavlink_estimator_status_t )),
		new message_info(231, "WIND_COV", 105, 40, 40, typeof( mavlink_wind_cov_t )),
		new message_info(232, "GPS_INPUT", 151, 63, 63, typeof( mavlink_gps_input_t )),
		new message_info(233, "GPS_RTCM_DATA", 35, 182, 182, typeof( mavlink_gps_rtcm_data_t )),
		new message_info(234, "HIGH_LATENCY", 150, 40, 40, typeof( mavlink_high_latency_t )),
		new message_info(241, "VIBRATION", 90, 32, 32, typeof( mavlink_vibration_t )),
		new message_info(242, "HOME_POSITION", 104, 52, 60, typeof( mavlink_home_position_t )),
		new message_info(243, "SET_HOME_POSITION", 85, 53, 61, typeof( mavlink_set_home_position_t )),
		new message_info(244, "MESSAGE_INTERVAL", 95, 6, 6, typeof( mavlink_message_interval_t )),
		new message_info(245, "EXTENDED_SYS_STATE", 130, 2, 2, typeof( mavlink_extended_sys_state_t )),
		new message_info(246, "ADSB_VEHICLE", 184, 38, 38, typeof( mavlink_adsb_vehicle_t )),
		new message_info(247, "COLLISION", 81, 19, 19, typeof( mavlink_collision_t )),
		new message_info(248, "V2_EXTENSION", 8, 254, 254, typeof( mavlink_v2_extension_t )),
		new message_info(249, "MEMORY_VECT", 204, 36, 36, typeof( mavlink_memory_vect_t )),
		new message_info(250, "DEBUG_VECT", 49, 30, 30, typeof( mavlink_debug_vect_t )),
		new message_info(251, "NAMED_VALUE_FLOAT", 170, 18, 18, typeof( mavlink_named_value_float_t )),
		new message_info(252, "NAMED_VALUE_INT", 44, 18, 18, typeof( mavlink_named_value_int_t )),
		new message_info(253, "STATUSTEXT", 83, 51, 51, typeof( mavlink_statustext_t )),
		new message_info(254, "DEBUG", 46, 9, 9, typeof( mavlink_debug_t )),
		new message_info(256, "SETUP_SIGNING", 71, 42, 42, typeof( mavlink_setup_signing_t )),
		new message_info(257, "BUTTON_CHANGE", 131, 9, 9, typeof( mavlink_button_change_t )),
		new message_info(258, "PLAY_TUNE", 187, 32, 232, typeof( mavlink_play_tune_t )),
		new message_info(259, "CAMERA_INFORMATION", 122, 86, 86, typeof( mavlink_camera_information_t )),
		new message_info(260, "CAMERA_SETTINGS", 8, 28, 28, typeof( mavlink_camera_settings_t )),
		new message_info(261, "STORAGE_INFORMATION", 179, 27, 27, typeof( mavlink_storage_information_t )),
		new message_info(262, "CAMERA_CAPTURE_STATUS", 69, 31, 31, typeof( mavlink_camera_capture_status_t )),
		new message_info(263, "CAMERA_IMAGE_CAPTURED", 133, 255, 255, typeof( mavlink_camera_image_captured_t )),
		new message_info(264, "FLIGHT_INFORMATION", 49, 28, 28, typeof( mavlink_flight_information_t )),
		new message_info(265, "MOUNT_ORIENTATION", 26, 16, 20, typeof( mavlink_mount_orientation_t )),
		new message_info(266, "LOGGING_DATA", 193, 255, 255, typeof( mavlink_logging_data_t )),
		new message_info(267, "LOGGING_DATA_ACKED", 35, 255, 255, typeof( mavlink_logging_data_acked_t )),
		new message_info(268, "LOGGING_ACK", 14, 4, 4, typeof( mavlink_logging_ack_t )),
		new message_info(299, "WIFI_CONFIG_AP", 19, 96, 96, typeof( mavlink_wifi_config_ap_t )),
		new message_info(310, "UAVCAN_NODE_STATUS", 28, 17, 17, typeof( mavlink_uavcan_node_status_t )),
		new message_info(311, "UAVCAN_NODE_INFO", 95, 116, 116, typeof( mavlink_uavcan_node_info_t )),
		new message_info(330, "OBSTACLE_DISTANCE", 23, 158, 158, typeof( mavlink_obstacle_distance_t )),
		new message_info(331, "ODOMETRY", 58, 230, 230, typeof( mavlink_odometry_t )),
		new message_info(10001, "UAVIONIX_ADSB_OUT_CFG", 209, 20, 20, typeof( mavlink_uavionix_adsb_out_cfg_t )),
		new message_info(10002, "UAVIONIX_ADSB_OUT_DYNAMIC", 186, 41, 41, typeof( mavlink_uavionix_adsb_out_dynamic_t )),
		new message_info(10003, "UAVIONIX_ADSB_TRANSCEIVER_HEALTH_REPORT", 4, 1, 1, typeof( mavlink_uavionix_adsb_transceiver_health_report_t )),
		new message_info(11000, "DEVICE_OP_READ", 134, 51, 51, typeof( mavlink_device_op_read_t )),
		new message_info(11001, "DEVICE_OP_READ_REPLY", 15, 135, 135, typeof( mavlink_device_op_read_reply_t )),
		new message_info(11002, "DEVICE_OP_WRITE", 234, 179, 179, typeof( mavlink_device_op_write_t )),
		new message_info(11003, "DEVICE_OP_WRITE_REPLY", 64, 5, 5, typeof( mavlink_device_op_write_reply_t )),
		new message_info(11010, "ADAP_TUNING", 46, 49, 49, typeof( mavlink_adap_tuning_t )),
		new message_info(11011, "VISION_POSITION_DELTA", 106, 44, 44, typeof( mavlink_vision_position_delta_t )),
		new message_info(11020, "AOA_SSA", 205, 16, 16, typeof( mavlink_aoa_ssa_t )),
		new message_info(11030, "ESC_TELEMETRY_1_TO_4", 144, 44, 44, typeof( mavlink_esc_telemetry_1_to_4_t )),
		new message_info(11031, "ESC_TELEMETRY_5_TO_8", 133, 44, 44, typeof( mavlink_esc_telemetry_5_to_8_t )),
		new message_info(11032, "ESC_TELEMETRY_9_TO_12", 85, 44, 44, typeof( mavlink_esc_telemetry_9_to_12_t )),
		new message_info(42000, "ICAROUS_HEARTBEAT", 227, 1, 1, typeof( mavlink_icarous_heartbeat_t )),
		new message_info(42001, "ICAROUS_KINEMATIC_BANDS", 239, 46, 46, typeof( mavlink_icarous_kinematic_bands_t )),

	};

    public const byte MAVLINK_VERSION = 2;

	public const byte MAVLINK_IFLAG_SIGNED=  0x01;
	public const byte MAVLINK_IFLAG_MASK   = 0x01;

    public struct message_info
    {
        public uint msgid { get; internal set; }
        public string name { get; internal set; }
        public byte crc { get; internal set; }
        public uint minlength { get; internal set; }
        public uint length { get; internal set; }
        public Type type { get; internal set; }

        public message_info(uint msgid, string name, byte crc, uint minlength, uint length, Type type)
        {
            this.msgid = msgid;
            this.name = name;
            this.crc = crc;
			this.minlength = minlength;
            this.length = length;
            this.type = type;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}",name,msgid);
        }
    }   

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
MISSION_REQUEST_INT = 51,
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
HIL_ACTUATOR_CONTROLS = 93,
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
CAMERA_TRIGGER = 112,
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
ALTITUDE = 141,
RESOURCE_REQUEST = 142,
SCALED_PRESSURE3 = 143,
FOLLOW_TARGET = 144,
CONTROL_SYSTEM_STATE = 146,
BATTERY_STATUS = 147,
AUTOPILOT_VERSION = 148,
LANDING_TARGET = 149,
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
COMPASSMOT_STATUS = 177,
AHRS2 = 178,
CAMERA_STATUS = 179,
CAMERA_FEEDBACK = 180,
BATTERY2 = 181,
AHRS3 = 182,
AUTOPILOT_VERSION_REQUEST = 183,
REMOTE_LOG_DATA_BLOCK = 184,
REMOTE_LOG_BLOCK_STATUS = 185,
LED_CONTROL = 186,
MAG_CAL_PROGRESS = 191,
MAG_CAL_REPORT = 192,
EKF_STATUS_REPORT = 193,
PID_TUNING = 194,
DEEPSTALL = 195,
GIMBAL_REPORT = 200,
GIMBAL_CONTROL = 201,
GIMBAL_TORQUE_CMD_REPORT = 214,
GOPRO_HEARTBEAT = 215,
GOPRO_GET_REQUEST = 216,
GOPRO_GET_RESPONSE = 217,
GOPRO_SET_REQUEST = 218,
GOPRO_SET_RESPONSE = 219,
RPM = 226,
ESTIMATOR_STATUS = 230,
WIND_COV = 231,
GPS_INPUT = 232,
GPS_RTCM_DATA = 233,
HIGH_LATENCY = 234,
VIBRATION = 241,
HOME_POSITION = 242,
SET_HOME_POSITION = 243,
MESSAGE_INTERVAL = 244,
EXTENDED_SYS_STATE = 245,
ADSB_VEHICLE = 246,
COLLISION = 247,
V2_EXTENSION = 248,
MEMORY_VECT = 249,
DEBUG_VECT = 250,
NAMED_VALUE_FLOAT = 251,
NAMED_VALUE_INT = 252,
STATUSTEXT = 253,
DEBUG = 254,
SETUP_SIGNING = 256,
BUTTON_CHANGE = 257,
PLAY_TUNE = 258,
CAMERA_INFORMATION = 259,
CAMERA_SETTINGS = 260,
STORAGE_INFORMATION = 261,
CAMERA_CAPTURE_STATUS = 262,
CAMERA_IMAGE_CAPTURED = 263,
FLIGHT_INFORMATION = 264,
MOUNT_ORIENTATION = 265,
LOGGING_DATA = 266,
LOGGING_DATA_ACKED = 267,
LOGGING_ACK = 268,
WIFI_CONFIG_AP = 299,
UAVCAN_NODE_STATUS = 310,
UAVCAN_NODE_INFO = 311,
OBSTACLE_DISTANCE = 330,
ODOMETRY = 331,
UAVIONIX_ADSB_OUT_CFG = 10001,
UAVIONIX_ADSB_OUT_DYNAMIC = 10002,
UAVIONIX_ADSB_TRANSCEIVER_HEALTH_REPORT = 10003,
DEVICE_OP_READ = 11000,
DEVICE_OP_READ_REPLY = 11001,
DEVICE_OP_WRITE = 11002,
DEVICE_OP_WRITE_REPLY = 11003,
ADAP_TUNING = 11010,
VISION_POSITION_DELTA = 11011,
AOA_SSA = 11020,
ESC_TELEMETRY_1_TO_4 = 11030,
ESC_TELEMETRY_5_TO_8 = 11031,
ESC_TELEMETRY_9_TO_12 = 11032,
ICAROUS_HEARTBEAT = 42000,
ICAROUS_KINEMATIC_BANDS = 42001,

    }  
	    
    
    ///<summary>  </summary>
    public enum ACCELCAL_VEHICLE_POS: int /*default*/
    {
			///<summary>  | </summary>
        LEVEL=1, 
    	///<summary>  | </summary>
        LEFT=2, 
    	///<summary>  | </summary>
        RIGHT=3, 
    	///<summary>  | </summary>
        NOSEDOWN=4, 
    	///<summary>  | </summary>
        NOSEUP=5, 
    	///<summary>  | </summary>
        BACK=6, 
    	///<summary>  | </summary>
        SUCCESS=16777215, 
    	///<summary>  | </summary>
        FAILED=16777216, 
    
    };
    
    ///<summary> Commands to be executed by the MAV. They can be executed on user request, or as part of a mission script. If the action is used in a mission, the parameter mapping to the waypoint/mission message is as follows: Param 1, Param 2, Param 3, Param 4, X: Param 5, Y:Param 6, Z:Param 7. This command list is similar what ARINC 424 is for commercial aircraft: A data format how to interpret waypoint/mission data. </summary>
    public enum MAV_CMD: ushort
    {
			///<summary> Navigate to waypoint. |Hold time in decimal seconds. (ignored by fixed wing, time to stay at waypoint for rotary wing)| Acceptance radius in meters (if the sphere with this radius is hit, the waypoint counts as reached)| 0 to pass through the WP, if > 0 radius in meters to pass by WP. Positive value for clockwise orbit, negative value for counter-clockwise orbit. Allows trajectory control.| Desired yaw angle at waypoint (rotary wing). NaN for unchanged.| Latitude| Longitude| Altitude|  </summary>
        WAYPOINT=16, 
    	///<summary> Loiter around this waypoint an unlimited amount of time |Empty| Empty| Radius around waypoint, in meters. If positive loiter clockwise, else counter-clockwise| Desired yaw angle.| Latitude| Longitude| Altitude|  </summary>
        LOITER_UNLIM=17, 
    	///<summary> Loiter around this waypoint for X turns |Turns| Empty| Radius around waypoint, in meters. If positive loiter clockwise, else counter-clockwise| Forward moving aircraft this sets exit xtrack location: 0 for center of loiter wp, 1 for exit location. Else, this is desired yaw angle| Latitude| Longitude| Altitude|  </summary>
        LOITER_TURNS=18, 
    	///<summary> Loiter around this waypoint for X seconds |Seconds (decimal)| Empty| Radius around waypoint, in meters. If positive loiter clockwise, else counter-clockwise| Forward moving aircraft this sets exit xtrack location: 0 for center of loiter wp, 1 for exit location. Else, this is desired yaw angle| Latitude| Longitude| Altitude|  </summary>
        LOITER_TIME=19, 
    	///<summary> Return to launch location |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        RETURN_TO_LAUNCH=20, 
    	///<summary> Land at location |Abort Alt| Empty| Empty| Desired yaw angle. NaN for unchanged.| Latitude| Longitude| Altitude (ground level)|  </summary>
        LAND=21, 
    	///<summary> Takeoff from ground / hand |Minimum pitch (if airspeed sensor present), desired pitch without sensor| Empty| Empty| Yaw angle (if magnetometer present), ignored without magnetometer. NaN for unchanged.| Latitude| Longitude| Altitude|  </summary>
        TAKEOFF=22, 
    	///<summary> Land at local position (local frame only) |Landing target number (if available)| Maximum accepted offset from desired landing position [m] - computed magnitude from spherical coordinates: d = sqrt(x^2 + y^2 + z^2), which gives the maximum accepted distance between the desired landing position and the position where the vehicle is about to land| Landing descend rate [ms^-1]| Desired yaw angle [rad]| Y-axis position [m]| X-axis position [m]| Z-axis / ground level position [m]|  </summary>
        LAND_LOCAL=23, 
    	///<summary> Takeoff from local position (local frame only) |Minimum pitch (if airspeed sensor present), desired pitch without sensor [rad]| Empty| Takeoff ascend rate [ms^-1]| Yaw angle [rad] (if magnetometer or another yaw estimation source present), ignored without one of these| Y-axis position [m]| X-axis position [m]| Z-axis position [m]|  </summary>
        TAKEOFF_LOCAL=24, 
    	///<summary> Vehicle following, i.e. this waypoint represents the position of a moving vehicle |Following logic to use (e.g. loitering or sinusoidal following) - depends on specific autopilot implementation| Ground speed of vehicle to be followed| Radius around waypoint, in meters. If positive loiter clockwise, else counter-clockwise| Desired yaw angle.| Latitude| Longitude| Altitude|  </summary>
        FOLLOW=25, 
    	///<summary> Continue on the current course and climb/descend to specified altitude.  When the altitude is reached continue to the next command (i.e., don't proceed to the next command until the desired altitude is reached. |Climb or Descend (0 = Neutral, command completes when within 5m of this command's altitude, 1 = Climbing, command completes when at or above this command's altitude, 2 = Descending, command completes when at or below this command's altitude. | Empty| Empty| Empty| Empty| Empty| Desired altitude in meters|  </summary>
        CONTINUE_AND_CHANGE_ALT=30, 
    	///<summary> Begin loiter at the specified Latitude and Longitude.  If Lat=Lon=0, then loiter at the current position.  Don't consider the navigation command complete (don't leave loiter) until the altitude has been reached.  Additionally, if the Heading Required parameter is non-zero the  aircraft will not leave the loiter until heading toward the next waypoint.  |Heading Required (0 = False)| Radius in meters. If positive loiter clockwise, negative counter-clockwise, 0 means no change to standard loiter.| Empty| Forward moving aircraft this sets exit xtrack location: 0 for center of loiter wp, 1 for exit location| Latitude| Longitude| Altitude|  </summary>
        LOITER_TO_ALT=31, 
    	///<summary> Being following a target |System ID (the system ID of the FOLLOW_TARGET beacon). Send 0 to disable follow-me and return to the default position hold mode| RESERVED| RESERVED| altitude flag: 0: Keep current altitude, 1: keep altitude difference to target, 2: go to a fixed altitude above home| altitude| RESERVED| TTL in seconds in which the MAV should go to the default position hold mode after a message rx timeout|  </summary>
        DO_FOLLOW=32, 
    	///<summary> Reposition the MAV after a follow target command has been sent |Camera q1 (where 0 is on the ray from the camera to the tracking device)| Camera q2| Camera q3| Camera q4| altitude offset from target (m)| X offset from target (m)| Y offset from target (m)|  </summary>
        DO_FOLLOW_REPOSITION=33, 
    	///<summary> THIS INTERFACE IS DEPRECATED AS OF JANUARY 2018. Please use MAV_CMD_DO_SET_ROI_* messages instead. Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Region of intereset mode. (see MAV_ROI enum)| Waypoint index/ target ID. (see MAV_ROI enum)| ROI index (allows a vehicle to manage multiple ROI's)| Empty| x the location of the fixed ROI (see MAV_FRAME)| y| z|  </summary>
        ROI=80, 
    	///<summary> Control autonomous path planning on the MAV. |0: Disable local obstacle avoidance / local path planning (without resetting map), 1: Enable local path planning, 2: Enable and reset local path planning| 0: Disable full path planning (without resetting map), 1: Enable, 2: Enable and reset map/occupancy grid, 3: Enable and reset planned route, but not occupancy grid| Empty| Yaw angle at goal, in compass degrees, [0..360]| Latitude/X of goal| Longitude/Y of goal| Altitude/Z of goal|  </summary>
        PATHPLANNING=81, 
    	///<summary> Navigate to waypoint using a spline path. |Hold time in decimal seconds. (ignored by fixed wing, time to stay at waypoint for rotary wing)| Empty| Empty| Empty| Latitude/X of goal| Longitude/Y of goal| Altitude/Z of goal|  </summary>
        SPLINE_WAYPOINT=82, 
    	///<summary> Mission command to wait for an altitude or downwards vertical speed. This is meant for high altitude balloon launches, allowing the aircraft to be idle until either an altitude is reached or a negative vertical speed is reached (indicating early balloon burst). The wiggle time is how often to wiggle the control surfaces to prevent them seizing up. |altitude (m)| descent speed (m/s)| Wiggle Time (s)| Empty| Empty| Empty| Empty|  </summary>
        ALTITUDE_WAIT=83, 
    	///<summary> Takeoff from ground using VTOL mode |Empty| Front transition heading, see VTOL_TRANSITION_HEADING enum.| Empty| Yaw angle in degrees. NaN for unchanged.| Latitude| Longitude| Altitude|  </summary>
        VTOL_TAKEOFF=84, 
    	///<summary> Land using VTOL mode |Empty| Empty| Approach altitude (with the same reference as the Altitude field). NaN if unspecified.| Yaw angle in degrees. NaN for unchanged.| Latitude| Longitude| Altitude (ground level)|  </summary>
        VTOL_LAND=85, 
    	///<summary> hand control over to an external controller |On / Off (> 0.5f on)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        GUIDED_ENABLE=92, 
    	///<summary> Delay the next navigation command a number of seconds or until a specified time |Delay in seconds (decimal, -1 to enable time-of-day fields)| hour (24h format, UTC, -1 to ignore)| minute (24h format, UTC, -1 to ignore)| second (24h format, UTC)| Empty| Empty| Empty|  </summary>
        DELAY=93, 
    	///<summary> Descend and place payload.  Vehicle descends until it detects a hanging payload has reached the ground, the gripper is opened to release the payload |Maximum distance to descend (meters)| Empty| Empty| Empty| Latitude (deg * 1E7)| Longitude (deg * 1E7)| Altitude (meters)|  </summary>
        PAYLOAD_PLACE=94, 
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
    	///<summary> Set system mode. |Mode, as defined by ENUM MAV_MODE| Custom mode - this is system specific, please refer to the individual autopilot specifications for details.| Custom sub mode - this is system specific, please refer to the individual autopilot specifications for details.| Empty| Empty| Empty| Empty|  </summary>
        DO_SET_MODE=176, 
    	///<summary> Jump to the desired command in the mission list.  Repeat this action only the specified number of times |Sequence number| Repeat count| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_JUMP=177, 
    	///<summary> Change speed and/or throttle set points. |Speed type (0=Airspeed, 1=Ground Speed)| Speed  (m/s, -1 indicates no change)| Throttle  ( Percent, -1 indicates no change)| absolute or relative [0,1]| Empty| Empty| Empty|  </summary>
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
    	///<summary> Change altitude set point. |Altitude in meters| Mav frame of new altitude (see MAV_FRAME)| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_CHANGE_ALTITUDE=186, 
    	///<summary> Mission command to perform a landing. This is used as a marker in a mission to tell the autopilot where a sequence of mission items that represents a landing starts. It may also be sent via a COMMAND_LONG to trigger a landing, in which case the nearest (geographically) landing sequence in the mission will be used. The Latitude/Longitude is optional, and may be set to 0 if not needed. If specified then it will be used to help find the closest landing sequence. |Empty| Empty| Empty| Empty| Latitude| Longitude| Empty|  </summary>
        DO_LAND_START=189, 
    	///<summary> Mission command to perform a landing from a rally point. |Break altitude (meters)| Landing speed (m/s)| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_RALLY_LAND=190, 
    	///<summary> Mission command to safely abort an autonmous landing. |Altitude (meters)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_GO_AROUND=191, 
    	///<summary> Reposition the vehicle to a specific WGS84 global position. |Ground speed, less than 0 (-1) for default| Bitmask of option flags, see the MAV_DO_REPOSITION_FLAGS enum.| Reserved| Yaw heading, NaN for unchanged. For planes indicates loiter direction (0: clockwise, 1: counter clockwise)| Latitude (deg * 1E7)| Longitude (deg * 1E7)| Altitude (meters)|  </summary>
        DO_REPOSITION=192, 
    	///<summary> If in a GPS controlled position mode, hold the current position or continue. |0: Pause current mission or reposition command, hold current position. 1: Continue mission. A VTOL capable vehicle should enter hover mode (multicopter and VTOL planes). A plane should loiter with the default loiter radius.| Reserved| Reserved| Reserved| Reserved| Reserved| Reserved|  </summary>
        DO_PAUSE_CONTINUE=193, 
    	///<summary> Set moving direction to forward or reverse. |Direction (0=Forward, 1=Reverse)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_SET_REVERSE=194, 
    	///<summary> Sets the region of interest (ROI) to a location. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Empty| Empty| Empty| Empty| Latitude| Longitude| Altitude|  </summary>
        DO_SET_ROI_LOCATION=195, 
    	///<summary> Sets the region of interest (ROI) to be toward next waypoint, with optional pitch/roll/yaw offset. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Empty| Empty| Empty| Empty| pitch offset from next waypoint| roll offset from next waypoint| yaw offset from next waypoint|  </summary>
        DO_SET_ROI_WPNEXT_OFFSET=196, 
    	///<summary> Cancels any previous ROI command returning the vehicle/sensors to default flight characteristics. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_SET_ROI_NONE=197, 
    	///<summary> Control onboard camera system. |Camera ID (-1 for all)| Transmission: 0: disabled, 1: enabled compressed, 2: enabled raw| Transmission mode: 0: video stream, >0: single images every n seconds (decimal)| Recording: 0: disabled, 1: enabled compressed, 2: enabled raw| Empty| Empty| Empty|  </summary>
        DO_CONTROL_VIDEO=200, 
    	///<summary> THIS INTERFACE IS DEPRECATED AS OF JANUARY 2018. Please use MAV_CMD_DO_SET_ROI_* messages instead. Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Region of intereset mode. (see MAV_ROI enum)| Waypoint index/ target ID. (see MAV_ROI enum)| ROI index (allows a vehicle to manage multiple ROI's)| Empty| x the location of the fixed ROI (see MAV_FRAME)| y| z|  </summary>
        DO_SET_ROI=201, 
    	///<summary> Mission command to configure an on-board camera controller system. |Modes: P, TV, AV, M, Etc| Shutter speed: Divisor number for one second| Aperture: F stop number| ISO number e.g. 80, 100, 200, Etc| Exposure type enumerator| Command Identity| Main engine cut-off time before camera trigger in seconds/10 (0 means no cut-off)|  </summary>
        DO_DIGICAM_CONFIGURE=202, 
    	///<summary> Mission command to control an on-board camera controller system. |Session control e.g. show/hide lens| Zoom's absolute position| Zooming step value to offset zoom from the current position| Focus Locking, Unlocking or Re-locking| Shooting Command| Command Identity| Test shot identifier. If set to 1, image will only be captured, but not counted towards internal frame count.|  </summary>
        DO_DIGICAM_CONTROL=203, 
    	///<summary> Mission command to configure a camera or antenna mount |Mount operation mode (see MAV_MOUNT_MODE enum)| stabilize roll? (1 = yes, 0 = no)| stabilize pitch? (1 = yes, 0 = no)| stabilize yaw? (1 = yes, 0 = no)| Empty| Empty| Empty|  </summary>
        DO_MOUNT_CONFIGURE=204, 
    	///<summary> Mission command to control a camera or antenna mount |pitch (WIP: DEPRECATED: or lat in degrees) depending on mount mode.| roll (WIP: DEPRECATED: or lon in degrees) depending on mount mode.| yaw (WIP: DEPRECATED: or alt in meters) depending on mount mode.| WIP: alt in meters depending on mount mode.| WIP: latitude in degrees * 1E7, set if appropriate mount mode.| WIP: longitude in degrees * 1E7, set if appropriate mount mode.| MAV_MOUNT_MODE enum value|  </summary>
        DO_MOUNT_CONTROL=205, 
    	///<summary> Mission command to set camera trigger distance for this flight. The camera is trigerred each time this distance is exceeded. This command can also be used to set the shutter integration time for the camera. |Camera trigger distance (meters). 0 to stop triggering.| Camera shutter integration time (milliseconds). -1 or 0 to ignore| Trigger camera once immediately. (0 = no trigger, 1 = trigger)| Empty| Empty| Empty| Empty|  </summary>
        DO_SET_CAM_TRIGG_DIST=206, 
    	///<summary> Mission command to enable the geofence |enable? (0=disable, 1=enable, 2=disable_floor_only)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_FENCE_ENABLE=207, 
    	///<summary> Mission command to trigger a parachute |action (0=disable, 1=enable, 2=release, for some systems see PARACHUTE_ACTION enum, not in general message set.)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_PARACHUTE=208, 
    	///<summary> Mission command to perform motor test |motor number (a number from 1 to max number of motors on the vehicle)| throttle type (0=throttle percentage, 1=PWM, 2=pilot throttle channel pass-through. See MOTOR_TEST_THROTTLE_TYPE enum)| throttle| timeout (in seconds)| motor count (number of motors to test to test in sequence, waiting for the timeout above between them; 0=1 motor, 1=1 motor, 2=2 motors...)| motor test order (See MOTOR_TEST_ORDER enum)| Empty|  </summary>
        DO_MOTOR_TEST=209, 
    	///<summary> Change to/from inverted flight |inverted (0=normal, 1=inverted)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_INVERTED_FLIGHT=210, 
    	///<summary> Mission command to operate EPM gripper |gripper number (a number from 1 to max number of grippers on the vehicle)| gripper action (0=release, 1=grab. See GRIPPER_ACTIONS enum)| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_GRIPPER=211, 
    	///<summary> Enable/disable autotune |enable (1: enable, 0:disable)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_AUTOTUNE_ENABLE=212, 
    	///<summary> Sets a desired vehicle turn angle and speed change |yaw angle to adjust steering by in centidegress| speed - normalized to 0 .. 1| Empty| Empty| Empty| Empty| Empty|  </summary>
        SET_YAW_SPEED=213, 
    	///<summary> Mission command to set camera trigger interval for this flight. If triggering is enabled, the camera is triggered each time this interval expires. This command can also be used to set the shutter integration time for the camera. |Camera trigger cycle time (milliseconds). -1 or 0 to ignore.| Camera shutter integration time (milliseconds). Should be less than trigger cycle time. -1 or 0 to ignore.| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_SET_CAM_TRIGG_INTERVAL=214, 
    	///<summary> Mission command to control a camera or antenna mount, using a quaternion as reference. |q1 - quaternion param #1, w (1 in null-rotation)| q2 - quaternion param #2, x (0 in null-rotation)| q3 - quaternion param #3, y (0 in null-rotation)| q4 - quaternion param #4, z (0 in null-rotation)| Empty| Empty| Empty|  </summary>
        DO_MOUNT_CONTROL_QUAT=220, 
    	///<summary> set id of master controller |System ID| Component ID| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_GUIDED_MASTER=221, 
    	///<summary> set limits for external control |timeout - maximum time (in seconds) that external controller will be allowed to control vehicle. 0 means no timeout| absolute altitude min (in meters, AMSL) - if vehicle moves below this alt, the command will be aborted and the mission will continue.  0 means no lower altitude limit| absolute altitude max (in meters)- if vehicle moves above this alt, the command will be aborted and the mission will continue.  0 means no upper altitude limit| horizontal move limit (in meters, AMSL) - if vehicle moves more than this distance from it's location at the moment the command was executed, the command will be aborted and the mission will continue. 0 means no horizontal altitude limit| Empty| Empty| Empty|  </summary>
        DO_GUIDED_LIMITS=222, 
    	///<summary> Control vehicle engine. This is interpreted by the vehicles engine controller to change the target engine state. It is intended for vehicles with internal combustion engines |0: Stop engine, 1:Start Engine| 0: Warm start, 1:Cold start. Controls use of choke where applicable| Height delay (meters). This is for commanding engine start only after the vehicle has gained the specified height. Used in VTOL vehicles during takeoff to start engine after the aircraft is off the ground. Zero for no delay.| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_ENGINE_CONTROL=223, 
    	///<summary> NOP - This command is only used to mark the upper limit of the DO commands in the enumeration |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_LAST=240, 
    	///<summary> Trigger calibration. This command will be only accepted if in pre-flight mode. Except for Temperature Calibration, only one sensor should be set in a single message and all others should be zero. |1: gyro calibration, 3: gyro temperature calibration| 1: magnetometer calibration| 1: ground pressure calibration| 1: radio RC calibration, 2: RC trim calibration| 1: accelerometer calibration, 2: board level calibration, 3: accelerometer temperature calibration, 4: simple accelerometer calibration| 1: APM: compass/motor interference calibration (PX4: airspeed calibration, deprecated), 2: airspeed calibration| 1: ESC calibration, 3: barometer temperature calibration|  </summary>
        PREFLIGHT_CALIBRATION=241, 
    	///<summary> Set sensor offsets. This command will be only accepted if in pre-flight mode. |Sensor to adjust the offsets for: 0: gyros, 1: accelerometer, 2: magnetometer, 3: barometer, 4: optical flow, 5: second magnetometer, 6: third magnetometer| X axis offset (or generic dimension 1), in the sensor's raw units| Y axis offset (or generic dimension 2), in the sensor's raw units| Z axis offset (or generic dimension 3), in the sensor's raw units| Generic dimension 4, in the sensor's raw units| Generic dimension 5, in the sensor's raw units| Generic dimension 6, in the sensor's raw units|  </summary>
        PREFLIGHT_SET_SENSOR_OFFSETS=242, 
    	///<summary> Trigger UAVCAN config. This command will be only accepted if in pre-flight mode. |1: Trigger actuator ID assignment and direction mapping.| Reserved| Reserved| Reserved| Reserved| Reserved| Reserved|  </summary>
        PREFLIGHT_UAVCAN=243, 
    	///<summary> Request storage of different parameter values and logs. This command will be only accepted if in pre-flight mode. |Parameter storage: 0: READ FROM FLASH/EEPROM, 1: WRITE CURRENT TO FLASH/EEPROM, 2: Reset to defaults| Mission storage: 0: READ FROM FLASH/EEPROM, 1: WRITE CURRENT TO FLASH/EEPROM, 2: Reset to defaults| Onboard logging: 0: Ignore, 1: Start default rate logging, -1: Stop logging, > 1: start logging with rate of param 3 in Hz (e.g. set to 1000 for 1000 Hz logging)| Reserved| Empty| Empty| Empty|  </summary>
        PREFLIGHT_STORAGE=245, 
    	///<summary> Request the reboot or shutdown of system components. |0: Do nothing for autopilot, 1: Reboot autopilot, 2: Shutdown autopilot, 3: Reboot autopilot and keep it in the bootloader until upgraded.| 0: Do nothing for onboard computer, 1: Reboot onboard computer, 2: Shutdown onboard computer, 3: Reboot onboard computer and keep it in the bootloader until upgraded.| WIP: 0: Do nothing for camera, 1: Reboot onboard camera, 2: Shutdown onboard camera, 3: Reboot onboard camera and keep it in the bootloader until upgraded| WIP: 0: Do nothing for mount (e.g. gimbal), 1: Reboot mount, 2: Shutdown mount, 3: Reboot mount and keep it in the bootloader until upgraded| Reserved, send 0| Reserved, send 0| WIP: ID (e.g. camera ID -1 for all IDs)|  </summary>
        PREFLIGHT_REBOOT_SHUTDOWN=246, 
    	///<summary> Hold / continue the current action |MAV_GOTO_DO_HOLD: hold MAV_GOTO_DO_CONTINUE: continue with next item in mission plan| MAV_GOTO_HOLD_AT_CURRENT_POSITION: Hold at current position MAV_GOTO_HOLD_AT_SPECIFIED_POSITION: hold at specified position| MAV_FRAME coordinate frame of hold point| Desired yaw angle in degrees| Latitude / X position| Longitude / Y position| Altitude / Z position|  </summary>
        OVERRIDE_GOTO=252, 
    	///<summary> start running a mission |first_item: the first mission item to run| last_item:  the last mission item to run (after this item is run, the mission ends)|  </summary>
        MISSION_START=300, 
    	///<summary> Arms / Disarms a component |1 to arm, 0 to disarm|  </summary>
        COMPONENT_ARM_DISARM=400, 
    	///<summary> Request the home position from the vehicle. |Reserved| Reserved| Reserved| Reserved| Reserved| Reserved| Reserved|  </summary>
        GET_HOME_POSITION=410, 
    	///<summary> Starts receiver pairing |0:Spektrum| RC type (see RC_TYPE enum)|  </summary>
        START_RX_PAIR=500, 
    	///<summary> Request the interval between messages for a particular MAVLink message ID |The MAVLink message ID|  </summary>
        GET_MESSAGE_INTERVAL=510, 
    	///<summary> Request the interval between messages for a particular MAVLink message ID. This interface replaces REQUEST_DATA_STREAM |The MAVLink message ID| The interval between two messages, in microseconds. Set to -1 to disable and 0 to request default rate.|  </summary>
        SET_MESSAGE_INTERVAL=511, 
    	///<summary> Request autopilot capabilities |1: Request autopilot version| Reserved (all remaining params)|  </summary>
        REQUEST_AUTOPILOT_CAPABILITIES=520, 
    	///<summary> WIP: Request camera information (CAMERA_INFORMATION) |1: Request camera capabilities| Camera ID| Reserved (all remaining params)|  </summary>
        REQUEST_CAMERA_INFORMATION=521, 
    	///<summary> WIP: Request camera settings (CAMERA_SETTINGS) |1: Request camera settings| Camera ID| Reserved (all remaining params)|  </summary>
        REQUEST_CAMERA_SETTINGS=522, 
    	///<summary> WIP: Set the camera settings part 1 (CAMERA_SETTINGS) |Camera ID| Aperture (1/value)| Aperture locked (0: auto, 1: locked)| Shutter speed in s| Shutter speed locked (0: auto, 1: locked)| ISO sensitivity| ISO sensitivity locked (0: auto, 1: locked)|  </summary>
        SET_CAMERA_SETTINGS_1=523, 
    	///<summary> WIP: Set the camera settings part 2 (CAMERA_SETTINGS) |Camera ID| White balance locked (0: auto, 1: locked)| White balance (color temperature in K)| Reserved for camera mode ID| Reserved for color mode ID| Reserved for image format ID| Reserved|  </summary>
        SET_CAMERA_SETTINGS_2=524, 
    	///<summary> Request storage information (STORAGE_INFORMATION) |1: Request storage information| Storage ID| Reserved (all remaining params)|  </summary>
        REQUEST_STORAGE_INFORMATION=525, 
    	///<summary> Format a storage medium |1: Format storage| Storage ID| Reserved (all remaining params)|  </summary>
        STORAGE_FORMAT=526, 
    	///<summary> Request camera capture status (CAMERA_CAPTURE_STATUS) |1: Request camera capture status| Camera ID| Reserved (all remaining params)|  </summary>
        REQUEST_CAMERA_CAPTURE_STATUS=527, 
    	///<summary> Request flight information (FLIGHT_INFORMATION) |1: Request flight information| Reserved (all remaining params)|  </summary>
        REQUEST_FLIGHT_INFORMATION=528, 
    	///<summary> Set camera running mode. Use NAN for reserved values. |Reserved (Set to 0)| Camera mode (see CAMERA_MODE enum)| Reserved (all remaining params)|  </summary>
        SET_CAMERA_MODE=530, 
    	///<summary> Start image capture sequence. Sends CAMERA_IMAGE_CAPTURED after each capture. Use NAN for reserved values. |Reserved (Set to 0)| Duration between two consecutive pictures (in seconds)| Number of images to capture total - 0 for unlimited capture| Capture sequence (ID to prevent double captures when a command is retransmitted, 0: unused, >= 1: used)| Reserved (all remaining params)|  </summary>
        IMAGE_START_CAPTURE=2000, 
    	///<summary> Stop image capture sequence |Camera ID| Reserved|  </summary>
        IMAGE_STOP_CAPTURE=2001, 
    	///<summary> Enable or disable on-board camera triggering system. |Trigger enable/disable (0 for disable, 1 for start), -1 to ignore| 1 to reset the trigger sequence, -1 or 0 to ignore| 1 to pause triggering, but without switching the camera off or retracting it. -1 to ignore|  </summary>
        DO_TRIGGER_CONTROL=2003, 
    	///<summary> Starts video capture (recording) |Camera ID (0 for all cameras), 1 for first, 2 for second, etc.| Frames per second, set to -1 for highest framerate possible.| Resolution in megapixels (0.3 for 640x480, 1.3 for 1280x720, etc), set to 0 if param 4/5 are used, set to -1 for highest resolution possible.| WIP: Resolution horizontal in pixels| WIP: Resolution horizontal in pixels| WIP: Frequency CAMERA_CAPTURE_STATUS messages should be sent while recording (0 for no messages, otherwise time in Hz)|  </summary>
        VIDEO_START_CAPTURE=2500, 
    	///<summary> Stop the current video capture (recording) |WIP: Camera ID| Reserved|  </summary>
        VIDEO_STOP_CAPTURE=2501, 
    	///<summary> Request to start streaming logging data over MAVLink (see also LOGGING_DATA message) |Format: 0: ULog| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)|  </summary>
        LOGGING_START=2510, 
    	///<summary> Request to stop streaming log data over MAVLink |Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)|  </summary>
        LOGGING_STOP=2511, 
    	///<summary>  |Landing gear ID (default: 0, -1 for all)| Landing gear position (Down: 0, Up: 1, NAN for no change)| Reserved, set to NAN| Reserved, set to NAN| Reserved, set to NAN| Reserved, set to NAN| Reserved, set to NAN|  </summary>
        AIRFRAME_CONFIGURATION=2520, 
    	///<summary> Request to start/stop transmitting over the high latency telemetry |Control transmittion over high latency telemetry (0: stop, 1: start)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        CONTROL_HIGH_LATENCY=2600, 
    	///<summary> Create a panorama at the current position |Viewing angle horizontal of the panorama (in degrees, +- 0.5 the total angle)| Viewing angle vertical of panorama (in degrees)| Speed of the horizontal rotation (in degrees per second)| Speed of the vertical rotation (in degrees per second)|  </summary>
        PANORAMA_CREATE=2800, 
    	///<summary> Request VTOL transition |The target VTOL state, as defined by ENUM MAV_VTOL_STATE. Only MAV_VTOL_STATE_MC and MAV_VTOL_STATE_FW can be used.|  </summary>
        DO_VTOL_TRANSITION=3000, 
    	///<summary> Request authorization to arm the vehicle to a external entity, the arm authorizer is resposible to request all data that is needs from the vehicle before authorize or deny the request. If approved the progress of command_ack message should be set with period of time that this authorization is valid in seconds or in case it was denied it should be set with one of the reasons in ARM_AUTH_DENIED_REASON.          |Vehicle system id, this way ground station can request arm authorization on behalf of any vehicle|  </summary>
        ARM_AUTHORIZATION_REQUEST=3001, 
    	///<summary> This command sets the submode to standard guided when vehicle is in guided mode. The vehicle holds position and altitude and the user can input the desired velocites along all three axes.                    | </summary>
        SET_GUIDED_SUBMODE_STANDARD=4000, 
    	///<summary> This command sets submode circle when vehicle is in guided mode. Vehicle flies along a circle facing the center of the circle. The user can input the velocity along the circle and change the radius. If no input is given the vehicle will hold position.                    |Radius of desired circle in CIRCLE_MODE| User defined| User defined| User defined| Unscaled target latitude of center of circle in CIRCLE_MODE| Unscaled target longitude of center of circle in CIRCLE_MODE|  </summary>
        SET_GUIDED_SUBMODE_CIRCLE=4001, 
    	///<summary> Fence return point. There can only be one fence return point.          |Reserved| Reserved| Reserved| Reserved| Latitude| Longitude| Altitude|  </summary>
        FENCE_RETURN_POINT=5000, 
    	///<summary> Fence vertex for an inclusion polygon (the polygon must not be self-intersecting). The vehicle must stay within this area. Minimum of 3 vertices required.          |Polygon vertex count| Reserved| Reserved| Reserved| Latitude| Longitude| Reserved|  </summary>
        FENCE_POLYGON_VERTEX_INCLUSION=5001, 
    	///<summary> Fence vertex for an exclusion polygon (the polygon must not be self-intersecting). The vehicle must stay outside this area. Minimum of 3 vertices required.          |Polygon vertex count| Reserved| Reserved| Reserved| Latitude| Longitude| Reserved|  </summary>
        FENCE_POLYGON_VERTEX_EXCLUSION=5002, 
    	///<summary> Circular fence area. The vehicle must stay inside this area.          |radius in meters| Reserved| Reserved| Reserved| Latitude| Longitude| Reserved|  </summary>
        FENCE_CIRCLE_INCLUSION=5003, 
    	///<summary> Circular fence area. The vehicle must stay outside this area.          |radius in meters| Reserved| Reserved| Reserved| Latitude| Longitude| Reserved|  </summary>
        FENCE_CIRCLE_EXCLUSION=5004, 
    	///<summary> Rally point. You can have multiple rally points defined.          |Reserved| Reserved| Reserved| Reserved| Latitude| Longitude| Altitude|  </summary>
        RALLY_POINT=5100, 
    	///<summary> Commands the vehicle to respond with a sequence of messages UAVCAN_NODE_INFO, one message per every UAVCAN node that is online. Note that some of the response messages can be lost, which the receiver can detect easily by checking whether every received UAVCAN_NODE_STATUS has a matching message UAVCAN_NODE_INFO received earlier; if not, this command should be sent again in order to request re-transmission of the node information messages. |Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)|  </summary>
        UAVCAN_GET_NODE_INFO=5200, 
    	///<summary> Deploy payload on a Lat / Lon / Alt position. This includes the navigation to reach the required release position and velocity. |Operation mode. 0: prepare single payload deploy (overwriting previous requests), but do not execute it. 1: execute payload deploy immediately (rejecting further deploy commands during execution, but allowing abort). 2: add payload deploy to existing deployment list.| Desired approach vector in degrees compass heading (0..360). A negative value indicates the system can define the approach vector at will.| Desired ground speed at release time. This can be overriden by the airframe in case it needs to meet minimum airspeed. A negative value indicates the system can define the ground speed at will.| Minimum altitude clearance to the release position in meters. A negative value indicates the system can define the clearance at will.| Latitude unscaled for MISSION_ITEM or in 1e7 degrees for MISSION_ITEM_INT| Longitude unscaled for MISSION_ITEM or in 1e7 degrees for MISSION_ITEM_INT| Altitude, in meters AMSL|  </summary>
        PAYLOAD_PREPARE_DEPLOY=30001, 
    	///<summary> Control the payload deployment. |Operation mode. 0: Abort deployment, continue normal mission. 1: switch to payload deploment mode. 100: delete first payload deployment request. 101: delete all payload deployment requests.| Reserved| Reserved| Reserved| Reserved| Reserved| Reserved|  </summary>
        PAYLOAD_CONTROL_DEPLOY=30002, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        WAYPOINT_USER_1=31000, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        WAYPOINT_USER_2=31001, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        WAYPOINT_USER_3=31002, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        WAYPOINT_USER_4=31003, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        WAYPOINT_USER_5=31004, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        SPATIAL_USER_1=31005, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        SPATIAL_USER_2=31006, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        SPATIAL_USER_3=31007, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        SPATIAL_USER_4=31008, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude, in meters AMSL|  </summary>
        SPATIAL_USER_5=31009, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        USER_1=31010, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        USER_2=31011, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        USER_3=31012, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        USER_4=31013, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        USER_5=31014, 
    	///<summary> A system wide power-off event has been initiated. |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        POWER_OFF_INITIATED=42000, 
    	///<summary> FLY button has been clicked. |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        SOLO_BTN_FLY_CLICK=42001, 
    	///<summary> FLY button has been held for 1.5 seconds. |Takeoff altitude| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        SOLO_BTN_FLY_HOLD=42002, 
    	///<summary> PAUSE button has been clicked. |1 if Solo is in a shot mode, 0 otherwise| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        SOLO_BTN_PAUSE_CLICK=42003, 
    	///<summary> Magnetometer calibration based on fixed position         in earth field given by inclination, declination and intensity |MagDeclinationDegrees| MagInclinationDegrees| MagIntensityMilliGauss| YawDegrees| Empty| Empty| Empty|  </summary>
        FIXED_MAG_CAL=42004, 
    	///<summary> Magnetometer calibration based on fixed expected field values in milliGauss |FieldX| FieldY| FieldZ| Empty| Empty| Empty| Empty|  </summary>
        FIXED_MAG_CAL_FIELD=42005, 
    	///<summary> Initiate a magnetometer calibration |uint8_t bitmask of magnetometers (0 means all)| Automatically retry on failure (0=no retry, 1=retry).| Save without user input (0=require input, 1=autosave).| Delay (seconds)| Autoreboot (0=user reboot, 1=autoreboot)| Empty| Empty|  </summary>
        DO_START_MAG_CAL=42424, 
    	///<summary> Initiate a magnetometer calibration |uint8_t bitmask of magnetometers (0 means all)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_ACCEPT_MAG_CAL=42425, 
    	///<summary> Cancel a running magnetometer calibration |uint8_t bitmask of magnetometers (0 means all)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_CANCEL_MAG_CAL=42426, 
    	///<summary> Command autopilot to get into factory test/diagnostic mode |0 means get out of test mode, 1 means get into test mode| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        SET_FACTORY_TEST_MODE=42427, 
    	///<summary> Reply with the version banner |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        DO_SEND_BANNER=42428, 
    	///<summary> Used when doing accelerometer calibration. When sent to the GCS tells it what position to put the vehicle in. When sent to the vehicle says what position the vehicle is in. |Position, one of the ACCELCAL_VEHICLE_POS enum values| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        ACCELCAL_VEHICLE_POS=42429, 
    	///<summary> Causes the gimbal to reset and boot as if it was just powered on |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        GIMBAL_RESET=42501, 
    	///<summary> Reports progress and success or failure of gimbal axis calibration procedure |Gimbal axis we're reporting calibration progress for| Current calibration progress for this axis, 0x64=100%| Status of the calibration| Empty| Empty| Empty| Empty|  </summary>
        GIMBAL_AXIS_CALIBRATION_STATUS=42502, 
    	///<summary> Starts commutation calibration on the gimbal |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        GIMBAL_REQUEST_AXIS_CALIBRATION=42503, 
    	///<summary> Erases gimbal application and parameters |Magic number| Magic number| Magic number| Magic number| Magic number| Magic number| Magic number|  </summary>
        GIMBAL_FULL_RESET=42505, 
    	///<summary> Command to operate winch |winch number (0 for the default winch, otherwise a number from 1 to max number of winches on the vehicle)| action (0=relax, 1=relative length control, 2=rate control.  See WINCH_ACTIONS enum)| release length (cable distance to unwind in meters, negative numbers to wind in cable)| release rate (meters/second)| Empty| Empty| Empty|  </summary>
        DO_WINCH=42600, 
    	///<summary> Update the bootloader |Empty| Empty| Empty| Empty| Magic number - set to 290876 to actually flash| Empty| Empty|  </summary>
        FLASH_BOOTLOADER=42650, 
    
    };
    
    ///<summary>  </summary>
    public enum LIMITS_STATE: byte
    {
			///<summary> pre-initialization | </summary>
        LIMITS_INIT=0, 
    	///<summary> disabled | </summary>
        LIMITS_DISABLED=1, 
    	///<summary> checking limits | </summary>
        LIMITS_ENABLED=2, 
    	///<summary> a limit has been breached | </summary>
        LIMITS_TRIGGERED=3, 
    	///<summary> taking action eg. RTL | </summary>
        LIMITS_RECOVERING=4, 
    	///<summary> we're no longer in breach of a limit | </summary>
        LIMITS_RECOVERED=5, 
    
    };
    
    ///<summary>  </summary>
    public enum LIMIT_MODULE: byte
    {
			///<summary> pre-initialization | </summary>
        LIMIT_GPSLOCK=1, 
    	///<summary> disabled | </summary>
        LIMIT_GEOFENCE=2, 
    	///<summary> checking limits | </summary>
        LIMIT_ALTITUDE=4, 
    
    };
    
    ///<summary> Flags in RALLY_POINT message </summary>
    public enum RALLY_FLAGS: byte
    {
			///<summary> Flag set when requiring favorable winds for landing. | </summary>
        FAVORABLE_WIND=1, 
    	///<summary> Flag set when plane is to immediately descend to break altitude and land without GCS intervention. Flag not set when plane is to loiter at Rally point until commanded to land. | </summary>
        LAND_IMMEDIATELY=2, 
    
    };
    
    ///<summary>  </summary>
    public enum PARACHUTE_ACTION: int /*default*/
    {
			///<summary> Disable parachute release | </summary>
        PARACHUTE_DISABLE=0, 
    	///<summary> Enable parachute release | </summary>
        PARACHUTE_ENABLE=1, 
    	///<summary> Release parachute | </summary>
        PARACHUTE_RELEASE=2, 
    
    };
    
    ///<summary> Gripper actions. </summary>
    public enum GRIPPER_ACTIONS: int /*default*/
    {
			///<summary> gripper release of cargo | </summary>
        GRIPPER_ACTION_RELEASE=0, 
    	///<summary> gripper grabs onto cargo | </summary>
        GRIPPER_ACTION_GRAB=1, 
    
    };
    
    ///<summary> Winch actions </summary>
    public enum WINCH_ACTIONS: int /*default*/
    {
			///<summary> relax winch | </summary>
        WINCH_RELAXED=0, 
    	///<summary> winch unwinds or winds specified length of cable optionally using specified rate | </summary>
        WINCH_RELATIVE_LENGTH_CONTROL=1, 
    	///<summary> winch unwinds or winds cable at specified rate in meters/seconds | </summary>
        WINCH_RATE_CONTROL=2, 
    
    };
    
    ///<summary>  </summary>
    public enum CAMERA_STATUS_TYPES: byte
    {
			///<summary> Camera heartbeat, announce camera component ID at 1hz | </summary>
        CAMERA_STATUS_TYPE_HEARTBEAT=0, 
    	///<summary> Camera image triggered | </summary>
        CAMERA_STATUS_TYPE_TRIGGER=1, 
    	///<summary> Camera connection lost | </summary>
        CAMERA_STATUS_TYPE_DISCONNECT=2, 
    	///<summary> Camera unknown error | </summary>
        CAMERA_STATUS_TYPE_ERROR=3, 
    	///<summary> Camera battery low. Parameter p1 shows reported voltage | </summary>
        CAMERA_STATUS_TYPE_LOWBATT=4, 
    	///<summary> Camera storage low. Parameter p1 shows reported shots remaining | </summary>
        CAMERA_STATUS_TYPE_LOWSTORE=5, 
    	///<summary> Camera storage low. Parameter p1 shows reported video minutes remaining | </summary>
        CAMERA_STATUS_TYPE_LOWSTOREV=6, 
    
    };
    
    ///<summary>  </summary>
    public enum CAMERA_FEEDBACK_FLAGS: byte
    {
			///<summary> Shooting photos, not video | </summary>
        CAMERA_FEEDBACK_PHOTO=0, 
    	///<summary> Shooting video, not stills | </summary>
        CAMERA_FEEDBACK_VIDEO=1, 
    	///<summary> Unable to achieve requested exposure (e.g. shutter speed too low) | </summary>
        CAMERA_FEEDBACK_BADEXPOSURE=2, 
    	///<summary> Closed loop feedback from camera, we know for sure it has successfully taken a picture | </summary>
        CAMERA_FEEDBACK_CLOSEDLOOP=3, 
    	///<summary> Open loop camera, an image trigger has been requested but we can't know for sure it has successfully taken a picture | </summary>
        CAMERA_FEEDBACK_OPENLOOP=4, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_MODE_GIMBAL: int /*default*/
    {
			///<summary> Gimbal is powered on but has not started initializing yet | </summary>
        UNINITIALIZED=0, 
    	///<summary> Gimbal is currently running calibration on the pitch axis | </summary>
        CALIBRATING_PITCH=1, 
    	///<summary> Gimbal is currently running calibration on the roll axis | </summary>
        CALIBRATING_ROLL=2, 
    	///<summary> Gimbal is currently running calibration on the yaw axis | </summary>
        CALIBRATING_YAW=3, 
    	///<summary> Gimbal has finished calibrating and initializing, but is relaxed pending reception of first rate command from copter | </summary>
        INITIALIZED=4, 
    	///<summary> Gimbal is actively stabilizing | </summary>
        ACTIVE=5, 
    	///<summary> Gimbal is relaxed because it missed more than 10 expected rate command messages in a row. Gimbal will move back to active mode when it receives a new rate command | </summary>
        RATE_CMD_TIMEOUT=6, 
    
    };
    
    ///<summary>  </summary>
    public enum GIMBAL_AXIS: int /*default*/
    {
			///<summary> Gimbal yaw axis | </summary>
        YAW=0, 
    	///<summary> Gimbal pitch axis | </summary>
        PITCH=1, 
    	///<summary> Gimbal roll axis | </summary>
        ROLL=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GIMBAL_AXIS_CALIBRATION_STATUS: int /*default*/
    {
			///<summary> Axis calibration is in progress | </summary>
        IN_PROGRESS=0, 
    	///<summary> Axis calibration succeeded | </summary>
        SUCCEEDED=1, 
    	///<summary> Axis calibration failed | </summary>
        FAILED=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GIMBAL_AXIS_CALIBRATION_REQUIRED: int /*default*/
    {
			///<summary> Whether or not this axis requires calibration is unknown at this time | </summary>
        UNKNOWN=0, 
    	///<summary> This axis requires calibration | </summary>
        TRUE=1, 
    	///<summary> This axis does not require calibration | </summary>
        FALSE=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_HEARTBEAT_STATUS: byte
    {
			///<summary> No GoPro connected | </summary>
        DISCONNECTED=0, 
    	///<summary> The detected GoPro is not HeroBus compatible | </summary>
        INCOMPATIBLE=1, 
    	///<summary> A HeroBus compatible GoPro is connected | </summary>
        CONNECTED=2, 
    	///<summary> An unrecoverable error was encountered with the connected GoPro, it may require a power cycle | </summary>
        ERROR=3, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_HEARTBEAT_FLAGS: byte
    {
			///<summary> GoPro is currently recording | </summary>
        GOPRO_FLAG_RECORDING=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_REQUEST_STATUS: byte
    {
			///<summary> The write message with ID indicated succeeded | </summary>
        GOPRO_REQUEST_SUCCESS=0, 
    	///<summary> The write message with ID indicated failed | </summary>
        GOPRO_REQUEST_FAILED=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_COMMAND: byte
    {
			///<summary> (Get/Set) | </summary>
        POWER=0, 
    	///<summary> (Get/Set) | </summary>
        CAPTURE_MODE=1, 
    	///<summary> (___/Set) | </summary>
        SHUTTER=2, 
    	///<summary> (Get/___) | </summary>
        BATTERY=3, 
    	///<summary> (Get/___) | </summary>
        MODEL=4, 
    	///<summary> (Get/Set) | </summary>
        VIDEO_SETTINGS=5, 
    	///<summary> (Get/Set) | </summary>
        LOW_LIGHT=6, 
    	///<summary> (Get/Set) | </summary>
        PHOTO_RESOLUTION=7, 
    	///<summary> (Get/Set) | </summary>
        PHOTO_BURST_RATE=8, 
    	///<summary> (Get/Set) | </summary>
        PROTUNE=9, 
    	///<summary> (Get/Set) Hero 3+ Only | </summary>
        PROTUNE_WHITE_BALANCE=10, 
    	///<summary> (Get/Set) Hero 3+ Only | </summary>
        PROTUNE_COLOUR=11, 
    	///<summary> (Get/Set) Hero 3+ Only | </summary>
        PROTUNE_GAIN=12, 
    	///<summary> (Get/Set) Hero 3+ Only | </summary>
        PROTUNE_SHARPNESS=13, 
    	///<summary> (Get/Set) Hero 3+ Only | </summary>
        PROTUNE_EXPOSURE=14, 
    	///<summary> (Get/Set) | </summary>
        TIME=15, 
    	///<summary> (Get/Set) | </summary>
        CHARGING=16, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_CAPTURE_MODE: byte
    {
			///<summary> Video mode | </summary>
        VIDEO=0, 
    	///<summary> Photo mode | </summary>
        PHOTO=1, 
    	///<summary> Burst mode, hero 3+ only | </summary>
        BURST=2, 
    	///<summary> Time lapse mode, hero 3+ only | </summary>
        TIME_LAPSE=3, 
    	///<summary> Multi shot mode, hero 4 only | </summary>
        MULTI_SHOT=4, 
    	///<summary> Playback mode, hero 4 only, silver only except when LCD or HDMI is connected to black | </summary>
        PLAYBACK=5, 
    	///<summary> Playback mode, hero 4 only | </summary>
        SETUP=6, 
    	///<summary> Mode not yet known | </summary>
        UNKNOWN=255, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_RESOLUTION: int /*default*/
    {
			///<summary> 848 x 480 (480p) | </summary>
        _480p=0, 
    	///<summary> 1280 x 720 (720p) | </summary>
        _720p=1, 
    	///<summary> 1280 x 960 (960p) | </summary>
        _960p=2, 
    	///<summary> 1920 x 1080 (1080p) | </summary>
        _1080p=3, 
    	///<summary> 1920 x 1440 (1440p) | </summary>
        _1440p=4, 
    	///<summary> 2704 x 1440 (2.7k-17:9) | </summary>
        _2_7k_17_9=5, 
    	///<summary> 2704 x 1524 (2.7k-16:9) | </summary>
        _2_7k_16_9=6, 
    	///<summary> 2704 x 2028 (2.7k-4:3) | </summary>
        _2_7k_4_3=7, 
    	///<summary> 3840 x 2160 (4k-16:9) | </summary>
        _4k_16_9=8, 
    	///<summary> 4096 x 2160 (4k-17:9) | </summary>
        _4k_17_9=9, 
    	///<summary> 1280 x 720 (720p-SuperView) | </summary>
        _720p_SUPERVIEW=10, 
    	///<summary> 1920 x 1080 (1080p-SuperView) | </summary>
        _1080p_SUPERVIEW=11, 
    	///<summary> 2704 x 1520 (2.7k-SuperView) | </summary>
        _2_7k_SUPERVIEW=12, 
    	///<summary> 3840 x 2160 (4k-SuperView) | </summary>
        _4k_SUPERVIEW=13, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_FRAME_RATE: int /*default*/
    {
			///<summary> 12 FPS | </summary>
        _12=0, 
    	///<summary> 15 FPS | </summary>
        _15=1, 
    	///<summary> 24 FPS | </summary>
        _24=2, 
    	///<summary> 25 FPS | </summary>
        _25=3, 
    	///<summary> 30 FPS | </summary>
        _30=4, 
    	///<summary> 48 FPS | </summary>
        _48=5, 
    	///<summary> 50 FPS | </summary>
        _50=6, 
    	///<summary> 60 FPS | </summary>
        _60=7, 
    	///<summary> 80 FPS | </summary>
        _80=8, 
    	///<summary> 90 FPS | </summary>
        _90=9, 
    	///<summary> 100 FPS | </summary>
        _100=10, 
    	///<summary> 120 FPS | </summary>
        _120=11, 
    	///<summary> 240 FPS | </summary>
        _240=12, 
    	///<summary> 12.5 FPS | </summary>
        _12_5=13, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_FIELD_OF_VIEW: int /*default*/
    {
			///<summary> 0x00: Wide | </summary>
        WIDE=0, 
    	///<summary> 0x01: Medium | </summary>
        MEDIUM=1, 
    	///<summary> 0x02: Narrow | </summary>
        NARROW=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_VIDEO_SETTINGS_FLAGS: int /*default*/
    {
			///<summary> 0=NTSC, 1=PAL | </summary>
        GOPRO_VIDEO_SETTINGS_TV_MODE=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PHOTO_RESOLUTION: int /*default*/
    {
			///<summary> 5MP Medium | </summary>
        _5MP_MEDIUM=0, 
    	///<summary> 7MP Medium | </summary>
        _7MP_MEDIUM=1, 
    	///<summary> 7MP Wide | </summary>
        _7MP_WIDE=2, 
    	///<summary> 10MP Wide | </summary>
        _10MP_WIDE=3, 
    	///<summary> 12MP Wide | </summary>
        _12MP_WIDE=4, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_WHITE_BALANCE: int /*default*/
    {
			///<summary> Auto | </summary>
        AUTO=0, 
    	///<summary> 3000K | </summary>
        _3000K=1, 
    	///<summary> 5500K | </summary>
        _5500K=2, 
    	///<summary> 6500K | </summary>
        _6500K=3, 
    	///<summary> Camera Raw | </summary>
        RAW=4, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_COLOUR: int /*default*/
    {
			///<summary> Auto | </summary>
        STANDARD=0, 
    	///<summary> Neutral | </summary>
        NEUTRAL=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_GAIN: int /*default*/
    {
			///<summary> ISO 400 | </summary>
        _400=0, 
    	///<summary> ISO 800 (Only Hero 4) | </summary>
        _800=1, 
    	///<summary> ISO 1600 | </summary>
        _1600=2, 
    	///<summary> ISO 3200 (Only Hero 4) | </summary>
        _3200=3, 
    	///<summary> ISO 6400 | </summary>
        _6400=4, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_SHARPNESS: int /*default*/
    {
			///<summary> Low Sharpness | </summary>
        LOW=0, 
    	///<summary> Medium Sharpness | </summary>
        MEDIUM=1, 
    	///<summary> High Sharpness | </summary>
        HIGH=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_EXPOSURE: int /*default*/
    {
			///<summary> -5.0 EV (Hero 3+ Only) | </summary>
        NEG_5_0=0, 
    	///<summary> -4.5 EV (Hero 3+ Only) | </summary>
        NEG_4_5=1, 
    	///<summary> -4.0 EV (Hero 3+ Only) | </summary>
        NEG_4_0=2, 
    	///<summary> -3.5 EV (Hero 3+ Only) | </summary>
        NEG_3_5=3, 
    	///<summary> -3.0 EV (Hero 3+ Only) | </summary>
        NEG_3_0=4, 
    	///<summary> -2.5 EV (Hero 3+ Only) | </summary>
        NEG_2_5=5, 
    	///<summary> -2.0 EV | </summary>
        NEG_2_0=6, 
    	///<summary> -1.5 EV | </summary>
        NEG_1_5=7, 
    	///<summary> -1.0 EV | </summary>
        NEG_1_0=8, 
    	///<summary> -0.5 EV | </summary>
        NEG_0_5=9, 
    	///<summary> 0.0 EV | </summary>
        ZERO=10, 
    	///<summary> +0.5 EV | </summary>
        POS_0_5=11, 
    	///<summary> +1.0 EV | </summary>
        POS_1_0=12, 
    	///<summary> +1.5 EV | </summary>
        POS_1_5=13, 
    	///<summary> +2.0 EV | </summary>
        POS_2_0=14, 
    	///<summary> +2.5 EV (Hero 3+ Only) | </summary>
        POS_2_5=15, 
    	///<summary> +3.0 EV (Hero 3+ Only) | </summary>
        POS_3_0=16, 
    	///<summary> +3.5 EV (Hero 3+ Only) | </summary>
        POS_3_5=17, 
    	///<summary> +4.0 EV (Hero 3+ Only) | </summary>
        POS_4_0=18, 
    	///<summary> +4.5 EV (Hero 3+ Only) | </summary>
        POS_4_5=19, 
    	///<summary> +5.0 EV (Hero 3+ Only) | </summary>
        POS_5_0=20, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_CHARGING: int /*default*/
    {
			///<summary> Charging disabled | </summary>
        DISABLED=0, 
    	///<summary> Charging enabled | </summary>
        ENABLED=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_MODEL: int /*default*/
    {
			///<summary> Unknown gopro model | </summary>
        UNKNOWN=0, 
    	///<summary> Hero 3+ Silver (HeroBus not supported by GoPro) | </summary>
        HERO_3_PLUS_SILVER=1, 
    	///<summary> Hero 3+ Black | </summary>
        HERO_3_PLUS_BLACK=2, 
    	///<summary> Hero 4 Silver | </summary>
        HERO_4_SILVER=3, 
    	///<summary> Hero 4 Black | </summary>
        HERO_4_BLACK=4, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_BURST_RATE: int /*default*/
    {
			///<summary> 3 Shots / 1 Second | </summary>
        _3_IN_1_SECOND=0, 
    	///<summary> 5 Shots / 1 Second | </summary>
        _5_IN_1_SECOND=1, 
    	///<summary> 10 Shots / 1 Second | </summary>
        _10_IN_1_SECOND=2, 
    	///<summary> 10 Shots / 2 Second | </summary>
        _10_IN_2_SECOND=3, 
    	///<summary> 10 Shots / 3 Second (Hero 4 Only) | </summary>
        _10_IN_3_SECOND=4, 
    	///<summary> 30 Shots / 1 Second | </summary>
        _30_IN_1_SECOND=5, 
    	///<summary> 30 Shots / 2 Second | </summary>
        _30_IN_2_SECOND=6, 
    	///<summary> 30 Shots / 3 Second | </summary>
        _30_IN_3_SECOND=7, 
    	///<summary> 30 Shots / 6 Second | </summary>
        _30_IN_6_SECOND=8, 
    
    };
    
    ///<summary>  </summary>
    public enum LED_CONTROL_PATTERN: int /*default*/
    {
			///<summary> LED patterns off (return control to regular vehicle control) | </summary>
        OFF=0, 
    	///<summary> LEDs show pattern during firmware update | </summary>
        FIRMWAREUPDATE=1, 
    	///<summary> Custom Pattern using custom bytes fields | </summary>
        CUSTOM=255, 
    
    };
    
    ///<summary> Flags in EKF_STATUS message </summary>
    public enum EKF_STATUS_FLAGS: ushort
    {
			///<summary> set if EKF's attitude estimate is good | </summary>
        EKF_ATTITUDE=1, 
    	///<summary> set if EKF's horizontal velocity estimate is good | </summary>
        EKF_VELOCITY_HORIZ=2, 
    	///<summary> set if EKF's vertical velocity estimate is good | </summary>
        EKF_VELOCITY_VERT=4, 
    	///<summary> set if EKF's horizontal position (relative) estimate is good | </summary>
        EKF_POS_HORIZ_REL=8, 
    	///<summary> set if EKF's horizontal position (absolute) estimate is good | </summary>
        EKF_POS_HORIZ_ABS=16, 
    	///<summary> set if EKF's vertical position (absolute) estimate is good | </summary>
        EKF_POS_VERT_ABS=32, 
    	///<summary> set if EKF's vertical position (above ground) estimate is good | </summary>
        EKF_POS_VERT_AGL=64, 
    	///<summary> EKF is in constant position mode and does not know it's absolute or relative position | </summary>
        EKF_CONST_POS_MODE=128, 
    	///<summary> set if EKF's predicted horizontal position (relative) estimate is good | </summary>
        EKF_PRED_POS_HORIZ_REL=256, 
    	///<summary> set if EKF's predicted horizontal position (absolute) estimate is good | </summary>
        EKF_PRED_POS_HORIZ_ABS=512, 
    
    };
    
    ///<summary>  </summary>
    public enum PID_TUNING_AXIS: byte
    {
			///<summary>  | </summary>
        PID_TUNING_ROLL=1, 
    	///<summary>  | </summary>
        PID_TUNING_PITCH=2, 
    	///<summary>  | </summary>
        PID_TUNING_YAW=3, 
    	///<summary>  | </summary>
        PID_TUNING_ACCZ=4, 
    	///<summary>  | </summary>
        PID_TUNING_STEER=5, 
    	///<summary>  | </summary>
        PID_TUNING_LANDING=6, 
    
    };
    
    ///<summary>  </summary>
    public enum MAG_CAL_STATUS: byte
    {
			///<summary>  | </summary>
        MAG_CAL_NOT_STARTED=0, 
    	///<summary>  | </summary>
        MAG_CAL_WAITING_TO_START=1, 
    	///<summary>  | </summary>
        MAG_CAL_RUNNING_STEP_ONE=2, 
    	///<summary>  | </summary>
        MAG_CAL_RUNNING_STEP_TWO=3, 
    	///<summary>  | </summary>
        MAG_CAL_SUCCESS=4, 
    	///<summary>  | </summary>
        MAG_CAL_FAILED=5, 
    	///<summary>  | </summary>
        MAG_CAL_BAD_ORIENTATION=6, 
    
    };
    
    ///<summary> Special ACK block numbers control activation of dataflash log streaming </summary>
    public enum MAV_REMOTE_LOG_DATA_BLOCK_COMMANDS: uint
    {
			///<summary> UAV to stop sending DataFlash blocks | </summary>
        MAV_REMOTE_LOG_DATA_BLOCK_STOP=2147483645, 
    	///<summary> UAV to start sending DataFlash blocks | </summary>
        MAV_REMOTE_LOG_DATA_BLOCK_START=2147483646, 
    
    };
    
    ///<summary> Possible remote log data block statuses </summary>
    public enum MAV_REMOTE_LOG_DATA_BLOCK_STATUSES: byte
    {
			///<summary> This block has NOT been received | </summary>
        MAV_REMOTE_LOG_DATA_BLOCK_NACK=0, 
    	///<summary> This block has been received | </summary>
        MAV_REMOTE_LOG_DATA_BLOCK_ACK=1, 
    
    };
    
    ///<summary> Bus types for device operations </summary>
    public enum DEVICE_OP_BUSTYPE: byte
    {
			///<summary> I2C Device operation | </summary>
        I2C=0, 
    	///<summary> SPI Device operation | </summary>
        SPI=1, 
    
    };
    
    ///<summary> Deepstall flight stage </summary>
    public enum DEEPSTALL_STAGE: byte
    {
			///<summary> Flying to the landing point | </summary>
        FLY_TO_LANDING=0, 
    	///<summary> Building an estimate of the wind | </summary>
        ESTIMATE_WIND=1, 
    	///<summary> Waiting to breakout of the loiter to fly the approach | </summary>
        WAIT_FOR_BREAKOUT=2, 
    	///<summary> Flying to the first arc point to turn around to the landing point | </summary>
        FLY_TO_ARC=3, 
    	///<summary> Turning around back to the deepstall landing point | </summary>
        ARC=4, 
    	///<summary> Approaching the landing point | </summary>
        APPROACH=5, 
    	///<summary> Stalling and steering towards the land point | </summary>
        LAND=6, 
    
    };
    
    ///<summary> A mapping of plane flight modes for custom_mode field of heartbeat </summary>
    public enum PLANE_MODE: int /*default*/
    {
			///<summary>  | </summary>
        MANUAL=0, 
    	///<summary>  | </summary>
        CIRCLE=1, 
    	///<summary>  | </summary>
        STABILIZE=2, 
    	///<summary>  | </summary>
        TRAINING=3, 
    	///<summary>  | </summary>
        ACRO=4, 
    	///<summary>  | </summary>
        FLY_BY_WIRE_A=5, 
    	///<summary>  | </summary>
        FLY_BY_WIRE_B=6, 
    	///<summary>  | </summary>
        CRUISE=7, 
    	///<summary>  | </summary>
        AUTOTUNE=8, 
    	///<summary>  | </summary>
        AUTO=10, 
    	///<summary>  | </summary>
        RTL=11, 
    	///<summary>  | </summary>
        LOITER=12, 
    	///<summary>  | </summary>
        AVOID_ADSB=14, 
    	///<summary>  | </summary>
        GUIDED=15, 
    	///<summary>  | </summary>
        INITIALIZING=16, 
    	///<summary>  | </summary>
        QSTABILIZE=17, 
    	///<summary>  | </summary>
        QHOVER=18, 
    	///<summary>  | </summary>
        QLOITER=19, 
    	///<summary>  | </summary>
        QLAND=20, 
    	///<summary>  | </summary>
        QRTL=21, 
    
    };
    
    ///<summary> A mapping of copter flight modes for custom_mode field of heartbeat </summary>
    public enum COPTER_MODE: int /*default*/
    {
			///<summary>  | </summary>
        STABILIZE=0, 
    	///<summary>  | </summary>
        ACRO=1, 
    	///<summary>  | </summary>
        ALT_HOLD=2, 
    	///<summary>  | </summary>
        AUTO=3, 
    	///<summary>  | </summary>
        GUIDED=4, 
    	///<summary>  | </summary>
        LOITER=5, 
    	///<summary>  | </summary>
        RTL=6, 
    	///<summary>  | </summary>
        CIRCLE=7, 
    	///<summary>  | </summary>
        LAND=9, 
    	///<summary>  | </summary>
        DRIFT=11, 
    	///<summary>  | </summary>
        SPORT=13, 
    	///<summary>  | </summary>
        FLIP=14, 
    	///<summary>  | </summary>
        AUTOTUNE=15, 
    	///<summary>  | </summary>
        POSHOLD=16, 
    	///<summary>  | </summary>
        BRAKE=17, 
    	///<summary>  | </summary>
        THROW=18, 
    	///<summary>  | </summary>
        AVOID_ADSB=19, 
    	///<summary>  | </summary>
        GUIDED_NOGPS=20, 
    	///<summary>  | </summary>
        SMART_RTL=21, 
    
    };
    
    ///<summary> A mapping of sub flight modes for custom_mode field of heartbeat </summary>
    public enum SUB_MODE: int /*default*/
    {
			///<summary>  | </summary>
        STABILIZE=0, 
    	///<summary>  | </summary>
        ACRO=1, 
    	///<summary>  | </summary>
        ALT_HOLD=2, 
    	///<summary>  | </summary>
        AUTO=3, 
    	///<summary>  | </summary>
        GUIDED=4, 
    	///<summary>  | </summary>
        CIRCLE=7, 
    	///<summary>  | </summary>
        SURFACE=9, 
    	///<summary>  | </summary>
        POSHOLD=16, 
    	///<summary>  | </summary>
        MANUAL=19, 
    
    };
    
    ///<summary> A mapping of rover flight modes for custom_mode field of heartbeat </summary>
    public enum ROVER_MODE: int /*default*/
    {
			///<summary>  | </summary>
        MANUAL=0, 
    	///<summary>  | </summary>
        ACRO=1, 
    	///<summary>  | </summary>
        STEERING=3, 
    	///<summary>  | </summary>
        HOLD=4, 
    	///<summary>  | </summary>
        LOITER=5, 
    	///<summary>  | </summary>
        AUTO=10, 
    	///<summary>  | </summary>
        RTL=11, 
    	///<summary>  | </summary>
        SMART_RTL=12, 
    	///<summary>  | </summary>
        GUIDED=15, 
    	///<summary>  | </summary>
        INITIALIZING=16, 
    
    };
    
    ///<summary> A mapping of antenna tracker flight modes for custom_mode field of heartbeat </summary>
    public enum TRACKER_MODE: int /*default*/
    {
			///<summary>  | </summary>
        MANUAL=0, 
    	///<summary>  | </summary>
        STOP=1, 
    	///<summary>  | </summary>
        SCAN=2, 
    	///<summary>  | </summary>
        SERVO_TEST=3, 
    	///<summary>  | </summary>
        AUTO=10, 
    	///<summary>  | </summary>
        INITIALIZING=16, 
    
    };
    
    
    ///<summary> Micro air vehicle / autopilot classes. This identifies the individual model. </summary>
    public enum MAV_AUTOPILOT: byte
    {
			///<summary> Generic autopilot, full support for everything | </summary>
        GENERIC=0, 
    	///<summary> Reserved for future use. | </summary>
        RESERVED=1, 
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
    	///<summary> SmartAP Autopilot - http://sky-drones.com | </summary>
        SMARTAP=18, 
    	///<summary> AirRails - http://uaventure.com | </summary>
        AIRRAILS=19, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_TYPE: byte
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
    	///<summary> Tricopter | </summary>
        TRICOPTER=15, 
    	///<summary> Flapping wing | </summary>
        FLAPPING_WING=16, 
    	///<summary> Kite | </summary>
        KITE=17, 
    	///<summary> Onboard companion controller | </summary>
        ONBOARD_CONTROLLER=18, 
    	///<summary> Two-rotor VTOL using control surfaces in vertical operation in addition. Tailsitter. | </summary>
        VTOL_DUOROTOR=19, 
    	///<summary> Quad-rotor VTOL using a V-shaped quad config in vertical operation. Tailsitter. | </summary>
        VTOL_QUADROTOR=20, 
    	///<summary> Tiltrotor VTOL | </summary>
        VTOL_TILTROTOR=21, 
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
    	///<summary> Onboard ADSB peripheral | </summary>
        ADSB=27, 
    	///<summary> Steerable, nonrigid airfoil | </summary>
        PARAFOIL=28, 
    	///<summary> Dodecarotor | </summary>
        DODECAROTOR=29, 
    	///<summary> Camera | </summary>
        CAMERA=30, 
    	///<summary> Charging station | </summary>
        CHARGING_STATION=31, 
    	///<summary> Onboard FLARM collision avoidance system | </summary>
        FLARM=32, 
    
    };
    
    ///<summary> These values define the type of firmware release.  These values indicate the first version or release of this type.  For example the first alpha release would be 64, the second would be 65. </summary>
    public enum FIRMWARE_VERSION_TYPE: int /*default*/
    {
			///<summary> development release | </summary>
        DEV=0, 
    	///<summary> alpha release | </summary>
        ALPHA=64, 
    	///<summary> beta release | </summary>
        BETA=128, 
    	///<summary> release candidate | </summary>
        RC=192, 
    	///<summary> official stable release | </summary>
        OFFICIAL=255, 
    
    };
    
    ///<summary> These flags encode the MAV mode. </summary>
    public enum MAV_MODE_FLAG: byte
    {
			///<summary> 0b00000001 Reserved for future use. | </summary>
        CUSTOM_MODE_ENABLED=1, 
    	///<summary> 0b00000010 system has a test mode enabled. This flag is intended for temporary system tests and should not be used for stable implementations. | </summary>
        TEST_ENABLED=2, 
    	///<summary> 0b00000100 autonomous mode enabled, system finds its own goal positions. Guided flag can be set or not, depends on the actual implementation. | </summary>
        AUTO_ENABLED=4, 
    	///<summary> 0b00001000 guided mode enabled, system flies waypoints / mission items. | </summary>
        GUIDED_ENABLED=8, 
    	///<summary> 0b00010000 system stabilizes electronically its attitude (and optionally position). It needs however further control inputs to move around. | </summary>
        STABILIZE_ENABLED=16, 
    	///<summary> 0b00100000 hardware in the loop simulation. All motors / actuators are blocked, but internal software is full operational. | </summary>
        HIL_ENABLED=32, 
    	///<summary> 0b01000000 remote control input is enabled. | </summary>
        MANUAL_INPUT_ENABLED=64, 
    	///<summary> 0b10000000 MAV safety set to armed. Motors are enabled / running / can start. Ready to fly. Additional note: this flag is to be ignore when sent in the command MAV_CMD_DO_SET_MODE and MAV_CMD_COMPONENT_ARM_DISARM shall be used instead. The flag can still be used to report the armed state. | </summary>
        SAFETY_ARMED=128, 
    
    };
    
    ///<summary> These values encode the bit positions of the decode position. These values can be used to read the value of a flag bit by combining the base_mode variable with AND with the flag position value. The result will be either 0 or 1, depending on if the flag is set or not. </summary>
    public enum MAV_MODE_FLAG_DECODE_POSITION: int /*default*/
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
    
    };
    
    ///<summary> Override command, pauses current mission execution and moves immediately to a position </summary>
    public enum MAV_GOTO: int /*default*/
    {
			///<summary> Hold at the current position. | </summary>
        DO_HOLD=0, 
    	///<summary> Continue with the next item in mission execution. | </summary>
        DO_CONTINUE=1, 
    	///<summary> Hold at the current position of the system | </summary>
        HOLD_AT_CURRENT_POSITION=2, 
    	///<summary> Hold at the position specified in the parameters of the DO_HOLD action | </summary>
        HOLD_AT_SPECIFIED_POSITION=3, 
    
    };
    
    ///<summary> These defines are predefined OR-combined mode flags. There is no need to use values from this enum, but it    
///               simplifies the use of the mode flags. Note that manual input is enabled in all modes as a safety override. </summary>
    public enum MAV_MODE: byte
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
    	///<summary> System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by waypoints) | </summary>
        AUTO_DISARMED=92, 
    	///<summary> System is allowed to be active, under manual (RC) control, no stabilization | </summary>
        MANUAL_ARMED=192, 
    	///<summary> UNDEFINED mode. This solely depends on the autopilot - use with caution, intended for developers only. | </summary>
        TEST_ARMED=194, 
    	///<summary> System is allowed to be active, under assisted RC control. | </summary>
        STABILIZE_ARMED=208, 
    	///<summary> System is allowed to be active, under autonomous control, manual setpoint | </summary>
        GUIDED_ARMED=216, 
    	///<summary> System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by waypoints) | </summary>
        AUTO_ARMED=220, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_STATE: byte
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
    	///<summary> System is terminating itself. | </summary>
        FLIGHT_TERMINATION=8, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_COMPONENT: int /*default*/
    {
			///<summary>  | </summary>
        MAV_COMP_ID_ALL=0, 
    	///<summary>  | </summary>
        MAV_COMP_ID_AUTOPILOT1=1, 
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
        MAV_COMP_ID_LOG=155, 
    	///<summary>  | </summary>
        MAV_COMP_ID_ADSB=156, 
    	///<summary> On Screen Display (OSD) devices for video links | </summary>
        MAV_COMP_ID_OSD=157, 
    	///<summary> Generic autopilot peripheral component ID. Meant for devices that do not implement the parameter sub-protocol | </summary>
        MAV_COMP_ID_PERIPHERAL=158, 
    	///<summary>  | </summary>
        MAV_COMP_ID_QX1_GIMBAL=159, 
    	///<summary>  | </summary>
        MAV_COMP_ID_FLARM=160, 
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
        MAV_COMP_ID_GPS2=221, 
    	///<summary>  | </summary>
        MAV_COMP_ID_UDP_BRIDGE=240, 
    	///<summary>  | </summary>
        MAV_COMP_ID_UART_BRIDGE=241, 
    	///<summary>  | </summary>
        MAV_COMP_ID_SYSTEM_CONTROL=250, 
    
    };
    
    ///<summary> These encode the sensors whose status is sent as part of the SYS_STATUS message. </summary>
    public enum MAV_SYS_STATUS_SENSOR: uint
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
    	///<summary> 0x800000 Motors are reversed | </summary>
        MAV_SYS_STATUS_REVERSE_MOTOR=8388608, 
    	///<summary> 0x1000000 Logging | </summary>
        MAV_SYS_STATUS_LOGGING=16777216, 
    	///<summary> 0x2000000 Battery | </summary>
        BATTERY=33554432, 
    	///<summary> 0x4000000 Proximity | </summary>
        PROXIMITY=67108864, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_FRAME: byte
    {
			///<summary> Global coordinate frame, WGS84 coordinate system. First value / x: latitude, second value / y: longitude, third value / z: positive altitude over mean sea level (MSL). | </summary>
        GLOBAL=0, 
    	///<summary> Local coordinate frame, Z-down (x: north, y: east, z: down). | </summary>
        LOCAL_NED=1, 
    	///<summary> NOT a coordinate frame, indicates a mission command. | </summary>
        MISSION=2, 
    	///<summary> Global coordinate frame, WGS84 coordinate system, relative altitude over ground with respect to the home position. First value / x: latitude, second value / y: longitude, third value / z: positive altitude with 0 being at the altitude of the home location. | </summary>
        GLOBAL_RELATIVE_ALT=3, 
    	///<summary> Local coordinate frame, Z-up (x: east, y: north, z: up). | </summary>
        LOCAL_ENU=4, 
    	///<summary> Global coordinate frame, WGS84 coordinate system. First value / x: latitude in degrees*1.0e-7, second value / y: longitude in degrees*1.0e-7, third value / z: positive altitude over mean sea level (MSL). | </summary>
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
    	///<summary> Body fixed frame of reference, Z-down (x: forward, y: right, z: down). | </summary>
        BODY_FRD=12, 
    	///<summary> Body fixed frame of reference, Z-up (x: forward, y: left, z: up). | </summary>
        BODY_FLU=13, 
    	///<summary> Odometry local coordinate frame of data given by a motion capture system, Z-down (x: north, y: east, z: down). | </summary>
        MOCAP_NED=14, 
    	///<summary> Odometry local coordinate frame of data given by a motion capture system, Z-up (x: east, y: north, z: up). | </summary>
        MOCAP_ENU=15, 
    	///<summary> Odometry local coordinate frame of data given by a vision estimation system, Z-down (x: north, y: east, z: down). | </summary>
        VISION_NED=16, 
    	///<summary> Odometry local coordinate frame of data given by a vision estimation system, Z-up (x: east, y: north, z: up). | </summary>
        VISION_ENU=17, 
    	///<summary> Odometry local coordinate frame of data given by an estimator running onboard the vehicle, Z-down (x: north, y: east, z: down). | </summary>
        ESTIM_NED=18, 
    	///<summary> Odometry local coordinate frame of data given by an estimator running onboard the vehicle, Z-up (x: east, y: noth, z: up). | </summary>
        ESTIM_ENU=19, 
    
    };
    
    ///<summary>  </summary>
    public enum MAVLINK_DATA_STREAM_TYPE: int /*default*/
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
    
    };
    
    ///<summary>  </summary>
    public enum FENCE_ACTION: int /*default*/
    {
			///<summary> Disable fenced mode | </summary>
        NONE=0, 
    	///<summary> Switched to guided mode to return point (fence point 0) | </summary>
        GUIDED=1, 
    	///<summary> Report fence breach, but don't take action | </summary>
        REPORT=2, 
    	///<summary> Switched to guided mode to return point (fence point 0) with manual throttle control | </summary>
        GUIDED_THR_PASS=3, 
    	///<summary> Switch to RTL (return to launch) mode and head for the return point. | </summary>
        RTL=4, 
    
    };
    
    ///<summary>  </summary>
    public enum FENCE_BREACH: byte
    {
			///<summary> No last fence breach | </summary>
        NONE=0, 
    	///<summary> Breached minimum altitude | </summary>
        MINALT=1, 
    	///<summary> Breached maximum altitude | </summary>
        MAXALT=2, 
    	///<summary> Breached fence boundary | </summary>
        BOUNDARY=3, 
    
    };
    
    ///<summary> Enumeration of possible mount operation modes </summary>
    public enum MAV_MOUNT_MODE: byte
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
    
    };
    
    ///<summary> Generalized UAVCAN node health </summary>
    public enum UAVCAN_NODE_HEALTH: byte
    {
			///<summary> The node is functioning properly. | </summary>
        OK=0, 
    	///<summary> A critical parameter went out of range or the node has encountered a minor failure. | </summary>
        WARNING=1, 
    	///<summary> The node has encountered a major failure. | </summary>
        ERROR=2, 
    	///<summary> The node has suffered a fatal malfunction. | </summary>
        CRITICAL=3, 
    
    };
    
    ///<summary> Generalized UAVCAN node mode </summary>
    public enum UAVCAN_NODE_MODE: byte
    {
			///<summary> The node is performing its primary functions. | </summary>
        OPERATIONAL=0, 
    	///<summary> The node is initializing; this mode is entered immediately after startup. | </summary>
        INITIALIZATION=1, 
    	///<summary> The node is under maintenance. | </summary>
        MAINTENANCE=2, 
    	///<summary> The node is in the process of updating its software. | </summary>
        SOFTWARE_UPDATE=3, 
    	///<summary> The node is no longer available online. | </summary>
        OFFLINE=7, 
    
    };
    
    ///<summary> THIS INTERFACE IS DEPRECATED AS OF JULY 2015. Please use MESSAGE_INTERVAL instead. A data stream is not a fixed set of messages, but rather a    
///     recommendation to the autopilot software. Individual autopilots may or may not obey    
///     the recommended messages. </summary>
    public enum MAV_DATA_STREAM: int /*default*/
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
    
    };
    
    ///<summary> THIS INTERFACE IS DEPRECATED AS OF JANUARY 2018. Please use MAV_CMD_DO_SET_ROI_* messages instead. The ROI (region of interest) for the vehicle. This can be    
///                be used by the vehicle for camera/vehicle attitude alignment (see    
///                MAV_CMD_NAV_ROI). </summary>
    public enum MAV_ROI: int /*default*/
    {
			///<summary> No region of interest. | </summary>
        NONE=0, 
    	///<summary> Point toward next waypoint. | </summary>
        WPNEXT=1, 
    	///<summary> Point toward given waypoint. | </summary>
        WPINDEX=2, 
    	///<summary> Point toward fixed location. | </summary>
        LOCATION=3, 
    	///<summary> Point toward of given id. | </summary>
        TARGET=4, 
    
    };
    
    ///<summary> ACK / NACK / ERROR values as a result of MAV_CMDs and for mission item transmission. </summary>
    public enum MAV_CMD_ACK: int /*default*/
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
    
    };
    
    ///<summary> Specifies the datatype of a MAVLink parameter. </summary>
    public enum MAV_PARAM_TYPE: byte
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
    
    };
    
    ///<summary> result from a mavlink command </summary>
    public enum MAV_RESULT: byte
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
    
    };
    
    ///<summary> result in a mavlink mission ack </summary>
    public enum MAV_MISSION_RESULT: byte
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
    
    };
    
    ///<summary> Indicates the severity level, generally used for status messages to indicate their relative urgency. Based on RFC-5424 using expanded definitions at: http://www.kiwisyslog.com/kb/info:-syslog-message-levels/. </summary>
    public enum MAV_SEVERITY: byte
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
    
    };
    
    ///<summary> Power supply status flags (bitmask) </summary>
    public enum MAV_POWER_STATUS: ushort
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
    
    };
    
    ///<summary> SERIAL_CONTROL device types </summary>
    public enum SERIAL_CONTROL_DEV: byte
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
    
    };
    
    ///<summary> SERIAL_CONTROL flags (bitmask) </summary>
    public enum SERIAL_CONTROL_FLAG: byte
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
    
    };
    
    ///<summary> Enumeration of distance sensor types </summary>
    public enum MAV_DISTANCE_SENSOR: byte
    {
			///<summary> Laser rangefinder, e.g. LightWare SF02/F or PulsedLight units | </summary>
        LASER=0, 
    	///<summary> Ultrasound rangefinder, e.g. MaxBotix units | </summary>
        ULTRASOUND=1, 
    	///<summary> Infrared rangefinder, e.g. Sharp units | </summary>
        INFRARED=2, 
    	///<summary> Radar type, e.g. uLanding units | </summary>
        RADAR=3, 
    	///<summary> Broken or unknown type, e.g. analog units | </summary>
        UNKNOWN=4, 
    
    };
    
    ///<summary> Enumeration of sensor orientation, according to its rotations </summary>
    public enum MAV_SENSOR_ORIENTATION: byte
    {
			///<summary> Roll: 0, Pitch: 0, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_NONE=0, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 45 | </summary>
        MAV_SENSOR_ROTATION_YAW_45=1, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 90 | </summary>
        MAV_SENSOR_ROTATION_YAW_90=2, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 135 | </summary>
        MAV_SENSOR_ROTATION_YAW_135=3, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 180 | </summary>
        MAV_SENSOR_ROTATION_YAW_180=4, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 225 | </summary>
        MAV_SENSOR_ROTATION_YAW_225=5, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 270 | </summary>
        MAV_SENSOR_ROTATION_YAW_270=6, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 315 | </summary>
        MAV_SENSOR_ROTATION_YAW_315=7, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_180=8, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 45 | </summary>
        MAV_SENSOR_ROTATION_ROLL_180_YAW_45=9, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 90 | </summary>
        MAV_SENSOR_ROTATION_ROLL_180_YAW_90=10, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 135 | </summary>
        MAV_SENSOR_ROTATION_ROLL_180_YAW_135=11, 
    	///<summary> Roll: 0, Pitch: 180, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_PITCH_180=12, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 225 | </summary>
        MAV_SENSOR_ROTATION_ROLL_180_YAW_225=13, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 270 | </summary>
        MAV_SENSOR_ROTATION_ROLL_180_YAW_270=14, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 315 | </summary>
        MAV_SENSOR_ROTATION_ROLL_180_YAW_315=15, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_90=16, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 45 | </summary>
        MAV_SENSOR_ROTATION_ROLL_90_YAW_45=17, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 90 | </summary>
        MAV_SENSOR_ROTATION_ROLL_90_YAW_90=18, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 135 | </summary>
        MAV_SENSOR_ROTATION_ROLL_90_YAW_135=19, 
    	///<summary> Roll: 270, Pitch: 0, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_270=20, 
    	///<summary> Roll: 270, Pitch: 0, Yaw: 45 | </summary>
        MAV_SENSOR_ROTATION_ROLL_270_YAW_45=21, 
    	///<summary> Roll: 270, Pitch: 0, Yaw: 90 | </summary>
        MAV_SENSOR_ROTATION_ROLL_270_YAW_90=22, 
    	///<summary> Roll: 270, Pitch: 0, Yaw: 135 | </summary>
        MAV_SENSOR_ROTATION_ROLL_270_YAW_135=23, 
    	///<summary> Roll: 0, Pitch: 90, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_PITCH_90=24, 
    	///<summary> Roll: 0, Pitch: 270, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_PITCH_270=25, 
    	///<summary> Roll: 0, Pitch: 180, Yaw: 90 | </summary>
        MAV_SENSOR_ROTATION_PITCH_180_YAW_90=26, 
    	///<summary> Roll: 0, Pitch: 180, Yaw: 270 | </summary>
        MAV_SENSOR_ROTATION_PITCH_180_YAW_270=27, 
    	///<summary> Roll: 90, Pitch: 90, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_90=28, 
    	///<summary> Roll: 180, Pitch: 90, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_180_PITCH_90=29, 
    	///<summary> Roll: 270, Pitch: 90, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_270_PITCH_90=30, 
    	///<summary> Roll: 90, Pitch: 180, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_180=31, 
    	///<summary> Roll: 270, Pitch: 180, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_270_PITCH_180=32, 
    	///<summary> Roll: 90, Pitch: 270, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_270=33, 
    	///<summary> Roll: 180, Pitch: 270, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_180_PITCH_270=34, 
    	///<summary> Roll: 270, Pitch: 270, Yaw: 0 | </summary>
        MAV_SENSOR_ROTATION_ROLL_270_PITCH_270=35, 
    	///<summary> Roll: 90, Pitch: 180, Yaw: 90 | </summary>
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_180_YAW_90=36, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 270 | </summary>
        MAV_SENSOR_ROTATION_ROLL_90_YAW_270=37, 
    	///<summary> Roll: 315, Pitch: 315, Yaw: 315 | </summary>
        MAV_SENSOR_ROTATION_ROLL_315_PITCH_315_YAW_315=38, 
    
    };
    
    ///<summary> Bitmask of (optional) autopilot capabilities (64 bit). If a bit is set, the autopilot supports this capability. </summary>
    public enum MAV_PROTOCOL_CAPABILITY: ulong
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
    	///<summary> Autopilot supports the new FILE_TRANSFER_PROTOCOL message type. | </summary>
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
    	///<summary> Autopilot supports the flight termination command. | </summary>
        FLIGHT_TERMINATION=2048, 
    	///<summary> Autopilot supports onboard compass calibration. | </summary>
        COMPASS_CALIBRATION=4096, 
    	///<summary> Autopilot supports mavlink version 2. | </summary>
        MAVLINK2=8192, 
    	///<summary> Autopilot supports mission fence protocol. | </summary>
        MISSION_FENCE=16384, 
    	///<summary> Autopilot supports mission rally point protocol. | </summary>
        MISSION_RALLY=32768, 
    	///<summary> Autopilot supports the flight information protocol. | </summary>
        FLIGHT_INFORMATION=65536, 
    
    };
    
    ///<summary> Type of mission items being requested/sent in mission protocol. </summary>
    public enum MAV_MISSION_TYPE: byte
    {
			///<summary> Items are mission commands for main mission. | </summary>
        MISSION=0, 
    	///<summary> Specifies GeoFence area(s). Items are MAV_CMD_FENCE_ GeoFence items. | </summary>
        FENCE=1, 
    	///<summary> Specifies the rally points for the vehicle. Rally points are alternative RTL points. Items are MAV_CMD_RALLY_POINT rally point items. | </summary>
        RALLY=2, 
    	///<summary> Only used in MISSION_CLEAR_ALL to clear all mission types. | </summary>
        ALL=255, 
    
    };
    
    ///<summary> Enumeration of estimator types </summary>
    public enum MAV_ESTIMATOR_TYPE: byte
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
    
    };
    
    ///<summary> Enumeration of battery types </summary>
    public enum MAV_BATTERY_TYPE: byte
    {
			///<summary> Not specified. | </summary>
        UNKNOWN=0, 
    	///<summary> Lithium polymer battery | </summary>
        LIPO=1, 
    	///<summary> Lithium-iron-phosphate battery | </summary>
        LIFE=2, 
    	///<summary> Lithium-ION battery | </summary>
        LION=3, 
    	///<summary> Nickel metal hydride battery | </summary>
        NIMH=4, 
    
    };
    
    ///<summary> Enumeration of battery functions </summary>
    public enum MAV_BATTERY_FUNCTION: byte
    {
			///<summary> Battery function is unknown | </summary>
        UNKNOWN=0, 
    	///<summary> Battery supports all flight systems | </summary>
        ALL=1, 
    	///<summary> Battery for the propulsion system | </summary>
        PROPULSION=2, 
    	///<summary> Avionics battery | </summary>
        AVIONICS=3, 
    	///<summary> Payload battery | </summary>
        MAV_BATTERY_TYPE_PAYLOAD=4, 
    
    };
    
    ///<summary> Enumeration for low battery states. </summary>
    public enum MAV_BATTERY_CHARGE_STATE: byte
    {
			///<summary> Low battery state is not provided | </summary>
        UNDEFINED=0, 
    	///<summary> Battery is not in low state. Normal operation. | </summary>
        OK=1, 
    	///<summary> Battery state is low, warn and monitor close. | </summary>
        LOW=2, 
    	///<summary> Battery state is critical, return or abort immediately. | </summary>
        CRITICAL=3, 
    	///<summary> Battery state is too low for ordinary abort sequence. Perform fastest possible emergency stop to prevent damage. | </summary>
        EMERGENCY=4, 
    	///<summary> Battery failed, damage unavoidable. | </summary>
        FAILED=5, 
    	///<summary> Battery is diagnosed to be defective or an error occurred, usage is discouraged / prohibited. | </summary>
        UNHEALTHY=6, 
    
    };
    
    ///<summary> Enumeration of VTOL states </summary>
    public enum MAV_VTOL_STATE: byte
    {
			///<summary> MAV is not configured as VTOL | </summary>
        UNDEFINED=0, 
    	///<summary> VTOL is in transition from multicopter to fixed-wing | </summary>
        TRANSITION_TO_FW=1, 
    	///<summary> VTOL is in transition from fixed-wing to multicopter | </summary>
        TRANSITION_TO_MC=2, 
    	///<summary> VTOL is in multicopter state | </summary>
        MC=3, 
    	///<summary> VTOL is in fixed-wing state | </summary>
        FW=4, 
    
    };
    
    ///<summary> Enumeration of landed detector states </summary>
    public enum MAV_LANDED_STATE: byte
    {
			///<summary> MAV landed state is unknown | </summary>
        UNDEFINED=0, 
    	///<summary> MAV is landed (on ground) | </summary>
        ON_GROUND=1, 
    	///<summary> MAV is in air | </summary>
        IN_AIR=2, 
    	///<summary> MAV currently taking off | </summary>
        TAKEOFF=3, 
    	///<summary> MAV currently landing | </summary>
        LANDING=4, 
    
    };
    
    ///<summary> Enumeration of the ADSB altimeter types </summary>
    public enum ADSB_ALTITUDE_TYPE: byte
    {
			///<summary> Altitude reported from a Baro source using QNH reference | </summary>
        PRESSURE_QNH=0, 
    	///<summary> Altitude reported from a GNSS source | </summary>
        GEOMETRIC=1, 
    
    };
    
    ///<summary> ADSB classification for the type of vehicle emitting the transponder signal </summary>
    public enum ADSB_EMITTER_TYPE: byte
    {
			///<summary>  | </summary>
        NO_INFO=0, 
    	///<summary>  | </summary>
        LIGHT=1, 
    	///<summary>  | </summary>
        SMALL=2, 
    	///<summary>  | </summary>
        LARGE=3, 
    	///<summary>  | </summary>
        HIGH_VORTEX_LARGE=4, 
    	///<summary>  | </summary>
        HEAVY=5, 
    	///<summary>  | </summary>
        HIGHLY_MANUV=6, 
    	///<summary>  | </summary>
        ROTOCRAFT=7, 
    	///<summary>  | </summary>
        UNASSIGNED=8, 
    	///<summary>  | </summary>
        GLIDER=9, 
    	///<summary>  | </summary>
        LIGHTER_AIR=10, 
    	///<summary>  | </summary>
        PARACHUTE=11, 
    	///<summary>  | </summary>
        ULTRA_LIGHT=12, 
    	///<summary>  | </summary>
        UNASSIGNED2=13, 
    	///<summary>  | </summary>
        UAV=14, 
    	///<summary>  | </summary>
        SPACE=15, 
    	///<summary>  | </summary>
        UNASSGINED3=16, 
    	///<summary>  | </summary>
        EMERGENCY_SURFACE=17, 
    	///<summary>  | </summary>
        SERVICE_SURFACE=18, 
    	///<summary>  | </summary>
        POINT_OBSTACLE=19, 
    
    };
    
    ///<summary> These flags indicate status such as data validity of each data source. Set = data valid </summary>
    public enum ADSB_FLAGS: ushort
    {
			///<summary>  | </summary>
        VALID_COORDS=1, 
    	///<summary>  | </summary>
        VALID_ALTITUDE=2, 
    	///<summary>  | </summary>
        VALID_HEADING=4, 
    	///<summary>  | </summary>
        VALID_VELOCITY=8, 
    	///<summary>  | </summary>
        VALID_CALLSIGN=16, 
    	///<summary>  | </summary>
        VALID_SQUAWK=32, 
    	///<summary>  | </summary>
        SIMULATED=64, 
    
    };
    
    ///<summary> Bitmask of options for the MAV_CMD_DO_REPOSITION </summary>
    public enum MAV_DO_REPOSITION_FLAGS: int /*default*/
    {
			///<summary> The aircraft should immediately transition into guided. This should not be set for follow me applications | </summary>
        CHANGE_MODE=1, 
    
    };
    
    ///<summary> Flags in EKF_STATUS message </summary>
    public enum ESTIMATOR_STATUS_FLAGS: ushort
    {
			///<summary> True if the attitude estimate is good | </summary>
        ESTIMATOR_ATTITUDE=1, 
    	///<summary> True if the horizontal velocity estimate is good | </summary>
        ESTIMATOR_VELOCITY_HORIZ=2, 
    	///<summary> True if the  vertical velocity estimate is good | </summary>
        ESTIMATOR_VELOCITY_VERT=4, 
    	///<summary> True if the horizontal position (relative) estimate is good | </summary>
        ESTIMATOR_POS_HORIZ_REL=8, 
    	///<summary> True if the horizontal position (absolute) estimate is good | </summary>
        ESTIMATOR_POS_HORIZ_ABS=16, 
    	///<summary> True if the vertical position (absolute) estimate is good | </summary>
        ESTIMATOR_POS_VERT_ABS=32, 
    	///<summary> True if the vertical position (above ground) estimate is good | </summary>
        ESTIMATOR_POS_VERT_AGL=64, 
    	///<summary> True if the EKF is in a constant position mode and is not using external measurements (eg GPS or optical flow) | </summary>
        ESTIMATOR_CONST_POS_MODE=128, 
    	///<summary> True if the EKF has sufficient data to enter a mode that will provide a (relative) position estimate | </summary>
        ESTIMATOR_PRED_POS_HORIZ_REL=256, 
    	///<summary> True if the EKF has sufficient data to enter a mode that will provide a (absolute) position estimate | </summary>
        ESTIMATOR_PRED_POS_HORIZ_ABS=512, 
    	///<summary> True if the EKF has detected a GPS glitch | </summary>
        ESTIMATOR_GPS_GLITCH=1024, 
    	///<summary> True if the EKF has detected bad accelerometer data | </summary>
        ESTIMATOR_ACCEL_ERROR=2048, 
    
    };
    
    ///<summary>  </summary>
    public enum MOTOR_TEST_ORDER: int /*default*/
    {
			///<summary> default autopilot motor test method | </summary>
        DEFAULT=0, 
    	///<summary> motor numbers are specified as their index in a predefined vehicle-specific sequence | </summary>
        SEQUENCE=1, 
    	///<summary> motor numbers are specified as the output as labeled on the board | </summary>
        BOARD=2, 
    
    };
    
    ///<summary>  </summary>
    public enum MOTOR_TEST_THROTTLE_TYPE: int /*default*/
    {
			///<summary> throttle as a percentage from 0 ~ 100 | </summary>
        MOTOR_TEST_THROTTLE_PERCENT=0, 
    	///<summary> throttle as an absolute PWM value (normally in range of 1000~2000) | </summary>
        MOTOR_TEST_THROTTLE_PWM=1, 
    	///<summary> throttle pass-through from pilot's transmitter | </summary>
        MOTOR_TEST_THROTTLE_PILOT=2, 
    	///<summary> per-motor compass calibration test | </summary>
        MOTOR_TEST_COMPASS_CAL=3, 
    
    };
    
    ///<summary>  </summary>
    public enum GPS_INPUT_IGNORE_FLAGS: ushort
    {
			///<summary> ignore altitude field | </summary>
        GPS_INPUT_IGNORE_FLAG_ALT=1, 
    	///<summary> ignore hdop field | </summary>
        GPS_INPUT_IGNORE_FLAG_HDOP=2, 
    	///<summary> ignore vdop field | </summary>
        GPS_INPUT_IGNORE_FLAG_VDOP=4, 
    	///<summary> ignore horizontal velocity field (vn and ve) | </summary>
        GPS_INPUT_IGNORE_FLAG_VEL_HORIZ=8, 
    	///<summary> ignore vertical velocity field (vd) | </summary>
        GPS_INPUT_IGNORE_FLAG_VEL_VERT=16, 
    	///<summary> ignore speed accuracy field | </summary>
        GPS_INPUT_IGNORE_FLAG_SPEED_ACCURACY=32, 
    	///<summary> ignore horizontal accuracy field | </summary>
        GPS_INPUT_IGNORE_FLAG_HORIZONTAL_ACCURACY=64, 
    	///<summary> ignore vertical accuracy field | </summary>
        GPS_INPUT_IGNORE_FLAG_VERTICAL_ACCURACY=128, 
    
    };
    
    ///<summary> Possible actions an aircraft can take to avoid a collision. </summary>
    public enum MAV_COLLISION_ACTION: byte
    {
			///<summary> Ignore any potential collisions | </summary>
        NONE=0, 
    	///<summary> Report potential collision | </summary>
        REPORT=1, 
    	///<summary> Ascend or Descend to avoid threat | </summary>
        ASCEND_OR_DESCEND=2, 
    	///<summary> Move horizontally to avoid threat | </summary>
        MOVE_HORIZONTALLY=3, 
    	///<summary> Aircraft to move perpendicular to the collision's velocity vector | </summary>
        MOVE_PERPENDICULAR=4, 
    	///<summary> Aircraft to fly directly back to its launch point | </summary>
        RTL=5, 
    	///<summary> Aircraft to stop in place | </summary>
        HOVER=6, 
    
    };
    
    ///<summary> Aircraft-rated danger from this threat. </summary>
    public enum MAV_COLLISION_THREAT_LEVEL: byte
    {
			///<summary> Not a threat | </summary>
        NONE=0, 
    	///<summary> Craft is mildly concerned about this threat | </summary>
        LOW=1, 
    	///<summary> Craft is panicing, and may take actions to avoid threat | </summary>
        HIGH=2, 
    
    };
    
    ///<summary> Source of information about this collision. </summary>
    public enum MAV_COLLISION_SRC: byte
    {
			///<summary> ID field references ADSB_VEHICLE packets | </summary>
        ADSB=0, 
    	///<summary> ID field references MAVLink SRC ID | </summary>
        MAVLINK_GPS_GLOBAL_INT=1, 
    
    };
    
    ///<summary> Type of GPS fix </summary>
    public enum GPS_FIX_TYPE: byte
    {
			///<summary> No GPS connected | </summary>
        NO_GPS=0, 
    	///<summary> No position information, GPS is connected | </summary>
        NO_FIX=1, 
    	///<summary> 2D position | </summary>
        _2D_FIX=2, 
    	///<summary> 3D position | </summary>
        _3D_FIX=3, 
    	///<summary> DGPS/SBAS aided 3D position | </summary>
        DGPS=4, 
    	///<summary> RTK float, 3D position | </summary>
        RTK_FLOAT=5, 
    	///<summary> RTK Fixed, 3D position | </summary>
        RTK_FIXED=6, 
    	///<summary> Static fixed, typically used for base stations | </summary>
        STATIC=7, 
    	///<summary> PPP, 3D position. | </summary>
        PPP=8, 
    
    };
    
    ///<summary> RTK GPS baseline coordinate system, used for RTK corrections </summary>
    public enum RTK_BASELINE_COORDINATE_SYSTEM: byte
    {
			///<summary> Earth-centered, Earth-fixed | </summary>
        ECEF=0, 
    	///<summary> North, East, Down | </summary>
        NED=1, 
    
    };
    
    ///<summary> Type of landing target </summary>
    public enum LANDING_TARGET_TYPE: byte
    {
			///<summary> Landing target signaled by light beacon (ex: IR-LOCK) | </summary>
        LIGHT_BEACON=0, 
    	///<summary> Landing target signaled by radio beacon (ex: ILS, NDB) | </summary>
        RADIO_BEACON=1, 
    	///<summary> Landing target represented by a fiducial marker (ex: ARTag) | </summary>
        VISION_FIDUCIAL=2, 
    	///<summary> Landing target represented by a pre-defined visual shape/feature (ex: X-marker, H-marker, square) | </summary>
        VISION_OTHER=3, 
    
    };
    
    ///<summary> Direction of VTOL transition </summary>
    public enum VTOL_TRANSITION_HEADING: int /*default*/
    {
			///<summary> Respect the heading configuration of the vehicle. | </summary>
        VEHICLE_DEFAULT=0, 
    	///<summary> Use the heading pointing towards the next waypoint. | </summary>
        NEXT_WAYPOINT=1, 
    	///<summary> Use the heading on takeoff (while sitting on the ground). | </summary>
        TAKEOFF=2, 
    	///<summary> Use the specified heading in parameter 4. | </summary>
        SPECIFIED=3, 
    	///<summary> Use the current heading when reaching takeoff altitude (potentially facing the wind when weather-vaning is active). | </summary>
        ANY=4, 
    
    };
    
    ///<summary> Camera Modes. </summary>
    public enum CAMERA_MODE: int /*default*/
    {
			///<summary> Camera is in image/photo capture mode. | </summary>
        IMAGE=0, 
    	///<summary> Camera is in video capture mode. | </summary>
        VIDEO=1, 
    	///<summary> Camera is in image survey capture mode. It allows for camera controller to do specific settings for surveys. | </summary>
        IMAGE_SURVEY=2, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_ARM_AUTH_DENIED_REASON: int /*default*/
    {
			///<summary> Not a specific reason | </summary>
        GENERIC=0, 
    	///<summary> Authorizer will send the error as string to GCS | </summary>
        NONE=1, 
    	///<summary> At least one waypoint have a invalid value | </summary>
        INVALID_WAYPOINT=2, 
    	///<summary> Timeout in the authorizer process(in case it depends on network) | </summary>
        TIMEOUT=3, 
    	///<summary> Airspace of the mission in use by another vehicle, second result parameter can have the waypoint id that caused it to be denied. | </summary>
        AIRSPACE_IN_USE=4, 
    	///<summary> Weather is not good to fly | </summary>
        BAD_WEATHER=5, 
    
    };
    
    ///<summary> RC type </summary>
    public enum RC_TYPE: int /*default*/
    {
			///<summary> Spektrum DSM2 | </summary>
        SPEKTRUM_DSM2=0, 
    	///<summary> Spektrum DSMX | </summary>
        SPEKTRUM_DSMX=1, 
    
    };
    
    
    ///<summary> State flags for ADS-B transponder dynamic report </summary>
    public enum UAVIONIX_ADSB_OUT_DYNAMIC_STATE: ushort
    {
			///<summary>  | </summary>
        INTENT_CHANGE=1, 
    	///<summary>  | </summary>
        AUTOPILOT_ENABLED=2, 
    	///<summary>  | </summary>
        NICBARO_CROSSCHECKED=4, 
    	///<summary>  | </summary>
        ON_GROUND=8, 
    	///<summary>  | </summary>
        IDENT=16, 
    
    };
    
    ///<summary> Transceiver RF control flags for ADS-B transponder dynamic reports </summary>
    public enum UAVIONIX_ADSB_OUT_RF_SELECT: byte
    {
			///<summary>  | </summary>
        STANDBY=0, 
    	///<summary>  | </summary>
        RX_ENABLED=1, 
    	///<summary>  | </summary>
        TX_ENABLED=2, 
    
    };
    
    ///<summary> Status for ADS-B transponder dynamic input </summary>
    public enum UAVIONIX_ADSB_OUT_DYNAMIC_GPS_FIX: byte
    {
			///<summary>  | </summary>
        NONE_0=0, 
    	///<summary>  | </summary>
        NONE_1=1, 
    	///<summary>  | </summary>
        _2D=2, 
    	///<summary>  | </summary>
        _3D=3, 
    	///<summary>  | </summary>
        DGPS=4, 
    	///<summary>  | </summary>
        RTK=5, 
    
    };
    
    ///<summary> Status flags for ADS-B transponder dynamic output </summary>
    public enum UAVIONIX_ADSB_RF_HEALTH: byte
    {
			///<summary>  | </summary>
        INITIALIZING=0, 
    	///<summary>  | </summary>
        OK=1, 
    	///<summary>  | </summary>
        FAIL_TX=2, 
    	///<summary>  | </summary>
        FAIL_RX=16, 
    
    };
    
    ///<summary> Definitions for aircraft size </summary>
    public enum UAVIONIX_ADSB_OUT_CFG_AIRCRAFT_SIZE: byte
    {
			///<summary>  | </summary>
        NO_DATA=0, 
    	///<summary>  | </summary>
        L15M_W23M=1, 
    	///<summary>  | </summary>
        L25M_W28P5M=2, 
    	///<summary>  | </summary>
        L25_34M=3, 
    	///<summary>  | </summary>
        L35_33M=4, 
    	///<summary>  | </summary>
        L35_38M=5, 
    	///<summary>  | </summary>
        L45_39P5M=6, 
    	///<summary>  | </summary>
        L45_45M=7, 
    	///<summary>  | </summary>
        L55_45M=8, 
    	///<summary>  | </summary>
        L55_52M=9, 
    	///<summary>  | </summary>
        L65_59P5M=10, 
    	///<summary>  | </summary>
        L65_67M=11, 
    	///<summary>  | </summary>
        L75_W72P5M=12, 
    	///<summary>  | </summary>
        L75_W80M=13, 
    	///<summary>  | </summary>
        L85_W80M=14, 
    	///<summary>  | </summary>
        L85_W90M=15, 
    
    };
    
    ///<summary> GPS lataral offset encoding </summary>
    public enum UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LAT: byte
    {
			///<summary>  | </summary>
        NO_DATA=0, 
    	///<summary>  | </summary>
        LEFT_2M=1, 
    	///<summary>  | </summary>
        LEFT_4M=2, 
    	///<summary>  | </summary>
        LEFT_6M=3, 
    	///<summary>  | </summary>
        RIGHT_0M=4, 
    	///<summary>  | </summary>
        RIGHT_2M=5, 
    	///<summary>  | </summary>
        RIGHT_4M=6, 
    	///<summary>  | </summary>
        RIGHT_6M=7, 
    
    };
    
    ///<summary> GPS longitudinal offset encoding </summary>
    public enum UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LON: byte
    {
			///<summary>  | </summary>
        NO_DATA=0, 
    	///<summary>  | </summary>
        APPLIED_BY_SENSOR=1, 
    
    };
    
    ///<summary> Emergency status encoding </summary>
    public enum UAVIONIX_ADSB_EMERGENCY_STATUS: byte
    {
			///<summary>  | </summary>
        UAVIONIX_ADSB_OUT_NO_EMERGENCY=0, 
    	///<summary>  | </summary>
        UAVIONIX_ADSB_OUT_GENERAL_EMERGENCY=1, 
    	///<summary>  | </summary>
        UAVIONIX_ADSB_OUT_LIFEGUARD_EMERGENCY=2, 
    	///<summary>  | </summary>
        UAVIONIX_ADSB_OUT_MINIMUM_FUEL_EMERGENCY=3, 
    	///<summary>  | </summary>
        UAVIONIX_ADSB_OUT_NO_COMM_EMERGENCY=4, 
    	///<summary>  | </summary>
        UAVIONIX_ADSB_OUT_UNLAWFUL_INTERFERANCE_EMERGENCY=5, 
    	///<summary>  | </summary>
        UAVIONIX_ADSB_OUT_DOWNED_AIRCRAFT_EMERGENCY=6, 
    	///<summary>  | </summary>
        UAVIONIX_ADSB_OUT_RESERVED=7, 
    
    };
    
    
    ///<summary>  </summary>
    public enum ICAROUS_TRACK_BAND_TYPES: byte
    {
			///<summary>  | </summary>
        ICAROUS_TRACK_BAND_TYPE_NONE=0, 
    	///<summary>  | </summary>
        ICAROUS_TRACK_BAND_TYPE_NEAR=1, 
    	///<summary>  | </summary>
        ICAROUS_TRACK_BAND_TYPE_RECOVERY=2, 
    
    };
    
    ///<summary>  </summary>
    public enum ICAROUS_FMS_STATE: byte
    {
			///<summary>  | </summary>
        IDLE=0, 
    	///<summary>  | </summary>
        TAKEOFF=1, 
    	///<summary>  | </summary>
        CLIMB=2, 
    	///<summary>  | </summary>
        CRUISE=3, 
    	///<summary>  | </summary>
        APPROACH=4, 
    	///<summary>  | </summary>
        LAND=5, 
    
    };
    

    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> Offsets and calibrations values for hardware sensors. This makes it easier to debug the calibration process. </summary>
    public struct mavlink_sensor_offsets_t
    {
        /// <summary>magnetic declination (radians)  [rad] </summary>
        public  float mag_declination;
            /// <summary>raw pressure from barometer   </summary>
        public  int raw_press;
            /// <summary>raw temperature from barometer   </summary>
        public  int raw_temp;
            /// <summary>gyro X calibration   </summary>
        public  float gyro_cal_x;
            /// <summary>gyro Y calibration   </summary>
        public  float gyro_cal_y;
            /// <summary>gyro Z calibration   </summary>
        public  float gyro_cal_z;
            /// <summary>accel X calibration   </summary>
        public  float accel_cal_x;
            /// <summary>accel Y calibration   </summary>
        public  float accel_cal_y;
            /// <summary>accel Z calibration   </summary>
        public  float accel_cal_z;
            /// <summary>magnetometer X offset   </summary>
        public  short mag_ofs_x;
            /// <summary>magnetometer Y offset   </summary>
        public  short mag_ofs_y;
            /// <summary>magnetometer Z offset   </summary>
        public  short mag_ofs_z;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> Deprecated. Use MAV_CMD_PREFLIGHT_SET_SENSOR_OFFSETS instead. Set the magnetometer offsets </summary>
    public struct mavlink_set_mag_offsets_t
    {
        /// <summary>magnetometer X offset   </summary>
        public  short mag_ofs_x;
            /// <summary>magnetometer Y offset   </summary>
        public  short mag_ofs_y;
            /// <summary>magnetometer Z offset   </summary>
        public  short mag_ofs_z;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> state of APM memory </summary>
    public struct mavlink_meminfo_t
    {
        /// <summary>heap top   </summary>
        public  ushort brkval;
            /// <summary>free memory  [bytes] </summary>
        public  ushort freemem;
            /// <summary>free memory (32 bit)  [bytes] </summary>
        public  uint freemem32;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> raw ADC output </summary>
    public struct mavlink_ap_adc_t
    {
        /// <summary>ADC output 1   </summary>
        public  ushort adc1;
            /// <summary>ADC output 2   </summary>
        public  ushort adc2;
            /// <summary>ADC output 3   </summary>
        public  ushort adc3;
            /// <summary>ADC output 4   </summary>
        public  ushort adc4;
            /// <summary>ADC output 5   </summary>
        public  ushort adc5;
            /// <summary>ADC output 6   </summary>
        public  ushort adc6;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=15)]
    ///<summary> Configure on-board Camera Control System. </summary>
    public struct mavlink_digicam_configure_t
    {
        /// <summary>Correspondent value to given extra_param   </summary>
        public  float extra_value;
            /// <summary>Divisor number //e.g. 1000 means 1/1000 (0 means ignore)   </summary>
        public  ushort shutter_speed;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Mode enumeration from 1 to N //P, TV, AV, M, Etc (0 means ignore)   </summary>
        public  byte mode;
            /// <summary>F stop number x 10 //e.g. 28 means 2.8 (0 means ignore)   </summary>
        public  byte aperture;
            /// <summary>ISO enumeration from 1 to N //e.g. 80, 100, 200, Etc (0 means ignore)   </summary>
        public  byte iso;
            /// <summary>Exposure type enumeration from 1 to N (0 means ignore)   </summary>
        public  byte exposure_type;
            /// <summary>Command Identity (incremental loop: 0 to 255)//A command sent multiple times will be executed or pooled just once   </summary>
        public  byte command_id;
            /// <summary>Main engine cut-off time before camera trigger in seconds/10 (0 means no cut-off)  [ds] </summary>
        public  byte engine_cut_off;
            /// <summary>Extra parameters enumeration (0 means ignore)   </summary>
        public  byte extra_param;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=13)]
    ///<summary> Control on-board Camera Control System to take shots. </summary>
    public struct mavlink_digicam_control_t
    {
        /// <summary>Correspondent value to given extra_param   </summary>
        public  float extra_value;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>0: stop, 1: start or keep it up //Session control e.g. show/hide lens   </summary>
        public  byte session;
            /// <summary>1 to N //Zoom's absolute position (0 means ignore)   </summary>
        public  byte zoom_pos;
            /// <summary>-100 to 100 //Zooming step value to offset zoom from the current position   </summary>
        public  byte zoom_step;
            /// <summary>0: unlock focus or keep unlocked, 1: lock focus or keep locked, 3: re-lock focus   </summary>
        public  byte focus_lock;
            /// <summary>0: ignore, 1: shot or start filming   </summary>
        public  byte shot;
            /// <summary>Command Identity (incremental loop: 0 to 255)//A command sent multiple times will be executed or pooled just once   </summary>
        public  byte command_id;
            /// <summary>Extra parameters enumeration (0 means ignore)   </summary>
        public  byte extra_param;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Message to configure a camera mount, directional antenna, etc. </summary>
    public struct mavlink_mount_configure_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>mount operating mode (see MAV_MOUNT_MODE enum) MAV_MOUNT_MODE  </summary>
        public  /*MAV_MOUNT_MODE*/byte mount_mode;
            /// <summary>(1 = yes, 0 = no)   </summary>
        public  byte stab_roll;
            /// <summary>(1 = yes, 0 = no)   </summary>
        public  byte stab_pitch;
            /// <summary>(1 = yes, 0 = no)   </summary>
        public  byte stab_yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=15)]
    ///<summary> Message to control a camera mount, directional antenna, etc. </summary>
    public struct mavlink_mount_control_t
    {
        /// <summary>pitch(deg*100) or lat, depending on mount mode   </summary>
        public  int input_a;
            /// <summary>roll(deg*100) or lon depending on mount mode   </summary>
        public  int input_b;
            /// <summary>yaw(deg*100) or alt (in cm) depending on mount mode   </summary>
        public  int input_c;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>if "1" it will save current trimmed position on EEPROM (just valid for NEUTRAL and LANDING)   </summary>
        public  byte save_position;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Message with some status from APM to GCS about camera or antenna mount </summary>
    public struct mavlink_mount_status_t
    {
        /// <summary>pitch(deg*100)  [cdeg] </summary>
        public  int pointing_a;
            /// <summary>roll(deg*100)  [cdeg] </summary>
        public  int pointing_b;
            /// <summary>yaw(deg*100)  [cdeg] </summary>
        public  int pointing_c;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> A fence point. Used to set a point when from GCS -> MAV. Also used to return a point from MAV -> GCS </summary>
    public struct mavlink_fence_point_t
    {
        /// <summary>Latitude of point  [deg] </summary>
        public  float lat;
            /// <summary>Longitude of point  [deg] </summary>
        public  float lng;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>point index (first point is 1, 0 is for return point)   </summary>
        public  byte idx;
            /// <summary>total number of points (for sanity checking)   </summary>
        public  byte count;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Request a current fence point from MAV </summary>
    public struct mavlink_fence_fetch_point_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>point index (first point is 1, 0 is for return point)   </summary>
        public  byte idx;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> Status of geo-fencing. Sent in extended status stream when fencing enabled </summary>
    public struct mavlink_fence_status_t
    {
        /// <summary>time of last breach in milliseconds since boot  [ms] </summary>
        public  uint breach_time;
            /// <summary>number of fence breaches   </summary>
        public  ushort breach_count;
            /// <summary>0 if currently inside fence, 1 if outside   </summary>
        public  byte breach_status;
            /// <summary>last breach type (see FENCE_BREACH_* enum) FENCE_BREACH  </summary>
        public  /*FENCE_BREACH*/byte breach_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> Status of DCM attitude estimator </summary>
    public struct mavlink_ahrs_t
    {
        /// <summary>X gyro drift estimate rad/s  [rad/s] </summary>
        public  float omegaIx;
            /// <summary>Y gyro drift estimate rad/s  [rad/s] </summary>
        public  float omegaIy;
            /// <summary>Z gyro drift estimate rad/s  [rad/s] </summary>
        public  float omegaIz;
            /// <summary>average accel_weight   </summary>
        public  float accel_weight;
            /// <summary>average renormalisation value   </summary>
        public  float renorm_val;
            /// <summary>average error_roll_pitch value   </summary>
        public  float error_rp;
            /// <summary>average error_yaw value   </summary>
        public  float error_yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> Status of simulation environment, if used </summary>
    public struct mavlink_simstate_t
    {
        /// <summary>Roll angle (rad)  [rad] </summary>
        public  float roll;
            /// <summary>Pitch angle (rad)  [rad] </summary>
        public  float pitch;
            /// <summary>Yaw angle (rad)  [rad] </summary>
        public  float yaw;
            /// <summary>X acceleration m/s/s  [m/s/s] </summary>
        public  float xacc;
            /// <summary>Y acceleration m/s/s  [m/s/s] </summary>
        public  float yacc;
            /// <summary>Z acceleration m/s/s  [m/s/s] </summary>
        public  float zacc;
            /// <summary>Angular speed around X axis rad/s  [rad/s] </summary>
        public  float xgyro;
            /// <summary>Angular speed around Y axis rad/s  [rad/s] </summary>
        public  float ygyro;
            /// <summary>Angular speed around Z axis rad/s  [rad/s] </summary>
        public  float zgyro;
            /// <summary>Latitude in degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude in degrees * 1E7  [degE7] </summary>
        public  int lng;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Status of key hardware </summary>
    public struct mavlink_hwstatus_t
    {
        /// <summary>board voltage (mV)  [mV] </summary>
        public  ushort Vcc;
            /// <summary>I2C error count   </summary>
        public  byte I2Cerr;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> Status generated by radio </summary>
    public struct mavlink_radio_t
    {
        /// <summary>receive errors   </summary>
        public  ushort rxerrors;
            /// <summary>count of error corrected packets   </summary>
        public  ushort @fixed;
            /// <summary>local signal strength   </summary>
        public  byte rssi;
            /// <summary>remote signal strength   </summary>
        public  byte remrssi;
            /// <summary>how full the tx buffer is as a percentage  [%] </summary>
        public  byte txbuf;
            /// <summary>background noise level   </summary>
        public  byte noise;
            /// <summary>remote background noise level   </summary>
        public  byte remnoise;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> Status of AP_Limits. Sent in extended status stream when AP_Limits is enabled </summary>
    public struct mavlink_limits_status_t
    {
        /// <summary>time of last breach in milliseconds since boot  [ms] </summary>
        public  uint last_trigger;
            /// <summary>time of last recovery action in milliseconds since boot  [ms] </summary>
        public  uint last_action;
            /// <summary>time of last successful recovery in milliseconds since boot  [ms] </summary>
        public  uint last_recovery;
            /// <summary>time of last all-clear in milliseconds since boot  [ms] </summary>
        public  uint last_clear;
            /// <summary>number of fence breaches   </summary>
        public  ushort breach_count;
            /// <summary>state of AP_Limits, (see enum LimitState, LIMITS_STATE) LIMITS_STATE  </summary>
        public  /*LIMITS_STATE*/byte limits_state;
            /// <summary>AP_Limit_Module bitfield of enabled modules, (see enum moduleid or LIMIT_MODULE) LIMIT_MODULE  bitmask</summary>
        public  /*LIMIT_MODULE*/byte mods_enabled;
            /// <summary>AP_Limit_Module bitfield of required modules, (see enum moduleid or LIMIT_MODULE) LIMIT_MODULE  bitmask</summary>
        public  /*LIMIT_MODULE*/byte mods_required;
            /// <summary>AP_Limit_Module bitfield of triggered modules, (see enum moduleid or LIMIT_MODULE) LIMIT_MODULE  bitmask</summary>
        public  /*LIMIT_MODULE*/byte mods_triggered;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> Wind estimation </summary>
    public struct mavlink_wind_t
    {
        /// <summary>wind direction that wind is coming from (degrees)  [deg] </summary>
        public  float direction;
            /// <summary>wind speed in ground plane (m/s)  [m/s] </summary>
        public  float speed;
            /// <summary>vertical wind speed (m/s)  [m/s] </summary>
        public  float speed_z;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    ///<summary> Data packet, size 16 </summary>
    public struct mavlink_data16_t
    {
        /// <summary>data type   </summary>
        public  byte type;
            /// <summary>data length  [bytes] </summary>
        public  byte len;
            /// <summary>raw data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=34)]
    ///<summary> Data packet, size 32 </summary>
    public struct mavlink_data32_t
    {
        /// <summary>data type   </summary>
        public  byte type;
            /// <summary>data length  [bytes] </summary>
        public  byte len;
            /// <summary>raw data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=66)]
    ///<summary> Data packet, size 64 </summary>
    public struct mavlink_data64_t
    {
        /// <summary>data type   </summary>
        public  byte type;
            /// <summary>data length  [bytes] </summary>
        public  byte len;
            /// <summary>raw data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=64)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=98)]
    ///<summary> Data packet, size 96 </summary>
    public struct mavlink_data96_t
    {
        /// <summary>data type   </summary>
        public  byte type;
            /// <summary>data length  [bytes] </summary>
        public  byte len;
            /// <summary>raw data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=96)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> Rangefinder reporting </summary>
    public struct mavlink_rangefinder_t
    {
        /// <summary>distance in meters  [m] </summary>
        public  float distance;
            /// <summary>raw voltage if available, zero otherwise  [V] </summary>
        public  float voltage;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=48)]
    ///<summary> Airspeed auto-calibration </summary>
    public struct mavlink_airspeed_autocal_t
    {
        /// <summary>GPS velocity north m/s  [m/s] </summary>
        public  float vx;
            /// <summary>GPS velocity east m/s  [m/s] </summary>
        public  float vy;
            /// <summary>GPS velocity down m/s  [m/s] </summary>
        public  float vz;
            /// <summary>Differential pressure pascals  [Pa] </summary>
        public  float diff_pressure;
            /// <summary>Estimated to true airspeed ratio   </summary>
        public  float EAS2TAS;
            /// <summary>Airspeed ratio   </summary>
        public  float ratio;
            /// <summary>EKF state x   </summary>
        public  float state_x;
            /// <summary>EKF state y   </summary>
        public  float state_y;
            /// <summary>EKF state z   </summary>
        public  float state_z;
            /// <summary>EKF Pax   </summary>
        public  float Pax;
            /// <summary>EKF Pby   </summary>
        public  float Pby;
            /// <summary>EKF Pcz   </summary>
        public  float Pcz;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=19)]
    ///<summary> A rally point. Used to set a point when from GCS -> MAV. Also used to return a point from MAV -> GCS </summary>
    public struct mavlink_rally_point_t
    {
        /// <summary>Latitude of point in degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude of point in degrees * 1E7  [degE7] </summary>
        public  int lng;
            /// <summary>Transit / loiter altitude in meters relative to home  [m] </summary>
        public  short alt;
            /// <summary>Break altitude in meters relative to home  [m] </summary>
        public  short break_alt;
            /// <summary>Heading to aim for when landing. In centi-degrees.  [cdeg] </summary>
        public  ushort land_dir;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>point index (first point is 0)   </summary>
        public  byte idx;
            /// <summary>total number of points (for sanity checking)   </summary>
        public  byte count;
            /// <summary>See RALLY_FLAGS enum for definition of the bitmask. RALLY_FLAGS  bitmask</summary>
        public  /*RALLY_FLAGS*/byte flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Request a current rally point from MAV. MAV should respond with a RALLY_POINT message. MAV should not respond if the request is invalid. </summary>
    public struct mavlink_rally_fetch_point_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>point index (first point is 0)   </summary>
        public  byte idx;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Status of compassmot calibration </summary>
    public struct mavlink_compassmot_status_t
    {
        /// <summary>current (Ampere)  [A] </summary>
        public  float current;
            /// <summary>Motor Compensation X   </summary>
        public  float CompensationX;
            /// <summary>Motor Compensation Y   </summary>
        public  float CompensationY;
            /// <summary>Motor Compensation Z   </summary>
        public  float CompensationZ;
            /// <summary>throttle (percent*10)  [d%] </summary>
        public  ushort throttle;
            /// <summary>interference (percent)  [%] </summary>
        public  ushort interference;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=24)]
    ///<summary> Status of secondary AHRS filter if available </summary>
    public struct mavlink_ahrs2_t
    {
        /// <summary>Roll angle (rad)  [rad] </summary>
        public  float roll;
            /// <summary>Pitch angle (rad)  [rad] </summary>
        public  float pitch;
            /// <summary>Yaw angle (rad)  [rad] </summary>
        public  float yaw;
            /// <summary>Altitude (MSL)  [m] </summary>
        public  float altitude;
            /// <summary>Latitude in degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude in degrees * 1E7  [degE7] </summary>
        public  int lng;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=29)]
    ///<summary> Camera Event </summary>
    public struct mavlink_camera_status_t
    {
        /// <summary>Image timestamp (microseconds since UNIX epoch, according to camera clock)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Parameter 1 (meaning depends on event, see CAMERA_STATUS_TYPES enum)   </summary>
        public  float p1;
            /// <summary>Parameter 2 (meaning depends on event, see CAMERA_STATUS_TYPES enum)   </summary>
        public  float p2;
            /// <summary>Parameter 3 (meaning depends on event, see CAMERA_STATUS_TYPES enum)   </summary>
        public  float p3;
            /// <summary>Parameter 4 (meaning depends on event, see CAMERA_STATUS_TYPES enum)   </summary>
        public  float p4;
            /// <summary>Image index   </summary>
        public  ushort img_idx;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Camera ID   </summary>
        public  byte cam_idx;
            /// <summary>See CAMERA_STATUS_TYPES enum for definition of the bitmask CAMERA_STATUS_TYPES  </summary>
        public  /*CAMERA_STATUS_TYPES*/byte event_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=47)]
    ///<summary> Camera Capture Feedback </summary>
    public struct mavlink_camera_feedback_t
    {
        /// <summary>Image timestamp (microseconds since UNIX epoch), as passed in by CAMERA_STATUS message (or autopilot if no CCB)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Latitude in (deg * 1E7)  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude in (deg * 1E7)  [degE7] </summary>
        public  int lng;
            /// <summary>Altitude Absolute (meters AMSL)  [m] </summary>
        public  float alt_msl;
            /// <summary>Altitude Relative (meters above HOME location)  [m] </summary>
        public  float alt_rel;
            /// <summary>Camera Roll angle (earth frame, degrees, +-180)  [deg] </summary>
        public  float roll;
            /// <summary>Camera Pitch angle (earth frame, degrees, +-180)  [deg] </summary>
        public  float pitch;
            /// <summary>Camera Yaw (earth frame, degrees, 0-360, true)  [deg] </summary>
        public  float yaw;
            /// <summary>Focal Length (mm)  [mm] </summary>
        public  float foc_len;
            /// <summary>Image index   </summary>
        public  ushort img_idx;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Camera ID   </summary>
        public  byte cam_idx;
            /// <summary>See CAMERA_FEEDBACK_FLAGS enum for definition of the bitmask CAMERA_FEEDBACK_FLAGS  </summary>
        public  /*CAMERA_FEEDBACK_FLAGS*/byte flags;
            /// <summary>Completed image captures   </summary>
        public  ushort completed_captures;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> Deprecated. Use BATTERY_STATUS instead. 2nd Battery status </summary>
    public struct mavlink_battery2_t
    {
        /// <summary>voltage in millivolts  [mV] </summary>
        public  ushort voltage;
            /// <summary>Battery current, in centiamperes (1 = 10 milliampere), -1: autopilot does not measure the current  [cA] </summary>
        public  short current_battery;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=40)]
    ///<summary> Status of third AHRS filter if available. This is for ANU research group (Ali and Sean) </summary>
    public struct mavlink_ahrs3_t
    {
        /// <summary>Roll angle (rad)  [rad] </summary>
        public  float roll;
            /// <summary>Pitch angle (rad)  [rad] </summary>
        public  float pitch;
            /// <summary>Yaw angle (rad)  [rad] </summary>
        public  float yaw;
            /// <summary>Altitude (MSL)  [m] </summary>
        public  float altitude;
            /// <summary>Latitude in degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude in degrees * 1E7  [degE7] </summary>
        public  int lng;
            /// <summary>test variable1   </summary>
        public  float v1;
            /// <summary>test variable2   </summary>
        public  float v2;
            /// <summary>test variable3   </summary>
        public  float v3;
            /// <summary>test variable4   </summary>
        public  float v4;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Request the autopilot version from the system/component. </summary>
    public struct mavlink_autopilot_version_request_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=206)]
    ///<summary> Send a block of log data to remote location </summary>
    public struct mavlink_remote_log_data_block_t
    {
        /// <summary>log data block sequence number MAV_REMOTE_LOG_DATA_BLOCK_COMMANDS  </summary>
        public  /*MAV_REMOTE_LOG_DATA_BLOCK_COMMANDS*/uint seqno;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>log data block   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=200)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=7)]
    ///<summary> Send Status of each log block that autopilot board might have sent </summary>
    public struct mavlink_remote_log_block_status_t
    {
        /// <summary>log data block sequence number   </summary>
        public  uint seqno;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>log data block status MAV_REMOTE_LOG_DATA_BLOCK_STATUSES  </summary>
        public  /*MAV_REMOTE_LOG_DATA_BLOCK_STATUSES*/byte status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=29)]
    ///<summary> Control vehicle LEDs </summary>
    public struct mavlink_led_control_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Instance (LED instance to control or 255 for all LEDs)   </summary>
        public  byte instance;
            /// <summary>Pattern (see LED_PATTERN_ENUM)   </summary>
        public  byte pattern;
            /// <summary>Custom Byte Length   </summary>
        public  byte custom_len;
            /// <summary>Custom Bytes   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=24)]
		public byte[] custom_bytes;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=27)]
    ///<summary> Reports progress of compass calibration. </summary>
    public struct mavlink_mag_cal_progress_t
    {
        /// <summary>Body frame direction vector for display   </summary>
        public  float direction_x;
            /// <summary>Body frame direction vector for display   </summary>
        public  float direction_y;
            /// <summary>Body frame direction vector for display   </summary>
        public  float direction_z;
            /// <summary>Compass being calibrated   </summary>
        public  byte compass_id;
            /// <summary>Bitmask of compasses being calibrated   </summary>
        public  byte cal_mask;
            /// <summary>Status (see MAG_CAL_STATUS enum) MAG_CAL_STATUS  </summary>
        public  /*MAG_CAL_STATUS*/byte cal_status;
            /// <summary>Attempt number   </summary>
        public  byte attempt;
            /// <summary>Completion percentage  [%] </summary>
        public  byte completion_pct;
            /// <summary>Bitmask of sphere sections (see http://en.wikipedia.org/wiki/Geodesic_grid)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] completion_mask;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=50)]
    ///<summary> Reports results of completed compass calibration. Sent until MAG_CAL_ACK received. </summary>
    public struct mavlink_mag_cal_report_t
    {
        /// <summary>RMS milligauss residuals  [mgauss] </summary>
        public  float fitness;
            /// <summary>X offset   </summary>
        public  float ofs_x;
            /// <summary>Y offset   </summary>
        public  float ofs_y;
            /// <summary>Z offset   </summary>
        public  float ofs_z;
            /// <summary>X diagonal (matrix 11)   </summary>
        public  float diag_x;
            /// <summary>Y diagonal (matrix 22)   </summary>
        public  float diag_y;
            /// <summary>Z diagonal (matrix 33)   </summary>
        public  float diag_z;
            /// <summary>X off-diagonal (matrix 12 and 21)   </summary>
        public  float offdiag_x;
            /// <summary>Y off-diagonal (matrix 13 and 31)   </summary>
        public  float offdiag_y;
            /// <summary>Z off-diagonal (matrix 32 and 23)   </summary>
        public  float offdiag_z;
            /// <summary>Compass being calibrated   </summary>
        public  byte compass_id;
            /// <summary>Bitmask of compasses being calibrated   </summary>
        public  byte cal_mask;
            /// <summary>Status (see MAG_CAL_STATUS enum) MAG_CAL_STATUS  </summary>
        public  /*MAG_CAL_STATUS*/byte cal_status;
            /// <summary>0=requires a MAV_CMD_DO_ACCEPT_MAG_CAL, 1=saved to parameters   </summary>
        public  byte autosaved;
            /// <summary>Confidence in orientation (higher is better)   </summary>
        public  float orientation_confidence;
            /// <summary>orientation before calibration    </summary>
        public  byte old_orientation;
            /// <summary>orientation before calibration   </summary>
        public  byte new_orientation;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    ///<summary> EKF Status message including flags and variances </summary>
    public struct mavlink_ekf_status_report_t
    {
        /// <summary>Velocity variance   </summary>
        public  float velocity_variance;
            /// <summary>Horizontal Position variance   </summary>
        public  float pos_horiz_variance;
            /// <summary>Vertical Position variance   </summary>
        public  float pos_vert_variance;
            /// <summary>Compass variance   </summary>
        public  float compass_variance;
            /// <summary>Terrain Altitude variance   </summary>
        public  float terrain_alt_variance;
            /// <summary>Flags EKF_STATUS_FLAGS  </summary>
        public  /*EKF_STATUS_FLAGS*/ushort flags;
            /// <summary>Airspeed variance   </summary>
        public  float airspeed_variance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=25)]
    ///<summary> PID tuning information </summary>
    public struct mavlink_pid_tuning_t
    {
        /// <summary>desired rate (degrees/s)  [deg/s] </summary>
        public  float desired;
            /// <summary>achieved rate (degrees/s)  [deg/s] </summary>
        public  float achieved;
            /// <summary>FF component   </summary>
        public  float FF;
            /// <summary>P component   </summary>
        public  float P;
            /// <summary>I component   </summary>
        public  float I;
            /// <summary>D component   </summary>
        public  float D;
            /// <summary>axis PID_TUNING_AXIS  </summary>
        public  /*PID_TUNING_AXIS*/byte axis;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    ///<summary> Deepstall path planning </summary>
    public struct mavlink_deepstall_t
    {
        /// <summary>Landing latitude (deg * 1E7)  [degE7] </summary>
        public  int landing_lat;
            /// <summary>Landing longitude (deg * 1E7)  [degE7] </summary>
        public  int landing_lon;
            /// <summary>Final heading start point, latitude (deg * 1E7)  [degE7] </summary>
        public  int path_lat;
            /// <summary>Final heading start point, longitude (deg * 1E7)  [degE7] </summary>
        public  int path_lon;
            /// <summary>Arc entry point, latitude (deg * 1E7)  [degE7] </summary>
        public  int arc_entry_lat;
            /// <summary>Arc entry point, longitude (deg * 1E7)  [degE7] </summary>
        public  int arc_entry_lon;
            /// <summary>Altitude (meters)  [m] </summary>
        public  float altitude;
            /// <summary>Distance the aircraft expects to travel during the deepstall  [m] </summary>
        public  float expected_travel_distance;
            /// <summary>Deepstall cross track error in meters (only valid when in DEEPSTALL_STAGE_LAND)  [m] </summary>
        public  float cross_track_error;
            /// <summary>Deepstall stage, see enum MAV_DEEPSTALL_STAGE DEEPSTALL_STAGE  </summary>
        public  /*DEEPSTALL_STAGE*/byte stage;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> 3 axis gimbal mesuraments </summary>
    public struct mavlink_gimbal_report_t
    {
        /// <summary>Time since last update (seconds)  [s] </summary>
        public  float delta_time;
            /// <summary>Delta angle X (radians)  [rad] </summary>
        public  float delta_angle_x;
            /// <summary>Delta angle Y (radians)  [rad] </summary>
        public  float delta_angle_y;
            /// <summary>Delta angle X (radians)  [rad] </summary>
        public  float delta_angle_z;
            /// <summary>Delta velocity X (m/s)  [m/s] </summary>
        public  float delta_velocity_x;
            /// <summary>Delta velocity Y (m/s)  [m/s] </summary>
        public  float delta_velocity_y;
            /// <summary>Delta velocity Z (m/s)  [m/s] </summary>
        public  float delta_velocity_z;
            /// <summary>Joint ROLL (radians)  [rad] </summary>
        public  float joint_roll;
            /// <summary>Joint EL (radians)  [rad] </summary>
        public  float joint_el;
            /// <summary>Joint AZ (radians)  [rad] </summary>
        public  float joint_az;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Control message for rate gimbal </summary>
    public struct mavlink_gimbal_control_t
    {
        /// <summary>Demanded angular rate X (rad/s)  [rad/s] </summary>
        public  float demanded_rate_x;
            /// <summary>Demanded angular rate Y (rad/s)  [rad/s] </summary>
        public  float demanded_rate_y;
            /// <summary>Demanded angular rate Z (rad/s)  [rad/s] </summary>
        public  float demanded_rate_z;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> 100 Hz gimbal torque command telemetry </summary>
    public struct mavlink_gimbal_torque_cmd_report_t
    {
        /// <summary>Roll Torque Command   </summary>
        public  short rl_torque_cmd;
            /// <summary>Elevation Torque Command   </summary>
        public  short el_torque_cmd;
            /// <summary>Azimuth Torque Command   </summary>
        public  short az_torque_cmd;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Heartbeat from a HeroBus attached GoPro </summary>
    public struct mavlink_gopro_heartbeat_t
    {
        /// <summary>Status GOPRO_HEARTBEAT_STATUS  </summary>
        public  /*GOPRO_HEARTBEAT_STATUS*/byte status;
            /// <summary>Current capture mode GOPRO_CAPTURE_MODE  </summary>
        public  /*GOPRO_CAPTURE_MODE*/byte capture_mode;
            /// <summary>additional status bits GOPRO_HEARTBEAT_FLAGS  bitmask</summary>
        public  /*GOPRO_HEARTBEAT_FLAGS*/byte flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Request a GOPRO_COMMAND response from the GoPro </summary>
    public struct mavlink_gopro_get_request_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Command ID GOPRO_COMMAND  </summary>
        public  /*GOPRO_COMMAND*/byte cmd_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Response from a GOPRO_COMMAND get request </summary>
    public struct mavlink_gopro_get_response_t
    {
        /// <summary>Command ID GOPRO_COMMAND  </summary>
        public  /*GOPRO_COMMAND*/byte cmd_id;
            /// <summary>Status GOPRO_REQUEST_STATUS  </summary>
        public  /*GOPRO_REQUEST_STATUS*/byte status;
            /// <summary>Value   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] value;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=7)]
    ///<summary> Request to set a GOPRO_COMMAND with a desired </summary>
    public struct mavlink_gopro_set_request_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Command ID GOPRO_COMMAND  </summary>
        public  /*GOPRO_COMMAND*/byte cmd_id;
            /// <summary>Value   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] value;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Response from a GOPRO_COMMAND set request </summary>
    public struct mavlink_gopro_set_response_t
    {
        /// <summary>Command ID GOPRO_COMMAND  </summary>
        public  /*GOPRO_COMMAND*/byte cmd_id;
            /// <summary>Status GOPRO_REQUEST_STATUS  </summary>
        public  /*GOPRO_REQUEST_STATUS*/byte status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> RPM sensor output </summary>
    public struct mavlink_rpm_t
    {
        /// <summary>RPM Sensor1   </summary>
        public  float rpm1;
            /// <summary>RPM Sensor2   </summary>
        public  float rpm2;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    ///<summary> Read registers for a device </summary>
    public struct mavlink_device_op_read_t
    {
        /// <summary>request ID - copied to reply   </summary>
        public  uint request_id;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>The bus type DEVICE_OP_BUSTYPE  </summary>
        public  /*DEVICE_OP_BUSTYPE*/byte bustype;
            /// <summary>Bus number   </summary>
        public  byte bus;
            /// <summary>Bus address   </summary>
        public  byte address;
            /// <summary>Name of device on bus (for SPI)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=40)]
		public byte[] busname;
            /// <summary>First register to read   </summary>
        public  byte regstart;
            /// <summary>count of registers to read   </summary>
        public  byte count;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=135)]
    ///<summary> Read registers reply </summary>
    public struct mavlink_device_op_read_reply_t
    {
        /// <summary>request ID - copied from request   </summary>
        public  uint request_id;
            /// <summary>0 for success, anything else is failure code   </summary>
        public  byte result;
            /// <summary>starting register   </summary>
        public  byte regstart;
            /// <summary>count of bytes read   </summary>
        public  byte count;
            /// <summary>reply data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=128)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=179)]
    ///<summary> Write registers for a device </summary>
    public struct mavlink_device_op_write_t
    {
        /// <summary>request ID - copied to reply   </summary>
        public  uint request_id;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>The bus type DEVICE_OP_BUSTYPE  </summary>
        public  /*DEVICE_OP_BUSTYPE*/byte bustype;
            /// <summary>Bus number   </summary>
        public  byte bus;
            /// <summary>Bus address   </summary>
        public  byte address;
            /// <summary>Name of device on bus (for SPI)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=40)]
		public byte[] busname;
            /// <summary>First register to write   </summary>
        public  byte regstart;
            /// <summary>count of registers to write   </summary>
        public  byte count;
            /// <summary>write data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=128)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=5)]
    ///<summary> Write registers reply </summary>
    public struct mavlink_device_op_write_reply_t
    {
        /// <summary>request ID - copied from request   </summary>
        public  uint request_id;
            /// <summary>0 for success, anything else is failure code   </summary>
        public  byte result;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=49)]
    ///<summary> Adaptive Controller tuning information </summary>
    public struct mavlink_adap_tuning_t
    {
        /// <summary>desired rate (degrees/s)  [deg/s] </summary>
        public  float desired;
            /// <summary>achieved rate (degrees/s)  [deg/s] </summary>
        public  float achieved;
            /// <summary>error between model and vehicle   </summary>
        public  float error;
            /// <summary>theta estimated state predictor   </summary>
        public  float theta;
            /// <summary>omega estimated state predictor   </summary>
        public  float omega;
            /// <summary>sigma estimated state predictor   </summary>
        public  float sigma;
            /// <summary>theta derivative   </summary>
        public  float theta_dot;
            /// <summary>omega derivative   </summary>
        public  float omega_dot;
            /// <summary>sigma derivative   </summary>
        public  float sigma_dot;
            /// <summary>projection operator value   </summary>
        public  float f;
            /// <summary>projection operator derivative   </summary>
        public  float f_dot;
            /// <summary>u adaptive controlled output command   </summary>
        public  float u;
            /// <summary>axis PID_TUNING_AXIS  </summary>
        public  /*PID_TUNING_AXIS*/byte axis;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> camera vision based attitude and position deltas </summary>
    public struct mavlink_vision_position_delta_t
    {
        /// <summary>Timestamp (microseconds, synced to UNIX time or since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Time in microseconds since the last reported camera frame  [us] </summary>
        public  ulong time_delta_usec;
            /// <summary>Defines a rotation vector in body frame that rotates the vehicle from the previous to the current orientation   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] angle_delta;
            /// <summary>Change in position in meters from previous to current frame rotated into body frame (0=forward, 1=right, 2=down)  [m] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] position_delta;
            /// <summary>normalised confidence value from 0 to 100  [%] </summary>
        public  float confidence;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=16)]
    ///<summary> Angle of Attack and Side Slip Angle </summary>
    public struct mavlink_aoa_ssa_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Angle of Attack (degrees)  [deg] </summary>
        public  float AOA;
            /// <summary>Side Slip Angle (degrees)  [deg] </summary>
        public  float SSA;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> ESC Telemetry Data for ESCs 1 to 4, matching data sent by BLHeli ESCs </summary>
    public struct mavlink_esc_telemetry_1_to_4_t
    {
        /// <summary>Voltage  [cV] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] voltage;
            /// <summary>Current  [cA] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] current;
            /// <summary>Total current  [mAh] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] totalcurrent;
            /// <summary>RPM (eRPM)  [rpm] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] rpm;
            /// <summary>count of telemetry packets received (wraps at 65535)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] count;
            /// <summary>Temperature  [degC] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> ESC Telemetry Data for ESCs 5 to 8, matching data sent by BLHeli ESCs </summary>
    public struct mavlink_esc_telemetry_5_to_8_t
    {
        /// <summary>Voltage  [cV] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] voltage;
            /// <summary>Current  [cA] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] current;
            /// <summary>Total current  [mAh] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] totalcurrent;
            /// <summary>RPM (eRPM)  [rpm] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] rpm;
            /// <summary>count of telemetry packets received (wraps at 65535)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] count;
            /// <summary>Temperature  [degC] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> ESC Telemetry Data for ESCs 9 to 12, matching data sent by BLHeli ESCs </summary>
    public struct mavlink_esc_telemetry_9_to_12_t
    {
        /// <summary>Voltage  [cV] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] voltage;
            /// <summary>Current  [cA] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] current;
            /// <summary>Total current  [mAh] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] totalcurrent;
            /// <summary>RPM (eRPM)  [rpm] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] rpm;
            /// <summary>count of telemetry packets received (wraps at 65535)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] count;
            /// <summary>Temperature  [degC] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> The heartbeat message shows that a system is present and responding. The type of the MAV and Autopilot hardware allow the receiving system to treat further messages from this system appropriate (e.g. by laying out the user interface based on the autopilot). </summary>
    public struct mavlink_heartbeat_t
    {
        /// <summary>A bitfield for use for autopilot-specific flags   </summary>
        public  uint custom_mode;
            /// <summary>Type of the MAV (quadrotor, helicopter, etc., up to 15 types, defined in MAV_TYPE ENUM) MAV_TYPE  </summary>
        public  /*MAV_TYPE*/byte type;
            /// <summary>Autopilot type / class. defined in MAV_AUTOPILOT ENUM MAV_AUTOPILOT  </summary>
        public  /*MAV_AUTOPILOT*/byte autopilot;
            /// <summary>System mode bitfield, as defined by MAV_MODE_FLAG enum MAV_MODE_FLAG  bitmask</summary>
        public  /*MAV_MODE_FLAG*/byte base_mode;
            /// <summary>System status flag, as defined by MAV_STATE enum MAV_STATE  </summary>
        public  /*MAV_STATE*/byte system_status;
            /// <summary>MAVLink version, not writable by user, gets added by protocol because of magic data type: uint8_t_mavlink_version   </summary>
        public  byte mavlink_version;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=31)]
    ///<summary> The general system state. If the system is following the MAVLink standard, the system state is mainly defined by three orthogonal states/modes: The system mode, which is either LOCKED (motors shut down and locked), MANUAL (system under RC control), GUIDED (system with autonomous position control, position setpoint controlled manually) or AUTO (system guided by path/waypoint planner). The NAV_MODE defined the current flight state: LIFTOFF (often an open-loop maneuver), LANDING, WAYPOINTS or VECTOR. This represents the internal navigation state machine. The system status shows whether the system is currently active or not and if an emergency occured. During the CRITICAL and EMERGENCY states the MAV is still considered to be active, but should start emergency procedures autonomously. After a failure occured it should first move from active to critical to allow manual intervention and then move to emergency after a certain timeout. </summary>
    public struct mavlink_sys_status_t
    {
        /// <summary>Bitmask showing which onboard controllers and sensors are present. Value of 0: not present. Value of 1: present. Indices defined by ENUM MAV_SYS_STATUS_SENSOR MAV_SYS_STATUS_SENSOR  bitmask</summary>
        public  /*MAV_SYS_STATUS_SENSOR*/uint onboard_control_sensors_present;
            /// <summary>Bitmask showing which onboard controllers and sensors are enabled:  Value of 0: not enabled. Value of 1: enabled. Indices defined by ENUM MAV_SYS_STATUS_SENSOR MAV_SYS_STATUS_SENSOR  bitmask</summary>
        public  /*MAV_SYS_STATUS_SENSOR*/uint onboard_control_sensors_enabled;
            /// <summary>Bitmask showing which onboard controllers and sensors are operational or have an error:  Value of 0: not enabled. Value of 1: enabled. Indices defined by ENUM MAV_SYS_STATUS_SENSOR MAV_SYS_STATUS_SENSOR  bitmask</summary>
        public  /*MAV_SYS_STATUS_SENSOR*/uint onboard_control_sensors_health;
            /// <summary>Maximum usage in percent of the mainloop time, (0%: 0, 100%: 1000) should be always below 1000  [d%] </summary>
        public  ushort load;
            /// <summary>Battery voltage, in millivolts (1 = 1 millivolt)  [mV] </summary>
        public  ushort voltage_battery;
            /// <summary>Battery current, in 10*milliamperes (1 = 10 milliampere), -1: autopilot does not measure the current  [cA] </summary>
        public  short current_battery;
            /// <summary>Communication drops in percent, (0%: 0, 100%: 10'000), (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV)  [c%] </summary>
        public  ushort drop_rate_comm;
            /// <summary>Communication errors (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV)   </summary>
        public  ushort errors_comm;
            /// <summary>Autopilot-specific errors   </summary>
        public  ushort errors_count1;
            /// <summary>Autopilot-specific errors   </summary>
        public  ushort errors_count2;
            /// <summary>Autopilot-specific errors   </summary>
        public  ushort errors_count3;
            /// <summary>Autopilot-specific errors   </summary>
        public  ushort errors_count4;
            /// <summary>Remaining battery energy: (0%: 0, 100%: 100), -1: autopilot estimate the remaining battery  [%] </summary>
        public  byte battery_remaining;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> The system time is the time of the master clock, typically the computer clock of the main onboard computer. </summary>
    public struct mavlink_system_time_t
    {
        /// <summary>Timestamp of the master clock in microseconds since UNIX epoch.  [us] </summary>
        public  ulong time_unix_usec;
            /// <summary>Timestamp of the component clock since boot time in milliseconds.  [ms] </summary>
        public  uint time_boot_ms;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> A ping message either requesting or responding to a ping. This allows to measure the system latencies, including serial port, radio modem and UDP connections. </summary>
    public struct mavlink_ping_t
    {
        /// <summary>Unix timestamp in microseconds or since system boot if smaller than MAVLink epoch (1.1.2009)  [us] </summary>
        public  ulong time_usec;
            /// <summary>PING sequence   </summary>
        public  uint seq;
            /// <summary>0: request ping from all receiving systems, if greater than 0: message is a ping response and number is the system id of the requesting system   </summary>
        public  byte target_system;
            /// <summary>0: request ping from all receiving components, if greater than 0: message is a ping response and number is the system id of the requesting system   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> Request to control this MAV </summary>
    public struct mavlink_change_operator_control_t
    {
        /// <summary>System the GCS requests control for   </summary>
        public  byte target_system;
            /// <summary>0: request control of this MAV, 1: Release control of this MAV   </summary>
        public  byte control_request;
            /// <summary>0: key as plaintext, 1-255: future, different hashing/encryption variants. The GCS should in general use the safest mode possible initially and then gradually move down the encryption level if it gets a NACK message indicating an encryption mismatch.  [rad] </summary>
        public  byte version;
            /// <summary>Password / Key, depending on version plaintext or encrypted. 25 or less characters, NULL terminated. The characters may involve A-Z, a-z, 0-9, and "!?,.-"   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=25)]
		public byte[] passkey;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Accept / deny control of this MAV </summary>
    public struct mavlink_change_operator_control_ack_t
    {
        /// <summary>ID of the GCS this message    </summary>
        public  byte gcs_system_id;
            /// <summary>0: request control of this MAV, 1: Release control of this MAV   </summary>
        public  byte control_request;
            /// <summary>0: ACK, 1: NACK: Wrong passkey, 2: NACK: Unsupported passkey encryption method, 3: NACK: Already under control   </summary>
        public  byte ack;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    ///<summary> Emit an encrypted signature / key identifying this system. PLEASE NOTE: This protocol has been kept simple, so transmitting the key requires an encrypted channel for true safety. </summary>
    public struct mavlink_auth_key_t
    {
        /// <summary>key   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] key;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> THIS INTERFACE IS DEPRECATED. USE COMMAND_LONG with MAV_CMD_DO_SET_MODE INSTEAD. Set the system mode, as defined by enum MAV_MODE. There is no target component id as the mode is by definition for the overall aircraft, not only for one component. </summary>
    public struct mavlink_set_mode_t
    {
        /// <summary>The new autopilot-specific mode. This field can be ignored by an autopilot.   </summary>
        public  uint custom_mode;
            /// <summary>The system setting the mode   </summary>
        public  byte target_system;
            /// <summary>The new base mode MAV_MODE  </summary>
        public  /*MAV_MODE*/byte base_mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Request to read the onboard parameter with the param_id string id. Onboard parameters are stored as key[const char*] -> value[float]. This allows to send a parameter to any other component (such as the GCS) without the need of previous knowledge of possible parameter names. Thus the same GCS can store different parameters for different autopilots. See also https://mavlink.io/en/protocol/parameter.html for a full documentation of QGroundControl and IMU code. </summary>
    public struct mavlink_param_request_read_t
    {
        /// <summary>Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored)   </summary>
        public  short param_index;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Request all parameters of this component. After this request, all parameters are emitted. </summary>
    public struct mavlink_param_request_list_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=25)]
    ///<summary> Emit the value of a onboard parameter. The inclusion of param_count and param_index in the message allows the recipient to keep track of received parameters and allows him to re-request missing parameters after a loss or timeout. </summary>
    public struct mavlink_param_value_t
    {
        /// <summary>Onboard parameter value   </summary>
        public  float param_value;
            /// <summary>Total number of onboard parameters   </summary>
        public  ushort param_count;
            /// <summary>Index of this onboard parameter   </summary>
        public  ushort param_index;
            /// <summary>Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
            /// <summary>Onboard parameter type: see the MAV_PARAM_TYPE enum for supported data types. MAV_PARAM_TYPE  </summary>
        public  /*MAV_PARAM_TYPE*/byte param_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=23)]
    ///<summary> Set a parameter value TEMPORARILY to RAM. It will be reset to default on system reboot. Send the ACTION MAV_ACTION_STORAGE_WRITE to PERMANENTLY write the RAM contents to EEPROM. IMPORTANT: The receiving component should acknowledge the new parameter value by sending a param_value message to all communication partners. This will also ensure that multiple GCS all have an up-to-date list of all parameters. If the sending GCS did not receive a PARAM_VALUE message within its timeout time, it should re-send the PARAM_SET message. </summary>
    public struct mavlink_param_set_t
    {
        /// <summary>Onboard parameter value   </summary>
        public  float param_value;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
            /// <summary>Onboard parameter type: see the MAV_PARAM_TYPE enum for supported data types. MAV_PARAM_TYPE  </summary>
        public  /*MAV_PARAM_TYPE*/byte param_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=50)]
    ///<summary> The global position, as returned by the Global Positioning System (GPS). This is    
///                NOT the global position estimate of the system, but rather a RAW sensor value. See message GLOBAL_POSITION for the global position estimate. </summary>
    public struct mavlink_gps_raw_int_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Latitude (WGS84, EGM96 ellipsoid), in degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude (WGS84, EGM96 ellipsoid), in degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude (AMSL, NOT WGS84), in meters * 1000 (positive for up). Note that virtually all GPS modules provide the AMSL altitude in addition to the WGS84 altitude.  [mm] </summary>
        public  int alt;
            /// <summary>GPS HDOP horizontal dilution of position (unitless). If unknown, set to: UINT16_MAX   </summary>
        public  ushort eph;
            /// <summary>GPS VDOP vertical dilution of position (unitless). If unknown, set to: UINT16_MAX   </summary>
        public  ushort epv;
            /// <summary>GPS ground speed (m/s * 100). If unknown, set to: UINT16_MAX  [cm/s] </summary>
        public  ushort vel;
            /// <summary>Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX  [cdeg] </summary>
        public  ushort cog;
            /// <summary>See the GPS_FIX_TYPE enum. GPS_FIX_TYPE  </summary>
        public  /*GPS_FIX_TYPE*/byte fix_type;
            /// <summary>Number of satellites visible. If unknown, set to 255   </summary>
        public  byte satellites_visible;
            /// <summary>Altitude (above WGS84, EGM96 ellipsoid), in meters * 1000 (positive for up).  [mm] </summary>
        public  int alt_ellipsoid;
            /// <summary>Position uncertainty in meters * 1000 (positive for up).  [mm] </summary>
        public  uint h_acc;
            /// <summary>Altitude uncertainty in meters * 1000 (positive for up).  [mm] </summary>
        public  uint v_acc;
            /// <summary>Speed uncertainty in meters * 1000 (positive for up).  [mm] </summary>
        public  uint vel_acc;
            /// <summary>Heading / track uncertainty in degrees * 1e5.  [degE5] </summary>
        public  uint hdg_acc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=101)]
    ///<summary> The positioning status, as reported by GPS. This message is intended to display status information about each satellite visible to the receiver. See message GLOBAL_POSITION for the global position estimate. This message can contain information for up to 20 satellites. </summary>
    public struct mavlink_gps_status_t
    {
        /// <summary>Number of satellites visible   </summary>
        public  byte satellites_visible;
            /// <summary>Global satellite ID   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_prn;
            /// <summary>0: Satellite not used, 1: used for localization   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_used;
            /// <summary>Elevation (0: right on top of receiver, 90: on the horizon) of satellite  [deg] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_elevation;
            /// <summary>Direction of satellite, 0: 0 deg, 255: 360 deg.  [deg] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_azimuth;
            /// <summary>Signal to noise ratio of satellite  [dB] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_snr;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The RAW IMU readings for the usual 9DOF sensor setup. This message should contain the scaled values to the described units </summary>
    public struct mavlink_scaled_imu_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>X acceleration (mg)  [mG] </summary>
        public  short xacc;
            /// <summary>Y acceleration (mg)  [mG] </summary>
        public  short yacc;
            /// <summary>Z acceleration (mg)  [mG] </summary>
        public  short zacc;
            /// <summary>Angular speed around X axis (millirad /sec)  [mrad/s] </summary>
        public  short xgyro;
            /// <summary>Angular speed around Y axis (millirad /sec)  [mrad/s] </summary>
        public  short ygyro;
            /// <summary>Angular speed around Z axis (millirad /sec)  [mrad/s] </summary>
        public  short zgyro;
            /// <summary>X Magnetic field (milli tesla)  [mT] </summary>
        public  short xmag;
            /// <summary>Y Magnetic field (milli tesla)  [mT] </summary>
        public  short ymag;
            /// <summary>Z Magnetic field (milli tesla)  [mT] </summary>
        public  short zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    ///<summary> The RAW IMU readings for the usual 9DOF sensor setup. This message should always contain the true raw values without any scaling to allow data capture and system debugging. </summary>
    public struct mavlink_raw_imu_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>X acceleration (raw)   </summary>
        public  short xacc;
            /// <summary>Y acceleration (raw)   </summary>
        public  short yacc;
            /// <summary>Z acceleration (raw)   </summary>
        public  short zacc;
            /// <summary>Angular speed around X axis (raw)   </summary>
        public  short xgyro;
            /// <summary>Angular speed around Y axis (raw)   </summary>
        public  short ygyro;
            /// <summary>Angular speed around Z axis (raw)   </summary>
        public  short zgyro;
            /// <summary>X Magnetic field (raw)   </summary>
        public  short xmag;
            /// <summary>Y Magnetic field (raw)   </summary>
        public  short ymag;
            /// <summary>Z Magnetic field (raw)   </summary>
        public  short zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=16)]
    ///<summary> The RAW pressure readings for the typical setup of one absolute pressure and one differential pressure sensor. The sensor values should be the raw, UNSCALED ADC values. </summary>
    public struct mavlink_raw_pressure_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Absolute pressure (raw)   </summary>
        public  short press_abs;
            /// <summary>Differential pressure 1 (raw, 0 if nonexistant)   </summary>
        public  short press_diff1;
            /// <summary>Differential pressure 2 (raw, 0 if nonexistant)   </summary>
        public  short press_diff2;
            /// <summary>Raw Temperature measurement (raw)   </summary>
        public  short temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> The pressure readings for the typical setup of one absolute and differential pressure sensor. The units are as specified in each field. </summary>
    public struct mavlink_scaled_pressure_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Absolute pressure (hectopascal)  [hPa] </summary>
        public  float press_abs;
            /// <summary>Differential pressure 1 (hectopascal)  [hPa] </summary>
        public  float press_diff;
            /// <summary>Temperature measurement (0.01 degrees celsius)  [cdegC] </summary>
        public  short temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> The attitude in the aeronautical frame (right-handed, Z-down, X-front, Y-right). </summary>
    public struct mavlink_attitude_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Roll angle (rad, -pi..+pi)  [rad] </summary>
        public  float roll;
            /// <summary>Pitch angle (rad, -pi..+pi)  [rad] </summary>
        public  float pitch;
            /// <summary>Yaw angle (rad, -pi..+pi)  [rad] </summary>
        public  float yaw;
            /// <summary>Roll angular speed (rad/s)  [rad/s] </summary>
        public  float rollspeed;
            /// <summary>Pitch angular speed (rad/s)  [rad/s] </summary>
        public  float pitchspeed;
            /// <summary>Yaw angular speed (rad/s)  [rad/s] </summary>
        public  float yawspeed;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    ///<summary> The attitude in the aeronautical frame (right-handed, Z-down, X-front, Y-right), expressed as quaternion. Quaternion order is w, x, y, z and a zero rotation would be expressed as (1 0 0 0). </summary>
    public struct mavlink_attitude_quaternion_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Quaternion component 1, w (1 in null-rotation)   </summary>
        public  float q1;
            /// <summary>Quaternion component 2, x (0 in null-rotation)   </summary>
        public  float q2;
            /// <summary>Quaternion component 3, y (0 in null-rotation)   </summary>
        public  float q3;
            /// <summary>Quaternion component 4, z (0 in null-rotation)   </summary>
        public  float q4;
            /// <summary>Roll angular speed (rad/s)  [rad/s] </summary>
        public  float rollspeed;
            /// <summary>Pitch angular speed (rad/s)  [rad/s] </summary>
        public  float pitchspeed;
            /// <summary>Yaw angular speed (rad/s)  [rad/s] </summary>
        public  float yawspeed;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> The filtered local position (e.g. fused computer vision and accelerometers). Coordinate frame is right-handed, Z-axis down (aeronautical frame, NED / north-east-down convention) </summary>
    public struct mavlink_local_position_ned_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>X Position  [m] </summary>
        public  float x;
            /// <summary>Y Position  [m] </summary>
        public  float y;
            /// <summary>Z Position  [m] </summary>
        public  float z;
            /// <summary>X Speed  [m/s] </summary>
        public  float vx;
            /// <summary>Y Speed  [m/s] </summary>
        public  float vy;
            /// <summary>Z Speed  [m/s] </summary>
        public  float vz;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> The filtered global position (e.g. fused GPS and accelerometers). The position is in GPS-frame (right-handed, Z-up). It    
///               is designed as scaled integer message since the resolution of float is not sufficient. </summary>
    public struct mavlink_global_position_int_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Latitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude in meters, expressed as * 1000 (millimeters), AMSL (not WGS84 - note that virtually all GPS modules provide the AMSL as well)  [mm] </summary>
        public  int alt;
            /// <summary>Altitude above ground in meters, expressed as * 1000 (millimeters)  [mm] </summary>
        public  int relative_alt;
            /// <summary>Ground X Speed (Latitude, positive north), expressed as m/s * 100  [cm/s] </summary>
        public  short vx;
            /// <summary>Ground Y Speed (Longitude, positive east), expressed as m/s * 100  [cm/s] </summary>
        public  short vy;
            /// <summary>Ground Z Speed (Altitude, positive down), expressed as m/s * 100  [cm/s] </summary>
        public  short vz;
            /// <summary>Vehicle heading (yaw angle) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX  [cdeg] </summary>
        public  ushort hdg;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The scaled values of the RC channels received. (-100%) -10000, (0%) 0, (100%) 10000. Channels that are inactive should be set to UINT16_MAX. </summary>
    public struct mavlink_rc_channels_scaled_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>RC channel 1 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX.   </summary>
        public  short chan1_scaled;
            /// <summary>RC channel 2 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX.   </summary>
        public  short chan2_scaled;
            /// <summary>RC channel 3 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX.   </summary>
        public  short chan3_scaled;
            /// <summary>RC channel 4 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX.   </summary>
        public  short chan4_scaled;
            /// <summary>RC channel 5 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX.   </summary>
        public  short chan5_scaled;
            /// <summary>RC channel 6 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX.   </summary>
        public  short chan6_scaled;
            /// <summary>RC channel 7 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX.   </summary>
        public  short chan7_scaled;
            /// <summary>RC channel 8 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX.   </summary>
        public  short chan8_scaled;
            /// <summary>Servo output port (set of 8 outputs = 1 port). Most MAVs will just use one, but this allows for more than 8 servos.   </summary>
        public  byte port;
            /// <summary>Receive signal strength indicator, 0: 0%, 100: 100%, 255: invalid/unknown.  [%] </summary>
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The RAW values of the RC channels received. The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%. Individual receivers/transmitters might violate this specification. </summary>
    public struct mavlink_rc_channels_raw_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>RC channel 1 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan1_raw;
            /// <summary>RC channel 2 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan2_raw;
            /// <summary>RC channel 3 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan3_raw;
            /// <summary>RC channel 4 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan4_raw;
            /// <summary>RC channel 5 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan5_raw;
            /// <summary>RC channel 6 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan6_raw;
            /// <summary>RC channel 7 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan7_raw;
            /// <summary>RC channel 8 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan8_raw;
            /// <summary>Servo output port (set of 8 outputs = 1 port). Most MAVs will just use one, but this allows for more than 8 servos.   </summary>
        public  byte port;
            /// <summary>Receive signal strength indicator, 0: 0%, 100: 100%, 255: invalid/unknown.  [%] </summary>
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    ///<summary> The RAW values of the servo outputs (for RC input from the remote, use the RC_CHANNELS messages). The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%. </summary>
    public struct mavlink_servo_output_raw_t
    {
        /// <summary>Timestamp (microseconds since system boot)  [us] </summary>
        public  uint time_usec;
            /// <summary>Servo output 1 value, in microseconds  [us] </summary>
        public  ushort servo1_raw;
            /// <summary>Servo output 2 value, in microseconds  [us] </summary>
        public  ushort servo2_raw;
            /// <summary>Servo output 3 value, in microseconds  [us] </summary>
        public  ushort servo3_raw;
            /// <summary>Servo output 4 value, in microseconds  [us] </summary>
        public  ushort servo4_raw;
            /// <summary>Servo output 5 value, in microseconds  [us] </summary>
        public  ushort servo5_raw;
            /// <summary>Servo output 6 value, in microseconds  [us] </summary>
        public  ushort servo6_raw;
            /// <summary>Servo output 7 value, in microseconds  [us] </summary>
        public  ushort servo7_raw;
            /// <summary>Servo output 8 value, in microseconds  [us] </summary>
        public  ushort servo8_raw;
            /// <summary>Servo output port (set of 8 outputs = 1 port). Most MAVs will just use one, but this allows to encode more than 8 servos.   </summary>
        public  byte port;
            /// <summary>Servo output 9 value, in microseconds  [us] </summary>
        public  ushort servo9_raw;
            /// <summary>Servo output 10 value, in microseconds  [us] </summary>
        public  ushort servo10_raw;
            /// <summary>Servo output 11 value, in microseconds  [us] </summary>
        public  ushort servo11_raw;
            /// <summary>Servo output 12 value, in microseconds  [us] </summary>
        public  ushort servo12_raw;
            /// <summary>Servo output 13 value, in microseconds  [us] </summary>
        public  ushort servo13_raw;
            /// <summary>Servo output 14 value, in microseconds  [us] </summary>
        public  ushort servo14_raw;
            /// <summary>Servo output 15 value, in microseconds  [us] </summary>
        public  ushort servo15_raw;
            /// <summary>Servo output 16 value, in microseconds  [us] </summary>
        public  ushort servo16_raw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=7)]
    ///<summary> Request a partial list of mission items from the system/component. https://mavlink.io/en/protocol/mission.html. If start and end index are the same, just send one waypoint. </summary>
    public struct mavlink_mission_request_partial_list_t
    {
        /// <summary>Start index, 0 by default   </summary>
        public  short start_index;
            /// <summary>End index, -1 by default (-1: send list to end). Else a valid index of the list   </summary>
        public  short end_index;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=7)]
    ///<summary> This message is sent to the MAV to write a partial list. If start index == end index, only one item will be transmitted / updated. If the start index is NOT 0 and above the current list size, this request should be REJECTED! </summary>
    public struct mavlink_mission_write_partial_list_t
    {
        /// <summary>Start index, 0 by default and smaller / equal to the largest index of the current onboard list.   </summary>
        public  short start_index;
            /// <summary>End index, equal or greater than start index.   </summary>
        public  short end_index;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=38)]
    ///<summary> Message encoding a mission item. This message is emitted to announce    
///                the presence of a mission item and to set a mission item on the system. The mission item can be either in x, y, z meters (type: LOCAL) or x:lat, y:lon, z:altitude. Local frame is Z-down, right handed (NED), global frame is Z-up, right handed (ENU). See also https://mavlink.io/en/protocol/mission.html. </summary>
    public struct mavlink_mission_item_t
    {
        /// <summary>PARAM1, see MAV_CMD enum   </summary>
        public  float param1;
            /// <summary>PARAM2, see MAV_CMD enum   </summary>
        public  float param2;
            /// <summary>PARAM3, see MAV_CMD enum   </summary>
        public  float param3;
            /// <summary>PARAM4, see MAV_CMD enum   </summary>
        public  float param4;
            /// <summary>PARAM5 / local: x position, global: latitude   </summary>
        public  float x;
            /// <summary>PARAM6 / y position: global: longitude   </summary>
        public  float y;
            /// <summary>PARAM7 / z position: global: altitude (relative or absolute, depending on frame.   </summary>
        public  float z;
            /// <summary>Sequence   </summary>
        public  ushort seq;
            /// <summary>The scheduled action for the waypoint, as defined by MAV_CMD enum MAV_CMD  </summary>
        public  /*MAV_CMD*/ushort command;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>The coordinate system of the waypoint, as defined by MAV_FRAME enum MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte frame;
            /// <summary>false:0, true:1   </summary>
        public  byte current;
            /// <summary>autocontinue to next wp   </summary>
        public  byte autocontinue;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=5)]
    ///<summary> Request the information of the mission item with the sequence number seq. The response of the system to this message should be a MISSION_ITEM message. https://mavlink.io/en/protocol/mission.html </summary>
    public struct mavlink_mission_request_t
    {
        /// <summary>Sequence   </summary>
        public  ushort seq;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> Set the mission item with sequence number seq as current item. This means that the MAV will continue to this mission item on the shortest path (not following the mission items in-between). </summary>
    public struct mavlink_mission_set_current_t
    {
        /// <summary>Sequence   </summary>
        public  ushort seq;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Message that announces the sequence number of the current active mission item. The MAV will fly towards this mission item. </summary>
    public struct mavlink_mission_current_t
    {
        /// <summary>Sequence   </summary>
        public  ushort seq;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Request the overall list of mission items from the system/component. </summary>
    public struct mavlink_mission_request_list_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=5)]
    ///<summary> This message is emitted as response to MISSION_REQUEST_LIST by the MAV and to initiate a write transaction. The GCS can then request the individual mission item based on the knowledge of the total number of waypoints. </summary>
    public struct mavlink_mission_count_t
    {
        /// <summary>Number of mission items in the sequence   </summary>
        public  ushort count;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Delete all mission items at once. </summary>
    public struct mavlink_mission_clear_all_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> A certain mission item has been reached. The system will either hold this position (or circle on the orbit) or (if the autocontinue on the WP was set) continue to the next waypoint. </summary>
    public struct mavlink_mission_item_reached_t
    {
        /// <summary>Sequence   </summary>
        public  ushort seq;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> Ack message during waypoint handling. The type field states if this message is a positive ack (type=0) or if an error happened (type=non-zero). </summary>
    public struct mavlink_mission_ack_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>See MAV_MISSION_RESULT enum MAV_MISSION_RESULT  </summary>
        public  /*MAV_MISSION_RESULT*/byte type;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=21)]
    ///<summary> As local waypoints exist, the global waypoint reference allows to transform between the local coordinate frame and the global (GPS) coordinate frame. This can be necessary when e.g. in- and outdoor settings are connected and the MAV should move from in- to outdoor. </summary>
    public struct mavlink_set_gps_global_origin_t
    {
        /// <summary>Latitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int latitude;
            /// <summary>Longitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int longitude;
            /// <summary>Altitude (AMSL), in meters * 1000 (positive for up)  [mm] </summary>
        public  int altitude;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Once the MAV sets a new GPS-Local correspondence, this message announces the origin (0,0,0) position </summary>
    public struct mavlink_gps_global_origin_t
    {
        /// <summary>Latitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int latitude;
            /// <summary>Longitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int longitude;
            /// <summary>Altitude (AMSL), in meters * 1000 (positive for up)  [mm] </summary>
        public  int altitude;
            /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    ///<summary> Bind a RC channel to a parameter. The parameter should change accoding to the RC channel value. </summary>
    public struct mavlink_param_map_rc_t
    {
        /// <summary>Initial parameter value   </summary>
        public  float param_value0;
            /// <summary>Scale, maps the RC range [-1, 1] to a parameter value   </summary>
        public  float scale;
            /// <summary>Minimum param value. The protocol does not define if this overwrites an onboard minimum value. (Depends on implementation)   </summary>
        public  float param_value_min;
            /// <summary>Maximum param value. The protocol does not define if this overwrites an onboard maximum value. (Depends on implementation)   </summary>
        public  float param_value_max;
            /// <summary>Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored), send -2 to disable any existing map for this rc_channel_index.   </summary>
        public  short param_index;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
            /// <summary>Index of parameter RC channel. Not equal to the RC channel id. Typically correpsonds to a potentiometer-knob on the RC.   </summary>
        public  byte parameter_rc_channel_index;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=5)]
    ///<summary> Request the information of the mission item with the sequence number seq. The response of the system to this message should be a MISSION_ITEM_INT message. https://mavlink.io/en/protocol/mission.html </summary>
    public struct mavlink_mission_request_int_t
    {
        /// <summary>Sequence   </summary>
        public  ushort seq;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=27)]
    ///<summary> Set a safety zone (volume), which is defined by two corners of a cube. This message can be used to tell the MAV which setpoints/waypoints to accept and which to reject. Safety areas are often enforced by national or competition regulations. </summary>
    public struct mavlink_safety_set_allowed_area_t
    {
        /// <summary>x position 1 / Latitude 1  [m] </summary>
        public  float p1x;
            /// <summary>y position 1 / Longitude 1  [m] </summary>
        public  float p1y;
            /// <summary>z position 1 / Altitude 1  [m] </summary>
        public  float p1z;
            /// <summary>x position 2 / Latitude 2  [m] </summary>
        public  float p2x;
            /// <summary>y position 2 / Longitude 2  [m] </summary>
        public  float p2y;
            /// <summary>z position 2 / Altitude 2  [m] </summary>
        public  float p2z;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Coordinate frame, as defined by MAV_FRAME enum. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down. MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=25)]
    ///<summary> Read out the safety zone the MAV currently assumes. </summary>
    public struct mavlink_safety_allowed_area_t
    {
        /// <summary>x position 1 / Latitude 1  [m] </summary>
        public  float p1x;
            /// <summary>y position 1 / Longitude 1  [m] </summary>
        public  float p1y;
            /// <summary>z position 1 / Altitude 1  [m] </summary>
        public  float p1z;
            /// <summary>x position 2 / Latitude 2  [m] </summary>
        public  float p2x;
            /// <summary>y position 2 / Longitude 2  [m] </summary>
        public  float p2y;
            /// <summary>z position 2 / Altitude 2  [m] </summary>
        public  float p2z;
            /// <summary>Coordinate frame, as defined by MAV_FRAME enum. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down. MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=72)]
    ///<summary> The attitude in the aeronautical frame (right-handed, Z-down, X-front, Y-right), expressed as quaternion. Quaternion order is w, x, y, z and a zero rotation would be expressed as (1 0 0 0). </summary>
    public struct mavlink_attitude_quaternion_cov_t
    {
        /// <summary>Timestamp (microseconds since system boot or since UNIX epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Roll angular speed (rad/s)  [rad/s] </summary>
        public  float rollspeed;
            /// <summary>Pitch angular speed (rad/s)  [rad/s] </summary>
        public  float pitchspeed;
            /// <summary>Yaw angular speed (rad/s)  [rad/s] </summary>
        public  float yawspeed;
            /// <summary>Attitude covariance   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    ///<summary> The state of the fixed wing navigation and position controller. </summary>
    public struct mavlink_nav_controller_output_t
    {
        /// <summary>Current desired roll in degrees  [deg] </summary>
        public  float nav_roll;
            /// <summary>Current desired pitch in degrees  [deg] </summary>
        public  float nav_pitch;
            /// <summary>Current altitude error in meters  [m] </summary>
        public  float alt_error;
            /// <summary>Current airspeed error in meters/second  [m/s] </summary>
        public  float aspd_error;
            /// <summary>Current crosstrack error on x-y plane in meters  [m] </summary>
        public  float xtrack_error;
            /// <summary>Current desired heading in degrees  [deg] </summary>
        public  short nav_bearing;
            /// <summary>Bearing to current waypoint/target in degrees  [deg] </summary>
        public  short target_bearing;
            /// <summary>Distance to active waypoint in meters  [m] </summary>
        public  ushort wp_dist;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=181)]
    ///<summary> The filtered global position (e.g. fused GPS and accelerometers). The position is in GPS-frame (right-handed, Z-up). It  is designed as scaled integer message since the resolution of float is not sufficient. NOTE: This message is intended for onboard networks / companion computers and higher-bandwidth links and optimized for accuracy and completeness. Please use the GLOBAL_POSITION_INT message for a minimal subset. </summary>
    public struct mavlink_global_position_int_cov_t
    {
        /// <summary>Timestamp (microseconds since system boot or since UNIX epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Latitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude in meters, expressed as * 1000 (millimeters), above MSL  [mm] </summary>
        public  int alt;
            /// <summary>Altitude above ground in meters, expressed as * 1000 (millimeters)  [mm] </summary>
        public  int relative_alt;
            /// <summary>Ground X Speed (Latitude), expressed as m/s  [m/s] </summary>
        public  float vx;
            /// <summary>Ground Y Speed (Longitude), expressed as m/s  [m/s] </summary>
        public  float vy;
            /// <summary>Ground Z Speed (Altitude), expressed as m/s  [m/s] </summary>
        public  float vz;
            /// <summary>Covariance matrix (first six entries are the first ROW, next six entries are the second row, etc.)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)]
		public float[] covariance;
            /// <summary>Class id of the estimator this estimate originated from. MAV_ESTIMATOR_TYPE  </summary>
        public  /*MAV_ESTIMATOR_TYPE*/byte estimator_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=225)]
    ///<summary> The filtered local position (e.g. fused computer vision and accelerometers). Coordinate frame is right-handed, Z-axis down (aeronautical frame, NED / north-east-down convention) </summary>
    public struct mavlink_local_position_ned_cov_t
    {
        /// <summary>Timestamp (microseconds since system boot or since UNIX epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>X Position  [m] </summary>
        public  float x;
            /// <summary>Y Position  [m] </summary>
        public  float y;
            /// <summary>Z Position  [m] </summary>
        public  float z;
            /// <summary>X Speed (m/s)  [m/s] </summary>
        public  float vx;
            /// <summary>Y Speed (m/s)  [m/s] </summary>
        public  float vy;
            /// <summary>Z Speed (m/s)  [m/s] </summary>
        public  float vz;
            /// <summary>X Acceleration (m/s^2)  [m/s/s] </summary>
        public  float ax;
            /// <summary>Y Acceleration (m/s^2)  [m/s/s] </summary>
        public  float ay;
            /// <summary>Z Acceleration (m/s^2)  [m/s/s] </summary>
        public  float az;
            /// <summary>Covariance matrix upper right triangular (first nine entries are the first ROW, next eight entries are the second row, etc.)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=45)]
		public float[] covariance;
            /// <summary>Class id of the estimator this estimate originated from. MAV_ESTIMATOR_TYPE  </summary>
        public  /*MAV_ESTIMATOR_TYPE*/byte estimator_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> The PPM values of the RC channels received. The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%. Individual receivers/transmitters might violate this specification. </summary>
    public struct mavlink_rc_channels_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>RC channel 1 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan1_raw;
            /// <summary>RC channel 2 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan2_raw;
            /// <summary>RC channel 3 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan3_raw;
            /// <summary>RC channel 4 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan4_raw;
            /// <summary>RC channel 5 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan5_raw;
            /// <summary>RC channel 6 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan6_raw;
            /// <summary>RC channel 7 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan7_raw;
            /// <summary>RC channel 8 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan8_raw;
            /// <summary>RC channel 9 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan9_raw;
            /// <summary>RC channel 10 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan10_raw;
            /// <summary>RC channel 11 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan11_raw;
            /// <summary>RC channel 12 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan12_raw;
            /// <summary>RC channel 13 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan13_raw;
            /// <summary>RC channel 14 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan14_raw;
            /// <summary>RC channel 15 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan15_raw;
            /// <summary>RC channel 16 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan16_raw;
            /// <summary>RC channel 17 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan17_raw;
            /// <summary>RC channel 18 value, in microseconds. A value of UINT16_MAX implies the channel is unused.  [us] </summary>
        public  ushort chan18_raw;
            /// <summary>Total number of RC channels being received. This can be larger than 18, indicating that more channels are available but not given in this message. This value should be 0 when no RC channels are available.   </summary>
        public  byte chancount;
            /// <summary>Receive signal strength indicator, 0: 0%, 100: 100%, 255: invalid/unknown.  [%] </summary>
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> THIS INTERFACE IS DEPRECATED. USE SET_MESSAGE_INTERVAL INSTEAD. </summary>
    public struct mavlink_request_data_stream_t
    {
        /// <summary>The requested message rate  [Hz] </summary>
        public  ushort req_message_rate;
            /// <summary>The target requested to send the message stream.   </summary>
        public  byte target_system;
            /// <summary>The target requested to send the message stream.   </summary>
        public  byte target_component;
            /// <summary>The ID of the requested data stream   </summary>
        public  byte req_stream_id;
            /// <summary>1 to start sending, 0 to stop sending.   </summary>
        public  byte start_stop;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> THIS INTERFACE IS DEPRECATED. USE MESSAGE_INTERVAL INSTEAD. </summary>
    public struct mavlink_data_stream_t
    {
        /// <summary>The message rate  [Hz] </summary>
        public  ushort message_rate;
            /// <summary>The ID of the requested data stream   </summary>
        public  byte stream_id;
            /// <summary>1 stream is enabled, 0 stream is stopped.   </summary>
        public  byte on_off;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=11)]
    ///<summary> This message provides an API for manually controlling the vehicle using standard joystick axes nomenclature, along with a joystick-like input device. Unused axes can be disabled an buttons are also transmit as boolean values of their  </summary>
    public struct mavlink_manual_control_t
    {
        /// <summary>X-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to forward(1000)-backward(-1000) movement on a joystick and the pitch of a vehicle.   </summary>
        public  short x;
            /// <summary>Y-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to left(-1000)-right(1000) movement on a joystick and the roll of a vehicle.   </summary>
        public  short y;
            /// <summary>Z-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a separate slider movement with maximum being 1000 and minimum being -1000 on a joystick and the thrust of a vehicle. Positive values are positive thrust, negative values are negative thrust.   </summary>
        public  short z;
            /// <summary>R-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a twisting of the joystick, with counter-clockwise being 1000 and clockwise being -1000, and the yaw of a vehicle.   </summary>
        public  short r;
            /// <summary>A bitfield corresponding to the joystick buttons' current state, 1 for pressed, 0 for released. The lowest bit corresponds to Button 1.   </summary>
        public  ushort buttons;
            /// <summary>The system to be controlled.   </summary>
        public  byte target;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=38)]
    ///<summary> The RAW values of the RC channels sent to the MAV to override info received from the RC radio. A value of UINT16_MAX means no change to that channel. A value of 0 means control of that channel should be released back to the RC radio. The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%. Individual receivers/transmitters might violate this specification. </summary>
    public struct mavlink_rc_channels_override_t
    {
        /// <summary>RC channel 1 value, in microseconds. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan1_raw;
            /// <summary>RC channel 2 value, in microseconds. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan2_raw;
            /// <summary>RC channel 3 value, in microseconds. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan3_raw;
            /// <summary>RC channel 4 value, in microseconds. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan4_raw;
            /// <summary>RC channel 5 value, in microseconds. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan5_raw;
            /// <summary>RC channel 6 value, in microseconds. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan6_raw;
            /// <summary>RC channel 7 value, in microseconds. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan7_raw;
            /// <summary>RC channel 8 value, in microseconds. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan8_raw;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>RC channel 9 value, in microseconds. A value of 0 means to ignore this field.  [us] </summary>
        public  ushort chan9_raw;
            /// <summary>RC channel 10 value, in microseconds. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan10_raw;
            /// <summary>RC channel 11 value, in microseconds. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan11_raw;
            /// <summary>RC channel 12 value, in microseconds. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan12_raw;
            /// <summary>RC channel 13 value, in microseconds. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan13_raw;
            /// <summary>RC channel 14 value, in microseconds. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan14_raw;
            /// <summary>RC channel 15 value, in microseconds. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan15_raw;
            /// <summary>RC channel 16 value, in microseconds. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan16_raw;
            /// <summary>RC channel 17 value, in microseconds. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan17_raw;
            /// <summary>RC channel 18 value, in microseconds. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        public  ushort chan18_raw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=38)]
    ///<summary> Message encoding a mission item. This message is emitted to announce    
///                the presence of a mission item and to set a mission item on the system. The mission item can be either in x, y, z meters (type: LOCAL) or x:lat, y:lon, z:altitude. Local frame is Z-down, right handed (NED), global frame is Z-up, right handed (ENU). See also https://mavlink.io/en/protocol/mission.html. </summary>
    public struct mavlink_mission_item_int_t
    {
        /// <summary>PARAM1, see MAV_CMD enum   </summary>
        public  float param1;
            /// <summary>PARAM2, see MAV_CMD enum   </summary>
        public  float param2;
            /// <summary>PARAM3, see MAV_CMD enum   </summary>
        public  float param3;
            /// <summary>PARAM4, see MAV_CMD enum   </summary>
        public  float param4;
            /// <summary>PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7   </summary>
        public  int x;
            /// <summary>PARAM6 / y position: local: x position in meters * 1e4, global: longitude in degrees *10^7   </summary>
        public  int y;
            /// <summary>PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame.   </summary>
        public  float z;
            /// <summary>Waypoint ID (sequence number). Starts at zero. Increases monotonically for each waypoint, no gaps in the sequence (0,1,2,3,4).   </summary>
        public  ushort seq;
            /// <summary>The scheduled action for the waypoint, as defined by MAV_CMD enum MAV_CMD  </summary>
        public  /*MAV_CMD*/ushort command;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>The coordinate system of the waypoint, as defined by MAV_FRAME enum MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte frame;
            /// <summary>false:0, true:1   </summary>
        public  byte current;
            /// <summary>autocontinue to next wp   </summary>
        public  byte autocontinue;
            /// <summary>Mission type, see MAV_MISSION_TYPE MAV_MISSION_TYPE  </summary>
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Metrics typically displayed on a HUD for fixed wing aircraft </summary>
    public struct mavlink_vfr_hud_t
    {
        /// <summary>Current airspeed in m/s  [m/s] </summary>
        public  float airspeed;
            /// <summary>Current ground speed in m/s  [m/s] </summary>
        public  float groundspeed;
            /// <summary>Current altitude (MSL), in meters  [m] </summary>
        public  float alt;
            /// <summary>Current climb rate in meters/second  [m/s] </summary>
        public  float climb;
            /// <summary>Current heading in degrees, in compass units (0..360, 0=north)  [deg] </summary>
        public  short heading;
            /// <summary>Current throttle setting in integer percent, 0 to 100  [%] </summary>
        public  ushort throttle;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    ///<summary> Message encoding a command with parameters as scaled integers. Scaling depends on the actual command value. </summary>
    public struct mavlink_command_int_t
    {
        /// <summary>PARAM1, see MAV_CMD enum   </summary>
        public  float param1;
            /// <summary>PARAM2, see MAV_CMD enum   </summary>
        public  float param2;
            /// <summary>PARAM3, see MAV_CMD enum   </summary>
        public  float param3;
            /// <summary>PARAM4, see MAV_CMD enum   </summary>
        public  float param4;
            /// <summary>PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7   </summary>
        public  int x;
            /// <summary>PARAM6 / local: y position in meters * 1e4, global: longitude in degrees * 10^7   </summary>
        public  int y;
            /// <summary>PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame.   </summary>
        public  float z;
            /// <summary>The scheduled action for the mission item, as defined by MAV_CMD enum MAV_CMD  </summary>
        public  /*MAV_CMD*/ushort command;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>The coordinate system of the COMMAND, as defined by MAV_FRAME enum MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte frame;
            /// <summary>false:0, true:1   </summary>
        public  byte current;
            /// <summary>autocontinue to next wp   </summary>
        public  byte autocontinue;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=33)]
    ///<summary> Send a command with up to seven parameters to the MAV </summary>
    public struct mavlink_command_long_t
    {
        /// <summary>Parameter 1, as defined by MAV_CMD enum.   </summary>
        public  float param1;
            /// <summary>Parameter 2, as defined by MAV_CMD enum.   </summary>
        public  float param2;
            /// <summary>Parameter 3, as defined by MAV_CMD enum.   </summary>
        public  float param3;
            /// <summary>Parameter 4, as defined by MAV_CMD enum.   </summary>
        public  float param4;
            /// <summary>Parameter 5, as defined by MAV_CMD enum.   </summary>
        public  float param5;
            /// <summary>Parameter 6, as defined by MAV_CMD enum.   </summary>
        public  float param6;
            /// <summary>Parameter 7, as defined by MAV_CMD enum.   </summary>
        public  float param7;
            /// <summary>Command ID, as defined by MAV_CMD enum. MAV_CMD  </summary>
        public  /*MAV_CMD*/ushort command;
            /// <summary>System which should execute the command   </summary>
        public  byte target_system;
            /// <summary>Component which should execute the command, 0 for all components   </summary>
        public  byte target_component;
            /// <summary>0: First transmission of this command. 1-255: Confirmation transmissions (e.g. for kill command)   </summary>
        public  byte confirmation;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Report status of a command. Includes feedback whether the command was executed. </summary>
    public struct mavlink_command_ack_t
    {
        /// <summary>Command ID, as defined by MAV_CMD enum. MAV_CMD  </summary>
        public  /*MAV_CMD*/ushort command;
            /// <summary>See MAV_RESULT enum MAV_RESULT  </summary>
        public  /*MAV_RESULT*/byte result;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> Setpoint in roll, pitch, yaw and thrust from the operator </summary>
    public struct mavlink_manual_setpoint_t
    {
        /// <summary>Timestamp in milliseconds since system boot  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Desired roll rate in radians per second  [rad/s] </summary>
        public  float roll;
            /// <summary>Desired pitch rate in radians per second  [rad/s] </summary>
        public  float pitch;
            /// <summary>Desired yaw rate in radians per second  [rad/s] </summary>
        public  float yaw;
            /// <summary>Collective thrust, normalized to 0 .. 1   </summary>
        public  float thrust;
            /// <summary>Flight mode switch position, 0.. 255   </summary>
        public  byte mode_switch;
            /// <summary>Override mode switch position, 0.. 255   </summary>
        public  byte manual_override_switch;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=39)]
    ///<summary> Sets a desired vehicle attitude. Used by an external controller to command the vehicle (manual controller or other system). </summary>
    public struct mavlink_set_attitude_target_t
    {
        /// <summary>Timestamp in milliseconds since system boot  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Body roll rate in radians per second  [rad/s] </summary>
        public  float body_roll_rate;
            /// <summary>Body pitch rate in radians per second  [rad/s] </summary>
        public  float body_pitch_rate;
            /// <summary>Body yaw rate in radians per second  [rad/s] </summary>
        public  float body_yaw_rate;
            /// <summary>Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust)   </summary>
        public  float thrust;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 6: reserved, bit 7: throttle, bit 8: attitude   </summary>
        public  byte type_mask;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    ///<summary> Reports the current commanded attitude of the vehicle as specified by the autopilot. This should match the commands sent in a SET_ATTITUDE_TARGET message if the vehicle is being controlled this way. </summary>
    public struct mavlink_attitude_target_t
    {
        /// <summary>Timestamp in milliseconds since system boot  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Body roll rate in radians per second  [rad/s] </summary>
        public  float body_roll_rate;
            /// <summary>Body pitch rate in radians per second  [rad/s] </summary>
        public  float body_pitch_rate;
            /// <summary>Body yaw rate in radians per second  [rad/s] </summary>
        public  float body_yaw_rate;
            /// <summary>Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust)   </summary>
        public  float thrust;
            /// <summary>Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 7: reserved, bit 8: attitude   bitmask</summary>
        public  byte type_mask;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=53)]
    ///<summary> Sets a desired vehicle position in a local north-east-down coordinate frame. Used by an external controller to command the vehicle (manual controller or other system). </summary>
    public struct mavlink_set_position_target_local_ned_t
    {
        /// <summary>Timestamp in milliseconds since system boot  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>X Position in NED frame in meters  [m] </summary>
        public  float x;
            /// <summary>Y Position in NED frame in meters  [m] </summary>
        public  float y;
            /// <summary>Z Position in NED frame in meters (note, altitude is negative in NED)  [m] </summary>
        public  float z;
            /// <summary>X velocity in NED frame in meter / s  [m/s] </summary>
        public  float vx;
            /// <summary>Y velocity in NED frame in meter / s  [m/s] </summary>
        public  float vy;
            /// <summary>Z velocity in NED frame in meter / s  [m/s] </summary>
        public  float vz;
            /// <summary>X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afx;
            /// <summary>Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afy;
            /// <summary>Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afz;
            /// <summary>yaw setpoint in rad  [rad] </summary>
        public  float yaw;
            /// <summary>yaw rate setpoint in rad/s  [rad/s] </summary>
        public  float yaw_rate;
            /// <summary>Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate   bitmask</summary>
        public  ushort type_mask;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9 MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    ///<summary> Reports the current commanded vehicle position, velocity, and acceleration as specified by the autopilot. This should match the commands sent in SET_POSITION_TARGET_LOCAL_NED if the vehicle is being controlled this way. </summary>
    public struct mavlink_position_target_local_ned_t
    {
        /// <summary>Timestamp in milliseconds since system boot  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>X Position in NED frame in meters  [m] </summary>
        public  float x;
            /// <summary>Y Position in NED frame in meters  [m] </summary>
        public  float y;
            /// <summary>Z Position in NED frame in meters (note, altitude is negative in NED)  [m] </summary>
        public  float z;
            /// <summary>X velocity in NED frame in meter / s  [m/s] </summary>
        public  float vx;
            /// <summary>Y velocity in NED frame in meter / s  [m/s] </summary>
        public  float vy;
            /// <summary>Z velocity in NED frame in meter / s  [m/s] </summary>
        public  float vz;
            /// <summary>X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afx;
            /// <summary>Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afy;
            /// <summary>Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afz;
            /// <summary>yaw setpoint in rad  [rad] </summary>
        public  float yaw;
            /// <summary>yaw rate setpoint in rad/s  [rad/s] </summary>
        public  float yaw_rate;
            /// <summary>Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate   bitmask</summary>
        public  ushort type_mask;
            /// <summary>Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9 MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=53)]
    ///<summary> Sets a desired vehicle position, velocity, and/or acceleration in a global coordinate system (WGS84). Used by an external controller to command the vehicle (manual controller or other system). </summary>
    public struct mavlink_set_position_target_global_int_t
    {
        /// <summary>Timestamp in milliseconds since system boot. The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency.  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>X Position in WGS84 frame in 1e7 * degrees  [degE7] </summary>
        public  int lat_int;
            /// <summary>Y Position in WGS84 frame in 1e7 * degrees  [degE7] </summary>
        public  int lon_int;
            /// <summary>Altitude in meters in AMSL altitude, not WGS84 if absolute or relative, above terrain if GLOBAL_TERRAIN_ALT_INT  [m] </summary>
        public  float alt;
            /// <summary>X velocity in NED frame in meter / s  [m/s] </summary>
        public  float vx;
            /// <summary>Y velocity in NED frame in meter / s  [m/s] </summary>
        public  float vy;
            /// <summary>Z velocity in NED frame in meter / s  [m/s] </summary>
        public  float vz;
            /// <summary>X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afx;
            /// <summary>Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afy;
            /// <summary>Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afz;
            /// <summary>yaw setpoint in rad  [rad] </summary>
        public  float yaw;
            /// <summary>yaw rate setpoint in rad/s  [rad/s] </summary>
        public  float yaw_rate;
            /// <summary>Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate   bitmask</summary>
        public  ushort type_mask;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11 MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    ///<summary> Reports the current commanded vehicle position, velocity, and acceleration as specified by the autopilot. This should match the commands sent in SET_POSITION_TARGET_GLOBAL_INT if the vehicle is being controlled this way. </summary>
    public struct mavlink_position_target_global_int_t
    {
        /// <summary>Timestamp in milliseconds since system boot. The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency.  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>X Position in WGS84 frame in 1e7 * degrees  [degE7] </summary>
        public  int lat_int;
            /// <summary>Y Position in WGS84 frame in 1e7 * degrees  [degE7] </summary>
        public  int lon_int;
            /// <summary>Altitude in meters in AMSL altitude, not WGS84 if absolute or relative, above terrain if GLOBAL_TERRAIN_ALT_INT  [m] </summary>
        public  float alt;
            /// <summary>X velocity in NED frame in meter / s  [m/s] </summary>
        public  float vx;
            /// <summary>Y velocity in NED frame in meter / s  [m/s] </summary>
        public  float vy;
            /// <summary>Z velocity in NED frame in meter / s  [m/s] </summary>
        public  float vz;
            /// <summary>X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afx;
            /// <summary>Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afy;
            /// <summary>Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        public  float afz;
            /// <summary>yaw setpoint in rad  [rad] </summary>
        public  float yaw;
            /// <summary>yaw rate setpoint in rad/s  [rad/s] </summary>
        public  float yaw_rate;
            /// <summary>Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate   bitmask</summary>
        public  ushort type_mask;
            /// <summary>Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11 MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> The offset in X, Y, Z and yaw between the LOCAL_POSITION_NED messages of MAV X and the global coordinate frame in NED coordinates. Coordinate frame is right-handed, Z-axis down (aeronautical frame, NED / north-east-down convention) </summary>
    public struct mavlink_local_position_ned_system_global_offset_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>X Position  [m] </summary>
        public  float x;
            /// <summary>Y Position  [m] </summary>
        public  float y;
            /// <summary>Z Position  [m] </summary>
        public  float z;
            /// <summary>Roll  [rad] </summary>
        public  float roll;
            /// <summary>Pitch  [rad] </summary>
        public  float pitch;
            /// <summary>Yaw  [rad] </summary>
        public  float yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=56)]
    ///<summary> DEPRECATED PACKET! Suffers from missing airspeed fields and singularities due to Euler angles. Please use HIL_STATE_QUATERNION instead. Sent from simulation to autopilot. This packet is useful for high throughput applications such as hardware in the loop simulations. </summary>
    public struct mavlink_hil_state_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Roll angle (rad)  [rad] </summary>
        public  float roll;
            /// <summary>Pitch angle (rad)  [rad] </summary>
        public  float pitch;
            /// <summary>Yaw angle (rad)  [rad] </summary>
        public  float yaw;
            /// <summary>Body frame roll / phi angular speed (rad/s)  [rad/s] </summary>
        public  float rollspeed;
            /// <summary>Body frame pitch / theta angular speed (rad/s)  [rad/s] </summary>
        public  float pitchspeed;
            /// <summary>Body frame yaw / psi angular speed (rad/s)  [rad/s] </summary>
        public  float yawspeed;
            /// <summary>Latitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude in meters, expressed as * 1000 (millimeters)  [mm] </summary>
        public  int alt;
            /// <summary>Ground X Speed (Latitude), expressed as m/s * 100  [cm/s] </summary>
        public  short vx;
            /// <summary>Ground Y Speed (Longitude), expressed as m/s * 100  [cm/s] </summary>
        public  short vy;
            /// <summary>Ground Z Speed (Altitude), expressed as m/s * 100  [cm/s] </summary>
        public  short vz;
            /// <summary>X acceleration (mg)  [mG] </summary>
        public  short xacc;
            /// <summary>Y acceleration (mg)  [mG] </summary>
        public  short yacc;
            /// <summary>Z acceleration (mg)  [mG] </summary>
        public  short zacc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> Sent from autopilot to simulation. Hardware in the loop control outputs </summary>
    public struct mavlink_hil_controls_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Control output -1 .. 1   </summary>
        public  float roll_ailerons;
            /// <summary>Control output -1 .. 1   </summary>
        public  float pitch_elevator;
            /// <summary>Control output -1 .. 1   </summary>
        public  float yaw_rudder;
            /// <summary>Throttle 0 .. 1   </summary>
        public  float throttle;
            /// <summary>Aux 1, -1 .. 1   </summary>
        public  float aux1;
            /// <summary>Aux 2, -1 .. 1   </summary>
        public  float aux2;
            /// <summary>Aux 3, -1 .. 1   </summary>
        public  float aux3;
            /// <summary>Aux 4, -1 .. 1   </summary>
        public  float aux4;
            /// <summary>System mode (MAV_MODE) MAV_MODE  </summary>
        public  /*MAV_MODE*/byte mode;
            /// <summary>Navigation mode (MAV_NAV_MODE)   </summary>
        public  byte nav_mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=33)]
    ///<summary> Sent from simulation to autopilot. The RAW values of the RC channels received. The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%. Individual receivers/transmitters might violate this specification. </summary>
    public struct mavlink_hil_rc_inputs_raw_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>RC channel 1 value, in microseconds  [us] </summary>
        public  ushort chan1_raw;
            /// <summary>RC channel 2 value, in microseconds  [us] </summary>
        public  ushort chan2_raw;
            /// <summary>RC channel 3 value, in microseconds  [us] </summary>
        public  ushort chan3_raw;
            /// <summary>RC channel 4 value, in microseconds  [us] </summary>
        public  ushort chan4_raw;
            /// <summary>RC channel 5 value, in microseconds  [us] </summary>
        public  ushort chan5_raw;
            /// <summary>RC channel 6 value, in microseconds  [us] </summary>
        public  ushort chan6_raw;
            /// <summary>RC channel 7 value, in microseconds  [us] </summary>
        public  ushort chan7_raw;
            /// <summary>RC channel 8 value, in microseconds  [us] </summary>
        public  ushort chan8_raw;
            /// <summary>RC channel 9 value, in microseconds  [us] </summary>
        public  ushort chan9_raw;
            /// <summary>RC channel 10 value, in microseconds  [us] </summary>
        public  ushort chan10_raw;
            /// <summary>RC channel 11 value, in microseconds  [us] </summary>
        public  ushort chan11_raw;
            /// <summary>RC channel 12 value, in microseconds  [us] </summary>
        public  ushort chan12_raw;
            /// <summary>Receive signal strength indicator, 0: 0%, 255: 100%   </summary>
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=81)]
    ///<summary> Sent from autopilot to simulation. Hardware in the loop control outputs (replacement for HIL_CONTROLS) </summary>
    public struct mavlink_hil_actuator_controls_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Flags as bitfield, reserved for future use.   bitmask</summary>
        public  ulong flags;
            /// <summary>Control outputs -1 .. 1. Channel assignment depends on the simulated hardware.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public float[] controls;
            /// <summary>System mode (MAV_MODE), includes arming state. MAV_MODE  </summary>
        public  /*MAV_MODE*/byte mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=34)]
    ///<summary> Optical flow from a flow sensor (e.g. optical mouse sensor) </summary>
    public struct mavlink_optical_flow_t
    {
        /// <summary>Timestamp (UNIX)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Flow in x-sensor direction, angular-speed compensated  [m] </summary>
        public  float flow_comp_m_x;
            /// <summary>Flow in y-sensor direction, angular-speed compensated  [m] </summary>
        public  float flow_comp_m_y;
            /// <summary>Ground distance. Positive value: distance known. Negative value: Unknown distance  [m] </summary>
        public  float ground_distance;
            /// <summary>Flow in x-sensor direction  [dpix] </summary>
        public  short flow_x;
            /// <summary>Flow in y-sensor direction  [dpix] </summary>
        public  short flow_y;
            /// <summary>Sensor ID   </summary>
        public  byte sensor_id;
            /// <summary>Optical flow quality / confidence. 0: bad, 255: maximum quality   </summary>
        public  byte quality;
            /// <summary>Flow rate about X axis  [rad/s] </summary>
        public  float flow_rate_x;
            /// <summary>Flow rate about Y axis  [rad/s] </summary>
        public  float flow_rate_y;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=116)]
    ///<summary>  </summary>
    public struct mavlink_global_vision_position_estimate_t
    {
        /// <summary>Timestamp (microseconds, synced to UNIX time or since system boot)  [us] </summary>
        public  ulong usec;
            /// <summary>Global X position  [m] </summary>
        public  float x;
            /// <summary>Global Y position  [m] </summary>
        public  float y;
            /// <summary>Global Z position  [m] </summary>
        public  float z;
            /// <summary>Roll angle in rad  [rad] </summary>
        public  float roll;
            /// <summary>Pitch angle in rad  [rad] </summary>
        public  float pitch;
            /// <summary>Yaw angle in rad  [rad] </summary>
        public  float yaw;
            /// <summary>Pose covariance matrix upper right triangular (first six entries are the first ROW, next five entries are the second ROW, etc.)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=116)]
    ///<summary>  </summary>
    public struct mavlink_vision_position_estimate_t
    {
        /// <summary>Timestamp (microseconds, synced to UNIX time or since system boot)  [us] </summary>
        public  ulong usec;
            /// <summary>Global X position  [m] </summary>
        public  float x;
            /// <summary>Global Y position  [m] </summary>
        public  float y;
            /// <summary>Global Z position  [m] </summary>
        public  float z;
            /// <summary>Roll angle in rad  [rad] </summary>
        public  float roll;
            /// <summary>Pitch angle in rad  [rad] </summary>
        public  float pitch;
            /// <summary>Yaw angle in rad  [rad] </summary>
        public  float yaw;
            /// <summary>Pose covariance matrix upper right triangular (first six entries are the first ROW, next five entries are the second ROW, etc.)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=56)]
    ///<summary>  </summary>
    public struct mavlink_vision_speed_estimate_t
    {
        /// <summary>Timestamp (microseconds, synced to UNIX time or since system boot)  [us] </summary>
        public  ulong usec;
            /// <summary>Global X speed  [m/s] </summary>
        public  float x;
            /// <summary>Global Y speed  [m/s] </summary>
        public  float y;
            /// <summary>Global Z speed  [m/s] </summary>
        public  float z;
            /// <summary>Linear velocity covariance matrix (1st three entries - 1st row, etc.)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=116)]
    ///<summary>  </summary>
    public struct mavlink_vicon_position_estimate_t
    {
        /// <summary>Timestamp (microseconds, synced to UNIX time or since system boot)  [us] </summary>
        public  ulong usec;
            /// <summary>Global X position  [m] </summary>
        public  float x;
            /// <summary>Global Y position  [m] </summary>
        public  float y;
            /// <summary>Global Z position  [m] </summary>
        public  float z;
            /// <summary>Roll angle in rad  [rad] </summary>
        public  float roll;
            /// <summary>Pitch angle in rad  [rad] </summary>
        public  float pitch;
            /// <summary>Yaw angle in rad  [rad] </summary>
        public  float yaw;
            /// <summary>Pose covariance matrix upper right triangular (first six entries are the first ROW, next five entries are the second ROW, etc.)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=62)]
    ///<summary> The IMU readings in SI units in NED body frame </summary>
    public struct mavlink_highres_imu_t
    {
        /// <summary>Timestamp (microseconds, synced to UNIX time or since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>X acceleration (m/s^2)  [m/s/s] </summary>
        public  float xacc;
            /// <summary>Y acceleration (m/s^2)  [m/s/s] </summary>
        public  float yacc;
            /// <summary>Z acceleration (m/s^2)  [m/s/s] </summary>
        public  float zacc;
            /// <summary>Angular speed around X axis (rad / sec)  [rad/s] </summary>
        public  float xgyro;
            /// <summary>Angular speed around Y axis (rad / sec)  [rad/s] </summary>
        public  float ygyro;
            /// <summary>Angular speed around Z axis (rad / sec)  [rad/s] </summary>
        public  float zgyro;
            /// <summary>X Magnetic field (Gauss)  [gauss] </summary>
        public  float xmag;
            /// <summary>Y Magnetic field (Gauss)  [gauss] </summary>
        public  float ymag;
            /// <summary>Z Magnetic field (Gauss)  [gauss] </summary>
        public  float zmag;
            /// <summary>Absolute pressure in millibar  [mbar] </summary>
        public  float abs_pressure;
            /// <summary>Differential pressure in millibar  [mbar] </summary>
        public  float diff_pressure;
            /// <summary>Altitude calculated from pressure   </summary>
        public  float pressure_alt;
            /// <summary>Temperature in degrees celsius  [degC] </summary>
        public  float temperature;
            /// <summary>Bitmask for fields that have updated since last message, bit 0 = xacc, bit 12: temperature   bitmask</summary>
        public  ushort fields_updated;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> Optical flow from an angular rate flow sensor (e.g. PX4FLOW or mouse sensor) </summary>
    public struct mavlink_optical_flow_rad_t
    {
        /// <summary>Timestamp (microseconds, synced to UNIX time or since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Integration time in microseconds. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the.  [us] </summary>
        public  uint integration_time_us;
            /// <summary>Flow in radians around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.)  [rad] </summary>
        public  float integrated_x;
            /// <summary>Flow in radians around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.)  [rad] </summary>
        public  float integrated_y;
            /// <summary>RH rotation around X axis (rad)  [rad] </summary>
        public  float integrated_xgyro;
            /// <summary>RH rotation around Y axis (rad)  [rad] </summary>
        public  float integrated_ygyro;
            /// <summary>RH rotation around Z axis (rad)  [rad] </summary>
        public  float integrated_zgyro;
            /// <summary>Time in microseconds since the distance was sampled.  [us] </summary>
        public  uint time_delta_distance_us;
            /// <summary>Distance to the center of the flow field in meters. Positive value (including zero): distance known. Negative value: Unknown distance.  [m] </summary>
        public  float distance;
            /// <summary>Temperature * 100 in centi-degrees Celsius  [cdegC] </summary>
        public  short temperature;
            /// <summary>Sensor ID   </summary>
        public  byte sensor_id;
            /// <summary>Optical flow quality / confidence. 0: no valid flow, 255: maximum quality   </summary>
        public  byte quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=64)]
    ///<summary> The IMU readings in SI units in NED body frame </summary>
    public struct mavlink_hil_sensor_t
    {
        /// <summary>Timestamp (microseconds, synced to UNIX time or since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>X acceleration (m/s^2)  [m/s/s] </summary>
        public  float xacc;
            /// <summary>Y acceleration (m/s^2)  [m/s/s] </summary>
        public  float yacc;
            /// <summary>Z acceleration (m/s^2)  [m/s/s] </summary>
        public  float zacc;
            /// <summary>Angular speed around X axis in body frame (rad / sec)  [rad/s] </summary>
        public  float xgyro;
            /// <summary>Angular speed around Y axis in body frame (rad / sec)  [rad/s] </summary>
        public  float ygyro;
            /// <summary>Angular speed around Z axis in body frame (rad / sec)  [rad/s] </summary>
        public  float zgyro;
            /// <summary>X Magnetic field (Gauss)  [gauss] </summary>
        public  float xmag;
            /// <summary>Y Magnetic field (Gauss)  [gauss] </summary>
        public  float ymag;
            /// <summary>Z Magnetic field (Gauss)  [gauss] </summary>
        public  float zmag;
            /// <summary>Absolute pressure in millibar  [mbar] </summary>
        public  float abs_pressure;
            /// <summary>Differential pressure (airspeed) in millibar  [mbar] </summary>
        public  float diff_pressure;
            /// <summary>Altitude calculated from pressure   </summary>
        public  float pressure_alt;
            /// <summary>Temperature in degrees celsius  [degC] </summary>
        public  float temperature;
            /// <summary>Bitmask for fields that have updated since last message, bit 0 = xacc, bit 12: temperature, bit 31: full reset of attitude/position/velocities/etc was performed in sim.   bitmask</summary>
        public  uint fields_updated;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=84)]
    ///<summary> Status of simulation environment, if used </summary>
    public struct mavlink_sim_state_t
    {
        /// <summary>True attitude quaternion component 1, w (1 in null-rotation)   </summary>
        public  float q1;
            /// <summary>True attitude quaternion component 2, x (0 in null-rotation)   </summary>
        public  float q2;
            /// <summary>True attitude quaternion component 3, y (0 in null-rotation)   </summary>
        public  float q3;
            /// <summary>True attitude quaternion component 4, z (0 in null-rotation)   </summary>
        public  float q4;
            /// <summary>Attitude roll expressed as Euler angles, not recommended except for human-readable outputs   </summary>
        public  float roll;
            /// <summary>Attitude pitch expressed as Euler angles, not recommended except for human-readable outputs   </summary>
        public  float pitch;
            /// <summary>Attitude yaw expressed as Euler angles, not recommended except for human-readable outputs   </summary>
        public  float yaw;
            /// <summary>X acceleration m/s/s  [m/s/s] </summary>
        public  float xacc;
            /// <summary>Y acceleration m/s/s  [m/s/s] </summary>
        public  float yacc;
            /// <summary>Z acceleration m/s/s  [m/s/s] </summary>
        public  float zacc;
            /// <summary>Angular speed around X axis rad/s  [rad/s] </summary>
        public  float xgyro;
            /// <summary>Angular speed around Y axis rad/s  [rad/s] </summary>
        public  float ygyro;
            /// <summary>Angular speed around Z axis rad/s  [rad/s] </summary>
        public  float zgyro;
            /// <summary>Latitude in degrees  [deg] </summary>
        public  float lat;
            /// <summary>Longitude in degrees  [deg] </summary>
        public  float lon;
            /// <summary>Altitude in meters  [m] </summary>
        public  float alt;
            /// <summary>Horizontal position standard deviation   </summary>
        public  float std_dev_horz;
            /// <summary>Vertical position standard deviation   </summary>
        public  float std_dev_vert;
            /// <summary>True velocity in m/s in NORTH direction in earth-fixed NED frame  [m/s] </summary>
        public  float vn;
            /// <summary>True velocity in m/s in EAST direction in earth-fixed NED frame  [m/s] </summary>
        public  float ve;
            /// <summary>True velocity in m/s in DOWN direction in earth-fixed NED frame  [m/s] </summary>
        public  float vd;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> Status generated by radio and injected into MAVLink stream. </summary>
    public struct mavlink_radio_status_t
    {
        /// <summary>Receive errors   </summary>
        public  ushort rxerrors;
            /// <summary>Count of error corrected packets   </summary>
        public  ushort @fixed;
            /// <summary>Local signal strength   </summary>
        public  byte rssi;
            /// <summary>Remote signal strength   </summary>
        public  byte remrssi;
            /// <summary>Remaining free buffer space in percent.  [%] </summary>
        public  byte txbuf;
            /// <summary>Background noise level   </summary>
        public  byte noise;
            /// <summary>Remote background noise level   </summary>
        public  byte remnoise;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=254)]
    ///<summary> File transfer message </summary>
    public struct mavlink_file_transfer_protocol_t
    {
        /// <summary>Network ID (0 for broadcast)   </summary>
        public  byte target_network;
            /// <summary>System ID (0 for broadcast)   </summary>
        public  byte target_system;
            /// <summary>Component ID (0 for broadcast)   </summary>
        public  byte target_component;
            /// <summary>Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=251)]
		public byte[] payload;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=16)]
    ///<summary> Time synchronization message. </summary>
    public struct mavlink_timesync_t
    {
        /// <summary>Time sync timestamp 1   </summary>
        public  long tc1;
            /// <summary>Time sync timestamp 2   </summary>
        public  long ts1;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> Camera-IMU triggering and synchronisation message. </summary>
    public struct mavlink_camera_trigger_t
    {
        /// <summary>Timestamp for the image frame in microseconds  [us] </summary>
        public  ulong time_usec;
            /// <summary>Image frame sequence   </summary>
        public  uint seq;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=36)]
    ///<summary> The global position, as returned by the Global Positioning System (GPS). This is    
///                 NOT the global position estimate of the sytem, but rather a RAW sensor value. See message GLOBAL_POSITION for the global position estimate. </summary>
    public struct mavlink_hil_gps_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Latitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude (AMSL, not WGS84), in meters * 1000 (positive for up)  [mm] </summary>
        public  int alt;
            /// <summary>GPS HDOP horizontal dilution of position in cm (m*100). If unknown, set to: 65535   </summary>
        public  ushort eph;
            /// <summary>GPS VDOP vertical dilution of position in cm (m*100). If unknown, set to: 65535   </summary>
        public  ushort epv;
            /// <summary>GPS ground speed in cm/s. If unknown, set to: 65535  [cm/s] </summary>
        public  ushort vel;
            /// <summary>GPS velocity in cm/s in NORTH direction in earth-fixed NED frame  [cm/s] </summary>
        public  short vn;
            /// <summary>GPS velocity in cm/s in EAST direction in earth-fixed NED frame  [cm/s] </summary>
        public  short ve;
            /// <summary>GPS velocity in cm/s in DOWN direction in earth-fixed NED frame  [cm/s] </summary>
        public  short vd;
            /// <summary>Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: 65535  [cdeg] </summary>
        public  ushort cog;
            /// <summary>0-1: no fix, 2: 2D fix, 3: 3D fix. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix.   </summary>
        public  byte fix_type;
            /// <summary>Number of satellites visible. If unknown, set to 255   </summary>
        public  byte satellites_visible;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> Simulated optical flow from a flow sensor (e.g. PX4FLOW or optical mouse sensor) </summary>
    public struct mavlink_hil_optical_flow_t
    {
        /// <summary>Timestamp (microseconds, synced to UNIX time or since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Integration time in microseconds. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the.  [us] </summary>
        public  uint integration_time_us;
            /// <summary>Flow in radians around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.)  [rad] </summary>
        public  float integrated_x;
            /// <summary>Flow in radians around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.)  [rad] </summary>
        public  float integrated_y;
            /// <summary>RH rotation around X axis (rad)  [rad] </summary>
        public  float integrated_xgyro;
            /// <summary>RH rotation around Y axis (rad)  [rad] </summary>
        public  float integrated_ygyro;
            /// <summary>RH rotation around Z axis (rad)  [rad] </summary>
        public  float integrated_zgyro;
            /// <summary>Time in microseconds since the distance was sampled.  [us] </summary>
        public  uint time_delta_distance_us;
            /// <summary>Distance to the center of the flow field in meters. Positive value (including zero): distance known. Negative value: Unknown distance.  [m] </summary>
        public  float distance;
            /// <summary>Temperature * 100 in centi-degrees Celsius  [cdegC] </summary>
        public  short temperature;
            /// <summary>Sensor ID   </summary>
        public  byte sensor_id;
            /// <summary>Optical flow quality / confidence. 0: no valid flow, 255: maximum quality   </summary>
        public  byte quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=64)]
    ///<summary> Sent from simulation to autopilot, avoids in contrast to HIL_STATE singularities. This packet is useful for high throughput applications such as hardware in the loop simulations. </summary>
    public struct mavlink_hil_state_quaternion_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Vehicle attitude expressed as normalized quaternion in w, x, y, z order (with 1 0 0 0 being the null-rotation)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] attitude_quaternion;
            /// <summary>Body frame roll / phi angular speed (rad/s)  [rad/s] </summary>
        public  float rollspeed;
            /// <summary>Body frame pitch / theta angular speed (rad/s)  [rad/s] </summary>
        public  float pitchspeed;
            /// <summary>Body frame yaw / psi angular speed (rad/s)  [rad/s] </summary>
        public  float yawspeed;
            /// <summary>Latitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude in meters, expressed as * 1000 (millimeters)  [mm] </summary>
        public  int alt;
            /// <summary>Ground X Speed (Latitude), expressed as cm/s  [cm/s] </summary>
        public  short vx;
            /// <summary>Ground Y Speed (Longitude), expressed as cm/s  [cm/s] </summary>
        public  short vy;
            /// <summary>Ground Z Speed (Altitude), expressed as cm/s  [cm/s] </summary>
        public  short vz;
            /// <summary>Indicated airspeed, expressed as cm/s  [cm/s] </summary>
        public  ushort ind_airspeed;
            /// <summary>True airspeed, expressed as cm/s  [cm/s] </summary>
        public  ushort true_airspeed;
            /// <summary>X acceleration (mg)  [mG] </summary>
        public  short xacc;
            /// <summary>Y acceleration (mg)  [mG] </summary>
        public  short yacc;
            /// <summary>Z acceleration (mg)  [mG] </summary>
        public  short zacc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The RAW IMU readings for secondary 9DOF sensor setup. This message should contain the scaled values to the described units </summary>
    public struct mavlink_scaled_imu2_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>X acceleration (mg)  [mG] </summary>
        public  short xacc;
            /// <summary>Y acceleration (mg)  [mG] </summary>
        public  short yacc;
            /// <summary>Z acceleration (mg)  [mG] </summary>
        public  short zacc;
            /// <summary>Angular speed around X axis (millirad /sec)  [mrad/s] </summary>
        public  short xgyro;
            /// <summary>Angular speed around Y axis (millirad /sec)  [mrad/s] </summary>
        public  short ygyro;
            /// <summary>Angular speed around Z axis (millirad /sec)  [mrad/s] </summary>
        public  short zgyro;
            /// <summary>X Magnetic field (milli tesla)  [mT] </summary>
        public  short xmag;
            /// <summary>Y Magnetic field (milli tesla)  [mT] </summary>
        public  short ymag;
            /// <summary>Z Magnetic field (milli tesla)  [mT] </summary>
        public  short zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Request a list of available logs. On some systems calling this may stop on-board logging until LOG_REQUEST_END is called. </summary>
    public struct mavlink_log_request_list_t
    {
        /// <summary>First log id (0 for first available)   </summary>
        public  ushort start;
            /// <summary>Last log id (0xffff for last available)   </summary>
        public  ushort end;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Reply to LOG_REQUEST_LIST </summary>
    public struct mavlink_log_entry_t
    {
        /// <summary>UTC timestamp of log in seconds since 1970, or 0 if not available  [s] </summary>
        public  uint time_utc;
            /// <summary>Size of the log (may be approximate) in bytes  [bytes] </summary>
        public  uint size;
            /// <summary>Log id   </summary>
        public  ushort id;
            /// <summary>Total number of logs   </summary>
        public  ushort num_logs;
            /// <summary>High log number   </summary>
        public  ushort last_log_num;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> Request a chunk of a log </summary>
    public struct mavlink_log_request_data_t
    {
        /// <summary>Offset into the log   </summary>
        public  uint ofs;
            /// <summary>Number of bytes  [bytes] </summary>
        public  uint count;
            /// <summary>Log id (from LOG_ENTRY reply)   </summary>
        public  ushort id;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=97)]
    ///<summary> Reply to LOG_REQUEST_DATA </summary>
    public struct mavlink_log_data_t
    {
        /// <summary>Offset into the log   </summary>
        public  uint ofs;
            /// <summary>Log id (from LOG_ENTRY reply)   </summary>
        public  ushort id;
            /// <summary>Number of bytes (zero for end of log)  [bytes] </summary>
        public  byte count;
            /// <summary>log data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=90)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Erase all logs </summary>
    public struct mavlink_log_erase_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Stop log transfer and resume normal logging </summary>
    public struct mavlink_log_request_end_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=113)]
    ///<summary> data for injecting into the onboard GPS (used for DGPS) </summary>
    public struct mavlink_gps_inject_data_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>data length  [bytes] </summary>
        public  byte len;
            /// <summary>raw data (110 is enough for 12 satellites of RTCMv2)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=110)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    ///<summary> Second GPS data. </summary>
    public struct mavlink_gps2_raw_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Latitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude (AMSL, not WGS84), in meters * 1000 (positive for up)  [mm] </summary>
        public  int alt;
            /// <summary>Age of DGPS info  [ms] </summary>
        public  uint dgps_age;
            /// <summary>GPS HDOP horizontal dilution of position in cm (m*100). If unknown, set to: UINT16_MAX  [cm] </summary>
        public  ushort eph;
            /// <summary>GPS VDOP vertical dilution of position in cm (m*100). If unknown, set to: UINT16_MAX  [cm] </summary>
        public  ushort epv;
            /// <summary>GPS ground speed (m/s * 100). If unknown, set to: UINT16_MAX  [cm/s] </summary>
        public  ushort vel;
            /// <summary>Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX  [cdeg] </summary>
        public  ushort cog;
            /// <summary>See the GPS_FIX_TYPE enum. GPS_FIX_TYPE  </summary>
        public  /*GPS_FIX_TYPE*/byte fix_type;
            /// <summary>Number of satellites visible. If unknown, set to 255   </summary>
        public  byte satellites_visible;
            /// <summary>Number of DGPS satellites   </summary>
        public  byte dgps_numch;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Power supply status </summary>
    public struct mavlink_power_status_t
    {
        /// <summary>5V rail voltage in millivolts  [mV] </summary>
        public  ushort Vcc;
            /// <summary>servo rail voltage in millivolts  [mV] </summary>
        public  ushort Vservo;
            /// <summary>power supply status flags (see MAV_POWER_STATUS enum) MAV_POWER_STATUS  bitmask</summary>
        public  /*MAV_POWER_STATUS*/ushort flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=79)]
    ///<summary> Control a serial port. This can be used for raw access to an onboard serial peripheral such as a GPS or telemetry radio. It is designed to make it possible to update the devices firmware via MAVLink messages or change the devices settings. A message with zero bytes can be used to change just the baudrate. </summary>
    public struct mavlink_serial_control_t
    {
        /// <summary>Baudrate of transfer. Zero means no change.  [bits/s] </summary>
        public  uint baudrate;
            /// <summary>Timeout for reply data in milliseconds  [ms] </summary>
        public  ushort timeout;
            /// <summary>See SERIAL_CONTROL_DEV enum SERIAL_CONTROL_DEV  </summary>
        public  /*SERIAL_CONTROL_DEV*/byte device;
            /// <summary>See SERIAL_CONTROL_FLAG enum SERIAL_CONTROL_FLAG  bitmask</summary>
        public  /*SERIAL_CONTROL_FLAG*/byte flags;
            /// <summary>how many bytes in this transfer  [bytes] </summary>
        public  byte count;
            /// <summary>serial data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=70)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    ///<summary> RTK GPS data. Gives information on the relative baseline calculation the GPS is reporting </summary>
    public struct mavlink_gps_rtk_t
    {
        /// <summary>Time since boot of last baseline message received in ms.  [ms] </summary>
        public  uint time_last_baseline_ms;
            /// <summary>GPS Time of Week of last baseline  [ms] </summary>
        public  uint tow;
            /// <summary>Current baseline in ECEF x or NED north component in mm.  [mm] </summary>
        public  int baseline_a_mm;
            /// <summary>Current baseline in ECEF y or NED east component in mm.  [mm] </summary>
        public  int baseline_b_mm;
            /// <summary>Current baseline in ECEF z or NED down component in mm.  [mm] </summary>
        public  int baseline_c_mm;
            /// <summary>Current estimate of baseline accuracy.   </summary>
        public  uint accuracy;
            /// <summary>Current number of integer ambiguity hypotheses.   </summary>
        public  int iar_num_hypotheses;
            /// <summary>GPS Week Number of last baseline   </summary>
        public  ushort wn;
            /// <summary>Identification of connected RTK receiver.   </summary>
        public  byte rtk_receiver_id;
            /// <summary>GPS-specific health report for RTK data.   </summary>
        public  byte rtk_health;
            /// <summary>Rate of baseline messages being received by GPS, in HZ  [Hz] </summary>
        public  byte rtk_rate;
            /// <summary>Current number of sats used for RTK calculation.   </summary>
        public  byte nsats;
            /// <summary>Coordinate system of baseline RTK_BASELINE_COORDINATE_SYSTEM  </summary>
        public  /*RTK_BASELINE_COORDINATE_SYSTEM*/byte baseline_coords_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    ///<summary> RTK GPS data. Gives information on the relative baseline calculation the GPS is reporting </summary>
    public struct mavlink_gps2_rtk_t
    {
        /// <summary>Time since boot of last baseline message received in ms.  [ms] </summary>
        public  uint time_last_baseline_ms;
            /// <summary>GPS Time of Week of last baseline  [ms] </summary>
        public  uint tow;
            /// <summary>Current baseline in ECEF x or NED north component in mm.  [mm] </summary>
        public  int baseline_a_mm;
            /// <summary>Current baseline in ECEF y or NED east component in mm.  [mm] </summary>
        public  int baseline_b_mm;
            /// <summary>Current baseline in ECEF z or NED down component in mm.  [mm] </summary>
        public  int baseline_c_mm;
            /// <summary>Current estimate of baseline accuracy.   </summary>
        public  uint accuracy;
            /// <summary>Current number of integer ambiguity hypotheses.   </summary>
        public  int iar_num_hypotheses;
            /// <summary>GPS Week Number of last baseline   </summary>
        public  ushort wn;
            /// <summary>Identification of connected RTK receiver.   </summary>
        public  byte rtk_receiver_id;
            /// <summary>GPS-specific health report for RTK data.   </summary>
        public  byte rtk_health;
            /// <summary>Rate of baseline messages being received by GPS, in HZ  [Hz] </summary>
        public  byte rtk_rate;
            /// <summary>Current number of sats used for RTK calculation.   </summary>
        public  byte nsats;
            /// <summary>Coordinate system of baseline RTK_BASELINE_COORDINATE_SYSTEM  </summary>
        public  /*RTK_BASELINE_COORDINATE_SYSTEM*/byte baseline_coords_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The RAW IMU readings for 3rd 9DOF sensor setup. This message should contain the scaled values to the described units </summary>
    public struct mavlink_scaled_imu3_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>X acceleration (mg)  [mG] </summary>
        public  short xacc;
            /// <summary>Y acceleration (mg)  [mG] </summary>
        public  short yacc;
            /// <summary>Z acceleration (mg)  [mG] </summary>
        public  short zacc;
            /// <summary>Angular speed around X axis (millirad /sec)  [mrad/s] </summary>
        public  short xgyro;
            /// <summary>Angular speed around Y axis (millirad /sec)  [mrad/s] </summary>
        public  short ygyro;
            /// <summary>Angular speed around Z axis (millirad /sec)  [mrad/s] </summary>
        public  short zgyro;
            /// <summary>X Magnetic field (milli tesla)  [mT] </summary>
        public  short xmag;
            /// <summary>Y Magnetic field (milli tesla)  [mT] </summary>
        public  short ymag;
            /// <summary>Z Magnetic field (milli tesla)  [mT] </summary>
        public  short zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=13)]
    ///<summary>  </summary>
    public struct mavlink_data_transmission_handshake_t
    {
        /// <summary>total data size in bytes (set on ACK only)  [bytes] </summary>
        public  uint size;
            /// <summary>Width of a matrix or image   </summary>
        public  ushort width;
            /// <summary>Height of a matrix or image   </summary>
        public  ushort height;
            /// <summary>number of packets beeing sent (set on ACK only)   </summary>
        public  ushort packets;
            /// <summary>type of requested/acknowledged data (as defined in ENUM DATA_TYPES in mavlink/include/mavlink_types.h)   </summary>
        public  byte type;
            /// <summary>payload size per packet (normally 253 byte, see DATA field size in message ENCAPSULATED_DATA) (set on ACK only)  [bytes] </summary>
        public  byte payload;
            /// <summary>JPEG quality out of [1,100]  [%] </summary>
        public  byte jpg_quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    ///<summary>  </summary>
    public struct mavlink_encapsulated_data_t
    {
        /// <summary>sequence number (starting with 0 on every transmission)   </summary>
        public  ushort seqnr;
            /// <summary>image data bytes   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=253)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary>  </summary>
    public struct mavlink_distance_sensor_t
    {
        /// <summary>Time since system boot  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Minimum distance the sensor can measure in centimeters  [cm] </summary>
        public  ushort min_distance;
            /// <summary>Maximum distance the sensor can measure in centimeters  [cm] </summary>
        public  ushort max_distance;
            /// <summary>Current distance reading  [cm] </summary>
        public  ushort current_distance;
            /// <summary>Type from MAV_DISTANCE_SENSOR enum. MAV_DISTANCE_SENSOR  </summary>
        public  /*MAV_DISTANCE_SENSOR*/byte type;
            /// <summary>Onboard ID of the sensor   </summary>
        public  byte id;
            /// <summary>Direction the sensor faces from MAV_SENSOR_ORIENTATION enum. downward-facing: ROTATION_PITCH_270, upward-facing: ROTATION_PITCH_90, backward-facing: ROTATION_PITCH_180, forward-facing: ROTATION_NONE, left-facing: ROTATION_YAW_90, right-facing: ROTATION_YAW_270 MAV_SENSOR_ORIENTATION  </summary>
        public  /*MAV_SENSOR_ORIENTATION*/byte orientation;
            /// <summary>Measurement covariance in centimeters, 0 for unknown / invalid readings  [cm] </summary>
        public  byte covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    ///<summary> Request for terrain data and terrain status </summary>
    public struct mavlink_terrain_request_t
    {
        /// <summary>Bitmask of requested 4x4 grids (row major 8x7 array of grids, 56 bits)   bitmask</summary>
        public  ulong mask;
            /// <summary>Latitude of SW corner of first grid (degrees *10^7)  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude of SW corner of first grid (in degrees *10^7)  [degE7] </summary>
        public  int lon;
            /// <summary>Grid spacing in meters  [m] </summary>
        public  ushort grid_spacing;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=43)]
    ///<summary> Terrain data sent from GCS. The lat/lon and grid_spacing must be the same as a lat/lon from a TERRAIN_REQUEST </summary>
    public struct mavlink_terrain_data_t
    {
        /// <summary>Latitude of SW corner of first grid (degrees *10^7)  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude of SW corner of first grid (in degrees *10^7)  [degE7] </summary>
        public  int lon;
            /// <summary>Grid spacing in meters  [m] </summary>
        public  ushort grid_spacing;
            /// <summary>Terrain data in meters AMSL  [m] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public Int16[] data;
            /// <summary>bit within the terrain request mask   </summary>
        public  byte gridbit;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> Request that the vehicle report terrain height at the given location. Used by GCS to check if vehicle has all terrain data needed for a mission. </summary>
    public struct mavlink_terrain_check_t
    {
        /// <summary>Latitude (degrees *10^7)  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude (degrees *10^7)  [degE7] </summary>
        public  int lon;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> Response from a TERRAIN_CHECK request </summary>
    public struct mavlink_terrain_report_t
    {
        /// <summary>Latitude (degrees *10^7)  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude (degrees *10^7)  [degE7] </summary>
        public  int lon;
            /// <summary>Terrain height in meters AMSL  [m] </summary>
        public  float terrain_height;
            /// <summary>Current vehicle height above lat/lon terrain height (meters)  [m] </summary>
        public  float current_height;
            /// <summary>grid spacing (zero if terrain at this location unavailable)   </summary>
        public  ushort spacing;
            /// <summary>Number of 4x4 terrain blocks waiting to be received or read from disk   </summary>
        public  ushort pending;
            /// <summary>Number of 4x4 terrain blocks in memory   </summary>
        public  ushort loaded;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Barometer readings for 2nd barometer </summary>
    public struct mavlink_scaled_pressure2_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Absolute pressure (hectopascal)  [hPa] </summary>
        public  float press_abs;
            /// <summary>Differential pressure 1 (hectopascal)  [hPa] </summary>
        public  float press_diff;
            /// <summary>Temperature measurement (0.01 degrees celsius)  [cdegC] </summary>
        public  short temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=120)]
    ///<summary> Motion capture attitude and position </summary>
    public struct mavlink_att_pos_mocap_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>X position in meters (NED)  [m] </summary>
        public  float x;
            /// <summary>Y position in meters (NED)  [m] </summary>
        public  float y;
            /// <summary>Z position in meters (NED)  [m] </summary>
        public  float z;
            /// <summary>Pose covariance matrix upper right triangular (first six entries are the first ROW, next five entries are the second ROW, etc.)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=43)]
    ///<summary> Set the vehicle attitude and body angular rates. </summary>
    public struct mavlink_set_actuator_control_target_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public float[] controls;
            /// <summary>Actuator group. The "_mlx" indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances.   </summary>
        public  byte group_mlx;
            /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=41)]
    ///<summary> Set the vehicle attitude and body angular rates. </summary>
    public struct mavlink_actuator_control_target_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public float[] controls;
            /// <summary>Actuator group. The "_mlx" indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances.   </summary>
        public  byte group_mlx;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    ///<summary> The current system altitude. </summary>
    public struct mavlink_altitude_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>This altitude measure is initialized on system boot and monotonic (it is never reset, but represents the local altitude change). The only guarantee on this field is that it will never be reset and is consistent within a flight. The recommended value for this field is the uncorrected barometric altitude at boot time. This altitude will also drift and vary between flights.  [m] </summary>
        public  float altitude_monotonic;
            /// <summary>This altitude measure is strictly above mean sea level and might be non-monotonic (it might reset on events like GPS lock or when a new QNH value is set). It should be the altitude to which global altitude waypoints are compared to. Note that it is *not* the GPS altitude, however, most GPS modules already output AMSL by default and not the WGS84 altitude.  [m] </summary>
        public  float altitude_amsl;
            /// <summary>This is the local altitude in the local coordinate frame. It is not the altitude above home, but in reference to the coordinate origin (0, 0, 0). It is up-positive.  [m] </summary>
        public  float altitude_local;
            /// <summary>This is the altitude above the home position. It resets on each change of the current home position.  [m] </summary>
        public  float altitude_relative;
            /// <summary>This is the altitude above terrain. It might be fed by a terrain database or an altimeter. Values smaller than -1000 should be interpreted as unknown.  [m] </summary>
        public  float altitude_terrain;
            /// <summary>This is not the altitude, but the clear space below the system according to the fused clearance estimate. It generally should max out at the maximum range of e.g. the laser altimeter. It is generally a moving target. A negative value indicates no measurement available.  [m] </summary>
        public  float bottom_clearance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=243)]
    ///<summary> The autopilot is requesting a resource (file, binary, other type of data) </summary>
    public struct mavlink_resource_request_t
    {
        /// <summary>Request ID. This ID should be re-used when sending back URI contents   </summary>
        public  byte request_id;
            /// <summary>The type of requested URI. 0 = a file via URL. 1 = a UAVCAN binary   </summary>
        public  byte uri_type;
            /// <summary>The requested unique resource identifier (URI). It is not necessarily a straight domain name (depends on the URI type enum)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=120)]
		public byte[] uri;
            /// <summary>The way the autopilot wants to receive the URI. 0 = MAVLink FTP. 1 = binary stream.   </summary>
        public  byte transfer_type;
            /// <summary>The storage path the autopilot wants the URI to be stored in. Will only be valid if the transfer_type has a storage associated (e.g. MAVLink FTP).   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=120)]
		public byte[] storage;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Barometer readings for 3rd barometer </summary>
    public struct mavlink_scaled_pressure3_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Absolute pressure (hectopascal)  [hPa] </summary>
        public  float press_abs;
            /// <summary>Differential pressure 1 (hectopascal)  [hPa] </summary>
        public  float press_diff;
            /// <summary>Temperature measurement (0.01 degrees celsius)  [cdegC] </summary>
        public  short temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=93)]
    ///<summary> current motion information from a designated system </summary>
    public struct mavlink_follow_target_t
    {
        /// <summary>Timestamp in milliseconds since system boot  [ms] </summary>
        public  ulong timestamp;
            /// <summary>button states or switches of a tracker device   </summary>
        public  ulong custom_state;
            /// <summary>Latitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>AMSL, in meters  [m] </summary>
        public  float alt;
            /// <summary>target velocity (0,0,0) for unknown  [m/s] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] vel;
            /// <summary>linear target acceleration (0,0,0) for unknown  [m/s/s] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] acc;
            /// <summary>(1 0 0 0 for unknown)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] attitude_q;
            /// <summary>(0 0 0 for unknown)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] rates;
            /// <summary>eph epv   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] position_cov;
            /// <summary>bit positions for tracker reporting capabilities (POS = 0, VEL = 1, ACCEL = 2, ATT + RATES = 3)   </summary>
        public  byte est_capabilities;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=100)]
    ///<summary> The smoothed, monotonic system state used to feed the control loops of the system. </summary>
    public struct mavlink_control_system_state_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>X acceleration in body frame  [m/s/s] </summary>
        public  float x_acc;
            /// <summary>Y acceleration in body frame  [m/s/s] </summary>
        public  float y_acc;
            /// <summary>Z acceleration in body frame  [m/s/s] </summary>
        public  float z_acc;
            /// <summary>X velocity in body frame  [m/s] </summary>
        public  float x_vel;
            /// <summary>Y velocity in body frame  [m/s] </summary>
        public  float y_vel;
            /// <summary>Z velocity in body frame  [m/s] </summary>
        public  float z_vel;
            /// <summary>X position in local frame  [m] </summary>
        public  float x_pos;
            /// <summary>Y position in local frame  [m] </summary>
        public  float y_pos;
            /// <summary>Z position in local frame  [m] </summary>
        public  float z_pos;
            /// <summary>Airspeed, set to -1 if unknown  [m/s] </summary>
        public  float airspeed;
            /// <summary>Variance of body velocity estimate   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] vel_variance;
            /// <summary>Variance in local position   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] pos_variance;
            /// <summary>The attitude, represented as Quaternion   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Angular rate in roll axis  [rad/s] </summary>
        public  float roll_rate;
            /// <summary>Angular rate in pitch axis  [rad/s] </summary>
        public  float pitch_rate;
            /// <summary>Angular rate in yaw axis  [rad/s] </summary>
        public  float yaw_rate;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=41)]
    ///<summary> Battery information </summary>
    public struct mavlink_battery_status_t
    {
        /// <summary>Consumed charge, in milliampere hours (1 = 1 mAh), -1: autopilot does not provide mAh consumption estimate  [mAh] </summary>
        public  int current_consumed;
            /// <summary>Consumed energy, in HectoJoules (intergrated U*I*dt)  (1 = 100 Joule), -1: autopilot does not provide energy consumption estimate  [hJ] </summary>
        public  int energy_consumed;
            /// <summary>Temperature of the battery in centi-degrees celsius. INT16_MAX for unknown temperature.  [cdegC] </summary>
        public  short temperature;
            /// <summary>Battery voltage of cells, in millivolts (1 = 1 millivolt). Cells above the valid cell count for this battery should have the UINT16_MAX value.  [mV] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public UInt16[] voltages;
            /// <summary>Battery current, in 10*milliamperes (1 = 10 milliampere), -1: autopilot does not measure the current  [cA] </summary>
        public  short current_battery;
            /// <summary>Battery ID   </summary>
        public  byte id;
            /// <summary>Function of the battery MAV_BATTERY_FUNCTION  </summary>
        public  /*MAV_BATTERY_FUNCTION*/byte battery_function;
            /// <summary>Type (chemistry) of the battery MAV_BATTERY_TYPE  </summary>
        public  /*MAV_BATTERY_TYPE*/byte type;
            /// <summary>Remaining battery energy: (0%: 0, 100%: 100), -1: autopilot does not estimate the remaining battery  [%] </summary>
        public  byte battery_remaining;
            /// <summary>Remaining battery time, in seconds (1 = 1s = 0% energy left), 0: autopilot does not provide remaining battery time estimate  [s] </summary>
        public  int time_remaining;
            /// <summary>State for extent of discharge, provided by autopilot for warning or external reactions MAV_BATTERY_CHARGE_STATE  </summary>
        public  /*MAV_BATTERY_CHARGE_STATE*/byte charge_state;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=78)]
    ///<summary> Version and capability of autopilot software </summary>
    public struct mavlink_autopilot_version_t
    {
        /// <summary>bitmask of capabilities (see MAV_PROTOCOL_CAPABILITY enum) MAV_PROTOCOL_CAPABILITY  bitmask</summary>
        public  /*MAV_PROTOCOL_CAPABILITY*/ulong capabilities;
            /// <summary>UID if provided by hardware (see uid2)   </summary>
        public  ulong uid;
            /// <summary>Firmware version number   </summary>
        public  uint flight_sw_version;
            /// <summary>Middleware version number   </summary>
        public  uint middleware_sw_version;
            /// <summary>Operating system version number   </summary>
        public  uint os_sw_version;
            /// <summary>HW / board version (last 8 bytes should be silicon ID, if any)   </summary>
        public  uint board_version;
            /// <summary>ID of the board vendor   </summary>
        public  ushort vendor_id;
            /// <summary>ID of the product   </summary>
        public  ushort product_id;
            /// <summary>Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] flight_custom_version;
            /// <summary>Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] middleware_custom_version;
            /// <summary>Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] os_custom_version;
            /// <summary>UID if provided by hardware (supersedes the uid field. If this is non-zero, use this field, otherwise use uid)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=18)]
		public byte[] uid2;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=60)]
    ///<summary> The location of a landing area captured from a downward facing camera </summary>
    public struct mavlink_landing_target_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>X-axis angular offset (in radians) of the target from the center of the image  [rad] </summary>
        public  float angle_x;
            /// <summary>Y-axis angular offset (in radians) of the target from the center of the image  [rad] </summary>
        public  float angle_y;
            /// <summary>Distance to the target from the vehicle in meters  [m] </summary>
        public  float distance;
            /// <summary>Size in radians of target along x-axis  [rad] </summary>
        public  float size_x;
            /// <summary>Size in radians of target along y-axis  [rad] </summary>
        public  float size_y;
            /// <summary>The ID of the target if multiple targets are present   </summary>
        public  byte target_num;
            /// <summary>MAV_FRAME enum specifying the whether the following feilds are earth-frame, body-frame, etc. MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte frame;
            /// <summary>X Position of the landing target on MAV_FRAME  [m] </summary>
        public  float x;
            /// <summary>Y Position of the landing target on MAV_FRAME  [m] </summary>
        public  float y;
            /// <summary>Z Position of the landing target on MAV_FRAME  [m] </summary>
        public  float z;
            /// <summary>Quaternion of landing target orientation (w, x, y, z order, zero-rotation is 1, 0, 0, 0)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>LANDING_TARGET_TYPE enum specifying the type of landing target LANDING_TARGET_TYPE  </summary>
        public  /*LANDING_TARGET_TYPE*/byte type;
            /// <summary>Boolean indicating known position (1) or default unkown position (0), for validation of positioning of the landing target   </summary>
        public  byte position_valid;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> Estimator status message including flags, innovation test ratios and estimated accuracies. The flags message is an integer bitmask containing information on which EKF outputs are valid. See the ESTIMATOR_STATUS_FLAGS enum definition for further information. The innovaton test ratios show the magnitude of the sensor innovation divided by the innovation check threshold. Under normal operation the innovaton test ratios should be below 0.5 with occasional values up to 1.0. Values greater than 1.0 should be rare under normal operation and indicate that a measurement has been rejected by the filter. The user should be notified if an innovation test ratio greater than 1.0 is recorded. Notifications for values in the range between 0.5 and 1.0 should be optional and controllable by the user. </summary>
    public struct mavlink_estimator_status_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Velocity innovation test ratio   </summary>
        public  float vel_ratio;
            /// <summary>Horizontal position innovation test ratio   </summary>
        public  float pos_horiz_ratio;
            /// <summary>Vertical position innovation test ratio   </summary>
        public  float pos_vert_ratio;
            /// <summary>Magnetometer innovation test ratio   </summary>
        public  float mag_ratio;
            /// <summary>Height above terrain innovation test ratio   </summary>
        public  float hagl_ratio;
            /// <summary>True airspeed innovation test ratio   </summary>
        public  float tas_ratio;
            /// <summary>Horizontal position 1-STD accuracy relative to the EKF local origin (m)  [m] </summary>
        public  float pos_horiz_accuracy;
            /// <summary>Vertical position 1-STD accuracy relative to the EKF local origin (m)  [m] </summary>
        public  float pos_vert_accuracy;
            /// <summary>Integer bitmask indicating which EKF outputs are valid. See definition for ESTIMATOR_STATUS_FLAGS. ESTIMATOR_STATUS_FLAGS  bitmask</summary>
        public  /*ESTIMATOR_STATUS_FLAGS*/ushort flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=40)]
    ///<summary>  </summary>
    public struct mavlink_wind_cov_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Wind in X (NED) direction in m/s  [m/s] </summary>
        public  float wind_x;
            /// <summary>Wind in Y (NED) direction in m/s  [m/s] </summary>
        public  float wind_y;
            /// <summary>Wind in Z (NED) direction in m/s  [m/s] </summary>
        public  float wind_z;
            /// <summary>Variability of the wind in XY. RMS of a 1 Hz lowpassed wind estimate.  [m/s] </summary>
        public  float var_horiz;
            /// <summary>Variability of the wind in Z. RMS of a 1 Hz lowpassed wind estimate.  [m/s] </summary>
        public  float var_vert;
            /// <summary>AMSL altitude (m) this measurement was taken at  [m] </summary>
        public  float wind_alt;
            /// <summary>Horizontal speed 1-STD accuracy  [m] </summary>
        public  float horiz_accuracy;
            /// <summary>Vertical speed 1-STD accuracy  [m] </summary>
        public  float vert_accuracy;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=63)]
    ///<summary> GPS sensor input message.  This is a raw sensor value sent by the GPS. This is NOT the global position estimate of the sytem. </summary>
    public struct mavlink_gps_input_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>GPS time (milliseconds from start of GPS week)  [ms] </summary>
        public  uint time_week_ms;
            /// <summary>Latitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude (AMSL, not WGS84), in m (positive for up)  [m] </summary>
        public  float alt;
            /// <summary>GPS HDOP horizontal dilution of position in m  [m] </summary>
        public  float hdop;
            /// <summary>GPS VDOP vertical dilution of position in m  [m] </summary>
        public  float vdop;
            /// <summary>GPS velocity in m/s in NORTH direction in earth-fixed NED frame  [m/s] </summary>
        public  float vn;
            /// <summary>GPS velocity in m/s in EAST direction in earth-fixed NED frame  [m/s] </summary>
        public  float ve;
            /// <summary>GPS velocity in m/s in DOWN direction in earth-fixed NED frame  [m/s] </summary>
        public  float vd;
            /// <summary>GPS speed accuracy in m/s  [m/s] </summary>
        public  float speed_accuracy;
            /// <summary>GPS horizontal accuracy in m  [m] </summary>
        public  float horiz_accuracy;
            /// <summary>GPS vertical accuracy in m  [m] </summary>
        public  float vert_accuracy;
            /// <summary>Flags indicating which fields to ignore (see GPS_INPUT_IGNORE_FLAGS enum).  All other fields must be provided. GPS_INPUT_IGNORE_FLAGS  bitmask</summary>
        public  /*GPS_INPUT_IGNORE_FLAGS*/ushort ignore_flags;
            /// <summary>GPS week number   </summary>
        public  ushort time_week;
            /// <summary>ID of the GPS for multiple GPS inputs   </summary>
        public  byte gps_id;
            /// <summary>0-1: no fix, 2: 2D fix, 3: 3D fix. 4: 3D with DGPS. 5: 3D with RTK   </summary>
        public  byte fix_type;
            /// <summary>Number of satellites visible.   </summary>
        public  byte satellites_visible;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=182)]
    ///<summary> RTCM message for injecting into the onboard GPS (used for DGPS) </summary>
    public struct mavlink_gps_rtcm_data_t
    {
        /// <summary>LSB: 1 means message is fragmented, next 2 bits are the fragment ID, the remaining 5 bits are used for the sequence ID. Messages are only to be flushed to the GPS when the entire message has been reconstructed on the autopilot. The fragment ID specifies which order the fragments should be assembled into a buffer, while the sequence ID is used to detect a mismatch between different buffers. The buffer is considered fully reconstructed when either all 4 fragments are present, or all the fragments before the first fragment with a non full payload is received. This management is used to ensure that normal GPS operation doesn't corrupt RTCM data, and to recover from a unreliable transport delivery order.   </summary>
        public  byte flags;
            /// <summary>data length  [bytes] </summary>
        public  byte len;
            /// <summary>RTCM message (may be fragmented)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=180)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=40)]
    ///<summary> Message appropriate for high latency connections like Iridium </summary>
    public struct mavlink_high_latency_t
    {
        /// <summary>A bitfield for use for autopilot-specific flags.   bitmask</summary>
        public  uint custom_mode;
            /// <summary>Latitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int latitude;
            /// <summary>Longitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int longitude;
            /// <summary>roll (centidegrees)  [cdeg] </summary>
        public  short roll;
            /// <summary>pitch (centidegrees)  [cdeg] </summary>
        public  short pitch;
            /// <summary>heading (centidegrees)  [cdeg] </summary>
        public  ushort heading;
            /// <summary>heading setpoint (centidegrees)  [cdeg] </summary>
        public  short heading_sp;
            /// <summary>Altitude above mean sea level (meters)  [m] </summary>
        public  short altitude_amsl;
            /// <summary>Altitude setpoint relative to the home position (meters)  [m] </summary>
        public  short altitude_sp;
            /// <summary>distance to target  [m] </summary>
        public  ushort wp_distance;
            /// <summary>System mode bitfield, as defined by MAV_MODE_FLAG enum. MAV_MODE_FLAG  bitmask</summary>
        public  /*MAV_MODE_FLAG*/byte base_mode;
            /// <summary>The landed state. Is set to MAV_LANDED_STATE_UNDEFINED if landed state is unknown. MAV_LANDED_STATE  </summary>
        public  /*MAV_LANDED_STATE*/byte landed_state;
            /// <summary>throttle (percentage)  [%] </summary>
        public  byte throttle;
            /// <summary>airspeed (m/s)  [m/s] </summary>
        public  byte airspeed;
            /// <summary>airspeed setpoint (m/s)  [m/s] </summary>
        public  byte airspeed_sp;
            /// <summary>groundspeed (m/s)  [m/s] </summary>
        public  byte groundspeed;
            /// <summary>climb rate (m/s)  [m/s] </summary>
        public  byte climb_rate;
            /// <summary>Number of satellites visible. If unknown, set to 255   </summary>
        public  byte gps_nsat;
            /// <summary>See the GPS_FIX_TYPE enum. GPS_FIX_TYPE  </summary>
        public  /*GPS_FIX_TYPE*/byte gps_fix_type;
            /// <summary>Remaining battery (percentage)  [%] </summary>
        public  byte battery_remaining;
            /// <summary>Autopilot temperature (degrees C)  [degC] </summary>
        public  byte temperature;
            /// <summary>Air temperature (degrees C) from airspeed sensor  [degC] </summary>
        public  byte temperature_air;
            /// <summary>failsafe (each bit represents a failsafe where 0=ok, 1=failsafe active (bit0:RC, bit1:batt, bit2:GPS, bit3:GCS, bit4:fence)   </summary>
        public  byte failsafe;
            /// <summary>current waypoint number   </summary>
        public  byte wp_num;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    ///<summary> Vibration levels and accelerometer clipping </summary>
    public struct mavlink_vibration_t
    {
        /// <summary>Timestamp (micros since boot or Unix epoch)  [us] </summary>
        public  ulong time_usec;
            /// <summary>Vibration levels on X-axis   </summary>
        public  float vibration_x;
            /// <summary>Vibration levels on Y-axis   </summary>
        public  float vibration_y;
            /// <summary>Vibration levels on Z-axis   </summary>
        public  float vibration_z;
            /// <summary>first accelerometer clipping count   </summary>
        public  uint clipping_0;
            /// <summary>second accelerometer clipping count   </summary>
        public  uint clipping_1;
            /// <summary>third accelerometer clipping count   </summary>
        public  uint clipping_2;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=60)]
    ///<summary> This message can be requested by sending the MAV_CMD_GET_HOME_POSITION command. The position the system will return to and land on. The position is set automatically by the system during the takeoff in case it was not explicitely set by the operator before or after. The position the system will return to and land on. The global and local positions encode the position in the respective coordinate frames, while the q parameter encodes the orientation of the surface. Under normal conditions it describes the heading and terrain slope, which can be used by the aircraft to adjust the approach. The approach 3D vector describes the point to which the system should fly in normal flight mode and then perform a landing sequence along the vector. </summary>
    public struct mavlink_home_position_t
    {
        /// <summary>Latitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int latitude;
            /// <summary>Longitude (WGS84, in degrees * 1E7  [degE7] </summary>
        public  int longitude;
            /// <summary>Altitude (AMSL), in meters * 1000 (positive for up)  [mm] </summary>
        public  int altitude;
            /// <summary>Local X position of this position in the local coordinate frame  [m] </summary>
        public  float x;
            /// <summary>Local Y position of this position in the local coordinate frame  [m] </summary>
        public  float y;
            /// <summary>Local Z position of this position in the local coordinate frame  [m] </summary>
        public  float z;
            /// <summary>World to surface normal and heading transformation of the takeoff position. Used to indicate the heading and slope of the ground   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Local X position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        public  float approach_x;
            /// <summary>Local Y position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        public  float approach_y;
            /// <summary>Local Z position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        public  float approach_z;
            /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=61)]
    ///<summary> The position the system will return to and land on. The position is set automatically by the system during the takeoff in case it was not explicitely set by the operator before or after. The global and local positions encode the position in the respective coordinate frames, while the q parameter encodes the orientation of the surface. Under normal conditions it describes the heading and terrain slope, which can be used by the aircraft to adjust the approach. The approach 3D vector describes the point to which the system should fly in normal flight mode and then perform a landing sequence along the vector. </summary>
    public struct mavlink_set_home_position_t
    {
        /// <summary>Latitude (WGS84), in degrees * 1E7  [degE7] </summary>
        public  int latitude;
            /// <summary>Longitude (WGS84, in degrees * 1E7  [degE7] </summary>
        public  int longitude;
            /// <summary>Altitude (AMSL), in meters * 1000 (positive for up)  [mm] </summary>
        public  int altitude;
            /// <summary>Local X position of this position in the local coordinate frame  [m] </summary>
        public  float x;
            /// <summary>Local Y position of this position in the local coordinate frame  [m] </summary>
        public  float y;
            /// <summary>Local Z position of this position in the local coordinate frame  [m] </summary>
        public  float z;
            /// <summary>World to surface normal and heading transformation of the takeoff position. Used to indicate the heading and slope of the ground   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Local X position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        public  float approach_x;
            /// <summary>Local Y position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        public  float approach_y;
            /// <summary>Local Z position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        public  float approach_z;
            /// <summary>System ID.   </summary>
        public  byte target_system;
            /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> This interface replaces DATA_STREAM </summary>
    public struct mavlink_message_interval_t
    {
        /// <summary>The interval between two messages, in microseconds. A value of -1 indicates this stream is disabled, 0 indicates it is not available, > 0 indicates the interval at which it is sent.  [us] </summary>
        public  int interval_us;
            /// <summary>The ID of the requested MAVLink message. v1.0 is limited to 254 messages.   </summary>
        public  ushort message_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Provides state for additional features </summary>
    public struct mavlink_extended_sys_state_t
    {
        /// <summary>The VTOL state if applicable. Is set to MAV_VTOL_STATE_UNDEFINED if UAV is not in VTOL configuration. MAV_VTOL_STATE  </summary>
        public  /*MAV_VTOL_STATE*/byte vtol_state;
            /// <summary>The landed state. Is set to MAV_LANDED_STATE_UNDEFINED if landed state is unknown. MAV_LANDED_STATE  </summary>
        public  /*MAV_LANDED_STATE*/byte landed_state;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=38)]
    ///<summary> The location and information of an ADSB vehicle </summary>
    public struct mavlink_adsb_vehicle_t
    {
        /// <summary>ICAO address   </summary>
        public  uint ICAO_address;
            /// <summary>Latitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude, expressed as degrees * 1E7  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude(ASL) in millimeters  [mm] </summary>
        public  int altitude;
            /// <summary>Course over ground in centidegrees  [cdeg] </summary>
        public  ushort heading;
            /// <summary>The horizontal velocity in centimeters/second  [cm/s] </summary>
        public  ushort hor_velocity;
            /// <summary>The vertical velocity in centimeters/second, positive is up  [cm/s] </summary>
        public  short ver_velocity;
            /// <summary>Flags to indicate various statuses including valid data fields ADSB_FLAGS  bitmask</summary>
        public  /*ADSB_FLAGS*/ushort flags;
            /// <summary>Squawk code   </summary>
        public  ushort squawk;
            /// <summary>Type from ADSB_ALTITUDE_TYPE enum ADSB_ALTITUDE_TYPE  </summary>
        public  /*ADSB_ALTITUDE_TYPE*/byte altitude_type;
            /// <summary>The callsign, 8+null   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)]
		public byte[] callsign;
            /// <summary>Type from ADSB_EMITTER_TYPE enum ADSB_EMITTER_TYPE  </summary>
        public  /*ADSB_EMITTER_TYPE*/byte emitter_type;
            /// <summary>Time since last communication in seconds  [s] </summary>
        public  byte tslc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=19)]
    ///<summary> Information about a potential collision </summary>
    public struct mavlink_collision_t
    {
        /// <summary>Unique identifier, domain based on src field   </summary>
        public  uint id;
            /// <summary>Estimated time until collision occurs (seconds)  [s] </summary>
        public  float time_to_minimum_delta;
            /// <summary>Closest vertical distance in meters between vehicle and object  [m] </summary>
        public  float altitude_minimum_delta;
            /// <summary>Closest horizontal distance in meteres between vehicle and object  [m] </summary>
        public  float horizontal_minimum_delta;
            /// <summary>Collision data source MAV_COLLISION_SRC  </summary>
        public  /*MAV_COLLISION_SRC*/byte src;
            /// <summary>Action that is being taken to avoid this collision MAV_COLLISION_ACTION  </summary>
        public  /*MAV_COLLISION_ACTION*/byte action;
            /// <summary>How concerned the aircraft is about this collision MAV_COLLISION_THREAT_LEVEL  </summary>
        public  /*MAV_COLLISION_THREAT_LEVEL*/byte threat_level;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=254)]
    ///<summary> Message implementing parts of the V2 payload specs in V1 frames for transitional support. </summary>
    public struct mavlink_v2_extension_t
    {
        /// <summary>A code that identifies the software component that understands this message (analogous to usb device classes or mime type strings).  If this code is less than 32768, it is considered a 'registered' protocol extension and the corresponding entry should be added to https://github.com/mavlink/mavlink/extension-message-ids.xml.  Software creators can register blocks of message IDs as needed (useful for GCS specific metadata, etc...). Message_types greater than 32767 are considered local experiments and should not be checked in to any widely distributed codebase.   </summary>
        public  ushort message_type;
            /// <summary>Network ID (0 for broadcast)   </summary>
        public  byte target_network;
            /// <summary>System ID (0 for broadcast)   </summary>
        public  byte target_system;
            /// <summary>Component ID (0 for broadcast)   </summary>
        public  byte target_component;
            /// <summary>Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=249)]
		public byte[] payload;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=36)]
    ///<summary> Send raw controller memory. The use of this message is discouraged for normal packets, but a quite efficient way for testing new messages and getting experimental debug output. </summary>
    public struct mavlink_memory_vect_t
    {
        /// <summary>Starting address of the debug variables   </summary>
        public  ushort address;
            /// <summary>Version code of the type variable. 0=unknown, type ignored and assumed int16_t. 1=as below   </summary>
        public  byte ver;
            /// <summary>Type code of the memory variables. for ver = 1: 0=16 x int16_t, 1=16 x uint16_t, 2=16 x Q15, 3=16 x 1Q14   </summary>
        public  byte type;
            /// <summary>Memory contents at specified address   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] value;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=30)]
    ///<summary>  </summary>
    public struct mavlink_debug_vect_t
    {
        /// <summary>Timestamp  [us] </summary>
        public  ulong time_usec;
            /// <summary>x   </summary>
        public  float x;
            /// <summary>y   </summary>
        public  float y;
            /// <summary>z   </summary>
        public  float z;
            /// <summary>Name   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    ///<summary> Send a key-value pair as float. The use of this message is discouraged for normal packets, but a quite efficient way for testing new messages and getting experimental debug output. </summary>
    public struct mavlink_named_value_float_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Floating point value   </summary>
        public  float value;
            /// <summary>Name of the debug variable   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    ///<summary> Send a key-value pair as integer. The use of this message is discouraged for normal packets, but a quite efficient way for testing new messages and getting experimental debug output. </summary>
    public struct mavlink_named_value_int_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Signed integer value   </summary>
        public  int value;
            /// <summary>Name of the debug variable   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    ///<summary> Status text message. These messages are printed in yellow in the COMM console of QGroundControl. WARNING: They consume quite some bandwidth, so use only for important status and error messages. If implemented wisely, these messages are buffered on the MCU and sent only at a limited rate (e.g. 10 Hz). </summary>
    public struct mavlink_statustext_t
    {
        /// <summary>Severity of status. Relies on the definitions within RFC-5424. See enum MAV_SEVERITY. MAV_SEVERITY  </summary>
        public  /*MAV_SEVERITY*/byte severity;
            /// <summary>Status text message, without null termination character   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=50)]
		public byte[] text;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> Send a debug value. The index is used to discriminate between values. These values show up in the plot of QGroundControl as DEBUG N. </summary>
    public struct mavlink_debug_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>DEBUG value   </summary>
        public  float value;
            /// <summary>index of debug variable   </summary>
        public  byte ind;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> Setup a MAVLink2 signing key. If called with secret_key of all zero and zero initial_timestamp will disable signing </summary>
    public struct mavlink_setup_signing_t
    {
        /// <summary>initial timestamp   </summary>
        public  ulong initial_timestamp;
            /// <summary>system id of the target   </summary>
        public  byte target_system;
            /// <summary>component ID of the target   </summary>
        public  byte target_component;
            /// <summary>signing key   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] secret_key;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> Report button state change </summary>
    public struct mavlink_button_change_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Time of last change of button state  [ms] </summary>
        public  uint last_change_ms;
            /// <summary>Bitmap state of buttons   bitmask</summary>
        public  byte state;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=232)]
    ///<summary> Control vehicle tone generation (buzzer) </summary>
    public struct mavlink_play_tune_t
    {
        /// <summary>System ID   </summary>
        public  byte target_system;
            /// <summary>Component ID   </summary>
        public  byte target_component;
            /// <summary>tune in board specific format   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=30)]
		public byte[] tune;
            /// <summary>tune extension (appended to tune)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=200)]
		public byte[] tune2;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=86)]
    ///<summary> Information about a camera </summary>
    public struct mavlink_camera_information_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Focal length in mm  [mm] </summary>
        public  float focal_length;
            /// <summary>Image sensor size horizontal in mm  [mm] </summary>
        public  float sensor_size_h;
            /// <summary>Image sensor size vertical in mm  [mm] </summary>
        public  float sensor_size_v;
            /// <summary>Image resolution in pixels horizontal  [pix] </summary>
        public  ushort resolution_h;
            /// <summary>Image resolution in pixels vertical  [pix] </summary>
        public  ushort resolution_v;
            /// <summary>Camera ID if there are multiple   </summary>
        public  byte camera_id;
            /// <summary>Name of the camera vendor   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] vendor_name;
            /// <summary>Name of the camera model   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] model_name;
            /// <summary>Reserved for a lense ID   </summary>
        public  byte lense_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> Settings of a camera, can be requested using MAV_CMD_REQUEST_CAMERA_SETTINGS and written using MAV_CMD_SET_CAMERA_SETTINGS </summary>
    public struct mavlink_camera_settings_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Aperture is 1/value   </summary>
        public  float aperture;
            /// <summary>Shutter speed in s  [s] </summary>
        public  float shutter_speed;
            /// <summary>ISO sensitivity   </summary>
        public  float iso_sensitivity;
            /// <summary>Color temperature in degrees Kelvin  [K] </summary>
        public  float white_balance;
            /// <summary>Camera ID if there are multiple   </summary>
        public  byte camera_id;
            /// <summary>Aperture locked (0: auto, 1: locked)   </summary>
        public  byte aperture_locked;
            /// <summary>Shutter speed locked (0: auto, 1: locked)   </summary>
        public  byte shutter_speed_locked;
            /// <summary>ISO sensitivity locked (0: auto, 1: locked)   </summary>
        public  byte iso_sensitivity_locked;
            /// <summary>Color temperature locked (0: auto, 1: locked)   </summary>
        public  byte white_balance_locked;
            /// <summary>Reserved for a camera mode ID   </summary>
        public  byte mode_id;
            /// <summary>Reserved for a color mode ID   </summary>
        public  byte color_mode_id;
            /// <summary>Reserved for image format ID   </summary>
        public  byte image_format_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=27)]
    ///<summary> Information about a storage medium. </summary>
    public struct mavlink_storage_information_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Total capacity  [MiB] </summary>
        public  float total_capacity;
            /// <summary>Used capacity  [MiB] </summary>
        public  float used_capacity;
            /// <summary>Available capacity  [MiB] </summary>
        public  float available_capacity;
            /// <summary>Read speed  [MiB/s] </summary>
        public  float read_speed;
            /// <summary>Write speed  [MiB/s] </summary>
        public  float write_speed;
            /// <summary>Storage ID (1 for first, 2 for second, etc.)   </summary>
        public  byte storage_id;
            /// <summary>Number of storage devices   </summary>
        public  byte storage_count;
            /// <summary>Status of storage (0 not available, 1 unformatted, 2 formatted)   </summary>
        public  byte status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=31)]
    ///<summary> Information about the status of a capture </summary>
    public struct mavlink_camera_capture_status_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Image capture interval in seconds  [s] </summary>
        public  float image_interval;
            /// <summary>Video frame rate in Hz  [Hz] </summary>
        public  float video_framerate;
            /// <summary>Time in milliseconds since recording started  [ms] </summary>
        public  uint recording_time_ms;
            /// <summary>Available storage capacity  [MiB] </summary>
        public  float available_capacity;
            /// <summary>Image resolution in pixels horizontal  [pix] </summary>
        public  ushort image_resolution_h;
            /// <summary>Image resolution in pixels vertical  [pix] </summary>
        public  ushort image_resolution_v;
            /// <summary>Video resolution in pixels horizontal  [pix] </summary>
        public  ushort video_resolution_h;
            /// <summary>Video resolution in pixels vertical  [pix] </summary>
        public  ushort video_resolution_v;
            /// <summary>Camera ID if there are multiple   </summary>
        public  byte camera_id;
            /// <summary>Current status of image capturing (0: not running, 1: interval capture in progress)   </summary>
        public  byte image_status;
            /// <summary>Current status of video capturing (0: not running, 1: capture in progress)   </summary>
        public  byte video_status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    ///<summary> Information about a captured image </summary>
    public struct mavlink_camera_image_captured_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch) in UTC. 0 for unknown.  [us] </summary>
        public  ulong time_utc;
            /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Latitude, expressed as degrees * 1E7 where image was taken  [degE7] </summary>
        public  int lat;
            /// <summary>Longitude, expressed as degrees * 1E7 where capture was taken  [degE7] </summary>
        public  int lon;
            /// <summary>Altitude in meters, expressed as * 1E3 (AMSL, not WGS84) where image was taken  [m] </summary>
        public  int alt;
            /// <summary>Altitude above ground in meters, expressed as * 1E3 where image was taken  [m] </summary>
        public  int relative_alt;
            /// <summary>Quaternion of camera orientation (w, x, y, z order, zero-rotation is 0, 0, 0, 0)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Zero based index of this image (image count since armed -1)   </summary>
        public  int image_index;
            /// <summary>Camera ID (1 for first, 2 for second, etc.)   </summary>
        public  byte camera_id;
            /// <summary>Boolean indicating success (1) or failure (0) while capturing this image.   </summary>
        public  byte capture_result;
            /// <summary>URL of image taken. Either local storage or http://foo.jpg if camera provides an HTTP interface.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=205)]
		public byte[] file_url;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> Information about flight since last arming. </summary>
    public struct mavlink_flight_information_t
    {
        /// <summary>Timestamp at arming (time since UNIX epoch) in UTC, 0 for unknown  [us] </summary>
        public  ulong arming_time_utc;
            /// <summary>Timestamp at takeoff (time since UNIX epoch) in UTC, 0 for unknown  [us] </summary>
        public  ulong takeoff_time_utc;
            /// <summary>Universally unique identifier (UUID) of flight, should correspond to name of log files   </summary>
        public  ulong flight_uuid;
            /// <summary>Timestamp (time since system boot).  [ms] </summary>
        public  uint time_boot_ms;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Orientation of a mount </summary>
    public struct mavlink_mount_orientation_t
    {
        /// <summary>Timestamp (milliseconds since system boot)  [ms] </summary>
        public  uint time_boot_ms;
            /// <summary>Roll in global frame in degrees (set to NaN for invalid).  [deg] </summary>
        public  float roll;
            /// <summary>Pitch in global frame in degrees (set to NaN for invalid).  [deg] </summary>
        public  float pitch;
            /// <summary>Yaw relative to vehicle in degrees (set to NaN for invalid).  [deg] </summary>
        public  float yaw;
            /// <summary>Yaw in absolute frame in degrees, North is 0 (set to NaN for invalid).  [deg] </summary>
        public  float yaw_absolute;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    ///<summary> A message containing logged data (see also MAV_CMD_LOGGING_START) </summary>
    public struct mavlink_logging_data_t
    {
        /// <summary>sequence number (can wrap)   </summary>
        public  ushort sequence;
            /// <summary>system ID of the target   </summary>
        public  byte target_system;
            /// <summary>component ID of the target   </summary>
        public  byte target_component;
            /// <summary>data length  [bytes] </summary>
        public  byte length;
            /// <summary>offset into data where first message starts. This can be used for recovery, when a previous message got lost (set to 255 if no start exists).  [bytes] </summary>
        public  byte first_message_offset;
            /// <summary>logged data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=249)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    ///<summary> A message containing logged data which requires a LOGGING_ACK to be sent back </summary>
    public struct mavlink_logging_data_acked_t
    {
        /// <summary>sequence number (can wrap)   </summary>
        public  ushort sequence;
            /// <summary>system ID of the target   </summary>
        public  byte target_system;
            /// <summary>component ID of the target   </summary>
        public  byte target_component;
            /// <summary>data length  [bytes] </summary>
        public  byte length;
            /// <summary>offset into data where first message starts. This can be used for recovery, when a previous message got lost (set to 255 if no start exists).  [bytes] </summary>
        public  byte first_message_offset;
            /// <summary>logged data   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=249)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> An ack for a LOGGING_DATA_ACKED message </summary>
    public struct mavlink_logging_ack_t
    {
        /// <summary>sequence number (must match the one in LOGGING_DATA_ACKED)   </summary>
        public  ushort sequence;
            /// <summary>system ID of the target   </summary>
        public  byte target_system;
            /// <summary>component ID of the target   </summary>
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=96)]
    ///<summary> Configure AP SSID and Password. </summary>
    public struct mavlink_wifi_config_ap_t
    {
        /// <summary>Name of Wi-Fi network (SSID). Leave it blank to leave it unchanged.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] ssid;
            /// <summary>Password. Leave it blank for an open AP.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=64)]
		public byte[] password;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=17)]
    ///<summary> General status information of an UAVCAN node. Please refer to the definition of the UAVCAN message "uavcan.protocol.NodeStatus" for the background information. The UAVCAN specification is available at http://uavcan.org. </summary>
    public struct mavlink_uavcan_node_status_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>The number of seconds since the start-up of the node.  [s] </summary>
        public  uint uptime_sec;
            /// <summary>Vendor-specific status information.   </summary>
        public  ushort vendor_specific_status_code;
            /// <summary>Generalized node health status. UAVCAN_NODE_HEALTH  </summary>
        public  /*UAVCAN_NODE_HEALTH*/byte health;
            /// <summary>Generalized operating mode. UAVCAN_NODE_MODE  </summary>
        public  /*UAVCAN_NODE_MODE*/byte mode;
            /// <summary>Not used currently.   </summary>
        public  byte sub_mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=116)]
    ///<summary> General information describing a particular UAVCAN node. Please refer to the definition of the UAVCAN service "uavcan.protocol.GetNodeInfo" for the background information. This message should be emitted by the system whenever a new node appears online, or an existing node reboots. Additionally, it can be emitted upon request from the other end of the MAVLink channel (see MAV_CMD_UAVCAN_GET_NODE_INFO). It is also not prohibited to emit this message unconditionally at a low frequency. The UAVCAN specification is available at http://uavcan.org. </summary>
    public struct mavlink_uavcan_node_info_t
    {
        /// <summary>Timestamp (microseconds since UNIX epoch or microseconds since system boot)  [us] </summary>
        public  ulong time_usec;
            /// <summary>The number of seconds since the start-up of the node.  [s] </summary>
        public  uint uptime_sec;
            /// <summary>Version control system (VCS) revision identifier (e.g. git short commit hash). Zero if unknown.   </summary>
        public  uint sw_vcs_commit;
            /// <summary>Node name string. For example, "sapog.px4.io".   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=80)]
		public byte[] name;
            /// <summary>Hardware major version number.   </summary>
        public  byte hw_version_major;
            /// <summary>Hardware minor version number.   </summary>
        public  byte hw_version_minor;
            /// <summary>Hardware unique 128-bit ID.   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] hw_unique_id;
            /// <summary>Software major version number.   </summary>
        public  byte sw_version_major;
            /// <summary>Software minor version number.   </summary>
        public  byte sw_version_minor;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=158)]
    ///<summary> Obstacle distances in front of the sensor, starting from the left in increment degrees to the right </summary>
    public struct mavlink_obstacle_distance_t
    {
        /// <summary>Timestamp (microseconds since system boot or since UNIX epoch).  [us] </summary>
        public  ulong time_usec;
            /// <summary>Distance of obstacles around the UAV with index 0 corresponding to local North. A value of 0 means that the obstacle is right in front of the sensor. A value of max_distance +1 means no obstacle is present. A value of UINT16_MAX for unknown/not used. In a array element, one unit corresponds to 1cm.  [cm] </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=72)]
		public UInt16[] distances;
            /// <summary>Minimum distance the sensor can measure in centimeters.  [cm] </summary>
        public  ushort min_distance;
            /// <summary>Maximum distance the sensor can measure in centimeters.  [cm] </summary>
        public  ushort max_distance;
            /// <summary>Class id of the distance sensor type. MAV_DISTANCE_SENSOR  </summary>
        public  /*MAV_DISTANCE_SENSOR*/byte sensor_type;
            /// <summary>Angular width in degrees of each array element.  [deg] </summary>
        public  byte increment;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=230)]
    ///<summary> Odometry message to communicate odometry information with an external interface. Fits ROS REP 147 standard for aerial vehicles (http://www.ros.org/reps/rep-0147.html). </summary>
    public struct mavlink_odometry_t
    {
        /// <summary>Timestamp (microseconds since system boot or since UNIX epoch).  [us] </summary>
        public  ulong time_usec;
            /// <summary>X Position  [m] </summary>
        public  float x;
            /// <summary>Y Position  [m] </summary>
        public  float y;
            /// <summary>Z Position  [m] </summary>
        public  float z;
            /// <summary>Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>X linear speed  [m/s] </summary>
        public  float vx;
            /// <summary>Y linear speed  [m/s] </summary>
        public  float vy;
            /// <summary>Z linear speed  [m/s] </summary>
        public  float vz;
            /// <summary>Roll angular speed  [rad/s] </summary>
        public  float rollspeed;
            /// <summary>Pitch angular speed  [rad/s] </summary>
        public  float pitchspeed;
            /// <summary>Yaw angular speed  [rad/s] </summary>
        public  float yawspeed;
            /// <summary>Pose (states: x, y, z, roll, pitch, yaw) covariance matrix upper right triangle (first six entries are the first ROW, next five entries are the second ROW, etc.)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] pose_covariance;
            /// <summary>Twist (states: vx, vy, vz, rollspeed, pitchspeed, yawspeed) covariance matrix upper right triangle (first six entries are the first ROW, next five entries are the second ROW, etc.)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] twist_covariance;
            /// <summary>Coordinate frame of reference for the pose data, as defined by MAV_FRAME enum. MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte frame_id;
            /// <summary>Coordinate frame of reference for the velocity in free space (twist) data, as defined by MAV_FRAME enum. MAV_FRAME  </summary>
        public  /*MAV_FRAME*/byte child_frame_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Static data to configure the ADS-B transponder (send within 10 sec of a POR and every 10 sec thereafter) </summary>
    public struct mavlink_uavionix_adsb_out_cfg_t
    {
        /// <summary>Vehicle address (24 bit)   </summary>
        public  uint ICAO;
            /// <summary>Aircraft stall speed in cm/s  [cm/s] </summary>
        public  ushort stallSpeed;
            /// <summary>Vehicle identifier (8 characters, null terminated, valid characters are A-Z, 0-9, " " only)   </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)]
		public byte[] callsign;
            /// <summary>Transmitting vehicle type. See ADSB_EMITTER_TYPE enum ADSB_EMITTER_TYPE  </summary>
        public  /*ADSB_EMITTER_TYPE*/byte emitterType;
            /// <summary>Aircraft length and width encoding (table 2-35 of DO-282B) UAVIONIX_ADSB_OUT_CFG_AIRCRAFT_SIZE  </summary>
        public  /*UAVIONIX_ADSB_OUT_CFG_AIRCRAFT_SIZE*/byte aircraftSize;
            /// <summary>GPS antenna lateral offset (table 2-36 of DO-282B) UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LAT  </summary>
        public  /*UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LAT*/byte gpsOffsetLat;
            /// <summary>GPS antenna longitudinal offset from nose [if non-zero, take position (in meters) divide by 2 and add one] (table 2-37 DO-282B) UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LON  </summary>
        public  /*UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LON*/byte gpsOffsetLon;
            /// <summary>ADS-B transponder reciever and transmit enable flags UAVIONIX_ADSB_OUT_RF_SELECT  bitmask</summary>
        public  /*UAVIONIX_ADSB_OUT_RF_SELECT*/byte rfSelect;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=41)]
    ///<summary> Dynamic data used to generate ADS-B out transponder data (send at 5Hz) </summary>
    public struct mavlink_uavionix_adsb_out_dynamic_t
    {
        /// <summary>UTC time in seconds since GPS epoch (Jan 6, 1980). If unknown set to UINT32_MAX  [s] </summary>
        public  uint utcTime;
            /// <summary>Latitude WGS84 (deg * 1E7). If unknown set to INT32_MAX  [degE7] </summary>
        public  int gpsLat;
            /// <summary>Longitude WGS84 (deg * 1E7). If unknown set to INT32_MAX  [degE7] </summary>
        public  int gpsLon;
            /// <summary>Altitude in mm (m * 1E-3) UP +ve. WGS84 altitude. If unknown set to INT32_MAX  [mm] </summary>
        public  int gpsAlt;
            /// <summary>Barometric pressure altitude relative to a standard atmosphere of 1013.2 mBar and NOT bar corrected altitude (m * 1E-3). (up +ve). If unknown set to INT32_MAX  [mbar] </summary>
        public  int baroAltMSL;
            /// <summary>Horizontal accuracy in mm (m * 1E-3). If unknown set to UINT32_MAX  [mm] </summary>
        public  uint accuracyHor;
            /// <summary>Vertical accuracy in cm. If unknown set to UINT16_MAX  [cm] </summary>
        public  ushort accuracyVert;
            /// <summary>Velocity accuracy in mm/s (m * 1E-3). If unknown set to UINT16_MAX  [mm/s] </summary>
        public  ushort accuracyVel;
            /// <summary>GPS vertical speed in cm/s. If unknown set to INT16_MAX  [cm/s] </summary>
        public  short velVert;
            /// <summary>North-South velocity over ground in cm/s North +ve. If unknown set to INT16_MAX  [cm/s] </summary>
        public  short velNS;
            /// <summary>East-West velocity over ground in cm/s East +ve. If unknown set to INT16_MAX  [cm/s] </summary>
        public  short VelEW;
            /// <summary>ADS-B transponder dynamic input state flags UAVIONIX_ADSB_OUT_DYNAMIC_STATE  bitmask</summary>
        public  /*UAVIONIX_ADSB_OUT_DYNAMIC_STATE*/ushort state;
            /// <summary>Mode A code (typically 1200 [0x04B0] for VFR)   </summary>
        public  ushort squawk;
            /// <summary>0-1: no fix, 2: 2D fix, 3: 3D fix, 4: DGPS, 5: RTK UAVIONIX_ADSB_OUT_DYNAMIC_GPS_FIX  </summary>
        public  /*UAVIONIX_ADSB_OUT_DYNAMIC_GPS_FIX*/byte gpsFix;
            /// <summary>Number of satellites visible. If unknown set to UINT8_MAX   </summary>
        public  byte numSats;
            /// <summary>Emergency status UAVIONIX_ADSB_EMERGENCY_STATUS  </summary>
        public  /*UAVIONIX_ADSB_EMERGENCY_STATUS*/byte emergencyStatus;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=1)]
    ///<summary> Transceiver heartbeat with health report (updated every 10s) </summary>
    public struct mavlink_uavionix_adsb_transceiver_health_report_t
    {
        /// <summary>ADS-B transponder messages UAVIONIX_ADSB_RF_HEALTH  bitmask</summary>
        public  /*UAVIONIX_ADSB_RF_HEALTH*/byte rfHealth;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=1)]
    ///<summary> ICAROUS heartbeat </summary>
    public struct mavlink_icarous_heartbeat_t
    {
        /// <summary>See the FMS_STATE enum. ICAROUS_FMS_STATE  </summary>
        public  /*ICAROUS_FMS_STATE*/byte status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=46)]
    ///<summary> Kinematic multi bands (track) output from Daidalus </summary>
    public struct mavlink_icarous_kinematic_bands_t
    {
        /// <summary>min angle (degrees)  [deg] </summary>
        public  float min1;
            /// <summary>max angle (degrees)  [deg] </summary>
        public  float max1;
            /// <summary>min angle (degrees)  [deg] </summary>
        public  float min2;
            /// <summary>max angle (degrees)  [deg] </summary>
        public  float max2;
            /// <summary>min angle (degrees)  [deg] </summary>
        public  float min3;
            /// <summary>max angle (degrees)  [deg] </summary>
        public  float max3;
            /// <summary>min angle (degrees)  [deg] </summary>
        public  float min4;
            /// <summary>max angle (degrees)  [deg] </summary>
        public  float max4;
            /// <summary>min angle (degrees)  [deg] </summary>
        public  float min5;
            /// <summary>max angle (degrees)  [deg] </summary>
        public  float max5;
            /// <summary>Number of track bands   </summary>
        public  byte numBands;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type1;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type2;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type3;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type4;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type5;
    
    };

}
