using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;

namespace System.Drawing
{
    public class SolidBrush : Brush
    {
        public SolidBrush() : this(Color.Black)
        {
        }

        public SolidBrush(Color color)
        {
            // no native paint here: drawing uses the per-colour paint from Extension.ToSKPaint(Brush)
            _color = color;
        }

        public SolidBrush(uint color)
        {
            _color = Color.FromArgb((byte) (color >> 24), (byte) (color >> 16), (byte) (color >> 8), (byte) color);
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                if (nativeBrush != null)
                    nativeBrush.Color = value.ToSKColor();
            }
        }

        public void ScaleTransform(float rectangleWidth, float rectangleHeight, MatrixOrder append)
        {
        }

        public void TranslateTransform(float rectangleLeft, float rectangleTop, MatrixOrder append)
        {
        }
    }
}