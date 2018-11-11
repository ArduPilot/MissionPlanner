//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2004  John Champion
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections.Generic;

namespace ZedGraph
{
	/// <summary>
	/// A class representing all the characteristics of the Line
	/// segments that make up a curve on the graph.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.50 $ $Date: 2007-12-30 23:27:39 $ </version>
	[Serializable]
	public class Line : LineBase, ICloneable, ISerializable
	{

	#region Fields

		/// <summary>
		/// Private field that stores the smoothing flag for this
		/// <see cref="Line"/>.  Use the public
		/// property <see cref="IsSmooth"/> to access this value.
		/// </summary>
		private bool _isSmooth;
		/// <summary>
		/// Private field that stores the smoothing tension
		/// for this <see cref="Line"/>.  Use the public property
		/// <see cref="SmoothTension"/> to access this value.
		/// </summary>
		/// <value>A floating point value indicating the level of smoothing.
		/// 0.0F for no smoothing, 1.0F for lots of smoothing, >1.0 for odd
		/// smoothing.</value>
		/// <seealso cref="IsSmooth"/>
		/// <seealso cref="Default.IsSmooth"/>
		/// <seealso cref="Default.SmoothTension"/>
		private float _smoothTension;
		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.StepType"/> for this
		/// <see cref="CurveItem"/>.  Use the public
		/// property <see cref="StepType"/> to access this value.
		/// </summary>
		private StepType _stepType;
		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.Fill"/> data for this
		/// <see cref="Line"/>.  Use the public property <see cref="Fill"/> to
		/// access this value.
		/// </summary>
		private Fill _fill;
		/// <summary>
		/// Private field that determines if this <see cref="Line"/> will be drawn with
		/// optimizations enabled.  Use the public
		/// property <see cref="IsOptimizedDraw"/> to access this value.
		/// </summary>
		private bool _isOptimizedDraw;

        /// <summary>
		/// Private field that holds all unit values for CurveItems that are able to be 
        /// displayed on the Mavlink Graph log page
		/// </summary>
        private Dictionary<string, string> _unitList = new Dictionary<string, string>()
        {
            {"mag_declination sensor_offsets_t", "[rad]"},
            {"freemem meminfo_t", "[bytes]"},
            {"freemem32 meminfo_t", "[bytes]"},
            {"engine_cut_off digicam_configure_t", "[ds]"},
            {"pointing_a mount_status_t", "[cdeg]"},
            {"pointing_b mount_status_t", "[cdeg]"},
            {"pointing_c mount_status_t", "[cdeg]"},
            {"lat fence_point_t", "[deg]"},
            {"lng fence_point_t", "[deg]"},
            {"breach_time fence_status_t", "[ms]"},
            {"omegaIx ahrs_t", "[rad/s]"},
            {"omegaIy ahrs_t", "[rad/s]"},
            {"omegaIz ahrs_t", "[rad/s]"},
            {"roll simstate_t", "[rad]"},
            {"pitch simstate_t", "[rad]"},
            {"yaw simstate_t", "[rad]"},
            {"xacc simstate_t", "[m/s/s]"},
            {"yacc simstate_t", "[m/s/s]"},
            {"zacc simstate_t", "[m/s/s]"},
            {"xgyro simstate_t", "[rad/s]"},
            {"ygyro simstate_t", "[rad/s]"},
            {"zgyro simstate_t", "[rad/s]"},
            {"lat simstate_t", "[degE7]"},
            {"lng simstate_t", "[degE7]"},
            {"Vcc hwstatus_t", "[mV]"},
            {"txbuf radio_t", "[%]"},
            {"last_trigger limits_status_t", "[ms]"},
            {"last_action limits_status_t", "[ms]"},
            {"last_recovery limits_status_t", "[ms]"},
            {"last_clear limits_status_t", "[ms]"},
            {"direction wind_t", "[deg]"},
            {"speed wind_t", "[m/s]"},
            {"speed_z wind_t", "[m/s]"},
            {"len data16_t", "[bytes]"},
            {"len data32_t", "[bytes]"},
            {"len data64_t", "[bytes]"},
            {"len data96_t", "[bytes]"},
            {"distance rangefinder_t", "[m]"},
            {"voltage rangefinder_t", "[V]"},
            {"vx airspeed_autocal_t", "[m/s]"},
            {"vy airspeed_autocal_t", "[m/s]"},
            {"vz airspeed_autocal_t", "[m/s]"},
            {"diff_pressure airspeed_autocal_t", "[Pa]"},
            {"lat rally_point_t", "[degE7]"},
            {"lng rally_point_t", "[degE7]"},
            {"alt rally_point_t", "[m]"},
            {"break_alt rally_point_t", "[m]"},
            {"land_dir rally_point_t", "[cdeg]"},
            {"current compassmot_status_t", "[A]"},
            {"throttle compassmot_status_t", "[d%]"},
            {"interference compassmot_status_t", "[%]"},
            {"roll ahrs2_t", "[rad]"},
            {"pitch ahrs2_t", "[rad]"},
            {"yaw ahrs2_t", "[rad]"},
            {"altitude ahrs2_t", "[m]"},
            {"lat ahrs2_t", "[degE7]"},
            {"lng ahrs2_t", "[degE7]"},
            {"time_usec camera_status_t", "[us]"},
            {"time_usec camera_feedback_t", "[us]"},
            {"lat camera_feedback_t", "[degE7]"},
            {"lng camera_feedback_t", "[degE7]"},
            {"alt_msl camera_feedback_t", "[m]"},
            {"alt_rel camera_feedback_t", "[m]"},
            {"roll camera_feedback_t", "[deg]"},
            {"pitch camera_feedback_t", "[deg]"},
            {"yaw camera_feedback_t", "[deg]"},
            {"foc_len camera_feedback_t", "[mm]"},
            {"voltage battery2_t", "[mV]"},
            {"current_battery battery2_t", "[cA]"},
            {"roll ahrs3_t", "[rad]"},
            {"pitch ahrs3_t", "[rad]"},
            {"yaw ahrs3_t", "[rad]"},
            {"altitude ahrs3_t", "[m]"},
            {"lat ahrs3_t", "[degE7]"},
            {"lng ahrs3_t", "[degE7]"},
            {"completion_pct mag_cal_progress_t", "[%]"},
            {"fitness mag_cal_report_t", "[mgauss]"},
            {"desired pid_tuning_t", "[deg/s]"},
            {"achieved pid_tuning_t", "[deg/s]"},
            {"landing_lat deepstall_t", "[degE7]"},
            {"landing_lon deepstall_t", "[degE7]"},
            {"path_lat deepstall_t", "[degE7]"},
            {"path_lon deepstall_t", "[degE7]"},
            {"arc_entry_lat deepstall_t", "[degE7]"},
            {"arc_entry_lon deepstall_t", "[degE7]"},
            {"altitude deepstall_t", "[m]"},
            {"expected_travel_distance deepstall_t", "[m]"},
            {"cross_track_error deepstall_t", "[m]"},
            {"delta_time gimbal_report_t", "[s]"},
            {"delta_angle_x gimbal_report_t", "[rad]"},
            {"delta_angle_y gimbal_report_t", "[rad]"},
            {"delta_angle_z gimbal_report_t", "[rad]"},
            {"delta_velocity_x gimbal_report_t", "[m/s]"},
            {"delta_velocity_y gimbal_report_t", "[m/s]"},
            {"delta_velocity_z gimbal_report_t", "[m/s]"},
            {"joint_roll gimbal_report_t", "[rad]"},
            {"joint_el gimbal_report_t", "[rad]"},
            {"joint_az gimbal_report_t", "[rad]"},
            {"demanded_rate_x gimbal_control_t", "[rad/s]"},
            {"demanded_rate_y gimbal_control_t", "[rad/s]"},
            {"demanded_rate_z gimbal_control_t", "[rad/s]"},
            {"desired adap_tuning_t", "[deg/s]"},
            {"achieved adap_tuning_t", "[deg/s]"},
            {"time_usec vision_position_delta_t", "[us]"},
            {"time_delta_usec vision_position_delta_t", "[us]"},
            {"position_delta vision_position_delta_t", "[m]"},
            {"confidence vision_position_delta_t", "[%]"},
            {"time_usec aoa_ssa_t", "[us]"},
            {"AOA aoa_ssa_t", "[deg]"},
            {"SSA aoa_ssa_t", "[deg]"},
            {"voltage esc_telemetry_1_to_4_t", "[cV]"},
            {"current esc_telemetry_1_to_4_t", "[cA]"},
            {"totalcurrent esc_telemetry_1_to_4_t", "[mAh]"},
            {"rpm esc_telemetry_1_to_4_t", "[rpm]"},
            {"temperature esc_telemetry_1_to_4_t", "[degC]"},
            {"voltage esc_telemetry_5_to_8_t", "[cV]"},
            {"current esc_telemetry_5_to_8_t", "[cA]"},
            {"totalcurrent esc_telemetry_5_to_8_t", "[mAh]"},
            {"rpm esc_telemetry_5_to_8_t", "[rpm]"},
            {"temperature esc_telemetry_5_to_8_t", "[degC]"},
            {"voltage esc_telemetry_9_to_12_t", "[cV]"},
            {"current esc_telemetry_9_to_12_t", "[cA]"},
            {"totalcurrent esc_telemetry_9_to_12_t", "[mAh]"},
            {"rpm esc_telemetry_9_to_12_t", "[rpm]"},
            {"temperature esc_telemetry_9_to_12_t", "[degC]"},
            {"load sys_status_t", "[d%]"},
            {"voltage_battery sys_status_t", "[mV]"},
            {"current_battery sys_status_t", "[cA]"},
            {"drop_rate_comm sys_status_t", "[c%]"},
            {"battery_remaining sys_status_t", "[%]"},
            {"time_unix_usec system_time_t", "[us]"},
            {"time_boot_ms system_time_t", "[ms]"},
            {"time_usec ping_t", "[us]"},
            {"version change_operator_control_t", "[rad]"},
            {"time_usec gps_raw_int_t", "[us]"},
            {"lat gps_raw_int_t", "[degE7]"},
            {"lon gps_raw_int_t", "[degE7]"},
            {"alt gps_raw_int_t", "[mm]"},
            {"vel gps_raw_int_t", "[cm/s]"},
            {"cog gps_raw_int_t", "[cdeg]"},
            {"alt_ellipsoid gps_raw_int_t", "[mm]"},
            {"h_acc gps_raw_int_t", "[mm]"},
            {"v_acc gps_raw_int_t", "[mm]"},
            {"vel_acc gps_raw_int_t", "[mm]"},
            {"hdg_acc gps_raw_int_t", "[degE5]"},
            {"satellite_elevation gps_status_t", "[deg]"},
            {"satellite_azimuth gps_status_t", "[deg]"},
            {"satellite_snr gps_status_t", "[dB]"},
            {"time_boot_ms scaled_imu_t", "[ms]"},
            {"xacc scaled_imu_t", "[mG]"},
            {"yacc scaled_imu_t", "[mG]"},
            {"zacc scaled_imu_t", "[mG]"},
            {"xgyro scaled_imu_t", "[mrad/s]"},
            {"ygyro scaled_imu_t", "[mrad/s]"},
            {"zgyro scaled_imu_t", "[mrad/s]"},
            {"xmag scaled_imu_t", "[mT]"},
            {"ymag scaled_imu_t", "[mT]"},
            {"zmag scaled_imu_t", "[mT]"},
            {"time_usec raw_imu_t", "[us]"},
            {"time_usec raw_pressure_t", "[us]"},
            {"time_boot_ms scaled_pressure_t", "[ms]"},
            {"press_abs scaled_pressure_t", "[hPa]"},
            {"press_diff scaled_pressure_t", "[hPa]"},
            {"temperature scaled_pressure_t", "[cdegC]"},
            {"time_boot_ms attitude_t", "[ms]"},
            {"roll attitude_t", "[rad]"},
            {"pitch attitude_t", "[rad]"},
            {"yaw attitude_t", "[rad]"},
            {"rollspeed attitude_t", "[rad/s]"},
            {"pitchspeed attitude_t", "[rad/s]"},
            {"yawspeed attitude_t", "[rad/s]"},
            {"time_boot_ms attitude_quaternion_t", "[ms]"},
            {"rollspeed attitude_quaternion_t", "[rad/s]"},
            {"pitchspeed attitude_quaternion_t", "[rad/s]"},
            {"yawspeed attitude_quaternion_t", "[rad/s]"},
            {"time_boot_ms local_position_ned_t", "[ms]"},
            {"x local_position_ned_t", "[m]"},
            {"y local_position_ned_t", "[m]"},
            {"z local_position_ned_t", "[m]"},
            {"vx local_position_ned_t", "[m/s]"},
            {"vy local_position_ned_t", "[m/s]"},
            {"vz local_position_ned_t", "[m/s]"},
            {"time_boot_ms global_position_int_t", "[ms]"},
            {"lat global_position_int_t", "[degE7]"},
            {"lon global_position_int_t", "[degE7]"},
            {"alt global_position_int_t", "[mm]"},
            {"relative_alt global_position_int_t", "[mm]"},
            {"vx global_position_int_t", "[cm/s]"},
            {"vy global_position_int_t", "[cm/s]"},
            {"vz global_position_int_t", "[cm/s]"},
            {"hdg global_position_int_t", "[cdeg]"},
            {"time_boot_ms rc_channels_scaled_t", "[ms]"},
            {"rssi rc_channels_scaled_t", "[%]"},
            {"time_boot_ms rc_channels_raw_t", "[ms]"},
            {"chan1_raw rc_channels_raw_t", "[us]"},
            {"chan2_raw rc_channels_raw_t", "[us]"},
            {"chan3_raw rc_channels_raw_t", "[us]"},
            {"chan4_raw rc_channels_raw_t", "[us]"},
            {"chan5_raw rc_channels_raw_t", "[us]"},
            {"chan6_raw rc_channels_raw_t", "[us]"},
            {"chan7_raw rc_channels_raw_t", "[us]"},
            {"chan8_raw rc_channels_raw_t", "[us]"},
            {"rssi rc_channels_raw_t", "[%]"},
            {"time_usec servo_output_raw_t", "[us]"},
            {"servo1_raw servo_output_raw_t", "[us]"},
            {"servo2_raw servo_output_raw_t", "[us]"},
            {"servo3_raw servo_output_raw_t", "[us]"},
            {"servo4_raw servo_output_raw_t", "[us]"},
            {"servo5_raw servo_output_raw_t", "[us]"},
            {"servo6_raw servo_output_raw_t", "[us]"},
            {"servo7_raw servo_output_raw_t", "[us]"},
            {"servo8_raw servo_output_raw_t", "[us]"},
            {"servo9_raw servo_output_raw_t", "[us]"},
            {"servo10_raw servo_output_raw_t", "[us]"},
            {"servo11_raw servo_output_raw_t", "[us]"},
            {"servo12_raw servo_output_raw_t", "[us]"},
            {"servo13_raw servo_output_raw_t", "[us]"},
            {"servo14_raw servo_output_raw_t", "[us]"},
            {"servo15_raw servo_output_raw_t", "[us]"},
            {"servo16_raw servo_output_raw_t", "[us]"},
            {"latitude set_gps_global_origin_t", "[degE7]"},
            {"longitude set_gps_global_origin_t", "[degE7]"},
            {"altitude set_gps_global_origin_t", "[mm]"},
            {"time_usec set_gps_global_origin_t", "[us]"},
            {"latitude gps_global_origin_t", "[degE7]"},
            {"longitude gps_global_origin_t", "[degE7]"},
            {"altitude gps_global_origin_t", "[mm]"},
            {"time_usec gps_global_origin_t", "[us]"},
            {"scale param_map_rc_t", "[-1,"},
            {"p1x safety_set_allowed_area_t", "[m]"},
            {"p1y safety_set_allowed_area_t", "[m]"},
            {"p1z safety_set_allowed_area_t", "[m]"},
            {"p2x safety_set_allowed_area_t", "[m]"},
            {"p2y safety_set_allowed_area_t", "[m]"},
            {"p2z safety_set_allowed_area_t", "[m]"},
            {"p1x safety_allowed_area_t", "[m]"},
            {"p1y safety_allowed_area_t", "[m]"},
            {"p1z safety_allowed_area_t", "[m]"},
            {"p2x safety_allowed_area_t", "[m]"},
            {"p2y safety_allowed_area_t", "[m]"},
            {"p2z safety_allowed_area_t", "[m]"},
            {"time_usec attitude_quaternion_cov_t", "[us]"},
            {"rollspeed attitude_quaternion_cov_t", "[rad/s]"},
            {"pitchspeed attitude_quaternion_cov_t", "[rad/s]"},
            {"yawspeed attitude_quaternion_cov_t", "[rad/s]"},
            {"nav_roll nav_controller_output_t", "[deg]"},
            {"nav_pitch nav_controller_output_t", "[deg]"},
            {"alt_error nav_controller_output_t", "[m]"},
            {"aspd_error nav_controller_output_t", "[m/s]"},
            {"xtrack_error nav_controller_output_t", "[m]"},
            {"nav_bearing nav_controller_output_t", "[deg]"},
            {"target_bearing nav_controller_output_t", "[deg]"},
            {"wp_dist nav_controller_output_t", "[m]"},
            {"time_usec global_position_int_cov_t", "[us]"},
            {"lat global_position_int_cov_t", "[degE7]"},
            {"lon global_position_int_cov_t", "[degE7]"},
            {"alt global_position_int_cov_t", "[mm]"},
            {"relative_alt global_position_int_cov_t", "[mm]"},
            {"vx global_position_int_cov_t", "[m/s]"},
            {"vy global_position_int_cov_t", "[m/s]"},
            {"vz global_position_int_cov_t", "[m/s]"},
            {"time_usec local_position_ned_cov_t", "[us]"},
            {"x local_position_ned_cov_t", "[m]"},
            {"y local_position_ned_cov_t", "[m]"},
            {"z local_position_ned_cov_t", "[m]"},
            {"vx local_position_ned_cov_t", "[m/s]"},
            {"vy local_position_ned_cov_t", "[m/s]"},
            {"vz local_position_ned_cov_t", "[m/s]"},
            {"ax local_position_ned_cov_t", "[m/s/s]"},
            {"ay local_position_ned_cov_t", "[m/s/s]"},
            {"az local_position_ned_cov_t", "[m/s/s]"},
            {"time_boot_ms rc_channels_t", "[ms]"},
            {"chan1_raw rc_channels_t", "[us]"},
            {"chan2_raw rc_channels_t", "[us]"},
            {"chan3_raw rc_channels_t", "[us]"},
            {"chan4_raw rc_channels_t", "[us]"},
            {"chan5_raw rc_channels_t", "[us]"},
            {"chan6_raw rc_channels_t", "[us]"},
            {"chan7_raw rc_channels_t", "[us]"},
            {"chan8_raw rc_channels_t", "[us]"},
            {"chan9_raw rc_channels_t", "[us]"},
            {"chan10_raw rc_channels_t", "[us]"},
            {"chan11_raw rc_channels_t", "[us]"},
            {"chan12_raw rc_channels_t", "[us]"},
            {"chan13_raw rc_channels_t", "[us]"},
            {"chan14_raw rc_channels_t", "[us]"},
            {"chan15_raw rc_channels_t", "[us]"},
            {"chan16_raw rc_channels_t", "[us]"},
            {"chan17_raw rc_channels_t", "[us]"},
            {"chan18_raw rc_channels_t", "[us]"},
            {"rssi rc_channels_t", "[%]"},
            {"req_message_rate request_data_stream_t", "[Hz]"},
            {"message_rate data_stream_t", "[Hz]"},
            {"x manual_control_t", "[-1000,1000]"},
            {"y manual_control_t", "[-1000,1000]"},
            {"z manual_control_t", "[-1000,1000]"},
            {"r manual_control_t", "[-1000,1000]"},
            {"chan1_raw rc_channels_override_t", "[us]"},
            {"chan2_raw rc_channels_override_t", "[us]"},
            {"chan3_raw rc_channels_override_t", "[us]"},
            {"chan4_raw rc_channels_override_t", "[us]"},
            {"chan5_raw rc_channels_override_t", "[us]"},
            {"chan6_raw rc_channels_override_t", "[us]"},
            {"chan7_raw rc_channels_override_t", "[us]"},
            {"chan8_raw rc_channels_override_t", "[us]"},
            {"chan9_raw rc_channels_override_t", "[us]"},
            {"chan10_raw rc_channels_override_t", "[us]"},
            {"chan11_raw rc_channels_override_t", "[us]"},
            {"chan12_raw rc_channels_override_t", "[us]"},
            {"chan13_raw rc_channels_override_t", "[us]"},
            {"chan14_raw rc_channels_override_t", "[us]"},
            {"chan15_raw rc_channels_override_t", "[us]"},
            {"chan16_raw rc_channels_override_t", "[us]"},
            {"chan17_raw rc_channels_override_t", "[us]"},
            {"chan18_raw rc_channels_override_t", "[us]"},
            {"airspeed vfr_hud_t", "[m/s]"},
            {"groundspeed vfr_hud_t", "[m/s]"},
            {"alt vfr_hud_t", "[m]"},
            {"climb vfr_hud_t", "[m/s]"},
            {"heading vfr_hud_t", "[deg]"},
            {"throttle vfr_hud_t", "[%]"},
            {"time_boot_ms manual_setpoint_t", "[ms]"},
            {"roll manual_setpoint_t", "[rad/s]"},
            {"pitch manual_setpoint_t", "[rad/s]"},
            {"yaw manual_setpoint_t", "[rad/s]"},
            {"time_boot_ms set_attitude_target_t", "[ms]"},
            {"body_roll_rate set_attitude_target_t", "[rad/s]"},
            {"body_pitch_rate set_attitude_target_t", "[rad/s]"},
            {"body_yaw_rate set_attitude_target_t", "[rad/s]"},
            {"time_boot_ms attitude_target_t", "[ms]"},
            {"body_roll_rate attitude_target_t", "[rad/s]"},
            {"body_pitch_rate attitude_target_t", "[rad/s]"},
            {"body_yaw_rate attitude_target_t", "[rad/s]"},
            {"time_boot_ms set_position_target_local_ned_t", "[ms]"},
            {"x set_position_target_local_ned_t", "[m]"},
            {"y set_position_target_local_ned_t", "[m]"},
            {"z set_position_target_local_ned_t", "[m]"},
            {"vx set_position_target_local_ned_t", "[m/s]"},
            {"vy set_position_target_local_ned_t", "[m/s]"},
            {"vz set_position_target_local_ned_t", "[m/s]"},
            {"afx set_position_target_local_ned_t", "[m/s/s]"},
            {"afy set_position_target_local_ned_t", "[m/s/s]"},
            {"afz set_position_target_local_ned_t", "[m/s/s]"},
            {"yaw set_position_target_local_ned_t", "[rad]"},
            {"yaw_rate set_position_target_local_ned_t", "[rad/s]"},
            {"time_boot_ms position_target_local_ned_t", "[ms]"},
            {"x position_target_local_ned_t", "[m]"},
            {"y position_target_local_ned_t", "[m]"},
            {"z position_target_local_ned_t", "[m]"},
            {"vx position_target_local_ned_t", "[m/s]"},
            {"vy position_target_local_ned_t", "[m/s]"},
            {"vz position_target_local_ned_t", "[m/s]"},
            {"afx position_target_local_ned_t", "[m/s/s]"},
            {"afy position_target_local_ned_t", "[m/s/s]"},
            {"afz position_target_local_ned_t", "[m/s/s]"},
            {"yaw position_target_local_ned_t", "[rad]"},
            {"yaw_rate position_target_local_ned_t", "[rad/s]"},
            {"time_boot_ms set_position_target_global_int_t", "[ms]"},
            {"lat_int set_position_target_global_int_t", "[degE7]"},
            {"lon_int set_position_target_global_int_t", "[degE7]"},
            {"alt set_position_target_global_int_t", "[m]"},
            {"vx set_position_target_global_int_t", "[m/s]"},
            {"vy set_position_target_global_int_t", "[m/s]"},
            {"vz set_position_target_global_int_t", "[m/s]"},
            {"afx set_position_target_global_int_t", "[m/s/s]"},
            {"afy set_position_target_global_int_t", "[m/s/s]"},
            {"afz set_position_target_global_int_t", "[m/s/s]"},
            {"yaw set_position_target_global_int_t", "[rad]"},
            {"yaw_rate set_position_target_global_int_t", "[rad/s]"},
            {"time_boot_ms position_target_global_int_t", "[ms]"},
            {"lat_int position_target_global_int_t", "[degE7]"},
            {"lon_int position_target_global_int_t", "[degE7]"},
            {"alt position_target_global_int_t", "[m]"},
            {"vx position_target_global_int_t", "[m/s]"},
            {"vy position_target_global_int_t", "[m/s]"},
            {"vz position_target_global_int_t", "[m/s]"},
            {"afx position_target_global_int_t", "[m/s/s]"},
            {"afy position_target_global_int_t", "[m/s/s]"},
            {"afz position_target_global_int_t", "[m/s/s]"},
            {"yaw position_target_global_int_t", "[rad]"},
            {"yaw_rate position_target_global_int_t", "[rad/s]"},
            {"time_boot_ms local_position_ned_system_global_offset_t", "[ms]"},
            {"x local_position_ned_system_global_offset_t", "[m]"},
            {"y local_position_ned_system_global_offset_t", "[m]"},
            {"z local_position_ned_system_global_offset_t", "[m]"},
            {"roll local_position_ned_system_global_offset_t", "[rad]"},
            {"pitch local_position_ned_system_global_offset_t", "[rad]"},
            {"yaw local_position_ned_system_global_offset_t", "[rad]"},
            {"time_usec hil_state_t", "[us]"},
            {"roll hil_state_t", "[rad]"},
            {"pitch hil_state_t", "[rad]"},
            {"yaw hil_state_t", "[rad]"},
            {"rollspeed hil_state_t", "[rad/s]"},
            {"pitchspeed hil_state_t", "[rad/s]"},
            {"yawspeed hil_state_t", "[rad/s]"},
            {"lat hil_state_t", "[degE7]"},
            {"lon hil_state_t", "[degE7]"},
            {"alt hil_state_t", "[mm]"},
            {"vx hil_state_t", "[cm/s]"},
            {"vy hil_state_t", "[cm/s]"},
            {"vz hil_state_t", "[cm/s]"},
            {"xacc hil_state_t", "[mG]"},
            {"yacc hil_state_t", "[mG]"},
            {"zacc hil_state_t", "[mG]"},
            {"time_usec hil_controls_t", "[us]"},
            {"time_usec hil_rc_inputs_raw_t", "[us]"},
            {"chan1_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan2_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan3_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan4_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan5_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan6_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan7_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan8_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan9_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan10_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan11_raw hil_rc_inputs_raw_t", "[us]"},
            {"chan12_raw hil_rc_inputs_raw_t", "[us]"},
            {"time_usec hil_actuator_controls_t", "[us]"},
            {"time_usec optical_flow_t", "[us]"},
            {"flow_comp_m_x optical_flow_t", "[m]"},
            {"flow_comp_m_y optical_flow_t", "[m]"},
            {"ground_distance optical_flow_t", "[m]"},
            {"flow_x optical_flow_t", "[dpix]"},
            {"flow_y optical_flow_t", "[dpix]"},
            {"flow_rate_x optical_flow_t", "[rad/s]"},
            {"flow_rate_y optical_flow_t", "[rad/s]"},
            {"usec global_vision_position_estimate_t", "[us]"},
            {"x global_vision_position_estimate_t", "[m]"},
            {"y global_vision_position_estimate_t", "[m]"},
            {"z global_vision_position_estimate_t", "[m]"},
            {"roll global_vision_position_estimate_t", "[rad]"},
            {"pitch global_vision_position_estimate_t", "[rad]"},
            {"yaw global_vision_position_estimate_t", "[rad]"},
            {"usec vision_position_estimate_t", "[us]"},
            {"x vision_position_estimate_t", "[m]"},
            {"y vision_position_estimate_t", "[m]"},
            {"z vision_position_estimate_t", "[m]"},
            {"roll vision_position_estimate_t", "[rad]"},
            {"pitch vision_position_estimate_t", "[rad]"},
            {"yaw vision_position_estimate_t", "[rad]"},
            {"usec vision_speed_estimate_t", "[us]"},
            {"x vision_speed_estimate_t", "[m/s]"},
            {"y vision_speed_estimate_t", "[m/s]"},
            {"z vision_speed_estimate_t", "[m/s]"},
            {"usec vicon_position_estimate_t", "[us]"},
            {"x vicon_position_estimate_t", "[m]"},
            {"y vicon_position_estimate_t", "[m]"},
            {"z vicon_position_estimate_t", "[m]"},
            {"roll vicon_position_estimate_t", "[rad]"},
            {"pitch vicon_position_estimate_t", "[rad]"},
            {"yaw vicon_position_estimate_t", "[rad]"},
            {"time_usec highres_imu_t", "[us]"},
            {"xacc highres_imu_t", "[m/s/s]"},
            {"yacc highres_imu_t", "[m/s/s]"},
            {"zacc highres_imu_t", "[m/s/s]"},
            {"xgyro highres_imu_t", "[rad/s]"},
            {"ygyro highres_imu_t", "[rad/s]"},
            {"zgyro highres_imu_t", "[rad/s]"},
            {"xmag highres_imu_t", "[gauss]"},
            {"ymag highres_imu_t", "[gauss]"},
            {"zmag highres_imu_t", "[gauss]"},
            {"abs_pressure highres_imu_t", "[mbar]"},
            {"diff_pressure highres_imu_t", "[mbar]"},
            {"temperature highres_imu_t", "[degC]"},
            {"time_usec optical_flow_rad_t", "[us]"},
            {"integration_time_us optical_flow_rad_t", "[us]"},
            {"integrated_x optical_flow_rad_t", "[rad]"},
            {"integrated_y optical_flow_rad_t", "[rad]"},
            {"integrated_xgyro optical_flow_rad_t", "[rad]"},
            {"integrated_ygyro optical_flow_rad_t", "[rad]"},
            {"integrated_zgyro optical_flow_rad_t", "[rad]"},
            {"time_delta_distance_us optical_flow_rad_t", "[us]"},
            {"distance optical_flow_rad_t", "[m]"},
            {"temperature optical_flow_rad_t", "[cdegC]"},
            {"time_usec hil_sensor_t", "[us]"},
            {"xacc hil_sensor_t", "[m/s/s]"},
            {"yacc hil_sensor_t", "[m/s/s]"},
            {"zacc hil_sensor_t", "[m/s/s]"},
            {"xgyro hil_sensor_t", "[rad/s]"},
            {"ygyro hil_sensor_t", "[rad/s]"},
            {"zgyro hil_sensor_t", "[rad/s]"},
            {"xmag hil_sensor_t", "[gauss]"},
            {"ymag hil_sensor_t", "[gauss]"},
            {"zmag hil_sensor_t", "[gauss]"},
            {"abs_pressure hil_sensor_t", "[mbar]"},
            {"diff_pressure hil_sensor_t", "[mbar]"},
            {"temperature hil_sensor_t", "[degC]"},
            {"xacc sim_state_t", "[m/s/s]"},
            {"yacc sim_state_t", "[m/s/s]"},
            {"zacc sim_state_t", "[m/s/s]"},
            {"xgyro sim_state_t", "[rad/s]"},
            {"ygyro sim_state_t", "[rad/s]"},
            {"zgyro sim_state_t", "[rad/s]"},
            {"lat sim_state_t", "[deg]"},
            {"lon sim_state_t", "[deg]"},
            {"alt sim_state_t", "[m]"},
            {"vn sim_state_t", "[m/s]"},
            {"ve sim_state_t", "[m/s]"},
            {"vd sim_state_t", "[m/s]"},
            {"txbuf radio_status_t", "[%]"},
            {"time_usec camera_trigger_t", "[us]"},
            {"time_usec hil_gps_t", "[us]"},
            {"lat hil_gps_t", "[degE7]"},
            {"lon hil_gps_t", "[degE7]"},
            {"alt hil_gps_t", "[mm]"},
            {"vel hil_gps_t", "[cm/s]"},
            {"vn hil_gps_t", "[cm/s]"},
            {"ve hil_gps_t", "[cm/s]"},
            {"vd hil_gps_t", "[cm/s]"},
            {"cog hil_gps_t", "[cdeg]"},
            {"time_usec hil_optical_flow_t", "[us]"},
            {"integration_time_us hil_optical_flow_t", "[us]"},
            {"integrated_x hil_optical_flow_t", "[rad]"},
            {"integrated_y hil_optical_flow_t", "[rad]"},
            {"integrated_xgyro hil_optical_flow_t", "[rad]"},
            {"integrated_ygyro hil_optical_flow_t", "[rad]"},
            {"integrated_zgyro hil_optical_flow_t", "[rad]"},
            {"time_delta_distance_us hil_optical_flow_t", "[us]"},
            {"distance hil_optical_flow_t", "[m]"},
            {"temperature hil_optical_flow_t", "[cdegC]"},
            {"time_usec hil_state_quaternion_t", "[us]"},
            {"rollspeed hil_state_quaternion_t", "[rad/s]"},
            {"pitchspeed hil_state_quaternion_t", "[rad/s]"},
            {"yawspeed hil_state_quaternion_t", "[rad/s]"},
            {"lat hil_state_quaternion_t", "[degE7]"},
            {"lon hil_state_quaternion_t", "[degE7]"},
            {"alt hil_state_quaternion_t", "[mm]"},
            {"vx hil_state_quaternion_t", "[cm/s]"},
            {"vy hil_state_quaternion_t", "[cm/s]"},
            {"vz hil_state_quaternion_t", "[cm/s]"},
            {"ind_airspeed hil_state_quaternion_t", "[cm/s]"},
            {"true_airspeed hil_state_quaternion_t", "[cm/s]"},
            {"xacc hil_state_quaternion_t", "[mG]"},
            {"yacc hil_state_quaternion_t", "[mG]"},
            {"zacc hil_state_quaternion_t", "[mG]"},
            {"time_boot_ms scaled_imu2_t", "[ms]"},
            {"xacc scaled_imu2_t", "[mG]"},
            {"yacc scaled_imu2_t", "[mG]"},
            {"zacc scaled_imu2_t", "[mG]"},
            {"xgyro scaled_imu2_t", "[mrad/s]"},
            {"ygyro scaled_imu2_t", "[mrad/s]"},
            {"zgyro scaled_imu2_t", "[mrad/s]"},
            {"xmag scaled_imu2_t", "[mT]"},
            {"ymag scaled_imu2_t", "[mT]"},
            {"zmag scaled_imu2_t", "[mT]"},
            {"time_utc log_entry_t", "[s]"},
            {"size log_entry_t", "[bytes]"},
            {"count log_request_data_t", "[bytes]"},
            {"count log_data_t", "[bytes]"},
            {"len gps_inject_data_t", "[bytes]"},
            {"time_usec gps2_raw_t", "[us]"},
            {"lat gps2_raw_t", "[degE7]"},
            {"lon gps2_raw_t", "[degE7]"},
            {"alt gps2_raw_t", "[mm]"},
            {"dgps_age gps2_raw_t", "[ms]"},
            {"eph gps2_raw_t", "[cm]"},
            {"epv gps2_raw_t", "[cm]"},
            {"vel gps2_raw_t", "[cm/s]"},
            {"cog gps2_raw_t", "[cdeg]"},
            {"Vcc power_status_t", "[mV]"},
            {"Vservo power_status_t", "[mV]"},
            {"baudrate serial_control_t", "[bits/s]"},
            {"timeout serial_control_t", "[ms]"},
            {"count serial_control_t", "[bytes]"},
            {"time_last_baseline_ms gps_rtk_t", "[ms]"},
            {"tow gps_rtk_t", "[ms]"},
            {"baseline_a_mm gps_rtk_t", "[mm]"},
            {"baseline_b_mm gps_rtk_t", "[mm]"},
            {"baseline_c_mm gps_rtk_t", "[mm]"},
            {"rtk_rate gps_rtk_t", "[Hz]"},
            {"time_last_baseline_ms gps2_rtk_t", "[ms]"},
            {"tow gps2_rtk_t", "[ms]"},
            {"baseline_a_mm gps2_rtk_t", "[mm]"},
            {"baseline_b_mm gps2_rtk_t", "[mm]"},
            {"baseline_c_mm gps2_rtk_t", "[mm]"},
            {"rtk_rate gps2_rtk_t", "[Hz]"},
            {"time_boot_ms scaled_imu3_t", "[ms]"},
            {"xacc scaled_imu3_t", "[mG]"},
            {"yacc scaled_imu3_t", "[mG]"},
            {"zacc scaled_imu3_t", "[mG]"},
            {"xgyro scaled_imu3_t", "[mrad/s]"},
            {"ygyro scaled_imu3_t", "[mrad/s]"},
            {"zgyro scaled_imu3_t", "[mrad/s]"},
            {"xmag scaled_imu3_t", "[mT]"},
            {"ymag scaled_imu3_t", "[mT]"},
            {"zmag scaled_imu3_t", "[mT]"},
            {"size data_transmission_handshake_t", "[bytes]"},
            {"payload data_transmission_handshake_t", "[bytes]"},
            {"jpg_quality data_transmission_handshake_t", "[1,100]"},
            {"time_boot_ms distance_sensor_t", "[ms]"},
            {"min_distance distance_sensor_t", "[cm]"},
            {"max_distance distance_sensor_t", "[cm]"},
            {"current_distance distance_sensor_t", "[cm]"},
            {"covariance distance_sensor_t", "[cm]"},
            {"lat terrain_request_t", "[degE7]"},
            {"lon terrain_request_t", "[degE7]"},
            {"grid_spacing terrain_request_t", "[m]"},
            {"lat terrain_data_t", "[degE7]"},
            {"lon terrain_data_t", "[degE7]"},
            {"grid_spacing terrain_data_t", "[m]"},
            {"data terrain_data_t", "[m]"},
            {"lat terrain_check_t", "[degE7]"},
            {"lon terrain_check_t", "[degE7]"},
            {"lat terrain_report_t", "[degE7]"},
            {"lon terrain_report_t", "[degE7]"},
            {"terrain_height terrain_report_t", "[m]"},
            {"current_height terrain_report_t", "[m]"},
            {"time_boot_ms scaled_pressure2_t", "[ms]"},
            {"press_abs scaled_pressure2_t", "[hPa]"},
            {"press_diff scaled_pressure2_t", "[hPa]"},
            {"temperature scaled_pressure2_t", "[cdegC]"},
            {"time_usec att_pos_mocap_t", "[us]"},
            {"x att_pos_mocap_t", "[m]"},
            {"y att_pos_mocap_t", "[m]"},
            {"z att_pos_mocap_t", "[m]"},
            {"time_usec set_actuator_control_target_t", "[us]"},
            {"time_usec actuator_control_target_t", "[us]"},
            {"time_usec altitude_t", "[us]"},
            {"altitude_monotonic altitude_t", "[m]"},
            {"altitude_amsl altitude_t", "[m]"},
            {"altitude_local altitude_t", "[m]"},
            {"altitude_relative altitude_t", "[m]"},
            {"altitude_terrain altitude_t", "[m]"},
            {"bottom_clearance altitude_t", "[m]"},
            {"time_boot_ms scaled_pressure3_t", "[ms]"},
            {"press_abs scaled_pressure3_t", "[hPa]"},
            {"press_diff scaled_pressure3_t", "[hPa]"},
            {"temperature scaled_pressure3_t", "[cdegC]"},
            {"timestamp follow_target_t", "[ms]"},
            {"lat follow_target_t", "[degE7]"},
            {"lon follow_target_t", "[degE7]"},
            {"alt follow_target_t", "[m]"},
            {"vel follow_target_t", "[m/s]"},
            {"acc follow_target_t", "[m/s/s]"},
            {"time_usec control_system_state_t", "[us]"},
            {"x_acc control_system_state_t", "[m/s/s]"},
            {"y_acc control_system_state_t", "[m/s/s]"},
            {"z_acc control_system_state_t", "[m/s/s]"},
            {"x_vel control_system_state_t", "[m/s]"},
            {"y_vel control_system_state_t", "[m/s]"},
            {"z_vel control_system_state_t", "[m/s]"},
            {"x_pos control_system_state_t", "[m]"},
            {"y_pos control_system_state_t", "[m]"},
            {"z_pos control_system_state_t", "[m]"},
            {"airspeed control_system_state_t", "[m/s]"},
            {"roll_rate control_system_state_t", "[rad/s]"},
            {"pitch_rate control_system_state_t", "[rad/s]"},
            {"yaw_rate control_system_state_t", "[rad/s]"},
            {"current_consumed battery_status_t", "[mAh]"},
            {"energy_consumed battery_status_t", "[hJ]"},
            {"temperature battery_status_t", "[cdegC]"},
            {"voltages battery_status_t", "[mV]"},
            {"current_battery battery_status_t", "[cA]"},
            {"battery_remaining battery_status_t", "[%]"},
            {"time_remaining battery_status_t", "[s]"},
            {"time_usec landing_target_t", "[us]"},
            {"angle_x landing_target_t", "[rad]"},
            {"angle_y landing_target_t", "[rad]"},
            {"distance landing_target_t", "[m]"},
            {"size_x landing_target_t", "[rad]"},
            {"size_y landing_target_t", "[rad]"},
            {"x landing_target_t", "[m]"},
            {"y landing_target_t", "[m]"},
            {"z landing_target_t", "[m]"},
            {"time_usec estimator_status_t", "[us]"},
            {"pos_horiz_accuracy estimator_status_t", "[m]"},
            {"pos_vert_accuracy estimator_status_t", "[m]"},
            {"time_usec wind_cov_t", "[us]"},
            {"wind_x wind_cov_t", "[m/s]"},
            {"wind_y wind_cov_t", "[m/s]"},
            {"wind_z wind_cov_t", "[m/s]"},
            {"var_horiz wind_cov_t", "[m/s]"},
            {"var_vert wind_cov_t", "[m/s]"},
            {"wind_alt wind_cov_t", "[m]"},
            {"horiz_accuracy wind_cov_t", "[m]"},
            {"vert_accuracy wind_cov_t", "[m]"},
            {"time_usec gps_input_t", "[us]"},
            {"time_week_ms gps_input_t", "[ms]"},
            {"lat gps_input_t", "[degE7]"},
            {"lon gps_input_t", "[degE7]"},
            {"alt gps_input_t", "[m]"},
            {"hdop gps_input_t", "[m]"},
            {"vdop gps_input_t", "[m]"},
            {"vn gps_input_t", "[m/s]"},
            {"ve gps_input_t", "[m/s]"},
            {"vd gps_input_t", "[m/s]"},
            {"speed_accuracy gps_input_t", "[m/s]"},
            {"horiz_accuracy gps_input_t", "[m]"},
            {"vert_accuracy gps_input_t", "[m]"},
            {"len gps_rtcm_data_t", "[bytes]"},
            {"latitude high_latency_t", "[degE7]"},
            {"longitude high_latency_t", "[degE7]"},
            {"roll high_latency_t", "[cdeg]"},
            {"pitch high_latency_t", "[cdeg]"},
            {"heading high_latency_t", "[cdeg]"},
            {"heading_sp high_latency_t", "[cdeg]"},
            {"altitude_amsl high_latency_t", "[m]"},
            {"altitude_sp high_latency_t", "[m]"},
            {"wp_distance high_latency_t", "[m]"},
            {"throttle high_latency_t", "[%]"},
            {"airspeed high_latency_t", "[m/s]"},
            {"airspeed_sp high_latency_t", "[m/s]"},
            {"groundspeed high_latency_t", "[m/s]"},
            {"climb_rate high_latency_t", "[m/s]"},
            {"battery_remaining high_latency_t", "[%]"},
            {"temperature high_latency_t", "[degC]"},
            {"temperature_air high_latency_t", "[degC]"},
            {"time_usec vibration_t", "[us]"},
            {"latitude home_position_t", "[degE7]"},
            {"longitude home_position_t", "[degE7]"},
            {"altitude home_position_t", "[mm]"},
            {"x home_position_t", "[m]"},
            {"y home_position_t", "[m]"},
            {"z home_position_t", "[m]"},
            {"approach_x home_position_t", "[m]"},
            {"approach_y home_position_t", "[m]"},
            {"approach_z home_position_t", "[m]"},
            {"time_usec home_position_t", "[us]"},
            {"latitude set_home_position_t", "[degE7]"},
            {"longitude set_home_position_t", "[degE7]"},
            {"altitude set_home_position_t", "[mm]"},
            {"x set_home_position_t", "[m]"},
            {"y set_home_position_t", "[m]"},
            {"z set_home_position_t", "[m]"},
            {"approach_x set_home_position_t", "[m]"},
            {"approach_y set_home_position_t", "[m]"},
            {"approach_z set_home_position_t", "[m]"},
            {"time_usec set_home_position_t", "[us]"},
            {"interval_us message_interval_t", "[us]"},
            {"lat adsb_vehicle_t", "[degE7]"},
            {"lon adsb_vehicle_t", "[degE7]"},
            {"altitude adsb_vehicle_t", "[mm]"},
            {"heading adsb_vehicle_t", "[cdeg]"},
            {"hor_velocity adsb_vehicle_t", "[cm/s]"},
            {"ver_velocity adsb_vehicle_t", "[cm/s]"},
            {"tslc adsb_vehicle_t", "[s]"},
            {"time_to_minimum_delta collision_t", "[s]"},
            {"altitude_minimum_delta collision_t", "[m]"},
            {"horizontal_minimum_delta collision_t", "[m]"},
            {"time_usec debug_vect_t", "[us]"},
            {"time_boot_ms named_value_float_t", "[ms]"},
            {"time_boot_ms named_value_int_t", "[ms]"},
            {"time_boot_ms debug_t", "[ms]"},
            {"time_boot_ms button_change_t", "[ms]"},
            {"last_change_ms button_change_t", "[ms]"},
            {"time_boot_ms camera_information_t", "[ms]"},
            {"focal_length camera_information_t", "[mm]"},
            {"sensor_size_h camera_information_t", "[mm]"},
            {"sensor_size_v camera_information_t", "[mm]"},
            {"resolution_h camera_information_t", "[pix]"},
            {"resolution_v camera_information_t", "[pix]"},
            {"time_boot_ms camera_settings_t", "[ms]"},
            {"shutter_speed camera_settings_t", "[s]"},
            {"white_balance camera_settings_t", "[K]"},
            {"time_boot_ms storage_information_t", "[ms]"},
            {"total_capacity storage_information_t", "[MiB]"},
            {"used_capacity storage_information_t", "[MiB]"},
            {"available_capacity storage_information_t", "[MiB]"},
            {"read_speed storage_information_t", "[MiB/s]"},
            {"write_speed storage_information_t", "[MiB/s]"},
            {"time_boot_ms camera_capture_status_t", "[ms]"},
            {"image_interval camera_capture_status_t", "[s]"},
            {"video_framerate camera_capture_status_t", "[Hz]"},
            {"recording_time_ms camera_capture_status_t", "[ms]"},
            {"available_capacity camera_capture_status_t", "[MiB]"},
            {"image_resolution_h camera_capture_status_t", "[pix]"},
            {"image_resolution_v camera_capture_status_t", "[pix]"},
            {"video_resolution_h camera_capture_status_t", "[pix]"},
            {"video_resolution_v camera_capture_status_t", "[pix]"},
            {"time_utc camera_image_captured_t", "[us]"},
            {"time_boot_ms camera_image_captured_t", "[ms]"},
            {"lat camera_image_captured_t", "[degE7]"},
            {"lon camera_image_captured_t", "[degE7]"},
            {"alt camera_image_captured_t", "[m]"},
            {"relative_alt camera_image_captured_t", "[m]"},
            {"arming_time_utc flight_information_t", "[us]"},
            {"takeoff_time_utc flight_information_t", "[us]"},
            {"time_boot_ms flight_information_t", "[ms]"},
            {"time_boot_ms mount_orientation_t", "[ms]"},
            {"roll mount_orientation_t", "[deg]"},
            {"pitch mount_orientation_t", "[deg]"},
            {"yaw mount_orientation_t", "[deg]"},
            {"yaw_absolute mount_orientation_t", "[deg]"},
            {"length logging_data_t", "[bytes]"},
            {"first_message_offset logging_data_t", "[bytes]"},
            {"length logging_data_acked_t", "[bytes]"},
            {"first_message_offset logging_data_acked_t", "[bytes]"},
            {"time_usec uavcan_node_status_t", "[us]"},
            {"uptime_sec uavcan_node_status_t", "[s]"},
            {"time_usec uavcan_node_info_t", "[us]"},
            {"uptime_sec uavcan_node_info_t", "[s]"},
            {"time_usec obstacle_distance_t", "[us]"},
            {"distances obstacle_distance_t", "[cm]"},
            {"min_distance obstacle_distance_t", "[cm]"},
            {"max_distance obstacle_distance_t", "[cm]"},
            {"increment obstacle_distance_t", "[deg]"},
            {"time_usec odometry_t", "[us]"},
            {"x odometry_t", "[m]"},
            {"y odometry_t", "[m]"},
            {"z odometry_t", "[m]"},
            {"vx odometry_t", "[m/s]"},
            {"vy odometry_t", "[m/s]"},
            {"vz odometry_t", "[m/s]"},
            {"rollspeed odometry_t", "[rad/s]"},
            {"pitchspeed odometry_t", "[rad/s]"},
            {"yawspeed odometry_t", "[rad/s]"},
            {"stallSpeed uavionix_adsb_out_cfg_t", "[cm/s]"},
            {"gpsOffsetLon uavionix_adsb_out_cfg_t", "[if"},
            {"utcTime uavionix_adsb_out_dynamic_t", "[s]"},
            {"gpsLat uavionix_adsb_out_dynamic_t", "[degE7]"},
            {"gpsLon uavionix_adsb_out_dynamic_t", "[degE7]"},
            {"gpsAlt uavionix_adsb_out_dynamic_t", "[mm]"},
            {"baroAltMSL uavionix_adsb_out_dynamic_t", "[mbar]"},
            {"accuracyHor uavionix_adsb_out_dynamic_t", "[mm]"},
            {"accuracyVert uavionix_adsb_out_dynamic_t", "[cm]"},
            {"accuracyVel uavionix_adsb_out_dynamic_t", "[mm/s]"},
            {"velVert uavionix_adsb_out_dynamic_t", "[cm/s]"},
            {"velNS uavionix_adsb_out_dynamic_t", "[cm/s]"},
            {"VelEW uavionix_adsb_out_dynamic_t", "[cm/s]"},
            {"squawk uavionix_adsb_out_dynamic_t", "[0x04B0]"},
            {"min1 icarous_kinematic_bands_t", "[deg]"},
            {"max1 icarous_kinematic_bands_t", "[deg]"},
            {"min2 icarous_kinematic_bands_t", "[deg]"},
            {"max2 icarous_kinematic_bands_t", "[deg]"},
            {"min3 icarous_kinematic_bands_t", "[deg]"},
            {"max3 icarous_kinematic_bands_t", "[deg]"},
            {"min4 icarous_kinematic_bands_t", "[deg]"},
            {"max4 icarous_kinematic_bands_t", "[deg]"},
            {"min5 icarous_kinematic_bands_t", "[deg]"},
            {"max5 icarous_kinematic_bands_t", "[deg]"},
        };

        #endregion

        #region Defaults

        /// <summary>
        /// A simple struct that defines the
        /// default property values for the <see cref="Line"/> class.
        /// </summary>
        new public struct Default
		{
			// Default Line properties
			/// <summary>
			/// The default color for curves (line segments connecting the points).
			/// This is the default value for the <see cref="LineBase.Color"/> property.
			/// </summary>
			public static Color Color = Color.Red;
			/// <summary>
			/// The default color for filling in the area under the curve
			/// (<see cref="ZedGraph.Fill.Color"/> property).
			/// </summary>
			public static Color FillColor = Color.Red;
			/// <summary>
			/// The default custom brush for filling in the area under the curve
			/// (<see cref="ZedGraph.Fill.Brush"/> property).
			/// </summary>
			public static Brush FillBrush = null;
			/// <summary>
			/// The default fill mode for the curve (<see cref="ZedGraph.Fill.Type"/> property).
			/// </summary>
			public static FillType FillType = FillType.None;

			/// <summary>
			/// The default value for the <see cref="Line.IsSmooth"/>
			/// property.
			/// </summary>
			public static bool IsSmooth = false;
			/// <summary>
			/// The default value for the <see cref="Line.SmoothTension"/> property.
			/// </summary>
			public static float SmoothTension = 0.5F;
			/// <summary>
			/// The default value for the <see cref="Line.IsOptimizedDraw"/> property.
			/// </summary>
			public static bool IsOptimizedDraw = false;

			/// <summary>
			/// Default value for the curve type property
			/// (<see cref="Line.StepType"/>).  This determines if the curve
			/// will be drawn by directly connecting the points from the
			/// <see cref="CurveItem.Points"/> data collection,
			/// or if the curve will be a "stair-step" in which the points are
			/// connected by a series of horizontal and vertical lines that
			/// represent discrete, staticant values.  Note that the values can
			/// be forward oriented <code>ForwardStep</code> (<see cref="StepType"/>) or
			/// rearward oriented <code>RearwardStep</code>.
			/// That is, the points are defined at the beginning or end
			/// of the staticant value for which they apply, respectively.
			/// </summary>
			/// <value><see cref="StepType"/> enum value</value>
			public static StepType StepType = StepType.NonStep;
		}

	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets a property that determines if this <see cref="Line"/>
		/// will be drawn smooth.  The "smoothness" is controlled by
		/// the <see cref="SmoothTension"/> property.
		/// </summary>
		/// <value>true to smooth the line, false to just connect the dots
		/// with linear segments</value>
		/// <seealso cref="SmoothTension"/>
		/// <seealso cref="Default.IsSmooth"/>
		/// <seealso cref="Default.SmoothTension"/>
		public bool IsSmooth
		{
			get { return _isSmooth; }
			set { _isSmooth = value; }
		}
		/// <summary>
		/// Gets or sets a property that determines the smoothing tension
		/// for this <see cref="Line"/>.  This property is only used if
		/// <see cref="IsSmooth"/> is true.  A tension value 0.0 will just
		/// draw ordinary line segments like an unsmoothed line.  A tension
		/// value of 1.0 will be smooth.  Values greater than 1.0 will generally
		/// give odd results.
		/// </summary>
		/// <value>A floating point value indicating the level of smoothing.
		/// 0.0F for no smoothing, 1.0F for lots of smoothing, >1.0 for odd
		/// smoothing.</value>
		/// <seealso cref="IsSmooth"/>
		/// <seealso cref="Default.IsSmooth"/>
		/// <seealso cref="Default.SmoothTension"/>
		public float SmoothTension
		{
			get { return _smoothTension; }
			set { _smoothTension = value; }
		}
		/// <summary>
		/// Determines if the <see cref="CurveItem"/> will be drawn by directly connecting the
		/// points from the <see cref="CurveItem.Points"/> data collection,
		/// or if the curve will be a "stair-step" in which the points are
		/// connected by a series of horizontal and vertical lines that
		/// represent discrete, constant values.  Note that the values can
		/// be forward oriented <c>ForwardStep</c> (<see cref="ZedGraph.StepType"/>) or
		/// rearward oriented <c>RearwardStep</c>.
		/// That is, the points are defined at the beginning or end
		/// of the constant value for which they apply, respectively.
		/// The <see cref="StepType"/> property is ignored for lines
		/// that have <see cref="IsSmooth"/> set to true.
		/// </summary>
		/// <value><see cref="ZedGraph.StepType"/> enum value</value>
		/// <seealso cref="Default.StepType"/>
		public StepType StepType
		{
			get { return _stepType; }
			set { _stepType = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="ZedGraph.Fill"/> data for this
		/// <see cref="Line"/>.
		/// </summary>
		public Fill Fill
		{
			get { return _fill; }
			set { _fill = value; }
		}

		/// <summary>
		/// Gets or sets a boolean value that determines if this <see cref="Line"/> will be drawn with
		/// optimizations enabled.
		/// </summary>
		/// <remarks>
		/// Normally, the optimizations can be used without a problem, especially if the data
		/// are sorted.  The optimizations are particularly helpful with very large datasets.
		/// However, if the data are very discontinuous (for example, a curve that doubles back
		/// on itself), then the optimizations can cause drawing artifacts in the form of
		/// missing line segments.  The default option for this mode is false, so you must
		/// explicitly enable it for each <see cref="LineItem.Line">LineItem.Line</see>.
		/// Also note that, even if the optimizations are enabled explicitly, no actual
		/// optimization will be done for datasets of less than 1000 points.
		/// </remarks>
		public bool IsOptimizedDraw
		{
			get { return _isOptimizedDraw; }
			set { _isOptimizedDraw = value; }
		}

	#endregion

	#region Constructors

		/// <summary>
		/// Default constructor that sets all <see cref="Line"/> properties to default
		/// values as defined in the <see cref="Default"/> class.
		/// </summary>
		public Line()
			: this( Color.Empty )
		{
		}

		/// <summary>
		/// Constructor that sets the color property to the specified value, and sets
		/// the remaining <see cref="Line"/> properties to default
		/// values as defined in the <see cref="Default"/> class.
		/// </summary>
		/// <param name="color">The color to assign to this new Line object</param>
		public Line( Color color )
		{
			_color = color.IsEmpty ? Default.Color : color;
			_stepType = Default.StepType;
			_isSmooth = Default.IsSmooth;
			_smoothTension = Default.SmoothTension;
			_fill = new Fill( Default.FillColor, Default.FillBrush, Default.FillType );
			_isOptimizedDraw = Default.IsOptimizedDraw;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The Line object from which to copy</param>
		public Line( Line rhs ) : base( rhs )
		{
			_color = rhs._color;
			_stepType = rhs._stepType;
			_isSmooth = rhs._isSmooth;
			_smoothTension = rhs._smoothTension;
			_fill = rhs._fill.Clone();
			_isOptimizedDraw = rhs._isOptimizedDraw;
		}

		/// <summary>
		/// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
		/// calling the typed version of <see cref="Clone" />
		/// </summary>
		/// <returns>A deep copy of this object</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Typesafe, deep-copy clone method.
		/// </summary>
		/// <returns>A new, independent copy of this class</returns>
		public Line Clone()
		{
			return new Line( this );
		}

	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema = 14;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected Line( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			//if ( sch >= 14 )
			//	_color = (Color) info.GetValue( "color", typeof( Color ) );
			_stepType = (StepType)info.GetValue( "stepType", typeof( StepType ) );
			_isSmooth = info.GetBoolean( "isSmooth" );
			_smoothTension = info.GetSingle( "smoothTension" );
			_fill = (Fill)info.GetValue( "fill", typeof( Fill ) );

			if ( sch >= 13 )
				_isOptimizedDraw = info.GetBoolean( "isOptimizedDraw" );
		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );

			info.AddValue( "schema", schema );
			//info.AddValue( "color", _color );
			info.AddValue( "stepType", _stepType );
			info.AddValue( "isSmooth", _isSmooth );
			info.AddValue( "smoothTension", _smoothTension );
			info.AddValue( "fill", _fill );

			info.AddValue( "isOptimizedDraw", _isOptimizedDraw );
		}

	#endregion

	#region Rendering Methods

		/// <summary>
		/// Do all rendering associated with this <see cref="Line"/> to the specified
		/// <see cref="Graphics"/> device.  This method is normally only
		/// called by the Draw method of the parent <see cref="LineItem"/> object.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="curve">A <see cref="LineItem"/> representing this
		/// curve.</param>
		public void Draw( Graphics g, GraphPane pane, CurveItem curve, float scaleFactor )
		{
			// If the line is being shown, draw it
			if ( this.IsVisible )
			{
				//How to handle fill vs nofill?
				//if ( isSelected )
				//	GraphPane.Default.SelectedLine.

				SmoothingMode sModeSave = g.SmoothingMode;
				if ( _isAntiAlias )
					g.SmoothingMode = SmoothingMode.HighQuality;

				if ( curve is StickItem )
					DrawSticks( g, pane, curve, scaleFactor );
				else if ( this.IsSmooth || this.Fill.IsVisible )
					DrawSmoothFilledCurve( g, pane, curve, scaleFactor );
				else
					DrawCurve( g, pane, curve, scaleFactor );

				g.SmoothingMode = sModeSave;
			}
		}

		/// <summary>
		/// Render a single <see cref="Line"/> segment to the specified
		/// <see cref="Graphics"/> device.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="x1">The x position of the starting point that defines the
		/// line segment in screen pixel units</param>
		/// <param name="y1">The y position of the starting point that defines the
		/// line segment in screen pixel units</param>
		/// <param name="x2">The x position of the ending point that defines the
		/// line segment in screen pixel units</param>
		/// <param name="y2">The y position of the ending point that defines the
		/// line segment in screen pixel units</param>
		public void DrawSegment( Graphics g, GraphPane pane, float x1, float y1,
								  float x2, float y2, float scaleFactor )
		{
			if ( _isVisible && !this.Color.IsEmpty )
			{
				using ( Pen pen = GetPen( pane, scaleFactor ) )
				{
					g.DrawLine( pen, x1, y1, x2, y2 );
				}
			}
		}

		/// <summary>
		/// Render the <see cref="Line"/>'s as vertical sticks (from a <see cref="StickItem" />) to
		/// the specified <see cref="Graphics"/> device.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="curve">A <see cref="CurveItem"/> representing this
		/// curve.</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		public void DrawSticks( Graphics g, GraphPane pane, CurveItem curve, float scaleFactor )
		{
			Line source = this;
			if ( curve.IsSelected )
				source = Selection.Line;

			Axis yAxis = curve.GetYAxis( pane );
			Axis xAxis = curve.GetXAxis( pane );

			float basePix = yAxis.Scale.Transform( 0.0 );
			using ( Pen pen = source.GetPen( pane, scaleFactor ) )
			{
				for ( int i = 0; i < curve.Points.Count; i++ )
				{
					PointPair pt = curve.Points[i];

					if ( pt.X != PointPair.Missing &&
							pt.Y != PointPair.Missing &&
							!System.Double.IsNaN( pt.X ) &&
							!System.Double.IsNaN( pt.Y ) &&
							!System.Double.IsInfinity( pt.X ) &&
							!System.Double.IsInfinity( pt.Y ) &&
							( !xAxis._scale.IsLog || pt.X > 0.0 ) &&
							( !yAxis._scale.IsLog || pt.Y > 0.0 ) )
					{
						float pixY = yAxis.Scale.Transform( curve.IsOverrideOrdinal, i, pt.Y );
						float pixX = xAxis.Scale.Transform( curve.IsOverrideOrdinal, i, pt.X );

						if ( pixX >= pane.Chart._rect.Left && pixX <= pane.Chart._rect.Right )
						{
							if ( pixY > pane.Chart._rect.Bottom )
								pixY = pane.Chart._rect.Bottom;
							if ( pixY < pane.Chart._rect.Top )
								pixY = pane.Chart._rect.Top;

							if ( !curve.IsSelected && this._gradientFill.IsGradientValueType )
							{
								using ( Pen tPen = GetPen( pane, scaleFactor, pt ) )
									g.DrawLine( tPen, pixX, pixY, pixX, basePix );
							}
							else
								g.DrawLine( pen, pixX, pixY, pixX, basePix );

						}
					}
				}
			}
		}

		/// <summary>
		/// Draw the this <see cref="CurveItem"/> to the specified <see cref="Graphics"/>
		/// device using the specified smoothing property (<see cref="ZedGraph.Line.SmoothTension"/>).
		/// The routine draws the line segments and the area fill (if any, see <see cref="FillType"/>;
		/// the symbols are drawn by the <see cref="Symbol.Draw"/> method.  This method
		/// is normally only called by the Draw method of the
		/// <see cref="CurveItem"/> object.  Note that the <see cref="StepType"/> property
		/// is ignored for smooth lines (e.g., when <see cref="ZedGraph.Line.IsSmooth"/> is true).
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="curve">A <see cref="LineItem"/> representing this
		/// curve.</param>
		public void DrawSmoothFilledCurve( Graphics g, GraphPane pane,
                                CurveItem curve, float scaleFactor )
		{
			Line source = this;
			if ( curve.IsSelected )
				source = Selection.Line;

			PointF[] arrPoints;
			int count;
			IPointList points = curve.Points;

			if ( this.IsVisible && !this.Color.IsEmpty && points != null &&
				BuildPointsArray( pane, curve, out arrPoints, out count ) &&
				count > 2 )
			{
				float tension = _isSmooth ? _smoothTension : 0f;

				// Fill the curve if needed
				if ( this.Fill.IsVisible )
				{
					Axis yAxis = curve.GetYAxis( pane );

					using ( GraphicsPath path = new GraphicsPath( FillMode.Winding ) )
					{
						path.AddCurve( arrPoints, 0, count - 2, tension );

						double yMin = yAxis._scale._min < 0 ? 0.0 : yAxis._scale._min;
						CloseCurve( pane, curve, arrPoints, count, yMin, path );

						RectangleF rect = path.GetBounds();
						using ( Brush brush = source._fill.MakeBrush( rect ) )
						{
							if ( pane.LineType == LineType.Stack && yAxis.Scale._min < 0 &&
									this.IsFirstLine( pane, curve ) )
							{
								float zeroPix = yAxis.Scale.Transform( 0 );
								RectangleF tRect = pane.Chart._rect;
								tRect.Height = zeroPix - tRect.Top;
								if ( tRect.Height > 0 )
								{
									Region reg = g.Clip;
									g.SetClip( tRect );
									g.FillPath( brush, path );
									g.SetClip( pane.Chart._rect );
								}
							}
							else
								g.FillPath( brush, path );
							//brush.Dispose();
						}

						// restore the zero line if needed (since the fill tends to cover it up)
						yAxis.FixZeroLine( g, pane, scaleFactor, rect.Left, rect.Right );
					}
				}

				// If it's a smooth curve, go ahead and render the path.  Otherwise, use the
				// standard drawcurve method just in case there are missing values.
				if ( _isSmooth )
				{
					using ( Pen pen = GetPen( pane, scaleFactor ) )
					{
						// Stroke the curve
						g.DrawCurve( pen, arrPoints, 0, count - 2, tension );

						//pen.Dispose();
					}
				}
				else
					DrawCurve( g, pane, curve, scaleFactor );
			}
		}

		private bool IsFirstLine( GraphPane pane, CurveItem curve )
		{
			CurveList curveList = pane.CurveList;

			for ( int j = 0; j < curveList.Count; j++ )
			{
				CurveItem tCurve = curveList[j];

				if ( tCurve is LineItem && tCurve.IsY2Axis == curve.IsY2Axis &&
						tCurve.YAxisIndex == curve.YAxisIndex )
				{
					return tCurve == curve;
				}
			}

			return false;
		}

		/// <summary>
		/// Draw the this <see cref="CurveItem"/> to the specified <see cref="Graphics"/>
		/// device.  The format (stair-step or line) of the curve is
		/// defined by the <see cref="StepType"/> property.  The routine
		/// only draws the line segments; the symbols are drawn by the
		/// <see cref="Symbol.Draw"/> method.  This method
		/// is normally only called by the Draw method of the
		/// <see cref="CurveItem"/> object
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="curve">A <see cref="LineItem"/> representing this
		/// curve.</param>
		public void DrawCurve( Graphics g, GraphPane pane,
                                CurveItem curve, float scaleFactor )
		{
			Line source = this;
			if ( curve.IsSelected )
				source = Selection.Line;

			// switch to int to optimize drawing speed (per Dale-a-b)
			int	tmpX, tmpY,
					lastX = int.MaxValue,
					lastY = int.MaxValue;

			double curX, curY, lowVal;
            double curveMin = double.MaxValue, curveMax = double.MinValue, curveMean = 0, n = 0;
			PointPair curPt, lastPt = new PointPair();

			bool lastBad = true;
			IPointList points = curve.Points;
			ValueHandler valueHandler = new ValueHandler( pane, false );
			Axis yAxis = curve.GetYAxis( pane );
			Axis xAxis = curve.GetXAxis( pane );

			bool xIsLog = xAxis._scale.IsLog;
			bool yIsLog = yAxis._scale.IsLog;

			// switch to int to optimize drawing speed (per Dale-a-b)
			int minX = (int)pane.Chart.Rect.Left;
			int maxX = (int)pane.Chart.Rect.Right;
			int minY = (int)pane.Chart.Rect.Top;
			int maxY = (int)pane.Chart.Rect.Bottom;

			using ( Pen pen = source.GetPen( pane, scaleFactor ) )
			{
				if ( points != null && !_color.IsEmpty && this.IsVisible )
				{
					//bool lastOut = false;
					bool isOut;

					bool isOptDraw = _isOptimizedDraw && points.Count > 1000;

					// (Dale-a-b) we'll set an element to true when it has been drawn	
					bool[,] isPixelDrawn = null;
					
					if ( isOptDraw )
						isPixelDrawn = new bool[maxX + 1, maxY + 1]; 
					
					// Loop over each point in the curve
					for ( int i = 0; i < points.Count; i++ )
					{
						curPt = points[i];
						if ( pane.LineType == LineType.Stack )
						{
							if ( !valueHandler.GetValues( curve, i, out curX, out lowVal, out curY ) )
							{
								curX = PointPair.Missing;
								curY = PointPair.Missing;
							}
						}
						else
						{
							curX = curPt.X;
							curY = curPt.Y;
						}

						// Any value set to double max is invalid and should be skipped
						// This is used for calculated values that are out of range, divide
						//   by zero, etc.
						// Also, any value <= zero on a log scale is invalid
						if ( curX == PointPair.Missing ||
								curY == PointPair.Missing ||
								System.Double.IsNaN( curX ) ||
								System.Double.IsNaN( curY ) ||
								System.Double.IsInfinity( curX ) ||
								System.Double.IsInfinity( curY ) ||
								( xIsLog && curX <= 0.0 ) ||
								( yIsLog && curY <= 0.0 ) )
						{
							// If the point is invalid, then make a linebreak only if IsIgnoreMissing is false
							// LastX and LastY are always the last valid point, so this works out
							lastBad = lastBad || !pane.IsIgnoreMissing;
							isOut = true;
						}
						else
						{
							// Transform the current point from user scale units to
							// screen coordinates
							tmpX = (int) xAxis.Scale.Transform( curve.IsOverrideOrdinal, i, curX );
							tmpY = (int) yAxis.Scale.Transform( curve.IsOverrideOrdinal, i, curY );

							// Maintain an array of "used" pixel locations to avoid duplicate drawing operations
							// contributed by Dale-a-b
							if ( isOptDraw && tmpX >= minX && tmpX <= maxX &&
										tmpY >= minY && tmpY <= maxY ) // guard against the zoom-in case
							{
								if ( isPixelDrawn[tmpX, tmpY] )
									continue;
								isPixelDrawn[tmpX, tmpY] = true;
							}

							isOut = ( tmpX < minX && lastX < minX ) || ( tmpX > maxX && lastX > maxX ) ||
								( tmpY < minY && lastY < minY ) || ( tmpY > maxY && lastY > maxY );

							if ( !lastBad )
							{
								try
								{
									// GDI+ plots the data wrong and/or throws an exception for
									// outrageous coordinates, so we do a sanity check here
									if ( lastX > 5000000 || lastX < -5000000 ||
											lastY > 5000000 || lastY < -5000000 ||
											tmpX > 5000000 || tmpX < -5000000 ||
											tmpY > 5000000 || tmpY < -5000000 )
										InterpolatePoint( g, pane, curve, lastPt, scaleFactor, pen,
														lastX, lastY, tmpX, tmpY );
									else if ( !isOut )
									{
                                        n++;
                                        curveMin = Math.Min(curveMin, curY);
                                        curveMax = Math.Max(curveMax, curY);
                                        curveMean = curveMean * (n - 1) + curY;
                                        curveMean = curveMean / n;

										if ( !curve.IsSelected && this._gradientFill.IsGradientValueType )
										{
											using ( Pen tPen = GetPen( pane, scaleFactor, lastPt ) )
											{
												if ( this.StepType == StepType.NonStep )
												{
													g.DrawLine( tPen, lastX, lastY, tmpX, tmpY );
												}
												else if ( this.StepType == StepType.ForwardStep )
												{
													g.DrawLine( tPen, lastX, lastY, tmpX, lastY );
													g.DrawLine( tPen, tmpX, lastY, tmpX, tmpY );
												}
												else if ( this.StepType == StepType.RearwardStep )
												{
													g.DrawLine( tPen, lastX, lastY, lastX, tmpY );
													g.DrawLine( tPen, lastX, tmpY, tmpX, tmpY );
												}
												else if ( this.StepType == StepType.ForwardSegment )
												{
													g.DrawLine( tPen, lastX, lastY, tmpX, lastY );
												}
												else
												{
													g.DrawLine( tPen, lastX, tmpY, tmpX, tmpY );
												}
											}
										}
										else
										{
											if ( this.StepType == StepType.NonStep )
											{
												g.DrawLine( pen, lastX, lastY, tmpX, tmpY );
											}
											else if ( this.StepType == StepType.ForwardStep )
											{
												g.DrawLine( pen, lastX, lastY, tmpX, lastY );
												g.DrawLine( pen, tmpX, lastY, tmpX, tmpY );
											}
											else if ( this.StepType == StepType.RearwardStep )
											{
												g.DrawLine( pen, lastX, lastY, lastX, tmpY );
												g.DrawLine( pen, lastX, tmpY, tmpX, tmpY );
											}
											else if ( this.StepType == StepType.ForwardSegment )
											{
												g.DrawLine( pen, lastX, lastY, tmpX, lastY );
											}
											else if ( this.StepType == StepType.RearwardSegment )
											{
												g.DrawLine( pen, lastX, tmpY, tmpX, tmpY );
											}
										}
									}

								}
								catch
								{
									InterpolatePoint( g, pane, curve, lastPt, scaleFactor, pen,
												lastX, lastY, tmpX, tmpY );
								}

							}

							lastPt = curPt;
							lastX = tmpX;
							lastY = tmpY;
							lastBad = false;
							//lastOut = isOut;
						}
					}
				}
			}

            if (n > 0)
            {
                string label = curve.Label.Text;
                int idx = curve.Label.Text.IndexOf(" (Min: ");
                if (idx > 0)
                    label = label.Substring(0, idx);
                // If CurveItem has units associated with the line, append it to the end of the displayed curve line
                if (_unitList.ContainsKey(label))
                {
                    curve.Label.Text = label + " (Min: " + curveMin.ToString("0") + " Max: " + curveMax.ToString("0") + " Mean: " + curveMean.ToString("0") + ") " + _unitList[label];
                }
                else
                {
                    curve.Label.Text = label + " (Min: " + curveMin.ToString("0") + " Max: " + curveMax.ToString("0") + " Mean: " + curveMean.ToString("0") + ")";
                }
            }
		}

		/// <summary>
		/// Draw the this <see cref="CurveItem"/> to the specified <see cref="Graphics"/>
		/// device.  The format (stair-step or line) of the curve is
		/// defined by the <see cref="StepType"/> property.  The routine
		/// only draws the line segments; the symbols are drawn by the
		/// <see cref="Symbol.Draw"/> method.  This method
		/// is normally only called by the Draw method of the
		/// <see cref="CurveItem"/> object
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="curve">A <see cref="LineItem"/> representing this
		/// curve.</param>
		public void DrawCurveOriginal( Graphics g, GraphPane pane,
										  CurveItem curve, float scaleFactor )
		{
			Line source = this;
			if ( curve.IsSelected )
				source = Selection.Line;

			float tmpX, tmpY,
					lastX = float.MaxValue,
					lastY = float.MaxValue;
			double curX, curY, lowVal;
			PointPair curPt, lastPt = new PointPair();

			bool lastBad = true;
			IPointList points = curve.Points;
			ValueHandler valueHandler = new ValueHandler( pane, false );
			Axis yAxis = curve.GetYAxis( pane );
			Axis xAxis = curve.GetXAxis( pane );

			bool xIsLog = xAxis._scale.IsLog;
			bool yIsLog = yAxis._scale.IsLog;

			float minX = pane.Chart.Rect.Left;
			float maxX = pane.Chart.Rect.Right;
			float minY = pane.Chart.Rect.Top;
			float maxY = pane.Chart.Rect.Bottom;

			using ( Pen pen = source.GetPen( pane, scaleFactor ) )
			{
				if ( points != null && !_color.IsEmpty && this.IsVisible )
				{
					//bool lastOut = false;
					bool isOut;

					// Loop over each point in the curve
					for ( int i = 0; i < points.Count; i++ )
					{
						curPt = points[i];
						if ( pane.LineType == LineType.Stack )
						{
							if ( !valueHandler.GetValues( curve, i, out curX, out lowVal, out curY ) )
							{
								curX = PointPair.Missing;
								curY = PointPair.Missing;
							}
						}
						else
						{
							curX = curPt.X;
							curY = curPt.Y;
						}

						// Any value set to double max is invalid and should be skipped
						// This is used for calculated values that are out of range, divide
						//   by zero, etc.
						// Also, any value <= zero on a log scale is invalid
						if ( curX == PointPair.Missing ||
								curY == PointPair.Missing ||
								System.Double.IsNaN( curX ) ||
								System.Double.IsNaN( curY ) ||
								System.Double.IsInfinity( curX ) ||
								System.Double.IsInfinity( curY ) ||
								( xIsLog && curX <= 0.0 ) ||
								( yIsLog && curY <= 0.0 ) )
						{
							// If the point is invalid, then make a linebreak only if IsIgnoreMissing is false
							// LastX and LastY are always the last valid point, so this works out
							lastBad = lastBad || !pane.IsIgnoreMissing;
							isOut = true;
						}
						else
						{
							// Transform the current point from user scale units to
							// screen coordinates
							tmpX = xAxis.Scale.Transform( curve.IsOverrideOrdinal, i, curX );
							tmpY = yAxis.Scale.Transform( curve.IsOverrideOrdinal, i, curY );
							isOut = ( tmpX < minX && lastX < minX ) || ( tmpX > maxX && lastX > maxX ) ||
								( tmpY < minY && lastY < minY ) || ( tmpY > maxY && lastY > maxY );

							if ( !lastBad )
							{
								try
								{
									// GDI+ plots the data wrong and/or throws an exception for
									// outrageous coordinates, so we do a sanity check here
									if ( lastX > 5000000 || lastX < -5000000 ||
											lastY > 5000000 || lastY < -5000000 ||
											tmpX > 5000000 || tmpX < -5000000 ||
											tmpY > 5000000 || tmpY < -5000000 )
										InterpolatePoint( g, pane, curve, lastPt, scaleFactor, pen,
														lastX, lastY, tmpX, tmpY );
									else if ( !isOut )
									{
										if ( !curve.IsSelected && this._gradientFill.IsGradientValueType )
										{
											using ( Pen tPen = GetPen( pane, scaleFactor, lastPt ) )
											{
												if ( this.StepType == StepType.NonStep )
												{
													g.DrawLine( tPen, lastX, lastY, tmpX, tmpY );
												}
												else if ( this.StepType == StepType.ForwardStep )
												{
													g.DrawLine( tPen, lastX, lastY, tmpX, lastY );
													g.DrawLine( tPen, tmpX, lastY, tmpX, tmpY );
												}
												else if ( this.StepType == StepType.RearwardStep )
												{
													g.DrawLine( tPen, lastX, lastY, lastX, tmpY );
													g.DrawLine( tPen, lastX, tmpY, tmpX, tmpY );
												}
												else if ( this.StepType == StepType.ForwardSegment )
												{
													g.DrawLine( tPen, lastX, lastY, tmpX, lastY );
												}
												else
												{
													g.DrawLine( tPen, lastX, tmpY, tmpX, tmpY );
												}
											}
										}
										else
										{
											if ( this.StepType == StepType.NonStep )
											{
												g.DrawLine( pen, lastX, lastY, tmpX, tmpY );
											}
											else if ( this.StepType == StepType.ForwardStep )
											{
												g.DrawLine( pen, lastX, lastY, tmpX, lastY );
												g.DrawLine( pen, tmpX, lastY, tmpX, tmpY );
											}
											else if ( this.StepType == StepType.RearwardStep )
											{
												g.DrawLine( pen, lastX, lastY, lastX, tmpY );
												g.DrawLine( pen, lastX, tmpY, tmpX, tmpY );
											}
											else if ( this.StepType == StepType.ForwardSegment )
											{
												g.DrawLine( pen, lastX, lastY, tmpX, lastY );
											}
											else if ( this.StepType == StepType.RearwardSegment )
											{
												g.DrawLine( pen, lastX, tmpY, tmpX, tmpY );
											}
										}
									}

								}
								catch
								{
									InterpolatePoint( g, pane, curve, lastPt, scaleFactor, pen,
												lastX, lastY, tmpX, tmpY );
								}

							}

							lastPt = curPt;
							lastX = tmpX;
							lastY = tmpY;
							lastBad = false;
							//lastOut = isOut;
						}
					}
				}
			}
		}

		/// <summary>
		/// This method just handles the case where one or more of the coordinates are outrageous,
		/// or GDI+ threw an exception.  This method attempts to correct the outrageous coordinates by
		/// interpolating them to a point (along the original line) that lies at the edge of the ChartRect
		/// so that GDI+ will handle it properly.  GDI+ will throw an exception, or just plot the data
		/// incorrectly if the coordinates are too large (empirically, this appears to be when the
		/// coordinate value is greater than 5,000,000 or less than -5,000,000).  Although you typically
		/// would not see coordinates like this, if you repeatedly zoom in on a ZedGraphControl, eventually
		/// all your points will be way outside the bounds of the plot.
		/// </summary>
		private void InterpolatePoint( Graphics g, GraphPane pane, CurveItem curve, PointPair lastPt,
						float scaleFactor, Pen pen, float lastX, float lastY, float tmpX, float tmpY )
		{
			try
			{
				RectangleF chartRect = pane.Chart._rect;
				// try to interpolate values
				bool lastIn = chartRect.Contains( lastX, lastY );
				bool curIn = chartRect.Contains( tmpX, tmpY );

				// If both points are outside the ChartRect, make a new point that is on the LastX/Y
				// side of the ChartRect, and fall through to the code that handles lastIn == true
				if ( !lastIn )
				{
					float newX, newY;

					if ( Math.Abs( lastX ) > Math.Abs( lastY ) )
					{
						newX = lastX < 0 ? chartRect.Left : chartRect.Right;
						newY = lastY + ( tmpY - lastY ) * ( newX - lastX ) / ( tmpX - lastX );
					}
					else
					{
						newY = lastY < 0 ? chartRect.Top : chartRect.Bottom;
						newX = lastX + ( tmpX - lastX ) * ( newY - lastY ) / ( tmpY - lastY );
					}

					lastX = newX;
					lastY = newY;
				}

				if ( !curIn )
				{
					float newX, newY;

					if ( Math.Abs( tmpX ) > Math.Abs( tmpY ) )
					{
						newX = tmpX < 0 ? chartRect.Left : chartRect.Right;
						newY = tmpY + ( lastY - tmpY ) * ( newX - tmpX ) / ( lastX - tmpX );
					}
					else
					{
						newY = tmpY < 0 ? chartRect.Top : chartRect.Bottom;
						newX = tmpX + ( lastX - tmpX ) * ( newY - tmpY ) / ( lastY - tmpY );
					}

					tmpX = newX;
					tmpY = newY;
				}

                if (float.IsInfinity(tmpX) || float.IsInfinity(tmpY) || float.IsInfinity(lastX) || float.IsInfinity(lastY))
                    return;

				/*
				if ( this.StepType == StepType.ForwardStep )
				{
					g.DrawLine( pen, lastX, lastY, tmpX, lastY );
					g.DrawLine( pen, tmpX, lastY, tmpX, tmpY );
				}
				else if ( this.StepType == StepType.RearwardStep )
				{
					g.DrawLine( pen, lastX, lastY, lastX, tmpY );
					g.DrawLine( pen, lastX, tmpY, tmpX, tmpY );
				}
				else 		// non-step
					g.DrawLine( pen, lastX, lastY, tmpX, tmpY );
				*/
				if ( !curve.IsSelected && this._gradientFill.IsGradientValueType )
				{
					using ( Pen tPen = GetPen( pane, scaleFactor, lastPt ) )
					{
						if ( this.StepType == StepType.NonStep )
						{
							g.DrawLine( tPen, lastX, lastY, tmpX, tmpY );
						}
						else if ( this.StepType == StepType.ForwardStep )
						{
							g.DrawLine( tPen, lastX, lastY, tmpX, lastY );
							g.DrawLine( tPen, tmpX, lastY, tmpX, tmpY );
						}
						else if ( this.StepType == StepType.RearwardStep )
						{
							g.DrawLine( tPen, lastX, lastY, lastX, tmpY );
							g.DrawLine( tPen, lastX, tmpY, tmpX, tmpY );
						}
						else if ( this.StepType == StepType.ForwardSegment )
						{
							g.DrawLine( tPen, lastX, lastY, tmpX, lastY );
						}
						else
						{
							g.DrawLine( tPen, lastX, tmpY, tmpX, tmpY );
						}
					}
				}
				else
				{
					if ( this.StepType == StepType.NonStep )
					{
						g.DrawLine( pen, lastX, lastY, tmpX, tmpY );
					}
					else if ( this.StepType == StepType.ForwardStep )
					{
						g.DrawLine( pen, lastX, lastY, tmpX, lastY );
						g.DrawLine( pen, tmpX, lastY, tmpX, tmpY );
					}
					else if ( this.StepType == StepType.RearwardStep )
					{
						g.DrawLine( pen, lastX, lastY, lastX, tmpY );
						g.DrawLine( pen, lastX, tmpY, tmpX, tmpY );
					}
					else if ( this.StepType == StepType.ForwardSegment )
					{
						g.DrawLine( pen, lastX, lastY, tmpX, lastY );
					}
					else if ( this.StepType == StepType.RearwardSegment )
					{
						g.DrawLine( pen, lastX, tmpY, tmpX, tmpY );
					}
				}

			}

			catch { }
		}

		/// <summary>
		/// Build an array of <see cref="PointF"/> values (pixel coordinates) that represents
		/// the current curve.  Note that this drawing routine ignores <see cref="PointPairBase.Missing"/>
		/// values, but it does not "break" the line to indicate values are missing.
		/// </summary>
		/// <param name="pane">A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.</param>
		/// <param name="curve">A <see cref="LineItem"/> representing this
		/// curve.</param>
		/// <param name="arrPoints">An array of <see cref="PointF"/> values in pixel
		/// coordinates representing the current curve.</param>
		/// <param name="count">The number of points contained in the "arrPoints"
		/// parameter.</param>
		/// <returns>true for a successful points array build, false for data problems</returns>
		public bool BuildPointsArray( GraphPane pane, CurveItem curve,
			out PointF[] arrPoints, out int count )
		{
			arrPoints = null;
			count = 0;
			IPointList points = curve.Points;

			if ( this.IsVisible && !this.Color.IsEmpty && points != null )
			{
				int index = 0;
				float curX, curY,
							lastX = 0,
							lastY = 0;
				double x, y, lowVal;
				ValueHandler valueHandler = new ValueHandler( pane, false );

				// Step type plots get twice as many points.  Always add three points so there is
				// room to close out the curve for area fills.
				arrPoints = new PointF[( _stepType == ZedGraph.StepType.NonStep ? 1 : 2 ) *
											points.Count + 1];

				// Loop over all points in the curve
				for ( int i = 0; i < points.Count; i++ )
				{
					// make sure that the current point is valid
					if ( !points[i].IsInvalid )
					{
						// Get the user scale values for the current point
						// use the valueHandler only for stacked types
						if ( pane.LineType == LineType.Stack )
						{
							valueHandler.GetValues( curve, i, out x, out lowVal, out y );
						}
						// otherwise, just access the values directly.  Avoiding the valueHandler for
						// non-stacked types is an optimization to minimize overhead in case there are
						// a large number of points.
						else
						{
							x = points[i].X;
							y = points[i].Y;
						}

						if ( x == PointPair.Missing || y == PointPair.Missing )
							continue;

						// Transform the user scale values to pixel locations
						Axis xAxis = curve.GetXAxis( pane );
						curX = xAxis.Scale.Transform( curve.IsOverrideOrdinal, i, x );
						Axis yAxis = curve.GetYAxis( pane );
						curY = yAxis.Scale.Transform( curve.IsOverrideOrdinal, i, y );

						if ( curX < -1000000 || curY < -1000000 || curX > 1000000 || curY > 1000000 )
							continue;

						// Add the pixel value pair into the points array
						// Two points are added for step type curves
						// ignore step-type setting for smooth curves
						if ( _isSmooth || index == 0 || this.StepType == StepType.NonStep )
						{
							arrPoints[index].X = curX;
							arrPoints[index].Y = curY;
						}
						else if ( this.StepType == StepType.ForwardStep ||
										this.StepType == StepType.ForwardSegment )
						{
							arrPoints[index].X = curX;
							arrPoints[index].Y = lastY;
							index++;
							arrPoints[index].X = curX;
							arrPoints[index].Y = curY;
						}
						else if ( this.StepType == StepType.RearwardStep ||
										this.StepType == StepType.RearwardSegment )
						{
							arrPoints[index].X = lastX;
							arrPoints[index].Y = curY;
							index++;
							arrPoints[index].X = curX;
							arrPoints[index].Y = curY;
						}

						lastX = curX;
						lastY = curY;
						index++;

					}

				}

				// Make sure there is at least one valid point
				if ( index == 0 )
					return false;

				// Add an extra point at the end, since the smoothing algorithm requires it
				arrPoints[index] = arrPoints[index - 1];
				index++;

				count = index;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Build an array of <see cref="PointF"/> values (pixel coordinates) that represents
		/// the low values for the current curve.
		/// </summary>
		/// <remarks>Note that this drawing routine ignores <see cref="PointPairBase.Missing"/>
		/// values, but it does not "break" the line to indicate values are missing.
		/// </remarks>
		/// <param name="pane">A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.</param>
		/// <param name="curve">A <see cref="LineItem"/> representing this
		/// curve.</param>
		/// <param name="arrPoints">An array of <see cref="PointF"/> values in pixel
		/// coordinates representing the current curve.</param>
		/// <param name="count">The number of points contained in the "arrPoints"
		/// parameter.</param>
		/// <returns>true for a successful points array build, false for data problems</returns>
		public bool BuildLowPointsArray( GraphPane pane, CurveItem curve,
						out PointF[] arrPoints, out int count )
		{
			arrPoints = null;
			count = 0;
			IPointList points = curve.Points;

			if ( this.IsVisible && !this.Color.IsEmpty && points != null )
			{
				int index = 0;
				float curX, curY,
						lastX = 0,
						lastY = 0;
				double x, y, hiVal;
				ValueHandler valueHandler = new ValueHandler( pane, false );

				// Step type plots get twice as many points.  Always add three points so there is
				// room to close out the curve for area fills.
				arrPoints = new PointF[( _stepType == ZedGraph.StepType.NonStep ? 1 : 2 ) *
					( pane.LineType == LineType.Stack ? 2 : 1 ) *
					points.Count + 1];

				// Loop backwards over all points in the curve
				// In this case an array of points was already built forward by BuildPointsArray().
				// This time we build backwards to complete a loop around the area between two curves.
				for ( int i = points.Count - 1; i >= 0; i-- )
				{
					// Make sure the current point is valid
					if ( !points[i].IsInvalid )
					{
						// Get the user scale values for the current point
						valueHandler.GetValues( curve, i, out x, out y, out hiVal );

						if ( x == PointPair.Missing || y == PointPair.Missing )
							continue;

						// Transform the user scale values to pixel locations
						Axis xAxis = curve.GetXAxis( pane );
						curX = xAxis.Scale.Transform( curve.IsOverrideOrdinal, i, x );
						Axis yAxis = curve.GetYAxis( pane );
						curY = yAxis.Scale.Transform( curve.IsOverrideOrdinal, i, y );

						// Add the pixel value pair into the points array
						// Two points are added for step type curves
						// ignore step-type setting for smooth curves
						if ( _isSmooth || index == 0 || this.StepType == StepType.NonStep )
						{
							arrPoints[index].X = curX;
							arrPoints[index].Y = curY;
						}
						else if ( this.StepType == StepType.ForwardStep )
						{
							arrPoints[index].X = curX;
							arrPoints[index].Y = lastY;
							index++;
							arrPoints[index].X = curX;
							arrPoints[index].Y = curY;
						}
						else if ( this.StepType == StepType.RearwardStep )
						{
							arrPoints[index].X = lastX;
							arrPoints[index].Y = curY;
							index++;
							arrPoints[index].X = curX;
							arrPoints[index].Y = curY;
						}

						lastX = curX;
						lastY = curY;
						index++;

					}

				}

				// Make sure there is at least one valid point
				if ( index == 0 )
					return false;

				// Add an extra point at the end, since the smoothing algorithm requires it
				arrPoints[index] = arrPoints[index - 1];
				index++;

				count = index;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Close off a <see cref="GraphicsPath"/> that defines a curve
		/// </summary>
		/// <param name="pane">A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.</param>
		/// <param name="curve">A <see cref="LineItem"/> representing this
		/// curve.</param>
		/// <param name="arrPoints">An array of <see cref="PointF"/> values in screen pixel
		/// coordinates representing the current curve.</param>
		/// <param name="count">The number of points contained in the "arrPoints"
		/// parameter.</param>
		/// <param name="yMin">The Y axis value location where the X axis crosses.</param>
		/// <param name="path">The <see cref="GraphicsPath"/> class that represents the curve.</param>
		public void CloseCurve( GraphPane pane, CurveItem curve, PointF[] arrPoints,
									int count, double yMin, GraphicsPath path )
		{
			// For non-stacked lines, the fill area is just the area between the curve and the X axis
			if ( pane.LineType != LineType.Stack )
			{
				// Determine the current value for the bottom of the curve (usually the Y value where
				// the X axis crosses)
				float yBase;
				Axis yAxis = curve.GetYAxis( pane );
				yBase = yAxis.Scale.Transform( yMin );

				// Add three points to the path to move from the end of the curve (as defined by
				// arrPoints) to the X axis, from there to the start of the curve at the X axis,
				// and from there back up to the beginning of the curve.
				path.AddLine( arrPoints[count - 1].X, arrPoints[count - 1].Y, arrPoints[count - 1].X, yBase );
				path.AddLine( arrPoints[count - 1].X, yBase, arrPoints[0].X, yBase );
				path.AddLine( arrPoints[0].X, yBase, arrPoints[0].X, arrPoints[0].Y );
			}
			// For stacked line types, the fill area is the area between this curve and the curve below it
			else
			{
				PointF[] arrPoints2;
				int count2;

				float tension = _isSmooth ? _smoothTension : 0f;

				// Find the next lower curve in the curveList that is also a LineItem type, and use
				// its smoothing properties for the lower side of the filled area.
				int index = pane.CurveList.IndexOf( curve );
				if ( index > 0 )
				{
					CurveItem tmpCurve;
					for ( int i = index - 1; i >= 0; i-- )
					{
						tmpCurve = pane.CurveList[i];
						if ( tmpCurve is LineItem )
						{
							tension = ( (LineItem)tmpCurve ).Line.IsSmooth ? ( (LineItem)tmpCurve ).Line.SmoothTension : 0f;
							break;
						}
					}
				}

				// Build another points array consisting of the low points (which are actually the points for
				// the curve below the current curve)
				BuildLowPointsArray( pane, curve, out arrPoints2, out count2 );

				// Add the new points to the GraphicsPath
				path.AddCurve( arrPoints2, 0, count2 - 2, tension );
			}

		}

	#endregion

	}
}
