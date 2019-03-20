using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Cms.Tests
{
	[TestFixture]
	public class MiscDataStreamTest
	{
		private const string TestMessage = "Hello World!";
		private const string SignDN = "O=Bouncy Castle, C=AU";
		private static AsymmetricCipherKeyPair signKP;
		private static X509Certificate signCert;

		private const string OrigDN = "CN=Bob, OU=Sales, O=Bouncy Castle, C=AU";
		private static AsymmetricCipherKeyPair origKP;
		private static X509Certificate origCert;

		private const string ReciDN = "CN=Doug, OU=Sales, O=Bouncy Castle, C=AU";
		//		private static AsymmetricCipherKeyPair reciKP;
		//		private static X509Certificate reciCert;

		private static AsymmetricCipherKeyPair origDsaKP;
		private static X509Certificate origDsaCert;

		private static X509Crl signCrl;
		private static X509Crl origCrl;

		private static AsymmetricCipherKeyPair SignKP
		{
			get { return signKP == null ? (signKP = CmsTestUtil.MakeKeyPair()) : signKP; }
		}

		private static AsymmetricCipherKeyPair OrigKP
		{
			get { return origKP == null ? (origKP = CmsTestUtil.MakeKeyPair()) : origKP; }
		}

		//		private static AsymmetricCipherKeyPair ReciKP
		//		{
		//			get { return reciKP == null ? (reciKP = CmsTestUtil.MakeKeyPair()) : reciKP; }
		//		}

		private static AsymmetricCipherKeyPair OrigDsaKP
		{
			get { return origDsaKP == null ? (origDsaKP = CmsTestUtil.MakeDsaKeyPair()) : origDsaKP; }
		}

		private static X509Certificate SignCert
		{
			get { return signCert == null ? (signCert = CmsTestUtil.MakeCertificate(SignKP, SignDN, SignKP, SignDN)) : signCert; }
		}

		private static X509Certificate OrigCert
		{
			get { return origCert == null ? (origCert = CmsTestUtil.MakeCertificate(OrigKP, OrigDN, SignKP, SignDN)) : origCert; }
		}

		//		private static X509Certificate ReciCert
		//		{
		//			get { return reciCert == null ? (reciCert = CmsTestUtil.MakeCertificate(ReciKP, ReciDN, SignKP, SignDN)) : reciCert; }
		//		}

		private static X509Certificate OrigDsaCert
		{
			get { return origDsaCert == null ? (origDsaCert = CmsTestUtil.MakeCertificate(OrigDsaKP, OrigDN, SignKP, SignDN)) : origDsaCert; }
		}

		private static X509Crl SignCrl
		{
			get { return signCrl == null ? (signCrl = CmsTestUtil.MakeCrl(SignKP)) : signCrl; }
		}

		private static X509Crl OrigCrl
		{
			get { return origCrl == null ? (origCrl = CmsTestUtil.MakeCrl(OrigKP)) : origCrl; }
		}

		private void VerifySignatures(
			CmsSignedDataParser	sp,
			byte[]				contentDigest)
		{
			IX509Store certStore = sp.GetCertificates("Collection");
			SignerInformationStore signers = sp.GetSignerInfos();

			foreach (SignerInformation signer in signers.GetSigners())
			{
				ICollection certCollection = certStore.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate	cert = (X509Certificate) certEnum.Current;

				Assert.IsTrue(signer.Verify(cert));

				if (contentDigest != null)
				{
					Assert.IsTrue(Arrays.AreEqual(contentDigest, signer.GetContentDigest()));
				}
			}
		}

		private void VerifySignatures(
			CmsSignedDataParser sp)
		{
			VerifySignatures(sp, null);
		}

		private void VerifyEncodedData(
			MemoryStream bOut)
		{
			CmsSignedDataParser sp = new CmsSignedDataParser(bOut.ToArray());

			sp.GetSignedContent().Drain();

			VerifySignatures(sp);

			sp.Close();
		}

		private void CheckSigParseable(byte[] sig)
		{
			CmsSignedDataParser sp = new CmsSignedDataParser(sig);
			sp.Version.ToString();
			CmsTypedStream sc = sp.GetSignedContent();
			if (sc != null)
			{
				sc.Drain();
			}
			sp.GetAttributeCertificates("Collection");
			sp.GetCertificates("Collection");
			sp.GetCrls("Collection");
			sp.GetSignerInfos();
			sp.Close();
		}

		[Test]
		public void TestSha1WithRsa()
		{
			IList certList = new ArrayList();
			IList crlList = new ArrayList();
			MemoryStream bOut = new MemoryStream();

			certList.Add(OrigCert);
			certList.Add(SignCert);

			crlList.Add(SignCrl);
			crlList.Add(OrigCrl);

			IX509Store x509Certs = X509StoreFactory.Create(
				"Certificate/Collection",
				new X509CollectionStoreParameters(certList));
			IX509Store x509Crls = X509StoreFactory.Create(
				"CRL/Collection",
				new X509CollectionStoreParameters(crlList));

			CmsSignedDataStreamGenerator gen = new CmsSignedDataStreamGenerator();

			gen.AddSigner(OrigKP.Private, OrigCert, CmsSignedDataStreamGenerator.DigestSha1);

			gen.AddCertificates(x509Certs);
			gen.AddCrls(x509Crls);

			Stream sigOut = gen.Open(bOut);

			CmsCompressedDataStreamGenerator cGen = new CmsCompressedDataStreamGenerator();

			Stream cOut = cGen.Open(sigOut, CmsCompressedDataStreamGenerator.ZLib);

			byte[] testBytes = Encoding.ASCII.GetBytes(TestMessage);
			cOut.Write(testBytes, 0, testBytes.Length);

			cOut.Close();

			sigOut.Close();

			CheckSigParseable(bOut.ToArray());

			// generate compressed stream
			MemoryStream cDataOut = new MemoryStream();
		    
			cOut = cGen.Open(cDataOut, CmsCompressedDataStreamGenerator.ZLib);

			cOut.Write(testBytes, 0, testBytes.Length);

			cOut.Close();

			CmsSignedDataParser sp = new CmsSignedDataParser(
				new CmsTypedStream(new MemoryStream(cDataOut.ToArray(), false)), bOut.ToArray());

			sp.GetSignedContent().Drain();

            byte[] cDataOutBytes = cDataOut.ToArray();

            // compute expected content digest
            byte[] hash = DigestUtilities.CalculateDigest("SHA1", cDataOutBytes);

            VerifySignatures(sp, hash);
		}
	}
}
