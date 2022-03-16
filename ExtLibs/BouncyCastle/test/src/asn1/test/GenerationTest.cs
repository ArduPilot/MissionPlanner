using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class GenerationTest
        :	SimpleTest
    {
        private static readonly byte[] v1Cert = Base64.Decode(
			"MIGtAgEBMA0GCSqGSIb3DQEBBAUAMCUxCzAJBgNVBAMMAkFVMRYwFAYDVQQKDA1Cb"
			+ "3VuY3kgQ2FzdGxlMB4XDTcwMDEwMTAwMDAwMVoXDTcwMDEwMTAwMDAxMlowNjELMA"
			+ "kGA1UEAwwCQVUxFjAUBgNVBAoMDUJvdW5jeSBDYXN0bGUxDzANBgNVBAsMBlRlc3Q"
			+ "gMTAaMA0GCSqGSIb3DQEBAQUAAwkAMAYCAQECAQI=");

		private static readonly byte[] v3Cert = Base64.Decode(
			"MIIBSKADAgECAgECMA0GCSqGSIb3DQEBBAUAMCUxCzAJBgNVBAMMAkFVMRYwFAYD"
			+ "VQQKDA1Cb3VuY3kgQ2FzdGxlMB4XDTcwMDEwMTAwMDAwMVoXDTcwMDEwMTAwMDAw"
			+ "MlowNjELMAkGA1UEAwwCQVUxFjAUBgNVBAoMDUJvdW5jeSBDYXN0bGUxDzANBgNV"
			+ "BAsMBlRlc3QgMjAYMBAGBisOBwIBATAGAgEBAgECAwQAAgEDo4GVMIGSMGEGA1Ud"
			+ "IwEB/wRXMFWAFDZPdpHPzKi7o8EJokkQU2uqCHRRoTqkODA2MQswCQYDVQQDDAJB"
			+ "VTEWMBQGA1UECgwNQm91bmN5IENhc3RsZTEPMA0GA1UECwwGVGVzdCAyggECMCAG"
			+ "A1UdDgEB/wQWBBQ2T3aRz8you6PBCaJJEFNrqgh0UTALBgNVHQ8EBAMCBBA=");

		private static readonly byte[] v3CertNullSubject = Base64.Decode(
			"MIHGoAMCAQICAQIwDQYJKoZIhvcNAQEEBQAwJTELMAkGA1UEAwwCQVUxFjAUBgNVB"
			+ "AoMDUJvdW5jeSBDYXN0bGUwHhcNNzAwMTAxMDAwMDAxWhcNNzAwMTAxMDAwMDAyWj"
			+ "AAMBgwEAYGKw4HAgEBMAYCAQECAQIDBAACAQOjSjBIMEYGA1UdEQEB/wQ8MDqkODA"
			+ "2MQswCQYDVQQDDAJBVTEWMBQGA1UECgwNQm91bmN5IENhc3RsZTEPMA0GA1UECwwG"
			+ "VGVzdCAy");

		private static readonly byte[] v2CertList = Base64.Decode(
			"MIIBRQIBATANBgkqhkiG9w0BAQUFADAlMQswCQYDVQQDDAJBVTEWMBQGA1UECgwN"
			+ "Qm91bmN5IENhc3RsZRcNNzAwMTAxMDAwMDAwWhcNNzAwMTAxMDAwMDAyWjAkMCIC"
			+ "AQEXDTcwMDEwMTAwMDAwMVowDjAMBgNVHRUEBQoDAIAAoIHFMIHCMGEGA1UdIwEB"
			+ "/wRXMFWAFDZPdpHPzKi7o8EJokkQU2uqCHRRoTqkODA2MQswCQYDVQQDDAJBVTEW"
			+ "MBQGA1UECgwNQm91bmN5IENhc3RsZTEPMA0GA1UECwwGVGVzdCAyggECMEMGA1Ud"
			+ "EgQ8MDqkODA2MQswCQYDVQQDDAJBVTEWMBQGA1UECgwNQm91bmN5IENhc3RsZTEP"
			+ "MA0GA1UECwwGVGVzdCAzMAoGA1UdFAQDAgEBMAwGA1UdHAEB/wQCMAA=");

        private void TbsV1CertGenerate()
        {
			V1TbsCertificateGenerator gen = new V1TbsCertificateGenerator();
            DateTime startDate = MakeUtcDateTime(1970, 1, 1, 0, 0, 1);
            DateTime endDate = MakeUtcDateTime(1970, 1, 1, 0, 0, 12);

            gen.SetSerialNumber(new DerInteger(1));

            gen.SetStartDate(new Time(startDate));
            gen.SetEndDate(new Time(endDate));

            gen.SetIssuer(new X509Name("CN=AU,O=Bouncy Castle"));
            gen.SetSubject(new X509Name("CN=AU,O=Bouncy Castle,OU=Test 1"));

            gen.SetSignature(new AlgorithmIdentifier(PkcsObjectIdentifiers.MD5WithRsaEncryption, DerNull.Instance));

            SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(new AlgorithmIdentifier(PkcsObjectIdentifiers.RsaEncryption, DerNull.Instance),
                new RsaPublicKeyStructure(BigInteger.One, BigInteger.Two));

            gen.SetSubjectPublicKeyInfo(info);

            TbsCertificateStructure tbs = gen.GenerateTbsCertificate();

			if (!Arrays.AreEqual(tbs.GetEncoded(), v1Cert))
            {
                Fail("failed v1 cert generation");
            }

            //
            // read back test
            //
            Asn1InputStream aIn = new Asn1InputStream(v1Cert);
            Asn1Object o = aIn.ReadObject();

            if (!Arrays.AreEqual(o.GetEncoded(), v1Cert))
            {
                Fail("failed v1 cert read back test");
            }
        }

		private AuthorityKeyIdentifier CreateAuthorityKeyId(
            SubjectPublicKeyInfo	info,
            X509Name				name,
            int						sNumber)
        {
            GeneralName genName = new GeneralName(name);

			return new AuthorityKeyIdentifier(
				info,
				GeneralNames.GetInstance(new DerSequence(genName)),
				BigInteger.ValueOf(sNumber));
        }

		private void TbsV3CertGenerate()
        {
			V3TbsCertificateGenerator gen = new V3TbsCertificateGenerator();
            DateTime startDate = MakeUtcDateTime(1970, 1, 1, 0, 0, 1);
            DateTime endDate = MakeUtcDateTime(1970, 1, 1, 0, 0, 2);

			gen.SetSerialNumber(new DerInteger(2));

			gen.SetStartDate(new Time(startDate));
			gen.SetEndDate(new Time(endDate));

			gen.SetIssuer(new X509Name("CN=AU,O=Bouncy Castle"));
			gen.SetSubject(new X509Name("CN=AU,O=Bouncy Castle,OU=Test 2"));

			gen.SetSignature(new AlgorithmIdentifier(PkcsObjectIdentifiers.MD5WithRsaEncryption, DerNull.Instance));

			SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(
				new AlgorithmIdentifier(
					OiwObjectIdentifiers.ElGamalAlgorithm,
					new ElGamalParameter(BigInteger.One, BigInteger.Two)),
				new DerInteger(3));

			gen.SetSubjectPublicKeyInfo(info);

			//
			// add extensions
			//
			IList order = new ArrayList();
			IDictionary extensions = new Hashtable();

			order.Add(X509Extensions.AuthorityKeyIdentifier);
			order.Add(X509Extensions.SubjectKeyIdentifier);
			order.Add(X509Extensions.KeyUsage);

			extensions.Add(X509Extensions.AuthorityKeyIdentifier, new X509Extension(true, new DerOctetString(CreateAuthorityKeyId(info, new X509Name("CN=AU,O=Bouncy Castle,OU=Test 2"), 2))));
			extensions.Add(X509Extensions.SubjectKeyIdentifier, new X509Extension(true, new DerOctetString(new SubjectKeyIdentifier(info))));
			extensions.Add(X509Extensions.KeyUsage, new X509Extension(false, new DerOctetString(new KeyUsage(KeyUsage.DataEncipherment))));

			X509Extensions ex = new X509Extensions(order, extensions);

			gen.SetExtensions(ex);

			TbsCertificateStructure tbs = gen.GenerateTbsCertificate();

			if (!Arrays.AreEqual(tbs.GetEncoded(), v3Cert))
			{
				Fail("failed v3 cert generation");
			}

			//
			// read back test
			//
			Asn1Object o = Asn1Object.FromByteArray(v3Cert);

			if (!Arrays.AreEqual(o.GetEncoded(), v3Cert))
			{
				Fail("failed v3 cert read back test");
			}
        }

		private void TbsV3CertGenWithNullSubject()
		{
			V3TbsCertificateGenerator gen = new V3TbsCertificateGenerator();
            DateTime startDate = MakeUtcDateTime(1970, 1, 1, 0, 0, 1);
            DateTime endDate = MakeUtcDateTime(1970, 1, 1, 0, 0, 2);

			gen.SetSerialNumber(new DerInteger(2));

			gen.SetStartDate(new Time(startDate));
			gen.SetEndDate(new Time(endDate));

			gen.SetIssuer(new X509Name("CN=AU,O=Bouncy Castle"));

			gen.SetSignature(new AlgorithmIdentifier(PkcsObjectIdentifiers.MD5WithRsaEncryption, DerNull.Instance));

			SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(
				new AlgorithmIdentifier(OiwObjectIdentifiers.ElGamalAlgorithm,
					new ElGamalParameter(BigInteger.One, BigInteger.Two)),
				new DerInteger(3));

			gen.SetSubjectPublicKeyInfo(info);

			try
			{
				gen.GenerateTbsCertificate();
				Fail("null subject not caught!");
			}
			catch (InvalidOperationException e)
			{
				if (!e.Message.Equals("not all mandatory fields set in V3 TBScertificate generator"))
				{
					Fail("unexpected exception", e);
				}
			}

			//
			// add extensions
			//
			IList order = new ArrayList();
			IDictionary extensions = new Hashtable();

			order.Add(X509Extensions.SubjectAlternativeName);

			extensions.Add(
				X509Extensions.SubjectAlternativeName,
				new X509Extension(
					true,
					new DerOctetString(
						new GeneralNames(
							new GeneralName(
								new X509Name("CN=AU,O=Bouncy Castle,OU=Test 2"))))));

			X509Extensions ex = new X509Extensions(order, extensions);

			gen.SetExtensions(ex);

			TbsCertificateStructure tbs = gen.GenerateTbsCertificate();

			if (!Arrays.AreEqual(tbs.GetEncoded(), v3CertNullSubject))
			{
				Fail("failed v3 null sub cert generation");
			}

			//
			// read back test
			//
			Asn1Object o = Asn1Object.FromByteArray(v3CertNullSubject);

			if (!Arrays.AreEqual(o.GetEncoded(), v3CertNullSubject))
			{
				Fail("failed v3 null sub cert read back test");
			}
		}

		private void TbsV2CertListGenerate()
        {
            V2TbsCertListGenerator gen = new V2TbsCertListGenerator();

            gen.SetIssuer(new X509Name("CN=AU,O=Bouncy Castle"));

            gen.AddCrlEntry(new DerInteger(1), new Time(MakeUtcDateTime(1970, 1, 1, 0, 0, 1)), ReasonFlags.AACompromise);

            gen.SetNextUpdate(new Time(MakeUtcDateTime(1970, 1, 1, 0, 0, 2)));

            gen.SetThisUpdate(new Time(MakeUtcDateTime(1970, 1, 1, 0, 0, 0, 500)));

            gen.SetSignature(new AlgorithmIdentifier(PkcsObjectIdentifiers.Sha1WithRsaEncryption, DerNull.Instance));

            //
            // extensions
            //
            IList order = new ArrayList();
            IDictionary extensions = new Hashtable();
            SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(
				new AlgorithmIdentifier(
					OiwObjectIdentifiers.ElGamalAlgorithm,
					new ElGamalParameter(BigInteger.One, BigInteger.Two)),
				new DerInteger(3));

			order.Add(X509Extensions.AuthorityKeyIdentifier);
            order.Add(X509Extensions.IssuerAlternativeName);
            order.Add(X509Extensions.CrlNumber);
            order.Add(X509Extensions.IssuingDistributionPoint);

            extensions.Add(X509Extensions.AuthorityKeyIdentifier, new X509Extension(true, new DerOctetString(CreateAuthorityKeyId(info, new X509Name("CN=AU,O=Bouncy Castle,OU=Test 2"), 2))));
            extensions.Add(X509Extensions.IssuerAlternativeName, new X509Extension(false, new DerOctetString(GeneralNames.GetInstance(new DerSequence(new GeneralName(new X509Name("CN=AU,O=Bouncy Castle,OU=Test 3")))))));
            extensions.Add(X509Extensions.CrlNumber, new X509Extension(false, new DerOctetString(new DerInteger(1))));
            extensions.Add(X509Extensions.IssuingDistributionPoint, new X509Extension(true, new DerOctetString(IssuingDistributionPoint.GetInstance(DerSequence.Empty))));

            X509Extensions ex = new X509Extensions(order, extensions);

            gen.SetExtensions(ex);

            TbsCertificateList tbs = gen.GenerateTbsCertList();

            if (!Arrays.AreEqual(tbs.GetEncoded(), v2CertList))
            {
                Fail("failed v2 cert list generation");
            }

            //
            // read back test
            //
            Asn1InputStream aIn = new Asn1InputStream(v2CertList);
            Asn1Object o = aIn.ReadObject();

            if (!Arrays.AreEqual(o.GetEncoded(), v2CertList))
            {
                Fail("failed v2 cert list read back test");
            }
        }

		public override void PerformTest()
        {
            TbsV1CertGenerate();
            TbsV3CertGenerate();
			TbsV3CertGenWithNullSubject();
            TbsV2CertListGenerate();
        }

		public override string Name
		{
			get { return "Generation"; }
		}

        public static void Main(
            string[] args)
        {
			RunTest(new GenerationTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

			Assert.AreEqual(resultText, Name + ": Okay", resultText);
        }
    }
}
