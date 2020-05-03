using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using Microsoft.Scripting.Utils;
using MissionPlanner.GCSViews;
using MissionPlanner.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MathHelper = MissionPlanner.Utilities.MathHelper;
using Vector3 = OpenTK.Vector3;

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
        public int zoom { get; set; } = 20;

        RectLatLng area = new RectLatLng(-35.04286, 117.84262, 0.1, 0.1);
        private NumericUpDown num_minzoom;
        private NumericUpDown num_maxzoom;

        private PointLatLngAlt _center { get; set; } = new PointLatLngAlt(-34.9807459, 117.8514028, 70);

        public PointLatLngAlt LocationCenter
        {
            get { return _center; }
            set
            {
                if (value.Lat == 0 && value.Lng == 0)
                    return;

                if (_center.Lat == value.Lat && _center.Lng == value.Lng)
                    return;

                _center.Lat = value.Lat;
                _center.Lng = value.Lng;
                _center.Alt = value.Alt;

                if (utmzone != value.GetUTMZone())
                {
                    utmzone = value.GetUTMZone();

                    // set our pos
                    llacenter = value;
                    utmcenter = new double[] { 0, 0 };
                    // update a virtual center bases on llacenter
                    utmcenter = convertCoords(value);

                    textureid.ForEach(a => a.Value.Cleanup());

                    textureid.Clear();
                }

                this.Invalidate();
            }
        }

        Vector3 _rpy = new Vector3();

        public Vector3 rpy
        {
            get { return _rpy; }
            set
            {
                _rpy.X = (float)Math.Round(value.X, 2);
                _rpy.Y = (float)Math.Round(value.Y, 2);
                _rpy.Z = (float)Math.Round(value.Z, 2);
                this.Invalidate();
            }
        }

        public List<Locationwp> WPs { get; set; }

        public OpenGLtest2()
        {
            instance = this;

            InitializeComponent();

            Click += OnClick;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;

            core.OnMapOpen();

            type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            prj = type.Projection;

            this.Invalidate();

            bg = new Thread(imageLoader) { IsBackground = true };
            bg.Start();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            var x = ((MouseEventArgs)e).X;
            var y = ((MouseEventArgs)e).Y;

            mouseDownPos = getMousePos(x, y);

            MainV2.comPort.setGuidedModeWP(
                new Locationwp().Set(mouseDownPos.Lat, mouseDownPos.Lng, MainV2.comPort.MAV.GuidedMode.z,
                    (ushort)MAVLink.MAV_CMD.WAYPOINT), false);
            //MainV2.C
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var x = ((MouseEventArgs)e).X;
            var y = ((MouseEventArgs)e).Y;
            /*
            var point = getMousePos(x, y);

            
            GL.Color3(Color.White);

            GL.Begin(BeginMode.Lines);
            //GL.Vertex3(_start.X, _start.Y, _start.Z);
            GL.Vertex3(0, 0, 0);
            //GL.Vertex3(_end.X, _end.Y, _end.Z);

            if (Math.Abs(point.Lat) > 90 || Math.Abs(point.Lng) > 180)
                return;

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

            if (!Context.IsCurrent)
                MakeCurrent();

            GL.GetFloat(GetPName.ModelviewMatrix, out modelMatrix);
            GL.GetFloat(GetPName.ProjectionMatrix, out projMatrix);
            GL.GetInteger(GetPName.Viewport, viewport);

            var _start = UnProject(new Vector3(x, y, 0.0f), projMatrix, modelMatrix,
                new Size(viewport[2], viewport[3]));
            var _end = UnProject(new Vector3(x, y, 1), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));

            var pos = new utmpos(utmcenter[0] + _end.X, utmcenter[1] + _end.Y, utmzone);

            var plla = pos.ToLLA();
            plla.Alt = _end.Z;

            var point = srtm.getIntersectionWithTerrain(_center, plla);

            return point;
        }

        private void OnClick(object sender, EventArgs e)
        {
            //utmzone = 0;
            //this.LocationCenter = LocationCenter.newpos(0, 0.001);
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
                }
                catch
                {
                }
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
                catch
                {
                }
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

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode,
                (float)TextureEnvModeCombine.Replace); //Important, or wrong color on some computers

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texture;
        }

        void imageLoader()
        {
            core.Zoom = minzoom;

            while (!this.IsDisposed)
            {
                System.Threading.Thread.Sleep(100);

                if (core.tileLoadQueue.Count > 0)
                {
                    System.Threading.Thread.Sleep(500);
                    continue;
                }

                if (core.Zoom >= zoom)
                    core.Zoom = minzoom;

                core.Zoom = core.Zoom + 1;

                generateTextures();
            }
        }

        public int minzoom { get; set; } = 10;

        private int utmzone = -999;
        private double[] utmcenter = new double[2];
        private PointLatLngAlt llacenter = PointLatLngAlt.Zero;
        private PointLatLngAlt mouseDownPos;
        private Thread bg;

        double[] convertCoords(PointLatLngAlt plla)
        {
            /*
            var latMid = llacenter.Lat;
            var lngMid = llacenter.Lng;

            var m_per_deg_lat = 111132.954 - 559.822 * Math.Cos(2 * latMid) + 1.175 * Math.Cos(4 * latMid);
            var m_per_deg_lon = 111132.954 * Math.Cos(latMid);

            var deltaLat = (latMid - plla.Lat);
            var deltaLon = (lngMid - plla.Lng);

            return new[] { deltaLat * m_per_deg_lat, deltaLon * m_per_deg_lon,  plla.Alt};
            */

            var utm = plla.ToUTM(utmzone);

            Array.Resize(ref utm, 3);

            utm[0] -= utmcenter[0];
            utm[1] -= utmcenter[1];

            utm[2] = plla.Alt;

            return new[] { utm[0], utm[1], utm[2] };
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

            if (DateTime.Now.Second % 3 == 0)
                core.Position = _center;

            double heightscale = 1; //(step/90.0)*5;

            var campos = convertCoords(_center);

            cameraX = campos[0];
            cameraY = campos[1];
            cameraZ = (campos[2] < srtm.getAltitude(_center.Lat, _center.Lng).alt)
                ? (srtm.getAltitude(_center.Lat, _center.Lng).alt + 1) * heightscale
                : _center.Alt * heightscale; // (srtm.getAltitude(lookZ, lookX, 20) + 100) * heighscale;

            lookX = campos[0] + Math.Sin(MathHelper.Radians(rpy.Z)) * 100;
            lookY = campos[1] + Math.Cos(MathHelper.Radians(rpy.Z)) * 100;
            lookZ = cameraZ;

            var size = 5000;

            // in front
            PointLatLngAlt front = _center.newpos(rpy.Z, size);
            // behind
            PointLatLngAlt behind = _center.newpos(rpy.Z, -50);
            // left : 90 allows for 180 degree viewing angle
            PointLatLngAlt left = _center.newpos(rpy.Z - 45, size);
            // right
            PointLatLngAlt right = _center.newpos(rpy.Z + 45, size);

            double maxlat = Math.Max(left.Lat, Math.Max(right.Lat, Math.Max(front.Lat, behind.Lat)));
            double minlat = Math.Min(left.Lat, Math.Min(right.Lat, Math.Min(front.Lat, behind.Lat)));

            double maxlng = Math.Max(left.Lng, Math.Max(right.Lng, Math.Max(front.Lng, behind.Lng)));
            double minlng = Math.Min(left.Lng, Math.Min(right.Lng, Math.Min(front.Lng, behind.Lng)));

            area = RectLatLng.FromLTRB(minlng, maxlat, maxlng, minlat);

            if (!Context.IsCurrent)
                MakeCurrent();


            /*Console.WriteLine("cam: {0} {1} {2} lookat: {3} {4} {5}", (float) cameraX, (float) cameraY, (float) cameraZ,
                (float) lookX,
                (float) lookY, (float) lookZ);
              */
            Matrix4 modelview = Matrix4.LookAt((float)cameraX, (float)cameraY, (float)cameraZ + 100f * 0,
                (float)lookX, (float)lookY, (float)lookZ,
                0, 0, 1);

            GL.MatrixMode(MatrixMode.Modelview);

            // roll
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationZ((float)(rpy.X * MathHelper.deg2rad)));
            // pitch
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationX((float)(rpy.Y * -MathHelper.deg2rad)));

            GL.LoadMatrix(ref modelview);

            //GL.Viewport(0, 0, Width, Height);

            GL.ClearColor(Color.CornflowerBlue);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.AccumBufferBit);

            GL.Enable(EnableCap.Fog);
            GL.Disable(EnableCap.Lighting);

            Lighting.SetupAmbient(0.1f);
            Lighting.SetDefaultMaterial(1f);
            //      Lighting.SetupLightZero(new Vector3d(cameraX, cameraY, cameraZ + 100), 0f);


            GL.Fog(FogParameter.FogColor, new float[] { 100 / 255.0f, 149 / 255.0f, 237 / 255.0f, 1f });
            GL.Fog(FogParameter.FogDensity, 0.1f);
            GL.Fog(FogParameter.FogMode, (int)FogMode.Linear);
            GL.Fog(FogParameter.FogStart, (float)300);
            GL.Fog(FogParameter.FogEnd, (float)2000);

            GL.Disable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Always);

            var texlist = textureid.ToArray().ToSortedList(Comparison);

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

                GL.Clear(ClearBufferMask.DepthBufferBit);

                GL.Enable(EnableCap.DepthTest);

                if (tidict.Value.indices.Count > 0)
                    tidict.Value.Draw();



                GL.Disable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);

            }

            // draw after terrain - need depth check
            {
                GL.Enable(EnableCap.DepthTest);

                if (FlightPlanner.instance.pointlist.Count > 1)
                {
                    GL.Color3(Color.Red);

                    GL.LineWidth(3);

                    // render wps
                    GL.Begin(PrimitiveType.LineStrip);

                    foreach (var point in FlightPlanner.instance.pointlist)
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
                var list = FlightPlanner.instance.pointlist.ToList();
                if (MainV2.comPort.MAV.cs.mode.ToLower() == "guided")
                    list.Add((PointLatLngAlt)(Locationwp)MainV2.comPort.MAV.GuidedMode);
                if (MainV2.comPort.MAV.cs.TargetLocation != PointLatLngAlt.Zero)
                    list.Add(MainV2.comPort.MAV.cs.TargetLocation);
                foreach (var point in list)
                {
                    if (point == null)
                        continue;
                    if (point.Lat == 0 && point.Lng == 0)
                        continue;
                    var co = convertCoords(point);
                    GL.Begin(PrimitiveType.TriangleStrip);

                    GL.Color3(Color.Red); //tr
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2 + co[0],
                        Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2 + co[1], co[2] + 10);
                    GL.Color3(Color.Green); //tl
                    GL.TexCoord2(1, 0);
                    GL.Vertex3(co[0] - Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2,
                        co[1] - Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2, co[2] + 10);
                    GL.Color3(Color.Blue); // br
                    GL.TexCoord2(0, 1);
                    GL.Vertex3(co[0] + Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2,
                        co[1] + Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2, co[2] - 1);
                    GL.Color3(Color.Yellow); // bl
                    GL.TexCoord2(1, 1);
                    GL.Vertex3(co[0] - Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2,
                        co[1] - Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2, co[2] - 1);

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

        private int Comparison(KeyValuePair<GPoint, tileInfo> x, KeyValuePair<GPoint, tileInfo> y)
        {
            return x.Value.zoom.CompareTo(y.Value.zoom);
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

                for (int a = minzoom; a <= zoom; a++)
                {
                    var area2 = new RectLatLng(_center.Lat, _center.Lng, 0, 0);

                    // 200m at max zoom
                    // step at 0 zoom
                    var distm = MathHelper.map(a, 0, zoom, 3000, 10);

                    //Console.WriteLine("tiles z {0} max {1} dist {2}", a, zoom, distm);

                    var offset = _center.newpos(rpy.Z, distm);

                    area2.Inflate(Math.Abs(_center.Lat - offset.Lat), Math.Abs(_center.Lng - offset.Lng));

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

            var totaltiles = 0;
            foreach (var a in tileArea) totaltiles += a.points.Count;

            Console.WriteLine(DateTime.Now.Millisecond + " Total tiles " + totaltiles);


            if (DateTime.Now.Second % 3 == 1)
                CleanupOldTextures(tileArea);

            //https://wiki.openstreetmap.org/wiki/Zoom_levels
            var C = 2 * Math.PI * 6378137.000;
            // horizontal distance by each tile square
            var stile = C * Math.Cos(_center.Lat) / Math.Pow(2, zoom);

            var pxstep = 2;

            // get tiles & combine into one
            foreach (var tilearea in tileArea)
            {
                stile = C * Math.Cos(_center.Lat) / Math.Pow(2, tilearea.zoom);

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

                    long xstart = p.X * prj.TileSize.Width;
                    long ystart = p.Y * prj.TileSize.Width;

                    long xend = (p.X + 1) * prj.TileSize.Width;
                    long yend = (p.Y + 1) * prj.TileSize.Width;

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

                                        for (long x = xstart; x < xend; x += pxstep)
                                        {
                                            long xnext = x + pxstep;
                                            for (long y = ystart; y < yend; y += pxstep)
                                            {
                                                long ynext = y + pxstep;

                                                var latlng1 = prj.FromPixelToLatLng(x, y, tilearea.zoom); //bl
                                                var latlng2 = prj.FromPixelToLatLng(x, ynext, tilearea.zoom); //tl
                                                var latlng3 = prj.FromPixelToLatLng(xnext, y, tilearea.zoom); // br
                                                var latlng4 = prj.FromPixelToLatLng(xnext, ynext, tilearea.zoom); // tr

                                                if (srtm.getAltitude(latlng1.Lat, latlng1.Lng).currenttype ==
                                                    srtm.tiletype.invalid)
                                                {
                                                    ti = null;
                                                    x = xend;
                                                    y = yend;
                                                    break;
                                                }


                                                AddQuad(ti, latlng1, latlng2, latlng3, latlng4, xstart, x, xnext, xend,
                                                    ystart, y, ynext, yend);
                                            }
                                        }

                                        if (ti != null)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ti">tile</param>
        /// <param name="latlng1">bl</param>
        /// <param name="latlng2">tl</param>
        /// <param name="latlng3">br</param>
        /// <param name="latlng4">tr</param>
        /// <param name="xstart"></param>
        /// <param name="x"></param>
        /// <param name="xnext"></param>
        /// <param name="xend"></param>
        /// <param name="ystart"></param>
        /// <param name="y"></param>
        /// <param name="ynext"></param>
        /// <param name="yend"></param>
        private void AddQuad(tileInfo ti, PointLatLng latlng1, PointLatLng latlng2, PointLatLng latlng3,
            PointLatLng latlng4, long xstart, long x,
            long xnext, long xend, long ystart, long y, long ynext, long yend)
        {
            var utm1 = convertCoords(latlng1);
            utm1[2] = srtm.getAltitude(latlng1.Lat, latlng1.Lng).alt;

            //var imgx = MathHelper.map(x, xstart, xend, 0, 1);
            //var imgy = MathHelper.map(y, ystart, yend, 0, 1);

            //ti.texture.Add(new tileInfo.TextureCoords((float) imgx, (float) imgy));
            //ti.vertex.Add(new tileInfo.Vertex((float) utm1[0], (float) utm1[1],(float) utm1[2]));

            //
            var utm2 = convertCoords(latlng2);
            utm2[2] = srtm.getAltitude(latlng2.Lat, latlng2.Lng).alt;

            //imgx = MathHelper.map(x, xstart, xend, 0, 1);
            //imgy = MathHelper.map(ynext, ystart, yend, 0, 1);

            //ti.texture.Add(new tileInfo.TextureCoords((float) imgx, (float) imgy));
            //ti.vertex.Add(new tileInfo.Vertex((float) utm2[0], (float) utm2[1],(float) utm2[2]));

            //
            var utm3 = convertCoords(latlng3);
            utm3[2] = srtm.getAltitude(latlng3.Lat, latlng3.Lng).alt;

            //imgx = MathHelper.map(xnext, xstart, xend, 0, 1);
            //imgy = MathHelper.map(y, ystart, yend, 0, 1);

            //ti.texture.Add(new tileInfo.TextureCoords((float) imgx, (float) imgy));
            //ti.vertex.Add(new tileInfo.Vertex((float) utm3[0], (float) utm3[1],(float) utm3[2]));

            var utm4 = convertCoords(latlng4);
            utm4[2] = srtm.getAltitude(latlng4.Lat, latlng4.Lng).alt;

            //imgx = MathHelper.map(xnext, xstart, xend, 0, 1);
            //imgy = MathHelper.map(ynext, ystart, yend, 0, 1);

            //ti.texture.Add(new tileInfo.TextureCoords((float)imgx, (float)imgy));
            //ti.vertex.Add(new tileInfo.Vertex((float)utm4[0], (float)utm4[1],(float)utm4[2]));


            var imgx = MathHelper.map(xnext, xstart, xend, 0, 1);
            var imgy = MathHelper.map(ynext, ystart, yend, 0, 1);
            ti.vertex.Add(new tileInfo.Vertex(utm4[0], utm4[1], utm4[2], 1, 0, 0, imgx, imgy));

            imgx = MathHelper.map(xnext, xstart, xend, 0, 1);
            imgy = MathHelper.map(y, ystart, yend, 0, 1);
            ti.vertex.Add(new tileInfo.Vertex(utm3[0], utm3[1], utm3[2], 0, 1, 0, imgx, imgy));

            imgx = MathHelper.map(x, xstart, xend, 0, 1);
            imgy = MathHelper.map(y, ystart, yend, 0, 1);
            ti.vertex.Add(new tileInfo.Vertex(utm1[0], utm1[1], utm1[2], 0, 0, 1, imgx, imgy));

            imgx = MathHelper.map(x, xstart, xend, 0, 1);
            imgy = MathHelper.map(ynext, ystart, yend, 0, 1);
            ti.vertex.Add(new tileInfo.Vertex(utm2[0], utm2[1], utm2[2], 1, 1, 0, imgx, imgy));

            var startindex = (uint)ti.vertex.Count - 4;

            ti.indices.AddRange(new[]
            {
                startindex + 0, startindex + 1, startindex + 3,
                startindex + 1, startindex + 2, startindex + 3
            });
        }

        private void CleanupOldTextures(List<tileZoomArea> tileArea)
        {
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
        }

        private void InitializeComponent()
        {
            this.num_minzoom = new System.Windows.Forms.NumericUpDown();
            this.num_maxzoom = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.num_minzoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxzoom)).BeginInit();
            this.SuspendLayout();
            // 
            // num_minzoom
            // 
            this.num_minzoom.Location = new System.Drawing.Point(3, 3);
            this.num_minzoom.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.num_minzoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_minzoom.Name = "num_minzoom";
            this.num_minzoom.Size = new System.Drawing.Size(54, 20);
            this.num_minzoom.TabIndex = 0;
            this.num_minzoom.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            this.num_minzoom.ValueChanged += new System.EventHandler(this.num_minzoom_ValueChanged);
            // 
            // num_maxzoom
            // 
            this.num_maxzoom.Location = new System.Drawing.Point(3, 29);
            this.num_maxzoom.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.num_maxzoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_maxzoom.Name = "num_maxzoom";
            this.num_maxzoom.Size = new System.Drawing.Size(54, 20);
            this.num_maxzoom.TabIndex = 1;
            this.num_maxzoom.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.num_maxzoom.ValueChanged += new System.EventHandler(this.num_maxzoom_ValueChanged);
            // 
            // OpenGLtest2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.num_maxzoom);
            this.Controls.Add(this.num_minzoom);
            this.Name = "OpenGLtest2";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.test_Load);
            this.Resize += new System.EventHandler(this.test_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.num_minzoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxzoom)).EndInit();
            this.ResumeLayout(false);

        }

        private void test_Load(object sender, EventArgs e)
        {
            MakeCurrent();

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

            test_Resize(null, null);
        }

        private void num_minzoom_ValueChanged(object sender, EventArgs e)
        {
            minzoom = (int)num_minzoom.Value;
        }

        private void num_maxzoom_ValueChanged(object sender, EventArgs e)
        {
            zoom = (int)num_maxzoom.Value;
        }

        private void test_Resize(object sender, EventArgs e)
        {
            MakeCurrent();

            GL.Viewport(0, 0, this.Width, this.Height);

            GL.MatrixMode(MatrixMode.Projection);

            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)(90 * MathHelper.deg2rad),
                (float)Width / Height, 0.1f,
                (float)20000);

            GL.LoadMatrix(ref projection);

            core.OnMapSizeChanged(this.Width, this.Height);

            this.Invalidate();
        }

        public class tileZoomArea
        {
            public List<GPoint> points;
            public int zoom;
            public RectLatLng area { get; set; }
        }

        public class tileInfo : IDisposable
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

            public int idtexture
            {
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

            private int ID_VAO = 0;

            private int ID_EBO = 0;

            private static int GLHandle;
            private Dictionary<string, int> _uniformLocations;
            private bool init;

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
                    GL.BufferData(BufferTarget.ArrayBuffer, (vertex.Count * Vertex.Stride), vertex.ToArray(),
                        BufferUsageHint.StaticDraw);
                    long bufferSize;
                    // Validate that the buffer is the correct size
                    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                    if (vertex.Count * Vertex.Stride != bufferSize)
                        throw new ApplicationException("Vertex array not uploaded correctly");

                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                    return ID_VBO;
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
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (indices.Count * sizeof(uint)), indices.ToArray(),
                        BufferUsageHint.StaticDraw);
                    long bufferSize;
                    // Validate that the buffer is the correct size
                    GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize,
                        out bufferSize);
                    if (indices.Count * sizeof(int) != bufferSize)
                        throw new ApplicationException("Element array not uploaded correctly");

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                    return ID_EBO;
                }
            }

            public int idVAO
            {
                get
                {
                    if (ID_VAO != 0)
                        return ID_VAO;

                    ID_VAO = GL.GenVertexArray();
                    if (ID_VAO == 0)
                        return ID_VAO;

                    // ..:: Initialization code (done once (unless your object frequently changes)) :: ..
                    // 1. bind Vertex Array Object
                    GL.BindVertexArray(ID_VAO);
                    // 2. copy our vertices array in a buffer for OpenGL to use

                    var vertexLocation = GetAttribLocation("aPosition");
                    var texCoordLocation = GetAttribLocation("aTexCoord");

                    GL.EnableVertexAttribArray(vertexLocation);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, idVBO);
                    GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                        0);
                    //GL.BindAttribLocation(shaderProgramHandle, 0, "aPosition");

                    // Color attribute
                    //GL.EnableVertexAttribArray(1);
                    //GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (3 * sizeof(float)));

                    // TexCoord attribute
                    GL.EnableVertexAttribArray(texCoordLocation);
                    GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                        (3 * sizeof(float)));
                    //GL.BindAttribLocation(shaderProgramHandle, 0, "aTexCoord");

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, idEBO);

                    GL.BindVertexArray(0);

                    return ID_VAO;
                }
            }

            public int GetAttribLocation(string attribName)
            {
                return GL.GetAttribLocation(GLHandle, attribName);
            }

            public List<Vertex> vertex { get; } = new List<Vertex>();

            public List<uint> indices { get; } = new List<uint>();

            public override string ToString()
            {
                return String.Format("{1}-{0} {2}", point, zoom, textureReady);
            }

            public void Draw()
            {
                if (!init)
                {
                    if (idVBO == 0 || idEBO == 0)
                        return;

                    if (GLHandle == 0)
                        CreateShaders();

                    //GL.UseProgram(GLHandle);

                    if (idtexture == 0)
                        return;

                    if (idVAO == 0)
                        return;

                    init = true;
                }

                {
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, idtexture);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, idVBO);
                    GL.VertexPointer(3, VertexPointerType.Float, 5 * 4, IntPtr.Zero);
                    GL.EnableClientState(ArrayCap.VertexArray);

                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 5 * 4, 3 * 4);
                    GL.EnableClientState(ArrayCap.TextureCoordArray);

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, idEBO);
                    GL.DrawElements(PrimitiveType.TriangleStrip, indices.Count, DrawElementsType.UnsignedInt,
                        IntPtr.Zero);

                }

                // Restore the state
                //GL.PopClientAttrib();
            }

            private void CreateShaders()
            {
                VertexShader = GL.CreateShader(ShaderType.VertexShader);
                FragmentShader = GL.CreateShader(ShaderType.FragmentShader);

                GL.ShaderSource(VertexShader, @"
#version 330 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
out vec2 texCoord;
void main()
{
    texCoord = aTexCoord;
    gl_Position = vec4(aPosition, 1.0);
}
                ");

                GL.ShaderSource(FragmentShader, @"
#version 330
out vec4 outputColor;
in vec2 texCoord;
uniform sampler2D texture0;
void main()
{
    outputColor = texture(texture0, texCoord);
}
                ");

                GL.CompileShader(VertexShader);
                Debug.WriteLine(GL.GetShaderInfoLog(VertexShader));
                GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out var code);
                if (code != (int)All.True)
                {
                    // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                    throw new Exception(
                        $"Error occurred whilst compiling Shader({VertexShader}) {GL.GetShaderInfoLog(VertexShader)}");
                }

                GL.CompileShader(FragmentShader);
                Debug.WriteLine(GL.GetShaderInfoLog(FragmentShader));
                GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out code);
                if (code != (int)All.True)
                {
                    // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                    throw new Exception(
                        $"Error occurred whilst compiling Shader({FragmentShader}) {GL.GetShaderInfoLog(FragmentShader)}");
                }





                GLHandle = GL.CreateProgram();

                GL.AttachShader(GLHandle, VertexShader);
                GL.AttachShader(GLHandle, FragmentShader);

                GL.LinkProgram(GLHandle);
                Debug.WriteLine(GL.GetProgramInfoLog(GLHandle));
                GL.GetProgram(GLHandle, GetProgramParameterName.LinkStatus, out code);
                if (code != (int)All.True)
                {
                    // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                    throw new Exception(
                        $"Error occurred whilst linking Program({GLHandle}) {GL.GetProgramInfoLog(GLHandle)}");
                }



                GL.DetachShader(GLHandle, VertexShader);
                GL.DetachShader(GLHandle, FragmentShader);
                GL.DeleteShader(FragmentShader);
                GL.DeleteShader(VertexShader);

                // First, we have to get the number of active uniforms in the shader.
                GL.GetProgram(GLHandle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

                // Next, allocate the dictionary to hold the locations.
                _uniformLocations = new Dictionary<string, int>();

                // Loop over all the uniforms,
                for (var i = 0; i < numberOfUniforms; i++)
                {
                    // get the name of this uniform,
                    var key = GL.GetActiveUniform(GLHandle, i, out _, out _);

                    // get the location,
                    var location = GL.GetUniformLocation(GLHandle, key);

                    // and then add it to the dictionary.
                    _uniformLocations.Add(key, location);
                }
            }

            public int FragmentShader { get; set; }

            public int VertexShader { get; set; }

            public void Cleanup()
            {
                GL.DeleteTextures(1, ref _textid);

                GL.DeleteBuffers(1, ref ID_VBO);
                GL.DeleteBuffers(1, ref ID_EBO);
                GL.DeleteVertexArray(idVAO);
                GL.DeleteProgram(GLHandle);

                try
                {
                    img.Dispose();
                }
                catch
                {
                }
            }

            public void Dispose()
            {
                Cleanup();
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Vertex
            {
                //https://learnopengl.com/Getting-started/Textures
                public float X;
                public float Y;

                public float Z;

                //public float R;
                //public float G;
                //public float B;
                public float S;
                public float T;

                public Vertex(double x, double y, double z, double r, double g, double b, double s, double t)
                {
                    X = (float)x;
                    Y = (float)y;
                    Z = (float)z;

                    //R = (float)r;
                    //G = (float)g;
                    //B = (float)b;

                    S = (float)s;
                    T = (float)t;

                    if (S > 1 || S < 0 || T > 1 || T < 0)
                    {
                    }
                }

                public static readonly int Stride = System.Runtime.InteropServices.Marshal.SizeOf(new Vertex());
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



                GL.Light(LightName.Light0, LightParameter.Position,
                    new float[] { (float)position.X, (float)position.Y, (float)position.Z });

                GL.Light(LightName.Light0, LightParameter.Ambient, ambient_light);

                GL.Light(LightName.Light0, LightParameter.Diffuse, one);

                GL.Light(LightName.Light0, LightParameter.Specular, spec);

                //GL.Light( LightName.Light0, LightParameter.SpotExponent, 100 );	// For directional lights.



                GL.LightModel(LightModelParameter.LightModelTwoSide, 1);

                GL.LightModel(LightModelParameter.LightModelLocalViewer, 1); // Needed for specular

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

                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission,
                    new float[] { 0.1f, 0.1f, 0.1f, 1.0f });

                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 128);

            }
        }
    }
}