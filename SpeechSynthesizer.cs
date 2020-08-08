using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;

namespace tlogThumbnailHandler
{
    public class tlogThumbnailHandler
    {
        public static string queuefile = "queue.txt";

    }
}

namespace System.Windows.Forms
{
    public partial class LogIndex : Form
    {
    }

 

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

public class GdalConfiguration
{
    public static void ConfigureGdal()
    {
        
    }

    public static void ConfigureOgr()
    {
        
    }
}

public class GdiGraphics: Graphics
{
    public GdiGraphics(SKSurface surface) : base(surface)
    {
    }

    public GdiGraphics(IntPtr handle, int width, int height) : base(handle, width, height)
    {
    }

    public GdiGraphics(Graphics fromImage) : base(IntPtr.Zero,(int) fromImage.ClipBounds.Width, (int)fromImage.ClipBounds.Height)
    {
        
    }
}
public class SkiaGraphics: Graphics
{
    public SkiaGraphics(SKSurface surface) : base(surface)
    {
    }

    public SkiaGraphics(IntPtr handle, int width, int height) : base(handle, width, height)
    {
    }
}
/*
namespace OSGeo.GDAL
{
    public class Gdal
    {
        internal static string GetConfigOption(string v, string notSet)
        {
            throw new NotImplementedException();
        }

        internal static void SetConfigOption(string v, string gdalData)
        {
            throw new NotImplementedException();
        }

        internal static void AllRegister()
        {
            throw new NotImplementedException();
        }

        internal static int GetDriverCount()
        {
            return 0;
        }

        internal static (string ShortName, string LongName) GetDriver(int i)
        {
            throw new NotImplementedException();
        }
    }
}
namespace OSGeo.OGR
{
    public class Ogr
    {
        internal static void RegisterAll()
        {
            throw new NotImplementedException();
        }

        internal static int GetDriverCount()
        {
            return 0;
        }

        internal static Driver GetDriver(int i)
        {
            throw new NotImplementedException();
        }
    }

    internal class Driver
    {
        internal string GetName()
        {
            throw new NotImplementedException();
        }
    }
}*/
namespace System.Speech.Synthesis
{
    public class SpeechSynthesizer
    {
        public SynthesizerState State { get; set; }

        public void SpeakAsyncCancelAll()
        {

        }

        public void Dispose()
        {

        }

        public void SpeakAsync(string text)
        {

        }
    }

    public enum SynthesizerState
    {
        Ready,
        Speaking
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
            TextSize = font.Size * 1.4f,
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
    }
}


namespace System.Management
{
    public class ObjectQuery
    {
        private string v;

        public ObjectQuery(string v)
        {
            this.v = v;
        }
    }

    public class ManagementObjectSearcher: IDisposable
    {
        private ObjectQuery query;

        public ManagementObjectSearcher(ObjectQuery query)
        {
            this.query = query;
        }

        public IEnumerable<ManagementObject> Get()
        {
            return new List<ManagementObject>();
        }

        public void Dispose()
        {
        }
    }

    public class ManagementObject
    {
        public PropertyDataCollection Properties = new PropertyDataCollection();

        public class PropertyDataCollection : ICollection
        {
         

            public IEnumerator<extratype> GetEnumerator()
            {
                yield return new extratype();
            }

            public extratype this[string pnpdeviceid]
            {
                get { return null; }
            }

            public void CopyTo(Array array, int index)
            {
                
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count { get; }

            public bool IsSynchronized { get; }

            public object SyncRoot { get; }
        }

        public class extratype
        {
            public object Name { get; set; }
            public object Value { get; set; }
        }
    }
}