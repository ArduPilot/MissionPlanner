using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace MissionPlanner.Utilities
{
    public struct utmpos
    {
        public static readonly utmpos Zero;
        public double x;
        public double y;
        public int zone;
        public object Tag;

        static CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
        static IGeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

        public utmpos(double x, double y, int zone)
        {
            this.x = x;
            this.y = y;
            this.zone = zone;
            this.Tag = null;
        }

        public utmpos(utmpos pos)
        {
            this.x = pos.x;
            this.y = pos.y;
            this.zone = pos.zone;
            this.Tag = null;
        }

        public utmpos(PointLatLngAlt pos)
        {
            double[] dd = pos.ToUTM();
            this.x = dd[0];
            this.y = dd[1];
            this.zone = pos.GetUTMZone();
            this.Tag = null;
        }

        public static implicit operator double[](utmpos a)
        {
            return new double[] { a.x, a.y };
        }

        public static implicit operator PointLatLngAlt(utmpos a)
        {
            return a.ToLLA();
        }

        public PointLatLngAlt ToLLA2()
        {
            GeoUtility.GeoSystem.UTM utm = new GeoUtility.GeoSystem.UTM(Math.Abs(zone), x, y, zone < 0 ? GeoUtility.GeoSystem.Base.Geocentric.Hemisphere.South : GeoUtility.GeoSystem.Base.Geocentric.Hemisphere.North);

            PointLatLngAlt ans = ((GeoUtility.GeoSystem.Geographic)utm);
            if (this.Tag != null)
                ans.Tag = this.Tag.ToString();

            return ans;
        }

        public PointLatLngAlt ToLLA()
        {
            IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(Math.Abs(zone), zone < 0 ? false : true);

            ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

            // get leader utm coords
            double[] pll = trans.MathTransform.Inverse().Transform(this);

            PointLatLngAlt ans = new PointLatLngAlt(pll[1], pll[0]);
            if (this.Tag != null)
                ans.Tag = this.Tag.ToString();

            return ans;
        }

        public static List<utmpos> ToList(List<double[]> input, int zone)
        {
            List<utmpos> data = new List<utmpos>();

            input.ForEach(x => { data.Add(new utmpos(x[0], x[1], zone)); });

            return data;
        }

        public double GetDistance(utmpos b)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(x - b.x), 2) + Math.Pow(Math.Abs(y - b.y), 2));
        }

        public double GetBearing(utmpos b)
        {
            var y = b.y - this.y;
            var x = b.x - this.x;

            return (MathHelper.rad2deg * (Math.Atan2(x, y)) + 360) % 360;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is utmpos))
            {
                return false;
            }
            return (((((utmpos)obj).x == this.x) && (((utmpos)obj).y == this.y)) && obj.GetType().Equals(base.GetType()));
        }

        public static bool operator ==(utmpos left, utmpos right)
        {
            return ((left.x == right.x) && (left.y == right.y) && (left.zone == right.zone));
        }

        public static bool operator !=(utmpos left, utmpos right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return "utmpos: " + x + "," + y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ zone;
                return hashCode;
            }
        }

        public bool IsZero { get { if (this == Zero) return true; return false; } }
    }

}
