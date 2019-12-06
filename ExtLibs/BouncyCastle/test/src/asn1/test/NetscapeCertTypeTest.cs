using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class NetscapeCertTypeTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "NetscapeCertType"; }
		}

		public override void PerformTest()
		{
			BitStringConstantTester.testFlagValueCorrect(0, NetscapeCertType.SslClient);
			BitStringConstantTester.testFlagValueCorrect(1, NetscapeCertType.SslServer);
			BitStringConstantTester.testFlagValueCorrect(2, NetscapeCertType.Smime);
			BitStringConstantTester.testFlagValueCorrect(3, NetscapeCertType.ObjectSigning);
			BitStringConstantTester.testFlagValueCorrect(4, NetscapeCertType.Reserved);
			BitStringConstantTester.testFlagValueCorrect(5, NetscapeCertType.SslCA);
			BitStringConstantTester.testFlagValueCorrect(6, NetscapeCertType.SmimeCA);
			BitStringConstantTester.testFlagValueCorrect(7, NetscapeCertType.ObjectSigningCA);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new NetscapeCertTypeTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
