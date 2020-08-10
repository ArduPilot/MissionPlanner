using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;

namespace System.Drawing
{
    public class TextureBrush : Brush
    {
        public TextureBrush()
        {
            nativeBrush = new SKPaint() { };
        }

        public TextureBrush(Image backgroundImage, WrapMode tile)
        {
        }

        public Image Image;

        public Color Color
        {
            get
            {
                return Color.FromArgb(nativeBrush.Color.Alpha, nativeBrush.Color.Red, nativeBrush.Color.Green,
                    nativeBrush.Color.Blue);
            }
            set { nativeBrush.Color = value.ToSKColor(); }
        }

        public Brush Clone()
        {
            return new TextureBrush() {nativeBrush = nativeBrush?.Clone()};
        }

        public void ScaleTransform(float rectangleWidth, float rectangleHeight, MatrixOrder append)
        {
        }

        public void TranslateTransform(float rectangleLeft, float rectangleTop, MatrixOrder append)
        {
        }
    }
}