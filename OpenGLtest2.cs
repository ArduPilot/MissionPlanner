using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using GMap.NET.MapProviders;
using Microsoft.Scripting.Utils;
using MathHelper = MissionPlanner.Utilities.MathHelper;
using Vector3 = OpenTK.Vector3;
using System.IO;
using GMap.NET.Drawing;

namespace MissionPlanner.Controls
{
    public class OpenGLtest2 : GLControl, IDeactivate
    {
        public static OpenGLtest2 instance;

        int green = 0;

        ConcurrentDictionary<GPoint, tileInfo> textureid = new ConcurrentDictionary<GPoint, tileInfo>();

        GMap.NET.Internals.Core core = new GMap.NET.Internals.Core();

        private GMapProvider type;
        private PureProjection prj;

        double cameraX, cameraY, cameraZ; // camera coordinates
        double lookX, lookY, lookZ; // camera look-at coordinates

        // image zoom level
        int zoom = 20;

        RectLatLng area = new RectLatLng(-35.04286, 117.84262, 0.1, 0.1);
        PointLatLngAlt center = new PointLatLngAlt(-35.04286, 117.84262, 40);

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

                center.Lat = center.Lat * 0.5 + value.Lat * 0.5;
                center.Lng = center.Lng * 0.5 + value.Lng * 0.5;
                center.Alt = center.Alt * 0.5 + value.Alt * 0.5;

                this.Invalidate();
            }
        }

        Vector3 _rpy = new Vector3();

        public Vector3 rpy
        {
            get { return _rpy; }
            set
            {
                _rpy.X = (float) Math.Round(value.X, 2);
                _rpy.Y = (float) Math.Round(value.Y, 2);
                _rpy.Z = (float) Math.Round(value.Z, 2);
                this.Invalidate();
            }
        }
        public List<Locationwp> WPs { get; set; }
        public OpenGLtest2()
        {
            instance = this;

            InitializeComponent();

            Click += OnClick;
            MouseMove+= OnMouseMove;
            MouseDown+= OnMouseDown;

            core.OnMapOpen();

            type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            prj = type.Projection;

            this.Invalidate();

            Thread bg = new Thread(imageLoader) {IsBackground = true};
            bg.Start();
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            var x = ((MouseEventArgs)e).X;
            var y = ((MouseEventArgs)e).Y;

            mouseDownPos = getMousePos(x, y);

            MainV2.comPort.setGuidedModeWP(
                new Locationwp().Set(mouseDownPos.Lat, mouseDownPos.Lng, MainV2.comPort.MAV.GuidedMode.z, (ushort)MAVLink.MAV_CMD.WAYPOINT), false);
            //MainV2.C
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var x = ((MouseEventArgs) e).X;
            var y = ((MouseEventArgs) e).Y;

           // var point = getMousePos(x, y);

/*
            GL.Color3(Color.White);

            GL.Begin(BeginMode.Lines);
            //GL.Vertex3(_start.X, _start.Y, _start.Z);
            GL.Vertex3(0, 0, 0);
            //GL.Vertex3(_end.X, _end.Y, _end.Z);

            var utm = convertCoords(point);

            GL.Vertex3(utm[0], utm[1], point.Alt);

            GL.End();

            SwapBuffers();
            */
            //Thread.Sleep(1000);
        }

        public PointLatLngAlt getMousePos(int x, int y)
        { 
            //https://gamedev.stackexchange.com/questions/103483/opentk-ray-picking
            int[] viewport = new int[4];
            Matrix4 modelMatrix, projMatrix;

            MakeCurrent();

            GL.GetFloat(GetPName.ModelviewMatrix, out modelMatrix);
            GL.GetFloat(GetPName.ProjectionMatrix, out projMatrix);
            GL.GetInteger(GetPName.Viewport, viewport);

            var _start = UnProject(new Vector3(x, y, 0.0f), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
            var _end = UnProject(new Vector3(x, y, 1), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));

            var pos = new utmpos(utmcenter[0] + _end.X, utmcenter[1] + _end.Y, utmzone);

            var plla = pos.ToLLA();
            plla.Alt = _end.Z;

            var point = srtm.getIntersectionWithTerrain(center, plla);

            return point;
        }

        private void OnClick(object sender, EventArgs e)
        {
            
        }

        public static Vector3 UnProject(Vector3 mouse, Matrix4 projection, Matrix4 view, Size viewport)
        {
            Vector4 vec;

            vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = mouse.Z;
            vec.W = 1.0f;

            Matrix4 viewInv = Matrix4.Invert(view);
            Matrix4 projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > 0.000001f || vec.W < -0.000001f)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec.Xyz;
        }

        ~OpenGLtest2()
        {
            foreach (var tileInfo in textureid)
            {
                try
                {
                    tileInfo.Value.Cleanup();
                } catch { }
            }
        }

        public void Deactivate()
        {
            foreach (var tileInfo in textureid)
            {
                try
                {
                    tileInfo.Value.Cleanup();
                }
                catch { }
            }

            textureid.Clear();
        }

        public Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
        {
            var dir = Vector3.Cross(b - a, c - a);
            var norm = Vector3.Normalize(dir);
            return norm;
        }

        static int generateTexture(Bitmap image)
        {
            image.MakeTransparent();

            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int texture = generateTexture(data);

            image.UnlockBits(data);

            if (texture == 0)
            {
                image.Dispose();
                var error = GL.GetError();
            }
            else
            {
                image.Dispose();
            }

            return texture;
        }

        static int generateTexture(BitmapData data)
        {
            int texture = 0;
   
            GL.GenTextures(1, out texture);

            if (texture == 0)
            {
                return 0;
            }

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode,
                (float)TextureEnvModeCombine.Replace); //Important, or wrong color on some computers

            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);



            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);

            GL.GenerateTextureMipmap(texture);

            return texture;
        }

        void imageLoader()
        {
            core.Zoom = 12;

            while (!this.IsDisposed)
            {
                System.Threading.Thread.Sleep(1000);

                if (core.tileLoadQueue.Count > 0)
                    continue;

                if (core.Zoom >= zoom)
                    core.Zoom = 12;

                core.Zoom = core.Zoom + 1;

                generateTextures();
            }
        }

        private int utmzone = -999;
        private double[] utmcenter = new double[2];
        private PointLatLngAlt mouseDownPos;

        double[] convertCoords(PointLatLngAlt plla)
        {
            if (utmzone != plla.GetUTMZone())
            {
                utmzone = plla.GetUTMZone();

                utmcenter = plla.ToUTM(utmzone);

                textureid.ForEach(a => a.Value.Cleanup());

                textureid.Clear();
            }

            var utm = plla.ToUTM(utmzone);

            Array.Resize(ref utm, 3);

            utm[0] -= utmcenter[0];
            utm[1] -= utmcenter[1];

            utm[2] = plla.Alt;

            return new[] { utm[0], utm[1], utm[2]};
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

            core.Position = center;

            double heightscale = 1; //(step/90.0)*5;

            var campos = convertCoords(center);

            cameraX = campos[0];
            cameraY = campos[1];
            cameraZ = (campos[2] < srtm.getAltitude(center.Lat, center.Lng).alt)
                ? (srtm.getAltitude(center.Lat, center.Lng).alt + 1) * heightscale
                : center.Alt * heightscale; // (srtm.getAltitude(lookZ, lookX, 20) + 100) * heighscale;

            lookX = campos[0] + Math.Sin(MathHelper.Radians(rpy.Z)) * 100;
            lookY = campos[1] + Math.Cos(MathHelper.Radians(rpy.Z)) * 100;
            lookZ = cameraZ;

            var size = 5000;

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

            if (!Context.IsCurrent)
                MakeCurrent();

            GL.MatrixMode(MatrixMode.Projection);

            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float) (90 * MathHelper.deg2rad),
                (float)Width / Height, 0.1f,
                (float) 20000);
            GL.LoadMatrix(ref projection);

            /*Console.WriteLine("cam: {0} {1} {2} lookat: {3} {4} {5}", (float) cameraX, (float) cameraY, (float) cameraZ,
                (float) lookX,
                (float) lookY, (float) lookZ);
              */  
            Matrix4 modelview = Matrix4.LookAt((float) cameraX, (float) cameraY, (float) cameraZ + 100f * 0,
                (float) lookX,
                (float) lookY, (float) lookZ, 0, 0, 1);
                
            GL.MatrixMode(MatrixMode.Modelview);

            // roll
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationZ((float) (rpy.X * MathHelper.deg2rad)));
            // pitch
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationX((float) (rpy.Y * -MathHelper.deg2rad)));

            GL.LoadMatrix(ref modelview);

            GL.Viewport(0, 0, Width, Height);

            GL.ClearColor(Color.CornflowerBlue);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.AccumBufferBit);

    
            GL.Disable(EnableCap.Fog);
            GL.Enable(EnableCap.Fog);
            GL.Disable(EnableCap.Lighting);
         
            Lighting.SetupAmbient(0.1f);
            Lighting.SetDefaultMaterial(1f);
      //      Lighting.SetupLightZero(new Vector3d(cameraX, cameraY, cameraZ + 100), 0f);


            GL.Fog(FogParameter.FogColor, new float[] {100 / 255.0f, 149 / 255.0f, 237 / 255.0f, 1f});
            GL.Fog(FogParameter.FogDensity,0.1f);
            GL.Fog(FogParameter.FogMode, (int) FogMode.Linear);
            GL.Fog(FogParameter.FogStart, (float) 300);
            GL.Fog(FogParameter.FogEnd, (float) 2000);

            GL.Disable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Always);

            var texlist = textureid.ToArray().ToSortedList((a, b) => { return a.Value.zoom.CompareTo(b.Value.zoom); });

            int textureload = 0;

            foreach (var tidict in texlist)
            {
                if (!tidict.Value.textureReady)
                {
                    if (textureload < 1)
                    {
                        textureload++;
                    }
                    else
                    {
                        continue;
                    }
                }

                long xr = tidict.Key.X * prj.TileSize.Width;
                long yr = tidict.Key.Y * prj.TileSize.Width;

                long x2 = (tidict.Key.X + 1) * prj.TileSize.Width;
                long y2 = (tidict.Key.Y + 1) * prj.TileSize.Width;

                GL.Clear(ClearBufferMask.DepthBufferBit);

                GL.Enable(EnableCap.DepthTest);

                if (tidict.Value.texture.Count != 0)
                    tidict.Value.Draw();

                GL.Disable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                {
                    if (GCSViews.FlightPlanner.instance.pointlist.Count > 1)
                    {
                        GL.Color3(Color.Red);

                        GL.LineWidth(3);

                        // render wps
                        GL.Begin(PrimitiveType.LineStrip);

                        foreach (var point in GCSViews.FlightPlanner.instance.pointlist)
                        {
                            if (point == null)
                                continue;
                            var co = convertCoords(point);
                            GL.Vertex3(co[0], co[1], co[2]);
                        }

                        GL.End();
                    }

                    if (green == 0)
                    {
                        green = generateTexture(GMap.NET.Drawing.Properties.Resources.green.ToBitmap());
                    }

                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(false);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, green);
                    var list = GCSViews.FlightPlanner.instance.pointlist.ToList();
                    if (MainV2.comPort.MAV.cs.TargetLocation != PointLatLngAlt.Zero)
                        list.Add(MainV2.comPort.MAV.cs.TargetLocation);
                    foreach (var point in list)
                    {
                        if (point == null)
                            continue;
                        var co = convertCoords(point);
                        GL.Begin(PrimitiveType.TriangleStrip);

                        GL.Color3(Color.Red); //tr
                        GL.TexCoord2(0, 0);
                        GL.Vertex3(Math.Sin(MathHelper.Radians(rpy.Z+90)) * 2 + co[0], Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2 + co[1] , co[2] + 10);
                        GL.Color3(Color.Green); //tl
                        GL.TexCoord2(1, 0);
                        GL.Vertex3( co[0] - Math.Sin(MathHelper.Radians(rpy.Z + 90))*2,  co[1] - Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2, co[2] + 10);
                        GL.Color3(Color.Blue); // br
                        GL.TexCoord2(0, 1);
                        GL.Vertex3(co[0] + Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2, co[1] + Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2, co[2] - 1);
                        GL.Color3(Color.Yellow); // bl
                        GL.TexCoord2(1, 1);
                        GL.Vertex3(co[0] - Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2, co[1] - Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2, co[2] - 1);

                        GL.End();
                    }
                    GL.Disable(EnableCap.Blend);
                    GL.DepthMask(true);                


                    /*
                    WPs.ForEach(a =>
                    {
                        var co = convertCoords(new PointLatLngAlt(a.lat, a.lng, a.alt));
                        GL.Vertex3(co[0], co[1], co[2]);
                    });*/

                }
            }

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
            Console.Write("OpenGLTest2 {0}\r", delta.TotalMilliseconds);
        }

        private void generateTextures()
        {
            core.fillEmptyTiles = false;

            core.LevelsKeepInMemmory = 10;

            core.Provider = type;

            //core.ReloadMap();

            List<tileZoomArea> tileArea = new List<tileZoomArea>();
            //if (center.GetDistance(oldcenter) > 30)
            {
                oldcenter = new PointLatLngAlt(center);
                zoom = 18;

                for (int a = 12; a <= zoom; a++)
                {
                    var area2 = new RectLatLng(center.Lat, center.Lng, 0, 0);

                    // 200m at max zoom
                    // step at 0 zoom
                   var distm = MathHelper.map(a, 0, zoom, 3000, 10);

                    //Console.WriteLine("tiles z {0} max {1} dist {2}", a, zoom, distm);

                    var offset = center.newpos(rpy.Z, distm);

                    area2.Inflate(Math.Abs(center.Lat - offset.Lat), Math.Abs(center.Lng - offset.Lng));

                    var extratile = 0;

                    if (a == zoom)
                        extratile = 1;

                    var tiles = new tileZoomArea()
                    {
                        zoom = a,
                        points = prj.GetAreaTileList(area2, a, extratile),
                        area = area2
                    };

                    tileArea.Add(tiles);

                }
            }

            var totaltiles = tileArea.Sum(a => a.points.Count);

            Console.WriteLine(DateTime.Now.Millisecond + " Total tiles " + totaltiles);

            textureid.Where(a => !tileArea.Any(b => b.points.Contains(a.Key))).ForEach(c =>
             {
                 this.BeginInvoke((MethodInvoker)delegate
                 {
                     Console.WriteLine(DateTime.Now.Millisecond + " tile cleanup");
                     tileInfo temp;
                     textureid.TryRemove(c.Key, out temp);
                     temp?.Cleanup();
                 });
             });
            
            //https://wiki.openstreetmap.org/wiki/Zoom_levels
            var C = 2 * Math.PI * 6378137.000;
            // horizontal distance by each tile square
            var stile = C * Math.Cos(center.Lat) / Math.Pow(2, zoom);

            var pxstep = 2;     

            // get tiles & combine into one
            foreach (var tilearea in tileArea)
            {
                stile = C * Math.Cos(center.Lat) / Math.Pow(2, tilearea.zoom);

                if (tilearea.zoom == 20)
                    pxstep = 256;
                if (tilearea.zoom == 19)
                    pxstep = 128;
                if (tilearea.zoom == 18)
                    pxstep = 64;
                if (tilearea.zoom == 17)
                    pxstep = 32;
                if (tilearea.zoom == 16)
                    pxstep = 16;
                if (tilearea.zoom == 15)
                    pxstep = 8;
                if (tilearea.zoom == 14)
                    pxstep = 4;
                if (tilearea.zoom == 13)
                    pxstep = 2;
                if (tilearea.zoom == 12)
                    pxstep = 1;

                foreach (var p in tilearea.points)
                {
                    core.tileDrawingListLock.AcquireReaderLock();
                    core.Matrix.EnterReadLock();

                    long xr = p.X * prj.TileSize.Width;
                    long yr = p.Y * prj.TileSize.Width;

                    long x2 = (p.X + 1) * prj.TileSize.Width;
                    long y2 = (p.Y + 1) * prj.TileSize.Width;

                    try
                    {
                        GMap.NET.Internals.Tile t = core.Matrix.GetTileWithNoLock(tilearea.zoom, p);

                        if (t.NotEmpty)
                        {
                            foreach (GMapImage img in t.Overlays)
                            {
                                if (!textureid.ContainsKey(p))
                                {
                                    try
                                    {
                                        var ti = new tileInfo()
                                        {
                                            point = p,
                                            zoom = tilearea.zoom,
                                            img = (Image)img.Img.Clone()
                                        };

                                        for (long x = xr; x < x2; x += pxstep)
                                        {
                                            long xnext = x + pxstep;
                                            for (long y = yr; y < y2; y += pxstep)
                                            {
                                                long ynext = y + pxstep;

                                                var latlng1 = prj.FromPixelToLatLng(x, y, tilearea.zoom);
                                                if (srtm.getAltitude(latlng1.Lat, latlng1.Lng).currenttype == srtm.tiletype.invalid)
                                                {
                                                    ti = null;
                                                    x = x2;
                                                    y = y2;
                                                    break;
                                                }
                                               
                                                var utm1 = convertCoords(latlng1);
                                                utm1[2] = srtm.getAltitude(latlng1.Lat, latlng1.Lng).alt;

                                                var imgx = MathHelper.map(x, xr, x2, 0, 1);
                                                var imgy = MathHelper.map(y, yr, y2, 0, 1);

                                                ti.texture.Add(new tileInfo.TextureCoords((float)imgx, (float)imgy));
                                                ti.vertex.Add(new tileInfo.Vertex((float)utm1[0], (float)utm1[1],
                                                    (float)utm1[2]));

                                                //
                                                var latlng2 = prj.FromPixelToLatLng(x, ynext, tilearea.zoom);
                                                var utm2 = convertCoords(latlng2);
                                                utm2[2] = srtm.getAltitude(latlng2.Lat, latlng2.Lng).alt;

                                                imgx = MathHelper.map(x, xr, x2, 0, 1);
                                                imgy = MathHelper.map(ynext, yr, y2, 0, 1);

                                                ti.texture.Add(new tileInfo.TextureCoords((float)imgx, (float)imgy));
                                                ti.vertex.Add(new tileInfo.Vertex((float)utm2[0], (float)utm2[1],
                                                    (float)utm2[2]));

                                                //
                                                latlng2 = prj.FromPixelToLatLng(xnext, y, tilearea.zoom);
                                                utm2 = convertCoords(latlng2);
                                                utm2[2] = srtm.getAltitude(latlng2.Lat, latlng2.Lng).alt;

                                                imgx = MathHelper.map(xnext, xr, x2, 0, 1);
                                                imgy = MathHelper.map(y, yr, y2, 0, 1);

                                                ti.texture.Add(new tileInfo.TextureCoords((float)imgx, (float)imgy));
                                                ti.vertex.Add(new tileInfo.Vertex((float)utm2[0], (float)utm2[1],
                                                    (float)utm2[2]));

                                                //
                                                latlng2 = prj.FromPixelToLatLng(xnext, ynext, tilearea.zoom);
                                                utm2 = convertCoords(latlng2);
                                                utm2[2] = srtm.getAltitude(latlng2.Lat, latlng2.Lng).alt;

                                                imgx = MathHelper.map(xnext, xr, x2, 0, 1);
                                                imgy = MathHelper.map(ynext, yr, y2, 0, 1);

                                                ti.texture.Add(new tileInfo.TextureCoords((float)imgx, (float)imgy));
                                                ti.vertex.Add(new tileInfo.Vertex((float)utm2[0], (float)utm2[1],
                                                    (float)utm2[2]));
                                            }
                                        }

                                        if(ti != null)
                                            textureid[p] = ti;
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        core.Matrix.LeaveReadLock();
                        core.tileDrawingListLock.ReleaseReaderLock();
                    }

                }
            }
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
// 
// OpenGLtest
// 
            this.Width = 640;
            this.Height = 480;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "OpenGLtest";
            this.Load += new System.EventHandler(this.test_Load);
            this.Resize += new System.EventHandler(this.test_Resize);
            this.ResumeLayout(false);
        }

        private void test_Load(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
 
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
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

        public class tileInfo: IDisposable
        {
            private Image _img = null;
            private BitmapData _data = null;
            public Image img
            {
                get { return _img; }
                set
                {
                    _img = value;
                    _data = ((Bitmap)_img).LockBits(new System.Drawing.Rectangle(0, 0, _img.Width, _img.Height),
                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                }
            }
            public int zoom { get; set; }
            public GPoint point { get; set; }

            public bool textureReady
            {
                get { return _textid != 0; }
            }

            private int _textid = 0;
            public int idtexture {
                get
                {
                    if (_textid == 0)
                    {
                        try
                        {
                            _textid = generateTexture(_data);
                        }
                        catch
                        {
                        }
                    }

                    return _textid;
                }
            }

            private int ID_VBO = 0;

            private int ID_TBO = 0;

            private int ID_EBO = 0;

            public int idVBO
            {
                get
                {
                    if (ID_VBO != 0)
                        return ID_VBO;
                    
                    GL.GenBuffers(1, out ID_VBO);
                    if (ID_VBO == 0)
                        return ID_VBO;
                    GL.BindBuffer(BufferTarget.ArrayBuffer, ID_VBO);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (vertex.Count * Vertex.Stride), vertex.ToArray(),
                        BufferUsageHint.StaticDraw);
                    long bufferSize;
                    // Validate that the buffer is the correct size
                    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                    if (vertex.Count * Vector3.SizeInBytes != bufferSize)
                        throw new ApplicationException("Vertex array not uploaded correctly");

                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                    return ID_VBO;
                }
            }
            public int idTBO
            {
                get
                {
                    if (ID_TBO != 0)
                        return ID_TBO;

                    GL.GenBuffers(1, out ID_TBO);
                    if (ID_TBO == 0)
                        return ID_TBO;
                    GL.BindBuffer(BufferTarget.ArrayBuffer, ID_TBO);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(texture.Count * TextureCoords.Stride), texture.ToArray(),
                        BufferUsageHint.StaticDraw);
                    long bufferSize;
                    // Validate that the buffer is the correct size
                    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                    if (texture.Count * 8 != bufferSize)
                        throw new ApplicationException("TexCoord array not uploaded correctly");
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                    return ID_TBO;
                }
            }

            public int idEBO
            {
                get
                {
                    if (ID_EBO != 0)
                        return ID_EBO;

                    GL.GenBuffers(1, out ID_EBO);
                    if (ID_EBO == 0)
                        return ID_EBO;
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID_EBO);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Count * sizeof(int)), indices.ToArray(),
                        BufferUsageHint.StaticDraw);
                    long bufferSize;
                    // Validate that the buffer is the correct size
                    GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                    if (indices.Count * sizeof(int) != bufferSize)
                        throw new ApplicationException("Element array not uploaded correctly");
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                    return ID_EBO;
                }
            }

            List<TextureCoords> _texture = new List<TextureCoords>();
            List<Vertex> _vertex = new List<Vertex>();
            public List<TextureCoords> texture
            {
                get { return _texture; }
            }
            public List<Vertex> vertex
            {
                get { return _vertex; }
            }

            public List<int> indices
            {
                get { return Enumerable.Range(0, vertex.Count).ToList(); }
            }

            public override string ToString()
            {
                return String.Format("{1}-{0} {2}",point, zoom, textureReady);
            }

            public void Draw()
            {
                if (idVBO == 0 || idTBO == 0 || idEBO == 0)
                    return;

                // Push current Array Buffer state so we can restore it later
                GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, idtexture);

                if (GL.IsEnabled(EnableCap.Lighting))
                {
                    GL.DisableClientState(ArrayCap.NormalArray);
                }

                // Texture Array Buffer
                    if (GL.IsEnabled(EnableCap.Texture2D))
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, idTBO);
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, TextureCoords.Stride, IntPtr.Zero);
                    GL.EnableClientState(ArrayCap.TextureCoordArray);
                }

                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, idVBO);
                    GL.VertexPointer(3, VertexPointerType.Float, Vertex.Stride, IntPtr.Zero);
                    GL.EnableClientState(ArrayCap.VertexArray);
                }

                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, idEBO);
                    GL.DrawElements(PrimitiveType.TriangleStrip, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }

                // Restore the state
                GL.PopClientAttrib();
            }

            public void Cleanup()
            {
                GL.DeleteTextures(1, ref _textid);

                GL.DeleteBuffers(1, ref ID_VBO);
                GL.DeleteBuffers(1, ref ID_EBO);
                GL.DeleteBuffers(1, ref ID_TBO);

                try
                {
                    img.Dispose();
                } catch { }
            }

            public void Dispose()
            {
                Cleanup();
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Vertex
            {
                public float X, Y, Z;

                public Vertex(float x, float y, float z)
                {
                    X = x;
                    Y = y;
                    Z = z;
                }

                public static readonly int Stride = System.Runtime.InteropServices.Marshal.SizeOf(new Vertex());
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct TextureCoords
            {
                
                public float X, Y;

                public TextureCoords(float x, float y)
                {
                    X = x;
                    Y = y;
                }

                public static readonly int Stride = System.Runtime.InteropServices.Marshal.SizeOf(new TextureCoords());
            }
        }

    }

    /// <summary>

    /// A helper class with OpenGL lighting utility methods.

    /// </summary>

    public static class Lighting

    {

        /// <summary>

        /// Sets up ambient light.

        /// </summary>

        public static void SetupAmbient(float ambient)

        {

            float[] ambient_light = { ambient, ambient, ambient, 1 };

            GL.LightModel(LightModelParameter.LightModelAmbient, ambient_light);

        }



        /// <summary>

        /// Sets up and enables a light and a given position.

        /// </summary>

        public static void SetupLightZero(Vector3d position, float ambient)

        {

            float[] ambient_light = { ambient, ambient, ambient, 1.0f };

            float[] spec = { 0.5f, 0.5f, 0.5f, 1.0f };

            float[] one = { 1.0f, 1.0f, 1.0f, 1.0f };



            GL.Light(LightName.Light0, LightParameter.Position, new float[] { (float)position.X, (float)position.Y, (float)position.Z });

            GL.Light(LightName.Light0, LightParameter.Ambient, ambient_light);

            GL.Light(LightName.Light0, LightParameter.Diffuse, one);

            GL.Light(LightName.Light0, LightParameter.Specular, spec);

            //GL.Light( LightName.Light0, LightParameter.SpotExponent, 100 );	// For directional lights.



            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);

            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);    // Needed for specular

            GL.LightModel(LightModelParameter.LightModelColorControl, 0x81FA);

            GL.Enable(EnableCap.Light0);

        }



        public static void SetDefaultMaterial(float ambient)

        {

            float[] ambient_light = { ambient, ambient, ambient, 1.0f };

            float[] one = { 1.0f, 1.0f, 1.0f, 1.0f };

            float[] zero = { 0.0f, 0.0f, 0.0f, 1.0f };



            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, ambient_light);

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, one);

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, one);

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0.1f, 0.1f, 0.1f, 1.0f });

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 128);

        }

    }
}