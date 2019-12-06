using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	/**
	 * test for ECIES - Elliptic Curve Integrated Encryption Scheme
	 */
	[TestFixture]
	public class IesTest
		: SimpleTest
	{
		private static readonly BigInteger g512 = new BigInteger("153d5d6172adb43045b68ae8e1de1070b6137005686d29d3d73a7749199681ee5b212c9b96bfdcfa5b20cd5e3fd2044895d609cf9b410b7a0f12ca1cb9a428cc", 16);
		private static readonly BigInteger p512 = new BigInteger("9494fec095f3b85ee286542b3836fc81a5dd0a0349b4c239dd38744d488cf8e31db8bcb7d33b41abb9e5a33cca9144b1cef332c94bf0573bf047a3aca98cdf3b", 16);

		public override string Name
		{
			get { return "IES"; }
		}

		public override void PerformTest()
		{
			IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("ECIES");

            X9ECParameters x9 = ECNamedCurveTable.GetByName("prime239v1");
            ECDomainParameters ecSpec = new ECDomainParameters(x9.Curve, x9.G, x9.N, x9.H);

			g.Init(
				new ECKeyGenerationParameters(
					ecSpec,
					new SecureRandom()));

			IBufferedCipher c1 = CipherUtilities.GetCipher("ECIES");
			IBufferedCipher c2 = CipherUtilities.GetCipher("ECIES");

			doTest(g, c1, c2);

			g = GeneratorUtilities.GetKeyPairGenerator("ECIES");

			g.Init(new KeyGenerationParameters(new SecureRandom(), 192));

			doTest(g, c1, c2);

			g = GeneratorUtilities.GetKeyPairGenerator("ECIES");

			g.Init(new KeyGenerationParameters(new SecureRandom(), 239));

			doTest(g, c1, c2);

			g = GeneratorUtilities.GetKeyPairGenerator("ECIES");

			g.Init(new KeyGenerationParameters(new SecureRandom(), 256));

			doTest(g, c1, c2);

			doDefTest(g, c1, c2);

			c1 = CipherUtilities.GetCipher("IES");
			c2 = CipherUtilities.GetCipher("IES");

			g = GeneratorUtilities.GetKeyPairGenerator("DH");

//			DHParameterSpec dhParams = new DHParameterSpec(p512, g512);
//			g.initialize(dhParams);
			g.Init(
				new DHKeyGenerationParameters(
					new SecureRandom(),
					new DHParameters(p512, g512)));

			doTest(g, c1, c2);

			doDefTest(g, c1, c2);
		}

		public void doTest(
			IAsymmetricCipherKeyPairGenerator	g,
			IBufferedCipher						c1,
			IBufferedCipher						c2)
		{
			//
			// a side
			//
			AsymmetricCipherKeyPair aKeyPair = g.GenerateKeyPair();
			AsymmetricKeyParameter aPub = aKeyPair.Public;
			AsymmetricKeyParameter  aPriv = aKeyPair.Private;

			//
			// b side
			//
			AsymmetricCipherKeyPair bKeyPair = g.GenerateKeyPair();
			AsymmetricKeyParameter bPub = bKeyPair.Public;
			AsymmetricKeyParameter bPriv = bKeyPair.Private;

			// TODO Put back in
//			//
//			// stream test
//			//
//			IEKeySpec c1Key = new IEKeySpec(aPriv, bPub);
//			IEKeySpec c2Key = new IEKeySpec(bPriv, aPub);
//
//			byte[] d = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
//			byte[] e = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
//
//			IESParameterSpec param = new IESParameterSpec(d, e, 128);
//
//			c1.Init(true, c1Key, param);
//
//			c2.Init(false, c2Key, param);
//
//			byte[] message = Hex.Decode("1234567890abcdef");
//
//			byte[] out1 = c1.DoFinal(message, 0, message.Length);
//
//			byte[] out2 = c2.DoFinal(out1, 0, out1.Length);
//
//			if (!AreEqual(out2, message))
//			{
//				Fail("stream cipher test failed");
//			}
		}

		public void doDefTest(
			IAsymmetricCipherKeyPairGenerator	g,
			IBufferedCipher						c1,
			IBufferedCipher						c2)
		{
			//
			// a side
			//
			AsymmetricCipherKeyPair aKeyPair = g.GenerateKeyPair();
			AsymmetricKeyParameter aPub = aKeyPair.Public;
			AsymmetricKeyParameter aPriv = aKeyPair.Private;

			//
			// b side
			//
			AsymmetricCipherKeyPair bKeyPair = g.GenerateKeyPair();
			AsymmetricKeyParameter bPub = bKeyPair.Public;
			AsymmetricKeyParameter bPriv = bKeyPair.Private;

			// TODO Put back in
//			//
//			// stream test
//			//
//			IEKeySpec c1Key = new IEKeySpec(aPriv, bPub);
//			IEKeySpec c2Key = new IEKeySpec(bPriv, aPub);
//
//			c1.Init(true, c1Key);
//
//			AlgorithmParameters param = c1.getParameters();
//
//			c2.Init(false, c2Key, param);
//
//			byte[] message = Hex.Decode("1234567890abcdef");
//
//			byte[] out1 = c1.DoFinal(message, 0, message.Length);
//
//			byte[] out2 = c2.DoFinal(out1, 0, out1.Length);
//
//			if (!AreEqual(out2, message))
//			{
//				Fail("stream cipher test failed");
//			}
//
//			//
//			// int DoFinal
//			//
//			int len1 = c1.DoFinal(message, 0, message.Length, out1, 0);
//
//			if (len1 != out1.Length)
//			{
//				Fail("encryption length wrong");
//			}
//
//			int len2 = c2.DoFinal(out1, 0, out1.Length, out2, 0);
//
//			if (len2 != out2.Length)
//			{
//				Fail("decryption length wrong");
//			}
//        
//			if (!AreEqual(out2, message))
//			{
//				Fail("stream cipher test failed");
//			}
//        
//			//
//			// int DoFinal with update
//			//
//			len1 = c1.ProcessBytes(message, 0, 2, out1, 0);
//
//			len1 += c1.DoFinal(message, 2, message.Length - 2, out1, len1);
//
//			if (len1 != out1.Length)
//			{
//				Fail("update encryption length wrong");
//			}
//
//			len2 = c2.ProcessBytes(out1, 0, 2, out2, 0);
//
//			len2 += c2.DoFinal(out1, 2, out1.Length - 2, out2, len2);
//
//			if (len2 != out2.Length)
//			{
//				Fail("update decryption length wrong");
//			}
//
//			if (!AreEqual(out2, message))
//			{
//				Fail("update stream cipher test failed");
//			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new IesTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
