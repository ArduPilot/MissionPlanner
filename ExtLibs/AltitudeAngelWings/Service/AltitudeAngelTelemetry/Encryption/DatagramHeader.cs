using System;
using System.Runtime.InteropServices;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DatagramHeader
    {
        /// <summary>
        /// Datagram version
        /// </summary>
        public short Version
        {
            get; set;
        }

        /// <summary>
        /// Telemetry source id
        /// </summary>
        public Guid TelemetryId
        {
            get; set;
        }

        /// <summary>
        /// Sequence number to be incremented by 1 for each datagram sent
        /// </summary>
        public int SequenceNumber
        {
            get; set;
        }

        /// <summary>
        /// Length of payload in bytes
        /// </summary>
        public short PayloadLength
        {
            get; set;
        }

        /// <summary>
        /// Payload flags. <see cref="DatagramFlags"/>
        /// </summary>
        public sbyte Flags
        {
            get; set;
        }
    }
}
