using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews.ConfigurationView;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace MissionPlanner.GCSViews.Tests
{
    [TestClass]
    public class MagCalTrackerTests
    {
        private static MAVLink.mavlink_mag_cal_report_t Report(byte id, MAVLink.MAG_CAL_STATUS status,
            byte autosaved = 0, byte mask = 0, float ofsX = 1f)
        {
            return new MAVLink.mavlink_mag_cal_report_t
            {
                compass_id = id,
                cal_mask = mask,
                cal_status = (byte)status,
                autosaved = autosaved,
                ofs_x = ofsX,
            };
        }

        private static MAVLink.mavlink_mag_cal_progress_t Progress(byte id, byte pct, byte attempt = 1, byte mask = 0)
        {
            return new MAVLink.mavlink_mag_cal_progress_t
            {
                compass_id = id,
                cal_mask = mask,
                attempt = attempt,
                completion_pct = pct,
            };
        }

        [TestMethod]
        public void AllZeroNotStartedReport_IsIgnored()
        {
            var t = new MagCalTracker();

            var saved = t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_NOT_STARTED, ofsX: 0f));

            Assert.IsFalse(saved);
            Assert.AreEqual(0, t.Reports.Count());
            Assert.AreEqual(0, t.ExpectedCount);
        }

        [TestMethod]
        public void Report_SignalsSaveOnlyWhenAutosaved()
        {
            var t = new MagCalTracker();

            Assert.IsFalse(t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, autosaved: 0)));
            Assert.IsTrue(t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, autosaved: 1)));
        }

        [TestMethod]
        public void IsComplete_NeedsEveryCompassInTheMask()
        {
            var t = new MagCalTracker();

            t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, mask: 0b011));
            Assert.IsFalse(t.IsComplete, "compass 1 has not reported yet");

            t.Apply(Report(1, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, mask: 0b011));
            Assert.IsTrue(t.IsComplete);
        }

        [TestMethod]
        public void IsComplete_FalseWhileAnyCompassFailed()
        {
            var t = new MagCalTracker();

            t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, mask: 0b011));
            t.Apply(Report(1, MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS, mask: 0b011));

            Assert.IsFalse(t.IsComplete);
            Assert.IsTrue(t.AnyFailed);
            Assert.IsTrue(t.IsFailed(1));
            Assert.IsFalse(t.IsFailed(0));
        }

        [TestMethod]
        public void LaterSuccessReplacesEarlierFailure()
        {
            var t = new MagCalTracker();

            t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RESIDUALS_HIGH, mask: 0b001));
            Assert.IsTrue(t.IsFailed(0));

            t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, mask: 0b001));

            Assert.IsFalse(t.IsFailed(0));
            Assert.IsTrue(t.IsComplete);
        }

        [TestMethod]
        public void SuccessWithoutAutosave_CompletesAndLaterSavedReportStillSignals()
        {
            var t = new MagCalTracker();

            Assert.IsFalse(t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, autosaved: 0, mask: 0b001)));
            Assert.IsTrue(t.IsComplete, "completion must not wait for autosaved == 1");

            Assert.IsTrue(t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, autosaved: 1, mask: 0b001)));
        }

        [TestMethod]
        public void Percent_UsesStreamUntilSuccessThenPinsTo100()
        {
            var t = new MagCalTracker();
            Assert.AreEqual(0, t.Percent(0), "no data yet");

            t.Apply(Progress(0, 99));
            Assert.AreEqual(99, t.Percent(0));

            t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS));
            Assert.AreEqual(100, t.Percent(0));
        }

        [TestMethod]
        public void Attempt_IsLastProgressAttemptOrZero()
        {
            var t = new MagCalTracker();
            Assert.AreEqual(0, t.Attempt(0));

            t.Apply(Progress(0, 10, attempt: 1));
            t.Apply(Progress(0, 5, attempt: 2));

            Assert.AreEqual(2, t.Attempt(0));
        }

        [TestMethod]
        public void CompassIds_IncludeMaskedCompassesWithoutPackets_AndStayBelowMax()
        {
            var t = new MagCalTracker();
            t.Apply(Progress(1, 0, mask: 0b1101));

            CollectionAssert.AreEqual(new byte[] { 0, 1, 2 }, t.CompassIds(3).ToArray());
        }

        [TestMethod]
        public void ExpectedCount_FallsBackToReportsWhenNoMaskSeen()
        {
            var t = new MagCalTracker();
            t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, mask: 0));

            Assert.AreEqual(1, t.ExpectedCount);
        }

        [TestMethod]
        public void AutosavedSuccessIds_AreSortedAndExcludeUnsavedAndFailed()
        {
            var t = new MagCalTracker();
            t.Apply(Report(2, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, autosaved: 1));
            t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, autosaved: 1));
            t.Apply(Report(1, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, autosaved: 0));
            t.Apply(Report(3, MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS, autosaved: 1));

            CollectionAssert.AreEqual(new byte[] { 0, 2 }, t.AutosavedSuccessIds());
        }

        [TestMethod]
        public void Clear_ResetsEverything()
        {
            var t = new MagCalTracker();
            t.Apply(Progress(0, 50, attempt: 2, mask: 0b011));
            t.Apply(Report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED, mask: 0b011));

            t.Clear();

            Assert.AreEqual(0, t.ExpectedCount);
            Assert.AreEqual(0, t.Attempt(0));
            Assert.AreEqual(0, t.Percent(0));
            Assert.IsFalse(t.AnyFailed);
            Assert.AreEqual(0, t.CompassIds(3).Count);
        }
    }

    [TestClass]
    public class MagCalTextTests
    {
        private CultureInfo _culture;

        [TestInitialize]
        public void UseInvariantCulture()
        {
            _culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestCleanup]
        public void RestoreCulture()
        {
            Thread.CurrentThread.CurrentCulture = _culture;
        }

        private static MAVLink.mavlink_mag_cal_report_t Report(MAVLink.MAG_CAL_STATUS status)
        {
            return new MAVLink.mavlink_mag_cal_report_t
            {
                cal_status = (byte)status,
                ofs_x = 1.25f,
                ofs_y = -2.5f,
                ofs_z = 3f,
                fitness = 12.55f,
                orientation_confidence = 1.5f,
            };
        }

        [TestMethod]
        public void Status_UsesReadableFallbackWhenUpstreamDescriptionIsEmpty()
        {
            Assert.AreEqual("Success", MagCalText.Status(MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS));
            Assert.AreEqual("Calibration failed", MagCalText.Status(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED));
        }

        [TestMethod]
        public void Status_UsesUpstreamDescriptionForSpecificFailures()
        {
            StringAssert.Contains(MagCalText.Status(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS).ToLowerInvariant(), "offset");
        }

        [TestMethod]
        public void Status_UnknownValueFallsBackToItsNumber()
        {
            Assert.AreEqual("200", MagCalText.Status((MAVLink.MAG_CAL_STATUS)200));
        }

        [TestMethod]
        public void Detail_Success_ShowsOffsetsAndFitness()
        {
            Assert.AreEqual("  [x:1.3 y:-2.5 z:3.0  fit:12.6]",
                MagCalText.Detail(Report(MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS), null));
        }

        [TestMethod]
        public void Detail_FailedOffsets_ShowsLimitOnlyWhenParamIsKnown()
        {
            var r = Report(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS);

            Assert.AreEqual("  [x:1.3 y:-2.5 z:3.0  limit:\u00b11800]", MagCalText.Detail(r, 1800f));
            Assert.AreEqual("  [x:1.3 y:-2.5 z:3.0]", MagCalText.Detail(r, null));
        }

        [TestMethod]
        public void Detail_FailedResidualsHigh_ShowsFitness()
        {
            Assert.AreEqual("  [fit:12.6]",
                MagCalText.Detail(Report(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RESIDUALS_HIGH), null));
        }

        [TestMethod]
        public void Detail_FailedOrientation_ShowsConfidenceWithoutInventedThreshold()
        {
            var text = MagCalText.Detail(Report(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_ORIENTATION), null);

            Assert.AreEqual("  [orientation confidence:1.50]", text);
        }

        [TestMethod]
        public void Detail_IsEmptyForOtherStatuses()
        {
            Assert.AreEqual(string.Empty, MagCalText.Detail(Report(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED), null));
            Assert.AreEqual(string.Empty, MagCalText.Detail(Report(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RADIUS), null));
            Assert.AreEqual(string.Empty, MagCalText.Detail(Report(MAVLink.MAG_CAL_STATUS.MAG_CAL_RUNNING_STEP_ONE), null));
        }
    }
}
