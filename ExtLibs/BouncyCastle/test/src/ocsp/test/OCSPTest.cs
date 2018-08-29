using System;
using System.Collections;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;

namespace Org.BouncyCastle.Ocsp.Tests
{
	[TestFixture]
	public class OcspTest
		: SimpleTest
	{
		private static readonly byte[] testResp1 = Base64.Decode(
			  "MIIFnAoBAKCCBZUwggWRBgkrBgEFBQcwAQEEggWCMIIFfjCCARehgZ8wgZwx"
			+ "CzAJBgNVBAYTAklOMRcwFQYDVQQIEw5BbmRocmEgcHJhZGVzaDESMBAGA1UE"
			+ "BxMJSHlkZXJhYmFkMQwwCgYDVQQKEwNUQ1MxDDAKBgNVBAsTA0FUQzEeMBwG"
			+ "A1UEAxMVVENTLUNBIE9DU1AgUmVzcG9uZGVyMSQwIgYJKoZIhvcNAQkBFhVv"
			+ "Y3NwQHRjcy1jYS50Y3MuY28uaW4YDzIwMDMwNDAyMTIzNDU4WjBiMGAwOjAJ"
			+ "BgUrDgMCGgUABBRs07IuoCWNmcEl1oHwIak1BPnX8QQUtGyl/iL9WJ1VxjxF"
			+ "j0hAwJ/s1AcCAQKhERgPMjAwMjA4MjkwNzA5MjZaGA8yMDAzMDQwMjEyMzQ1"
			+ "OFowDQYJKoZIhvcNAQEFBQADgYEAfbN0TCRFKdhsmvOdUoiJ+qvygGBzDxD/"
			+ "VWhXYA+16AphHLIWNABR3CgHB3zWtdy2j7DJmQ/R7qKj7dUhWLSqclAiPgFt"
			+ "QQ1YvSJAYfEIdyHkxv4NP0LSogxrumANcDyC9yt/W9yHjD2ICPBIqCsZLuLk"
			+ "OHYi5DlwWe9Zm9VFwCGgggPMMIIDyDCCA8QwggKsoAMCAQICAQYwDQYJKoZI"
			+ "hvcNAQEFBQAwgZQxFDASBgNVBAMTC1RDUy1DQSBPQ1NQMSYwJAYJKoZIhvcN"
			+ "AQkBFhd0Y3MtY2FAdGNzLWNhLnRjcy5jby5pbjEMMAoGA1UEChMDVENTMQww"
			+ "CgYDVQQLEwNBVEMxEjAQBgNVBAcTCUh5ZGVyYWJhZDEXMBUGA1UECBMOQW5k"
			+ "aHJhIHByYWRlc2gxCzAJBgNVBAYTAklOMB4XDTAyMDgyOTA3MTE0M1oXDTAz"
			+ "MDgyOTA3MTE0M1owgZwxCzAJBgNVBAYTAklOMRcwFQYDVQQIEw5BbmRocmEg"
			+ "cHJhZGVzaDESMBAGA1UEBxMJSHlkZXJhYmFkMQwwCgYDVQQKEwNUQ1MxDDAK"
			+ "BgNVBAsTA0FUQzEeMBwGA1UEAxMVVENTLUNBIE9DU1AgUmVzcG9uZGVyMSQw"
			+ "IgYJKoZIhvcNAQkBFhVvY3NwQHRjcy1jYS50Y3MuY28uaW4wgZ8wDQYJKoZI"
			+ "hvcNAQEBBQADgY0AMIGJAoGBAM+XWW4caMRv46D7L6Bv8iwtKgmQu0SAybmF"
			+ "RJiz12qXzdvTLt8C75OdgmUomxp0+gW/4XlTPUqOMQWv463aZRv9Ust4f8MH"
			+ "EJh4ekP/NS9+d8vEO3P40ntQkmSMcFmtA9E1koUtQ3MSJlcs441JjbgUaVnm"
			+ "jDmmniQnZY4bU3tVAgMBAAGjgZowgZcwDAYDVR0TAQH/BAIwADALBgNVHQ8E"
			+ "BAMCB4AwEwYDVR0lBAwwCgYIKwYBBQUHAwkwNgYIKwYBBQUHAQEEKjAoMCYG"
			+ "CCsGAQUFBzABhhpodHRwOi8vMTcyLjE5LjQwLjExMDo3NzAwLzAtBgNVHR8E"
			+ "JjAkMCKgIKAehhxodHRwOi8vMTcyLjE5LjQwLjExMC9jcmwuY3JsMA0GCSqG"
			+ "SIb3DQEBBQUAA4IBAQB6FovM3B4VDDZ15o12gnADZsIk9fTAczLlcrmXLNN4"
			+ "PgmqgnwF0Ymj3bD5SavDOXxbA65AZJ7rBNAguLUo+xVkgxmoBH7R2sBxjTCc"
			+ "r07NEadxM3HQkt0aX5XYEl8eRoifwqYAI9h0ziZfTNes8elNfb3DoPPjqq6V"
			+ "mMg0f0iMS4W8LjNPorjRB+kIosa1deAGPhq0eJ8yr0/s2QR2/WFD5P4aXc8I"
			+ "KWleklnIImS3zqiPrq6tl2Bm8DZj7vXlTOwmraSQxUwzCKwYob1yGvNOUQTq"
			+ "pG6jxn7jgDawHU1+WjWQe4Q34/pWeGLysxTraMa+Ug9kPe+jy/qRX2xwvKBZ"
//			+ "====");
			+ "");

		private static readonly byte[] testResp2 = Base64.Decode(
			  "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"
			+ "A5oyOoNv0w+8bbtMNEc1"
//			+ "====");
			+ "");

		/**
		 * extra version number encoding.
		 */
		private static readonly byte[] irregReq = Base64.Decode(
			  "MIIQpTBUoAMCAQAwTTBLMEkwCQYFKw4DAhoFAAQUIcFvFFVjPem15pKox4cfcnzF"
			+ "Kf4EFJf8OQzmVmyJ/hc4EhitQbXcqAzDAhB9ePsP19SuP6CsAgFwQuEAoIIQSzCC"
			+ "EEcwDQYJKoZIhvcNAQEFBQADgYEAlq/Tjl8OtFM8Tib1JYTiaPy9vFDr8UZhqXJI"
			+ "FyrdgtUyyDt0EcrgnBGacAeRZzF5sokIC6DjXweU7EItGqrpw/RaCUPUWFpPxR6y"
			+ "HjuzrLmICocTI9MH7dRUXm0qpxoY987sx1PtWB4pSR99ixBtq3OPNdsI0uJ+Qkei"
			+ "LbEZyvWggg+wMIIPrDCCA5owggKCoAMCAQICEEAxXx/eFe7gm/NX7AkcS68wDQYJ"
			+ "KoZIhvcNAQEFBQAwgZoxCzAJBgNVBAYTAlNFMTMwMQYDVQQKDCpMw6Ruc2bDtnJz"
			+ "w6RrcmluZ2FyIEJhbmsgQWt0aWVib2xhZyAocHVibCkxFTATBgNVBAUTDDExMTEx"
			+ "MTExMTExMTE/MD0GA1UEAww2TMOkbnNmw7Zyc8Oka3JpbmdhciBCYW5rIFB1cmNo"
			+ "YXNlciBDQTEgZm9yIEJhbmtJRCBURVNUMB4XDTA4MTAwNjIyMDAwMFoXDTEwMTAx"
			+ "MDIxNTk1OVowgZExCzAJBgNVBAYTAlNFMTMwMQYDVQQKDCpMw6Ruc2bDtnJzw6Rr"
			+ "cmluZ2FyIEJhbmsgQWt0aWVib2xhZyAocHVibCkxFTATBgNVBAUTDDExMTExMTEx"
			+ "MTExMTE2MDQGA1UEAwwtTMOkbnNmw7Zyc8Oka3JpbmdhciBCYW5rIE9DU1AgZm9y"
			+ "IEJhbmtJRCBURVNUMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC5e/h6aL2m"
			+ "DVpWeu5e5p1Ps9kbvuuGeAp9zJDYLbZz7uzT67X+s59HaViroD2+2my/gg7rX7tK"
			+ "H9VXpJad1W9O19SjfNyxgeAMwVMkrbb4IlrQwu0v/Ub8JPxSWwZZXYiODq5abeXA"
			+ "abMYIHxSaSkhrsUj1dpSAohHLJRlq707swIDAQABo2cwZTAfBgNVHSMEGDAWgBTR"
			+ "vcp2QyNdNGZ+q7TjKSrrHZqxmDATBgNVHSAEDDAKMAgGBiqFcDwBBjAOBgNVHQ8B"
			+ "Af8EBAMCBkAwHQYDVR0OBBYEFF/3557FEvkA8iiPv2XcBclxKnTdMA0GCSqGSIb3"
			+ "DQEBBQUAA4IBAQAOxRvHO89XJ0v83BZdPFzEBA4B2Tqc1oABUn13S6fAkcGWvOmG"
			+ "eY61MK16aMnLPNDadZrAqJc6PEtVY57uaywE9acwv9XpHO0bcS94tLwvZZJ2KBt0"
			+ "Oq96gaI6gnJViUjyWjm+qBZvod0QPOLGv6wUPoiNcCpSid/COTjKpLYpCJj3ZWUV"
			+ "nsTRWSRVXsdY/xI0gs/A8/c5P1PuTxoi99RTmcruoFxvV4MmhWyX7IGqG4OAtLdo"
			+ "yefz/90FPGOrmqY9OgEb+gNuTM26YDvSs1dfarPl89d8jjwxHgNbZjh2VHFqKolJ"
			+ "8TB8ZS5aNvhHPumOOE47y95rTBxrxSmGvKb8MIIENDCCAxygAwIBAgIRAJAFaeOw"
			+ "7XbxH/DN/Vvhjx8wDQYJKoZIhvcNAQEFBQAwgZUxCzAJBgNVBAYTAlNFMTMwMQYD"
			+ "VQQKDCpMw6Ruc2bDtnJzw6RrcmluZ2FyIEJhbmsgQWt0aWVib2xhZyAocHVibCkx"
			+ "FTATBgNVBAUTDDExMTExMTExMTExMTE6MDgGA1UEAwwxTMOkbnNmw7Zyc8Oka3Jp"
			+ "bmdhciBCYW5rIFJvb3QgQ0ExIGZvciBCYW5rSUQgVEVTVDAeFw0wNzEwMDExMjAw"
			+ "MzdaFw0yOTA3MDExMjAwMzdaMIGaMQswCQYDVQQGEwJTRTEzMDEGA1UECgwqTMOk"
			+ "bnNmw7Zyc8Oka3JpbmdhciBCYW5rIEFrdGllYm9sYWcgKHB1YmwpMRUwEwYDVQQF"
			+ "EwwxMTExMTExMTExMTExPzA9BgNVBAMMNkzDpG5zZsO2cnPDpGtyaW5nYXIgQmFu"
			+ "ayBQdXJjaGFzZXIgQ0ExIGZvciBCYW5rSUQgVEVTVDCCASIwDQYJKoZIhvcNAQEB"
			+ "BQADggEPADCCAQoCggEBAMK5WbYojYRX1ZKrbxJBgbd4x503LfMWgr67sVD5L0NY"
			+ "1RPhZVFJRKJWvawE5/eXJ4oNQwc831h2jiOgINXuKyGXqdAVGBcpFwIxTfzxwT4l"
			+ "fvztr8pE6wk7mLLwKUvIjbM3EF1IL3zUI3UU/U5ioyGmcb/o4GGN71kMmvV/vrkU"
			+ "02/s7xicXNxYej4ExLiCkS5+j/+3sR47Uq5cL9e8Yg7t5/6FyLGQjKoS8HU/abYN"
			+ "4kpx/oyrxzrXMhnMVDiI8QX9NYGJwI8KZ/LU6GDq/NnZ3gG5v4l4UU1GhgUbrk4I"
			+ "AZPDu99zvwCtkdj9lJN0eDv8jdyEPZ6g1qPBE0pCNqcCAwEAAaN4MHYwDwYDVR0T"
			+ "AQH/BAUwAwEB/zATBgNVHSAEDDAKMAgGBiqFcDwBBjAOBgNVHQ8BAf8EBAMCAQYw"
			+ "HwYDVR0jBBgwFoAUnkjp1bkQUOrkRiLgxpxwAe2GQFYwHQYDVR0OBBYEFNG9ynZD"
			+ "I100Zn6rtOMpKusdmrGYMA0GCSqGSIb3DQEBBQUAA4IBAQAPVSC4HEd+yCtSgL0j"
			+ "NI19U2hJeP28lAD7OA37bcLP7eNrvfU/2tuqY7rEn1m44fUbifewdgR8x2DzhM0m"
			+ "fJcA5Z12PYUb85L9z8ewGQdyHLNlMpKSTP+0lebSc/obFbteC4jjuvux60y5KVOp"
			+ "osXbGw2qyrS6uhZJrTDP1B+bYg/XBttG+i7Qzx0S5Tq//VU9OfAQZWpvejadKAk9"
			+ "WCcXq6zALiJcxsUwOHZRvvHDxkHuf5eZpPvm1gaqa+G9CtV+oysZMU1eTRasBHsB"
			+ "NRWYfOSXggsyqRHfIAVieB4VSsB8WhZYm8UgYoLhAQfSJ5Xq5cwBOHkVj33MxAyP"
			+ "c7Y5MIID/zCCAuegAwIBAgIRAOXEoBcV4gV3Z92gk5AuRgwwDQYJKoZIhvcNAQEF"
			+ "BQAwZjEkMCIGA1UECgwbRmluYW5zaWVsbCBJRC1UZWtuaWsgQklEIEFCMR8wHQYD"
			+ "VQQLDBZCYW5rSUQgTWVtYmVyIEJhbmtzIENBMR0wGwYDVQQDDBRCYW5rSUQgUm9v"
			+ "dCBDQSBURVNUMjAeFw0wNzEwMDExMTQ1NDlaFw0yOTA4MDExMTU4MjVaMIGVMQsw"
			+ "CQYDVQQGEwJTRTEzMDEGA1UECgwqTMOkbnNmw7Zyc8Oka3JpbmdhciBCYW5rIEFr"
			+ "dGllYm9sYWcgKHB1YmwpMRUwEwYDVQQFEwwxMTExMTExMTExMTExOjA4BgNVBAMM"
			+ "MUzDpG5zZsO2cnPDpGtyaW5nYXIgQmFuayBSb290IENBMSBmb3IgQmFua0lEIFRF"
			+ "U1QwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDBzn7IXIpyOGCCTuzL"
			+ "DKE/T+pFRTgFh3QgKtifZ4zxdvB2Sd5+90vUEGcGExUhzpgb9gOUrT1eE0XhdiUR"
			+ "YuYYpJI/nzPQWTsRtEaql7NHBPKnEauoA9oAhCT4pE5gLlqpTfkB8nAsRTI2XqpI"
			+ "hQ7vTvnTRx20xog21NIbz1GztV8H1kBH2eDvRX7cXGiugp6CXV/le9cB+/4TBNUN"
			+ "Xqupt79dM49KCoDuYr72W7Hv4BSWw3IInEN2m8T2X6UBpBGkCiGwLQy/+KOmYRK7"
			+ "1PSFC0rXDwOJ0HJ/8fHwx6vLMxHAQ6s/9vOW10MjgjSQlbVqH/4Pa+TlpWumSV4E"
			+ "l0z9AgMBAAGjeDB2MA8GA1UdEwEB/wQFMAMBAf8wEwYDVR0gBAwwCjAIBgYqhXA8"
			+ "AQYwDgYDVR0PAQH/BAQDAgEGMB8GA1UdIwQYMBaAFJuTMPljHcYdrRO9sEi1amb4"
			+ "tE3VMB0GA1UdDgQWBBSeSOnVuRBQ6uRGIuDGnHAB7YZAVjANBgkqhkiG9w0BAQUF"
			+ "AAOCAQEArnW/9n+G+84JOgv1Wn4tsBBS7QgJp1rdCoiNrZPx2du/7Wz3wQVNKBjL"
			+ "eMCyLjg0OVHuq4hpCv9MZpUqdcUW8gpp4dLDAAd1uE7xqVuG8g4Ir5qocxbZHQew"
			+ "fnqSJJDlEZgDeZIzod92OO+htv0MWqKWbr3Mo2Hqhn+t0+UVWsW4k44e7rUw3xQq"
			+ "r2VdMJv/C68BXUgqh3pplUDjWyXfreiACTT0q3HT6v6WaihKCa2WY9Kd1IkDcLHb"
			+ "TZk8FqMmGn72SgJw3H5Dvu7AiZijjNAUulMnMpxBEKyFTU2xRBlZZVcp50VJ2F7+"
			+ "siisxbcYOAX4GztLMlcyq921Ov/ipDCCA88wggK3oAMCAQICEQCmaX+5+m5bF5us"
			+ "CtyMq41SMA0GCSqGSIb3DQEBBQUAMGYxJDAiBgNVBAoMG0ZpbmFuc2llbGwgSUQt"
			+ "VGVrbmlrIEJJRCBBQjEfMB0GA1UECwwWQmFua0lEIE1lbWJlciBCYW5rcyBDQTEd"
			+ "MBsGA1UEAwwUQmFua0lEIFJvb3QgQ0EgVEVTVDIwHhcNMDQwODEzMDcyMDEwWhcN"
			+ "MjkwODEyMTIwMjQ2WjBmMSQwIgYDVQQKDBtGaW5hbnNpZWxsIElELVRla25payBC"
			+ "SUQgQUIxHzAdBgNVBAsMFkJhbmtJRCBNZW1iZXIgQmFua3MgQ0ExHTAbBgNVBAMM"
			+ "FEJhbmtJRCBSb290IENBIFRFU1QyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIB"
			+ "CgKCAQEA25D0f1gipbACk4Bg3t6ODUlCWOU0TWeTkzAHR7IRB5T++yvsVosedMMW"
			+ "6KYYTbPONeJSt5kydX+wZi9nVNdlhkNULLbDKWfRY7x+B9MR1Q0Kq/e4VR0uRsak"
			+ "Bv5iwEYZ7cSR63HfBaPTqQsGobq+wtGH5JeTBrmCt4A3kN1UWgX32Dv/I3m7v8bK"
			+ "iwh4cnvAD9PIOtq6pOmAkSvLvp8jCy3qFLe9KAxm8M/ZAmnxYaRV8DVEg57FGoG6"
			+ "oiG3Ixx8PSVVdzpFY4kuUFLi4ueMPwjnXFiBhhWJJeOtFG3Lc2aW3zvcDbD/MsDm"
			+ "rSZNTmtbOOou8xuMKjlNY9PU5MHIaQIDAQABo3gwdjAPBgNVHRMBAf8EBTADAQH/"
			+ "MBMGA1UdIAQMMAowCAYGKoVwPAEGMA4GA1UdDwEB/wQEAwIBBjAfBgNVHSMEGDAW"
			+ "gBSbkzD5Yx3GHa0TvbBItWpm+LRN1TAdBgNVHQ4EFgQUm5Mw+WMdxh2tE72wSLVq"
			+ "Zvi0TdUwDQYJKoZIhvcNAQEFBQADggEBAIQ4ZBHWssA38pfNzH5A+H3SXpAlI8Jc"
			+ "LuoMVOIwwbfd1Up0xopCs+Ay41v8FZtcTMFqCVTih2nzVusTgnFBPMPJ2cnTlRue"
			+ "kAtVRNsiWn2/Ool/OXoYf5YnpgYu8t9jLCBCoDS5YJg714r9V9hCwfey8TCWBU80"
			+ "vL7EIfjK13nUxf8d49GzZlFMNqGDMjfMp1FYrHBGLZBr8br/G/7em1Cprw7iR8cw"
			+ "pddz+QXXFIrIz5Y9D/x1RrwoLibPw0kMrSwI2G4aCvoBySfbD6cpnJf6YHRctdSb"
			+ "755zhdBW7XWTl6ReUVuEt0hTFms4F60kFAi5hIbDRSN1Slv5yP2b0EA=");

		public override string Name
		{
			get { return "OCSP"; }
		}

		private void doTestECDsa()
		{
			string signDN = "O=Bouncy Castle, C=AU";
			AsymmetricCipherKeyPair signKP = OcspTestUtil.MakeECKeyPair();
			X509Certificate testCert = OcspTestUtil.MakeECDsaCertificate(signKP, signDN, signKP, signDN);

			string origDN = "CN=Eric H. Echidna, E=eric@bouncycastle.org, O=Bouncy Castle, C=AU";
			GeneralName origName = new GeneralName(new X509Name(origDN));

			//
			// general id value for our test issuer cert and a serial number.
			//
			CertificateID id = new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One);

			//
			// basic request generation
			//
			OcspReqGenerator gen = new OcspReqGenerator();

			gen.AddRequest(id);

			OcspReq req = gen.Generate();

			if (req.IsSigned)
			{
				Fail("signed but shouldn't be");
			}

			X509Certificate[] certs = req.GetCerts();

			if (certs != null)
			{
				Fail("null certs expected, but not found");
			}

			Req[] requests = req.GetRequestList();

			if (!requests[0].GetCertID().Equals(id))
			{
				Fail("Failed isFor test");
			}

			//
			// request generation with signing
			//
			X509Certificate[] chain = new X509Certificate[1];

			gen = new OcspReqGenerator();

			gen.SetRequestorName(new GeneralName(GeneralName.DirectoryName, new X509Name("CN=fred")));

			gen.AddRequest(new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One));

			chain[0] = testCert;

			req = gen.Generate("SHA1withECDSA", signKP.Private, chain);

			if (!req.IsSigned)
			{
				Fail("not signed but should be");
			}

			if (!req.Verify(signKP.Public))
			{
				Fail("signature failed to Verify");
			}

			requests = req.GetRequestList();

			if (!requests[0].GetCertID().Equals(id))
			{
				Fail("Failed isFor test");
			}

			certs = req.GetCerts();

			if (certs == null)
			{
				Fail("null certs found");
			}

			if (certs.Length != 1 || !certs[0].Equals(testCert))
			{
				Fail("incorrect certs found in request");
			}

			//
			// encoding test
			//
			byte[] reqEnc = req.GetEncoded();

			OcspReq newReq = new OcspReq(reqEnc);

			if (!newReq.Verify(signKP.Public))
			{
				Fail("newReq signature failed to Verify");
			}

			//
			// request generation with signing and nonce
			//
			chain = new X509Certificate[1];

			gen = new OcspReqGenerator();

			IList oids = new ArrayList();
			IList values = new ArrayList();
			byte[] sampleNonce = new byte[16];
			Random rand = new Random();

			rand.NextBytes(sampleNonce);

			gen.SetRequestorName(new GeneralName(GeneralName.DirectoryName, new X509Name("CN=fred")));

			oids.Add(OcspObjectIdentifiers.PkixOcspNonce);
			values.Add(new X509Extension(false, new DerOctetString(new DerOctetString(sampleNonce))));

			gen.SetRequestExtensions(new X509Extensions(oids, values));

			gen.AddRequest(new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One));

			chain[0] = testCert;

			req = gen.Generate("SHA1withECDSA", signKP.Private, chain);

			if (!req.IsSigned)
			{
				Fail("not signed but should be");
			}

			if (!req.Verify(signKP.Public))
			{
				Fail("signature failed to Verify");
			}

			//
			// extension check.
			//
			ISet extOids = req.GetCriticalExtensionOids();

			if (extOids.Count != 0)
			{
				Fail("wrong number of critical extensions in OCSP request.");
			}

			extOids = req.GetNonCriticalExtensionOids();

			if (extOids.Count != 1)
			{
				Fail("wrong number of non-critical extensions in OCSP request.");
			}

			Asn1OctetString extValue = req.GetExtensionValue(OcspObjectIdentifiers.PkixOcspNonce);

			Asn1Encodable extObj = X509ExtensionUtilities.FromExtensionValue(extValue);

			if (!(extObj is Asn1OctetString))
			{
				Fail("wrong extension type found.");
			}

			if (!AreEqual(((Asn1OctetString)extObj).GetOctets(), sampleNonce))
			{
				Fail("wrong extension value found.");
			}

			//
			// request list check
			//
			requests = req.GetRequestList();

			if (!requests[0].GetCertID().Equals(id))
			{
				Fail("Failed isFor test");
			}

			//
			// response generation
			//
			BasicOcspRespGenerator respGen = new BasicOcspRespGenerator(signKP.Public);

			respGen.AddResponse(id, CertificateStatus.Good);

			respGen.Generate("SHA1withECDSA", signKP.Private, chain, DateTime.UtcNow);
		}

		private void doTestRsa()
		{
			string signDN = "O=Bouncy Castle, C=AU";
			AsymmetricCipherKeyPair signKP = OcspTestUtil.MakeKeyPair();
			X509Certificate testCert = OcspTestUtil.MakeCertificate(signKP, signDN, signKP, signDN);

			string origDN = "CN=Eric H. Echidna, E=eric@bouncycastle.org, O=Bouncy Castle, C=AU";
			GeneralName origName = new GeneralName(new X509Name(origDN));

			//
			// general id value for our test issuer cert and a serial number.
			//
			CertificateID id = new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One);

			//
			// basic request generation
			//
			OcspReqGenerator gen = new OcspReqGenerator();

			gen.AddRequest(
				new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One));

			OcspReq req = gen.Generate();

			if (req.IsSigned)
			{
				Fail("signed but shouldn't be");
			}

			X509Certificate[] certs = req.GetCerts();

			if (certs != null)
			{
				Fail("null certs expected, but not found");
			}

			Req[] requests = req.GetRequestList();

			if (!requests[0].GetCertID().Equals(id))
			{
				Fail("Failed isFor test");
			}

			//
			// request generation with signing
			//
			X509Certificate[] chain = new X509Certificate[1];

			gen = new OcspReqGenerator();

			gen.SetRequestorName(new GeneralName(GeneralName.DirectoryName, new X509Name("CN=fred")));

			gen.AddRequest(
				new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One));

			chain[0] = testCert;

			req = gen.Generate("SHA1withRSA", signKP.Private, chain);

			if (!req.IsSigned)
			{
				Fail("not signed but should be");
			}

			if (!req.Verify(signKP.Public))
			{
				Fail("signature failed to Verify");
			}

			requests = req.GetRequestList();

			if (!requests[0].GetCertID().Equals(id))
			{
				Fail("Failed isFor test");
			}

			certs = req.GetCerts();

			if (certs == null)
			{
				Fail("null certs found");
			}

			if (certs.Length != 1 || !certs[0].Equals(testCert))
			{
				Fail("incorrect certs found in request");
			}

			//
			// encoding test
			//
			byte[] reqEnc = req.GetEncoded();

			OcspReq newReq = new OcspReq(reqEnc);

			if (!newReq.Verify(signKP.Public))
			{
				Fail("newReq signature failed to Verify");
			}

			//
			// request generation with signing and nonce
			//
			chain = new X509Certificate[1];

			gen = new OcspReqGenerator();

			IList oids = new ArrayList();
			IList values = new ArrayList();
			byte[] sampleNonce = new byte[16];
			Random rand = new Random();

			rand.NextBytes(sampleNonce);

			gen.SetRequestorName(new GeneralName(GeneralName.DirectoryName, new X509Name("CN=fred")));

			oids.Add(OcspObjectIdentifiers.PkixOcspNonce);
			values.Add(new X509Extension(false, new DerOctetString(new DerOctetString(sampleNonce))));

			gen.SetRequestExtensions(new X509Extensions(oids, values));

			gen.AddRequest(
				new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One));

			chain[0] = testCert;

			req = gen.Generate("SHA1withRSA", signKP.Private, chain);

			if (!req.IsSigned)
			{
				Fail("not signed but should be");
			}

			if (!req.Verify(signKP.Public))
			{
				Fail("signature failed to Verify");
			}

			//
			// extension check.
			//
			ISet extOids = req.GetCriticalExtensionOids();

			if (extOids.Count != 0)
			{
				Fail("wrong number of critical extensions in OCSP request.");
			}

			extOids = req.GetNonCriticalExtensionOids();

			if (extOids.Count != 1)
			{
				Fail("wrong number of non-critical extensions in OCSP request.");
			}

			Asn1OctetString extValue = req.GetExtensionValue(OcspObjectIdentifiers.PkixOcspNonce);

			Asn1Object extObj = X509ExtensionUtilities.FromExtensionValue(extValue);

			if (!(extObj is Asn1OctetString))
			{
				Fail("wrong extension type found.");
			}

			if (!AreEqual(((Asn1OctetString)extObj).GetOctets(), sampleNonce))
			{
				Fail("wrong extension value found.");
			}

			//
			// request list check
			//
			requests = req.GetRequestList();

			if (!requests[0].GetCertID().Equals(id))
			{
				Fail("Failed isFor test");
			}

			//
			// response generation
			//
			BasicOcspRespGenerator respGen = new BasicOcspRespGenerator(signKP.Public);

			respGen.AddResponse(id, CertificateStatus.Good);

			BasicOcspResp resp = respGen.Generate("SHA1withRSA", signKP.Private, chain, DateTime.UtcNow);
			OCSPRespGenerator rGen = new OCSPRespGenerator();

			byte[] enc = rGen.Generate(OCSPRespGenerator.Successful, resp).GetEncoded();
		}

		private void doTestIrregularVersionReq()
		{
			OcspReq ocspRequest = new OcspReq(irregReq);
			X509Certificate cert = ocspRequest.GetCerts()[0];
			if (!ocspRequest.Verify(cert.GetPublicKey()))
			{
				Fail("extra version encoding test failed");
			}
		}

		public override void PerformTest()
		{
			string signDN = "O=Bouncy Castle, C=AU";
			AsymmetricCipherKeyPair signKP = OcspTestUtil.MakeKeyPair();
			X509Certificate testCert = OcspTestUtil.MakeCertificate(signKP, signDN, signKP, signDN);

			string origDN = "CN=Eric H. Echidna, E=eric@bouncycastle.org, O=Bouncy Castle, C=AU";
			GeneralName origName = new GeneralName(new X509Name(origDN));

			//
			// general id value for our test issuer cert and a serial number.
			//
			CertificateID   id = new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One);

			//
			// basic request generation
			//
			OcspReqGenerator gen = new OcspReqGenerator();

			gen.AddRequest(
				new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One));

			OcspReq req = gen.Generate();

			if (req.IsSigned)
			{
				Fail("signed but shouldn't be");
			}

			X509Certificate[] certs = req.GetCerts();

			if (certs != null)
			{
				Fail("null certs expected, but not found");
			}

			Req[] requests = req.GetRequestList();

			if (!requests[0].GetCertID().Equals(id))
			{
				Fail("Failed isFor test");
			}

			//
			// request generation with signing
			//
			X509Certificate[] chain = new X509Certificate[1];

			gen = new OcspReqGenerator();

			gen.SetRequestorName(new GeneralName(GeneralName.DirectoryName, new X509Name("CN=fred")));

			gen.AddRequest(
				new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One));

			chain[0] = testCert;

			req = gen.Generate("SHA1withRSA", signKP.Private, chain);

			if (!req.IsSigned)
			{
				Fail("not signed but should be");
			}

			if (!req.Verify(signKP.Public))
			{
				Fail("signature failed to Verify");
			}

			requests = req.GetRequestList();

			if (!requests[0].GetCertID().Equals(id))
			{
				Fail("Failed isFor test");
			}

			certs = req.GetCerts();

			if (certs == null)
			{
				Fail("null certs found");
			}

			if (certs.Length != 1 || !testCert.Equals(certs[0]))
			{
				Fail("incorrect certs found in request");
			}

			//
			// encoding test
			//
			byte[] reqEnc = req.GetEncoded();

			OcspReq newReq = new OcspReq(reqEnc);

			if (!newReq.Verify(signKP.Public))
			{
				Fail("newReq signature failed to Verify");
			}

			//
			// request generation with signing and nonce
			//
			chain = new X509Certificate[1];

			gen = new OcspReqGenerator();

			IList oids = new ArrayList();
			IList values = new ArrayList();
			byte[] sampleNonce = new byte[16];
			Random rand = new Random();

			rand.NextBytes(sampleNonce);

			gen.SetRequestorName(new GeneralName(GeneralName.DirectoryName, new X509Name("CN=fred")));

			oids.Add(OcspObjectIdentifiers.PkixOcspNonce);
			values.Add(new X509Extension(false, new DerOctetString(new DerOctetString(sampleNonce))));

			gen.SetRequestExtensions(new X509Extensions(oids, values));

			gen.AddRequest(
				new CertificateID(CertificateID.HashSha1, testCert, BigInteger.One));

			chain[0] = testCert;

			req = gen.Generate("SHA1withRSA", signKP.Private, chain);

			if (!req.IsSigned)
			{
				Fail("not signed but should be");
			}

			if (!req.Verify(signKP.Public))
			{
				Fail("signature failed to Verify");
			}

			//
			// extension check.
			//
			ISet extOids = req.GetCriticalExtensionOids();

			if (extOids.Count != 0)
			{
				Fail("wrong number of critical extensions in OCSP request.");
			}

			extOids = req.GetNonCriticalExtensionOids();

			if (extOids.Count != 1)
			{
				Fail("wrong number of non-critical extensions in OCSP request.");
			}

			Asn1OctetString extValue = req.GetExtensionValue(OcspObjectIdentifiers.PkixOcspNonce);
			Asn1Object extObj = X509ExtensionUtilities.FromExtensionValue(extValue);

			if (!(extObj is Asn1OctetString))
			{
				Fail("wrong extension type found.");
			}

			byte[] compareNonce = ((Asn1OctetString) extObj).GetOctets();

			if (!AreEqual(compareNonce, sampleNonce))
			{
				Fail("wrong extension value found.");
			}

			//
			// request list check
			//
			requests = req.GetRequestList();

			if (!requests[0].GetCertID().Equals(id))
			{
				Fail("Failed isFor test");
			}

			//
			// response parsing - test 1
			//
			OcspResp response = new OcspResp(testResp1);

			if (response.Status != 0)
			{
				Fail("response status not zero.");
			}

			BasicOcspResp brep = (BasicOcspResp) response.GetResponseObject();
			chain = brep.GetCerts();

			if (!brep.Verify(chain[0].GetPublicKey()))
			{
				Fail("response 1 failed to Verify.");
			}

			//
			// test 2
			//
			SingleResp[] singleResp = brep.Responses;

			response = new OcspResp(testResp2);

			if (response.Status != 0)
			{
				Fail("response status not zero.");
			}

			brep = (BasicOcspResp)response.GetResponseObject();
			chain = brep.GetCerts();

			if (!brep.Verify(chain[0].GetPublicKey()))
			{
				Fail("response 2 failed to Verify.");
			}

			singleResp = brep.Responses;

			//
			// simple response generation
			//
			OCSPRespGenerator respGen = new OCSPRespGenerator();
			OcspResp resp = respGen.Generate(OCSPRespGenerator.Successful, response.GetResponseObject());

			if (!resp.GetResponseObject().Equals(response.GetResponseObject()))
			{
				Fail("response fails to match");
			}

			doTestECDsa();
			doTestRsa();
			doTestIrregularVersionReq();
		}

		public static void Main(
			string[] args)
		{
			RunTest(new OcspTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
