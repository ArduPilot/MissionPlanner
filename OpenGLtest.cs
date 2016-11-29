using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using GMap.NET.MapProviders;
using GMap.NET.Projections;

namespace MissionPlanner.Controls
{
    public class OpenGLtest : GLControl
    {
        public static OpenGLtest instance;

        // terrain image
        Bitmap _terrain = new Bitmap(640, 480);
        int texture = 0;

        GMap.NET.Internals.Core core = new GMap.NET.Internals.Core();

        float _angle = 0;
        double cameraX, cameraY, cameraZ; // camera coordinates
        double lookX, lookY, lookZ; // camera look-at coordinates

        double step = 1/1200.0;

        // image zoom level
        int zoom = 14;

        RectLatLng area = new RectLatLng(-35.04286, 117.84262, 0.1, 0.1);

        double _alt = 0;

        public PointLatLngAlt LocationCenter
        {
            get { return new PointLatLngAlt(area.LocationMiddle.Lat, area.LocationMiddle.Lng, _alt, ""); }
            set
            {
                if (area.LocationMiddle.Lat == value.Lat && area.LocationMiddle.Lng == value.Lng)
                    return;

                if (value.Lat == 0 && value.Lng == 0)
                    return;

                _alt = value.Alt;
                double size = 0.01;
                area = new RectLatLng(value.Lat + size, value.Lng - size, size*2, size*2);
                // Console.WriteLine(area.LocationMiddle + " " + value.ToString());
                this.Invalidate();
            }
        }

        public Vector3 rpy = new Vector3();

        public OpenGLtest()
        {
            instance = this;

            InitializeComponent();

            core.OnMapOpen();
        }

        void getImage()
        {
            GMapProvider type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            PureProjection prj = type.Projection;

            //GMap.NET.GMaps.Instance.GetImageFrom();

            DateTime startimage = DateTime.Now;

            if (!area.IsEmpty)
            {
                try
                {
                    //string bigImage = zoom + "-" + type + "-vilnius.png";

                    //Console.WriteLine("Preparing: " + bigImage);
                    //Console.WriteLine("Zoom: " + zoom);
                    //Console.WriteLine("Type: " + type.ToString());
                    //Console.WriteLine("Area: " + area);

                    var types = type; // GMaps.Instance.GetAllLayersOfType(type);

                    // max zoom level
                    zoom = 20;

                    GPoint topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
                    GPoint rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
                    GPoint pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);

                    // zoom based on pixel density
                    while (pxDelta.X > 2000)
                    {
                        zoom--;

                        // current area
                        topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
                        rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
                        pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);
                    }

                    // get tiles - bg
                    core.Provider = type;
                    core.Position = LocationCenter;
                    core.Zoom = zoom;

                    // get type list at new zoom level
                    List<GPoint> tileArea = prj.GetAreaTileList(area, zoom, 0);

                    //this.Invalidate();

                    Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);

                    int padding = 0;
                    {
                        using (
                            Bitmap bmpDestination = new Bitmap((int) pxDelta.X + padding*2, (int) pxDelta.Y + padding*2)
                            )
                        {
                            Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                            using (Graphics gfx = Graphics.FromImage(bmpDestination))
                            {
                                Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                                gfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                                // get tiles & combine into one
                                foreach (var p in tileArea)
                                {
                                    Console.WriteLine("Downloading[" + p + "]: " + tileArea.IndexOf(p) + " of " +
                                                      tileArea.Count);

                                    foreach (var tp in type.Overlays)
                                    {
                                        Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                                        GMapImage tile =
                                            ((PureImageCache) Maps.MyImageCache.Instance).GetImageFromCache(type.DbId, p,
                                                zoom) as GMapImage;

                                        //GMapImage tile = GMaps.Instance.GetImageFrom(tp, p, zoom, out ex) as GMapImage;
                                        //GMapImage tile = type.GetTileImage(p, zoom) as GMapImage;
                                        //tile.Img.Save(zoom + "-" + p.X + "-" + p.Y + ".bmp");

                                        if (tile != null)
                                        {
                                            using (tile)
                                            {
                                                long x = p.X*prj.TileSize.Width - topLeftPx.X + padding;
                                                long y = p.Y*prj.TileSize.Width - topLeftPx.Y + padding;
                                                {
                                                    Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                                                    gfx.DrawImage(tile.Img, x, y, prj.TileSize.Width,
                                                        prj.TileSize.Height);
                                                    Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                                                }
                                            }
                                        }
                                        else
                                        {
                                        }
                                    }
                                }
                            }

                            Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                            _terrain = new Bitmap(bmpDestination, 1024*2, 1024*2);

                            // _terrain.Save(zoom +"-map.bmp");


                            GL.BindTexture(TextureTarget.Texture2D, texture);

                            BitmapData data =
                                _terrain.LockBits(new System.Drawing.Rectangle(0, 0, _terrain.Width, _terrain.Height),
                                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                            //Console.WriteLine("w {0} h {1}",data.Width, data.Height);

                            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height,
                                0,
                                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                            _terrain.UnlockBits(data);

                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                                (int) TextureMinFilter.Linear);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                                (int) TextureMagFilter.Linear);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        public Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
        {
            var dir = Vector3.Cross(b - a, c - a);
            var norm = Vector3.Normalize(dir);
            return norm;
        }


        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (this.DesignMode)
                return;

            if (area.LocationMiddle.Lat == 0 && area.LocationMiddle.Lng == 0)
                return;

            _angle += 1f;

            // area.LocationTopLeft = new PointLatLng(area.LocationTopLeft.Lat + 0.0001,area.LocationTopLeft.Lng);

            //area.Size = new SizeLatLng(0.1, 0.1);

            try
            {
                base.OnPaint(e);
            }
            catch
            {
                return;
            }

            double heightscale = (step/90.0)*1.3;

            float radians = (float) (Math.PI*(rpy.Z*-1)/180.0f);

            //radians = 0;

            float mouseY = (float) (0.1);

            cameraX = area.LocationMiddle.Lng; // multiplying by mouseY makes the
            cameraZ = area.LocationMiddle.Lat; // camera get closer/farther away with mouseY
            cameraY = (LocationCenter.Alt < srtm.getAltitude(cameraZ, cameraX, 20).alt)
                ? (srtm.getAltitude(cameraZ, cameraX, 20).alt + 0.2)*heightscale
                : LocationCenter.Alt*heightscale; // (srtm.getAltitude(lookZ, lookX, 20) + 100) * heighscale;


            lookX = area.LocationMiddle.Lng + Math.Sin(radians)*mouseY;
            ;
            lookY = cameraY;
            lookZ = area.LocationMiddle.Lat + Math.Cos(radians)*mouseY;
            ;


            MakeCurrent();


            GL.MatrixMode(MatrixMode.Projection);

            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView(100*deg2rad, 1f, 0.00001f,
                (float) step*50);
            GL.LoadMatrix(ref projection);

            Matrix4 modelview = Matrix4.LookAt((float) cameraX, (float) cameraY, (float) cameraZ, (float) lookX,
                (float) lookY, (float) lookZ, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);

            // roll
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationZ(rpy.X*deg2rad));
            // pitch
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationX(rpy.Y*-deg2rad));

            GL.LoadMatrix(ref modelview);

            GL.ClearColor(Color.LightBlue);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.LightModel(LightModelParameter.LightModelAmbient, new float[] {1f, 1f, 1f, 1f});

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            /*
            GL.Begin(BeginMode.LineStrip);

            GL.Color3(Color.White);
            GL.Vertex3(0, 0, 0);

            GL.Vertex3(area.Bottom, 0, area.Left);

            GL.Vertex3(lookX, lookY, lookZ);

            //GL.Vertex3(cameraX, cameraY, cameraZ);

            GL.End();
            */
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Start();

            //zoom = 14;

            getImage();

            sw.Stop();

            Console.WriteLine("img " + sw.ElapsedMilliseconds);

            sw.Start();

            double increment = step*1;

            double cleanup = area.Bottom%increment;
            double cleanup2 = area.Left%increment;

            for (double z = (area.Bottom - cleanup); z < area.Top - step; z += increment)
            {
                //Makes OpenGL draw a triangle at every three consecutive vertices
                GL.Begin(PrimitiveType.TriangleStrip);
                for (double x = (area.Left - cleanup2); x < area.Right - step; x += increment)
                {
                    double heightl = srtm.getAltitude(z, area.Right + area.Left - x, 20).alt;

                    //  Console.WriteLine(x + " " + z);

                    GL.Color3(Color.White);


                    //  int heightl = 0;

                    double scale2 = (Math.Abs(x - area.Left)/area.WidthLng); // / (float)_terrain.Width;

                    double scale3 = (Math.Abs(z - area.Bottom)/area.HeightLat); // / (float)_terrain.Height;

                    double imgx = 1 - scale2;
                    double imgy = 1 - scale3;
                    //GL.Color3(Color.Red);

                    //GL.Color3(_terrain.GetPixel(imgx, imgy));
                    GL.TexCoord2(imgx, imgy);
                    GL.Vertex3(x, heightl*heightscale, z); //  _terrain.GetPixel(x, z).R

                    try
                    {
                        heightl = srtm.getAltitude(z + increment, area.Right + area.Left - x, 20).alt;

                        //scale2 = (Math.Abs(x - area.Left) / area.WidthLng) * (float)_terrain.Width;

                        scale3 = (Math.Abs(((z + increment) - area.Bottom))/area.HeightLat);
                            // / (float)_terrain.Height;

                        imgx = 1 - scale2;
                        imgy = 1 - scale3;
                        // GL.Color3(Color.Green);
                        //GL.Color3(_terrain.GetPixel(imgx, imgy));
                        GL.TexCoord2(imgx, imgy);
                        GL.Vertex3(x, heightl*heightscale, z + increment);

                        //  Console.WriteLine(x + " " + (z + step));
                    }
                    catch
                    {
                        break;
                    }
                }
                GL.End();
            }

            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);

            GL.Flush();

            sw.Stop();

            Console.WriteLine("GL  " + sw.ElapsedMilliseconds);

            try
            {
                this.SwapBuffers();

                Context.MakeCurrent(null);
            }
            catch
            {
            }

            //this.Invalidate();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // OpenGLtest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "OpenGLtest";
            this.Load += new System.EventHandler(this.test_Load);
            this.Resize += new System.EventHandler(this.test_Resize);
            this.ResumeLayout(false);
        }

        private void test_Load(object sender, EventArgs e)
        {
            GL.GenTextures(1, out texture);

            GL.Enable(EnableCap.DepthTest);
            // GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Normalize);

            //GL.Enable(EnableCap.LineSmooth);
            //GL.Enable(EnableCap.PointSmooth);
            //GL.Enable(EnableCap.PolygonSmooth);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);
        }

        private void test_Resize(object sender, EventArgs e)
        {
            MakeCurrent();

            GL.Viewport(0, 0, this.Width, this.Height);

            this.Invalidate();
        }
    }
}