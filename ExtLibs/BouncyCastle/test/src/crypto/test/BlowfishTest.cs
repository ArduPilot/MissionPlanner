using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/// <remarks>Blowfish tester - vectors from http://www.counterpane.com/vectors.txt</remarks>
    [TestFixture]
    public class BlowfishTest
		: CipherTest
    {
        public override string Name
        {
			get { return "Blowfish"; }
        }

        internal static SimpleTest[] tests = new SimpleTest[]{
            new BlockCipherVectorTest(0, new BlowfishEngine(), new KeyParameter(Hex.Decode("0000000000000000")), "0000000000000000", "4EF997456198DD78"),
            new BlockCipherVectorTest(1, new BlowfishEngine(), new KeyParameter(Hex.Decode("FFFFFFFFFFFFFFFF")), "FFFFFFFFFFFFFFFF", "51866FD5B85ECB8A"),
            new BlockCipherVectorTest(2, new BlowfishEngine(), new KeyParameter(Hex.Decode("3000000000000000")), "1000000000000001", "7D856F9A613063F2"),
            new BlockCipherVectorTest(3, new BlowfishEngine(), new KeyParameter(Hex.Decode("1111111111111111")), "1111111111111111", "2466DD878B963C9D"),
            new BlockCipherVectorTest(4, new BlowfishEngine(), new KeyParameter(Hex.Decode("0123456789ABCDEF")), "1111111111111111", "61F9C3802281B096"),
            new BlockCipherVectorTest(5, new BlowfishEngine(), new KeyParameter(Hex.Decode("FEDCBA9876543210")), "0123456789ABCDEF", "0ACEAB0FC6A0A28D"),
            new BlockCipherVectorTest(6, new BlowfishEngine(), new KeyParameter(Hex.Decode("7CA110454A1A6E57")), "01A1D6D039776742", "59C68245EB05282B"),
            new BlockCipherVectorTest(7, new BlowfishEngine(), new KeyParameter(Hex.Decode("0131D9619DC1376E")), "5CD54CA83DEF57DA", "B1B8CC0B250F09A0")
        };

        public BlowfishTest()
			: base(tests, new BlowfishEngine(), new KeyParameter(new byte[16]))
		{
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }

		public static void Main(
			string[] args)
        {
            ITest test = new BlowfishTest();
            ITestResult result = test.Perform();

            Console.WriteLine(result);
        }
    }
}
