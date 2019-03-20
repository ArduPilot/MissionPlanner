using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.GeoRef
{
    public class SingleLocation: ICloneable
    {
        DateTime time;

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        double lat;

        public double Lat
        {
            get { return lat; }
            set { lat = value; }
        }

        double lon;

        public double Lon
        {
            get { return lon; }
            set { lon = value; }
        }

        double altAMSL;

        public double AltAMSL
        {
            get { return altAMSL; }
            set { altAMSL = value; }
        }

        double relAlt;

        public double RelAlt
        {
            get { return relAlt; }
            set { relAlt = value; }
        }

        double gpsalt;

        public double GPSAlt
        {
            get { return gpsalt; }
            set { gpsalt = value; }
        }

        double salt;

        public double SAlt
        {
            get { return salt; }
            set { salt = value; }
        }

        float roll;

        public float Roll
        {
            get { return roll; }
            set { roll = value; }
        }

        float pitch;

        public float Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }

        float yaw;

        public float Yaw
        {
            get { return yaw; }
            set { yaw = value; }
        }

        public double getAltitude(bool AMSL, bool gpsalt)
        {
            if (gpsalt)
                return GPSAlt;
            return (AMSL ? AltAMSL : RelAlt);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}