using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using MissionPlanner.Utilities;
using log4net;
using MissionPlanner.Attributes;
using MissionPlanner;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using MissionPlanner.ArduPilot;
using Newtonsoft.Json;

namespace MissionPlanner
{
    public class CurrentState : ICloneable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler csCallBack;

        [JsonIgnore]
        [IgnoreDataMember]
        public MAVState parent;

        [JsonIgnore]
        [IgnoreDataMember]
        public static ISpeech Speech;

        public int lastautowp = -1;

        // multipliers
        public static float multiplierdist = 1;
        public static string DistanceUnit = "";
        public static float multiplierspeed = 1;
        public static string SpeedUnit = "";
        public static float multiplieralt = 1;
        public static string AltUnit = "";

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

        // orientation - rads
        [DisplayText("Roll (deg)")]
        public float roll { get; set; }

        [DisplayText("Pitch (deg)")]
        public float pitch { get; set; }

        [DisplayText("Yaw (deg)")]
        public float yaw
        {
            get { return _yaw; }
            set
            {
                if (value < 0)
                {
                    _yaw = value + 360;
                }
                else
                {
                    _yaw = value;
                }
            }
        }

        private float _yaw = 0;

        [DisplayText("SSA (deg)")]
        public float SSA { get; set; }

        [DisplayText("AOA (deg)")]
        public float AOA { get; set; }

        [DisplayText("GroundCourse (deg)")]
        public float groundcourse
        {
            get { return _groundcourse; }
            set
            {
                if (value < 0)
                {
                    _groundcourse = value + 360;
                }
                else
                {
                    _groundcourse = value;
                }
            }
        }



        private float _groundcourse = 0;

        // position
        [DisplayText("Latitude (dd)")]
        public double lat { get; set; }

        [DisplayText("Longitude (dd)")]
        public double lng { get; set; }

        [DisplayText("Altitude (alt)")]
        public float alt
        {
            get { return (_alt - altoffsethome) * multiplieralt; }
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

        DateTime lastalt = DateTime.MinValue;

        [DisplayText("Altitude (dist)")]
        public float altasl
        {
            get { return _altasl * multiplieralt; }
            set { _altasl = value; }
        }

        [DisplayText("Horizon Dist (dist)")]
        public float horizondist
        {
            // alt = above home
            get { return (float)(3570 * Math.Sqrt(alt)) * multiplierdist; }
        }

        float _altasl = 0;
        float oldalt = 0;

        [DisplayText("Velocity X (ms)")]
        public double vx { get; set; }
        [DisplayText("Velocity Y (ms)")]
        public double vy { get; set; }
        [DisplayText("Velocity Z (ms)")]
        public double vz { get; set; }

        public double vlen
        {
            get { return Math.Sqrt(Math.Pow(vx, 2) + Math.Pow(vy, 2) + Math.Pow(vz, 2)); }
        }

        [DisplayText("Alt Home Offset (dist)")]
        public float altoffsethome { get; set; }

        private float _alt = 0;

        [DisplayText("Gps Status")]
        public float gpsstatus { get; set; }

        [DisplayText("Gps HDOP")]
        public float gpshdop { get; set; }

        [DisplayText("Sat Count")]
        public float satcount { get; set; }

        [DisplayText("Horizontal Accuracy")]
        public float gpsh_acc { get; private set; }
        [DisplayText("Vertical Accuracy")]
        public float gpsv_acc { get; private set; }
        [DisplayText("Velocity Accuracy")]
        public float gpsvel_acc { get; private set; }
        [DisplayText("Heading Accuracy")]
        public float gpshdg_acc { get; private set; }

        [DisplayText("Latitude2 (dd)")]
        public double lat2 { get; set; }

        [DisplayText("Longitude2 (dd)")]
        public double lng2 { get; set; }

        [DisplayText("Altitude2 (dist)")]
        public float altasl2 { get; set; }

        [DisplayText("Gps Status2")]
        public float gpsstatus2 { get; set; }

        [DisplayText("Gps HDOP2")]
        public float gpshdop2 { get; set; }

        [DisplayText("Sat Count2")]
        public float satcount2 { get; set; }

        public float groundspeed2 { get; set; }

        [DisplayText("GroundCourse2 (deg)")]
        public float groundcourse2 { get; set; }

        [DisplayText("Sat Count Blend")]
        public float satcountB { get { return satcount + satcount2; } }

        public DateTime gpstime { get; set; }

        public float altd1000
        {
            get { return (alt / 1000) % 10; }
        }

        public float altd100
        {
            get { return (alt / 100) % 10; }
        }

        // speeds
        [DisplayText("AirSpeed (speed)")]
        public float airspeed
        {
            get { return _airspeed * multiplierspeed; }
            set { _airspeed = value; }
        }

        [DisplayText("Airspeed Target (speed)")]
        public float targetairspeed
        {
            get { return _targetairspeed; }
        }

        public bool lowairspeed { get; set; }

        [DisplayText("Airspeed Ratio")]
        public float asratio { get; set; }

        [DisplayText("GroundSpeed (speed)")]
        public float groundspeed
        {
            get { return _groundspeed * multiplierspeed; }
            set { _groundspeed = value; }
        }

        // accel
        [DisplayText("Accel X")]
        public float ax { get; set; }

        [DisplayText("Accel Y")]
        public float ay { get; set; }

        [DisplayText("Accel Z")]
        public float az { get; set; }

        [DisplayText("Accel Strength")]
        public float accelsq
        {
            get { return (float)Math.Sqrt(Math.Pow(ax, 2) + Math.Pow(ay, 2) + Math.Pow(az, 2)) / 1000.0f /*980.665f*/; }
        }

        // gyro
        [DisplayText("Gyro X")]
        public float gx { get; set; }

        [DisplayText("Gyro Y")]
        public float gy { get; set; }

        [DisplayText("Gyro Z")]
        public float gz { get; set; }

        [DisplayText("Gyro Strength")]
        public float gyrosq
        {
            get { return (float)Math.Sqrt(Math.Pow(gx, 2) + Math.Pow(gy, 2) + Math.Pow(gz, 2)); }
        }

        // mag
        [DisplayText("Mag X")]
        public float mx { get; set; }

        [DisplayText("Mag Y")]
        public float my { get; set; }

        [DisplayText("Mag Z")]
        public float mz { get; set; }

        [DisplayText("Mag Field")]
        public float magfield
        {
            get { return (float)Math.Sqrt(Math.Pow(mx, 2) + Math.Pow(my, 2) + Math.Pow(mz, 2)); }
        }

        // accel2
        [DisplayText("Accel2 X")]
        public float ax2 { get; set; }

        [DisplayText("Accel2 Y")]
        public float ay2 { get; set; }

        [DisplayText("Accel2 Z")]
        public float az2 { get; set; }

        [DisplayText("Accel Strength2")]
        public float accelsq2
        {
            get { return (float)Math.Sqrt(Math.Pow(ax2, 2) + Math.Pow(ay2, 2) + Math.Pow(az2, 2)) / 1000.0f /*980.665f*/; }
        }

        // gyro2
        [DisplayText("Gyro2 X")]
        public float gx2 { get; set; }

        [DisplayText("Gyro2 Y")]
        public float gy2 { get; set; }

        [DisplayText("Gyro2 Z")]
        public float gz2 { get; set; }

        [DisplayText("Gyro Strength2")]
        public float gyrosq2
        {
            get { return (float)Math.Sqrt(Math.Pow(gx2, 2) + Math.Pow(gy2, 2) + Math.Pow(gz2, 2)); }
        }

        // mag2
        [DisplayText("Mag2 X")]
        public float mx2 { get; set; }

        [DisplayText("Mag2 Y")]
        public float my2 { get; set; }

        [DisplayText("Mag2 Z")]
        public float mz2 { get; set; }

        [DisplayText("Mag Field2")]
        public float magfield2
        {
            get { return (float)Math.Sqrt(Math.Pow(mx2, 2) + Math.Pow(my2, 2) + Math.Pow(mz2, 2)); }
        }

        // accel3
        [DisplayText("Accel3 X")]
        public float ax3 { get; set; }

        [DisplayText("Accel3 Y")]
        public float ay3 { get; set; }

        [DisplayText("Accel3 Z")]
        public float az3 { get; set; }

        [DisplayText("Accel Strength3")]
        public float accelsq3
        {
            get { return (float)Math.Sqrt(Math.Pow(ax3, 2) + Math.Pow(ay3, 2) + Math.Pow(az3, 2)) / 1000.0f /*980.665f*/; }
        }

        // gyro3
        [DisplayText("Gyro3 X")]
        public float gx3 { get; set; }

        [DisplayText("Gyro3 Y")]
        public float gy3 { get; set; }

        [DisplayText("Gyro3 Z")]
        public float gz3 { get; set; }

        [DisplayText("Gyro Strength3")]
        public float gyrosq3
        {
            get { return (float)Math.Sqrt(Math.Pow(gx3, 2) + Math.Pow(gy3, 2) + Math.Pow(gz3, 2)); }
        }

        // mag3
        [DisplayText("Mag3 X")]
        public float mx3 { get; set; }

        [DisplayText("Mag3 Y")]
        public float my3 { get; set; }

        [DisplayText("Mag3 Z")]
        public float mz3 { get; set; }

        [DisplayText("Mag Field3")]
        public float magfield3
        {
            get { return (float)Math.Sqrt(Math.Pow(mx3, 2) + Math.Pow(my3, 2) + Math.Pow(mz3, 2)); }
        }

        //radio
        public float ch1in { get; set; }
        public float ch2in { get; set; }
        public float ch3in { get; set; }
        public float ch4in { get; set; }
        public float ch5in { get; set; }
        public float ch6in { get; set; }
        public float ch7in { get; set; }
        public float ch8in { get; set; }

        public float ch9in { get; set; }
        public float ch10in { get; set; }
        public float ch11in { get; set; }
        public float ch12in { get; set; }
        public float ch13in { get; set; }
        public float ch14in { get; set; }
        public float ch15in { get; set; }
        public float ch16in { get; set; }

        // motors
        public float ch1out { get; set; }
        public float ch2out { get; set; }
        public float ch3out { get; set; }
        public float ch4out { get; set; }
        public float ch5out { get; set; }
        public float ch6out { get; set; }
        public float ch7out { get; set; }
        public float ch8out { get; set; }

        public float ch9out { get; set; }
        public float ch10out { get; set; }
        public float ch11out { get; set; }
        public float ch12out { get; set; }
        public float ch13out { get; set; }
        public float ch14out { get; set; }
        public float ch15out { get; set; }
        public float ch16out { get; set; }

        public float esc1_volt { get; set; }
        public float esc1_curr { get; set; }
        public float esc1_rpm { get; set; }
        public float esc1_temp { get; set; }

        public float esc2_volt { get; set; }
        public float esc2_curr { get; set; }
        public float esc2_rpm { get; set; }
        public float esc2_temp { get; set; }

        public float esc3_volt { get; set; }
        public float esc3_curr { get; set; }
        public float esc3_rpm { get; set; }
        public float esc3_temp { get; set; }

        public float esc4_volt { get; set; }
        public float esc4_curr { get; set; }
        public float esc4_rpm { get; set; }
        public float esc4_temp { get; set; }

        public float esc5_volt { get; set; }
        public float esc5_curr { get; set; }
        public float esc5_rpm { get; set; }
        public float esc5_temp { get; set; }

        public float esc6_volt { get; set; }
        public float esc6_curr { get; set; }
        public float esc6_rpm { get; set; }
        public float esc6_temp { get; set; }

        public float esc7_volt { get; set; }
        public float esc7_curr { get; set; }
        public float esc7_rpm { get; set; }
        public float esc7_temp { get; set; }

        public float esc8_volt { get; set; }
        public float esc8_curr { get; set; }
        public float esc8_rpm { get; set; }
        public float esc8_temp { get; set; }

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
                    {
                        return
                            (int)
                                (((ch3out - parent.parent.MAV.param["RC3_MIN"].Value) /
                                  (parent.parent.MAV.param["RC3_MAX"].Value - parent.parent.MAV.param["RC3_MIN"].Value)) *
                                 100);
                    }
                    else
                    {
                        if (ch3out == 0)
                            return 0;
                        return (int)((ch3out - 1100) / (1900 - 1100) * 100);
                    }
                }
                catch
                {
                    return 0;
                }
            }

            set { _ch3percent = value; }
        }

        [DisplayText("Failsafe")]
        public bool failsafe { get; set; }

        [DisplayText("RX Rssi")]
        public int rxrssi { get; set; }

        float _ch3percent = -1;

        public float crit_AOA
        {
            get
            {
                try
                {
                    if (parent.parent.MAV.param.ContainsKey("AOA_CRIT"))
                    {
                        return
                            (int)
                                (parent.parent.MAV.param["AOA_CRIT"].Value);
                    }
                    else
                    {
                        return 25;
                    }
                }
                catch
                {
                    return 0;
                }
            }

            set { _crit_aoa = value; }
        }
        float _crit_aoa = 25;

        public bool lowgroundspeed { get; set; }
        float _airspeed;
        float _groundspeed;
        float _verticalspeed;

        [DisplayText("Vertical Speed (speed)")]
        public float verticalspeed
        {
            get
            {
                if (float.IsNaN(_verticalspeed)) _verticalspeed = 0;
                return _verticalspeed * multiplierspeed;
            }
            set { _verticalspeed = _verticalspeed * 0.4f + value * 0.6f; }
        }


        //nav state
        [DisplayText("Roll Target (deg)")]
        public float nav_roll { get; set; }

        [DisplayText("Pitch Target (deg)")]
        public float nav_pitch { get; set; }

        [DisplayText("Bearing Target (deg)")]
        public float nav_bearing { get; set; }

        [DisplayText("Bearing Target (deg)")]
        public float target_bearing { get; set; }

        [DisplayText("Dist to WP (dist)")]
        public float wp_dist
        {
            get { return (_wpdist * multiplierdist); }
            set { _wpdist = value; }
        }

        [DisplayText("Altitude Error (dist)")]
        public float alt_error
        {
            get { return _alt_error * multiplieralt; }
            set
            {
                if (_alt_error == value) return;
                _alt_error = value;
                _targetalt = _targetalt * 0.5f + (float)Math.Round(alt + alt_error, 0) * 0.5f;
            }
        }

        [DisplayText("Bearing Error (deg)")]
        public float ber_error
        {
            get { return (target_bearing - yaw); }
            set { }
        }

        [DisplayText("Airspeed Error (speed)")]
        public float aspd_error
        {
            get { return _aspd_error * multiplierspeed; }
            set
            {
                if (_aspd_error == value) return;
                _aspd_error = value;
                _targetairspeed = _targetairspeed * 0.5f + (float)Math.Round(airspeed + aspd_error, 0) * 0.5f;
            }
        }

        [DisplayText("Xtrack Error (m)")]
        public float xtrack_error { get; set; }

        [DisplayText("WP No")]
        public float wpno { get; set; }

        [DisplayText("Mode")]
        public string mode { get; set; }

        uint _mode = 99999;

        [DisplayText("ClimbRate (speed)")]
        public float climbrate
        {
            get { return _climbrate * multiplierspeed; }
            set { _climbrate = value; }
        }


        /// <summary>
        /// time over target in seconds
        /// </summary>
        [DisplayText("Time over Target (sec)")]
        public int tot
        {
            get
            {
                if (_groundspeed <= 0) return 0;
                return (int)(_wpdist / _groundspeed);
            }
        }

        [DisplayText("Time over Home (sec)")]
        public int toh
        {
            get
            {
                if (_groundspeed <= 0) return 0;
                return (int)(DistToHome / groundspeed);
            }
        }

        [DisplayText("Dist Traveled (dist)")]
        public float distTraveled { get; set; }

        [DisplayText("Time in Air (sec)")]
        public float timeSinceArmInAir { get; set; }

        [DisplayText("Time in Air (sec)")]
        public float timeInAir { get; set; }

        //Time in Air converted to min.sec format for easier reading
        [DisplayText("Time in Air (min.sec)")]
        public float timeInAirMinSec
        {
            get
            {
                return ((float)((int)(timeInAir / 60))) + ((timeInAir % 60) / 100);
            }
        }

        // calced turn rate
        [DisplayText("Turn Rate (speed)")]
        public float turnrate
        {
            get
            {
                if (_groundspeed <= 1) return 0;
                return (float)((roll * 9.80665) / groundspeed);
            }
        }

        //https://en.wikipedia.org/wiki/Load_factor_(aeronautics)
        [DisplayText("Turn Gs (load)")]
        public float turng
        {
            get { return (float)(1 / Math.Cos(MathHelper.deg2rad * roll)); }
        }

        // turn radius
        [DisplayText("Turn Radius (dist)")]
        public float radius
        {
            get
            {
                if (_groundspeed <= 1) return 0;
                return (float)((groundspeed * groundspeed) / (9.80665 * Math.Tan(roll * MathHelper.deg2rad)));
            }
        }

        float _wpdist;
        float _aspd_error;
        float _alt_error;
        float _targetalt;
        float _targetairspeed;
        float _climbrate;

        public float QNH
        {
            get
            {
                var pressure = press_abs;
                var alt_m = altasl;

                var gndtemp = 15f;              
                var C_TO_KELVIN = 273.15f;               
                float temp = gndtemp + C_TO_KELVIN; // kelvin
                float scaling = (float)Math.Exp(Math.Log(1.0 - (alt_m / (153.8462 * temp))) / 0.190259);
                float base_pressure = pressure / scaling;
                return base_pressure;
            }
        }

        [DisplayText("Wind Direction (Deg)")]
        public float wind_dir { get; set; }

        [DisplayText("Wind Velocity (speed)")]
        public float wind_vel { get; set; }

        /// <summary>
        /// used in wind calc
        /// </summary>
        double Wn_fgo;

        /// <summary>
        /// used for wind calc
        /// </summary>
        double We_fgo;

        public float targetaltd100
        {
            get { return (_targetalt / 100) % 10; }
        }

        public float targetalt
        {
            get { return _targetalt; }
        }

        //airspeed_error = (airspeed_error - airspeed);

        //message
        public List<string> messages { get; set; }

        public string message
        {
            get
            {
                if (messages.Count == 0) return "";
                return messages[messages.Count - 1];
            }
        }

        public string messageHigh
        {
            get { return _messagehigh; }
            set { _messagehigh = value; }
        }

        private string _messagehigh = "";
        public DateTime messageHighTime { get; set; }

        //battery
        [DisplayText("Bat Voltage (V)")]
        public double battery_voltage
        {
            get { return _battery_voltage; }
            set
            {
                if (_battery_voltage == 0) _battery_voltage = value;
                _battery_voltage = value * 0.4f + _battery_voltage * 0.6f;
            }
        }

        internal double _battery_voltage;

        [DisplayText("Bat Voltage (V)")] public double battery_voltage3 { get; set; }
        [DisplayText("Bat Voltage (V)")] public double battery_voltage4 { get; set; }
        [DisplayText("Bat Voltage (V)")] public double battery_voltage5 { get; set; }
        [DisplayText("Bat Voltage (V)")] public double battery_voltage6 { get; set; }
        [DisplayText("Bat Voltage (V)")] public double battery_voltage7 { get; set; }
        [DisplayText("Bat Voltage (V)")] public double battery_voltage8 { get; set; }

        [DisplayText("Bat Remaining (%)")]
        public int battery_remaining
        {
            get { return _battery_remaining; }
            set
            {
                _battery_remaining = value;
                if (_battery_remaining < 0 || _battery_remaining > 100) _battery_remaining = 0;
            }
        }

        private int _battery_remaining;

        [DisplayText("Bat Remaining (%)")] public int battery_remaining2 { get; set; }
        [DisplayText("Bat Remaining (%)")] public int battery_remaining3 { get; set; }
        [DisplayText("Bat Remaining (%)")] public int battery_remaining4 { get; set; }
        [DisplayText("Bat Remaining (%)")] public int battery_remaining5 { get; set; }
        [DisplayText("Bat Remaining (%)")] public int battery_remaining6 { get; set; }
        [DisplayText("Bat Remaining (%)")] public int battery_remaining7 { get; set; }
        [DisplayText("Bat Remaining (%)")] public int battery_remaining8 { get; set; }

        [DisplayText("Bat Current (Amps)")]
        public double current
        {
            get { return _current; }
            set
            {
                if (_lastcurrent == DateTime.MinValue) _lastcurrent = datetime;
                // case for no sensor
                if (value == -0.01f)
                {
                    _current = 0;
                    return;
                }
                battery_usedmah += ((value * 1000.0) * (datetime - _lastcurrent).TotalHours);
                _current = value;
                _lastcurrent = datetime;
            }
        } //current may to be below zero - recuperation in arduplane
        private double _current;


        private DateTime _lastcurrent2 = DateTime.MinValue;

        [DisplayText("Bat2 Current (Amps)")]
        public double current2
        {
            get { return _current2; }
            set
            {
                if (_lastcurrent2 == DateTime.MinValue) _lastcurrent2 = datetime;
                if (value < 0) return;
                battery_usedmah2 += ((value * 1000.0) * (datetime - _lastcurrent2).TotalHours);
                _current2 = value;
                _lastcurrent2 = datetime;
            }
        }

        private double _current2;

        [DisplayText("Bat2 Current (Amps)")] public double current3 { get; set; }
        [DisplayText("Bat2 Current (Amps)")] public double current4 { get; set; }
        [DisplayText("Bat2 Current (Amps)")] public double current5 { get; set; }
        [DisplayText("Bat2 Current (Amps)")] public double current6 { get; set; }
        [DisplayText("Bat2 Current (Amps)")] public double current7 { get; set; }
        [DisplayText("Bat2 Current (Amps)")] public double current8 { get; set; }

        [DisplayText("Bat Watts")]
        public double watts
        {
            get { return battery_voltage * current; }
        }

        private DateTime _lastcurrent = DateTime.MinValue;

        [DisplayText("Bat efficiency (mah/km)")]
        public double battery_mahperkm { get { return battery_usedmah / (distTraveled / 1000.0f); } }

        [DisplayText("Bat km left EST (km)")]
        public double battery_kmleft { get { return (((100.0f / (100.0f - battery_remaining)) * battery_usedmah) - battery_usedmah) / battery_mahperkm; } }

        [DisplayText("Bat used EST (mah)")]
        public double battery_usedmah { get; set; }

        public double battery_cell1 { get; set; }
        public double battery_cell2 { get; set; }
        public double battery_cell3 { get; set; }
        public double battery_cell4 { get; set; }
        public double battery_cell5 { get; set; }
        public double battery_cell6 { get; set; }

        public double battery_temp { get; set; }

        [DisplayText("Bat used EST (mah)")] public double battery_usedmah2 { get; set; }
        [DisplayText("Bat used EST (mah)")] public double battery_usedmah3 { get; set; }
        [DisplayText("Bat used EST (mah)")] public double battery_usedmah4 { get; set; }
        [DisplayText("Bat used EST (mah)")] public double battery_usedmah5 { get; set; }
        [DisplayText("Bat used EST (mah)")] public double battery_usedmah6 { get; set; }
        [DisplayText("Bat used EST (mah)")] public double battery_usedmah7 { get; set; }
        [DisplayText("Bat used EST (mah)")] public double battery_usedmah8 { get; set; }


        [DisplayText("Bat2 Voltage (V)")]
        public double battery_voltage2
        {
            get { return _battery_voltage2; }
            set
            {
                if (_battery_voltage2 == 0) _battery_voltage2 = value;
                _battery_voltage2 = value * 0.4f + _battery_voltage2 * 0.6f;
            }
        }

        internal double _battery_voltage2;


        public double HomeAlt
        {
            get { return HomeLocation.Alt; }
            set { }
        }

        static PointLatLngAlt _homelocation = new PointLatLngAlt();

        public PointLatLngAlt HomeLocation
        {
            get { return _homelocation; }
            set { _homelocation = value; }
        }

        PointLatLngAlt _movingbase = new PointLatLngAlt();

        public PointLatLngAlt MovingBase
        {
            get { return _movingbase; }
            set
            {
                if (value == null)
                    _movingbase = new PointLatLngAlt();

                if (_movingbase.Lat != value.Lat || _movingbase.Lng != value.Lng || _movingbase.Alt
                    != value.Alt)
                    _movingbase = value;
            }
        }

        static PointLatLngAlt _trackerloc = new PointLatLngAlt();

        public PointLatLngAlt TrackerLocation
        {
            get
            {
                if (_trackerloc.Lng != 0) return _trackerloc;
                return HomeLocation;
            }
            set { _trackerloc = value; }
        }

        public PointLatLngAlt Location
        {
            get { return new PointLatLngAlt(lat, lng, altasl); }
        }

        public PointLatLngAlt TargetLocation { get; set; } = PointLatLngAlt.Zero;

        public float GeoFenceDist
        {
            get
            {
                try
                {
                    float disttotal = 99999;
                    PointLatLngAlt lineStartLatLngAlt = null;
                    var R = 6371e3;
                    // close loop
                    var list = parent.parent.MAV.fencepoints.ToList();
                    if (list.Count > 0)
                    {
                        // remove return location
                        list.RemoveAt(0);
                    }

                    // check all segments
                    foreach (var mavlinkFencePointT in list)
                    {
                        if (lineStartLatLngAlt == null)
                        {
                            lineStartLatLngAlt = new PointLatLngAlt(mavlinkFencePointT.Value.lat,
                                mavlinkFencePointT.Value.lng);
                            continue;
                        }

                        // crosstrack distance
                        var lineEndLatLngAlt = new PointLatLngAlt(mavlinkFencePointT.Value.lat, mavlinkFencePointT.Value.lng);

                        var lineDist = lineStartLatLngAlt.GetDistance2(lineEndLatLngAlt);

                        var distToLocation = lineStartLatLngAlt.GetDistance2(Location);
                        var bearToLocation = lineStartLatLngAlt.GetBearing(Location);
                        var lineBear = lineStartLatLngAlt.GetBearing(lineEndLatLngAlt);

                        var angle = bearToLocation - lineBear;
                        if (angle < 0)
                            angle += 360;

                        var alongline = Math.Cos(angle * MathHelper.deg2rad) * distToLocation;

                        // check to see if our point is still within the line length
                        if (alongline > lineDist)
                        {
                            lineStartLatLngAlt = lineEndLatLngAlt;
                            continue;
                        }

                        var dXt2 = Math.Sin(angle * MathHelper.deg2rad) * distToLocation;

                        var dXt = Math.Asin(Math.Sin(distToLocation / R) * Math.Sin(angle * MathHelper.deg2rad)) * R;

                        disttotal = (float)Math.Min(disttotal, Math.Abs(dXt2));

                        lineStartLatLngAlt = lineEndLatLngAlt;
                    }

                    // check also distance from the points - because if we are outside the polygon, we may be on a corner segment
                    foreach (var mavlinkFencePointT in list)
                    {
                        var pathpoint = new PointLatLngAlt(mavlinkFencePointT.Value.lat, mavlinkFencePointT.Value.lng);
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

        [DisplayText("Dist to Home (dist)")]
        public float DistToHome
        {
            get
            {
                if (lat == 0 && lng == 0 || TrackerLocation.Lat == 0)
                    return 0;

                // shrinking factor for longitude going to poles direction
                double rads = Math.Abs(TrackerLocation.Lat) * 0.0174532925;
                double scaleLongDown = Math.Cos(rads);
                double scaleLongUp = 1.0f / Math.Cos(rads);

                //DST to Home
                double dstlat = Math.Abs(TrackerLocation.Lat - lat) * 111319.5;
                double dstlon = Math.Abs(TrackerLocation.Lng - lng) * 111319.5 * scaleLongDown;
                return (float)Math.Sqrt((dstlat * dstlat) + (dstlon * dstlon)) * multiplierdist;
            }
        }

        [DisplayText("Dist to Moving Base (dist)")]
        public float DistFromMovingBase
        {
            get
            {
                if (lat == 0 && lng == 0 || MovingBase == null)
                    return 0;

                // shrinking factor for longitude going to poles direction
                double rads = Math.Abs(MovingBase.Lat) * 0.0174532925;
                double scaleLongDown = Math.Cos(rads);
                double scaleLongUp = 1.0f / Math.Cos(rads);

                //DST to Home
                double dstlat = Math.Abs(MovingBase.Lat - lat) * 111319.5;
                double dstlon = Math.Abs(MovingBase.Lng - lng) * 111319.5 * scaleLongDown;
                return (float)Math.Sqrt((dstlat * dstlat) + (dstlon * dstlon)) * multiplierdist;
            }
        }

        [DisplayText("Elevation to Mav (deg)")]
        public float ELToMAV
        {
            get
            {
                float dist = DistToHome / multiplierdist;

                if (dist < 5)
                    return 0;

                float altdiff = (float)(_altasl - TrackerLocation.Alt);

                float angle = (float)(Math.Atan(altdiff / dist) * MathHelper.rad2deg);

                return angle;
            }
        }

        [DisplayText("Bearing to Mav (deg)")]
        public float AZToMAV
        {
            get
            {
                // shrinking factor for longitude going to poles direction
                double rads = Math.Abs(TrackerLocation.Lat) * 0.0174532925;
                double scaleLongDown = Math.Cos(rads);
                double scaleLongUp = 1.0f / Math.Cos(rads);

                //DIR to Home
                double dstlon = (TrackerLocation.Lng - lng); //OffSet_X
                double dstlat = (TrackerLocation.Lat - lat) * scaleLongUp; //OffSet Y
                double bearing = 90 + (Math.Atan2(dstlat, -dstlon) * 57.295775); //absolut home direction
                if (bearing < 0) bearing += 360; //normalization
                //bearing = bearing - 180;//absolut return direction
                //if (bearing < 0) bearing += 360;//normalization

                float dist = DistToHome / multiplierdist;

                if (dist < 5)
                    return 0;

                return (float)bearing;
            }
        }

        [DisplayText("Sonar Range (meters)")]
        public float sonarrange
        {
            get { return (float)toDistDisplayUnit(_sonarrange); }
            set { _sonarrange = value; }
        }

        float _sonarrange = 0;

        [DisplayText("Sonar Voltage (Volt)")]
        public float sonarvoltage { get; set; }

        [DisplayText("RangeFinder1 (cm)")]
        public uint rangefinder1 { get; set; }

        [DisplayText("RangeFinder2 (cm)")]
        public uint rangefinder2 { get; set; }

        [DisplayText("RangeFinder3 (cm)")]
        public uint rangefinder3 { get; set; }

        // current firmware
        public Firmwares firmware = Firmwares.ArduCopter2;
        public float freemem { get; set; }
        public float load { get; set; }
        public float brklevel { get; set; }
        public bool armed { get; set; }

        // Sik radio
        [DisplayText("Sik Radio rssi")]
        public float rssi { get; set; }

        [DisplayText("Sik Radio remote rssi")]
        public float remrssi { get; set; }

        public byte txbuffer { get; set; }

        [DisplayText("Sik Radio noise")]
        public float noise { get; set; }

        [DisplayText("Sik Radio remote noise")]
        public float remnoise { get; set; }

        public ushort rxerrors { get; set; }
        public ushort fixedp { get; set; }
        private float _localsnrdb = 0;
        private float _remotesnrdb = 0;
        private DateTime lastrssi = DateTime.Now;
        private DateTime lastremrssi = DateTime.Now;

        [DisplayText("Sik Radio snr")]
        public float localsnrdb
        {
            get
            {
                if (lastrssi.AddSeconds(1) > DateTime.Now)
                {
                    return _localsnrdb;
                }
                lastrssi = DateTime.Now;
                _localsnrdb = ((rssi - noise) / 1.9f) * 0.5f + _localsnrdb * 0.5f;
                return _localsnrdb;
            }
        }

        [DisplayText("Sik Radio remote snr")]
        public float remotesnrdb
        {
            get
            {
                if (lastremrssi.AddSeconds(1) > DateTime.Now)
                {
                    return _remotesnrdb;
                }
                lastremrssi = DateTime.Now;
                _remotesnrdb = ((remrssi - remnoise) / 1.9f) * 0.5f + _remotesnrdb * 0.5f;
                return _remotesnrdb;
            }
        }

        [DisplayText("Sik Radio est dist (m)")]
        public float DistRSSIRemain
        {
            get
            {
                float work = 0;
                if (localsnrdb == 0)
                {
                    return 0;
                }
                if (localsnrdb > remotesnrdb)
                {
                    // remote
                    // minus fade margin
                    work = remotesnrdb - 5;
                }
                else
                {
                    // local
                    // minus fade margin
                    work = localsnrdb - 5;
                }

                {
                    float dist = DistToHome / multiplierdist;

                    work = dist * (float)Math.Pow(2.0, work / 6.0);
                }

                return work;
            }
        }

        // stats
        public ushort packetdropremote { get; set; }
        public ushort linkqualitygcs { get; set; }

        [DisplayText("HW Voltage")]
        public float hwvoltage { get; set; }

        [DisplayText("Board Voltage")]
        public float boardvoltage { get; set; }

        [DisplayText("Servo Rail Voltage")]
        public float servovoltage { get; set; }

        [DisplayText("Voltage Flags")]
        public uint voltageflag { get; set; }

        public ushort i2cerrors { get; set; }

        public double timesincelastshot { get; set; }

        // pressure
        public float press_abs { get; set; }
        public int press_temp { get; set; }
        public float press_abs2 { get; set; }
        public int press_temp2 { get; set; }

        // sensor offsets
        public int mag_ofs_x { get; set; }
        public int mag_ofs_y { get; set; }
        public int mag_ofs_z { get; set; }
        public float mag_declination { get; set; }
        public int raw_press { get; set; }
        public int raw_temp { get; set; }
        public float gyro_cal_x { get; set; }
        public float gyro_cal_y { get; set; }
        public float gyro_cal_z { get; set; }
        public float accel_cal_x { get; set; }
        public float accel_cal_y { get; set; }
        public float accel_cal_z { get; set; }

        // requested stream rates
        public int rateattitude { get; set; }
        public int rateposition { get; set; }
        public int ratestatus { get; set; }
        public int ratesensors { get; set; }
        public int raterc { get; set; }

        public static int rateattitudebackup;
        public static int ratepositionbackup;
        public static int ratestatusbackup;
        public static int ratesensorsbackup;
        public static int ratercbackup;

        // reference
        public DateTime datetime { get; set; }

        public bool connected
        {
            get { return ((parent.parent.BaseStream != null && parent.parent.BaseStream.IsOpen) || parent.parent.logreadmode); }
        }


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

        public bool terrainactive { get; set; }

        float _ter_curalt;

        [DisplayText("Terrain AGL")]
        public float ter_curalt
        {
            get { return _ter_curalt * multiplieralt; }
            set { _ter_curalt = value; }
        }

        float _ter_alt;

        [DisplayText("Terrain GL")]
        public float ter_alt
        {
            get { return _ter_alt * multiplieralt; }
            set { _ter_alt = value; }
        }

        public float ter_load { get; set; }

        public float ter_pend { get; set; }

        public float ter_space { get; set; }

        public static int KIndexstatic = -1;

        public int KIndex
        {
            get { return (int)CurrentState.KIndexstatic; }
        }

        [DisplayText("flow_comp_m_x")]
        public float opt_m_x { get; set; }

        [DisplayText("flow_comp_m_y")]
        public float opt_m_y { get; set; }

        [DisplayText("flow_x")]
        public short opt_x { get; set; }

        [DisplayText("flow_y")]
        public short opt_y { get; set; }

        [DisplayText("flow quality")]
        public byte opt_qua { get; set; }

        public float ekfstatus { get; set; }

        public int ekfflags { get; set; }

        public float ekfvelv { get; set; }

        public float ekfcompv { get; set; }

        public float ekfposhor { get; set; }

        public float ekfposvert { get; set; }

        public float ekfteralt { get; set; }

        public float pidff { get; set; }

        public float pidP { get; set; }

        public float pidI { get; set; }

        public float pidD { get; set; }

        public byte pidaxis { get; set; }

        public float piddesired { get; set; }

        public float pidachieved { get; set; }

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

        // HIL
        public int hilch1;// { get; set; }
        public int hilch2;// { get; set; }
        public int hilch3;// { get; set; }
        public int hilch4;// { get; set; }
        public int hilch5;
        public int hilch6;
        public int hilch7;
        public int hilch8;

        // rc override
        public short rcoverridech1;//{ get; set; }
        public short rcoverridech2;// { get; set; }
        public short rcoverridech3;//{ get; set; }
        public short rcoverridech4;//{ get; set; }
        public short rcoverridech5;// { get; set; }
        public short rcoverridech6;// { get; set; }
        public short rcoverridech7;// { get; set; }
        public short rcoverridech8;// { get; set; }

        public short rcoverridech9;//{ get; set; }
        public short rcoverridech10;// { get; set; }
        public short rcoverridech11;//{ get; set; }
        public short rcoverridech12;//{ get; set; }
        public short rcoverridech13;// { get; set; }
        public short rcoverridech14;// { get; set; }
        public short rcoverridech15;// { get; set; }
        public short rcoverridech16;// { get; set; }
        public short rcoverridech17;// { get; set; }
        public short rcoverridech18;// { get; set; }

        public Mavlink_Sensors sensors_enabled = new Mavlink_Sensors();
        public Mavlink_Sensors sensors_health = new Mavlink_Sensors();
        public Mavlink_Sensors sensors_present = new Mavlink_Sensors();

        bool useLocation = false;
        bool gotwind = false;
        internal bool batterymonitoring = false;

        // for calc of sitl speedup
        internal DateTime lastimutime = DateTime.MinValue;
        internal double imutime = 0;

        internal bool MONO = false;

        static CurrentState()
        {
            // set default telemrates
            rateattitudebackup = 4;
            ratepositionbackup = 2;
            ratestatusbackup = 2;
            ratesensorsbackup = 2;
            ratercbackup = 2;
        }

        public CurrentState()
        {
            ResetInternals();

            var t = Type.GetType("Mono.Runtime");
            MONO = (t != null);
        }

        public void ResetInternals()
        {
            lock (this)
            {
                mode = "Unknown";
                _mode = 99999;
                messages = new List<string>();
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
                capabilities = (uint)MAVLink.MAV_PROTOCOL_CAPABILITY.MISSION_FLOAT;
            }
        }

        public List<string> GetItemList(bool alpha = false)
        {
            List<string> ans = new List<string>();

            object thisBoxed = this;
            Type test = thisBoxed.GetType();

            // public instance props
            PropertyInfo[] props = test.GetProperties();

            //props

            foreach (var field in props)
            {
                ans.Add(field.Name);
            }

            if (alpha)
                ans.Sort();

            return ans;
        }

        private DateTime lastupdate = DateTime.Now;

        private DateTime lastsecondcounter = DateTime.Now;
        private PointLatLngAlt lastpos = new PointLatLngAlt();

        DateTime lastdata = DateTime.MinValue;

        public string GetNameandUnit(string name)
        {
            string desc = name;
            try
            {
                var typeofthing = typeof(CurrentState).GetProperty(name);
                if (typeofthing != null)
                {
                    var attrib = typeofthing.GetCustomAttributes(false);
                    if (attrib.Length > 0)
                        desc = ((Attributes.DisplayTextAttribute)attrib[0]).Text;
                }
            }
            catch
            {
            }

            if (desc.Contains("(dist)"))
            {
                desc = desc.Replace("(dist)", "(" + CurrentState.DistanceUnit + ")");
            }
            else if (desc.Contains("(speed)"))
            {
                desc = desc.Replace("(speed)", "(" + CurrentState.SpeedUnit + ")");
            }
            else if (desc.Contains("(alt)"))
            {
                desc = desc.Replace("(alt)", "(" + CurrentState.AltUnit + ")");
            }

            return desc;
        }


        /// <summary>
        /// use for main serial port only
        /// </summary>
        /// <param name="bs"></param>
        public void UpdateCurrentSettings(Action<CurrentState> bs)
        {
            UpdateCurrentSettings(bs, false, parent.parent, parent.parent.MAV);
        }

        /// <summary>
        /// Use the default sysid
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
                        {
                            linkqualitygcs = 0;
                        }
                        else
                        {
                            linkqualitygcs =
                                (ushort)((parent.packetsnotlost / (parent.packetsnotlost + parent.packetslost)) * 100.0);
                        }

                        if (linkqualitygcs > 100)
                            linkqualitygcs = 100;
                    }

                    if (datetime.Second != lastsecondcounter.Second)
                    {
                        lastsecondcounter = datetime;

                        if (lastpos.Lat != 0 && lastpos.Lng != 0 && armed)
                        {
                            if ((mavinterface.BaseStream != null && !mavinterface.BaseStream.IsOpen) && !mavinterface.logreadmode)
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
                    if (!(lastdata.AddSeconds(8) > DateTime.Now) && mavinterface.BaseStream != null && mavinterface.BaseStream.IsOpen)
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
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA3, MAV.cs.ratesensors, MAV.sysid,
                                MAV.compid);// request extra stuff - tridge
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MAV.cs.ratesensors,
                                MAV.sysid, MAV.compid); // request raw sensor
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, MAV.cs.raterc, MAV.sysid,
                                MAV.compid);// request rc info
                        }
                        catch
                        {
                            log.Error("Failed to request rates");
                        }
                        lastdata = DateTime.Now.AddSeconds(30); // prevent flooding
                    }

                    MAVLink.MAVLinkMessage mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.RC_CHANNELS_SCALED);

                    if (mavLinkMessage != null) // hil mavlink 0.9
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

                        MAV.clearPacket((uint)MAVLink.MAVLINK_MSG_ID.RC_CHANNELS_SCALED);
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.LOCAL_POSITION_NED);

                    if (mavLinkMessage != null)
                    {
                        var lpned = mavLinkMessage.ToStructure<MAVLink.mavlink_local_position_ned_t>();

                        var loc = HomeLocation.gps_offset(lpned.y, lpned.x);

                        //lat = loc.Lat;
                        //lng = loc.Lng;
                        //alt = (float)(loc.Alt + lpned.z);
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.POSITION_TARGET_GLOBAL_INT);

                    if (mavLinkMessage != null)
                    {
                        var postraget = mavLinkMessage.ToStructure<MAVLink.mavlink_position_target_global_int_t>();

                        if (postraget.coordinate_frame == (byte)MAVLink.MAV_FRAME.GLOBAL_INT)
                            TargetLocation = new PointLatLngAlt(postraget.lat_int / 1e7, postraget.lon_int / 1e7, postraget.alt,
                                postraget.type_mask.ToString());

                        if (postraget.coordinate_frame == (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT)
                            TargetLocation = new PointLatLngAlt(postraget.lat_int / 1e7, postraget.lon_int / 1e7, postraget.alt + HomeAlt,
                                postraget.type_mask.ToString());
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION);

                    if (mavLinkMessage != null)
                    {
                        var version = mavLinkMessage.ToStructure<MAVLink.mavlink_autopilot_version_t>();
                        //#define FIRMWARE_VERSION 3,4,0,FIRMWARE_VERSION_TYPE_DEV
                        //		flight_sw_version	0x03040000	uint

                        byte main = (byte)(version.flight_sw_version >> 24);
                        byte sub = (byte)((version.flight_sw_version >> 16) & 0xff);
                        byte rev = (byte)((version.flight_sw_version >> 8) & 0xff);
                        MAVLink.FIRMWARE_VERSION_TYPE type =
                            (MAVLink.FIRMWARE_VERSION_TYPE)(version.flight_sw_version & 0xff);

                        this.version = new Version(main, sub, rev, (int)type);

                        try
                        {
                            capabilities = (uint)(MAVLink.MAV_PROTOCOL_CAPABILITY)version.capabilities;
                        }
                        catch
                        {

                        }

                        Serial.print("Flight SW Version: "); Serial.println(version.flight_sw_version);
                        Serial.print("Middleware SW: "); Serial.println(version.middleware_sw_version);
                        Serial.print("OS Custom: "); Serial.println(version.os_custom_version);
                        Serial.print("OS SW: "); Serial.println(version.os_sw_version);
                        Serial.print("board_version: "); Serial.println(version.board_version);
                        Serial.print("Vendor ID: "); Serial.println(version.vendor_id);
                        Serial.print("Product ID: "); Serial.println(version.product_id);
                        Serial.print("Board Version: "); Serial.println(version.board_version);

                        MAV.clearPacket((uint)MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION);
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.FENCE_STATUS);

                    if (mavLinkMessage != null)
                    {
                        var fence = mavLinkMessage.ToStructure<MAVLink.mavlink_fence_status_t>();

                        if (fence.breach_status != (byte)MAVLink.FENCE_BREACH.NONE)
                        {
                            // fence breached
                            messageHigh = "Fence Breach";
                            messageHighTime = DateTime.Now;
                        }

                        MAV.clearPacket((uint)MAVLink.MAVLINK_MSG_ID.FENCE_STATUS);
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.HIGH_LATENCY);

                    if (mavLinkMessage != null)
                    {
                        var highlatency = mavLinkMessage.ToStructure<MAVLink.mavlink_high_latency_t>();

                        landed = highlatency.landed_state == 1;

                        if ((highlatency.base_mode & (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED) != 0)
                        {
                            List<KeyValuePair<int, string>> modelist = Common.getModesList(this.firmware);

                            if (modelist != null)
                            {
                                bool found = false;

                                foreach (KeyValuePair<int, string> pair in modelist)
                                {
                                    if (pair.Key == highlatency.custom_mode)
                                    {
                                        mode = pair.Value.ToString();
                                        _mode = highlatency.custom_mode;
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    log.Warn("Mode not found bm:" + highlatency.base_mode + " cm:" + highlatency.custom_mode);
                                }
                            }
                        }

                        roll = highlatency.roll / 100f;
                        pitch = highlatency.pitch / 100f;
                        yaw = highlatency.heading / 100f;
                        ch3percent = highlatency.throttle;
                        lat = highlatency.latitude / 1e7;
                        lng = highlatency.longitude / 1e7;
                        altasl = highlatency.altitude_amsl;
                        alt = highlatency.altitude_sp;
                        airspeed = highlatency.airspeed;
                        _targetairspeed = highlatency.airspeed_sp;
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

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.HIL_CONTROLS);

                    if (mavLinkMessage != null) // hil mavlink 0.9 and 1.0
                    {
                        var hil = mavLinkMessage.ToStructure<MAVLink.mavlink_hil_controls_t>();

                        hilch1 = (int)(hil.roll_ailerons * 10000);
                        hilch2 = (int)(hil.pitch_elevator * 10000);
                        hilch3 = (int)(hil.throttle * 10000);
                        hilch4 = (int)(hil.yaw_rudder * 10000);

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.HIL_CONTROLS);
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.OPTICAL_FLOW);

                    if (mavLinkMessage != null)
                    {
                        var optflow = mavLinkMessage.ToStructure<MAVLink.mavlink_optical_flow_t>();

                        opt_m_x = optflow.flow_comp_m_x;
                        opt_m_y = optflow.flow_comp_m_y;
                        opt_x = optflow.flow_x;
                        opt_y = optflow.flow_y;
                        opt_qua = optflow.quality;
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.MOUNT_STATUS);

                    if (mavLinkMessage != null)
                    {
                        var status = mavLinkMessage.ToStructure<MAVLink.mavlink_mount_status_t>();

                        campointa = status.pointing_a / 100.0f;
                        campointb = status.pointing_b / 100.0f;
                        campointc = status.pointing_c / 100.0f;
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.VIBRATION);

                    if (mavLinkMessage != null)
                    {
                        var vibe = mavLinkMessage.ToStructure<MAVLink.mavlink_vibration_t>();

                        vibeclip0 = vibe.clipping_0;
                        vibeclip1 = vibe.clipping_1;
                        vibeclip2 = vibe.clipping_2;
                        vibex = vibe.vibration_x;
                        vibey = vibe.vibration_y;
                        vibez = vibe.vibration_z;
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.AIRSPEED_AUTOCAL);

                    if (mavLinkMessage != null)
                    {
                        var asac = mavLinkMessage.ToStructure<MAVLink.mavlink_airspeed_autocal_t>();

                        asratio = asac.ratio;
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.SYSTEM_TIME);

                    if (mavLinkMessage != null)
                    {
                        var systime = mavLinkMessage.ToStructure<MAVLink.mavlink_system_time_t>();

                        DateTime date1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        try
                        {
                            date1 = date1.AddMilliseconds(systime.time_unix_usec / 1000);

                            gpstime = date1;
                        }
                        catch
                        {
                        }
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.HWSTATUS);

                    if (mavLinkMessage != null)
                    {
                        var hwstatus = mavLinkMessage.ToStructure<MAVLink.mavlink_hwstatus_t>();

                        hwvoltage = hwstatus.Vcc / 1000.0f;
                        i2cerrors = hwstatus.I2Cerr;

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.HWSTATUS);
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.EKF_STATUS_REPORT);
                    if (mavLinkMessage != null)
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
                            (float)
                                Math.Max(ekfvelv,
                                    Math.Max(ekfcompv, Math.Max(ekfposhor, Math.Max(ekfposvert, ekfteralt))));

                        if (ekfvelv >= 1)
                        {
                            messageHigh = Strings.ERROR + " " + Strings.velocity_variance;
                            messageHighTime = DateTime.Now;
                        }
                        if (ekfcompv >= 1)
                        {
                            messageHigh = Strings.ERROR + " " + Strings.compass_variance;
                            messageHighTime = DateTime.Now;
                        }
                        if (ekfposhor >= 1)
                        {
                            messageHigh = Strings.ERROR + " " + Strings.pos_horiz_variance;
                            messageHighTime = DateTime.Now;
                        }
                        if (ekfposvert >= 1)
                        {
                            messageHigh = Strings.ERROR + " " + Strings.pos_vert_variance;
                            messageHighTime = DateTime.Now;
                        }
                        if (ekfteralt >= 1)
                        {
                            messageHigh = Strings.ERROR + " " + Strings.terrain_alt_variance;
                            messageHighTime = DateTime.Now;
                        }

                        for (int a = 1; a <= (int)MAVLink.EKF_STATUS_FLAGS.EKF_PRED_POS_HORIZ_ABS; a = a << 1)
                        {
                            int currentbit = (ekfstatusm.flags & a);
                            if (currentbit == 0)
                            {
                                var currentflag =
                                    (MAVLink.EKF_STATUS_FLAGS)
                                        Enum.Parse(typeof(MAVLink.EKF_STATUS_FLAGS), a.ToString());

                                switch (currentflag)
                                {
                                    case MAVLink.EKF_STATUS_FLAGS.EKF_ATTITUDE: // step 1
                                    case MAVLink.EKF_STATUS_FLAGS.EKF_VELOCITY_HORIZ: // with pos
                                        if (gpsstatus > 0) // we have gps and dont have vel_hoz
                                            ekfstatus = 1;
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
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.RANGEFINDER);
                    if (mavLinkMessage != null)
                    {
                        var sonar = mavLinkMessage.ToStructure<MAVLink.mavlink_rangefinder_t>();

                        sonarrange = sonar.distance;
                        sonarvoltage = sonar.voltage;
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.DISTANCE_SENSOR);
                    if (mavLinkMessage != null)
                    {
                        var sonar = mavLinkMessage.ToStructure<MAVLink.mavlink_distance_sensor_t>();
                        if (sonar.id == 0)
                        {
                            rangefinder1 = sonar.current_distance;
                        }
                        else if (sonar.id == 1)
                        {
                            rangefinder2 = sonar.current_distance;
                        }
                        else if (sonar.id == 2)
                        {
                            rangefinder3 = sonar.current_distance;
                        }
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.POWER_STATUS);
                    if (mavLinkMessage != null)
                    {
                        var power = mavLinkMessage.ToStructure<MAVLink.mavlink_power_status_t>();

                        boardvoltage = power.Vcc;
                        servovoltage = power.Vservo;

                        try
                        {
                            voltageflag = (uint)((MAVLink.MAV_POWER_STATUS)power.flags);
                        }
                        catch
                        {
                        }
                    }


                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.WIND);
                    if (mavLinkMessage != null)
                    {
                        var wind = mavLinkMessage.ToStructure<MAVLink.mavlink_wind_t>();

                        gotwind = true;

                        wind_dir = (wind.direction + 360) % 360;
                        wind_vel = wind.speed * multiplierspeed;
                    }


                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT);
                    if (mavLinkMessage != null)
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
                                messageHighTime = DateTime.Now;
                            }

                            // for future use
                            landed = hb.system_status == (byte)MAVLink.MAV_STATE.STANDBY;

                            failsafe = hb.system_status == (byte)MAVLink.MAV_STATE.CRITICAL;

                            string oldmode = mode;

                            if ((hb.base_mode & (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED) != 0)
                            {
                                // prevent running thsi unless we have to
                                if (_mode != hb.custom_mode)
                                {
                                    List<KeyValuePair<int, string>> modelist = Common.getModesList(this.firmware);

                                    if (modelist != null)
                                    {
                                        bool found = false;

                                        foreach (KeyValuePair<int, string> pair in modelist)
                                        {
                                            if (pair.Key == hb.custom_mode)
                                            {
                                                mode = pair.Value.ToString();
                                                _mode = hb.custom_mode;
                                                found = true;
                                                break;
                                            }
                                        }

                                        if (!found)
                                        {
                                            log.Warn("Mode not found bm:" + hb.base_mode + " cm:" + hb.custom_mode);
                                        }
                                    }

                                    _mode = hb.custom_mode;
                                }
                            }

                            if (oldmode != mode && Speech != null && Speech.speechEnable &&
                                parent?.parent?.MAV?.cs == this &&
                                Settings.Instance.GetBoolean("speechmodeenabled"))
                            {
                                Speech.SpeakAsync(Common.speechConversion(parent,
                                    "" + Settings.Instance["speechmode"]));
                            }
                        }
                    }


                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.SYS_STATUS);
                    if (mavLinkMessage != null)
                    {
                        var sysstatus = mavLinkMessage.ToStructure<MAVLink.mavlink_sys_status_t>();

                        load = (float)sysstatus.load / 10.0f;

                        battery_voltage = (float)sysstatus.voltage_battery / 1000.0f;
                        battery_remaining = sysstatus.battery_remaining;
                        current = (float)sysstatus.current_battery / 100.0f;

                        packetdropremote = sysstatus.drop_rate_comm;

                        sensors_enabled.Value = sysstatus.onboard_control_sensors_enabled;
                        sensors_health.Value = sysstatus.onboard_control_sensors_health;
                        sensors_present.Value = sysstatus.onboard_control_sensors_present;

                        terrainactive = sensors_health.terrain && sensors_enabled.terrain && sensors_present.terrain;

                        if (!sensors_health.gps && sensors_enabled.gps && sensors_present.gps)
                        {
                            messageHigh = Strings.BadGPSHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.gyro && sensors_enabled.gyro && sensors_present.gyro)
                        {
                            messageHigh = Strings.BadGyroHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.accelerometer && sensors_enabled.accelerometer &&
                                 sensors_present.accelerometer)
                        {
                            messageHigh = Strings.BadAccelHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.compass && sensors_enabled.compass && sensors_present.compass)
                        {
                            messageHigh = Strings.BadCompassHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.barometer && sensors_enabled.barometer && sensors_present.barometer)
                        {
                            messageHigh = Strings.BadBaroHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.LASER_POSITION && sensors_enabled.LASER_POSITION &&
                                 sensors_present.LASER_POSITION)
                        {
                            messageHigh = Strings.BadLiDARHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.optical_flow && sensors_enabled.optical_flow &&
                                 sensors_present.optical_flow)
                        {
                            messageHigh = Strings.BadOptFlowHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.VISION_POSITION && sensors_enabled.VISION_POSITION &&
                                 sensors_present.VISION_POSITION)
                        {
                            messageHigh = Strings.Bad_Vision_Position;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.terrain && sensors_enabled.terrain && sensors_present.terrain)
                        {
                            messageHigh = Strings.BadorNoTerrainData;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.geofence && sensors_enabled.geofence &&
                                 sensors_present.geofence)
                        {
                            messageHigh = Strings.GeofenceBreach;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.ahrs && sensors_enabled.ahrs && sensors_present.ahrs)
                        {
                            messageHigh = Strings.BadAHRS;
                            messageHighTime = DateTime.Now;
                        }
                        else if (!sensors_health.rc_receiver && sensors_enabled.rc_receiver &&
                                 sensors_present.rc_receiver)
                        {
                            bool reporterror = true;
                            if (Settings.Instance["norcreceiver"] != null)
                                reporterror = !bool.Parse(Settings.Instance["norcreceiver"]);
                            if (reporterror)
                            {
                                messageHigh = Strings.NORCReceiver;
                                messageHighTime = DateTime.Now;
                            }
                        }
                        else if (!sensors_health.logging && sensors_enabled.logging && sensors_present.logging)
                        {
                            messageHigh = Strings.BadLogging;
                            messageHighTime = DateTime.Now;
                        }


                        MAV.clearPacket((uint)MAVLink.MAVLINK_MSG_ID.SYS_STATUS);
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.EXTENDED_SYS_STATE);
                    if (mavLinkMessage != null)
                    {
                        var extsysstatus = mavLinkMessage.ToStructure<MAVLink.mavlink_extended_sys_state_t>();

                        vtol_state = extsysstatus.vtol_state;
                        landed_state = extsysstatus.landed_state;
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.BATTERY2);
                    if (mavLinkMessage != null)
                    {
                        var bat = mavLinkMessage.ToStructure<MAVLink.mavlink_battery2_t>();
                        _battery_voltage2 = bat.voltage / 1000.0f;
                        current2 = bat.current_battery / 100.0f;
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.BATTERY_STATUS);
                    if (mavLinkMessage != null)
                    {
                        var bats = mavLinkMessage.ToStructure<MAVLink.mavlink_battery_status_t>();

                        if (bats.id == 0)
                        {
                            if (bats.voltages[0] != ushort.MaxValue)
                            {
                                battery_cell1 = bats.voltages[0] / 1000.0;
                                if (bats.voltages[1] != ushort.MaxValue) battery_cell2 = bats.voltages[1] / 1000.0; else battery_cell2 = 0.0;
                                if (bats.voltages[2] != ushort.MaxValue) battery_cell3 = bats.voltages[2] / 1000.0; else battery_cell3 = 0.0;
                                if (bats.voltages[3] != ushort.MaxValue) battery_cell4 = bats.voltages[3] / 1000.0; else battery_cell4 = 0.0;
                                if (bats.voltages[4] != ushort.MaxValue) battery_cell5 = bats.voltages[4] / 1000.0; else battery_cell5 = 0.0;
                                if (bats.voltages[5] != ushort.MaxValue) battery_cell6 = bats.voltages[5] / 1000.0; else battery_cell6 = 0.0;
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
                            battery_voltage3 = bats.voltages.Sum(a => a != ushort.MaxValue ? a/1000.0 : 0);
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

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.SCALED_PRESSURE);
                    if (mavLinkMessage != null)
                    {
                        var pres = mavLinkMessage.ToStructure<MAVLink.mavlink_scaled_pressure_t>();
                        press_abs = pres.press_abs;
                        press_temp = pres.temperature;
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.SCALED_PRESSURE2);
                    if (mavLinkMessage != null)
                    {
                        var pres = mavLinkMessage.ToStructure<MAVLink.mavlink_scaled_pressure2_t>();
                        press_abs2 = pres.press_abs;
                        press_temp2 = pres.temperature;
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.TERRAIN_REPORT);
                    if (mavLinkMessage != null)
                    {
                        var terrainrep = mavLinkMessage.ToStructure<MAVLink.mavlink_terrain_report_t>();
                        ter_curalt = terrainrep.current_height;
                        ter_alt = terrainrep.terrain_height;
                        ter_load = terrainrep.loaded;
                        ter_pend = terrainrep.pending;
                        ter_space = terrainrep.spacing;
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.SENSOR_OFFSETS);
                    if (mavLinkMessage != null)
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

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.ATTITUDE);

                    if (mavLinkMessage != null)
                    {
                        var att = mavLinkMessage.ToStructure<MAVLink.mavlink_attitude_t>();

                        roll = (float)(att.roll*MathHelper.rad2deg);
                        pitch = (float)(att.pitch*MathHelper.rad2deg);
                        yaw = (float)(att.yaw*MathHelper.rad2deg);

                        //Console.WriteLine(MAV.sysid + " " +roll + " " + pitch + " " + yaw);

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.ATTITUDE);
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT);
                    if (mavLinkMessage != null)
                    {
                        var loc = mavLinkMessage.ToStructure<MAVLink.mavlink_global_position_int_t>();

                        // the new arhs deadreckoning may send 0 alt and 0 long. check for and undo

                        alt = loc.relative_alt/1000.0f;

                        useLocation = true;
                        if (loc.lat == 0 && loc.lon == 0)
                        {
                            useLocation = false;
                        }
                        else
                        {
                            lat = loc.lat/10000000.0;
                            lng = loc.lon/10000000.0;

                            altasl = loc.alt/1000.0f;

                            vx = loc.vx * 0.01;
                            vy = loc.vy * 0.01;
                            vz = loc.vz * 0.01;
                        }
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT);
                    if (mavLinkMessage != null)
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
                            gpshdop = (float) Math.Round((double) gps.eph / 100.0, 2);

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

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.GPS2_RAW);
                    if (mavLinkMessage != null)
                    {
                        var gps = mavLinkMessage.ToStructure<MAVLink.mavlink_gps2_raw_t>();

                        lat2 = gps.lat*1.0e-7;
                        lng2 = gps.lon*1.0e-7;
                        altasl2 = gps.alt/1000.0f;

                        gpsstatus2 = gps.fix_type;
                        gpshdop2 = (float) Math.Round((double) gps.eph/100.0, 2);

                        satcount2 = gps.satellites_visible;

                        groundspeed2 = gps.vel*1.0e-2f;
                        groundcourse2 = gps.cog*1.0e-2f;
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.GPS_STATUS);
                    if (mavLinkMessage != null)
                    {
                        var gps = mavLinkMessage.ToStructure<MAVLink.mavlink_gps_status_t>();
                        satcount = gps.satellites_visible;
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.RADIO);
                    if (mavLinkMessage != null)
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

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.RADIO_STATUS);
                    if (mavLinkMessage != null)
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

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.MISSION_CURRENT);
                    if (mavLinkMessage != null)
                    {
                        var wpcur = mavLinkMessage.ToStructure<MAVLink.mavlink_mission_current_t>();

                        int oldwp = (int) wpno;

                        wpno = wpcur.seq;

                        if (mode.ToLower() == "auto" && wpno != 0)
                        {
                            lastautowp = (int) wpno;
                        }

                        if (oldwp != wpno && Speech != null && Speech.speechEnable && parent != null && parent.parent.MAV.cs == this &&
                            Settings.Instance.GetBoolean("speechwaypointenabled"))
                        {
                            Speech.SpeakAsync(Common.speechConversion(parent, "" + Settings.Instance["speechwaypoint"]));
                        }

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.WAYPOINT_CURRENT);
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.NAV_CONTROLLER_OUTPUT);

                    if (mavLinkMessage != null)
                    {
                        var nav = mavLinkMessage.ToStructure<MAVLink.mavlink_nav_controller_output_t>();

                        nav_roll = nav.nav_roll;
                        nav_pitch = nav.nav_pitch;
                        nav_bearing = nav.nav_bearing;
                        target_bearing = nav.target_bearing;
                        wp_dist = nav.wp_dist;
                        alt_error = nav.alt_error;
						aspd_error = nav.aspd_error/100.0f;
                        xtrack_error = nav.xtrack_error;

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.NAV_CONTROLLER_OUTPUT);
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.RPM);

                    if (mavLinkMessage != null)
                    {
                        var rpm = mavLinkMessage.ToStructure<MAVLink.mavlink_rpm_t>();

                        rpm1 = rpm.rpm1;
                        rpm2 = rpm.rpm2;

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.NAV_CONTROLLER_OUTPUT);
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.RC_CHANNELS_RAW);
                    if (mavLinkMessage != null)
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
                        rxrssi = (int) ((rcin.rssi/255.0)*100.0);

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RC_CHANNELS_RAW);
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.RC_CHANNELS);
                    if (mavLinkMessage != null)
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
                        rxrssi = (int) ((rcin.rssi/255.0)*100.0);

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RC_CHANNELS_RAW);
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.ESC_TELEMETRY_1_TO_4);
                    if (mavLinkMessage != null)
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

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.ESC_TELEMETRY_5_TO_8);
                    if (mavLinkMessage != null)
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

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.SERVO_OUTPUT_RAW);
                    if (mavLinkMessage != null)
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

                        MAV.clearPacket((uint)MAVLink.MAVLINK_MSG_ID.SERVO_OUTPUT_RAW);
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.RAW_IMU);
                    if (mavLinkMessage != null)
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

                        var timesec = imu.time_usec*1.0e-6;

                        var deltawall = (DateTime.Now - lastimutime).TotalSeconds;

                        var deltaimu = timesec - imutime;

                        //Console.WriteLine( + " " + deltawall + " " + deltaimu + " " + System.Threading.Thread.CurrentThread.Name);
                        if (deltaimu > 0 && deltaimu < 10)
                        {
                            speedup = (float) (speedup * 0.95 + (deltaimu / deltawall) * 0.05);

                            imutime = timesec;
                            lastimutime = DateTime.Now;
                        }

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RAW_IMU);
                    }

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.SCALED_IMU);
                    if (mavLinkMessage != null)
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

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.SCALED_IMU2);
                    if (mavLinkMessage != null)
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


                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.SCALED_IMU3);
                    if (mavLinkMessage != null)
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

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.PID_TUNING);
                    if (mavLinkMessage != null)
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

                    mavLinkMessage = MAV.getPacket((uint) MAVLink.MAVLINK_MSG_ID.VFR_HUD);
                    if (mavLinkMessage != null)
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

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.MEMINFO);
                    if (mavLinkMessage != null)
                    {
                        var mem = mavLinkMessage.ToStructure<MAVLink.mavlink_meminfo_t>();
                        freemem = mem.freemem;
                        brklevel = mem.brkval;
                    }

                    mavLinkMessage = MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.AOA_SSA);
                    if (mavLinkMessage != null)
                    {
                        var aoa_ssa = mavLinkMessage.ToStructure<MAVLink.mavlink_aoa_ssa_t>();

                        AOA = aoa_ssa.AOA;
                        SSA = aoa_ssa.SSA;
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
                    if (bs != null)
                    {
                        bs?.Invoke(this);
                        //bs.DataSource = this;
                        //bs.ResetBindings(false);

                        return;
                        /*

                        if (bs.Count > 200)
                        {
                            while (bs.Count > 3)
                                bs.RemoveAt(1);
                            //bs.Clear();
                        }
                        bs.Add(this);
                        /*
                        return;

                        bs.DataSource = this;
                        bs.ResetBindings(false);

                        return;

                        hires.Stopwatch sw = new hires.Stopwatch();

                        sw.Start();
                        bs.DataSource = this;
                        bs.ResetBindings(false);
                        sw.Stop();
                        var elaps = sw.Elapsed;
                        Console.WriteLine("1 " + elaps.ToString("0.#####") + " done ");

                        sw.Start();
                        bs.SuspendBinding();
                        bs.Clear();
                        bs.ResumeBinding();
                        bs.Add(this);
                        sw.Stop();
                        elaps = sw.Elapsed;
                        Console.WriteLine("2 " + elaps.ToString("0.#####") + " done ");
                     
                        sw.Start();
                        if (bs.Count > 100)
                            bs.Clear();
                        bs.Add(this);
                        sw.Stop();
                        elaps = sw.Elapsed;
                        Console.WriteLine("3 " + elaps.ToString("0.#####") + " done ");
                        */
                    }
                }
                catch
                {
                    log.InfoFormat("CurrentState Binding error");
                }
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void dowindcalc()
        {
            //Wind Fixed gain Observer
            //Ryan Beall 
            //8FEB10

            double Kw = 0.010; // 0.01 // 0.10

            if (airspeed < 1 || _groundspeed < 1)
                return;

            double Wn_error = airspeed*Math.Cos((yaw)*MathHelper.deg2rad)*Math.Cos(pitch*MathHelper.deg2rad) -
                              _groundspeed*Math.Cos((groundcourse)*MathHelper.deg2rad) - Wn_fgo;
            double We_error = airspeed*Math.Sin((yaw)*MathHelper.deg2rad)*Math.Cos(pitch*MathHelper.deg2rad) -
                              _groundspeed*Math.Sin((groundcourse)*MathHelper.deg2rad) - We_fgo;

            Wn_fgo = Wn_fgo + Kw*Wn_error;
            We_fgo = We_fgo + Kw*We_error;

            double wind_dir = (Math.Atan2(We_fgo, Wn_fgo)*(180/Math.PI));
            double wind_vel = (Math.Sqrt(Math.Pow(We_fgo, 2) + Math.Pow(Wn_fgo, 2)));

            wind_dir = (wind_dir + 360)%360;

            this.wind_dir = (float) wind_dir; // (float)(wind_dir * 0.5 + this.wind_dir * 0.5);
            this.wind_vel = (float) wind_vel; // (float)(wind_vel * 0.5 + this.wind_vel * 0.5);

            //Console.WriteLine("Wn_error = {0}\nWe_error = {1}\nWn_fgo =    {2}\nWe_fgo =  {3}\nWind_dir =    {4}\nWind_vel =    {5}\n",Wn_error,We_error,Wn_fgo,We_fgo,wind_dir,wind_vel);

            //Console.WriteLine("wind_dir: {0} wind_vel: {1}    as {4} yaw {5} pitch {6} gs {7} cog {8}", wind_dir, wind_vel, Wn_fgo, We_fgo , airspeed,yaw,pitch,groundspeed,groundcourse);

            //low pass the outputs for better results!
        }

        /// <summary>
        /// derived from MAV_SYS_STATUS_SENSOR
        /// </summary>
        public class Mavlink_Sensors
        {
            BitArray bitArray = new BitArray(32);

            public bool seen = false;

            public Mavlink_Sensors()
            {
                //var item = MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL;
            }

            public Mavlink_Sensors(uint p)
            {
                seen = true;
                bitArray = new BitArray(new int[] {(int) p});
            }

            public bool gyro
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_GYRO)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_GYRO)] = value; }
            }

            public bool accelerometer
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL)] = value; }
            }

            public bool compass
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_MAG)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_MAG)] = value; }
            }

            public bool barometer
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ABSOLUTE_PRESSURE)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ABSOLUTE_PRESSURE)] = value; }
            }

            public bool differential_pressure
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.DIFFERENTIAL_PRESSURE)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.DIFFERENTIAL_PRESSURE)] = value; }
            }

            public bool gps
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.GPS)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.GPS)] = value; }
            }

            public bool optical_flow
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.OPTICAL_FLOW)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.OPTICAL_FLOW)] = value; }
            }

            public bool VISION_POSITION
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.VISION_POSITION)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.VISION_POSITION)] = value; }
            }

            public bool LASER_POSITION
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.LASER_POSITION)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.LASER_POSITION)] = value; }
            }

            public bool GROUND_TRUTH
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.EXTERNAL_GROUND_TRUTH)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.EXTERNAL_GROUND_TRUTH)] = value; }
            }

            public bool rate_control
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ANGULAR_RATE_CONTROL)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ANGULAR_RATE_CONTROL)] = value; }
            }

            public bool attitude_stabilization
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ATTITUDE_STABILIZATION)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.ATTITUDE_STABILIZATION)] = value; }
            }

            public bool yaw_position
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.YAW_POSITION)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.YAW_POSITION)] = value; }
            }

            public bool altitude_control
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.Z_ALTITUDE_CONTROL)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.Z_ALTITUDE_CONTROL)] = value; }
            }

            public bool xy_position_control
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.XY_POSITION_CONTROL)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.XY_POSITION_CONTROL)] = value; }
            }

            public bool motor_control
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MOTOR_OUTPUTS)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MOTOR_OUTPUTS)] = value; }
            }

            public bool rc_receiver
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.RC_RECEIVER)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.RC_RECEIVER)] = value; }
            }

            public bool gyro2
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_GYRO2)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_GYRO2)] = value; }
            }

            public bool accel2
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL2)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL2)] = value; }
            }

            public bool mag2
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_MAG2)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR._3D_MAG2)] = value; }
            }

            public bool geofence
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_GEOFENCE)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_GEOFENCE)] = value; }
            }

            public bool ahrs
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_AHRS)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_AHRS)] = value; }
            }

            public bool terrain
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_TERRAIN)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_TERRAIN)] = value; }
            }

            public bool revthrottle
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_REVERSE_MOTOR)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_REVERSE_MOTOR)] = value; }
            }

            public bool logging
            {
                get { return bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_LOGGING)]; }
                set { bitArray[ConvertValuetoBitmaskOffset((int)MAVLink.MAV_SYS_STATUS_SENSOR.MAV_SYS_STATUS_LOGGING)] = value; }
            }

            int ConvertValuetoBitmaskOffset(int input)
            {
                int offset = 0;
                for (int a = 0; a < sizeof (int)*8; a++)
                {
                    offset = 1 << a;
                    if (input == offset)
                        return a;
                }
                return 0;
            }

            public uint Value
            {
                get
                {
                    int[] array = new int[1];
                    bitArray.CopyTo(array, 0);
                    return (uint)array[0];
                }
                set
                {
                    seen = true;
                    bitArray = new BitArray(new int[] { (int)value });
                }
            }

            public override string ToString()
            {
                return Convert.ToString(Value,2);
            }
        }
    }
}