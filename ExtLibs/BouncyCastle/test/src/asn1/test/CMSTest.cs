using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class CmsTest
		: ITest
	{
		//
		// compressed data object
		//
		private static readonly byte[] compData = Base64.Decode(
			"MIAGCyqGSIb3DQEJEAEJoIAwgAIBADANBgsqhkiG9w0BCRADCDCABgkqhkiG9w0BBwGggCSABIIC"
			+ "Hnic7ZRdb9owFIbvK/k/5PqVYPFXGK12YYyboVFASSp1vQtZGiLRACZE49/XHoUW7S/0tXP8Efux"
			+ "fU5ivWnasml72XFb3gb5druui7ytN803M570nii7C5r8tfwR281hy/p/KSM3+jzH5s3+pbQ90xSb"
			+ "P3VT3QbLusnt8WPIuN5vN/vaA2+DulnXTXkXvNTr8j8ouZmkCmGI/UW+ZS/C8zP0bz2dz0zwLt+1"
			+ "UEk2M8mlaxjRMByAhZTj0RGYg4TvogiRASROsZgjpVcJCb1KV6QzQeDJ1XkoQ5Jm+C5PbOHZZGRi"
			+ "v+ORAcshOGeCcdFJyfgFxdtCdEcmOrbinc/+BBMzRThEYpwl+jEBpciSGWQkI0TSlREmD/eOHb2D"
			+ "SGLuESm/iKUFt1y4XHBO2a5oq0IKJKWLS9kUZTA7vC5LSxYmgVL46SIWxIfWBQd6AdrnjLmH94UT"
			+ "vGxVibLqRCtIpp4g2qpdtqK1LiOeolpVK5wVQ5P7+QjZAlrh0cePYTx/gNZuB9Vhndtgujl9T/tg"
			+ "W9ogK+3rnmg3YWygnTuF5GDS+Q/jIVLnCcYZFc6Kk/+c80wKwZjwdZIqDYWRH68MuBQSXLgXYXj2"
			+ "3CAaYOBNJMliTl0X7eV5DnoKIFSKYdj3cRpD/cK/JWTHJRe76MUXnfBW8m7Hd5zhQ4ri2NrVF/WL"
			+ "+kV1/3AGSlJ32bFPd2BsQD8uSzIx6lObkjdz95c0AAAAAAAAAAAAAAAA");

		//
		// enveloped data
		//
		private static readonly byte[] envDataKeyTrns = Base64.Decode(
			  "MIAGCSqGSIb3DQEHA6CAMIACAQAxgcQwgcECAQAwKjAlMRYwFAYDVQQKEw1Cb3Vu"
			+ "Y3kgQ2FzdGxlMQswCQYDVQQGEwJBVQIBCjANBgkqhkiG9w0BAQEFAASBgC5vdGrB"
			+ "itQSGwifLf3KwPILjaB4WEXgT/IIO1KDzrsbItCJsMA0Smq2y0zptxT0pSRL6JRg"
			+ "NMxLk1ySnrIrvGiEPLMR1zjxlT8yQ6VLX+kEoK43ztd1aaLw0oBfrcXcLN7BEpZ1"
			+ "TIdjlBfXIOx1S88WY1MiYqJJFc3LMwRUaTEDMIAGCSqGSIb3DQEHATAdBglghkgB"
			+ "ZQMEARYEEAfxLMWeaBOTTZQwUq0Y5FuggAQgwOJhL04rjSZCBCSOv5i5XpFfGsOd"
			+ "YSHSqwntGpFqCx4AAAAAAAAAAAAA");

		private static readonly byte[] envDataKEK = Base64.Decode(
			  "MIAGCSqGSIb3DQEHA6CAMIACAQIxUqJQAgEEMAcEBQECAwQFMBAGCyqGSIb3DQEJE"
			+ "AMHAgE6BDC7G/HyUPilIrin2Yeajqmj795VoLWETRnZAAFcAiQdoQWyz+oCh6WY/H"
			+ "jHHi+0y+cwgAYJKoZIhvcNAQcBMBQGCCqGSIb3DQMHBAiY3eDBBbF6naCABBiNdzJb"
			+ "/v6+UZB3XXKipxFDUpz9GyjzB+gAAAAAAAAAAAAA");

		private static readonly byte[] envDataNestedNDEF = Base64.Decode(
			  "MIAGCSqGSIb3DQEHA6CAMIACAQAxge8wgewCAQAwgZUwgY8xKDAmBgNVBAoMH1RoZSBMZWdpb24g"
			+ "b2YgdGhlIEJvdW5jeSBDYXN0bGUxLzAtBgkqhkiG9w0BCQEWIGZlZWRiYWNrLWNyeXB0b0Bib3Vu"
			+ "Y3ljYXN0bGUub3JnMREwDwYDVQQIDAhWaWN0b3JpYTESMBAGA1UEBwwJTWVsYm91cm5lMQswCQYD"
			+ "VQQGEwJBVQIBATANBgkqhkiG9w0BAQEFAARABIXMd8xiTyWDKO/LQfvdGYTPW3I9oSQWwtm4OIaN"
			+ "VINpfY2lfwTvbmE6VXiLKeALC0dMBV8z7DEM9hE0HVmvLDCABgkqhkiG9w0BBwEwHQYJYIZIAWUD"
			+ "BAECBBB32ko6WrVxDTqwUYEpV6IUoIAEggKgS6RowrhNlmWWI13zxD/lryxkZ5oWXPUfNiUxYX/P"
			+ "r5iscW3s8VKJKUpJ4W5SNA7JGL4l/5LmSnJ4Qu/xzxcoH4r4vmt75EDE9p2Ob2Xi1NuSFAZubJFc"
			+ "Zlnp4e05UHKikmoaz0PbiAi277sLQlK2FcVsntTYVT00y8+IwuuQu0ATVqkXC+VhfjV/sK6vQZnw"
			+ "2rQKedZhLB7B4dUkmxCujb/UAq4lgSpLMXg2P6wMimTczXyQxRiZxPeI4ByCENjkafXbfcJft2eD"
			+ "gv1DEDdYM5WrW9Z75b4lmJiOJ/xxDniHCvum7KGXzpK1d1mqTlpzPC2xoz08/MO4lRf5Mb0bYdq6"
			+ "CjMaYqVwGsYryp/2ayX+d8H+JphEG+V9Eg8uPcDoibwhDI4KkoyGHstPw5bxcy7vVFt7LXUdNjJc"
			+ "K1wxaUKEXDGKt9Vj93FnBTLMX0Pc9HpueV5o1ipX34dn/P3HZB9XK8ScbrE38B1VnIgylStnhVFO"
			+ "Cj9s7qSVqI2L+xYHJRHsxaMumIRnmRuOqdXDfIo28EZAnFtQ/b9BziMGVvAW5+A8h8s2oazhSmK2"
			+ "23ftV7uv98ScgE8fCd3PwT1kKJM83ThTYyBzokvMfPYCCvsonMV+kTWXhWcwjYTS4ukrpR452ZdW"
			+ "l3aJqDnzobt5FK4T8OGciOj+1PxYFZyRmCuafm2Dx6o7Et2Tu/T5HYvhdY9jHyqtDl2PXH4CTnVi"
			+ "gA1YOAArjPVmsZVwAM3Ml46uyXXhcsXwQ1X0Tv4D+PSa/id4UQ2cObOw8Cj1eW2GB8iJIZVqkZaU"
			+ "XBexqgWYOIoxjqODSeoZKiBsTK3c+oOUBqBDueY1i55swE2o6dDt95FluX6iyr/q4w2wLt3upY1J"
			+ "YL+TuvZxAKviuAczMS1bAAAAAAAAAAAAAA==");

		//
		// signed data
		//
		private static readonly byte[] signedData = Base64.Decode(
			  "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"
			+ "a+fEv4J8wQ8Xwvk0C8iMAAAAAAAA");

		private static void Touch(object o)
		{
		}

		private ITestResult CompressionTest()
		{
			try
			{
				ContentInfo info = ContentInfo.GetInstance(
					Asn1Object.FromByteArray(compData));
				CompressedData data = CompressedData.GetInstance(info.Content);

				data = new CompressedData(data.CompressionAlgorithmIdentifier, data.EncapContentInfo);
				info = new ContentInfo(CmsObjectIdentifiers.CompressedData, data);

				if (!Arrays.AreEqual(info.GetEncoded(), compData))
				{
					return new SimpleTestResult(false, Name + ": CMS compression failed to re-encode");
				}

				return new SimpleTestResult(true, Name + ": Okay");
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": CMS compression failed - " + e.ToString(), e);
			}
		}

		private ITestResult EnvelopedTest()
		{
			try
			{
				// Key trans
				ContentInfo info = ContentInfo.GetInstance(
					Asn1Object.FromByteArray(envDataKeyTrns));
				EnvelopedData envData = EnvelopedData.GetInstance(info.Content);
				Asn1Set s = envData.RecipientInfos;

				if (s.Count != 1)
				{
					return new SimpleTestResult(false, Name + ": CMS KeyTrans enveloped, wrong number of recipients");
				}

				RecipientInfo recip = RecipientInfo.GetInstance(s[0]);

				if (recip.Info is KeyTransRecipientInfo)
				{
					KeyTransRecipientInfo inf = KeyTransRecipientInfo.GetInstance(recip.Info);

					inf = new KeyTransRecipientInfo(inf.RecipientIdentifier, inf.KeyEncryptionAlgorithm, inf.EncryptedKey);

					s = new DerSet(new RecipientInfo(inf));
				}
				else
				{
					return new SimpleTestResult(false, Name + ": CMS KeyTrans enveloped, wrong recipient type");
				}

				envData = new EnvelopedData(envData.OriginatorInfo, s, envData.EncryptedContentInfo, envData.UnprotectedAttrs);
				info = new ContentInfo(CmsObjectIdentifiers.EnvelopedData, envData);

				if (!Arrays.AreEqual(info.GetEncoded(), envDataKeyTrns))
				{
					return new SimpleTestResult(false, Name + ": CMS KeyTrans enveloped failed to re-encode");
				}


				// KEK
				info = ContentInfo.GetInstance(
					Asn1Object.FromByteArray(envDataKEK));
				envData = EnvelopedData.GetInstance(info.Content);
				s = envData.RecipientInfos;

				if (s.Count != 1)
				{
					return new SimpleTestResult(false, Name + ": CMS KEK enveloped, wrong number of recipients");
				}

				recip = RecipientInfo.GetInstance(s[0]);

				if (recip.Info is KekRecipientInfo)
				{
					KekRecipientInfo inf = KekRecipientInfo.GetInstance(recip.Info);

					inf = new KekRecipientInfo(inf.KekID, inf.KeyEncryptionAlgorithm, inf.EncryptedKey);

					s = new DerSet(new RecipientInfo(inf));
				}
				else
				{
					return new SimpleTestResult(false, Name + ": CMS KEK enveloped, wrong recipient type");
				}

				envData = new EnvelopedData(envData.OriginatorInfo, s, envData.EncryptedContentInfo, envData.UnprotectedAttrs);
				info = new ContentInfo(CmsObjectIdentifiers.EnvelopedData, envData);

				if (!Arrays.AreEqual(info.GetEncoded(), envDataKEK))
				{
					return new SimpleTestResult(false, Name + ": CMS KEK enveloped failed to re-encode");
				}

				// Nested NDEF problem
				Asn1StreamParser asn1In = new Asn1StreamParser(new MemoryStream(envDataNestedNDEF, false));
				ContentInfoParser ci = new ContentInfoParser((Asn1SequenceParser)asn1In.ReadObject());
				EnvelopedDataParser ed = new EnvelopedDataParser((Asn1SequenceParser)ci
					.GetContent(Asn1Tags.Sequence));
				Touch(ed.Version);
				ed.GetOriginatorInfo();
				ed.GetRecipientInfos().ToAsn1Object();
				EncryptedContentInfoParser eci = ed.GetEncryptedContentInfo();
				Touch(eci.ContentType);
				Touch(eci.ContentEncryptionAlgorithm);

				Stream dataIn = ((Asn1OctetStringParser)eci.GetEncryptedContent(Asn1Tags.OctetString))
					.GetOctetStream();
				Streams.Drain(dataIn);
				dataIn.Close();

				// Test data doesn't have unprotected attrs, bug was being thrown by this call
				Asn1SetParser upa = ed.GetUnprotectedAttrs();
				if (upa != null)
				{
					upa.ToAsn1Object();
				}

				return new SimpleTestResult(true, Name + ": Okay");
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": CMS enveloped failed - " + e.ToString(), e);
			}
		}

		private ITestResult SignedTest()
		{
			try
			{
				ContentInfo info = ContentInfo.GetInstance(
					Asn1Object.FromByteArray(signedData));
				SignedData sData = SignedData.GetInstance(info.Content);

				sData = new SignedData(sData.DigestAlgorithms, sData.EncapContentInfo, sData.Certificates, sData.CRLs, sData.SignerInfos);
				info = new ContentInfo(CmsObjectIdentifiers.SignedData, sData);

				if (!Arrays.AreEqual(info.GetEncoded(), signedData))
				{
					return new SimpleTestResult(false, Name + ": CMS signed failed to re-encode");
				}

				return new SimpleTestResult(true, Name + ": Okay");
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": CMS signed failed - " + e.ToString(), e);
			}
		}

		public ITestResult Perform()
		{
			ITestResult res = CompressionTest();

			if (!res.IsSuccessful())
			{
				return res;
			}

			res = EnvelopedTest();
			if (!res.IsSuccessful())
			{
				return res;
			}

			return SignedTest();
		}

		public string Name
		{
			get { return "CMS"; }
		}

		public static void Main(
			string[] args)
		{
			ITest test = new CmsTest();
			ITestResult result = test.Perform();

			Console.WriteLine(result);
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
