using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Tsp.Tests
{
	[TestFixture]
	public class TspTest
	{
		private static AsymmetricKeyParameter privateKey;
		private static X509Certificate cert;
		private static IX509Store certs;

		static TspTest()
		{
			string signDN = "O=Bouncy Castle, C=AU";
			AsymmetricCipherKeyPair signKP = TspTestUtil.MakeKeyPair();
			X509Certificate signCert = TspTestUtil.MakeCACertificate(signKP, signDN, signKP, signDN);

			string origDN = "CN=Eric H. Echidna, E=eric@bouncycastle.org, O=Bouncy Castle, C=AU";
			AsymmetricCipherKeyPair origKP = TspTestUtil.MakeKeyPair();
			privateKey = origKP.Private;

			cert = TspTestUtil.MakeCertificate(origKP, origDN, signKP, signDN);

			IList certList = new ArrayList();
			certList.Add(cert);
			certList.Add(signCert);

			certs = X509StoreFactory.Create(
				"Certificate/Collection",
				new X509CollectionStoreParameters(certList));
		}

		[Test]
		public void TestBasic()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.Sha1, "1.2");

			tsTokenGen.SetCertificates(certs);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
			TimeStampRequest request = reqGen.Generate(TspAlgorithms.Sha1, new byte[20], BigInteger.ValueOf(100));

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed);

			TimeStampResponse tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(23), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken  tsToken = tsResp.TimeStampToken;

			tsToken.Validate(cert);

			AttributeTable table = tsToken.SignedAttributes;

			Assert.IsNotNull(table[PkcsObjectIdentifiers.IdAASigningCertificate], "no signingCertificate attribute found");
		}

		[Test]
		public void TestResponseValidation()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.MD5, "1.2");

			tsTokenGen.SetCertificates(certs);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
			TimeStampRequest request = reqGen.Generate(TspAlgorithms.Sha1, new byte[20], BigInteger.ValueOf(100));

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed);

			TimeStampResponse tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(23), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken tsToken = tsResp.TimeStampToken;

			tsToken.Validate(cert);

			//
			// check validation
			//
			tsResp.Validate(request);

			try
			{
				request = reqGen.Generate(TspAlgorithms.Sha1, new byte[20], BigInteger.ValueOf(101));

				tsResp.Validate(request);

				Assert.Fail("response validation failed on invalid nonce.");
			}
			catch (TspValidationException)
			{
				// ignore
			}

			try
			{
				request = reqGen.Generate(TspAlgorithms.Sha1, new byte[22], BigInteger.ValueOf(100));

				tsResp.Validate(request);

				Assert.Fail("response validation failed on wrong digest.");
			}
			catch (TspValidationException)
			{
				// ignore
			}

			try
			{
				request = reqGen.Generate(TspAlgorithms.MD5, new byte[20], BigInteger.ValueOf(100));

				tsResp.Validate(request);

				Assert.Fail("response validation failed on wrong digest.");
			}
			catch (TspValidationException)
			{
				// ignore
			}
		}

		[Test]
		public void TestIncorrectHash()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.Sha1, "1.2");

			tsTokenGen.SetCertificates(certs);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
			TimeStampRequest request = reqGen.Generate(TspAlgorithms.Sha1, new byte[16]);

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed);

			TimeStampResponse tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(23), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken tsToken = tsResp.TimeStampToken;

			if (tsToken != null)
			{
				Assert.Fail("incorrectHash - token not null.");
			}

			PkiFailureInfo failInfo = tsResp.GetFailInfo();

			if (failInfo == null)
			{
				Assert.Fail("incorrectHash - failInfo set to null.");
			}

			if (failInfo.IntValue != PkiFailureInfo.BadDataFormat)
			{
				Assert.Fail("incorrectHash - wrong failure info returned.");
			}
		}

		[Test]
		public void TestBadAlgorithm()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.Sha1, "1.2");

			tsTokenGen.SetCertificates(certs);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
			TimeStampRequest request = reqGen.Generate("1.2.3.4.5", new byte[20]);

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed);

			TimeStampResponse tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(23), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken  tsToken = tsResp.TimeStampToken;

			if (tsToken != null)
			{
				Assert.Fail("badAlgorithm - token not null.");
			}

			PkiFailureInfo failInfo = tsResp.GetFailInfo();

			if (failInfo == null)
			{
				Assert.Fail("badAlgorithm - failInfo set to null.");
			}

			if (failInfo.IntValue != PkiFailureInfo.BadAlg)
			{
				Assert.Fail("badAlgorithm - wrong failure info returned.");
			}
		}

		[Test]
		public void TestTimeNotAvailable()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.Sha1, "1.2");

			tsTokenGen.SetCertificates(certs);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
			TimeStampRequest request = reqGen.Generate("1.2.3.4.5", new byte[20]);

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(
				tsTokenGen, TspAlgorithms.Allowed);

			TimeStampResponse tsResp = tsRespGen.Generate(request, new BigInteger("23"), null);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken tsToken = tsResp.TimeStampToken;

			if (tsToken != null)
			{
				Assert.Fail("timeNotAvailable - token not null.");
			}

			PkiFailureInfo failInfo = tsResp.GetFailInfo();

			if (failInfo == null)
			{
				Assert.Fail("timeNotAvailable - failInfo set to null.");
			}

			if (failInfo.IntValue != PkiFailureInfo.TimeNotAvailable)
			{
				Assert.Fail("timeNotAvailable - wrong failure info returned.");
			}
		}

		[Test]
		public void TestBadPolicy()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.Sha1, "1.2");

			tsTokenGen.SetCertificates(certs);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();

			reqGen.SetReqPolicy("1.1");

			TimeStampRequest request = reqGen.Generate(TspAlgorithms.Sha1, new byte[20]);

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed, new ArrayList());

			TimeStampResponse tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(23), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken tsToken = tsResp.TimeStampToken;

			if (tsToken != null)
			{
				Assert.Fail("badPolicy - token not null.");
			}

			PkiFailureInfo  failInfo = tsResp.GetFailInfo();

			if (failInfo == null)
			{
				Assert.Fail("badPolicy - failInfo set to null.");
			}

			if (failInfo.IntValue != PkiFailureInfo.UnacceptedPolicy)
			{
				Assert.Fail("badPolicy - wrong failure info returned.");
			}
		}

		[Test]
		public void TestCertReq()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.MD5, "1.2");

			tsTokenGen.SetCertificates(certs);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();

			//
			// request with certReq false
			//
			reqGen.SetCertReq(false);

			TimeStampRequest request = reqGen.Generate(TspAlgorithms.Sha1, new byte[20], BigInteger.ValueOf(100));

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed);

			TimeStampResponse tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(23), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken tsToken = tsResp.TimeStampToken;

			Assert.IsNull(tsToken.TimeStampInfo.GenTimeAccuracy); // check for abscence of accuracy

			Assert.AreEqual("1.2", tsToken.TimeStampInfo.Policy);

			try
			{
				tsToken.Validate(cert);
			}
			catch (TspValidationException)
			{
				Assert.Fail("certReq(false) verification of token failed.");
			}

			IX509Store respCerts = tsToken.GetCertificates("Collection");

			ICollection certsColl = respCerts.GetMatches(null);

			if (certsColl.Count != 0)
			{
				Assert.Fail("certReq(false) found certificates in response.");
			}
		}

		[Test]
		public void TestTokenEncoding()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.Sha1, "1.2.3.4.5.6");

			tsTokenGen.SetCertificates(certs);

			TimeStampRequestGenerator  reqGen = new TimeStampRequestGenerator();
			TimeStampRequest           request = reqGen.Generate(TspAlgorithms.Sha1, new byte[20], BigInteger.ValueOf(100));
			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed);
			TimeStampResponse          tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(23), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampResponse tsResponse = new TimeStampResponse(tsResp.GetEncoded());

			if (!Arrays.AreEqual(tsResponse.GetEncoded(), tsResp.GetEncoded())
				|| !Arrays.AreEqual(tsResponse.TimeStampToken.GetEncoded(),
							tsResp.TimeStampToken.GetEncoded()))
			{
				Assert.Fail();
			}
		}

		[Test]
		public void TestAccuracyZeroCerts()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.MD5, "1.2");

			tsTokenGen.SetCertificates(certs);

			tsTokenGen.SetAccuracySeconds(1);
			tsTokenGen.SetAccuracyMillis(2);
			tsTokenGen.SetAccuracyMicros(3);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
			TimeStampRequest request = reqGen.Generate(TspAlgorithms.Sha1, new byte[20], BigInteger.ValueOf(100));

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed);

			TimeStampResponse tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(23), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken  tsToken = tsResp.TimeStampToken;

			tsToken.Validate(cert);

			//
			// check validation
			//
			tsResp.Validate(request);

			//
			// check tstInfo
			//
			TimeStampTokenInfo tstInfo = tsToken.TimeStampInfo;

			//
			// check accuracy
			//
			GenTimeAccuracy accuracy = tstInfo.GenTimeAccuracy;

			Assert.AreEqual(1, accuracy.Seconds);
			Assert.AreEqual(2, accuracy.Millis);
			Assert.AreEqual(3, accuracy.Micros);

			Assert.AreEqual(BigInteger.ValueOf(23), tstInfo.SerialNumber);

			Assert.AreEqual("1.2", tstInfo.Policy);

			//
			// test certReq
			//
			IX509Store store = tsToken.GetCertificates("Collection");

			ICollection certificates = store.GetMatches(null);

			Assert.AreEqual(0, certificates.Count);
		}

		[Test]
		public void TestAccuracyWithCertsAndOrdering()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.MD5, "1.2.3");

			tsTokenGen.SetCertificates(certs);

			tsTokenGen.SetAccuracySeconds(3);
			tsTokenGen.SetAccuracyMillis(1);
			tsTokenGen.SetAccuracyMicros(2);

			tsTokenGen.SetOrdering(true);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();

			reqGen.SetCertReq(true);

			TimeStampRequest request = reqGen.Generate(TspAlgorithms.Sha1, new byte[20], BigInteger.ValueOf(100));

			Assert.IsTrue(request.CertReq);

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed);

			TimeStampResponse tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(23), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken tsToken = tsResp.TimeStampToken;

			tsToken.Validate(cert);

			//
			// check validation
			//
			tsResp.Validate(request);

			//
			// check tstInfo
			//
			TimeStampTokenInfo tstInfo = tsToken.TimeStampInfo;

			//
			// check accuracy
			//
			GenTimeAccuracy accuracy = tstInfo.GenTimeAccuracy;

			Assert.AreEqual(3, accuracy.Seconds);
			Assert.AreEqual(1, accuracy.Millis);
			Assert.AreEqual(2, accuracy.Micros);

			Assert.AreEqual(BigInteger.ValueOf(23), tstInfo.SerialNumber);

			Assert.AreEqual("1.2.3", tstInfo.Policy);

			Assert.AreEqual(true, tstInfo.IsOrdered);

			Assert.AreEqual(tstInfo.Nonce, BigInteger.ValueOf(100));

			//
			// test certReq
			//
			IX509Store store = tsToken.GetCertificates("Collection");

			ICollection certificates = store.GetMatches(null);

			Assert.AreEqual(2, certificates.Count);
		}

		[Test]
		public void TestNoNonce()
		{
			TimeStampTokenGenerator tsTokenGen = new TimeStampTokenGenerator(
				privateKey, cert, TspAlgorithms.MD5, "1.2.3");

			tsTokenGen.SetCertificates(certs);

			TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
			TimeStampRequest request = reqGen.Generate(TspAlgorithms.Sha1, new byte[20]);

			Assert.IsFalse(request.CertReq);

			TimeStampResponseGenerator tsRespGen = new TimeStampResponseGenerator(tsTokenGen, TspAlgorithms.Allowed);

			TimeStampResponse tsResp = tsRespGen.Generate(request, BigInteger.ValueOf(24), DateTime.UtcNow);

			tsResp = new TimeStampResponse(tsResp.GetEncoded());

			TimeStampToken tsToken = tsResp.TimeStampToken;

			tsToken.Validate(cert);

			//
			// check validation
			//
			tsResp.Validate(request);

			//
			// check tstInfo
			//
			TimeStampTokenInfo tstInfo = tsToken.TimeStampInfo;

			//
			// check accuracy
			//
			GenTimeAccuracy accuracy = tstInfo.GenTimeAccuracy;

			Assert.IsNull(accuracy);

			Assert.AreEqual(BigInteger.ValueOf(24), tstInfo.SerialNumber);

			Assert.AreEqual("1.2.3", tstInfo.Policy);

			Assert.IsFalse(tstInfo.IsOrdered);

			Assert.IsNull(tstInfo.Nonce);

			//
			// test certReq
			//
			IX509Store store = tsToken.GetCertificates("Collection");

			ICollection certificates = store.GetMatches(null);

			Assert.AreEqual(0, certificates.Count);
		}
	}
}
