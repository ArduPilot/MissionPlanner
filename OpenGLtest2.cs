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
        Bitmap _terrain = new Bitmap(640, 480);
        Dictionary<GPoint, int> textureid = new Dictionary<GPoint, int>();

        GMap.NET.Internals.Core core = new GMap.NET.Internals.Core();

        double cameraX, cameraY, cameraZ; // camera coordinates
        double lookX, lookY, lookZ; // camera look-at coordinates

        double step = 1/1200.0;

        // image zoom level
        int zoom = 14;

        RectLatLng area = new RectLatLng(-35.04286, 117.84262, 0.1, 0.1);
        PointLatLngAlt center = new PointLatLngAlt();

        public PointLatLngAlt LocationCenter
        {
            get { return center; }
            set
            {
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
        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        public Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
        {
            var dir = Vector3.Cross(b - a, c - a);
            var norm = Vector3.Normalize(dir);
            return norm;
        }

        void generateTexture(GPoint point, Bitmap image)
        {
            int texture = 0;

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode,
                (float) TextureEnvModeCombine.Replace); //Important, or wrong color on some computers

            GL.GenTextures(1, out texture);

            GL.BindTexture(TextureTarget.Texture2D, texture);

            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);

            GL.Finish();

            GL.End();

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
            catch
            {
                return;
            }

            double heightscale = (step/90.0)*1;

            float yawradians = (float) (Math.PI*(rpy.Z*1)/180.0f);

            //radians = 0;

            float mouseY = (float) step/10f;

            cameraX = center.Lng; // -Math.Sin(yawradians) * mouseY;     // multiplying by mouseY makes the
            cameraY = center.Lat; // -Math.Cos(yawradians) * mouseY;    // camera get closer/farther away with mouseY
            cameraZ = (center.Alt < srtm.getAltitude(center.Lat, center.Lng).alt)
                ? (srtm.getAltitude(center.Lat, center.Lng).alt + 1)*heightscale
                : center.Alt*heightscale; // (srtm.getAltitude(lookZ, lookX, 20) + 100) * heighscale;

            lookX = center.Lng + Math.Sin(yawradians)*mouseY;
            lookY = center.Lat + Math.Cos(yawradians)*mouseY;
            lookZ = cameraZ;

            // cameraZ += 0.04;

            GMapProvider type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            PureProjection prj = type.Projection;

            int size = (int) (cameraZ*150000);

            // in front
            PointLatLngAlt leftf = center.newpos(rpy.Z, size);
            // behind
            PointLatLngAlt rightf = center.newpos(rpy.Z, 50);
            // left : 90 allows for 180 degree viewing angle
            PointLatLngAlt left = center.newpos(rpy.Z - 90, size);
            // right
            PointLatLngAlt right = center.newpos(rpy.Z + 90, size);

            double maxlat = Math.Max(left.Lat, Math.Max(right.Lat, Math.Max(leftf.Lat, rightf.Lat)));
            double minlat = Math.Min(left.Lat, Math.Min(right.Lat, Math.Min(leftf.Lat, rightf.Lat)));

            double maxlng = Math.Max(left.Lng, Math.Max(right.Lng, Math.Max(leftf.Lng, rightf.Lng)));
            double minlng = Math.Min(left.Lng, Math.Min(right.Lng, Math.Min(leftf.Lng, rightf.Lng)));

            // if (Math.Abs(area.Lat - maxlat) < 0.001)
            {
            }
            // else
            {
                area = RectLatLng.FromLTRB(minlng, maxlat, maxlng, minlat);
            }

            GPoint topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
            GPoint rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
            GPoint pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);

            zoom = 21;
            pxDelta.X = 9999;

            int otherzoomlevel = 12;

            // zoom based on pixel density
            while (pxDelta.X > this.Width)
            {
                zoom--;

                // current area
                topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
                rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
                pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);
            }

            otherzoomlevel = zoom - 4;

            Console.WriteLine("zoom {0}", zoom);

            // update once per seconds - we only read from disk, so need to let cahce settle
            if (lastrefresh.AddSeconds(0.5) < DateTime.Now)
            {
                // get tiles - bg
                core.Provider = type;
                core.Position = LocationCenter;

                // get zoom 10
                core.Zoom = otherzoomlevel;
                core.OnMapSizeChanged(this.Width, this.Height);

                // get actual current zoom
                core.Zoom = zoom;
                core.OnMapSizeChanged(this.Width, this.Height);

                lastrefresh = DateTime.Now;
            }
            else
            {
                //return;
            }

            float screenscale = this.Width/(float) this.Height;

            MakeCurrent();

            GL.MatrixMode(MatrixMode.Projection);

            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView(120*deg2rad, screenscale, 0.00001f,
                (float) step*20000);
            GL.LoadMatrix(ref projection);

            Matrix4 modelview = Matrix4.LookAt((float) cameraX, (float) cameraY, (float) cameraZ, (float) lookX,
                (float) lookY, (float) lookZ, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);

            // roll
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationZ(rpy.X*deg2rad));
            // pitch
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationX((rpy.Y - 15)*-deg2rad));

            GL.LoadMatrix(ref modelview);

            GL.ClearColor(Color.CornflowerBlue);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.AccumBufferBit);

            GL.LightModel(LightModelParameter.LightModelAmbient, new float[] {1f, 1f, 1f, 1f});

            //  GL.Disable(EnableCap.Fog);
            GL.Enable(EnableCap.Fog);
            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);

            GL.Fog(FogParameter.FogColor, new float[] {100/255.0f, 149/255.0f, 237/255.0f, 1f});
            //GL.Fog(FogParameter.FogDensity,0.1f);
            GL.Fog(FogParameter.FogMode, (int) FogMode.Linear);
            GL.Fog(FogParameter.FogStart, (float) step*40);
            GL.Fog(FogParameter.FogEnd, (float) (step*50));

//            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Always);

            /*
            GL.Begin(BeginMode.LineStrip);

            GL.Color3(Color.White);
            GL.Vertex3(0, 0, 0);

            //GL.Color3(Color.Red);
            GL.Vertex3(area.Bottom, 0, area.Left);

            //GL.Color3(Color.Yellow);
            GL.Vertex3(lookX, lookY, lookZ);

            //GL.Color3(Color.Green);
            GL.Vertex3(cameraX, cameraY, cameraZ);

            GL.End();
             */
            /*
            GL.PointSize(10);
            GL.Color4(Color.Yellow);
            GL.LineWidth(5);
           

            GL.Begin(PrimitiveType.LineStrip);
 
            //GL.Vertex3(new Vector3((float)center.Lng,(float)center.Lat,(float)(center.Alt * heightscale)));
            //GL.Vertex3(new Vector3(0, 0, 0));
            //GL.Vertex3(new Vector3((float)cameraX, (float)cameraY, (float)cameraZ));
            //GL.Color3(Color.Green);
            //GL.Vertex3(new Vector3((float)lookX, (float)lookY, (float)lookZ));

            GL.Vertex3(area.LocationTopLeft.Lng, area.LocationTopLeft.Lat, (float)cameraZ);
            GL.Vertex3(area.LocationTopLeft.Lng, area.LocationRightBottom.Lat, (float)cameraZ);
            GL.Vertex3(area.LocationRightBottom.Lng, area.LocationRightBottom.Lat, (float)cameraZ);
            GL.Vertex3(area.LocationRightBottom.Lng, area.LocationTopLeft.Lat, (float)cameraZ);
            GL.Vertex3(area.LocationTopLeft.Lng, area.LocationTopLeft.Lat, (float)cameraZ);

            GL.End();
            */
            GL.Finish();

            GL.PointSize((float) (step*1));
            GL.Color3(Color.Blue);
            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(new Vector3((float) center.Lng, (float) center.Lat, (float) cameraZ));
            GL.End();


            //GL.ClampColor(ClampColorTarget.ClampReadColor, ClampColorMode.True);
            /*
            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);
            GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.Src1Color);
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
            */
            // textureid.Clear();

            // get level 10 tiles
            List<GPoint> tileArea1 = prj.GetAreaTileList(area, otherzoomlevel, 1);

            // get type list at new zoom level
            List<GPoint> tileArea2 = prj.GetAreaTileList(area, zoom, 2);

            List<GPoint> tileArea = new List<GPoint>();

            tileArea.AddRange(tileArea1);
            tileArea.AddRange(tileArea2);

            // get tiles & combine into one
            foreach (var p in tileArea)
            {
                int localzoom = zoom;

                core.tileDrawingListLock.AcquireReaderLock();
                core.Matrix.EnterReadLock();
                try
                {
                    if (tileArea1.Contains(p))
                        localzoom = otherzoomlevel;

                    topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, localzoom);

                    GMap.NET.Internals.Tile t = core.Matrix.GetTileWithNoLock(localzoom, p);

                    if (t.NotEmpty)
                    {
                        foreach (GMapImage img in t.Overlays)
                        {
                            if (!textureid.ContainsKey(p))
                            {
                                generateTexture(p, (Bitmap) img.Img);
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
                    //Console.WriteLine("Missing tile");
                    continue;
                }

                long x = p.X*prj.TileSize.Width - topLeftPx.X;
                long y = p.Y*prj.TileSize.Width - topLeftPx.Y;

                long xr = p.X*prj.TileSize.Width;
                long yr = p.Y*prj.TileSize.Width;

                long x2 = (p.X + 1)*prj.TileSize.Width;
                long y2 = (p.Y + 1)*prj.TileSize.Width;


                GL.LineWidth(0);
                GL.Color3(Color.White);

                // generate terrain
                GL.Begin(PrimitiveType.TriangleStrip);

                var latlng = prj.FromPixelToLatLng(xr, yr, localzoom);

                double heightl = srtm.getAltitude(latlng.Lat, latlng.Lng).alt;
                if (localzoom == 10)
                    heightl = 0;

                //xr - topLeftPx.X, yr - topLeftPx.Y
                GL.TexCoord2(0, 0);
                GL.Vertex3(latlng.Lng, latlng.Lat, heightl*heightscale);


                // next down
                latlng = prj.FromPixelToLatLng(xr, y2, localzoom);

                heightl = srtm.getAltitude(latlng.Lat, latlng.Lng).alt;
                if (localzoom == 10)
                    heightl = 0;

                GL.TexCoord2(0, 1);
                GL.Vertex3(latlng.Lng, latlng.Lat, heightl*heightscale);


                // next right
                latlng = prj.FromPixelToLatLng(x2, yr, localzoom);

                heightl = srtm.getAltitude(latlng.Lat, latlng.Lng).alt;
                if (localzoom == 10)
                    heightl = 0;

                GL.TexCoord2(1, 0);
                GL.Vertex3(latlng.Lng, latlng.Lat, heightl*heightscale);


                // next right down
                latlng = prj.FromPixelToLatLng(x2, y2, localzoom);

                heightl = srtm.getAltitude(latlng.Lat, latlng.Lng).alt;
                if (localzoom == 10)
                    heightl = 0;

                GL.TexCoord2(1, 1);
                GL.Vertex3(latlng.Lng, latlng.Lat, heightl*heightscale);

                GL.End();
            }

            GL.Flush();

            try
            {
                this.SwapBuffers();


                Context.MakeCurrent(null);
            }
            catch
            {
            }

            //this.Invalidate();

            return;
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