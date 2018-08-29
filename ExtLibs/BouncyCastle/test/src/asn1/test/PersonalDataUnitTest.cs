using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509.SigI;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class PersonalDataUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "PersonalData"; }
		}

		public override void PerformTest()
		{
			NameOrPseudonym nameOrPseudonym = new NameOrPseudonym("pseudonym");
			BigInteger nameDistinguisher = BigInteger.ValueOf(10);
			DerGeneralizedTime dateOfBirth= new DerGeneralizedTime("20070315173729Z");
			DirectoryString placeOfBirth = new DirectoryString("placeOfBirth");
			string gender = "M";
			DirectoryString postalAddress = new DirectoryString("address");

			PersonalData data = new PersonalData(nameOrPseudonym, nameDistinguisher, dateOfBirth, placeOfBirth, gender, postalAddress);

			checkConstruction(data, nameOrPseudonym, nameDistinguisher, dateOfBirth, placeOfBirth, gender, postalAddress);

			data = new PersonalData(nameOrPseudonym, null, dateOfBirth, placeOfBirth, gender, postalAddress);

			checkConstruction(data, nameOrPseudonym, null, dateOfBirth, placeOfBirth, gender, postalAddress);

			data = new PersonalData(nameOrPseudonym, nameDistinguisher, null, placeOfBirth, gender, postalAddress);

			checkConstruction(data, nameOrPseudonym, nameDistinguisher, null, placeOfBirth, gender, postalAddress);

			data = new PersonalData(nameOrPseudonym, nameDistinguisher, dateOfBirth, null, gender, postalAddress);

			checkConstruction(data, nameOrPseudonym, nameDistinguisher, dateOfBirth, null, gender, postalAddress);

			data = new PersonalData(nameOrPseudonym, nameDistinguisher, dateOfBirth, placeOfBirth, null, postalAddress);

			checkConstruction(data, nameOrPseudonym, nameDistinguisher, dateOfBirth, placeOfBirth, null, postalAddress);

			data = new PersonalData(nameOrPseudonym, nameDistinguisher, dateOfBirth, placeOfBirth, gender, null);

			checkConstruction(data, nameOrPseudonym, nameDistinguisher, dateOfBirth, placeOfBirth, gender, null);

			data = PersonalData.GetInstance(null);

			if (data != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				PersonalData.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			PersonalData		data,
			NameOrPseudonym		nameOrPseudonym,
			BigInteger			nameDistinguisher,
			DerGeneralizedTime	dateOfBirth,
			DirectoryString		placeOfBirth,
			string				gender,
			DirectoryString		postalAddress)
		{
			checkValues(data, nameOrPseudonym, nameDistinguisher, dateOfBirth, placeOfBirth, gender, postalAddress);

			data = PersonalData.GetInstance(data);

			checkValues(data, nameOrPseudonym, nameDistinguisher, dateOfBirth, placeOfBirth, gender, postalAddress);

			Asn1InputStream aIn = new Asn1InputStream(data.ToAsn1Object().GetEncoded());

			Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

			data = PersonalData.GetInstance(seq);

			checkValues(data, nameOrPseudonym, nameDistinguisher, dateOfBirth, placeOfBirth, gender, postalAddress);
		}

		private void checkValues(
			PersonalData		data,
			NameOrPseudonym		nameOrPseudonym,
			BigInteger			nameDistinguisher,
			DerGeneralizedTime	dateOfBirth,
			DirectoryString		placeOfBirth,
			string				gender,
			DirectoryString		postalAddress)
		{
			checkMandatoryField("nameOrPseudonym", nameOrPseudonym, data.NameOrPseudonym);
			checkOptionalField("nameDistinguisher", nameDistinguisher, data.NameDistinguisher);
			checkOptionalField("dateOfBirth", dateOfBirth, data.DateOfBirth);
			checkOptionalField("placeOfBirth", placeOfBirth, data.PlaceOfBirth);
			checkOptionalField("gender", gender, data.Gender);
			checkOptionalField("postalAddress", postalAddress, data.PostalAddress);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new PersonalDataUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
