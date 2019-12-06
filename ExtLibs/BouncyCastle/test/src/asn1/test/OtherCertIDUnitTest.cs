using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class OtherCertIDUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "OtherCertID"; }
		}

		public override void PerformTest()
		{
			AlgorithmIdentifier algId = new AlgorithmIdentifier(new DerObjectIdentifier("1.2.2.3"));
			byte[] digest = new byte[20];
			OtherHash otherHash = new OtherHash(new OtherHashAlgAndValue(algId, digest));
			IssuerSerial issuerSerial = new IssuerSerial(new GeneralNames(new GeneralName(new X509Name("CN=test"))), new DerInteger(1));

			OtherCertID certID = new OtherCertID(otherHash);

			checkConstruction(certID, algId, digest, null);

			certID = new OtherCertID(otherHash, issuerSerial);

			checkConstruction(certID, algId, digest, issuerSerial);

			certID = OtherCertID.GetInstance(null);

			if (certID != null)
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
			OtherCertID			certID,
			AlgorithmIdentifier	algId,
			byte[]				digest,
			IssuerSerial		issuerSerial)
		{
			checkValues(certID, algId, digest, issuerSerial);

			certID = OtherCertID.GetInstance(certID);

			checkValues(certID, algId, digest, issuerSerial);

			Asn1InputStream aIn = new Asn1InputStream(certID.ToAsn1Object().GetEncoded());

			Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

			certID = OtherCertID.GetInstance(seq);

			checkValues(certID, algId, digest, issuerSerial);
		}

		private void checkValues(
			OtherCertID			certID,
			AlgorithmIdentifier	algId,
			byte[]				digest,
			IssuerSerial		issuerSerial)
		{
			checkMandatoryField("hashAlgorithm", algId, certID.OtherCertHash.HashAlgorithm);
			checkMandatoryField("hashValue", digest, certID.OtherCertHash.GetHashValue());

			checkOptionalField("issuerSerial", issuerSerial, certID.IssuerSerial);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new OtherCertIDUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
