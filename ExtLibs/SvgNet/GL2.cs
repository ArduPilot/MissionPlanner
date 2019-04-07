using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using LibTessDotNet;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace SvgNet.SvgGdi
{
    public class GL2 : IGraphics, IDisposable
    {
        private static Dictionary<IntPtr, IGraphicsContext>
            control2ContextList = new Dictionary<IntPtr, IGraphicsContext>();

        private static Dictionary<IntPtr, IWindowInfo>
            IWindowInfoList = new Dictionary<IntPtr, IWindowInfo>();

        IGraphicsContext control2Context;

        public GL2(IntPtr handle, int width, int height)
        {
            //Console.WriteLine(handle);

            IWindowInfo window_info;

            if (!IWindowInfoList.ContainsKey(handle))
            {
                window_info = Utilities.CreateWindowsWindowInfo(handle);
                IWindowInfoList.Add(handle, window_info);
            }

            window_info = IWindowInfoList[handle];

            if (!control2ContextList.ContainsKey(handle))
            {
                control2ContextList.Add(handle, new GraphicsContext(GraphicsMode.Default, window_info));

                string versionString = GL.GetString(StringName.Version);
                if (versionString == "")
                    return;
                string majorString = versionString.Split(' ')[0];
                var v = new Version(majorString);
                npotSupported = v.Major >= 2;


                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, width, height, 0, -1, 1);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                GL.Viewport(0, 0, width, height);
            }

            control2Context = control2ContextList[handle];

            //var implementation = new GLControlFactory().CreateGLControl(format, this);

            //context = implementation.CreateContext(major, minor, flags);

            control2Context.MakeCurrent(window_info);

            GL.Viewport(0, 0, width, height);

            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
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
        public Matrix Transform { get; set; }
        public RectangleF VisibleClipBounds { get; }

        public void AddMetafileComment(byte[] data)
        {
            throw new NotImplementedException();
        }

        public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
        {
            throw new NotImplementedException();
        }

        public GraphicsContainer BeginContainer()
        {
            throw new NotImplementedException();
        }

        public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
        {
            throw new NotImplementedException();
        }

        public void Clear(Color color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        const double rad2deg = (180 / Math.PI);
        const double deg2rad = (1.0 / rad2deg);

        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            GL.LineWidth(pen.Width);
            GL.Color4(pen.Color);

            GL.Begin(PrimitiveType.LineStrip);

            startAngle = 360 - startAngle;
            startAngle -= 30;

            float x = 0, y = 0;
            for (float i = startAngle; i <= startAngle + sweepAngle; i++)
            {
                x = (float)Math.Sin(i * deg2rad) * rect.Width / 2;
                y = (float)Math.Cos(i * deg2rad) * rect.Height / 2;
                x = x + rect.X + rect.Width / 2;
                y = y + rect.Y + rect.Height / 2;
                GL.Vertex2(x, y);
            }

            GL.End();
        }

        public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, (RectangleF)rect, startAngle, sweepAngle);
        }

        public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            throw new NotImplementedException();
        }

        public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            throw new NotImplementedException();
        }

        public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            throw new NotImplementedException();
        }

        public void DrawBeziers(Pen pen, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawBeziers(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve(Pen pen, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, Point[] points, float tension)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
        {
            throw new NotImplementedException();
        }

        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            GL.LineWidth(pen.Width);
            GL.Color4(pen.Color);

            GL.Begin(PrimitiveType.LineLoop);
            float x, y;
            for (float i = 0; i < 360; i += 1)
            {
                x = (float)Math.Sin(i * deg2rad) * rect.Width / 2;
                y = (float)Math.Cos(i * deg2rad) * rect.Height / 2;
                x = x + rect.X + rect.Width / 2;
                y = y + rect.Y + rect.Height / 2;
                GL.Vertex2(x, y);
            }

            GL.End();
        }

        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, float x, float y)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            DrawImage(image, (int)x, (int)y, (int)width, (int)height);
        }

        public void DrawImage(Image image, Point point)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, int x, int y)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.ClearOutputChannelColorProfile();
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private class character
        {
            public GraphicsPath pth;
            public Bitmap bitmap;
            public int gltextureid;
            public int width;
            public int size;
        }

        [Browsable(false)] public static bool npotSupported { get; private set; }
        private character[] _texture = new character[2];

        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            int textureno = 0;
            var img = image;
            if (img == null)
                return;

            if (_texture[textureno] == null)
                _texture[textureno] = new character();

            // If the image is already a bitmap and we support NPOT textures then simply use it.
            if (npotSupported && img is Bitmap)
            {
                _texture[textureno].bitmap = (Bitmap)img;
                _texture[textureno].bitmap.MakeTransparent(Color.Transparent);
            }
            else
            {
                // Otherwise we have to resize img to be POT.
                _texture[textureno].bitmap = ResizeImage(img, 512, 512);
            }
            

            // generate the texture
            if (_texture[textureno].gltextureid == 0)
            {
                GL.GenTextures(1, out _texture[textureno].gltextureid);
            }

            GL.BindTexture(TextureTarget.Texture2D, _texture[textureno].gltextureid);

            BitmapData data = _texture[textureno].bitmap.LockBits(
                new Rectangle(0, 0, _texture[textureno].bitmap.Width, _texture[textureno].bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // create the texture type/dimensions
            if (_texture[textureno].width != _texture[textureno].bitmap.Width)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                _texture[textureno].width = data.Width;
            }
            else
            {
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, data.Width, data.Height,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            }

            _texture[textureno].bitmap.UnlockBits(data);

            bool polySmoothEnabled = GL.IsEnabled(EnableCap.PolygonSmooth);
            if (polySmoothEnabled)
                GL.Disable(EnableCap.PolygonSmooth);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, _texture[textureno].gltextureid);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.ClampToEdge);
                
            GL.Begin(PrimitiveType.TriangleStrip);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(x, y);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(x, y + height);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(x + width, y);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(x + width, y + height);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
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

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr,
            Graphics.DrawImageAbort callback)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttrs)
        {
            DrawImage(ResizeImage(image, destRect.Width, destRect.Height), destRect.X, destRect.Y, destRect.Width,
                destRect.Height);
            //throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit,
            ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, Point point)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, int x, int y)
        {
            DrawImage(image, x, y, image.Width, image.Height);
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            GL.Color4(pen.Color);
            GL.LineWidth(pen.Width);
            if (pen.DashStyle != DashStyle.Solid)
            {
                GL.LineStipple(1, 0xAAAA);
                GL.Enable(EnableCap.LineStipple);
            }

            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x2, y2);
            GL.End();

            GL.Disable(EnableCap.LineStipple);
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            DrawLine(pen, (float)x1, y1, x2, y2);
        }

        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            throw new NotImplementedException();
        }

        public void DrawLines(Pen pen, PointF[] points)
        {
            PointF last = new PointF(99999,0);
            foreach (var pointF in points)
            {
                if (last.X!=99999)
                    DrawLine(pen, last.X, last.Y, pointF.X, pointF.Y);
                last = pointF;
            }
        }

        public void DrawLines(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            List<PointF> list = new List<PointF>();
            PointF lastpnt = PointF.Empty;
            for (int i = 0; i < path.PointCount; i++)
            {
                var pnt = path.PathPoints[i];
                var type = path.PathTypes[i];

                //start
                if (type == 0)
                {
                    if (list.Count != 0)
                        DrawPolygon(pen, list.ToArray());
                    list.Clear();
                    list.Add(pnt);
                    continue;
                }

                // last point in a closed subpath
                if ((type & 0x80) > 0)
                {
                    list.Add(pnt);
                    list.Add(list[0]);
                    DrawPolygon(pen, list.ToArray());
                    list.Clear();
                    continue;
                }

                // one of 2 endpoints of a line
                if (type == 1)
                {
                    list.Add(pnt);

                    continue;
                }

                // is an endpoint 
                if (type == 3)
                {
                    list.Add(pnt);
                    DrawPolygon(pen, list.ToArray());
                    list.Clear();
                }
            }

            if (list.Count >= 2)
                DrawLines(pen, list.ToArray());
        }

        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect, startAngle, sweepAngle);
        }

        public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPolygon(Pen pen, PointF[] points)
        {
            GL.LineWidth(pen.Width);
            GL.Color4(pen.Color);

            GL.Begin(PrimitiveType.LineLoop);
            foreach (PointF pnt in points)
            {
                GL.Vertex2(pnt.X, pnt.Y);
            }

            GL.End();
        }

        public void DrawPolygon(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            GL.LineWidth(pen.Width);
            GL.Color4(pen.Color);

            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(x, y);
            GL.Vertex2(x + width, y);
            GL.Vertex2(x + width, y + height);
            GL.Vertex2(x, y + height);
            GL.End();
        }

        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            DrawRectangle(pen, (float)x, y, width, height);
        }

        public void DrawRectangles(Pen pen, RectangleF[] rects)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangles(Pen pen, Rectangle[] rects)
        {
            throw new NotImplementedException();
        }

        static private Dictionary<int, character> charDict = new Dictionary<int, character>();

        public void DrawString(string text, Font font, Brush brush1, float x, float y)
        {
            if (text == null || text == "")
                return;

            var brush = (SolidBrush) brush1;
            var fontsize = font.Size;
            Pen _p = new Pen(brush.Color, 2);

            float maxy = 1;

            foreach (char cha in text)
            {
                int charno = (int)cha;

                int charid = charno ^ (int)(fontsize * 1000) ^ brush.Color.ToArgb();

                if (!charDict.ContainsKey(charid))
                {
                    charDict[charid] = new character()
                    {
                        bitmap = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb),
                        size = (int)fontsize
                    };

                    charDict[charid].bitmap.MakeTransparent(Color.Transparent);

                    //charbitmaptexid

                    float maxx = 0;// this.Width / 150; // for space


                    // create bitmap
                    using (var gfx = Graphics.FromImage(charDict[charid].bitmap))
                    {
                        var pth = new GraphicsPath();

                        if (text != null)
                            pth.AddString(cha + "", font.FontFamily, 0, fontsize + 2, new Point((int)0, (int)0),
                                StringFormat.GenericTypographic);

                        charDict[charid].pth = pth;

                        gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        //gfx.DrawPath(_p, pth);

                        //Draw the face

                        gfx.FillPath(brush, pth);


                        if (pth.PointCount > 0)
                        {
                            foreach (PointF pnt in pth.PathPoints)
                            {
                                if (pnt.X > maxx)
                                    maxx = pnt.X;

                                if (pnt.Y > maxy)
                                    maxy = pnt.Y;
                            }
                        }
                    }

                    charDict[charid].width = (int)(maxx + 2);

                    //charbitmaps[charid] = charbitmaps[charid].Clone(new RectangleF(0, 0, maxx + 2, maxy + 2), charbitmaps[charid].PixelFormat);

                    //charbitmaps[charno * (int)fontsize].Save(charno + " " + (int)fontsize + ".png");

                    // create texture
                    int textureId;
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode,
                        (float)TextureEnvModeCombine.Replace); //Important, or wrong color on some computers

                    Bitmap bitmap = charDict[charid].bitmap;
                    GL.GenTextures(1, out textureId);
                    GL.BindTexture(TextureTarget.Texture2D, textureId);

                    BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                        (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                        (int)TextureMagFilter.Linear);

                    //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                    //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                    GL.Flush();
                    bitmap.UnlockBits(data);

                    charDict[charid].gltextureid = textureId;
                }

                float scale = 1.0f;

                // dont draw spaces
                if (cha != ' ')
                {
                    /*
                    TranslateTransform(x, y);
                    DrawPath(this._p, charDict[charid].pth);

                    //Draw the face

                    FillPath(brush, charDict[charid].pth);

                    TranslateTransform(-x, -y);
                    */
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, charDict[charid].gltextureid);

                    GL.Begin(PrimitiveType.TriangleFan);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y);
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + charDict[charid].bitmap.Width * scale, y);
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + charDict[charid].bitmap.Width * scale, y + charDict[charid].bitmap.Height * scale);
                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x + 0, y + charDict[charid].bitmap.Height * scale);
                    GL.End();

                    //GL.Disable(EnableCap.Blend);
                    GL.Disable(EnableCap.Texture2D);
                }

                x += charDict[charid].width * scale;
            }
        }

        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            DrawString(s, font, brush, x, y);
            //throw new NotImplementedException();
        }

        public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            DrawString(s, font, brush, layoutRectangle.X, layoutRectangle.Y);
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            DrawString(s, font, brush, layoutRectangle.X, layoutRectangle.Y);
        }

        public void EndContainer(GraphicsContainer container)
        {
            throw new NotImplementedException();
        }

        public void ExcludeClip(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void ExcludeClip(Region region)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(Brush brush, RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(Brush brush, float xi, float yi, float width, float height)
        {
            GL.Color4(((SolidBrush)brush).Color);
            float x, y;
            GL.Begin(PrimitiveType.LineLoop);
            for (float i = 0; i < 360; i += 1)
            {
                x = (float)Math.Sin(i * deg2rad) * width / 2;
                y = (float)Math.Cos(i * deg2rad) * height / 2;
                x = x + xi + width / 2;
                y = y + yi + height / 2;
                GL.Vertex2(x, y);
            }

            GL.End();
        }

        public void FillEllipse(Brush brush, Rectangle rect)
        {
            FillEllipse(brush, (float)rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            Tess tess = new Tess();

            var contour = path.PathPoints.Select(a =>
            {
                return new ContourVertex() {Position = new Vec3() {X = a.X, Y = a.Y}};
            }).ToArray();

            // Add the contour with a specific orientation, use "Original" if you want to keep the input orientation.
            tess.AddContour(contour, LibTessDotNet.ContourOrientation.Clockwise);
            tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3);

            //Console.WriteLine("Output triangles:");
            int numTriangles = tess.ElementCount;

            GL.Color4(((SolidBrush)brush).Color.R / 255f, ((SolidBrush)brush).Color.G / 255f,
                ((SolidBrush)brush).Color.B / 255f, ((SolidBrush)brush).Color.A / 255f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            for (int i = 0; i < numTriangles; i++)
            {
                var v0 = tess.Vertices[tess.Elements[i * 3]].Position;
                var v1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
                var v2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;
                //Console.WriteLine("#{0} ({1:F1},{2:F1}) ({3:F1},{4:F1}) ({5:F1},{6:F1})", i, v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y);

                GL.LineWidth(0);
                GL.Begin(PrimitiveType.TriangleFan);
                GL.Vertex2(v0.X, v0.Y);
                GL.Vertex2(v1.X, v1.Y);
                GL.Vertex2(v2.X, v2.Y);
                GL.End();
            }

        }

        public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void FillPie(Brush brush, float xi, float yi, float width, float height, float startAngle, float sweepAngle)
        {
            GL.Color4(((SolidBrush)brush).Color.R / 255f, ((SolidBrush)brush).Color.G / 255f,
                ((SolidBrush)brush).Color.B / 255f, ((SolidBrush)brush).Color.A / 255f);

            float x, y;
            GL.Begin(PrimitiveType.LineLoop);
            for (float i = startAngle; i < (startAngle+sweepAngle); i += 1)
            {
                x = (float)Math.Sin(i * deg2rad) * width / 2;
                y = (float)Math.Cos(i * deg2rad) * height / 2;
                x = x + xi + width / 2;
                y = y + yi + height / 2;
                GL.Vertex2(x, y);
            }

            GL.End();
        }

        public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            FillPie(brush, (float)x, y, width, height, startAngle, sweepAngle);
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            Tess tess = new Tess();

            var contour = points.Select(a =>
            {
                return new ContourVertex() { Position = new Vec3() { X = a.X, Y = a.Y } };
            }).ToArray();

            // Add the contour with a specific orientation, use "Original" if you want to keep the input orientation.
            tess.AddContour(contour, LibTessDotNet.ContourOrientation.Clockwise);
            tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3);

            //Console.WriteLine("Output triangles:");
            int numTriangles = tess.ElementCount;

            GL.Color4(((SolidBrush)brush).Color.R / 255f, ((SolidBrush)brush).Color.G / 255f,
                ((SolidBrush)brush).Color.B / 255f, ((SolidBrush)brush).Color.A / 255f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            for (int i = 0; i < numTriangles; i++)
            {
                var v0 = tess.Vertices[tess.Elements[i * 3]].Position;
                var v1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
                var v2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;
                //Console.WriteLine("#{0} ({1:F1},{2:F1}) ({3:F1},{4:F1}) ({5:F1},{6:F1})", i, v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y);

                //GL.LineWidth(0);
                GL.Begin(PrimitiveType.TriangleFan);
                GL.Vertex2(v0.X, v0.Y);
                GL.Vertex2(v1.X, v1.Y);
                GL.Vertex2(v2.X, v2.Y);
                GL.End();
            }
        }

        public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            float x1 = rect.X;
            float y1 = rect.Y;

            float width = rect.Width;
            float height = rect.Height;

            GL.Begin(PrimitiveType.TriangleFan);

            GL.LineWidth(0);

            if (((Type)brush.GetType()) == typeof(LinearGradientBrush))
            {
                LinearGradientBrush temp = (LinearGradientBrush)brush;
                GL.Color4(temp.LinearColors[0]);
            }
            else
            {
                GL.Color4(((SolidBrush)brush).Color.R / 255f, ((SolidBrush)brush).Color.G / 255f,
                    ((SolidBrush)brush).Color.B / 255f, ((SolidBrush)brush).Color.A / 255f);
            }

            GL.Vertex2(x1, y1);
            GL.Vertex2(x1 + width, y1);

            if (((Type)brush.GetType()) == typeof(LinearGradientBrush))
            {
                LinearGradientBrush temp = (LinearGradientBrush)brush;
                GL.Color4(temp.LinearColors[1]);
            }
            else
            {
                GL.Color4(((SolidBrush)brush).Color.R / 255f, ((SolidBrush)brush).Color.G / 255f,
                    ((SolidBrush)brush).Color.B / 255f, ((SolidBrush)brush).Color.A / 255f);
            }

            GL.Vertex2(x1 + width, y1 + height);
            GL.Vertex2(x1, y1 + height);
            GL.End();
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(Brush brush, Rectangle rect)
        {
            FillRectangle(brush, (RectangleF) rect);
        }

        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            throw new NotImplementedException();
        }

        public void FillRectangles(Brush brush, Rectangle[] rects)
        {
            throw new NotImplementedException();
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

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted,
            out int linesFilled)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font)
        {
            return Graphics.FromImage(new Bitmap(1, 1)).MeasureString(text, font);
        }

        public SizeF MeasureString(string text, Font font, int width)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, int width, StringFormat format)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void ResetTransform()
        {
            GL.LoadIdentity();
        }

        public void Restore(GraphicsState gstate)
        {
            throw new NotImplementedException();
        }

        public void RotateTransform(float angle)
        {
            GL.Rotate(angle, 0, 0, 1);
        }

        public void RotateTransform(float angle, MatrixOrder order)
        {
            throw new NotImplementedException();
        }

        public GraphicsState Save()
        {
            throw new NotImplementedException();
        }

        public void ScaleTransform(float sx, float sy)
        {
            throw new NotImplementedException();
        }

        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
                GL.Scale(sx, sy, 1);

            //throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SetClip(Rectangle rect, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void SetClip(RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void SetClip(RectangleF rect, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void SetClip(GraphicsPath path)
        {
            throw new NotImplementedException();
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
            GL.Translate(dx, dy, 0f);
        }

        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            if (MatrixOrder.Append == order)
            {
                float[] model = new float[16];
                GL.GetFloat(GetPName.ModelviewMatrix, model);

                var current = new Matrix4(model[0], model[1], model[2], model[3],
                    model[4], model[5], model[6], model[7],
                    model[8], model[9], model[10], model[11],
                    model[12], model[13], model[14], model[15]);

                var matrix = current * Matrix4.CreateTranslation(dx, dy, 0);
                GL.LoadIdentity();
                GL.LoadMatrix(ref matrix);
                //GL.GetFloat(GetPName.ModelviewMatrix, model);
            }
            else
            {
                GL.Translate(dx, dy, 0);
            }
        }

        public void SwapBuffer()
        {
            control2Context.SwapBuffers();
            GL.DeleteTexture(_texture[0].gltextureid);
        }

        public void Dispose()
        {
            control2Context.Dispose();
        }
    }

    public class WHandle : IWindowInfo
    {
        public WHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public void Dispose()
        {
            
        }
    }
}