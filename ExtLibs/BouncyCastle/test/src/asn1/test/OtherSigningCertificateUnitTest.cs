using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class OtherSigningCertificateUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "OtherSigningCertificate"; }
		}

		public override void PerformTest()
		{
			AlgorithmIdentifier algId = new AlgorithmIdentifier(new DerObjectIdentifier("1.2.2.3"));
			byte[] digest = new byte[20];
			OtherHash otherHash = new OtherHash(
				new OtherHashAlgAndValue(algId, digest));
			OtherCertID otherCertID = new OtherCertID(otherHash);

			OtherSigningCertificate otherCert = new OtherSigningCertificate(otherCertID);

			checkConstruction(otherCert, otherCertID);

			otherCert = OtherSigningCertificate.GetInstance(null);

			if (otherCert != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				OtherCertID.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			OtherSigningCertificate	otherCert,
			OtherCertID				otherCertID)
		{
			checkValues(otherCert, otherCertID);

			otherCert = OtherSigningCertificate.GetInstance(otherCert);

			checkValues(otherCert, otherCertID);

			Asn1InputStream aIn = new Asn1InputStream(otherCert.ToAsn1Object().GetEncoded());

			Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

			otherCert = OtherSigningCertificate.GetInstance(seq);

			checkValues(otherCert, otherCertID);
		}

		private void checkValues(
			OtherSigningCertificate	otherCert,
			OtherCertID				otherCertID)
		{
			if (otherCert.GetCerts().Length != 1)
			{
				Fail("GetCerts() length wrong");
			}
			checkMandatoryField("GetCerts()[0]", otherCertID, otherCert.GetCerts()[0]);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new OtherSigningCertificateUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
