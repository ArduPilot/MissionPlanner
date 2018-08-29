using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class Gost28147Test
		: CipherTest
	{
		static string input1 =  "0000000000000000";
		static string output1 = "1b0bbc32cebcab42";
		static string input2 =  "bc350e71aac5f5c2";
		static string output2 = "d35ab653493b49f5";
		static string input3 =  "bc350e71aa11345709acde";
		static string output3 = "8824c124c4fd14301fb1e8";
		static string input4 =  "000102030405060708090a0b0c0d0e0fff0102030405060708090a0b0c0d0e0f";
		static string output4 = "29b7083e0a6d955ca0ec5b04fdb4ea41949f1dd2efdf17baffc1780b031f3934";

		static byte[] TestSBox = {
				0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
				0xF,0xE,0xD,0xC,0xB,0xA,0x9,0x8,0x7,0x6,0x5,0x4,0x3,0x2,0x1,0x0,
				0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
				0xF,0xE,0xD,0xC,0xB,0xA,0x9,0x8,0x7,0x6,0x5,0x4,0x3,0x2,0x1,0x0,
				0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
				0xF,0xE,0xD,0xC,0xB,0xA,0x9,0x8,0x7,0x6,0x5,0x4,0x3,0x2,0x1,0x0,
				0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
				0xF,0xE,0xD,0xC,0xB,0xA,0x9,0x8,0x7,0x6,0x5,0x4,0x3,0x2,0x1,0x0
		};

        static byte[] TestSBox_1 =
        {
            0xE, 0x3, 0xC, 0xD, 0x1, 0xF, 0xA, 0x9, 0xB, 0x6, 0x2, 0x7, 0x5, 0x0, 0x8, 0x4,
            0xD, 0x9, 0x0, 0x4, 0x7, 0x1, 0x3, 0xB, 0x6, 0xC, 0x2, 0xA, 0xF, 0xE, 0x5, 0x8,
            0x8, 0xB, 0xA, 0x7, 0x1, 0xD, 0x5, 0xC, 0x6, 0x3, 0x9, 0x0, 0xF, 0xE, 0x2, 0x4,
            0xD, 0x7, 0xC, 0x9, 0xF, 0x0, 0x5, 0x8, 0xA, 0x2, 0xB, 0x6, 0x4, 0x3, 0x1, 0xE,
            0xB, 0x4, 0x6, 0x5, 0x0, 0xF, 0x1, 0xC, 0x9, 0xE, 0xD, 0x8, 0x3, 0x7, 0xA, 0x2,
            0xD, 0xF, 0x9, 0x4, 0x2, 0xC, 0x5, 0xA, 0x6, 0x0, 0x3, 0x8, 0x7, 0xE, 0x1, 0xB,
            0xF, 0xE, 0x9, 0x5, 0xB, 0x2, 0x1, 0x8, 0x6, 0x0, 0xD, 0x3, 0x4, 0x7, 0xC, 0xA,
            0xA, 0x3, 0xE, 0x2, 0x0, 0x1, 0x4, 0x6, 0xB, 0x8, 0xC, 0x7, 0xD, 0x5, 0xF, 0x9
        };

        static SimpleTest[] tests =
		{   new BlockCipherVectorTest(1, new Gost28147Engine(),
				new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")),
					input1, output1),
			new BlockCipherVectorTest(2, new CbcBlockCipher(new Gost28147Engine()),
				new ParametersWithIV(new KeyParameter(Hex.Decode("00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF")),
				Hex.Decode("1234567890abcdef")), input2, output2),
			new BlockCipherVectorTest(3, new GOfbBlockCipher(new Gost28147Engine()),
				new ParametersWithIV(new KeyParameter(Hex.Decode("0011223344556677889900112233445566778899001122334455667788990011")),
				Hex.Decode("1234567890abcdef")), //IV
				input3, output3),
			new BlockCipherVectorTest(4, new CfbBlockCipher(new Gost28147Engine(), 64),
				new ParametersWithIV(new KeyParameter(Hex.Decode("aafd12f659cae63489b479e5076ddec2f06cb58faafd12f659cae63489b479e5")),
				Hex.Decode("aafd12f659cae634")), input4, output4),

			//tests with parameters, set S-box.
			new BlockCipherVectorTest(5, new Gost28147Engine(),
				new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")),//key , default parameter S-box set to D-Test
				input1, output1),
			new BlockCipherVectorTest(6, new CfbBlockCipher(new Gost28147Engine(), 64),
				new ParametersWithIV(
					new ParametersWithSBox(
						new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")), //key
						Gost28147Engine.GetSBox("D-Test")), //type S-box
					Hex.Decode("1234567890abcdef")), //IV
					"0000000000000000", //input message
					"b587f7a0814c911d"), //encrypt message
			new BlockCipherVectorTest(7, new CfbBlockCipher(new Gost28147Engine(), 64),
				new ParametersWithIV(
					new ParametersWithSBox(
						new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")), //key
						Gost28147Engine.GetSBox("E-Test")), //type S-box
					Hex.Decode("1234567890abcdef")), //IV
					"0000000000000000", //input message
					"e8287f53f991d52b"), //encrypt message
			new BlockCipherVectorTest(8, new CfbBlockCipher(new Gost28147Engine(), 64),
				new ParametersWithIV(
					new ParametersWithSBox(
						new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")), //key
						Gost28147Engine.GetSBox("E-A")), //type S-box
					Hex.Decode("1234567890abcdef")), //IV
					"0000000000000000", //input message
					"c41009dba22ebe35"), //encrypt message
			new BlockCipherVectorTest(9, new CfbBlockCipher(new Gost28147Engine(), 8),
				new ParametersWithIV(
					new ParametersWithSBox(
						new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")), //key
						Gost28147Engine.GetSBox("E-B")), //type S-box
					Hex.Decode("1234567890abcdef")), //IV
					"0000000000000000", //input message
					"80d8723fcd3aba28"), //encrypt message
			new BlockCipherVectorTest(10, new CfbBlockCipher(new Gost28147Engine(), 8),
				new ParametersWithIV(
					new ParametersWithSBox(
						new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")), //key
						Gost28147Engine.GetSBox("E-C")), //type S-box
					Hex.Decode("1234567890abcdef")), //IV
					"0000000000000000", //input message
					"739f6f95068499b5"), //encrypt message
			new BlockCipherVectorTest(11, new CfbBlockCipher(new Gost28147Engine(), 8),
				new ParametersWithIV(
					new ParametersWithSBox(
						new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")), //key
						Gost28147Engine.GetSBox("E-D")), //type S-box
					Hex.Decode("1234567890abcdef")), //IV
					"0000000000000000", //input message
					"4663f720f4340f57"), //encrypt message
			new BlockCipherVectorTest(12, new CfbBlockCipher(new Gost28147Engine(), 8),
				new ParametersWithIV(
					new ParametersWithSBox(
						new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")), //key
						Gost28147Engine.GetSBox("D-A")), //type S-box
					Hex.Decode("1234567890abcdef")), //IV
					"0000000000000000", //input message
					"5bb0a31d218ed564"), //encrypt message
			new BlockCipherVectorTest(13, new CfbBlockCipher(new Gost28147Engine(), 8),
				new ParametersWithIV(
					new ParametersWithSBox(
						new KeyParameter(Hex.Decode("546d203368656c326973652073736e62206167796967747473656865202c3d73")), //key
						TestSBox), //set own S-box
					Hex.Decode("1234567890abcdef")), //IV
					"0000000000000000", //input message
					"c3af96ef788667c5"), //encrypt message
			new BlockCipherVectorTest(14, new GOfbBlockCipher(new Gost28147Engine()),
				new ParametersWithIV(
					new ParametersWithSBox(
							new KeyParameter(Hex.Decode("4ef72b778f0b0bebeef4f077551cb74a927b470ad7d7f2513454569a247e989d")), //key
							Gost28147Engine.GetSBox("E-A")), //type S-box
					Hex.Decode("1234567890abcdef")), //IV
					"bc350e71aa11345709acde",  //input message
					"1bcc2282707c676fb656dc"), //encrypt message
            new BlockCipherVectorTest(15, new GOfbBlockCipher(new Gost28147Engine()),
                new ParametersWithIV(
                    new ParametersWithSBox(
                        new KeyParameter(Hex.Decode("0A43145BA8B9E9FF0AEA67D3F26AD87854CED8D9017B3D33ED81301F90FDF993")), //key
                        TestSBox_1), //type, IV, S-box
                    Hex.Decode("8001069080010690")),
                "094C912C5EFDD703D42118971694580B", //input message
                "2707B58DF039D1A64460735FFE76D55F"), //encrypt message
            new BlockCipherVectorTest(16, new GOfbBlockCipher(new Gost28147Engine()),
                new ParametersWithIV(
                    new ParametersWithSBox(
                        new KeyParameter(Hex.Decode("0A43145BA8B9E9FF0AEA67D3F26AD87854CED8D9017B3D33ED81301F90FDF993")), //key
                        TestSBox_1), //type, S-box
                        Hex.Decode("800107A0800107A0")),
                "FE780800E0690083F20C010CF00C0329", //input message
                "9AF623DFF948B413B53171E8D546188D"), //encrypt message
            new BlockCipherVectorTest(17, new GOfbBlockCipher(new Gost28147Engine()),
                new ParametersWithIV(
                    new ParametersWithSBox(
                        new KeyParameter(Hex.Decode("0A43145BA8B9E9FF0AEA67D3F26AD87854CED8D9017B3D33ED81301F90FDF993")), //key
                        TestSBox_1), //type, S-box
                        Hex.Decode("8001114080011140")),
                "D1088FD8C0A86EE8F1DCD1088FE8C058", //input message
                "62A6B64D12253BCD8241A4BB0CFD3E7C"), //encrypt message
            new BlockCipherVectorTest(18, new GOfbBlockCipher(new Gost28147Engine()),
                new ParametersWithIV(
                    new ParametersWithSBox(
                        new KeyParameter(Hex.Decode("0A43145BA8B9E9FF0AEA67D3F26AD87854CED8D9017B3D33ED81301F90FDF993")), //key
                        TestSBox_1), //type, IV, S-box
                        Hex.Decode("80011A3080011A30")),
                "D431FACD011C502C501B500A12921090", //input message
                "07313C89D302FF73234B4A0506AB00F3"), //encrypt message
		};

		private const int Gost28147_KEY_LENGTH = 32;

		private byte[] generateKey(byte[] startkey)
		{
			byte[] newKey = new byte[Gost28147_KEY_LENGTH];

			Gost3411Digest digest = new Gost3411Digest();

			digest.BlockUpdate(startkey, 0, startkey.Length);
			digest.DoFinal(newKey, 0);

			return newKey;
		}

		public Gost28147Test()
			: base(tests, new Gost28147Engine(), new KeyParameter(new byte[32]))
		{
		}

		public override void PerformTest()
		{
			base.PerformTest();

			//advanced tests with Gost28147KeyGenerator:
			//encrypt on hesh message; ECB mode:
			byte[] inBytes = Hex.Decode("4e6f77206973207468652074696d6520666f7220616c6c20");
			byte[] output = Hex.Decode("8ad3c8f56b27ff1fbd46409359bdc796bc350e71aac5f5c0");
			byte[] outBytes = new byte[inBytes.Length];

			byte[] key = generateKey(Hex.Decode("0123456789abcdef"));  //!!! heshing start_key - get 256 bits !!!
	//        System.out.println(new string(Hex.Encode(key)));
			ICipherParameters  param = new ParametersWithSBox(new KeyParameter(key), Gost28147Engine.GetSBox("E-A"));
			//CipherParameters  param = new Gost28147Parameters(key,"D-Test");
			BufferedBlockCipher cipher = new BufferedBlockCipher(new Gost28147Engine());

			cipher.Init(true, param);
			int len1 = cipher.ProcessBytes(inBytes, 0, inBytes.Length, outBytes, 0);
			try
			{
				cipher.DoFinal(outBytes, len1);
			}
			catch (CryptoException e)
			{
				Fail("failed - exception " + e.ToString(), e);
			}

			if (outBytes.Length != output.Length)
			{
				Fail("failed - "
					+ "expected " + Hex.ToHexString(output) + " got "
					+ Hex.ToHexString(outBytes));
			}

			for (int i = 0; i != outBytes.Length; i++)
			{
				if (outBytes[i] != output[i])
				{
					Fail("failed - "
						+ "expected " + Hex.ToHexString(output)
						+ " got " + Hex.ToHexString(outBytes));
				}
			}


			//encrypt on hesh message; CFB mode:
			inBytes = Hex.Decode("bc350e71aac5f5c2");
			output = Hex.Decode("0ebbbafcf38f14a5");
			outBytes = new byte[inBytes.Length];

			key = generateKey(Hex.Decode("0123456789abcdef"));  //!!! heshing start_key - get 256 bits !!!
			param = new ParametersWithIV(
				new ParametersWithSBox(
					new KeyParameter(key), //key
					Gost28147Engine.GetSBox("E-A")), //type S-box
				Hex.Decode("1234567890abcdef")); //IV

			cipher = new BufferedBlockCipher(new CfbBlockCipher(new Gost28147Engine(), 64));

			cipher.Init(true, param);
			len1 = cipher.ProcessBytes(inBytes, 0, inBytes.Length, outBytes, 0);
			try
			{
				cipher.DoFinal(outBytes, len1);
			}
			catch (CryptoException e)
			{
				Fail("failed - exception " + e.ToString(), e);
			}
			if (outBytes.Length != output.Length)
			{
				Fail("failed - "
					+ "expected " + Hex.ToHexString(output)
					+ " got " + Hex.ToHexString(outBytes));
			}
			for (int i = 0; i != outBytes.Length; i++)
			{
				if (outBytes[i] != output[i])
				{
					Fail("failed - "
						+ "expected " + Hex.ToHexString(output)
						+ " got " + Hex.ToHexString(outBytes));
				}
			}


			//encrypt on hesh message; CFB mode:
			inBytes = Hex.Decode("000102030405060708090a0b0c0d0e0fff0102030405060708090a0b0c0d0e0f");
			output = Hex.Decode("64988982819f0a1655e226e19ecad79d10cc73bac95c5d7da034786c12294225");
			outBytes = new byte[inBytes.Length];

			key = generateKey(Hex.Decode("aafd12f659cae63489b479e5076ddec2f06cb58faafd12f659cae63489b479e5"));  //!!! heshing start_key - get 256 bits !!!
			param = new ParametersWithIV(
				new ParametersWithSBox(
					new KeyParameter(key), //key
					Gost28147Engine.GetSBox("E-A")), //type S-box
				Hex.Decode("aafd12f659cae634")); //IV

			cipher = new BufferedBlockCipher(new CfbBlockCipher(new Gost28147Engine(), 64));

			cipher.Init(true, param);
			len1 = cipher.ProcessBytes(inBytes, 0, inBytes.Length, outBytes, 0);

			cipher.DoFinal(outBytes, len1);

			if (outBytes.Length != output.Length)
			{
				Fail("failed - "
					+ "expected " + Hex.ToHexString(output)
					+ " got " + Hex.ToHexString(outBytes));
			}

			for (int i = 0; i != outBytes.Length; i++)
			{
				if (outBytes[i] != output[i])
				{
					Fail("failed - "
						+ "expected " + Hex.ToHexString(output)
						+ " got " + Hex.ToHexString(outBytes));
				}
			}

			//encrypt on hesh message; OFB mode:
			inBytes = Hex.Decode("bc350e71aa11345709acde");
			output = Hex.Decode("1bcc2282707c676fb656dc");
			outBytes = new byte[inBytes.Length];

			key = generateKey(Hex.Decode("0123456789abcdef"));  //!!! heshing start_key - get 256 bits !!!
			param = new ParametersWithIV(
				new ParametersWithSBox(
					new KeyParameter(key), //key
					Gost28147Engine.GetSBox("E-A")), //type S-box
				Hex.Decode("1234567890abcdef")); //IV

			cipher = new BufferedBlockCipher(new GOfbBlockCipher(new Gost28147Engine()));

			cipher.Init(true, param);
			len1 = cipher.ProcessBytes(inBytes, 0, inBytes.Length, outBytes, 0);

			cipher.DoFinal(outBytes, len1);

			if (outBytes.Length != output.Length)
			{
				Fail("failed - "
					+ "expected " + Hex.ToHexString(output)
					+ " got " + Hex.ToHexString(outBytes));
			}

			for (int i = 0; i != outBytes.Length; i++)
			{
				if (outBytes[i] != output[i])
				{
					Fail("failed - "
						+ "expected " + Hex.ToHexString(output)
						+ " got " + Hex.ToHexString(outBytes));
				}
			}
		}

		public override string Name
		{
			get { return "Gost28147"; }
		}

		public static void Main(
			string[] args)
		{
			ITest test = new Gost28147Test();
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
