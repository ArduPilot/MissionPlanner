using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules.PythonThread;

namespace TerrainMakerPlugin
{
    public class Location
    {


        const double LOCATION_SCALING_FACTOR = 0.011131884502145034;
        const double LOCATION_SCALING_FACTOR_INV = 89.83204953368922;

        // note that mission storage only stores 24 bits of altitude (~ +/- 83km)
        public Int32 lat; // in 1E7 degrees
        public Int32 lng; // in 1E7 degrees


        public Location()
        {
            lat = 0;
            lng = 0;
        }

        public Location(Int32 lat, Int32 lng)
        {
            this.lat = lat;
            this.lng = lng;
        }



        // return horizontal distance in meters between two locations
        public double get_distance(Location loc2)
        {
            double dlat = (double)(loc2.lat - lat);
            double dlng = ((double)diff_longitude(loc2.lng, lng)) * longitude_scale((lat + loc2.lat) / 2);

            return Math.Sqrt(dlat*dlat +  dlng*dlng) * LOCATION_SCALING_FACTOR;
        }


        public double longitude_scale(Int32 lat)
        {

            double scale = Math.Cos(lat * (1.0e-7 * (Math.PI / 180.0)));
            return Math.Max(scale, 0.01);
        }

        /*
          get lon1-lon2, wrapping at -180e7 to 180e7
        */
        public Int32 diff_longitude(Int32 lon1, Int32 lon2)
        {
            if ((lon1 & 0x80000000) == (lon2 & 0x80000000))
            {
                // common case of same sign
                return lon1 - lon2;
            }
            long dlon = (long)lon1 - (long)lon2;
            if (dlon > 1800000000L)
            {
                dlon -= 3600000000L;
            }
            else if (dlon < -1800000000L)
            {
                dlon += 3600000000L;
            }
            return (Int32)dlon;
        }


        /*
          return the distance in meters in North/East plane as a N/E vector
          from loc1 to loc2
        */
        public Vector2d get_distance_NE(Location loc2)
        {

            Vector2d retvect = new Vector2d((loc2.lat - lat) * LOCATION_SCALING_FACTOR,
                    diff_longitude(loc2.lng, lng) * LOCATION_SCALING_FACTOR * longitude_scale((loc2.lat + lat) / 2));

            return retvect;
        }


        // extrapolate latitude/longitude given distances (in meters) north and east
        public Location add_offset_meters(double ofs_north, double ofs_east)
        {
            double dlat = ofs_north * LOCATION_SCALING_FACTOR_INV;
            double dlng = (ofs_east * LOCATION_SCALING_FACTOR_INV) / longitude_scale((Int32)(lat + dlat / 2));

            Location retval = new Location(lat + (Int32)dlat, (Int32)(dlng + lng));

            return retval;

        }


    }


    public class Vector2d
    {
        public double x;
        public double y;


        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }




    }



}
