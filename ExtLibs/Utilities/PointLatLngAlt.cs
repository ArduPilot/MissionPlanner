using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GMap.NET;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using GeoUtility;
using GeoUtility.GeoSystem;

namespace MissionPlanner.Utilities
{

    public class PointLatLngAlt: IComparable
    {
        public static readonly PointLatLngAlt Zero = new PointLatLngAlt();
        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
        public double Alt { get; set; } = 0;
        public string Tag { get; set; } = "";
        public string Tag2 { get; set; } = "";
        public Color color { get; set; } = Color.White;

        static CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
        static IGeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

        public PointLatLngAlt(double lat, double lng, double alt, string tag)
        {
            this.Lat = lat;
            this.Lng = lng;
            this.Alt = alt;
            this.Tag = tag;
        }

        public PointLatLngAlt()
        {

        }

        public PointLatLngAlt(GMap.NET.PointLatLng pll)
        {
            this.Lat = pll.Lat;
            this.Lng = pll.Lng;
        }

        public PointLatLngAlt(double lat, double lng)
        {
            this.Lat = lat;
            this.Lng = lng;
        }

        public PointLatLngAlt(double lat, double lng, double alt)
        {
            this.Lat = lat;
            this.Lng = lng;
            this.Alt = alt;
        }

        public PointLatLngAlt(Locationwp locwp)
        {
            this.Lat = locwp.lat;
            this.Lng = locwp.lng;
            this.Alt = locwp.alt;
        }

        public PointLatLngAlt(double[] dblarr)
        {
            this.Lat = dblarr[0];
            this.Lng = dblarr[1];
            if (dblarr.Length > 2)
                this.Alt = dblarr[2];
        }

        public PointLatLngAlt(PointLatLngAlt plla)
        {
            this.Lat = plla.Lat;
            this.Lng = plla.Lng;
            this.Alt = plla.Alt;
            this.color = plla.color;
            this.Tag = plla.Tag;
        }

        public PointLatLng Point()
        {
            PointLatLng pnt = new PointLatLng(Lat, Lng);
            return pnt;
        }

        public static implicit operator PointLatLngAlt(PointLatLng a)
        {
            return new PointLatLngAlt(a);
        }

        public static implicit operator PointLatLng(PointLatLngAlt a)
        {
            return a.Point();
        }

        public static implicit operator double[](PointLatLngAlt a)
        {
            return new double[] { a.Lng, a.Lat };
        }

        public static implicit operator PointLatLngAlt(double[] a)
        {
            if (a.Count() == 3)
            {
                return new PointLatLngAlt() { Lng = a[0], Lat = a[1], Alt = a[2]};
            }
            return new PointLatLngAlt() { Lng =  a[0], Lat = a[1] };
        }

        public static implicit operator PointLatLngAlt(GeoUtility.GeoSystem.Geographic geo)
        {
            return new PointLatLngAlt() { Lat = geo.Latitude, Lng = geo.Longitude};
        }        

        public override bool Equals(Object pllaobj)
        {
            PointLatLngAlt plla = pllaobj as PointLatLngAlt;

            if (plla == null)
                return false;

            if (this.Lat == plla.Lat &&
            this.Lng == plla.Lng &&
            this.Alt == plla.Alt &&
            this.color == plla.color &&
            this.Tag == plla.Tag)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int) (BitConverter.DoubleToInt64Bits(Lat) ^
                          BitConverter.DoubleToInt64Bits(Lng) ^
                          BitConverter.DoubleToInt64Bits(Alt));
        }

        public override string ToString()
        {
            return Lat + "," + Lng + "," + Alt + "," + Tag;
        }

        public int GetUTMZone()
        {
            int zone = (int)((Lng - -186.0) / 6.0);
            if (Lat < 0)
                zone *= -1;

            return zone;
        }

        public string GetFriendlyZone()
        {
            return GetUTMZone().ToString("0N;0S");
        }

        public string GetMGRS()
        {
            Geographic geo = new Geographic(Lng, Lat);

            MGRS mgrs = (MGRS)geo;

            return mgrs.ToString();
        }

        public static PointLatLngAlt FromUTM(int zone,double x, double y)
        {
            GeoUtility.GeoSystem.UTM utm = new GeoUtility.GeoSystem.UTM(Math.Abs(zone), x, y, zone < 0 ? GeoUtility.GeoSystem.Base.Geocentric.Hemisphere.South : GeoUtility.GeoSystem.Base.Geocentric.Hemisphere.North);

            PointLatLngAlt ans = ((GeoUtility.GeoSystem.Geographic)utm);

            return ans;
        }

        public double[] ToUTM()
        {
           return ToUTM(GetUTMZone());
        }

        // force a zone
        public double[] ToUTM(int utmzone)
        {
            IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(Math.Abs(utmzone), Lat < 0 ? false : true);

            ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

            double[] pll = { Lng, Lat };

            // get leader utm coords
            double[] utmxy = trans.MathTransform.Transform(pll);

            return utmxy;
        }

        public static List<double[]> ToUTM(int utmzone, List<PointLatLngAlt> list)
        {
            IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(Math.Abs(utmzone), list[0].Lat < 0 ? false : true);

            ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

            List<double[]> data = new List<double[]>();

            list.ForEach(x => { data.Add((double[])x); });

            return trans.MathTransform.TransformList(data).ToList();
        }


        public PointLatLngAlt newpos(double bearing, double distance)
        {
            // '''extrapolate latitude/longitude given a heading and distance 
            //   thanks to http://www.movable-type.co.uk/scripts/latlong.html
            //  '''
            // from math import sin, asin, cos, atan2, radians, degrees
            double radius_of_earth = 6378100.0;//# in meters

            double lat1 = MathHelper.deg2rad * (this.Lat);
            double lon1 = MathHelper.deg2rad * (this.Lng);
            double brng = MathHelper.deg2rad * (bearing);
            double dr = distance / radius_of_earth;

            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dr) +
                        Math.Cos(lat1) * Math.Sin(dr) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dr) * Math.Cos(lat1),
                                Math.Cos(dr) - Math.Sin(lat1) * Math.Sin(lat2));

            double latout = MathHelper.rad2deg * (lat2);
            double lngout = MathHelper.rad2deg * (lon2);

            return new PointLatLngAlt(latout, lngout, this.Alt, this.Tag);
        }

        /// <summary>
        /// move a point a specific number of meters
        /// </summary>
        /// <param name="east"></param>
        /// <param name="north"></param>
        /// <returns></returns>
        public PointLatLngAlt gps_offset(double east, double north)
        {
            double bearing = Math.Atan2(east, north) * MathHelper.rad2deg;
            double distance = Math.Sqrt(Math.Pow(east, 2) + Math.Pow(north, 2));
            return newpos(bearing, distance);
        }

        public double GetBearing(PointLatLngAlt p2)
        {
            var latitude1 = MathHelper.deg2rad * (this.Lat);
            var latitude2 = MathHelper.deg2rad * (p2.Lat);
            var longitudeDifference = MathHelper.deg2rad * (p2.Lng - this.Lng);

            var y = Math.Sin(longitudeDifference) * Math.Cos(latitude2);
            var x = Math.Cos(latitude1) * Math.Sin(latitude2) - Math.Sin(latitude1) * Math.Cos(latitude2) * Math.Cos(longitudeDifference);

            return (MathHelper.rad2deg * (Math.Atan2(y, x)) + 360) % 360;
        }

        /// <summary>
        /// Calc Distance in M
        /// </summary>
        /// <param name="p2"></param>
        /// <returns>Distance in M</returns>
        public double GetDistance(PointLatLngAlt p2)
        {
            double d = Lat * 0.017453292519943295;
            double num2 = Lng * 0.017453292519943295;
            double num3 = p2.Lat * 0.017453292519943295;
            double num4 = p2.Lng * 0.017453292519943295;
            double num5 = num4 - num2;
            double num6 = num3 - d;
            double num7 = Math.Pow(Math.Sin(num6 / 2.0), 2.0) + ((Math.Cos(d) * Math.Cos(num3)) * Math.Pow(Math.Sin(num5 / 2.0), 2.0));
            double num8 = 2.0 * Math.Atan2(Math.Sqrt(num7), Math.Sqrt(1.0 - num7));
            return (6371 * num8) * 1000.0; // M
        }

        public double GetDistance2(PointLatLngAlt p2)
        {
            //http://www.movable-type.co.uk/scripts/latlong.html
            var R = 6371.0; // 6371 km
            var dLat = (p2.Lat - Lat) * MathHelper.deg2rad;
            var dLon = (p2.Lng - Lng) * MathHelper.deg2rad;
            var lat1 = Lat * MathHelper.deg2rad;
            var lat2 = p2.Lat * MathHelper.deg2rad;

            var a = Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0) +
                    Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            var d = R * c * 1000.0; // M

            return d;
        }

        public int CompareTo(object obj)
        {
            PointLatLngAlt pnt = obj as PointLatLngAlt;

            if (pnt == null)
                return 1;

            double wpno = 0;
            double wpnothis = 0;

            if (!double.TryParse(this.Tag, out wpnothis))
            {
                return 0;
            }

            if (double.TryParse(pnt.Tag, out wpno))
            {
                if (wpno < wpnothis)
                    return 1;
                if (wpno > wpnothis)
                    return -1;
                return 0;
            } 
            else 
            {
                return 0;
            }
        }
    }

}
