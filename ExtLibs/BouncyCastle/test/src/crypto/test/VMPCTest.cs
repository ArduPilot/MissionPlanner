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
	public class VmpcTest
		: SimpleTest
	{
		private static readonly byte[] input = new byte[1000000];

		public override string Name
		{
			get { return "VMPC"; }
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

			VmpcEngine engine = new VmpcEngine();

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

		private byte[] checkEngine(VmpcEngine engine)
		{
			byte[] output = new byte[input.Length];
			engine.ProcessBytes(input, 0, output.Length, output, 0);

			checkByte(output, 0, (byte) 0xA8);
			checkByte(output, 1, (byte) 0x24);
			checkByte(output, 2, (byte) 0x79);
			checkByte(output, 3, (byte) 0xF5);
			checkByte(output, 252, (byte) 0xB8);
			checkByte(output, 253, (byte) 0xFC);
			checkByte(output, 254, (byte) 0x66);
			checkByte(output, 255, (byte) 0xA4);
			checkByte(output, 1020, (byte) 0xE0);
			checkByte(output, 1021, (byte) 0x56);
			checkByte(output, 1022, (byte) 0x40);
			checkByte(output, 1023, (byte) 0xA5);
			checkByte(output, 102396, (byte) 0x81);
			checkByte(output, 102397, (byte) 0xCA);
			checkByte(output, 102398, (byte) 0x49);
			checkByte(output, 102399, (byte) 0x9A);

			return output;
		}

		public static void Main(
			string[] args)
		{
			RunTest(new VmpcTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
