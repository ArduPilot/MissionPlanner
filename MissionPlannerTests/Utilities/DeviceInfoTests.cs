using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews.ConfigurationView;
using System.Collections.Generic;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass()]
    public class DeviceInfoTests
    {
        [TestMethod()]
        public void MavDevIdKnownValuesTest()
        {
            var expected = new Dictionary<uint, string>()
            {
                { 0, "Unknown" },
                { 6, "USB0" },
                { 14, "SERIAL1" },
                { 22, "SERIAL2" },
                { 30, "SERIAL3" },
                { 38, "SERIAL4" },
                { 46, "SERIAL5" },
                { 54, "SERIAL6" },
                { 62, "SERIAL7" },
                { 70, "SERIAL8" },
                { 78, "SERIAL9" },
                { 174, "NET_P1" },
                { 182, "NET_P2" },
                { 190, "NET_P3" },
                { 198, "NET_P4" },
                { 334, "CAN_D1_UC_S1" },
                { 414, "CAN_D2_UC_S1" },
                { 494, "SCR_SDEV1" },
                { 502, "SCR_SDEV2" },
            };

            foreach (var kvp in expected)
            {
                var info = new DeviceInfo(0, "MAV1_DEVID", kvp.Key);
                Assert.AreEqual(kvp.Value, info.DevType, "devid " + kvp.Key + " should decode to " + kvp.Value);
            }
        }

        [TestMethod()]
        public void MavDevIdUnknownValueFallsBackToNumberTest()
        {
            var info = new DeviceInfo(0, "MAV2_DEVID", 12345);
            Assert.AreEqual("12345", info.DevType);
        }
    }
}
