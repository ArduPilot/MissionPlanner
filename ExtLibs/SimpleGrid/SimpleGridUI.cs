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

namespace MissionPlanner.SimpleGrid
{
    public partial class GridUI : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        GMapOverlay layerpolygons;
        GMapPolygon wppoly;
        private GridPlugin plugin;
        List<PointLatLngAlt> grid;

        public GridUI(GridPlugin plugin)
        {
            this.plugin = plugin;

            InitializeComponent();

            map.MapProvider = plugin.Host.FDMapType;

            layerpolygons = new GMapOverlay( "polygons");
            map.Overlays.Add(layerpolygons);

            CMB_startfrom.DataSource = Enum.GetNames(typeof(Grid.StartPosition));
            CMB_startfrom.SelectedIndex = 0;

            // set and angle that is good
            List<PointLatLngAlt> list = new List<PointLatLngAlt>();
            plugin.Host.FPDrawnPolygon.Points.ForEach(x => { list.Add(x); });
            NUM_angle.Value = (decimal)((getAngleOfLongestSide(list) + 360) % 360);

        }

        void loadsettings()
        {
            if (plugin.Host.config.ContainsKey("simplegrid_camera"))
            {
                
                loadsetting("simplegrid_alt", NUM_altitude);

                loadsetting("simplegrid_dist", NUM_Distance);
                loadsetting("simplegrid_overshoot1", NUM_overshoot);
                loadsetting("simplegrid_overshoot2", NUM_overshoot2);

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
            plugin.Host.config["simplegrid_alt"] = NUM_altitude.Value.ToString();
            plugin.Host.config["simplegrid_angle"] = NUM_angle.Value.ToString();

            plugin.Host.config["simplegrid_dist"] = NUM_Distance.Value.ToString();
            plugin.Host.config["simplegrid_overshoot1"] = NUM_overshoot.Value.ToString();
            plugin.Host.config["simplegrid_overshoot2"] = NUM_overshoot2.Value.ToString();
        }

        void AddDrawPolygon()
        {
            layerpolygons.Polygons.Add(plugin.Host.FPDrawnPolygon);

            layerpolygons.Polygons[0].Fill = Brushes.Transparent;

            foreach (var item in plugin.Host.FPDrawnPolygon.Points)
            {
                layerpolygons.Markers.Add(new GMarkerGoogle(item,GMarkerGoogleType.red));
            }
        }

        double getAngleOfLongestSide(List<PointLatLngAlt> list)
        {
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

        private void domainUpDown1_ValueChanged(object sender, EventArgs e)
        {

            // new grid system test
            List<PointLatLngAlt> list = new List<PointLatLngAlt>();

            plugin.Host.FPDrawnPolygon.Points.ForEach(x => { list.Add(x); });

            grid = Grid.CreateGrid(list, (double)NUM_altitude.Value, (double)NUM_Distance.Value, 999999, (double)NUM_angle.Value, (double)NUM_overshoot.Value, (double)NUM_overshoot2.Value, (Grid.StartPosition)Enum.Parse(typeof(Grid.StartPosition), CMB_startfrom.Text), false);

            List<PointLatLng> list2 = new List<PointLatLng>();

            grid.ForEach(x => { list2.Add(x); });

            map.HoldInvalidation = true;

            layerpolygons.Polygons.Clear();
            layerpolygons.Markers.Clear();

            if (grid.Count == 0)
            {
                return;
            }

            if (chk_boundary.Checked)
                AddDrawPolygon();

            int strips = 0;
            int a = 1;
            PointLatLngAlt prevpoint = grid[0];
            foreach (var item in grid)
            {
                if (item.Tag == "M")
                {
                }
                else
                {
                    strips++;
                    if (chk_markers.Checked)
                        layerpolygons.Markers.Add(new GMarkerGoogle(item,GMarkerGoogleType.green) { ToolTipText = a.ToString(), ToolTipMode = MarkerTooltipMode.Always });

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

            lbl_area.Text = calcpolygonarea(plugin.Host.FPDrawnPolygon.Points).ToString("#") + " m^2";

            lbl_distance.Text = wppoly.Distance.ToString("0.##") + " km";


            lbl_strips.Text = ((int)(strips / 2)).ToString();
            lbl_distbetweenlines.Text = NUM_Distance.Value.ToString("0.##") + " m";

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
              

                grid.ForEach(plla =>
                {
                    if (plla.Tag == "M")
                    {
                    }
                    else
                    {
                        plugin.Host.AddWPtoList(MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, plla.Lng, plla.Lat, plla.Alt);
                    }
                });

                savesettings();

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

        private void GridUI_Load(object sender, EventArgs e)
        {
            loadsettings();
        }
    }
}

