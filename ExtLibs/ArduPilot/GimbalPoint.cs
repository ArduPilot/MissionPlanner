using System;

namespace MissionPlanner.Utilities
{
    public class GimbalPoint
    {
        public static int yawchannel = 7;
        public static int pitchchannel = 5;
        public static int rollchannel = -1;

        public enum axis
        {
            roll,
            pitch,
            yaw
        }

        public static MAVLinkInterface comPort { get; set; }

        /// returns the angle (degrees*100) that the RC_Channel input is receiving
        static float angle_input(bool rev, float radio_in, float radio_min, float radio_max, float angle_min,
            float angle_max)
        {
            return (rev ? -1.0f : 1.0f)*(radio_in - radio_min)*(angle_max - angle_min)/(radio_max - radio_min) +
                   (rev ? angle_max : angle_min);
        }

        static int channelpwm(int channel)
        {
            if (channel == 1)
                return (int) (float) comPort.MAV.cs.ch1out;
            if (channel == 2)
                return (int) (float) comPort.MAV.cs.ch2out;
            if (channel == 3)
                return (int) (float) comPort.MAV.cs.ch3out;
            if (channel == 4)
                return (int) (float) comPort.MAV.cs.ch4out;
            if (channel == 5)
                return (int) (float) comPort.MAV.cs.ch5out;
            if (channel == 6)
                return (int) (float) comPort.MAV.cs.ch6out;
            if (channel == 7)
                return (int) (float) comPort.MAV.cs.ch7out;
            if (channel == 8)
                return (int) (float) comPort.MAV.cs.ch8out;

            return 0;
        }

        public static double ConvertPwmtoAngle(axis axis)
        {
            int pwmvalue = -1;

            if (!comPort.MAV.param.ContainsKey("RC" + yawchannel + "_MIN"))
                return 0;

            switch (axis)
            {
                case GimbalPoint.axis.roll:
                    pwmvalue = channelpwm(rollchannel);
                    float minr = (float) comPort.MAV.param["RC" + rollchannel + "_MIN"];
                    float maxr = (float) comPort.MAV.param["RC" + rollchannel + "_MAX"];
                    float minroll = (float) comPort.MAV.param["MNT_ANGMIN_ROL"];
                    float maxroll = (float) comPort.MAV.param["MNT_ANGMAX_ROL"];
                    float revr = (float) comPort.MAV.param["RC" + rollchannel + "_REV"];

                    return angle_input(revr != 1, pwmvalue, minr, maxr, minroll, maxroll)/100.0;

                case GimbalPoint.axis.pitch:
                    pwmvalue = channelpwm(pitchchannel);
                    float minp = (float) comPort.MAV.param["RC" + pitchchannel + "_MIN"];
                    float maxp = (float) comPort.MAV.param["RC" + pitchchannel + "_MAX"];
                    float minpitch = (float) comPort.MAV.param["MNT_ANGMIN_TIL"];
                    float maxpitch = (float) comPort.MAV.param["MNT_ANGMAX_TIL"];
                    float revp = (float) comPort.MAV.param["RC" + pitchchannel + "_REV"];


                    return angle_input(revp != 1, pwmvalue, minp, maxp, minpitch, maxpitch)/100.0;

                case GimbalPoint.axis.yaw:
                    pwmvalue = channelpwm(yawchannel);
                    float miny = (float) comPort.MAV.param["RC" + yawchannel + "_MIN"];
                    float maxy = (float) comPort.MAV.param["RC" + yawchannel + "_MAX"];
                    float minyaw = (float) comPort.MAV.param["MNT_ANGMIN_PAN"];
                    float maxyaw = (float) comPort.MAV.param["MNT_ANGMAX_PAN"];
                    float revy = (float) comPort.MAV.param["RC" + yawchannel + "_REV"];

                    return angle_input(revy != 1, pwmvalue, miny, maxy, minyaw, maxyaw)/100.0;
            }

            return 0;
        }

        public static Point FindLineIntersection(Point start1, Point end1, Point start2, Point end2)
        {
            double denom = ((end1.X - start1.X)*(end2.Y - start2.Y)) - ((end1.Y - start1.Y)*(end2.X - start2.X));
            //  AB & CD are parallel         
            if (denom == 0)
                return new Point();
            double numer = ((start1.Y - start2.Y)*(end2.X - start2.X)) - ((start1.X - start2.X)*(end2.Y - start2.Y));
            double r = numer/denom;
            double numer2 = ((start1.Y - start2.Y)*(end1.X - start1.X)) - ((start1.X - start2.X)*(end1.Y - start1.Y));
            double s = numer2/denom;
            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return new Point();
            // Find intersection point      
            Point result = new Point();
            result.X = (float) (start1.X + (r*(end1.X - start1.X)));
            result.Y = (float) (start1.Y + (r*(end1.Y - start1.Y)));
            return result;
        }

        public static PointLatLngAlt ProjectPoint(MAVLinkInterface comPort)
        {
            GimbalPoint.comPort = comPort;
            comPort.GetMountStatus();

            // this should be looking at rc_channel function
            yawchannel = (int) (float) comPort.MAV.param["MNT_RC_IN_PAN"];

            pitchchannel = (int) (float) comPort.MAV.param["MNT_RC_IN_TILT"];

            rollchannel = (int) (float) comPort.MAV.param["MNT_RC_IN_ROLL"];

            //if (!comPort.BaseStream.IsOpen)
            //  return PointLatLngAlt.Zero;

            PointLatLngAlt currentlocation = new PointLatLngAlt(comPort.MAV.cs.lat, comPort.MAV.cs.lng);

            double yawangle = comPort.MAV.cs.campointc;
            double rollangle = comPort.MAV.cs.campointb;
            double pitchangle = comPort.MAV.cs.campointa;

            //
            if ((double) comPort.MAV.param["MNT_TYPE"] == 4)
            {
                yawangle = comPort.MAVlist[comPort.sysidcurrent, 67].cs.yaw;
                rollangle = comPort.MAVlist[comPort.sysidcurrent, 67].cs.roll;
                pitchangle = comPort.MAVlist[comPort.sysidcurrent, 67].cs.pitch;
            }

            if (Math.Abs(rollangle) > 180 || yawangle == 0 && pitchangle == 0)
            {
                yawangle = ConvertPwmtoAngle(axis.yaw);
                //rollangle = ConvertPwmtoAngle(axis.roll);
                pitchangle = ConvertPwmtoAngle(axis.pitch) + comPort.MAV.cs.pitch;

                pitchangle -= Math.Sin(yawangle*MathHelper.deg2rad)*comPort.MAV.cs.roll;
            }

            // tan (o/a)
            // todo account for ground elevation change.

            int distout = 10;
            PointLatLngAlt newpos = PointLatLngAlt.Zero;

            //dist = Math.Tan((90 + pitchangle) * MathHelper.deg2rad) * (comPort.MAV.cs.alt);

            while (distout < 1000)
            {
                // get a projected point to test intersection against - not using slope distance
                PointLatLngAlt newposdist = currentlocation.newpos(yawangle + comPort.MAV.cs.yaw, distout);
                newposdist.Alt = srtm.getAltitude(newposdist.Lat, newposdist.Lng).alt;

                // get another point 50 infront
                PointLatLngAlt newposdist2 = currentlocation.newpos(yawangle + comPort.MAV.cs.yaw, distout + 50);
                newposdist2.Alt = srtm.getAltitude(newposdist2.Lat, newposdist2.Lng).alt;

                // get the flat terrain distance out - at 0 alt
                double distflat = Math.Tan((90 + pitchangle)*MathHelper.deg2rad)*(comPort.MAV.cs.altasl);

                // x is dist from plane, y is alt
                var newpoint = FindLineIntersection(start1: new Point(0, comPort.MAV.cs.altasl),
                    new Point((float) distflat, 0),
                    new Point((float) distout, (float) newposdist.Alt),
                    new Point((float) distout + 50, (float) newposdist2.Alt));

                if (newpoint.X != 0)
                {
                    newpos = newposdist2;
                    break;
                }

                distout += 50;
            }

            //Console.WriteLine("pitch " + pitchangle.ToString("0.000") + " yaw " + yawangle.ToString("0.000") + " dist" + dist.ToString("0.000"));

            //PointLatLngAlt newpos = currentlocation.newpos( yawangle + comPort.MAV.cs.yaw, dist);

            //Console.WriteLine(newpos);
            return newpos;
        }

        public class Point
        {
            public float X { get; set; }
            public float Y { get; set; }

            public Point(float X = 0, float Y = 0)
            {
                this.X = X;
                this.Y = Y;
            }
        }
    }
}