using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public static class Extension
    {
        public static SKColor ToSKColor(this Color color)
        {
            var skcol = SkiaSharp.SKColor.Empty.WithAlpha(color.A).WithRed(color.R).WithGreen(color.G)
                .WithBlue(color.B);
            return skcol;
        }

        public static Color ToColor(this SKColor color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

        public static SKRect ToSKRect(this Rectangle rect)
        {
            return new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public static SKPaint ToSKPaint(this Pen pen)
        {
            return pen.nativePen;
            var paint = new SKPaint
            {
                Color = pen.Color.ToSKColor(),
                StrokeWidth = pen.Width,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcOver,
                FilterQuality = SKFilterQuality.High
            };

            if (pen.DashStyle != DashStyle.Solid)
                paint.PathEffect = SKPathEffect.CreateDash(pen.DashPattern, 0);
            return paint;
        }

        public static SKPaint ToSKPaint(this Font font)
        {
            return new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName(font.SystemFontName),
                TextSize = font.Size * 1.4f,
                StrokeWidth = 2

            };
        }

        public static SKPoint ToSKPoint(this PointF pnt)
        {
            return new SKPoint(pnt.X, pnt.Y);
        }

        public static SKPoint ToSKPoint(this Point pnt)
        {
            return new SKPoint(pnt.X, pnt.Y);
        }

        public static SKPaint ToSKPaint(this Brush brush)
        {
            if (brush is SolidBrush)
                return new SKPaint
                    { Color = ((SolidBrush)brush).Color.ToSKColor(), IsAntialias = true, Style = SKPaintStyle.Fill };

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
                            ((LinearGradientBrush) brush).LinearColors[0].ToSKColor(),
                            ((LinearGradientBrush) brush).LinearColors[1].ToSKColor()
                        }
                        , null, SKShaderTileMode.Clamp, SKMatrix.MakeIdentity())
                };
            }

            return new SKPaint();
        }
    }
}