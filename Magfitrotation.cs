using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using MissionPlanner;
using MissionPlanner.HIL;

namespace MissionPlanner
{
    public class Magfitrotation : HIL.Utils
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // copy of https://github.com/mavlink/mavlink/blob/master/pymavlink/tools/magfit_rotation_gyro.py
        class Rotation
        {
            Rotation self;
            public string name;
            public double roll;
            public double pitch;
            public double yaw;
            public Matrix3 r;

            public Rotation(string name, double roll, double pitch, double yaw)
            {
                self = this;
                self.name = name;
                self.roll = roll;
                self.pitch = pitch;
                self.yaw = yaw;
                self.r = new Matrix3();
                r.from_euler(roll, pitch, yaw);
            }

            public bool is_90_degrees()
            {
                return (self.roll%90 == 0) && (self.pitch%90 == 0) && (self.yaw%90 == 0);
            }

            public override string ToString()
            {
                return self.name;
            }
        }

        //# the rotations used in APM
        static Rotation[] rotations =
        {
            new Rotation("ROTATION_NONE", 0, 0, 0),
            new Rotation("ROTATION_YAW_45", 0, 0, 45),
            new Rotation("ROTATION_YAW_90", 0, 0, 90),
            new Rotation("ROTATION_YAW_135", 0, 0, 135),
            new Rotation("ROTATION_YAW_180", 0, 0, 180),
            new Rotation("ROTATION_YAW_225", 0, 0, 225),
            new Rotation("ROTATION_YAW_270", 0, 0, 270),
            new Rotation("ROTATION_YAW_315", 0, 0, 315),
            new Rotation("ROTATION_ROLL_180", 180, 0, 0),
            new Rotation("ROTATION_ROLL_180_YAW_45", 180, 0, 45),
            new Rotation("ROTATION_ROLL_180_YAW_90", 180, 0, 90),
            new Rotation("ROTATION_ROLL_180_YAW_135", 180, 0, 135),
            new Rotation("ROTATION_PITCH_180", 0, 180, 0),
            new Rotation("ROTATION_ROLL_180_YAW_225", 180, 0, 225),
            new Rotation("ROTATION_ROLL_180_YAW_270", 180, 0, 270),
            new Rotation("ROTATION_ROLL_180_YAW_315", 180, 0, 315),
            new Rotation("ROTATION_ROLL_90", 90, 0, 0),
            new Rotation("ROTATION_ROLL_90_YAW_45", 90, 0, 45),
            new Rotation("ROTATION_ROLL_90_YAW_90", 90, 0, 90),
            new Rotation("ROTATION_ROLL_90_YAW_135", 90, 0, 135),
            new Rotation("ROTATION_ROLL_270", 270, 0, 0),
            new Rotation("ROTATION_ROLL_270_YAW_45", 270, 0, 45),
            new Rotation("ROTATION_ROLL_270_YAW_90", 270, 0, 90),
            new Rotation("ROTATION_ROLL_270_YAW_135", 270, 0, 135),
            new Rotation("ROTATION_PITCH_90", 0, 90, 0),
            new Rotation("ROTATION_PITCH_270", 0, 270, 0),
            new Rotation("ROTATION_PITCH_180_YAW_90", 0, 180, 90),
            new Rotation("ROTATION_PITCH_180_YAW_270", 0, 180, 270),
            new Rotation("ROTATION_ROLL_90_PITCH_90", 90, 90, 0),
            new Rotation("ROTATION_ROLL_180_PITCH_90", 180, 90, 0),
            new Rotation("ROTATION_ROLL_270_PITCH_90", 270, 90, 0),
            new Rotation("ROTATION_ROLL_90_PITCH_180", 90, 180, 0),
            new Rotation("ROTATION_ROLL_270_PITCH_180", 270, 180, 0),
            new Rotation("ROTATION_ROLL_90_PITCH_270", 90, 270, 0),
            new Rotation("ROTATION_ROLL_180_PITCH_270", 180, 270, 0),
            new Rotation("ROTATION_ROLL_270_PITCH_270", 270, 270, 0),
            new Rotation("ROTATION_ROLL_90_PITCH_180_YAW_90", 90, 180, 90),
            new Rotation("ROTATION_ROLL_90_YAW_270", 90, 0, 270)
        };

        static Vector3 mag_fixup(Vector3 mag, float AHRS_ORIENTATION, float COMPASS_ORIENT, float COMPASS_EXTERNAL)
        {
            //'''fixup a mag vector back to original value using AHRS and Compass orientation parameters'''
            if (COMPASS_EXTERNAL == 0 && AHRS_ORIENTATION != 0)
            {
                //# undo any board orientation
                mag = rotations[(int) AHRS_ORIENTATION].r.transposed()*mag;
            }
            //# undo any compass orientation
            if (COMPASS_ORIENT != 0)
            {
                mag = rotations[(int) COMPASS_ORIENT].r.transposed()*mag;
            }
            return mag;
        }

        static void add_errors(Vector3 mag, Vector3 gyr, Vector3 last_mag, double deltat, double[] total_error,
            Rotation[] rotations)
        {
            foreach (var i in range(len(rotations)))
            {
                // if (!rotations[i].is_90_degrees())
                //    continue;
                Matrix3 r = rotations[i].r;
                Matrix3 m = new Matrix3();
                m.rotate(gyr*deltat);
                Vector3 rmag1 = r*last_mag;
                Vector3 rmag2 = r*mag;
                Vector3 rmag3 = m.transposed()*rmag1;
                Vector3 err = rmag3 - rmag2;
                total_error[i] += err.length();
            }
        }


        public static string magfit(string logfile)
        {
            //'''find best magnetometer rotation fit to a log file'''


            // print("Processing log %s" % filename);
            // mlog = mavutil.mavlink_connection(filename, notimestamps=opts.notimestamps);

            using (MAVLinkInterface mavint = new MAVLinkInterface())
            {
                try
                {
                    mavint.BaseStream = new Comms.CommsFile();
                    mavint.BaseStream.PortName = logfile;
                    mavint.BaseStream.Open();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    return "";
                }

                mavint.logreadmode = true;

                return process(mavint);
            }
        }

        public static string magfit()
        {
            // give exclusive access ot this function
            MainV2.comPort.giveComport = true;

            // request more mag data and gyro data
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, 20);

            // process the data
            string ans = process(MainV2.comPort);

            MainV2.comPort.giveComport = false;

            return ans;
        }

        static string process(MAVLinkInterface mavint)
        {
            DateTime Deadline = DateTime.Now.AddSeconds(60);

            Vector3 last_mag = null;
            double last_usec = 0;
            double count = 0;
            double[] total_error = new double[rotations.Length];

            float AHRS_ORIENTATION = 0;
            float COMPASS_ORIENT = 0;
            float COMPASS_EXTERNAL = 0;

            //# now gather all the data
            while (DateTime.Now < Deadline || mavint.BaseStream.BytesToRead > 0)
            {
                byte[] packetbytes = mavint.readPacket();
                if (packetbytes.Length < 5)
                    continue;
                object packet = mavint.GetPacket(packetbytes);
                if (packet == null)
                    continue;
                if (packet is MAVLink.mavlink_param_value_t)
                {
                    MAVLink.mavlink_param_value_t m = (MAVLink.mavlink_param_value_t) packet;
                    if (str(m.param_id) == "AHRS_ORIENTATION")
                        AHRS_ORIENTATION = (int) (m.param_value);
                    if (str(m.param_id) == "COMPASS_ORIENT")
                        COMPASS_ORIENT = (int) (m.param_value);
                    if (str(m.param_id) == "COMPASS_EXTERNAL")
                        COMPASS_EXTERNAL = (int) (m.param_value);
                }

                if (packet is MAVLink.mavlink_raw_imu_t)
                {
                    MAVLink.mavlink_raw_imu_t m = (MAVLink.mavlink_raw_imu_t) packet;
                    Vector3 mag = new Vector3(m.xmag, m.ymag, m.zmag);
                    mag = mag_fixup(mag, AHRS_ORIENTATION, COMPASS_ORIENT, COMPASS_EXTERNAL);
                    Vector3 gyr = new Vector3(m.xgyro, m.ygyro, m.zgyro)*0.001;
                    double usec = m.time_usec;
                    if (last_mag != null && gyr.length() > radians(5.0))
                    {
                        add_errors(mag, gyr, last_mag, (usec - last_usec)*1.0e-6, total_error, rotations);
                        count += 1;
                    }
                    last_mag = mag;
                    last_usec = usec;
                }
            }

            int best_i = 0;
            double best_err = total_error[0];
            foreach (var i in range(len(rotations)))
            {
                Rotation r = rotations[i];
                //  if (!r.is_90_degrees())
                //      continue;

                //if ( opts.verbose) {
                //  print("%s err=%.2f" % (r, total_error[i]/count));}
                if (total_error[i] < best_err)
                {
                    best_i = i;
                    best_err = total_error[i];
                }
            }
            Rotation rans = rotations[best_i];
            Console.WriteLine("Best rotation is {0} err={1} from {2} points", rans, best_err/count, count);
            //print("Best rotation is %s err=%.2f from %u points" % (r, best_err/count, count));
            return rans.name;
        }
    }
}