using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * CMAC tester - <a href="http://www.nuee.nagoya-u.ac.jp/labs/tiwata/omac/tv/omac1-tv.txt">Official Test Vectors</a>.
     */
    [TestFixture]
    public class CMacTest
        : SimpleTest
    {
        private static readonly byte[] keyBytes128 = Hex.Decode("2b7e151628aed2a6abf7158809cf4f3c");
        private static readonly byte[] keyBytes192 = Hex.Decode(
            "8e73b0f7da0e6452c810f32b809079e5"
            + "62f8ead2522c6b7b");
        private static readonly byte[] keyBytes256 = Hex.Decode(
            "603deb1015ca71be2b73aef0857d7781"
            + "1f352c073b6108d72d9810a30914dff4");

        private static readonly byte[] input0 = Hex.Decode("");
        private static readonly byte[] input16 = Hex.Decode("6bc1bee22e409f96e93d7e117393172a");
        private static readonly byte[] input40 = Hex.Decode(
            "6bc1bee22e409f96e93d7e117393172a"
            + "ae2d8a571e03ac9c9eb76fac45af8e5130c81c46a35ce411");
        private static readonly byte[] input64 = Hex.Decode(
            "6bc1bee22e409f96e93d7e117393172a"
            + "ae2d8a571e03ac9c9eb76fac45af8e51"
            + "30c81c46a35ce411e5fbc1191a0a52ef"
            + "f69f2445df4f9b17ad2b417be66c3710");

        private static readonly byte[] output_k128_m0 = Hex.Decode("bb1d6929e95937287fa37d129b756746");
        private static readonly byte[] output_k128_m16 = Hex.Decode("070a16b46b4d4144f79bdd9dd04a287c");
        private static readonly byte[] output_k128_m40 = Hex.Decode("dfa66747de9ae63030ca32611497c827");
        private static readonly byte[] output_k128_m64 = Hex.Decode("51f0bebf7e3b9d92fc49741779363cfe");

        private static readonly byte[] output_k192_m0 = Hex.Decode("d17ddf46adaacde531cac483de7a9367");
        private static readonly byte[] output_k192_m16 = Hex.Decode("9e99a7bf31e710900662f65e617c5184");
        private static readonly byte[] output_k192_m40 = Hex.Decode("8a1de5be2eb31aad089a82e6ee908b0e");
        private static readonly byte[] output_k192_m64 = Hex.Decode("a1d5df0eed790f794d77589659f39a11");

        private static readonly byte[] output_k256_m0 = Hex.Decode("028962f61b7bf89efc6b551f4667d983");
        private static readonly byte[] output_k256_m16 = Hex.Decode("28a7023f452e8f82bd4bf28d8c37c35c");
        private static readonly byte[] output_k256_m40 = Hex.Decode("aaf3d8f1de5640c232f5b169b9c911e6");
        private static readonly byte[] output_k256_m64 = Hex.Decode("e1992190549f6ed5696a2c056c315410");

        public CMacTest()
        {
        }

        public override void PerformTest()
        {
            IBlockCipher cipher = new AesEngine();
            IMac mac = new CMac(cipher, 128);

            //128 bytes key

            KeyParameter key = new KeyParameter(keyBytes128);

            // 0 bytes message - 128 bytes key
            mac.Init(key);

            mac.BlockUpdate(input0, 0, input0.Length);

            byte[] outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k128_m0))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k128_m0) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // 16 bytes message - 128 bytes key
            mac.Init(key);

            mac.BlockUpdate(input16, 0, input16.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k128_m16))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k128_m16) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // 40 bytes message - 128 bytes key
            mac.Init(key);

            mac.BlockUpdate(input40, 0, input40.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k128_m40))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k128_m40) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // 64 bytes message - 128 bytes key
            mac.Init(key);

            mac.BlockUpdate(input64, 0, input64.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k128_m64))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k128_m64) + " got "
                    + Hex.ToHexString(outBytes));
            }

            //192 bytes key
            key = new KeyParameter(keyBytes192);

            // 0 bytes message - 192 bytes key
            mac.Init(key);

            mac.BlockUpdate(input0, 0, input0.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k192_m0))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k192_m0) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // 16 bytes message - 192 bytes key
            mac.Init(key);

            mac.BlockUpdate(input16, 0, input16.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k192_m16))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k192_m16) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // 40 bytes message - 192 bytes key
            mac.Init(key);

            mac.BlockUpdate(input40, 0, input40.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k192_m40))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k192_m40) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // 64 bytes message - 192 bytes key
            mac.Init(key);

            mac.BlockUpdate(input64, 0, input64.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k192_m64))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k192_m64) + " got "
                    + Hex.ToHexString(outBytes));
            }

            //256 bytes key

            key = new KeyParameter(keyBytes256);

            // 0 bytes message - 256 bytes key
            mac.Init(key);

            mac.BlockUpdate(input0, 0, input0.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k256_m0))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k256_m0) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // 16 bytes message - 256 bytes key
            mac.Init(key);

            mac.BlockUpdate(input16, 0, input16.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k256_m16))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k256_m16) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // 40 bytes message - 256 bytes key
            mac.Init(key);

            mac.BlockUpdate(input40, 0, input40.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k256_m40))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k256_m40) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // 64 bytes message - 256 bytes key
            mac.Init(key);

            mac.BlockUpdate(input64, 0, input64.Length);

            outBytes = new byte[16];

            mac.DoFinal(outBytes, 0);

            if (!AreEqual(outBytes, output_k256_m64))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output_k256_m64) + " got "
                    + Hex.ToHexString(outBytes));
            }

            TestExceptions();
        }

        private void TestExceptions()
        {
            try 
            {
                CMac mac = new CMac(new AesEngine());
                mac.Init(new ParametersWithIV(new KeyParameter(new byte[16]), new byte[16]));
                Fail("CMac does not accept IV");
            }
            catch(ArgumentException)
            {
                // Expected
            }
        }

        public override string Name
        {
            get { return "CMac"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new CMacTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
