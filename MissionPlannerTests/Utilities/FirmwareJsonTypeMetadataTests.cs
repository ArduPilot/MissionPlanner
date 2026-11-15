using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace MissionPlanner.Utilities.Tests
{
    /// <summary>
    /// A firmware file's JSON header must not be able to choose which type is created while it is
    /// parsed ($type / $types metadata, CWE-502).
    /// </summary>
    [TestClass]
    public class FirmwareJsonTypeMetadataTests
    {
        public class Gadget
        {
            public static int Created;

            public int board_id;

            public Gadget()
            {
                Created++;
            }
        }

        private static readonly string GadgetType = typeof(Gadget).AssemblyQualifiedName;

        private static px4uploader.Firmware Load(string json)
        {
            return px4uploader.Firmware.ProcessFirmware(
                new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(json))));
        }

        [TestInitialize]
        public void Reset()
        {
            Gadget.Created = 0;
        }

        [TestMethod]
        public void TypesTableIsIgnored()
        {
            var json = "{\"$types\":{\"" + GadgetType + "\":\"1\"},\"$type\":\"1\",\"board_id\":9}";

            var fw = Load(json);

            Assert.AreEqual(0, Gadget.Created, "the $types entry must not create the named type");
            Assert.IsInstanceOfType(fw, typeof(px4uploader.Firmware));
            Assert.AreEqual(9, fw.board_id);
        }

        [TestMethod]
        public void TypeNameIsIgnored()
        {
            var json = "{\"$type\":\"" + GadgetType + "\",\"board_id\":140}";

            var fw = Load(json);

            Assert.AreEqual(0, Gadget.Created, "the $type entry must not create the named type");
            Assert.AreEqual(140, fw.board_id);
        }

        [TestMethod]
        public void PlainHeaderStillParses()
        {
            var fw = Load("{\"board_id\":9,\"magic\":\"APJFWv1\",\"description\":\"Firmware for a STM32F427 board\"," +
                          "\"image_size\":0,\"build_time\":1700000000,\"summary\":\"PX4FMUv3\",\"version\":\"0.1\"," +
                          "\"git_identity\":\"abcdef\",\"board_revision\":0,\"USBID\":\"0x1209/0x5741\"}");

            Assert.AreEqual(9, fw.board_id);
            Assert.AreEqual("APJFWv1", fw.magic);
            Assert.AreEqual(1700000000u, fw.build_time);
            Assert.AreEqual("0x1209/0x5741", fw.USBID);
        }
    }
}
