using System;

namespace MissionPlanner.Utilities
{
    internal class AP_HAL
    {
        readonly static DateTime unixstart = new DateTime(1970, 1, 1);

        internal static uint millis()
        {
            return (uint)(DateTime.UtcNow - unixstart).TotalMilliseconds;
        }
    }
}