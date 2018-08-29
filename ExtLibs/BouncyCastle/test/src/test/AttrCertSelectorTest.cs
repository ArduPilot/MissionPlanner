using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Tests
{
	[TestFixture]
	public class AttrCertSelectorTest
		: SimpleTest
	{
		private static readonly RsaPrivateCrtKeyParameters RsaPrivateKeySpec = new RsaPrivateCrtKeyParameters(
			new BigInteger("b4a7e46170574f16a97082b22be58b6a2a629798419be12872a4bdba626cfae9900f76abfb12139dce5de56564fab2b6543165a040c606887420e33d91ed7ed7", 16),
			new BigInteger("11", 16),
			new BigInteger("9f66f6b05410cd503b2709e88115d55daced94d1a34d4e32bf824d0dde6028ae79c5f07b580f5dce240d7111f7ddb130a7945cd7d957d1920994da389f490c89", 16),
			new BigInteger("c0a0758cdf14256f78d4708c86becdead1b50ad4ad6c5c703e2168fbf37884cb", 16),
			new BigInteger("f01734d7960ea60070f1b06f2bb81bfac48ff192ae18451d5e56c734a5aab8a5", 16),
			new BigInteger("b54bb9edff22051d9ee60f9351a48591b6500a319429c069a3e335a1d6171391", 16),
			new BigInteger("d3d83daf2a0cecd3367ae6f8ae1aeb82e9ac2f816c6fc483533d8297dd7884cd", 16),
			new BigInteger("b8f52fc6f38593dabb661d3f50f8897f8106eee68b1bce78a95b132b4e5b5d19", 16));

		private static readonly byte[] holderCert = Base64.Decode(
			  "MIIGjTCCBXWgAwIBAgICAPswDQYJKoZIhvcNAQEEBQAwaTEdMBsGCSqGSIb3DQEJ"
			+ "ARYOaXJtaGVscEB2dC5lZHUxLjAsBgNVBAMTJVZpcmdpbmlhIFRlY2ggQ2VydGlm"
			+ "aWNhdGlvbiBBdXRob3JpdHkxCzAJBgNVBAoTAnZ0MQswCQYDVQQGEwJVUzAeFw0w"
			+ "MzAxMzExMzUyMTRaFw0wNDAxMzExMzUyMTRaMIGDMRswGQYJKoZIhvcNAQkBFgxz"
			+ "c2hhaEB2dC5lZHUxGzAZBgNVBAMTElN1bWl0IFNoYWggKHNzaGFoKTEbMBkGA1UE"
			+ "CxMSVmlyZ2luaWEgVGVjaCBVc2VyMRAwDgYDVQQLEwdDbGFzcyAxMQswCQYDVQQK"
			+ "EwJ2dDELMAkGA1UEBhMCVVMwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBAPDc"
			+ "scgSKmsEp0VegFkuitD5j5PUkDuzLjlfaYONt2SN8WeqU4j2qtlCnsipa128cyKS"
			+ "JzYe9duUdNxquh5BPIkMkHBw4jHoQA33tk0J/sydWdN74/AHPpPieK5GHwhU7GTG"
			+ "rCCS1PJRxjXqse79ExAlul+gjQwHeldAC+d4A6oZAgMBAAGjggOmMIIDojAMBgNV"
			+ "HRMBAf8EAjAAMBEGCWCGSAGG+EIBAQQEAwIFoDAOBgNVHQ8BAf8EBAMCA/gwHQYD"
			+ "VR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMEMB0GA1UdDgQWBBRUIoWAzlXbzBYE"
			+ "yVTjQFWyMMKo1jCBkwYDVR0jBIGLMIGIgBTgc3Fm+TGqKDhen+oKfbl+xVbj2KFt"
			+ "pGswaTEdMBsGCSqGSIb3DQEJARYOaXJtaGVscEB2dC5lZHUxLjAsBgNVBAMTJVZp"
			+ "cmdpbmlhIFRlY2ggQ2VydGlmaWNhdGlvbiBBdXRob3JpdHkxCzAJBgNVBAoTAnZ0"
			+ "MQswCQYDVQQGEwJVU4IBADCBiwYJYIZIAYb4QgENBH4WfFZpcmdpbmlhIFRlY2gg"
			+ "Q2VydGlmaWNhdGlvbiBBdXRob3JpdHkgZGlnaXRhbCBjZXJ0aWZpY2F0ZXMgYXJl"
			+ "IHN1YmplY3QgdG8gcG9saWNpZXMgbG9jYXRlZCBhdCBodHRwOi8vd3d3LnBraS52"
			+ "dC5lZHUvY2EvY3BzLy4wFwYDVR0RBBAwDoEMc3NoYWhAdnQuZWR1MBkGA1UdEgQS"
			+ "MBCBDmlybWhlbHBAdnQuZWR1MEMGCCsGAQUFBwEBBDcwNTAzBggrBgEFBQcwAoYn"
			+ "aHR0cDovL2JveDE3Ny5jYy52dC5lZHUvY2EvaXNzdWVycy5odG1sMEQGA1UdHwQ9"
			+ "MDswOaA3oDWGM2h0dHA6Ly9ib3gxNzcuY2MudnQuZWR1L2h0ZG9jcy1wdWJsaWMv"
			+ "Y3JsL2NhY3JsLmNybDBUBgNVHSAETTBLMA0GCysGAQQBtGgFAQEBMDoGCysGAQQB"
			+ "tGgFAQEBMCswKQYIKwYBBQUHAgEWHWh0dHA6Ly93d3cucGtpLnZ0LmVkdS9jYS9j"
			+ "cHMvMD8GCWCGSAGG+EIBBAQyFjBodHRwOi8vYm94MTc3LmNjLnZ0LmVkdS9jZ2kt"
			+ "cHVibGljL2NoZWNrX3Jldl9jYT8wPAYJYIZIAYb4QgEDBC8WLWh0dHA6Ly9ib3gx"
			+ "NzcuY2MudnQuZWR1L2NnaS1wdWJsaWMvY2hlY2tfcmV2PzBLBglghkgBhvhCAQcE"
			+ "PhY8aHR0cHM6Ly9ib3gxNzcuY2MudnQuZWR1L35PcGVuQ0E4LjAxMDYzMC9jZ2kt"
			+ "cHVibGljL3JlbmV3YWw/MCwGCWCGSAGG+EIBCAQfFh1odHRwOi8vd3d3LnBraS52"
			+ "dC5lZHUvY2EvY3BzLzANBgkqhkiG9w0BAQQFAAOCAQEAHJ2ls9yjpZVcu5DqiE67"
			+ "r7BfkdMnm7IOj2v8cd4EAlPp6OPBmjwDMwvKRBb/P733kLBqFNWXWKTpT008R0KB"
			+ "8kehbx4h0UPz9vp31zhGv169+5iReQUUQSIwTGNWGLzrT8kPdvxiSAvdAJxcbRBm"
			+ "KzDic5I8PoGe48kSCkPpT1oNmnivmcu5j1SMvlx0IS2BkFMksr0OHiAW1elSnE/N"
			+ "RuX2k73b3FucwVxB3NRo3vgoHPCTnh9r4qItAHdxFlF+pPtbw2oHESKRfMRfOIHz"
			+ "CLQWSIa6Tvg4NIV3RRJ0sbCObesyg08lymalQMdkXwtRn5eGE00SHWwEUjSXP2gR"
			+ "3g==");

		public override string Name
		{
			get { return "AttrCertSelector"; }
		}

		private IX509AttributeCertificate CreateAttrCert()
		{
//			CertificateFactory fact = CertificateFactory.getInstance("X.509", "BC");
//			X509Certificate iCert = (X509Certificate) fact
//				.generateCertificate(new ByteArrayInputStream(holderCert));
			X509Certificate iCert = new X509CertificateParser().ReadCertificate(holderCert);

			//
			// a sample key pair.
			//
			// RSAPublicKeySpec pubKeySpec = new RSAPublicKeySpec(
			// new BigInteger(
			// "b4a7e46170574f16a97082b22be58b6a2a629798419be12872a4bdba626cfae9900f76abfb12139dce5de56564fab2b6543165a040c606887420e33d91ed7ed7",
			// 16), new BigInteger("11", 16));

			//
			// set up the keys
			//
//			KeyFactory kFact = KeyFactory.getInstance("RSA", "BC");
//			PrivateKey privKey = kFact.generatePrivate(RsaPrivateKeySpec);
			AsymmetricKeyParameter privKey = RsaPrivateKeySpec;

			X509V2AttributeCertificateGenerator gen = new X509V2AttributeCertificateGenerator();

			// the actual attributes
			GeneralName roleName = new GeneralName(GeneralName.Rfc822Name, "DAU123456789@test.com");
			Asn1EncodableVector roleSyntax = new Asn1EncodableVector(roleName);

			// roleSyntax OID: 2.5.24.72
			X509Attribute attributes = new X509Attribute("2.5.24.72",
				new DerSequence(roleSyntax));

			gen.AddAttribute(attributes);
			gen.SetHolder(new AttributeCertificateHolder(PrincipalUtilities.GetSubjectX509Principal(iCert)));
			gen.SetIssuer(new AttributeCertificateIssuer(new X509Name("cn=test")));
			gen.SetNotBefore(DateTime.UtcNow.AddSeconds(-50));
			gen.SetNotAfter(DateTime.UtcNow.AddSeconds(50));
			gen.SetSerialNumber(BigInteger.One);
			gen.SetSignatureAlgorithm("SHA1WithRSAEncryption");

			Target targetName = new Target(
				Target.Choice.Name,
				new GeneralName(GeneralName.DnsName, "www.test.com"));

			Target targetGroup = new Target(
				Target.Choice.Group,
				new GeneralName(GeneralName.DirectoryName, "o=Test, ou=Test"));

			Target[] targets = new Target[]{ targetName, targetGroup };

			TargetInformation targetInformation = new TargetInformation(targets);
			gen.AddExtension(X509Extensions.TargetInformation.Id, true, targetInformation);

			return gen.Generate(privKey);
		}

		[Test]
		public void TestSelector()
		{
			IX509AttributeCertificate aCert = CreateAttrCert();
			X509AttrCertStoreSelector sel = new X509AttrCertStoreSelector();
			sel.AttributeCert = aCert;
			bool match = sel.Match(aCert);
			if (!match)
			{
				Fail("Selector does not match attribute certificate.");
			}
			sel.AttributeCert = null;
			match = sel.Match(aCert);
			if (!match)
			{
				Fail("Selector does not match attribute certificate.");
			}
			sel.Holder = aCert.Holder;
			match = sel.Match(aCert);
			if (!match)
			{
				Fail("Selector does not match attribute certificate holder.");
			}
			sel.Holder = null;
			sel.Issuer = aCert.Issuer;
			match = sel.Match(aCert);
			if (!match)
			{
				Fail("Selector does not match attribute certificate issuer.");
			}
			sel.Issuer = null;

//			CertificateFactory fact = CertificateFactory.getInstance("X.509", "BC");
//			X509Certificate iCert = (X509Certificate) fact.generateCertificate(
//				new ByteArrayInputStream(holderCert));
			X509Certificate iCert = new X509CertificateParser().ReadCertificate(holderCert);
			match = aCert.Holder.Match(iCert);
			if (!match)
			{
				Fail("Issuer holder does not match signing certificate of attribute certificate.");
			}

			sel.SerialNumber = aCert.SerialNumber;
			match = sel.Match(aCert);
			if (!match)
			{
				Fail("Selector does not match attribute certificate serial number.");
			}

			sel.AttributeCertificateValid = new DateTimeObject(DateTime.UtcNow);
			match = sel.Match(aCert);
			if (!match)
			{
				Fail("Selector does not match attribute certificate time.");
			}

			sel.AddTargetName(new GeneralName(2, "www.test.com"));
			match = sel.Match(aCert);
			if (!match)
			{
				Fail("Selector does not match attribute certificate target name.");
			}
			sel.SetTargetNames(null);
			sel.AddTargetGroup(new GeneralName(4, "o=Test, ou=Test"));
			match = sel.Match(aCert);
			if (!match)
			{
				Fail("Selector does not match attribute certificate target group.");
			}
			sel.SetTargetGroups(null);
		}

		public override void PerformTest()
		{
			TestSelector();
		}

		public static void Main(
			string[] args)
		{
			RunTest(new AttrCertSelectorTest());
		}
	}
}
