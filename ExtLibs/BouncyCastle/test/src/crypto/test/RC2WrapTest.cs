using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	* RC2 wrap tester
	*/
	[TestFixture]
	public class RC2WrapTest
		: ITest
	{
		private class RFCRandom
			: SecureRandom
		{
			public override void NextBytes(
				byte[] nextBytes)
			{
				Array.Copy(Hex.Decode("4845cce7fd1250"), 0, nextBytes, 0, nextBytes.Length);
			}
		}

		private ITestResult wrapTest(
			int     id,
			ICipherParameters paramsWrap,
			ICipherParameters paramsUnwrap,
			byte[]  inBytes,
			byte[]  outBytes)
		{
			IWrapper wrapper = new RC2WrapEngine();

			wrapper.Init(true, paramsWrap);

			try
			{
				byte[]  cText = wrapper.Wrap(inBytes, 0, inBytes.Length);
				if (!Arrays.AreEqual(cText, outBytes))
				{
					return new SimpleTestResult(false, Name + ": failed wrap test " + id
						+ " expected " + Hex.ToHexString(outBytes)
						+ " got " + Hex.ToHexString(cText));
				}
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": failed wrap test exception " + e, e);
			}

			wrapper.Init(false, paramsUnwrap);

			try
			{
				byte[]  pText = wrapper.Unwrap(outBytes, 0, outBytes.Length);
				if (!Arrays.AreEqual(pText, inBytes))
				{
					return new SimpleTestResult(false, Name + ": failed unwrap test " + id
						+ " expected " + Hex.ToHexString(inBytes)
						+ " got " + Hex.ToHexString(pText));
				}
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": failed unwrap test exception " + e, e);
			}

			return new SimpleTestResult(true, Name + ": Okay");
		}

		public ITestResult Perform()
		{
			byte[]  kek1 = Hex.Decode("fd04fd08060707fb0003fefffd02fe05");
			byte[]  iv1 = Hex.Decode("c7d90059b29e97f7");
			byte[]  in1 = Hex.Decode("b70a25fbc9d86a86050ce0d711ead4d9");
			byte[]  out1 = Hex.Decode("70e699fb5701f7833330fb71e87c85a420bdc99af05d22af5a0e48d35f3138986cbaafb4b28d4f35");
			//
			// note the RFC 3217 test specifies a key to be used with an effective key size of
			// 40 bits which is why it is done here - in practice nothing less than 128 bits should be used.
			//
			ICipherParameters paramWrap = new ParametersWithRandom(new ParametersWithIV(new RC2Parameters(kek1, 40), iv1), new RFCRandom());
			ICipherParameters paramUnwrap = new RC2Parameters(kek1, 40);

			ITestResult result = wrapTest(1, paramWrap, paramUnwrap, in1, out1);

			if (!result.IsSuccessful())
			{
				return result;
			}

			return new SimpleTestResult(true, Name + ": Okay");
		}

		public string Name
		{
			get { return "RC2Wrap"; }
		}

		public static void Main(
			string[] args)
		{
			ITest test = new RC2WrapTest();
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
