using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public static class MathHelper
    {
        public const double rad2deg = (180 / Math.PI);
        public const double deg2rad = (1.0 / rad2deg);

        public static double Degrees(double rad)
        {
            return rad * rad2deg;
        }

        public static double Radians(double deg)
        {
            return deg * deg2rad;
        }

        public static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }
}
