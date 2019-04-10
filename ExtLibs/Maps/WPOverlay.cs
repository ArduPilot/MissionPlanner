using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Controls.Waypoints;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot
{
    public class WPOverlay
    {
        public GMapOverlay overlay = new GMapOverlay("WPOverlay");
        public GMapRoute route = new GMapRoute("wp route");
        public GMapRoute homeroute = new GMapRoute("home route");
        /// list of points as per the mission
        public List<PointLatLngAlt> pointlist = new List<PointLatLngAlt>();
        /// list of point as per mission including jump repeats
        public List<PointLatLngAlt> fullpointlist = new List<PointLatLngAlt>();

        public void CreateOverlay(MAVLink.MAV_FRAME altmode, PointLatLngAlt home, List<Locationwp> missionitems, double wpradius, double loiterradius)
        {
            overlay.Clear();

            double maxlat = -180;
            double maxlong = -180;
            double minlat = 180;
            double minlong = 180;

            Func<double, double, double> gethomealt = (lat, lng) => GetHomeAlt(altmode, home.Alt, lat, lng);

            home.Tag = "H";
            home.Tag2 = altmode.ToString();

            pointlist.Add(home);
            fullpointlist.Add(pointlist[pointlist.Count - 1]);
            addpolygonmarker("H", home.Lng, home.Lat, home.Alt, null, 0);

            int a = 0;
            foreach (var itemtuple in missionitems.PrevNowNext())
            {
                var itemprev = itemtuple.Item1;
                var item = itemtuple.Item2;
                var itemnext = itemtuple.Item3;

                ushort command = item.id;

                // invalid locationwp
                if(command == 0)
                    continue;

                if (command < (ushort)MAVLink.MAV_CMD.LAST &&
                    command != (ushort)MAVLink.MAV_CMD.TAKEOFF && // doesnt have a position
                    command != (ushort)MAVLink.MAV_CMD.VTOL_TAKEOFF && // doesnt have a position
                    command != (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH &&
                    command != (ushort)MAVLink.MAV_CMD.CONTINUE_AND_CHANGE_ALT &&
                    command != (ushort)MAVLink.MAV_CMD.DELAY &&
                    command != (ushort)MAVLink.MAV_CMD.GUIDED_ENABLE
                    || command == (ushort)MAVLink.MAV_CMD.DO_SET_ROI)
                {
                    // land can be 0,0 or a lat,lng
                    if (command == (ushort)MAVLink.MAV_CMD.LAND && item.lat == 0 && item.lng == 0)
                        continue;

                    if (command == (ushort)MAVLink.MAV_CMD.DO_SET_ROI)
                    {
                        pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                                item.alt + gethomealt(item.lat, item.lng), "ROI" + (a + 1))
                        { color = Color.Red });
                        // do set roi is not a nav command. so we dont route through it
                        //fullpointlist.Add(pointlist[pointlist.Count - 1]);
                        GMarkerGoogle m =
                            new GMarkerGoogle(new PointLatLng(item.lat, item.lng),
                                GMarkerGoogleType.red);
                        m.ToolTipMode = MarkerTooltipMode.Always;
                        m.ToolTipText = (a + 1).ToString();
                        m.Tag = (a + 1).ToString();

                        GMapMarkerRect mBorders = new GMapMarkerRect(m.Position);
                        {
                            mBorders.InnerMarker = m;
                            mBorders.Tag = "Dont draw line";
                        }

                        // check for clear roi, and hide it
                        if (m.Position.Lat != 0 && m.Position.Lng != 0)
                        {
                            // order matters
                            overlay.Markers.Add(m);
                            overlay.Markers.Add(mBorders);
                        }
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.LOITER_TIME ||
                             command == (ushort)MAVLink.MAV_CMD.LOITER_TURNS ||
                             command == (ushort)MAVLink.MAV_CMD.LOITER_UNLIM)
                    {
                        pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                            item.alt + gethomealt(item.lat, item.lng), (a + 1).ToString())
                        {
                            color = Color.LightBlue
                        });

                        // exit at tangent
                        if (item.p4 == 1)
                        {
                            var from = pointlist.Last();
                            var to = itemnext.lat != 0 && itemnext.lng != 0
                                ? new PointLatLngAlt(itemnext) {Alt = itemnext.alt + gethomealt(item.lat, item.lng)}
                                : from;

                            var bearing = from.GetBearing(to);
                            var dist = from.GetDistance(to);

                            if (dist > loiterradius)
                            {
                                fullpointlist.Add(pointlist[pointlist.Count - 1]);
                                var offset = from.newpos(bearing + 90, loiterradius);
                                fullpointlist.Add(offset);
                            }
                            else
                            {
                                fullpointlist.Add(pointlist[pointlist.Count - 1]);
                            }
                        }
                        else
                            fullpointlist.Add(pointlist[pointlist.Count - 1]);

                        addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                            item.alt, Color.LightBlue, loiterradius);
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT)
                    {
                        pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                                item.alt + gethomealt(item.lat, item.lng), (a + 1).ToString())
                        { Tag2 = "spline" });
                        fullpointlist.Add(pointlist[pointlist.Count - 1]);
                        addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                            item.alt, Color.Green, wpradius);
                    }
                    else
                    {
                        pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                            item.alt + gethomealt(item.lat, item.lng), (a + 1).ToString()));
                        fullpointlist.Add(pointlist[pointlist.Count - 1]);
                        addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                            item.alt, null, wpradius);
                    }

                    maxlong = Math.Max(item.lng, maxlong);
                    maxlat = Math.Max(item.lat, maxlat);
                    minlong = Math.Min(item.lng, minlong);
                    minlat = Math.Min(item.lat, minlat);
                }
                else if (command == (ushort)MAVLink.MAV_CMD.DO_JUMP) // fix do jumps into the future
                {
                    pointlist.Add(null);

                    int wpno = (int)item.p1;
                    int repeat = (int)item.p2;

                    List<PointLatLngAlt> list = new List<PointLatLngAlt>();

                    // cycle through reps
                    for (int repno = repeat; repno > 0; repno--)
                    {
                        // cycle through wps
                        for (int no = wpno; no <= a; no++)
                        {
                            if (pointlist[no] != null)
                                list.Add(pointlist[no]);
                        }
                    }

                    fullpointlist.AddRange(list);
                }
                else
                {
                    pointlist.Add(null);
                }

                a++;
            }

            RegenerateWPRoute(fullpointlist, home);

        }

        private double GetHomeAlt(MAVLink.MAV_FRAME altmode, double homealt, double lat, double lng)
        {
            if (altmode == MAVLink.MAV_FRAME.GLOBAL_INT || altmode == MAVLink.MAV_FRAME.GLOBAL)
            {
                return 0; // for absolute we dont need to add homealt
            }

            if (altmode == MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT_INT || altmode == MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT)
            {
                return srtm.getAltitude(lat, lng).alt;
            }

            return homealt;
        }

        /// <summary>
        /// used to add a marker to the map display
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        /// <param name="color"></param>
        private void addpolygonmarker(string tag, double lng, double lat, double alt, Color? color, double wpradius)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMapMarkerWP m = new GMapMarkerWP(point, tag);
                m.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                m.ToolTipText = "Alt: " + alt.ToString("0");
                m.Tag = tag;

                int wpno = -1;
                if (int.TryParse(tag, out wpno))
                {
                    // preselect groupmarker
                    //if (groupmarkers.Contains(wpno))
                        //m.selected = true;
                }

                //MissionPlanner.GMapMarkerRectWPRad mBorders = new MissionPlanner.GMapMarkerRectWPRad(point, (int)float.Parse(TXT_WPRad.Text), MainMap);
                GMapMarkerRect mBorders = new GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                    mBorders.Tag = tag;
                    mBorders.wprad = (int)wpradius;
                    if (color.HasValue)
                    {
                        mBorders.Color = color.Value;
                    }
                }

                overlay.Markers.Add(m);
                overlay.Markers.Add(mBorders);
            }
            catch (Exception)
            {
            }
        }

        private void RegenerateWPRoute(List<PointLatLngAlt> fullpointlist, PointLatLngAlt HomeLocation)
        {
            route.Clear();
            homeroute.Clear();

            PointLatLngAlt lastpnt = fullpointlist[0];
            PointLatLngAlt lastpnt2 = fullpointlist[0];
            PointLatLngAlt lastnonspline = fullpointlist[0];
            List<PointLatLngAlt> splinepnts = new List<PointLatLngAlt>();
            List<PointLatLngAlt> wproute = new List<PointLatLngAlt>();

            // add home - this causeszx the spline to always have a straight finish
            fullpointlist.Add(fullpointlist[0]);

            for (int a = 0; a < fullpointlist.Count; a++)
            {
                if (fullpointlist[a] == null)
                    continue;

                if (fullpointlist[a].Tag2 == "spline")
                {
                    if (splinepnts.Count == 0)
                        splinepnts.Add(lastpnt);

                    splinepnts.Add(fullpointlist[a]);
                }
                else
                {
                    if (splinepnts.Count > 0)
                    {
                        List<PointLatLng> list = new List<PointLatLng>();

                        splinepnts.Add(fullpointlist[a]);

                        Spline2 sp = new Spline2(HomeLocation);

                        sp.set_wp_origin_and_destination(sp.pv_location_to_vector(lastpnt2),
                            sp.pv_location_to_vector(lastpnt));

                        sp._flags.reached_destination = true;

                        for (int no = 1; no < (splinepnts.Count - 1); no++)
                        {
                            Spline2.spline_segment_end_type segtype =
                                Spline2.spline_segment_end_type.SEGMENT_END_STRAIGHT;

                            if (no < (splinepnts.Count - 2))
                            {
                                segtype = Spline2.spline_segment_end_type.SEGMENT_END_SPLINE;
                            }

                            sp.set_spline_destination(sp.pv_location_to_vector(splinepnts[no]), false, segtype,
                                sp.pv_location_to_vector(splinepnts[no + 1]));

                            //sp.update_spline();

                            while (sp._flags.reached_destination == false)
                            {
                                float t = 1f;
                                //sp.update_spline();
                                sp.advance_spline_target_along_track(t);
                                // Console.WriteLine(sp.pv_vector_to_location(sp.target_pos).ToString());
                                list.Add(sp.pv_vector_to_location(sp.target_pos));
                            }

                            list.Add(splinepnts[no]);
                        }

                        list.ForEach(x => { wproute.Add(x); });


                        splinepnts.Clear();

                        lastnonspline = fullpointlist[a];
                    }

                    wproute.Add(fullpointlist[a]);

                    lastpnt2 = lastpnt;
                    lastpnt = fullpointlist[a];
                }
            }

            int count = wproute.Count;
            int counter = 0;
            PointLatLngAlt homepoint = new PointLatLngAlt();
            PointLatLngAlt firstpoint = new PointLatLngAlt();
            PointLatLngAlt lastpoint = new PointLatLngAlt();

            if (count > 2)
            {
                // homeroute = last, home, first
                wproute.ForEach(x =>
                {
                    counter++;
                    if (counter == 1)
                    {
                        homepoint = x;
                        return;
                    }
                    if (counter == 2)
                    {
                        firstpoint = x;
                    }
                    if (counter == count - 1)
                    {
                        lastpoint = x;
                    }
                    if (counter == count)
                    {
                        homeroute.Points.Add(lastpoint);
                        homeroute.Points.Add(homepoint);
                        homeroute.Points.Add(firstpoint);
                        return;
                    }
                    route.Points.Add(x);
                });

                homeroute.Stroke = new Pen(Color.Yellow, 2);
                // if we have a large distance between home and the first/last point, it hangs on the draw of a the dashed line.
                if (homepoint.GetDistance(lastpoint) < 5000 && homepoint.GetDistance(firstpoint) < 5000)
                    homeroute.Stroke.DashStyle = DashStyle.Dash;

                overlay.Routes.Add(homeroute);

                route.Stroke = new Pen(Color.Yellow, 4);
                route.Stroke.DashStyle = DashStyle.Custom;
                overlay.Routes.Add(route);
            }
        }
    }
}
