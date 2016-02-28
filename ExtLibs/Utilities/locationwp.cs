using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Struct as used in Ardupilot
    /// </summary>
    public struct Locationwp
    {
        public Locationwp Set(double lat, double lng, double alt, byte id)
        {
            this.lat = lat;
            this.lng = lng;
            this.alt = (float)alt;
            this.id = id;

            return this;
        }

        public static implicit operator MAVLink.mavlink_mission_item_t(Locationwp input)
        {
            return (MAVLink.mavlink_mission_item_t)Convert(input, false);
        }

        public static implicit operator MAVLink.mavlink_mission_item_int_t(Locationwp input)
        {
            return (MAVLink.mavlink_mission_item_int_t)Convert(input, true);
        }

        public static implicit operator Locationwp(MAVLink.mavlink_mission_item_t input)
        {
            Locationwp temp = new Locationwp()
            {
                id = (byte)input.command,
                p1 = input.param1,
                p2 = input.param2,
                p3 = input.param3,
                p4 = input.param4,
                lat = input.x,
                lng = input.y,
                alt = input.z
            };

            return temp;
        }

        public static implicit operator Locationwp(MAVLink.mavlink_mission_item_int_t input)
        {
            bool copy_location = Locationwp.copy_location((MAVLink.MAV_CMD)input.command);

            Locationwp temp = new Locationwp()
            {
                id = (byte)input.command,
                p1 = input.param1,
                p2 = input.param2,
                p3 = input.param3,
                p4 = input.param4,
                lat = input.x,
                lng = input.y,
                alt = input.z
            };

            // convert int input back to float
            if (copy_location)
            {
                temp.lat = input.x/1.0e7;
                temp.lng = input.y/1.0e7;
            }

            return temp;
        }

        /// <summary>
        /// extracted from ap_mission.cpp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static bool copy_location(MAVLink.MAV_CMD id)
        {
            bool copy_location = false;

            // command specific conversions from mavlink packet to mission command
            switch (id)
            {
                case MAVLink.MAV_CMD.WAYPOINT: // MAV ID: 16
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.LOITER_UNLIM: // MAV ID: 17
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.LOITER_TURNS: // MAV ID: 18
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.LOITER_TIME: // MAV ID: 19
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.RETURN_TO_LAUNCH: // MAV ID: 20
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.LAND: // MAV ID: 21
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.TAKEOFF: // MAV ID: 22
                    copy_location = true; // only altitude is used
                    break;

                case MAVLink.MAV_CMD.CONTINUE_AND_CHANGE_ALT: // MAV ID: 30
                    copy_location = true; // lat/lng used for heading lock
                    break;

                case MAVLink.MAV_CMD.LOITER_TO_ALT: // MAV ID: 31
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.SPLINE_WAYPOINT: // MAV ID: 82
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.GUIDED_ENABLE: // MAV ID: 92
                    break;

                case MAVLink.MAV_CMD.CONDITION_DELAY: // MAV ID: 112
                    break;

                case MAVLink.MAV_CMD.CONDITION_CHANGE_ALT: // MAV ID: 113
                    break;

                case MAVLink.MAV_CMD.CONDITION_DISTANCE: // MAV ID: 114
                    break;

                case MAVLink.MAV_CMD.CONDITION_YAW: // MAV ID: 115
                    break;

                case MAVLink.MAV_CMD.DO_SET_MODE: // MAV ID: 176
                    break;

                case MAVLink.MAV_CMD.DO_JUMP: // MAV ID: 177
                    break;

                case MAVLink.MAV_CMD.DO_CHANGE_SPEED: // MAV ID: 178
                    break;

                case MAVLink.MAV_CMD.DO_SET_HOME:
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.DO_SET_PARAMETER: // MAV ID: 180
                    break;

                case MAVLink.MAV_CMD.DO_SET_RELAY: // MAV ID: 181
                    break;

                case MAVLink.MAV_CMD.DO_REPEAT_RELAY: // MAV ID: 182
                    break;

                case MAVLink.MAV_CMD.DO_SET_SERVO: // MAV ID: 183
                    break;

                case MAVLink.MAV_CMD.DO_REPEAT_SERVO: // MAV ID: 184
                    break;

                case MAVLink.MAV_CMD.DO_LAND_START: // MAV ID: 189
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.DO_SET_ROI: // MAV ID: 201
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.DO_DIGICAM_CONFIGURE: // MAV ID: 202
                    break;

                case MAVLink.MAV_CMD.DO_DIGICAM_CONTROL: // MAV ID: 203
                    break;

                case MAVLink.MAV_CMD.DO_MOUNT_CONTROL: // MAV ID: 205
                    break;

                case MAVLink.MAV_CMD.DO_SET_CAM_TRIGG_DIST: // MAV ID: 206
                    break;

                case MAVLink.MAV_CMD.DO_FENCE_ENABLE: // MAV ID: 207
                    break;

                case MAVLink.MAV_CMD.DO_PARACHUTE: // MAV ID: 208
                    break;

                case MAVLink.MAV_CMD.DO_INVERTED_FLIGHT: // MAV ID: 210
                    break;

                case MAVLink.MAV_CMD.DO_GRIPPER: // MAV ID: 211
                    break;

                case MAVLink.MAV_CMD.DO_GUIDED_LIMITS: // MAV ID: 222
                    break;

                case MAVLink.MAV_CMD.DO_AUTOTUNE_ENABLE: // MAV ID: 211
                    break;

                case MAVLink.MAV_CMD.ALTITUDE_WAIT: // MAV ID: 83
                    break;

                case MAVLink.MAV_CMD.VTOL_TAKEOFF:
                    copy_location = true;
                    break;

                case MAVLink.MAV_CMD.VTOL_LAND:
                    copy_location = true;
                    break;

                default:
                    // unrecognised command
                    return false; //MAV_MISSION_UNSUPPORTED;
            }

            return copy_location;
        }

        static object Convert(Locationwp cmd, bool isint = false)
        {
            bool copy_location = Locationwp.copy_location((MAVLink.MAV_CMD) cmd.id);

            if (isint)
            {
                var temp = new MAVLink.mavlink_mission_item_int_t()
                {
                    command = cmd.id,
                    param1 = cmd.p1,
                    param2 = cmd.p2,
                    param3 = cmd.p3,
                    param4 = cmd.p4,
                    x = (int) cmd.lat,
                    y = (int) cmd.lng,
                    z = (float) cmd.alt
                };

                if (copy_location)
                {
                    temp.x = (int) (cmd.lat*1.0e7);
                    temp.y = (int) (cmd.lng*1.0e7);
                }

                return temp;
            }
            else
            {
                var temp = new MAVLink.mavlink_mission_item_t()
                {
                    command = cmd.id,
                    param1 = cmd.p1,
                    param2 = cmd.p2,
                    param3 = cmd.p3,
                    param4 = cmd.p4,
                    x = (float) cmd.lat,
                    y = (float) cmd.lng,
                    z = (float) cmd.alt
                };

                return temp;
            }
        }

        public byte id;				// command id
        public byte options;
        public float p1;				// param 1
        public float p2;				// param 2
        public float p3;				// param 3
        public float p4;				// param 4
        public double lat;				// Lattitude * 10**7
        public double lng;				// Longitude * 10**7
        public float alt;				// Altitude in centimeters (meters * 100)
    };
}
