using System;
using System.Collections.Generic;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    // User-facing text for MAG_CAL_REPORT results; no UI or vehicle dependencies.
    public static class MagCalText
    {
        // Readable text for statuses whose upstream [Description] is empty (SUCCESS, FAILED).
        private static readonly Dictionary<MAVLink.MAG_CAL_STATUS, string> Fallback =
            new Dictionary<MAVLink.MAG_CAL_STATUS, string>
            {
                { MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, "Success" },
                { MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED,  "Calibration failed" },
            };

        // Uses mavgen's MAVLink.Description attribute, not System.ComponentModel's.
        public static string Status(MAVLink.MAG_CAL_STATUS status)
        {
            var field = typeof(MAVLink.MAG_CAL_STATUS).GetField(status.ToString());
            var attr = field == null ? null
                : (MAVLink.Description)Attribute.GetCustomAttribute(field, typeof(MAVLink.Description));

            if (!string.IsNullOrWhiteSpace(attr?.Text))
                return attr.Text;

            return Fallback.TryGetValue(status, out var fallback) ? fallback : status.ToString();
        }

        // Values that help fix the specific failure; empty for other statuses.
        // offsetLimit is COMPASS_OFFS_MAX when the param is loaded, otherwise null.
        public static string Detail(MAVLink.mavlink_mag_cal_report_t r, float? offsetLimit)
        {
            switch ((MAVLink.MAG_CAL_STATUS)r.cal_status)
            {
                case MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS:
                    return $"  [x:{r.ofs_x:F1} y:{r.ofs_y:F1} z:{r.ofs_z:F1}  fit:{r.fitness:F1}]";

                case MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS:
                {
                    var offsets = $"x:{r.ofs_x:F1} y:{r.ofs_y:F1} z:{r.ofs_z:F1}";
                    return offsetLimit.HasValue
                        ? $"  [{offsets}  limit:\u00b1{offsetLimit.Value:F0}]"
                        : $"  [{offsets}]";
                }

                case MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_RESIDUALS_HIGH:
                    return $"  [fit:{r.fitness:F1}]";

                case MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_ORIENTATION:
                    return $"  [orientation confidence:{r.orientation_confidence:F2}]";

                default:
                    return string.Empty;
            }
        }
    }
}
