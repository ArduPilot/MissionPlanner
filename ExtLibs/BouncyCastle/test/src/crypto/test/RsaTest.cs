using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class RsaTest
		: SimpleTest
	{
        /*
         * Based on https://github.com/crocs-muni/roca/blob/master/java/BrokenKey.java
         * Credits: ported to Java by Martin Paljak
         */
        internal class BrokenKey_CVE_2017_15361
        {
            private static readonly int[] prims = new int[]{ 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61,
                67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167 };
            private static readonly BigInteger[] primes = new BigInteger[prims.Length];

            static BrokenKey_CVE_2017_15361()
            {
                for (int i = 0; i < prims.Length; i++)
                {
                    primes[i] = BigInteger.ValueOf(prims[i]);
                }
            }

            private static readonly BigInteger[] markers = new BigInteger[]
            {
                new BigInteger("6"),
                new BigInteger("30"),
                new BigInteger("126"),
                new BigInteger("1026"),
                new BigInteger("5658"),
                new BigInteger("107286"),
                new BigInteger("199410"),
                new BigInteger("8388606"),
                new BigInteger("536870910"),
                new BigInteger("2147483646"),
                new BigInteger("67109890"),
                new BigInteger("2199023255550"),
                new BigInteger("8796093022206"),
                new BigInteger("140737488355326"),
                new BigInteger("5310023542746834"),
                new BigInteger("576460752303423486"),
                new BigInteger("1455791217086302986"),
                new BigInteger("147573952589676412926"),
                new BigInteger("20052041432995567486"),
                new BigInteger("6041388139249378920330"),
                new BigInteger("207530445072488465666"),
                new BigInteger("9671406556917033397649406"),
                new BigInteger("618970019642690137449562110"),
                new BigInteger("79228162521181866724264247298"),
                new BigInteger("2535301200456458802993406410750"),
                new BigInteger("1760368345969468176824550810518"),
                new BigInteger("50079290986288516948354744811034"),
                new BigInteger("473022961816146413042658758988474"),
                new BigInteger("10384593717069655257060992658440190"),
                new BigInteger("144390480366845522447407333004847678774"),
                new BigInteger("2722258935367507707706996859454145691646"),
                new BigInteger("174224571863520493293247799005065324265470"),
                new BigInteger("696898287454081973172991196020261297061886"),
                new BigInteger("713623846352979940529142984724747568191373310"),
                new BigInteger("1800793591454480341970779146165214289059119882"),
                new BigInteger("126304807362733370595828809000324029340048915994"),
                new BigInteger("11692013098647223345629478661730264157247460343806"),
                new BigInteger("187072209578355573530071658587684226515959365500926")
            };

            public static bool IsAffected(RsaKeyParameters publicKey)
            {
                BigInteger modulus = publicKey.Modulus;

                for (int i = 0; i < primes.Length; i++)
                {
                    int remainder = modulus.Remainder(primes[i]).IntValue;
                    if (!markers[i].TestBit(remainder))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        static BigInteger  mod = new BigInteger("b259d2d6e627a768c94be36164c2d9fc79d97aab9253140e5bf17751197731d6f7540d2509e7b9ffee0a70a6e26d56e92d2edd7f85aba85600b69089f35f6bdbf3c298e05842535d9f064e6b0391cb7d306e0a2d20c4dfb4e7b49a9640bdea26c10ad69c3f05007ce2513cee44cfe01998e62b6c3637d3fc0391079b26ee36d5", 16);
		static BigInteger  pubExp = new BigInteger("11", 16);
		static BigInteger  privExp = new BigInteger("92e08f83cc9920746989ca5034dcb384a094fb9c5a6288fcc4304424ab8f56388f72652d8fafc65a4b9020896f2cde297080f2a540e7b7ce5af0b3446e1258d1dd7f245cf54124b4c6e17da21b90a0ebd22605e6f45c9f136d7a13eaac1c0f7487de8bd6d924972408ebb58af71e76fd7b012a8d0e165f3ae2e5077a8648e619", 16);
		static BigInteger  p = new BigInteger("f75e80839b9b9379f1cf1128f321639757dba514642c206bbbd99f9a4846208b3e93fbbe5e0527cc59b1d4b929d9555853004c7c8b30ee6a213c3d1bb7415d03", 16);
		static BigInteger  q = new BigInteger("b892d9ebdbfc37e397256dd8a5d3123534d1f03726284743ddc6be3a709edb696fc40c7d902ed804c6eee730eee3d5b20bf6bd8d87a296813c87d3b3cc9d7947", 16);
		static BigInteger  pExp = new BigInteger("1d1a2d3ca8e52068b3094d501c9a842fec37f54db16e9a67070a8b3f53cc03d4257ad252a1a640eadd603724d7bf3737914b544ae332eedf4f34436cac25ceb5", 16);
		static BigInteger  qExp = new BigInteger("6c929e4e81672fef49d9c825163fec97c4b7ba7acb26c0824638ac22605d7201c94625770984f78a56e6e25904fe7db407099cad9b14588841b94f5ab498dded", 16);
		static BigInteger  crtCoef = new BigInteger("dae7651ee69ad1d081ec5e7188ae126f6004ff39556bde90e0b870962fa7b926d070686d8244fe5a9aa709a95686a104614834b0ada4b10f53197a5cb4c97339", 16);

		static string input = "4e6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e";

		//
		// to check that we handling byte extension by big number correctly.
		//
		static string edgeInput = "ff6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e";

		static byte[] oversizedSig = Hex.Decode("01ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff004e6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e");
		static byte[] dudBlock = Hex.Decode("000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff004e6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e");
		static byte[] truncatedDataBlock = Hex.Decode("0001ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff004e6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e");
		static byte[] incorrectPadding = Hex.Decode("0001ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff4e6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e");
		static byte[] missingDataBlock = Hex.Decode("0001ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff");

		public override string Name
		{
			get { return "RSA"; }
		}

		private void doTestStrictPkcs1Length(RsaKeyParameters pubParameters, RsaKeyParameters privParameters)
		{
			IAsymmetricBlockCipher   eng = new RsaEngine();

			eng.Init(true, privParameters);

			byte[] data = null;
            byte[] overSized = null;

            try
            {
				overSized = data = eng.ProcessBlock(oversizedSig, 0, oversizedSig.Length);
			}
			catch (Exception e)
			{
				Fail("RSA: failed - exception " + e.ToString(), e);
			}

			eng = new Pkcs1Encoding(eng);

			eng.Init(false, pubParameters);

			try
			{
				data = eng.ProcessBlock(overSized, 0, overSized.Length);

				Fail("oversized signature block not recognised");
			}
			catch (InvalidCipherTextException e)
			{
				if (!e.Message.Equals("block incorrect size"))
				{
					Fail("RSA: failed - exception " + e.ToString(), e);
				}
			}

            eng = new Pkcs1Encoding(new RsaEngine(), Hex.Decode("feedbeeffeedbeeffeedbeef"));
            eng.Init(false, new ParametersWithRandom(privParameters, new SecureRandom()));

            try
            {
                data = eng.ProcessBlock(overSized, 0, overSized.Length);
                IsTrue("not fallback", Arrays.AreEqual(Hex.Decode("feedbeeffeedbeeffeedbeef"), data));
            }
            catch (InvalidCipherTextException e)
            {
                Fail("RSA: failed - exception " + e.ToString(), e);
            }


            // Create the encoding with StrictLengthEnabled=false (done thru environment in Java version)
            Pkcs1Encoding.StrictLengthEnabled = false;

			eng = new Pkcs1Encoding(new RsaEngine());

			eng.Init(false, pubParameters);

			try
			{
				data = eng.ProcessBlock(overSized, 0, overSized.Length);
			}
			catch (InvalidCipherTextException e)
			{
				Fail("RSA: failed - exception " + e.ToString(), e);
			}

			Pkcs1Encoding.StrictLengthEnabled = true;
		}

		private void doTestTruncatedPkcs1Block(RsaKeyParameters pubParameters, RsaKeyParameters privParameters)
		{
			checkForPkcs1Exception(pubParameters, privParameters, truncatedDataBlock, "block incorrect");
		}

		private void doTestDudPkcs1Block(RsaKeyParameters pubParameters, RsaKeyParameters privParameters)
		{
			checkForPkcs1Exception(pubParameters, privParameters, dudBlock, "block incorrect");
		}

		private void doTestWrongPaddingPkcs1Block(RsaKeyParameters pubParameters, RsaKeyParameters privParameters)
		{
			checkForPkcs1Exception(pubParameters, privParameters, incorrectPadding, "block incorrect");
		}

		private void doTestMissingDataPkcs1Block(RsaKeyParameters pubParameters, RsaKeyParameters privParameters)
		{
			checkForPkcs1Exception(pubParameters, privParameters, missingDataBlock, "block incorrect");
		}

		private void checkForPkcs1Exception(RsaKeyParameters pubParameters, RsaKeyParameters privParameters, byte[] inputData, string expectedMessage)
		{
			IAsymmetricBlockCipher   eng = new RsaEngine();

			eng.Init(true, privParameters);

			byte[] data = null;

			try
			{
				data = eng.ProcessBlock(inputData, 0, inputData.Length);
			}
			catch (Exception e)
			{
				Fail("RSA: failed - exception " + e.ToString(), e);
			}

			eng = new Pkcs1Encoding(eng);

			eng.Init(false, pubParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);

				Fail("missing data block not recognised");
			}
			catch (InvalidCipherTextException e)
			{
				if (!e.Message.Equals(expectedMessage))
				{
					Fail("RSA: failed - exception " + e.ToString(), e);
				}
			}
		}

		private void doTestOaep(RsaKeyParameters pubParameters, RsaKeyParameters privParameters)
		{
			//
			// OAEP - public encrypt, private decrypt
			//
			IAsymmetricBlockCipher eng = new OaepEncoding(new RsaEngine());
			byte[] data = Hex.Decode(input);

			eng.Init(true, pubParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString(), e);
			}

			eng.Init(false, privParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString(), e);
			}

			if (!input.Equals(Hex.ToHexString(data)))
			{
				Fail("failed OAEP Test");
			}

            // check for oversized input
            byte[] message = new byte[87];
            RsaEngine rsaEngine = new RsaEngine();
            IAsymmetricBlockCipher cipher = new OaepEncoding(rsaEngine, new Sha1Digest(), new Sha1Digest(), message);
            cipher.Init(true, new ParametersWithRandom(pubParameters, new SecureRandom()));

            try
            {
                cipher.ProcessBlock(message, 0, message.Length);

                Fail("no exception thrown");
            }
            catch (DataLengthException e)
            {
                IsTrue("message mismatch", "input data too long".Equals(e.Message));
            }
            catch (InvalidCipherTextException e)
            {
                Fail("failed - exception " + e.ToString(), e);
            }
        }

        // TODO Move this when other JCE tests are ported from Java
        /**
		 * signature with a "forged signature" (sig block not at end of plain text)
		 */
        private void doTestBadSig()//PrivateKey priv, PublicKey pub)
		{
//			Signature           sig = Signature.getInstance("SHA1WithRSAEncryption", "BC");
			ISigner sig = SignerUtilities.GetSigner("SHA1WithRSAEncryption");
//			KeyPairGenerator    fact;
//			KeyPair             keyPair;
//			byte[]              data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

//			fact = KeyPairGenerator.getInstance("RSA", "BC");
			RsaKeyPairGenerator fact = new RsaKeyPairGenerator();

//			fact.initialize(768, new SecureRandom());
			RsaKeyGenerationParameters factParams = new RsaKeyGenerationParameters(
//				BigInteger.ValueOf(0x11), new SecureRandom(), 768, 25);
				BigInteger.ValueOf(3), new SecureRandom(), 768, 25);
			fact.Init(factParams);

//			keyPair = fact.generateKeyPair();
//
//			PrivateKey  signingKey = keyPair.getPrivate();
//			PublicKey   verifyKey = keyPair.getPublic();
			AsymmetricCipherKeyPair keyPair = fact.GenerateKeyPair();

			AsymmetricKeyParameter priv = keyPair.Private;
			AsymmetricKeyParameter pub = keyPair.Public;

//			testBadSig(signingKey, verifyKey);





//			MessageDigest sha1 = MessageDigest.getInstance("SHA1", "BC");
			IDigest sha1 = DigestUtilities.GetDigest("SHA1");

//			Cipher signer = Cipher.getInstance("RSA/ECB/PKCS1Padding", "BC");
//			IBufferedCipher signer = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
			IAsymmetricBlockCipher signer = new Pkcs1Encoding(new RsaEngine());

//			signer.init(Cipher.ENCRYPT_MODE, priv);
			signer.Init(true, priv);

//			byte[] block = new byte[signer.getBlockSize()];
//			byte[] block = new byte[signer.GetBlockSize()];
			byte[] block = new byte[signer.GetInputBlockSize()];

//			sha1.update((byte)0);
			sha1.Update(0);

//			byte[] sigHeader = Hex.decode("3021300906052b0e03021a05000414");
			byte[] sigHeader = Hex.Decode("3021300906052b0e03021a05000414");
//			System.arraycopy(sigHeader, 0, block, 0, sigHeader.length);
			Array.Copy(sigHeader, 0, block, 0, sigHeader.Length);

//			sha1.digest(block, sigHeader.length, sha1.getDigestLength());
			sha1.DoFinal(block, sigHeader.Length);

//			System.arraycopy(sigHeader, 0, block,
//				sigHeader.length + sha1.getDigestLength(), sigHeader.length);
			Array.Copy(sigHeader, 0, block,
				sigHeader.Length + sha1.GetDigestSize(), sigHeader.Length);

//			byte[] sigBytes = signer.doFinal(block);
			byte[] sigBytes = signer.ProcessBlock(block, 0, block.Length);

//			Signature verifier = Signature.getInstance("SHA1WithRSA", "BC");
			ISigner verifier = SignerUtilities.GetSigner("SHA1WithRSA");

//			verifier.initVerify(pub);
			verifier.Init(false, pub);

//			verifier.update((byte)0);
			verifier.Update(0);

//			if (verifier.verify(sig))
			if (verifier.VerifySignature(sigBytes))
			{
//				fail("bad signature passed");
				Fail("bad signature passed");
			}
		}

		private void testZeroBlock(ICipherParameters encParameters, ICipherParameters decParameters)
		{
			IAsymmetricBlockCipher eng = new Pkcs1Encoding(new RsaEngine());
		
			eng.Init(true, encParameters);

			if (eng.GetOutputBlockSize() != ((Pkcs1Encoding)eng).GetUnderlyingCipher().GetOutputBlockSize())
			{
				Fail("PKCS1 output block size incorrect");
			}

			byte[] zero = new byte[0];
			byte[] data = null;

			try
			{
				data = eng.ProcessBlock(zero, 0, zero.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString(), e);
			}

			eng.Init(false, decParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString(), e);
			}

			if (!Arrays.AreEqual(zero, data))
			{
				Fail("failed PKCS1 zero Test");
			}
		}

        private void doTest_CVE_2017_15361()
        {
            SecureRandom random = new SecureRandom();
            RsaKeyPairGenerator pGen = new RsaKeyPairGenerator();
            BigInteger e = BigInteger.ValueOf(0x11);

            for (int strength = 512; strength <= 2048; strength += 32)
            {
                pGen.Init(new RsaKeyGenerationParameters(
                    e, random, strength, 100));

                RsaKeyParameters pubKey = (RsaKeyParameters)pGen.GenerateKeyPair().Public;

                if (BrokenKey_CVE_2017_15361.IsAffected(pubKey))
                {
                    Fail("failed CVE-2017-15361 vulnerability test for generated RSA key");
                }
            }
        }

		public override void PerformTest()
		{
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod, pubExp);
			RsaKeyParameters privParameters = new RsaPrivateCrtKeyParameters(mod, pubExp, privExp, p, q, pExp, qExp, crtCoef);
			byte[] data = Hex.Decode(edgeInput);

			//
			// RAW
			//
			IAsymmetricBlockCipher   eng = new RsaEngine();

			eng.Init(true, pubParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("RSA: failed - exception " + e.ToString());
			}

			eng.Init(false, privParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			if (!edgeInput.Equals(Hex.ToHexString(data)))
			{
				Fail("failed RAW edge Test");
			}

			data = Hex.Decode(input);

			eng.Init(true, pubParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			eng.Init(false, privParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			if (!input.Equals(Hex.ToHexString(data)))
			{
				Fail("failed RAW Test");
			}

			//
			// PKCS1 - public encrypt, private decrypt
			//
			eng = new Pkcs1Encoding(eng);

			eng.Init(true, pubParameters);

			if (eng.GetOutputBlockSize() != ((Pkcs1Encoding)eng).GetUnderlyingCipher().GetOutputBlockSize())
			{
				Fail("PKCS1 output block size incorrect");
			}

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			eng.Init(false, privParameters);

            byte[] plainData = null;
            try
            {
				plainData = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

            if (!input.Equals(Hex.ToHexString(plainData)))
            {
                Fail("failed PKCS1 public/private Test");
			}

            Pkcs1Encoding fEng = new Pkcs1Encoding(new RsaEngine(), input.Length / 2);
            fEng.Init(false, new ParametersWithRandom(privParameters, new SecureRandom()));
            try
            {
                plainData = fEng.ProcessBlock(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Fail("failed - exception " + e.ToString(), e);
            }

            if (!input.Equals(Hex.ToHexString(plainData)))
            {
                Fail("failed PKCS1 public/private fixed Test");
            }

            fEng = new Pkcs1Encoding(new RsaEngine(), input.Length);
            fEng.Init(false, new ParametersWithRandom(privParameters, new SecureRandom()));
            try
            {
                data = fEng.ProcessBlock(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Fail("failed - exception " + e.ToString(), e);
            }

            if (input.Equals(Hex.ToHexString(data)))
            {
                Fail("failed to recognise incorrect plaint text length");
            }

            data = plainData;

            //
            // PKCS1 - private encrypt, public decrypt
            //
            eng = new Pkcs1Encoding(((Pkcs1Encoding)eng).GetUnderlyingCipher());

			eng.Init(true, privParameters);

			try
			{
				data = eng.ProcessBlock(plainData, 0, plainData.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			eng.Init(false, pubParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			if (!input.Equals(Hex.ToHexString(data)))
			{
				Fail("failed PKCS1 private/public Test");
			}

			testZeroBlock(pubParameters, privParameters);
			testZeroBlock(privParameters, pubParameters);

			//
			// key generation test
			//
			RsaKeyPairGenerator  pGen = new RsaKeyPairGenerator();
			RsaKeyGenerationParameters  genParam = new RsaKeyGenerationParameters(
				BigInteger.ValueOf(0x11), new SecureRandom(), 768, 25);

			pGen.Init(genParam);

			AsymmetricCipherKeyPair  pair = pGen.GenerateKeyPair();

			eng = new RsaEngine();

			if (((RsaKeyParameters)pair.Public).Modulus.BitLength < 768)
			{
				Fail("failed key generation (768) length test");
			}

			eng.Init(true, pair.Public);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			eng.Init(false, pair.Private);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			if (!input.Equals(Hex.ToHexString(data)))
			{
				Fail("failed key generation (768) Test");
			}

			genParam = new RsaKeyGenerationParameters(BigInteger.ValueOf(0x11), new SecureRandom(), 1024, 25);

			pGen.Init(genParam);
			pair = pGen.GenerateKeyPair();

			eng.Init(true, pair.Public);

			if (((RsaKeyParameters)pair.Public).Modulus.BitLength < 1024)
			{
				Fail("failed key generation (1024) length test");
			}

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			eng.Init(false, pair.Private);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString());
			}

			if (!input.Equals(Hex.ToHexString(data)))
			{
				Fail("failed key generation (1024) test");
			}

			genParam = new RsaKeyGenerationParameters(
				BigInteger.ValueOf(0x11), new SecureRandom(), 128, 25);
			pGen.Init(genParam);

			for (int i = 0; i < 100; ++i)
			{
				pair = pGen.GenerateKeyPair();
				RsaPrivateCrtKeyParameters privKey = (RsaPrivateCrtKeyParameters) pair.Private;
				BigInteger pqDiff = privKey.P.Subtract(privKey.Q).Abs();

				if (pqDiff.BitLength < 42)
				{
					Fail("P and Q too close in RSA key pair");
				}
			}

			doTestBadSig();
			doTestOaep(pubParameters, privParameters);
			doTestStrictPkcs1Length(pubParameters, privParameters);
			doTestDudPkcs1Block(pubParameters, privParameters);
			doTestMissingDataPkcs1Block(pubParameters, privParameters);
			doTestTruncatedPkcs1Block(pubParameters, privParameters);
			doTestWrongPaddingPkcs1Block(pubParameters, privParameters);
            doTest_CVE_2017_15361();

			try
			{
				new RsaEngine().ProcessBlock(new byte[]{ 1 }, 0, 1);
				Fail("failed initialisation check");
			}
			catch (InvalidOperationException)
			{
				// expected
			}
		}

		public static void Main(
			string[] args)
		{
			ITest test = new RsaTest();
			ITestResult result = test.Perform();

			Console.WriteLine(result);
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
