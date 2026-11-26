using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using System.Text;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass]
    public class OsdTuningSlotProviderTests
    {
        [TestMethod]
        public void DecodeParamIdStopsAtNullTerminator()
        {
            var paramId = new byte[16];
            Encoding.UTF8.GetBytes("SHORT_NAME").CopyTo(paramId, 0);

            Assert.AreEqual("SHORT_NAME", OsdTuningSlotProvider.DecodeParamId(paramId));
        }

        [TestMethod]
        public void DecodeParamIdUsesAllBytesForSixteenCharacterName()
        {
            var paramId = Encoding.UTF8.GetBytes("PARAM_NAME_12345");

            Assert.AreEqual(16, paramId.Length);
            Assert.AreEqual("PARAM_NAME_12345", OsdTuningSlotProvider.DecodeParamId(paramId));
        }
    }
}
