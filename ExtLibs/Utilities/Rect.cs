using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public struct Rect
    {
        public double Top;
        public double Bottom;
        public double Left;
        public double Right;

        public double Width { get { return Right - Left; } }
        public double Height { get { return Top - Bottom; } }

        public double MidWidth { get { return ((Right - Left) / 2) + Left; } }
        public double MidHeight { get { return ((Top - Bottom) / 2) + Bottom; } }

        public Rect(double Left, double Top, double Width, double Height)
        {
            this.Left = Left;
            this.Top = Top;
            this.Right = Left + Width;
            this.Bottom = Top + Height;
        }

        public double DiagDistance()
        {
            // pythagarus
            return Math.Sqrt(Math.Pow(Width, 2) + Math.Pow(Height, 2));
        }

    }

}
