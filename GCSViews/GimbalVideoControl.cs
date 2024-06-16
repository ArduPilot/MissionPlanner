#if !LIB
// XXX: We need both the System.Drawing.Bitmap from System.Drawing and MissionPlanner.Drawing
extern alias Drawing;
using MPBitmap = Drawing::System.Drawing.Bitmap;
#else
using MPBitmap = System.Drawing.Bitmap;
#endif

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using SkiaSharp;
using OpenTK.Input;
using log4net;
using System.Collections.Generic;
using static MAVLink;
using MissionPlanner.GCSViews;
using System.Threading.Tasks;
using MissionPlanner.ArduPilot.Mavlink;

namespace MissionPlanner
{
    public partial class GimbalVideoControl : UserControl, IMessageFilter
    {
        // logger
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private GimbalControlPreferences preferences = new GimbalControlPreferences();

        private readonly GStreamer _stream = new GStreamer();

        private HashSet<Keys> heldKeys = new HashSet<Keys>();
        private HashSet<Keys> boundHoldKeys = new HashSet<Keys>();
        private HashSet<Keys> boundPressKeys = new HashSet<Keys>();

        private Dictionary<Keys, Keys> mod2key = new Dictionary<Keys, Keys>()
        {
            { Keys.Shift, Keys.ShiftKey },
            { Keys.Control, Keys.ControlKey },
            { Keys.Alt, Keys.Menu }
        };

        private float previousPitchRate = 0;
        private float previousYawRate = 0;
        private float previousZoomRate = 0;
        private UInt32 gimbalManagerFlags = 0;
        private byte gimbalDeviceId = 0;
        private bool yaw_lock = false;

        private CameraProtocol _selectedCamera;
        private CameraProtocol selectedCamera
        {
            get
            {
                return _selectedCamera ?? MainV2.comPort?.MAV?.Camera;
            }
            set
            {
                _selectedCamera = value;
            }
        }

        private GimbalManagerProtocol _selectedGimbalManager;
        private GimbalManagerProtocol selectedGimbalManager
        {
            get
            {
                return _selectedGimbalManager ?? MainV2.comPort?.MAV?.GimbalManager;
            }
            set
            {
                _selectedGimbalManager = value;
            }
        }

        public GimbalVideoControl()
        {
            InitializeComponent();

            _stream.OnNewImage += RenderFrame;

            loadPreferences();

            // Register the global key handler
            Application.AddMessageFilter(this);
        }

        private void loadPreferences()
        {
            var json = Settings.Instance["GimbalControlPreferences", ""];
            if (json != "")
            {
                try
                {
                    preferences = Newtonsoft.Json.JsonConvert.DeserializeObject<GimbalControlPreferences>(json);
                }
                catch (Exception ex)
                {
                    log.Error("Invalid GimbalControlPreferences, reverting to default", ex);
                }
            }

            setCameraControlPanelVisibility(preferences.ShowCameraControls);

            // Populate the list of keys that are expected to be pressed
            boundPressKeys.Clear();
            boundPressKeys.Add(preferences.TakePicture);
            boundPressKeys.Add(preferences.ToggleRecording);
            boundPressKeys.Add(preferences.StartRecording);
            boundPressKeys.Add(preferences.StopRecording);
            boundPressKeys.Add(preferences.ToggleLockFollow);
            boundPressKeys.Add(preferences.SetLock);
            boundPressKeys.Add(preferences.SetFollow);
            boundPressKeys.Add(preferences.Retract);
            boundPressKeys.Add(preferences.Neutral);
            boundPressKeys.Add(preferences.Home);

            // Populate the list of keys that are expected to be held down
            boundHoldKeys.Clear();
            boundHoldKeys.Add(preferences.SlewLeft);
            boundHoldKeys.Add(preferences.SlewRight);
            boundHoldKeys.Add(preferences.SlewUp);
            boundHoldKeys.Add(preferences.SlewDown);
            boundHoldKeys.Add(preferences.ZoomIn);
            boundHoldKeys.Add(preferences.ZoomOut);
            // Add relevant modifier keys
            boundHoldKeys.Add(mod2key[preferences.SlewFastModifier]);
            boundHoldKeys.Add(mod2key[preferences.SlewSlowModifier]);
        }

        private void setCameraControlPanelVisibility(bool visibility)
        {
            CameraLayoutPanel.Visible = visibility;
        }

        private void RenderFrame(object sender, MPBitmap image)
        {
            try
            {
                if (image == null)
                {
                    VideoBox.Image?.Dispose();
                    VideoBox.Image = null;
                    return;
                }

                var old = VideoBox.Image;
                VideoBox.Image = new Bitmap(
                    image.Width, image.Height, 4 * image.Width,
                    PixelFormat.Format32bppPArgb,
                    image.LockBits(Rectangle.Empty, null, SKColorType.Bgra8888).Scan0);

                // Overlay crosshairs
                if (Control.ModifierKeys == preferences.SlewCameraBasedOnMouseModifier)
                {
                    using (var g = Graphics.FromImage(VideoBox.Image))
                    {
                        g.DrawLine(Pens.Red, image.Width / 2, 0, image.Width / 2, image.Height);
                        g.DrawLine(Pens.Red, 0, image.Height / 2, image.Width, image.Height / 2);
                    }
                }

                old?.Dispose();
            }
            catch (Exception ex)
            {
                log.Error("Error rendering frame", ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.OnNewImage -= RenderFrame;
                _stream.Stop();

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void videoStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GStreamer.GstLaunch = GStreamer.LookForGstreamer();

            if (!GStreamer.GstLaunchExists)
            {
                GStreamerUI.DownloadGStreamer();

                if (!GStreamer.GstLaunchExists)
                {
                    return;
                }
            }

            //_stream.Start("rtspsrc location=rtsp://192.168.144.25:8554/main.264 latency=41 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false");
            _stream.Start("videotestsrc ! video/x-raw, width=1280, height=720, framerate=30/1 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink");
        }

        public bool PreFilterMessage(ref Message m)
        {
            // Don't hog the keyboard when this control doesn't have focus
            if (!ContainsFocus)
            {
                if(heldKeys.Count > 0)
                {
                    heldKeys.Clear();
                    HandleHeldKeys();
                }
                return false;
            }

            const int WM_KEYDOWN = 0x0100;
            const int WM_KEYUP = 0x0101;
            const int WM_SYSKEYDOWN = 0x0104;
            const int WM_SYSKEYUP = 0x0105;

            if (m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)
            {
                // Don't handle repeated keydown events from holding down a key
                if ((m.LParam.ToInt32() & 0x40000000) != 0)
                {
                    return false;
                }
                return HandleKeyDown((Keys)m.WParam);
            }
            else if (m.Msg == WM_KEYUP || m.Msg == WM_SYSKEYUP)
            {
                return HandleKeyUp((Keys)m.WParam);
            }

            return false; // Allow the message to continue to the next filter
        }

        private bool HandleKeyDown(Keys key)
        {
            if (boundHoldKeys.Contains(key))
            {
                heldKeys.Add(key);
                HandleHeldKeys();
                return true;
            }
            else if (boundPressKeys.Contains(key | Control.ModifierKeys))
            {
                HandleKeyPress(key | Control.ModifierKeys);
                return true;
            }
            return false;
        }

        private bool HandleKeyUp(Keys key)
        {
            // Always try to remove the key from the list of pressed keys, even if not bound, just in case
            heldKeys.Remove(key);
            if (boundHoldKeys.Contains(key))
            {
                HandleHeldKeys();
            }
            return boundHoldKeys.Contains(key);
        }

        private void HandleHeldKeys()
        {
            float pitch = 0;
            float yaw = 0;
            if (heldKeys.Contains(preferences.SlewDown))
            {
                pitch -= 1;
            }
            if (heldKeys.Contains(preferences.SlewUp))
            {
                pitch += 1;
            }
            if (heldKeys.Contains(preferences.SlewLeft))
            {
                yaw -= 1;
            }
            if (heldKeys.Contains(preferences.SlewRight))
            {
                yaw += 1;
            }

            float speed = (float)preferences.SlewSpeedNormal;
            if (Control.ModifierKeys == preferences.SlewFastModifier)
            {
                speed = (float)preferences.SlewSpeedFast;
            }
            else if (Control.ModifierKeys == preferences.SlewSlowModifier)
            {
                speed = (float)preferences.SlewSpeedSlow;
            }

            pitch *= speed;
            yaw *= speed;

            if (pitch != previousPitchRate || yaw != previousYawRate)
            {
                previousPitchRate = pitch;
                previousYawRate = yaw;
                selectedGimbalManager?.SetRatesCommand(pitch, yaw, yaw_lock, gimbalDeviceId);
                Console.WriteLine($"Pitch: {pitch}, Yaw: {yaw}");
            }

            float zoom = 0;
            if (heldKeys.Contains(preferences.ZoomIn))
            {
                zoom += 1;
            }
            if (heldKeys.Contains(preferences.ZoomOut))
            {
                zoom -= 1;
            }

            zoom *= (float)preferences.ZoomSpeed;

            if (zoom != previousZoomRate)
            {
                previousZoomRate = zoom;
                selectedCamera?.SetZoomAsync(zoom, CAMERA_ZOOM_TYPE.ZOOM_TYPE_CONTINUOUS);
                Console.WriteLine($"Zoom: {zoom}");
            }
        }

        private bool recording = false;
        private bool lockMode = false;
        private void HandleKeyPress(Keys key)
        {
            if (key == preferences.TakePicture)
            {
                Console.WriteLine("Take picture");
                selectedCamera?.TakeSinglePictureAsync();
            }
            else if (key == preferences.StartRecording || (key == preferences.ToggleRecording && !recording))
            {
                Console.WriteLine("Start recording");
                selectedCamera?.StartRecordingAsync();
            }
            else if (key == preferences.StopRecording || (key == preferences.ToggleRecording && recording))
            {
                Console.WriteLine("Stop recording");
                selectedCamera?.StopRecordingAsync();
            }
            else if (key == preferences.ToggleLockFollow)
            {
                Console.WriteLine("Toggle lock/follow");
                yaw_lock = !yaw_lock;
                selectedGimbalManager.SetRatesCommand(previousPitchRate, previousYawRate, yaw_lock);
            }
            else if (key == preferences.SetLock)
            {
                Console.WriteLine("Set lock");
                yaw_lock = true;
                selectedGimbalManager.SetRatesCommand(previousPitchRate, previousYawRate, yaw_lock);
            }
            else if (key == preferences.SetFollow)
            {
                Console.WriteLine("Set follow");
                yaw_lock = false;
                selectedGimbalManager.SetRatesCommand(previousPitchRate, previousYawRate, yaw_lock);
            }
            else if (key == preferences.Retract)
            {
                Console.WriteLine("Retract");
                selectedGimbalManager?.Retract();
            }
            else if (key == preferences.Neutral)
            {
                Console.WriteLine("Neutral");
                selectedGimbalManager?.Neutral();
            }
            else if (key == preferences.Home)
            {
                Console.WriteLine("Home");
                var loc = MainV2.comPort?.MAV?.cs.HomeLocation;
                selectedGimbalManager?.SetROILocation(loc.Lat, loc.Lng, loc.Alt, frame: MAV_FRAME.GLOBAL);
            }
        }
    }

    public class GimbalControlPreferences
    {
        // Keybindings for various actions
        public Keys SlewLeft { get; set; }
        public Keys SlewRight { get; set; }
        public Keys SlewUp { get; set; }
        public Keys SlewDown { get; set; }
        public Keys ZoomIn { get; set; }
        public Keys ZoomOut { get; set; }

        public Keys SlewFastModifier { get; set; }
        public Keys SlewSlowModifier { get; set; }

        public Keys TakePicture { get; set; }
        public Keys ToggleRecording { get; set; }
        public Keys StartRecording { get; set; }
        public Keys StopRecording { get; set; }

        public Keys ToggleLockFollow { get; set; }
        public Keys SetLock { get; set; }
        public Keys SetFollow { get; set; }
        public Keys Retract { get; set; }
        public Keys Neutral { get; set; }
        public Keys Home { get; set; }


        public MouseButton MoveCameraToMouseLocation { get; set; }
        public MouseButton MoveCameraPOIToMouseLocation { get; set; }
        public MouseButton SlewCameraBasedOnMouse { get; set; }
        public MouseButton TrackObjectUnderMouse { get; set; }
        
        public Keys MoveCameraToMouseLocationModifier { get; set; }
        public Keys MoveCameraPOIToMouseLocationModifier { get; set; }
        public Keys SlewCameraBasedOnMouseModifier { get; set; }
        public Keys TrackObjectUnderMouseModifier { get; set; }

        // Speed settings
        public decimal SlewSpeedSlow { get; set; }
        public decimal SlewSpeedNormal { get; set; }
        public decimal SlewSpeedFast { get; set; }
        public decimal ZoomSpeed { get; set; }
        public decimal CameraFOV { get; set; }

        // Boolean options
        public bool UseScrollForZoom { get; set; }
        public bool DefaultLockedMode { get; set; }
        public bool UseFOVReportedByCamera { get; set; }
        public bool ShowCameraControls { get; set; }

        public GimbalControlPreferences()
        {
            SlewLeft = Keys.A;
            SlewRight = Keys.D;
            SlewUp = Keys.W;
            SlewDown = Keys.S;
            ZoomIn = Keys.E;
            ZoomOut = Keys.Q;

            SlewSlowModifier = Keys.Control;
            SlewFastModifier = Keys.Shift;
            
            TakePicture = Keys.Alt | Keys.F;
            ToggleRecording = Keys.Alt | Keys.R;
            StartRecording = Keys.None;
            StopRecording = Keys.None;

            ToggleLockFollow = Keys.L;
            SetLock = Keys.None;
            SetFollow = Keys.None;
            Retract = Keys.None;
            Neutral = Keys.N;
            Home = Keys.H;

            MoveCameraToMouseLocation = MouseButton.Left;
            MoveCameraPOIToMouseLocation = MouseButton.Left;
            SlewCameraBasedOnMouse = MouseButton.Left;
            TrackObjectUnderMouse = MouseButton.Left;

            MoveCameraToMouseLocationModifier = Keys.None;
            MoveCameraPOIToMouseLocationModifier = Keys.Shift;
            SlewCameraBasedOnMouseModifier = Keys.Alt;
            TrackObjectUnderMouseModifier = Keys.Control;

            // Default speed settings
            SlewSpeedSlow = 1m; // deg/sec
            SlewSpeedNormal = 5m; // deg/sec
            SlewSpeedFast = 25m; // deg/sec
            ZoomSpeed = 1.0m; // unitless [0, 1]
            CameraFOV = 50.0m; // horizontal, degrees

            // Default boolean options
            UseScrollForZoom = true;
            DefaultLockedMode = false;
            UseFOVReportedByCamera = true;
            ShowCameraControls = true;
        }
    }
}
