using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DirectShowLib;
using MissionPlanner.Controls;
using MissionPlanner.Joystick;
using MissionPlanner.Utilities;
using WebCamService;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigPlanner : UserControl, IActivate
    {
        private List<CultureInfo> _languages;
        private bool startup;

        public ConfigPlanner()
        {
            InitializeComponent();
            txt_log_dir.TextChanged += OnLogDirTextChanged;

        }


        // Called every time that this control is made current in the backstage view
        public void Activate()
        {
            startup = true; // flag to ignore changes while we programatically populate controls


            CMB_osdcolor.DataSource = Enum.GetNames(typeof (KnownColor));

            // set distance/speed unit states
            CMB_distunits.DataSource = Enum.GetNames(typeof (Common.distances));
            CMB_speedunits.DataSource = Enum.GetNames(typeof (Common.speeds));

            CMB_theme.DataSource = Enum.GetNames(typeof (ThemeManager.Themes));

            CMB_theme.Text = ThemeManager.CurrentTheme.ToString();

            // setup language selection
            var cultureCodes = new[]
            {"en-US", "zh-Hans", "zh-TW", "ru-RU", "Fr", "Pl", "it-IT", "es-ES", "de-DE", "ja-JP", "id-ID", "ko-KR"};

            _languages = cultureCodes
                .Select(CultureInfoEx.GetCultureInfo)
                .Where(c => c != null)
                .ToList();

            CMB_language.DisplayMember = "DisplayName";
            CMB_language.DataSource = _languages;
            var currentUiCulture = Thread.CurrentThread.CurrentUICulture;

            for (var i = 0; i < _languages.Count; i++)
            {
                if (currentUiCulture.IsChildOf(_languages[i]))
                {
                    try
                    {
                        CMB_language.SelectedIndex = i;
                    }
                    catch
                    {
                    }
                    break;
                }
            }

            // setup up camera button states
            if (MainV2.cam != null)
            {
                BUT_videostart.Enabled = false;
                CHK_hudshow.Checked = FlightData.myhud.hudon;
            }
            else
            {
                BUT_videostart.Enabled = true;
            }

            // setup speech states
            SetCheckboxFromConfig("speechenable", CHK_enablespeech);
            SetCheckboxFromConfig("speechwaypointenabled", CHK_speechwaypoint);
            SetCheckboxFromConfig("speechmodeenabled", CHK_speechmode);
            SetCheckboxFromConfig("speechcustomenabled", CHK_speechcustom);
            SetCheckboxFromConfig("speechbatteryenabled", CHK_speechbattery);
            SetCheckboxFromConfig("speechaltenabled", CHK_speechaltwarning);
            SetCheckboxFromConfig("speecharmenabled", CHK_speecharmdisarm);
            SetCheckboxFromConfig("speechlowspeedenabled", CHK_speechlowspeed);
            SetCheckboxFromConfig("beta_updates", CHK_beta);
            SetCheckboxFromConfig("password_protect", CHK_Password);
            SetCheckboxFromConfig("advancedview", CHK_advancedview);
            SetCheckboxFromConfig("showairports", CHK_showairports);
            SetCheckboxFromConfig("enableadsb", chk_ADSB);

            // this can't fail because it set at startup
            NUM_tracklength.Value = Settings.Instance.GetInt32("NUM_tracklength");

            // get wps on connect
            SetCheckboxFromConfig("loadwpsonconnect", CHK_loadwponconnect);

            // setup other config state
            SetCheckboxFromConfig("CHK_resetapmonconnect", CHK_resetapmonconnect);

            CMB_rateattitude.Text = MainV2.comPort.MAV.cs.rateattitude.ToString();
            CMB_rateposition.Text = MainV2.comPort.MAV.cs.rateposition.ToString();
            CMB_raterc.Text = MainV2.comPort.MAV.cs.raterc.ToString();
            CMB_ratestatus.Text = MainV2.comPort.MAV.cs.ratestatus.ToString();
            CMB_ratesensors.Text = MainV2.comPort.MAV.cs.ratesensors.ToString();

            SetCheckboxFromConfig("analyticsoptout", chk_analytics);

            SetCheckboxFromConfig("CHK_GDIPlus", CHK_GDIPlus);
            SetCheckboxFromConfig("CHK_maprotation", CHK_maprotation);

            SetCheckboxFromConfig("CHK_disttohomeflightdata", CHK_disttohomeflightdata);

            //set hud color state
            var hudcolor = Settings.Instance["hudcolor"];
            if (hudcolor != null)
            {
                var index = CMB_osdcolor.Items.IndexOf(hudcolor ?? "White");
                try
                {
                    CMB_osdcolor.SelectedIndex = index;
                }
                catch
                {
                }
            }


            if (Settings.Instance["distunits"] != null)
                CMB_distunits.Text = Settings.Instance["distunits"].ToString();
            if (Settings.Instance["speedunits"] != null)
                CMB_speedunits.Text = Settings.Instance["speedunits"].ToString();

            try
            {
                if (Settings.Instance["video_device"] != null)
                {
                    CMB_videosources_Click(this, null);
                    CMB_videosources.SelectedIndex = Settings.Instance.GetInt32("video_device");

                    if (Settings.Instance["video_options"] != "" && CMB_videosources.Text != "")
                    {
                        CMB_videoresolutions.SelectedIndex = Settings.Instance.GetInt32("video_options");
                    }
                }
            }
            catch
            {
            }


            txt_log_dir.Text = Settings.Instance.LogDir;

            startup = false;
        }

        private void BUT_videostart_Click(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

            // stop first
            BUT_videostop_Click(sender, e);

            var bmp = (GCSBitmapInfo) CMB_videoresolutions.SelectedItem;

            try
            {
                MainV2.cam = new Capture(CMB_videosources.SelectedIndex, bmp.Media);

                MainV2.cam.Start();

                Settings.Instance["video_device"] = CMB_videosources.SelectedIndex.ToString();

                Settings.Instance["video_options"] = CMB_videoresolutions.SelectedIndex.ToString();

                BUT_videostart.Enabled = false;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Camera Fail: " + ex.Message);
            }
        }

        private void BUT_videostop_Click(object sender, EventArgs e)
        {
            BUT_videostart.Enabled = true;
            if (MainV2.cam != null)
            {
                MainV2.cam.Dispose();
                MainV2.cam = null;
            }
        }

        private void CMB_videosources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

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

            // Get the ICaptureGraphBuilder2
            capGraph = (ICaptureGraphBuilder2) new CaptureGraphBuilder2();
            var m_FilterGraph = (IFilterGraph2) new FilterGraph();

            DsDevice[] capDevices;
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            // Add the video device
            hr = m_FilterGraph.AddSourceFilterForMoniker(capDevices[CMB_videosources.SelectedIndex].Mon, null,
                "Video input", out capFilter);
            try
            {
                DsError.ThrowExceptionForHR(hr);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Can not add video source\n" + ex);
                return;
            }

            // Find the stream config interface
            hr = capGraph.FindInterface(PinCategory.Capture, MediaType.Video, capFilter, typeof (IAMStreamConfig).GUID,
                out o);
            DsError.ThrowExceptionForHR(hr);

            var videoStreamConfig = o as IAMStreamConfig;
            if (videoStreamConfig == null)
            {
                CustomMessageBox.Show("Failed to get IAMStreamConfig");
                return;
            }

            hr = videoStreamConfig.GetNumberOfCapabilities(out count, out size);
            DsError.ThrowExceptionForHR(hr);
            var TaskMemPointer = Marshal.AllocCoTaskMem(size);
            for (var i = 0; i < count; i++)
            {
                var ptr = IntPtr.Zero;

                hr = videoStreamConfig.GetStreamCaps(i, out media, TaskMemPointer);
                v = (VideoInfoHeader) Marshal.PtrToStructure(media.formatPtr, typeof (VideoInfoHeader));
                c = (VideoStreamConfigCaps) Marshal.PtrToStructure(TaskMemPointer, typeof (VideoStreamConfigCaps));
                modes.Add(new GCSBitmapInfo(v.BmiHeader.Width, v.BmiHeader.Height, c.MaxFrameInterval,
                    c.VideoStandard.ToString(), media));
            }
            Marshal.FreeCoTaskMem(TaskMemPointer);
            DsUtils.FreeAMMediaType(media);

            CMB_videoresolutions.DataSource = modes;

            if (Settings.Instance["video_options"] != "" && CMB_videosources.Text != "")
            {
                try
                {
                    CMB_videoresolutions.SelectedIndex = Settings.Instance.GetInt32("video_options");
                }
                catch
                {
                } // ignore bad entries
            }
        }

        private void CHK_hudshow_CheckedChanged(object sender, EventArgs e)
        {
            FlightData.myhud.hudon = CHK_hudshow.Checked;
            Settings.Instance["CHK_hudshow"] = CHK_hudshow.Checked.ToString();
        }

        private void CHK_enablespeech_CheckedChanged(object sender, EventArgs e)
        {
            MainV2.speechEnable = CHK_enablespeech.Checked;
            Settings.Instance["speechenable"] = CHK_enablespeech.Checked.ToString();
            if (MainV2.speechEngine != null)
                MainV2.speechEngine.SpeakAsyncCancelAll();

            if (CHK_enablespeech.Checked)
            {
                CHK_speechwaypoint.Visible = true;
                CHK_speechaltwarning.Visible = true;
                CHK_speechbattery.Visible = true;
                CHK_speechcustom.Visible = true;
                CHK_speechmode.Visible = true;
                CHK_speecharmdisarm.Visible = true;
                CHK_speechlowspeed.Visible = true;
            }
            else
            {
                CHK_speechwaypoint.Visible = false;
                CHK_speechaltwarning.Visible = false;
                CHK_speechbattery.Visible = false;
                CHK_speechcustom.Visible = false;
                CHK_speechmode.Visible = false;
                CHK_speecharmdisarm.Visible = false;
                CHK_speechlowspeed.Visible = false;
            }
        }

        private void CMB_language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.instance.changelanguage((CultureInfo) CMB_language.SelectedItem);

            MessageBox.Show("Please Restart the Planner");

            MainV2.instance.Close();
            //Application.Exit();
        }

        private void CMB_osdcolor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            if (CMB_osdcolor.Text != "")
            {
                Settings.Instance["hudcolor"] = CMB_osdcolor.Text;
                FlightData.myhud.hudcolor =
                    Color.FromKnownColor((KnownColor) Enum.Parse(typeof (KnownColor), CMB_osdcolor.Text));
            }
        }

        private void CHK_speechwaypoint_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechwaypointenabled"] = ((CheckBox) sender).Checked.ToString();

            if (((CheckBox) sender).Checked)
            {
                var speechstring = "Heading to Waypoint {wpn}";
                if (Settings.Instance["speechwaypoint"] != null)
                    speechstring = Settings.Instance["speechwaypoint"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Notification", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speechwaypoint"] = speechstring;
            }
        }

        private void CHK_speechmode_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechmodeenabled"] = ((CheckBox) sender).Checked.ToString();

            if (((CheckBox) sender).Checked)
            {
                var speechstring = "Mode changed to {mode}";
                if (Settings.Instance["speechmode"] != null)
                    speechstring = Settings.Instance["speechmode"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Notification", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speechmode"] = speechstring;
            }
        }

        private void CHK_speechcustom_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechcustomenabled"] = ((CheckBox) sender).Checked.ToString();

            if (((CheckBox) sender).Checked)
            {
                var speechstring = "Heading to Waypoint {wpn}, altitude is {alt}, Ground speed is {gsp} ";
                if (Settings.Instance["speechcustom"] != null)
                    speechstring = Settings.Instance["speechcustom"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Notification", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speechcustom"] = speechstring;
            }
        }

        private void BUT_rerequestparams_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;
            ((MyButton) sender).Enabled = false;
            try
            {
                MainV2.comPort.getParamList();
            }
            catch
            {
                CustomMessageBox.Show("Error: getting param list");
            }


            ((MyButton) sender).Enabled = true;
            startup = true;


            startup = false;
        }

        private void CHK_speechbattery_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechbatteryenabled"] = ((CheckBox) sender).Checked.ToString();

            if (((CheckBox) sender).Checked)
            {
                var speechstring = "WARNING, Battery at {batv} Volt, {batp} percent";
                if (Settings.Instance["speechbattery"] != null)
                    speechstring = Settings.Instance["speechbattery"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Notification", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speechbattery"] = speechstring;

                speechstring = "9.6";
                if (Settings.Instance["speechbatteryvolt"] != null)
                    speechstring = Settings.Instance["speechbatteryvolt"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Battery Level", "What Voltage do you want to warn at?", ref speechstring))
                    return;
                Settings.Instance["speechbatteryvolt"] = speechstring;

                speechstring = "20";
                if (Settings.Instance["speechbatterypercent"] != null)
                    speechstring = Settings.Instance["speechbatterypercent"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Battery Level", "What percentage do you want to warn at?", ref speechstring))
                    return;
                Settings.Instance["speechbatterypercent"] = speechstring;
            }
        }

        private void BUT_Joystick_Click(object sender, EventArgs e)
        {
            Form joy = new JoystickSetup();
            ThemeManager.ApplyThemeTo(joy);
            joy.Show();
        }

        private void CMB_distunits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["distunits"] = CMB_distunits.Text;
            MainV2.instance.ChangeUnits();
        }

        private void CMB_speedunits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speedunits"] = CMB_speedunits.Text;
            MainV2.instance.ChangeUnits();
        }

        private void CMB_rateattitude_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance[((ComboBox) sender).Name] = ((ComboBox) sender).Text;
            MainV2.comPort.MAV.cs.rateattitude = byte.Parse(((ComboBox) sender).Text);

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA1, MainV2.comPort.MAV.cs.rateattitude);
            // request attitude
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA2, MainV2.comPort.MAV.cs.rateattitude);
            // request vfr
        }

        private void CMB_rateposition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance[((ComboBox) sender).Name] = ((ComboBox) sender).Text;
            MainV2.comPort.MAV.cs.rateposition = byte.Parse(((ComboBox) sender).Text);

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, MainV2.comPort.MAV.cs.rateposition);
            // request gps
        }

        private void CMB_ratestatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance[((ComboBox) sender).Name] = ((ComboBox) sender).Text;
            MainV2.comPort.MAV.cs.ratestatus = byte.Parse(((ComboBox) sender).Text);

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTENDED_STATUS, MainV2.comPort.MAV.cs.ratestatus);
            // mode
        }

        private void CMB_raterc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance[((ComboBox) sender).Name] = ((ComboBox) sender).Text;
            MainV2.comPort.MAV.cs.raterc = byte.Parse(((ComboBox) sender).Text);

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, MainV2.comPort.MAV.cs.raterc);
            // request rc info 
        }

        private void CMB_ratesensors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance[((ComboBox) sender).Name] = ((ComboBox) sender).Text;
            MainV2.comPort.MAV.cs.ratesensors = byte.Parse(((ComboBox) sender).Text);

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA3, MainV2.comPort.MAV.cs.ratesensors);
            // request extra stuff - tridge
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors);
            // request raw sensor
        }

        private void CHK_mavdebug_CheckedChanged(object sender, EventArgs e)
        {
            MainV2.comPort.debugmavlink = CHK_mavdebug.Checked;
        }

        private void CHK_resetapmonconnect_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance[((CheckBox) sender).Name] = ((CheckBox) sender).Checked.ToString();
        }

        private void CHK_speechaltwarning_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechaltenabled"] = ((CheckBox) sender).Checked.ToString();

            if (((CheckBox) sender).Checked)
            {
                var speechstring = "WARNING, low altitude {alt}";
                if (Settings.Instance["speechalt"] != null)
                    speechstring = Settings.Instance["speechalt"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Notification", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speechalt"] = speechstring;

                speechstring = "2";
                if (Settings.Instance["speechaltheight"] != null)
                    speechstring = Settings.Instance["speechaltheight"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Min Alt", "What altitude do you want to warn at? (relative to home)",
                        ref speechstring))
                    return;
                Settings.Instance["speechaltheight"] = (double.Parse(speechstring)/CurrentState.multiplierdist).ToString();
                // save as m
            }
        }

        private void NUM_tracklength_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["NUM_tracklength"] = NUM_tracklength.Value.ToString();
        }

        private void CHK_loadwponconnect_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["loadwpsonconnect"] = CHK_loadwponconnect.Checked.ToString();
        }

        private void CHK_GDIPlus_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            CustomMessageBox.Show("You need to restart the planner for this to take effect");
            Settings.Instance["CHK_GDIPlus"] = CHK_GDIPlus.Checked.ToString();
        }

        // This load handler now only contains code that should execute once
        // on start up. See Activate() for the remainder
        private void ConfigPlanner_Load(object sender, EventArgs e)
        {
        }

        private void CMB_osdcolor_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            var g = e.Graphics;
            var rect = e.Bounds;
            Brush brush = null;

            if ((e.State & DrawItemState.Selected) == 0)
                brush = new SolidBrush(CMB_osdcolor.BackColor);
            else
                brush = SystemBrushes.Highlight;

            g.FillRectangle(brush, rect);

            brush = new SolidBrush(Color.FromName((string) CMB_osdcolor.Items[e.Index]));

            g.FillRectangle(brush, rect.X + 2, rect.Y + 2, 30, rect.Height - 4);
            g.DrawRectangle(Pens.Black, rect.X + 2, rect.Y + 2, 30, rect.Height - 4);

            if ((e.State & DrawItemState.Selected) == 0)
                brush = new SolidBrush(CMB_osdcolor.ForeColor);
            else
                brush = SystemBrushes.HighlightText;
            g.DrawString(CMB_osdcolor.Items[e.Index].ToString(),
                CMB_osdcolor.Font, brush, rect.X + 35, rect.Top + rect.Height - CMB_osdcolor.Font.Height);
        }

        private void CMB_videosources_Click(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;
            // the reason why i dont populate this list is because on linux/mac this call will fail.
            var capt = new Capture();

            var devices = WebCamService.Capture.getDevices();

            CMB_videosources.DataSource = devices;

            capt.Dispose();
        }

        private void CHK_maprotation_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["CHK_maprotation"] = CHK_maprotation.Checked.ToString();
        }

        private static void SetCheckboxFromConfig(string configKey, CheckBox chk)
        {
            if (Settings.Instance[configKey] != null)
                chk.Checked = Settings.Instance.GetBoolean(configKey);
        }

        private void CHK_disttohomeflightdata_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["CHK_disttohomeflightdata"] = CHK_disttohomeflightdata.Checked.ToString();
        }

        private void BUT_logdirbrowse_Click(object sender, EventArgs e)
        {
            var ofd = new FolderBrowserDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txt_log_dir.Text = ofd.SelectedPath;
            }
        }

        private void OnLogDirTextChanged(object sender, EventArgs e)
        {
            string path = txt_log_dir.Text;
            if (!string.IsNullOrEmpty(path) && System.IO.Directory.Exists(path))
            {
                Settings.Instance.LogDir = path;
            }
        }

        private void CMB_theme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            Settings.Instance["theme"] = CMB_theme.Text;
            ThemeManager.SetTheme((ThemeManager.Themes) Enum.Parse(typeof (ThemeManager.Themes), CMB_theme.Text));
            ThemeManager.ApplyThemeTo(MainV2.instance);

            CustomMessageBox.Show("You may need to restart to see the full effect.");
        }

        private void BUT_themecustom_Click(object sender, EventArgs e)
        {
            ThemeManager.CustomColor();
            CMB_theme.Text = "Custom";
        }

        private void CHK_speecharmdisarm_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speecharmenabled"] = ((CheckBox) sender).Checked.ToString();

            if (((CheckBox) sender).Checked)
            {
                var speechstring = "Armed";
                if (Settings.Instance["speecharm"] != null)
                    speechstring = Settings.Instance["speecharm"];
                if (DialogResult.Cancel == InputBox.Show("Arm", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speecharm"] = speechstring;

                speechstring = "Disarmed";
                if (Settings.Instance["speechdisarm"] != null)
                    speechstring = Settings.Instance["speechdisarm"];
                if (DialogResult.Cancel == InputBox.Show("Disarmed", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speechdisarm"] = speechstring;
            }
        }

        private void BUT_Vario_Click(object sender, EventArgs e)
        {
            if (Vario.run)
            {
                Vario.Stop();
            }
            else
            {
                Vario.Start();
            }
        }

        private void chk_analytics_CheckedChanged(object sender, EventArgs e)
        {
            Tracking.OptOut = chk_analytics.Checked;
            Settings.Instance["analyticsoptout"] = chk_analytics.Checked.ToString();
        }

        private void CHK_beta_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["beta_updates"] = CHK_beta.Checked.ToString();
        }

        private void CHK_Password_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            Settings.Instance["password_protect"] = CHK_Password.Checked.ToString();
            if (CHK_Password.Checked)
            {
                Password.EnterPassword();
            }
        }

        private void CHK_speechlowspeed_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechlowspeedenabled"] = ((CheckBox) sender).Checked.ToString();

            if (((CheckBox) sender).Checked)
            {
                var speechstring = "Low Ground Speed {gsp}";
                if (Settings.Instance["speechlowgroundspeed"] != null)
                    speechstring = Settings.Instance["speechlowgroundspeed"];
                if (DialogResult.Cancel ==
                    InputBox.Show("Ground Speed", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speechlowgroundspeed"] = speechstring;

                speechstring = "0";
                if (Settings.Instance["speechlowgroundspeedtrigger"] != null)
                    speechstring = Settings.Instance["speechlowgroundspeedtrigger"];
                if (DialogResult.Cancel ==
                    InputBox.Show("speed trigger", "What speed do you want to warn at (m/s)?", ref speechstring))
                    return;
                Settings.Instance["speechlowgroundspeedtrigger"] = speechstring;

                speechstring = "Low Air Speed {asp}";
                if (Settings.Instance["speechlowairspeed"] != null)
                    speechstring = Settings.Instance["speechlowairspeed"];
                if (DialogResult.Cancel == InputBox.Show("Air Speed", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speechlowairspeed"] = speechstring;

                speechstring = "0";
                if (Settings.Instance["speechlowairspeedtrigger"] != null)
                    speechstring = Settings.Instance["speechlowairspeedtrigger"];
                if (DialogResult.Cancel ==
                    InputBox.Show("speed trigger", "What speed do you want to warn at (m/s)?", ref speechstring))
                    return;
                Settings.Instance["speechlowairspeedtrigger"] = speechstring;
            }
        }

        private void CHK_advancedview_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["advancedview"] = CHK_advancedview.Checked.ToString();
            MainV2.Advanced = CHK_advancedview.Checked;
        }

        private void CHK_showairports_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["showairports"] = CHK_showairports.Checked.ToString();
            MainV2.ShowAirports = CHK_showairports.Checked;
        }

        private void chk_ADSB_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            if (((CheckBox) sender).Checked)
            {
                var server = "127.0.0.1";
                if (Settings.Instance["adsbserver"] != null)
                    server = Settings.Instance["adsbserver"];
                if (DialogResult.Cancel == InputBox.Show("Server", "Server IP?", ref server))
                    return;
                Settings.Instance["adsbserver"] = server;

                var port = "30003";
                if (Settings.Instance["adsbport"] != null)
                    port = Settings.Instance["adsbport"];
                if (DialogResult.Cancel == InputBox.Show("Server port", "Server port?", ref port))
                    return;
                Settings.Instance["adsbport"] = port;
            }

            Settings.Instance["enableadsb"] = chk_ADSB.Checked.ToString();
            MainV2.instance.EnableADSB = CHK_showairports.Checked;
        }

        private void chk_tfr_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["showtfr"] = chk_tfr.Checked.ToString();
            MainV2.ShowTFR = chk_tfr.Checked;
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
                return Width + " x " + Height + string.Format(" {0:0.00} fps ", 10000000.0/Fps) + Standard;
            }
        }

        private void chk_temp_CheckedChanged(object sender, EventArgs e)
        {
            var temp = new temp();
            temp.Show();
        }
    }
}