using System.Runtime.InteropServices;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TelemetryHeader
    {
        public TelemetryHeader(short messageType, short messageSize, uint crc)
        {
            this.MessageType = messageType;
            this.MessageSize = messageSize;
            this.Crc = crc;
        }

        /// <summary>
        /// Type of message <see cref="AutpMessageTypes"/>
        /// </summary>
        public short MessageType { get; }

        /// <summary>
        /// Size of message in bytes
        /// </summary>
        public short MessageSize { get; }

        /// <summary>
        /// Expected CRC value of message
        /// </summary>
        public uint Crc { get; }
    }
}
