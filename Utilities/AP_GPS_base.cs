using System;
using uint32_t = System.UInt32;
using uint16_t = System.UInt16;
using uint8_t = System.Byte;
using int32_t = System.Int32;
using int8_t = System.SByte;

namespace MissionPlanner.Utilities
{
    public class AP_GPS_base
    {
        public GPS_State state;

        public double safe_sqrt(double ground_vector_sq)
        {
            return Math.Sqrt(ground_vector_sq);
        }

        public double ToDeg(double p)
        {
            return p*(180/Math.PI);
        }

        public double atan2f(double p1, double p2)
        {
            return Math.Atan2(p1, p2);
        }

        public double ToRad(double p)
        {
            return p * (Math.PI / 180);
        }

        public void fill_3d_velocity()
        {
            double gps_heading = ToRad(state.ground_course);

            state.velocity.x = state.ground_speed * (float)Math.Cos(gps_heading);
            state.velocity.y = state.ground_speed * (float)Math.Sin(gps_heading);
            state.velocity.z = 0;
            state.have_vertical_velocity = false;
        }

        public struct Location
        {
            public uint8_t options;

            /// allows writing all flags to eeprom as one byte

            // by making alt 24 bit we can make p1 in a command 16 bit,
            // allowing an accurate angle in centi-degrees. This keeps the
            // storage cost per mission item at 15 bytes, and allows mission
            // altitudes of up to +/- 83km
            public int32_t alt;

            ///< param 2 - Altitude in centimeters (meters * 100)
            public int32_t lat;

            ///< param 3 - Lattitude * 10**7
            public int32_t lng;

            ///< param 4 - Longitude * 10**7
            ///
            public override string ToString()
            {
                return lat + ", " + lng + ", " + alt;
            }
        };

        public enum AP_GPS
        {
            NO_GPS = 0,

            ///< No GPS connected/detected
            NO_FIX = 1,

            ///< Receiving valid GPS messages but no lock
            GPS_OK_FIX_2D = 2,

            ///< Receiving valid messages and 2D lock
            GPS_OK_FIX_3D = 3,

            ///< Receiving valid messages and 3D lock
            GPS_OK_FIX_3D_DGPS = 4,

            ///< Receiving valid messages and 3D lock with differential improvements
            GPS_OK_FIX_3D_RTK = 5, ///< Receiving valid messages and 3D lock, with relative-positioning improvements
        };

        public struct GPS_State
        {
            public uint8_t instance; // the instance number of this GPS

            // all the following fields must all be filled by the backend driver
            public AP_GPS status;

            ///< driver fix status
            public uint32_t time_week_ms;

            ///< GPS time (milliseconds from start of GPS week)
            public uint16_t time_week;

            ///< GPS week number
            public Location location;

            ///< last fix location
            public float ground_speed;

            ///< ground speed in m/sec
            public float ground_course;

            ///< ground course in degree
            public uint16_t hdop;

            ///< horizontal dilution of precision in cm
             
            public uint16_t vdop;
            public uint8_t num_sats;

            ///< Number of visible satelites        
            public Vector3 velocity;

            ///< 3D velocitiy in m/s, in NED format
            public float speed_accuracy;

            public float horizontal_accuracy;
            public float vertical_accuracy;
            public bool have_vertical_velocity; //:1;      ///< does this GPS give vertical velocity?
            public bool have_speed_accuracy; //:1;
            public bool have_horizontal_accuracy; //:1;
            public bool have_vertical_accuracy; //:1;
            public uint32_t last_gps_time_ms; ///< the system time we got the last GPS timestamp, milliseconds
        }
    }
}
