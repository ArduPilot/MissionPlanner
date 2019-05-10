using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class HatchBrush : Brush
    {
        public HatchBrush(HatchStyle hatchStyle, Color foreColor, Color backColor)
        {
            nativeBrush = new SKPaint() {Color = foreColor.ToSKColor(), IsStroke = true};
        }

        public Color Color
        {
            get
            {
                return Color.FromArgb(nativeBrush.Color.Alpha, nativeBrush.Color.Red, nativeBrush.Color.Green, nativeBrush.Color.Blue);
            }
            set { nativeBrush.Color = value.ToSKColor(); }
        }
    }
}
