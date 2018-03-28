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
        public Locationwp Set(double lat, double lng, double alt, ushort id)
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

        public static implicit operator Locationwp(MAVLink.mavlink_set_position_target_global_int_t input)
        {
            Locationwp temp = new Locationwp()
            {
                id = (byte) MAVLink.MAV_CMD.WAYPOINT,
                p1 = 0,
                p2 = 0,
                p3 = 0,
                p4 = 0,
                lat = input.lat_int / 1e7,
                lng = input.lon_int / 1e7,
                alt = input.alt,
                _seq = 0,
                _frame = input.coordinate_frame
            };

            return temp;
        }

        public static implicit operator Locationwp(MAVLink.mavlink_mission_item_t input)
        {
            Locationwp temp = new Locationwp()
            {
                id = input.command,
                p1 = input.param1,
                p2 = input.param2,
                p3 = input.param3,
                p4 = input.param4,
                lat = input.x,
                lng = input.y,
                alt = input.z,
                _seq = input.seq,
                _frame = input.frame
            };

            return temp;
        }

        public static implicit operator Locationwp(MAVLink.mavlink_mission_item_int_t input)
        {
            Locationwp temp = new Locationwp()
            {
                id = input.command,
                p1 = input.param1,
                p2 = input.param2,
                p3 = input.param3,
                p4 = input.param4,
                lat = input.x / 1.0e7,
                lng = input.y / 1.0e7,
                alt = input.z,
                _seq = input.seq,
                _frame = input.frame
            };

            return temp;
        }

        public static implicit operator Locationwp(MissionFile.Item input)
        {
            Locationwp temp = new Locationwp()
            {
                id = (ushort)input.command,
                p1 = (float)input.@params[0],
                p2 = (float)input.@params[1],
                p3 = (float)input.@params[2],
                p4 = (float)input.@params[3],
                lat = input.coordinate[0],
                lng = input.coordinate[1],
                alt = (float)input.coordinate[2],
                _seq = (ushort)input.doJumpId,
                _frame = (byte)input.frame
            };

            return temp;
        }

        public static implicit operator MissionFile.Item(Locationwp input)
        {
            MissionFile.Item temp = new MissionFile.Item()
            {
                command = input.id,
                @params = new List<double>(new double[] { input.p1,input.p2,input.p3,input.p4}),
                coordinate = new List<double>(new double[] { input.lat, input.lng, input.alt }),
                doJumpId = input._seq,
                frame = input._frame
            };

            return temp;
        }

        static object Convert(Locationwp cmd, bool isint = false)
        {
            if (isint)
            {
                var temp = new MAVLink.mavlink_mission_item_int_t()
                {
                    command = cmd.id,
                    param1 = cmd.p1,
                    param2 = cmd.p2,
                    param3 = cmd.p3,
                    param4 = cmd.p4,
                    x = (int)(cmd.lat * 1.0e7),
                    y = (int)(cmd.lng * 1.0e7),
                    z = (float) cmd.alt,
                    seq = cmd._seq,
                    frame = cmd._frame
                };

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
                    z = (float) cmd.alt,
                    seq = cmd._seq,
                    frame = cmd._frame
                };

                return temp;
            }
        }

        private ushort _seq;
        private byte _frame;
        public object Tag;

        public ushort id;				// command id
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
