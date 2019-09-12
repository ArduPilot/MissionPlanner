using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using log4net;
using MissionPlanner;
using MissionPlanner.ArduPilot;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Drawing;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Color = System.Drawing.Color;
using Graphics = MissionPlanner.Utilities.Drawing.Graphics;
using Pen = MissionPlanner.Utilities.Drawing.Pen;

namespace Xamarin
{
    public partial class FlightData : ContentPage
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
        private bool run;

        public FlightData()
        {
            InitializeComponent();

            //AGauge.BackgroundImage = Image.FromStream(new MemoryStream(Properties.Resources.guagebg));

            GMap.NET.GMaps.Instance.PrimaryCache = new MissionPlanner.Maps.MyImageCache();

            GMapControl.MapProvider = GMapProviders.GoogleSatelliteMap;

            GMapControl.MapScaleInfoEnabled = true;
            GMapControl.ScalePen = new Pen(Color.White);
            GMapControl.Position = new PointLatLng(0,0);

            this.gMapControl1.OnPositionChanged += new GMap.NET.PositionChanged(this.gMapControl1_OnPositionChanged);
           // this.gMapControl1.Click += new System.EventHandler(this.gMapControl1_Click);
            this.gMapControl1.MouseDown += this.gMapControl1_MouseDown;
            this.gMapControl1.MouseLeave += this.gMapControl1_MouseLeave;
            this.gMapControl1.MouseMove += this.gMapControl1_MouseMove;

            //gMapControl1.ShowTileGridLines = true;

            // same as flightdata
            gMapControl1.CacheLocation = Settings.GetDataDirectory() +
                                         "gmapcache" + Path.DirectorySeparatorChar;
            gMapControl1.MaxZoom = 24;
            gMapControl1.MinZoom = 1;
            gMapControl1.Zoom = 3;

            gMapControl1.ScaleMode = ScaleModes.Fractional;
            gMapControl1.LevelsKeepInMemmory = 5;

            TRK_zoom.Maximum = 24;
            TRK_zoom.Minimum = 1;
            TRK_zoom.Value = 3;

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

            Task.Run(mainloop);

            prop = new Propagation(gMapControl1);
        }
        private void setMapBearing()
        {
            Invoke((Action)delegate { gMapControl1.Bearing = (int)((MainV2.comPort.MAV.cs.yaw + 360) % 360); });
        }
        private void updateRoutePosition()
        {
            // not async
            Invoke((Action)delegate
            {
                gMapControl1.UpdateRouteLocalPosition(route);
            });
        }

        public void Invoke(Action action)
        {
            Forms.Device.BeginInvokeOnMainThread(action);
        }

        private void updateClearMissionRouteMarkers()
        {
            // not async
            Invoke((Action)delegate
            {
                polygons.Routes.Clear();
                polygons.Markers.Clear();
                routes.Markers.Clear();
            });
        }      /// <summary>
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

            Invoke((Action)delegate
            {
                polygons.Routes.Add(homeroute);
                polygons.Routes.Add(wppath);
            });
        }
        private void updateClearRoutesMarkers()
        {
            Invoke((Action)delegate
            {
                routes.Markers.Clear();
            });
        }
        private void addMissionRouteMarker(GMapMarker marker)
        {
            // not async
            Invoke((Action)delegate
            {
                routes.Markers.Add(marker);
            });
        }    /// <summary>
        /// Try to reduce the number of map position changes generated by the code
        /// </summary>
        DateTime lastmapposchange = DateTime.MinValue;

        private void updateMapPosition(PointLatLng currentloc)
        {
            Invoke((Action)delegate
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
            Invoke((Action)delegate
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
                            (int)(Settings.Instance.GetFloat("TXT_WPRad") / CurrentState.multiplierdist);
                    }
                    catch
                    {
                    }
                    if (color.HasValue)
                    {
                        mBorders.Color = color.Value;
                    }
                }

                Invoke((Action)delegate
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

                Invoke((Action)delegate
                {
                    overlay.Markers.Add(m);
                    overlay.Markers.Add(mBorders);
                });
            }
            catch (Exception)
            {
            }
        }
        private void addMissionPhotoMarker(GMapMarker marker)
        {
            // not async
            Invoke((Action)delegate
            {
                photosoverlay.Markers.Add(marker);
            });
        }

        private Propagation prop; private bool CameraOverlap;
        private void mainloop()
        {
            run = true;

            DateTime tracklast = DateTime.Now.AddSeconds(0);
            List<PointLatLng> trackPoints = new List<PointLatLng>();
            DateTime waypoints = DateTime.Now.AddSeconds(0);
            DateTime mapupdate = DateTime.Now.AddSeconds(0);

            while (run)
            {
                try
                {
                    Thread.Sleep(50);

                    Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        var start = DateTime.Now;

                        Hud.airspeed = MainV2.comPort.MAV.cs.airspeed;
                        Hud.alt = MainV2.comPort.MAV.cs.alt;
                        Hud.batterylevel = (float) MainV2.comPort.MAV.cs.battery_voltage;
                        Hud.batteryremaining = MainV2.comPort.MAV.cs.battery_remaining;
                        Hud.connected = MainV2.comPort.MAV.cs.connected;
                        Hud.current = (float) MainV2.comPort.MAV.cs.current;
                        Hud.datetime = MainV2.comPort.MAV.cs.datetime;
                        Hud.disttowp = MainV2.comPort.MAV.cs.wp_dist;
                        Hud.ekfstatus = MainV2.comPort.MAV.cs.ekfstatus;
                        Hud.failsafe = MainV2.comPort.MAV.cs.failsafe;
                        Hud.gpsfix = MainV2.comPort.MAV.cs.gpsstatus;
                        Hud.gpsfix2 = MainV2.comPort.MAV.cs.gpsstatus2;
                        Hud.gpshdop = MainV2.comPort.MAV.cs.gpshdop;
                        Hud.gpshdop2 = MainV2.comPort.MAV.cs.gpshdop2;
                        Hud.groundalt = (float) MainV2.comPort.MAV.cs.HomeAlt;
                        Hud.groundcourse = MainV2.comPort.MAV.cs.groundcourse;
                        Hud.groundspeed = MainV2.comPort.MAV.cs.groundspeed;
                        Hud.heading = MainV2.comPort.MAV.cs.yaw;
                        Hud.linkqualitygcs = MainV2.comPort.MAV.cs.linkqualitygcs;
                        Hud.message = MainV2.comPort.MAV.cs.messageHigh;
                        Hud.messagetime = MainV2.comPort.MAV.cs.messageHighTime;
                        Hud.mode = MainV2.comPort.MAV.cs.mode;
                        Hud.navpitch = MainV2.comPort.MAV.cs.nav_pitch;
                        Hud.navroll = MainV2.comPort.MAV.cs.nav_roll;
                        Hud.pitch = MainV2.comPort.MAV.cs.pitch;
                        Hud.roll = MainV2.comPort.MAV.cs.roll;
                        Hud.status = MainV2.comPort.MAV.cs.armed;
                        Hud.targetalt = MainV2.comPort.MAV.cs.targetalt;
                        Hud.targetheading = MainV2.comPort.MAV.cs.nav_bearing;
                        Hud.targetspeed = MainV2.comPort.MAV.cs.targetairspeed;
                        Hud.turnrate = MainV2.comPort.MAV.cs.turnrate;
                        Hud.verticalspeed = MainV2.comPort.MAV.cs.verticalspeed;
                        Hud.vibex = MainV2.comPort.MAV.cs.vibex;
                        Hud.vibey = MainV2.comPort.MAV.cs.vibey;
                        Hud.vibez = MainV2.comPort.MAV.cs.vibez;
                        Hud.wpno = (int) MainV2.comPort.MAV.cs.wpno;
                        Hud.xtrack_error = MainV2.comPort.MAV.cs.xtrack_error;
                        Hud.AOA = MainV2.comPort.MAV.cs.AOA;
                        Hud.SSA = MainV2.comPort.MAV.cs.SSA;
                        Hud.critAOA = MainV2.comPort.MAV.cs.crit_AOA;

                        // update map
                        if (tracklast.AddSeconds(Settings.Instance.GetDouble("FD_MapUpdateDelay", 1.2)) < DateTime.Now)
                        {
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

                            int numTrackLength = Settings.Instance.GetInt32("NUM_tracklength", 200);
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

                                    //distanceBar1.ClearWPDist();

                                    var i = -1;
                                    var travdist = 0.0;
                                    var lastplla = overlay.pointlist.First();
                                    foreach (var plla in overlay.pointlist)
                                    {
                                        i++;
                                        if (plla == null)
                                            continue;

                                        var dist = lastplla.GetDistance(plla);

                                        //distanceBar1.AddWPDist((float)dist);

                                        if (i <= MainV2.comPort.MAV.cs.wpno)
                                        {
                                            travdist += dist;
                                        }
                                    }

                                    travdist -= MainV2.comPort.MAV.cs.wp_dist;

                                    //if (MainV2.comPort.MAV.cs.mode.ToUpper() == "AUTO")
                                        //distanceBar1.traveleddist = (float)travdist;
                                }

                                RegeneratePolygon();

                                // update rally points

                                rallypointoverlay.Markers.Clear();

                                foreach (var mark in MainV2.comPort.MAV.rallypoints.Values)
                                {
                                    rallypointoverlay.Markers.Add(
                                        new GMapMarkerRallyPt(new PointLatLngAlt(mark.x / 1e7, mark.y / 1e7, mark.z)));
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
                                        /*
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
                                        */
                                    }
                                }


                                // cleanup old - no markers where added, so remove all old 
                                if (MainV2.comPort.MAV.camerapoints.Count < photosoverlay.Markers.Count)
                                    photosoverlay.Markers.Clear();

                                var min_interval = 0.0;
                                if (MainV2.comPort.MAV.param.ContainsKey("CAM_MIN_INTERVAL"))
                                    min_interval = MainV2.comPort.MAV.param["CAM_MIN_INTERVAL"].Value / 1000.0;

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
                                    var timesincelastshot = (mark.time_usec / 1000.0) / 1000.0 - oldtime;
                                    MainV2.comPort.MAV.cs.timesincelastshot = timesincelastshot;
                                    bool contains = photosoverlay.Markers.Any(p => p.Tag.Equals(mark.time_usec));
                                    if (!contains)
                                    {
                                        if (timesincelastshot < min_interval)
                                            addMissionPhotoMarker(new GMapMarkerPhoto(mark, true));
                                        else
                                            addMissionPhotoMarker(new GMapMarkerPhoto(mark, false));
                                    }
                                    oldtime = (mark.time_usec / 1000.0) / 1000.0;
                                }

                                var GMapMarkerOverlapCount = new GMapMarkerOverlapCount(PointLatLng.Empty);

                                // age current
                                int camcount = MainV2.comPort.MAV.camerapoints.Count;
                                int a = 0;
                                foreach (var mark in photosoverlay.Markers)
                                {
                                    if (mark is GMapMarkerPhoto)
                                    {
                                        if (CameraOverlap)
                                        {
                                            var marker = ((GMapMarkerPhoto)mark);
                                            // abandon roll higher than 25 degrees
                                            if (Math.Abs(marker.Roll) < 25)
                                            {
                                                GMapMarkerOverlapCount.Add(
                                                    ((GMapMarkerPhoto)mark).footprintpoly);
                                            }
                                        }
                                        if (a < (camcount - 4))
                                            ((GMapMarkerPhoto)mark).drawfootprint = false;
                                    }
                                    a++;
                                }

                                if (CameraOverlap)
                                {
                                    if (!kmlpolygons.Markers.Contains(GMapMarkerOverlapCount) &&
                                        camcount > 0)
                                    {
                                        kmlpolygons.Markers.Clear();
                                        kmlpolygons.Markers.Add(GMapMarkerOverlapCount);
                                    }
                                }
                                else if (kmlpolygons.Markers.Contains(GMapMarkerOverlapCount))
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
                                    if (((DateTime)plla.Time) > DateTime.Now.AddSeconds(-30))
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
                                    addpolygonmarker("Guided Mode", MainV2.comPort.MAV.GuidedMode.y / 1e7,
                                        MainV2.comPort.MAV.GuidedMode.x / 1e7, (int)MainV2.comPort.MAV.GuidedMode.z,
                                        Color.Blue,
                                        routes);
                                }

                                // draw all icons for all connected mavs
                                foreach (var port in MainV2.Comports.ToArray())
                                {
                                    // draw the mavs seen on this port
                                    foreach (var MAV in port.MAVlist)
                                    {
                                        var marker = Common.getMAVMarker(MAV);

                                        if (marker.Position.Lat == 0 && marker.Position.Lng == 0)
                                            continue;

                                        addMissionRouteMarker(marker);
                                    }
                                }

                                if (route.Points.Count == 0 || route.Points[route.Points.Count - 1].Lat != 0 &&
                                    (mapupdate.AddSeconds(3) < DateTime.Now) && CHK_autopan.IsToggled)
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

                        var ts = (DateTime.Now - start);

                        //Console.WriteLine("Hud update {0}", ts.TotalSeconds);
                    });
                }
                catch
                {

                }
            }
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
                TRK_zoom.Value = gMapControl1.Zoom;
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

            //e.Surface.Canvas.DrawText(SkglView.CanvasSize.ToString(), 80, 60,new SKPaint() {TextSize = 16, StrokeWidth = 2});

            e.Surface.Canvas.Flush();

            //AGauge.Value++;
        }
        private void SkglView_OnTouch(object sender, SKTouchEventArgs e)
        {
            touchpoint = e.Location;
            //SkglView.InvalidateSurface();

            e.Handled = true;
        }

        private void PinchGestureRecognizer_OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            
        }

        private void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            var start = DateTime.Now;
            gMapControl1.Zoom = Math.Round(TRK_zoom.Value, 1);
            System.Diagnostics.Debug.WriteLine("Slider_OnValueChanged " + (DateTime.Now - start).TotalSeconds);
        }
    }
}