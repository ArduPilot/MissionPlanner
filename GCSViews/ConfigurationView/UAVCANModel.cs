using System;
using UAVCAN;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public class UAVCANModel
    {
        public byte ID { get; set; }
        public string Name { get; set; } = "?";
        public string Mode { get; set; }
        public string Health { get; set; }
        public TimeSpan Uptime { get; set; }
        public string HardwareVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public ulong SoftwareCRC { get; set; }
        public uavcan.uavcan_protocol_GetNodeInfo_res RawMsg { get; set; }
        public string HardwareUID { get; set; }
        public ushort VSC { get; set; }
    }
}