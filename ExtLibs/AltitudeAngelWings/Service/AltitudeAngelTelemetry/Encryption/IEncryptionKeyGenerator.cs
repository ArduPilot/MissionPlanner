using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public interface IEncryptionKeyGenerator
    {
        /// <summary>
        /// Generate an encryption key from a telemetry id
        /// The key size must be valid for the encryption algorithm being used
        /// </summary>
        /// <param name="telemetryId">Telemetry id</param>
        /// <param name="keySize">Size of the key to generate in bytes.</param>
        /// <returns>A key of the required size</returns>
        byte[] GenerateKey(Guid telemetryId, byte keySize);
    }
}
