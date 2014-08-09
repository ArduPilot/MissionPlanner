using System;
using System.Collections.Generic; // Lists
using System.Text; // stringbuilder
using System.Drawing; // pens etc
using System.IO; // file io
using System.IO.Ports; // serial
using System.Windows.Forms; // Forms
using System.Collections; // hashs
using System.Text.RegularExpressions; // regex
using System.Xml; // GE xml alt reader
using System.Net; // dns, ip address
using System.Net.Sockets; // tcplistner
using System.Threading;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Globalization; // language
using GMap.NET.WindowsForms.Markers;
using ZedGraph; // Graphs
using System.Drawing.Drawing2D;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using MissionPlanner.Controls.BackstageView;
//using Crom.Controls.Docking;
using log4net;
using System.Reflection;
using MissionPlanner.Log;

// written by michael oborne
namespace MissionPlanner.GCSViews
{
    partial class Simple : Form, IActivate, IDeactivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static int threadrun = 0;
        StreamWriter swlog;
        int tickStart = 0;
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

        System.Reflection.PropertyInfo list1item = null;
        System.Reflection.PropertyInfo list2item = null;
        System.Reflection.PropertyInfo list3item = null;
        System.Reflection.PropertyInfo list4item = null;
        System.Reflection.PropertyInfo list5item = null;
        System.Reflection.PropertyInfo list6item = null;
        System.Reflection.PropertyInfo list7item = null;
        System.Reflection.PropertyInfo list8item = null;
        System.Reflection.PropertyInfo list9item = null;
        System.Reflection.PropertyInfo list10item = null;

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

        internal static GMapOverlay kmlpolygons;
        internal static GMapOverlay geofence;

        Antenna.Tracker track = new Antenna.Tracker();

        Dictionary<Guid, Form> formguids = new Dictionary<Guid, Form>();

        //      private DockStateSerializer _serializer = null;

        List<PointLatLng> trackPoints = new List<PointLatLng>();

        const float rad2deg = (float)(180 / Math.PI);

        const float deg2rad = (float)(1.0 / rad2deg);

        public static GMapControl mymap = null;

        GMapMarker marker;


        public SplitContainer MainHcopy = null;

        public static Simple instance;

        protected override void Dispose(bool disposing)
        {
            threadrun = 0;
            MainV2.comPort.logreadmode = false;

            System.Threading.Thread.Sleep(100);

            if (swlog != null)
                swlog.Dispose();
            if (polygon != null)
                polygon.Dispose();
            if (polygons != null)
                polygons.Dispose();
            if (marker != null)
                marker.Dispose();

            if (components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        public Simple()
        {
            InitializeComponent();

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);

            instance = this;
            //    _serializer = new DockStateSerializer(dockContainer1);
            //    _serializer.SavePath = Application.StartupPath + Path.DirectorySeparatorChar + "FDscreen.xml";
            //    dockContainer1.PreviewRenderer = new PreviewRenderer();
            //
            mymap = gMapControl1;
     
            MainHcopy = MainH;

            // setup default tuning graph
            if (MainV2.config["Tuning_Graph_Selected"] != null)
            {
                string line = MainV2.config["Tuning_Graph_Selected"].ToString();
                string[] lines = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string option in lines)
                {
                    chk_box_CheckedChanged((object)(new CheckBox() { Name = option, Checked = true }), new EventArgs());
                }
            }
            else
            {
                chk_box_CheckedChanged((object)(new CheckBox() { Name = "roll", Checked = true }), new EventArgs());
                chk_box_CheckedChanged((object)(new CheckBox() { Name = "pitch", Checked = true }), new EventArgs());
                chk_box_CheckedChanged((object)(new CheckBox() { Name = "nav_roll", Checked = true }), new EventArgs());
                chk_box_CheckedChanged((object)(new CheckBox() { Name = "nav_pitch", Checked = true }), new EventArgs());
            }


            List<string> list = new List<string>();

            {
                list.Add("LOITER_UNLIM");
                list.Add("RETURN_TO_LAUNCH");
                list.Add("PREFLIGHT_CALIBRATION");
                list.Add("MISSION_START");
                list.Add("PREFLIGHT_REBOOT_SHUTDOWN");
                //DO_SET_SERVO
                //DO_REPEAT_SERVO
            }


   

            zg1.Visible = true;

            CreateChart(zg1);

            // config map             
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            gMapControl1.MinZoom = 1;
            gMapControl1.CacheLocation = Path.GetDirectoryName(Application.ExecutablePath) + "/gmapcache/";

            gMapControl1.OnMapZoomChanged += new MapZoomChanged(gMapControl1_OnMapZoomChanged);

            gMapControl1.Zoom = 3;

            gMapControl1.RoutesEnabled = true;
            gMapControl1.PolygonsEnabled = true;

            kmlpolygons = new GMapOverlay( "kmlpolygons");
            gMapControl1.Overlays.Add(kmlpolygons);

            geofence = new GMapOverlay( "geofence");
            gMapControl1.Overlays.Add(geofence);

            polygons = new GMapOverlay( "polygons");
            gMapControl1.Overlays.Add(polygons);

            routes = new GMapOverlay( "routes");
            gMapControl1.Overlays.Add(routes);


            MainV2.comPort.ParamListChanged += FlightData_ParentChanged;
        }
   
  
        public new void Activate()
        {
            if (CB_tuning.Checked)
                ZedGraphTimer.Start();


            for (int f = 1; f < 10; f++)
            {
                // load settings
                if (MainV2.config["quickView" + f] != null)
                {
                    Control[] ctls = this.Controls.Find("quickView" + f, true);
                    if (ctls.Length > 0)
                    {
                        // set description
                        ((QuickView)ctls[0]).desc = MainV2.comPort.MAV.cs.GetNameandUnit(MainV2.config["quickView" + f].ToString());

                        // set databinding for value
                        ((QuickView)ctls[0]).DataBindings.Clear();
                        ((QuickView)ctls[0]).DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSource1, MainV2.config["quickView" + f].ToString(), true));
                    }
                }
                else
                {
                    // if no config, update description on predefined
                    try
                    {
                          Control[] ctls = this.Controls.Find("quickView" + f, true);
                          if (ctls.Length > 0)
                          {
                              ((QuickView)ctls[0]).desc = MainV2.comPort.MAV.cs.GetNameandUnit(((QuickView)ctls[0]).desc);
                          }
                    }
                    catch { }
                }
            }



            foreach (Control ctl in splitContainer1.Panel2.Controls)
            {
                ctl.Visible = true;
            }
        }

        public new void Deactivate()
        {

            //     hud1.Location = new Point(-1000,-1000);

            ZedGraphTimer.Stop();
        }

        void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Draw the background of the ListBox control for each item.
            //e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;

            LinearGradientBrush linear = new LinearGradientBrush(e.Bounds, Color.FromArgb(0x94, 0xc1, 0x1f), Color.FromArgb(0xcd, 0xe2, 0x96), LinearGradientMode.Vertical);

            e.Graphics.FillRectangle(linear, e.Bounds);

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            e.Graphics.DrawString(((TabControl)sender).TabPages[e.Index].Text.ToString(),
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();

        }

        void gMapControl1_OnMapZoomChanged()
        {
            TRK_zoom.Value = (float)gMapControl1.Zoom;
            Zoomlevel.Value = Convert.ToDecimal(gMapControl1.Zoom);
        }

        private void FlightData_Load(object sender, EventArgs e)
        {
            System.Threading.Thread t11 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                IsBackground = true,
                Name = "FlightData updater"
            };

            t11.Start();
            //MainH.threads.Add(t11);

            TRK_zoom.Minimum = gMapControl1.MinZoom;
            TRK_zoom.Maximum = gMapControl1.MaxZoom + 1;
            TRK_zoom.Value = (float)gMapControl1.Zoom;

            Zoomlevel.Minimum = gMapControl1.MinZoom;
            Zoomlevel.Maximum = gMapControl1.MaxZoom + 1;
            Zoomlevel.Value = Convert.ToDecimal(gMapControl1.Zoom);

            if (MainV2.config["CHK_autopan"] != null)
                CHK_autopan.Checked = bool.Parse(MainV2.config["CHK_autopan"].ToString());

            if (MainV2.config.Contains("FlightSplitter"))
            {
                //  hud1.Width = int.Parse(MainV2.config["FlightSplitter"].ToString());
                //  MainH.PerformLayout();
                MainH.SplitterDistance = int.Parse(MainV2.config["FlightSplitter"].ToString());
            }
        }

        private void mainloop()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            threadrun = 1;
            EndPoint Remote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));

            DateTime lastdata = DateTime.MinValue;

            DateTime tracklast = DateTime.Now.AddSeconds(0);

            DateTime tunning = DateTime.Now.AddSeconds(0);

            DateTime mapupdate = DateTime.Now.AddSeconds(0);

            DateTime vidrec = DateTime.Now.AddSeconds(0);

            DateTime waypoints = DateTime.Now.AddSeconds(0);

            DateTime updatescreen = DateTime.Now;

            DateTime tsreal = DateTime.Now;
 

            //comPort.stopall(true);

            while (threadrun == 1)
            {
                if (threadrun == 0) { return; }

                if (MainV2.comPort.giveComport == true)
                {
                    System.Threading.Thread.Sleep(50);
                    continue;
                }
                if (!MainV2.comPort.BaseStream.IsOpen)
                {
                    lastdata = DateTime.Now;
                    continue;
                }

                // actauly read the packets
                while (MainV2.comPort.BaseStream.BytesToRead > 0 && MainV2.comPort.giveComport == false)
                {
                    try
                    {
                        MainV2.comPort.readPacket();
                    }
                    catch { }
                }

                // re-request servo data
                if (!(lastdata.AddSeconds(8) > DateTime.Now) && MainV2.comPort.BaseStream.IsOpen)
                {
                    //Console.WriteLine("REQ streams - flightdata");
                    try
                    {
                        //System.Threading.Thread.Sleep(1000);

                        //comPort.requestDatastream((byte)MissionPlanner.MAVLink09.MAV_DATA_STREAM.RAW_CONTROLLER, 0); // request servoout
                        MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTENDED_STATUS, MainV2.comPort.MAV.cs.ratestatus); // mode
                        MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, MainV2.comPort.MAV.cs.rateposition); // request gps
                        MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA1, MainV2.comPort.MAV.cs.rateattitude); // request attitude
                        MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA2, MainV2.comPort.MAV.cs.rateattitude); // request vfr
                        MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA3, MainV2.comPort.MAV.cs.ratesensors); // request extra stuff - tridge
                        MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors); // request raw sensor
                        MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, MainV2.comPort.MAV.cs.raterc); // request rc info
                    }
                    catch { log.Error("Failed to request rates"); }
                    lastdata = DateTime.Now.AddSeconds(120); // prevent flooding
                }

                if (!MainV2.comPort.logreadmode)
                    System.Threading.Thread.Sleep(100); // max is only ever 10 hz

    


                try
                {
                     //Console.WriteLine(DateTime.Now.Millisecond);
                    updateBindingSource();
                    // Console.WriteLine(DateTime.Now.Millisecond + " done ");

                    // battery warning.
                    float warnvolt = 0;
                    float.TryParse(MainV2.getConfig("speechbatteryvolt"), out warnvolt);
                    float warnpercent = 0;
                    float.TryParse(MainV2.getConfig("speechbatterypercent"), out warnpercent);


                    if (OpenGLtest.instance != null)
                    {
                        OpenGLtest.instance.rpy = new OpenTK.Vector3(MainV2.comPort.MAV.cs.roll, MainV2.comPort.MAV.cs.pitch, MainV2.comPort.MAV.cs.yaw);
                        OpenGLtest.instance.LocationCenter = new PointLatLngAlt(MainV2.comPort.MAV.cs.lat, MainV2.comPort.MAV.cs.lng, MainV2.comPort.MAV.cs.alt, "here");
                    }

                    if (tunning.AddMilliseconds(50) < DateTime.Now && CB_tuning.Checked == true)
                    {

                        double time = (Environment.TickCount - tickStart) / 1000.0;
                        if (list1item != null)
                            list1.Add(time, (float)list1item.GetValue((object)MainV2.comPort.MAV.cs, null));
                        if (list2item != null)
                            list2.Add(time, (float)list2item.GetValue((object)MainV2.comPort.MAV.cs, null));
                        if (list3item != null)
                            list3.Add(time, (float)list3item.GetValue((object)MainV2.comPort.MAV.cs, null));
                        if (list4item != null)
                            list4.Add(time, (float)list4item.GetValue((object)MainV2.comPort.MAV.cs, null));
                        if (list5item != null)
                            list5.Add(time, (float)list5item.GetValue((object)MainV2.comPort.MAV.cs, null));
                        if (list6item != null)
                            list6.Add(time, (float)list6item.GetValue((object)MainV2.comPort.MAV.cs, null));
                        if (list7item != null)
                            list7.Add(time, (float)list7item.GetValue((object)MainV2.comPort.MAV.cs, null));
                        if (list8item != null)
                            list8.Add(time, (float)list8item.GetValue((object)MainV2.comPort.MAV.cs, null));
                        if (list9item != null)
                            list9.Add(time, (float)list9item.GetValue((object)MainV2.comPort.MAV.cs, null));
                        if (list10item != null)
                            list10.Add(time, (float)list10item.GetValue((object)MainV2.comPort.MAV.cs, null));
                    }

                    if (tracklast.AddSeconds(1) < DateTime.Now)
                    {
                        if (MainV2.config["CHK_maprotation"] != null && MainV2.config["CHK_maprotation"].ToString() == "True")
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

                        int cnt = 0;

                        while (gMapControl1.inOnPaint == true)
                        {
                            System.Threading.Thread.Sleep(1);
                            cnt++;
                        }


                        while (route.Points.Count > 200)
                        {
                            //  trackPoints.RemoveRange(0, trackPoints.Count - int.Parse(MainV2.config["NUM_tracklength"].ToString()));
                            route.Points.RemoveAt(0);
                        }
                        if (MainV2.comPort.MAV.cs.lat != 0)
                        {
                            // trackPoints.Add(currentloc);
                            route.Points.Add(currentloc);
                        }


                        //                        if (CB_tuning.Checked == false) // draw if in view
                        {

                            while (gMapControl1.inOnPaint == true)
                            {
                                System.Threading.Thread.Sleep(1);
                            }

                            //route = new GMapRoute(route.Points, "track");
                            //track.Stroke = Pens.Red;
                            //route.Stroke = new Pen(Color.FromArgb(144, Color.Red));
                            //route.Stroke.Width = 5;
                            //route.Tag = "track";

                            //updateClearRoutes();

                            gMapControl1.UpdateRouteLocalPosition(route);

                            if (waypoints.AddSeconds(10) < DateTime.Now)
                            {
                                //Console.WriteLine("Doing FD WP's");
                                updateMissionRouteMarkers();

                                foreach (MAVLink.mavlink_mission_item_t plla in MainV2.comPort.MAV.wps.Values)
                                {
                                    if (plla.x == 0 || plla.y == 0)
                                        continue;

                                    if (plla.command == (byte)MAVLink.MAV_CMD.DO_SET_ROI)
                                    {
                                        addpolygonmarkerred(plla.seq.ToString(), plla.y, plla.x, (int)plla.z, Color.Red, routes);
                                        continue;
                                    }

                                    string tag = plla.seq.ToString();
                                    if (plla.seq == 0 && plla.current != 2)
                                    {
                                        tag = "Home";
                                    }
                                    if (plla.current == 2)
                                    {
                                        continue;
                                    }

                                    addpolygonmarker(tag, plla.y, plla.x, (int)plla.z, Color.White, polygons);
                                }

                                RegeneratePolygon();

                                waypoints = DateTime.Now;
                            }

                            //routes.Polygons.Add(poly);    

                            if (route.Points.Count > 0)
                            {
                                // add primary route icon
                                if (routes.Markers.Count != 1)
                                {
                                    routes.Markers.Clear();
                                    routes.Markers.Add(new GMarkerGoogle(currentloc,GMarkerGoogleType.none));
                                }

                                if (MainV2.comPort.MAV.cs.mode.ToLower() == "guided" && MainV2.comPort.MAV.GuidedMode.x != 0)
                                {
                                    addpolygonmarker("Guided Mode", MainV2.comPort.MAV.GuidedMode.y, MainV2.comPort.MAV.GuidedMode.x, (int)MainV2.comPort.MAV.GuidedMode.z, Color.Blue, routes);
                                }

                                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx)
                                {
                                    routes.Markers[0] = (new GMapMarkerPlane(currentloc, MainV2.comPort.MAV.cs.yaw, MainV2.comPort.MAV.cs.groundcourse, MainV2.comPort.MAV.cs.nav_bearing, MainV2.comPort.MAV.cs.target_bearing)
                                    {
                                        ToolTipText = MainV2.comPort.MAV.cs.mode + "\n" + MainV2.comPort.MAV.cs.alt.ToString("alt 0") + "\n" + MainV2.comPort.MAV.cs.groundspeed.ToString("gs 0"),
                                        ToolTipMode = MarkerTooltipMode.Always
                                    });
                                }
                                else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover)
                                {
                                    routes.Markers[0] = (new GMapMarkerRover(currentloc, MainV2.comPort.MAV.cs.yaw, MainV2.comPort.MAV.cs.groundcourse, MainV2.comPort.MAV.cs.nav_bearing, MainV2.comPort.MAV.cs.target_bearing));
                                }
                                else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER)
                                {
                                    routes.Markers[0] = (new GMapMarkerHeli(currentloc, MainV2.comPort.MAV.cs.yaw, MainV2.comPort.MAV.cs.groundcourse, MainV2.comPort.MAV.cs.nav_bearing));
                                }
                                else
                                {
                                    routes.Markers[0] = (new GMapMarkerQuad(currentloc, MainV2.comPort.MAV.cs.yaw, MainV2.comPort.MAV.cs.groundcourse, MainV2.comPort.MAV.cs.nav_bearing)
                                          {
                                              ToolTipText = MainV2.comPort.MAV.cs.mode + "\n" + "alt " + MainV2.comPort.MAV.cs.alt.ToString("0") + "\ngs " + MainV2.comPort.MAV.cs.groundspeed.ToString("0"),
                                              ToolTipMode = MarkerTooltipMode.Always
                                          });
                                }

                                if (route.Points[route.Points.Count - 1].Lat != 0 && (mapupdate.AddSeconds(3) < DateTime.Now) && CHK_autopan.Checked)
                                {
                                    updateMapPosition(currentloc);
                                    mapupdate = DateTime.Now;
                                }

                                if (route.Points.Count == 1 && gMapControl1.Zoom == 3) // 3 is the default load zoom
                                {
                                    updateMapPosition(currentloc);
                                    updateMapZoom(17);
                                    //gMapControl1.ZoomAndCenterMarkers("routes");// ZoomAndCenterRoutes("routes");
                                }
                            }

                            gMapControl1.HoldInvalidation = false;

                            gMapControl1.Invalidate();
                        }

                        tracklast = DateTime.Now;
                    }
                }
                catch (Exception ex) { Console.WriteLine("FD Main loop exception " + ex.ToString()); }
            }
            Console.WriteLine("FD Main loop exit");
        }

        private void setMapBearing()
        {
            this.Invoke((System.Windows.Forms.MethodInvoker)delegate()
            {
                gMapControl1.Bearing = (int)MainV2.comPort.MAV.cs.yaw;
            });
        }


        // to prevent cross thread calls while in a draw and exception
        private void updateClearRoutes()
        {
            // not async
            this.Invoke((System.Windows.Forms.MethodInvoker)delegate()
            {
                routes.Routes.Clear();
                routes.Routes.Add(route);
            });
        }

        // to prevent cross thread calls while in a draw and exception
        private void updateMissionRouteMarkers()
        {
            // not async
            this.Invoke((System.Windows.Forms.MethodInvoker)delegate()
            {
                polygons.Markers.Clear();
                routes.Markers.Clear();
            });
        }



        private void updateBindingSource()
        {
            this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
            {
                try
                {
                    MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSource1);
                }
                catch { }
            });
        }

        private void updateMapPosition(PointLatLng currentloc)
        {
            this.BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    gMapControl1.Position = currentloc;
                    //hud1.Refresh();
                }
                catch { }
            });
        }

        private void updateMapZoom(int zoom)
        {
            this.BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    gMapControl1.Zoom = zoom;
                }
                catch { }
            });
        }

        private void addpolygonmarker(string tag, double lng, double lat, int alt, Color? color, GMapOverlay overlay)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMarkerGoogle m = new GMarkerGoogle(point,GMarkerGoogleType.green);
                m.ToolTipMode = MarkerTooltipMode.Always;
                m.ToolTipText = tag;
                m.Tag = tag;

                GMapMarkerRect mBorders = new GMapMarkerRect(point);
                {

                    mBorders.InnerMarker = m;
                    try
                    {
                        mBorders.wprad = (int)(float.Parse(MissionPlanner.MainV2.config["TXT_WPRad"].ToString()) / MainV2.comPort.MAV.cs.multiplierdist);
                    }
                    catch { }
                    if (color.HasValue)
                    {
                        mBorders.Color = color.Value;
                    }
                }

                overlay.Markers.Add(m);
                overlay.Markers.Add(mBorders);
            }
            catch (Exception) { }
        }

        private void addpolygonmarkerred(string tag, double lng, double lat, int alt, Color? color, GMapOverlay overlay)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMarkerGoogle m = new GMarkerGoogle(point,GMarkerGoogleType.red);
                m.ToolTipMode = MarkerTooltipMode.Always;
                m.ToolTipText = tag;
                m.Tag = tag;

                GMapMarkerRect mBorders = new GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                }

                overlay.Markers.Add(m);
                overlay.Markers.Add(mBorders);
            }
            catch (Exception) { }
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

            if (polygon == null)
            {
                polygon = new GMapPolygon(polygonPoints, "polygon test");
                polygons.Polygons.Add(polygon);
            }
            else
            {
                polygon.Points.Clear();
                polygon.Points.AddRange(polygonPoints);

                polygon.Stroke = new Pen(Color.Yellow, 4);

                if (polygons.Polygons.Count == 0)
                {
                    polygons.Polygons.Add(polygon);
                }
                else
                {
                    gMapControl1.UpdatePolygonLocalPosition(polygon);
                }
            }
        }

        GMapPolygon polygon;
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
            ZedGraphTimer.Interval = 100;
            //timer1.Enabled = true;
            //timer1.Start();


            // Calculate the Axis Scale Ranges
            zgc.AxisChange();

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
                double time = (Environment.TickCount - tickStart) / 1000.0;

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
            catch { }

        }

        private void FlightData_FormClosing(object sender, FormClosingEventArgs e)
        {
            ZedGraphTimer.Stop();
            threadrun = 0;
            try
            {
                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    MainV2.comPort.Close();
                }
            }
            catch { }
        }

        private void BUT_clear_track_Click(object sender, EventArgs e)
        {
            if (route != null)
                route.Points.Clear();
        }

        private void BUT_save_log_Click(object sender, EventArgs e)
        {
            // close existing log first
            if (swlog != null)
                swlog.Close();

            try
            {
                Directory.CreateDirectory(MainV2.LogDir);
                swlog = new StreamWriter(MainV2.LogDir + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + " telem.log");
            }
            catch { CustomMessageBox.Show("Log creation error"); }
        }

        private void BUTrestartmission_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button)sender).Enabled = false;

                //comPort.doAction(MAVLink09.MAV_ACTION.MAV_ACTION_RETURN); // set nav from
                //System.Threading.Thread.Sleep(100);
                MainV2.comPort.setWPCurrent(1); // set nav to
                //System.Threading.Thread.Sleep(100);
                //comPort.doAction(MAVLink09.MAV_ACTION.MAV_ACTION_SET_AUTO); // set auto
            }
            catch { CustomMessageBox.Show("The command failed to execute"); }
            ((Button)sender).Enabled = true;
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

        PointLatLng gotolocation = new PointLatLng();

        private void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            gotolocation = gMapControl1.FromLocalToLatLng(e.X, e.Y);
        }

        private void goHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show("Please Connect First");
                return;
            }

            if (MainV2.comPort.MAV.GuidedMode.z == 0)
            {
                flyToHereAltToolStripMenuItem_Click(null, null);

                if (MainV2.comPort.MAV.GuidedMode.z == 0)
                    return;
            }

            if (gotolocation.Lat == 0 || gotolocation.Lng == 0)
            {
                CustomMessageBox.Show("Bad Lat/Long");
                return;
            }

            Locationwp gotohere = new Locationwp();

            gotohere.id = (byte)MAVLink.MAV_CMD.WAYPOINT;
            gotohere.alt = (float)(MainV2.comPort.MAV.GuidedMode.z); // back to m
            gotohere.lat = (gotolocation.Lat);
            gotohere.lng = (gotolocation.Lng);

            try
            {
                MainV2.comPort.setGuidedModeWP(gotohere);
            }
            catch (Exception ex) { MainV2.comPort.giveComport = false; CustomMessageBox.Show("Error sending command : " + ex.Message); }

        }

        private void Zoomlevel_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (gMapControl1.MaxZoom + 1 == (double)Zoomlevel.Value)
                {
                    gMapControl1.Zoom = (double)Zoomlevel.Value - .1;
                }
                else
                {
                    gMapControl1.Zoom = (double)Zoomlevel.Value;
                }
            }
            catch { }
        }

        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                double latdif = gotolocation.Lat - point.Lat;
                double lngdif = gotolocation.Lng - point.Lng;

                try
                {
                    gMapControl1.Position = new PointLatLng(gMapControl1.Position.Lat + latdif, gMapControl1.Position.Lng + lngdif);
                }
                catch { }
            }
            else
            {
                // setup a ballon with home distance
                if (marker != null)
                {
                    if (routes.Markers.Contains(marker))
                        routes.Markers.Remove(marker);
                }

                if (MainV2.getConfig("CHK_disttohomeflightdata") != false.ToString())
                {
                    marker = new GMapMarkerRect(point);
                    marker.ToolTip = new GMapToolTip(marker);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = "Dist to Home: " + ((gMapControl1.MapProvider.Projection.GetDistance(point, MainV2.comPort.MAV.cs.HomeLocation.Point()) * 1000) * MainV2.comPort.MAV.cs.multiplierdist).ToString("0");

                    routes.Markers.Add(marker);
                }
            }
        }

        private void FlightData_ParentChanged(object sender, EventArgs e)
        {
 

        }


        private void BUT_Homealt_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.MAV.cs.altoffsethome != 0)
            {
                MainV2.comPort.MAV.cs.altoffsethome = 0;
            }
            else
            {
                MainV2.comPort.MAV.cs.altoffsethome = MainV2.comPort.MAV.cs.alt / MainV2.comPort.MAV.cs.multiplierdist;
            }
        }

        private void gMapControl1_Resize(object sender, EventArgs e)
        {
            gMapControl1.Zoom = gMapControl1.Zoom + 0.01;
        }



        private void BUT_quickauto_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button)sender).Enabled = false;
                MainV2.comPort.setMode("Auto");
            }
            catch { CustomMessageBox.Show("The Command failed to execute"); }
            ((Button)sender).Enabled = true;
        }

        private void BUT_quickrtl_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button)sender).Enabled = false;
                MainV2.comPort.setMode("RTL");
            }
            catch { CustomMessageBox.Show("The Command failed to execute"); }
            ((Button)sender).Enabled = true;
        }

        private void BUT_quickmanual_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button)sender).Enabled = false;
                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx)
                    MainV2.comPort.setMode("Manual");
                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                    MainV2.comPort.setMode("Stabilize");

            }
            catch { CustomMessageBox.Show("The Command failed to execute"); }
            ((Button)sender).Enabled = true;
        }

        private void BUT_log2kml_Click(object sender, EventArgs e)
        {
            Form frm = new Log.MavlinkLog();
            ThemeManager.ApplyThemeTo(frm);
            frm.ShowDialog();
        }

        private void BUT_joystick_Click(object sender, EventArgs e)
        {
            Form joy = new Joystick.JoystickSetup();
            ThemeManager.ApplyThemeTo(joy);
            joy.Show();
        }


        bool setupPropertyInfo(ref System.Reflection.PropertyInfo input, string name, object source)
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
            Form selectform = new Form()
            {
                Name = "select",
                Width = 50,
                Height = 250,
                Text = "Graph This"
            };

            int x = 10;
            int y = 10;

            {
                CheckBox chk_box = new CheckBox();
                chk_box.Text = "Logarithmic";
                chk_box.Name = "Logarithmic";
                chk_box.Location = new Point(x, y);
                chk_box.Size = new System.Drawing.Size(100, 20);
                chk_box.CheckedChanged += new EventHandler(chk_log_CheckedChanged);

                selectform.Controls.Add(chk_box);
            }

            y += 20;

            object thisBoxed = MainV2.comPort.MAV.cs;
            Type test = thisBoxed.GetType();

            foreach (var field in test.GetProperties())
            {
                // field.Name has the field's name.
                object fieldValue;
                try
                {
                    fieldValue = field.GetValue(thisBoxed, null); // Get value
                }
                catch { continue; }

                // Get the TypeCode enumeration. Multiple types get mapped to a common typecode.
                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());

                if (!(typeCode == TypeCode.Single))
                    continue;

                CheckBox chk_box = new CheckBox();

                if (list1item != null && list1item.Name == field.Name)
                    chk_box.Checked = true;
                if (list2item != null && list2item.Name == field.Name)
                    chk_box.Checked = true;
                if (list3item != null && list3item.Name == field.Name)
                    chk_box.Checked = true;
                if (list4item != null && list4item.Name == field.Name)
                    chk_box.Checked = true;
                if (list5item != null && list5item.Name == field.Name)
                    chk_box.Checked = true;
                if (list6item != null && list6item.Name == field.Name)
                    chk_box.Checked = true;
                if (list7item != null && list7item.Name == field.Name)
                    chk_box.Checked = true;
                if (list8item != null && list8item.Name == field.Name)
                    chk_box.Checked = true;
                if (list9item != null && list9item.Name == field.Name)
                    chk_box.Checked = true;
                if (list10item != null && list10item.Name == field.Name)
                    chk_box.Checked = true;

                chk_box.Text = field.Name;
                chk_box.Name = field.Name;
                chk_box.Location = new Point(x, y);
                chk_box.Size = new System.Drawing.Size(100, 20);
                chk_box.CheckedChanged += new EventHandler(chk_box_CheckedChanged);

                selectform.Controls.Add(chk_box);

                Application.DoEvents();

                x += 0;
                y += 20;

                if (y > selectform.Height - 50)
                {
                    x += 100;
                    y = 10;

                    selectform.Width = x + 100;
                }
            }
            ThemeManager.ApplyThemeTo(selectform);
            selectform.Show();
        }


        void chk_log_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
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
            if (((CheckBox)sender).Checked)
            {
                if (list1item == null)
                {
                    if (setupPropertyInfo(ref list1item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list1.Clear();
                        list1curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list1, Color.Red, SymbolType.None);
                    }
                }
                else if (list2item == null)
                {
                    if (setupPropertyInfo(ref list2item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list2.Clear();
                        list2curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list2, Color.Blue, SymbolType.None);
                    }
                }
                else if (list3item == null)
                {
                    if (setupPropertyInfo(ref list3item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list3.Clear();
                        list3curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list3, Color.Green, SymbolType.None);
                    }
                }
                else if (list4item == null)
                {
                    if (setupPropertyInfo(ref list4item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list4.Clear();
                        list4curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list4, Color.Orange, SymbolType.None);
                    }
                }
                else if (list5item == null)
                {
                    if (setupPropertyInfo(ref list5item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list5.Clear();
                        list5curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list5, Color.Yellow, SymbolType.None);
                    }
                }
                else if (list6item == null)
                {
                    if (setupPropertyInfo(ref list6item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list6.Clear();
                        list6curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list6, Color.Magenta, SymbolType.None);
                    }
                }
                else if (list7item == null)
                {
                    if (setupPropertyInfo(ref list7item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list7.Clear();
                        list7curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list7, Color.Purple, SymbolType.None);
                    }
                }
                else if (list8item == null)
                {
                    if (setupPropertyInfo(ref list8item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list8.Clear();
                        list8curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list8, Color.LimeGreen, SymbolType.None);
                    }
                }
                else if (list9item == null)
                {
                    if (setupPropertyInfo(ref list9item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list9.Clear();
                        list9curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list9, Color.Cyan, SymbolType.None);
                    }
                }
                else if (list10item == null)
                {
                    if (setupPropertyInfo(ref list10item, ((CheckBox)sender).Name, MainV2.comPort.MAV.cs))
                    {
                        list10.Clear();
                        list10curve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name, list10, Color.Violet, SymbolType.None);
                    }
                }
                else
                {
                    CustomMessageBox.Show("Max 10 at a time.");
                    ((CheckBox)sender).Checked = false;
                }
                ThemeManager.ApplyThemeTo(this);

                string selected = "";
                try
                {
                    foreach (var curve in zg1.GraphPane.CurveList)
                    {
                        selected = selected + curve.Label.Text + "|";
                    }
                }
                catch { }
                MainV2.config["Tuning_Graph_Selected"] = selected;
            }
            else
            {
                // reset old stuff
                if (list1item != null && list1item.Name == ((CheckBox)sender).Name)
                {
                    list1item = null;
                    zg1.GraphPane.CurveList.Remove(list1curve);
                }
                if (list2item != null && list2item.Name == ((CheckBox)sender).Name)
                {
                    list2item = null;
                    zg1.GraphPane.CurveList.Remove(list2curve);
                }
                if (list3item != null && list3item.Name == ((CheckBox)sender).Name)
                {
                    list3item = null;
                    zg1.GraphPane.CurveList.Remove(list3curve);
                }
                if (list4item != null && list4item.Name == ((CheckBox)sender).Name)
                {
                    list4item = null;
                    zg1.GraphPane.CurveList.Remove(list4curve);
                }
                if (list5item != null && list5item.Name == ((CheckBox)sender).Name)
                {
                    list5item = null;
                    zg1.GraphPane.CurveList.Remove(list5curve);
                }
                if (list6item != null && list6item.Name == ((CheckBox)sender).Name)
                {
                    list6item = null;
                    zg1.GraphPane.CurveList.Remove(list6curve);
                }
                if (list7item != null && list7item.Name == ((CheckBox)sender).Name)
                {
                    list7item = null;
                    zg1.GraphPane.CurveList.Remove(list7curve);
                }
                if (list8item != null && list8item.Name == ((CheckBox)sender).Name)
                {
                    list8item = null;
                    zg1.GraphPane.CurveList.Remove(list8curve);
                }
                if (list9item != null && list9item.Name == ((CheckBox)sender).Name)
                {
                    list9item = null;
                    zg1.GraphPane.CurveList.Remove(list9curve);
                }
                if (list10item != null && list10item.Name == ((CheckBox)sender).Name)
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

            string alt = (100 * MainV2.comPort.MAV.cs.multiplierdist).ToString("0");
            if (System.Windows.Forms.DialogResult.Cancel == InputBox.Show("Enter Alt", "Enter Target Alt (absolute)", ref alt))
                return;

            int intalt = (int)(100 * MainV2.comPort.MAV.cs.multiplierdist);
            if (!int.TryParse(alt, out intalt))
            {
                CustomMessageBox.Show("Bad Alt");
                return;
            }

            if (gotolocation.Lat == 0 || gotolocation.Lng == 0)
            {
                CustomMessageBox.Show("Bad Lat/Long");
                return;
            }

            MainV2.comPort.setMountConfigure(MAVLink.MAV_MOUNT_MODE.GPS_POINT, true, true, true);
            MainV2.comPort.setMountControl(gotolocation.Lat, gotolocation.Lng, (int)(intalt / MainV2.comPort.MAV.cs.multiplierdist), true);

        }

        private void CHK_autopan_CheckedChanged(object sender, EventArgs e)
        {
            MainV2.config["CHK_autopan"] = CHK_autopan.Checked.ToString();
        }


        private void setMJPEGSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = MainV2.config["mjpeg_url"] != null ? MainV2.config["mjpeg_url"].ToString() : @"http://127.0.0.1:56781/map.jpg";

            if (DialogResult.OK == InputBox.Show("Mjpeg url", "Enter the url to the mjpeg source url", ref url))
            {

                MainV2.config["mjpeg_url"] = url;

                Utilities.CaptureMJPEG.Stop();

                Utilities.CaptureMJPEG.URL = url;

                Utilities.CaptureMJPEG.OnNewImage += new EventHandler(CaptureMJPEG_OnNewImage);

                Utilities.CaptureMJPEG.runAsync();
            }
            else
            {
                Utilities.CaptureMJPEG.Stop();
            }
        }

        void CaptureMJPEG_OnNewImage(object sender, EventArgs e)
        {
            GCSViews.FlightData.myhud.bgimage = (Image)sender;
        }

        private void quickView_DoubleClick(object sender, EventArgs e)
        {

            Form selectform = new Form()
            {
                Name = "select",
                Width = 50,
                Height = 250,
                Text = "Display This"
            };

            int x = 10;
            int y = 10;

            object thisBoxed = MainV2.comPort.MAV.cs;
            Type test = thisBoxed.GetType();

            foreach (var field in test.GetProperties())
            {
                // field.Name has the field's name.
                object fieldValue;
                try
                {
                    fieldValue = field.GetValue(thisBoxed, null); // Get value
                }
                catch { continue; }

                // Get the TypeCode enumeration. Multiple types get mapped to a common typecode.
                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());

                if (!(typeCode == TypeCode.Single))
                    continue;

                CheckBox chk_box = new CheckBox();

                // dont change to ToString() = null exception
                if (((QuickView)sender).Tag != null && ((QuickView)sender).Tag.ToString() == field.Name)
                    chk_box.Checked = true;

                chk_box.Text = field.Name;
                chk_box.Name = field.Name;
                chk_box.Tag = ((QuickView)sender);
                chk_box.Location = new Point(x, y);
                chk_box.Size = new System.Drawing.Size(100, 20);
                chk_box.CheckedChanged += new EventHandler(chk_box_quickview_CheckedChanged);

                selectform.Controls.Add(chk_box);

                Application.DoEvents();

                x += 0;
                y += 20;

                if (y > selectform.Height - 50)
                {
                    x += 100;
                    y = 10;

                    selectform.Width = x + 100;
                }
            }
            ThemeManager.ApplyThemeTo(selectform);
            selectform.Show();
        }

        void chk_box_quickview_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                // save settings
                MainV2.config[((QuickView)((CheckBox)sender).Tag).Name] = ((CheckBox)sender).Name;

                // set description
                string desc = ((CheckBox)sender).Name;
                ((QuickView)((CheckBox)sender).Tag).Tag = desc;

                desc = MainV2.comPort.MAV.cs.GetNameandUnit(desc);

                ((QuickView)((CheckBox)sender).Tag).desc = desc;

                // set databinding for value
                ((QuickView)((CheckBox)sender).Tag).DataBindings.Clear();
                ((QuickView)((CheckBox)sender).Tag).DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSource1, ((CheckBox)sender).Name, true));

                // close selection form
                ((Form)((CheckBox)sender).Parent).Close();
            }
        }

        private void flyToHereAltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string alt = "100";

            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
            {
                alt = (10 * MainV2.comPort.MAV.cs.multiplierdist).ToString("0");
            }
            else
            {
                alt = (100 * MainV2.comPort.MAV.cs.multiplierdist).ToString("0");
            }

            if (MainV2.config.ContainsKey("guided_alt"))
                alt = MainV2.config["guided_alt"].ToString();

            if (DialogResult.Cancel == InputBox.Show("Enter Alt", "Enter Guided Mode Alt", ref alt))
                return;

            MainV2.config["guided_alt"] = alt;

            int intalt = (int)(100 * MainV2.comPort.MAV.cs.multiplierdist);
            if (!int.TryParse(alt, out intalt))
            {
                CustomMessageBox.Show("Bad Alt");
                return;
            }

            MainV2.comPort.MAV.GuidedMode.z = intalt;

            if (MainV2.comPort.MAV.cs.mode == "Guided")
            {
                MainV2.comPort.setGuidedModeWP(new Locationwp() { alt = (float)MainV2.comPort.MAV.GuidedMode.z, lat = MainV2.comPort.MAV.GuidedMode.x, lng = MainV2.comPort.MAV.GuidedMode.y });
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
                    MyButton but = new MyButton() { Location = new Point(splitContainer1.Panel2.Width / 2, 0), Text = "Close" };
                    but.Click += new EventHandler(but_Click);

                    splitContainer1.Panel2.Controls.Add(but);
                    splitContainer1.Panel2.Controls.Add(sc.Control);
                    ThemeManager.ApplyThemeTo(sc.Control);

                    sc.Control.Dock = DockStyle.Fill;
                    sc.Control.Visible = true;

                    if (sc.Control is IActivate)
                    {
                        ((IActivate)(sc.Control)).Activate();
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
                    splitContainer1.Panel2.Controls.Remove((Control)sender);
                    sc.Control.Visible = false;

                    if (sc.Control is IDeactivate)
                    {
                        ((IDeactivate)(sc.Control)).Deactivate();
                    }

                    break;
                }
            }

            foreach (Control ctl in splitContainer1.Panel2.Controls)
            {
                ctl.Visible = true;
            }
        }


        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //dockContainer1.Clear();
            //SetupDocking();
            this.Refresh();
        }




        private void gMapControl1_MouseLeave(object sender, EventArgs e)
        {
            if (marker != null)
            {
                if (routes.Markers.Contains(marker))
                    routes.Markers.Remove(marker);
            }
        }


        private void triggerCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.setDigicamControl(true);
            }
            catch { CustomMessageBox.Show("Error sending command"); }
        }

        private void TRK_zoom_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (gMapControl1.MaxZoom + 1 == (double)TRK_zoom.Value)
                {
                    gMapControl1.Zoom = (double)TRK_zoom.Value - .1;
                }
                else
                {
                    gMapControl1.Zoom = (double)TRK_zoom.Value;
                }
            }
            catch { }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Comms.CommsSerialScan.Scan(false);

            DateTime deadline = DateTime.Now.AddSeconds(50);

            while (Comms.CommsSerialScan.foundport == false)
            {
                System.Threading.Thread.Sleep(100);

                if (DateTime.Now > deadline)
                {
                    CustomMessageBox.Show("Timeout waiting for autoscan/no mavlink device connected");
                    return;
                }
            }

            MainV2.comPort.BaseStream.PortName = Comms.CommsSerialScan.portinterface.PortName;
            MainV2.comPort.BaseStream.BaudRate = Comms.CommsSerialScan.portinterface.BaudRate;

            MainV2.comPort.Open(false);

            return;
            

            
        }

        private void trackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            frm.Size = track.Size;
            frm.Controls.Add(track);
            track.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            frm.ShowDialog();
        }

        private void rTLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainV2.comPort.setMode("RLT");
        }

        private void loiterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainV2.comPort.setMode("LOITER");
        }
    }
}
