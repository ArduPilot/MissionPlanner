using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public partial class MAVLink
{
    public const string MAVLINK_BUILD_DATE = "Tue Mar 19 2019";
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
		new message_info(132, "DISTANCE_SENSOR", 85, 14, 38, typeof( mavlink_distance_sensor_t )),
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
		new message_info(259, "CAMERA_INFORMATION", 92, 235, 235, typeof( mavlink_camera_information_t )),
		new message_info(260, "CAMERA_SETTINGS", 146, 5, 13, typeof( mavlink_camera_settings_t )),
		new message_info(261, "STORAGE_INFORMATION", 179, 27, 27, typeof( mavlink_storage_information_t )),
		new message_info(262, "CAMERA_CAPTURE_STATUS", 12, 18, 18, typeof( mavlink_camera_capture_status_t )),
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
		new message_info(331, "ODOMETRY", 91, 230, 230, typeof( mavlink_odometry_t )),
		new message_info(350, "DEBUG_FLOAT_ARRAY", 232, 20, 252, typeof( mavlink_debug_float_array_t )),
		new message_info(365, "STATUSTEXT_LONG", 36, 255, 255, typeof( mavlink_statustext_long_t )),
		new message_info(9000, "WHEEL_DISTANCE", 113, 137, 137, typeof( mavlink_wheel_distance_t )),
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
DEBUG_FLOAT_ARRAY = 350,
STATUSTEXT_LONG = 365,
WHEEL_DISTANCE = 9000,
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
        [Description("")]
        LEVEL=1, 
    	///<summary>  | </summary>
        [Description("")]
        LEFT=2, 
    	///<summary>  | </summary>
        [Description("")]
        RIGHT=3, 
    	///<summary>  | </summary>
        [Description("")]
        NOSEDOWN=4, 
    	///<summary>  | </summary>
        [Description("")]
        NOSEUP=5, 
    	///<summary>  | </summary>
        [Description("")]
        BACK=6, 
    	///<summary>  | </summary>
        [Description("")]
        SUCCESS=16777215, 
    	///<summary>  | </summary>
        [Description("")]
        FAILED=16777216, 
    
    };
    
    ///<summary> Commands to be executed by the MAV. They can be executed on user request, or as part of a mission script. If the action is used in a mission, the parameter mapping to the waypoint/mission message is as follows: Param 1, Param 2, Param 3, Param 4, X: Param 5, Y:Param 6, Z:Param 7. This command list is similar what ARINC 424 is for commercial aircraft: A data format how to interpret waypoint/mission data. </summary>
    public enum MAV_CMD: ushort
    {
			///<summary> Navigate to waypoint. |Hold time in decimal seconds. (ignored by fixed wing, time to stay at waypoint for rotary wing)| Acceptance radius in meters (if the sphere with this radius is hit, the waypoint counts as reached)| 0 to pass through the WP, if > 0 radius in meters to pass by WP. Positive value for clockwise orbit, negative value for counter-clockwise orbit. Allows trajectory control.| Desired yaw angle at waypoint (rotary wing). NaN for unchanged.| Latitude| Longitude| Altitude|  </summary>
        [Description("Navigate to waypoint.")]
        WAYPOINT=16, 
    	///<summary> Loiter around this waypoint an unlimited amount of time |Empty| Empty| Radius around waypoint, in meters. If positive loiter clockwise, else counter-clockwise| Desired yaw angle.| Latitude| Longitude| Altitude|  </summary>
        [Description("Loiter around this waypoint an unlimited amount of time")]
        LOITER_UNLIM=17, 
    	///<summary> Loiter around this waypoint for X turns |Turns| Empty| Radius around waypoint, in meters. If positive loiter clockwise, else counter-clockwise| Forward moving aircraft this sets exit xtrack location: 0 for center of loiter wp, 1 for exit location. Else, this is desired yaw angle| Latitude| Longitude| Altitude|  </summary>
        [Description("Loiter around this waypoint for X turns")]
        LOITER_TURNS=18, 
    	///<summary> Loiter around this waypoint for X seconds |Seconds (decimal)| Empty| Radius around waypoint, in meters. If positive loiter clockwise, else counter-clockwise| Forward moving aircraft this sets exit xtrack location: 0 for center of loiter wp, 1 for exit location. Else, this is desired yaw angle| Latitude| Longitude| Altitude|  </summary>
        [Description("Loiter around this waypoint for X seconds")]
        LOITER_TIME=19, 
    	///<summary> Return to launch location |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Return to launch location")]
        RETURN_TO_LAUNCH=20, 
    	///<summary> Land at location. |Minimum target altitude if landing is aborted (0 = undefined/use system default).| Precision land mode.| Empty.| Desired yaw angle. NaN for unchanged.| Latitude.| Longitude.| Landing altitude (ground level in current frame).|  </summary>
        [Description("Land at location.")]
        LAND=21, 
    	///<summary> Takeoff from ground / hand |Minimum pitch (if airspeed sensor present), desired pitch without sensor| Empty| Empty| Yaw angle (if magnetometer present), ignored without magnetometer. NaN for unchanged.| Latitude| Longitude| Altitude|  </summary>
        [Description("Takeoff from ground / hand")]
        TAKEOFF=22, 
    	///<summary> Land at local position (local frame only) |Landing target number (if available)| Maximum accepted offset from desired landing position [m] - computed magnitude from spherical coordinates: d = sqrt(x^2 + y^2 + z^2), which gives the maximum accepted distance between the desired landing position and the position where the vehicle is about to land| Landing descend rate [ms^-1]| Desired yaw angle [rad]| Y-axis position [m]| X-axis position [m]| Z-axis / ground level position [m]|  </summary>
        [Description("Land at local position (local frame only)")]
        LAND_LOCAL=23, 
    	///<summary> Takeoff from local position (local frame only) |Minimum pitch (if airspeed sensor present), desired pitch without sensor [rad]| Empty| Takeoff ascend rate [ms^-1]| Yaw angle [rad] (if magnetometer or another yaw estimation source present), ignored without one of these| Y-axis position [m]| X-axis position [m]| Z-axis position [m]|  </summary>
        [Description("Takeoff from local position (local frame only)")]
        TAKEOFF_LOCAL=24, 
    	///<summary> Vehicle following, i.e. this waypoint represents the position of a moving vehicle |Following logic to use (e.g. loitering or sinusoidal following) - depends on specific autopilot implementation| Ground speed of vehicle to be followed| Radius around waypoint, in meters. If positive loiter clockwise, else counter-clockwise| Desired yaw angle.| Latitude| Longitude| Altitude|  </summary>
        [Description("Vehicle following, i.e. this waypoint represents the position of a moving vehicle")]
        FOLLOW=25, 
    	///<summary> Continue on the current course and climb/descend to specified altitude.  When the altitude is reached continue to the next command (i.e., don't proceed to the next command until the desired altitude is reached. |Climb or Descend (0 = Neutral, command completes when within 5m of this command's altitude, 1 = Climbing, command completes when at or above this command's altitude, 2 = Descending, command completes when at or below this command's altitude.| Empty| Empty| Empty| Empty| Empty| Desired altitude in meters|  </summary>
        [Description("Continue on the current course and climb/descend to specified altitude.  When the altitude is reached continue to the next command (i.e., don't proceed to the next command until the desired altitude is reached.")]
        CONTINUE_AND_CHANGE_ALT=30, 
    	///<summary> Begin loiter at the specified Latitude and Longitude.  If Lat=Lon=0, then loiter at the current position.  Don't consider the navigation command complete (don't leave loiter) until the altitude has been reached.  Additionally, if the Heading Required parameter is non-zero the  aircraft will not leave the loiter until heading toward the next waypoint. |Heading Required (0 = False)| Radius in meters. If positive loiter clockwise, negative counter-clockwise, 0 means no change to standard loiter.| Empty| Forward moving aircraft this sets exit xtrack location: 0 for center of loiter wp, 1 for exit location| Latitude| Longitude| Altitude|  </summary>
        [Description("Begin loiter at the specified Latitude and Longitude.  If Lat=Lon=0, then loiter at the current position.  Don't consider the navigation command complete (don't leave loiter) until the altitude has been reached.  Additionally, if the Heading Required parameter is non-zero the  aircraft will not leave the loiter until heading toward the next waypoint.")]
        LOITER_TO_ALT=31, 
    	///<summary> Being following a target |System ID (the system ID of the FOLLOW_TARGET beacon). Send 0 to disable follow-me and return to the default position hold mode| RESERVED| RESERVED| altitude flag: 0: Keep current altitude, 1: keep altitude difference to target, 2: go to a fixed altitude above home| altitude| RESERVED| TTL in seconds in which the MAV should go to the default position hold mode after a message rx timeout|  </summary>
        [Description("Being following a target")]
        DO_FOLLOW=32, 
    	///<summary> Reposition the MAV after a follow target command has been sent |Camera q1 (where 0 is on the ray from the camera to the tracking device)| Camera q2| Camera q3| Camera q4| altitude offset from target (m)| X offset from target (m)| Y offset from target (m)|  </summary>
        [Description("Reposition the MAV after a follow target command has been sent")]
        DO_FOLLOW_REPOSITION=33, 
    	///<summary> THIS INTERFACE IS DEPRECATED AS OF JANUARY 2018. Please use MAV_CMD_DO_SET_ROI_* messages instead. Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Region of interest mode. (see MAV_ROI enum)| Waypoint index/ target ID. (see MAV_ROI enum)| ROI index (allows a vehicle to manage multiple ROI's)| Empty| x the location of the fixed ROI (see MAV_FRAME)| y| z|  </summary>
        [Description("THIS INTERFACE IS DEPRECATED AS OF JANUARY 2018. Please use MAV_CMD_DO_SET_ROI_* messages instead. Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras.")]
        ROI=80, 
    	///<summary> Control autonomous path planning on the MAV. |0: Disable local obstacle avoidance / local path planning (without resetting map), 1: Enable local path planning, 2: Enable and reset local path planning| 0: Disable full path planning (without resetting map), 1: Enable, 2: Enable and reset map/occupancy grid, 3: Enable and reset planned route, but not occupancy grid| Empty| Yaw angle at goal, in compass degrees, [0..360]| Latitude/X of goal| Longitude/Y of goal| Altitude/Z of goal|  </summary>
        [Description("Control autonomous path planning on the MAV.")]
        PATHPLANNING=81, 
    	///<summary> Navigate to waypoint using a spline path. |Hold time in decimal seconds. (ignored by fixed wing, time to stay at waypoint for rotary wing)| Empty| Empty| Empty| Latitude/X of goal| Longitude/Y of goal| Altitude/Z of goal|  </summary>
        [Description("Navigate to waypoint using a spline path.")]
        SPLINE_WAYPOINT=82, 
    	///<summary> Mission command to wait for an altitude or downwards vertical speed. This is meant for high altitude balloon launches, allowing the aircraft to be idle until either an altitude is reached or a negative vertical speed is reached (indicating early balloon burst). The wiggle time is how often to wiggle the control surfaces to prevent them seizing up. |Altitude (m).| Descent speed (m/s).| Wiggle Time (s).| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Mission command to wait for an altitude or downwards vertical speed. This is meant for high altitude balloon launches, allowing the aircraft to be idle until either an altitude is reached or a negative vertical speed is reached (indicating early balloon burst). The wiggle time is how often to wiggle the control surfaces to prevent them seizing up.")]
        ALTITUDE_WAIT=83, 
    	///<summary> Takeoff from ground using VTOL mode, and transition to forward flight with specified heading. |Empty| Front transition heading.| Empty| Yaw angle in degrees. NaN for unchanged.| Latitude| Longitude| Altitude|  </summary>
        [Description("Takeoff from ground using VTOL mode, and transition to forward flight with specified heading.")]
        VTOL_TAKEOFF=84, 
    	///<summary> Land using VTOL mode |Empty| Empty| Approach altitude (with the same reference as the Altitude field). NaN if unspecified.| Yaw angle in degrees. NaN for unchanged.| Latitude| Longitude| Altitude (ground level)|  </summary>
        [Description("Land using VTOL mode")]
        VTOL_LAND=85, 
    	///<summary> hand control over to an external controller |On / Off (> 0.5f on)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("hand control over to an external controller")]
        GUIDED_ENABLE=92, 
    	///<summary> Delay the next navigation command a number of seconds or until a specified time |Delay in seconds (decimal, -1 to enable time-of-day fields)| hour (24h format, UTC, -1 to ignore)| minute (24h format, UTC, -1 to ignore)| second (24h format, UTC)| Empty| Empty| Empty|  </summary>
        [Description("Delay the next navigation command a number of seconds or until a specified time")]
        DELAY=93, 
    	///<summary> Descend and place payload. Vehicle moves to specified location, descends until it detects a hanging payload has reached the ground, and then releases the payload. If ground is not detected before the reaching the maximum descent value (param1), the command will complete without releasing the payload. |Maximum distance to descend.| Empty| Empty| Empty| Latitude| Longitude| Altitude|  </summary>
        [Description("Descend and place payload. Vehicle moves to specified location, descends until it detects a hanging payload has reached the ground, and then releases the payload. If ground is not detected before the reaching the maximum descent value (param1), the command will complete without releasing the payload.")]
        PAYLOAD_PLACE=94, 
    	///<summary> NOP - This command is only used to mark the upper limit of the NAV/ACTION commands in the enumeration |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("NOP - This command is only used to mark the upper limit of the NAV/ACTION commands in the enumeration")]
        LAST=95, 
    	///<summary> Delay mission state machine. |Delay in seconds (decimal)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Delay mission state machine.")]
        CONDITION_DELAY=112, 
    	///<summary> Ascend/descend at rate.  Delay mission state machine until desired altitude reached. |Descent / Ascend rate (m/s)| Empty| Empty| Empty| Empty| Empty| Finish Altitude|  </summary>
        [Description("Ascend/descend at rate.  Delay mission state machine until desired altitude reached.")]
        CONDITION_CHANGE_ALT=113, 
    	///<summary> Delay mission state machine until within desired distance of next NAV point. |Distance (meters)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Delay mission state machine until within desired distance of next NAV point.")]
        CONDITION_DISTANCE=114, 
    	///<summary> Reach a certain target angle. |target angle: [0-360], 0 is north| speed during yaw change:[deg per second]| direction: negative: counter clockwise, positive: clockwise [-1,1]| relative offset or absolute angle: [ 1,0]| Empty| Empty| Empty|  </summary>
        [Description("Reach a certain target angle.")]
        CONDITION_YAW=115, 
    	///<summary> NOP - This command is only used to mark the upper limit of the CONDITION commands in the enumeration |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("NOP - This command is only used to mark the upper limit of the CONDITION commands in the enumeration")]
        CONDITION_LAST=159, 
    	///<summary> Set system mode. |Mode, as defined by ENUM MAV_MODE| Custom mode - this is system specific, please refer to the individual autopilot specifications for details.| Custom sub mode - this is system specific, please refer to the individual autopilot specifications for details.| Empty| Empty| Empty| Empty|  </summary>
        [Description("Set system mode.")]
        DO_SET_MODE=176, 
    	///<summary> Jump to the desired command in the mission list.  Repeat this action only the specified number of times |Sequence number| Repeat count| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Jump to the desired command in the mission list.  Repeat this action only the specified number of times")]
        DO_JUMP=177, 
    	///<summary> Change speed and/or throttle set points. |Speed type (0=Airspeed, 1=Ground Speed, 2=Climb Speed, 3=Descent Speed)| Speed  (m/s, -1 indicates no change)| Throttle  ( Percent, -1 indicates no change)| absolute or relative [0,1]| Empty| Empty| Empty|  </summary>
        [Description("Change speed and/or throttle set points.")]
        DO_CHANGE_SPEED=178, 
    	///<summary> Changes the home location either to the current location or a specified location. |Use current (1=use current location, 0=use specified location)| Empty| Empty| Empty| Latitude| Longitude| Altitude|  </summary>
        [Description("Changes the home location either to the current location or a specified location.")]
        DO_SET_HOME=179, 
    	///<summary> Set a system parameter.  Caution!  Use of this command requires knowledge of the numeric enumeration value of the parameter. |Parameter number| Parameter value| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Set a system parameter.  Caution!  Use of this command requires knowledge of the numeric enumeration value of the parameter.")]
        DO_SET_PARAMETER=180, 
    	///<summary> Set a relay to a condition. |Relay number| Setting (1=on, 0=off, others possible depending on system hardware)| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Set a relay to a condition.")]
        DO_SET_RELAY=181, 
    	///<summary> Cycle a relay on and off for a desired number of cycles with a desired period. |Relay number| Cycle count| Cycle time (seconds, decimal)| Empty| Empty| Empty| Empty|  </summary>
        [Description("Cycle a relay on and off for a desired number of cycles with a desired period.")]
        DO_REPEAT_RELAY=182, 
    	///<summary> Set a servo to a desired PWM value. |Servo number| PWM (microseconds, 1000 to 2000 typical)| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Set a servo to a desired PWM value.")]
        DO_SET_SERVO=183, 
    	///<summary> Cycle a between its nominal setting and a desired PWM for a desired number of cycles with a desired period. |Servo number| PWM (microseconds, 1000 to 2000 typical)| Cycle count| Cycle time (seconds)| Empty| Empty| Empty|  </summary>
        [Description("Cycle a between its nominal setting and a desired PWM for a desired number of cycles with a desired period.")]
        DO_REPEAT_SERVO=184, 
    	///<summary> Terminate flight immediately |Flight termination activated if > 0.5| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Terminate flight immediately")]
        DO_FLIGHTTERMINATION=185, 
    	///<summary> Change altitude set point. |Altitude in meters| Mav frame of new altitude (see MAV_FRAME)| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Change altitude set point.")]
        DO_CHANGE_ALTITUDE=186, 
    	///<summary> Mission command to perform a landing. This is used as a marker in a mission to tell the autopilot where a sequence of mission items that represents a landing starts. It may also be sent via a COMMAND_LONG to trigger a landing, in which case the nearest (geographically) landing sequence in the mission will be used. The Latitude/Longitude is optional, and may be set to 0 if not needed. If specified then it will be used to help find the closest landing sequence. |Empty| Empty| Empty| Empty| Latitude| Longitude| Empty|  </summary>
        [Description("Mission command to perform a landing. This is used as a marker in a mission to tell the autopilot where a sequence of mission items that represents a landing starts. It may also be sent via a COMMAND_LONG to trigger a landing, in which case the nearest (geographically) landing sequence in the mission will be used. The Latitude/Longitude is optional, and may be set to 0 if not needed. If specified then it will be used to help find the closest landing sequence.")]
        DO_LAND_START=189, 
    	///<summary> Mission command to perform a landing from a rally point. |Break altitude (meters)| Landing speed (m/s)| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Mission command to perform a landing from a rally point.")]
        DO_RALLY_LAND=190, 
    	///<summary> Mission command to safely abort an autonomous landing. |Altitude (meters)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Mission command to safely abort an autonomous landing.")]
        DO_GO_AROUND=191, 
    	///<summary> Reposition the vehicle to a specific WGS84 global position. |Ground speed, less than 0 (-1) for default| Bitmask of option flags, see the MAV_DO_REPOSITION_FLAGS enum.| Reserved| Yaw heading, NaN for unchanged. For planes indicates loiter direction (0: clockwise, 1: counter clockwise)| Latitude (deg * 1E7)| Longitude (deg * 1E7)| Altitude (meters)|  </summary>
        [Description("Reposition the vehicle to a specific WGS84 global position.")]
        DO_REPOSITION=192, 
    	///<summary> If in a GPS controlled position mode, hold the current position or continue. |0: Pause current mission or reposition command, hold current position. 1: Continue mission. A VTOL capable vehicle should enter hover mode (multicopter and VTOL planes). A plane should loiter with the default loiter radius.| Reserved| Reserved| Reserved| Reserved| Reserved| Reserved|  </summary>
        [Description("If in a GPS controlled position mode, hold the current position or continue.")]
        DO_PAUSE_CONTINUE=193, 
    	///<summary> Set moving direction to forward or reverse. |Direction (0=Forward, 1=Reverse)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Set moving direction to forward or reverse.")]
        DO_SET_REVERSE=194, 
    	///<summary> Sets the region of interest (ROI) to a location. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Empty| Empty| Empty| Empty| Latitude| Longitude| Altitude|  </summary>
        [Description("Sets the region of interest (ROI) to a location. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras.")]
        DO_SET_ROI_LOCATION=195, 
    	///<summary> Sets the region of interest (ROI) to be toward next waypoint, with optional pitch/roll/yaw offset. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Empty| Empty| Empty| Empty| pitch offset from next waypoint| roll offset from next waypoint| yaw offset from next waypoint|  </summary>
        [Description("Sets the region of interest (ROI) to be toward next waypoint, with optional pitch/roll/yaw offset. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras.")]
        DO_SET_ROI_WPNEXT_OFFSET=196, 
    	///<summary> Cancels any previous ROI command returning the vehicle/sensors to default flight characteristics. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Cancels any previous ROI command returning the vehicle/sensors to default flight characteristics. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras.")]
        DO_SET_ROI_NONE=197, 
    	///<summary> Control onboard camera system. |Camera ID (-1 for all)| Transmission: 0: disabled, 1: enabled compressed, 2: enabled raw| Transmission mode: 0: video stream, >0: single images every n seconds (decimal)| Recording: 0: disabled, 1: enabled compressed, 2: enabled raw| Empty| Empty| Empty|  </summary>
        [Description("Control onboard camera system.")]
        DO_CONTROL_VIDEO=200, 
    	///<summary> THIS INTERFACE IS DEPRECATED AS OF JANUARY 2018. Please use MAV_CMD_DO_SET_ROI_* messages instead. Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras. |Region of interest mode. (see MAV_ROI enum)| Waypoint index/ target ID. (see MAV_ROI enum)| ROI index (allows a vehicle to manage multiple ROI's)| Empty| x the location of the fixed ROI (see MAV_FRAME)| y| z|  </summary>
        [Description("THIS INTERFACE IS DEPRECATED AS OF JANUARY 2018. Please use MAV_CMD_DO_SET_ROI_* messages instead. Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicles control system to control the vehicle attitude and the attitude of various sensors such as cameras.")]
        DO_SET_ROI=201, 
    	///<summary> Mission command to configure an on-board camera controller system. |Modes: P, TV, AV, M, Etc| Shutter speed: Divisor number for one second| Aperture: F stop number| ISO number e.g. 80, 100, 200, Etc| Exposure type enumerator| Command Identity| Main engine cut-off time before camera trigger in seconds/10 (0 means no cut-off)|  </summary>
        [Description("Mission command to configure an on-board camera controller system.")]
        DO_DIGICAM_CONFIGURE=202, 
    	///<summary> Mission command to control an on-board camera controller system. |Session control e.g. show/hide lens| Zoom's absolute position| Zooming step value to offset zoom from the current position| Focus Locking, Unlocking or Re-locking| Shooting Command| Command Identity| Test shot identifier. If set to 1, image will only be captured, but not counted towards internal frame count.|  </summary>
        [Description("Mission command to control an on-board camera controller system.")]
        DO_DIGICAM_CONTROL=203, 
    	///<summary> Mission command to configure a camera or antenna mount |Mount operation mode (see MAV_MOUNT_MODE enum)| stabilize roll? (1 = yes, 0 = no)| stabilize pitch? (1 = yes, 0 = no)| stabilize yaw? (1 = yes, 0 = no)| Empty| Empty| Empty|  </summary>
        [Description("Mission command to configure a camera or antenna mount")]
        DO_MOUNT_CONFIGURE=204, 
    	///<summary> Mission command to control a camera or antenna mount |pitch (WIP: DEPRECATED: or lat in degrees) depending on mount mode.| roll (WIP: DEPRECATED: or lon in degrees) depending on mount mode.| yaw (WIP: DEPRECATED: or alt in meters) depending on mount mode.| WIP: alt in meters depending on mount mode.| WIP: latitude in degrees * 1E7, set if appropriate mount mode.| WIP: longitude in degrees * 1E7, set if appropriate mount mode.| MAV_MOUNT_MODE enum value|  </summary>
        [Description("Mission command to control a camera or antenna mount")]
        DO_MOUNT_CONTROL=205, 
    	///<summary> Mission command to set camera trigger distance for this flight. The camera is triggered each time this distance is exceeded. This command can also be used to set the shutter integration time for the camera. |Camera trigger distance (meters). 0 to stop triggering.| Camera shutter integration time (milliseconds). -1 or 0 to ignore| Trigger camera once immediately. (0 = no trigger, 1 = trigger)| Empty| Empty| Empty| Empty|  </summary>
        [Description("Mission command to set camera trigger distance for this flight. The camera is triggered each time this distance is exceeded. This command can also be used to set the shutter integration time for the camera.")]
        DO_SET_CAM_TRIGG_DIST=206, 
    	///<summary> Mission command to enable the geofence |enable? (0=disable, 1=enable, 2=disable_floor_only)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Mission command to enable the geofence")]
        DO_FENCE_ENABLE=207, 
    	///<summary> Mission command to trigger a parachute |action (0=disable, 1=enable, 2=release, for some systems see PARACHUTE_ACTION enum, not in general message set.)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Mission command to trigger a parachute")]
        DO_PARACHUTE=208, 
    	///<summary> Mission command to perform motor test |motor number (a number from 1 to max number of motors on the vehicle)| throttle type (0=throttle percentage, 1=PWM, 2=pilot throttle channel pass-through. See MOTOR_TEST_THROTTLE_TYPE enum)| throttle| timeout (in seconds)| motor count (number of motors to test to test in sequence, waiting for the timeout above between them; 0=1 motor, 1=1 motor, 2=2 motors...)| motor test order (See MOTOR_TEST_ORDER enum)| Empty|  </summary>
        [Description("Mission command to perform motor test")]
        DO_MOTOR_TEST=209, 
    	///<summary> Change to/from inverted flight |inverted (0=normal, 1=inverted)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Change to/from inverted flight")]
        DO_INVERTED_FLIGHT=210, 
    	///<summary> Mission command to operate EPM gripper. |Gripper number (a number from 1 to max number of grippers on the vehicle).| Gripper action (0=release, 1=grab. See GRIPPER_ACTIONS enum).| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Mission command to operate EPM gripper.")]
        DO_GRIPPER=211, 
    	///<summary> Enable/disable autotune. |Enable (1: enable, 0:disable).| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Enable/disable autotune.")]
        DO_AUTOTUNE_ENABLE=212, 
    	///<summary> Sets a desired vehicle turn angle and speed change |yaw angle to adjust steering by in centidegress| speed - normalized to 0 .. 1| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Sets a desired vehicle turn angle and speed change")]
        SET_YAW_SPEED=213, 
    	///<summary> Mission command to set camera trigger interval for this flight. If triggering is enabled, the camera is triggered each time this interval expires. This command can also be used to set the shutter integration time for the camera. |Camera trigger cycle time (milliseconds). -1 or 0 to ignore.| Camera shutter integration time (milliseconds). Should be less than trigger cycle time. -1 or 0 to ignore.| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Mission command to set camera trigger interval for this flight. If triggering is enabled, the camera is triggered each time this interval expires. This command can also be used to set the shutter integration time for the camera.")]
        DO_SET_CAM_TRIGG_INTERVAL=214, 
    	///<summary> Mission command to control a camera or antenna mount, using a quaternion as reference. |q1 - quaternion param #1, w (1 in null-rotation)| q2 - quaternion param #2, x (0 in null-rotation)| q3 - quaternion param #3, y (0 in null-rotation)| q4 - quaternion param #4, z (0 in null-rotation)| Empty| Empty| Empty|  </summary>
        [Description("Mission command to control a camera or antenna mount, using a quaternion as reference.")]
        DO_MOUNT_CONTROL_QUAT=220, 
    	///<summary> set id of master controller |System ID| Component ID| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("set id of master controller")]
        DO_GUIDED_MASTER=221, 
    	///<summary> Set limits for external control |Timeout - maximum time (in seconds) that external controller will be allowed to control vehicle. 0 means no timeout.| Altitude (MSL) min, in meters - if vehicle moves below this alt, the command will be aborted and the mission will continue. 0 means no lower altitude limit.| Altitude (MSL) max, in meters - if vehicle moves above this alt, the command will be aborted and the mission will continue. 0 means no upper altitude limit.| Horizontal move limit, in meters - if vehicle moves more than this distance from its location at the moment the command was executed, the command will be aborted and the mission will continue. 0 means no horizontal move limit.| Empty| Empty| Empty|  </summary>
        [Description("Set limits for external control")]
        DO_GUIDED_LIMITS=222, 
    	///<summary> Control vehicle engine. This is interpreted by the vehicles engine controller to change the target engine state. It is intended for vehicles with internal combustion engines |0: Stop engine, 1:Start Engine| 0: Warm start, 1:Cold start. Controls use of choke where applicable| Height delay (meters). This is for commanding engine start only after the vehicle has gained the specified height. Used in VTOL vehicles during takeoff to start engine after the aircraft is off the ground. Zero for no delay.| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Control vehicle engine. This is interpreted by the vehicles engine controller to change the target engine state. It is intended for vehicles with internal combustion engines")]
        DO_ENGINE_CONTROL=223, 
    	///<summary> Set the mission item with sequence number seq as current item. This means that the MAV will continue to this mission item on the shortest path (not following the mission items in-between). |Mission sequence value to set| Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Set the mission item with sequence number seq as current item. This means that the MAV will continue to this mission item on the shortest path (not following the mission items in-between).")]
        DO_SET_MISSION_CURRENT=224, 
    	///<summary> NOP - This command is only used to mark the upper limit of the DO commands in the enumeration |Empty| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("NOP - This command is only used to mark the upper limit of the DO commands in the enumeration")]
        DO_LAST=240, 
    	///<summary> Trigger calibration. This command will be only accepted if in pre-flight mode. Except for Temperature Calibration, only one sensor should be set in a single message and all others should be zero. |1: gyro calibration, 3: gyro temperature calibration| 1: magnetometer calibration| 1: ground pressure calibration| 1: radio RC calibration, 2: RC trim calibration| 1: accelerometer calibration, 2: board level calibration, 3: accelerometer temperature calibration, 4: simple accelerometer calibration| 1: APM: compass/motor interference calibration (PX4: airspeed calibration, deprecated), 2: airspeed calibration| 1: ESC calibration, 3: barometer temperature calibration|  </summary>
        [Description("Trigger calibration. This command will be only accepted if in pre-flight mode. Except for Temperature Calibration, only one sensor should be set in a single message and all others should be zero.")]
        PREFLIGHT_CALIBRATION=241, 
    	///<summary> Set sensor offsets. This command will be only accepted if in pre-flight mode. |Sensor to adjust the offsets for: 0: gyros, 1: accelerometer, 2: magnetometer, 3: barometer, 4: optical flow, 5: second magnetometer, 6: third magnetometer| X axis offset (or generic dimension 1), in the sensor's raw units| Y axis offset (or generic dimension 2), in the sensor's raw units| Z axis offset (or generic dimension 3), in the sensor's raw units| Generic dimension 4, in the sensor's raw units| Generic dimension 5, in the sensor's raw units| Generic dimension 6, in the sensor's raw units|  </summary>
        [Description("Set sensor offsets. This command will be only accepted if in pre-flight mode.")]
        PREFLIGHT_SET_SENSOR_OFFSETS=242, 
    	///<summary> Trigger UAVCAN config. This command will be only accepted if in pre-flight mode. |1: Trigger actuator ID assignment and direction mapping.| Reserved| Reserved| Reserved| Reserved| Reserved| Reserved|  </summary>
        [Description("Trigger UAVCAN config. This command will be only accepted if in pre-flight mode.")]
        PREFLIGHT_UAVCAN=243, 
    	///<summary> Request storage of different parameter values and logs. This command will be only accepted if in pre-flight mode. |Parameter storage: 0: READ FROM FLASH/EEPROM, 1: WRITE CURRENT TO FLASH/EEPROM, 2: Reset to defaults| Mission storage: 0: READ FROM FLASH/EEPROM, 1: WRITE CURRENT TO FLASH/EEPROM, 2: Reset to defaults| Onboard logging: 0: Ignore, 1: Start default rate logging, -1: Stop logging, > 1: start logging with rate of param 3 in Hz (e.g. set to 1000 for 1000 Hz logging)| Reserved| Empty| Empty| Empty|  </summary>
        [Description("Request storage of different parameter values and logs. This command will be only accepted if in pre-flight mode.")]
        PREFLIGHT_STORAGE=245, 
    	///<summary> Request the reboot or shutdown of system components. |0: Do nothing for autopilot, 1: Reboot autopilot, 2: Shutdown autopilot, 3: Reboot autopilot and keep it in the bootloader until upgraded.| 0: Do nothing for onboard computer, 1: Reboot onboard computer, 2: Shutdown onboard computer, 3: Reboot onboard computer and keep it in the bootloader until upgraded.| WIP: 0: Do nothing for camera, 1: Reboot onboard camera, 2: Shutdown onboard camera, 3: Reboot onboard camera and keep it in the bootloader until upgraded| WIP: 0: Do nothing for mount (e.g. gimbal), 1: Reboot mount, 2: Shutdown mount, 3: Reboot mount and keep it in the bootloader until upgraded| Reserved, send 0| Reserved, send 0| WIP: ID (e.g. camera ID -1 for all IDs)|  </summary>
        [Description("Request the reboot or shutdown of system components.")]
        PREFLIGHT_REBOOT_SHUTDOWN=246, 
    	///<summary> Hold / continue the current action |MAV_GOTO_DO_HOLD: hold MAV_GOTO_DO_CONTINUE: continue with next item in mission plan| MAV_GOTO_HOLD_AT_CURRENT_POSITION: Hold at current position MAV_GOTO_HOLD_AT_SPECIFIED_POSITION: hold at specified position| MAV_FRAME coordinate frame of hold point| Desired yaw angle in degrees| Latitude / X position| Longitude / Y position| Altitude / Z position|  </summary>
        [Description("Hold / continue the current action")]
        OVERRIDE_GOTO=252, 
    	///<summary> start running a mission |first_item: the first mission item to run| last_item:  the last mission item to run (after this item is run, the mission ends)|  </summary>
        [Description("start running a mission")]
        MISSION_START=300, 
    	///<summary> Arms / Disarms a component |1 to arm, 0 to disarm|  </summary>
        [Description("Arms / Disarms a component")]
        COMPONENT_ARM_DISARM=400, 
    	///<summary> Request the home position from the vehicle. |Reserved| Reserved| Reserved| Reserved| Reserved| Reserved| Reserved|  </summary>
        [Description("Request the home position from the vehicle.")]
        GET_HOME_POSITION=410, 
    	///<summary> Starts receiver pairing |0:Spektrum| RC type (see RC_TYPE enum)|  </summary>
        [Description("Starts receiver pairing")]
        START_RX_PAIR=500, 
    	///<summary> Request the interval between messages for a particular MAVLink message ID |The MAVLink message ID|  </summary>
        [Description("Request the interval between messages for a particular MAVLink message ID")]
        GET_MESSAGE_INTERVAL=510, 
    	///<summary> Set the interval between messages for a particular MAVLink message ID. This interface replaces REQUEST_DATA_STREAM |The MAVLink message ID| The interval between two messages, in microseconds. Set to -1 to disable and 0 to request default rate.|  </summary>
        [Description("Set the interval between messages for a particular MAVLink message ID. This interface replaces REQUEST_DATA_STREAM")]
        SET_MESSAGE_INTERVAL=511, 
    	///<summary> Request the target system(s) emit a single instance of a specified message (i.e. a 'one-shot' version of MAV_CMD_SET_MESSAGE_INTERVAL). |The MAVLink message ID of the requested message.| Index id (if appropriate). The use of this parameter (if any), must be defined in the requested message.|  </summary>
        [Description("Request the target system(s) emit a single instance of a specified message (i.e. a 'one-shot' version of MAV_CMD_SET_MESSAGE_INTERVAL).")]
        REQUEST_MESSAGE=512, 
    	///<summary> Request autopilot capabilities |1: Request autopilot version| Reserved (all remaining params)|  </summary>
        [Description("Request autopilot capabilities")]
        REQUEST_AUTOPILOT_CAPABILITIES=520, 
    	///<summary> Request camera information (CAMERA_INFORMATION). |0: No action 1: Request camera capabilities| Reserved (all remaining params)|  </summary>
        [Description("Request camera information (CAMERA_INFORMATION).")]
        REQUEST_CAMERA_INFORMATION=521, 
    	///<summary> Request camera settings (CAMERA_SETTINGS). |0: No Action 1: Request camera settings| Reserved (all remaining params)|  </summary>
        [Description("Request camera settings (CAMERA_SETTINGS).")]
        REQUEST_CAMERA_SETTINGS=522, 
    	///<summary> Request storage information (STORAGE_INFORMATION). Use the command's target_component to target a specific component's storage. |Storage ID (0 for all, 1 for first, 2 for second, etc.)| 0: No Action 1: Request storage information| Reserved (all remaining params)|  </summary>
        [Description("Request storage information (STORAGE_INFORMATION). Use the command's target_component to target a specific component's storage.")]
        REQUEST_STORAGE_INFORMATION=525, 
    	///<summary> Format a storage medium. Once format is complete, a STORAGE_INFORMATION message is sent. Use the command's target_component to target a specific component's storage. |Storage ID (1 for first, 2 for second, etc.)| 0: No action 1: Format storage| Reserved (all remaining params)|  </summary>
        [Description("Format a storage medium. Once format is complete, a STORAGE_INFORMATION message is sent. Use the command's target_component to target a specific component's storage.")]
        STORAGE_FORMAT=526, 
    	///<summary> Request camera capture status (CAMERA_CAPTURE_STATUS) |0: No Action 1: Request camera capture status| Reserved (all remaining params)|  </summary>
        [Description("Request camera capture status (CAMERA_CAPTURE_STATUS)")]
        REQUEST_CAMERA_CAPTURE_STATUS=527, 
    	///<summary> Request flight information (FLIGHT_INFORMATION) |1: Request flight information| Reserved (all remaining params)|  </summary>
        [Description("Request flight information (FLIGHT_INFORMATION)")]
        REQUEST_FLIGHT_INFORMATION=528, 
    	///<summary> Reset all camera settings to Factory Default |0: No Action 1: Reset all settings| Reserved (all remaining params)|  </summary>
        [Description("Reset all camera settings to Factory Default")]
        RESET_CAMERA_SETTINGS=529, 
    	///<summary> Set camera running mode. Use NaN for reserved values. GCS will send a MAV_CMD_REQUEST_VIDEO_STREAM_STATUS command after a mode change if the camera supports video streaming. |Reserved (Set to 0)| Camera mode| Reserved (all remaining params)|  </summary>
        [Description("Set camera running mode. Use NaN for reserved values. GCS will send a MAV_CMD_REQUEST_VIDEO_STREAM_STATUS command after a mode change if the camera supports video streaming.")]
        SET_CAMERA_MODE=530, 
    	///<summary> Tagged jump target. Can be jumped to with MAV_CMD_DO_JUMP_TAG. |Tag.|  </summary>
        [Description("Tagged jump target. Can be jumped to with MAV_CMD_DO_JUMP_TAG.")]
        JUMP_TAG=600, 
    	///<summary> Jump to the matching tag in the mission list. Repeat this action for the specified number of times. A mission should contain a single matching tag for each jump. If this is not the case then a jump to a missing tag should complete the mission, and a jump where there are multiple matching tags should always select the one with the lowest mission sequence number. |Target tag to jump to.| Repeat count|  </summary>
        [Description("Jump to the matching tag in the mission list. Repeat this action for the specified number of times. A mission should contain a single matching tag for each jump. If this is not the case then a jump to a missing tag should complete the mission, and a jump where there are multiple matching tags should always select the one with the lowest mission sequence number.")]
        DO_JUMP_TAG=601, 
    	///<summary> Start image capture sequence. Sends CAMERA_IMAGE_CAPTURED after each capture. Use NaN for reserved values. |Reserved (Set to 0)| Desired elapsed time between two consecutive pictures (in seconds). Minimum values depend on hardware (typically greater than 2 seconds).| Total number of images to capture. 0 to capture forever/until MAV_CMD_IMAGE_STOP_CAPTURE.| Capture sequence number starting from 1. This is only valid for single-capture (param3 == 1). Increment the capture ID for each capture command to prevent double captures when a command is re-transmitted. Use 0 to ignore it.| Reserved (all remaining params)|  </summary>
        [Description("Start image capture sequence. Sends CAMERA_IMAGE_CAPTURED after each capture. Use NaN for reserved values.")]
        IMAGE_START_CAPTURE=2000, 
    	///<summary> Stop image capture sequence Use NaN for reserved values. |Reserved (Set to 0)| Reserved (all remaining params)|  </summary>
        [Description("Stop image capture sequence Use NaN for reserved values.")]
        IMAGE_STOP_CAPTURE=2001, 
    	///<summary> Enable or disable on-board camera triggering system. |Trigger enable/disable (0 for disable, 1 for start), -1 to ignore| 1 to reset the trigger sequence, -1 or 0 to ignore| 1 to pause triggering, but without switching the camera off or retracting it. -1 to ignore|  </summary>
        [Description("Enable or disable on-board camera triggering system.")]
        DO_TRIGGER_CONTROL=2003, 
    	///<summary> Starts video capture (recording). Use NaN for reserved values. |Video Stream ID (0 for all streams)| Frequency CAMERA_CAPTURE_STATUS messages should be sent while recording (0 for no messages, otherwise frequency in Hz)| Reserved (all remaining params)|  </summary>
        [Description("Starts video capture (recording). Use NaN for reserved values.")]
        VIDEO_START_CAPTURE=2500, 
    	///<summary> Stop the current video capture (recording). Use NaN for reserved values. |Video Stream ID (0 for all streams)| Reserved (all remaining params)|  </summary>
        [Description("Stop the current video capture (recording). Use NaN for reserved values.")]
        VIDEO_STOP_CAPTURE=2501, 
    	///<summary> Request to start streaming logging data over MAVLink (see also LOGGING_DATA message) |Format: 0: ULog| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)|  </summary>
        [Description("Request to start streaming logging data over MAVLink (see also LOGGING_DATA message)")]
        LOGGING_START=2510, 
    	///<summary> Request to stop streaming log data over MAVLink |Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)|  </summary>
        [Description("Request to stop streaming log data over MAVLink")]
        LOGGING_STOP=2511, 
    	///<summary>  |Landing gear ID (default: 0, -1 for all)| Landing gear position (Down: 0, Up: 1, NaN for no change)| Reserved, set to NaN| Reserved, set to NaN| Reserved, set to NaN| Reserved, set to NaN| Reserved, set to NaN|  </summary>
        [Description("")]
        AIRFRAME_CONFIGURATION=2520, 
    	///<summary> Request to start/stop transmitting over the high latency telemetry |Control transmission over high latency telemetry (0: stop, 1: start)| Empty| Empty| Empty| Empty| Empty| Empty|  </summary>
        [Description("Request to start/stop transmitting over the high latency telemetry")]
        CONTROL_HIGH_LATENCY=2600, 
    	///<summary> Create a panorama at the current position |Viewing angle horizontal of the panorama (in degrees, +- 0.5 the total angle)| Viewing angle vertical of panorama (in degrees)| Speed of the horizontal rotation (in degrees per second)| Speed of the vertical rotation (in degrees per second)|  </summary>
        [Description("Create a panorama at the current position")]
        PANORAMA_CREATE=2800, 
    	///<summary> Request VTOL transition |The target VTOL state, as defined by ENUM MAV_VTOL_STATE. Only MAV_VTOL_STATE_MC and MAV_VTOL_STATE_FW can be used.|  </summary>
        [Description("Request VTOL transition")]
        DO_VTOL_TRANSITION=3000, 
    	///<summary> Request authorization to arm the vehicle to a external entity, the arm authorizer is responsible to request all data that is needs from the vehicle before authorize or deny the request. If approved the progress of command_ack message should be set with period of time that this authorization is valid in seconds or in case it was denied it should be set with one of the reasons in ARM_AUTH_DENIED_REASON.          |Vehicle system id, this way ground station can request arm authorization on behalf of any vehicle|  </summary>
        [Description("Request authorization to arm the vehicle to a external entity, the arm authorizer is responsible to request all data that is needs from the vehicle before authorize or deny the request. If approved the progress of command_ack message should be set with period of time that this authorization is valid in seconds or in case it was denied it should be set with one of the reasons in ARM_AUTH_DENIED_REASON.         ")]
        ARM_AUTHORIZATION_REQUEST=3001, 
    	///<summary> This command sets the submode to standard guided when vehicle is in guided mode. The vehicle holds position and altitude and the user can input the desired velocities along all three axes.                    | </summary>
        [Description("This command sets the submode to standard guided when vehicle is in guided mode. The vehicle holds position and altitude and the user can input the desired velocities along all three axes.                   ")]
        SET_GUIDED_SUBMODE_STANDARD=4000, 
    	///<summary> This command sets submode circle when vehicle is in guided mode. Vehicle flies along a circle facing the center of the circle. The user can input the velocity along the circle and change the radius. If no input is given the vehicle will hold position.                    |Radius of desired circle in CIRCLE_MODE| User defined| User defined| User defined| Unscaled target latitude of center of circle in CIRCLE_MODE| Unscaled target longitude of center of circle in CIRCLE_MODE|  </summary>
        [Description("This command sets submode circle when vehicle is in guided mode. Vehicle flies along a circle facing the center of the circle. The user can input the velocity along the circle and change the radius. If no input is given the vehicle will hold position.                   ")]
        SET_GUIDED_SUBMODE_CIRCLE=4001, 
    	///<summary> Fence return point. There can only be one fence return point.          |Reserved| Reserved| Reserved| Reserved| Latitude| Longitude| Altitude|  </summary>
        [Description("Fence return point. There can only be one fence return point.         ")]
        FENCE_RETURN_POINT=5000, 
    	///<summary> Fence vertex for an inclusion polygon (the polygon must not be self-intersecting). The vehicle must stay within this area. Minimum of 3 vertices required.          |Polygon vertex count| Reserved| Reserved| Reserved| Latitude| Longitude| Reserved|  </summary>
        [Description("Fence vertex for an inclusion polygon (the polygon must not be self-intersecting). The vehicle must stay within this area. Minimum of 3 vertices required.         ")]
        FENCE_POLYGON_VERTEX_INCLUSION=5001, 
    	///<summary> Fence vertex for an exclusion polygon (the polygon must not be self-intersecting). The vehicle must stay outside this area. Minimum of 3 vertices required.          |Polygon vertex count| Reserved| Reserved| Reserved| Latitude| Longitude| Reserved|  </summary>
        [Description("Fence vertex for an exclusion polygon (the polygon must not be self-intersecting). The vehicle must stay outside this area. Minimum of 3 vertices required.         ")]
        FENCE_POLYGON_VERTEX_EXCLUSION=5002, 
    	///<summary> Circular fence area. The vehicle must stay inside this area.          |radius in meters| Reserved| Reserved| Reserved| Latitude| Longitude| Reserved|  </summary>
        [Description("Circular fence area. The vehicle must stay inside this area.         ")]
        FENCE_CIRCLE_INCLUSION=5003, 
    	///<summary> Circular fence area. The vehicle must stay outside this area.          |radius in meters| Reserved| Reserved| Reserved| Latitude| Longitude| Reserved|  </summary>
        [Description("Circular fence area. The vehicle must stay outside this area.         ")]
        FENCE_CIRCLE_EXCLUSION=5004, 
    	///<summary> Rally point. You can have multiple rally points defined.          |Reserved| Reserved| Reserved| Reserved| Latitude| Longitude| Altitude|  </summary>
        [Description("Rally point. You can have multiple rally points defined.         ")]
        RALLY_POINT=5100, 
    	///<summary> Commands the vehicle to respond with a sequence of messages UAVCAN_NODE_INFO, one message per every UAVCAN node that is online. Note that some of the response messages can be lost, which the receiver can detect easily by checking whether every received UAVCAN_NODE_STATUS has a matching message UAVCAN_NODE_INFO received earlier; if not, this command should be sent again in order to request re-transmission of the node information messages. |Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)| Reserved (set to 0)|  </summary>
        [Description("Commands the vehicle to respond with a sequence of messages UAVCAN_NODE_INFO, one message per every UAVCAN node that is online. Note that some of the response messages can be lost, which the receiver can detect easily by checking whether every received UAVCAN_NODE_STATUS has a matching message UAVCAN_NODE_INFO received earlier; if not, this command should be sent again in order to request re-transmission of the node information messages.")]
        UAVCAN_GET_NODE_INFO=5200, 
    	///<summary> Deploy payload on a Lat / Lon / Alt position. This includes the navigation to reach the required release position and velocity. |Operation mode. 0: prepare single payload deploy (overwriting previous requests), but do not execute it. 1: execute payload deploy immediately (rejecting further deploy commands during execution, but allowing abort). 2: add payload deploy to existing deployment list.| Desired approach vector in degrees compass heading (0..360). A negative value indicates the system can define the approach vector at will.| Desired ground speed at release time. This can be overridden by the airframe in case it needs to meet minimum airspeed. A negative value indicates the system can define the ground speed at will.| Minimum altitude clearance to the release position in meters. A negative value indicates the system can define the clearance at will.| Latitude unscaled for MISSION_ITEM or in 1e7 degrees for MISSION_ITEM_INT| Longitude unscaled for MISSION_ITEM or in 1e7 degrees for MISSION_ITEM_INT| Altitude (MSL), in meters|  </summary>
        [Description("Deploy payload on a Lat / Lon / Alt position. This includes the navigation to reach the required release position and velocity.")]
        PAYLOAD_PREPARE_DEPLOY=30001, 
    	///<summary> Control the payload deployment. |Operation mode. 0: Abort deployment, continue normal mission. 1: switch to payload deployment mode. 100: delete first payload deployment request. 101: delete all payload deployment requests.| Reserved| Reserved| Reserved| Reserved| Reserved| Reserved|  </summary>
        [Description("Control the payload deployment.")]
        PAYLOAD_CONTROL_DEPLOY=30002, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
        WAYPOINT_USER_1=31000, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
        WAYPOINT_USER_2=31001, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
        WAYPOINT_USER_3=31002, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
        WAYPOINT_USER_4=31003, 
    	///<summary> User defined waypoint item. Ground Station will show the Vehicle as flying through this item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
        WAYPOINT_USER_5=31004, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
        SPATIAL_USER_1=31005, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
        SPATIAL_USER_2=31006, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
        SPATIAL_USER_3=31007, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
        SPATIAL_USER_4=31008, 
    	///<summary> User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item. |User defined| User defined| User defined| User defined| Latitude unscaled| Longitude unscaled| Altitude (MSL), in meters|  </summary>
        [Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
        SPATIAL_USER_5=31009, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        [Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
        USER_1=31010, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        [Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
        USER_2=31011, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        [Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
        USER_3=31012, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        [Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
        USER_4=31013, 
    	///<summary> User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item. |User defined| User defined| User defined| User defined| User defined| User defined| User defined|  </summary>
        [Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
        USER_5=31014, 
    	///<summary> A system wide power-off event has been initiated. |Empty.| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("A system wide power-off event has been initiated.")]
        POWER_OFF_INITIATED=42000, 
    	///<summary> FLY button has been clicked. |Empty.| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("FLY button has been clicked.")]
        SOLO_BTN_FLY_CLICK=42001, 
    	///<summary> FLY button has been held for 1.5 seconds. |Takeoff altitude.| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("FLY button has been held for 1.5 seconds.")]
        SOLO_BTN_FLY_HOLD=42002, 
    	///<summary> PAUSE button has been clicked. |1 if Solo is in a shot mode, 0 otherwise.| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("PAUSE button has been clicked.")]
        SOLO_BTN_PAUSE_CLICK=42003, 
    	///<summary> Magnetometer calibration based on fixed position         in earth field given by inclination, declination and intensity. |MagDeclinationDegrees.| MagInclinationDegrees.| MagIntensityMilliGauss.| YawDegrees.| Empty.| Empty.| Empty.|  </summary>
        [Description("Magnetometer calibration based on fixed position         in earth field given by inclination, declination and intensity.")]
        FIXED_MAG_CAL=42004, 
    	///<summary> Magnetometer calibration based on fixed expected field values in milliGauss. |FieldX.| FieldY.| FieldZ.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Magnetometer calibration based on fixed expected field values in milliGauss.")]
        FIXED_MAG_CAL_FIELD=42005, 
    	///<summary> Initiate a magnetometer calibration. |uint8_t bitmask of magnetometers (0 means all).| Automatically retry on failure (0=no retry, 1=retry).| Save without user input (0=require input, 1=autosave).| Delay (seconds).| Autoreboot (0=user reboot, 1=autoreboot).| Empty.| Empty.|  </summary>
        [Description("Initiate a magnetometer calibration.")]
        DO_START_MAG_CAL=42424, 
    	///<summary> Initiate a magnetometer calibration. |uint8_t bitmask of magnetometers (0 means all).| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Initiate a magnetometer calibration.")]
        DO_ACCEPT_MAG_CAL=42425, 
    	///<summary> Cancel a running magnetometer calibration. |uint8_t bitmask of magnetometers (0 means all).| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Cancel a running magnetometer calibration.")]
        DO_CANCEL_MAG_CAL=42426, 
    	///<summary> Command autopilot to get into factory test/diagnostic mode. |0 means get out of test mode, 1 means get into test mode.| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Command autopilot to get into factory test/diagnostic mode.")]
        SET_FACTORY_TEST_MODE=42427, 
    	///<summary> Reply with the version banner. |Empty.| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Reply with the version banner.")]
        DO_SEND_BANNER=42428, 
    	///<summary> Used when doing accelerometer calibration. When sent to the GCS tells it what position to put the vehicle in. When sent to the vehicle says what position the vehicle is in. |Position, one of the ACCELCAL_VEHICLE_POS enum values.| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Used when doing accelerometer calibration. When sent to the GCS tells it what position to put the vehicle in. When sent to the vehicle says what position the vehicle is in.")]
        ACCELCAL_VEHICLE_POS=42429, 
    	///<summary> Causes the gimbal to reset and boot as if it was just powered on. |Empty.| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Causes the gimbal to reset and boot as if it was just powered on.")]
        GIMBAL_RESET=42501, 
    	///<summary> Reports progress and success or failure of gimbal axis calibration procedure. |Gimbal axis we're reporting calibration progress for.| Current calibration progress for this axis, 0x64=100%.| Status of the calibration.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Reports progress and success or failure of gimbal axis calibration procedure.")]
        GIMBAL_AXIS_CALIBRATION_STATUS=42502, 
    	///<summary> Starts commutation calibration on the gimbal. |Empty.| Empty.| Empty.| Empty.| Empty.| Empty.| Empty.|  </summary>
        [Description("Starts commutation calibration on the gimbal.")]
        GIMBAL_REQUEST_AXIS_CALIBRATION=42503, 
    	///<summary> Erases gimbal application and parameters. |Magic number.| Magic number.| Magic number.| Magic number.| Magic number.| Magic number.| Magic number.|  </summary>
        [Description("Erases gimbal application and parameters.")]
        GIMBAL_FULL_RESET=42505, 
    	///<summary> Command to operate winch. |Winch number (0 for the default winch, otherwise a number from 1 to max number of winches on the vehicle).| Action (0=relax, 1=relative length control, 2=rate control. See WINCH_ACTIONS enum.).| Release length (cable distance to unwind in meters, negative numbers to wind in cable).| Release rate (meters/second).| Empty.| Empty.| Empty.|  </summary>
        [Description("Command to operate winch.")]
        DO_WINCH=42600, 
    	///<summary> Update the bootloader |Empty| Empty| Empty| Empty| Magic number - set to 290876 to actually flash| Empty| Empty|  </summary>
        [Description("Update the bootloader")]
        FLASH_BOOTLOADER=42650, 
    
    };
    
    ///<summary>  </summary>
    public enum LIMITS_STATE: byte
    {
			///<summary> Pre-initialization. | </summary>
        [Description("Pre-initialization.")]
        LIMITS_INIT=0, 
    	///<summary> Disabled. | </summary>
        [Description("Disabled.")]
        LIMITS_DISABLED=1, 
    	///<summary> Checking limits. | </summary>
        [Description("Checking limits.")]
        LIMITS_ENABLED=2, 
    	///<summary> A limit has been breached. | </summary>
        [Description("A limit has been breached.")]
        LIMITS_TRIGGERED=3, 
    	///<summary> Taking action e.g. Return/RTL. | </summary>
        [Description("Taking action e.g. Return/RTL.")]
        LIMITS_RECOVERING=4, 
    	///<summary> We're no longer in breach of a limit. | </summary>
        [Description("We're no longer in breach of a limit.")]
        LIMITS_RECOVERED=5, 
    
    };
    
    ///<summary>  </summary>
    public enum LIMIT_MODULE: byte
    {
			///<summary> Pre-initialization. | </summary>
        [Description("Pre-initialization.")]
        LIMIT_GPSLOCK=1, 
    	///<summary> Disabled. | </summary>
        [Description("Disabled.")]
        LIMIT_GEOFENCE=2, 
    	///<summary> Checking limits. | </summary>
        [Description("Checking limits.")]
        LIMIT_ALTITUDE=4, 
    
    };
    
    ///<summary> Flags in RALLY_POINT message. </summary>
    public enum RALLY_FLAGS: byte
    {
			///<summary> Flag set when requiring favorable winds for landing. | </summary>
        [Description("Flag set when requiring favorable winds for landing.")]
        FAVORABLE_WIND=1, 
    	///<summary> Flag set when plane is to immediately descend to break altitude and land without GCS intervention. Flag not set when plane is to loiter at Rally point until commanded to land. | </summary>
        [Description("Flag set when plane is to immediately descend to break altitude and land without GCS intervention. Flag not set when plane is to loiter at Rally point until commanded to land.")]
        LAND_IMMEDIATELY=2, 
    
    };
    
    ///<summary>  </summary>
    public enum PARACHUTE_ACTION: int /*default*/
    {
			///<summary> Disable parachute release. | </summary>
        [Description("Disable parachute release.")]
        PARACHUTE_DISABLE=0, 
    	///<summary> Enable parachute release. | </summary>
        [Description("Enable parachute release.")]
        PARACHUTE_ENABLE=1, 
    	///<summary> Release parachute. | </summary>
        [Description("Release parachute.")]
        PARACHUTE_RELEASE=2, 
    
    };
    
    ///<summary> Gripper actions. </summary>
    public enum GRIPPER_ACTIONS: int /*default*/
    {
			///<summary> Gripper release cargo. | </summary>
        [Description("Gripper release cargo.")]
        GRIPPER_ACTION_RELEASE=0, 
    	///<summary> Gripper grab onto cargo. | </summary>
        [Description("Gripper grab onto cargo.")]
        GRIPPER_ACTION_GRAB=1, 
    
    };
    
    ///<summary> Winch actions. </summary>
    public enum WINCH_ACTIONS: int /*default*/
    {
			///<summary> Relax winch. | </summary>
        [Description("Relax winch.")]
        WINCH_RELAXED=0, 
    	///<summary> Winch unwinds or winds specified length of cable optionally using specified rate. | </summary>
        [Description("Winch unwinds or winds specified length of cable optionally using specified rate.")]
        WINCH_RELATIVE_LENGTH_CONTROL=1, 
    	///<summary> Winch unwinds or winds cable at specified rate in meters/seconds. | </summary>
        [Description("Winch unwinds or winds cable at specified rate in meters/seconds.")]
        WINCH_RATE_CONTROL=2, 
    
    };
    
    ///<summary>  </summary>
    public enum CAMERA_STATUS_TYPES: byte
    {
			///<summary> Camera heartbeat, announce camera component ID at 1Hz. | </summary>
        [Description("Camera heartbeat, announce camera component ID at 1Hz.")]
        CAMERA_STATUS_TYPE_HEARTBEAT=0, 
    	///<summary> Camera image triggered. | </summary>
        [Description("Camera image triggered.")]
        CAMERA_STATUS_TYPE_TRIGGER=1, 
    	///<summary> Camera connection lost. | </summary>
        [Description("Camera connection lost.")]
        CAMERA_STATUS_TYPE_DISCONNECT=2, 
    	///<summary> Camera unknown error. | </summary>
        [Description("Camera unknown error.")]
        CAMERA_STATUS_TYPE_ERROR=3, 
    	///<summary> Camera battery low. Parameter p1 shows reported voltage. | </summary>
        [Description("Camera battery low. Parameter p1 shows reported voltage.")]
        CAMERA_STATUS_TYPE_LOWBATT=4, 
    	///<summary> Camera storage low. Parameter p1 shows reported shots remaining. | </summary>
        [Description("Camera storage low. Parameter p1 shows reported shots remaining.")]
        CAMERA_STATUS_TYPE_LOWSTORE=5, 
    	///<summary> Camera storage low. Parameter p1 shows reported video minutes remaining. | </summary>
        [Description("Camera storage low. Parameter p1 shows reported video minutes remaining.")]
        CAMERA_STATUS_TYPE_LOWSTOREV=6, 
    
    };
    
    ///<summary>  </summary>
    public enum CAMERA_FEEDBACK_FLAGS: byte
    {
			///<summary> Shooting photos, not video. | </summary>
        [Description("Shooting photos, not video.")]
        CAMERA_FEEDBACK_PHOTO=0, 
    	///<summary> Shooting video, not stills. | </summary>
        [Description("Shooting video, not stills.")]
        CAMERA_FEEDBACK_VIDEO=1, 
    	///<summary> Unable to achieve requested exposure (e.g. shutter speed too low). | </summary>
        [Description("Unable to achieve requested exposure (e.g. shutter speed too low).")]
        CAMERA_FEEDBACK_BADEXPOSURE=2, 
    	///<summary> Closed loop feedback from camera, we know for sure it has successfully taken a picture. | </summary>
        [Description("Closed loop feedback from camera, we know for sure it has successfully taken a picture.")]
        CAMERA_FEEDBACK_CLOSEDLOOP=3, 
    	///<summary> Open loop camera, an image trigger has been requested but we can't know for sure it has successfully taken a picture. | </summary>
        [Description("Open loop camera, an image trigger has been requested but we can't know for sure it has successfully taken a picture.")]
        CAMERA_FEEDBACK_OPENLOOP=4, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_MODE_GIMBAL: int /*default*/
    {
			///<summary> Gimbal is powered on but has not started initializing yet. | </summary>
        [Description("Gimbal is powered on but has not started initializing yet.")]
        UNINITIALIZED=0, 
    	///<summary> Gimbal is currently running calibration on the pitch axis. | </summary>
        [Description("Gimbal is currently running calibration on the pitch axis.")]
        CALIBRATING_PITCH=1, 
    	///<summary> Gimbal is currently running calibration on the roll axis. | </summary>
        [Description("Gimbal is currently running calibration on the roll axis.")]
        CALIBRATING_ROLL=2, 
    	///<summary> Gimbal is currently running calibration on the yaw axis. | </summary>
        [Description("Gimbal is currently running calibration on the yaw axis.")]
        CALIBRATING_YAW=3, 
    	///<summary> Gimbal has finished calibrating and initializing, but is relaxed pending reception of first rate command from copter. | </summary>
        [Description("Gimbal has finished calibrating and initializing, but is relaxed pending reception of first rate command from copter.")]
        INITIALIZED=4, 
    	///<summary> Gimbal is actively stabilizing. | </summary>
        [Description("Gimbal is actively stabilizing.")]
        ACTIVE=5, 
    	///<summary> Gimbal is relaxed because it missed more than 10 expected rate command messages in a row. Gimbal will move back to active mode when it receives a new rate command. | </summary>
        [Description("Gimbal is relaxed because it missed more than 10 expected rate command messages in a row. Gimbal will move back to active mode when it receives a new rate command.")]
        RATE_CMD_TIMEOUT=6, 
    
    };
    
    ///<summary>  </summary>
    public enum GIMBAL_AXIS: int /*default*/
    {
			///<summary> Gimbal yaw axis. | </summary>
        [Description("Gimbal yaw axis.")]
        YAW=0, 
    	///<summary> Gimbal pitch axis. | </summary>
        [Description("Gimbal pitch axis.")]
        PITCH=1, 
    	///<summary> Gimbal roll axis. | </summary>
        [Description("Gimbal roll axis.")]
        ROLL=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GIMBAL_AXIS_CALIBRATION_STATUS: int /*default*/
    {
			///<summary> Axis calibration is in progress. | </summary>
        [Description("Axis calibration is in progress.")]
        IN_PROGRESS=0, 
    	///<summary> Axis calibration succeeded. | </summary>
        [Description("Axis calibration succeeded.")]
        SUCCEEDED=1, 
    	///<summary> Axis calibration failed. | </summary>
        [Description("Axis calibration failed.")]
        FAILED=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GIMBAL_AXIS_CALIBRATION_REQUIRED: int /*default*/
    {
			///<summary> Whether or not this axis requires calibration is unknown at this time. | </summary>
        [Description("Whether or not this axis requires calibration is unknown at this time.")]
        UNKNOWN=0, 
    	///<summary> This axis requires calibration. | </summary>
        [Description("This axis requires calibration.")]
        TRUE=1, 
    	///<summary> This axis does not require calibration. | </summary>
        [Description("This axis does not require calibration.")]
        FALSE=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_HEARTBEAT_STATUS: byte
    {
			///<summary> No GoPro connected. | </summary>
        [Description("No GoPro connected.")]
        DISCONNECTED=0, 
    	///<summary> The detected GoPro is not HeroBus compatible. | </summary>
        [Description("The detected GoPro is not HeroBus compatible.")]
        INCOMPATIBLE=1, 
    	///<summary> A HeroBus compatible GoPro is connected. | </summary>
        [Description("A HeroBus compatible GoPro is connected.")]
        CONNECTED=2, 
    	///<summary> An unrecoverable error was encountered with the connected GoPro, it may require a power cycle. | </summary>
        [Description("An unrecoverable error was encountered with the connected GoPro, it may require a power cycle.")]
        ERROR=3, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_HEARTBEAT_FLAGS: byte
    {
			///<summary> GoPro is currently recording. | </summary>
        [Description("GoPro is currently recording.")]
        GOPRO_FLAG_RECORDING=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_REQUEST_STATUS: byte
    {
			///<summary> The write message with ID indicated succeeded. | </summary>
        [Description("The write message with ID indicated succeeded.")]
        GOPRO_REQUEST_SUCCESS=0, 
    	///<summary> The write message with ID indicated failed. | </summary>
        [Description("The write message with ID indicated failed.")]
        GOPRO_REQUEST_FAILED=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_COMMAND: byte
    {
			///<summary> (Get/Set). | </summary>
        [Description("(Get/Set).")]
        POWER=0, 
    	///<summary> (Get/Set). | </summary>
        [Description("(Get/Set).")]
        CAPTURE_MODE=1, 
    	///<summary> (___/Set). | </summary>
        [Description("(___/Set).")]
        SHUTTER=2, 
    	///<summary> (Get/___). | </summary>
        [Description("(Get/___).")]
        BATTERY=3, 
    	///<summary> (Get/___). | </summary>
        [Description("(Get/___).")]
        MODEL=4, 
    	///<summary> (Get/Set). | </summary>
        [Description("(Get/Set).")]
        VIDEO_SETTINGS=5, 
    	///<summary> (Get/Set). | </summary>
        [Description("(Get/Set).")]
        LOW_LIGHT=6, 
    	///<summary> (Get/Set). | </summary>
        [Description("(Get/Set).")]
        PHOTO_RESOLUTION=7, 
    	///<summary> (Get/Set). | </summary>
        [Description("(Get/Set).")]
        PHOTO_BURST_RATE=8, 
    	///<summary> (Get/Set). | </summary>
        [Description("(Get/Set).")]
        PROTUNE=9, 
    	///<summary> (Get/Set) Hero 3+ Only. | </summary>
        [Description("(Get/Set) Hero 3+ Only.")]
        PROTUNE_WHITE_BALANCE=10, 
    	///<summary> (Get/Set) Hero 3+ Only. | </summary>
        [Description("(Get/Set) Hero 3+ Only.")]
        PROTUNE_COLOUR=11, 
    	///<summary> (Get/Set) Hero 3+ Only. | </summary>
        [Description("(Get/Set) Hero 3+ Only.")]
        PROTUNE_GAIN=12, 
    	///<summary> (Get/Set) Hero 3+ Only. | </summary>
        [Description("(Get/Set) Hero 3+ Only.")]
        PROTUNE_SHARPNESS=13, 
    	///<summary> (Get/Set) Hero 3+ Only. | </summary>
        [Description("(Get/Set) Hero 3+ Only.")]
        PROTUNE_EXPOSURE=14, 
    	///<summary> (Get/Set). | </summary>
        [Description("(Get/Set).")]
        TIME=15, 
    	///<summary> (Get/Set). | </summary>
        [Description("(Get/Set).")]
        CHARGING=16, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_CAPTURE_MODE: byte
    {
			///<summary> Video mode. | </summary>
        [Description("Video mode.")]
        VIDEO=0, 
    	///<summary> Photo mode. | </summary>
        [Description("Photo mode.")]
        PHOTO=1, 
    	///<summary> Burst mode, Hero 3+ only. | </summary>
        [Description("Burst mode, Hero 3+ only.")]
        BURST=2, 
    	///<summary> Time lapse mode, Hero 3+ only. | </summary>
        [Description("Time lapse mode, Hero 3+ only.")]
        TIME_LAPSE=3, 
    	///<summary> Multi shot mode, Hero 4 only. | </summary>
        [Description("Multi shot mode, Hero 4 only.")]
        MULTI_SHOT=4, 
    	///<summary> Playback mode, Hero 4 only, silver only except when LCD or HDMI is connected to black. | </summary>
        [Description("Playback mode, Hero 4 only, silver only except when LCD or HDMI is connected to black.")]
        PLAYBACK=5, 
    	///<summary> Playback mode, Hero 4 only. | </summary>
        [Description("Playback mode, Hero 4 only.")]
        SETUP=6, 
    	///<summary> Mode not yet known. | </summary>
        [Description("Mode not yet known.")]
        UNKNOWN=255, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_RESOLUTION: int /*default*/
    {
			///<summary> 848 x 480 (480p). | </summary>
        [Description("848 x 480 (480p).")]
        _480p=0, 
    	///<summary> 1280 x 720 (720p). | </summary>
        [Description("1280 x 720 (720p).")]
        _720p=1, 
    	///<summary> 1280 x 960 (960p). | </summary>
        [Description("1280 x 960 (960p).")]
        _960p=2, 
    	///<summary> 1920 x 1080 (1080p). | </summary>
        [Description("1920 x 1080 (1080p).")]
        _1080p=3, 
    	///<summary> 1920 x 1440 (1440p). | </summary>
        [Description("1920 x 1440 (1440p).")]
        _1440p=4, 
    	///<summary> 2704 x 1440 (2.7k-17:9). | </summary>
        [Description("2704 x 1440 (2.7k-17:9).")]
        _2_7k_17_9=5, 
    	///<summary> 2704 x 1524 (2.7k-16:9). | </summary>
        [Description("2704 x 1524 (2.7k-16:9).")]
        _2_7k_16_9=6, 
    	///<summary> 2704 x 2028 (2.7k-4:3). | </summary>
        [Description("2704 x 2028 (2.7k-4:3).")]
        _2_7k_4_3=7, 
    	///<summary> 3840 x 2160 (4k-16:9). | </summary>
        [Description("3840 x 2160 (4k-16:9).")]
        _4k_16_9=8, 
    	///<summary> 4096 x 2160 (4k-17:9). | </summary>
        [Description("4096 x 2160 (4k-17:9).")]
        _4k_17_9=9, 
    	///<summary> 1280 x 720 (720p-SuperView). | </summary>
        [Description("1280 x 720 (720p-SuperView).")]
        _720p_SUPERVIEW=10, 
    	///<summary> 1920 x 1080 (1080p-SuperView). | </summary>
        [Description("1920 x 1080 (1080p-SuperView).")]
        _1080p_SUPERVIEW=11, 
    	///<summary> 2704 x 1520 (2.7k-SuperView). | </summary>
        [Description("2704 x 1520 (2.7k-SuperView).")]
        _2_7k_SUPERVIEW=12, 
    	///<summary> 3840 x 2160 (4k-SuperView). | </summary>
        [Description("3840 x 2160 (4k-SuperView).")]
        _4k_SUPERVIEW=13, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_FRAME_RATE: int /*default*/
    {
			///<summary> 12 FPS. | </summary>
        [Description("12 FPS.")]
        _12=0, 
    	///<summary> 15 FPS. | </summary>
        [Description("15 FPS.")]
        _15=1, 
    	///<summary> 24 FPS. | </summary>
        [Description("24 FPS.")]
        _24=2, 
    	///<summary> 25 FPS. | </summary>
        [Description("25 FPS.")]
        _25=3, 
    	///<summary> 30 FPS. | </summary>
        [Description("30 FPS.")]
        _30=4, 
    	///<summary> 48 FPS. | </summary>
        [Description("48 FPS.")]
        _48=5, 
    	///<summary> 50 FPS. | </summary>
        [Description("50 FPS.")]
        _50=6, 
    	///<summary> 60 FPS. | </summary>
        [Description("60 FPS.")]
        _60=7, 
    	///<summary> 80 FPS. | </summary>
        [Description("80 FPS.")]
        _80=8, 
    	///<summary> 90 FPS. | </summary>
        [Description("90 FPS.")]
        _90=9, 
    	///<summary> 100 FPS. | </summary>
        [Description("100 FPS.")]
        _100=10, 
    	///<summary> 120 FPS. | </summary>
        [Description("120 FPS.")]
        _120=11, 
    	///<summary> 240 FPS. | </summary>
        [Description("240 FPS.")]
        _240=12, 
    	///<summary> 12.5 FPS. | </summary>
        [Description("12.5 FPS.")]
        _12_5=13, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_FIELD_OF_VIEW: int /*default*/
    {
			///<summary> 0x00: Wide. | </summary>
        [Description("0x00: Wide.")]
        WIDE=0, 
    	///<summary> 0x01: Medium. | </summary>
        [Description("0x01: Medium.")]
        MEDIUM=1, 
    	///<summary> 0x02: Narrow. | </summary>
        [Description("0x02: Narrow.")]
        NARROW=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_VIDEO_SETTINGS_FLAGS: int /*default*/
    {
			///<summary> 0=NTSC, 1=PAL. | </summary>
        [Description("0=NTSC, 1=PAL.")]
        GOPRO_VIDEO_SETTINGS_TV_MODE=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PHOTO_RESOLUTION: int /*default*/
    {
			///<summary> 5MP Medium. | </summary>
        [Description("5MP Medium.")]
        _5MP_MEDIUM=0, 
    	///<summary> 7MP Medium. | </summary>
        [Description("7MP Medium.")]
        _7MP_MEDIUM=1, 
    	///<summary> 7MP Wide. | </summary>
        [Description("7MP Wide.")]
        _7MP_WIDE=2, 
    	///<summary> 10MP Wide. | </summary>
        [Description("10MP Wide.")]
        _10MP_WIDE=3, 
    	///<summary> 12MP Wide. | </summary>
        [Description("12MP Wide.")]
        _12MP_WIDE=4, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_WHITE_BALANCE: int /*default*/
    {
			///<summary> Auto. | </summary>
        [Description("Auto.")]
        AUTO=0, 
    	///<summary> 3000K. | </summary>
        [Description("3000K.")]
        _3000K=1, 
    	///<summary> 5500K. | </summary>
        [Description("5500K.")]
        _5500K=2, 
    	///<summary> 6500K. | </summary>
        [Description("6500K.")]
        _6500K=3, 
    	///<summary> Camera Raw. | </summary>
        [Description("Camera Raw.")]
        RAW=4, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_COLOUR: int /*default*/
    {
			///<summary> Auto. | </summary>
        [Description("Auto.")]
        STANDARD=0, 
    	///<summary> Neutral. | </summary>
        [Description("Neutral.")]
        NEUTRAL=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_GAIN: int /*default*/
    {
			///<summary> ISO 400. | </summary>
        [Description("ISO 400.")]
        _400=0, 
    	///<summary> ISO 800 (Only Hero 4). | </summary>
        [Description("ISO 800 (Only Hero 4).")]
        _800=1, 
    	///<summary> ISO 1600. | </summary>
        [Description("ISO 1600.")]
        _1600=2, 
    	///<summary> ISO 3200 (Only Hero 4). | </summary>
        [Description("ISO 3200 (Only Hero 4).")]
        _3200=3, 
    	///<summary> ISO 6400. | </summary>
        [Description("ISO 6400.")]
        _6400=4, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_SHARPNESS: int /*default*/
    {
			///<summary> Low Sharpness. | </summary>
        [Description("Low Sharpness.")]
        LOW=0, 
    	///<summary> Medium Sharpness. | </summary>
        [Description("Medium Sharpness.")]
        MEDIUM=1, 
    	///<summary> High Sharpness. | </summary>
        [Description("High Sharpness.")]
        HIGH=2, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_PROTUNE_EXPOSURE: int /*default*/
    {
			///<summary> -5.0 EV (Hero 3+ Only). | </summary>
        [Description("-5.0 EV (Hero 3+ Only).")]
        NEG_5_0=0, 
    	///<summary> -4.5 EV (Hero 3+ Only). | </summary>
        [Description("-4.5 EV (Hero 3+ Only).")]
        NEG_4_5=1, 
    	///<summary> -4.0 EV (Hero 3+ Only). | </summary>
        [Description("-4.0 EV (Hero 3+ Only).")]
        NEG_4_0=2, 
    	///<summary> -3.5 EV (Hero 3+ Only). | </summary>
        [Description("-3.5 EV (Hero 3+ Only).")]
        NEG_3_5=3, 
    	///<summary> -3.0 EV (Hero 3+ Only). | </summary>
        [Description("-3.0 EV (Hero 3+ Only).")]
        NEG_3_0=4, 
    	///<summary> -2.5 EV (Hero 3+ Only). | </summary>
        [Description("-2.5 EV (Hero 3+ Only).")]
        NEG_2_5=5, 
    	///<summary> -2.0 EV. | </summary>
        [Description("-2.0 EV.")]
        NEG_2_0=6, 
    	///<summary> -1.5 EV. | </summary>
        [Description("-1.5 EV.")]
        NEG_1_5=7, 
    	///<summary> -1.0 EV. | </summary>
        [Description("-1.0 EV.")]
        NEG_1_0=8, 
    	///<summary> -0.5 EV. | </summary>
        [Description("-0.5 EV.")]
        NEG_0_5=9, 
    	///<summary> 0.0 EV. | </summary>
        [Description("0.0 EV.")]
        ZERO=10, 
    	///<summary> +0.5 EV. | </summary>
        [Description("+0.5 EV.")]
        POS_0_5=11, 
    	///<summary> +1.0 EV. | </summary>
        [Description("+1.0 EV.")]
        POS_1_0=12, 
    	///<summary> +1.5 EV. | </summary>
        [Description("+1.5 EV.")]
        POS_1_5=13, 
    	///<summary> +2.0 EV. | </summary>
        [Description("+2.0 EV.")]
        POS_2_0=14, 
    	///<summary> +2.5 EV (Hero 3+ Only). | </summary>
        [Description("+2.5 EV (Hero 3+ Only).")]
        POS_2_5=15, 
    	///<summary> +3.0 EV (Hero 3+ Only). | </summary>
        [Description("+3.0 EV (Hero 3+ Only).")]
        POS_3_0=16, 
    	///<summary> +3.5 EV (Hero 3+ Only). | </summary>
        [Description("+3.5 EV (Hero 3+ Only).")]
        POS_3_5=17, 
    	///<summary> +4.0 EV (Hero 3+ Only). | </summary>
        [Description("+4.0 EV (Hero 3+ Only).")]
        POS_4_0=18, 
    	///<summary> +4.5 EV (Hero 3+ Only). | </summary>
        [Description("+4.5 EV (Hero 3+ Only).")]
        POS_4_5=19, 
    	///<summary> +5.0 EV (Hero 3+ Only). | </summary>
        [Description("+5.0 EV (Hero 3+ Only).")]
        POS_5_0=20, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_CHARGING: int /*default*/
    {
			///<summary> Charging disabled. | </summary>
        [Description("Charging disabled.")]
        DISABLED=0, 
    	///<summary> Charging enabled. | </summary>
        [Description("Charging enabled.")]
        ENABLED=1, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_MODEL: int /*default*/
    {
			///<summary> Unknown gopro model. | </summary>
        [Description("Unknown gopro model.")]
        UNKNOWN=0, 
    	///<summary> Hero 3+ Silver (HeroBus not supported by GoPro). | </summary>
        [Description("Hero 3+ Silver (HeroBus not supported by GoPro).")]
        HERO_3_PLUS_SILVER=1, 
    	///<summary> Hero 3+ Black. | </summary>
        [Description("Hero 3+ Black.")]
        HERO_3_PLUS_BLACK=2, 
    	///<summary> Hero 4 Silver. | </summary>
        [Description("Hero 4 Silver.")]
        HERO_4_SILVER=3, 
    	///<summary> Hero 4 Black. | </summary>
        [Description("Hero 4 Black.")]
        HERO_4_BLACK=4, 
    
    };
    
    ///<summary>  </summary>
    public enum GOPRO_BURST_RATE: int /*default*/
    {
			///<summary> 3 Shots / 1 Second. | </summary>
        [Description("3 Shots / 1 Second.")]
        _3_IN_1_SECOND=0, 
    	///<summary> 5 Shots / 1 Second. | </summary>
        [Description("5 Shots / 1 Second.")]
        _5_IN_1_SECOND=1, 
    	///<summary> 10 Shots / 1 Second. | </summary>
        [Description("10 Shots / 1 Second.")]
        _10_IN_1_SECOND=2, 
    	///<summary> 10 Shots / 2 Second. | </summary>
        [Description("10 Shots / 2 Second.")]
        _10_IN_2_SECOND=3, 
    	///<summary> 10 Shots / 3 Second (Hero 4 Only). | </summary>
        [Description("10 Shots / 3 Second (Hero 4 Only).")]
        _10_IN_3_SECOND=4, 
    	///<summary> 30 Shots / 1 Second. | </summary>
        [Description("30 Shots / 1 Second.")]
        _30_IN_1_SECOND=5, 
    	///<summary> 30 Shots / 2 Second. | </summary>
        [Description("30 Shots / 2 Second.")]
        _30_IN_2_SECOND=6, 
    	///<summary> 30 Shots / 3 Second. | </summary>
        [Description("30 Shots / 3 Second.")]
        _30_IN_3_SECOND=7, 
    	///<summary> 30 Shots / 6 Second. | </summary>
        [Description("30 Shots / 6 Second.")]
        _30_IN_6_SECOND=8, 
    
    };
    
    ///<summary>  </summary>
    public enum LED_CONTROL_PATTERN: int /*default*/
    {
			///<summary> LED patterns off (return control to regular vehicle control). | </summary>
        [Description("LED patterns off (return control to regular vehicle control).")]
        OFF=0, 
    	///<summary> LEDs show pattern during firmware update. | </summary>
        [Description("LEDs show pattern during firmware update.")]
        FIRMWAREUPDATE=1, 
    	///<summary> Custom Pattern using custom bytes fields. | </summary>
        [Description("Custom Pattern using custom bytes fields.")]
        CUSTOM=255, 
    
    };
    
    ///<summary> Flags in EKF_STATUS message. </summary>
    public enum EKF_STATUS_FLAGS: ushort
    {
			///<summary> Set if EKF's attitude estimate is good. | </summary>
        [Description("Set if EKF's attitude estimate is good.")]
        EKF_ATTITUDE=1, 
    	///<summary> Set if EKF's horizontal velocity estimate is good. | </summary>
        [Description("Set if EKF's horizontal velocity estimate is good.")]
        EKF_VELOCITY_HORIZ=2, 
    	///<summary> Set if EKF's vertical velocity estimate is good. | </summary>
        [Description("Set if EKF's vertical velocity estimate is good.")]
        EKF_VELOCITY_VERT=4, 
    	///<summary> Set if EKF's horizontal position (relative) estimate is good. | </summary>
        [Description("Set if EKF's horizontal position (relative) estimate is good.")]
        EKF_POS_HORIZ_REL=8, 
    	///<summary> Set if EKF's horizontal position (absolute) estimate is good. | </summary>
        [Description("Set if EKF's horizontal position (absolute) estimate is good.")]
        EKF_POS_HORIZ_ABS=16, 
    	///<summary> Set if EKF's vertical position (absolute) estimate is good. | </summary>
        [Description("Set if EKF's vertical position (absolute) estimate is good.")]
        EKF_POS_VERT_ABS=32, 
    	///<summary> Set if EKF's vertical position (above ground) estimate is good. | </summary>
        [Description("Set if EKF's vertical position (above ground) estimate is good.")]
        EKF_POS_VERT_AGL=64, 
    	///<summary> EKF is in constant position mode and does not know it's absolute or relative position. | </summary>
        [Description("EKF is in constant position mode and does not know it's absolute or relative position.")]
        EKF_CONST_POS_MODE=128, 
    	///<summary> Set if EKF's predicted horizontal position (relative) estimate is good. | </summary>
        [Description("Set if EKF's predicted horizontal position (relative) estimate is good.")]
        EKF_PRED_POS_HORIZ_REL=256, 
    	///<summary> Set if EKF's predicted horizontal position (absolute) estimate is good. | </summary>
        [Description("Set if EKF's predicted horizontal position (absolute) estimate is good.")]
        EKF_PRED_POS_HORIZ_ABS=512, 
    
    };
    
    ///<summary>  </summary>
    public enum PID_TUNING_AXIS: byte
    {
			///<summary>  | </summary>
        [Description("")]
        PID_TUNING_ROLL=1, 
    	///<summary>  | </summary>
        [Description("")]
        PID_TUNING_PITCH=2, 
    	///<summary>  | </summary>
        [Description("")]
        PID_TUNING_YAW=3, 
    	///<summary>  | </summary>
        [Description("")]
        PID_TUNING_ACCZ=4, 
    	///<summary>  | </summary>
        [Description("")]
        PID_TUNING_STEER=5, 
    	///<summary>  | </summary>
        [Description("")]
        PID_TUNING_LANDING=6, 
    
    };
    
    ///<summary>  </summary>
    public enum MAG_CAL_STATUS: byte
    {
			///<summary>  | </summary>
        [Description("")]
        MAG_CAL_NOT_STARTED=0, 
    	///<summary>  | </summary>
        [Description("")]
        MAG_CAL_WAITING_TO_START=1, 
    	///<summary>  | </summary>
        [Description("")]
        MAG_CAL_RUNNING_STEP_ONE=2, 
    	///<summary>  | </summary>
        [Description("")]
        MAG_CAL_RUNNING_STEP_TWO=3, 
    	///<summary>  | </summary>
        [Description("")]
        MAG_CAL_SUCCESS=4, 
    	///<summary>  | </summary>
        [Description("")]
        MAG_CAL_FAILED=5, 
    	///<summary>  | </summary>
        [Description("")]
        MAG_CAL_BAD_ORIENTATION=6, 
    
    };
    
    ///<summary> Special ACK block numbers control activation of dataflash log streaming. </summary>
    public enum MAV_REMOTE_LOG_DATA_BLOCK_COMMANDS: uint
    {
			///<summary> UAV to stop sending DataFlash blocks. | </summary>
        [Description("UAV to stop sending DataFlash blocks.")]
        MAV_REMOTE_LOG_DATA_BLOCK_STOP=2147483645, 
    	///<summary> UAV to start sending DataFlash blocks. | </summary>
        [Description("UAV to start sending DataFlash blocks.")]
        MAV_REMOTE_LOG_DATA_BLOCK_START=2147483646, 
    
    };
    
    ///<summary> Possible remote log data block statuses. </summary>
    public enum MAV_REMOTE_LOG_DATA_BLOCK_STATUSES: byte
    {
			///<summary> This block has NOT been received. | </summary>
        [Description("This block has NOT been received.")]
        MAV_REMOTE_LOG_DATA_BLOCK_NACK=0, 
    	///<summary> This block has been received. | </summary>
        [Description("This block has been received.")]
        MAV_REMOTE_LOG_DATA_BLOCK_ACK=1, 
    
    };
    
    ///<summary> Bus types for device operations. </summary>
    public enum DEVICE_OP_BUSTYPE: byte
    {
			///<summary> I2C Device operation. | </summary>
        [Description("I2C Device operation.")]
        I2C=0, 
    	///<summary> SPI Device operation. | </summary>
        [Description("SPI Device operation.")]
        SPI=1, 
    
    };
    
    ///<summary> Deepstall flight stage. </summary>
    public enum DEEPSTALL_STAGE: byte
    {
			///<summary> Flying to the landing point. | </summary>
        [Description("Flying to the landing point.")]
        FLY_TO_LANDING=0, 
    	///<summary> Building an estimate of the wind. | </summary>
        [Description("Building an estimate of the wind.")]
        ESTIMATE_WIND=1, 
    	///<summary> Waiting to breakout of the loiter to fly the approach. | </summary>
        [Description("Waiting to breakout of the loiter to fly the approach.")]
        WAIT_FOR_BREAKOUT=2, 
    	///<summary> Flying to the first arc point to turn around to the landing point. | </summary>
        [Description("Flying to the first arc point to turn around to the landing point.")]
        FLY_TO_ARC=3, 
    	///<summary> Turning around back to the deepstall landing point. | </summary>
        [Description("Turning around back to the deepstall landing point.")]
        ARC=4, 
    	///<summary> Approaching the landing point. | </summary>
        [Description("Approaching the landing point.")]
        APPROACH=5, 
    	///<summary> Stalling and steering towards the land point. | </summary>
        [Description("Stalling and steering towards the land point.")]
        LAND=6, 
    
    };
    
    ///<summary> A mapping of plane flight modes for custom_mode field of heartbeat. </summary>
    public enum PLANE_MODE: int /*default*/
    {
			///<summary>  | </summary>
        [Description("")]
        MANUAL=0, 
    	///<summary>  | </summary>
        [Description("")]
        CIRCLE=1, 
    	///<summary>  | </summary>
        [Description("")]
        STABILIZE=2, 
    	///<summary>  | </summary>
        [Description("")]
        TRAINING=3, 
    	///<summary>  | </summary>
        [Description("")]
        ACRO=4, 
    	///<summary>  | </summary>
        [Description("")]
        FLY_BY_WIRE_A=5, 
    	///<summary>  | </summary>
        [Description("")]
        FLY_BY_WIRE_B=6, 
    	///<summary>  | </summary>
        [Description("")]
        CRUISE=7, 
    	///<summary>  | </summary>
        [Description("")]
        AUTOTUNE=8, 
    	///<summary>  | </summary>
        [Description("")]
        AUTO=10, 
    	///<summary>  | </summary>
        [Description("")]
        RTL=11, 
    	///<summary>  | </summary>
        [Description("")]
        LOITER=12, 
    	///<summary>  | </summary>
        [Description("")]
        AVOID_ADSB=14, 
    	///<summary>  | </summary>
        [Description("")]
        GUIDED=15, 
    	///<summary>  | </summary>
        [Description("")]
        INITIALIZING=16, 
    	///<summary>  | </summary>
        [Description("")]
        QSTABILIZE=17, 
    	///<summary>  | </summary>
        [Description("")]
        QHOVER=18, 
    	///<summary>  | </summary>
        [Description("")]
        QLOITER=19, 
    	///<summary>  | </summary>
        [Description("")]
        QLAND=20, 
    	///<summary>  | </summary>
        [Description("")]
        QRTL=21, 
    	///<summary>  | </summary>
        [Description("")]
        QAUTOTUNE=22, 
    
    };
    
    ///<summary> A mapping of copter flight modes for custom_mode field of heartbeat. </summary>
    public enum COPTER_MODE: int /*default*/
    {
			///<summary>  | </summary>
        [Description("")]
        STABILIZE=0, 
    	///<summary>  | </summary>
        [Description("")]
        ACRO=1, 
    	///<summary>  | </summary>
        [Description("")]
        ALT_HOLD=2, 
    	///<summary>  | </summary>
        [Description("")]
        AUTO=3, 
    	///<summary>  | </summary>
        [Description("")]
        GUIDED=4, 
    	///<summary>  | </summary>
        [Description("")]
        LOITER=5, 
    	///<summary>  | </summary>
        [Description("")]
        RTL=6, 
    	///<summary>  | </summary>
        [Description("")]
        CIRCLE=7, 
    	///<summary>  | </summary>
        [Description("")]
        LAND=9, 
    	///<summary>  | </summary>
        [Description("")]
        DRIFT=11, 
    	///<summary>  | </summary>
        [Description("")]
        SPORT=13, 
    	///<summary>  | </summary>
        [Description("")]
        FLIP=14, 
    	///<summary>  | </summary>
        [Description("")]
        AUTOTUNE=15, 
    	///<summary>  | </summary>
        [Description("")]
        POSHOLD=16, 
    	///<summary>  | </summary>
        [Description("")]
        BRAKE=17, 
    	///<summary>  | </summary>
        [Description("")]
        THROW=18, 
    	///<summary>  | </summary>
        [Description("")]
        AVOID_ADSB=19, 
    	///<summary>  | </summary>
        [Description("")]
        GUIDED_NOGPS=20, 
    	///<summary>  | </summary>
        [Description("")]
        SMART_RTL=21, 
    
    };
    
    ///<summary> A mapping of sub flight modes for custom_mode field of heartbeat. </summary>
    public enum SUB_MODE: int /*default*/
    {
			///<summary>  | </summary>
        [Description("")]
        STABILIZE=0, 
    	///<summary>  | </summary>
        [Description("")]
        ACRO=1, 
    	///<summary>  | </summary>
        [Description("")]
        ALT_HOLD=2, 
    	///<summary>  | </summary>
        [Description("")]
        AUTO=3, 
    	///<summary>  | </summary>
        [Description("")]
        GUIDED=4, 
    	///<summary>  | </summary>
        [Description("")]
        CIRCLE=7, 
    	///<summary>  | </summary>
        [Description("")]
        SURFACE=9, 
    	///<summary>  | </summary>
        [Description("")]
        POSHOLD=16, 
    	///<summary>  | </summary>
        [Description("")]
        MANUAL=19, 
    
    };
    
    ///<summary> A mapping of rover flight modes for custom_mode field of heartbeat. </summary>
    public enum ROVER_MODE: int /*default*/
    {
			///<summary>  | </summary>
        [Description("")]
        MANUAL=0, 
    	///<summary>  | </summary>
        [Description("")]
        ACRO=1, 
    	///<summary>  | </summary>
        [Description("")]
        STEERING=3, 
    	///<summary>  | </summary>
        [Description("")]
        HOLD=4, 
    	///<summary>  | </summary>
        [Description("")]
        LOITER=5, 
    	///<summary>  | </summary>
        [Description("")]
        AUTO=10, 
    	///<summary>  | </summary>
        [Description("")]
        RTL=11, 
    	///<summary>  | </summary>
        [Description("")]
        SMART_RTL=12, 
    	///<summary>  | </summary>
        [Description("")]
        GUIDED=15, 
    	///<summary>  | </summary>
        [Description("")]
        INITIALIZING=16, 
    
    };
    
    ///<summary> A mapping of antenna tracker flight modes for custom_mode field of heartbeat. </summary>
    public enum TRACKER_MODE: int /*default*/
    {
			///<summary>  | </summary>
        [Description("")]
        MANUAL=0, 
    	///<summary>  | </summary>
        [Description("")]
        STOP=1, 
    	///<summary>  | </summary>
        [Description("")]
        SCAN=2, 
    	///<summary>  | </summary>
        [Description("")]
        SERVO_TEST=3, 
    	///<summary>  | </summary>
        [Description("")]
        AUTO=10, 
    	///<summary>  | </summary>
        [Description("")]
        INITIALIZING=16, 
    
    };
    
    
    ///<summary> Micro air vehicle / autopilot classes. This identifies the individual model. </summary>
    public enum MAV_AUTOPILOT: byte
    {
			///<summary> Generic autopilot, full support for everything | </summary>
        [Description("Generic autopilot, full support for everything")]
        GENERIC=0, 
    	///<summary> Reserved for future use. | </summary>
        [Description("Reserved for future use.")]
        RESERVED=1, 
    	///<summary> SLUGS autopilot, http://slugsuav.soe.ucsc.edu | </summary>
        [Description("SLUGS autopilot, http://slugsuav.soe.ucsc.edu")]
        SLUGS=2, 
    	///<summary> ArduPilot - Plane/Copter/Rover/Sub/Tracker, http://ardupilot.org | </summary>
        [Description("ArduPilot - Plane/Copter/Rover/Sub/Tracker, http://ardupilot.org")]
        ARDUPILOTMEGA=3, 
    	///<summary> OpenPilot, http://openpilot.org | </summary>
        [Description("OpenPilot, http://openpilot.org")]
        OPENPILOT=4, 
    	///<summary> Generic autopilot only supporting simple waypoints | </summary>
        [Description("Generic autopilot only supporting simple waypoints")]
        GENERIC_WAYPOINTS_ONLY=5, 
    	///<summary> Generic autopilot supporting waypoints and other simple navigation commands | </summary>
        [Description("Generic autopilot supporting waypoints and other simple navigation commands")]
        GENERIC_WAYPOINTS_AND_SIMPLE_NAVIGATION_ONLY=6, 
    	///<summary> Generic autopilot supporting the full mission command set | </summary>
        [Description("Generic autopilot supporting the full mission command set")]
        GENERIC_MISSION_FULL=7, 
    	///<summary> No valid autopilot, e.g. a GCS or other MAVLink component | </summary>
        [Description("No valid autopilot, e.g. a GCS or other MAVLink component")]
        INVALID=8, 
    	///<summary> PPZ UAV - http://nongnu.org/paparazzi | </summary>
        [Description("PPZ UAV - http://nongnu.org/paparazzi")]
        PPZ=9, 
    	///<summary> UAV Dev Board | </summary>
        [Description("UAV Dev Board")]
        UDB=10, 
    	///<summary> FlexiPilot | </summary>
        [Description("FlexiPilot")]
        FP=11, 
    	///<summary> PX4 Autopilot - http://px4.io/ | </summary>
        [Description("PX4 Autopilot - http://px4.io/")]
        PX4=12, 
    	///<summary> SMACCMPilot - http://smaccmpilot.org | </summary>
        [Description("SMACCMPilot - http://smaccmpilot.org")]
        SMACCMPILOT=13, 
    	///<summary> AutoQuad -- http://autoquad.org | </summary>
        [Description("AutoQuad -- http://autoquad.org")]
        AUTOQUAD=14, 
    	///<summary> Armazila -- http://armazila.com | </summary>
        [Description("Armazila -- http://armazila.com")]
        ARMAZILA=15, 
    	///<summary> Aerob -- http://aerob.ru | </summary>
        [Description("Aerob -- http://aerob.ru")]
        AEROB=16, 
    	///<summary> ASLUAV autopilot -- http://www.asl.ethz.ch | </summary>
        [Description("ASLUAV autopilot -- http://www.asl.ethz.ch")]
        ASLUAV=17, 
    	///<summary> SmartAP Autopilot - http://sky-drones.com | </summary>
        [Description("SmartAP Autopilot - http://sky-drones.com")]
        SMARTAP=18, 
    	///<summary> AirRails - http://uaventure.com | </summary>
        [Description("AirRails - http://uaventure.com")]
        AIRRAILS=19, 
    
    };
    
    ///<summary> MAVLINK system type. All components in a system should report this type in their HEARTBEAT. </summary>
    public enum MAV_TYPE: byte
    {
			///<summary> Generic micro air vehicle. | </summary>
        [Description("Generic micro air vehicle.")]
        GENERIC=0, 
    	///<summary> Fixed wing aircraft. | </summary>
        [Description("Fixed wing aircraft.")]
        FIXED_WING=1, 
    	///<summary> Quadrotor | </summary>
        [Description("Quadrotor")]
        QUADROTOR=2, 
    	///<summary> Coaxial helicopter | </summary>
        [Description("Coaxial helicopter")]
        COAXIAL=3, 
    	///<summary> Normal helicopter with tail rotor. | </summary>
        [Description("Normal helicopter with tail rotor.")]
        HELICOPTER=4, 
    	///<summary> Ground installation | </summary>
        [Description("Ground installation")]
        ANTENNA_TRACKER=5, 
    	///<summary> Operator control unit / ground control station | </summary>
        [Description("Operator control unit / ground control station")]
        GCS=6, 
    	///<summary> Airship, controlled | </summary>
        [Description("Airship, controlled")]
        AIRSHIP=7, 
    	///<summary> Free balloon, uncontrolled | </summary>
        [Description("Free balloon, uncontrolled")]
        FREE_BALLOON=8, 
    	///<summary> Rocket | </summary>
        [Description("Rocket")]
        ROCKET=9, 
    	///<summary> Ground rover | </summary>
        [Description("Ground rover")]
        GROUND_ROVER=10, 
    	///<summary> Surface vessel, boat, ship | </summary>
        [Description("Surface vessel, boat, ship")]
        SURFACE_BOAT=11, 
    	///<summary> Submarine | </summary>
        [Description("Submarine")]
        SUBMARINE=12, 
    	///<summary> Hexarotor | </summary>
        [Description("Hexarotor")]
        HEXAROTOR=13, 
    	///<summary> Octorotor | </summary>
        [Description("Octorotor")]
        OCTOROTOR=14, 
    	///<summary> Tricopter | </summary>
        [Description("Tricopter")]
        TRICOPTER=15, 
    	///<summary> Flapping wing | </summary>
        [Description("Flapping wing")]
        FLAPPING_WING=16, 
    	///<summary> Kite | </summary>
        [Description("Kite")]
        KITE=17, 
    	///<summary> Onboard companion controller | </summary>
        [Description("Onboard companion controller")]
        ONBOARD_CONTROLLER=18, 
    	///<summary> Two-rotor VTOL using control surfaces in vertical operation in addition. Tailsitter. | </summary>
        [Description("Two-rotor VTOL using control surfaces in vertical operation in addition. Tailsitter.")]
        VTOL_DUOROTOR=19, 
    	///<summary> Quad-rotor VTOL using a V-shaped quad config in vertical operation. Tailsitter. | </summary>
        [Description("Quad-rotor VTOL using a V-shaped quad config in vertical operation. Tailsitter.")]
        VTOL_QUADROTOR=20, 
    	///<summary> Tiltrotor VTOL | </summary>
        [Description("Tiltrotor VTOL")]
        VTOL_TILTROTOR=21, 
    	///<summary> VTOL reserved 2 | </summary>
        [Description("VTOL reserved 2")]
        VTOL_RESERVED2=22, 
    	///<summary> VTOL reserved 3 | </summary>
        [Description("VTOL reserved 3")]
        VTOL_RESERVED3=23, 
    	///<summary> VTOL reserved 4 | </summary>
        [Description("VTOL reserved 4")]
        VTOL_RESERVED4=24, 
    	///<summary> VTOL reserved 5 | </summary>
        [Description("VTOL reserved 5")]
        VTOL_RESERVED5=25, 
    	///<summary> Gimbal (standalone) | </summary>
        [Description("Gimbal (standalone)")]
        GIMBAL=26, 
    	///<summary> ADSB system (standalone) | </summary>
        [Description("ADSB system (standalone)")]
        ADSB=27, 
    	///<summary> Steerable, nonrigid airfoil | </summary>
        [Description("Steerable, nonrigid airfoil")]
        PARAFOIL=28, 
    	///<summary> Dodecarotor | </summary>
        [Description("Dodecarotor")]
        DODECAROTOR=29, 
    	///<summary> Camera (standalone) | </summary>
        [Description("Camera (standalone)")]
        CAMERA=30, 
    	///<summary> Charging station | </summary>
        [Description("Charging station")]
        CHARGING_STATION=31, 
    	///<summary> FLARM collision avoidance system (standalone) | </summary>
        [Description("FLARM collision avoidance system (standalone)")]
        FLARM=32, 
    
    };
    
    ///<summary> These values define the type of firmware release.  These values indicate the first version or release of this type.  For example the first alpha release would be 64, the second would be 65. </summary>
    public enum FIRMWARE_VERSION_TYPE: int /*default*/
    {
			///<summary> development release | </summary>
        [Description("development release")]
        DEV=0, 
    	///<summary> alpha release | </summary>
        [Description("alpha release")]
        ALPHA=64, 
    	///<summary> beta release | </summary>
        [Description("beta release")]
        BETA=128, 
    	///<summary> release candidate | </summary>
        [Description("release candidate")]
        RC=192, 
    	///<summary> official stable release | </summary>
        [Description("official stable release")]
        OFFICIAL=255, 
    
    };
    
    ///<summary> These flags encode the MAV mode. </summary>
    public enum MAV_MODE_FLAG: byte
    {
			///<summary> 0b00000001 Reserved for future use. | </summary>
        [Description("0b00000001 Reserved for future use.")]
        CUSTOM_MODE_ENABLED=1, 
    	///<summary> 0b00000010 system has a test mode enabled. This flag is intended for temporary system tests and should not be used for stable implementations. | </summary>
        [Description("0b00000010 system has a test mode enabled. This flag is intended for temporary system tests and should not be used for stable implementations.")]
        TEST_ENABLED=2, 
    	///<summary> 0b00000100 autonomous mode enabled, system finds its own goal positions. Guided flag can be set or not, depends on the actual implementation. | </summary>
        [Description("0b00000100 autonomous mode enabled, system finds its own goal positions. Guided flag can be set or not, depends on the actual implementation.")]
        AUTO_ENABLED=4, 
    	///<summary> 0b00001000 guided mode enabled, system flies waypoints / mission items. | </summary>
        [Description("0b00001000 guided mode enabled, system flies waypoints / mission items.")]
        GUIDED_ENABLED=8, 
    	///<summary> 0b00010000 system stabilizes electronically its attitude (and optionally position). It needs however further control inputs to move around. | </summary>
        [Description("0b00010000 system stabilizes electronically its attitude (and optionally position). It needs however further control inputs to move around.")]
        STABILIZE_ENABLED=16, 
    	///<summary> 0b00100000 hardware in the loop simulation. All motors / actuators are blocked, but internal software is full operational. | </summary>
        [Description("0b00100000 hardware in the loop simulation. All motors / actuators are blocked, but internal software is full operational.")]
        HIL_ENABLED=32, 
    	///<summary> 0b01000000 remote control input is enabled. | </summary>
        [Description("0b01000000 remote control input is enabled.")]
        MANUAL_INPUT_ENABLED=64, 
    	///<summary> 0b10000000 MAV safety set to armed. Motors are enabled / running / can start. Ready to fly. Additional note: this flag is to be ignore when sent in the command MAV_CMD_DO_SET_MODE and MAV_CMD_COMPONENT_ARM_DISARM shall be used instead. The flag can still be used to report the armed state. | </summary>
        [Description("0b10000000 MAV safety set to armed. Motors are enabled / running / can start. Ready to fly. Additional note: this flag is to be ignore when sent in the command MAV_CMD_DO_SET_MODE and MAV_CMD_COMPONENT_ARM_DISARM shall be used instead. The flag can still be used to report the armed state.")]
        SAFETY_ARMED=128, 
    
    };
    
    ///<summary> These values encode the bit positions of the decode position. These values can be used to read the value of a flag bit by combining the base_mode variable with AND with the flag position value. The result will be either 0 or 1, depending on if the flag is set or not. </summary>
    public enum MAV_MODE_FLAG_DECODE_POSITION: int /*default*/
    {
			///<summary> Eighth bit: 00000001 | </summary>
        [Description("Eighth bit: 00000001")]
        CUSTOM_MODE=1, 
    	///<summary> Seventh bit: 00000010 | </summary>
        [Description("Seventh bit: 00000010")]
        TEST=2, 
    	///<summary> Sixt bit:   00000100 | </summary>
        [Description("Sixt bit:   00000100")]
        AUTO=4, 
    	///<summary> Fifth bit:  00001000 | </summary>
        [Description("Fifth bit:  00001000")]
        GUIDED=8, 
    	///<summary> Fourth bit: 00010000 | </summary>
        [Description("Fourth bit: 00010000")]
        STABILIZE=16, 
    	///<summary> Third bit:  00100000 | </summary>
        [Description("Third bit:  00100000")]
        HIL=32, 
    	///<summary> Second bit: 01000000 | </summary>
        [Description("Second bit: 01000000")]
        MANUAL=64, 
    	///<summary> First bit:  10000000 | </summary>
        [Description("First bit:  10000000")]
        SAFETY=128, 
    
    };
    
    ///<summary> Override command, pauses current mission execution and moves immediately to a position </summary>
    public enum MAV_GOTO: int /*default*/
    {
			///<summary> Hold at the current position. | </summary>
        [Description("Hold at the current position.")]
        DO_HOLD=0, 
    	///<summary> Continue with the next item in mission execution. | </summary>
        [Description("Continue with the next item in mission execution.")]
        DO_CONTINUE=1, 
    	///<summary> Hold at the current position of the system | </summary>
        [Description("Hold at the current position of the system")]
        HOLD_AT_CURRENT_POSITION=2, 
    	///<summary> Hold at the position specified in the parameters of the DO_HOLD action | </summary>
        [Description("Hold at the position specified in the parameters of the DO_HOLD action")]
        HOLD_AT_SPECIFIED_POSITION=3, 
    
    };
    
    ///<summary> These defines are predefined OR-combined mode flags. There is no need to use values from this enum, but it    
///               simplifies the use of the mode flags. Note that manual input is enabled in all modes as a safety override. </summary>
    public enum MAV_MODE: byte
    {
			///<summary> System is not ready to fly, booting, calibrating, etc. No flag is set. | </summary>
        [Description("System is not ready to fly, booting, calibrating, etc. No flag is set.")]
        PREFLIGHT=0, 
    	///<summary> System is allowed to be active, under manual (RC) control, no stabilization | </summary>
        [Description("System is allowed to be active, under manual (RC) control, no stabilization")]
        MANUAL_DISARMED=64, 
    	///<summary> UNDEFINED mode. This solely depends on the autopilot - use with caution, intended for developers only. | </summary>
        [Description("UNDEFINED mode. This solely depends on the autopilot - use with caution, intended for developers only.")]
        TEST_DISARMED=66, 
    	///<summary> System is allowed to be active, under assisted RC control. | </summary>
        [Description("System is allowed to be active, under assisted RC control.")]
        STABILIZE_DISARMED=80, 
    	///<summary> System is allowed to be active, under autonomous control, manual setpoint | </summary>
        [Description("System is allowed to be active, under autonomous control, manual setpoint")]
        GUIDED_DISARMED=88, 
    	///<summary> System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by waypoints) | </summary>
        [Description("System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by waypoints)")]
        AUTO_DISARMED=92, 
    	///<summary> System is allowed to be active, under manual (RC) control, no stabilization | </summary>
        [Description("System is allowed to be active, under manual (RC) control, no stabilization")]
        MANUAL_ARMED=192, 
    	///<summary> UNDEFINED mode. This solely depends on the autopilot - use with caution, intended for developers only. | </summary>
        [Description("UNDEFINED mode. This solely depends on the autopilot - use with caution, intended for developers only.")]
        TEST_ARMED=194, 
    	///<summary> System is allowed to be active, under assisted RC control. | </summary>
        [Description("System is allowed to be active, under assisted RC control.")]
        STABILIZE_ARMED=208, 
    	///<summary> System is allowed to be active, under autonomous control, manual setpoint | </summary>
        [Description("System is allowed to be active, under autonomous control, manual setpoint")]
        GUIDED_ARMED=216, 
    	///<summary> System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by waypoints) | </summary>
        [Description("System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by waypoints)")]
        AUTO_ARMED=220, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_STATE: byte
    {
			///<summary> Uninitialized system, state is unknown. | </summary>
        [Description("Uninitialized system, state is unknown.")]
        UNINIT=0, 
    	///<summary> System is booting up. | </summary>
        [Description("System is booting up.")]
        BOOT=1, 
    	///<summary> System is calibrating and not flight-ready. | </summary>
        [Description("System is calibrating and not flight-ready.")]
        CALIBRATING=2, 
    	///<summary> System is grounded and on standby. It can be launched any time. | </summary>
        [Description("System is grounded and on standby. It can be launched any time.")]
        STANDBY=3, 
    	///<summary> System is active and might be already airborne. Motors are engaged. | </summary>
        [Description("System is active and might be already airborne. Motors are engaged.")]
        ACTIVE=4, 
    	///<summary> System is in a non-normal flight mode. It can however still navigate. | </summary>
        [Description("System is in a non-normal flight mode. It can however still navigate.")]
        CRITICAL=5, 
    	///<summary> System is in a non-normal flight mode. It lost control over parts or over the whole airframe. It is in mayday and going down. | </summary>
        [Description("System is in a non-normal flight mode. It lost control over parts or over the whole airframe. It is in mayday and going down.")]
        EMERGENCY=6, 
    	///<summary> System just initialized its power-down sequence, will shut down now. | </summary>
        [Description("System just initialized its power-down sequence, will shut down now.")]
        POWEROFF=7, 
    	///<summary> System is terminating itself. | </summary>
        [Description("System is terminating itself.")]
        FLIGHT_TERMINATION=8, 
    
    };
    
    ///<summary> Component ids (values) for the different types and instances of onboard hardware/software that might make up a MAVLink system (autopilot, cameras, servos, GPS systems, avoidance systems etc.).    
///      Components must use the appropriate ID in their source address when sending messages. Components can also use IDs to determine if they are the intended recipient of an incoming message. The MAV_COMP_ID_ALL value is used to indicate messages that must be processed by all components.    
///      When creating new entries, components that can have multiple instances (e.g. cameras, servos etc.) should be allocated sequential values. An appropriate number of values should be left free after these components to allow the number of instances to be expanded. </summary>
    public enum MAV_COMPONENT: int /*default*/
    {
			///<summary> Used to broadcast messages to all components of the receiving system. Components should attempt to process messages with this component ID and forward to components on any other interfaces. | </summary>
        [Description("Used to broadcast messages to all components of the receiving system. Components should attempt to process messages with this component ID and forward to components on any other interfaces.")]
        MAV_COMP_ID_ALL=0, 
    	///<summary> System flight controller component ('autopilot'). Only one autopilot is expected in a particular system. | </summary>
        [Description("System flight controller component ('autopilot'). Only one autopilot is expected in a particular system.")]
        MAV_COMP_ID_AUTOPILOT1=1, 
    	///<summary> Camera #1. | </summary>
        [Description("Camera #1.")]
        MAV_COMP_ID_CAMERA=100, 
    	///<summary> Camera #2. | </summary>
        [Description("Camera #2.")]
        MAV_COMP_ID_CAMERA2=101, 
    	///<summary> Camera #3. | </summary>
        [Description("Camera #3.")]
        MAV_COMP_ID_CAMERA3=102, 
    	///<summary> Camera #4. | </summary>
        [Description("Camera #4.")]
        MAV_COMP_ID_CAMERA4=103, 
    	///<summary> Camera #5. | </summary>
        [Description("Camera #5.")]
        MAV_COMP_ID_CAMERA5=104, 
    	///<summary> Camera #6. | </summary>
        [Description("Camera #6.")]
        MAV_COMP_ID_CAMERA6=105, 
    	///<summary> Servo #1. | </summary>
        [Description("Servo #1.")]
        MAV_COMP_ID_SERVO1=140, 
    	///<summary> Servo #2. | </summary>
        [Description("Servo #2.")]
        MAV_COMP_ID_SERVO2=141, 
    	///<summary> Servo #3. | </summary>
        [Description("Servo #3.")]
        MAV_COMP_ID_SERVO3=142, 
    	///<summary> Servo #4. | </summary>
        [Description("Servo #4.")]
        MAV_COMP_ID_SERVO4=143, 
    	///<summary> Servo #5. | </summary>
        [Description("Servo #5.")]
        MAV_COMP_ID_SERVO5=144, 
    	///<summary> Servo #6. | </summary>
        [Description("Servo #6.")]
        MAV_COMP_ID_SERVO6=145, 
    	///<summary> Servo #7. | </summary>
        [Description("Servo #7.")]
        MAV_COMP_ID_SERVO7=146, 
    	///<summary> Servo #8. | </summary>
        [Description("Servo #8.")]
        MAV_COMP_ID_SERVO8=147, 
    	///<summary> Servo #9. | </summary>
        [Description("Servo #9.")]
        MAV_COMP_ID_SERVO9=148, 
    	///<summary> Servo #10. | </summary>
        [Description("Servo #10.")]
        MAV_COMP_ID_SERVO10=149, 
    	///<summary> Servo #11. | </summary>
        [Description("Servo #11.")]
        MAV_COMP_ID_SERVO11=150, 
    	///<summary> Servo #12. | </summary>
        [Description("Servo #12.")]
        MAV_COMP_ID_SERVO12=151, 
    	///<summary> Servo #13. | </summary>
        [Description("Servo #13.")]
        MAV_COMP_ID_SERVO13=152, 
    	///<summary> Servo #14. | </summary>
        [Description("Servo #14.")]
        MAV_COMP_ID_SERVO14=153, 
    	///<summary> Gimbal component. | </summary>
        [Description("Gimbal component.")]
        MAV_COMP_ID_GIMBAL=154, 
    	///<summary> Logging component. | </summary>
        [Description("Logging component.")]
        MAV_COMP_ID_LOG=155, 
    	///<summary> Automatic Dependent Surveillance-Broadcast (ADS-B) component. | </summary>
        [Description("Automatic Dependent Surveillance-Broadcast (ADS-B) component.")]
        MAV_COMP_ID_ADSB=156, 
    	///<summary> On Screen Display (OSD) devices for video links. | </summary>
        [Description("On Screen Display (OSD) devices for video links.")]
        MAV_COMP_ID_OSD=157, 
    	///<summary> Generic autopilot peripheral component ID. Meant for devices that do not implement the parameter microservice. | </summary>
        [Description("Generic autopilot peripheral component ID. Meant for devices that do not implement the parameter microservice.")]
        MAV_COMP_ID_PERIPHERAL=158, 
    	///<summary> Gimbal ID for QX1. | </summary>
        [Description("Gimbal ID for QX1.")]
        MAV_COMP_ID_QX1_GIMBAL=159, 
    	///<summary> FLARM collision alert component. | </summary>
        [Description("FLARM collision alert component.")]
        MAV_COMP_ID_FLARM=160, 
    	///<summary> Component that can generate/supply a mission flight plan (e.g. GCS or developer API). | </summary>
        [Description("Component that can generate/supply a mission flight plan (e.g. GCS or developer API).")]
        MAV_COMP_ID_MISSIONPLANNER=190, 
    	///<summary> Component that finds an optimal path between points based on a certain constraint (e.g. minimum snap, shortest path, cost, etc.). | </summary>
        [Description("Component that finds an optimal path between points based on a certain constraint (e.g. minimum snap, shortest path, cost, etc.).")]
        MAV_COMP_ID_PATHPLANNER=195, 
    	///<summary> Component that plans a collision free path between two points. | </summary>
        [Description("Component that plans a collision free path between two points.")]
        MAV_COMP_ID_OBSTACLE_AVOIDANCE=196, 
    	///<summary> Component that provides position estimates using VIO techniques. | </summary>
        [Description("Component that provides position estimates using VIO techniques.")]
        MAV_COMP_ID_VISUAL_INERTIAL_ODOMETRY=197, 
    	///<summary> Inertial Measurement Unit (IMU) #1. | </summary>
        [Description("Inertial Measurement Unit (IMU) #1.")]
        MAV_COMP_ID_IMU=200, 
    	///<summary> Inertial Measurement Unit (IMU) #2. | </summary>
        [Description("Inertial Measurement Unit (IMU) #2.")]
        MAV_COMP_ID_IMU_2=201, 
    	///<summary> Inertial Measurement Unit (IMU) #3. | </summary>
        [Description("Inertial Measurement Unit (IMU) #3.")]
        MAV_COMP_ID_IMU_3=202, 
    	///<summary> GPS #1. | </summary>
        [Description("GPS #1.")]
        MAV_COMP_ID_GPS=220, 
    	///<summary> GPS #2. | </summary>
        [Description("GPS #2.")]
        MAV_COMP_ID_GPS2=221, 
    	///<summary> Component to bridge MAVLink to UDP (i.e. from a UART). | </summary>
        [Description("Component to bridge MAVLink to UDP (i.e. from a UART).")]
        MAV_COMP_ID_UDP_BRIDGE=240, 
    	///<summary> Component to bridge to UART (i.e. from UDP). | </summary>
        [Description("Component to bridge to UART (i.e. from UDP).")]
        MAV_COMP_ID_UART_BRIDGE=241, 
    	///<summary> Component for handling system messages (e.g. to ARM, takeoff, etc.). | </summary>
        [Description("Component for handling system messages (e.g. to ARM, takeoff, etc.).")]
        MAV_COMP_ID_SYSTEM_CONTROL=250, 
    
    };
    
    ///<summary> These encode the sensors whose status is sent as part of the SYS_STATUS message. </summary>
    public enum MAV_SYS_STATUS_SENSOR: uint
    {
			///<summary> 0x01 3D gyro | </summary>
        [Description("0x01 3D gyro")]
        _3D_GYRO=1, 
    	///<summary> 0x02 3D accelerometer | </summary>
        [Description("0x02 3D accelerometer")]
        _3D_ACCEL=2, 
    	///<summary> 0x04 3D magnetometer | </summary>
        [Description("0x04 3D magnetometer")]
        _3D_MAG=4, 
    	///<summary> 0x08 absolute pressure | </summary>
        [Description("0x08 absolute pressure")]
        ABSOLUTE_PRESSURE=8, 
    	///<summary> 0x10 differential pressure | </summary>
        [Description("0x10 differential pressure")]
        DIFFERENTIAL_PRESSURE=16, 
    	///<summary> 0x20 GPS | </summary>
        [Description("0x20 GPS")]
        GPS=32, 
    	///<summary> 0x40 optical flow | </summary>
        [Description("0x40 optical flow")]
        OPTICAL_FLOW=64, 
    	///<summary> 0x80 computer vision position | </summary>
        [Description("0x80 computer vision position")]
        VISION_POSITION=128, 
    	///<summary> 0x100 laser based position | </summary>
        [Description("0x100 laser based position")]
        LASER_POSITION=256, 
    	///<summary> 0x200 external ground truth (Vicon or Leica) | </summary>
        [Description("0x200 external ground truth (Vicon or Leica)")]
        EXTERNAL_GROUND_TRUTH=512, 
    	///<summary> 0x400 3D angular rate control | </summary>
        [Description("0x400 3D angular rate control")]
        ANGULAR_RATE_CONTROL=1024, 
    	///<summary> 0x800 attitude stabilization | </summary>
        [Description("0x800 attitude stabilization")]
        ATTITUDE_STABILIZATION=2048, 
    	///<summary> 0x1000 yaw position | </summary>
        [Description("0x1000 yaw position")]
        YAW_POSITION=4096, 
    	///<summary> 0x2000 z/altitude control | </summary>
        [Description("0x2000 z/altitude control")]
        Z_ALTITUDE_CONTROL=8192, 
    	///<summary> 0x4000 x/y position control | </summary>
        [Description("0x4000 x/y position control")]
        XY_POSITION_CONTROL=16384, 
    	///<summary> 0x8000 motor outputs / control | </summary>
        [Description("0x8000 motor outputs / control")]
        MOTOR_OUTPUTS=32768, 
    	///<summary> 0x10000 rc receiver | </summary>
        [Description("0x10000 rc receiver")]
        RC_RECEIVER=65536, 
    	///<summary> 0x20000 2nd 3D gyro | </summary>
        [Description("0x20000 2nd 3D gyro")]
        _3D_GYRO2=131072, 
    	///<summary> 0x40000 2nd 3D accelerometer | </summary>
        [Description("0x40000 2nd 3D accelerometer")]
        _3D_ACCEL2=262144, 
    	///<summary> 0x80000 2nd 3D magnetometer | </summary>
        [Description("0x80000 2nd 3D magnetometer")]
        _3D_MAG2=524288, 
    	///<summary> 0x100000 geofence | </summary>
        [Description("0x100000 geofence")]
        MAV_SYS_STATUS_GEOFENCE=1048576, 
    	///<summary> 0x200000 AHRS subsystem health | </summary>
        [Description("0x200000 AHRS subsystem health")]
        MAV_SYS_STATUS_AHRS=2097152, 
    	///<summary> 0x400000 Terrain subsystem health | </summary>
        [Description("0x400000 Terrain subsystem health")]
        MAV_SYS_STATUS_TERRAIN=4194304, 
    	///<summary> 0x800000 Motors are reversed | </summary>
        [Description("0x800000 Motors are reversed")]
        MAV_SYS_STATUS_REVERSE_MOTOR=8388608, 
    	///<summary> 0x1000000 Logging | </summary>
        [Description("0x1000000 Logging")]
        MAV_SYS_STATUS_LOGGING=16777216, 
    	///<summary> 0x2000000 Battery | </summary>
        [Description("0x2000000 Battery")]
        BATTERY=33554432, 
    	///<summary> 0x4000000 Proximity | </summary>
        [Description("0x4000000 Proximity")]
        PROXIMITY=67108864, 
    	///<summary> 0x8000000 Satellite Communication  | </summary>
        [Description("0x8000000 Satellite Communication ")]
        SATCOM=134217728, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_FRAME: byte
    {
			///<summary> Global (WGS84) coordinate frame + MSL altitude. First value / x: latitude, second value / y: longitude, third value / z: positive altitude over mean sea level (MSL). | </summary>
        [Description("Global (WGS84) coordinate frame + MSL altitude. First value / x: latitude, second value / y: longitude, third value / z: positive altitude over mean sea level (MSL).")]
        GLOBAL=0, 
    	///<summary> Local coordinate frame, Z-down (x: north, y: east, z: down). | </summary>
        [Description("Local coordinate frame, Z-down (x: north, y: east, z: down).")]
        LOCAL_NED=1, 
    	///<summary> NOT a coordinate frame, indicates a mission command. | </summary>
        [Description("NOT a coordinate frame, indicates a mission command.")]
        MISSION=2, 
    	///<summary> Global (WGS84) coordinate frame + altitude relative to the home position. First value / x: latitude, second value / y: longitude, third value / z: positive altitude with 0 being at the altitude of the home location. | </summary>
        [Description("Global (WGS84) coordinate frame + altitude relative to the home position. First value / x: latitude, second value / y: longitude, third value / z: positive altitude with 0 being at the altitude of the home location.")]
        GLOBAL_RELATIVE_ALT=3, 
    	///<summary> Local coordinate frame, Z-up (x: east, y: north, z: up). | </summary>
        [Description("Local coordinate frame, Z-up (x: east, y: north, z: up).")]
        LOCAL_ENU=4, 
    	///<summary> Global (WGS84) coordinate frame (scaled) + MSL altitude. First value / x: latitude in degrees*1.0e-7, second value / y: longitude in degrees*1.0e-7, third value / z: positive altitude over mean sea level (MSL). | </summary>
        [Description("Global (WGS84) coordinate frame (scaled) + MSL altitude. First value / x: latitude in degrees*1.0e-7, second value / y: longitude in degrees*1.0e-7, third value / z: positive altitude over mean sea level (MSL).")]
        GLOBAL_INT=5, 
    	///<summary> Global (WGS84) coordinate frame (scaled) + altitude relative to the home position. First value / x: latitude in degrees*10e-7, second value / y: longitude in degrees*10e-7, third value / z: positive altitude with 0 being at the altitude of the home location. | </summary>
        [Description("Global (WGS84) coordinate frame (scaled) + altitude relative to the home position. First value / x: latitude in degrees*10e-7, second value / y: longitude in degrees*10e-7, third value / z: positive altitude with 0 being at the altitude of the home location.")]
        GLOBAL_RELATIVE_ALT_INT=6, 
    	///<summary> Offset to the current local frame. Anything expressed in this frame should be added to the current local frame position. | </summary>
        [Description("Offset to the current local frame. Anything expressed in this frame should be added to the current local frame position.")]
        LOCAL_OFFSET_NED=7, 
    	///<summary> Setpoint in body NED frame. This makes sense if all position control is externalized - e.g. useful to command 2 m/s^2 acceleration to the right. | </summary>
        [Description("Setpoint in body NED frame. This makes sense if all position control is externalized - e.g. useful to command 2 m/s^2 acceleration to the right.")]
        BODY_NED=8, 
    	///<summary> Offset in body NED frame. This makes sense if adding setpoints to the current flight path, to avoid an obstacle - e.g. useful to command 2 m/s^2 acceleration to the east. | </summary>
        [Description("Offset in body NED frame. This makes sense if adding setpoints to the current flight path, to avoid an obstacle - e.g. useful to command 2 m/s^2 acceleration to the east.")]
        BODY_OFFSET_NED=9, 
    	///<summary> Global (WGS84) coordinate frame with AGL altitude (at the waypoint coordinate). First value / x: latitude in degrees, second value / y: longitude in degrees, third value / z: positive altitude in meters with 0 being at ground level in terrain model. | </summary>
        [Description("Global (WGS84) coordinate frame with AGL altitude (at the waypoint coordinate). First value / x: latitude in degrees, second value / y: longitude in degrees, third value / z: positive altitude in meters with 0 being at ground level in terrain model.")]
        GLOBAL_TERRAIN_ALT=10, 
    	///<summary> Global (WGS84) coordinate frame (scaled) with AGL altitude (at the waypoint coordinate). First value / x: latitude in degrees*10e-7, second value / y: longitude in degrees*10e-7, third value / z: positive altitude in meters with 0 being at ground level in terrain model. | </summary>
        [Description("Global (WGS84) coordinate frame (scaled) with AGL altitude (at the waypoint coordinate). First value / x: latitude in degrees*10e-7, second value / y: longitude in degrees*10e-7, third value / z: positive altitude in meters with 0 being at ground level in terrain model.")]
        GLOBAL_TERRAIN_ALT_INT=11, 
    	///<summary> Body fixed frame of reference, Z-down (x: forward, y: right, z: down). | </summary>
        [Description("Body fixed frame of reference, Z-down (x: forward, y: right, z: down).")]
        BODY_FRD=12, 
    	///<summary> Body fixed frame of reference, Z-up (x: forward, y: left, z: up). | </summary>
        [Description("Body fixed frame of reference, Z-up (x: forward, y: left, z: up).")]
        BODY_FLU=13, 
    	///<summary> Odometry local coordinate frame of data given by a motion capture system, Z-down (x: north, y: east, z: down). | </summary>
        [Description("Odometry local coordinate frame of data given by a motion capture system, Z-down (x: north, y: east, z: down).")]
        MOCAP_NED=14, 
    	///<summary> Odometry local coordinate frame of data given by a motion capture system, Z-up (x: east, y: north, z: up). | </summary>
        [Description("Odometry local coordinate frame of data given by a motion capture system, Z-up (x: east, y: north, z: up).")]
        MOCAP_ENU=15, 
    	///<summary> Odometry local coordinate frame of data given by a vision estimation system, Z-down (x: north, y: east, z: down). | </summary>
        [Description("Odometry local coordinate frame of data given by a vision estimation system, Z-down (x: north, y: east, z: down).")]
        VISION_NED=16, 
    	///<summary> Odometry local coordinate frame of data given by a vision estimation system, Z-up (x: east, y: north, z: up). | </summary>
        [Description("Odometry local coordinate frame of data given by a vision estimation system, Z-up (x: east, y: north, z: up).")]
        VISION_ENU=17, 
    	///<summary> Odometry local coordinate frame of data given by an estimator running onboard the vehicle, Z-down (x: north, y: east, z: down). | </summary>
        [Description("Odometry local coordinate frame of data given by an estimator running onboard the vehicle, Z-down (x: north, y: east, z: down).")]
        ESTIM_NED=18, 
    	///<summary> Odometry local coordinate frame of data given by an estimator running onboard the vehicle, Z-up (x: east, y: noth, z: up). | </summary>
        [Description("Odometry local coordinate frame of data given by an estimator running onboard the vehicle, Z-up (x: east, y: noth, z: up).")]
        ESTIM_ENU=19, 
    
    };
    
    ///<summary>  </summary>
    public enum MAVLINK_DATA_STREAM_TYPE: byte
    {
			///<summary>  | </summary>
        [Description("")]
        MAVLINK_DATA_STREAM_IMG_JPEG=1, 
    	///<summary>  | </summary>
        [Description("")]
        MAVLINK_DATA_STREAM_IMG_BMP=2, 
    	///<summary>  | </summary>
        [Description("")]
        MAVLINK_DATA_STREAM_IMG_RAW8U=3, 
    	///<summary>  | </summary>
        [Description("")]
        MAVLINK_DATA_STREAM_IMG_RAW32U=4, 
    	///<summary>  | </summary>
        [Description("")]
        MAVLINK_DATA_STREAM_IMG_PGM=5, 
    	///<summary>  | </summary>
        [Description("")]
        MAVLINK_DATA_STREAM_IMG_PNG=6, 
    
    };
    
    ///<summary>  </summary>
    public enum FENCE_ACTION: int /*default*/
    {
			///<summary> Disable fenced mode | </summary>
        [Description("Disable fenced mode")]
        NONE=0, 
    	///<summary> Switched to guided mode to return point (fence point 0) | </summary>
        [Description("Switched to guided mode to return point (fence point 0)")]
        GUIDED=1, 
    	///<summary> Report fence breach, but don't take action | </summary>
        [Description("Report fence breach, but don't take action")]
        REPORT=2, 
    	///<summary> Switched to guided mode to return point (fence point 0) with manual throttle control | </summary>
        [Description("Switched to guided mode to return point (fence point 0) with manual throttle control")]
        GUIDED_THR_PASS=3, 
    	///<summary> Switch to RTL (return to launch) mode and head for the return point. | </summary>
        [Description("Switch to RTL (return to launch) mode and head for the return point.")]
        RTL=4, 
    
    };
    
    ///<summary>  </summary>
    public enum FENCE_BREACH: byte
    {
			///<summary> No last fence breach | </summary>
        [Description("No last fence breach")]
        NONE=0, 
    	///<summary> Breached minimum altitude | </summary>
        [Description("Breached minimum altitude")]
        MINALT=1, 
    	///<summary> Breached maximum altitude | </summary>
        [Description("Breached maximum altitude")]
        MAXALT=2, 
    	///<summary> Breached fence boundary | </summary>
        [Description("Breached fence boundary")]
        BOUNDARY=3, 
    
    };
    
    ///<summary> Enumeration of possible mount operation modes </summary>
    public enum MAV_MOUNT_MODE: byte
    {
			///<summary> Load and keep safe position (Roll,Pitch,Yaw) from permant memory and stop stabilization | </summary>
        [Description("Load and keep safe position (Roll,Pitch,Yaw) from permant memory and stop stabilization")]
        RETRACT=0, 
    	///<summary> Load and keep neutral position (Roll,Pitch,Yaw) from permanent memory. | </summary>
        [Description("Load and keep neutral position (Roll,Pitch,Yaw) from permanent memory.")]
        NEUTRAL=1, 
    	///<summary> Load neutral position and start MAVLink Roll,Pitch,Yaw control with stabilization | </summary>
        [Description("Load neutral position and start MAVLink Roll,Pitch,Yaw control with stabilization")]
        MAVLINK_TARGETING=2, 
    	///<summary> Load neutral position and start RC Roll,Pitch,Yaw control with stabilization | </summary>
        [Description("Load neutral position and start RC Roll,Pitch,Yaw control with stabilization")]
        RC_TARGETING=3, 
    	///<summary> Load neutral position and start to point to Lat,Lon,Alt | </summary>
        [Description("Load neutral position and start to point to Lat,Lon,Alt")]
        GPS_POINT=4, 
    
    };
    
    ///<summary> Generalized UAVCAN node health </summary>
    public enum UAVCAN_NODE_HEALTH: byte
    {
			///<summary> The node is functioning properly. | </summary>
        [Description("The node is functioning properly.")]
        OK=0, 
    	///<summary> A critical parameter went out of range or the node has encountered a minor failure. | </summary>
        [Description("A critical parameter went out of range or the node has encountered a minor failure.")]
        WARNING=1, 
    	///<summary> The node has encountered a major failure. | </summary>
        [Description("The node has encountered a major failure.")]
        ERROR=2, 
    	///<summary> The node has suffered a fatal malfunction. | </summary>
        [Description("The node has suffered a fatal malfunction.")]
        CRITICAL=3, 
    
    };
    
    ///<summary> Generalized UAVCAN node mode </summary>
    public enum UAVCAN_NODE_MODE: byte
    {
			///<summary> The node is performing its primary functions. | </summary>
        [Description("The node is performing its primary functions.")]
        OPERATIONAL=0, 
    	///<summary> The node is initializing; this mode is entered immediately after startup. | </summary>
        [Description("The node is initializing; this mode is entered immediately after startup.")]
        INITIALIZATION=1, 
    	///<summary> The node is under maintenance. | </summary>
        [Description("The node is under maintenance.")]
        MAINTENANCE=2, 
    	///<summary> The node is in the process of updating its software. | </summary>
        [Description("The node is in the process of updating its software.")]
        SOFTWARE_UPDATE=3, 
    	///<summary> The node is no longer available online. | </summary>
        [Description("The node is no longer available online.")]
        OFFLINE=7, 
    
    };
    
    ///<summary> A data stream is not a fixed set of messages, but rather a    
///     recommendation to the autopilot software. Individual autopilots may or may not obey    
///     the recommended messages. </summary>
    public enum MAV_DATA_STREAM: int /*default*/
    {
			///<summary> Enable all data streams | </summary>
        [Description("Enable all data streams")]
        ALL=0, 
    	///<summary> Enable IMU_RAW, GPS_RAW, GPS_STATUS packets. | </summary>
        [Description("Enable IMU_RAW, GPS_RAW, GPS_STATUS packets.")]
        RAW_SENSORS=1, 
    	///<summary> Enable GPS_STATUS, CONTROL_STATUS, AUX_STATUS | </summary>
        [Description("Enable GPS_STATUS, CONTROL_STATUS, AUX_STATUS")]
        EXTENDED_STATUS=2, 
    	///<summary> Enable RC_CHANNELS_SCALED, RC_CHANNELS_RAW, SERVO_OUTPUT_RAW | </summary>
        [Description("Enable RC_CHANNELS_SCALED, RC_CHANNELS_RAW, SERVO_OUTPUT_RAW")]
        RC_CHANNELS=3, 
    	///<summary> Enable ATTITUDE_CONTROLLER_OUTPUT, POSITION_CONTROLLER_OUTPUT, NAV_CONTROLLER_OUTPUT. | </summary>
        [Description("Enable ATTITUDE_CONTROLLER_OUTPUT, POSITION_CONTROLLER_OUTPUT, NAV_CONTROLLER_OUTPUT.")]
        RAW_CONTROLLER=4, 
    	///<summary> Enable LOCAL_POSITION, GLOBAL_POSITION/GLOBAL_POSITION_INT messages. | </summary>
        [Description("Enable LOCAL_POSITION, GLOBAL_POSITION/GLOBAL_POSITION_INT messages.")]
        POSITION=6, 
    	///<summary> Dependent on the autopilot | </summary>
        [Description("Dependent on the autopilot")]
        EXTRA1=10, 
    	///<summary> Dependent on the autopilot | </summary>
        [Description("Dependent on the autopilot")]
        EXTRA2=11, 
    	///<summary> Dependent on the autopilot | </summary>
        [Description("Dependent on the autopilot")]
        EXTRA3=12, 
    
    };
    
    ///<summary> THIS INTERFACE IS DEPRECATED AS OF JANUARY 2018. Please use MAV_CMD_DO_SET_ROI_* messages instead. The ROI (region of interest) for the vehicle. This can be    
///                be used by the vehicle for camera/vehicle attitude alignment (see    
///                MAV_CMD_NAV_ROI). </summary>
    public enum MAV_ROI: int /*default*/
    {
			///<summary> No region of interest. | </summary>
        [Description("No region of interest.")]
        NONE=0, 
    	///<summary> Point toward next waypoint, with optional pitch/roll/yaw offset. | </summary>
        [Description("Point toward next waypoint, with optional pitch/roll/yaw offset.")]
        WPNEXT=1, 
    	///<summary> Point toward given waypoint. | </summary>
        [Description("Point toward given waypoint.")]
        WPINDEX=2, 
    	///<summary> Point toward fixed location. | </summary>
        [Description("Point toward fixed location.")]
        LOCATION=3, 
    	///<summary> Point toward of given id. | </summary>
        [Description("Point toward of given id.")]
        TARGET=4, 
    
    };
    
    ///<summary> ACK / NACK / ERROR values as a result of MAV_CMDs and for mission item transmission. </summary>
    public enum MAV_CMD_ACK: int /*default*/
    {
			///<summary> Command / mission item is ok. | </summary>
        [Description("Command / mission item is ok.")]
        OK=1, 
    	///<summary> Generic error message if none of the other reasons fails or if no detailed error reporting is implemented. | </summary>
        [Description("Generic error message if none of the other reasons fails or if no detailed error reporting is implemented.")]
        ERR_FAIL=2, 
    	///<summary> The system is refusing to accept this command from this source / communication partner. | </summary>
        [Description("The system is refusing to accept this command from this source / communication partner.")]
        ERR_ACCESS_DENIED=3, 
    	///<summary> Command or mission item is not supported, other commands would be accepted. | </summary>
        [Description("Command or mission item is not supported, other commands would be accepted.")]
        ERR_NOT_SUPPORTED=4, 
    	///<summary> The coordinate frame of this command / mission item is not supported. | </summary>
        [Description("The coordinate frame of this command / mission item is not supported.")]
        ERR_COORDINATE_FRAME_NOT_SUPPORTED=5, 
    	///<summary> The coordinate frame of this command is ok, but he coordinate values exceed the safety limits of this system. This is a generic error, please use the more specific error messages below if possible. | </summary>
        [Description("The coordinate frame of this command is ok, but he coordinate values exceed the safety limits of this system. This is a generic error, please use the more specific error messages below if possible.")]
        ERR_COORDINATES_OUT_OF_RANGE=6, 
    	///<summary> The X or latitude value is out of range. | </summary>
        [Description("The X or latitude value is out of range.")]
        ERR_X_LAT_OUT_OF_RANGE=7, 
    	///<summary> The Y or longitude value is out of range. | </summary>
        [Description("The Y or longitude value is out of range.")]
        ERR_Y_LON_OUT_OF_RANGE=8, 
    	///<summary> The Z or altitude value is out of range. | </summary>
        [Description("The Z or altitude value is out of range.")]
        ERR_Z_ALT_OUT_OF_RANGE=9, 
    
    };
    
    ///<summary> Specifies the datatype of a MAVLink parameter. </summary>
    public enum MAV_PARAM_TYPE: byte
    {
			///<summary> 8-bit unsigned integer | </summary>
        [Description("8-bit unsigned integer")]
        UINT8=1, 
    	///<summary> 8-bit signed integer | </summary>
        [Description("8-bit signed integer")]
        INT8=2, 
    	///<summary> 16-bit unsigned integer | </summary>
        [Description("16-bit unsigned integer")]
        UINT16=3, 
    	///<summary> 16-bit signed integer | </summary>
        [Description("16-bit signed integer")]
        INT16=4, 
    	///<summary> 32-bit unsigned integer | </summary>
        [Description("32-bit unsigned integer")]
        UINT32=5, 
    	///<summary> 32-bit signed integer | </summary>
        [Description("32-bit signed integer")]
        INT32=6, 
    	///<summary> 64-bit unsigned integer | </summary>
        [Description("64-bit unsigned integer")]
        UINT64=7, 
    	///<summary> 64-bit signed integer | </summary>
        [Description("64-bit signed integer")]
        INT64=8, 
    	///<summary> 32-bit floating-point | </summary>
        [Description("32-bit floating-point")]
        REAL32=9, 
    	///<summary> 64-bit floating-point | </summary>
        [Description("64-bit floating-point")]
        REAL64=10, 
    
    };
    
    ///<summary> result from a mavlink command </summary>
    public enum MAV_RESULT: byte
    {
			///<summary> Command ACCEPTED and EXECUTED | </summary>
        [Description("Command ACCEPTED and EXECUTED")]
        ACCEPTED=0, 
    	///<summary> Command TEMPORARY REJECTED/DENIED | </summary>
        [Description("Command TEMPORARY REJECTED/DENIED")]
        TEMPORARILY_REJECTED=1, 
    	///<summary> Command PERMANENTLY DENIED | </summary>
        [Description("Command PERMANENTLY DENIED")]
        DENIED=2, 
    	///<summary> Command UNKNOWN/UNSUPPORTED | </summary>
        [Description("Command UNKNOWN/UNSUPPORTED")]
        UNSUPPORTED=3, 
    	///<summary> Command executed, but failed | </summary>
        [Description("Command executed, but failed")]
        FAILED=4, 
    
    };
    
    ///<summary> Result of mission operation (in a MISSION_ACK message). </summary>
    public enum MAV_MISSION_RESULT: byte
    {
			///<summary> mission accepted OK | </summary>
        [Description("mission accepted OK")]
        MAV_MISSION_ACCEPTED=0, 
    	///<summary> Generic error / not accepting mission commands at all right now. | </summary>
        [Description("Generic error / not accepting mission commands at all right now.")]
        MAV_MISSION_ERROR=1, 
    	///<summary> Coordinate frame is not supported. | </summary>
        [Description("Coordinate frame is not supported.")]
        MAV_MISSION_UNSUPPORTED_FRAME=2, 
    	///<summary> Command is not supported. | </summary>
        [Description("Command is not supported.")]
        MAV_MISSION_UNSUPPORTED=3, 
    	///<summary> Mission item exceeds storage space. | </summary>
        [Description("Mission item exceeds storage space.")]
        MAV_MISSION_NO_SPACE=4, 
    	///<summary> One of the parameters has an invalid value. | </summary>
        [Description("One of the parameters has an invalid value.")]
        MAV_MISSION_INVALID=5, 
    	///<summary> param1 has an invalid value. | </summary>
        [Description("param1 has an invalid value.")]
        MAV_MISSION_INVALID_PARAM1=6, 
    	///<summary> param2 has an invalid value. | </summary>
        [Description("param2 has an invalid value.")]
        MAV_MISSION_INVALID_PARAM2=7, 
    	///<summary> param3 has an invalid value. | </summary>
        [Description("param3 has an invalid value.")]
        MAV_MISSION_INVALID_PARAM3=8, 
    	///<summary> param4 has an invalid value. | </summary>
        [Description("param4 has an invalid value.")]
        MAV_MISSION_INVALID_PARAM4=9, 
    	///<summary> x / param5 has an invalid value. | </summary>
        [Description("x / param5 has an invalid value.")]
        MAV_MISSION_INVALID_PARAM5_X=10, 
    	///<summary> y / param6 has an invalid value. | </summary>
        [Description("y / param6 has an invalid value.")]
        MAV_MISSION_INVALID_PARAM6_Y=11, 
    	///<summary> z / param7 has an invalid value. | </summary>
        [Description("z / param7 has an invalid value.")]
        MAV_MISSION_INVALID_PARAM7=12, 
    	///<summary> Mission item received out of sequence | </summary>
        [Description("Mission item received out of sequence")]
        MAV_MISSION_INVALID_SEQUENCE=13, 
    	///<summary> Not accepting any mission commands from this communication partner. | </summary>
        [Description("Not accepting any mission commands from this communication partner.")]
        MAV_MISSION_DENIED=14, 
    	///<summary> Current mission operation cancelled (e.g. mission upload, mission download). | </summary>
        [Description("Current mission operation cancelled (e.g. mission upload, mission download).")]
        MAV_MISSION_OPERATION_CANCELLED=15, 
    
    };
    
    ///<summary> Indicates the severity level, generally used for status messages to indicate their relative urgency. Based on RFC-5424 using expanded definitions at: http://www.kiwisyslog.com/kb/info:-syslog-message-levels/. </summary>
    public enum MAV_SEVERITY: byte
    {
			///<summary> System is unusable. This is a 'panic' condition. | </summary>
        [Description("System is unusable. This is a 'panic' condition.")]
        EMERGENCY=0, 
    	///<summary> Action should be taken immediately. Indicates error in non-critical systems. | </summary>
        [Description("Action should be taken immediately. Indicates error in non-critical systems.")]
        ALERT=1, 
    	///<summary> Action must be taken immediately. Indicates failure in a primary system. | </summary>
        [Description("Action must be taken immediately. Indicates failure in a primary system.")]
        CRITICAL=2, 
    	///<summary> Indicates an error in secondary/redundant systems. | </summary>
        [Description("Indicates an error in secondary/redundant systems.")]
        ERROR=3, 
    	///<summary> Indicates about a possible future error if this is not resolved within a given timeframe. Example would be a low battery warning. | </summary>
        [Description("Indicates about a possible future error if this is not resolved within a given timeframe. Example would be a low battery warning.")]
        WARNING=4, 
    	///<summary> An unusual event has occurred, though not an error condition. This should be investigated for the root cause. | </summary>
        [Description("An unusual event has occurred, though not an error condition. This should be investigated for the root cause.")]
        NOTICE=5, 
    	///<summary> Normal operational messages. Useful for logging. No action is required for these messages. | </summary>
        [Description("Normal operational messages. Useful for logging. No action is required for these messages.")]
        INFO=6, 
    	///<summary> Useful non-operational messages that can assist in debugging. These should not occur during normal operation. | </summary>
        [Description("Useful non-operational messages that can assist in debugging. These should not occur during normal operation.")]
        DEBUG=7, 
    
    };
    
    ///<summary> Power supply status flags (bitmask) </summary>
    public enum MAV_POWER_STATUS: ushort
    {
			///<summary> main brick power supply valid | </summary>
        [Description("main brick power supply valid")]
        BRICK_VALID=1, 
    	///<summary> main servo power supply valid for FMU | </summary>
        [Description("main servo power supply valid for FMU")]
        SERVO_VALID=2, 
    	///<summary> USB power is connected | </summary>
        [Description("USB power is connected")]
        USB_CONNECTED=4, 
    	///<summary> peripheral supply is in over-current state | </summary>
        [Description("peripheral supply is in over-current state")]
        PERIPH_OVERCURRENT=8, 
    	///<summary> hi-power peripheral supply is in over-current state | </summary>
        [Description("hi-power peripheral supply is in over-current state")]
        PERIPH_HIPOWER_OVERCURRENT=16, 
    	///<summary> Power status has changed since boot | </summary>
        [Description("Power status has changed since boot")]
        CHANGED=32, 
    
    };
    
    ///<summary> SERIAL_CONTROL device types </summary>
    public enum SERIAL_CONTROL_DEV: byte
    {
			///<summary> First telemetry port | </summary>
        [Description("First telemetry port")]
        TELEM1=0, 
    	///<summary> Second telemetry port | </summary>
        [Description("Second telemetry port")]
        TELEM2=1, 
    	///<summary> First GPS port | </summary>
        [Description("First GPS port")]
        GPS1=2, 
    	///<summary> Second GPS port | </summary>
        [Description("Second GPS port")]
        GPS2=3, 
    	///<summary> system shell | </summary>
        [Description("system shell")]
        SHELL=10, 
    
    };
    
    ///<summary> SERIAL_CONTROL flags (bitmask) </summary>
    public enum SERIAL_CONTROL_FLAG: byte
    {
			///<summary> Set if this is a reply | </summary>
        [Description("Set if this is a reply")]
        REPLY=1, 
    	///<summary> Set if the sender wants the receiver to send a response as another SERIAL_CONTROL message | </summary>
        [Description("Set if the sender wants the receiver to send a response as another SERIAL_CONTROL message")]
        RESPOND=2, 
    	///<summary> Set if access to the serial port should be removed from whatever driver is currently using it, giving exclusive access to the SERIAL_CONTROL protocol. The port can be handed back by sending a request without this flag set | </summary>
        [Description("Set if access to the serial port should be removed from whatever driver is currently using it, giving exclusive access to the SERIAL_CONTROL protocol. The port can be handed back by sending a request without this flag set")]
        EXCLUSIVE=4, 
    	///<summary> Block on writes to the serial port | </summary>
        [Description("Block on writes to the serial port")]
        BLOCKING=8, 
    	///<summary> Send multiple replies until port is drained | </summary>
        [Description("Send multiple replies until port is drained")]
        MULTI=16, 
    
    };
    
    ///<summary> Enumeration of distance sensor types </summary>
    public enum MAV_DISTANCE_SENSOR: byte
    {
			///<summary> Laser rangefinder, e.g. LightWare SF02/F or PulsedLight units | </summary>
        [Description("Laser rangefinder, e.g. LightWare SF02/F or PulsedLight units")]
        LASER=0, 
    	///<summary> Ultrasound rangefinder, e.g. MaxBotix units | </summary>
        [Description("Ultrasound rangefinder, e.g. MaxBotix units")]
        ULTRASOUND=1, 
    	///<summary> Infrared rangefinder, e.g. Sharp units | </summary>
        [Description("Infrared rangefinder, e.g. Sharp units")]
        INFRARED=2, 
    	///<summary> Radar type, e.g. uLanding units | </summary>
        [Description("Radar type, e.g. uLanding units")]
        RADAR=3, 
    	///<summary> Broken or unknown type, e.g. analog units | </summary>
        [Description("Broken or unknown type, e.g. analog units")]
        UNKNOWN=4, 
    
    };
    
    ///<summary> Enumeration of sensor orientation, according to its rotations </summary>
    public enum MAV_SENSOR_ORIENTATION: byte
    {
			///<summary> Roll: 0, Pitch: 0, Yaw: 0 | </summary>
        [Description("Roll: 0, Pitch: 0, Yaw: 0")]
        MAV_SENSOR_ROTATION_NONE=0, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 45 | </summary>
        [Description("Roll: 0, Pitch: 0, Yaw: 45")]
        MAV_SENSOR_ROTATION_YAW_45=1, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 90 | </summary>
        [Description("Roll: 0, Pitch: 0, Yaw: 90")]
        MAV_SENSOR_ROTATION_YAW_90=2, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 135 | </summary>
        [Description("Roll: 0, Pitch: 0, Yaw: 135")]
        MAV_SENSOR_ROTATION_YAW_135=3, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 180 | </summary>
        [Description("Roll: 0, Pitch: 0, Yaw: 180")]
        MAV_SENSOR_ROTATION_YAW_180=4, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 225 | </summary>
        [Description("Roll: 0, Pitch: 0, Yaw: 225")]
        MAV_SENSOR_ROTATION_YAW_225=5, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 270 | </summary>
        [Description("Roll: 0, Pitch: 0, Yaw: 270")]
        MAV_SENSOR_ROTATION_YAW_270=6, 
    	///<summary> Roll: 0, Pitch: 0, Yaw: 315 | </summary>
        [Description("Roll: 0, Pitch: 0, Yaw: 315")]
        MAV_SENSOR_ROTATION_YAW_315=7, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 0 | </summary>
        [Description("Roll: 180, Pitch: 0, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_180=8, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 45 | </summary>
        [Description("Roll: 180, Pitch: 0, Yaw: 45")]
        MAV_SENSOR_ROTATION_ROLL_180_YAW_45=9, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 90 | </summary>
        [Description("Roll: 180, Pitch: 0, Yaw: 90")]
        MAV_SENSOR_ROTATION_ROLL_180_YAW_90=10, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 135 | </summary>
        [Description("Roll: 180, Pitch: 0, Yaw: 135")]
        MAV_SENSOR_ROTATION_ROLL_180_YAW_135=11, 
    	///<summary> Roll: 0, Pitch: 180, Yaw: 0 | </summary>
        [Description("Roll: 0, Pitch: 180, Yaw: 0")]
        MAV_SENSOR_ROTATION_PITCH_180=12, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 225 | </summary>
        [Description("Roll: 180, Pitch: 0, Yaw: 225")]
        MAV_SENSOR_ROTATION_ROLL_180_YAW_225=13, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 270 | </summary>
        [Description("Roll: 180, Pitch: 0, Yaw: 270")]
        MAV_SENSOR_ROTATION_ROLL_180_YAW_270=14, 
    	///<summary> Roll: 180, Pitch: 0, Yaw: 315 | </summary>
        [Description("Roll: 180, Pitch: 0, Yaw: 315")]
        MAV_SENSOR_ROTATION_ROLL_180_YAW_315=15, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 0 | </summary>
        [Description("Roll: 90, Pitch: 0, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_90=16, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 45 | </summary>
        [Description("Roll: 90, Pitch: 0, Yaw: 45")]
        MAV_SENSOR_ROTATION_ROLL_90_YAW_45=17, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 90 | </summary>
        [Description("Roll: 90, Pitch: 0, Yaw: 90")]
        MAV_SENSOR_ROTATION_ROLL_90_YAW_90=18, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 135 | </summary>
        [Description("Roll: 90, Pitch: 0, Yaw: 135")]
        MAV_SENSOR_ROTATION_ROLL_90_YAW_135=19, 
    	///<summary> Roll: 270, Pitch: 0, Yaw: 0 | </summary>
        [Description("Roll: 270, Pitch: 0, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_270=20, 
    	///<summary> Roll: 270, Pitch: 0, Yaw: 45 | </summary>
        [Description("Roll: 270, Pitch: 0, Yaw: 45")]
        MAV_SENSOR_ROTATION_ROLL_270_YAW_45=21, 
    	///<summary> Roll: 270, Pitch: 0, Yaw: 90 | </summary>
        [Description("Roll: 270, Pitch: 0, Yaw: 90")]
        MAV_SENSOR_ROTATION_ROLL_270_YAW_90=22, 
    	///<summary> Roll: 270, Pitch: 0, Yaw: 135 | </summary>
        [Description("Roll: 270, Pitch: 0, Yaw: 135")]
        MAV_SENSOR_ROTATION_ROLL_270_YAW_135=23, 
    	///<summary> Roll: 0, Pitch: 90, Yaw: 0 | </summary>
        [Description("Roll: 0, Pitch: 90, Yaw: 0")]
        MAV_SENSOR_ROTATION_PITCH_90=24, 
    	///<summary> Roll: 0, Pitch: 270, Yaw: 0 | </summary>
        [Description("Roll: 0, Pitch: 270, Yaw: 0")]
        MAV_SENSOR_ROTATION_PITCH_270=25, 
    	///<summary> Roll: 0, Pitch: 180, Yaw: 90 | </summary>
        [Description("Roll: 0, Pitch: 180, Yaw: 90")]
        MAV_SENSOR_ROTATION_PITCH_180_YAW_90=26, 
    	///<summary> Roll: 0, Pitch: 180, Yaw: 270 | </summary>
        [Description("Roll: 0, Pitch: 180, Yaw: 270")]
        MAV_SENSOR_ROTATION_PITCH_180_YAW_270=27, 
    	///<summary> Roll: 90, Pitch: 90, Yaw: 0 | </summary>
        [Description("Roll: 90, Pitch: 90, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_90=28, 
    	///<summary> Roll: 180, Pitch: 90, Yaw: 0 | </summary>
        [Description("Roll: 180, Pitch: 90, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_180_PITCH_90=29, 
    	///<summary> Roll: 270, Pitch: 90, Yaw: 0 | </summary>
        [Description("Roll: 270, Pitch: 90, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_270_PITCH_90=30, 
    	///<summary> Roll: 90, Pitch: 180, Yaw: 0 | </summary>
        [Description("Roll: 90, Pitch: 180, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_180=31, 
    	///<summary> Roll: 270, Pitch: 180, Yaw: 0 | </summary>
        [Description("Roll: 270, Pitch: 180, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_270_PITCH_180=32, 
    	///<summary> Roll: 90, Pitch: 270, Yaw: 0 | </summary>
        [Description("Roll: 90, Pitch: 270, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_270=33, 
    	///<summary> Roll: 180, Pitch: 270, Yaw: 0 | </summary>
        [Description("Roll: 180, Pitch: 270, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_180_PITCH_270=34, 
    	///<summary> Roll: 270, Pitch: 270, Yaw: 0 | </summary>
        [Description("Roll: 270, Pitch: 270, Yaw: 0")]
        MAV_SENSOR_ROTATION_ROLL_270_PITCH_270=35, 
    	///<summary> Roll: 90, Pitch: 180, Yaw: 90 | </summary>
        [Description("Roll: 90, Pitch: 180, Yaw: 90")]
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_180_YAW_90=36, 
    	///<summary> Roll: 90, Pitch: 0, Yaw: 270 | </summary>
        [Description("Roll: 90, Pitch: 0, Yaw: 270")]
        MAV_SENSOR_ROTATION_ROLL_90_YAW_270=37, 
    	///<summary> Roll: 90, Pitch: 68, Yaw: 293 | </summary>
        [Description("Roll: 90, Pitch: 68, Yaw: 293")]
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_68_YAW_293=38, 
    	///<summary> Pitch: 315 | </summary>
        [Description("Pitch: 315")]
        MAV_SENSOR_ROTATION_PITCH_315=39, 
    	///<summary> Roll: 90, Pitch: 315 | </summary>
        [Description("Roll: 90, Pitch: 315")]
        MAV_SENSOR_ROTATION_ROLL_90_PITCH_315=40, 
    	///<summary> Custom orientation | </summary>
        [Description("Custom orientation")]
        MAV_SENSOR_ROTATION_CUSTOM=100, 
    
    };
    
    ///<summary> Bitmask of (optional) autopilot capabilities (64 bit). If a bit is set, the autopilot supports this capability. </summary>
    public enum MAV_PROTOCOL_CAPABILITY: ulong
    {
			///<summary> Autopilot supports MISSION float message type. | </summary>
        [Description("Autopilot supports MISSION float message type.")]
        MISSION_FLOAT=1, 
    	///<summary> Autopilot supports the new param float message type. | </summary>
        [Description("Autopilot supports the new param float message type.")]
        PARAM_FLOAT=2, 
    	///<summary> Autopilot supports MISSION_INT scaled integer message type. | </summary>
        [Description("Autopilot supports MISSION_INT scaled integer message type.")]
        MISSION_INT=4, 
    	///<summary> Autopilot supports COMMAND_INT scaled integer message type. | </summary>
        [Description("Autopilot supports COMMAND_INT scaled integer message type.")]
        COMMAND_INT=8, 
    	///<summary> Autopilot supports the new param union message type. | </summary>
        [Description("Autopilot supports the new param union message type.")]
        PARAM_UNION=16, 
    	///<summary> Autopilot supports the new FILE_TRANSFER_PROTOCOL message type. | </summary>
        [Description("Autopilot supports the new FILE_TRANSFER_PROTOCOL message type.")]
        FTP=32, 
    	///<summary> Autopilot supports commanding attitude offboard. | </summary>
        [Description("Autopilot supports commanding attitude offboard.")]
        SET_ATTITUDE_TARGET=64, 
    	///<summary> Autopilot supports commanding position and velocity targets in local NED frame. | </summary>
        [Description("Autopilot supports commanding position and velocity targets in local NED frame.")]
        SET_POSITION_TARGET_LOCAL_NED=128, 
    	///<summary> Autopilot supports commanding position and velocity targets in global scaled integers. | </summary>
        [Description("Autopilot supports commanding position and velocity targets in global scaled integers.")]
        SET_POSITION_TARGET_GLOBAL_INT=256, 
    	///<summary> Autopilot supports terrain protocol / data handling. | </summary>
        [Description("Autopilot supports terrain protocol / data handling.")]
        TERRAIN=512, 
    	///<summary> Autopilot supports direct actuator control. | </summary>
        [Description("Autopilot supports direct actuator control.")]
        SET_ACTUATOR_TARGET=1024, 
    	///<summary> Autopilot supports the flight termination command. | </summary>
        [Description("Autopilot supports the flight termination command.")]
        FLIGHT_TERMINATION=2048, 
    	///<summary> Autopilot supports onboard compass calibration. | </summary>
        [Description("Autopilot supports onboard compass calibration.")]
        COMPASS_CALIBRATION=4096, 
    	///<summary> Autopilot supports MAVLink version 2. | </summary>
        [Description("Autopilot supports MAVLink version 2.")]
        MAVLINK2=8192, 
    	///<summary> Autopilot supports mission fence protocol. | </summary>
        [Description("Autopilot supports mission fence protocol.")]
        MISSION_FENCE=16384, 
    	///<summary> Autopilot supports mission rally point protocol. | </summary>
        [Description("Autopilot supports mission rally point protocol.")]
        MISSION_RALLY=32768, 
    	///<summary> Autopilot supports the flight information protocol. | </summary>
        [Description("Autopilot supports the flight information protocol.")]
        FLIGHT_INFORMATION=65536, 
    
    };
    
    ///<summary> Type of mission items being requested/sent in mission protocol. </summary>
    public enum MAV_MISSION_TYPE: byte
    {
			///<summary> Items are mission commands for main mission. | </summary>
        [Description("Items are mission commands for main mission.")]
        MISSION=0, 
    	///<summary> Specifies GeoFence area(s). Items are MAV_CMD_NAV_FENCE_ GeoFence items. | </summary>
        [Description("Specifies GeoFence area(s). Items are MAV_CMD_NAV_FENCE_ GeoFence items.")]
        FENCE=1, 
    	///<summary> Specifies the rally points for the vehicle. Rally points are alternative RTL points. Items are MAV_CMD_NAV_RALLY_POINT rally point items. | </summary>
        [Description("Specifies the rally points for the vehicle. Rally points are alternative RTL points. Items are MAV_CMD_NAV_RALLY_POINT rally point items.")]
        RALLY=2, 
    	///<summary> Only used in MISSION_CLEAR_ALL to clear all mission types. | </summary>
        [Description("Only used in MISSION_CLEAR_ALL to clear all mission types.")]
        ALL=255, 
    
    };
    
    ///<summary> Enumeration of estimator types </summary>
    public enum MAV_ESTIMATOR_TYPE: byte
    {
			///<summary> This is a naive estimator without any real covariance feedback. | </summary>
        [Description("This is a naive estimator without any real covariance feedback.")]
        NAIVE=1, 
    	///<summary> Computer vision based estimate. Might be up to scale. | </summary>
        [Description("Computer vision based estimate. Might be up to scale.")]
        VISION=2, 
    	///<summary> Visual-inertial estimate. | </summary>
        [Description("Visual-inertial estimate.")]
        VIO=3, 
    	///<summary> Plain GPS estimate. | </summary>
        [Description("Plain GPS estimate.")]
        GPS=4, 
    	///<summary> Estimator integrating GPS and inertial sensing. | </summary>
        [Description("Estimator integrating GPS and inertial sensing.")]
        GPS_INS=5, 
    
    };
    
    ///<summary> Enumeration of battery types </summary>
    public enum MAV_BATTERY_TYPE: byte
    {
			///<summary> Not specified. | </summary>
        [Description("Not specified.")]
        UNKNOWN=0, 
    	///<summary> Lithium polymer battery | </summary>
        [Description("Lithium polymer battery")]
        LIPO=1, 
    	///<summary> Lithium-iron-phosphate battery | </summary>
        [Description("Lithium-iron-phosphate battery")]
        LIFE=2, 
    	///<summary> Lithium-ION battery | </summary>
        [Description("Lithium-ION battery")]
        LION=3, 
    	///<summary> Nickel metal hydride battery | </summary>
        [Description("Nickel metal hydride battery")]
        NIMH=4, 
    
    };
    
    ///<summary> Enumeration of battery functions </summary>
    public enum MAV_BATTERY_FUNCTION: byte
    {
			///<summary> Battery function is unknown | </summary>
        [Description("Battery function is unknown")]
        UNKNOWN=0, 
    	///<summary> Battery supports all flight systems | </summary>
        [Description("Battery supports all flight systems")]
        ALL=1, 
    	///<summary> Battery for the propulsion system | </summary>
        [Description("Battery for the propulsion system")]
        PROPULSION=2, 
    	///<summary> Avionics battery | </summary>
        [Description("Avionics battery")]
        AVIONICS=3, 
    	///<summary> Payload battery | </summary>
        [Description("Payload battery")]
        MAV_BATTERY_TYPE_PAYLOAD=4, 
    
    };
    
    ///<summary> Enumeration for battery charge states. </summary>
    public enum MAV_BATTERY_CHARGE_STATE: byte
    {
			///<summary> Low battery state is not provided | </summary>
        [Description("Low battery state is not provided")]
        UNDEFINED=0, 
    	///<summary> Battery is not in low state. Normal operation. | </summary>
        [Description("Battery is not in low state. Normal operation.")]
        OK=1, 
    	///<summary> Battery state is low, warn and monitor close. | </summary>
        [Description("Battery state is low, warn and monitor close.")]
        LOW=2, 
    	///<summary> Battery state is critical, return or abort immediately. | </summary>
        [Description("Battery state is critical, return or abort immediately.")]
        CRITICAL=3, 
    	///<summary> Battery state is too low for ordinary abort sequence. Perform fastest possible emergency stop to prevent damage. | </summary>
        [Description("Battery state is too low for ordinary abort sequence. Perform fastest possible emergency stop to prevent damage.")]
        EMERGENCY=4, 
    	///<summary> Battery failed, damage unavoidable. | </summary>
        [Description("Battery failed, damage unavoidable.")]
        FAILED=5, 
    	///<summary> Battery is diagnosed to be defective or an error occurred, usage is discouraged / prohibited. | </summary>
        [Description("Battery is diagnosed to be defective or an error occurred, usage is discouraged / prohibited.")]
        UNHEALTHY=6, 
    	///<summary> Battery is charging. | </summary>
        [Description("Battery is charging.")]
        CHARGING=7, 
    
    };
    
    ///<summary> Enumeration of VTOL states </summary>
    public enum MAV_VTOL_STATE: byte
    {
			///<summary> MAV is not configured as VTOL | </summary>
        [Description("MAV is not configured as VTOL")]
        UNDEFINED=0, 
    	///<summary> VTOL is in transition from multicopter to fixed-wing | </summary>
        [Description("VTOL is in transition from multicopter to fixed-wing")]
        TRANSITION_TO_FW=1, 
    	///<summary> VTOL is in transition from fixed-wing to multicopter | </summary>
        [Description("VTOL is in transition from fixed-wing to multicopter")]
        TRANSITION_TO_MC=2, 
    	///<summary> VTOL is in multicopter state | </summary>
        [Description("VTOL is in multicopter state")]
        MC=3, 
    	///<summary> VTOL is in fixed-wing state | </summary>
        [Description("VTOL is in fixed-wing state")]
        FW=4, 
    
    };
    
    ///<summary> Enumeration of landed detector states </summary>
    public enum MAV_LANDED_STATE: byte
    {
			///<summary> MAV landed state is unknown | </summary>
        [Description("MAV landed state is unknown")]
        UNDEFINED=0, 
    	///<summary> MAV is landed (on ground) | </summary>
        [Description("MAV is landed (on ground)")]
        ON_GROUND=1, 
    	///<summary> MAV is in air | </summary>
        [Description("MAV is in air")]
        IN_AIR=2, 
    	///<summary> MAV currently taking off | </summary>
        [Description("MAV currently taking off")]
        TAKEOFF=3, 
    	///<summary> MAV currently landing | </summary>
        [Description("MAV currently landing")]
        LANDING=4, 
    
    };
    
    ///<summary> Enumeration of the ADSB altimeter types </summary>
    public enum ADSB_ALTITUDE_TYPE: byte
    {
			///<summary> Altitude reported from a Baro source using QNH reference | </summary>
        [Description("Altitude reported from a Baro source using QNH reference")]
        PRESSURE_QNH=0, 
    	///<summary> Altitude reported from a GNSS source | </summary>
        [Description("Altitude reported from a GNSS source")]
        GEOMETRIC=1, 
    
    };
    
    ///<summary> ADSB classification for the type of vehicle emitting the transponder signal </summary>
    public enum ADSB_EMITTER_TYPE: byte
    {
			///<summary>  | </summary>
        [Description("")]
        NO_INFO=0, 
    	///<summary>  | </summary>
        [Description("")]
        LIGHT=1, 
    	///<summary>  | </summary>
        [Description("")]
        SMALL=2, 
    	///<summary>  | </summary>
        [Description("")]
        LARGE=3, 
    	///<summary>  | </summary>
        [Description("")]
        HIGH_VORTEX_LARGE=4, 
    	///<summary>  | </summary>
        [Description("")]
        HEAVY=5, 
    	///<summary>  | </summary>
        [Description("")]
        HIGHLY_MANUV=6, 
    	///<summary>  | </summary>
        [Description("")]
        ROTOCRAFT=7, 
    	///<summary>  | </summary>
        [Description("")]
        UNASSIGNED=8, 
    	///<summary>  | </summary>
        [Description("")]
        GLIDER=9, 
    	///<summary>  | </summary>
        [Description("")]
        LIGHTER_AIR=10, 
    	///<summary>  | </summary>
        [Description("")]
        PARACHUTE=11, 
    	///<summary>  | </summary>
        [Description("")]
        ULTRA_LIGHT=12, 
    	///<summary>  | </summary>
        [Description("")]
        UNASSIGNED2=13, 
    	///<summary>  | </summary>
        [Description("")]
        UAV=14, 
    	///<summary>  | </summary>
        [Description("")]
        SPACE=15, 
    	///<summary>  | </summary>
        [Description("")]
        UNASSGINED3=16, 
    	///<summary>  | </summary>
        [Description("")]
        EMERGENCY_SURFACE=17, 
    	///<summary>  | </summary>
        [Description("")]
        SERVICE_SURFACE=18, 
    	///<summary>  | </summary>
        [Description("")]
        POINT_OBSTACLE=19, 
    
    };
    
    ///<summary> These flags indicate status such as data validity of each data source. Set = data valid </summary>
    public enum ADSB_FLAGS: ushort
    {
			///<summary>  | </summary>
        [Description("")]
        VALID_COORDS=1, 
    	///<summary>  | </summary>
        [Description("")]
        VALID_ALTITUDE=2, 
    	///<summary>  | </summary>
        [Description("")]
        VALID_HEADING=4, 
    	///<summary>  | </summary>
        [Description("")]
        VALID_VELOCITY=8, 
    	///<summary>  | </summary>
        [Description("")]
        VALID_CALLSIGN=16, 
    	///<summary>  | </summary>
        [Description("")]
        VALID_SQUAWK=32, 
    	///<summary>  | </summary>
        [Description("")]
        SIMULATED=64, 
    
    };
    
    ///<summary> Bitmap of options for the MAV_CMD_DO_REPOSITION </summary>
    public enum MAV_DO_REPOSITION_FLAGS: int /*default*/
    {
			///<summary> The aircraft should immediately transition into guided. This should not be set for follow me applications | </summary>
        [Description("The aircraft should immediately transition into guided. This should not be set for follow me applications")]
        CHANGE_MODE=1, 
    
    };
    
    ///<summary> Flags in EKF_STATUS message </summary>
    public enum ESTIMATOR_STATUS_FLAGS: ushort
    {
			///<summary> True if the attitude estimate is good | </summary>
        [Description("True if the attitude estimate is good")]
        ESTIMATOR_ATTITUDE=1, 
    	///<summary> True if the horizontal velocity estimate is good | </summary>
        [Description("True if the horizontal velocity estimate is good")]
        ESTIMATOR_VELOCITY_HORIZ=2, 
    	///<summary> True if the  vertical velocity estimate is good | </summary>
        [Description("True if the  vertical velocity estimate is good")]
        ESTIMATOR_VELOCITY_VERT=4, 
    	///<summary> True if the horizontal position (relative) estimate is good | </summary>
        [Description("True if the horizontal position (relative) estimate is good")]
        ESTIMATOR_POS_HORIZ_REL=8, 
    	///<summary> True if the horizontal position (absolute) estimate is good | </summary>
        [Description("True if the horizontal position (absolute) estimate is good")]
        ESTIMATOR_POS_HORIZ_ABS=16, 
    	///<summary> True if the vertical position (absolute) estimate is good | </summary>
        [Description("True if the vertical position (absolute) estimate is good")]
        ESTIMATOR_POS_VERT_ABS=32, 
    	///<summary> True if the vertical position (above ground) estimate is good | </summary>
        [Description("True if the vertical position (above ground) estimate is good")]
        ESTIMATOR_POS_VERT_AGL=64, 
    	///<summary> True if the EKF is in a constant position mode and is not using external measurements (eg GPS or optical flow) | </summary>
        [Description("True if the EKF is in a constant position mode and is not using external measurements (eg GPS or optical flow)")]
        ESTIMATOR_CONST_POS_MODE=128, 
    	///<summary> True if the EKF has sufficient data to enter a mode that will provide a (relative) position estimate | </summary>
        [Description("True if the EKF has sufficient data to enter a mode that will provide a (relative) position estimate")]
        ESTIMATOR_PRED_POS_HORIZ_REL=256, 
    	///<summary> True if the EKF has sufficient data to enter a mode that will provide a (absolute) position estimate | </summary>
        [Description("True if the EKF has sufficient data to enter a mode that will provide a (absolute) position estimate")]
        ESTIMATOR_PRED_POS_HORIZ_ABS=512, 
    	///<summary> True if the EKF has detected a GPS glitch | </summary>
        [Description("True if the EKF has detected a GPS glitch")]
        ESTIMATOR_GPS_GLITCH=1024, 
    	///<summary> True if the EKF has detected bad accelerometer data | </summary>
        [Description("True if the EKF has detected bad accelerometer data")]
        ESTIMATOR_ACCEL_ERROR=2048, 
    
    };
    
    ///<summary>  </summary>
    public enum MOTOR_TEST_ORDER: int /*default*/
    {
			///<summary> default autopilot motor test method | </summary>
        [Description("default autopilot motor test method")]
        DEFAULT=0, 
    	///<summary> motor numbers are specified as their index in a predefined vehicle-specific sequence | </summary>
        [Description("motor numbers are specified as their index in a predefined vehicle-specific sequence")]
        SEQUENCE=1, 
    	///<summary> motor numbers are specified as the output as labeled on the board | </summary>
        [Description("motor numbers are specified as the output as labeled on the board")]
        BOARD=2, 
    
    };
    
    ///<summary>  </summary>
    public enum MOTOR_TEST_THROTTLE_TYPE: int /*default*/
    {
			///<summary> throttle as a percentage from 0 ~ 100 | </summary>
        [Description("throttle as a percentage from 0 ~ 100")]
        MOTOR_TEST_THROTTLE_PERCENT=0, 
    	///<summary> throttle as an absolute PWM value (normally in range of 1000~2000) | </summary>
        [Description("throttle as an absolute PWM value (normally in range of 1000~2000)")]
        MOTOR_TEST_THROTTLE_PWM=1, 
    	///<summary> throttle pass-through from pilot's transmitter | </summary>
        [Description("throttle pass-through from pilot's transmitter")]
        MOTOR_TEST_THROTTLE_PILOT=2, 
    	///<summary> per-motor compass calibration test | </summary>
        [Description("per-motor compass calibration test")]
        MOTOR_TEST_COMPASS_CAL=3, 
    
    };
    
    ///<summary>  </summary>
    public enum GPS_INPUT_IGNORE_FLAGS: ushort
    {
			///<summary> ignore altitude field | </summary>
        [Description("ignore altitude field")]
        GPS_INPUT_IGNORE_FLAG_ALT=1, 
    	///<summary> ignore hdop field | </summary>
        [Description("ignore hdop field")]
        GPS_INPUT_IGNORE_FLAG_HDOP=2, 
    	///<summary> ignore vdop field | </summary>
        [Description("ignore vdop field")]
        GPS_INPUT_IGNORE_FLAG_VDOP=4, 
    	///<summary> ignore horizontal velocity field (vn and ve) | </summary>
        [Description("ignore horizontal velocity field (vn and ve)")]
        GPS_INPUT_IGNORE_FLAG_VEL_HORIZ=8, 
    	///<summary> ignore vertical velocity field (vd) | </summary>
        [Description("ignore vertical velocity field (vd)")]
        GPS_INPUT_IGNORE_FLAG_VEL_VERT=16, 
    	///<summary> ignore speed accuracy field | </summary>
        [Description("ignore speed accuracy field")]
        GPS_INPUT_IGNORE_FLAG_SPEED_ACCURACY=32, 
    	///<summary> ignore horizontal accuracy field | </summary>
        [Description("ignore horizontal accuracy field")]
        GPS_INPUT_IGNORE_FLAG_HORIZONTAL_ACCURACY=64, 
    	///<summary> ignore vertical accuracy field | </summary>
        [Description("ignore vertical accuracy field")]
        GPS_INPUT_IGNORE_FLAG_VERTICAL_ACCURACY=128, 
    
    };
    
    ///<summary> Possible actions an aircraft can take to avoid a collision. </summary>
    public enum MAV_COLLISION_ACTION: byte
    {
			///<summary> Ignore any potential collisions | </summary>
        [Description("Ignore any potential collisions")]
        NONE=0, 
    	///<summary> Report potential collision | </summary>
        [Description("Report potential collision")]
        REPORT=1, 
    	///<summary> Ascend or Descend to avoid threat | </summary>
        [Description("Ascend or Descend to avoid threat")]
        ASCEND_OR_DESCEND=2, 
    	///<summary> Move horizontally to avoid threat | </summary>
        [Description("Move horizontally to avoid threat")]
        MOVE_HORIZONTALLY=3, 
    	///<summary> Aircraft to move perpendicular to the collision's velocity vector | </summary>
        [Description("Aircraft to move perpendicular to the collision's velocity vector")]
        MOVE_PERPENDICULAR=4, 
    	///<summary> Aircraft to fly directly back to its launch point | </summary>
        [Description("Aircraft to fly directly back to its launch point")]
        RTL=5, 
    	///<summary> Aircraft to stop in place | </summary>
        [Description("Aircraft to stop in place")]
        HOVER=6, 
    
    };
    
    ///<summary> Aircraft-rated danger from this threat. </summary>
    public enum MAV_COLLISION_THREAT_LEVEL: byte
    {
			///<summary> Not a threat | </summary>
        [Description("Not a threat")]
        NONE=0, 
    	///<summary> Craft is mildly concerned about this threat | </summary>
        [Description("Craft is mildly concerned about this threat")]
        LOW=1, 
    	///<summary> Craft is panicking, and may take actions to avoid threat | </summary>
        [Description("Craft is panicking, and may take actions to avoid threat")]
        HIGH=2, 
    
    };
    
    ///<summary> Source of information about this collision. </summary>
    public enum MAV_COLLISION_SRC: byte
    {
			///<summary> ID field references ADSB_VEHICLE packets | </summary>
        [Description("ID field references ADSB_VEHICLE packets")]
        ADSB=0, 
    	///<summary> ID field references MAVLink SRC ID | </summary>
        [Description("ID field references MAVLink SRC ID")]
        MAVLINK_GPS_GLOBAL_INT=1, 
    
    };
    
    ///<summary> Type of GPS fix </summary>
    public enum GPS_FIX_TYPE: byte
    {
			///<summary> No GPS connected | </summary>
        [Description("No GPS connected")]
        NO_GPS=0, 
    	///<summary> No position information, GPS is connected | </summary>
        [Description("No position information, GPS is connected")]
        NO_FIX=1, 
    	///<summary> 2D position | </summary>
        [Description("2D position")]
        _2D_FIX=2, 
    	///<summary> 3D position | </summary>
        [Description("3D position")]
        _3D_FIX=3, 
    	///<summary> DGPS/SBAS aided 3D position | </summary>
        [Description("DGPS/SBAS aided 3D position")]
        DGPS=4, 
    	///<summary> RTK float, 3D position | </summary>
        [Description("RTK float, 3D position")]
        RTK_FLOAT=5, 
    	///<summary> RTK Fixed, 3D position | </summary>
        [Description("RTK Fixed, 3D position")]
        RTK_FIXED=6, 
    	///<summary> Static fixed, typically used for base stations | </summary>
        [Description("Static fixed, typically used for base stations")]
        STATIC=7, 
    	///<summary> PPP, 3D position. | </summary>
        [Description("PPP, 3D position.")]
        PPP=8, 
    
    };
    
    ///<summary> RTK GPS baseline coordinate system, used for RTK corrections </summary>
    public enum RTK_BASELINE_COORDINATE_SYSTEM: byte
    {
			///<summary> Earth-centered, Earth-fixed | </summary>
        [Description("Earth-centered, Earth-fixed")]
        ECEF=0, 
    	///<summary> North, East, Down | </summary>
        [Description("North, East, Down")]
        NED=1, 
    
    };
    
    ///<summary> Type of landing target </summary>
    public enum LANDING_TARGET_TYPE: byte
    {
			///<summary> Landing target signaled by light beacon (ex: IR-LOCK) | </summary>
        [Description("Landing target signaled by light beacon (ex: IR-LOCK)")]
        LIGHT_BEACON=0, 
    	///<summary> Landing target signaled by radio beacon (ex: ILS, NDB) | </summary>
        [Description("Landing target signaled by radio beacon (ex: ILS, NDB)")]
        RADIO_BEACON=1, 
    	///<summary> Landing target represented by a fiducial marker (ex: ARTag) | </summary>
        [Description("Landing target represented by a fiducial marker (ex: ARTag)")]
        VISION_FIDUCIAL=2, 
    	///<summary> Landing target represented by a pre-defined visual shape/feature (ex: X-marker, H-marker, square) | </summary>
        [Description("Landing target represented by a pre-defined visual shape/feature (ex: X-marker, H-marker, square)")]
        VISION_OTHER=3, 
    
    };
    
    ///<summary> Direction of VTOL transition </summary>
    public enum VTOL_TRANSITION_HEADING: int /*default*/
    {
			///<summary> Respect the heading configuration of the vehicle. | </summary>
        [Description("Respect the heading configuration of the vehicle.")]
        VEHICLE_DEFAULT=0, 
    	///<summary> Use the heading pointing towards the next waypoint. | </summary>
        [Description("Use the heading pointing towards the next waypoint.")]
        NEXT_WAYPOINT=1, 
    	///<summary> Use the heading on takeoff (while sitting on the ground). | </summary>
        [Description("Use the heading on takeoff (while sitting on the ground).")]
        TAKEOFF=2, 
    	///<summary> Use the specified heading in parameter 4. | </summary>
        [Description("Use the specified heading in parameter 4.")]
        SPECIFIED=3, 
    	///<summary> Use the current heading when reaching takeoff altitude (potentially facing the wind when weather-vaning is active). | </summary>
        [Description("Use the current heading when reaching takeoff altitude (potentially facing the wind when weather-vaning is active).")]
        ANY=4, 
    
    };
    
    ///<summary> Camera capability flags (Bitmap) </summary>
    public enum CAMERA_CAP_FLAGS: uint
    {
			///<summary> Camera is able to record video | </summary>
        [Description("Camera is able to record video")]
        CAPTURE_VIDEO=1, 
    	///<summary> Camera is able to capture images | </summary>
        [Description("Camera is able to capture images")]
        CAPTURE_IMAGE=2, 
    	///<summary> Camera has separate Video and Image/Photo modes (MAV_CMD_SET_CAMERA_MODE) | </summary>
        [Description("Camera has separate Video and Image/Photo modes (MAV_CMD_SET_CAMERA_MODE)")]
        HAS_MODES=4, 
    	///<summary> Camera can capture images while in video mode | </summary>
        [Description("Camera can capture images while in video mode")]
        CAN_CAPTURE_IMAGE_IN_VIDEO_MODE=8, 
    	///<summary> Camera can capture videos while in Photo/Image mode | </summary>
        [Description("Camera can capture videos while in Photo/Image mode")]
        CAN_CAPTURE_VIDEO_IN_IMAGE_MODE=16, 
    	///<summary> Camera has image survey mode (MAV_CMD_SET_CAMERA_MODE) | </summary>
        [Description("Camera has image survey mode (MAV_CMD_SET_CAMERA_MODE)")]
        HAS_IMAGE_SURVEY_MODE=32, 
    	///<summary> Camera has basic zoom control (MAV_CMD_SET_CAMERA_ZOOM) | </summary>
        [Description("Camera has basic zoom control (MAV_CMD_SET_CAMERA_ZOOM)")]
        HAS_BASIC_ZOOM=64, 
    	///<summary> Camera has basic focus control (MAV_CMD_SET_CAMERA_FOCUS) | </summary>
        [Description("Camera has basic focus control (MAV_CMD_SET_CAMERA_FOCUS)")]
        HAS_BASIC_FOCUS=128, 
    	///<summary> Camera has video streaming capabilities (use MAV_CMD_REQUEST_VIDEO_STREAM_INFORMATION for video streaming info) | </summary>
        [Description("Camera has video streaming capabilities (use MAV_CMD_REQUEST_VIDEO_STREAM_INFORMATION for video streaming info)")]
        HAS_VIDEO_STREAM=256, 
    
    };
    
    ///<summary> Camera Modes. </summary>
    public enum CAMERA_MODE: byte
    {
			///<summary> Camera is in image/photo capture mode. | </summary>
        [Description("Camera is in image/photo capture mode.")]
        IMAGE=0, 
    	///<summary> Camera is in video capture mode. | </summary>
        [Description("Camera is in video capture mode.")]
        VIDEO=1, 
    	///<summary> Camera is in image survey capture mode. It allows for camera controller to do specific settings for surveys. | </summary>
        [Description("Camera is in image survey capture mode. It allows for camera controller to do specific settings for surveys.")]
        IMAGE_SURVEY=2, 
    
    };
    
    ///<summary>  </summary>
    public enum MAV_ARM_AUTH_DENIED_REASON: int /*default*/
    {
			///<summary> Not a specific reason | </summary>
        [Description("Not a specific reason")]
        GENERIC=0, 
    	///<summary> Authorizer will send the error as string to GCS | </summary>
        [Description("Authorizer will send the error as string to GCS")]
        NONE=1, 
    	///<summary> At least one waypoint have a invalid value | </summary>
        [Description("At least one waypoint have a invalid value")]
        INVALID_WAYPOINT=2, 
    	///<summary> Timeout in the authorizer process(in case it depends on network) | </summary>
        [Description("Timeout in the authorizer process(in case it depends on network)")]
        TIMEOUT=3, 
    	///<summary> Airspace of the mission in use by another vehicle, second result parameter can have the waypoint id that caused it to be denied. | </summary>
        [Description("Airspace of the mission in use by another vehicle, second result parameter can have the waypoint id that caused it to be denied.")]
        AIRSPACE_IN_USE=4, 
    	///<summary> Weather is not good to fly | </summary>
        [Description("Weather is not good to fly")]
        BAD_WEATHER=5, 
    
    };
    
    ///<summary> RC type </summary>
    public enum RC_TYPE: int /*default*/
    {
			///<summary> Spektrum DSM2 | </summary>
        [Description("Spektrum DSM2")]
        SPEKTRUM_DSM2=0, 
    	///<summary> Spektrum DSMX | </summary>
        [Description("Spektrum DSMX")]
        SPEKTRUM_DSMX=1, 
    
    };
    
    ///<summary> Bitmap to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 9 is set the floats afx afy afz should be interpreted as force instead of acceleration. </summary>
    public enum POSITION_TARGET_TYPEMASK: ushort
    {
			///<summary> Ignore position x | </summary>
        [Description("Ignore position x")]
        X_IGNORE=1, 
    	///<summary> Ignore position y | </summary>
        [Description("Ignore position y")]
        Y_IGNORE=2, 
    	///<summary> Ignore position z | </summary>
        [Description("Ignore position z")]
        Z_IGNORE=4, 
    	///<summary> Ignore velocity x | </summary>
        [Description("Ignore velocity x")]
        VX_IGNORE=8, 
    	///<summary> Ignore velocity y | </summary>
        [Description("Ignore velocity y")]
        VY_IGNORE=16, 
    	///<summary> Ignore velocity z | </summary>
        [Description("Ignore velocity z")]
        VZ_IGNORE=32, 
    	///<summary> Ignore acceleration x | </summary>
        [Description("Ignore acceleration x")]
        AX_IGNORE=64, 
    	///<summary> Ignore acceleration y | </summary>
        [Description("Ignore acceleration y")]
        AY_IGNORE=128, 
    	///<summary> Ignore acceleration z | </summary>
        [Description("Ignore acceleration z")]
        AZ_IGNORE=256, 
    	///<summary> Use force instead of acceleration | </summary>
        [Description("Use force instead of acceleration")]
        FORCE_SET=512, 
    	///<summary> Ignore yaw | </summary>
        [Description("Ignore yaw")]
        YAW_IGNORE=1024, 
    	///<summary> Ignore yaw rate | </summary>
        [Description("Ignore yaw rate")]
        YAW_RATE_IGNORE=2048, 
    
    };
    
    ///<summary> Precision land modes (used in MAV_CMD_NAV_LAND). </summary>
    public enum PRECISION_LAND_MODE: int /*default*/
    {
			///<summary> Normal (non-precision) landing. | </summary>
        [Description("Normal (non-precision) landing.")]
        DISABLED=0, 
    	///<summary> Use precision landing if beacon detected when land command accepted, otherwise land normally. | </summary>
        [Description("Use precision landing if beacon detected when land command accepted, otherwise land normally.")]
        OPPORTUNISTIC=1, 
    	///<summary> Use precision landing, searching for beacon if not found when land command accepted (land normally if beacon cannot be found). | </summary>
        [Description("Use precision landing, searching for beacon if not found when land command accepted (land normally if beacon cannot be found).")]
        REQUIRED=2, 
    
    };
    
    
    ///<summary> State flags for ADS-B transponder dynamic report </summary>
    public enum UAVIONIX_ADSB_OUT_DYNAMIC_STATE: ushort
    {
			///<summary>  | </summary>
        [Description("")]
        INTENT_CHANGE=1, 
    	///<summary>  | </summary>
        [Description("")]
        AUTOPILOT_ENABLED=2, 
    	///<summary>  | </summary>
        [Description("")]
        NICBARO_CROSSCHECKED=4, 
    	///<summary>  | </summary>
        [Description("")]
        ON_GROUND=8, 
    	///<summary>  | </summary>
        [Description("")]
        IDENT=16, 
    
    };
    
    ///<summary> Transceiver RF control flags for ADS-B transponder dynamic reports </summary>
    public enum UAVIONIX_ADSB_OUT_RF_SELECT: byte
    {
			///<summary>  | </summary>
        [Description("")]
        STANDBY=0, 
    	///<summary>  | </summary>
        [Description("")]
        RX_ENABLED=1, 
    	///<summary>  | </summary>
        [Description("")]
        TX_ENABLED=2, 
    
    };
    
    ///<summary> Status for ADS-B transponder dynamic input </summary>
    public enum UAVIONIX_ADSB_OUT_DYNAMIC_GPS_FIX: byte
    {
			///<summary>  | </summary>
        [Description("")]
        NONE_0=0, 
    	///<summary>  | </summary>
        [Description("")]
        NONE_1=1, 
    	///<summary>  | </summary>
        [Description("")]
        _2D=2, 
    	///<summary>  | </summary>
        [Description("")]
        _3D=3, 
    	///<summary>  | </summary>
        [Description("")]
        DGPS=4, 
    	///<summary>  | </summary>
        [Description("")]
        RTK=5, 
    
    };
    
    ///<summary> Status flags for ADS-B transponder dynamic output </summary>
    public enum UAVIONIX_ADSB_RF_HEALTH: byte
    {
			///<summary>  | </summary>
        [Description("")]
        INITIALIZING=0, 
    	///<summary>  | </summary>
        [Description("")]
        OK=1, 
    	///<summary>  | </summary>
        [Description("")]
        FAIL_TX=2, 
    	///<summary>  | </summary>
        [Description("")]
        FAIL_RX=16, 
    
    };
    
    ///<summary> Definitions for aircraft size </summary>
    public enum UAVIONIX_ADSB_OUT_CFG_AIRCRAFT_SIZE: byte
    {
			///<summary>  | </summary>
        [Description("")]
        NO_DATA=0, 
    	///<summary>  | </summary>
        [Description("")]
        L15M_W23M=1, 
    	///<summary>  | </summary>
        [Description("")]
        L25M_W28P5M=2, 
    	///<summary>  | </summary>
        [Description("")]
        L25_34M=3, 
    	///<summary>  | </summary>
        [Description("")]
        L35_33M=4, 
    	///<summary>  | </summary>
        [Description("")]
        L35_38M=5, 
    	///<summary>  | </summary>
        [Description("")]
        L45_39P5M=6, 
    	///<summary>  | </summary>
        [Description("")]
        L45_45M=7, 
    	///<summary>  | </summary>
        [Description("")]
        L55_45M=8, 
    	///<summary>  | </summary>
        [Description("")]
        L55_52M=9, 
    	///<summary>  | </summary>
        [Description("")]
        L65_59P5M=10, 
    	///<summary>  | </summary>
        [Description("")]
        L65_67M=11, 
    	///<summary>  | </summary>
        [Description("")]
        L75_W72P5M=12, 
    	///<summary>  | </summary>
        [Description("")]
        L75_W80M=13, 
    	///<summary>  | </summary>
        [Description("")]
        L85_W80M=14, 
    	///<summary>  | </summary>
        [Description("")]
        L85_W90M=15, 
    
    };
    
    ///<summary> GPS lataral offset encoding </summary>
    public enum UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LAT: byte
    {
			///<summary>  | </summary>
        [Description("")]
        NO_DATA=0, 
    	///<summary>  | </summary>
        [Description("")]
        LEFT_2M=1, 
    	///<summary>  | </summary>
        [Description("")]
        LEFT_4M=2, 
    	///<summary>  | </summary>
        [Description("")]
        LEFT_6M=3, 
    	///<summary>  | </summary>
        [Description("")]
        RIGHT_0M=4, 
    	///<summary>  | </summary>
        [Description("")]
        RIGHT_2M=5, 
    	///<summary>  | </summary>
        [Description("")]
        RIGHT_4M=6, 
    	///<summary>  | </summary>
        [Description("")]
        RIGHT_6M=7, 
    
    };
    
    ///<summary> GPS longitudinal offset encoding </summary>
    public enum UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LON: byte
    {
			///<summary>  | </summary>
        [Description("")]
        NO_DATA=0, 
    	///<summary>  | </summary>
        [Description("")]
        APPLIED_BY_SENSOR=1, 
    
    };
    
    ///<summary> Emergency status encoding </summary>
    public enum UAVIONIX_ADSB_EMERGENCY_STATUS: byte
    {
			///<summary>  | </summary>
        [Description("")]
        UAVIONIX_ADSB_OUT_NO_EMERGENCY=0, 
    	///<summary>  | </summary>
        [Description("")]
        UAVIONIX_ADSB_OUT_GENERAL_EMERGENCY=1, 
    	///<summary>  | </summary>
        [Description("")]
        UAVIONIX_ADSB_OUT_LIFEGUARD_EMERGENCY=2, 
    	///<summary>  | </summary>
        [Description("")]
        UAVIONIX_ADSB_OUT_MINIMUM_FUEL_EMERGENCY=3, 
    	///<summary>  | </summary>
        [Description("")]
        UAVIONIX_ADSB_OUT_NO_COMM_EMERGENCY=4, 
    	///<summary>  | </summary>
        [Description("")]
        UAVIONIX_ADSB_OUT_UNLAWFUL_INTERFERANCE_EMERGENCY=5, 
    	///<summary>  | </summary>
        [Description("")]
        UAVIONIX_ADSB_OUT_DOWNED_AIRCRAFT_EMERGENCY=6, 
    	///<summary>  | </summary>
        [Description("")]
        UAVIONIX_ADSB_OUT_RESERVED=7, 
    
    };
    
    
    ///<summary>  </summary>
    public enum ICAROUS_TRACK_BAND_TYPES: byte
    {
			///<summary>  | </summary>
        [Description("")]
        ICAROUS_TRACK_BAND_TYPE_NONE=0, 
    	///<summary>  | </summary>
        [Description("")]
        ICAROUS_TRACK_BAND_TYPE_NEAR=1, 
    	///<summary>  | </summary>
        [Description("")]
        ICAROUS_TRACK_BAND_TYPE_RECOVERY=2, 
    
    };
    
    ///<summary>  </summary>
    public enum ICAROUS_FMS_STATE: byte
    {
			///<summary>  | </summary>
        [Description("")]
        IDLE=0, 
    	///<summary>  | </summary>
        [Description("")]
        TAKEOFF=1, 
    	///<summary>  | </summary>
        [Description("")]
        CLIMB=2, 
    	///<summary>  | </summary>
        [Description("")]
        CRUISE=3, 
    	///<summary>  | </summary>
        [Description("")]
        APPROACH=4, 
    	///<summary>  | </summary>
        [Description("")]
        LAND=5, 
    
    };
    

    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> Offsets and calibrations values for hardware sensors. This makes it easier to debug the calibration process. </summary>
    public struct mavlink_sensor_offsets_t
    {
        /// <summary>Magnetic declination.  [rad] </summary>
        [Units("[rad]")]
        [Description("Magnetic declination.")]
        public  float mag_declination;
            /// <summary>Raw pressure from barometer.   </summary>
        [Units("")]
        [Description("Raw pressure from barometer.")]
        public  int raw_press;
            /// <summary>Raw temperature from barometer.   </summary>
        [Units("")]
        [Description("Raw temperature from barometer.")]
        public  int raw_temp;
            /// <summary>Gyro X calibration.   </summary>
        [Units("")]
        [Description("Gyro X calibration.")]
        public  float gyro_cal_x;
            /// <summary>Gyro Y calibration.   </summary>
        [Units("")]
        [Description("Gyro Y calibration.")]
        public  float gyro_cal_y;
            /// <summary>Gyro Z calibration.   </summary>
        [Units("")]
        [Description("Gyro Z calibration.")]
        public  float gyro_cal_z;
            /// <summary>Accel X calibration.   </summary>
        [Units("")]
        [Description("Accel X calibration.")]
        public  float accel_cal_x;
            /// <summary>Accel Y calibration.   </summary>
        [Units("")]
        [Description("Accel Y calibration.")]
        public  float accel_cal_y;
            /// <summary>Accel Z calibration.   </summary>
        [Units("")]
        [Description("Accel Z calibration.")]
        public  float accel_cal_z;
            /// <summary>Magnetometer X offset.   </summary>
        [Units("")]
        [Description("Magnetometer X offset.")]
        public  short mag_ofs_x;
            /// <summary>Magnetometer Y offset.   </summary>
        [Units("")]
        [Description("Magnetometer Y offset.")]
        public  short mag_ofs_y;
            /// <summary>Magnetometer Z offset.   </summary>
        [Units("")]
        [Description("Magnetometer Z offset.")]
        public  short mag_ofs_z;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> Set the magnetometer offsets </summary>
    public struct mavlink_set_mag_offsets_t
    {
        /// <summary>Magnetometer X offset.   </summary>
        [Units("")]
        [Description("Magnetometer X offset.")]
        public  short mag_ofs_x;
            /// <summary>Magnetometer Y offset.   </summary>
        [Units("")]
        [Description("Magnetometer Y offset.")]
        public  short mag_ofs_y;
            /// <summary>Magnetometer Z offset.   </summary>
        [Units("")]
        [Description("Magnetometer Z offset.")]
        public  short mag_ofs_z;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> State of APM memory. </summary>
    public struct mavlink_meminfo_t
    {
        /// <summary>Heap top.   </summary>
        [Units("")]
        [Description("Heap top.")]
        public  ushort brkval;
            /// <summary>Free memory.  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Free memory.")]
        public  ushort freemem;
            /// <summary>Free memory (32 bit).  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Free memory (32 bit).")]
        public  uint freemem32;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> Raw ADC output. </summary>
    public struct mavlink_ap_adc_t
    {
        /// <summary>ADC output 1.   </summary>
        [Units("")]
        [Description("ADC output 1.")]
        public  ushort adc1;
            /// <summary>ADC output 2.   </summary>
        [Units("")]
        [Description("ADC output 2.")]
        public  ushort adc2;
            /// <summary>ADC output 3.   </summary>
        [Units("")]
        [Description("ADC output 3.")]
        public  ushort adc3;
            /// <summary>ADC output 4.   </summary>
        [Units("")]
        [Description("ADC output 4.")]
        public  ushort adc4;
            /// <summary>ADC output 5.   </summary>
        [Units("")]
        [Description("ADC output 5.")]
        public  ushort adc5;
            /// <summary>ADC output 6.   </summary>
        [Units("")]
        [Description("ADC output 6.")]
        public  ushort adc6;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=15)]
    ///<summary> Configure on-board Camera Control System. </summary>
    public struct mavlink_digicam_configure_t
    {
        /// <summary>Correspondent value to given extra_param.   </summary>
        [Units("")]
        [Description("Correspondent value to given extra_param.")]
        public  float extra_value;
            /// <summary>Divisor number //e.g. 1000 means 1/1000 (0 means ignore).   </summary>
        [Units("")]
        [Description("Divisor number //e.g. 1000 means 1/1000 (0 means ignore).")]
        public  ushort shutter_speed;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Mode enumeration from 1 to N //P, TV, AV, M, etc. (0 means ignore).   </summary>
        [Units("")]
        [Description("Mode enumeration from 1 to N //P, TV, AV, M, etc. (0 means ignore).")]
        public  byte mode;
            /// <summary>F stop number x 10 //e.g. 28 means 2.8 (0 means ignore).   </summary>
        [Units("")]
        [Description("F stop number x 10 //e.g. 28 means 2.8 (0 means ignore).")]
        public  byte aperture;
            /// <summary>ISO enumeration from 1 to N //e.g. 80, 100, 200, Etc (0 means ignore).   </summary>
        [Units("")]
        [Description("ISO enumeration from 1 to N //e.g. 80, 100, 200, Etc (0 means ignore).")]
        public  byte iso;
            /// <summary>Exposure type enumeration from 1 to N (0 means ignore).   </summary>
        [Units("")]
        [Description("Exposure type enumeration from 1 to N (0 means ignore).")]
        public  byte exposure_type;
            /// <summary>Command Identity (incremental loop: 0 to 255). //A command sent multiple times will be executed or pooled just once.   </summary>
        [Units("")]
        [Description("Command Identity (incremental loop: 0 to 255). //A command sent multiple times will be executed or pooled just once.")]
        public  byte command_id;
            /// <summary>Main engine cut-off time before camera trigger (0 means no cut-off).  [ds] </summary>
        [Units("[ds]")]
        [Description("Main engine cut-off time before camera trigger (0 means no cut-off).")]
        public  byte engine_cut_off;
            /// <summary>Extra parameters enumeration (0 means ignore).   </summary>
        [Units("")]
        [Description("Extra parameters enumeration (0 means ignore).")]
        public  byte extra_param;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=13)]
    ///<summary> Control on-board Camera Control System to take shots. </summary>
    public struct mavlink_digicam_control_t
    {
        /// <summary>Correspondent value to given extra_param.   </summary>
        [Units("")]
        [Description("Correspondent value to given extra_param.")]
        public  float extra_value;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>0: stop, 1: start or keep it up //Session control e.g. show/hide lens.   </summary>
        [Units("")]
        [Description("0: stop, 1: start or keep it up //Session control e.g. show/hide lens.")]
        public  byte session;
            /// <summary>1 to N //Zoom's absolute position (0 means ignore).   </summary>
        [Units("")]
        [Description("1 to N //Zoom's absolute position (0 means ignore).")]
        public  byte zoom_pos;
            /// <summary>-100 to 100 //Zooming step value to offset zoom from the current position.   </summary>
        [Units("")]
        [Description("-100 to 100 //Zooming step value to offset zoom from the current position.")]
        public  byte zoom_step;
            /// <summary>0: unlock focus or keep unlocked, 1: lock focus or keep locked, 3: re-lock focus.   </summary>
        [Units("")]
        [Description("0: unlock focus or keep unlocked, 1: lock focus or keep locked, 3: re-lock focus.")]
        public  byte focus_lock;
            /// <summary>0: ignore, 1: shot or start filming.   </summary>
        [Units("")]
        [Description("0: ignore, 1: shot or start filming.")]
        public  byte shot;
            /// <summary>Command Identity (incremental loop: 0 to 255)//A command sent multiple times will be executed or pooled just once.   </summary>
        [Units("")]
        [Description("Command Identity (incremental loop: 0 to 255)//A command sent multiple times will be executed or pooled just once.")]
        public  byte command_id;
            /// <summary>Extra parameters enumeration (0 means ignore).   </summary>
        [Units("")]
        [Description("Extra parameters enumeration (0 means ignore).")]
        public  byte extra_param;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Message to configure a camera mount, directional antenna, etc. </summary>
    public struct mavlink_mount_configure_t
    {
        /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Mount operating mode. MAV_MOUNT_MODE  </summary>
        [Units("")]
        [Description("Mount operating mode.")]
        public  /*MAV_MOUNT_MODE*/byte mount_mode;
            /// <summary>(1 = yes, 0 = no).   </summary>
        [Units("")]
        [Description("(1 = yes, 0 = no).")]
        public  byte stab_roll;
            /// <summary>(1 = yes, 0 = no).   </summary>
        [Units("")]
        [Description("(1 = yes, 0 = no).")]
        public  byte stab_pitch;
            /// <summary>(1 = yes, 0 = no).   </summary>
        [Units("")]
        [Description("(1 = yes, 0 = no).")]
        public  byte stab_yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=15)]
    ///<summary> Message to control a camera mount, directional antenna, etc. </summary>
    public struct mavlink_mount_control_t
    {
        /// <summary>Pitch (centi-degrees) or lat (degE7), depending on mount mode.   </summary>
        [Units("")]
        [Description("Pitch (centi-degrees) or lat (degE7), depending on mount mode.")]
        public  int input_a;
            /// <summary>Roll (centi-degrees) or lon (degE7) depending on mount mode.   </summary>
        [Units("")]
        [Description("Roll (centi-degrees) or lon (degE7) depending on mount mode.")]
        public  int input_b;
            /// <summary>Yaw (centi-degrees) or alt (cm) depending on mount mode.   </summary>
        [Units("")]
        [Description("Yaw (centi-degrees) or alt (cm) depending on mount mode.")]
        public  int input_c;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>If '1' it will save current trimmed position on EEPROM (just valid for NEUTRAL and LANDING).   </summary>
        [Units("")]
        [Description("If '1' it will save current trimmed position on EEPROM (just valid for NEUTRAL and LANDING).")]
        public  byte save_position;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Message with some status from APM to GCS about camera or antenna mount. </summary>
    public struct mavlink_mount_status_t
    {
        /// <summary>Pitch.  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("Pitch.")]
        public  int pointing_a;
            /// <summary>Roll.  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("Roll.")]
        public  int pointing_b;
            /// <summary>Yaw.  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("Yaw.")]
        public  int pointing_c;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> A fence point. Used to set a point when from GCS -> MAV. Also used to return a point from MAV -> GCS. </summary>
    public struct mavlink_fence_point_t
    {
        /// <summary>Latitude of point.  [deg] </summary>
        [Units("[deg]")]
        [Description("Latitude of point.")]
        public  float lat;
            /// <summary>Longitude of point.  [deg] </summary>
        [Units("[deg]")]
        [Description("Longitude of point.")]
        public  float lng;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Point index (first point is 1, 0 is for return point).   </summary>
        [Units("")]
        [Description("Point index (first point is 1, 0 is for return point).")]
        public  byte idx;
            /// <summary>Total number of points (for sanity checking).   </summary>
        [Units("")]
        [Description("Total number of points (for sanity checking).")]
        public  byte count;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Request a current fence point from MAV. </summary>
    public struct mavlink_fence_fetch_point_t
    {
        /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Point index (first point is 1, 0 is for return point).   </summary>
        [Units("")]
        [Description("Point index (first point is 1, 0 is for return point).")]
        public  byte idx;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> Status of geo-fencing. Sent in extended status stream when fencing enabled. </summary>
    public struct mavlink_fence_status_t
    {
        /// <summary>Time (since boot) of last breach.  [ms] </summary>
        [Units("[ms]")]
        [Description("Time (since boot) of last breach.")]
        public  uint breach_time;
            /// <summary>Number of fence breaches.   </summary>
        [Units("")]
        [Description("Number of fence breaches.")]
        public  ushort breach_count;
            /// <summary>Breach status (0 if currently inside fence, 1 if outside).   </summary>
        [Units("")]
        [Description("Breach status (0 if currently inside fence, 1 if outside).")]
        public  byte breach_status;
            /// <summary>Last breach type. FENCE_BREACH  </summary>
        [Units("")]
        [Description("Last breach type.")]
        public  /*FENCE_BREACH*/byte breach_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> Status of DCM attitude estimator. </summary>
    public struct mavlink_ahrs_t
    {
        /// <summary>X gyro drift estimate.  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("X gyro drift estimate.")]
        public  float omegaIx;
            /// <summary>Y gyro drift estimate.  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Y gyro drift estimate.")]
        public  float omegaIy;
            /// <summary>Z gyro drift estimate.  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Z gyro drift estimate.")]
        public  float omegaIz;
            /// <summary>Average accel_weight.   </summary>
        [Units("")]
        [Description("Average accel_weight.")]
        public  float accel_weight;
            /// <summary>Average renormalisation value.   </summary>
        [Units("")]
        [Description("Average renormalisation value.")]
        public  float renorm_val;
            /// <summary>Average error_roll_pitch value.   </summary>
        [Units("")]
        [Description("Average error_roll_pitch value.")]
        public  float error_rp;
            /// <summary>Average error_yaw value.   </summary>
        [Units("")]
        [Description("Average error_yaw value.")]
        public  float error_yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> Status of simulation environment, if used. </summary>
    public struct mavlink_simstate_t
    {
        /// <summary>Roll angle.  [rad] </summary>
        [Units("[rad]")]
        [Description("Roll angle.")]
        public  float roll;
            /// <summary>Pitch angle.  [rad] </summary>
        [Units("[rad]")]
        [Description("Pitch angle.")]
        public  float pitch;
            /// <summary>Yaw angle.  [rad] </summary>
        [Units("[rad]")]
        [Description("Yaw angle.")]
        public  float yaw;
            /// <summary>X acceleration.  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X acceleration.")]
        public  float xacc;
            /// <summary>Y acceleration.  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y acceleration.")]
        public  float yacc;
            /// <summary>Z acceleration.  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z acceleration.")]
        public  float zacc;
            /// <summary>Angular speed around X axis.  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around X axis.")]
        public  float xgyro;
            /// <summary>Angular speed around Y axis.  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around Y axis.")]
        public  float ygyro;
            /// <summary>Angular speed around Z axis.  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around Z axis.")]
        public  float zgyro;
            /// <summary>Latitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude.")]
        public  int lat;
            /// <summary>Longitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude.")]
        public  int lng;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Status of key hardware. </summary>
    public struct mavlink_hwstatus_t
    {
        /// <summary>Board voltage.  [mV] </summary>
        [Units("[mV]")]
        [Description("Board voltage.")]
        public  ushort Vcc;
            /// <summary>I2C error count.   </summary>
        [Units("")]
        [Description("I2C error count.")]
        public  byte I2Cerr;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> Status generated by radio. </summary>
    public struct mavlink_radio_t
    {
        /// <summary>Receive errors.   </summary>
        [Units("")]
        [Description("Receive errors.")]
        public  ushort rxerrors;
            /// <summary>Count of error corrected packets.   </summary>
        [Units("")]
        [Description("Count of error corrected packets.")]
        public  ushort @fixed;
            /// <summary>Local signal strength.   </summary>
        [Units("")]
        [Description("Local signal strength.")]
        public  byte rssi;
            /// <summary>Remote signal strength.   </summary>
        [Units("")]
        [Description("Remote signal strength.")]
        public  byte remrssi;
            /// <summary>How full the tx buffer is.  [%] </summary>
        [Units("[%]")]
        [Description("How full the tx buffer is.")]
        public  byte txbuf;
            /// <summary>Background noise level.   </summary>
        [Units("")]
        [Description("Background noise level.")]
        public  byte noise;
            /// <summary>Remote background noise level.   </summary>
        [Units("")]
        [Description("Remote background noise level.")]
        public  byte remnoise;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> Status of AP_Limits. Sent in extended status stream when AP_Limits is enabled. </summary>
    public struct mavlink_limits_status_t
    {
        /// <summary>Time (since boot) of last breach.  [ms] </summary>
        [Units("[ms]")]
        [Description("Time (since boot) of last breach.")]
        public  uint last_trigger;
            /// <summary>Time (since boot) of last recovery action.  [ms] </summary>
        [Units("[ms]")]
        [Description("Time (since boot) of last recovery action.")]
        public  uint last_action;
            /// <summary>Time (since boot) of last successful recovery.  [ms] </summary>
        [Units("[ms]")]
        [Description("Time (since boot) of last successful recovery.")]
        public  uint last_recovery;
            /// <summary>Time (since boot) of last all-clear.  [ms] </summary>
        [Units("[ms]")]
        [Description("Time (since boot) of last all-clear.")]
        public  uint last_clear;
            /// <summary>Number of fence breaches.   </summary>
        [Units("")]
        [Description("Number of fence breaches.")]
        public  ushort breach_count;
            /// <summary>State of AP_Limits. LIMITS_STATE  </summary>
        [Units("")]
        [Description("State of AP_Limits.")]
        public  /*LIMITS_STATE*/byte limits_state;
            /// <summary>AP_Limit_Module bitfield of enabled modules. LIMIT_MODULE  bitmask</summary>
        [Units("")]
        [Description("AP_Limit_Module bitfield of enabled modules.")]
        public  /*LIMIT_MODULE*/byte mods_enabled;
            /// <summary>AP_Limit_Module bitfield of required modules. LIMIT_MODULE  bitmask</summary>
        [Units("")]
        [Description("AP_Limit_Module bitfield of required modules.")]
        public  /*LIMIT_MODULE*/byte mods_required;
            /// <summary>AP_Limit_Module bitfield of triggered modules. LIMIT_MODULE  bitmask</summary>
        [Units("")]
        [Description("AP_Limit_Module bitfield of triggered modules.")]
        public  /*LIMIT_MODULE*/byte mods_triggered;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> Wind estimation. </summary>
    public struct mavlink_wind_t
    {
        /// <summary>Wind direction (that wind is coming from).  [deg] </summary>
        [Units("[deg]")]
        [Description("Wind direction (that wind is coming from).")]
        public  float direction;
            /// <summary>Wind speed in ground plane.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Wind speed in ground plane.")]
        public  float speed;
            /// <summary>Vertical wind speed.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Vertical wind speed.")]
        public  float speed_z;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    ///<summary> Data packet, size 16. </summary>
    public struct mavlink_data16_t
    {
        /// <summary>Data type.   </summary>
        [Units("")]
        [Description("Data type.")]
        public  byte type;
            /// <summary>Data length.  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Data length.")]
        public  byte len;
            /// <summary>Raw data.   </summary>
        [Units("")]
        [Description("Raw data.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=34)]
    ///<summary> Data packet, size 32. </summary>
    public struct mavlink_data32_t
    {
        /// <summary>Data type.   </summary>
        [Units("")]
        [Description("Data type.")]
        public  byte type;
            /// <summary>Data length.  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Data length.")]
        public  byte len;
            /// <summary>Raw data.   </summary>
        [Units("")]
        [Description("Raw data.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=66)]
    ///<summary> Data packet, size 64. </summary>
    public struct mavlink_data64_t
    {
        /// <summary>Data type.   </summary>
        [Units("")]
        [Description("Data type.")]
        public  byte type;
            /// <summary>Data length.  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Data length.")]
        public  byte len;
            /// <summary>Raw data.   </summary>
        [Units("")]
        [Description("Raw data.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=64)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=98)]
    ///<summary> Data packet, size 96. </summary>
    public struct mavlink_data96_t
    {
        /// <summary>Data type.   </summary>
        [Units("")]
        [Description("Data type.")]
        public  byte type;
            /// <summary>Data length.  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Data length.")]
        public  byte len;
            /// <summary>Raw data.   </summary>
        [Units("")]
        [Description("Raw data.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=96)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> Rangefinder reporting. </summary>
    public struct mavlink_rangefinder_t
    {
        /// <summary>Distance.  [m] </summary>
        [Units("[m]")]
        [Description("Distance.")]
        public  float distance;
            /// <summary>Raw voltage if available, zero otherwise.  [V] </summary>
        [Units("[V]")]
        [Description("Raw voltage if available, zero otherwise.")]
        public  float voltage;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=48)]
    ///<summary> Airspeed auto-calibration. </summary>
    public struct mavlink_airspeed_autocal_t
    {
        /// <summary>GPS velocity north.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("GPS velocity north.")]
        public  float vx;
            /// <summary>GPS velocity east.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("GPS velocity east.")]
        public  float vy;
            /// <summary>GPS velocity down.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("GPS velocity down.")]
        public  float vz;
            /// <summary>Differential pressure.  [Pa] </summary>
        [Units("[Pa]")]
        [Description("Differential pressure.")]
        public  float diff_pressure;
            /// <summary>Estimated to true airspeed ratio.   </summary>
        [Units("")]
        [Description("Estimated to true airspeed ratio.")]
        public  float EAS2TAS;
            /// <summary>Airspeed ratio.   </summary>
        [Units("")]
        [Description("Airspeed ratio.")]
        public  float ratio;
            /// <summary>EKF state x.   </summary>
        [Units("")]
        [Description("EKF state x.")]
        public  float state_x;
            /// <summary>EKF state y.   </summary>
        [Units("")]
        [Description("EKF state y.")]
        public  float state_y;
            /// <summary>EKF state z.   </summary>
        [Units("")]
        [Description("EKF state z.")]
        public  float state_z;
            /// <summary>EKF Pax.   </summary>
        [Units("")]
        [Description("EKF Pax.")]
        public  float Pax;
            /// <summary>EKF Pby.   </summary>
        [Units("")]
        [Description("EKF Pby.")]
        public  float Pby;
            /// <summary>EKF Pcz.   </summary>
        [Units("")]
        [Description("EKF Pcz.")]
        public  float Pcz;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=19)]
    ///<summary> A rally point. Used to set a point when from GCS -> MAV. Also used to return a point from MAV -> GCS. </summary>
    public struct mavlink_rally_point_t
    {
        /// <summary>Latitude of point.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude of point.")]
        public  int lat;
            /// <summary>Longitude of point.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude of point.")]
        public  int lng;
            /// <summary>Transit / loiter altitude relative to home.  [m] </summary>
        [Units("[m]")]
        [Description("Transit / loiter altitude relative to home.")]
        public  short alt;
            /// <summary>Break altitude relative to home.  [m] </summary>
        [Units("[m]")]
        [Description("Break altitude relative to home.")]
        public  short break_alt;
            /// <summary>Heading to aim for when landing.  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("Heading to aim for when landing.")]
        public  ushort land_dir;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Point index (first point is 0).   </summary>
        [Units("")]
        [Description("Point index (first point is 0).")]
        public  byte idx;
            /// <summary>Total number of points (for sanity checking).   </summary>
        [Units("")]
        [Description("Total number of points (for sanity checking).")]
        public  byte count;
            /// <summary>Configuration flags. RALLY_FLAGS  bitmask</summary>
        [Units("")]
        [Description("Configuration flags.")]
        public  /*RALLY_FLAGS*/byte flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Request a current rally point from MAV. MAV should respond with a RALLY_POINT message. MAV should not respond if the request is invalid. </summary>
    public struct mavlink_rally_fetch_point_t
    {
        /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Point index (first point is 0).   </summary>
        [Units("")]
        [Description("Point index (first point is 0).")]
        public  byte idx;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Status of compassmot calibration. </summary>
    public struct mavlink_compassmot_status_t
    {
        /// <summary>Current.  [A] </summary>
        [Units("[A]")]
        [Description("Current.")]
        public  float current;
            /// <summary>Motor Compensation X.   </summary>
        [Units("")]
        [Description("Motor Compensation X.")]
        public  float CompensationX;
            /// <summary>Motor Compensation Y.   </summary>
        [Units("")]
        [Description("Motor Compensation Y.")]
        public  float CompensationY;
            /// <summary>Motor Compensation Z.   </summary>
        [Units("")]
        [Description("Motor Compensation Z.")]
        public  float CompensationZ;
            /// <summary>Throttle.  [d%] </summary>
        [Units("[d%]")]
        [Description("Throttle.")]
        public  ushort throttle;
            /// <summary>Interference.  [%] </summary>
        [Units("[%]")]
        [Description("Interference.")]
        public  ushort interference;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=24)]
    ///<summary> Status of secondary AHRS filter if available. </summary>
    public struct mavlink_ahrs2_t
    {
        /// <summary>Roll angle.  [rad] </summary>
        [Units("[rad]")]
        [Description("Roll angle.")]
        public  float roll;
            /// <summary>Pitch angle.  [rad] </summary>
        [Units("[rad]")]
        [Description("Pitch angle.")]
        public  float pitch;
            /// <summary>Yaw angle.  [rad] </summary>
        [Units("[rad]")]
        [Description("Yaw angle.")]
        public  float yaw;
            /// <summary>Altitude (MSL).  [m] </summary>
        [Units("[m]")]
        [Description("Altitude (MSL).")]
        public  float altitude;
            /// <summary>Latitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude.")]
        public  int lat;
            /// <summary>Longitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude.")]
        public  int lng;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=29)]
    ///<summary> Camera Event. </summary>
    public struct mavlink_camera_status_t
    {
        /// <summary>Image timestamp (since UNIX epoch, according to camera clock).  [us] </summary>
        [Units("[us]")]
        [Description("Image timestamp (since UNIX epoch, according to camera clock).")]
        public  ulong time_usec;
            /// <summary>Parameter 1 (meaning depends on event_id, see CAMERA_STATUS_TYPES enum).   </summary>
        [Units("")]
        [Description("Parameter 1 (meaning depends on event_id, see CAMERA_STATUS_TYPES enum).")]
        public  float p1;
            /// <summary>Parameter 2 (meaning depends on event_id, see CAMERA_STATUS_TYPES enum).   </summary>
        [Units("")]
        [Description("Parameter 2 (meaning depends on event_id, see CAMERA_STATUS_TYPES enum).")]
        public  float p2;
            /// <summary>Parameter 3 (meaning depends on event_id, see CAMERA_STATUS_TYPES enum).   </summary>
        [Units("")]
        [Description("Parameter 3 (meaning depends on event_id, see CAMERA_STATUS_TYPES enum).")]
        public  float p3;
            /// <summary>Parameter 4 (meaning depends on event_id, see CAMERA_STATUS_TYPES enum).   </summary>
        [Units("")]
        [Description("Parameter 4 (meaning depends on event_id, see CAMERA_STATUS_TYPES enum).")]
        public  float p4;
            /// <summary>Image index.   </summary>
        [Units("")]
        [Description("Image index.")]
        public  ushort img_idx;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Camera ID.   </summary>
        [Units("")]
        [Description("Camera ID.")]
        public  byte cam_idx;
            /// <summary>Event type. CAMERA_STATUS_TYPES  </summary>
        [Units("")]
        [Description("Event type.")]
        public  /*CAMERA_STATUS_TYPES*/byte event_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=47)]
    ///<summary> Camera Capture Feedback. </summary>
    public struct mavlink_camera_feedback_t
    {
        /// <summary>Image timestamp (since UNIX epoch), as passed in by CAMERA_STATUS message (or autopilot if no CCB).  [us] </summary>
        [Units("[us]")]
        [Description("Image timestamp (since UNIX epoch), as passed in by CAMERA_STATUS message (or autopilot if no CCB).")]
        public  ulong time_usec;
            /// <summary>Latitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude.")]
        public  int lat;
            /// <summary>Longitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude.")]
        public  int lng;
            /// <summary>Altitude (MSL).  [m] </summary>
        [Units("[m]")]
        [Description("Altitude (MSL).")]
        public  float alt_msl;
            /// <summary>Altitude (Relative to HOME location).  [m] </summary>
        [Units("[m]")]
        [Description("Altitude (Relative to HOME location).")]
        public  float alt_rel;
            /// <summary>Camera Roll angle (earth frame, +-180).  [deg] </summary>
        [Units("[deg]")]
        [Description("Camera Roll angle (earth frame, +-180).")]
        public  float roll;
            /// <summary>Camera Pitch angle (earth frame, +-180).  [deg] </summary>
        [Units("[deg]")]
        [Description("Camera Pitch angle (earth frame, +-180).")]
        public  float pitch;
            /// <summary>Camera Yaw (earth frame, 0-360, true).  [deg] </summary>
        [Units("[deg]")]
        [Description("Camera Yaw (earth frame, 0-360, true).")]
        public  float yaw;
            /// <summary>Focal Length.  [mm] </summary>
        [Units("[mm]")]
        [Description("Focal Length.")]
        public  float foc_len;
            /// <summary>Image index.   </summary>
        [Units("")]
        [Description("Image index.")]
        public  ushort img_idx;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Camera ID.   </summary>
        [Units("")]
        [Description("Camera ID.")]
        public  byte cam_idx;
            /// <summary>Feedback flags. CAMERA_FEEDBACK_FLAGS  </summary>
        [Units("")]
        [Description("Feedback flags.")]
        public  /*CAMERA_FEEDBACK_FLAGS*/byte flags;
            /// <summary>Completed image captures.   </summary>
        [Units("")]
        [Description("Completed image captures.")]
        public  ushort completed_captures;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> 2nd Battery status </summary>
    public struct mavlink_battery2_t
    {
        /// <summary>Voltage.  [mV] </summary>
        [Units("[mV]")]
        [Description("Voltage.")]
        public  ushort voltage;
            /// <summary>Battery current, -1: autopilot does not measure the current.  [cA] </summary>
        [Units("[cA]")]
        [Description("Battery current, -1: autopilot does not measure the current.")]
        public  short current_battery;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=40)]
    ///<summary> Status of third AHRS filter if available. This is for ANU research group (Ali and Sean). </summary>
    public struct mavlink_ahrs3_t
    {
        /// <summary>Roll angle.  [rad] </summary>
        [Units("[rad]")]
        [Description("Roll angle.")]
        public  float roll;
            /// <summary>Pitch angle.  [rad] </summary>
        [Units("[rad]")]
        [Description("Pitch angle.")]
        public  float pitch;
            /// <summary>Yaw angle.  [rad] </summary>
        [Units("[rad]")]
        [Description("Yaw angle.")]
        public  float yaw;
            /// <summary>Altitude (MSL).  [m] </summary>
        [Units("[m]")]
        [Description("Altitude (MSL).")]
        public  float altitude;
            /// <summary>Latitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude.")]
        public  int lat;
            /// <summary>Longitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude.")]
        public  int lng;
            /// <summary>Test variable1.   </summary>
        [Units("")]
        [Description("Test variable1.")]
        public  float v1;
            /// <summary>Test variable2.   </summary>
        [Units("")]
        [Description("Test variable2.")]
        public  float v2;
            /// <summary>Test variable3.   </summary>
        [Units("")]
        [Description("Test variable3.")]
        public  float v3;
            /// <summary>Test variable4.   </summary>
        [Units("")]
        [Description("Test variable4.")]
        public  float v4;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Request the autopilot version from the system/component. </summary>
    public struct mavlink_autopilot_version_request_t
    {
        /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=206)]
    ///<summary> Send a block of log data to remote location. </summary>
    public struct mavlink_remote_log_data_block_t
    {
        /// <summary>Log data block sequence number. MAV_REMOTE_LOG_DATA_BLOCK_COMMANDS  </summary>
        [Units("")]
        [Description("Log data block sequence number.")]
        public  /*MAV_REMOTE_LOG_DATA_BLOCK_COMMANDS*/uint seqno;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Log data block.   </summary>
        [Units("")]
        [Description("Log data block.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=200)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=7)]
    ///<summary> Send Status of each log block that autopilot board might have sent. </summary>
    public struct mavlink_remote_log_block_status_t
    {
        /// <summary>Log data block sequence number.   </summary>
        [Units("")]
        [Description("Log data block sequence number.")]
        public  uint seqno;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Log data block status. MAV_REMOTE_LOG_DATA_BLOCK_STATUSES  </summary>
        [Units("")]
        [Description("Log data block status.")]
        public  /*MAV_REMOTE_LOG_DATA_BLOCK_STATUSES*/byte status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=29)]
    ///<summary> Control vehicle LEDs. </summary>
    public struct mavlink_led_control_t
    {
        /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Instance (LED instance to control or 255 for all LEDs).   </summary>
        [Units("")]
        [Description("Instance (LED instance to control or 255 for all LEDs).")]
        public  byte instance;
            /// <summary>Pattern (see LED_PATTERN_ENUM).   </summary>
        [Units("")]
        [Description("Pattern (see LED_PATTERN_ENUM).")]
        public  byte pattern;
            /// <summary>Custom Byte Length.   </summary>
        [Units("")]
        [Description("Custom Byte Length.")]
        public  byte custom_len;
            /// <summary>Custom Bytes.   </summary>
        [Units("")]
        [Description("Custom Bytes.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=24)]
		public byte[] custom_bytes;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=27)]
    ///<summary> Reports progress of compass calibration. </summary>
    public struct mavlink_mag_cal_progress_t
    {
        /// <summary>Body frame direction vector for display.   </summary>
        [Units("")]
        [Description("Body frame direction vector for display.")]
        public  float direction_x;
            /// <summary>Body frame direction vector for display.   </summary>
        [Units("")]
        [Description("Body frame direction vector for display.")]
        public  float direction_y;
            /// <summary>Body frame direction vector for display.   </summary>
        [Units("")]
        [Description("Body frame direction vector for display.")]
        public  float direction_z;
            /// <summary>Compass being calibrated.   </summary>
        [Units("")]
        [Description("Compass being calibrated.")]
        public  byte compass_id;
            /// <summary>Bitmask of compasses being calibrated.   bitmask</summary>
        [Units("")]
        [Description("Bitmask of compasses being calibrated.")]
        public  byte cal_mask;
            /// <summary>Calibration Status. MAG_CAL_STATUS  </summary>
        [Units("")]
        [Description("Calibration Status.")]
        public  /*MAG_CAL_STATUS*/byte cal_status;
            /// <summary>Attempt number.   </summary>
        [Units("")]
        [Description("Attempt number.")]
        public  byte attempt;
            /// <summary>Completion percentage.  [%] </summary>
        [Units("[%]")]
        [Description("Completion percentage.")]
        public  byte completion_pct;
            /// <summary>Bitmask of sphere sections (see http://en.wikipedia.org/wiki/Geodesic_grid).   </summary>
        [Units("")]
        [Description("Bitmask of sphere sections (see http://en.wikipedia.org/wiki/Geodesic_grid).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] completion_mask;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=50)]
    ///<summary> Reports results of completed compass calibration. Sent until MAG_CAL_ACK received. </summary>
    public struct mavlink_mag_cal_report_t
    {
        /// <summary>RMS milligauss residuals.  [mgauss] </summary>
        [Units("[mgauss]")]
        [Description("RMS milligauss residuals.")]
        public  float fitness;
            /// <summary>X offset.   </summary>
        [Units("")]
        [Description("X offset.")]
        public  float ofs_x;
            /// <summary>Y offset.   </summary>
        [Units("")]
        [Description("Y offset.")]
        public  float ofs_y;
            /// <summary>Z offset.   </summary>
        [Units("")]
        [Description("Z offset.")]
        public  float ofs_z;
            /// <summary>X diagonal (matrix 11).   </summary>
        [Units("")]
        [Description("X diagonal (matrix 11).")]
        public  float diag_x;
            /// <summary>Y diagonal (matrix 22).   </summary>
        [Units("")]
        [Description("Y diagonal (matrix 22).")]
        public  float diag_y;
            /// <summary>Z diagonal (matrix 33).   </summary>
        [Units("")]
        [Description("Z diagonal (matrix 33).")]
        public  float diag_z;
            /// <summary>X off-diagonal (matrix 12 and 21).   </summary>
        [Units("")]
        [Description("X off-diagonal (matrix 12 and 21).")]
        public  float offdiag_x;
            /// <summary>Y off-diagonal (matrix 13 and 31).   </summary>
        [Units("")]
        [Description("Y off-diagonal (matrix 13 and 31).")]
        public  float offdiag_y;
            /// <summary>Z off-diagonal (matrix 32 and 23).   </summary>
        [Units("")]
        [Description("Z off-diagonal (matrix 32 and 23).")]
        public  float offdiag_z;
            /// <summary>Compass being calibrated.   </summary>
        [Units("")]
        [Description("Compass being calibrated.")]
        public  byte compass_id;
            /// <summary>Bitmask of compasses being calibrated.   bitmask</summary>
        [Units("")]
        [Description("Bitmask of compasses being calibrated.")]
        public  byte cal_mask;
            /// <summary>Calibration Status. MAG_CAL_STATUS  </summary>
        [Units("")]
        [Description("Calibration Status.")]
        public  /*MAG_CAL_STATUS*/byte cal_status;
            /// <summary>0=requires a MAV_CMD_DO_ACCEPT_MAG_CAL, 1=saved to parameters.   </summary>
        [Units("")]
        [Description("0=requires a MAV_CMD_DO_ACCEPT_MAG_CAL, 1=saved to parameters.")]
        public  byte autosaved;
            /// <summary>Confidence in orientation (higher is better).   </summary>
        [Units("")]
        [Description("Confidence in orientation (higher is better).")]
        public  float orientation_confidence;
            /// <summary>orientation before calibration. MAV_SENSOR_ORIENTATION  </summary>
        [Units("")]
        [Description("orientation before calibration.")]
        public  /*MAV_SENSOR_ORIENTATION*/byte old_orientation;
            /// <summary>orientation after calibration. MAV_SENSOR_ORIENTATION  </summary>
        [Units("")]
        [Description("orientation after calibration.")]
        public  /*MAV_SENSOR_ORIENTATION*/byte new_orientation;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    ///<summary> EKF Status message including flags and variances. </summary>
    public struct mavlink_ekf_status_report_t
    {
        /// <summary>Velocity variance.   </summary>
        [Units("")]
        [Description("Velocity variance.")]
        public  float velocity_variance;
            /// <summary>Horizontal Position variance.   </summary>
        [Units("")]
        [Description("Horizontal Position variance.")]
        public  float pos_horiz_variance;
            /// <summary>Vertical Position variance.   </summary>
        [Units("")]
        [Description("Vertical Position variance.")]
        public  float pos_vert_variance;
            /// <summary>Compass variance.   </summary>
        [Units("")]
        [Description("Compass variance.")]
        public  float compass_variance;
            /// <summary>Terrain Altitude variance.   </summary>
        [Units("")]
        [Description("Terrain Altitude variance.")]
        public  float terrain_alt_variance;
            /// <summary>Flags. EKF_STATUS_FLAGS  </summary>
        [Units("")]
        [Description("Flags.")]
        public  /*EKF_STATUS_FLAGS*/ushort flags;
            /// <summary>Airspeed variance.   </summary>
        [Units("")]
        [Description("Airspeed variance.")]
        public  float airspeed_variance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=25)]
    ///<summary> PID tuning information. </summary>
    public struct mavlink_pid_tuning_t
    {
        /// <summary>Desired rate.  [deg/s] </summary>
        [Units("[deg/s]")]
        [Description("Desired rate.")]
        public  float desired;
            /// <summary>Achieved rate.  [deg/s] </summary>
        [Units("[deg/s]")]
        [Description("Achieved rate.")]
        public  float achieved;
            /// <summary>FF component.   </summary>
        [Units("")]
        [Description("FF component.")]
        public  float FF;
            /// <summary>P component.   </summary>
        [Units("")]
        [Description("P component.")]
        public  float P;
            /// <summary>I component.   </summary>
        [Units("")]
        [Description("I component.")]
        public  float I;
            /// <summary>D component.   </summary>
        [Units("")]
        [Description("D component.")]
        public  float D;
            /// <summary>Axis. PID_TUNING_AXIS  </summary>
        [Units("")]
        [Description("Axis.")]
        public  /*PID_TUNING_AXIS*/byte axis;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    ///<summary> Deepstall path planning. </summary>
    public struct mavlink_deepstall_t
    {
        /// <summary>Landing latitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Landing latitude.")]
        public  int landing_lat;
            /// <summary>Landing longitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Landing longitude.")]
        public  int landing_lon;
            /// <summary>Final heading start point, latitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Final heading start point, latitude.")]
        public  int path_lat;
            /// <summary>Final heading start point, longitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Final heading start point, longitude.")]
        public  int path_lon;
            /// <summary>Arc entry point, latitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Arc entry point, latitude.")]
        public  int arc_entry_lat;
            /// <summary>Arc entry point, longitude.  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Arc entry point, longitude.")]
        public  int arc_entry_lon;
            /// <summary>Altitude.  [m] </summary>
        [Units("[m]")]
        [Description("Altitude.")]
        public  float altitude;
            /// <summary>Distance the aircraft expects to travel during the deepstall.  [m] </summary>
        [Units("[m]")]
        [Description("Distance the aircraft expects to travel during the deepstall.")]
        public  float expected_travel_distance;
            /// <summary>Deepstall cross track error (only valid when in DEEPSTALL_STAGE_LAND).  [m] </summary>
        [Units("[m]")]
        [Description("Deepstall cross track error (only valid when in DEEPSTALL_STAGE_LAND).")]
        public  float cross_track_error;
            /// <summary>Deepstall stage. DEEPSTALL_STAGE  </summary>
        [Units("")]
        [Description("Deepstall stage.")]
        public  /*DEEPSTALL_STAGE*/byte stage;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> 3 axis gimbal measurements. </summary>
    public struct mavlink_gimbal_report_t
    {
        /// <summary>Time since last update.  [s] </summary>
        [Units("[s]")]
        [Description("Time since last update.")]
        public  float delta_time;
            /// <summary>Delta angle X.  [rad] </summary>
        [Units("[rad]")]
        [Description("Delta angle X.")]
        public  float delta_angle_x;
            /// <summary>Delta angle Y.  [rad] </summary>
        [Units("[rad]")]
        [Description("Delta angle Y.")]
        public  float delta_angle_y;
            /// <summary>Delta angle X.  [rad] </summary>
        [Units("[rad]")]
        [Description("Delta angle X.")]
        public  float delta_angle_z;
            /// <summary>Delta velocity X.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Delta velocity X.")]
        public  float delta_velocity_x;
            /// <summary>Delta velocity Y.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Delta velocity Y.")]
        public  float delta_velocity_y;
            /// <summary>Delta velocity Z.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Delta velocity Z.")]
        public  float delta_velocity_z;
            /// <summary>Joint ROLL.  [rad] </summary>
        [Units("[rad]")]
        [Description("Joint ROLL.")]
        public  float joint_roll;
            /// <summary>Joint EL.  [rad] </summary>
        [Units("[rad]")]
        [Description("Joint EL.")]
        public  float joint_el;
            /// <summary>Joint AZ.  [rad] </summary>
        [Units("[rad]")]
        [Description("Joint AZ.")]
        public  float joint_az;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Control message for rate gimbal. </summary>
    public struct mavlink_gimbal_control_t
    {
        /// <summary>Demanded angular rate X.  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Demanded angular rate X.")]
        public  float demanded_rate_x;
            /// <summary>Demanded angular rate Y.  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Demanded angular rate Y.")]
        public  float demanded_rate_y;
            /// <summary>Demanded angular rate Z.  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Demanded angular rate Z.")]
        public  float demanded_rate_z;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> 100 Hz gimbal torque command telemetry. </summary>
    public struct mavlink_gimbal_torque_cmd_report_t
    {
        /// <summary>Roll Torque Command.   </summary>
        [Units("")]
        [Description("Roll Torque Command.")]
        public  short rl_torque_cmd;
            /// <summary>Elevation Torque Command.   </summary>
        [Units("")]
        [Description("Elevation Torque Command.")]
        public  short el_torque_cmd;
            /// <summary>Azimuth Torque Command.   </summary>
        [Units("")]
        [Description("Azimuth Torque Command.")]
        public  short az_torque_cmd;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Heartbeat from a HeroBus attached GoPro. </summary>
    public struct mavlink_gopro_heartbeat_t
    {
        /// <summary>Status. GOPRO_HEARTBEAT_STATUS  </summary>
        [Units("")]
        [Description("Status.")]
        public  /*GOPRO_HEARTBEAT_STATUS*/byte status;
            /// <summary>Current capture mode. GOPRO_CAPTURE_MODE  </summary>
        [Units("")]
        [Description("Current capture mode.")]
        public  /*GOPRO_CAPTURE_MODE*/byte capture_mode;
            /// <summary>Additional status bits. GOPRO_HEARTBEAT_FLAGS  bitmask</summary>
        [Units("")]
        [Description("Additional status bits.")]
        public  /*GOPRO_HEARTBEAT_FLAGS*/byte flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Request a GOPRO_COMMAND response from the GoPro. </summary>
    public struct mavlink_gopro_get_request_t
    {
        /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Command ID. GOPRO_COMMAND  </summary>
        [Units("")]
        [Description("Command ID.")]
        public  /*GOPRO_COMMAND*/byte cmd_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Response from a GOPRO_COMMAND get request. </summary>
    public struct mavlink_gopro_get_response_t
    {
        /// <summary>Command ID. GOPRO_COMMAND  </summary>
        [Units("")]
        [Description("Command ID.")]
        public  /*GOPRO_COMMAND*/byte cmd_id;
            /// <summary>Status. GOPRO_REQUEST_STATUS  </summary>
        [Units("")]
        [Description("Status.")]
        public  /*GOPRO_REQUEST_STATUS*/byte status;
            /// <summary>Value.   </summary>
        [Units("")]
        [Description("Value.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] value;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=7)]
    ///<summary> Request to set a GOPRO_COMMAND with a desired. </summary>
    public struct mavlink_gopro_set_request_t
    {
        /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>Command ID. GOPRO_COMMAND  </summary>
        [Units("")]
        [Description("Command ID.")]
        public  /*GOPRO_COMMAND*/byte cmd_id;
            /// <summary>Value.   </summary>
        [Units("")]
        [Description("Value.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] value;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Response from a GOPRO_COMMAND set request. </summary>
    public struct mavlink_gopro_set_response_t
    {
        /// <summary>Command ID. GOPRO_COMMAND  </summary>
        [Units("")]
        [Description("Command ID.")]
        public  /*GOPRO_COMMAND*/byte cmd_id;
            /// <summary>Status. GOPRO_REQUEST_STATUS  </summary>
        [Units("")]
        [Description("Status.")]
        public  /*GOPRO_REQUEST_STATUS*/byte status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> RPM sensor output. </summary>
    public struct mavlink_rpm_t
    {
        /// <summary>RPM Sensor1.   </summary>
        [Units("")]
        [Description("RPM Sensor1.")]
        public  float rpm1;
            /// <summary>RPM Sensor2.   </summary>
        [Units("")]
        [Description("RPM Sensor2.")]
        public  float rpm2;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    ///<summary> Read registers for a device. </summary>
    public struct mavlink_device_op_read_t
    {
        /// <summary>Request ID - copied to reply.   </summary>
        [Units("")]
        [Description("Request ID - copied to reply.")]
        public  uint request_id;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>The bus type. DEVICE_OP_BUSTYPE  </summary>
        [Units("")]
        [Description("The bus type.")]
        public  /*DEVICE_OP_BUSTYPE*/byte bustype;
            /// <summary>Bus number.   </summary>
        [Units("")]
        [Description("Bus number.")]
        public  byte bus;
            /// <summary>Bus address.   </summary>
        [Units("")]
        [Description("Bus address.")]
        public  byte address;
            /// <summary>Name of device on bus (for SPI).   </summary>
        [Units("")]
        [Description("Name of device on bus (for SPI).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=40)]
		public byte[] busname;
            /// <summary>First register to read.   </summary>
        [Units("")]
        [Description("First register to read.")]
        public  byte regstart;
            /// <summary>Count of registers to read.   </summary>
        [Units("")]
        [Description("Count of registers to read.")]
        public  byte count;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=135)]
    ///<summary> Read registers reply. </summary>
    public struct mavlink_device_op_read_reply_t
    {
        /// <summary>Request ID - copied from request.   </summary>
        [Units("")]
        [Description("Request ID - copied from request.")]
        public  uint request_id;
            /// <summary>0 for success, anything else is failure code.   </summary>
        [Units("")]
        [Description("0 for success, anything else is failure code.")]
        public  byte result;
            /// <summary>Starting register.   </summary>
        [Units("")]
        [Description("Starting register.")]
        public  byte regstart;
            /// <summary>Count of bytes read.   </summary>
        [Units("")]
        [Description("Count of bytes read.")]
        public  byte count;
            /// <summary>Reply data.   </summary>
        [Units("")]
        [Description("Reply data.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=128)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=179)]
    ///<summary> Write registers for a device. </summary>
    public struct mavlink_device_op_write_t
    {
        /// <summary>Request ID - copied to reply.   </summary>
        [Units("")]
        [Description("Request ID - copied to reply.")]
        public  uint request_id;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Component ID.   </summary>
        [Units("")]
        [Description("Component ID.")]
        public  byte target_component;
            /// <summary>The bus type. DEVICE_OP_BUSTYPE  </summary>
        [Units("")]
        [Description("The bus type.")]
        public  /*DEVICE_OP_BUSTYPE*/byte bustype;
            /// <summary>Bus number.   </summary>
        [Units("")]
        [Description("Bus number.")]
        public  byte bus;
            /// <summary>Bus address.   </summary>
        [Units("")]
        [Description("Bus address.")]
        public  byte address;
            /// <summary>Name of device on bus (for SPI).   </summary>
        [Units("")]
        [Description("Name of device on bus (for SPI).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=40)]
		public byte[] busname;
            /// <summary>First register to write.   </summary>
        [Units("")]
        [Description("First register to write.")]
        public  byte regstart;
            /// <summary>Count of registers to write.   </summary>
        [Units("")]
        [Description("Count of registers to write.")]
        public  byte count;
            /// <summary>Write data.   </summary>
        [Units("")]
        [Description("Write data.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=128)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=5)]
    ///<summary> Write registers reply. </summary>
    public struct mavlink_device_op_write_reply_t
    {
        /// <summary>Request ID - copied from request.   </summary>
        [Units("")]
        [Description("Request ID - copied from request.")]
        public  uint request_id;
            /// <summary>0 for success, anything else is failure code.   </summary>
        [Units("")]
        [Description("0 for success, anything else is failure code.")]
        public  byte result;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=49)]
    ///<summary> Adaptive Controller tuning information. </summary>
    public struct mavlink_adap_tuning_t
    {
        /// <summary>Desired rate.  [deg/s] </summary>
        [Units("[deg/s]")]
        [Description("Desired rate.")]
        public  float desired;
            /// <summary>Achieved rate.  [deg/s] </summary>
        [Units("[deg/s]")]
        [Description("Achieved rate.")]
        public  float achieved;
            /// <summary>Error between model and vehicle.   </summary>
        [Units("")]
        [Description("Error between model and vehicle.")]
        public  float error;
            /// <summary>Theta estimated state predictor.   </summary>
        [Units("")]
        [Description("Theta estimated state predictor.")]
        public  float theta;
            /// <summary>Omega estimated state predictor.   </summary>
        [Units("")]
        [Description("Omega estimated state predictor.")]
        public  float omega;
            /// <summary>Sigma estimated state predictor.   </summary>
        [Units("")]
        [Description("Sigma estimated state predictor.")]
        public  float sigma;
            /// <summary>Theta derivative.   </summary>
        [Units("")]
        [Description("Theta derivative.")]
        public  float theta_dot;
            /// <summary>Omega derivative.   </summary>
        [Units("")]
        [Description("Omega derivative.")]
        public  float omega_dot;
            /// <summary>Sigma derivative.   </summary>
        [Units("")]
        [Description("Sigma derivative.")]
        public  float sigma_dot;
            /// <summary>Projection operator value.   </summary>
        [Units("")]
        [Description("Projection operator value.")]
        public  float f;
            /// <summary>Projection operator derivative.   </summary>
        [Units("")]
        [Description("Projection operator derivative.")]
        public  float f_dot;
            /// <summary>u adaptive controlled output command.   </summary>
        [Units("")]
        [Description("u adaptive controlled output command.")]
        public  float u;
            /// <summary>Axis. PID_TUNING_AXIS  </summary>
        [Units("")]
        [Description("Axis.")]
        public  /*PID_TUNING_AXIS*/byte axis;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> Camera vision based attitude and position deltas. </summary>
    public struct mavlink_vision_position_delta_t
    {
        /// <summary>Timestamp (synced to UNIX time or since system boot).  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (synced to UNIX time or since system boot).")]
        public  ulong time_usec;
            /// <summary>Time since the last reported camera frame.  [us] </summary>
        [Units("[us]")]
        [Description("Time since the last reported camera frame.")]
        public  ulong time_delta_usec;
            /// <summary>Defines a rotation vector in body frame that rotates the vehicle from the previous to the current orientation.   </summary>
        [Units("")]
        [Description("Defines a rotation vector in body frame that rotates the vehicle from the previous to the current orientation.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] angle_delta;
            /// <summary>Change in position from previous to current frame rotated into body frame (0=forward, 1=right, 2=down).  [m] </summary>
        [Units("[m]")]
        [Description("Change in position from previous to current frame rotated into body frame (0=forward, 1=right, 2=down).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] position_delta;
            /// <summary>Normalised confidence value from 0 to 100.  [%] </summary>
        [Units("[%]")]
        [Description("Normalised confidence value from 0 to 100.")]
        public  float confidence;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=16)]
    ///<summary> Angle of Attack and Side Slip Angle. </summary>
    public struct mavlink_aoa_ssa_t
    {
        /// <summary>Timestamp (since boot or Unix epoch).  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (since boot or Unix epoch).")]
        public  ulong time_usec;
            /// <summary>Angle of Attack.  [deg] </summary>
        [Units("[deg]")]
        [Description("Angle of Attack.")]
        public  float AOA;
            /// <summary>Side Slip Angle.  [deg] </summary>
        [Units("[deg]")]
        [Description("Side Slip Angle.")]
        public  float SSA;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> ESC Telemetry Data for ESCs 1 to 4, matching data sent by BLHeli ESCs. </summary>
    public struct mavlink_esc_telemetry_1_to_4_t
    {
        /// <summary>Voltage.  [cV] </summary>
        [Units("[cV]")]
        [Description("Voltage.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] voltage;
            /// <summary>Current.  [cA] </summary>
        [Units("[cA]")]
        [Description("Current.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] current;
            /// <summary>Total current.  [mAh] </summary>
        [Units("[mAh]")]
        [Description("Total current.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] totalcurrent;
            /// <summary>RPM (eRPM).  [rpm] </summary>
        [Units("[rpm]")]
        [Description("RPM (eRPM).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] rpm;
            /// <summary>count of telemetry packets received (wraps at 65535).   </summary>
        [Units("")]
        [Description("count of telemetry packets received (wraps at 65535).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] count;
            /// <summary>Temperature.  [degC] </summary>
        [Units("[degC]")]
        [Description("Temperature.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> ESC Telemetry Data for ESCs 5 to 8, matching data sent by BLHeli ESCs. </summary>
    public struct mavlink_esc_telemetry_5_to_8_t
    {
        /// <summary>Voltage.  [cV] </summary>
        [Units("[cV]")]
        [Description("Voltage.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] voltage;
            /// <summary>Current.  [cA] </summary>
        [Units("[cA]")]
        [Description("Current.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] current;
            /// <summary>Total current.  [mAh] </summary>
        [Units("[mAh]")]
        [Description("Total current.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] totalcurrent;
            /// <summary>RPM (eRPM).  [rpm] </summary>
        [Units("[rpm]")]
        [Description("RPM (eRPM).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] rpm;
            /// <summary>count of telemetry packets received (wraps at 65535).   </summary>
        [Units("")]
        [Description("count of telemetry packets received (wraps at 65535).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] count;
            /// <summary>Temperature.  [degC] </summary>
        [Units("[degC]")]
        [Description("Temperature.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> ESC Telemetry Data for ESCs 9 to 12, matching data sent by BLHeli ESCs. </summary>
    public struct mavlink_esc_telemetry_9_to_12_t
    {
        /// <summary>Voltage.  [cV] </summary>
        [Units("[cV]")]
        [Description("Voltage.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] voltage;
            /// <summary>Current.  [cA] </summary>
        [Units("[cA]")]
        [Description("Current.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] current;
            /// <summary>Total current.  [mAh] </summary>
        [Units("[mAh]")]
        [Description("Total current.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] totalcurrent;
            /// <summary>RPM (eRPM).  [rpm] </summary>
        [Units("[rpm]")]
        [Description("RPM (eRPM).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] rpm;
            /// <summary>count of telemetry packets received (wraps at 65535).   </summary>
        [Units("")]
        [Description("count of telemetry packets received (wraps at 65535).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public UInt16[] count;
            /// <summary>Temperature.  [degC] </summary>
        [Units("[degC]")]
        [Description("Temperature.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public byte[] temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> The heartbeat message shows that a system or component is present and responding. The type and autopilot fields (along with the message component id), allow the receiving system to treat further messages from this system appropriately (e.g. by laying out the user interface based on the autopilot). </summary>
    public struct mavlink_heartbeat_t
    {
        /// <summary>A bitfield for use for autopilot-specific flags   </summary>
        [Units("")]
        [Description("A bitfield for use for autopilot-specific flags")]
        public  uint custom_mode;
            /// <summary>Type of the system (quadrotor, helicopter, etc.). Components use the same type as their associated system. MAV_TYPE  </summary>
        [Units("")]
        [Description("Type of the system (quadrotor, helicopter, etc.). Components use the same type as their associated system.")]
        public  /*MAV_TYPE*/byte type;
            /// <summary>Autopilot type / class. MAV_AUTOPILOT  </summary>
        [Units("")]
        [Description("Autopilot type / class.")]
        public  /*MAV_AUTOPILOT*/byte autopilot;
            /// <summary>System mode bitmap. MAV_MODE_FLAG  bitmask</summary>
        [Units("")]
        [Description("System mode bitmap.")]
        public  /*MAV_MODE_FLAG*/byte base_mode;
            /// <summary>System status flag. MAV_STATE  </summary>
        [Units("")]
        [Description("System status flag.")]
        public  /*MAV_STATE*/byte system_status;
            /// <summary>MAVLink version, not writable by user, gets added by protocol because of magic data type: uint8_t_mavlink_version   </summary>
        [Units("")]
        [Description("MAVLink version, not writable by user, gets added by protocol because of magic data type: uint8_t_mavlink_version")]
        public  byte mavlink_version;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=31)]
    ///<summary> The general system state. If the system is following the MAVLink standard, the system state is mainly defined by three orthogonal states/modes: The system mode, which is either LOCKED (motors shut down and locked), MANUAL (system under RC control), GUIDED (system with autonomous position control, position setpoint controlled manually) or AUTO (system guided by path/waypoint planner). The NAV_MODE defined the current flight state: LIFTOFF (often an open-loop maneuver), LANDING, WAYPOINTS or VECTOR. This represents the internal navigation state machine. The system status shows whether the system is currently active or not and if an emergency occurred. During the CRITICAL and EMERGENCY states the MAV is still considered to be active, but should start emergency procedures autonomously. After a failure occurred it should first move from active to critical to allow manual intervention and then move to emergency after a certain timeout. </summary>
    public struct mavlink_sys_status_t
    {
        /// <summary>Bitmap showing which onboard controllers and sensors are present. Value of 0: not present. Value of 1: present. MAV_SYS_STATUS_SENSOR  bitmask</summary>
        [Units("")]
        [Description("Bitmap showing which onboard controllers and sensors are present. Value of 0: not present. Value of 1: present.")]
        public  /*MAV_SYS_STATUS_SENSOR*/uint onboard_control_sensors_present;
            /// <summary>Bitmap showing which onboard controllers and sensors are enabled:  Value of 0: not enabled. Value of 1: enabled. MAV_SYS_STATUS_SENSOR  bitmask</summary>
        [Units("")]
        [Description("Bitmap showing which onboard controllers and sensors are enabled:  Value of 0: not enabled. Value of 1: enabled.")]
        public  /*MAV_SYS_STATUS_SENSOR*/uint onboard_control_sensors_enabled;
            /// <summary>Bitmap showing which onboard controllers and sensors are operational or have an error:  Value of 0: not enabled. Value of 1: enabled. MAV_SYS_STATUS_SENSOR  bitmask</summary>
        [Units("")]
        [Description("Bitmap showing which onboard controllers and sensors are operational or have an error:  Value of 0: not enabled. Value of 1: enabled.")]
        public  /*MAV_SYS_STATUS_SENSOR*/uint onboard_control_sensors_health;
            /// <summary>Maximum usage in percent of the mainloop time. Values: [0-1000] - should always be below 1000  [d%] </summary>
        [Units("[d%]")]
        [Description("Maximum usage in percent of the mainloop time. Values: [0-1000] - should always be below 1000")]
        public  ushort load;
            /// <summary>Battery voltage  [mV] </summary>
        [Units("[mV]")]
        [Description("Battery voltage")]
        public  ushort voltage_battery;
            /// <summary>Battery current, -1: autopilot does not measure the current  [cA] </summary>
        [Units("[cA]")]
        [Description("Battery current, -1: autopilot does not measure the current")]
        public  short current_battery;
            /// <summary>Communication drop rate, (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV)  [c%] </summary>
        [Units("[c%]")]
        [Description("Communication drop rate, (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV)")]
        public  ushort drop_rate_comm;
            /// <summary>Communication errors (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV)   </summary>
        [Units("")]
        [Description("Communication errors (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV)")]
        public  ushort errors_comm;
            /// <summary>Autopilot-specific errors   </summary>
        [Units("")]
        [Description("Autopilot-specific errors")]
        public  ushort errors_count1;
            /// <summary>Autopilot-specific errors   </summary>
        [Units("")]
        [Description("Autopilot-specific errors")]
        public  ushort errors_count2;
            /// <summary>Autopilot-specific errors   </summary>
        [Units("")]
        [Description("Autopilot-specific errors")]
        public  ushort errors_count3;
            /// <summary>Autopilot-specific errors   </summary>
        [Units("")]
        [Description("Autopilot-specific errors")]
        public  ushort errors_count4;
            /// <summary>Remaining battery energy, -1: autopilot estimate the remaining battery  [%] </summary>
        [Units("[%]")]
        [Description("Remaining battery energy, -1: autopilot estimate the remaining battery")]
        public  byte battery_remaining;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> The system time is the time of the master clock, typically the computer clock of the main onboard computer. </summary>
    public struct mavlink_system_time_t
    {
        /// <summary>Timestamp (UNIX epoch time).  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX epoch time).")]
        public  ulong time_unix_usec;
            /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> A ping message either requesting or responding to a ping. This allows to measure the system latencies, including serial port, radio modem and UDP connections. </summary>
    public struct mavlink_ping_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>PING sequence   </summary>
        [Units("")]
        [Description("PING sequence")]
        public  uint seq;
            /// <summary>0: request ping from all receiving systems. If greater than 0: message is a ping response and number is the system id of the requesting system   </summary>
        [Units("")]
        [Description("0: request ping from all receiving systems. If greater than 0: message is a ping response and number is the system id of the requesting system")]
        public  byte target_system;
            /// <summary>0: request ping from all receiving components. If greater than 0: message is a ping response and number is the component id of the requesting component.   </summary>
        [Units("")]
        [Description("0: request ping from all receiving components. If greater than 0: message is a ping response and number is the component id of the requesting component.")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> Request to control this MAV </summary>
    public struct mavlink_change_operator_control_t
    {
        /// <summary>System the GCS requests control for   </summary>
        [Units("")]
        [Description("System the GCS requests control for")]
        public  byte target_system;
            /// <summary>0: request control of this MAV, 1: Release control of this MAV   </summary>
        [Units("")]
        [Description("0: request control of this MAV, 1: Release control of this MAV")]
        public  byte control_request;
            /// <summary>0: key as plaintext, 1-255: future, different hashing/encryption variants. The GCS should in general use the safest mode possible initially and then gradually move down the encryption level if it gets a NACK message indicating an encryption mismatch.  [rad] </summary>
        [Units("[rad]")]
        [Description("0: key as plaintext, 1-255: future, different hashing/encryption variants. The GCS should in general use the safest mode possible initially and then gradually move down the encryption level if it gets a NACK message indicating an encryption mismatch.")]
        public  byte version;
            /// <summary>Password / Key, depending on version plaintext or encrypted. 25 or less characters, NULL terminated. The characters may involve A-Z, a-z, 0-9, and '!?,.-'   </summary>
        [Units("")]
        [Description("Password / Key, depending on version plaintext or encrypted. 25 or less characters, NULL terminated. The characters may involve A-Z, a-z, 0-9, and '!?,.-'")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=25)]
		public byte[] passkey;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Accept / deny control of this MAV </summary>
    public struct mavlink_change_operator_control_ack_t
    {
        /// <summary>ID of the GCS this message    </summary>
        [Units("")]
        [Description("ID of the GCS this message ")]
        public  byte gcs_system_id;
            /// <summary>0: request control of this MAV, 1: Release control of this MAV   </summary>
        [Units("")]
        [Description("0: request control of this MAV, 1: Release control of this MAV")]
        public  byte control_request;
            /// <summary>0: ACK, 1: NACK: Wrong passkey, 2: NACK: Unsupported passkey encryption method, 3: NACK: Already under control   </summary>
        [Units("")]
        [Description("0: ACK, 1: NACK: Wrong passkey, 2: NACK: Unsupported passkey encryption method, 3: NACK: Already under control")]
        public  byte ack;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    ///<summary> Emit an encrypted signature / key identifying this system. PLEASE NOTE: This protocol has been kept simple, so transmitting the key requires an encrypted channel for true safety. </summary>
    public struct mavlink_auth_key_t
    {
        /// <summary>key   </summary>
        [Units("")]
        [Description("key")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] key;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Set the system mode, as defined by enum MAV_MODE. There is no target component id as the mode is by definition for the overall aircraft, not only for one component. </summary>
    public struct mavlink_set_mode_t
    {
        /// <summary>The new autopilot-specific mode. This field can be ignored by an autopilot.   </summary>
        [Units("")]
        [Description("The new autopilot-specific mode. This field can be ignored by an autopilot.")]
        public  uint custom_mode;
            /// <summary>The system setting the mode   </summary>
        [Units("")]
        [Description("The system setting the mode")]
        public  byte target_system;
            /// <summary>The new base mode. MAV_MODE  </summary>
        [Units("")]
        [Description("The new base mode.")]
        public  /*MAV_MODE*/byte base_mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Request to read the onboard parameter with the param_id string id. Onboard parameters are stored as key[const char*] -> value[float]. This allows to send a parameter to any other component (such as the GCS) without the need of previous knowledge of possible parameter names. Thus the same GCS can store different parameters for different autopilots. See also https://mavlink.io/en/services/parameter.html for a full documentation of QGroundControl and IMU code. </summary>
    public struct mavlink_param_request_read_t
    {
        /// <summary>Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored)   </summary>
        [Units("")]
        [Description("Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored)")]
        public  short param_index;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string   </summary>
        [Units("")]
        [Description("Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Request all parameters of this component. After this request, all parameters are emitted. </summary>
    public struct mavlink_param_request_list_t
    {
        /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=25)]
    ///<summary> Emit the value of a onboard parameter. The inclusion of param_count and param_index in the message allows the recipient to keep track of received parameters and allows him to re-request missing parameters after a loss or timeout. </summary>
    public struct mavlink_param_value_t
    {
        /// <summary>Onboard parameter value   </summary>
        [Units("")]
        [Description("Onboard parameter value")]
        public  float param_value;
            /// <summary>Total number of onboard parameters   </summary>
        [Units("")]
        [Description("Total number of onboard parameters")]
        public  ushort param_count;
            /// <summary>Index of this onboard parameter   </summary>
        [Units("")]
        [Description("Index of this onboard parameter")]
        public  ushort param_index;
            /// <summary>Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string   </summary>
        [Units("")]
        [Description("Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
            /// <summary>Onboard parameter type. MAV_PARAM_TYPE  </summary>
        [Units("")]
        [Description("Onboard parameter type.")]
        public  /*MAV_PARAM_TYPE*/byte param_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=23)]
    ///<summary> Set a parameter value (write new value to permanent storage). IMPORTANT: The receiving component should acknowledge the new parameter value by sending a PARAM_VALUE message to all communication partners. This will also ensure that multiple GCS all have an up-to-date list of all parameters. If the sending GCS did not receive a PARAM_VALUE message within its timeout time, it should re-send the PARAM_SET message. </summary>
    public struct mavlink_param_set_t
    {
        /// <summary>Onboard parameter value   </summary>
        [Units("")]
        [Description("Onboard parameter value")]
        public  float param_value;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string   </summary>
        [Units("")]
        [Description("Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
            /// <summary>Onboard parameter type. MAV_PARAM_TYPE  </summary>
        [Units("")]
        [Description("Onboard parameter type.")]
        public  /*MAV_PARAM_TYPE*/byte param_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=50)]
    ///<summary> The global position, as returned by the Global Positioning System (GPS). This is    
///                NOT the global position estimate of the system, but rather a RAW sensor value. See message GLOBAL_POSITION for the global position estimate. </summary>
    public struct mavlink_gps_raw_int_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Latitude (WGS84, EGM96 ellipsoid)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude (WGS84, EGM96 ellipsoid)")]
        public  int lat;
            /// <summary>Longitude (WGS84, EGM96 ellipsoid)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude (WGS84, EGM96 ellipsoid)")]
        public  int lon;
            /// <summary>Altitude (MSL). Positive for up. Note that virtually all GPS modules provide the MSL altitude in addition to the WGS84 altitude.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (MSL). Positive for up. Note that virtually all GPS modules provide the MSL altitude in addition to the WGS84 altitude.")]
        public  int alt;
            /// <summary>GPS HDOP horizontal dilution of position (unitless). If unknown, set to: UINT16_MAX   </summary>
        [Units("")]
        [Description("GPS HDOP horizontal dilution of position (unitless). If unknown, set to: UINT16_MAX")]
        public  ushort eph;
            /// <summary>GPS VDOP vertical dilution of position (unitless). If unknown, set to: UINT16_MAX   </summary>
        [Units("")]
        [Description("GPS VDOP vertical dilution of position (unitless). If unknown, set to: UINT16_MAX")]
        public  ushort epv;
            /// <summary>GPS ground speed. If unknown, set to: UINT16_MAX  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("GPS ground speed. If unknown, set to: UINT16_MAX")]
        public  ushort vel;
            /// <summary>Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX")]
        public  ushort cog;
            /// <summary>GPS fix type. GPS_FIX_TYPE  </summary>
        [Units("")]
        [Description("GPS fix type.")]
        public  /*GPS_FIX_TYPE*/byte fix_type;
            /// <summary>Number of satellites visible. If unknown, set to 255   </summary>
        [Units("")]
        [Description("Number of satellites visible. If unknown, set to 255")]
        public  byte satellites_visible;
            /// <summary>Altitude (above WGS84, EGM96 ellipsoid). Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (above WGS84, EGM96 ellipsoid). Positive for up.")]
        public  int alt_ellipsoid;
            /// <summary>Position uncertainty. Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Position uncertainty. Positive for up.")]
        public  uint h_acc;
            /// <summary>Altitude uncertainty. Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude uncertainty. Positive for up.")]
        public  uint v_acc;
            /// <summary>Speed uncertainty. Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Speed uncertainty. Positive for up.")]
        public  uint vel_acc;
            /// <summary>Heading / track uncertainty  [degE5] </summary>
        [Units("[degE5]")]
        [Description("Heading / track uncertainty")]
        public  uint hdg_acc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=101)]
    ///<summary> The positioning status, as reported by GPS. This message is intended to display status information about each satellite visible to the receiver. See message GLOBAL_POSITION for the global position estimate. This message can contain information for up to 20 satellites. </summary>
    public struct mavlink_gps_status_t
    {
        /// <summary>Number of satellites visible   </summary>
        [Units("")]
        [Description("Number of satellites visible")]
        public  byte satellites_visible;
            /// <summary>Global satellite ID   </summary>
        [Units("")]
        [Description("Global satellite ID")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_prn;
            /// <summary>0: Satellite not used, 1: used for localization   </summary>
        [Units("")]
        [Description("0: Satellite not used, 1: used for localization")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_used;
            /// <summary>Elevation (0: right on top of receiver, 90: on the horizon) of satellite  [deg] </summary>
        [Units("[deg]")]
        [Description("Elevation (0: right on top of receiver, 90: on the horizon) of satellite")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_elevation;
            /// <summary>Direction of satellite, 0: 0 deg, 255: 360 deg.  [deg] </summary>
        [Units("[deg]")]
        [Description("Direction of satellite, 0: 0 deg, 255: 360 deg.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_azimuth;
            /// <summary>Signal to noise ratio of satellite  [dB] </summary>
        [Units("[dB]")]
        [Description("Signal to noise ratio of satellite")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)]
		public byte[] satellite_snr;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The RAW IMU readings for the usual 9DOF sensor setup. This message should contain the scaled values to the described units </summary>
    public struct mavlink_scaled_imu_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>X acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("X acceleration")]
        public  short xacc;
            /// <summary>Y acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Y acceleration")]
        public  short yacc;
            /// <summary>Z acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Z acceleration")]
        public  short zacc;
            /// <summary>Angular speed around X axis  [mrad/s] </summary>
        [Units("[mrad/s]")]
        [Description("Angular speed around X axis")]
        public  short xgyro;
            /// <summary>Angular speed around Y axis  [mrad/s] </summary>
        [Units("[mrad/s]")]
        [Description("Angular speed around Y axis")]
        public  short ygyro;
            /// <summary>Angular speed around Z axis  [mrad/s] </summary>
        [Units("[mrad/s]")]
        [Description("Angular speed around Z axis")]
        public  short zgyro;
            /// <summary>X Magnetic field  [mT] </summary>
        [Units("[mT]")]
        [Description("X Magnetic field")]
        public  short xmag;
            /// <summary>Y Magnetic field  [mT] </summary>
        [Units("[mT]")]
        [Description("Y Magnetic field")]
        public  short ymag;
            /// <summary>Z Magnetic field  [mT] </summary>
        [Units("[mT]")]
        [Description("Z Magnetic field")]
        public  short zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    ///<summary> The RAW IMU readings for the usual 9DOF sensor setup. This message should always contain the true raw values without any scaling to allow data capture and system debugging. </summary>
    public struct mavlink_raw_imu_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>X acceleration (raw)   </summary>
        [Units("")]
        [Description("X acceleration (raw)")]
        public  short xacc;
            /// <summary>Y acceleration (raw)   </summary>
        [Units("")]
        [Description("Y acceleration (raw)")]
        public  short yacc;
            /// <summary>Z acceleration (raw)   </summary>
        [Units("")]
        [Description("Z acceleration (raw)")]
        public  short zacc;
            /// <summary>Angular speed around X axis (raw)   </summary>
        [Units("")]
        [Description("Angular speed around X axis (raw)")]
        public  short xgyro;
            /// <summary>Angular speed around Y axis (raw)   </summary>
        [Units("")]
        [Description("Angular speed around Y axis (raw)")]
        public  short ygyro;
            /// <summary>Angular speed around Z axis (raw)   </summary>
        [Units("")]
        [Description("Angular speed around Z axis (raw)")]
        public  short zgyro;
            /// <summary>X Magnetic field (raw)   </summary>
        [Units("")]
        [Description("X Magnetic field (raw)")]
        public  short xmag;
            /// <summary>Y Magnetic field (raw)   </summary>
        [Units("")]
        [Description("Y Magnetic field (raw)")]
        public  short ymag;
            /// <summary>Z Magnetic field (raw)   </summary>
        [Units("")]
        [Description("Z Magnetic field (raw)")]
        public  short zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=16)]
    ///<summary> The RAW pressure readings for the typical setup of one absolute pressure and one differential pressure sensor. The sensor values should be the raw, UNSCALED ADC values. </summary>
    public struct mavlink_raw_pressure_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Absolute pressure (raw)   </summary>
        [Units("")]
        [Description("Absolute pressure (raw)")]
        public  short press_abs;
            /// <summary>Differential pressure 1 (raw, 0 if nonexistent)   </summary>
        [Units("")]
        [Description("Differential pressure 1 (raw, 0 if nonexistent)")]
        public  short press_diff1;
            /// <summary>Differential pressure 2 (raw, 0 if nonexistent)   </summary>
        [Units("")]
        [Description("Differential pressure 2 (raw, 0 if nonexistent)")]
        public  short press_diff2;
            /// <summary>Raw Temperature measurement (raw)   </summary>
        [Units("")]
        [Description("Raw Temperature measurement (raw)")]
        public  short temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> The pressure readings for the typical setup of one absolute and differential pressure sensor. The units are as specified in each field. </summary>
    public struct mavlink_scaled_pressure_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Absolute pressure  [hPa] </summary>
        [Units("[hPa]")]
        [Description("Absolute pressure")]
        public  float press_abs;
            /// <summary>Differential pressure 1  [hPa] </summary>
        [Units("[hPa]")]
        [Description("Differential pressure 1")]
        public  float press_diff;
            /// <summary>Temperature  [cdegC] </summary>
        [Units("[cdegC]")]
        [Description("Temperature")]
        public  short temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> The attitude in the aeronautical frame (right-handed, Z-down, X-front, Y-right). </summary>
    public struct mavlink_attitude_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Roll angle (-pi..+pi)  [rad] </summary>
        [Units("[rad]")]
        [Description("Roll angle (-pi..+pi)")]
        public  float roll;
            /// <summary>Pitch angle (-pi..+pi)  [rad] </summary>
        [Units("[rad]")]
        [Description("Pitch angle (-pi..+pi)")]
        public  float pitch;
            /// <summary>Yaw angle (-pi..+pi)  [rad] </summary>
        [Units("[rad]")]
        [Description("Yaw angle (-pi..+pi)")]
        public  float yaw;
            /// <summary>Roll angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Roll angular speed")]
        public  float rollspeed;
            /// <summary>Pitch angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Pitch angular speed")]
        public  float pitchspeed;
            /// <summary>Yaw angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Yaw angular speed")]
        public  float yawspeed;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    ///<summary> The attitude in the aeronautical frame (right-handed, Z-down, X-front, Y-right), expressed as quaternion. Quaternion order is w, x, y, z and a zero rotation would be expressed as (1 0 0 0). </summary>
    public struct mavlink_attitude_quaternion_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Quaternion component 1, w (1 in null-rotation)   </summary>
        [Units("")]
        [Description("Quaternion component 1, w (1 in null-rotation)")]
        public  float q1;
            /// <summary>Quaternion component 2, x (0 in null-rotation)   </summary>
        [Units("")]
        [Description("Quaternion component 2, x (0 in null-rotation)")]
        public  float q2;
            /// <summary>Quaternion component 3, y (0 in null-rotation)   </summary>
        [Units("")]
        [Description("Quaternion component 3, y (0 in null-rotation)")]
        public  float q3;
            /// <summary>Quaternion component 4, z (0 in null-rotation)   </summary>
        [Units("")]
        [Description("Quaternion component 4, z (0 in null-rotation)")]
        public  float q4;
            /// <summary>Roll angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Roll angular speed")]
        public  float rollspeed;
            /// <summary>Pitch angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Pitch angular speed")]
        public  float pitchspeed;
            /// <summary>Yaw angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Yaw angular speed")]
        public  float yawspeed;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> The filtered local position (e.g. fused computer vision and accelerometers). Coordinate frame is right-handed, Z-axis down (aeronautical frame, NED / north-east-down convention) </summary>
    public struct mavlink_local_position_ned_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>X Position  [m] </summary>
        [Units("[m]")]
        [Description("X Position")]
        public  float x;
            /// <summary>Y Position  [m] </summary>
        [Units("[m]")]
        [Description("Y Position")]
        public  float y;
            /// <summary>Z Position  [m] </summary>
        [Units("[m]")]
        [Description("Z Position")]
        public  float z;
            /// <summary>X Speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("X Speed")]
        public  float vx;
            /// <summary>Y Speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Y Speed")]
        public  float vy;
            /// <summary>Z Speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Z Speed")]
        public  float vz;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> The filtered global position (e.g. fused GPS and accelerometers). The position is in GPS-frame (right-handed, Z-up). It    
///               is designed as scaled integer message since the resolution of float is not sufficient. </summary>
    public struct mavlink_global_position_int_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Latitude, expressed  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude, expressed")]
        public  int lat;
            /// <summary>Longitude, expressed  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude, expressed")]
        public  int lon;
            /// <summary>Altitude (MSL). Note that virtually all GPS modules provide both WGS84 and MSL.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (MSL). Note that virtually all GPS modules provide both WGS84 and MSL.")]
        public  int alt;
            /// <summary>Altitude above ground  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude above ground")]
        public  int relative_alt;
            /// <summary>Ground X Speed (Latitude, positive north)  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Ground X Speed (Latitude, positive north)")]
        public  short vx;
            /// <summary>Ground Y Speed (Longitude, positive east)  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Ground Y Speed (Longitude, positive east)")]
        public  short vy;
            /// <summary>Ground Z Speed (Altitude, positive down)  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Ground Z Speed (Altitude, positive down)")]
        public  short vz;
            /// <summary>Vehicle heading (yaw angle), 0.0..359.99 degrees. If unknown, set to: UINT16_MAX  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("Vehicle heading (yaw angle), 0.0..359.99 degrees. If unknown, set to: UINT16_MAX")]
        public  ushort hdg;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The scaled values of the RC channels received: (-100%) -10000, (0%) 0, (100%) 10000. Channels that are inactive should be set to UINT16_MAX. </summary>
    public struct mavlink_rc_channels_scaled_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>RC channel 1 value scaled.   </summary>
        [Units("")]
        [Description("RC channel 1 value scaled.")]
        public  short chan1_scaled;
            /// <summary>RC channel 2 value scaled.   </summary>
        [Units("")]
        [Description("RC channel 2 value scaled.")]
        public  short chan2_scaled;
            /// <summary>RC channel 3 value scaled.   </summary>
        [Units("")]
        [Description("RC channel 3 value scaled.")]
        public  short chan3_scaled;
            /// <summary>RC channel 4 value scaled.   </summary>
        [Units("")]
        [Description("RC channel 4 value scaled.")]
        public  short chan4_scaled;
            /// <summary>RC channel 5 value scaled.   </summary>
        [Units("")]
        [Description("RC channel 5 value scaled.")]
        public  short chan5_scaled;
            /// <summary>RC channel 6 value scaled.   </summary>
        [Units("")]
        [Description("RC channel 6 value scaled.")]
        public  short chan6_scaled;
            /// <summary>RC channel 7 value scaled.   </summary>
        [Units("")]
        [Description("RC channel 7 value scaled.")]
        public  short chan7_scaled;
            /// <summary>RC channel 8 value scaled.   </summary>
        [Units("")]
        [Description("RC channel 8 value scaled.")]
        public  short chan8_scaled;
            /// <summary>Servo output port (set of 8 outputs = 1 port). Flight stacks running on Pixhawk should use: 0 = MAIN, 1 = AUX.   </summary>
        [Units("")]
        [Description("Servo output port (set of 8 outputs = 1 port). Flight stacks running on Pixhawk should use: 0 = MAIN, 1 = AUX.")]
        public  byte port;
            /// <summary>Receive signal strength indicator in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.   </summary>
        [Units("")]
        [Description("Receive signal strength indicator in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.")]
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The RAW values of the RC channels received. The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%. A value of UINT16_MAX implies the channel is unused. Individual receivers/transmitters might violate this specification. </summary>
    public struct mavlink_rc_channels_raw_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>RC channel 1 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 1 value.")]
        public  ushort chan1_raw;
            /// <summary>RC channel 2 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 2 value.")]
        public  ushort chan2_raw;
            /// <summary>RC channel 3 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 3 value.")]
        public  ushort chan3_raw;
            /// <summary>RC channel 4 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 4 value.")]
        public  ushort chan4_raw;
            /// <summary>RC channel 5 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 5 value.")]
        public  ushort chan5_raw;
            /// <summary>RC channel 6 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 6 value.")]
        public  ushort chan6_raw;
            /// <summary>RC channel 7 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 7 value.")]
        public  ushort chan7_raw;
            /// <summary>RC channel 8 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 8 value.")]
        public  ushort chan8_raw;
            /// <summary>Servo output port (set of 8 outputs = 1 port). Flight stacks running on Pixhawk should use: 0 = MAIN, 1 = AUX.   </summary>
        [Units("")]
        [Description("Servo output port (set of 8 outputs = 1 port). Flight stacks running on Pixhawk should use: 0 = MAIN, 1 = AUX.")]
        public  byte port;
            /// <summary>Receive signal strength indicator in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.   </summary>
        [Units("")]
        [Description("Receive signal strength indicator in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.")]
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    ///<summary> The RAW values of the servo outputs (for RC input from the remote, use the RC_CHANNELS messages). The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%. </summary>
    public struct mavlink_servo_output_raw_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  uint time_usec;
            /// <summary>Servo output 1 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 1 value")]
        public  ushort servo1_raw;
            /// <summary>Servo output 2 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 2 value")]
        public  ushort servo2_raw;
            /// <summary>Servo output 3 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 3 value")]
        public  ushort servo3_raw;
            /// <summary>Servo output 4 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 4 value")]
        public  ushort servo4_raw;
            /// <summary>Servo output 5 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 5 value")]
        public  ushort servo5_raw;
            /// <summary>Servo output 6 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 6 value")]
        public  ushort servo6_raw;
            /// <summary>Servo output 7 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 7 value")]
        public  ushort servo7_raw;
            /// <summary>Servo output 8 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 8 value")]
        public  ushort servo8_raw;
            /// <summary>Servo output port (set of 8 outputs = 1 port). Flight stacks running on Pixhawk should use: 0 = MAIN, 1 = AUX.   </summary>
        [Units("")]
        [Description("Servo output port (set of 8 outputs = 1 port). Flight stacks running on Pixhawk should use: 0 = MAIN, 1 = AUX.")]
        public  byte port;
            /// <summary>Servo output 9 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 9 value")]
        public  ushort servo9_raw;
            /// <summary>Servo output 10 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 10 value")]
        public  ushort servo10_raw;
            /// <summary>Servo output 11 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 11 value")]
        public  ushort servo11_raw;
            /// <summary>Servo output 12 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 12 value")]
        public  ushort servo12_raw;
            /// <summary>Servo output 13 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 13 value")]
        public  ushort servo13_raw;
            /// <summary>Servo output 14 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 14 value")]
        public  ushort servo14_raw;
            /// <summary>Servo output 15 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 15 value")]
        public  ushort servo15_raw;
            /// <summary>Servo output 16 value  [us] </summary>
        [Units("[us]")]
        [Description("Servo output 16 value")]
        public  ushort servo16_raw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=7)]
    ///<summary> Request a partial list of mission items from the system/component. https://mavlink.io/en/services/mission.html. If start and end index are the same, just send one waypoint. </summary>
    public struct mavlink_mission_request_partial_list_t
    {
        /// <summary>Start index   </summary>
        [Units("")]
        [Description("Start index")]
        public  short start_index;
            /// <summary>End index, -1 by default (-1: send list to end). Else a valid index of the list   </summary>
        [Units("")]
        [Description("End index, -1 by default (-1: send list to end). Else a valid index of the list")]
        public  short end_index;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=7)]
    ///<summary> This message is sent to the MAV to write a partial list. If start index == end index, only one item will be transmitted / updated. If the start index is NOT 0 and above the current list size, this request should be REJECTED! </summary>
    public struct mavlink_mission_write_partial_list_t
    {
        /// <summary>Start index. Must be smaller / equal to the largest index of the current onboard list.   </summary>
        [Units("")]
        [Description("Start index. Must be smaller / equal to the largest index of the current onboard list.")]
        public  short start_index;
            /// <summary>End index, equal or greater than start index.   </summary>
        [Units("")]
        [Description("End index, equal or greater than start index.")]
        public  short end_index;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=38)]
    ///<summary> Message encoding a mission item. This message is emitted to announce    
///                the presence of a mission item and to set a mission item on the system. The mission item can be either in x, y, z meters (type: LOCAL) or x:lat, y:lon, z:altitude. Local frame is Z-down, right handed (NED), global frame is Z-up, right handed (ENU). See also https://mavlink.io/en/services/mission.html. </summary>
    public struct mavlink_mission_item_t
    {
        /// <summary>PARAM1, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM1, see MAV_CMD enum")]
        public  float param1;
            /// <summary>PARAM2, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM2, see MAV_CMD enum")]
        public  float param2;
            /// <summary>PARAM3, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM3, see MAV_CMD enum")]
        public  float param3;
            /// <summary>PARAM4, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM4, see MAV_CMD enum")]
        public  float param4;
            /// <summary>PARAM5 / local: X coordinate, global: latitude   </summary>
        [Units("")]
        [Description("PARAM5 / local: X coordinate, global: latitude")]
        public  float x;
            /// <summary>PARAM6 / local: Y coordinate, global: longitude   </summary>
        [Units("")]
        [Description("PARAM6 / local: Y coordinate, global: longitude")]
        public  float y;
            /// <summary>PARAM7 / local: Z coordinate, global: altitude (relative or absolute, depending on frame).   </summary>
        [Units("")]
        [Description("PARAM7 / local: Z coordinate, global: altitude (relative or absolute, depending on frame).")]
        public  float z;
            /// <summary>Sequence   </summary>
        [Units("")]
        [Description("Sequence")]
        public  ushort seq;
            /// <summary>The scheduled action for the waypoint. MAV_CMD  </summary>
        [Units("")]
        [Description("The scheduled action for the waypoint.")]
        public  /*MAV_CMD*/ushort command;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>The coordinate system of the waypoint. MAV_FRAME  </summary>
        [Units("")]
        [Description("The coordinate system of the waypoint.")]
        public  /*MAV_FRAME*/byte frame;
            /// <summary>false:0, true:1   </summary>
        [Units("")]
        [Description("false:0, true:1")]
        public  byte current;
            /// <summary>Autocontinue to next waypoint   </summary>
        [Units("")]
        [Description("Autocontinue to next waypoint")]
        public  byte autocontinue;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=5)]
    ///<summary> Request the information of the mission item with the sequence number seq. The response of the system to this message should be a MISSION_ITEM message. https://mavlink.io/en/services/mission.html </summary>
    public struct mavlink_mission_request_t
    {
        /// <summary>Sequence   </summary>
        [Units("")]
        [Description("Sequence")]
        public  ushort seq;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> Set the mission item with sequence number seq as current item. This means that the MAV will continue to this mission item on the shortest path (not following the mission items in-between). </summary>
    public struct mavlink_mission_set_current_t
    {
        /// <summary>Sequence   </summary>
        [Units("")]
        [Description("Sequence")]
        public  ushort seq;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Message that announces the sequence number of the current active mission item. The MAV will fly towards this mission item. </summary>
    public struct mavlink_mission_current_t
    {
        /// <summary>Sequence   </summary>
        [Units("")]
        [Description("Sequence")]
        public  ushort seq;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Request the overall list of mission items from the system/component. </summary>
    public struct mavlink_mission_request_list_t
    {
        /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=5)]
    ///<summary> This message is emitted as response to MISSION_REQUEST_LIST by the MAV and to initiate a write transaction. The GCS can then request the individual mission item based on the knowledge of the total number of waypoints. </summary>
    public struct mavlink_mission_count_t
    {
        /// <summary>Number of mission items in the sequence   </summary>
        [Units("")]
        [Description("Number of mission items in the sequence")]
        public  ushort count;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Delete all mission items at once. </summary>
    public struct mavlink_mission_clear_all_t
    {
        /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> A certain mission item has been reached. The system will either hold this position (or circle on the orbit) or (if the autocontinue on the WP was set) continue to the next waypoint. </summary>
    public struct mavlink_mission_item_reached_t
    {
        /// <summary>Sequence   </summary>
        [Units("")]
        [Description("Sequence")]
        public  ushort seq;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> Acknowledgment message during waypoint handling. The type field states if this message is a positive ack (type=0) or if an error happened (type=non-zero). </summary>
    public struct mavlink_mission_ack_t
    {
        /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Mission result. MAV_MISSION_RESULT  </summary>
        [Units("")]
        [Description("Mission result.")]
        public  /*MAV_MISSION_RESULT*/byte type;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=21)]
    ///<summary> As local waypoints exist, the global waypoint reference allows to transform between the local coordinate frame and the global (GPS) coordinate frame. This can be necessary when e.g. in- and outdoor settings are connected and the MAV should move from in- to outdoor. </summary>
    public struct mavlink_set_gps_global_origin_t
    {
        /// <summary>Latitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude (WGS84)")]
        public  int latitude;
            /// <summary>Longitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude (WGS84)")]
        public  int longitude;
            /// <summary>Altitude (MSL). Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (MSL). Positive for up.")]
        public  int altitude;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Once the MAV sets a new GPS-Local correspondence, this message announces the origin (0,0,0) position </summary>
    public struct mavlink_gps_global_origin_t
    {
        /// <summary>Latitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude (WGS84)")]
        public  int latitude;
            /// <summary>Longitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude (WGS84)")]
        public  int longitude;
            /// <summary>Altitude (MSL). Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (MSL). Positive for up.")]
        public  int altitude;
            /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    ///<summary> Bind a RC channel to a parameter. The parameter should change according to the RC channel value. </summary>
    public struct mavlink_param_map_rc_t
    {
        /// <summary>Initial parameter value   </summary>
        [Units("")]
        [Description("Initial parameter value")]
        public  float param_value0;
            /// <summary>Scale, maps the RC range [-1, 1] to a parameter value   </summary>
        [Units("")]
        [Description("Scale, maps the RC range [-1, 1] to a parameter value")]
        public  float scale;
            /// <summary>Minimum param value. The protocol does not define if this overwrites an onboard minimum value. (Depends on implementation)   </summary>
        [Units("")]
        [Description("Minimum param value. The protocol does not define if this overwrites an onboard minimum value. (Depends on implementation)")]
        public  float param_value_min;
            /// <summary>Maximum param value. The protocol does not define if this overwrites an onboard maximum value. (Depends on implementation)   </summary>
        [Units("")]
        [Description("Maximum param value. The protocol does not define if this overwrites an onboard maximum value. (Depends on implementation)")]
        public  float param_value_max;
            /// <summary>Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored), send -2 to disable any existing map for this rc_channel_index.   </summary>
        [Units("")]
        [Description("Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored), send -2 to disable any existing map for this rc_channel_index.")]
        public  short param_index;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string   </summary>
        [Units("")]
        [Description("Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] param_id;
            /// <summary>Index of parameter RC channel. Not equal to the RC channel id. Typically corresponds to a potentiometer-knob on the RC.   </summary>
        [Units("")]
        [Description("Index of parameter RC channel. Not equal to the RC channel id. Typically corresponds to a potentiometer-knob on the RC.")]
        public  byte parameter_rc_channel_index;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=5)]
    ///<summary> Request the information of the mission item with the sequence number seq. The response of the system to this message should be a MISSION_ITEM_INT message. https://mavlink.io/en/services/mission.html </summary>
    public struct mavlink_mission_request_int_t
    {
        /// <summary>Sequence   </summary>
        [Units("")]
        [Description("Sequence")]
        public  ushort seq;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=27)]
    ///<summary> Set a safety zone (volume), which is defined by two corners of a cube. This message can be used to tell the MAV which setpoints/waypoints to accept and which to reject. Safety areas are often enforced by national or competition regulations. </summary>
    public struct mavlink_safety_set_allowed_area_t
    {
        /// <summary>x position 1 / Latitude 1  [m] </summary>
        [Units("[m]")]
        [Description("x position 1 / Latitude 1")]
        public  float p1x;
            /// <summary>y position 1 / Longitude 1  [m] </summary>
        [Units("[m]")]
        [Description("y position 1 / Longitude 1")]
        public  float p1y;
            /// <summary>z position 1 / Altitude 1  [m] </summary>
        [Units("[m]")]
        [Description("z position 1 / Altitude 1")]
        public  float p1z;
            /// <summary>x position 2 / Latitude 2  [m] </summary>
        [Units("[m]")]
        [Description("x position 2 / Latitude 2")]
        public  float p2x;
            /// <summary>y position 2 / Longitude 2  [m] </summary>
        [Units("[m]")]
        [Description("y position 2 / Longitude 2")]
        public  float p2y;
            /// <summary>z position 2 / Altitude 2  [m] </summary>
        [Units("[m]")]
        [Description("z position 2 / Altitude 2")]
        public  float p2z;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Coordinate frame. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down. MAV_FRAME  </summary>
        [Units("")]
        [Description("Coordinate frame. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down.")]
        public  /*MAV_FRAME*/byte frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=25)]
    ///<summary> Read out the safety zone the MAV currently assumes. </summary>
    public struct mavlink_safety_allowed_area_t
    {
        /// <summary>x position 1 / Latitude 1  [m] </summary>
        [Units("[m]")]
        [Description("x position 1 / Latitude 1")]
        public  float p1x;
            /// <summary>y position 1 / Longitude 1  [m] </summary>
        [Units("[m]")]
        [Description("y position 1 / Longitude 1")]
        public  float p1y;
            /// <summary>z position 1 / Altitude 1  [m] </summary>
        [Units("[m]")]
        [Description("z position 1 / Altitude 1")]
        public  float p1z;
            /// <summary>x position 2 / Latitude 2  [m] </summary>
        [Units("[m]")]
        [Description("x position 2 / Latitude 2")]
        public  float p2x;
            /// <summary>y position 2 / Longitude 2  [m] </summary>
        [Units("[m]")]
        [Description("y position 2 / Longitude 2")]
        public  float p2y;
            /// <summary>z position 2 / Altitude 2  [m] </summary>
        [Units("[m]")]
        [Description("z position 2 / Altitude 2")]
        public  float p2z;
            /// <summary>Coordinate frame. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down. MAV_FRAME  </summary>
        [Units("")]
        [Description("Coordinate frame. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down.")]
        public  /*MAV_FRAME*/byte frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=72)]
    ///<summary> The attitude in the aeronautical frame (right-handed, Z-down, X-front, Y-right), expressed as quaternion. Quaternion order is w, x, y, z and a zero rotation would be expressed as (1 0 0 0). </summary>
    public struct mavlink_attitude_quaternion_cov_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation)   </summary>
        [Units("")]
        [Description("Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Roll angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Roll angular speed")]
        public  float rollspeed;
            /// <summary>Pitch angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Pitch angular speed")]
        public  float pitchspeed;
            /// <summary>Yaw angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Yaw angular speed")]
        public  float yawspeed;
            /// <summary>Row-major representation of a 3x3 attitude covariance matrix (states: roll, pitch, yaw; first three entries are the first ROW, next three entries are the second row, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of a 3x3 attitude covariance matrix (states: roll, pitch, yaw; first three entries are the first ROW, next three entries are the second row, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=26)]
    ///<summary> The state of the fixed wing navigation and position controller. </summary>
    public struct mavlink_nav_controller_output_t
    {
        /// <summary>Current desired roll  [deg] </summary>
        [Units("[deg]")]
        [Description("Current desired roll")]
        public  float nav_roll;
            /// <summary>Current desired pitch  [deg] </summary>
        [Units("[deg]")]
        [Description("Current desired pitch")]
        public  float nav_pitch;
            /// <summary>Current altitude error  [m] </summary>
        [Units("[m]")]
        [Description("Current altitude error")]
        public  float alt_error;
            /// <summary>Current airspeed error  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Current airspeed error")]
        public  float aspd_error;
            /// <summary>Current crosstrack error on x-y plane  [m] </summary>
        [Units("[m]")]
        [Description("Current crosstrack error on x-y plane")]
        public  float xtrack_error;
            /// <summary>Current desired heading  [deg] </summary>
        [Units("[deg]")]
        [Description("Current desired heading")]
        public  short nav_bearing;
            /// <summary>Bearing to current waypoint/target  [deg] </summary>
        [Units("[deg]")]
        [Description("Bearing to current waypoint/target")]
        public  short target_bearing;
            /// <summary>Distance to active waypoint  [m] </summary>
        [Units("[m]")]
        [Description("Distance to active waypoint")]
        public  ushort wp_dist;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=181)]
    ///<summary> The filtered global position (e.g. fused GPS and accelerometers). The position is in GPS-frame (right-handed, Z-up). It  is designed as scaled integer message since the resolution of float is not sufficient. NOTE: This message is intended for onboard networks / companion computers and higher-bandwidth links and optimized for accuracy and completeness. Please use the GLOBAL_POSITION_INT message for a minimal subset. </summary>
    public struct mavlink_global_position_int_cov_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Latitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude")]
        public  int lat;
            /// <summary>Longitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude")]
        public  int lon;
            /// <summary>Altitude in meters above MSL  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude in meters above MSL")]
        public  int alt;
            /// <summary>Altitude above ground  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude above ground")]
        public  int relative_alt;
            /// <summary>Ground X Speed (Latitude)  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Ground X Speed (Latitude)")]
        public  float vx;
            /// <summary>Ground Y Speed (Longitude)  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Ground Y Speed (Longitude)")]
        public  float vy;
            /// <summary>Ground Z Speed (Altitude)  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Ground Z Speed (Altitude)")]
        public  float vz;
            /// <summary>Row-major representation of a 6x6 position and velocity 6x6 cross-covariance matrix (states: lat, lon, alt, vx, vy, vz; first six entries are the first ROW, next six entries are the second row, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of a 6x6 position and velocity 6x6 cross-covariance matrix (states: lat, lon, alt, vx, vy, vz; first six entries are the first ROW, next six entries are the second row, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)]
		public float[] covariance;
            /// <summary>Class id of the estimator this estimate originated from. MAV_ESTIMATOR_TYPE  </summary>
        [Units("")]
        [Description("Class id of the estimator this estimate originated from.")]
        public  /*MAV_ESTIMATOR_TYPE*/byte estimator_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=225)]
    ///<summary> The filtered local position (e.g. fused computer vision and accelerometers). Coordinate frame is right-handed, Z-axis down (aeronautical frame, NED / north-east-down convention) </summary>
    public struct mavlink_local_position_ned_cov_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>X Position  [m] </summary>
        [Units("[m]")]
        [Description("X Position")]
        public  float x;
            /// <summary>Y Position  [m] </summary>
        [Units("[m]")]
        [Description("Y Position")]
        public  float y;
            /// <summary>Z Position  [m] </summary>
        [Units("[m]")]
        [Description("Z Position")]
        public  float z;
            /// <summary>X Speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("X Speed")]
        public  float vx;
            /// <summary>Y Speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Y Speed")]
        public  float vy;
            /// <summary>Z Speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Z Speed")]
        public  float vz;
            /// <summary>X Acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X Acceleration")]
        public  float ax;
            /// <summary>Y Acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y Acceleration")]
        public  float ay;
            /// <summary>Z Acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z Acceleration")]
        public  float az;
            /// <summary>Row-major representation of position, velocity and acceleration 9x9 cross-covariance matrix upper right triangle (states: x, y, z, vx, vy, vz, ax, ay, az; first nine entries are the first ROW, next eight entries are the second row, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of position, velocity and acceleration 9x9 cross-covariance matrix upper right triangle (states: x, y, z, vx, vy, vz, ax, ay, az; first nine entries are the first ROW, next eight entries are the second row, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=45)]
		public float[] covariance;
            /// <summary>Class id of the estimator this estimate originated from. MAV_ESTIMATOR_TYPE  </summary>
        [Units("")]
        [Description("Class id of the estimator this estimate originated from.")]
        public  /*MAV_ESTIMATOR_TYPE*/byte estimator_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> The PPM values of the RC channels received. The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%.  A value of UINT16_MAX implies the channel is unused. Individual receivers/transmitters might violate this specification. </summary>
    public struct mavlink_rc_channels_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>RC channel 1 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 1 value.")]
        public  ushort chan1_raw;
            /// <summary>RC channel 2 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 2 value.")]
        public  ushort chan2_raw;
            /// <summary>RC channel 3 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 3 value.")]
        public  ushort chan3_raw;
            /// <summary>RC channel 4 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 4 value.")]
        public  ushort chan4_raw;
            /// <summary>RC channel 5 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 5 value.")]
        public  ushort chan5_raw;
            /// <summary>RC channel 6 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 6 value.")]
        public  ushort chan6_raw;
            /// <summary>RC channel 7 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 7 value.")]
        public  ushort chan7_raw;
            /// <summary>RC channel 8 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 8 value.")]
        public  ushort chan8_raw;
            /// <summary>RC channel 9 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 9 value.")]
        public  ushort chan9_raw;
            /// <summary>RC channel 10 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 10 value.")]
        public  ushort chan10_raw;
            /// <summary>RC channel 11 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 11 value.")]
        public  ushort chan11_raw;
            /// <summary>RC channel 12 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 12 value.")]
        public  ushort chan12_raw;
            /// <summary>RC channel 13 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 13 value.")]
        public  ushort chan13_raw;
            /// <summary>RC channel 14 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 14 value.")]
        public  ushort chan14_raw;
            /// <summary>RC channel 15 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 15 value.")]
        public  ushort chan15_raw;
            /// <summary>RC channel 16 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 16 value.")]
        public  ushort chan16_raw;
            /// <summary>RC channel 17 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 17 value.")]
        public  ushort chan17_raw;
            /// <summary>RC channel 18 value.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 18 value.")]
        public  ushort chan18_raw;
            /// <summary>Total number of RC channels being received. This can be larger than 18, indicating that more channels are available but not given in this message. This value should be 0 when no RC channels are available.   </summary>
        [Units("")]
        [Description("Total number of RC channels being received. This can be larger than 18, indicating that more channels are available but not given in this message. This value should be 0 when no RC channels are available.")]
        public  byte chancount;
            /// <summary>Receive signal strength indicator in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.   </summary>
        [Units("")]
        [Description("Receive signal strength indicator in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.")]
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Request a data stream. </summary>
    public struct mavlink_request_data_stream_t
    {
        /// <summary>The requested message rate  [Hz] </summary>
        [Units("[Hz]")]
        [Description("The requested message rate")]
        public  ushort req_message_rate;
            /// <summary>The target requested to send the message stream.   </summary>
        [Units("")]
        [Description("The target requested to send the message stream.")]
        public  byte target_system;
            /// <summary>The target requested to send the message stream.   </summary>
        [Units("")]
        [Description("The target requested to send the message stream.")]
        public  byte target_component;
            /// <summary>The ID of the requested data stream   </summary>
        [Units("")]
        [Description("The ID of the requested data stream")]
        public  byte req_stream_id;
            /// <summary>1 to start sending, 0 to stop sending.   </summary>
        [Units("")]
        [Description("1 to start sending, 0 to stop sending.")]
        public  byte start_stop;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> Data stream status information. </summary>
    public struct mavlink_data_stream_t
    {
        /// <summary>The message rate  [Hz] </summary>
        [Units("[Hz]")]
        [Description("The message rate")]
        public  ushort message_rate;
            /// <summary>The ID of the requested data stream   </summary>
        [Units("")]
        [Description("The ID of the requested data stream")]
        public  byte stream_id;
            /// <summary>1 stream is enabled, 0 stream is stopped.   </summary>
        [Units("")]
        [Description("1 stream is enabled, 0 stream is stopped.")]
        public  byte on_off;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=11)]
    ///<summary> This message provides an API for manually controlling the vehicle using standard joystick axes nomenclature, along with a joystick-like input device. Unused axes can be disabled an buttons are also transmit as boolean values of their  </summary>
    public struct mavlink_manual_control_t
    {
        /// <summary>X-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to forward(1000)-backward(-1000) movement on a joystick and the pitch of a vehicle.   </summary>
        [Units("")]
        [Description("X-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to forward(1000)-backward(-1000) movement on a joystick and the pitch of a vehicle.")]
        public  short x;
            /// <summary>Y-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to left(-1000)-right(1000) movement on a joystick and the roll of a vehicle.   </summary>
        [Units("")]
        [Description("Y-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to left(-1000)-right(1000) movement on a joystick and the roll of a vehicle.")]
        public  short y;
            /// <summary>Z-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a separate slider movement with maximum being 1000 and minimum being -1000 on a joystick and the thrust of a vehicle. Positive values are positive thrust, negative values are negative thrust.   </summary>
        [Units("")]
        [Description("Z-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a separate slider movement with maximum being 1000 and minimum being -1000 on a joystick and the thrust of a vehicle. Positive values are positive thrust, negative values are negative thrust.")]
        public  short z;
            /// <summary>R-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a twisting of the joystick, with counter-clockwise being 1000 and clockwise being -1000, and the yaw of a vehicle.   </summary>
        [Units("")]
        [Description("R-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a twisting of the joystick, with counter-clockwise being 1000 and clockwise being -1000, and the yaw of a vehicle.")]
        public  short r;
            /// <summary>A bitfield corresponding to the joystick buttons' current state, 1 for pressed, 0 for released. The lowest bit corresponds to Button 1.   </summary>
        [Units("")]
        [Description("A bitfield corresponding to the joystick buttons' current state, 1 for pressed, 0 for released. The lowest bit corresponds to Button 1.")]
        public  ushort buttons;
            /// <summary>The system to be controlled.   </summary>
        [Units("")]
        [Description("The system to be controlled.")]
        public  byte target;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=38)]
    ///<summary> The RAW values of the RC channels sent to the MAV to override info received from the RC radio. A value of UINT16_MAX means no change to that channel. A value of 0 means control of that channel should be released back to the RC radio. The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%. Individual receivers/transmitters might violate this specification. </summary>
    public struct mavlink_rc_channels_override_t
    {
        /// <summary>RC channel 1 value. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 1 value. A value of UINT16_MAX means to ignore this field.")]
        public  ushort chan1_raw;
            /// <summary>RC channel 2 value. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 2 value. A value of UINT16_MAX means to ignore this field.")]
        public  ushort chan2_raw;
            /// <summary>RC channel 3 value. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 3 value. A value of UINT16_MAX means to ignore this field.")]
        public  ushort chan3_raw;
            /// <summary>RC channel 4 value. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 4 value. A value of UINT16_MAX means to ignore this field.")]
        public  ushort chan4_raw;
            /// <summary>RC channel 5 value. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 5 value. A value of UINT16_MAX means to ignore this field.")]
        public  ushort chan5_raw;
            /// <summary>RC channel 6 value. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 6 value. A value of UINT16_MAX means to ignore this field.")]
        public  ushort chan6_raw;
            /// <summary>RC channel 7 value. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 7 value. A value of UINT16_MAX means to ignore this field.")]
        public  ushort chan7_raw;
            /// <summary>RC channel 8 value. A value of UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 8 value. A value of UINT16_MAX means to ignore this field.")]
        public  ushort chan8_raw;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>RC channel 9 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 9 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan9_raw;
            /// <summary>RC channel 10 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 10 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan10_raw;
            /// <summary>RC channel 11 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 11 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan11_raw;
            /// <summary>RC channel 12 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 12 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan12_raw;
            /// <summary>RC channel 13 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 13 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan13_raw;
            /// <summary>RC channel 14 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 14 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan14_raw;
            /// <summary>RC channel 15 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 15 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan15_raw;
            /// <summary>RC channel 16 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 16 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan16_raw;
            /// <summary>RC channel 17 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 17 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan17_raw;
            /// <summary>RC channel 18 value. A value of 0 or UINT16_MAX means to ignore this field.  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 18 value. A value of 0 or UINT16_MAX means to ignore this field.")]
        public  ushort chan18_raw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=38)]
    ///<summary> Message encoding a mission item. This message is emitted to announce    
///                the presence of a mission item and to set a mission item on the system. The mission item can be either in x, y, z meters (type: LOCAL) or x:lat, y:lon, z:altitude. Local frame is Z-down, right handed (NED), global frame is Z-up, right handed (ENU). See also https://mavlink.io/en/services/mission.html. </summary>
    public struct mavlink_mission_item_int_t
    {
        /// <summary>PARAM1, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM1, see MAV_CMD enum")]
        public  float param1;
            /// <summary>PARAM2, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM2, see MAV_CMD enum")]
        public  float param2;
            /// <summary>PARAM3, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM3, see MAV_CMD enum")]
        public  float param3;
            /// <summary>PARAM4, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM4, see MAV_CMD enum")]
        public  float param4;
            /// <summary>PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7   </summary>
        [Units("")]
        [Description("PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7")]
        public  int x;
            /// <summary>PARAM6 / y position: local: x position in meters * 1e4, global: longitude in degrees *10^7   </summary>
        [Units("")]
        [Description("PARAM6 / y position: local: x position in meters * 1e4, global: longitude in degrees *10^7")]
        public  int y;
            /// <summary>PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame.   </summary>
        [Units("")]
        [Description("PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame.")]
        public  float z;
            /// <summary>Waypoint ID (sequence number). Starts at zero. Increases monotonically for each waypoint, no gaps in the sequence (0,1,2,3,4).   </summary>
        [Units("")]
        [Description("Waypoint ID (sequence number). Starts at zero. Increases monotonically for each waypoint, no gaps in the sequence (0,1,2,3,4).")]
        public  ushort seq;
            /// <summary>The scheduled action for the waypoint. MAV_CMD  </summary>
        [Units("")]
        [Description("The scheduled action for the waypoint.")]
        public  /*MAV_CMD*/ushort command;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>The coordinate system of the waypoint. MAV_FRAME  </summary>
        [Units("")]
        [Description("The coordinate system of the waypoint.")]
        public  /*MAV_FRAME*/byte frame;
            /// <summary>false:0, true:1   </summary>
        [Units("")]
        [Description("false:0, true:1")]
        public  byte current;
            /// <summary>Autocontinue to next waypoint   </summary>
        [Units("")]
        [Description("Autocontinue to next waypoint")]
        public  byte autocontinue;
            /// <summary>Mission type. MAV_MISSION_TYPE  </summary>
        [Units("")]
        [Description("Mission type.")]
        public  /*MAV_MISSION_TYPE*/byte mission_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Metrics typically displayed on a HUD for fixed wing aircraft. </summary>
    public struct mavlink_vfr_hud_t
    {
        /// <summary>Current indicated airspeed (IAS).  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Current indicated airspeed (IAS).")]
        public  float airspeed;
            /// <summary>Current ground speed.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Current ground speed.")]
        public  float groundspeed;
            /// <summary>Current altitude (MSL).  [m] </summary>
        [Units("[m]")]
        [Description("Current altitude (MSL).")]
        public  float alt;
            /// <summary>Current climb rate.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Current climb rate.")]
        public  float climb;
            /// <summary>Current heading in compass units (0-360, 0=north).  [deg] </summary>
        [Units("[deg]")]
        [Description("Current heading in compass units (0-360, 0=north).")]
        public  short heading;
            /// <summary>Current throttle setting (0 to 100).  [%] </summary>
        [Units("[%]")]
        [Description("Current throttle setting (0 to 100).")]
        public  ushort throttle;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    ///<summary> Message encoding a command with parameters as scaled integers. Scaling depends on the actual command value. </summary>
    public struct mavlink_command_int_t
    {
        /// <summary>PARAM1, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM1, see MAV_CMD enum")]
        public  float param1;
            /// <summary>PARAM2, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM2, see MAV_CMD enum")]
        public  float param2;
            /// <summary>PARAM3, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM3, see MAV_CMD enum")]
        public  float param3;
            /// <summary>PARAM4, see MAV_CMD enum   </summary>
        [Units("")]
        [Description("PARAM4, see MAV_CMD enum")]
        public  float param4;
            /// <summary>PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7   </summary>
        [Units("")]
        [Description("PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7")]
        public  int x;
            /// <summary>PARAM6 / local: y position in meters * 1e4, global: longitude in degrees * 10^7   </summary>
        [Units("")]
        [Description("PARAM6 / local: y position in meters * 1e4, global: longitude in degrees * 10^7")]
        public  int y;
            /// <summary>PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame).   </summary>
        [Units("")]
        [Description("PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame).")]
        public  float z;
            /// <summary>The scheduled action for the mission item. MAV_CMD  </summary>
        [Units("")]
        [Description("The scheduled action for the mission item.")]
        public  /*MAV_CMD*/ushort command;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>The coordinate system of the COMMAND. MAV_FRAME  </summary>
        [Units("")]
        [Description("The coordinate system of the COMMAND.")]
        public  /*MAV_FRAME*/byte frame;
            /// <summary>false:0, true:1   </summary>
        [Units("")]
        [Description("false:0, true:1")]
        public  byte current;
            /// <summary>autocontinue to next wp   </summary>
        [Units("")]
        [Description("autocontinue to next wp")]
        public  byte autocontinue;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=33)]
    ///<summary> Send a command with up to seven parameters to the MAV </summary>
    public struct mavlink_command_long_t
    {
        /// <summary>Parameter 1 (for the specific command).   </summary>
        [Units("")]
        [Description("Parameter 1 (for the specific command).")]
        public  float param1;
            /// <summary>Parameter 2 (for the specific command).   </summary>
        [Units("")]
        [Description("Parameter 2 (for the specific command).")]
        public  float param2;
            /// <summary>Parameter 3 (for the specific command).   </summary>
        [Units("")]
        [Description("Parameter 3 (for the specific command).")]
        public  float param3;
            /// <summary>Parameter 4 (for the specific command).   </summary>
        [Units("")]
        [Description("Parameter 4 (for the specific command).")]
        public  float param4;
            /// <summary>Parameter 5 (for the specific command).   </summary>
        [Units("")]
        [Description("Parameter 5 (for the specific command).")]
        public  float param5;
            /// <summary>Parameter 6 (for the specific command).   </summary>
        [Units("")]
        [Description("Parameter 6 (for the specific command).")]
        public  float param6;
            /// <summary>Parameter 7 (for the specific command).   </summary>
        [Units("")]
        [Description("Parameter 7 (for the specific command).")]
        public  float param7;
            /// <summary>Command ID (of command to send). MAV_CMD  </summary>
        [Units("")]
        [Description("Command ID (of command to send).")]
        public  /*MAV_CMD*/ushort command;
            /// <summary>System which should execute the command   </summary>
        [Units("")]
        [Description("System which should execute the command")]
        public  byte target_system;
            /// <summary>Component which should execute the command, 0 for all components   </summary>
        [Units("")]
        [Description("Component which should execute the command, 0 for all components")]
        public  byte target_component;
            /// <summary>0: First transmission of this command. 1-255: Confirmation transmissions (e.g. for kill command)   </summary>
        [Units("")]
        [Description("0: First transmission of this command. 1-255: Confirmation transmissions (e.g. for kill command)")]
        public  byte confirmation;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=3)]
    ///<summary> Report status of a command. Includes feedback whether the command was executed. </summary>
    public struct mavlink_command_ack_t
    {
        /// <summary>Command ID (of acknowledged command). MAV_CMD  </summary>
        [Units("")]
        [Description("Command ID (of acknowledged command).")]
        public  /*MAV_CMD*/ushort command;
            /// <summary>Result of command. MAV_RESULT  </summary>
        [Units("")]
        [Description("Result of command.")]
        public  /*MAV_RESULT*/byte result;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> Setpoint in roll, pitch, yaw and thrust from the operator </summary>
    public struct mavlink_manual_setpoint_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Desired roll rate  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Desired roll rate")]
        public  float roll;
            /// <summary>Desired pitch rate  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Desired pitch rate")]
        public  float pitch;
            /// <summary>Desired yaw rate  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Desired yaw rate")]
        public  float yaw;
            /// <summary>Collective thrust, normalized to 0 .. 1   </summary>
        [Units("")]
        [Description("Collective thrust, normalized to 0 .. 1")]
        public  float thrust;
            /// <summary>Flight mode switch position, 0.. 255   </summary>
        [Units("")]
        [Description("Flight mode switch position, 0.. 255")]
        public  byte mode_switch;
            /// <summary>Override mode switch position, 0.. 255   </summary>
        [Units("")]
        [Description("Override mode switch position, 0.. 255")]
        public  byte manual_override_switch;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=39)]
    ///<summary> Sets a desired vehicle attitude. Used by an external controller to command the vehicle (manual controller or other system). </summary>
    public struct mavlink_set_attitude_target_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)   </summary>
        [Units("")]
        [Description("Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Body roll rate  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body roll rate")]
        public  float body_roll_rate;
            /// <summary>Body pitch rate  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body pitch rate")]
        public  float body_pitch_rate;
            /// <summary>Body yaw rate  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body yaw rate")]
        public  float body_yaw_rate;
            /// <summary>Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust)   </summary>
        [Units("")]
        [Description("Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust)")]
        public  float thrust;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 6: reserved, bit 7: throttle, bit 8: attitude   </summary>
        [Units("")]
        [Description("Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 6: reserved, bit 7: throttle, bit 8: attitude")]
        public  byte type_mask;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=37)]
    ///<summary> Reports the current commanded attitude of the vehicle as specified by the autopilot. This should match the commands sent in a SET_ATTITUDE_TARGET message if the vehicle is being controlled this way. </summary>
    public struct mavlink_attitude_target_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)   </summary>
        [Units("")]
        [Description("Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Body roll rate  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body roll rate")]
        public  float body_roll_rate;
            /// <summary>Body pitch rate  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body pitch rate")]
        public  float body_pitch_rate;
            /// <summary>Body yaw rate  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body yaw rate")]
        public  float body_yaw_rate;
            /// <summary>Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust)   </summary>
        [Units("")]
        [Description("Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust)")]
        public  float thrust;
            /// <summary>Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 7: reserved, bit 8: attitude   bitmask</summary>
        [Units("")]
        [Description("Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 7: reserved, bit 8: attitude")]
        public  byte type_mask;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=53)]
    ///<summary> Sets a desired vehicle position in a local north-east-down coordinate frame. Used by an external controller to command the vehicle (manual controller or other system). </summary>
    public struct mavlink_set_position_target_local_ned_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>X Position in NED frame  [m] </summary>
        [Units("[m]")]
        [Description("X Position in NED frame")]
        public  float x;
            /// <summary>Y Position in NED frame  [m] </summary>
        [Units("[m]")]
        [Description("Y Position in NED frame")]
        public  float y;
            /// <summary>Z Position in NED frame (note, altitude is negative in NED)  [m] </summary>
        [Units("[m]")]
        [Description("Z Position in NED frame (note, altitude is negative in NED)")]
        public  float z;
            /// <summary>X velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("X velocity in NED frame")]
        public  float vx;
            /// <summary>Y velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Y velocity in NED frame")]
        public  float vy;
            /// <summary>Z velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Z velocity in NED frame")]
        public  float vz;
            /// <summary>X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afx;
            /// <summary>Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afy;
            /// <summary>Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afz;
            /// <summary>yaw setpoint  [rad] </summary>
        [Units("[rad]")]
        [Description("yaw setpoint")]
        public  float yaw;
            /// <summary>yaw rate setpoint  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("yaw rate setpoint")]
        public  float yaw_rate;
            /// <summary>Bitmap to indicate which dimensions should be ignored by the vehicle. POSITION_TARGET_TYPEMASK  bitmask</summary>
        [Units("")]
        [Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
        public  /*POSITION_TARGET_TYPEMASK*/ushort type_mask;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9 MAV_FRAME  </summary>
        [Units("")]
        [Description("Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9")]
        public  /*MAV_FRAME*/byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    ///<summary> Reports the current commanded vehicle position, velocity, and acceleration as specified by the autopilot. This should match the commands sent in SET_POSITION_TARGET_LOCAL_NED if the vehicle is being controlled this way. </summary>
    public struct mavlink_position_target_local_ned_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>X Position in NED frame  [m] </summary>
        [Units("[m]")]
        [Description("X Position in NED frame")]
        public  float x;
            /// <summary>Y Position in NED frame  [m] </summary>
        [Units("[m]")]
        [Description("Y Position in NED frame")]
        public  float y;
            /// <summary>Z Position in NED frame (note, altitude is negative in NED)  [m] </summary>
        [Units("[m]")]
        [Description("Z Position in NED frame (note, altitude is negative in NED)")]
        public  float z;
            /// <summary>X velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("X velocity in NED frame")]
        public  float vx;
            /// <summary>Y velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Y velocity in NED frame")]
        public  float vy;
            /// <summary>Z velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Z velocity in NED frame")]
        public  float vz;
            /// <summary>X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afx;
            /// <summary>Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afy;
            /// <summary>Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afz;
            /// <summary>yaw setpoint  [rad] </summary>
        [Units("[rad]")]
        [Description("yaw setpoint")]
        public  float yaw;
            /// <summary>yaw rate setpoint  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("yaw rate setpoint")]
        public  float yaw_rate;
            /// <summary>Bitmap to indicate which dimensions should be ignored by the vehicle. POSITION_TARGET_TYPEMASK  bitmask</summary>
        [Units("")]
        [Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
        public  /*POSITION_TARGET_TYPEMASK*/ushort type_mask;
            /// <summary>Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9 MAV_FRAME  </summary>
        [Units("")]
        [Description("Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9")]
        public  /*MAV_FRAME*/byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=53)]
    ///<summary> Sets a desired vehicle position, velocity, and/or acceleration in a global coordinate system (WGS84). Used by an external controller to command the vehicle (manual controller or other system). </summary>
    public struct mavlink_set_position_target_global_int_t
    {
        /// <summary>Timestamp (time since system boot). The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency.  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot). The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency.")]
        public  uint time_boot_ms;
            /// <summary>X Position in WGS84 frame  [degE7] </summary>
        [Units("[degE7]")]
        [Description("X Position in WGS84 frame")]
        public  int lat_int;
            /// <summary>Y Position in WGS84 frame  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Y Position in WGS84 frame")]
        public  int lon_int;
            /// <summary>Altitude (MSL, Relative to home, or AGL - depending on frame)  [m] </summary>
        [Units("[m]")]
        [Description("Altitude (MSL, Relative to home, or AGL - depending on frame)")]
        public  float alt;
            /// <summary>X velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("X velocity in NED frame")]
        public  float vx;
            /// <summary>Y velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Y velocity in NED frame")]
        public  float vy;
            /// <summary>Z velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Z velocity in NED frame")]
        public  float vz;
            /// <summary>X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afx;
            /// <summary>Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afy;
            /// <summary>Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afz;
            /// <summary>yaw setpoint  [rad] </summary>
        [Units("[rad]")]
        [Description("yaw setpoint")]
        public  float yaw;
            /// <summary>yaw rate setpoint  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("yaw rate setpoint")]
        public  float yaw_rate;
            /// <summary>Bitmap to indicate which dimensions should be ignored by the vehicle. POSITION_TARGET_TYPEMASK  bitmask</summary>
        [Units("")]
        [Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
        public  /*POSITION_TARGET_TYPEMASK*/ushort type_mask;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11 MAV_FRAME  </summary>
        [Units("")]
        [Description("Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11")]
        public  /*MAV_FRAME*/byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    ///<summary> Reports the current commanded vehicle position, velocity, and acceleration as specified by the autopilot. This should match the commands sent in SET_POSITION_TARGET_GLOBAL_INT if the vehicle is being controlled this way. </summary>
    public struct mavlink_position_target_global_int_t
    {
        /// <summary>Timestamp (time since system boot). The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency.  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot). The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency.")]
        public  uint time_boot_ms;
            /// <summary>X Position in WGS84 frame  [degE7] </summary>
        [Units("[degE7]")]
        [Description("X Position in WGS84 frame")]
        public  int lat_int;
            /// <summary>Y Position in WGS84 frame  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Y Position in WGS84 frame")]
        public  int lon_int;
            /// <summary>Altitude (MSL, AGL or relative to home altitude, depending on frame)  [m] </summary>
        [Units("[m]")]
        [Description("Altitude (MSL, AGL or relative to home altitude, depending on frame)")]
        public  float alt;
            /// <summary>X velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("X velocity in NED frame")]
        public  float vx;
            /// <summary>Y velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Y velocity in NED frame")]
        public  float vy;
            /// <summary>Z velocity in NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Z velocity in NED frame")]
        public  float vz;
            /// <summary>X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afx;
            /// <summary>Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afy;
            /// <summary>Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
        public  float afz;
            /// <summary>yaw setpoint  [rad] </summary>
        [Units("[rad]")]
        [Description("yaw setpoint")]
        public  float yaw;
            /// <summary>yaw rate setpoint  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("yaw rate setpoint")]
        public  float yaw_rate;
            /// <summary>Bitmap to indicate which dimensions should be ignored by the vehicle. POSITION_TARGET_TYPEMASK  bitmask</summary>
        [Units("")]
        [Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
        public  /*POSITION_TARGET_TYPEMASK*/ushort type_mask;
            /// <summary>Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11 MAV_FRAME  </summary>
        [Units("")]
        [Description("Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11")]
        public  /*MAV_FRAME*/byte coordinate_frame;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> The offset in X, Y, Z and yaw between the LOCAL_POSITION_NED messages of MAV X and the global coordinate frame in NED coordinates. Coordinate frame is right-handed, Z-axis down (aeronautical frame, NED / north-east-down convention) </summary>
    public struct mavlink_local_position_ned_system_global_offset_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>X Position  [m] </summary>
        [Units("[m]")]
        [Description("X Position")]
        public  float x;
            /// <summary>Y Position  [m] </summary>
        [Units("[m]")]
        [Description("Y Position")]
        public  float y;
            /// <summary>Z Position  [m] </summary>
        [Units("[m]")]
        [Description("Z Position")]
        public  float z;
            /// <summary>Roll  [rad] </summary>
        [Units("[rad]")]
        [Description("Roll")]
        public  float roll;
            /// <summary>Pitch  [rad] </summary>
        [Units("[rad]")]
        [Description("Pitch")]
        public  float pitch;
            /// <summary>Yaw  [rad] </summary>
        [Units("[rad]")]
        [Description("Yaw")]
        public  float yaw;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=56)]
    ///<summary> Sent from simulation to autopilot. This packet is useful for high throughput applications such as hardware in the loop simulations. </summary>
    public struct mavlink_hil_state_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Roll angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Roll angle")]
        public  float roll;
            /// <summary>Pitch angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Pitch angle")]
        public  float pitch;
            /// <summary>Yaw angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Yaw angle")]
        public  float yaw;
            /// <summary>Body frame roll / phi angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body frame roll / phi angular speed")]
        public  float rollspeed;
            /// <summary>Body frame pitch / theta angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body frame pitch / theta angular speed")]
        public  float pitchspeed;
            /// <summary>Body frame yaw / psi angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body frame yaw / psi angular speed")]
        public  float yawspeed;
            /// <summary>Latitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude")]
        public  int lat;
            /// <summary>Longitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude")]
        public  int lon;
            /// <summary>Altitude  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude")]
        public  int alt;
            /// <summary>Ground X Speed (Latitude)  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Ground X Speed (Latitude)")]
        public  short vx;
            /// <summary>Ground Y Speed (Longitude)  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Ground Y Speed (Longitude)")]
        public  short vy;
            /// <summary>Ground Z Speed (Altitude)  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Ground Z Speed (Altitude)")]
        public  short vz;
            /// <summary>X acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("X acceleration")]
        public  short xacc;
            /// <summary>Y acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Y acceleration")]
        public  short yacc;
            /// <summary>Z acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Z acceleration")]
        public  short zacc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> Sent from autopilot to simulation. Hardware in the loop control outputs </summary>
    public struct mavlink_hil_controls_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Control output -1 .. 1   </summary>
        [Units("")]
        [Description("Control output -1 .. 1")]
        public  float roll_ailerons;
            /// <summary>Control output -1 .. 1   </summary>
        [Units("")]
        [Description("Control output -1 .. 1")]
        public  float pitch_elevator;
            /// <summary>Control output -1 .. 1   </summary>
        [Units("")]
        [Description("Control output -1 .. 1")]
        public  float yaw_rudder;
            /// <summary>Throttle 0 .. 1   </summary>
        [Units("")]
        [Description("Throttle 0 .. 1")]
        public  float throttle;
            /// <summary>Aux 1, -1 .. 1   </summary>
        [Units("")]
        [Description("Aux 1, -1 .. 1")]
        public  float aux1;
            /// <summary>Aux 2, -1 .. 1   </summary>
        [Units("")]
        [Description("Aux 2, -1 .. 1")]
        public  float aux2;
            /// <summary>Aux 3, -1 .. 1   </summary>
        [Units("")]
        [Description("Aux 3, -1 .. 1")]
        public  float aux3;
            /// <summary>Aux 4, -1 .. 1   </summary>
        [Units("")]
        [Description("Aux 4, -1 .. 1")]
        public  float aux4;
            /// <summary>System mode. MAV_MODE  </summary>
        [Units("")]
        [Description("System mode.")]
        public  /*MAV_MODE*/byte mode;
            /// <summary>Navigation mode (MAV_NAV_MODE)   </summary>
        [Units("")]
        [Description("Navigation mode (MAV_NAV_MODE)")]
        public  byte nav_mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=33)]
    ///<summary> Sent from simulation to autopilot. The RAW values of the RC channels received. The standard PPM modulation is as follows: 1000 microseconds: 0%, 2000 microseconds: 100%. Individual receivers/transmitters might violate this specification. </summary>
    public struct mavlink_hil_rc_inputs_raw_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>RC channel 1 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 1 value")]
        public  ushort chan1_raw;
            /// <summary>RC channel 2 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 2 value")]
        public  ushort chan2_raw;
            /// <summary>RC channel 3 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 3 value")]
        public  ushort chan3_raw;
            /// <summary>RC channel 4 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 4 value")]
        public  ushort chan4_raw;
            /// <summary>RC channel 5 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 5 value")]
        public  ushort chan5_raw;
            /// <summary>RC channel 6 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 6 value")]
        public  ushort chan6_raw;
            /// <summary>RC channel 7 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 7 value")]
        public  ushort chan7_raw;
            /// <summary>RC channel 8 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 8 value")]
        public  ushort chan8_raw;
            /// <summary>RC channel 9 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 9 value")]
        public  ushort chan9_raw;
            /// <summary>RC channel 10 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 10 value")]
        public  ushort chan10_raw;
            /// <summary>RC channel 11 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 11 value")]
        public  ushort chan11_raw;
            /// <summary>RC channel 12 value  [us] </summary>
        [Units("[us]")]
        [Description("RC channel 12 value")]
        public  ushort chan12_raw;
            /// <summary>Receive signal strength indicator in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.   </summary>
        [Units("")]
        [Description("Receive signal strength indicator in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.")]
        public  byte rssi;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=81)]
    ///<summary> Sent from autopilot to simulation. Hardware in the loop control outputs (replacement for HIL_CONTROLS) </summary>
    public struct mavlink_hil_actuator_controls_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Flags as bitfield, reserved for future use.   bitmask</summary>
        [Units("")]
        [Description("Flags as bitfield, reserved for future use.")]
        public  ulong flags;
            /// <summary>Control outputs -1 .. 1. Channel assignment depends on the simulated hardware.   </summary>
        [Units("")]
        [Description("Control outputs -1 .. 1. Channel assignment depends on the simulated hardware.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public float[] controls;
            /// <summary>System mode. Includes arming state. MAV_MODE_FLAG  bitmask</summary>
        [Units("")]
        [Description("System mode. Includes arming state.")]
        public  /*MAV_MODE_FLAG*/byte mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=34)]
    ///<summary> Optical flow from a flow sensor (e.g. optical mouse sensor) </summary>
    public struct mavlink_optical_flow_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Flow in x-sensor direction, angular-speed compensated  [m] </summary>
        [Units("[m]")]
        [Description("Flow in x-sensor direction, angular-speed compensated")]
        public  float flow_comp_m_x;
            /// <summary>Flow in y-sensor direction, angular-speed compensated  [m] </summary>
        [Units("[m]")]
        [Description("Flow in y-sensor direction, angular-speed compensated")]
        public  float flow_comp_m_y;
            /// <summary>Ground distance. Positive value: distance known. Negative value: Unknown distance  [m] </summary>
        [Units("[m]")]
        [Description("Ground distance. Positive value: distance known. Negative value: Unknown distance")]
        public  float ground_distance;
            /// <summary>Flow in x-sensor direction  [dpix] </summary>
        [Units("[dpix]")]
        [Description("Flow in x-sensor direction")]
        public  short flow_x;
            /// <summary>Flow in y-sensor direction  [dpix] </summary>
        [Units("[dpix]")]
        [Description("Flow in y-sensor direction")]
        public  short flow_y;
            /// <summary>Sensor ID   </summary>
        [Units("")]
        [Description("Sensor ID")]
        public  byte sensor_id;
            /// <summary>Optical flow quality / confidence. 0: bad, 255: maximum quality   </summary>
        [Units("")]
        [Description("Optical flow quality / confidence. 0: bad, 255: maximum quality")]
        public  byte quality;
            /// <summary>Flow rate about X axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Flow rate about X axis")]
        public  float flow_rate_x;
            /// <summary>Flow rate about Y axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Flow rate about Y axis")]
        public  float flow_rate_y;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=116)]
    ///<summary> Global position/attitude estimate from a vision source. </summary>
    public struct mavlink_global_vision_position_estimate_t
    {
        /// <summary>Timestamp (UNIX time or since system boot)  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX time or since system boot)")]
        public  ulong usec;
            /// <summary>Global X position  [m] </summary>
        [Units("[m]")]
        [Description("Global X position")]
        public  float x;
            /// <summary>Global Y position  [m] </summary>
        [Units("[m]")]
        [Description("Global Y position")]
        public  float y;
            /// <summary>Global Z position  [m] </summary>
        [Units("[m]")]
        [Description("Global Z position")]
        public  float z;
            /// <summary>Roll angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Roll angle")]
        public  float roll;
            /// <summary>Pitch angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Pitch angle")]
        public  float pitch;
            /// <summary>Yaw angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Yaw angle")]
        public  float yaw;
            /// <summary>Row-major representation of pose 6x6 cross-covariance matrix upper right triangle (states: x_global, y_global, z_global, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of pose 6x6 cross-covariance matrix upper right triangle (states: x_global, y_global, z_global, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=116)]
    ///<summary> Global position/attitude estimate from a vision source. </summary>
    public struct mavlink_vision_position_estimate_t
    {
        /// <summary>Timestamp (UNIX time or time since system boot)  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX time or time since system boot)")]
        public  ulong usec;
            /// <summary>Global X position  [m] </summary>
        [Units("[m]")]
        [Description("Global X position")]
        public  float x;
            /// <summary>Global Y position  [m] </summary>
        [Units("[m]")]
        [Description("Global Y position")]
        public  float y;
            /// <summary>Global Z position  [m] </summary>
        [Units("[m]")]
        [Description("Global Z position")]
        public  float z;
            /// <summary>Roll angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Roll angle")]
        public  float roll;
            /// <summary>Pitch angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Pitch angle")]
        public  float pitch;
            /// <summary>Yaw angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Yaw angle")]
        public  float yaw;
            /// <summary>Row-major representation of pose 6x6 cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of pose 6x6 cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=56)]
    ///<summary> Speed estimate from a vision source. </summary>
    public struct mavlink_vision_speed_estimate_t
    {
        /// <summary>Timestamp (UNIX time or time since system boot)  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX time or time since system boot)")]
        public  ulong usec;
            /// <summary>Global X speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Global X speed")]
        public  float x;
            /// <summary>Global Y speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Global Y speed")]
        public  float y;
            /// <summary>Global Z speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Global Z speed")]
        public  float z;
            /// <summary>Row-major representation of 3x3 linear velocity covariance matrix (states: vx, vy, vz; 1st three entries - 1st row, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of 3x3 linear velocity covariance matrix (states: vx, vy, vz; 1st three entries - 1st row, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=116)]
    ///<summary> Global position estimate from a Vicon motion system source. </summary>
    public struct mavlink_vicon_position_estimate_t
    {
        /// <summary>Timestamp (UNIX time or time since system boot)  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX time or time since system boot)")]
        public  ulong usec;
            /// <summary>Global X position  [m] </summary>
        [Units("[m]")]
        [Description("Global X position")]
        public  float x;
            /// <summary>Global Y position  [m] </summary>
        [Units("[m]")]
        [Description("Global Y position")]
        public  float y;
            /// <summary>Global Z position  [m] </summary>
        [Units("[m]")]
        [Description("Global Z position")]
        public  float z;
            /// <summary>Roll angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Roll angle")]
        public  float roll;
            /// <summary>Pitch angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Pitch angle")]
        public  float pitch;
            /// <summary>Yaw angle  [rad] </summary>
        [Units("[rad]")]
        [Description("Yaw angle")]
        public  float yaw;
            /// <summary>Row-major representation of 6x6 pose cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of 6x6 pose cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=62)]
    ///<summary> The IMU readings in SI units in NED body frame </summary>
    public struct mavlink_highres_imu_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>X acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X acceleration")]
        public  float xacc;
            /// <summary>Y acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y acceleration")]
        public  float yacc;
            /// <summary>Z acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z acceleration")]
        public  float zacc;
            /// <summary>Angular speed around X axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around X axis")]
        public  float xgyro;
            /// <summary>Angular speed around Y axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around Y axis")]
        public  float ygyro;
            /// <summary>Angular speed around Z axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around Z axis")]
        public  float zgyro;
            /// <summary>X Magnetic field  [gauss] </summary>
        [Units("[gauss]")]
        [Description("X Magnetic field")]
        public  float xmag;
            /// <summary>Y Magnetic field  [gauss] </summary>
        [Units("[gauss]")]
        [Description("Y Magnetic field")]
        public  float ymag;
            /// <summary>Z Magnetic field  [gauss] </summary>
        [Units("[gauss]")]
        [Description("Z Magnetic field")]
        public  float zmag;
            /// <summary>Absolute pressure  [mbar] </summary>
        [Units("[mbar]")]
        [Description("Absolute pressure")]
        public  float abs_pressure;
            /// <summary>Differential pressure  [mbar] </summary>
        [Units("[mbar]")]
        [Description("Differential pressure")]
        public  float diff_pressure;
            /// <summary>Altitude calculated from pressure   </summary>
        [Units("")]
        [Description("Altitude calculated from pressure")]
        public  float pressure_alt;
            /// <summary>Temperature  [degC] </summary>
        [Units("[degC]")]
        [Description("Temperature")]
        public  float temperature;
            /// <summary>Bitmap for fields that have updated since last message, bit 0 = xacc, bit 12: temperature   bitmask</summary>
        [Units("")]
        [Description("Bitmap for fields that have updated since last message, bit 0 = xacc, bit 12: temperature")]
        public  ushort fields_updated;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> Optical flow from an angular rate flow sensor (e.g. PX4FLOW or mouse sensor) </summary>
    public struct mavlink_optical_flow_rad_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Integration time. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the.  [us] </summary>
        [Units("[us]")]
        [Description("Integration time. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the.")]
        public  uint integration_time_us;
            /// <summary>Flow around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.)  [rad] </summary>
        [Units("[rad]")]
        [Description("Flow around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.)")]
        public  float integrated_x;
            /// <summary>Flow around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.)  [rad] </summary>
        [Units("[rad]")]
        [Description("Flow around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.)")]
        public  float integrated_y;
            /// <summary>RH rotation around X axis  [rad] </summary>
        [Units("[rad]")]
        [Description("RH rotation around X axis")]
        public  float integrated_xgyro;
            /// <summary>RH rotation around Y axis  [rad] </summary>
        [Units("[rad]")]
        [Description("RH rotation around Y axis")]
        public  float integrated_ygyro;
            /// <summary>RH rotation around Z axis  [rad] </summary>
        [Units("[rad]")]
        [Description("RH rotation around Z axis")]
        public  float integrated_zgyro;
            /// <summary>Time since the distance was sampled.  [us] </summary>
        [Units("[us]")]
        [Description("Time since the distance was sampled.")]
        public  uint time_delta_distance_us;
            /// <summary>Distance to the center of the flow field. Positive value (including zero): distance known. Negative value: Unknown distance.  [m] </summary>
        [Units("[m]")]
        [Description("Distance to the center of the flow field. Positive value (including zero): distance known. Negative value: Unknown distance.")]
        public  float distance;
            /// <summary>Temperature  [cdegC] </summary>
        [Units("[cdegC]")]
        [Description("Temperature")]
        public  short temperature;
            /// <summary>Sensor ID   </summary>
        [Units("")]
        [Description("Sensor ID")]
        public  byte sensor_id;
            /// <summary>Optical flow quality / confidence. 0: no valid flow, 255: maximum quality   </summary>
        [Units("")]
        [Description("Optical flow quality / confidence. 0: no valid flow, 255: maximum quality")]
        public  byte quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=64)]
    ///<summary> The IMU readings in SI units in NED body frame </summary>
    public struct mavlink_hil_sensor_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>X acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X acceleration")]
        public  float xacc;
            /// <summary>Y acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y acceleration")]
        public  float yacc;
            /// <summary>Z acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z acceleration")]
        public  float zacc;
            /// <summary>Angular speed around X axis in body frame  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around X axis in body frame")]
        public  float xgyro;
            /// <summary>Angular speed around Y axis in body frame  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around Y axis in body frame")]
        public  float ygyro;
            /// <summary>Angular speed around Z axis in body frame  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around Z axis in body frame")]
        public  float zgyro;
            /// <summary>X Magnetic field  [gauss] </summary>
        [Units("[gauss]")]
        [Description("X Magnetic field")]
        public  float xmag;
            /// <summary>Y Magnetic field  [gauss] </summary>
        [Units("[gauss]")]
        [Description("Y Magnetic field")]
        public  float ymag;
            /// <summary>Z Magnetic field  [gauss] </summary>
        [Units("[gauss]")]
        [Description("Z Magnetic field")]
        public  float zmag;
            /// <summary>Absolute pressure  [mbar] </summary>
        [Units("[mbar]")]
        [Description("Absolute pressure")]
        public  float abs_pressure;
            /// <summary>Differential pressure (airspeed)  [mbar] </summary>
        [Units("[mbar]")]
        [Description("Differential pressure (airspeed)")]
        public  float diff_pressure;
            /// <summary>Altitude calculated from pressure   </summary>
        [Units("")]
        [Description("Altitude calculated from pressure")]
        public  float pressure_alt;
            /// <summary>Temperature  [degC] </summary>
        [Units("[degC]")]
        [Description("Temperature")]
        public  float temperature;
            /// <summary>Bitmap for fields that have updated since last message, bit 0 = xacc, bit 12: temperature, bit 31: full reset of attitude/position/velocities/etc was performed in sim.   bitmask</summary>
        [Units("")]
        [Description("Bitmap for fields that have updated since last message, bit 0 = xacc, bit 12: temperature, bit 31: full reset of attitude/position/velocities/etc was performed in sim.")]
        public  uint fields_updated;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=84)]
    ///<summary> Status of simulation environment, if used </summary>
    public struct mavlink_sim_state_t
    {
        /// <summary>True attitude quaternion component 1, w (1 in null-rotation)   </summary>
        [Units("")]
        [Description("True attitude quaternion component 1, w (1 in null-rotation)")]
        public  float q1;
            /// <summary>True attitude quaternion component 2, x (0 in null-rotation)   </summary>
        [Units("")]
        [Description("True attitude quaternion component 2, x (0 in null-rotation)")]
        public  float q2;
            /// <summary>True attitude quaternion component 3, y (0 in null-rotation)   </summary>
        [Units("")]
        [Description("True attitude quaternion component 3, y (0 in null-rotation)")]
        public  float q3;
            /// <summary>True attitude quaternion component 4, z (0 in null-rotation)   </summary>
        [Units("")]
        [Description("True attitude quaternion component 4, z (0 in null-rotation)")]
        public  float q4;
            /// <summary>Attitude roll expressed as Euler angles, not recommended except for human-readable outputs   </summary>
        [Units("")]
        [Description("Attitude roll expressed as Euler angles, not recommended except for human-readable outputs")]
        public  float roll;
            /// <summary>Attitude pitch expressed as Euler angles, not recommended except for human-readable outputs   </summary>
        [Units("")]
        [Description("Attitude pitch expressed as Euler angles, not recommended except for human-readable outputs")]
        public  float pitch;
            /// <summary>Attitude yaw expressed as Euler angles, not recommended except for human-readable outputs   </summary>
        [Units("")]
        [Description("Attitude yaw expressed as Euler angles, not recommended except for human-readable outputs")]
        public  float yaw;
            /// <summary>X acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X acceleration")]
        public  float xacc;
            /// <summary>Y acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y acceleration")]
        public  float yacc;
            /// <summary>Z acceleration  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z acceleration")]
        public  float zacc;
            /// <summary>Angular speed around X axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around X axis")]
        public  float xgyro;
            /// <summary>Angular speed around Y axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around Y axis")]
        public  float ygyro;
            /// <summary>Angular speed around Z axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular speed around Z axis")]
        public  float zgyro;
            /// <summary>Latitude  [deg] </summary>
        [Units("[deg]")]
        [Description("Latitude")]
        public  float lat;
            /// <summary>Longitude  [deg] </summary>
        [Units("[deg]")]
        [Description("Longitude")]
        public  float lon;
            /// <summary>Altitude  [m] </summary>
        [Units("[m]")]
        [Description("Altitude")]
        public  float alt;
            /// <summary>Horizontal position standard deviation   </summary>
        [Units("")]
        [Description("Horizontal position standard deviation")]
        public  float std_dev_horz;
            /// <summary>Vertical position standard deviation   </summary>
        [Units("")]
        [Description("Vertical position standard deviation")]
        public  float std_dev_vert;
            /// <summary>True velocity in NORTH direction in earth-fixed NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("True velocity in NORTH direction in earth-fixed NED frame")]
        public  float vn;
            /// <summary>True velocity in EAST direction in earth-fixed NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("True velocity in EAST direction in earth-fixed NED frame")]
        public  float ve;
            /// <summary>True velocity in DOWN direction in earth-fixed NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("True velocity in DOWN direction in earth-fixed NED frame")]
        public  float vd;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> Status generated by radio and injected into MAVLink stream. </summary>
    public struct mavlink_radio_status_t
    {
        /// <summary>Count of radio packet receive errors (since boot).   </summary>
        [Units("")]
        [Description("Count of radio packet receive errors (since boot).")]
        public  ushort rxerrors;
            /// <summary>Count of error corrected radio packets (since boot).   </summary>
        [Units("")]
        [Description("Count of error corrected radio packets (since boot).")]
        public  ushort @fixed;
            /// <summary>Local (message sender) recieved signal strength indication in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.   </summary>
        [Units("")]
        [Description("Local (message sender) recieved signal strength indication in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.")]
        public  byte rssi;
            /// <summary>Remote (message receiver) signal strength indication in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.   </summary>
        [Units("")]
        [Description("Remote (message receiver) signal strength indication in device-dependent units/scale. Values: [0-254], 255: invalid/unknown.")]
        public  byte remrssi;
            /// <summary>Remaining free transmitter buffer space.  [%] </summary>
        [Units("[%]")]
        [Description("Remaining free transmitter buffer space.")]
        public  byte txbuf;
            /// <summary>Local background noise level. These are device dependent RSSI values (scale as approx 2x dB on SiK radios). Values: [0-254], 255: invalid/unknown.   </summary>
        [Units("")]
        [Description("Local background noise level. These are device dependent RSSI values (scale as approx 2x dB on SiK radios). Values: [0-254], 255: invalid/unknown.")]
        public  byte noise;
            /// <summary>Remote background noise level. These are device dependent RSSI values (scale as approx 2x dB on SiK radios). Values: [0-254], 255: invalid/unknown.   </summary>
        [Units("")]
        [Description("Remote background noise level. These are device dependent RSSI values (scale as approx 2x dB on SiK radios). Values: [0-254], 255: invalid/unknown.")]
        public  byte remnoise;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=254)]
    ///<summary> File transfer message </summary>
    public struct mavlink_file_transfer_protocol_t
    {
        /// <summary>Network ID (0 for broadcast)   </summary>
        [Units("")]
        [Description("Network ID (0 for broadcast)")]
        public  byte target_network;
            /// <summary>System ID (0 for broadcast)   </summary>
        [Units("")]
        [Description("System ID (0 for broadcast)")]
        public  byte target_system;
            /// <summary>Component ID (0 for broadcast)   </summary>
        [Units("")]
        [Description("Component ID (0 for broadcast)")]
        public  byte target_component;
            /// <summary>Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification.   </summary>
        [Units("")]
        [Description("Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=251)]
		public byte[] payload;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=16)]
    ///<summary> Time synchronization message. </summary>
    public struct mavlink_timesync_t
    {
        /// <summary>Time sync timestamp 1   </summary>
        [Units("")]
        [Description("Time sync timestamp 1")]
        public  long tc1;
            /// <summary>Time sync timestamp 2   </summary>
        [Units("")]
        [Description("Time sync timestamp 2")]
        public  long ts1;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> Camera-IMU triggering and synchronisation message. </summary>
    public struct mavlink_camera_trigger_t
    {
        /// <summary>Timestamp for image frame (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp for image frame (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Image frame sequence   </summary>
        [Units("")]
        [Description("Image frame sequence")]
        public  uint seq;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=36)]
    ///<summary> The global position, as returned by the Global Positioning System (GPS). This is    
///                 NOT the global position estimate of the sytem, but rather a RAW sensor value. See message GLOBAL_POSITION for the global position estimate. </summary>
    public struct mavlink_hil_gps_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Latitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude (WGS84)")]
        public  int lat;
            /// <summary>Longitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude (WGS84)")]
        public  int lon;
            /// <summary>Altitude (MSL). Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (MSL). Positive for up.")]
        public  int alt;
            /// <summary>GPS HDOP horizontal dilution of position. If unknown, set to: 65535  [cm] </summary>
        [Units("[cm]")]
        [Description("GPS HDOP horizontal dilution of position. If unknown, set to: 65535")]
        public  ushort eph;
            /// <summary>GPS VDOP vertical dilution of position. If unknown, set to: 65535  [cm] </summary>
        [Units("[cm]")]
        [Description("GPS VDOP vertical dilution of position. If unknown, set to: 65535")]
        public  ushort epv;
            /// <summary>GPS ground speed. If unknown, set to: 65535  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("GPS ground speed. If unknown, set to: 65535")]
        public  ushort vel;
            /// <summary>GPS velocity in NORTH direction in earth-fixed NED frame  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("GPS velocity in NORTH direction in earth-fixed NED frame")]
        public  short vn;
            /// <summary>GPS velocity in EAST direction in earth-fixed NED frame  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("GPS velocity in EAST direction in earth-fixed NED frame")]
        public  short ve;
            /// <summary>GPS velocity in DOWN direction in earth-fixed NED frame  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("GPS velocity in DOWN direction in earth-fixed NED frame")]
        public  short vd;
            /// <summary>Course over ground (NOT heading, but direction of movement), 0.0..359.99 degrees. If unknown, set to: 65535  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("Course over ground (NOT heading, but direction of movement), 0.0..359.99 degrees. If unknown, set to: 65535")]
        public  ushort cog;
            /// <summary>0-1: no fix, 2: 2D fix, 3: 3D fix. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix.   </summary>
        [Units("")]
        [Description("0-1: no fix, 2: 2D fix, 3: 3D fix. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix.")]
        public  byte fix_type;
            /// <summary>Number of satellites visible. If unknown, set to 255   </summary>
        [Units("")]
        [Description("Number of satellites visible. If unknown, set to 255")]
        public  byte satellites_visible;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=44)]
    ///<summary> Simulated optical flow from a flow sensor (e.g. PX4FLOW or optical mouse sensor) </summary>
    public struct mavlink_hil_optical_flow_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Integration time. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the.  [us] </summary>
        [Units("[us]")]
        [Description("Integration time. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the.")]
        public  uint integration_time_us;
            /// <summary>Flow in radians around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.)  [rad] </summary>
        [Units("[rad]")]
        [Description("Flow in radians around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.)")]
        public  float integrated_x;
            /// <summary>Flow in radians around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.)  [rad] </summary>
        [Units("[rad]")]
        [Description("Flow in radians around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.)")]
        public  float integrated_y;
            /// <summary>RH rotation around X axis  [rad] </summary>
        [Units("[rad]")]
        [Description("RH rotation around X axis")]
        public  float integrated_xgyro;
            /// <summary>RH rotation around Y axis  [rad] </summary>
        [Units("[rad]")]
        [Description("RH rotation around Y axis")]
        public  float integrated_ygyro;
            /// <summary>RH rotation around Z axis  [rad] </summary>
        [Units("[rad]")]
        [Description("RH rotation around Z axis")]
        public  float integrated_zgyro;
            /// <summary>Time since the distance was sampled.  [us] </summary>
        [Units("[us]")]
        [Description("Time since the distance was sampled.")]
        public  uint time_delta_distance_us;
            /// <summary>Distance to the center of the flow field. Positive value (including zero): distance known. Negative value: Unknown distance.  [m] </summary>
        [Units("[m]")]
        [Description("Distance to the center of the flow field. Positive value (including zero): distance known. Negative value: Unknown distance.")]
        public  float distance;
            /// <summary>Temperature  [cdegC] </summary>
        [Units("[cdegC]")]
        [Description("Temperature")]
        public  short temperature;
            /// <summary>Sensor ID   </summary>
        [Units("")]
        [Description("Sensor ID")]
        public  byte sensor_id;
            /// <summary>Optical flow quality / confidence. 0: no valid flow, 255: maximum quality   </summary>
        [Units("")]
        [Description("Optical flow quality / confidence. 0: no valid flow, 255: maximum quality")]
        public  byte quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=64)]
    ///<summary> Sent from simulation to autopilot, avoids in contrast to HIL_STATE singularities. This packet is useful for high throughput applications such as hardware in the loop simulations. </summary>
    public struct mavlink_hil_state_quaternion_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Vehicle attitude expressed as normalized quaternion in w, x, y, z order (with 1 0 0 0 being the null-rotation)   </summary>
        [Units("")]
        [Description("Vehicle attitude expressed as normalized quaternion in w, x, y, z order (with 1 0 0 0 being the null-rotation)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] attitude_quaternion;
            /// <summary>Body frame roll / phi angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body frame roll / phi angular speed")]
        public  float rollspeed;
            /// <summary>Body frame pitch / theta angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body frame pitch / theta angular speed")]
        public  float pitchspeed;
            /// <summary>Body frame yaw / psi angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Body frame yaw / psi angular speed")]
        public  float yawspeed;
            /// <summary>Latitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude")]
        public  int lat;
            /// <summary>Longitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude")]
        public  int lon;
            /// <summary>Altitude  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude")]
        public  int alt;
            /// <summary>Ground X Speed (Latitude)  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Ground X Speed (Latitude)")]
        public  short vx;
            /// <summary>Ground Y Speed (Longitude)  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Ground Y Speed (Longitude)")]
        public  short vy;
            /// <summary>Ground Z Speed (Altitude)  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Ground Z Speed (Altitude)")]
        public  short vz;
            /// <summary>Indicated airspeed  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Indicated airspeed")]
        public  ushort ind_airspeed;
            /// <summary>True airspeed  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("True airspeed")]
        public  ushort true_airspeed;
            /// <summary>X acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("X acceleration")]
        public  short xacc;
            /// <summary>Y acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Y acceleration")]
        public  short yacc;
            /// <summary>Z acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Z acceleration")]
        public  short zacc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The RAW IMU readings for secondary 9DOF sensor setup. This message should contain the scaled values to the described units </summary>
    public struct mavlink_scaled_imu2_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>X acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("X acceleration")]
        public  short xacc;
            /// <summary>Y acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Y acceleration")]
        public  short yacc;
            /// <summary>Z acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Z acceleration")]
        public  short zacc;
            /// <summary>Angular speed around X axis  [mrad/s] </summary>
        [Units("[mrad/s]")]
        [Description("Angular speed around X axis")]
        public  short xgyro;
            /// <summary>Angular speed around Y axis  [mrad/s] </summary>
        [Units("[mrad/s]")]
        [Description("Angular speed around Y axis")]
        public  short ygyro;
            /// <summary>Angular speed around Z axis  [mrad/s] </summary>
        [Units("[mrad/s]")]
        [Description("Angular speed around Z axis")]
        public  short zgyro;
            /// <summary>X Magnetic field  [mT] </summary>
        [Units("[mT]")]
        [Description("X Magnetic field")]
        public  short xmag;
            /// <summary>Y Magnetic field  [mT] </summary>
        [Units("[mT]")]
        [Description("Y Magnetic field")]
        public  short ymag;
            /// <summary>Z Magnetic field  [mT] </summary>
        [Units("[mT]")]
        [Description("Z Magnetic field")]
        public  short zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Request a list of available logs. On some systems calling this may stop on-board logging until LOG_REQUEST_END is called. </summary>
    public struct mavlink_log_request_list_t
    {
        /// <summary>First log id (0 for first available)   </summary>
        [Units("")]
        [Description("First log id (0 for first available)")]
        public  ushort start;
            /// <summary>Last log id (0xffff for last available)   </summary>
        [Units("")]
        [Description("Last log id (0xffff for last available)")]
        public  ushort end;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Reply to LOG_REQUEST_LIST </summary>
    public struct mavlink_log_entry_t
    {
        /// <summary>UTC timestamp of log since 1970, or 0 if not available  [s] </summary>
        [Units("[s]")]
        [Description("UTC timestamp of log since 1970, or 0 if not available")]
        public  uint time_utc;
            /// <summary>Size of the log (may be approximate)  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Size of the log (may be approximate)")]
        public  uint size;
            /// <summary>Log id   </summary>
        [Units("")]
        [Description("Log id")]
        public  ushort id;
            /// <summary>Total number of logs   </summary>
        [Units("")]
        [Description("Total number of logs")]
        public  ushort num_logs;
            /// <summary>High log number   </summary>
        [Units("")]
        [Description("High log number")]
        public  ushort last_log_num;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=12)]
    ///<summary> Request a chunk of a log </summary>
    public struct mavlink_log_request_data_t
    {
        /// <summary>Offset into the log   </summary>
        [Units("")]
        [Description("Offset into the log")]
        public  uint ofs;
            /// <summary>Number of bytes  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Number of bytes")]
        public  uint count;
            /// <summary>Log id (from LOG_ENTRY reply)   </summary>
        [Units("")]
        [Description("Log id (from LOG_ENTRY reply)")]
        public  ushort id;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=97)]
    ///<summary> Reply to LOG_REQUEST_DATA </summary>
    public struct mavlink_log_data_t
    {
        /// <summary>Offset into the log   </summary>
        [Units("")]
        [Description("Offset into the log")]
        public  uint ofs;
            /// <summary>Log id (from LOG_ENTRY reply)   </summary>
        [Units("")]
        [Description("Log id (from LOG_ENTRY reply)")]
        public  ushort id;
            /// <summary>Number of bytes (zero for end of log)  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Number of bytes (zero for end of log)")]
        public  byte count;
            /// <summary>log data   </summary>
        [Units("")]
        [Description("log data")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=90)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Erase all logs </summary>
    public struct mavlink_log_erase_t
    {
        /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Stop log transfer and resume normal logging </summary>
    public struct mavlink_log_request_end_t
    {
        /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=113)]
    ///<summary> Data for injecting into the onboard GPS (used for DGPS) </summary>
    public struct mavlink_gps_inject_data_t
    {
        /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>Data length  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Data length")]
        public  byte len;
            /// <summary>Raw data (110 is enough for 12 satellites of RTCMv2)   </summary>
        [Units("")]
        [Description("Raw data (110 is enough for 12 satellites of RTCMv2)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=110)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    ///<summary> Second GPS data. </summary>
    public struct mavlink_gps2_raw_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Latitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude (WGS84)")]
        public  int lat;
            /// <summary>Longitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude (WGS84)")]
        public  int lon;
            /// <summary>Altitude (MSL). Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (MSL). Positive for up.")]
        public  int alt;
            /// <summary>Age of DGPS info  [ms] </summary>
        [Units("[ms]")]
        [Description("Age of DGPS info")]
        public  uint dgps_age;
            /// <summary>GPS HDOP horizontal dilution of position. If unknown, set to: UINT16_MAX  [cm] </summary>
        [Units("[cm]")]
        [Description("GPS HDOP horizontal dilution of position. If unknown, set to: UINT16_MAX")]
        public  ushort eph;
            /// <summary>GPS VDOP vertical dilution of position. If unknown, set to: UINT16_MAX  [cm] </summary>
        [Units("[cm]")]
        [Description("GPS VDOP vertical dilution of position. If unknown, set to: UINT16_MAX")]
        public  ushort epv;
            /// <summary>GPS ground speed. If unknown, set to: UINT16_MAX  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("GPS ground speed. If unknown, set to: UINT16_MAX")]
        public  ushort vel;
            /// <summary>Course over ground (NOT heading, but direction of movement): 0.0..359.99 degrees. If unknown, set to: UINT16_MAX  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("Course over ground (NOT heading, but direction of movement): 0.0..359.99 degrees. If unknown, set to: UINT16_MAX")]
        public  ushort cog;
            /// <summary>GPS fix type. GPS_FIX_TYPE  </summary>
        [Units("")]
        [Description("GPS fix type.")]
        public  /*GPS_FIX_TYPE*/byte fix_type;
            /// <summary>Number of satellites visible. If unknown, set to 255   </summary>
        [Units("")]
        [Description("Number of satellites visible. If unknown, set to 255")]
        public  byte satellites_visible;
            /// <summary>Number of DGPS satellites   </summary>
        [Units("")]
        [Description("Number of DGPS satellites")]
        public  byte dgps_numch;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> Power supply status </summary>
    public struct mavlink_power_status_t
    {
        /// <summary>5V rail voltage.  [mV] </summary>
        [Units("[mV]")]
        [Description("5V rail voltage.")]
        public  ushort Vcc;
            /// <summary>Servo rail voltage.  [mV] </summary>
        [Units("[mV]")]
        [Description("Servo rail voltage.")]
        public  ushort Vservo;
            /// <summary>Bitmap of power supply status flags. MAV_POWER_STATUS  bitmask</summary>
        [Units("")]
        [Description("Bitmap of power supply status flags.")]
        public  /*MAV_POWER_STATUS*/ushort flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=79)]
    ///<summary> Control a serial port. This can be used for raw access to an onboard serial peripheral such as a GPS or telemetry radio. It is designed to make it possible to update the devices firmware via MAVLink messages or change the devices settings. A message with zero bytes can be used to change just the baudrate. </summary>
    public struct mavlink_serial_control_t
    {
        /// <summary>Baudrate of transfer. Zero means no change.  [bits/s] </summary>
        [Units("[bits/s]")]
        [Description("Baudrate of transfer. Zero means no change.")]
        public  uint baudrate;
            /// <summary>Timeout for reply data  [ms] </summary>
        [Units("[ms]")]
        [Description("Timeout for reply data")]
        public  ushort timeout;
            /// <summary>Serial control device type. SERIAL_CONTROL_DEV  </summary>
        [Units("")]
        [Description("Serial control device type.")]
        public  /*SERIAL_CONTROL_DEV*/byte device;
            /// <summary>Bitmap of serial control flags. SERIAL_CONTROL_FLAG  bitmask</summary>
        [Units("")]
        [Description("Bitmap of serial control flags.")]
        public  /*SERIAL_CONTROL_FLAG*/byte flags;
            /// <summary>how many bytes in this transfer  [bytes] </summary>
        [Units("[bytes]")]
        [Description("how many bytes in this transfer")]
        public  byte count;
            /// <summary>serial data   </summary>
        [Units("")]
        [Description("serial data")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=70)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    ///<summary> RTK GPS data. Gives information on the relative baseline calculation the GPS is reporting </summary>
    public struct mavlink_gps_rtk_t
    {
        /// <summary>Time since boot of last baseline message received.  [ms] </summary>
        [Units("[ms]")]
        [Description("Time since boot of last baseline message received.")]
        public  uint time_last_baseline_ms;
            /// <summary>GPS Time of Week of last baseline  [ms] </summary>
        [Units("[ms]")]
        [Description("GPS Time of Week of last baseline")]
        public  uint tow;
            /// <summary>Current baseline in ECEF x or NED north component.  [mm] </summary>
        [Units("[mm]")]
        [Description("Current baseline in ECEF x or NED north component.")]
        public  int baseline_a_mm;
            /// <summary>Current baseline in ECEF y or NED east component.  [mm] </summary>
        [Units("[mm]")]
        [Description("Current baseline in ECEF y or NED east component.")]
        public  int baseline_b_mm;
            /// <summary>Current baseline in ECEF z or NED down component.  [mm] </summary>
        [Units("[mm]")]
        [Description("Current baseline in ECEF z or NED down component.")]
        public  int baseline_c_mm;
            /// <summary>Current estimate of baseline accuracy.   </summary>
        [Units("")]
        [Description("Current estimate of baseline accuracy.")]
        public  uint accuracy;
            /// <summary>Current number of integer ambiguity hypotheses.   </summary>
        [Units("")]
        [Description("Current number of integer ambiguity hypotheses.")]
        public  int iar_num_hypotheses;
            /// <summary>GPS Week Number of last baseline   </summary>
        [Units("")]
        [Description("GPS Week Number of last baseline")]
        public  ushort wn;
            /// <summary>Identification of connected RTK receiver.   </summary>
        [Units("")]
        [Description("Identification of connected RTK receiver.")]
        public  byte rtk_receiver_id;
            /// <summary>GPS-specific health report for RTK data.   </summary>
        [Units("")]
        [Description("GPS-specific health report for RTK data.")]
        public  byte rtk_health;
            /// <summary>Rate of baseline messages being received by GPS  [Hz] </summary>
        [Units("[Hz]")]
        [Description("Rate of baseline messages being received by GPS")]
        public  byte rtk_rate;
            /// <summary>Current number of sats used for RTK calculation.   </summary>
        [Units("")]
        [Description("Current number of sats used for RTK calculation.")]
        public  byte nsats;
            /// <summary>Coordinate system of baseline RTK_BASELINE_COORDINATE_SYSTEM  </summary>
        [Units("")]
        [Description("Coordinate system of baseline")]
        public  /*RTK_BASELINE_COORDINATE_SYSTEM*/byte baseline_coords_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=35)]
    ///<summary> RTK GPS data. Gives information on the relative baseline calculation the GPS is reporting </summary>
    public struct mavlink_gps2_rtk_t
    {
        /// <summary>Time since boot of last baseline message received.  [ms] </summary>
        [Units("[ms]")]
        [Description("Time since boot of last baseline message received.")]
        public  uint time_last_baseline_ms;
            /// <summary>GPS Time of Week of last baseline  [ms] </summary>
        [Units("[ms]")]
        [Description("GPS Time of Week of last baseline")]
        public  uint tow;
            /// <summary>Current baseline in ECEF x or NED north component.  [mm] </summary>
        [Units("[mm]")]
        [Description("Current baseline in ECEF x or NED north component.")]
        public  int baseline_a_mm;
            /// <summary>Current baseline in ECEF y or NED east component.  [mm] </summary>
        [Units("[mm]")]
        [Description("Current baseline in ECEF y or NED east component.")]
        public  int baseline_b_mm;
            /// <summary>Current baseline in ECEF z or NED down component.  [mm] </summary>
        [Units("[mm]")]
        [Description("Current baseline in ECEF z or NED down component.")]
        public  int baseline_c_mm;
            /// <summary>Current estimate of baseline accuracy.   </summary>
        [Units("")]
        [Description("Current estimate of baseline accuracy.")]
        public  uint accuracy;
            /// <summary>Current number of integer ambiguity hypotheses.   </summary>
        [Units("")]
        [Description("Current number of integer ambiguity hypotheses.")]
        public  int iar_num_hypotheses;
            /// <summary>GPS Week Number of last baseline   </summary>
        [Units("")]
        [Description("GPS Week Number of last baseline")]
        public  ushort wn;
            /// <summary>Identification of connected RTK receiver.   </summary>
        [Units("")]
        [Description("Identification of connected RTK receiver.")]
        public  byte rtk_receiver_id;
            /// <summary>GPS-specific health report for RTK data.   </summary>
        [Units("")]
        [Description("GPS-specific health report for RTK data.")]
        public  byte rtk_health;
            /// <summary>Rate of baseline messages being received by GPS  [Hz] </summary>
        [Units("[Hz]")]
        [Description("Rate of baseline messages being received by GPS")]
        public  byte rtk_rate;
            /// <summary>Current number of sats used for RTK calculation.   </summary>
        [Units("")]
        [Description("Current number of sats used for RTK calculation.")]
        public  byte nsats;
            /// <summary>Coordinate system of baseline RTK_BASELINE_COORDINATE_SYSTEM  </summary>
        [Units("")]
        [Description("Coordinate system of baseline")]
        public  /*RTK_BASELINE_COORDINATE_SYSTEM*/byte baseline_coords_type;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> The RAW IMU readings for 3rd 9DOF sensor setup. This message should contain the scaled values to the described units </summary>
    public struct mavlink_scaled_imu3_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>X acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("X acceleration")]
        public  short xacc;
            /// <summary>Y acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Y acceleration")]
        public  short yacc;
            /// <summary>Z acceleration  [mG] </summary>
        [Units("[mG]")]
        [Description("Z acceleration")]
        public  short zacc;
            /// <summary>Angular speed around X axis  [mrad/s] </summary>
        [Units("[mrad/s]")]
        [Description("Angular speed around X axis")]
        public  short xgyro;
            /// <summary>Angular speed around Y axis  [mrad/s] </summary>
        [Units("[mrad/s]")]
        [Description("Angular speed around Y axis")]
        public  short ygyro;
            /// <summary>Angular speed around Z axis  [mrad/s] </summary>
        [Units("[mrad/s]")]
        [Description("Angular speed around Z axis")]
        public  short zgyro;
            /// <summary>X Magnetic field  [mT] </summary>
        [Units("[mT]")]
        [Description("X Magnetic field")]
        public  short xmag;
            /// <summary>Y Magnetic field  [mT] </summary>
        [Units("[mT]")]
        [Description("Y Magnetic field")]
        public  short ymag;
            /// <summary>Z Magnetic field  [mT] </summary>
        [Units("[mT]")]
        [Description("Z Magnetic field")]
        public  short zmag;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=13)]
    ///<summary> Handshake message to initiate, control and stop image streaming when using the Image Transmission Protocol: https://mavlink.io/en/services/image_transmission.html. </summary>
    public struct mavlink_data_transmission_handshake_t
    {
        /// <summary>total data size (set on ACK only).  [bytes] </summary>
        [Units("[bytes]")]
        [Description("total data size (set on ACK only).")]
        public  uint size;
            /// <summary>Width of a matrix or image.   </summary>
        [Units("")]
        [Description("Width of a matrix or image.")]
        public  ushort width;
            /// <summary>Height of a matrix or image.   </summary>
        [Units("")]
        [Description("Height of a matrix or image.")]
        public  ushort height;
            /// <summary>Number of packets being sent (set on ACK only).   </summary>
        [Units("")]
        [Description("Number of packets being sent (set on ACK only).")]
        public  ushort packets;
            /// <summary>Type of requested/acknowledged data. MAVLINK_DATA_STREAM_TYPE  </summary>
        [Units("")]
        [Description("Type of requested/acknowledged data.")]
        public  /*MAVLINK_DATA_STREAM_TYPE*/byte type;
            /// <summary>Payload size per packet (normally 253 byte, see DATA field size in message ENCAPSULATED_DATA) (set on ACK only).  [bytes] </summary>
        [Units("[bytes]")]
        [Description("Payload size per packet (normally 253 byte, see DATA field size in message ENCAPSULATED_DATA) (set on ACK only).")]
        public  byte payload;
            /// <summary>JPEG quality. Values: [1-100].  [%] </summary>
        [Units("[%]")]
        [Description("JPEG quality. Values: [1-100].")]
        public  byte jpg_quality;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    ///<summary> Data packet for images sent using the Image Transmission Protocol: https://mavlink.io/en/services/image_transmission.html. </summary>
    public struct mavlink_encapsulated_data_t
    {
        /// <summary>sequence number (starting with 0 on every transmission)   </summary>
        [Units("")]
        [Description("sequence number (starting with 0 on every transmission)")]
        public  ushort seqnr;
            /// <summary>image data bytes   </summary>
        [Units("")]
        [Description("image data bytes")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=253)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=38)]
    ///<summary> Distance sensor information for an onboard rangefinder. </summary>
    public struct mavlink_distance_sensor_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Minimum distance the sensor can measure  [cm] </summary>
        [Units("[cm]")]
        [Description("Minimum distance the sensor can measure")]
        public  ushort min_distance;
            /// <summary>Maximum distance the sensor can measure  [cm] </summary>
        [Units("[cm]")]
        [Description("Maximum distance the sensor can measure")]
        public  ushort max_distance;
            /// <summary>Current distance reading  [cm] </summary>
        [Units("[cm]")]
        [Description("Current distance reading")]
        public  ushort current_distance;
            /// <summary>Type of distance sensor. MAV_DISTANCE_SENSOR  </summary>
        [Units("")]
        [Description("Type of distance sensor.")]
        public  /*MAV_DISTANCE_SENSOR*/byte type;
            /// <summary>Onboard ID of the sensor   </summary>
        [Units("")]
        [Description("Onboard ID of the sensor")]
        public  byte id;
            /// <summary>Direction the sensor faces. downward-facing: ROTATION_PITCH_270, upward-facing: ROTATION_PITCH_90, backward-facing: ROTATION_PITCH_180, forward-facing: ROTATION_NONE, left-facing: ROTATION_YAW_90, right-facing: ROTATION_YAW_270 MAV_SENSOR_ORIENTATION  </summary>
        [Units("")]
        [Description("Direction the sensor faces. downward-facing: ROTATION_PITCH_270, upward-facing: ROTATION_PITCH_90, backward-facing: ROTATION_PITCH_180, forward-facing: ROTATION_NONE, left-facing: ROTATION_YAW_90, right-facing: ROTATION_YAW_270")]
        public  /*MAV_SENSOR_ORIENTATION*/byte orientation;
            /// <summary>Measurement variance. Max standard deviation is 6cm. 256 if unknown.  [cm^2] </summary>
        [Units("[cm^2]")]
        [Description("Measurement variance. Max standard deviation is 6cm. 256 if unknown.")]
        public  byte covariance;
            /// <summary>Horizontal Field of View (angle) where the distance measurement is valid and the field of view is known. Otherwise this is set to 0.  [rad] </summary>
        [Units("[rad]")]
        [Description("Horizontal Field of View (angle) where the distance measurement is valid and the field of view is known. Otherwise this is set to 0.")]
        public  float horizontal_fov;
            /// <summary>Vertical Field of View (angle) where the distance measurement is valid and the field of view is known. Otherwise this is set to 0.  [rad] </summary>
        [Units("[rad]")]
        [Description("Vertical Field of View (angle) where the distance measurement is valid and the field of view is known. Otherwise this is set to 0.")]
        public  float vertical_fov;
            /// <summary>Quaternion of the sensor orientation in vehicle body frame (w, x, y, z order, zero-rotation is 1, 0, 0, 0). Zero-rotation is along the vehicle body x-axis. This field is required if the orientation is set to MAV_SENSOR_ROTATION_CUSTOM. Set it to 0 if invalid.'   </summary>
        [Units("")]
        [Description("Quaternion of the sensor orientation in vehicle body frame (w, x, y, z order, zero-rotation is 1, 0, 0, 0). Zero-rotation is along the vehicle body x-axis. This field is required if the orientation is set to MAV_SENSOR_ROTATION_CUSTOM. Set it to 0 if invalid.'")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] quaternion;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    ///<summary> Request for terrain data and terrain status </summary>
    public struct mavlink_terrain_request_t
    {
        /// <summary>Bitmask of requested 4x4 grids (row major 8x7 array of grids, 56 bits)   bitmask</summary>
        [Units("")]
        [Description("Bitmask of requested 4x4 grids (row major 8x7 array of grids, 56 bits)")]
        public  ulong mask;
            /// <summary>Latitude of SW corner of first grid  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude of SW corner of first grid")]
        public  int lat;
            /// <summary>Longitude of SW corner of first grid  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude of SW corner of first grid")]
        public  int lon;
            /// <summary>Grid spacing  [m] </summary>
        [Units("[m]")]
        [Description("Grid spacing")]
        public  ushort grid_spacing;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=43)]
    ///<summary> Terrain data sent from GCS. The lat/lon and grid_spacing must be the same as a lat/lon from a TERRAIN_REQUEST </summary>
    public struct mavlink_terrain_data_t
    {
        /// <summary>Latitude of SW corner of first grid  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude of SW corner of first grid")]
        public  int lat;
            /// <summary>Longitude of SW corner of first grid  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude of SW corner of first grid")]
        public  int lon;
            /// <summary>Grid spacing  [m] </summary>
        [Units("[m]")]
        [Description("Grid spacing")]
        public  ushort grid_spacing;
            /// <summary>Terrain data MSL  [m] </summary>
        [Units("[m]")]
        [Description("Terrain data MSL")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public Int16[] data;
            /// <summary>bit within the terrain request mask   </summary>
        [Units("")]
        [Description("bit within the terrain request mask")]
        public  byte gridbit;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=8)]
    ///<summary> Request that the vehicle report terrain height at the given location. Used by GCS to check if vehicle has all terrain data needed for a mission. </summary>
    public struct mavlink_terrain_check_t
    {
        /// <summary>Latitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude")]
        public  int lat;
            /// <summary>Longitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude")]
        public  int lon;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=22)]
    ///<summary> Response from a TERRAIN_CHECK request </summary>
    public struct mavlink_terrain_report_t
    {
        /// <summary>Latitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude")]
        public  int lat;
            /// <summary>Longitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude")]
        public  int lon;
            /// <summary>Terrain height MSL  [m] </summary>
        [Units("[m]")]
        [Description("Terrain height MSL")]
        public  float terrain_height;
            /// <summary>Current vehicle height above lat/lon terrain height  [m] </summary>
        [Units("[m]")]
        [Description("Current vehicle height above lat/lon terrain height")]
        public  float current_height;
            /// <summary>grid spacing (zero if terrain at this location unavailable)   </summary>
        [Units("")]
        [Description("grid spacing (zero if terrain at this location unavailable)")]
        public  ushort spacing;
            /// <summary>Number of 4x4 terrain blocks waiting to be received or read from disk   </summary>
        [Units("")]
        [Description("Number of 4x4 terrain blocks waiting to be received or read from disk")]
        public  ushort pending;
            /// <summary>Number of 4x4 terrain blocks in memory   </summary>
        [Units("")]
        [Description("Number of 4x4 terrain blocks in memory")]
        public  ushort loaded;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Barometer readings for 2nd barometer </summary>
    public struct mavlink_scaled_pressure2_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Absolute pressure  [hPa] </summary>
        [Units("[hPa]")]
        [Description("Absolute pressure")]
        public  float press_abs;
            /// <summary>Differential pressure  [hPa] </summary>
        [Units("[hPa]")]
        [Description("Differential pressure")]
        public  float press_diff;
            /// <summary>Temperature measurement  [cdegC] </summary>
        [Units("[cdegC]")]
        [Description("Temperature measurement")]
        public  short temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=120)]
    ///<summary> Motion capture attitude and position </summary>
    public struct mavlink_att_pos_mocap_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)   </summary>
        [Units("")]
        [Description("Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>X position (NED)  [m] </summary>
        [Units("[m]")]
        [Description("X position (NED)")]
        public  float x;
            /// <summary>Y position (NED)  [m] </summary>
        [Units("[m]")]
        [Description("Y position (NED)")]
        public  float y;
            /// <summary>Z position (NED)  [m] </summary>
        [Units("[m]")]
        [Description("Z position (NED)")]
        public  float z;
            /// <summary>Row-major representation of a pose 6x6 cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of a pose 6x6 cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] covariance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=43)]
    ///<summary> Set the vehicle attitude and body angular rates. </summary>
    public struct mavlink_set_actuator_control_target_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs.   </summary>
        [Units("")]
        [Description("Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public float[] controls;
            /// <summary>Actuator group. The '_mlx' indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances.   </summary>
        [Units("")]
        [Description("Actuator group. The '_mlx' indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances.")]
        public  byte group_mlx;
            /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=41)]
    ///<summary> Set the vehicle attitude and body angular rates. </summary>
    public struct mavlink_actuator_control_target_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs.   </summary>
        [Units("")]
        [Description("Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public float[] controls;
            /// <summary>Actuator group. The '_mlx' indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances.   </summary>
        [Units("")]
        [Description("Actuator group. The '_mlx' indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances.")]
        public  byte group_mlx;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    ///<summary> The current system altitude. </summary>
    public struct mavlink_altitude_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>This altitude measure is initialized on system boot and monotonic (it is never reset, but represents the local altitude change). The only guarantee on this field is that it will never be reset and is consistent within a flight. The recommended value for this field is the uncorrected barometric altitude at boot time. This altitude will also drift and vary between flights.  [m] </summary>
        [Units("[m]")]
        [Description("This altitude measure is initialized on system boot and monotonic (it is never reset, but represents the local altitude change). The only guarantee on this field is that it will never be reset and is consistent within a flight. The recommended value for this field is the uncorrected barometric altitude at boot time. This altitude will also drift and vary between flights.")]
        public  float altitude_monotonic;
            /// <summary>This altitude measure is strictly above mean sea level and might be non-monotonic (it might reset on events like GPS lock or when a new QNH value is set). It should be the altitude to which global altitude waypoints are compared to. Note that it is *not* the GPS altitude, however, most GPS modules already output MSL by default and not the WGS84 altitude.  [m] </summary>
        [Units("[m]")]
        [Description("This altitude measure is strictly above mean sea level and might be non-monotonic (it might reset on events like GPS lock or when a new QNH value is set). It should be the altitude to which global altitude waypoints are compared to. Note that it is *not* the GPS altitude, however, most GPS modules already output MSL by default and not the WGS84 altitude.")]
        public  float altitude_amsl;
            /// <summary>This is the local altitude in the local coordinate frame. It is not the altitude above home, but in reference to the coordinate origin (0, 0, 0). It is up-positive.  [m] </summary>
        [Units("[m]")]
        [Description("This is the local altitude in the local coordinate frame. It is not the altitude above home, but in reference to the coordinate origin (0, 0, 0). It is up-positive.")]
        public  float altitude_local;
            /// <summary>This is the altitude above the home position. It resets on each change of the current home position.  [m] </summary>
        [Units("[m]")]
        [Description("This is the altitude above the home position. It resets on each change of the current home position.")]
        public  float altitude_relative;
            /// <summary>This is the altitude above terrain. It might be fed by a terrain database or an altimeter. Values smaller than -1000 should be interpreted as unknown.  [m] </summary>
        [Units("[m]")]
        [Description("This is the altitude above terrain. It might be fed by a terrain database or an altimeter. Values smaller than -1000 should be interpreted as unknown.")]
        public  float altitude_terrain;
            /// <summary>This is not the altitude, but the clear space below the system according to the fused clearance estimate. It generally should max out at the maximum range of e.g. the laser altimeter. It is generally a moving target. A negative value indicates no measurement available.  [m] </summary>
        [Units("[m]")]
        [Description("This is not the altitude, but the clear space below the system according to the fused clearance estimate. It generally should max out at the maximum range of e.g. the laser altimeter. It is generally a moving target. A negative value indicates no measurement available.")]
        public  float bottom_clearance;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=243)]
    ///<summary> The autopilot is requesting a resource (file, binary, other type of data) </summary>
    public struct mavlink_resource_request_t
    {
        /// <summary>Request ID. This ID should be re-used when sending back URI contents   </summary>
        [Units("")]
        [Description("Request ID. This ID should be re-used when sending back URI contents")]
        public  byte request_id;
            /// <summary>The type of requested URI. 0 = a file via URL. 1 = a UAVCAN binary   </summary>
        [Units("")]
        [Description("The type of requested URI. 0 = a file via URL. 1 = a UAVCAN binary")]
        public  byte uri_type;
            /// <summary>The requested unique resource identifier (URI). It is not necessarily a straight domain name (depends on the URI type enum)   </summary>
        [Units("")]
        [Description("The requested unique resource identifier (URI). It is not necessarily a straight domain name (depends on the URI type enum)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=120)]
		public byte[] uri;
            /// <summary>The way the autopilot wants to receive the URI. 0 = MAVLink FTP. 1 = binary stream.   </summary>
        [Units("")]
        [Description("The way the autopilot wants to receive the URI. 0 = MAVLink FTP. 1 = binary stream.")]
        public  byte transfer_type;
            /// <summary>The storage path the autopilot wants the URI to be stored in. Will only be valid if the transfer_type has a storage associated (e.g. MAVLink FTP).   </summary>
        [Units("")]
        [Description("The storage path the autopilot wants the URI to be stored in. Will only be valid if the transfer_type has a storage associated (e.g. MAVLink FTP).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=120)]
		public byte[] storage;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=14)]
    ///<summary> Barometer readings for 3rd barometer </summary>
    public struct mavlink_scaled_pressure3_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Absolute pressure  [hPa] </summary>
        [Units("[hPa]")]
        [Description("Absolute pressure")]
        public  float press_abs;
            /// <summary>Differential pressure  [hPa] </summary>
        [Units("[hPa]")]
        [Description("Differential pressure")]
        public  float press_diff;
            /// <summary>Temperature measurement  [cdegC] </summary>
        [Units("[cdegC]")]
        [Description("Temperature measurement")]
        public  short temperature;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=93)]
    ///<summary> Current motion information from a designated system </summary>
    public struct mavlink_follow_target_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  ulong timestamp;
            /// <summary>button states or switches of a tracker device   </summary>
        [Units("")]
        [Description("button states or switches of a tracker device")]
        public  ulong custom_state;
            /// <summary>Latitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude (WGS84)")]
        public  int lat;
            /// <summary>Longitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude (WGS84)")]
        public  int lon;
            /// <summary>Altitude (MSL)  [m] </summary>
        [Units("[m]")]
        [Description("Altitude (MSL)")]
        public  float alt;
            /// <summary>target velocity (0,0,0) for unknown  [m/s] </summary>
        [Units("[m/s]")]
        [Description("target velocity (0,0,0) for unknown")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] vel;
            /// <summary>linear target acceleration (0,0,0) for unknown  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("linear target acceleration (0,0,0) for unknown")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] acc;
            /// <summary>(1 0 0 0 for unknown)   </summary>
        [Units("")]
        [Description("(1 0 0 0 for unknown)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] attitude_q;
            /// <summary>(0 0 0 for unknown)   </summary>
        [Units("")]
        [Description("(0 0 0 for unknown)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] rates;
            /// <summary>eph epv   </summary>
        [Units("")]
        [Description("eph epv")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] position_cov;
            /// <summary>bit positions for tracker reporting capabilities (POS = 0, VEL = 1, ACCEL = 2, ATT + RATES = 3)   </summary>
        [Units("")]
        [Description("bit positions for tracker reporting capabilities (POS = 0, VEL = 1, ACCEL = 2, ATT + RATES = 3)")]
        public  byte est_capabilities;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=100)]
    ///<summary> The smoothed, monotonic system state used to feed the control loops of the system. </summary>
    public struct mavlink_control_system_state_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>X acceleration in body frame  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("X acceleration in body frame")]
        public  float x_acc;
            /// <summary>Y acceleration in body frame  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Y acceleration in body frame")]
        public  float y_acc;
            /// <summary>Z acceleration in body frame  [m/s/s] </summary>
        [Units("[m/s/s]")]
        [Description("Z acceleration in body frame")]
        public  float z_acc;
            /// <summary>X velocity in body frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("X velocity in body frame")]
        public  float x_vel;
            /// <summary>Y velocity in body frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Y velocity in body frame")]
        public  float y_vel;
            /// <summary>Z velocity in body frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Z velocity in body frame")]
        public  float z_vel;
            /// <summary>X position in local frame  [m] </summary>
        [Units("[m]")]
        [Description("X position in local frame")]
        public  float x_pos;
            /// <summary>Y position in local frame  [m] </summary>
        [Units("[m]")]
        [Description("Y position in local frame")]
        public  float y_pos;
            /// <summary>Z position in local frame  [m] </summary>
        [Units("[m]")]
        [Description("Z position in local frame")]
        public  float z_pos;
            /// <summary>Airspeed, set to -1 if unknown  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Airspeed, set to -1 if unknown")]
        public  float airspeed;
            /// <summary>Variance of body velocity estimate   </summary>
        [Units("")]
        [Description("Variance of body velocity estimate")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] vel_variance;
            /// <summary>Variance in local position   </summary>
        [Units("")]
        [Description("Variance in local position")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
		public float[] pos_variance;
            /// <summary>The attitude, represented as Quaternion   </summary>
        [Units("")]
        [Description("The attitude, represented as Quaternion")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Angular rate in roll axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular rate in roll axis")]
        public  float roll_rate;
            /// <summary>Angular rate in pitch axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular rate in pitch axis")]
        public  float pitch_rate;
            /// <summary>Angular rate in yaw axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Angular rate in yaw axis")]
        public  float yaw_rate;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=41)]
    ///<summary> Battery information </summary>
    public struct mavlink_battery_status_t
    {
        /// <summary>Consumed charge, -1: autopilot does not provide consumption estimate  [mAh] </summary>
        [Units("[mAh]")]
        [Description("Consumed charge, -1: autopilot does not provide consumption estimate")]
        public  int current_consumed;
            /// <summary>Consumed energy, -1: autopilot does not provide energy consumption estimate  [hJ] </summary>
        [Units("[hJ]")]
        [Description("Consumed energy, -1: autopilot does not provide energy consumption estimate")]
        public  int energy_consumed;
            /// <summary>Temperature of the battery. INT16_MAX for unknown temperature.  [cdegC] </summary>
        [Units("[cdegC]")]
        [Description("Temperature of the battery. INT16_MAX for unknown temperature.")]
        public  short temperature;
            /// <summary>Battery voltage of cells. Cells above the valid cell count for this battery should have the UINT16_MAX value.  [mV] </summary>
        [Units("[mV]")]
        [Description("Battery voltage of cells. Cells above the valid cell count for this battery should have the UINT16_MAX value.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public UInt16[] voltages;
            /// <summary>Battery current, -1: autopilot does not measure the current  [cA] </summary>
        [Units("[cA]")]
        [Description("Battery current, -1: autopilot does not measure the current")]
        public  short current_battery;
            /// <summary>Battery ID   </summary>
        [Units("")]
        [Description("Battery ID")]
        public  byte id;
            /// <summary>Function of the battery MAV_BATTERY_FUNCTION  </summary>
        [Units("")]
        [Description("Function of the battery")]
        public  /*MAV_BATTERY_FUNCTION*/byte battery_function;
            /// <summary>Type (chemistry) of the battery MAV_BATTERY_TYPE  </summary>
        [Units("")]
        [Description("Type (chemistry) of the battery")]
        public  /*MAV_BATTERY_TYPE*/byte type;
            /// <summary>Remaining battery energy. Values: [0-100], -1: autopilot does not estimate the remaining battery.  [%] </summary>
        [Units("[%]")]
        [Description("Remaining battery energy. Values: [0-100], -1: autopilot does not estimate the remaining battery.")]
        public  byte battery_remaining;
            /// <summary>Remaining battery time, 0: autopilot does not provide remaining battery time estimate  [s] </summary>
        [Units("[s]")]
        [Description("Remaining battery time, 0: autopilot does not provide remaining battery time estimate")]
        public  int time_remaining;
            /// <summary>State for extent of discharge, provided by autopilot for warning or external reactions MAV_BATTERY_CHARGE_STATE  </summary>
        [Units("")]
        [Description("State for extent of discharge, provided by autopilot for warning or external reactions")]
        public  /*MAV_BATTERY_CHARGE_STATE*/byte charge_state;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=78)]
    ///<summary> Version and capability of autopilot software </summary>
    public struct mavlink_autopilot_version_t
    {
        /// <summary>Bitmap of capabilities MAV_PROTOCOL_CAPABILITY  bitmask</summary>
        [Units("")]
        [Description("Bitmap of capabilities")]
        public  /*MAV_PROTOCOL_CAPABILITY*/ulong capabilities;
            /// <summary>UID if provided by hardware (see uid2)   </summary>
        [Units("")]
        [Description("UID if provided by hardware (see uid2)")]
        public  ulong uid;
            /// <summary>Firmware version number   </summary>
        [Units("")]
        [Description("Firmware version number")]
        public  uint flight_sw_version;
            /// <summary>Middleware version number   </summary>
        [Units("")]
        [Description("Middleware version number")]
        public  uint middleware_sw_version;
            /// <summary>Operating system version number   </summary>
        [Units("")]
        [Description("Operating system version number")]
        public  uint os_sw_version;
            /// <summary>HW / board version (last 8 bytes should be silicon ID, if any)   </summary>
        [Units("")]
        [Description("HW / board version (last 8 bytes should be silicon ID, if any)")]
        public  uint board_version;
            /// <summary>ID of the board vendor   </summary>
        [Units("")]
        [Description("ID of the board vendor")]
        public  ushort vendor_id;
            /// <summary>ID of the product   </summary>
        [Units("")]
        [Description("ID of the product")]
        public  ushort product_id;
            /// <summary>Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.   </summary>
        [Units("")]
        [Description("Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] flight_custom_version;
            /// <summary>Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.   </summary>
        [Units("")]
        [Description("Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] middleware_custom_version;
            /// <summary>Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.   </summary>
        [Units("")]
        [Description("Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] os_custom_version;
            /// <summary>UID if provided by hardware (supersedes the uid field. If this is non-zero, use this field, otherwise use uid)   </summary>
        [Units("")]
        [Description("UID if provided by hardware (supersedes the uid field. If this is non-zero, use this field, otherwise use uid)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=18)]
		public byte[] uid2;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=60)]
    ///<summary> The location of a landing target. See: https://mavlink.io/en/services/landing_target.html </summary>
    public struct mavlink_landing_target_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>X-axis angular offset of the target from the center of the image  [rad] </summary>
        [Units("[rad]")]
        [Description("X-axis angular offset of the target from the center of the image")]
        public  float angle_x;
            /// <summary>Y-axis angular offset of the target from the center of the image  [rad] </summary>
        [Units("[rad]")]
        [Description("Y-axis angular offset of the target from the center of the image")]
        public  float angle_y;
            /// <summary>Distance to the target from the vehicle  [m] </summary>
        [Units("[m]")]
        [Description("Distance to the target from the vehicle")]
        public  float distance;
            /// <summary>Size of target along x-axis  [rad] </summary>
        [Units("[rad]")]
        [Description("Size of target along x-axis")]
        public  float size_x;
            /// <summary>Size of target along y-axis  [rad] </summary>
        [Units("[rad]")]
        [Description("Size of target along y-axis")]
        public  float size_y;
            /// <summary>The ID of the target if multiple targets are present   </summary>
        [Units("")]
        [Description("The ID of the target if multiple targets are present")]
        public  byte target_num;
            /// <summary>Coordinate frame used for following fields. MAV_FRAME  </summary>
        [Units("")]
        [Description("Coordinate frame used for following fields.")]
        public  /*MAV_FRAME*/byte frame;
            /// <summary>X Position of the landing target in MAV_FRAME  [m] </summary>
        [Units("[m]")]
        [Description("X Position of the landing target in MAV_FRAME")]
        public  float x;
            /// <summary>Y Position of the landing target in MAV_FRAME  [m] </summary>
        [Units("[m]")]
        [Description("Y Position of the landing target in MAV_FRAME")]
        public  float y;
            /// <summary>Z Position of the landing target in MAV_FRAME  [m] </summary>
        [Units("[m]")]
        [Description("Z Position of the landing target in MAV_FRAME")]
        public  float z;
            /// <summary>Quaternion of landing target orientation (w, x, y, z order, zero-rotation is 1, 0, 0, 0)   </summary>
        [Units("")]
        [Description("Quaternion of landing target orientation (w, x, y, z order, zero-rotation is 1, 0, 0, 0)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Type of landing target LANDING_TARGET_TYPE  </summary>
        [Units("")]
        [Description("Type of landing target")]
        public  /*LANDING_TARGET_TYPE*/byte type;
            /// <summary>Boolean indicating whether the position fields (x, y, z, q, type) contain valid target position information (valid: 1, invalid: 0). Default is 0 (invalid).   </summary>
        [Units("")]
        [Description("Boolean indicating whether the position fields (x, y, z, q, type) contain valid target position information (valid: 1, invalid: 0). Default is 0 (invalid).")]
        public  byte position_valid;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> Estimator status message including flags, innovation test ratios and estimated accuracies. The flags message is an integer bitmask containing information on which EKF outputs are valid. See the ESTIMATOR_STATUS_FLAGS enum definition for further information. The innovation test ratios show the magnitude of the sensor innovation divided by the innovation check threshold. Under normal operation the innovation test ratios should be below 0.5 with occasional values up to 1.0. Values greater than 1.0 should be rare under normal operation and indicate that a measurement has been rejected by the filter. The user should be notified if an innovation test ratio greater than 1.0 is recorded. Notifications for values in the range between 0.5 and 1.0 should be optional and controllable by the user. </summary>
    public struct mavlink_estimator_status_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Velocity innovation test ratio   </summary>
        [Units("")]
        [Description("Velocity innovation test ratio")]
        public  float vel_ratio;
            /// <summary>Horizontal position innovation test ratio   </summary>
        [Units("")]
        [Description("Horizontal position innovation test ratio")]
        public  float pos_horiz_ratio;
            /// <summary>Vertical position innovation test ratio   </summary>
        [Units("")]
        [Description("Vertical position innovation test ratio")]
        public  float pos_vert_ratio;
            /// <summary>Magnetometer innovation test ratio   </summary>
        [Units("")]
        [Description("Magnetometer innovation test ratio")]
        public  float mag_ratio;
            /// <summary>Height above terrain innovation test ratio   </summary>
        [Units("")]
        [Description("Height above terrain innovation test ratio")]
        public  float hagl_ratio;
            /// <summary>True airspeed innovation test ratio   </summary>
        [Units("")]
        [Description("True airspeed innovation test ratio")]
        public  float tas_ratio;
            /// <summary>Horizontal position 1-STD accuracy relative to the EKF local origin  [m] </summary>
        [Units("[m]")]
        [Description("Horizontal position 1-STD accuracy relative to the EKF local origin")]
        public  float pos_horiz_accuracy;
            /// <summary>Vertical position 1-STD accuracy relative to the EKF local origin  [m] </summary>
        [Units("[m]")]
        [Description("Vertical position 1-STD accuracy relative to the EKF local origin")]
        public  float pos_vert_accuracy;
            /// <summary>Bitmap indicating which EKF outputs are valid. ESTIMATOR_STATUS_FLAGS  bitmask</summary>
        [Units("")]
        [Description("Bitmap indicating which EKF outputs are valid.")]
        public  /*ESTIMATOR_STATUS_FLAGS*/ushort flags;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=40)]
    ///<summary> Wind covariance estimate from vehicle. </summary>
    public struct mavlink_wind_cov_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Wind in X (NED) direction  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Wind in X (NED) direction")]
        public  float wind_x;
            /// <summary>Wind in Y (NED) direction  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Wind in Y (NED) direction")]
        public  float wind_y;
            /// <summary>Wind in Z (NED) direction  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Wind in Z (NED) direction")]
        public  float wind_z;
            /// <summary>Variability of the wind in XY. RMS of a 1 Hz lowpassed wind estimate.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Variability of the wind in XY. RMS of a 1 Hz lowpassed wind estimate.")]
        public  float var_horiz;
            /// <summary>Variability of the wind in Z. RMS of a 1 Hz lowpassed wind estimate.  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Variability of the wind in Z. RMS of a 1 Hz lowpassed wind estimate.")]
        public  float var_vert;
            /// <summary>Altitude (MSL) that this measurement was taken at  [m] </summary>
        [Units("[m]")]
        [Description("Altitude (MSL) that this measurement was taken at")]
        public  float wind_alt;
            /// <summary>Horizontal speed 1-STD accuracy  [m] </summary>
        [Units("[m]")]
        [Description("Horizontal speed 1-STD accuracy")]
        public  float horiz_accuracy;
            /// <summary>Vertical speed 1-STD accuracy  [m] </summary>
        [Units("[m]")]
        [Description("Vertical speed 1-STD accuracy")]
        public  float vert_accuracy;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=63)]
    ///<summary> GPS sensor input message.  This is a raw sensor value sent by the GPS. This is NOT the global position estimate of the system. </summary>
    public struct mavlink_gps_input_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>GPS time (from start of GPS week)  [ms] </summary>
        [Units("[ms]")]
        [Description("GPS time (from start of GPS week)")]
        public  uint time_week_ms;
            /// <summary>Latitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude (WGS84)")]
        public  int lat;
            /// <summary>Longitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude (WGS84)")]
        public  int lon;
            /// <summary>Altitude (MSL). Positive for up.  [m] </summary>
        [Units("[m]")]
        [Description("Altitude (MSL). Positive for up.")]
        public  float alt;
            /// <summary>GPS HDOP horizontal dilution of position  [m] </summary>
        [Units("[m]")]
        [Description("GPS HDOP horizontal dilution of position")]
        public  float hdop;
            /// <summary>GPS VDOP vertical dilution of position  [m] </summary>
        [Units("[m]")]
        [Description("GPS VDOP vertical dilution of position")]
        public  float vdop;
            /// <summary>GPS velocity in NORTH direction in earth-fixed NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("GPS velocity in NORTH direction in earth-fixed NED frame")]
        public  float vn;
            /// <summary>GPS velocity in EAST direction in earth-fixed NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("GPS velocity in EAST direction in earth-fixed NED frame")]
        public  float ve;
            /// <summary>GPS velocity in DOWN direction in earth-fixed NED frame  [m/s] </summary>
        [Units("[m/s]")]
        [Description("GPS velocity in DOWN direction in earth-fixed NED frame")]
        public  float vd;
            /// <summary>GPS speed accuracy  [m/s] </summary>
        [Units("[m/s]")]
        [Description("GPS speed accuracy")]
        public  float speed_accuracy;
            /// <summary>GPS horizontal accuracy  [m] </summary>
        [Units("[m]")]
        [Description("GPS horizontal accuracy")]
        public  float horiz_accuracy;
            /// <summary>GPS vertical accuracy  [m] </summary>
        [Units("[m]")]
        [Description("GPS vertical accuracy")]
        public  float vert_accuracy;
            /// <summary>Bitmap indicating which GPS input flags fields to ignore.  All other fields must be provided. GPS_INPUT_IGNORE_FLAGS  bitmask</summary>
        [Units("")]
        [Description("Bitmap indicating which GPS input flags fields to ignore.  All other fields must be provided.")]
        public  /*GPS_INPUT_IGNORE_FLAGS*/ushort ignore_flags;
            /// <summary>GPS week number   </summary>
        [Units("")]
        [Description("GPS week number")]
        public  ushort time_week;
            /// <summary>ID of the GPS for multiple GPS inputs   </summary>
        [Units("")]
        [Description("ID of the GPS for multiple GPS inputs")]
        public  byte gps_id;
            /// <summary>0-1: no fix, 2: 2D fix, 3: 3D fix. 4: 3D with DGPS. 5: 3D with RTK   </summary>
        [Units("")]
        [Description("0-1: no fix, 2: 2D fix, 3: 3D fix. 4: 3D with DGPS. 5: 3D with RTK")]
        public  byte fix_type;
            /// <summary>Number of satellites visible.   </summary>
        [Units("")]
        [Description("Number of satellites visible.")]
        public  byte satellites_visible;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=182)]
    ///<summary> RTCM message for injecting into the onboard GPS (used for DGPS) </summary>
    public struct mavlink_gps_rtcm_data_t
    {
        /// <summary>LSB: 1 means message is fragmented, next 2 bits are the fragment ID, the remaining 5 bits are used for the sequence ID. Messages are only to be flushed to the GPS when the entire message has been reconstructed on the autopilot. The fragment ID specifies which order the fragments should be assembled into a buffer, while the sequence ID is used to detect a mismatch between different buffers. The buffer is considered fully reconstructed when either all 4 fragments are present, or all the fragments before the first fragment with a non full payload is received. This management is used to ensure that normal GPS operation doesn't corrupt RTCM data, and to recover from a unreliable transport delivery order.   </summary>
        [Units("")]
        [Description("LSB: 1 means message is fragmented, next 2 bits are the fragment ID, the remaining 5 bits are used for the sequence ID. Messages are only to be flushed to the GPS when the entire message has been reconstructed on the autopilot. The fragment ID specifies which order the fragments should be assembled into a buffer, while the sequence ID is used to detect a mismatch between different buffers. The buffer is considered fully reconstructed when either all 4 fragments are present, or all the fragments before the first fragment with a non full payload is received. This management is used to ensure that normal GPS operation doesn't corrupt RTCM data, and to recover from a unreliable transport delivery order.")]
        public  byte flags;
            /// <summary>data length  [bytes] </summary>
        [Units("[bytes]")]
        [Description("data length")]
        public  byte len;
            /// <summary>RTCM message (may be fragmented)   </summary>
        [Units("")]
        [Description("RTCM message (may be fragmented)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=180)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=40)]
    ///<summary> Message appropriate for high latency connections like Iridium </summary>
    public struct mavlink_high_latency_t
    {
        /// <summary>A bitfield for use for autopilot-specific flags.   bitmask</summary>
        [Units("")]
        [Description("A bitfield for use for autopilot-specific flags.")]
        public  uint custom_mode;
            /// <summary>Latitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude")]
        public  int latitude;
            /// <summary>Longitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude")]
        public  int longitude;
            /// <summary>roll  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("roll")]
        public  short roll;
            /// <summary>pitch  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("pitch")]
        public  short pitch;
            /// <summary>heading  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("heading")]
        public  ushort heading;
            /// <summary>heading setpoint  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("heading setpoint")]
        public  short heading_sp;
            /// <summary>Altitude above mean sea level  [m] </summary>
        [Units("[m]")]
        [Description("Altitude above mean sea level")]
        public  short altitude_amsl;
            /// <summary>Altitude setpoint relative to the home position  [m] </summary>
        [Units("[m]")]
        [Description("Altitude setpoint relative to the home position")]
        public  short altitude_sp;
            /// <summary>distance to target  [m] </summary>
        [Units("[m]")]
        [Description("distance to target")]
        public  ushort wp_distance;
            /// <summary>Bitmap of enabled system modes. MAV_MODE_FLAG  bitmask</summary>
        [Units("")]
        [Description("Bitmap of enabled system modes.")]
        public  /*MAV_MODE_FLAG*/byte base_mode;
            /// <summary>The landed state. Is set to MAV_LANDED_STATE_UNDEFINED if landed state is unknown. MAV_LANDED_STATE  </summary>
        [Units("")]
        [Description("The landed state. Is set to MAV_LANDED_STATE_UNDEFINED if landed state is unknown.")]
        public  /*MAV_LANDED_STATE*/byte landed_state;
            /// <summary>throttle (percentage)  [%] </summary>
        [Units("[%]")]
        [Description("throttle (percentage)")]
        public  byte throttle;
            /// <summary>airspeed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("airspeed")]
        public  byte airspeed;
            /// <summary>airspeed setpoint  [m/s] </summary>
        [Units("[m/s]")]
        [Description("airspeed setpoint")]
        public  byte airspeed_sp;
            /// <summary>groundspeed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("groundspeed")]
        public  byte groundspeed;
            /// <summary>climb rate  [m/s] </summary>
        [Units("[m/s]")]
        [Description("climb rate")]
        public  byte climb_rate;
            /// <summary>Number of satellites visible. If unknown, set to 255   </summary>
        [Units("")]
        [Description("Number of satellites visible. If unknown, set to 255")]
        public  byte gps_nsat;
            /// <summary>GPS Fix type. GPS_FIX_TYPE  </summary>
        [Units("")]
        [Description("GPS Fix type.")]
        public  /*GPS_FIX_TYPE*/byte gps_fix_type;
            /// <summary>Remaining battery (percentage)  [%] </summary>
        [Units("[%]")]
        [Description("Remaining battery (percentage)")]
        public  byte battery_remaining;
            /// <summary>Autopilot temperature (degrees C)  [degC] </summary>
        [Units("[degC]")]
        [Description("Autopilot temperature (degrees C)")]
        public  byte temperature;
            /// <summary>Air temperature (degrees C) from airspeed sensor  [degC] </summary>
        [Units("[degC]")]
        [Description("Air temperature (degrees C) from airspeed sensor")]
        public  byte temperature_air;
            /// <summary>failsafe (each bit represents a failsafe where 0=ok, 1=failsafe active (bit0:RC, bit1:batt, bit2:GPS, bit3:GCS, bit4:fence)   </summary>
        [Units("")]
        [Description("failsafe (each bit represents a failsafe where 0=ok, 1=failsafe active (bit0:RC, bit1:batt, bit2:GPS, bit3:GCS, bit4:fence)")]
        public  byte failsafe;
            /// <summary>current waypoint number   </summary>
        [Units("")]
        [Description("current waypoint number")]
        public  byte wp_num;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=32)]
    ///<summary> Vibration levels and accelerometer clipping </summary>
    public struct mavlink_vibration_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Vibration levels on X-axis   </summary>
        [Units("")]
        [Description("Vibration levels on X-axis")]
        public  float vibration_x;
            /// <summary>Vibration levels on Y-axis   </summary>
        [Units("")]
        [Description("Vibration levels on Y-axis")]
        public  float vibration_y;
            /// <summary>Vibration levels on Z-axis   </summary>
        [Units("")]
        [Description("Vibration levels on Z-axis")]
        public  float vibration_z;
            /// <summary>first accelerometer clipping count   </summary>
        [Units("")]
        [Description("first accelerometer clipping count")]
        public  uint clipping_0;
            /// <summary>second accelerometer clipping count   </summary>
        [Units("")]
        [Description("second accelerometer clipping count")]
        public  uint clipping_1;
            /// <summary>third accelerometer clipping count   </summary>
        [Units("")]
        [Description("third accelerometer clipping count")]
        public  uint clipping_2;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=60)]
    ///<summary> This message can be requested by sending the MAV_CMD_GET_HOME_POSITION command. The position the system will return to and land on. The position is set automatically by the system during the takeoff in case it was not explicitly set by the operator before or after. The position the system will return to and land on. The global and local positions encode the position in the respective coordinate frames, while the q parameter encodes the orientation of the surface. Under normal conditions it describes the heading and terrain slope, which can be used by the aircraft to adjust the approach. The approach 3D vector describes the point to which the system should fly in normal flight mode and then perform a landing sequence along the vector. </summary>
    public struct mavlink_home_position_t
    {
        /// <summary>Latitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude (WGS84)")]
        public  int latitude;
            /// <summary>Longitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude (WGS84)")]
        public  int longitude;
            /// <summary>Altitude (MSL). Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (MSL). Positive for up.")]
        public  int altitude;
            /// <summary>Local X position of this position in the local coordinate frame  [m] </summary>
        [Units("[m]")]
        [Description("Local X position of this position in the local coordinate frame")]
        public  float x;
            /// <summary>Local Y position of this position in the local coordinate frame  [m] </summary>
        [Units("[m]")]
        [Description("Local Y position of this position in the local coordinate frame")]
        public  float y;
            /// <summary>Local Z position of this position in the local coordinate frame  [m] </summary>
        [Units("[m]")]
        [Description("Local Z position of this position in the local coordinate frame")]
        public  float z;
            /// <summary>World to surface normal and heading transformation of the takeoff position. Used to indicate the heading and slope of the ground   </summary>
        [Units("")]
        [Description("World to surface normal and heading transformation of the takeoff position. Used to indicate the heading and slope of the ground")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Local X position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        [Units("[m]")]
        [Description("Local X position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
        public  float approach_x;
            /// <summary>Local Y position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        [Units("[m]")]
        [Description("Local Y position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
        public  float approach_y;
            /// <summary>Local Z position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        [Units("[m]")]
        [Description("Local Z position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
        public  float approach_z;
            /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=61)]
    ///<summary> The position the system will return to and land on. The position is set automatically by the system during the takeoff in case it was not explicitly set by the operator before or after. The global and local positions encode the position in the respective coordinate frames, while the q parameter encodes the orientation of the surface. Under normal conditions it describes the heading and terrain slope, which can be used by the aircraft to adjust the approach. The approach 3D vector describes the point to which the system should fly in normal flight mode and then perform a landing sequence along the vector. </summary>
    public struct mavlink_set_home_position_t
    {
        /// <summary>Latitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude (WGS84)")]
        public  int latitude;
            /// <summary>Longitude (WGS84)  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude (WGS84)")]
        public  int longitude;
            /// <summary>Altitude (MSL). Positive for up.  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (MSL). Positive for up.")]
        public  int altitude;
            /// <summary>Local X position of this position in the local coordinate frame  [m] </summary>
        [Units("[m]")]
        [Description("Local X position of this position in the local coordinate frame")]
        public  float x;
            /// <summary>Local Y position of this position in the local coordinate frame  [m] </summary>
        [Units("[m]")]
        [Description("Local Y position of this position in the local coordinate frame")]
        public  float y;
            /// <summary>Local Z position of this position in the local coordinate frame  [m] </summary>
        [Units("[m]")]
        [Description("Local Z position of this position in the local coordinate frame")]
        public  float z;
            /// <summary>World to surface normal and heading transformation of the takeoff position. Used to indicate the heading and slope of the ground   </summary>
        [Units("")]
        [Description("World to surface normal and heading transformation of the takeoff position. Used to indicate the heading and slope of the ground")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Local X position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        [Units("[m]")]
        [Description("Local X position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
        public  float approach_x;
            /// <summary>Local Y position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        [Units("[m]")]
        [Description("Local Y position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
        public  float approach_y;
            /// <summary>Local Z position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.  [m] </summary>
        [Units("[m]")]
        [Description("Local Z position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
        public  float approach_z;
            /// <summary>System ID.   </summary>
        [Units("")]
        [Description("System ID.")]
        public  byte target_system;
            /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=6)]
    ///<summary> The interval between messages for a particular MAVLink message ID. This interface replaces DATA_STREAM </summary>
    public struct mavlink_message_interval_t
    {
        /// <summary>The interval between two messages. A value of -1 indicates this stream is disabled, 0 indicates it is not available, > 0 indicates the interval at which it is sent.  [us] </summary>
        [Units("[us]")]
        [Description("The interval between two messages. A value of -1 indicates this stream is disabled, 0 indicates it is not available, > 0 indicates the interval at which it is sent.")]
        public  int interval_us;
            /// <summary>The ID of the requested MAVLink message. v1.0 is limited to 254 messages.   </summary>
        [Units("")]
        [Description("The ID of the requested MAVLink message. v1.0 is limited to 254 messages.")]
        public  ushort message_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
    ///<summary> Provides state for additional features </summary>
    public struct mavlink_extended_sys_state_t
    {
        /// <summary>The VTOL state if applicable. Is set to MAV_VTOL_STATE_UNDEFINED if UAV is not in VTOL configuration. MAV_VTOL_STATE  </summary>
        [Units("")]
        [Description("The VTOL state if applicable. Is set to MAV_VTOL_STATE_UNDEFINED if UAV is not in VTOL configuration.")]
        public  /*MAV_VTOL_STATE*/byte vtol_state;
            /// <summary>The landed state. Is set to MAV_LANDED_STATE_UNDEFINED if landed state is unknown. MAV_LANDED_STATE  </summary>
        [Units("")]
        [Description("The landed state. Is set to MAV_LANDED_STATE_UNDEFINED if landed state is unknown.")]
        public  /*MAV_LANDED_STATE*/byte landed_state;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=38)]
    ///<summary> The location and information of an ADSB vehicle </summary>
    public struct mavlink_adsb_vehicle_t
    {
        /// <summary>ICAO address   </summary>
        [Units("")]
        [Description("ICAO address")]
        public  uint ICAO_address;
            /// <summary>Latitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude")]
        public  int lat;
            /// <summary>Longitude  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude")]
        public  int lon;
            /// <summary>Altitude(ASL)  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude(ASL)")]
        public  int altitude;
            /// <summary>Course over ground  [cdeg] </summary>
        [Units("[cdeg]")]
        [Description("Course over ground")]
        public  ushort heading;
            /// <summary>The horizontal velocity  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("The horizontal velocity")]
        public  ushort hor_velocity;
            /// <summary>The vertical velocity. Positive is up  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("The vertical velocity. Positive is up")]
        public  short ver_velocity;
            /// <summary>Bitmap to indicate various statuses including valid data fields ADSB_FLAGS  bitmask</summary>
        [Units("")]
        [Description("Bitmap to indicate various statuses including valid data fields")]
        public  /*ADSB_FLAGS*/ushort flags;
            /// <summary>Squawk code   </summary>
        [Units("")]
        [Description("Squawk code")]
        public  ushort squawk;
            /// <summary>ADSB altitude type. ADSB_ALTITUDE_TYPE  </summary>
        [Units("")]
        [Description("ADSB altitude type.")]
        public  /*ADSB_ALTITUDE_TYPE*/byte altitude_type;
            /// <summary>The callsign, 8+null   </summary>
        [Units("")]
        [Description("The callsign, 8+null")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)]
		public byte[] callsign;
            /// <summary>ADSB emitter type. ADSB_EMITTER_TYPE  </summary>
        [Units("")]
        [Description("ADSB emitter type.")]
        public  /*ADSB_EMITTER_TYPE*/byte emitter_type;
            /// <summary>Time since last communication in seconds  [s] </summary>
        [Units("[s]")]
        [Description("Time since last communication in seconds")]
        public  byte tslc;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=19)]
    ///<summary> Information about a potential collision </summary>
    public struct mavlink_collision_t
    {
        /// <summary>Unique identifier, domain based on src field   </summary>
        [Units("")]
        [Description("Unique identifier, domain based on src field")]
        public  uint id;
            /// <summary>Estimated time until collision occurs  [s] </summary>
        [Units("[s]")]
        [Description("Estimated time until collision occurs")]
        public  float time_to_minimum_delta;
            /// <summary>Closest vertical distance between vehicle and object  [m] </summary>
        [Units("[m]")]
        [Description("Closest vertical distance between vehicle and object")]
        public  float altitude_minimum_delta;
            /// <summary>Closest horizontal distance between vehicle and object  [m] </summary>
        [Units("[m]")]
        [Description("Closest horizontal distance between vehicle and object")]
        public  float horizontal_minimum_delta;
            /// <summary>Collision data source MAV_COLLISION_SRC  </summary>
        [Units("")]
        [Description("Collision data source")]
        public  /*MAV_COLLISION_SRC*/byte src;
            /// <summary>Action that is being taken to avoid this collision MAV_COLLISION_ACTION  </summary>
        [Units("")]
        [Description("Action that is being taken to avoid this collision")]
        public  /*MAV_COLLISION_ACTION*/byte action;
            /// <summary>How concerned the aircraft is about this collision MAV_COLLISION_THREAT_LEVEL  </summary>
        [Units("")]
        [Description("How concerned the aircraft is about this collision")]
        public  /*MAV_COLLISION_THREAT_LEVEL*/byte threat_level;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=254)]
    ///<summary> Message implementing parts of the V2 payload specs in V1 frames for transitional support. </summary>
    public struct mavlink_v2_extension_t
    {
        /// <summary>A code that identifies the software component that understands this message (analogous to USB device classes or mime type strings).  If this code is less than 32768, it is considered a 'registered' protocol extension and the corresponding entry should be added to https://github.com/mavlink/mavlink/extension-message-ids.xml.  Software creators can register blocks of message IDs as needed (useful for GCS specific metadata, etc...). Message_types greater than 32767 are considered local experiments and should not be checked in to any widely distributed codebase.   </summary>
        [Units("")]
        [Description("A code that identifies the software component that understands this message (analogous to USB device classes or mime type strings).  If this code is less than 32768, it is considered a 'registered' protocol extension and the corresponding entry should be added to https://github.com/mavlink/mavlink/extension-message-ids.xml.  Software creators can register blocks of message IDs as needed (useful for GCS specific metadata, etc...). Message_types greater than 32767 are considered local experiments and should not be checked in to any widely distributed codebase.")]
        public  ushort message_type;
            /// <summary>Network ID (0 for broadcast)   </summary>
        [Units("")]
        [Description("Network ID (0 for broadcast)")]
        public  byte target_network;
            /// <summary>System ID (0 for broadcast)   </summary>
        [Units("")]
        [Description("System ID (0 for broadcast)")]
        public  byte target_system;
            /// <summary>Component ID (0 for broadcast)   </summary>
        [Units("")]
        [Description("Component ID (0 for broadcast)")]
        public  byte target_component;
            /// <summary>Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification.   </summary>
        [Units("")]
        [Description("Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=249)]
		public byte[] payload;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=36)]
    ///<summary> Send raw controller memory. The use of this message is discouraged for normal packets, but a quite efficient way for testing new messages and getting experimental debug output. </summary>
    public struct mavlink_memory_vect_t
    {
        /// <summary>Starting address of the debug variables   </summary>
        [Units("")]
        [Description("Starting address of the debug variables")]
        public  ushort address;
            /// <summary>Version code of the type variable. 0=unknown, type ignored and assumed int16_t. 1=as below   </summary>
        [Units("")]
        [Description("Version code of the type variable. 0=unknown, type ignored and assumed int16_t. 1=as below")]
        public  byte ver;
            /// <summary>Type code of the memory variables. for ver = 1: 0=16 x int16_t, 1=16 x uint16_t, 2=16 x Q15, 3=16 x 1Q14   </summary>
        [Units("")]
        [Description("Type code of the memory variables. for ver = 1: 0=16 x int16_t, 1=16 x uint16_t, 2=16 x Q15, 3=16 x 1Q14")]
        public  byte type;
            /// <summary>Memory contents at specified address   </summary>
        [Units("")]
        [Description("Memory contents at specified address")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] value;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=30)]
    ///<summary> To debug something using a named 3D vector. </summary>
    public struct mavlink_debug_vect_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>x   </summary>
        [Units("")]
        [Description("x")]
        public  float x;
            /// <summary>y   </summary>
        [Units("")]
        [Description("y")]
        public  float y;
            /// <summary>z   </summary>
        [Units("")]
        [Description("z")]
        public  float z;
            /// <summary>Name   </summary>
        [Units("")]
        [Description("Name")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    ///<summary> Send a key-value pair as float. The use of this message is discouraged for normal packets, but a quite efficient way for testing new messages and getting experimental debug output. </summary>
    public struct mavlink_named_value_float_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Floating point value   </summary>
        [Units("")]
        [Description("Floating point value")]
        public  float value;
            /// <summary>Name of the debug variable   </summary>
        [Units("")]
        [Description("Name of the debug variable")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    ///<summary> Send a key-value pair as integer. The use of this message is discouraged for normal packets, but a quite efficient way for testing new messages and getting experimental debug output. </summary>
    public struct mavlink_named_value_int_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Signed integer value   </summary>
        [Units("")]
        [Description("Signed integer value")]
        public  int value;
            /// <summary>Name of the debug variable   </summary>
        [Units("")]
        [Description("Name of the debug variable")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=51)]
    ///<summary> Status text message. These messages are printed in yellow in the COMM console of QGroundControl. WARNING: They consume quite some bandwidth, so use only for important status and error messages. If implemented wisely, these messages are buffered on the MCU and sent only at a limited rate (e.g. 10 Hz). </summary>
    public struct mavlink_statustext_t
    {
        /// <summary>Severity of status. Relies on the definitions within RFC-5424. MAV_SEVERITY  </summary>
        [Units("")]
        [Description("Severity of status. Relies on the definitions within RFC-5424.")]
        public  /*MAV_SEVERITY*/byte severity;
            /// <summary>Status text message, without null termination character   </summary>
        [Units("")]
        [Description("Status text message, without null termination character")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=50)]
		public byte[] text;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> Send a debug value. The index is used to discriminate between values. These values show up in the plot of QGroundControl as DEBUG N. </summary>
    public struct mavlink_debug_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>DEBUG value   </summary>
        [Units("")]
        [Description("DEBUG value")]
        public  float value;
            /// <summary>index of debug variable   </summary>
        [Units("")]
        [Description("index of debug variable")]
        public  byte ind;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=42)]
    ///<summary> Setup a MAVLink2 signing key. If called with secret_key of all zero and zero initial_timestamp will disable signing </summary>
    public struct mavlink_setup_signing_t
    {
        /// <summary>initial timestamp   </summary>
        [Units("")]
        [Description("initial timestamp")]
        public  ulong initial_timestamp;
            /// <summary>system id of the target   </summary>
        [Units("")]
        [Description("system id of the target")]
        public  byte target_system;
            /// <summary>component ID of the target   </summary>
        [Units("")]
        [Description("component ID of the target")]
        public  byte target_component;
            /// <summary>signing key   </summary>
        [Units("")]
        [Description("signing key")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] secret_key;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=9)]
    ///<summary> Report button state change. </summary>
    public struct mavlink_button_change_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Time of last change of button state.  [ms] </summary>
        [Units("[ms]")]
        [Description("Time of last change of button state.")]
        public  uint last_change_ms;
            /// <summary>Bitmap for state of buttons.   bitmask</summary>
        [Units("")]
        [Description("Bitmap for state of buttons.")]
        public  byte state;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=232)]
    ///<summary> Control vehicle tone generation (buzzer) </summary>
    public struct mavlink_play_tune_t
    {
        /// <summary>System ID   </summary>
        [Units("")]
        [Description("System ID")]
        public  byte target_system;
            /// <summary>Component ID   </summary>
        [Units("")]
        [Description("Component ID")]
        public  byte target_component;
            /// <summary>tune in board specific format   </summary>
        [Units("")]
        [Description("tune in board specific format")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=30)]
		public byte[] tune;
            /// <summary>tune extension (appended to tune)   </summary>
        [Units("")]
        [Description("tune extension (appended to tune)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=200)]
		public byte[] tune2;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=235)]
    ///<summary> Information about a camera </summary>
    public struct mavlink_camera_information_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Version of the camera firmware (v << 24 & 0xff = Dev, v << 16 & 0xff = Patch, v << 8 & 0xff = Minor, v & 0xff = Major)   </summary>
        [Units("")]
        [Description("Version of the camera firmware (v << 24 & 0xff = Dev, v << 16 & 0xff = Patch, v << 8 & 0xff = Minor, v & 0xff = Major)")]
        public  uint firmware_version;
            /// <summary>Focal length  [mm] </summary>
        [Units("[mm]")]
        [Description("Focal length")]
        public  float focal_length;
            /// <summary>Image sensor size horizontal  [mm] </summary>
        [Units("[mm]")]
        [Description("Image sensor size horizontal")]
        public  float sensor_size_h;
            /// <summary>Image sensor size vertical  [mm] </summary>
        [Units("[mm]")]
        [Description("Image sensor size vertical")]
        public  float sensor_size_v;
            /// <summary>Bitmap of camera capability flags. CAMERA_CAP_FLAGS  </summary>
        [Units("")]
        [Description("Bitmap of camera capability flags.")]
        public  /*CAMERA_CAP_FLAGS*/uint flags;
            /// <summary>Horizontal image resolution  [pix] </summary>
        [Units("[pix]")]
        [Description("Horizontal image resolution")]
        public  ushort resolution_h;
            /// <summary>Vertical image resolution  [pix] </summary>
        [Units("[pix]")]
        [Description("Vertical image resolution")]
        public  ushort resolution_v;
            /// <summary>Camera definition version (iteration)   </summary>
        [Units("")]
        [Description("Camera definition version (iteration)")]
        public  ushort cam_definition_version;
            /// <summary>Name of the camera vendor   </summary>
        [Units("")]
        [Description("Name of the camera vendor")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] vendor_name;
            /// <summary>Name of the camera model   </summary>
        [Units("")]
        [Description("Name of the camera model")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] model_name;
            /// <summary>Reserved for a lens ID   </summary>
        [Units("")]
        [Description("Reserved for a lens ID")]
        public  byte lens_id;
            /// <summary>Camera definition URI (if any, otherwise only basic functions will be available).   </summary>
        [Units("")]
        [Description("Camera definition URI (if any, otherwise only basic functions will be available).")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=140)]
		public byte[] cam_definition_uri;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=13)]
    ///<summary> Settings of a camera, can be requested using MAV_CMD_REQUEST_CAMERA_SETTINGS. </summary>
    public struct mavlink_camera_settings_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Camera mode CAMERA_MODE  </summary>
        [Units("")]
        [Description("Camera mode")]
        public  /*CAMERA_MODE*/byte mode_id;
            /// <summary>Current zoom level (0.0 to 100.0, NaN if not known)   </summary>
        [Units("")]
        [Description("Current zoom level (0.0 to 100.0, NaN if not known)")]
        public  float zoomLevel;
            /// <summary>Current focus level (0.0 to 100.0, NaN if not known)   </summary>
        [Units("")]
        [Description("Current focus level (0.0 to 100.0, NaN if not known)")]
        public  float focusLevel;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=27)]
    ///<summary> Information about a storage medium. </summary>
    public struct mavlink_storage_information_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Total capacity.  [MiB] </summary>
        [Units("[MiB]")]
        [Description("Total capacity.")]
        public  float total_capacity;
            /// <summary>Used capacity.  [MiB] </summary>
        [Units("[MiB]")]
        [Description("Used capacity.")]
        public  float used_capacity;
            /// <summary>Available storage capacity.  [MiB] </summary>
        [Units("[MiB]")]
        [Description("Available storage capacity.")]
        public  float available_capacity;
            /// <summary>Read speed.  [MiB/s] </summary>
        [Units("[MiB/s]")]
        [Description("Read speed.")]
        public  float read_speed;
            /// <summary>Write speed.  [MiB/s] </summary>
        [Units("[MiB/s]")]
        [Description("Write speed.")]
        public  float write_speed;
            /// <summary>Storage ID (1 for first, 2 for second, etc.)   </summary>
        [Units("")]
        [Description("Storage ID (1 for first, 2 for second, etc.)")]
        public  byte storage_id;
            /// <summary>Number of storage devices   </summary>
        [Units("")]
        [Description("Number of storage devices")]
        public  byte storage_count;
            /// <summary>Status of storage (0 not available, 1 unformatted, 2 formatted)   </summary>
        [Units("")]
        [Description("Status of storage (0 not available, 1 unformatted, 2 formatted)")]
        public  byte status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=18)]
    ///<summary> Information about the status of a capture. </summary>
    public struct mavlink_camera_capture_status_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Image capture interval  [s] </summary>
        [Units("[s]")]
        [Description("Image capture interval")]
        public  float image_interval;
            /// <summary>Time since recording started  [ms] </summary>
        [Units("[ms]")]
        [Description("Time since recording started")]
        public  uint recording_time_ms;
            /// <summary>Available storage capacity.  [MiB] </summary>
        [Units("[MiB]")]
        [Description("Available storage capacity.")]
        public  float available_capacity;
            /// <summary>Current status of image capturing (0: idle, 1: capture in progress, 2: interval set but idle, 3: interval set and capture in progress)   </summary>
        [Units("")]
        [Description("Current status of image capturing (0: idle, 1: capture in progress, 2: interval set but idle, 3: interval set and capture in progress)")]
        public  byte image_status;
            /// <summary>Current status of video capturing (0: idle, 1: capture in progress)   </summary>
        [Units("")]
        [Description("Current status of video capturing (0: idle, 1: capture in progress)")]
        public  byte video_status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    ///<summary> Information about a captured image </summary>
    public struct mavlink_camera_image_captured_t
    {
        /// <summary>Timestamp (time since UNIX epoch) in UTC. 0 for unknown.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (time since UNIX epoch) in UTC. 0 for unknown.")]
        public  ulong time_utc;
            /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Latitude where image was taken  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude where image was taken")]
        public  int lat;
            /// <summary>Longitude where capture was taken  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude where capture was taken")]
        public  int lon;
            /// <summary>Altitude (MSL) where image was taken  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (MSL) where image was taken")]
        public  int alt;
            /// <summary>Altitude above ground  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude above ground")]
        public  int relative_alt;
            /// <summary>Quaternion of camera orientation (w, x, y, z order, zero-rotation is 0, 0, 0, 0)   </summary>
        [Units("")]
        [Description("Quaternion of camera orientation (w, x, y, z order, zero-rotation is 0, 0, 0, 0)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>Zero based index of this image (image count since armed -1)   </summary>
        [Units("")]
        [Description("Zero based index of this image (image count since armed -1)")]
        public  int image_index;
            /// <summary>Camera ID (1 for first, 2 for second, etc.)   </summary>
        [Units("")]
        [Description("Camera ID (1 for first, 2 for second, etc.)")]
        public  byte camera_id;
            /// <summary>Boolean indicating success (1) or failure (0) while capturing this image.   </summary>
        [Units("")]
        [Description("Boolean indicating success (1) or failure (0) while capturing this image.")]
        public  byte capture_result;
            /// <summary>URL of image taken. Either local storage or http://foo.jpg if camera provides an HTTP interface.   </summary>
        [Units("")]
        [Description("URL of image taken. Either local storage or http://foo.jpg if camera provides an HTTP interface.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=205)]
		public byte[] file_url;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=28)]
    ///<summary> Information about flight since last arming. </summary>
    public struct mavlink_flight_information_t
    {
        /// <summary>Timestamp at arming (time since UNIX epoch) in UTC, 0 for unknown  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp at arming (time since UNIX epoch) in UTC, 0 for unknown")]
        public  ulong arming_time_utc;
            /// <summary>Timestamp at takeoff (time since UNIX epoch) in UTC, 0 for unknown  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp at takeoff (time since UNIX epoch) in UTC, 0 for unknown")]
        public  ulong takeoff_time_utc;
            /// <summary>Universally unique identifier (UUID) of flight, should correspond to name of log files   </summary>
        [Units("")]
        [Description("Universally unique identifier (UUID) of flight, should correspond to name of log files")]
        public  ulong flight_uuid;
            /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Orientation of a mount </summary>
    public struct mavlink_mount_orientation_t
    {
        /// <summary>Timestamp (time since system boot).  [ms] </summary>
        [Units("[ms]")]
        [Description("Timestamp (time since system boot).")]
        public  uint time_boot_ms;
            /// <summary>Roll in global frame (set to NaN for invalid).  [deg] </summary>
        [Units("[deg]")]
        [Description("Roll in global frame (set to NaN for invalid).")]
        public  float roll;
            /// <summary>Pitch in global frame (set to NaN for invalid).  [deg] </summary>
        [Units("[deg]")]
        [Description("Pitch in global frame (set to NaN for invalid).")]
        public  float pitch;
            /// <summary>Yaw relative to vehicle(set to NaN for invalid).  [deg] </summary>
        [Units("[deg]")]
        [Description("Yaw relative to vehicle(set to NaN for invalid).")]
        public  float yaw;
            /// <summary>Yaw in absolute frame, North is 0 (set to NaN for invalid).  [deg] </summary>
        [Units("[deg]")]
        [Description("Yaw in absolute frame, North is 0 (set to NaN for invalid).")]
        public  float yaw_absolute;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    ///<summary> A message containing logged data (see also MAV_CMD_LOGGING_START) </summary>
    public struct mavlink_logging_data_t
    {
        /// <summary>sequence number (can wrap)   </summary>
        [Units("")]
        [Description("sequence number (can wrap)")]
        public  ushort sequence;
            /// <summary>system ID of the target   </summary>
        [Units("")]
        [Description("system ID of the target")]
        public  byte target_system;
            /// <summary>component ID of the target   </summary>
        [Units("")]
        [Description("component ID of the target")]
        public  byte target_component;
            /// <summary>data length  [bytes] </summary>
        [Units("[bytes]")]
        [Description("data length")]
        public  byte length;
            /// <summary>offset into data where first message starts. This can be used for recovery, when a previous message got lost (set to 255 if no start exists).  [bytes] </summary>
        [Units("[bytes]")]
        [Description("offset into data where first message starts. This can be used for recovery, when a previous message got lost (set to 255 if no start exists).")]
        public  byte first_message_offset;
            /// <summary>logged data   </summary>
        [Units("")]
        [Description("logged data")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=249)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    ///<summary> A message containing logged data which requires a LOGGING_ACK to be sent back </summary>
    public struct mavlink_logging_data_acked_t
    {
        /// <summary>sequence number (can wrap)   </summary>
        [Units("")]
        [Description("sequence number (can wrap)")]
        public  ushort sequence;
            /// <summary>system ID of the target   </summary>
        [Units("")]
        [Description("system ID of the target")]
        public  byte target_system;
            /// <summary>component ID of the target   </summary>
        [Units("")]
        [Description("component ID of the target")]
        public  byte target_component;
            /// <summary>data length  [bytes] </summary>
        [Units("[bytes]")]
        [Description("data length")]
        public  byte length;
            /// <summary>offset into data where first message starts. This can be used for recovery, when a previous message got lost (set to 255 if no start exists).  [bytes] </summary>
        [Units("[bytes]")]
        [Description("offset into data where first message starts. This can be used for recovery, when a previous message got lost (set to 255 if no start exists).")]
        public  byte first_message_offset;
            /// <summary>logged data   </summary>
        [Units("")]
        [Description("logged data")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=249)]
		public byte[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=4)]
    ///<summary> An ack for a LOGGING_DATA_ACKED message </summary>
    public struct mavlink_logging_ack_t
    {
        /// <summary>sequence number (must match the one in LOGGING_DATA_ACKED)   </summary>
        [Units("")]
        [Description("sequence number (must match the one in LOGGING_DATA_ACKED)")]
        public  ushort sequence;
            /// <summary>system ID of the target   </summary>
        [Units("")]
        [Description("system ID of the target")]
        public  byte target_system;
            /// <summary>component ID of the target   </summary>
        [Units("")]
        [Description("component ID of the target")]
        public  byte target_component;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=96)]
    ///<summary> Configure AP SSID and Password. </summary>
    public struct mavlink_wifi_config_ap_t
    {
        /// <summary>Name of Wi-Fi network (SSID). Leave it blank to leave it unchanged.   </summary>
        [Units("")]
        [Description("Name of Wi-Fi network (SSID). Leave it blank to leave it unchanged.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
		public byte[] ssid;
            /// <summary>Password. Leave it blank for an open AP.   </summary>
        [Units("")]
        [Description("Password. Leave it blank for an open AP.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=64)]
		public byte[] password;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=17)]
    ///<summary> General status information of an UAVCAN node. Please refer to the definition of the UAVCAN message 'uavcan.protocol.NodeStatus' for the background information. The UAVCAN specification is available at http://uavcan.org. </summary>
    public struct mavlink_uavcan_node_status_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Time since the start-up of the node.  [s] </summary>
        [Units("[s]")]
        [Description("Time since the start-up of the node.")]
        public  uint uptime_sec;
            /// <summary>Vendor-specific status information.   </summary>
        [Units("")]
        [Description("Vendor-specific status information.")]
        public  ushort vendor_specific_status_code;
            /// <summary>Generalized node health status. UAVCAN_NODE_HEALTH  </summary>
        [Units("")]
        [Description("Generalized node health status.")]
        public  /*UAVCAN_NODE_HEALTH*/byte health;
            /// <summary>Generalized operating mode. UAVCAN_NODE_MODE  </summary>
        [Units("")]
        [Description("Generalized operating mode.")]
        public  /*UAVCAN_NODE_MODE*/byte mode;
            /// <summary>Not used currently.   </summary>
        [Units("")]
        [Description("Not used currently.")]
        public  byte sub_mode;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=116)]
    ///<summary> General information describing a particular UAVCAN node. Please refer to the definition of the UAVCAN service 'uavcan.protocol.GetNodeInfo' for the background information. This message should be emitted by the system whenever a new node appears online, or an existing node reboots. Additionally, it can be emitted upon request from the other end of the MAVLink channel (see MAV_CMD_UAVCAN_GET_NODE_INFO). It is also not prohibited to emit this message unconditionally at a low frequency. The UAVCAN specification is available at http://uavcan.org. </summary>
    public struct mavlink_uavcan_node_info_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Time since the start-up of the node.  [s] </summary>
        [Units("[s]")]
        [Description("Time since the start-up of the node.")]
        public  uint uptime_sec;
            /// <summary>Version control system (VCS) revision identifier (e.g. git short commit hash). Zero if unknown.   </summary>
        [Units("")]
        [Description("Version control system (VCS) revision identifier (e.g. git short commit hash). Zero if unknown.")]
        public  uint sw_vcs_commit;
            /// <summary>Node name string. For example, 'sapog.px4.io'.   </summary>
        [Units("")]
        [Description("Node name string. For example, 'sapog.px4.io'.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=80)]
		public byte[] name;
            /// <summary>Hardware major version number.   </summary>
        [Units("")]
        [Description("Hardware major version number.")]
        public  byte hw_version_major;
            /// <summary>Hardware minor version number.   </summary>
        [Units("")]
        [Description("Hardware minor version number.")]
        public  byte hw_version_minor;
            /// <summary>Hardware unique 128-bit ID.   </summary>
        [Units("")]
        [Description("Hardware unique 128-bit ID.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public byte[] hw_unique_id;
            /// <summary>Software major version number.   </summary>
        [Units("")]
        [Description("Software major version number.")]
        public  byte sw_version_major;
            /// <summary>Software minor version number.   </summary>
        [Units("")]
        [Description("Software minor version number.")]
        public  byte sw_version_minor;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=158)]
    ///<summary> Obstacle distances in front of the sensor, starting from the left in increment degrees to the right </summary>
    public struct mavlink_obstacle_distance_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Distance of obstacles around the UAV with index 0 corresponding to local North. A value of 0 means that the obstacle is right in front of the sensor. A value of max_distance +1 means no obstacle is present. A value of UINT16_MAX for unknown/not used. In a array element, one unit corresponds to 1cm.  [cm] </summary>
        [Units("[cm]")]
        [Description("Distance of obstacles around the UAV with index 0 corresponding to local North. A value of 0 means that the obstacle is right in front of the sensor. A value of max_distance +1 means no obstacle is present. A value of UINT16_MAX for unknown/not used. In a array element, one unit corresponds to 1cm.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=72)]
		public UInt16[] distances;
            /// <summary>Minimum distance the sensor can measure.  [cm] </summary>
        [Units("[cm]")]
        [Description("Minimum distance the sensor can measure.")]
        public  ushort min_distance;
            /// <summary>Maximum distance the sensor can measure.  [cm] </summary>
        [Units("[cm]")]
        [Description("Maximum distance the sensor can measure.")]
        public  ushort max_distance;
            /// <summary>Class id of the distance sensor type. MAV_DISTANCE_SENSOR  </summary>
        [Units("")]
        [Description("Class id of the distance sensor type.")]
        public  /*MAV_DISTANCE_SENSOR*/byte sensor_type;
            /// <summary>Angular width in degrees of each array element.  [deg] </summary>
        [Units("[deg]")]
        [Description("Angular width in degrees of each array element.")]
        public  byte increment;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=230)]
    ///<summary> Odometry message to communicate odometry information with an external interface. Fits ROS REP 147 standard for aerial vehicles (http://www.ros.org/reps/rep-0147.html). </summary>
    public struct mavlink_odometry_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>X Position  [m] </summary>
        [Units("[m]")]
        [Description("X Position")]
        public  float x;
            /// <summary>Y Position  [m] </summary>
        [Units("[m]")]
        [Description("Y Position")]
        public  float y;
            /// <summary>Z Position  [m] </summary>
        [Units("[m]")]
        [Description("Z Position")]
        public  float z;
            /// <summary>Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation)   </summary>
        [Units("")]
        [Description("Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] q;
            /// <summary>X linear speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("X linear speed")]
        public  float vx;
            /// <summary>Y linear speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Y linear speed")]
        public  float vy;
            /// <summary>Z linear speed  [m/s] </summary>
        [Units("[m/s]")]
        [Description("Z linear speed")]
        public  float vz;
            /// <summary>Roll angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Roll angular speed")]
        public  float rollspeed;
            /// <summary>Pitch angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Pitch angular speed")]
        public  float pitchspeed;
            /// <summary>Yaw angular speed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Yaw angular speed")]
        public  float yawspeed;
            /// <summary>Row-major representation of a 6x6 pose cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of a 6x6 pose cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] pose_covariance;
            /// <summary>Row-major representation of a 6x6 velocity cross-covariance matrix upper right triangle (states: vx, vy, vz, rollspeed, pitchspeed, yawspeed; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.   </summary>
        [Units("")]
        [Description("Row-major representation of a 6x6 velocity cross-covariance matrix upper right triangle (states: vx, vy, vz, rollspeed, pitchspeed, yawspeed; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=21)]
		public float[] velocity_covariance;
            /// <summary>Coordinate frame of reference for the pose data. MAV_FRAME  </summary>
        [Units("")]
        [Description("Coordinate frame of reference for the pose data.")]
        public  /*MAV_FRAME*/byte frame_id;
            /// <summary>Coordinate frame of reference for the velocity in free space (twist) data. MAV_FRAME  </summary>
        [Units("")]
        [Description("Coordinate frame of reference for the velocity in free space (twist) data.")]
        public  /*MAV_FRAME*/byte child_frame_id;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=252)]
    ///<summary> Large debug/prototyping array. The message uses the maximum available payload for data. The array_id and name fields are used to discriminate between messages in code and in user interfaces (respectively). Do not use in production code. </summary>
    public struct mavlink_debug_float_array_t
    {
        /// <summary>Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
        public  ulong time_usec;
            /// <summary>Unique ID used to discriminate between arrays   </summary>
        [Units("")]
        [Description("Unique ID used to discriminate between arrays")]
        public  ushort array_id;
            /// <summary>Name, for human-friendly display in a Ground Control Station   </summary>
        [Units("")]
        [Description("Name, for human-friendly display in a Ground Control Station")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public byte[] name;
            /// <summary>data   </summary>
        [Units("")]
        [Description("data")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=58)]
		public float[] data;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=255)]
    ///<summary> Status text message (use only for important status and error messages). The full message payload can be used for status text, but we recommend that updates be kept concise. Note: The message is intended as a less restrictive replacement for STATUSTEXT. </summary>
    public struct mavlink_statustext_long_t
    {
        /// <summary>Severity of status. Relies on the definitions within RFC-5424. MAV_SEVERITY  </summary>
        [Units("")]
        [Description("Severity of status. Relies on the definitions within RFC-5424.")]
        public  /*MAV_SEVERITY*/byte severity;
            /// <summary>Status text message, without null termination character.   </summary>
        [Units("")]
        [Description("Status text message, without null termination character.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=254)]
		public byte[] text;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=137)]
    ///<summary> Cumulative distance traveled for each reported wheel. </summary>
    public struct mavlink_wheel_distance_t
    {
        /// <summary>Timestamp (synced to UNIX time or since system boot).  [us] </summary>
        [Units("[us]")]
        [Description("Timestamp (synced to UNIX time or since system boot).")]
        public  ulong time_usec;
            /// <summary>Distance reported by individual wheel encoders. Forward rotations increase values, reverse rotations decrease them. Not all wheels will necessarily have wheel encoders; the mapping of encoders to wheel positions must be agreed/understood by the endpoints.  [m] </summary>
        [Units("[m]")]
        [Description("Distance reported by individual wheel encoders. Forward rotations increase values, reverse rotations decrease them. Not all wheels will necessarily have wheel encoders; the mapping of encoders to wheel positions must be agreed/understood by the endpoints.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public double distance;
            /// <summary>Number of wheels reported.   </summary>
        [Units("")]
        [Description("Number of wheels reported.")]
        public  byte count;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=20)]
    ///<summary> Static data to configure the ADS-B transponder (send within 10 sec of a POR and every 10 sec thereafter) </summary>
    public struct mavlink_uavionix_adsb_out_cfg_t
    {
        /// <summary>Vehicle address (24 bit)   </summary>
        [Units("")]
        [Description("Vehicle address (24 bit)")]
        public  uint ICAO;
            /// <summary>Aircraft stall speed in cm/s  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("Aircraft stall speed in cm/s")]
        public  ushort stallSpeed;
            /// <summary>Vehicle identifier (8 characters, null terminated, valid characters are A-Z, 0-9, ' ' only)   </summary>
        [Units("")]
        [Description("Vehicle identifier (8 characters, null terminated, valid characters are A-Z, 0-9, ' ' only)")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)]
		public byte[] callsign;
            /// <summary>Transmitting vehicle type. See ADSB_EMITTER_TYPE enum ADSB_EMITTER_TYPE  </summary>
        [Units("")]
        [Description("Transmitting vehicle type. See ADSB_EMITTER_TYPE enum")]
        public  /*ADSB_EMITTER_TYPE*/byte emitterType;
            /// <summary>Aircraft length and width encoding (table 2-35 of DO-282B) UAVIONIX_ADSB_OUT_CFG_AIRCRAFT_SIZE  </summary>
        [Units("")]
        [Description("Aircraft length and width encoding (table 2-35 of DO-282B)")]
        public  /*UAVIONIX_ADSB_OUT_CFG_AIRCRAFT_SIZE*/byte aircraftSize;
            /// <summary>GPS antenna lateral offset (table 2-36 of DO-282B) UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LAT  </summary>
        [Units("")]
        [Description("GPS antenna lateral offset (table 2-36 of DO-282B)")]
        public  /*UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LAT*/byte gpsOffsetLat;
            /// <summary>GPS antenna longitudinal offset from nose [if non-zero, take position (in meters) divide by 2 and add one] (table 2-37 DO-282B) UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LON  </summary>
        [Units("")]
        [Description("GPS antenna longitudinal offset from nose [if non-zero, take position (in meters) divide by 2 and add one] (table 2-37 DO-282B)")]
        public  /*UAVIONIX_ADSB_OUT_CFG_GPS_OFFSET_LON*/byte gpsOffsetLon;
            /// <summary>ADS-B transponder reciever and transmit enable flags UAVIONIX_ADSB_OUT_RF_SELECT  bitmask</summary>
        [Units("")]
        [Description("ADS-B transponder reciever and transmit enable flags")]
        public  /*UAVIONIX_ADSB_OUT_RF_SELECT*/byte rfSelect;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=41)]
    ///<summary> Dynamic data used to generate ADS-B out transponder data (send at 5Hz) </summary>
    public struct mavlink_uavionix_adsb_out_dynamic_t
    {
        /// <summary>UTC time in seconds since GPS epoch (Jan 6, 1980). If unknown set to UINT32_MAX  [s] </summary>
        [Units("[s]")]
        [Description("UTC time in seconds since GPS epoch (Jan 6, 1980). If unknown set to UINT32_MAX")]
        public  uint utcTime;
            /// <summary>Latitude WGS84 (deg * 1E7). If unknown set to INT32_MAX  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Latitude WGS84 (deg * 1E7). If unknown set to INT32_MAX")]
        public  int gpsLat;
            /// <summary>Longitude WGS84 (deg * 1E7). If unknown set to INT32_MAX  [degE7] </summary>
        [Units("[degE7]")]
        [Description("Longitude WGS84 (deg * 1E7). If unknown set to INT32_MAX")]
        public  int gpsLon;
            /// <summary>Altitude (WGS84). UP +ve. If unknown set to INT32_MAX  [mm] </summary>
        [Units("[mm]")]
        [Description("Altitude (WGS84). UP +ve. If unknown set to INT32_MAX")]
        public  int gpsAlt;
            /// <summary>Barometric pressure altitude (MSL) relative to a standard atmosphere of 1013.2 mBar and NOT bar corrected altitude (m * 1E-3). (up +ve). If unknown set to INT32_MAX  [mbar] </summary>
        [Units("[mbar]")]
        [Description("Barometric pressure altitude (MSL) relative to a standard atmosphere of 1013.2 mBar and NOT bar corrected altitude (m * 1E-3). (up +ve). If unknown set to INT32_MAX")]
        public  int baroAltMSL;
            /// <summary>Horizontal accuracy in mm (m * 1E-3). If unknown set to UINT32_MAX  [mm] </summary>
        [Units("[mm]")]
        [Description("Horizontal accuracy in mm (m * 1E-3). If unknown set to UINT32_MAX")]
        public  uint accuracyHor;
            /// <summary>Vertical accuracy in cm. If unknown set to UINT16_MAX  [cm] </summary>
        [Units("[cm]")]
        [Description("Vertical accuracy in cm. If unknown set to UINT16_MAX")]
        public  ushort accuracyVert;
            /// <summary>Velocity accuracy in mm/s (m * 1E-3). If unknown set to UINT16_MAX  [mm/s] </summary>
        [Units("[mm/s]")]
        [Description("Velocity accuracy in mm/s (m * 1E-3). If unknown set to UINT16_MAX")]
        public  ushort accuracyVel;
            /// <summary>GPS vertical speed in cm/s. If unknown set to INT16_MAX  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("GPS vertical speed in cm/s. If unknown set to INT16_MAX")]
        public  short velVert;
            /// <summary>North-South velocity over ground in cm/s North +ve. If unknown set to INT16_MAX  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("North-South velocity over ground in cm/s North +ve. If unknown set to INT16_MAX")]
        public  short velNS;
            /// <summary>East-West velocity over ground in cm/s East +ve. If unknown set to INT16_MAX  [cm/s] </summary>
        [Units("[cm/s]")]
        [Description("East-West velocity over ground in cm/s East +ve. If unknown set to INT16_MAX")]
        public  short VelEW;
            /// <summary>ADS-B transponder dynamic input state flags UAVIONIX_ADSB_OUT_DYNAMIC_STATE  bitmask</summary>
        [Units("")]
        [Description("ADS-B transponder dynamic input state flags")]
        public  /*UAVIONIX_ADSB_OUT_DYNAMIC_STATE*/ushort state;
            /// <summary>Mode A code (typically 1200 [0x04B0] for VFR)   </summary>
        [Units("")]
        [Description("Mode A code (typically 1200 [0x04B0] for VFR)")]
        public  ushort squawk;
            /// <summary>0-1: no fix, 2: 2D fix, 3: 3D fix, 4: DGPS, 5: RTK UAVIONIX_ADSB_OUT_DYNAMIC_GPS_FIX  </summary>
        [Units("")]
        [Description("0-1: no fix, 2: 2D fix, 3: 3D fix, 4: DGPS, 5: RTK")]
        public  /*UAVIONIX_ADSB_OUT_DYNAMIC_GPS_FIX*/byte gpsFix;
            /// <summary>Number of satellites visible. If unknown set to UINT8_MAX   </summary>
        [Units("")]
        [Description("Number of satellites visible. If unknown set to UINT8_MAX")]
        public  byte numSats;
            /// <summary>Emergency status UAVIONIX_ADSB_EMERGENCY_STATUS  </summary>
        [Units("")]
        [Description("Emergency status")]
        public  /*UAVIONIX_ADSB_EMERGENCY_STATUS*/byte emergencyStatus;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=1)]
    ///<summary> Transceiver heartbeat with health report (updated every 10s) </summary>
    public struct mavlink_uavionix_adsb_transceiver_health_report_t
    {
        /// <summary>ADS-B transponder messages UAVIONIX_ADSB_RF_HEALTH  bitmask</summary>
        [Units("")]
        [Description("ADS-B transponder messages")]
        public  /*UAVIONIX_ADSB_RF_HEALTH*/byte rfHealth;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=1)]
    ///<summary> ICAROUS heartbeat </summary>
    public struct mavlink_icarous_heartbeat_t
    {
        /// <summary>See the FMS_STATE enum. ICAROUS_FMS_STATE  </summary>
        [Units("")]
        [Description("See the FMS_STATE enum.")]
        public  /*ICAROUS_FMS_STATE*/byte status;
    
    };


    [StructLayout(LayoutKind.Sequential,Pack=1,Size=46)]
    ///<summary> Kinematic multi bands (track) output from Daidalus </summary>
    public struct mavlink_icarous_kinematic_bands_t
    {
        /// <summary>min angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("min angle (degrees)")]
        public  float min1;
            /// <summary>max angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("max angle (degrees)")]
        public  float max1;
            /// <summary>min angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("min angle (degrees)")]
        public  float min2;
            /// <summary>max angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("max angle (degrees)")]
        public  float max2;
            /// <summary>min angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("min angle (degrees)")]
        public  float min3;
            /// <summary>max angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("max angle (degrees)")]
        public  float max3;
            /// <summary>min angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("min angle (degrees)")]
        public  float min4;
            /// <summary>max angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("max angle (degrees)")]
        public  float max4;
            /// <summary>min angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("min angle (degrees)")]
        public  float min5;
            /// <summary>max angle (degrees)  [deg] </summary>
        [Units("[deg]")]
        [Description("max angle (degrees)")]
        public  float max5;
            /// <summary>Number of track bands   </summary>
        [Units("")]
        [Description("Number of track bands")]
        public  byte numBands;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        [Units("")]
        [Description("See the TRACK_BAND_TYPES enum.")]
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type1;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        [Units("")]
        [Description("See the TRACK_BAND_TYPES enum.")]
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type2;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        [Units("")]
        [Description("See the TRACK_BAND_TYPES enum.")]
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type3;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        [Units("")]
        [Description("See the TRACK_BAND_TYPES enum.")]
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type4;
            /// <summary>See the TRACK_BAND_TYPES enum. ICAROUS_TRACK_BAND_TYPES  </summary>
        [Units("")]
        [Description("See the TRACK_BAND_TYPES enum.")]
        public  /*ICAROUS_TRACK_BAND_TYPES*/byte type5;
    
    };

}
