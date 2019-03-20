using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	* VMPC Test
	*/
	[TestFixture]
	public class VmpcKsa3Test
		: SimpleTest
	{
		private static readonly byte[] input = new byte[1000000];

		public override string Name
		{
			get { return "VMPC-KSA3"; }
		}

		private void checkByte(byte[] array, int position, byte b)
		{
			if (array[position] != b)
			{
				Fail("Fail on position " + position,
					Hex.ToHexString(new byte[] { b }),
					Hex.ToHexString(new byte[] { array[position] }));
			}
		}

		public override void PerformTest()
		{
			byte[] key = Hex.Decode("9661410AB797D8A9EB767C21172DF6C7");
			byte[] iv = Hex.Decode("4B5C2F003E67F39557A8D26F3DA2B155");
			ICipherParameters kp = new KeyParameter(key);
			ICipherParameters kpwiv = new ParametersWithIV(kp, iv);

			VmpcKsa3Engine engine = new VmpcKsa3Engine();

			try
			{
				engine.Init(true, kp);
				Fail("Init failed to throw expected exception");
			}
			catch (ArgumentException)
			{
				// Expected
			}

			engine.Init(true, kpwiv);
			checkEngine(engine);

			engine.Reset();
			byte[] output = checkEngine(engine);

			engine.Init(false, kpwiv);
			byte[] recovered = new byte[output.Length];
			engine.ProcessBytes(output, 0, output.Length, recovered, 0);

			if (!Arrays.AreEqual(input, recovered))
			{
				Fail("decrypted bytes differ from original bytes");
			}
		}

		private byte[] checkEngine(VmpcKsa3Engine engine)
		{
			byte[] output = new byte[input.Length];
			engine.ProcessBytes(input, 0, output.Length, output, 0);

			checkByte(output, 0, (byte) 0xB6);
			checkByte(output, 1, (byte) 0xEB);
			checkByte(output, 2, (byte) 0xAE);
			checkByte(output, 3, (byte) 0xFE);
			checkByte(output, 252, (byte) 0x48);
			checkByte(output, 253, (byte) 0x17);
			checkByte(output, 254, (byte) 0x24);
			checkByte(output, 255, (byte) 0x73);
			checkByte(output, 1020, (byte) 0x1D);
			checkByte(output, 1021, (byte) 0xAE);
			checkByte(output, 1022, (byte) 0xC3);
			checkByte(output, 1023, (byte) 0x5A);
			checkByte(output, 102396, (byte) 0x1D);
			checkByte(output, 102397, (byte) 0xA7);
			checkByte(output, 102398, (byte) 0xE1);
			checkByte(output, 102399, (byte) 0xDC);

			return output;
		}

		public static void Main(
			string[] args)
		{
			RunTest(new VmpcKsa3Test());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
