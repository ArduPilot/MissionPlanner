using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DirectShowLib;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Joystick;
using MissionPlanner.Log;
using MissionPlanner.Utilities;
using MissionPlanner.Warnings;
using WebCamService;
using ZedGraph;
using LogAnalyzer = MissionPlanner.Utilities.LogAnalyzer;
using MissionPlanner.Maps;

// written by michael oborne

namespace MissionPlanner.GCSViews
{
    public partial class FlightData : MyUserControl, IActivate, IDeactivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool threadrun;
        int tickStart;
        RollingPointPairList list1 = new RollingPointPairList(1200);
        RollingPointPairList list2 = new RollingPointPairList(1200);
        RollingPointPairList list3 = new RollingPointPairList(1200);
        RollingPointPairList list4 = new RollingPointPairList(1200);
        RollingPointPairList list5 = new RollingPointPairList(1200);
        RollingPointPairList list6 = new RollingPointPairList(1200);
        RollingPointPairList list7 = new RollingPointPairList(1200);
        RollingPointPairList list8 = new RollingPointPairList(1200);
        RollingPointPairList list9 = new RollingPointPairList(1200);
        RollingPointPairList list10 = new RollingPointPairList(1200);

        PropertyInfo list1item;
        PropertyInfo list2item;
        PropertyInfo list3item;
        PropertyInfo list4item;
        PropertyInfo list5item;
        PropertyInfo list6item;
        PropertyInfo list7item;
        PropertyInfo list8item;
        PropertyInfo list9item;
        PropertyInfo list10item;

        CurveItem list1curve;
        CurveItem list2curve;
        CurveItem list3curve;
        CurveItem list4curve;
        CurveItem list5curve;
        CurveItem list6curve;
        CurveItem list7curve;
        CurveItem list8curve;
        CurveItem list9curve;
        CurveItem list10curve;

        internal static GMapOverlay tfrpolygons;
        public static GMapOverlay kmlpolygons;
        internal static GMapOverlay geofence;
        internal static GMapOverlay rallypointoverlay;
        internal static GMapOverlay photosoverlay;
        internal static GMapOverlay poioverlay = new GMapOverlay("POI"); // poi layer

        private Propagation prop;

        List<TabPage> TabListOriginal = new List<TabPage>();

        bool huddropout;
        bool huddropoutresize;

        Thread thisthread;

        //      private DockStateSerializer _serializer = null;

        List<PointLatLng> trackPoints = new List<PointLatLng>();

        public static HUD myhud;
        public static myGMAP mymap;

        bool playingLog;
        double LogPlayBackSpeed = 1.0;

        GMapMarker marker;

        AviWriter aviwriter;

        public SplitContainer MainHcopy;

        public static FlightData instance;

        //The file path of the selected script
        internal string selectedscript = "";
        //the thread the script is running on
        Thread scriptthread;
        //whether or not a script is running
        bool scriptrunning;
        Script script;
        //whether or not the output console has already started
        bool outputwindowstarted;

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentGMapMarker == null || !(CurrentGMapMarker is GMapMarkerPOI))
                return;

            POI.POIDelete((GMapMarkerPOI)CurrentGMapMarker);
        }

        private void addPoiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POI.POIAdd(MouseDownStart);
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POI.POISave();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            MainV2.comPort.logreadmode = false;
            try
            {
                if (hud1 != null)
                    Settings.Instance["FlightSplitter"] = MainH.SplitterDistance.ToString();
            }
            catch
            {
            }

            if (polygons != null)
                polygons.Dispose();
            if (routes != null)
                routes.Dispose();
            if (route != null)
                route.Dispose();
            if (marker != null)
                marker.Dispose();
            if (aviwriter != null)
                aviwriter.Dispose();

            if (prop != null)
                prop.Stop();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
        }

        public FlightData()
        {
            log.Info("Ctor Start");

            InitializeComponent();

            log.Info("Components Done");

            instance = this;
            //    _serializer = new DockStateSerializer(dockContainer1);
            //    _serializer.SavePath = Application.StartupPath + Path.DirectorySeparatorChar + "FDscreen.xml";
            //    dockContainer1.PreviewRenderer = new PreviewRenderer();
            //
            mymap = gMapControl1;
            myhud = hud1;
            MainHcopy = MainH;

            mymap.Paint += mymap_Paint;

            // populate the unmodified base list
            tabControlactions.TabPages.ForEach(i => { TabListOriginal.Add((TabPage)i); });

            // update tabs displayed
            loadTabControlActions();

            //  mymap.Manager.UseMemoryCache = false;

            log.Info("Tunning Graph Settings");
            // setup default tuning graph
            if (Settings.Instance["Tuning_Graph_Selected"] != null)
            {
                string line = Settings.Instance["Tuning_Graph_Selected"].ToString();
                string[] lines = line.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string option in lines)
                {
                    using (var cb = new CheckBox {Name = option, Checked = true})
                    {
                        chk_box_CheckedChanged(cb, EventArgs.Empty);
                    }
                }
            }
            else
            {
                using (var cb = new CheckBox {Name = "roll", Checked = true})
                {
                    chk_box_CheckedChanged(cb, EventArgs.Empty);
                }
                using (var cb = new CheckBox {Name = "pitch", Checked = true})
                {
                    chk_box_CheckedChanged(cb, EventArgs.Empty);
                }
                using (var cb = new CheckBox {Name = "nav_roll", Checked = true})
                {
                    chk_box_CheckedChanged(cb, EventArgs.Empty);
                }
                using (var cb = new CheckBox {Name = "nav_pitch", Checked = true})
                {
                    chk_box_CheckedChanged(cb, EventArgs.Empty);
                }
            }

            if (!string.IsNullOrEmpty(Settings.Instance["hudcolor"]))
            {
                hud1.hudcolor = Color.FromName(Settings.Instance["hudcolor"]);
            }

            log.Info("HUD Settings");
            foreach (string item in Settings.Instance.Keys)
            {
                if (item.StartsWith("hud1_useritem_"))
                {
                    string selection = item.Replace("hud1_useritem_", "");

                    CheckBox chk = new CheckBox();
                    chk.Name = selection;
                    chk.Checked = true;

                    HUD.Custom cust = new HUD.Custom();
                    cust.Header = Settings.Instance[item];
                    HUD.Custom.src = MainV2.comPort.MAV.cs;

                    addHudUserItem(ref cust, chk);

                    chk.Dispose();
                }
            }


            List<string> list = new List<string>();

            {
                list.Add("LOITER_UNLIM");
                list.Add("RETURN_TO_LAUNCH");
                list.Add("PREFLIGHT_CALIBRATION");
                list.Add("MISSION_START");
                list.Add("PREFLIGHT_REBOOT_SHUTDOWN");
                list.Add("Trigger Camera NOW");
                //DO_SET_SERVO
                //DO_REPEAT_SERVO
            }


            CMB_action.DataSource = list;

            CMB_modes.DataSource = Common.getModesList(MainV2.comPort.MAV.cs.firmware);
            CMB_modes.ValueMember = "Key";
            CMB_modes.DisplayMember = "Value";

            //default to auto
            CMB_modes.Text = "Auto";

            CMB_setwp.SelectedIndex = 0;

            log.Info("Graph Setup");
            CreateChart(zg1);

            // config map      
            log.Info("Map Setup");
            gMapControl1.CacheLocation = Settings.GetDataDirectory() +
                                         "gmapcache" + Path.DirectorySeparatorChar;
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 3;

            gMapControl1.OnMapZoomChanged += gMapControl1_OnMapZoomChanged;

            gMapControl1.DisableFocusOnMouseEnter = true;

            gMapControl1.OnMarkerEnter += gMapControl1_OnMarkerEnter;
            gMapControl1.OnMarkerLeave += gMapControl1_OnMarkerLeave;

            gMapControl1.RoutesEnabled = true;
            gMapControl1.PolygonsEnabled = true;

            tfrpolygons = new GMapOverlay("tfrpolygons");
            gMapControl1.Overlays.Add(tfrpolygons);

            kmlpolygons = new GMapOverlay("kmlpolygons");
            gMapControl1.Overlays.Add(kmlpolygons);

            geofence = new GMapOverlay("geofence");
            gMapControl1.Overlays.Add(geofence);

            polygons = new GMapOverlay("polygons");
            gMapControl1.Overlays.Add(polygons);

            photosoverlay = new GMapOverlay("photos overlay");
            gMapControl1.Overlays.Add(photosoverlay);

            routes = new GMapOverlay("routes");
            gMapControl1.Overlays.Add(routes);

            rallypointoverlay = new GMapOverlay("rally points");
            gMapControl1.Overlays.Add(rallypointoverlay);

            gMapControl1.Overlays.Add(poioverlay);

            float gspeedMax = Settings.Instance.GetFloat("GspeedMAX");
            if (gspeedMax != 0)
            {
                Gspeed.MaxValue = gspeedMax;
            }

            MainV2.comPort.ParamListChanged += FlightData_ParentChanged;

        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            updateBindingSourceWork();
        }

        void NoFly_NoFlyEvent(object sender, NoFly.NoFly.NoFlyEventArgs e)
        {
            Invoke((Action) delegate
            {
                foreach (var poly in e.NoFlyZones.Polygons)
                {
                    kmlpolygons.Polygons.Add(poly);
                }
            });
        }

        void mymap_Paint(object sender, PaintEventArgs e)
        {
            distanceBar1.DoPaintRemote(e);
        }

        internal GMapMarker CurrentGMapMarker;

        void gMapControl1_OnMarkerLeave(GMapMarker item)
        {
            CurrentGMapMarker = null;
        }

        void gMapControl1_OnMarkerEnter(GMapMarker item)
        {
            CurrentGMapMarker = item;
        }

        void tabStatus_Resize(object sender, EventArgs e)
        {
            // localise it
            //Control tabStatus = sender as Control;

            //  tabStatus.SuspendLayout();

            //foreach (Control temp in tabStatus.Controls)
            {
                //  temp.DataBindings.Clear();
                //temp.Dispose();
            }
            //tabStatus.Controls.Clear();

            int x = 10;
            int y = 10;

            var list = MainV2.comPort.MAV.cs.GetItemList();

            tabStatus.SuspendLayout();

            foreach (var field in list)
            {
                MyLabel lbl1 = null;
                MyLabel lbl2 = null;
                try
                {
                    var temp = tabStatus.Controls.Find(field, false);

                    if (temp.Length > 0)
                        lbl1 = (MyLabel) temp[0];

                    var temp2 = tabStatus.Controls.Find(field + "value", false);

                    if (temp2.Length > 0)
                        lbl2 = (MyLabel) temp2[0];
                }
                catch
                {
                }


                if (lbl1 == null)
                {
                    lbl1 = new MyLabel();
                    lbl1.Location = new Point(x, y);
                    lbl1.Size = new Size(90, 13);
                    lbl1.Text = field;
                    lbl1.Name = field;
                    lbl1.Visible = true;
                }

                if (lbl2 == null)
                {
                    lbl2 = new MyLabel();

                    lbl2.AutoSize = false;

                    lbl2.Location = new Point(lbl1.Right + 5, y);
                    lbl2.Size = new Size(50, 13);

                    lbl2.Name = field + "value";
                    lbl2.Visible = true;
                }

                if (lbl2.DataBindings.Count == 0)
                {
                    lbl2.DataBindings.Add(new Binding("Text", bindingSourceStatusTab, field, false,
                        DataSourceUpdateMode.Never, "0"));
                }
                //lbl2.Text = fieldValue.ToString();

                if (!tabStatus.Controls.Contains(lbl1))
                {
                    tabStatus.Controls.Add(lbl1);
                }
                if (!tabStatus.Controls.Contains(lbl2))
                {
                    tabStatus.Controls.Add(lbl2);
                }

                x += 0;
                y += 15;

                if (y > tabStatus.Height - 30)
                {
                    x = lbl2.Right + 10; //+= 165;
                    y = 10;
                }
            }

            tabStatus.ResumeLayout();
            tabStatus.Width = x;

            ThemeManager.ApplyThemeTo(tabStatus);
        }

        public void Activate()
        {
            log.Info("Activate Called");

            OnResize(EventArgs.Empty);

            if (CB_tuning.Checked)
                ZedGraphTimer.Start();

            hud1.altunit = CurrentState.AltUnit;
            hud1.speedunit = CurrentState.SpeedUnit;
            hud1.distunit = CurrentState.DistanceUnit;

            if (MainV2.MONO)
            {
                if (!hud1.Visible)
                    hud1.Visible = true;
                if (!hud1.Enabled)
                    hud1.Enabled = true;

                hud1.Dock = DockStyle.Fill;
            }

            if (Settings.Instance.ContainsKey("quickViewRows"))
            {
                setQuickViewRowsCols(Settings.Instance["quickViewCols"], Settings.Instance["quickViewRows"]);
            }

            for (int f = 1; f < 30; f++)
            {
                // load settings
                if (Settings.Instance["quickView" + f] != null)
                {
                    Control[] ctls = Controls.Find("quickView" + f, true);
                    if (ctls.Length > 0)
                    {
                        QuickView QV = (QuickView) ctls[0];

                        // set description and unit
                        string desc = Settings.Instance["quickView" + f];
                        QV.Tag = QV.desc;
                        QV.desc = MainV2.comPort.MAV.cs.GetNameandUnit(desc);

                        // set databinding for value
                        QV.DataBindings.Clear();
                        try
                        {
                            QV.DataBindings.Add(new Binding("number", bindingSourceQuickTab,
                                Settings.Instance["quickView" + f], false));
                        }
                        catch (Exception ex)
                        {
                            log.Debug(ex);
                        }
                    }
                }
                else
                {
                    // if no config, update description on predefined
                    try
                    {
                        Control[] ctls = Controls.Find("quickView" + f, true);
                        if (ctls.Length > 0)
                        {
                            QuickView QV = (QuickView) ctls[0];
                            string desc = QV.desc;
                            QV.Tag = desc;
                            QV.desc = MainV2.comPort.MAV.cs.GetNameandUnit(desc);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Debug(ex);
                    }
                }
            }

            CheckBatteryShow();

            // make sure the hud user items/warnings/checklist are using the current state
            HUD.Custom.src = MainV2.comPort.MAV.cs;
            CustomWarning.defaultsrc = MainV2.comPort.MAV.cs;
            MissionPlanner.Controls.PreFlight.CheckListItem.defaultsrc = MainV2.comPort.MAV.cs;

            if (Settings.Instance["maplast_lat"] != "")
            {
                try
                {
                    gMapControl1.Position = new PointLatLng(Settings.Instance.GetDouble("maplast_lat"),
                        Settings.Instance.GetDouble("maplast_lng"));
                    if (Math.Round(Settings.Instance.GetDouble("maplast_lat"), 1) == 0)
                    {
                        // no zoom in
                        Zoomlevel.Value = 3;
                        TRK_zoom.Value = 3;
                    }
                    else
                    {
                        var zoom = Settings.Instance.GetFloat("maplast_zoom");
                        if (Zoomlevel.Maximum < (decimal) zoom)
                            zoom = (float)Zoomlevel.Maximum;
                        Zoomlevel.Value = (decimal)zoom;
                        TRK_zoom.Value = (float) Zoomlevel.Value;
                    }
                }
                catch
                {
                }
            }

            hud1.doResize();
        }

        public void CheckBatteryShow()
        {
            // ensure battery display is on - also set in hud if current is updated
            if (MainV2.comPort.MAV.param.ContainsKey("BATT_MONITOR") &&
                (float) MainV2.comPort.MAV.param["BATT_MONITOR"] != 0)
            {
                hud1.batteryon = true;
            }
            else
            {
                hud1.batteryon = false;
            }
        }

        public void Deactivate()
        {
            if (MainV2.MONO)
            {
                hud1.Dock = DockStyle.None;
                hud1.Size = new Size(5, 5);
                hud1.Enabled = false;
                hud1.Visible = false;
            }
            //     hud1.Location = new Point(-1000,-1000);

            Settings.Instance["maplast_lat"] = gMapControl1.Position.Lat.ToString();
            Settings.Instance["maplast_lng"] = gMapControl1.Position.Lng.ToString();
            Settings.Instance["maplast_zoom"] = gMapControl1.Zoom.ToString();

            ZedGraphTimer.Stop();
        }

        void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Draw the background of the ListBox control for each item.
            //e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;

            LinearGradientBrush linear = new LinearGradientBrush(e.Bounds, Color.FromArgb(0x94, 0xc1, 0x1f),
                Color.FromArgb(0xcd, 0xe2, 0x96), LinearGradientMode.Vertical);

            e.Graphics.FillRectangle(linear, e.Bounds);

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            e.Graphics.DrawString(((TabControl) sender).TabPages[e.Index].Text,
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        void gMapControl1_OnMapZoomChanged()
        {
            try
            {
                // Exception System.Runtime.InteropServices.SEHException: External component has thrown an exception.
                TRK_zoom.Value = (float) gMapControl1.Zoom;
                Zoomlevel.Value = Convert.ToDecimal(gMapControl1.Zoom);
            }
            catch
            {
            }

            center.Position = gMapControl1.Position;
        }

        private void FlightData_Load(object sender, EventArgs e)
        {
            POI.POIModified += POI_POIModified;

            tfr.GotTFRs += tfr_GotTFRs;

            if (!Settings.Instance.ContainsKey("ShowNoFly") || Settings.Instance.GetBoolean("ShowNoFly"))
                NoFly.NoFly.NoFlyEvent += NoFly_NoFlyEvent;

            TRK_zoom.Minimum = gMapControl1.MapProvider.MinZoom;
            TRK_zoom.Maximum = 24;
            TRK_zoom.Value = (float) gMapControl1.Zoom;

            gMapControl1.EmptyTileColor = Color.Gray;

            Zoomlevel.Minimum = gMapControl1.MapProvider.MinZoom;
            Zoomlevel.Maximum = 24;
            Zoomlevel.Value = Convert.ToDecimal(gMapControl1.Zoom);

            var item1 = ParameterMetaDataRepository.GetParameterOptionsInt("MNT_MODE",
                MainV2.comPort.MAV.cs.firmware.ToString());
            var item2 = ParameterMetaDataRepository.GetParameterOptionsInt("MNT_DEFLT_MODE",
                MainV2.comPort.MAV.cs.firmware.ToString());
            if (item1.Count > 0)
                CMB_mountmode.DataSource = item1;

            if (item2.Count > 0)
                CMB_mountmode.DataSource = item2;

            CMB_mountmode.DisplayMember = "Value";
            CMB_mountmode.ValueMember = "Key";

            if (Settings.Instance["CHK_autopan"] != null)
                CHK_autopan.Checked = Settings.Instance.GetBoolean("CHK_autopan");

            if (Settings.Instance.ContainsKey("HudSwap") && Settings.Instance["HudSwap"] == "true")
                SwapHud1AndMap();

            if (Settings.Instance.ContainsKey("FlightSplitter"))
            {
                MainH.SplitterDistance = Settings.Instance.GetInt32("FlightSplitter");
            }

            if (Settings.Instance.ContainsKey("russian_hud"))
            {
                hud1.Russian = Settings.Instance.GetBoolean("russian_hud");
            }

            groundColorToolStripMenuItem.Checked = Settings.Instance.GetBoolean("groundColorToolStripMenuItem");
            groundColorToolStripMenuItem_Click(null, null);

            hud1.doResize();

            prop = new Propagation(gMapControl1);

            thisthread = new Thread(mainloop);
            thisthread.Name = "FD Mainloop";
            thisthread.IsBackground = true;
            thisthread.Start();
        }

        void tfr_GotTFRs(object sender, EventArgs e)
        {
            Invoke((Action) delegate
            {
                foreach (var item in tfr.tfrs)
                {
                    List<List<PointLatLng>> points = item.GetPaths();

                    foreach (var list in points)
                    {
                        GMapPolygon poly = new GMapPolygon(list, item.NAME);

                        poly.Fill = new SolidBrush(Color.FromArgb(30, Color.Blue));

                        tfrpolygons.Polygons.Add(poly);
                    }
                }
                tfrpolygons.IsVisibile = MainV2.ShowTFR;
            });
        }

        void POI_POIModified(object sender, EventArgs e)
        {
            POI.UpdateOverlay(poioverlay);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D1))
            {
                tabControlactions.SelectedIndex = 0;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D2))
            {
                tabControlactions.SelectedIndex = 1;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D3))
            {
                tabControlactions.SelectedIndex = 2;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D4))
            {
                tabControlactions.SelectedIndex = 3;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D5))
            {
                tabControlactions.SelectedIndex = 4;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D6))
            {
                tabControlactions.SelectedIndex = 5;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D7))
            {
                tabControlactions.SelectedIndex = 6;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D8))
            {
                tabControlactions.SelectedIndex = 7;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D9))
            {
                tabControlactions.SelectedIndex = 8;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D0))
            {
                tabControlactions.SelectedIndex = 9;
                return true;
            }

            if (keyData == (Keys.Space))
            {
                if (MainV2.comPort.logplaybackfile != null)
                {
                    BUT_playlog_Click(null, null);
                    return true;
                }
            }
            else if (keyData == (Keys.Subtract))
            {
                if (LogPlayBackSpeed > 1)
                    LogPlayBackSpeed--;
                else
                    LogPlayBackSpeed /= 2;

                updateLogPlayPosition();
            }
            else if (keyData == (Keys.Add))
            {
                if (LogPlayBackSpeed > 1)
                    LogPlayBackSpeed++;
                else
                    LogPlayBackSpeed *= 2;

                updateLogPlayPosition();
            }

            return false;
        }

        private void mainloop()
        {
            threadrun = true;
            EndPoint Remote = new IPEndPoint(IPAddress.Any, 0);

            DateTime tracklast = DateTime.Now.AddSeconds(0);

            DateTime tunning = DateTime.Now.AddSeconds(0);

            DateTime mapupdate = DateTime.Now.AddSeconds(0);

            DateTime vidrec = DateTime.Now.AddSeconds(0);

            DateTime waypoints = DateTime.Now.AddSeconds(0);

            DateTime updatescreen = DateTime.Now;

            DateTime tsreal = DateTime.Now;
            double taketime = 0;
            double timeerror = 0;

            while (!IsHandleCreated)
                Thread.Sleep(1000);

            while (threadrun)
            {
                if (MainV2.comPort.giveComport)
                {
                    Thread.Sleep(50);
                    updateBindingSource();
                    continue;
                }

                if (!MainV2.comPort.logreadmode)
                    Thread.Sleep(50); // max is only ever 10 hz but we go a little faster to empty the serial queue

                if (this.IsDisposed)
                {
                    threadrun = false;
                    break;
                }

                try
                {
                    if (aviwriter != null && vidrec.AddMilliseconds(1000/25.0) <= DateTime.Now)
                    {
                        vidrec = DateTime.Now;

                        hud1.streamjpgenable = true;

                        //aviwriter.avi_start("test.avi");
                        // add a frame
                        aviwriter.avi_add(hud1.streamjpg.ToArray(), (uint) hud1.streamjpg.Length);
                        // write header - so even partial files will play
                        aviwriter.avi_end(hud1.Width, hud1.Height, 25);
                    }
                }
                catch
                {
                    log.Error("Failed to write avi");
                }

                // log playback
                if (MainV2.comPort.logreadmode && MainV2.comPort.logplaybackfile != null)
                {
                    if (MainV2.comPort.BaseStream.IsOpen)
                    {
                        MainV2.comPort.logreadmode = false;
                        try
                        {
                            MainV2.comPort.logplaybackfile.Close();
                        }
                        catch
                        {
                            log.Error("Failed to close logfile");
                        }
                        MainV2.comPort.logplaybackfile = null;
                    }


                    //Console.WriteLine(DateTime.Now.Millisecond);

                    if (updatescreen.AddMilliseconds(300) < DateTime.Now)
                    {
                        try
                        {
                            updatePlayPauseButton(true);
                            updateLogPlayPosition();
                        }
                        catch
                        {
                            log.Error("Failed to update log playback pos");
                        }
                        updatescreen = DateTime.Now;
                    }

                    //Console.WriteLine(DateTime.Now.Millisecond + " done ");

                    DateTime logplayback = MainV2.comPort.lastlogread;
                    try
                    {
                        if (!MainV2.comPort.giveComport)
                            MainV2.comPort.readPacket();

                        // update currentstate of sysids on the port
                        foreach (var MAV in MainV2.comPort.MAVlist)
                        {
                            try
                            {
                                MAV.cs.UpdateCurrentSettings(null, false, MainV2.comPort, MAV);
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }
                    catch
                    {
                        log.Error("Failed to read log packet");
                    }

                    double act = (MainV2.comPort.lastlogread - logplayback).TotalMilliseconds;

                    if (act > 9999 || act < 0)
                        act = 0;

                    double ts = 0;
                    if (LogPlayBackSpeed == 0)
                        LogPlayBackSpeed = 0.01;
                    try
                    {
                        ts = Math.Min((act/LogPlayBackSpeed), 1000);
                    }
                    catch
                    {
                    }

                    if (LogPlayBackSpeed >= 4 && MainV2.speechEnable)
                        MainV2.speechEngine.SpeakAsyncCancelAll();

                    double timetook = (DateTime.Now - tsreal).TotalMilliseconds;
                    if (timetook != 0)
                    {
                        //Console.WriteLine("took: " + timetook + "=" + taketime + " " + (taketime - timetook) + " " + ts);
                        //Console.WriteLine(MainV2.comPort.lastlogread.Second + " " + DateTime.Now.Second + " " + (MainV2.comPort.lastlogread.Second - DateTime.Now.Second));
                        //if ((taketime - timetook) < 0)
                        {
                            timeerror += (taketime - timetook);
                            if (ts != 0)
                            {
                                ts += timeerror;
                                timeerror = 0;
                            }
                        }
                        if (Math.Abs(ts) > 1000)
                            ts = 1000;
                    }

                    taketime = ts;
                    tsreal = DateTime.Now;

                    if (ts > 0 && ts < 1000)
                        Thread.Sleep((int) ts);

                    tracklast = tracklast.AddMilliseconds(ts - act);
                    tunning = tunning.AddMilliseconds(ts - act);

                    if (tracklast.Month != DateTime.Now.Month)
                    {
                        tracklast = DateTime.Now;
                        tunning = DateTime.Now;
                    }

                    try
                    {
                        if (MainV2.comPort.logplaybackfile != null &&
                            MainV2.comPort.logplaybackfile.BaseStream.Position ==
                            MainV2.comPort.logplaybackfile.BaseStream.Length)
                        {
                            MainV2.comPort.logreadmode = false;
                        }
                    }
                    catch
                    {
                        MainV2.comPort.logreadmode = false;
                    }
                }
                else
                {
                    // ensure we know to stop
                    if (MainV2.comPort.logreadmode)
                        MainV2.comPort.logreadmode = false;
                    updatePlayPauseButton(false);

                    if (!playingLog && MainV2.comPort.logplaybackfile != null)
                    {
                        continue;
                    }
                }

                try
                {
                    CheckAndBindPreFlightData();
                    //Console.WriteLine(DateTime.Now.Millisecond);
                    //int fixme;
                    updateBindingSource();
                    // Console.WriteLine(DateTime.Now.Millisecond + " done ");

                    // battery warning.
                    float warnvolt = Settings.Instance.GetFloat("speechbatteryvolt");
                    float warnpercent = Settings.Instance.GetFloat("speechbatterypercent");

                    if (MainV2.comPort.MAV.cs.battery_voltage <= warnvolt)
                    {
                        hud1.lowvoltagealert = true;
                    }
                    else if ((MainV2.comPort.MAV.cs.battery_remaining) < warnpercent)
                    {
                        hud1.lowvoltagealert = true;
                    }
                    else
                    {
                        hud1.lowvoltagealert = false;
                    }

                    // update opengltest
                    if (OpenGLtest.instance != null)
                    {
                        OpenGLtest.instance.rpy = new OpenTK.Vector3(MainV2.comPort.MAV.cs.roll, MainV2.comPort.MAV.cs.pitch,
                            MainV2.comPort.MAV.cs.yaw);
                        OpenGLtest.instance.LocationCenter = new PointLatLngAlt(MainV2.comPort.MAV.cs.lat,
                            MainV2.comPort.MAV.cs.lng, MainV2.comPort.MAV.cs.altasl / CurrentState.multiplieralt, "here");
                    }

                    // update opengltest2
                    if (OpenGLtest2.instance != null)
                    {
                        OpenGLtest2.instance.rpy = new OpenTK.Vector3(MainV2.comPort.MAV.cs.roll, MainV2.comPort.MAV.cs.pitch,
                            MainV2.comPort.MAV.cs.yaw);
                        OpenGLtest2.instance.LocationCenter = new PointLatLngAlt(MainV2.comPort.MAV.cs.lat,
                            MainV2.comPort.MAV.cs.lng, MainV2.comPort.MAV.cs.altasl / CurrentState.multiplieralt, "here");
                        OpenGLtest2.instance.WPs = MainV2.comPort.MAV.wps.Values.Select(a => (Locationwp)a).ToList();
                    }

                    // update vario info
                    Vario.SetValue(MainV2.comPort.MAV.cs.climbrate);

                    // udpate tunning tab
                    if (tunning.AddMilliseconds(50) < DateTime.Now && CB_tuning.Checked)
                    {
                        double time = (Environment.TickCount - tickStart)/1000.0;
                        if (list1item != null)
                            list1.Add(time, ConvertToDouble(list1item.GetValue(MainV2.comPort.MAV.cs, null)));
                        if (list2item != null)
                            list2.Add(time, ConvertToDouble(list2item.GetValue(MainV2.comPort.MAV.cs, null)));
                        if (list3item != null)
                            list3.Add(time, ConvertToDouble(list3item.GetValue(MainV2.comPort.MAV.cs, null)));
                        if (list4item != null)
                            list4.Add(time, ConvertToDouble(list4item.GetValue(MainV2.comPort.MAV.cs, null)));
                        if (list5item != null)
                            list5.Add(time, ConvertToDouble(list5item.GetValue(MainV2.comPort.MAV.cs, null)));
                        if (list6item != null)
                            list6.Add(time, ConvertToDouble(list6item.GetValue(MainV2.comPort.MAV.cs, null)));
                        if (list7item != null)
                            list7.Add(time, ConvertToDouble(list7item.GetValue(MainV2.comPort.MAV.cs, null)));
                        if (list8item != null)
                            list8.Add(time, ConvertToDouble(list8item.GetValue(MainV2.comPort.MAV.cs, null)));
                        if (list9item != null)
                            list9.Add(time, ConvertToDouble(list9item.GetValue(MainV2.comPort.MAV.cs, null)));
                        if (list10item != null)
                            list10.Add(time, ConvertToDouble(list10item.GetValue(MainV2.comPort.MAV.cs, null)));
                    }

                    // update map
                    if (tracklast.AddSeconds(1.2) < DateTime.Now)
                    {
                        // show disable joystick button
                        if (MainV2.joystick != null && MainV2.joystick.enabled)
                        {
                            this.Invoke((MethodInvoker) delegate {
                                but_disablejoystick.Visible = true;
                            });
                        }

                        adsb.CurrentPosition = MainV2.comPort.MAV.cs.HomeLocation;

                        // show proximity screen
                        if (MainV2.comPort.MAV?.Proximity != null && MainV2.comPort.MAV.Proximity.DataAvailable)
                        {
                            //this.BeginInvoke((MethodInvoker)delegate { new ProximityControl(MainV2.comPort.MAV).Show(); });
                        }

                        if (Settings.Instance.GetBoolean("CHK_maprotation"))
                        {
                            // dont holdinvalidation here
                            setMapBearing();
                        }

                        if (route == null)
                        {
                            route = new GMapRoute(trackPoints, "track");
                            routes.Routes.Add(route);
                        }

                        PointLatLng currentloc = new PointLatLng(MainV2.comPort.MAV.cs.lat, MainV2.comPort.MAV.cs.lng);

                        gMapControl1.HoldInvalidation = true;

                        int numTrackLength = Settings.Instance.GetInt32("NUM_tracklength");
                        // maintain route history length
                        if (route.Points.Count > numTrackLength)
                        {
                            route.Points.RemoveRange(0,
                                route.Points.Count - numTrackLength);
                        }
                        // add new route point
                        if (MainV2.comPort.MAV.cs.lat != 0 && MainV2.comPort.MAV.cs.lng != 0)
                        {
                            route.Points.Add(currentloc);
                        }

                        if (!this.IsHandleCreated)
                            continue;

                        updateRoutePosition();

                        // update programed wp course
                        if (waypoints.AddSeconds(5) < DateTime.Now)
                        {
                            //Console.WriteLine("Doing FD WP's");
                            updateClearMissionRouteMarkers();
                            
                            var wps = MainV2.comPort.MAV.wps.Values.ToList();
                            if (wps.Count >= 1)
                            {
                                var homeplla = new PointLatLngAlt(MainV2.comPort.MAV.cs.HomeLocation.Lat,
                                    MainV2.comPort.MAV.cs.HomeLocation.Lng,
                                    MainV2.comPort.MAV.cs.HomeLocation.Alt / CurrentState.multiplieralt, "H");

                                var overlay = new WPOverlay();

                                {
                                    List<Locationwp> mission_items;
                                    mission_items = MainV2.comPort.MAV.wps.Values.Select(a => (Locationwp)a).ToList();
                                    mission_items.RemoveAt(0);

                                    if (wps.Count == 1)
                                    {
                                        overlay.CreateOverlay((MAVLink.MAV_FRAME)wps[0].frame, homeplla,
                                            mission_items,
                                            0 / CurrentState.multiplieralt, 0 / CurrentState.multiplieralt);
                                    }
                                    else
                                    {
                                        overlay.CreateOverlay((MAVLink.MAV_FRAME)wps[1].frame, homeplla,
                                            mission_items,
                                            0 / CurrentState.multiplieralt, 0 / CurrentState.multiplieralt);

                                    }
                                }

                                var existing = gMapControl1.Overlays.Where(a => a.Id == overlay.overlay.Id).ToList();
                                foreach (var b in existing)
                                {
                                    gMapControl1.Overlays.Remove(b);
                                }

                                gMapControl1.Overlays.Insert(1, overlay.overlay);

                                overlay.overlay.ForceUpdate();

                                distanceBar1.ClearWPDist();

                                var i = -1;
                                var travdist = 0.0;
                                var lastplla = overlay.pointlist.First();
                                foreach (var plla in overlay.pointlist)
                                {
                                    i++;
                                    if (plla == null)
                                        continue;

                                    var dist = lastplla.GetDistance(plla);

                                    distanceBar1.AddWPDist((float) dist);

                                    if (i <= MainV2.comPort.MAV.cs.wpno)
                                    {
                                        travdist += dist;
                                    }
                                }

                                travdist -= MainV2.comPort.MAV.cs.wp_dist;

                                if (MainV2.comPort.MAV.cs.mode.ToUpper() == "AUTO")
                                    distanceBar1.traveleddist = (float) travdist;
                            }

                            RegeneratePolygon();

                            // update rally points

                            rallypointoverlay.Markers.Clear();

                            foreach (var mark in MainV2.comPort.MAV.rallypoints.Values)
                            {
                                rallypointoverlay.Markers.Add(new GMapMarkerRallyPt(mark));
                            }

                            // optional on Flight data
                            if (MainV2.ShowAirports)
                            {
                                // airports
                                foreach (var item in Airports.getAirports(gMapControl1.Position).ToArray())
                                {
                                    try
                                    {
                                        rallypointoverlay.Markers.Add(new GMapMarkerAirport(item)
                                        {
                                            ToolTipText = item.Tag,
                                            ToolTipMode = MarkerTooltipMode.OnMouseOver
                                        });
                                    }
                                    catch (Exception e)
                                    {
                                        log.Error(e);
                                    }
                                }
                            }
                            waypoints = DateTime.Now;
                        }

                        updateClearRoutesMarkers();

                        // add this after the mav icons are drawn
                        if (MainV2.comPort.MAV.cs.MovingBase != null && MainV2.comPort.MAV.cs.MovingBase == PointLatLngAlt.Zero)
                        {
                            addMissionRouteMarker(new GMarkerGoogle(currentloc, GMarkerGoogleType.blue_dot)
                            {
                                Position = MainV2.comPort.MAV.cs.MovingBase,
                                ToolTipText = "Moving Base",
                                ToolTipMode = MarkerTooltipMode.OnMouseOver
                            });
                        }

                        // add gimbal point center
                        try
                        {
                            if (MainV2.comPort.MAV.param.ContainsKey("MNT_STAB_TILT") 
                                && MainV2.comPort.MAV.param.ContainsKey("MNT_STAB_ROLL")
                                && MainV2.comPort.MAV.param.ContainsKey("MNT_TYPE"))
                            {
                                float temp1 = (float)MainV2.comPort.MAV.param["MNT_STAB_TILT"];
                                float temp2 = (float)MainV2.comPort.MAV.param["MNT_STAB_ROLL"];

                                float temp3 = (float)MainV2.comPort.MAV.param["MNT_TYPE"];

                                if (MainV2.comPort.MAV.param.ContainsKey("MNT_STAB_PAN") &&
                                    // (float)MainV2.comPort.MAV.param["MNT_STAB_PAN"] == 1 &&
                                    ((float)MainV2.comPort.MAV.param["MNT_STAB_TILT"] == 1 &&
                                      (float)MainV2.comPort.MAV.param["MNT_STAB_ROLL"] == 0) ||
                                     (float)MainV2.comPort.MAV.param["MNT_TYPE"] == 4) // storm driver
                                {
                                    var marker = GimbalPoint.ProjectPoint();

                                    if (marker != PointLatLngAlt.Zero)
                                    {
                                        MainV2.comPort.MAV.cs.GimbalPoint = marker;

                                        addMissionRouteMarker(new GMarkerGoogle(marker, GMarkerGoogleType.blue_dot)
                                        {
                                            ToolTipText = "Camera Target\n" + marker,
                                            ToolTipMode = MarkerTooltipMode.OnMouseOver
                                        });
                                    }
                                }
                            }

                            
                            // cleanup old - no markers where added, so remove all old 
                            if (MainV2.comPort.MAV.camerapoints.Count < photosoverlay.Markers.Count)
                                photosoverlay.Markers.Clear();

                            var min_interval = 0.0;
                            if (MainV2.comPort.MAV.param.ContainsKey("CAM_MIN_INTERVAL"))
                                min_interval = MainV2.comPort.MAV.param["CAM_MIN_INTERVAL"].Value/1000.0;

                            // set fov's based on last grid calc
                            if (Settings.Instance["camera_fovh"] != null)
                            {
                                GMapMarkerPhoto.hfov = Settings.Instance.GetDouble("camera_fovh");
                                GMapMarkerPhoto.vfov = Settings.Instance.GetDouble("camera_fovv");
                            }

                            // add new - populate camera_feedback to map
                            double oldtime = double.MinValue;
                            foreach (var mark in MainV2.comPort.MAV.camerapoints.ToArray())
                            {
                                var timesincelastshot = (mark.time_usec/1000.0)/1000.0 - oldtime;
                                MainV2.comPort.MAV.cs.timesincelastshot = timesincelastshot;
                                bool contains = photosoverlay.Markers.Any(p => p.Tag.Equals(mark.time_usec));
                                if (!contains)
                                {
                                    if (timesincelastshot < min_interval)
                                        addMissionPhotoMarker(new GMapMarkerPhoto(mark, true));
                                    else
                                        addMissionPhotoMarker(new GMapMarkerPhoto(mark, false));
                                }
                                oldtime = (mark.time_usec/1000.0)/1000.0;
                            }
                            
                            // age current
                            int camcount = MainV2.comPort.MAV.camerapoints.Count;
                            int a = 0;
                            foreach (var mark in photosoverlay.Markers)
                            {
                                if (mark is GMapMarkerPhoto)
                                {
                                    if (CameraOverlap)
                                    {
                                        var marker = ((GMapMarkerPhoto) mark);
                                        // abandon roll higher than 25 degrees
                                        if (Math.Abs(marker.Roll) < 25)
                                        {
                                            MainV2.comPort.MAV.GMapMarkerOverlapCount.Add(
                                                ((GMapMarkerPhoto) mark).footprintpoly);
                                        }
                                    }
                                    if (a < (camcount-4))
                                        ((GMapMarkerPhoto)mark).drawfootprint = false;
                                }
                                a++;
                            }

                            if (CameraOverlap)
                            {
                                if (!kmlpolygons.Markers.Contains(MainV2.comPort.MAV.GMapMarkerOverlapCount) &&
                                    camcount > 0)
                                {
                                    kmlpolygons.Markers.Clear();
                                    kmlpolygons.Markers.Add(MainV2.comPort.MAV.GMapMarkerOverlapCount);
                                }
                            }
                            else if (kmlpolygons.Markers.Contains(MainV2.comPort.MAV.GMapMarkerOverlapCount))
                            {
                                kmlpolygons.Markers.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        lock (MainV2.instance.adsblock)
                        {
                            foreach (adsb.PointLatLngAltHdg plla in MainV2.instance.adsbPlanes.Values)
                            {
                                // 30 seconds history
                                if (((DateTime) plla.Time) > DateTime.Now.AddSeconds(-30))
                                {
                                    var adsbplane = new GMapMarkerADSBPlane(plla, plla.Heading)
                                    {
                                        ToolTipText = "ICAO: " + plla.Tag + "\n" +
                                        "Alt: " + plla.Alt.ToString("0") + "\n" +
                                        "Speed: " + plla.Speed.ToString("0") + "\n" +
                                        "Heading: " + plla.Heading.ToString("0")
                                        ,
                                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                                        Tag = plla
                                    };

                                    if (plla.DisplayICAO)
                                        adsbplane.ToolTipMode = MarkerTooltipMode.Always;

                                    switch (plla.ThreatLevel)
                                    {
                                        case MAVLink.MAV_COLLISION_THREAT_LEVEL.NONE:
                                            adsbplane.AlertLevel = GMapMarkerADSBPlane.AlertLevelOptions.Green;
                                            break;
                                        case MAVLink.MAV_COLLISION_THREAT_LEVEL.LOW:
                                            adsbplane.AlertLevel = GMapMarkerADSBPlane.AlertLevelOptions.Orange;
                                            break;
                                        case MAVLink.MAV_COLLISION_THREAT_LEVEL.HIGH:
                                            adsbplane.AlertLevel = GMapMarkerADSBPlane.AlertLevelOptions.Red;
                                            break;
                                    }

                                    addMissionRouteMarker(adsbplane);
                                }
                            }
                        }


                        if (route.Points.Count > 0)
                        {
                            // add primary route icon

                            // draw guide mode point for only main mav
                            if (MainV2.comPort.MAV.cs.mode.ToLower() == "guided" && MainV2.comPort.MAV.GuidedMode.x != 0)
                            {
                                addpolygonmarker("Guided Mode", MainV2.comPort.MAV.GuidedMode.y,
                                    MainV2.comPort.MAV.GuidedMode.x, (int)MainV2.comPort.MAV.GuidedMode.z, Color.Blue,
                                    routes);
                            }

                            // draw all icons for all connected mavs
                            foreach (var port in MainV2.Comports.ToArray())
                            {
                                // draw the mavs seen on this port
                                foreach (var MAV in port.MAVlist)
                                {
                                    var marker = ArduPilot.Common.getMAVMarker(MAV);

                                    if(marker.Position.Lat == 0 && marker.Position.Lng == 0)
                                        continue;

                                    addMissionRouteMarker(marker);
                                }
                            }

                            if (route.Points.Count == 0 || route.Points[route.Points.Count - 1].Lat != 0 &&
                                (mapupdate.AddSeconds(3) < DateTime.Now) && CHK_autopan.Checked)
                            {
                                updateMapPosition(currentloc);
                                mapupdate = DateTime.Now;
                            }

                            if (route.Points.Count == 1 && gMapControl1.Zoom == 3) // 3 is the default load zoom
                            {
                                updateMapPosition(currentloc);
                                updateMapZoom(17);
                            }
                        }

                        prop.Update(MainV2.comPort.MAV.cs.HomeLocation, MainV2.comPort.MAV.cs.Location,
                            MainV2.comPort.MAV.cs.battery_kmleft);

                        prop.alt = MainV2.comPort.MAV.cs.alt;
                        prop.altasl = MainV2.comPort.MAV.cs.altasl;
                        prop.center = gMapControl1.Position;

                        gMapControl1.HoldInvalidation = false;

                        if (gMapControl1.Visible)
                        {
                            gMapControl1.Invalidate();
                        }

                        tracklast = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    Tracking.AddException(ex);
                    Console.WriteLine("FD Main loop exception " + ex);
                }
            }
            Console.WriteLine("FD Main loop exit");
        }

        private double ConvertToDouble(object input)
        {
            if (input.GetType() == typeof (float))
            {
                return (float) input;
            }
            if (input.GetType() == typeof (double))
            {
                return (double) input;
            }
            if (input.GetType() == typeof(ulong))
            {
                return (ulong)input;
            }
            if (input.GetType() == typeof(long))
            {
                return (long)input;
            }
            if (input.GetType() == typeof (int))
            {
                return (int) input;
            }
            if (input.GetType() == typeof(uint))
            {
                return (uint)input;
            }
            if (input.GetType() == typeof(short))
            {
                return (short)input;
            }
            if (input.GetType() == typeof (ushort))
            {
                return (ushort) input;
            }
            if (input.GetType() == typeof (bool))
            {
                return (bool) input ? 1 : 0;
            }
            if (input.GetType() == typeof(string))
            {
                double ans = 0;
                if (double.TryParse((string)input, out ans))
                {
                    return ans;
                }
            }
            if (input is Enum)
            {
                return Convert.ToInt32(input);
            }

            if (input == null)
                throw new Exception("Bad Type Null");
            else 
                throw new Exception("Bad Type " + input.GetType().ToString());
        }

        private void updateClearRoutesMarkers()
        {
            Invoke((MethodInvoker) delegate
            {
                routes.Markers.Clear();
            });
        }

        private void setMapBearing()
        {
            Invoke((MethodInvoker) delegate { gMapControl1.Bearing = (int)((MainV2.comPort.MAV.cs.yaw + 360) % 360); });
        }

        // to prevent cross thread calls while in a draw and exception
        private void updateClearRoutes()
        {
            // not async
            Invoke((MethodInvoker) delegate
            {
                routes.Routes.Clear();
                routes.Routes.Add(route);
            });
        }

        // to prevent cross thread calls while in a draw and exception
        private void updateClearMissionRouteMarkers()
        {
            // not async
            Invoke((MethodInvoker) delegate
            {
                polygons.Routes.Clear();
                polygons.Markers.Clear();
                routes.Markers.Clear();
            });
        }

        private void updateRoutePosition()
        {
            // not async
            Invoke((MethodInvoker) delegate
            {
                gMapControl1.UpdateRouteLocalPosition(route);
            });
        }

        private void addMissionRouteMarker(GMapMarker marker)
        {
            // not async
            Invoke((MethodInvoker)delegate
            {
                routes.Markers.Add(marker);
            });
        }

        private void addMissionPhotoMarker(GMapMarker marker)
        {
            // not async
            Invoke((MethodInvoker)delegate
            {
                photosoverlay.Markers.Add(marker);
            });
        }

        private void updatePlayPauseButton(bool playing)
        {
            if (playing)
            {
                if (BUT_playlog.Text == "Pause")
                    return;

                BeginInvoke((MethodInvoker) delegate
                {
                    try
                    {
                        BUT_playlog.Text = "Pause";
                    }
                    catch
                    {
                    }
                });
            }
            else
            {
                if (BUT_playlog.Text == "Play")
                    return;

                BeginInvoke((MethodInvoker) delegate
                {
                    try
                    {
                        BUT_playlog.Text = "Play";
                    }
                    catch
                    {
                    }
                });
            }
        }

        DateTime lastscreenupdate = DateTime.Now;
        object updateBindingSourcelock = new object();
        volatile int updateBindingSourcecount;
        string updateBindingSourceThreadName = "";

        private void updateBindingSource()
        {
            //  run at 25 hz.
            if (lastscreenupdate.AddMilliseconds(40) < DateTime.Now)
            {
                lock (updateBindingSourcelock)
                {
                    // this is an attempt to prevent an invoke queue on the binding update on slow machines
                    if (updateBindingSourcecount > 0)
                    {
                        if (lastscreenupdate < DateTime.Now.AddSeconds(-5))
                        {
                            updateBindingSourcecount = 0;
                        }
                        return;
                    }

                    updateBindingSourcecount++;
                    updateBindingSourceThreadName = Thread.CurrentThread.Name;
                }

                this.BeginInvokeIfRequired((MethodInvoker)delegate
                {
                    updateBindingSourceWork();

                    lock (updateBindingSourcelock)
                    {
                        updateBindingSourcecount--;
                    }
                });
            }
        }

        private void updateBindingSourceWork()
        {
            try
            {
                if (this.Visible)
                {
                    //Console.Write("bindingSource1 ");
                    MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSource1.UpdateDataSource(MainV2.comPort.MAV.cs));
                    //Console.Write("bindingSourceHud ");
                    MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSourceHud.UpdateDataSource(MainV2.comPort.MAV.cs));
                    //Console.WriteLine("DONE ");

                    if (tabControlactions.SelectedTab == tabStatus)
                    {
                        MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSourceStatusTab.UpdateDataSource(MainV2.comPort.MAV.cs));
                    }
                    else if (tabControlactions.SelectedTab == tabQuick)
                    {
                        MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSourceQuickTab.UpdateDataSource(MainV2.comPort.MAV.cs));
                    }
                    else if (tabControlactions.SelectedTab == tabGauges)
                    {
                        MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSourceGaugesTab.UpdateDataSource(MainV2.comPort.MAV.cs));
                    }
                    else if (tabControlactions.SelectedTab == tabPagePreFlight)
                    {
                        MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSourceGaugesTab.UpdateDataSource(MainV2.comPort.MAV.cs));
                    }
                    else if (tabControlactions.SelectedTab == tabPayload)
                    {
                        MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSourcePayloadTab.UpdateDataSource(MainV2.comPort.MAV.cs));
                    }
                }
                else
                {
                    //Console.WriteLine("Null Binding");
                    MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSourceHud.UpdateDataSource(MainV2.comPort.MAV.cs));
                }
                lastscreenupdate = DateTime.Now;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Tracking.AddException(ex);
            }
        }

        /// <summary>
        /// Try to reduce the number of map position changes generated by the code
        /// </summary>
        DateTime lastmapposchange = DateTime.MinValue;

        private void updateMapPosition(PointLatLng currentloc)
        {
            Invoke((MethodInvoker) delegate
            {
                try
                {
                    if (lastmapposchange.Second != DateTime.Now.Second)
                    {
                        if (Math.Abs(currentloc.Lat - gMapControl1.Position.Lat) > 0.0001 || Math.Abs(currentloc.Lng - gMapControl1.Position.Lng) > 0.0001)
                        {
                            gMapControl1.Position = currentloc;
                        }
                        lastmapposchange = DateTime.Now;
                    }
                    //hud1.Refresh();
                }
                catch
                {
                }
            });
        }

        private void updateMapZoom(int zoom)
        {
            Invoke((MethodInvoker) delegate
            {
                try
                {
                    gMapControl1.Zoom = zoom;
                }
                catch
                {
                }
            });
        }

        private void updateLogPlayPosition()
        {
            BeginInvoke((MethodInvoker) delegate
            {
                try
                {
                    if (tracklog.Visible)
                        tracklog.Value =
                            (int)
                                (MainV2.comPort.logplaybackfile.BaseStream.Position/
                                 (double) MainV2.comPort.logplaybackfile.BaseStream.Length*100);
                    if (lbl_logpercent.Visible)
                        lbl_logpercent.Text =
                            (MainV2.comPort.logplaybackfile.BaseStream.Position/
                             (double) MainV2.comPort.logplaybackfile.BaseStream.Length).ToString("0.00%");

                    if (lbl_playbackspeed.Visible)
                        lbl_playbackspeed.Text = "x " + LogPlayBackSpeed;
                }
                catch
                {
                }
            });
        }

        private void addpolygonmarker(string tag, double lng, double lat, int alt, Color? color, GMapOverlay overlay)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMarkerGoogle m = new GMarkerGoogle(point, GMarkerGoogleType.green);
                m.ToolTipMode = MarkerTooltipMode.Always;
                m.ToolTipText = tag;
                m.Tag = tag;

                GMapMarkerRect mBorders = new GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                    try
                    {
                        mBorders.wprad =
                            (int) (Settings.Instance.GetFloat("TXT_WPRad")/CurrentState.multiplierdist);
                    }
                    catch
                    {
                    }
                    if (color.HasValue)
                    {
                        mBorders.Color = color.Value;
                    }
                }

                Invoke((MethodInvoker) delegate
                {
                    overlay.Markers.Add(m);
                    overlay.Markers.Add(mBorders);
                });
            }
            catch (Exception)
            {
            }
        }

        private void addpolygonmarkerred(string tag, double lng, double lat, int alt, Color? color, GMapOverlay overlay)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMarkerGoogle m = new GMarkerGoogle(point, GMarkerGoogleType.red);
                m.ToolTipMode = MarkerTooltipMode.Always;
                m.ToolTipText = tag;
                m.Tag = tag;

                GMapMarkerRect mBorders = new GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                }

                Invoke((MethodInvoker) delegate
                {
                    overlay.Markers.Add(m);
                    overlay.Markers.Add(mBorders);
                });
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// used to redraw the polygon
        /// </summary>
        void RegeneratePolygon()
        {
            List<PointLatLng> polygonPoints = new List<PointLatLng>();

            if (routes == null)
                return;

            foreach (GMapMarker m in polygons.Markers)
            {
                if (m is GMapMarkerRect)
                {
                    m.Tag = polygonPoints.Count;
                    polygonPoints.Add(m.Position);
                }
            }

            if (polygonPoints.Count < 2)
                return;

            GMapRoute homeroute = new GMapRoute("homepath");
            homeroute.Stroke = new Pen(Color.Yellow, 2);
            homeroute.Stroke.DashStyle = DashStyle.Dash;
            // add first point past home
            homeroute.Points.Add(polygonPoints[1]);
            // add home location
            homeroute.Points.Add(polygonPoints[0]);
            // add last point
            homeroute.Points.Add(polygonPoints[polygonPoints.Count - 1]);

            GMapRoute wppath = new GMapRoute("wp path");
            wppath.Stroke = new Pen(Color.Yellow, 4);
            wppath.Stroke.DashStyle = DashStyle.Custom;

            for (int a = 1; a < polygonPoints.Count; a++)
            {
                wppath.Points.Add(polygonPoints[a]);
            }

            Invoke((MethodInvoker) delegate
            {
                polygons.Routes.Add(homeroute);
                polygons.Routes.Add(wppath);
            });
        }

        GMapOverlay polygons;
        GMapOverlay routes;
        GMapRoute route;

        public void CreateChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Tuning";
            myPane.XAxis.Title.Text = "Time (s)";
            myPane.YAxis.Title.Text = "Unit";

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;

            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 5;

            // Make the Y axis scale red
            myPane.YAxis.Scale.FontSpec.FontColor = Color.White;
            myPane.YAxis.Title.FontSpec.FontColor = Color.White;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = true;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;
            // Manually set the axis range
            //myPane.YAxis.Scale.Min = -1;
            //myPane.YAxis.Scale.Max = 1;

            // Fill the axis background with a gradient
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0f);

            // Sample at 50ms intervals
            ZedGraphTimer.Interval = 200;
            //timer1.Enabled = true;
            //timer1.Start();


            // Calculate the Axis Scale Ranges
            //zgc.AxisChange();

            tickStart = Environment.TickCount;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // Make sure that the curvelist has at least one curve
                if (zg1.GraphPane.CurveList.Count <= 0)
                    return;

                // Get the first CurveItem in the graph
                LineItem curve = zg1.GraphPane.CurveList[0] as LineItem;
                if (curve == null)
                    return;

                // Get the PointPairList
                IPointListEdit list = curve.Points as IPointListEdit;
                // If this is null, it means the reference at curve.Points does not
                // support IPointListEdit, so we won't be able to modify it
                if (list == null)
                    return;

                // Time is measured in seconds
                double time = (Environment.TickCount - tickStart)/1000.0;

                // Keep the X scale at a rolling 30 second interval, with one
                // major step between the max X value and the end of the axis
                Scale xScale = zg1.GraphPane.XAxis.Scale;
                if (time > xScale.Max - xScale.MajorStep)
                {
                    xScale.Max = time + xScale.MajorStep;
                    xScale.Min = xScale.Max - 10.0;
                }

                // Make sure the Y axis is rescaled to accommodate actual data
                zg1.AxisChange();

                // Force a redraw

                zg1.Invalidate();
            }
            catch
            {
            }
        }

        private void BUT_clear_track_Click(object sender, EventArgs e)
        {
            if (route != null)
                route.Points.Clear();

            if (MainV2.comPort.MAV.camerapoints != null)
                MainV2.comPort.MAV.camerapoints.Clear();
        }

        private void BUTactiondo_Click(object sender, EventArgs e)
        {
            try
            {
                if (CMB_action.Text == "Trigger Camera NOW")
                {
                    MainV2.comPort.setDigicamControl(true);
                    return;
                }
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                return;
            }

            if (
                CustomMessageBox.Show("Are you sure you want to do " + CMB_action.Text + " ?", "Action",
                    MessageBoxButtons.YesNo) == (int)DialogResult.Yes)
            {
                try
                {
                    ((Button) sender).Enabled = false;

                    int param1 = 0;
                    int param3 = 1;

                    // request gyro
                    if (CMB_action.Text == "PREFLIGHT_CALIBRATION")
                    {
                        if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
                            param1 = 1; // gyro
                        param3 = 1; // baro / airspeed
                    }
                    if (CMB_action.Text == "PREFLIGHT_REBOOT_SHUTDOWN")
                    {
                        param1 = 1; // reboot
                    }

                    MainV2.comPort.doCommand((MAVLink.MAV_CMD) Enum.Parse(typeof (MAVLink.MAV_CMD), CMB_action.Text),
                        param1, 0, param3, 0, 0, 0, 0);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
                ((Button) sender).Enabled = true;
            }
        }

        private void BUTrestartmission_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button) sender).Enabled = false;

                MainV2.comPort.setWPCurrent(0); // set nav to
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
            ((Button) sender).Enabled = true;
        }

        private void FlightData_Resize(object sender, EventArgs e)
        {
            //Gspeed;
            //Galt;
            //Gheading;
            //attitudeIndicatorInstrumentControl1;
        }

        private void CB_tuning_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_tuning.Checked)
            {
                splitContainer1.Panel1Collapsed = false;
                ZedGraphTimer.Enabled = true;
                ZedGraphTimer.Start();
                zg1.Visible = true;
                zg1.Refresh();
            }
            else
            {
                splitContainer1.Panel1Collapsed = true;
                ZedGraphTimer.Enabled = false;
                ZedGraphTimer.Stop();
                zg1.Visible = false;
            }
        }

        private void BUT_RAWSensor_Click(object sender, EventArgs e)
        {
            Form temp = new RAW_Sensor();
            ThemeManager.ApplyThemeTo(temp);
            temp.Show();
        }

        private void gMapControl1_Click(object sender, EventArgs e)
        {
        }

        internal PointLatLng MouseDownStart;

        private void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownStart = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            if (ModifierKeys == Keys.Control)
            {
                goHereToolStripMenuItem_Click(null, null);
            }

            if (gMapControl1.IsMouseOverMarker)
            {
                if (CurrentGMapMarker is GMapMarkerADSBPlane)
                {
                    var marker = CurrentGMapMarker as GMapMarkerADSBPlane;
                    if (marker.Tag is adsb.PointLatLngAltHdg)
                    {
                        var plla = marker.Tag as adsb.PointLatLngAltHdg;
                        plla.DisplayICAO = !plla.DisplayICAO;
                    }
                }
            }
        }

        private void goHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show(Strings.PleaseConnect, Strings.ERROR);
                return;
            }

            if (MainV2.comPort.MAV.GuidedMode.z == 0)
            {
                flyToHereAltToolStripMenuItem_Click(null, null);

                if (MainV2.comPort.MAV.GuidedMode.z == 0)
                    return;
            }

            if (MouseDownStart.Lat == 0 || MouseDownStart.Lng == 0)
            {
                CustomMessageBox.Show(Strings.BadCoords, Strings.ERROR);
                return;
            }

            Locationwp gotohere = new Locationwp();

            gotohere.id = (ushort) MAVLink.MAV_CMD.WAYPOINT;
            gotohere.alt = MainV2.comPort.MAV.GuidedMode.z; // back to m
            gotohere.lat = (MouseDownStart.Lat);
            gotohere.lng = (MouseDownStart.Lng);

            try
            {
                MainV2.comPort.setGuidedModeWP(gotohere);
            }
            catch (Exception ex)
            {
                MainV2.comPort.giveComport = false;
                CustomMessageBox.Show(Strings.CommandFailed + ex.Message, Strings.ERROR);
            }
        }

        private void Zoomlevel_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (gMapControl1.MaxZoom + 1 == (double) Zoomlevel.Value)
                {
                    gMapControl1.Zoom = (double) Zoomlevel.Value - .1;
                }
                else
                {
                    gMapControl1.Zoom = (double) Zoomlevel.Value;
                }
            }
            catch
            {
            }
        }

        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                double latdif = MouseDownStart.Lat - point.Lat;
                double lngdif = MouseDownStart.Lng - point.Lng;

                gMapControl1.Position = new PointLatLng(center.Position.Lat + latdif,
                    center.Position.Lng + lngdif);
            }
            else
            {
                // setup a ballon with home distance
                if (marker != null)
                {
                    if (routes.Markers.Contains(marker))
                        routes.Markers.Remove(marker);
                }

                if (Settings.Instance.GetBoolean("CHK_disttohomeflightdata") != false)
                {
                    PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                    marker = new GMapMarkerRect(point);
                    marker.ToolTip = new GMapToolTip(marker);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = "Dist to Home: " +
                                         ((gMapControl1.MapProvider.Projection.GetDistance(point,
                                             MainV2.comPort.MAV.cs.HomeLocation.Point())*1000)*
                                          CurrentState.multiplierdist).ToString("0");

                    routes.Markers.Add(marker);
                }
            }
        }

        private void FlightData_ParentChanged(object sender, EventArgs e)
        {
            if (MainV2.cam != null)
            {
                MainV2.cam.camimage += cam_camimage;
            }

            // QUAD
            if (MainV2.comPort.MAV.param.ContainsKey("WP_SPEED_MAX"))
            {
                try
                {
                    modifyandSetSpeed.Value = (decimal) ((float) MainV2.comPort.MAV.param["WP_SPEED_MAX"]/100.0);
                }
                catch
                {
                    modifyandSetSpeed.Enabled = false;
                }
            }
            // plane 3.7 and below with airspeed, uses ARSPD_ENABLE:
            else if ((MainV2.comPort.MAV.param.ContainsKey("TRIM_ARSPD_CM") &&
                     MainV2.comPort.MAV.param.ContainsKey("ARSPD_ENABLE")
                     && MainV2.comPort.MAV.param.ContainsKey("ARSPD_USE") &&
                     (float) MainV2.comPort.MAV.param["ARSPD_ENABLE"] == 1
                     && (float) MainV2.comPort.MAV.param["ARSPD_USE"] == 1) ||
              // plane 3.8 and above with airspeed as per plane 3.7 to plane 3.8 migration wiki page, no longer uses ARSPD_ENABLE, uses ARSPD_TYPE instead:
                     (MainV2.comPort.MAV.param.ContainsKey("TRIM_ARSPD_CM") &&
                     MainV2.comPort.MAV.param.ContainsKey("ARSPD_TYPE")
                     && MainV2.comPort.MAV.param.ContainsKey("ARSPD_USE") &&
                     (float) MainV2.comPort.MAV.param["ARSPD_TYPE"] > 0
                     && (float) MainV2.comPort.MAV.param["ARSPD_USE"] == 1))
            {
                try
                {
                    modifyandSetSpeed.Value = (decimal) ((float) MainV2.comPort.MAV.param["TRIM_ARSPD_CM"]/100.0);
                }
                catch
                {
                    modifyandSetSpeed.Enabled = false;
                }
            } // plane without airspeed
            else if (MainV2.comPort.MAV.param.ContainsKey("TRIM_THROTTLE") &&
                     MainV2.comPort.MAV.param.ContainsKey("ARSPD_USE")
                     && (float) MainV2.comPort.MAV.param["ARSPD_USE"] == 0)
            {
                try
                {
                    modifyandSetSpeed.Value = (decimal) (float) MainV2.comPort.MAV.param["TRIM_THROTTLE"];
                }
                catch
                {
                    modifyandSetSpeed.Enabled = false;
                }
                // percent
                modifyandSetSpeed.ButtonText = Strings.ChangeThrottle;
            }

            try
            {
                if (MainV2.comPort.MAV.param.ContainsKey("LOITER_RAD"))
                    modifyandSetLoiterRad.Value =
                        (decimal) ((float) MainV2.comPort.MAV.param["LOITER_RAD"]*CurrentState.multiplierdist);
            }
            catch
            {
                modifyandSetLoiterRad.Enabled = false;
            }
            try
            {
                if (MainV2.comPort.MAV.param.ContainsKey("WP_LOITER_RAD"))
                {
                    modifyandSetLoiterRad.Value =
                        (decimal) ((float) MainV2.comPort.MAV.param["WP_LOITER_RAD"]*CurrentState.multiplierdist);
                }
            }
            catch
            {
                modifyandSetLoiterRad.Enabled = false;
            }
        }
        

        void cam_camimage(Image camimage)
        {
            hud1.bgimage = camimage;
        }

        private void BUT_Homealt_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.MAV.cs.altoffsethome != 0)
            {
                MainV2.comPort.MAV.cs.altoffsethome = 0;
            }
            else
            {
                MainV2.comPort.MAV.cs.altoffsethome = (float)(-MainV2.comPort.MAV.cs.HomeAlt/CurrentState.multiplieralt);
            }
        }

        private void gMapControl1_Resize(object sender, EventArgs e)
        {
            gMapControl1.Zoom = gMapControl1.Zoom + 0.01;
        }

        private void BUT_loadtelem_Click(object sender, EventArgs e)
        {
            LBL_logfn.Text = "";

            if (MainV2.comPort.logplaybackfile != null)
            {
                try
                {
                    MainV2.comPort.logplaybackfile.Close();
                    MainV2.comPort.logplaybackfile = null;
                }
                catch
                {
                }
            }

            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.AddExtension = true;
                fd.Filter = "Telemetry log (*.tlog)|*.tlog;*.tlog.*|Mavlink Log (*.mavlog)|*.mavlog";
                fd.InitialDirectory = Settings.Instance.LogDir;
                fd.DefaultExt = ".tlog";
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                LoadLogFile(file);
            }
        }

        public void LoadLogFile(string file)
        {
            if (file != "")
            {
                try
                {
                    BUT_clear_track_Click(null, null);

                    MainV2.comPort.logreadmode = true;
                    MainV2.comPort.logplaybackfile = new BinaryReader(File.OpenRead(file));
                    MainV2.comPort.lastlogread = DateTime.MinValue;

                    LBL_logfn.Text = Path.GetFileName(file);

                    log.Info("Open logfile " + file);

                    MainV2.comPort.getHeartBeat();

                    tracklog.Value = 0;
                    tracklog.Minimum = 0;
                    tracklog.Maximum = 100;
                }
                catch
                {
                    CustomMessageBox.Show(Strings.PleaseLoadValidFile, Strings.ERROR);
                }
            }
        }

        public void BUT_playlog_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.logreadmode)
            {
                MainV2.comPort.logreadmode = false;
                ZedGraphTimer.Stop();
                playingLog = false;
            }
            else
            {
                // BUT_clear_track_Click(sender, e);
                MainV2.comPort.logreadmode = true;
                list1.Clear();
                list2.Clear();
                list3.Clear();
                list4.Clear();
                list5.Clear();
                list6.Clear();
                list7.Clear();
                list8.Clear();
                list9.Clear();
                list10.Clear();
                tickStart = Environment.TickCount;

                zg1.GraphPane.XAxis.Scale.Min = 0;
                zg1.GraphPane.XAxis.Scale.Max = 1;
                ZedGraphTimer.Start();
                playingLog = true;
            }
        }

        private void tracklog_Scroll(object sender, EventArgs e)
        {
            try
            {
                BUT_clear_track_Click(sender, e);

                MainV2.comPort.lastlogread = DateTime.MinValue;
                MainV2.comPort.MAV.cs.ResetInternals();

                if (MainV2.comPort.logplaybackfile != null)
                    MainV2.comPort.logplaybackfile.BaseStream.Position =
                        (long) (MainV2.comPort.logplaybackfile.BaseStream.Length*(tracklog.Value/100.0));

                updateLogPlayPosition();
            }
            catch
            {
            } // ignore any invalid 
        }

        private void tabPage1_Resize(object sender, EventArgs e)
        {
            int mywidth, myheight;

            // localize it
            Control tabGauges = sender as Control;

            float scale = tabGauges.Width/(float) tabGauges.Height;

            if (scale > 0.5 && scale < 1.9)
            {
// square
                Gvspeed.Visible = true;

                if (tabGauges.Height < tabGauges.Width)
                    myheight = tabGauges.Height/2;
                else
                    myheight = tabGauges.Width/2;

                Gvspeed.Height = myheight;
                Gspeed.Height = myheight;
                Galt.Height = myheight;
                Gheading.Height = myheight;

                Gvspeed.Location = new Point(0, 0);
                Gspeed.Location = new Point(Gvspeed.Right, 0);


                Galt.Location = new Point(0, Gspeed.Bottom);
                Gheading.Location = new Point(Galt.Right, Gspeed.Bottom);

                return;
            }

            if (tabGauges.Width < 500)
            {
                Gvspeed.Visible = false;
                mywidth = tabGauges.Width/3;

                Gspeed.Height = mywidth;
                Galt.Height = mywidth;
                Gheading.Height = mywidth;

                Gspeed.Location = new Point(0, 0);
            }
            else
            {
                Gvspeed.Visible = true;
                mywidth = tabGauges.Width/4;

                Gvspeed.Height = mywidth;
                Gspeed.Height = mywidth;
                Galt.Height = mywidth;
                Gheading.Height = mywidth;

                Gvspeed.Location = new Point(0, 0);
                Gspeed.Location = new Point(Gvspeed.Right, 0);
            }

            Galt.Location = new Point(Gspeed.Right, 0);
            Gheading.Location = new Point(Galt.Right, 0);
        }

        private void BUT_setmode_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.MAV.cs.failsafe)
            {
                if (CustomMessageBox.Show("You are in failsafe, are you sure?", "Failsafe",MessageBoxButtons.YesNo) != (int)DialogResult.Yes)
                {
                    return;
                }
            }
            MainV2.comPort.setMode(CMB_modes.Text);
        }

        private void BUT_setwp_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button) sender).Enabled = false;
                MainV2.comPort.setWPCurrent((ushort) CMB_setwp.SelectedIndex); // set nav to
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
            ((Button) sender).Enabled = true;
        }

        private void CMB_setwp_Click(object sender, EventArgs e)
        {
            CMB_setwp.Items.Clear();

            CMB_setwp.Items.Add("0 (Home)");

            if (MainV2.comPort.MAV.param["CMD_TOTAL"] != null)
            {
                int wps = int.Parse(MainV2.comPort.MAV.param["CMD_TOTAL"].ToString());
                for (int z = 1; z <= wps; z++)
                {
                    CMB_setwp.Items.Add(z.ToString());
                }
                return;
            }

            if (MainV2.comPort.MAV.param["WP_TOTAL"] != null)
            {
                int wps = int.Parse(MainV2.comPort.MAV.param["WP_TOTAL"].ToString());
                for (int z = 1; z <= wps; z++)
                {
                    CMB_setwp.Items.Add(z.ToString());
                }
                return;
            }

            if (MainV2.comPort.MAV.param["MIS_TOTAL"] != null)
            {
                int wps = int.Parse(MainV2.comPort.MAV.param["MIS_TOTAL"].ToString());
                for (int z = 1; z <= wps; z++)
                {
                    CMB_setwp.Items.Add(z.ToString());
                }
                return;
            }

            if (MainV2.comPort.MAV.wps.Count > 0)
            {
                int wps = MainV2.comPort.MAV.wps.Count;
                for (int z = 1; z <= wps; z++)
                {
                    CMB_setwp.Items.Add(z.ToString());
                }
                return;
            }
        }

        private void BUT_quickauto_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button) sender).Enabled = false;
                MainV2.comPort.setMode("Auto");
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
            ((Button) sender).Enabled = true;
        }

        private void BUT_quickrtl_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button) sender).Enabled = false;
                MainV2.comPort.setMode("RTL");
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
            ((Button) sender).Enabled = true;
        }

        private void BUT_quickmanual_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button) sender).Enabled = false;
                if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane ||
                    MainV2.comPort.MAV.cs.firmware == Firmwares.Ateryx ||
                    MainV2.comPort.MAV.cs.firmware == Firmwares.ArduRover)
                    MainV2.comPort.setMode("Loiter");
                if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
                    MainV2.comPort.setMode("Loiter");
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
            ((Button) sender).Enabled = true;
        }

        private void BUT_log2kml_Click(object sender, EventArgs e)
        {
            Form frm = new MavlinkLog();
            ThemeManager.ApplyThemeTo(frm);
            frm.Show();
        }

        private void BUT_joystick_Click(object sender, EventArgs e)
        {
            Form joy = new JoystickSetup();
            ThemeManager.ApplyThemeTo(joy);
            joy.Show();
        }

        private void CMB_modes_Click(object sender, EventArgs e)
        {
            CMB_modes.DataSource = Common.getModesList(MainV2.comPort.MAV.cs.firmware);
            CMB_modes.ValueMember = "Key";
            CMB_modes.DisplayMember = "Value";
        }

        private void hud1_DoubleClick(object sender, EventArgs e)
        {
            if (huddropout)
                return;

            SubMainLeft.Panel1Collapsed = true;
            Form dropout = new Form();
            dropout.Size = new Size(hud1.Width, hud1.Height + 20);
            SubMainLeft.Panel1.Controls.Remove(hud1);
            dropout.Controls.Add(hud1);
            dropout.Resize += dropout_Resize;
            dropout.FormClosed += dropout_FormClosed;
            dropout.Show();
            huddropout = true;
        }

        void dropout_FormClosed(object sender, FormClosedEventArgs e)
        {
            //GetFormFromGuid(GetOrCreateGuid("fd_hud_guid")).Controls.Add(hud1);
            SubMainLeft.Panel1.Controls.Add(hud1);
            SubMainLeft.Panel1Collapsed = false;
            huddropout = false;
        }

        void dropout_Resize(object sender, EventArgs e)
        {
            if (huddropoutresize)
                return;

            huddropoutresize = true;

            int hudh = hud1.Height;
            int formh = ((Form) sender).Height - 30;

            if (((Form) sender).Height < hudh)
            {
                if (((Form) sender).WindowState == FormWindowState.Maximized)
                {
                    Point tl = ((Form) sender).DesktopLocation;
                    ((Form) sender).WindowState = FormWindowState.Normal;
                    ((Form) sender).Location = tl;
                }
                ((Form) sender).Width = (int) (formh*(hud1.SixteenXNine ? 1.777f : 1.333f));
                ((Form) sender).Height = formh + 20;
            }
            hud1.Refresh();
            huddropoutresize = false;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Messagetabtimer.Stop();

            if (tabControlactions.SelectedTab == tabStatus)
            {
                tabControlactions.Visible = false;
                tabStatus.Visible = false;
                tabStatus_Resize(sender, e);
                tabStatus.Visible = true;
                tabControlactions.Visible = true;
            }
            else if (tabControlactions.SelectedTab == tabPagemessages)
            {
                Messagetabtimer.Start();
            }
            else
            {
                // foreach (Control temp in tabStatus.Controls)
                // {
                //   temp.DataBindings.Clear();
                //  temp.Dispose();
                //  tabStatus.Controls.Remove(temp);
                // }

                if (tabControlactions.SelectedTab == tabQuick)
                {
                }
            }
        }

        private void CheckAndBindPreFlightData()
        {
            //this.Invoke((Action) delegate { preFlightChecklist1.BindData(); });
        }

        private void Gspeed_DoubleClick(object sender, EventArgs e)
        {
            string max = "60";
            if (DialogResult.OK == InputBox.Show("Enter Max", "Enter Max Speed", ref max))
            {
                Gspeed.MaxValue = float.Parse(max);
                Settings.Instance["GspeedMAX"] = Gspeed.MaxValue.ToString();
            }
        }

        private void recordHudToAVIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopRecordToolStripMenuItem_Click(sender, e);

            CustomMessageBox.Show("Output avi will be saved to the log folder");

            aviwriter = new AviWriter();
            Directory.CreateDirectory(Settings.Instance.LogDir);
            aviwriter.avi_start(Settings.Instance.LogDir + Path.DirectorySeparatorChar +
                                DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".avi");

            recordHudToAVIToolStripMenuItem.Text = "Recording";
        }

        private void stopRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recordHudToAVIToolStripMenuItem.Text = "Start Recording";

            try
            {
                if (aviwriter != null)
                    aviwriter.avi_close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ERROR + " " + ex, Strings.ERROR);
            }

            aviwriter = null;
        }

        bool setupPropertyInfo(ref PropertyInfo input, string name, object source)
        {
            Type test = source.GetType();

            foreach (var field in test.GetProperties())
            {
                if (field.Name == name)
                {
                    input = field;
                    return true;
                }
            }

            return false;
        }

        private void zg1_DoubleClick(object sender, EventArgs e)
        {

            var selectform = new Form
            {
                Name = "select",
                Width = 50,
                Height = 50,
                Text = "Display This",
                AutoSize = true,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                AutoScroll = true
            };

            int x = 5;
            int y = 2;

            {
                CheckBox chk_box = new CheckBox();
                chk_box.Text = "Logarithmic";
                chk_box.Name = "Logarithmic";
                chk_box.Location = new Point(x, y);
                chk_box.Size = new Size(100, 20);
                chk_box.CheckedChanged += chk_log_CheckedChanged;

                selectform.Controls.Add(chk_box);
            }

            ThemeManager.ApplyThemeTo(selectform);

            y += 20;

            object thisBoxed = MainV2.comPort.MAV.cs;
            Type test = thisBoxed.GetType();

            int max_length = 0;
            List<string> fields = new List<string>();

            foreach (var field in test.GetProperties())
            {
                // field.Name has the field's name.
                object fieldValue = field.GetValue(thisBoxed, null); // Get value
                if (fieldValue == null)
                    continue;

                if (!fieldValue.IsNumber())
                    continue;

                max_length = Math.Max(max_length, TextRenderer.MeasureText(field.Name, selectform.Font).Width);
                fields.Add(field.Name);
            }
            max_length += 25;
            fields.Sort();

            int col_count = (int)(Screen.FromControl(this).Bounds.Width * 0.8f) / max_length;
            int row_count = fields.Count / col_count + ((fields.Count % col_count == 0) ? 0 : 1);
            int row_height = 20;

            int i = 1;
            foreach (var field in fields)
            {
                CheckBox chk_box = new CheckBox();

                ThemeManager.ApplyThemeTo(chk_box);

                if (list1item != null && list1item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }
                if (list2item != null && list2item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }
                if (list3item != null && list3item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }
                if (list4item != null && list4item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }
                if (list5item != null && list5item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }
                if (list6item != null && list6item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }
                if (list7item != null && list7item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }
                if (list8item != null && list8item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }
                if (list9item != null && list9item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }
                if (list10item != null && list10item.Name == field)
                {
                    chk_box.Checked = true;
                    chk_box.BackColor = Color.Green;
                }

                chk_box.Text = field;
                chk_box.Name = field;
                chk_box.Tag = "custom";
                chk_box.Location = new Point(5 + (i / row_count) * (max_length + 5), 2 + (i % row_count) * row_height);
                chk_box.Size = new Size(120, 20);
                chk_box.CheckedChanged += chk_box_CheckedChanged;
                chk_box.AutoSize = true;

                selectform.Controls.Add(chk_box);

                x += 0;
                y += 20;
                i++;

                if (y > selectform.Height - 50)
                {
                    x += 120;
                    y = 10;

                    selectform.Width = x + 120;
                }
            }

            selectform.Shown += (o, args) => {
                selectform.Controls.ForEach(a =>
                {
                    if (a is CheckBox && ((CheckBox)a).Checked)
                        ((CheckBox)a).BackColor = Color.Green;
                });
            };

            selectform.Show();
        }

        private void hud_UserItem(object sender, EventArgs e)
        {
            Form selectform = new Form
            {
                Name = "select",
                Width = 50,
                Height = 50,
                Text = "Display This",
                AutoSize = true,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                AutoScroll = true

            };
            ThemeManager.ApplyThemeTo(selectform);

            object thisBoxed = MainV2.comPort.MAV.cs;
            Type test = thisBoxed.GetType();

            int max_length = 0;
            List<string> fields = new List<string>();

            foreach (var field in test.GetProperties())
            {
                // field.Name has the field's name.
                object fieldValue = field.GetValue(thisBoxed, null); // Get value
                if (fieldValue == null)
                    continue;

                // Get the TypeCode enumeration. Multiple types get mapped to a common typecode.
                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());

                if (!fieldValue.IsNumber())
                    continue;

                max_length = Math.Max(max_length, TextRenderer.MeasureText(field.Name, selectform.Font).Width);
                fields.Add(field.Name);
            }
            max_length += 15;
            fields.Sort();

            int col_count = (int) (Screen.FromControl(this).Bounds.Width*0.8f)/max_length;
            int row_count = fields.Count/col_count + ((fields.Count%col_count == 0) ? 0 : 1);
            int row_height = 20;
            //selectform.MinimumSize = new Size(col_count * max_length, row_count * row_height);

            for (int i = 0; i < fields.Count; i++)
            {
                CheckBox chk_box = new CheckBox
                {
                    Text = fields[i],
                    Name = fields[i],
                    Tag = "custom",
                    Location = new Point(5 + (i/row_count)*(max_length + 5), 2 + (i%row_count)*row_height),
                    Size = new Size(max_length, row_height),
                    Checked = hud1.CustomItems.ContainsKey(fields[i]),
                    AutoSize = true
                };
                chk_box.CheckedChanged += chk_box_hud_UserItem_CheckedChanged;
                if (chk_box.Checked)
                    chk_box.BackColor = Color.Green;
                selectform.Controls.Add(chk_box);
            }

            selectform.Shown += (o, args) => {
                selectform.Controls.ForEach(a =>
                {
                    if (a is CheckBox && ((CheckBox)a).Checked)
                        ((CheckBox)a).BackColor = Color.Green;
                });
            };

            selectform.ShowDialog(this);
        }

        void addHudUserItem(ref HUD.Custom cust, CheckBox sender)
        {
            setupPropertyInfo(ref cust.Item, (sender).Name, MainV2.comPort.MAV.cs);

            hud1.CustomItems[(sender).Name] = cust;

            hud1.Invalidate();
        }

        void chk_box_hud_UserItem_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox) sender;

            if (checkbox.Checked)
            {
                checkbox.BackColor = Color.Green;

                HUD.Custom cust = new HUD.Custom();
                HUD.Custom.src = MainV2.comPort.MAV.cs;

                string prefix = checkbox.Name + ": ";
                if (Settings.Instance["hud1_useritem_" + checkbox.Name] != null)
                    prefix = Settings.Instance["hud1_useritem_" + checkbox.Name];

                if (DialogResult.Cancel == InputBox.Show("Header", "Please enter your item prefix", ref prefix))
                {
                    checkbox.Checked = false;
                    return;
                }

                Settings.Instance["hud1_useritem_" + checkbox.Name] = prefix;

                cust.Header = prefix;

                addHudUserItem(ref cust, checkbox);
            }
            else
            {
                checkbox.BackColor = Color.Transparent;

                if (hud1.CustomItems.ContainsKey(checkbox.Name))
                    hud1.CustomItems.Remove(checkbox.Name);

                Settings.Instance.Remove("hud1_useritem_" + checkbox.Name);
                hud1.Invalidate();
            }
        }

        void chk_log_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox) sender).Checked)
            {
                zg1.GraphPane.YAxis.Type = AxisType.Log;
            }
            else
            {
                zg1.GraphPane.YAxis.Type = AxisType.Linear;
            }
        }

        void chk_box_CheckedChanged(object sender, EventArgs e)
        {
            ThemeManager.ApplyThemeTo((Control)sender);

            if (((CheckBox) sender).Checked)
            {
                ((CheckBox) sender).BackColor = Color.Green;

                if (list1item == null)
                {
                    if (setupPropertyInfo(ref list1item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list1.Clear();
                        list1curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list1, Color.Red, SymbolType.None);
                    }
                }
                else if (list2item == null)
                {
                    if (setupPropertyInfo(ref list2item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list2.Clear();
                        list2curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list2, Color.Blue, SymbolType.None);
                    }
                }
                else if (list3item == null)
                {
                    if (setupPropertyInfo(ref list3item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list3.Clear();
                        list3curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list3, Color.Green,
                            SymbolType.None);
                    }
                }
                else if (list4item == null)
                {
                    if (setupPropertyInfo(ref list4item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list4.Clear();
                        list4curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list4, Color.Orange,
                            SymbolType.None);
                    }
                }
                else if (list5item == null)
                {
                    if (setupPropertyInfo(ref list5item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list5.Clear();
                        list5curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list5, Color.Yellow,
                            SymbolType.None);
                    }
                }
                else if (list6item == null)
                {
                    if (setupPropertyInfo(ref list6item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list6.Clear();
                        list6curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list6, Color.Magenta,
                            SymbolType.None);
                    }
                }
                else if (list7item == null)
                {
                    if (setupPropertyInfo(ref list7item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list7.Clear();
                        list7curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list7, Color.Purple,
                            SymbolType.None);
                    }
                }
                else if (list8item == null)
                {
                    if (setupPropertyInfo(ref list8item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list8.Clear();
                        list8curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list8, Color.LimeGreen,
                            SymbolType.None);
                    }
                }
                else if (list9item == null)
                {
                    if (setupPropertyInfo(ref list9item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list9.Clear();
                        list9curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list9, Color.Cyan, SymbolType.None);
                    }
                }
                else if (list10item == null)
                {
                    if (setupPropertyInfo(ref list10item, ((CheckBox) sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list10.Clear();
                        list10curve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name, list10, Color.Violet,
                            SymbolType.None);
                    }
                }
                else
                {
                    CustomMessageBox.Show("Max 10 at a time.");
                    ((CheckBox) sender).Checked = false;
                }
            
                string selected = "";
                try
                {
                    foreach (var curve in zg1.GraphPane.CurveList)
                    {
                        selected = selected + curve.Label.Text + "|";
                    }
                }
                catch
                {
                }
                Settings.Instance["Tuning_Graph_Selected"] = selected;
            }
            else
            {
                ((CheckBox) sender).BackColor = Color.Transparent;

                // reset old stuff
                if (list1item != null && list1item.Name == ((CheckBox) sender).Name)
                {
                    list1item = null;
                    zg1.GraphPane.CurveList.Remove(list1curve);
                }
                if (list2item != null && list2item.Name == ((CheckBox) sender).Name)
                {
                    list2item = null;
                    zg1.GraphPane.CurveList.Remove(list2curve);
                }
                if (list3item != null && list3item.Name == ((CheckBox) sender).Name)
                {
                    list3item = null;
                    zg1.GraphPane.CurveList.Remove(list3curve);
                }
                if (list4item != null && list4item.Name == ((CheckBox) sender).Name)
                {
                    list4item = null;
                    zg1.GraphPane.CurveList.Remove(list4curve);
                }
                if (list5item != null && list5item.Name == ((CheckBox) sender).Name)
                {
                    list5item = null;
                    zg1.GraphPane.CurveList.Remove(list5curve);
                }
                if (list6item != null && list6item.Name == ((CheckBox) sender).Name)
                {
                    list6item = null;
                    zg1.GraphPane.CurveList.Remove(list6curve);
                }
                if (list7item != null && list7item.Name == ((CheckBox) sender).Name)
                {
                    list7item = null;
                    zg1.GraphPane.CurveList.Remove(list7curve);
                }
                if (list8item != null && list8item.Name == ((CheckBox) sender).Name)
                {
                    list8item = null;
                    zg1.GraphPane.CurveList.Remove(list8curve);
                }
                if (list9item != null && list9item.Name == ((CheckBox) sender).Name)
                {
                    list9item = null;
                    zg1.GraphPane.CurveList.Remove(list9curve);
                }
                if (list10item != null && list10item.Name == ((CheckBox) sender).Name)
                {
                    list10item = null;
                    zg1.GraphPane.CurveList.Remove(list10curve);
                }
            }
        }

        private void pointCameraHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show("Please Connect First");
                return;
            }

            double srtmalt = srtm.getAltitude(MouseDownStart.Lat, MouseDownStart.Lng).alt;

            string alt = (srtmalt *CurrentState.multiplieralt).ToString("0");
            if (DialogResult.Cancel == InputBox.Show("Enter Alt", "Enter Target Alt (absolute, default value is ground alt)", ref alt))
                return;

            float intalt = 0;
            if (!float.TryParse(alt, out intalt))
            {
                CustomMessageBox.Show("Bad Alt");
                return;
            }

            if (MouseDownStart.Lat == 0 || MouseDownStart.Lng == 0)
            {
                CustomMessageBox.Show("Bad Lat/Long");
                return;
            }

            try
            {
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0, (float) MouseDownStart.Lat,
                    (float) MouseDownStart.Lng, intalt/CurrentState.multiplieralt);
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
        }

        private void CHK_autopan_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["CHK_autopan"] = CHK_autopan.Checked.ToString();

            //GCSViews.FlightPlanner.instance.autopan = CHK_autopan.Checked;
        }

        private void setMJPEGSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = Settings.Instance["mjpeg_url"] != null
                ? Settings.Instance["mjpeg_url"]
                : @"http://127.0.0.1:56781/map.jpg";

            if (DialogResult.OK == InputBox.Show("Mjpeg url", "Enter the url to the mjpeg source url", ref url))
            {
                Settings.Instance["mjpeg_url"] = url;

                CaptureMJPEG.Stop();

                CaptureMJPEG.URL = url;

                CaptureMJPEG.OnNewImage += CaptureMJPEG_OnNewImage;

                CaptureMJPEG.runAsync();
            }
            else
            {
                CaptureMJPEG.Stop();
            }
        }

        public void CaptureMJPEG_OnNewImage(object sender, EventArgs e)
        {
            myhud.bgimage = (Image) sender;
        }

        private void setAspectRatioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hud1.SixteenXNine = !hud1.SixteenXNine;
            hud1.doResize();
        }

        private void quickView_DoubleClick(object sender, EventArgs e)
        {
            QuickView qv = (QuickView) sender;

            Form selectform = new Form
            {
                Name = "select",
                Width = 50,
                Height = 50,
                Text = "Display This",
                AutoSize = true,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                AutoScroll = true

            };
            ThemeManager.ApplyThemeTo(selectform);

            object thisBoxed = MainV2.comPort.MAV.cs;
            Type test = thisBoxed.GetType();

            int max_length = 0;
            List<string> fields = new List<string>();

            foreach (var field in test.GetProperties())
            {
                // field.Name has the field's name.
                object fieldValue = field.GetValue(thisBoxed, null); // Get value
                if (fieldValue == null)
                    continue;

                if (!fieldValue.IsNumber())
                    continue;

                max_length = Math.Max(max_length, TextRenderer.MeasureText(field.Name, selectform.Font).Width);
                fields.Add(field.Name);
            }
            max_length += 25;
            fields.Sort();

            int col_count = (int) (Screen.FromControl(this).Bounds.Width*0.8f)/max_length;
            int row_count = fields.Count/col_count + ((fields.Count%col_count == 0) ? 0 : 1);
            int row_height = 20;
            //selectform.MinimumSize = new Size(col_count * max_length, row_count * row_height);

            for (int i = 0; i < fields.Count; i++)
            {
                CheckBox chk_box = new CheckBox
                {
                    // dont change to ToString() = null exception
                    Checked = qv.Tag != null && qv.Tag.ToString() == fields[i],
                    Text = fields[i],
                    Name = fields[i],
                    Tag = qv,
                    Location = new Point(5 + (i/row_count)*(max_length + 5 ), 2 + (i%row_count)*row_height),
                    Size = new Size(max_length, row_height),
                    AutoSize = true
                };
                chk_box.CheckedChanged += chk_box_quickview_CheckedChanged;
                if (chk_box.Checked)
                    chk_box.BackColor = Color.Green;
                selectform.Controls.Add(chk_box);
            }

            selectform.Shown += (o, args) => { selectform.Controls.ForEach(a =>
            {
                if (a is CheckBox && ((CheckBox) a).Checked)
                    ((CheckBox) a).BackColor = Color.Green;
            }); };

            selectform.ShowDialog(this);
        }

        void chk_box_quickview_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox) sender;

            if (checkbox.Checked)
            {
                // save settings
                Settings.Instance[((QuickView) checkbox.Tag).Name] = checkbox.Name;

                // set description
                string desc = checkbox.Name;
                ((QuickView) checkbox.Tag).Tag = desc;

                desc = MainV2.comPort.MAV.cs.GetNameandUnit(desc);

                ((QuickView) checkbox.Tag).desc = desc;

                // set databinding for value
                ((QuickView) checkbox.Tag).DataBindings.Clear();
                ((QuickView) checkbox.Tag).DataBindings.Add(new Binding("number", bindingSourceQuickTab, checkbox.Name,
                    true));

                // close selection form
                ((Form) checkbox.Parent).Close();
            }
        }

        private void flyToHereAltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string alt = "100";

            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
            {
                alt = (10*CurrentState.multiplieralt).ToString("0");
            }
            else
            {
                alt = (100*CurrentState.multiplieralt).ToString("0");
            }

            if (Settings.Instance.ContainsKey("guided_alt"))
                alt = Settings.Instance["guided_alt"];

            if (DialogResult.Cancel == InputBox.Show("Enter Alt", "Enter Guided Mode Alt", ref alt))
                return;

            Settings.Instance["guided_alt"] = alt;

            int intalt = (int) (100*CurrentState.multiplieralt);
            if (!int.TryParse(alt, out intalt))
            {
                CustomMessageBox.Show("Bad Alt");
                return;
            }

            MainV2.comPort.MAV.GuidedMode.z = intalt/CurrentState.multiplieralt;

            if (MainV2.comPort.MAV.cs.mode == "Guided")
            {
                MainV2.comPort.setGuidedModeWP(new Locationwp
                {
                    alt = MainV2.comPort.MAV.GuidedMode.z,
                    lat = MainV2.comPort.MAV.GuidedMode.x,
                    lng = MainV2.comPort.MAV.GuidedMode.y
                });
            }
        }

        private void flightPlannerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in splitContainer1.Panel2.Controls)
            {
                ctl.Visible = false;
            }

            foreach (MainSwitcher.Screen sc in MainV2.View.screens)
            {
                if (sc.Name == "FlightPlanner")
                {
                    MyButton but = new MyButton
                    {
                        Location = new Point(splitContainer1.Panel2.Width/2, 0),
                        Text = "Close"
                    };
                    but.Click += but_Click;

                    splitContainer1.Panel2.Controls.Add(but);
                    splitContainer1.Panel2.Controls.Add(sc.Control);
                    ThemeManager.ApplyThemeTo(sc.Control);
                    ThemeManager.ApplyThemeTo(this);

                    sc.Control.Dock = DockStyle.Fill;
                    sc.Control.Visible = true;

                    if (sc.Control is IActivate)
                    {
                        ((IActivate) (sc.Control)).Activate();
                    }

                    but.BringToFront();
                    break;
                }
            }
        }

        void but_Click(object sender, EventArgs e)
        {
            foreach (MainSwitcher.Screen sc in MainV2.View.screens)
            {
                if (sc.Name == "FlightPlanner")
                {
                    splitContainer1.Panel2.Controls.Remove(sc.Control);
                    splitContainer1.Panel2.Controls.Remove((Control) sender);
                    sc.Control.Visible = false;

                    if (sc.Control is IDeactivate)
                    {
                        ((IDeactivate) (sc.Control)).Deactivate();
                    }

                    break;
                }
            }

            foreach (Control ctl in splitContainer1.Panel2.Controls)
            {
                ctl.Visible = true;
            }
        }

        private void tabQuick_Resize(object sender, EventArgs e)
        {
            tableLayoutPanelQuick.Width = tabQuick.Width;
            tableLayoutPanelQuick.AutoScroll = false;
        }

        private void hud1_Resize(object sender, EventArgs e)
        {
            Console.WriteLine("HUD resize " + hud1.Width + " " + hud1.Height); // +"\n"+ System.Environment.StackTrace);

            if (hud1.Parent == this.SubMainLeft.Panel1)
                SubMainLeft.SplitterDistance = hud1.Height;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void BUT_ARM_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            // arm the MAV
            try
            {
                var action = MainV2.comPort.MAV.cs.armed ? "Disarm" : "Arm";

                if (MainV2.comPort.MAV.cs.armed)
                    if (CustomMessageBox.Show("Are you sure you want to " + action, action, CustomMessageBox.MessageBoxButtons.YesNo) !=
                        CustomMessageBox.DialogResult.Yes)
                        return;

                bool ans = MainV2.comPort.doARM(!MainV2.comPort.MAV.cs.armed);
                if (ans == false)
                {
                    if (CustomMessageBox.Show(action + " failed. Force " + action, Strings.ERROR, CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
                    {
                        ans = MainV2.comPort.doARM(!MainV2.comPort.MAV.cs.armed, true);
                        if (ans == false)
                        {
                            CustomMessageBox.Show(Strings.ErrorRejectedByMAV, Strings.ERROR);
                        }
                    }
                }
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
            }
        }

        private void modifyandSetAlt_Click(object sender, EventArgs e)
        {
            int newalt = (int) modifyandSetAlt.Value;
            try
            {
                MainV2.comPort.setNewWPAlt(new Locationwp {alt = newalt/CurrentState.multiplieralt });
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorCommunicating, Strings.ERROR);
            }
        }

        private void gMapControl1_MouseLeave(object sender, EventArgs e)
        {
            if (marker != null)
            {
                try
                {
                    if (routes.Markers.Contains(marker))
                        routes.Markers.Remove(marker);
                }
                catch
                {
                }
            }
        }

        private void modifyandSetSpeed_Click(object sender, EventArgs e)
        {
            // QUAD
            if (MainV2.comPort.MAV.param.ContainsKey("WP_SPEED_MAX"))
            {
                try
                {
                    MainV2.comPort.setParam("WP_SPEED_MAX", ((float) modifyandSetSpeed.Value*100.0f));
                }
                catch
                {
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, "WP_SPEED_MAX"), Strings.ERROR);
                }
            } // plane 3.7 and below with airspeed, uses ARSPD_ENABLE:
            else if ((MainV2.comPort.MAV.param.ContainsKey("TRIM_ARSPD_CM") &&
                     MainV2.comPort.MAV.param.ContainsKey("ARSPD_ENABLE")
                     && MainV2.comPort.MAV.param.ContainsKey("ARSPD_USE") &&
                     (float)MainV2.comPort.MAV.param["ARSPD_ENABLE"] == 1
                     && (float)MainV2.comPort.MAV.param["ARSPD_USE"] == 1) ||
              // plane 3.8 and above with airspeed as per plane 3.7 to plane 3.8 migration wiki page, no longer uses ARSPD_ENABLE, uses ARSPD_TYPE instead:
                     (MainV2.comPort.MAV.param.ContainsKey("TRIM_ARSPD_CM") &&
                     MainV2.comPort.MAV.param.ContainsKey("ARSPD_TYPE")
                     && MainV2.comPort.MAV.param.ContainsKey("ARSPD_USE") &&
                     (float)MainV2.comPort.MAV.param["ARSPD_TYPE"] > 0
                     && (float)MainV2.comPort.MAV.param["ARSPD_USE"] == 1))
            {
                try
                {
                    MainV2.comPort.setParam("TRIM_ARSPD_CM", ((float) modifyandSetSpeed.Value*100.0f));
                }
                catch
                {
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, "TRIM_ARSPD_CM"), Strings.ERROR);
                }
            } // plane without airspeed
            else if (MainV2.comPort.MAV.param.ContainsKey("TRIM_THROTTLE") &&
                     MainV2.comPort.MAV.param.ContainsKey("ARSPD_USE")
                     && (float) MainV2.comPort.MAV.param["ARSPD_USE"] == 0)
            {
                try
                {
                    MainV2.comPort.setParam("TRIM_THROTTLE", (float) modifyandSetSpeed.Value);
                }
                catch
                {
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, "TRIM_THROTTLE"),
                        Strings.ERROR);
                }
            }
        }

        private void modifyandSetSpeed_ParentChanged(object sender, EventArgs e)
        {
        }

        private void triggerCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.setDigicamControl(true);
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
        }

        private void TRK_zoom_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (gMapControl1.MaxZoom + 1 == (double) TRK_zoom.Value)
                {
                    gMapControl1.Zoom = TRK_zoom.Value - .1;
                }
                else
                {
                    gMapControl1.Zoom = TRK_zoom.Value;
                }

                UpdateOverlayVisibility();
            }
            catch
            {
            }
        }

        private void BUT_speed1_Click(object sender, EventArgs e)
        {
            LogPlayBackSpeed = double.Parse(((MyButton) sender).Tag.ToString(), CultureInfo.InvariantCulture);
            lbl_playbackspeed.Text = "x " + LogPlayBackSpeed;
        }

        private void BUT_logbrowse_Click(object sender, EventArgs e)
        {
            Form logbrowse = new LogBrowse();
            ThemeManager.ApplyThemeTo(logbrowse);
            logbrowse.Show();
        }

        private void BUT_select_script_Click(object sender, EventArgs e)
        {
            if (openScriptDialog.ShowDialog() == DialogResult.OK)
            {
                selectedscript = openScriptDialog.FileName;
                BUT_run_script.Visible = BUT_edit_selected.Visible = true;
                labelSelectedScript.Text = "Selected Script: " + selectedscript;
            }
            else
            {
                selectedscript = "";
            }
        }

        internal void BUT_run_script_Click(object sender, EventArgs e)
        {
            if (File.Exists(selectedscript))
            {
                scriptthread = new Thread(run_selected_script)
                {
                    IsBackground = true,
                    Name = "Script Thread (new)"
                };
                labelScriptStatus.Text = "Script Status: Running";

                script = null;
                outputwindowstarted = false;

                scriptthread.Start();
                scriptrunning = true;
                BUT_run_script.Enabled = false;
                BUT_select_script.Enabled = false;
                BUT_abort_script.Visible = true;
                BUT_edit_selected.Enabled = false;
                scriptChecker.Enabled = true;
                checkBoxRedirectOutput.Enabled = false;

                while (script == null)
                {
                }

                scriptChecker_Tick(null, null);

                MissionPlanner.Utilities.Tracking.AddPage(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(),
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            else
            {
                CustomMessageBox.Show("Please select a valid script", "Bad Script");
            }
        }

        void run_selected_script()
        {
            script = new Script(checkBoxRedirectOutput.Checked);
            script.runScript(selectedscript);
            scriptrunning = false;
        }

        private void scriptChecker_Tick(object sender, EventArgs e)
        {
            if (!scriptrunning)
            {
                labelScriptStatus.Text = "Script Status: Finished (or aborted)";
                scriptChecker.Enabled = false;
                BUT_select_script.Enabled = true;
                BUT_run_script.Enabled = true;
                BUT_abort_script.Visible = false;
                BUT_edit_selected.Enabled = true;
                checkBoxRedirectOutput.Enabled = true;
            }
            else if ((script != null) && (checkBoxRedirectOutput.Checked) && (!outputwindowstarted))
            {
                outputwindowstarted = true;

                ScriptConsole console = new ScriptConsole();
                console.SetScript(script);
                ThemeManager.ApplyThemeTo(console);
                console.Show();
                console.BringToFront();
                components.Add(console);
            }
        }

        private void BUT_abort_script_Click(object sender, EventArgs e)
        {
            scriptthread.Abort();
            scriptrunning = false;
            BUT_abort_script.Visible = false;
        }

        private void BUT_edit_selected_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(selectedscript);
                psi.UseShellExecute = true;
                Process.Start(psi);
            }
            catch
            {
            }
        }

        private void russianHudToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hud1.Russian = !hud1.Russian;
            Settings.Instance["russian_hud"] = hud1.Russian.ToString();
        }

        private void setHomeHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.BaseStream.IsOpen)
            {
                try
                {
                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_HOME, 0, 0, 0, 0, (float) MouseDownStart.Lat,
                        (float) MouseDownStart.Lng, (float) srtm.getAltitude(MouseDownStart.Lat, MouseDownStart.Lng).alt);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
        }

        private void BUT_matlab_Click(object sender, EventArgs e)
        {
            MatLabForms.ProcessLog();
        }

        private void BUT_mountmode_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param.ContainsKey("MNT_MODE"))
                {
                    MainV2.comPort.setParam("MNT_MODE",(int) CMB_mountmode.SelectedValue);
                }
                else
                {
                    // copter 3.3 acks with an error, but is ok
                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_MOUNT_CONTROL, 0, 0, 0, 0, 0, 0,
                        (int) CMB_mountmode.SelectedValue);
                }
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
            }
        }

        private void but_bintolog_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Binary Log|*.bin;*.BIN";
                ofd.Multiselect = true;

                ofd.ShowDialog();

                foreach (string logfile in ofd.FileNames)
                {
                    string outfilename = Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar +
                                         Path.GetFileNameWithoutExtension(logfile) + ".log";

                    BinaryLog.ConvertBin(logfile, outfilename);
                }
            }
        }

        private void but_dflogtokml_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "Log Files|*.log;*.bin";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = Settings.Instance.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        LogOutput lo = new LogOutput();
                        try
                        {
                            StreamReader tr;

                            if (logfile.ToLower().EndsWith(".bin"))
                            {
                                using (tr = new StreamReader(logfile))
                                {
                                    GC.Collect();
                                    CollectionBuffer temp = new CollectionBuffer(tr.BaseStream);

                                    uint a = 0;
                                    foreach (var line in temp)
                                    {
                                        lo.processLine(line);
                                        a++;

                                        if ((a % 100000) == 0)
                                            Console.WriteLine(a);
                                    }

                                    temp.Dispose();
                                }
                            }
                            else
                            {
                                using (tr = new StreamReader(logfile))
                                {
                                    while (!tr.EndOfStream)
                                    {
                                        lo.processLine(tr.ReadLine());
                                    }

                                    tr.Close();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show("Error processing file. Make sure the file is not in use.\n" + ex);
                        }

                        lo.writeKML(logfile + ".kml");
                    }
                }
            }
        }

        private void BUT_DFMavlink_Click(object sender, EventArgs e)
        {
            var form = new LogDownloadMavLink();

            form.Show();
        }

        int messagecount;
        private bool CameraOverlap;

        private void Messagetabtimer_Tick(object sender, EventArgs e)
        {
            var newmsgcount = MainV2.comPort.MAV.cs.messages.Count;
            if (messagecount != newmsgcount)
            {
                try
                {
                    StringBuilder message = new StringBuilder();
                    MainV2.comPort.MAV.cs.messages.ForEach(x => { message.Insert(0, x + "\r\n"); });
                    txt_messagebox.Text = message.ToString();

                    messagecount = newmsgcount;
                }
                catch
                {
                }
            }
        }

        private void dropOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void BUT_loganalysis_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.log;*.bin|*.log;*.bin";
                ofd.ShowDialog();

                if (ofd.FileName != "")
                {
                    string newlogfile = null;

                    if (ofd.FileName.ToLower().EndsWith(".bin"))
                    {
                        newlogfile = Path.GetTempFileName() + ".log";

                        try
                        {
                            BinaryLog.ConvertBin(ofd.FileName, newlogfile);
                        }
                        catch (IOException ex)
                        {
                            CustomMessageBox.Show("File access issue: " + ex.Message, Strings.ERROR);
                            return;
                        }

                        ofd.FileName = newlogfile;
                    }

                    string xmlfile = LogAnalyzer.CheckLogFile(ofd.FileName);

                    GC.Collect();

                    if (File.Exists(xmlfile))
                    {
                        try
                        {
                            var out1 = LogAnalyzer.Results(xmlfile);

                            Controls.LogAnalyzer frm = new Controls.LogAnalyzer(out1);

                            ThemeManager.ApplyThemeTo(frm);

                            frm.Show();
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show("Failed to load analyzer results\n"+ex.ToString());
                        }
                    }
                    else
                    {
                        CustomMessageBox.Show("Bad input file");
                    }

                    if (!String.IsNullOrEmpty(newlogfile))
                    {
                        try
                        {
                            File.Delete(newlogfile);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void FlightData_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;

            DateTime end = DateTime.Now.AddSeconds(5);

            while (thisthread.IsAlive && DateTime.Now < end)
            {
                Application.DoEvents();
            }

            // you cannot call join on the main thread, and invoke on the thread. as it just hangs on the invoke.

            //thisthread.Join();
        }

        private void takeOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.BaseStream.IsOpen)
            {
                flyToHereAltToolStripMenuItem_Click(null, null);

                MainV2.comPort.setMode("GUIDED");

                try
                {
                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, MainV2.comPort.MAV.GuidedMode.z);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
        }

        private void txt_messagebox_TextChanged(object sender, EventArgs e)
        {
            txt_messagebox.Select(txt_messagebox.Text.Length, 0);
            txt_messagebox.ScrollToCaret();
        }

        private void BUT_resumemis_Click(object sender, EventArgs e)
        {
            if (
                Common.MessageShowAgain("Resume Mission",
                    "Warning this will reprogram your mission, arm and issue a takeoff command (copter)") !=
                DialogResult.OK)
                return;

            try
            {
                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    string lastwp = MainV2.comPort.MAV.cs.lastautowp.ToString();
                    if (lastwp == "-1")
                        lastwp = "1";

                    if (InputBox.Show("Resume at", "Resume mission at waypoint#", ref lastwp) == DialogResult.OK)
                    {
                        int timeout = 0;
                        int lastwpno = int.Parse(lastwp);

                        // scan and check wp's we are skipping
                        // get our target wp
                        var lastwpdata = MainV2.comPort.getWP((ushort) lastwpno);

                        // get all
                        List<Locationwp> cmds = new List<Locationwp>();

                        var wpcount = MainV2.comPort.getWPCount();

                        for (ushort a = 0; a < wpcount; a++)
                        {
                            var wpdata = MainV2.comPort.getWP(a);

                            if (a < lastwpno && a != 0) // allow home
                            {
                                if (wpdata.id != (ushort) MAVLink.MAV_CMD.TAKEOFF)
                                    if (wpdata.id < (ushort) MAVLink.MAV_CMD.LAST)
                                        continue;

                                if (wpdata.id > (ushort) MAVLink.MAV_CMD.DO_LAST)
                                    continue;
                            }

                            cmds.Add(wpdata);
                        }

                        ushort wpno = 0;
                        // upload from wp 0 to end
                        MainV2.comPort.setWPTotal((ushort) (cmds.Count));

                        // add our do commands
                        foreach (var loc in cmds)
                        {
                            MAVLink.MAV_MISSION_RESULT ans = MainV2.comPort.setWP(loc, wpno,
                                (MAVLink.MAV_FRAME) (loc.options));
                            if (ans != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                            {
                                CustomMessageBox.Show("Upload wps failed " +
                                                      Enum.Parse(typeof (MAVLink.MAV_CMD), loc.id.ToString()) + " " +
                                                      Enum.Parse(typeof (MAVLink.MAV_MISSION_RESULT), ans.ToString()));
                                return;
                            }
                            wpno++;
                        }

                        MainV2.comPort.setWPACK();

                        FlightPlanner.instance.BUT_read_Click(this, null);

                        // set index back to 1
                        MainV2.comPort.setWPCurrent(1);

                        if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
                        {
                            while (MainV2.comPort.MAV.cs.mode.ToLower() != "Guided".ToLower())
                            {
                                MainV2.comPort.setMode("GUIDED");
                                Thread.Sleep(1000);
                                Application.DoEvents();
                                timeout++;

                                if (timeout > 30)
                                {
                                    CustomMessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
                                    return;
                                }
                            }

                            timeout = 0;
                            while (!MainV2.comPort.MAV.cs.armed)
                            {
                                MainV2.comPort.doARM(true);
                                Thread.Sleep(1000);
                                Application.DoEvents();
                                timeout++;

                                if (timeout > 30)
                                {
                                    CustomMessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
                                    return;
                                }
                            }

                            timeout = 0;
                            while (MainV2.comPort.MAV.cs.alt < (lastwpdata.alt - 2))
                            {
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, lastwpdata.alt);
                                Thread.Sleep(1000);
                                Application.DoEvents();
                                timeout++;

                                if (timeout > 40)
                                {
                                    CustomMessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
                                    return;
                                }
                            }
                        }

                        timeout = 0;
                        while (MainV2.comPort.MAV.cs.mode.ToLower() != "AUTO".ToLower())
                        {
                            MainV2.comPort.setMode("AUTO");
                            Thread.Sleep(1000);
                            Application.DoEvents();
                            timeout++;

                            if (timeout > 30)
                            {
                                CustomMessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + "\n"+ex.ToString(), Strings.ERROR);
            }
        }

        private void hud1_ekfclick(object sender, EventArgs e)
        {
            EKFStatus frm = new EKFStatus();
            frm.TopMost = true;
            frm.Show();
        }

        private void hud1_vibeclick(object sender, EventArgs e)
        {
            Vibration frm = new Vibration();
            frm.TopMost = true;
            frm.Show();
        }

        private void SwapHud1AndMap()
        {
            if (this.huddropout)
                return;

            MainH.Panel2.SuspendLayout();

            if (this.SubMainLeft.Panel1.Controls.Contains(hud1))
            {
                Settings.Instance["HudSwap"] = "true";
                MainH.Panel2.Controls.Add(hud1);
                SubMainLeft.Panel1.Controls.Add(tableMap);
            }
            else
            {
                Settings.Instance["HudSwap"] = "false";
                MainH.Panel2.Controls.Add(tableMap);
                SubMainLeft.Panel1.Controls.Add(hud1);
            }

            MainH.Panel2.ResumeLayout();
        }

        private void swapWithMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwapHud1AndMap();
        }

        private void BUT_abortland_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            try
            {
                MainV2.comPort.doAbortLand();
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
        }

        GMapMarker center = new GMarkerGoogle(new PointLatLng(0.0, 0.0), GMarkerGoogleType.none);

        private void gMapControl1_OnPositionChanged(PointLatLng point)
        {
            center.Position = point;

            UpdateOverlayVisibility();
        }

        void UpdateOverlayVisibility()
        {
            // change overlay visability
            if (gMapControl1.ViewArea != null)
            {
                var bounds = gMapControl1.ViewArea;
                bounds.Inflate(1, 1);

                foreach (var poly in kmlpolygons.Polygons)
                {
                    if (bounds.Contains(poly.Points[0]))
                        poly.IsVisible = true;
                    else
                        poly.IsVisible = false;
                }
            }
        }

        private void but_disablejoystick_Click(object sender, EventArgs e)
        {
            if (MainV2.joystick != null && MainV2.joystick.enabled)
            {
                MainV2.joystick.enabled = false;

                MainV2.joystick.clearRCOverride();

                but_disablejoystick.Visible = false;
            }
        }

        private void startCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;
            if (MainV2.cam == null)
            {
                try
                {
                    MainV2.cam = new Capture(Settings.Instance.GetInt32("video_device"), new AMMediaType());

                    MainV2.cam.Start();

                    MainV2.cam.camimage += new CamImage(cam_camimage);
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show("Camera Fail: " + ex.ToString(), Strings.ERROR);
                }
            }
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form customForm = new Form())
            {
                CheckedListBox left = new CheckedListBox();
                left.Dock = DockStyle.Fill;
                left.CheckOnClick = true;

                customForm.Controls.Add(left);

                string tabs = Settings.Instance["tabcontrolactions"];

                // setup default if doesnt exist
                if (tabs == null)
                {
                    saveTabControlActions();
                    tabs = Settings.Instance["tabcontrolactions"];
                }

                string[] tabarray = tabs.Split(';');

                foreach (TabPage tabPage in TabListOriginal)
                {
                    if (tabarray.Contains(tabPage.Name))
                        left.Items.Add(tabPage.Name, true);
                    else
                        left.Items.Add(tabPage.Name, false);
                }

                ThemeManager.ApplyThemeTo(customForm);

                customForm.ShowDialog();

                string answer = "";
                foreach (var tabPage in left.CheckedItems)
                {
                    answer += tabPage + ";";
                }

                Settings.Instance["tabcontrolactions"] = answer;

                loadTabControlActions();
            }
        }

        public void loadTabControlActions()
        {
            string tabs = Settings.Instance["tabcontrolactions"];

            if (String.IsNullOrEmpty(tabs) || TabListOriginal == null || TabListOriginal.Count == 0)
                return;

            string[] tabarray = tabs.Split(';');

            if (tabarray.Length == 0)
                return;

            tabControlactions.TabPages.Clear();

            foreach (var tabname in tabarray)
            {
                int a = 0;
                foreach (TabPage tabPage in TabListOriginal)
                {
                    if (tabPage.Name == tabname)
                    {
                        tabControlactions.TabPages.Add(tabPage);
                        break;
                    }
                    a++;
                }
            }

            ThemeManager.ApplyThemeTo(tabControlactions);
        }

        private void saveTabControlActions()
        {
            string answer = "";

            foreach (TabPage tabPage in tabControlactions.TabPages)
            {
                answer += tabPage.Name + ";";
            }

            Settings.Instance["tabcontrolactions"] = answer;
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POI.POILoad();
        }

        private void PointCameraCoordsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var location = "";
            InputBox.Show("Enter Coords", "Please enter the coords 'lat;long;alt' or 'lat;long'", ref location);

            var split = location.Split(';');

            if (split.Length == 3)
            {
                var lat = float.Parse(split[0], CultureInfo.InvariantCulture);
                var lng = float.Parse(split[1], CultureInfo.InvariantCulture);
                var alt = float.Parse(split[2], CultureInfo.InvariantCulture);

                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0, lat, lng,
                    alt/CurrentState.multiplieralt);
            } 
            else if (split.Length == 2)
            {
                var lat = float.Parse(split[0], CultureInfo.InvariantCulture);
                var lng = float.Parse(split[1], CultureInfo.InvariantCulture);
                var alt = srtm.getAltitude(MouseDownStart.Lat, MouseDownStart.Lng).alt/CurrentState.multiplieralt;

                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0, lat, lng, (float) alt);
            }
            else
            {
                CustomMessageBox.Show(Strings.InvalidField, Strings.ERROR);
            }
        }

        private void modifyandSetLoiterRad_Click(object sender, EventArgs e)
        {
            int newrad = (int)modifyandSetLoiterRad.Value;
            
            try
            {
                MainV2.comPort.setParam(new[] { "LOITER_RAD", "WP_LOITER_RAD" },newrad / CurrentState.multiplierdist);
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorCommunicating, Strings.ERROR);
            }
        }

        private void onOffCameraOverlapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraOverlap = onOffCameraOverlapToolStripMenuItem.Checked;
        }

        private void altitudeAngelSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Utilities.AltitudeAngel.AASettings().Show(this);
        }

        private void setViewCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cols = "2", rows = "3";

            if (Settings.Instance["quickViewRows"]!= null)
            {
                rows = Settings.Instance["quickViewRows"];
                cols = Settings.Instance["quickViewCols"]; 
            }

            if (InputBox.Show("Columns", "Enter number of columns to have.", ref cols) == DialogResult.OK)
            {
                if (InputBox.Show("Rows", "Enter number of rows to have.", ref rows) == DialogResult.OK)
                {
                    setQuickViewRowsCols(cols, rows);

                    Activate();
                }
            }
        }

        private void setQuickViewRowsCols(string cols, string rows)
        {
            tableLayoutPanelQuick.ColumnCount = Math.Max(1,int.Parse(cols));
            tableLayoutPanelQuick.RowCount = Math.Max(1,int.Parse(rows));

            Settings.Instance["quickViewRows"] = tableLayoutPanelQuick.RowCount.ToString();
            Settings.Instance["quickViewCols"] = tableLayoutPanelQuick.ColumnCount.ToString();

            int total = tableLayoutPanelQuick.ColumnCount * tableLayoutPanelQuick.RowCount;

            // clean up extra
            while (tableLayoutPanelQuick.Controls.Count > total)
                tableLayoutPanelQuick.Controls.RemoveAt(tableLayoutPanelQuick.Controls.Count - 1);

            // add extra
            while (total != tableLayoutPanelQuick.Controls.Count)
            {
                var QV = new QuickView()
                {
                    Name = "quickView" + (tableLayoutPanelQuick.Controls.Count + 1)
                };
                QV.DoubleClick += quickView_DoubleClick;
                QV.ContextMenuStrip = contextMenuStripQuickView;
                QV.Dock = DockStyle.Fill;
                QV.numberColor = GetColor();
                QV.number = 0;

                tableLayoutPanelQuick.Controls.Add(QV);
                QV.Invalidate();
            }

            for (int i = 0; i < tableLayoutPanelQuick.ColumnCount; i++)
            {
                if (tableLayoutPanelQuick.ColumnStyles.Count <= i)
                    tableLayoutPanelQuick.ColumnStyles.Add(new ColumnStyle());
                tableLayoutPanelQuick.ColumnStyles[i].SizeType = SizeType.Percent;
                tableLayoutPanelQuick.ColumnStyles[i].Width = 100.0f / tableLayoutPanelQuick.ColumnCount;
            }
            for (int j = 0; j < tableLayoutPanelQuick.RowCount; j++)
            {
                if (tableLayoutPanelQuick.RowStyles.Count <= j)
                    tableLayoutPanelQuick.RowStyles.Add(new RowStyle());
                tableLayoutPanelQuick.RowStyles[j].SizeType = SizeType.Percent;
                tableLayoutPanelQuick.RowStyles[j].Height = 100.0f / tableLayoutPanelQuick.RowCount;
            }
        }

        Random random = new Random();

        Color GetColor()
        {
            //The mix color is set to the inverse of background color, so white background will get dark colors
            Color mix = Color.FromArgb(ThemeManager.BGColor.ToArgb() ^ 0xffffff); 
            
            int red = random.Next(256);
            int green = random.Next(256);
            int blue = random.Next(256);

            // mix the color
            if (mix != null)
            {
                red = (red + mix.R) / 2;
                green = (green + mix.G) / 2;
                blue = (blue + mix.B) / 2;
            }

            var col = Color.FromArgb(red, green, blue);

            this.LogInfo("GetColor() " + col);

            return col;
        }

        private void setGStreamerSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = Settings.Instance["gstreamer_url"] != null
                ? Settings.Instance["gstreamer_url"]
                : @"rtspsrc location=rtsp://192.168.1.133:8554/video1 ! application/x-rtp ! rtpjpegdepay ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink";

            if (DialogResult.OK == InputBox.Show("GStreamer url", "Enter the source pipeline\nEnsure the final payload is ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink", ref url))
            {
                Settings.Instance["gstreamer_url"] = url;

                if (!File.Exists(GStreamer.gstlaunch))
                {
                    UDPVideoShim.DownloadGStreamer();

                    if (!File.Exists(GStreamer.gstlaunch))
                    {
                        return;
                    }
                }

                GStreamer.StartA(url);
            }
            else
            {
                GStreamer.Stop(null);
            }
        }

        private void setEKFHomeHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            MAVLink.mavlink_set_gps_global_origin_t go = new MAVLink.mavlink_set_gps_global_origin_t()
            {
                latitude = (int)(MouseDownStart.Lat * 1e7),
                longitude = (int)(MouseDownStart.Lng * 1e7),
                altitude = (int)srtm.getAltitude(MouseDownStart.Lat, MouseDownStart.Lng).alt,
                target_system = MainV2.comPort.MAV.sysid
            };

            MainV2.comPort.sendPacket(go, MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid);
        }


        //Updates the visibility of the payload control tab based on whether the payload target is available or not
        public void updatePayloadTabVisible()
        {
            bool gimbalPresent = false;

            //if the currently connected target is a flight controller check if there is an associated mavlink gimbal
            if (MainV2.comPort.compidcurrent == 1)
            {
                foreach (var mav in MainV2.comPort.MAVlist)
                {
                    if (mav.sysid == MainV2.comPort.sysidcurrent && mav.compid == (int)MAVLink.MAV_COMPONENT.MAV_COMP_ID_GIMBAL)
                    {
                        gimbalPresent = true;
                        break;
                    }
                }
            }

            if (tabControlactions.TabPages.Contains(tabPayload) == true && gimbalPresent == false)
            {
                tabControlactions.TabPages.Remove(tabPayload);
            }
            else if (tabControlactions.TabPages.Contains(tabPayload) == false && gimbalPresent == true)
            {
                tabControlactions.TabPages.Add(tabPayload);
            }
        }

        private void gimbalTrackbar_Scroll(object sender, EventArgs e)
        {
            MainV2.comPort.setMountControl((float)trackBarPitch.Value * 100.0f, (float)trackBarRoll.Value * 100.0f, (float)trackBarYaw.Value * 100.0f, false);
        }

        private void BUT_resetGimbalPos_Click(object sender, EventArgs e)
        {
            trackBarPitch.Value = 0;
            trackBarRoll.Value = 0;
            trackBarYaw.Value = 0;
            MainV2.comPort.setMountConfigure(MAVLink.MAV_MOUNT_MODE.MAVLINK_TARGETING, false, false, false);
            MainV2.comPort.setMountControl((float)trackBarPitch.Value * 100.0f, (float)trackBarRoll.Value * 100.0f, (float)trackBarYaw.Value * 100.0f, false);
        }

        private void groundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groundColorToolStripMenuItem.Checked)
            {
                // brown
                hud1.groundColor1 = Color.FromArgb(147, 78, 1);
                hud1.groundColor2 = Color.FromArgb(60, 33, 4);
            }
            else
            {
                // green
                hud1.groundColor1 = Color.FromArgb(0x9b, 0xb8, 0x24);
                hud1.groundColor2 = Color.FromArgb(0x41, 0x4f, 0x07);
            }

            Settings.config["groundColorToolStripMenuItem"] = groundColorToolStripMenuItem.Checked.ToString();
        }
    }
}
