using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using MissionPlanner.Utilities;

namespace MissionPlanner.HIL
{
    public class Vector3
    {
        Vector3 self;
        public double x;
        public double y;
        public double z;

        public Vector3(double x = 0, double y = 0, double z = 0)
        {
            self = this;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(Vector3 copyme)
        {
            self = this;
            this.x = copyme.x;
            this.y = copyme.y;
            this.z = copyme.z;
        }

        public new string ToString()
        {
            return String.Format("Vector3({0}, {1}, {2})", self.x,
                self.y,
                self.z);
        }

        public static implicit operator Vector3(PointLatLngAlt a)
        {
            return new Vector3(a.Lat, a.Lng, a.Alt);
        }

        public static implicit operator Vector3(PointLatLng a)
        {
            return new Vector3(a.Lat, a.Lng, 0);
        }

        public static implicit operator PointLatLng(Vector3 a)
        {
            return new PointLatLng(a.x, a.y);
        }

        public static Vector3 operator +(Vector3 self, Vector3 v)
        {
            return new Vector3(self.x + v.x,
                self.y + v.y,
                self.z + v.z);
        }


        public static Vector3 operator -(Vector3 self, Vector3 v)
        {
            return new Vector3(self.x - v.x,
                self.y - v.y,
                self.z - v.z);
        }

        public static Vector3 operator -(Vector3 self)
        {
            return new Vector3(-self.x, -self.y, -self.z);
        }


        public static Vector3 operator *(Vector3 self, Vector3 v)
        {
            //  '''dot product'''
            return new Vector3(self.x*v.x + self.y*v.y + self.z*v.z);
        }

        public static Vector3 operator *(Vector3 self, double v)
        {
            return new Vector3(self.x*v,
                self.y*v,
                self.z*v);
        }

        public static Vector3 operator *(double v, Vector3 self)
        {
            return (self*v);
        }

        public static Vector3 operator /(Vector3 self, double v)
        {
            return new Vector3(self.x/v,
                self.y/v,
                self.z/v);
        }

        public static Vector3 operator %(Vector3 self, Vector3 v)
        {
            //  '''cross product'''
            return new Vector3(self.y*v.z - self.z*v.y,
                self.z*v.x - self.x*v.z,
                self.x*v.y - self.y*v.x);
        }

        public Vector3 copy()
        {
            return new Vector3(self.x, self.y, self.z);
        }


        public double length()
        {
            return Math.Sqrt(self.x*self.x + self.y*self.y + self.z*self.z);
        }

        public void zero()
        {
            self.x = self.y = self.z = 0;
        }

        //public double angle (Vector3 self, Vector3 v) {
        //   '''return the angle between this vector and another vector'''
        //  return Math.Acos(self * v) / (self.length() * v.length());
        //}

        public Vector3 normalized()
        {
            return self/self.length();
        }

        public void normalize()
        {
            Vector3 v = self.normalized();
            self.x = v.x;
            self.y = v.y;
            self.z = v.z;
        }

        const double HALF_SQRT_2 = 0.70710678118654757;

        public Vector3 rotate(Common.Rotation rotation)
        {
            double tmp;
            switch (rotation)
            {
                case Common.Rotation.ROTATION_NONE:
                case Common.Rotation.ROTATION_MAX:
                    return this;
                case Common.Rotation.ROTATION_YAW_45:
                {
                    tmp = HALF_SQRT_2*(x - y);
                    y = HALF_SQRT_2*(x + y);
                    x = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_YAW_90:
                {
                    tmp = x;
                    x = -y;
                    y = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_YAW_135:
                {
                    tmp = -HALF_SQRT_2*(x + y);
                    y = HALF_SQRT_2*(x - y);
                    x = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_YAW_180:
                    x = -x;
                    y = -y;
                    return this;
                case Common.Rotation.ROTATION_YAW_225:
                {
                    tmp = HALF_SQRT_2*(y - x);
                    y = -HALF_SQRT_2*(x + y);
                    x = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_YAW_270:
                {
                    tmp = x;
                    x = y;
                    y = -tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_YAW_315:
                {
                    tmp = HALF_SQRT_2*(x + y);
                    y = HALF_SQRT_2*(y - x);
                    x = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_180:
                {
                    y = -y;
                    z = -z;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_180_YAW_45:
                {
                    tmp = HALF_SQRT_2*(x + y);
                    y = HALF_SQRT_2*(x - y);
                    x = tmp;
                    z = -z;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_180_YAW_90:
                {
                    tmp = x;
                    x = y;
                    y = tmp;
                    z = -z;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_180_YAW_135:
                {
                    tmp = HALF_SQRT_2*(y - x);
                    y = HALF_SQRT_2*(y + x);
                    x = tmp;
                    z = -z;
                    return this;
                }
                case Common.Rotation.ROTATION_PITCH_180:
                {
                    x = -x;
                    z = -z;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_180_YAW_225:
                {
                    tmp = -HALF_SQRT_2*(x + y);
                    y = HALF_SQRT_2*(y - x);
                    x = tmp;
                    z = -z;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_180_YAW_270:
                {
                    tmp = x;
                    x = -y;
                    y = -tmp;
                    z = -z;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_180_YAW_315:
                {
                    tmp = HALF_SQRT_2*(x - y);
                    y = -HALF_SQRT_2*(x + y);
                    x = tmp;
                    z = -z;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_90:
                {
                    tmp = z;
                    z = y;
                    y = -tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_90_YAW_45:
                {
                    tmp = z;
                    z = y;
                    y = -tmp;
                    tmp = HALF_SQRT_2*(x - y);
                    y = HALF_SQRT_2*(x + y);
                    x = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_90_YAW_90:
                {
                    tmp = z;
                    z = y;
                    y = -tmp;
                    tmp = x;
                    x = -y;
                    y = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_90_YAW_135:
                {
                    tmp = z;
                    z = y;
                    y = -tmp;
                    tmp = -HALF_SQRT_2*(x + y);
                    y = HALF_SQRT_2*(x - y);
                    x = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_270:
                {
                    tmp = z;
                    z = -y;
                    y = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_270_YAW_45:
                {
                    tmp = z;
                    z = -y;
                    y = tmp;
                    tmp = HALF_SQRT_2*(x - y);
                    y = HALF_SQRT_2*(x + y);
                    x = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_270_YAW_90:
                {
                    tmp = z;
                    z = -y;
                    y = tmp;
                    tmp = x;
                    x = -y;
                    y = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_ROLL_270_YAW_135:
                {
                    tmp = z;
                    z = -y;
                    y = tmp;
                    tmp = -HALF_SQRT_2*(x + y);
                    y = HALF_SQRT_2*(x - y);
                    x = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_PITCH_90:
                {
                    tmp = z;
                    z = -x;
                    x = tmp;
                    return this;
                }
                case Common.Rotation.ROTATION_PITCH_270:
                {
                    tmp = z;
                    z = x;
                    x = -tmp;
                    return this;
                }
            }
            throw new Exception("Invalid Rotation");
            //return this;
        }
    }
}