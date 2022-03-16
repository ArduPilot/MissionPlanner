using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Asn1.X500;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class RestrictionUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "Restriction"; }
		}

		public override void PerformTest()
		{
			DirectoryString res = new DirectoryString("test");
			Restriction restriction = new Restriction(res.GetString());

			checkConstruction(restriction, res);

			try
			{
				Restriction.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			Restriction		restriction,
			DirectoryString	res)
		{
			checkValues(restriction, res);

			restriction = Restriction.GetInstance(restriction);

			checkValues(restriction, res);

			Asn1InputStream aIn = new Asn1InputStream(restriction.ToAsn1Object().GetEncoded());

			IAsn1String str = (IAsn1String) aIn.ReadObject();

			restriction = Restriction.GetInstance(str);

			checkValues(restriction, res);
		}

		private void checkValues(
			Restriction		restriction,
			DirectoryString	res)
		{
			checkMandatoryField("restriction", res, restriction.RestrictionString);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new RestrictionUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
