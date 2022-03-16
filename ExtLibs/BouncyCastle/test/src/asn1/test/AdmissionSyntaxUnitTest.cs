using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class AdmissionSyntaxUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "AdmissionSyntax"; }
		}

		public override void PerformTest()
		{
			GeneralName name = new GeneralName(new X509Name("CN=hello world"));
			Asn1Sequence admissions = new DerSequence(
				new Admissions(name,
				new NamingAuthority(new DerObjectIdentifier("1.2.3"), "url", new DirectoryString("fred")),
				new ProfessionInfo[0]));
			AdmissionSyntax syntax = new AdmissionSyntax(name, admissions);

			checkConstruction(syntax, name, admissions);

			syntax = AdmissionSyntax.GetInstance(null);

			if (syntax != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				AdmissionSyntax.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			AdmissionSyntax	syntax,
			GeneralName		authority,
			Asn1Sequence	admissions)
		{
			checkValues(syntax, authority, admissions);

			syntax = AdmissionSyntax.GetInstance(syntax);

			checkValues(syntax, authority, admissions);

			Asn1InputStream aIn = new Asn1InputStream(syntax.ToAsn1Object().GetEncoded());

			Asn1Sequence info = (Asn1Sequence) aIn.ReadObject();

			syntax = AdmissionSyntax.GetInstance(info);

			checkValues(syntax, authority, admissions);
		}

		private void checkValues(
			AdmissionSyntax syntax,
			GeneralName     authority,
			Asn1Sequence    admissions)
		{
			checkMandatoryField("admissionAuthority", authority, syntax.AdmissionAuthority);

			Admissions[] adm = syntax.GetContentsOfAdmissions();

			if (adm.Length != 1 || !adm[0].Equals(admissions[0]))
			{
				Fail("admissions check failed");
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new AdmissionSyntaxUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
