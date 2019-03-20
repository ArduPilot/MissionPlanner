using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.Ocsp;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class CertHashUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "CertHash"; }
		}

		public override void PerformTest()
		{
			AlgorithmIdentifier algId = new AlgorithmIdentifier(new DerObjectIdentifier("1.2.2.3"));
			byte[] digest = new byte[20];

			CertHash certID = new CertHash(algId, digest);

			checkConstruction(certID, algId, digest);

			certID = CertHash.GetInstance(null);

			if (certID != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				CertHash.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			CertHash			certHash,
			AlgorithmIdentifier	algId,
			byte[]				digest)
		{
			checkValues(certHash, algId, digest);

			certHash = CertHash.GetInstance(certHash);

			checkValues(certHash, algId, digest);

			Asn1InputStream aIn = new Asn1InputStream(certHash.ToAsn1Object().GetEncoded());

			Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

			certHash = CertHash.GetInstance(seq);

			checkValues(certHash, algId, digest);
		}

		private void checkValues(
			CertHash certHash,
			AlgorithmIdentifier algId,
			byte[] digest)
		{
			checkMandatoryField("algorithmHash", algId, certHash.HashAlgorithm);

			checkMandatoryField("certificateHash", digest, certHash.CertificateHash);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new CertHashUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
