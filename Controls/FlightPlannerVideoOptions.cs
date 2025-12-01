using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DirectShowLib;
using MissionPlanner.Utilities;
using WebCamService;

namespace MissionPlanner.Controls
{
    public class FlightPlannerVideoOptions : UserControl
    {
        private Label labelVideoDevice;
        private Label labelVideoFormat;
        private Label labelOsdColor;
        private ComboBox cmbVideoSources;
        private ComboBox cmbVideoResolutions;
        private ComboBox cmbOsdColor;
        private MyButton btnVideoStart;
        private MyButton btnVideoStop;
        private CheckBox chkHudShow;
        private bool startup = true;

        public FlightPlannerVideoOptions()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.labelVideoDevice = new Label();
            this.labelVideoFormat = new Label();
            this.labelOsdColor = new Label();
            this.cmbVideoSources = new ComboBox();
            this.cmbVideoResolutions = new ComboBox();
            this.cmbOsdColor = new ComboBox();
            this.btnVideoStart = new MyButton();
            this.btnVideoStop = new MyButton();
            this.chkHudShow = new CheckBox();
            this.SuspendLayout();

            // labelVideoDevice
            this.labelVideoDevice.AutoSize = true;
            this.labelVideoDevice.Location = new Point(10, 15);
            this.labelVideoDevice.Name = "labelVideoDevice";
            this.labelVideoDevice.Text = "Video Device";

            // cmbVideoSources
            this.cmbVideoSources.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbVideoSources.FormattingEnabled = true;
            this.cmbVideoSources.Location = new Point(120, 12);
            this.cmbVideoSources.Name = "cmbVideoSources";
            this.cmbVideoSources.Size = new Size(200, 21);
            this.cmbVideoSources.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.cmbVideoSources.Click += CmbVideoSources_Click;
            this.cmbVideoSources.SelectedIndexChanged += CmbVideoSources_SelectedIndexChanged;

            // labelVideoFormat
            this.labelVideoFormat.AutoSize = true;
            this.labelVideoFormat.Location = new Point(10, 45);
            this.labelVideoFormat.Name = "labelVideoFormat";
            this.labelVideoFormat.Text = "Video Format";

            // cmbVideoResolutions
            this.cmbVideoResolutions.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbVideoResolutions.FormattingEnabled = true;
            this.cmbVideoResolutions.Location = new Point(120, 42);
            this.cmbVideoResolutions.Name = "cmbVideoResolutions";
            this.cmbVideoResolutions.Size = new Size(200, 21);
            this.cmbVideoResolutions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // labelOsdColor
            this.labelOsdColor.AutoSize = true;
            this.labelOsdColor.Location = new Point(10, 75);
            this.labelOsdColor.Name = "labelOsdColor";
            this.labelOsdColor.Text = "OSD Color";

            // cmbOsdColor
            this.cmbOsdColor.DrawMode = DrawMode.OwnerDrawFixed;
            this.cmbOsdColor.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbOsdColor.FormattingEnabled = true;
            this.cmbOsdColor.Location = new Point(120, 72);
            this.cmbOsdColor.Name = "cmbOsdColor";
            this.cmbOsdColor.Size = new Size(200, 21);
            this.cmbOsdColor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.cmbOsdColor.DrawItem += CmbOsdColor_DrawItem;
            this.cmbOsdColor.SelectedIndexChanged += CmbOsdColor_SelectedIndexChanged;

            // btnVideoStart
            this.btnVideoStart.Location = new Point(10, 105);
            this.btnVideoStart.Name = "btnVideoStart";
            this.btnVideoStart.Size = new Size(75, 23);
            this.btnVideoStart.Text = "Start";
            this.btnVideoStart.UseVisualStyleBackColor = true;
            this.btnVideoStart.Click += BtnVideoStart_Click;

            // btnVideoStop
            this.btnVideoStop.Location = new Point(95, 105);
            this.btnVideoStop.Name = "btnVideoStop";
            this.btnVideoStop.Size = new Size(75, 23);
            this.btnVideoStop.Text = "Stop";
            this.btnVideoStop.UseVisualStyleBackColor = true;
            this.btnVideoStop.Click += BtnVideoStop_Click;

            // chkHudShow
            this.chkHudShow.AutoSize = true;
            this.chkHudShow.Location = new Point(180, 109);
            this.chkHudShow.Name = "chkHudShow";
            this.chkHudShow.Text = "HUD Overlay";
            this.chkHudShow.UseVisualStyleBackColor = true;
            this.chkHudShow.CheckedChanged += ChkHudShow_CheckedChanged;

            // FlightPlannerVideoOptions
            this.Controls.Add(this.labelVideoDevice);
            this.Controls.Add(this.cmbVideoSources);
            this.Controls.Add(this.labelVideoFormat);
            this.Controls.Add(this.cmbVideoResolutions);
            this.Controls.Add(this.labelOsdColor);
            this.Controls.Add(this.cmbOsdColor);
            this.Controls.Add(this.btnVideoStart);
            this.Controls.Add(this.btnVideoStop);
            this.Controls.Add(this.chkHudShow);
            this.Name = "FlightPlannerVideoOptions";
            this.Size = new Size(330, 140);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Activate();
        }

        public void Activate()
        {
            startup = true;

            // Populate OSD color dropdown
            cmbOsdColor.DataSource = Enum.GetNames(typeof(KnownColor));

            // Set OSD color from settings
            var hudcolor = Settings.Instance["hudcolor"];
            if (hudcolor != null)
            {
                var index = cmbOsdColor.Items.IndexOf(hudcolor ?? "White");
                try
                {
                    if (index >= 0)
                        cmbOsdColor.SelectedIndex = index;
                }
                catch { }
            }

            // Setup video start/stop button states
            if (MainV2.cam != null)
            {
                btnVideoStart.Enabled = false;
                chkHudShow.Checked = GCSViews.FlightData.myhud.hudon;
            }
            else
            {
                btnVideoStart.Enabled = true;
            }

            // Try to load saved video device
            try
            {
                if (Settings.Instance["video_device"] != null)
                {
                    CmbVideoSources_Click(this, null);
                    var device = Settings.Instance.GetInt32("video_device");
                    if (cmbVideoSources.Items.Count > device)
                        cmbVideoSources.SelectedIndex = device;

                    if (Settings.Instance["video_options"] != "" && cmbVideoSources.Text != "")
                    {
                        if (cmbVideoResolutions.Items.Count > Settings.Instance.GetInt32("video_options"))
                            cmbVideoResolutions.SelectedIndex = Settings.Instance.GetInt32("video_options");
                    }
                }
            }
            catch { }

            startup = false;
        }

        private void CmbVideoSources_Click(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

            try
            {
                var capt = new Capture();
                var devices = WebCamService.Capture.getDevices();
                cmbVideoSources.DataSource = devices;
                capt.Dispose();
            }
            catch { }
        }

        private void CmbVideoSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

            try
            {
                int hr;
                int count;
                int size;
                object o;
                IBaseFilter capFilter = null;
                ICaptureGraphBuilder2 capGraph = null;
                AMMediaType media = null;
                VideoInfoHeader v;
                VideoStreamConfigCaps c;
                var modes = new List<GCSBitmapInfo>();

                capGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                var m_FilterGraph = (IFilterGraph2)new FilterGraph();

                DsDevice[] capDevices;
                capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

                if (cmbVideoSources.SelectedIndex < 0 || cmbVideoSources.SelectedIndex >= capDevices.Length)
                    return;

                hr = m_FilterGraph.AddSourceFilterForMoniker(capDevices[cmbVideoSources.SelectedIndex].Mon, null,
                    "Video input", out capFilter);
                try
                {
                    DsError.ThrowExceptionForHR(hr);
                }
                catch
                {
                    return;
                }

                hr = capGraph.FindInterface(PinCategory.Capture, MediaType.Video, capFilter, typeof(IAMStreamConfig).GUID, out o);
                DsError.ThrowExceptionForHR(hr);

                var videoStreamConfig = o as IAMStreamConfig;
                if (videoStreamConfig == null)
                    return;

                hr = videoStreamConfig.GetNumberOfCapabilities(out count, out size);
                DsError.ThrowExceptionForHR(hr);
                var TaskMemPointer = Marshal.AllocCoTaskMem(size);
                for (var i = 0; i < count; i++)
                {
                    hr = videoStreamConfig.GetStreamCaps(i, out media, TaskMemPointer);
                    v = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
                    c = (VideoStreamConfigCaps)Marshal.PtrToStructure(TaskMemPointer, typeof(VideoStreamConfigCaps));
                    modes.Add(new GCSBitmapInfo(v.BmiHeader.Width, v.BmiHeader.Height, c.MaxFrameInterval,
                        c.VideoStandard.ToString(), media));
                }
                Marshal.FreeCoTaskMem(TaskMemPointer);
                DsUtils.FreeAMMediaType(media);

                cmbVideoResolutions.DataSource = modes;

                if (Settings.Instance["video_options"] != "" && cmbVideoSources.Text != "")
                {
                    try
                    {
                        cmbVideoResolutions.SelectedIndex = Settings.Instance.GetInt32("video_options");
                    }
                    catch { }
                }

                // Save selected device
                if (!startup)
                {
                    Settings.Instance["video_device"] = cmbVideoSources.SelectedIndex.ToString();
                }
            }
            catch { }
        }

        private void BtnVideoStart_Click(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

            // Stop first
            BtnVideoStop_Click(sender, e);

            var bmp = cmbVideoResolutions.SelectedItem as GCSBitmapInfo;
            if (bmp == null)
                return;

            try
            {
                MainV2.cam = new Capture(cmbVideoSources.SelectedIndex, bmp.Media);
                MainV2.cam.Start();

                // Hook up the camera image event to display on HUD
                MainV2.cam.camimage += GCSViews.FlightData.instance.cam_camimage;

                Settings.Instance["video_device"] = cmbVideoSources.SelectedIndex.ToString();
                Settings.Instance["video_options"] = cmbVideoResolutions.SelectedIndex.ToString();

                btnVideoStart.Enabled = false;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Camera Fail: " + ex.Message);
            }
        }

        private void BtnVideoStop_Click(object sender, EventArgs e)
        {
            btnVideoStart.Enabled = true;
            if (MainV2.cam != null)
            {
                MainV2.cam.Dispose();
                MainV2.cam = null;
            }
        }

        private void ChkHudShow_CheckedChanged(object sender, EventArgs e)
        {
            GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;
            Settings.Instance["CHK_hudshow"] = chkHudShow.Checked.ToString();
        }

        private void CmbOsdColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            if (cmbOsdColor.Text != "")
            {
                Settings.Instance["hudcolor"] = cmbOsdColor.Text;
                GCSViews.FlightData.myhud.hudcolor =
                    Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), cmbOsdColor.Text));
            }
        }

        private void CmbOsdColor_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            var g = e.Graphics;
            var rect = e.Bounds;
            Brush brush = null;

            if ((e.State & DrawItemState.Selected) == 0)
                brush = new SolidBrush(cmbOsdColor.BackColor);
            else
                brush = SystemBrushes.Highlight;

            g.FillRectangle(brush, rect);

            brush = new SolidBrush(Color.FromName((string)cmbOsdColor.Items[e.Index]));

            g.FillRectangle(brush, rect.X + 2, rect.Y + 2, 30, rect.Height - 4);
            g.DrawRectangle(Pens.Black, rect.X + 2, rect.Y + 2, 30, rect.Height - 4);

            if ((e.State & DrawItemState.Selected) == 0)
                brush = new SolidBrush(cmbOsdColor.ForeColor);
            else
                brush = SystemBrushes.HighlightText;
            g.DrawString(cmbOsdColor.Items[e.Index].ToString(),
                cmbOsdColor.Font, brush, rect.X + 35, rect.Top + rect.Height - cmbOsdColor.Font.Height);
        }

        public class GCSBitmapInfo
        {
            public GCSBitmapInfo(int width, int height, long fps, string standard, AMMediaType media)
            {
                Width = width;
                Height = height;
                Fps = fps;
                Standard = standard;
                Media = media;
            }

            public int Width { get; set; }
            public int Height { get; set; }
            public long Fps { get; set; }
            public string Standard { get; set; }
            public AMMediaType Media { get; set; }

            public override string ToString()
            {
                return Width + " x " + Height + string.Format(" {0:0.00} fps ", 10000000.0 / Fps) + Standard;
            }
        }
    }
}
