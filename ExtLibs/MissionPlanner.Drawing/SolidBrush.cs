using System;
using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Drawing
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
                Console.WriteLine(e);
            }
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
                    Console.WriteLine(e);
                }
            }
        }
    }
}
