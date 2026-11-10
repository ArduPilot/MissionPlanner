using System.Collections.Generic;
using System.Linq;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    // Per-compass state of an onboard mag calibration, built from MAG_CAL_PROGRESS/REPORT packets.
    // Not thread safe; feed it from the UI thread only.
    public class MagCalTracker
    {
        private readonly Dictionary<byte, MAVLink.mavlink_mag_cal_report_t> _reports =
            new Dictionary<byte, MAVLink.mavlink_mag_cal_report_t>();
        private readonly Dictionary<byte, byte> _progress = new Dictionary<byte, byte>();
        private readonly Dictionary<byte, byte> _attempt = new Dictionary<byte, byte>();
        private byte _calMask;

        public void Clear()
        {
            _reports.Clear();
            _progress.Clear();
            _attempt.Clear();
            _calMask = 0;
        }

        public void Apply(MAVLink.mavlink_mag_cal_progress_t p)
        {
            _calMask |= p.cal_mask;
            _attempt[p.compass_id] = p.attempt;
            _progress[p.compass_id] = p.completion_pct;
        }

        // Returns true when firmware reports this compass's offsets were saved (autosaved == 1).
        public bool Apply(MAVLink.mavlink_mag_cal_report_t r)
        {
            // Ignore the all-zero NOT_STARTED report.
            if (r.ofs_x == 0 && r.ofs_y == 0 && r.ofs_z == 0
                && r.cal_status == (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_NOT_STARTED)
                return false;

            _calMask |= r.cal_mask;
            _reports[r.compass_id] = r;
            return r.autosaved == 1;
        }

        // Every compass heard from, plus those named in the calibration mask (below maxInstances).
        public SortedSet<byte> CompassIds(int maxInstances)
        {
            var ids = new SortedSet<byte>(_progress.Keys);
            foreach (var id in _reports.Keys)
                ids.Add(id);
            for (byte i = 0; i < maxInstances; i++)
                if (((_calMask >> i) & 1) != 0)
                    ids.Add(i);
            return ids;
        }

        // Last terminal report per compass, ordered by id.
        public IEnumerable<KeyValuePair<byte, MAVLink.mavlink_mag_cal_report_t>> Reports =>
            _reports.OrderBy(kv => kv.Key);

        // Progress to display: firmware reports at most 99% until SUCCESS, which pins it to 100.
        public int Percent(byte compassId) =>
            IsSucceeded(compassId) ? 100 : (_progress.TryGetValue(compassId, out var pct) ? pct : 0);

        // Firmware retry attempt from the last progress packet, 0 if none seen.
        public int Attempt(byte compassId) => _attempt.TryGetValue(compassId, out var a) ? a : 0;

        public bool IsSucceeded(byte compassId) =>
            _reports.TryGetValue(compassId, out var r)
            && (MAVLink.MAG_CAL_STATUS)r.cal_status == MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS;

        public bool IsFailed(byte compassId) =>
            _reports.TryGetValue(compassId, out var r)
            && (MAVLink.MAG_CAL_STATUS)r.cal_status > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS;

        public bool AnyFailed =>
            _reports.Values.Any(r => (MAVLink.MAG_CAL_STATUS)r.cal_status > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS);

        public int SucceededCount =>
            _reports.Values.Count(r => (MAVLink.MAG_CAL_STATUS)r.cal_status == MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS);

        // Compasses being calibrated: the bits set in cal_mask, or the reports seen if no mask arrived.
        public int ExpectedCount => _calMask == 0 ? _reports.Count : CountBits(_calMask);

        // Every expected compass has succeeded and none is in a failed state.
        public bool IsComplete => ExpectedCount > 0 && !AnyFailed && SucceededCount >= ExpectedCount;

        // Compasses that succeeded and were autosaved by firmware, ordered by id.
        public List<byte> AutosavedSuccessIds() =>
            _reports
                .Where(kv => kv.Value.autosaved == 1
                             && (MAVLink.MAG_CAL_STATUS)kv.Value.cal_status == MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                .Select(kv => kv.Key)
                .OrderBy(id => id)
                .ToList();

        private static int CountBits(byte mask)
        {
            int count = 0;
            while (mask != 0)
            {
                count += mask & 1;
                mask >>= 1;
            }

            return count;
        }
    }
}
