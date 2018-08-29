using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	/// <remarks>
	/// Basic FIPS test class for a block cipher, just to make sure ECB/CBC/OFB/CFB are behaving
	/// correctly. Tests from <a href="http://www.itl.nist.gov/fipspubs/fip81.htm">FIPS 81</a>.
	/// </remarks>
	[TestFixture]
	public class FipsDesTest
		: ITest
	{
		private static readonly string[] fips1Tests =
		{
			"DES/ECB/NoPadding",
			"3fa40e8a984d48156a271787ab8883f9893d51ec4b563b53",
			"DES/CBC/NoPadding",
			"e5c7cdde872bf27c43e934008c389c0f683788499a7c05f6",
			"DES/CFB/NoPadding",
			"f3096249c7f46e51a69e839b1a92f78403467133898ea622"
		};

		private static readonly string[] fips2Tests =
		{
			"DES/CFB8/NoPadding",
			"f31fda07011462ee187f",
			"DES/OFB8/NoPadding",
			"f34a2850c9c64985d684"
		};

		private static readonly byte[] input1 = Hex.Decode("4e6f77206973207468652074696d6520666f7220616c6c20");
		private static readonly byte[] input2 = Hex.Decode("4e6f7720697320746865");

		public string Name
		{
			get { return "FIPSDES"; }
		}

		public ITestResult doTest(
			string	algorithm,
			byte[]	input,
			byte[]	output)
		{
			KeyParameter key;
			IBufferedCipher inCipher, outCipher;
			CipherStream cIn, cOut;
			MemoryStream bIn, bOut;

//			IvParameterSpec spec = new IvParameterSpec();
			byte[] spec = Hex.Decode("1234567890abcdef");

			try
			{
				key = new DesParameters(Hex.Decode("0123456789abcdef"));

				inCipher = CipherUtilities.GetCipher(algorithm);
				outCipher = CipherUtilities.GetCipher(algorithm);

				if (algorithm.StartsWith("DES/ECB"))
				{
					outCipher.Init(true, key);
				}
				else
				{
					outCipher.Init(true, new ParametersWithIV(key, spec));
				}
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": " + algorithm + " failed initialisation - " + e.ToString(), e);
			}

			try
			{
				if (algorithm.StartsWith("DES/ECB"))
				{
					inCipher.Init(false, key);
				}
				else
				{
					inCipher.Init(false, new ParametersWithIV(key, spec));
				}
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": " + algorithm + " failed initialisation - " + e.ToString(), e);
			}

			//
			// encryption pass
			//
			bOut = new MemoryStream();
			cOut = new CipherStream(bOut, null, outCipher);

			try
			{
				for (int i = 0; i != input.Length / 2; i++)
				{
					cOut.WriteByte(input[i]);
				}
				cOut.Write(input, input.Length / 2, input.Length - input.Length / 2);
				cOut.Close();
			}
			catch (IOException e)
			{
				return new SimpleTestResult(false, Name + ": " + algorithm + " failed encryption - " + e.ToString());
			}

			byte[] bytes = bOut.ToArray();

			if (!Arrays.AreEqual(bytes, output))
			{
				return new SimpleTestResult(false, Name + ": " + algorithm + " failed encryption - expected "
					+ Hex.ToHexString(output) + " got " + Hex.ToHexString(bytes));
			}

			//
			// decryption pass
			//
			bIn = new MemoryStream(bytes, false);
			cIn = new CipherStream(bIn, inCipher, null);

			try
			{
				BinaryReader dIn = new BinaryReader(cIn);

				bytes = new byte[input.Length];

				for (int i = 0; i != input.Length / 2; i++)
				{
					bytes[i] = dIn.ReadByte();
				}

				int remaining = bytes.Length - input.Length / 2;
				byte[] extra = dIn.ReadBytes(remaining);
				if (extra.Length < remaining)
					throw new EndOfStreamException();
				extra.CopyTo(bytes, input.Length / 2);
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": " + algorithm + " failed encryption - " + e.ToString());
			}

			if (!Arrays.AreEqual(bytes, input))
			{
				return new SimpleTestResult(false, Name + ": " + algorithm + " failed decryption - expected "
					+ Hex.ToHexString(input) + " got " + Hex.ToHexString(bytes));
			}

			return new SimpleTestResult(true, Name + ": " + algorithm + " Okay");
		}

		public ITestResult Perform()
		{
			for (int i = 0; i != fips1Tests.Length; i += 2)
			{
				ITestResult result = doTest(fips1Tests[i], input1, Hex.Decode(fips1Tests[i + 1]));
				if (!result.IsSuccessful())
				{
					return result;
				}
			}

			for (int i = 0; i != fips2Tests.Length; i += 2)
			{
				ITestResult result = doTest(fips2Tests[i], input2, Hex.Decode(fips2Tests[i + 1]));
				if (!result.IsSuccessful())
				{
					return result;
				}
			}

			return new SimpleTestResult(true, Name + ": Okay");
		}

		public static void Main(
			string[] args)
		{
			ITest test = new FipsDesTest();
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
