using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Tests
{
	[TestFixture]
	public class AttrCertTest
		: SimpleTest
	{
		private static readonly RsaPrivateCrtKeyParameters RSA_PRIVATE_KEY_SPEC = new RsaPrivateCrtKeyParameters(
			new BigInteger("b4a7e46170574f16a97082b22be58b6a2a629798419be12872a4bdba626cfae9900f76abfb12139dce5de56564fab2b6543165a040c606887420e33d91ed7ed7", 16),
			new BigInteger("11", 16),
			new BigInteger("9f66f6b05410cd503b2709e88115d55daced94d1a34d4e32bf824d0dde6028ae79c5f07b580f5dce240d7111f7ddb130a7945cd7d957d1920994da389f490c89", 16),
			new BigInteger("c0a0758cdf14256f78d4708c86becdead1b50ad4ad6c5c703e2168fbf37884cb", 16),
			new BigInteger("f01734d7960ea60070f1b06f2bb81bfac48ff192ae18451d5e56c734a5aab8a5", 16),
			new BigInteger("b54bb9edff22051d9ee60f9351a48591b6500a319429c069a3e335a1d6171391", 16),
			new BigInteger("d3d83daf2a0cecd3367ae6f8ae1aeb82e9ac2f816c6fc483533d8297dd7884cd", 16),
			new BigInteger("b8f52fc6f38593dabb661d3f50f8897f8106eee68b1bce78a95b132b4e5b5d19", 16));

		internal static readonly byte[] attrCert = Base64.Decode(
			"MIIHQDCCBqkCAQEwgZChgY2kgYowgYcxHDAaBgkqhkiG9w0BCQEWDW1sb3JjaEB2"
			+ "dC5lZHUxHjAcBgNVBAMTFU1hcmt1cyBMb3JjaCAobWxvcmNoKTEbMBkGA1UECxMS"
			+ "VmlyZ2luaWEgVGVjaCBVc2VyMRAwDgYDVQQLEwdDbGFzcyAyMQswCQYDVQQKEwJ2"
			+ "dDELMAkGA1UEBhMCVVMwgYmkgYYwgYMxGzAZBgkqhkiG9w0BCQEWDHNzaGFoQHZ0"
			+ "LmVkdTEbMBkGA1UEAxMSU3VtaXQgU2hhaCAoc3NoYWgpMRswGQYDVQQLExJWaXJn"
			+ "aW5pYSBUZWNoIFVzZXIxEDAOBgNVBAsTB0NsYXNzIDExCzAJBgNVBAoTAnZ0MQsw"
			+ "CQYDVQQGEwJVUzANBgkqhkiG9w0BAQQFAAIBBTAiGA8yMDAzMDcxODE2MDgwMloY"
			+ "DzIwMDMwNzI1MTYwODAyWjCCBU0wggVJBgorBgEEAbRoCAEBMYIFORaCBTU8UnVs"
			+ "ZSBSdWxlSWQ9IkZpbGUtUHJpdmlsZWdlLVJ1bGUiIEVmZmVjdD0iUGVybWl0Ij4K"
			+ "IDxUYXJnZXQ+CiAgPFN1YmplY3RzPgogICA8U3ViamVjdD4KICAgIDxTdWJqZWN0"
			+ "TWF0Y2ggTWF0Y2hJZD0idXJuOm9hc2lzOm5hbWVzOnRjOnhhY21sOjEuMDpmdW5j"
			+ "dGlvbjpzdHJpbmctZXF1YWwiPgogICAgIDxBdHRyaWJ1dGVWYWx1ZSBEYXRhVHlw"
			+ "ZT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS9YTUxTY2hlbWEjc3RyaW5nIj4KICAg"
			+ "ICAgIENOPU1hcmt1cyBMb3JjaDwvQXR0cmlidXRlVmFsdWU+CiAgICAgPFN1Ympl"
			+ "Y3RBdHRyaWJ1dGVEZXNpZ25hdG9yIEF0dHJpYnV0ZUlkPSJ1cm46b2FzaXM6bmFt"
			+ "ZXM6dGM6eGFjbWw6MS4wOnN1YmplY3Q6c3ViamVjdC1pZCIgRGF0YVR5cGU9Imh0"
			+ "dHA6Ly93d3cudzMub3JnLzIwMDEvWE1MU2NoZW1hI3N0cmluZyIgLz4gCiAgICA8"
			+ "L1N1YmplY3RNYXRjaD4KICAgPC9TdWJqZWN0PgogIDwvU3ViamVjdHM+CiAgPFJl"
			+ "c291cmNlcz4KICAgPFJlc291cmNlPgogICAgPFJlc291cmNlTWF0Y2ggTWF0Y2hJ"
			+ "ZD0idXJuOm9hc2lzOm5hbWVzOnRjOnhhY21sOjEuMDpmdW5jdGlvbjpzdHJpbmct"
			+ "ZXF1YWwiPgogICAgIDxBdHRyaWJ1dGVWYWx1ZSBEYXRhVHlwZT0iaHR0cDovL3d3"
			+ "dy53My5vcmcvMjAwMS9YTUxTY2hlbWEjYW55VVJJIj4KICAgICAgaHR0cDovL3p1"
			+ "bmkuY3MudnQuZWR1PC9BdHRyaWJ1dGVWYWx1ZT4KICAgICA8UmVzb3VyY2VBdHRy"
			+ "aWJ1dGVEZXNpZ25hdG9yIEF0dHJpYnV0ZUlkPSJ1cm46b2FzaXM6bmFtZXM6dGM6"
			+ "eGFjbWw6MS4wOnJlc291cmNlOnJlc291cmNlLWlkIiBEYXRhVHlwZT0iaHR0cDov"
			+ "L3d3dy53My5vcmcvMjAwMS9YTUxTY2hlbWEjYW55VVJJIiAvPiAKICAgIDwvUmVz"
			+ "b3VyY2VNYXRjaD4KICAgPC9SZXNvdXJjZT4KICA8L1Jlc291cmNlcz4KICA8QWN0"
			+ "aW9ucz4KICAgPEFjdGlvbj4KICAgIDxBY3Rpb25NYXRjaCBNYXRjaElkPSJ1cm46"
			+ "b2FzaXM6bmFtZXM6dGM6eGFjbWw6MS4wOmZ1bmN0aW9uOnN0cmluZy1lcXVhbCI+"
			+ "CiAgICAgPEF0dHJpYnV0ZVZhbHVlIERhdGFUeXBlPSJodHRwOi8vd3d3LnczLm9y"
			+ "Zy8yMDAxL1hNTFNjaGVtYSNzdHJpbmciPgpEZWxlZ2F0ZSBBY2Nlc3MgICAgIDwv"
			+ "QXR0cmlidXRlVmFsdWU+CgkgIDxBY3Rpb25BdHRyaWJ1dGVEZXNpZ25hdG9yIEF0"
			+ "dHJpYnV0ZUlkPSJ1cm46b2FzaXM6bmFtZXM6dGM6eGFjbWw6MS4wOmFjdGlvbjph"
			+ "Y3Rpb24taWQiIERhdGFUeXBlPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxL1hNTFNj"
			+ "aGVtYSNzdHJpbmciIC8+IAogICAgPC9BY3Rpb25NYXRjaD4KICAgPC9BY3Rpb24+"
			+ "CiAgPC9BY3Rpb25zPgogPC9UYXJnZXQ+CjwvUnVsZT4KMA0GCSqGSIb3DQEBBAUA"
			+ "A4GBAGiJSM48XsY90HlYxGmGVSmNR6ZW2As+bot3KAfiCIkUIOAqhcphBS23egTr"
			+ "6asYwy151HshbPNYz+Cgeqs45KkVzh7bL/0e1r8sDVIaaGIkjHK3CqBABnfSayr3"
			+ "Rd1yBoDdEv8Qb+3eEPH6ab9021AsLEnJ6LWTmybbOpMNZ3tv");

		internal static readonly byte[] signCert = Base64.Decode(
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

		internal static readonly byte[] certWithBaseCertificateID = Base64.Decode(
			"MIIBqzCCARQCAQEwSKBGMD6kPDA6MQswCQYDVQQGEwJJVDEOMAwGA1UEChMFVU5JVE4xDDAKBgNV"
			+ "BAsTA0RJVDENMAsGA1UEAxMEcm9vdAIEAVMVjqB6MHikdjB0MQswCQYDVQQGEwJBVTEoMCYGA1UE"
			+ "ChMfVGhlIExlZ2lvbiBvZiB0aGUgQm91bmN5IENhc3RsZTEjMCEGA1UECxMaQm91bmN5IFByaW1h"
			+ "cnkgQ2VydGlmaWNhdGUxFjAUBgNVBAMTDUJvdW5jeSBDYXN0bGUwDQYJKoZIhvcNAQEFBQACBQKW"
			+ "RhnHMCIYDzIwMDUxMjEyMTIwMDQyWhgPMjAwNTEyMTkxMjAxMzJaMA8wDQYDVRhIMQaBBGVWSVAw"
			+ "DQYJKoZIhvcNAQEFBQADgYEAUAVin9StDaA+InxtXq/av6rUQLI9p1X6louBcj4kYJnxRvTrHpsr"
			+ "N3+i9Uq/uk5lRdAqmPFvcmSbuE3TRAsjrXON5uFiBBKZ1AouLqcr8nHbwcdwjJ9TyUNO9I4hfpSH"
			+ "UHHXMtBKgp4MOkhhX8xTGyWg3hp23d3GaUeg/IYlXBI=");

		internal static readonly byte[] holderCertWithBaseCertificateID = Base64.Decode(
			"MIIBwDCCASmgAwIBAgIEAVMVjjANBgkqhkiG9w0BAQUFADA6MQswCQYDVQQGEwJJVDEOMAwGA1UE"
			+ "ChMFVU5JVE4xDDAKBgNVBAsTA0RJVDENMAsGA1UEAxMEcm9vdDAeFw0wNTExMTExMjAxMzJaFw0w"
			+ "NjA2MTYxMjAxMzJaMD4xCzAJBgNVBAYTAklUMQ4wDAYDVQQKEwVVTklUTjEMMAoGA1UECxMDRElU"
			+ "MREwDwYDVQQDEwhMdWNhQm9yejBaMA0GCSqGSIb3DQEBAQUAA0kAMEYCQQC0p+RhcFdPFqlwgrIr"
			+ "5YtqKmKXmEGb4ShypL26Ymz66ZAPdqv7EhOdzl3lZWT6srZUMWWgQMYGiHQg4z2R7X7XAgERoxUw"
			+ "EzARBglghkgBhvhCAQEEBAMCBDAwDQYJKoZIhvcNAQEFBQADgYEAsX50VPQQCWmHvPq9y9DeCpmS"
			+ "4szcpFAhpZyn6gYRwY9CRZVtmZKH8713XhkGDWcIEMcG0u3oTz3tdKgPU5uyIPrDEWr6w8ClUj4x"
			+ "5aVz5c2223+dVY7KES//JSB2bE/KCIchN3kAioQ4K8O3e0OL6oDVjsqKGw5bfahgKuSIk/Q=");


		public override string Name
		{
			get { return "AttrCertTest"; }
		}

		private void doTestCertWithBaseCertificateID()
		{
			IX509AttributeCertificate attrCert = new X509V2AttributeCertificate(certWithBaseCertificateID);
			X509CertificateParser fact = new X509CertificateParser();
			X509Certificate cert = fact.ReadCertificate(holderCertWithBaseCertificateID);

			AttributeCertificateHolder holder = attrCert.Holder;

			if (holder.GetEntityNames() != null)
			{
				Fail("entity names set when none expected");
			}

			if (!holder.SerialNumber.Equals(cert.SerialNumber))
			{
				Fail("holder serial number doesn't Match");
			}

			if (!holder.GetIssuer()[0].Equivalent(cert.IssuerDN))
			{
				Fail("holder issuer doesn't Match");
			}

			if (!holder.Match(cert))
			{
				Fail("holder not matching holder certificate");
			}

			if (!holder.Equals(holder.Clone()))
			{
				Fail("holder clone test failed");
			}

			if (!attrCert.Issuer.Equals(attrCert.Issuer.Clone()))
			{
				Fail("issuer clone test failed");
			}

			equalityAndHashCodeTest(attrCert, certWithBaseCertificateID);
		}

		private void equalityAndHashCodeTest(
			IX509AttributeCertificate	attrCert,
			byte[]						encoding)
		{
			if (!attrCert.Equals(attrCert))
			{
				Fail("same certificate not equal");
			}

			if (!attrCert.Holder.Equals(attrCert.Holder))
			{
				Fail("same holder not equal");
			}

			if (!attrCert.Issuer.Equals(attrCert.Issuer))
			{
				Fail("same issuer not equal");
			}

			if (attrCert.Holder.Equals(attrCert.Issuer))
			{
				Fail("wrong holder equal");
			}

			if (attrCert.Issuer.Equals(attrCert.Holder))
			{
				Fail("wrong issuer equal");
			}

			IX509AttributeCertificate attrCert2 = new X509V2AttributeCertificate(encoding);

			if (attrCert2.Holder.GetHashCode() != attrCert.Holder.GetHashCode())
			{
				Fail("holder hashCode test failed");
			}

			if (!attrCert2.Holder.Equals(attrCert.Holder))
			{
				Fail("holder Equals test failed");
			}

			if (attrCert2.Issuer.GetHashCode() != attrCert.Issuer.GetHashCode())
			{
				Fail("issuer hashCode test failed");
			}

			if (!attrCert2.Issuer.Equals(attrCert.Issuer))
			{
				Fail("issuer Equals test failed");
			}
		}

		private void doTestGenerateWithCert()
		{
			X509CertificateParser fact = new X509CertificateParser();
			X509Certificate iCert = fact.ReadCertificate(signCert);

			//
			// a sample key pair.
			//
			RsaKeyParameters pubKey = new RsaKeyParameters(
				false,
				new BigInteger("b4a7e46170574f16a97082b22be58b6a2a629798419be12872a4bdba626cfae9900f76abfb12139dce5de56564fab2b6543165a040c606887420e33d91ed7ed7", 16),
				new BigInteger("11", 16));

			//
			// set up the keys
			//
//			PrivateKey privKey;
//			PublicKey pubKey;
//
//			KeyFactory  kFact = KeyFactory.getInstance("RSA");
//
//			privKey = kFact.generatePrivate(RSA_PRIVATE_KEY_SPEC);
//			pubKey = kFact.generatePublic(pubKeySpec);
			AsymmetricKeyParameter privKey = RSA_PRIVATE_KEY_SPEC;

			X509V2AttributeCertificateGenerator gen = new X509V2AttributeCertificateGenerator();

			// the actual attributes
			GeneralName roleName = new GeneralName(GeneralName.Rfc822Name, "DAU123456789");

			// roleSyntax OID: 2.5.24.72
			X509Attribute attributes = new X509Attribute("2.5.24.72",
				new DerSequence(roleName));

			gen.AddAttribute(attributes);
			gen.SetHolder(new AttributeCertificateHolder(iCert));
			gen.SetIssuer(new AttributeCertificateIssuer(new X509Name("cn=test")));
			gen.SetNotBefore(DateTime.UtcNow.AddSeconds(-50));
			gen.SetNotAfter(DateTime.UtcNow.AddSeconds(50));
			gen.SetSerialNumber(BigInteger.One);
			gen.SetSignatureAlgorithm("SHA1WithRSAEncryption");

			IX509AttributeCertificate aCert = gen.Generate(privKey);

			aCert.CheckValidity();

			aCert.Verify(pubKey);

			AttributeCertificateHolder holder = aCert.Holder;

			if (holder.GetEntityNames() != null)
			{
				Fail("entity names set when none expected");
			}

			if (!holder.SerialNumber.Equals(iCert.SerialNumber))
			{
				Fail("holder serial number doesn't Match");
			}

			if (!holder.GetIssuer()[0].Equivalent(iCert.IssuerDN))
			{
				Fail("holder issuer doesn't Match");
			}

			if (!holder.Match(iCert))
			{
				Fail("generated holder not matching holder certificate");
			}

			X509Attribute[] attrs = aCert.GetAttributes("2.5.24.72");

			if (attrs == null)
			{
				Fail("attributes related to 2.5.24.72 not found");
			}

			X509Attribute attr = attrs[0];

			if (!attr.Oid.Equals("2.5.24.72"))
			{
				Fail("attribute oid mismatch");
			}

			Asn1Encodable[] values = attr.GetValues();

			GeneralName role = GeneralNames.GetInstance(values[0]).GetNames()[0];

			if (role.TagNo != GeneralName.Rfc822Name)
			{
				Fail("wrong general name type found in role");
			}

			if (!((IAsn1String)role.Name).GetString().Equals("DAU123456789"))
			{
				Fail("wrong general name value found in role");
			}

			X509Certificate sCert = fact.ReadCertificate(holderCertWithBaseCertificateID);

			if (holder.Match(sCert))
			{
				Fail("generated holder matching wrong certificate");
			}

			equalityAndHashCodeTest(aCert, aCert.GetEncoded());
		}

		private void doTestGenerateWithPrincipal()
		{
			X509CertificateParser fact = new X509CertificateParser();
			X509Certificate iCert = fact.ReadCertificate(signCert);

			//
			// a sample key pair.
			//
			RsaKeyParameters pubKey = new RsaKeyParameters(
				false,
				new BigInteger("b4a7e46170574f16a97082b22be58b6a2a629798419be12872a4bdba626cfae9900f76abfb12139dce5de56564fab2b6543165a040c606887420e33d91ed7ed7", 16),
				new BigInteger("11", 16));

			//
			// set up the keys
			//
//			PrivateKey          privKey;
//			PublicKey           pubKey;
//
//			KeyFactory  kFact = KeyFactory.getInstance("RSA");
//
//			privKey = kFact.generatePrivate(RSA_PRIVATE_KEY_SPEC);
//			pubKey = kFact.generatePublic(pubKeySpec);
			AsymmetricKeyParameter privKey = RSA_PRIVATE_KEY_SPEC;

			X509V2AttributeCertificateGenerator gen = new X509V2AttributeCertificateGenerator();

			// the actual attributes
			GeneralName roleName = new GeneralName(GeneralName.Rfc822Name, "DAU123456789");

			// roleSyntax OID: 2.5.24.72
			X509Attribute attributes = new X509Attribute("2.5.24.72",
				new DerSequence(roleName));

			gen.AddAttribute(attributes);
			gen.SetHolder(new AttributeCertificateHolder(iCert.SubjectDN));
			gen.SetIssuer(new AttributeCertificateIssuer(new X509Name("cn=test")));
			gen.SetNotBefore(DateTime.UtcNow.AddSeconds(-50));
			gen.SetNotAfter(DateTime.UtcNow.AddSeconds(50));
			gen.SetSerialNumber(BigInteger.One);
			gen.SetSignatureAlgorithm("SHA1WithRSAEncryption");

			IX509AttributeCertificate aCert = gen.Generate(privKey);

			aCert.CheckValidity();

			aCert.Verify(pubKey);

			AttributeCertificateHolder holder = aCert.Holder;

			if (holder.GetEntityNames() == null)
			{
				Fail("entity names not set when expected");
			}

			if (holder.SerialNumber != null)
			{
				Fail("holder serial number found when none expected");
			}

			if (holder.GetIssuer() != null)
			{
				Fail("holder issuer found when none expected");
			}

			if (!holder.Match(iCert))
			{
				Fail("generated holder not matching holder certificate");
			}

			X509Certificate sCert = fact.ReadCertificate(holderCertWithBaseCertificateID);

			if (holder.Match(sCert))
			{
				Fail("principal generated holder matching wrong certificate");
			}

			equalityAndHashCodeTest(aCert, aCert.GetEncoded());
		}

		public override void PerformTest()
		{
			IX509AttributeCertificate aCert = new X509V2AttributeCertificate(attrCert);
			X509CertificateParser fact = new X509CertificateParser();
			X509Certificate sCert = fact.ReadCertificate(signCert);

			aCert.Verify(sCert.GetPublicKey());

			//
			// search test
			//
			IList list = new ArrayList();

			list.Add(sCert);

//			CollectionCertStoreParameters ccsp = new CollectionCertStoreParameters(list);
//			CertStore store = CertStore.getInstance("Collection", ccsp);
			IX509Store store = X509StoreFactory.Create(
				"Certificate/Collection",
				new X509CollectionStoreParameters(list));

			ArrayList certs = new ArrayList(
//				store.getCertificates(aCert.getIssuer()));
				store.GetMatches(aCert.Issuer));

			if (certs.Count != 1 || !certs.Contains(sCert))
			{
				Fail("sCert not found by issuer");
			}

			X509Attribute[] attrs = aCert.GetAttributes("1.3.6.1.4.1.6760.8.1.1");
			if (attrs == null || attrs.Length != 1)
			{
				Fail("attribute not found");
			}

			//
			// reencode test
			//
			aCert = new X509V2AttributeCertificate(aCert.GetEncoded());

			aCert.Verify(sCert.GetPublicKey());

			IX509AttributeCertificate saCert = new X509V2AttributeCertificate(aCert.GetEncoded());

			if (!aCert.NotAfter.Equals(saCert.NotAfter))
			{
				Fail("failed date comparison");
			}

			// base generator test

			//
			// a sample key pair.
			//
			RsaKeyParameters pubKey = new RsaKeyParameters(
				false,
				new BigInteger("b4a7e46170574f16a97082b22be58b6a2a629798419be12872a4bdba626cfae9900f76abfb12139dce5de56564fab2b6543165a040c606887420e33d91ed7ed7", 16),
				new BigInteger("11", 16));

			AsymmetricKeyParameter privKey = RSA_PRIVATE_KEY_SPEC;

			//
			// set up the keys
			//
//			PrivateKey          privKey;
//			PublicKey           pubKey;
//
//			KeyFactory  kFact = KeyFactory.getInstance("RSA");
//
//			privKey = kFact.generatePrivate(privKeySpec);
//			pubKey = kFact.generatePublic(pubKeySpec);

			X509V2AttributeCertificateGenerator gen = new X509V2AttributeCertificateGenerator();

			gen.AddAttribute(attrs[0]);
			gen.SetHolder(aCert.Holder);
			gen.SetIssuer(aCert.Issuer);
			gen.SetNotBefore(DateTime.UtcNow.AddSeconds(-50));
			gen.SetNotAfter(DateTime.UtcNow.AddSeconds(50));
			gen.SetSerialNumber(aCert.SerialNumber);
			gen.SetSignatureAlgorithm("SHA1WithRSAEncryption");

			aCert = gen.Generate(privKey);

			aCert.CheckValidity();

			aCert.Verify(pubKey);

			// as the issuer is the same this should still work (even though it is not
			// technically correct

			certs = new ArrayList(
//				store.getCertificates(aCert.Issuer));
				store.GetMatches(aCert.Issuer));

			if (certs.Count != 1 || !certs.Contains(sCert))
			{
				Fail("sCert not found by issuer");
			}

			attrs = aCert.GetAttributes("1.3.6.1.4.1.6760.8.1.1");
			if (attrs == null || attrs.Length != 1)
			{
				Fail("attribute not found");
			}

			//
			// reencode test
			//
			aCert = new X509V2AttributeCertificate(aCert.GetEncoded());

			aCert.Verify(pubKey);

			AttributeCertificateIssuer issuer = aCert.Issuer;

			X509Name[] principals = issuer.GetPrincipals();

			//
			// test holder
			//
			AttributeCertificateHolder holder = aCert.Holder;

			if (holder.GetEntityNames() == null)
			{
				Fail("entity names not set");
			}

			if (holder.SerialNumber != null)
			{
				Fail("holder serial number set when none expected");
			}

			if (holder.GetIssuer() != null)
			{
				Fail("holder issuer set when none expected");
			}

			principals = holder.GetEntityNames();

			string ps = principals[0].ToString();

			// TODO Check that this is a good enough test
//			if (!ps.Equals("C=US, O=vt, OU=Class 2, OU=Virginia Tech User, CN=Markus Lorch (mlorch), EMAILADDRESS=mlorch@vt.edu"))
			if (!principals[0].Equivalent(new X509Name("C=US, O=vt, OU=Class 2, OU=Virginia Tech User, CN=Markus Lorch (mlorch), EMAILADDRESS=mlorch@vt.edu")))
			{
				Fail("principal[0] for entity names don't Match");
			}

			//
			// extension test
			//

			gen.AddExtension("1.1", true, new DerOctetString(new byte[10]));

			gen.AddExtension("2.2", false, new DerOctetString(new byte[20]));

			aCert = gen.Generate(privKey);

			ISet exts = aCert.GetCriticalExtensionOids();

			if (exts.Count != 1 || !exts.Contains("1.1"))
			{
				Fail("critical extension test failed");
			}

			exts = aCert.GetNonCriticalExtensionOids();

			if (exts.Count != 1 || !exts.Contains("2.2"))
			{
				Fail("non-critical extension test failed");
			}

			Asn1OctetString extString = aCert.GetExtensionValue(new DerObjectIdentifier("1.1"));
			Asn1Encodable extValue = X509ExtensionUtilities.FromExtensionValue(extString);

			if (!extValue.Equals(new DerOctetString(new byte[10])))
			{
				Fail("wrong extension value found for 1.1");
			}

			doTestCertWithBaseCertificateID();
			doTestGenerateWithCert();
			doTestGenerateWithPrincipal();
		}

		public static void Main(
			string[] args)
		{
			RunTest(new AttrCertTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
