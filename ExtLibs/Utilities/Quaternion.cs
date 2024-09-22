using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public class Quaternion
    {
        public double q1, q2, q3, q4;

        /// <summary>
        /// Create a quaternion that represents no rotation
        /// </summary>
        public Quaternion()
        {
            q1 = 1;
            q2 = q3 = q4 = 0;
        }

        /// <summary>
        /// Create a quaternion with the specified values. The values are not required to be normalized,
        /// but no normalization will happen automatically 
        /// </summary>
        public Quaternion(double q1, double q2, double q3, double q4)
        {
            this.q1 = q1;
            this.q2 = q2;
            this.q3 = q3;
            this.q4 = q4;
        }

        /// <summary>
        /// Create a quaternion equivalent to the supplied matrix
        /// Thanks to Martin John Baker
        /// http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        /// </summary>
        /// <param name="m">Rotation matrix. Must be a valid rotation matrix (orthogonal and with determinant 1)</param>
        /// <returns></returns>
        public static Quaternion from_rotation_matrix(Matrix3 m)
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

            double tr = m00 + m11 + m22;

            if (tr > 0)
            {
                double S = Math.Sqrt(tr + 1) * 2;
                return new Quaternion(
                    0.25 * S,
                    (m21 - m12) / S,
                    (m02 - m20) / S,
                    (m10 - m01) / S
                );
            }
            else if ((m00 > m11) && (m00 > m22))
            {
                double S = Math.Sqrt(1.0 + m00 - m11 - m22) * 2.0;
                return new Quaternion(
                    (m21 - m12) / S,
                    0.25 * S,
                    (m01 + m10) / S,
                    (m02 + m20) / S
                );
            }
            else if (m11 > m22)
            {
                double S = Math.Sqrt(1.0 + m11 - m00 - m22) * 2.0;
                return new Quaternion(
                    (m02 - m20) / S,
                    (m01 + m10) / S,
                    0.25 * S,
                    (m12 + m21) / S
                );
            }
            else
            {
                double S = Math.Sqrt(1.0 + m22 - m00 - m11) * 2.0;
                return new Quaternion(
                    (m10 - m01) / S,
                    (m02 + m20) / S,
                    (m12 + m21) / S,
                    0.25 * S
                );
            }
        }

        /// <summary>
        /// create a quaternion from Euler angles
        /// </summary>
        /// <param name="roll">Roll angle in radians</param>
        /// <param name="pitch">Pitch angle in radians</param>
        /// <param name="yaw">Yaw angle in radians</param>
        /// <returns></returns>
        public static Quaternion from_euler(double roll, double pitch, double yaw)
        {
            double cr2 = Math.Cos(roll * 0.5);
            double cp2 = Math.Cos(pitch * 0.5);
            double cy2 = Math.Cos(yaw * 0.5);
            double sr2 = Math.Sin(roll * 0.5);
            double sp2 = Math.Sin(pitch * 0.5);
            double sy2 = Math.Sin(yaw * 0.5);

            return new Quaternion(
                cr2 * cp2 * cy2 + sr2 * sp2 * sy2,
                sr2 * cp2 * cy2 - cr2 * sp2 * sy2,
                cr2 * sp2 * cy2 + sr2 * cp2 * sy2,
                cr2 * cp2 * sy2 - sr2 * sp2 * cy2
            );
        }

        /// <summary>
        /// Create a quaternion from Euler angles in 3-1-2 order (common joint order for gimbals)
        /// </summary>
        /// <param name="roll">Roll angle in radians</param>
        /// <param name="pitch">Pitch angle in radians</param>
        /// <param name="yaw">Yaw angle in radians</param>
        /// <returns></returns>
        public static Quaternion from_euler312(double roll, double pitch, double yaw)
        {
            Matrix3 m = new Matrix3();
            m.from_euler312(roll, pitch, yaw);

            return from_rotation_matrix(m);
        }

        /// <summary>
        /// Create a quaternion that represents a rotation about a vector axis (right-hand rule)
        /// </summary>
        /// <param name="axis">Axis about which to rotate. This must be a unit vector, or the resulting quaternion will be wrong.</param>
        /// <param name="theta">Angle in radians</param>
        /// <returns></returns>
        public static Quaternion from_axis_angle(Vector3 axis, double theta)
        {
            if (theta == 0)
            {
                return new Quaternion();
            }

            double st2 = Math.Sin(theta / 2.0);

            return new Quaternion(
                Math.Cos(theta / 2.0),
                axis.x * st2,
                axis.y * st2,
                axis.z * st2
            );
        }

        /// <summary>
        /// Create a quaternion that represents a rotation about a vector axis (right-hand rule)
        /// </summary>
        /// <param name="v">Axis about which to rotate, with a length that represents the angle in radians</param>
        /// <returns></returns>
        public static Quaternion from_axis_angle(Vector3 v)
        {
            double theta = v.length();
            if (theta == 0)
            {
                return new Quaternion();
            }

            return from_axis_angle(v / theta, theta);
        }

        /// <summary>
        /// Check if any element of the quaternion is NaN
        /// </summary>
        /// <returns></returns>
        public bool is_nan()
        {
            return double.IsNaN(q1) || double.IsNaN(q2) || double.IsNaN(q3) || double.IsNaN(q4);
        }

        /// <summary>
        /// Return the equivalent rotation matrix for this quaternion
        /// </summary>
        /// <returns></returns>
        public Matrix3 rotation_matrix()
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

            var m = new Matrix3();
            m.a.x = 1.0 - 2.0 * (q3q3 + q4q4);
            m.a.y = 2.0 * (q2q3 - q1q4);
            m.a.z = 2.0 * (q2q4 + q1q3);
            m.b.x = 2.0 * (q2q3 + q1q4);
            m.b.y = 1.0 - 2.0 * (q2q2 + q4q4);
            m.b.z = 2.0 * (q3q4 - q1q2);
            m.c.x = 2.0 * (q2q4 - q1q3);
            m.c.y = 2.0 * (q3q4 + q1q2);
            m.c.z = 1.0 - 2.0 * (q2q2 + q3q3);

            return m;
        }

        /// <summary>
        /// Return the equivalent rotation matrix for a normalized version of this quaternion.
        /// (this does not modify this quaternion)
        /// </summary>
        /// <returns></returns>
        public Matrix3 rotation_matrix_norm()
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
            double invs = 1.0 / (q1q1 + q2q2 + q3q3 + q4q4);

            var m = new Matrix3();
            m.a.x = (q2q2 - q3q3 - q4q4 + q1q1) * invs;
            m.a.y = 2.0 * (q2q3 - q1q4) * invs;
            m.a.z = 2.0 * (q2q4 + q1q3) * invs;
            m.b.x = 2.0 * (q2q3 + q1q4) * invs;
            m.b.y = (-q2q2 + q3q3 - q4q4 + q1q1) * invs;
            m.b.z = 2.0 * (q3q4 - q1q2) * invs;
            m.c.x = 2.0 * (q2q4 - q1q3) * invs;
            m.c.y = 2.0 * (q3q4 + q1q2) * invs;
            m.c.z = (-q2q2 - q3q3 + q4q4 + q1q1) * invs;

            return m;
        }

        /// <summary>
        /// Convert a vector from body to earth frame
        /// (assuming this is a normalized quaternion that represents the rotation from earth to body)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 body_to_earth(Vector3 v)
        {
            return rotation_matrix() * v;
        }

        /// <summary>
        /// Convert a vector from earth to body frame
        /// (assuming this is a normalized quaternion that represents the rotation from earth to body)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 earth_to_body(Vector3 v)
        {
            return conjugate().body_to_earth(v);
        }

        /// <summary>
        /// Convert this quaternion to an axis-angle representation
        /// </summary>
        /// <returns>Vector of axis with a length of rotation angle in radians</returns>
        public Vector3 to_axis_angle()
        {
            var v = new Vector3(q2, q3, q4);
            var l = v.length();
            if (l != 0)
            {
                v /= l; // normalize
                v *= wrap_PI(2.0 * Math.Atan2(l, q1));
            }
            return v;
        }

        /// <summary>
        /// Wrap an angle to the range -PI to PI
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        double wrap_PI(double radian)
        {
            var res = wrap_2PI(radian);
            if (res > Math.PI)
            {
                res -= (Math.PI * 2);
            }

            return res;
        }

        /// <summary>
        /// Wrap an angle to the range 0 to 2PI
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        double wrap_2PI(double radian)
        {
            var res = radian % (Math.PI * 2);
            if (res < 0)
            {
                res += (Math.PI * 2);
            }

            return res;
        }
        
        /// <summary>
        /// Square a number
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private double sq(double n)
        {
            return n * n;
        }

        /// <summary>
        /// Get euler roll angle in radians
        /// </summary>
        /// <returns></returns>
        public double get_euler_roll()
        {
            return (Math.Atan2(2.0 * (q1 * q2 + q3 * q4), 1.0 - 2.0 * (q2 * q2 + q3 * q3)));
        }

        /// <summary>
        /// Get euler pitch angle in radians
        /// </summary>
        /// <returns></returns>
        public double get_euler_pitch()
        {
            return Math.Asin(2.0 * (q1 * q3 - q4 * q2));
        }

        /// <summary>
        /// Get euler yaw angle in radians
        /// </summary>
        /// <returns></returns>
        public double get_euler_yaw()
        {
            return Math.Atan2(2.0 * (q1 * q4 + q2 * q3), 1.0 - 2.0 * (q3 * q3 + q4 * q4));
        }

        /// <summary>
        /// Output euler angles for this quaternion in radians
        /// </summary>
        /// <param name="roll"></param>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        public void to_euler(out double roll, out double pitch, out double yaw)
        {
            roll = get_euler_roll();
            pitch = get_euler_pitch();
            yaw = get_euler_yaw();
        }

        /// <summary>
        /// Get the magnitude of this quaternion
        /// </summary>
        /// <returns></returns>
        public double length()
        {
            return Math.Sqrt(sq(q1) + sq(q2) + sq(q3) + sq(q4));
        }

        /// <summary>
        /// Get the conjugate of this quaternion
        /// (for unit quaternions, this is the same as the inverse)
        /// </summary>
        /// <returns></returns>
        public Quaternion conjugate()
        {
            return new Quaternion(q1, -q2, -q3, -q4);
        }

        /// <summary>
        /// Normalize this quaternion
        /// </summary>
        public void normalize()
        {
            double quatMag = length();
            if (quatMag != 0)
            {
                double quatMagInv = 1.0 / quatMag;
                q1 *= quatMagInv;
                q2 *= quatMagInv;
                q3 *= quatMagInv;
                q4 *= quatMagInv;
            }
        }

        /// <summary>
        /// Quaternion multiplication w * v
        /// (for rotations, this is equivalent to applying v and then w)
        /// </summary>
        /// <param name="w"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Quaternion operator *(Quaternion w, Quaternion v)
        {
            return new Quaternion(
                w.q1 * v.q1 - w.q2 * v.q2 - w.q3 * v.q3 - w.q4 * v.q4,
                w.q1 * v.q2 + w.q2 * v.q1 + w.q3 * v.q4 - w.q4 * v.q3,
                w.q1 * v.q3 - w.q2 * v.q4 + w.q3 * v.q1 + w.q4 * v.q2,
                w.q1 * v.q4 + w.q2 * v.q3 - w.q3 * v.q2 + w.q4 * v.q1
            );
        }
    }
}
