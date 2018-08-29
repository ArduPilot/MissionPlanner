using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class AdmissionsUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "Admissions"; }
		}

		public override void PerformTest()
		{
			GeneralName name = new GeneralName(new X509Name("CN=hello world"));
			NamingAuthority auth = new NamingAuthority(new DerObjectIdentifier("1.2.3"), "url", new DirectoryString("fred"));
			Admissions admissions = new Admissions(name, auth, new ProfessionInfo[0]);

			checkConstruction(admissions, name, auth);

			admissions = Admissions.GetInstance(null);

			if (admissions != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				Admissions.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			Admissions      admissions,
			GeneralName     name,
			NamingAuthority auth)
		{
			checkValues(admissions, name, auth);

			admissions = Admissions.GetInstance(admissions);

			checkValues(admissions, name, auth);

			Asn1InputStream aIn = new Asn1InputStream(admissions.ToAsn1Object().GetEncoded());

			Asn1Sequence info = (Asn1Sequence)aIn.ReadObject();

			admissions = Admissions.GetInstance(info);

			checkValues(admissions, name, auth);
		}

		private void checkValues(
			Admissions		admissions,
			GeneralName		name,
			NamingAuthority	auth)
		{
			checkMandatoryField("admissionAuthority", name, admissions.AdmissionAuthority);
			checkMandatoryField("namingAuthority", auth, admissions.NamingAuthority);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new AdmissionsUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
