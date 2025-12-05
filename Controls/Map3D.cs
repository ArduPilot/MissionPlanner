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
    /// <summary>
    /// Simple 1D Kalman filter for smooth interpolation
    /// </summary>
    public class SimpleKalmanFilter
    {
        private double _q; // process noise covariance
        private double _r; // measurement noise covariance
        private double _x; // estimated value
        private double _p; // estimation error covariance
        private double _k; // kalman gain

        public SimpleKalmanFilter(double q = 0.1, double r = 1.0, double initialValue = 0)
        {
            _q = q;
            _r = r;
            _x = initialValue;
            _p = 1.0;
        }

        public double Update(double measurement)
        {
            // Prediction update
            _p = _p + _q;

            // Measurement update
            _k = _p / (_p + _r);
            _x = _x + _k * (measurement - _x);
            _p = (1 - _k) * _p;

            return _x;
        }

        public double Value => _x;

        public void Reset(double value)
        {
            _x = value;
            _p = 1.0;
        }
    }

    public class Map3D : GLControl, IDeactivate
    {
        public static Map3D instance;
        private static GraphicsMode _graphicsMode;

        #region Constants
        private const double HEADING_LINE_LENGTH = 100; // meters
        private const double TURN_RADIUS_ARC_LENGTH = 200; // meters
        private const int TURN_RADIUS_SEGMENTS = 50;
        private const double ADSB_MAX_DISTANCE = 50000; // 50km
        private const double ADSB_RED_DISTANCE = 5000; // 5km
        private const double ADSB_YELLOW_DISTANCE = 10000; // 10km
        private const double ADSB_GREEN_DISTANCE = 20000; // 20km
        private const int ADSB_CIRCLE_SEGMENTS = 24;
        private const int TRAIL_SMOOTHING_WINDOW = 61;
        private const double WAYPOINT_MIN_DISTANCE = 61.0; // 200 feet in meters
        #endregion

        private static GraphicsMode GetGraphicsMode()
        {
            if (_graphicsMode != null)
                return _graphicsMode;

            // Prefer a 32-bit color buffer, 24-bit depth, 8-bit stencil, and 4x MSAA.
            try
            {
                _graphicsMode = new GraphicsMode(new ColorFormat(32), 24, 8, 4);
                return _graphicsMode;
            }
            catch
            {
                // Fall back to no multisampling if the platform/driver rejects MSAA.
                try
                {
                    _graphicsMode = new GraphicsMode(new ColorFormat(32), 24, 8, 0);
                    return _graphicsMode;
                }
                catch
                {
                    _graphicsMode = GraphicsMode.Default;
                    return _graphicsMode;
                }
            }
        }

        #region Helper Methods
        /// <summary>
        /// Gets a color based on distance for ADSB aircraft visualization.
        /// Red for close aircraft, yellow for medium distance, green for far.
        /// </summary>
        /// <param name="distance">Distance to aircraft in meters</param>
        /// <param name="isGrounded">Whether the aircraft is on the ground</param>
        /// <returns>RGBA color values (0.0-1.0)</returns>
        private (float r, float g, float b, float a) GetADSBDistanceColor(double distance, bool isGrounded)
        {
            if (isGrounded)
            {
                return (0.7f, 0.7f, 0.7f, 1.0f); // Light gray for grounded
            }

            if (distance <= ADSB_RED_DISTANCE)
            {
                return (1.0f, 0.0f, 0.0f, 1.0f); // Red
            }
            else if (distance <= ADSB_YELLOW_DISTANCE)
            {
                // Interpolate red to yellow
                float t = (float)((distance - ADSB_RED_DISTANCE) / (ADSB_YELLOW_DISTANCE - ADSB_RED_DISTANCE));
                return (1.0f, t, 0.0f, 1.0f);
            }
            else if (distance <= ADSB_GREEN_DISTANCE)
            {
                // Interpolate yellow to green
                float t = (float)((distance - ADSB_YELLOW_DISTANCE) / (ADSB_GREEN_DISTANCE - ADSB_YELLOW_DISTANCE));
                return (1.0f - t, 1.0f, 0.0f, 1.0f);
            }
            else
            {
                return (0.0f, 1.0f, 0.0f, 1.0f); // Green
            }
        }

        /// <summary>
        /// Calculates billboard orientation vectors for a point facing the camera.
        /// </summary>
        /// <param name="posX">Position X</param>
        /// <param name="posY">Position Y</param>
        /// <param name="posZ">Position Z</param>
        /// <param name="camX">Camera X</param>
        /// <param name="camY">Camera Y</param>
        /// <param name="camZ">Camera Z</param>
        /// <returns>Right and Up vectors for billboard orientation, or null if too close to camera</returns>
        private (double rightX, double rightY, double rightZ, double upX, double upY, double upZ)?
            CalculateBillboardOrientation(double posX, double posY, double posZ, double camX, double camY, double camZ)
        {
            // Calculate direction to camera
            double dx = posX - camX;
            double dy = posY - camY;
            double dz = posZ - camZ;
            double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);

            if (distance < 1.0)
                return null; // Too close to camera

            // Normalize view direction
            double viewDirX = dx / distance;
            double viewDirY = dy / distance;
            double viewDirZ = dz / distance;

            // Right vector (cross product of view dir with world up [0,0,1])
            double rightX = viewDirY;
            double rightY = -viewDirX;
            double rightZ = 0;
            double rightLen = Math.Sqrt(rightX * rightX + rightY * rightY);

            if (rightLen > 0.001)
            {
                rightX /= rightLen;
                rightY /= rightLen;
            }
            else
            {
                // Looking straight up/down, use arbitrary right
                rightX = 1;
                rightY = 0;
            }

            // Up vector (cross product of right with view dir)
            double upX = rightY * viewDirZ - rightZ * viewDirY;
            double upY = rightZ * viewDirX - rightX * viewDirZ;
            double upZ = rightX * viewDirY - rightY * viewDirX;

            return (rightX, rightY, rightZ, upX, upY, upZ);
        }

        /// <summary>
        /// Generates vertices for a billboarded circle facing the camera.
        /// </summary>
        /// <param name="centerX">Center X position</param>
        /// <param name="centerY">Center Y position</param>
        /// <param name="centerZ">Center Z position</param>
        /// <param name="radius">Circle radius</param>
        /// <param name="segments">Number of segments</param>
        /// <param name="r">Red color component (0-1)</param>
        /// <param name="g">Green color component (0-1)</param>
        /// <param name="b">Blue color component (0-1)</param>
        /// <param name="a">Alpha component (0-1)</param>
        /// <param name="vertices">List to add vertices to</param>
        private void AddBillboardCircleVertices(
            double centerX, double centerY, double centerZ,
            double radius, int segments,
            float r, float g, float b, float a,
            double rightX, double rightY, double rightZ,
            double upX, double upY, double upZ,
            List<float> vertices)
        {
            for (int i = 0; i < segments; i++)
            {
                double angle1 = (2 * Math.PI * i) / segments;
                double angle2 = (2 * Math.PI * (i + 1)) / segments;

                double cos1 = Math.Cos(angle1);
                double sin1 = Math.Sin(angle1);
                double cos2 = Math.Cos(angle2);
                double sin2 = Math.Sin(angle2);

                // Point 1: center + radius * (cos * right + sin * up)
                double x1 = centerX + radius * (cos1 * rightX + sin1 * upX);
                double y1 = centerY + radius * (cos1 * rightY + sin1 * upY);
                double z1 = centerZ + radius * (cos1 * rightZ + sin1 * upZ);

                // Point 2
                double x2 = centerX + radius * (cos2 * rightX + sin2 * upX);
                double y2 = centerY + radius * (cos2 * rightY + sin2 * upY);
                double z2 = centerZ + radius * (cos2 * rightZ + sin2 * upZ);

                // Add as separate line segment (x, y, z, r, g, b, a for each vertex)
                vertices.AddRange(new float[] { (float)x1, (float)y1, (float)z1, r, g, b, a });
                vertices.AddRange(new float[] { (float)x2, (float)y2, (float)z2, r, g, b, a });
            }
        }
        #endregion

        int green = 0;
        int greenAlt = 0;

        // Plane STL model
        private List<float> _planeVertices;
        private List<float> _planeNormals;
        private int _planeVBO = 0;
        private int _planeNormalVBO = 0;
        private int _planeVertexCount = 0;
        private float _planeScale = 1.0f;
        private bool _planeLoaded = false;
        private bool _settingsLoaded = false;
        // Plane position and rotation for current frame
        private double _planeDrawX, _planeDrawY, _planeDrawZ;
        private float _planeRoll, _planePitch, _planeYaw;
        // Configurable camera and plane settings
        private double _cameraDist = 0.8;    // Distance from plane
        private double _cameraAngle = 0.0;   // Angle offset from behind plane (degrees, 0=behind, 90=right, -90=left)
        private double _cameraHeight = 0.2;  // Height above plane
        private float _planeScaleMultiplier = 1.0f; // 1.0 = 1 meter wingspan
        private float _cameraFOV = 60f; // Field of view in degrees
        private Color _planeColor = Color.Red;
        private string _planeSTLPath = ""; // Empty = use embedded resource
        private int _whitePlaneTexture = 0; // White texture for plane rendering
        // Heading indicator line options
        private bool _showHeadingLine = true;
        private bool _showNavBearingLine = true;
        private bool _showGpsHeadingLine = true;
        private bool _showTurnRadius = true;
        private bool _showTrail = true;
        private double _waypointMarkerSize = 60; // Half-size of waypoint markers in meters
        private double _adsbCircleSize = 500; // Diameter of ADSB aircraft circles in meters
        // Trail (flight path history) - stored as absolute UTM coordinates (X, Y, Z)
        private List<double[]> _trailPoints = new List<double[]>();
        private int _trailUtmZone = -999;
        private Lines _trailLine = null;
        // ADSB aircraft hit testing - stores screen positions and data for tooltip
        private List<ADSBScreenPosition> _adsbScreenPositions = new List<ADSBScreenPosition>();
        private ToolTip _adsbToolTip;
        private adsb.PointLatLngAltHdg _lastHoveredADSB = null;
        ConcurrentDictionary<GPoint, tileInfo> textureid = new ConcurrentDictionary<GPoint, tileInfo>();
        GMap.NET.Internals.Core core = new GMap.NET.Internals.Core();
        private GMapProvider type;
        private PureProjection prj;
        double cameraX, cameraY, cameraZ; // camera coordinates

        double lookX, lookY, lookZ; // camera look-at coordinates

        // image zoom level
        public int zoom { get; set; } = 15;
        private const int zoomLevelOffset = 5;
        private int minzoom => Math.Max(1, zoom - zoomLevelOffset);
        private MyButton btn_configure;
        private SemaphoreSlim textureSemaphore = new SemaphoreSlim(1, 1);
        private Timer timer1;
        private bool _stopRequested;
        private System.ComponentModel.IContainer components;
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

        public Map3D() : base(GetGraphicsMode())
        {
            instance = this;

            // Load settings early, before any rendering
            zoom = Settings.Instance.GetInt32("map3d_zoom_level", 15);
            _cameraDist = Settings.Instance.GetDouble("map3d_camera_dist", 0.8);
            _cameraAngle = Settings.Instance.GetDouble("map3d_camera_angle", 0.0);
            _cameraHeight = Settings.Instance.GetDouble("map3d_camera_height", 0.2);
            _planeScaleMultiplier = (float)Settings.Instance.GetDouble("map3d_mav_scale", Settings.Instance.GetDouble("map3d_plane_scale", 1.0));
            _cameraFOV = (float)Settings.Instance.GetDouble("map3d_fov", 60);
            _planeSTLPath = Settings.Instance.GetString("map3d_plane_stl_path", "");
            try
            {
                int colorArgb = Settings.Instance.GetInt32("map3d_mav_color", Settings.Instance.GetInt32("map3d_plane_color", Color.Red.ToArgb()));
                _planeColor = Color.FromArgb(colorArgb);
            }
            catch { _planeColor = Color.Red; }
            _showHeadingLine = Settings.Instance.GetBoolean("map3d_show_heading", true);
            _showNavBearingLine = Settings.Instance.GetBoolean("map3d_show_nav_bearing", true);
            _showGpsHeadingLine = Settings.Instance.GetBoolean("map3d_show_gps_heading", true);
            _showTurnRadius = Settings.Instance.GetBoolean("map3d_show_turn_radius", true);
            _showTrail = Settings.Instance.GetBoolean("map3d_show_trail", false);
            _waypointMarkerSize = Settings.Instance.GetDouble("map3d_waypoint_marker_size", 60);
            _adsbCircleSize = Settings.Instance.GetDouble("map3d_adsb_size", 500);

            InitializeComponent();
            Click += OnClick;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;

            // Initialize ADSB tooltip
            _adsbToolTip = new ToolTip();
            _adsbToolTip.AutoPopDelay = 10000;
            _adsbToolTip.InitialDelay = 0;
            _adsbToolTip.ReshowDelay = 0;
            _adsbToolTip.ShowAlways = true;
            core.OnMapOpen();
            type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            prj = type.Projection;
            LocationCenter = LocationCenter.newpos(0, 0.1);
            // Disable VSync for smoother rendering
            try
            {
                VSync = false;
            }
            catch { }
            this.Invalidate();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Start dragging to adjust camera Y (side) and Z (height)
                _isDragging = true;
                _dragStartX = e.X;
                _dragStartY = e.Y;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isDragging)
            {
                _isDragging = false;
                // Don't save - drag changes are temporary
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            mousex = e.X;
            mousey = e.Y;

            // Handle left-drag to rotate camera around vehicle (X) and adjust height (Y)
            if (_isDragging)
            {
                int deltaX = mousex - _dragStartX; // Positive = dragged right = rotate camera right
                int deltaY = mousey - _dragStartY; // Positive = dragged down = increase height

                _cameraAngle += deltaX * 0.5; // Degrees per pixel
                _cameraHeight = Math.Max(-5.0, Math.Min(5.0, _cameraHeight + deltaY * 0.005));

                _dragStartX = mousex;
                _dragStartY = mousey;
                return;
            }

            try
            {
                mousePosition = getMousePos(mousex, mousey);
            } catch { }

            // Check for ADSB aircraft hover
            CheckADSBHover(e.X, e.Y);
        }

        /// <summary>
        /// Checks if the mouse is hovering over an ADSB aircraft circle and shows tooltip.
        /// </summary>
        private void CheckADSBHover(int mouseX, int mouseY)
        {
            adsb.PointLatLngAltHdg hoveredPlane = null;
            double hoveredDistance = 0;

            foreach (var pos in _adsbScreenPositions)
            {
                float dx = mouseX - pos.ScreenX;
                float dy = mouseY - pos.ScreenY;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist <= pos.Radius)
                {
                    hoveredPlane = pos.PlaneData;
                    hoveredDistance = pos.DistanceToOwn;
                    break;
                }
            }

            if (hoveredPlane != null)
            {
                if (!object.ReferenceEquals(_lastHoveredADSB, hoveredPlane))
                {
                    _lastHoveredADSB = hoveredPlane;

                    // Build tooltip text similar to 2D map
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine("ICAO: " + hoveredPlane.Tag);
                    sb.AppendLine("Callsign: " + hoveredPlane.CallSign);

                    // Type/Category
                    string typeCategory = "";
                    try
                    {
                        var raw = hoveredPlane.Raw as Newtonsoft.Json.Linq.JObject;
                        if (raw != null && raw.ContainsKey("t"))
                        {
                            typeCategory = raw["t"].ToString() + " ";
                        }
                    }
                    catch { }
                    typeCategory += hoveredPlane.GetCategoryFriendlyString();
                    sb.AppendLine("Type\\Category: " + typeCategory);

                    sb.AppendLine("Squawk: " + hoveredPlane.Squawk.ToString());
                    sb.AppendLine("Altitude: " + (hoveredPlane.Alt * CurrentState.multiplieralt).ToString("0") + " " + CurrentState.AltUnit);
                    sb.AppendLine("Speed: " + (hoveredPlane.Speed / 100.0 * CurrentState.multiplierspeed).ToString("0") + " " + CurrentState.SpeedUnit);
                    sb.AppendLine("Heading: " + hoveredPlane.Heading.ToString("0") + "°");

                    // Distance
                    string distanceStr;
                    if (hoveredDistance > 1000)
                        distanceStr = (hoveredDistance / 1000).ToString("0.#") + " km";
                    else
                        distanceStr = (hoveredDistance * CurrentState.multiplierdist).ToString("0") + " " + CurrentState.DistanceUnit;
                    sb.AppendLine("Distance: " + distanceStr);

                    // Altitude delta
                    double ownAlt = MainV2.comPort?.MAV?.cs?.alt ?? 0;
                    double altDelta = (hoveredPlane.Alt - ownAlt) * CurrentState.multiplieralt;
                    string altDeltaStr = (altDelta >= 0 ? "+" : "") + altDelta.ToString("0");
                    sb.AppendLine("Alt Delta: " + altDeltaStr + " " + CurrentState.AltUnit);

                    // Collision threat level
                    if (hoveredPlane.ThreatLevel != MAVLink.MAV_COLLISION_THREAT_LEVEL.NONE)
                        sb.AppendLine("Collision risk: " + (hoveredPlane.ThreatLevel == MAVLink.MAV_COLLISION_THREAT_LEVEL.LOW ? "Warning" : "Danger"));

                    _adsbToolTip.Show(sb.ToString().TrimEnd(), this, mouseX + 15, mouseY + 15);
                }
            }
            else
            {
                if (_lastHoveredADSB != null)
                {
                    _lastHoveredADSB = null;
                    _adsbToolTip.Hide(this);
                }
            }
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _cameraDist = Settings.Instance.GetDouble("map3d_camera_dist", 0.8);
                _cameraAngle = Settings.Instance.GetDouble("map3d_camera_angle", 0.0);
                _cameraHeight = Settings.Instance.GetDouble("map3d_camera_height", 0.2);
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            // Adjust camera distance with scroll wheel (temporary, not saved)
            // Scroll up (positive delta) = move camera closer (decrease distance)
            // Scroll down (negative delta) = move camera further (increase distance)
            double adjustment = -e.Delta / 1200.0; // 0.1 per scroll notch (120 units per notch)
            _cameraDist = Math.Max(0.1, Math.Min(10.0, _cameraDist + adjustment));
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

        ~Map3D()
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
            timer1?.Stop();
            started = false;

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

        public void Activate()
        {
            if (!started)
            {
                timer1?.Start();
                started = true;
            }
            this.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stopRequested = true;
                timer1?.Stop();
                timer1?.Dispose();

                try
                {
                    _imageloaderThread?.Join(200);
                }
                catch { }

                try
                {
                    _imageLoaderWindow?.Close();
                    _imageLoaderWindow?.Dispose();
                }
                catch
                {
                }

                _imageLoaderWindow = null;

                // Clean up textures
                Deactivate();

                // Clean up the image loader thread context
                try
                {
                    IMGContext?.Dispose();
                }
                catch { }
            }
            base.Dispose(disposing);
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

        private Dictionary<string, int> wpLabelTextures = new Dictionary<string, int>();

        private int getWpLabelTexture(string label)
        {
            if (wpLabelTextures.ContainsKey(label))
                return wpLabelTextures[label];

            // Create a bitmap with the waypoint label
            int size = 128;
            using (Bitmap bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    using (Font font = new Font("Arial", 48, FontStyle.Bold))
                    using (StringFormat sf = new StringFormat())
                    {
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;

                        // Draw white text with black outline
                        RectangleF rect = new RectangleF(0, 0, size, size);

                        // Draw outline
                        using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            path.AddString(label, font.FontFamily, (int)font.Style, font.Size,
                                rect, sf);
                            using (Pen outlinePen = new Pen(Color.Black, 6))
                            {
                                outlinePen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                                g.DrawPath(outlinePen, path);
                            }
                            g.FillPath(Brushes.White, path);
                        }
                    }
                }

                int texture = generateTexture(new Bitmap(bmp));
                wpLabelTextures[label] = texture;
                return texture;
            }
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

        private void LoadPlaneSTL()
        {
            if (_planeLoaded) return;
            if (!_settingsLoaded) return; // Wait for settings to load first

            try
            {
                string stlContent;
                if (!string.IsNullOrEmpty(_planeSTLPath) && File.Exists(_planeSTLPath))
                {
                    // Load from custom file path
                    stlContent = File.ReadAllText(_planeSTLPath);
                }
                else
                {
                    // Load from embedded resource
                    stlContent = MissionPlanner.Properties.Resources.plane_stl;
                }

                if (string.IsNullOrEmpty(stlContent))
                {
                    MessageBox.Show("plane.stl resource not found", "STL Load Error");
                    return;
                }

                _planeVertices = new List<float>();
                _planeNormals = new List<float>();

                float minX = float.MaxValue, maxX = float.MinValue;
                float minY = float.MaxValue, maxY = float.MinValue;
                float minZ = float.MaxValue, maxZ = float.MinValue;

                var lines = stlContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                float nx = 0, ny = 0, nz = 0;

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("facet normal"))
                    {
                        var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        nx = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                        ny = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
                        nz = float.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else if (trimmed.StartsWith("vertex"))
                    {
                        var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        float vx = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                        float vy = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                        float vz = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);

                        _planeVertices.Add(vx);
                        _planeVertices.Add(vy);
                        _planeVertices.Add(vz);

                        _planeNormals.Add(nx);
                        _planeNormals.Add(ny);
                        _planeNormals.Add(nz);

                        minX = Math.Min(minX, vx); maxX = Math.Max(maxX, vx);
                        minY = Math.Min(minY, vy); maxY = Math.Max(maxY, vy);
                        minZ = Math.Min(minZ, vz); maxZ = Math.Max(maxZ, vz);
                    }
                }

                // Calculate scale to make model 1 meter wide
                float modelWidth = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));
                _planeScale = 1.0f / modelWidth; // 1 meter wide

                // Center the model
                float centerX = (minX + maxX) / 2;
                float centerY = (minY + maxY) / 2;
                float centerZ = (minZ + maxZ) / 2;

                for (int i = 0; i < _planeVertices.Count; i += 3)
                {
                    _planeVertices[i] -= centerX;
                    _planeVertices[i + 1] -= centerY;
                    _planeVertices[i + 2] -= centerZ;
                }

                RotatePlane();

                _planeVertexCount = _planeVertices.Count / 3;
                _planeLoaded = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading plane.stl: " + ex.Message, "STL Load Error");
            }
        }

        private void RotatePlane()
        {
            // Rotate STL -90 degrees around Z axis (swap X and Y, negate new Y)
            for (int i = 0; i < _planeVertices.Count; i += 3)
            {
                float x = _planeVertices[i];
                float y = _planeVertices[i + 1];
                _planeVertices[i] = y;
                _planeVertices[i + 1] = -x;
            }
            // Also rotate normals
            for (int i = 0; i < _planeNormals.Count; i += 3)
            {
                float normX = _planeNormals[i];
                float normY = _planeNormals[i + 1];
                _planeNormals[i] = normY;
                _planeNormals[i + 1] = -normX;
            }
        }

        private void DrawPlane(Matrix4 projMatrix, Matrix4 viewMatrix)
        {
            if (!_planeLoaded || _planeVertices == null || _planeVertices.Count == 0)
            {
                LoadPlaneSTL();
                if (!_planeLoaded)
                    return;
            }

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Disable(EnableCap.CullFace); // Show both sides

            // STL is in mm, _planeScale normalizes to 1 meter, then apply user scale multiplier
            float scale = _planeScale * _planeScaleMultiplier;

            // Create model matrix for the plane
            // Build in order: Scale -> Rotate -> Translate
            var planeModelMatrix = Matrix4.CreateScale(scale);

            // Rotate: roll, pitch, then yaw (negate pitch and yaw for correct direction)
            planeModelMatrix = Matrix4.Mult(planeModelMatrix, Matrix4.CreateRotationY((float)MathHelper.Radians(_planeRoll)));
            planeModelMatrix = Matrix4.Mult(planeModelMatrix, Matrix4.CreateRotationX((float)MathHelper.Radians(_planePitch)));
            planeModelMatrix = Matrix4.Mult(planeModelMatrix, Matrix4.CreateRotationZ((float)MathHelper.Radians(-_planeYaw)));

            // Translate to position
            planeModelMatrix = Matrix4.Mult(planeModelMatrix, Matrix4.CreateTranslation((float)_planeDrawX, (float)_planeDrawY, (float)_planeDrawZ));

            // Combine model with view matrix (like other objects in the scene)
            var modelViewMatrix = Matrix4.Mult(planeModelMatrix, viewMatrix);

            // Use Lines shader which supports vertex colors for shading
            GL.UseProgram(Lines.Program);

            // set matrices
            GL.UniformMatrix4(Lines.modelViewSlot, 1, false, ref modelViewMatrix.Row0.X);
            GL.UniformMatrix4(Lines.projectionSlot, 1, false, ref projMatrix.Row0.X);

            GL.EnableVertexAttribArray(Lines.positionSlot);
            GL.EnableVertexAttribArray(Lines.colorSlot);

            // Light direction in model space (sun from above-right-front)
            // We need to transform this by inverse of model rotation to get consistent lighting
            float lightX = 0.4f, lightY = 0.6f, lightZ = 0.7f;
            float lightLen = (float)Math.Sqrt(lightX * lightX + lightY * lightY + lightZ * lightZ);
            lightX /= lightLen; lightY /= lightLen; lightZ /= lightLen;

            // Build vertex array with per-vertex lighting
            var planeVerts = new List<Vertex>();
            for (int i = 0; i < _planeVertices.Count; i += 3)
            {
                float vx = _planeVertices[i];
                float vy = _planeVertices[i + 1];
                float vz = _planeVertices[i + 2];

                // Get normal for this vertex
                float nx = _planeNormals[i];
                float ny = _planeNormals[i + 1];
                float nz = _planeNormals[i + 2];

                // Normalize
                float nlen = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
                if (nlen > 0.001f) { nx /= nlen; ny /= nlen; nz /= nlen; }

                // Compute diffuse lighting (N dot L)
                float diffuse = Math.Max(0, nx * lightX + ny * lightY + nz * lightZ);

                // Ambient + diffuse lighting
                float ambient = 0.3f;
                float light = ambient + diffuse * 0.7f;
                light = Math.Min(1.0f, light);

                // Apply plane color with shading
                float r = (_planeColor.R / 255f) * light;
                float g = (_planeColor.G / 255f) * light;
                float b = (_planeColor.B / 255f) * light;
                planeVerts.Add(new Vertex(vx, vy, vz, r, g, b, 1.0, 0, 0));
            }

            // Create temporary VBO
            int vbo;
            GL.GenBuffers(1, out vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, planeVerts.Count * Vertex.Stride, planeVerts.ToArray(), BufferUsageHint.StreamDraw);

            GL.VertexAttribPointer(Lines.positionSlot, 3, VertexAttribPointerType.Float, false, Vertex.Stride, IntPtr.Zero);
            GL.VertexAttribPointer(Lines.colorSlot, 4, VertexAttribPointerType.Float, false, Vertex.Stride, (IntPtr)(sizeof(float) * 3));

            GL.DrawArrays(BeginMode.Triangles, 0, _planeVertexCount);

            GL.DisableVertexAttribArray(Lines.positionSlot);
            GL.DisableVertexAttribArray(Lines.colorSlot);

            // Cleanup
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vbo);
        }

        private void DrawHeadingLines(Matrix4 projMatrix, Matrix4 viewMatrix)
        {
            // Get current heading (yaw), nav bearing, and GPS heading
            double heading = MainV2.comPort?.MAV?.cs?.yaw ?? 0;
            double navBearing = MainV2.comPort?.MAV?.cs?.nav_bearing ?? 0;
            double gpsHeading = MainV2.comPort?.MAV?.cs?.groundcourse ?? 0;

            // Heading line (red)
            if (_showHeadingLine)
            {
                double headingRad = MathHelper.Radians(heading);
                double headingEndX = _planeDrawX + Math.Sin(headingRad) * HEADING_LINE_LENGTH;
                double headingEndY = _planeDrawY + Math.Cos(headingRad) * HEADING_LINE_LENGTH;

                _headingLine?.Dispose();
                _headingLine = new Lines();
                _headingLine.Width = 1.5f;
                _headingLine.Add(_planeDrawX, _planeDrawY, _planeDrawZ, 1, 0, 0, 1);
                _headingLine.Add(headingEndX, headingEndY, _planeDrawZ, 1, 0, 0, 1);
                _headingLine.Draw(projMatrix, viewMatrix);
            }

            // Nav bearing line (orange) - draws from plane to target waypoint or nav_bearing direction
            if (_showNavBearingLine)
            {
                double navEndX, navEndY, navEndZ;
                PointLatLngAlt targetWp = null;

                // Only connect to actual waypoint in Auto, Guided, or RTL modes
                // In other modes, just draw a directional line like the 2D map does
                var mode = MainV2.comPort?.MAV?.cs?.mode?.ToLower() ?? "";
                bool isNavigatingMode = mode == "auto" || mode == "guided" || mode == "rtl" ||
                                        mode == "land" || mode == "smart_rtl";

                if (isNavigatingMode)
                {
                    if (mode == "guided" && MainV2.comPort?.MAV?.GuidedMode.x != 0)
                    {
                        // In guided mode, target is the guided waypoint
                        targetWp = new PointLatLngAlt(MainV2.comPort.MAV.GuidedMode)
                            { Alt = MainV2.comPort.MAV.GuidedMode.z + MainV2.comPort.MAV.cs.HomeAlt };
                    }
                    else if (mode == "rtl" || mode == "land" || mode == "smart_rtl")
                    {
                        // In RTL/Land modes, target is Home
                        var pointlist = FlightPlanner.instance?.pointlist?.Where(a => a != null).ToList();
                        targetWp = pointlist?.FirstOrDefault(p => p.Tag == "H");
                    }
                    else if (mode == "auto")
                    {
                        // Auto mode - use current waypoint number
                        int wpno = (int)(MainV2.comPort?.MAV?.cs?.wpno ?? 0);
                        var pointlist = FlightPlanner.instance?.pointlist?.Where(a => a != null).ToList();
                        if (pointlist != null && wpno > 0 && wpno < pointlist.Count)
                            targetWp = pointlist[wpno];
                    }
                }

                if (targetWp != null && targetWp.Lat != 0 && targetWp.Lng != 0)
                {
                    // Calculate position exactly as waypoint markers are drawn
                    var co = convertCoords(targetWp);
                    var targetTerrainAlt = srtm.getAltitude(targetWp.Lat, targetWp.Lng).alt;
                    navEndX = co[0];
                    navEndY = co[1];
                    navEndZ = co[2] + targetTerrainAlt;
                }
                else
                {
                    // Not in navigation mode or no target: use nav_bearing direction with fixed length (like 2D map)
                    double navBearingRad = MathHelper.Radians(navBearing);
                    navEndX = _planeDrawX + Math.Sin(navBearingRad) * HEADING_LINE_LENGTH;
                    navEndY = _planeDrawY + Math.Cos(navBearingRad) * HEADING_LINE_LENGTH;
                    navEndZ = _planeDrawZ;
                }

                _navBearingLine?.Dispose();
                _navBearingLine = new Lines();
                _navBearingLine.Width = 1.5f;
                _navBearingLine.Add(_planeDrawX, _planeDrawY, _planeDrawZ, 1, 0.5f, 0, 1);
                _navBearingLine.Add(navEndX, navEndY, navEndZ, 1, 0.5f, 0, 1);
                _navBearingLine.Draw(projMatrix, viewMatrix);
            }

            // GPS heading line (black)
            if (_showGpsHeadingLine)
            {
                double gpsRad = MathHelper.Radians(gpsHeading);
                double gpsEndX = _planeDrawX + Math.Sin(gpsRad) * HEADING_LINE_LENGTH;
                double gpsEndY = _planeDrawY + Math.Cos(gpsRad) * HEADING_LINE_LENGTH;

                _gpsHeadingLine?.Dispose();
                _gpsHeadingLine = new Lines();
                _gpsHeadingLine.Width = 1.5f;
                _gpsHeadingLine.Add(_planeDrawX, _planeDrawY, _planeDrawZ, 0, 0, 0, 1);
                _gpsHeadingLine.Add(gpsEndX, gpsEndY, _planeDrawZ, 0, 0, 0, 1);
                _gpsHeadingLine.Draw(projMatrix, viewMatrix);
            }

            // Turn radius arc (hot pink) - shows predicted turn path based on current bank angle
            if (_showTurnRadius)
            {
                float radius = (float)(MainV2.comPort?.MAV?.cs?.radius ?? 0);

                if (Math.Abs(radius) > 1)
                {
                    double alpha = (TURN_RADIUS_ARC_LENGTH / Math.Abs(radius)) * MathHelper.rad2deg;
                    if (alpha > 180) alpha = 180;

                    // Calculate center of turn circle perpendicular to travel direction
                    double cogRad = MathHelper.Radians(gpsHeading);
                    double perpAngle = cogRad + (radius > 0 ? Math.PI / 2 : -Math.PI / 2);
                    double centerX = _planeDrawX + Math.Sin(perpAngle) * Math.Abs(radius);
                    double centerY = _planeDrawY + Math.Cos(perpAngle) * Math.Abs(radius);
                    double startAngle = Math.Atan2(_planeDrawX - centerX, _planeDrawY - centerY);

                    _turnRadiusLine?.Dispose();
                    _turnRadiusLine = new Lines();
                    _turnRadiusLine.Width = 2f;

                    // HotPink color
                    float r = 1.0f;
                    float g = 105f / 255f;
                    float b = 180f / 255f;

                    double alphaRad = MathHelper.Radians(alpha);
                    double angleStep = alphaRad / TURN_RADIUS_SEGMENTS;
                    double direction = radius > 0 ? 1 : -1;

                    double prevX = _planeDrawX;
                    double prevY = _planeDrawY;

                    for (int i = 1; i <= TURN_RADIUS_SEGMENTS; i++)
                    {
                        double angle = startAngle + direction * angleStep * i;
                        double x = centerX + Math.Sin(angle) * Math.Abs(radius);
                        double y = centerY + Math.Cos(angle) * Math.Abs(radius);

                        _turnRadiusLine.Add(prevX, prevY, _planeDrawZ, r, g, b, 1);
                        _turnRadiusLine.Add(x, y, _planeDrawZ, r, g, b, 1);

                        prevX = x;
                        prevY = y;
                    }

                    _turnRadiusLine.Draw(projMatrix, viewMatrix);
                }
            }
        }

        private void DrawTrail(Matrix4 projMatrix, Matrix4 viewMatrix)
        {
            if (!_showTrail || MainV2.comPort?.MAV?.cs?.armed != true || _trailPoints.Count < 2)
                return;

            _trailLine?.Dispose();
            _trailLine = new Lines();
            _trailLine.Width = 5f;

            // MidnightBlue color with alpha (same as 2D map GMapRoute default)
            float r = 25f / 255f;
            float g = 25f / 255f;
            float b = 112f / 255f;
            float a = 144f / 255f;

            // Convert trail points to relative coordinates
            var relPoints = new List<double[]>();
            foreach (var pt in _trailPoints)
            {
                relPoints.Add(new double[] { pt[0] - utmcenter[0], pt[1] - utmcenter[1], pt[2] });
            }
            // Add current plane position
            relPoints.Add(new double[] { _planeDrawX, _planeDrawY, _planeDrawZ });

            // Apply moving average smoothing
            int windowSize = TRAIL_SMOOTHING_WINDOW;
            var smoothed = new List<double[]>();

            for (int i = 0; i < relPoints.Count; i++)
            {
                double sumX = 0, sumY = 0, sumZ = 0;
                int count = 0;
                int halfWindow = windowSize / 2;

                for (int j = Math.Max(0, i - halfWindow); j <= Math.Min(relPoints.Count - 1, i + halfWindow); j++)
                {
                    sumX += relPoints[j][0];
                    sumY += relPoints[j][1];
                    sumZ += relPoints[j][2];
                    count++;
                }

                smoothed.Add(new double[] { sumX / count, sumY / count, sumZ / count });
            }

            // Draw smoothed points, but force the last point to be exact plane position
            for (int i = 0; i < smoothed.Count - 1; i++)
            {
                _trailLine.Add(smoothed[i][0], smoothed[i][1], smoothed[i][2], r, g, b, a);
            }
            // Last point is always the exact plane position
            _trailLine.Add(_planeDrawX, _planeDrawY, _planeDrawZ, r, g, b, a);

            _trailLine.Draw(projMatrix, viewMatrix);
        }

        /// <summary>
        /// Clears the 3D map trail. Called from FlightData when clearing the 2D track.
        /// </summary>
        public void ClearTrail()
        {
            _trailPoints.Clear();
        }

        /// <summary>
        /// Draws ADSB aircraft as circles on the 3D map.
        /// ADSB altitude is MSL (barometric), so no terrain adjustment needed.
        /// Circle diameter is fixed at 250m (25m for grounded aircraft).
        /// Color based on distance from own aircraft:
        /// Red if within 5km, Yellow if within 20km, Green if > 50km, interpolated between.
        /// Grounded aircraft are drawn as light gray circles.
        /// Circles are billboarded to always face the camera.
        /// Drawn in reverse order of distance (farthest first) for proper depth ordering.
        /// </summary>
        private void DrawADSB(Matrix4 projMatrix, Matrix4 viewMatrix)
        {
            double circleRadius = _adsbCircleSize / 2.0;
            double groundedCircleRadius = circleRadius / 10.0;

            var ownPosition = MainV2.comPort?.MAV?.cs?.Location ?? PointLatLngAlt.Zero;

            _adsbScreenPositions.Clear();

            var planeList = new List<Tuple<adsb.PointLatLngAltHdg, double>>();

            lock (MainV2.instance.adsblock)
            {
                foreach (var kvp in MainV2.instance.adsbPlanes)
                {
                    var plane = kvp.Value;
                    if (plane == null)
                        continue;

                    if (plane.Time < DateTime.Now.AddSeconds(-30))
                        continue;

                    if (plane.Lat == 0 && plane.Lng == 0)
                        continue;

                    var plla = new PointLatLngAlt(plane.Lat, plane.Lng, plane.Alt);
                    double distanceToOwn = ownPosition.GetDistance(plla);

                    if (distanceToOwn > ADSB_MAX_DISTANCE)
                        continue;

                    planeList.Add(Tuple.Create(plane, distanceToOwn));
                }
            }

            // Sort by distance descending (farthest first) so closer planes render on top
            planeList.Sort((a, b) => b.Item2.CompareTo(a.Item2));

            var circleVertices = new List<float>();

            foreach (var item in planeList)
            {
                var plane = item.Item1;
                double distanceToOwn = item.Item2;

                var plla = new PointLatLngAlt(plane.Lat, plane.Lng, plane.Alt);
                var co = convertCoords(plla);

                bool isGrounded = plane.IsOnGround;
                double radius = isGrounded ? groundedCircleRadius : circleRadius;

                var color = GetADSBDistanceColor(distanceToOwn, isGrounded);

                var billboard = CalculateBillboardOrientation(co[0], co[1], co[2], cameraX, cameraY, cameraZ);
                if (!billboard.HasValue)
                    continue;

                // Calculate distance to camera for screen position calculation
                double dx = co[0] - cameraX;
                double dy = co[1] - cameraY;
                double dz = co[2] - cameraZ;
                double distanceToCamera = Math.Sqrt(dx * dx + dy * dy + dz * dz);

                // Store screen position for hit testing
                var worldPos = new Vector4((float)co[0], (float)co[1], (float)co[2], 1.0f);
                var clipPos = Vector4.Transform(worldPos, viewMatrix * projMatrix);
                if (clipPos.W > 0)
                {
                    float ndcX = clipPos.X / clipPos.W;
                    float ndcY = clipPos.Y / clipPos.W;
                    float screenX = (ndcX + 1.0f) * 0.5f * Width;
                    float screenY = (1.0f - ndcY) * 0.5f * Height;

                    double fovRad = _cameraFOV * MathHelper.deg2rad;
                    float screenRadius = (float)((radius / distanceToCamera) * (Height / 2.0) / Math.Tan(fovRad / 2.0));

                    _adsbScreenPositions.Add(new ADSBScreenPosition
                    {
                        ScreenX = screenX,
                        ScreenY = screenY,
                        Radius = Math.Max(screenRadius, 20),
                        PlaneData = plane,
                        DistanceToOwn = distanceToOwn
                    });
                }

                var (rightX, rightY, rightZ, upX, upY, upZ) = billboard.Value;
                AddBillboardCircleVertices(
                    co[0], co[1], co[2],
                    radius, ADSB_CIRCLE_SEGMENTS,
                    color.r, color.g, color.b, color.a,
                    rightX, rightY, rightZ, upX, upY, upZ,
                    circleVertices);
            }

            if (circleVertices.Count > 0)
            {
                DrawADSBCircles(projMatrix, viewMatrix, circleVertices.ToArray());
            }
        }

        /// <summary>
        /// Draws ADSB circles using GL.Lines primitive (not LineStrip) so circles are not connected.
        /// </summary>
        private void DrawADSBCircles(Matrix4 projMatrix, Matrix4 viewMatrix, float[] vertices)
        {
            if (Lines.Program == 0)
                return;

            GL.UseProgram(Lines.Program);

            GL.EnableVertexAttribArray(Lines.positionSlot);
            GL.EnableVertexAttribArray(Lines.colorSlot);

            GL.UniformMatrix4(Lines.modelViewSlot, 1, false, ref viewMatrix.Row0.X);
            GL.UniformMatrix4(Lines.projectionSlot, 1, false, ref projMatrix.Row0.X);

            GL.LineWidth(3f);

            // Create and bind vertex buffer
            int vbo;
            GL.GenBuffers(1, out vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            // Stride: 7 floats per vertex (x, y, z, r, g, b, a)
            int stride = 7 * sizeof(float);
            GL.VertexAttribPointer(Lines.positionSlot, 3, VertexAttribPointerType.Float, false, stride, IntPtr.Zero);
            GL.VertexAttribPointer(Lines.colorSlot, 4, VertexAttribPointerType.Float, false, stride, (IntPtr)(3 * sizeof(float)));

            // Draw as GL.Lines (pairs of vertices form separate lines)
            GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Length / 7);

            GL.DisableVertexAttribArray(Lines.positionSlot);
            GL.DisableVertexAttribArray(Lines.colorSlot);

            // Clean up
            GL.DeleteBuffers(1, ref vbo);
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
            _imageLoaderWindow = new OpenTK.GameWindow(640, 480, Context.GraphicsMode);
            _imageLoaderWindow.Visible = false;
            IMGContext = _imageLoaderWindow.Context;
            core.Zoom = 20;

            while (!_stopRequested && !this.IsDisposed)
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
        private OpenTK.GameWindow _imageLoaderWindow;
        private int mousex;
        private int mousey;
        private bool _isDragging = false;
        private int _dragStartX;
        private int _dragStartY;
        private IGraphicsContext IMGContext;
        private bool started;
        private bool onpaintrun;
        private bool sizeChanged;
        private double[] mypos = new double[3];
        Vector3 myrpy = Vector3.UnitX;
        private bool fogon = false;
        private Lines _flightPlanLines;
        private int _flightPlanLinesCount = -1;
        private int _flightPlanLinesHash = 0;
        private DateTime _centerTime;
        private List<tileZoomArea> tileArea = new List<tileZoomArea>();

        // Kalman filters for smooth position and rotation interpolation
        private SimpleKalmanFilter _kalmanPosX = new SimpleKalmanFilter(0.05, 0.5);
        private SimpleKalmanFilter _kalmanPosY = new SimpleKalmanFilter(0.05, 0.5);
        private SimpleKalmanFilter _kalmanPosZ = new SimpleKalmanFilter(0.05, 0.5);

        // Default ground plane (shown before tiles load)
        private Lines _defaultGround;
        // Sky gradient quad
        private Lines _skyGradient;
        // Heading and nav bearing indicator lines
        private Lines _headingLine;
        private Lines _navBearingLine;
        private Lines _gpsHeadingLine;
        private Lines _turnRadiusLine;
        private SimpleKalmanFilter _kalmanRoll = new SimpleKalmanFilter(0.1, 0.3);
        private SimpleKalmanFilter _kalmanPitch = new SimpleKalmanFilter(0.1, 0.3);
        private SimpleKalmanFilter _kalmanYaw = new SimpleKalmanFilter(0.1, 0.3);
        private bool _kalmanInitialized = false;

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
                // Check if connected - stop updating plane position/camera when disconnected
                bool isConnected = MainV2.comPort?.BaseStream?.IsOpen == true;

                double heightscale = 1; //(step/90.0)*5;
                var campos = convertCoords(_center);

                // Only update positions from Kalman filter when connected
                if (isConnected)
                {
                    campos = projectLocation(mypos);
                    // Apply Kalman filter to rotation for smooth interpolation
                    var rpy = filterRotation(this.rpy);

                    // save the state
                    mypos = campos;
                    myrpy = new OpenTK.Vector3((float) rpy.x, (float) rpy.y, (float) rpy.z);

                    // Plane position (where camera used to be)
                    _planeDrawX = campos[0];
                    _planeDrawY = campos[1];
                    _planeDrawZ = (campos[2] < srtm.getAltitude(_center.Lat, _center.Lng).alt)
                        ? (srtm.getAltitude(_center.Lat, _center.Lng).alt + 1) * heightscale
                        : _center.Alt * heightscale;

                    // Store plane rotation
                    _planeRoll = (float)rpy.X;
                    _planePitch = (float)rpy.Y;
                    _planeYaw = (float)rpy.Z;

                    // Update trail points using the smoothly rendered plane position (every frame)
                    // This gives us smooth trails since _planeDrawX/Y/Z is already Kalman filtered
                    if (_showTrail && MainV2.comPort?.MAV?.cs?.armed == true && _center.Lat != 0 && _center.Lng != 0)
                    {
                        // Store absolute UTM coordinates (add back utmcenter offset)
                        double absX = _planeDrawX + utmcenter[0];
                        double absY = _planeDrawY + utmcenter[1];
                        double absZ = _planeDrawZ;

                        // Clear trail if UTM zone changed
                        if (_trailUtmZone != utmzone)
                        {
                            _trailPoints.Clear();
                            _trailUtmZone = utmzone;
                        }

                        // Add point every frame - the positions are already smooth from Kalman filter
                        // Use a larger point count since we're adding every frame (~30fps)
                        int numTrackLength = Settings.Instance.GetInt32("NUM_tracklength", 200) * 15; // 3000 points for smooth 3D trail
                        if (_trailPoints.Count > numTrackLength)
                            _trailPoints.RemoveRange(0, _trailPoints.Count - numTrackLength);
                        _trailPoints.Add(new double[] { absX, absY, absZ });
                    }

                    // Camera orbits around plane at _cameraDist, offset by _cameraAngle from behind
                    double cameraAngleRad = MathHelper.Radians(rpy.Z + _cameraAngle + 180); // +180 to start behind plane
                    cameraX = _planeDrawX + Math.Sin(cameraAngleRad) * _cameraDist;
                    cameraY = _planeDrawY + Math.Cos(cameraAngleRad) * _cameraDist;
                    cameraZ = _planeDrawZ + _cameraHeight;

                    // Look at the plane
                    lookX = _planeDrawX;
                    lookY = _planeDrawY;
                    lookZ = _planeDrawZ;
                }
                if (!Context.IsCurrent)
                    Context.MakeCurrent(this.WindowInfo);
                /*Console.WriteLine("cam: {0} {1} {2} lookat: {3} {4} {5}", (float) cameraX, (float) cameraY, (float) cameraZ,
                    (float) lookX,
                    (float) lookY, (float) lookZ);
                  */
                modelMatrix = Matrix4.LookAt((float) cameraX, (float) cameraY, (float) cameraZ,
                    (float) lookX, (float) lookY, (float) lookZ,
                    0, 0, 1);

                // Update projection matrix based on altitude - 100km render distance when >500m altitude
                float renderDistance = _center.Alt > 500 ? 100000f : 50000f;
                projMatrix = OpenTK.Matrix4.CreatePerspectiveFieldOfView(
                    (float) (_cameraFOV * MathHelper.deg2rad),
                    (float) Width / Height, 0.1f,
                    renderDistance);

                {
                    // for unproject - updated on every draw
                    GL.GetInteger(GetPName.Viewport, viewport);
                }


                var beforeclear = DateTime.Now;
                //GL.Viewport(0, 0, Width, Height);
                // Clear depth and draw sky gradient
                GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.AccumBufferBit);

                // Draw sky gradient background (uses theme colors)
                GL.Disable(EnableCap.DepthTest);
                DrawSkyGradient();

                // disable depth during terrain draw
                GL.Disable(EnableCap.DepthTest);
                GL.DepthFunc(DepthFunction.Lequal);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.BlendEquation(BlendEquationMode.FuncAdd);

                // Draw ADSB aircraft circles (before tiles so they appear behind terrain)
                DrawADSB(projMatrix, modelMatrix);

                // Draw default green ground plane (visible before tiles load)
                var texlist = textureid.ToArray().ToSortedList(Comparison);
                if (texlist.Count == 0)
                {
                    // No tiles loaded yet - draw a large green ground plane
                    DrawDefaultGround(projMatrix, modelMatrix);
                }
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
                    var pointlistCount = FlightPlanner.instance.pointlist.Count;
                    if (pointlistCount > 1)
                    {
                        var currentHash = ComputeWaypointHash(FlightPlanner.instance.pointlist);
                        // Only rebuild lines if pointlist changed
                        if (_flightPlanLines == null || _flightPlanLinesCount != pointlistCount || _flightPlanLinesHash != currentHash)
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
                                // Add terrain altitude to waypoint altitude
                                var terrainAlt = srtm.getAltitude(point.Lat, point.Lng).alt;
                                _flightPlanLines.Add(co[0], co[1], co[2] + terrainAlt, 1, 1, 0, 1);
                            }
                            _flightPlanLinesCount = pointlistCount;
                            _flightPlanLinesHash = currentHash;
                        }

                        _flightPlanLines.Draw(projMatrix, modelMatrix);
                    }
                }
                // Only draw plane and indicators when connected
                if (isConnected)
                {
                    // Draw the plane model
                    DrawPlane(projMatrix, modelMatrix);

                    // Draw heading (red) and nav bearing (orange) lines from plane center
                    DrawHeadingLines(projMatrix, modelMatrix);

                    // Draw flight path trail
                    DrawTrail(projMatrix, modelMatrix);
                }

                var beforewpsmarkers = DateTime.Now;
                // Draw waypoint markers (hidden if within 200ft / 61m of camera)
                {
                    if (green == 0)
                        green = generateTexture(GMap.NET.Drawing.Properties.Resources.wp_3d.ToBitmap());
                    if (greenAlt == 0)
                        greenAlt = generateTexture(GMap.NET.Drawing.Properties.Resources.wp_3d_alt.ToBitmap());

                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(false);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    GL.Enable(EnableCap.Texture2D);

                    var list = FlightPlanner.instance.pointlist.Where(a => a != null).ToList();
                    if (MainV2.comPort.MAV.cs.mode.ToLower() == "guided")
                        list.Add(new PointLatLngAlt(MainV2.comPort.MAV.GuidedMode)
                            {Alt = MainV2.comPort.MAV.GuidedMode.z + MainV2.comPort.MAV.cs.HomeAlt});
                    if (MainV2.comPort.MAV.cs.TargetLocation != PointLatLngAlt.Zero)
                        list.Add(MainV2.comPort.MAV.cs.TargetLocation);

                    // Get pointlist for wp number lookup
                    var pointlist = FlightPlanner.instance.pointlist.Where(a => a != null).ToList();

                    foreach (var point in list.OrderByDescending((a)=> a.GetDistance(MainV2.comPort.MAV.cs.Location)))
                    {
                        if (point == null)
                            continue;
                        if (point.Lat == 0 && point.Lng == 0)
                            continue;

                        // Skip markers within 200ft of the camera/vehicle
                        var distanceToCamera = point.GetDistance(_center);
                        if (distanceToCamera < WAYPOINT_MIN_DISTANCE)
                            continue;

                        var co = convertCoords(point);
                        // Add terrain altitude to waypoint altitude
                        var terrainAlt = srtm.getAltitude(point.Lat, point.Lng).alt;
                        var wpAlt = co[2] + terrainAlt;

                        // Determine label first to choose correct marker texture
                        int wpIndex = pointlist.IndexOf(point);
                        string wpLabel = null;
                        if (point.Tag == "H")
                            wpLabel = "H";
                        else if (point.Tag != null && point.Tag.StartsWith("ROI"))
                            wpLabel = "R";
                        else if (IsGuidedWaypoint(point))
                            wpLabel = "G";
                        else if (wpIndex >= 0)
                            wpLabel = wpIndex.ToString();

                        // Use alt texture for non-number labels (H, R, G, etc.)
                        bool isSpecialLabel = wpLabel != null && !char.IsDigit(wpLabel[0]);

                        var wpmarker = new tileInfo(Context, WindowInfo, textureSemaphore);
                        wpmarker.idtexture = isSpecialLabel ? greenAlt : green;

                        double markerHalfSize = _waypointMarkerSize;
                        // Calculate angle from waypoint to camera so marker always faces camera
                        double dx = cameraX - co[0];
                        double dy = cameraY - co[1];
                        double angleToCamera = Math.Atan2(dx, dy);
                        double sinAngle = Math.Sin(angleToCamera + Math.PI / 2);
                        double cosAngle = Math.Cos(angleToCamera + Math.PI / 2);

                        // Rotation around the axis facing the camera (perpendicular to billboard)
                        double rotationAngle = (DateTime.Now.TimeOfDay.TotalSeconds * 30.0) % 360.0;
                        double rotRad = MathHelper.Radians(rotationAngle);
                        double cosRot = Math.Cos(rotRad);
                        double sinRot = Math.Sin(rotRad);

                        // Corner offsets in local 2D space (horizontal, vertical)
                        // tr: (+1, +1), tl: (-1, +1), br: (+1, -1), bl: (-1, -1)
                        double[][] corners = new double[][] {
                            new double[] { 1, 1, 1, 0 },   // tr + tex coords (flipped U)
                            new double[] { -1, 1, 0, 0 },  // tl
                            new double[] { 1, -1, 1, 1 },  // br
                            new double[] { -1, -1, 0, 1 }  // bl
                        };

                        foreach (var corner in corners)
                        {
                            // Rotate in local 2D space (horizontal = along billboard width, vertical = along Z)
                            double localH = corner[0] * cosRot - corner[1] * sinRot;
                            double localV = corner[0] * sinRot + corner[1] * cosRot;

                            // Transform to world coordinates
                            wpmarker.vertex.Add(new Vertex(
                                co[0] + sinAngle * localH * markerHalfSize,
                                co[1] + cosAngle * localH * markerHalfSize,
                                wpAlt + localV * markerHalfSize,
                                0, 0, 0, 1, corner[2], corner[3]));
                        }

                        var startindex = (uint)wpmarker.vertex.Count - 4;
                        wpmarker.indices.AddRange(new[]
                                        {
                                startindex + 1, startindex + 2, startindex + 0,
                                startindex + 1, startindex + 3, startindex + 2
                            });


                        wpmarker.Draw(projMatrix, modelMatrix);

                        wpmarker.Cleanup(true);

                        // Draw waypoint label at top of sprite (no rotation)
                        if (wpLabel != null)
                        {
                            int wpNumberTex = getWpLabelTexture(wpLabel);
                            if (wpNumberTex != 0)
                            {
                                var wpnumber = new tileInfo(Context, WindowInfo, textureSemaphore);
                                wpnumber.idtexture = wpNumberTex;

                                // H, R, G labels are centered, numbers are at top
                                bool centerLabel = isSpecialLabel;
                                double numberHalfSize = centerLabel ? markerHalfSize * 0.6 : markerHalfSize * 0.4;
                                double numberOffsetZ = centerLabel ? 0 : markerHalfSize * 0.5;

                                // Static corners (no rotation applied), shifted up for numbers
                                // Flip horizontally by negating corner[0] to unmirror the number
                                foreach (var corner in corners)
                                {
                                    wpnumber.vertex.Add(new Vertex(
                                        co[0] - sinAngle * corner[0] * numberHalfSize,
                                        co[1] - cosAngle * corner[0] * numberHalfSize,
                                        wpAlt + corner[1] * numberHalfSize + numberOffsetZ,
                                        0, 0, 0, 1, corner[2], corner[3]));
                                }

                                var numStartindex = (uint)wpnumber.vertex.Count - 4;
                                wpnumber.indices.AddRange(new[]
                                {
                                    numStartindex + 1, numStartindex + 2, numStartindex + 0,
                                    numStartindex + 1, numStartindex + 3, numStartindex + 2
                                });

                                wpnumber.Draw(projMatrix, modelMatrix);
                                wpnumber.Cleanup(true);
                            }
                        }
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

            // Initialize Kalman filters on first run
            if (!_kalmanInitialized)
            {
                _kalmanPosX.Reset(newpos[0]);
                _kalmanPosY.Reset(newpos[1]);
                _kalmanPosZ.Reset(newpos[2]);
                _kalmanInitialized = true;
            }

            // Use Kalman filter for smooth interpolation
            return new double[]
            {
                _kalmanPosX.Update(newpos[0]),
                _kalmanPosY.Update(newpos[1]),
                _kalmanPosZ.Update(newpos[2])
            };
        }

        private bool _rotationKalmanInitialized = false;
        private MissionPlanner.Utilities.Vector3 filterRotation(MissionPlanner.Utilities.Vector3 rawRpy)
        {
            // Initialize rotation Kalman filters on first run to prevent slow rotation from 0
            if (!_rotationKalmanInitialized)
            {
                _kalmanRoll.Reset(rawRpy.X);
                _kalmanPitch.Reset(rawRpy.Y);
                _kalmanYaw.Reset(rawRpy.Z);
                _rotationKalmanInitialized = true;
            }

            // Handle yaw wraparound for smooth interpolation
            double yaw = rawRpy.Z;
            double currentYaw = _kalmanYaw.Value;

            // Normalize yaw difference to prevent jumps at 0/360 boundary
            double yawDiff = yaw - currentYaw;
            if (yawDiff > 180) yaw -= 360;
            else if (yawDiff < -180) yaw += 360;

            return new MissionPlanner.Utilities.Vector3(
                (float)_kalmanRoll.Update(rawRpy.X),
                (float)_kalmanPitch.Update(rawRpy.Y),
                (float)_kalmanYaw.Update(yaw)
            );
        }

        private int Comparison(KeyValuePair<GPoint, tileInfo> x, KeyValuePair<GPoint, tileInfo> y)
        {
            return x.Value.zoom.CompareTo(y.Value.zoom);
        }

        private bool IsGuidedWaypoint(PointLatLngAlt point)
        {
            var guided = MainV2.comPort?.MAV?.GuidedMode;
            if (guided == null || guided.Value.x == 0 && guided.Value.y == 0)
                return false;

            double guidedLat = guided.Value.x / 1e7;
            double guidedLng = guided.Value.y / 1e7;

            const double tolerance = 0.0001;
            return Math.Abs(point.Lat - guidedLat) < tolerance &&
                   Math.Abs(point.Lng - guidedLng) < tolerance;
        }

        private void DrawSkyGradient()
        {
            // Draw a full-screen quad with gradient colors from ThemeManager
            // Top half: gradient from skyTop to skyBot
            // Bottom half: solid skyBot color
            var orthoProj = Matrix4.CreateOrthographicOffCenter(0, Width, 0, Height, -1, 1);
            var identity = Matrix4.Identity;

            // Get sky colors from theme (with fallback defaults if theme not yet loaded)
            Color skyTop = ThemeManager.HudSkyTop.A > 0 ? ThemeManager.HudSkyTop : Color.Blue;
            Color skyBot = ThemeManager.HudSkyBot.A > 0 ? ThemeManager.HudSkyBot : Color.LightBlue;

            // Lighten colors by 2x to match HUD appearance (blend towards white)
            float topR = Math.Min(1.0f, (skyTop.R / 255f) * 0.5f + 0.5f);
            float topG = Math.Min(1.0f, (skyTop.G / 255f) * 0.5f + 0.5f);
            float topB = Math.Min(1.0f, (skyTop.B / 255f) * 0.5f + 0.5f);
            float botR = Math.Min(1.0f, (skyBot.R / 255f) * 0.5f + 0.5f);
            float botG = Math.Min(1.0f, (skyBot.G / 255f) * 0.5f + 0.5f);
            float botB = Math.Min(1.0f, (skyBot.B / 255f) * 0.5f + 0.5f);

            float midY = Height / 2f;

            // Use Lines shader which supports vertex colors
            GL.UseProgram(Lines.Program);
            GL.UniformMatrix4(Lines.modelViewSlot, 1, false, ref identity.Row0.X);
            GL.UniformMatrix4(Lines.projectionSlot, 1, false, ref orthoProj.Row0.X);

            // Create vertices: bottom half solid + top half gradient
            var skyVerts = new List<Vertex>();
            // Bottom half - solid skyBot color (vertices 0-3)
            skyVerts.Add(new Vertex(0, 0, 0, botR, botG, botB, 1.0, 0, 0));      // Bottom-left
            skyVerts.Add(new Vertex(Width, 0, 0, botR, botG, botB, 1.0, 0, 0));  // Bottom-right
            skyVerts.Add(new Vertex(0, midY, 0, botR, botG, botB, 1.0, 0, 0));   // Mid-left
            skyVerts.Add(new Vertex(Width, midY, 0, botR, botG, botB, 1.0, 0, 0)); // Mid-right
            // Top half - gradient from skyBot to skyTop (vertices 4-5)
            skyVerts.Add(new Vertex(0, Height, 0, topR, topG, topB, 1.0, 0, 0)); // Top-left
            skyVerts.Add(new Vertex(Width, Height, 0, topR, topG, topB, 1.0, 0, 0)); // Top-right

            // Upload vertices
            int vbo;
            GL.GenBuffers(1, out vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            var vertArray = skyVerts.ToArray();
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertArray.Length * Vertex.Stride), vertArray, BufferUsageHint.StreamDraw);

            // Set up vertex attributes
            GL.EnableVertexAttribArray(Lines.positionSlot);
            GL.VertexAttribPointer(Lines.positionSlot, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);
            GL.EnableVertexAttribArray(Lines.colorSlot);
            GL.VertexAttribPointer(Lines.colorSlot, 4, VertexAttribPointerType.Float, false, Vertex.Stride, 12);

            // Draw as triangle strip: 0-1-2-3 (bottom half), then 2-3-4-5 (top half)
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 6);

            // Cleanup
            GL.DisableVertexAttribArray(Lines.positionSlot);
            GL.DisableVertexAttribArray(Lines.colorSlot);
            GL.DeleteBuffer(vbo);
        }

        private void DrawDefaultGround(Matrix4 projection, Matrix4 modelView)
        {
            // Create a large green ground plane centered at camera position
            if (_defaultGround == null)
            {
                _defaultGround = new Lines();
            }

            // Draw a simple colored ground quad using the existing tileInfo infrastructure
            // This creates a large flat green surface at altitude 0
            float groundSize = 2000f; // meters
            float groundAlt = 0f;

            // Dark grass green color
            float r = 0.2f, g = 0.45f, b = 0.15f, a = 1.0f;

            var groundTile = new tileInfo(Context, WindowInfo, textureSemaphore);

            // Create a quad (two triangles)
            // Bottom-left
            groundTile.vertex.Add(new Vertex(-groundSize, -groundSize, groundAlt, r, g, b, a, 0, 0));
            // Top-left
            groundTile.vertex.Add(new Vertex(-groundSize, groundSize, groundAlt, r, g, b, a, 0, 1));
            // Bottom-right
            groundTile.vertex.Add(new Vertex(groundSize, -groundSize, groundAlt, r, g, b, a, 1, 0));
            // Top-right
            groundTile.vertex.Add(new Vertex(groundSize, groundSize, groundAlt, r, g, b, a, 1, 1));

            // Two triangles: 0-1-2 and 1-3-2
            groundTile.indices.AddRange(new uint[] { 0, 1, 2, 1, 3, 2 });

            // Draw without texture
            GL.Disable(EnableCap.Texture2D);
            groundTile.Draw(projection, modelView);
            groundTile.Cleanup(true);
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
                for (int a = zoom; a >= minzoom; a--)
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

                                        // Calculate grid dimensions for altitude caching
                                        int gridWidth = (int)((xend - xstart) / pxstep) + 1;
                                        int gridHeight = (int)((yend - ystart) / pxstep) + 1;

                                        // Pre-cache all lat/lng coordinates and altitudes for this tile
                                        var latlngGrid = new PointLatLng[gridWidth, gridHeight];
                                        var altCache = new double[gridWidth, gridHeight];
                                        var utmCache = new double[gridWidth, gridHeight][];
                                        bool hasInvalidAlt = false;

                                        // First pass: compute all coordinates
                                        for (int gx = 0; gx < gridWidth && !hasInvalidAlt; gx++)
                                        {
                                            long px = xstart + gx * pxstep;
                                            for (int gy = 0; gy < gridHeight; gy++)
                                            {
                                                long py = ystart + gy * pxstep;
                                                latlngGrid[gx, gy] = prj.FromPixelToLatLng(px, py, tilearea.zoom);
                                            }
                                        }

                                        // Second pass: batch fetch altitudes using fast method
                                        for (int gx = 0; gx < gridWidth && !hasInvalidAlt; gx++)
                                        {
                                            for (int gy = 0; gy < gridHeight; gy++)
                                            {
                                                var latlng = latlngGrid[gx, gy];
                                                var altResult = srtm.getAltitudeFast(latlng.Lat, latlng.Lng);
                                                if (altResult.currenttype == srtm.tiletype.invalid)
                                                {
                                                    hasInvalidAlt = true;
                                                    break;
                                                }
                                                altCache[gx, gy] = altResult.alt;
                                                utmCache[gx, gy] = convertCoords(latlng);
                                                utmCache[gx, gy][2] = altResult.alt;
                                            }
                                        }

                                        if (hasInvalidAlt)
                                        {
                                            ti = null;
                                        }
                                        else
                                        {
                                            // Third pass: build quads using cached data
                                            var zindexmod = (20 - ti.zoom) * 0.30;
                                            for (int gx = 0; gx < gridWidth - 1; gx++)
                                            {
                                                long x = xstart + gx * pxstep;
                                                long xnext = x + pxstep;
                                                for (int gy = 0; gy < gridHeight - 1; gy++)
                                                {
                                                    long y = ystart + gy * pxstep;
                                                    long ynext = y + pxstep;

                                                    // Reuse cached UTM coordinates and altitudes
                                                    var utm1 = utmCache[gx, gy];       // bl
                                                    var utm2 = utmCache[gx, gy + 1];   // tl
                                                    var utm3 = utmCache[gx + 1, gy];   // br
                                                    var utm4 = utmCache[gx + 1, gy + 1]; // tr

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
                                                    var startindex = (uint)ti.vertex.Count - 4;
                                                    ti.indices.AddRange(new[]
                                                    {
                                                        startindex + 0, startindex + 1, startindex + 3,
                                                        startindex + 1, startindex + 2, startindex + 3
                                                    });
                                                }
                                            }
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
            utm1[2] = srtm.getAltitudeFast(latlng1.Lat, latlng1.Lng).alt;
            var utm2 = convertCoords(latlng2);
            utm2[2] = srtm.getAltitudeFast(latlng2.Lat, latlng2.Lng).alt;
            var utm3 = convertCoords(latlng3);
            utm3[2] = srtm.getAltitudeFast(latlng3.Lat, latlng3.Lng).alt;
            var utm4 = convertCoords(latlng4);
            utm4[2] = srtm.getAltitudeFast(latlng4.Lat, latlng4.Lng).alt;
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
            this.btn_configure = new MissionPlanner.Controls.MyButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            //
            // btn_configure
            //
            this.btn_configure.Location = new System.Drawing.Point(3, 3);
            this.btn_configure.Name = "btn_configure";
            this.btn_configure.Size = new System.Drawing.Size(75, 23);
            this.btn_configure.TabIndex = 0;
            this.btn_configure.Text = "Configure";
            this.btn_configure.UseVisualStyleBackColor = true;
            this.btn_configure.Click += new System.EventHandler(this.btn_configure_Click);
            //
            // timer1
            //
            this.timer1.Interval = 33;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // Map3D
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.btn_configure);
            this.Name = "Map3D";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.test_Load);
            this.Resize += new System.EventHandler(this.test_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void test_Load(object sender, EventArgs e)
        {
            _settingsLoaded = true;

            if (!Context.IsCurrent)
                Context.MakeCurrent(this.WindowInfo);

            _imageloaderThread = new Thread(imageLoader)
            {
                IsBackground = true,
                Name = "gl imageLoader"
            };
            _imageloaderThread.Start();

            // Request driver-side anti-aliasing (works when the context was created with samples).
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

        private void btn_configure_Click(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "3D Map Settings";
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                int margin = 15;
                int y = margin;
                int inputWidth = 80;
                int inputX = 110;
                int contentWidth = inputX + inputWidth;

                var lblZoom = new Label { Text = "Map Zoom:", Location = new Point(margin, y + 3), AutoSize = true };
                var numZoom = new NumericUpDown { Location = new Point(inputX, y), Width = inputWidth, Minimum = 6, Maximum = 24, Value = Math.Max(6, Math.Min(24, zoom)) };
                dialog.Controls.Add(lblZoom);
                dialog.Controls.Add(numZoom);
                y += 30;

                var lblDist = new Label { Text = "Camera Dist:", Location = new Point(margin, y + 3), AutoSize = true };
                var numDist = new NumericUpDown { Location = new Point(inputX, y), Width = inputWidth, Minimum = (decimal)0.1, Maximum = 100, DecimalPlaces = 2, Increment = (decimal)0.05, Value = (decimal)Math.Max(0.1, Math.Min(100, _cameraDist)) };
                dialog.Controls.Add(lblDist);
                dialog.Controls.Add(numDist);
                y += 30;

                var lblAngle = new Label { Text = "Camera Angle:", Location = new Point(margin, y + 3), AutoSize = true };
                var numAngle = new NumericUpDown { Location = new Point(inputX, y), Width = inputWidth, Minimum = -360, Maximum = 360, DecimalPlaces = 0, Increment = 15, Value = (decimal)Math.Max(-360, Math.Min(360, _cameraAngle)) };
                dialog.Controls.Add(lblAngle);
                dialog.Controls.Add(numAngle);
                y += 30;

                var lblHeight = new Label { Text = "Camera Height:", Location = new Point(margin, y + 3), AutoSize = true };
                var numHeight = new NumericUpDown { Location = new Point(inputX, y), Width = inputWidth, Minimum = -100, Maximum = 100, DecimalPlaces = 2, Increment = (decimal)0.05, Value = (decimal)Math.Max(-100, Math.Min(100, _cameraHeight)) };
                dialog.Controls.Add(lblHeight);
                dialog.Controls.Add(numHeight);
                y += 30;

                var lblFOV = new Label { Text = "Camera FoV:", Location = new Point(margin, y + 3), AutoSize = true };
                var numFOV = new NumericUpDown { Location = new Point(inputX, y), Width = inputWidth, Minimum = 30, Maximum = 120, Increment = 5, Value = (decimal)Math.Max(30, Math.Min(120, _cameraFOV)) };
                dialog.Controls.Add(lblFOV);
                dialog.Controls.Add(numFOV);
                y += 30;

                var lblScale = new Label { Text = "MAV Scale (m):", Location = new Point(margin, y + 3), AutoSize = true };
                var numScale = new NumericUpDown { Location = new Point(inputX, y), Width = inputWidth, Minimum = (decimal)0.1, Maximum = 10, DecimalPlaces = 2, Increment = (decimal)0.05, Value = (decimal)Math.Max(0.1, Math.Min(10, _planeScaleMultiplier)) };
                dialog.Controls.Add(lblScale);
                dialog.Controls.Add(numScale);
                y += 30;

                var lblMarkerSize = new Label { Text = "WP Marker Size:", Location = new Point(margin, y + 3), AutoSize = true };
                var numMarkerSize = new NumericUpDown { Location = new Point(inputX, y), Width = inputWidth, Minimum = 10, Maximum = 500, DecimalPlaces = 0, Increment = 10, Value = (decimal)Math.Max(10, Math.Min(500, _waypointMarkerSize)) };
                dialog.Controls.Add(lblMarkerSize);
                dialog.Controls.Add(numMarkerSize);
                y += 30;

                var lblADSBSize = new Label { Text = "ADSB Size:", Location = new Point(margin, y + 3), AutoSize = true };
                var numADSBSize = new NumericUpDown { Location = new Point(inputX, y), Width = inputWidth, Minimum = 50, Maximum = 2000, DecimalPlaces = 0, Increment = 50, Value = (decimal)Math.Max(50, Math.Min(2000, _adsbCircleSize)) };
                dialog.Controls.Add(lblADSBSize);
                dialog.Controls.Add(numADSBSize);
                y += 30;

                var lblColor = new Label { Text = "MAV Color:", Location = new Point(margin, y + 3), AutoSize = true };
                var pnlColor = new Panel { Location = new Point(inputX, y), Width = inputWidth, Height = 23, BorderStyle = BorderStyle.FixedSingle, Cursor = Cursors.Hand, Tag = "IgnoreTheme", BackColor = _planeColor };
                Color selectedColor = _planeColor;
                pnlColor.Click += (s, ev) =>
                {
                    using (var colorDialog = new ColorDialog())
                    {
                        colorDialog.Color = selectedColor;
                        colorDialog.FullOpen = true;
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            selectedColor = colorDialog.Color;
                            pnlColor.BackColor = selectedColor;
                        }
                    }
                };
                dialog.Controls.Add(lblColor);
                dialog.Controls.Add(pnlColor);
                y += 30;

                var lblSTL = new Label { Text = "STL File:", Location = new Point(margin, y + 3), AutoSize = true };
                string selectedSTLPath = _planeSTLPath;
                string stlButtonText = string.IsNullOrEmpty(selectedSTLPath) ? "Default" : Path.GetFileName(selectedSTLPath);
                var btnSTL = new Button { Location = new Point(inputX, y), Width = inputWidth, Height = 23, Text = stlButtonText };
                btnSTL.Click += (s, ev) =>
                {
                    using (var openDialog = new OpenFileDialog())
                    {
                        openDialog.Filter = "STL Files (*.stl)|*.stl|All Files (*.*)|*.*";
                        openDialog.Title = "Select Plane STL File";
                        if (!string.IsNullOrEmpty(selectedSTLPath) && File.Exists(selectedSTLPath))
                            openDialog.InitialDirectory = Path.GetDirectoryName(selectedSTLPath);
                        if (openDialog.ShowDialog() == DialogResult.OK)
                        {
                            selectedSTLPath = openDialog.FileName;
                            btnSTL.Text = Path.GetFileName(selectedSTLPath);
                        }
                    }
                };
                dialog.Controls.Add(lblSTL);
                dialog.Controls.Add(btnSTL);
                y += 30;

                var chkHeading = new CheckBox { Text = "Heading Line (Red)", Location = new Point(margin, y), AutoSize = true, Checked = _showHeadingLine };
                dialog.Controls.Add(chkHeading);
                y += 24;

                var chkNavBearing = new CheckBox { Text = "Nav Bearing Line (Orange)", Location = new Point(margin, y), AutoSize = true, Checked = _showNavBearingLine };
                dialog.Controls.Add(chkNavBearing);
                y += 24;

                var chkGpsHeading = new CheckBox { Text = "GPS Heading Line (Black)", Location = new Point(margin, y), AutoSize = true, Checked = _showGpsHeadingLine };
                dialog.Controls.Add(chkGpsHeading);
                y += 24;

                var chkTurnRadius = new CheckBox { Text = "Turn Radius Arc (Pink)", Location = new Point(margin, y), AutoSize = true, Checked = _showTurnRadius };
                dialog.Controls.Add(chkTurnRadius);
                y += 24;

                var chkTrail = new CheckBox { Text = "Flight Path Trail (armed only)", Location = new Point(margin, y), AutoSize = true, Checked = _showTrail };
                dialog.Controls.Add(chkTrail);
                y += 30;

                int btnWidth = 75;
                int btnGap = 10;
                int totalBtnWidth = btnWidth * 2 + btnGap;
                int btnStartX = (contentWidth + margin * 2 - totalBtnWidth) / 2;

                var btnSave = new Button { Text = "Save", Location = new Point(btnStartX, y), Width = btnWidth };
                btnSave.Click += (s, ev) => { dialog.Close(); };
                dialog.Controls.Add(btnSave);

                var btnReset = new Button { Text = "Reset", Location = new Point(btnStartX + btnWidth + btnGap, y), Width = btnWidth };
                btnReset.Click += (s, ev) =>
                {
                    numZoom.Value = 15;
                    numDist.Value = (decimal)0.8;
                    numAngle.Value = 0;
                    numHeight.Value = (decimal)0.2;
                    numFOV.Value = 60;
                    numScale.Value = 1;
                    numMarkerSize.Value = 60;
                    numADSBSize.Value = 500;
                    selectedColor = Color.Red;
                    pnlColor.BackColor = Color.Red;
                    selectedSTLPath = "";
                    btnSTL.Text = "Default";
                    chkHeading.Checked = true;
                    chkNavBearing.Checked = true;
                    chkGpsHeading.Checked = true;
                    chkTurnRadius.Checked = true;
                    chkTrail.Checked = false;
                };
                dialog.Controls.Add(btnReset);
                y += 23;

                dialog.Padding = new Padding(0, 0, margin, margin);
                dialog.AutoSize = true;
                dialog.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                dialog.FormClosing += (s, ev) =>
                {
                    zoom = (int)numZoom.Value;
                    _cameraDist = (double)numDist.Value;
                    _cameraAngle = (double)numAngle.Value;
                    _cameraHeight = (double)numHeight.Value;
                    _planeScaleMultiplier = (float)numScale.Value;
                    _cameraFOV = (float)numFOV.Value;
                    _waypointMarkerSize = (double)numMarkerSize.Value;
                    _adsbCircleSize = (double)numADSBSize.Value;
                    _planeColor = selectedColor;
                    _showHeadingLine = chkHeading.Checked;
                    _showNavBearingLine = chkNavBearing.Checked;
                    _showGpsHeadingLine = chkGpsHeading.Checked;
                    _showTurnRadius = chkTurnRadius.Checked;
                    _showTrail = chkTrail.Checked;

                    bool stlChanged = _planeSTLPath != selectedSTLPath;
                    _planeSTLPath = selectedSTLPath;
                    if (stlChanged)
                        _planeLoaded = false;

                    Settings.Instance["map3d_zoom_level"] = zoom.ToString();
                    Settings.Instance["map3d_camera_dist"] = _cameraDist.ToString();
                    Settings.Instance["map3d_camera_angle"] = _cameraAngle.ToString();
                    Settings.Instance["map3d_camera_height"] = _cameraHeight.ToString();
                    Settings.Instance["map3d_mav_scale"] = _planeScaleMultiplier.ToString();
                    Settings.Instance["map3d_fov"] = _cameraFOV.ToString();
                    Settings.Instance["map3d_waypoint_marker_size"] = _waypointMarkerSize.ToString();
                    Settings.Instance["map3d_adsb_size"] = _adsbCircleSize.ToString();
                    Settings.Instance["map3d_mav_color"] = _planeColor.ToArgb().ToString();
                    Settings.Instance["map3d_plane_stl_path"] = _planeSTLPath;
                    Settings.Instance["map3d_show_heading"] = _showHeadingLine.ToString();
                    Settings.Instance["map3d_show_nav_bearing"] = _showNavBearingLine.ToString();
                    Settings.Instance["map3d_show_gps_heading"] = _showGpsHeadingLine.ToString();
                    Settings.Instance["map3d_show_turn_radius"] = _showTurnRadius.ToString();
                    Settings.Instance["map3d_show_trail"] = _showTrail.ToString();
                    Settings.Instance.Save();

                    test_Resize(null, null);
                };

                dialog.ShowDialog();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (onpaintrun == true && IsHandleCreated && !IsDisposed && !Disposing)
            {
                this.Invalidate();
                onpaintrun = false;
            }
        }

        private void test_Resize(object sender, EventArgs e)
        {
            textureSemaphore.Wait();
            try
            {
                if (!Context.IsCurrent)
                    Context.MakeCurrent(this.WindowInfo);
                GL.Viewport(0, 0, this.Width, this.Height);
                float renderDistance = _center.Alt > 500 ? 100000f : 50000f;
                projMatrix = OpenTK.Matrix4.CreatePerspectiveFieldOfView(
                    (float) (_cameraFOV * MathHelper.deg2rad),
                    (float) Width / Height, 0.1f,
                    renderDistance);
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
            internal static int positionSlot;
            internal static int colorSlot;
            internal static int projectionSlot;
            internal static int modelViewSlot;
            private static int textureSlot;
            private static int texCoordSlot;

            internal static int Program
            {
                get
                {
                    if (_program == 0)
                        CreateShaders();
                    return _program;
                }
            }

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

        /// <summary>
        /// Stores ADSB aircraft screen position for hit testing
        /// </summary>
        private struct ADSBScreenPosition
        {
            public float ScreenX;
            public float ScreenY;
            public float Radius; // Screen radius for hit testing
            public adsb.PointLatLngAltHdg PlaneData;
            public double DistanceToOwn; // Distance in meters to own aircraft
        }

        private int ComputeWaypointHash(IList<PointLatLngAlt> waypoints)
        {
            unchecked
            {
                int hash = 17;
                for (int i = 0; i < waypoints.Count; i++)
                {
                    var wp = waypoints[i];
                    if (wp == null)
                        continue;
                    hash = hash * 31 + wp.Lat.GetHashCode();
                    hash = hash * 31 + wp.Lng.GetHashCode();
                    hash = hash * 31 + wp.Alt.GetHashCode();
                    hash = hash * 31 + (wp.Tag?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }
    }
}
