using System.Runtime.InteropServices;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AutpStatusMessage
    {
        public ushort EquipmentId { get; set; }
        public byte Index { get; set; }
        public byte Status { get; set; }
    }
}
