using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackerHomeGPS
{
    public struct GPSPosition
    {
        public static explicit operator GPSPosition(GarminUSBGPS.GarminRadianPosition grp)
        {
            GPSPosition temp = new GPSPosition();
            temp.Lat = grp.lat * (180.0 / Math.PI);
            temp.Lng = grp.lon * (180.0 / Math.PI);

            return temp;
        }

        /// <summary>
        /// Latitude (degrees)
        /// </summary>
        public double Lat;
        /// <summary>
        /// Longitude (degrees)
        /// </summary>
        public double Lng;

        public double LatRad()
        {
            return Lat * (Math.PI / 180.0);
        }

        public double LngRad()
        {
            return Lng * (Math.PI / 180.0);
        }
    }

    public abstract class GPSDevice : Device
    {
        //public event EventHandler GPSAvailable;
        //public event EventHandler GPSUnavailable;

        public abstract GPSPosition GetCoordinates();
        public abstract bool IsAvailable();
        public abstract string DeviceName();
    }
}
