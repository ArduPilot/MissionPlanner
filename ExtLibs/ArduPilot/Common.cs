using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MissionPlanner.ArduPilot
{
    //from px4firmwareplugin.cc

    public class Common
    {
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

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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

       

   }
}