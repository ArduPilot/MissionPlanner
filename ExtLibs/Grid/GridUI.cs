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
        GMapOverlay polygons;

        public GridUI()
        {
            InitializeComponent();

            map.MapType = MapType.GoogleSatellite;

            polygons = new GMapOverlay(map, "polygons");
            map.Overlays.Add(polygons);

            CMB_startfrom.DataSource = Enum.GetNames(typeof(Grid.StartPosition));
            CMB_startfrom.SelectedIndex = 0;
        }

        private void domainUpDown1_ValueChanged(object sender, EventArgs e)
        {
            MissionPlanner.Grid gr = new MissionPlanner.Grid();

            // new grid system test
            List<PointLatLngAlt> list = new List<PointLatLngAlt>();

            StreamReader sr = new StreamReader(File.OpenRead(@"C:\Users\hog\Documents\apm logs\test.poly"));

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.StartsWith("#"))
                {
                    continue;
                }
                else
                {
                    string[] items = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    list.Add(new PointLatLngAlt(double.Parse(items[0]), double.Parse(items[1])));
                }
            }

            sr.Close();

            List<PointLatLngAlt> grid = gr.CreateGrid(list, (double)NUM_altitude.Value, (double)NUM_Distance.Value, (double)NUM_spacing.Value, (double)NUM_angle.Value, (double)NUM_overshoot.Value, (Grid.StartPosition)Enum.Parse(typeof(Grid.StartPosition), CMB_startfrom.Text), false);

            List<PointLatLng> list2 = new List<PointLatLng>();

            grid.ForEach(x => { list2.Add(x); });

            map.HoldInvalidation = true;

            polygons.Polygons.Clear();
            polygons.Markers.Clear();

            GMapPolygon poly = new GMapPolygon(list2, "Grid");
            polygons.Polygons.Add(poly);



            foreach (var item in list2) 
            {
                polygons.Markers.Add(new GMapMarkerGoogleGreen(item));
            }

                map.HoldInvalidation = false;

            map.ZoomAndCenterMarkers("polygons");

        }
    }
}
