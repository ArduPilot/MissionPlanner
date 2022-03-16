using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace MissionPlanner.Utilities
{
    public class dfgpslag
    {
        public static void getdata(string file)
        {
            DFLog.DFItem? IMU = null;
            DFLog.DFItem? ATT = null;
            List<Vector3> gaccel = new List<Vector3>();
            List<Vector3> vel = new List<Vector3>();
            List<DateTime> timestamps = new List<DateTime>();
            List<int> accel_indexes = new List<int>();
            var dtsum = 0.0;
            var dtcount = 0;

            var cb = new DFLogBuffer(File.OpenRead(file));
            foreach (var item in cb.GetEnumeratorType(new string[] { "GPS", "GPS2", "IMU", "ATT" }))
            {   
                switch (item.msgtype)
                {
                    case "GPS":
                    case "GPS2":
                        if ((Byte)item.GetRaw("Status") >= 3)
                        {
                            var v = new Vector3(
                                (Single) item.GetRaw("Spd") * Math.Cos(radians((Single) item.GetRaw("GCrs"))),
                                (Single) item.GetRaw("Spd") * Math.Sin(radians((Single) item.GetRaw("GCrs"))),
                                (Single) item.GetRaw("VZ"));
                            vel.Add(v);
                            timestamps.Add(item.time);
                            accel_indexes.Add(Math.Max(gaccel.Count - 1, 0));
                        }

                        break;
                    case "IMU":
                        if (ATT.HasValue)
                        {
                            gaccel.Add(earth_accel_df(item, ATT.Value));
                            if (IMU.HasValue)
                            {
                                var dt = item.time - IMU.Value.time;
                                dtsum += dt.TotalSeconds;
                                dtcount += 1;
                            }
                            IMU = item;
                        }
                        break;
                    case "ATT":
                        ATT = item;
                        break;
                }
            }

            var imu_dt = dtsum / dtcount;

            Console.WriteLine("{0} samples at dt {1}", vel.Count, imu_dt);

            var besti = -1;
            var besterr = 0.0;
            List<double> delays = new List<double>();
            List<double> errors = new List<double>();

            foreach (var i in Enumerable.Range(0, 100))
            {
                var err = velocity_error(timestamps, vel, gaccel, accel_indexes, imu_dt, i);
                if (err == null)
                    break;
                errors.Add(err);
                delays.Add(i * imu_dt);
                if (besti == -1 || err < besterr)
                {
                    besti = i;
                    besterr = err;
                }
            }

            Console.WriteLine("Best {0} ({1}s) {2}", besti, besti * imu_dt, besterr);

        }

        public static double velocity_error(List<DateTime> timestamps, List<Vector3> vel, List<Vector3> gaccel,
            List<int> accel_indexes, double imu_dt, int shift = 0)
        {
            //'''return summed velocity error'''
            var sum = 0.0;
            var count = 0;
            foreach (var i in Enumerable.Range(0, vel.Count - 1))
            {
                var dv = vel[i + 1] - vel[i];
                var da = new Vector3();
                foreach (var idx in range(1 + accel_indexes[i] - shift, 1 + accel_indexes[i + 1] - shift))
                {
                    if (idx < 0 || idx >= gaccel.Count)
                        continue;
                    da += gaccel[idx];
                }

                var dt1 = (timestamps[i + 1] - timestamps[i]).TotalSeconds;
                var dt2 = (accel_indexes[i + 1] - accel_indexes[i]) * imu_dt;
                da *= imu_dt;
                da *= dt1 / dt2;
//# print(accel_indexes[i+1] - accel_indexes[i]);
                var ex = Math.Abs(dv.x - da.x);
                var ey = Math.Abs(dv.y - da.y);
                sum += 0.5 * (ex + ey);
                count += 1;
            }

            if (count == 0)
                return 0;
            return sum / count;
        }

        private static IEnumerable<int> range(int start, int stop, int step=1)
        {
            for (int i = start; i < stop; i+=step)
            {
                yield return i;
            }
        }

        public static Vector3 earth_accel_df(DFLog.DFItem IMU, DFLog.DFItem ATT)
        {
            //return earth frame acceleration vector from df log
            var r = rotation_df(ATT);
            var accel = new Vector3((float)IMU.GetRaw("AccX"), (float)IMU.GetRaw("AccY"), (float)IMU.GetRaw("AccZ"));
            return r * accel;
        }

        public static Matrix3 rotation_df(DFLog.DFItem ATT)
        {
            //return the current DCM rotation matrix''' 
            var r = new Matrix3();
            r.from_euler(radians((Double)ATT.GetRaw("Roll")), radians((Double)ATT.GetRaw("Pitch")), radians((Double)ATT.GetRaw("Yaw")));
            return r;
        }
        public static double radians(double val)
        {
            return val * MathHelper.deg2rad;
        }
    }
}
