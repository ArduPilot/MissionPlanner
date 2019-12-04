using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace MissionPlanner
{
    public partial class GridUIv2 : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        const double rad2deg = (180 / Math.PI);
        const double deg2rad = (1.0 / rad2deg);

        GMapOverlay layerpolygons;
        GMapPolygon wppoly;
        GMapPolygon boxpoly;
        private GridPluginv2 plugin;
        List<PointLatLng> list = new List<PointLatLng>();
        List<PointLatLngAlt> grid;

        mode currentmode = mode.panmode;       

        Dictionary<string, camerainfo> cameras = new Dictionary<string, camerainfo>();

        enum mode
        {
            panmode,
            drawbox,
            editbox,
            movebox,
        }

        public struct camerainfo
        {
            public string name;
            public float focallen;
            public float sensorwidth;
            public float sensorheight;
            public float imagewidth;
            public float imageheight;
        }

        Dictionary<string, aircraftinfo> aircrafts = new Dictionary<string, aircraftinfo>();

        public struct aircraftinfo
        {
            public string name;
            public float turnrad;
            public float minalt;
            public float maxalt;
            public float minvel;
            public float maxvel;
        }

        public GridUIv2(GridPluginv2 plugin)
        {
            this.plugin = plugin;

            InitializeComponent();

            map.MapProvider = plugin.Host.FDMapType;

            map.Position = plugin.Host.FPMenuMapPosition;
            map.Zoom = 16;

            layerpolygons = new GMapOverlay( "polygons");
            map.Overlays.Add(layerpolygons);

            plugin.Host.FPDrawnPolygon.Points.ForEach(x => { list.Add(x); });

			num_overlap = 50;
			num_sidelap = 60;

            NUM_altitude.Value = 50;
			
            // set and angle that is good
            NUM_angle.Value = (decimal)((getAngleOfLongestSide(list) + 360) % 360);

        }

        void loadsettings()
        {
            if (plugin.Host.config.ContainsKey("grid_camera"))
            {
                
                loadsetting("grid_alt", NUM_altitude);

                // camera last to it invokes a reload
                loadsetting("grid_camera", CMB_camera);
            }
        }

        void loadsetting(string key, Control item)
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

        void savesettings()
        {
            plugin.Host.config["grid_camera"] = CMB_camera.Text;
            plugin.Host.config["grid_alt"] = NUM_altitude.Value.ToString();
            plugin.Host.config["grid_angle"] = NUM_angle.Value.ToString();
            plugin.Host.config["grid_autotakeoff"] = CHK_includetakeoff.Checked.ToString();
        }

        public struct GridData
        {
            public List<PointLatLng> poly;
            public string camera;
            public decimal alt;
            public decimal angle;
            public bool camdir;
            public decimal dist;
            public decimal overshoot1;
            public decimal overshoot2;
            public decimal overlap;
            public decimal sidelap;
            public decimal spacing;
            public string startfrom;
            public bool autotakeoff;
            public bool advanced;
        }

        GridData savegriddata()
        {
            GridData griddata = new GridData();

            griddata.poly = list;

            griddata.camera = CMB_camera.Text;
            griddata.alt = NUM_altitude.Value;
            griddata.angle = NUM_angle.Value;

            griddata.autotakeoff = CHK_includetakeoff.Checked;

            return griddata;
        }

        void loadgriddata(GridData griddata)
        {
            list = griddata.poly;

            CMB_camera.Text = griddata.camera;
            NUM_altitude.Value = griddata.alt;
            NUM_angle.Value = griddata.angle;

            CHK_includetakeoff.Checked = griddata.autotakeoff;
        }

        public void LoadGrid()
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(GridData));

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
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
        }

        public void SaveGrid()
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(GridData));

            var griddata = savegriddata();

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
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

        void AddDrawPolygon()
        {
            List<PointLatLng> list2 = new List<PointLatLng>();

            list.ForEach(x => { list2.Add(x); });

            var poly = new GMapPolygon(list2, "poly");
            poly.Stroke.Brush = Brushes.Red;
            poly.Stroke.Color = Color.Red;
            poly.Fill = Brushes.Transparent;

            layerpolygons.Polygons.Add(poly);
        

            foreach (var item in list)
            {
                layerpolygons.Markers.Add(new GMarkerGoogle(item,GMarkerGoogleType.red));
            }
        }

        double getAngleOfLongestSide(List<PointLatLng> list)
        {
            if (list.Count == 0)
                return 0;
            double angle = 0;
            double maxdist = 0;
            PointLatLngAlt last = list[list.Count - 1];
            foreach (PointLatLngAlt item in list)
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

        private void domainUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (CMB_camera.Text != "")
                doCalc();

            // new grid system test
            if (boxpoly == null || boxpoly.Points == null || boxpoly.Points.Count == 0)
                return;

            var newlist = new List<PointLatLngAlt>();

            boxpoly.Points.ForEach(x => { newlist.Add(x); });

            grid = Utilities.Grid.CreateGrid(newlist, (double) NUM_altitude.Value, (double) NUM_Distance,
                (double) NUM_spacing, (double) NUM_angle.Value, 0, 0, Utilities.Grid.StartPosition.Home, false, 0, 0,
                plugin.Host.cs.HomeLocation);

            List<PointLatLng> list2 = new List<PointLatLng>();

            grid.ForEach(x => { list2.Add(x); });

            map.HoldInvalidation = true;

            layerpolygons.Polygons.Clear();
            layerpolygons.Markers.Clear();

            layerpolygons.Polygons.Add(boxpoly);

            if (grid.Count == 0)
            {
                return;
            }

           // if (chk_boundary.Checked)
          //      AddDrawPolygon();

            int strips = 0;
            int images = 0;
            int a = 1;
            PointLatLngAlt prevpoint = grid[0];
            foreach (var item in grid)
            {
                if (item.Tag == "M")
                {
                    images++;

                    if (chk_internals.Checked)
                    {
                        layerpolygons.Markers.Add(new GMarkerGoogle(item, GMarkerGoogleType.green) { ToolTipText = a.ToString(), ToolTipMode = MarkerTooltipMode.OnMouseOver });
                        a++;
                    }
                    try
                    {
                        if (chk_footprints.Checked)
                        {
                            if (TXT_fovH != "")
                            {
                                double fovh = double.Parse(TXT_fovH);
                                double fovv = double.Parse(TXT_fovV);

                                double startangle = 0;

                                if (!RAD_camdirectionland.Checked)
                                {
                                    startangle = 90;
                                }

                                double angle1 = startangle - (Math.Tan((fovv/2.0)/(fovh/2.0))*rad2deg);
                                double dist1 = Math.Sqrt(Math.Pow(fovh/2.0, 2) + Math.Pow(fovv/2.0, 2));

                                double bearing = (double) NUM_angle.Value;
                                    // (prevpoint.GetBearing(item) + 360.0) % 360;

                                List<PointLatLng> footprint = new List<PointLatLng>();
                                footprint.Add(item.newpos(bearing + angle1, dist1));
                                footprint.Add(item.newpos(bearing + 180 - angle1, dist1));
                                footprint.Add(item.newpos(bearing + 180 + angle1, dist1));
                                footprint.Add(item.newpos(bearing - angle1, dist1));

                                GMapPolygon poly = new GMapPolygon(footprint, a.ToString());
                                poly.Stroke.Color = Color.FromArgb(250 - ((a*5)%240), 250 - ((a*3)%240),
                                    250 - ((a*9)%240));
                                poly.Stroke.Width = 1;
                                poly.Fill = new SolidBrush(Color.FromArgb(40, Color.Purple));

                                layerpolygons.Polygons.Add(poly);
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    strips++;
                    if (chk_markers.Checked)
                        layerpolygons.Markers.Add(new GMarkerGoogle(item, GMarkerGoogleType.green) { ToolTipText = a.ToString(), ToolTipMode = MarkerTooltipMode.Always });

                    a++;
                }
                prevpoint = item;
            }

            // add wp polygon
            wppoly = new GMapPolygon(list2, "Grid");
            wppoly.Stroke.Color = Color.Yellow;
            wppoly.Fill = Brushes.Transparent;
            wppoly.Stroke.Width = 4;
            if (chk_grid.Checked)
                layerpolygons.Polygons.Add(wppoly);

            Console.WriteLine("Poly Dist " + wppoly.Distance);

            quickViewarea.number = calcpolygonarea(list) / (1000.0 * 1000.0);

            lbl_distance.Text = wppoly.Distance.ToString("0.##") + " km";

            lbl_spacing.Text = NUM_spacing.ToString("#") + " m";

            quickViewgroundres.number = TXT_cmpixel;

            quickViewimagecount.number = images;

            lbl_strips.Text = ((int)(strips / 2)).ToString();
            lbl_distbetweenlines.Text = NUM_Distance.ToString("0.##") + " m";

            lbl_footprint.Text = TXT_fovH + " x " + TXT_fovV + " m";

            double seconds = ((wppoly.Distance * 1000.0) / ((double)numericUpDownFlySpeed.Value * 0.8));
            // reduce flying speed by 20 %
            label28.Text = secondsToNice(seconds);

            quickViewflighttime.number = seconds / 60.0;

            seconds = ((wppoly.Distance * 1000.0) / ((double)numericUpDownFlySpeed.Value));

            label32.Text = secondsToNice(((double)NUM_spacing / (double)numericUpDownFlySpeed.Value));

            map.HoldInvalidation = false;

            map.ZoomAndCenterMarkers("polygons");

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

        double calcpolygonarea(List<PointLatLng> polygon)
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

            IGeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

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

            return Math.Abs( answer);
        }

        private void BUT_Accept_Click(object sender, EventArgs e)
        {
            if (grid != null && grid.Count > 0)
            {
                MainV2.instance.FlightPlanner.quickadd = true;

                if (CHK_includetakeoff.Checked)
                {
                    if (plugin.Host.cs.firmware == Firmwares.ArduCopter2)
                    {
                        plugin.Host.AddWPtoList(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 30);
                    }
                    else
                    {
                        plugin.Host.AddWPtoList(MAVLink.MAV_CMD.TAKEOFF, 20, 0, 0, 0, 0, 0, 30);
                    }
                }

                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_SET_CAM_TRIGG_DIST, (float)NUM_spacing, 0, 0, 0, 0, 0, 0);

                grid.ForEach(plla =>
                {
                    // skip internals
                    if (plla.Tag == "M")
                    {

                    }
                    else
                    {
                        plugin.Host.AddWPtoList(MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, plla.Lng, plla.Lat, plla.Alt);
                    }
                });

                    plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_SET_CAM_TRIGG_DIST, 0, 0, 0, 0, 0, 0, 0);

                if (chk_includeland.Checked)
                {
                    plugin.Host.AddWPtoList(MAVLink.MAV_CMD.LAND, 0, 0, 0, 0, plugin.Host.cs.HomeLocation.Lng,plugin.Host.cs.HomeLocation.Lat, 0);
                }

                savesettings();

                MainV2.instance.FlightPlanner.quickadd = false;

                MainV2.instance.FlightPlanner.writeKML();

                this.Close();
            }
            else
            {
                CustomMessageBox.Show("Bad Grid", "Error");
            }
        }

        private void GridUI_Resize(object sender, EventArgs e)
        {
            map.ZoomAndCenterMarkers("polygons");
        }

        private void num_ValueChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void xmlcamera(bool write, string filename)
        {
            bool exists = File.Exists(filename);

            if (write || !exists)
            {
                try
                {
                    XmlTextWriter xmlwriter = new XmlTextWriter(filename, Encoding.ASCII);
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
                    using (XmlTextReader xmlreader = new XmlTextReader(filename))
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

        private void xmlaircraft(string filename = "aircraft.xml")
        {
            try
            {
                using (XmlTextReader xmlreader = new XmlTextReader(filename))
                {
                    while (xmlreader.Read())
                    {
                        xmlreader.MoveToElement();
                        try
                        {
                            switch (xmlreader.Name)
                            {
                                case "Vehicle":
                                    {
                                        aircraftinfo aircraft = new aircraftinfo();

                                        while (xmlreader.Read())
                                        {
                                            bool dobreak = false;
                                            xmlreader.MoveToElement();
                                            switch (xmlreader.Name)
                                            {
                                                case "name":
                                                    aircraft.name = xmlreader.ReadString();
                                                    break;
                                                case "turn_rad_m":
                                                    aircraft.turnrad = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                    break;
                                                case "min_alt_m":
                                                    aircraft.minalt = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                    break;
                                                case "max_alt_m":
                                                    aircraft.maxalt = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                    break;
                                                case "min_vel_ms":
                                                    aircraft.minvel = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                    break;
                                                case "max_vel_ms":
                                                    aircraft.maxvel = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                    break;
                                                case "Vehicle":
                                                    aircrafts[aircraft.name] = aircraft;
                                                    dobreak = true;
                                                    break;
                                            }
                                            if (dobreak)
                                                break;
                                        }
                                        string temp = xmlreader.ReadString();
                                    }
                                    break;
                                case "xml":
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception ee) { Console.WriteLine(ee.Message); } // silent fail on bad entry
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Bad Aircraft File: " + ex.ToString()); } // bad config file

            // populate list
            foreach (var aircraft in aircrafts.Values)
            {
                if (!CMB_aircraft.Items.Contains(aircraft.name))
                    CMB_aircraft.Items.Add(aircraft.name);
            }
        }

        void doCalc()
        {
            try
            {
                // entered values
                float focallen = (float)num_focallength;
                float flyalt = (float)NUM_altitude.Value;
                int imagewidth = int.Parse(TXT_imgwidth);
                int imageheight = int.Parse(TXT_imgheight);

                float sensorwidth = float.Parse(TXT_senswidth);
                float sensorheight = float.Parse(TXT_sensheight);

                int overlap = (int)num_overlap;
                int sidelap = (int)num_sidelap;


                // scale      mm / mm
                float flscale = (1000 * flyalt) / focallen;

                //   mm * mm / 1000
                float viewwidth = (sensorwidth * flscale / 1000);
                float viewheight = (sensorheight * flscale / 1000);

                TXT_fovH = viewwidth.ToString("#.#");
                TXT_fovV = viewheight.ToString("#.#");

                float fovh = (float)(Math.Atan(sensorwidth / (2 * focallen)) * rad2deg * 2);
                float fovv = (float)(Math.Atan(sensorheight / (2 * focallen)) * rad2deg * 2);

                //    mm  / pixels * 100
                TXT_cmpixel = ((viewheight / imageheight) * 100);

                if (!RAD_camdirectionport.Checked)
                {
                    NUM_spacing = ((1 - (overlap / 100.0f)) * viewheight);
                    NUM_Distance = ((1 - (sidelap / 100.0f)) * viewwidth);
                }
                else
                {
                    NUM_spacing = ((1 - (overlap / 100.0f)) * viewwidth);
                    NUM_Distance = ((1 - (sidelap / 100.0f)) * viewheight);
                }

            }
            catch { return; }
        }

        private void CMB_camera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cameras.ContainsKey(CMB_camera.Text))
            {
                camerainfo camera = cameras[CMB_camera.Text];

                num_focallength = (decimal)camera.focallen;
                TXT_imgheight = camera.imageheight.ToString();
                TXT_imgwidth = camera.imagewidth.ToString();
                TXT_sensheight = camera.sensorheight.ToString();
                TXT_senswidth = camera.sensorwidth.ToString();
            }

            doCalc();
        }
       
        private void GridUI_Load(object sender, EventArgs e)
        {
            xmlcamera(false, Settings.GetRunningDirectory() + "camerasBuiltin.xml");

            xmlcamera(false, Settings.GetUserDataDirectory() + "cameras.xml");

            xmlaircraft();

            loadsettings();

            domainUpDown1_ValueChanged(null, null);
        }

        private void TXT_TextChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void CHK_camdirection_CheckedChanged(object sender, EventArgs e)
        {
            doCalc();
        }
     
        public double NUM_spacing { get; set; }

        public double NUM_Distance { get; set; }

        public string TXT_fovH { get; set; }

        public string TXT_fovV { get; set; }

        public double TXT_cmpixel { get; set; }

        public int num_overlap { get; set; }

        public int num_sidelap { get; set; }

        public string TXT_senswidth { get; set; }

        public string TXT_sensheight { get; set; }

        public string TXT_imgwidth { get; set; }

        public string TXT_imgheight { get; set; }

        public decimal num_focallength { get; set; }

        private void CMB_aircraft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (aircrafts.ContainsKey(CMB_aircraft.Text))
            {
                aircraftinfo aircraft = aircrafts[CMB_aircraft.Text];

                NUM_maxspd.Value = (decimal)aircraft.maxvel;
                NUM_minspd.Value = (decimal)aircraft.minvel;

                TBAR_zoom.Minimum = (int)aircraft.minalt;
                TBAR_zoom.Maximum = (int)aircraft.maxalt;
            }

            doCalc();
        }

        private void TBAR_zoom_Scroll(object sender, EventArgs e)
        {
            NUM_altitude.Value = TBAR_zoom.Value;
        }

        bool mousedown = false;
        bool mousedragging = false;
        PointLatLng mousestart = PointLatLng.Empty;

        private void map_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            mousestart = map.FromLocalToLatLng(e.X, e.Y);
        }

        private void map_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
            mousedragging = false;

            domainUpDown1_ValueChanged(null, null);
        }

        private void map_MouseMove(object sender, MouseEventArgs e)
        {
            var mousecurrent = map.FromLocalToLatLng(e.X, e.Y);

            if (mousedown)
            {
                mousedragging = true;

                if (currentmode == mode.panmode)
                {
                    if (e.Button == MouseButtons.Left)
                    {

                        double latdif = mousestart.Lat - mousecurrent.Lat;
                        double lngdif = mousestart.Lng - mousecurrent.Lng;

                        try
                        {
                            map.Position = new PointLatLng(map.Position.Lat + latdif, map.Position.Lng + lngdif);
                        }
                        catch { }
                    }
                }
                else if (currentmode == mode.drawbox)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        var rect = RectangleF.FromLTRB((float)mousestart.Lng, (float)mousestart.Lat, (float)mousecurrent.Lng, (float)mousecurrent.Lat);

                        list.Clear();

                        // tl
                        list.Add(mousestart);
                        // tr
                        list.Add(new PointLatLng(rect.Top, rect.Right));
                        // br
                        list.Add(mousecurrent);
                        // bl
                        list.Add(new PointLatLng(rect.Bottom, rect.Left));

                        if (boxpoly != null)
                            layerpolygons.Polygons.Remove(boxpoly);

                        boxpoly = null;

                        boxpoly = new GMapPolygon(list, "boxpoly");

                        boxpoly.IsHitTestVisible = true;
                        boxpoly.Stroke = new Pen(Color.Red, 2);
                        boxpoly.Fill = Brushes.Transparent;

                        layerpolygons.Polygons.Add(boxpoly);

                        map.Invalidate();
                    }
                }
                else if (currentmode == mode.movebox)
                {
                    //if (mouseinsidepoly)
                    {
                        double latdif = mousestart.Lat - mousecurrent.Lat;
                        double lngdif = mousestart.Lng - mousecurrent.Lng;

                        for (int a = 0; a < boxpoly.Points.Count; a++)
                        {
                            boxpoly.Points[a] = new PointLatLng(boxpoly.Points[a].Lat - latdif, boxpoly.Points[a].Lng - lngdif);
                        }

                        UpdateListFromBox();

                        map.UpdatePolygonLocalPosition(boxpoly);
                        map.Invalidate();

                        mousestart = mousecurrent;
                    }
                }
                else if (currentmode == mode.editbox)
                {
                    double latdif = mousestart.Lat - mousecurrent.Lat;
                    double lngdif = mousestart.Lng - mousecurrent.Lng;

                    // 2 point the create the lowest crosstrack distance
                    // extend at 90 degrees to the bearing of the points based on mouse position

                    PointLatLngAlt p0;
                    PointLatLngAlt p1;

                    PointLatLngAlt bestp0 = PointLatLngAlt.Zero;
                    PointLatLngAlt bestp1 = PointLatLngAlt.Zero;
                    double bestcrosstrack = 9999999;
                    double R = 6371000;

                    for (int a = 0; a < boxpoly.Points.Count; a++)
                    {
                        p0 = boxpoly.Points[a];
                        p1 = boxpoly.Points[(a + 1) % (boxpoly.Points.Count)];

                        var distp0p1 = p0.GetDistance(mousecurrent);
                        var bearingp0curr = p0.GetBearing(mousecurrent);
                        var bearringp0p1 = p0.GetBearing(p1);

                        var ct = Math.Asin(Math.Sin(distp0p1 / R) * Math.Sin((bearingp0curr - bearringp0p1) * deg2rad)) * R;

                        if (Math.Abs(ct) < Math.Abs(bestcrosstrack))
                        {
                            bestp0 = p0;
                            bestp1 = p1;
                            bestcrosstrack = ct;
                        }
                    }

                    var bearing = bestp0.GetBearing(bestp1);

                    layerpolygons.Markers.Clear();
                    layerpolygons.Markers.Add(new GMarkerGoogle(bestp0, GMarkerGoogleType.blue));
                    layerpolygons.Markers.Add(new GMarkerGoogle(bestp1, GMarkerGoogleType.blue));

                    bearing = ((PointLatLngAlt)mousestart).GetBearing(mousecurrent);

                    var newposp0 = bestp0.newpos(bearing, Math.Abs(bestcrosstrack));
                    var newposp1 = bestp1.newpos(bearing, Math.Abs(bestcrosstrack));

                    boxpoly.Points[boxpoly.Points.IndexOf(bestp0)] = newposp0;
                    boxpoly.Points[boxpoly.Points.IndexOf(bestp1)] = newposp1;

                    UpdateListFromBox();

                    map.UpdatePolygonLocalPosition(boxpoly);
                    map.Invalidate();

                    mousestart = mousecurrent;
                }
            }

            mousedragging = false;
        }

        private void UpdateListFromBox()
        {
            list.Clear();

            foreach (var pnt in boxpoly.Points)
            {
                list.Add(pnt);
            }
        }

        private void map_OnPolygonEnter(GMapPolygon item)
        {
            mouseinsidepoly = true;
        }

        private void map_OnPolygonLeave(GMapPolygon item)
        {
            mouseinsidepoly = false;
        }

        private void toolStripButtonpan_Click(object sender, EventArgs e)
        {
            currentmode = mode.panmode;
        }

        private void toolStripButtonbox_Click(object sender, EventArgs e)
        {
            currentmode = mode.drawbox;
        }

        private void toolStripButtoneditbox_Click(object sender, EventArgs e)
        {
            currentmode = mode.editbox;
        }

        private void toolStripButtonmovebox_Click(object sender, EventArgs e)
        {
            currentmode = mode.movebox;
        }

        public bool mouseinsidepoly { get; set; }
    }
}

