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

namespace MissionPlanner
{
    public partial class GridUI : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        GMapOverlay layerpolygons;
        GMapPolygon wppoly;
        private Plugin plugin;
        List<PointLatLngAlt> grid;

        Dictionary<string, camerainfo> cameras = new Dictionary<string, camerainfo>();

        public struct camerainfo
        {
            public string name;
            public float focallen;
            public float sensorwidth;
            public float sensorheight;
            public float imagewidth;
            public float imageheight;
        }

        public GridUI(Plugin plugin)
        {
            this.plugin = plugin;

            InitializeComponent();

            map.MapType = MapType.GoogleSatellite;

            layerpolygons = new GMapOverlay(map, "polygons");
            map.Overlays.Add(layerpolygons);

            CMB_startfrom.DataSource = Enum.GetNames(typeof(Grid.StartPosition));
            CMB_startfrom.SelectedIndex = 0;

            // set and angle that is good
            List<PointLatLngAlt> list = new List<PointLatLngAlt>();
            plugin.Host.FPDrawnPolygon.Points.ForEach(x => { list.Add(x); });
            NUM_angle.Value = (decimal)((getAngleOfLongestSide(list) + 360) % 360);

        }

        void AddDrawPolygon()
        {
            
            layerpolygons.Polygons.Add(plugin.Host.FPDrawnPolygon);

            foreach (var item in plugin.Host.FPDrawnPolygon.Points)
            {
                layerpolygons.Markers.Add(new GMapMarkerGoogleRed(item));
            }
        }

        double getAngleOfLongestSide(List<PointLatLngAlt> list)
        {
            double angle = 0;
            double maxdist = 0;
            PointLatLngAlt last = list[0];
            foreach (var item in list)
            {
                 if (item.GetDistance(last) > maxdist) 
                 {
                     angle = item.GetBearing(last);
                 }
                 last = item;
            }

            return angle;
        }

        private void domainUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (CMB_camera.Text != "")
                doCalc();

            // new grid system test
            List<PointLatLngAlt> list = new List<PointLatLngAlt>();

            plugin.Host.FPDrawnPolygon.Points.ForEach(x => { list.Add(x); });

            grid = Grid.CreateGrid(list, (double)NUM_altitude.Value, (double)NUM_Distance.Value, (double)NUM_spacing.Value, (double)NUM_angle.Value, (double)NUM_overshoot.Value, (double)NUM_overshoot2.Value, (Grid.StartPosition)Enum.Parse(typeof(Grid.StartPosition), CMB_startfrom.Text), false);

            List<PointLatLng> list2 = new List<PointLatLng>();

            grid.ForEach(x => { list2.Add(x); });

            map.HoldInvalidation = true;

            layerpolygons.Polygons.Clear();
            layerpolygons.Markers.Clear();

            if (chk_boundary.Checked)
                AddDrawPolygon();

            int a = 1;
            PointLatLngAlt prevpoint = grid[0];
            foreach (var item in grid)
            {
                if (item.Tag == "M")
                {
                    if (chk_internals.Checked)
                        layerpolygons.Markers.Add(new GMapMarkerGoogleGreen(item) { ToolTipText = a.ToString(), ToolTipMode = MarkerTooltipMode.OnMouseOver });
                    try
                    {
                        if (TXT_fovH.Text != "")
                        {
                            float fovh = float.Parse(TXT_fovH.Text);
                            float fovv = float.Parse(TXT_fovV.Text);

                            float startangle = 0;

                            if (!CHK_camdirection.Checked)
                            {
                                startangle = 90;
                            }

                            float angle1 = startangle - (float)(Math.Tan((fovv / 2) / (fovh / 2)) * rad2deg);
                            float dist1 = (float)Math.Sqrt(Math.Pow(fovh / 2, 2) + Math.Pow(fovv / 2, 2));

                            float bearing = (float)prevpoint.GetBearing(item);

                            List<PointLatLng> footprint = new List<PointLatLng>();
                            footprint.Add(item.newpos(bearing + angle1, dist1));
                            footprint.Add(item.newpos(bearing + 180 - angle1, dist1));
                            footprint.Add(item.newpos(bearing + 180 + angle1, dist1));
                            footprint.Add(item.newpos(bearing - angle1, dist1));

                            GMapPolygon poly = new GMapPolygon(footprint, a.ToString());
                            poly.Stroke.Color = Color.FromArgb(250 - ((a * 5) % 240), 250 - ((a * 3) % 240), 250 - ((a * 9) % 240));
                            poly.Stroke.Width = 1;
                            poly.Fill = new SolidBrush(Color.FromArgb(40, Color.Purple));
                            if (chk_footprints.Checked)
                                layerpolygons.Polygons.Add(poly);
                        }
                    }
                    catch { }
                }
                else
                {
                    if (chk_markers.Checked)
                        layerpolygons.Markers.Add(new GMapMarkerGoogleGreen(item) { ToolTipText = a.ToString(), ToolTipMode = MarkerTooltipMode.Always });
                }
                prevpoint = item;
                a++;
            }

            // add wp polygon
            wppoly = new GMapPolygon(list2, "Grid");
            wppoly.Stroke.Color = Color.Yellow;
            wppoly.Stroke.Width = 4;
            if (chk_grid.Checked)
                layerpolygons.Polygons.Add(wppoly);

            Console.WriteLine("Poly Dist " + wppoly.Distance);

            lbl_area.Text = calcpolygonarea(plugin.Host.FPDrawnPolygon.Points).ToString("#") + " m^2";

            lbl_distance.Text = wppoly.Distance.ToString("0.##") + " km";

            lbl_spacing.Text = NUM_spacing.Value.ToString("#") + " m";

            lbl_grndres.Text = TXT_cmpixel.Text;

                map.HoldInvalidation = false;

            map.ZoomAndCenterMarkers("polygons");

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

            GeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

            int utmzone = (int)((polygon[0].Lng - -183.0) / 6.0);

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

            return answer;
        }

        private void BUT_Accept_Click(object sender, EventArgs e)
        {
            if (grid != null && grid.Count > 0)
            {
                if (rad_trigdist.Checked)
                {
                    plugin.Host.comPort.setParam("CAM_TRIGG_DIST",(float)NUM_spacing.Value);
                }

                grid.ForEach(plla =>
                {
                    if (plla.Tag == "M")
                    {
                        if (rad_repeatservo.Checked)
                        {
                            plugin.Host.AddWPtoList(ArdupilotMega.MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, plla.Lng, plla.Lat, plla.Alt);
                            plugin.Host.AddWPtoList(ArdupilotMega.MAVLink.MAV_CMD.DO_REPEAT_SERVO, (float)num_reptservo.Value, (float)num_reptpwm.Value, 999, (float)num_repttime.Value, 0, 0, 0);
                        }
                        if (rad_digicam.Checked)
                        {
                            plugin.Host.AddWPtoList(ArdupilotMega.MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, plla.Lng, plla.Lat, plla.Alt);
                            plugin.Host.AddWPtoList(ArdupilotMega.MAVLink.MAV_CMD.DO_DIGICAM_CONTROL, 0, 0, 0, 0, 0, 0, 0);
                        }
                    }
                    else
                    {
                        plugin.Host.AddWPtoList(ArdupilotMega.MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, plla.Lng, plla.Lat, plla.Alt);
                    }
                });

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
                    CMB_camera.Items.Add(camera.name);
                }
            }
        }

        void doCalc()
        {
            try
            {
                // entered values
                float focallen = (float)num_focallength.Value;
                float flyalt = (float)NUM_altitude.Value;
                int imagewidth = int.Parse(TXT_imgwidth.Text);
                int imageheight = int.Parse(TXT_imgheight.Text);

                float sensorwidth = float.Parse(TXT_senswidth.Text);
                float sensorheight = float.Parse(TXT_sensheight.Text);

                int overlap = (int)num_overlap.Value;
                int sidelap = (int)num_sidelap.Value;


                // scale
                float flscale = 1000 * flyalt / focallen;

                float viewwidth = (sensorwidth * flscale / 1000);
                float viewheight = (sensorheight * flscale / 1000);

                TXT_fovH.Text = viewwidth.ToString("#.#");
                TXT_fovV.Text = viewheight.ToString("#.#");

                float fovh = (float)(Math.Atan(sensorwidth / (2 * focallen)) * rad2deg * 2);
                float fovv = (float)(Math.Atan(sensorheight / (2 * focallen)) * rad2deg * 2);

                TXT_cmpixel.Text = ((viewheight / imageheight) * 100).ToString("0.00 cm");

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

        private void CMB_camera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cameras.ContainsKey(CMB_camera.Text))
            {
                camerainfo camera = cameras[CMB_camera.Text];

                num_focallength.Value = (decimal)camera.focallen;
                TXT_imgheight.Text = camera.imageheight.ToString();
                TXT_imgwidth.Text = camera.imagewidth.ToString();
                TXT_sensheight.Text = camera.sensorheight.ToString();
                TXT_senswidth.Text = camera.sensorwidth.ToString();
            }

            doCalc();
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
                camera.focallen = (float)num_focallength.Value;
                camera.imageheight = float.Parse(TXT_imgheight.Text);
                camera.imagewidth = float.Parse(TXT_imgwidth.Text);
                camera.sensorheight = float.Parse(TXT_sensheight.Text);
                camera.sensorwidth = float.Parse(TXT_senswidth.Text);
            }
            catch { CustomMessageBox.Show("One of your entries is not a valid number"); return; }

            cameras[CMB_camera.Text] = camera;

            xmlcamera(true);
        }

        private void GridUI_Load(object sender, EventArgs e)
        {
            xmlcamera(false, "camerasBuiltin.xml");

            xmlcamera(false);                

            CHK_advanced_CheckedChanged(null, null);
        }

        private void TXT_TextChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void CHK_camdirection_CheckedChanged(object sender, EventArgs e)
        {
            doCalc();
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
                        var item = lcDirectory.GetFloat(ExifDirectory.TAG_FOCAL_LENGTH);
                        num_focallength.Value = (decimal)item;
                    }
                    

                    if (lcDirectory.ContainsTag(ExifDirectory.TAG_DATETIME_ORIGINAL))
                    {

                    }

                }
            }
        }

        private void CHK_advanced_CheckedChanged(object sender, EventArgs e)
        {
            if (CHK_advanced.Checked)
            {
                tabControl1.TabPages.Add(tabGrid);
                tabControl1.TabPages.Add(tabCamera);
                tabControl1.TabPages.Add(tabTrigger);
            }
            else
            {
                tabControl1.TabPages.Remove(tabGrid);
                tabControl1.TabPages.Remove(tabCamera);
                tabControl1.TabPages.Remove(tabTrigger);
            }
        }


    }
}

