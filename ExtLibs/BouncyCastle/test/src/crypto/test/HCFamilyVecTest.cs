using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	* HC-128 and HC-256 Tests. Based on the test vectors in the official reference
	* papers, respectively:
	* 
	* http://www.ecrypt.eu.org/stream/p3ciphers/hc/hc128_p3.pdf
	* http://www.ecrypt.eu.org/stream/p3ciphers/hc/hc256_p3.pdf
	*/
	[TestFixture]
	public class HCFamilyVecTest
		: SimpleTest
	{
		private class PeekableLineReader
			: StreamReader
		{
			public PeekableLineReader(Stream s)
				: base(s)
			{
				peek = base.ReadLine();
			}

			public string PeekLine()
			{
				return peek;
			}

			public override string ReadLine()
			{
				string tmp = peek;
				peek = base.ReadLine();
				return tmp;
			}

			private string peek; 
	    }

		public override string Name
		{
			get { return "HC-128 and HC-256"; }
		}

		public override void PerformTest()
		{
			RunTests(new HC128Engine(), "hc128/ecrypt_HC-128.txt");
			RunTests(new HC256Engine(), "hc256/ecrypt_HC-256_128K_128IV.txt");
			RunTests(new HC256Engine(), "hc256/ecrypt_HC-256_256K_128IV.txt");
			RunTests(new HC256Engine(), "hc256/ecrypt_HC-256_128K_256IV.txt");
			RunTests(new HC256Engine(), "hc256/ecrypt_HC-256_256K_256IV.txt");
		}

		private void RunTests(IStreamCipher hc, string fileName)
		{
			Stream resource = SimpleTest.GetTestDataAsStream(
				"hc256." + fileName.Replace('/', '.'));
			PeekableLineReader r = new PeekableLineReader(resource);
			RunAllVectors(hc, fileName, r);
		}

		private void RunAllVectors(IStreamCipher hc, string fileName, PeekableLineReader r)
		{
			for (;;)
			{
				string line = r.ReadLine();
				if (line == null)
					break;

				line = line.Trim();

				if (line.StartsWith("Set "))
				{
					RunVector(hc, fileName, r, line.Replace(":", ""));
				}
			}
		}

		private void RunVector(IStreamCipher hc, string fileName, PeekableLineReader r, string vectorName)
		{
//			Console.WriteLine(fileName + " => " + vectorName);
			string hexKey = ReadBlock(r);
			string hexIV = ReadBlock(r);

			ICipherParameters cp = new KeyParameter(Hex.Decode(hexKey));
			cp = new ParametersWithIV(cp, Hex.Decode(hexIV));
			hc.Init(true, cp);

			byte[] input = new byte[64];
			byte[] output = new byte[64];
			byte[] digest = new byte[64];
			int pos = 0;

			for (;;)
			{
				string line1 = r.PeekLine().Trim();
				int equalsPos = line1.IndexOf('=');
				string lead = line1.Substring(0, equalsPos - 1);

				string hexData = ReadBlock(r);
				byte[] data = Hex.Decode(hexData);

				if (lead.Equals("xor-digest"))
				{
					if (!Arrays.AreEqual(data, digest))
					{
						Fail("Failed in " + fileName + " for test vector: " + vectorName + " at " + lead);
//						Console.WriteLine(fileName + " => " + vectorName + " failed at " + lead); return;
					}
					break;
				}

				int posA = lead.IndexOf('[');
				int posB = lead.IndexOf("..");
				int posC = lead.IndexOf(']');
				int start = Int32.Parse(lead.Substring(posA + 1, posB - (posA + 1)));
				int end = Int32.Parse(lead.Substring(posB + 2, posC - (posB + 2)));

				if (start % 64 != 0 || (end - start != 63))
					throw new InvalidOperationException(vectorName + ": " + lead + " not on 64 byte boundaries");

				while (pos < end)
				{
					hc.ProcessBytes(input, 0, input.Length, output, 0);
					xor(digest, output);
					pos += 64;
				}

				if (!Arrays.AreEqual(data, output))
				{
					Fail("Failed in " + fileName + " for test vector: " + vectorName + " at " + lead);
//					Console.WriteLine(fileName + " => " + vectorName + " failed at " + lead); return;
				}
			}
		}

		private static string ReadBlock(PeekableLineReader r)
		{
			string first = r.ReadLine().Trim();
			string result = first.Substring(first.LastIndexOf(' ') + 1);

			for (;;)
			{
				string peek = r.PeekLine().Trim();
				if (peek.Length < 1 || peek.IndexOf('=') >= 0)
					break;
				result += r.ReadLine().Trim();
			}

			return result;
		}

		private static void xor(byte[] digest, byte[] block)
		{
			for (int i = 0; i < digest.Length; ++i)
			{
				digest[i] ^= block[i];
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new HCFamilyVecTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();
			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
