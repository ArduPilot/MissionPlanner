using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509.SigI;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class NameOrPseudonymUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "NameOrPseudonym"; }
		}

		public override void PerformTest()
		{
			string pseudonym = "pseudonym";
			DirectoryString surname = new DirectoryString("surname");
			Asn1Sequence givenName = new DerSequence(new DirectoryString("givenName"));

			NameOrPseudonym id = new NameOrPseudonym(pseudonym);

			checkConstruction(id, pseudonym, null, null);

			id = new NameOrPseudonym(surname, givenName);

			checkConstruction(id, null, surname, givenName);

			id = NameOrPseudonym.GetInstance(null);

			if (id != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				NameOrPseudonym.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			NameOrPseudonym	id,
			string			pseudonym,
			DirectoryString	surname,
			Asn1Sequence	givenName)
		{
			checkValues(id, pseudonym, surname, givenName);

			id = NameOrPseudonym.GetInstance(id);

			checkValues(id, pseudonym, surname, givenName);

			Asn1InputStream aIn = new Asn1InputStream(id.ToAsn1Object().GetEncoded());

			if (surname != null)
			{
				Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

				id = NameOrPseudonym.GetInstance(seq);
			}
			else
			{
				IAsn1String s = (IAsn1String) aIn.ReadObject();

				id = NameOrPseudonym.GetInstance(s);
			}

			checkValues(id, pseudonym, surname, givenName);
		}

		private void checkValues(
			NameOrPseudonym	id,
			string			pseudonym,
			DirectoryString	surname,
			Asn1Sequence	givenName)
		{

			if (surname != null)
			{
				checkMandatoryField("surname", surname, id.Surname);
				checkMandatoryField("givenName", givenName, new DerSequence(id.GetGivenName()[0]));
			}
			else
			{
				checkOptionalField("pseudonym", new DirectoryString(pseudonym), id.Pseudonym);
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new NameOrPseudonymUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
