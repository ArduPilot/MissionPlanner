using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using int32_t = System.Int32;

namespace MissionPlanner.Utilities
{
    class AP_Terrain : HIL.Utils
    {
        private bool enable = true;
        private int32_t grid_spacing = 100;
        /*
        calculate lookahead rise in terrain. This returns extra altitude
        needed to clear upcoming terrain in meters
        */

        float lookahead(Locationwp loc, float bearing, float distance, float climb_ratio)
        {
            if (!enable || grid_spacing <= 0)
            {
                return 0;
            }

            //Locationwp loc;
            //if (!ahrs.get_position(loc))
            {
                // we don't know where we are
                //return 0;
            }
            float base_height = 0;
            if (!height_amsl(loc, ref base_height))
            {
                // we don't know our current terrain height
                return 0;
            }

            float climb = 0;
            float lookahead_estimate = 0;

            // check for terrain at grid spacing intervals
            while (distance > 0)
            {
                location_update(loc, bearing, grid_spacing);
                climb += climb_ratio*grid_spacing;
                distance -= grid_spacing;
                float height = 0;
                if (height_amsl(loc, ref height))
                {
                    float rise = (height - base_height) - climb;
                    if (rise > lookahead_estimate)
                    {
                        lookahead_estimate = rise;
                    }
                }
            }

            return lookahead_estimate;
        }


        /*
        return terrain height in meters above average sea level (WGS84) for
        a given position

        This is the base function that other height calculations are derived
        from. The functions below are more convenient for most uses

        This function costs about 20 microseconds on Pixhawk
        */

        bool height_amsl(Locationwp loc, ref float height)
        {
            height = (float) srtm.getAltitude(loc.lat, loc.lng).alt;
            return true;
        }

        /*
        *  extrapolate latitude/longitude given bearing and distance
        * Note that this function is accurate to about 1mm at a distance of 
        * 100m. This function has the advantage that it works in relative
        * positions, so it keeps the accuracy even when dealing with small
        * distances and floating point numbers
        */

        void location_update(Locationwp loc, float bearing, float distance)
        {
            float ofs_north = cosf(radians(bearing))*distance;
            float ofs_east = sinf(radians(bearing))*distance;
            location_offset(loc, ofs_north, ofs_east);
        }

        /*
        *  extrapolate latitude/longitude given distances north and east
        *  This function costs about 80 usec on an AVR2560
        */

        void location_offset(Locationwp loc, float ofs_north, float ofs_east)
        {
            if (!is_zero(ofs_north) || !is_zero(ofs_east))
            {
                int32_t dlat = (int32_t) (ofs_north*LOCATION_SCALING_FACTOR_INV);
                int32_t dlng = (int32_t) ((ofs_east*LOCATION_SCALING_FACTOR_INV)/longitude_scale(loc));
                loc.lat += dlat;
                loc.lng += dlng;
            }
        }

        float longitude_scale(Locationwp loc)
        {
            int32_t last_lat = -999999999;
            float scale = 1.0f;
            if (labs(last_lat - loc.lat) < 100000)
            {
                // we are within 0.01 degrees (about 1km) of the
                // same latitude. We can avoid the cos() and return
                // the same scale factor.
                return scale;
            }
            scale = cosf(loc.lat*1.0e-7f*DEG_TO_RAD);
            scale = constrain_float(scale, 0.01f, 1.0f);
            last_lat = (int32_t) loc.lat;
            return scale;
        }
    }
}