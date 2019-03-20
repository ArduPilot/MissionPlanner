using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class DeclarationOfMajorityUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "DeclarationOfMajority"; }
		}

		public override void PerformTest()
		{
			DerGeneralizedTime dateOfBirth = new DerGeneralizedTime("20070315173729Z");
			DeclarationOfMajority decl = new DeclarationOfMajority(dateOfBirth);

			checkConstruction(decl, DeclarationOfMajority.Choice.DateOfBirth, dateOfBirth, -1);

			decl = new DeclarationOfMajority(6);

			checkConstruction(decl, DeclarationOfMajority.Choice.NotYoungerThan, null, 6);

			decl = DeclarationOfMajority.GetInstance(null);

			if (decl != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				DeclarationOfMajority.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			DeclarationOfMajority			decl,
			DeclarationOfMajority.Choice	type,
			DerGeneralizedTime				dateOfBirth,
			int								notYoungerThan)
		{
			checkValues(decl, type, dateOfBirth, notYoungerThan);

			decl = DeclarationOfMajority.GetInstance(decl);

			checkValues(decl, type, dateOfBirth, notYoungerThan);

			Asn1InputStream aIn = new Asn1InputStream(decl.ToAsn1Object().GetEncoded());

			DerTaggedObject info = (DerTaggedObject) aIn.ReadObject();

			decl = DeclarationOfMajority.GetInstance(info);

			checkValues(decl, type, dateOfBirth, notYoungerThan);
		}

		private void checkValues(
			DeclarationOfMajority			decl,
			DeclarationOfMajority.Choice	type,
			DerGeneralizedTime				dateOfBirth,
			int								notYoungerThan)
		{
			checkMandatoryField("type", (int) type, (int) decl.Type);
			checkOptionalField("dateOfBirth", dateOfBirth, decl.DateOfBirth);
			if (notYoungerThan != -1 && notYoungerThan != decl.NotYoungerThan)
			{
				Fail("notYoungerThan mismatch");
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new DeclarationOfMajorityUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
