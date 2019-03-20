using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class NullTest
		: CipherTest
	{
		static SimpleTest[]  tests =
		{
			new BlockCipherVectorTest(0, new NullEngine(),
					new KeyParameter(Hex.Decode("00")), "00", "00")
		};

		public NullTest()
			: base(tests, new NullEngine(), new KeyParameter(new byte[2]))
		{
		}

		public override string Name
		{
			get { return "Null"; }
		}

		public override void PerformTest()
		{
			base.PerformTest();

			IBlockCipher engine = new NullEngine();

			engine.Init(true, null);

			byte[] buf = new byte[1];

			engine.ProcessBlock(buf, 0, buf, 0);

			if (buf[0] != 0)
			{
				Fail("NullCipher changed data!");
			}

			byte[] shortBuf = new byte[0];

			try
			{
				engine.ProcessBlock(shortBuf, 0, buf, 0);

				Fail("failed short input check");
			}
			catch (DataLengthException)
			{
				// expected
			}

			try
			{
				engine.ProcessBlock(buf, 0, shortBuf, 0);

				Fail("failed short output check");
			}
			catch (DataLengthException)
			{
				// expected
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new NullTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
