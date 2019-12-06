using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms.Tests
{
	[TestFixture]
	public class EnvelopedDataStreamTest
	{
		private const int BufferSize = 4000;

		private const string SignDN = "O=Bouncy Castle, C=AU";
		private static AsymmetricCipherKeyPair signKP;
//		private static X509Certificate signCert;
		//signCert = CmsTestUtil.MakeCertificate(_signKP, SignDN, _signKP, SignDN);

//		private const string OrigDN = "CN=Bob, OU=Sales, O=Bouncy Castle, C=AU";
//		private static AsymmetricCipherKeyPair origKP;
		//origKP   = CmsTestUtil.MakeKeyPair();
//		private static X509Certificate origCert;
		//origCert = CmsTestUtil.MakeCertificate(origKP, OrigDN, _signKP, SignDN);

		private const string ReciDN = "CN=Doug, OU=Sales, O=Bouncy Castle, C=AU";
		private static AsymmetricCipherKeyPair reciKP;
		private static X509Certificate reciCert;

		private static AsymmetricCipherKeyPair origECKP;
		private static AsymmetricCipherKeyPair reciECKP;
		private static X509Certificate reciECCert;

		private static AsymmetricCipherKeyPair SignKP
		{
			get { return signKP == null ? (signKP   = CmsTestUtil.MakeKeyPair()) : signKP; }
		}

		private static AsymmetricCipherKeyPair ReciKP
		{
			get { return reciKP == null ? (reciKP   = CmsTestUtil.MakeKeyPair()) : reciKP; }
		}

		private static X509Certificate ReciCert
		{
			get { return reciCert == null ? (reciCert = CmsTestUtil.MakeCertificate(ReciKP, ReciDN, SignKP, SignDN)) : reciCert;}
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
			get { return reciECCert == null ? (reciECCert = CmsTestUtil.MakeCertificate(ReciECKP, ReciDN, SignKP, SignDN)) : reciECCert;}
		}

		[Test]
		public void TestWorkingData()
		{
			byte[] keyData = Base64.Decode(
				"MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAKrAz/SQKrcQ" +
				"nj9IxHIfKDbuXsMqUpI06s2gps6fp7RDNvtUDDMOciWGFhD45YSy8GO0mPx3" +
				"Nkc7vKBqX4TLcqLUz7kXGOHGOwiPZoNF+9jBMPNROe/B0My0PkWg9tuq+nxN" +
				"64oD47+JvDwrpNOS5wsYavXeAW8Anv9ZzHLU7KwZAgMBAAECgYA/fqdVt+5K" +
				"WKGfwr1Z+oAHvSf7xtchiw/tGtosZ24DOCNP3fcTXUHQ9kVqVkNyzt9ZFCT3" +
				"bJUAdBQ2SpfuV4DusVeQZVzcROKeA09nPkxBpTefWbSDQGhb+eZq9L8JDRSW" +
				"HyYqs+MBoUpLw7GKtZiJkZyY6CsYkAnQ+uYVWq/TIQJBAP5zafO4HUV/w4KD" +
				"VJi+ua+GYF1Sg1t/dYL1kXO9GP1p75YAmtm6LdnOCas7wj70/G1YlPGkOP0V" +
				"GFzeG5KAmAUCQQCryvKU9nwWA+kypcQT9Yr1P4vGS0APYoBThnZq7jEPc5Cm" +
				"ZI82yseSxSeea0+8KQbZ5mvh1p3qImDLEH/iNSQFAkAghS+tboKPN10NeSt+" +
				"uiGRRWNbiggv0YJ7Uldcq3ZeLQPp7/naiekCRUsHD4Qr97OrZf7jQ1HlRqTu" +
				"eZScjMLhAkBNUMZCQnhwFAyEzdPkQ7LpU1MdyEopYmRssuxijZao5JLqQAGw" +
				"YCzXokGFa7hz72b09F4DQurJL/WuDlvvu4jdAkEAxwT9lylvfSfEQw4/qQgZ" +
				"MFB26gqB6Gqs1pHIZCzdliKx5BO3VDeUGfXMI8yOkbXoWbYx5xPid/+N8R//" +
				"+sxLBw==");

			byte[] envData = Base64.Decode(
				"MIAGCSqGSIb3DQEHA6CAMIACAQAxgcQwgcECAQAwKjAlMRYwFAYDVQQKEw1C" +
				"b3VuY3kgQ2FzdGxlMQswCQYDVQQGEwJBVQIBHjANBgkqhkiG9w0BAQEFAASB" +
				"gDmnaDZ0vDJNlaUSYyEXsgbaUH+itNTjCOgv77QTX2ImXj+kTctM19PQF2I1" +
				"0/NL0fjakvCgBTHKmk13a7jqB6cX3bysenHNrglHsgNGgeXQ7ggAq5fV/JQQ" +
				"T7rSxEtuwpbuHQnoVUZahOHVKy/a0uLr9iIh1A3y+yZTZaG505ZJMIAGCSqG" +
				"SIb3DQEHATAdBglghkgBZQMEAQIEENmkYNbDXiZxJWtq82qIRZKggAQgkOGr" +
				"1JcTsADStez1eY4+rO4DtyBIyUYQ3pilnbirfPkAAAAAAAAAAAAA");


			CmsEnvelopedDataParser ep = new CmsEnvelopedDataParser(envData);

			RecipientInformationStore recipients = ep.GetRecipientInfos();

			Assert.AreEqual(ep.EncryptionAlgOid, CmsEnvelopedDataGenerator.Aes128Cbc);

			ICollection c = recipients.GetRecipients();

//            PKCS8EncodedKeySpec	keySpec = new PKCS8EncodedKeySpec(keyData);
//            KeyFactory			keyFact = KeyFactory.GetInstance("RSA");
//            Key					priKey = keyFact.generatePrivate(keySpec);
			AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(keyData);
            byte[] data = Hex.Decode("57616c6c6157616c6c6157617368696e67746f6e");

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(recipient.KeyEncryptionAlgOid, PkcsObjectIdentifiers.RsaEncryption.Id);

				CmsTypedStream recData = recipient.GetContentStream(priKey);

				byte[] compare = CmsTestUtil.StreamToByteArray(recData.ContentStream);
				Assert.IsTrue(Arrays.AreEqual(data, compare));
			}
		}

		private void VerifyData(
			byte[]	encodedBytes,
			string	expectedOid,
			byte[]	expectedData)
		{
			CmsEnvelopedDataParser ep = new CmsEnvelopedDataParser(encodedBytes);
			RecipientInformationStore recipients = ep.GetRecipientInfos();

			Assert.AreEqual(ep.EncryptionAlgOid, expectedOid);

			ICollection c = recipients.GetRecipients();

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(recipient.KeyEncryptionAlgOid, PkcsObjectIdentifiers.RsaEncryption.Id);

				CmsTypedStream recData = recipient.GetContentStream(ReciKP.Private);

				Assert.IsTrue(Arrays.AreEqual(expectedData, CmsTestUtil.StreamToByteArray(
					recData.ContentStream)));
			}
		}

		[Test]
		public void TestKeyTransAes128BufferedStream()
		{
			byte[] data = new byte[2000];
			for (int i = 0; i != 2000; i++)
			{
				data[i] = (byte)(i & 0xff);
			}

			//
			// unbuffered
			//
			CmsEnvelopedDataStreamGenerator edGen = new CmsEnvelopedDataStreamGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			MemoryStream bOut = new MemoryStream();

			Stream outStream = edGen.Open(
				bOut, CmsEnvelopedDataGenerator.Aes128Cbc);

			for (int i = 0; i != 2000; i++)
			{
				outStream.WriteByte(data[i]);
			}

			outStream.Close();

			VerifyData(bOut.ToArray(), CmsEnvelopedDataGenerator.Aes128Cbc, data);

			int unbufferedLength = bOut.ToArray().Length;

			//
			// Using buffered output - should be == to unbuffered
			//
			edGen = new CmsEnvelopedDataStreamGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			bOut.SetLength(0);

			outStream = edGen.Open(bOut, CmsEnvelopedDataGenerator.Aes128Cbc);

			Streams.PipeAll(new MemoryStream(data, false), outStream);
			outStream.Close();

			VerifyData(bOut.ToArray(), CmsEnvelopedDataGenerator.Aes128Cbc, data);

			Assert.AreEqual(unbufferedLength, bOut.ToArray().Length);
		}

		[Test]
		public void TestKeyTransAes128Buffered()
		{
			byte[] data = new byte[2000];
			for (int i = 0; i != 2000; i++)
			{
				data[i] = (byte)(i & 0xff);
			}

			//
			// unbuffered
			//
			CmsEnvelopedDataStreamGenerator edGen = new CmsEnvelopedDataStreamGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			MemoryStream  bOut = new MemoryStream();

			Stream outStream = edGen.Open(
				bOut, CmsEnvelopedDataGenerator.Aes128Cbc);

			for (int i = 0; i != 2000; i++)
			{
				outStream.WriteByte(data[i]);
			}

			outStream.Close();

			VerifyData(bOut.ToArray(), CmsEnvelopedDataGenerator.Aes128Cbc, data);

			int unbufferedLength = bOut.ToArray().Length;

			//
			// buffered - less than default of 1000
			//
			edGen = new CmsEnvelopedDataStreamGenerator();

			edGen.SetBufferSize(300);

			edGen.AddKeyTransRecipient(ReciCert);

			bOut.SetLength(0);

			outStream = edGen.Open(bOut, CmsEnvelopedDataGenerator.Aes128Cbc);

			for (int i = 0; i != 2000; i++)
			{
				outStream.WriteByte(data[i]);
			}

			outStream.Close();

			VerifyData(bOut.ToArray(), CmsEnvelopedDataGenerator.Aes128Cbc, data);

			Assert.IsTrue(unbufferedLength < bOut.ToArray().Length);
		}

		[Test]
		public void TestKeyTransAes128Der()
		{
			byte[] data = new byte[2000];
			for (int i = 0; i != 2000; i++)
			{
				data[i] = (byte)(i & 0xff);
			}

			CmsEnvelopedDataStreamGenerator edGen = new CmsEnvelopedDataStreamGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			MemoryStream bOut = new MemoryStream();

			Stream outStream = edGen.Open(
				bOut, CmsEnvelopedDataGenerator.Aes128Cbc);

			for (int i = 0; i != 2000; i++)
			{
				outStream.WriteByte(data[i]);
			}

			outStream.Close();

			// convert to DER
			byte[] derEncodedBytes = Asn1Object.FromByteArray(bOut.ToArray()).GetDerEncoded();

			VerifyData(derEncodedBytes, CmsEnvelopedDataGenerator.Aes128Cbc, data);
		}

		[Test]
		public void TestKeyTransAes128Throughput()
		{
			byte[] data = new byte[40001];
			for (int i = 0; i != data.Length; i++)
			{
				data[i] = (byte)(i & 0xff);
			}

			//
			// buffered
			//
			CmsEnvelopedDataStreamGenerator edGen = new CmsEnvelopedDataStreamGenerator();

			edGen.SetBufferSize(BufferSize);

			edGen.AddKeyTransRecipient(ReciCert);

			MemoryStream bOut = new MemoryStream();

			Stream outStream = edGen.Open(bOut, CmsEnvelopedDataGenerator.Aes128Cbc);

			for (int i = 0; i != data.Length; i++)
			{
				outStream.WriteByte(data[i]);
			}

			outStream.Close();

			CmsEnvelopedDataParser		ep = new CmsEnvelopedDataParser(bOut.ToArray());
			RecipientInformationStore	recipients = ep.GetRecipientInfos();
			ICollection					c = recipients.GetRecipients();

			IEnumerator e = c.GetEnumerator();

			if (e.MoveNext())
			{
				RecipientInformation recipient = (RecipientInformation) e.Current;

				Assert.AreEqual(recipient.KeyEncryptionAlgOid, PkcsObjectIdentifiers.RsaEncryption.Id);

				CmsTypedStream recData = recipient.GetContentStream(ReciKP.Private);

				Stream dataStream = recData.ContentStream;
				MemoryStream dataOut = new MemoryStream();
				int len;
				byte[] buf = new byte[BufferSize];
				int count = 0;

				while (count != 10 && (len = dataStream.Read(buf, 0, buf.Length)) > 0)
				{
					Assert.AreEqual(buf.Length, len);

					dataOut.Write(buf, 0, buf.Length);
					count++;
				}

				len = dataStream.Read(buf, 0, buf.Length);
				dataOut.Write(buf, 0, len);

				Assert.IsTrue(Arrays.AreEqual(data, dataOut.ToArray()));
			}
			else
			{
				Assert.Fail("recipient not found.");
			}
		}

		[Test]
		public void TestKeyTransAes128()
		{
			byte[] data = Encoding.Default.GetBytes("WallaWallaWashington");

			CmsEnvelopedDataStreamGenerator edGen = new CmsEnvelopedDataStreamGenerator();

			edGen.AddKeyTransRecipient(ReciCert);

			MemoryStream bOut = new MemoryStream();

			Stream outStream = edGen.Open(
				bOut, CmsEnvelopedDataGenerator.Aes128Cbc);

			outStream.Write(data, 0, data.Length);

			outStream.Close();

			CmsEnvelopedDataParser ep = new CmsEnvelopedDataParser(bOut.ToArray());

			RecipientInformationStore recipients = ep.GetRecipientInfos();

			Assert.AreEqual(ep.EncryptionAlgOid, CmsEnvelopedDataGenerator.Aes128Cbc);

			ICollection c = recipients.GetRecipients();

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(recipient.KeyEncryptionAlgOid, PkcsObjectIdentifiers.RsaEncryption.Id);

				CmsTypedStream recData = recipient.GetContentStream(ReciKP.Private);

				Assert.IsTrue(Arrays.AreEqual(data, CmsTestUtil.StreamToByteArray(recData.ContentStream)));
			}

			ep.Close();
		}

		[Test]
		public void TestAesKek()
		{
			byte[] data = Encoding.Default.GetBytes("WallaWallaWashington");
			KeyParameter kek = CmsTestUtil.MakeAes192Key();

			CmsEnvelopedDataStreamGenerator edGen = new CmsEnvelopedDataStreamGenerator();

			byte[] kekId = new byte[] { 1, 2, 3, 4, 5 };

			edGen.AddKekRecipient("AES192", kek, kekId);

			MemoryStream  bOut = new MemoryStream();

			Stream outStream = edGen.Open(
				bOut,
				CmsEnvelopedDataGenerator.DesEde3Cbc);
			outStream.Write(data, 0, data.Length);

			outStream.Close();

			CmsEnvelopedDataParser ep = new CmsEnvelopedDataParser(bOut.ToArray());

			RecipientInformationStore recipients = ep.GetRecipientInfos();

			Assert.AreEqual(ep.EncryptionAlgOid, CmsEnvelopedDataGenerator.DesEde3Cbc);

			ICollection c = recipients.GetRecipients();

			foreach (RecipientInformation recipient in c)
			{
				Assert.AreEqual(recipient.KeyEncryptionAlgOid, "2.16.840.1.101.3.4.1.25");

				CmsTypedStream recData = recipient.GetContentStream(kek);

				Assert.IsTrue(Arrays.AreEqual(data, CmsTestUtil.StreamToByteArray(recData.ContentStream)));
			}

			ep.Close();
		}

		[Test]
		public void TestTwoAesKek()
		{
			byte[] data = Encoding.Default.GetBytes("WallaWallaWashington");
			KeyParameter kek1 = CmsTestUtil.MakeAes192Key();
			KeyParameter kek2 = CmsTestUtil.MakeAes192Key();

			CmsEnvelopedDataStreamGenerator edGen = new CmsEnvelopedDataStreamGenerator();

			byte[]  kekId1 = new byte[] { 1, 2, 3, 4, 5 };
			byte[]  kekId2 = new byte[] { 5, 4, 3, 2, 1 };

			edGen.AddKekRecipient("AES192", kek1, kekId1);
			edGen.AddKekRecipient("AES192", kek2, kekId2);

			MemoryStream bOut = new MemoryStream();

			Stream outStream = edGen.Open(
				bOut,
				CmsEnvelopedDataGenerator.DesEde3Cbc);
			outStream.Write(data, 0, data.Length);

			outStream.Close();

			CmsEnvelopedDataParser ep = new CmsEnvelopedDataParser(bOut.ToArray());

			RecipientInformationStore recipients = ep.GetRecipientInfos();

			Assert.AreEqual(ep.EncryptionAlgOid, CmsEnvelopedDataGenerator.DesEde3Cbc);

			RecipientID recSel = new RecipientID();

			recSel.KeyIdentifier = kekId2;

			RecipientInformation recipient = recipients.GetFirstRecipient(recSel);

			Assert.AreEqual(recipient.KeyEncryptionAlgOid, "2.16.840.1.101.3.4.1.25");

			CmsTypedStream recData = recipient.GetContentStream(kek2);

			Assert.IsTrue(Arrays.AreEqual(data, CmsTestUtil.StreamToByteArray(recData.ContentStream)));

			ep.Close();
		}

		[Test]
		public void TestECKeyAgree()
		{
			byte[] data = Hex.Decode("504b492d4320434d5320456e76656c6f706564446174612053616d706c65");

			CmsEnvelopedDataStreamGenerator edGen = new CmsEnvelopedDataStreamGenerator();

			edGen.AddKeyAgreementRecipient(
				CmsEnvelopedDataGenerator.ECDHSha1Kdf,
				OrigECKP.Private,
				OrigECKP.Public,
				ReciECCert,
				CmsEnvelopedDataGenerator.Aes128Wrap);

			MemoryStream bOut = new MemoryStream();

			Stream outStr = edGen.Open(bOut, CmsEnvelopedDataGenerator.Aes128Cbc);
			outStr.Write(data, 0, data.Length);

			outStr.Close();

			CmsEnvelopedDataParser ep = new CmsEnvelopedDataParser(bOut.ToArray());

			RecipientInformationStore recipients = ep.GetRecipientInfos();

			Assert.AreEqual(ep.EncryptionAlgOid, CmsEnvelopedDataGenerator.Aes128Cbc);

			RecipientID recSel = new RecipientID();

//			recSel.SetIssuer(PrincipalUtilities.GetIssuerX509Principal(ReciECCert).GetEncoded());
			recSel.Issuer = PrincipalUtilities.GetIssuerX509Principal(ReciECCert);
			recSel.SerialNumber = ReciECCert.SerialNumber;

			RecipientInformation recipient = recipients.GetFirstRecipient(recSel);

			CmsTypedStream recData = recipient.GetContentStream(ReciECKP.Private);

			Assert.IsTrue(Arrays.AreEqual(data, CmsTestUtil.StreamToByteArray(recData.ContentStream)));

			ep.Close();
		}

		[Test]
		public void TestOriginatorInfo()
		{
			CmsEnvelopedDataParser env = new CmsEnvelopedDataParser(CmsSampleMessages.originatorMessage);

			env.GetRecipientInfos();

			Assert.AreEqual(CmsEnvelopedDataGenerator.DesEde3Cbc, env.EncryptionAlgOid);
		}
	}
}
