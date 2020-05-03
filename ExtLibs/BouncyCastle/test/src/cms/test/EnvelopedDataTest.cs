using System;
using System.Collections;
using System.Text;

using NUnit.Framework;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms.Tests
{
	[TestFixture]
	public class EnvelopedDataTest
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
		private const string ReciDN2 = "CN=Fred, OU=Sales, O=Bouncy Castle, C=AU";

		private static AsymmetricCipherKeyPair reciKP;
		private static X509Certificate reciCert;

		private static AsymmetricCipherKeyPair origECKP;
		private static AsymmetricCipherKeyPair reciECKP;
		private static X509Certificate reciECCert;
		private static AsymmetricCipherKeyPair reciECKP2;
		private static X509Certificate reciECCert2;

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

		private static AsymmetricCipherKeyPair ReciECKP2
		{
			get { return reciECKP2 == null ? (reciECKP2 = CmsTestUtil.MakeECDsaKeyPair()) : reciECKP2; }
		}

		private static X509Certificate ReciECCert2
		{
			get { return reciECCert2 == null ? (reciECCert2 = CmsTestUtil.MakeCertificate(ReciECKP2, ReciDN2, SignKP, SignDN)) : reciECCert2; }
		}

		private static readonly byte[] oldKEK = Base64.Decode(
			"MIAGCSqGSIb3DQEHA6CAMIACAQIxQaI/MD0CAQQwBwQFAQIDBAUwDQYJYIZIAWUDBAEFBQAEI"
			+ "Fi2eHTPM4bQSjP4DUeDzJZLpfemW2gF1SPq7ZPHJi1mMIAGCSqGSIb3DQEHATAUBggqhkiG9w"
			+ "0DBwQImtdGyUdGGt6ggAQYk9X9z01YFBkU7IlS3wmsKpm/zpZClTceAAAAAAAAAAAAAA==");

		private static readonly byte[] ecKeyAgreeMsgAES256 = Base64.Decode(
			"MIAGCSqGSIb3DQEHA6CAMIACAQIxgcShgcECAQOgQ6FBMAsGByqGSM49AgEF"
			+ "AAMyAAPdXlSTpub+qqno9hUGkUDl+S3/ABhPziIB5yGU4678tgOgU5CiKG9Z"
			+ "kfnabIJ3nZYwGgYJK4EFEIZIPwACMA0GCWCGSAFlAwQBLQUAMFswWTAtMCgx"
			+ "EzARBgNVBAMTCkFkbWluLU1EU0UxETAPBgNVBAoTCDRCQ1QtMklEAgEBBCi/"
			+ "rJRLbFwEVW6PcLLmojjW9lI/xGD7CfZzXrqXFw8iHaf3hTRau1gYMIAGCSqG"
			+ "SIb3DQEHATAdBglghkgBZQMEASoEEMtCnKKPwccmyrbgeSIlA3qggAQQDLw8"
			+ "pNJR97bPpj6baG99bQQQwhEDsoj5Xg1oOxojHVcYzAAAAAAAAAAAAAA=");

		private static readonly byte[] ecKeyAgreeMsgAES128 = Base64.Decode(
			"MIAGCSqGSIb3DQEHA6CAMIACAQIxgbShgbECAQOgQ6FBMAsGByqGSM49AgEF"
			+ "AAMyAAL01JLEgKvKh5rbxI/hOxs/9WEezMIsAbUaZM4l5tn3CzXAN505nr5d"
			+ "LhrcurMK+tAwGgYJK4EFEIZIPwACMA0GCWCGSAFlAwQBBQUAMEswSTAtMCgx"
			+ "EzARBgNVBAMTCkFkbWluLU1EU0UxETAPBgNVBAoTCDRCQ1QtMklEAgEBBBhi"
			+ "FLjc5g6aqDT3f8LomljOwl1WTrplUT8wgAYJKoZIhvcNAQcBMB0GCWCGSAFl"
			+ "AwQBAgQQzXjms16Y69S/rB0EbHqRMaCABBAFmc/QdVW6LTKdEy97kaZzBBBa"
			+ "fQuviUS03NycpojELx0bAAAAAAAAAAAAAA==");

		private static readonly byte[] ecKeyAgreeMsgDESEDE = Base64.Decode(
			"MIAGCSqGSIb3DQEHA6CAMIACAQIxgcahgcMCAQOgQ6FBMAsGByqGSM49AgEF"
			+ "AAMyAALIici6Nx1WN5f0ThH2A8ht9ovm0thpC5JK54t73E1RDzCifePaoQo0"
			+ "xd6sUqoyGaYwHAYJK4EFEIZIPwACMA8GCyqGSIb3DQEJEAMGBQAwWzBZMC0w"
			+ "KDETMBEGA1UEAxMKQWRtaW4tTURTRTERMA8GA1UEChMINEJDVC0ySUQCAQEE"
			+ "KJuqZQ1NB1vXrKPOnb4TCpYOsdm6GscWdwAAZlm2EHMp444j0s55J9wwgAYJ"
			+ "KoZIhvcNAQcBMBQGCCqGSIb3DQMHBAjwnsDMsafCrKCABBjyPvqFOVMKxxut"
			+ "VfTx4fQlNGJN8S2ATRgECMcTQ/dsmeViAAAAAAAAAAAAAA==");

		private static readonly byte[] ecMqvKeyAgreeMsgAes128 = Base64.Decode(
			  "MIAGCSqGSIb3DQEHA6CAMIACAQIxgf2hgfoCAQOgQ6FBMAsGByqGSM49AgEF"
			+ "AAMyAAPDKU+0H58tsjpoYmYCInMr/FayvCCkupebgsnpaGEB7qS9vzcNVUj6"
			+ "mrnmiC2grpmhRwRFMEMwQTALBgcqhkjOPQIBBQADMgACZpD13z9c7DzRWx6S"
			+ "0xdbq3S+EJ7vWO+YcHVjTD8NcQDcZcWASW899l1PkL936zsuMBoGCSuBBRCG"
			+ "SD8AEDANBglghkgBZQMEAQUFADBLMEkwLTAoMRMwEQYDVQQDEwpBZG1pbi1N"
			+ "RFNFMREwDwYDVQQKEwg0QkNULTJJRAIBAQQYFq58L71nyMK/70w3nc6zkkRy"
			+ "RL7DHmpZMIAGCSqGSIb3DQEHATAdBglghkgBZQMEAQIEEDzRUpreBsZXWHBe"
			+ "onxOtSmggAQQ7csAZXwT1lHUqoazoy8bhAQQq+9Zjj8iGdOWgyebbfj67QAA"
			+ "AAAAAAAAAAA=");

		private static readonly byte[] ecKeyAgreeKey = Base64.Decode(
			"MIG2AgEAMBAGByqGSM49AgEGBSuBBAAiBIGeMIGbAgEBBDC8vp7xVTbKSgYVU5Wc"
			+ "hGkWbzaj+yUFETIWP1Dt7+WSpq3ikSPdl7PpHPqnPVZfoIWhZANiAgSYHTgxf+Dd"
			+ "Tt84dUvuSKkFy3RhjxJmjwIscK6zbEUzKhcPQG2GHzXhWK5x1kov0I74XpGhVkya"
			+ "ElH5K6SaOXiXAzcyNGggTOk4+ZFnz5Xl0pBje3zKxPhYu0SnCw7Pcqw=");

		private static readonly byte[] bobPrivRsaEncrypt = Base64.Decode(
			"MIIChQIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAKnhZ5g/OdVf"
			+ "8qCTQV6meYmFyDVdmpFb+x0B2hlwJhcPvaUi0DWFbXqYZhRBXM+3twg7CcmR"
			+ "uBlpN235ZR572akzJKN/O7uvRgGGNjQyywcDWVL8hYsxBLjMGAgUSOZPHPtd"
			+ "YMTgXB9T039T2GkB8QX4enDRvoPGXzjPHCyqaqfrAgMBAAECgYBnzUhMmg2P"
			+ "mMIbZf8ig5xt8KYGHbztpwOIlPIcaw+LNd4Ogngwy+e6alatd8brUXlweQqg"
			+ "9P5F4Kmy9Bnah5jWMIR05PxZbMHGd9ypkdB8MKCixQheIXFD/A0HPfD6bRSe"
			+ "TmPwF1h5HEuYHD09sBvf+iU7o8AsmAX2EAnYh9sDGQJBANDDIsbeopkYdo+N"
			+ "vKZ11mY/1I1FUox29XLE6/BGmvE+XKpVC5va3Wtt+Pw7PAhDk7Vb/s7q/WiE"
			+ "I2Kv8zHCueUCQQDQUfweIrdb7bWOAcjXq/JY1PeClPNTqBlFy2bKKBlf4hAr"
			+ "84/sajB0+E0R9KfEILVHIdxJAfkKICnwJAiEYH2PAkA0umTJSChXdNdVUN5q"
			+ "SO8bKlocSHseIVnDYDubl6nA7xhmqU5iUjiEzuUJiEiUacUgFJlaV/4jbOSn"
			+ "I3vQgLeFAkEAni+zN5r7CwZdV+EJBqRd2ZCWBgVfJAZAcpw6iIWchw+dYhKI"
			+ "FmioNRobQ+g4wJhprwMKSDIETukPj3d9NDAlBwJAVxhn1grStavCunrnVNqc"
			+ "BU+B1O8BiR4yPWnLMcRSyFRVJQA7HCp8JlDV6abXd8vPFfXuC9WN7rOvTKF8"
			+ "Y0ZB9qANMAsGA1UdDzEEAwIAEA==");

		private static readonly byte[] rfc4134ex5_1 = Base64.Decode(
			"MIIBHgYJKoZIhvcNAQcDoIIBDzCCAQsCAQAxgcAwgb0CAQAwJjASMRAwDgYD"
			+ "VQQDEwdDYXJsUlNBAhBGNGvHgABWvBHTbi7NXXHQMA0GCSqGSIb3DQEBAQUA"
			+ "BIGAC3EN5nGIiJi2lsGPcP2iJ97a4e8kbKQz36zg6Z2i0yx6zYC4mZ7mX7FB"
			+ "s3IWg+f6KgCLx3M1eCbWx8+MDFbbpXadCDgO8/nUkUNYeNxJtuzubGgzoyEd"
			+ "8Ch4H/dd9gdzTd+taTEgS0ipdSJuNnkVY4/M652jKKHRLFf02hosdR8wQwYJ"
			+ "KoZIhvcNAQcBMBQGCCqGSIb3DQMHBAgtaMXpRwZRNYAgDsiSf8Z9P43LrY4O"
			+ "xUk660cu1lXeCSFOSOpOJ7FuVyU=");

		private static readonly byte[] rfc4134ex5_2 = Base64.Decode(
			"MIIBZQYJKoZIhvcNAQcDoIIBVjCCAVICAQIxggEAMIG9AgEAMCYwEjEQMA4G"
			+ "A1UEAxMHQ2FybFJTQQIQRjRrx4AAVrwR024uzV1x0DANBgkqhkiG9w0BAQEF"
			+ "AASBgJQmQojGi7Z4IP+CVypBmNFoCDoEp87khtgyff2N4SmqD3RxPx+8hbLQ"
			+ "t9i3YcMwcap+aiOkyqjMalT03VUC0XBOGv+HYI3HBZm/aFzxoq+YOXAWs5xl"
			+ "GerZwTOc9j6AYlK4qXvnztR5SQ8TBjlzytm4V7zg+TGrnGVNQBNw47Ewoj4C"
			+ "AQQwDQQLTWFpbExpc3RSQzIwEAYLKoZIhvcNAQkQAwcCAToEGHcUr5MSJ/g9"
			+ "HnJVHsQ6X56VcwYb+OfojTBJBgkqhkiG9w0BBwEwGgYIKoZIhvcNAwIwDgIC"
			+ "AKAECJwE0hkuKlWhgCBeKNXhojuej3org9Lt7n+wWxOhnky5V50vSpoYRfRR"
			+ "yw==");

		[Test]
		public void TestKeyTrans()
		{
			byte[] data = Encoding.ASCII.GetBytes("WallaWallaWashington");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				CmsEnvelopedDataGenerator.DesEde3Cbc);

			RecipientInformationStore recipients = ed.GetRecipientInfos();


			Assert.AreEqual(ed.EncryptionAlgOid, CmsEnvelopedDataGenerator.DesEde3Cbc);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(recipient.KeyEncryptionAlgOid, PkcsObjectIdentifiers.RsaEncryption.Id);

				byte[] recData = recipient.GetContent(ReciKP.Private);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		[Test]
		public void TestKeyTransRC4()
		{
			byte[] data = Encoding.ASCII.GetBytes("WallaWallaBouncyCastle");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				"1.2.840.113549.3.4"); // RC4 OID

			RecipientInformationStore  recipients = ed.GetRecipientInfos();

			Assert.AreEqual(ed.EncryptionAlgOid, "1.2.840.113549.3.4");

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				byte[] recData = recipient.GetContent(ReciKP.Private);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		[Test]
		public void TestKeyTrans128RC4()
		{
			byte[] data = Encoding.ASCII.GetBytes("WallaWallaBouncyCastle");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				"1.2.840.113549.3.4", 128);  // RC4 OID

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual(ed.EncryptionAlgOid, "1.2.840.113549.3.4");

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				byte[] recData = recipient.GetContent(ReciKP.Private);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		[Test]
		public void TestKeyTransOdes()
		{
			byte[] data = Encoding.ASCII.GetBytes("WallaWallaBouncyCastle");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				OiwObjectIdentifiers.DesCbc.Id);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual(ed.EncryptionAlgOid, OiwObjectIdentifiers.DesCbc.Id);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				byte[] recData = recipient.GetContent(ReciKP.Private);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		[Test]
		public void TestKeyTransSmallAes()
		{
			byte[] data = new byte[] { 0, 1, 2, 3 };

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				CmsEnvelopedDataGenerator.Aes128Cbc);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual(ed.EncryptionAlgOid,
				CmsEnvelopedDataGenerator.Aes128Cbc);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				byte[] recData = recipient.GetContent(ReciKP.Private);
				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		[Test]
		public void TestKeyTransCast5()
		{
			TryKeyTrans(CmsEnvelopedDataGenerator.Cast5Cbc,
				new DerObjectIdentifier(CmsEnvelopedDataGenerator.Cast5Cbc),
				typeof(Asn1Sequence));
		}

		[Test]
		public void TestKeyTransAes128()
		{
			TryKeyTrans(CmsEnvelopedDataGenerator.Aes128Cbc,
				NistObjectIdentifiers.IdAes128Cbc,
				typeof(DerOctetString));
		}

		[Test]
		public void TestKeyTransAes192()
		{
			TryKeyTrans(CmsEnvelopedDataGenerator.Aes192Cbc,
				NistObjectIdentifiers.IdAes192Cbc,
				typeof(DerOctetString));
		}

		[Test]
		public void TestKeyTransAes256()
		{
			TryKeyTrans(CmsEnvelopedDataGenerator.Aes256Cbc,
				NistObjectIdentifiers.IdAes256Cbc,
				typeof(DerOctetString));
		}

		[Test]
		public void TestKeyTransSeed()
		{
			TryKeyTrans(CmsEnvelopedDataGenerator.SeedCbc,
				KisaObjectIdentifiers.IdSeedCbc,
				typeof(DerOctetString));
		}

		public void TestKeyTransCamellia128()
		{
			TryKeyTrans(CmsEnvelopedDataGenerator.Camellia128Cbc,
				NttObjectIdentifiers.IdCamellia128Cbc,
				typeof(DerOctetString));
		}

		public void TestKeyTransCamellia192()
		{
			TryKeyTrans(CmsEnvelopedDataGenerator.Camellia192Cbc,
				NttObjectIdentifiers.IdCamellia192Cbc,
				typeof(DerOctetString));
		}

		public void TestKeyTransCamellia256()
		{
			TryKeyTrans(CmsEnvelopedDataGenerator.Camellia256Cbc,
				NttObjectIdentifiers.IdCamellia256Cbc,
				typeof(DerOctetString));
		}

		private void TryKeyTrans(
			string				generatorOID,
			DerObjectIdentifier	checkOID,
			Type				asn1Params)
		{
			byte[] data = Encoding.ASCII.GetBytes("WallaWallaWashington");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			CmsEnvelopedData ed = edGen.Generate(new CmsProcessableByteArray(data), generatorOID);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual(checkOID.Id, ed.EncryptionAlgOid);

			if (asn1Params != null)
			{
				Assert.IsTrue(asn1Params.IsInstanceOfType(ed.EncryptionAlgorithmID.Parameters));
			}

			ArrayList c = new ArrayList(recipients.GetRecipients());

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(recipient.KeyEncryptionAlgOid, PkcsObjectIdentifiers.RsaEncryption.Id);

				byte[] recData = recipient.GetContent(ReciKP.Private);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		[Test]
		public void TestErroneousKek()
		{
			byte[] data = Encoding.ASCII.GetBytes("WallaWallaWashington");
			KeyParameter kek = ParameterUtilities.CreateKeyParameter(
				"AES",
				new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });

			CmsEnvelopedData ed = new CmsEnvelopedData(oldKEK);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual(ed.EncryptionAlgOid, CmsEnvelopedDataGenerator.DesEde3Cbc);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(recipient.KeyEncryptionAlgOid, NistObjectIdentifiers.IdAes128Wrap.Id);

				byte[] recData = recipient.GetContent(kek);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		[Test]
		public void TestDesKek()
		{
			TryKekAlgorithm(CmsTestUtil.MakeDesEde192Key(), new DerObjectIdentifier("1.2.840.113549.1.9.16.3.6"));
		}

		[Test]
		public void TestRC2128Kek()
		{
			TryKekAlgorithm(CmsTestUtil.MakeRC2128Key(), new DerObjectIdentifier("1.2.840.113549.1.9.16.3.7"));
		}

		[Test]
		public void TestAes128Kek()
		{
			TryKekAlgorithm(CmsTestUtil.MakeAesKey(128), NistObjectIdentifiers.IdAes128Wrap);
		}

		[Test]
		public void TestAes192Kek()
		{
			TryKekAlgorithm(CmsTestUtil.MakeAesKey(192), NistObjectIdentifiers.IdAes192Wrap);
		}

		[Test]
		public void TestAes256Kek()
		{
			TryKekAlgorithm(CmsTestUtil.MakeAesKey(256), NistObjectIdentifiers.IdAes256Wrap);
		}

		[Test]
		public void TestSeed128Kek()
		{
			TryKekAlgorithm(CmsTestUtil.MakeSeedKey(), KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap);
		}

		[Test]
		public void TestCamellia128Kek()
		{
			TryKekAlgorithm(CmsTestUtil.MakeCamelliaKey(128), NttObjectIdentifiers.IdCamellia128Wrap);
		}

		[Test]
		public void TestCamellia192Kek()
		{
			TryKekAlgorithm(CmsTestUtil.MakeCamelliaKey(192), NttObjectIdentifiers.IdCamellia192Wrap);
		}

		[Test]
		public void TestCamellia256Kek()
		{
			TryKekAlgorithm(CmsTestUtil.MakeCamelliaKey(256), NttObjectIdentifiers.IdCamellia256Wrap);
		}

		private void TryKekAlgorithm(
			KeyParameter		kek,
			DerObjectIdentifier	algOid)
		{
			byte[] data = Encoding.ASCII.GetBytes("WallaWallaWashington");
			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			byte[] kekId = new byte[] { 1, 2, 3, 4, 5 };

			string keyAlgorithm = ParameterUtilities.GetCanonicalAlgorithmName(algOid.Id);

			edGen.AddKekRecipient(keyAlgorithm, kek, kekId);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				CmsEnvelopedDataGenerator.DesEde3Cbc);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual(ed.EncryptionAlgOid, CmsEnvelopedDataGenerator.DesEde3Cbc);

			ArrayList c = new ArrayList(recipients.GetRecipients());

			Assert.IsTrue(c.Count > 0);

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(algOid.Id, recipient.KeyEncryptionAlgOid);

				byte[] recData = recipient.GetContent(kek);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		[Test]
		public void TestECKeyAgree()
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddKeyAgreementRecipient(
				CmsEnvelopedDataGenerator.ECDHSha1Kdf,
				OrigECKP.Private,
				OrigECKP.Public,
				ReciECCert,
				CmsEnvelopedDataGenerator.Aes128Wrap);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				CmsEnvelopedDataGenerator.Aes128Cbc);

			Assert.AreEqual(ed.EncryptionAlgOid, CmsEnvelopedDataGenerator.Aes128Cbc);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			ConfirmDataReceived(recipients, data, ReciECCert, ReciECKP.Private);
			ConfirmNumberRecipients(recipients, 1);
		}

		[Test]
		public void TestECMqvKeyAgree()
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddKeyAgreementRecipient(
				CmsEnvelopedDataGenerator.ECMqvSha1Kdf,
				OrigECKP.Private,
				OrigECKP.Public,
				ReciECCert,
				CmsEnvelopedDataGenerator.Aes128Wrap);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				CmsEnvelopedDataGenerator.Aes128Cbc);

			Assert.AreEqual(ed.EncryptionAlgOid, CmsEnvelopedDataGenerator.Aes128Cbc);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			ConfirmDataReceived(recipients, data, ReciECCert, ReciECKP.Private);
			ConfirmNumberRecipients(recipients, 1);
		}

		[Test]
		public void TestECMqvKeyAgreeMultiple()
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			ArrayList recipientCerts = new ArrayList();
			recipientCerts.Add(ReciECCert);
			recipientCerts.Add(ReciECCert2);

			edGen.AddKeyAgreementRecipients(
				CmsEnvelopedDataGenerator.ECMqvSha1Kdf,
				OrigECKP.Private,
				OrigECKP.Public,
				recipientCerts,
				CmsEnvelopedDataGenerator.Aes128Wrap);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				CmsEnvelopedDataGenerator.Aes128Cbc);

			Assert.AreEqual(ed.EncryptionAlgOid, CmsEnvelopedDataGenerator.Aes128Cbc);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			ConfirmDataReceived(recipients, data, ReciECCert, ReciECKP.Private);
			ConfirmDataReceived(recipients, data, ReciECCert2, ReciECKP2.Private);
			ConfirmNumberRecipients(recipients, 2);
		}

		private static void ConfirmDataReceived(RecipientInformationStore recipients,
			byte[] expectedData, X509Certificate reciCert, AsymmetricKeyParameter reciPrivKey)
		{
			RecipientID rid = new RecipientID();
			rid.Issuer = PrincipalUtilities.GetIssuerX509Principal(reciCert);
			rid.SerialNumber = reciCert.SerialNumber;

			RecipientInformation recipient = recipients[rid];
			Assert.IsNotNull(recipient);

			byte[] actualData = recipient.GetContent(reciPrivKey);
			Assert.IsTrue(Arrays.AreEqual(expectedData, actualData));
		}

		private static void ConfirmNumberRecipients(RecipientInformationStore recipients, int count)
		{
			Assert.AreEqual(count, recipients.GetRecipients().Count);
		}

		[Test]
		public void TestECKeyAgreeVectors()
		{
			AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(ecKeyAgreeKey);

			VerifyECKeyAgreeVectors(privKey, "2.16.840.1.101.3.4.1.42", ecKeyAgreeMsgAES256);
			VerifyECKeyAgreeVectors(privKey, "2.16.840.1.101.3.4.1.2", ecKeyAgreeMsgAES128);
			VerifyECKeyAgreeVectors(privKey, "1.2.840.113549.3.7", ecKeyAgreeMsgDESEDE);
		}

		[Test]
		public void TestECMqvKeyAgreeVectors()
		{
			AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(ecKeyAgreeKey);

			VerifyECMqvKeyAgreeVectors(privKey, "2.16.840.1.101.3.4.1.2", ecMqvKeyAgreeMsgAes128);
		}

		[Test]
		public void TestPasswordAes256()
		{
			PasswordTest(CmsEnvelopedDataGenerator.Aes256Cbc);
			PasswordUtf8Test(CmsEnvelopedDataGenerator.Aes256Cbc);
		}

		[Test]
		public void TestPasswordDesEde()
		{
			PasswordTest(CmsEnvelopedDataGenerator.DesEde3Cbc);
			PasswordUtf8Test(CmsEnvelopedDataGenerator.DesEde3Cbc);
		}

		[Test]
		public void TestRfc4134Ex5_1()
		{
			byte[] data = Hex.Decode("5468697320697320736f6d652073616d706c6520636f6e74656e742e");

//			KeyFactory kFact = KeyFactory.GetInstance("RSA");
//			Key key = kFact.generatePrivate(new PKCS8EncodedKeySpec(bobPrivRsaEncrypt));
			AsymmetricKeyParameter key = PrivateKeyFactory.CreateKey(bobPrivRsaEncrypt);

			CmsEnvelopedData ed = new CmsEnvelopedData(rfc4134ex5_1);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual("1.2.840.113549.3.7", ed.EncryptionAlgOid);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				byte[] recData = recipient.GetContent(key);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		[Test]
		public void TestRfc4134Ex5_2()
		{
			byte[] data = Hex.Decode("5468697320697320736f6d652073616d706c6520636f6e74656e742e");

//			KeyFactory kFact = KeyFactory.GetInstance("RSA");
//			Key key = kFact.generatePrivate(new PKCS8EncodedKeySpec(bobPrivRsaEncrypt));
			AsymmetricKeyParameter key = PrivateKeyFactory.CreateKey(bobPrivRsaEncrypt);

			CmsEnvelopedData ed = new CmsEnvelopedData(rfc4134ex5_2);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual("1.2.840.113549.3.2", ed.EncryptionAlgOid);

			ICollection c = recipients.GetRecipients();
			IEnumerator e = c.GetEnumerator();

			if (e.MoveNext())
			{
				do
				{
					RecipientInformation recipient = (RecipientInformation) e.Current;

					if (recipient is KeyTransRecipientInformation)
					{
						byte[] recData = recipient.GetContent(key);

						Assert.IsTrue(Arrays.AreEqual(data, recData));
					}
				}
				while (e.MoveNext());
			}
			else
			{
				Assert.Fail("no recipient found");
			}
		}

		[Test]
		public void TestOriginatorInfo()
		{
			CmsEnvelopedData env = new CmsEnvelopedData(CmsSampleMessages.originatorMessage);

			RecipientInformationStore  recipients = env.GetRecipientInfos();

			Assert.AreEqual(CmsEnvelopedDataGenerator.DesEde3Cbc, env.EncryptionAlgOid);
		}

		private void PasswordTest(
			string algorithm)
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddPasswordRecipient(new Pkcs5Scheme2PbeKey("password".ToCharArray(), new byte[20], 5), algorithm);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				CmsEnvelopedDataGenerator.Aes128Cbc);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual(ed.EncryptionAlgOid, CmsEnvelopedDataGenerator.Aes128Cbc);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (PasswordRecipientInformation recipient in c)
			{
				CmsPbeKey key = new Pkcs5Scheme2PbeKey("password".ToCharArray(), recipient.KeyDerivationAlgorithm);

				byte[] recData = recipient.GetContent(key);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		private void PasswordUtf8Test(
			string algorithm)
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsEnvelopedDataGenerator edGen = new CmsEnvelopedDataGenerator();

			edGen.AddPasswordRecipient(
				new Pkcs5Scheme2Utf8PbeKey("abc\u5639\u563b".ToCharArray(), new byte[20], 5),
				algorithm);

			CmsEnvelopedData ed = edGen.Generate(
				new CmsProcessableByteArray(data),
				CmsEnvelopedDataGenerator.Aes128Cbc);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual(ed.EncryptionAlgOid, CmsEnvelopedDataGenerator.Aes128Cbc);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (PasswordRecipientInformation recipient in c)
			{
				CmsPbeKey key = new Pkcs5Scheme2Utf8PbeKey(
					"abc\u5639\u563b".ToCharArray(), recipient.KeyDerivationAlgorithm);

				byte[] recData = recipient.GetContent(key);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		private void VerifyECKeyAgreeVectors(
			AsymmetricKeyParameter	privKey,
			string					wrapAlg,
			byte[]					message)
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsEnvelopedData ed = new CmsEnvelopedData(message);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			Assert.AreEqual(wrapAlg, ed.EncryptionAlgOid);

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual("1.3.133.16.840.63.0.2", recipient.KeyEncryptionAlgOid);

				byte[] recData = recipient.GetContent(privKey);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}

		private void VerifyECMqvKeyAgreeVectors(
			AsymmetricKeyParameter	privKey,
			string					wrapAlg,
			byte[]					message)
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsEnvelopedData ed = new CmsEnvelopedData(message);

			RecipientInformationStore recipients = ed.GetRecipientInfos();

			ICollection c = recipients.GetRecipients();

			Assert.AreEqual(wrapAlg, ed.EncryptionAlgOid);
			Assert.AreEqual(1, c.Count);

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual("1.3.133.16.840.63.0.16", recipient.KeyEncryptionAlgOid);

				byte[] recData = recipient.GetContent(privKey);

				Assert.IsTrue(Arrays.AreEqual(data, recData));
			}
		}
	}
}
