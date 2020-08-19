using System.Drawing;
using SkiaSharp;

namespace System.Drawing.Drawing2D
{
    public class HatchBrush : Brush
    {
        public HatchBrush(HatchStyle hatchStyle, Color foreColor, Color backColor)
        {
            nativeBrush = new SKPaint() {Color = foreColor.ToSKColor(), IsStroke = true};
        }

        public Color BackgroundColor { get; }

        public Color ForegroundColor
        {
            get
            {
                return Color.FromArgb(nativeBrush.Color.Alpha, nativeBrush.Color.Red, nativeBrush.Color.Green,
                    nativeBrush.Color.Blue);
            }
            set { nativeBrush.Color = value.ToSKColor(); }
        }

        public HatchStyle HatchStyle { get; }
    }
}