using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Utilities;

namespace MissionPlanner
{
    public partial class GridUI : Form
    {
        GMapOverlay layerpolygons;
        GMapPolygon wppoly;
        private Plugin plugin;
        List<PointLatLngAlt> grid;

        public GridUI(Plugin plugin)
        {
            this.plugin = plugin;

            InitializeComponent();

            map.MapType = MapType.GoogleSatellite;

            layerpolygons = new GMapOverlay(map, "polygons");
            map.Overlays.Add(layerpolygons);

            CMB_startfrom.DataSource = Enum.GetNames(typeof(Grid.StartPosition));
            CMB_startfrom.SelectedIndex = 0;

            layerpolygons.Polygons.Add(plugin.Host.FPDrawnPolygon);

            foreach (var item in plugin.Host.FPDrawnPolygon.Points)
            {
                layerpolygons.Markers.Add(new GMapMarkerGoogleRed(item));
            }
        }

        private void domainUpDown1_ValueChanged(object sender, EventArgs e)
        {
            // new grid system test
            List<PointLatLngAlt> list = new List<PointLatLngAlt>();

            plugin.Host.FPDrawnPolygon.Points.ForEach(x => { list.Add(x); });

            grid = Grid.CreateGrid(list, (double)NUM_altitude.Value, (double)NUM_Distance.Value, (double)NUM_spacing.Value, (double)NUM_angle.Value, (double)NUM_overshoot.Value, (Grid.StartPosition)Enum.Parse(typeof(Grid.StartPosition), CMB_startfrom.Text), false);

            List<PointLatLng> list2 = new List<PointLatLng>();

            grid.ForEach(x => { list2.Add(x); });

            map.HoldInvalidation = true;

            layerpolygons.Polygons.Remove(wppoly);
            layerpolygons.Markers.Clear();

            wppoly = new GMapPolygon(list2, "Grid");
            layerpolygons.Polygons.Add(wppoly);
            wppoly.Stroke.Color = Color.Yellow;
            wppoly.Stroke.Width = 4;

            Console.WriteLine("Poly Dist " + wppoly.Distance);

            int a = 1;
            foreach (var item in list2) 
            {
                layerpolygons.Markers.Add(new GMapMarkerGoogleGreen(item) { ToolTipText = a.ToString(), ToolTipMode = MarkerTooltipMode.Always });
                a++;
            }

                map.HoldInvalidation = false;

            map.ZoomAndCenterMarkers("polygons");

        }

        private void BUT_Accept_Click(object sender, EventArgs e)
        {
            if (grid != null && grid.Count > 0)
            {
                grid.ForEach(plla =>
                {
                    if (plla.Tag == "M")
                    {
                        plugin.Host.AddWPtoList(ArdupilotMega.MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, plla.Lng, plla.Lat, plla.Alt);
                        plugin.Host.AddWPtoList(ArdupilotMega.MAVLink.MAV_CMD.DO_DIGICAM_CONTROL, 0, 0, 0, 0, 0, 0, 0);
                    }
                    else
                    {
                        plugin.Host.AddWPtoList(ArdupilotMega.MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, plla.Lng, plla.Lat, plla.Alt);
                    }
                }
                );

                this.Close();
            }
            else
            {
                CustomMessageBox.Show("Bad Grid");
            }
        }

        private void GridUI_Resize(object sender, EventArgs e)
        {
            map.ZoomAndCenterMarkers("polygons");
        }
    }
}
