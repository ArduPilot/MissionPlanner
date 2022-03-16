using System;

using NUnit.Framework;

using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;


namespace Org.BouncyCastle.Crypto.Tests
{

    /**
    * RC5 tester - vectors from ftp://ftp.nordu.net/rfc/rfc2040.txt
    *
    * RFC 2040 "The RC5, RC5-CBC, RC5-CBC-Pad, and RC5-CTS Algorithms"
    */
    [TestFixture]
    public class RC5Test: ITest
    {
        BlockCipherVectorTest[] tests =
        {
            new BlockCipherVectorTest(0, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 0),
                        Hex.Decode("0000000000000000")),
                    "0000000000000000", "7a7bba4d79111d1e"),
            new BlockCipherVectorTest(1, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 0),
                        Hex.Decode("0000000000000000")),
                    "ffffffffffffffff", "797bba4d78111d1e"),
            new BlockCipherVectorTest(2, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 0),
                        Hex.Decode("0000000000000001")),
                    "0000000000000000", "7a7bba4d79111d1f"),
            new BlockCipherVectorTest(3, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 0),
                        Hex.Decode("0000000000000000")),
                    "0000000000000001", "7a7bba4d79111d1f"),
            new BlockCipherVectorTest(4, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 0),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "8b9ded91ce7794a6"),
            new BlockCipherVectorTest(5, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("11"), 1),
                        Hex.Decode("0000000000000000")),
                    "0000000000000000", "2f759fe7ad86a378"),
            new BlockCipherVectorTest(6, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 2),
                        Hex.Decode("0000000000000000")),
                    "0000000000000000", "dca2694bf40e0788"),
            new BlockCipherVectorTest(7, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00000000"), 2),
                        Hex.Decode("0000000000000000")),
                    "0000000000000000", "dca2694bf40e0788"),
            new BlockCipherVectorTest(8, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00000000"), 8),
                        Hex.Decode("0000000000000000")),
                    "0000000000000000", "dcfe098577eca5ff"),
            new BlockCipherVectorTest(9, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 8),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "9646fb77638f9ca8"),
            new BlockCipherVectorTest(10, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 12),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "b2b3209db6594da4"),
            new BlockCipherVectorTest(11, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 16),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "545f7f32a5fc3836"),
            new BlockCipherVectorTest(12, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("01020304"), 8),
                        Hex.Decode("0000000000000000")),
                    "ffffffffffffffff", "8285e7c1b5bc7402"),
            new BlockCipherVectorTest(13, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("01020304"), 12),
                        Hex.Decode("0000000000000000")),
                    "ffffffffffffffff", "fc586f92f7080934"),
            new BlockCipherVectorTest(14, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("01020304"), 16),
                        Hex.Decode("0000000000000000")),
                    "ffffffffffffffff", "cf270ef9717ff7c4"),
            new BlockCipherVectorTest(15, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("0102030405060708"), 12),
                        Hex.Decode("0000000000000000")),
                    "ffffffffffffffff", "e493f1c1bb4d6e8c"),
            new BlockCipherVectorTest(16, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("0102030405060708"), 8),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "5c4c041e0f217ac3"),
            new BlockCipherVectorTest(17, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("0102030405060708"), 12),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "921f12485373b4f7"),
            new BlockCipherVectorTest(18, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("0102030405060708"), 16),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "5ba0ca6bbe7f5fad"),
            new BlockCipherVectorTest(19, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("01020304050607081020304050607080"), 8),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "c533771cd0110e63"),
            new BlockCipherVectorTest(20, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("01020304050607081020304050607080"), 12),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "294ddb46b3278d60"),
            new BlockCipherVectorTest(21, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("01020304050607081020304050607080"), 16),
                        Hex.Decode("0102030405060708")),
                    "1020304050607080", "dad6bda9dfe8f7e8"),
            new BlockCipherVectorTest(22, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("0102030405"), 12),
                        Hex.Decode("0000000000000000")),
                    "ffffffffffffffff", "97e0787837ed317f"),
            new BlockCipherVectorTest(23, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("0102030405"), 8),
                        Hex.Decode("0000000000000000")),
                    "ffffffffffffffff", "7875dbf6738c6478"),
            new BlockCipherVectorTest(23, new CbcBlockCipher(new RC532Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("0102030405"), 8),
                        Hex.Decode("7875dbf6738c6478")),
                    "0808080808080808", "8f34c3c681c99695"),
            new BlockCipherVectorTest(640, new CbcBlockCipher(new RC564Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 0),
                        Hex.Decode("00000000000000000000000000000000")),
                    "00000000000000000000000000000000", "9f09b98d3f6062d9d4d59973d00e0e63"),
            new BlockCipherVectorTest(641, new CbcBlockCipher(new RC564Engine()),
                    new ParametersWithIV(
                        new RC5Parameters(Hex.Decode("00"), 0),
                        Hex.Decode("00000000000000000000000000000000")),
                    "ffffffffffffffffffffffffffffffff", "9e09b98d3f6062d9d3d59973d00e0e63")
        };

		public string Name
        {
			get { return "RC5"; }
        }

		public ITestResult Perform()
        {
            for (int i = 0; i != tests.Length; i++)
            {
                ITestResult  res = tests[i].Perform();

                if (!res.IsSuccessful())
                {
                    return res;
                }
            }

            return new SimpleTestResult(true, Name + ": Okay");
        }

        public static void Main(
            string[] args)
        {
            RC5Test test = new RC5Test();
            ITestResult result = test.Perform();

            Console.WriteLine(result);
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
