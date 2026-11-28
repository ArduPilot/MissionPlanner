using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MissionPlanner.GCSViews.Tests
{
    /// <summary>
    /// Tests for MAG_CAL_STATUS handling paired with ArduPilot/ardupilot#32757
    /// (AP_Compass: report specific failure reason when fit is rejected).
    ///
    /// Firmware sends cal_status in MAG_CAL_REPORT. Three failure codes are
    /// exercised end-to-end here:
    ///   8 = MAG_CAL_FAILED_OFFSETS         – any offset component >= COMPASS_OFFS_MAX
    ///   9 = MAG_CAL_FAILED_DIAG_SCALING    – diagonal or off-diagonal scaling out of range
    ///  10 = MAG_CAL_FAILED_RESIDUALS_HIGH  – fitness (RMS residual) exceeds tolerance
    ///
    /// The named enum members now live in the generated Mavlink.cs (mavlink/mavlink#2478),
    /// so the wire-value checks below are real regression guards: if upstream ever
    /// renumbers a member or Mavlink.cs is regenerated against a diverged xml,
    /// these fail loudly.
    /// </summary>
    [TestClass]
    public class MagCalStatusTests
    {
        // ── 1. Wire values (regression guards) ────────────────────────────────
        //
        // Pin the byte value of every failure code the UI branches on. A silent
        // renumber upstream would otherwise break the >MAG_CAL_SUCCESS guard
        // used in both ConfigHWCompass and ConfigHWCompass2.

        [TestMethod]
        public void FailedOffsets_WireValue_Is8()
        {
            Assert.AreEqual((byte)8, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS);
        }

        [TestMethod]
        public void FailedDiagScaling_WireValue_Is9()
        {
            Assert.AreEqual((byte)9, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_DIAG_SCALING);
        }

        [TestMethod]
        public void FailedResidualsHigh_WireValue_Is10()
        {
            Assert.AreEqual((byte)10, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RESIDUALS_HIGH);
        }

        [TestMethod]
        public void KnownStatus_WireValues_ArePinned()
        {
            // Full snapshot of the enum contract the UI relies on. If any of
            // these shift, the >MAG_CAL_SUCCESS guard partitions incorrectly.
            Assert.AreEqual((byte)0, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_NOT_STARTED);
            Assert.AreEqual((byte)1, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_WAITING_TO_START);
            Assert.AreEqual((byte)2, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_RUNNING_STEP_ONE);
            Assert.AreEqual((byte)3, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_RUNNING_STEP_TWO);
            Assert.AreEqual((byte)4, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS);
            Assert.AreEqual((byte)5, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED);
            Assert.AreEqual((byte)6, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_ORIENTATION);
            Assert.AreEqual((byte)7, (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RADIUS);
        }

        // ── 2. Failure guard: calStatus > MAG_CAL_SUCCESS (value 4) ──────────
        //
        // The timer_Tick guard `if (calStatus > MAG_CAL_SUCCESS)` must capture
        // every failure code, including the three added by PR#32757.

        [TestMethod]
        public void FailedOffsets_IsGreaterThanSuccess()
        {
            Assert.IsTrue(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,
                "MAG_CAL_FAILED_OFFSETS must trigger the failure guard");
        }

        [TestMethod]
        public void FailedDiagScaling_IsGreaterThanSuccess()
        {
            Assert.IsTrue(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_DIAG_SCALING > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,
                "MAG_CAL_FAILED_DIAG_SCALING must trigger the failure guard");
        }

        [TestMethod]
        public void FailedResidualsHigh_IsGreaterThanSuccess()
        {
            Assert.IsTrue(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RESIDUALS_HIGH > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,
                "MAG_CAL_FAILED_RESIDUALS_HIGH must trigger the failure guard");
        }

        [TestMethod]
        public void AllFailureCodes_AreGreaterThanSuccess()
        {
            var failures = new[]
            {
                MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_ORIENTATION,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RADIUS,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_DIAG_SCALING,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RESIDUALS_HIGH,
            };
            foreach (var f in failures)
                Assert.IsTrue(f > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,
                    $"{f} (raw={(byte)f}) should be > MAG_CAL_SUCCESS");
        }

        [TestMethod]
        public void RunningAndWaiting_AreNotGreaterThanSuccess()
        {
            var nonFailed = new[]
            {
                MAVLink.MAG_CAL_STATUS.MAG_CAL_NOT_STARTED,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_WAITING_TO_START,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_RUNNING_STEP_ONE,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_RUNNING_STEP_TWO,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,
            };
            foreach (var s in nonFailed)
                Assert.IsFalse(s > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,
                    $"{s} should NOT be > MAG_CAL_SUCCESS");
        }

        // ── 3. Named string representation ────────────────────────────────────
        //
        // The UI prints `calStatus.ToString()` verbatim as a fallback when no
        // [Description] is available. Lock the names so a renamed member is caught.

        [TestMethod]
        public void Failed_ToStringIsNamed()
        {
            Assert.AreEqual("MAG_CAL_FAILED", MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED.ToString());
        }

        [TestMethod]
        public void FailedRadius_ToStringIsNamed()
        {
            Assert.AreEqual("MAG_CAL_FAILED_RADIUS", MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RADIUS.ToString());
        }

        // ── 4. MAVLink [Description] surfaces the failure reason ──────────────
        //
        // The compass config views print status text using the mavlink dialect's
        // [Description] attribute when present. We don't pin exact upstream wording
        // (mavlink is allowed to reword), but we do lock that each specific failure
        // code carries *some* non-empty description whose key term matches the code.
        // If this ever regresses to empty descriptions, the UI silently falls back
        // to bare enum names and the user loses the "why".

        private static string DescriptionOf(MAVLink.MAG_CAL_STATUS status)
        {
            var field = typeof(MAVLink.MAG_CAL_STATUS).GetField(status.ToString());
            var attr = field == null ? null
                : (MAVLink.Description)Attribute.GetCustomAttribute(field, typeof(MAVLink.Description));
            return attr?.Text ?? "";
        }

        [TestMethod]
        public void SpecificFailures_CarryDescriptionsWithKeyTerm()
        {
            var expectedTerms = new Dictionary<MAVLink.MAG_CAL_STATUS, string>
            {
                { MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_ORIENTATION,    "orientation" },
                { MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RADIUS,         "radius" },
                { MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS,        "offset" },
                { MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_DIAG_SCALING,   "scaling" },
                { MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RESIDUALS_HIGH, "fitness" },
            };

            foreach (var kv in expectedTerms)
            {
                var desc = DescriptionOf(kv.Key);
                Assert.IsFalse(string.IsNullOrEmpty(desc),
                    $"{kv.Key} has no [Description]; UI would fall back to bare enum name.");
                Assert.IsTrue(desc.IndexOf(kv.Value, StringComparison.OrdinalIgnoreCase) >= 0,
                    $"{kv.Key} description \"{desc}\" no longer contains key term \"{kv.Value}\".");
            }
        }
    }
}
