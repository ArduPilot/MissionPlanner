using System;
using System.Runtime.InteropServices;

namespace GDAL
{
    [StructLayout(LayoutKind.Explicit, Size = (3 * 8))]
    public struct point
    {
        [FieldOffset(0)]
        public double x;
        [FieldOffset(8)]
        public double y;
        [FieldOffset(16)]
        public double z;

        public point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public point(double[] pnt)
        {
            this.x = pnt[0];
            this.y = pnt[1];
            this.z = pnt[2];
        }

        public static implicit operator point(double[] a)
        {
            return new point(a);
        }

        public static implicit operator double[](point a)
        {
            return new[] { a.x, a.y, a.z };
        }

        public override string ToString()
        {
            return String.Format("point ({0},{1},{2})", x, y, z);
        }
    }
}