﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Collections;
using System.Threading;
 
using System.Drawing.Drawing2D;
using log4net;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using OpenTK.Graphics;


// Control written by Michael Oborne 2011
// dual opengl and GDI+

namespace MissionPlanner.Controls
{
    public class HUD : GLControl
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        object paintlock = new object();
        object streamlock = new object();
        MemoryStream _streamjpg = new MemoryStream();
        //[System.ComponentModel.Browsable(false)]
        public MemoryStream streamjpg { get { lock (streamlock) { return _streamjpg; } } set { lock (streamlock) { _streamjpg = value; } } }
        /// <summary>
        /// this is to reduce cpu usage
        /// </summary>
        public bool streamjpgenable = false;

        public bool HoldInvalidation = false;

        public bool Russian {get;set;}

        class character {
            public Bitmap bitmap;
            public int gltextureid;
            public int width;
            public int size;
        }

        Dictionary<int, character> charDict = new Dictionary<int, character>();

        //Bitmap[] charbitmaps = new Bitmap[6000];
        //int[] charbitmaptexid = new int[6000];
        //int[] charwidth = new int[6000];

        public int huddrawtime = 0;

        public bool opengl { get { return UseOpenGL; } set { UseOpenGL = value; } }

        bool started = false;

        public bool SixteenXNine = false;

        static ImageCodecInfo ici = GetImageCodec("image/jpeg");
        static EncoderParameters eps = new EncoderParameters(1);

        public HUD()
        {
            if (this.DesignMode)
            {
                opengl = false;
                //return;
            }

            this.Name = "Hud";

            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L); // or whatever other quality value you want

            objBitmap.MakeTransparent();

            //InitializeComponent();

            graphicsObject = this;
            graphicsObjectGDIP = Graphics.FromImage(objBitmap);
        }

        /*
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HUD));
            this.SuspendLayout();
            // 
            // HUD
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Name = "HUD";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

        }*/

        float _roll = 0;
        float _navroll = 0;
        float _pitch = 0;
        float _navpitch = 0;
        float _heading = 0;
        float _targetheading = 0;
        float _alt = 0;
        float _targetalt = 0;
        float _groundspeed = 0;
        float _airspeed = 0;
        bool _lowgroundspeed = false;
        bool _lowairspeed = false;
        float _targetspeed = 0;
        float _batterylevel = 0;
        float _current = 0;
        float _batteryremaining = 0;
        float _gpsfix = 0;
        float _gpshdop = 0;
        float _disttowp = 0;
        float _groundcourse = 0;
        float _xtrack_error = 0;
        float _turnrate = 0;
        float _verticalspeed = 0;
        float _linkqualitygcs = 0;
        DateTime _datetime;
        string _mode = "Manual";
        int _wpno = 0;
        
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float roll { get { return _roll; } set { if (_roll != value) { _roll = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float navroll { get { return _navroll; } set { if (_navroll != value) { _navroll = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float pitch { get { return _pitch; } set { if (_pitch != value) { _pitch = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float navpitch { get { return _navpitch; } set { if (_navpitch != value) { _navpitch = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float heading { get { return _heading; } set { if (_heading != value) { _heading = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float targetheading { get { return _targetheading; } set { if (_targetheading != value) { _targetheading = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float alt { get { return _alt; } set { if (_alt != value) { _alt = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float targetalt { get { return _targetalt; } set { if (_targetalt != value) { _targetalt = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float groundspeed { get { return _groundspeed; } set { if (_groundspeed != value) { _groundspeed = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float airspeed { get { return _airspeed; } set { if (_airspeed != value) { _airspeed = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool lowgroundspeed { get { return _lowgroundspeed; } set { if (_lowgroundspeed != value) { _lowgroundspeed = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool lowairspeed { get { return _lowairspeed; } set { if (_lowairspeed != value) { _lowairspeed = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float targetspeed { get { return _targetspeed; } set { if (_targetspeed != value) { _targetspeed = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float batterylevel { get { return _batterylevel; } set { if (_batterylevel != value) { _batterylevel = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float batteryremaining { get { return _batteryremaining; } set { if (_batteryremaining != value) { _batteryremaining = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float current { get { return _current; } set { if (_current != value) { _current = value; this.Invalidate(); if (_current > 0) batteryon = true; } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float gpsfix { get { return _gpsfix; } set { if (_gpsfix != value) { _gpsfix = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float gpshdop { get { return _gpshdop; } set { if (_gpshdop != value) { _gpshdop = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float disttowp { get { return _disttowp; } set { if (_disttowp != value) { _disttowp = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public string mode { get { return _mode; } set { if (_mode != value) { _mode = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public int wpno { get { return _wpno; } set { if (_wpno != value) { _wpno = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float groundcourse { get { return _groundcourse; } set { if (_groundcourse != value) { _groundcourse = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float xtrack_error { get { return _xtrack_error; } set { if (_xtrack_error != value) { _xtrack_error = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float turnrate { get { return _turnrate; } set { if (_turnrate != value) { _turnrate = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float verticalspeed { get { return _verticalspeed; } set { if (_verticalspeed != value) { _verticalspeed = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float linkqualitygcs { get { return _linkqualitygcs; } set { if (_linkqualitygcs != value) { _linkqualitygcs = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public DateTime datetime { get { return _datetime; } set { if (_datetime != value) { _datetime = value; this.Invalidate(); } } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool failsafe { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool lowvoltagealert { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool connected { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float groundalt { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool status { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public string message { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public DateTime messagetime { get; set; }

        bool statuslast = false;
        DateTime armedtimer = DateTime.MinValue;

        public struct Custom
        {
            //public Point Position;
            //public float FontSize;
            public string Header;
            public System.Reflection.PropertyInfo Item;
            public double GetValue
            {
                get
                {
                    if (Item.PropertyType == typeof(Single))
                    {
                        return (double)(float)Item.GetValue(src, null);
                    }
                    if (Item.PropertyType == typeof(Int32))
                    {
                        return (double)(int)Item.GetValue(src, null);
                    }
                    if (Item.PropertyType == typeof(double))
                    {
                        return (double)Item.GetValue(src, null);
                    }

                    throw new Exception("Bad data type");
                }
            }
            public static object src { get; set; }
        }

        public Hashtable CustomItems = new Hashtable();

        public bool bgon = true;
        public bool hudon = true;
        public bool batteryon = true;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public Color hudcolor { get { return whitePen.Color; } set { _hudcolor = value; whitePen = new Pen(value, 2); } }
        Color _hudcolor = Color.White;
        Pen whitePen = new Pen(Color.White, 2);

        public Image bgimage { set { while (inOnPaint) { System.Threading.Thread.Sleep(1); } if (_bgimage != null) _bgimage.Dispose(); try { _bgimage = (Image)value; } catch { _bgimage = null; } this.Invalidate(); } }
        Image _bgimage;

        // move these global as they rarely change - reduce GC
        Font font = new Font(HUDT.Font, 10);
        public Bitmap objBitmap = new Bitmap(1024, 1024,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        int count = 0;
        DateTime countdate = DateTime.Now;
        HUD graphicsObject;
        Graphics graphicsObjectGDIP;

        DateTime starttime = DateTime.MinValue;

        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HUD));

        public override void Refresh()
        {
            if (!ThisReallyVisible())
            {
              //  return;
            }

            //base.Refresh();
            using (Graphics gg = this.CreateGraphics())
            {
                OnPaint(new PaintEventArgs(gg, this.ClientRectangle));
            }
        }

        DateTime lastinvalidate = DateTime.MinValue;

        /// <summary>
        /// Override to prevent offscreen drawing the control - mono mac
        /// </summary>
        public new void Invalidate()
        {
            if (HoldInvalidation)
                return;

            if (!ThisReallyVisible())
            {
                //  return;
            }

            lastinvalidate = DateTime.Now;

            base.Invalidate();
        }

        /// <summary>
        /// this is to fix a mono off screen drawing issue
        /// </summary>
        /// <returns></returns>
        public bool ThisReallyVisible()
        {
            //Control ctl = Control.FromHandle(this.Handle);
            return this.Visible;
        } 

        protected override void OnLoad(EventArgs e)
        {
            if (opengl)
            {
                try
                {

                    OpenTK.Graphics.GraphicsMode test = this.GraphicsMode;
                   // log.Info(test.ToString());
                    log.Info("Vendor: " + GL.GetString(StringName.Vendor));
                    log.Info("Version: " + GL.GetString(StringName.Version));
                    log.Info("Device: " + GL.GetString(StringName.Renderer));
                    //Console.WriteLine("Extensions: " + GL.GetString(StringName.Extensions));

                    int[] viewPort = new int[4];

                    GL.GetInteger(GetPName.Viewport, viewPort);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(0, Width, Height, 0, -1, 1);
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadIdentity();

                    GL.PushAttrib(AttribMask.DepthBufferBit);
                    GL.Disable(EnableCap.DepthTest);
                    //GL.Enable(EnableCap.Texture2D); 
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    GL.Enable(EnableCap.Blend);

                }
                catch (Exception ex) { log.Error("HUD opengl onload 1 ", ex); }

                try
                {
                    GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

                    GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
                    GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
                    GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

                    GL.Hint(HintTarget.TextureCompressionHint, HintMode.Nicest);
                }
                catch (Exception ex) { log.Error("HUD opengl onload 2 ", ex); }

                try
                {

                    GL.Enable(EnableCap.LineSmooth);
                    GL.Enable(EnableCap.PointSmooth);
                    GL.Enable(EnableCap.PolygonSmooth);

                }
                catch (Exception ex) { log.Error("HUD opengl onload 3 ", ex); }
            }

            started = true;
        }

        bool inOnPaint = false;
        string otherthread = "";


        protected override void OnPaint(PaintEventArgs e)
        {
            //GL.Enable(EnableCap.AlphaTest)

           // Console.WriteLine("hud paint");

           // Console.WriteLine("hud ms " + (DateTime.Now.Millisecond));

            if (!started)
                return;

            if (this.DesignMode)
            {
                e.Graphics.Clear(this.BackColor);
                e.Graphics.Flush();
            }

            if ((DateTime.Now - starttime).TotalMilliseconds < 30 && (_bgimage == null))
            {
                //Console.WriteLine("ms "+(DateTime.Now - starttime).TotalMilliseconds);
                //e.Graphics.DrawImageUnscaled(objBitmap, 0, 0);          
                return;              
            }

            lock (this)
            {

                if (inOnPaint)
                {
                    log.Info("Was in onpaint Hud th:" + System.Threading.Thread.CurrentThread.Name + " in " + otherthread);
                    return;
                }

                otherthread = System.Threading.Thread.CurrentThread.Name;

                inOnPaint = true;

            }

            starttime = DateTime.Now;

            try
            {

                if (opengl)
                {
                    // make this gl window and thread current
                    MakeCurrent();

                    GL.Clear(ClearBufferMask.ColorBufferBit);

                }

                doPaint(e);

                if (opengl)
                {
                    this.SwapBuffers();

                    // free from this thread
                    Context.MakeCurrent(null);
                }

            }
            catch (Exception ex) { log.Info(ex.ToString()); }

            count++;

            huddrawtime += (int)(DateTime.Now - starttime).TotalMilliseconds;

            if (DateTime.Now.Second != countdate.Second)
            {
                countdate = DateTime.Now;
                Console.WriteLine("HUD " + count + " hz drawtime " + (huddrawtime / count) + " gl " + opengl);
                if ((huddrawtime / count) > 1000)
                    opengl = false;

                count = 0;
                huddrawtime = 0;
            }

            inOnPaint = false;
        }

        void Clear(Color color)
        {
            if (opengl)
            {
                GL.ClearColor(color);

            }
            else
            {
                graphicsObjectGDIP.Clear(color);
            }
        }

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public void DrawArc(Pen penn,RectangleF rect, float start,float degrees)
        {
            if (opengl)
            {
                GL.LineWidth(penn.Width);
                GL.Color4(penn.Color);

                GL.Begin(PrimitiveType.LineStrip);

                start = 360 - start;
                start -= 30;
 
                float x = 0, y = 0;
                for (float i = start; i <= start + degrees; i++)
                {
                    x = (float)Math.Sin(i * deg2rad) * rect.Width / 2;
                    y = (float)Math.Cos(i * deg2rad) * rect.Height / 2;
                    x = x + rect.X + rect.Width / 2;
                    y = y + rect.Y + rect.Height / 2;
                    GL.Vertex2(x, y);
                }
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawArc(penn, rect, start, degrees);
            }
        }

        public void DrawEllipse(Pen penn, Rectangle rect)
        {
            if (opengl)
            {
                GL.LineWidth(penn.Width);
                GL.Color4(penn.Color);

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
            else
            {
                graphicsObjectGDIP.DrawEllipse(penn, rect);
            }
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

        int texture;
        Bitmap bitmap = new Bitmap(512,512);

        public void DrawImage(Image img, int x, int y, int width, int height)
        {
            if (opengl)
            {
                if (img == null)
                    return;
                //bitmap = new Bitmap(512,512);

                bitmap = ResizeImage(img, bitmap.Width, bitmap.Height);

                GL.DeleteTexture(texture);

                GL.GenTextures(1, out texture);
                GL.BindTexture(TextureTarget.Texture2D, texture);

                BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //Console.WriteLine("w {0} h {1}",data.Width, data.Height);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bitmap.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                GL.Enable(EnableCap.Texture2D);

                GL.BindTexture(TextureTarget.Texture2D, texture);

                GL.Begin(PrimitiveType.Quads);

                GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(0, this.Height);
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(this.Width, this.Height);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(this.Width, 0);
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0, 0);

                GL.End();

                GL.Disable(EnableCap.Texture2D);
            }
            else
            {
                graphicsObjectGDIP.DrawImage(img,x,y,width,height);
            }
        }

        public void DrawPath(Pen penn, GraphicsPath gp)
        {
            try
            {
               DrawPolygon(penn, gp.PathPoints);
            }
            catch { }
        }

        public void FillPath(Brush brushh, GraphicsPath gp)
        {
            try
            {
                FillPolygon(brushh, gp.PathPoints);
            }
            catch { }
        }

        public void SetClip(Rectangle rect)
        {
            
        }

        public void ResetClip()
        {

        }

        public void ResetTransform()
        {
            if (opengl)
            {
                GL.LoadIdentity();
            }
            else
            {
                graphicsObjectGDIP.ResetTransform();
            }
        }

        public void RotateTransform(float angle)
        {
            if (opengl)
            {
                GL.Rotate(angle, 0, 0, 1);
            }
            else
            {
                graphicsObjectGDIP.RotateTransform(angle);
            }
        }

        public void TranslateTransform(float x, float y)
        {
            if (opengl)
            {
                GL.Translate(x, y, 0f);
            }
            else
            {
                graphicsObjectGDIP.TranslateTransform(x, y);
            }
        }

        public void FillPolygon(Brush brushh, Point[] list)
        {
            if (opengl)
            {
                GL.Begin(PrimitiveType.TriangleFan);
                GL.Color4(((SolidBrush)brushh).Color);
                foreach (Point pnt in list)
                {
                    GL.Vertex2(pnt.X, pnt.Y);
                }
                GL.Vertex2(list[list.Length - 1].X, list[list.Length - 1].Y);
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.FillPolygon(brushh, list);
            }
        }

        public void FillPolygon(Brush brushh, PointF[] list)
        {
            if (opengl)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color4(((SolidBrush)brushh).Color);
                foreach (PointF pnt in list)
                {
                    GL.Vertex2(pnt.X, pnt.Y);
                }
                GL.Vertex2(list[0].X, list[0].Y);
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.FillPolygon(brushh, list);
            }
        }

        public void DrawPolygon(Pen penn, Point[] list)
        {
            if (opengl)
            {
                GL.LineWidth(penn.Width);
                GL.Color4(penn.Color);

                GL.Begin(PrimitiveType.LineLoop);
                foreach (Point pnt in list)
                {
                    GL.Vertex2(pnt.X, pnt.Y);
                }
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawPolygon(penn, list);
            }                       
        }

        public void DrawPolygon(Pen penn, PointF[] list)
        {         
            if (opengl)
            {
            GL.LineWidth(penn.Width);
            GL.Color4(penn.Color);

            GL.Begin(PrimitiveType.LineLoop);
            foreach (PointF pnt in list)
            {
                GL.Vertex2(pnt.X, pnt.Y);
            }

            GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawPolygon(penn, list);
            } 
        }


        public void FillRectangle(Brush brushh, RectangleF rectf)
        {
            if (opengl)
            {
                float x1 = rectf.X;
                float y1 = rectf.Y;

                float width = rectf.Width;
                float height = rectf.Height;

                GL.Begin(PrimitiveType.Quads);

                GL.LineWidth(0);

                if (((Type)brushh.GetType()) == typeof(LinearGradientBrush))
                {
                    LinearGradientBrush temp = (LinearGradientBrush)brushh;
                    GL.Color4(temp.LinearColors[0]);
                }
                else
                {
                    GL.Color4(((SolidBrush)brushh).Color.R / 255f, ((SolidBrush)brushh).Color.G / 255f, ((SolidBrush)brushh).Color.B / 255f, ((SolidBrush)brushh).Color.A / 255f);
                }

                GL.Vertex2(x1, y1);
                GL.Vertex2(x1 + width, y1);

                if (((Type)brushh.GetType()) == typeof(LinearGradientBrush))
                {
                    LinearGradientBrush temp = (LinearGradientBrush)brushh;
                    GL.Color4(temp.LinearColors[1]);
                }
                else
                {
                    GL.Color4(((SolidBrush)brushh).Color.R / 255f, ((SolidBrush)brushh).Color.G / 255f, ((SolidBrush)brushh).Color.B / 255f, ((SolidBrush)brushh).Color.A / 255f);
                }

                GL.Vertex2(x1 + width, y1 + height);
                GL.Vertex2(x1, y1 + height);
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.FillRectangle(brushh, rectf);
            }
        }

        public void DrawRectangle(Pen penn, RectangleF rect)
        {
            DrawRectangle(penn, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawRectangle(Pen penn, double x1, double y1, double width, double height)
        {

            if (opengl)
            {
                GL.LineWidth(penn.Width);
                GL.Color4(penn.Color);

                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(x1, y1);
                GL.Vertex2(x1 + width, y1);
                GL.Vertex2(x1 + width, y1 + height);
                GL.Vertex2(x1, y1 + height);
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawRectangle(penn, (float)x1, (float)y1, (float)width, (float)height);
            }
        }

        public void DrawLine(Pen penn, double x1, double y1, double x2, double y2)
        {

            if (opengl)
            {
                GL.Color4(penn.Color);
                GL.LineWidth(penn.Width);

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(x1, y1);
                GL.Vertex2(x2, y2);
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawLine(penn, (float)x1, (float)y1, (float)x2, (float)y2);
            }
        }

        Pen blackPen = new Pen(Color.Black, 2);
        Pen greenPen = new Pen(Color.Green, 2);
        Pen redPen = new Pen(Color.Red, 2);

        void doPaint(PaintEventArgs e)
        {
            //Console.WriteLine("hud paint "+DateTime.Now.Millisecond);
            bool isNaN = false;
            try
            {
                if (graphicsObjectGDIP == null || !opengl && (objBitmap.Width != this.Width || objBitmap.Height != this.Height))
                {
                    objBitmap = new Bitmap(this.Width, this.Height,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    objBitmap.MakeTransparent();
                    graphicsObjectGDIP = Graphics.FromImage(objBitmap);

                    graphicsObjectGDIP.SmoothingMode = SmoothingMode.AntiAlias;
                    graphicsObjectGDIP.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphicsObjectGDIP.CompositingMode = CompositingMode.SourceOver;
                    graphicsObjectGDIP.CompositingQuality = CompositingQuality.HighSpeed;
                    graphicsObjectGDIP.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    graphicsObjectGDIP.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                }

                graphicsObjectGDIP.InterpolationMode = InterpolationMode.Bilinear;

                try
                {
                    graphicsObject.Clear(Color.Transparent);
                }
                catch
                {
                    // this is the first posible opengl call
                    // in vmware fusion on mac, this fails, so switch back to legacy
                    opengl = false;
                }

                if (_bgimage != null)
                {
                    bgon = false;
                    graphicsObject.DrawImage(_bgimage, 0, 0, this.Width, this.Height);

                    if (hudon == false)
                    {
                        return;
                    }
                }
                else
                {
                    bgon = true;
                }


                if (float.IsNaN(_roll) || float.IsNaN(_pitch) || float.IsNaN(_heading))
                {
                    isNaN = true;

                    _roll = 0;
                    _pitch = 0;
                    _heading = 0;
                }

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);


                if (!Russian)
                {
                    // horizon
                    graphicsObject.RotateTransform(-_roll);
                }
                else
                {
                    _roll *= -1;
                }


                int fontsize = this.Height / 30; // = 10
                int fontoffset = fontsize - 10;

                float every5deg = -this.Height / 65;

                float pitchoffset = -_pitch * every5deg;

                int halfwidth = this.Width / 2;
                int halfheight = this.Height / 2;

                SolidBrush whiteBrush = new SolidBrush(whitePen.Color);

                blackPen = new Pen(Color.Black, 2);
                greenPen = new Pen(Color.Green, 2);
                redPen = new Pen(Color.Red, 2);

                if (!connected)
                {
                    whiteBrush.Color = Color.LightGray;
                    whitePen.Color = Color.LightGray;
                }
                else
                {
                    whitePen.Color = _hudcolor;
                }

                // draw sky
                if (bgon == true)
                {
                    RectangleF bg = new RectangleF(-halfwidth * 2, -halfheight * 2, this.Width * 2, halfheight * 2 + pitchoffset);

                    if (bg.Height != 0)
                    {
                        LinearGradientBrush linearBrush = new LinearGradientBrush(bg, Color.Blue,
                            Color.LightBlue, LinearGradientMode.Vertical);

                        graphicsObject.FillRectangle(linearBrush, bg);
                    }
                    // draw ground

                    bg = new RectangleF(-halfwidth * 2, pitchoffset, this.Width * 2, halfheight * 2 - pitchoffset);

                    if (bg.Height != 0)
                    {
                        LinearGradientBrush linearBrush = new LinearGradientBrush(bg, Color.FromArgb(0x9b, 0xb8, 0x24),
                            Color.FromArgb(0x41, 0x4f, 0x07), LinearGradientMode.Vertical);

                        graphicsObject.FillRectangle(linearBrush, bg);
                    }

                    //draw centerline
                    graphicsObject.DrawLine(whitePen, -halfwidth * 2, pitchoffset + 0, halfwidth * 2, pitchoffset + 0);
                }

                graphicsObject.ResetTransform();

                graphicsObject.SetClip(new Rectangle(0, this.Height / 14, this.Width, this.Height - this.Height / 14));

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                graphicsObject.RotateTransform(-_roll);

                //draw pitch           

                int lengthshort = this.Width / 14;
                int lengthlong = this.Width / 10;

                for (int a = -90; a <= 90; a += 5)
                {
                    // limit to 40 degrees
                    if (a >= _pitch - 29 && a <= _pitch + 20)
                    {
                        if (a % 10 == 0)
                        {
                            if (a == 0)
                            {
                                graphicsObject.DrawLine(greenPen, this.Width / 2 - lengthlong - halfwidth, pitchoffset + a * every5deg, this.Width / 2 + lengthlong - halfwidth, pitchoffset + a * every5deg);
                            }
                            else
                            {
                                graphicsObject.DrawLine(whitePen, this.Width / 2 - lengthlong - halfwidth, pitchoffset + a * every5deg, this.Width / 2 + lengthlong - halfwidth, pitchoffset + a * every5deg);
                            }
                            drawstring(graphicsObject, a.ToString(), font, fontsize + 2, whiteBrush, this.Width / 2 - lengthlong - 30 - halfwidth - (int)(fontoffset * 1.7), pitchoffset + a * every5deg - 8 - fontoffset);
                        }
                        else
                        {
                            graphicsObject.DrawLine(whitePen, this.Width / 2 - lengthshort - halfwidth, pitchoffset + a * every5deg, this.Width / 2 + lengthshort - halfwidth, pitchoffset + a * every5deg);
                            //drawstring(e,a.ToString(), new Font("Arial", 10), whiteBrush, this.Width / 2 - lengthshort - 20 - halfwidth, this.Height / 2 + pitchoffset + a * every5deg - 8);
                        }
                    }
                }

                graphicsObject.ResetTransform();

                // draw roll ind needle

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);
                
                Point[] pointlist = new Point[3];

                lengthlong = this.Height / 66;

                int extra = (int)(this.Height / 15 * 4.9f);

                int lengthlongex = lengthlong + 2;

                pointlist[0] = new Point(0, -lengthlongex * 2 - extra);
                pointlist[1] = new Point(-lengthlongex, -lengthlongex - extra);
                pointlist[2] = new Point(lengthlongex, -lengthlongex - extra);

                redPen.Width = 2;

                if (Math.Abs(_roll) > 45)
                {
                    redPen.Width = 4;
                }

                graphicsObject.DrawPolygon(redPen, pointlist);

                redPen.Width = 2;

                int[] array = new int[] { -60,-45, -30,-20,-10,0,10,20,30,45,60 };

                foreach (int a in array)
                {
                    graphicsObject.ResetTransform();
                    graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2); 
                    graphicsObject.RotateTransform(a - _roll);
                    drawstring(graphicsObject, Math.Abs(a).ToString("0").PadLeft(2), font, fontsize, whiteBrush, 0 - 6 - fontoffset, -lengthlong * 8 - extra);
                    graphicsObject.DrawLine(whitePen, 0, -lengthlong * 3 - extra, 0, -lengthlong * 3 - extra - lengthlong);
                }

                graphicsObject.ResetTransform();
                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                // draw roll ind
                RectangleF arcrect = new RectangleF(-lengthlong * 3 - extra, -lengthlong * 3 - extra, (extra + lengthlong * 3) * 2f, (extra + lengthlong * 3) * 2f);

                //DrawRectangle(Pens.Beige, arcrect);

                graphicsObject.DrawArc(whitePen, arcrect, 180 + 30 + -_roll, 120); // 120

                graphicsObject.ResetTransform();

                //draw centre / current att

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);//  +this.Height / 14);

                // plane wings
                if (Russian)
                    graphicsObject.RotateTransform(-_roll);

                Rectangle centercircle = new Rectangle(-halfwidth / 2, -halfwidth / 2, halfwidth, halfwidth);

              //  graphicsObject.DrawEllipse(redPen, centercircle);
                Pen redtemp = new Pen(Color.FromArgb(200, redPen.Color.R, redPen.Color.G, redPen.Color.B));
                redtemp.Width = 4.0f;
                // left
                graphicsObject.DrawLine(redtemp, centercircle.Left - halfwidth / 5, 0, centercircle.Left, 0);
                // right
                graphicsObject.DrawLine(redtemp, centercircle.Right, 0, centercircle.Right + halfwidth / 5, 0);
                // center point
                graphicsObject.DrawLine(redtemp, 0-1, 0, centercircle.Right - halfwidth / 3, 0 + halfheight / 10);
                graphicsObject.DrawLine(redtemp, 0+1, 0, centercircle.Left + halfwidth / 3, 0 + halfheight / 10);

                //draw heading ind

                graphicsObject.ResetTransform();

                graphicsObject.ResetClip();

                Rectangle headbg = new Rectangle(0, 0, this.Width - 0, this.Height / 14);

                graphicsObject.DrawRectangle(blackPen, headbg);

                SolidBrush solidBrush = new SolidBrush(Color.FromArgb(0x55, 0xff, 0xff, 0xff));

                graphicsObject.FillRectangle(solidBrush, headbg);

                // center
             //   graphicsObject.DrawLine(redPen, headbg.Width / 2, headbg.Bottom, headbg.Width / 2, headbg.Top);

                //bottom line
                graphicsObject.DrawLine(whitePen, headbg.Left + 5, headbg.Bottom - 5, headbg.Width - 5, headbg.Bottom - 5);

                float space = (headbg.Width - 10) / 120.0f;
                int start = (int)Math.Round((_heading - 60),1);

                // draw for outside the 60 deg
                if (_targetheading < start)
                {
                    greenPen.Width = 6;
                    graphicsObject.DrawLine(greenPen, headbg.Left + 5 + space * 0, headbg.Bottom, headbg.Left + 5 + space * (0), headbg.Top);
                }
                if (_targetheading > _heading + 60)
                {
                    greenPen.Width = 6;
                    graphicsObject.DrawLine(greenPen, headbg.Left + 5 + space * 60, headbg.Bottom, headbg.Left + 5 + space * (60), headbg.Top);
                }

                for (int a = start; a <= _heading + 60; a += 1)
                {
                    // target heading
                    if (((int)(a + 360) % 360) == (int)_targetheading)
                    {
                        greenPen.Width = 6;
                        graphicsObject.DrawLine(greenPen, headbg.Left + 5 + space * (a - start), headbg.Bottom, headbg.Left + 5 + space * (a - start), headbg.Top);
                    }

                    if (((int)(a + 360) % 360) == (int)_groundcourse)
                    {
                        blackPen.Width = 6;
                        graphicsObject.DrawLine(blackPen, headbg.Left + 5 + space * (a - start), headbg.Bottom, headbg.Left + 5 + space * (a - start), headbg.Top);
                        blackPen.Width = 2;
                    }

                    if ((int)a % 15 == 0)
                    {
                        //Console.WriteLine(a + " " + Math.Round(a, 1, MidpointRounding.AwayFromZero));
                        //Console.WriteLine(space +" " + a +" "+ (headbg.Left + 5 + space * (a - start)));
                        graphicsObject.DrawLine(whitePen, headbg.Left + 5 + space * (a - start), headbg.Bottom - 5, headbg.Left + 5 + space * (a - start), headbg.Bottom - 10);
                        int disp = (int)a;
                        if (disp < 0)
                            disp += 360;
                        disp = disp % 360;
                        if (disp == 0)
                        {
                            drawstring(graphicsObject, HUDT.N.PadLeft(2), font, fontsize + 4, whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset, headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                        }
                        else if (disp == 45)
                        {
                            drawstring(graphicsObject, HUDT.NE.PadLeft(2), font, fontsize + 4, whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset, headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                        }
                        else if (disp == 90)
                        {
                            drawstring(graphicsObject, HUDT.E.PadLeft(2), font, fontsize + 4, whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset, headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                        }
                        else if (disp == 135)
                        {
                            drawstring(graphicsObject, HUDT.SE.PadLeft(2), font, fontsize + 4, whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset, headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                        }
                        else if (disp == 180)
                        {
                            drawstring(graphicsObject, HUDT.S.PadLeft(2), font, fontsize + 4, whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset, headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                        }
                        else if (disp == 225)
                        {
                            drawstring(graphicsObject, HUDT.SW.PadLeft(2), font, fontsize + 4, whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset, headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                        }
                        else if (disp == 270)
                        {
                            drawstring(graphicsObject, HUDT.W.PadLeft(2), font, fontsize + 4, whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset, headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                        }
                        else if (disp == 315)
                        {
                            drawstring(graphicsObject, HUDT.NW.PadLeft(2), font, fontsize + 4, whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset, headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                        }
                        else
                        {
                            drawstring(graphicsObject, (disp % 360).ToString().PadLeft(3), font, fontsize, whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset, headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                        }
                    }
                    else if ((int)a % 5 == 0)
                    {
                        graphicsObject.DrawLine(whitePen, headbg.Left + 5 + space * (a - start), headbg.Bottom - 5, headbg.Left + 5 + space * (a - start), headbg.Bottom - 10);
                    }
                }

                RectangleF rect = new RectangleF(headbg.Width / 2 - (fontsize * 2.4f) / 2, 0, (fontsize * 2.4f), headbg.Height);

                //DrawRectangle(whitePen, rect);

                FillRectangle(new SolidBrush(Color.FromArgb(220,255,255,255)), rect);

                if (Math.Abs(_heading - _targetheading) < 4)
                {
                    drawstring(graphicsObject, (heading % 360).ToString("0").PadLeft(3), font, fontsize, whiteBrush, headbg.Width / 2 - (fontsize * 1f), headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                }
                else
                {
                    drawstring(graphicsObject, (heading % 360).ToString("0").PadLeft(3), font, fontsize, whiteBrush, headbg.Width / 2 - (fontsize * 1f), headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                }

                //                Console.WriteLine("HUD 0 " + (DateTime.Now - starttime).TotalMilliseconds + " " + DateTime.Now.Millisecond);

                // xtrack error
                // center

                float xtspace = this.Width / 10.0f / 3.0f;
                int pad = 10;

                float myxtrack_error = _xtrack_error;

                myxtrack_error = Math.Min(myxtrack_error, 40);
                myxtrack_error = Math.Max(myxtrack_error, -40);

                //  xtrack - distance scale - space
                float loc = myxtrack_error / 20.0f * xtspace;

                // current xtrack
                if (Math.Abs(myxtrack_error) == 40)
                {
                    greenPen.Color = Color.FromArgb(128, greenPen.Color);
                }

                graphicsObject.DrawLine(greenPen, this.Width / 10 + loc, headbg.Bottom + 5, this.Width / 10 + loc, headbg.Bottom + this.Height / 10);

                greenPen.Color = Color.FromArgb(255, greenPen.Color);

                graphicsObject.DrawLine(whitePen, this.Width / 10, headbg.Bottom + 5, this.Width / 10, headbg.Bottom + this.Height / 10);

                graphicsObject.DrawLine(whitePen, this.Width / 10 - xtspace, headbg.Bottom + 5 + pad, this.Width / 10 - xtspace, headbg.Bottom + this.Height / 10 - pad);

                graphicsObject.DrawLine(whitePen, this.Width / 10 - xtspace * 2, headbg.Bottom + 5 + pad, this.Width / 10 - xtspace * 2, headbg.Bottom + this.Height / 10 - pad);

                graphicsObject.DrawLine(whitePen, this.Width / 10 + xtspace, headbg.Bottom + 5 + pad, this.Width / 10 + xtspace, headbg.Bottom + this.Height / 10 - pad);

                graphicsObject.DrawLine(whitePen, this.Width / 10 + xtspace * 2, headbg.Bottom + 5 + pad, this.Width / 10 + xtspace * 2, headbg.Bottom + this.Height / 10 - pad);

                // rate of turn

                whitePen.Width = 4;
                graphicsObject.DrawLine(whitePen, this.Width / 10 - xtspace * 2 - xtspace / 2, headbg.Bottom + this.Height / 10 + 10, this.Width / 10 - xtspace * 2 - xtspace / 2 + xtspace, headbg.Bottom + this.Height / 10 + 10);

                graphicsObject.DrawLine(whitePen, this.Width / 10 - xtspace * 0 - xtspace / 2, headbg.Bottom + this.Height / 10 + 10, this.Width / 10 - xtspace * 0 - xtspace / 2 + xtspace, headbg.Bottom + this.Height / 10 + 10);

                graphicsObject.DrawLine(whitePen, this.Width / 10 + xtspace * 2 - xtspace / 2, headbg.Bottom + this.Height / 10 + 10, this.Width / 10 + xtspace * 2 - xtspace / 2 + xtspace, headbg.Bottom + this.Height / 10 + 10);

                float myturnrate = _turnrate;
                float trwidth = (this.Width / 10 + xtspace * 2 - xtspace / 2) - (this.Width / 10 - xtspace * 2 - xtspace / 2);

                float range = 12;

                myturnrate = Math.Min(myturnrate, range / 2);
                myturnrate = Math.Max(myturnrate, (range / 2) * -1.0f);

                loc = myturnrate / range * trwidth;

                greenPen.Width = 4;

                if (Math.Abs(myturnrate) == (range / 2))
                {
                    greenPen.Color = Color.FromArgb(128, greenPen.Color);
                }

                graphicsObject.DrawLine(greenPen, this.Width / 10 + loc - xtspace / 2, headbg.Bottom + this.Height / 10 + 10 + 3, this.Width / 10 + loc + xtspace / 2, headbg.Bottom + this.Height / 10 + 10 + 3);
                graphicsObject.DrawLine(greenPen, this.Width / 10 + loc, headbg.Bottom + this.Height / 10 + 10 + 3, this.Width / 10 + loc, headbg.Bottom + this.Height / 10 + 10 + 10);

                greenPen.Color = Color.FromArgb(255, greenPen.Color);

                whitePen.Width = 2;



                // left scroller

                Rectangle scrollbg = new Rectangle(0, halfheight - halfheight / 2, this.Width / 10, this.Height / 2);

                graphicsObject.DrawRectangle(whitePen, scrollbg);

                graphicsObject.FillRectangle(solidBrush, scrollbg);

                Point[] arrow = new Point[5];

                arrow[0] = new Point(0, -10);
                arrow[1] = new Point(scrollbg.Width - 10, -10);
                arrow[2] = new Point(scrollbg.Width - 5, 0);
                arrow[3] = new Point(scrollbg.Width - 10, 10);
                arrow[4] = new Point(0, 10);

                graphicsObject.TranslateTransform(0, this.Height / 2);

                int viewrange = 26;

                float speed = _airspeed;
                if (speed == 0)
                    speed = _groundspeed;

                space = (scrollbg.Height) / (float)viewrange;
                start = ((int)speed - viewrange / 2);

                if (start > _targetspeed)
                {
                    greenPen.Color = Color.FromArgb(128, greenPen.Color);
                    greenPen.Width = 6;
                    graphicsObject.DrawLine(greenPen, scrollbg.Left, scrollbg.Top, scrollbg.Left + scrollbg.Width, scrollbg.Top);
                    greenPen.Color = Color.FromArgb(255, greenPen.Color);
                }
                if ((speed + viewrange / 2) < _targetspeed)
                {
                    greenPen.Color = Color.FromArgb(128, greenPen.Color);
                    greenPen.Width = 6;
                    graphicsObject.DrawLine(greenPen, scrollbg.Left, scrollbg.Top - space * viewrange, scrollbg.Left + scrollbg.Width, scrollbg.Top - space * viewrange);
                    greenPen.Color = Color.FromArgb(255, greenPen.Color);
                }

                for (int a = (int)start; a <= (speed + viewrange / 2); a += 1)
                {
                    if (a == (int)_targetspeed && _targetspeed != 0)
                    {
                        greenPen.Width = 6;
                        graphicsObject.DrawLine(greenPen, scrollbg.Left, scrollbg.Top - space * (a - start), scrollbg.Left + scrollbg.Width, scrollbg.Top - space * (a - start));
                    }
                    if (a % 5 == 0)
                    {
                        //Console.WriteLine(a + " " + scrollbg.Right + " " + (scrollbg.Top - space * (a - start)) + " " + (scrollbg.Right - 20) + " " + (scrollbg.Top - space * (a - start)));
                        graphicsObject.DrawLine(whitePen, scrollbg.Right, scrollbg.Top - space * (a - start), scrollbg.Right - 10, scrollbg.Top - space * (a - start));
                        drawstring(graphicsObject, a.ToString().PadLeft(5), font, fontsize, whiteBrush, scrollbg.Right - 50 - 4 * fontoffset, scrollbg.Top - space * (a - start) - 6 - fontoffset);
                    }
                }

                graphicsObject.DrawPolygon(blackPen, arrow);
                graphicsObject.FillPolygon(Brushes.Black, arrow);
                drawstring(graphicsObject, ((int)speed).ToString("0"), font, 10, (SolidBrush)Brushes.AliceBlue, 0, -9);

                graphicsObject.ResetTransform();

                // extra text data

                if (_lowairspeed)
                {
                    drawstring(graphicsObject, HUDT.AS + _airspeed.ToString("0.0"), font, fontsize, (SolidBrush)Brushes.Red, 1, scrollbg.Bottom + 5);
                }
                else
                {
                    drawstring(graphicsObject, HUDT.AS + _airspeed.ToString("0.0"), font, fontsize, whiteBrush, 1, scrollbg.Bottom + 5);
                }

                if (_lowgroundspeed)
                {
                    drawstring(graphicsObject, HUDT.GS + _groundspeed.ToString("0.0"), font, fontsize, (SolidBrush)Brushes.Red, 1, scrollbg.Bottom + fontsize + 2 + 10);
                }
                else
                {
                    drawstring(graphicsObject, HUDT.GS + _groundspeed.ToString("0.0"), font, fontsize, whiteBrush, 1, scrollbg.Bottom + fontsize + 2 + 10);
                }

                //drawstring(e,, new Font("Arial", fontsize + 2), whiteBrush, 1, scrollbg.Bottom + fontsize + 2 + 10);

                // right scroller

                scrollbg = new Rectangle(this.Width - this.Width / 10, halfheight - halfheight / 2, this.Width / 10, this.Height / 2);

                graphicsObject.DrawRectangle(whitePen, scrollbg);

                graphicsObject.FillRectangle(solidBrush, scrollbg);

                arrow = new Point[5];

                arrow[0] = new Point(0, -10);
                arrow[1] = new Point(scrollbg.Width - 10, -10);
                arrow[2] = new Point(scrollbg.Width - 5, 0);
                arrow[3] = new Point(scrollbg.Width - 10, 10);
                arrow[4] = new Point(0, 10);



                graphicsObject.TranslateTransform(0, this.Height / 2);




                viewrange = 26;

                space = (scrollbg.Height) / (float)viewrange;
                start = ((int)_alt - viewrange / 2);

                if (start > _targetalt)
                {
                    greenPen.Color = Color.FromArgb(128, greenPen.Color);
                    greenPen.Width = 6;
                    graphicsObject.DrawLine(greenPen, scrollbg.Left, scrollbg.Top, scrollbg.Left + scrollbg.Width, scrollbg.Top);
                    greenPen.Color = Color.FromArgb(255, greenPen.Color);
                }
                if ((_alt + viewrange / 2) < _targetalt)
                {
                    greenPen.Color = Color.FromArgb(128, greenPen.Color);
                    greenPen.Width = 6;
                    graphicsObject.DrawLine(greenPen, scrollbg.Left, scrollbg.Top - space * viewrange, scrollbg.Left + scrollbg.Width, scrollbg.Top - space * viewrange);
                    greenPen.Color = Color.FromArgb(255, greenPen.Color);
                }

                bool ground = false;

                for (int a = (int)start; a <= (_alt + viewrange / 2); a += 1)
                {
                    if (a == Math.Round(_targetalt) && _targetalt != 0)
                    {
                        greenPen.Width = 6;
                        graphicsObject.DrawLine(greenPen, scrollbg.Left, scrollbg.Top - space * (a - start), scrollbg.Left + scrollbg.Width, scrollbg.Top - space * (a - start));
                    }


                    // ground doesnt appear if we are not in view or below ground level
                    if (a == Math.Round(groundalt) && groundalt != 0 && ground == false)
                    {
                        graphicsObject.FillRectangle(new SolidBrush(Color.FromArgb(100,Color.BurlyWood)), new RectangleF(scrollbg.Left, scrollbg.Top - space * (a - start), scrollbg.Width, (space * (a - start))));
                    }

                    if (a % 5 == 0)
                    {
                        //Console.WriteLine(a + " " + scrollbg.Left + " " + (scrollbg.Top - space * (a - start)) + " " + (scrollbg.Left + 20) + " " + (scrollbg.Top - space * (a - start)));
                        graphicsObject.DrawLine(whitePen, scrollbg.Left, scrollbg.Top - space * (a - start), scrollbg.Left + 10, scrollbg.Top - space * (a - start));
                        drawstring(graphicsObject, a.ToString().PadLeft(5), font, fontsize, whiteBrush, scrollbg.Left + 0 + (int)(0 * fontoffset), scrollbg.Top - space * (a - start) - 6 - fontoffset);
                    }

                }

                greenPen.Width = 4;

                // vsi

                graphicsObject.ResetTransform();

                PointF[] poly = new PointF[4];

                poly[0] = new PointF(scrollbg.Left, scrollbg.Top);
                poly[1] = new PointF(scrollbg.Left - scrollbg.Width / 4, scrollbg.Top + scrollbg.Width / 4);
                poly[2] = new PointF(scrollbg.Left - scrollbg.Width / 4, scrollbg.Bottom - scrollbg.Width / 4);
                poly[3] = new PointF(scrollbg.Left, scrollbg.Bottom);

                //verticalspeed

                viewrange = 12;

                _verticalspeed = Math.Min(viewrange / 2, _verticalspeed);
                _verticalspeed = Math.Max(viewrange / -2, _verticalspeed);

                float scaledvalue = _verticalspeed / -viewrange * (scrollbg.Bottom - scrollbg.Top);

                float linespace = (float)1 / -viewrange * (scrollbg.Bottom - scrollbg.Top);

                PointF[] polyn = new PointF[4];

                polyn[0] = new PointF(scrollbg.Left, scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2);
                polyn[1] = new PointF(scrollbg.Left - scrollbg.Width / 4, scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2);
                polyn[2] = polyn[1];
                float peak = 0;
                if (scaledvalue > 0)
                {
                    peak = -scrollbg.Width / 4;
                    if (scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2 + scaledvalue + peak < scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2)
                        peak = -scaledvalue;
                }
                else if (scaledvalue < 0)
                {
                    peak = +scrollbg.Width / 4;
                    if (scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2 + scaledvalue + peak > scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2)
                        peak = -scaledvalue;
                }
                
                polyn[2] = new PointF(scrollbg.Left - scrollbg.Width / 4, scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2 + scaledvalue + peak);
                polyn[3] = new PointF(scrollbg.Left, scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2 + scaledvalue);

                //graphicsObject.DrawPolygon(redPen, poly);
                graphicsObject.FillPolygon(Brushes.Blue, polyn);

                // draw outsidebox
                graphicsObject.DrawPolygon(whitePen, poly);

                for (int a = 1; a < viewrange; a++)
                {
                    graphicsObject.DrawLine(whitePen, scrollbg.Left - scrollbg.Width / 4, scrollbg.Top - linespace * a, scrollbg.Left - scrollbg.Width / 8, scrollbg.Top - linespace * a);
                }

                // draw arrow and text
                
                graphicsObject.ResetTransform();
                graphicsObject.TranslateTransform(this.Width, this.Height / 2);
                graphicsObject.RotateTransform(180);

                graphicsObject.DrawPolygon(blackPen, arrow);
                graphicsObject.FillPolygon(Brushes.Black, arrow);
                graphicsObject.ResetTransform();
                graphicsObject.TranslateTransform(0, this.Height / 2);

                drawstring(graphicsObject, ((int)_alt).ToString("0"), font, 10, (SolidBrush)Brushes.AliceBlue, scrollbg.Left + 10, -9);
                graphicsObject.ResetTransform();

                // mode and wp dist and wp

                drawstring(graphicsObject, _mode, font, fontsize, whiteBrush, scrollbg.Left - 30, scrollbg.Bottom + 5);
                drawstring(graphicsObject, (int)_disttowp + ">" + _wpno, font, fontsize, whiteBrush, scrollbg.Left - 30, scrollbg.Bottom + fontsize + 2 + 10);

                graphicsObject.DrawLine(greenPen, scrollbg.Left - 5, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20, scrollbg.Left - 5, scrollbg.Top - (int)(fontsize) - 2 - 20);
                graphicsObject.DrawLine(greenPen, scrollbg.Left - 10, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 15, scrollbg.Left - 10, scrollbg.Top - (int)(fontsize) - 2 - 20);
                graphicsObject.DrawLine(greenPen, scrollbg.Left - 15, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 10, scrollbg.Left - 15, scrollbg.Top - (int)(fontsize ) - 2 - 20);

                drawstring(graphicsObject, _linkqualitygcs.ToString("0") + "%", font, fontsize, whiteBrush, scrollbg.Left, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20);
                if (_linkqualitygcs == 0)
                {
                    graphicsObject.DrawLine(redPen, scrollbg.Left, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20, scrollbg.Left + 50, scrollbg.Top - (int)(fontsize * 2.2) - 2);

                    graphicsObject.DrawLine(redPen, scrollbg.Left, scrollbg.Top - (int)(fontsize * 2.2) - 2, scrollbg.Left + 50, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20);
                }
                drawstring(graphicsObject, _datetime.ToString("HH:mm:ss"), font, fontsize, whiteBrush, scrollbg.Left - 30, scrollbg.Top - fontsize - 2 - 20);


                // battery
                if (batteryon)
                {
                    graphicsObject.ResetTransform();

                    string text = HUDT.Bat + _batterylevel.ToString("0.00v") + " " + _current.ToString("0.0 A");

                    text = HUDT.Bat + _batterylevel.ToString("0.00v") + " " + _current.ToString("0.0 A") + " " + (_batteryremaining) + "%";

                    if (lowvoltagealert)
                    {
                        drawstring(graphicsObject, text, font, fontsize + 2, (SolidBrush)Brushes.Red, fontsize, this.Height - 30 - fontoffset);
                    }
                    else
                    {
                        drawstring(graphicsObject, text, font, fontsize + 2, whiteBrush, fontsize, this.Height - 30 - fontoffset);
                    }
                }
                // gps

                string gps = "";
                SolidBrush col = whiteBrush;

                if (_gpsfix == 0)
                {
                    gps = (HUDT.GPS0);
                    col = (SolidBrush)Brushes.Red;
                }
                else if (_gpsfix == 1)
                {
                    gps = (HUDT.GPS1);
                    col = (SolidBrush)Brushes.Red;
                }
                else if (_gpsfix == 2)
                {
                    gps = (HUDT.GPS2);
                }
                else if (_gpsfix == 3)
                {
                    gps = (HUDT.GPS3);
                }
                else if (_gpsfix == 4)
                {
                    gps = (HUDT.GPS4);
                }
                else if (_gpsfix == 5)
                {
                    gps = (HUDT.GPS5);
                }
                drawstring(graphicsObject, gps, font, fontsize + 2, col, this.Width - 13 * fontsize, this.Height - 30 - fontoffset);


                if (isNaN)
                    drawstring(graphicsObject, "NaN Error " + DateTime.Now, font, this.Height / 30 + 10, (SolidBrush)Brushes.Red, 50, 50);

                // custom user items
                graphicsObject.ResetTransform();
                int height = this.Height - 30 - fontoffset - fontsize - 8;
                foreach (string key in CustomItems.Keys)
                {
                    try
                    {
                        Custom item = (Custom)CustomItems[key];
                        if (item.Item == null)
                            continue;
                        if (item.Item.Name.Contains("lat") || item.Item.Name.Contains("lng")) 
                        {
                            drawstring(graphicsObject, item.Header + item.GetValue.ToString("0.#######"), font, fontsize + 2, whiteBrush, this.Width / 8, height);
                        }
                        else if (item.Item.Name == "battery_usedmah")
                        {
                            drawstring(graphicsObject, item.Header + item.GetValue.ToString("0"), font, fontsize + 2, whiteBrush, this.Width / 8, height);
                        }
                        else if (item.Item.Name == "timeInAir")
                        {
                            double stime = item.GetValue;
                            int hrs = (int)(stime / (60 * 60));
                            //stime -= hrs * 60 * 60;
                            int mins = (int)(stime / (60)) % 60;
                            //stime = mins * 60;
                            int secs = (int)(stime % 60);
                            drawstring(graphicsObject, item.Header + hrs.ToString("00") + ":" + mins.ToString("00") + ":" + secs.ToString("00"), font, fontsize + 2, whiteBrush, this.Width / 8, height);
                        }
                        else
                        {
                            drawstring(graphicsObject, item.Header + item.GetValue.ToString("0.##"), font, fontsize + 2, whiteBrush, this.Width / 8, height);
                        }
                        height -= fontsize+5;
                    }
                    catch { }

                }




                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                // draw armed

                if (status != statuslast)
                {
                    armedtimer = DateTime.Now;
                }

                if (status == false) // not armed
                {
                    //if ((armedtimer.AddSeconds(8) > DateTime.Now))
                    {
                        drawstring(graphicsObject, HUDT.DISARMED, font, fontsize + 10, (SolidBrush)Brushes.Red, -85, halfheight / -3);
                        statuslast = status;
                    }
                }
                else if (status == true) // armed
                {
                    if ((armedtimer.AddSeconds(8) > DateTime.Now))
                    {
                        drawstring(graphicsObject, HUDT.ARMED, font, fontsize + 20, (SolidBrush)Brushes.Red, -70, halfheight / -3);
                        statuslast = status;
                    }
                }

                if (failsafe == true)
                {
                    drawstring(graphicsObject, HUDT.FAILSAFE, font, fontsize + 20, (SolidBrush)Brushes.Red, -85, halfheight / -5);
                    statuslast = status;
                }

                if (message != "" && messagetime.AddSeconds(15) > DateTime.Now)
                {
                    drawstring(graphicsObject, message, font, fontsize + 10, (SolidBrush)Brushes.Red, -halfwidth + 50, halfheight / 3);
                }



                graphicsObject.ResetTransform();



                

                if (!opengl)
                {
                    e.Graphics.DrawImageUnscaled(objBitmap, 0, 0);
                }

                if (DesignMode)
                {
                    return;
                }

                //                Console.WriteLine("HUD 1 " + (DateTime.Now - starttime).TotalMilliseconds + " " + DateTime.Now.Millisecond);

                lock (streamlock)
                {
                    if (streamjpgenable || streamjpg == null) // init image and only update when needed
                    {
                        if (opengl)
                        {
                            objBitmap = GrabScreenshot();
                        }

                        streamjpg = new MemoryStream();
                        objBitmap.Save(streamjpg, ici, eps);
                        //objBitmap.Save(streamjpg,ImageFormat.Bmp);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info("hud error "+ex.ToString());
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        static ImageCodecInfo GetImageCodec(string mimetype)
        {
            foreach (ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
            {
                if (ici.MimeType == mimetype) return ici;
            }
            return null;
        }

        // Returns a System.Drawing.Bitmap with the contents of the current framebuffer
        public new Bitmap GrabScreenshot()
        {
            if (OpenTK.Graphics.GraphicsContext.CurrentContext == null)
                throw new OpenTK.Graphics.GraphicsContextMissingException();

            Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(this.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height,OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }


        float wrap360(float noin)
        {
            if (noin < 0)
                return noin + 360;
            return noin;
        }

        /// <summary>
        /// pen for drawstring
        /// </summary>
        Pen P = new Pen(Color.FromArgb(0x26, 0x27, 0x28), 2f);
        /// <summary>
        /// pth for drawstring
        /// </summary>
        GraphicsPath pth = new GraphicsPath();

        void drawstring(HUD e, string text, Font font, float fontsize, SolidBrush brush, float x, float y)
        {
            if (!opengl)
            {
                drawstring(graphicsObjectGDIP, text, font, fontsize, brush, x, y);
                return;
            }

            if (text == null || text == "")
                return;
            /*
            OpenTK.Graphics.Begin(); 
            GL.PushMatrix(); 
            GL.Translate(x, y, 0);
            printer.Print(text, font, c); 
            GL.PopMatrix(); printer.End();
            */

            char[] chars = text.ToCharArray();

            float maxy = 1;

            foreach (char cha in chars)
            {
                int charno = (int)cha;

                int charid = charno ^ (int)(fontsize * 1000) ^ brush.Color.ToArgb();

                if (!charDict.ContainsKey(charid))
                {
                    charDict[charid] = new character() { bitmap = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb) , size = (int)fontsize };

                    charDict[charid].bitmap.MakeTransparent(Color.Transparent);

                    //charbitmaptexid

                    float maxx = this.Width / 150; // for space


                    // create bitmap
                    using (Graphics gfx = Graphics.FromImage(charDict[charid].bitmap))
                    {
                        pth.Reset();

                        if (text != null)
                            pth.AddString(cha + "", font.FontFamily, 0, fontsize + 5, new Point((int)0, (int)0), StringFormat.GenericTypographic);

                        gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        gfx.DrawPath(P, pth);

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
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvModeCombine.Replace);//Important, or wrong color on some computers

                    Bitmap bitmap = charDict[charid].bitmap;
                    GL.GenTextures(1, out textureId);
                    GL.BindTexture(TextureTarget.Texture2D, textureId);

                    BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                    //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                    GL.Finish();
                    bitmap.UnlockBits(data);

                    charDict[charid].gltextureid = textureId;
                }

                //GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, charDict[charid].gltextureid);

                float scale = 1.0f;

                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 0); GL.Vertex2(x, y);
                GL.TexCoord2(1, 0); GL.Vertex2(x + charDict[charid].bitmap.Width * scale, y);
                GL.TexCoord2(1, 1); GL.Vertex2(x + charDict[charid].bitmap.Width * scale, y + charDict[charid].bitmap.Height * scale);
                GL.TexCoord2(0, 1); GL.Vertex2(x + 0, y + charDict[charid].bitmap.Height * scale);
                GL.End();

                //GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.Texture2D);

                x += charDict[charid].width * scale;
            }
        }
		
		void drawstring(Graphics e, string text, Font font, float fontsize, SolidBrush brush, float x, float y)
        {
            if (text == null || text == "")
                return;

                       
            char[] chars = text.ToCharArray();

            float maxy = 0;

            foreach (char cha in chars)
            {
                int charno = (int)cha;

                int charid = charno ^ (int)(fontsize * 1000) ^ brush.Color.ToArgb();

                if (!charDict.ContainsKey(charid))
                {
                    charDict[charid] = new character() { bitmap = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb), size = (int)fontsize };

                    charDict[charid].bitmap.MakeTransparent(Color.Transparent);

                    //charbitmaptexid

                    float maxx = this.Width / 150; // for space


                    // create bitmap
                    using (Graphics gfx = Graphics.FromImage(charDict[charid].bitmap))
                    {
                        pth.Reset();

                        if (text != null)
                            pth.AddString(cha + "", font.FontFamily, 0, fontsize + 5, new Point((int)0, (int)0), StringFormat.GenericTypographic);

                        gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        gfx.DrawPath(P, pth);

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
                }

                // draw it

                float scale = 1.0f;

                DrawImage(charDict[charid].bitmap, (int)x, (int)y, charDict[charid].bitmap.Width, charDict[charid].bitmap.Height);

                x += charDict[charid].width * scale;
            }

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            try
            {
                if (opengl)
                {
                    base.OnHandleCreated(e);
                }
            }
            catch (Exception ex) { log.Error(ex); opengl = false; } // macs fail here
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            try
            {
                if (opengl)
                {
                    base.OnHandleDestroyed(e);
                }
            }
            catch (Exception ex) { log.Info(ex.ToString()); opengl = false; }
        }

        public void doResize()
        {
            OnResize(new EventArgs());
        }

        protected override void OnResize(EventArgs e)
        {
            if (DesignMode || !IsHandleCreated || !started)
                return;

            base.OnResize(e);

            if (SixteenXNine)
            {
                int ht = (int)(this.Width / 1.777f);
                if (ht >= this.Height + 5 || ht <= this.Height - 5)
                {
                    this.Height = ht;
                    return;
                }
            }
            else
            {
                // 4x3
                int ht = (int)(this.Width / 1.333f);
                if (ht >= this.Height + 5 || ht <= this.Height - 5)
                {
                    this.Height = ht;
                    return;
                }
            }

            graphicsObjectGDIP = Graphics.FromImage(objBitmap);

            try
            {
                foreach (character texid in charDict.Values)
                {
                    try
                    {
                        texid.bitmap.Dispose();
                    }
                    catch { }
                }
                charDict.Clear();

                if (opengl)
                {
                    foreach (character texid in charDict.Values)
                    {
                        if (texid.gltextureid != 0)
                            GL.DeleteTexture(texid.gltextureid);
                    }
                }
            }
            catch { }

           // GC.Collect();
            
            try
            {
                if (opengl)
                {
                    MakeCurrent();

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(0, Width, Height, 0, -1, 1);
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadIdentity();

                    GL.Viewport(0, 0, Width, Height);
                }
            }
            catch { }

            Refresh();
        }

        public bool UseOpenGL { get; set; }
    }
}