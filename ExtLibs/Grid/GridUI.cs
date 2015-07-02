using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using com.drew.imaging.jpg;
using com.drew.metadata;
using com.drew.metadata.exif;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using log4net;
using MissionPlanner.Utilities;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using MissionPlanner;            //I added this

namespace MissionPlanner
{
    public partial class GridUI : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Variables
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        private GridPlugin plugin;
        static public Object thisLock = new Object();

        GMapOverlay routesOverlay;
        List<PointLatLngAlt> list = new List<PointLatLngAlt>();
        List<PointLatLngAlt> grid;
        List<PointLatLngAlt> myGrid = new List<PointLatLngAlt>();        //I added this to manipulate grid

        Dictionary<string, camerainfo> cameras = new Dictionary<string, camerainfo>();

        public string DistUnits = "";
        public string inchpixel = "";
        public string feet_fovH = "";
        public string feet_fovV = "";
        public double maxFlightDist = 5.000000;    //I added this variable        now this is in km for a 15 min flight time

        internal PointLatLng MouseDownStart = new PointLatLng();
        internal PointLatLng MouseDownEnd;
        internal PointLatLngAlt CurrentGMapMarkerStartPos;
        PointLatLng currentMousePosition;
        GMapMarker marker;
        GMapMarker CurrentGMapMarker = null;
        int CurrentGMapMarkerIndex = 0;
        bool isMouseDown = false;
        bool isMouseDraging = false;

        // Structures
        public struct camerainfo
        {
            public string name;
            public float focallen;
            public float sensorwidth;
            public float sensorheight;
            public float imagewidth;
            public float imageheight;
        }
        public struct GridData
        {
            public List<PointLatLngAlt> poly;
            public string camera;
            public decimal alt;
            public decimal angle;
            public bool camdir;
            public bool usespeed;
            public decimal dist;
            public decimal overshoot1;
            public decimal overshoot2;
            public decimal overlap;
            public decimal sidelap;
            public decimal spacing;
            public string startfrom;
            public bool autotakeoff;
            public bool autotakeoff_RTL;
            public bool alternateLanes;
            public decimal minlaneseparation;

            public bool internals;
            public bool footprints;
            public bool advanced;

            public bool trigdist;
            public bool digicam;
            public bool repeatservo;

            // Copter Settings
            public decimal copter_delay;
            public bool copter_headinghold_chk;
            public decimal copter_headinghold;
        }

        // GridUI
        public GridUI(GridPlugin plugin)
        {
            this.plugin = plugin;       //makes this equal to the plugin that we just sent in

            InitializeComponent();      //initializes variables, probably not important to look at again

            map.MapProvider = plugin.Host.FDMapType;    //google maps provider - figure out what it is doing later

            routesOverlay = new GMapOverlay("routes");  //routesOverLay ID is being set to equal a routes string
            map.Overlays.Add(routesOverlay);

            // Map Events
            map.OnMapZoomChanged += new MapZoomChanged(map_OnMapZoomChanged);
            map.OnMarkerEnter += new MarkerEnter(map_OnMarkerEnter);
            map.OnMarkerLeave += new MarkerLeave(map_OnMarkerLeave);
            map.MouseUp += new MouseEventHandler(map_MouseUp);

            map.OnRouteEnter += new RouteEnter(map_OnRouteEnter);
            map.OnRouteLeave += new RouteLeave(map_OnRouteLeave);

            plugin.Host.FPDrawnPolygon.Points.ForEach(x => { list.Add(x); });     //adds each Polygon Point (x) to the end of the list
            if (plugin.Host.config["distunits"] != null)
                DistUnits = plugin.Host.config["distunits"].ToString();

            CMB_startfrom.DataSource = Enum.GetNames(typeof(Grid.StartPosition));
            CMB_startfrom.SelectedIndex = 0;

            // set an angle that is good
            NUM_angle.Value = (decimal)((getAngleOfLongestSide(list) + 360) % 360);
            TXT_headinghold.Text = (Math.Round(NUM_angle.Value)).ToString();
        }

        private void GridUI_Load(object sender, EventArgs e)
        {
            xmlcamera(false, "camerasBuiltin.xml");

            xmlcamera(false);

            // setup state before settings load
            CHK_advanced_CheckedChanged(null, null);

            loadsettings();

            //CHK_advanced_CheckedChanged(null, null);

            TRK_zoom.Value = (float)map.Zoom;

            label1.Text += " (" + CurrentState.DistanceUnit+")";
            label24.Text += " (" + CurrentState.SpeedUnit + ")";
        }

        private void GridUI_Resize(object sender, EventArgs e)
        {
            map.ZoomAndCenterMarkers("polygons");
        }

        // Load/Save
        public void LoadGrid()
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(GridData));

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.grid|*.grid";
            ofd.ShowDialog();

            if (File.Exists(ofd.FileName))
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    var test = (GridData)reader.Deserialize(sr);

                    loadgriddata(test);
                }
            }
        }

        public void SaveGrid()
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(GridData));

            var griddata = savegriddata();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.grid|*.grid";
            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    writer.Serialize(sw, griddata);
                }
            }
        }

        void loadgriddata(GridData griddata)
        {
            list = griddata.poly;

            CMB_camera.Text = griddata.camera;
            NUM_altitude.Value = griddata.alt;
            NUM_angle.Value = griddata.angle;
            CHK_camdirection.Checked = griddata.camdir;

            CHK_usespeed.Checked = griddata.usespeed;

            NUM_Distance.Value = griddata.dist;
            NUM_overshoot.Value = griddata.overshoot1;
            NUM_overshoot2.Value = griddata.overshoot2;
            num_overlap.Value = griddata.overlap;
            num_sidelap.Value = griddata.sidelap;
            NUM_spacing.Value = griddata.spacing;

            CMB_startfrom.Text = griddata.startfrom;

            CHK_toandland.Checked = griddata.autotakeoff;
            CHK_toandland_RTL.Checked = griddata.autotakeoff_RTL;

            CHK_internals.Checked = griddata.internals;
            CHK_footprints.Checked = griddata.footprints;
            CHK_advanced.Checked = griddata.advanced;

            rad_trigdist.Checked = griddata.trigdist;
            rad_digicam.Checked = griddata.digicam;
            rad_repeatservo.Checked = griddata.repeatservo;

            // Copter Settings
            NUM_copter_delay.Value = griddata.copter_delay;
            //CHK_copter_headinghold.Checked = griddata.copter_headinghold_chk; //UNcomment after adding headinghold offset function

            // Plane Settings
            NUM_Lane_Dist.Value = griddata.minlaneseparation;
        }

        GridData savegriddata()
        {
            GridData griddata = new GridData();

            griddata.poly = list;

            griddata.camera = CMB_camera.Text;
            griddata.alt = NUM_altitude.Value;
            griddata.angle = NUM_angle.Value;
            griddata.camdir = CHK_camdirection.Checked;

            griddata.usespeed = CHK_usespeed.Checked;


            griddata.dist = NUM_Distance.Value;
            griddata.overshoot1 = NUM_overshoot.Value;
            griddata.overshoot2 = NUM_overshoot2.Value;
            griddata.overlap = num_overlap.Value;
            griddata.sidelap = num_sidelap.Value;
            griddata.spacing = NUM_spacing.Value;

            griddata.startfrom = CMB_startfrom.Text;

            griddata.autotakeoff = CHK_toandland.Checked;
            griddata.autotakeoff_RTL = CHK_toandland_RTL.Checked;

            griddata.internals = CHK_internals.Checked;
            griddata.footprints = CHK_footprints.Checked;
            griddata.advanced = CHK_advanced.Checked;

            griddata.trigdist = rad_trigdist.Checked;
            griddata.digicam = rad_digicam.Checked;
            griddata.repeatservo = rad_repeatservo.Checked;

            // Copter Settings
            griddata.copter_delay = NUM_copter_delay.Value;
            griddata.copter_headinghold_chk = CHK_copter_headinghold.Checked;
            griddata.copter_headinghold = NUM_spacing.Value;

            // Plane Settings
            griddata.minlaneseparation = NUM_Lane_Dist.Value;

            return griddata;
        }

        void loadsettings()
        {
            if (plugin.Host.config.ContainsKey("grid_camera"))
            {

                loadsetting("grid_alt", NUM_altitude);
                //  loadsetting("grid_angle", NUM_angle);
                loadsetting("grid_camdir", CHK_camdirection);

                loadsetting("grid_usespeed", CHK_usespeed);

                loadsetting("grid_dist", NUM_Distance);
                loadsetting("grid_overshoot1", NUM_overshoot);
                loadsetting("grid_overshoot2", NUM_overshoot2);
                loadsetting("grid_overlap", num_overlap);
                loadsetting("grid_sidelap", num_sidelap);
                loadsetting("grid_spacing", NUM_spacing);

                loadsetting("grid_startfrom", CMB_startfrom);

                loadsetting("grid_autotakeoff", CHK_toandland);
                loadsetting("grid_autotakeoff_RTL", CHK_toandland_RTL);

                loadsetting("grid_internals", CHK_internals);
                loadsetting("grid_footprints", CHK_footprints);
                loadsetting("grid_advanced", CHK_advanced);

                // Should probably be saved as one setting, and us logic
                loadsetting("grid_trigdist", rad_trigdist);
                loadsetting("grid_digicam", rad_digicam);
                loadsetting("grid_repeatservo", rad_repeatservo);

                // camera last to it invokes a reload
                loadsetting("grid_camera", CMB_camera);

                // Copter Settings
                loadsetting("grid_copter_delay", NUM_copter_delay);
                //loadsetting("grid_copter_headinghold_chk", CHK_copter_headinghold);

                // Plane Settings
                loadsetting("grid_min_lane_separation", NUM_Lane_Dist);
            }
        }

        void loadsetting(string key, Control item)
        {
            // soft fail on bad param
            try
            {
                if (plugin.Host.config.ContainsKey(key))
                {
                    if (item is NumericUpDown)
                    {
                        ((NumericUpDown)item).Value = decimal.Parse(plugin.Host.config[key].ToString());
                    }
                    else if (item is ComboBox)
                    {
                        ((ComboBox)item).Text = plugin.Host.config[key].ToString();
                    }
                    else if (item is CheckBox)
                    {
                        ((CheckBox)item).Checked = bool.Parse(plugin.Host.config[key].ToString());
                    }
                    else if (item is RadioButton)
                    {
                        ((RadioButton)item).Checked = bool.Parse(plugin.Host.config[key].ToString());
                    }
                }
            }
            catch { }
        }

        void savesettings()
        {
            plugin.Host.config["grid_camera"] = CMB_camera.Text;
            plugin.Host.config["grid_alt"] = NUM_altitude.Value.ToString();
            plugin.Host.config["grid_angle"] = NUM_angle.Value.ToString();
            plugin.Host.config["grid_camdir"] = CHK_camdirection.Checked.ToString();

            plugin.Host.config["grid_usespeed"] = CHK_usespeed.Checked.ToString();

            plugin.Host.config["grid_dist"] = NUM_Distance.Value.ToString();
            plugin.Host.config["grid_overshoot1"] = NUM_overshoot.Value.ToString();
            plugin.Host.config["grid_overshoot2"] = NUM_overshoot2.Value.ToString();
            plugin.Host.config["grid_overlap"] = num_overlap.Value.ToString();
            plugin.Host.config["grid_sidelap"] = num_sidelap.Value.ToString();
            plugin.Host.config["grid_spacing"] = NUM_spacing.Value.ToString();

            plugin.Host.config["grid_startfrom"] = CMB_startfrom.Text;

            plugin.Host.config["grid_autotakeoff"] = CHK_toandland.Checked.ToString();
            plugin.Host.config["grid_autotakeoff_RTL"] = CHK_toandland_RTL.Checked.ToString();

            plugin.Host.config["grid_internals"] = CHK_internals.Checked.ToString();
            plugin.Host.config["grid_footprints"] = CHK_footprints.Checked.ToString();
            plugin.Host.config["grid_advanced"] = CHK_advanced.Checked.ToString();

            plugin.Host.config["grid_trigdist"] = rad_trigdist.Checked.ToString();
            plugin.Host.config["grid_digicam"] = rad_digicam.Checked.ToString();
            plugin.Host.config["grid_repeatservo"] = rad_repeatservo.Checked.ToString();

            // Copter Settings
            plugin.Host.config["grid_copter_delay"] = NUM_copter_delay.Value.ToString();
            plugin.Host.config["grid_copter_headinghold_chk"] = CHK_copter_headinghold.Checked.ToString();

            // Plane Settings
            plugin.Host.config["grid_min_lane_separation"] = NUM_Lane_Dist.Value.ToString();
        }

        private void xmlcamera(bool write, string filename = "cameras.xml")
        {
            bool exists = File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + filename);

            if (write || !exists)
            {
                try
                {
                    XmlTextWriter xmlwriter = new XmlTextWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + filename, Encoding.ASCII);
                    xmlwriter.Formatting = Formatting.Indented;

                    xmlwriter.WriteStartDocument();

                    xmlwriter.WriteStartElement("Cameras");

                    foreach (string key in cameras.Keys)
                    {
                        try
                        {
                            if (key == "")
                                continue;
                            xmlwriter.WriteStartElement("Camera");
                            xmlwriter.WriteElementString("name", cameras[key].name);
                            xmlwriter.WriteElementString("flen", cameras[key].focallen.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("imgh", cameras[key].imageheight.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("imgw", cameras[key].imagewidth.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("senh", cameras[key].sensorheight.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("senw", cameras[key].sensorwidth.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteEndElement();
                        }
                        catch { }
                    }

                    xmlwriter.WriteEndElement();

                    xmlwriter.WriteEndDocument();
                    xmlwriter.Close();

                }
                catch (Exception ex) { CustomMessageBox.Show(ex.ToString()); }
            }
            else
            {
                try
                {
                    using (XmlTextReader xmlreader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + filename))
                    {
                        while (xmlreader.Read())
                        {
                            xmlreader.MoveToElement();
                            try
                            {
                                switch (xmlreader.Name)
                                {
                                    case "Camera":
                                        {
                                            camerainfo camera = new camerainfo();

                                            while (xmlreader.Read())
                                            {
                                                bool dobreak = false;
                                                xmlreader.MoveToElement();
                                                switch (xmlreader.Name)
                                                {
                                                    case "name":
                                                        camera.name = xmlreader.ReadString();
                                                        break;
                                                    case "imgw":
                                                        camera.imagewidth = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "imgh":
                                                        camera.imageheight = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "senw":
                                                        camera.sensorwidth = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "senh":
                                                        camera.sensorheight = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "flen":
                                                        camera.focallen = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "Camera":
                                                        cameras[camera.name] = camera;
                                                        dobreak = true;
                                                        break;
                                                }
                                                if (dobreak)
                                                    break;
                                            }
                                            string temp = xmlreader.ReadString();
                                        }
                                        break;
                                    case "Config":
                                        break;
                                    case "xml":
                                        break;
                                    default:
                                        if (xmlreader.Name == "") // line feeds
                                            break;
                                        //config[xmlreader.Name] = xmlreader.ReadString();
                                        break;
                                }
                            }
                            catch (Exception ee) { Console.WriteLine(ee.Message); } // silent fail on bad entry
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Bad Camera File: " + ex.ToString()); } // bad config file

                // populate list
                foreach (var camera in cameras.Values)
                {
                    if (!CMB_camera.Items.Contains(camera.name))
                        CMB_camera.Items.Add(camera.name);
                }
            }
        }

        CurrentState c = new MissionPlanner.CurrentState(); //reference or instance so that HomeBase is actually home now, not a WP
        // Do Work
        private void domainUpDown1_ValueChanged(object sender, EventArgs e)
        {
            myGrid.Clear();         //reset the elements in myGrid because it'll be a new version since this function was called again

            if (CHK_usespeed.Checked)    //if the use speed box is checked
            {         //if user unchecks the box, the same speed that was previously there will be used, not the default 5 m/s
                speed = (double)NUM_UpDownFlySpeed.Value;
                maxFlightDist = ((double)maxFlight.Value * 60) * (speed / 1000);
            }

            if (CMB_camera.Text != "")
                    doCalc();

            // new grid system test

            grid = Grid.CreateGrid(list, CurrentState.fromDistDisplayUnit((double)NUM_altitude.Value), 
                (double)NUM_Distance.Value, (double)NUM_spacing.Value, (double)NUM_angle.Value, 
                (double)NUM_overshoot.Value, (double)NUM_overshoot2.Value, 
                (Grid.StartPosition)Enum.Parse(typeof(Grid.StartPosition), CMB_startfrom.Text), 
                false, (float)NUM_Lane_Dist.Value, (float)NUM_leadin.Value);

            List<PointLatLng> list2 = new List<PointLatLng>(); //not really sure what this number is because
                                                               //doesn't seem like the numberflight path points
            grid.ForEach(x => { list2.Add(x); });              //adds the number of WPs to list two

            map.HoldInvalidation = true;
            //below is clearing everything on the map I think
            routesOverlay.Routes.Clear();
            routesOverlay.Polygons.Clear();
            routesOverlay.Markers.Clear();

            if (grid.Count == 0)    //if there is nothing to plot on the map
            {
                return;
            }

            if (CHK_boundary.Checked)
                AddDrawPolygon();

            int strips = 0;
            int images = 0;
            int a = 1;
            PointLatLngAlt prevpoint = grid[0];
            float routetotal = 0;
            float renewedRouteTotal = 0;                                //I added this temp variable
            List<PointLatLng> segment = new List<PointLatLng>();        //list of the WPs to add to the map
            List<PointLatLng> segmentHome = new List<PointLatLng>();    //list of the WP from end of segment back to home
            List<PointLatLng> segmentNext = new List<PointLatLng>();    //list of the WP for the next segment
            GMapRoute seg;      //I added this
            GMapRoute segHome;  //I added this too as a test variable to get dist from current point to home
            GMapRoute segNext;  //I added this too as a test variable to get dist for the next segment
            PointLatLngAlt homeBaseBegin = c.HomeLocation;   // grid[0];    //stores the very first point       && I added this
            c.HomeLocation.Tag = "Home";                     //creating the home tag
            int backHomeLines = 0;      //going to be used for debugging
            double distance1 = 0;

            //I added so that I could see the home base in the beginning
            var marker = new GMapMarkerWP(homeBaseBegin, "H") { ToolTipText = a.ToString(), ToolTipMode = MarkerTooltipMode.OnMouseOver };
            routesOverlay.Markers.Add(marker);  //adds a H marker for home

            for (int i = 0; i < grid.Count; i++)
            {
                if (grid[i].Tag == "M")         //I think that this if statement just places down all of the markers we use for the segments
                {
                    images++;

                    if (CHK_internals.Checked)
                    {
                        routesOverlay.Markers.Add(new GMarkerGoogle(grid[i], GMarkerGoogleType.green) { ToolTipText = a.ToString(), ToolTipMode = MarkerTooltipMode.OnMouseOver });
                        a++;

                        segment.Add(prevpoint);
                        segment.Add(grid[i]);
                        prevpoint = grid[i];
                    }
                    try
                    {
                        if (TXT_fovH.Text != "")
                        {
                            double fovh = double.Parse(TXT_fovH.Text);
                            double fovv = double.Parse(TXT_fovV.Text);

                            double startangle = 0;

                            if (!CHK_camdirection.Checked)
                            {
                                startangle = 90;
                            }

                            double angle1 = startangle - (Math.Tan((fovv / 2.0) / (fovh / 2.0)) * rad2deg);
                            double dist1 = Math.Sqrt(Math.Pow(fovh / 2.0, 2) + Math.Pow(fovv / 2.0, 2));

                            double bearing = (double)NUM_angle.Value;// (prevpoint.GetBearing(item) + 360.0) % 360;

                            List<PointLatLng> footprint = new List<PointLatLng>();
                            footprint.Add(grid[i].newpos(bearing + angle1, dist1));
                            footprint.Add(grid[i].newpos(bearing + 180 - angle1, dist1));
                            footprint.Add(grid[i].newpos(bearing + 180 + angle1, dist1));
                            footprint.Add(grid[i].newpos(bearing - angle1, dist1));

                            GMapPolygon poly = new GMapPolygon(footprint, i.ToString());
                            poly.Stroke = new Pen(Color.FromArgb(250 - ((a * 5) % 240), 250 - ((a * 3) % 240), 250 - ((a * 9) % 240)), 1);
                            poly.Fill = new SolidBrush(Color.FromArgb(40, Color.Purple));
                            if (CHK_footprints.Checked)
                                routesOverlay.Polygons.Add(poly);
                        }
                    }
                    catch { }
                }
                else
                {   
                    strips++;                                   //number of line strips to connect the WPs
                    if (CHK_markers.Checked)
                    {
                        marker = new GMapMarkerWP(grid[i], a.ToString()) { ToolTipText = a.ToString(), ToolTipMode = MarkerTooltipMode.OnMouseOver };
                        routesOverlay.Markers.Add(marker);      //adding new WPs to the map to fill the Polygon Points in
                    }
                    
                    //below is only run when the drone needs to go back home
                    if (distance1 >= maxFlightDist)  //should be updated when the Max Flight Dist NumUpDown is changed
                    {   //need to check if the drone can make it to both the next WP and back home before the routetotal is 10 hectares
                        createMyGridList(i);        //creates a new list for me to manipulate only when need to return home for recharge

                        backHomeLines++;            //increment every time drone goes back home
                        segment.Add(prevpoint);     //whatever beginning point the drone was at previously
                        segment.Add(homeBaseBegin); //telling the drone to go back to home
                        renewedRouteTotal = 0;      //reset this variable

                        //making the segment for this point to home segment
                        seg = new GMapRoute(segment, "segment" + a.ToString());     //I modified this from the above line && commented out
                        seg.Stroke = new Pen(Color.Yellow, 4);
                        seg.Stroke.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                        seg.IsHitTestVisible = true;
                        if (CHK_grid.Checked)
                            routesOverlay.Routes.Add(seg);
                        routetotal = routetotal + (float)seg.Distance;      //routetotal is the summation of the total flight distance
                        segment.Clear();

                        //point going from home back to the point we left off
                        segment.Add(homeBaseBegin);
                        segment.Add(prevpoint);       //grid[i] should be at the same WP point right now
                        seg = new GMapRoute(segment, "segment" + a.ToString());
                        seg.Stroke = new Pen(Color.Yellow, 4);
                        seg.Stroke.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                        seg.IsHitTestVisible = true;
                        if (CHK_grid.Checked)
                            routesOverlay.Routes.Add(seg);
                        routetotal = routetotal + (float)seg.Distance;
                        renewedRouteTotal = renewedRouteTotal + (float)seg.Distance;    //stores the distance from home to point

                        segment.Clear();
                     }

                     segment.Add(prevpoint);     //adding the beginning points of the line segment to output onto the map
                     segment.Add(grid[i]);       //ending point of line segment
                     prevpoint = grid[i];        //new beginning point for next time around the loop       moved below at the end
                     a++;
                }
                seg = new GMapRoute(segment, "segment" + a.ToString());     //I modified this from the above line && commented out
                seg.Stroke = new Pen(Color.Navy, 4);                        //changed base segment color
                seg.Stroke.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                seg.IsHitTestVisible = true;
                if (CHK_grid.Checked)
                    routesOverlay.Routes.Add(seg);
                routetotal = routetotal + (float)seg.Distance;      //routetotal is the summation of the total flight distance
                renewedRouteTotal = renewedRouteTotal + (float)seg.Distance; //temporary routetotal to clear upon landing
                segment.Clear();

                if (i + 1 < grid.Count)
                {
                    segmentHome.Add(grid[i + 1]);     //this should be the new segment from the next point to home
                }
                if (i + 1 > grid.Count)
                {
                    segmentHome.Add(grid[grid.Count - 2]);  //last point in the grid
                }
                if (i + 1 == grid.Count)
                {
                    segmentHome.Add(grid[grid.Count - 1]);  //last point in the grid
                }
                segmentHome.Add(homeBaseBegin);       //used to calculate the distance from point to home below
                segHome = new GMapRoute(segmentHome, "segment home" + a.ToString());

                if (i + 1 < grid.Count)
                {
                    segmentNext.Add(prevpoint);       //used to calculate the distance from point to home below
                    segmentNext.Add(grid[i + 1]);     //this should be the new segment from point to home
                }
                else
                {
                    segmentNext.Add(prevpoint);     //two of the same points should give
                    segmentNext.Add(prevpoint);     //a distance of 0 since there is no next point left
                }
                segNext = new GMapRoute(segmentNext, "segment next" + a.ToString());

                distance1 = renewedRouteTotal + segNext.Distance + segHome.Distance;    //need to get seg.Distance of next segment
                segmentHome.Clear();
                segmentNext.Clear();
            }

            double numShouldReturnHome = routetotal / maxFlightDist;

            /*      Old way of drawing route, incase something breaks using segments
            GMapRoute wproute = new GMapRoute(list2, "GridRoute");
            wproute.Stroke = new Pen(Color.Yellow, 4);
            if (chk_grid.Checked)
                routesOverlay.Routes.Add(wproute);
            */

            // turn radrad = tas^2 / (tan(angle) * G)
            float v_sq = (float)(NUM_UpDownFlySpeed.Value * NUM_UpDownFlySpeed.Value);
            float turnrad = (float)(v_sq / (float)(9.808f * Math.Tan(35 * deg2rad)));

            // Update Stats 
            if (DistUnits == "Feet")
            {
                // Area
                float area = (float)calcpolygonarea(list) * 10.7639f; // Calculate the area in square feet
                lbl_area.Text = area.ToString("#") + " ft^2";
                if (area < 21780f)
                {
                    lbl_area.Text = area.ToString("#") + " ft^2";
                }
                else
                {
                    area = area / 43560f;
                    if (area < 640f)
                    {
                        lbl_area.Text = area.ToString("0.##") + " acres";
                    }
                    else
                    {
                        area = area / 640f;
                        lbl_area.Text = area.ToString("0.##") + " miles^2";
                    }
                }

                // Distance
                float distance = routetotal * 3280.84f; // Calculate the distance in feet
                if (distance < 5280f)
                {
                    lbl_distance.Text = distance.ToString("#") + " ft";
                }
                else
                {
                    distance = distance / 5280f;
                    lbl_distance.Text = distance.ToString("0.##") + " miles";
                }

                lbl_spacing.Text = (NUM_spacing.Value * 3.2808399m).ToString("#") + " ft";
                lbl_grndres.Text = inchpixel;
                lbl_distbetweenlines.Text = (NUM_Distance.Value * 3.2808399m).ToString("0.##") + " ft";
                lbl_footprint.Text = feet_fovH + " x " + feet_fovV + " ft";
                lbl_turnrad.Text = (turnrad * 2 * 3.2808399).ToString("0") + " ft";
            }
            else
            {
                // Meters
                lbl_area.Text = calcpolygonarea(list).ToString("#") + " m^2";
                lbl_distance.Text = routetotal.ToString("0.##") + " km";
                lbl_spacing.Text = NUM_spacing.Value.ToString("#") + " m";
                lbl_grndres.Text = TXT_cmpixel.Text;
                lbl_distbetweenlines.Text = NUM_Distance.Value.ToString("0.##") + " m";
                lbl_footprint.Text = TXT_fovH.Text + " x " + TXT_fovV.Text + " m";
                lbl_turnrad.Text = (turnrad * 2).ToString("0") + " m";
                lbl_recharge.Text = backHomeLines.ToString();
                lbl_shouldrecharge.Text = numShouldReturnHome.ToString();

                if ((int)numShouldReturnHome > backHomeLines)
                {
                    CustomMessageBox.Show("Error! Bad grid! Need more flights back home. Drone will not complete mission. Move home base closer to survey area.");
                }
            }

            double flyspeedms = CurrentState.fromSpeedDisplayUnit(speed);

            lbl_pictures.Text = images.ToString();
            lbl_strips.Text = ((int)(strips / 2)).ToString();
            double seconds = ((routetotal * 1000.0) / ((flyspeedms) * 0.8));
            // reduce flying speed by 20 %
            lbl_flighttime.Text = secondsToNice(seconds);
            seconds = ((routetotal * 1000.0) / (flyspeedms));
            lbl_photoevery.Text = secondsToNice(((double)NUM_spacing.Value / flyspeedms));
            map.HoldInvalidation = false;
            if (!isMouseDown)
                map.ZoomAndCenterMarkers("routes");

            CalcHeadingHold();

            completeMyGridList();       //copies the remaining WPs over
        }

        //copies over the WPs from grid to myGrid so that I can manipulate the WPs
        int k = 0;
        int l = 0;
        private void createMyGridList(int i)     //I is the next WP to add, prevpoint was the one that was just placed down
        {
            for (l = k; l < i; l++)      //stores from the beginning of the list to the prevpoint
            {
                myGrid.Add(grid[l]);     //trying a new way to write the line above
            }
            PointLatLngAlt temp = new PointLatLngAlt(grid[l - 1]);  //used as a copy of the first point in point - home - point sequence
            temp.Tag = "duplicate WP";
            myGrid.Add(c.HomeLocation);
            myGrid.Add(temp);       //this stores the same WP from before (point - home - point sandwhich)      && grid[l - 1]
            k = i;
        }

        void completeMyGridList()
        {
            for (l = k; l < grid.Count; l++) //stores from the beginning of the list to the prevpoint
            {
                myGrid.Add(grid[l]);     //trying a new way to write the line above
            }
            l = 0;      //reset global for reuse
            k = 0;      //reset global for reuse
        }

        //puts MyGrid in to grid so that the WP list on the screen after the accept button matches the one on the Survey (Grid) screen
        private void changeGridToMyGrid(){
            int k = 0;
            for (k = 0; k < grid.Count; k++)
            {
                grid[k] = myGrid[k];        //copy array over
            }
            for (int j = k; j < myGrid.Count; j++)
            {
                grid.Add(myGrid[j]);        //add the extra elements
            }
        }

        private void AddWP(double Lng, double Lat, double Alt)
        {
            if (CHK_copter_headinghold.Checked)
            {
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.CONDITION_YAW, Convert.ToInt32(TXT_headinghold.Text), 0, 0, 0, 0, 0, 0);
            }

            if (NUM_copter_delay.Value > 0)
            {
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.WAYPOINT, (double)NUM_copter_delay.Value, 0, 0, 0, Lng, Lat, Alt * CurrentState.multiplierdist);        //altitude stays the same in this case during the delay
            }
            else
            {
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, Lng, Lat, (int)(Alt * CurrentState.multiplierdist));
            }
        }

        string secondsToNice(double seconds)
        {
            if (seconds < 0)
                return "Infinity Seconds";

            double secs = seconds % 60;
            int mins = (int)(seconds / 60) % 60;
            int hours = (int)(seconds / 3600) % 24;

            if (hours > 0)
            {
                return hours + ":" + mins.ToString("00") + ":" + secs.ToString("00") + " Hours";
            }
            else if (mins > 0)
            {
                return mins + ":" + secs.ToString("00") + " Minutes";
            }
            else
            {
                return secs.ToString("0.00") + " Seconds";
            }
        }

        void AddDrawPolygon()
        {
            List<PointLatLng> list2 = new List<PointLatLng>();

            list.ForEach(x => { list2.Add(x); });

            var poly = new GMapPolygon(list2, "poly");
            poly.Stroke = new Pen(Color.Red, 2);
            poly.Fill = Brushes.Transparent;

            routesOverlay.Polygons.Add(poly);

            foreach (var item in list)
            {
                routesOverlay.Markers.Add(new GMarkerGoogle(item, GMarkerGoogleType.red));
            }
        }

        double calcpolygonarea(List<PointLatLngAlt> polygon)
        {
            // should be a closed polygon
            // coords are in lat long
            // need utm to calc area

            if (polygon.Count == 0)
            {
                CustomMessageBox.Show("Please define a polygon!");
                return 0;
            }

            // close the polygon
            if (polygon[0] != polygon[polygon.Count - 1])
                polygon.Add(polygon[0]); // make a full loop

            CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();

            GeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

            int utmzone = (int)((polygon[0].Lng - -186.0) / 6.0);

            IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmzone, polygon[0].Lat < 0 ? false : true);

            ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

            double prod1 = 0;
            double prod2 = 0;

            for (int a = 0; a < (polygon.Count - 1); a++)
            {
                double[] pll1 = { polygon[a].Lng, polygon[a].Lat };
                double[] pll2 = { polygon[a + 1].Lng, polygon[a + 1].Lat };

                double[] p1 = trans.MathTransform.Transform(pll1);
                double[] p2 = trans.MathTransform.Transform(pll2);

                prod1 += p1[0] * p2[1];
                prod2 += p1[1] * p2[0];
            }

            double answer = (prod1 - prod2) / 2;

            if (polygon[0] == polygon[polygon.Count - 1])
                polygon.RemoveAt(polygon.Count - 1); // unmake a full loop

            return Math.Abs(answer);
        }

        double getAngleOfLongestSide(List<PointLatLngAlt> list)
        {
            if (list.Count == 0)
                return 0;
            double angle = 0;
            double maxdist = 0;
            PointLatLngAlt last = list[list.Count - 1];
            foreach (var item in list)
            {
                if (item.GetDistance(last) > maxdist)
                {
                    angle = item.GetBearing(last);
                    maxdist = item.GetDistance(last);
                }
                last = item;
            }

            return (angle + 360) % 360;
        }

        void doCalc()
        {
            try
            {
                // entered values
                float focallen = (float)NUM_focallength.Value;
                float flyalt = (float)NUM_altitude.Value;
                int imagewidth = int.Parse(TXT_imgwidth.Text);
                int imageheight = int.Parse(TXT_imgheight.Text);

                float sensorwidth = float.Parse(TXT_senswidth.Text);
                float sensorheight = float.Parse(TXT_sensheight.Text);

                int overlap = (int)num_overlap.Value;
                int sidelap = (int)num_sidelap.Value;


                // scale      mm / mm
                float flscale = (1000 * flyalt) / focallen;

                //   mm * mm / 1000
                float viewwidth = (sensorwidth * flscale / 1000);
                float viewheight = (sensorheight * flscale / 1000);

                TXT_fovH.Text = viewwidth.ToString("#.#");
                TXT_fovV.Text = viewheight.ToString("#.#");
                // Imperial
                feet_fovH = (viewwidth * 3.2808399f).ToString("#.#");
                feet_fovV = (viewheight * 3.2808399f).ToString("#.#");

                float fovh = (float)(Math.Atan(sensorwidth / (2 * focallen)) * rad2deg * 2);
                float fovv = (float)(Math.Atan(sensorheight / (2 * focallen)) * rad2deg * 2);

                //    mm  / pixels * 100
                TXT_cmpixel.Text = ((viewheight / imageheight) * 100).ToString("0.00 cm");
                // Imperial
                inchpixel = (((viewheight / imageheight) * 100) * 0.393701).ToString("0.00 inches");

                if (CHK_camdirection.Checked)
                {
                    NUM_spacing.Value = (decimal)((1 - (overlap / 100.0f)) * viewheight);
                    NUM_Distance.Value = (decimal)((1 - (sidelap / 100.0f)) * viewwidth);
                }
                else
                {
                    NUM_spacing.Value = (decimal)((1 - (overlap / 100.0f)) * viewwidth);
                    NUM_Distance.Value = (decimal)((1 - (sidelap / 100.0f)) * viewheight);
                }

            }
            catch { return; }
        }

        private void CalcHeadingHold()
        {
            int previous = (int)Math.Round(Convert.ToDecimal(((UpDownBase)NUM_angle).Text)); //((UpDownBase)sender).Text
            int current = (int)Math.Round(NUM_angle.Value);

            int change = current - previous;
            
            if (change > 0) // Positive change
            {
                int val = Convert.ToInt32(TXT_headinghold.Text) + change;
                if (val > 359) 
                {
                    val = val - 360;
                }
                TXT_headinghold.Text = val.ToString();
            }

            if (change < 0) // Negative change
            {
                int val = Convert.ToInt32(TXT_headinghold.Text) + change;
                if (val < 0)
                {
                    val = val + 360;
                }
                TXT_headinghold.Text = val.ToString();
            }
        }

        // Map Operators
        private void map_OnRouteEnter(GMapRoute item)
        {
            string dist;
            if (DistUnits == "Feet")
            {
                dist = ((float)item.Distance * 3280.84f).ToString("0.##") + " ft";
            }
            else
            {
                dist = ((float)item.Distance * 1000f).ToString("0.##") + " m";
            }
            if (marker != null)
            {
                if (routesOverlay.Markers.Contains(marker))
                    routesOverlay.Markers.Remove(marker);
            }

            PointLatLng point = currentMousePosition;

            marker = new GMapMarkerRect(point);
            marker.ToolTip = new GMapToolTip(marker);
            marker.ToolTipMode = MarkerTooltipMode.Always;
            marker.ToolTipText = "Line: " + dist;
            routesOverlay.Markers.Add(marker);
        }

        private void map_OnRouteLeave(GMapRoute item)
        {
            if (marker != null)
            {
                try
                {
                    if (routesOverlay.Markers.Contains(marker))
                        routesOverlay.Markers.Remove(marker);
                }
                catch { }
            }
        }

        private void map_OnMarkerLeave(GMapMarker item)
        {
            if (!isMouseDown)
            {
                if (item is GMapMarker)
                {
                    // when you click the context menu this triggers and causes problems
                    CurrentGMapMarker = null;
                }

            }
        }

        private void map_OnMarkerEnter(GMapMarker item)
        {
            if (!isMouseDown)
            {
                if (item is GMapMarker)
                {
                    CurrentGMapMarker = item as GMapMarker;
                    CurrentGMapMarkerStartPos = CurrentGMapMarker.Position;
                }
            }
        }

        private void map_MouseUp(object sender, MouseEventArgs e)
        {
            MouseDownEnd = map.FromLocalToLatLng(e.X, e.Y);

            // Console.WriteLine("MainMap MU");

            if (e.Button == MouseButtons.Right) // ignore right clicks
            {
                return;
            }

            if (isMouseDown) // mouse down on some other object and dragged to here.
            {
                if (e.Button == MouseButtons.Left)
                {
                    isMouseDown = false;
                }
                if (!isMouseDraging)
                {
                    if (CurrentGMapMarker != null)
                    {
                        // Redraw polygon
                        //AddDrawPolygon();
                    }
                }
            }
            isMouseDraging = false;
            CurrentGMapMarker = null;
            CurrentGMapMarkerIndex = 0;
            CurrentGMapMarkerStartPos = null;
        }

        private void map_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownStart = map.FromLocalToLatLng(e.X, e.Y);

            if (e.Button == MouseButtons.Left && Control.ModifierKeys != Keys.Alt)
            {
                isMouseDown = true;
                isMouseDraging = false;

                if (CurrentGMapMarkerStartPos != null)
                    CurrentGMapMarkerIndex = list.FindIndex(c => c.ToString() == CurrentGMapMarkerStartPos.ToString());
            }
        }

        private void map_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng point = map.FromLocalToLatLng(e.X, e.Y);
            currentMousePosition = point;

            if (MouseDownStart == point)
                return;

            if (!isMouseDown)
            {
                // update mouse pos display
                //SetMouseDisplay(point.Lat, point.Lng, 0);
            }

            //draging
            if (e.Button == MouseButtons.Left && isMouseDown)
            {
                isMouseDraging = true;

                if (CurrentGMapMarker != null)
                {
                    PointLatLng pnew = map.FromLocalToLatLng(e.X, e.Y);

                    CurrentGMapMarker.Position = pnew;

                    list[CurrentGMapMarkerIndex] = new PointLatLngAlt(pnew);
                    domainUpDown1_ValueChanged(sender, e);
                }
                else // left click pan
                {
                    double latdif = MouseDownStart.Lat - point.Lat;
                    double lngdif = MouseDownStart.Lng - point.Lng;

                    try
                    {
                        lock (thisLock)
                        {
                            map.Position = new PointLatLng(map.Position.Lat + latdif, map.Position.Lng + lngdif);
                        }
                    }
                    catch { }
                }
            }
        }

        private void map_OnMapZoomChanged()
        {
            if (map.Zoom > 0)
            {
                try
                {
                    TRK_zoom.Value = (float)map.Zoom;
                }
                catch { }
            }
        }

        // Operators
        //the scroll bar to the right of the screen that lets you zoom
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                lock (thisLock)
                {
                    map.Zoom = TRK_zoom.Value;
                }
            }
            catch { }
        }

        //zooming with your mouse to zoom into the map
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            try
            {
                lock (thisLock)
                {
                    map.Zoom = TRK_zoom.Value;
                }
            }
            catch { }
        }

        //I believe that this updates all of the data on after you AutoWP -> Survey (Grid)
        private void NUM_ValueChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void CMB_camera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cameras.ContainsKey(CMB_camera.Text))
            {
                camerainfo camera = cameras[CMB_camera.Text];

                NUM_focallength.Value = (decimal)camera.focallen;
                TXT_imgheight.Text = camera.imageheight.ToString();
                TXT_imgwidth.Text = camera.imagewidth.ToString();
                TXT_sensheight.Text = camera.sensorheight.ToString();
                TXT_senswidth.Text = camera.sensorwidth.ToString();

                NUM_Distance.Enabled = false;
            }

            doCalc();
        }

        private void TXT_TextChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void CHK_camdirection_CheckedChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void CHK_advanced_CheckedChanged(object sender, EventArgs e)
        {
            if (CHK_advanced.Checked)
            {
                tabControl1.TabPages.Add(tabGrid);
                tabControl1.TabPages.Add(tabCamera);
            }
            else
            {
                tabControl1.TabPages.Remove(tabGrid);
                tabControl1.TabPages.Remove(tabCamera);
            }
        }

        private void CHK_copter_headinghold_CheckedChanged(object sender, EventArgs e)
        {
            if (CHK_copter_headinghold.Checked)
            {
                TXT_headinghold.Enabled = true;
                CHK_copter_headingholdlock.Enabled = true;
                CHK_copter_headingholdlock.Checked = false;
                BUT_headingholdplus.Enabled = true;
                BUT_headingholdminus.Enabled = true;
            }
            else
            {
                TXT_headinghold.Enabled = false;
                CHK_copter_headingholdlock.Enabled = false;
                BUT_headingholdplus.Enabled = false;
                BUT_headingholdminus.Enabled = false;
            }
        }

        private void CHK_copter_headingholdlock_CheckedChanged(object sender, EventArgs e)
        {
            if (CHK_copter_headingholdlock.Checked)
            {
                TXT_headinghold.ReadOnly = false;
            }
            else
            {
                TXT_headinghold.ReadOnly = true;
                TXT_headinghold.Text = Decimal.Round(NUM_angle.Value).ToString();
            }
        }

        private void BUT_headingholdplus_Click(object sender, EventArgs e)
        {
            int previous = Convert.ToInt32(TXT_headinghold.Text);
            if(!CHK_copter_headingholdlock.Checked)
            {                
                if (previous + 180 > 359)
                {
                    TXT_headinghold.Text = (previous - 180).ToString();
                }
                else
                {
                    TXT_headinghold.Text = (previous + 180).ToString();
                }
            }
            else
            {
                if (previous + 1 > 359)
                {
                    TXT_headinghold.Text = (previous - 359).ToString();
                }
                else
                {
                    TXT_headinghold.Text = (previous + 1).ToString();
                }
            }
        }

        private void BUT_headingholdminus_Click(object sender, EventArgs e)
        {
            int previous = Convert.ToInt32(TXT_headinghold.Text);
            
            if (!CHK_copter_headingholdlock.Checked)
            {
                if (previous - 180 < 0)
                {
                    TXT_headinghold.Text = (previous + 180).ToString();
                }
                else
                {
                    TXT_headinghold.Text = (previous - 180).ToString();
                }
            }
            else
            {
                if (previous - 1 < 0)
                {
                    TXT_headinghold.Text = (previous + 359).ToString();
                }
                else
                {
                    TXT_headinghold.Text = (previous - 1).ToString();
                }
            }
        }

        private void BUT_samplephoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.jpg|*.jpg";

            ofd.ShowDialog();

            if (File.Exists(ofd.FileName))
            {
                string fn = ofd.FileName;

                Metadata lcMetadata = null;
                try
                {
                    FileInfo lcImgFile = new FileInfo(fn);
                    // Loading all meta data
                    lcMetadata = JpegMetadataReader.ReadMetadata(lcImgFile);
                }
                catch (JpegProcessingException ex)
                {
                    log.InfoFormat(ex.Message);
                    return;
                }

                foreach (AbstractDirectory lcDirectory in lcMetadata)
                {
                    foreach (var tag in lcDirectory)
                    {
                        Console.WriteLine(lcDirectory.GetName() + " - " + tag.GetTagName() + " " + tag.GetTagValue().ToString());
                    }

                    if (lcDirectory.ContainsTag(ExifDirectory.TAG_EXIF_IMAGE_HEIGHT))
                    {
                        TXT_imgheight.Text = lcDirectory.GetInt(ExifDirectory.TAG_EXIF_IMAGE_HEIGHT).ToString();
                    }

                    if (lcDirectory.ContainsTag(ExifDirectory.TAG_EXIF_IMAGE_WIDTH))
                    {
                        TXT_imgwidth.Text = lcDirectory.GetInt(ExifDirectory.TAG_EXIF_IMAGE_WIDTH).ToString();
                    }

                    if (lcDirectory.ContainsTag(ExifDirectory.TAG_FOCAL_PLANE_X_RES))
                    {
                        var unit = lcDirectory.GetFloat(ExifDirectory.TAG_FOCAL_PLANE_UNIT);

                        // TXT_senswidth.Text = lcDirectory.GetDouble(ExifDirectory.TAG_FOCAL_PLANE_X_RES).ToString();
                    }

                    if (lcDirectory.ContainsTag(ExifDirectory.TAG_FOCAL_PLANE_Y_RES))
                    {
                        var unit = lcDirectory.GetFloat(ExifDirectory.TAG_FOCAL_PLANE_UNIT);

                        // TXT_sensheight.Text = lcDirectory.GetDouble(ExifDirectory.TAG_FOCAL_PLANE_Y_RES).ToString();
                    }

                    if (lcDirectory.ContainsTag(ExifDirectory.TAG_FOCAL_LENGTH))
                    {
                        try
                        {
                            var item = lcDirectory.GetFloat(ExifDirectory.TAG_FOCAL_LENGTH);
                            NUM_focallength.Value = (decimal)item;
                        }
                        catch { }
                    }


                    if (lcDirectory.ContainsTag(ExifDirectory.TAG_DATETIME_ORIGINAL))
                    {

                    }

                }
            }
        }

        private void BUT_save_Click(object sender, EventArgs e)
        {
            camerainfo camera = new camerainfo();

            string camname = "Default";

            if (MissionPlanner.Controls.InputBox.Show("Camera Name", "Please and a camera name", ref camname) != System.Windows.Forms.DialogResult.OK)
                return;

            CMB_camera.Text = camname;

            // check if camera exists alreay
            if (cameras.ContainsKey(CMB_camera.Text))
            {
                camera = cameras[CMB_camera.Text];
            }
            else
            {
                cameras.Add(CMB_camera.Text, camera);
            }

            try
            {
                camera.name = CMB_camera.Text;
                camera.focallen = (float)NUM_focallength.Value;
                camera.imageheight = float.Parse(TXT_imgheight.Text);
                camera.imagewidth = float.Parse(TXT_imgwidth.Text);
                camera.sensorheight = float.Parse(TXT_sensheight.Text);
                camera.sensorwidth = float.Parse(TXT_senswidth.Text);
            }
            catch { CustomMessageBox.Show("One of your entries is not a valid number"); return; }

            cameras[CMB_camera.Text] = camera;

            xmlcamera(true);
        }

        private void BUT_Accept_Click(object sender, EventArgs e)
        {
            changeGridToMyGrid();       //reformatting grid so that it contains the return to home WPs in the middle of the program
            if (grid != null && grid.Count > 0)  //changed all grids to segments (?) - I dont remember what this comment meant
            {
                MainV2.instance.FlightPlanner.quickadd = true;

                if (CHK_toandland.Checked)      //drone takeoff command
                {
                    if (plugin.Host.cs.firmware == MainV2.Firmwares.ArduCopter2)
                    {
                        plugin.Host.AddWPtoList(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, (int)(30 * CurrentState.multiplierdist));
                    }
                    else
                    {
                        plugin.Host.AddWPtoList(MAVLink.MAV_CMD.TAKEOFF, 20, 0, 0, 0, 0, 0, (int)(30 * CurrentState.multiplierdist));
                    }
                }

                if (CHK_usespeed.Checked)       //change drone speed after takeoff if this has been checked
                {
                    plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_CHANGE_SPEED, 0, (int)NUM_UpDownFlySpeed.Value, 0, 0, 0, 0, 0);
                }

                PointLatLngAlt temp = new PointLatLngAlt();     //temp to store the previous point
                int i = 0;                  //WP grid list is produced/used here
                grid.ForEach(plla =>        //plla is pointlatlngalt
                {
                    if (i > 0)
                    {
                        if (plla.Tag == "M")        //M represents a marker
                        {
                            if (rad_repeatservo.Checked)
                            {
                                AddWP(plla.Lng, plla.Lat, plla.Alt);
                                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_REPEAT_SERVO, (float)NUM_reptservo.Value, (float)num_reptpwm.Value, 999, (float)NUM_repttime.Value, 0, 0, 0);
                            }
                            if (rad_digicam.Checked)
                            {
                                AddWP(plla.Lng, plla.Lat, plla.Alt);
                                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_DIGICAM_CONTROL, 0, 0, 0, 0, 0, 0, 0);
                            }
                        }

                        //I added the else if below
                        else if (plla.Tag == "Home"){
                            plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_SET_CAM_TRIGG_DIST, 0, 0, 0, 0, 0, 0, 0);
                            plugin.Host.AddWPtoList(MAVLink.MAV_CMD.RETURN_TO_LAUNCH, 0, 0, 0, 0, 0, 0, 0);
                            plugin.Host.AddWPtoList(MAVLink.MAV_CMD.TAKEOFF, 20, 0, 0, 0, 0, 0, (int)(30 * CurrentState.multiplierdist));
                            if (CHK_usespeed.Checked)        //change speed after drone takeoff for multiple flight paths
                            {
                                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_CHANGE_SPEED, 0, (int)NUM_UpDownFlySpeed.Value, 0, 0, 0, 0, 0);
                            }
                        }

                        else if (plla.Tag == "duplicate WP")
                        {
                            AddWP(temp.Lng, temp.Lat, temp.Alt);    //should be coordinates of the previous point
                            plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_SET_CAM_TRIGG_DIST, (float)NUM_spacing.Value, 0, 0, 0, 0, 0, 0);
                        }

                        else    //adding all of the waypoints to the initial flight path screen now
                        {
                            AddWP(plla.Lng, plla.Lat, plla.Alt);
                            temp = plla;    //store all data from previous plla to use when home happens
                        }
                    }
                    else    //add a camera trigger when the drone starts flying
                    {
                        AddWP(plla.Lng, plla.Lat, plla.Alt);
                        if (rad_trigdist.Checked)       //camera trigger on
                        {
                            plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_SET_CAM_TRIGG_DIST, (float)NUM_spacing.Value, 0, 0, 0, 0, 0, 0);
                        }
                    }
                    ++i;
                });

                if (rad_trigdist.Checked)       //camera trigger off
                {
                    plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_SET_CAM_TRIGG_DIST, 0, 0, 0, 0, 0, 0, 0);
                }

                if (CHK_usespeed.Checked)       //drone speed changer
                {
                    if (MainV2.comPort.MAV.param["WPNAV_SPEED"] != null)
                    {
                        float speed = ((float)MainV2.comPort.MAV.param["WPNAV_SPEED"]);
                        speed = speed / 100;
                        plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_CHANGE_SPEED, 0, speed, 0, 0, 0, 0, 0);
                    }
                }

                if (CHK_toandland.Checked)      //drone return to launch after mission command add
                {
                    if (CHK_toandland_RTL.Checked)
                    {
                        plugin.Host.AddWPtoList(MAVLink.MAV_CMD.RETURN_TO_LAUNCH, 0, 0, 0, 0, 0, 0, 0);
                    }
                    else
                    {
                        plugin.Host.AddWPtoList(MAVLink.MAV_CMD.LAND, 0, 0, 0, 0, plugin.Host.cs.HomeLocation.Lng, plugin.Host.cs.HomeLocation.Lat, 0);
                    }
                }

                //output everything that we just did/added to the list
                //so I need to change the lines above to insert the RETURN_TO_LAUNCH command lines
                // Redraw the polygon in FP
                plugin.Host.RedrawFPPolygon(list);

                savesettings();

                MainV2.instance.FlightPlanner.quickadd = false;

                MainV2.instance.FlightPlanner.writeKML();

                this.Close();
            }
            else    //if bad grid then no map is outputted to the screen and only the error message is shown
            {
                CustomMessageBox.Show("Bad Grid", "Error");
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.O))
            {
                LoadGrid();

                return true;
            }
            if (keyData == (Keys.Control | Keys.S))
            {
                SaveGrid();

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void NUM_Lane_Dist_ValueChanged(object sender, EventArgs e)
        {
            // doCalc
            domainUpDown1_ValueChanged(sender, e);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)   //when the max flight distance up down is changed
        {   //get value of the NumUpDown for time(*60) and speed to multiply together for Max Flight Distance
            maxFlightDist = ((double)maxFlight.Value*60) * (speed/1000);
            domainUpDown1_ValueChanged(sender, e);      //send the data to the function that makes the list of all the new WPs
        }

        double speed = 5;   //speed value so that the speed doesn't affect the flight if the box isn't checked
        private void CHK_usespeed_CheckedChanged(object sender, EventArgs e)        //for when the speed is changed
        {
            speed = (double)NUM_UpDownFlySpeed.Value;
            maxFlightDist = ((double)maxFlight.Value * 60) * (speed / 1000);
            domainUpDown1_ValueChanged(sender, e);      //send the data to the function that makes the list of all the new WPs
        }
    }
}