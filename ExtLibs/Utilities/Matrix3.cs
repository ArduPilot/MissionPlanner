using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class Matrix3
    {
        //   '''a 3x3 matrix, intended as a rotation matrix'''

        public Vector3 a = new Vector3();
        public Vector3 b = new Vector3();
        public Vector3 c = new Vector3();

        public Matrix3()
        {
            identity();
        }

        public Matrix3(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public Matrix3(float a, float b, float c, float d, float e, float f, float g, float h, float i) : this(new Vector3(a,b,c), new Vector3(d,e,f), new Vector3(g,h,i))
        {

        }

        public new string ToString()
        {
            return String.Format("Matrix3(({0}, {1}, {2}), ({3}, {4}, {5}), ({6}, {7}, {8}))",
                a.x, a.y, a.z,
                b.x, b.y, b.z,
                c.x, c.y, c.z);
        }

        public void identity()
        {
            a = new Vector3(1, 0, 0);
            b = new Vector3(0, 1, 0);
            c = new Vector3(0, 0, 1);
        }

        public Matrix3 transposed()
        {
            return new Matrix3(new Vector3(a.x, b.x, c.x),
                new Vector3(a.y, b.y, c.y),
                new Vector3(a.z, b.z, c.z));
        }


        public void from_euler(double roll, double pitch, double yaw)
        {
            // '''fill the matrix from Euler angles in radians'''
            double cp = Utils.cos(pitch);
            double sp = Utils.sin(pitch);
            double sr = Utils.sin(roll);
            double cr = Utils.cos(roll);
            double sy = Utils.sin(yaw);
            double cy = Utils.cos(yaw);

            a.x = cp*cy;
            a.y = (sr*sp*cy) - (cr*sy);
            a.z = (cr*sp*cy) + (sr*sy);
            b.x = cp*sy;
            b.y = (sr*sp*sy) + (cr*cy);
            b.z = (cr*sp*sy) - (sr*cy);
            c.x = -sp;
            c.y = sr*cp;
            c.z = cr*cp;
        }

        public void to_euler(ref double roll, ref double pitch, ref double yaw)
        {
            // '''find Euler angles for the matrix'''
            if (c.x >= 1.0)
                pitch = Math.PI;
            else if (c.x <= -1.0)
                pitch = -Math.PI;
            else
                pitch = -Utils.asin(c.x);
            roll = Utils.atan2(c.y, c.z);
            yaw = Utils.atan2(b.x, a.x);
            //return (roll, pitch, yaw)
        }

        public Vector3 to_euler312()
        {
            return new Vector3(Utils.asin(c.y),
                Utils.atan2(-c.x, c.z),
                Utils.atan2(-a.y, b.y));
        }

        public void from_euler312(double roll, double pitch, double yaw)
        {
            double cp = Utils.cosf(pitch);
            double sp = Utils.sinf(pitch);
            double sr = Utils.sinf(roll);
            double cr = Utils.cosf(roll);
            double sy = Utils.sinf(yaw);
            double cy = Utils.cosf(yaw);

            a.x = cy*cp - sy*sr*sp;
            b.y = cy*cr;
            c.z = cp*cr;
            a.y = -cr*sy;
            a.z = sp*cy + cp*sr*sy;
            b.x = cp*sy + sp*sr*cy;
            b.z = sy*sp - sr*cy*cp;
            c.x = -sp*cr;
            c.y = sr;
        }

        public Vector3 to_euler_yxz()
        {
            double cos_phi = Math.Sqrt(1 - c.y*c.y);
            double phi = Utils.atan2(c.y, cos_phi);
            double omega = Utils.atan2(-c.x, c.z);
            double kappa = Utils.atan2(-a.y, b.y);

            return new Vector3(omega, phi, kappa);
        }


        public static Matrix3 operator +(Matrix3 self, Matrix3 m)
        {
            return new Matrix3(self.a + m.a, self.b + m.b, self.c + m.c);
        }

        public static Matrix3 operator -(Matrix3 self, Matrix3 m)
        {
            return new Matrix3(self.a - m.a, self.b - m.b, self.c - m.c);
        }

        public static Vector3 operator *(Matrix3 self, Vector3 v)
        {
            return new Vector3(self.a.x*v.x + self.a.y*v.y + self.a.z*v.z,
                self.b.x*v.x + self.b.y*v.y + self.b.z*v.z,
                self.c.x*v.x + self.c.y*v.y + self.c.z*v.z);
        }

        public static Matrix3 operator *(Matrix3 self, Matrix3 m)
        {
            return new Matrix3(new Vector3(self.a.x*m.a.x + self.a.y*m.b.x + self.a.z*m.c.x,
                self.a.x*m.a.y + self.a.y*m.b.y + self.a.z*m.c.y,
                self.a.x*m.a.z + self.a.y*m.b.z + self.a.z*m.c.z),
                new Vector3(self.b.x*m.a.x + self.b.y*m.b.x + self.b.z*m.c.x,
                    self.b.x*m.a.y + self.b.y*m.b.y + self.b.z*m.c.y,
                    self.b.x*m.a.z + self.b.y*m.b.z + self.b.z*m.c.z),
                new Vector3(self.c.x*m.a.x + self.c.y*m.b.x + self.c.z*m.c.x,
                    self.c.x*m.a.y + self.c.y*m.b.y + self.c.z*m.c.y,
                    self.c.x*m.a.z + self.c.y*m.b.z + self.c.z*m.c.z));
        }

        public static Matrix3 operator *(Matrix3 self, double v)
        {
            return new Matrix3(self.a*v, self.b*v, self.c*v);
        }

        public static Matrix3 operator /(Matrix3 self, double v)
        {
            return new Matrix3(self.a/v, self.b/v, self.c/v);
        }

        public static Matrix3 operator -(Matrix3 self)
        {
            return new Matrix3(-self.a, -self.b, -self.c);
        }

        public Matrix3 copy()
        {
            return new Matrix3(a.copy(), b.copy(), c.copy());
        }

        public void rotate(Vector3 g)
        {
            //   '''rotate the matrix by a given amount on 3 axes'''
            Matrix3 temp_matrix = new Matrix3();

            temp_matrix.a.x = a.y * g.z - a.z * g.y;
            temp_matrix.a.y = a.z * g.x - a.x * g.z;
            temp_matrix.a.z = a.x * g.y - a.y * g.x;
            temp_matrix.b.x = b.y * g.z - b.z * g.y;
            temp_matrix.b.y = b.z * g.x - b.x * g.z;
            temp_matrix.b.z = b.x * g.y - b.y * g.x;
            temp_matrix.c.x = c.y * g.z - c.z * g.y;
            temp_matrix.c.y = c.z * g.x - c.x * g.z;
            temp_matrix.c.z = c.x * g.y - c.y * g.x;

            this.a += temp_matrix.a;
            this.b += temp_matrix.b;
            this.c += temp_matrix.c;
        }

        public void rotateXY(Vector3 g)
        {
            Matrix3 temp_matrix = new Matrix3();
            temp_matrix.a.x = -a.z*g.y;
            temp_matrix.a.y = a.z*g.x;
            temp_matrix.a.z = a.x*g.y - a.y*g.x;
            temp_matrix.b.x = -b.z*g.y;
            temp_matrix.b.y = b.z*g.x;
            temp_matrix.b.z = b.x*g.y - b.y*g.x;
            temp_matrix.c.x = -c.z*g.y;
            temp_matrix.c.y = c.z*g.x;
            temp_matrix.c.z = c.x*g.y - c.y*g.x;

            this.a += temp_matrix.a;
            this.b += temp_matrix.b;
            this.c += temp_matrix.c;
        }

        public  void rotateXYinv(Vector3 g)
        {
            Matrix3 temp_matrix = new Matrix3();
            temp_matrix.a.x = a.z*g.y;
            temp_matrix.a.y = -a.z*g.x;
            temp_matrix.a.z = -a.x*g.y + a.y*g.x;
            temp_matrix.b.x = b.z*g.y;
            temp_matrix.b.y = -b.z*g.x;
            temp_matrix.b.z = -b.x*g.y + b.y*g.x;
            temp_matrix.c.x = c.z*g.y;
            temp_matrix.c.y = -c.z*g.x;
            temp_matrix.c.z = -c.x*g.y + c.y*g.x;

            this.a += temp_matrix.a;
            this.b += temp_matrix.b;
            this.c += temp_matrix.c;
        }

        public void normalize()
        {
            //  '''re-normalise a rotation matrix'''
            double error = a*b;
            Vector3 t0 = a - (b*(0.5*error));
            Vector3 t1 = b - (a*(0.5*error));
            Vector3 t2 = t0%t1;
            a = t0*(1.0/t0.length());
            b = t1*(1.0/t1.length());
            c = t2*(1.0/t2.length());
        }

        public double trace()
        {
            //  '''the trace of the matrix'''
            return a.x + b.y + c.z;
        }
    }
}