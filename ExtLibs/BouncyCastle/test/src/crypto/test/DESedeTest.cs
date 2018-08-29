using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
/**
 * DESede tester
 */
	[TestFixture]
	public class DesEdeTest
		:CipherTest
	{
		private static readonly byte[] weakKey =     // first 8 bytes non-weak
		{
			(byte)0x06,(byte)0x01,(byte)0x01,(byte)0x01, (byte)0x01,(byte)0x01,(byte)0x01,(byte)0x01,
			(byte)0x1f,(byte)0x1f,(byte)0x1f,(byte)0x1f, (byte)0x0e,(byte)0x0e,(byte)0x0e,(byte)0x0e,
			(byte)0xe0,(byte)0xe0,(byte)0xe0,(byte)0xe0, (byte)0xf1,(byte)0xf1,(byte)0xf1,(byte)0xf1,
		};

		private const string input1 = "4e6f77206973207468652074696d6520666f7220616c6c20";
//		private const string input2 = "4e6f7720697320746865";

		private static SimpleTest[] tests =
		{
			new BlockCipherVectorTest(0, new DesEdeEngine(),
				new DesEdeParameters(Hex.Decode("0123456789abcdef0123456789abcdef")),
				input1, "3fa40e8a984d48156a271787ab8883f9893d51ec4b563b53"),
			new BlockCipherVectorTest(1, new DesEdeEngine(),
				new DesEdeParameters(Hex.Decode("0123456789abcdeffedcba9876543210")),
				input1, "d80a0d8b2bae5e4e6a0094171abcfc2775d2235a706e232c"),
			new BlockCipherVectorTest(2, new DesEdeEngine(),
				new DesEdeParameters(Hex.Decode("0123456789abcdef0123456789abcdef0123456789abcdef")),
				input1, "3fa40e8a984d48156a271787ab8883f9893d51ec4b563b53"),
			new BlockCipherVectorTest(3, new DesEdeEngine(),
				new DesEdeParameters(Hex.Decode("0123456789abcdeffedcba98765432100123456789abcdef")),
				input1, "d80a0d8b2bae5e4e6a0094171abcfc2775d2235a706e232c")
		};

		public DesEdeTest()
			: base(tests, new DesEdeEngine(), new DesEdeParameters(new byte[16]))
		{
		}

		private void WrapTest(
			int     id,
			byte[]  kek,
			byte[]  iv,
			byte[]  input,
			byte[]  output)
		{
			IWrapper wrapper = new DesEdeWrapEngine();

			wrapper.Init(true, new ParametersWithIV(new DesEdeParameters(kek), iv));

			try
			{
				byte[] cText = wrapper.Wrap(input, 0, input.Length);
				if (!AreEqual(cText, output))
				{
					Fail(": failed wrap test " + id  + " expected " + Hex.ToHexString(output)
						+ " got " + Hex.ToHexString(cText));
				}
			}
			catch (Exception e)
			{
				Fail("failed wrap test exception: " + e.ToString(), e);
			}

			wrapper.Init(false, new DesEdeParameters(kek));

			try
			{
				byte[] pText = wrapper.Unwrap(output, 0, output.Length);
				if (!AreEqual(pText, input))
				{
					Fail("failed unwrap test " + id  + " expected " + Hex.ToHexString(input)
						+ " got " + Hex.ToHexString(pText));
				}
			}
			catch (Exception e)
			{
				Fail("failed unwrap test exception: " + e.ToString(), e);
			}
		}

		public override void PerformTest()
		{
			base.PerformTest();

			byte[] kek1 = Hex.Decode("255e0d1c07b646dfb3134cc843ba8aa71f025b7c0838251f");
			byte[] iv1 = Hex.Decode("5dd4cbfc96f5453b");
			byte[] in1 = Hex.Decode("2923bf85e06dd6ae529149f1f1bae9eab3a7da3d860d3e98");
			byte[] out1 = Hex.Decode("690107618ef092b3b48ca1796b234ae9fa33ebb4159604037db5d6a84eb3aac2768c632775a467d4");

			WrapTest(1, kek1, iv1, in1, out1);

			//
			// key generation
			//
			SecureRandom random = new SecureRandom();
			DesEdeKeyGenerator keyGen = new DesEdeKeyGenerator();

			keyGen.Init(new KeyGenerationParameters(random, 112));

			byte[] kB = keyGen.GenerateKey();
	        
			if (kB.Length != 16)
			{
				Fail("112 bit key wrong length.");
			}
	        
			keyGen.Init(new KeyGenerationParameters(random, 168));
	        
			kB = keyGen.GenerateKey();
	        
			if (kB.Length != 24)
			{
				Fail("168 bit key wrong length.");
			}
	        
			try
			{
				keyGen.Init(new KeyGenerationParameters(random, 200));
	            
				Fail("invalid key length not detected.");
			}
			catch (ArgumentException)
			{
				// expected
			}

			try
			{
				DesEdeParameters.IsWeakKey(new byte[4], 0);
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
				new DesEdeParameters(weakKey);
				Fail("no exception on weak key");
			}
			catch (ArgumentException e)
			{
				if (!e.Message.Equals("attempt to create weak DESede key"))
				{
					Fail("wrong exception");
				}
			}
		}

		public override string Name
		{
			get { return "DESede"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new DesEdeTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
