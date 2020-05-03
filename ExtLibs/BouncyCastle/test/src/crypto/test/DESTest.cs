using System;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

using NUnit.Framework;

namespace Org.BouncyCastle.Crypto.Tests
{
	internal class DesParityTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "DESParityTest"; }
		}

		public override void PerformTest()
		{
			byte[] k1In = { (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff,
							   (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff };
			byte[] k1Out = { (byte)0xfe, (byte)0xfe, (byte)0xfe, (byte)0xfe,
								(byte)0xfe, (byte)0xfe, (byte)0xfe, (byte)0xfe };

			byte[] k2In = { (byte)0xef, (byte)0xcb, (byte)0xda, (byte)0x4f,
							   (byte)0xaa, (byte)0x99, (byte)0x7f, (byte)0x63 };
			byte[] k2Out = { (byte)0xef, (byte)0xcb, (byte)0xda, (byte)0x4f,
								(byte)0xab, (byte)0x98, (byte)0x7f, (byte)0x62 };

			DesParameters.SetOddParity(k1In);

			for (int i = 0; i != k1In.Length; i++)
			{
				if (k1In[i] != k1Out[i])
				{
					Fail("Failed "
						+ "got " + Hex.ToHexString(k1In)
						+ " expected " + Hex.ToHexString(k1Out));
				}
			}

			DesParameters.SetOddParity(k2In);

			for (int i = 0; i != k2In.Length; i++)
			{
				if (k2In[i] != k2Out[i])
				{
					Fail("Failed "
						+ "got " + Hex.ToHexString(k2In)
						+ " expected " + Hex.ToHexString(k2Out));
				}
			}
		}
	}

	internal class KeyGenTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "KeyGenTest"; }
		}

		public override void PerformTest()
		{
			DesKeyGenerator keyGen = new DesKeyGenerator();

			keyGen.Init(new KeyGenerationParameters(new SecureRandom(), 56));

			byte[] kB = keyGen.GenerateKey();

			if (kB.Length != 8)
			{
				Fail("DES bit key wrong length.");
			}
		}
	}

	internal class DesParametersTest
		: SimpleTest
	{
		private static readonly byte[] weakKeys =
		{
			(byte)0x01,(byte)0x01,(byte)0x01,(byte)0x01, (byte)0x01,(byte)0x01,(byte)0x01,(byte)0x01,
			(byte)0x1f,(byte)0x1f,(byte)0x1f,(byte)0x1f, (byte)0x0e,(byte)0x0e,(byte)0x0e,(byte)0x0e,
			(byte)0xe0,(byte)0xe0,(byte)0xe0,(byte)0xe0, (byte)0xf1,(byte)0xf1,(byte)0xf1,(byte)0xf1,
			(byte)0xfe,(byte)0xfe,(byte)0xfe,(byte)0xfe, (byte)0xfe,(byte)0xfe,(byte)0xfe,(byte)0xfe,
			/* semi-weak keys */
			(byte)0x01,(byte)0xfe,(byte)0x01,(byte)0xfe, (byte)0x01,(byte)0xfe,(byte)0x01,(byte)0xfe,
			(byte)0x1f,(byte)0xe0,(byte)0x1f,(byte)0xe0, (byte)0x0e,(byte)0xf1,(byte)0x0e,(byte)0xf1,
			(byte)0x01,(byte)0xe0,(byte)0x01,(byte)0xe0, (byte)0x01,(byte)0xf1,(byte)0x01,(byte)0xf1,
			(byte)0x1f,(byte)0xfe,(byte)0x1f,(byte)0xfe, (byte)0x0e,(byte)0xfe,(byte)0x0e,(byte)0xfe,
			(byte)0x01,(byte)0x1f,(byte)0x01,(byte)0x1f, (byte)0x01,(byte)0x0e,(byte)0x01,(byte)0x0e,
			(byte)0xe0,(byte)0xfe,(byte)0xe0,(byte)0xfe, (byte)0xf1,(byte)0xfe,(byte)0xf1,(byte)0xfe,
			(byte)0xfe,(byte)0x01,(byte)0xfe,(byte)0x01, (byte)0xfe,(byte)0x01,(byte)0xfe,(byte)0x01,
			(byte)0xe0,(byte)0x1f,(byte)0xe0,(byte)0x1f, (byte)0xf1,(byte)0x0e,(byte)0xf1,(byte)0x0e,
			(byte)0xe0,(byte)0x01,(byte)0xe0,(byte)0x01, (byte)0xf1,(byte)0x01,(byte)0xf1,(byte)0x01,
			(byte)0xfe,(byte)0x1f,(byte)0xfe,(byte)0x1f, (byte)0xfe,(byte)0x0e,(byte)0xfe,(byte)0x0e,
			(byte)0x1f,(byte)0x01,(byte)0x1f,(byte)0x01, (byte)0x0e,(byte)0x01,(byte)0x0e,(byte)0x01,
			(byte)0xfe,(byte)0xe0,(byte)0xfe,(byte)0xe0, (byte)0xfe,(byte)0xf1,(byte)0xfe,(byte)0xf1
		};

		public override string Name
		{
			get { return "DesParameters"; }
		}

		public override void PerformTest()
		{
			try
			{
				DesParameters.IsWeakKey(new byte[4], 0);
				Fail("no exception on small key");
			}
			catch (ArgumentException e)
			{
				if (!e.Message.Equals("key material too short."))
				{
					Fail("wrong exception");
				}
			}

			try
			{
				new DesParameters(weakKeys);
				Fail("no exception on weak key");
			}
			catch (ArgumentException e)
			{
				if (!e.Message.Equals("attempt to create weak DES key"))
				{
					Fail("wrong exception");
				}
			}

			for (int i = 0; i != weakKeys.Length; i += 8)
			{
				if (!DesParameters.IsWeakKey(weakKeys, i))
				{
					Fail("weakKey test failed");
				}
			}
		}
	}

	/**
	 * DES tester - vectors from <a href="http://www.itl.nist.gov/fipspubs/fip81.htm">FIPS 81</a>
	 */
	[TestFixture]
	public class DesTest
		: CipherTest
	{
		static string input1 = "4e6f77206973207468652074696d6520666f7220616c6c20";
		static string input2 = "4e6f7720697320746865";
		static string input3 = "4e6f7720697320746865aabbcc";

		static SimpleTest[] tests =
		{
			new BlockCipherVectorTest(0, new DesEngine(),
				new DesParameters(Hex.Decode("0123456789abcdef")),
				input1, "3fa40e8a984d48156a271787ab8883f9893d51ec4b563b53"),
			new BlockCipherVectorTest(1, new CbcBlockCipher(new DesEngine()),
				new ParametersWithIV(new DesParameters(Hex.Decode("0123456789abcdef")), Hex.Decode("1234567890abcdef")),
				input1, "e5c7cdde872bf27c43e934008c389c0f683788499a7c05f6"),
			new BlockCipherVectorTest(2, new CfbBlockCipher(new DesEngine(), 8),
				new ParametersWithIV(new DesParameters(Hex.Decode("0123456789abcdef")), Hex.Decode("1234567890abcdef")),
				input2, "f31fda07011462ee187f"),
			new BlockCipherVectorTest(3, new CfbBlockCipher(new DesEngine(), 64),
				new ParametersWithIV(new DesParameters(Hex.Decode("0123456789abcdef")), Hex.Decode("1234567890abcdef")),
				input1, "f3096249c7f46e51a69e839b1a92f78403467133898ea622"),
			new BlockCipherVectorTest(4, new OfbBlockCipher(new DesEngine(), 8),
				new ParametersWithIV(new DesParameters(Hex.Decode("0123456789abcdef")), Hex.Decode("1234567890abcdef")),
				input2, "f34a2850c9c64985d684"),
			new BlockCipherVectorTest(5, new CfbBlockCipher(new DesEngine(), 64),
				new ParametersWithIV(new DesParameters(Hex.Decode("0123456789abcdef")), Hex.Decode("1234567890abcdef")),
				input3, "f3096249c7f46e51a69e0954bf"),
			new BlockCipherVectorTest(6, new OfbBlockCipher(new DesEngine(), 64),
				new ParametersWithIV(new DesParameters(Hex.Decode("0123456789abcdef")), Hex.Decode("1234567890abcdef")),
				input3, "f3096249c7f46e5135f2c0eb8b"),
			new DesParityTest(),
			new DesParametersTest(),
			new KeyGenTest()
		};

		public DesTest()
			: base(tests, new DesEngine(), new DesParameters(new byte[8]))
		{
		}

		public override string Name
		{
			get { return "DES"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new DesTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
