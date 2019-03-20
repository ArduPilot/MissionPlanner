using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class ReasonFlagsTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "ReasonFlags"; }
		}

		public override void PerformTest()
		{
			BitStringConstantTester.testFlagValueCorrect(0, ReasonFlags.Unused);
			BitStringConstantTester.testFlagValueCorrect(1, ReasonFlags.KeyCompromise);
			BitStringConstantTester.testFlagValueCorrect(2, ReasonFlags.CACompromise);
			BitStringConstantTester.testFlagValueCorrect(3, ReasonFlags.AffiliationChanged);
			BitStringConstantTester.testFlagValueCorrect(4, ReasonFlags.Superseded);
			BitStringConstantTester.testFlagValueCorrect(5, ReasonFlags.CessationOfOperation);
			BitStringConstantTester.testFlagValueCorrect(6, ReasonFlags.CertificateHold);
			BitStringConstantTester.testFlagValueCorrect(7, ReasonFlags.PrivilegeWithdrawn);
			BitStringConstantTester.testFlagValueCorrect(8, ReasonFlags.AACompromise);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new ReasonFlagsTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
