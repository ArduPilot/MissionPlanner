using System;
using System.Security.Cryptography;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public class Hkdf
    {
        private readonly HashType hashType;

        public Hkdf(HashType hashType)
        {
            this.hashType = hashType;
        }

        private HMAC GetHmac(byte[] key)
        {
            switch (this.hashType)
            {
                case HashType.Hmac256: return new HMACSHA256(key);
                case HashType.Hmac384: return new HMACSHA384(key);
                case HashType.Hmac512: return new HMACSHA512(key);
                default: throw new ArgumentException(nameof(this.hashType));
            }
        }

        /// <summary>
        /// Derive a key given a variable salt and key
        /// </summary>
        /// <param name="salt">Variable salt input</param>
        /// <param name="secret">Key bytes</param>
        /// <param name="outputLength">Desired output length in bytes</param>
        /// <returns>A key of the required length as a byte array</returns>
        public byte[] DeriveKey(byte[] salt, byte[] secret, int outputLength)
        {
            using (HMAC hmac = GetHmac(salt))
            {
                byte[] privateKey = hmac.ComputeHash(secret);
                byte[] result = Expand(hmac, privateKey, outputLength);
                return result;
            }

        }

        /// <summary>
        /// Expand a key to the required byte length
        /// </summary>
        /// <remarks>
        ///     Chains hashes of the key together until the correct length is reached.
        ///     e.g.
        ///     [1][hash(key,1)][2][hash(key,2)]...[n][hash(key,n)]
        ///     Final block may be truncated if length has been reached.
        /// </remarks>
        /// <param name="hmac">Hmac algorithm</param>
        /// <param name="key">Key</param>
        /// <param name="outputLength">Desired length</param>
        /// <returns>Expanded key</returns>
        private byte[] Expand(HMAC hmac, byte[] key, int outputLength)
        {
            byte[] resultBlock = new byte[0];
            byte[] result = new byte[outputLength];
            int bytesRemaining = outputLength;

            for (int i = 1; bytesRemaining > 0; i++)
            {
                byte[] currentInfo = new byte[resultBlock.Length + 1];
                Array.Copy(resultBlock, 0, currentInfo, 0, resultBlock.Length);
                currentInfo[currentInfo.Length - 1] = (byte)i;

                hmac.Key = key;
                resultBlock = hmac.ComputeHash(currentInfo);

                Array.Copy(resultBlock, 0, result, outputLength - bytesRemaining,
                    Math.Min(resultBlock.Length, bytesRemaining));

                bytesRemaining -= resultBlock.Length;
            }

            return result;
        }

    }
}
