using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    [TestFixture]
    public class Blake2sDigestTest
        : SimpleTest
    {
        // Vectors from BLAKE2 web site: https://blake2.net/blake2s-test.txt
        private static readonly string[][] keyedTestVectors = {
            // input/message, key, hash
            new string[]{
                "",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
                "48a8997da407876b3d79c0d92325ad3b89cbb754d86ab71aee047ad345fd2c49",
            },
            new string[]{
                "00",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
                "40d15fee7c328830166ac3f918650f807e7e01e177258cdc0a39b11f598066f1",
            },
            new string[]{
                "0001",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
                "6bb71300644cd3991b26ccd4d274acd1adeab8b1d7914546c1198bbe9fc9d803",
            },
            new string[]{
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
                "172ffc67153d12e0ca76a8b6cd5d4731885b39ce0cac93a8972a18006c8b8baf",
            },
            new string[]{
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
                "4f8ce1e51d2fe7f24043a904d898ebfc91975418753413aa099b795ecb35cedb",
            },
            new string[]{
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfe",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
                "3fb735061abc519dfe979e54c1ee5bfad0a9d858b3315bad34bde999efd724dd",
            },
        };

        public override string Name
        {
            get { return "BLAKE2s"; }
        }

        public void DoTestDigestWithKeyedTestVectors()
        {
            Blake2sDigest digest = new Blake2sDigest(Hex.Decode(
                keyedTestVectors[0][1]));
            for (int i = 0; i != keyedTestVectors.Length; i++)
            {
                String[] keyedTestVector = keyedTestVectors[i];
                byte[] input = Hex.Decode(keyedTestVector[0]);
                digest.Reset();

                digest.BlockUpdate(input, 0, input.Length);
                byte[] hash = new byte[32];
                digest.DoFinal(hash, 0);

                if (!AreEqual(Hex.Decode(keyedTestVector[2]), hash))
                {
                    Fail("BLAKE2s mismatch on test vector ",
                        keyedTestVector[2],
                        Hex.ToHexString(hash));
                }
            }
        }

        public void DoTestDigestWithKeyedTestVectorsAndRandomUpdate()
        {
            Blake2sDigest digest = new Blake2sDigest(Hex.Decode(
                keyedTestVectors[0][1]));
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j != keyedTestVectors.Length; j++)
                {
                    String[] keyedTestVector = keyedTestVectors[j];
                    byte[] input = Hex.Decode(keyedTestVector[0]);
                    if (input.Length < 3)
                    {
                        continue;
                    }
                    digest.Reset();

                    int pos = (random.Next() & 0xffff) % input.Length;
                    if (pos > 0)
                    {
                        digest.BlockUpdate(input, 0, pos);
                    }
                    digest.Update(input[pos]);
                    if (pos < (input.Length - 1))
                    {
                        digest.BlockUpdate(input, pos + 1, input.Length - (pos + 1));
                    }

                    byte[] hash = new byte[32];
                    digest.DoFinal(hash, 0);

                    if (!AreEqual(Hex.Decode(keyedTestVector[2]), hash))
                    {
                        Fail("BLAKE2s mismatch on test vector ",
                            keyedTestVector[2],
                            Hex.ToHexString(hash));
                    }
                }
            }
        }

        public void DoTestReset()
        {
            // Generate a non-zero key
            byte[] key = new byte[32];
            for (byte i = 0; i < key.Length; i++)
            {
                key[i] = i;
            }
            // Generate some non-zero input longer than the key
            byte[] input = new byte[key.Length + 1];
            for (byte i = 0; i < input.Length; i++)
            {
                input[i] = i;
            }
            // Hash the input
            Blake2sDigest digest = new Blake2sDigest(key);
            digest.BlockUpdate(input, 0, input.Length);
            byte[] hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);
            // Create a second instance, hash the input without calling doFinal()
            Blake2sDigest digest1 = new Blake2sDigest(key);
            digest1.BlockUpdate(input, 0, input.Length);
            // Reset the second instance and hash the input again
            digest1.Reset();
            digest1.BlockUpdate(input, 0, input.Length);
            byte[] hash1 = new byte[digest.GetDigestSize()];
            digest1.DoFinal(hash1, 0);
            // The hashes should be identical
            if (!AreEqual(hash, hash1))
            {
                Fail("BLAKE2s mismatch on test vector ",
                    Hex.ToHexString(hash),
                    Hex.ToHexString(hash1));
            }
        }

        // Self-test routine from https://tools.ietf.org/html/rfc7693#appendix-E
        private static readonly string SELF_TEST_RESULT =
            "6A411F08CE25ADCDFB02ABA641451CEC53C598B24F4FC787FBDC88797F4C1DFE";
        private static readonly int[] SELF_TEST_DIGEST_LEN = {16, 20, 28, 32};
        private static readonly int[] SELF_TEST_INPUT_LEN = {0, 3, 64, 65, 255, 1024};

        private static byte[] selfTestSequence(int len, int seed)
        {
            int a = (int)(0xDEAD4BAD * seed);
            int b = 1;
            int t;
            byte[] output = new byte[len];

            for (int i = 0; i < len; i++)
            {
                t = a + b;
                a = b;
                b = t;
                output[i] = (byte)(t >> 24);
            }

            return output;
        }

        public void RunSelfTest()
        {
            Blake2sDigest testDigest = new Blake2sDigest();
            byte[] md = new byte[32];

            for (int i = 0; i < 4; i++)
            {
                int outlen = SELF_TEST_DIGEST_LEN[i];
                for (int j = 0; j < 6; j++)
                {
                    int inlen = SELF_TEST_INPUT_LEN[j];

                    // unkeyed hash
                    byte[] input = selfTestSequence(inlen, inlen);
                    Blake2sDigest unkeyedDigest = new Blake2sDigest(outlen * 8);
                    unkeyedDigest.BlockUpdate(input, 0, inlen);
                    unkeyedDigest.DoFinal(md, 0);
                    // hash the hash
                    testDigest.BlockUpdate(md, 0, outlen);

                    // keyed hash
                    byte[] key = selfTestSequence(outlen, outlen);
                    Blake2sDigest keyedDigest = new Blake2sDigest(key, outlen, null,
                        null);
                    keyedDigest.BlockUpdate(input, 0, inlen);
                    keyedDigest.DoFinal(md, 0);
                    // hash the hash
                    testDigest.BlockUpdate(md, 0, outlen);
                }
            }

            byte[] hash = new byte[32];
            testDigest.DoFinal(hash, 0);
            if (!AreEqual(Hex.Decode(SELF_TEST_RESULT), hash))
            {
                Fail("BLAKE2s mismatch on test vector ",
                    SELF_TEST_RESULT,
                    Hex.ToHexString(hash));
            }
        }

        public override void PerformTest()
        {
            DoTestDigestWithKeyedTestVectors();
            DoTestDigestWithKeyedTestVectorsAndRandomUpdate();
            DoTestReset();
            RunSelfTest();
        }

        public static void Main(string[] args)
        {
            RunTest(new Blake2sDigestTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
