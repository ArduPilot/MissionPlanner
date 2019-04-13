using System;
using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class Pen
    {
        internal SKPaint nativePen;

        public Pen(Color color)
        {
            nativePen = new SKPaint() {Color = new SKColor((uint)color.ToArgb()),
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcOver,
                FilterQuality = SKFilterQuality.High
            };
        }
    }
}