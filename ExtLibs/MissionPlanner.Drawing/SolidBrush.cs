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
            _color = color;

            try
            {
                nativeBrush = new SKPaint() {Color = color.ToSKColor()};
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
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
                try
                {
                    nativeBrush.Color = value.ToSKColor();
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }
        }

        public Brush Clone()
        {
            return new SolidBrush() {nativeBrush = nativeBrush?.Clone()};
        }

        public void ScaleTransform(float rectangleWidth, float rectangleHeight, MatrixOrder append)
        {
        }

        public void TranslateTransform(float rectangleLeft, float rectangleTop, MatrixOrder append)
        {
        }
    }
}