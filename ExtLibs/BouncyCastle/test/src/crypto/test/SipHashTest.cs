using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /// <summary>
    /// SipHash test values from "SipHash: a fast short-input PRF", by Jean-Philippe
    /// Aumasson and Daniel J. Bernstein (https://131002.net/siphash/siphash.pdf), Appendix A.
    /// </summary>
    [TestFixture]
    public class SipHashTest
        : SimpleTest
    {
        private const int UPDATE_BYTES = 0;
        private const int UPDATE_FULL = 1;
        private const int UPDATE_MIX = 2;

        public override string Name
        {
            get { return "SipHash"; }
        }

        public override void PerformTest()
        {
            byte[] key = Hex.Decode("000102030405060708090a0b0c0d0e0f");
            byte[] input = Hex.Decode("000102030405060708090a0b0c0d0e");

            RunMac(key, input, UPDATE_BYTES);
            RunMac(key, input, UPDATE_FULL);
            RunMac(key, input, UPDATE_MIX);

            SecureRandom random = new SecureRandom();
            for (int i = 0; i < 100; ++i)
            {
                RandomTest(random);
            }
        }

        private void RunMac(byte[] key, byte[] input, int updateType)
        {
            long expected = unchecked((long)0xa129ca6149be45e5);

            SipHash mac = new SipHash();
            mac.Init(new KeyParameter(key));

            UpdateMac(mac, input, updateType);

            long result = mac.DoFinal();
            if (expected != result)
            {
                Fail("Result does not match expected value for DoFinal()");
            }

            // NOTE: Little-endian representation of 0xa129ca6149be45e5
            byte[] expectedBytes = Hex.Decode("e545be4961ca29a1");

            UpdateMac(mac, input, updateType);

            byte[] output = new byte[mac.GetMacSize()];
            int len = mac.DoFinal(output, 0);
            if (len != output.Length)
            {
                Fail("Result length does not equal GetMacSize() for DoFinal(byte[],int)");
            }
            if (!AreEqual(expectedBytes, output))
            {
                Fail("Result does not match expected value for DoFinal(byte[],int)");
            }
        }

        private void RandomTest(SecureRandom random)
        {
            byte[] key = new byte[16];
            random.NextBytes(key);

            int length = 1 + random.Next(1024);
            byte[] input = new byte[length];
            random.NextBytes(input);

            SipHash mac = new SipHash();
            mac.Init(new KeyParameter(key));

            UpdateMac(mac, input, UPDATE_BYTES);
            long result1 = mac.DoFinal();

            UpdateMac(mac, input, UPDATE_FULL);
            long result2 = mac.DoFinal();

            UpdateMac(mac, input, UPDATE_MIX);
            long result3 = mac.DoFinal();

            if (result1 != result2 || result1 != result3)
            {
                Fail("Inconsistent results in random test");
            }
        }

        private void UpdateMac(SipHash mac, byte[] input, int updateType)
        {
            switch (updateType)
            {
            case UPDATE_BYTES:
            {
                for (int i = 0; i < input.Length; ++i)
                {
                    mac.Update(input[i]);
                }
                break;
            }
            case UPDATE_FULL:
            {
                mac.BlockUpdate(input, 0, input.Length);
                break;
            }
            case UPDATE_MIX:
            {
                int step = System.Math.Max(1, input.Length / 3);
                int pos = 0;
                while (pos < input.Length)
                {
                    mac.Update(input[pos++]);
                    int len = System.Math.Min(input.Length - pos, step);
                    mac.BlockUpdate(input, pos, len);
                    pos += len;
                }
                break;
            }
            default:
                throw new InvalidOperationException();
            }
        }

        public static void Main(string[] args)
        {
            RunTest(new SipHashTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
