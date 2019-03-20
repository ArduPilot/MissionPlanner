using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    [TestFixture]
    public class SkipjackTest
		: CipherTest
    {
        public override string Name
        {
			get { return "SKIPJACK"; }
        }

        internal static SimpleTest[] tests = new SimpleTest[]{
            new BlockCipherVectorTest(0, new SkipjackEngine(), new KeyParameter(Hex.Decode("00998877665544332211")), "33221100ddccbbaa", "2587cae27a12d300")};

		public SkipjackTest()
			: base(tests, new SkipjackEngine(), new KeyParameter(new byte[16]))
		{
        }

		public static void Main(
            string[] args)
        {
            ITest test = new SkipjackTest();
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
