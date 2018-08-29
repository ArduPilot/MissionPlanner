using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	* Wrap Test based on RFC3211 test vectors
	*/
	[TestFixture]
	public class Rfc3211WrapTest
		: SimpleTest
	{
		// Note: These test data assume the Rfc3211WrapEngine will call SecureRandom.NextBytes

		SecureRandom r1 = FixedSecureRandom.From(
			new byte[]{ 0xC4, 0x36, 0xF5, 0x41 });

		SecureRandom r2 = FixedSecureRandom.From(
			new byte[]{ 0xFA, 0x06, 0x0A, 0x45 });

		public override string Name
		{
			get { return "RFC3211Wrap"; }
		}

		private void doWrapTest(
			int				id,
			IBlockCipher	engine,
			byte[]			kek,
			byte[]			iv,
			SecureRandom	rand,
			byte[]			inBytes,
			byte[]			outBytes)
		{
			IWrapper wrapper = new Rfc3211WrapEngine(engine);

			wrapper.Init(true, new ParametersWithRandom(
				new ParametersWithIV(new KeyParameter(kek), iv), rand));

			byte[] cText = wrapper.Wrap(inBytes, 0, inBytes.Length);
			if (!AreEqual(cText, outBytes))
			{
				Fail("failed Wrap test " + id  + " expected "
					+ Hex.ToHexString(outBytes) + " got " + Hex.ToHexString(cText));
			}

			wrapper.Init(false, new ParametersWithIV(new KeyParameter(kek), iv));

			byte[] pText = wrapper.Unwrap(outBytes, 0, outBytes.Length);
			if (!AreEqual(pText, inBytes))
			{
				Fail("rfailed Unwrap test " + id  + " expected "
					+ Hex.ToHexString(inBytes) + " got " + Hex.ToHexString(pText));
			}
		}

		private void doTestCorruption()
		{
			byte[] kek = Hex.Decode("D1DAA78615F287E6");
			byte[] iv = Hex.Decode("EFE598EF21B33D6D");

			IWrapper wrapper = new Rfc3211WrapEngine(new DesEngine());

			wrapper.Init(false, new ParametersWithIV(new KeyParameter(kek), iv));

			byte[] block = Hex.Decode("ff739D838C627C897323A2F8C436F541");
			encryptBlock(kek, iv, block);

			try
			{
				wrapper.Unwrap(block, 0, block.Length);

				Fail("bad length not detected");
			}
			catch (InvalidCipherTextException e)
			{
				if (!e.Message.Equals("wrapped key corrupted"))
				{
					Fail("wrong exception on length");
				}
			}

			block = Hex.Decode("08639D838C627C897323A2F8C436F541");
			doTestChecksum(kek, iv, block, wrapper);

			block = Hex.Decode("08736D838C627C897323A2F8C436F541");
			doTestChecksum(kek, iv, block, wrapper);
	        
			block = Hex.Decode("08739D638C627C897323A2F8C436F541");
			doTestChecksum(kek, iv, block, wrapper);
		}

		private void doTestChecksum(
			byte[]		kek,
			byte[]		iv,
			byte[]		block,
			IWrapper	wrapper)
		{
			encryptBlock(kek, iv, block);

			try
			{
				wrapper.Unwrap(block, 0, block.Length);

				Fail("bad checksum not detected");
			}
			catch (InvalidCipherTextException e)
			{
				if (!e.Message.Equals("wrapped key fails checksum"))
				{
					Fail("wrong exception");
				}
			}
		}

		private void encryptBlock(byte[] key, byte[] iv, byte[] cekBlock)
		{
			IBlockCipher engine = new CbcBlockCipher(new DesEngine());

			engine.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

			for (int i = 0; i < cekBlock.Length; i += 8)
			{
				engine.ProcessBlock(cekBlock, i, cekBlock, i);
			}

			for (int i = 0; i < cekBlock.Length; i += 8)
			{
				engine.ProcessBlock(cekBlock, i, cekBlock, i);
			}
		}

		public override void PerformTest()
		{
			doWrapTest(1, new DesEngine(), Hex.Decode("D1DAA78615F287E6"), Hex.Decode("EFE598EF21B33D6D"), r1, Hex.Decode("8C627C897323A2F8"), Hex.Decode("B81B2565EE373CA6DEDCA26A178B0C10"));
			doWrapTest(2, new DesEdeEngine(), Hex.Decode("6A8970BF68C92CAEA84A8DF28510858607126380CC47AB2D"), Hex.Decode("BAF1CA7931213C4E"), r2,
				Hex.Decode("8C637D887223A2F965B566EB014B0FA5D52300A3F7EA40FFFC577203C71BAF3B"),
				Hex.Decode("C03C514ABDB9E2C5AAC038572B5E24553876B377AAFB82ECA5A9D73F8AB143D9EC74E6CAD7DB260C"));

			doTestCorruption();
	        
			IWrapper wrapper = new Rfc3211WrapEngine(new DesEngine());
			ParametersWithIV parameters = new ParametersWithIV(new KeyParameter(new byte[16]), new byte[16]);
			byte[] buf = new byte[16];

			try
			{
				wrapper.Init(true, parameters);

				wrapper.Unwrap(buf, 0, buf.Length);

				Fail("failed Unwrap state test.");
			}
			catch (InvalidOperationException)
			{
				// expected
			}
			catch (InvalidCipherTextException e)
			{
				Fail("unexpected exception: " + e, e);
			}

			try
			{
				wrapper.Init(false, parameters);

				wrapper.Wrap(buf, 0, buf.Length);

				Fail("failed Unwrap state test.");
			}
			catch (InvalidOperationException)
			{
				// expected
			}

			//
			// short test
			//
			try
			{
				wrapper.Init(false, parameters);

				wrapper.Unwrap(buf, 0, buf.Length / 2);

				Fail("failed Unwrap short test.");
			}
			catch (InvalidCipherTextException)
			{
				// expected
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new Rfc3211WrapTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
