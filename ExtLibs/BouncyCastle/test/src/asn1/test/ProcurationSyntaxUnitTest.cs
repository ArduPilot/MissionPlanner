using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class ProcurationSyntaxUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "ProcurationSyntax"; }
		}

		public override void PerformTest()
		{
			string country = "AU";
			DirectoryString typeOfSubstitution = new DirectoryString("substitution");
			GeneralName thirdPerson = new GeneralName(new X509Name("CN=thirdPerson"));
			IssuerSerial certRef = new IssuerSerial(new GeneralNames(new GeneralName(new X509Name("CN=test"))), new DerInteger(1));

			ProcurationSyntax procuration = new ProcurationSyntax(country, typeOfSubstitution, thirdPerson);

			checkConstruction(procuration, country, typeOfSubstitution, thirdPerson, null);

			procuration = new ProcurationSyntax(country, typeOfSubstitution, certRef);

			checkConstruction(procuration, country, typeOfSubstitution, null, certRef);

			procuration = new ProcurationSyntax(null, typeOfSubstitution, certRef);

			checkConstruction(procuration, null, typeOfSubstitution, null, certRef);

			procuration = new ProcurationSyntax(country, null, certRef);

			checkConstruction(procuration, country, null, null, certRef);

			procuration = ProcurationSyntax.GetInstance(null);

			if (procuration != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				ProcurationSyntax.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			ProcurationSyntax	procuration,
			string				country,
			DirectoryString		typeOfSubstitution,
			GeneralName			thirdPerson,
			IssuerSerial		certRef)
		{
			checkValues(procuration, country, typeOfSubstitution, thirdPerson, certRef);

			procuration = ProcurationSyntax.GetInstance(procuration);

			checkValues(procuration, country, typeOfSubstitution, thirdPerson, certRef);

			Asn1InputStream aIn = new Asn1InputStream(procuration.ToAsn1Object().GetEncoded());

			Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

			procuration = ProcurationSyntax.GetInstance(seq);

			checkValues(procuration, country, typeOfSubstitution, thirdPerson, certRef);
		}

		private void checkValues(
			ProcurationSyntax procuration,
			string country,
			DirectoryString  typeOfSubstitution,
			GeneralName thirdPerson,
			IssuerSerial certRef)
		{
			checkOptionalField("country", country, procuration.Country);
			checkOptionalField("typeOfSubstitution", typeOfSubstitution, procuration.TypeOfSubstitution);
			checkOptionalField("thirdPerson", thirdPerson, procuration.ThirdPerson);
			checkOptionalField("certRef", certRef, procuration.CertRef);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new ProcurationSyntaxUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
