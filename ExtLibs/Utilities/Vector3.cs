using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using MissionPlanner.Utilities;
using Newtonsoft.Json;

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
        public Vector3(double x = 0, double y = 0, double z = 0) : base(x, y, z) { }

        public Vector3(Vector3<double> copyme) : base(copyme) { }
    }

    public class Vector3<T> where T: struct
    {
        public T x;
        public T y;
        public T z;

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

        public Vector3(Tuple<T,T,T> inp)
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

        public new string ToString()
        {
            return String.Format("Vector3<T>({0}, {1}, {2})", x,
                y,
                z);
        }

        public static implicit operator Vector3<T>(PointLatLngAlt a)
        {
            return new Vector3<T>((dynamic)a.Lat, (dynamic)a.Lng, (dynamic)a.Alt);
        }

        public static implicit operator Vector3<T>(PointLatLng a)
        {
            return new Vector3<T>((dynamic)a.Lat, (dynamic)a.Lng, (dynamic)0);
        }

        public static implicit operator PointLatLng(Vector3<T> a)
        {
            return new PointLatLng((dynamic)a.x, (dynamic)a.y);
        }

        public static implicit operator T[](Vector3<T> a)
        {
            return new T[] { a.x, a.y, a.z};
        }

        public static implicit operator Vector3(Vector3<T> a)
        {
            return new Vector3((double) (dynamic) a.x, (double) (dynamic) a.y, (double) (dynamic) a.z);
        }

        public static implicit operator Vector3f(Vector3<T> a)
        {
            var x = (float)(dynamic)a.x ;
            var y = (float)(dynamic)a.y ;
            var z = (float)(dynamic)a.z ;

            return new Vector3f(x,y,z);
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
            return new Vector3<T>((T)((dynamic)self.x + v.x),
                (T)((dynamic)self.y + v.y),
                (T)((dynamic)self.z + v.z));
        }


        public static Vector3<T> operator -(Vector3<T> self, Vector3<T> v)
        {
            return new Vector3<T>((T)((dynamic)self.x - v.x),
                (T)((dynamic)self.y - v.y),
                (T)((dynamic)self.z - v.z));
        }

        public static Vector3<T> operator -(Vector3<T> self)
        {
            return new Vector3<T>((T)(-(dynamic)self.x), (T)(-(dynamic)self.y), (T)(-(dynamic)self.z));
        }

        public static double operator *(Vector3<T> self, Vector3<T> v)
        {
            //  '''dot product'''
            return ((dynamic)self.x*v.x + (dynamic)self.y*v.y + (dynamic)self.z*v.z);
        }

        public static Vector3<T> operator *(Vector3<T> self, double v)
        {
            return new Vector3<T>((T)((dynamic)self.x*v),
                (T)((dynamic)self.y*v),
                (T)((dynamic)self.z*v));
        }

        public static Vector3<T> operator *(double v, Vector3<T> self)
        {
            return (self*v);
        }

        public static Vector3<T> operator /(Vector3<T> self, double v)
        {
            return new Vector3<T>((T)((dynamic)self.x/v),
                (T)((dynamic)self.y/v),
                (T)((dynamic)self.z/v));
        }

        public static Vector3<T> operator %(Vector3<T> self, Vector3<T> v)
        {
            //  '''cross product'''
            return new Vector3<T>((T)((dynamic)self.y*v.z - (dynamic)self.z*v.y),
                (T)((dynamic)self.z*v.x - (dynamic)self.x*v.z),
                (T)((dynamic)self.x*v.y - (dynamic)self.y*v.x));
        }

        public Vector3<T> copy()
        {
            return new Vector3<T>(x, y, z);
        }


        public T length()
        {
            return (T)Math.Sqrt((dynamic) x * x + (dynamic) y * y + (dynamic) z * z);
        }

        public void zero()
        {
            x = y = z = (dynamic)0;
        }

        //public double angle (Vector3<T> self, Vector3<T> v) {
        //   '''return the angle between this vector and another vector'''
        //  return Math.Acos(self * v) / (self.length() * v.length());
        //}

        public Vector3<T> normalized()
        {
            return (dynamic)this/length();
        }

        public void normalize()
        {
            Vector3<T> v = normalized();
            x = v.x;
            y = v.y;
            z = v.z;
        }

        private T HALF_SQRT_2
        {
            get { return (T)(dynamic)0.70710678118654757; }
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
                    tmp = HALF_SQRT_2*((dynamic)x - y);
                    y = HALF_SQRT_2*((dynamic)x + y);
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_90:
                {
                    tmp = x;
                    x = -(dynamic)y;
                    y = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_135:
                {
                    tmp = -(dynamic)HALF_SQRT_2 *((dynamic)x + y);
                    y = HALF_SQRT_2*((dynamic)x - y);
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_180:
                    x = -(dynamic)x;
                    y = -(dynamic)y;
                    return this;
                case Rotation.ROTATION_YAW_225:
                {
                    tmp = HALF_SQRT_2*((dynamic)y - x);
                    y = -(dynamic)HALF_SQRT_2 *((dynamic)x + y);
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_270:
                {
                    tmp = x;
                    x = y;
                    y = -(dynamic)tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_315:
                {
                    tmp = HALF_SQRT_2*((dynamic)x + y);
                    y = HALF_SQRT_2*((dynamic)y - x);
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_180:
                {
                    y = -(dynamic)y;
                    z = -(dynamic)z;
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_45:
                {
                    tmp = HALF_SQRT_2*((dynamic)x + y);
                    y = HALF_SQRT_2*((dynamic)x - y);
                    x = tmp;
                    z = -(dynamic)z;
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_90:
                {
                    tmp = x;
                    x = y;
                    y = tmp;
                    z = -(dynamic)z;
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_135:
                {
                    tmp = HALF_SQRT_2*((dynamic)y - x);
                    y = HALF_SQRT_2*((dynamic)y + x);
                    x = tmp;
                    z = -(dynamic)z;
                    return this;
                }
                case Rotation.ROTATION_PITCH_180:
                {
                    x = -(dynamic)x;
                    z = -(dynamic)z;
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_225:
                {
                    tmp = -(dynamic)HALF_SQRT_2 *((dynamic)x + y);
                    y = HALF_SQRT_2*((dynamic)y - x);
                    x = tmp;
                    z = -(dynamic)z;
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_270:
                {
                    tmp = x;
                    x = -(dynamic)y;
                    y = -(dynamic)tmp;
                    z = -(dynamic)z;
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_315:
                {
                    tmp = HALF_SQRT_2*((dynamic)x - y);
                    y = -(dynamic)HALF_SQRT_2 *((dynamic)x + y);
                    x = tmp;
                    z = -(dynamic)z;
                    return this;
                }
                case Rotation.ROTATION_ROLL_90:
                {
                    tmp = z;
                    z = y;
                    y = -(dynamic)tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_90_YAW_45:
                {
                    tmp = z;
                    z = y;
                    y = -(dynamic)tmp;
                    tmp = HALF_SQRT_2*((dynamic)x - y);
                    y = HALF_SQRT_2*((dynamic)x + y);
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_90_YAW_90:
                {
                    tmp = z;
                    z = y;
                    y = -(dynamic)tmp;
                    tmp = x;
                    x = -(dynamic)y;
                    y = tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_90_YAW_135:
                {
                    tmp = z;
                    z = y;
                    y = -(dynamic)tmp;
                    tmp = -(dynamic)HALF_SQRT_2 *((dynamic)x + y);
                    y = HALF_SQRT_2*((dynamic)x - y);
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270:
                {
                    tmp = z;
                    z = -(dynamic)y;
                    y = tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270_YAW_45:
                {
                    tmp = z;
                    z = -(dynamic)y;
                    y = tmp;
                    tmp = HALF_SQRT_2*((dynamic)x - y);
                    y = HALF_SQRT_2*((dynamic)x + y);
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270_YAW_90:
                {
                    tmp = z;
                    z = -(dynamic)y;
                    y = tmp;
                    tmp = x;
                    x = -(dynamic)y;
                    y = tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270_YAW_135:
                {
                    tmp = z;
                    z = -(dynamic)y;
                    y = tmp;
                    tmp = -(dynamic)HALF_SQRT_2 *((dynamic)x + y);
                    y = HALF_SQRT_2*((dynamic)x - y);
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_PITCH_90:
                {
                    tmp = z;
                    z = -(dynamic)x;
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_PITCH_270:
                {
                    tmp = z;
                    z = x;
                    x = -(dynamic)tmp;
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