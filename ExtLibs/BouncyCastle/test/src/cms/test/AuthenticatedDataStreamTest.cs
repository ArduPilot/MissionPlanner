using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms.Tests
{
	[TestFixture]
	public class AuthenticatedDataStreamTest
	{
		private const string SignDN = "O=Bouncy Castle, C=AU";

		private static AsymmetricCipherKeyPair signKP;
//		private static X509Certificate signCert;
		//signCert = CmsTestUtil.MakeCertificate(_signKP, SignDN, _signKP, SignDN);

//		private const string OrigDN = "CN=Bob, OU=Sales, O=Bouncy Castle, C=AU";

//		private static AsymmetricCipherKeyPair origKP;
		//origKP = CmsTestUtil.MakeKeyPair();
//		private static X509Certificate origCert;
		//origCert = CmsTestUtil.MakeCertificate(_origKP, OrigDN, _signKP, SignDN);

		private const string ReciDN = "CN=Doug, OU=Sales, O=Bouncy Castle, C=AU";

		private static AsymmetricCipherKeyPair reciKP;
		private static X509Certificate reciCert;

		private static AsymmetricCipherKeyPair origECKP;
		private static AsymmetricCipherKeyPair reciECKP;
		private static X509Certificate reciECCert;

		private static AsymmetricCipherKeyPair SignKP
		{
			get { return signKP == null ? (signKP = CmsTestUtil.MakeKeyPair()) : signKP; }
		}

		private static AsymmetricCipherKeyPair ReciKP
		{
			get { return reciKP == null ? (reciKP = CmsTestUtil.MakeKeyPair()) : reciKP; }
		}

		private static X509Certificate ReciCert
		{
			get { return reciCert == null ? (reciCert = CmsTestUtil.MakeCertificate(ReciKP, ReciDN, SignKP, SignDN)) : reciCert; }
		}

		private static AsymmetricCipherKeyPair OrigECKP
		{
			get { return origECKP == null ? (origECKP = CmsTestUtil.MakeECDsaKeyPair()) : origECKP; }
		}

		private static AsymmetricCipherKeyPair ReciECKP
		{
			get { return reciECKP == null ? (reciECKP = CmsTestUtil.MakeECDsaKeyPair()) : reciECKP; }
		}

		private static X509Certificate ReciECCert
		{
			get { return reciECCert == null ? (reciECCert = CmsTestUtil.MakeCertificate(ReciECKP, ReciDN, SignKP, SignDN)) : reciECCert; }
		}

		[Test]
		public void TestKeyTransDESede()
		{
			tryKeyTrans(CmsAuthenticatedDataGenerator.DesEde3Cbc);
		}

		private void tryKeyTrans(
			string macAlg)
		{
			byte[] data = Encoding.ASCII.GetBytes("Eric H. Echidna");

			CmsAuthenticatedDataStreamGenerator adGen = new CmsAuthenticatedDataStreamGenerator();

			adGen.AddKeyTransRecipient(ReciCert);

			MemoryStream bOut = new MemoryStream();
			Stream aOut = adGen.Open(bOut, macAlg);
			aOut.Write(data, 0, data.Length);
			aOut.Close();

			CmsAuthenticatedDataParser ad = new CmsAuthenticatedDataParser(bOut.ToArray());

			RecipientInformationStore recipients = ad.GetRecipientInfos();

			Assert.AreEqual(ad.MacAlgOid, macAlg);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(recipient.KeyEncryptionAlgOid, PkcsObjectIdentifiers.RsaEncryption.Id);

				byte[] recData = recipient.GetContent(ReciKP.Private);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
				Assert.IsTrue(Arrays.AreEqual(ad.GetMac(), recipient.GetMac()));
			}
		}
	}
}