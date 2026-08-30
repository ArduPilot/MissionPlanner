using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews.ConfigurationView;
using System.Collections.Generic;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass()]
    public class DeviceInfoTests
    {
        // devid = bus_type(6=BUS_TYPE_SERIAL) | (bus<<3) | (address<<8) | (devtype<<16),
        // matching AP_SerialManager::UARTState::get_device_id() and the values ArduPilot
        // publishes in GCS_MAVLink_Parameters.cpp for MAVn_DEVID.
        [TestMethod()]
        public void MavDevIdKnownValuesTest()
        {
            var expected = new Dictionary<uint, string>()
            {
                { 0, "Unknown" },
                { 65542, "SERIAL0 (USB)" },
                { 65798, "SERIAL1" },
                { 66054, "SERIAL2" },
                { 66310, "SERIAL3" },
                { 66566, "SERIAL4" },
                { 66822, "SERIAL5" },
                { 67078, "SERIAL6" },
                { 67334, "SERIAL7" },
                { 67590, "SERIAL8" },
                { 67846, "SERIAL9" },
                { 131078, "NET_P1" },
                { 131334, "NET_P2" },
                { 131590, "NET_P3" },
                { 131846, "NET_P4" },
                { 196614, "CAN_D1_UC_S1" },
                { 196870, "CAN_D1_UC_S2" },
                { 197126, "CAN_D1_UC_S3" },
                { 196622, "CAN_D2_UC_S1" },
                { 196878, "CAN_D2_UC_S2" },
                { 197134, "CAN_D2_UC_S3" },
                { 262150, "SCR_SDEV1" },
                { 262406, "SCR_SDEV2" },
                { 262662, "SCR_SDEV3" },
            };

            foreach (var kvp in expected)
            {
                var info = new DeviceInfo(0, "MAV1_DEVID", kvp.Key);
                Assert.AreEqual(kvp.Value, info.DevType, "devid " + kvp.Key + " should decode to " + kvp.Value);
            }
        }

        // SERIAL10 isn't in ArduPilot's published table, but the decode is closed-form on
        // (devtype, bus, address) rather than an enumerated list, so it still resolves.
        [TestMethod()]
        public void MavDevIdUnlistedSerialPortDecodesTest()
        {
            var info = new DeviceInfo(0, "MAV1_DEVID", 68102); // 6 | (10<<8) | (1<<16)
            Assert.AreEqual("SERIAL10", info.DevType);
        }

        // devtype=5 is not a defined AP_HAL::Device::DeviceType, so this must fall back to the
        // raw number rather than throwing or guessing.
        [TestMethod()]
        public void MavDevIdUnknownFamilyFallsBackToNumberTest()
        {
            var info = new DeviceInfo(0, "MAV2_DEVID", 327686); // 6 | (5<<16)
            Assert.AreEqual("327686", info.DevType);
        }
    }
}
