using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class KeyUsageTest
		: SimpleTest
	{
		public override string Name
		{
			get
			{
				return "KeyUsage";
			}
		}

		public override void PerformTest()
		{
			BitStringConstantTester.testFlagValueCorrect(0, KeyUsage.DigitalSignature);
			BitStringConstantTester.testFlagValueCorrect(1, KeyUsage.NonRepudiation);
			BitStringConstantTester.testFlagValueCorrect(2, KeyUsage.KeyEncipherment);
			BitStringConstantTester.testFlagValueCorrect(3, KeyUsage.DataEncipherment);
			BitStringConstantTester.testFlagValueCorrect(4, KeyUsage.KeyAgreement);
			BitStringConstantTester.testFlagValueCorrect(5, KeyUsage.KeyCertSign);
			BitStringConstantTester.testFlagValueCorrect(6, KeyUsage.CrlSign);
			BitStringConstantTester.testFlagValueCorrect(7, KeyUsage.EncipherOnly);
			BitStringConstantTester.testFlagValueCorrect(8, KeyUsage.DecipherOnly);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new KeyUsageTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
