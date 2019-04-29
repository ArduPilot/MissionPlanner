using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using PointF = System.Drawing.PointF;
using RectangleF = System.Drawing.RectangleF;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;

namespace MissionPlanner.Utilities.Drawing
{
    public class Graphics : IGraphics, IDisposable
    {
        private readonly SKPaint _paint =
            new SKPaint
            {
                IsAntialias = true,
                FilterQuality = SKFilterQuality.High,
                StrokeWidth = 0,
                BlendMode = SKBlendMode.SrcOver
            };

        private SKSurface _surface;

        //SKPictureRecorder _rec = new SKPictureRecorder();

        /*
        public static implicit operator SkiaGraphics(IGraphics g)
        {
            var ret = new SkiaGraphics();

            var imageWidth = (int) g.VisibleClipBounds.Width;
            var imageHeight = (int) g.VisibleClipBounds.Height;

            //var hdc = g.GetHdc();

            ret._surface = SKSurface.Create(imageWidth, imageHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            return ret;
        }
        */

        public Graphics()
        {
            _surface = SKSurface.Create(9999, 9999, SKColorType.Bgra8888, SKAlphaType.Opaque);
        }

        public Graphics(SKSurface surface)
        {
            _surface = surface;
        }

        public Graphics(IntPtr handle, int width, int height)
        {

            /*
             * 
GRGlInterface glInterface = GRGlInterface.AssembleGlInterface(GLFW.GetWGLContext(window), (contextHandle, name) => GLFW.GetProcAddress(name));
GRContext context = GRContext.Create(GRBackend.OpenGL, glInterface);
GRBackendRenderTargetDesc backendRenderTargetDescription = new GRBackendRenderTargetDesc
             */
            //_surface = SKSurface.Create(GRContext.Create(GRBackend.OpenGL,GRGlInterface.AssembleInterface(handle,)))();

            _surface = SKSurface.Create(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);

            //_rec.BeginRecording(new SKRect(0, 0, width, height));
        }

        public Region Clip { get; set; }
        public RectangleF ClipBounds { get; }
        public CompositingMode CompositingMode { get; set; }
        public CompositingQuality CompositingQuality { get; set; }
        public float DpiX { get; }
        public float DpiY { get; }
        public InterpolationMode InterpolationMode { get; set; }
        public bool IsClipEmpty { get; }
        public bool IsVisibleClipEmpty { get; }
        public float PageScale { get; set; }
        public GraphicsUnit PageUnit { get; set; }
        public PixelOffsetMode PixelOffsetMode { get; set; }
        public Point RenderingOrigin { get; set; }
        public SmoothingMode SmoothingMode { get; set; }
        public int TextContrast { get; set; }
        public TextRenderingHint TextRenderingHint { get; set; }

        public Matrix Transform
        {
            get { return null; }//return new Matrix(_image.TotalMatrix.Persp0, _image.TotalMatrix.Persp1, _image.TotalMatrix.ScaleX,_image.TotalMatrix.ScaleY, _image.TotalMatrix.TransX, _image.TotalMatrix.TransY);
            
            set { }
        }

        public RectangleF VisibleClipBounds { get; }
        private SKCanvas _image => _surface.Canvas;//_rec.RecordingCanvas;//_surface.Canvas;

        public byte[] WriteImage()
        {
            /*
            return SKImage.FromPicture(_rec.EndRecording(),
                    new SKSizeI((int)_rec.RecordingCanvas.LocalClipBounds.Width,
                        (int)_rec.RecordingCanvas.LocalClipBounds.Height))
                .Encode().ToArray();
                */
            return _surface.Snapshot().Encode().ToArray();
        }
        public static Graphics FromImage(Bitmap bmpDestination)
        {
            var bmpdata = bmpDestination.LockBits(new Rectangle(0, 0, bmpDestination.Width, bmpDestination.Height),
                null, SKColorType.Bgra8888);
            return new Graphics()
            {
                _surface = SKSurface.Create(bmpDestination.nativeSkBitmap.Info, bmpdata.Scan0)
            };
        }

        public static implicit operator Image(Graphics sk)
        {
            using (var snap = sk._surface.Snapshot())
            using (var rast = snap.ToRasterImage())
            using (var pxmap = rast.PeekPixels())
            {
                var bmpdata = pxmap.GetPixels();
                return new Bitmap(rast.Width, rast.Height, rast.Width * 4, SKColorType.Bgra8888, bmpdata);
            }
        }

        public void AddMetafileComment(byte[] data)
        {
          
        }



        public void Clear(Color color)
        {
            _image.Clear(color.ToSKColor());
            CompositingMode = CompositingMode.SourceOver;
        }

   

        public void Dispose()
        {
            _surface?.Dispose();
            _paint?.Dispose();
        }

        public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle);
            _image.DrawPath(path, pen.ToSKPaint());
        }

        
        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

  
        public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            DrawArc(pen, x, y, width, height, startAngle, (float)sweepAngle);
        }

        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            DrawBeziers(pen,
                new PointF[] {new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3), new PointF(x4, y4)});
        }

        public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        public void DrawBeziers(Pen pen, PointF[] points)
        {
            var path = new SKPath();
            path.MoveTo(points[0].ToSKPoint());
            path.CubicTo(points[1].ToSKPoint(),
                points[2].ToSKPoint(),
                points[3].ToSKPoint());

            _image.DrawPath(path, pen.ToSKPaint());
        }

        public void DrawBeziers(Pen pen, Point[] points)
        {
            var pts = Point2PointF(points);
            DrawBeziers(pen, pts);
        }

        public void DrawClosedCurve(Pen pen, PointF[] points)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, .5f);
            DrawBeziers(pen, pts);
        }

        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, tension);
            DrawBeziers(pen, pts);
        }

        public void DrawClosedCurve(Pen pen, Point[] points)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, .5f);
            DrawBeziers(pen, pts);
        }

        public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, tension);
            DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, PointF[] points)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, false, .5f);
            DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, false, tension);
            DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
        {
            var pts = Spline2Bez(points, offset, numberOfSegments, false, .5f);
            DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
        {
            var pts = Spline2Bez(points, offset, numberOfSegments, false, tension);
            DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, Point[] points)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, false, .5f);
            DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, Point[] points, float tension)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, false, tension);
            DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
        {
            var pts = Spline2Bez(Point2PointF(points), offset, numberOfSegments, false, tension);
            DrawBeziers(pen, pts);
        }

        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            DrawArc(pen, x, y, width, height, 0, 360);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            DrawEllipse(pen, rect.X, rect.Y, rect.Width, (float)rect.Height);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            DrawEllipse(pen, x, y, width, (float)height);
        }

        public void DrawIcon(Icon icon, int x, int y)
        {
            throw new NotImplementedException();
        }

        public void DrawIcon(Icon icon, Rectangle targetRect)
        {
            throw new NotImplementedException();
        }

        public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, PointF point)
        {
            DrawImage(image, point.X, point.Y);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawImage(Image image, float x, float y)
        {
            DrawImage(image, x, y, image.Width, image.Height);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawImage(Image image, RectangleF rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            DrawImage(image, (long)x, (long)y, (long)width, (long)height);
            //throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point point)
        {
            DrawImage(image, point.X, (float)point.Y);
        }

        public void DrawImage(Image image, int x, int y)
        {
            DrawImage(image, x, (float)y);
        }

        public void DrawImage(Image image, Rectangle rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, (float)rect.Height);
        }

        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            DrawImage(image, (float)x, y, width, height);
        }

        public void DrawImage(Image image, PointF[] destPoints)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point[] destPoints)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image img, Rectangle rectangle, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit graphicsUnit, ImageAttributes tileFlipXYAttributes)
        {
            var coltype = SKColorType.Bgra8888;
            ((Bitmap)img).MakeTransparent(Color.Transparent);
            var data = ((Bitmap) img).LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                SKColorType.Bgra8888);

            if(CompositingMode == CompositingMode.SourceOver)
                _paint.BlendMode = SKBlendMode.SrcOver;
            if (CompositingMode == CompositingMode.SourceCopy)
                _paint.BlendMode = SKBlendMode.Src;
            //if(img.PixelFormat == PixelFormat.Format32bppArgb)
               _paint.Color = SKColors.Black;

               


            var imginfo = new SKImageInfo(img.Width, img.Height, coltype, SKAlphaType.Premul);

            var pxmap = new SKPixmap(imginfo, data.Scan0, data.Stride);

            var image = SKImage.FromPixels(pxmap);

            if (image == null)
                return;

            _image.DrawImage(image, new SKRect(srcX, srcY, srcX + srcWidth, srcY + srcHeight),
                new SKRect(rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom), _paint);
            ((Bitmap)img).UnlockBits(data);
            data = null;

            return;

            try
            {
                using (var skbmp =
                    new SKBitmap(new SKImageInfo(img.Width, img.Height, coltype, SKAlphaType.Unpremul)))
                {
                    skbmp.SetPixels(data.Scan0);

         

              
                    _image.DrawBitmap(skbmp, new SKRect(srcX, srcY, srcX + srcWidth, srcY + srcHeight),
                        new SKRect(rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom), _paint);
                }
            }
            catch
            {
            }

            ((Bitmap) img).UnlockBits(data);
            data = null;
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit,
            ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image img, long i, long i1, long width, long height)
        {
            DrawImage(img, new Rectangle((int) i, (int) i1, (int) width, (int) height), 0.0f, 0, width, height,
                GraphicsUnit.Pixel,
                new ImageAttributes());
        }

        public void DrawImage(Image image, Rectangle rectangle, int p1, int p2, long p3, long p4,
            GraphicsUnit graphicsUnit, ImageAttributes TileFlipXYAttributes)
        {
            DrawImage(image, rectangle, (float) rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                GraphicsUnit.Pixel, new ImageAttributes());
        }

        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, Point point)
        {
            DrawImage(image, point.X, (float)point.Y);
        }

        public void DrawImageUnscaled(Image image, int x, int y)
        {
            DrawImage(image, x, y, image.Width, image.Height);
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            DrawImageUnscaled(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            if (x1 == x2 && y1 == y2)
                return;
            _image.DrawLine(x1, y1, x2, y2, pen.ToSKPaint());
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            DrawLine(pen, x1, y1, x2, (float)y2);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, (float)pt2.Y);
        }

        public void DrawLines(Pen pen, PointF[] points)
        {
            var start = points.First();
            foreach (var pointF in points.Skip(1))
            {
                DrawLine(pen, start.X, start.Y, pointF.X, pointF.Y);
                start = pointF;
            }
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawLines(Pen pen, Point[] points)
        {
            var pts = Point2PointF(points);
            DrawLines(pen, pts);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        /// <remarks>
        ///     Mainly based on the libgdi+ implementation: https://github.com/mono/libgdiplus/blob/master/src/graphics-cairo.c
        ///     and this SO question reply:
        ///     https://stackoverflow.com/questions/1790862/how-to-determine-endpoints-of-arcs-in-graphicspath-pathpoints-and-pathtypes-arra
        ///     from SiliconMind.
        /// </remarks>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            //Save the original pen dash style in case we need to change it
            var originalPenDashStyle = pen.DashStyle;

            PointF last= PointF.Empty;
            foreach (var pathPathPoint in path.PathPoints)
            {
                if (last == PointF.Empty)
                {
                    last = pathPathPoint;
                    continue;
                }

                DrawLine(pen, last, pathPathPoint);

                last = pathPathPoint;
            }

            pen.DashStyle = originalPenDashStyle;
        }


        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle);
            _image.DrawPath(path, pen.ToSKPaint());
        }

        public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawPie(pen, rect.X, rect.X, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            DrawPie(pen, x, y, width, height, startAngle, (float)sweepAngle);
        }

        public void DrawPolygon(Pen pen, Point[] points)
        {
            var path = new SKPath();
            path.AddPoly(points.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            if (path.Bounds.Width == 0 || path.Bounds.Height == 0)
                return;
            _image.DrawPath(path, pen.ToSKPaint());
        }

        public void DrawPolygon(Pen pen, PointF[] points)
        {
            var path = new SKPath();
            path.AddPoly(points.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            if (path.Bounds.Width == 0 || path.Bounds.Height == 0)
                return;
            _image.DrawPath(path, pen.ToSKPaint());
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            _image.DrawRect(new SKRect(x, y, x + width, y + height), pen.ToSKPaint());
        }

        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            DrawRectangle(pen, x, y, width, (float)height);
        }

        public void DrawRectangles(Pen pen, RectangleF[] rects)
        {
            foreach (var rc in rects) DrawRectangle(pen, rc.Left, rc.Top, rc.Width, rc.Height);
        }

        public void DrawRectangles(Pen pen, Rectangle[] rects)
        {
            foreach (var rc in rects) DrawRectangle(pen, rc.Left, rc.Top, rc.Width, (float)rc.Height);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            DrawText(s, font, brush, new RectangleF(x, y, 0, 0), StringFormat.GenericDefault, true);
        }

        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            DrawText(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), StringFormat.GenericDefault,
                true);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            DrawText(s, font, brush, new RectangleF(x, y, 0, 0), format, true);
        }

        public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            DrawText(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), format, true);
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            DrawText(s, font, brush, layoutRectangle, StringFormat.GenericDefault, false);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle,
            StringFormat format)
        {
            DrawText(s, font, brush, layoutRectangle, format, false);
        }

        public void DrawText(string s, Font font, Brush brush, RectangleF layoutRectangle,
            StringFormat format, bool duno)
        {
            if (String.IsNullOrEmpty(s))
                return;
            var pnt = brush.ToSKPaint();
            // Find the text bounds
            var textBounds = SKRect.Empty;
            var fnt = font.ToSKPaint();
            fnt.MeasureText(s, ref textBounds);

            pnt.TextSize = fnt.TextSize;
            pnt.Typeface = fnt.Typeface;

            pnt.StrokeWidth = 1;

            if (textBounds.Width > layoutRectangle.Width-2 && layoutRectangle.Width != 0)
            {
                DrawText(_image, s,
                    new SKRect(layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height),
                    pnt);
                return;
            }

            if (format.Alignment == StringAlignment.Center)
            {
                layoutRectangle.X += layoutRectangle.Width / 2 - textBounds.MidX;
            }
            if (format.LineAlignment == StringAlignment.Center) // vertical
            {
                layoutRectangle.Y += layoutRectangle.Height / 2 -  textBounds.Height/2;
            }
            _image.DrawText(s, layoutRectangle.X, layoutRectangle.Y + textBounds.Height, pnt);
        }

        private void DrawText(SKCanvas canvas, string text, SKRect area, SKPaint paint)
        {
            float lineHeight = paint.TextSize * 1.1f;
            var lines = SplitLines(text, paint, area.Width);
            var height = lines.Count() * lineHeight;

            var y = area.MidY-2 - height / 2;

            foreach (var line in lines)
            {
                y += lineHeight;
                var x = area.MidX - line.Width / 2;
                canvas.DrawText(line.Value, x, y, paint);
            }
        }

        public class Line
        {
            public string Value { get; set; }

            public float Width { get; set; }
        }

        private Line[] SplitLines(string text, SKPaint paint, float maxWidth)
        {
            var spaceWidth = paint.MeasureText(" ");
            var lines = text.Split('\n');

            return lines.SelectMany((line) =>
            {
                var result = new List<Line>();

                var words = line.Split(new[] { " " }, StringSplitOptions.None);

                var lineResult = new StringBuilder();
                float width = 0;
                foreach (var word in words)
                {
                    var wordWidth = paint.MeasureText(word);
                    var wordWithSpaceWidth = wordWidth + spaceWidth;
                    var wordWithSpace = word + " ";

                    if (width + wordWidth > maxWidth)
                    {
                        result.Add(new Line() { Value = lineResult.ToString(), Width = width });
                        lineResult = new StringBuilder(wordWithSpace);
                        width = wordWithSpaceWidth;
                    }
                    else
                    {
                        lineResult.Append(wordWithSpace);
                        width += wordWithSpaceWidth;
                    }
                }

                result.Add(new Line() { Value = lineResult.ToString(), Width = width });

                return result.ToArray();
            }).ToArray();
        }

        public void ExcludeClip(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void ExcludeClip(Region region)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, FillMode.Alternate);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, tension);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, FillMode.Alternate);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, tension);
            FillBeziers(brush, pts, fillmode);
        }

        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            FillEllipse(brush, (float)x, y, width, height);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillEllipse(Brush brush, RectangleF rect)
        {
            FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillEllipse(Brush brush, float x, float y, float width, float height)
        {
            var path = new SKPath();
            path.AddOval(SKRect.Create(x, y, width, height));
            _image.DrawPath(path, brush.ToSKPaint());
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillEllipse(Brush brush, Rectangle rect)
        {
            FillEllipse(brush, rect.X, rect.Y, rect.Width, (float)rect.Height);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            FillPolygon(brush, path.PathPoints);
        }

        public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
        {
            FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle,
            float sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle);
            _image.DrawPath(path, brush.ToSKPaint());
        }

        public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle);
            _image.DrawPath(path, brush.ToSKPaint());
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            FillPolygon(brush, points, FillMode.Alternate);
        }

        public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            var path = new SKPath();
            path.AddPoly(points.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            if (path.Bounds.Width == 0 || path.Bounds.Height == 0)
                return;
            _image.DrawPath(path, brush.ToSKPaint());
        }

        public void FillPolygon(Brush brushh, PointF[] list)
        {
            var path = new SKPath();
            path.AddPoly(list.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            if (path.Bounds.Width == 0 || path.Bounds.Height == 0)
                return;
            _image.DrawPath(path, brushh.ToSKPaint());
        }

        public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
        {
            FillPolygon(brush, points);
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            _image.DrawRect(new SKRect(x, y, x + width, y + height), brush.ToSKPaint());
        }

        public void FillRectangle(Brush brush, Rectangle rect)
        {
            FillRectangle(brush, (float)rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            FillRectangle(brush, (float)x, y, width, height);
        }

        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            foreach (var rect in rects)
            {
                FillRectangle(brush, (float)rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        public void FillRectangles(Brush brush, Rectangle[] rects)
        {
            foreach (var rect in rects)
            {
                FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        public void FillRegion(Brush brush, Region region)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void Flush(FlushIntention intention)
        {
            throw new NotImplementedException();
        }

        public Color GetNearestColor(Color color)
        {
            throw new NotImplementedException();
        }

        public void IntersectClip(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void IntersectClip(RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void IntersectClip(Region region)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(int x, int y)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(Point point)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(float x, float y)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(PointF point)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            if (String.IsNullOrEmpty(text))
                return new SizeF();
            var bound = new SKRect();
            font.ToSKPaint().MeasureText(text, ref bound);
            return new SizeF(bound.Width+5, bound.Height);
        }

        public SizeF MeasureString(string text, Font font)
        {
            return MeasureString(text, font, SizeF.Empty, StringFormat.GenericDefault);
        }

        public SizeF MeasureString(string text, Font font, int width)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, int width, StringFormat format)
        {
            return MeasureString(text, font, PointF.Empty, StringFormat.GenericDefault);
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat,
            out int charactersFitted,
            out int linesFilled)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
        {
            if (String.IsNullOrEmpty(text))
                return new SizeF();
            var bound = new SKRect();
            font.ToSKPaint().MeasureText(text, ref bound);
            return new SizeF(bound.Width /* + origin.X*/, bound.Height /* + origin.Y*/);
        }

        public void MultiplyTransform(Matrix matrix)
        {
            throw new NotImplementedException();
        }

        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            throw new NotImplementedException();
        }

        public void ResetClip()
        {
            _image.ClipRect(SKRect.Create(0,0,10000,10000), (SKClipOperation)5); // kReplace_Op
            //_image.RestoreToCount(-1);
            //_image.ClipRect(SKRect.Create(0, 0, 10000, 10000), SKClipOperation.Intersect, false);
        }

        public void ResetTransform()
        {
            _image.ResetMatrix();
        }


        public void RotateTransform(float angle)
        {
            _surface.Canvas.RotateDegrees(angle);
        }

        public void RotateTransform(float angle, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
                _image.RotateDegrees(angle);

            if (order == MatrixOrder.Append)
            {
                //checkthis
                var old = _image.TotalMatrix;
                var extra = SKMatrix.MakeRotation(angle);
                SKMatrix.PreConcat(ref old, extra);
                _image.SetMatrix(old);
            }
        }


        public void ScaleTransform(float sx, float sy)
        {
            ScaleTransform(sx,sy, MatrixOrder.Prepend);
        }

        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
                _image.Scale(sx, sy);

            if (order == MatrixOrder.Append)
            {
                var old = _image.TotalMatrix;
                old.SetScaleTranslate(sx, sy, old.TransX, old.TransY);
                _image.SetMatrix(old);
            }
        }

        public void SetClip(Graphics g)
        {
            throw new NotImplementedException();
        }

        public void SetClip(Graphics g, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void SetClip(Rectangle rect)
        {
            _image.ClipRect(rect.ToSKRect());
        }

        public void SetClip(Rectangle rect, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void SetClip(RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void SetClip(RectangleF rect, CombineMode mode)
        {
            _image.ClipRect(new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom), SKClipOperation.Intersect,
                false);
        }

        public void SetClip(GraphicsPath path)
        {
            var skpath = new SKPath();
            skpath.AddPoly(path.PathPoints.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.ClipPath(skpath);
        }

        public void SetClip(GraphicsPath path, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void SetClip(Region region, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
        {
            throw new NotImplementedException();
        }

        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
        {
            throw new NotImplementedException();
        }

        public void TranslateClip(float dx, float dy)
        {
            throw new NotImplementedException();
        }

        public void TranslateClip(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        public void TranslateTransform(float dx, float dy)
        {
            TranslateTransform(dx, dy, MatrixOrder.Prepend);
        }

        public void TranslateTransform(float f, float f1, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
                _image.Translate(f, f1);

            if (order == MatrixOrder.Append)
            {
                var old = _image.TotalMatrix;
                old.SetScaleTranslate(old.ScaleX, old.ScaleY, f, f1);
                _image.SetMatrix(old);
            }
        }

        private static PointF ControlPoint(PointF l, PointF pt, float t)
        {
            var v = new PointF(l.X - pt.X, l.Y - pt.Y);

            var vlen = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            v.X /= (float)Math.Sqrt(vlen / (10 * t * t));
            v.Y /= (float)Math.Sqrt(vlen / (10 * t * t));

            return new PointF(pt.X + v.X, pt.Y + v.Y);
        }

        private static PointF[] ControlPoints(PointF l, PointF r, PointF pt, float t)
        {
            //points to vectors
            var lv = new PointF(l.X - pt.X, l.Y - pt.Y);
            var rv = new PointF(r.X - pt.X, r.Y - pt.Y);

            var nlv = new PointF(lv.X - rv.X, lv.Y - rv.Y);
            var nrv = new PointF(rv.X - lv.X, rv.Y - lv.Y);

            var nlvlen = (float)Math.Sqrt(nlv.X * nlv.X + nlv.Y * nlv.Y);
            nlv.X /= (float)Math.Sqrt(nlvlen / (10 * t * t));
            nlv.Y /= (float)Math.Sqrt(nlvlen / (10 * t * t));

            var nrvlen = (float)Math.Sqrt(nrv.X * nrv.X + nrv.Y * nrv.Y);
            nrv.X /= (float)Math.Sqrt(nrvlen / (10 * t * t));
            nrv.Y /= (float)Math.Sqrt(nrvlen / (10 * t * t));

            var ret = new PointF[2];

            ret[0] = new PointF(pt.X + nlv.X, pt.Y + nlv.Y);
            ret[1] = new PointF(pt.X + nrv.X, pt.Y + nrv.Y);

            return ret;
        }

        private static string GDIArc2SVGPath(float x, float y, float width, float height, float startAngle,
            float sweepAngle, bool pie)
        {
            var longArc = 0;

            var start = new PointF();
            var end = new PointF();
            var center = new PointF(x + width / 2f, y + height / 2f);

            startAngle = startAngle / 360f * 2f * (float)Math.PI;
            sweepAngle = sweepAngle / 360f * 2f * (float)Math.PI;

            sweepAngle += startAngle;

            if (sweepAngle > startAngle)
            {
                var tmp = startAngle;
                startAngle = sweepAngle;
                sweepAngle = tmp;
            }

            if (sweepAngle - startAngle > Math.PI || startAngle - sweepAngle > Math.PI) longArc = 1;

            start.X = (float)Math.Cos(startAngle) * (width / 2f) + center.X;
            start.Y = (float)Math.Sin(startAngle) * (height / 2f) + center.Y;

            end.X = (float)Math.Cos(sweepAngle) * (width / 2f) + center.X;
            end.Y = (float)Math.Sin(sweepAngle) * (height / 2f) + center.Y;

            var s = "M " + start.X.ToString("F", CultureInfo.InvariantCulture) + "," +
                    start.Y.ToString("F", CultureInfo.InvariantCulture) +
                    " A " + (width / 2f).ToString("F", CultureInfo.InvariantCulture) + " " +
                    (height / 2f).ToString("F", CultureInfo.InvariantCulture) + " " +
                    "0 " + longArc + " 0 " + end.X.ToString("F", CultureInfo.InvariantCulture) + " " +
                    end.Y.ToString("F", CultureInfo.InvariantCulture);

            if (pie)
            {
                s += " L " + center.X.ToString("F", CultureInfo.InvariantCulture) + "," +
                     center.Y.ToString("F", CultureInfo.InvariantCulture);
                s += " L " + start.X.ToString("F", CultureInfo.InvariantCulture) + "," +
                     start.Y.ToString("F", CultureInfo.InvariantCulture);
            }

            return s;
        }

        private static PointF[] Point2PointF(Point[] p)
        {
            var pf = new PointF[p.Length];
            for (var i = 0; i < p.Length; ++i) pf[i] = new PointF(p[i].X, p[i].Y);

            return pf;
        }

        //This seems to be a very good approximation.  GDI must be using a similar simplistic method for some odd reason.
        //If a curve is closed, it uses all points, and ignores start and num.
        private static PointF[] Spline2Bez(PointF[] points, int start, int num, bool closed, float tension)
        {
            var cp = new ArrayList();
            var res = new ArrayList();

            var l = points.Length - 1;

            res.Add(points[0]);
            res.Add(ControlPoint(points[1], points[0], tension));

            for (var i = 1; i < l; ++i)
            {
                var pts = ControlPoints(points[i - 1], points[i + 1], points[i], tension);
                res.Add(pts[0]);
                res.Add(points[i]);
                res.Add(pts[1]);
            }

            res.Add(ControlPoint(points[l - 1], points[l], tension));
            res.Add(points[l]);

            if (closed)
            {
                //adjust rh cp of point 0
                var pts = ControlPoints(points[l], points[1], points[0], tension);
                res[1] = pts[1];

                //adjust lh cp of point l and add rh cp
                pts = ControlPoints(points[l - 1], points[0], points[l], tension);
                res[res.Count - 2] = pts[0];
                res.Add(pts[1]);

                //add new end point and its lh cp
                pts = ControlPoints(points[l], points[1], points[0], tension);
                res.Add(pts[0]);
                res.Add(points[0]);

                return (PointF[])res.ToArray(typeof(PointF));
            }

            var subset = new ArrayList();

            for (var i = start * 3; i < (start + num) * 3; ++i) subset.Add(res[i]);

            subset.Add(res[(start + num) * 3]);

            return (PointF[])subset.ToArray(typeof(PointF));
        }

        private void FillBeziers(Brush brush, PointF[] points, FillMode fillmode)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, int charactersFitted, int linesFilled)
        {
            throw new NotImplementedException();
        }
    }
}