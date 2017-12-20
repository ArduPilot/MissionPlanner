using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace MissionPlanner.Utilities
{
    public sealed class Crypto : IDisposable
    {
        private static readonly byte[] Key =
        {
            0xd1, 0x3c, 0x35, 0x6f, 0xb5, 0xd, 0x87, 0xf0,
            0x92, 0x07, 0x6d, 0xab, 0x76, 0x82, 0x36, 0xa,
            0x13, 0x5a, 0x77, 0xfe, 0x77, 0xf3, 0x7f, 0xa8,
            0xa4, 0x04, 0x11, 0x46, 0x68, 0x2d, 0x48, 0xa1
        };

        private static readonly byte[] IV =
        {
            0x6d, 0x2d, 0xf5, 0x34, 0xc7, 0x60, 0xc5, 0x33,
            0xe2, 0xa3, 0xd7, 0xc3, 0xf3, 0x39, 0xf2, 0x16
        };

        /// <summary>
        /// Abstract object
        /// </summary>
        public SymmetricAlgorithm algorithm;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Crypto()
        {
            try
            {
                var macAddr = (
                    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    // where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress()
                    ).FirstOrDefault();

                var bytes = macAddr.GetAddressBytes();

                Array.Copy(bytes, IV, bytes.Length);

                Array.Copy(bytes, Key, bytes.Length);
            }
            catch
            {
            }


            this.algorithm = new RijndaelManaged();
            this.algorithm.Mode = CipherMode.CBC;
            this.algorithm.Key = Key;
            this.algorithm.IV = IV;
        }

        /// <summary>
        /// Release all resources used by the SymmetricAlgorithm class
        /// </summary>
        public void Dispose()
        {
            this.algorithm.Clear();
        }

        /// <summary>
        /// Set Binary Keys
        /// </summary>
        public void SetBinaryKeys(byte[] Key, byte[] IV)
        {
            this.algorithm.Key = Key;
            this.algorithm.IV = IV;
        }

        /// <summary>
        /// Extract Binary Keys
        /// </summary>
        public void ExtractBinaryKeys(out byte[] Key, out byte[] IV)
        {
            Key = this.algorithm.Key;
            IV = this.algorithm.IV;
        }

        /// <summary>
        /// Process the data with CryptoStream
        /// </summary>
        byte[] Process(byte[] data, int startIndex, int count, ICryptoTransform cryptor)
        {
            //
            // the memory stream granularity must match the block size
            // of the current cryptographic operation
            //
            int capacity = count;
            int mod = count%algorithm.BlockSize;
            if (mod > 0)
            {
                capacity += (algorithm.BlockSize - mod);
            }

            MemoryStream memoryStream = new MemoryStream(capacity);

            CryptoStream cryptoStream = new CryptoStream(
                memoryStream,
                cryptor,
                CryptoStreamMode.Write);

            cryptoStream.Write(data, startIndex, count);
            cryptoStream.FlushFinalBlock();

            cryptoStream.Close();
            cryptoStream = null;

            cryptor.Dispose();
            cryptor = null;

            return memoryStream.ToArray();
        }

        /// <summary>
        ///  Byte array encryption function
        /// </summary>
        /// <param name="cleanBuffer">input byte array</param>
        /// <returns>output encrypted byte array</returns>
        public byte[] EncryptBuffer(byte[] cleanBuffer)
        {
            byte[] output;

            // Encryptor object
            ICryptoTransform cryptoTransform = this.algorithm.CreateEncryptor();

            // Get the result
            output = this.Process(cleanBuffer, 0, cleanBuffer.Length, cryptoTransform);

            //clean
            cryptoTransform.Dispose();

            return output;
        }

        /// <summary>
        ///  Byte array decryption function
        /// </summary>
        /// <param name="cryptoBuffer">input chiper byte array</param>
        /// <returns>output decrypted byte array</returns>
        public byte[] DecryptBuffer(byte[] cryptoBuffer)
        {
            byte[] output;

            // Decryptor object
            ICryptoTransform cryptoTransform = this.algorithm.CreateDecryptor();

            // Get the result   
            output = this.Process(cryptoBuffer, 0, cryptoBuffer.Length, cryptoTransform);

            //clean
            cryptoTransform.Dispose();

            return output;
        }

        /// <summary>
        /// String encryption function
        /// </summary>
        /// <param name="plainText">clean text</param>
        /// <returns>base64 encrypted string</returns>
        public string EncryptString(string plainText)
        {
            return Convert.ToBase64String(EncryptBuffer(Encoding.UTF8.GetBytes(plainText)));
        }

        /// <summary>
        /// String decryption function
        /// </summary>
        /// <param name="encyptedText">base64 encrypted string</param>
        /// <returns>decrypted text</returns>
        public string DecryptString(string encyptedText)
        {
            return Encoding.UTF8.GetString(DecryptBuffer(Convert.FromBase64String(encyptedText)));
        }
    }
}