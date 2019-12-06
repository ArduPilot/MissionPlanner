using System;
using System.Collections.Generic;
using System.Text;

namespace MissionPlanner.ArduPilot
{
    public class AP_InternalError
    {
        public enum error_t: uint
        {
            logger_mapfailure = (1U << 0),
            logger_missing_logstructure = (1U << 1),
            logger_logwrite_missingfmt = (1U << 2),
            logger_too_many_deletions = (1U << 3),
            logger_bad_getfilename = (1U << 4),
            unused1 = (1U << 5), // was logger_stopping_without_sem
            logger_flushing_without_sem = (1U << 6),
            logger_bad_current_block = (1U << 7),
            logger_blockcount_mismatch = (1U << 8),
            logger_dequeue_failure = (1U << 9),
            constraining_nan = (1U << 10),
            watchdog_reset = (1U << 11),
            iomcu_reset = (1U << 12),
            iomcu_fail = (1U << 13),
            spi_fail = (1U << 14),
            main_loop_stuck = (1U << 15),
            gcs_bad_missionprotocol_link = (1U << 16),
            bitmask_range = (1U << 17),
            gcs_offset = (1U << 18),
            i2c_isr = (1U << 19),
            flow_of_control = (1U << 20), // for generic we-should-never-get-here situations
        }
    }
}
