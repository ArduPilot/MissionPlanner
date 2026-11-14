using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews;
using MissionPlanner.Utilities;

namespace MissionPlanner.Tests.GCSViews
{
    /// <summary>
    /// The Plan page keeps the grid's rows of each item type while the dropdown shows another
    /// type, so switching between MISSION, FENCE, RALLY and ALL does not throw away rows that
    /// have not been written to the vehicle. The private helpers behind the dropdown handler
    /// (KeepGridRows, RowsToShow, AllRowsToShow) are driven by reflection, so no dialog opens.
    /// </summary>
    [TestClass]
    public class KeptRowsTests
    {
        // a made up position: round numbers, nowhere in particular
        private const double Lat0 = 10.5;
        private const double Lng0 = 20.25;

        private static FlightPlanner NewPlanner()
        {
            var fp = new FlightPlanner();
            fp.quickadd = true; // no map redraw
            MainV2.comPort.MAV.wps.Clear();
            MainV2.comPort.MAV.fencepoints.Clear();
            MainV2.comPort.MAV.rallypoints.Clear();
            return fp;
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

        private static void Keep(FlightPlanner fp, MAVLink.MAV_MISSION_TYPE shown)
        {
            var args = new object[] {shown, null};
            var kept = (bool) typeof(FlightPlanner)
                .GetMethod("KeepGridRows", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(fp, args);
            Assert.IsTrue(kept, (string) args[1]);
        }

        private static int AddRowFor(FlightPlanner fp, MAVLink.MAV_MISSION_TYPE type, string command)
        {
            var row = (int) typeof(FlightPlanner)
                .GetMethod("AddRowFor", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(fp, new object[] {type});
            fp.Commands.Rows[row].Cells["Command"].Value = command;
            return row;
        }

        private static List<Locationwp> RowsToShow(FlightPlanner fp, MAVLink.MAV_MISSION_TYPE type)
        {
            return (List<Locationwp>) typeof(FlightPlanner)
                .GetMethod("RowsToShow", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(fp, new object[] {type});
        }

        private static List<Locationwp> AllRowsToShow(FlightPlanner fp)
        {
            return (List<Locationwp>) typeof(FlightPlanner)
                .GetMethod("AllRowsToShow", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(fp, null);
        }

        private static MAVLink.mavlink_mission_item_int_t Item(MAVLink.MAV_CMD cmd, double lat, double lng, double alt)
        {
            return new MAVLink.mavlink_mission_item_int_t
            {
                command = (ushort) cmd,
                x = (int) (lat * 1e7),
                y = (int) (lng * 1e7),
                z = (float) alt,
                frame = (byte) MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT
            };
        }

        [TestMethod]
        public void RallyRowsComeBackAfterShowingAnotherType()
        {
            var fp = NewPlanner();
            AddRow(fp, "RALLY_POINT", Lat0 + 0.01, Lng0, 80);
            AddRow(fp, "RALLY_POINT", Lat0 + 0.02, Lng0 + 0.01, 110);

            // the dropdown leaves RALLY: the two dropped points are kept, written or not
            Keep(fp, MAVLink.MAV_MISSION_TYPE.RALLY);

            var rally = RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.RALLY);
            Assert.AreEqual(2, rally.Count);
            Assert.AreEqual((ushort) MAVLink.MAV_CMD.RALLY_POINT, rally[0].id);
            Assert.AreEqual(Lat0 + 0.01, rally[0].lat, 1e-6);
            Assert.AreEqual(110, rally[1].alt, 1e-6);

            // a type never shown still comes from the vehicle's copy (MISSION is shown from the
            // start, so FENCE is the one never shown here)
            MainV2.comPort.MAV.fencepoints[0] = Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.05, Lng0 - 0.05, 0);
            MainV2.comPort.MAV.fencepoints[1] = Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.05, Lng0 + 0.05, 0);
            try
            {
                var fence = RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.FENCE);
                Assert.AreEqual(2, fence.Count);
                Assert.AreEqual(Lat0 + 0.05, fence[0].lat, 1e-6);
                Assert.AreEqual(Lng0 + 0.05, fence[1].lng, 1e-6);
            }
            finally
            {
                MainV2.comPort.MAV.fencepoints.Clear();
            }
        }

        [TestMethod]
        public void KeptMissionRowsGetAHomePlaceholder()
        {
            var fp = NewPlanner();
            AddRow(fp, "TAKEOFF", 0, 0, 40);
            AddRow(fp, "WAYPOINT", Lat0 + 0.01, Lng0, 100);
            Keep(fp, MAVLink.MAV_MISSION_TYPE.MISSION);

            // processToScreen strips the first row of a mission list as home, so the kept rows
            // are led by a placeholder at 0, 0 and not by the takeoff
            var mission = RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.MISSION);
            Assert.AreEqual(3, mission.Count);
            Assert.AreEqual(0, mission[0].lat, 1e-9);
            Assert.AreEqual(0, mission[0].lng, 1e-9);
            Assert.AreEqual((ushort) MAVLink.MAV_CMD.TAKEOFF, mission[1].id);
            Assert.AreEqual((ushort) MAVLink.MAV_CMD.WAYPOINT, mission[2].id);

            // nothing kept shows as nothing: a lone placeholder would reach processToScreen,
            // which cannot run before the page has a handle (the dropdown's first event does)
            fp.Commands.Rows.Clear();
            Keep(fp, MAVLink.MAV_MISSION_TYPE.MISSION);
            Assert.AreEqual(0, RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.MISSION).Count);
            Assert.AreEqual(0, AllRowsToShow(fp).Count);
        }

        [TestMethod]
        public void ANewPageHasAnEmptyGrid()
        {
            // the dropdown's first event, during construction, kept an empty mission and showed it
            // back as a placeholder row that stayed in the grid (and left quickadd set)
            var fp = new FlightPlanner();
            Assert.AreEqual(0, fp.Commands.Rows.Count);
            Assert.IsFalse(fp.quickadd);
        }

        [TestMethod]
        public void AllModeSplitsAndJoinsTheKeptRows()
        {
            var fp = NewPlanner();
            AddRow(fp, "WAYPOINT", Lat0 + 0.01, Lng0, 100);
            AddRow(fp, "FENCE_POLYGON_VERTEX_INCLUSION", Lat0 + 0.05, Lng0 - 0.05, 0);
            AddRow(fp, "RALLY_POINT", Lat0 + 0.02, Lng0 + 0.01, 110);

            // leaving ALL keeps each type's rows apart
            Keep(fp, MAVLink.MAV_MISSION_TYPE.ALL);
            Assert.AreEqual(1, RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.FENCE).Count);
            Assert.AreEqual(1, RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.RALLY).Count);
            Assert.AreEqual(2, RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.MISSION).Count); // placeholder + waypoint

            // and ALL joins them again: home placeholder, mission, fence, rally
            var all = AllRowsToShow(fp);
            CollectionAssert.AreEqual(
                new[]
                {
                    (ushort) MAVLink.MAV_CMD.WAYPOINT, (ushort) MAVLink.MAV_CMD.WAYPOINT,
                    (ushort) MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, (ushort) MAVLink.MAV_CMD.RALLY_POINT
                },
                all.Select(a => a.id).ToArray());
            Assert.AreEqual(0, all[0].lat, 1e-9);
            Assert.AreEqual(Lat0 + 0.01, all[1].lat, 1e-6);
        }

        [TestMethod]
        public void AllModeWithNothingKeptLeadsWithAPlaceholder()
        {
            var fp = NewPlanner();
            MainV2.comPort.MAV.rallypoints[0] = Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0 + 0.02, Lng0, 90);
            try
            {
                // no mission anywhere: the rally row must not be taken for home
                var all = AllRowsToShow(fp);
                Assert.AreEqual(2, all.Count);
                Assert.AreEqual(0, all[0].lat, 1e-9);
                Assert.AreEqual((ushort) MAVLink.MAV_CMD.RALLY_POINT, all[1].id);
            }
            finally
            {
                MainV2.comPort.MAV.rallypoints.Clear();
            }
        }

        [TestMethod]
        public void TheGridAsLastShownWinsOverOlderKeptRows()
        {
            var fp = NewPlanner();
            AddRow(fp, "RALLY_POINT", Lat0 + 0.01, Lng0, 80);
            Keep(fp, MAVLink.MAV_MISSION_TYPE.RALLY);

            // shown again and edited (a read, a drop, a delete): the grid is what is kept next
            fp.Commands.Rows.Clear();
            AddRow(fp, "RALLY_POINT", Lat0 + 0.04, Lng0, 60);
            AddRow(fp, "RALLY_POINT", Lat0 + 0.05, Lng0, 70);
            Keep(fp, MAVLink.MAV_MISSION_TYPE.RALLY);

            var rally = RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.RALLY);
            Assert.AreEqual(2, rally.Count);
            Assert.AreEqual(60, rally[0].alt, 1e-6);

            // every row of a type the vehicle holds deleted: that is an unsent change too, kept as
            // such and not replaced by the vehicle's copy
            MainV2.comPort.MAV.rallypoints[0] = Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0 + 0.02, Lng0, 90);
            try
            {
                fp.Commands.Rows.Clear();
                Keep(fp, MAVLink.MAV_MISSION_TYPE.RALLY);
                Assert.AreEqual(0, RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.RALLY).Count);
            }
            finally
            {
                MainV2.comPort.MAV.rallypoints.Clear();
            }
        }

        /// <summary>
        /// A connect, as far as the Plan page sees one: MAVLinkInterface.Open clears the MAV list, so
        /// MainV2.comPort.MAV is another MAVState with its own, empty, copies of the three lists.
        /// Dispose puts the vehicle of before back.
        /// </summary>
        private sealed class Connect : IDisposable
        {
            private readonly uint sysid = MainV2.comPort.sysidcurrent;
            private readonly int compid = MainV2.comPort.compidcurrent;
            private readonly MAVState before = MainV2.comPort.MAV;

            public Connect()
            {
                MainV2.comPort.sysidcurrent = 77;
                MainV2.comPort.compidcurrent = 1;
                Assert.AreNotSame(before, MainV2.comPort.MAV);
                Clear();
            }

            private static void Clear()
            {
                MainV2.comPort.MAV.wps.Clear();
                MainV2.comPort.MAV.fencepoints.Clear();
                MainV2.comPort.MAV.rallypoints.Clear();
            }

            public void Dispose()
            {
                Clear();
                MainV2.comPort.sysidcurrent = sysid;
                MainV2.comPort.compidcurrent = compid;
                Clear();
            }
        }

        [TestMethod]
        public void AFenceDrawnOffline_IsStillThereAfterAConnect()
        {
            // PR 3780, review of 2026-11-10: the kept rows hung on the MAVState, which a connect
            // replaces. Plan a fence offline in ALL mode, switch to MISSION, connect, switch back
            // to ALL: the fence was gone.
            var fp = NewPlanner();
            AddRow(fp, "WAYPOINT", Lat0 + 0.01, Lng0, 100);
            AddRow(fp, "FENCE_POLYGON_VERTEX_INCLUSION", Lat0 + 0.05, Lng0 - 0.05, 0);
            AddRow(fp, "FENCE_POLYGON_VERTEX_INCLUSION", Lat0 + 0.05, Lng0 + 0.05, 0);
            AddRow(fp, "FENCE_POLYGON_VERTEX_INCLUSION", Lat0 - 0.05, Lng0, 0);
            Keep(fp, MAVLink.MAV_MISSION_TYPE.ALL);

            // MISSION is shown: only the waypoint stays in the grid
            fp.Commands.Rows.Clear();
            AddRow(fp, "WAYPOINT", Lat0 + 0.01, Lng0, 100);

            using (new Connect())
            {
                Keep(fp, MAVLink.MAV_MISSION_TYPE.MISSION);
                var all = AllRowsToShow(fp);
                CollectionAssert.AreEqual(
                    new[]
                    {
                        (ushort) MAVLink.MAV_CMD.WAYPOINT, (ushort) MAVLink.MAV_CMD.WAYPOINT,
                        (ushort) MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION,
                        (ushort) MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION,
                        (ushort) MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION
                    },
                    all.Select(a => a.id).ToArray(), "home placeholder, the waypoint, the fence drawn offline");
                Assert.AreEqual(Lat0 - 0.05, all[4].lat, 1e-6);
            }
        }

        [TestMethod]
        public void AnUnsentEdit_SurvivesAReconnect()
        {
            var fp = NewPlanner();
            MainV2.comPort.MAV.rallypoints[0] = Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0 + 0.02, Lng0, 90);
            MainV2.comPort.MAV.rallypoints[1] = Item(MAVLink.MAV_CMD.RALLY_POINT, Lat0 + 0.03, Lng0, 90);

            // the vehicle's two rally points as read, the second one raised and not written
            AddRow(fp, "RALLY_POINT", Lat0 + 0.02, Lng0, 90);
            AddRow(fp, "RALLY_POINT", Lat0 + 0.03, Lng0, 120);
            Keep(fp, MAVLink.MAV_MISSION_TYPE.RALLY);

            using (new Connect())
            {
                // a telemetry drop and a reconnect: the vehicle's copy has not been read again yet
                var rally = RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.RALLY);
                Assert.AreEqual(2, rally.Count);
                Assert.AreEqual(120, rally[1].alt, 1e-6);
            }
        }

        [TestMethod]
        public void RowsThatRepeatTheVehiclesCopy_AreNotShownForTheNextVehicle()
        {
            // PR 3780, review of 2026-10-22: rows kept from one vehicle showed up as the fence of
            // the next, and Write offered to send them to it
            var fp = NewPlanner();
            MainV2.comPort.MAV.fencepoints[0] = Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.05, Lng0 - 0.05, 0);
            MainV2.comPort.MAV.fencepoints[1] = Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 + 0.05, Lng0 + 0.05, 0);
            MainV2.comPort.MAV.fencepoints[2] = Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, Lat0 - 0.05, Lng0, 0);

            // the first vehicle's fence as read into the grid, untouched. The row's frame is not
            // the GLOBAL one a fence item is sent with, and does not count.
            AddRow(fp, "FENCE_POLYGON_VERTEX_INCLUSION", Lat0 + 0.05, Lng0 - 0.05, 0);
            AddRow(fp, "FENCE_POLYGON_VERTEX_INCLUSION", Lat0 + 0.05, Lng0 + 0.05, 0);
            AddRow(fp, "FENCE_POLYGON_VERTEX_INCLUSION", Lat0 - 0.05, Lng0, 0);
            foreach (System.Windows.Forms.DataGridViewRow row in fp.Commands.Rows)
                row.Cells["Frame"].Value = (int) MAVLink.MAV_FRAME.GLOBAL;
            Keep(fp, MAVLink.MAV_MISSION_TYPE.FENCE);

            // still shown from the first vehicle's copy while it is the vehicle
            Assert.AreEqual(3, RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.FENCE).Count);

            using (new Connect())
            {
                Assert.AreEqual(0, RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.FENCE).Count,
                    "the next vehicle has no fence, and the first one's is not offered to it");

                MainV2.comPort.MAV.fencepoints[0] = Item(MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION, Lat0, Lng0, 0);
                var fence = RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.FENCE);
                Assert.AreEqual(1, fence.Count);
                Assert.AreEqual((ushort) MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION, fence[0].id);
            }
        }

        [TestMethod]
        public void AMissionThatRepeatsTheVehiclesCopy_IsNotKept()
        {
            var fp = NewPlanner();
            // the vehicle's mission starts with home, which the grid does not hold as a row
            MainV2.comPort.MAV.wps[0] = Item(MAVLink.MAV_CMD.WAYPOINT, Lat0, Lng0, 0);
            MainV2.comPort.MAV.wps[1] = Item(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 40);
            MainV2.comPort.MAV.wps[2] = Item(MAVLink.MAV_CMD.WAYPOINT, Lat0 + 0.01, Lng0, 100);
            try
            {
                AddRow(fp, "TAKEOFF", 0, 0, 40);
                AddRow(fp, "WAYPOINT", Lat0 + 0.01, Lng0, 100);
                Keep(fp, MAVLink.MAV_MISSION_TYPE.MISSION);

                // from the vehicle's copy: its own home leads, not a placeholder at 0, 0
                var mission = RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.MISSION);
                Assert.AreEqual(3, mission.Count);
                Assert.AreEqual(Lat0, mission[0].lat, 1e-6);

                // one altitude changed: unsent, so the rows are kept and led by the placeholder
                fp.Commands.Rows[1].Cells["Alt"].Value = "130";
                Keep(fp, MAVLink.MAV_MISSION_TYPE.MISSION);
                mission = RowsToShow(fp, MAVLink.MAV_MISSION_TYPE.MISSION);
                Assert.AreEqual(3, mission.Count);
                Assert.AreEqual(0, mission[0].lat, 1e-9);
                Assert.AreEqual(130, mission[2].alt, 1e-6);
            }
            finally
            {
                MainV2.comPort.MAV.wps.Clear();
            }
        }

        [TestMethod]
        public void SameItem_ComparesAsTheVehicleStoresIt()
        {
            var item = new Locationwp
            {
                id = (ushort) MAVLink.MAV_CMD.WAYPOINT, frame = (byte) MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT,
                lat = Lat0, lng = Lng0, alt = 100.3f, p1 = 5, p4 = float.NaN
            };

            // through a grid cell and back: the float as text; and to the vehicle and back: the
            // position cut down to a 1e-7 degree step, not rounded to it (Locationwp.Convert)
            var back = item;
            back.alt = (float) double.Parse(((double) item.alt).ToString());
            back.lat = Lat0 + 1e-7;
            back.lng = Lng0 - 1e-7;
            Assert.IsTrue(FlightPlanner.SameItem(item, back));

            var moved = item;
            moved.lat = Lat0 + 2e-7;
            Assert.IsFalse(FlightPlanner.SameItem(item, moved), "two steps of what the vehicle stores");

            var higher = item;
            higher.alt = 100.4f;
            Assert.IsFalse(FlightPlanner.SameItem(item, higher));

            var terrain = item;
            terrain.frame = (byte) MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT;
            Assert.IsFalse(FlightPlanner.SameItem(item, terrain), "a frame change is an unsent edit");
            Assert.IsTrue(FlightPlanner.SameItem(item, terrain, false), "except on fence items, always sent as GLOBAL");

            var other = item;
            other.id = (ushort) MAVLink.MAV_CMD.SPLINE_WAYPOINT;
            Assert.IsFalse(FlightPlanner.SameItem(item, other));
        }

        [TestMethod]
        public void AllMode_NewRowsGoToTheEndOfTheirTypesRows()
        {
            // the vehicle numbers mission items among themselves, so a waypoint added below the fence
            // and rally rows would get another number there than in the grid (PR 3780, maintainer review)
            var fp = NewPlanner();
            fp.cmb_missiontype.SelectedIndexChanged -= fp.Cmb_missiontype_SelectedIndexChanged;
            fp.cmb_missiontype.SelectedItem = MAVLink.MAV_MISSION_TYPE.ALL;
            AddRow(fp, "WAYPOINT", Lat0 + 0.01, Lng0, 100);
            AddRow(fp, "FENCE_POLYGON_VERTEX_INCLUSION", Lat0, Lng0, 0);
            AddRow(fp, "RALLY_POINT", Lat0 + 0.02, Lng0, 80);

            Assert.AreEqual(1, AddRowFor(fp, MAVLink.MAV_MISSION_TYPE.MISSION, "WAYPOINT"));
            Assert.AreEqual(3, AddRowFor(fp, MAVLink.MAV_MISSION_TYPE.FENCE, "FENCE_POLYGON_VERTEX_INCLUSION"));
            Assert.AreEqual(5, AddRowFor(fp, MAVLink.MAV_MISSION_TYPE.RALLY, "RALLY_POINT"));

            var commands = fp.Commands.Rows.Cast<System.Windows.Forms.DataGridViewRow>()
                .Select(r => r.Cells["Command"].Value.ToString()).ToArray();
            CollectionAssert.AreEqual(new[]
            {
                "WAYPOINT", "WAYPOINT", "FENCE_POLYGON_VERTEX_INCLUSION", "FENCE_POLYGON_VERTEX_INCLUSION",
                "RALLY_POINT", "RALLY_POINT"
            }, commands);

            // outside ALL a row goes at the end, as before
            fp.cmb_missiontype.SelectedItem = MAVLink.MAV_MISSION_TYPE.MISSION;
            Assert.AreEqual(6, AddRowFor(fp, MAVLink.MAV_MISSION_TYPE.MISSION, "WAYPOINT"));
        }

        [TestMethod]
        public void MissionRowsFirst_OnlyWhileNoMissionRowFollowsAFenceOrRallyRow()
        {
            Func<MAVLink.MAV_CMD[], bool> first = cmds => FlightPlanner.MissionRowsFirst(
                cmds.Select(c => new Locationwp {id = (ushort) c}));

            Assert.IsTrue(first(new MAVLink.MAV_CMD[0]));
            Assert.IsTrue(first(new[]
            {
                MAVLink.MAV_CMD.WAYPOINT, MAVLink.MAV_CMD.DO_JUMP, MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION,
                MAVLink.MAV_CMD.RALLY_POINT, MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION
            }));
            Assert.IsFalse(first(new[]
            {
                MAVLink.MAV_CMD.WAYPOINT, MAVLink.MAV_CMD.RALLY_POINT, MAVLink.MAV_CMD.WAYPOINT
            }));
            Assert.IsFalse(first(new[] {MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, MAVLink.MAV_CMD.LAND}));
        }
    }
}
