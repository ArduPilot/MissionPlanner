using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{

    /// <summary> RC2 tester - vectors from  ftp://ftp.isi.edu/in-notes/rfc2268.txt
    ///
    /// RFC 2268 "A Description of the RC2(r) Encryption Algorithm"
    /// </summary>

    [TestFixture]
    public class RC2Test:CipherTest
    {
        public override string Name
        {
			get { return "RC2"; }
        }

		internal static BlockCipherVectorTest[] tests = new BlockCipherVectorTest[]{
            new BlockCipherVectorTest(0, new RC2Engine(), new RC2Parameters(Hex.Decode("0000000000000000"), 63), "0000000000000000", "ebb773f993278eff"),
            new BlockCipherVectorTest(1, new RC2Engine(), new RC2Parameters(Hex.Decode("ffffffffffffffff"), 64), "ffffffffffffffff", "278b27e42e2f0d49"),
            new BlockCipherVectorTest(2, new RC2Engine(), new RC2Parameters(Hex.Decode("3000000000000000"), 64), "1000000000000001", "30649edf9be7d2c2"),
            new BlockCipherVectorTest(3, new RC2Engine(), new RC2Parameters(Hex.Decode("88"), 64), "0000000000000000", "61a8a244adacccf0"),
            new BlockCipherVectorTest(4, new RC2Engine(), new RC2Parameters(Hex.Decode("88bca90e90875a"), 64), "0000000000000000", "6ccf4308974c267f"),
            new BlockCipherVectorTest(5, new RC2Engine(), new RC2Parameters(Hex.Decode("88bca90e90875a7f0f79c384627bafb2"), 64), "0000000000000000", "1a807d272bbe5db1"),
            new BlockCipherVectorTest(6, new RC2Engine(), new RC2Parameters(Hex.Decode("88bca90e90875a7f0f79c384627bafb2"), 128), "0000000000000000", "2269552ab0f85ca6"),
            new BlockCipherVectorTest(7, new RC2Engine(), new RC2Parameters(Hex.Decode("88bca90e90875a7f0f79c384627bafb216f80a6f85920584c42fceb0be255daf1e"), 129), "0000000000000000", "5b78d3a43dfff1f1")};

		public RC2Test()
			:base(tests, new RC2Engine(), new RC2Parameters(new byte[16]))
        {
        }

		public static void Main(
            string[] args)
        {
            ITest test = new RC2Test();
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
