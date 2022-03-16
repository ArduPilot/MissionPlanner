using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
	[TestFixture]
	public class PgpParsingTest
		: SimpleTest
	{
		public override void PerformTest()
		{
            Stream fIn = SimpleTest.GetTestDataAsStream("openpgp.bigpub.asc");
            Stream keyIn = PgpUtilities.GetDecoderStream(fIn);
            PgpPublicKeyRingBundle pubRings = new PgpPublicKeyRingBundle(keyIn);
		}

        public override string Name
		{
			get { return "PgpParsingTest"; }
		}

        public static void Main(
			string[] args)
		{
			RunTest(new PgpParsingTest());
		}

        [Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
