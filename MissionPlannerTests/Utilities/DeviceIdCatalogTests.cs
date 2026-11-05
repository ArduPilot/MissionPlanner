using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using HardwareDeviceInfo = MissionPlanner.GCSViews.ConfigurationView.DeviceInfo;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass]
    public class DeviceIdCatalogTests
    {
        [DataTestMethod]
        [DataRow(0x40, "INS_ADIS16607")]
        [DataRow(0x41, "INS_ZEROONE_FPGA_SCH16T")]
        [DataRow(0x42, "INS_LSM6DSV32X")]
        [DataRow(0x43, "INS_LSM6DSK320X")]
        [DataRow(0x44, "INS_ICM56686")]
        public void NewImuTypesUseEmbeddedCatalog(int type, string name)
        {
            Assert.AreEqual("DEVTYPE_" + name, DeviceIdCatalog.GetName("imu", type, "unknown"));

            // SPI bus 5, chip select 107, with the device type in bits 16..23.
            var id = (uint)((type << 16) | (107 << 8) | (5 << 3) | 2);
            var device = new Device.DeviceStructure("INS_ACC_ID", id);
            var info = new HardwareDeviceInfo(0, "INS_ACC_ID", id);

            Assert.AreEqual(name, info.DevType);
            Assert.AreEqual("SPI", info.BusType);
            Assert.AreEqual(5, info.Bus);
            Assert.AreEqual(107, info.Address);
            StringAssert.Contains(device.ToString(), "devtype " + name + " ");
        }

        [DataTestMethod]
        [DataRow("COMPASS_DEV_ID", 1, "HMC5883_OLD")]
        [DataRow("COMPASS_DEV_ID", 0x13, "MMC5983")]
        [DataRow("INS_ACC_ID", 0x3e, "INS_LSM6DSV16X")]
        [DataRow("BARO_DEVID", 1, "BARO_SITL")]
        [DataRow("ARSPD_DEVID", 1, "AIRSPEED_SITL")]
        [DataRow("ARSPD2_DEVID", 1, "AIRSPEED_SITL")]
        [DataRow("ASP_DEVID", 1, "AIRSPEED_SITL")]
        [DataRow("INS_GYR_ID", 0x21, "GYR_MPU6000")]
        public void ExistingTypesAreResolvedWithinTheirCategory(string parameter, int type, string name)
        {
            var id = (uint)((type << 16) | 1);
            Assert.AreEqual(name, new HardwareDeviceInfo(0, parameter, id).DevType);
            StringAssert.Contains(new Device.DeviceStructure(parameter, id).ToString(), "devtype " + name + " ");
        }

        [TestMethod]
        public void UnknownTypesAndCategoriesKeepFallback()
        {
            Assert.AreEqual("legacy", DeviceIdCatalog.GetName("imu", 255, "legacy"));
            Assert.AreEqual("legacy", DeviceIdCatalog.GetName("unknown", 65, "legacy"));
            Assert.AreEqual("legacy", DeviceIdCatalog.GetName(null, 65, "legacy"));
            var id = (uint)((255 << 16) | 2);
            Assert.AreEqual("255", new HardwareDeviceInfo(0, "INS_ACC_ID", id).DevType);
            StringAssert.Contains(new Device.DeviceStructure("INS_ACC_ID", id).ToString(), "devtype 255 ");
        }

        [DataTestMethod]
        [DataRow("COMPASS_DEV_ID")]
        [DataRow("BARO_DEVID")]
        [DataRow("ARSPD_DEVID")]
        [DataRow("INS_ACC_ID")]
        public void DroneCanTypesRemainSensorIds(string parameter)
        {
            var id = (uint)((0x41 << 16) | (42 << 8) | (2 << 3) | 3);
            var info = new HardwareDeviceInfo(0, parameter, id);
            Assert.AreEqual("SENSOR_ID#65", info.DevType);
            Assert.AreEqual("UAVCAN", info.BusType);
            Assert.AreEqual(42, info.Address);
            Assert.AreEqual(2, info.Bus);
        }
    }
}
