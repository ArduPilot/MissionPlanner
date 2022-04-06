using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public class UdpTelemetryDatagram
    {
        public UdpTelemetryDatagram()
        {
        }

        public UdpTelemetryDatagram(Guid telemetryId, int sequenceNumber, byte[] payload, bool encrypted)
        {
            if (payload.Length > short.MaxValue)
            {
                throw new ArgumentException(payload.Length.ToString());
            }

            this.Payload = payload;
            this.Header = new DatagramHeader
            {
                Version = 1,
                TelemetryId = telemetryId,
                Flags = encrypted ? DatagramFlags.Encrypted : DatagramFlags.None,
                SequenceNumber = sequenceNumber,
                PayloadLength = (short)payload.Length
            };
        }

        public DatagramHeader Header
        {
            get;
            set;
        }

        /// <summary>
        /// Payload is encrypted
        /// </summary>
        public bool IsEncrypted => (this.Header.Flags & DatagramFlags.Encrypted) == DatagramFlags.Encrypted;

        public byte[] Payload
        {
            get;
            set;
        }
    }
}
