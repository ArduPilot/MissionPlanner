using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.HIL
{
    public class Utils
    {
        public const double rad2deg = (180/System.Math.PI);
        public const double deg2rad = (1.0/rad2deg);
        public const double ft2m = (1.0/3.2808399);
        public const double kts2fps = 1.68780986;
        public const float PI = (float) Math.PI;
        public const bool True = true;
        public const bool False = false;

        public static string str(byte[] input)
        {
            string st = System.Text.ASCIIEncoding.ASCII.GetString(input);

            int pos = st.IndexOf('\0');

            if (pos != -1)
            {
                st = st.Substring(0, pos);
            }

            return st;
        }

        // scaling factor from 1e-7 degrees to meters at equater
        // == 1.0e-7 * DEG_TO_RAD * RADIUS_OF_EARTH
        public const float LOCATION_SCALING_FACTOR = 0.011131884502145034f;
        // inverse of LOCATION_SCALING_FACTOR
        public const float LOCATION_SCALING_FACTOR_INV = 89.83204953368922f;

        //Single precision conversions
        public const float DEG_TO_RAD = 0.017453292519943295769236907684886f;
        public const float RAD_TO_DEG = 57.295779513082320876798154814105f;

        // are two floats equal
        public static bool is_equal(float fVal1, float fVal2)
        {
            return fabsf(fVal1 - fVal2) < Single.Epsilon ? true : false;
        }

// is a float is zero
        public static bool is_zero(float fVal1)
        {
            return fabsf(fVal1) < Single.Epsilon ? true : false;
        }


        public static Int32 labs(double val)
        {
            return (Int32) Math.Abs(val);
        }

        public static float fabsf(double val)
        {
            return (float) Math.Abs(val);
        }

        public static float sinf(double val)
        {
            return (float) System.Math.Sin(val);
        }

        public static float cosf(double val)
        {
            return (float) System.Math.Cos(val);
        }

        public static double sin(double val)
        {
            return System.Math.Sin(val);
        }

        public static double cos(double val)
        {
            return System.Math.Cos(val);
        }

        public static double acos(double val)
        {
            return System.Math.Acos(val);
        }

        public static float asinf(float val)
        {
            return (float) System.Math.Asin(val);
        }

        public static double asin(double val)
        {
            return System.Math.Asin(val);
        }

        public static double atan2(double val, double val2)
        {
            return System.Math.Atan2(val, val2);
        }

        public static double radians(double val)
        {
            return val*deg2rad;
        }

        public static double degrees(double val)
        {
            return val*rad2deg;
        }

        public static double sqrt(double val)
        {
            return System.Math.Sqrt(val);
        }

        public static float sqrtf(float val)
        {
            return (float) System.Math.Sqrt(val);
        }

        public static double abs(double val)
        {
            return System.Math.Abs(val);
        }

        public static bool isnan(float val)
        {
            return float.IsNaN(val);
        }

        // constrain a value
        public static float constrain_float(float amt, float low, float high)
        {
            // the check for NaN as a float prevents propogation of
            // floating point errors through any function that uses
            // constrain_float(). The normal float semantics already handle -Inf
            // and +Inf
            if (isnan(amt))
            {
                return (low + high)*0.5f;
            }
            return ((amt) < (low) ? (low) : ((amt) > (high) ? (high) : (amt)));
        }


        // a varient of asin() that checks the input ranges and ensures a
        // valid angle as output. If nan is given as input then zero is
        // returned.
        public static float safe_asin(float v)
        {
            if (isnan(v))
            {
                return 0.0f;
            }
            if (v >= 1.0f)
            {
                return PI/2;
            }
            if (v <= -1.0f)
            {
                return -PI/2;
            }
            return asinf(v);
        }

        // a varient of sqrt() that checks the input ranges and ensures a
        // valid value as output. If a negative number is given then 0 is
        // returned. The reasoning is that a negative number for sqrt() in our
        // code is usually caused by small numerical rounding errors, so the
        // real input should have been zero
        public static float safe_sqrt(float v)
        {
            float ret = sqrtf(v);
            if (isnan(ret))
            {
                return 0;
            }
            return ret;
        }

        public static int[] range(int no)
        {
            int[] range = new int[no];
            for (int a = 0; a < no; a++)
            {
                range[a] = a;
            }
            return range;
        }

        public static double fabs(double val)
        {
            return System.Math.Abs(val);
        }

        public static double tan(double val)
        {
            return System.Math.Tan(val);
        }

        public static int len(object[] data)
        {
            return data.Length;
        }

        public static int len<T>(List<T> data)
        {
            return data.Count;
        }

        public static Tuple<double, double, double> EarthRatesToBodyRates(double roll, double pitch, double yaw,
            double rollRate, double pitchRate, double yawRate)
        {
            //convert the angular velocities from earth frame to
            //body frame. Thanks to James Goppert for the formula

            //all inputs and outputs are in degrees

            //returns a tuple, (p,q,r)


            var phi = radians(roll);
            var theta = radians(pitch);
            var phiDot = radians(rollRate);
            var thetaDot = radians(pitchRate);
            var psiDot = radians(yawRate);

            var p = phiDot - psiDot*sin(theta);
            var q = cos(phi)*thetaDot + sin(phi)*psiDot*cos(theta);
            var r = cos(phi)*psiDot*cos(theta) - sin(phi)*thetaDot;

            return new Tuple<double, double, double>(degrees(p), degrees(q), degrees(r));
        }

        public static Tuple<double, double, double> EarthRatesToBodyRatesRyan(double roll, double pitch, double yaw,
            double x, double y, double z)
        {
            // thanks to ryan beall

            var phi = radians(roll);
            var theta = radians(pitch);
            var psi = radians((360 - yaw)*1.0);
            var Po = radians(x);
            var Qo = radians(y);
            var Ro = radians(-z);

            var P = Po*cos(psi)*cos(theta) - Ro*sin(theta) + Qo*cos(theta)*sin(psi);

            var Q = Qo*(cos(phi)*cos(psi) + sin(phi)*sin(psi)*sin(theta)) -
                    Po*(cos(phi)*sin(psi) - cos(psi)*sin(phi)*sin(theta)) + Ro*cos(theta)*sin(phi);

            var R = Po*(sin(phi)*sin(psi) + cos(phi)*cos(psi)*sin(theta)) -
                    Qo*(cos(psi)*sin(phi) - cos(phi)*sin(psi)*sin(theta)) + Ro*cos(phi)*cos(theta);


            //            P = 0;
            //Q = 0;
            //R = 0;


            return new Tuple<double, double, double>(degrees(P), degrees(Q), degrees(R));
        }

        public static Tuple<double, double, double> EarthRatesToBodyRatesMine(double roll, double pitch, double yaw,
            double rollRate, double pitchRate, double yawRate)
        {
            // thanks to ryan beall

            var phi = radians(roll);
            var theta = radians(pitch);
            var psi = radians(yaw);
            var Po = radians(pitchRate);
            var Ro = radians(yawRate);
            var Qo = radians(rollRate);

            var Q = Po*cos(psi) + Qo*sin(psi);

            var P = Po*sin(psi) + Qo*cos(psi);
            ;

            var R = Ro;

            return new Tuple<double, double, double>(degrees(P), degrees(Q), degrees(R));


            /*
            double Cr, Cp, Cz;
            double Sr, Sp, Sz;

            var phi = radians(roll);
            var theta = radians(pitch);
            var psi = radians(yaw);
            var Po = radians(rollRate);
            var Ro = radians(pitchRate);
            var Qo = radians(yawRate);

            Cr = Math.Cos((phi));
            Cp = Math.Cos((theta));
            Cz = Math.Cos((psi));
            Sr = Math.Sin((phi));
            Sp = Math.Sin((theta));
            Sz = Math.Sin((psi));

            // http://planning.cs.uiuc.edu/node102.html
            //        Z    Y     X
            // roll  -Sp, CpSr, CpCr
            // pitch SzCp, SzSpSr+CzCr, SzSpCr-CpCr
            // yaw   CzCp, CzSpSr-SzCr, CzSpCr+SzSr
            
            var P = -(Qo * Sp) + Po * Cp * Sr + Ro * Cp * Cr;

            var Q = Qo * (Sz * Cp) + Po * (Sz * Sp * Sr + Cz * Cr) + Ro * (Sz * Sp * Cr - Cp * Cr);

            var R = Qo * (Cz * Cp) + Po * (Cz * Sp * Sr - Sz * Cr) + Ro * (Cz * Sp * Cr + Sz * Sr);
            
            return new Tuple<double, double, double>(degrees(P), degrees(Q), degrees(R));
             * */
        }

        public static Tuple<double, double, double> OGLtoBCBF(double phi, double theta, double psi, double x, double y,
            double z)
        {
            double x_NED, y_NED, z_NED;
            double Cr, Cp, Cy;
            double Sr, Sp, Sy;

            //Accelerations in X-Plane are expressed in the local OpenGL reference frame, for whatever reason. 
            //This coordinate system is defined as follows (taken from the X-Plane SDK Wiki):

            //	The origin 0,0,0 is on the surface of the earth at sea level at some "reference point".
            //	The +X axis points east from the reference point.
            //	The +Z axis points south from the reference point.
            //	The +Y axis points straight up away from the center of the earth at the reference point.

            // First we shall convert from this East Up South frame, to a more conventional NED (North East Down) frame.
            x_NED = (x)*-1.0;
            y_NED = (y)*1.0;
            z_NED = (z)*-1.0;

            // Next calculate cos & sin of angles for use in the transformation matrix.
            // r, p & y subscripts stand for roll pitch and yaw.

            Cr = Math.Cos((phi));
            Cp = Math.Cos((theta));
            Cy = Math.Cos((psi));
            Sr = Math.Sin((phi));
            Sp = Math.Sin((theta));
            Sy = Math.Sin((psi));

            // Next we need to rotate our accelerations from the NED reference frame, into the body fixed reference frame

            // THANKS TO GEORGE M SIOURIS WHOSE "MISSILE GUIDANCE AND CONTROL SYSTEMS" BOOK SEEMS TO BE THE ONLY EASY TO FIND REFERENCE THAT
            // ACTUALLY GETS THE NED TO BODY FRAME ROTATION MATRIX CORRECT!!

            // CpCy, CpSy, -Sp					| local_ax
            // SrSpCy-CrSy, SrSpSy+CrCy, SrCp	| local_ay
            // CrSpCy+SrSy, CrSpSy-SrCy, CrCp	| local_az

            x = (x_NED*Cp*Cy) + (y_NED*Cp*Sy) - (z_NED*Sp);
            y = (x_NED*((Sr*Sp*Cy) - (Cr*Sy))) + (y_NED*((Sr*Sp*Sy) + (Cr*Cy))) + (z_NED*Sr*Cp);
            z = (x_NED*((Cr*Sp*Cy) + (Sr*Sy))) + (y_NED*((Cr*Sp*Sy) - (Sr*Cy))) + (z_NED*Cr*Cp);

            return new Tuple<double, double, double>((x), (y), (z));
        }


        /// <summary>
        /// From http://code.google.com/p/gentlenav/source/browse/trunk/Tools/XP_UDB_HILSIM/utility.cpp
        /// Converts from xplanes to fixed body ref
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        public static void FLIGHTtoBCBF(ref float x, ref float y, ref float z, float alpha, float beta)
        {
            float Ca = (float) Math.Cos(alpha);
            float Cb = (float) Math.Cos(beta);
            float Sa = (float) Math.Sin(alpha);
            float Sb = (float) Math.Sin(beta);

            float X_plane = (x*Ca*Cb) - (z*Sa*Cb) - (y*Sb);
            float Y_plane = (z*Sa*Sb) - (x*Ca*Sb) - (y*Cb);
            float Z_plane = (x*Sa) + (z*Ca);

            x = X_plane;
            y = Y_plane;
            z = Z_plane;
        }

        public static Vector3 BodyRatesToEarthRates(Matrix3 dcm, Vector3 gyro)
        {
            //'''convert the angular velocities from body frame to
            //earth frame.

            //all inputs and outputs are in radians/s

            //returns a earth rate vector
            //'''

            var p = gyro.x;
            var q = gyro.y;
            var r = gyro.z;

            double phi = 0;
            double theta = 0;
            double psi = 0;

            dcm.to_euler(ref phi, ref theta, ref psi);

            var phiDot = p + tan(theta)*(q*sin(phi) + r*cos(phi));
            var thetaDot = q*cos(phi) - r*sin(phi);
            if (fabs(cos(theta)) < 1.0e-20)
                theta += 1.0e-10;
            var psiDot = (q*sin(phi) + r*cos(phi))/cos(theta);

            return new Vector3((phiDot), (thetaDot), (psiDot));
        }


        public void gps_newpos(double lat, double lon, double bearing, double distance, ref double latitude,
            ref double longitude)
        {
            // '''extrapolate latitude/longitude given a heading and distance 
            //   thanks to http://www.movable-type.co.uk/scripts/latlong.html
            //  '''
            // from math import sin, asin, cos, atan2, radians, degrees
            double radius_of_earth = 6378100.0; //# in meters

            double lat1 = radians(lat);
            double lon1 = radians(lon);
            double brng = radians(bearing);
            double dr = distance/radius_of_earth;

            double lat2 = asin(sin(lat1)*cos(dr) +
                               cos(lat1)*sin(dr)*cos(brng));
            double lon2 = lon1 + atan2(sin(brng)*sin(dr)*cos(lat1),
                cos(dr) - sin(lat1)*sin(lat2));

            latitude = degrees(lat2);
            longitude = degrees(lon2);
            //return (degrees(lat2), degrees(lon2));
        }
    }
}