using System;
using System.Collections.Generic;

namespace MissionPlanner.Utilities
{
    internal class MainV2
    {
        internal static MAVLinkInterface comPort;
        internal static MainV2 instance;

        static MainV2()
        {
            instance = new MainV2();
        }

        public MainV2()
        {
            instance = this;
            comPort = new MAVLinkInterface();
            comPort.BaseStream = new Comms.UdpSerial();
        }

        public static IEnumerable<MAVLinkInterface> Comports { get; internal set; }

        internal void Invoke(Action methodInvoker)
        {
            throw new NotImplementedException();
        }
    }
}