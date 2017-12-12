using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using MathHelper = MissionPlanner.Utilities.MathHelper;
using Vector3 = OpenTK.Vector3;

namespace MissionPlanner.Controls
{
    public class OpenGLtest2 : GLControl
    {
        public static OpenGLtest2 instance;

        // terrain image
        Bitmap _terrain = new Bitmap(640, 480);
        Dictionary<GPoint, int> textureid = new Dictionary<GPoint, int>();

        GMap.NET.Internals.Core core = new GMap.NET.Internals.Core();

        private GMapProvider type;
        private PureProjection prj;

        double cameraX, cameraY, cameraZ; // camera coordinates
        double lookX, lookY, lookZ; // camera look-at coordinates

        double step = 1/1200.0;

        // image zoom level
        int zoom = 14;

        RectLatLng area = new RectLatLng(-35.04286, 117.84262, 0.1, 0.1);
        PointLatLngAlt center = new PointLatLngAlt(-35.04286, 117.84262,40);

        public PointLatLngAlt LocationCenter
        {
            get { return center; }
            set
            {
                if (value.Lat == 0 && value.Lng == 0)
                    return;

                if (center.Lat == value.Lat && center.Lng == value.Lng)
                    return;

                center.Lat = Math.Round(value.Lat,7);
                center.Lng = Math.Round(value.Lng,7);
                center.Alt = Math.Round(value.Alt,1);

                this.Invalidate();
            }
        }

        Vector3 _rpy = new Vector3();

        public Vector3 rpy
        {
            get { return _rpy; }
            set { _rpy.X = (float)Math.Round(value.X,1); _rpy.Y = (float)Math.Round(value.Y, 1); _rpy.Z = (float)Math.Round(value.Z, 1); this.Invalidate(); }
        }

        public OpenGLtest2()
        {
            instance = this;

            InitializeComponent();

            core.OnMapOpen();

            type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            prj = type.Projection;

            this.Invalidate();

            Thread bg = new Thread(imageLoader) {IsBackground = true};
            bg.Start();
        }

        public Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
        {
            var dir = Vector3.Cross(b - a, c - a);
            var norm = Vector3.Normalize(dir);
            return norm;
        }

        void generateTexture(GPoint point, Bitmap image)
        {
            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int texture = 0;

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode,
                (float)TextureEnvModeCombine.Replace); //Important, or wrong color on some computers

            GL.GenTextures(1, out texture);

            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);

            GL.End();

            image.Dispose();

            textureid[point] = texture;
        }

        void imageLoader()
        {
            var lastpos = PointLatLngAlt.Zero;

            while (!this.IsDisposed)
            {
                System.Threading.Thread.Sleep(500);

                // update once per seconds - we only read from disk, so need to let cahce settle
                if (lastrefresh.AddSeconds(5) < DateTime.Now || lastpos.GetDistance(LocationCenter) > 50)
                {
                    // get tiles - bg
                    //core.Provider = type;
                    //core.Position = LocationCenter;

                    // preload zooms
                    for (int z = 10; z <= zoom; z++)
                    {
                        //core.Zoom = z;
                        //core.OnMapSizeChanged(this.Width, this.Height);
                    }

                    lastpos = new PointLatLngAlt(core.Position);
                    lastrefresh = DateTime.Now;
                }
                else
                {
                    //return;
                }
               
            }
        }

        DateTime lastrefresh = DateTime.MinValue;

        private int utmzone = -999;

        double[] convertCoords(PointLatLngAlt plla)
        {
            if (utmzone < -360)
                utmzone = plla.GetUTMZone();

            var utm = plla.ToUTM(utmzone);

            Array.Resize(ref utm, 3);

            utm[2] = plla.Alt;

            return utm;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            DateTime start = DateTime.Now;
            
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

            utmzone = center.GetUTMZone();

            double heightscale = 1;//(step/90.0)*5;

            var campos = convertCoords(center);

            cameraX = campos[0];
            cameraY = campos[1];
            cameraZ = (campos[2] < srtm.getAltitude(center.Lat, center.Lng).alt)
                ? (srtm.getAltitude(center.Lat, center.Lng).alt + 1)*heightscale
                : center.Alt*heightscale; // (srtm.getAltitude(lookZ, lookX, 20) + 100) * heighscale;

            lookX = campos[0] + Math.Sin(MathHelper.Radians(rpy.Z)) * 100;
            lookY = campos[1] + Math.Cos(MathHelper.Radians(rpy.Z)) * 100;
            lookZ = cameraZ;

            var size = 20000;

            // in front
            PointLatLngAlt front = center.newpos(rpy.Z, size);
            // behind
            PointLatLngAlt behind = center.newpos(rpy.Z, -50);
            // left : 90 allows for 180 degree viewing angle
            PointLatLngAlt left = center.newpos(rpy.Z - 45, size);
            // right
            PointLatLngAlt right = center.newpos(rpy.Z + 45, size);

            double maxlat = Math.Max(left.Lat, Math.Max(right.Lat, Math.Max(front.Lat, behind.Lat)));
            double minlat = Math.Min(left.Lat, Math.Min(right.Lat, Math.Min(front.Lat, behind.Lat)));

            double maxlng = Math.Max(left.Lng, Math.Max(right.Lng, Math.Max(front.Lng, behind.Lng)));
            double minlng = Math.Min(left.Lng, Math.Min(right.Lng, Math.Min(front.Lng, behind.Lng)));

            area = RectLatLng.FromLTRB(minlng, maxlat, maxlng, minlat);

            zoom = 20;

       
            float screenscale = 1;//this.Width/(float) this.Height*1f;

            if(!Context.IsCurrent)
                MakeCurrent();

            GL.MatrixMode(MatrixMode.Projection);

            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)(90*MathHelper.deg2rad), screenscale, 0.00000001f,
                (float) 20000);
            GL.LoadMatrix(ref projection);

            Console.WriteLine("cam: {0} {1} {2} lookat: {3} {4} {5}", (float) cameraX, (float) cameraY, (float) cameraZ,
                (float) lookX,
                (float) lookY, (float) lookZ);

            Matrix4 modelview = Matrix4.LookAt((float) cameraX, (float) cameraY, (float) cameraZ+100f*0, (float) lookX,
                (float) lookY, (float) lookZ, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);

            // roll
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationZ((float)(rpy.X*MathHelper.deg2rad)));
            // pitch
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationX((float)(rpy.Y*-MathHelper.deg2rad)));

            GL.LoadMatrix(ref modelview);

            GL.ClearColor(Color.CornflowerBlue);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.AccumBufferBit);

            GL.LightModel(LightModelParameter.LightModelAmbient, new float[] {1f, 1f, 1f, 1f});

              GL.Disable(EnableCap.Fog);
            GL.Enable(EnableCap.Fog);
            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);

            GL.Fog(FogParameter.FogColor, new float[] {100/255.0f, 149/255.0f, 237/255.0f, 1f});
            //GL.Fog(FogParameter.FogDensity,0.1f);
            GL.Fog(FogParameter.FogMode, (int) FogMode.Linear);
            GL.Fog(FogParameter.FogStart, (float) 4000);
            GL.Fog(FogParameter.FogEnd, (float) size);

            GL.Disable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Always);

            /*
            GL.Begin(BeginMode.LineStrip);

            GL.Color3(Color.White);
            GL.Vertex3(0, 0, 0);

            GL.Color3(Color.Red);
            GL.Vertex3(area.Bottom, 0, area.Left);

            GL.Color3(Color.Yellow);
            GL.Vertex3(lookX, lookY, lookZ);

            GL.Color3(Color.Green);
            GL.Vertex3(cameraX, cameraY, cameraZ);

            GL.End();
            */
            /*
            GL.PointSize(10);
            GL.Color4(Color.Yellow);
            GL.LineWidth(5);
           

            GL.Begin(PrimitiveType.LineStrip);

            GL.Vertex3(area.LocationTopLeft.Lng, area.LocationTopLeft.Lat, (float)cameraZ);
            GL.Vertex3(area.LocationTopLeft.Lng, area.LocationRightBottom.Lat, (float)cameraZ);
            GL.Vertex3(area.LocationRightBottom.Lng, area.LocationRightBottom.Lat, (float)cameraZ);
            GL.Vertex3(area.LocationRightBottom.Lng, area.LocationTopLeft.Lat, (float)cameraZ);
            GL.Vertex3(area.LocationTopLeft.Lng, area.LocationTopLeft.Lat, (float)cameraZ);

            GL.End();

            GL.PointSize((float) (step*1));
            GL.Color3(Color.Blue);
            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(new Vector3((float) center.Lng, (float) center.Lat, (float) cameraZ));
            GL.End();
            */

            //GL.ClampColor(ClampColorTarget.ClampReadColor, ClampColorMode.True);
            /*
            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);
            GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.Src1Color);
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
            */
            // textureid.Clear();

            core.fillEmptyTiles = true;

            core.LevelsKeepInMemmory = 20;

            core.Provider = type;
            core.Position = center;

            //core.ReloadMap();

            List<tileZoomArea> tileArea = new List<tileZoomArea>();

            for (int a = 10; a <= zoom; a++)
            {
                core.Zoom = a;

                var area2 = new RectLatLng(center.Lat, center.Lng, 0, 0);

                // 200m at max zoom
                // step at 0 zoom
                var distm = MathHelper.map(a, 0, zoom, size, 50);

                var offset = center.newpos(rpy.Z, distm);

                area2.Inflate(Math.Abs(center.Lat - offset.Lat), Math.Abs(center.Lng - offset.Lng));

                var extratile = 0;

                if (a == zoom)
                    extratile = 1;

                var tiles = new tileZoomArea() {zoom = a, points = prj.GetAreaTileList(area2, a, extratile), area = area2 };

                tileArea.Add(tiles);
            }

            //tileArea.Reverse();

            while (textureid.Count > 250)
            {
                var first = textureid.Keys.First();
                GL.DeleteTexture(textureid[first]);
                textureid.Remove(first);
            }

            // get tiles & combine into one
            foreach (var tilearea in tileArea)
            {
                foreach (var p in tilearea.points)
                {
                    core.tileDrawingListLock.AcquireReaderLock();
                    core.Matrix.EnterReadLock();
                    try
                    {
                        GMap.NET.Internals.Tile t = core.Matrix.GetTileWithNoLock(tilearea.zoom, p);

                        if (t.NotEmpty)
                        {
                            foreach (GMapImage img in t.Overlays)
                            {
                                if (img.IsParent)
                                {
                                    
                                }

                                if (!textureid.ContainsKey(p))
                                {
                                    try
                                    {
                                        generateTexture(p, (Bitmap) img.Img);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
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

                    if (textureid.ContainsKey(p))
                    {
                        int texture = textureid[p];

                        GL.Enable(EnableCap.Texture2D);
                        GL.BindTexture(TextureTarget.Texture2D, texture);
                    }
                    else
                    {
                        //Console.WriteLine("Missing tile");
                        GL.Disable(EnableCap.Texture2D);
                        continue;
                    }

                    long xr = p.X * prj.TileSize.Width;
                    long yr = p.Y * prj.TileSize.Width;

                    long x2 = (p.X + 1) * prj.TileSize.Width;
                    long y2 = (p.Y + 1) * prj.TileSize.Width;

                    GL.LineWidth(4);
                    GL.Color3(Color.White);

                    GL.Clear(ClearBufferMask.DepthBufferBit);

                    GL.Enable(EnableCap.DepthTest);
                    
                    // generate terrain
                    GL.Begin(PrimitiveType.Points);

                    GL.PointSize((float)(20));
                  
                    //GL.Begin(PrimitiveType.Points);
                    GL.Color3(Color.Blue);
                    
                    var latlng = prj.FromPixelToLatLng(xr, yr, tilearea.zoom);
                    var utm = convertCoords(latlng);
                    utm[2] = srtm.getAltitude(latlng.Lat, latlng.Lng).alt;
         
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(utm[0], utm[1], utm[2]);

                    // next down
                    latlng = prj.FromPixelToLatLng(xr, y2, tilearea.zoom);
                    utm = convertCoords(latlng);
                    utm[2] = srtm.getAltitude(latlng.Lat, latlng.Lng).alt;

                    GL.TexCoord2(0, 1);
                    GL.Vertex3(utm[0], utm[1], utm[2]);

                    // next right
                    latlng = prj.FromPixelToLatLng(x2, yr, tilearea.zoom);
                    utm = convertCoords(latlng);
                    utm[2] = srtm.getAltitude(latlng.Lat, latlng.Lng).alt;

                    GL.TexCoord2(1, 0);
                    GL.Vertex3(utm[0], utm[1], utm[2]);

                    // next right down
                    latlng = prj.FromPixelToLatLng(x2, y2, tilearea.zoom);
                    utm = convertCoords(latlng);
                    utm[2] = srtm.getAltitude(latlng.Lat, latlng.Lng).alt;

                    GL.TexCoord2(1, 1);
                    GL.Vertex3(utm[0], utm[1], utm[2]);
                    
                    GL.End();

                    var dist = LocationCenter.GetDistance(latlng);

                    var pxstep = 128;

                    if (dist < 500)
                        pxstep = 32;

                    double[] oldutm = null;
                    GL.Begin(PrimitiveType.TriangleStrip);
                    for (long x = xr; x < x2; x += pxstep)
                    {
                        long xnext = x + pxstep;
                        //GL.Begin(PrimitiveType.LineStrip);
                        for (long y = yr;y < y2; y += pxstep)
                        {
                            long ynext = y + pxstep;

                            //GL.Begin(PrimitiveType.Lines);
                            var latlng1 = prj.FromPixelToLatLng(x, y, tilearea.zoom);
                            var utm1 = convertCoords(latlng1);
                            utm1[2] = srtm.getAltitude(latlng1.Lat, latlng1.Lng).alt;

                            var imgx = MathHelper.map(x, xr, x2, 0, 1);
                            var imgy = MathHelper.map(y, yr, y2, 0, 1);

                            GL.TexCoord2(imgx, imgy);
                            GL.Vertex3(utm1[0], utm1[1], utm1[2]);

                            //
                            var latlng2 = prj.FromPixelToLatLng(x, ynext, tilearea.zoom);
                            var utm2 = convertCoords(latlng2);
                            utm2[2] = srtm.getAltitude(latlng2.Lat, latlng2.Lng).alt;

                            imgx = MathHelper.map(x, xr, x2, 0, 1);
                            imgy = MathHelper.map(ynext, yr, y2, 0, 1);

                            GL.TexCoord2(imgx, imgy);
                            GL.Vertex3(utm2[0], utm2[1], utm2[2]);

                            //
                            latlng2 = prj.FromPixelToLatLng(xnext, y, tilearea.zoom);
                            utm2 = convertCoords(latlng2);
                            utm2[2] = srtm.getAltitude(latlng2.Lat, latlng2.Lng).alt;

                            imgx = MathHelper.map(xnext, xr, x2, 0, 1);
                            imgy = MathHelper.map(y, yr, y2, 0, 1);

                            GL.TexCoord2(imgx, imgy);
                            GL.Vertex3(utm2[0], utm2[1], utm2[2]);
                   
                            //
                            latlng2 = prj.FromPixelToLatLng(xnext, ynext, tilearea.zoom);
                            utm2 = convertCoords(latlng2);
                            utm2[2] = srtm.getAltitude(latlng2.Lat, latlng2.Lng).alt;

                            imgx = MathHelper.map(xnext, xr, x2, 0, 1);
                            imgy = MathHelper.map(ynext, yr, y2, 0, 1);

                            GL.TexCoord2(imgx, imgy);
                            GL.Vertex3(utm2[0], utm2[1], utm2[2]);
                        }
                    }

                    GL.End();
                    GL.Disable(EnableCap.Texture2D);
                }
            }

            GL.Flush();

            try
            {
                this.SwapBuffers();


                //Context.MakeCurrent(null);
            }
            catch
            {
            }

            //this.Invalidate();

            var delta = DateTime.Now - start;
            Console.WriteLine("OpenGLTest2 {0}", delta.TotalMilliseconds);
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

            core.OnMapSizeChanged(this.Width, this.Height);

            this.Invalidate();
        }

        public class tileZoomArea
        {
            public List<GPoint> points;
            public int zoom;
            public RectLatLng area { get; set; }
        }
    }
}