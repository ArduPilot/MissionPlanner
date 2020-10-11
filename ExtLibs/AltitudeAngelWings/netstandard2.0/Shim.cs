using SkiaSharp;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace SvgNet.SvgGdi
{
}
namespace System.Windows.Forms
{


 

    public class WebBrowser : Control
    {
        public bool CanGoBack { get; internal set; }

        public object Url { get; set; }

        public bool CanGoForward { get; set; }

        public string DocumentText { get; set; }

        public HtmlDocument Document { get; set; }

        public event EventHandler<WebBrowserNavigatingEventArgs> Navigating;
        public event EventHandler<WebBrowserNavigatedEventArgs> Navigated;

        public void Navigate(Uri authorizeUri)
        {
            
        }

        public void GoBack()
        {
          
        }

        public void GoForward()
        {
         
        }

        public void Navigate(string authorizeUri)
        {
            
        }
    }

    public class HtmlDocument
    {
        public void InvokeScript(string script)
        {
        }
    }

    public class WebBrowserNavigatedEventArgs : EventArgs
    {
        public Uri Url;
    }

    public class WebBrowserNavigatingEventArgs : EventArgs
    {
        public Uri Url;

        public bool Cancel;
    }
}

public class GdiGraphics : Graphics
{
    public GdiGraphics(SKSurface surface) : base(surface)
    {
    }

    public GdiGraphics(IntPtr handle, int width, int height) : base(handle, width, height)
    {
    }

    public GdiGraphics(Graphics fromImage) : base(fromImage.Surface)
    {
    }
}

public class SkiaGraphics : Graphics
{
    public SkiaGraphics(SKSurface surface) : base(surface)
    {
    }

    public SkiaGraphics(IntPtr handle, int width, int height) : base(handle, width, height)
    {
    }
}

public static class Extension
{
    public static SKColor SKColor(this Color color)
    {
        var skcol = SkiaSharp.SKColor.Empty.WithAlpha(color.A).WithRed(color.R).WithGreen(color.G)
            .WithBlue(color.B);
        return skcol;
    }

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
    }

    public static SKPaint SKPaint(this Font font)
    {
        return new SKPaint
        {
            Typeface = SKTypeface.FromFamilyName(font.SystemFontName),
            TextSize = font.SizeInPoints * 1.33334f,
            StrokeWidth = 2
        };
    }

    public static SKPoint SKPoint(this PointF pnt)
    {
        return new SKPoint(pnt.X, pnt.Y);
    }

    public static SKPoint SKPoint(this Point pnt)
    {
        return new SKPoint(pnt.X, pnt.Y);
    }

    public static SKPaint SKPaint(this Brush brush)
    {
        if (brush is SolidBrush)
            return new SKPaint
                {Color = ((SolidBrush) brush).Color.SKColor(), IsAntialias = true, Style = SKPaintStyle.Fill};

        if (brush is LinearGradientBrush)
        {
            var lgb = (LinearGradientBrush) brush;
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
    }
}