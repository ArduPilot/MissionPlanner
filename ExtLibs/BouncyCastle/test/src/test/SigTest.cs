using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	[TestFixture]
	public class SigTest
		: SimpleTest
	{
		/**
		 * signature with a "forged signature" (sig block not at end of plain text)
		 */
		private void doTestBadSig(
			AsymmetricKeyParameter	priv,
			AsymmetricKeyParameter	pub)
		{
			IDigest sha1 = DigestUtilities.GetDigest("SHA1");
			IBufferedCipher signer = CipherUtilities.GetCipher("RSA//PKCS1Padding");

			signer.Init(true, priv);

			byte[] block = new byte[signer.GetBlockSize()];

			sha1.Update((byte)0);

			byte[] sigHeader = Hex.Decode("3021300906052b0e03021a05000414");
			Array.Copy(sigHeader, 0, block, 0, sigHeader.Length);

//			byte[] dig = sha1.digest();
			byte[] dig = DigestUtilities.DoFinal(sha1);

			Array.Copy(dig, 0, block, sigHeader.Length, dig.Length);

			Array.Copy(sigHeader, 0, block,
				sigHeader.Length + dig.Length, sigHeader.Length);

			byte[] sig = signer.DoFinal(block);

			ISigner verifier = SignerUtilities.GetSigner("SHA1WithRSA");

			verifier.Init(false, pub);

			verifier.Update((byte)0);

			if (verifier.VerifySignature(sig))
			{
				Fail("bad signature passed");
			}
		}

		public override void PerformTest()
		{
			ISigner sig = SignerUtilities.GetSigner("SHA1WithRSAEncryption");

			byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

			IAsymmetricCipherKeyPairGenerator fact = GeneratorUtilities.GetKeyPairGenerator("RSA");
			fact.Init(
				new RsaKeyGenerationParameters(
					BigInteger.ValueOf(0x10001),
					new SecureRandom(),
					768,
					25));

			AsymmetricCipherKeyPair keyPair = fact.GenerateKeyPair();

			AsymmetricKeyParameter signingKey = keyPair.Private;
			AsymmetricKeyParameter verifyKey = keyPair.Public;

			doTestBadSig(signingKey, verifyKey);

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			byte[] sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("SHA1 verification failed");
			}

			sig = SignerUtilities.GetSigner("MD2WithRSAEncryption");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("MD2 verification failed");
			}

			sig = SignerUtilities.GetSigner("MD5WithRSAEncryption");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("MD5 verification failed");
			}

			sig = SignerUtilities.GetSigner("RIPEMD160WithRSAEncryption");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("RIPEMD160 verification failed");
			}

			//
			// RIPEMD-128
			//
			sig = SignerUtilities.GetSigner("RIPEMD128WithRSAEncryption");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("RIPEMD128 verification failed");
			}

			//
			// RIPEMD256
			//
			sig = SignerUtilities.GetSigner("RIPEMD256WithRSAEncryption");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("RIPEMD256 verification failed");
			}

			//
			// SHA-224
			//
			sig = SignerUtilities.GetSigner("SHA224WithRSAEncryption");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("SHA224 verification failed");
			}

			//
			// SHA-256
			//
			sig = SignerUtilities.GetSigner("SHA256WithRSAEncryption");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("SHA256 verification failed");
			}

			//
			// SHA-384
			//
			sig = SignerUtilities.GetSigner("SHA384WithRSAEncryption");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("SHA384 verification failed");
			}

			//
			// SHA-512
			//
			sig = SignerUtilities.GetSigner("SHA512WithRSAEncryption");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("SHA512 verification failed");
			}

			//
			// ISO Sigs.
			//
			sig = SignerUtilities.GetSigner("MD5WithRSA/ISO9796-2");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("MD5/ISO verification failed");
			}

			sig = SignerUtilities.GetSigner("SHA1WithRSA/ISO9796-2");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("SHA1/ISO verification failed");
			}

			sig = SignerUtilities.GetSigner("RIPEMD160WithRSA/ISO9796-2");

			sig.Init(true, signingKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			sig.Init(false, verifyKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("RIPEMD160/ISO verification failed");
			}

			//
			// standard vector test - B.1.3 RIPEMD160, implicit.
			//
			BigInteger  mod = new BigInteger("ffffffff78f6c55506c59785e871211ee120b0b5dd644aa796d82413a47b24573f1be5745b5cd9950f6b389b52350d4e01e90009669a8720bf265a2865994190a661dea3c7828e2e7ca1b19651adc2d5", 16);
			BigInteger  pub = new BigInteger("03", 16);
			BigInteger  pri = new BigInteger("2aaaaaaa942920e38120ee965168302fd0301d73a4e60c7143ceb0adf0bf30b9352f50e8b9e4ceedd65343b2179005b2f099915e4b0c37e41314bb0821ad8330d23cba7f589e0f129b04c46b67dfce9d", 16);

//			KeyFactory  f = KeyFactory.getInstance("RSA");
//			AsymmetricKeyParameter privKey = f.generatePrivate(new RSAPrivateKeySpec(mod, pri));
//			AsymmetricKeyParameter pubKey = f.generatePublic(new RSAPublicKeySpec(mod, pub));
			AsymmetricKeyParameter privKey = new RsaKeyParameters(true, mod, pri);
			AsymmetricKeyParameter pubKey = new RsaKeyParameters(false, mod, pub);
			byte[] testSig = Hex.Decode("5cf9a01854dbacaec83aae8efc563d74538192e95466babacd361d7c86000fe42dcb4581e48e4feb862d04698da9203b1803b262105104d510b365ee9c660857ba1c001aa57abfd1c8de92e47c275cae");

			data = Hex.Decode("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210");

			sig = SignerUtilities.GetSigner("RIPEMD160WithRSA/ISO9796-2");

			sig.Init(true, privKey);

			sig.BlockUpdate(data, 0, data.Length);

			sigBytes = sig.GenerateSignature();

			if (!AreEqual(testSig, sigBytes))
			{
				Fail("SigTest: failed ISO9796-2 generation Test");
			}

			sig.Init(false, pubKey);

			sig.BlockUpdate(data, 0, data.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("RIPEMD160/ISO verification failed");
			}
		}

		public override string Name
		{
			get { return "SigTest"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new SigTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
