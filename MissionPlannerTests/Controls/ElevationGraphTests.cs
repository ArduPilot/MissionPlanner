using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using GMap.NET.WindowsForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using ZedGraph;

namespace MissionPlanner.Controls.Tests
{
    /// <summary>
    /// The Plan page's elevation graph: which plan items it draws and how they are numbered
    /// (commands without a position, takeoff and landing in place, ALL mode's mixed grid), how
    /// altitude frames become heights above sea level, terrain following legs, the clearance
    /// check, rally points placed on the path, the fence limits and fence state read from the
    /// vehicle, and the drawing itself (rally points purple, fence lines named and greyed out with
    /// the fence off), and the fence circle on the graph and the map, drawn only while it is
    /// active. FlightPlanner's internal helpers
    /// are reached by reflection, which works with or without InternalsVisibleTo.
    /// </summary>
    [TestClass]
    public class ElevationGraphTests
    {
        // a made up position: round numbers, nowhere in particular
        private const double Lat0 = 10.0;
        private const double Lng0 = 20.0;

        /// <summary>metres in 0.01 degree of latitude, on the 6371 km sphere GetDistance uses</summary>
        private const double MetresPer001 = 6371000 * Math.PI / 180 * 0.01;

        private static Locationwp Item(MAVLink.MAV_CMD cmd, double lat, double lng, double alt,
            MAVLink.MAV_FRAME frame = MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT)
        {
            return new Locationwp {id = (ushort) cmd, lat = lat, lng = lng, alt = (float) alt, frame = (byte) frame};
        }

        private static ElevationGraphItem Wp(int row, double lat, double alt,
            ElevationGraphAltFrame frame = ElevationGraphAltFrame.Relative)
        {
            return new ElevationGraphItem
                {Row = row, Label = row.ToString(), Lat = lat, Lng = Lng0, Alt = alt, Frame = frame};
        }

        private static ElevationGraphInput WithHome(double homeAlt, params ElevationGraphItem[] waypoints)
        {
            return new ElevationGraphInput
                {Home = new PointLatLngAlt(Lat0, Lng0, homeAlt, "H"), Waypoints = waypoints.ToList()};
        }

        private static double? Flat(double lat, double lng)
        {
            return 100;
        }

        private static Func<string, double?> Params(params object[] nameThenValue)
        {
            var values = new Dictionary<string, double>();
            for (int i = 0; i < nameThenValue.Length; i += 2)
                values[(string) nameThenValue[i]] = Convert.ToDouble(nameThenValue[i + 1]);
            return name => values.ContainsKey(name) ? values[name] : (double?) null;
        }

        [TestMethod]
        public void WaypointsOf_NumbersRowsAndSkipsCommandsWithoutAPosition()
        {
            var items = new List<Locationwp>
            {
                Item(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 20), // 1: climbs where the vehicle is
                Item(MAVLink.MAV_CMD.WAYPOINT, Lat0 + 0.01, Lng0, 50), // 2
                Item(MAVLink.MAV_CMD.DO_CHANGE_SPEED, 0, 0, 0), // 3: no position
                Item(MAVLink.MAV_CMD.DO_SET_ROI, Lat0 + 0.02, Lng0, 0), // 4: aims the camera only
                Item(MAVLink.MAV_CMD.WAYPOINT, 0, 0, 50), // 5: 0, 0 is left out, as on the map
                Item(MAVLink.MAV_CMD.LOITER_TIME, Lat0 + 0.02, Lng0, 60), // 6
                Item(MAVLink.MAV_CMD.RETURN_TO_LAUNCH, 0, 0, 0), // 7
                Item(MAVLink.MAV_CMD.LAND, 0, 0, 0), // 8: comes down where the vehicle is
            };

            var points = ElevationGraphInput.WaypointsOf(items);

            CollectionAssert.AreEqual(new[] {"1", "2", "6", "8"}, points.Select(a => a.Label).ToArray());
            Assert.IsTrue(points[0].AtPreviousPosition);
            Assert.IsFalse(points[0].OnGround);
            Assert.IsFalse(points[1].AtPreviousPosition);
            Assert.IsTrue(points[3].AtPreviousPosition);
            Assert.IsTrue(points[3].OnGround);
            Assert.AreEqual(ElevationGraphAltFrame.Terrain, points[3].Frame);
            Assert.AreEqual(0, points[3].Alt);
        }

        [TestMethod]
        public void AllMode_MissionAndRallyRowsKeepTheirGridNumbers()
        {
            // one grid holding mission, fence and rally rows, as the dropdown's ALL shows them
            var grid = new List<Locationwp>
            {
                Item(MAVLink.MAV_CMD.WAYPOINT, Lat0 + 0.01, Lng0, 50), // 1
                Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.03, Lng0 - 0.01, 3), // 2
                Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.03, Lng0 + 0.01, 3), // 3
                Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 - 0.01, Lng0, 3), // 4
                Item(MAVLink.MAV_CMD.WAYPOINT, Lat0 + 0.02, Lng0, 60), // 5
                Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0 + 0.01, Lng0 + 0.005, 80), // 6
                Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0 + 0.02, Lng0 - 0.005, 90), // 7
            };

            var waypoints = ElevationGraphInput.WaypointsOf(grid);
            var rally = ElevationGraphInput.RallyPointsOf(grid);

            CollectionAssert.AreEqual(new[] {"1", "5"}, waypoints.Select(a => a.Label).ToArray());
            CollectionAssert.AreEqual(new[] {"R6", "R7"}, rally.Select(a => a.Label).ToArray());
            CollectionAssert.AreEqual(new[] {6, 7}, rally.Select(a => a.Row).ToArray());
            Assert.AreEqual(80, rally[0].Alt);
            Assert.AreEqual(ElevationGraphAltFrame.Relative, rally[0].Frame);
        }

        [TestMethod]
        public void Frames_BecomeHeightsAboveSeaLevel()
        {
            // terrain at 120 m everywhere, home at 100 m
            var input = WithHome(100,
                Wp(1, Lat0 + 0.01, 50), // relative
                Wp(2, Lat0 + 0.02, 300, ElevationGraphAltFrame.Absolute),
                Wp(3, Lat0 + 0.03, 40, ElevationGraphAltFrame.Terrain));
            var profile = ElevationGraphProfile.Build(input, (lat, lng) => 120);

            CollectionAssert.AreEqual(new[] {"H", "1", "2", "3"}, profile.Waypoints.Select(a => a.Label).ToArray());
            Assert.AreEqual(100, profile.Waypoints[0].Alt, 1e-9);
            Assert.AreEqual(150, profile.Waypoints[1].Alt, 1e-9);
            Assert.AreEqual(300, profile.Waypoints[2].Alt, 1e-9);
            Assert.AreEqual(160, profile.Waypoints[3].Alt, 1e-9);
            Assert.AreEqual(3 * MetresPer001, profile.TotalDistance, 0.01);
        }

        [TestMethod]
        public void TerrainFrameLeg_KeepsItsHeightAboveTheGround()
        {
            // the ground climbs 200 m over the second leg, with a hump in the middle
            Func<double, double, double?> ground = (lat, lng) =>
            {
                var f = Math.Max(0, Math.Min(1, (lat - (Lat0 + 0.01)) / 0.01));
                return 100 + 200 * f + 50 * Math.Sin(f * Math.PI);
            };
            var input = WithHome(100,
                Wp(1, Lat0 + 0.01, 60), // relative: 160 AMSL, 60 m over the ground there
                Wp(2, Lat0 + 0.02, 60, ElevationGraphAltFrame.Terrain));
            var profile = ElevationGraphProfile.Build(input, ground);

            var leg = profile.Samples.Where(s => s.Dist > MetresPer001 + 1).ToList();
            Assert.IsTrue(leg.Count > 10);
            foreach (var sample in leg)
                Assert.AreEqual(60, sample.Alt - sample.Terrain, 1e-6);
            Assert.AreEqual(360, profile.Waypoints[2].Alt, 1e-6);
        }

        [TestMethod]
        public void OtherLegs_AreStraightBetweenTheirEnds()
        {
            // the ground does not matter to a relative leg
            var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 150));
            var profile = ElevationGraphProfile.Build(input, (lat, lng) => 100 + 5000 * (lat - Lat0));

            var leg = profile.Samples.Where(s => s.Dist > MetresPer001 + 1).ToList();
            Assert.IsTrue(leg.Count > 10);
            foreach (var sample in leg)
                Assert.AreEqual(150 + 100 * (sample.Dist - MetresPer001) / MetresPer001, sample.Alt, 1e-6);
        }

        [TestMethod]
        public void TakeoffAndLandingInPlace_AreVertical()
        {
            var items = new List<Locationwp>
            {
                Item(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 30),
                Item(MAVLink.MAV_CMD.WAYPOINT, Lat0 + 0.01, Lng0, 30),
                Item(MAVLink.MAV_CMD.LAND, 0, 0, 0),
            };
            var input = new ElevationGraphInput
            {
                Home = new PointLatLngAlt(Lat0, Lng0, 100, "H"),
                Waypoints = ElevationGraphInput.WaypointsOf(items)
            };
            // the ground under the waypoint is 20 m higher than home
            var profile = ElevationGraphProfile.Build(input, (lat, lng) => lat > Lat0 + 0.005 ? 120 : 100);

            var takeoff = profile.Waypoints[1];
            Assert.AreEqual(0, takeoff.Dist);
            Assert.AreEqual(130, takeoff.Alt, 1e-9);
            var land = profile.Waypoints[3];
            Assert.AreEqual(MetresPer001, land.Dist, 0.01);
            Assert.AreEqual(120, land.Alt, 1e-9); // down to the ground under the vehicle

            // a climb at the start and a descent at the end, both straight up and down
            Assert.AreEqual(0, profile.Samples[1].Dist);
            Assert.AreEqual(130, profile.Samples[1].Alt, 1e-9);
            var last = profile.Samples[profile.Samples.Count - 1];
            Assert.AreEqual(land.Dist, last.Dist, 1e-9);
            Assert.AreEqual(120, last.Alt, 1e-9);
        }

        [TestMethod]
        public void Clearance_IsCheckedOnlyBetweenPointsInTheAir()
        {
            // home 5 m under the terrain data there (GPS altitude against SRTM), mission at 50 m
            var input = WithHome(95, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 50));
            var profile = ElevationGraphProfile.Build(input, Flat);

            // the climb out of home starts under the terrain data, which is not a warning
            Assert.IsTrue(profile.Samples[0].Alt < profile.Samples[0].Terrain);
            Assert.IsFalse(profile.Samples.Any(s => s.Airborne && s.Alt < s.Terrain));
            Assert.AreEqual(45, profile.LowestClearance, 1e-9);
        }

        [TestMethod]
        public void Clearance_FindsARidgeBetweenWaypoints()
        {
            // an 80 m ridge halfway along the second leg, which flies at 150 m AMSL
            Func<double, double, double?> ridge = (lat, lng) => Math.Abs(lat - (Lat0 + 0.015)) < 0.001 ? 180 : 100;
            var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 50));
            var profile = ElevationGraphProfile.Build(input, ridge);

            Assert.AreEqual(-30, profile.LowestClearance, 1e-9);
            Assert.AreEqual(1.5 * MetresPer001, profile.LowestClearanceDist, 0.15 * MetresPer001);
        }

        [TestMethod]
        public void RallyPoint_SitsAtTheNearestPointOfThePath()
        {
            var input = WithHome(100, Wp(1, Lat0 + 0.02, 50));
            // east of the middle of the path
            input.RallyPoints.Add(new ElevationGraphItem
                {Row = 1, Label = "R1", Lat = Lat0 + 0.01, Lng = Lng0 + 0.005, Alt = 80});
            var profile = ElevationGraphProfile.Build(input, Flat);

            var rally = profile.RallyPoints.Single();
            Assert.AreEqual(MetresPer001, rally.Dist, 1);
            var east = new PointLatLngAlt(Lat0 + 0.01, Lng0).GetDistance(new PointLatLngAlt(Lat0 + 0.01, Lng0 + 0.005));
            Assert.AreEqual(east, rally.OffPath, east * 0.01);
            Assert.AreEqual(180, rally.Alt, 1e-9); // 80 m above home
            Assert.AreEqual(80, rally.Alt - rally.Terrain, 1e-9);
        }

        /// <summary>
        /// Home, waypoint 1 north of it, spline waypoints 2 (further north) and 3 (east of 2), and
        /// waypoint 4 back south of 3.
        /// </summary>
        private static ElevationGraphInput SplineMission()
        {
            var input = WithHome(100,
                Wp(1, Lat0 + 0.01, 50),
                Wp(2, Lat0 + 0.02, 50),
                Wp(3, Lat0 + 0.02, 50),
                Wp(4, Lat0 + 0.01, 50));
            input.Waypoints[2].Lng = Lng0 + 0.01;
            input.Waypoints[3].Lng = Lng0 + 0.01;
            input.Waypoints[1].Spline = true;
            input.Waypoints[2].Spline = true;
            return input;
        }

        [TestMethod]
        public void SplineLegs_FollowTheCurveTheMapDraws()
        {
            // spline legs were sampled along the straight line between the waypoints, while the
            // map draws the curve Spline2 flies (PR 3813, third review)
            var input = SplineMission();
            var profile = ElevationGraphProfile.Build(input, Flat);
            var nodes = profile.Waypoints; // H, 1, 2, 3, 4

            // the map's curve over the same points: the run 2, 3 with 1 before it, 4 after it, and
            // home before 1 setting the direction it starts in
            Func<ElevationGraphProfile.Marker, PointLatLngAlt> at = m => new PointLatLngAlt(m.Lat, m.Lng, m.Alt, "");
            var legs = MissionPlanner.ArduPilot.WPOverlay.SplineLegs(input.Home, at(nodes[0]),
                new List<PointLatLngAlt> {at(nodes[1]), at(nodes[2]), at(nodes[3]), at(nodes[4])});
            Assert.IsNull(nodes[1].Route);
            CollectionAssert.AreEqual(legs[0].Select(p => p.Lng).ToArray(), nodes[2].Route.Select(p => p.Lng).ToArray());
            CollectionAssert.AreEqual(legs[1].Select(p => p.Lat).ToArray(), nodes[3].Route.Select(p => p.Lat).ToArray());
            Assert.IsNull(nodes[4].Route);

            // the leg into 2 bends away from the straight line north from 1, and the path is
            // longer than the straight legs
            Func<ElevationGraphProfile.Sample, double> offLine = s =>
                new PointLatLngAlt(s.Lat, Lng0).GetDistance(new PointLatLngAlt(s.Lat, s.Lng));
            var leg = profile.Samples.Where(s => s.Dist > nodes[1].Dist && s.Dist < nodes[2].Dist).ToList();
            Assert.IsTrue(leg.Count > 10);
            var widest = leg.OrderByDescending(offLine).First();
            Assert.IsTrue(offLine(widest) > 20, "the curve swings off the straight line: " + offLine(widest));
            var straight = 0.0;
            for (int i = 1; i < nodes.Count; i++)
                straight += new PointLatLngAlt(nodes[i - 1].Lat, nodes[i - 1].Lng)
                    .GetDistance(new PointLatLngAlt(nodes[i].Lat, nodes[i].Lng));
            Assert.IsTrue(profile.TotalDistance > straight + 1);
            Assert.AreEqual(nodes[4].Dist, profile.TotalDistance, 1e-6);
            Assert.AreEqual(nodes[2].Lat, profile.Samples.Last(s => s.Dist <= nodes[2].Dist + 1e-6).Lat, 1e-9);

            // a rally point on the curve is on the path, not off the straight line
            input.RallyPoints.Add(new ElevationGraphItem
                {Row = 1, Label = "R1", Lat = widest.Lat, Lng = widest.Lng, Alt = 80});
            var rally = ElevationGraphProfile.Build(input, Flat).RallyPoints.Single();
            Assert.IsTrue(rally.OffPath < 1, "off the curve by " + rally.OffPath);
            Assert.AreEqual(widest.Dist, rally.Dist, 1);
        }

        [TestMethod]
        public void SplineWaypoints_AreMarkedAndChangeTheSignature()
        {
            var items = new List<Locationwp>
            {
                Item(MAVLink.MAV_CMD.SPLINE_WAYPOINT, Lat0 + 0.01, Lng0, 50),
                Item(MAVLink.MAV_CMD.WAYPOINT, Lat0 + 0.02, Lng0, 50)
            };
            var waypoints = ElevationGraphInput.WaypointsOf(items);
            Assert.IsTrue(waypoints[0].Spline);
            Assert.IsFalse(waypoints[1].Spline);

            var spline = SplineMission();
            var straight = SplineMission();
            straight.Waypoints[1].Spline = false;
            Assert.AreNotEqual(spline.Signature(), straight.Signature());
        }

        [TestMethod]
        public void FenceLimits_FromAcFence_FollowFenceType()
        {
            ElevationGraphFenceLimit ceiling, floor;

            // Copter's default type: maximum altitude, circle, polygon
            ElevationGraphInput.FenceAltLimits(Params("FENCE_ALT_MAX", 120, "FENCE_ALT_MIN", -10, "FENCE_TYPE", 7),
                out ceiling, out floor);
            Assert.AreEqual(120, ceiling.Alt);
            Assert.IsTrue(ceiling.Used);
            Assert.AreEqual(-10, floor.Alt);
            Assert.IsFalse(floor.Used);

            // with the minimum altitude (bit 3)
            ElevationGraphInput.FenceAltLimits(Params("FENCE_ALT_MAX", 120, "FENCE_ALT_MIN", 15, "FENCE_TYPE", 15),
                out ceiling, out floor);
            Assert.IsTrue(ceiling.Used);
            Assert.IsTrue(floor.Used);

            // polygon only
            ElevationGraphInput.FenceAltLimits(Params("FENCE_ALT_MAX", 100, "FENCE_ALT_MIN", -10, "FENCE_TYPE", 4),
                out ceiling, out floor);
            Assert.IsFalse(ceiling.Used);
            Assert.IsFalse(floor.Used);
        }

        [TestMethod]
        public void FenceLimits_FromOlderPlaneParameters()
        {
            ElevationGraphFenceLimit ceiling, floor;

            ElevationGraphInput.FenceAltLimits(Params("FENCE_MAXALT", 150, "FENCE_MINALT", 0), out ceiling, out floor);
            Assert.AreEqual(150, ceiling.Alt);
            Assert.IsTrue(ceiling.Used);
            Assert.AreEqual(0, floor.Alt);
            Assert.IsFalse(floor.Used); // 0 turns it off

            ElevationGraphInput.FenceAltLimits(Params("FENCE_MAXALT", 150, "FENCE_MINALT", 30), out ceiling, out floor);
            Assert.IsTrue(ceiling.Used);
            Assert.IsTrue(floor.Used);

            // a maximum that is not above the minimum turns both off
            ElevationGraphInput.FenceAltLimits(Params("FENCE_MAXALT", 30, "FENCE_MINALT", 50), out ceiling, out floor);
            Assert.IsFalse(ceiling.Used);
            Assert.IsFalse(floor.Used);
        }

        [TestMethod]
        public void FenceLimits_NoneWithoutFenceParameters()
        {
            ElevationGraphFenceLimit ceiling, floor;
            ElevationGraphInput.FenceAltLimits(Params("WPNAV_SPEED", 500), out ceiling, out floor);
            Assert.IsNull(ceiling);
            Assert.IsNull(floor);
        }

        [TestMethod]
        public void FenceLimits_AreMeasuredFromHome()
        {
            var input = WithHome(584, Wp(1, Lat0 + 0.01, 50));
            input.FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = true};
            input.FenceFloor = new ElevationGraphFenceLimit {Alt = 15, Used = true};
            var profile = ElevationGraphProfile.Build(input, Flat);

            Assert.AreEqual(704, profile.FenceCeilingAlt, 1e-9);
            Assert.AreEqual(599, profile.FenceFloorAlt, 1e-9);
        }

        [TestMethod]
        public void WithoutHome_RelativeHeightsAndTheFenceAreLeftOut()
        {
            var input = new ElevationGraphInput
            {
                Waypoints = {Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 400, ElevationGraphAltFrame.Absolute)},
                FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = true}
            };
            var profile = ElevationGraphProfile.Build(input, Flat);

            Assert.IsFalse(profile.HomeSet);
            CollectionAssert.AreEqual(new[] {"1", "2"}, profile.Waypoints.Select(a => a.Label).ToArray());
            Assert.IsTrue(double.IsNaN(profile.Waypoints[0].Alt));
            Assert.AreEqual(400, profile.Waypoints[1].Alt, 1e-9);
            Assert.IsTrue(double.IsNaN(profile.FenceCeilingAlt));
            Assert.AreEqual(MetresPer001, profile.TotalDistance, 0.01);
        }

        [TestMethod]
        public void MissingTerrain_IsNotedAndLeftOutOfTheClearance()
        {
            // no terrain yet north of the first waypoint (its tile still downloading)
            Func<double, double, double?> partial = (lat, lng) => lat > Lat0 + 0.0105 ? (double?) null : 100;
            var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 50));
            var profile = ElevationGraphProfile.Build(input, partial);

            Assert.IsTrue(profile.TerrainMissing);
            var last = profile.Samples[profile.Samples.Count - 1];
            Assert.IsTrue(double.IsNaN(last.Terrain));
            Assert.AreEqual(150, last.Alt, 1e-9); // a relative leg does not need the terrain
            Assert.AreEqual(50, profile.LowestClearance, 1e-9);
        }

        [TestMethod]
        public void PositionAt_FollowsThePath()
        {
            var input = WithHome(100, Wp(1, Lat0 + 0.01, 50),
                new ElevationGraphItem {Row = 2, Label = "2", Lat = Lat0 + 0.01, Lng = Lng0 + 0.01, Alt = 50});
            var profile = ElevationGraphProfile.Build(input, Flat);

            var halfway = profile.PositionAt(MetresPer001 / 2);
            Assert.AreEqual(Lat0 + 0.005, halfway.Lat, 1e-6);
            Assert.AreEqual(Lng0, halfway.Lng, 1e-9);
            Assert.AreEqual(Lng0 + 0.01, profile.PositionAt(profile.TotalDistance + 100).Lng, 1e-9);
            Assert.AreEqual(Lat0, profile.PositionAt(-5).Lat, 1e-9);
        }

        [TestMethod]
        public void Signature_ChangesWithTheInput()
        {
            Func<ElevationGraphInput> plan = () => WithHome(100, Wp(1, Lat0 + 0.01, 50));
            Assert.AreEqual(plan().Signature(), plan().Signature());

            var higher = plan();
            higher.Waypoints[0].Alt = 51;
            Assert.AreNotEqual(plan().Signature(), higher.Signature());

            var fenced = plan();
            fenced.FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = true};
            Assert.AreNotEqual(plan().Signature(), fenced.Signature());

            var off = plan();
            off.FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = false};
            Assert.AreNotEqual(fenced.Signature(), off.Signature());

            // the fence switched on or off on the vehicle redraws the graph
            var on = plan();
            on.FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = true};
            on.FenceState = ElevationGraphFenceState.Enabled;
            var switchedOff = plan();
            switchedOff.FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = true};
            switchedOff.FenceState = ElevationGraphFenceState.Disabled;
            Assert.AreNotEqual(on.Signature(), switchedOff.Signature());
        }

        [TestMethod]
        public void FenceState_FollowsWhatTheVehicleReports()
        {
            // SYS_STATUS wins: an RC switch or a command turns the fence on or off without
            // touching FENCE_ENABLE
            Assert.AreEqual(ElevationGraphFenceState.Enabled,
                ElevationGraphInput.FenceStateOf(Params("FENCE_ENABLE", 0, "FENCE_ACTION", 1), true));
            Assert.AreEqual(ElevationGraphFenceState.Disabled,
                ElevationGraphInput.FenceStateOf(Params("FENCE_ENABLE", 1, "FENCE_ACTION", 1), false));

            // no SYS_STATUS report yet: FENCE_ENABLE says
            Assert.AreEqual(ElevationGraphFenceState.Enabled,
                ElevationGraphInput.FenceStateOf(Params("FENCE_ENABLE", 1, "FENCE_ACTION", 1), null));
            Assert.AreEqual(ElevationGraphFenceState.Disabled,
                ElevationGraphInput.FenceStateOf(Params("FENCE_ENABLE", 0, "FENCE_ACTION", 1), null));
        }

        [TestMethod]
        public void FenceState_ReportOnlyAndAutoEnable()
        {
            // FENCE_ACTION 0 on AC_Fence only reports, and SYS_STATUS shows such a fence as off
            Assert.AreEqual(ElevationGraphFenceState.ReportOnly,
                ElevationGraphInput.FenceStateOf(Params("FENCE_ENABLE", 1, "FENCE_ACTION", 0), false));
            // however the fence comes on: turned on by auto-enable it still only reports, and a
            // fence that never comes on is off (PR 3813, third review)
            Assert.AreEqual(ElevationGraphFenceState.ReportOnly, ElevationGraphInput.FenceStateOf(
                Params("FENCE_ENABLE", 0, "FENCE_ACTION", 0, "FENCE_AUTOENABLE", 3), false));
            Assert.AreEqual(ElevationGraphFenceState.Disabled, ElevationGraphInput.FenceStateOf(
                Params("FENCE_ENABLE", 0, "FENCE_ACTION", 0, "FENCE_AUTOENABLE", 0), null));

            Assert.AreEqual(ElevationGraphFenceState.EnablesAfterTakeoff, ElevationGraphInput.FenceStateOf(
                Params("FENCE_ENABLE", 0, "FENCE_ACTION", 1, "FENCE_AUTOENABLE", 1), false));
            Assert.AreEqual(ElevationGraphFenceState.EnablesAfterTakeoff, ElevationGraphInput.FenceStateOf(
                Params("FENCE_ENABLE", 0, "FENCE_ACTION", 1, "FENCE_AUTOENABLE", 2), false));
            Assert.AreEqual(ElevationGraphFenceState.EnablesWhenArmed, ElevationGraphInput.FenceStateOf(
                Params("FENCE_ENABLE", 0, "FENCE_ACTION", 1, "FENCE_AUTOENABLE", 3), false));

            // older Plane has no FENCE_ENABLE: its FENCE_ACTION 0 turned geofencing off and 2 only
            // reported (ArduPlane geofence.cpp, removed in 2020 by aae092b88f)
            Assert.AreEqual(ElevationGraphFenceState.Disabled,
                ElevationGraphInput.FenceStateOf(Params("FENCE_ACTION", 0, "FENCE_MAXALT", 150), null));
            Assert.AreEqual(ElevationGraphFenceState.ReportOnly,
                ElevationGraphInput.FenceStateOf(Params("FENCE_ACTION", 2, "FENCE_MAXALT", 150), null));
            Assert.AreEqual(ElevationGraphFenceState.Enabled,
                ElevationGraphInput.FenceStateOf(Params("FENCE_ACTION", 1, "FENCE_MAXALT", 150), null));
        }

        [TestMethod]
        public void Enforces_NeedsTheLimitUsedAndTheFenceOn()
        {
            var used = new ElevationGraphFenceLimit {Alt = 120, Used = true};
            var unused = new ElevationGraphFenceLimit {Alt = 120, Used = false};
            Func<ElevationGraphFenceState?, ElevationGraphProfile> withState = state =>
            {
                var input = WithHome(100, Wp(1, Lat0 + 0.01, 50));
                input.FenceState = state;
                return ElevationGraphProfile.Build(input, Flat);
            };

            Assert.IsTrue(withState(ElevationGraphFenceState.Enabled).Enforces(used));
            Assert.IsFalse(withState(ElevationGraphFenceState.Enabled).Enforces(unused));
            Assert.IsFalse(withState(ElevationGraphFenceState.Disabled).Enforces(used));
            Assert.IsFalse(withState(ElevationGraphFenceState.ReportOnly).Enforces(used));
            Assert.IsTrue(withState(ElevationGraphFenceState.EnablesAfterTakeoff).Enforces(used));
            Assert.IsTrue(withState(ElevationGraphFenceState.EnablesWhenArmed).Enforces(used));
            Assert.IsTrue(withState(null).Enforces(used)); // not known: as before, by FENCE_TYPE alone
        }

        [TestMethod]
        public void Graph_DrawsRallyPointsPurpleAndNamesTheFenceLines()
        {
            var altScale = CurrentState.multiplieralt;
            var altUnit = CurrentState.AltUnit;
            CurrentState.multiplieralt = 1;
            CurrentState.AltUnit = "m";
            try
            {
                var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 50));
                input.RallyPoints.Add(new ElevationGraphItem
                    {Row = 1, Label = "R1", Lat = Lat0 + 0.015, Lng = Lng0 + 0.003, Alt = 80});
                input.FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = true};
                input.FenceFloor = new ElevationGraphFenceLimit {Alt = 10, Used = false};
                input.FenceState = ElevationGraphFenceState.Enabled;
                var profile = ElevationGraphProfile.Build(input, Flat);

                using (var view = new PlanElevationGraph())
                {
                    view.ShowProfile(profile, false);
                    var pane = ((ZedGraphControl) typeof(PlanElevationGraph)
                        .GetField("graph", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(view)).GraphPane;

                    var rally = pane.CurveList.Single(c => c.Label.Text == Strings.ElevationGraphRallyPoints);
                    Assert.AreEqual(PlanElevationGraph.RallyColor, rally.Color);
                    Assert.AreEqual(1, rally.Points.Count);
                    Assert.AreEqual(180, rally.Points[0].Y, 1e-9);

                    var waypoints = pane.CurveList.Single(c => c.Label.Text == Strings.ElevationGraphWaypoints);
                    Assert.AreNotEqual(PlanElevationGraph.RallyColor, waypoints.Color);
                    Assert.AreEqual(3, waypoints.Points.Count); // H, 1, 2

                    var ceilingName = string.Format(Strings.ElevationGraphFenceCeiling, "120 m");
                    var floorName = string.Format(Strings.ElevationGraphFenceNote,
                        string.Format(Strings.ElevationGraphFenceFloor, "10 m"), Strings.ElevationGraphFenceNotInType);
                    var ceiling = (LineItem) pane.CurveList.Single(c => c.Label.Text == ceilingName);
                    Assert.AreEqual(220, ceiling.Points[0].Y, 1e-9); // home 100 m + 120 m
                    Assert.AreEqual(Color.Red, ceiling.Color);
                    var floor = (LineItem) pane.CurveList.Single(c => c.Label.Text == floorName);
                    Assert.AreEqual(110, floor.Points[0].Y, 1e-9);
                    Assert.AreEqual(Color.Gray, floor.Color); // FENCE_TYPE leaves it out

                    var texts = pane.GraphObjList.OfType<TextObj>().Select(t => t.Text).ToList();
                    CollectionAssert.Contains(texts, ceilingName);
                    CollectionAssert.Contains(texts, floorName);
                    CollectionAssert.Contains(texts, "R1");
                    CollectionAssert.Contains(texts, "H");

                    // this ZedGraph writes Min/Max/Mean into the label of each line it draws, so the
                    // legend shows empty stand-ins for the lines, in this order
                    var legend = pane.CurveList.Where(c => c.Label.IsVisible).ToList();
                    CollectionAssert.AreEqual(new[]
                    {
                        Strings.ElevationGraphTerrain, Strings.ElevationGraphPath, Strings.ElevationGraphWaypoints,
                        Strings.ElevationGraphRallyPoints
                    }, legend.Select(c => c.Label.Text).ToArray());
                    Assert.IsTrue(legend.OfType<LineItem>().Where(c => c.Line.IsVisible).All(c => c.Points.Count == 0));
                }
            }
            finally
            {
                CurrentState.multiplieralt = altScale;
                CurrentState.AltUnit = altUnit;
            }
        }

        [TestMethod]
        public void Graph_FollowsTheFenceState()
        {
            var altScale = CurrentState.multiplieralt;
            var altUnit = CurrentState.AltUnit;
            CurrentState.multiplieralt = 1;
            CurrentState.AltUnit = "m";
            try
            {
                using (var view = new PlanElevationGraph())
                {
                    var pane = ((ZedGraphControl) typeof(PlanElevationGraph)
                        .GetField("graph", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(view)).GraphPane;
                    var fence = (System.Windows.Forms.Control) typeof(PlanElevationGraph)
                        .GetField("fence", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view);

                    var ceilingName = string.Format(Strings.ElevationGraphFenceCeiling, "120 m");
                    var floor = string.Format(Strings.ElevationGraphFenceFloor, "20 m");
                    var floorArmed = string.Format(Strings.ElevationGraphFenceNote, floor,
                        Strings.ElevationGraphFenceFloorArms);
                    Func<string, LineItem> line = name => (LineItem) pane.CurveList.Single(c => c.Label.Text == name);

                    Action<ElevationGraphFenceState?> show = state =>
                    {
                        var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 50));
                        input.FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = true};
                        input.FenceFloor = new ElevationGraphFenceLimit {Alt = 20, Used = true};
                        input.FenceState = state;
                        view.ShowProfile(ElevationGraphProfile.Build(input, Flat), false);
                    };

                    // on: both limits held to, the floor once the vehicle is above it
                    show(ElevationGraphFenceState.Enabled);
                    Assert.AreEqual(Strings.ElevationGraphFenceEnabled, fence.Text);
                    Assert.AreEqual(Color.Red, line(ceilingName).Color);
                    Assert.AreEqual(Color.Red, line(floorArmed).Color);

                    // switched off on the vehicle: the same limits, greyed out
                    show(ElevationGraphFenceState.Disabled);
                    Assert.AreEqual(Strings.ElevationGraphFenceDisabled, fence.Text);
                    Assert.AreEqual(Color.Gray, line(ceilingName).Color);
                    Assert.AreEqual(Color.Gray, line(floor).Color);
                    // by name: DashStyle exists in System.Drawing and MissionPlanner.Drawing alike
                    Assert.AreEqual("Dot", line(floor).Line.Style.ToString());

                    // off now, but on by itself after takeoff: drawn as held to
                    show(ElevationGraphFenceState.EnablesAfterTakeoff);
                    Assert.AreEqual(Strings.ElevationGraphFenceAfterTakeoff, fence.Text);
                    Assert.AreEqual(Color.Red, line(ceilingName).Color);

                    show(ElevationGraphFenceState.ReportOnly);
                    Assert.AreEqual(Strings.ElevationGraphFenceReportOnly, fence.Text);
                    Assert.AreEqual(Color.Gray, line(ceilingName).Color);

                    // not connected: no state to show
                    show(null);
                    Assert.AreEqual("", fence.Text);
                }
            }
            finally
            {
                CurrentState.multiplieralt = altScale;
                CurrentState.AltUnit = altUnit;
            }
        }

        [TestMethod]
        public void Graph_MarksThePathWhereItIsUnderTheTerrain()
        {
            Func<double, double, double?> ridge = (lat, lng) => Math.Abs(lat - (Lat0 + 0.015)) < 0.001 ? 180 : 100;
            var profile = ElevationGraphProfile.Build(
                WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 50)), ridge);

            using (var view = new PlanElevationGraph())
            {
                view.ShowProfile(profile, false);
                var pane = ((ZedGraphControl) typeof(PlanElevationGraph)
                    .GetField("graph", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(view)).GraphPane;

                var below = pane.CurveList.Single(c => c.Label.Text == Strings.ElevationGraphBelowTerrain &&
                                                       c.Points.Count > 0);
                var marked = Enumerable.Range(0, below.Points.Count).Select(i => below.Points[i])
                    .Where(p => p.Y != PointPair.Missing).ToList();
                Assert.IsTrue(marked.Count > 10);
                // the path, at 150 m, under the 180 m ridge
                Assert.IsTrue(marked.All(p => Math.Abs(p.Y - 150 * CurrentState.multiplieralt) < 1e-6));
            }
        }

        [TestMethod]
        public void Graph_MarksASingleSampleUnderTheTerrain()
        {
            // the ground rises above the level path under one sample only: the red line runs from
            // where the path goes into the ground to where it comes out, where one point alone drew
            // nothing (PR 3813, third review)
            var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 50));
            var leg = ElevationGraphProfile.Build(input, Flat).Samples.Where(s => s.Dist > MetresPer001 + 1).ToList();
            var spikeLat = leg[leg.Count / 2].Lat;
            Func<double, double, double?> spike = (lat, lng) => Math.Abs(lat - spikeLat) < 1e-9 ? 180 : 100;
            var profile = ElevationGraphProfile.Build(input, spike);
            Assert.AreEqual(1, profile.Samples.Count(s => s.Airborne && s.Alt < s.Terrain));

            using (var view = new PlanElevationGraph())
            {
                view.ShowProfile(profile, false);
                var pane = ((ZedGraphControl) typeof(PlanElevationGraph)
                    .GetField("graph", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(view)).GraphPane;

                var below = pane.CurveList.Single(c => c.Label.Text == Strings.ElevationGraphBelowTerrain &&
                                                       c.Points.Count > 0);
                var marked = Enumerable.Range(0, below.Points.Count).Select(i => below.Points[i])
                    .Where(p => p.Y != PointPair.Missing).ToList();
                Assert.AreEqual(3, marked.Count, "into the ground, the sample, out of the ground");
                Assert.IsTrue(marked[0].X < marked[1].X && marked[1].X < marked[2].X);
                // the path is level at 150 m, so it meets the ground at 150 m on both sides
                Assert.AreEqual(150 * CurrentState.multiplieralt, marked[0].Y, 1e-6);
                Assert.AreEqual(150 * CurrentState.multiplieralt, marked[2].Y, 1e-6);
            }
        }

        [TestMethod]
        public void Graph_WithNothingToMeasure_SaysThereAreNoWaypoints()
        {
            using (var view = new PlanElevationGraph())
            {
                var summary = (System.Windows.Forms.Control) typeof(PlanElevationGraph)
                    .GetField("summary", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view);

                view.ShowProfile(ElevationGraphProfile.Build(new ElevationGraphInput(), Flat), false);
                Assert.AreEqual(Strings.ElevationGraphNoWaypoints, summary.Text);

                // rally points with no mission make no path either, but they are measured on
                // their own line (Graph_RallyPointsAlone_SaysHowManyAndTheirClearance)
                var rallyOnly = WithHome(100);
                rallyOnly.RallyPoints.Add(new ElevationGraphItem
                    {Row = 1, Label = "R1", Lat = Lat0 + 0.01, Lng = Lng0, Alt = 80});
                view.ShowProfile(ElevationGraphProfile.Build(rallyOnly, Flat), false);
                Assert.AreNotEqual(Strings.ElevationGraphNoWaypoints, summary.Text);
                StringAssert.StartsWith(summary.Text, Strings.ElevationGraphRallySummary.Split('{')[0]);

                // relative heights without home: nothing to place, but no error either
                view.ShowProfile(ElevationGraphProfile.Build(new ElevationGraphInput
                {
                    Waypoints = {Wp(1, Lat0 + 0.01, 50)}
                }, Flat), true);
            }
        }

        [TestMethod]
        public void PlanPage_TakesTheGridHomeAndTheVehiclesRallyPoints()
        {
            var fp = new FlightPlanner();
            fp.quickadd = true; // no map redraw
            fp.TXT_homelat.Text = Lat0.ToString();
            fp.TXT_homelng.Text = Lng0.ToString();
            fp.TXT_homealt.Text = "584";
            AddRow(fp, "TAKEOFF", 0, 0, 30);
            AddRow(fp, "WAYPOINT", Lat0 + 0.01, Lng0, 100);
            AddRow(fp, "DO_CHANGE_SPEED", 0, 0, 0);
            AddRow(fp, "WAYPOINT", Lat0 + 0.02, Lng0, 120);

            // the dropdown is on MISSION, so the rally points come from the connected vehicle
            using (ConnectedVehicle())
            {
                MainV2.comPort.MAV.rallypoints[0] = Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0 + 0.015, Lng0 + 0.002, 80);
                var input = GraphInput(fp);

                Assert.AreEqual(584, input.Home.Alt, 1e-9);
                CollectionAssert.AreEqual(new[] {"1", "2", "4"}, input.Waypoints.Select(a => a.Label).ToArray());
                Assert.IsTrue(input.Waypoints[0].AtPreviousPosition);
                Assert.AreEqual(120, input.Waypoints[2].Alt, 1e-6);
                CollectionAssert.AreEqual(new[] {"R1"}, input.RallyPoints.Select(a => a.Label).ToArray());
                Assert.AreEqual(Lat0 + 0.015, input.RallyPoints[0].Lat, 1e-6);
                Assert.IsNull(input.FenceCeiling); // the vehicle has no fence parameters
            }
        }

        /// <summary>
        /// Connects MainV2.comPort to a vehicle until disposed: the Plan page reads the vehicle's
        /// mission, fence and rally copies only while connected.
        /// </summary>
        private static IDisposable ConnectedVehicle()
        {
            var previous = MainV2.comPort;
            var link = new OpenLink();
            link.Mav.sysidcurrent = OpenLink.SysId;
            link.Mav.compidcurrent = OpenLink.CompId;
            MainV2.comPort = link.Mav;
            return new Undo(() =>
            {
                MainV2.comPort = previous;
                link.Dispose();
            });
        }

        /// <summary>
        /// A link that is open with nothing on it, set to an ArduPilot vehicle 1, 1: enough for the
        /// Plan page to take a vehicle as connected and to read its parameters and its copies of the
        /// mission, fence and rally points, which the tests fill in themselves.
        /// </summary>
        private sealed class OpenLink : IDisposable
        {
            public const byte SysId = 1;
            public const byte CompId = 1;

            public readonly MAVLinkInterface Mav = new MAVLinkInterface {BaseStream = new SilentPort()};

            public OpenLink()
            {
                Mav.sysidcurrent = SysId;
                Mav.compidcurrent = CompId;
                Mav.MAV.apname = MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA;
            }

            public void Dispose()
            {
                Mav.BaseStream.Close();
            }
        }

        /// <summary>A port that is open, takes what is written and never has anything to read.</summary>
        private sealed class SilentPort : ICommsSerial
        {
            private bool open = true;

            public System.IO.Stream BaseStream => System.IO.Stream.Null;
            public int BaudRate { get; set; } = 115200;
            public int BytesToRead => 0;
            public int BytesToWrite => 0;
            public int DataBits { get; set; } = 8;
            public bool DtrEnable { get; set; }
            public bool IsOpen => open;
            public string PortName { get; set; } = "SILENT";
            public int ReadBufferSize { get; set; } = 4096;
            public int ReadTimeout { get; set; } = 1000;
            public bool RtsEnable { get; set; }
            public int WriteBufferSize { get; set; } = 4096;
            public int WriteTimeout { get; set; } = 1000;

            public void Close()
            {
                open = false;
            }

            public void Dispose()
            {
                open = false;
            }

            public void DiscardInBuffer()
            {
            }

            public void Open()
            {
                open = true;
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                return 0;
            }

            public int ReadByte()
            {
                return -1;
            }

            public int ReadChar()
            {
                return -1;
            }

            public string ReadExisting()
            {
                return "";
            }

            public string ReadLine()
            {
                return "";
            }

            public void Write(string text)
            {
            }

            public void Write(byte[] buffer, int offset, int count)
            {
            }

            public void WriteLine(string text)
            {
            }

            public void toggleDTR()
            {
            }
        }

        private sealed class Undo : IDisposable
        {
            private readonly Action undo;

            public Undo(Action undo)
            {
                this.undo = undo;
            }

            public void Dispose()
            {
                undo();
            }
        }

        private static ElevationGraphInput GraphInput(FlightPlanner fp)
        {
            return (ElevationGraphInput) typeof(FlightPlanner)
                .GetMethod("GetElevationGraphInput", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(fp, null);
        }

        [TestMethod]
        public void PlanPage_LaysTheGraphOutBeforeThePageIsShown()
        {
            // at startup the saved setting ticks the box while the page is still hidden; the graph
            // once stayed at its default 150 x 150 in the map's top left corner
            var fp = new FlightPlanner();
            fp.quickadd = true;
            fp.CHK_elevationgraph.Checked = false;
            fp.CHK_elevationgraph.Checked = true;
            fp.panelMap_Resize(null, null);

            var graph = (System.Windows.Forms.Control) typeof(FlightPlanner)
                .GetField("elevationGraph", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fp);
            Assert.IsFalse(graph.Visible); // the page itself is not shown
            Assert.AreEqual(fp.MainMap.Bottom, graph.Bottom);
            Assert.AreEqual(fp.MainMap.Left, graph.Left);
            Assert.AreEqual(fp.MainMap.Width, graph.Width);
            Assert.IsTrue(graph.Height >= 90);
        }

        [TestMethod]
        public void RallyPointsAlone_LayTheTerrainOutThroughThem()
        {
            // no mission path: the terrain runs home -> R1 -> R2, each point over its own ground,
            // and nothing is drawn as a path
            var input = WithHome(100);
            input.RallyPoints.Add(new ElevationGraphItem {Row = 1, Label = "R1", Lat = Lat0 + 0.01, Lng = Lng0, Alt = 80});
            input.RallyPoints.Add(new ElevationGraphItem {Row = 2, Label = "R2", Lat = Lat0 + 0.02, Lng = Lng0, Alt = 30});

            var profile = ElevationGraphProfile.Build(input, Flat);

            Assert.IsTrue(profile.RallyChain);
            Assert.AreEqual(2, profile.RallyPoints.Count);
            Assert.AreEqual(MetresPer001, profile.RallyPoints[0].Dist, 1);
            Assert.AreEqual(2 * MetresPer001, profile.RallyPoints[1].Dist, 1);
            Assert.AreEqual(2 * MetresPer001, profile.TotalDistance, 1);
            Assert.IsTrue(profile.Samples.Count > 2);
            Assert.IsTrue(profile.Samples.All(s => double.IsNaN(s.Alt) && !s.Airborne));
            Assert.IsTrue(profile.Samples.All(s => s.Terrain == 100));
            // the lowest clearance is a point's own: R2 at 130 m AMSL over 100 m of terrain
            Assert.AreEqual(30, profile.LowestClearance, 1e-6);
            Assert.AreEqual(profile.RallyPoints[1].Dist, profile.LowestClearanceDist, 1e-6);
            // home is the only "waypoint", at the start of the line
            Assert.AreEqual(1, profile.Waypoints.Count);
            Assert.IsNull(profile.Waypoints[0].Item);
        }

        [TestMethod]
        public void RallyPointsAlone_WithoutHome_StartAtTheFirstPoint()
        {
            var input = new ElevationGraphInput();
            input.RallyPoints.Add(new ElevationGraphItem
                {Row = 1, Label = "R1", Lat = Lat0, Lng = Lng0, Alt = 500, Frame = ElevationGraphAltFrame.Absolute});
            input.RallyPoints.Add(new ElevationGraphItem
                {Row = 2, Label = "R2", Lat = Lat0 + 0.01, Lng = Lng0, Alt = 120, Frame = ElevationGraphAltFrame.Absolute});

            var profile = ElevationGraphProfile.Build(input, Flat);

            Assert.IsTrue(profile.RallyChain);
            Assert.AreEqual(0, profile.RallyPoints[0].Dist, 1e-9);
            Assert.AreEqual(MetresPer001, profile.RallyPoints[1].Dist, 1);
            Assert.AreEqual(20, profile.LowestClearance, 1e-6);
            Assert.AreEqual(0, profile.Waypoints.Count);
        }

        [TestMethod]
        public void RallyPoints_WithAMission_StayOnThePath()
        {
            var input = WithHome(100, Wp(1, Lat0 + 0.02, 50));
            input.RallyPoints.Add(new ElevationGraphItem {Row = 1, Label = "R1", Lat = Lat0 + 0.01, Lng = Lng0 + 0.001, Alt = 80});

            var profile = ElevationGraphProfile.Build(input, Flat);

            Assert.IsFalse(profile.RallyChain);
            Assert.AreEqual(MetresPer001, profile.RallyPoints[0].Dist, 5);
            Assert.IsTrue(profile.RallyPoints[0].OffPath > 50);
            Assert.IsTrue(profile.Samples.All(s => !double.IsNaN(s.Alt)));
        }

        [TestMethod]
        public void Graph_RallyPointsAlone_SaysHowManyAndTheirClearance()
        {
            using (var view = new PlanElevationGraph())
            {
                var summary = (System.Windows.Forms.Control) typeof(PlanElevationGraph)
                    .GetField("summary", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view);
                var graph = (ZedGraphControl) typeof(PlanElevationGraph)
                    .GetField("graph", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view);
                var multiplier = CurrentState.multiplieralt;
                var unit = CurrentState.AltUnit;
                CurrentState.multiplieralt = 1;
                CurrentState.AltUnit = "m";
                try
                {
                    var input = WithHome(100);
                    input.RallyPoints.Add(new ElevationGraphItem {Row = 1, Label = "R1", Lat = Lat0 + 0.01, Lng = Lng0, Alt = 80});
                    input.RallyPoints.Add(new ElevationGraphItem {Row = 2, Label = "R2", Lat = Lat0 + 0.02, Lng = Lng0, Alt = 30});
                    view.ShowProfile(ElevationGraphProfile.Build(input, Flat), false);

                    Assert.AreEqual(string.Format(Strings.ElevationGraphRallySummary, 2, "30 m"), summary.Text);
                    var legend = graph.GraphPane.CurveList.Where(c => c.Label.IsVisible).Select(c => c.Label.Text).ToList();
                    CollectionAssert.Contains(legend, Strings.ElevationGraphTerrain);
                    CollectionAssert.Contains(legend, Strings.ElevationGraphRallyPoints);
                    CollectionAssert.DoesNotContain(legend, Strings.ElevationGraphPath);
                    Assert.AreEqual(string.Format(Strings.ElevationGraphRallyDistanceAxis, "m"),
                        graph.GraphPane.XAxis.Title.Text);
                }
                finally
                {
                    CurrentState.multiplieralt = multiplier;
                    CurrentState.AltUnit = unit;
                }
            }
        }

        [TestMethod]
        public void Graph_TitleBarDrag_ChangesTheHeightOnceOnRelease()
        {
            using (var view = new PlanElevationGraph())
            {
                view.Height = 170;
                view.MaximumHeight = 400;
                var heights = new List<int>();
                view.HeightDragged += (sender, height) => heights.Add(height);

                var header = (System.Windows.Forms.Control) typeof(PlanElevationGraph)
                    .GetField("header", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view);
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var down = typeof(PlanElevationGraph).GetMethod("Header_MouseDown", flags);
                var move = typeof(PlanElevationGraph).GetMethod("Header_MouseMove", flags);
                var up = typeof(PlanElevationGraph).GetMethod("Header_MouseUp", flags);
                var lost = typeof(PlanElevationGraph).GetMethod("Header_MouseCaptureChanged", flags);
                Func<int, int, System.Windows.Forms.MouseEventArgs> at = (clicks, y) =>
                    new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, clicks, 5, y, 0);

                // dragging 60 px up: nothing changes until the button is released, then once
                down.Invoke(view, new object[] {header, at(1, 10)});
                move.Invoke(view, new object[] {header, at(0, -20)});
                move.Invoke(view, new object[] {header, at(0, -50)});
                Assert.AreEqual(0, heights.Count);
                Assert.AreEqual(170, view.Height);
                up.Invoke(view, new object[] {header, at(1, -50)});
                CollectionAssert.AreEqual(new[] {230}, heights);
                Assert.AreEqual(170, view.Height); // the owner lays the graph out

                // held between the smallest useful graph and the room the owner gave
                down.Invoke(view, new object[] {header, at(1, 10)});
                move.Invoke(view, new object[] {header, at(0, -900)});
                up.Invoke(view, new object[] {header, at(1, -900)});
                down.Invoke(view, new object[] {header, at(1, 10)});
                move.Invoke(view, new object[] {header, at(0, 900)});
                up.Invoke(view, new object[] {header, at(1, 900)});
                CollectionAssert.AreEqual(new[] {230, 400, PlanElevationGraph.MinimumHeight}, heights);

                // a plain click, the second press of a double click, and a drag that loses the
                // mouse change nothing
                down.Invoke(view, new object[] {header, at(1, 10)});
                up.Invoke(view, new object[] {header, at(1, 10)});
                down.Invoke(view, new object[] {header, at(2, 10)});
                move.Invoke(view, new object[] {header, at(0, -50)});
                up.Invoke(view, new object[] {header, at(2, -50)});
                down.Invoke(view, new object[] {header, at(1, 10)});
                move.Invoke(view, new object[] {header, at(0, -50)});
                lost.Invoke(view, new object[] {header, EventArgs.Empty});
                up.Invoke(view, new object[] {header, at(1, -50)});
                Assert.AreEqual(3, heights.Count);
            }
        }

        [TestMethod]
        public void PlanPage_GraphHeight_LeavesTheMapInView()
        {
            var height = typeof(FlightPlanner).GetMethod("ElevationGraphHeightFor",
                BindingFlags.Static | BindingFlags.NonPublic);
            Func<int, int, bool, int> heightFor = (map, wanted, expanded) =>
                (int) height.Invoke(null, new object[] {map, wanted, expanded});

            Assert.AreEqual(170, heightFor(600, 170, false));
            Assert.AreEqual(560, heightFor(600, 1000, false)); // 40 px of map stay in view
            Assert.AreEqual(90, heightFor(600, 10, false)); // no smaller than the smallest useful graph
            Assert.AreEqual(400, heightFor(600, 170, true)); // two thirds of the map
            Assert.AreEqual(90, heightFor(130, 170, false)); // just room for the smallest graph
            Assert.AreEqual(0, heightFor(129, 170, false)); // none: the graph hides
            Assert.AreEqual(0, heightFor(129, 170, true));
        }

        [TestMethod]
        public void PlanPage_HidesTheGraphWhileTheMapIsTooShort()
        {
            var fp = new FlightPlanner();
            fp.quickadd = true;
            fp.CHK_elevationgraph.Checked = true;
            var graph = (System.Windows.Forms.Control) typeof(FlightPlanner)
                .GetField("elevationGraph", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fp);
            var layout = typeof(FlightPlanner).GetMethod("LayoutElevationGraph",
                BindingFlags.Instance | BindingFlags.NonPublic);
            // Visible reads false while the page is hidden; the control's own flag is what the
            // layout sets, and it shows through GetState(2) (STATE_VISIBLE)
            var state = typeof(System.Windows.Forms.Control).GetMethod("GetState",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Func<bool> wantsToShow = () => (bool) state.Invoke(graph, new object[] {2});

            var mapHeight = fp.MainMap.Height;
            layout.Invoke(fp, null);
            Assert.IsTrue(wantsToShow());

            fp.panelMap.Height = fp.panelMap.Height - fp.MainMap.Height + 100; // the map 100 px tall
            layout.Invoke(fp, null);
            Assert.IsFalse(wantsToShow());

            fp.panelMap.Height = fp.panelMap.Height - fp.MainMap.Height + mapHeight;
            layout.Invoke(fp, null);
            Assert.IsTrue(wantsToShow());
            Assert.AreEqual(fp.MainMap.Bottom, graph.Bottom);
        }

        [TestMethod]
        public void FenceCircle_FromParameters_FollowsFenceType()
        {
            var circle = ElevationGraphInput.FenceCircleOf(Params("FENCE_RADIUS", 300, "FENCE_TYPE", 7));
            Assert.AreEqual(300, circle.Radius, 1e-9);
            Assert.IsTrue(circle.Used);

            // max alt, polygon and min alt: no circle
            Assert.IsFalse(ElevationGraphInput.FenceCircleOf(Params("FENCE_RADIUS", 300, "FENCE_TYPE", 13)).Used);

            // older Plane firmware has no circle fence
            Assert.IsNull(ElevationGraphInput.FenceCircleOf(Params("FENCE_MAXALT", 150, "FENCE_MINALT", 0)));
        }

        [TestMethod]
        public void ActiveFenceCircle_OnlyWhileTheVehicleHoldsToIt()
        {
            Func<bool?, object[], ElevationGraphFenceCircle> active = (reported, values) =>
                ElevationGraphInput.ActiveFenceCircle(Params(values), reported);

            Assert.AreEqual(300, active(true, new object[]
                {"FENCE_RADIUS", 300, "FENCE_TYPE", 15, "FENCE_ENABLE", 1, "FENCE_ACTION", 1}).Radius, 1e-9);

            // switched off on the vehicle (an RC switch, say) with FENCE_ENABLE still 1
            Assert.IsNull(active(false, new object[]
                {"FENCE_RADIUS", 300, "FENCE_TYPE", 15, "FENCE_ENABLE", 1, "FENCE_ACTION", 1}));

            // off now, on by itself when armed
            Assert.IsNotNull(active(false, new object[]
                {"FENCE_RADIUS", 300, "FENCE_TYPE", 15, "FENCE_ENABLE", 0, "FENCE_ACTION", 1, "FENCE_AUTOENABLE", 3}));

            // only reports a breach
            Assert.IsNull(active(false, new object[]
                {"FENCE_RADIUS", 300, "FENCE_TYPE", 15, "FENCE_ENABLE", 1, "FENCE_ACTION", 0}));

            // on, but FENCE_TYPE leaves the circle out, or the radius is 0
            Assert.IsNull(active(true, new object[]
                {"FENCE_RADIUS", 300, "FENCE_TYPE", 13, "FENCE_ENABLE", 1, "FENCE_ACTION", 1}));
            Assert.IsNull(active(true, new object[]
                {"FENCE_RADIUS", 0, "FENCE_TYPE", 15, "FENCE_ENABLE", 1, "FENCE_ACTION", 1}));
        }

        [TestMethod]
        public void FenceCircle_IsCrossedWhereThePathLeavesAndComesBack()
        {
            // out 0.01 degree north and back to 0.002: a 500 m circle around home is left 500 m
            // along the path and entered again on the way back
            var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.002, 50));
            input.FenceCircle = new ElevationGraphFenceCircle {Radius = 500, Used = true};
            var profile = ElevationGraphProfile.Build(input, Flat);

            Assert.AreEqual(2, profile.FenceCircleCrossings.Count);
            Assert.AreEqual(500, profile.FenceCircleCrossings[0], 1);
            Assert.AreEqual(2 * MetresPer001 - 500, profile.FenceCircleCrossings[1], 1);

            // a circle the fence does not use, or no home to centre it on: none
            input.FenceCircle.Used = false;
            Assert.AreEqual(0, ElevationGraphProfile.Build(input, Flat).FenceCircleCrossings.Count);
            input.FenceCircle.Used = true;
            input.Home = null;
            Assert.AreEqual(0, ElevationGraphProfile.Build(input, Flat).FenceCircleCrossings.Count);
        }

        [TestMethod]
        public void Signature_ChangesWithTheFenceCircle()
        {
            Func<double, bool, ElevationGraphInput> plan = (radius, used) =>
            {
                var input = WithHome(100, Wp(1, Lat0 + 0.01, 50));
                input.FenceCircle = new ElevationGraphFenceCircle {Radius = radius, Used = used};
                return input;
            };

            Assert.AreEqual(plan(300, true).Signature(), plan(300, true).Signature());
            Assert.AreNotEqual(WithHome(100, Wp(1, Lat0 + 0.01, 50)).Signature(), plan(300, true).Signature());
            Assert.AreNotEqual(plan(300, true).Signature(), plan(400, true).Signature());
            Assert.AreNotEqual(plan(300, true).Signature(), plan(300, false).Signature());
        }

        [TestMethod]
        public void Graph_DrawsTheFenceCircleCrossingsOnlyWhileTheCircleIsActive()
        {
            var distScale = CurrentState.multiplierdist;
            var distUnit = CurrentState.DistanceUnit;
            CurrentState.multiplierdist = 1;
            CurrentState.DistanceUnit = "m";
            try
            {
                using (var view = new PlanElevationGraph())
                {
                    var pane = ((ZedGraphControl) typeof(PlanElevationGraph)
                        .GetField("graph", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(view)).GraphPane;
                    var label = string.Format(Strings.ElevationGraphFenceRadius, "500 m");
                    Func<List<LineObj>> lines = () => pane.GraphObjList.OfType<LineObj>().ToList();
                    Func<List<string>> texts = () => pane.GraphObjList.OfType<TextObj>().Select(t => t.Text).ToList();

                    Action<ElevationGraphFenceState?, bool> show = (state, used) =>
                    {
                        var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.002, 50));
                        input.FenceCircle = new ElevationGraphFenceCircle {Radius = 500, Used = used};
                        input.FenceState = state;
                        view.ShowProfile(ElevationGraphProfile.Build(input, Flat), false);
                    };

                    show(ElevationGraphFenceState.Enabled, true);
                    var drawn = lines();
                    Assert.AreEqual(2, drawn.Count);
                    Assert.AreEqual(500, drawn[0].Location.X, 1);
                    Assert.AreEqual(2 * MetresPer001 - 500, drawn[1].Location.X, 1);
                    Assert.IsTrue(drawn.All(l => l.Line.Color == Color.Red));
                    // by name: DashStyle exists in System.Drawing and MissionPlanner.Drawing alike
                    Assert.AreEqual("Dash", drawn[0].Line.Style.ToString());
                    // top to bottom of the plot, whatever the altitude scale
                    Assert.AreEqual(CoordType.XScaleYChartFraction, drawn[0].Location.CoordinateFrame);
                    Assert.AreEqual(2, texts().Count(t => t == label)); // far apart, so each has its label

                    // off now, on by itself when armed: drawn as well
                    show(ElevationGraphFenceState.EnablesWhenArmed, true);
                    Assert.AreEqual(2, lines().Count);

                    // off, report only, or not in FENCE_TYPE: no circle at all, not even greyed out
                    show(ElevationGraphFenceState.Disabled, true);
                    Assert.AreEqual(0, lines().Count);
                    CollectionAssert.DoesNotContain(texts(), label);
                    show(ElevationGraphFenceState.ReportOnly, true);
                    Assert.AreEqual(0, lines().Count);
                    show(ElevationGraphFenceState.Enabled, false);
                    Assert.AreEqual(0, lines().Count);
                }
            }
            finally
            {
                CurrentState.multiplierdist = distScale;
                CurrentState.DistanceUnit = distUnit;
            }
        }

        [TestMethod]
        public void PlanPage_DrawsTheFenceCircleOnTheMapOnlyWhileItIsActive()
        {
            var fp = new FlightPlanner();
            fp.quickadd = true;
            fp.TXT_homelat.Text = Lat0.ToString();
            fp.TXT_homelng.Text = Lng0.ToString();
            fp.TXT_homealt.Text = "584";

            var update = typeof(FlightPlanner).GetMethod("UpdateFenceCircle", BindingFlags.Instance | BindingFlags.NonPublic);
            var overlay = (GMapOverlay) typeof(FlightPlanner)
                .GetField("fenceCircleOverlay", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fp);

            // no vehicle: no circle
            update.Invoke(fp, null);
            Assert.AreEqual(0, overlay.Polygons.Count);

            var previous = MainV2.comPort;
            using (var link = new OpenLink())
            {
                try
                {
                    link.Mav.sysidcurrent = OpenLink.SysId;
                    link.Mav.compidcurrent = OpenLink.CompId;
                    MainV2.comPort = link.Mav;
                    var param = link.Mav.MAV.param;
                    Action<string, double> set = (name, value) =>
                    {
                        param.RemoveAll(p => p.Name == name);
                        param.Add(new MAVLink.MAVLinkParam(name, value, MAVLink.MAV_PARAM_TYPE.REAL32));
                    };
                    set("FENCE_RADIUS", 300);
                    set("FENCE_TYPE", 15);
                    set("FENCE_ENABLE", 1);
                    set("FENCE_ACTION", 1);

                    update.Invoke(fp, null);
                    Assert.AreEqual(1, overlay.Polygons.Count);
                    var ring = overlay.Polygons[0];
                    Assert.IsFalse(ring.IsHitTestVisible); // the map's clicks and drags pass through it
                    // the ring is laid out with newpos on a 6378.1 km earth, close to ArduPilot's own
                    // scaling (6378.137 km); GetDistance measures on a 6371 km one
                    Func<GMap.NET.PointLatLng, double> radiusOf = corner =>
                        new PointLatLngAlt(Lat0, Lng0).GetDistance(new PointLatLngAlt(corner.Lat, corner.Lng)) *
                        6378100.0 / 6371000.0;
                    foreach (var corner in ring.Points)
                        Assert.AreEqual(300, radiusOf(corner), 0.5);

                    // switched off: gone; on by itself when armed: back
                    set("FENCE_ENABLE", 0);
                    update.Invoke(fp, null);
                    Assert.AreEqual(0, overlay.Polygons.Count);
                    set("FENCE_AUTOENABLE", 3);
                    update.Invoke(fp, null);
                    Assert.AreEqual(1, overlay.Polygons.Count);

                    // on, but FENCE_TYPE leaves the circle out
                    set("FENCE_AUTOENABLE", 0);
                    set("FENCE_ENABLE", 1);
                    set("FENCE_TYPE", 13);
                    update.Invoke(fp, null);
                    Assert.AreEqual(0, overlay.Polygons.Count);

                    // a wider circle replaces the old one
                    set("FENCE_TYPE", 15);
                    set("FENCE_RADIUS", 500);
                    update.Invoke(fp, null);
                    Assert.AreEqual(1, overlay.Polygons.Count);
                    Assert.AreEqual(500, radiusOf(overlay.Polygons[0].Points[0]), 0.5);
                }
                finally
                {
                    MainV2.comPort = previous;
                }
            }

            // the vehicle gone: so is the circle
            update.Invoke(fp, null);
            Assert.AreEqual(0, overlay.Polygons.Count);
        }

        private static Locationwp FenceItem(MAVLink.MAV_CMD cmd, float p1, double lat, double lng)
        {
            var item = Item(cmd, lat, lng, 0);
            item.p1 = p1;
            return item;
        }

        /// <summary>Fence vertices of a square around (lat, lng), half a side of <paramref name="half"/> degrees, vertex count 4.</summary>
        private static IEnumerable<Locationwp> Square(MAVLink.MAV_CMD cmd, double lat, double lng, double half)
        {
            yield return FenceItem(cmd, 4, lat - half, lng - half);
            yield return FenceItem(cmd, 4, lat + half, lng - half);
            yield return FenceItem(cmd, 4, lat + half, lng + half);
            yield return FenceItem(cmd, 4, lat - half, lng + half);
        }

        [TestMethod]
        public void FenceAreasOf_ReadsPolygonsAndCirclesAsArduPilotLoadsThem()
        {
            var items = new List<Locationwp>();
            items.Add(Item(MAVLink.MAV_CMD.WAYPOINT, Lat0, Lng0, 50)); // ALL mode mixes other rows in
            items.AddRange(Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0, Lng0, 0.05));
            // a second inclusion polygon straight after the first: the vertex count starts it
            items.AddRange(Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.2, Lng0, 0.05));
            items.AddRange(Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION, Lat0, Lng0, 0.01));
            items.Add(FenceItem(MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION, 200, Lat0 + 0.02, Lng0));
            items.Add(FenceItem(MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION, 0, Lat0, Lng0)); // no radius
            items.Add(FenceItem(MAVLink.MAV_CMD.FENCE_RETURN_POINT, 0, Lat0, Lng0));
            items.Add(FenceItem(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION, 2, Lat0, Lng0)); // two vertices
            items.Add(FenceItem(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION, 2, Lat0 + 0.01, Lng0));

            var areas = ElevationGraphInput.FenceAreasOf(items);

            CollectionAssert.AreEqual(new[] {true, true, false, false}, areas.Select(a => a.Inclusion).ToArray());
            Assert.AreEqual(4, areas[0].Vertices.Count);
            Assert.AreEqual(Lat0 + 0.2 - 0.05, areas[1].Vertices[0].Lat, 1e-9);
            Assert.IsNull(areas[3].Vertices);
            Assert.AreEqual(200, areas[3].Radius, 1e-9);
        }

        [TestMethod]
        public void FenceAreasOf_MissionAndRallyRowsBetweenVerticesDoNotSplitAPolygon()
        {
            // ALL mode: a waypoint and a rally point added between the vertices of a triangle. The
            // fence upload sends the three vertices together, so the graph must see one triangle
            // (PR 3813, third review); a fence item between vertices still ends the polygon.
            var items = new List<Locationwp>
            {
                FenceItem(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, 3, Lat0, Lng0),
                Item(MAVLink.MAV_CMD.WAYPOINT, Lat0 + 0.01, Lng0, 50),
                FenceItem(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, 3, Lat0 + 0.02, Lng0),
                Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0 + 0.01, Lng0 + 0.01, 80),
                FenceItem(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, 3, Lat0 + 0.01, Lng0 + 0.02),
            };

            var areas = ElevationGraphInput.FenceAreasOf(items);

            Assert.AreEqual(1, areas.Count);
            Assert.AreEqual(3, areas[0].Vertices.Count);

            items.Insert(2, FenceItem(MAVLink.MAV_CMD.FENCE_RETURN_POINT, 0, Lat0, Lng0));
            Assert.AreEqual(0, ElevationGraphInput.FenceAreasOf(items).Count,
                "a return point between the vertices leaves two fragments, both too short");
        }

        [TestMethod]
        public void FenceArea_Contains()
        {
            var square = ElevationGraphInput.FenceAreasOf(
                Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0, Lng0, 0.01).ToList())[0];
            Assert.IsTrue(square.Contains(Lat0, Lng0));
            Assert.IsTrue(square.Contains(Lat0 + 0.009, Lng0 - 0.009));
            Assert.IsFalse(square.Contains(Lat0 + 0.011, Lng0));
            Assert.IsFalse(square.Contains(Lat0, Lng0 + 0.02));

            var circle = new ElevationGraphFenceArea {Centre = new PointLatLngAlt(Lat0, Lng0), Radius = 500};
            Assert.IsTrue(circle.Contains(Lat0 + 400 / MetresPer001 * 0.01, Lng0));
            Assert.IsFalse(circle.Contains(Lat0 + 600 / MetresPer001 * 0.01, Lng0));
        }

        [TestMethod]
        public void FencePolygonOptions_FromFenceTypeAndOptions()
        {
            bool used, union;
            ElevationGraphInput.FencePolygonOptions(Params("FENCE_TYPE", 7, "FENCE_OPTIONS", 2), out used, out union);
            Assert.IsTrue(used);
            Assert.IsTrue(union);
            ElevationGraphInput.FencePolygonOptions(Params("FENCE_TYPE", 3, "FENCE_OPTIONS", 1), out used, out union);
            Assert.IsFalse(used);
            Assert.IsFalse(union);
            // older Plane has no FENCE_TYPE; its fence is the polygon
            ElevationGraphInput.FencePolygonOptions(Params("FENCE_ACTION", 1), out used, out union);
            Assert.IsTrue(used);
        }

        [TestMethod]
        public void OutsideFence_AboveTheCeilingAndBelowTheFloorOnceItIsArmed()
        {
            // home 100 m; ceiling 120 m and floor 20 m above it
            var input = WithHome(100,
                Wp(1, Lat0 + 0.001, 10), // under the floor, but the floor has not armed yet
                Wp(2, Lat0 + 0.002, 50),
                Wp(3, Lat0 + 0.003, 150), // above the ceiling
                Wp(4, Lat0 + 0.004, 10), // under the floor, which armed at 2
                Wp(5, Lat0 + 0.005, 0));
            input.Waypoints[4].OnGround = true; // a landing is meant to be on the ground
            input.FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = true};
            input.FenceFloor = new ElevationGraphFenceLimit {Alt = 20, Used = true};
            input.FenceState = ElevationGraphFenceState.Enabled;
            var profile = ElevationGraphProfile.Build(input, Flat);

            CollectionAssert.AreEqual(new[] {"3", "4"}, profile.OutsideFence.Select(a => a.Label).ToArray());
            Assert.AreEqual(ElevationGraphFenceBreach.AboveCeiling, profile.OutsideFence[0].Breach);
            Assert.AreEqual(ElevationGraphFenceBreach.BelowFloor, profile.OutsideFence[1].Breach);

            // the same plan with the fence off, or the limits left out of FENCE_TYPE: nothing outside
            input.FenceState = ElevationGraphFenceState.Disabled;
            Assert.AreEqual(0, ElevationGraphProfile.Build(input, Flat).OutsideFence.Count);
            input.FenceState = ElevationGraphFenceState.Enabled;
            input.FenceCeiling.Used = false;
            input.FenceFloor.Used = false;
            Assert.AreEqual(0, ElevationGraphProfile.Build(input, Flat).OutsideFence.Count);
        }

        [TestMethod]
        public void OutsideFence_FloorArmsWhereATerrainFollowingLegClimbsAboveIt()
        {
            // home 100 m on flat ground, the floor 50 m above it. Both waypoints are 20 m over the
            // ground, under the floor, but the leg into the terrain frame one follows the ground over
            // a 300 m ridge, well above the floor, which arms there (PR 3813, third review)
            Func<double, double, double?> ridge = (lat, lng) => Math.Abs(lat - (Lat0 + 0.015)) < 0.001 ? 300 : 100;
            var input = WithHome(100,
                Wp(1, Lat0 + 0.01, 20), // relative: 120 AMSL
                Wp(2, Lat0 + 0.02, 20, ElevationGraphAltFrame.Terrain)); // 120 AMSL again
            input.FenceFloor = new ElevationGraphFenceLimit {Alt = 50, Used = true};
            input.FenceState = ElevationGraphFenceState.Enabled;
            var profile = ElevationGraphProfile.Build(input, ridge);

            Assert.IsTrue(profile.Samples.Max(s => s.Alt) > 300, "the leg climbs over the ridge");
            CollectionAssert.AreEqual(new[] {"2"}, profile.OutsideFence.Select(a => a.Label).ToArray());
            Assert.AreEqual(ElevationGraphFenceBreach.BelowFloor, profile.OutsideFence[0].Breach);
        }

        [TestMethod]
        public void OutsideFence_TheCircleAndRallyPoints()
        {
            // a 500 m circle around home: 1 at about 111 m is in, 2 at about 1112 m is out, and so is R1
            var input = WithHome(100, Wp(1, Lat0 + 0.001, 50), Wp(2, Lat0 + 0.01, 50));
            input.RallyPoints.Add(new ElevationGraphItem {Row = 1, Label = "R1", Lat = Lat0 + 0.008, Lng = Lng0, Alt = 50});
            input.FenceCircle = new ElevationGraphFenceCircle {Radius = 500, Used = true};
            input.FenceState = ElevationGraphFenceState.EnablesWhenArmed;
            var profile = ElevationGraphProfile.Build(input, Flat);

            CollectionAssert.AreEqual(new[] {"2", "R1"}, profile.OutsideFence.Select(a => a.Label).ToArray());
            Assert.IsTrue(profile.OutsideFence.All(a => a.Breach == ElevationGraphFenceBreach.OutsideCircle));
        }

        [TestMethod]
        public void OutsideFence_PolygonInclusionAndExclusionAreas()
        {
            // inclusion squares A (around 1) and B (around 2 and 3), and an exclusion circle on 3
            var items = Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.01, Lng0, 0.005)
                .Concat(Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.03, Lng0, 0.015)).ToList();
            items.Add(FenceItem(MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION, 100, Lat0 + 0.04, Lng0));
            Func<bool, ElevationGraphProfile> build = inclusionUnion =>
            {
                var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.03, 50), Wp(3, Lat0 + 0.04, 50));
                input.FenceAreas = ElevationGraphInput.FenceAreasOf(items);
                input.FenceInclusionUnion = inclusionUnion;
                input.FenceState = ElevationGraphFenceState.Enabled;
                return ElevationGraphProfile.Build(input, Flat);
            };

            // the default: inside every inclusion area, so each point is outside one of them
            var all = build(false);
            CollectionAssert.AreEqual(new[] {"1", "2", "3"}, all.OutsideFence.Select(a => a.Label).ToArray());
            Assert.AreEqual(ElevationGraphFenceBreach.OutsideInclusion, all.OutsideFence[0].Breach);
            Assert.AreEqual(ElevationGraphFenceBreach.OutsideInclusion | ElevationGraphFenceBreach.InsideExclusion,
                all.OutsideFence[2].Breach);

            // FENCE_OPTIONS bit 1: inside any one is enough, but the exclusion zone still counts
            var union = build(true);
            CollectionAssert.AreEqual(new[] {"3"}, union.OutsideFence.Select(a => a.Label).ToArray());
            Assert.AreEqual(ElevationGraphFenceBreach.InsideExclusion, union.OutsideFence[0].Breach);
        }

        [TestMethod]
        public void OutsideFence_PlannedPolygonIsCheckedWithoutAVehicle()
        {
            // not connected: no fence state and no FENCE_TYPE, so a planned fence counts
            var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.05, 50));
            input.FenceAreas = ElevationGraphInput.FenceAreasOf(
                Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.01, Lng0, 0.02).ToList());
            CollectionAssert.AreEqual(new[] {"2"},
                ElevationGraphProfile.Build(input, Flat).OutsideFence.Select(a => a.Label).ToArray());

            // a vehicle whose FENCE_TYPE leaves the polygon out
            input.FencePolygonUsed = false;
            input.FenceState = ElevationGraphFenceState.Enabled;
            Assert.AreEqual(0, ElevationGraphProfile.Build(input, Flat).OutsideFence.Count);
        }

        [TestMethod]
        public void Signature_ChangesWithThePolygonFence()
        {
            Func<ElevationGraphInput> plan = () => WithHome(100, Wp(1, Lat0 + 0.01, 50));
            var fenced = plan();
            fenced.FenceAreas = ElevationGraphInput.FenceAreasOf(
                Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0, Lng0, 0.02).ToList());
            Assert.AreNotEqual(plan().Signature(), fenced.Signature());

            var moved = plan();
            moved.FenceAreas = ElevationGraphInput.FenceAreasOf(
                Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0, Lng0, 0.03).ToList());
            Assert.AreNotEqual(fenced.Signature(), moved.Signature());

            var union = plan();
            union.FenceInclusionUnion = true;
            Assert.AreNotEqual(plan().Signature(), union.Signature());
        }

        [TestMethod]
        public void Graph_MarksThePointsOutsideTheFence()
        {
            var altScale = CurrentState.multiplieralt;
            var altUnit = CurrentState.AltUnit;
            CurrentState.multiplieralt = 1;
            CurrentState.AltUnit = "m";
            try
            {
                using (var view = new PlanElevationGraph())
                {
                    var pane = ((ZedGraphControl) typeof(PlanElevationGraph)
                        .GetField("graph", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(view)).GraphPane;
                    var outsideLabel = (System.Windows.Forms.Control) typeof(PlanElevationGraph)
                        .GetField("outsideFence", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view);
                    Func<ElevationGraphFenceState?, ElevationGraphProfile> show = state =>
                    {
                        var input = WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 150), Wp(3, Lat0 + 0.03, 50));
                        input.FenceCeiling = new ElevationGraphFenceLimit {Alt = 120, Used = true};
                        input.FenceState = state;
                        var profile = ElevationGraphProfile.Build(input, Flat);
                        view.ShowProfile(profile, false);
                        return profile;
                    };

                    show(ElevationGraphFenceState.Enabled);
                    Assert.AreEqual(string.Format(Strings.ElevationGraphOutsideFence, "2"), outsideLabel.Text);
                    var ring = (LineItem) pane.CurveList.Single(c => c.Label.Text == Strings.ElevationGraphOutsideFenceLegend);
                    Assert.AreEqual(Color.Red, ring.Color);
                    Assert.AreEqual(1, ring.Points.Count);
                    Assert.AreEqual(250, ring.Points[0].Y, 1e-9); // home 100 m + 150 m
                    Assert.IsTrue(ring.Label.IsVisible); // in the legend
                    var label = pane.GraphObjList.OfType<TextObj>().Single(t => t.Text == "2");
                    Assert.AreEqual(Color.White, label.FontSpec.FontColor);
                    Assert.IsTrue(label.FontSpec.Fill.IsVisible);
                    Assert.AreEqual(Color.Red, label.FontSpec.Fill.Color);
                    var inside = pane.GraphObjList.OfType<TextObj>().Single(t => t.Text == "1");
                    Assert.IsFalse(inside.FontSpec.Fill.IsVisible);

                    // the fence switched off: no ring, no names
                    show(ElevationGraphFenceState.Disabled);
                    Assert.AreEqual("", outsideLabel.Text);
                    Assert.IsFalse(pane.CurveList.Any(c => c.Label.Text == Strings.ElevationGraphOutsideFenceLegend));
                }
            }
            finally
            {
                CurrentState.multiplieralt = altScale;
                CurrentState.AltUnit = altUnit;
            }
        }

        [TestMethod]
        public void PlanPage_TakesTheVehiclesFenceWhileEditingTheMission()
        {
            var fp = new FlightPlanner();
            fp.quickadd = true;
            fp.TXT_homelat.Text = Lat0.ToString();
            fp.TXT_homelng.Text = Lng0.ToString();
            fp.TXT_homealt.Text = "584";
            AddRow(fp, "WAYPOINT", Lat0 + 0.01, Lng0, 100);

            // the dropdown is on MISSION, so the fence comes from the connected vehicle's copy
            using (ConnectedVehicle())
            {
                var key = 0;
                foreach (var item in Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0, Lng0, 0.005))
                    MainV2.comPort.MAV.fencepoints[key++] = item;

                var input = GraphInput(fp);
                Assert.AreEqual(1, input.FenceAreas.Count);
                Assert.IsTrue(input.FencePolygonUsed); // FENCE_TYPE not known
                var profile = ElevationGraphProfile.Build(input, Flat);
                CollectionAssert.AreEqual(new[] {"1"}, profile.OutsideFence.Select(a => a.Label).ToArray());
            }

            // offline the copy a vehicle left behind (not cleared on a disconnect) flags nothing
            // (PR 3813, maintainer review)
            var stale = MainV2.comPort.MAV.fencepoints;
            stale.Clear();
            var at = 0;
            foreach (var item in Square(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0, Lng0, 0.005))
                stale[at++] = item;
            try
            {
                var input = GraphInput(fp);
                Assert.AreEqual(0, input.FenceAreas.Count);
                Assert.AreEqual(0, ElevationGraphProfile.Build(input, Flat).OutsideFence.Count);
            }
            finally
            {
                stale.Clear();
            }
        }

        [TestMethod]
        public void PlanPage_DropsMissionItemZeroOnlyOnArduPilot()
        {
            // while the dropdown shows FENCE or RALLY the mission comes from the vehicle's copy,
            // whose item 0 is home on ArduPilot only (saveWPs puts it there); on PX4 it is the first
            // real item (PR 3813, third review)
            var fp = new FlightPlanner();
            fp.quickadd = true;
            fp.TXT_homelat.Text = Lat0.ToString();
            fp.TXT_homelng.Text = Lng0.ToString();
            fp.TXT_homealt.Text = "584";

            // switch the dropdown without its handler, which would swap the grid's rows
            fp.cmb_missiontype.SelectedIndexChanged -= fp.Cmb_missiontype_SelectedIndexChanged;
            fp.cmb_missiontype.SelectedItem = MAVLink.MAV_MISSION_TYPE.FENCE;
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.FENCE, fp.cmb_missiontype.SelectedValue);

            using (ConnectedVehicle())
            {
                var mav = MainV2.comPort.MAV;
                mav.wps[0] = Item(MAVLink.MAV_CMD.WAYPOINT, Lat0, Lng0, 0);
                mav.wps[1] = Item(MAVLink.MAV_CMD.WAYPOINT, Lat0 + 0.01, Lng0, 100);

                mav.apname = MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA;
                Assert.AreEqual(1, GraphInput(fp).Waypoints.Count, "ArduPilot: item 0 is home");

                mav.apname = MAVLink.MAV_AUTOPILOT.PX4;
                Assert.AreEqual(2, GraphInput(fp).Waypoints.Count, "PX4: item 0 is a waypoint");
            }
        }

        private static void AddRow(FlightPlanner fp, string command, double lat, double lng, double alt)
        {
            var row = fp.Commands.Rows[fp.Commands.Rows.Add()];
            row.Cells["Command"].Value = command;
            row.Cells["Lat"].Value = lat.ToString();
            row.Cells["Lon"].Value = lng.ToString();
            row.Cells["Alt"].Value = alt.ToString();
            row.Cells["Frame"].Value = (int) MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
        }

        [TestMethod]
        public void FenceAreasOf_HomeCircleIsAnInclusionCircleAroundTheHomeGiven()
        {
            var items = new List<Locationwp> {FenceItem(MAVLink.MAV_CMD.FENCE_HOME_CIRCLE_INCLUSION, 300, 0, 0)};
            var home = new PointLatLngAlt(Lat0, Lng0, 584);

            var areas = ElevationGraphInput.FenceAreasOf(items, home);

            Assert.AreEqual(1, areas.Count);
            Assert.IsTrue(areas[0].Inclusion);
            Assert.IsNull(areas[0].Vertices);
            Assert.AreEqual(Lat0, areas[0].Centre.Lat, 1e-9);
            Assert.AreEqual(Lng0, areas[0].Centre.Lng, 1e-9);
            Assert.AreEqual(300, areas[0].Radius, 1e-9);
            Assert.IsTrue(areas[0].Contains(Lat0 + 0.001, Lng0), "about 110 m north is inside");
            Assert.IsFalse(areas[0].Contains(Lat0 + 0.01, Lng0), "about 1.1 km north is outside");

            // without a home the vehicle cannot place it either, and without a radius it is nothing
            Assert.AreEqual(0, ElevationGraphInput.FenceAreasOf(items).Count, "no home");
            Assert.AreEqual(0, ElevationGraphInput.FenceAreasOf(items, new PointLatLngAlt(0, 0)).Count, "home at 0, 0");
            Assert.AreEqual(0, ElevationGraphInput.FenceAreasOf(
                new List<Locationwp> {FenceItem(MAVLink.MAV_CMD.FENCE_HOME_CIRCLE_INCLUSION, 0, 0, 0)}, home).Count, "no radius");
        }

        // ---- PR 3813, review of 2026-11-12 ----

        [TestMethod]
        public void ALandingWithAPosition_EndsOnTheGroundUnderIt()
        {
            // a relative LAND with a position was drawn at home altitude plus its own: on sloping
            // ground the path ended in the air or under the ground
            Func<MAVLink.MAV_CMD, MAVLink.MAV_FRAME, double, ElevationGraphInput> plan = (land, frame, alt) =>
                new ElevationGraphInput
                {
                    Home = new PointLatLngAlt(Lat0, Lng0, 100, "H"),
                    Waypoints = ElevationGraphInput.WaypointsOf(new List<Locationwp>
                    {
                        Item(MAVLink.MAV_CMD.WAYPOINT, Lat0 + 0.01, Lng0, 50),
                        Item(land, Lat0 + 0.02, Lng0, alt, frame)
                    })
                };
            // the ground rises 30 m between home and the landing point
            Func<double, double, double?> slope = (lat, lng) => 100 + 30 * (lat - Lat0) / 0.02;

            foreach (var land in new[] {MAVLink.MAV_CMD.LAND, MAVLink.MAV_CMD.VTOL_LAND})
            {
                var profile = ElevationGraphProfile.Build(plan(land, MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT, 0), slope);
                var marker = profile.Waypoints[2];
                Assert.AreEqual(130, marker.Terrain, 1e-6, land.ToString());
                Assert.AreEqual(130, marker.Alt, 1e-6, land + ": on the ground there, not at home's 100");
                var last = profile.Samples[profile.Samples.Count - 1];
                Assert.AreEqual(130, last.Alt, 1e-6, land.ToString());

                // a straight line down from the waypoint (150) to the ground at the landing point
                var half = profile.Samples.First(s => s.Dist >= 1.5 * MetresPer001);
                Assert.AreEqual(140, half.Alt, 0.5, land.ToString());
            }

            // whatever altitude and frame the item carries
            foreach (var frame in new[] {MAVLink.MAV_FRAME.GLOBAL, MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT})
            {
                var profile = ElevationGraphProfile.Build(plan(MAVLink.MAV_CMD.LAND, frame, 10), slope);
                Assert.AreEqual(130, profile.Waypoints[2].Alt, 1e-6, frame.ToString());
                Assert.AreEqual(130, profile.Samples[profile.Samples.Count - 1].Alt, 1e-6, frame.ToString());
            }

            // without terrain data there is no ground to end on: the altitude it carries, as before
            var blind = ElevationGraphProfile.Build(plan(MAVLink.MAV_CMD.LAND, MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT, 5),
                (lat, lng) => null);
            Assert.AreEqual(105, blind.Waypoints[2].Alt, 1e-6);
        }

        [TestMethod]
        public void Signature_IsThePlanPartAndTheFencePart()
        {
            Func<ElevationGraphInput> plan = () => WithHome(100, Wp(1, Lat0 + 0.01, 50), Wp(2, Lat0 + 0.02, 60));

            var input = plan();
            Assert.AreEqual(input.PlanSignature() + input.FenceSignature(), input.Signature());

            // the fence settings the vehicle gives change the fence part only
            var fenced = plan();
            fenced.FenceCeiling = new ElevationGraphFenceLimit {Alt = 150, Used = true};
            fenced.FenceState = ElevationGraphFenceState.Enabled;
            Assert.AreEqual(input.PlanSignature(), fenced.PlanSignature());
            Assert.AreNotEqual(input.FenceSignature(), fenced.FenceSignature());

            // and the plan the plan part only
            var higher = plan();
            higher.Waypoints[1].Alt = 70;
            Assert.AreNotEqual(input.PlanSignature(), higher.PlanSignature());
            Assert.AreEqual(input.FenceSignature(), higher.FenceSignature());

            // a copy takes the vehicle's fence without changing the plan it was made from
            var copy = input.Copy();
            copy.FenceCeiling = new ElevationGraphFenceLimit {Alt = 150, Used = true};
            Assert.IsNull(input.FenceCeiling);
            Assert.AreSame(input.Waypoints, copy.Waypoints);
            Assert.AreEqual(input.PlanSignature(), copy.PlanSignature());
        }

        private static T Field<T>(FlightPlanner fp, string name)
        {
            return (T) typeof(FlightPlanner).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fp);
        }

        private static void SetField(FlightPlanner fp, string name, object value)
        {
            typeof(FlightPlanner).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(fp, value);
        }

        [TestMethod]
        public void PlanPage_TheGridAndHomeSayWhenThePlanChanged()
        {
            // the graph reads the grid again only after a change: every way the grid, the home boxes
            // or the item type can change has to say so, with or without a map redraw (quickadd)
            var fp = new FlightPlanner();
            fp.quickadd = true;
            Action<string, Action> changes = (what, change) =>
            {
                SetField(fp, "elevationGraphPlanChanged", false);
                change();
                Assert.IsTrue(Field<bool>(fp, "elevationGraphPlanChanged"), what);
            };

            changes("a row added", () => fp.Commands.Rows.Add());
            changes("a cell set", () => fp.Commands.Rows[0].Cells["Alt"].Value = "120");
            changes("a row removed", () => fp.Commands.Rows.RemoveAt(0));
            // values the boxes cannot hold already: the planned home is static and other tests set it
            changes("home latitude", () => fp.TXT_homelat.Text = (Lat0 + 0.0123).ToString());
            changes("home longitude", () => fp.TXT_homelng.Text = (Lng0 + 0.0123).ToString());
            changes("home altitude", () => fp.TXT_homealt.Text = "777.5");
            changes("the map redraw", () =>
                typeof(FlightPlanner).GetMethod("RequestElevationGraphUpdate", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(fp, null));
        }

        [TestMethod]
        public void VehicleStamp_ChangesWithTheVehiclesLists()
        {
            var stampOf = typeof(FlightPlanner).GetMethod("ElevationGraphVehicleStamp", BindingFlags.Static | BindingFlags.NonPublic);
            Func<string> stamp = () => (string) stampOf.Invoke(null, null);

            // not connected: the vehicle adds nothing
            Assert.AreEqual("", stamp());

            var previous = MainV2.comPort;
            using (var link = new OpenLink())
            {
                try
                {
                    link.Mav.sysidcurrent = OpenLink.SysId;
                    link.Mav.compidcurrent = OpenLink.CompId;
                    MainV2.comPort = link.Mav;
                    var mav = link.Mav.MAV;
                    mav.wps.Clear();
                    mav.rallypoints.Clear();
                    mav.fencepoints.Clear();

                    var empty = stamp();
                    Assert.AreNotEqual("", empty);
                    Assert.AreEqual(empty, stamp(), "nothing changed");

                    mav.rallypoints[0] = (MAVLink.mavlink_mission_item_int_t) Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0, Lng0, 80);
                    var one = stamp();
                    Assert.AreNotEqual(empty, one, "an item added");
                    Assert.AreEqual(one, stamp());

                    // the same count, another altitude: a download that changed one item
                    mav.rallypoints[0] = (MAVLink.mavlink_mission_item_int_t) Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0, Lng0, 90);
                    var raised = stamp();
                    Assert.AreNotEqual(one, raised, "an item changed");

                    mav.wps[0] = (MAVLink.mavlink_mission_item_int_t) Item(MAVLink.MAV_CMD.WAYPOINT, Lat0, Lng0, 0);
                    Assert.AreNotEqual(raised, stamp(), "the mission");
                    var withMission = stamp();
                    mav.fencepoints[0] = (MAVLink.mavlink_mission_item_int_t) Item(MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION, Lat0, Lng0, 0);
                    Assert.AreNotEqual(withMission, stamp(), "the fence");
                }
                finally
                {
                    MainV2.comPort = previous;
                }
            }
        }

        [TestMethod]
        public void PlanPage_AFailedBuildIsNotStartedAgainEverySecond()
        {
            // a build that faulted never stored its signature, so every tick saw a changed plan,
            // built again and logged the error again
            var fp = new FlightPlanner();
            fp.quickadd = true;
            var show = typeof(FlightPlanner).GetMethod("ShowElevationGraph", BindingFlags.Instance | BindingFlags.NonPublic);
            var failed = System.Threading.Tasks.Task.FromException<ElevationGraphProfile>(new InvalidOperationException("test"));

            SetField(fp, "elevationGraphSignature", "before");
            SetField(fp, "elevationGraphBuild", 3);
            SetField(fp, "elevationGraphTerrainTry", 0);

            // overtaken by a newer build: nothing of it is taken
            show.Invoke(fp, new object[] {failed, "older", 2});
            Assert.AreEqual("before", Field<string>(fp, "elevationGraphSignature"));

            // the current build failed: its plan is remembered, and its terrain is not looked for
            show.Invoke(fp, new object[] {failed, "failed plan", 3});
            Assert.AreEqual("failed plan", Field<string>(fp, "elevationGraphSignature"));
            Assert.IsTrue(Field<int>(fp, "elevationGraphTerrainTry") >= 30);

            // a result that a newer build has overtaken is not shown either
            var graph = Field<PlanElevationGraph>(fp, "elevationGraph");
            var built = System.Threading.Tasks.Task.FromResult(
                ElevationGraphProfile.Build(WithHome(100, Wp(1, Lat0 + 0.01, 50)), Flat));
            show.Invoke(fp, new object[] {built, "stale", 2});
            Assert.AreEqual("failed plan", Field<string>(fp, "elevationGraphSignature"));
            show.Invoke(fp, new object[] {built, "current", 3});
            Assert.AreEqual("current", Field<string>(fp, "elevationGraphSignature"));
            Assert.IsNotNull(graph);
        }

        [TestMethod]
        public void PlanPage_TheFenceCircleGoesWithTheGraph()
        {
            // the ring was drawn from the page's timer for every user; it now runs on the graph's
            // timer, and stopping that (the box unticked, the page left) takes it off the map
            var fp = new FlightPlanner();
            fp.quickadd = true;
            fp.TXT_homelat.Text = Lat0.ToString();
            fp.TXT_homelng.Text = Lng0.ToString();
            fp.TXT_homealt.Text = "584";

            var update = typeof(FlightPlanner).GetMethod("UpdateFenceCircle", BindingFlags.Instance | BindingFlags.NonPublic);
            var timer = typeof(FlightPlanner).GetMethod("UpdateElevationGraphTimer", BindingFlags.Instance | BindingFlags.NonPublic);
            var overlay = Field<GMapOverlay>(fp, "fenceCircleOverlay");

            var previous = MainV2.comPort;
            using (var link = new OpenLink())
            {
                try
                {
                    link.Mav.sysidcurrent = OpenLink.SysId;
                    link.Mav.compidcurrent = OpenLink.CompId;
                    MainV2.comPort = link.Mav;
                    var param = link.Mav.MAV.param;
                    foreach (var p in new[] {"FENCE_RADIUS", "300", "FENCE_TYPE", "15", "FENCE_ENABLE", "1", "FENCE_ACTION", "1"}
                                 .Select((v, i) => new {v, i}).GroupBy(a => a.i / 2).Select(g => g.Select(a => a.v).ToArray()))
                    {
                        param.RemoveAll(a => a.Name == p[0]);
                        param.Add(new MAVLink.MAVLinkParam(p[0], double.Parse(p[1]), MAVLink.MAV_PARAM_TYPE.REAL32));
                    }

                    update.Invoke(fp, null);
                    Assert.AreEqual(1, overlay.Polygons.Count, "drawn while the graph's timer runs");

                    // the graph's timer stops: the box is not ticked
                    fp.CHK_elevationgraph.Checked = false;
                    timer.Invoke(fp, null);
                    Assert.AreEqual(0, overlay.Polygons.Count, "taken off with the graph");
                }
                finally
                {
                    MainV2.comPort = previous;
                }
            }
        }
    }
}
