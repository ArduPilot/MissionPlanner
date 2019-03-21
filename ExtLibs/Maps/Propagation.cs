using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsForms;
using log4net;
using MissionPlanner.Utilities;
using Extensions = MissionPlanner.Utilities.Extensions;

namespace MissionPlanner.Maps
{
    public class Propagation: IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private double[,] alts;
        public PointLatLngAlt center = PointLatLngAlt.Zero;
        private float clearance;

        //Releif Shading Parameters
        private readonly byte[] colors = {220, 226, 232, 233, 244, 250, 214, 142, 106}; //Colors: Red - Yellow - Green

        private readonly byte[] colors2 =
        {
            195, 189, 123, 80, 81, 45, 51, 57, 105, 111, 75, 74, 70, 72, 106, 108, 142, 178, 214, 215, 250, 244, 239,
            238, 233, 232, 226, 220
        }; //Colors: Blue - Green - Yellow - Red

        public GMapOverlay distance;
        private readonly Thread elevation; //Map Overlay Thread

        public GMapOverlay elevationoverlay;
        private readonly int extend = 384; //elevation extention in pixels

        private readonly IControl gMapControl1;

        private byte[,] imageData;
        private PointLatLngAlt imageDataCenter;
        private RectLatLng imageDataRect;
        public GMapOverlay LineOfSight;
        private double max_alt;
        private double min_alt;
        private float prev_alt;
        private double prev_alt2;
        private int prev_height;
        private PointLatLngAlt prev_home;
        private PointLatLngAlt prev_position;
        private double prev_range;
        private int prev_res;
        private int prev_width;
        private double prev_zoom;
        private bool need_rf_redraw;

        public Propagation(IControl gMapControl1)
        {
            this.gMapControl1 = gMapControl1;

            elevationoverlay = new GMapOverlay("elevation overlay");
            gMapControl1.Overlays.Insert(0, elevationoverlay);

            LineOfSight = new GMapOverlay("LineOfSight");
            gMapControl1.Overlays.Insert(0, LineOfSight);

            distance = new GMapOverlay("distance");
            gMapControl1.Overlays.Insert(0, distance);

            elevation = new Thread(elevation_calc);
            elevation.Name = "elevation";
            elevation.IsBackground = true;
            elevation.Start();

            need_rf_redraw = true;
        }

        // based on current drone alt, shows where can fly without hitting the surface
        public static bool ele_run
        {
            get => Settings.Instance.GetBoolean("Propagation_Elemap");
            set => Settings.Instance["Propagation_Elemap"] = value.ToString();
        }

        // terrain shader
        public static bool ter_run
        {
            get => Settings.Instance.GetBoolean("Propagation_Termap");
            set => Settings.Instance["Propagation_Termap"] = value.ToString();
        }

        //thread run
        public bool ele_enabled { get; set; }

        public bool connected { get; set; } = true;

        public static bool home_kmleft
        {
            get => Settings.Instance.GetBoolean("Propagation_home_kmleft");
            set => Settings.Instance["Propagation_home_kmleft"] = value.ToString();
        }

        public static bool drone_kmleft
        {
            get => Settings.Instance.GetBoolean("Propagation_drone_kmleft");
            set => Settings.Instance["Propagation_drone_kmleft"] = value.ToString();
        }

        public static bool rf_run
        {
            get => Settings.Instance.GetBoolean("Propagation_RFmap");
            set => Settings.Instance["Propagation_RFmap"] = value.ToString();
        }

        public float alt { get; set; }
        public float altasl { get; set; }
        public PointLatLngAlt HomeLocation { get; set; } = PointLatLngAlt.Zero;
        public PointLatLngAlt DroneLocation { get; set; } = PointLatLngAlt.Zero;

        public void Stop()
        {
            ele_enabled = false;
        }

        public void Update(PointLatLngAlt HomeLocation, PointLatLngAlt Location, double battery_kmleft)
        {
            this.HomeLocation = HomeLocation;
            DroneLocation = Location;
            distance.Markers.Clear();

            if (connected && home_kmleft)
            {
                GMapMarkerDistance home_kmleft_marker = new GMapMarkerDistance(HomeLocation, battery_kmleft, Settings.Instance.GetFloat("Propagation_Tolerance"));
                home_kmleft_marker.Pen = new Pen(Brushes.Red, 1);
                home_kmleft_marker.Pen2 = new Pen(Brushes.Orange, 1);
                distance.Markers.Add(home_kmleft_marker);
            }

            if (connected && drone_kmleft)
            {
                GMapMarkerDistance drone_kmleft_marker = new GMapMarkerDistance(Location, battery_kmleft, Settings.Instance.GetFloat("Propagation_Tolerance"));
                drone_kmleft_marker.Pen = new Pen(Brushes.Red, 1);
                drone_kmleft_marker.Pen2 = new Pen(Brushes.Orange, 1);
                distance.Markers.Add(drone_kmleft_marker);
            }
        }

        private void elevation_calc()
        {
            ele_enabled = true;

            while (ele_enabled)
            {
                try
                {
                    var start = DateTime.Now;
                    var start1 = DateTime.Now;
                    var start2 = DateTime.Now;
                    var start3 = DateTime.Now;
                    //
                    // Color gradient
                    //
                    if (ele_run == false && ter_run == false && elevationoverlay.Markers.Count > 0)
                        elevationoverlay.Markers.RemoveAt(0);

                    if (center == PointLatLngAlt.Zero)
                    {
                        // center has not been set yet
                        Thread.Sleep(100);
                        continue;
                    }

                    if (connected && ele_run || ter_run)
                    {
                        var res = Settings.Instance.GetInt32("Propagation_Resolution", 4);
                        var width = gMapControl1.Width;
                        var height = gMapControl1.Height;
                        var zoom = gMapControl1.Zoom;
                        var area = gMapControl1.ViewArea;

                        if (elevationoverlay.Markers.Count == 0 || center != prev_position || alt != prev_alt ||
                            height != prev_height || width != prev_width ||
                            zoom != prev_zoom)
                        {
                            // reset imagedata and alts
                            if (height != prev_height || width != prev_width)
                            {
                                imageData = new byte[width + extend, height + extend + 1];
                                alts = new double[width + extend, height + extend + 1];
                            }

                            // get alt data
                            if (elevationoverlay.Markers.Count == 0 || center != prev_position ||
                                zoom != prev_zoom)
                            {
                                max_alt = srtm.getAltitude(center.Lat, center.Lng).alt;
                                min_alt = max_alt;

                                imageDataCenter = center;
                                var tl = gMapControl1.FromLocalToLatLng(-extend / 2, -extend / 2);
                                var rb = gMapControl1.FromLocalToLatLng(width + extend / 2,
                                    height + extend / 2);
                                imageDataRect = RectLatLng.FromLTRB(tl.Lng, tl.Lat, rb.Lng, rb.Lat);

                                Parallel.ForEach(
                                    Extensions.SteppedRange(res / 2, height + extend + 1 - res, res), y =>
                                    {
                                        for (var x = res / 2; x < width + extend - res; x += res)
                                        {
                                            // dont process if changed
                                            if (zoom != gMapControl1.Zoom || area != gMapControl1.ViewArea)
                                                return;
                                            var lnglat = gMapControl1.FromLocalToLatLng(x - extend / 2, y - extend / 2);
                                            var altresponce = srtm.getAltitude(lnglat.Lat, lnglat.Lng, zoom);
                                            if (altresponce != srtm.altresponce.Invalid &&
                                                altresponce != srtm.altresponce.Ocean)
                                            {
                                                alts[x, y] = altresponce.alt;

                                                if (ter_run)
                                                {
                                                    if (max_alt < altresponce.alt) max_alt = altresponce.alt;

                                                    if (min_alt > altresponce.alt) min_alt = altresponce.alt;
                                                }
                                            }
                                            else
                                                alts[x, y] = -65535;
                                        }
                                    });

                                if (zoom != gMapControl1.Zoom || area != gMapControl1.ViewArea)
                                {
                                    // zoom or view area has been modified while we where getting data
                                    continue;
                                }

                            }

                            start1 = DateTime.Now;

                            // populate imagedata from altdata
                            Parallel.ForEach(
                                Extensions.SteppedRange(res / 2, height + extend + 1 - res, res), y =>
                                {
                                    for (var x = res / 2; x < width + extend - res; x += res)
                                    {
                                        if (!ele_enabled)
                                            return;

                                        if (ele_run)
                                        {
                                            var rel = altasl - alts[x, y];

                                            var normvalue = normalize(rel);

                                            /*
                                            //diagonal pattern
                                            for (int i = -res / 2; i <= res / 2; i++)
                                            {
                                                imageData[x + i, y + i] = Gradient_byte(normvalue,colors);
                                            }
                                            */

                                            var gradcolor = Gradient_byte(normvalue, colors);

                                            if (alts[x, y] < -999)
                                                gradcolor = 0;

                                            //Square pattern
                                            for (var i = -res / 2; i <= res / 2; i++)
                                            for (var j = -res / 2; j <= res / 2; j++)
                                                imageData[x + i, y + j] = gradcolor;
                                        }
                                        else if (ter_run)
                                        {
                                            var normvalue = normalize(alts[x, y]);
                                            /*
                                            //diagonal pattern
                                            for (int i = -res / 2; i <= res / 2; i++)
                                            {
                                                imageData[x + i, y + i] = Gradient_byte(normvalue,colors2);
                                            }
                                            */

                                            var gradcolor = Gradient_byte(normvalue, colors2);

                                            if (alts[x, y] < -999)
                                                gradcolor = 0;

                                            //Square pattern
                                            for (var i = -res / 2; i <= res / 2; i++)
                                            for (var j = -res / 2; j <= res / 2; j++)
                                                imageData[x + i, y + j] = gradcolor;
                                        }
                                    }
                                });

                            start2 = DateTime.Now;

                            var gMapMarkerElevation = new GMapMarkerElevation(imageData,
                                new RectLatLng(imageDataRect.LocationTopLeft, imageDataRect.Size),
                                new PointLatLngAlt(imageDataCenter));

                            if (!ele_enabled)
                                return;

                            gMapControl1.Invoke((Action) delegate
                            {
                                elevationoverlay.Markers.Add(gMapMarkerElevation);
                                if (elevationoverlay.Markers.Count > 1) elevationoverlay.Markers.RemoveAt(0);
                            });

                            prev_position = center;
                            prev_alt = alt;
                            prev_height = height;
                            prev_width = width;
                            prev_zoom = zoom;
                            prev_res = res;
                        }
                    }

                    //
                    // Propagation
                    //
                    if (rf_run == false && LineOfSight.Polygons.Count > 0) LineOfSight.Polygons.RemoveAt(0);

                    if (rf_run && connected)
                    {
                        start3 = DateTime.Now;

                        if (prev_home != HomeLocation || need_rf_redraw ||
                            prev_range != Settings.Instance.GetFloat("Propagation_Range") || prev_alt2 != alt)
                        {
                            need_rf_redraw = false;
                            var pointslist = new List<PointLatLng>();

                            new SightGen(HomeLocation, pointslist, HomeLocation.Alt, DroneLocation, altasl);

                            gMapControl1.Invoke((Action) delegate
                            {
                                LineOfSight.Polygons.Add(new GMapPolygon(pointslist.Cast<PointLatLng>().ToList(), "LOS")
                                {
                                    Fill = Brushes.Transparent,
                                    Stroke = new Pen(Color.White, 3) {DashStyle = DashStyle.Dash}
                                });

                                if (LineOfSight.Polygons.Count > 1) LineOfSight.Polygons.RemoveAt(0);
                            });

                            prev_home = HomeLocation;
                            prev_range = Settings.Instance.GetFloat("Propagation_Range");
                            prev_alt2 = altasl;
                        }
                    }
                    else need_rf_redraw = true;

                    /*Console.WriteLine("Propagation all {0} ms {1} ms  {2} ms {3} ms",
                        (DateTime.Now - start).TotalMilliseconds,
                        (start - start1).TotalMilliseconds, (start1 - start2).TotalMilliseconds,
                        (start2 - start3).TotalMilliseconds);
                        */
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                Thread.Sleep(100);
            }
        }

        private byte Gradient_byte(double value, byte[] colr)
        {
            byte val = 0;
            if (value == 1)
                val = colr[(int) (colr.Length * value) - 1];

            else
                val = colr[(int) (colr.Length * value)];

            return val;
        }


        private double normalize(double value)
        {
            double normvalue = 0;

            if (ele_run)
            {
                clearance = Settings.Instance.GetFloat("Propagation_Clearance", 5); //metres
                normvalue = value / clearance;
            }

            else if (ter_run)
            {
                normvalue = (value - min_alt) / (max_alt - min_alt);
            }
            
            if (normvalue < 0)
                normvalue = 0;

            else if (normvalue > 1) normvalue = 1;

            return normvalue;
        }

        public void Dispose()
        {
            Stop();

            distance?.Dispose();
            elevationoverlay?.Dispose();
            LineOfSight?.Dispose();
        }
    }
}