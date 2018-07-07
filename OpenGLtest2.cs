using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
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

        PointLatLngAlt oldcenter = PointLatLngAlt.Zero;
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

        private Dictionary<object, double> coordcache = new Dictionary<object, double>();

        double[] convertCoords(PointLatLngAlt plla)
        {
            if (utmzone < -360)
                utmzone = plla.GetUTMZone();

            var minlat = LocationCenter.Lat - 0.5;
            var maxlat = LocationCenter.Lat + 0.5;
            var minlng = LocationCenter.Lng - 0.5;
            var maxlng = LocationCenter.Lng + 0.5;

            var id = maxlat * 1e10 + minlng;
            var diagdist = 0.0;

            if (!coordcache.ContainsKey(id))
            {
                diagdist = new PointLatLngAlt(maxlat, minlng).GetDistance(new PointLatLngAlt(minlat, maxlng));
                coordcache[id] = diagdist;
            }
            else
            {
                diagdist = coordcache[id];
            }

            var lat = MathHelper.map(plla.Lat, minlat, maxlat, 0, diagdist);
            var lng = MathHelper.map(plla.Lng, minlng, maxlng, 0, diagdist);

            //var utm = plla.ToUTM(utmzone);

            //Array.Resize(ref utm, 3);

            //utm[2] = plla.Alt;

            return new[] {lng, lat, plla.Alt};
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch
            {
                return;
            }

            Utilities.Extensions.ProtectReentry(doPaint);
        }

        public void doPaint()
        {
            DateTime start = DateTime.Now;
            
            if (this.DesignMode)
                return;

            if (area.LocationMiddle.Lat == 0 && area.LocationMiddle.Lng == 0)
                return;

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

            var size = 10000;

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
            GL.Fog(FogParameter.FogStart, (float) 700);
            GL.Fog(FogParameter.FogEnd, (float) size);

            GL.Disable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Always);

          
            if (center.GetDistance(oldcenter) > 500)
            {
                oldcenter = center;
            }

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

                //Console.WriteLine("tiles z {0} max {1} dist {2}", a, zoom, distm);

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

            mesh.RenderVBO();

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

            var delta = DateTime.Now - start;
            Console.WriteLine("OpenGLTest2 {0}", delta.TotalMilliseconds);
        }

        private Mesh mesh;

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

            mesh = new Mesh();
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

        public class Mesh
        {
            public Mesh()
            {
                InitializeVBO();
            }

            //A simple 3D vertex
            [StructLayout(LayoutKind.Sequential)]
            public struct Vertex
            {
                public float X, Y, Z;
                public Vertex(float x, float y, float z)
                {
                    X = x; Y = y; Z = z;
                }

                //The stride of this vertex
                //3 floats 4 bytes each
                //if you dont like counting bytes you can get this value a few different ways
                //System.Runtime.InteropServices.Marshal.SizeOf(new Vertex());
                //OpenTK.BlittableValueType.StrideOf(new Vertex());
                public static readonly int Stride = System.Runtime.InteropServices.Marshal.SizeOf(new Vertex());
            }

            //ID of the vertex buffer we will use
            private int ID_VBO;
            //ID of the element buffer
            private int ID_EBO;

            //You are not limited to using a struct
            //you can use (float[], byte[], int[]) really anything that can hold data
            //then you could stride the array with
            //Vertices.Length * sizeof(float)
            private Vertex[] Vertices;
            //I use ushort, for the simple reasons of
            //i know im never going to index a negative value
            //and 65,535 is a good vertex limit
            //but you can use whatever just remember the max_value is your limit
            private ushort[] Indices;

            //Keep in mind that you are using memory for everything
            //the best way to think about any GL buffer is as if its byte[] or ByteArray/ByteBuffer

            //Only need to call this once, place it in your programs initialize/load method
            private void InitializeVBO()
            {
                //Initialize vertex data and index data

                //Simple triangle in CW winding
                Vertices = new Vertex[3]
                {
        new Vertex(  0.0f,   0.0f, 0.0f),
        new Vertex(100.0f,   0.0f, 0.0f),
        //gl y coords are reversed depending on what direction you consider is down
        new Vertex(  0.0f, -100.0f, 0.0f)
                };

                //Index data for what order to draw vertices
                Indices = new ushort[3]
                {
        //This is also something to keep in mind
        //reversing your winding has no performance advantage, 
        //dont let someone tell you otherwise, all data gets read the same way
        //its always easier to change a few indices than edit vertices
        //since you are most likely in CCW winding since that is GL's default
        //i reversed it since the vertices are in CW :D
        //you can change GL's default winding by GL.FrontFace
        0, 1, 2
                };

                //Setting up the vertex buffer
                //data has to be initialize before this part.

                //Generate a single buffer
                GL.GenBuffers(1, out ID_VBO);
                //Tell gl we are going to be using this buffer as an Array_Buffer for vertex data
                GL.BindBuffer(BufferTarget.ArrayBuffer, ID_VBO);
                //Add our vertex data to it, essentially the same thing as Array.Copy if it were a byte[]
                //1: targeting the Array_Buffer
                //2: for this we will need the full length in bytes of our vertex data
                //3: the data we want in this new buffer
                //4: we are only going to set this once so our usage will be static and draw/write
                //basically in full we're are telling gl that we want a write-only buffer that is locked :P
                GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Vertex.Stride), Vertices, BufferUsageHint.StaticDraw);
                //This is just good practice
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                //Generate another single buffer - for indices
                GL.GenBuffers(1, out ID_EBO);
                //Tell gl we are going to be using this buffer as an Element_Buffer for indexing data already bound to an Array_Buffer
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID_EBO);
                //the same as above only changing the data
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(ushort)), Indices, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            //This will need to be called everyframe you want to see the buffer data so use in OnRenderFrame
            //or any method with a GL render context.
            public void RenderVBO()
            {
                GL.PushMatrix();

                //So you can see it onscreen :P
                GL.Translate(-50, 50, -100);

                //We dont have color data so it will resort to the last use GL_Color
                GL.Color3(1.0f, 1.0f, 1.0f);

                //since we only have position data thats all that needs to be writen to gl
                //if you enable another as in ArrayCap.ColorArray and have no color data
                //you will get an outofbounds/outofmemory exception because there is not data to read
                //so good practice to enable/disable so you dont run into that problem when using
                //different types of vertices :D
                //Although if you know what type you are going to use throughout the whole process
                //you can remove enable/disable from this method and add enable to your init function
                GL.EnableClientState(ArrayCap.VertexArray);

                GL.EnableClientState(ArrayCap.TextureCoordArray);

                //Bind our vertex data
                GL.BindBuffer(BufferTarget.ArrayBuffer, ID_VBO);
                //Tell gl where to start reading our position data in the length of out Vertex.Stride
                //so we will begin reading 3 floats with a length of 12 starting at 0
                GL.VertexPointer(3, VertexPointerType.Float, Vertex.Stride, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID_EBO);
                //tell gl to draw from the bound Array_Buffer in the form of triangles with a length of indices of type ushort starting at 0
                GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedShort, IntPtr.Zero);

                //unlike above you will have to unbind after the data is indexed else the Element_Buffer would have nothing to index
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                //Remember to disable
                GL.DisableClientState(ArrayCap.VertexArray);

                GL.PopMatrix();
            }
        }
    }
}