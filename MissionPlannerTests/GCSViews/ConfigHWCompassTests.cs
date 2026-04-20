using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MissionPlannerTests.GCSViews
{
    [TestClass]
    public class ConfigHWCompassTests
    {
        /// <summary>
        /// Same regex used in ConfigHWCompass and ConfigHWCompass2 to filter
        /// compass calibration failure STATUSTEXT messages.
        /// </summary>
        private static readonly Regex BadMagRegex = new Regex(@"Mag\((\d+)\) bad ");

        [TestMethod]
        public void BadMagRegex_MatchesBadOrientation()
        {
            Assert.IsTrue(BadMagRegex.IsMatch("Mag(0) bad orientation: 6/0 0.9"));
        }

        [TestMethod]
        public void BadMagRegex_MatchesBadRadius()
        {
            Assert.IsTrue(BadMagRegex.IsMatch("Mag(0) bad radius: 456.7"));
        }

        [TestMethod]
        public void BadMagRegex_MatchesBadFit()
        {
            Assert.IsTrue(BadMagRegex.IsMatch("Mag(0) bad fit: fitness 145.3 tolerance 100"));
        }

        [TestMethod]
        public void BadMagRegex_MatchesMultipleCompassIds()
        {
            Assert.IsTrue(BadMagRegex.IsMatch("Mag(1) bad orientation: 3/2 0.5"));
            Assert.IsTrue(BadMagRegex.IsMatch("Mag(2) bad fit: fitness 200 tolerance 100"));
        }

        [TestMethod]
        public void BadMagRegex_DoesNotMatchUnrelatedStatusText()
        {
            Assert.IsFalse(BadMagRegex.IsMatch("PreArm: compass not healthy"));
            Assert.IsFalse(BadMagRegex.IsMatch("EKF3 IMU0 started"));
            Assert.IsFalse(BadMagRegex.IsMatch("Calibration started"));
            Assert.IsFalse(BadMagRegex.IsMatch("Mag field 523"));
        }

        [TestMethod]
        public void BadFitMessages_KeepsLatestPerCompassId()
        {
            var badFitMessages = new Dictionary<string, string>();
            var messages = new[]
            {
                "Mag(0) bad orientation: 6/0 0.9",
                "EKF3 IMU0 started",
                "Mag(1) bad fit: fitness 145.3 tolerance 100",
                "PreArm: compass not healthy",
                "Mag(0) bad radius: 456.7"
            };

            foreach (var text in messages)
            {
                var match = BadMagRegex.Match(text);
                if (match.Success)
                {
                    badFitMessages[match.Groups[1].Value] = text;
                }
            }

            Assert.AreEqual(2, badFitMessages.Count);
            Assert.AreEqual("Mag(0) bad radius: 456.7", badFitMessages["0"]);
            Assert.AreEqual("Mag(1) bad fit: fitness 145.3 tolerance 100", badFitMessages["1"]);
        }

        [TestMethod]
        public void NullTerminatedString_IsTrimmedCorrectly()
        {
            // Simulates how STATUSTEXT messages are processed:
            // ASCII bytes with null padding
            var text = "Mag(0) bad fit: fitness 145.3\0\0\0\0\0\0";
            int nul = text.IndexOf('\0');
            if (nul != -1) text = text.Substring(0, nul);

            Assert.AreEqual("Mag(0) bad fit: fitness 145.3", text);
            Assert.IsTrue(BadMagRegex.IsMatch(text));
        }
    }
}
