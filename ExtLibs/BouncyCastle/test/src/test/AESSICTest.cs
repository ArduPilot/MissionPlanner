using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	/// <remarks>
	/// Test vectors based on NIST Special Publication 800-38A,
	/// "Recommendation for Block Cipher Modes of Operation"
	/// </remarks>
	[TestFixture]
	public class AesSicTest
		: SimpleTest
	{
		private static readonly byte[][] keys =
		{
			Hex.Decode("2b7e151628aed2a6abf7158809cf4f3c"),
			Hex.Decode("8e73b0f7da0e6452c810f32b809079e562f8ead2522c6b7b"),
			Hex.Decode("603deb1015ca71be2b73aef0857d77811f352c073b6108d72d9810a30914dff4")
		};

		private static readonly byte[][] plain =
		{
			Hex.Decode("6bc1bee22e409f96e93d7e117393172a"),
			Hex.Decode("ae2d8a571e03ac9c9eb76fac45af8e51"),
			Hex.Decode("30c81c46a35ce411e5fbc1191a0a52ef"),
			Hex.Decode("f69f2445df4f9b17ad2b417be66c3710")
		};

		private static readonly byte[,][] cipher =
		{
			{
				Hex.Decode("874d6191b620e3261bef6864990db6ce"),
				Hex.Decode("9806f66b7970fdff8617187bb9fffdff"),
				Hex.Decode("5ae4df3edbd5d35e5b4f09020db03eab"),
				Hex.Decode("1e031dda2fbe03d1792170a0f3009cee")
			},
			{
				Hex.Decode("1abc932417521ca24f2b0459fe7e6e0b"),
				Hex.Decode("090339ec0aa6faefd5ccc2c6f4ce8e94"),
				Hex.Decode("1e36b26bd1ebc670d1bd1d665620abf7"),
				Hex.Decode("4f78a7f6d29809585a97daec58c6b050")
			},
			{
				Hex.Decode("601ec313775789a5b7a7f504bbf3d228"),
				Hex.Decode("f443e3ca4d62b59aca84e990cacaf5c5"),
				Hex.Decode("2b0930daa23de94ce87017ba2d84988d"),
				Hex.Decode("dfc9c58db67aada613c2dd08457941a6")
			}
		};

		public override string Name
		{
			get { return "AESSIC"; }
		}

		public override void PerformTest()
		{
			IBufferedCipher c = CipherUtilities.GetCipher("AES/SIC/NoPadding");

			//
			// NIST vectors
			//
			for (int i = 0; i != keys.Length; i++)
			{
				KeyParameter skey = ParameterUtilities.CreateKeyParameter("AES", keys[i]);
				c.Init(true, new ParametersWithIV(skey, Hex.Decode("F0F1F2F3F4F5F6F7F8F9FAFBFCFDFEFF")));

				for (int j = 0; j != plain.Length; j++)
				{
					byte[] enc = c.ProcessBytes(plain[j]);
					if (!AreEqual(enc, cipher[i, j]))
					{
						Fail("AESSIC encrypt failed: key " + i + " block " + j);
					}
				}

				c.Init(false, new ParametersWithIV(skey, Hex.Decode("F0F1F2F3F4F5F6F7F8F9FAFBFCFDFEFF")));

				for (int j = 0; j != plain.Length; j++)
				{
					byte[] enc = c.ProcessBytes(cipher[i, j]);
					if (!AreEqual(enc, plain[j]))
					{
						Fail("AESSIC decrypt failed: key " + i + " block " + j);
					}
				}
			}

			//
			// check CTR also recognised.
			//
			c = CipherUtilities.GetCipher("AES/CTR/NoPadding");

			KeyParameter sk = ParameterUtilities.CreateKeyParameter("AES", Hex.Decode("2B7E151628AED2A6ABF7158809CF4F3C"));

			c.Init(true, new ParametersWithIV(sk, Hex.Decode("F0F1F2F3F4F5F6F7F8F9FAFBFCFD0001")));

			byte[] crypt = c.DoFinal(Hex.Decode("00000000000000000000000000000000"));

			if (!AreEqual(crypt, Hex.Decode("D23513162B02D0F72A43A2FE4A5F97AB")))
			{
				Fail("AESSIC failed test 2");
			}

			//
			// check partial block processing
			//
			c = CipherUtilities.GetCipher("AES/CTR/NoPadding");

			sk = ParameterUtilities.CreateKeyParameter("AES", Hex.Decode("2B7E151628AED2A6ABF7158809CF4F3C"));

			c.Init(true, new ParametersWithIV(sk, Hex.Decode("F0F1F2F3F4F5F6F7F8F9FAFBFCFD0001")));

			crypt = c.DoFinal(Hex.Decode("12345678"));

			c.Init(false, new ParametersWithIV(sk, Hex.Decode("F0F1F2F3F4F5F6F7F8F9FAFBFCFD0001")));

			crypt = c.DoFinal(crypt);

			if (!AreEqual(crypt, Hex.Decode("12345678")))
			{
				Fail("AESSIC failed partial test");
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new AesSicTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
