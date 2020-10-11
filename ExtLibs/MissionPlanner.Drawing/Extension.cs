using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using SkiaSharp;

namespace System.Drawing
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

        public static SKRectI ToSKRectI(this Rectangle rect)
        {
            return new SKRectI(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public static Rectangle ToRectangle(this RectangleF rect)
        {
            return new Rectangle((int)rect.X,(int)rect.Y,(int)rect.Width,(int)rect.Height);
        }

        public static RectangleF ToRectangleF(this Rectangle rect)
        {
            return new Rectangle(rect.X,rect.Y,rect.Width,rect.Height);
        }

        public static SKPaint ToSKPaint(this Pen pen)
        {
            pen.nativePen.StrokeWidth = pen.Width;
            pen.nativePen.Color = pen.Color.ToSKColor();
            pen.nativePen.Style = SKPaintStyle.Stroke;
            if (pen.DashStyle != DashStyle.Solid)
                pen.nativePen.PathEffect = SKPathEffect.CreateDash(pen.DashPattern, 0);
            return pen.nativePen;
        }


        static Dictionary<string, SKTypeface> fontcache = new Dictionary<string, SKTypeface>();

        public static SKPaint ToSKPaint(this Font font)
        {
            var fm = SKFontManager.Default;
            var id = font.Name;
            lock (fontcache)
                if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "zh")
                {
                    id = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    if (!fontcache.ContainsKey(id))
                        fontcache[CultureInfo.CurrentUICulture.TwoLetterISOLanguageName] =
                            fm.MatchCharacter("", new[] {"zh"}, '飞');
                }
                else if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                {
                    id = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    if (!fontcache.ContainsKey(id))
                        fontcache[CultureInfo.CurrentUICulture.TwoLetterISOLanguageName] =
                            fm.MatchCharacter("", new[] {"ja"}, 'フ');
                }
                else if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "kr")
                {
                    id = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    if (!fontcache.ContainsKey(id))
                        fontcache[CultureInfo.CurrentUICulture.TwoLetterISOLanguageName] =
                            fm.MatchCharacter("", new[] {"kr"}, '비');
                }
                else
                {
                    if (!fontcache.ContainsKey(id))
                        fontcache[id] = SKTypeface.FromFamilyName(id);
                }

            return new SKPaint
            {
                Typeface = fontcache[id],
                TextSize = font.SizeInPoints * 1.33334f,
                StrokeWidth = 2,
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

        static Dictionary<Color, SKPaint> brushcache = new Dictionary<Color, SKPaint>();

        public static SKPaint ToSKPaint(this Brush brush)
        {
            if (brush is SolidBrush)
            {
                lock (brushcache)
                    if (!brushcache.ContainsKey(((SolidBrush) brush).Color))
                        brushcache[((SolidBrush) brush).Color] = new SKPaint
                        {
                            Color = ((SolidBrush) brush).Color.ToSKColor(), IsAntialias = true,
                            Style = SKPaintStyle.Fill
                        };

                return brushcache[((SolidBrush) brush).Color];
            }

            if (brush is HatchBrush)
            {
                return brush.nativeBrush;
            }

            if (brush is TextureBrush)
            {
                return brush.nativeBrush;
            }

            if (brush is LinearGradientBrush)
            {
                var lgb = (LinearGradientBrush) brush;
                if (lgb._gradMode == LinearGradientMode.Horizontal)
                {
                    return new SKPaint
                    {
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill,
                        Shader = SKShader.CreateLinearGradient(new SKPoint(lgb.Rectangle.X, lgb.Rectangle.Y),
                            new SKPoint(lgb.Rectangle.Right, lgb.Rectangle.Y),
                            new[]
                            {
                                ((LinearGradientBrush) brush).LinearColors[0].ToSKColor(),
                                ((LinearGradientBrush) brush).LinearColors[1].ToSKColor()
                            }
                            , null, SKShaderTileMode.Clamp, SKMatrix.MakeIdentity())
                    };
                }

                if (lgb._gradMode == LinearGradientMode.Vertical)
                {
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
            }

            return new SKPaint();
        }
    }
}