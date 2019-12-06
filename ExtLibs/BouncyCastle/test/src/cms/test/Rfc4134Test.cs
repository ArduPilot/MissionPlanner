using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
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
	public class Rfc4134Test
	{
		private static readonly byte[] exContent = GetRfc4134Data("ExContent.bin");
		private static readonly byte[] sha1 = Hex.Decode("406aec085279ba6e16022d9e0629c0229687dd48");

		[Test]
		public void Test4_1()
		{
			byte[] data = GetRfc4134Data("4.1.bin");
			CmsSignedData signedData = new CmsSignedData(data);

			VerifySignatures(signedData);

			CmsSignedDataParser parser = new CmsSignedDataParser(data);

			VerifySignatures(parser);
		}

		[Test]
		public void Test4_2()
		{
			byte[] data = GetRfc4134Data("4.2.bin");
			CmsSignedData signedData = new CmsSignedData(data);

			VerifySignatures(signedData);

			CmsSignedDataParser parser = new CmsSignedDataParser(data);

			VerifySignatures(parser);
		}

		[Test]
		public void Test4_3()
		{
			CmsProcessableByteArray unencap = new CmsProcessableByteArray(exContent);
			byte[] data = GetRfc4134Data("4.3.bin");
			CmsSignedData signedData = new CmsSignedData(unencap, data);

			VerifySignatures(signedData, sha1);

			CmsSignedDataParser parser = new CmsSignedDataParser(
				new CmsTypedStream(unencap.GetInputStream()), data);

			VerifySignatures(parser);
		}

		[Test]
		public void Test4_4()
		{
			byte[] data = GetRfc4134Data("4.4.bin");
			byte[] counterSigCert = GetRfc4134Data("AliceRSASignByCarl.cer");
			CmsSignedData signedData = new CmsSignedData(data);

			VerifySignatures(signedData, sha1);

			VerifySignerInfo4_4(GetFirstSignerInfo(signedData.GetSignerInfos()), counterSigCert);

			CmsSignedDataParser parser = new CmsSignedDataParser(data);

			VerifySignatures(parser);

			VerifySignerInfo4_4(GetFirstSignerInfo(parser.GetSignerInfos()), counterSigCert);
		}

		[Test]
		public void Test4_5()
		{
			byte[] data = GetRfc4134Data("4.5.bin");
			CmsSignedData signedData = new CmsSignedData(data);

			VerifySignatures(signedData);

			CmsSignedDataParser parser = new CmsSignedDataParser(data);

			VerifySignatures(parser);
		}

		[Test]
		public void Test4_6()
		{
			byte[] data = GetRfc4134Data("4.6.bin");
			CmsSignedData signedData = new CmsSignedData(data);

			VerifySignatures(signedData);

			CmsSignedDataParser parser = new CmsSignedDataParser(data);

			VerifySignatures(parser);
		}
		
		[Test]
		public void Test4_7()
		{
			byte[] data = GetRfc4134Data("4.7.bin");
			CmsSignedData signedData = new CmsSignedData(data);

			VerifySignatures(signedData);

			CmsSignedDataParser parser = new CmsSignedDataParser(data);

			VerifySignatures(parser);
		}

		[Test]
		public void Test5_1()
		{
			byte[] data = GetRfc4134Data("5.1.bin");
			CmsEnvelopedData envelopedData = new CmsEnvelopedData(data);

			VerifyEnvelopedData(envelopedData, CmsEnvelopedDataGenerator.DesEde3Cbc);

			CmsEnvelopedDataParser envelopedParser = new CmsEnvelopedDataParser(data);

			VerifyEnvelopedData(envelopedParser, CmsEnvelopedDataGenerator.DesEde3Cbc);
		}

		[Test]
		public void Test5_2()
		{
			byte[] data = GetRfc4134Data("5.2.bin");
			CmsEnvelopedData envelopedData = new CmsEnvelopedData(data);

			VerifyEnvelopedData(envelopedData, CmsEnvelopedDataGenerator.RC2Cbc);

			CmsEnvelopedDataParser envelopedParser = new CmsEnvelopedDataParser(data);

			VerifyEnvelopedData(envelopedParser, CmsEnvelopedDataGenerator.RC2Cbc);
		}

		private void VerifyEnvelopedData(CmsEnvelopedData envelopedData, string symAlgorithmOID)
		{
			byte[] privKeyData = GetRfc4134Data("BobPrivRSAEncrypt.pri");
			AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(privKeyData);
			Assert.IsTrue(privKey.IsPrivate);
			Assert.IsTrue(privKey is RsaKeyParameters);

			RecipientInformationStore recipients = envelopedData.GetRecipientInfos();

			Assert.AreEqual(envelopedData.EncryptionAlgOid, symAlgorithmOID);

			ArrayList c = new ArrayList(recipients.GetRecipients());
			Assert.LessOrEqual(1, c.Count);
			Assert.GreaterOrEqual(2, c.Count);

			VerifyRecipient((RecipientInformation)c[0], privKey);

			if (c.Count == 2)
			{
				RecipientInformation recInfo = (RecipientInformation)c[1];

				Assert.AreEqual(PkcsObjectIdentifiers.IdAlgCmsRC2Wrap.Id, recInfo.KeyEncryptionAlgOid);
			}
		}

		private void VerifyEnvelopedData(CmsEnvelopedDataParser envelopedParser, string symAlgorithmOID)
		{
			byte[] privKeyData = GetRfc4134Data("BobPrivRSAEncrypt.pri");
			AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(privKeyData);
			Assert.IsTrue(privKey.IsPrivate);
			Assert.IsTrue(privKey is RsaKeyParameters);

			RecipientInformationStore recipients = envelopedParser.GetRecipientInfos();

			Assert.AreEqual(envelopedParser.EncryptionAlgOid, symAlgorithmOID);

			ArrayList c = new ArrayList(recipients.GetRecipients());
			Assert.LessOrEqual(1, c.Count);
			Assert.GreaterOrEqual(2, c.Count);

			VerifyRecipient((RecipientInformation)c[0], privKey);

			if (c.Count == 2)
			{
				RecipientInformation recInfo = (RecipientInformation)c[1];

				Assert.AreEqual(PkcsObjectIdentifiers.IdAlgCmsRC2Wrap.Id, recInfo.KeyEncryptionAlgOid);
			}
		}

		private void VerifyRecipient(RecipientInformation recipient, AsymmetricKeyParameter privKey)
		{
			Assert.IsTrue(privKey.IsPrivate);

			Assert.AreEqual(recipient.KeyEncryptionAlgOid, PkcsObjectIdentifiers.RsaEncryption.Id);

			byte[] recData = recipient.GetContent(privKey);

			Assert.IsTrue(Arrays.AreEqual(exContent, recData));
		}

		private void VerifySignerInfo4_4(SignerInformation signerInfo, byte[] counterSigCert)
		{
			VerifyCounterSignature(signerInfo, counterSigCert);

			VerifyContentHint(signerInfo);
		}

		private SignerInformation GetFirstSignerInfo(SignerInformationStore store)
		{
			IEnumerator e = store.GetSigners().GetEnumerator();
			e.MoveNext();
			return (SignerInformation)e.Current;
		}

		private void VerifyCounterSignature(SignerInformation signInfo, byte[] certificate)
		{
			SignerInformation csi = GetFirstSignerInfo(signInfo.GetCounterSignatures());

			X509Certificate cert = new X509CertificateParser().ReadCertificate(certificate);

			Assert.IsTrue(csi.Verify(cert));
		}

		private void VerifyContentHint(SignerInformation signInfo)
		{
			Asn1.Cms.AttributeTable attrTable = signInfo.UnsignedAttributes;

			Asn1.Cms.Attribute attr = attrTable[CmsAttributes.ContentHint];

			Assert.AreEqual(1, attr.AttrValues.Count);
		
			Asn1EncodableVector v = new Asn1EncodableVector(
				new DerUtf8String("Content Hints Description Buffer"),
				CmsObjectIdentifiers.Data);

			Assert.IsTrue(attr.AttrValues[0].Equals(new DerSequence(v)));
		}

		private void VerifySignatures(CmsSignedData s, byte[] contentDigest)
		{
			IX509Store x509Certs = s.GetCertificates("Collection");
			IX509Store x509Crls = s.GetCrls("Collection");
			SignerInformationStore signers = s.GetSignerInfos();

			foreach (SignerInformation signer in signers.GetSigners())
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();

				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate) certEnum.Current;

				VerifySigner(signer, cert);

				if (contentDigest != null)
				{
					Assert.IsTrue(Arrays.AreEqual(contentDigest, signer.GetContentDigest()));
				}
			}

			ICollection certColl = x509Certs.GetMatches(null);
			ICollection crlColl = x509Crls.GetMatches(null);

			Assert.AreEqual(certColl.Count, s.GetCertificates("Collection").GetMatches(null).Count);
			Assert.AreEqual(crlColl.Count, s.GetCrls("Collection").GetMatches(null).Count);
		}

		private void VerifySignatures(CmsSignedData s)
		{
			VerifySignatures(s, null);
		}

		private void VerifySignatures(CmsSignedDataParser sp)
		{
	        CmsTypedStream sc = sp.GetSignedContent();
	        if (sc != null)
	        {
	            sc.Drain();
	        }
			
			IX509Store x509Certs = sp.GetCertificates("Collection");
			SignerInformationStore signers = sp.GetSignerInfos();

			foreach (SignerInformation signer in signers.GetSigners())
			{
				ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

				IEnumerator certEnum = certCollection.GetEnumerator();
				certEnum.MoveNext();
				X509Certificate cert = (X509Certificate)certEnum.Current;

				VerifySigner(signer, cert);
			}
		}

		private void VerifySigner(SignerInformation signer, X509Certificate cert)
		{
			if (cert.GetPublicKey() is DsaPublicKeyParameters)
			{
				DsaPublicKeyParameters key = (DsaPublicKeyParameters)cert.GetPublicKey();

				if (key.Parameters == null)
				{
					Assert.IsTrue(signer.Verify(GetInheritedKey(key)));
				}
				else
				{
					Assert.IsTrue(signer.Verify(cert));
				}
			}
			else
			{
				Assert.IsTrue(signer.Verify(cert));
			}
		}

		private DsaPublicKeyParameters GetInheritedKey(DsaPublicKeyParameters dsaPubKey)
		{
			X509Certificate cert = new X509CertificateParser().ReadCertificate(
				GetRfc4134Data("CarlDSSSelf.cer"));

			DsaParameters dsaParams = ((DsaPublicKeyParameters)cert.GetPublicKey()).Parameters;

			return new DsaPublicKeyParameters(dsaPubKey.Y, dsaParams);
		}

		private static byte[] GetRfc4134Data(string name)
		{
			return Streams.ReadAll(SimpleTest.GetTestDataAsStream("rfc4134." + name));
		}
	}
}
