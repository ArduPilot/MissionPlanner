using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	/// <remarks>
	/// MAC tester - vectors from
	/// <a href="http://www.itl.nist.gov/fipspubs/fip81.htm">FIP 81</a> and
	/// <a href="http://www.itl.nist.gov/fipspubs/fip113.htm">FIP 113</a>.
	/// </remarks>
	[TestFixture]
	public class MacTest
		: SimpleTest
	{
		private static readonly byte[] keyBytes = Hex.Decode("0123456789abcdef");
		private static readonly byte[] ivBytes = Hex.Decode("1234567890abcdef");

		private static readonly byte[] input = Hex.Decode("37363534333231204e6f77206973207468652074696d6520666f7220");

		private static readonly byte[] output1 = Hex.Decode("f1d30f68");
		private static readonly byte[] output2 = Hex.Decode("58d2e77e");
		private static readonly byte[] output3 = Hex.Decode("cd647403");

		private static readonly byte[] keyBytesISO9797 = Hex.Decode("7CA110454A1A6E570131D9619DC1376E");

		private static readonly byte[] inputISO9797 = Encoding.ASCII.GetBytes("Hello World !!!!");

		private static readonly byte[] outputISO9797 = Hex.Decode("F09B856213BAB83B");

		private static readonly byte[] inputDesEDE64 = Encoding.ASCII.GetBytes("Hello World !!!!");

		private static readonly byte[] outputDesEDE64 = Hex.Decode("862304d33af01096");

		private void aliasTest(
			KeyParameter	key,
			string			primary,
			params string[]	aliases)
		{
			IMac mac = MacUtilities.GetMac(primary);

			//
			// standard DAC - zero IV
			//
			mac.Init(key);

			mac.BlockUpdate(input, 0, input.Length);

			byte[] refBytes = new byte[mac.GetMacSize()];
			mac.DoFinal(refBytes, 0);

			for (int i = 0; i != aliases.Length; i++)
			{
				mac = MacUtilities.GetMac(aliases[i]);

				mac.Init(key);

				mac.BlockUpdate(input, 0, input.Length);

				byte[] outBytes = new byte[mac.GetMacSize()];
				mac.DoFinal(outBytes, 0);

				if (!AreEqual(outBytes, refBytes))
				{
					Fail("Failed - expected "
						+ Hex.ToHexString(refBytes) + " got "
						+ Hex.ToHexString(outBytes));
				}
			}
		}

		public override void PerformTest()
		{
			KeyParameter key = new DesParameters(keyBytes);
			IMac mac = MacUtilities.GetMac("DESMac");

			//
			// standard DAC - zero IV
			//
			mac.Init(key);

			mac.BlockUpdate(input, 0, input.Length);

			//byte[] outBytes = mac.DoFinal();
			byte[] outBytes = new byte[mac.GetMacSize()];
			mac.DoFinal(outBytes, 0);

			if (!AreEqual(outBytes, output1))
			{
				Fail("Failed - expected "
					+ Hex.ToHexString(output1) + " got "
					+ Hex.ToHexString(outBytes));
			}

			//
			// mac with IV.
			//
			mac.Init(new ParametersWithIV(key, ivBytes));

			mac.BlockUpdate(input, 0, input.Length);

			//outBytes = mac.DoFinal();
			outBytes = new byte[mac.GetMacSize()];
			mac.DoFinal(outBytes, 0);

			if (!AreEqual(outBytes, output2))
			{
				Fail("Failed - expected "
					+ Hex.ToHexString(output2) + " got "
					+ Hex.ToHexString(outBytes));
			}

			//
			// CFB mac with IV - 8 bit CFB mode
			//
			mac = MacUtilities.GetMac("DESMac/CFB8");

			mac.Init(new ParametersWithIV(key, ivBytes));

			mac.BlockUpdate(input, 0, input.Length);

			//outBytes = mac.DoFinal();
			outBytes = new byte[mac.GetMacSize()];
			mac.DoFinal(outBytes, 0);

			if (!AreEqual(outBytes, output3))
			{
				Fail("Failed - expected "
					+ Hex.ToHexString(output3) + " got "
					+ Hex.ToHexString(outBytes));
			}

			//
			// ISO9797 algorithm 3 using DESEDE
			//
			key = new DesEdeParameters(keyBytesISO9797);

			mac = MacUtilities.GetMac("ISO9797ALG3");

			mac.Init(key);

			mac.BlockUpdate(inputISO9797, 0, inputISO9797.Length);

			//outBytes = mac.DoFinal();
			outBytes = new byte[mac.GetMacSize()];
			mac.DoFinal(outBytes, 0);

			if (!AreEqual(outBytes, outputISO9797))
			{
				Fail("Failed - expected "
					+ Hex.ToHexString(outputISO9797) + " got "
					+ Hex.ToHexString(outBytes));
			}

			//
			// 64bit DESede Mac
			//
			key = new DesEdeParameters(keyBytesISO9797);

			mac = MacUtilities.GetMac("DESEDE64");

			mac.Init(key);

			mac.BlockUpdate(inputDesEDE64, 0, inputDesEDE64.Length);

			//outBytes = mac.DoFinal();
			outBytes = new byte[mac.GetMacSize()];
			mac.DoFinal(outBytes, 0);

			if (!AreEqual(outBytes, outputDesEDE64))
			{
				Fail("Failed - expected "
					+ Hex.ToHexString(outputDesEDE64) + " got "
					+ Hex.ToHexString(outBytes));
			}

			aliasTest(
				ParameterUtilities.CreateKeyParameter("DESede", keyBytesISO9797),
				"DESedeMac64withISO7816-4Padding",
				"DESEDE64WITHISO7816-4PADDING",
				"DESEDEISO9797ALG1MACWITHISO7816-4PADDING",
				"DESEDEISO9797ALG1WITHISO7816-4PADDING");

			aliasTest(
				ParameterUtilities.CreateKeyParameter("DESede", keyBytesISO9797),
				"ISO9797ALG3WITHISO7816-4PADDING",
				"ISO9797ALG3MACWITHISO7816-4PADDING");
		}

		public override string Name
		{
			get { return "Mac"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new MacTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
