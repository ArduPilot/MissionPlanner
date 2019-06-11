using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using MissionPlanner;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Drawing;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Controls;
using Xamarin.Forms;
using Brushes = MissionPlanner.Utilities.Drawing.Brushes;
using Color = System.Drawing.Color;
using Graphics = MissionPlanner.Utilities.Drawing.Graphics;
using Image = MissionPlanner.Utilities.Drawing.Image;
using Pen = MissionPlanner.Utilities.Drawing.Pen;
using StringFormat = MissionPlanner.Utilities.Drawing.StringFormat;
using SystemFonts = MissionPlanner.Utilities.Drawing.SystemFonts;

namespace Xamarin
{
    public partial class MainPage_old : ContentPage
    {
        public static GMapOverlay kmlpolygons;
        internal static GMapOverlay geofence;
        internal static GMapOverlay photosoverlay;
        internal static GMapOverlay poioverlay = new GMapOverlay("POI");
        internal static GMapOverlay rallypointoverlay;
        internal static GMapOverlay tfrpolygons;
        internal GMapMarker CurrentGMapMarker;

        internal PointLatLng MouseDownStart;

        private GMapMarker center = new GMapMarker(new PointLatLng(0.0, 0.0));

        GMapMarker marker;

        // poi layer
        GMapOverlay polygons;
        GMapRoute route;
        GMapOverlay routes;
        SKPoint touchpoint = new SKPoint();

        public MainPage_old()
        {
            InitializeComponent();

            AGauge.BackgroundImage = Image.FromStream(new MemoryStream(Properties.Resources.guagebg));

            GMap.NET.GMaps.Instance.PrimaryCache = new MissionPlanner.Maps.MyImageCache();


            GMapControl.MapProvider = GMapProviders.BingSatelliteMap;

            GMapControl.MapScaleInfoEnabled = true;
            GMapControl.ScalePen = new Pen(Color.Orange);
            GMapControl.Position = new PointLatLng(-35, 117.89);

            this.gMapControl1.OnPositionChanged += new GMap.NET.PositionChanged(this.gMapControl1_OnPositionChanged);
           // this.gMapControl1.Click += new System.EventHandler(this.gMapControl1_Click);
            this.gMapControl1.MouseDown += this.gMapControl1_MouseDown;
            this.gMapControl1.MouseLeave += this.gMapControl1_MouseLeave;
            this.gMapControl1.MouseMove += this.gMapControl1_MouseMove;

            // same as flightdata
            gMapControl1.CacheLocation = Settings.GetDataDirectory() +
                                         "gmapcache" + Path.DirectorySeparatorChar;
            gMapControl1.MinZoom = 1;
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
        }
        public GMapControl gMapControl1
        {
            get { return GMapControl; }
        }

        private void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownStart = gMapControl1.FromLocalToLatLng(e.X, e.Y);


        }

        private void gMapControl1_MouseLeave(object sender, EventArgs e)
        {
   
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
                                               MainV2.comPort.MAV.cs.HomeLocation.Point()) * 1000) *
                                          CurrentState.multiplierdist).ToString("0");

                    routes.Markers.Add(marker);
                }
            }
        }
        void gMapControl1_OnMapZoomChanged()
        {
            try
            {
                // Exception System.Runtime.InteropServices.SEHException: External component has thrown an exception.
                //  TRK_zoom.Value = (float)gMapControl1.Zoom;
                //  Zoomlevel.Value = Convert.ToDecimal(gMapControl1.Zoom);
            }
            catch
            {
            }

            center.Position = gMapControl1.Position;
        }

        void gMapControl1_OnMarkerEnter(GMapMarker item)
        {
            CurrentGMapMarker = item;
        }

        void gMapControl1_OnMarkerLeave(GMapMarker item)
        {
            CurrentGMapMarker = null;
        }

        private void gMapControl1_OnPositionChanged(PointLatLng point)
        {
            center.Position = point;

            //UpdateOverlayVisibility();
        }
        private void SKGLView_OnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {


            e.Surface.Canvas.Clear(SKColors.AliceBlue);

            e.Surface.Canvas.DrawRect(10, 10, 50, 50, new SKPaint() {StrokeWidth = 2, Color = SKColors.Red});

            e.Surface.Canvas.DrawText(touchpoint.ToString(), 80, 20, new SKPaint() {TextSize = 16, StrokeWidth = 2});

            var g = new Graphics(e.Surface);



            g.DrawRectangle(new Pen(Color.Red), touchpoint.X, touchpoint.Y, 12, 12);



            e.Surface.Canvas.DrawText(base.Width + " " + base.Height, 80, 40,
                new SKPaint() {TextSize = 16, StrokeWidth = 2});

            e.Surface.Canvas.DrawText(SkglView.CanvasSize.ToString(), 80, 60,
                new SKPaint() {TextSize = 16, StrokeWidth = 2});

            e.Surface.Canvas.Flush();

            AGauge.Value++;
        }
        private void SkglView_OnTouch(object sender, SKTouchEventArgs e)
        {
            touchpoint = e.Location;
            SkglView.InvalidateSurface();

            e.Handled = true;
        }
    }
}