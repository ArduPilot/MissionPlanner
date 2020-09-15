﻿using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Attributes;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace MissionPlanner
{
    public class CurrentState : ICloneable, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [JsonIgnore] [IgnoreDataMember] public static ISpeech Speech;

        // multipliers
        public static float multiplierdist = 1;
        public static string DistanceUnit = "";
        public static float multiplierspeed = 1;
        public static string SpeedUnit = "";
        public static float multiplieralt = 1;
        public static string AltUnit = "";

        private PointLatLngAlt _homelocation = new PointLatLngAlt();
        private static PointLatLngAlt _plannedhomelocation = new PointLatLngAlt();

        private static PointLatLngAlt _trackerloc = new PointLatLngAlt();

        public static int rateattitudebackup;
        public static int ratepositionbackup;
        public static int ratestatusbackup;
        public static int ratesensorsbackup;
        public static int ratercbackup;

        public static int KIndexstatic = -1;
        private float _airspeed;

        private float _alt;
        private float _alt_error;

        private float _altasl;
        private float _aspd_error;

        private int _battery_remaining;

        internal double _battery_voltage;

        internal double _battery_voltage2;

        private float _ch3percent = -1;
        private float _climbrate;
        private double _current;

        private double _current2;


        private float _groundcourse;
        private float _groundspeed;

        private DateTime _lastcurrent = DateTime.MinValue;


        private DateTime _lastcurrent2 = DateTime.MinValue;
        private float _localsnrdb;

        private uint _mode = 99999;

        private PointLatLngAlt _movingbase = new PointLatLngAlt();
        private float _remotesnrdb;

        private float _sonarrange;

        private float _ter_alt;

        private float _ter_curalt;
        private float _verticalspeed;

        private float _wpdist;

        private float _yaw;
        internal bool batterymonitoring = false;

        // current firmware
        public Firmwares firmware = Firmwares.ArduCopter2;
        private bool gotwind;

        // HIL
        public int hilch1; // { get; set; }
        public int hilch2; // { get; set; }
        public int hilch3; // { get; set; }
        public int hilch4; // { get; set; }
        public int hilch5;
        public int hilch6;
        public int hilch7;
        public int hilch8;

        internal double imutime;

        private DateTime lastalt = DateTime.MinValue;

        public int lastautowp = -1;

        private DateTime lastdata = DateTime.MinValue;

        // for calc of sitl speedup
        internal DateTime lastimutime = DateTime.MinValue;
        private PointLatLngAlt lastpos = new PointLatLngAlt();
        private DateTime lastremrssi = DateTime.Now;
        private DateTime lastrssi = DateTime.Now;

        private DateTime lastsecondcounter = DateTime.Now;

        private DateTime lastupdate = DateTime.Now;

        internal bool MONO;
        private float oldalt;
        private MAVState _parent;

        [JsonIgnore]
        [IgnoreDataMember]
        public MAVState parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (parent != null)
                    if (parent.parent != null)
                        parent.parent.OnPacketReceived += Parent_OnPacketReceived;
            }
        }

        // rc override
        public short rcoverridech1; //{ get; set; }
        public short rcoverridech10; // { get; set; }
        public short rcoverridech11; //{ get; set; }
        public short rcoverridech12; //{ get; set; }
        public short rcoverridech13; // { get; set; }
        public short rcoverridech14; // { get; set; }
        public short rcoverridech15; // { get; set; }
        public short rcoverridech16; // { get; set; }
        public short rcoverridech17; // { get; set; }
        public short rcoverridech18; // { get; set; }
        public short rcoverridech2; // { get; set; }
        public short rcoverridech3; //{ get; set; }
        public short rcoverridech4; //{ get; set; }
        public short rcoverridech5; // { get; set; }
        public short rcoverridech6; // { get; set; }
        public short rcoverridech7; // { get; set; }
        public short rcoverridech8; // { get; set; }
        public short rcoverridech9; //{ get; set; }

        public Mavlink_Sensors sensors_enabled = new Mavlink_Sensors();
        public Mavlink_Sensors sensors_health = new Mavlink_Sensors();
        public Mavlink_Sensors sensors_present = new Mavlink_Sensors();

        private bool useLocation;

        /// <summary>
        ///     used for wind calc
        /// </summary>
        private double We_fgo;

        /// <summary>
        ///     used in wind calc
        /// </summary>
        private double Wn_fgo;

        static CurrentState()
        {
            // set default telemrates
            rateattitudebackup = 4;
            ratepositionbackup = 2;
            ratestatusbackup = 2;
            ratesensorsbackup = 2;
            ratercbackup = 2;
            //Init dictionary for storing names for customfields
            custom_field_names = new Dictionary<string, string>();
        }

        ~CurrentState()
        {
            log.Info("CurrentState .dtor");
            Dispose();
        }

        public void Dispose()
        {
            log.Info("CurrentState Dispose");
            if (parent != null)
                if (parent.parent != null)
                    parent.parent.OnPacketReceived -= Parent_OnPacketReceived;
        }

        public CurrentState()
        {
            ResetInternals();

            var t = Type.GetType("Mono.Runtime");
            MONO = t != null;
        }

        // propery name, Name   Name starts with MAV_ will link to named_value_float messages
        public static Dictionary<string, string> custom_field_names;

        public float customfield0 { get; set; }
        public float customfield1 { get; set; }
        public float customfield2 { get; set; }
        public float customfield3 { get; set; }
        public float customfield4 { get; set; }
        public float customfield5 { get; set; }
        public float customfield6 { get; set; }
        public float customfield7 { get; set; }
        public float customfield8 { get; set; }
        public float customfield9 { get; set; }

        // orientation - rads
        [DisplayText("Roll (deg)")]
        [GroupText("Attitude")]
        public float roll { get; set; }

        [GroupText("Attitude")]
        [DisplayText("Pitch (deg)")]
        public float pitch { get; set; }

        [GroupText("Attitude")]
        [DisplayText("Yaw (deg)")]
        public float yaw
        {
            get => _yaw;
            set
            {
                if (value < 0)
                    _yaw = value + 360;
                else
                    _yaw = value;
            }
        }

        [GroupText("Attitude")]
        [DisplayText("SSA (deg)")]
        public float SSA { get; set; }

        [GroupText("Attitude")]
        [DisplayText("AOA (deg)")]
        public float AOA { get; set; }

        [GroupText("Position")]
        [DisplayText("GroundCourse (deg)")]
        public float groundcourse
        {
            get => _groundcourse;
            set
            {
                if (value < 0)
                    _groundcourse = value + 360;
                else
                    _groundcourse = value;
            }
        }

        // position
        [DisplayText("Latitude (dd)")]
        [GroupText("Position")]
        public double lat { get; set; }

        [GroupText("Position")]
        [DisplayText("Longitude (dd)")]
        public double lng { get; set; }

        [GroupText("Position")]
        [DisplayText("Altitude (alt)")]
        public float alt
        {
            get => (_alt - altoffsethome) * multiplieralt;
            set
            {
                // check update rate, and ensure time hasnt gone backwards                
                _alt = value;

                if ((datetime - lastalt).TotalSeconds >= 0.2 && oldalt != alt || lastalt > datetime)
                {
                    climbrate = (alt - oldalt) / (float)(datetime - lastalt).TotalSeconds;
                    verticalspeed = (alt - oldalt) / (float)(datetime - lastalt).TotalSeconds;
                    if (float.IsInfinity(_verticalspeed))
                        _verticalspeed = 0;
                    lastalt = datetime;
                    oldalt = alt;
                }
            }
        }

        [GroupText("Position")]
        [DisplayText("Altitude (dist)")]
        public float altasl
        {
            get => _altasl * multiplieralt;
            set => _altasl = value;
        }

        [GroupText("Position")]
        [DisplayText("Horizon Dist (dist)")]
        public float horizondist => (float)(3570 * Math.Sqrt(alt)) * multiplierdist;

        [GroupText("Position")]
        [DisplayText("Velocity X (ms)")]
        public double vx { get; set; }

        [DisplayText("Velocity Y (ms)")]
        [GroupText("Position")]
        public double vy { get; set; }

        [DisplayText("Velocity Z (ms)")]
        [GroupText("Position")]
        public double vz { get; set; }

        [GroupText("Position")] public double vlen => Math.Sqrt(Math.Pow(vx, 2) + Math.Pow(vy, 2) + Math.Pow(vz, 2));

        [GroupText("Position")]
        [DisplayText("Alt Home Offset (dist)")]
        public float altoffsethome { get; set; }

        [GroupText("Position")]
        [DisplayText("Gps Status")]
        public float gpsstatus { get; set; }

        [DisplayText("Gps HDOP")]
        [GroupText("Position")]
        public float gpshdop { get; set; }

        [DisplayText("Sat Count")]
        [GroupText("Position")]
        public float satcount { get; set; }

        [DisplayText("Horizontal Accuracy")]
        [GroupText("Position")]
        public float gpsh_acc { get; private set; }

        [DisplayText("Vertical Accuracy")]
        [GroupText("Position")]
        public float gpsv_acc { get; private set; }

        [DisplayText("Velocity Accuracy")]
        [GroupText("Position")]
        public float gpsvel_acc { get; private set; }

        [DisplayText("Heading Accuracy")]
        [GroupText("Position")]
        public float gpshdg_acc { get; private set; }

        [DisplayText("Latitude2 (dd)")]
        [GroupText("Position")]
        public double lat2 { get; set; }

        [DisplayText("Longitude2 (dd)")]
        [GroupText("Position")]
        public double lng2 { get; set; }

        [DisplayText("Altitude2 (dist)")]
        [GroupText("Position")]
        public float altasl2 { get; set; }

        [DisplayText("Gps Status2")]
        [GroupText("Position")]
        public float gpsstatus2 { get; set; }

        [DisplayText("Gps HDOP2")]
        [GroupText("Position")]
        public float gpshdop2 { get; set; }

        [DisplayText("Sat Count2")]
        [GroupText("Position")]
        public float satcount2 { get; set; }

        [GroupText("Position")]
        public float groundspeed2 { get; set; }

        [DisplayText("GroundCourse2 (deg)")]
        [GroupText("Position")]
        public float groundcourse2 { get; set; }

        [DisplayText("Sat Count Blend")]
        [GroupText("Position")]
        public float satcountB => satcount + satcount2;

        [GroupText("Position")]
        public DateTime gpstime { get; set; }

        public float altd1000 => alt / 1000 % 10;

        public float altd100 => alt / 100 % 10;

        // speeds
        [DisplayText("AirSpeed (speed)")]
        public float airspeed
        {
            get => _airspeed * multiplierspeed;
            set => _airspeed = value;
        }

        [DisplayText("Airspeed Target (speed)")]
        public float targetairspeed { get; private set; }

        public bool lowairspeed { get; set; }

        [DisplayText("Airspeed Ratio")]
        public float asratio { get; set; }

        [GroupText("Position")]
        [DisplayText("GroundSpeed (speed)")]
        public float groundspeed
        {
            get => _groundspeed * multiplierspeed;
            set => _groundspeed = value;
        }

        // accel
        [DisplayText("Accel X")]
        [GroupText("Sensor")]
        public float ax { get; set; }

        [DisplayText("Accel Y")]
        [GroupText("Sensor")]
        public float ay { get; set; }

        [DisplayText("Accel Z")]
        [GroupText("Sensor")]
        public float az { get; set; }

        [DisplayText("Accel Strength")]
        [GroupText("Sensor")]
        public float accelsq => (float)Math.Sqrt(Math.Pow(ax, 2) + Math.Pow(ay, 2) + Math.Pow(az, 2)) / 1000.0f;

        // gyro
        [DisplayText("Gyro X")]
        [GroupText("Sensor")]
        public float gx { get; set; }

        [DisplayText("Gyro Y")]
        [GroupText("Sensor")]
        public float gy { get; set; }

        [DisplayText("Gyro Z")]
        [GroupText("Sensor")]
        public float gz { get; set; }

        [DisplayText("Gyro Strength")]
        [GroupText("Sensor")]
        public float gyrosq => (float)Math.Sqrt(Math.Pow(gx, 2) + Math.Pow(gy, 2) + Math.Pow(gz, 2));

        // mag
        [DisplayText("Mag X")]
        [GroupText("Sensor")]
        public float mx { get; set; }

        [DisplayText("Mag Y")]
        [GroupText("Sensor")]
        public float my { get; set; }

        [DisplayText("Mag Z")]
        [GroupText("Sensor")]
        public float mz { get; set; }

        [DisplayText("Mag Field")]
        [GroupText("Sensor")]
        public float magfield => (float)Math.Sqrt(Math.Pow(mx, 2) + Math.Pow(my, 2) + Math.Pow(mz, 2));

        // accel2
        [DisplayText("Accel2 X")]
        [GroupText("Sensor")]
        public float ax2 { get; set; }

        [DisplayText("Accel2 Y")]
        [GroupText("Sensor")]
        public float ay2 { get; set; }

        [DisplayText("Accel2 Z")]
        [GroupText("Sensor")]
        public float az2 { get; set; }

        [DisplayText("Accel2 Strength")]
        [GroupText("Sensor")]
        public float accelsq2 => (float)Math.Sqrt(Math.Pow(ax2, 2) + Math.Pow(ay2, 2) + Math.Pow(az2, 2)) / 1000.0f;

        // gyro2
        [DisplayText("Gyro2 X")]
        [GroupText("Sensor")]
        public float gx2 { get; set; }

        [DisplayText("Gyro2 Y")]
        [GroupText("Sensor")]
        public float gy2 { get; set; }

        [DisplayText("Gyro2 Z")]
        [GroupText("Sensor")]
        public float gz2 { get; set; }

        [DisplayText("Gyro2 Strength")]
        [GroupText("Sensor")]
        public float gyrosq2 => (float)Math.Sqrt(Math.Pow(gx2, 2) + Math.Pow(gy2, 2) + Math.Pow(gz2, 2));

        // mag2
        [DisplayText("Mag2 X")]
        [GroupText("Sensor")]
        public float mx2 { get; set; }

        [DisplayText("Mag2 Y")]
        [GroupText("Sensor")]
        public float my2 { get; set; }

        [DisplayText("Mag2 Z")]
        [GroupText("Sensor")]
        public float mz2 { get; set; }

        [DisplayText("Mag2 Field")]
        [GroupText("Sensor")]
        public float magfield2 => (float)Math.Sqrt(Math.Pow(mx2, 2) + Math.Pow(my2, 2) + Math.Pow(mz2, 2));

        // accel3
        [DisplayText("Accel3 X")]
        [GroupText("Sensor")]
        public float ax3 { get; set; }

        [DisplayText("Accel3 Y")]
        [GroupText("Sensor")]
        public float ay3 { get; set; }

        [DisplayText("Accel3 Z")]
        [GroupText("Sensor")]
        public float az3 { get; set; }

        [DisplayText("Accel3 Strength")]
        [GroupText("Sensor")]
        public float accelsq3 => (float)Math.Sqrt(Math.Pow(ax3, 2) + Math.Pow(ay3, 2) + Math.Pow(az3, 2)) / 1000.0f;

        // gyro3
        [DisplayText("Gyro3 X")]
        [GroupText("Sensor")]
        public float gx3 { get; set; }

        [DisplayText("Gyro3 Y")]
        [GroupText("Sensor")]
        public float gy3 { get; set; }

        [DisplayText("Gyro3 Z")]
        [GroupText("Sensor")]
        public float gz3 { get; set; }

        [DisplayText("Gyro3 Strength")]
        [GroupText("Sensor")]
        public float gyrosq3 => (float)Math.Sqrt(Math.Pow(gx3, 2) + Math.Pow(gy3, 2) + Math.Pow(gz3, 2));

        // mag3
        [DisplayText("Mag3 X")]
        [GroupText("Sensor")]
        public float mx3 { get; set; }

        [DisplayText("Mag3 Y")]
        [GroupText("Sensor")]
        public float my3 { get; set; }

        [DisplayText("Mag3 Z")]
        [GroupText("Sensor")]
        public float mz3 { get; set; }

        [DisplayText("Mag3 Field")]
        [GroupText("Sensor")]
        public float magfield3 => (float)Math.Sqrt(Math.Pow(mx3, 2) + Math.Pow(my3, 2) + Math.Pow(mz3, 2));

        //radio
        [GroupText("RadioIn")] public float ch1in { get; set; }

        [GroupText("RadioIn")] public float ch2in { get; set; }

        [GroupText("RadioIn")] public float ch3in { get; set; }

        [GroupText("RadioIn")] public float ch4in { get; set; }

        [GroupText("RadioIn")] public float ch5in { get; set; }

        [GroupText("RadioIn")] public float ch6in { get; set; }

        [GroupText("RadioIn")] public float ch7in { get; set; }

        [GroupText("RadioIn")] public float ch8in { get; set; }

        [GroupText("RadioIn")] public float ch9in { get; set; }

        [GroupText("RadioIn")] public float ch10in { get; set; }

        [GroupText("RadioIn")] public float ch11in { get; set; }

        [GroupText("RadioIn")] public float ch12in { get; set; }

        [GroupText("RadioIn")] public float ch13in { get; set; }

        [GroupText("RadioIn")] public float ch14in { get; set; }

        [GroupText("RadioIn")] public float ch15in { get; set; }

        [GroupText("RadioIn")] public float ch16in { get; set; }

        // motors
        [GroupText("RadioOut")] public float ch1out { get; set; }

        [GroupText("RadioOut")] public float ch2out { get; set; }

        [GroupText("RadioOut")] public float ch3out { get; set; }

        [GroupText("RadioOut")] public float ch4out { get; set; }

        [GroupText("RadioOut")] public float ch5out { get; set; }

        [GroupText("RadioOut")] public float ch6out { get; set; }

        [GroupText("RadioOut")] public float ch7out { get; set; }

        [GroupText("RadioOut")] public float ch8out { get; set; }

        [GroupText("RadioOut")] public float ch9out { get; set; }

        [GroupText("RadioOut")] public float ch10out { get; set; }

        [GroupText("RadioOut")] public float ch11out { get; set; }

        [GroupText("RadioOut")] public float ch12out { get; set; }

        [GroupText("RadioOut")] public float ch13out { get; set; }

        [GroupText("RadioOut")] public float ch14out { get; set; }

        [GroupText("RadioOut")] public float ch15out { get; set; }

        [GroupText("RadioOut")] public float ch16out { get; set; }

        [GroupText("ESC")] public float esc1_volt { get; set; }
        [GroupText("ESC")] public float esc1_curr { get; set; }
        [GroupText("ESC")] public float esc1_rpm { get; set; }
        [GroupText("ESC")] public float esc1_temp { get; set; }

        [GroupText("ESC")] public float esc2_volt { get; set; }
        [GroupText("ESC")] public float esc2_curr { get; set; }
        [GroupText("ESC")] public float esc2_rpm { get; set; }
        [GroupText("ESC")] public float esc2_temp { get; set; }

        [GroupText("ESC")] public float esc3_volt { get; set; }
        [GroupText("ESC")] public float esc3_curr { get; set; }
        [GroupText("ESC")] public float esc3_rpm { get; set; }
        [GroupText("ESC")] public float esc3_temp { get; set; }

        [GroupText("ESC")] public float esc4_volt { get; set; }
        [GroupText("ESC")] public float esc4_curr { get; set; }
        [GroupText("ESC")] public float esc4_rpm { get; set; }
        [GroupText("ESC")] public float esc4_temp { get; set; }

        [GroupText("ESC")] public float esc5_volt { get; set; }
        [GroupText("ESC")] public float esc5_curr { get; set; }
        [GroupText("ESC")] public float esc5_rpm { get; set; }
        [GroupText("ESC")] public float esc5_temp { get; set; }

        [GroupText("ESC")] public float esc6_volt { get; set; }
        [GroupText("ESC")] public float esc6_curr { get; set; }
        [GroupText("ESC")] public float esc6_rpm { get; set; }
        [GroupText("ESC")] public float esc6_temp { get; set; }

        [GroupText("ESC")] public float esc7_volt { get; set; }
        [GroupText("ESC")] public float esc7_curr { get; set; }
        [GroupText("ESC")] public float esc7_rpm { get; set; }
        [GroupText("ESC")] public float esc7_temp { get; set; }

        [GroupText("ESC")] public float esc8_volt { get; set; }
        [GroupText("ESC")] public float esc8_curr { get; set; }
        [GroupText("ESC")] public float esc8_rpm { get; set; }
        [GroupText("ESC")] public float esc8_temp { get; set; }

        public float ch3percent
        {
            get
            {
                if (_ch3percent != -1)
                    return _ch3percent;
                try
                {
                    if (parent != null && parent.parent.MAV.param.ContainsKey("RC3_MIN") &&
                        parent.parent.MAV.param.ContainsKey("RC3_MAX"))
                        return
                            (int)
                            ((ch3out - parent.parent.MAV.param["RC3_MIN"].Value) /
                             (parent.parent.MAV.param["RC3_MAX"].Value - parent.parent.MAV.param["RC3_MIN"].Value) *
                             100);

                    if (ch3out == 0)
                        return 0;
                    return (int)((ch3out - 1100) / (1900 - 1100) * 100);
                }
                catch
                {
                    return 0;
                }
            }

            set => _ch3percent = value;
        }

        [DisplayText("Failsafe")] public bool failsafe { get; set; }

        [DisplayText("RX Rssi")] public int rxrssi { get; set; }

        public float crit_AOA
        {
            get
            {
                try
                {
                    if (parent.parent.MAV.param.ContainsKey("AOA_CRIT"))
                        return
                            (int)
                            parent.parent.MAV.param["AOA_CRIT"].Value;
                    return 25;
                }
                catch
                {
                    return 0;
                }
            }

            set { }
        }

        public bool lowgroundspeed { get; set; }

        [DisplayText("Vertical Speed (speed)")]
        public float verticalspeed
        {
            get
            {
                if (float.IsNaN(_verticalspeed)) _verticalspeed = 0;
                return _verticalspeed * multiplierspeed;
            }
            set => _verticalspeed = _verticalspeed * 0.4f + value * 0.6f;
        }

        [DisplayText("Vertical Speed (fpm)")] public double verticalspeed_fpm => vz * -3.28084 * 60;

        [DisplayText("Glide Ratio")]
        public double glide_ratio
        {
            get
            {
                var hor = Math.Sqrt(vx * vx + vy * vy);
                return hor / vz;
            }
        }

        //nav state
        [GroupText("NAV")]
        [DisplayText("Roll Target (deg)")]
        public float nav_roll { get; set; }

        [GroupText("NAV")]
        [DisplayText("Pitch Target (deg)")]
        public float nav_pitch { get; set; }

        [GroupText("NAV")]
        [DisplayText("Bearing Target (deg)")]
        public float nav_bearing { get; set; }

        [GroupText("NAV")]
        [DisplayText("Bearing Target (deg)")]
        public float target_bearing { get; set; }

        [GroupText("NAV")]
        [DisplayText("Dist to WP (dist)")]
        public float wp_dist
        {
            get => _wpdist * multiplierdist;
            set => _wpdist = value;
        }

        [GroupText("NAV")]
        [DisplayText("Altitude Error (dist)")]
        public float alt_error
        {
            get => _alt_error * multiplieralt;
            set
            {
                if (_alt_error == value) return;
                _alt_error = value;
                targetalt = targetalt * 0.5f + (float)Math.Round(alt + alt_error, 0) * 0.5f;
            }
        }

        [GroupText("NAV")]
        [DisplayText("Bearing Error (deg)")]
        public float ber_error
        {
            get => target_bearing - yaw;
            set { }
        }

        [GroupText("NAV")]
        [DisplayText("Airspeed Error (speed)")]
        public float aspd_error
        {
            get => _aspd_error * multiplierspeed;
            set
            {
                if (_aspd_error == value) return;
                _aspd_error = value;
                targetairspeed = targetairspeed * 0.5f + (float)Math.Round(airspeed + aspd_error, 0) * 0.5f;
            }
        }

        [GroupText("NAV")]
        [DisplayText("Xtrack Error (m)")]
        public float xtrack_error { get; set; }

        [GroupText("NAV")]
        [DisplayText("WP No")]
        public float wpno { get; set; }

        [GroupText("NAV")]
        [DisplayText("Mode")]
        public string mode { get; set; }

        [DisplayText("ClimbRate (speed)")]
        public float climbrate
        {
            get => _climbrate * multiplierspeed;
            set => _climbrate = value;
        }


        /// <summary>
        ///     time over target in seconds
        /// </summary>
        [DisplayText("Time over Target (sec)")]
        [GroupText("NAV")]
        public int tot
        {
            get
            {
                if (_groundspeed <= 0) return 0;
                return (int)(_wpdist / _groundspeed);
            }
        }

        [DisplayText("Time over Home (sec)")]
        [GroupText("NAV")]
        public int toh
        {
            get
            {
                if (_groundspeed <= 0) return 0;
                return (int)(DistToHome / groundspeed);
            }
        }

        [DisplayText("Dist Traveled (dist)")] public float distTraveled { get; set; }

        [DisplayText("Time in Air (sec)")] public float timeSinceArmInAir { get; set; }

        [DisplayText("Time in Air (sec)")] public float timeInAir { get; set; }

        //Time in Air converted to min.sec format for easier reading
        [DisplayText("Time in Air (min.sec)")]
        public float timeInAirMinSec => (int)(timeInAir / 60) + timeInAir % 60 / 100;

        // calced turn rate
        [DisplayText("Turn Rate (speed)")]
        public float turnrate
        {
            get
            {
                if (_groundspeed <= 1) return 0;
                return (float)(roll * 9.80665 / groundspeed);
            }
        }

        //https://en.wikipedia.org/wiki/Load_factor_(aeronautics)
        [DisplayText("Turn Gs (load)")] public float turng => (float)(1 / Math.Cos(MathHelper.deg2rad * roll));

        // turn radius
        [DisplayText("Turn Radius (dist)")]
        public float radius
        {
            get
            {
                if (_groundspeed <= 1) return 0;
                return (float)(groundspeed * groundspeed / (9.80665 * Math.Tan(roll * MathHelper.deg2rad)));
            }
        }

        public float QNH
        {
            get
            {
                var pressure = press_abs;
                var alt_m = altasl;

                var gndtemp = 15f;
                var C_TO_KELVIN = 273.15f;
                var temp = gndtemp + C_TO_KELVIN; // kelvin
                var scaling = (float)Math.Exp(Math.Log(1.0 - alt_m / (153.8462 * temp)) / 0.190259);
                var base_pressure = pressure / scaling;
                return base_pressure;
            }
        }

        [DisplayText("Wind Direction (Deg)")] public float wind_dir { get; set; }

        [DisplayText("Wind Velocity (speed)")] public float wind_vel { get; set; }

        public float targetaltd100 => targetalt / 100 % 10;
        [GroupText("NAV")]
        public float targetalt { get; private set; }

        public List<(DateTime time, string message)> messages { get; set; } = new List<(DateTime, string)>();

        /// <summary>
        /// a message that originates from the mav
        /// </summary>
        public string message
        {
            get { return messages.LastOrDefault().message; }
        }

        /// <summary>
        /// a message that originates from within the gcs
        /// </summary>
        public string messageHigh
        {
            get { if (DateTime.Now > _messageHighTime.AddSeconds(10)) return ""; return _messagehigh; }
            set
            {
                // check against get
                if (messageHigh == value)
                    return;
                log.Info("messageHigh " + value);
                _messageHighTime = DateTime.Now;
                _messagehigh = value;
                messageHighSeverity = MAVLink.MAV_SEVERITY.EMERGENCY;
            }
        }

        string _messagehigh = "";
        DateTime _messageHighTime;

        public MAVLink.MAV_SEVERITY messageHighSeverity { get; set; }

        //battery
        [GroupText("Battery")]
        [DisplayText("Bat Voltage (V)")]
        public double battery_voltage
        {
            get => _battery_voltage;
            set
            {
                if (_battery_voltage == 0) _battery_voltage = value;
                _battery_voltage = value * 0.4f + _battery_voltage * 0.6f;
            }
        }

        [GroupText("Battery")]
        [DisplayText("Bat Voltage (V)")]
        public double battery_voltage3 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Voltage (V)")]
        public double battery_voltage4 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Voltage (V)")]
        public double battery_voltage5 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Voltage (V)")]
        public double battery_voltage6 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Voltage (V)")]
        public double battery_voltage7 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Voltage (V)")]
        public double battery_voltage8 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Remaining (%)")]
        public int battery_remaining
        {
            get => _battery_remaining;
            set
            {
                _battery_remaining = value;
                if (_battery_remaining < 0 || _battery_remaining > 100) _battery_remaining = 0;
            }
        }

        [GroupText("Battery")]
        [DisplayText("Bat Remaining (%)")]
        public int battery_remaining2 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Remaining (%)")]
        public int battery_remaining3 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Remaining (%)")]
        public int battery_remaining4 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Remaining (%)")]
        public int battery_remaining5 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Remaining (%)")]
        public int battery_remaining6 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Remaining (%)")]
        public int battery_remaining7 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Remaining (%)")]
        public int battery_remaining8 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Current (Amps)")]
        public double current
        {
            get => _current;
            set
            {
                if (_lastcurrent == DateTime.MinValue) _lastcurrent = datetime;
                // break; case for no sensor
                if (value == -0.01f)
                {
                    _current = 0;
                    return;
                }

                battery_usedmah += value * 1000.0 * (datetime - _lastcurrent).TotalHours;
                _current = value;
                _lastcurrent = datetime;
            }
        } //current may to be below zero - recuperation in arduplane

        [GroupText("Battery")]
        [DisplayText("Bat2 Current (Amps)")]
        public double current2
        {
            get => _current2;
            set
            {
                if (_lastcurrent2 == DateTime.MinValue) _lastcurrent2 = datetime;
                if (value < 0) return;
                battery_usedmah2 += value * 1000.0 * (datetime - _lastcurrent2).TotalHours;
                _current2 = value;
                _lastcurrent2 = datetime;
            }
        }

        [GroupText("Battery")]
        [DisplayText("Bat2 Current (Amps)")]
        public double current3 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat2 Current (Amps)")]
        public double current4 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat2 Current (Amps)")]
        public double current5 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat2 Current (Amps)")]
        public double current6 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat2 Current (Amps)")]
        public double current7 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat2 Current (Amps)")]
        public double current8 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat Watts")]
        public double watts => battery_voltage * current;

        [GroupText("Battery")]
        [DisplayText("Bat efficiency (mah/km)")]
        public double battery_mahperkm => battery_usedmah / (distTraveled / 1000.0f);

        [GroupText("Battery")]
        [DisplayText("Bat km left EST (km)")]
        public double battery_kmleft => (100.0f / (100.0f - battery_remaining) * battery_usedmah - battery_usedmah) /
                                        battery_mahperkm;

        [GroupText("Battery")]
        [DisplayText("Bat used EST (mah)")]
        public double battery_usedmah { get; set; }

        [GroupText("Battery")] public double battery_cell1 { get; set; }
        [GroupText("Battery")] public double battery_cell2 { get; set; }
        [GroupText("Battery")] public double battery_cell3 { get; set; }
        [GroupText("Battery")] public double battery_cell4 { get; set; }
        [GroupText("Battery")] public double battery_cell5 { get; set; }
        [GroupText("Battery")] public double battery_cell6 { get; set; }

        [GroupText("Battery")] public double battery_temp { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat used EST (mah)")]
        public double battery_usedmah2 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat used EST (mah)")]
        public double battery_usedmah3 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat used EST (mah)")]
        public double battery_usedmah4 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat used EST (mah)")]
        public double battery_usedmah5 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat used EST (mah)")]
        public double battery_usedmah6 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat used EST (mah)")]
        public double battery_usedmah7 { get; set; }

        [GroupText("Battery")]
        [DisplayText("Bat used EST (mah)")]
        public double battery_usedmah8 { get; set; }


        [GroupText("Battery")]
        [DisplayText("Bat2 Voltage (V)")]
        public double battery_voltage2
        {
            get => _battery_voltage2;
            set
            {
                if (_battery_voltage2 == 0) _battery_voltage2 = value;
                _battery_voltage2 = value * 0.4f + _battery_voltage2 * 0.6f;
            }
        }

        [GroupText("Position")]
        public double HomeAlt
        {
            get => HomeLocation.Alt;
            set { }
        }

        [GroupText("Position")]
        public PointLatLngAlt HomeLocation
        {
            get => _homelocation;
            set
            {
                _homelocation = value;
            }
        }

        [GroupText("Position")]
        public PointLatLngAlt PlannedHomeLocation
        {
            get => _plannedhomelocation;
            set => _plannedhomelocation = value;
        }

        [GroupText("Position")]
        public PointLatLngAlt MovingBase
        {
            get => _movingbase;
            set
            {
                if (value == null)
                    _movingbase = new PointLatLngAlt();

                if (_movingbase.Lat != value.Lat || _movingbase.Lng != value.Lng || _movingbase.Alt
                    != value.Alt)
                    _movingbase = value;
            }
        }

        [GroupText("Position")]
        public PointLatLngAlt TrackerLocation
        {
            get
            {
                if (_trackerloc.Lng != 0) return _trackerloc;
                return HomeLocation;
            }
            set => _trackerloc = value;
        }

        [GroupText("Position")] public PointLatLngAlt Location => new PointLatLngAlt(lat, lng, altasl);
        [GroupText("Position")] public PointLatLngAlt TargetLocation { get; set; } = PointLatLngAlt.Zero;


        public float GeoFenceDist
        {
            get
            {
                try
                {
                    float disttotal = 99999;
                    var R = 6371e3;

                    var list = parent.fencepoints
                        .Where(a => a.Value.command != (ushort)MAVLink.MAV_CMD.FENCE_RETURN_POINT)
                        .ChunkByField((a, b, count) =>
                        {
                            // these fields types stand alone
                            if (a.Value.command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION ||
                                a.Value.command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION)
                                return false;

                            if (count >= b.Value.param1)
                                return false;

                            return a.Value.command == b.Value.command;
                        });

                    // check all sublists
                    foreach (var sublist in list)
                    {
                        // process circles
                        if (sublist.Count() == 1)
                        {
                            var item = sublist.First().Value;
                            if (item.command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION)
                            {
                                var lla = new PointLatLngAlt(item.x / 1e7,
                                    item.y / 1e7);
                                var dist = lla.GetDistance(Location);
                                if (dist < item.param1)
                                    return 0;
                                disttotal = (float)Math.Min(dist - item.param1, disttotal);
                            }
                            else if (item.command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION)
                            {
                                var lla = new PointLatLngAlt(item.x / 1e7,
                                    item.y / 1e7);

                                var dist = lla.GetDistance(Location);
                                if (dist > item.param1)
                                    return 0;
                                disttotal = (float)Math.Min(item.param1 - dist, disttotal);
                            }
                        }

                        if (sublist == null || sublist.Count() < 3)
                            continue;

                        if (PolygonTools.isInside(
                            sublist.CloseLoop().Select(a => new PolygonTools.Point(a.Value.y / 1e7, a.Value.x / 1e7)).ToList(),
                            new PolygonTools.Point(Location.Lng, Location.Lat)))
                        {
                            if (sublist.First().Value.command ==
                                (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION)
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            if (sublist.First().Value.command ==
                                (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION)
                            {
                                return 0;
                            }
                        }

                        PointLatLngAlt lineStartLatLngAlt = null;
                        // check all segments
                        foreach (var mavlinkFencePointT in sublist.CloseLoop())
                        {
                            if (lineStartLatLngAlt == null)
                            {
                                lineStartLatLngAlt = new PointLatLngAlt(mavlinkFencePointT.Value.x / 1e7,
                                    mavlinkFencePointT.Value.y / 1e7);
                                continue;
                            }

                            // crosstrack distance
                            var lineEndLatLngAlt = new PointLatLngAlt(mavlinkFencePointT.Value.x / 1e7,
                                mavlinkFencePointT.Value.y / 1e7);

                            var lineDist = lineStartLatLngAlt.GetDistance2(lineEndLatLngAlt);

                            var distToLocation = lineStartLatLngAlt.GetDistance2(Location);
                            var bearToLocation = lineStartLatLngAlt.GetBearing(Location);
                            var lineBear = lineStartLatLngAlt.GetBearing(lineEndLatLngAlt);

                            var angle = bearToLocation - lineBear;
                            if (angle < 0)
                                angle += 360;

                            var alongline = Math.Cos(angle * MathHelper.deg2rad) * distToLocation;

                            // check to see if our point is still within the line length
                            if (alongline < 0 || alongline > lineDist)
                            {
                                lineStartLatLngAlt = lineEndLatLngAlt;
                                continue;
                            }

                            var dXt2 = Math.Sin(angle * MathHelper.deg2rad) * distToLocation;

                            var dXt = Math.Asin(Math.Sin(distToLocation / R) * Math.Sin(angle * MathHelper.deg2rad)) *
                                      R;

                            disttotal = (float)Math.Min(disttotal, Math.Abs(dXt2));

                            lineStartLatLngAlt = lineEndLatLngAlt;
                        }
                    }

                    // check also distance from the points - because if we are outside the polygon, we may be on a corner segment
                    foreach (var sublist in list)
                        foreach (var mavlinkFencePointT in sublist)
                        {
                            if (mavlinkFencePointT.Value.command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION)
                                continue;
                            var pathpoint = new PointLatLngAlt(mavlinkFencePointT.Value.x / 1e7,
                                mavlinkFencePointT.Value.y / 1e7);
                            var dXt2 = pathpoint.GetDistance(Location);
                            disttotal = (float)Math.Min(disttotal, Math.Abs(dXt2));
                        }

                    return disttotal;
                }
                catch
                {
                    return 0;
                }
            }
        }

        [GroupText("Position")]
        [DisplayText("Dist to Home (dist)")]
        public float DistToHome
        {
            get
            {
                if (lat == 0 && lng == 0 || TrackerLocation.Lat == 0)
                    return 0;

                // shrinking factor for longitude going to poles direction
                var rads = Math.Abs(TrackerLocation.Lat) * 0.0174532925;
                var scaleLongDown = Math.Cos(rads);
                var scaleLongUp = 1.0f / Math.Cos(rads);

                //DST to Home
                var dstlat = Math.Abs(TrackerLocation.Lat - lat) * 111319.5;
                var dstlon = Math.Abs(TrackerLocation.Lng - lng) * 111319.5 * scaleLongDown;
                return (float)Math.Sqrt(dstlat * dstlat + dstlon * dstlon) * multiplierdist;
            }
        }

        [GroupText("Position")]
        [DisplayText("Dist to Moving Base (dist)")]
        public float DistFromMovingBase
        {
            get
            {
                if (lat == 0 && lng == 0 || MovingBase == null)
                    return 0;

                // shrinking factor for longitude going to poles direction
                var rads = Math.Abs(MovingBase.Lat) * 0.0174532925;
                var scaleLongDown = Math.Cos(rads);
                var scaleLongUp = 1.0f / Math.Cos(rads);

                //DST to Home
                var dstlat = Math.Abs(MovingBase.Lat - lat) * 111319.5;
                var dstlon = Math.Abs(MovingBase.Lng - lng) * 111319.5 * scaleLongDown;
                return (float)Math.Sqrt(dstlat * dstlat + dstlon * dstlon) * multiplierdist;
            }
        }

        [DisplayText("Elevation to Mav (deg)")]
        public float ELToMAV
        {
            get
            {
                var dist = DistToHome / multiplierdist;

                if (dist < 5)
                    return 0;

                var altdiff = (float)(_altasl - TrackerLocation.Alt);

                var angle = (float)(Math.Atan(altdiff / dist) * MathHelper.rad2deg);

                return angle;
            }
        }

        [DisplayText("Bearing to Mav (deg)")]
        public float AZToMAV
        {
            get
            {
                // shrinking factor for longitude going to poles direction
                var rads = Math.Abs(TrackerLocation.Lat) * 0.0174532925;
                var scaleLongDown = Math.Cos(rads);
                var scaleLongUp = 1.0f / Math.Cos(rads);

                //DIR to Home
                var dstlon = TrackerLocation.Lng - lng; //OffSet_X
                var dstlat = (TrackerLocation.Lat - lat) * scaleLongUp; //OffSet Y
                var bearing = 90 + Math.Atan2(dstlat, -dstlon) * 57.295775; //absolut home direction
                if (bearing < 0) bearing += 360; //normalization
                //bearing = bearing - 180;//absolut return direction
                //if (bearing < 0) bearing += 360;//normalization

                var dist = DistToHome / multiplierdist;

                if (dist < 5)
                    return 0;

                return (float)bearing;
            }
        }

        [DisplayText("Sonar Range (meters)")]
        public float sonarrange
        {
            get => (float)toDistDisplayUnit(_sonarrange);
            set => _sonarrange = value;
        }

        [DisplayText("Sonar Voltage (Volt)")] public float sonarvoltage { get; set; }

        [DisplayText("RangeFinder1 (cm)")] public uint rangefinder1 { get; set; }

        [DisplayText("RangeFinder2 (cm)")] public uint rangefinder2 { get; set; }

        [DisplayText("RangeFinder3 (cm)")] public uint rangefinder3 { get; set; }

        public float freemem { get; set; }
        public float load { get; set; }
        public float brklevel { get; set; }
        public bool armed { get; set; }

        // Sik radio
        [GroupText("Telem")]
        [DisplayText("Sik Radio rssi")]
        public float rssi { get; set; }

        [GroupText("Telem")]
        [DisplayText("Sik Radio remote rssi")]
        public float remrssi { get; set; }

        [GroupText("Telem")] public byte txbuffer { get; set; }

        [GroupText("Telem")]
        [DisplayText("Sik Radio noise")]
        public float noise { get; set; }

        [GroupText("Telem")]
        [DisplayText("Sik Radio remote noise")]
        public float remnoise { get; set; }

        [GroupText("Telem")] public ushort rxerrors { get; set; }
        [GroupText("Telem")] public ushort fixedp { get; set; }

        [GroupText("Telem")]
        [DisplayText("Sik Radio snr")]
        public float localsnrdb
        {
            get
            {
                if (lastrssi.AddSeconds(1) > DateTime.Now) return _localsnrdb;
                lastrssi = DateTime.Now;
                _localsnrdb = (rssi - noise) / 1.9f * 0.5f + _localsnrdb * 0.5f;
                return _localsnrdb;
            }
        }

        [GroupText("Telem")]
        [DisplayText("Sik Radio remote snr")]
        public float remotesnrdb
        {
            get
            {
                if (lastremrssi.AddSeconds(1) > DateTime.Now) return _remotesnrdb;
                lastremrssi = DateTime.Now;
                _remotesnrdb = (remrssi - remnoise) / 1.9f * 0.5f + _remotesnrdb * 0.5f;
                return _remotesnrdb;
            }
        }

        [GroupText("Telem")]
        [DisplayText("Sik Radio est dist (m)")]
        public float DistRSSIRemain
        {
            get
            {
                float work = 0;
                if (localsnrdb == 0) return 0;
                if (localsnrdb > remotesnrdb)
                    // remote
                    // minus fade margin
                    work = remotesnrdb - 5;
                else
                    // local
                    // minus fade margin
                    work = localsnrdb - 5;

                {
                    var dist = DistToHome / multiplierdist;

                    work = dist * (float)Math.Pow(2.0, work / 6.0);
                }

                return work;
            }
        }

        // stats
        public ushort packetdropremote { get; set; }
        public ushort linkqualitygcs { get; set; }

        [DisplayText("Error Type")] public ushort errors_count1 { get; set; }

        [DisplayText("Error Type")] public ushort errors_count2 { get; set; }
        public ushort errors_count3 { get; set; }

        [DisplayText("Error Count")] public ushort errors_count4 { get; set; }

        [DisplayText("HW Voltage")] public float hwvoltage { get; set; }

        [DisplayText("Board Voltage")] public float boardvoltage { get; set; }

        [DisplayText("Servo Rail Voltage")] public float servovoltage { get; set; }

        [DisplayText("Voltage Flags")] public uint voltageflag { get; set; }

        public ushort i2cerrors { get; set; }

        public double timesincelastshot { get; set; }

        // pressure
        [GroupText("Sensor")] public float press_abs { get; set; }
        [GroupText("Sensor")] public int press_temp { get; set; }
        [GroupText("Sensor")] public float press_abs2 { get; set; }
        [GroupText("Sensor")] public int press_temp2 { get; set; }

        // sensor offsets
        [GroupText("Calibration")] public int mag_ofs_x { get; set; }
        [GroupText("Calibration")] public int mag_ofs_y { get; set; }
        [GroupText("Calibration")] public int mag_ofs_z { get; set; }
        [GroupText("Calibration")] public float mag_declination { get; set; }
        [GroupText("Calibration")] public int raw_press { get; set; }
        [GroupText("Calibration")] public int raw_temp { get; set; }
        [GroupText("Calibration")] public float gyro_cal_x { get; set; }
        [GroupText("Calibration")] public float gyro_cal_y { get; set; }
        [GroupText("Calibration")] public float gyro_cal_z { get; set; }
        [GroupText("Calibration")] public float accel_cal_x { get; set; }
        [GroupText("Calibration")] public float accel_cal_y { get; set; }
        [GroupText("Calibration")] public float accel_cal_z { get; set; }

        // requested stream rates
        public int rateattitude { get; set; }
        public int rateposition { get; set; }
        public int ratestatus { get; set; }
        public int ratesensors { get; set; }
        public int raterc { get; set; }

        // reference
        public DateTime datetime { get; set; }

        public bool connected => parent.parent.BaseStream != null && parent.parent.BaseStream.IsOpen ||
                                 parent.parent.logreadmode;


        public float campointa { get; set; }

        public float campointb { get; set; }

        public float campointc { get; set; }

        public PointLatLngAlt GimbalPoint { get; set; }

        public float gimballat
        {
            get
            {
                if (GimbalPoint == null) return 0;
                return (float)GimbalPoint.Lat;
            }
        }

        public float gimballng
        {
            get
            {
                if (GimbalPoint == null) return 0;
                return (float)GimbalPoint.Lng;
            }
        }


        public bool landed { get; set; }

        [GroupText("Terrain")] public bool terrainactive { get; set; }

        [GroupText("Terrain")]
        [DisplayText("Terrain AGL")]
        public float ter_curalt
        {
            get => _ter_curalt * multiplieralt;
            set => _ter_curalt = value;
        }

        [GroupText("Terrain")]
        [DisplayText("Terrain GL")]
        public float ter_alt
        {
            get => _ter_alt * multiplieralt;
            set => _ter_alt = value;
        }

        [GroupText("Terrain")] public float ter_load { get; set; }

        [GroupText("Terrain")] public float ter_pend { get; set; }

        [GroupText("Terrain")] public float ter_space { get; set; }

        public int KIndex => KIndexstatic;

        [GroupText("Flow")] [DisplayText("flow_comp_m_x")] public float opt_m_x { get; set; }

        [GroupText("Flow")] [DisplayText("flow_comp_m_y")] public float opt_m_y { get; set; }

        [GroupText("Flow")] [DisplayText("flow_x")] public short opt_x { get; set; }

        [GroupText("Flow")] [DisplayText("flow_y")] public short opt_y { get; set; }

        [GroupText("Flow")] [DisplayText("flow quality")] public byte opt_qua { get; set; }

        [GroupText("EKF")] public float ekfstatus { get; set; }

        [GroupText("EKF")] public int ekfflags { get; set; }

        [GroupText("EKF")] public float ekfvelv { get; set; }

        [GroupText("EKF")] public float ekfcompv { get; set; }

        [GroupText("EKF")] public float ekfposhor { get; set; }

        [GroupText("EKF")] public float ekfposvert { get; set; }

        [GroupText("EKF")] public float ekfteralt { get; set; }

        [GroupText("PID")] public float pidff { get; set; }

        [GroupText("PID")] public float pidP { get; set; }

        [GroupText("PID")] public float pidI { get; set; }

        [GroupText("PID")] public float pidD { get; set; }

        [GroupText("PID")] public byte pidaxis { get; set; }

        [GroupText("PID")] public float piddesired { get; set; }

        [GroupText("PID")] public float pidachieved { get; set; }

        public uint vibeclip0 { get; set; }

        public uint vibeclip1 { get; set; }

        public uint vibeclip2 { get; set; }

        public float vibex { get; set; }

        public float vibey { get; set; }

        public float vibez { get; set; }

        public Version version { get; set; }

        public float rpm1 { get; set; }

        public float rpm2 { get; set; }

        public uint capabilities { get; set; }

        public float speedup { get; set; }

        public byte vtol_state { get; private set; }
        public byte landed_state { get; private set; }
        [GroupText("Generator")]
        public float gen_status { get; set; }
        [GroupText("Generator")]
        public float gen_speed { get; set; }
        [GroupText("Generator")]
        public float gen_current { get; set; }
        [GroupText("Generator")]
        public float gen_voltage { get; set; }
        [GroupText("Generator")]
        public uint gen_runtime { get; set; }
        [GroupText("Generator")]
        public int gen_maint_time { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        private void Parent_OnPacketReceived(object sender, MAVLink.MAVLinkMessage mavLinkMessage)
        {
            if (mavLinkMessage.sysid == parent.sysid && mavLinkMessage.compid == parent.compid
                || mavLinkMessage.msgid == (uint)MAVLink.MAVLINK_MSG_ID.RADIO // propagate the RADIO/RADIO_STATUS message across all devices on this link
                || mavLinkMessage.msgid == (uint)MAVLink.MAVLINK_MSG_ID.RADIO_STATUS)
            {
                switch (mavLinkMessage.msgid)
                {
                    case (uint)MAVLink.MAVLINK_MSG_ID.RC_CHANNELS_SCALED:

                        // hil mavlink 0.9
                        {
                            var hil = mavLinkMessage.ToStructure<MAVLink.mavlink_rc_channels_scaled_t>();

                            hilch1 = hil.chan1_scaled;
                            hilch2 = hil.chan2_scaled;
                            hilch3 = hil.chan3_scaled;
                            hilch4 = hil.chan4_scaled;
                            hilch5 = hil.chan5_scaled;
                            hilch6 = hil.chan6_scaled;
                            hilch7 = hil.chan7_scaled;
                            hilch8 = hil.chan8_scaled;

                            // Console.WriteLine("RC_CHANNELS_SCALED Packet");
                        }

                        break;


                    case (uint)MAVLink.MAVLINK_MSG_ID.LOCAL_POSITION_NED:


                        {
                            var lpned = mavLinkMessage.ToStructure<MAVLink.mavlink_local_position_ned_t>();

                            var loc = HomeLocation.gps_offset(lpned.y, lpned.x);

                            //lat = loc.Lat;
                            //lng = loc.Lng;
                            //alt = (float)(loc.Alt + lpned.z);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.POSITION_TARGET_GLOBAL_INT:


                        {
                            var postraget = mavLinkMessage.ToStructure<MAVLink.mavlink_position_target_global_int_t>();

                            if (postraget.coordinate_frame == (byte)MAVLink.MAV_FRAME.GLOBAL_INT)
                                TargetLocation = new PointLatLngAlt(postraget.lat_int / 1e7, postraget.lon_int / 1e7,
                                    postraget.alt,
                                    postraget.type_mask.ToString());

                            if (postraget.coordinate_frame == (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT)
                                TargetLocation = new PointLatLngAlt(postraget.lat_int / 1e7, postraget.lon_int / 1e7,
                                    postraget.alt + HomeAlt,
                                    postraget.type_mask.ToString());
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION:


                        {
                            var version = mavLinkMessage.ToStructure<MAVLink.mavlink_autopilot_version_t>();
                            //#define FIRMWARE_VERSION 3,4,0,FIRMWARE_VERSION_TYPE_DEV
                            //		flight_sw_version	0x03040000	uint

                            var main = (byte)(version.flight_sw_version >> 24);
                            var sub = (byte)((version.flight_sw_version >> 16) & 0xff);
                            var rev = (byte)((version.flight_sw_version >> 8) & 0xff);
                            var type =
                                (MAVLink.FIRMWARE_VERSION_TYPE)(version.flight_sw_version & 0xff);

                            this.version = new Version(main, sub, rev, (int)type);

                            try
                            {
                                capabilities = (uint)(MAVLink.MAV_PROTOCOL_CAPABILITY)version.capabilities;
                                var test = (MAVLink.MAV_PROTOCOL_CAPABILITY)version.capabilities;

                                Console.WriteLine(test);
                            }
                            catch
                            {
                            }

                            Serial.print("Flight SW Version: ");
                            Serial.println(version.flight_sw_version);
                            Serial.print("Middleware SW: ");
                            Serial.println(version.middleware_sw_version);
                            Serial.print("OS Custom: ");
                            Serial.println(version.os_custom_version);
                            Serial.print("OS SW: ");
                            Serial.println(version.os_sw_version);
                            Serial.print("board_version: ");
                            Serial.println(version.board_version);
                            Serial.print("Vendor ID: ");
                            Serial.println(version.vendor_id);
                            Serial.print("Product ID: ");
                            Serial.println(version.product_id);
                            Serial.print("Board Version: ");
                            Serial.println(version.board_version);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.FENCE_STATUS:


                        {
                            var fence = mavLinkMessage.ToStructure<MAVLink.mavlink_fence_status_t>();

                            if (fence.breach_status != (byte)MAVLink.FENCE_BREACH.NONE)
                            {
                                // fence breached
                                messageHigh = "Fence Breach";
                            }
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.GENERATOR_STATUS:
                        {
                            var gen = mavLinkMessage.ToStructure<MAVLink.mavlink_generator_status_t>();

                            gen_status = gen.status;
                            gen_speed = gen.generator_speed;
                            gen_current = gen.load_current;
                            gen_voltage = gen.bus_voltage;
                            gen_runtime = gen.runtime;
                            gen_maint_time = gen.time_until_maintenance;
                        }
                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.HIGH_LATENCY:
                        {
                            var highlatency = mavLinkMessage.ToStructure<MAVLink.mavlink_high_latency_t>();

                            landed = highlatency.landed_state == 1;

                            if ((highlatency.base_mode & (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED) != 0)
                            {
                                var modelist = Common.getModesList(firmware);

                                if (modelist != null)
                                {
                                    var found = false;

                                    foreach (var pair in modelist)
                                        if (pair.Key == highlatency.custom_mode)
                                        {
                                            mode = pair.Value;
                                            _mode = highlatency.custom_mode;
                                            found = true;
                                            break;
                                        }

                                    if (!found)
                                        log.Warn("Mode not found bm:" + highlatency.base_mode + " cm:" +
                                                 highlatency.custom_mode);
                                }
                            }

                            roll = highlatency.roll / 100f;
                            pitch = highlatency.pitch / 100f;
                            yaw = highlatency.heading / 100f;
                            ch3percent = highlatency.throttle;
                            lat = highlatency.latitude / 1e7;
                            lng = highlatency.longitude / 1e7;
                            altasl = highlatency.altitude_amsl;
                            alt = altasl - (float)HomeAlt;
                            alt_error = highlatency.altitude_sp - alt;
                            airspeed = highlatency.airspeed;
                            targetairspeed = highlatency.airspeed_sp;
                            groundspeed = highlatency.groundspeed;
                            climbrate = highlatency.climb_rate;
                            satcount = highlatency.gps_nsat;
                            gpsstatus = highlatency.gps_fix_type;
                            battery_remaining = highlatency.battery_remaining;
                            press_temp = highlatency.temperature;
                            raw_temp = highlatency.temperature_air;
                            failsafe = highlatency.failsafe > 0;
                            wpno = highlatency.wp_num;
                            wp_dist = highlatency.wp_distance;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.HIL_CONTROLS:

                        // hil mavlink 0.9 and 1.0
                        {
                            var hil = mavLinkMessage.ToStructure<MAVLink.mavlink_hil_controls_t>();

                            hilch1 = (int)(hil.roll_ailerons * 10000);
                            hilch2 = (int)(hil.pitch_elevator * 10000);
                            hilch3 = (int)(hil.throttle * 10000);
                            hilch4 = (int)(hil.yaw_rudder * 10000);

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.HIL_CONTROLS);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.OPTICAL_FLOW:


                        {
                            var optflow = mavLinkMessage.ToStructure<MAVLink.mavlink_optical_flow_t>();

                            opt_m_x = optflow.flow_comp_m_x;
                            opt_m_y = optflow.flow_comp_m_y;
                            opt_x = optflow.flow_x;
                            opt_y = optflow.flow_y;
                            opt_qua = optflow.quality;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.MOUNT_STATUS:


                        {
                            var status = mavLinkMessage.ToStructure<MAVLink.mavlink_mount_status_t>();

                            campointa = status.pointing_a / 100.0f;
                            campointb = status.pointing_b / 100.0f;
                            campointc = status.pointing_c / 100.0f;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.VIBRATION:


                        {
                            var vibe = mavLinkMessage.ToStructure<MAVLink.mavlink_vibration_t>();

                            vibeclip0 = vibe.clipping_0;
                            vibeclip1 = vibe.clipping_1;
                            vibeclip2 = vibe.clipping_2;
                            vibex = vibe.vibration_x;
                            vibey = vibe.vibration_y;
                            vibez = vibe.vibration_z;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.AIRSPEED_AUTOCAL:


                        {
                            var asac = mavLinkMessage.ToStructure<MAVLink.mavlink_airspeed_autocal_t>();

                            asratio = asac.ratio;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.SYSTEM_TIME:


                        {
                            var systime = mavLinkMessage.ToStructure<MAVLink.mavlink_system_time_t>();

                            var date1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                            try
                            {
                                date1 = date1.AddMilliseconds(systime.time_unix_usec / 1000);

                                gpstime = date1;
                            }
                            catch
                            {
                            }
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.HWSTATUS:


                        {
                            var hwstatus = mavLinkMessage.ToStructure<MAVLink.mavlink_hwstatus_t>();

                            hwvoltage = hwstatus.Vcc / 1000.0f;
                            i2cerrors = hwstatus.I2Cerr;

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.HWSTATUS);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.EKF_STATUS_REPORT:

                        {
                            var ekfstatusm = mavLinkMessage.ToStructure<MAVLink.mavlink_ekf_status_report_t>();

                            // > 1, between 0-1 typical > 1 = reject measurement - red
                            // 0.5 > amber

                            ekfvelv = ekfstatusm.velocity_variance;
                            ekfcompv = ekfstatusm.compass_variance;
                            ekfposhor = ekfstatusm.pos_horiz_variance;
                            ekfposvert = ekfstatusm.pos_vert_variance;
                            ekfteralt = ekfstatusm.terrain_alt_variance;

                            ekfflags = ekfstatusm.flags;

                            ekfstatus =
                                Math.Max(ekfvelv,
                                    Math.Max(ekfcompv, Math.Max(ekfposhor, Math.Max(ekfposvert, ekfteralt))));

                            if (ekfvelv >= 1)
                            {
                                messageHigh = Strings.ERROR + " " + Strings.velocity_variance;
                            }

                            if (ekfcompv >= 1)
                            {
                                messageHigh = Strings.ERROR + " " + Strings.compass_variance;
                            }

                            if (ekfposhor >= 1)
                            {
                                messageHigh = Strings.ERROR + " " + Strings.pos_horiz_variance;
                            }

                            if (ekfposvert >= 1)
                            {
                                messageHigh = Strings.ERROR + " " + Strings.pos_vert_variance;
                            }

                            if (ekfteralt >= 1)
                            {
                                messageHigh = Strings.ERROR + " " + Strings.terrain_alt_variance;
                            }

                            for (var a = 1; a <= (int)MAVLink.EKF_STATUS_FLAGS.EKF_UNINITIALIZED; a = a << 1)
                            {
                                var currentbit = ekfstatusm.flags & a;
                                if (currentbit == 0)
                                {
                                    var currentflag =
                                        (MAVLink.EKF_STATUS_FLAGS)
                                        Enum.Parse(typeof(MAVLink.EKF_STATUS_FLAGS), a.ToString());

                                    switch (currentflag)
                                    {
                                        case MAVLink.EKF_STATUS_FLAGS.EKF_ATTITUDE: // step 1
                                            ekfstatus = 1;
                                            log.Info("EKF red has no EKF_ATTITUDE - " +
                                                     (MAVLink.EKF_STATUS_FLAGS)ekfstatusm.flags);
                                            break;
                                        case MAVLink.EKF_STATUS_FLAGS.EKF_VELOCITY_HORIZ: // with pos
                                            if (gpsstatus > 0)
                                            {
                                                // we have gps and dont have vel_hoz
                                                ekfstatus = 1;
                                                log.Info(
                                                    "EKF red has gps lock but no EKF_ATTITUDE and EKF_VELOCITY_HORIZ - " +
                                                    (MAVLink.EKF_STATUS_FLAGS)ekfstatusm.flags);
                                            }

                                            break;
                                        case MAVLink.EKF_STATUS_FLAGS.EKF_VELOCITY_VERT: // with pos
                                                                                         //case MAVLink.EKF_STATUS_FLAGS.EKF_POS_HORIZ_REL: // optical flow
                                        case MAVLink.EKF_STATUS_FLAGS.EKF_POS_HORIZ_ABS: // step 1
                                        case MAVLink.EKF_STATUS_FLAGS.EKF_POS_VERT_ABS: // step 1
                                                                                        //case MAVLink.EKF_STATUS_FLAGS.EKF_POS_VERT_AGL: //  range finder
                                                                                        //case MAVLink.EKF_STATUS_FLAGS.EKF_CONST_POS_MODE:  // never true when absolute - non gps
                                                                                        //case MAVLink.EKF_STATUS_FLAGS.EKF_PRED_POS_HORIZ_REL: // optical flow
                                        case MAVLink.EKF_STATUS_FLAGS.EKF_PRED_POS_HORIZ_ABS: // ekf has origin - post arm
                                                                                              //messageHigh = Strings.ERROR + " " + currentflag.ToString().Replace("_", " ");
                                                                                              //messageHighTime = DateTime.Now;
                                            break;

                                    }
                                }
                                else
                                {
                                    if (a == (int)MAVLink.EKF_STATUS_FLAGS.EKF_UNINITIALIZED)
                                    {
                                        ekfstatus = 1;
                                        log.Info("EKF red uninit " + (MAVLink.EKF_STATUS_FLAGS)ekfstatusm.flags);
                                    }
                                }
                            }
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.RANGEFINDER:

                        {
                            var sonar = mavLinkMessage.ToStructure<MAVLink.mavlink_rangefinder_t>();

                            sonarrange = sonar.distance;
                            sonarvoltage = sonar.voltage;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.DISTANCE_SENSOR:

                        {
                            var sonar = mavLinkMessage.ToStructure<MAVLink.mavlink_distance_sensor_t>();
                            if (sonar.id == 0)
                                rangefinder1 = sonar.current_distance;
                            else if (sonar.id == 1)
                                rangefinder2 = sonar.current_distance;
                            else if (sonar.id == 2) rangefinder3 = sonar.current_distance;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.POWER_STATUS:

                        {
                            var power = mavLinkMessage.ToStructure<MAVLink.mavlink_power_status_t>();

                            boardvoltage = power.Vcc;
                            servovoltage = power.Vservo;

                            try
                            {
                                voltageflag = (uint)(MAVLink.MAV_POWER_STATUS)power.flags;

                                if(voltageflag == (uint)MAVLink.MAV_POWER_STATUS.PERIPH_OVERCURRENT)
                                {
                                    messageHigh = "PERIPH_OVERCURRENT";
                                } 
                                else if(voltageflag == (uint)MAVLink.MAV_POWER_STATUS.PERIPH_HIPOWER_OVERCURRENT)
                                {
                                    messageHigh = "PERIPH_HIPOWER_OVERCURRENT";
                                }
                            }
                            catch
                            {
                            }
                        }


                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.WIND:

                        {
                            var wind = mavLinkMessage.ToStructure<MAVLink.mavlink_wind_t>();

                            gotwind = true;

                            wind_dir = (wind.direction + 360) % 360;
                            wind_vel = wind.speed * multiplierspeed;
                        }


                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT:

                        {
                            var hb = mavLinkMessage.ToStructure<MAVLink.mavlink_heartbeat_t>();

                            if (hb.type == (byte)MAVLink.MAV_TYPE.GCS)
                            {
                                // skip gcs hb's
                                // only happens on log playback - and shouldnt get them here
                            }
                            else
                            {
                                armed = (hb.base_mode & (byte)MAVLink.MAV_MODE_FLAG.SAFETY_ARMED) ==
                                        (byte)MAVLink.MAV_MODE_FLAG.SAFETY_ARMED;

                                // saftey switch
                                if (armed && sensors_enabled.motor_control == false && sensors_enabled.seen)
                                {
                                    messageHigh = "(SAFE)";
                                }

                                // for future use
                                landed = hb.system_status == (byte)MAVLink.MAV_STATE.STANDBY;

                                failsafe = hb.system_status == (byte)MAVLink.MAV_STATE.CRITICAL;

                                var oldmode = mode;

                                if ((hb.base_mode & (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED) != 0)
                                    // prevent running thsi unless we have to
                                    if (_mode != hb.custom_mode)
                                    {
                                        var modelist = Common.getModesList(firmware);

                                        if (modelist != null)
                                        {
                                            var found = false;

                                            foreach (var pair in modelist)
                                                if (pair.Key == hb.custom_mode)
                                                {
                                                    mode = pair.Value;
                                                    _mode = hb.custom_mode;
                                                    found = true;
                                                    break;
                                                }

                                            if (!found)
                                                log.Warn("Mode not found bm:" + hb.base_mode + " cm:" + hb.custom_mode);
                                        }

                                        _mode = hb.custom_mode;
                                    }

                                try
                                {
                                    if (oldmode != mode && Speech != null && Speech.speechEnable &&
                                        parent?.parent?.MAV?.cs == this &&
                                        Settings.Instance.GetBoolean("speechmodeenabled"))
                                        Speech.SpeakAsync(Common.speechConversion(parent,
                                            "" + Settings.Instance["speechmode"]));
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                }
                            }
                        }


                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.SYS_STATUS:

                        {
                            var sysstatus = mavLinkMessage.ToStructure<MAVLink.mavlink_sys_status_t>();

                            load = sysstatus.load / 10.0f;

                            battery_voltage = sysstatus.voltage_battery / 1000.0f;
                            battery_remaining = sysstatus.battery_remaining;
                            current = sysstatus.current_battery / 100.0f;

                            packetdropremote = sysstatus.drop_rate_comm;

                            //sysstatus.errors_comm;
                            errors_count1 = sysstatus.errors_count1;
                            errors_count2 = sysstatus.errors_count2;
                            errors_count3 = sysstatus.errors_count3;
                            errors_count4 = sysstatus.errors_count4;

                            sensors_enabled.Value = sysstatus.onboard_control_sensors_enabled;
                            sensors_health.Value = sysstatus.onboard_control_sensors_health;
                            sensors_present.Value = sysstatus.onboard_control_sensors_present;

                            terrainactive = sensors_health.terrain && sensors_enabled.terrain && sensors_present.terrain;

                            if (errors_count1 > 0 || errors_count2 > 0)
                            {
                                messageHigh = "InternalError 0x" + (errors_count1 + (errors_count2 << 16)).ToString("X");
                            }

                            if (!sensors_health.prearm && sensors_enabled.prearm && sensors_present.prearm)
                            {
                                messageHigh = messages.LastOrDefault(a => a.message.ToLower().Contains("prearm")).message
                                    ?.ToString();
                            }
                            else if (!sensors_health.gps && sensors_enabled.gps && sensors_present.gps)
                            {
                                messageHigh = Strings.BadGPSHealth;
                            }
                            else if (!sensors_health.gyro && sensors_enabled.gyro && sensors_present.gyro)
                            {
                                messageHigh = Strings.BadGyroHealth;
                            }
                            else if (!sensors_health.accelerometer && sensors_enabled.accelerometer &&
                                     sensors_present.accelerometer)
                            {
                                messageHigh = Strings.BadAccelHealth;
                            }
                            else if (!sensors_health.compass && sensors_enabled.compass && sensors_present.compass)
                            {
                                messageHigh = Strings.BadCompassHealth;
                            }
                            else if (!sensors_health.barometer && sensors_enabled.barometer && sensors_present.barometer)
                            {
                                messageHigh = Strings.BadBaroHealth;
                            }
                            else if (!sensors_health.LASER_POSITION && sensors_enabled.LASER_POSITION &&
                                     sensors_present.LASER_POSITION)
                            {
                                messageHigh = Strings.BadLiDARHealth;
                            }
                            else if (!sensors_health.optical_flow && sensors_enabled.optical_flow &&
                                     sensors_present.optical_flow)
                            {
                                messageHigh = Strings.BadOptFlowHealth;
                            }
                            else if (!sensors_health.VISION_POSITION && sensors_enabled.VISION_POSITION &&
                                     sensors_present.VISION_POSITION)
                            {
                                messageHigh = Strings.Bad_Vision_Position;
                            }
                            else if (!sensors_health.terrain && sensors_enabled.terrain && sensors_present.terrain)
                            {
                                messageHigh = Strings.BadorNoTerrainData;
                            }
                            else if (!sensors_health.geofence && sensors_enabled.geofence &&
                                     sensors_present.geofence)
                            {
                                messageHigh = Strings.GeofenceBreach;
                            }
                            else if (!sensors_health.ahrs && sensors_enabled.ahrs && sensors_present.ahrs)
                            {
                                messageHigh = Strings.BadAHRS;
                            }
                            else if (!sensors_health.rc_receiver && sensors_enabled.rc_receiver &&
                                     sensors_present.rc_receiver)
                            {
                                var reporterror = true;
                                if (Settings.Instance["norcreceiver"] != null)
                                    reporterror = !bool.Parse(Settings.Instance["norcreceiver"]);
                                if (reporterror)
                                {
                                    messageHigh = Strings.NORCReceiver;
                                }
                            }
                            else if (!sensors_health.battery && sensors_enabled.battery && sensors_present.battery)
                            {
                                messageHigh = Strings.Bad_Battery;
                            }
                            else if (!sensors_health.proximity && sensors_enabled.proximity && sensors_present.proximity)
                            {
                                messageHigh = Strings.Bad_Proximity;
                            }
                            else if (!sensors_health.satcom && sensors_enabled.satcom && sensors_present.satcom)
                            {
                                messageHigh = Strings.Bad_SatCom;
                            }
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.EXTENDED_SYS_STATE:

                        {
                            var extsysstatus = mavLinkMessage.ToStructure<MAVLink.mavlink_extended_sys_state_t>();

                            vtol_state = extsysstatus.vtol_state;
                            landed_state = extsysstatus.landed_state;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.BATTERY2:

                        {
                            var bat = mavLinkMessage.ToStructure<MAVLink.mavlink_battery2_t>();
                            _battery_voltage2 = bat.voltage / 1000.0f;
                            current2 = bat.current_battery / 100.0f;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.BATTERY_STATUS:

                        {
                            var bats = mavLinkMessage.ToStructure<MAVLink.mavlink_battery_status_t>();

                            if (bats.id == 0)
                            {
                                if (bats.voltages[0] != ushort.MaxValue)
                                {
                                    battery_cell1 = bats.voltages[0] / 1000.0;
                                    if (bats.voltages[1] != ushort.MaxValue) battery_cell2 = bats.voltages[1] / 1000.0;
                                    else battery_cell2 = 0.0;
                                    if (bats.voltages[2] != ushort.MaxValue) battery_cell3 = bats.voltages[2] / 1000.0;
                                    else battery_cell3 = 0.0;
                                    if (bats.voltages[3] != ushort.MaxValue) battery_cell4 = bats.voltages[3] / 1000.0;
                                    else battery_cell4 = 0.0;
                                    if (bats.voltages[4] != ushort.MaxValue) battery_cell5 = bats.voltages[4] / 1000.0;
                                    else battery_cell5 = 0.0;
                                    if (bats.voltages[5] != ushort.MaxValue) battery_cell6 = bats.voltages[5] / 1000.0;
                                    else battery_cell6 = 0.0;
                                }

                                battery_usedmah = bats.current_consumed;
                                battery_remaining = bats.battery_remaining;
                                _current = bats.current_battery / 100.0f;
                                if (bats.temperature != short.MaxValue)
                                    battery_temp = bats.temperature / 100.0;
                            }
                            else if (bats.id == 1)
                            {
                                battery_usedmah2 = bats.current_consumed;
                                battery_remaining2 = bats.battery_remaining;
                                _current2 = bats.current_battery / 100.0f;
                            }
                            else if (bats.id == 2)
                            {
                                battery_usedmah3 = bats.current_consumed;
                                battery_remaining3 = bats.battery_remaining;
                                battery_voltage3 = bats.voltages.Sum(a => a != ushort.MaxValue ? a / 1000.0 : 0);
                                current3 = bats.current_battery / 100.0f;
                            }
                            else if (bats.id == 3)
                            {
                                battery_usedmah4 = bats.current_consumed;
                                battery_remaining4 = bats.battery_remaining;
                                battery_voltage4 = bats.voltages.Sum(a => a != ushort.MaxValue ? a / 1000.0 : 0);
                                current4 = bats.current_battery / 100.0f;
                            }
                            else if (bats.id == 4)
                            {
                                battery_usedmah5 = bats.current_consumed;
                                battery_remaining5 = bats.battery_remaining;
                                battery_voltage5 = bats.voltages.Sum(a => a != ushort.MaxValue ? a / 1000.0 : 0);
                                current5 = bats.current_battery / 100.0f;
                            }
                            else if (bats.id == 5)
                            {
                                battery_usedmah6 = bats.current_consumed;
                                battery_remaining6 = bats.battery_remaining;
                                battery_voltage6 = bats.voltages.Sum(a => a != ushort.MaxValue ? a / 1000.0 : 0);
                                current6 = bats.current_battery / 100.0f;
                            }
                            else if (bats.id == 6)
                            {
                                battery_usedmah7 = bats.current_consumed;
                                battery_remaining7 = bats.battery_remaining;
                                battery_voltage7 = bats.voltages.Sum(a => a != ushort.MaxValue ? a / 1000.0 : 0);
                                current7 = bats.current_battery / 100.0f;
                            }
                            else if (bats.id == 7)
                            {
                                battery_usedmah8 = bats.current_consumed;
                                battery_remaining8 = bats.battery_remaining;
                                battery_voltage8 = bats.voltages.Sum(a => a != ushort.MaxValue ? a / 1000.0 : 0);
                                current8 = bats.current_battery / 100.0f;
                            }
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.SCALED_PRESSURE:

                        {
                            var pres = mavLinkMessage.ToStructure<MAVLink.mavlink_scaled_pressure_t>();
                            press_abs = pres.press_abs;
                            press_temp = pres.temperature;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.SCALED_PRESSURE2:

                        {
                            var pres = mavLinkMessage.ToStructure<MAVLink.mavlink_scaled_pressure2_t>();
                            press_abs2 = pres.press_abs;
                            press_temp2 = pres.temperature;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.TERRAIN_REPORT:

                        {
                            var terrainrep = mavLinkMessage.ToStructure<MAVLink.mavlink_terrain_report_t>();
                            ter_curalt = terrainrep.current_height;
                            ter_alt = terrainrep.terrain_height;
                            ter_load = terrainrep.loaded;
                            ter_pend = terrainrep.pending;
                            ter_space = terrainrep.spacing;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.SENSOR_OFFSETS:

                        {
                            var sensofs = mavLinkMessage.ToStructure<MAVLink.mavlink_sensor_offsets_t>();

                            mag_ofs_x = sensofs.mag_ofs_x;
                            mag_ofs_y = sensofs.mag_ofs_y;
                            mag_ofs_z = sensofs.mag_ofs_z;
                            mag_declination = sensofs.mag_declination;

                            raw_press = sensofs.raw_press;
                            raw_temp = sensofs.raw_temp;

                            gyro_cal_x = sensofs.gyro_cal_x;
                            gyro_cal_y = sensofs.gyro_cal_y;
                            gyro_cal_z = sensofs.gyro_cal_z;

                            accel_cal_x = sensofs.accel_cal_x;
                            accel_cal_y = sensofs.accel_cal_y;
                            accel_cal_z = sensofs.accel_cal_z;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.ATTITUDE:


                        {
                            var att = mavLinkMessage.ToStructure<MAVLink.mavlink_attitude_t>();

                            roll = (float)(att.roll * MathHelper.rad2deg);
                            pitch = (float)(att.pitch * MathHelper.rad2deg);
                            yaw = (float)(att.yaw * MathHelper.rad2deg);

                            //Console.WriteLine(MAV.sysid + " " +roll + " " + pitch + " " + yaw);

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.ATTITUDE);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT:

                        {
                            var loc = mavLinkMessage.ToStructure<MAVLink.mavlink_global_position_int_t>();

                            // the new arhs deadreckoning may send 0 alt and 0 long. check for and undo

                            alt = loc.relative_alt / 1000.0f;

                            useLocation = true;
                            if (loc.lat == 0 && loc.lon == 0)
                            {
                                useLocation = false;
                            }
                            else
                            {
                                lat = loc.lat / 10000000.0;
                                lng = loc.lon / 10000000.0;

                                altasl = loc.alt / 1000.0f;

                                vx = loc.vx * 0.01;
                                vy = loc.vy * 0.01;
                                vz = loc.vz * 0.01;
                            }
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT:

                        {
                            var gps = mavLinkMessage.ToStructure<MAVLink.mavlink_gps_raw_int_t>();

                            if (!useLocation)
                            {
                                lat = gps.lat * 1.0e-7;
                                lng = gps.lon * 1.0e-7;

                                altasl = gps.alt / 1000.0f;
                                // alt = gps.alt; // using vfr as includes baro calc
                            }

                            gpsstatus = gps.fix_type;
                            //                    Console.WriteLine("gpsfix {0}",gpsstatus);

                            if (gps.eph != ushort.MaxValue)
                                gpshdop = (float)Math.Round(gps.eph / 100.0, 2);

                            if (gps.satellites_visible != byte.MaxValue)
                                satcount = gps.satellites_visible;

                            if (gps.vel != ushort.MaxValue)
                                groundspeed = gps.vel * 1.0e-2f;

                            if (groundspeed > 0.5 && gps.cog != ushort.MaxValue)
                                groundcourse = gps.cog * 1.0e-2f;

                            if (mavLinkMessage.ismavlink2)
                            {
                                gpsh_acc = gps.h_acc / 1000.0f;
                                gpsv_acc = gps.v_acc / 1000.0f;
                                gpsvel_acc = gps.vel_acc / 1000.0f;
                                gpshdg_acc = gps.hdg_acc / 1e5f;
                            }
                            else
                            {
                                gpsh_acc = -1;
                                gpsv_acc = -1;
                                gpsvel_acc = -1;
                                gpshdg_acc = -1;
                            }

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.GPS_RAW);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.GPS2_RAW:

                        {
                            var gps = mavLinkMessage.ToStructure<MAVLink.mavlink_gps2_raw_t>();

                            lat2 = gps.lat * 1.0e-7;
                            lng2 = gps.lon * 1.0e-7;
                            altasl2 = gps.alt / 1000.0f;

                            gpsstatus2 = gps.fix_type;
                            gpshdop2 = (float)Math.Round(gps.eph / 100.0, 2);

                            satcount2 = gps.satellites_visible;

                            groundspeed2 = gps.vel * 1.0e-2f;
                            groundcourse2 = gps.cog * 1.0e-2f;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.GPS_STATUS:

                        {
                            var gps = mavLinkMessage.ToStructure<MAVLink.mavlink_gps_status_t>();
                            satcount = gps.satellites_visible;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.RADIO:

                        {
                            var radio = mavLinkMessage.ToStructure<MAVLink.mavlink_radio_t>();
                            rssi = radio.rssi;
                            remrssi = radio.remrssi;
                            txbuffer = radio.txbuf;
                            rxerrors = radio.rxerrors;
                            noise = radio.noise;
                            remnoise = radio.remnoise;
                            fixedp = radio.@fixed;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.RADIO_STATUS:

                        {
                            var radio = mavLinkMessage.ToStructure<MAVLink.mavlink_radio_status_t>();
                            rssi = radio.rssi;
                            remrssi = radio.remrssi;
                            txbuffer = radio.txbuf;
                            rxerrors = radio.rxerrors;
                            noise = radio.noise;
                            remnoise = radio.remnoise;
                            fixedp = radio.@fixed;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.MISSION_CURRENT:

                        {
                            var wpcur = mavLinkMessage.ToStructure<MAVLink.mavlink_mission_current_t>();

                            var oldwp = (int)wpno;

                            wpno = wpcur.seq;

                            if (mode.ToLower() == "auto" && wpno != 0) lastautowp = (int)wpno;
                            try
                            {
                                if (oldwp != wpno && Speech != null && Speech.speechEnable && parent != null &&
                                    parent.parent.MAV.cs == this &&
                                    Settings.Instance.GetBoolean("speechwaypointenabled"))
                                    Speech.SpeakAsync(Common.speechConversion(parent,
                                        "" + Settings.Instance["speechwaypoint"]));
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.WAYPOINT_CURRENT);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.NAV_CONTROLLER_OUTPUT:


                        {
                            var nav = mavLinkMessage.ToStructure<MAVLink.mavlink_nav_controller_output_t>();

                            nav_roll = nav.nav_roll;
                            nav_pitch = nav.nav_pitch;
                            nav_bearing = nav.nav_bearing;
                            target_bearing = nav.target_bearing;
                            wp_dist = nav.wp_dist;
                            alt_error = nav.alt_error;
                            aspd_error = nav.aspd_error / 100.0f;
                            xtrack_error = nav.xtrack_error;

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.NAV_CONTROLLER_OUTPUT);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.RPM:


                        {
                            var rpm = mavLinkMessage.ToStructure<MAVLink.mavlink_rpm_t>();

                            rpm1 = rpm.rpm1;
                            rpm2 = rpm.rpm2;

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.NAV_CONTROLLER_OUTPUT);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.RC_CHANNELS_RAW:

                        {
                            var rcin = mavLinkMessage.ToStructure<MAVLink.mavlink_rc_channels_raw_t>();

                            ch1in = rcin.chan1_raw;
                            ch2in = rcin.chan2_raw;
                            ch3in = rcin.chan3_raw;
                            ch4in = rcin.chan4_raw;
                            ch5in = rcin.chan5_raw;
                            ch6in = rcin.chan6_raw;
                            ch7in = rcin.chan7_raw;
                            ch8in = rcin.chan8_raw;

                            //percent
                            rxrssi = (int)(rcin.rssi / 255.0 * 100.0);

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RC_CHANNELS_RAW);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.RC_CHANNELS:

                        {
                            var rcin = mavLinkMessage.ToStructure<MAVLink.mavlink_rc_channels_t>();

                            ch1in = rcin.chan1_raw;
                            ch2in = rcin.chan2_raw;
                            ch3in = rcin.chan3_raw;
                            ch4in = rcin.chan4_raw;
                            ch5in = rcin.chan5_raw;
                            ch6in = rcin.chan6_raw;
                            ch7in = rcin.chan7_raw;
                            ch8in = rcin.chan8_raw;

                            ch9in = rcin.chan9_raw;
                            ch10in = rcin.chan10_raw;
                            ch11in = rcin.chan11_raw;
                            ch12in = rcin.chan12_raw;
                            ch13in = rcin.chan13_raw;
                            ch14in = rcin.chan14_raw;
                            ch15in = rcin.chan15_raw;
                            ch16in = rcin.chan16_raw;

                            //percent
                            rxrssi = (int)(rcin.rssi / 255.0 * 100.0);

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RC_CHANNELS_RAW);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.ESC_TELEMETRY_1_TO_4:

                        {
                            var esc = mavLinkMessage.ToStructure<MAVLink.mavlink_esc_telemetry_1_to_4_t>();
                            esc1_volt = esc.voltage[0] / 100.0f;
                            esc1_curr = esc.current[0] / 100.0f;
                            esc1_rpm = esc.rpm[0];
                            esc1_temp = esc.temperature[0];

                            esc2_volt = esc.voltage[1] / 100.0f;
                            esc2_curr = esc.current[1] / 100.0f;
                            esc2_rpm = esc.rpm[1];
                            esc2_temp = esc.temperature[1];

                            esc3_volt = esc.voltage[2] / 100.0f;
                            esc3_curr = esc.current[2] / 100.0f;
                            esc3_rpm = esc.rpm[2];
                            esc3_temp = esc.temperature[2];

                            esc4_volt = esc.voltage[3] / 100.0f;
                            esc4_curr = esc.current[3] / 100.0f;
                            esc4_rpm = esc.rpm[3];
                            esc4_temp = esc.temperature[3];
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.ESC_TELEMETRY_5_TO_8:

                        {
                            var esc = mavLinkMessage.ToStructure<MAVLink.mavlink_esc_telemetry_5_to_8_t>();
                            esc5_volt = esc.voltage[0] / 100.0f;
                            esc5_curr = esc.current[0] / 100.0f;
                            esc5_rpm = esc.rpm[0];
                            esc5_temp = esc.temperature[0];

                            esc6_volt = esc.voltage[1] / 100.0f;
                            esc6_curr = esc.current[1] / 100.0f;
                            esc6_rpm = esc.rpm[1];
                            esc6_temp = esc.temperature[1];

                            esc7_volt = esc.voltage[2] / 100.0f;
                            esc7_curr = esc.current[2] / 100.0f;
                            esc7_rpm = esc.rpm[2];
                            esc7_temp = esc.temperature[2];

                            esc8_volt = esc.voltage[3] / 100.0f;
                            esc8_curr = esc.current[3] / 100.0f;
                            esc8_rpm = esc.rpm[3];
                            esc8_temp = esc.temperature[3];
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.SERVO_OUTPUT_RAW:

                        {
                            var servoout = mavLinkMessage.ToStructure<MAVLink.mavlink_servo_output_raw_t>();

                            ch1out = servoout.servo1_raw;
                            ch2out = servoout.servo2_raw;
                            ch3out = servoout.servo3_raw;
                            ch4out = servoout.servo4_raw;
                            ch5out = servoout.servo5_raw;
                            ch6out = servoout.servo6_raw;
                            ch7out = servoout.servo7_raw;
                            ch8out = servoout.servo8_raw;

                            // mavlink2 extension
                            ch9out = servoout.servo9_raw;
                            ch10out = servoout.servo10_raw;
                            ch11out = servoout.servo11_raw;
                            ch12out = servoout.servo12_raw;
                            ch13out = servoout.servo13_raw;
                            ch14out = servoout.servo14_raw;
                            ch15out = servoout.servo15_raw;
                            ch16out = servoout.servo16_raw;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.RAW_IMU:

                        {
                            var imu = mavLinkMessage.ToStructure<MAVLink.mavlink_raw_imu_t>();

                            gx = imu.xgyro;
                            gy = imu.ygyro;
                            gz = imu.zgyro;

                            ax = imu.xacc;
                            ay = imu.yacc;
                            az = imu.zacc;

                            mx = imu.xmag;
                            my = imu.ymag;
                            mz = imu.zmag;

                            var timesec = imu.time_usec * 1.0e-6;

                            var deltawall = (DateTime.Now - lastimutime).TotalSeconds;

                            var deltaimu = timesec - imutime;

                            //Console.WriteLine( + " " + deltawall + " " + deltaimu + " " + System.Threading.Thread.CurrentThread.Name);
                            if (deltaimu > 0 && deltaimu < 10)
                            {
                                speedup = (float)(speedup * 0.95 + deltaimu / deltawall * 0.05);

                                imutime = timesec;
                                lastimutime = DateTime.Now;
                            }

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RAW_IMU);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.SCALED_IMU:

                        {
                            var imu = mavLinkMessage.ToStructure<MAVLink.mavlink_scaled_imu_t>();

                            gx = imu.xgyro;
                            gy = imu.ygyro;
                            gz = imu.zgyro;

                            ax = imu.xacc;
                            ay = imu.yacc;
                            az = imu.zacc;

                            mx = imu.xmag;
                            my = imu.ymag;
                            mz = imu.zmag;

                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RAW_IMU);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.SCALED_IMU2:

                        {
                            var imu2 = mavLinkMessage.ToStructure<MAVLink.mavlink_scaled_imu2_t>();

                            gx2 = imu2.xgyro;
                            gy2 = imu2.ygyro;
                            gz2 = imu2.zgyro;

                            ax2 = imu2.xacc;
                            ay2 = imu2.yacc;
                            az2 = imu2.zacc;

                            mx2 = imu2.xmag;
                            my2 = imu2.ymag;
                            mz2 = imu2.zmag;
                        }


                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.SCALED_IMU3:

                        {
                            var imu3 = mavLinkMessage.ToStructure<MAVLink.mavlink_scaled_imu3_t>();

                            gx3 = imu3.xgyro;
                            gy3 = imu3.ygyro;
                            gz3 = imu3.zgyro;

                            ax3 = imu3.xacc;
                            ay3 = imu3.yacc;
                            az3 = imu3.zacc;

                            mx3 = imu3.xmag;
                            my3 = imu3.ymag;
                            mz3 = imu3.zmag;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.PID_TUNING:

                        {
                            var pid = mavLinkMessage.ToStructure<MAVLink.mavlink_pid_tuning_t>();

                            //todo: currently only deals with single axis at once

                            pidff = pid.FF;
                            pidP = pid.P;
                            pidI = pid.I;
                            pidD = pid.D;
                            pidaxis = pid.axis;
                            piddesired = pid.desired;
                            pidachieved = pid.achieved;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.VFR_HUD:

                        {
                            var vfr = mavLinkMessage.ToStructure<MAVLink.mavlink_vfr_hud_t>();

                            groundspeed = vfr.groundspeed;

                            airspeed = vfr.airspeed;

                            //alt = vfr.alt; // this might include baro

                            ch3percent = vfr.throttle;

                            if (sensors_present.revthrottle && sensors_enabled.revthrottle && sensors_health.revthrottle)
                                if (ch3percent > 0)
                                    ch3percent *= -1;

                            //Console.WriteLine(alt);

                            //climbrate = vfr.climb;

                            // heading = vfr.heading;


                            //MAVLink.packets[(byte)MAVLink.MSG_NAMES.VFR_HUD);
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.MEMINFO:

                        {
                            var mem = mavLinkMessage.ToStructure<MAVLink.mavlink_meminfo_t>();
                            if (mem.freemem32 > 0)
                                freemem = mem.freemem32;
                            else
                                freemem = mem.freemem;
                            brklevel = mem.brkval;
                        }

                        break;
                    case (uint)MAVLink.MAVLINK_MSG_ID.AOA_SSA:

                        {
                            var aoa_ssa = mavLinkMessage.ToStructure<MAVLink.mavlink_aoa_ssa_t>();

                            AOA = aoa_ssa.AOA;
                            SSA = aoa_ssa.SSA;
                        }
                        break;

                    case (uint)MAVLink.MAVLINK_MSG_ID.NAMED_VALUE_FLOAT:

                        {
                            var named_float = mavLinkMessage.ToStructure<MAVLink.mavlink_named_value_float_t>();

                            string mav_value_name = Encoding.ASCII.GetString(named_float.name);

                            int ind = mav_value_name.IndexOf('\0');
                            if (ind != -1)
                                mav_value_name = mav_value_name.Substring(0, ind);

                            string name = "MAV_" + mav_value_name.ToUpper();

                            float value = named_float.value;
                            var field = custom_field_names.FirstOrDefault(x => x.Value == name).Key;

                            //todo: if field is null then check if we have a free customfield and add the named_value 
                            if (field == null)
                            {
                                short i;
                                for (i = 0; i < 10; i++)
                                {
                                    if (!custom_field_names.ContainsKey("customfield" + i.ToString())) break;
                                }
                                if (i < 10)
                                {
                                    field = "customfield" + i.ToString();
                                    custom_field_names.Add(field, name);
                                }
                            }


                            if (field != null)
                            {
                                switch (field)
                                {
                                    case "customfield0":
                                        customfield0 = value;
                                        break;
                                    case "customfield1":
                                        customfield1 = value;
                                        break;
                                    case "customfield2":
                                        customfield2 = value;
                                        break;
                                    case "customfield3":
                                        customfield3 = value;
                                        break;
                                    case "customfield4":
                                        customfield4 = value;
                                        break;
                                    case "customfield5":
                                        customfield5 = value;
                                        break;
                                    case "customfield6":
                                        customfield6 = value;
                                        break;
                                    case "customfield7":
                                        customfield7 = value;
                                        break;
                                    case "customfield8":
                                        customfield8 = value;
                                        break;
                                    case "customfield9":
                                        customfield9 = value;
                                        break;
                                    default:
                                        break;
                                }

                            }

                        }
                        break;
                }
            }
        }

        public event EventHandler csCallBack;

        public static double toDistDisplayUnit(double input)
        {
            return input * multiplierdist;
        }

        public static double toSpeedDisplayUnit(double input)
        {
            return input * multiplierspeed;
        }

        public static double fromDistDisplayUnit(double input)
        {
            return input / multiplierdist;
        }

        public static double fromSpeedDisplayUnit(double input)
        {
            return input / multiplierspeed;
        }

        public void ResetInternals()
        {
            lock (this)
            {
                mode = "Unknown";
                _mode = 99999;
                messages = new List<(DateTime time, string message)>();
                useLocation = false;
                rateattitude = rateattitudebackup;
                rateposition = ratepositionbackup;
                ratestatus = ratestatusbackup;
                ratesensors = ratesensorsbackup;
                raterc = ratercbackup;
                datetime = DateTime.MinValue;
                battery_usedmah = 0;
                _lastcurrent = DateTime.MinValue;
                distTraveled = 0;
                timeInAir = 0;
                version = new Version();
                voltageflag = (uint)MAVLink.MAV_POWER_STATUS.USB_CONNECTED;
                capabilities = 0;
            }
        }

        public List<string> GetItemList(bool alpha = false, bool numbersonly = false)
        {
            var ans = new List<string>();

            object thisBoxed = this;
            var test = thisBoxed.GetType();

            // public instance props
            var props = test.GetProperties();

            //props
            foreach (var field in props)
            {
                if (numbersonly)
                    if (!field.PropertyType.IsNumber())
                        continue;

                ans.Add(field.Name);
            }

            if (alpha)
                ans.Sort();

            return ans;
        }

        public string GetNameandUnit(string name)
        {
            var desc = name;

            if (custom_field_names.ContainsKey(name))
            {
                desc = custom_field_names[name];
                return desc;
            }
            try
            {
                var typeofthing = typeof(CurrentState).GetProperty(name);
                if (typeofthing != null)
                {
                    var attrib = typeofthing.GetCustomAttributes(false).OfType<DisplayTextAttribute>().ToArray();
                    if (attrib.Length > 0)
                        desc = attrib.OfType<DisplayTextAttribute>().First().Text;
                }
            }
            catch
            {
            }

            if (desc.Contains("(dist)"))
                desc = desc.Replace("(dist)", "(" + DistanceUnit + ")");
            else if (desc.Contains("(speed)"))
                desc = desc.Replace("(speed)", "(" + SpeedUnit + ")");
            else if (desc.Contains("(alt)")) desc = desc.Replace("(alt)", "(" + AltUnit + ")");

            return desc;
        }


        /// <summary>
        ///     use for main serial port only
        /// </summary>
        /// <param name="bs"></param>
        public void UpdateCurrentSettings(Action<CurrentState> bs)
        {
            UpdateCurrentSettings(bs, false, parent.parent, parent.parent.MAV);
        }

        /// <summary>
        ///     Use the default sysid
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="updatenow"></param>
        /// <param name="mavinterface"></param>
        public void UpdateCurrentSettings(Action<CurrentState> bs, bool updatenow,
            MAVLinkInterface mavinterface)
        {
            UpdateCurrentSettings(bs, updatenow, mavinterface, mavinterface.MAV);
        }

        public void UpdateCurrentSettings(Action<CurrentState> bs, bool updatenow,
            MAVLinkInterface mavinterface, MAVState MAV)
        {
            lock (this)
            {
                if (DateTime.Now > lastupdate.AddMilliseconds(50) || updatenow) // 20 hz
                {
                    lastupdate = DateTime.Now;

                    //check if valid mavinterface
                    if (parent != null && parent.packetsnotlost != 0)
                    {
                        if ((DateTime.Now - MAV.lastvalidpacket).TotalSeconds > 10)
                            linkqualitygcs = 0;
                        else
                            linkqualitygcs =
                                (ushort)(parent.packetsnotlost / (parent.packetsnotlost + parent.packetslost) * 100.0);

                        if (linkqualitygcs > 100)
                            linkqualitygcs = 100;
                    }

                    if (datetime.Second != lastsecondcounter.Second)
                    {
                        lastsecondcounter = datetime;

                        if (lastpos.Lat != 0 && lastpos.Lng != 0 && armed && lat != 0 && lng != 0 && gpsstatus >= 3)
                        {
                            if (mavinterface.BaseStream != null && !mavinterface.BaseStream.IsOpen &&
                                !mavinterface.logreadmode)
                                distTraveled = 0;

                            distTraveled += (float)lastpos.GetDistance(new PointLatLngAlt(lat, lng, 0, "")) *
                                            multiplierdist;
                            lastpos = new PointLatLngAlt(lat, lng, 0, "");
                        }
                        else
                        {
                            lastpos = new PointLatLngAlt(lat, lng, 0, "");
                        }

                        // throttle is up, or groundspeed is > 3 m/s
                        if ((ch3percent > 12 || _groundspeed > 3.0) && armed)
                        {
                            timeInAir++;
                            timeSinceArmInAir++;
                        }

                        // to maintain total timeinair for this session not just based on arming
                        if (!armed)
                            timeSinceArmInAir = 0;

                        if (!gotwind)
                            dowindcalc();
                    }

                    // re-request streams
                    if (!(lastdata.AddSeconds(8) > DateTime.Now) && mavinterface.BaseStream != null &&
                        mavinterface.BaseStream.IsOpen)
                    {
                        try
                        {
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTENDED_STATUS, MAV.cs.ratestatus,
                                MAV.sysid, MAV.compid); // mode
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, MAV.cs.rateposition,
                                MAV.sysid, MAV.compid); // request gps
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA1, MAV.cs.rateattitude,
                                MAV.sysid, MAV.compid); // request attitude
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA2, MAV.cs.rateattitude,
                                MAV.sysid, MAV.compid); // request vfr
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA3, MAV.cs.ratesensors,
                                MAV.sysid,
                                MAV.compid); // request extra stuff - tridge
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MAV.cs.ratesensors,
                                MAV.sysid, MAV.compid); // request raw sensor
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, MAV.cs.raterc,
                                MAV.sysid,
                                MAV.compid); // request rc info
                        }
                        catch
                        {
                            log.Error("Failed to request rates");
                        }

                        lastdata = DateTime.Now.AddSeconds(30); // prevent flooding
                    }
                }

                try
                {
                    if (csCallBack != null)
                        csCallBack(this, null);
                }
                catch
                {
                }

                //Console.Write(DateTime.Now.Millisecond + " start ");
                // update form
                try
                {
                    if (bs != null) bs?.Invoke(this);
                }
                catch
                {
                    log.InfoFormat("CurrentState Binding error");
                }
            }
        }

        public void dowindcalc()
        {
            //Wind Fixed gain Observer
            //Ryan Beall 
            //8FEB10

            var Kw = 0.010; // 0.01 // 0.10

            if (airspeed < 1 || _groundspeed < 1)
                return;

            var Wn_error = airspeed * Math.Cos(yaw * MathHelper.deg2rad) * Math.Cos(pitch * MathHelper.deg2rad) -
                           _groundspeed * Math.Cos(groundcourse * MathHelper.deg2rad) - Wn_fgo;
            var We_error = airspeed * Math.Sin(yaw * MathHelper.deg2rad) * Math.Cos(pitch * MathHelper.deg2rad) -
                           _groundspeed * Math.Sin(groundcourse * MathHelper.deg2rad) - We_fgo;

            Wn_fgo = Wn_fgo + Kw * Wn_error;
            We_fgo = We_fgo + Kw * We_error;

            var wind_dir = Math.Atan2(We_fgo, Wn_fgo) * (180 / Math.PI);
            var wind_vel = Math.Sqrt(Math.Pow(We_fgo, 2) + Math.Pow(Wn_fgo, 2));

            wind_dir = (wind_dir + 360) % 360;

            this.wind_dir = (float)wind_dir; // (float)(wind_dir * 0.5 + this.wind_dir * 0.5);
            this.wind_vel = (float)wind_vel; // (float)(wind_vel * 0.5 + this.wind_vel * 0.5);

            //Console.WriteLine("Wn_error = {0}\nWe_error = {1}\nWn_fgo =    {2}\nWe_fgo =  {3}\nWind_dir =    {4}\nWind_vel =    {5}\n",Wn_error,We_error,Wn_fgo,We_fgo,wind_dir,wind_vel);

            //Console.WriteLine("wind_dir: {0} wind_vel: {1}    as {4} yaw {5} pitch {6} gs {7} cog {8}", wind_dir, wind_vel, Wn_fgo, We_fgo , airspeed,yaw,pitch,groundspeed,groundcourse);

            //low pass the outputs for better results!
        }

        /// <summary>
        ///     derived from MAV_SYS_STATUS_SENSOR
        /// </summary>
        public class Mavlink_Sensors
        {
            private BitArray bitArray = new BitArray(32);

            public bool seen;

            public Mavlink_Sensors()
            {
                //var item = MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL;
            }

            public Mavlink_Sensors(uint p)
            {
                seen = true;
                bitArray = new BitArray(new[] { (int)p });
            }

            public bool gyro
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_GYRO)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_GYRO)] = value;
            }

            public bool accelerometer
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL)] = value;
            }

            public bool compass
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_MAG)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_MAG)] = value;
            }

            public bool barometer
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ABSOLUTE_PRESSURE)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ABSOLUTE_PRESSURE)] =
                    value;
            }

            public bool differential_pressure
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.DIFFERENTIAL_PRESSURE)];
                set =>
                    bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.DIFFERENTIAL_PRESSURE)] =
                        value;
            }

            public bool gps
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.GPS)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.GPS)] = value;
            }

            public bool optical_flow
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.OPTICAL_FLOW)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.OPTICAL_FLOW)] = value;
            }

            public bool VISION_POSITION
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.VISION_POSITION)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.VISION_POSITION)] =
                    value;
            }

            public bool LASER_POSITION
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.LASER_POSITION)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.LASER_POSITION)] =
                    value;
            }

            public bool GROUND_TRUTH
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.EXTERNAL_GROUND_TRUTH)];
                set =>
                    bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.EXTERNAL_GROUND_TRUTH)] =
                        value;
            }

            public bool rate_control
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ANGULAR_RATE_CONTROL)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ANGULAR_RATE_CONTROL)] =
                    value;
            }

            public bool attitude_stabilization
            {
                get =>
                    bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ATTITUDE_STABILIZATION)];
                set =>
                    bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ATTITUDE_STABILIZATION)] =
                        value;
            }

            public bool yaw_position
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.YAW_POSITION)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.YAW_POSITION)] = value;
            }

            public bool altitude_control
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.Z_ALTITUDE_CONTROL)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.Z_ALTITUDE_CONTROL)] =
                    value;
            }

            public bool xy_position_control
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.XY_POSITION_CONTROL)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.XY_POSITION_CONTROL)] =
                    value;
            }

            public bool motor_control
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MOTOR_OUTPUTS)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MOTOR_OUTPUTS)] = value;
            }

            public bool rc_receiver
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.RC_RECEIVER)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.RC_RECEIVER)] = value;
            }

            public bool gyro2
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_GYRO2)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_GYRO2)] = value;
            }

            public bool accel2
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL2)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL2)] = value;
            }

            public bool mag2
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_MAG2)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_MAG2)] = value;
            }

            public bool geofence
            {
                get => bitArray[
                    ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_GEOFENCE)];
                set => bitArray[
                    ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_GEOFENCE)] = value;
            }

            public bool ahrs
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_AHRS)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_AHRS)] =
                    value;
            }

            public bool terrain
            {
                get =>
                    bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_TERRAIN)];
                set =>
                    bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_TERRAIN)] =
                        value;
            }

            public bool revthrottle
            {
                get => bitArray[
                    ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_REVERSE_MOTOR)];
                set => bitArray[
                        ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_REVERSE_MOTOR)] =
                    value;
            }

            public bool logging
            {
                get =>
                    bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_LOGGING)];
                set =>
                    bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_LOGGING)] =
                        value;
            }

            public bool battery
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.BATTERY)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.BATTERY)] = value;
            }

            public bool proximity
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.PROXIMITY)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.PROXIMITY)] = value;
            }

            public bool satcom
            {
                get => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.SATCOM)];
                set => bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.SATCOM)] = value;
            }

            public bool prearm
            {
                get => bitArray[
                    ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_PREARM_CHECK)];
                set => bitArray[
                        ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_PREARM_CHECK)] =
                    value;
            }

            public uint Value
            {
                get
                {
                    var array = new int[1];
                    bitArray.CopyTo(array, 0);
                    return (uint)array[0];
                }
                set
                {
                    seen = true;
                    bitArray = new BitArray(new[] { (int)value });
                }
            }

            private int ConvertValuetoBitmaskOffset(int input)
            {
                var offset = 0;
                for (var a = 0; a < sizeof(int) * 8; a++)
                {
                    offset = 1 << a;
                    if (input == offset)
                        return a;
                }

                return 0;
            }

            public override string ToString()
            {
                return Convert.ToString(Value, 2);
            }
        }
    }
}