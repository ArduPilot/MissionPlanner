using System;
using System.Collections.Generic;
using System.Linq;
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
            return new MAVLink.mavlink_mission_item_t()
            {
                command = input.id,
                param1 = input.p1,
                param2 = input.p2,
                param3 = input.p3,
                param4 = input.p4,
                x = (float)input.lat,
                y = (float)input.lng,
                z = (float)input.alt
            };
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
