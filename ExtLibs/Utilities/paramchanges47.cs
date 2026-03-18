using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class paramchanges47
    {
        // 4.7 parameter changes - keyed by new param name, value is "OLD → NEW: description"
        private static readonly Dictionary<string, string> _byNewParam = new Dictionary<string, string>
        {
            // ARMING
            { "ARMING_SKIPCHK", "ARMING_CHECK → ARMING_SKIPCHK: select which checks to disable" },
            // ATC (Copter)
            { "ATC_ANGLE_MAX", "ANGLE_MAX → ATC_ANGLE_MAX: units changed from cd to deg" },
            { "ATC_ACC_R_MAX", "ATC_ACCEL_R_MAX → ATC_ACC_R_MAX: units changed from cm/s/s to m/s/s" },
            { "ATC_ACC_P_MAX", "ATC_ACCEL_P_MAX → ATC_ACC_P_MAX: units changed from cm/s/s to m/s/s" },
            { "ATC_ACC_Y_MAX", "ATC_ACCEL_Y_MAX → ATC_ACC_Y_MAX: units changed from cm/s/s to m/s/s" },
            { "ATC_RATE_WPY_MAX", "ATC_SLEW_YAW → ATC_RATE_WPY_MAX: units changed from cdeg/s to deg/s" },
            // CIRCLE (Copter)
            { "CIRCLE_RADIUS_M", "CIRCLE_RADIUS → CIRCLE_RADIUS_M: units changed from cm to m" },
            // LAND (Copter)
            { "LAND_SPD_MS", "LAND_SPEED → LAND_SPD_MS: units changed from cm/s to m/s" },
            { "LAND_SPD_HIGH_MS", "LAND_SPEED_HIGH → LAND_SPD_HIGH_MS: units changed from cm/s to m/s" },
            { "LAND_ALT_LOW_M", "LAND_ALT_LOW → LAND_ALT_LOW_M: units changed from cm to m" },
            // LOIT (Copter)
            { "LOIT_SPEED_MS", "LOIT_SPEED → LOIT_SPEED_MS: units changed from cm/s to m/s" },
            { "LOIT_ACC_MAX_M", "LOIT_ACC_MAX → LOIT_ACC_MAX_M: units changed from cm/s/s to m/s/s" },
            { "LOIT_BRK_ACC_M", "LOIT_BRK_ACCEL → LOIT_BRK_ACC_M: units changed from cm/s/s to m/s/s" },
            { "LOIT_BRK_JRK_M", "LOIT_BRK_JERK → LOIT_BRK_JRK_M: units changed from cm/s/s/s to m/s/s/s" },
            // PILOT (Copter)
            { "PILOT_ACC_Z", "PILOT_ACCEL_Z → PILOT_ACC_Z: units changed from cm/s/s to m/s/s" },
            { "PILOT_SPD_UP", "PILOT_SPEED_UP → PILOT_SPD_UP: units changed from cm/s to m/s" },
            { "PILOT_SPD_DN", "PILOT_SPEED_DN → PILOT_SPD_DN: units changed from cm/s to m/s" },
            { "PILOT_TKO_ALT_M", "PILOT_TKOFF_ALT → PILOT_TKO_ALT_M: units changed from cm to m" },
            // PHLD (Copter)
            { "PHLD_BRK_ANGLE", "PHLD_BRAKE_ANGLE → PHLD_BRK_ANGLE: units changed from cd to deg" },
            { "PHLD_BRK_RATE", "PHLD_BRAKE_RATE → PHLD_BRK_RATE: no scaling change, name only" },
            // PSC_VELXY → PSC_NE_VEL (Copter)
            { "PSC_NE_VEL_P", "PSC_VELXY_P → PSC_NE_VEL_P: no scaling change, name only" },
            { "PSC_NE_VEL_I", "PSC_VELXY_I → PSC_NE_VEL_I: no scaling change, name only" },
            { "PSC_NE_VEL_D", "PSC_VELXY_D → PSC_NE_VEL_D: no scaling change, name only" },
            { "PSC_NE_VEL_IMAX", "PSC_VELXY_IMAX → PSC_NE_VEL_IMAX: value is now 100x smaller" },
            { "PSC_NE_VEL_FILT", "PSC_VELXY_FILT → PSC_NE_VEL_FILT: no scaling change, name only" },
            { "PSC_NE_VEL_FF", "PSC_VELXY_FF → PSC_NE_VEL_FF: no scaling change, name only" },
            { "PSC_NE_VEL_FLTE", "PSC_VELXY_FLTE → PSC_NE_VEL_FLTE: no scaling change, name only" },
            { "PSC_NE_VEL_FLTD", "PSC_VELXY_FLTD → PSC_NE_VEL_FLTD: no scaling change, name only" },
            // PSC_VELZ → PSC_D_VEL (Copter)
            { "PSC_D_VEL_P", "PSC_VELZ_P → PSC_D_VEL_P: no scaling change, name only" },
            { "PSC_D_VEL_I", "PSC_VELZ_I → PSC_D_VEL_I: no scaling change, name only" },
            { "PSC_D_VEL_D", "PSC_VELZ_D → PSC_D_VEL_D: no scaling change, name only" },
            { "PSC_D_VEL_IMAX", "PSC_VELZ_IMAX → PSC_D_VEL_IMAX: value is now 100x smaller" },
            { "PSC_D_VEL_FILT", "PSC_VELZ_FILT → PSC_D_VEL_FILT: no scaling change, name only" },
            { "PSC_D_VEL_FF", "PSC_VELZ_FF → PSC_D_VEL_FF: no scaling change, name only" },
            { "PSC_D_VEL_FLTE", "PSC_VELZ_FLTE → PSC_D_VEL_FLTE: no scaling change, name only" },
            { "PSC_D_VEL_FLTD", "PSC_VELZ_FLTD → PSC_D_VEL_FLTD: no scaling change, name only" },
            // PSC_ACCZ → PSC_D_ACC (Copter)
            { "PSC_D_ACC_P", "PSC_ACCZ_P → PSC_D_ACC_P: value is now 10x smaller" },
            { "PSC_D_ACC_I", "PSC_ACCZ_I → PSC_D_ACC_I: value is now 10x smaller" },
            { "PSC_D_ACC_D", "PSC_ACCZ_D → PSC_D_ACC_D: value is now 10x smaller" },
            { "PSC_D_ACC_IMAX", "PSC_ACCZ_IMAX → PSC_D_ACC_IMAX: no scaling change, name only" },
            { "PSC_D_ACC_FILT", "PSC_ACCZ_FILT → PSC_D_ACC_FILT: no scaling change, name only" },
            { "PSC_D_ACC_FF", "PSC_ACCZ_FF → PSC_D_ACC_FF: no scaling change, name only" },
            { "PSC_D_ACC_FLTE", "PSC_ACCZ_FLTE → PSC_D_ACC_FLTE: no scaling change, name only" },
            { "PSC_D_ACC_FLTD", "PSC_ACCZ_FLTD → PSC_D_ACC_FLTD: no scaling change, name only" },
            // RTL (Copter)
            { "RTL_ALT_M", "RTL_ALT → RTL_ALT_M: units changed from cm to m" },
            { "RTL_SPEED_MS", "RTL_SPEED → RTL_SPEED_MS: units changed from cm/s to m/s" },
            { "RTL_ALT_FINAL_M", "RTL_ALT_FINAL → RTL_ALT_FINAL_M: units changed from cm to m" },
            { "RTL_CLIMB_MIN_M", "RTL_CLIMB_MIN → RTL_CLIMB_MIN_M: units changed from cm to m" },
            // WPNAV (Copter)
            { "WP_ACC", "WPNAV_ACCEL → WP_ACC: units changed from cm/s/s to m/s/s" },
            { "WP_ACC_CNR", "WPNAV_ACCEL_C → WP_ACC_CNR: units changed from cm/s/s to m/s/s" },
            { "WP_ACC_Z", "WPNAV_ACCEL_Z → WP_ACC_Z: units changed from cm/s/s to m/s/s" },
            { "WP_RADIUS_M", "WPNAV_RADIUS → WP_RADIUS_M: units changed from cm to m" },
            { "WP_SPD", "WPNAV_SPEED → WP_SPD: units changed from cm/s to m/s" },
            { "WP_SPD_DN", "WPNAV_SPEED_DN → WP_SPD_DN: units changed from cm/s to m/s" },
            { "WP_SPD_UP", "WPNAV_SPEED_UP → WP_SPD_UP: units changed from cm/s to m/s" },
            // Q_A (Plane QuadPlane)
            { "Q_A_ANGLE_MAX", "Q_ANGLE_MAX → Q_A_ANGLE_MAX: units changed from cd to deg" },
            { "Q_A_ACC_R_MAX", "Q_A_ACCEL_R_MAX → Q_A_ACC_R_MAX: units changed from cm/s/s to m/s/s" },
            { "Q_A_ACC_P_MAX", "Q_A_ACCEL_P_MAX → Q_A_ACC_P_MAX: units changed from cm/s/s to m/s/s" },
            { "Q_A_ACC_Y_MAX", "Q_A_ACCEL_Y_MAX → Q_A_ACC_Y_MAX: units changed from cm/s/s to m/s/s" },
            { "Q_A_RATE_WPY_MAX", "Q_A_SLEW_YAW → Q_A_RATE_WPY_MAX: units changed from cdeg/s to deg/s" },
            // Q_LOIT (Plane QuadPlane)
            { "Q_LOIT_SPEED_MS", "Q_LOIT_SPEED → Q_LOIT_SPEED_MS: units changed from cm/s to m/s" },
            { "Q_LOIT_ACC_MAX_M", "Q_LOIT_ACC_MAX → Q_LOIT_ACC_MAX_M: units changed from cm/s/s to m/s/s" },
            { "Q_LOIT_BRK_ACC_M", "Q_LOIT_BRK_ACCEL → Q_LOIT_BRK_ACC_M: units changed from cm/s/s to m/s/s" },
            { "Q_LOIT_BRK_JRK_M", "Q_LOIT_BRK_JERK → Q_LOIT_BRK_JRK_M: units changed from cm/s/s/s to m/s/s/s" },
            // Q_PILOT (Plane QuadPlane)
            { "Q_PILOT_ACCEL_Z", "Q_PILOT_ACCEL_Z → Q_PILOT_ACCEL_Z: units changed from cm/s/s to m/s/s" },
            { "Q_PILOT_SPD_UP", "Q_PILOT_SPEED_UP → Q_PILOT_SPD_UP: units changed from cm/s to m/s" },
            { "Q_PILOT_SPD_DN", "Q_PILOT_SPEED_DN → Q_PILOT_SPD_DN: units changed from cm/s to m/s" },
            { "Q_PILOT_TKO_ALT_M", "Q_PILOT_TKOFF_ALT → Q_PILOT_TKO_ALT_M: units changed from cm to m" },
            // Q_P_VELXY → Q_P_NE_VEL (Plane QuadPlane)
            { "Q_P_NE_VEL_P", "Q_P_VELXY_P → Q_P_NE_VEL_P: no scaling change, name only" },
            { "Q_P_NE_VEL_I", "Q_P_VELXY_I → Q_P_NE_VEL_I: no scaling change, name only" },
            { "Q_P_NE_VEL_D", "Q_P_VELXY_D → Q_P_NE_VEL_D: no scaling change, name only" },
            { "Q_P_NE_VEL_IMAX", "Q_P_VELXY_IMAX → Q_P_NE_VEL_IMAX: value is now 100x smaller" },
            { "Q_P_NE_VEL_FILT", "Q_P_VELXY_FILT → Q_P_NE_VEL_FILT: no scaling change, name only" },
            { "Q_P_NE_VEL_FF", "Q_P_VELXY_FF → Q_P_NE_VEL_FF: no scaling change, name only" },
            { "Q_P_NE_VEL_FLTE", "Q_P_VELXY_FLTE → Q_P_NE_VEL_FLTE: no scaling change, name only" },
            { "Q_P_NE_VEL_FLTD", "Q_P_VELXY_FLTD → Q_P_NE_VEL_FLTD: no scaling change, name only" },
            // Q_P_VELZ → Q_P_D_VEL (Plane QuadPlane)
            { "Q_P_D_VEL_P", "Q_P_VELZ_P → Q_P_D_VEL_P: no scaling change, name only" },
            { "Q_P_D_VEL_I", "Q_P_VELZ_I → Q_P_D_VEL_I: no scaling change, name only" },
            { "Q_P_D_VEL_D", "Q_P_VELZ_D → Q_P_D_VEL_D: no scaling change, name only" },
            { "Q_P_D_VEL_IMAX", "Q_P_VELZ_IMAX → Q_P_D_VEL_IMAX: value is now 100x smaller" },
            { "Q_P_D_VEL_FILT", "Q_P_VELZ_FILT → Q_P_D_VEL_FILT: no scaling change, name only" },
            { "Q_P_D_VEL_FF", "Q_P_VELZ_FF → Q_P_D_VEL_FF: no scaling change, name only" },
            { "Q_P_D_VEL_FLTE", "Q_P_VELZ_FLTE → Q_P_D_VEL_FLTE: no scaling change, name only" },
            { "Q_P_D_VEL_FLTD", "Q_P_VELZ_FLTD → Q_P_D_VEL_FLTD: no scaling change, name only" },
            // Q_P_ACCZ → Q_P_D_ACC (Plane QuadPlane)
            { "Q_P_D_ACC_P", "Q_P_ACCZ_P → Q_P_D_ACC_P: value is now 10x smaller" },
            { "Q_P_D_ACC_I", "Q_P_ACCZ_I → Q_P_D_ACC_I: value is now 10x smaller" },
            { "Q_P_D_ACC_D", "Q_P_ACCZ_D → Q_P_D_ACC_D: value is now 10x smaller" },
            { "Q_P_D_ACC_IMAX", "Q_P_ACCZ_IMAX → Q_P_D_ACC_IMAX: no scaling change, name only" },
            { "Q_P_D_ACC_FILT", "Q_P_ACCZ_FILT → Q_P_D_ACC_FILT: no scaling change, name only" },
            { "Q_P_D_ACC_FF", "Q_P_ACCZ_FF → Q_P_D_ACC_FF: no scaling change, name only" },
            { "Q_P_D_ACC_FLTE", "Q_P_ACCZ_FLTE → Q_P_D_ACC_FLTE: no scaling change, name only" },
            { "Q_P_D_ACC_FLTD", "Q_P_ACCZ_FLTD → Q_P_D_ACC_FLTD: no scaling change, name only" },
            // Q_WP (Plane QuadPlane)
            { "Q_WP_ACC", "Q_WP_ACCEL → Q_WP_ACC: units changed from cm/s/s to m/s/s" },
            { "Q_WP_ACC_CNR", "Q_WP_ACCEL_C → Q_WP_ACC_CNR: units changed from cm/s/s to m/s/s" },
            { "Q_WP_ACC_Z", "Q_WP_ACCEL_Z → Q_WP_ACC_Z: units changed from cm/s/s to m/s/s" },
            { "Q_WP_RADIUS_M", "Q_WP_RADIUS → Q_WP_RADIUS_M: units changed from cm to m" },
            { "Q_WP_SPD", "Q_WP_SPEED → Q_WP_SPD: units changed from cm/s to m/s" },
            { "Q_WP_SPD_DN", "Q_WP_SPEED_DN → Q_WP_SPD_DN: units changed from cm/s to m/s" },
            { "Q_WP_SPD_UP", "Q_WP_SPEED_UP → Q_WP_SPD_UP: units changed from cm/s to m/s" },
        };

        // reverse index: old param name → new param name
        private static readonly Dictionary<string, string> _oldToNew = _byNewParam
            .ToDictionary(
                kv => kv.Value.Substring(0, kv.Value.IndexOf(" →")),
                kv => kv.Key);

        /// <summary>
        /// Given an old param name, returns the new 4.7 name, or null if not in the change list.
        /// </summary>
        public static string changedByOldParam(string oldName)
        {
            return _oldToNew.TryGetValue(oldName, out var newName) ? newName : null;
        }

        /// <summary>
        /// Given a new 4.7 param name, returns the old param name, or null if not in the change list.
        /// </summary>
        public static string changedByNewParam(string newName)
        {
            if (_byNewParam.TryGetValue(newName, out var description))
                return description.Substring(0, description.IndexOf(" →"));
            return null;
        }

        /// <summary>
        /// Given a new 4.7 param name, returns the full warning text, or null if not in the change list.
        /// </summary>
        public static string changedByNewParamWarning(string newName)
        {
            return _byNewParam.TryGetValue(newName, out var warning) ? warning : null;
        }
    }
}
