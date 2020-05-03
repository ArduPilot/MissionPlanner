using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/// <summary>
	/// scrypt test vectors from "Stronger Key Derivation Via Sequential Memory-hard Functions" Appendix B.
	/// (http://www.tarsnap.com/scrypt/scrypt.pdf)
	/// </summary>
	[TestFixture]
	public class SCryptTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "SCrypt"; }
		}

		public override void PerformTest()
		{
            TestParameters();
            TestVectors();
        }

        [Test]
        public void TestParameters()
        {
            CheckOK("Minimal values", new byte[0], new byte[0], 2, 1, 1, 1);
            CheckIllegal("Cost parameter must be > 1", new byte[0], new byte[0], 1, 1, 1, 1);
            CheckOK("Cost parameter 32768 OK for r == 1", new byte[0], new byte[0], 32768, 1, 1, 1);
            CheckIllegal("Cost parameter must < 65536 for r == 1", new byte[0], new byte[0], 65536, 1, 1, 1);
            CheckIllegal("Block size must be >= 1", new byte[0], new byte[0], 2, 0, 2, 1);
            CheckIllegal("Parallelisation parameter must be >= 1", new byte[0], new byte[0], 2, 1, 0, 1);
            // CheckOK("Parallelisation parameter 65535 OK for r = 4", new byte[0], new byte[0], 2, 32, 65535, 1);
            CheckIllegal("Parallelisation parameter must be < 65535 for r = 4", new byte[0], new byte[0], 2, 32, 65536, 1);

            CheckIllegal("Len parameter must be > 1", new byte[0], new byte[0], 2, 1, 1, 0);
        }

        private void CheckOK(String msg, byte[] pass, byte[] salt, int N, int r, int p, int len)
        {
            try
            {
                SCrypt.Generate(pass, salt, N, r, p, len);
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Fail(msg);
            }
        }

        private void CheckIllegal(String msg, byte[] pass, byte[] salt, int N, int r, int p, int len)
        {
            try
            {
                SCrypt.Generate(pass, salt, N, r, p, len);
                Fail(msg);
            }
            catch (ArgumentException e)
            {
                //Console.Error.WriteLine(e.StackTrace);
            }
        }

        [Test]
        public void TestVectors()
        {
			using (StreamReader sr = new StreamReader(SimpleTest.GetTestDataAsStream("scrypt.TestVectors.txt")))
			{
				int count = 0;
				string line = sr.ReadLine();

				while (line != null)
				{
					++count;
					string header = line;
					StringBuilder data = new StringBuilder();

					while (!IsEndData(line = sr.ReadLine()))
					{
						data.Append(line.Replace(" ", ""));
					}

					int start = header.IndexOf('(') + 1;
					int limit = header.LastIndexOf(')');
					string argStr = header.Substring(start, limit - start);
					string[] args = argStr.Split(',');

					byte[] P = ExtractQuotedString(args[0]);
					byte[] S = ExtractQuotedString(args[1]);
					int N = ExtractInteger(args[2]);
					int r = ExtractInteger(args[3]);
					int p = ExtractInteger(args[4]);
					int dkLen = ExtractInteger(args[5]);
					byte[] expected = Hex.Decode(data.ToString());

					// This skips very expensive test case(s), remove check to re-enable
					if (N <= 16384)
					{
						byte[] result = SCrypt.Generate(P, S, N, r, p, dkLen);

						if (!AreEqual(expected, result))
						{
							Fail("Result does not match expected value in test case " + count);
						}
					}
				}
			}
		}

		private static bool IsEndData(string line)
		{
			return line == null || line.StartsWith("scrypt");
		}

		private static byte[] ExtractQuotedString(string arg)
		{
			arg = arg.Trim();
			arg = arg.Substring(1, arg.Length - 2);
			return Encoding.ASCII.GetBytes(arg);
		}

		private static int ExtractInteger(string arg)
		{
			return int.Parse(arg.Trim());
		}

		public static void Main(
			string[] args)
		{
			RunTest(new SCryptTest());
		}
	}
}
