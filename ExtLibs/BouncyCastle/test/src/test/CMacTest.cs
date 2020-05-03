using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	/**
	 * CMAC tester - <a href="http://www.nuee.nagoya-u.ac.jp/labs/tiwata/omac/tv/omac1-tv.txt">AES Official Test Vectors</a>.
	 */
	[TestFixture]
	public class CMacTest
		:	SimpleTest
	{
		private static readonly byte[] keyBytes128 = Hex.Decode("2b7e151628aed2a6abf7158809cf4f3c");
		private static readonly byte[] keyBytes192 = Hex.Decode(
			  "8e73b0f7da0e6452c810f32b809079e5"
			+ "62f8ead2522c6b7b");
		private static readonly byte[] keyBytes256 = Hex.Decode(
			  "603deb1015ca71be2b73aef0857d7781"
			+ "1f352c073b6108d72d9810a30914dff4");
		
		private static readonly byte[] input0 = Hex.Decode("");
		private static readonly byte[] input16 = Hex.Decode("6bc1bee22e409f96e93d7e117393172a");
		private static readonly byte[] input40 = Hex.Decode(
			  "6bc1bee22e409f96e93d7e117393172a"
			+ "ae2d8a571e03ac9c9eb76fac45af8e5130c81c46a35ce411");
		private static readonly byte[] input64 = Hex.Decode(
			  "6bc1bee22e409f96e93d7e117393172a"
			+ "ae2d8a571e03ac9c9eb76fac45af8e51"
			+ "30c81c46a35ce411e5fbc1191a0a52ef"
			+ "f69f2445df4f9b17ad2b417be66c3710");

		private static readonly byte[] output_k128_m0 = Hex.Decode("bb1d6929e95937287fa37d129b756746");
		private static readonly byte[] output_k128_m16 = Hex.Decode("070a16b46b4d4144f79bdd9dd04a287c");
		private static readonly byte[] output_k128_m40 = Hex.Decode("dfa66747de9ae63030ca32611497c827");
		private static readonly byte[] output_k128_m64 = Hex.Decode("51f0bebf7e3b9d92fc49741779363cfe");

		private static readonly byte[] output_k192_m0 = Hex.Decode("d17ddf46adaacde531cac483de7a9367");
		private static readonly byte[] output_k192_m16 = Hex.Decode("9e99a7bf31e710900662f65e617c5184");
		private static readonly byte[] output_k192_m40 = Hex.Decode("8a1de5be2eb31aad089a82e6ee908b0e");
		private static readonly byte[] output_k192_m64 = Hex.Decode("a1d5df0eed790f794d77589659f39a11");

		private static readonly byte[] output_k256_m0 = Hex.Decode("028962f61b7bf89efc6b551f4667d983");
		private static readonly byte[] output_k256_m16 = Hex.Decode("28a7023f452e8f82bd4bf28d8c37c35c");
		private static readonly byte[] output_k256_m40 = Hex.Decode("aaf3d8f1de5640c232f5b169b9c911e6");
		private static readonly byte[] output_k256_m64 = Hex.Decode("e1992190549f6ed5696a2c056c315410");

		private static readonly byte[] output_des_ede = Hex.Decode("1ca670dea381d37c");

		public CMacTest()
		{
		}
		
		public override void PerformTest()
		{
//			Mac mac = Mac.getInstance("AESCMAC", "BC");
			IMac mac = MacUtilities.GetMac("AESCMAC");

			//128 bytes key

//			SecretKeySpec key = new SecretKeySpec(keyBytes128, "AES");
			KeyParameter key = new KeyParameter(keyBytes128);

			// 0 bytes message - 128 bytes key
			mac.Init(key);

			mac.BlockUpdate(input0, 0, input0.Length);

			byte[] output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k128_m0))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k128_m0)
					+ " got " + Hex.ToHexString(output));
			}

			// 16 bytes message - 128 bytes key
			mac.Init(key);

			mac.BlockUpdate(input16, 0, input16.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k128_m16))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k128_m16)
					+ " got " + Hex.ToHexString(output));
			}

			// 40 bytes message - 128 bytes key
			mac.Init(key);

			mac.BlockUpdate(input40, 0, input40.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k128_m40))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k128_m40)
					+ " got " + Hex.ToHexString(output));
			}

			// 64 bytes message - 128 bytes key
			mac.Init(key);

			mac.BlockUpdate(input64, 0, input64.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k128_m64))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k128_m64)
					+ " got " + Hex.ToHexString(output));
			}

			//192 bytes key

//			key = new SecretKeySpec(keyBytes192, "AES");
			key = new KeyParameter(keyBytes192);

			// 0 bytes message - 192 bytes key
			mac.Init(key);

			mac.BlockUpdate(input0, 0, input0.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k192_m0))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k192_m0)
					+ " got " + Hex.ToHexString(output));
			}

			// 16 bytes message - 192 bytes key
			mac.Init(key);

			mac.BlockUpdate(input16, 0, input16.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k192_m16))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k192_m16)
					+ " got " + Hex.ToHexString(output));
			}

			// 40 bytes message - 192 bytes key
			mac.Init(key);

			mac.BlockUpdate(input40, 0, input40.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k192_m40))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k192_m40)
					+ " got " + Hex.ToHexString(output));
			}

			// 64 bytes message - 192 bytes key
			mac.Init(key);

			mac.BlockUpdate(input64, 0, input64.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k192_m64))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k192_m64)
					+ " got " + Hex.ToHexString(output));
			}

			//256 bytes key

//			key = new SecretKeySpec(keyBytes256, "AES");
			key = new KeyParameter(keyBytes256);

			// 0 bytes message - 256 bytes key
			mac.Init(key);

			mac.BlockUpdate(input0, 0, input0.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k256_m0))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k256_m0)
					+ " got " + Hex.ToHexString(output));
			}

			// 16 bytes message - 256 bytes key
			mac.Init(key);

			mac.BlockUpdate(input16, 0, input16.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k256_m16))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k256_m16)
					+ " got " + Hex.ToHexString(output));
			}

			// 40 bytes message - 256 bytes key
			mac.Init(key);

			mac.BlockUpdate(input40, 0, input40.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k256_m40))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k256_m40)
					+ " got " + Hex.ToHexString(output));
			}

			// 64 bytes message - 256 bytes key
			mac.Init(key);

			mac.BlockUpdate(input64, 0, input64.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_k256_m64))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_k256_m64)
					+ " got " + Hex.ToHexString(output));
			}

//			mac = Mac.getInstance("DESedeCMAC", "BC");
			mac = MacUtilities.GetMac("DESedeCMAC");

			//DESede
			
//			key = new SecretKeySpec(keyBytes128, "DESede");
			key = new KeyParameter(keyBytes128);

			// 0 bytes message - 128 bytes key
			mac.Init(key);

			mac.BlockUpdate(input0, 0, input0.Length);

			output = MacUtilities.DoFinal(mac);

			if (!AreEqual(output, output_des_ede))
			{
				Fail("Failed - expected " + Hex.ToHexString(output_des_ede)
					+ " got " + Hex.ToHexString(output));
			}
		}

		public override string Name
		{
			get { return "CMac"; }
		}

		public static void Main(string[] args)
		{
			RunTest(new CMacTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
