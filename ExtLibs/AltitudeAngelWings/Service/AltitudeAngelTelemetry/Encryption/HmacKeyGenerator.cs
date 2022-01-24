using System;
using System.Text;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public class HmacKeyGenerator : IEncryptionKeyGenerator
    {
        private readonly Hkdf hkdf;
        private readonly byte[] keyBytes;


        public HmacKeyGenerator(HashType hashType, string encryptionKeySecret)
        {
            this.hkdf = new Hkdf(hashType);

            this.keyBytes = Encoding.UTF8.GetBytes(encryptionKeySecret);
        }

        /// <inheritdoc />
        public byte[] GenerateKey(Guid telemetryId, byte keySize)
        {
            byte[] salt = telemetryId.ToByteArray();
            return this.hkdf.DeriveKey(salt, this.keyBytes, keySize);
        }
    }
}
