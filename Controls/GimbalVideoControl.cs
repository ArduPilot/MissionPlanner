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
using log4net;
using System.Collections.Generic;
using static MAVLink;
using MissionPlanner.GCSViews;
using System.Threading.Tasks;
using MissionPlanner.ArduPilot.Mavlink;
using GMap.NET.WindowsForms;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace MissionPlanner.Controls
{
    public partial class GimbalVideoControl : UserControl, IMessageFilter
    {
        // logger
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private GimbalControlSettings preferences = new GimbalControlSettings();

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

        private bool isRecording
        {
            // TODO: ArduPilot hard-codes this to 0 presently, so we can't check it
            /*get
            {
                return selectedCamera?.CameraCaptureStatus.video_status > 0;
            }*/

            // So for now, we will manually track it
            // TODO: Remove this once ArduPilot is fixed
            get;
            set;
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

        // The selected gimbal ID for the currently-selected gimbal manager
        // (0 means all gimbals)
        private byte selectedGimbalID = 0;

        private GMapOverlay mouseMapMarker;
        public GimbalVideoControl()
        {
            InitializeComponent();

            _stream.OnNewImage += RenderFrame;

            loadPreferences();

            // Register the global key handler
            Application.AddMessageFilter(this);

            mouseMapMarker = new GMapOverlay("MouseMarker");
            MainV2.instance.FlightData.gMapControl1.Overlays.Add(mouseMapMarker);
        }

        private void loadPreferences()
        {
            preferences = new GimbalControlSettings();
            var json = Settings.Instance["GimbalControlPreferences", ""];
            if (json != "")
            {
                try
                {
                    preferences = Newtonsoft.Json.JsonConvert.DeserializeObject<GimbalControlSettings>(json);
                }
                catch (Exception ex)
                {
                    log.Error("Invalid GimbalControlPreferences, reverting to default", ex);
                }
            }

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
            boundPressKeys.Add(preferences.PointDown);
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

        private void RenderFrame(object sender, MPBitmap image)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => RenderFrame(sender, image)));
                return;
            }
            try
            {
                if (image == null || image.Width <= 0 || image.Height <= 0)
                {
                    VideoBox.Image?.Dispose();
                    VideoBox.Image = null;
                    VideoBox.Image = VideoBox.ErrorImage;
                    return;
                }

                var old = VideoBox.Image;
                VideoBox.Image = new Bitmap(
                    image.Width, image.Height, 4 * image.Width,
                    PixelFormat.Format32bppPArgb,
                    image.LockBits(Rectangle.Empty, null, SKColorType.Bgra8888).Scan0);

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

            var form = new VideoStreamSelector()
            {
                StartPosition = FormStartPosition.CenterParent,
            };
            if (form.ShowDialog() == DialogResult.OK)
            {
                _stream.Start(form.gstreamer_pipeline);
            }
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
                selectedGimbalManager?.SetRatesCommandAsync(pitch, yaw, yaw_lock, selectedGimbalID);
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

        private void HandleKeyPress(Keys key)
        {
            if (key == preferences.TakePicture)
            {
                TakePicture();
            }
            else if (key == preferences.ToggleRecording)
            {
                SetRecording(!isRecording);
            }
            else if (key == preferences.StartRecording)
            {
                SetRecording(true);
            }
            else if (key == preferences.StopRecording)
            {
                SetRecording(false);
            }
            else if (key == preferences.ToggleLockFollow)
            {
                SetYawLock(!yaw_lock);
            }
            else if (key == preferences.SetLock)
            {
                SetYawLock(true);
            }
            else if (key == preferences.SetFollow)
            {
                SetYawLock(false);
            }
            else if (key == preferences.Retract)
            {
                Retract();
            }
            else if (key == preferences.Neutral)
            {
                Neutral();
            }
            else if (key == preferences.PointDown)
            {
                PointDown();
            }
            else if (key == preferences.Home)
            {
                Home();
            }
        }

        private void TakePicture()
        {
                Console.WriteLine("Take picture");
                selectedCamera?.TakeSinglePictureAsync();
        }

        private void SetRecording(bool start)
        {
            isRecording = start;
            if(start)
            {
                Console.WriteLine("Start recording");
                selectedCamera?.StartRecordingAsync();
            }
            else
            {
                Console.WriteLine("Stop recording");
                selectedCamera?.StopRecordingAsync();
            }
        }

        private void SetYawLock(bool locked)
        {
            string message = locked ? "lock" : "follow";
            Console.WriteLine($"Set yaw {message}");
            yaw_lock = locked;
            yawLockToolStripMenuItem.Checked = locked;
            selectedGimbalManager?.SetRatesCommandAsync(previousPitchRate, previousYawRate, yaw_lock, selectedGimbalID);
        }

        private void Retract()
        {
            Console.WriteLine("Retract");
            selectedGimbalManager?.RetractAsync();
        }

        private void Neutral()
        {
            Console.WriteLine("Neutral");
            selectedGimbalManager?.NeutralAsync();
        }

        private void PointDown()
        {
            Console.WriteLine("Point down");
            selectedGimbalManager?.SetAnglesCommandAsync(-90, 0, false, selectedGimbalID);
        }

        private void Home()
        {
            Console.WriteLine("Home");
            var loc = MainV2.comPort?.MAV?.cs.HomeLocation;
            selectedGimbalManager?.SetROILocationAsync(loc.Lat, loc.Lng, loc.Alt, frame: MAV_FRAME.GLOBAL);
        }
    
        private DateTime lastMouseMove = DateTime.MinValue;
        private void VideoBox_MouseMove(object sender, MouseEventArgs e)
        {
            if(DateTime.Now > lastMouseMove.AddMilliseconds(100))
            {
                lastMouseMove = DateTime.Now;
                mouseMapMarker.Clear();


                var point = getMousePosition(e.X, e.Y);
                if(!point.HasValue) { return; }
                var latlon = selectedCamera?.CalculateImagePointLocation(point.Value.x, point.Value.y);
                if (latlon != null)
                {
                    mouseMapMarker.Markers.Add(
                        new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                            new GMap.NET.PointLatLng(latlon.Lat, latlon.Lng),
                            GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_small
                        )
                    );
                }
            }
        }

        private void VideoBox_MouseLeave(object sender, EventArgs e)
        {
            mouseMapMarker.Markers.Clear();
        }

        private void VideoBox_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            var point = getMousePosition(me.X, me.Y);
            if (!point.HasValue)
            {
                return;
            }

            // Check the key/button combination to determine the action
            if ((Control.ModifierKeys, me.Button) == preferences.MoveCameraToMouseLocation)
            {
                var attitude = selectedGimbalManager?.GetAttitude(selectedGimbalID);
                if (attitude == null)
                {
                    return;
                }
                var q = selectedCamera?.CalculateImagePointRotation(point.Value.x, point.Value.y);
                if (q == null)
                {
                    return;
                }
                q = attitude * q;
                Console.WriteLine("Attitude: {0:0.0} {1:0.0} {2:0.0}", attitude.get_euler_yaw() * MathHelper.rad2deg, attitude.get_euler_pitch() * MathHelper.rad2deg, attitude.get_euler_roll() * MathHelper.rad2deg);
                Console.WriteLine("New: {0:0.0} {1:0.0} {2:0.0}", q.get_euler_yaw() * MathHelper.rad2deg, q.get_euler_pitch() * MathHelper.rad2deg, q.get_euler_roll() * MathHelper.rad2deg);

                selectedGimbalManager?.SetAttitudeAsync(q, yaw_lock, selectedGimbalID);
               
            }
            else if ((Control.ModifierKeys, me.Button) == preferences.MoveCameraPOIToMouseLocation)
            {
                var loc = selectedCamera?.CalculateImagePointLocation(point.Value.x, point.Value.y);
                if (loc == null)
                {
                    return;
                }
                selectedGimbalManager?.SetROILocationAsync(loc.Lat, loc.Lng, loc.Alt, frame: MAV_FRAME.GLOBAL);
            }
        }

        private (double x, double y)? getMousePosition(int x, int y)
        {
            if (VideoBox.Image == null)
            {
                return null;
            }

            // Find the point within the image inside VideoBox, not just the VideoBox
            var imageWidth = Math.Min(VideoBox.Width, VideoBox.Height * VideoBox.Image.Width / VideoBox.Image.Height);
            var imageHeight = Math.Min(VideoBox.Height, VideoBox.Width * VideoBox.Image.Height / VideoBox.Image.Width);
            if (imageWidth < VideoBox.Width)
            {
                x -= (VideoBox.Width - imageWidth) / 2;
                x *= VideoBox.Width / imageWidth;
                if (x < 0 || x >= imageWidth)
                {
                    return null;
                }
            }
            if (imageHeight < VideoBox.Height)
            {
                y -= (VideoBox.Height - imageHeight) / 2;
                y *= VideoBox.Height / imageHeight;
                if (y < 0 || y >= imageHeight)
                {
                    return null;
                }
            }

            return (2 * x / (double)imageWidth - 1, 2 * y / (double)imageHeight - 1);
        }

        /// <summary>
        /// Update the camera controls based on the current camera capabilities
        /// </summary>
        private void UITimer_Tick(object sender, EventArgs e)
        {
            takePictureToolStripMenuItem.Enabled = selectedCamera?.CanCaptureImage ?? false;
            startRecordingToolStripMenuItem.Enabled = selectedCamera?.CanCaptureVideo ?? false;
            stopRecordingToolStripMenuItem.Enabled = selectedCamera?.CanCaptureVideo ?? false;

            if (selectedCamera != null)
            {
                selectedCamera.HFOV = (float)preferences.CameraHFOV;
                selectedCamera.VFOV = (float)preferences.CameraVFOV;
                selectedCamera.UseFOVStatus = preferences.UseFOVReportedByCamera;
            }
        }

        private void yawLockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null) { return; }
            SetYawLock(!item.Checked);
        }

        private void retractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Retract();
        }

        private void neutralToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Neutral();
        }

        private void pointDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PointDown();
        }

        private void pointHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Home();
        }

        private void takePictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TakePicture();
        }

        private void startRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRecording(true);
        }

        private void stopRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRecording(false);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new GimbalControlSettingsForm(preferences);
            if (form.ShowDialog() == DialogResult.OK)
            {
                Settings.Instance["GimbalControlPreferences"] = Newtonsoft.Json.JsonConvert.SerializeObject(form.preferences);
                loadPreferences();
            }
        }
    }
}
