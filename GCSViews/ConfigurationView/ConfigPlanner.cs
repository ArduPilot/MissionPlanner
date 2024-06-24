using DirectShowLib;
using MissionPlanner.Controls;
using MissionPlanner.Joystick;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WebCamService;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigPlanner : MyUserControl, IActivate
    {
        private List<CultureInfo> _languages;
        private bool startup;
        static temp temp;

        public ConfigPlanner()
        {
            startup = true;

            InitializeComponent();
            CMB_Layout.Items.Add(DisplayNames.Basic);
            CMB_Layout.Items.Add(DisplayNames.Advanced);
            CMB_Layout.Items.Add(DisplayNames.Custom);

            txt_log_dir.TextChanged += OnLogDirTextChanged;

            CMB_severity.Items.Add(SeverityLevel.Emergency);
            CMB_severity.Items.Add(SeverityLevel.Alert);
            CMB_severity.Items.Add(SeverityLevel.Critical);
            CMB_severity.Items.Add(SeverityLevel.Error);
            CMB_severity.Items.Add(SeverityLevel.Warning);
            CMB_severity.Items.Add(SeverityLevel.Notice);
            CMB_severity.Items.Add(SeverityLevel.Info);
            CMB_severity.Items.Add(SeverityLevel.Debug);

            cmb_secondarydisplaystyle.DataSource = Enum.GetNames(typeof(Maps.GMapMarkerBase.InactiveDisplayStyleEnum));
            cmb_secondarydisplaystyle.Text = Settings.Instance.GetString(
                "GMapMarkerBase_InactiveDisplayStyle",
                Maps.GMapMarkerBase.InactiveDisplayStyleEnum.Normal.ToString()
            );
        }


        // Called every time that this control is made current in the backstage view
        public void Activate()
        {
            startup = true; // flag to ignore changes while we programatically populate controls
            if (MainV2.DisplayConfiguration.displayName == DisplayNames.Advanced)
            {
                CMB_Layout.SelectedIndex = 1;
            }
            else if (MainV2.DisplayConfiguration.displayName == DisplayNames.Basic)
            {
                CMB_Layout.SelectedIndex = 0;
            }
            else if (MainV2.DisplayConfiguration.displayName == DisplayNames.Custom)
            {
                CMB_Layout.SelectedIndex = 2;
            }
            else
            {
                CMB_Layout.SelectedIndex = 0;
            }

            if (!MainV2.DisplayConfiguration.displayPlannerLayout)
            {
                label5.Visible = false;
                CMB_Layout.Visible = false;
            }

            CMB_osdcolor.DataSource = Enum.GetNames(typeof(KnownColor));

            // set distance/speed unit states
            CMB_distunits.DataSource = Enum.GetNames(typeof(distances));
            CMB_speedunits.DataSource = Enum.GetNames(typeof(speeds));
            CMB_altunits.DataSource = Enum.GetNames(typeof(altitudes));

            CMB_theme.DataSource = ThemeManager.ThemeNames;

            CMB_theme.Text = ThemeManager.thmColor.strThemeName;

            num_gcsid.Value = MAVLinkInterface.gcssysid;

            // setup severity selection
            if (Settings.Instance["severity"] != null)
            {
                CMB_severity.SelectedIndex = Settings.Instance.GetInt32("severity");
            }
            else
            {
                CMB_severity.SelectedIndex = 4;  // SeverityLevel.Warning
                Settings.Instance["severity"] = CMB_severity.SelectedIndex.ToString();
            }

            // setup language selection
            var cultureCodes = new[]
            {
                "en-US", "zh-Hans", "zh-TW", "ru-RU", "Fr", "Pl", "it-IT", "es-ES", "de-DE", "ja-JP", "id-ID", "ko-KR",
                "ar", "pt", "tr", "ru-KZ", "uk"
            };

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
            SetCheckboxFromConfig("showairports", CHK_showairports);
            SetCheckboxFromConfig("enableadsb", chk_ADSB);
            SetCheckboxFromConfig("norcreceiver", chk_norcreceiver);
            SetCheckboxFromConfig("showtfr", chk_tfr);
            SetCheckboxFromConfig("autoParamCommit", CHK_AutoParamCommit);
            SetCheckboxFromConfig("ShowNoFly", chk_shownofly);
            SetCheckboxFromConfig("Params_BG", CHK_params_bg);
            SetCheckboxFromConfig("SlowMachine", chk_slowMachine);
            SetCheckboxFromConfig("speech_armed_only", CHK_speechArmedOnly);

            // this can't fail because it set at startup
            NUM_tracklength.Value = Settings.Instance.GetInt32("NUM_tracklength", 200);

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

            CHK_AutoParamCommit.Visible = MainV2.DisplayConfiguration.displayParamCommitButton;

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
            if (Settings.Instance["altunits"] != null)
                CMB_altunits.Text = Settings.Instance["altunits"].ToString();

            try
            {
                if (Settings.Instance["video_device"] != null)
                {
                    CMB_videosources_Click(this, null);
                    var device = Settings.Instance.GetInt32("video_device");
                    if(CMB_videosources.Items.Count > device)
                        CMB_videosources.SelectedIndex = device;

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

            // Setup aircraft icon settings
            chk_displaycog.Checked = Settings.Instance.GetBoolean("GMapMarkerBase_DisplayCOG", true);
            chk_displayheading.Checked = Settings.Instance.GetBoolean("GMapMarkerBase_DisplayHeading", true);
            chk_displaynavbearing.Checked = Settings.Instance.GetBoolean("GMapMarkerBase_DisplayNavBearing", true);
            chk_displayradius.Checked = Settings.Instance.GetBoolean("GMapMarkerBase_DisplayRadius", true);
            chk_displaytarget.Checked = Settings.Instance.GetBoolean("GMapMarkerBase_DisplayTarget", true);
            chk_displaytooltip.Checked = Settings.Instance.GetString("mapicondesc", "") != "";
            num_linelength.Value = Settings.Instance.GetInt32("GMapMarkerBase_Length", 500);

            CMB_mapCache.DataSource = Enum.GetNames(typeof(GMap.NET.AccessMode));
            try
            {
                CMB_mapCache.SelectedIndex = CMB_mapCache.Items.IndexOf(Settings.Instance["mapCache"] ?? GMap.NET.GMaps.Instance.Mode.ToString());
            }
            catch
            {
            }

            startup = false;
        }

        private void BUT_videostart_Click(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

            // stop first
            BUT_videostop_Click(sender, e);

            var bmp = (GCSBitmapInfo)CMB_videoresolutions.SelectedItem;

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
            capGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            var m_FilterGraph = (IFilterGraph2)new FilterGraph();

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
            hr = capGraph.FindInterface(PinCategory.Capture, MediaType.Video, capFilter, typeof(IAMStreamConfig).GUID,
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
                v = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
                c = (VideoStreamConfigCaps)Marshal.PtrToStructure(TaskMemPointer, typeof(VideoStreamConfigCaps));
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
                CHK_speechArmedOnly.Visible = true;
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
                CHK_speechArmedOnly.Visible = false;
                CHK_speechwaypoint.Visible = false;
                CHK_speechaltwarning.Visible = false;
                CHK_speechbattery.Visible = false;
                CHK_speechcustom.Visible = false;
                CHK_speechmode.Visible = false;
                CHK_speecharmdisarm.Visible = false;
                CHK_speechlowspeed.Visible = false;
            }
        }

        private void CMB_severity_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance["severity"] = CMB_severity.SelectedIndex.ToString();
        }

        private void CMB_language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.instance.changelanguage((CultureInfo)CMB_language.SelectedItem);

            MessageBox.Show("Please Restart the Planner");

            MainV2.instance.Close();
            //Application.Exit();
        }

        private void CMB_osdcolor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
/*
            if (CMB_osdcolor.Text != "")
            {
                Settings.Instance["hudcolor"] = CMB_osdcolor.Text;
                FlightData.myhud.hudcolor =
                    Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), CMB_osdcolor.Text));
            }
            */
        }

        private void CHK_speechwaypoint_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechwaypointenabled"] = ((CheckBox)sender).Checked.ToString();

            if (((CheckBox)sender).Checked)
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
            Settings.Instance["speechmodeenabled"] = ((CheckBox)sender).Checked.ToString();

            if (((CheckBox)sender).Checked)
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
            Settings.Instance["speechcustomenabled"] = ((CheckBox)sender).Checked.ToString();

            if (((CheckBox)sender).Checked)
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
            ((MyButton)sender).Enabled = false;
            try
            {
                MainV2.comPort.getParamList();
            }
            catch
            {
                CustomMessageBox.Show("Error: getting param list");
            }


            ((MyButton)sender).Enabled = true;
            startup = true;


            startup = false;
        }

        private void CHK_speechbattery_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechbatteryenabled"] = ((CheckBox)sender).Checked.ToString();

            if (((CheckBox)sender).Checked)
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
            new JoystickSetup().ShowUserControl();
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
            Settings.Instance[((ComboBox)sender).Name] = ((ComboBox)sender).Text;
            MainV2.comPort.MAV.cs.rateattitude = int.Parse(((ComboBox)sender).Text);

            CurrentState.rateattitudebackup = MainV2.comPort.MAV.cs.rateattitude;

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA1, MainV2.comPort.MAV.cs.rateattitude);
            // request attitude
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA2, MainV2.comPort.MAV.cs.rateattitude);
            // request vfr
        }

        private void CMB_rateposition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance[((ComboBox)sender).Name] = ((ComboBox)sender).Text;
            MainV2.comPort.MAV.cs.rateposition = int.Parse(((ComboBox)sender).Text);

            CurrentState.ratepositionbackup = MainV2.comPort.MAV.cs.rateposition;

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, MainV2.comPort.MAV.cs.rateposition);
            // request gps
        }

        private void CMB_ratestatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance[((ComboBox)sender).Name] = ((ComboBox)sender).Text;
            MainV2.comPort.MAV.cs.ratestatus = int.Parse(((ComboBox)sender).Text);

            CurrentState.ratestatusbackup = MainV2.comPort.MAV.cs.ratestatus;

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTENDED_STATUS, MainV2.comPort.MAV.cs.ratestatus);
            // mode
        }

        private void CMB_raterc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance[((ComboBox)sender).Name] = ((ComboBox)sender).Text;
            MainV2.comPort.MAV.cs.raterc = int.Parse(((ComboBox)sender).Text);

            CurrentState.ratercbackup = MainV2.comPort.MAV.cs.raterc;

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, MainV2.comPort.MAV.cs.raterc);
            // request rc info 
        }

        private void CMB_ratesensors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance[((ComboBox)sender).Name] = ((ComboBox)sender).Text;
            MainV2.comPort.MAV.cs.ratesensors = int.Parse(((ComboBox)sender).Text);

            CurrentState.ratesensorsbackup = MainV2.comPort.MAV.cs.ratesensors;

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
            Settings.Instance[((CheckBox)sender).Name] = ((CheckBox)sender).Checked.ToString();
        }

        private void CHK_speechaltwarning_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechaltenabled"] = ((CheckBox)sender).Checked.ToString();

            if (((CheckBox)sender).Checked)
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
                Settings.Instance["speechaltheight"] = (double.Parse(speechstring) / CurrentState.multiplieralt).ToString();
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

            brush = new SolidBrush(Color.FromName((string)CMB_osdcolor.Items[e.Index]));

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
            if (CHK_maprotation.Checked)
            {
                chk_shownofly.Checked = false;
            }
            FlightData.instance.gMapControl1.Bearing = 0;
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

            ThemeManager.LoadTheme(CMB_theme.Text);
            ThemeManager.ApplyThemeTo(MainV2.instance);
            CustomMessageBox.Show("You may need to select another tab or restart to see the full effect.");
        }

        private void BUT_themecustom_Click(object sender, EventArgs e)
        {
            ThemeManager.StartThemeEditor();
        }

        private void CHK_speecharmdisarm_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speecharmenabled"] = ((CheckBox)sender).Checked.ToString();

            if (((CheckBox)sender).Checked)
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

            MissionPlanner.Utilities.Update.dobeta = CHK_beta.Checked;
        }

        private void CHK_Password_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            Settings.Instance["password_protect"] = CHK_Password.Checked.ToString();
            if (CHK_Password.Checked)
            {
                // keep this one local
                string pw = "";

                InputBox.Show("Enter Password", "Please enter a password", ref pw, true);

                Password.EnterPassword(pw);
            }
        }

        private void CHK_speechlowspeed_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["speechlowspeedenabled"] = ((CheckBox)sender).Checked.ToString();

            if (((CheckBox)sender).Checked)
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

        private void CHK_showairports_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["showairports"] = CHK_showairports.Checked.ToString();
            MainV2.ShowAirports = CHK_showairports.Checked;
        }

        private void chk_ADSB_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            if (((CheckBox)sender).Checked)
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
            MainV2.instance.EnableADSB = chk_ADSB.Checked;
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
                return Width + " x " + Height + string.Format(" {0:0.00} fps ", 10000000.0 / Fps) + Standard;
            }
        }

        private void chk_temp_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_temp.Checked)
            {
                temp = new temp();
                temp.FormClosing += chk_temp_FormClosing;
                temp.Show();
            }
            else
            {
                if (temp != null)
                {
                    temp.Close();
                }
            }
        }

        private void chk_temp_FormClosing(object sender, EventArgs e)
        {
            chk_temp.Checked = false;
        }

        private void chk_norcreceiver_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["norcreceiver"] = chk_norcreceiver.Checked.ToString();
        }

        private void CMB_Layout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((DisplayNames)CMB_Layout.SelectedItem == DisplayNames.Advanced)
            {
                MainV2.DisplayConfiguration = MainV2.DisplayConfiguration.Advanced();
            }
            else if ((DisplayNames)CMB_Layout.SelectedItem == DisplayNames.Basic)
            {
                MainV2.DisplayConfiguration = MainV2.DisplayConfiguration.Basic();
            }
            else if ((DisplayNames)CMB_Layout.SelectedItem == DisplayNames.Custom)
            {
                MainV2.DisplayConfiguration = MainV2.DisplayConfiguration.Custom();
            }
            Settings.Instance["displayview"] = MainV2.DisplayConfiguration.ConvertToString();
        }

        private void CHK_AutoParamCommit_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["autoParamCommit"] = CHK_AutoParamCommit.Checked.ToString();
        }

        private void chk_shownofly_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["ShowNoFly"] = chk_shownofly.Checked.ToString();
            if (chk_shownofly.Checked)
            {
                CHK_maprotation.Checked = false;
            }
        }

        private void CMB_altunits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["altunits"] = CMB_altunits.Text;
            MainV2.instance.ChangeUnits();
        }

        private void num_gcsid_ValueChanged(object sender, EventArgs e)
        {
            MAVLinkInterface.gcssysid = (byte)num_gcsid.Value;
            Settings.Instance["gcsid"] = num_gcsid.Value.ToString();
        }

        private void CHK_params_bg_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["Params_BG"] = CHK_params_bg.Checked.ToString();
        }

        private void chk_slowMachine_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["SlowMachine"] = chk_slowMachine.Checked.ToString();
        }

        private void CHK_speechArmedOnly_CheckedChanged(object sender, EventArgs e)
        {
            MainV2.speech_armed_only = CHK_speechArmedOnly.Checked;
            Settings.Instance["speech_armed_only"] = CHK_speechArmedOnly.Checked.ToString();
        }

        private void chk_displaycog_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["GMapMarkerBase_DisplayCOG"] = chk_displaycog.Checked.ToString();
            Maps.GMapMarkerBase.DisplayCOGSetting = chk_displaycog.Checked;
        }

        private void chk_displayheading_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["GMapMarkerBase_DisplayHeading"] = chk_displayheading.Checked.ToString();
            Maps.GMapMarkerBase.DisplayHeadingSetting = chk_displayheading.Checked;
        }

        private void chk_displaynavbearing_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["GMapMarkerBase_DisplayNavBearing"] = chk_displaynavbearing.Checked.ToString();
            Maps.GMapMarkerBase.DisplayNavBearingSetting = chk_displaynavbearing.Checked;
        }

        private void chk_displayradius_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["GMapMarkerBase_DisplayRadius"] = chk_displayradius.Checked.ToString();
            Maps.GMapMarkerBase.DisplayRadiusSetting = chk_displayradius.Checked;
        }

        private void chk_displaytarget_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["GMapMarkerBase_DisplayTarget"] = chk_displaytarget.Checked.ToString();
            Maps.GMapMarkerBase.DisplayTargetSetting = chk_displaytarget.Checked;
        }

        private void chk_displaytooltip_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
            {
                return;
            }
            if (chk_displaytooltip.Checked)
            {
                // Prompt user for text
                var descstring = Settings.Instance["mapicondesc_default",
                    "{alt}{altunit} {airspeed}{speedunit} id:{sysid} Sats:{satcount} HDOP:{gpshdop} Volts:{battery_voltage}"];

                if (DialogResult.Cancel == InputBox.Show("Description", "What do you want it to show?", ref descstring))
                {
                    return;
                }

                Settings.Instance["mapicondesc"] = descstring;
                Settings.Instance["mapicondesc_default"] = descstring;
            }
            else
            {
                Settings.Instance["mapicondesc"] = "";
            }
            
        }

        private void num_linelength_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["GMapMarkerBase_length"] = num_linelength.Value.ToString();
            Maps.GMapMarkerBase.length = (int)(num_linelength.Value);
        }

        private void cmb_secondarydisplaystyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(startup)
            {
                return;
            }
            if (Enum.TryParse(cmb_secondarydisplaystyle.Text,
                              out Maps.GMapMarkerBase.InactiveDisplayStyleEnum result))
            {
                Settings.Instance["GMapMarkerBase_InactiveDisplayStyle"] = cmb_secondarydisplaystyle.Text;
                Maps.GMapMarkerBase.InactiveDisplayStyle = result;
            }
            else
            {
                Settings.Instance["GMapMarkerBase_InactiveDisplayStyle"] = Maps.GMapMarkerBase.InactiveDisplayStyleEnum.Normal.ToString();
                Maps.GMapMarkerBase.InactiveDisplayStyle = Maps.GMapMarkerBase.InactiveDisplayStyleEnum.Normal;
            }
        }

        private void CMB_mapCache_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            Settings.Instance["mapCache"] = CMB_mapCache.Text;
            GMap.NET.GMaps.Instance.Mode = (GMap.NET.AccessMode)Enum.Parse(typeof(GMap.NET.AccessMode), Settings.Instance["mapCache"].ToString());
        }

        private void BUT_mapCacheDir_Click(object sender, EventArgs e)
        {
            try
            {
                string folderPath = MyImageCache.Instance.CacheLocation;
                if (Directory.Exists(folderPath))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        Arguments = folderPath,
                        FileName = "explorer.exe"
                    };

                    Process.Start(startInfo);
                }
                else
                {
                    MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
                }
            }
            catch (Exception)
            {
            }
        }
    }
}