using System;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews
{
    public partial class FlightData2 : MyUserControl, IActivate, IDeactivate
    {
        public FlightData2()
        {
            InitializeComponent();

            gMapControl1.MapProvider = GMapProviders.GoogleSatelliteMap;
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 3;

            gMapControl1.DisableFocusOnMouseEnter = true;

            gMapControl1.RoutesEnabled = true;
            gMapControl1.PolygonsEnabled = true;
        }

        public void Activate()
        {
            var bg = new Bitmap(640, 480);

            var g = Graphics.FromImage(bg);

            g.Clear(Color.FromArgb(34, 34, 34));

            hud1.bgimage = bg;
            hud1.ForeColor = Color.White;

            //throw new NotImplementedException();
        }

        public void Deactivate()
        {
            //throw new NotImplementedException();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bindingSourceHud.Add(MainV2.comPort.MAV.cs);
            bindingSourceQuickTab.Add(MainV2.comPort.MAV.cs);
        }
    }
}