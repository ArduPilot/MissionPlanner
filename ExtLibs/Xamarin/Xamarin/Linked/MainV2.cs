using System;
using System.Collections.Generic;

namespace MissionPlanner.Utilities
{
    internal class MainV2
    {
        internal static MAVLinkInterface comPort;
        internal static MainV2 instance;

        public static IEnumerable<MAVLinkInterface> Comports { get; internal set; }

        internal void Invoke(Action methodInvoker)
        {
            throw new NotImplementedException();
        }
    }
}