using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Drawing
{
    public class SolidBrush : Brush
    {
        public SolidBrush(): this(Color.Black)
        {
        }

        public SolidBrush(Color color)
        {
            nativeBrush = new SKPaint() {Color = color.ToSKColor()};
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
