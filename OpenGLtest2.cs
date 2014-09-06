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
    public class OpenGLtest2 : GLControl
    {
        public static OpenGLtest2 instance;

        // terrain image
        Bitmap _terrain = new Bitmap(640,480);
        Dictionary<GPoint, int> textureid = new Dictionary<GPoint, int>();
        
        GMap.NET.Internals.Core core = new GMap.NET.Internals.Core();

        float _angle = 0;
        double cameraX, cameraY, cameraZ;       // camera coordinates
        double lookX, lookY, lookZ;             // camera look-at coordinates

        double step = 1 / 1200.0;

        // image zoom level
        int zoom = 14;

        RectLatLng area = new RectLatLng(-35.04286,117.84262,0.1,0.1);
        PointLatLngAlt center = new PointLatLngAlt();

        public PointLatLngAlt LocationCenter { 
            get {
                return center;
            }
            set {
                if (value.Lat == 0 && value.Lng == 0)
                    return;

                center = value;

                this.Invalidate();
            } 
        }

        public Vector3 rpy = new Vector3();

        public OpenGLtest2()
        {
            instance = this;

            InitializeComponent();

            core.OnMapOpen();
        }
        /*
        void getImage()
        {


            //GMap.NET.GMaps.Instance.GetImageFrom();

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            
            if (!area.IsEmpty)
            {
                try
                {
                    //string bigImage = zoom + "-" + type + "-vilnius.png";

                    //Console.WriteLine("Preparing: " + bigImage);
                    //Console.WriteLine("Zoom: " + zoom);
                    //Console.WriteLine("Type: " + type.ToString());
                    //Console.WriteLine("Area: " + area);

                    var types = type;// GMaps.Instance.GetAllLayersOfType(type);

                    // max zoom level
                    zoom = 17;

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
                    core.OnMapSizeChanged((int)pxDelta.X,(int)pxDelta.Y);

                    // get type list at new zoom level
                    List<GPoint> tileArea = prj.GetAreaTileList(area, zoom, 0);

                    //this.Invalidate();

                    Console.WriteLine("1 " + sw.ElapsedMilliseconds);

                    int padding = 0;
                    {
                        using (Bitmap bmpDestination = new Bitmap((int)pxDelta.X + padding * 2, (int)pxDelta.Y + padding * 2))
                        {
                            Console.WriteLine("2 " + sw.ElapsedMilliseconds);
                            using (Graphics gfx = Graphics.FromImage(bmpDestination))
                            {
                                gfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                                // get tiles & combine into one
                                foreach (var p in tileArea)
                                {
                                   Console.WriteLine("Downloading[" + p + "]: " + tileArea.IndexOf(p) + " of " + tileArea.Count);

                                   foreach (var tp in type.Overlays)
                                    {
                                        Exception ex;

                                        Console.WriteLine("3"+sw.ElapsedMilliseconds);
                                        GMapImage tile = ((PureImageCache)Maps.MyImageCache.Instance).GetImageFromCache(type.DbId, p, zoom) as GMapImage;

                                        //GMapImage tile = GMaps.Instance.GetImageFrom(tp, p, zoom, out ex) as GMapImage;
                                        //GMapImage tile = type.GetTileImage(p, zoom) as GMapImage;
                                        //tile.Img.Save(zoom + "-" + p.X + "-" + p.Y + ".bmp");

                                        if (tile != null)
                                        {
                                            using (tile)
                                            {
                                                long x = p.X * prj.TileSize.Width - topLeftPx.X + padding;
                                                long y = p.Y * prj.TileSize.Width - topLeftPx.Y + padding;
                                                {
                                                    Console.WriteLine("4 " + sw.ElapsedMilliseconds);
                                                    gfx.DrawImage(tile.Img, x, y, prj.TileSize.Width, prj.TileSize.Height);
                                                    Console.WriteLine("5 " + +sw.ElapsedMilliseconds);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            
                                        }
                                    }
                                }
                            }

                            Console.WriteLine("6 " + sw.ElapsedMilliseconds);
                            _terrain.Dispose();
                            _terrain = new Bitmap(bmpDestination, 1024*2, 1024*2);

                           // _terrain.Save(zoom +"-map.bmp");


                            GL.BindTexture(TextureTarget.Texture2D, texture);

                            BitmapData data = _terrain.LockBits(new System.Drawing.Rectangle(0, 0, _terrain.Width, _terrain.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                            //Console.WriteLine("w {0} h {1}",data.Width, data.Height);

                            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                            _terrain.UnlockBits(data);

                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }
        }
        */
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
        {
                var dir = Vector3.Cross(b - a, c - a);
                var norm = Vector3.Normalize(dir);
                return norm;
        }

        void generateTexture(GPoint point, Bitmap image)
        {
            int texture = 0;

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvModeCombine.Replace);//Important, or wrong color on some computers

            GL.GenTextures(1, out texture);

            GL.BindTexture(TextureTarget.Texture2D, texture);

            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.Finish();

          

            image.Dispose();

            textureid[point] = texture;
        }

        DateTime lastrefresh = DateTime.MinValue;

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (this.DesignMode)
                return;

            if (area.LocationMiddle.Lat == 0 && area.LocationMiddle.Lng == 0)
                return;
            
            try
            {
                base.OnPaint(e);

            }
            catch { return;  }

            double heightscale = (step / 90.0) * 3;

            float yawradians = (float)(Math.PI * (rpy.Z * 1) / 180.0f);

             //radians = 0;

            float mouseY = (float)(0.0025);

            cameraX = area.LocationMiddle.Lng;// -Math.Sin(yawradians) * mouseY;     // multiplying by mouseY makes the
            cameraY = area.LocationMiddle.Lat;// -Math.Cos(yawradians) * mouseY;    // camera get closer/farther away with mouseY
            cameraZ = (LocationCenter.Alt < srtm.getAltitude(center.Lat, center.Lng)) ? (srtm.getAltitude(center.Lat, center.Lng) + 1) * heightscale : LocationCenter.Alt * heightscale;// (srtm.getAltitude(lookZ, lookX, 20) + 100) * heighscale;

      lookX = area.LocationMiddle.Lng + Math.Sin(yawradians) * mouseY;
      lookY = area.LocationMiddle.Lat + Math.Cos(yawradians) * mouseY;
      lookZ = cameraZ;

     // cameraZ += 0.04;
            
            GMapProvider type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            PureProjection prj = type.Projection;

          
            PointLatLngAlt leftf = center.newpos(rpy.Z ,500);
            PointLatLngAlt rightf = center.newpos(rpy.Z, 10);
            PointLatLngAlt left = center.newpos(rpy.Z - 90, 500);
            PointLatLngAlt right = center.newpos(rpy.Z + 90, 500);

            double maxlat = Math.Max(left.Lat,Math.Max(right.Lat,Math.Max(leftf.Lat,rightf.Lat)));
            double minlat = Math.Min(left.Lat, Math.Min(right.Lat, Math.Min(leftf.Lat, rightf.Lat)));

            double maxlng = Math.Max(left.Lng, Math.Max(right.Lng, Math.Max(leftf.Lng, rightf.Lng)));
            double minlng = Math.Min(left.Lng, Math.Min(right.Lng, Math.Min(leftf.Lng, rightf.Lng)));

            area = RectLatLng.FromLTRB(minlng, maxlat, maxlng, minlat);

            GPoint topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
            GPoint rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
            GPoint pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);

            zoom = 17;

            // zoom based on pixel density
            while (pxDelta.X > 2000)
            {
                zoom--;

                // current area
                topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
                rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
                pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);

            }

            // update once per seconds - we only read from disk, so need to let cahce settle
            if (lastrefresh.AddSeconds(.6) < DateTime.Now)
            {
                // get tiles - bg
                core.Provider = type;
                core.Position = LocationCenter;
                core.Zoom = zoom;
                core.OnMapSizeChanged(this.Width, this.Height);

                lastrefresh = DateTime.Now;
            }
            else
            {
                //return;
            }

            MakeCurrent();


            GL.MatrixMode(MatrixMode.Projection);

            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView(130 * deg2rad, 1f, 0.00001f, (float)step * 100);
            GL.LoadMatrix(ref projection);

            Matrix4 modelview = Matrix4.LookAt((float)cameraX, (float)cameraY, (float)cameraZ, (float)lookX, (float)lookY, (float)lookZ, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);

            // roll
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationZ(rpy.X * deg2rad));
            // pitch
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationX((rpy.Y - 10) * -deg2rad));

            GL.LoadMatrix(ref modelview);

            GL.ClearColor(Color.LightBlue);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 1f, 1f, 1f, 1f });

            GL.Enable(EnableCap.Fog);
            // GL.Enable(EnableCap.Lighting);
            // GL.Enable(EnableCap.Light0);

            GL.Fog(FogParameter.FogColor, new float[] { 0.5f, 0.5f, 0.5f, 1f });
            //GL.Fog(FogParameter.FogDensity,0.1f);
            GL.Fog(FogParameter.FogMode, (int)FogMode.Linear);
            GL.Fog(FogParameter.FogStart, (float)step * 1);
            GL.Fog(FogParameter.FogEnd, (float)(step * 7));

            /*
            GL.Begin(BeginMode.LineStrip);

            GL.Color3(Color.White);
            GL.Vertex3(0, 0, 0);

            GL.Vertex3(area.Bottom, 0, area.Left);

            GL.Vertex3(lookX, lookY, lookZ);

            //GL.Vertex3(cameraX, cameraY, cameraZ);

            GL.End();
            */

            GL.Begin(BeginMode.LineStrip);
            GL.PointSize(200);
            GL.Color3(Color.Red);
            //GL.Vertex3(new Vector3((float)center.Lng,(float)center.Lat,(float)(center.Alt * heightscale)));
            //GL.Vertex3(new Vector3(0, 0, 0));
            //GL.Vertex3(new Vector3((float)cameraX, (float)cameraY, (float)cameraZ));
            //GL.Color3(Color.Green);
            //GL.Vertex3(new Vector3((float)lookX, (float)lookY, (float)lookZ));

            GL.Vertex3(area.LocationTopLeft.Lng, area.LocationTopLeft.Lat, 0);
            GL.Vertex3(area.LocationTopLeft.Lng, area.LocationRightBottom.Lat, 0);
            GL.Vertex3(area.LocationRightBottom.Lng, area.LocationRightBottom.Lat, 0);
            GL.Vertex3(area.LocationRightBottom.Lng, area.LocationTopLeft.Lat, 0);
            GL.Vertex3(area.LocationTopLeft.Lng, area.LocationTopLeft.Lat, 0);

            GL.End();

            GL.Begin(BeginMode.LineStrip);
            GL.PointSize(200);
            GL.Color3(Color.Red);
            GL.Vertex3(new Vector3((float)center.Lng, (float)center.Lat, 0));
            GL.Vertex3(new Vector3((float)leftf.Lng, (float)leftf.Lat, 0));

            GL.End();

            GL.Begin(BeginMode.Points);
            GL.PointSize(100);
            GL.Color3(Color.Blue);
            GL.Vertex3(new Vector3((float)center.Lng, (float)center.Lat, 0));
            GL.End();

            //textureid.Clear();

            // get type list at new zoom level
            List<GPoint> tileArea = prj.GetAreaTileList(area, zoom, 0);

             // get tiles & combine into one
            foreach (var p in tileArea)
            {
                core.tileDrawingListLock.AcquireReaderLock();
                core.Matrix.EnterReadLock();
                try
                {
                    GMap.NET.Internals.Tile t = core.Matrix.GetTileWithNoLock(core.Zoom, p);

                    if (t.NotEmpty)
                    {
                        foreach (GMapImage img in t.Overlays)
                        {
                            if (!textureid.ContainsKey(p))
                            {
                                generateTexture(p, (Bitmap)img.Img);
                            }
                        }
                    }
                    else
                    {

                    }
                }
                finally
                {
                    core.Matrix.LeaveReadLock();
                    core.tileDrawingListLock.ReleaseReaderLock();
                }

                //GMapImage tile = ((PureImageCache)Maps.MyImageCache.Instance).GetImageFromCache(type.DbId, p, zoom) as GMapImage;

                //if (tile != null && !textureid.ContainsKey(p))
                {
                  //  generateTexture(p, (Bitmap)tile.Img);
                }

                if (textureid.ContainsKey(p))
                {
                    int texture = textureid[p];

                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, texture);

                }
                else
                {
                    GL.Disable(EnableCap.Texture2D);
                }

                long x = p.X * prj.TileSize.Width - topLeftPx.X;
                long y = p.Y * prj.TileSize.Width - topLeftPx.Y;

                long xr = p.X * prj.TileSize.Width;
                long yr = p.Y * prj.TileSize.Width;

                long x2 = (p.X+1) * prj.TileSize.Width;
                long y2 = (p.Y+1) * prj.TileSize.Width;

                // generate terrain
                GL.Begin(PrimitiveType.TriangleStrip);

                double z = 0;

                var latlng = prj.FromPixelToLatLng(xr,yr,zoom);

                double heightl = srtm.getAltitude(latlng.Lat, latlng.Lng);

                GL.Color3(Color.White);
                //xr - topLeftPx.X, yr - topLeftPx.Y
                GL.TexCoord2(0,0);
                GL.Vertex3(latlng.Lng, latlng.Lat, heightl * heightscale);


                // next down
                latlng = prj.FromPixelToLatLng(xr, y2, zoom);

                heightl = srtm.getAltitude(latlng.Lat, latlng.Lng);

                GL.TexCoord2(0,.99);
                GL.Vertex3(latlng.Lng, latlng.Lat, heightl * heightscale);


                // next right
                latlng = prj.FromPixelToLatLng(x2, yr, zoom);

                heightl = srtm.getAltitude(latlng.Lat, latlng.Lng);

                GL.TexCoord2(.99,0);
                GL.Vertex3(latlng.Lng, latlng.Lat, heightl * heightscale);


                // next right down
                latlng = prj.FromPixelToLatLng(x2, y2, zoom);

                heightl = srtm.getAltitude(latlng.Lat, latlng.Lng);

                GL.TexCoord2(.99,.99);
                GL.Vertex3(latlng.Lng, latlng.Lat, heightl * heightscale);

                GL.End();

            }


            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);

            GL.Flush();

            try
            {

                this.SwapBuffers();


                Context.MakeCurrent(null);
            }
            catch { }

            return;


            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Start();

            //zoom = 14;

            //getImage();

            sw.Stop();

            Console.WriteLine("img " +sw.ElapsedMilliseconds);

            sw.Start();

           double increment = step *1;

            double cleanup = area.Bottom % increment;
            double cleanup2 = area.Left % increment;

            cleanup = 0;
            cleanup2 = 0;

            for (double z = (area.Bottom - cleanup); z < area.Top - step; z += increment)
            {
                //Makes OpenGL draw a triangle at every three consecutive vertices
                GL.Begin(PrimitiveType.TriangleStrip);
                for (double x = (area.Left - cleanup2); x < area.Right - step; x += increment)
                {
                    double heightl = srtm.getAltitude(z, area.Right + area.Left - x);

                  //  Console.WriteLine(x + " " + z);

                    GL.Color3(Color.White);


                  //  int heightl = 0;

                    double scale2 = (Math.Abs(x - area.Left) / area.WidthLng);// / (float)_terrain.Width;

                    double scale3 = (Math.Abs(z - area.Bottom) / area.HeightLat);// / (float)_terrain.Height;

                    double imgx = 1 - scale2;
                    double imgy = 1 - scale3;
                    //GL.Color3(Color.Red);

                    //GL.Color3(_terrain.GetPixel(imgx, imgy));
                    GL.TexCoord2(imgx,imgy);
                    GL.Vertex3(x, heightl * heightscale, z); //  _terrain.GetPixel(x, z).R

                    try
                    {
                        heightl = srtm.getAltitude(z + increment, area.Right + area.Left - x);

                        //scale2 = (Math.Abs(x - area.Left) / area.WidthLng) * (float)_terrain.Width;

                        scale3 = (Math.Abs(((z + increment) - area.Bottom)) / area.HeightLat);// / (float)_terrain.Height;

                        imgx = 1- scale2;
                        imgy = 1 - scale3;
                       // GL.Color3(Color.Green);
                       //GL.Color3(_terrain.GetPixel(imgx, imgy));
                        GL.TexCoord2(imgx,imgy);
                        GL.Vertex3(x, heightl * heightscale, z + increment);

                      //  Console.WriteLine(x + " " + (z + step));
                    }
                    catch { break; }
                    
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

            Console.WriteLine("GL  "+sw.ElapsedMilliseconds);

            try
            {

            this.SwapBuffers();

          
                Context.MakeCurrent(null);
            }
            catch { }

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
