using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Geometry
{
    public class Calc
    {
        public static PointE PointBetween(PointE p1, PointE p2, double frac) {
            return new PointE(p1.X + (p2.X - p1.X) * frac, p1.Y + (p2.Y - p1.Y) * frac);
        }
    }
}
