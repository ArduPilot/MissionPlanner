using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /// <summary> RC4 Test</summary>
    [TestFixture]
    public class RC4Test
		: ITest
    {
        public string Name
        {
			get { return "RC4"; }
        }

		internal StreamCipherVectorTest[] tests = new StreamCipherVectorTest[]{
            new StreamCipherVectorTest(0, new RC4Engine(), new KeyParameter(Hex.Decode("0123456789ABCDEF")), "4e6f772069732074", "3afbb5c77938280d"),
            new StreamCipherVectorTest(0, new RC4Engine(), new KeyParameter(Hex.Decode("0123456789ABCDEF")), "68652074696d6520", "1cf1e29379266d59"),
            new StreamCipherVectorTest(0, new RC4Engine(), new KeyParameter(Hex.Decode("0123456789ABCDEF")), "666f7220616c6c20", "12fbb0c771276459")};

        public virtual ITestResult Perform()
        {
            for (int i = 0; i != tests.Length; i++)
            {
                ITestResult res = tests[i].Perform();

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
            ITest test = new RC4Test();
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
