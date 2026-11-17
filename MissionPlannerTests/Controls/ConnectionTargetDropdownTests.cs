using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Controls;

namespace MissionPlannerTests.Controls
{
    [TestClass]
    [DoNotParallelize]
    public class ConnectionTargetDropdownTests
    {
        private static MAVLink.MAVLinkMessage Heartbeat(uint system, byte component, MAVLink.MAV_AUTOPILOT autopilot,
            MAVLink.MAV_TYPE type = MAVLink.MAV_TYPE.HEXAROTOR)
        {
            return new MAVLink.MAVLinkMessage { buffer = new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(
                MAVLink.MAVLINK_MSG_ID.HEARTBEAT,
                new MAVLink.mavlink_heartbeat_t { autopilot = (byte)autopilot, type = (byte)type, mavlink_version = 3 },
                false, system, component) };
        }

        [TestMethod]
        [TestCategory("GUI")]
        public void DropdownSharesRankingAndPreservesSelectedDeviceAcrossDiscoveryAndConnections()
        {
            Exception failure = null;
            var thread = new Thread(() =>
            {
                try
                {
                    CheckDropdown();
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            }) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            Assert.IsTrue(thread.Join(TimeSpan.FromSeconds(15)), "Dropdown check did not finish.");
            if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
        }

        private static uint[] Systems(ConnectionControl control)
        {
            return control.cmb_sysid.Items.Cast<object>().Select(item =>
                (uint)item.GetType().GetField("sysid", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(item)).ToArray();
        }

        private static void CheckDropdown()
        {
            var oldPorts = MainV2.Comports.ToArray();
            var oldSelectedPort = MainV2.comPort;
            try
            {
                using (var first = new MAVLinkInterface())
                using (var second = new MAVLinkInterface())
                using (var control = new ConnectionControl())
                {
                    first.BaseStream = new CommsInjection { PortName = "first" };
                    second.BaseStream = new CommsInjection { PortName = "second" };
                    MainV2.Comports.Clear();
                    MainV2.Comports.Add(first);
                    MainV2.Comports.Add(second);
                    MainV2.comPort = first;
                    first.MAVlist.ObserveHeartbeat(Heartbeat(11, 1, MAVLink.MAV_AUTOPILOT.INVALID));
                    first.MAVlist.ObserveHeartbeat(Heartbeat(3, 100, MAVLink.MAV_AUTOPILOT.GENERIC));
                    first.MAVlist.ObserveHeartbeat(Heartbeat(255, 42, MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA, MAVLink.MAV_TYPE.GCS));
                    first.MAVlist.Create(20, 100);
                    first.MAVlist[20, 100].CANNode = true;
                    // The same IDs on another connection must not steal selection.
                    second.MAVlist.ObserveHeartbeat(Heartbeat(11, 1, MAVLink.MAV_AUTOPILOT.PX4));
                    first.sysidcurrent = 11;
                    first.compidcurrent = 1;

                    control.UpdateSysIDS();
                    CollectionAssert.AreEqual(new uint[] { 3, 11, 20, 11 }, Systems(control));
                    Assert.AreEqual(1, control.cmb_sysid.SelectedIndex);
                    var selected = control.cmb_sysid.SelectedItem;

                    first.MAVlist.ObserveHeartbeat(Heartbeat(1, 1, MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA));
                    control.UpdateSysIDS();
                    CollectionAssert.AreEqual(new uint[] { 1, 3, 11, 20, 11 }, Systems(control));
                    Assert.AreEqual(2, control.cmb_sysid.SelectedIndex);
                    Assert.AreEqual(selected, control.cmb_sysid.SelectedItem);
                    Assert.AreSame(first, MainV2.comPort);
                    Assert.AreEqual(11u, first.sysidcurrent);
                }
            }
            finally
            {
                MainV2.Comports.Clear();
                MainV2.Comports.AddRange(oldPorts);
                MainV2.comPort = oldSelectedPort;
            }
        }
    }
}
