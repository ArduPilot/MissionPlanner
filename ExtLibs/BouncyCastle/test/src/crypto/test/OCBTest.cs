using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * Test vectors from <a href="http://tools.ietf.org/html/rfc7253">RFC 7253 on The OCB
     * Authenticated-Encryption Algorithm</a>
     */
    public class OcbTest
        : SimpleTest
    {
        private const string KEY_128 = "000102030405060708090A0B0C0D0E0F";
        private const string KEY_96 = "0F0E0D0C0B0A09080706050403020100";

        /*
         * Test vectors from Appendix A of the specification, containing the strings N, A, P, C in order
         */

        private static readonly string[][] TEST_VECTORS_128 = new string[][]{
            new string[]{ "BBAA99887766554433221100",
              "",
              "",
              "785407BFFFC8AD9EDCC5520AC9111EE6" },
            new string[]{ "BBAA99887766554433221101",
              "0001020304050607",
              "0001020304050607",
              "6820B3657B6F615A5725BDA0D3B4EB3A257C9AF1F8F03009" },
            new string[]{ "BBAA99887766554433221102",
              "0001020304050607",
              "",
              "81017F8203F081277152FADE694A0A00" },
            new string[]{ "BBAA99887766554433221103",
              "",
              "0001020304050607",
              "45DD69F8F5AAE72414054CD1F35D82760B2CD00D2F99BFA9" },
            new string[]{ "BBAA99887766554433221104",
              "000102030405060708090A0B0C0D0E0F",
              "000102030405060708090A0B0C0D0E0F",
              "571D535B60B277188BE5147170A9A22C3AD7A4FF3835B8C5701C1CCEC8FC3358" },
            new string[]{ "BBAA99887766554433221105",
              "000102030405060708090A0B0C0D0E0F",
              "",
              "8CF761B6902EF764462AD86498CA6B97" },
            new string[]{ "BBAA99887766554433221106",
              "",
              "000102030405060708090A0B0C0D0E0F",
              "5CE88EC2E0692706A915C00AEB8B2396F40E1C743F52436BDF06D8FA1ECA343D" },
            new string[]{ "BBAA99887766554433221107",
              "000102030405060708090A0B0C0D0E0F1011121314151617",
              "000102030405060708090A0B0C0D0E0F1011121314151617",
              "1CA2207308C87C010756104D8840CE1952F09673A448A122C92C62241051F57356D7F3C90BB0E07F" },
            new string[]{ "BBAA99887766554433221108",
              "000102030405060708090A0B0C0D0E0F1011121314151617",
              "",
              "6DC225A071FC1B9F7C69F93B0F1E10DE" },
            new string[]{ "BBAA99887766554433221109",
              "",
              "000102030405060708090A0B0C0D0E0F1011121314151617",
              "221BD0DE7FA6FE993ECCD769460A0AF2D6CDED0C395B1C3CE725F32494B9F914D85C0B1EB38357FF" },
            new string[]{ "BBAA9988776655443322110A",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F",
              "BD6F6C496201C69296C11EFD138A467ABD3C707924B964DEAFFC40319AF5A48540FBBA186C5553C68AD9F592A79A4240" },
            new string[]{ "BBAA9988776655443322110B",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F",
              "",
              "FE80690BEE8A485D11F32965BC9D2A32" },
            new string[]{ "BBAA9988776655443322110C",
              "",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F",
              "2942BFC773BDA23CABC6ACFD9BFD5835BD300F0973792EF46040C53F1432BCDFB5E1DDE3BC18A5F840B52E653444D5DF" },
            new string[]{ "BBAA9988776655443322110D",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F2021222324252627",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F2021222324252627",
              "D5CA91748410C1751FF8A2F618255B68A0A12E093FF454606E59F9C1D0DDC54B65E8628E568BAD7AED07BA06A4A69483A7035490C5769E60" },
            new string[]{ "BBAA9988776655443322110E",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F2021222324252627",
              "",
              "C5CD9D1850C141E358649994EE701B68" },
            new string[]{ "BBAA9988776655443322110F",
              "",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F2021222324252627",
              "4412923493C57D5DE0D700F753CCE0D1D2D95060122E9F15A5DDBFC5787E50B5CC55EE507BCB084E479AD363AC366B95A98CA5F3000B1479" },
        };

        private static readonly string[][] TEST_VECTORS_96 = new string[][]{
            new string[]{ "BBAA9988776655443322110D",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F2021222324252627",
              "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F2021222324252627",
              "1792A4E31E0755FB03E31B22116E6C2DDF9EFD6E33D536F1A0124B0A55BAE884ED93481529C76B6AD0C515F4D1CDD4FDAC4F02AA" },
        };

        public override string Name
        {
            get { return "OCB"; }
        }

        public override void PerformTest()
        {
            byte[] K128 = Hex.Decode(KEY_128);
            for (int i = 0; i < TEST_VECTORS_128.Length; ++i)
            {
                RunTestCase("Test Case " + i, TEST_VECTORS_128[i], 128, K128);
            }

            byte[] K96 = Hex.Decode(KEY_96);
            for (int i = 0; i < TEST_VECTORS_96.Length; ++i)
            {
                RunTestCase("Test Case " + i, TEST_VECTORS_96[i], 96, K96);
            }

            RunLongerTestCase(128, 128, "67E944D23256C5E0B6C61FA22FDF1EA2");
            RunLongerTestCase(192, 128, "F673F2C3E7174AAE7BAE986CA9F29E17");
            RunLongerTestCase(256, 128, "D90EB8E9C977C88B79DD793D7FFA161C");
            RunLongerTestCase(128, 96, "77A3D8E73589158D25D01209");
            RunLongerTestCase(192, 96, "05D56EAD2752C86BE6932C5E");
            RunLongerTestCase(256, 96, "5458359AC23B0CBA9E6330DD");
            RunLongerTestCase(128, 64, "192C9B7BD90BA06A");
            RunLongerTestCase(192, 64, "0066BC6E0EF34E24");
            RunLongerTestCase(256, 64, "7D4EA5D445501CBE");

            RandomTests();
            OutputSizeTests();
            DoTestExceptions();
        }

        private void DoTestExceptions()
        {
            IAeadBlockCipher ocb = CreateOcbCipher();

            try
            {
                ocb = new OcbBlockCipher(new DesEngine(), new DesEngine());
                Fail("incorrect block size not picked up");
            }
            catch (ArgumentException)
            {
                // expected
            }

            try
            {
                ocb.Init(false, new KeyParameter(new byte[16]));
                Fail("illegal argument not picked up");
            }
            catch (ArgumentException)
            {
                // expected
            }

            // TODO
            //AEADTestUtil.testReset(this, createOCBCipher(), createOCBCipher(), new AEADParameters(new KeyParameter(new byte[16]), 128, new byte[15]));
            //AEADTestUtil.testTampering(this, ocb, new AEADParameters(new KeyParameter(new byte[16]), 128, new byte[15]));
        }

        private void RunTestCase(string testName, string[] testVector, int macLengthBits, byte[] K)
        {
            int pos = 0;
            byte[] N = Hex.Decode(testVector[pos++]);
            byte[] A = Hex.Decode(testVector[pos++]);
            byte[] P = Hex.Decode(testVector[pos++]);
            byte[] C = Hex.Decode(testVector[pos++]);

            int macLengthBytes = macLengthBits / 8;

            KeyParameter keyParameter = new KeyParameter(K);
            AeadParameters parameters = new AeadParameters(keyParameter, macLengthBits, N, A);

            IAeadBlockCipher encCipher = InitOcbCipher(true, parameters);
            IAeadBlockCipher decCipher = InitOcbCipher(false, parameters);

            CheckTestCase(encCipher, decCipher, testName, macLengthBytes, P, C);
            CheckTestCase(encCipher, decCipher, testName + " (reused)", macLengthBytes, P, C);

            // Key reuse
            AeadParameters keyReuseParams = AeadTestUtilities.ReuseKey(parameters);
            encCipher.Init(true, keyReuseParams);
            decCipher.Init(false, keyReuseParams);
            CheckTestCase(encCipher, decCipher, testName + " (key reuse)", macLengthBytes, P, C);
        }

        private IBlockCipher CreateUnderlyingCipher()
        {
            return new AesEngine();
        }

        private IAeadBlockCipher CreateOcbCipher()
        {
            return new OcbBlockCipher(CreateUnderlyingCipher(), CreateUnderlyingCipher());
        }

        private IAeadBlockCipher InitOcbCipher(bool forEncryption, AeadParameters parameters)
        {
            IAeadBlockCipher c = CreateOcbCipher();
            c.Init(forEncryption, parameters);
            return c;
        }

        private void CheckTestCase(IAeadBlockCipher encCipher, IAeadBlockCipher decCipher, string testName,
            int macLengthBytes, byte[] P, byte[] C)
        {
            byte[] tag = Arrays.CopyOfRange(C, C.Length - macLengthBytes, C.Length);

            {
                byte[] enc = new byte[encCipher.GetOutputSize(P.Length)];
                int len = encCipher.ProcessBytes(P, 0, P.Length, enc, 0);
                len += encCipher.DoFinal(enc, len);

                if (enc.Length != len)
                {
                    Fail("encryption reported incorrect length: " + testName);
                }

                if (!AreEqual(C, enc))
                {
                    Fail("incorrect encrypt in: " + testName);
                }

                if (!AreEqual(tag, encCipher.GetMac()))
                {
                    Fail("getMac() not the same as the appended tag: " + testName);
                }
            }

            {
                byte[] dec = new byte[decCipher.GetOutputSize(C.Length)];
                int len = decCipher.ProcessBytes(C, 0, C.Length, dec, 0);
                len += decCipher.DoFinal(dec, len);

                if (dec.Length != len)
                {
                    Fail("decryption reported incorrect length: " + testName);
                }

                if (!AreEqual(P, dec))
                {
                    Fail("incorrect decrypt in: " + testName);
                }

                if (!AreEqual(tag, decCipher.GetMac()))
                {
                    Fail("getMac() not the same as the appended tag: " + testName);
                }
            }
        }

        private void RunLongerTestCase(int keyLen, int tagLen, string expectedOutputHex)
        {
            byte[] expectedOutput = Hex.Decode(expectedOutputHex);
            byte[] keyBytes = new byte[keyLen / 8];
            keyBytes[keyBytes.Length - 1] = (byte)tagLen;
            KeyParameter key = new KeyParameter(keyBytes);

            IAeadBlockCipher c1 = InitOcbCipher(true, new AeadParameters(key, tagLen, CreateNonce(385)));
            IAeadBlockCipher c2 = CreateOcbCipher();

            long total = 0;

            byte[] S = new byte[128];

            uint n = 0;
            for (int i = 0; i < 128; ++i)
            {
                c2.Init(true, new AeadParameters(key, tagLen, CreateNonce(++n)));
                total += UpdateCiphers(c1, c2, S, i, true, true);
                c2.Init(true, new AeadParameters(key, tagLen, CreateNonce(++n)));
                total += UpdateCiphers(c1, c2, S, i, false, true);
                c2.Init(true, new AeadParameters(key, tagLen, CreateNonce(++n)));
                total += UpdateCiphers(c1, c2, S, i, true, false);
            }

            long expectedTotal = 16256 + (48 * tagLen);

            if (total != expectedTotal)
            {
                Fail("test generated the wrong amount of input: " + total);
            }

            byte[] output = new byte[c1.GetOutputSize(0)];
            c1.DoFinal(output, 0);

            if (!AreEqual(expectedOutput, output))
            {
                Fail("incorrect encrypt in long-form test");
            }
        }

        private byte[] CreateNonce(uint n)
        {
            return new byte[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)(n >> 8), (byte)n };
        }

        private int UpdateCiphers(IAeadBlockCipher c1, IAeadBlockCipher c2, byte[] S, int i,
            bool includeAAD, bool includePlaintext)
        {
            int inputLen = includePlaintext ? i : 0;
            int outputLen = c2.GetOutputSize(inputLen);

            byte[] output = new byte[outputLen];

            int len = 0;

            if (includeAAD) {
                c2.ProcessAadBytes(S, 0, i);
            }

            if (includePlaintext) {
                len += c2.ProcessBytes(S, 0, i, output, len);
            }

            len += c2.DoFinal(output, len);

            c1.ProcessAadBytes(output, 0, len);

            return len;
        }

        private void RandomTests()
        {
            SecureRandom srng = new SecureRandom();
            srng.SetSeed(DateTimeUtilities.CurrentUnixMs());
            for (int i = 0; i < 10; ++i)
            {
                RandomTest(srng);
            }
        }

        private void RandomTest(SecureRandom srng)
        {
            int kLength = 16 + 8 * (System.Math.Abs(srng.NextInt()) % 3);
            byte[] K = new byte[kLength];
            srng.NextBytes(K);

            int pLength = (int)((uint)srng.NextInt() >> 16);
            byte[] P = new byte[pLength];
            srng.NextBytes(P);

            int aLength = (int)((uint)srng.NextInt() >> 24);
            byte[] A = new byte[aLength];
            srng.NextBytes(A);

            int saLength = (int)((uint)srng.NextInt() >> 24);
            byte[] SA = new byte[saLength];
            srng.NextBytes(SA);

            int ivLength = 1 + NextInt(srng, 15);
            byte[] IV = new byte[ivLength];
            srng.NextBytes(IV);

            AeadParameters parameters = new AeadParameters(new KeyParameter(K), 16 * 8, IV, A);
            IAeadBlockCipher cipher = InitOcbCipher(true, parameters);
            byte[] C = new byte[cipher.GetOutputSize(P.Length)];
            int predicted = cipher.GetUpdateOutputSize(P.Length);

            int split = NextInt(srng, SA.Length + 1);
            cipher.ProcessAadBytes(SA, 0, split);
            int len = cipher.ProcessBytes(P, 0, P.Length, C, 0);
            cipher.ProcessAadBytes(SA, split, SA.Length - split);

            if (predicted != len)
            {
                Fail("encryption reported incorrect update length in randomised test");
            }

            len += cipher.DoFinal(C, len);

            if (C.Length != len)
            {
                Fail("encryption reported incorrect length in randomised test");
            }

            byte[] encT = cipher.GetMac();
            byte[] tail = new byte[C.Length - P.Length];
            Array.Copy(C, P.Length, tail, 0, tail.Length);

            if (!AreEqual(encT, tail))
            {
                Fail("stream contained wrong mac in randomised test");
            }

            cipher.Init(false, parameters);
            byte[] decP = new byte[cipher.GetOutputSize(C.Length)];
            predicted = cipher.GetUpdateOutputSize(C.Length);

            split = NextInt(srng, SA.Length + 1);
            cipher.ProcessAadBytes(SA, 0, split);
            len = cipher.ProcessBytes(C, 0, C.Length, decP, 0);
            cipher.ProcessAadBytes(SA, split, SA.Length - split);

            if (predicted != len)
            {
                Fail("decryption reported incorrect update length in randomised test");
            }

            len += cipher.DoFinal(decP, len);

            if (!AreEqual(P, decP))
            {
                Fail("incorrect decrypt in randomised test");
            }

            byte[] decT = cipher.GetMac();
            if (!AreEqual(encT, decT))
            {
                Fail("decryption produced different mac from encryption");
            }

            //
            // key reuse test
            //
            cipher.Init(false, AeadTestUtilities.ReuseKey(parameters));
            decP = new byte[cipher.GetOutputSize(C.Length)];

            split = NextInt(srng, SA.Length + 1);
            cipher.ProcessAadBytes(SA, 0, split);
            len = cipher.ProcessBytes(C, 0, C.Length, decP, 0);
            cipher.ProcessAadBytes(SA, split, SA.Length - split);

            len += cipher.DoFinal(decP, len);

            if (!AreEqual(P, decP))
            {
                Fail("incorrect decrypt in randomised test");
            }

            decT = cipher.GetMac();
            if (!AreEqual(encT, decT))
            {
                Fail("decryption produced different mac from encryption");
            }
        }

        private void OutputSizeTests()
        {
            byte[] K = new byte[16];
            byte[] A = null;
            byte[] IV = new byte[15];

            AeadParameters parameters = new AeadParameters(new KeyParameter(K), 16 * 8, IV, A);
            IAeadBlockCipher cipher = InitOcbCipher(true, parameters);

            if (cipher.GetUpdateOutputSize(0) != 0)
            {
                Fail("incorrect getUpdateOutputSize for initial 0 bytes encryption");
            }

            if (cipher.GetOutputSize(0) != 16)
            {
                Fail("incorrect getOutputSize for initial 0 bytes encryption");
            }

            cipher.Init(false, parameters);

            if (cipher.GetUpdateOutputSize(0) != 0)
            {
                Fail("incorrect getUpdateOutputSize for initial 0 bytes decryption");
            }

            // NOTE: 0 bytes would be truncated data, but we want it to fail in the doFinal, not here
            if (cipher.GetOutputSize(0) != 0)
            {
                Fail("fragile getOutputSize for initial 0 bytes decryption");
            }

            if (cipher.GetOutputSize(16) != 0)
            {
                Fail("incorrect getOutputSize for initial MAC-size bytes decryption");
            }
        }

        private static int NextInt(SecureRandom rand, int n)
        {
            if ((n & -n) == n)  // i.e., n is a power of 2
            {
                return (int)(((uint)n * (ulong)((uint)rand.NextInt() >> 1)) >> 31);
            }

            int bits, value;
            do
            {
                bits = (int)((uint)rand.NextInt() >> 1);
                value = bits % n;
            }
            while (bits - value + (n - 1) < 0);

            return value;
        }

        public static void Main(
            string[] args)
        {
            RunTest(new OcbTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
