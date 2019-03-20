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
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class RsaBlindedTest
		: SimpleTest
	{
		static BigInteger mod = new BigInteger("b259d2d6e627a768c94be36164c2d9fc79d97aab9253140e5bf17751197731d6f7540d2509e7b9ffee0a70a6e26d56e92d2edd7f85aba85600b69089f35f6bdbf3c298e05842535d9f064e6b0391cb7d306e0a2d20c4dfb4e7b49a9640bdea26c10ad69c3f05007ce2513cee44cfe01998e62b6c3637d3fc0391079b26ee36d5", 16);
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
			get { return "RSABlinded"; }
		}

		private void doTestStrictPkcs1Length(RsaKeyParameters pubParameters, RsaKeyParameters privParameters)
		{
			IAsymmetricBlockCipher eng = new RsaBlindedEngine();

			eng.Init(true, privParameters);

			byte[] data = null;

			try
			{
				data = eng.ProcessBlock(oversizedSig, 0, oversizedSig.Length);
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

				Fail("oversized signature block not recognised");
			}
			catch (InvalidCipherTextException e)
			{
				if (!e.Message.Equals("block incorrect size"))
				{
					Fail("RSA: failed - exception " + e.ToString(), e);
				}
			}


			// Create the encoding with StrictLengthEnabled=false (done thru environment in Java version)
			Pkcs1Encoding.StrictLengthEnabled = false;

			eng = new Pkcs1Encoding(new RsaBlindedEngine());

			eng.Init(false, pubParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
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
			IAsymmetricBlockCipher eng = new RsaBlindedEngine();

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
			IAsymmetricBlockCipher eng = new OaepEncoding(new RsaBlindedEngine());
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
		}

		public override void PerformTest()
		{
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod, pubExp);
			RsaKeyParameters privParameters = new RsaPrivateCrtKeyParameters(mod, pubExp, privExp, p, q, pExp, qExp, crtCoef);
			byte[] data = Hex.Decode(edgeInput);

			//
			// RAW
			//
			IAsymmetricBlockCipher eng = new RsaBlindedEngine();

			eng.Init(true, pubParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("RSA: failed - exception " + e.ToString(), e);
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
				Fail("failed PKCS1 public/private Test");
			}

			//
			// PKCS1 - private encrypt, public decrypt
			//
			eng = new Pkcs1Encoding(((Pkcs1Encoding)eng).GetUnderlyingCipher());

			eng.Init(true, privParameters);

			try
			{
				data = eng.ProcessBlock(data, 0, data.Length);
			}
			catch (Exception e)
			{
				Fail("failed - exception " + e.ToString(), e);
			}

			eng.Init(false, pubParameters);

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
				Fail("failed PKCS1 private/public Test");
			}

			//
			// key generation test
			//
			RsaKeyPairGenerator pGen = new RsaKeyPairGenerator();
			RsaKeyGenerationParameters genParam = new RsaKeyGenerationParameters(
				BigInteger.ValueOf(0x11), new SecureRandom(), 768, 25);

			pGen.Init(genParam);

			AsymmetricCipherKeyPair pair = pGen.GenerateKeyPair();

			eng = new RsaBlindedEngine();

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
				Fail("failed - exception " + e.ToString(), e);
			}

			eng.Init(false, pair.Private);

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
				Fail("failed - exception " + e.ToString(), e);
			}

			eng.Init(false, pair.Private);

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
				Fail("failed key generation (1024) test");
			}

			doTestOaep(pubParameters, privParameters);
			doTestStrictPkcs1Length(pubParameters, privParameters);
			doTestDudPkcs1Block(pubParameters, privParameters);
			doTestMissingDataPkcs1Block(pubParameters, privParameters);
			doTestTruncatedPkcs1Block(pubParameters, privParameters);
			doTestWrongPaddingPkcs1Block(pubParameters, privParameters);

			try
			{
				new RsaBlindedEngine().ProcessBlock(new byte[]{ 1 }, 0, 1);
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
			ITest test = new RsaBlindedTest();
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
