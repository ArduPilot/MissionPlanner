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

namespace MissionPlanner
{
    public class CurrentState : ICloneable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler csCallBack;

        internal MAVState parent;

        internal int lastautowp = -1;

        // multipliers
        public static float multiplierdist = 1;
        public static string DistanceUnit = "";
        public static float multiplierspeed = 1;
        public static string SpeedUnit = "";

        public static double toDistDisplayUnit(double input)
        {
            return input*multiplierdist;
        }

        public static double toSpeedDisplayUnit(double input)
        {
            return input*multiplierspeed;
        }

        public static double fromDistDisplayUnit(double input)
        {
            return input/multiplierdist;
        }

        public static double fromSpeedDisplayUnit(double input)
        {
            return input/multiplierspeed;
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

        [DisplayText("Altitude (dist)")]
        public float alt
        {
            get { return (_alt - altoffsethome)*multiplierdist; }
            set
            {
                // check update rate, and ensure time hasnt gone backwards                
                _alt = value;

                if ((datetime - lastalt).TotalSeconds >= 0.2 && oldalt != alt || lastalt > datetime)
                {
                    climbrate = (alt - oldalt)/(float) (datetime - lastalt).TotalSeconds;
                    verticalspeed = (alt - oldalt)/(float) (datetime - lastalt).TotalSeconds;
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
            get { return _altasl*multiplierdist; }
            set { _altasl = value; }
        }

        float _altasl = 0;
        float oldalt = 0;

        [DisplayText("Alt Home Offset (dist)")]
        public float altoffsethome { get; set; }

        private float _alt = 0;

        [DisplayText("Gps Status")]
        public float gpsstatus { get; set; }

        [DisplayText("Gps HDOP")]
        public float gpshdop { get; set; }

        [DisplayText("Sat Count")]
        public float satcount { get; set; }

        public double lat2 { get; set; }

        public double lng2 { get; set; }

        public float altasl2 { get; set; }

        public float gpsstatus2 { get; set; }

        public float gpshdop2 { get; set; }

        public float satcount2 { get; set; }

        public float groundspeed2 { get; set; }

        public float groundcourse2 { get; set; }

        public float altd1000
        {
            get { return (alt/1000)%10; }
        }

        public float altd100
        {
            get { return (alt/100)%10; }
        }

        // speeds
        [DisplayText("AirSpeed (speed)")]
        public float airspeed
        {
            get { return _airspeed*multiplierspeed; }
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
            get { return _groundspeed*multiplierspeed; }
            set { _groundspeed = value; }
        }

        // accel
        [DisplayText("Accel X")]
        public float ax { get; set; }

        [DisplayText("Accel Y")]
        public float ay { get; set; }

        [DisplayText("Accel Z")]
        public float az { get; set; }

        // gyro
        [DisplayText("Gyro X")]
        public float gx { get; set; }

        [DisplayText("Gyro Y")]
        public float gy { get; set; }

        [DisplayText("Gyro Z")]
        public float gz { get; set; }

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
            get { return (float) Math.Sqrt(Math.Pow(mx, 2) + Math.Pow(my, 2) + Math.Pow(mz, 2)); }
        }

        [DisplayText("Accel Strength")]
        public float accelsq
        {
            get { return (float) Math.Sqrt(Math.Pow(ax, 2) + Math.Pow(ay, 2) + Math.Pow(az, 2))/1000.0f /*980.665f*/; }
        }

        [DisplayText("Gyro Strength")]
        public float gyrosq
        {
            get { return (float) Math.Sqrt(Math.Pow(gx, 2) + Math.Pow(gy, 2) + Math.Pow(gz, 2)); }
        }

        // accel2
        [DisplayText("Accel2 X")]
        public float ax2 { get; set; }

        [DisplayText("Accel2 Y")]
        public float ay2 { get; set; }

        [DisplayText("Accel2 Z")]
        public float az2 { get; set; }

        // gyro2
        [DisplayText("Gyro2 X")]
        public float gx2 { get; set; }

        [DisplayText("Gyro2 Y")]
        public float gy2 { get; set; }

        [DisplayText("Gyro2 Z")]
        public float gz2 { get; set; }

        // mag2
        [DisplayText("Mag2 X")]
        public float mx2 { get; set; }

        [DisplayText("Mag2 Y")]
        public float my2 { get; set; }

        [DisplayText("Mag2 Z")]
        public float mz2 { get; set; }

        // accel3
        [DisplayText("Accel3 X")]
        public float ax3 { get; set; }

        [DisplayText("Accel3 Y")]
        public float ay3 { get; set; }

        [DisplayText("Accel3 Z")]
        public float az3 { get; set; }

        // gyro3
        [DisplayText("Gyro3 X")]
        public float gx3 { get; set; }

        [DisplayText("Gyro3 Y")]
        public float gy3 { get; set; }

        [DisplayText("Gyro3 Z")]
        public float gz3 { get; set; }

        // mag3
        [DisplayText("Mag3 X")]
        public float mx3 { get; set; }

        [DisplayText("Mag3 Y")]
        public float my3 { get; set; }

        [DisplayText("Mag3 Z")]
        public float mz3 { get; set; }

        [DisplayText("Failsafe")]
        public bool failsafe { get; set; }

        [DisplayText("RX Rssi")]
        public int rxrssi { get; set; }

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

        public float ch3percent
        {
            get
            {
                if (_ch3percent != -1)
                    return _ch3percent;
                try
                {
                    if (MainV2.comPort.MAV.param.ContainsKey("RC3_MIN") &&
                        MainV2.comPort.MAV.param.ContainsKey("RC3_MAX"))
                    {
                        return
                            (int)
                                (((ch3out - MainV2.comPort.MAV.param["RC3_MIN"].Value)/
                                  (MainV2.comPort.MAV.param["RC3_MAX"].Value - MainV2.comPort.MAV.param["RC3_MIN"].Value))*
                                 100);
                    }
                    else
                    {
                        if (ch3out == 0)
                            return 0;
                        return (int) ((ch3out - 1100)/(1900 - 1100)*100);
                    }
                }
                catch
                {
                    return 0;
                }
            }

            set { _ch3percent = value; }
        }

        float _ch3percent = -1;

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
                return _verticalspeed*multiplierspeed;
            }
            set { _verticalspeed = _verticalspeed*0.4f + value*0.6f; }
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
            get { return (_wpdist*multiplierdist); }
            set { _wpdist = value; }
        }

        [DisplayText("Altitude Error (dist)")]
        public float alt_error
        {
            get { return _alt_error*multiplierdist; }
            set
            {
                if (_alt_error == value) return;
                _alt_error = value;
                _targetalt = _targetalt*0.5f + (float) Math.Round(alt + alt_error, 0)*0.5f;
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
            get { return _aspd_error*multiplierspeed; }
            set
            {
                if (_aspd_error == value) return;
                _aspd_error = value;
                _targetairspeed = _targetairspeed*0.5f + (float) Math.Round(airspeed + aspd_error, 0)*0.5f;
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
            get { return _climbrate*multiplierspeed; }
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
                if (groundspeed <= 0) return 0;
                return (int) (wp_dist/groundspeed);
            }
        }

        [DisplayText("Time over Home (sec)")]
        public int toh
        {
            get
            {
                if (groundspeed <= 0) return 0;
                return (int) (DistToHome/groundspeed);
            }
        }

        [DisplayText("Dist Traveled (dist)")]
        public float distTraveled { get; set; }

        [DisplayText("Time in Air (sec)")]
        public float timeInAir { get; set; }

        // calced turn rate
        [DisplayText("Turn Rate (speed)")]
        public float turnrate
        {
            get
            {
                if (groundspeed <= 1) return 0;
                return (roll*9.8f)/groundspeed;
            }
        }

        // turn radius
        [DisplayText("Turn Radius (dist)")]
        public float radius
        {
            get
            {
                if (groundspeed <= 1) return 0;
                return ((groundspeed*groundspeed)/(float) (9.8f*Math.Tan(roll*deg2rad)));
            }
        }

        float _wpdist;
        float _aspd_error;
        float _alt_error;
        float _targetalt;
        float _targetairspeed;
        float _climbrate;

        public float targetaltd100
        {
            get { return (_targetalt/100)%10; }
        }

        public float targetalt
        {
            get { return _targetalt; }
        }

        //airspeed_error = (airspeed_error - airspeed);

        //message
        public List<string> messages { get; set; }

        internal string message
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

        private string _messagehigh;
        public DateTime messageHighTime { get; set; }

        //battery
        [DisplayText("Bat Voltage (V)")]
        public float battery_voltage
        {
            get { return _battery_voltage; }
            set
            {
                if (_battery_voltage == 0) _battery_voltage = value;
                _battery_voltage = value*0.2f + _battery_voltage*0.8f;
            }
        }

        internal float _battery_voltage;

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

        [DisplayText("Bat Current (Amps)")]
        public float current
        {
            get { return _current; }
            set
            {
                if (_lastcurrent == DateTime.MinValue) _lastcurrent = datetime;
                battery_usedmah += (float) ((value*1000.0)*(datetime - _lastcurrent).TotalHours);
                _current = value;
                _lastcurrent = datetime;
            }
        } //current may to be below zero - recuperation in arduplane
        private float _current;

        [DisplayText("Bat Watts")]
        public float watts
        {
            get { return battery_voltage*current; }
        }

        private DateTime _lastcurrent = DateTime.MinValue;

        [DisplayText("Bat used EST (mah)")]
        public float battery_usedmah { get; set; }

        [DisplayText("Bat2 Voltage (V)")]
        public float battery_voltage2
        {
            get { return _battery_voltage2; }
            set
            {
                if (_battery_voltage2 == 0) _battery_voltage2 = value;
                _battery_voltage2 = value*0.2f + _battery_voltage2*0.8f;
            }
        }

        internal float _battery_voltage2;

        [DisplayText("Bat2 Current (Amps)")]
        public float current2
        {
            get { return _current2; }
            set
            {
                if (value < 0) return;
                _current2 = value;
            }
        }

        private float _current2;

        public float HomeAlt
        {
            get { return (float) HomeLocation.Alt; }
            set { }
        }

        static PointLatLngAlt _homelocation = new PointLatLngAlt();

        public PointLatLngAlt HomeLocation
        {
            get { return _homelocation; }
            set { _homelocation = value; }
        }

        public PointLatLngAlt MovingBase = null;

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

        [DisplayText("Distance to Home (dist)")]
        public float DistToHome
        {
            get
            {
                if (lat == 0 && lng == 0 || TrackerLocation.Lat == 0)
                    return 0;

                // shrinking factor for longitude going to poles direction
                double rads = Math.Abs(TrackerLocation.Lat)*0.0174532925;
                double scaleLongDown = Math.Cos(rads);
                double scaleLongUp = 1.0f/Math.Cos(rads);

                //DST to Home
                double dstlat = Math.Abs(TrackerLocation.Lat - lat)*111319.5;
                double dstlon = Math.Abs(TrackerLocation.Lng - lng)*111319.5*scaleLongDown;
                return (float) Math.Sqrt((dstlat*dstlat) + (dstlon*dstlon))*multiplierdist;
            }
        }

        [DisplayText("Distance From Moving Base (dist)")]
        public float DistFromMovingBase
        {
            get
            {
                if (lat == 0 && lng == 0 || MovingBase == null)
                    return 0;

                // shrinking factor for longitude going to poles direction
                double rads = Math.Abs(MovingBase.Lat)*0.0174532925;
                double scaleLongDown = Math.Cos(rads);
                double scaleLongUp = 1.0f/Math.Cos(rads);

                //DST to Home
                double dstlat = Math.Abs(MovingBase.Lat - lat)*111319.5;
                double dstlon = Math.Abs(MovingBase.Lng - lng)*111319.5*scaleLongDown;
                return (float) Math.Sqrt((dstlat*dstlat) + (dstlon*dstlon))*multiplierdist;
            }
        }

        [DisplayText("Elevation to Mav (deg)")]
        public float ELToMAV
        {
            get
            {
                float dist = DistToHome/multiplierdist;

                if (dist < 5)
                    return 0;

                float altdiff = (float) (_altasl - TrackerLocation.Alt);

                float angle = (float) Math.Atan(altdiff/dist)*rad2deg;

                return angle;
            }
        }

        [DisplayText("Bearing to Mav (deg)")]
        public float AZToMAV
        {
            get
            {
                // shrinking factor for longitude going to poles direction
                double rads = Math.Abs(TrackerLocation.Lat)*0.0174532925;
                double scaleLongDown = Math.Cos(rads);
                double scaleLongUp = 1.0f/Math.Cos(rads);

                //DIR to Home
                double dstlon = (TrackerLocation.Lng - lng); //OffSet_X
                double dstlat = (TrackerLocation.Lat - lat)*scaleLongUp; //OffSet Y
                double bearing = 90 + (Math.Atan2(dstlat, -dstlon)*57.295775); //absolut home direction
                if (bearing < 0) bearing += 360; //normalization
                //bearing = bearing - 180;//absolut return direction
                //if (bearing < 0) bearing += 360;//normalization

                float dist = DistToHome/multiplierdist;

                if (dist < 5)
                    return 0;

                return (float) bearing;
            }
        }


        // pressure
        public float press_abs { get; set; }
        public int press_temp { get; set; }

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

        [DisplayText("Sonar Range (meters)")]
        public float sonarrange
        {
            get { return (float) toDistDisplayUnit(_sonarrange); }
            set { _sonarrange = value; }
        }

        float _sonarrange = 0;

        [DisplayText("Sonar Voltage (Volt)")]
        public float sonarvoltage { get; set; }

        // current firmware
        public MainV2.Firmwares firmware = MainV2.Firmwares.ArduCopter2;
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
                _localsnrdb = ((rssi - noise)/1.9f)*0.5f + _localsnrdb*0.5f;
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
                _remotesnrdb = ((remrssi - remnoise)/1.9f)*0.5f + _remotesnrdb*0.5f;
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
                    float dist = DistToHome/multiplierdist;

                    work = dist*(float) Math.Pow(2.0, work/6.0);
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
        public MAVLink.MAV_POWER_STATUS voltageflag { get; set; }

        public ushort i2cerrors { get; set; }

        // requested stream rates
        public byte rateattitude { get; set; }
        public byte rateposition { get; set; }
        public byte ratestatus { get; set; }
        public byte ratesensors { get; set; }
        public byte raterc { get; set; }

        internal static byte rateattitudebackup { get; set; }
        internal static byte ratepositionbackup { get; set; }
        internal static byte ratestatusbackup { get; set; }
        internal static byte ratesensorsbackup { get; set; }
        internal static byte ratercbackup { get; set; }

        // reference
        public DateTime datetime { get; set; }
        public DateTime gpstime { get; set; }

        // HIL
        public int hilch1 { get; set; }
        public int hilch2 { get; set; }
        public int hilch3 { get; set; }
        public int hilch4 { get; set; }
        public int hilch5;
        public int hilch6;
        public int hilch7;
        public int hilch8;

        // rc override
        public ushort rcoverridech1 { get; set; }
        public ushort rcoverridech2 { get; set; }
        public ushort rcoverridech3 { get; set; }
        public ushort rcoverridech4 { get; set; }
        public ushort rcoverridech5 { get; set; }
        public ushort rcoverridech6 { get; set; }
        public ushort rcoverridech7 { get; set; }
        public ushort rcoverridech8 { get; set; }

        public bool connected
        {
            get { return (MainV2.comPort.BaseStream.IsOpen || MainV2.comPort.logreadmode); }
        }

        bool useLocation = false;
        bool gotwind = false;
        internal bool batterymonitoring = false;

        // for calc of sitl speedup
        internal DateTime lastimutime = DateTime.MinValue;
        internal double imutime = 0;

        public float speedup { get; set; }

        Mavlink_Sensors sensors_enabled = new Mavlink_Sensors();
        Mavlink_Sensors sensors_health = new Mavlink_Sensors();
        Mavlink_Sensors sensors_present = new Mavlink_Sensors();

        internal bool MONO = false;

        static CurrentState()
        {
            // set default telemrates
            rateattitudebackup = 6;
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
                voltageflag = MAVLink.MAV_POWER_STATUS.USB_CONNECTED;
            }
        }

        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        private DateTime lastupdate = DateTime.Now;

        private DateTime lastsecondcounter = DateTime.Now;
        private PointLatLngAlt lastpos = new PointLatLngAlt();

        DateTime lastdata = DateTime.MinValue;

        public string GetNameandUnit(string name)
        {
            string desc = name;
            try
            {
                var typeofthing = typeof (CurrentState).GetProperty(name);
                if (typeofthing != null)
                {
                    var attrib = typeofthing.GetCustomAttributes(false);
                    if (attrib.Length > 0)
                        desc = ((Attributes.DisplayTextAttribute) attrib[0]).Text;
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

            return desc;
        }

        /// <summary>
        /// use for main serial port only
        /// </summary>
        /// <param name="bs"></param>
        public void UpdateCurrentSettings(System.Windows.Forms.BindingSource bs)
        {
            UpdateCurrentSettings(bs, false, MainV2.comPort, MainV2.comPort.MAV);
        }

        /// <summary>
        /// Use the default sysid
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="updatenow"></param>
        /// <param name="mavinterface"></param>
        public void UpdateCurrentSettings(System.Windows.Forms.BindingSource bs, bool updatenow,
            MAVLinkInterface mavinterface)
        {
            UpdateCurrentSettings(bs, updatenow, mavinterface, mavinterface.MAV);
        }

        public void UpdateCurrentSettings(System.Windows.Forms.BindingSource bs, bool updatenow,
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
                                (ushort) ((parent.packetsnotlost/(parent.packetsnotlost + parent.packetslost))*100.0);
                        }

                        if (linkqualitygcs > 100)
                            linkqualitygcs = 100;
                    }

                    if (datetime.Second != lastsecondcounter.Second)
                    {
                        lastsecondcounter = datetime;

                        if (lastpos.Lat != 0 && lastpos.Lng != 0 && armed)
                        {
                            if (!mavinterface.BaseStream.IsOpen && !mavinterface.logreadmode)
                                distTraveled = 0;

                            distTraveled += (float) lastpos.GetDistance(new PointLatLngAlt(lat, lng, 0, ""))*
                                            multiplierdist;
                            lastpos = new PointLatLngAlt(lat, lng, 0, "");
                        }
                        else
                        {
                            lastpos = new PointLatLngAlt(lat, lng, 0, "");
                        }

                        // throttle is up, or groundspeed is > 3 m/s
                        if (ch3percent > 12 || _groundspeed > 3.0)
                            timeInAir++;

                        if (!gotwind)
                            dowindcalc();
                    }

                    // re-request streams
                    if (!(lastdata.AddSeconds(8) > DateTime.Now) && mavinterface.BaseStream.IsOpen)
                    {
                        try
                        {
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTENDED_STATUS, MAV.cs.ratestatus,
                                MAV.sysid); // mode
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, MAV.cs.rateposition,
                                MAV.sysid); // request gps
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA1, MAV.cs.rateattitude,
                                MAV.sysid); // request attitude
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA2, MAV.cs.rateattitude,
                                MAV.sysid); // request vfr
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA3, MAV.cs.ratesensors, MAV.sysid);
                                // request extra stuff - tridge
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MAV.cs.ratesensors,
                                MAV.sysid); // request raw sensor
                            mavinterface.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, MAV.cs.raterc, MAV.sysid);
                                // request rc info
                        }
                        catch
                        {
                            log.Error("Failed to request rates");
                        }
                        lastdata = DateTime.Now.AddSeconds(30); // prevent flooding
                    }

                    byte[] bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.RC_CHANNELS_SCALED];

                    if (bytearray != null) // hil mavlink 0.9
                    {
                        var hil = bytearray.ByteArrayToStructure<MAVLink.mavlink_rc_channels_scaled_t>(6);

                        hilch1 = hil.chan1_scaled;
                        hilch2 = hil.chan2_scaled;
                        hilch3 = hil.chan3_scaled;
                        hilch4 = hil.chan4_scaled;
                        hilch5 = hil.chan5_scaled;
                        hilch6 = hil.chan6_scaled;
                        hilch7 = hil.chan7_scaled;
                        hilch8 = hil.chan8_scaled;

                        // Console.WriteLine("RC_CHANNELS_SCALED Packet");

                        MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.RC_CHANNELS_SCALED] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION];

                    if (bytearray != null)
                    {
                        var version = bytearray.ByteArrayToStructure<MAVLink.mavlink_autopilot_version_t>(6);
                        //#define FIRMWARE_VERSION 3,4,0,FIRMWARE_VERSION_TYPE_DEV

                        //		flight_sw_version	0x03040000	uint

                        byte main = (byte) (version.flight_sw_version >> 24);
                        byte sub = (byte) ((version.flight_sw_version >> 16) & 0xff);
                        byte rev = (byte) ((version.flight_sw_version >> 8) & 0xff);
                        MAVLink.FIRMWARE_VERSION_TYPE type =
                            (MAVLink.FIRMWARE_VERSION_TYPE) (version.flight_sw_version & 0xff);

                        this.version = new Version(main, sub, (int) type, rev);

                        MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.FENCE_STATUS];

                    if (bytearray != null)
                    {
                        var fence = bytearray.ByteArrayToStructure<MAVLink.mavlink_fence_status_t>(6);

                        if (fence.breach_status != (byte) MAVLink.FENCE_BREACH.NONE)
                        {
                            // fence breached
                            messageHigh = "Fence Breach";
                            messageHighTime = DateTime.Now;
                        }

                        MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.FENCE_STATUS] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.HIL_CONTROLS];

                    if (bytearray != null) // hil mavlink 0.9 and 1.0
                    {
                        var hil = bytearray.ByteArrayToStructure<MAVLink.mavlink_hil_controls_t>(6);

                        hilch1 = (int) (hil.roll_ailerons*10000);
                        hilch2 = (int) (hil.pitch_elevator*10000);
                        hilch3 = (int) (hil.throttle*10000);
                        hilch4 = (int) (hil.yaw_rudder*10000);

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.HIL_CONTROLS] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.OPTICAL_FLOW];

                    if (bytearray != null)
                    {
                        var optflow = bytearray.ByteArrayToStructure<MAVLink.mavlink_optical_flow_t>(6);

                        opt_m_x = optflow.flow_comp_m_x;
                        opt_m_y = optflow.flow_comp_m_y;
                        opt_x = optflow.flow_x;
                        opt_y = optflow.flow_y;
                        opt_qua = optflow.quality;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.MOUNT_STATUS];

                    if (bytearray != null)
                    {
                        var status = bytearray.ByteArrayToStructure<MAVLink.mavlink_mount_status_t>(6);

                        campointa = status.pointing_a/100.0f;
                        campointb = status.pointing_b/100.0f;
                        campointc = status.pointing_c/100.0f;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.VIBRATION];

                    if (bytearray != null)
                    {
                        var vibe = bytearray.ByteArrayToStructure<MAVLink.mavlink_vibration_t>(6);

                        vibeclip0 = vibe.clipping_0;
                        vibeclip1 = vibe.clipping_1;
                        vibeclip2 = vibe.clipping_2;
                        vibex = vibe.vibration_x;
                        vibey = vibe.vibration_y;
                        vibez = vibe.vibration_z;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.AIRSPEED_AUTOCAL];

                    if (bytearray != null)
                    {
                        var asac = bytearray.ByteArrayToStructure<MAVLink.mavlink_airspeed_autocal_t>(6);

                        asratio = asac.ratio;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SYSTEM_TIME];

                    if (bytearray != null)
                    {
                        var systime = bytearray.ByteArrayToStructure<MAVLink.mavlink_system_time_t>(6);

                        DateTime date1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        try
                        {
                            date1 = date1.AddMilliseconds(systime.time_unix_usec/1000);

                            gpstime = date1;
                        }
                        catch
                        {
                        }
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.HWSTATUS];

                    if (bytearray != null)
                    {
                        var hwstatus = bytearray.ByteArrayToStructure<MAVLink.mavlink_hwstatus_t>(6);

                        hwvoltage = hwstatus.Vcc/1000.0f;
                        i2cerrors = hwstatus.I2Cerr;

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.HWSTATUS] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.EKF_STATUS_REPORT];
                    if (bytearray != null)
                    {
                        var ekfstatusm = bytearray.ByteArrayToStructure<MAVLink.mavlink_ekf_status_report_t>(6);

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

                        for (int a = 1; a < (int) MAVLink.EKF_STATUS_FLAGS.ENUM_END; a = a << 1)
                        {
                            int currentbit = (ekfstatusm.flags & a);
                            if (currentbit == 0)
                            {
                                var currentflag =
                                    (MAVLink.EKF_STATUS_FLAGS)
                                        Enum.Parse(typeof (MAVLink.EKF_STATUS_FLAGS), a.ToString());

                                switch (currentflag)
                                {
                                    case MAVLink.EKF_STATUS_FLAGS.EKF_ATTITUDE: // step 1
                                    case MAVLink.EKF_STATUS_FLAGS.EKF_VELOCITY_HORIZ: // with pos
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

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.RANGEFINDER];
                    if (bytearray != null)
                    {
                        var sonar = bytearray.ByteArrayToStructure<MAVLink.mavlink_rangefinder_t>(6);

                        sonarrange = sonar.distance;
                        sonarvoltage = sonar.voltage;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.POWER_STATUS];
                    if (bytearray != null)
                    {
                        var power = bytearray.ByteArrayToStructure<MAVLink.mavlink_power_status_t>(6);

                        boardvoltage = power.Vcc;
                        servovoltage = power.Vservo;

                        voltageflag = (MAVLink.MAV_POWER_STATUS) power.flags;
                    }


                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.WIND];
                    if (bytearray != null)
                    {
                        var wind = bytearray.ByteArrayToStructure<MAVLink.mavlink_wind_t>(6);

                        gotwind = true;

                        wind_dir = (wind.direction + 360)%360;
                        wind_vel = wind.speed*multiplierspeed;
                    }


                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.HEARTBEAT];
                    if (bytearray != null)
                    {
                        var hb = bytearray.ByteArrayToStructure<MAVLink.mavlink_heartbeat_t>(6);

                        if (hb.type == (byte) MAVLink.MAV_TYPE.GCS)
                        {
                            // skip gcs hb's
                            // only happens on log playback - and shouldnt get them here
                        }
                        else
                        {
                            armed = (hb.base_mode & (byte) MAVLink.MAV_MODE_FLAG.SAFETY_ARMED) ==
                                    (byte) MAVLink.MAV_MODE_FLAG.SAFETY_ARMED;

                            // for future use
                            landed = hb.system_status == (byte) MAVLink.MAV_STATE.STANDBY;

                            failsafe = hb.system_status == (byte) MAVLink.MAV_STATE.CRITICAL;

                            string oldmode = mode;

                            if ((hb.base_mode & (byte) MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED) != 0)
                            {
                                // prevent running thsi unless we have to
                                if (_mode != hb.custom_mode)
                                {
                                    List<KeyValuePair<int, string>> modelist = Common.getModesList(this);

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
                                        _mode = hb.custom_mode;
                                    }
                                }
                            }

                            if (oldmode != mode && MainV2.speechEnable && MainV2.comPort.MAV.cs == this &&
                                Settings.Instance.GetBoolean("speechmodeenabled"))
                            {
                                MainV2.speechEngine.SpeakAsync(Common.speechConversion(""+ Settings.Instance["speechmode"]));
                            }
                        }
                    }


                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SYS_STATUS];
                    if (bytearray != null)
                    {
                        var sysstatus = bytearray.ByteArrayToStructure<MAVLink.mavlink_sys_status_t>(6);

                        load = (float) sysstatus.load/10.0f;

                        battery_voltage = (float) sysstatus.voltage_battery/1000.0f;
                        battery_remaining = sysstatus.battery_remaining;
                        current = (float) sysstatus.current_battery/100.0f;

                        packetdropremote = sysstatus.drop_rate_comm;

                        sensors_enabled.Value = sysstatus.onboard_control_sensors_enabled;
                        sensors_health.Value = sysstatus.onboard_control_sensors_health;
                        sensors_present.Value = sysstatus.onboard_control_sensors_present;

                        terrainactive = sensors_health.terrain && sensors_enabled.terrain && sensors_present.terrain;

                        if (sensors_health.gps != sensors_enabled.gps && sensors_present.gps)
                        {
                            messageHigh = Strings.BadGPSHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.gyro != sensors_enabled.gyro && sensors_present.gyro)
                        {
                            messageHigh = Strings.BadGyroHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.accelerometer != sensors_enabled.accelerometer &&
                                 sensors_present.accelerometer)
                        {
                            messageHigh = Strings.BadAccelHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.compass != sensors_enabled.compass && sensors_present.compass)
                        {
                            messageHigh = Strings.BadCompassHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.barometer != sensors_enabled.barometer && sensors_present.barometer)
                        {
                            messageHigh = Strings.BadBaroHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.LASER_POSITION != sensors_enabled.LASER_POSITION &&
                                 sensors_present.LASER_POSITION)
                        {
                            messageHigh = Strings.BadLiDARHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.optical_flow != sensors_enabled.optical_flow &&
                                 sensors_present.optical_flow)
                        {
                            messageHigh = Strings.BadOptFlowHealth;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.terrain != sensors_enabled.terrain && sensors_present.terrain)
                        {
                            messageHigh = Strings.BadorNoTerrainData;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.geofence != sensors_enabled.geofence &&
                                 sensors_present.geofence)
                        {
                            messageHigh = Strings.GeofenceBreach;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.ahrs != sensors_enabled.ahrs && sensors_present.ahrs)
                        {
                            messageHigh = Strings.BadAHRS;
                            messageHighTime = DateTime.Now;
                        }
                        else if (sensors_health.rc_receiver != sensors_enabled.rc_receiver &&
                                 sensors_present.rc_receiver)
                        {
                            messageHigh = Strings.NORCReceiver;
                            messageHighTime = DateTime.Now;
                        }


                        MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SYS_STATUS] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.BATTERY2];
                    if (bytearray != null)
                    {
                        var bat = bytearray.ByteArrayToStructure<MAVLink.mavlink_battery2_t>(6);
                        _battery_voltage2 = bat.voltage;
                        current2 = bat.current_battery;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SCALED_PRESSURE];
                    if (bytearray != null)
                    {
                        var pres = bytearray.ByteArrayToStructure<MAVLink.mavlink_scaled_pressure_t>(6);
                        press_abs = pres.press_abs;
                        press_temp = pres.temperature;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.TERRAIN_REPORT];
                    if (bytearray != null)
                    {
                        var terrainrep = bytearray.ByteArrayToStructure<MAVLink.mavlink_terrain_report_t>(6);
                        ter_curalt = terrainrep.current_height;
                        ter_alt = terrainrep.terrain_height;
                        ter_load = terrainrep.loaded;
                        ter_pend = terrainrep.pending;
                        ter_space = terrainrep.spacing;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SENSOR_OFFSETS];
                    if (bytearray != null)
                    {
                        var sensofs = bytearray.ByteArrayToStructure<MAVLink.mavlink_sensor_offsets_t>(6);

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

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.ATTITUDE];

                    if (bytearray != null)
                    {
                        var att = bytearray.ByteArrayToStructure<MAVLink.mavlink_attitude_t>(6);

                        roll = att.roll*rad2deg;
                        pitch = att.pitch*rad2deg;
                        yaw = att.yaw*rad2deg;

                        //Console.WriteLine(MAV.sysid + " " +roll + " " + pitch + " " + yaw);

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.ATTITUDE] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT];
                    if (bytearray != null)
                    {
                        var loc = bytearray.ByteArrayToStructure<MAVLink.mavlink_global_position_int_t>(6);

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
                        }
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT];
                    if (bytearray != null)
                    {
                        var gps = bytearray.ByteArrayToStructure<MAVLink.mavlink_gps_raw_int_t>(6);

                        if (!useLocation)
                        {
                            lat = gps.lat*1.0e-7;
                            lng = gps.lon*1.0e-7;

                            altasl = gps.alt/1000.0f;
                            // alt = gps.alt; // using vfr as includes baro calc
                        }

                        gpsstatus = gps.fix_type;
                        //                    Console.WriteLine("gpsfix {0}",gpsstatus);

                        gpshdop = (float) Math.Round((double) gps.eph/100.0, 2);

                        satcount = gps.satellites_visible;

                        groundspeed = gps.vel*1.0e-2f;
                        groundcourse = gps.cog*1.0e-2f;

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.GPS_RAW] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.GPS2_RAW];
                    if (bytearray != null)
                    {
                        var gps = bytearray.ByteArrayToStructure<MAVLink.mavlink_gps2_raw_t>(6);

                        lat2 = gps.lat*1.0e-7;
                        lng2 = gps.lon*1.0e-7;
                        altasl2 = gps.alt/1000.0f;

                        gpsstatus2 = gps.fix_type;
                        gpshdop2 = (float) Math.Round((double) gps.eph/100.0, 2);

                        satcount2 = gps.satellites_visible;

                        groundspeed2 = gps.vel*1.0e-2f;
                        groundcourse2 = gps.cog*1.0e-2f;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.GPS_STATUS];
                    if (bytearray != null)
                    {
                        var gps = bytearray.ByteArrayToStructure<MAVLink.mavlink_gps_status_t>(6);
                        satcount = gps.satellites_visible;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.RADIO];
                    if (bytearray != null)
                    {
                        var radio = bytearray.ByteArrayToStructure<MAVLink.mavlink_radio_t>(6);
                        rssi = radio.rssi;
                        remrssi = radio.remrssi;
                        txbuffer = radio.txbuf;
                        rxerrors = radio.rxerrors;
                        noise = radio.noise;
                        remnoise = radio.remnoise;
                        fixedp = radio.@fixed;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.RADIO_STATUS];
                    if (bytearray != null)
                    {
                        var radio = bytearray.ByteArrayToStructure<MAVLink.mavlink_radio_status_t>(6);
                        rssi = radio.rssi;
                        remrssi = radio.remrssi;
                        txbuffer = radio.txbuf;
                        rxerrors = radio.rxerrors;
                        noise = radio.noise;
                        remnoise = radio.remnoise;
                        fixedp = radio.@fixed;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.MISSION_CURRENT];
                    if (bytearray != null)
                    {
                        var wpcur = bytearray.ByteArrayToStructure<MAVLink.mavlink_mission_current_t>(6);

                        int oldwp = (int) wpno;

                        wpno = wpcur.seq;

                        if (mode.ToLower() == "auto" && wpno != 0)
                        {
                            lastautowp = (int) wpno;
                        }

                        if (oldwp != wpno && MainV2.speechEnable && MainV2.comPort.MAV.cs == this &&
                            Settings.Instance.GetBoolean("speechwaypointenabled"))
                        {
                            MainV2.speechEngine.SpeakAsync(Common.speechConversion(""+ Settings.Instance["speechwaypoint"]));
                        }

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.WAYPOINT_CURRENT] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.NAV_CONTROLLER_OUTPUT];

                    if (bytearray != null)
                    {
                        var nav = bytearray.ByteArrayToStructure<MAVLink.mavlink_nav_controller_output_t>(6);

                        nav_roll = nav.nav_roll;
                        nav_pitch = nav.nav_pitch;
                        nav_bearing = nav.nav_bearing;
                        target_bearing = nav.target_bearing;
                        wp_dist = nav.wp_dist;
                        alt_error = nav.alt_error;
                        aspd_error = nav.aspd_error/100.0f;
                        xtrack_error = nav.xtrack_error;

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.NAV_CONTROLLER_OUTPUT] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.RPM];

                    if (bytearray != null)
                    {
                        var rpm = bytearray.ByteArrayToStructure<MAVLink.mavlink_rpm_t>(6);

                        rpm1 = rpm.rpm1;
                        rpm2 = rpm.rpm2;

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.NAV_CONTROLLER_OUTPUT] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.RC_CHANNELS_RAW];
                    if (bytearray != null)
                    {
                        var rcin = bytearray.ByteArrayToStructure<MAVLink.mavlink_rc_channels_raw_t>(6);

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

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RC_CHANNELS_RAW] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.RC_CHANNELS];
                    if (bytearray != null)
                    {
                        var rcin = bytearray.ByteArrayToStructure<MAVLink.mavlink_rc_channels_t>(6);

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

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RC_CHANNELS_RAW] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SERVO_OUTPUT_RAW];
                    if (bytearray != null)
                    {
                        var servoout = bytearray.ByteArrayToStructure<MAVLink.mavlink_servo_output_raw_t>(6);

                        ch1out = servoout.servo1_raw;
                        ch2out = servoout.servo2_raw;
                        ch3out = servoout.servo3_raw;
                        ch4out = servoout.servo4_raw;
                        ch5out = servoout.servo5_raw;
                        ch6out = servoout.servo6_raw;
                        ch7out = servoout.servo7_raw;
                        ch8out = servoout.servo8_raw;

                        MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SERVO_OUTPUT_RAW] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.RAW_IMU];
                    if (bytearray != null)
                    {
                        var imu = bytearray.ByteArrayToStructure<MAVLink.mavlink_raw_imu_t>(6);

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
                        if (speedup > 0)
                            speedup = (float) (speedup*0.95 + (deltaimu/deltawall)*0.05);

                        imutime = timesec;
                        lastimutime = DateTime.Now;

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RAW_IMU] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SCALED_IMU];
                    if (bytearray != null)
                    {
                        var imu = bytearray.ByteArrayToStructure<MAVLink.mavlink_scaled_imu_t>(6);

                        gx = imu.xgyro;
                        gy = imu.ygyro;
                        gz = imu.zgyro;

                        ax = imu.xacc;
                        ay = imu.yacc;
                        az = imu.zacc;

                        mx = imu.xmag;
                        my = imu.ymag;
                        mz = imu.zmag;

                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.RAW_IMU] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SCALED_IMU2];
                    if (bytearray != null)
                    {
                        var imu2 = bytearray.ByteArrayToStructure<MAVLink.mavlink_scaled_imu2_t>(6);

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


                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.SCALED_IMU3];
                    if (bytearray != null)
                    {
                        var imu3 = bytearray.ByteArrayToStructure<MAVLink.mavlink_scaled_imu3_t>(6);

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

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.PID_TUNING];
                    if (bytearray != null)
                    {
                        var pid = bytearray.ByteArrayToStructure<MAVLink.mavlink_pid_tuning_t>(6);

                        //todo: currently only deals with single axis at once

                        pidff = pid.FF;
                        pidP = pid.P;
                        pidI = pid.I;
                        pidD = pid.D;
                        pidaxis = pid.axis;
                        piddesired = pid.desired;
                        pidachieved = pid.achieved;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.VFR_HUD];
                    if (bytearray != null)
                    {
                        var vfr = bytearray.ByteArrayToStructure<MAVLink.mavlink_vfr_hud_t>(6);

                        groundspeed = vfr.groundspeed;

                        airspeed = vfr.airspeed;

                        //alt = vfr.alt; // this might include baro

                        ch3percent = vfr.throttle;

                        if (sensors_present.revthrottle && sensors_enabled.revthrottle && sensors_health.revthrottle)
                            ch3percent *= -1;

                        //Console.WriteLine(alt);

                        //climbrate = vfr.climb;

                        // heading = vfr.heading;


                        //MAVLink.packets[(byte)MAVLink.MSG_NAMES.VFR_HUD] = null;
                    }

                    bytearray = MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.MEMINFO];
                    if (bytearray != null)
                    {
                        var mem = bytearray.ByteArrayToStructure<MAVLink.mavlink_meminfo_t>(6);
                        freemem = mem.freemem;
                        brklevel = mem.brkval;
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
                        bs.DataSource = this;
                        bs.ResetBindings(false);

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

            if (airspeed < 1 || groundspeed < 1)
                return;

            double Wn_error = airspeed*Math.Cos((yaw)*deg2rad)*Math.Cos(pitch*deg2rad) -
                              groundspeed*Math.Cos((groundcourse)*deg2rad) - Wn_fgo;
            double We_error = airspeed*Math.Sin((yaw)*deg2rad)*Math.Cos(pitch*deg2rad) -
                              groundspeed*Math.Sin((groundcourse)*deg2rad) - We_fgo;

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

            public Mavlink_Sensors()
            {
                //var item = MAVLink.MAV_SYS_STATUS_SENSOR._3D_ACCEL;
            }

            public Mavlink_Sensors(uint p)
            {
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
                set { bitArray = new BitArray((int)value); }
            }

            public override string ToString()
            {
                return Convert.ToString(Value,2);
            }
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
                return (float) GimbalPoint.Lat;
            }
        }

        public float gimballng
        {
            get
            {
                if (GimbalPoint == null) return 0;
                return (float) GimbalPoint.Lng;
            }
        }


        public bool landed { get; set; }

        public bool terrainactive { get; set; }

        float _ter_curalt;

        [DisplayText("Terrain AGL")]
        public float ter_curalt
        {
            get { return _ter_curalt*multiplierdist; }
            set { _ter_curalt = value; }
        }

        float _ter_alt;

        [DisplayText("Terrain GL")]
        public float ter_alt
        {
            get { return _ter_alt*multiplierdist; }
            set { _ter_alt = value; }
        }

        public float ter_load { get; set; }

        public float ter_pend { get; set; }

        public float ter_space { get; set; }

        public static int KIndexstatic = -1;

        public int KIndex
        {
            get { return (int) CurrentState.KIndexstatic; }
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
    }
}