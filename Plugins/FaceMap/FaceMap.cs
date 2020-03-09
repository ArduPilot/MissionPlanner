using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;


namespace MissionPlanner
{
    public class FaceMap
    {
        const double rad2deg = (180 / Math.PI);
        const double deg2rad = (1.0 / rad2deg);

        public struct Linelatlng
        {
            // start of line
            public utmpos p1;
            // end of line
            public utmpos p2;
            // used as a base for grid along line (initial setout)
            public utmpos basepnt;
        }

        public static PointLatLngAlt StartPointLatLngAlt = PointLatLngAlt.Zero;

        static void Addtomap(Linelatlng pos)
        {
            List<PointLatLng> list = new List<PointLatLng>
            {
                pos.p1.ToLLA(),
                pos.p2.ToLLA()
            };

            polygons.Routes.Add(new GMapRoute(list, "test") { Stroke = new System.Drawing.Pen(System.Drawing.Color.Yellow, 4) });
        }


        /// <summary>
        /// this is a debug function
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="tag"></param>
        static void Addtomap(utmpos pos, string tag)
        {
            polygons.Markers.Add(new GMapMarkerWP(pos.ToLLA(), tag));
        }

        static void Zoomandcentermap()
        {
            map.ZoomAndCenterMarkers("polygons");

            map.Invalidate();

            timer.Stop();
        }

        static GMapOverlay polygons = new GMapOverlay("polygons");
        static myGMAP map = new myGMAP();
        static Timer timer = new Timer();


        static void DoDebug()
        {
            polygons.Clear();

            timer.Interval = 2000;
            timer.Tick += (sender, args) => { Zoomandcentermap(); };
            timer.Start();

            if (map.IsHandleCreated)
                return;

            polygons = new GMapOverlay("polygons");
            map = new myGMAP
            {
                MapProvider = GMapProviders.GoogleSatelliteMap,
                MaxZoom = 20
            };
            map.Overlays.Add(polygons);
            map.Size = new Size(1024, 768);
            map.Dock = DockStyle.Fill;

            map.ShowUserControl();
        }

        public static List<PointLatLngAlt> CreateCorridor(List<PointLatLngAlt> polygon, double height, double camViewHeight, double camVertSpacing, double distance, double angle,
            double camPitch, bool flipDirection, double bermDepth, int numBenches, double toeHeight, double toepoint, double toepoint_runs, bool pathHome, double homeAlt, FlightPlanner.altmode altmode)
        {
            int direction = (flipDirection == true ? -1 : 1);

            if (camVertSpacing < 0.1)
                camVertSpacing = 0.1;

            if (polygon.Count == 0 || numBenches < 1)
                return new List<PointLatLngAlt>();

            List<PointLatLngAlt> ans = new List<PointLatLngAlt>();

            // utm zone distance calcs will be done in
            int utmzone = polygon[0].GetUTMZone();

            // utm position list
            List<utmpos> utmpositions = utmpos.ToList(PointLatLngAlt.ToUTM(utmzone, polygon), utmzone);

            double vertOffset = 0;
            double horizOffset = 0;
            double toepoint_runs_count = 0;

            //calculate number of lanes with a starting altitude of half the calculated cam view height
            // double initialAltitude = camViewHeight * Math.Sin(angle * deg2rad) / 3;
            double initialAltitude = toepoint;
            var vertIncrement = camVertSpacing * Math.Sin(angle * deg2rad);
            var lanes = Math.Round((height - initialAltitude) / vertIncrement) + toepoint_runs + 1;

            //repeat for each bench, applying height/berm depth offsets
            for (int bench = 0; bench < numBenches; bench++)
            {
                //repeat for each increment up face
                for (int lane = 0; lane < lanes; lane++)
                {

                    if (toepoint_runs_count < toepoint_runs)
                    {
                        //calculate offset from the base of the face based on toe angle, camera pitch, camera overlap % and bench offset
                        vertOffset = distance * Math.Sin(camPitch * deg2rad) + (initialAltitude + (bench * height) + toeHeight);
                        horizOffset = distance * Math.Cos(camPitch * deg2rad) - ((initialAltitude) / Math.Tan(angle * deg2rad)) - bench * (bermDepth + height / Math.Tan(angle * deg2rad));
                        toepoint_runs_count++;
                    }
                    else
                    {
                        //calculate offset from the base of the face based on toe angle, camera pitch, camera overlap % and bench offset
                        vertOffset = distance * Math.Sin(camPitch * deg2rad) + (initialAltitude + ((lane - toepoint_runs) * vertIncrement) + (bench * height) + toeHeight);
                        horizOffset = distance * Math.Cos(camPitch * deg2rad) - ((initialAltitude + ((lane - toepoint_runs) * vertIncrement)) / Math.Tan(angle * deg2rad)) - bench * (bermDepth + height / Math.Tan(angle * deg2rad));
                    }

                    // THIS IS COMMENTED OUT BECAUSE IT'S INSANE NONSENSE THAT SHOULD NEVER HAVE BEEN IN HERE.
                    //
                    //    //convert to absolute if flight planner is set to absolute mode (shift up by home alt)
                    //    if (altmode == FlightPlanner.altmode.Absolute)
                    //    {
                    //        vertOffset += homeAlt;
                    //    }

                    //if this is the first lane of a bench, climb to the altitude of the first waypoint of the lane before moving to the waypoint 
                    if (lane == 0 && ans.Count > 0)
                    {
                        PointLatLngAlt intermediateWP = new PointLatLngAlt(ans.Last().Lat, ans.Last().Lng, vertOffset)
                        {
                            Tag = "S"
                        };
                        ans.Add(intermediateWP);
                    }

                    GenerateOffsetPath(utmpositions, horizOffset * direction, utmzone)
                        .ForEach(pnt => { ans.Add(pnt); ans.Last().Alt = vertOffset; });

                    //reverse the order of waypoints and direction of offset on the way back 
                    utmpositions.Reverse();
                    direction = -direction;
                }
            }

            //if an odd number of lanes were specified create one last run along the path back to home
            if (pathHome && ((lanes * numBenches) % 2) == 1)
            {
                GenerateOffsetPath(utmpositions, horizOffset * direction, utmzone)
                    .ForEach(pnt => { ans.Add(pnt); ans.Last().Alt = vertOffset; ans.Last().Tag = "R"; });
            }
            return ans;
        }

        private static List<utmpos> GenerateOffsetPath(List<utmpos> utmpositions, double distance, int utmzone)
        {
            List<utmpos> ans = new List<utmpos>();

            utmpos oldpos = utmpos.Zero;

            for (int a = 0; a < utmpositions.Count - 2; a++)
            {
                var prevCenter = utmpositions[a];
                var currCenter = utmpositions[a + 1];
                var nextCenter = utmpositions[a + 2];

                var l1bearing = prevCenter.GetBearing(currCenter);
                var l2bearing = currCenter.GetBearing(nextCenter);

                var l1prev = Newpos(prevCenter, l1bearing + 90, distance);
                var l1curr = Newpos(currCenter, l1bearing + 90, distance);

                var l2curr = Newpos(currCenter, l2bearing + 90, distance);
                var l2next = Newpos(nextCenter, l2bearing + 90, distance);

                var l1l2center = FindLineIntersectionExtension(l1prev, l1curr, l2curr, l2next);

                //start
                if (a == 0)
                {
                    // add start
                    l1prev.Tag = "S";
                    ans.Add(l1prev);

                    // add start/trigger
                    l1prev.Tag = "SM";
                    ans.Add(l1prev);

                    oldpos = l1prev;
                }

                //middle of leg
                l1l2center.Tag = "M";
                ans.Add(l1l2center);
                oldpos = l1l2center;

                // last leg
                if ((a + 3) == utmpositions.Count)
                {
                    l2next.Tag = "ME";
                    ans.Add(l2next);

                    l2next.Tag = "E";
                    ans.Add(l2next);
                }
            }

            return ans;
        }

        // polar to rectangular
        static void Newpos(ref double x, ref double y, double bearing, double distance)
        {
            double degN = 90 - bearing;
            if (degN < 0)
                degN += 360;
            x = x + distance * Math.Cos(degN * deg2rad);
            y = y + distance * Math.Sin(degN * deg2rad);
        }

        // polar to rectangular
        static utmpos Newpos(utmpos input, double bearing, double distance)
        {
            double degN = 90 - bearing;
            if (degN < 0)
                degN += 360;
            double x = input.x + distance * Math.Cos(degN * deg2rad);
            double y = input.y + distance * Math.Sin(degN * deg2rad);

            return new utmpos(x, y, input.zone);
        }

        /// <summary>
        /// from http://stackoverflow.com/questions/1119451/how-to-tell-if-a-line-intersects-a-polygon-in-c
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <returns></returns>
        public static utmpos FindLineIntersectionExtension(utmpos start1, utmpos end1, utmpos start2, utmpos end2)
        {
            double denom = ((end1.x - start1.x) * (end2.y - start2.y)) - ((end1.y - start1.y) * (end2.x - start2.x));
            //  AB & CD are parallel         
            if (denom == 0)
                return utmpos.Zero;
            double numer = ((start1.y - start2.y) * (end2.x - start2.x)) -
                           ((start1.x - start2.x) * (end2.y - start2.y));
            double r = numer / denom;
            double numer2 = ((start1.y - start2.y) * (end1.x - start1.x)) -
                            ((start1.x - start2.x) * (end1.y - start1.y));
            double s = numer2 / denom;
            if ((r < 0 || r > 1) || (s < 0 || s > 1))
            {
                // line intersection is outside our lines.
            }
            // Find intersection point      
            utmpos result = new utmpos
            {
                x = start1.x + (r * (end1.x - start1.x)),
                y = start1.y + (r * (end1.y - start1.y)),
                zone = start1.zone
            };
            return result;
        }
    }
}
