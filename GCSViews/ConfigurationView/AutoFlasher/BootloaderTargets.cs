using System.Collections.Generic;

namespace MissionPlanner.GCSViews.ConfigurationView.AutoFlasher
{
    public sealed class UsbId
    {
        public ushort Vid { get; }
        public ushort Pid { get; }
        public string Description { get; }

        public UsbId(ushort vid, ushort pid, string description)
        {
            Vid = vid;
            Pid = pid;
            Description = description;
        }

        public override string ToString() => $"VID_{Vid:X4}&PID_{Pid:X4} ({Description})";
    }

    public static class BootloaderTargets
    {
        public static readonly IReadOnlyList<UsbId> Known = new[]
        {
            new UsbId(0x26AC, 0x0011, "PX4 FMU v2 bootloader"),
            new UsbId(0x26AC, 0x0001, "PX4 IO bootloader"),
            new UsbId(0x26AC, 0x0010, "PX4 FMU"),
            new UsbId(0x26AC, 0x0016, "Pixhawk Mini bootloader"),
            new UsbId(0x26AC, 0x0017, "Pixracer bootloader"),
            new UsbId(0x2DAE, 0x1001, "Cube Black bootloader"),
            new UsbId(0x2DAE, 0x1011, "Cube Orange bootloader"),
            new UsbId(0x2DAE, 0x1016, "Cube Orange+ bootloader"),
            new UsbId(0x2DAE, 0x1101, "Cube+ bootloader"),
            new UsbId(0x1209, 0x5740, "ArduPilot generic bootloader"),
            new UsbId(0x1209, 0x5741, "ArduPilot generic"),
            new UsbId(0x0483, 0xDF11, "STM32 DFU"),
        };
    }
}
