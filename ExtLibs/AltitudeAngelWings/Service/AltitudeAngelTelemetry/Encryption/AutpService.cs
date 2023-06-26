using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;
using System;
using System.Runtime.Serialization;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public class AutpService : IAutpService
    {
        private readonly IEncryptionAlgorithm encryptionAlgorithm;

        public AutpService(IEncryptionAlgorithm encryptionAlgorithm)
        {
            this.encryptionAlgorithm = encryptionAlgorithm;
        }

        public byte[] WriteTelemetry(ITelemetryEvent telemetryEvent, byte[] encryptionKey = null)
        {
            var t = (TelemetryEvent)telemetryEvent;
            if (t == null)
            {
                throw new SerializationException("Unknown telemetry event type");
            }

            byte[] payload;
            switch (t.Message)
            {
                case UavPositionReport r:
                    payload = UavPositionReportSerializer.WriteReport(r);
                    break;
                default:
                    throw new SerializationException($"Unknown message type: {t.MessageType}");
            }

            var wrapper = new TelemetryWrapper(AutpMessageTypes.UavPosition, payload);
            byte[] wrapperBytes = TelemetryWrapperSerializer.WriteWrapper(wrapper);


            wrapperBytes = encryptionKey != null
                ? EncryptBytes(encryptionKey, t.SequenceNumber, wrapperBytes)
                : EncryptBytes(t.TelemetryId, t.SequenceNumber, wrapperBytes);

            var datagram = new UdpTelemetryDatagram(t.TelemetryId, t.SequenceNumber, wrapperBytes, true);
            return TelemetryDatagramSerializer.WriteDatagram(datagram);
        }

        private bool CheckCrc(uint expectedCrc, byte[] bytes) => Crc32.CalculateHash(bytes) == expectedCrc;

        private byte[] DecryptBytes(Guid telemetryId, int sequenceNumber, byte[] bytes) =>
            this.encryptionAlgorithm.DecryptBytes(telemetryId, sequenceNumber, bytes);

        private byte[] DecryptBytes(byte[] key, int sequenceNumber, byte[] bytes) =>
            this.encryptionAlgorithm.DecryptBytes(key, sequenceNumber, bytes);

        private byte[] EncryptBytes(Guid telemetryId, int sequenceNumber, byte[] bytes) =>
            this.encryptionAlgorithm.EncryptBytes(telemetryId, sequenceNumber, bytes);

        private byte[] EncryptBytes(byte[] key, int sequenceNumber, byte[] bytes) =>
            this.encryptionAlgorithm.EncryptBytes(key, sequenceNumber, bytes);
    }
}
