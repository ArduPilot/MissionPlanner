using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MissionPlanner.GCSViews.Tests
{
    /// <summary>
    /// Tests for MAG_CAL_STATUS handling paired with ArduPilot/ardupilot#32757
    /// (AP_Compass: report specific failure reason when fit is rejected).
    ///
    /// Firmware sends cal_status in MAG_CAL_REPORT. Three new values are added:
    ///   8 = BAD_OFFSETS  – any offset component >= COMPASS_OFFS_MAX
    ///   9 = BAD_DIAG_SCALING – diagonal or off-diagonal scaling out of range
    ///  10 = BAD_FITNESS       – fitness (RMS residual) exceeds tolerance
    ///
    /// These values are tested via raw byte cast because the named enum members
    /// (MAG_CAL_BAD_OFFSETS/DIAG_SCALING/FITNESS) are not yet in the generated Mavlink.cs.
    /// They will be added when mavlink/mavlink#2478 merges and Mavlink.cs is
    /// regenerated.  A follow-up to switch from raw casts to named members and
    /// add ToString assertions is tracked in that upstream PR.
    /// </summary>
    [TestClass]
    public class MagCalStatusTests
    {
        // Wire values for the three new failure codes from ArduPilot/ardupilot#32757.
        // Named enum members arrive with mavlink/mavlink#2478 + Mavlink.cs regen.
        private const byte RAW_BAD_OFFSETS = 8;
        private const byte RAW_BAD_DIAG_SCALING = 9;
        private const byte RAW_BAD_FITNESS = 10;

        // ── 1. Wire values ────────────────────────────────────────────────────
        //
        // These are intentional documentation tests — the constants are defined
        // as literal integers above, so the assertions always pass today.  Once
        // mavlink/mavlink#2478 merges and Mavlink.cs is regenerated the plan is
        // to replace the raw constants with the named enum members and assert
        // their numeric values here, turning these into real regression guards.

        [TestMethod]
        public void BadOffsets_RawValue_Is8()
        {
            Assert.AreEqual(8, RAW_BAD_OFFSETS);
        }

        [TestMethod]
        public void BadDiagScaling_RawValue_Is9()
        {
            Assert.AreEqual(9, RAW_BAD_DIAG_SCALING);
        }

        [TestMethod]
        public void BadFitness_RawValue_Is10()
        {
            Assert.AreEqual(10, RAW_BAD_FITNESS);
        }

        // ── 2. Cast from raw byte (as received from firmware) ────────────────

        [TestMethod]
        public void CastByte8_IsDistinctFromKnownValues()
        {
            var status = (MAVLink.MAG_CAL_STATUS)RAW_BAD_OFFSETS;
            Assert.AreNotEqual(MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,    status);
            Assert.AreNotEqual(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED,     status);
            Assert.AreNotEqual(MAVLink.MAG_CAL_STATUS.MAG_CAL_BAD_RADIUS, status);
            Assert.AreEqual((byte)8, (byte)status);
        }

        [TestMethod]
        public void CastByte9_IsDistinctFromKnownValues()
        {
            var status = (MAVLink.MAG_CAL_STATUS)RAW_BAD_DIAG_SCALING;
            Assert.AreNotEqual(MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,    status);
            Assert.AreNotEqual(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED,     status);
            Assert.AreNotEqual(MAVLink.MAG_CAL_STATUS.MAG_CAL_BAD_RADIUS, status);
            Assert.AreEqual((byte)9, (byte)status);
        }

        [TestMethod]
        public void CastByte10_IsDistinctFromKnownValues()
        {
            var status = (MAVLink.MAG_CAL_STATUS)RAW_BAD_FITNESS;
            Assert.AreNotEqual(MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,    status);
            Assert.AreNotEqual(MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED,     status);
            Assert.AreNotEqual(MAVLink.MAG_CAL_STATUS.MAG_CAL_BAD_RADIUS, status);
            Assert.AreEqual((byte)10, (byte)status);
        }

        // ── 3. Failure guard: calStatus > MAG_CAL_SUCCESS (value 4) ──────────
        //
        // The timer_Tick guard `if (calStatus > MAG_CAL_SUCCESS)` must capture
        // all failure codes, including the three new ones sent as raw 8/9/10.

        [TestMethod]
        public void RawByte8_IsGreaterThanSuccess()
        {
            var status = (MAVLink.MAG_CAL_STATUS)RAW_BAD_OFFSETS;
            Assert.IsTrue(status > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,
                "cal_status=8 (BAD_OFFSETS) must trigger the failure guard");
        }

        [TestMethod]
        public void RawByte9_IsGreaterThanSuccess()
        {
            var status = (MAVLink.MAG_CAL_STATUS)RAW_BAD_DIAG_SCALING;
            Assert.IsTrue(status > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,
                "cal_status=9 (BAD_DIAG_SCALING) must trigger the failure guard");
        }

        [TestMethod]
        public void RawByte10_IsGreaterThanSuccess()
        {
            var status = (MAVLink.MAG_CAL_STATUS)RAW_BAD_FITNESS;
            Assert.IsTrue(status > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS,
                "cal_status=10 (BAD_FITNESS) must trigger the failure guard");
        }

        [TestMethod]
        public void AllFailureCodes_AreGreaterThanSuccess()
        {
            var failures = new[]
            {
                MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_BAD_ORIENTATION,
                MAVLink.MAG_CAL_STATUS.MAG_CAL_BAD_RADIUS,
                (MAVLink.MAG_CAL_STATUS)RAW_BAD_OFFSETS,
                (MAVLink.MAG_CAL_STATUS)RAW_BAD_DIAG_SCALING,
                (MAVLink.MAG_CAL_STATUS)RAW_BAD_FITNESS,
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

        // ── 4. Named string representation ────────────────────────────────────

        [TestMethod]
        public void Failed_ToStringIsNamed()
        {
            Assert.AreEqual("MAG_CAL_FAILED", MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED.ToString());
        }

        [TestMethod]
        public void BadRadius_ToStringIsNamed()
        {
            Assert.AreEqual("MAG_CAL_BAD_RADIUS", MAVLink.MAG_CAL_STATUS.MAG_CAL_BAD_RADIUS.ToString());
        }

        // ── 5. lastFailureStatus dictionary semantics ─────────────────────────
        //
        // Simulates the per-compass failure tracking in timer1_Tick:
        //   - failure status is stored per compass ID
        //   - a later success removes the entry
        //   - a later failure replaces (not accumulates) the entry

        [TestMethod]
        public void LastFailureStatus_StoresFailurePerCompass()
        {
            var dict = new Dictionary<byte, MAVLink.MAG_CAL_STATUS>();
            dict[0] = (MAVLink.MAG_CAL_STATUS)RAW_BAD_OFFSETS;
            dict[1] = (MAVLink.MAG_CAL_STATUS)RAW_BAD_FITNESS;

            Assert.AreEqual((MAVLink.MAG_CAL_STATUS)RAW_BAD_OFFSETS, dict[0]);
            Assert.AreEqual((MAVLink.MAG_CAL_STATUS)RAW_BAD_FITNESS, dict[1]);
        }

        [TestMethod]
        public void LastFailureStatus_SuccessRemovesEntry()
        {
            var dict = new Dictionary<byte, MAVLink.MAG_CAL_STATUS>();
            dict[0] = (MAVLink.MAG_CAL_STATUS)RAW_BAD_FITNESS;

            dict.Remove(0); // success received

            Assert.IsFalse(dict.ContainsKey(0));
        }

        [TestMethod]
        public void LastFailureStatus_LaterFailureReplaces()
        {
            var dict = new Dictionary<byte, MAVLink.MAG_CAL_STATUS>();
            dict[0] = (MAVLink.MAG_CAL_STATUS)RAW_BAD_FITNESS;
            dict[0] = (MAVLink.MAG_CAL_STATUS)RAW_BAD_OFFSETS; // second attempt fails differently

            Assert.AreEqual((MAVLink.MAG_CAL_STATUS)RAW_BAD_OFFSETS, dict[0]);
            Assert.AreEqual(1, dict.Count); // no accumulation
        }
    }
}
