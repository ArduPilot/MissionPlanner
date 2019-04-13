using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public static class Extension
    {
        public static SKColor SKColor(this Color color)
        {
            var skcol = new SkiaSharp.SKColor((uint)color.ToArgb());
            return skcol;
        }
        /*
        public static SKPaint SKPaint(this Pen pen)
        {
            var paint = new SKPaint
            {
                Color = pen.Color.SKColor(),
                StrokeWidth = pen.Width,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcOver,
                FilterQuality = SKFilterQuality.High
            };

            if (pen.DashStyle != DashStyle.Solid)
                paint.PathEffect = SKPathEffect.CreateDash(pen.DashPattern, 0);
            return paint;
        }*/
        /*
        public static SKPaint SKPaint(this Font font)
        {
            return new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName(font.SystemFontName),
                TextSize = font.Size * 1.5f
            };
        }
        */
        public static SKPoint ToSKPoint(this PointF pnt)
        {
            return new SKPoint(pnt.X, pnt.Y);
        }

        public static SKPoint ToSKPoint(this Point pnt)
        {
            return new SKPoint(pnt.X, pnt.Y);
        }
        /*
        public static SKPaint SKPaint(this Brush brush)
        {
            if (brush is SolidBrush)
                return new SKPaint
                    { Color = ((SolidBrush)brush).Color.SKColor(), IsAntialias = true, Style = SKPaintStyle.Fill };

            if (brush is LinearGradientBrush)
            {
                var lgb = (LinearGradientBrush)brush;
                return new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Shader = SKShader.CreateLinearGradient(new SKPoint(lgb.Rectangle.X, lgb.Rectangle.Y),
                        new SKPoint(lgb.Rectangle.X, lgb.Rectangle.Bottom),
                        new[]
                        {
                            ((LinearGradientBrush) brush).LinearColors[0].SKColor(),
                            ((LinearGradientBrush) brush).LinearColors[1].SKColor()
                        }
                        , null, SKShaderTileMode.Clamp, SKMatrix.MakeIdentity())
                };
            }

            return new SKPaint();
        }*/
    }
}