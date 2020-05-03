using System;
using System.Collections;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms.Tests
{
	[TestFixture]
	public class AuthenticatedDataTest
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
		
//		private static string          _signDN;
//		private static KeyPair _signKP;
//		private static X509Certificate _signCert;
//	
//		private static string          _origDN;
//		private static KeyPair         _origKP;
//		private static X509Certificate _origCert;
//	
//		private static string          _reciDN;
//		private static KeyPair         _reciKP;
//		private static X509Certificate _reciCert;
//	
//		private static KeyPair         _origEcKP;
//		private static KeyPair         _reciEcKP;
//		private static X509Certificate _reciEcCert;
//	
//		private static bool         _initialised = false;
//	
//		public bool DEBUG = true;
//	
//		private static void init()
//		{
//			if (!_initialised)
//			{
//			_initialised = true;
//			
//			_signDN   = "O=Bouncy Castle, C=AU";
//			_signKP   = CmsTestUtil.makeKeyPair();
//			_signCert = CmsTestUtil.makeCertificate(_signKP, _signDN, _signKP, _signDN);
//			
//			_origDN   = "CN=Bob, OU=Sales, O=Bouncy Castle, C=AU";
//			_origKP   = CmsTestUtil.makeKeyPair();
//			_origCert = CmsTestUtil.makeCertificate(_origKP, _origDN, _signKP, _signDN);
//			
//			_reciDN   = "CN=Doug, OU=Sales, O=Bouncy Castle, C=AU";
//			_reciKP   = CmsTestUtil.makeKeyPair();
//			_reciCert = CmsTestUtil.makeCertificate(_reciKP, _reciDN, _signKP, _signDN);
//			
//			_origEcKP = CmsTestUtil.makeEcDsaKeyPair();
//			_reciEcKP = CmsTestUtil.makeEcDsaKeyPair();
//			_reciEcCert = CmsTestUtil.makeCertificate(_reciEcKP, _reciDN, _signKP, _signDN);
//			}
//		}
//	
//		public void setUp()
//		{
//			init();
//		}
//		
//		public AuthenticatedDataTest(string name)
//		{
//		super(name);
//		}
//		
//		public static void main(string args[])
//		{
//		junit.textui.TestRunner.run(AuthenticatedDataTest.class);
//		}
//		
//		public static Test suite()
//		throws Exception
//		{
//		init();
//		
//		return new CMSTestSetup(new TestSuite(AuthenticatedDataTest.class));
//		}

		[Test]
		public void TestKeyTransDESede()
		{
			tryKeyTrans(CmsAuthenticatedDataGenerator.DesEde3Cbc);
		}
		
		[Test]
		public void TestKEKDESede()
		{
			tryKekAlgorithm(CmsTestUtil.MakeDesEde192Key(), new DerObjectIdentifier("1.2.840.113549.1.9.16.3.6"));
		}
		
		[Test]
		public void TestPasswordAES256()
		{
			passwordTest(CmsAuthenticatedDataGenerator.Aes256Cbc);
		}
		
		[Test]
		public void TestECKeyAgree()
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsAuthenticatedDataGenerator adGen = new CmsAuthenticatedDataGenerator();

			adGen.AddKeyAgreementRecipient(CmsAuthenticatedDataGenerator.ECDHSha1Kdf, OrigECKP.Private, OrigECKP.Public, ReciECCert, CmsAuthenticatedDataGenerator.Aes128Wrap);

			CmsAuthenticatedData ad = adGen.Generate(
				new CmsProcessableByteArray(data),
				CmsAuthenticatedDataGenerator.DesEde3Cbc);

			RecipientInformationStore recipients = ad.GetRecipientInfos();

			Assert.AreEqual(CmsAuthenticatedDataGenerator.DesEde3Cbc, ad.MacAlgOid);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				byte[] recData = recipient.GetContent(ReciECKP.Private);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
				Assert.IsTrue(Arrays.AreEqual(ad.GetMac(), recipient.GetMac()));
			}
		}
		
		[Test]
		public void TestEncoding()
		{
			byte[] data = Encoding.ASCII.GetBytes("Eric H. Echidna");

			CmsAuthenticatedDataGenerator adGen = new CmsAuthenticatedDataGenerator();

			adGen.AddKeyTransRecipient(ReciCert);

			CmsAuthenticatedData ad = adGen.Generate(
				new CmsProcessableByteArray(data),
				CmsAuthenticatedDataGenerator.DesEde3Cbc);

			ad = new CmsAuthenticatedData(ad.GetEncoded());

			RecipientInformationStore recipients = ad.GetRecipientInfos();

			Assert.AreEqual(CmsAuthenticatedDataGenerator.DesEde3Cbc, ad.MacAlgOid);

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

		private void tryKeyTrans(string macAlg)
		{
			byte[] data = Encoding.ASCII.GetBytes("Eric H. Echidna");

			CmsAuthenticatedDataGenerator adGen = new CmsAuthenticatedDataGenerator();

			adGen.AddKeyTransRecipient(ReciCert);

			CmsAuthenticatedData ad = adGen.Generate(
				new CmsProcessableByteArray(data),
				macAlg);

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
		
		private void tryKekAlgorithm(KeyParameter kek, DerObjectIdentifier algOid)
		{
			byte[] data = Encoding.ASCII.GetBytes("Eric H. Echidna");

			CmsAuthenticatedDataGenerator adGen = new CmsAuthenticatedDataGenerator();

			byte[] kekId = new byte[] { 1, 2, 3, 4, 5 };

			// FIXME Will this work for macs?
			string keyAlgorithm = ParameterUtilities.GetCanonicalAlgorithmName(algOid.Id);

			adGen.AddKekRecipient(keyAlgorithm, kek, kekId);

			CmsAuthenticatedData ad = adGen.Generate(
				new CmsProcessableByteArray(data),
				CmsAuthenticatedDataGenerator.DesEde3Cbc);

			RecipientInformationStore recipients = ad.GetRecipientInfos();

			Assert.AreEqual(CmsAuthenticatedDataGenerator.DesEde3Cbc, ad.MacAlgOid);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(recipient.KeyEncryptionAlgOid, algOid.Id);

				byte[] recData = recipient.GetContent(kek);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
				Assert.IsTrue(Arrays.AreEqual(ad.GetMac(), recipient.GetMac()));
			}
		}
		
		private void passwordTest(string algorithm)
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsAuthenticatedDataGenerator adGen = new CmsAuthenticatedDataGenerator();

			adGen.AddPasswordRecipient(new Pkcs5Scheme2PbeKey("password".ToCharArray(), new byte[20], 5), algorithm);
		
			CmsAuthenticatedData ad = adGen.Generate(
				new CmsProcessableByteArray(data),
				CmsAuthenticatedDataGenerator.DesEde3Cbc);

			RecipientInformationStore recipients = ad.GetRecipientInfos();

			Assert.AreEqual(CmsAuthenticatedDataGenerator.DesEde3Cbc, ad.MacAlgOid);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (PasswordRecipientInformation recipient in c)
			{
				CmsPbeKey key = new Pkcs5Scheme2PbeKey("password".ToCharArray(), recipient.KeyDerivationAlgorithm);

				byte[] recData = recipient.GetContent(key);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
				Assert.IsTrue(Arrays.AreEqual(ad.GetMac(), recipient.GetMac()));
			}
		}
	}
}
