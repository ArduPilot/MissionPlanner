using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using Org.BouncyCastle.Crypto.Operators;

namespace Org.BouncyCastle.Cms.Tests
{
	[TestFixture]
	public class SignedDataTest
	{
		private const string OrigDN = "O=Bouncy Castle, C=AU";
		private static AsymmetricCipherKeyPair origKP;
		private static X509Certificate origCert;

		private const string SignDN = "CN=Bob, OU=Sales, O=Bouncy Castle, C=AU";
		private static AsymmetricCipherKeyPair signKP;
		private static X509Certificate signCert;

		private const string ReciDN = "CN=Doug, OU=Sales, O=Bouncy Castle, C=AU";
//		private static AsymmetricCipherKeyPair reciKP;
//		private static X509Certificate reciCert;

		private static X509Crl signCrl;

		private static AsymmetricCipherKeyPair signGostKP;
		private static X509Certificate signGostCert;

		private static AsymmetricCipherKeyPair signDsaKP;
		private static X509Certificate signDsaCert;

		private static AsymmetricCipherKeyPair signECGostKP;
		private static X509Certificate signECGostCert;

		private static AsymmetricCipherKeyPair signECDsaKP;
		private static X509Certificate signECDsaCert;

		private static AsymmetricCipherKeyPair OrigKP
		{
			get { return origKP == null ? (origKP = CmsTestUtil.MakeKeyPair()) : origKP; }
		}

		private static AsymmetricCipherKeyPair SignKP
		{
			get { return signKP == null ? (signKP = CmsTestUtil.MakeKeyPair()) : signKP; }
		}

//		private static AsymmetricCipherKeyPair ReciKP
//		{
//			get { return reciKP == null ? (reciKP = CmsTestUtil.MakeKeyPair()) : reciKP; }
//		}

		private static AsymmetricCipherKeyPair SignGostKP
		{
			get { return signGostKP == null ? (signGostKP = CmsTestUtil.MakeGostKeyPair()) : signGostKP; }
		}

		private static AsymmetricCipherKeyPair SignDsaKP
		{
			get { return signDsaKP == null ? (signDsaKP = CmsTestUtil.MakeDsaKeyPair()) : signDsaKP; }
		}

		private static AsymmetricCipherKeyPair SignECGostKP
		{
			get { return signECGostKP == null ? (signECGostKP = CmsTestUtil.MakeECGostKeyPair()) : signECGostKP; }
		}

		private static AsymmetricCipherKeyPair SignECDsaKP
		{
			get { return signECDsaKP == null ? (signECDsaKP = CmsTestUtil.MakeECDsaKeyPair()) : signECDsaKP; }
		}

		private static X509Certificate OrigCert
		{
			get { return origCert == null ? (origCert = CmsTestUtil.MakeCertificate(OrigKP, OrigDN, OrigKP, OrigDN)) : origCert; }
		}

		private static X509Certificate SignCert
		{
			get { return signCert == null ? (signCert = CmsTestUtil.MakeCertificate(SignKP, SignDN, OrigKP, OrigDN)) : signCert; }
		}

//		private static X509Certificate ReciCert
//		{
//			get { return reciCert == null ? (reciCert = CmsTestUtil.MakeCertificate(ReciKP, ReciDN, SignKP, SignDN)) : reciCert; }
//		}

		private static X509Crl SignCrl
		{
			get { return signCrl == null ? (signCrl = CmsTestUtil.MakeCrl(SignKP)) : signCrl; }
		}

		private static X509Certificate SignGostCert
		{
			get { return signGostCert == null ? (signGostCert = CmsTestUtil.MakeCertificate(SignGostKP, SignDN, OrigKP, OrigDN)) : signGostCert; }
		}

		private static X509Certificate SignECGostCert
		{
			get { return signECGostCert == null ? (signECGostCert = CmsTestUtil.MakeCertificate(SignECGostKP, SignDN, OrigKP, OrigDN)) : signECGostCert; }
		}

		private static X509Certificate SignDsaCert
		{
			get { return signDsaCert == null ? (signDsaCert = CmsTestUtil.MakeCertificate(SignDsaKP, SignDN, OrigKP, OrigDN)) : signDsaCert; }
		}

		private static X509Certificate SignECDsaCert
		{
			get { return signECDsaCert == null ? (signECDsaCert = CmsTestUtil.MakeCertificate(SignECDsaKP, SignDN, OrigKP, OrigDN)) : signECDsaCert; }
		}

		private static readonly byte[] disorderedMessage = Base64.Decode(
			"SU9fc3RkaW5fdXNlZABfX2xpYmNfc3RhcnRfbWFpbgBnZXRob3N0aWQAX19n"
			+ "bW9uX3M=");

		private static readonly byte[] disorderedSet = Base64.Decode(
			"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"
			+ "JyQFn5msmhPQT/6XSGOlrWRoFummXN9lQzAjCj1sgTcmoLCVQ5s5WpCAOXFw"
			+ "VWu16qndz3sPItn3jJ0F3Kh3w79NglvPAgUAwAAAATAKBgYrJAMDAQIFAAOB"
			+ "gQBpSRdnDb6AcNVaXSmGo6+kVPIBhot1LzJOGaPyDNpGXxd7LV4tMBF1U7gr"
			+ "4k1g9BO6YiMWvw9uiTZmn0CfV8+k4fWEuG/nmafRoGIuay2f+ILuT+C0rnp1"
			+ "4FgMsEhuVNJJAmb12QV0PZII+UneyhAneZuQQzVUkTcVgYxogxdSOzCCAlUw"
			+ "ggHBoAMCAQICBACdekowCgYGKyQDAwECBQAwbzELMAkGA1UEBhMCREUxPTA7"
			+ "BgNVBAoUNFJlZ3VsaWVydW5nc2JlaMhvcmRlIGbIdXIgVGVsZWtvbW11bmlr"
			+ "YXRpb24gdW5kIFBvc3QxITAMBgcCggYBCgcUEwExMBEGA1UEAxQKNlItQ2Eg"
			+ "MTpQTjAiGA8yMDAxMDIwMTEzNDcwN1oYDzIwMDUwMzIyMDg1NTUxWjBvMQsw"
			+ "CQYDVQQGEwJERTE9MDsGA1UEChQ0UmVndWxpZXJ1bmdzYmVoyG9yZGUgZsh1"
			+ "ciBUZWxla29tbXVuaWthdGlvbiB1bmQgUG9zdDEhMAwGBwKCBgEKBxQTATEw"
			+ "EQYDVQQDFAo1Ui1DQSAxOlBOMIGhMA0GCSqGSIb3DQEBAQUAA4GPADCBiwKB"
			+ "gQCKHkFTJx8GmoqFTxEOxpK9XkC3NZ5dBEKiUv0Ife3QMqeGMoCUnyJxwW0k"
			+ "2/53duHxtv2yHSZpFKjrjvE/uGwdOMqBMTjMzkFg19e9JPv061wyADOucOIa"
			+ "NAgha/zFt9XUyrHF21knKCvDNExv2MYIAagkTKajLMAw0bu1J0FadQIFAMAA"
			+ "AAEwCgYGKyQDAwECBQADgYEAV1yTi+2gyB7sUhn4PXmi/tmBxAfe5oBjDW8m"
			+ "gxtfudxKGZ6l/FUPNcrSc5oqBYxKWtLmf3XX87LcblYsch617jtNTkMzhx9e"
			+ "qxiD02ufcrxz2EVt0Akdqiz8mdVeqp3oLcNU/IttpSrcA91CAnoUXtDZYwb/"
			+ "gdQ4FI9l3+qo/0UwggJVMIIBwaADAgECAgQAxIymMAoGBiskAwMBAgUAMG8x"
			+ "CzAJBgNVBAYTAkRFMT0wOwYDVQQKFDRSZWd1bGllcnVuZ3NiZWjIb3JkZSBm"
			+ "yHVyIFRlbGVrb21tdW5pa2F0aW9uIHVuZCBQb3N0MSEwDAYHAoIGAQoHFBMB"
			+ "MTARBgNVBAMUCjZSLUNhIDE6UE4wIhgPMjAwMTEwMTUxMzMxNThaGA8yMDA1"
			+ "MDYwMTA5NTIxN1owbzELMAkGA1UEBhMCREUxPTA7BgNVBAoUNFJlZ3VsaWVy"
			+ "dW5nc2JlaMhvcmRlIGbIdXIgVGVsZWtvbW11bmlrYXRpb24gdW5kIFBvc3Qx"
			+ "ITAMBgcCggYBCgcUEwExMBEGA1UEAxQKN1ItQ0EgMTpQTjCBoTANBgkqhkiG"
			+ "9w0BAQEFAAOBjwAwgYsCgYEAiokD/j6lEP4FexF356OpU5teUpGGfUKjIrFX"
			+ "BHc79G0TUzgVxqMoN1PWnWktQvKo8ETaugxLkP9/zfX3aAQzDW4Zki6x6GDq"
			+ "fy09Agk+RJvhfbbIzRkV4sBBco0n73x7TfG/9NTgVr/96U+I+z/1j30aboM6"
			+ "9OkLEhjxAr0/GbsCBQDAAAABMAoGBiskAwMBAgUAA4GBAHWRqRixt+EuqHhR"
			+ "K1kIxKGZL2vZuakYV0R24Gv/0ZR52FE4ECr+I49o8FP1qiGSwnXB0SwjuH2S"
			+ "iGiSJi+iH/MeY85IHwW1P5e+bOMvEOFhZhQXQixOD7totIoFtdyaj1XGYRef"
			+ "0f2cPOjNJorXHGV8wuBk+/j++sxbd/Net3FtMIICVTCCAcGgAwIBAgIEAMSM"
			+ "pzAKBgYrJAMDAQIFADBvMQswCQYDVQQGEwJERTE9MDsGA1UEChQ0UmVndWxp"
			+ "ZXJ1bmdzYmVoyG9yZGUgZsh1ciBUZWxla29tbXVuaWthdGlvbiB1bmQgUG9z"
			+ "dDEhMAwGBwKCBgEKBxQTATEwEQYDVQQDFAo3Ui1DQSAxOlBOMCIYDzIwMDEx"
			+ "MDE1MTMzNDE0WhgPMjAwNTA2MDEwOTUyMTdaMG8xCzAJBgNVBAYTAkRFMT0w"
			+ "OwYDVQQKFDRSZWd1bGllcnVuZ3NiZWjIb3JkZSBmyHVyIFRlbGVrb21tdW5p"
			+ "a2F0aW9uIHVuZCBQb3N0MSEwDAYHAoIGAQoHFBMBMTARBgNVBAMUCjZSLUNh"
			+ "IDE6UE4wgaEwDQYJKoZIhvcNAQEBBQADgY8AMIGLAoGBAIOiqxUkzVyqnvth"
			+ "ihnltsE5m1Xn5TZKeR/2MQPStc5hJ+V4yptEtIx+Fn5rOoqT5VEVWhcE35wd"
			+ "bPvgJyQFn5msmhPQT/6XSGOlrWRoFummXN9lQzAjCj1sgTcmoLCVQ5s5WpCA"
			+ "OXFwVWu16qndz3sPItn3jJ0F3Kh3w79NglvPAgUAwAAAATAKBgYrJAMDAQIF"
			+ "AAOBgQBi5W96UVDoNIRkCncqr1LLG9vF9SGBIkvFpLDIIbcvp+CXhlvsdCJl"
			+ "0pt2QEPSDl4cmpOet+CxJTdTuMeBNXxhb7Dvualog69w/+K2JbPhZYxuVFZs"
			+ "Zh5BkPn2FnbNu3YbJhE60aIkikr72J4XZsI5DxpZCGh6xyV/YPRdKSljFjCC"
			+ "AlQwggHAoAMCAQICAwyDqzAKBgYrJAMDAQIFADBvMQswCQYDVQQGEwJERTE9"
			+ "MDsGA1UEChQ0UmVndWxpZXJ1bmdzYmVoyG9yZGUgZsh1ciBUZWxla29tbXVu"
			+ "aWthdGlvbiB1bmQgUG9zdDEhMAwGBwKCBgEKBxQTATEwEQYDVQQDFAo1Ui1D"
			+ "QSAxOlBOMCIYDzIwMDAwMzIyMDk0MTI3WhgPMjAwNDAxMjExNjA0NTNaMG8x"
			+ "CzAJBgNVBAYTAkRFMT0wOwYDVQQKFDRSZWd1bGllcnVuZ3NiZWjIb3JkZSBm"
			+ "yHVyIFRlbGVrb21tdW5pa2F0aW9uIHVuZCBQb3N0MSEwDAYHAoIGAQoHFBMB"
			+ "MTARBgNVBAMUCjRSLUNBIDE6UE4wgaEwDQYJKoZIhvcNAQEBBQADgY8AMIGL"
			+ "AoGBAI8x26tmrFJanlm100B7KGlRemCD1R93PwdnG7svRyf5ZxOsdGrDszNg"
			+ "xg6ouO8ZHQMT3NC2dH8TvO65Js+8bIyTm51azF6clEg0qeWNMKiiXbBXa+ph"
			+ "hTkGbXiLYvACZ6/MTJMJ1lcrjpRF7BXtYeYMcEF6znD4pxOqrtbf9z5hAgUA"
			+ "wAAAATAKBgYrJAMDAQIFAAOBgQB99BjSKlGPbMLQAgXlvA9jUsDNhpnVm3a1"
			+ "YkfxSqS/dbQlYkbOKvCxkPGA9NBxisBM8l1zFynVjJoy++aysRmcnLY/sHaz"
			+ "23BF2iU7WERy18H3lMBfYB6sXkfYiZtvQZcWaO48m73ZBySuiV3iXpb2wgs/"
			+ "Cs20iqroAWxwq/W/9jCCAlMwggG/oAMCAQICBDsFZ9UwCgYGKyQDAwECBQAw"
			+ "bzELMAkGA1UEBhMCREUxITAMBgcCggYBCgcUEwExMBEGA1UEAxQKNFItQ0Eg"
			+ "MTpQTjE9MDsGA1UEChQ0UmVndWxpZXJ1bmdzYmVoyG9yZGUgZsh1ciBUZWxl"
			+ "a29tbXVuaWthdGlvbiB1bmQgUG9zdDAiGA8xOTk5MDEyMTE3MzUzNFoYDzIw"
			+ "MDQwMTIxMTYwMDAyWjBvMQswCQYDVQQGEwJERTE9MDsGA1UEChQ0UmVndWxp"
			+ "ZXJ1bmdzYmVoyG9yZGUgZsh1ciBUZWxla29tbXVuaWthdGlvbiB1bmQgUG9z"
			+ "dDEhMAwGBwKCBgEKBxQTATEwEQYDVQQDFAozUi1DQSAxOlBOMIGfMA0GCSqG"
			+ "SIb3DQEBAQUAA4GNADCBiQKBgI4B557mbKQg/AqWBXNJhaT/6lwV93HUl4U8"
			+ "u35udLq2+u9phns1WZkdM3gDfEpL002PeLfHr1ID/96dDYf04lAXQfombils"
			+ "of1C1k32xOvxjlcrDOuPEMxz9/HDAQZA5MjmmYHAIulGI8Qg4Tc7ERRtg/hd"
			+ "0QX0/zoOeXoDSEOBAgTAAAABMAoGBiskAwMBAgUAA4GBAIyzwfT3keHI/n2P"
			+ "LrarRJv96mCohmDZNpUQdZTVjGu5VQjVJwk3hpagU0o/t/FkdzAjOdfEw8Ql"
			+ "3WXhfIbNLv1YafMm2eWSdeYbLcbB5yJ1od+SYyf9+tm7cwfDAcr22jNRBqx8"
			+ "wkWKtKDjWKkevaSdy99sAI8jebHtWz7jzydKMIID9TCCA16gAwIBAgICbMcw"
			+ "DQYJKoZIhvcNAQEFBQAwSzELMAkGA1UEBhMCREUxEjAQBgNVBAoUCVNpZ250"
			+ "cnVzdDEoMAwGBwKCBgEKBxQTATEwGAYDVQQDFBFDQSBTSUdOVFJVU1QgMTpQ"
			+ "TjAeFw0wNDA3MzAxMzAyNDZaFw0wNzA3MzAxMzAyNDZaMDwxETAPBgNVBAMM"
			+ "CFlhY29tOlBOMQ4wDAYDVQRBDAVZYWNvbTELMAkGA1UEBhMCREUxCjAIBgNV"
			+ "BAUTATEwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBAIWzLlYLQApocXIp"
			+ "pgCCpkkOUVLgcLYKeOd6/bXAnI2dTHQqT2bv7qzfUnYvOqiNgYdF13pOYtKg"
			+ "XwXMTNFL4ZOI6GoBdNs9TQiZ7KEWnqnr2945HYx7UpgTBclbOK/wGHuCdcwO"
			+ "x7juZs1ZQPFG0Lv8RoiV9s6HP7POqh1sO0P/AgMBAAGjggH1MIIB8TCBnAYD"
			+ "VR0jBIGUMIGRgBQcZzNghfnXoXRm8h1+VITC5caNRqFzpHEwbzELMAkGA1UE"
			+ "BhMCREUxPTA7BgNVBAoUNFJlZ3VsaWVydW5nc2JlaMhvcmRlIGbIdXIgVGVs"
			+ "ZWtvbW11bmlrYXRpb24gdW5kIFBvc3QxITAMBgcCggYBCgcUEwExMBEGA1UE"
			+ "AxQKNVItQ0EgMTpQToIEALs8rjAdBgNVHQ4EFgQU2e5KAzkVuKaM9I5heXkz"
			+ "bcAIuR8wDgYDVR0PAQH/BAQDAgZAMBIGA1UdIAQLMAkwBwYFKyQIAQEwfwYD"
			+ "VR0fBHgwdjB0oCygKoYobGRhcDovL2Rpci5zaWdudHJ1c3QuZGUvbz1TaWdu"
			+ "dHJ1c3QsYz1kZaJEpEIwQDEdMBsGA1UEAxMUQ1JMU2lnblNpZ250cnVzdDE6"
			+ "UE4xEjAQBgNVBAoTCVNpZ250cnVzdDELMAkGA1UEBhMCREUwYgYIKwYBBQUH"
			+ "AQEEVjBUMFIGCCsGAQUFBzABhkZodHRwOi8vZGlyLnNpZ250cnVzdC5kZS9T"
			+ "aWdudHJ1c3QvT0NTUC9zZXJ2bGV0L2h0dHBHYXRld2F5LlBvc3RIYW5kbGVy"
			+ "MBgGCCsGAQUFBwEDBAwwCjAIBgYEAI5GAQEwDgYHAoIGAQoMAAQDAQH/MA0G"
			+ "CSqGSIb3DQEBBQUAA4GBAHn1m3GcoyD5GBkKUY/OdtD6Sj38LYqYCF+qDbJR"
			+ "6pqUBjY2wsvXepUppEler+stH8mwpDDSJXrJyuzf7xroDs4dkLl+Rs2x+2tg"
			+ "BjU+ABkBDMsym2WpwgA8LCdymmXmjdv9tULxY+ec2pjSEzql6nEZNEfrU8nt"
			+ "ZCSCavgqW4TtMYIBejCCAXYCAQEwUTBLMQswCQYDVQQGEwJERTESMBAGA1UE"
			+ "ChQJU2lnbnRydXN0MSgwDAYHAoIGAQoHFBMBMTAYBgNVBAMUEUNBIFNJR05U"
			+ "UlVTVCAxOlBOAgJsxzAJBgUrDgMCGgUAoIGAMBgGCSqGSIb3DQEJAzELBgkq"
			+ "hkiG9w0BBwEwIwYJKoZIhvcNAQkEMRYEFIYfhPoyfGzkLWWSSLjaHb4HQmaK"
			+ "MBwGCSqGSIb3DQEJBTEPFw0wNTAzMjQwNzM4MzVaMCEGBSskCAYFMRgWFi92"
			+ "YXIvZmlsZXMvdG1wXzEvdGVzdDEwDQYJKoZIhvcNAQEFBQAEgYA2IvA8lhVz"
			+ "VD5e/itUxbFboKxeKnqJ5n/KuO/uBCl1N14+7Z2vtw1sfkIG+bJdp3OY2Cmn"
			+ "mrQcwsN99Vjal4cXVj8t+DJzFG9tK9dSLvD3q9zT/GQ0kJXfimLVwCa4NaSf"
			+ "Qsu4xtG0Rav6bCcnzabAkKuNNvKtH8amSRzk870DBg==");

		private static readonly byte[] xtraCounterSig = Base64.Decode(
			  "MIIR/AYJKoZIhvcNAQcCoIIR7TCCEekCAQExCzAJBgUrDgMCGgUAMBoGCSqG"
			+ "SIb3DQEHAaANBAtIZWxsbyB3b3JsZKCCDnkwggTPMIIDt6ADAgECAgRDnYD3"
			+ "MA0GCSqGSIb3DQEBBQUAMFgxCzAJBgNVBAYTAklUMRowGAYDVQQKExFJbi5U"
			+ "ZS5TLkEuIFMucC5BLjEtMCsGA1UEAxMkSW4uVGUuUy5BLiAtIENlcnRpZmlj"
			+ "YXRpb24gQXV0aG9yaXR5MB4XDTA4MDkxMjExNDMxMloXDTEwMDkxMjExNDMx"
			+ "MlowgdgxCzAJBgNVBAYTAklUMSIwIAYDVQQKDBlJbnRlc2EgUy5wLkEuLzA1"
			+ "MjYyODkwMDE0MSowKAYDVQQLDCFCdXNpbmVzcyBDb2xsYWJvcmF0aW9uICYg"
			+ "U2VjdXJpdHkxHjAcBgNVBAMMFU1BU1NJTUlMSUFOTyBaSUNDQVJESTERMA8G"
			+ "A1UEBAwIWklDQ0FSREkxFTATBgNVBCoMDE1BU1NJTUlMSUFOTzEcMBoGA1UE"
			+ "BRMTSVQ6WkNDTVNNNzZIMTRMMjE5WTERMA8GA1UELhMIMDAwMDI1ODUwgaAw"
			+ "DQYJKoZIhvcNAQEBBQADgY4AMIGKAoGBALeJTjmyFgx1SIP6c2AuB/kuyHo5"
			+ "j/prKELTALsFDimre/Hxr3wOSet1TdQfFzU8Lu+EJqgfV9cV+cI1yeH1rZs7"
			+ "lei7L3tX/VR565IywnguX5xwvteASgWZr537Fkws50bvTEMyYOj1Tf3FZvZU"
			+ "z4n4OD39KI4mfR9i1eEVIxR3AgQAizpNo4IBoTCCAZ0wHQYDVR0RBBYwFIES"
			+ "emljY2FyZGlAaW50ZXNhLml0MC8GCCsGAQUFBwEDBCMwITAIBgYEAI5GAQEw"
			+ "CwYGBACORgEDAgEUMAgGBgQAjkYBBDBZBgNVHSAEUjBQME4GBgQAizABATBE"
			+ "MEIGCCsGAQUFBwIBFjZodHRwOi8vZS10cnVzdGNvbS5pbnRlc2EuaXQvY2Ff"
			+ "cHViYmxpY2EvQ1BTX0lOVEVTQS5odG0wDgYDVR0PAQH/BAQDAgZAMIGDBgNV"
			+ "HSMEfDB6gBQZCQOW0bjFWBt+EORuxPagEgkQqKFcpFowWDELMAkGA1UEBhMC"
			+ "SVQxGjAYBgNVBAoTEUluLlRlLlMuQS4gUy5wLkEuMS0wKwYDVQQDEyRJbi5U"
			+ "ZS5TLkEuIC0gQ2VydGlmaWNhdGlvbiBBdXRob3JpdHmCBDzRARMwOwYDVR0f"
			+ "BDQwMjAwoC6gLIYqaHR0cDovL2UtdHJ1c3Rjb20uaW50ZXNhLml0L0NSTC9J"
			+ "TlRFU0EuY3JsMB0GA1UdDgQWBBTf5ItL8KmQh541Dxt7YxcWI1254TANBgkq"
			+ "hkiG9w0BAQUFAAOCAQEAgW+uL1CVWQepbC/wfCmR6PN37Sueb4xiKQj2mTD5"
			+ "UZ5KQjpivy/Hbuf0NrfKNiDEhAvoHSPC31ebGiKuTMFNyZPHfPEUnyYGSxea"
			+ "2w837aXJFr6utPNQGBRi89kH90sZDlXtOSrZI+AzJJn5QK3F9gjcayU2NZXQ"
			+ "MJgRwYmFyn2w4jtox+CwXPQ9E5XgxiMZ4WDL03cWVXDLX00EOJwnDDMUNTRI"
			+ "m9Zv+4SKTNlfFbi9UTBqWBySkDzAelsfB2U61oqc2h1xKmCtkGMmN9iZT+Qz"
			+ "ZC/vaaT+hLEBFGAH2gwFrYc4/jTBKyBYeU1vsAxsibIoTs1Apgl6MH75qPDL"
			+ "BzCCBM8wggO3oAMCAQICBEOdgPcwDQYJKoZIhvcNAQEFBQAwWDELMAkGA1UE"
			+ "BhMCSVQxGjAYBgNVBAoTEUluLlRlLlMuQS4gUy5wLkEuMS0wKwYDVQQDEyRJ"
			+ "bi5UZS5TLkEuIC0gQ2VydGlmaWNhdGlvbiBBdXRob3JpdHkwHhcNMDgwOTEy"
			+ "MTE0MzEyWhcNMTAwOTEyMTE0MzEyWjCB2DELMAkGA1UEBhMCSVQxIjAgBgNV"
			+ "BAoMGUludGVzYSBTLnAuQS4vMDUyNjI4OTAwMTQxKjAoBgNVBAsMIUJ1c2lu"
			+ "ZXNzIENvbGxhYm9yYXRpb24gJiBTZWN1cml0eTEeMBwGA1UEAwwVTUFTU0lN"
			+ "SUxJQU5PIFpJQ0NBUkRJMREwDwYDVQQEDAhaSUNDQVJESTEVMBMGA1UEKgwM"
			+ "TUFTU0lNSUxJQU5PMRwwGgYDVQQFExNJVDpaQ0NNU003NkgxNEwyMTlZMREw"
			+ "DwYDVQQuEwgwMDAwMjU4NTCBoDANBgkqhkiG9w0BAQEFAAOBjgAwgYoCgYEA"
			+ "t4lOObIWDHVIg/pzYC4H+S7IejmP+msoQtMAuwUOKat78fGvfA5J63VN1B8X"
			+ "NTwu74QmqB9X1xX5wjXJ4fWtmzuV6Lsve1f9VHnrkjLCeC5fnHC+14BKBZmv"
			+ "nfsWTCznRu9MQzJg6PVN/cVm9lTPifg4Pf0ojiZ9H2LV4RUjFHcCBACLOk2j"
			+ "ggGhMIIBnTAdBgNVHREEFjAUgRJ6aWNjYXJkaUBpbnRlc2EuaXQwLwYIKwYB"
			+ "BQUHAQMEIzAhMAgGBgQAjkYBATALBgYEAI5GAQMCARQwCAYGBACORgEEMFkG"
			+ "A1UdIARSMFAwTgYGBACLMAEBMEQwQgYIKwYBBQUHAgEWNmh0dHA6Ly9lLXRy"
			+ "dXN0Y29tLmludGVzYS5pdC9jYV9wdWJibGljYS9DUFNfSU5URVNBLmh0bTAO"
			+ "BgNVHQ8BAf8EBAMCBkAwgYMGA1UdIwR8MHqAFBkJA5bRuMVYG34Q5G7E9qAS"
			+ "CRCooVykWjBYMQswCQYDVQQGEwJJVDEaMBgGA1UEChMRSW4uVGUuUy5BLiBT"
			+ "LnAuQS4xLTArBgNVBAMTJEluLlRlLlMuQS4gLSBDZXJ0aWZpY2F0aW9uIEF1"
			+ "dGhvcml0eYIEPNEBEzA7BgNVHR8ENDAyMDCgLqAshipodHRwOi8vZS10cnVz"
			+ "dGNvbS5pbnRlc2EuaXQvQ1JML0lOVEVTQS5jcmwwHQYDVR0OBBYEFN/ki0vw"
			+ "qZCHnjUPG3tjFxYjXbnhMA0GCSqGSIb3DQEBBQUAA4IBAQCBb64vUJVZB6ls"
			+ "L/B8KZHo83ftK55vjGIpCPaZMPlRnkpCOmK/L8du5/Q2t8o2IMSEC+gdI8Lf"
			+ "V5saIq5MwU3Jk8d88RSfJgZLF5rbDzftpckWvq6081AYFGLz2Qf3SxkOVe05"
			+ "Ktkj4DMkmflArcX2CNxrJTY1ldAwmBHBiYXKfbDiO2jH4LBc9D0TleDGIxnh"
			+ "YMvTdxZVcMtfTQQ4nCcMMxQ1NEib1m/7hIpM2V8VuL1RMGpYHJKQPMB6Wx8H"
			+ "ZTrWipzaHXEqYK2QYyY32JlP5DNkL+9ppP6EsQEUYAfaDAWthzj+NMErIFh5"
			+ "TW+wDGyJsihOzUCmCXowfvmo8MsHMIIEzzCCA7egAwIBAgIEQ52A9zANBgkq"
			+ "hkiG9w0BAQUFADBYMQswCQYDVQQGEwJJVDEaMBgGA1UEChMRSW4uVGUuUy5B"
			+ "LiBTLnAuQS4xLTArBgNVBAMTJEluLlRlLlMuQS4gLSBDZXJ0aWZpY2F0aW9u"
			+ "IEF1dGhvcml0eTAeFw0wODA5MTIxMTQzMTJaFw0xMDA5MTIxMTQzMTJaMIHY"
			+ "MQswCQYDVQQGEwJJVDEiMCAGA1UECgwZSW50ZXNhIFMucC5BLi8wNTI2Mjg5"
			+ "MDAxNDEqMCgGA1UECwwhQnVzaW5lc3MgQ29sbGFib3JhdGlvbiAmIFNlY3Vy"
			+ "aXR5MR4wHAYDVQQDDBVNQVNTSU1JTElBTk8gWklDQ0FSREkxETAPBgNVBAQM"
			+ "CFpJQ0NBUkRJMRUwEwYDVQQqDAxNQVNTSU1JTElBTk8xHDAaBgNVBAUTE0lU"
			+ "OlpDQ01TTTc2SDE0TDIxOVkxETAPBgNVBC4TCDAwMDAyNTg1MIGgMA0GCSqG"
			+ "SIb3DQEBAQUAA4GOADCBigKBgQC3iU45shYMdUiD+nNgLgf5Lsh6OY/6ayhC"
			+ "0wC7BQ4pq3vx8a98DknrdU3UHxc1PC7vhCaoH1fXFfnCNcnh9a2bO5Xouy97"
			+ "V/1UeeuSMsJ4Ll+ccL7XgEoFma+d+xZMLOdG70xDMmDo9U39xWb2VM+J+Dg9"
			+ "/SiOJn0fYtXhFSMUdwIEAIs6TaOCAaEwggGdMB0GA1UdEQQWMBSBEnppY2Nh"
			+ "cmRpQGludGVzYS5pdDAvBggrBgEFBQcBAwQjMCEwCAYGBACORgEBMAsGBgQA"
			+ "jkYBAwIBFDAIBgYEAI5GAQQwWQYDVR0gBFIwUDBOBgYEAIswAQEwRDBCBggr"
			+ "BgEFBQcCARY2aHR0cDovL2UtdHJ1c3Rjb20uaW50ZXNhLml0L2NhX3B1YmJs"
			+ "aWNhL0NQU19JTlRFU0EuaHRtMA4GA1UdDwEB/wQEAwIGQDCBgwYDVR0jBHww"
			+ "eoAUGQkDltG4xVgbfhDkbsT2oBIJEKihXKRaMFgxCzAJBgNVBAYTAklUMRow"
			+ "GAYDVQQKExFJbi5UZS5TLkEuIFMucC5BLjEtMCsGA1UEAxMkSW4uVGUuUy5B"
			+ "LiAtIENlcnRpZmljYXRpb24gQXV0aG9yaXR5ggQ80QETMDsGA1UdHwQ0MDIw"
			+ "MKAuoCyGKmh0dHA6Ly9lLXRydXN0Y29tLmludGVzYS5pdC9DUkwvSU5URVNB"
			+ "LmNybDAdBgNVHQ4EFgQU3+SLS/CpkIeeNQ8be2MXFiNdueEwDQYJKoZIhvcN"
			+ "AQEFBQADggEBAIFvri9QlVkHqWwv8Hwpkejzd+0rnm+MYikI9pkw+VGeSkI6"
			+ "Yr8vx27n9Da3yjYgxIQL6B0jwt9XmxoirkzBTcmTx3zxFJ8mBksXmtsPN+2l"
			+ "yRa+rrTzUBgUYvPZB/dLGQ5V7Tkq2SPgMySZ+UCtxfYI3GslNjWV0DCYEcGJ"
			+ "hcp9sOI7aMfgsFz0PROV4MYjGeFgy9N3FlVwy19NBDicJwwzFDU0SJvWb/uE"
			+ "ikzZXxW4vVEwalgckpA8wHpbHwdlOtaKnNodcSpgrZBjJjfYmU/kM2Qv72mk"
			+ "/oSxARRgB9oMBa2HOP40wSsgWHlNb7AMbImyKE7NQKYJejB++ajwywcxggM8"
			+ "MIIDOAIBATBgMFgxCzAJBgNVBAYTAklUMRowGAYDVQQKExFJbi5UZS5TLkEu"
			+ "IFMucC5BLjEtMCsGA1UEAxMkSW4uVGUuUy5BLiAtIENlcnRpZmljYXRpb24g"
			+ "QXV0aG9yaXR5AgRDnYD3MAkGBSsOAwIaBQAwDQYJKoZIhvcNAQEBBQAEgYB+"
			+ "lH2cwLqc91mP8prvgSV+RRzk13dJdZvdoVjgQoFrPhBiZCNIEoHvIhMMA/sM"
			+ "X6euSRZk7EjD24FasCEGYyd0mJVLEy6TSPmuW+wWz/28w3a6IWXBGrbb/ild"
			+ "/CJMkPgLPGgOVD1WDwiNKwfasiQSFtySf5DPn3jFevdLeMmEY6GCAjIwggEV"
			+ "BgkqhkiG9w0BCQYxggEGMIIBAgIBATBgMFgxCzAJBgNVBAYTAklUMRowGAYD"
			+ "VQQKExFJbi5UZS5TLkEuIFMucC5BLjEtMCsGA1UEAxMkSW4uVGUuUy5BLiAt"
			+ "IENlcnRpZmljYXRpb24gQXV0aG9yaXR5AgRDnYD3MAkGBSsOAwIaBQAwDQYJ"
			+ "KoZIhvcNAQEBBQAEgYBHlOULfT5GDigIvxP0qZOy8VbpntmzaPF55VV4buKV"
			+ "35J+uHp98gXKp0LrHM69V5IRKuyuQzHHFBqsXxsRI9o6KoOfgliD9Xc+BeMg"
			+ "dKzQhBhBYoFREq8hQM0nSbqDNHYAQyNHMzUA/ZQUO5dlFuH8Dw3iDYAhNtfd"
			+ "PrlchKJthDCCARUGCSqGSIb3DQEJBjGCAQYwggECAgEBMGAwWDELMAkGA1UE"
			+ "BhMCSVQxGjAYBgNVBAoTEUluLlRlLlMuQS4gUy5wLkEuMS0wKwYDVQQDEyRJ"
			+ "bi5UZS5TLkEuIC0gQ2VydGlmaWNhdGlvbiBBdXRob3JpdHkCBEOdgPcwCQYF"
			+ "Kw4DAhoFADANBgkqhkiG9w0BAQEFAASBgEeU5Qt9PkYOKAi/E/Spk7LxVume"
			+ "2bNo8XnlVXhu4pXfkn64en3yBcqnQusczr1XkhEq7K5DMccUGqxfGxEj2joq"
			+ "g5+CWIP1dz4F4yB0rNCEGEFigVESryFAzSdJuoM0dgBDI0czNQD9lBQ7l2UW"
			+ "4fwPDeINgCE2190+uVyEom2E");

		private void VerifySignatures(
			CmsSignedData	s,
			byte[]			contentDigest)
		{
			IX509Store x509Certs = s.GetCertificates("Collection");

			SignerInformationStore signers = s.GetSignerInfos();
			ICollection c = signers.GetSigners();

			foreach (SignerInformation signer in c)
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				Assert.IsTrue(signer.Verify(cert));

				if (contentDigest != null)
				{
					Assert.IsTrue(Arrays.AreEqual(contentDigest, signer.GetContentDigest()));
				}
			}
		}

		private void VerifySignatures(
			CmsSignedData s)
		{
			VerifySignatures(s, null);
		}

		[Test]
		public void TestDetachedVerification()
		{
			byte[] data = Encoding.ASCII.GetBytes("Hello World!");
			CmsProcessable msg = new CmsProcessableByteArray(data);

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestMD5);
			gen.AddCertificates(x509Certs);

            CmsSignedData s = gen.Generate(msg);

			IDictionary hashes = new Hashtable();
			hashes.Add(CmsSignedDataGenerator.DigestSha1, DigestUtilities.CalculateDigest("SHA1", data));
            hashes.Add(CmsSignedDataGenerator.DigestMD5, DigestUtilities.CalculateDigest("MD5", data));

			s = new CmsSignedData(hashes, s.GetEncoded());

			VerifySignatures(s, null);
		}

        [Test]
		public void TestSha1AndMD5WithRsaEncapsulatedRepeated()
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

			CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestMD5);
			gen.AddCertificates(x509Certs);

            CmsSignedData s = gen.Generate(msg, true);

			s = new CmsSignedData(ContentInfo.GetInstance(Asn1Object.FromByteArray(s.GetEncoded())));

			x509Certs = s.GetCertificates("Collection");

			SignerInformationStore signers = s.GetSignerInfos();

			Assert.AreEqual(2, signers.Count);

			SignerID sid = null;
			ICollection c = signers.GetSigners();

			foreach (SignerInformation signer in c)
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				sid = signer.SignerID;

				Assert.IsTrue(signer.Verify(cert));

				//
				// check content digest
				//

				byte[] contentDigest = (byte[])gen.GetGeneratedDigests()[signer.DigestAlgOid];

				AttributeTable table = signer.SignedAttributes;
				Asn1.Cms.Attribute hash = table[CmsAttributes.MessageDigest];

				Assert.IsTrue(Arrays.AreEqual(contentDigest, ((Asn1OctetString)hash.AttrValues[0]).GetOctets()));
			}

			c = signers.GetSigners(sid);

			Assert.AreEqual(2, c.Count);

			//
			// try using existing signer
			//

			gen = new CmsSignedDataGenerator();

			gen.AddSigners(s.GetSignerInfos());

			gen.AddCertificates(s.GetCertificates("Collection"));
			gen.AddCrls(s.GetCrls("Collection"));

			s = gen.Generate(msg, true);

			s = new CmsSignedData(ContentInfo.GetInstance(Asn1Object.FromByteArray(s.GetEncoded())));

			x509Certs = s.GetCertificates("Collection");

			signers = s.GetSignerInfos();
			c = signers.GetSigners();

			Assert.AreEqual(2, c.Count);

			foreach (SignerInformation signer in c)
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				Assert.AreEqual(true, signer.Verify(cert));
			}

			CheckSignerStoreReplacement(s, signers);
		}

        [Test]
        public void TestSha1AndMD5WithRsaEncapsulatedRepeatedWithSignerInfoGen()
        {
            CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
            gen.AddSignerInfoGenerator(new SignerInfoGeneratorBuilder().Build(
                new Asn1SignatureFactory("SHA1withRSA", OrigKP.Private), OrigCert));
            gen.AddSignerInfoGenerator(new SignerInfoGeneratorBuilder().Build(
                new Asn1SignatureFactory("MD5withRSA", OrigKP.Private), OrigCert));

            gen.AddCertificates(x509Certs);

            CmsSignedData s = gen.Generate(msg, true);

            s = new CmsSignedData(ContentInfo.GetInstance(Asn1Object.FromByteArray(s.GetEncoded())));

            x509Certs = s.GetCertificates("Collection");

            SignerInformationStore signers = s.GetSignerInfos();

            Assert.AreEqual(2, signers.Count);

            SignerID sid = null;
            ICollection c = signers.GetSigners();

            foreach (SignerInformation signer in c)
            {
                ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

                IEnumerator certEnum = certCollection.GetEnumerator();

                certEnum.MoveNext();
                X509Certificate cert = (X509Certificate)certEnum.Current;

                sid = signer.SignerID;

                Assert.IsTrue(signer.Verify(cert));

                //
                // check content digest
                //

                byte[] contentDigest = (byte[])gen.GetGeneratedDigests()[signer.DigestAlgOid];

                AttributeTable table = signer.SignedAttributes;
                Asn1.Cms.Attribute hash = table[CmsAttributes.MessageDigest];

                Assert.IsTrue(Arrays.AreEqual(contentDigest, ((Asn1OctetString)hash.AttrValues[0]).GetOctets()));
            }

            c = signers.GetSigners(sid);

            Assert.AreEqual(2, c.Count);

            //
            // try using existing signer
            //

            gen = new CmsSignedDataGenerator();

            gen.AddSigners(s.GetSignerInfos());

            gen.AddCertificates(s.GetCertificates("Collection"));
            gen.AddCrls(s.GetCrls("Collection"));

            s = gen.Generate(msg, true);

            s = new CmsSignedData(ContentInfo.GetInstance(Asn1Object.FromByteArray(s.GetEncoded())));

            x509Certs = s.GetCertificates("Collection");

            signers = s.GetSignerInfos();
            c = signers.GetSigners();

            Assert.AreEqual(2, c.Count);

            foreach (SignerInformation signer in c)
            {
                ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

                IEnumerator certEnum = certCollection.GetEnumerator();

                certEnum.MoveNext();
                X509Certificate cert = (X509Certificate)certEnum.Current;

                Assert.AreEqual(true, signer.Verify(cert));
            }

            CheckSignerStoreReplacement(s, signers);
        }

        // NB: C# build doesn't support "no attributes" version of CmsSignedDataGenerator.Generate
        //[Test]
        //public void TestSha1WithRsaNoAttributes()
        //{
        //    CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello world!"));

        //    IX509Store x509Certs = MakeCertStore(OrigCert, SignCert);

        //    CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
        //    gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
        //    gen.AddCertificates(x509Certs);

        //    CmsSignedData s = gen.Generate(CmsSignedDataGenerator.Data, msg, false, false);

        //    byte[] testBytes = Encoding.ASCII.GetBytes("Hello world!");

        //    // compute expected content digest
        //    byte[] hash = DigestUtilities.CalculateDigest("SHA1", testBytes);

        //    VerifySignatures(s, hash);
        //}

        [Test]
		public void TestSha1WithRsaAndAttributeTable()
		{
			byte[] testBytes = Encoding.ASCII.GetBytes("Hello world!");
			CmsProcessable msg = new CmsProcessableByteArray(testBytes);

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

            byte[] hash = DigestUtilities.CalculateDigest("SHA1", testBytes);

			Asn1.Cms.Attribute attr = new Asn1.Cms.Attribute(CmsAttributes.MessageDigest,
				new DerSet(new DerOctetString(hash)));

			Asn1EncodableVector v = new Asn1EncodableVector(attr);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
            gen.AddSigner(SignKP.Private, SignCert, CmsSignedDataGenerator.DigestSha1,
				new AttributeTable(v), null);
			gen.AddCertificates(x509Certs);

            CmsSignedData s = gen.Generate(CmsSignedDataGenerator.Data, null, false);

            //
			// the signature is detached, so need to add msg before passing on
			//
			s = new CmsSignedData(msg, s.GetEncoded());

			//
			// compute expected content digest
			//
			VerifySignatures(s, hash);
		}

        [Test]
        public void TestRawSha256MissingNull()
        {
            byte[] document = GetInput("rawsha256nonull.p7m");

            CmsSignedData s = new CmsSignedData(document);

            IX509Store certStore = s.GetCertificates("Collection");
            foreach (SignerInformation signerInformation in s.GetSignerInfos().GetSigners())
            {
                ICollection certCollection = certStore.GetMatches(signerInformation.SignerID);
                foreach (X509Certificate cert in certCollection)
                {
                    Assert.IsTrue(signerInformation.Verify(cert), "raw sig failed");
                }
            }
        }

		[Test]
		public void TestSha1WithRsaEncapsulated()
		{
			EncapsulatedTest(SignKP, SignCert, CmsSignedDataGenerator.DigestSha1);
		}

		[Test]
		public void TestSha1WithRsaEncapsulatedSubjectKeyID()
		{
			SubjectKeyIDTest(SignKP, SignCert, CmsSignedDataGenerator.DigestSha1);
		}

		[Test]
		public void TestSha1WithRsaPss()
		{
			rsaPssTest("SHA1", CmsSignedDataGenerator.DigestSha1);
		}

		[Test]
		public void TestSha224WithRsaPss()
		{
			rsaPssTest("SHA224", CmsSignedDataGenerator.DigestSha224);
		}

		[Test]
		public void TestSha256WithRsaPss()
		{
			rsaPssTest("SHA256", CmsSignedDataGenerator.DigestSha256);
		}

		[Test]
		public void TestSha384WithRsaPss()
		{
			rsaPssTest("SHA384", CmsSignedDataGenerator.DigestSha384);
		}

		[Test]
		public void TestSha224WithRsaEncapsulated()
		{
			EncapsulatedTest(SignKP, SignCert, CmsSignedDataGenerator.DigestSha224);
		}

		[Test]
		public void TestSha256WithRsaEncapsulated()
		{
			EncapsulatedTest(SignKP, SignCert, CmsSignedDataGenerator.DigestSha256);
		}

		[Test]
		public void TestRipeMD128WithRsaEncapsulated()
		{
			EncapsulatedTest(SignKP, SignCert, CmsSignedDataGenerator.DigestRipeMD128);
		}

		[Test]
		public void TestRipeMD160WithRsaEncapsulated()
		{
			EncapsulatedTest(SignKP, SignCert, CmsSignedDataGenerator.DigestRipeMD160);
		}

		[Test]
		public void TestRipeMD256WithRsaEncapsulated()
		{
			EncapsulatedTest(SignKP, SignCert, CmsSignedDataGenerator.DigestRipeMD256);
		}

        [Test]
        public void TestSha224WithDsaEncapsulated()
        {
            EncapsulatedTest(SignDsaKP, SignDsaCert, CmsSignedDataGenerator.DigestSha224);
        }

        [Test]
        public void TestSha256WithDsaEncapsulated()
        {
            EncapsulatedTest(SignDsaKP, SignDsaCert, CmsSignedDataGenerator.DigestSha256);
        }

        [Test]
        public void TestSha384WithDsaEncapsulated()
        {
            EncapsulatedTest(SignDsaKP, SignDsaCert, CmsSignedDataGenerator.DigestSha384);
        }

        [Test]
        public void TestSha512WithDsaEncapsulated()
        {
            EncapsulatedTest(SignDsaKP, SignDsaCert, CmsSignedDataGenerator.DigestSha512);
        }

        [Test]
		public void TestECDsaEncapsulated()
		{
			EncapsulatedTest(SignECDsaKP, SignECDsaCert, CmsSignedDataGenerator.DigestSha1);
		}

		[Test]
		public void TestECDsaEncapsulatedSubjectKeyID()
		{
			SubjectKeyIDTest(SignECDsaKP, SignECDsaCert, CmsSignedDataGenerator.DigestSha1);
		}

		[Test]
		public void TestECDsaSha224Encapsulated()
		{
			EncapsulatedTest(SignECDsaKP, SignECDsaCert, CmsSignedDataGenerator.DigestSha224);
		}

		[Test]
		public void TestECDsaSha256Encapsulated()
		{
			EncapsulatedTest(SignECDsaKP, SignECDsaCert, CmsSignedDataGenerator.DigestSha256);
		}

		[Test]
		public void TestECDsaSha384Encapsulated()
		{
			EncapsulatedTest(SignECDsaKP, SignECDsaCert, CmsSignedDataGenerator.DigestSha384);
		}

		[Test]
		public void TestECDsaSha512Encapsulated()
		{
			EncapsulatedTest(SignECDsaKP, SignECDsaCert, CmsSignedDataGenerator.DigestSha512);
		}

		[Test]
		public void TestECDsaSha512EncapsulatedWithKeyFactoryAsEC()
		{
//			X509EncodedKeySpec  pubSpec = new X509EncodedKeySpec(_signEcDsaKP.getPublic().getEncoded());
			byte[] pubEnc = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(SignECDsaKP.Public).GetDerEncoded();
//			PKCS8EncodedKeySpec privSpec = new PKCS8EncodedKeySpec(_signEcDsaKP.Private.getEncoded());
			byte[] privEnc = PrivateKeyInfoFactory.CreatePrivateKeyInfo(SignECDsaKP.Private).GetDerEncoded();
//			KeyFactory          keyFact = KeyFactory.GetInstance("EC", "BC");
//			KeyPair             kp = new KeyPair(keyFact.generatePublic(pubSpec), keyFact.generatePrivate(privSpec));
			AsymmetricCipherKeyPair kp = new AsymmetricCipherKeyPair(
                PublicKeyFactory.CreateKey(pubEnc),
				PrivateKeyFactory.CreateKey(privEnc));

			EncapsulatedTest(kp, SignECDsaCert, CmsSignedDataGenerator.DigestSha512);
		}

		[Test]
		public void TestDsaEncapsulated()
		{
			EncapsulatedTest(SignDsaKP, SignDsaCert, CmsSignedDataGenerator.DigestSha1);
		}

		[Test]
		public void TestDsaEncapsulatedSubjectKeyID()
		{
			SubjectKeyIDTest(SignDsaKP, SignDsaCert, CmsSignedDataGenerator.DigestSha1);
		}

		[Test]
		public void TestGost3411WithGost3410Encapsulated()
		{
			EncapsulatedTest(SignGostKP, SignGostCert, CmsSignedDataGenerator.DigestGost3411);
		}

		[Test]
		public void TestGost3411WithECGost3410Encapsulated()
		{
			EncapsulatedTest(SignECGostKP, SignECGostCert, CmsSignedDataGenerator.DigestGost3411);
		}

		[Test]
		public void TestSha1WithRsaCounterSignature()
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(SignCert, OrigCert);
            IX509Store x509Crls = CmsTestUtil.MakeCrlStore(SignCrl);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(SignKP.Private, SignCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddCertificates(x509Certs);
			gen.AddCrls(x509Crls);

			CmsSignedData s = gen.Generate(msg, true);
			SignerInformation origSigner = (SignerInformation) new ArrayList(s.GetSignerInfos().GetSigners())[0];
			SignerInformationStore counterSigners1 = gen.GenerateCounterSigners(origSigner);
			SignerInformationStore counterSigners2 = gen.GenerateCounterSigners(origSigner);

			SignerInformation signer1 = SignerInformation.AddCounterSigners(origSigner, counterSigners1);
			SignerInformation signer2 = SignerInformation.AddCounterSigners(signer1, counterSigners2);

			SignerInformationStore cs = signer2.GetCounterSignatures();
			ICollection csSigners = cs.GetSigners();
			Assert.AreEqual(2, csSigners.Count);

			foreach (SignerInformation cSigner in csSigners)
			{
				ICollection certCollection = x509Certs.GetMatches(cSigner.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				Assert.IsNull(cSigner.SignedAttributes[Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtContentType]);
				Assert.IsTrue(cSigner.Verify(cert));
			}
		}

		private void rsaPssTest(
			string	digestName,
			string	digestOID)
		{
			byte[] msgBytes = Encoding.ASCII.GetBytes("Hello World!");
			CmsProcessable msg = new CmsProcessableByteArray(msgBytes);

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.EncryptionRsaPss, digestOID);
			gen.AddCertificates(x509Certs);

            CmsSignedData s = gen.Generate(CmsSignedDataGenerator.Data, msg, false);

            // compute expected content digest
            byte[] expectedDigest = DigestUtilities.CalculateDigest(digestName, msgBytes);

            VerifySignatures(s, expectedDigest);
		}

		private void SubjectKeyIDTest(
			AsymmetricCipherKeyPair	signaturePair,
			X509Certificate			signatureCert,
			string					digestAlgorithm)
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(signatureCert, OrigCert);
            IX509Store x509Crls = CmsTestUtil.MakeCrlStore(SignCrl);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(signaturePair.Private,
				CmsTestUtil.CreateSubjectKeyId(signatureCert.GetPublicKey()).GetKeyIdentifier(),
				digestAlgorithm);
			gen.AddCertificates(x509Certs);
			gen.AddCrls(x509Crls);

            CmsSignedData s = gen.Generate(msg, true);

			Assert.AreEqual(3, s.Version);

			MemoryStream bIn = new MemoryStream(s.GetEncoded(), false);
			Asn1InputStream aIn = new Asn1InputStream(bIn);

			s = new CmsSignedData(ContentInfo.GetInstance(aIn.ReadObject()));

			x509Certs = s.GetCertificates("Collection");
			x509Crls = s.GetCrls("Collection");

			SignerInformationStore signers = s.GetSignerInfos();

			foreach (SignerInformation signer in signers.GetSigners())
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				Assert.IsTrue(signer.Verify(cert));
			}

			//
			// check for CRLs
			//
			ArrayList crls = new ArrayList(x509Crls.GetMatches(null));

			Assert.AreEqual(1, crls.Count);

			Assert.IsTrue(crls.Contains(SignCrl));

			//
			// try using existing signer
			//

			gen = new CmsSignedDataGenerator();

			gen.AddSigners(s.GetSignerInfos());

			gen.AddCertificates(s.GetCertificates("Collection"));
			gen.AddCrls(s.GetCrls("Collection"));

			s = gen.Generate(msg, true);

			bIn = new MemoryStream(s.GetEncoded(), false);
			aIn = new Asn1InputStream(bIn);

			s = new CmsSignedData(ContentInfo.GetInstance(aIn.ReadObject()));

			x509Certs = s.GetCertificates("Collection");
			x509Crls = s.GetCrls("Collection");

			signers = s.GetSignerInfos();

			foreach (SignerInformation signer in signers.GetSigners())
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				Assert.IsTrue(signer.Verify(cert));
			}

			CheckSignerStoreReplacement(s, signers);
		}

		private void EncapsulatedTest(
			AsymmetricCipherKeyPair	signaturePair,
			X509Certificate			signatureCert,
			string					digestAlgorithm)
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(signatureCert, OrigCert);
            IX509Store x509Crls = CmsTestUtil.MakeCrlStore(SignCrl);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(signaturePair.Private, signatureCert, digestAlgorithm);
			gen.AddCertificates(x509Certs);
			gen.AddCrls(x509Crls);

			CmsSignedData s = gen.Generate(msg, true);

			s = new CmsSignedData(ContentInfo.GetInstance(Asn1Object.FromByteArray(s.GetEncoded())));

			x509Certs = s.GetCertificates("Collection");
			x509Crls = s.GetCrls("Collection");

			SignerInformationStore signers = s.GetSignerInfos();
			ICollection c = signers.GetSigners();

			foreach (SignerInformation signer in c)
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

                Assert.AreEqual(digestAlgorithm, signer.DigestAlgOid);

				Assert.IsTrue(signer.Verify(cert));
			}

			//
			// check for CRLs
			//
			ArrayList crls = new ArrayList(x509Crls.GetMatches(null));

			Assert.AreEqual(1, crls.Count);

			Assert.IsTrue(crls.Contains(SignCrl));

			//
			// try using existing signer
			//

			gen = new CmsSignedDataGenerator();

			gen.AddSigners(s.GetSignerInfos());

			gen.AddCertificates(s.GetCertificates("Collection"));
			gen.AddCrls(s.GetCrls("Collection"));

			s = gen.Generate(msg, true);

			s = new CmsSignedData(ContentInfo.GetInstance(Asn1Object.FromByteArray(s.GetEncoded())));

			x509Certs = s.GetCertificates("Collection");
			x509Crls = s.GetCrls("Collection");

			signers = s.GetSignerInfos();
			c = signers.GetSigners();

			foreach (SignerInformation signer in c)
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				Assert.IsTrue(signer.Verify(cert));
			}

			CheckSignerStoreReplacement(s, signers);
		}

		//
		// signerInformation store replacement test.
		//
		private void CheckSignerStoreReplacement(
			CmsSignedData orig,
			SignerInformationStore signers)
		{
			CmsSignedData s = CmsSignedData.ReplaceSigners(orig, signers);

			IX509Store x509Certs = s.GetCertificates("Collection");

			signers = s.GetSignerInfos();
			ICollection c = signers.GetSigners();

			foreach (SignerInformation signer in c)
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				Assert.IsTrue(signer.Verify(cert));
			}
		}

		[Test]
		public void TestUnsortedAttributes()
		{
			CmsSignedData s = new CmsSignedData(new CmsProcessableByteArray(disorderedMessage), disorderedSet);

			IX509Store x509Certs = s.GetCertificates("Collection");

			SignerInformationStore	signers = s.GetSignerInfos();
			ICollection				c = signers.GetSigners();

			foreach (SignerInformation signer in c)
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				Assert.IsTrue(signer.Verify(cert));
			}
		}

		[Test]
		public void TestNullContentWithSigner()
		{
            IX509Store x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddCertificates(x509Certs);

            CmsSignedData s = gen.Generate(null, false);

			s = new CmsSignedData(ContentInfo.GetInstance(Asn1Object.FromByteArray(s.GetEncoded())));

			VerifySignatures(s);
		}

		[Test]
		public void TestWithAttributeCertificate()
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(SignDsaCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddCertificates(x509Certs);

            IX509AttributeCertificate attrCert = CmsTestUtil.GetAttributeCertificate();

            IX509Store store = CmsTestUtil.MakeAttrCertStore(attrCert);

			gen.AddAttributeCertificates(store);

			CmsSignedData sd = gen.Generate(msg);

			Assert.AreEqual(4, sd.Version);

			store = sd.GetAttributeCertificates("Collection");

			ArrayList coll = new ArrayList(store.GetMatches(null));

			Assert.AreEqual(1, coll.Count);

			Assert.IsTrue(coll.Contains(attrCert));

			//
			// create new certstore
			//
            x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

			//
			// replace certs
			//
			sd = CmsSignedData.ReplaceCertificatesAndCrls(sd, x509Certs, null, null);

			VerifySignatures(sd);
		}

		[Test]
		public void TestCertStoreReplacement()
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(SignDsaCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddCertificates(x509Certs);

            CmsSignedData sd = gen.Generate(msg);

			//
			// create new certstore
			//
            x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

            //
			// replace certs
			//
			sd = CmsSignedData.ReplaceCertificatesAndCrls(sd, x509Certs, null, null);

			VerifySignatures(sd);
		}

		[Test]
		public void TestEncapsulatedCertStoreReplacement()
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(SignDsaCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddCertificates(x509Certs);

            CmsSignedData sd = gen.Generate(msg, true);

			//
			// create new certstore
			//
            x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

            //
			// replace certs
			//
			sd = CmsSignedData.ReplaceCertificatesAndCrls(sd, x509Certs, null, null);

			VerifySignatures(sd);
		}

		[Test]
		public void TestCertOrdering1()
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert, SignDsaCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddCertificates(x509Certs);

            CmsSignedData sd = gen.Generate(msg, true);

			x509Certs = sd.GetCertificates("Collection");
			ArrayList a = new ArrayList(x509Certs.GetMatches(null));

			Assert.AreEqual(3, a.Count);
			Assert.AreEqual(OrigCert, a[0]);
			Assert.AreEqual(SignCert, a[1]);
			Assert.AreEqual(SignDsaCert, a[2]);
		}

		[Test]
		public void TestCertOrdering2()
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(SignCert, SignDsaCert, OrigCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddCertificates(x509Certs);

			CmsSignedData sd = gen.Generate(msg, true);

            x509Certs = sd.GetCertificates("Collection");
            ArrayList a = new ArrayList(x509Certs.GetMatches(null));

            Assert.AreEqual(3, a.Count);
			Assert.AreEqual(SignCert, a[0]);
			Assert.AreEqual(SignDsaCert, a[1]);
			Assert.AreEqual(OrigCert, a[2]);
		}

		[Test]
		public void TestSignerStoreReplacement()
		{
			CmsProcessable msg = new CmsProcessableByteArray(Encoding.ASCII.GetBytes("Hello World!"));

            IX509Store x509Certs = CmsTestUtil.MakeCertStore(OrigCert, SignCert);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha1);
			gen.AddCertificates(x509Certs);

            CmsSignedData original = gen.Generate(msg, true);

            //
			// create new Signer
			//
			gen = new CmsSignedDataGenerator();
			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataGenerator.DigestSha224);
			gen.AddCertificates(x509Certs);

            CmsSignedData newSD = gen.Generate(msg, true);

            //
			// replace signer
			//
			CmsSignedData sd = CmsSignedData.ReplaceSigners(original, newSD.GetSignerInfos());

			IEnumerator signerEnum = sd.GetSignerInfos().GetSigners().GetEnumerator();
			signerEnum.MoveNext();
			SignerInformation signer = (SignerInformation) signerEnum.Current;

			Assert.AreEqual(CmsSignedDataGenerator.DigestSha224, signer.DigestAlgOid);

			// we use a parser here as it requires the digests to be correct in the digest set, if it
			// isn't we'll get a NullPointerException
			CmsSignedDataParser sp = new CmsSignedDataParser(sd.GetEncoded());

			sp.GetSignedContent().Drain();

			VerifySignatures(sp);
		}

		[Test]
		public void TestEncapsulatedSamples()
		{
			DoTestSample("PSSSignDataSHA1Enc.sig");
			DoTestSample("PSSSignDataSHA256Enc.sig");
			DoTestSample("PSSSignDataSHA512Enc.sig");
		}

		[Test]
		public void TestSamples()
		{
			DoTestSample("PSSSignData.data", "PSSSignDataSHA1.sig");
			DoTestSample("PSSSignData.data", "PSSSignDataSHA256.sig");
			DoTestSample("PSSSignData.data", "PSSSignDataSHA512.sig");
		}

		[Test]
		public void TestCounterSig()
		{
			CmsSignedData sig = new CmsSignedData(GetInput("counterSig.p7m"));

			SignerInformationStore ss = sig.GetSignerInfos();
			ArrayList signers = new ArrayList(ss.GetSigners());

			SignerInformationStore cs = ((SignerInformation)signers[0]).GetCounterSignatures();
			ArrayList csSigners = new ArrayList(cs.GetSigners());
			Assert.AreEqual(1, csSigners.Count);

			foreach (SignerInformation cSigner in csSigners)
			{
				ArrayList certCollection = new ArrayList(
					sig.GetCertificates("Collection").GetMatches(cSigner.SignerID));

				X509Certificate cert = (X509Certificate)certCollection[0];

				Assert.IsNull(cSigner.SignedAttributes[Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtContentType]);
				Assert.IsTrue(cSigner.Verify(cert));
			}

			VerifySignatures(sig);
		}

		private void DoTestSample(string sigName)
		{
			CmsSignedData sig = new CmsSignedData(GetInput(sigName));
			VerifySignatures(sig);
		}

        private void DoTestSample(string messageName, string sigName)
		{
			CmsSignedData sig = new CmsSignedData(
				new CmsProcessableByteArray(GetInput(messageName)),
				GetInput(sigName));

            VerifySignatures(sig);
		}

        private byte[] GetInput(string name)
		{
			return Streams.ReadAll(SimpleTest.GetTestDataAsStream("cms.sigs." + name));
		}

        [Test]
		public void TestForMultipleCounterSignatures()
		{
			CmsSignedData sd = new CmsSignedData(xtraCounterSig);

			foreach (SignerInformation sigI in sd.GetSignerInfos().GetSigners())
			{
				SignerInformationStore counter = sigI.GetCounterSignatures();
				IList sigs = new ArrayList(counter.GetSigners());

				Assert.AreEqual(2, sigs.Count);
			}
		}

		private void VerifySignatures(
			CmsSignedDataParser sp)
		{
			IX509Store x509Certs = sp.GetCertificates("Collection");
			SignerInformationStore signers = sp.GetSignerInfos();

			foreach (SignerInformation signer in signers.GetSigners())
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();
				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate)certEnum.Current;

				Assert.IsTrue(signer.Verify(cert));
			}
		}
    }
}
