using System;
using System.Drawing;

namespace MissionPlanner.Utilities.Drawing
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

        public RectangleF Rectangle { get; set; }
        public Color[] LinearColors { get; set; }
    }
}