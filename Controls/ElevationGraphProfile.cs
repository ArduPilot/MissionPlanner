using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// How an item's altitude is measured, from its MAV_FRAME.
    /// </summary>
    public enum ElevationGraphAltFrame
    {
        /// <summary>above home: GLOBAL_RELATIVE_ALT, and any frame not listed below, as the map draws it</summary>
        Relative,

        /// <summary>above mean sea level: GLOBAL</summary>
        Absolute,

        /// <summary>above the terrain under the item: GLOBAL_TERRAIN_ALT</summary>
        Terrain
    }

    /// <summary>
    /// A waypoint or rally point as the plan holds it, for the elevation graph.
    /// </summary>
    public class ElevationGraphItem
    {
        /// <summary>Row number in its list (index + 1), as the grid and the map number it.</summary>
        public int Row;

        /// <summary>Text drawn beside the point: the row, with an R in front for a rally point.</summary>
        public string Label;

        public double Lat;
        public double Lng;

        /// <summary>Metres, measured as <see cref="Frame"/> says.</summary>
        public double Alt;

        public ElevationGraphAltFrame Frame;

        /// <summary>
        /// The item has no position of its own and acts where the vehicle already is: a TAKEOFF
        /// climbs there, a LAND comes down there.
        /// </summary>
        public bool AtPreviousPosition;

        /// <summary>A landing: the vehicle ends up on the ground here.</summary>
        public bool OnGround;

        public static ElevationGraphAltFrame FrameOf(byte frame)
        {
            switch ((MAVLink.MAV_FRAME) frame)
            {
                case MAVLink.MAV_FRAME.GLOBAL:
                case MAVLink.MAV_FRAME.GLOBAL_INT:
                    return ElevationGraphAltFrame.Absolute;
                case MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT:
                case MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT_INT:
                    return ElevationGraphAltFrame.Terrain;
                default:
                    return ElevationGraphAltFrame.Relative;
            }
        }
    }

    /// <summary>
    /// A fence altitude limit from the vehicle's parameters.
    /// </summary>
    public class ElevationGraphFenceLimit
    {
        /// <summary>Metres above home.</summary>
        public double Alt;

        /// <summary>
        /// The fence uses this limit: FENCE_TYPE includes it (older Plane: it is set). Whether the
        /// fence is on at all is <see cref="ElevationGraphFenceState"/>.
        /// </summary>
        public bool Used;
    }

    /// <summary>
    /// Whether the vehicle's fence is on.
    /// </summary>
    public enum ElevationGraphFenceState
    {
        Enabled,

        /// <summary>off; an RC switch or a DO_FENCE_ENABLE command can still turn it on</summary>
        Disabled,

        /// <summary>on with FENCE_ACTION 0: a breach is only reported</summary>
        ReportOnly,

        /// <summary>off until an automatic takeoff completes (FENCE_AUTOENABLE 1 or 2)</summary>
        EnablesAfterTakeoff,

        /// <summary>off until the vehicle arms (FENCE_AUTOENABLE 3)</summary>
        EnablesWhenArmed
    }

    /// <summary>
    /// What the elevation graph shows, as plain data so the profile can be built off the UI thread.
    /// </summary>
    public class ElevationGraphInput
    {
        /// <summary>Planned home, Alt in metres AMSL; null when the home boxes are empty.</summary>
        public PointLatLngAlt Home;

        /// <summary>The points of the mission the vehicle flies through, in order, home not included.</summary>
        public List<ElevationGraphItem> Waypoints = new List<ElevationGraphItem>();

        public List<ElevationGraphItem> RallyPoints = new List<ElevationGraphItem>();

        /// <summary>null when not connected, or the vehicle has no such parameter.</summary>
        public ElevationGraphFenceLimit FenceCeiling;

        /// <summary>null when not connected, or the vehicle has no such parameter.</summary>
        public ElevationGraphFenceLimit FenceFloor;

        /// <summary>null when not known (not connected).</summary>
        public ElevationGraphFenceState? FenceState;

        /// <summary>
        /// The points of a mission list the vehicle flies through, in order, each labelled with
        /// its row (index + 1) as the map numbers it. Commands without a position of their own
        /// are left out (DO_*, CONDITION_*, RTL, a WAYPOINT or LOITER at 0, 0, as on the map),
        /// except a TAKEOFF at 0, 0, which climbs where the vehicle is, and a LAND at 0, 0, which
        /// comes down there. Fence and rally items (the ALL list mixes them in) are never flown to,
        /// so they are left out too and the rows after them keep their numbers.
        /// </summary>
        public static List<ElevationGraphItem> WaypointsOf(IList<Locationwp> items)
        {
            var list = new List<ElevationGraphItem>();
            for (int a = 0; a < items.Count; a++)
            {
                var item = items[a];
                if (!IsFlownTo(item.id))
                    continue;

                var point = new ElevationGraphItem
                {
                    Row = a + 1,
                    Label = (a + 1).ToString(),
                    Lat = item.lat,
                    Lng = item.lng,
                    Alt = item.alt,
                    Frame = ElevationGraphItem.FrameOf(item.frame),
                    OnGround = item.id == (ushort) MAVLink.MAV_CMD.LAND ||
                               item.id == (ushort) MAVLink.MAV_CMD.VTOL_LAND
                };

                if (item.lat == 0 && item.lng == 0)
                {
                    if (point.OnGround)
                    {
                        // down to the ground wherever the vehicle is; the altitude is not used
                        point.AtPreviousPosition = true;
                        point.Frame = ElevationGraphAltFrame.Terrain;
                        point.Alt = 0;
                    }
                    else if (item.id == (ushort) MAVLink.MAV_CMD.TAKEOFF ||
                             item.id == (ushort) MAVLink.MAV_CMD.VTOL_TAKEOFF)
                    {
                        point.AtPreviousPosition = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                list.Add(point);
            }

            return list;
        }

        /// <summary>
        /// The RALLY_POINT items of a list (the rally list, or the ALL list that mixes every type),
        /// labelled R and their row (index + 1).
        /// </summary>
        public static List<ElevationGraphItem> RallyPointsOf(IList<Locationwp> items)
        {
            var list = new List<ElevationGraphItem>();
            for (int a = 0; a < items.Count; a++)
            {
                var item = items[a];
                if (item.id != (ushort) MAVLink.MAV_CMD.RALLY_POINT || (item.lat == 0 && item.lng == 0))
                    continue;

                list.Add(new ElevationGraphItem
                {
                    Row = a + 1,
                    Label = string.Format(Strings.ElevationGraphRallyLabel, a + 1),
                    Lat = item.lat,
                    Lng = item.lng,
                    Alt = item.alt,
                    Frame = ElevationGraphItem.FrameOf(item.frame)
                });
            }

            return list;
        }

        /// <summary>
        /// The fence ceiling and floor from the vehicle's parameters, metres above home, or null
        /// where the vehicle has no such parameter. AC_Fence (Copter, and Plane since it moved to
        /// AC_Fence) has FENCE_ALT_MAX and FENCE_ALT_MIN, used when FENCE_TYPE has bit 0 (max) or
        /// bit 3 (min); Plane's default FENCE_TYPE is the polygon alone. Older Plane firmware has
        /// FENCE_MAXALT and FENCE_MINALT, where 0 turns a limit off and both are off unless the
        /// maximum is above the minimum.
        /// </summary>
        /// <param name="param">a parameter's value, or null when the vehicle does not have it</param>
        public static void FenceAltLimits(Func<string, double?> param, out ElevationGraphFenceLimit ceiling,
            out ElevationGraphFenceLimit floor)
        {
            ceiling = null;
            floor = null;

            var max = param("FENCE_ALT_MAX");
            var min = param("FENCE_ALT_MIN");
            if (max.HasValue || min.HasValue)
            {
                var type = param("FENCE_TYPE");
                var bits = type.HasValue ? (int) type.Value : 0;
                if (max.HasValue)
                    ceiling = new ElevationGraphFenceLimit {Alt = max.Value, Used = (bits & 1) != 0};
                if (min.HasValue)
                    floor = new ElevationGraphFenceLimit {Alt = min.Value, Used = (bits & 8) != 0};
                return;
            }

            var oldmax = param("FENCE_MAXALT");
            var oldmin = param("FENCE_MINALT");
            var ordered = (oldmax ?? 0) > (oldmin ?? 0);
            if (oldmax.HasValue)
                ceiling = new ElevationGraphFenceLimit {Alt = oldmax.Value, Used = oldmax.Value != 0 && ordered};
            if (oldmin.HasValue)
                floor = new ElevationGraphFenceLimit {Alt = oldmin.Value, Used = oldmin.Value != 0 && ordered};
        }

        /// <summary>
        /// Whether the fence is on. The vehicle's own report (the geofence bit of SYS_STATUS's
        /// enabled sensors) follows FENCE_ENABLE, an RC switch, a DO_FENCE_ENABLE command and
        /// auto-enable alike, so it decides when there is one; FENCE_ENABLE stands in otherwise.
        /// </summary>
        /// <param name="param">a parameter's value, or null when the vehicle does not have it</param>
        /// <param name="reportedEnabled">the SYS_STATUS geofence enabled bit, or null when the
        /// vehicle has not reported a fence (no SYS_STATUS yet, or its geofence present bit clear)</param>
        public static ElevationGraphFenceState FenceStateOf(Func<string, double?> param, bool? reportedEnabled)
        {
            var enable = param("FENCE_ENABLE");
            var action = param("FENCE_ACTION");

            // AC_Fence's FENCE_ACTION 0 only reports a breach, and SYS_STATUS then shows the fence
            // off whatever FENCE_ENABLE says. (Older Plane, without FENCE_ENABLE, used 0 for none.)
            if (enable.HasValue && enable.Value != 0 && action.HasValue && action.Value == 0)
                return ElevationGraphFenceState.ReportOnly;

            if (reportedEnabled ?? (!enable.HasValue || enable.Value != 0))
                return ElevationGraphFenceState.Enabled;

            switch ((int) (param("FENCE_AUTOENABLE") ?? 0))
            {
                case 1:
                case 2:
                    return ElevationGraphFenceState.EnablesAfterTakeoff;
                case 3:
                    return ElevationGraphFenceState.EnablesWhenArmed;
                default:
                    return ElevationGraphFenceState.Disabled;
            }
        }

        /// <summary>
        /// A text form of the whole input: two inputs with the same signature draw the same graph.
        /// </summary>
        public string Signature()
        {
            var sb = new StringBuilder();
            Action<double> add = v => sb.Append(v.ToString("R", CultureInfo.InvariantCulture)).Append(',');

            if (Home != null)
            {
                add(Home.Lat);
                add(Home.Lng);
                add(Home.Alt);
            }

            foreach (var list in new[] {Waypoints, RallyPoints})
            {
                sb.Append('|');
                foreach (var item in list)
                {
                    sb.Append(item.Label).Append(':');
                    add(item.Lat);
                    add(item.Lng);
                    add(item.Alt);
                    sb.Append((int) item.Frame).Append(item.AtPreviousPosition ? 'p' : '-')
                        .Append(item.OnGround ? 'g' : '-').Append(';');
                }
            }

            foreach (var limit in new[] {FenceCeiling, FenceFloor})
            {
                sb.Append('|');
                if (limit != null)
                {
                    add(limit.Alt);
                    sb.Append(limit.Used ? 'u' : '-');
                }
            }

            sb.Append('|').Append(FenceState);
            return sb.ToString();
        }

        /// <summary>
        /// Whether the vehicle flies to the item's position: navigation commands, without RTL,
        /// CONTINUE_AND_CHANGE_ALT, DELAY, GUIDED_ENABLE or a NAV_ROI (which only aims the camera).
        /// </summary>
        private static bool IsFlownTo(ushort id)
        {
            if (id == 0 || id >= (ushort) MAVLink.MAV_CMD.LAST)
                return false;

            switch ((MAVLink.MAV_CMD) id)
            {
                case MAVLink.MAV_CMD.RETURN_TO_LAUNCH:
                case MAVLink.MAV_CMD.CONTINUE_AND_CHANGE_ALT:
                case MAVLink.MAV_CMD.DELAY:
                case MAVLink.MAV_CMD.GUIDED_ENABLE:
                case MAVLink.MAV_CMD.ROI:
                    return false;
                default:
                    return true;
            }
        }
    }

    /// <summary>
    /// The terrain and the planned path along a mission, for the Plan screen's elevation graph.
    /// Distances are metres along the path from its start (home when it is set), altitudes are
    /// metres above mean sea level, and NaN marks a value that cannot be worked out yet (no
    /// terrain data there, or an altitude relative to a home that is not set).
    /// </summary>
    public class ElevationGraphProfile
    {
        /// <summary>Closest spacing of terrain samples; the SRTM grid is about 30 m.</summary>
        public const double MinSampleSpacing = 5;

        private const double MetresPerDegree = 6371000 * Math.PI / 180;

        /// <summary>A point along the path.</summary>
        public class Sample
        {
            /// <summary>Metres along the path from its start.</summary>
            public double Dist;

            public double Lat;
            public double Lng;

            /// <summary>Terrain height AMSL.</summary>
            public double Terrain = double.NaN;

            /// <summary>The planned altitude AMSL.</summary>
            public double Alt = double.NaN;

            /// <summary>
            /// On a leg between two points in the air. Legs from home or into a landing start or
            /// end on the ground, so a small or negative clearance there is no warning.
            /// </summary>
            public bool Airborne;
        }

        /// <summary>Home, a waypoint or a rally point placed along the path.</summary>
        public class Marker
        {
            /// <summary>The plan item; null for home.</summary>
            public ElevationGraphItem Item;

            public string Label;
            public double Dist;
            public double Lat;
            public double Lng;
            public double Alt = double.NaN;
            public double Terrain = double.NaN;

            /// <summary>Rally points: metres from the nearest point of the path.</summary>
            public double OffPath;
        }

        public List<Sample> Samples { get; } = new List<Sample>();

        /// <summary>Home (when set) and the waypoints, in the order they are flown.</summary>
        public List<Marker> Waypoints { get; } = new List<Marker>();

        public List<Marker> RallyPoints { get; } = new List<Marker>();

        public double TotalDistance { get; private set; }

        public bool HomeSet { get; private set; }

        public ElevationGraphFenceLimit FenceCeiling { get; private set; }

        /// <summary>The fence ceiling AMSL, NaN when there is none or home is not set.</summary>
        public double FenceCeilingAlt { get; private set; } = double.NaN;

        public ElevationGraphFenceLimit FenceFloor { get; private set; }

        /// <summary>The fence floor AMSL, NaN when there is none or home is not set.</summary>
        public double FenceFloorAlt { get; private set; } = double.NaN;

        /// <summary>Whether the fence is on; null when not known.</summary>
        public ElevationGraphFenceState? FenceState { get; private set; }

        /// <summary>
        /// The vehicle will hold to <paramref name="limit"/> on this flight: the fence uses the
        /// limit, and the fence is on or turns itself on at takeoff or arming. Not knowing the
        /// fence state counts as on.
        /// </summary>
        public bool Enforces(ElevationGraphFenceLimit limit)
        {
            if (limit == null || !limit.Used)
                return false;
            switch (FenceState)
            {
                case ElevationGraphFenceState.Disabled:
                case ElevationGraphFenceState.ReportOnly:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>Some terrain lookups found no data (an SRTM tile still downloading, or none).</summary>
        public bool TerrainMissing { get; private set; }

        /// <summary>The smallest height above the terrain on airborne legs, NaN when unknown.</summary>
        public double LowestClearance { get; private set; } = double.NaN;

        public double LowestClearanceDist { get; private set; } = double.NaN;

        /// <summary>
        /// Lay the mission out along its path and sample the terrain under it. A leg into a
        /// terrain frame point follows the ground, its height above the ground changing evenly
        /// from one end to the other, as ArduPilot flies it; any other leg is a straight line
        /// between the altitudes of its ends.
        /// </summary>
        /// <param name="terrainAt">terrain height AMSL at a lat, lng, or null when there is no data</param>
        /// <param name="maxSamples">about how many terrain samples to take along the whole path</param>
        public static ElevationGraphProfile Build(ElevationGraphInput input, Func<double, double, double?> terrainAt,
            int maxSamples = 1500)
        {
            var profile = new ElevationGraphProfile
            {
                HomeSet = input.Home != null,
                FenceCeiling = input.FenceCeiling,
                FenceFloor = input.FenceFloor,
                FenceState = input.FenceState
            };

            // without home, altitudes relative to it have nothing to stand on and stay NaN
            var homeAlt = input.Home != null ? input.Home.Alt : double.NaN;

            Func<double, double, double> terrain = (lat, lng) =>
            {
                var alt = terrainAt(lat, lng);
                if (alt.HasValue)
                    return alt.Value;
                profile.TerrainMissing = true;
                return double.NaN;
            };

            var nodes = profile.Waypoints;
            if (input.Home != null)
            {
                nodes.Add(new Marker
                {
                    Label = "H",
                    Lat = input.Home.Lat,
                    Lng = input.Home.Lng,
                    Alt = input.Home.Alt,
                    Terrain = terrain(input.Home.Lat, input.Home.Lng)
                });
            }

            foreach (var item in input.Waypoints)
            {
                var lat = item.Lat;
                var lng = item.Lng;
                if (item.AtPreviousPosition)
                {
                    // a takeoff or landing in place needs somewhere to be first
                    if (nodes.Count == 0)
                        continue;
                    lat = nodes[nodes.Count - 1].Lat;
                    lng = nodes[nodes.Count - 1].Lng;
                }

                var ground = terrain(lat, lng);
                var node = new Marker
                {
                    Item = item,
                    Label = item.Label,
                    Lat = lat,
                    Lng = lng,
                    Terrain = ground,
                    Alt = AltitudeAmsl(item.Frame, item.Alt, homeAlt, ground)
                };

                if (nodes.Count > 0)
                {
                    var prev = nodes[nodes.Count - 1];
                    node.Dist = prev.Dist +
                                new PointLatLngAlt(prev.Lat, prev.Lng).GetDistance(new PointLatLngAlt(lat, lng));
                }

                nodes.Add(node);
            }

            profile.TotalDistance = nodes.Count > 0 ? nodes[nodes.Count - 1].Dist : 0;

            if (nodes.Count > 0)
            {
                var first = nodes[0];
                profile.Samples.Add(new Sample
                {
                    Lat = first.Lat,
                    Lng = first.Lng,
                    Terrain = first.Terrain,
                    Alt = first.Alt,
                    Airborne = !IsOnGround(first)
                });
            }

            var spacing = Math.Max(profile.TotalDistance / Math.Max(1, maxSamples), MinSampleSpacing);
            for (int i = 1; i < nodes.Count; i++)
            {
                var from = nodes[i - 1];
                var to = nodes[i];
                var length = to.Dist - from.Dist;
                var steps = Math.Max(1, (int) Math.Ceiling(length / spacing));
                var follow = to.Item.Frame == ElevationGraphAltFrame.Terrain;
                var fromHeight = from.Alt - from.Terrain;
                var airborne = !IsOnGround(from) && !IsOnGround(to);

                for (int s = 1; s <= steps; s++)
                {
                    var f = (double) s / steps;
                    var lat = from.Lat + (to.Lat - from.Lat) * f;
                    var lng = from.Lng + (to.Lng - from.Lng) * f;
                    var ground = s == steps ? to.Terrain : terrain(lat, lng);

                    profile.Samples.Add(new Sample
                    {
                        Dist = from.Dist + length * f,
                        Lat = lat,
                        Lng = lng,
                        Terrain = ground,
                        Alt = follow
                            ? ground + fromHeight + (to.Item.Alt - fromHeight) * f
                            : from.Alt + (to.Alt - from.Alt) * f,
                        Airborne = airborne
                    });
                }
            }

            foreach (var item in input.RallyPoints)
            {
                var ground = terrain(item.Lat, item.Lng);
                var marker = new Marker
                {
                    Item = item,
                    Label = item.Label,
                    Lat = item.Lat,
                    Lng = item.Lng,
                    Terrain = ground,
                    Alt = AltitudeAmsl(item.Frame, item.Alt, homeAlt, ground)
                };
                PlaceOnPath(nodes, marker);
                profile.RallyPoints.Add(marker);
            }

            // fence altitudes are measured from home
            if (input.FenceCeiling != null)
                profile.FenceCeilingAlt = homeAlt + input.FenceCeiling.Alt;
            if (input.FenceFloor != null)
                profile.FenceFloorAlt = homeAlt + input.FenceFloor.Alt;

            foreach (var sample in profile.Samples)
            {
                var clearance = sample.Alt - sample.Terrain;
                if (!sample.Airborne || double.IsNaN(clearance))
                    continue;
                if (double.IsNaN(profile.LowestClearance) || clearance < profile.LowestClearance)
                {
                    profile.LowestClearance = clearance;
                    profile.LowestClearanceDist = sample.Dist;
                }
            }

            return profile;
        }

        /// <summary>
        /// An altitude as entered, in metres above mean sea level, worked out as the map does
        /// (WPOverlay): relative adds home, terrain adds the ground under the point.
        /// </summary>
        public static double AltitudeAmsl(ElevationGraphAltFrame frame, double alt, double homeAlt, double terrain)
        {
            switch (frame)
            {
                case ElevationGraphAltFrame.Absolute:
                    return alt;
                case ElevationGraphAltFrame.Terrain:
                    return terrain + alt;
                default:
                    return homeAlt + alt;
            }
        }

        /// <summary>
        /// The position on the path <paramref name="dist"/> metres from its start, or null when
        /// there is no path.
        /// </summary>
        public PointLatLngAlt PositionAt(double dist)
        {
            if (Samples.Count == 0)
                return null;

            int lo = 0;
            int hi = Samples.Count - 1;
            if (dist <= Samples[lo].Dist)
                return new PointLatLngAlt(Samples[lo].Lat, Samples[lo].Lng);
            if (dist >= Samples[hi].Dist)
                return new PointLatLngAlt(Samples[hi].Lat, Samples[hi].Lng);

            // samples are in distance order
            while (hi - lo > 1)
            {
                var mid = (lo + hi) / 2;
                if (Samples[mid].Dist <= dist)
                    lo = mid;
                else
                    hi = mid;
            }

            var a = Samples[lo];
            var b = Samples[hi];
            var f = b.Dist > a.Dist ? (dist - a.Dist) / (b.Dist - a.Dist) : 0;
            return new PointLatLngAlt(a.Lat + (b.Lat - a.Lat) * f, a.Lng + (b.Lng - a.Lng) * f);
        }

        private static bool IsOnGround(Marker node)
        {
            return node.Item == null || node.Item.OnGround;
        }

        /// <summary>
        /// Put a point that is off the path (a rally point) at the distance along the path of the
        /// path's nearest point, and note how far off the path it is.
        /// </summary>
        private static void PlaceOnPath(List<Marker> nodes, Marker point)
        {
            if (nodes.Count == 0)
                return;

            if (nodes.Count == 1)
            {
                point.OffPath = new PointLatLngAlt(nodes[0].Lat, nodes[0].Lng)
                    .GetDistance(new PointLatLngAlt(point.Lat, point.Lng));
                return;
            }

            var best = double.MaxValue;
            for (int i = 1; i < nodes.Count; i++)
            {
                var a = nodes[i - 1];
                var b = nodes[i];

                // flat metres around the start of the leg: close enough to pick the nearest point
                var coslat = Math.Cos(a.Lat * Math.PI / 180);
                var bx = (b.Lng - a.Lng) * coslat * MetresPerDegree;
                var by = (b.Lat - a.Lat) * MetresPerDegree;
                var px = (point.Lng - a.Lng) * coslat * MetresPerDegree;
                var py = (point.Lat - a.Lat) * MetresPerDegree;
                var length2 = bx * bx + by * by;
                var t = length2 > 0 ? Math.Max(0, Math.Min(1, (px * bx + py * by) / length2)) : 0;
                var dx = px - t * bx;
                var dy = py - t * by;
                var off = Math.Sqrt(dx * dx + dy * dy);

                if (off < best)
                {
                    best = off;
                    point.Dist = a.Dist + t * (b.Dist - a.Dist);
                    point.OffPath = off;
                }
            }
        }
    }
}
