using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public class Quaternion
    {
        private const double M_PI = Math.PI;
        private const double M_2PI = (M_PI * 2);

        public double q1, q2, q3, q4;

        // constructor creates a quaternion equivalent
        // to roll=0, pitch=0, yaw=0
        public Quaternion()
        {
            q1 = 1;
            q2 = q3 = q4 = 0;
        }

        // setting constructor
        public Quaternion(double _q1, double _q2, double _q3, double _q4)
        {
            q1 = _q1;
            q2 = _q2;
            q3 = _q3;
            q4 = _q4;
        }


        // check if any elements are NAN
        bool is_nan()
        {
            return isnan(q1) || isnan(q2) || isnan(q3) || isnan(q4);
        }

        private bool isnan(double q1)
        {
            return double.IsNaN(q1);
        }

        // return the rotation matrix equivalent for this quaternion
        public void rotation_matrix(Matrix3 m)
        {
            double q3q3 = q3 * q3;
            double q3q4 = q3 * q4;
            double q2q2 = q2 * q2;
            double q2q3 = q2 * q3;
            double q2q4 = q2 * q4;
            double q1q2 = q1 * q2;
            double q1q3 = q1 * q3;
            double q1q4 = q1 * q4;
            double q4q4 = q4 * q4;

            m.a.x = 1.0f - 2.0f * (q3q3 + q4q4);
            m.a.y = 2.0f * (q2q3 - q1q4);
            m.a.z = 2.0f * (q2q4 + q1q3);
            m.b.x = 2.0f * (q2q3 + q1q4);
            m.b.y = 1.0f - 2.0f * (q2q2 + q4q4);
            m.b.z = 2.0f * (q3q4 - q1q2);
            m.c.x = 2.0f * (q2q4 - q1q3);
            m.c.y = 2.0f * (q3q4 + q1q2);
            m.c.z = 1.0f - 2.0f * (q2q2 + q3q3);
        }

        // return the rotation matrix equivalent for this quaternion after normalization
        public void rotation_matrix_norm(Matrix3 m)
        {
            double q1q1 = q1 * q1;
            double q1q2 = q1 * q2;
            double q1q3 = q1 * q3;
            double q1q4 = q1 * q4;
            double q2q2 = q2 * q2;
            double q2q3 = q2 * q3;
            double q2q4 = q2 * q4;
            double q3q3 = q3 * q3;
            double q3q4 = q3 * q4;
            double q4q4 = q4 * q4;
            double invs = 1.0f / (q1q1 + q2q2 + q3q3 + q4q4);

            m.a.x = (q2q2 - q3q3 - q4q4 + q1q1) * invs;
            m.a.y = 2.0f * (q2q3 - q1q4) * invs;
            m.a.z = 2.0f * (q2q4 + q1q3) * invs;
            m.b.x = 2.0f * (q2q3 + q1q4) * invs;
            m.b.y = (-q2q2 + q3q3 - q4q4 + q1q1) * invs;
            m.b.z = 2.0f * (q3q4 - q1q2) * invs;
            m.c.x = 2.0f * (q2q4 - q1q3) * invs;
            m.c.y = 2.0f * (q3q4 + q1q2) * invs;
            m.c.z = (-q2q2 - q3q3 + q4q4 + q1q1) * invs;
        }

        // return the rotation matrix equivalent for this quaternion
        // Thanks to Martin John Baker
        // http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        public void from_rotation_matrix(Matrix3 m)
        {
            double m00 = m.a.x;
            double m11 = m.b.y;
            double m22 = m.c.z;
            double m10 = m.b.x;
            double m01 = m.a.y;
            double m20 = m.c.x;
            double m02 = m.a.z;
            double m21 = m.c.y;
            double m12 = m.b.z;
            ref double qw = ref q1;
            ref double qx = ref q2;
            ref double qy = ref q3;
            ref double qz = ref q4;

            double tr = m00 + m11 + m22;

            if (tr > 0)
            {
                double S = sqrtf(tr + 1) * 2;
                qw = 0.25f * S;
                qx = (m21 - m12) / S;
                qy = (m02 - m20) / S;
                qz = (m10 - m01) / S;
            }
            else if ((m00 > m11) && (m00 > m22))
            {
                double S = sqrtf(1.0f + m00 - m11 - m22) * 2.0f;
                qw = (m21 - m12) / S;
                qx = 0.25f * S;
                qy = (m01 + m10) / S;
                qz = (m02 + m20) / S;
            }
            else if (m11 > m22)
            {
                double S = sqrtf(1.0f + m11 - m00 - m22) * 2.0f;
                qw = (m02 - m20) / S;
                qx = (m01 + m10) / S;
                qy = 0.25f * S;
                qz = (m12 + m21) / S;
            }
            else
            {
                double S = sqrtf(1.0f + m22 - m00 - m11) * 2.0f;
                qw = (m10 - m01) / S;
                qx = (m02 + m20) / S;
                qy = (m12 + m21) / S;
                qz = 0.25f * S;
            }
        }

        private double sqrtf(double v)
        {
            return Math.Sqrt(v);
        }

        // convert a vector from earth to body frame
        public void earth_to_body(Vector3f v)
        {
            Matrix3 m = new Matrix3();
            rotation_matrix(m);
            v = m * v;
        }

        // create a quaternion from Euler angles
        public void from_euler(double roll, double pitch, double yaw)
        {
            double cr2 = cosf(roll * 0.5f);
            double cp2 = cosf(pitch * 0.5f);
            double cy2 = cosf(yaw * 0.5f);
            double sr2 = sinf(roll * 0.5f);
            double sp2 = sinf(pitch * 0.5f);
            double sy2 = sinf(yaw * 0.5f);

            q1 = cr2 * cp2 * cy2 + sr2 * sp2 * sy2;
            q2 = sr2 * cp2 * cy2 - cr2 * sp2 * sy2;
            q3 = cr2 * sp2 * cy2 + sr2 * cp2 * sy2;
            q4 = cr2 * cp2 * sy2 - sr2 * sp2 * cy2;
        }

        private double sinf(double v)
        {
            return Math.Sin(v);
        }

        private double cosf(double v)
        {
            return Math.Cos(v);
        }

        // create a quaternion from Euler angles
        public void from_vector312(double roll, double pitch, double yaw)
        {
            Matrix3 m = new Matrix3();
            m.from_euler312(roll, pitch, yaw);

            from_rotation_matrix(m);
        }

        public void from_axis_angle(Vector3f v)
        {
            double theta = v.length();
            if (is_zero(theta))
            {
                q1 = 1.0f;
                q2 = q3 = q4 = 0.0f;
                return;
            }

            v /= theta;
            from_axis_angle(v, theta);
        }

        public void from_axis_angle(Vector3f axis, double theta)
        {
            // axis must be a unit vector as there is no check for length
            if (is_zero(theta))
            {
                q1 = 1.0f;
                q2 = q3 = q4 = 0.0f;
                return;
            }

            double st2 = sinf(theta / 2.0f);

            q1 = cosf(theta / 2.0f);
            q2 = axis.x * st2;
            q3 = axis.y * st2;
            q4 = axis.z * st2;
        }

        private bool is_zero(double theta)
        {
            return theta == 0;
        }

        public Quaternion rotate(Vector3f v)
        {
            Quaternion r = new Quaternion();
            r.from_axis_angle(v);
            return r;
        }

        public void to_axis_angle(Vector3f v)
        {
            double l = sqrt(sq(q2) + sq(q3) + sq(q4));
            v = new Vector3(q2, q3, q4);
            if (!is_zero(l))
            {
                v /= l;
                v *= wrap_PI(2.0f * atan2f(l, q1));
            }
        }

        double wrap_PI(double radian)
        {
            var res = wrap_2PI(radian);
            if (res > M_PI)
            {
                res -= M_2PI;
            }

            return res;
        }

        double wrap_2PI(double radian)
        {
            var res = radian % M_2PI;
            if (res < 0)
            {
                res += M_2PI;
            }

            return res;
        }

        private double atan2f(double l, double q1)
        {
            return Math.Atan2(l, q1);
        }

        private double sqrt(double v)
        {
            return Math.Sqrt(v);
        }

        private double sq(double q2)
        {
            return q2 * q2;
        }

        public void from_axis_angle_fast(Vector3f v)
        {
            double theta = v.length();
            if (is_zero(theta))
            {
                q1 = 1.0f;
                q2 = q3 = q4 = 0.0f;
                return;
            }

            v /= theta;
            from_axis_angle_fast(v, theta);
        }

        public void from_axis_angle_fast(Vector3f axis, double theta)
        {
            double t2 = theta / 2.0f;
            double sqt2 = sq(t2);
            double st2 = t2 - sqt2 * t2 / 6.0f;

            q1 = 1.0f - (sqt2 / 2.0f) + sq(sqt2) / 24.0f;
            q2 = axis.x * st2;
            q3 = axis.y * st2;
            q4 = axis.z * st2;
        }

        public void rotate_fast(Vector3f v)
        {
            double theta = v.length();
            if (is_zero(theta))
            {
                return;
            }

            double t2 = theta / 2.0f;
            double sqt2 = sq(t2);
            double st2 = t2 - sqt2 * t2 / 6.0f;
            st2 /= theta;

            //"rotation quaternion"
            double w2 = 1.0f - (sqt2 / 2.0f) + sq(sqt2) / 24.0f;
            double x2 = v.x * st2;
            double y2 = v.y * st2;
            double z2 = v.z * st2;

            //copy our quaternion
            double w1 = q1;
            double x1 = q2;
            double y1 = q3;
            double z1 = q4;

            //do the multiply into our quaternion
            q1 = w1 * w2 - x1 * x2 - y1 * y2 - z1 * z2;
            q2 = w1 * x2 + x1 * w2 + y1 * z2 - z1 * y2;
            q3 = w1 * y2 - x1 * z2 + y1 * w2 + z1 * x2;
            q4 = w1 * z2 + x1 * y2 - y1 * x2 + z1 * w2;
        }

        // get euler roll angle
        public double get_euler_roll()
        {
            return (atan2f(2.0f * (q1 * q2 + q3 * q4), 1.0f - 2.0f * (q2 * q2 + q3 * q3)));
        }

        // get euler pitch angle
        public double get_euler_pitch()
        {
            return safe_asin(2.0f * (q1 * q3 - q4 * q2));
        }

        private double safe_asin(double v)
        {
            return Math.Asin(v);
        }

        // get euler yaw angle
        public double get_euler_yaw()
        {
            return atan2f(2.0f * (q1 * q4 + q2 * q3), 1.0f - 2.0f * (q3 * q3 + q4 * q4));
        }

        // create eulers from a quaternion
        public void to_euler(ref double roll,ref  double pitch,ref double yaw)
        {
            roll = get_euler_roll();
            pitch = get_euler_pitch();
            yaw = get_euler_yaw();
        }

        // create eulers from a quaternion
        public Vector3f to_vector312()
        {
            Matrix3 m = new Matrix3();
            rotation_matrix(m);
            return m.to_euler312();
        }

        public double length()
        {
            return sqrtf(sq(q1) + sq(q2) + sq(q3) + sq(q4));
        }

        public Quaternion inverse()
        {
            return new Quaternion(q1, -q2, -q3, -q4);
        }

        public void normalize()
        {
            double quatMag = length();
            if (!is_zero(quatMag))
            {
                double quatMagInv = 1.0f / quatMag;
                q1 *= quatMagInv;
                q2 *= quatMagInv;
                q3 *= quatMagInv;
                q4 *= quatMagInv;
            }
        }

        public static Quaternion operator *(Quaternion self, Quaternion v)
        {
            Quaternion ret = new Quaternion();
            double w1 = self.q1;
            double x1 = self.q2;
            double y1 = self.q3;
            double z1 = self.q4;

            double w2 = v.q1;
            double x2 = v.q2;
            double y2 = v.q3;
            double z2 = v.q4;

            ret.q1 = w1 * w2 - x1 * x2 - y1 * y2 - z1 * z2;
            ret.q2 = w1 * x2 + x1 * w2 + y1 * z2 - z1 * y2;
            ret.q3 = w1 * y2 - x1 * z2 + y1 * w2 + z1 * x2;
            ret.q4 = w1 * z2 + x1 * y2 - y1 * x2 + z1 * w2;

            return ret;
        }

        public static Quaternion operator /(Quaternion self, Quaternion v)
        {
            Quaternion ret = new Quaternion();
            double quat0 = self.q1;
            double quat1 = self.q2;
            double quat2 = self.q3;
            double quat3 = self.q4;

            double rquat0 = v.q1;
            double rquat1 = v.q2;
            double rquat2 = v.q3;
            double rquat3 = v.q4;

            ret.q1 = (rquat0 * quat0 + rquat1 * quat1 + rquat2 * quat2 + rquat3 * quat3);
            ret.q2 = (rquat0 * quat1 - rquat1 * quat0 - rquat2 * quat3 + rquat3 * quat2);
            ret.q3 = (rquat0 * quat2 + rquat1 * quat3 - rquat2 * quat0 - rquat3 * quat1);
            ret.q4 = (rquat0 * quat3 - rquat1 * quat2 + rquat2 * quat1 - rquat3 * quat0);
            return ret;
        }
    }
}