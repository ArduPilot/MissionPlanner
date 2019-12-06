using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /// <remarks>Cast tester - vectors from http://www.ietf.org/rfc/rfc2144.txt</remarks>
    [TestFixture]
    public class Cast5Test
		: CipherTest
    {
        public override string Name
        {
			get { return "Cast5"; }
        }

		internal static SimpleTest[] testlist = new SimpleTest[]{
            new BlockCipherVectorTest(0, new Cast5Engine(), new KeyParameter(Hex.Decode("0123456712345678234567893456789A")), "0123456789ABCDEF", "238B4FE5847E44B2"),
            new BlockCipherVectorTest(0, new Cast5Engine(), new KeyParameter(Hex.Decode("01234567123456782345")), "0123456789ABCDEF", "EB6A711A2C02271B"),
            new BlockCipherVectorTest(0, new Cast5Engine(), new KeyParameter(Hex.Decode("0123456712")), "0123456789ABCDEF", "7Ac816d16E9B302E")};

		public Cast5Test()
			: base(testlist , new Cast5Engine(), new KeyParameter(new byte[16]))
        {
        }

        public static void Main(
			string[] args)
        {
            ITest test = new Cast5Test();
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
