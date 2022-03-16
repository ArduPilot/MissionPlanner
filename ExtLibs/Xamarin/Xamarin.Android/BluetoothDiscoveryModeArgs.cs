using System;

namespace MissionPlanner.Comms
{
    public class BluetoothDiscoveryModeArgs: EventArgs 
    {
        public BluetoothDiscoveryModeArgs(bool inDiscoveryMode)
        {
            InDiscoveryMode = inDiscoveryMode;
        }
        public bool InDiscoveryMode { get; private set; }
    }
}