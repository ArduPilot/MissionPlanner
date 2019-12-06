using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Drawing
{
    public class TextureBrush : Brush
    {
        public TextureBrush()
        {
            nativeBrush = new SKPaint() {};
        }

        public TextureBrush(Image backgroundImage, WrapMode tile)
        {
          
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
