using System;
using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class Pen: ICloneable, IDisposable
    {
        internal SKPaint nativePen;

        public Pen(Color color): this(color.SKColor())
        {
        }

        public Pen(SKColor color, float width = 1)
        {
            Width = width;

            nativePen = new SKPaint()
            {
                Color = color,
                StrokeWidth = Width,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcOver,
                FilterQuality = SKFilterQuality.High
            };
        }

        public Pen(Color brush, float width) : this(brush.SKColor(), width)
        {
        }

        public Pen(Brush brush, float width): this(brush.nativeBrush.Color,width)
        {
        }

        public LineJoin LineJoin { get; set; }
        public float Width { get; set; }
        public LineCap StartCap { get; set; }
        public DashStyle DashStyle { get; set; }
        public Color Color { get; set; }
        public Brush Brush { get; set; }

        public object Clone()
        {
            return new Pen(nativePen.Color, Width);
        }

        public void Dispose()
        {
           nativePen?.Dispose();
        }
    }
}