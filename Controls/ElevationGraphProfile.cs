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
    /// The circle fence from the vehicle's parameters: FENCE_RADIUS around home.
    /// </summary>
    public class ElevationGraphFenceCircle
    {
        /// <summary>Metres from home.</summary>
        public double Radius;

        /// <summary>
        /// The fence uses the circle: FENCE_TYPE has bit 1. Whether the fence is on at all is
        /// <see cref="ElevationGraphFenceState"/>.
        /// </summary>
        public bool Used;
    }

    /// <summary>
    /// An area of the polygon fence, from the fence items in the plan: a polygon, or a circle
    /// around a point. The vehicle stays inside an inclusion area and out of an exclusion area.
    /// </summary>
    public class ElevationGraphFenceArea
    {
        public bool Inclusion;

        /// <summary>The polygon's corners in order; null for a circle.</summary>
        public List<PointLatLngAlt> Vertices;

        /// <summary>A circle's centre; null for a polygon.</summary>
        public PointLatLngAlt Centre;

        /// <summary>A circle's radius in metres.</summary>
        public double Radius;

        /// <summary>The area holds the point: inside the polygon, or no further than the radius from the centre.</summary>
        public bool Contains(double lat, double lng)
        {
            if (Vertices == null)
                return Centre.GetDistance(new PointLatLngAlt(lat, lng)) <= Radius;

            // a ray from the point crosses the edges an odd number of times when it is inside
            var inside = false;
            for (int i = 0, j = Vertices.Count - 1; i < Vertices.Count; j = i++)
            {
                var a = Vertices[i];
                var b = Vertices[j];
                if ((a.Lat > lat) != (b.Lat > lat) &&
                    lng < (b.Lng - a.Lng) * (lat - a.Lat) / (b.Lat - a.Lat) + a.Lng)
                    inside = !inside;
            }

            return inside;
        }
    }

    /// <summary>
    /// Why a point of the plan is outside the fence the vehicle will hold to.
    /// </summary>
    [Flags]
    public enum ElevationGraphFenceBreach
    {
        None = 0,
        AboveCeiling = 1,

        /// <summary>below the floor once the path has been above it, which is when ArduPilot arms the floor</summary>
        BelowFloor = 2,

        /// <summary>further from home than FENCE_RADIUS</summary>
        OutsideCircle = 4,

        /// <summary>outside an inclusion polygon or circle (outside all of them with FENCE_OPTIONS bit 1)</summary>
        OutsideInclusion = 8,

        /// <summary>inside an exclusion polygon or circle</summary>
        InsideExclusion = 16
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

        /// <summary>null when not connected, or the vehicle has no such parameter.</summary>
        public ElevationGraphFenceCircle FenceCircle;

        /// <summary>The polygon fence: the inclusion and exclusion areas among the fence items.</summary>
        public List<ElevationGraphFenceArea> FenceAreas = new List<ElevationGraphFenceArea>();

        /// <summary>FENCE_TYPE has bit 2 (polygon); true when not known, as for a plan made offline.</summary>
        public bool FencePolygonUsed = true;

        /// <summary>FENCE_OPTIONS bit 1: inside any one inclusion area is enough, instead of inside all of them.</summary>
        public bool FenceInclusionUnion;

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
        /// The circle fence from the vehicle's parameters, or null when the vehicle has no
        /// FENCE_RADIUS (older Plane firmware). AC_Fence uses it when FENCE_TYPE has bit 1, and
        /// centres it on home.
        /// </summary>
        /// <param name="param">a parameter's value, or null when the vehicle does not have it</param>
        public static ElevationGraphFenceCircle FenceCircleOf(Func<string, double?> param)
        {
            var radius = param("FENCE_RADIUS");
            if (!radius.HasValue)
                return null;

            var type = param("FENCE_TYPE");
            var bits = type.HasValue ? (int) type.Value : 0;
            return new ElevationGraphFenceCircle {Radius = radius.Value, Used = (bits & 2) != 0};
        }

        /// <summary>
        /// The areas of the polygon fence among a list of plan items (the fence list, or the ALL
        /// list that mixes every type), as ArduPilot loads them: a polygon's vertices follow one
        /// another, each with the polygon's vertex count in param 1, and a circle has its radius in
        /// param 1. Polygons with fewer than three vertices and circles without a radius are left out.
        /// </summary>
        public static List<ElevationGraphFenceArea> FenceAreasOf(IList<Locationwp> items)
        {
            var areas = new List<ElevationGraphFenceArea>();
            ElevationGraphFenceArea polygon = null;
            var count = 0;
            foreach (var item in items)
            {
                var id = (MAVLink.MAV_CMD) item.id;
                if (id == MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION ||
                    id == MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION)
                {
                    var inclusion = id == MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION;
                    if (polygon == null || polygon.Inclusion != inclusion || polygon.Vertices.Count >= count)
                    {
                        count = (int) item.p1;
                        polygon = new ElevationGraphFenceArea {Inclusion = inclusion, Vertices = new List<PointLatLngAlt>()};
                        areas.Add(polygon);
                    }

                    polygon.Vertices.Add(new PointLatLngAlt(item.lat, item.lng));
                    continue;
                }

                polygon = null;
                if ((id == MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION || id == MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION) &&
                    item.p1 > 0)
                {
                    areas.Add(new ElevationGraphFenceArea
                    {
                        Inclusion = id == MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION,
                        Centre = new PointLatLngAlt(item.lat, item.lng),
                        Radius = item.p1
                    });
                }
            }

            areas.RemoveAll(a => a.Vertices != null && a.Vertices.Count < 3);
            return areas;
        }

        /// <summary>
        /// How the vehicle uses the polygon fence: FENCE_TYPE bit 2 (true when the vehicle has no
        /// FENCE_TYPE), and FENCE_OPTIONS bit 1, the union of the inclusion areas.
        /// </summary>
        /// <param name="param">a parameter's value, or null when the vehicle does not have it</param>
        public static void FencePolygonOptions(Func<string, double?> param, out bool used, out bool inclusionUnion)
        {
            var type = param("FENCE_TYPE");
            used = !type.HasValue || ((int) type.Value & 4) != 0;
            inclusionUnion = ((int) (param("FENCE_OPTIONS") ?? 0) & 2) != 0;
        }

        /// <summary>
        /// The vehicle will hold to its fence on this flight: the fence is on, or turns itself on
        /// at takeoff or arming. Not knowing the fence state counts as on.
        /// </summary>
        public static bool FenceActive(ElevationGraphFenceState? state)
        {
            switch (state)
            {
                case ElevationGraphFenceState.Disabled:
                case ElevationGraphFenceState.ReportOnly:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// The circle fence the vehicle will hold to on this flight, or null when there is none:
        /// FENCE_TYPE has the circle, the radius is above 0 and the fence is active
        /// (<see cref="FenceActive"/>).
        /// </summary>
        /// <param name="param">a parameter's value, or null when the vehicle does not have it</param>
        /// <param name="reportedEnabled">as for <see cref="FenceStateOf"/></param>
        public static ElevationGraphFenceCircle ActiveFenceCircle(Func<string, double?> param, bool? reportedEnabled)
        {
            var circle = FenceCircleOf(param);
            if (circle == null || !circle.Used || !(circle.Radius > 0))
                return null;
            return FenceActive(FenceStateOf(param, reportedEnabled)) ? circle : null;
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

            sb.Append('|');
            if (FenceCircle != null)
            {
                add(FenceCircle.Radius);
                sb.Append(FenceCircle.Used ? 'u' : '-');
            }

            sb.Append('|').Append(FencePolygonUsed ? 'u' : '-').Append(FenceInclusionUnion ? 'n' : '-');
            foreach (var area in FenceAreas)
            {
                sb.Append(area.Inclusion ? 'i' : 'e');
                if (area.Vertices == null)
                {
                    add(area.Centre.Lat);
                    add(area.Centre.Lng);
                    add(area.Radius);
                }
                else
                {
                    foreach (var vertex in area.Vertices)
                    {
                        add(vertex.Lat);
                        add(vertex.Lng);
                    }
                }

                sb.Append(';');
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

            /// <summary>How the point is outside the fence the vehicle will hold to; None when inside.</summary>
            public ElevationGraphFenceBreach Breach;
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

        public ElevationGraphFenceCircle FenceCircle { get; private set; }

        /// <summary>
        /// Metres along the path where it crosses the circle fence, in order; empty when the
        /// fence does not use the circle or home is not set.
        /// </summary>
        public List<double> FenceCircleCrossings { get; } = new List<double>();

        /// <summary>The polygon fence's areas.</summary>
        public List<ElevationGraphFenceArea> FenceAreas { get; private set; } = new List<ElevationGraphFenceArea>();

        public bool FencePolygonUsed { get; private set; } = true;

        public bool FenceInclusionUnion { get; private set; }

        /// <summary>
        /// The waypoints and rally points outside the fence the vehicle will hold to, waypoints
        /// first, in order; each says how in <see cref="Marker.Breach"/>.
        /// </summary>
        public List<Marker> OutsideFence { get; } = new List<Marker>();

        /// <summary>Whether the fence is on; null when not known.</summary>
        public ElevationGraphFenceState? FenceState { get; private set; }

        /// <summary>
        /// The vehicle will hold to <paramref name="limit"/> on this flight: the fence uses the
        /// limit, and the fence is on or turns itself on at takeoff or arming. Not knowing the
        /// fence state counts as on.
        /// </summary>
        public bool Enforces(ElevationGraphFenceLimit limit)
        {
            return limit != null && limit.Used && ElevationGraphInput.FenceActive(FenceState);
        }

        /// <summary>The vehicle will hold to the circle fence on this flight, as for a limit.</summary>
        public bool Enforces(ElevationGraphFenceCircle circle)
        {
            return circle != null && circle.Used && circle.Radius > 0 &&
                   ElevationGraphInput.FenceActive(FenceState);
        }

        /// <summary>The vehicle will hold to the polygon fence on this flight, as for a limit.</summary>
        public bool EnforcesPolygon
        {
            get { return FencePolygonUsed && FenceAreas.Count > 0 && ElevationGraphInput.FenceActive(FenceState); }
        }

        /// <summary>Some terrain lookups found no data (an SRTM tile still downloading, or none).</summary>
        public bool TerrainMissing { get; private set; }

        /// <summary>
        /// The smallest height above the terrain on airborne legs (of the rally points
        /// themselves when <see cref="RallyChain"/>), NaN when unknown.
        /// </summary>
        public double LowestClearance { get; private set; } = double.NaN;

        public double LowestClearanceDist { get; private set; } = double.NaN;

        /// <summary>
        /// There is no mission path, so the terrain runs along a line through the rally points
        /// instead, home first. Nothing flies along that line, so it has no path.
        /// </summary>
        public bool RallyChain { get; private set; }

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
                FenceCircle = input.FenceCircle,
                FenceAreas = input.FenceAreas,
                FencePolygonUsed = input.FencePolygonUsed,
                FenceInclusionUnion = input.FenceInclusionUnion,
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

            // Rally points sit on the path at its nearest point. Without a path (rally points
            // planned before the mission, or on their own) the terrain is laid out along a line
            // through them instead, home first, so each shows over the ground under it.
            profile.RallyChain = input.RallyPoints.Count > 0 && !nodes.Exists(a => a.Item != null);

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
                if (!profile.RallyChain)
                    PlaceOnPath(nodes, marker);
                profile.RallyPoints.Add(marker);
            }

            if (profile.RallyChain)
            {
                var chain = new List<Marker>(nodes);
                foreach (var marker in profile.RallyPoints)
                {
                    if (chain.Count > 0)
                    {
                        var prev = chain[chain.Count - 1];
                        marker.Dist = prev.Dist +
                                      new PointLatLngAlt(prev.Lat, prev.Lng).GetDistance(new PointLatLngAlt(marker.Lat, marker.Lng));
                    }

                    chain.Add(marker);
                }

                SampleLegs(profile, chain, terrain, maxSamples, false);

                // nothing flies between the points, so the clearance is the points' own
                foreach (var marker in profile.RallyPoints)
                    profile.NoteClearance(marker.Alt - marker.Terrain, marker.Dist);
            }
            else
            {
                SampleLegs(profile, nodes, terrain, maxSamples, true);

                foreach (var sample in profile.Samples)
                {
                    if (sample.Airborne)
                        profile.NoteClearance(sample.Alt - sample.Terrain, sample.Dist);
                }
            }

            // fence altitudes are measured from home
            if (input.FenceCeiling != null)
                profile.FenceCeilingAlt = homeAlt + input.FenceCeiling.Alt;
            if (input.FenceFloor != null)
                profile.FenceFloorAlt = homeAlt + input.FenceFloor.Alt;

            // the circle fence is centred on home
            var circle = input.FenceCircle;
            if (input.Home != null && circle != null && circle.Used && circle.Radius > 0)
                profile.FindCircleCrossings(input.Home, circle.Radius);

            profile.CheckFence(input.Home);
            return profile;
        }

        /// <summary>
        /// Note which waypoints and rally points are outside the fence the vehicle will hold to,
        /// and how, as ArduPilot checks it: above the ceiling; below the floor once the path has
        /// been above it (the floor arms itself only then, and rally points are reached from the
        /// air); further from home than the circle; outside an inclusion area (outside all of them
        /// with FENCE_OPTIONS bit 1) or inside an exclusion area. Home and landings are on the
        /// ground and checked for neither ceiling nor floor.
        /// </summary>
        private void CheckFence(PointLatLngAlt home)
        {
            var ceiling = Enforces(FenceCeiling);
            var floor = Enforces(FenceFloor);
            var circle = Enforces(FenceCircle) && home != null;
            var polygon = EnforcesPolygon;
            if (!ceiling && !floor && !circle && !polygon)
                return;

            var centre = home != null ? new PointLatLngAlt(home.Lat, home.Lng) : null;
            var inclusions = FenceAreas.FindAll(a => a.Inclusion);
            var exclusions = FenceAreas.FindAll(a => !a.Inclusion);
            Func<Marker, bool, ElevationGraphFenceBreach> check = (marker, floorArmed) =>
            {
                var breach = ElevationGraphFenceBreach.None;
                var onGround = marker.Item.OnGround;
                if (ceiling && !onGround && marker.Alt > FenceCeilingAlt)
                    breach |= ElevationGraphFenceBreach.AboveCeiling;
                if (floor && floorArmed && !onGround && marker.Alt < FenceFloorAlt)
                    breach |= ElevationGraphFenceBreach.BelowFloor;
                if (circle && centre.GetDistance(new PointLatLngAlt(marker.Lat, marker.Lng)) > FenceCircle.Radius)
                    breach |= ElevationGraphFenceBreach.OutsideCircle;
                if (polygon)
                {
                    var outside = inclusions.FindAll(a => !a.Contains(marker.Lat, marker.Lng)).Count;
                    if (FenceInclusionUnion ? outside > 0 && outside == inclusions.Count : outside > 0)
                        breach |= ElevationGraphFenceBreach.OutsideInclusion;
                    if (exclusions.Exists(a => a.Contains(marker.Lat, marker.Lng)))
                        breach |= ElevationGraphFenceBreach.InsideExclusion;
                }

                return breach;
            };

            var armed = false;
            foreach (var marker in Waypoints)
            {
                if (marker.Item == null)
                    continue;
                marker.Breach = check(marker, armed);
                if (marker.Breach != ElevationGraphFenceBreach.None)
                    OutsideFence.Add(marker);
                armed |= marker.Alt > FenceFloorAlt;
            }

            foreach (var marker in RallyPoints)
            {
                marker.Breach = check(marker, true);
                if (marker.Breach != ElevationGraphFenceBreach.None)
                    OutsideFence.Add(marker);
            }
        }

        /// <summary>
        /// Note where the line through the samples crosses the circle of <paramref name="radius"/>
        /// metres around <paramref name="centre"/>, going out or coming back in.
        /// </summary>
        private void FindCircleCrossings(PointLatLngAlt centre, double radius)
        {
            var middle = new PointLatLngAlt(centre.Lat, centre.Lng);
            Sample prev = null;
            var prevOver = 0.0;
            foreach (var sample in Samples)
            {
                // metres outside the circle, negative inside
                var over = middle.GetDistance(new PointLatLngAlt(sample.Lat, sample.Lng)) - radius;
                if (prev != null && (prevOver > 0) != (over > 0))
                {
                    // the signs differ, so the difference is not 0
                    var f = prevOver / (prevOver - over);
                    FenceCircleCrossings.Add(prev.Dist + (sample.Dist - prev.Dist) * f);
                }

                prev = sample;
                prevOver = over;
            }
        }

        /// <summary>
        /// Sample the terrain along the legs between <paramref name="nodes"/>, with the path's
        /// altitude at each sample when the legs are <paramref name="flown"/>: a leg into a
        /// terrain frame point follows the ground, its height above the ground changing evenly
        /// from one end to the other, as ArduPilot flies it; any other leg is a straight line
        /// between the altitudes of its ends. Legs that are not flown get the terrain alone.
        /// </summary>
        private static void SampleLegs(ElevationGraphProfile profile, List<Marker> nodes,
            Func<double, double, double> terrain, int maxSamples, bool flown)
        {
            profile.TotalDistance = nodes.Count > 0 ? nodes[nodes.Count - 1].Dist : 0;

            if (nodes.Count > 0)
            {
                var first = nodes[0];
                profile.Samples.Add(new Sample
                {
                    Lat = first.Lat,
                    Lng = first.Lng,
                    Terrain = first.Terrain,
                    Alt = flown ? first.Alt : double.NaN,
                    Airborne = flown && !IsOnGround(first)
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
                var airborne = flown && !IsOnGround(from) && !IsOnGround(to);

                for (int s = 1; s <= steps; s++)
                {
                    var f = (double) s / steps;
                    var lat = from.Lat + (to.Lat - from.Lat) * f;
                    var lng = from.Lng + (to.Lng - from.Lng) * f;
                    var ground = s == steps ? to.Terrain : terrain(lat, lng);
                    var alt = double.NaN;
                    if (flown)
                        alt = follow
                            ? ground + fromHeight + (to.Item.Alt - fromHeight) * f
                            : from.Alt + (to.Alt - from.Alt) * f;

                    profile.Samples.Add(new Sample
                    {
                        Dist = from.Dist + length * f,
                        Lat = lat,
                        Lng = lng,
                        Terrain = ground,
                        Alt = alt,
                        Airborne = airborne
                    });
                }
            }
        }

        private void NoteClearance(double clearance, double dist)
        {
            if (double.IsNaN(clearance))
                return;
            if (double.IsNaN(LowestClearance) || clearance < LowestClearance)
            {
                LowestClearance = clearance;
                LowestClearanceDist = dist;
            }
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
