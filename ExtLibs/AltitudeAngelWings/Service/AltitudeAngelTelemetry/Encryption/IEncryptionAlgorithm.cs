using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public interface IEncryptionAlgorithm
    {
        byte[] EncryptBytes(Guid telemetryId, int sequenceNumber, byte[] bytes);
        byte[] EncryptBytes(byte[] key, int sequenceNumber, byte[] bytes);
        byte[] DecryptBytes(Guid telemetryId, int sequenceNumber, byte[] bytes);
        byte[] DecryptBytes(byte[] key, int sequenceNumber, byte[] bytes);
    }
}
