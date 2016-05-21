using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.ExtendedObjects;
using MissionPlanner.HIL;
using ZedGraph;

namespace MissionPlanner.Log
{
    public class DFLogScript : HIL.Utils
    {
        public static Vector3 earth_accel_df(IMU IMU, ATT ATT)
        {
            //return earth frame acceleration vector from df log
            var r = rotation_df(ATT);
            var accel = new Vector3(IMU.AccX, IMU.AccY, IMU.AccZ);
            return r*accel;
        }

        public static Matrix3 rotation_df(ATT ATT)
        {
            //return the current DCM rotation matrix''' 
            var r = new Matrix3();
            r.from_euler(radians(ATT.Roll), radians(ATT.Pitch), radians(ATT.Yaw));
            return r;
        }

        public static Vector3 gps_velocity_df(GPS GPS)
        {
            //return GPS velocity vector
            var vx = GPS.Spd*cos(radians(GPS.GCrs));
            var vy = GPS.Spd*sin(radians(GPS.GCrs));
            return new Vector3(vx, vy, GPS.VZ);
        }

        static double last_v = 0;
        static double last_t = 0;

        public static double delta(double vari, string key, double tusec)
        {
            //calculate slope
            //global last_delta 
            double tnow = 0;
            if (tusec != 0)
            {
                tnow = tusec*1.0e-6;
            }
            double ret = 0.0;

            if (tnow == last_t)
            {
                ret = 0;
            }
            else
            {
                ret = (vari - last_v)/(tnow - last_t);
            }
            last_v = vari;
            last_t = tnow;
            return ret;
        }

        public static PointPairList ProcessExpression(ref DFLog dflog, ref CollectionBuffer<string> logdata, string expression)
        {
            PointPairList answer = new PointPairList();

            //earth_accel_df(IMU2,ATT).x
            if (expression.Contains("earth_accel_df"))
            {
                var matchs = Regex.Matches(expression, @"([A-z0-9_]+),([A-z0-9_]+)");

                List<string> msglist = new List<string>();

                foreach (Match match in matchs)
                {
                    foreach (var item in match.Groups)
                    {
                        msglist.Add(item.ToString());
                    }
                }

                foreach (var item in logdata.GetEnumeratorType(msglist.ToArray()))
                {
                    IMU imu = new IMU();
                    ATT att = new ATT();

                    if (item.msgtype == "ATT")
                    {
                        ATT.Roll = double.Parse(item.items[dflog.FindMessageOffset("ATT", "Roll")]);
                        ATT.Pitch = double.Parse(item.items[dflog.FindMessageOffset("ATT", "Pitch")]);
                        ATT.Yaw = double.Parse(item.items[dflog.FindMessageOffset("ATT", "Yaw")]);
                    } 
                    else if (item.msgtype == "IMU")
                    {
                        IMU.AccX = double.Parse(item.items[dflog.FindMessageOffset("IMU", "AccX")]);
                        IMU.AccY = double.Parse(item.items[dflog.FindMessageOffset("IMU", "AccY")]);
                        IMU.AccZ = double.Parse(item.items[dflog.FindMessageOffset("IMU", "AccZ")]);
                    }
                    else if (item.msgtype == "IMU2")
                    {
                        IMU.AccX = double.Parse(item.items[dflog.FindMessageOffset("IMU2", "AccX")]);
                        IMU.AccY = double.Parse(item.items[dflog.FindMessageOffset("IMU2", "AccY")]);
                        IMU.AccZ = double.Parse(item.items[dflog.FindMessageOffset("IMU2", "AccZ")]);
                    }

                    if (expression.Contains(".x"))
                    {
                        answer.Add(item.lineno, earth_accel_df(imu, att).x);
                    }
                    if (expression.Contains(".y"))
                    {
                        answer.Add(item.lineno,earth_accel_df(imu, att).y);
                    }
                    if (expression.Contains(".z"))
                    {
                        answer.Add(item.lineno,earth_accel_df(imu, att).z);
                    }
                }
            } // delta(gps_velocity_df(GPS).x,'x',GPS.TimeUS)
            else if (expression.Contains("delta(gps_velocity_df(GPS)"))
            {
                foreach (var item in logdata.GetEnumeratorType("GPS"))
                {
                    var gps = new GPS();

                    if (item.msgtype == "GPS")
                    {
                        GPS.Spd = double.Parse(item.items[dflog.FindMessageOffset("GPS", "Spd")]);
                        GPS.GCrs = double.Parse(item.items[dflog.FindMessageOffset("GPS", "GCrs")]);
                        GPS.VZ = double.Parse(item.items[dflog.FindMessageOffset("GPS", "VZ")]);
                    }

                    if (expression.Contains(".x"))
                    {
                        answer.Add(item.lineno,delta(gps_velocity_df(gps).x, "x", item.timems * 1000));
                    }
                    else if (expression.Contains(".y"))
                    {
                        answer.Add(item.lineno,delta(gps_velocity_df(gps).y, "y", item.timems * 1000));
                    }
                    else if (expression.Contains(".z"))
                    {
                        answer.Add(item.lineno, delta(gps_velocity_df(gps).z, "z", item.timems * 1000) - 9.8);
                    }
                }
            }
            else if (expression.StartsWith("degrees"))
            {
                var matchs = Regex.Matches(expression, @"([A-z0-9_]+)\.([A-z0-9_]+)");

                if (matchs.Count > 0)
                {
                    var type = matchs[0].Groups[1].Value.ToString();
                    var field = matchs[0].Groups[2].Value.ToString();

                    foreach (var item in logdata.GetEnumeratorType(type))
                    {
                        answer.Add(item.lineno, degrees(double.Parse(item.items[dflog.FindMessageOffset(type, field)])));
                    }
                }
            }
            else if (expression.StartsWith("sqrt"))
            {
                // there are alot of assumptions made in this code
                Dictionary<int,double> work = new Dictionary<int, double>();
                List<KeyValuePair<string, string>> types = new EventList<KeyValuePair<string, string>>();

                var matchs = Regex.Matches(expression, @"(([A-z0-9_]+)\.([A-z0-9_]+)\*\*2)");

                if (matchs.Count > 0)
                {
                    foreach (Match match in matchs)
                    {
                        var type = match.Groups[2].Value.ToString();
                        var field = match.Groups[3].Value.ToString();

                        types.Add(new KeyValuePair<string, string>(type,field));
                    }

                    List<string> keyarray = new List<string>();
                    types.ForEach(g => { keyarray.Add(g.Key); });
                    List<string> valuearray= new List<string>();
                    types.ForEach(g => { valuearray.Add(g.Value); });

                    foreach (var item in logdata.GetEnumeratorType(keyarray.ToArray()))
                    {
                        for (int a = 0; a < types.Count; a++)
                        {
                            var key = keyarray[a];
                            var value = valuearray[a];
                            var offset = dflog.FindMessageOffset(key, value);
                            if (offset == -1)
                                continue;
                            work[a] = double.Parse(item.items[offset]);
                        }

                        double workanswer = 0;
                        foreach (var value in work.Values)
                        {
                            workanswer += Math.Pow(value, 2);
                        }
                        answer.Add(item.lineno, Math.Sqrt(workanswer));
                    }
                }
            }

            return answer;
        }
    }

    public class ATT
    {
        public static double Roll;
        public static double Pitch;
        public static double Yaw;
    }

    public class IMU
    {
        public static double AccX;
        public static double AccY;
        public static double AccZ;
    }

    public class GPS
    {
        public static double Spd;
        public static double GCrs;
        public static double VZ;
    }
}