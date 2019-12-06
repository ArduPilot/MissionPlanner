using GMap.NET;
using Newtonsoft.Json;
using System;

namespace MissionPlanner.Utilities
{
    public class Vector3f : Vector3<float>
    {
        public static readonly Vector3f Zero = new Vector3f(0, 0, 0);
        public static readonly Vector3f One = new Vector3f(1.0f, 1.0f, 1.0f);

        [JsonConstructor]
        public Vector3f(float x = 0, float y = 0, float z = 0) : base(x, y, z)
        {
        }
    }

    public class Vector3 : Vector3<double>
    {
        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        public static readonly Vector3 One = new Vector3(1.0, 1.0, 1.0);

        [JsonConstructor]
        public Vector3(double x = 0, double y = 0, double z = 0) : base(x, y, z)
        {
        }

        public Vector3(Vector3<double> copyme) : base(copyme)
        {
        }
    }

    public class Vector3<T> where T : struct, IConvertible
    {
        public T x;
        public T y;
        public T z;

        public double xd
        {
            get { return (double) (IConvertible) x; }
        }

        public double yd
        {
            get { return (double) (IConvertible) y; }
        }

        public double zd
        {
            get { return (double) (IConvertible) z; }
        }

        [JsonIgnore]
        public T X
        {
            get { return x; }
            set { x = value; }
        }

        [JsonIgnore]
        public T Y
        {
            get { return y; }
            set { y = value; }
        }

        [JsonIgnore]
        public T Z
        {
            get { return z; }
            set { z = value; }
        }

        [JsonConstructor]
        public Vector3(T x, T y, T z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(Tuple<T, T, T> inp)
        {
            this.x = inp.Item1;
            this.y = inp.Item2;
            this.z = inp.Item3;
        }

        public Vector3(Vector3<T> copyme)
        {
            this.x = copyme.x;
            this.y = copyme.y;
            this.z = copyme.z;
        }

        private Vector3(double x, double y, double z)
        {
            this.x = (T) (IConvertible) x;
            this.y = (T) (IConvertible) y;
            this.z = (T) (IConvertible) z;
        }

        public new string ToString()
        {
            return String.Format("Vector3<T>({0}, {1}, {2})", x,
                y,
                z);
        }

        public static implicit operator Vector3<T>(PointLatLngAlt a)
        {
            return new Vector3<T>((T) (IConvertible) a.Lat, (T) (IConvertible) a.Lng, (T) (IConvertible) a.Alt);
        }

        public static implicit operator Vector3<T>(PointLatLng a)
        {
            return new Vector3<T>((T) (IConvertible) a.Lat, (T) (IConvertible) a.Lng, (T) (IConvertible) 0);
        }

        public static implicit operator PointLatLng(Vector3<T> a)
        {
            return new PointLatLng((double) (IConvertible) a.x, (double) (IConvertible) a.y);
        }

        public static implicit operator T[](Vector3<T> a)
        {
            return new T[] {a.x, a.y, a.z};
        }

        public static implicit operator Vector3(Vector3<T> a)
        {
            return new Vector3((double) (IConvertible) a.x, (double) (IConvertible) a.y, (double) (IConvertible) a.z);
        }

        public static implicit operator Vector3f(Vector3<T> a)
        {
            var x = (float) (IConvertible) a.x;
            var y = (float) (IConvertible) a.y;
            var z = (float) (IConvertible) a.z;

            return new Vector3f(x, y, z);
        }

        public T this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                if (index == 1)
                    return y;
                if (index == 2)
                    return z;

                throw new Exception("Bad index");
            }
            set
            {
                if (index == 0)
                    x = value;
                if (index == 1)
                    y = value;
                if (index == 2)
                    z = value;

                throw new Exception("Bad index");
            }
        }

        public static Vector3<T> operator +(Vector3<T> self, Vector3<T> v)
        {
            return new Vector3<T>((T) (IConvertible) (self.xd + v.xd),
                (T) (IConvertible) (self.yd + v.yd),
                (T) (IConvertible) (self.zd + v.zd));
        }

        public static Vector3<T> operator -(Vector3<T> self, Vector3<T> v)
        {
            return new Vector3<T>((T) (IConvertible) (self.xd - v.xd),
                (T) (IConvertible) (self.yd - v.yd),
                (T) (IConvertible) (self.zd - v.zd));
        }

        public static Vector3<T> operator -(Vector3<T> self)
        {
            return new Vector3<T>((T) (IConvertible) (-self.xd), (T) (IConvertible) (-self.yd),
                (T) (IConvertible) (-self.zd));
        }

        public static double operator *(Vector3<T> self, Vector3<T> v)
        {
            //  '''dot product'''
            return (self.xd * v.xd + self.yd * v.yd + self.zd * v.zd);
        }

        public static Vector3<T> operator *(Vector3<T> self, double v)
        {
            return new Vector3<T>(self.xd * v,
                self.yd * v,
                self.zd * v);
        }

        public static Vector3<T> operator *(double v, Vector3<T> self)
        {
            return (self * v);
        }

        public static Vector3<T> operator /(Vector3<T> self, double v)
        {
            return new Vector3<T>(self.xd / v,
                self.yd / v,
                self.zd / v);
        }

        public static Vector3<T> operator %(Vector3<T> self, Vector3<T> v)
        {
            //  '''cross product'''
            return new Vector3<T>((self.yd * v.zd - self.zd * v.yd),
                (self.zd * v.xd - self.xd * v.zd),
                (self.xd * v.yd - self.yd * v.xd));
        }

        public Vector3<T> copy()
        {
            return new Vector3<T>(x, y, z);
        }

        public T length()
        {
            return (T) (IConvertible) Math.Sqrt((xd * xd + yd * yd + zd * zd));
        }

        public void zero()
        {
            x = y = z = (T) (IConvertible) 0;
        }

        //public double angle (Vector3<T> self, Vector3<T> v) {
        //   '''return the angle between this vector and another vector'''
        //  return Math.Acos(self * v) / (self.length() * v.length());
        //}

        public Vector3<T> normalized()
        {
            return this / (double) (IConvertible) length();
        }

        public void normalize()
        {
            Vector3<T> v = normalized();
            x = v.x;
            y = v.y;
            z = v.z;
        }

        private double HALF_SQRT_2
        {
            get { return 0.70710678118654757; }
        }

        public Vector3<T> rotate(Rotation rotation)
        {
            T tmp;
            switch (rotation)
            {
                case Rotation.ROTATION_NONE:
                case Rotation.ROTATION_MAX:
                    return this;

                case Rotation.ROTATION_YAW_45:
                {
                    tmp = (T) (IConvertible) (HALF_SQRT_2 * (xd - yd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (xd + yd));
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_90:
                {
                    tmp = x;
                    x = (T) (IConvertible) (-yd);
                    y = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_135:
                {
                    tmp = (T) (IConvertible) (-HALF_SQRT_2 * (xd + yd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (xd - yd));
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_180:
                    x = (T) (IConvertible) (-xd);
                    y = (T) (IConvertible) (-yd);
                    return this;

                case Rotation.ROTATION_YAW_225:
                {
                    tmp = (T) (IConvertible) (HALF_SQRT_2 * (yd - xd));
                    y = (T) (IConvertible) (-HALF_SQRT_2 * (xd + yd));
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_270:
                {
                    tmp = x;
                    x = y;
                    y = (T) (IConvertible) (-(double) (IConvertible) tmp);
                    return this;
                }
                case Rotation.ROTATION_YAW_315:
                {
                    tmp = (T) (IConvertible) (HALF_SQRT_2 * (xd + yd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (yd - xd));
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_180:
                {
                    y = (T) (IConvertible) (-yd);
                    z = (T) (IConvertible) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_45:
                {
                    tmp = (T) (IConvertible) (HALF_SQRT_2 * (xd + yd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (xd - yd));
                    x = tmp;
                    z = (T) (IConvertible) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_90:
                {
                    tmp = x;
                    x = y;
                    y = tmp;
                    z = (T) (IConvertible) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_135:
                {
                    tmp = (T) (IConvertible) (HALF_SQRT_2 * (yd - xd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (yd + xd));
                    x = tmp;
                    z = (T) (IConvertible) (-zd);
                    return this;
                }
                case Rotation.ROTATION_PITCH_180:
                {
                    x = (T) (IConvertible) (-xd);
                    z = (T) (IConvertible) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_225:
                {
                    tmp = (T) (IConvertible) (-HALF_SQRT_2 * (xd + yd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (yd - xd));
                    x = (T) (IConvertible) tmp;
                    z = (T) (IConvertible) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_270:
                {
                    tmp = (T) (IConvertible) xd;
                    x = (T) (IConvertible) (-yd);
                    y = (T) (IConvertible) (-(double) (IConvertible) tmp);
                    z = (T) (IConvertible) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_315:
                {
                    tmp = (T) (IConvertible) (HALF_SQRT_2 * (xd - yd));
                    y = (T) (IConvertible) (-HALF_SQRT_2 * (xd + yd));
                    x = (T) (IConvertible) tmp;
                    z = (T) (IConvertible) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_90:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) yd;
                    y = (T) (IConvertible) (-(double) (IConvertible) tmp);
                    return this;
                }
                case Rotation.ROTATION_ROLL_90_YAW_45:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) yd;
                    y = (T) (IConvertible) (-(double) (IConvertible) tmp);
                    tmp = (T) (IConvertible) (HALF_SQRT_2 * (xd - yd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (xd + yd));
                    x = (T) (IConvertible) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_90_YAW_90:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) yd;
                    y = (T) (IConvertible) (-(double) (IConvertible) tmp);
                    tmp = (T) (IConvertible) xd;
                    x = (T) (IConvertible) (-yd);
                    y = (T) (IConvertible) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_90_YAW_135:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) yd;
                    y = (T) (IConvertible) (-(double) (IConvertible) tmp);
                    tmp = (T) (IConvertible) (-HALF_SQRT_2 * (xd + yd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (xd - yd));
                    x = (T) (IConvertible) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) (-yd);
                    y = (T) (IConvertible) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270_YAW_45:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) (-yd);
                    y = (T) (IConvertible) tmp;
                    tmp = (T) (IConvertible) (HALF_SQRT_2 * (xd - yd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (xd + yd));
                    x = (T) (IConvertible) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270_YAW_90:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) (-yd);
                    y = (T) (IConvertible) tmp;
                    tmp = (T) (IConvertible) xd;
                    x = (T) (IConvertible) (-yd);
                    y = (T) (IConvertible) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270_YAW_135:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) (-yd);
                    y = (T) (IConvertible) tmp;
                    tmp = (T) (IConvertible) (-HALF_SQRT_2 * (xd + yd));
                    y = (T) (IConvertible) (HALF_SQRT_2 * (xd - yd));
                    x = (T) (IConvertible) tmp;
                    return this;
                }
                case Rotation.ROTATION_PITCH_90:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) (-xd);
                    x = (T) (IConvertible) tmp;
                    return this;
                }
                case Rotation.ROTATION_PITCH_270:
                {
                    tmp = (T) (IConvertible) zd;
                    z = (T) (IConvertible) xd;
                    x = (T) (IConvertible) (-(double) (IConvertible) tmp);
                    return this;
                }
            }

            throw new Exception("Invalid Rotation");
            //return this;
        }
    }

    /// <summary>
    /// from libraries\AP_Math\rotations.h
    /// </summary>
    public enum Rotation
    {
        ROTATION_NONE = 0,
        ROTATION_YAW_45,
        ROTATION_YAW_90,
        ROTATION_YAW_135,
        ROTATION_YAW_180,
        ROTATION_YAW_225,
        ROTATION_YAW_270,
        ROTATION_YAW_315,
        ROTATION_ROLL_180,
        ROTATION_ROLL_180_YAW_45,
        ROTATION_ROLL_180_YAW_90,
        ROTATION_ROLL_180_YAW_135,
        ROTATION_PITCH_180,
        ROTATION_ROLL_180_YAW_225,
        ROTATION_ROLL_180_YAW_270,
        ROTATION_ROLL_180_YAW_315,
        ROTATION_ROLL_90,
        ROTATION_ROLL_90_YAW_45,
        ROTATION_ROLL_90_YAW_90,
        ROTATION_ROLL_90_YAW_135,
        ROTATION_ROLL_270,
        ROTATION_ROLL_270_YAW_45,
        ROTATION_ROLL_270_YAW_90,
        ROTATION_ROLL_270_YAW_135,
        ROTATION_PITCH_90,
        ROTATION_PITCH_270,
        ROTATION_MAX
    }
}