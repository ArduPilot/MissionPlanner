﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MissionPlanner.Utilities;
using org.mariuszgromada.math.mxparser;
using org.mariuszgromada.math.mxparser.mathcollection;

namespace MissionPlanner.Log
{
    public class DFLogScript : Utils
    {
        public static Vector3 earth_accel_df(IMU_t IMU, ATT_t ATT)
        {
            //return earth frame acceleration vector from df log
            var r = rotation_df(ATT);
            var accel = new Vector3(IMU.AccX, IMU.AccY, IMU.AccZ);
            return r*accel;
        }

        public static Matrix3 rotation_df(ATT_t ATT)
        {
            //return the current DCM rotation matrix''' 
            var r = new Matrix3();
            r.from_euler(radians(ATT.Roll), radians(ATT.Pitch), radians(ATT.Yaw));
            return r;
        }

        public static Vector3 gps_velocity_df(GPS_t GPS)
        {
            //return GPS velocity vector
            var vx = GPS.Spd*cos(radians(GPS.GCrs));
            var vy = GPS.Spd*sin(radians(GPS.GCrs));
            return new Vector3(vx, vy, GPS.VZ);
        }

        public static double mag_heading_df(MAG_t MAG, ATT_t ATT, double declination = 0, Vector3 ofs = null,
            Vector3 diagonals = null, Vector3 offdiagonals = null)
        {
            if (diagonals == null)
                diagonals = Vector3.One;
            if (offdiagonals == null)
                offdiagonals = Vector3.Zero;
            //'''calculate heading from raw magnetometer'''
            //if (declination == 0)
            //declination = degrees(mavutil.mavfile_global.param('COMPASS_DEC', 0));
            var mag = new Vector3(MAG.MagX, MAG.MagY, MAG.MagZ);
            if (ofs != null)
            {
                mag += new Vector3(ofs[0], ofs[1], ofs[2]) - new Vector3(MAG.OfsX, MAG.OfsY, MAG.OfsZ);
                diagonals = new Vector3(diagonals[0], diagonals[1], diagonals[2]);
                offdiagonals = new Vector3(offdiagonals[0], offdiagonals[1], offdiagonals[2]);
                var rot = new Matrix3(new Vector3(diagonals.x, offdiagonals.x, offdiagonals.y),
                    new Vector3(offdiagonals.x, diagonals.y, offdiagonals.z),
                    new Vector3(offdiagonals.y, offdiagonals.z, diagonals.z));
                mag = rot*mag;
            }
//# go via a DCM matrix to match the APM calculation
            var dcm_matrix = rotation_df(ATT);
            var cos_pitch_sq = 1.0 - (dcm_matrix.c.x*dcm_matrix.c.x);
            var headY = mag.y*dcm_matrix.c.z - mag.z*dcm_matrix.c.y;
            var headX = mag.x*cos_pitch_sq - dcm_matrix.c.x*(mag.y*dcm_matrix.c.y + mag.z*dcm_matrix.c.z);

            var heading = degrees(atan2(-headY, headX)) + declination;
            if (heading < 0)
                heading += 360;
            return heading;
        }

        private static double last_v = 0;
        private static double last_t = 0;

        public static double delta(double vari, string key, double tusec)
        {
            //calculate slope
            //global last_delta 
            double tnow = 0;
            if (tusec != 0)
            {
                tnow = tusec * 1.0e-6;
            }
            double ret = 0.0;

            if (tnow == last_t)
            {
                ret = 0;
            }
            else
            {
                ret = (vari - last_v) / (tnow - last_t);
            }
            last_v = vari;
            last_t = tnow;
            return ret;
        }

        public class atan2 : FunctionExtension
        {
            private double x;
            private double y;
            public atan2()
            {
                x = Double.NaN;
                y = Double.NaN;
            }
            public atan2(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
            public double calculate()
            {
                return Math.Atan2(y, x);
            }

            public FunctionExtension clone()
            {
                return new atan2(x, y);
            }

            public string getParameterName(int argumentIndex)
            {
                if (argumentIndex == 0) return "y";
                if (argumentIndex == 1) return "x";
                return "";
            }

            public int getParametersNumber()
            {
                return 2;
            }

            public void setParameterValue(int argumentIndex, double argumentValue)
            {
                if (argumentIndex == 0) x = argumentValue;
                if (argumentIndex == 1) y = argumentValue;
            }
        }

        public static List<Tuple<DFLog.DFItem, double>> ProcessExpression(DFLog dflog, DFLogBuffer logdata, string expression)
        {
            List<Tuple<DFLog.DFItem, double>> answer = new List<Tuple<DFLog.DFItem, double>>();

            Dictionary<string, List<string>> fieldsUsed = new Dictionary<string, List<string>>();

            var fieldmatchs = Regex.Matches(expression, @"(([A-z0-9_]{2,20})\.([A-z0-9_]+))");

            if (fieldmatchs.Count > 0)
            {
                foreach (Match match in fieldmatchs)
                {
                    var type = match.Groups[2].Value.ToString();
                    var field = match.Groups[3].Value.ToString();

                    if (!fieldsUsed.ContainsKey(type))
                        fieldsUsed[type] = new List<string>();

                    fieldsUsed[type].Add(field);
                }
            }

            Function f = new Function("wrap_360(x) = (x+360) # 360");
            Function f1 = new Function("degrees(x) = x*57.295779513");
            Function f2 = new Function("atan2", new atan2());
            Function f3 = new Function("lowpass", new lowpass());
            Function f4 = new Function("delta", new deltaF());

            // fix maths operators
            var filter1 =
                expression.Replace("%", "#").Replace("**", "^").Replace(":2", "");

            //convert paramnames to remove .
            var filter2 = Regex.Replace(filter1, @"(([A-z0-9_]{2,20})\.([A-z0-9_]+))",
                match => match.Groups[2].ToString() + match.Groups[3]);

            // convert strings to long
            var filter3 = Regex.Replace(filter2, @"([""']{1}[^""]+[""']{1})",
                match => BitConverter.ToUInt64(match.Groups[0].Value.MakeBytesSize(8), 0).ToString());

            Expression e = new Expression(filter3);
            
            e.addFunctions(f);
            e.addFunctions(f1);
            e.addFunctions(f2);
            e.addFunctions(f3);
            e.addFunctions(f4);

            foreach (var item in fieldsUsed)
            {
                foreach (var value in item.Value)
                {
                    Argument x = new Argument(item.Key + "" + value);
                    e.addArguments(x);
                }
            }

            bool bad = false;

            try
            {
                mXparser.consolePrintTokens(e.getCopyOfInitialTokens());

                if (!e.checkSyntax())
                {
                    bad = true;
                }
            }
            catch
            {
                bad = true;
            }

            if (!bad)
                foreach (var line in logdata.GetEnumeratorType(fieldsUsed.Keys.ToArray()))
                {
                    foreach (var item in fieldsUsed)
                    {
                        foreach (var value in item.Value.Distinct())
                        {
                            e.setArgumentValue(item.Key + "" + value,
                                double.Parse(line.items[dflog.FindMessageOffset(item.Key, value)]));
                        }
                    }

                    answer.Add(line, e.calculate());
                }

            if (answer.Count > 0)
                return answer;

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

                IMU_t IMU = new IMU_t();
                ATT_t ATT = new ATT_t();

                foreach (var item in logdata.GetEnumeratorType(msglist.ToArray()))
                {
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
                        answer.Add(item, earth_accel_df(IMU, ATT).x);
                    }
                    if (expression.Contains(".y"))
                    {
                        answer.Add(item, earth_accel_df(IMU, ATT).y);
                    }
                    if (expression.Contains(".z"))
                    {
                        answer.Add(item, earth_accel_df(IMU, ATT).z);
                    }
                }
            } // delta(gps_velocity_df(GPS).x,'x',GPS.TimeUS)
            else if (expression.Contains("delta(gps_velocity_df(GPS)"))
            {
                foreach (var item in logdata.GetEnumeratorType("GPS"))
                {
                    var GPS = new GPS_t();

                    if (item.msgtype == "GPS")
                    {
                        GPS.Spd = double.Parse(item.items[dflog.FindMessageOffset("GPS", "Spd")]);
                        GPS.GCrs = double.Parse(item.items[dflog.FindMessageOffset("GPS", "GCrs")]);
                        GPS.VZ = double.Parse(item.items[dflog.FindMessageOffset("GPS", "VZ")]);
                    }

                    if (expression.Contains(".x"))
                    {
                        answer.Add(item, delta(gps_velocity_df(GPS).x, "x", item.timems*1000));
                    }
                    else if (expression.Contains(".y"))
                    {
                        answer.Add(item, delta(gps_velocity_df(GPS).y, "y", item.timems*1000));
                    }
                    else if (expression.Contains(".z"))
                    {
                        answer.Add(item, delta(gps_velocity_df(GPS).z, "z", item.timems*1000) - 9.8);
                    }
                }
            }
            else if (expression.Contains("delta(gps_velocity_df(GPS2)"))
            {
                foreach (var item in logdata.GetEnumeratorType("GPS2"))
                {
                    var GPS = new GPS_t();

                    if (item.msgtype == "GPS2")
                    {
                        GPS.Spd = double.Parse(item.items[dflog.FindMessageOffset("GPS2", "Spd")]);
                        GPS.GCrs = double.Parse(item.items[dflog.FindMessageOffset("GPS2", "GCrs")]);
                        GPS.VZ = double.Parse(item.items[dflog.FindMessageOffset("GPS2", "VZ")]);
                    }

                    if (expression.Contains(".x"))
                    {
                        answer.Add(item, delta(gps_velocity_df(GPS).x, "x", item.timems*1000));
                    }
                    else if (expression.Contains(".y"))
                    {
                        answer.Add(item, delta(gps_velocity_df(GPS).y, "y", item.timems*1000));
                    }
                    else if (expression.Contains(".z"))
                    {
                        answer.Add(item, delta(gps_velocity_df(GPS).z, "z", item.timems*1000) - 9.8);
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
                        answer.Add(item, degrees(double.Parse(item.items[dflog.FindMessageOffset(type, field)])));
                    }
                }
            }
            else if (expression.StartsWith("sqrt"))
            {
                // there are alot of assumptions made in this code
                Dictionary<int, double> work = new Dictionary<int, double>();
                List<KeyValuePair<string, string>> types = new List<KeyValuePair<string, string>>();

                var matchs = Regex.Matches(expression, @"(([A-z0-9_]+)\.([A-z0-9_]+)\*\*2)");

                if (matchs.Count > 0)
                {
                    foreach (Match match in matchs)
                    {
                        var type = match.Groups[2].Value.ToString();
                        var field = match.Groups[3].Value.ToString();

                        types.Add(new KeyValuePair<string, string>(type, field));
                    }

                    List<string> keyarray = new List<string>();
                    types.ForEach(g => { keyarray.Add(g.Key); });
                    List<string> valuearray = new List<string>();
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
                            var ans = logdata.GetUnit(key, value);
                            string unit = ans.Item1;
                            double multiplier = ans.Item2;
                            work[a] = double.Parse(item.items[offset]) * multiplier;
                        }



                        double workanswer = 0;
                        foreach (var value in work.Values)
                        {
                            workanswer += Math.Pow(value, 2);
                        }
                        answer.Add(item, Math.Sqrt(workanswer));
                    }
                }
            }
            else if (expression.Contains("*")) // ATT.DesRoll*ATT.Roll
            {
                var matchs = Regex.Matches(expression, @"([A-z0-9_]+)\.([A-z0-9_]+)\*([A-z0-9_]+)\.([A-z0-9_]+)");

                if (matchs.Count > 0)
                {
                    var type = matchs[0].Groups[1].Value.ToString();
                    var field = matchs[0].Groups[2].Value.ToString();

                    var type2 = matchs[0].Groups[3].Value.ToString();
                    var field2 = matchs[0].Groups[4].Value.ToString();

                    foreach (var item in logdata.GetEnumeratorType(new[] { type, type2 }))
                    {
                        if (type == type2)
                        {
                            var idx1 = dflog.FindMessageOffset(type, field);
                            var idx2 = dflog.FindMessageOffset(type2, field2);
                            if (idx1 == -1 || idx2 == -1)
                                break;
                            answer.Add(item,
                                double.Parse(item.items[idx1]) *
                                double.Parse(item.items[idx2]));
                        }
                    }
                }
            }
            else if (expression.Contains("-")) // ATT.DesRoll-ATT.Roll
            {
                var matchs = Regex.Matches(expression, @"([A-z0-9_]+)\.([A-z0-9_]+)-([A-z0-9_]+)\.([A-z0-9_]+)");

                if (matchs.Count > 0)
                {
                    var type = matchs[0].Groups[1].Value.ToString();
                    var field = matchs[0].Groups[2].Value.ToString();

                    var type2 = matchs[0].Groups[3].Value.ToString();
                    var field2 = matchs[0].Groups[4].Value.ToString();

                    foreach (var item in logdata.GetEnumeratorType(new[] {type, type2}))
                    {
                        if (type == type2)
                        {
                            var idx1 = dflog.FindMessageOffset(type, field);
                            var idx2 = dflog.FindMessageOffset(type2, field2);
                            if(idx1 == -1 || idx2 == -1)
                                break;
                            answer.Add(item,
                                double.Parse(item.items[idx1]) -
                                double.Parse(item.items[idx2]));
                        }
                    }
                }
            }
            else if (expression.Contains("mag_heading_df"))
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

                var MAG = new MAG_t();
                var ATT = new ATT_t();

                foreach (var item in logdata.GetEnumeratorType(msglist.ToArray()))
                {
                    if (item.msgtype.StartsWith("MAG"))
                    {
                        MAG.MagX = double.Parse(item.items[dflog.FindMessageOffset(item.msgtype, "MagX")]);
                        MAG.MagY = double.Parse(item.items[dflog.FindMessageOffset(item.msgtype, "MagY")]);
                        MAG.MagZ = double.Parse(item.items[dflog.FindMessageOffset(item.msgtype, "MagZ")]);
                        MAG.OfsX = double.Parse(item.items[dflog.FindMessageOffset(item.msgtype, "OfsX")]);
                        MAG.OfsY = double.Parse(item.items[dflog.FindMessageOffset(item.msgtype, "OfsY")]);
                        MAG.OfsZ = double.Parse(item.items[dflog.FindMessageOffset(item.msgtype, "OfsZ")]);
                    }
                    else if (item.msgtype == "ATT")
                    {
                        ATT.Roll = double.Parse(item.items[dflog.FindMessageOffset("ATT", "Roll")]);
                        ATT.Pitch = double.Parse(item.items[dflog.FindMessageOffset("ATT", "Pitch")]);
                        ATT.Yaw = double.Parse(item.items[dflog.FindMessageOffset("ATT", "Yaw")]);
                    }

                    answer.Add(item, mag_heading_df(MAG, ATT));
                }
            }


            return answer;
        }

        private class lowpass : FunctionExtension
        {
            private  double var;
            private  string key;
            private  double factor;
            static Dictionary<string,double> lowpass_data = new Dictionary<string, double>();

            public lowpass()
            {
            }
            public lowpass(double var, double key, double factor)
            {
                this.var = var;
                this.key = key.ToString();
                this.factor = factor;
            }
            public double calculate()
            {
                if (!lowpass_data.ContainsKey(key))
                    lowpass_data[key] = var;
                return factor * lowpass_data[key] + (1.0 - factor) * var;
            }

            public FunctionExtension clone()
            {
                return new lowpass(var, double.Parse(key), factor);
            }

            public string getParameterName(int argumentIndex)
            {
                if (argumentIndex == 0) return "var";
                if (argumentIndex == 1) return "key";
                if (argumentIndex == 2) return "factor";
                return "";
            }

            public int getParametersNumber()
            {
                return 3;
            }

            public void setParameterValue(int argumentIndex, double argumentValue)
            {
                if (argumentIndex == 0) var = argumentValue;
                if (argumentIndex == 1) key = argumentValue.ToString();
                if (argumentIndex == 2) factor = argumentValue;
            }
        }

        private class deltaF : FunctionExtension
        {
            private double vari;
            private string key;
            private double tusec;
            private double last_v;
            private double last_t;

            public deltaF()
            {
            }
            public deltaF(double vari, double key, double tusec)
            {
                this.vari = vari;
                this.key = key.ToString();
                this.tusec = tusec;
            }

            public double calculate()
            {
                double tnow = 0;
                if (tusec != 0)
                {
                    tnow = tusec * 1.0e-6;
                }
                double ret = 0.0;

                if (tnow == last_t)
                {
                    ret = 0;
                }
                else
                {
                    ret = (vari - last_v) / (tnow - last_t);
                }
                last_v = vari;
                last_t = tnow;
                return ret;
            }

            public FunctionExtension clone()
            {
                return new deltaF(vari, double.Parse(key), tusec);
            }

            public string getParameterName(int argumentIndex)
            {
                if (argumentIndex == 0) return "vari";
                if (argumentIndex == 1) return "key";
                if (argumentIndex == 2) return "tusec";
                return "";
            }

            public int getParametersNumber()
            {
                return 3;
            }

            public void setParameterValue(int argumentIndex, double argumentValue)
            {
                if (argumentIndex == 0) vari = argumentValue;
                if (argumentIndex == 1) key = argumentValue.ToString();
                if (argumentIndex == 2) tusec = argumentValue;
            }
        }
    }

    public class ATT_t
    {
        public double Roll;
        public double Pitch;
        public double Yaw;
    }

    public class MAG_t
    {
        public double MagX;
        public double MagY;
        public double MagZ;
        public double OfsX;
        public double OfsY;
        public double OfsZ;
    }

    public class IMU_t
    {
        public double AccX;
        public double AccY;
        public double AccZ;
    }

    public class GPS_t
    {
        public double Spd;
        public double GCrs;
        public double VZ;
    }
}