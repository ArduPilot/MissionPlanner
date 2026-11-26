using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Field = MissionPlanner.ArduPilot.FirmwareFilter.Field;

namespace MissionPlanner.ArduPilot.Tests
{
    [TestClass()]
    public class FirmwareFilterTests
    {
        private static APFirmware.FirmwareInfo FW(string platform, string type, string reltype, string version,
            long boardid, string format = "apj", string[] usbid = null)
        {
            return new APFirmware.FirmwareInfo
            {
                Platform = platform,
                MavType = type,
                MavFirmwareVersionType = reltype,
                MavFirmwareVersion = new Version(version),
                BoardId = boardid,
                Format = format,
                Usbid = usbid ?? new string[0],
                Url = new Uri("https://firmware.ardupilot.org/" + platform + "/" + type + "/" + reltype + "/" +
                              version + "." + format)
            };
        }

        private static List<APFirmware.FirmwareInfo> Sample()
        {
            return new List<APFirmware.FirmwareInfo>
            {
                FW("CubeOrange", "QUADROTOR", "OFFICIAL", "4.5.7", 140, usbid: new[] { "0x2DAE/0x1016" }),
                FW("CubeOrange-bdshot", "QUADROTOR", "OFFICIAL", "4.5.7", 140, usbid: new[] { "0x2DAE/0x1016" }),
                FW("CubeOrange", "FIXED_WING", "OFFICIAL", "4.5.7", 140),
                FW("CubeOrange", "QUADROTOR", "BETA", "4.6.0", 140),
                FW("CubeOrange", "QUADROTOR", "OFFICIAL", "4.5.7", 140, "hex"),
                FW("Pixhawk1", "QUADROTOR", "OFFICIAL", "4.5.7", 9),
            };
        }

        [TestMethod()]
        public void NoSelectionMatchesAll()
        {
            var filter = new FirmwareFilter(Sample());
            Assert.AreEqual(6, filter.GetMatches().Count);
        }

        [TestMethod()]
        public void SelectionsNarrowToSingleMatch()
        {
            var filter = new FirmwareFilter(Sample());
            filter.Set(Field.Platform, "CubeOrange");
            filter.Set(Field.MavType, "QUADROTOR");
            filter.Set(Field.VersionType, "OFFICIAL");
            filter.Set(Field.Format, "apj");

            var matches = filter.GetMatches();
            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual("CubeOrange", matches[0].Platform);
        }

        [TestMethod()]
        public void NoMatches()
        {
            var filter = new FirmwareFilter(Sample());
            filter.Set(Field.Platform, "Pixhawk1");
            filter.Set(Field.VersionType, "BETA");
            Assert.AreEqual(0, filter.GetMatches().Count);
        }

        [TestMethod()]
        public void OptionsIgnoreOwnSelection()
        {
            var filter = new FirmwareFilter(Sample());
            filter.Set(Field.Platform, "Pixhawk1");

            // the selected field still offers every platform so the choice can be changed
            CollectionAssert.AreEqual(new[] { "CubeOrange", "CubeOrange-bdshot", "Pixhawk1" },
                filter.GetOptions(Field.Platform));
            // other fields only offer what is reachable from the selection
            CollectionAssert.AreEqual(new[] { "QUADROTOR" }, filter.GetOptions(Field.MavType));
            CollectionAssert.AreEqual(new[] { "9" }, filter.GetOptions(Field.BoardId));
        }

        [TestMethod()]
        public void ClearingSelectionWidens()
        {
            var filter = new FirmwareFilter(Sample());
            filter.Set(Field.MavType, "FIXED_WING");
            Assert.AreEqual(1, filter.GetMatches().Count);

            filter.Set(Field.MavType, null);
            Assert.AreEqual(6, filter.GetMatches().Count);
            Assert.IsNull(filter.Get(Field.MavType));
        }

        [TestMethod()]
        public void MultiValueFields()
        {
            var filter = new FirmwareFilter(Sample());
            CollectionAssert.AreEqual(new[] { "0x2DAE/0x1016" }, filter.GetOptions(Field.UsbId));

            filter.Set(Field.UsbId, "0x2DAE/0x1016");
            Assert.AreEqual(2, filter.GetMatches().Count);
        }

        [TestMethod()]
        public void BootloaderStrField()
        {
            var list = Sample();
            list[0].BootloaderStr = new[] { "CubeOrange-BL", "CubeOrange" };
            list[1].BootloaderStr = new[] { "CubeOrange-BL" };
            list[5].BootloaderStr = null;
            var filter = new FirmwareFilter(list);

            // entries with no or null bootloader strings are skipped, not an empty option
            CollectionAssert.AreEqual(new[] { "CubeOrange", "CubeOrange-BL" },
                filter.GetOptions(Field.BootloaderStr));

            filter.Set(Field.BootloaderStr, "CubeOrange-BL");
            Assert.AreEqual(2, filter.GetMatches().Count);

            filter.Set(Field.BootloaderStr, "CubeOrange");
            var matches = filter.GetMatches();
            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual("CubeOrange", matches[0].Platform);
        }

        [TestMethod()]
        public void NumericOrdering()
        {
            var list = new List<APFirmware.FirmwareInfo>
            {
                FW("a", "QUADROTOR", "OFFICIAL", "4.10.0", 140),
                FW("b", "QUADROTOR", "OFFICIAL", "4.9.1", 9),
                FW("c", "QUADROTOR", "OFFICIAL", "4.2.0", 1013),
            };
            var filter = new FirmwareFilter(list);

            CollectionAssert.AreEqual(new[] { "9", "140", "1013" }, filter.GetOptions(Field.BoardId));
            CollectionAssert.AreEqual(new[] { "4.2.0", "4.9.1", "4.10.0" }, filter.GetOptions(Field.Version));
        }

        [TestMethod()]
        public void NullInputIsEmpty()
        {
            var filter = new FirmwareFilter(null);
            Assert.AreEqual(0, filter.GetMatches().Count);
            Assert.AreEqual(0, filter.GetOptions(Field.Platform).Count);
        }
    }
}
