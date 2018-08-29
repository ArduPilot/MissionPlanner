using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	[TestFixture]
	public class EncryptedPrivateKeyInfoTest
		: SimpleTest
	{
		private const string alg = "1.2.840.113549.1.12.1.3"; // 3 key triple DES with SHA-1

		public override void PerformTest()
		{
			IAsymmetricCipherKeyPairGenerator fact = GeneratorUtilities.GetKeyPairGenerator("RSA");
			fact.Init(new KeyGenerationParameters(new SecureRandom(), 512));

			AsymmetricCipherKeyPair keyPair = fact.GenerateKeyPair();

			AsymmetricKeyParameter priKey = keyPair.Private;
			AsymmetricKeyParameter pubKey = keyPair.Public;

			//
			// set up the parameters
			//
			byte[] salt = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			int iterationCount = 100;
			Asn1Encodable defParams = PbeUtilities.GenerateAlgorithmParameters(alg, salt, iterationCount);
			char[] password1 = { 'h', 'e', 'l', 'l', 'o' };

//				AlgorithmParameters parameters = AlgorithmParameters.getInstance(alg);
//
//				parameters.init(defParams);

			//
			// set up the key
			//
//				PBEKeySpec pbeSpec = new PBEKeySpec(password1);
//				SecretKeyFactory keyFact = SecretKeyFactory.getInstance(alg);

//				IBufferedCipher cipher = CipherUtilities.GetCipher(alg);
			IWrapper wrapper = WrapperUtilities.GetWrapper(alg);

			ICipherParameters parameters = PbeUtilities.GenerateCipherParameters(
				alg, password1, defParams);

//				cipher.Init(IBufferedCipher.WRAP_MODE, keyFact.generateSecret(pbeSpec), parameters);
			wrapper.Init(true, parameters);

//				byte[] wrappedKey = cipher.Wrap(priKey);
			byte[] pkb = PrivateKeyInfoFactory.CreatePrivateKeyInfo(priKey).GetDerEncoded();
			byte[] wrappedKey = wrapper.Wrap(pkb, 0, pkb.Length);

			//
			// create encrypted object
			//

			// TODO Figure out what this was supposed to do
//				EncryptedPrivateKeyInfo pInfo = new EncryptedPrivateKeyInfo(parameters, wrappedKey);
			PrivateKeyInfo plain = PrivateKeyInfoFactory.CreatePrivateKeyInfo(priKey);
			EncryptedPrivateKeyInfo pInfo = EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(
				alg, password1, salt, iterationCount,  plain);


			//
			// decryption step
			//
			char[] password2 = { 'h', 'e', 'l', 'l', 'o' };

//				pbeSpec = new PBEKeySpec(password2);
//
//				cipher = CipherUtilities.GetCipher(pInfo.EncryptionAlgorithm);
//
//				cipher.Init(false, keyFact.generateSecret(pbeSpec), pInfo.getAlgParameters());
//
//				PKCS8EncodedKeySpec keySpec = pInfo.getKeySpec(cipher);
			PrivateKeyInfo decrypted = PrivateKeyInfoFactory.CreatePrivateKeyInfo(password2, pInfo);

//				if (!MessageDigest.isEqual(priKey.GetEncoded(), keySpec.GetEncoded()))
			if (!decrypted.Equals(plain))
			{
				Fail("Private key does not match");
			}

			//
			// using ICipherParameters test
			//
//			pbeSpec = new PBEKeySpec(password1);
//			keyFact = SecretKeyFactory.getInstance(alg);
//			cipher = CipherUtilities.GetCipher(alg);
			wrapper = WrapperUtilities.GetWrapper(alg);

//			cipher.init(IBufferedCipher.WRAP_MODE, keyFact.generateSecret(pbeSpec), parameters);
			wrapper.Init(true, parameters);

//			wrappedKey = cipher.wrap(priKey);
			wrappedKey = wrapper.Wrap(pkb, 0, pkb.Length);

			//
			// create encrypted object
			//

			// TODO Figure out what this was supposed to do
//			pInfo = new EncryptedPrivateKeyInfo(cipher.getParameters(), wrappedKey);
			plain = PrivateKeyInfoFactory.CreatePrivateKeyInfo(priKey);
			pInfo = EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(
				alg, password1, salt, iterationCount,  plain);

			//
			// decryption step
			//
//			pbeSpec = new PBEKeySpec(password2);
//
//			cipher = CipherUtilities.GetCipher(pInfo.getAlgName());
//
//			cipher.init(IBufferedCipher.DECRYPT_MODE, keyFact.generateSecret(pbeSpec), pInfo.getAlgParameters());
//
//			keySpec = pInfo.getKeySpec(cipher);
			decrypted = PrivateKeyInfoFactory.CreatePrivateKeyInfo(password2, pInfo);

//			if (!MessageDigest.isEqual(priKey.GetEncoded(), keySpec.GetEncoded()))
			if (!decrypted.Equals(plain))
			{
				Fail("Private key does not match");
			}
		}

		public override string Name
		{
			get { return "EncryptedPrivateKeyInfoTest"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new EncryptedPrivateKeyInfoTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
