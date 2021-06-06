using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Controls.Waypoints;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace MissionPlanner.ArduPilot
{
    public class WPOverlay
    {
        public GMapOverlay overlay = new GMapOverlay("WPOverlay");

        /// <summary>
        /// list of points as per the mission
        /// </summary>
        public List<PointLatLngAlt> pointlist = new List<PointLatLngAlt>();
        /// <summary>
        /// list of point as per mission including jump repeats
        /// </summary>
        List<PointLatLngAlt> route = new List<PointLatLngAlt>();

        public void CreateOverlay(PointLatLngAlt home, List<Locationwp> missionitems, double wpradius, double loiterradius, double altunitmultiplier)
        {
            overlay.Clear();

            GMapPolygon fencepoly = null;

            double maxlat = -180;
            double maxlong = -180;
            double minlat = 180;
            double minlong = 180;

            int dolandstart = -1;

            Func<MAVLink.MAV_FRAME, double, double, double> gethomealt = (altmode, lat, lng) =>
                GetHomeAlt(altmode, home.Alt, lat, lng);

            if (home != PointLatLngAlt.Zero)
            {
                home.Tag = "H";
                pointlist.Add(home);
                route.Add(pointlist[pointlist.Count - 1]);
                addpolygonmarker("H", home.Lng, home.Lat, home.Alt * altunitmultiplier, null, 0);
            }

            for (int a = 0; a < missionitems.Count; a++)
            {
                var item = missionitems[a];
                var itemnext = a + 1 < missionitems.Count ? missionitems[a + 1] : default(Locationwp);

                ushort command = item.id;

                // invalid locationwp
                if (command == 0)
                {
                    pointlist.Add(null);
                    continue;
                }

                // navigatable points
                if (command < (ushort) MAVLink.MAV_CMD.LAST &&
                    command != (ushort) MAVLink.MAV_CMD.RETURN_TO_LAUNCH &&
                    command != (ushort) MAVLink.MAV_CMD.CONTINUE_AND_CHANGE_ALT &&
                    command != (ushort) MAVLink.MAV_CMD.DELAY &&
                    command != (ushort) MAVLink.MAV_CMD.GUIDED_ENABLE
                    || command == (ushort) MAVLink.MAV_CMD.DO_SET_ROI || command == (ushort)MAVLink.MAV_CMD.DO_LAND_START)
                {
                    // land can be 0,0 or a lat,lng
                    if (command == (ushort) MAVLink.MAV_CMD.LAND && item.lat == 0 && item.lng == 0)
                    {
                        continue;
                    }

                    if (command == (ushort) MAVLink.MAV_CMD.DO_LAND_START && item.lat != 0 && item.lng != 0)
                    {     
                        pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME) item.frame, item.lat, item.lng),
                            (a + 1).ToString()));
                        route.Add(pointlist[pointlist.Count - 1]);

                        dolandstart = a;
                        // draw everything before
                        if (route.Count > 0)
                        {
                            RegenerateWPRoute(route, home, false);
                            route.Clear();
                        }
                        
                        route.Add(pointlist[pointlist.Count - 1]);
                        addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                            item.alt * altunitmultiplier, null, wpradius);
                    } 
                    else if (command == (ushort) MAVLink.MAV_CMD.LAND && item.lat != 0 && item.lng != 0)
                    {
                        pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME) item.frame, item.lat, item.lng),
                            (a + 1).ToString()));
                        route.Add(pointlist[pointlist.Count - 1]);
                        addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                            item.alt * altunitmultiplier, null, wpradius);

                        RegenerateWPRoute(route, home,  false);
                        route.Clear();
                    } 
                    else if (command == (ushort) MAVLink.MAV_CMD.DO_SET_ROI)
                    {
                        pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                                item.alt + gethomealt((MAVLink.MAV_FRAME) item.frame, item.lat, item.lng),
                                "ROI" + (a + 1))
                            {color = Color.Red});
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
                    else if (command == (ushort) MAVLink.MAV_CMD.LOITER_TIME ||
                             command == (ushort) MAVLink.MAV_CMD.LOITER_TURNS ||
                             command == (ushort) MAVLink.MAV_CMD.LOITER_UNLIM)
                    {
                        if (item.lat == 0 && item.lng == 0)
                        {
                            pointlist.Add(null);
                            // loiter at current location.
                            if (route.Count >= 1)
                            {
                                var lastpnt = route[route.Count - 1];
                                //addpolygonmarker((a + 1).ToString(), lastpnt.Lng, lastpnt.Lat,item.alt, Color.LightBlue, loiterradius);
                            }
                        }
                        else
                        {
                            pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                                item.alt + gethomealt((MAVLink.MAV_FRAME) item.frame, item.lat, item.lng),
                                (a + 1).ToString())
                            {
                                color = Color.LightBlue
                            });

                            // exit at tangent
                            if (item.p4 == 1)
                            {
                                var from = pointlist.Last();
                                var to = itemnext.lat != 0 && itemnext.lng != 0
                                    ? new PointLatLngAlt(itemnext)
                                    {
                                        Alt = itemnext.alt + gethomealt((MAVLink.MAV_FRAME) item.frame, item.lat,
                                                  item.lng)
                                    }
                                    : from;

                                var bearing = from.GetBearing(to);
                                var dist = from.GetDistance(to);

                                if (dist > loiterradius)
                                {
                                    route.Add(pointlist[pointlist.Count - 1]);
                                    var offset = from.newpos(bearing + 90, loiterradius);
                                    route.Add(offset);
                                }
                                else
                                {
                                    route.Add(pointlist[pointlist.Count - 1]);
                                }
                            }
                            else
                                route.Add(pointlist[pointlist.Count - 1]);

                            addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                                item.alt * altunitmultiplier, Color.LightBlue, loiterradius);
                        }
                    }
                    else if (command == (ushort) MAVLink.MAV_CMD.SPLINE_WAYPOINT)
                    {
                        pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                                item.alt + gethomealt((MAVLink.MAV_FRAME) item.frame, item.lat, item.lng),
                                (a + 1).ToString())
                            {Tag2 = "spline"});
                        route.Add(pointlist[pointlist.Count - 1]);
                        addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                            item.alt * altunitmultiplier, Color.Green, wpradius);
                    }
                    else if (command == (ushort) MAVLink.MAV_CMD.WAYPOINT && item.lat == 0 && item.lng == 0)
                    {
                        if(pointlist.Count > 0)
                            route.Add(pointlist[pointlist.Count - 1]);
                        pointlist.Add(null);
                    }
                    else
                    {
                        if (item.lat != 0 && item.lng != 0)
                        {
                            pointlist.Add(new PointLatLngAlt(item.lat, item.lng,
                                item.alt + gethomealt((MAVLink.MAV_FRAME) item.frame, item.lat, item.lng),
                                (a + 1).ToString()));
                            route.Add(pointlist[pointlist.Count - 1]);
                            addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                                item.alt * altunitmultiplier, null, wpradius);
                        }
                        else
                        {
                            pointlist.Add(null);
                        }
                    }

                    maxlong = Math.Max(item.lng, maxlong);
                    maxlat = Math.Max(item.lat, maxlat);
                    minlong = Math.Min(item.lng, minlong);
                    minlat = Math.Min(item.lat, minlat);
                }
                else if (command == (ushort)MAVLink.MAV_CMD.DO_JUMP) // fix do jumps into the future
                {
                    pointlist.Add(null);

                    int wpno = (int) Math.Max(item.p1, 0);
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
                    /*
                    if (repeat == -1)
                    {
                        for (int wps = wpno; wps < missionitems.Count; wps++)
                        {
                            var newitem = missionitems[wps-1];
                            if (newitem.lat == 0 && newitem.lng == 0 && newitem.id < (ushort)MAVLink.MAV_CMD.LAST)
                                continue;
                            list.Add((PointLatLngAlt) newitem);
                            if (newitem.id == (ushort) MAVLink.MAV_CMD.LAND)
                            {
                                route.AddRange(list);
                                RegenerateWPRoute(route, home,  false);
                                route.Clear();
                                list.Clear();
                                break;
                            }
                        }
                    }
                    */
                    route.AddRange(list);
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION) // fence
                {
                    if(fencepoly == null)
                        fencepoly = new GMapPolygon(new List<PointLatLng>(), a.ToString());
                    pointlist.Add(new PointLatLngAlt(item.lat, item.lng, 0, (a + 1).ToString()));
                    fencepoly.Points.Add(new PointLatLngAlt(item.lat, item.lng, 0, (a + 1).ToString()));
                    addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                        null, Color.Blue, 0, MAVLink.MAV_MISSION_TYPE.FENCE);
                    if (fencepoly.Points.Count == item.p1)
                    {
                        fencepoly.Fill = Brushes.Transparent;
                        fencepoly.Stroke = Pens.Pink;
                        overlay.Polygons.Add(fencepoly);
                        fencepoly = null;
                    }
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION) // fence
                {
                    if (fencepoly == null)
                        fencepoly = new GMapPolygon(new List<PointLatLng>(), a.ToString());
                    pointlist.Add(new PointLatLngAlt(item.lat, item.lng, 0, (a + 1).ToString()));
                    fencepoly.Points.Add(new PointLatLngAlt(item.lat, item.lng, 0, (a + 1).ToString()));
                    addpolygonmarker((a + 1).ToString(), item.lng, item.lat,null, Color.Red, 0, MAVLink.MAV_MISSION_TYPE.FENCE);
                    if (fencepoly.Points.Count == item.p1)
                    {
                        fencepoly.Fill = new SolidBrush(Color.FromArgb(30, 255, 0, 0));
                        fencepoly.Stroke = Pens.Red;
                        overlay.Polygons.Add(fencepoly);
                        fencepoly = null;
                    }
                }
                else if ( command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION) // fence
                {
                    pointlist.Add(new PointLatLngAlt(item.lat, item.lng, 0, (a + 1).ToString()));
                    addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                        null, Color.Red, item.p1, MAVLink.MAV_MISSION_TYPE.FENCE, Color.FromArgb(30, 255, 0, 0));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION) // fence
                {
                    pointlist.Add(new PointLatLngAlt(item.lat, item.lng, 0, (a + 1).ToString()));
                    addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                        null, Color.Blue, item.p1, MAVLink.MAV_MISSION_TYPE.FENCE);
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_RETURN_POINT) // fence
                {
                    pointlist.Add(new PointLatLngAlt(item.lat, item.lng, 0, (a + 1).ToString()));
                    addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                        null, Color.Orange, 0, MAVLink.MAV_MISSION_TYPE.FENCE);
                }
                else if (command >= (ushort)MAVLink.MAV_CMD.RALLY_POINT) // rally
                {
                    pointlist.Add(new PointLatLngAlt(item.lat, item.lng, 0, (a + 1).ToString()));
                    addpolygonmarker((a + 1).ToString(), item.lng, item.lat,
                        null, Color.Orange, 0, MAVLink.MAV_MISSION_TYPE.RALLY);
                }
                else
                {
                    pointlist.Add(null);
                }

                //a++;
            }

            RegenerateWPRoute(route, home);

        }

        private double GetHomeAlt(MAVLink.MAV_FRAME altmode, double homealt, double lat, double lng)
        {
            if (altmode == MAVLink.MAV_FRAME.GLOBAL_INT || altmode == MAVLink.MAV_FRAME.GLOBAL)
            {
                return 0; // for absolute we dont need to add homealt
            }

            if (altmode == MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT_INT || altmode == MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT)
            {
                var sralt = srtm.getAltitude(lat, lng);
                if (sralt.currenttype == srtm.tiletype.invalid)
                    return -999;
                return sralt.alt;
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
        private void addpolygonmarker(string tag, double lng, double lat, double? alt, Color? color, double wpradius, MAVLink.MAV_MISSION_TYPE type = MAVLink.MAV_MISSION_TYPE.MISSION, Color? fillcolor = null)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMapMarker m = null;                
                if(type == MAVLink.MAV_MISSION_TYPE.MISSION)
                {
                    m = new GMapMarkerWP(point, tag);
                    if (alt.HasValue)
                    {
                        m.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        m.ToolTipText = "Alt: " + alt.Value.ToString("0");
                    }
                    m.Tag = tag;
                }
                else if (type == MAVLink.MAV_MISSION_TYPE.FENCE)
                {
                    m = new GMarkerGoogle(point, GMarkerGoogleType.blue_dot);
                    m.Tag = tag;
                }
                else if (type == MAVLink.MAV_MISSION_TYPE.RALLY)
                {
                    m = new GMapMarkerRallyPt(point);
                    if (alt.HasValue)
                    {
                        m.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        m.ToolTipText = "Alt: " + alt.Value.ToString("0");
                    }
                    m.Tag = tag;
                }

                //MissionPlanner.GMapMarkerRectWPRad mBorders = new MissionPlanner.GMapMarkerRectWPRad(point, (int)float.Parse(TXT_WPRad.Text), MainMap);
                GMapMarkerRect mBorders = new GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                    mBorders.Tag = tag;
                    mBorders.wprad = wpradius;
                    if (color.HasValue)
                    {
                        mBorders.Color = color.Value;
                    }
                    if (fillcolor.HasValue)
                    {
                        mBorders.FillColor = fillcolor.Value;
                    }
                }

                overlay.Markers.Add(m);
                overlay.Markers.Add(mBorders);
            }
            catch (Exception)
            {
            }
        }

        private void RegenerateWPRoute(List<PointLatLngAlt> fullpointlist, PointLatLngAlt HomeLocation,
            bool includehomeroute = true)
        {
            if (fullpointlist.Count == 0)
                return;

            GMapRoute route = new GMapRoute("wp route");
            GMapRoute homeroute = new GMapRoute("home route");

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

            // interpolate
            //wproute = wproute.Interpolate();

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
                        if (includehomeroute)
                        {
                            homepoint = x;
                            return;
                        }
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
                        if (includehomeroute)
                        {
                            homeroute.Points.Add(lastpoint);
                            homeroute.Points.Add(homepoint);
                            homeroute.Points.Add(firstpoint);
                        }
                        return;
                    }

                    route.Points.Add(x);
                });

                homeroute.Stroke = new Pen(Color.Yellow, 2);
                // if we have a large distance between home and the first/last point, it hangs on the draw of a the dashed line.
                if (homepoint.GetDistance(lastpoint) < 5000 && homepoint.GetDistance(firstpoint) < 5000)
                    homeroute.Stroke.DashStyle = DashStyle.Dash;


                if (includehomeroute)
                {
                    overlay.Routes.Add(homeroute);
                }

                route.Stroke = new Pen(Color.Yellow, 4);
                route.Stroke.DashStyle = DashStyle.Custom;
                overlay.Routes.Add(route);
            }
        }
    }
}
