using System;
using System.Drawing;

namespace Transitions
{
    internal class ManagedType_Color : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(Color);
        }

        public object copy(object o)
        {
            return Color.FromArgb(((Color) o).ToArgb());
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            var color1 = (Color) start;
            var color2 = (Color) end;
            var r1 = (int) color1.R;
            var g1 = (int) color1.G;
            var b1 = (int) color1.B;
            var a1 = (int) color1.A;
            var r2 = (int) color2.R;
            var g2 = (int) color2.G;
            var b2 = (int) color2.B;
            var a2 = (int) color2.A;
            var red = Utility.interpolate(r1, r2, dPercentage);
            var green = Utility.interpolate(g1, g2, dPercentage);
            var blue = Utility.interpolate(b1, b2, dPercentage);
            return Color.FromArgb(Utility.interpolate(a1, a2, dPercentage), red, green, blue);
        }
    }
}