using GMap.NET;
using GMap.NET.Internals;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using Microsoft.Scripting.Utils;
using MissionPlanner.GCSViews;
using MissionPlanner.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Input;
using OpenTK.Platform;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MathHelper = MissionPlanner.Utilities.MathHelper;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Timer = System.Windows.Forms.Timer;
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
        private NumericUpDown num_minzoom;
        private NumericUpDown num_maxzoom;
        private SemaphoreSlim textureSemaphore = new SemaphoreSlim(1, 1);
        private CheckBox chk_locktomav;
        private Timer timer1;
        private System.ComponentModel.IContainer components;
        private CheckBox chk_fog;
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
                _centerTime = DateTime.Now;
                _center.Lat = value.Lat;
                _center.Lng = value.Lng;
                _center.Alt = value.Alt;
                if (utmzone != value.GetUTMZone() || llacenter.GetDistance(_center) > 10000)
                {
                    utmzone = value.GetUTMZone();
                    // set our pos
                    llacenter = value;
                    utmcenter = new double[] {0, 0};
                    // update a virtual center bases on llacenter
                    utmcenter = convertCoords(value);
                    textureid.ForEach(a => a.Value.Cleanup());
                    textureid.Clear();
                }

                this.Invalidate();
            }
        }

        private MissionPlanner.Utilities.Vector3 _velocity = new MissionPlanner.Utilities.Vector3();

        MissionPlanner.Utilities.Vector3 _rpy = new MissionPlanner.Utilities.Vector3();

        public MissionPlanner.Utilities.Vector3 rpy
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

        public OpenGLtest2() : base()
        {
            instance = this;
            InitializeComponent();
            Click += OnClick;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            core.OnMapOpen();
            type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            prj = type.Projection;
            LocationCenter = LocationCenter.newpos(0, 0.1);
            this.Invalidate();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            var x = ((MouseEventArgs) e).X;
            var y = ((MouseEventArgs) e).Y;
            mouseDownPos = getMousePos(x, y);
            MainV2.comPort.setGuidedModeWP(
                new Locationwp().Set(mouseDownPos.Lat, mouseDownPos.Lng, MainV2.comPort.MAV.GuidedMode.z,
                    (ushort) MAVLink.MAV_CMD.WAYPOINT), false);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            mousex = ((MouseEventArgs) e).X;
            mousey = ((MouseEventArgs) e).Y;
            try
            {
                mousePosition = getMousePos(mousex, mousey);
                /*
                if (!Context.IsCurrent)
                    Context.MakeCurrent(WindowInfo);
                GL.Color3(Color.White);
                GL.Begin(BeginMode.Lines);
                //GL.Vertex3(_start.X, _start.Y, _start.Z);
                //GL.Vertex3(_end.X, _end.Y, _end.Z);
                if (Math.Abs(point.Lat) > 90 || Math.Abs(point.Lng) > 180)
                    return;
                var utm = convertCoords(point);
                GL.Vertex3(utm[0], utm[1], point.Alt + 100);
                GL.Vertex3(utm[0], utm[1], point.Alt);
                GL.End();
                SwapBuffers();
                Context.MakeCurrent(null);
                */
            } catch { }

            //Thread.Sleep(1000);
        }

        int[] viewport = new int[4];
        Matrix4 modelMatrix = Matrix4.Identity;
        private Matrix4 projMatrix = Matrix4.Identity;

        public PointLatLngAlt getMousePos(int x, int y)
        {
            //https://gamedev.stackexchange.com/questions/103483/opentk-ray-picking
            var _start = UnProject(new Vector3(x, y, 0.0f), projMatrix, modelMatrix,
                new Size(viewport[2], viewport[3]));
            var _end = UnProject(new Vector3(x, y, 1), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
            var pos = new utmpos(utmcenter[0] + _end.X, utmcenter[1] + _end.Y, utmzone);
            var plla = pos.ToLLA();
            plla.Alt = _end.Z;
            var camera = new utmpos(utmcenter[0] + cameraX, utmcenter[1] + cameraY, utmzone).ToLLA();
            camera.Alt = cameraZ;
            var point = srtm.getIntersectionWithTerrain(camera, plla);
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
            vec.X = 2.0f * mouse.X / (float) viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float) viewport.Height - 1);
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
            ConvertColorSpace(data);
            int texture = CreateTexture(data);
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

        static void ConvertColorSpace(BitmapData _data)
        {
            // bgra to rgba
            var x = 0; var y = 0; var width = _data.Width; var height = _data.Height;
            for (y = 0; y < height; y++)
            {
                for (x = width - 1; x >= 0; x -= 1)
                {
                    var offset = y * _data.Stride + x * 4;
                    Marshal.WriteInt32(_data.Scan0, offset, ColorBGRA2RGBA(Marshal.ReadInt32(_data.Scan0, offset)));
                }
            }
        }

        static int CreateTexture(BitmapData data)
        {
            int texture = 0;
            GL.GenTextures(1, out texture);
            if (texture == 0)
            {
                return 0;
            }

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget2d.Texture2D, 0, TextureComponentCount.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.ES20.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);

            //https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/texParameter
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            if (isPowerOf2(data.Width) && isPowerOf2(data.Height))
            {
                // Yes, it's a power of 2. Generate mips.
                GL.GenerateMipmap(TextureTarget.Texture2D);
            }
            else
            {
                // No, it's not a power of 2. Turn of mips and set wrapping to clamp to edge
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            }
            return texture;
        }

        static bool isPowerOf2(int value)
        {
            return (value & (value - 1)) == 0;
        }

        static int FloorPowerOf2(int value)
        {
            var ans = Math.Log(value, 2);

            return (int)Math.Pow(2, Math.Floor(ans));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int ColorBGRA2RGBA(int x)
        {
            // bgra > rgba
            unchecked
            {
                return
                    // Source is in format: 0xAARRGGBB
                    (int)((x & 0xFF000000) >> 0) |
                    ((x & 0x00FF0000) >> 16) |
                    ((x & 0x0000FF00) << 0) |
                    ((x & 0x000000FF) << 16);
                    // Return value is in format:  0xAABBGGRR
            }
        }

        void imageLoader()
        {
            core.Zoom = minzoom;
            GMaps.Instance.CacheOnIdleRead = false;
            GMaps.Instance.BoostCacheEngine = true;
            // shared context
            var win = new OpenTK.GameWindow(640, 480, Context.GraphicsMode);
            win.Visible = false;
            IMGContext = win.Context;
            core.Zoom = 20;

            while (!this.IsDisposed)
            {
                if (sizeChanged)
                {
                    sizeChanged = false;
                    core.OnMapSizeChanged(1000, 1000);
                }

                if (_center.GetDistance(core.Position) > 30)
                {
                    core.Position = _center;
                }

                if (DateTime.Now.Second % 3 == 1 && tileArea != null)
                    lock(tileArea)
                        CleanupOldTextures(tileArea);

                // wait for current to load
                if (core.tileLoadQueue.Count > 0)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }

                if (core.FailedLoads.Count > 0)
                {
                }

                // current has loaded - process
                generateTextures();

                System.Threading.Thread.Sleep(100);
            }
        }

        public int minzoom { get; set; } = 12;
        public PointLatLngAlt mousePosition { get; private set; }

        public Utilities.Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        private int utmzone = -999;
        private PointLatLngAlt llacenter = PointLatLngAlt.Zero;
        private double[] utmcenter = new double[2];
        private PointLatLngAlt mouseDownPos;
        private Thread _imageloaderThread;
        private int mousex;
        private int mousey;
        private IGraphicsContext IMGContext;
        private bool started;
        private bool onpaintrun;
        private bool sizeChanged;
        private double[] mypos = new double[3];
        Vector3 myrpy = Vector3.UnitX;
        private bool fogon = true;
        private Lines _flightPlanLines;
        private DateTime _centerTime;
        private List<tileZoomArea> tileArea = new List<tileZoomArea>();

        double[] convertCoords(PointLatLngAlt plla)
        {
            var utm = plla.ToUTM(utmzone);
            Array.Resize(ref utm, 3);
            utm[0] -= utmcenter[0];
            utm[1] -= utmcenter[1];
            utm[2] = plla.Alt;
            return new[] {utm[0], utm[1], utm[2]};
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (!started)
                timer1.Start();
            started = true;
            onpaintrun = true;
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
            var beforewait = DateTime.Now;
            if (textureSemaphore.Wait(1) == false)
                return;
            var afterwait = DateTime.Now;
            try
            {
                double heightscale = 1; //(step/90.0)*5;
                var campos = convertCoords(_center);
                campos = projectLocation(mypos);
                var rpy = this.rpy;
                // use mypos if we are not tracking the mav
                if (!chk_locktomav.Checked)
                {
                    campos = mypos;
                    rpy = new MissionPlanner.Utilities.Vector3((float) myrpy.X, (float) myrpy.Y, (float) myrpy.Z);
                    KeyboardState input = Keyboard.GetState();
                    float speed = (1.5f);
                    Vector3 position = new Vector3((float) campos[0], (float) campos[1], (float) campos[2]);
                    Vector3 front = Vector3.Normalize(new Vector3((float) Math.Sin(MathHelper.Radians(rpy.Z)) * 1,
                        (float) Math.Cos(MathHelper.Radians(rpy.Z)) * 1, 0));
                    Vector3 up = new Vector3(0.0f, 0.0f, 1.0f);
                    if (input.IsKeyDown(Key.W))
                    {
                        position += front * speed; //Forward
                    }

                    if (input.IsKeyDown(Key.S))
                    {
                        position -= front * speed; //Backwards
                    }

                    if (input.IsKeyDown(Key.A))
                    {
                        position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed; //Left
                    }

                    if (input.IsKeyDown(Key.D))
                    {
                        position += Vector3.Normalize(Vector3.Cross(front, up)) * speed; //Right
                    }

                    if (input.IsKeyDown(Key.Q))
                    {
                        rpy.Z -= speed;
                    }

                    if (input.IsKeyDown(Key.E))
                    {
                        rpy.Z += speed;
                    }

                    if (input.IsKeyDown(Key.R))
                    {
                        position.Z += speed / 2;
                    }

                    if (input.IsKeyDown(Key.F))
                    {
                        position.Z -= speed / 2;
                    }

                    campos[0] = position.X;
                    campos[1] = position.Y;
                    campos[2] = position.Z;
                    _center.Alt = campos[2];
                }

                // save the state
                mypos = campos;
                myrpy = new OpenTK.Vector3((float) rpy.x, (float) rpy.y, (float) rpy.z);

                cameraX = campos[0];
                cameraY = campos[1];
                cameraZ = (campos[2] < srtm.getAltitude(_center.Lat, _center.Lng).alt)
                    ? (srtm.getAltitude(_center.Lat, _center.Lng).alt + 1) * heightscale
                    : _center.Alt * heightscale; // (srtm.getAltitude(lookZ, lookX, 20) + 100) * heighscale;
                lookX = campos[0] + Math.Sin(MathHelper.Radians(rpy.Z)) * 100;
                lookY = campos[1] + Math.Cos(MathHelper.Radians(rpy.Z)) * 100;
                lookZ = cameraZ;
                if (!Context.IsCurrent)
                    Context.MakeCurrent(this.WindowInfo);
                /*Console.WriteLine("cam: {0} {1} {2} lookat: {3} {4} {5}", (float) cameraX, (float) cameraY, (float) cameraZ,
                    (float) lookX,
                    (float) lookY, (float) lookZ);
                  */
                modelMatrix = Matrix4.LookAt((float) cameraX, (float) cameraY, (float) cameraZ + 100f * 0,
                    (float) lookX, (float) lookY, (float) lookZ,
                    0, 0, 1);
                // roll
                modelMatrix = Matrix4.Mult(modelMatrix, Matrix4.CreateRotationZ((float) (rpy.X * MathHelper.deg2rad)));
                // pitch
                modelMatrix = Matrix4.Mult(modelMatrix, Matrix4.CreateRotationX((float) (rpy.Y * -MathHelper.deg2rad)));

                {
                    // for unproject - updated on every draw
                    GL.GetInteger(GetPName.Viewport, viewport);
                }


                var beforeclear = DateTime.Now;
                //GL.Viewport(0, 0, Width, Height);
                GL.ClearColor(Color.CornflowerBlue);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit |
                         ClearBufferMask.AccumBufferBit);

                // disable depth during terrain draw
                GL.Disable(EnableCap.DepthTest);
                GL.DepthFunc(DepthFunction.Lequal);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.BlendEquation(BlendEquationMode.FuncAdd);

                var texlist = textureid.ToArray().ToSortedList(Comparison);
                int lastzoom = texlist.Count == 0 ? 0 : texlist[0].Value.zoom;
                var beforedraw = DateTime.Now;
                foreach (var tidict in texlist)
                {
                    if (lastzoom != tidict.Value.zoom)
                    {
                        lastzoom = tidict.Value.zoom;
                    }

                    if (tidict.Value.indices.Count > 0)
                    {
                        tidict.Value.Draw(projMatrix, modelMatrix);
                    }
                }

                var beforewps = DateTime.Now;
                GL.Enable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Texture2D);
                // draw after terrain - need depth check
                {
                    GL.Enable(EnableCap.DepthTest);
                    if (FlightPlanner.instance.pointlist.Count > 1)
                    {
                        if (_flightPlanLines != null)
                            _flightPlanLines.Dispose();
                        _flightPlanLines = new Lines();
                        _flightPlanLines.Width = 3.0f;
                        // render wps
                        foreach (var point in FlightPlanner.instance.pointlist)
                        {
                            if (point == null)
                                continue;
                            var co = convertCoords(point);
                            _flightPlanLines.Add(co[0], co[1], co[2], 1, 0, 0, 1);
                        }

                        _flightPlanLines.Draw(projMatrix, modelMatrix);
                    }
                }
                var beforewpsmarkers = DateTime.Now;
                {
                    if (green == 0)
                    {
                        green = generateTexture(GMap.NET.Drawing.Properties.Resources.green.ToBitmap());
                    }

                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(false);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, green);
                    var list = FlightPlanner.instance.pointlist.Where(a => a != null).ToList();
                    if (MainV2.comPort.MAV.cs.mode.ToLower() == "guided")
                        list.Add(new PointLatLngAlt(MainV2.comPort.MAV.GuidedMode)
                            {Alt = MainV2.comPort.MAV.GuidedMode.z + MainV2.comPort.MAV.cs.HomeAlt});
                    if (MainV2.comPort.MAV.cs.TargetLocation != PointLatLngAlt.Zero)
                        list.Add(MainV2.comPort.MAV.cs.TargetLocation);

                    if (MainV2.comPort.MAV.cs.Location != PointLatLngAlt.Zero)
                        list.Add(MainV2.comPort.MAV.cs.Location);

                    foreach (var point in list.OrderBy((a)=> a.GetDistance(MainV2.comPort.MAV.cs.Location)))
                    {
                        if (point == null)
                            continue;
                        if (point.Lat == 0 && point.Lng == 0)
                            continue;
                        var co = convertCoords(point);

                        var wpmarker = new tileInfo(Context, WindowInfo, textureSemaphore);
                        wpmarker.idtexture = green;

                        //tr
                        wpmarker.vertex.Add(new Vertex(Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2 + co[0],
                            Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2 + co[1], co[2] + 10, 0, 0, 0, 1, 0, 0));
                        //tl
                        wpmarker.vertex.Add(new Vertex(co[0] - Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2,
                            co[1] - Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2, co[2] + 10, 0, 0, 0, 1, 1, 0));
                        //br
                        wpmarker.vertex.Add(new Vertex(co[0] + Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2,
                            co[1] + Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2, co[2] - 1, 0, 0, 0, 1, 0, 1));
                        //bl
                        wpmarker.vertex.Add(new Vertex(co[0] - Math.Sin(MathHelper.Radians(rpy.Z + 90)) * 2,
                            co[1] - Math.Cos(MathHelper.Radians(rpy.Z + 90)) * 2, co[2] - 1, 0, 0, 0, 1, 1, 1));

                        var startindex = (uint)wpmarker.vertex.Count - 4;
                        wpmarker.indices.AddRange(new[]
                                        {
                                startindex + 1, startindex + 2, startindex + 0,
                                startindex + 1, startindex + 3, startindex + 2
                            });


                        wpmarker.Draw(projMatrix, modelMatrix);

                        wpmarker.Cleanup(true);
                    }

                    GL.Disable(EnableCap.Blend);
                    GL.DepthMask(true);
                }
                var beforeswapbuffer = DateTime.Now;
                try
                {
                    this.SwapBuffers();
                }
                catch
                {
                }

                try
                {
                    if (Context.IsCurrent)
                        Context.MakeCurrent(null);
                }
                catch
                {
                }

                //this.Invalidate();
                var delta = DateTime.Now - start;
                //Console.Write("OpenGLTest2 {0}    \r", delta.TotalMilliseconds);
                if (delta.TotalMilliseconds > 20)
                    Console.Write("OpenGLTest2 total {0} swap {1} wps {2} draw {3} clear {4} wait {5} bwait {6} wpmark {7}  \n",
                        delta.TotalMilliseconds,
                        (beforeswapbuffer - start).TotalMilliseconds,
                        (beforewps - start).TotalMilliseconds,
                        (beforedraw - start).TotalMilliseconds,
                        (beforeclear - start).TotalMilliseconds,
                        (afterwait - start).TotalMilliseconds,
                        (beforewait - start).TotalMilliseconds,
                        (beforewpsmarkers - start).TotalMilliseconds);
            }
            finally
            {
                textureSemaphore.Release();
            }
        }

        private double[] projectLocation(double[] oldpos)
        {
            var newloc = LocationProjection.Project(_center, _velocity, _centerTime, DateTime.Now);
            var newpos = convertCoords(newloc);
            var factor = 0.3;
            return new double[]
            {
                oldpos[0] * factor + newpos[0] * (1.0 - factor),
                oldpos[1] * factor + newpos[1] * (1.0 - factor),
                oldpos[2] * factor + newpos[2] * (1.0 - factor)
            };
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
            lock (tileArea)
            {
                tileArea = new List<tileZoomArea>();
                for (int a = minzoom; a <= zoom; a++)
                {
                    var area2 = new RectLatLng(_center.Lat, _center.Lng, 0, 0);
                    // 50m at max zoom
                    // step at 0 zoom
                    var distm = MathHelper.map(a, 0, zoom, 3000, 50);
                    var offset = _center.newpos(45, distm);
                    area2.Inflate(Math.Abs(_center.Lat - offset.Lat), Math.Abs(_center.Lng - offset.Lng));
                    var extratile = 0;
                    if (a == minzoom)
                        extratile = 4;
                    var tiles = new tileZoomArea()
                    {
                        zoom = a,
                        points = prj.GetAreaTileList(area2, a, extratile),
                        area = area2
                    };
                    //Console.WriteLine("tiles z {0} max {1} dist {2} tiles {3} pxper100m {4} - {5}", a, zoom, distm,
                    //  tiles.points.Count, core.pxRes100m, core.Zoom);
                    tileArea.Add(tiles);

                    // queue the tile load/fetch
                    foreach (var p in tiles.points)
                    {
                        LoadTask task = new LoadTask(p, a);
                        {
                            if (!core.tileLoadQueue.Contains(task))
                            {
                                core.tileLoadQueue.Push(task);
                            }
                        }
                    }
                }

                //Minimumtile(tileArea);

                var totaltiles = 0;
                foreach (var a in tileArea) totaltiles += a.points.Count;
                Console.Write(DateTime.Now.Millisecond + " Total tiles " + totaltiles + "   \r");
                if (DateTime.Now.Second % 3 == 1)
                    CleanupOldTextures(tileArea);
            }

            //https://wiki.openstreetmap.org/wiki/Zoom_levels
            var C = 2 * Math.PI * 6378137.000;
            // horizontal distance by each tile square
            var stile = C * Math.Cos(_center.Lat) / Math.Pow(2, zoom);
            var pxstep = 2;
            //https://wiki.openstreetmap.org/wiki/Zoom_levels
            // zoom 20 = 38m
            // get tiles & combine into one
            tileZoomArea[] talist;
            lock (tileArea)
                talist = tileArea.ToArray();
            foreach (var tilearea in talist)
            {
                stile = C * Math.Cos(_center.Lat) / Math.Pow(2, tilearea.zoom);
                pxstep = (int)(stile / 45);
                pxstep = FloorPowerOf2(pxstep);
                if (pxstep == int.MinValue) pxstep = 0;
                if (pxstep == 0)
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
                            foreach (var imgPI in t.Overlays)
                            {
                                var img = (GMapImage) imgPI;
                                if (!textureid.ContainsKey(p))
                                {
                                    try
                                    {
                                        var ti = new tileInfo(Context, this.WindowInfo, textureSemaphore)
                                        {
                                            point = p,
                                            zoom = tilearea.zoom,
                                            img = (Image) img.Img.Clone()
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
                                            if (ti == null)
                                                break;
                                        }

                                        if (ti != null)
                                        {
                                            //textureSemaphore.Wait();
                                            try
                                            {
                                                // do this on UI thread
                                                //if (!Context.IsCurrent)
                                                //Context.MakeCurrent(this.WindowInfo);
                                                // load it all
                                                //var temp = ti.idtexture; -- intel hd graphics cant share textures
                                                var temp2 = ti.idEBO;
                                                var temp3 = ti.idVBO;
                                                //var temp4 = ti.idVAO;
                                                // release it
                                                //if (Context.IsCurrent)
                                                //Context.MakeCurrent(null);
                                            }
                                            catch
                                            {
                                                return;
                                            }
                                            finally
                                            {
                                                //textureSemaphore.Release();
                                            }

                                            textureid[p] = ti;

                                            //File.WriteAllText(p.ToString(), ti.ToJSON());
                                        }
                                    }
                                    catch
                                    {
                                        return;
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

        private void Minimumtile(List<tileZoomArea> tileArea)
        {
            foreach (tileZoomArea tileZoomArea in tileArea.Reverse<tileZoomArea>())
            {
                //GPoint centerPixel = Provider.Projection.FromLatLngToPixel(center, Zoom);
                //var centerTileXYLocation = Provider.Projection.FromPixelToTileXY(centerPixel);
                foreach (var pnt in tileZoomArea.points)
                {
                    var dx = pnt.X / 2.0;
                    var dy = pnt.Y / 2.0;

                    var zoomup = new GPoint(pnt.X / 2, pnt.Y / 2);

                    var pixel = core.Provider.Projection.FromTileXYToPixel(pnt);
                    var pixelup = core.Provider.Projection.FromTileXYToPixel(zoomup);

                    var tilesup = tileArea.Where(a => a.zoom == tileZoomArea.zoom - 1);
                    if (tilesup.Count() > 0 && tilesup.First().points.Contains(zoomup))
                        tilesup.First().points.Remove(zoomup);
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
            var zindexmod = (20 - ti.zoom) * 0.30;
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
            ti.vertex.Add(new Vertex(utm4[0], utm4[1], utm4[2] - zindexmod, 1, 0, 0, 1, imgx, imgy));
            imgx = MathHelper.map(xnext, xstart, xend, 0, 1);
            imgy = MathHelper.map(y, ystart, yend, 0, 1);
            ti.vertex.Add(new Vertex(utm3[0], utm3[1], utm3[2] - zindexmod, 0, 1, 0, 1, imgx, imgy));
            imgx = MathHelper.map(x, xstart, xend, 0, 1);
            imgy = MathHelper.map(y, ystart, yend, 0, 1);
            ti.vertex.Add(new Vertex(utm1[0], utm1[1], utm1[2] - zindexmod, 0, 0, 1, 1, imgx, imgy));
            imgx = MathHelper.map(x, xstart, xend, 0, 1);
            imgy = MathHelper.map(ynext, ystart, yend, 0, 1);
            ti.vertex.Add(new Vertex(utm2[0], utm2[1], utm2[2] - zindexmod, 1, 1, 0, 1, imgx, imgy));
            var startindex = (uint) ti.vertex.Count - 4;
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
                this.BeginInvoke((MethodInvoker) delegate
                {
                    Console.WriteLine(DateTime.Now.Millisecond + " tile cleanup    \r");
                    tileInfo temp;
                    textureid.TryRemove(c.Key, out temp);
                    temp?.Cleanup();
                });
            });
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.num_minzoom = new System.Windows.Forms.NumericUpDown();
            this.num_maxzoom = new System.Windows.Forms.NumericUpDown();
            this.chk_locktomav = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chk_fog = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize) (this.num_minzoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.num_maxzoom)).BeginInit();
            this.SuspendLayout();
            //
            // num_minzoom
            //
            this.num_minzoom.Location = new System.Drawing.Point(3, 3);
            this.num_minzoom.Maximum = new decimal(new int[]
            {
                20,
                0,
                0,
                0
            });
            this.num_minzoom.Minimum = new decimal(new int[]
            {
                1,
                0,
                0,
                0
            });
            this.num_minzoom.Name = "num_minzoom";
            this.num_minzoom.Size = new System.Drawing.Size(54, 20);
            this.num_minzoom.TabIndex = 0;
            this.num_minzoom.Value = new decimal(new int[]
            {
                12,
                0,
                0,
                0
            });
            this.num_minzoom.ValueChanged += new System.EventHandler(this.num_minzoom_ValueChanged);
            //
            // num_maxzoom
            //
            this.num_maxzoom.Location = new System.Drawing.Point(3, 29);
            this.num_maxzoom.Maximum = new decimal(new int[]
            {
                20,
                0,
                0,
                0
            });
            this.num_maxzoom.Minimum = new decimal(new int[]
            {
                1,
                0,
                0,
                0
            });
            this.num_maxzoom.Name = "num_maxzoom";
            this.num_maxzoom.Size = new System.Drawing.Size(54, 20);
            this.num_maxzoom.TabIndex = 1;
            this.num_maxzoom.Value = new decimal(new int[]
            {
                20,
                0,
                0,
                0
            });
            this.num_maxzoom.ValueChanged += new System.EventHandler(this.num_maxzoom_ValueChanged);
            //
            // chk_locktomav
            //
            this.chk_locktomav.AutoSize = true;
            this.chk_locktomav.Checked = true;
            this.chk_locktomav.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_locktomav.Location = new System.Drawing.Point(63, 6);
            this.chk_locktomav.Name = "chk_locktomav";
            this.chk_locktomav.Size = new System.Drawing.Size(88, 17);
            this.chk_locktomav.TabIndex = 2;
            this.chk_locktomav.Text = "Lock to MAV";
            this.chk_locktomav.UseVisualStyleBackColor = true;
            //
            // timer1
            //
            this.timer1.Interval = 40;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // chk_fog
            //
            this.chk_fog.AutoSize = true;
            this.chk_fog.Checked = true;
            this.chk_fog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_fog.Location = new System.Drawing.Point(63, 32);
            this.chk_fog.Name = "chk_fog";
            this.chk_fog.Size = new System.Drawing.Size(44, 17);
            this.chk_fog.TabIndex = 3;
            this.chk_fog.Text = "Fog";
            this.chk_fog.UseVisualStyleBackColor = true;
            this.chk_fog.CheckedChanged += new System.EventHandler(this.chk_fog_CheckedChanged);
            //
            // OpenGLtest2
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.chk_fog);
            this.Controls.Add(this.chk_locktomav);
            this.Controls.Add(this.num_maxzoom);
            this.Controls.Add(this.num_minzoom);
            this.Name = "OpenGLtest2";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.test_Load);
            this.Resize += new System.EventHandler(this.test_Resize);
            ((System.ComponentModel.ISupportInitialize) (this.num_minzoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.num_maxzoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void test_Load(object sender, EventArgs e)
        {
            if (!Context.IsCurrent)
                Context.MakeCurrent(this.WindowInfo);

            _imageloaderThread = new Thread(imageLoader)
            {
                IsBackground = true,
                Name = "gl imageLoader"
            };
            _imageloaderThread.Start();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Normalize);
            //GL.Enable(EnableCap.LineSmooth);
            //GL.Enable(EnableCap.PointSmooth);
            //GL.Enable(EnableCap.PolygonSmooth);
            //GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);
            var preload = tileInfo.Program;
            test_Resize(null, null);
        }

        private void num_minzoom_ValueChanged(object sender, EventArgs e)
        {
            minzoom = (int) num_minzoom.Value;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (onpaintrun == true && IsHandleCreated && !IsDisposed && !Disposing)
            {
                this.Invalidate();
                onpaintrun = false;
            }
        }

        private void chk_fog_CheckedChanged(object sender, EventArgs e)
        {
            fogon = chk_fog.Checked;
        }

        private void num_maxzoom_ValueChanged(object sender, EventArgs e)
        {
            zoom = (int) num_maxzoom.Value;
        }

        private void test_Resize(object sender, EventArgs e)
        {
            textureSemaphore.Wait();
            try
            {
                if (!Context.IsCurrent)
                    Context.MakeCurrent(this.WindowInfo);
                GL.Viewport(0, 0, this.Width, this.Height);
                projMatrix = OpenTK.Matrix4.CreatePerspectiveFieldOfView(
                    (float) (90 * MathHelper.deg2rad),
                    (float) Width / Height, 2f,
                    (float) 20000);
                GL.UniformMatrix4(tileInfo.projectionSlot, 1, false, ref projMatrix.Row0.X);
                {
                    // for unproject - updated on every draw
                    GL.GetInteger(GetPName.Viewport, viewport);
                }
                if (Context.IsCurrent)
                    Context.MakeCurrent(null);
            }
            finally
            {
                textureSemaphore.Release();
            }

            sizeChanged = true;
            this.Invalidate();
        }

        public class tileZoomArea
        {
            public List<GPoint> points;
            public int zoom;
            public RectLatLng area { get; set; }
        }

        public class Lines: IDisposable
        {
            private static int _program;
            private static int VertexShader;
            private static int FragmentShader;
            private static int positionSlot;
            private static int colorSlot;
            private static int projectionSlot;
            private static int modelViewSlot;
            private static int textureSlot;
            private static int texCoordSlot;
            private int ID_VBO;
            private int ID_EBO;
            private bool disposedValue;

            public List<Vertex> vertex { get; } = new List<Vertex>();
            public List<uint> indices { get; } = new List<uint>();
            public float Width { get; set; } = 1.0f;

            public void Add(double x,double y, double z, double r, double g, double b, double a)
            {
                vertex.Add(new Vertex(x, y, z, r, g, b, a, 0, 0));
                indices.Add((uint)(vertex.Count - 1));
            }

            /// <summary>
            /// Vertex Buffer
            /// </summary>
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
                        BufferUsageHint.DynamicDraw);
                    int bufferSize;
                    // Validate that the buffer is the correct size
                    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                    if (vertex.Count * Vertex.Stride != bufferSize)
                        throw new ApplicationException("Vertex array not uploaded correctly");
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    return ID_VBO;
                }
            }

            /// <summary>
            /// Element index Buffer
            /// </summary>
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
                        BufferUsageHint.DynamicDraw);
                    int bufferSize;
                    // Validate that the buffer is the correct size
                    GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize,
                        out bufferSize);
                    if (indices.Count * sizeof(int) != bufferSize)
                        throw new ApplicationException("Element array not uploaded correctly");
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                    return ID_EBO;
                }
            }

            public void Draw(Matrix4 Projection, Matrix4 ModelView)
            {
                if (idVBO == 0 || idEBO == 0)
                    return;

                if (_program == 0)
                    CreateShaders();

                // use the shader
                GL.UseProgram(_program);

                // enable position
                GL.EnableVertexAttribArray(positionSlot);
                // enable color
                GL.EnableVertexAttribArray(colorSlot);

                // set matrix
                GL.UniformMatrix4(modelViewSlot, 1, false, ref ModelView.Row0.X);
                GL.UniformMatrix4(projectionSlot, 1, false, ref Projection.Row0.X);

                // set linewidth in px
                GL.LineWidth(Width);

                // bind the vertex buffers
                GL.BindBuffer(BufferTarget.ArrayBuffer, idVBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, idEBO);

                // map the vertex buffers
                GL.VertexAttribPointer(positionSlot, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), (IntPtr)0);
                GL.VertexAttribPointer(colorSlot, 4, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), (IntPtr)(sizeof(float) * 3));
                GL.VertexAttribPointer(texCoordSlot, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), (IntPtr)(sizeof(float) * 7));

                // draw it
                GL.DrawArrays(PrimitiveType.LineStrip, 0, indices.Count);

                // disable vertex array
                GL.DisableVertexAttribArray(positionSlot);
                GL.DisableVertexAttribArray(colorSlot);
            }

            private static void CreateShaders()
            {
                VertexShader = GL.CreateShader(ShaderType.VertexShader);
                FragmentShader = GL.CreateShader(ShaderType.FragmentShader);

                //https://webglfundamentals.org/webgl/lessons/webgl-fog.html
                //http://www.ozone3d.net/tutorials/glsl_fog/p04.php

                GL.ShaderSource(VertexShader, @"
attribute vec3 Position;
attribute vec4 SourceColor;
attribute vec2 TexCoordIn;
varying vec4 DestinationColor;
varying vec2 TexCoordOut;
uniform mat4 Projection;
uniform mat4 ModelView;
void main(void) {
    DestinationColor = SourceColor;
    gl_Position = Projection * ModelView * vec4(Position, 1.0);
    TexCoordOut = TexCoordIn;
}
                ");
                GL.ShaderSource(FragmentShader, @"
varying vec4 DestinationColor;
varying vec2 TexCoordOut;
uniform sampler2D Texture;
void main(void) {
    float z = gl_FragCoord.z / gl_FragCoord.w;
    gl_FragColor = DestinationColor;
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

                _program = GL.CreateProgram();
                GL.AttachShader(_program, VertexShader);
                GL.AttachShader(_program, FragmentShader);
                GL.LinkProgram(_program);
                Debug.WriteLine(GL.GetProgramInfoLog(_program));
                GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out code);
                if (code != (int)All.True)
                {
                    // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                    throw new Exception(
                        $"Error occurred whilst linking Program({_program}) {GL.GetProgramInfoLog(_program)}");
                }

                GL.UseProgram(_program);
                positionSlot = GL.GetAttribLocation(_program, "Position");
                colorSlot = GL.GetAttribLocation(_program, "SourceColor");
                texCoordSlot = GL.GetAttribLocation(_program, "TexCoordIn");
                projectionSlot = GL.GetUniformLocation(_program, "Projection");
                modelViewSlot = GL.GetUniformLocation(_program, "ModelView");
                textureSlot = GL.GetUniformLocation(_program, "Texture");
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    GL.DeleteBuffers(1, ref ID_VBO);
                    GL.DeleteBuffers(1, ref ID_EBO);
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// inspired by https://github.com/jlyonsmith/GLES2Tutorial/blob/master/08/OpenGLView.cs
        /// </summary>
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
                    _data = ((Bitmap) _img).LockBits(new System.Drawing.Rectangle(0, 0, _img.Width, _img.Height),
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
                            if (_data == null)
                                return 0;

                            ConvertColorSpace(_data);
                            _textid = CreateTexture(_data);
                        }
                        catch
                        {
                        }
                    }

                    return _textid;
                }

                set
                {
                    _textmanual = true;
                    _textid = value;
                }
            }

            private int ID_VBO = 0;
            private int ID_EBO = 0;
            private static int _program = 0;

            internal static int Program
            {
                get
                {
                    if (_program == 0)
                        CreateShaders();
                    return _program;
                }
            }

            private bool init;
            private IGraphicsContext Context;
            private IWindowInfo WindowInfo;
            private readonly SemaphoreSlim contextLock;
            private static int positionSlot;
            private static int colorSlot;
            private static int texCoordSlot;
            internal static int projectionSlot;
            internal static int modelViewSlot;
            private static int textureSlot;
            private bool _textmanual = false;

            public tileInfo(IGraphicsContext context, IWindowInfo windowInfo, SemaphoreSlim contextLock)
            {
                this.Context = context;
                this.WindowInfo = windowInfo;
                this.contextLock = contextLock;
            }

            /// <summary>
            /// Vertex Buffer
            /// </summary>
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
                    int bufferSize;
                    // Validate that the buffer is the correct size
                    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                    if (vertex.Count * Vertex.Stride != bufferSize)
                        throw new ApplicationException("Vertex array not uploaded correctly");
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    return ID_VBO;
                }
            }

            /// <summary>
            /// Element index Buffer
            /// </summary>
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
                    int bufferSize;
                    // Validate that the buffer is the correct size
                    GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize,
                        out bufferSize);
                    if (indices.Count * sizeof(int) != bufferSize)
                        throw new ApplicationException("Element array not uploaded correctly");
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                    return ID_EBO;
                }
            }

            public List<Vertex> vertex { get; } = new List<Vertex>();
            public List<uint> indices { get; } = new List<uint>();

            public override string ToString()
            {
                return String.Format("{1}-{0} {2}", point, zoom, textureReady);
            }

            public void Draw(Matrix4 Projection, Matrix4 ModelView)
            {
                if (!init)
                {
                    if (idVBO == 0 || idEBO == 0)
                        return;
                    if (idtexture == 0)
                        return;

                    if (Program == 0)
                        CreateShaders();
                    init = true;
                }

                {
                    GL.UseProgram(Program);

                    GL.EnableVertexAttribArray(positionSlot);
                    GL.EnableVertexAttribArray(colorSlot);
                    GL.EnableVertexAttribArray(texCoordSlot);

                    // set matrix
                    GL.UniformMatrix4(modelViewSlot, 1, false, ref ModelView.Row0.X);
                    GL.UniformMatrix4(projectionSlot, 1, false, ref Projection.Row0.X);

                    if (textureReady)
                    {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.Enable(EnableCap.Texture2D);
                        GL.BindTexture(TextureTarget.Texture2D, idtexture);
                        GL.Uniform1(textureSlot, 0);
                    }
                    else
                    {
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                        GL.Disable(EnableCap.Texture2D);
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, idVBO);
                    GL.VertexAttribPointer(positionSlot, 3, VertexAttribPointerType.Float, false,
                        Marshal.SizeOf(typeof(Vertex)), (IntPtr) 0);
                    GL.VertexAttribPointer(colorSlot, 4, VertexAttribPointerType.Float, false,
                        Marshal.SizeOf(typeof(Vertex)), (IntPtr) (sizeof(float) * 3));
                    GL.VertexAttribPointer(texCoordSlot, 2, VertexAttribPointerType.Float, false,
                        Marshal.SizeOf(typeof(Vertex)), (IntPtr) (sizeof(float) * 7));
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, idEBO);
                    GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt,
                        IntPtr.Zero);

                    GL.DisableVertexAttribArray(positionSlot);
                    GL.DisableVertexAttribArray(colorSlot);
                    GL.DisableVertexAttribArray(texCoordSlot);
                }
            }

            private static void CreateShaders()
            {
                VertexShader = GL.CreateShader(ShaderType.VertexShader);
                FragmentShader = GL.CreateShader(ShaderType.FragmentShader);

                //https://webglfundamentals.org/webgl/lessons/webgl-fog.html
                //http://www.ozone3d.net/tutorials/glsl_fog/p04.php

                GL.ShaderSource(VertexShader, @"
attribute vec3 Position;
attribute vec4 SourceColor;
attribute vec2 TexCoordIn;
varying vec4 DestinationColor;
varying vec2 TexCoordOut;
uniform mat4 Projection;
uniform mat4 ModelView;
void main(void) {
    gl_Position = Projection * ModelView * vec4(Position, 1.0);
    TexCoordOut = TexCoordIn;
}
                ");
                GL.ShaderSource(FragmentShader, @"
varying vec4 DestinationColor;
varying vec2 TexCoordOut;
uniform sampler2D Texture;
void main(void) {
    vec4 color = texture2D(Texture, TexCoordOut);
    float z = gl_FragCoord.z / gl_FragCoord.w;
    float fogAmount = smoothstep(0.8, 1.0, gl_FragCoord.w);
    //if(fogAmount > 1.)         discard;
    gl_FragColor = color;// mix(color, vec4(0.4,0.6,0.9,1), fogAmount);
}
                ");
                GL.CompileShader(VertexShader);
                Debug.WriteLine(GL.GetShaderInfoLog(VertexShader));
                GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out var code);
                if (code != (int) All.True)
                {
                    // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                    throw new Exception(
                        $"Error occurred whilst compiling Shader({VertexShader}) {GL.GetShaderInfoLog(VertexShader)}");
                }

                GL.CompileShader(FragmentShader);
                Debug.WriteLine(GL.GetShaderInfoLog(FragmentShader));
                GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out code);
                if (code != (int) All.True)
                {
                    // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                    throw new Exception(
                        $"Error occurred whilst compiling Shader({FragmentShader}) {GL.GetShaderInfoLog(FragmentShader)}");
                }

                _program = GL.CreateProgram();
                GL.AttachShader(_program, VertexShader);
                GL.AttachShader(_program, FragmentShader);
                GL.LinkProgram(_program);
                Debug.WriteLine(GL.GetProgramInfoLog(_program));
                GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out code);
                if (code != (int) All.True)
                {
                    // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                    throw new Exception(
                        $"Error occurred whilst linking Program({_program}) {GL.GetProgramInfoLog(_program)}");
                }

                GL.UseProgram(_program);
                positionSlot = GL.GetAttribLocation(_program, "Position");
                colorSlot = GL.GetAttribLocation(_program, "SourceColor");
                texCoordSlot = GL.GetAttribLocation(_program, "TexCoordIn");
                projectionSlot = GL.GetUniformLocation(_program, "Projection");
                modelViewSlot = GL.GetUniformLocation(_program, "ModelView");
                textureSlot = GL.GetUniformLocation(_program, "Texture");
            }

            public static int FragmentShader { get; set; }
            public static int VertexShader { get; set; }

            public void Cleanup(bool nolock = false)
            {
                if (!nolock)
                    contextLock.Wait();
                try
                {
                    if (!nolock)
                        if (!Context.IsCurrent)
                            Context.MakeCurrent(WindowInfo);
                    if (!_textmanual)
                        GL.DeleteTextures(1, ref _textid);
                    GL.DeleteBuffers(1, ref ID_VBO);
                    GL.DeleteBuffers(1, ref ID_EBO);
                    //GL.DeleteProgram(Program);
                    try
                    {
                        if (img != null)
                            img.Dispose();
                    }
                    catch
                    {
                    }
                    if (!nolock)
                        if (Context.IsCurrent)
                            Context.MakeCurrent(null);
                }
                finally
                {
                    if (!nolock)
                        contextLock.Release();
                }
            }

            public void Dispose()
            {
                Cleanup();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vertex
        {
            //https://learnopengl.com/Getting-started/Textures
            public float X;
            public float Y;
            public float Z;
            public float R;
            public float G;
            public float B;
            public float A;
            public float S;
            public float T;
            /// <summary>
            /// Vertex
            /// </summary>
            /// <param name="x">position</param>
            /// <param name="y">position</param>
            /// <param name="z">position</param>
            /// <param name="r">color</param>
            /// <param name="g">color</param>
            /// <param name="b">color</param>
            /// <param name="a">color</param>
            /// <param name="s">texture</param>
            /// <param name="t">texture</param>
            public Vertex(double x, double y, double z, double r, double g, double b, double a, double s, double t)
            {
                X = (float)x;
                Y = (float)y;
                Z = (float)z;
                R = (float)r;
                G = (float)g;
                B = (float)b;
                A = (float)a;
                S = (float)s;
                T = (float)t;
                if (S > 1 || S < 0 || T > 1 || T < 0)
                {
                }
            }

            public static readonly int Stride = System.Runtime.InteropServices.Marshal.SizeOf(new Vertex());
        }
    }
}