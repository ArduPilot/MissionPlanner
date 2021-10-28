using System;
using DroneCAN;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public class DroneCANModel
    {
        public byte ID { get; set; }
        public string Name { get; set; } = "?";
        public string Mode { get; set; }
        public string Health { get; set; }
        public TimeSpan Uptime { get; set; }
        public string HardwareVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public ulong SoftwareCRC { get; set; }
        public DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_res RawMsg { get; set; }
        public string HardwareUID { get; set; }
        public ushort VSC { get; set; }
    }
}