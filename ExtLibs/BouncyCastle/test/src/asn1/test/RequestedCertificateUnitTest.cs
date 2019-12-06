using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class RequestedCertificateUnitTest
		: Asn1UnitTest
	{
		private static readonly byte[] certBytes = Base64.Decode(
			"MIIBWzCCAQYCARgwDQYJKoZIhvcNAQEEBQAwODELMAkGA1UEBhMCQVUxDDAKBgNV"
			+ "BAgTA1FMRDEbMBkGA1UEAxMSU1NMZWF5L3JzYSB0ZXN0IENBMB4XDTk1MDYxOTIz"
			+ "MzMxMloXDTk1MDcxNzIzMzMxMlowOjELMAkGA1UEBhMCQVUxDDAKBgNVBAgTA1FM"
			+ "RDEdMBsGA1UEAxMUU1NMZWF5L3JzYSB0ZXN0IGNlcnQwXDANBgkqhkiG9w0BAQEF"
			+ "AANLADBIAkEAqtt6qS5GTxVxGZYWa0/4u+IwHf7p2LNZbcPBp9/OfIcYAXBQn8hO"
			+ "/Re1uwLKXdCjIoaGs4DLdG88rkzfyK5dPQIDAQABMAwGCCqGSIb3DQIFBQADQQAE"
			+ "Wc7EcF8po2/ZO6kNCwK/ICH6DobgLekA5lSLr5EvuioZniZp5lFzAw4+YzPQ7XKJ"
			+ "zl9HYIMxATFyqSiD9jsx");

		public override string Name
		{
			get { return "RequestedCertificate"; }
		}

		public override void PerformTest()
		{
			RequestedCertificate.Choice type = RequestedCertificate.Choice.AttributeCertificate;
			X509CertificateStructure cert = X509CertificateStructure.GetInstance(
				Asn1Object.FromByteArray(certBytes));

			byte[] certOctets = new byte[20];
			RequestedCertificate requested = new RequestedCertificate(type, certOctets);

			checkConstruction(requested, type, certOctets, null);

			requested = new RequestedCertificate(cert);

			checkConstruction(requested, RequestedCertificate.Choice.Certificate, null, cert);

			requested = RequestedCertificate.GetInstance(null);

			if (requested != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				RequestedCertificate.GetInstance(new object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			RequestedCertificate		requested,
			RequestedCertificate.Choice	type,
			byte[]						certOctets,
			X509CertificateStructure	cert)
		{
			checkValues(requested, type, certOctets, cert);

			requested = RequestedCertificate.GetInstance(requested);

			checkValues(requested, type, certOctets, cert);

			Asn1InputStream aIn = new Asn1InputStream(requested.ToAsn1Object().GetEncoded());

			object obj = aIn.ReadObject();

			requested = RequestedCertificate.GetInstance(obj);

			checkValues(requested, type, certOctets, cert);
		}

		private void checkValues(
			RequestedCertificate		requested,
			RequestedCertificate.Choice	type,
			byte[]						certOctets,
			X509CertificateStructure	cert)
		{
			checkMandatoryField("certType", (int) type, (int) requested.Type);

			if (requested.Type == RequestedCertificate.Choice.Certificate)
			{
				checkMandatoryField("certificate", cert.GetEncoded(), requested.GetCertificateBytes());
			}
			else
			{
				checkMandatoryField("certificateOctets", certOctets, requested.GetCertificateBytes());
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new RequestedCertificateUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
