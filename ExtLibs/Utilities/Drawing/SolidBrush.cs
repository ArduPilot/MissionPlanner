using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class SolidBrush : Brush
    {
        public SolidBrush(): this(Color.Black)
        {
        }

        public SolidBrush(Color color)
        {
            nativeBrush = new SKPaint() {Color = color.SKColor()};
        }
    }
}
