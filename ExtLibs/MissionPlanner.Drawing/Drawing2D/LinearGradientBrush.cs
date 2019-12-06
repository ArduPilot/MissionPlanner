﻿using System.Drawing;

namespace MissionPlanner.Drawing.Drawing2D
{
    public class LinearGradientBrush : Brush
    {
        private LinearGradientMode _gradMode;

        public LinearGradientBrush(RectangleF bg, Color skyColor1, Color skyColor2, LinearGradientMode gradMode)
        {
            _gradMode = gradMode;
            Rectangle = bg;
            LinearColors = new[] {skyColor1, skyColor2};
        }

        public LinearGradientBrush(Point TL, Point BR, Color skyColor1, Color skyColor2)
        {
            _gradMode = LinearGradientMode.Vertical;
            Rectangle = new RectangleF(TL, new SizeF(BR.X, BR.Y));
            LinearColors = new[] { skyColor1, skyColor2 };
        }

        public RectangleF Rectangle { get; set; }
        public Color[] LinearColors { get; set; }
    }
}