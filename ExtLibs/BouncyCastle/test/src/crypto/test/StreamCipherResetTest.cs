using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * Test whether block ciphers implement reset contract on init, encrypt/decrypt and reset.
     */
    [TestFixture]
    public class StreamCipherResetTest
        :   SimpleTest
    {
        public override string Name
        {
            get { return "Stream Cipher Reset"; }
        }

        public override void PerformTest()
        {
            TestReset(typeof(Salsa20Engine), 32, 8);
            TestReset(typeof(Salsa20Engine), 16, 8);
            TestReset(typeof(XSalsa20Engine), 32, 24);
            TestReset(typeof(ChaChaEngine), 32, 8);
            TestReset(typeof(ChaChaEngine), 16, 8);
            TestReset(typeof(RC4Engine), 16);
            TestReset(typeof(IsaacEngine), 16);
            TestReset(typeof(HC128Engine), 16, 16);
            TestReset(typeof(HC256Engine), 16, 16);
            //TestReset(typeof(Grainv1Engine), 16, 8);
            //TestReset(typeof(Grain128Engine), 16, 12);
        }

        private static readonly SecureRandom RAND = new SecureRandom();

        private static byte[] Random(int size)
        {
            return SecureRandom.GetNextBytes(RAND, size);
        }

        private IStreamCipher Make(Type sct)
        {
            return (IStreamCipher)Activator.CreateInstance(sct);
        }

        private void TestReset(Type sct, int keyLen)
        {
            TestReset(Make(sct), Make(sct), new KeyParameter(Random(keyLen)));
        }

        private void TestReset(Type sct, int keyLen, int ivLen)
        {
            TestReset(Make(sct), Make(sct), new ParametersWithIV(new KeyParameter(Random(keyLen)), Random(ivLen)));
        }

        private void TestReset(IStreamCipher cipher1, IStreamCipher cipher2, ICipherParameters cipherParams)
        {
            cipher1.Init(true, cipherParams);

            byte[] plaintext = new byte[1023];
            byte[] ciphertext = new byte[plaintext.Length];

            // Establish baseline answer
            cipher1.ProcessBytes(plaintext, 0, plaintext.Length, ciphertext, 0);

            // Test encryption resets
            CheckReset(cipher1, cipherParams, true, plaintext, ciphertext);

            // Test decryption resets with fresh instance
            cipher2.Init(false, cipherParams);
            CheckReset(cipher2, cipherParams, false, ciphertext, plaintext);
        }

        private void CheckReset(IStreamCipher cipher, ICipherParameters cipherParams,
            bool encrypt, byte[] pretext, byte[] posttext)
        {
            // Do initial run
            byte[] output = new byte[posttext.Length];
            cipher.ProcessBytes(pretext, 0, pretext.Length, output, 0);

            // Check encrypt resets cipher
            cipher.Init(encrypt, cipherParams);

            try
            {
                cipher.ProcessBytes(pretext, 0, pretext.Length, output, 0);
            }
            catch (Exception e)
            {
                Fail(cipher.AlgorithmName + " init did not reset: " + e.Message);
            }
            if (!Arrays.AreEqual(output, posttext))
            {
                Fail(cipher.AlgorithmName + " init did not reset.", Hex.ToHexString(posttext), Hex.ToHexString(output));
            }

            // Check reset resets data
            cipher.Reset();

            try
            {
                cipher.ProcessBytes(pretext, 0, pretext.Length, output, 0);
            }
            catch (Exception e)
            {
                Fail(cipher.AlgorithmName + " reset did not reset: " + e.Message);
            }
            if (!Arrays.AreEqual(output, posttext))
            {
                Fail(cipher.AlgorithmName + " reset did not reset.");
            }
        }

        public static void Main(string[] args)
        {
            RunTest(new StreamCipherResetTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
