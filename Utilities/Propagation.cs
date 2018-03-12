using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Maps;

namespace MissionPlanner.Utilities
{
    public class Propagation
    {
        //Releif Shading Parameters
        byte[] colors = new byte[] { 220, 226, 232, 233, 244, 250, 214, 142, 106 }; //Colors: Red - Yellow - Green
        byte[] colors2 = new byte[] { 195, 189, 123, 80, 81, 45, 51, 57, 105, 111, 75, 74, 70, 72, 106, 108, 142, 178, 214, 215, 250, 244, 239, 238, 233, 232, 226, 220 }; //Colors: Blue - Green - Yellow - Red
        byte[,] imageData;
        RectLatLng imageDataRect;
        private PointLatLngAlt imageDataCenter;
        double[,] alts;
        float prev_alt = 0;
        PointLatLngAlt prev_position;
        int prev_height = 0;
        int prev_width = 0;
        double prev_zoom = 0;
        int width; //width of screen
        int res; //resolution
        int extend = 384; //elevation extention
        double rel; //relative elevation between drone and ground
        float clearance;
        int prev_res;
        double max_alt;
        double min_alt;
        double prev_range;
        double prev_alt2;
        PointLatLngAlt prev_home;
        public bool switched = false;

        public GMapOverlay elevationoverlay;
        public GMapOverlay LineOfSight;
        public GMapOverlay distance;

        // based on current drone alt, shows where can fly without hitting the surface
        public static bool ele_run
        {
            get { return Settings.Instance.GetBoolean("Propagation_Elemap"); }
            set { Settings.Instance["Propagation_Elemap"] = value.ToString(); }
        }
        // terrain shader
        public static bool ter_run
        {
            get { return Settings.Instance.GetBoolean("Propagation_Termap"); }
            set { Settings.Instance["Propagation_Termap"] = value.ToString(); }
        }

        //thread run
        public bool ele_enabled { get; set; } = false;
        Thread elevation; //Map Overlay Thread

        public bool connected { get;  set; } = true;
        public static bool home_kmleft
        {
            get { return Settings.Instance.GetBoolean("Propagation_home_kmleft"); }
            set { Settings.Instance["Propagation_home_kmleft"] = value.ToString(); }
        }
        public static bool drone_kmleft
        {
            get { return Settings.Instance.GetBoolean("Propagation_drone_kmleft"); }
            set { Settings.Instance["Propagation_drone_kmleft"] = value.ToString(); }
        }
        public static bool rf_run
        {
            get { return Settings.Instance.GetBoolean("Propagation_RFmap"); }
            set { Settings.Instance["Propagation_RFmap"] = value.ToString(); }
        }

        public float alt { get;  set; }
        public float altasl { get; set; }
        public PointLatLngAlt HomeLocation { get; set; } = PointLatLngAlt.Zero;

        private GMapControl gMapControl1;
        public PointLatLngAlt center = PointLatLngAlt.Zero;

        public Propagation(GMapControl gMapControl1)
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
        }

        public void Update(PointLatLngAlt HomeLocation, PointLatLngAlt Location, double battery_kmleft)
        {
            this.HomeLocation = HomeLocation;
            distance.Markers.Clear();
            if (connected && home_kmleft)
            {
                // radius from home based on km left
                distance.Markers.Add(new GMapMarkerDistance(HomeLocation, battery_kmleft, Settings.Instance.GetFloat("Propagation_Tolerance")));
            }

            if (connected && drone_kmleft)
            {
                // radius from drone based on km left
                distance.Markers.Add(new GMapMarkerDistance(Location, battery_kmleft, Settings.Instance.GetFloat("Propagation_Tolerance")));
            }
        }

        void elevation_calc()
        {
            ele_enabled = true;

            while (ele_enabled)
            {
                DateTime start = DateTime.Now;
                DateTime start1 = DateTime.Now;
                DateTime start2 = DateTime.Now;
                DateTime start3 = DateTime.Now;
                //
                // Color gradient
                //
                if (ele_run == false && ter_run == false && elevationoverlay.Markers.Count > 0)
                {
                    elevationoverlay.Markers.RemoveAt(0);
                }

                if (connected && ele_run || ter_run)
                {
                    width = gMapControl1.Width;
                    res = Settings.Instance.GetInt32("Propagation_Resolution", 4);

                    if (elevationoverlay.Markers.Count == 0 || center != prev_position || alt != prev_alt || gMapControl1.Height != prev_height || gMapControl1.Width != prev_width || gMapControl1.Zoom != prev_zoom)
                    {
                        if (gMapControl1.Height != prev_height || gMapControl1.Width != prev_width || res != prev_res)
                        {
                            imageData = new byte[(width+ extend), (gMapControl1.Height + extend + 1)];
                            alts = new double[(width + extend), (gMapControl1.Height + extend + 1)];
                        }

                        if (elevationoverlay.Markers.Count == 0 || center != prev_position || gMapControl1.Zoom != prev_zoom)
                        {
                            max_alt = srtm.getAltitude(center.Lat, center.Lng).alt;
                            min_alt = max_alt;

                            imageDataCenter = center;
                            var tl = gMapControl1.FromLocalToLatLng(-extend / 2, -extend / 2);
                            var rb = gMapControl1.FromLocalToLatLng(gMapControl1.Width + extend / 2, gMapControl1.Height + extend / 2);
                            imageDataRect = RectLatLng.FromLTRB(tl.Lng, tl.Lat, rb.Lng, rb.Lat);

                            for (int y = res / 2; y < gMapControl1.Height + extend + 1 - res; y += res)
                            {
                                for (int x = res / 2; x < width + extend - res; x += res)
                                {
                                    var lnglat = gMapControl1.FromLocalToLatLng(x - extend / 2, y - extend / 2);
                                    var alt = srtm.getAltitude(lnglat.Lat, lnglat.Lng).alt;
                                    alts[x, y] = alt;

                                    if (ter_run)
                                    {
                                        if (max_alt < alt)
                                        {
                                            max_alt = alt;
                                        }

                                        if (min_alt > alt)
                                        {
                                            min_alt = alt;
                                        }
                                    }
                                }
                            }
                        }

                        start1 = DateTime.Now;

                        for (int y = res / 2; y < gMapControl1.Height + extend + 1 - res; y += res)
                        {
                            for (int x = res / 2; x < width + extend - res; x += res)
                            {

                                if (ele_run)
                                {
                                    rel = (altasl) - alts[x, y];

                                    var normvalue = normalize(rel);

                                    /*
                                    //diagonal pattern
                                    for (int i = -res / 2; i <= res / 2; i++)
                                    {
                                        imageData[x + i, y + i] = Gradient_byte(normvalue,colors);
                                    }
                                    */

                                    //Square pattern
                                    for (int i = -res / 2; i <= res / 2; i++)
                                    {
                                        for (int j = -res / 2; j <= res / 2; j++)
                                        {
                                            imageData[x + i, y + j] = Gradient_byte(normvalue, colors);
                                        }
                                    }
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

                                    //Square pattern
                                    for (int i = -res / 2; i <= res / 2; i++)
                                    {
                                        for (int j = -res / 2; j <= res / 2; j++)
                                        {
                                            imageData[x + i, y + j] = Gradient_byte(normvalue, colors2);
                                        }
                                    }

                                }


                            }
                        }

                        start2 = DateTime.Now;

                        gMapControl1.Invoke((Action) delegate
                        {
                            elevationoverlay.Markers.Add(new GMapMarkerElevation(imageData, imageDataRect, imageDataCenter));
                            if (elevationoverlay.Markers.Count > 1)
                            {
                                elevationoverlay.Markers.RemoveAt(0);
                            }
                        });

                        prev_position = center;
                        prev_alt = alt;
                        prev_height = gMapControl1.Height;
                        prev_width = gMapControl1.Width;
                        prev_zoom = gMapControl1.Zoom;
                        prev_res = res;
                    }
                }

                //
                // Propagation
                //
                if (rf_run == false && LineOfSight.Polygons.Count > 0)
                {
                    LineOfSight.Polygons.RemoveAt(0);
                }

                if (rf_run && connected)
                {
                    start3 = DateTime.Now;

                    if (prev_home != HomeLocation || switched || prev_range != Settings.Instance.GetFloat("Propagation_Range") || prev_alt2 != alt)
                    {
                        switched = false;
                        List<PointLatLng> pointslist = new List<PointLatLng>();

                        new SightGen(HomeLocation, pointslist, HomeLocation.Alt, altasl);

                        gMapControl1.Invoke((Action) delegate
                        {
                            LineOfSight.Polygons.Add(new GMapPolygon(pointslist, "LOS")
                            {
                                Fill = Brushes.Transparent,
                                Stroke = new Pen(Color.White, 3) {DashStyle = DashStyle.Dash}
                            });

                            if (LineOfSight.Polygons.Count > 1)
                            {
                                LineOfSight.Polygons.RemoveAt(0);
                            }
                        });

                        prev_home = HomeLocation;
                        prev_range = Settings.Instance.GetFloat("Propagation_Range");
                        prev_alt2 = altasl;
                    }
                }

                Console.WriteLine("Propagation all {0} ms {1} ms  {2} ms {3} ms", (DateTime.Now - start).TotalMilliseconds,
                    (DateTime.Now - start1).TotalMilliseconds, (DateTime.Now - start2).TotalMilliseconds,
                    (DateTime.Now - start3).TotalMilliseconds);

                Thread.Sleep(500);
            }

        }

        private byte Gradient_byte(double value, byte[] colr)
        {
            byte val = 0;
            if (value == 1)
            {
                val = colr[(int)(colr.Length * (value)) - 1];
            }

            else
            {
                val = colr[(int)(colr.Length * (value))];
            }

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
            {
                normvalue = 0;
            }

            else if (normvalue > 1)
            {
                normvalue = 1;
            }

            return normvalue;
        }

    }
}
