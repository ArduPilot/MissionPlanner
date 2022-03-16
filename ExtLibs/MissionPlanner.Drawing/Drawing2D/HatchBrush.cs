using System.Drawing;
using SkiaSharp;

namespace System.Drawing.Drawing2D
{
    public class HatchBrush : Brush
    {
        public HatchBrush(HatchStyle hatchStyle, Color foreColor, Color backColor)
        {
            HatchStyle = hatchStyle;
            SKBitmap bitmap = new SKBitmap(new SKImageInfo(2, 2, SKColorType.Bgra8888));

            bitmap.Erase(backColor.ToSKColor());
            bitmap.SetPixel(0,0, foreColor.ToSKColor());
            bitmap.SetPixel(1,1, foreColor.ToSKColor());

            // create the bitmap shader
            var shader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);

            // add to the paint
            nativeBrush = new SKPaint() {Shader = shader, Style = SKPaintStyle.Fill};
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