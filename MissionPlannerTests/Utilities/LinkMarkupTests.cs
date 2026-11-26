using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace MissionPlanner.Utilities.Tests
{
    /// <summary>
    /// Vehicle text shown in a message box must not create [link;target;label] links, and a link's
    /// label must not hide which site it opens.
    /// </summary>
    [TestClass]
    public class LinkMarkupTests
    {
        // the same pattern CustomMessageBox and Common.MessageShowAgain use to find a link
        private static readonly Regex linkregex = new Regex(@"(\[link;([^\]]+);([^\]]+)\])", RegexOptions.IgnoreCase);

        [TestMethod]
        public void NeutralisedVehicleTextHasNoLink()
        {
            var vehicle = "PreArm: Compass not calibrated\n" +
                          "[link;https://evil.example/x;https://ardupilot.org/copter/docs/common-compass-calibration.html]";

            var shown = CustomMessageBox.NeutraliseLinkMarkup(vehicle);

            Assert.IsFalse(linkregex.IsMatch(shown));
            StringAssert.Contains(shown, "PreArm: Compass not calibrated");
        }

        [TestMethod]
        public void NeutraliseIsCaseInsensitive()
        {
            Assert.IsFalse(linkregex.IsMatch(CustomMessageBox.NeutraliseLinkMarkup("[LINK;https://a.example;b]")));
            Assert.IsFalse(linkregex.IsMatch(CustomMessageBox.NeutraliseLinkMarkup("[Link;https://a.example;b]")));
        }

        [TestMethod]
        public void NeutraliseLeavesOrdinaryTextAlone()
        {
            Assert.AreEqual("PreArm: Battery 1 low voltage", CustomMessageBox.NeutraliseLinkMarkup("PreArm: Battery 1 low voltage"));
            Assert.AreEqual("", CustomMessageBox.NeutraliseLinkMarkup(""));
            Assert.IsNull(CustomMessageBox.NeutraliseLinkMarkup(null));
        }

        [TestMethod]
        public void LabelShowsTheRealHost()
        {
            Assert.AreEqual("https://ardupilot.org/copter/docs/x.html (evil.example)",
                CustomMessageBox.LinkDisplayText("https://evil.example/x", "https://ardupilot.org/copter/docs/x.html"));
            Assert.AreEqual("Release Notes (discuss.ardupilot.org)",
                CustomMessageBox.LinkDisplayText("https://discuss.ardupilot.org/tags/stable-release", "Release Notes"));
        }

        [TestMethod]
        public void LabelEqualToTargetIsUnchanged()
        {
            Assert.AreEqual("https://ardupilot.org/", CustomMessageBox.LinkDisplayText("https://ardupilot.org/", "https://ardupilot.org/"));
        }
    }
}
