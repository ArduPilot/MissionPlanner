using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Mission;
using static MissionPlanner.Utilities.Mission.CommandUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using GMap.NET.WindowsForms.Markers;

namespace MissionPlanner.Maps
{
    /// <summary>
    /// Graph-based mission overlay with configurable styles, replacing the legacy
    /// <c>WPOverlay</c>. Builds a <see cref="MissionGraph"/>, generates segments
    /// via <see cref="MissionSegmentizer"/>, and renders styled routes and markers.
    /// </summary>
    public class WPOverlay2
    {
        public GMapOverlay overlay = new GMapOverlay("WPOverlay2");

        /// <summary>
        /// List of points as per the mission (non-expanded, skips DO_JUMP repeats),
        /// compatible with WPOverlay.pointlist semantics.
        /// </summary>
        public List<PointLatLngAlt> pointlist = new List<PointLatLngAlt>();

        public VehicleClass VehicleClass = VehicleClass.Copter;
        public bool ShowPlusMarkers = true;

        /// <summary>
        /// Active style, loaded from settings at startup. Updated by the style editor.
        /// </summary>
        public static MissionStyle missionStyle = MissionStyle.LoadFromConfig(
            Path.Combine(Settings.GetUserDataDirectory(), Settings.Instance["missionstyle", ""]));

        /// <summary>
        /// Clears and rebuilds the overlay from the given mission items.
        /// </summary>
        public void CreateOverlay(
            PointLatLngAlt home,
            List<Locationwp> missionitems,
            double wpradius,
            double loiterradius,
            double altunitmultiplier)
        {
            overlay.Clear();
            
            // Only planes should have a default loiter radius
            if (VehicleClass != VehicleClass.Plane)
            {
                loiterradius = wpradius;
            }

            // 1) Rebuild pointlist with legacy semantics
            BuildPointList(home, missionitems);

            // 2) Build a mission graph (no jump expansion)
            var graph = MissionGraph.Create(home, missionitems);

            // 3) Generate segments from the graph
            var segments = MissionSegmentizer.BuildSegments(graph, VehicleClass, loiterradius);

            // 4) Render markers and segments to overlay
            RenderMarkers(overlay, graph, missionitems, wpradius, loiterradius, altunitmultiplier);
            RenderSegments(overlay, segments, wpradius, loiterradius, ShowPlusMarkers);
        }

        public void RenderMarkers(
            GMapOverlay overlay,
            MissionGraph graph,
            List<Locationwp> missionitems,
            double wpradius,
            double loiterradius,
            double altunitmultiplier)
        {
            if(graph.Home != PointLatLngAlt.Zero)
            {
                AddMarker(
                    overlay,
                    0,
                    new PointLatLng(graph.Home.Lat, graph.Home.Lng),
                    PointTag(-1),
                    tooltip: AltitudeTooltip(graph.Home.Alt, altunitmultiplier)
                );
            }

            foreach (var node in graph.Nodes)
            {
                if (!HasLocation(node.Command))
                    continue;

                AddMarker(
                    overlay,
                    node.Command.id,
                    new PointLatLng(node.Command.lat, node.Command.lng),
                    PointTag(node.MissionIndex),
                    radius: MarkerRadius(node.Command, loiterradius, wpradius),
                    tooltip: AltitudeTooltip(node.Command.alt, altunitmultiplier, node.Command.frame)
                );
            }

            foreach (var bookmark in graph.Bookmarks)
            {
                var bookmarkLocation = GetBookmarkLocation(bookmark);
                if (bookmarkLocation.IsEmpty)
                    continue;
                var hasOwnLocation = HasLocation(bookmark.Command);
                var markerIndex = hasOwnLocation ? bookmark.MissionIndex : bookmark.Target.MissionIndex;
                AddMarker(
                    overlay,
                    bookmark.Command.id,
                    bookmarkLocation,
                    PointTag(markerIndex),
                    isDraggable: hasOwnLocation,
                    label: GetBookmarkLabel(bookmark)
                );
            }

            // Find other miscellaneous markers
            for (int i = 0; i < missionitems.Count; i++)
            {
                var cmd = missionitems[i];
                if (IsNode(cmd) || IsBookmark(cmd.id) || !HasLocation(cmd))
                {
                    continue;
                }
                string altText = $"UNKNOWN: {cmd.id}";
                var point = new PointLatLng(cmd.lat, cmd.lng);
                if (IsRegionOfInterest(cmd.id))
                {
                    altText = $"ROI: {i + 1}";
                }
                AddMarker(
                    overlay,
                    cmd.id,
                    point,
                    PointTag(i),
                    tooltip: altText
                );
            }

        }

        static string AltitudeTooltip(double alt, double altunitmultiplier, byte frame = 0)
        {
            var tooltip = $"Alt: {alt * altunitmultiplier:0}";
            switch (frame)
            {
                case (byte)MAVLink.MAV_FRAME.GLOBAL:
                case (byte)MAVLink.MAV_FRAME.GLOBAL_INT:
                    tooltip += " (MSL)";
                    break;
                case (byte)MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT:
                case (byte)MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT_INT:
                    tooltip += " (AGL)";
                    break;
                case (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT:
                case (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT:
                    tooltip += " (Rel)";
                    break;
                default:
                    tooltip += " (UNKNOWN)";
                    break;
            }
            return tooltip;
        }

        public void RenderSegments(
            GMapOverlay overlay,
            List<MissionSegmentizer.Segment> segments,
            double wpradius,
            double loiterradius,
            bool showPlusMarkers)
        {
            foreach (var segment in segments)
            {

                var route = AddRoute(overlay, segment, "segment");

                // These markers are handled by FlightPlanner.cs to insert waypoints
                if (showPlusMarkers && 
                    !segment.Flags.HasFlag(SegmentFlags.Alternate) &&
                    segment.StartNode != null &&
                    segment.EndNode != null &&
                    segment.Midpoint != null)
                {
                    // Skip the insert marker if the segment is too short
                    var markerRadius = Math.Max(
                        MarkerRadius(segment.StartNode.Command, loiterradius, wpradius),
                        MarkerRadius(segment.EndNode.Command, loiterradius, wpradius));
                    if (1000 * route.Distance < markerRadius)
                    {
                        continue;
                    }
                    var midLine = MakeMidlineObject(segment);
                    var plusMarker = new GMapMarkerPlus(segment.Midpoint)
                    {
                        Tag = midLine,
                    };
                    overlay.Markers.Add(plusMarker);
                }
            }
        }

        private GMapRoute AddRoute(GMapOverlay overlay, MissionSegmentizer.Segment segment, string name)
        {
            var points = new List<PointLatLng>();
            foreach (var pt in segment.Path)
            {
                points.Add(new PointLatLng(pt.Lat, pt.Lng));
            }
            var segmentStyle = missionStyle.GetSegmentStyle(segment);
            var route = new GMapRoute(points, name)
            {
                Stroke = new Pen(segmentStyle.StrokeColor, segmentStyle.StrokeWidth)
                {
                    DashStyle = (DashStyle)segmentStyle.DashStyle,
                },
                ArrowMode = segmentStyle.ShowArrow ? GMapRoute.ArrowDrawMode.SinglePerRoute : GMapRoute.ArrowDrawMode.None,
            };
            overlay.Routes.Add(route);
            return route;
        }

        /// <summary>
        /// Adds a waypoint marker to the overlay. When <paramref name="isDraggable"/>
        /// is true, a <see cref="GMapMarkerRect"/> is also added (FlightPlanner
        /// requires one for drag handling, even at zero radius).
        /// FlightPlanner also parses <paramref name="tag"/> from the marker
        /// to identify which waypoint is being dragged.
        /// </summary>
        private void AddMarker(GMapOverlay overlay, ushort cmd, PointLatLng point, string tag,
            double radius = 0, bool isDraggable = true, string label = null, string tooltip = null)
        {
            if (point.IsEmpty || (point.Lat == 0 && point.Lng == 0))
            {
                return;
            }
            var markerStyle = missionStyle.GetMarkerStyle(cmd);
            var marker = new GMapMarkerWP(point, label ?? tag, markerStyle.MarkerType)
            {
                ToolTipMode = MarkerTooltipMode.OnMouseOver,
                ToolTipText = tooltip,
                Tag = tag
            };
            overlay.Markers.Add(marker);
            if (isDraggable)
            {
                var mBorders = new GMapMarkerRect(point)
                {
                    InnerMarker = marker,
                    Tag = tag,
                    wprad = radius,
                    Color = markerStyle.CircleColor
                };
                overlay.Markers.Add(mBorders);
            }
        }
            
        static midline MakeMidlineObject(MissionSegmentizer.Segment segment)
        {
            var startNode = segment.StartNode;
            var endNode = segment.EndNode;
            return new midline
            {
                now = new PointLatLngAlt(
                    startNode.Command.lat,
                    startNode.Command.lng,
                    startNode.Command.alt,
                    PointTag(startNode.MissionIndex)
                ),
                next = new PointLatLngAlt(
                    endNode.Command.lat,
                    endNode.Command.lng,
                    endNode.Command.alt,
                    PointTag(endNode.MissionIndex)
                )
            };
        }

        /// <summary>
        /// Converts a 0-based mission index to the 1-based tag string that
        /// FlightPlanner expects for waypoint identification and drag handling.
        /// Negative indices produce "H" (home).
        /// </summary>
        static string PointTag(int index)
        {
            return (index < 0) ? "H" : (index + 1).ToString();
        }

        static PointLatLng GetBookmarkLocation(MissionBookmark bookmark)
        {
            if (HasLocation(bookmark.Command))
            {
                return new PointLatLng(bookmark.Command.lat, bookmark.Command.lng);
            }
            else if (bookmark.Target != null && HasLocation(bookmark.Target.Command))
            {
                return new PointLatLng(bookmark.Target.Command.lat, bookmark.Target.Command.lng);
            }
            return PointLatLng.Empty;
        }

        static string GetBookmarkLabel(MissionBookmark bookmark)
        {
            switch (bookmark.Command.id)
            {
            case (ushort)MAVLink.MAV_CMD.JUMP_TAG:
                return "T" + ((int)bookmark.Command.p1).ToString();
            case (ushort)MAVLink.MAV_CMD.DO_LAND_START:
                return "LS";
            case (ushort)MAVLink.MAV_CMD.DO_RETURN_PATH_START:
                return "RP";
            default:
                return "";
            }
        }

        /// <summary>
        /// Build the public <see cref="pointlist"/> from the mission items exactly the
        /// same way as the old WPOverlay did. Will be phased out in the future.
        /// </summary>
        void BuildPointList(
            PointLatLngAlt home,
            List<Locationwp> missionitems)
        {
            pointlist.Clear();

            double gethomealt(MAVLink.MAV_FRAME altmode, double lat, double lng) =>
                GetHomeAlt(altmode, home.Alt, lat, lng);

            if (home != PointLatLngAlt.Zero)
            {
                var homeCopy = new PointLatLngAlt(home.Lat, home.Lng, home.Alt, "H");
                pointlist.Add(homeCopy);
            }

            for (int a = 0; a < missionitems.Count; a++)
            {
                var item = missionitems[a];

                ushort command = item.id;

                if (command == 0)
                {
                    pointlist.Add(null);
                    continue;
                }

                if (command < (ushort)MAVLink.MAV_CMD.LAST &&
                    command != (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH &&
                    command != (ushort)MAVLink.MAV_CMD.CONTINUE_AND_CHANGE_ALT &&
                    command != (ushort)MAVLink.MAV_CMD.DELAY &&
                    command != (ushort)MAVLink.MAV_CMD.GUIDED_ENABLE
                    || command == (ushort)MAVLink.MAV_CMD.DO_SET_ROI
                    || command == (ushort)MAVLink.MAV_CMD.DO_LAND_START)
                {
                    if ((command == (ushort)MAVLink.MAV_CMD.LAND ||
                         command == (ushort)MAVLink.MAV_CMD.VTOL_LAND) &&
                        item.lat == 0 && item.lng == 0)
                    {
                        continue;
                    }

                    if (command == (ushort)MAVLink.MAV_CMD.DO_LAND_START &&
                        item.lat != 0 && item.lng != 0)
                    {
                        pointlist.Add(new PointLatLngAlt(
                            item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                            (a + 1).ToString()));
                    }
                    else if ((command == (ushort)MAVLink.MAV_CMD.LAND ||
                              command == (ushort)MAVLink.MAV_CMD.VTOL_LAND) &&
                             item.lat != 0 && item.lng != 0)
                    {
                        pointlist.Add(new PointLatLngAlt(
                            item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                            (a + 1).ToString()));
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.DO_SET_ROI)
                    {
                        pointlist.Add(new PointLatLngAlt(
                            item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                            "ROI" + (a + 1))
                        { color = Color.Red });
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.LOITER_TIME ||
                             command == (ushort)MAVLink.MAV_CMD.LOITER_TURNS ||
                             command == (ushort)MAVLink.MAV_CMD.LOITER_TO_ALT ||
                             command == (ushort)MAVLink.MAV_CMD.LOITER_UNLIM)
                    {
                        if (item.lat == 0 && item.lng == 0)
                        {
                            // loiter at current location -> null entry (matches old)
                            pointlist.Add(null);
                        }
                        else
                        {
                            pointlist.Add(new PointLatLngAlt(
                                item.lat, item.lng,
                                item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                                (a + 1).ToString())
                            { color = Color.LightBlue });
                        }
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT)
                    {
                        pointlist.Add(new PointLatLngAlt(
                            item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                            (a + 1).ToString())
                        { Tag2 = "spline" });
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.WAYPOINT &&
                             item.lat == 0 && item.lng == 0)
                    {
                        pointlist.Add(null);
                    }
                    else
                    {
                        if (item.lat != 0 && item.lng != 0)
                        {
                            pointlist.Add(new PointLatLngAlt(
                                item.lat, item.lng,
                                item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                                (a + 1).ToString()));
                        }
                        else
                        {
                            pointlist.Add(null);
                        }
                    }
                }
                else if (command == (ushort)MAVLink.MAV_CMD.DO_JUMP)
                {
                    pointlist.Add(null);
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_RETURN_POINT)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.RALLY_POINT)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else
                {
                    pointlist.Add(null);
                }
            }
        }

        private double GetHomeAlt(
            MAVLink.MAV_FRAME altmode,
            double homealt,
            double lat,
            double lng)
        {
            if (altmode == MAVLink.MAV_FRAME.GLOBAL_INT ||
                altmode == MAVLink.MAV_FRAME.GLOBAL)
            {
                // absolute: don't add home alt
                return 0;
            }

            if (altmode == MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT_INT ||
                altmode == MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT)
            {
                var sralt = srtm.getAltitude(lat, lng);
                if (sralt.currenttype == srtm.tiletype.invalid)
                    return -999;
                return sralt.alt;
            }

            return homealt;
        }
    }
}
