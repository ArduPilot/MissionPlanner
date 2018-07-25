using System;
using System.Collections.Generic;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using log4net;
using System.Reflection;
using MissionPlanner.Utilities;
using System.IO;
using MissionPlanner.Maps;

namespace MissionPlanner.ArduPilot
{
    //from px4firmwareplugin.cc

    public class Common
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        // list of valid options enterd from https://github.com/ArduPilot/ardupilot/blob/master/libraries/AP_Motors/AP_MotorsMatrix.cpp#L378
        public readonly static List<Tuple<motor_frame_class, motor_frame_type?>> ValidList =
            new List<Tuple<motor_frame_class, motor_frame_type?>>()
            {
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_QUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_PLUS),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_QUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_X),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_QUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_V),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_QUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_H),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_QUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_VTAIL),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_QUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_ATAIL),

                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_HEXA,
                    motor_frame_type.MOTOR_FRAME_TYPE_PLUS),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_HEXA,
                    motor_frame_type.MOTOR_FRAME_TYPE_X),

                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_OCTA,
                    motor_frame_type.MOTOR_FRAME_TYPE_PLUS),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_OCTA,
                    motor_frame_type.MOTOR_FRAME_TYPE_X),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_OCTA,
                    motor_frame_type.MOTOR_FRAME_TYPE_V),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_OCTA,
                    motor_frame_type.MOTOR_FRAME_TYPE_H),

                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_OCTAQUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_PLUS),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_OCTAQUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_X),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_OCTAQUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_V),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_OCTAQUAD,
                    motor_frame_type.MOTOR_FRAME_TYPE_H),

                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_DODECAHEXA,
                    motor_frame_type.MOTOR_FRAME_TYPE_PLUS),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_DODECAHEXA,
                    motor_frame_type.MOTOR_FRAME_TYPE_X),

                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_Y6,
                    motor_frame_type.MOTOR_FRAME_TYPE_Y6B),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_Y6,
                    motor_frame_type.MOTOR_FRAME_TYPE_X),

                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_HELI,
                    null),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_TRI,
                    null),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_SINGLE,
                    null),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_COAX,
                    null),

                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_TAILSITTER,
                    null),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_HELI_DUAL,
                    null),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_DODECAHEXA,
                    null),
                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_HELI_QUAD,
                    null),

                new Tuple<motor_frame_class, motor_frame_type?>(motor_frame_class.MOTOR_FRAME_UNDEFINED,
                    null),
            };
   

        public static List<KeyValuePair<int, string>> getModesList(Firmwares firmware)
        {
            log.Info("getModesList Called");

            if (firmware == Firmwares.PX4)
            {
                /*
union px4_custom_mode {
    struct {
        uint16_t reserved;
        uint8_t main_mode;
        uint8_t sub_mode;
    };
    uint32_t data;
    float data_float;
};
                 */


                var temp = new List<KeyValuePair<int, string>>()
                {
                    new KeyValuePair<int, string>((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_MANUAL << 16, "Manual"),
                    new KeyValuePair<int, string>((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_ACRO << 16, "Acro"),
                    new KeyValuePair<int, string>((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_STABILIZED << 16,
                        "Stabalized"),
                    new KeyValuePair<int, string>((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_RATTITUDE << 16,
                        "Rattitude"),
                    new KeyValuePair<int, string>((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_ALTCTL << 16,
                        "Altitude Control"),
                    new KeyValuePair<int, string>((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_POSCTL << 16,
                        "Position Control"),
                    new KeyValuePair<int, string>((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_OFFBOARD << 16,
                        "Offboard Control"),
                    new KeyValuePair<int, string>(
                        ((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_AUTO << 16) +
                        (int) PX4_CUSTOM_SUB_MODE_AUTO.PX4_CUSTOM_SUB_MODE_AUTO_READY << 24, "Auto: Ready"),
                    new KeyValuePair<int, string>(
                        ((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_AUTO << 16) +
                        (int) PX4_CUSTOM_SUB_MODE_AUTO.PX4_CUSTOM_SUB_MODE_AUTO_TAKEOFF << 24, "Auto: Takeoff"),
                    new KeyValuePair<int, string>(
                        ((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_AUTO << 16) +
                        (int) PX4_CUSTOM_SUB_MODE_AUTO.PX4_CUSTOM_SUB_MODE_AUTO_LOITER << 24, "Loiter"),
                    new KeyValuePair<int, string>(
                        ((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_AUTO << 16) +
                        (int) PX4_CUSTOM_SUB_MODE_AUTO.PX4_CUSTOM_SUB_MODE_AUTO_MISSION << 24, "Auto"),
                    new KeyValuePair<int, string>(
                        ((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_AUTO << 16) +
                        (int) PX4_CUSTOM_SUB_MODE_AUTO.PX4_CUSTOM_SUB_MODE_AUTO_RTL << 24, "RTL"),
                    new KeyValuePair<int, string>(
                        ((int) PX4_CUSTOM_MAIN_MODE.PX4_CUSTOM_MAIN_MODE_AUTO << 16) +
                        (int) PX4_CUSTOM_SUB_MODE_AUTO.PX4_CUSTOM_SUB_MODE_AUTO_LAND << 24, "Auto: Landing")
                };

                return temp;
            }
            else if (firmware == Firmwares.ArduPlane)
            {
                var flightModes = Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("FLTMODE1",
                    firmware.ToString());
                flightModes.Add(new KeyValuePair<int, string>(16, "INITIALISING"));

                flightModes.Add(new KeyValuePair<int, string>(17, "QStabilize"));
                flightModes.Add(new KeyValuePair<int, string>(18, "QHover"));
                flightModes.Add(new KeyValuePair<int, string>(19, "QLoiter"));
                flightModes.Add(new KeyValuePair<int, string>(20, "QLand"));
                flightModes.Add(new KeyValuePair<int, string>(21, "QRTL"));

                return flightModes;
            }
            else if (firmware == Firmwares.Ateryx)
            {
                var flightModes = Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("FLTMODE1",
                    firmware.ToString()); //same as apm
                return flightModes;
            }
            else if (firmware == Firmwares.ArduCopter2)
            {
                var flightModes = Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("FLTMODE1",
                    firmware.ToString());
                return flightModes;
            }
            else if (firmware == Firmwares.ArduRover)
            {
                var flightModes = Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("MODE1",
                    firmware.ToString());
                return flightModes;
            }
            else if (firmware == Firmwares.ArduTracker)
            {
                var temp = new List<KeyValuePair<int, string>>();
                temp.Add(new KeyValuePair<int, string>(0, "MANUAL"));
                temp.Add(new KeyValuePair<int, string>(1, "STOP"));
                temp.Add(new KeyValuePair<int, string>(2, "SCAN"));
                temp.Add(new KeyValuePair<int, string>(3, "SERVO_TEST"));
                temp.Add(new KeyValuePair<int, string>(10, "AUTO"));
                temp.Add(new KeyValuePair<int, string>(16, "INITIALISING"));

                return temp;
            }

            return null;
        }

        public static string speechConversion(MAVState MAV, string input)
        {
            if (MAV.cs.wpno == 0)
            {
                input = input.Replace("{wpn}", "Home");
            }
            else
            {
                input = input.Replace("{wpn}", MAV.cs.wpno.ToString());
            }

            input = input.Replace("{asp}", MAV.cs.airspeed.ToString("0"));

            input = input.Replace("{alt}", MAV.cs.alt.ToString("0"));

            input = input.Replace("{wpa}", MAV.cs.targetalt.ToString("0"));

            input = input.Replace("{gsp}", MAV.cs.groundspeed.ToString("0"));

            input = input.Replace("{mode}", MAV.cs.mode.ToString());

            input = input.Replace("{batv}", MAV.cs.battery_voltage.ToString("0.00"));

            input = input.Replace("{batp}", (MAV.cs.battery_remaining).ToString("0"));

            input = input.Replace("{vsp}", (MAV.cs.verticalspeed).ToString("0.0"));

            input = input.Replace("{curr}", (MAV.cs.current).ToString("0.0"));

            input = input.Replace("{hdop}", (MAV.cs.gpshdop).ToString("0.00"));

            input = input.Replace("{satcount}", (MAV.cs.satcount).ToString("0"));

            input = input.Replace("{rssi}", (MAV.cs.rssi).ToString("0"));

            input = input.Replace("{disthome}", (MAV.cs.DistToHome).ToString("0"));

            input = input.Replace("{timeinair}",
                (new TimeSpan(0, 0, 0, (int)MAV.cs.timeInAir)).ToString());

            try
            {
                object thisBoxed = MAV.cs;
                Type test = thisBoxed.GetType();

                PropertyInfo[] props = test.GetProperties();

                //props
                foreach (var field in props)
                {
                    // field.Name has the field's name.
                    object fieldValue;
                    TypeCode typeCode;
                    try
                    {
                        fieldValue = field.GetValue(thisBoxed, null); // Get value

                        if (fieldValue == null)
                            continue;
                        // Get the TypeCode enumeration. Multiple types get mapped to a common typecode.
                        typeCode = Type.GetTypeCode(fieldValue.GetType());
                    }
                    catch
                    {
                        continue;
                    }

                    var fname = String.Format("{{{0}}}", field.Name);
                    input = input.Replace(fname, fieldValue.ToString());
                }
            }
            catch
            {
                
            }

            return input;
        }

       

        public static GMapMarker getMAVMarker(MAVState MAV)
        {
            PointLatLng portlocation = new PointLatLng(MAV.cs.lat, MAV.cs.lng);

            if (MAV.aptype == MAVLink.MAV_TYPE.FIXED_WING)
            {
                // colorise map marker/s based on their sysid, for common sysid/s used 1-6, 11-16, and 101-106
                // its rare for ArduPilot to be used to fly more than 6 planes at a time from one console.
                int which = 0; // default 0=red for other sysids
                if ((MAV.sysid >= 1) && (MAV.sysid <= 6)) { which = MAV.sysid-1; }  //1=black, 2=blue, 3=green,4=yellow,5=orange,6=red
                if ((MAV.sysid >= 11) && (MAV.sysid <= 16)) { which = MAV.sysid-11; }  //1=black, 2=blue, 3=green,4=yellow,5=orange,6=red
                if ((MAV.sysid >= 101) && (MAV.sysid <= 106)) { which = MAV.sysid-101; }  //1=black, 2=blue, 3=green,4=yellow,5=orange,6=red

                return (new GMapMarkerPlane(which, portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing,
                    MAV.cs.radius * CurrentState.multiplierdist)
                {
                    ToolTipText = MAV.cs.alt.ToString("0") + CurrentState.AltUnit + " | " + (int) MAV.cs.airspeed +
                                  CurrentState.SpeedUnit + " | id:" + (int) MAV.sysid,
                    ToolTipMode = MarkerTooltipMode.Always
                });
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.GROUND_ROVER)
            {
                return  (new GMapMarkerRover(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing)
                {
                    ToolTipText = MAV.cs.alt.ToString("0") + "\n" + MAV.sysid.ToString("sysid: 0"),
                    ToolTipMode = MarkerTooltipMode.Always
                });
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SURFACE_BOAT)
            { 
                return (new GMapMarkerBoat(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing));
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SUBMARINE)
            {
                return (new GMapMarkerSub(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing));
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER)
            {
                return (new GMapMarkerHeli(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing));
            }
            else if (MAV.cs.firmware == Firmwares.ArduTracker)
            {
                return (new GMapMarkerAntennaTracker(portlocation, MAV.cs.yaw,
                    MAV.cs.target_bearing));
            }
            else if (MAV.cs.firmware == Firmwares.ArduCopter2 || MAV.aptype == MAVLink.MAV_TYPE.QUADROTOR)
            {
                if (MAV.param.ContainsKey("AVD_W_DIST_XY") && MAV.param.ContainsKey("AVD_F_DIST_XY"))
                {
                    var w = MAV.param["AVD_W_DIST_XY"].Value;
                    var f = MAV.param["AVD_F_DIST_XY"].Value;
                    return (new GMapMarkerQuad(portlocation, MAV.cs.yaw,
                        MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid)
                    {
                        danger = (int)f,
                        warn = (int)w
                    });
                }

                return (new GMapMarkerQuad(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid));
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.COAXIAL)
            {
                return (new GMapMarkerSingle(portlocation, MAV.cs.yaw,
                   MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid));
            }
            else
            {
                // unknown type
                return (new GMarkerGoogle(portlocation, GMarkerGoogleType.green_dot));
            }
        }
    }
}