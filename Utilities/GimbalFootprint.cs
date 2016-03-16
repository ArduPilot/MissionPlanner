using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    class GimbalFootprint
    {
        public static PointF FindLineIntersection(PointF start1, PointF end1, PointF start2, PointF end2)
        {
            double denom = ((end1.X - start1.X) * (end2.Y - start2.Y)) - ((end1.Y - start1.Y) * (end2.X - start2.X));
            //  AB & CD are parallel         
            if (denom == 0)
                return new PointF();
            double numer = ((start1.Y - start2.Y) * (end2.X - start2.X)) - ((start1.X - start2.X) * (end2.Y - start2.Y));
            double r = numer / denom;
            double numer2 = ((start1.Y - start2.Y) * (end1.X - start1.X)) - ((start1.X - start2.X) * (end1.Y - start1.Y));
            double s = numer2 / denom;
            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return new PointF();
            // Find intersection point      
            PointF result = new PointF();
            result.X = (float)(start1.X + (r * (end1.X - start1.X)));
            result.Y = (float)(start1.Y + (r * (end1.Y - start1.Y)));
            return result;
        }

        public struct Rect
        {
            public PointLatLngAlt tl;
            public PointLatLngAlt tr;
            public PointLatLngAlt bl;
            public PointLatLngAlt br;

        }

        public const double rad2deg = (180 / System.Math.PI);
        public const double deg2rad = (1.0 / rad2deg);

        public static PointLatLngAlt ProjectPoint()
        {
            PointLatLngAlt currentlocation = new PointLatLngAlt(MainV2.comPort.MAV.cs.lat, MainV2.comPort.MAV.cs.lng);

            double yawangle = MainV2.comPort.MAV.cs.campointc;
            double rollangle = MainV2.comPort.MAV.cs.campointb;
            double pitchangle = MainV2.comPort.MAV.cs.campointa;

            if ((float) MainV2.comPort.MAV.param["MNT_TYPE"] == 4)
            {
                yawangle = MainV2.comPort.MAVlist[71].cs.yaw;
                rollangle = MainV2.comPort.MAVlist[71].cs.roll;
                pitchangle = MainV2.comPort.MAVlist[71].cs.pitch;
            }

            int distout = 10;
            PointLatLngAlt newpos = PointLatLngAlt.Zero;

            //dist = Math.Tan((90 + pitchangle) * deg2rad) * (MainV2.comPort.MAV.cs.alt);

            while (distout < 1000)
            {
                // get a projected point to test intersection against - not using slope distance
                PointLatLngAlt newposdist = currentlocation.newpos(yawangle + MainV2.comPort.MAV.cs.yaw, distout);
                newposdist.Alt = srtm.getAltitude(newposdist.Lat, newposdist.Lng).alt;

                // get another point 50 infront
                PointLatLngAlt newposdist2 = currentlocation.newpos(yawangle + MainV2.comPort.MAV.cs.yaw, distout + 50);
                newposdist2.Alt = srtm.getAltitude(newposdist2.Lat, newposdist2.Lng).alt;

                // get the flat terrain distance out - at 0 alt
                double distflat = Math.Tan((90 + pitchangle) * deg2rad) * (MainV2.comPort.MAV.cs.altasl);

                // x is dist from plane, y is alt
                var newpoint = FindLineIntersection(new PointF(0,MainV2.comPort.MAV.cs.altasl), new PointF((float)distflat, 0),
                    new PointF((float)distout, (float)newposdist.Alt), new PointF((float)distout + 50, (float)newposdist2.Alt));

                if (newpoint.X != 0)
                {
                    newpos = newposdist2;
                    break;
                }

                distout += 50;
            }

            //Console.WriteLine("pitch " + pitchangle.ToString("0.000") + " yaw " + yawangle.ToString("0.000") + " dist" + dist.ToString("0.000"));

            //PointLatLngAlt newpos = currentlocation.newpos( yawangle + MainV2.comPort.MAV.cs.yaw, dist);

            //Console.WriteLine(newpos);
            return newpos;
        }
    }
}
