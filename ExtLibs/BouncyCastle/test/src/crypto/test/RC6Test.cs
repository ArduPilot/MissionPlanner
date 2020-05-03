using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /// <remarks> RC6 Test - test vectors from AES Submitted RSA Reference implementation.
    /// ftp://ftp.funet.fi/pub/crypt/cryptography/symmetric/aes/rc6-unix-refc.tar
    /// </remarks>
	[TestFixture]
    public class RC6Test
		: CipherTest
    {
        public override string Name
        {
			get { return "RC6"; }
        }

		internal static SimpleTest[] tests = new SimpleTest[]{
            new BlockCipherVectorTest(0, new RC6Engine(), new KeyParameter(Hex.Decode("00000000000000000000000000000000")), "80000000000000000000000000000000", "f71f65e7b80c0c6966fee607984b5cdf"),
            new BlockCipherVectorTest(1, new RC6Engine(), new KeyParameter(Hex.Decode("000000000000000000000000000000008000000000000000")), "00000000000000000000000000000000", "dd04c176440bbc6686c90aee775bd368"),
            new BlockCipherVectorTest(2, new RC6Engine(), new KeyParameter(Hex.Decode("000000000000000000000000000000000000001000000000")), "00000000000000000000000000000000", "937fe02d20fcb72f0f57201012b88ba4"),
            new BlockCipherVectorTest(3, new RC6Engine(), new KeyParameter(Hex.Decode("00000001000000000000000000000000")), "00000000000000000000000000000000", "8a380594d7396453771a1dfbe2914c8e"),
            new BlockCipherVectorTest(4, new RC6Engine(), new KeyParameter(Hex.Decode("1000000000000000000000000000000000000000000000000000000000000000")), "00000000000000000000000000000000", "11395d4bfe4c8258979ee2bf2d24dff4"),
            new BlockCipherVectorTest(5, new RC6Engine(), new KeyParameter(Hex.Decode("0000000000000000000000000000000000080000000000000000000000000000")), "00000000000000000000000000000000", "3d6f7e99f6512553bb983e8f75672b97")};

        public RC6Test()
			: base(tests, new RC6Engine(), new KeyParameter(new byte[32]))
        {
        }

        public static void Main(
            string[] args)
        {
            ITest test = new RC6Test();
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
