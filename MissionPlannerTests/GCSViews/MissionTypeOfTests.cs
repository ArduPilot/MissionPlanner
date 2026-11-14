using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews;

namespace MissionPlanner.GCSViews.Tests
{
    /// <summary>
    /// How the Plan page sorts a command into mission, fence or rally items in ALL mode: every
    /// FENCE_* command, including the legacy home centred circle, is a fence item, RALLY_POINT is a
    /// rally item, and anything else, including an id this build has no name for, belongs to the
    /// mission. Before a write the validation loop uses this on the parsed grid, so an UNKNOWN row
    /// is classified by the id it carries, the same way the upload treats it.
    /// </summary>
    [TestClass]
    public class MissionTypeOfTests
    {
        private static MAVLink.MAV_MISSION_TYPE TypeOf(MAVLink.MAV_CMD cmd)
        {
            return FlightPlanner.MissionTypeOf((ushort) cmd);
        }

        [TestMethod]
        public void FenceCommands_AreFenceItems()
        {
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.FENCE, TypeOf(MAVLink.MAV_CMD.FENCE_RETURN_POINT));
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.FENCE, TypeOf(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION));
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.FENCE, TypeOf(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION));
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.FENCE, TypeOf(MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION));
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.FENCE, TypeOf(MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION));
#pragma warning disable CS0612 // the legacy home centred fence is obsolete but still a fence item
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.FENCE, TypeOf(MAVLink.MAV_CMD.FENCE_HOME_CIRCLE_INCLUSION));
#pragma warning restore CS0612
        }

        [TestMethod]
        public void RallyPoint_IsARallyItem()
        {
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.RALLY, TypeOf(MAVLink.MAV_CMD.RALLY_POINT));
        }

        [TestMethod]
        public void EverythingElse_IsPartOfTheMission()
        {
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.MISSION, TypeOf(MAVLink.MAV_CMD.WAYPOINT));
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.MISSION, TypeOf(MAVLink.MAV_CMD.DO_SET_SERVO));
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.MISSION, TypeOf(MAVLink.MAV_CMD.DO_JUMP));
            // an id this build has no name for shows as UNKNOWN in the grid
            Assert.AreEqual(MAVLink.MAV_MISSION_TYPE.MISSION, FlightPlanner.MissionTypeOf(31000));
        }
    }
}
