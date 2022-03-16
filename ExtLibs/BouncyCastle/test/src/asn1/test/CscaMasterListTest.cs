using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Icao;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class CscaMasterListTest
        : SimpleTest
    {
		public override string Name
		{
			get { return "CscaMasterList"; }
		}

		public override void PerformTest() 
		{
			byte[] input = GetInput("masterlist-content.data");
			CscaMasterList parsedList = CscaMasterList.GetInstance(Asn1Object.FromByteArray(input));

			if (parsedList.GetCertStructs().Length != 3)
			{
				Fail("Cert structure parsing failed: incorrect length");
			}

			byte[] output = parsedList.GetEncoded();
			if (!AreEqual(input, output))
			{
				Fail("Encoding failed after parse");
			}
		}

		private byte[] GetInput(string name)
		{
			return Streams.ReadAll(SimpleTest.GetTestDataAsStream("asn1." + name));
		}

		public static void Main(
            string[] args)
        {
            RunTest(new CscaMasterListTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
        }
	}
}
