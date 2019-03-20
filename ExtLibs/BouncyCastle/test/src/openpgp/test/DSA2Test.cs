using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
	/**
	* GPG compatability test vectors
	*/
	[TestFixture]
	public class Dsa2Test
		//extends TestCase
	{
		[Test]
		public void TestK1024H160()
		{
			doSigVerifyTest("DSA-1024-160.pub", "dsa-1024-160-sign.gpg");
		}

		[Test]
		public void TestK1024H224()
		{
			doSigVerifyTest("DSA-1024-160.pub", "dsa-1024-224-sign.gpg");
		}

		[Test]
		public void TestK1024H256()
		{
			doSigVerifyTest("DSA-1024-160.pub", "dsa-1024-256-sign.gpg");
		}

		[Test]
		public void TestK1024H384()
		{
			doSigVerifyTest("DSA-1024-160.pub", "dsa-1024-384-sign.gpg");
		}

		[Test]
		public void TestK1024H512()
		{
			doSigVerifyTest("DSA-1024-160.pub", "dsa-1024-512-sign.gpg");
		}

		[Test]
		public void TestK2048H224()
		{
			doSigVerifyTest("DSA-2048-224.pub", "dsa-2048-224-sign.gpg");
		}

		[Test]
		public void TestK3072H256()
		{
			doSigVerifyTest("DSA-3072-256.pub", "dsa-3072-256-sign.gpg");
		}

		[Test]
		public void TestK7680H384()
		{
			doSigVerifyTest("DSA-7680-384.pub", "dsa-7680-384-sign.gpg");
		}

		[Test]
		public void TestK15360H512()
		{
			doSigVerifyTest("DSA-15360-512.pub", "dsa-15360-512-sign.gpg");
		}

		[Test]
		public void TestGenerateK1024H224()
		{
			doSigGenerateTest("DSA-1024-160.sec", "DSA-1024-160.pub", HashAlgorithmTag.Sha224);
		}

		[Test]
		public void TestGenerateK1024H256()
		{
			doSigGenerateTest("DSA-1024-160.sec", "DSA-1024-160.pub", HashAlgorithmTag.Sha256);
		}

		[Test]
		public void TestGenerateK1024H384()
		{
			doSigGenerateTest("DSA-1024-160.sec", "DSA-1024-160.pub", HashAlgorithmTag.Sha384);
		}

		[Test]
		public void TestGenerateK1024H512()
		{
			doSigGenerateTest("DSA-1024-160.sec", "DSA-1024-160.pub", HashAlgorithmTag.Sha512);
		}

		[Test]
		public void TestGenerateK2048H256()
		{
			doSigGenerateTest("DSA-2048-224.sec", "DSA-2048-224.pub", HashAlgorithmTag.Sha256);
		}

		[Test]
		public void TestGenerateK2048H512()
		{
			doSigGenerateTest("DSA-2048-224.sec", "DSA-2048-224.pub", HashAlgorithmTag.Sha512);
		}

		private void doSigGenerateTest(
			string				privateKeyFile,
			string				publicKeyFile,
			HashAlgorithmTag	digest)
		{
			PgpSecretKeyRing		secRing = loadSecretKey(privateKeyFile);
			PgpPublicKeyRing		pubRing = loadPublicKey(publicKeyFile);
			string					data = "hello world!";
			byte[]					dataBytes = Encoding.ASCII.GetBytes(data);
			MemoryStream			bOut = new MemoryStream();
			MemoryStream			testIn = new MemoryStream(dataBytes, false);
			PgpSignatureGenerator	sGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.Dsa, digest);

			sGen.InitSign(PgpSignature.BinaryDocument, secRing.GetSecretKey().ExtractPrivateKey("test".ToCharArray()));

			BcpgOutputStream bcOut = new BcpgOutputStream(bOut);

			sGen.GenerateOnePassVersion(false).Encode(bcOut);

			PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();

//			Date testDate = new Date((System.currentTimeMillis() / 1000) * 1000);
			DateTime testDate = new DateTime(
				(DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);

			Stream lOut = lGen.Open(
				new UncloseableStream(bcOut),
				PgpLiteralData.Binary,
				"_CONSOLE",
				dataBytes.Length,
				testDate);

			int ch;
			while ((ch = testIn.ReadByte()) >= 0)
			{
				lOut.WriteByte((byte)ch);
				sGen.Update((byte)ch);
			}

			lGen.Close();

			sGen.Generate().Encode(bcOut);

			PgpObjectFactory        pgpFact = new PgpObjectFactory(bOut.ToArray());
			PgpOnePassSignatureList p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();
			PgpOnePassSignature     ops = p1[0];

			Assert.AreEqual(digest, ops.HashAlgorithm);
			Assert.AreEqual(PublicKeyAlgorithmTag.Dsa, ops.KeyAlgorithm);

			PgpLiteralData          p2 = (PgpLiteralData)pgpFact.NextPgpObject();
			if (!p2.ModificationTime.Equals(testDate))
			{
				Assert.Fail("Modification time not preserved");
			}

			Stream dIn = p2.GetInputStream();

			ops.InitVerify(pubRing.GetPublicKey());

			while ((ch = dIn.ReadByte()) >= 0)
			{
				ops.Update((byte)ch);
			}

			PgpSignatureList p3 = (PgpSignatureList)pgpFact.NextPgpObject();
			PgpSignature sig = p3[0];

			Assert.AreEqual(digest, sig.HashAlgorithm);
			Assert.AreEqual(PublicKeyAlgorithmTag.Dsa, sig.KeyAlgorithm);

			Assert.IsTrue(ops.Verify(sig));
		}

		private void doSigVerifyTest(
			string	publicKeyFile,
			string	sigFile)
		{
			PgpPublicKeyRing publicKey = loadPublicKey(publicKeyFile);
			PgpObjectFactory pgpFact = loadSig(sigFile);

			PgpCompressedData c1 = (PgpCompressedData)pgpFact.NextPgpObject();

			pgpFact = new PgpObjectFactory(c1.GetDataStream());

			PgpOnePassSignatureList p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();
			PgpOnePassSignature ops = p1[0];

			PgpLiteralData p2 = (PgpLiteralData)pgpFact.NextPgpObject();

			Stream dIn = p2.GetInputStream();

			ops.InitVerify(publicKey.GetPublicKey());

			int ch;
			while ((ch = dIn.ReadByte()) >= 0)
			{
				ops.Update((byte)ch);
			}

			PgpSignatureList p3 = (PgpSignatureList)pgpFact.NextPgpObject();

			Assert.IsTrue(ops.Verify(p3[0]));
		}

		private PgpObjectFactory loadSig(
			string sigName)
		{
			Stream fIn = SimpleTest.GetTestDataAsStream("openpgp.dsa.sigs." + sigName);

			return new PgpObjectFactory(fIn);
		}

		private PgpPublicKeyRing loadPublicKey(
			string keyName)
		{
			Stream fIn = SimpleTest.GetTestDataAsStream("openpgp.dsa.keys." + keyName);

			return new PgpPublicKeyRing(fIn);
		}

		private PgpSecretKeyRing loadSecretKey(
			string keyName)
		{
			Stream fIn = SimpleTest.GetTestDataAsStream("openpgp.dsa.keys." + keyName);

			return new PgpSecretKeyRing(fIn);
		}
	}
}
