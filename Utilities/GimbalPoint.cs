using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    class GimbalPoint
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

        /// returns the angle (degrees*100) that the RC_Channel input is receiving
        static float angle_input(bool rev, float radio_in, float radio_min, float radio_max, float angle_min, float angle_max)
        {
            return (rev ? -1.0f : 1.0f) * (radio_in - radio_min) * (angle_max - angle_min) / (radio_max - radio_min) + (rev ? angle_max : angle_min);
        }

        static int channelpwm(int channel)
        {
            if (channel == 1)
                return (int)(float)MainV2.comPort.MAV.cs.ch1out;
            if (channel == 2)
                return (int)(float)MainV2.comPort.MAV.cs.ch2out;
            if (channel == 3)
                return (int)(float)MainV2.comPort.MAV.cs.ch3out;
            if (channel == 4)
                return (int)(float)MainV2.comPort.MAV.cs.ch4out;
            if (channel == 5)
                return (int)(float)MainV2.comPort.MAV.cs.ch5out;
            if (channel == 6)
                return (int)(float)MainV2.comPort.MAV.cs.ch6out;
            if (channel == 7)
                return (int)(float)MainV2.comPort.MAV.cs.ch7out;
            if (channel == 8)
                return (int)(float)MainV2.comPort.MAV.cs.ch8out;

            return 0;
        }

        public static double ConvertPwmtoAngle(axis axis)
        {
            int pwmvalue =-1;

            if (!MainV2.comPort.MAV.param.ContainsKey("RC" + yawchannel + "_MIN"))
                return 0;

            switch (axis)
            {
                case GimbalPoint.axis.roll:
                    pwmvalue = channelpwm(rollchannel);
                    float minr = (float)MainV2.comPort.MAV.param["RC" + rollchannel + "_MIN"];
                    float maxr = (float)MainV2.comPort.MAV.param["RC" + rollchannel + "_MAX"];
                    float minroll = (float)MainV2.comPort.MAV.param["MNT_ANGMIN_ROL"];
                    float maxroll = (float)MainV2.comPort.MAV.param["MNT_ANGMAX_ROL"];
                    float revr = (float)MainV2.comPort.MAV.param["RC" + rollchannel + "_REV"];

                    return angle_input(revr != 1,pwmvalue,minr,maxr,minroll,maxroll)/100.0;
                    
                case GimbalPoint.axis.pitch:
                    pwmvalue = channelpwm(pitchchannel);
                    float minp = (float)MainV2.comPort.MAV.param["RC" + pitchchannel + "_MIN"];
                    float maxp = (float)MainV2.comPort.MAV.param["RC" + pitchchannel + "_MAX"];
                    float minpitch = (float)MainV2.comPort.MAV.param["MNT_ANGMIN_TIL"];
                    float maxpitch = (float)MainV2.comPort.MAV.param["MNT_ANGMAX_TIL"];
                    float revp = (float)MainV2.comPort.MAV.param["RC" + pitchchannel + "_REV"];


                    return angle_input(revp != 1, pwmvalue, minp, maxp, minpitch, maxpitch) / 100.0;
                    
                case GimbalPoint.axis.yaw:
                    pwmvalue = channelpwm(yawchannel);
                    float miny = (float)MainV2.comPort.MAV.param["RC" + yawchannel + "_MIN"];
                    float maxy = (float)MainV2.comPort.MAV.param["RC" + yawchannel + "_MAX"];
                    float minyaw = (float)MainV2.comPort.MAV.param["MNT_ANGMIN_PAN"];
                    float maxyaw = (float)MainV2.comPort.MAV.param["MNT_ANGMAX_PAN"];
                    float revy = (float)MainV2.comPort.MAV.param["RC" + yawchannel + "_REV"];


                    return angle_input(revy != 1, pwmvalue, miny, maxy, minyaw, maxyaw) / 100.0;
                    
            }

            return 0;
        }

        public const double rad2deg = (180 / System.Math.PI);
        public const double deg2rad = (1.0 / rad2deg);

        public static PointLatLngAlt ProjectPoint()
        {
            //MainV2.comPort.GetMountStatus();

            yawchannel =  (int)(float)MainV2.comPort.MAV.param["MNT_RC_IN_PAN"];

            pitchchannel = (int)(float)MainV2.comPort.MAV.param["MNT_RC_IN_TILT"];

            rollchannel = (int)(float)MainV2.comPort.MAV.param["MNT_RC_IN_ROLL"];

            if (!MainV2.comPort.BaseStream.IsOpen)
                return PointLatLngAlt.Zero;

            PointLatLngAlt currentlocation = new PointLatLngAlt(MainV2.comPort.MAV.cs.lat, MainV2.comPort.MAV.cs.lng);

            double yawangle = MainV2.comPort.MAV.cs.campointc * 0.01f;
            double rollangle = MainV2.comPort.MAV.cs.campointb * 0.01f;
            double pitchangle = MainV2.comPort.MAV.cs.campointa * 0.01f;

            yawangle = ConvertPwmtoAngle(axis.yaw);
            //rollangle = ConvertPwmtoAngle(axis.roll);
            pitchangle = ConvertPwmtoAngle(axis.pitch) + MainV2.comPort.MAV.cs.pitch;

            pitchangle -= Math.Sin(yawangle * deg2rad) * MainV2.comPort.MAV.cs.roll;

            // tan (o/a)
            double dist = Math.Tan((90 +pitchangle)* deg2rad) * (MainV2.comPort.MAV.cs.alt);

            if (dist > 9999)
                return PointLatLngAlt.Zero;

            //Console.WriteLine("pitch " + pitchangle.ToString("0.000") + " yaw " + yawangle.ToString("0.000") + " dist" + dist.ToString("0.000"));

            PointLatLngAlt newpos = currentlocation.newpos( yawangle + MainV2.comPort.MAV.cs.yaw, dist);

            //Console.WriteLine(newpos);
            return newpos;
        }

        public void test()
        {

        }
    }
}
