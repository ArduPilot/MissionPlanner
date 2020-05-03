#if !LIB
using System;
using System.IO;

using NUnit.Core;
using NUnit.Framework;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Examples.Tests
{
	[TestFixture]
	public class AllTests
	{
		private static readonly byte[] clearSignedPublicKey = Base64.Decode(
			  "mQELBEQh2+wBCAD26kte0hO6flr7Y2aetpPYutHY4qsmDPy+GwmmqVeCDkX+"
			+ "r1g7DuFbMhVeu0NkKDnVl7GsJ9VarYsFYyqu0NzLa9XS2qlTIkmJV+2/xKa1"
			+ "tzjn18fT/cnAWL88ZLCOWUr241aPVhLuIc6vpHnySpEMkCh4rvMaimnTrKwO"
			+ "42kgeDGd5cXfs4J4ovRcTbc4hmU2BRVsRjiYMZWWx0kkyL2zDVyaJSs4yVX7"
			+ "Jm4/LSR1uC/wDT0IJJuZT/gQPCMJNMEsVCziRgYkAxQK3OWojPSuv4rXpyd4"
			+ "Gvo6IbvyTgIskfpSkCnQtORNLIudQSuK7pW+LkL62N+ohuKdMvdxauOnAAYp"
			+ "tBNnZ2dnZ2dnZyA8Z2dnQGdnZ2c+iQE2BBMBAgAgBQJEIdvsAhsDBgsJCAcD"
			+ "AgQVAggDBBYCAwECHgECF4AACgkQ4M/Ier3f9xagdAf/fbKWBjLQM8xR7JkR"
			+ "P4ri8YKOQPhK+VrddGUD59/wzVnvaGyl9MZE7TXFUeniQq5iXKnm22EQbYch"
			+ "v2Jcxyt2H9yptpzyh4tP6tEHl1C887p2J4qe7F2ATua9CzVGwXQSUbKtj2fg"
			+ "UZP5SsNp25guhPiZdtkf2sHMeiotmykFErzqGMrvOAUThrO63GiYsRk4hF6r"
			+ "cQ01d+EUVpY/sBcCxgNyOiB7a84sDtrxnX5BTEZDTEj8LvuEyEV3TMUuAjx1"
			+ "7Eyd+9JtKzwV4v3hlTaWOvGro9nPS7YaPuG+RtufzXCUJPbPfTjTvtGOqvEz"
			+ "oztls8tuWA0OGHba9XfX9rfgorACAAM=");

		private static readonly string crOnlyMessage =
			"\r"
			+ " hello world!\r"
			+ "\r"
			+ "- dash\r";

		private static readonly string nlOnlyMessage =
			"\n"
			+ " hello world!\n"
			+ "\n"
			+ "- dash\n";

		private static readonly string crNlMessage =
			"\r\n"
			+ " hello world!\r\n"
			+ "\r\n"
			+ "- dash\r\n";

		private static readonly string crNlMessageTrailingWhiteSpace =
			"\r\n"
			+ " hello world! \t\r\n"
			+ "\r\n"
			+ "\r\n";

		private static readonly string crOnlySignedMessage =
			"-----BEGIN PGP SIGNED MESSAGE-----\r"
			+ "Hash: SHA256\r"
			+ "\r"
			+ "\r"
			+ " hello world!\r"
			+ "\r"
			+ "- - dash\r"
			+ "-----BEGIN PGP SIGNATURE-----\r"
			+ "Version: GnuPG v1.4.2.1 (GNU/Linux)\r"
			+ "\r"
			+ "iQEVAwUBRCNS8+DPyHq93/cWAQi6SwgAj3ItmSLr/sd/ixAQLW7/12jzEjfNmFDt\r"
			+ "WOZpJFmXj0fnMzTrOILVnbxHv2Ru+U8Y1K6nhzFSR7d28n31/XGgFtdohDEaFJpx\r"
			+ "Fl+KvASKIonnpEDjFJsPIvT1/G/eCPalwO9IuxaIthmKj0z44SO1VQtmNKxdLAfK\r"
			+ "+xTnXGawXS1WUE4CQGPM45mIGSqXcYrLtJkAg3jtRa8YRUn2d7b2BtmWH+jVaVuC\r"
			+ "hNrXYv7iHFOu25yRWhUQJisvdC13D/gKIPRvARXPgPhAC2kovIy6VS8tDoyG6Hm5\r"
			+ "dMgLEGhmqsgaetVq1ZIuBZj5S4j2apBJCDpF6GBfpBOfwIZs0Tpmlw==\r"
			+ "=84Nd\r"
			+ "-----END PGP SIGNATURE-----\r";

		private static readonly string nlOnlySignedMessage =
			"-----BEGIN PGP SIGNED MESSAGE-----\n"
			+ "Hash: SHA256\n"
			+ "\n"
			+ "\n"
			+ " hello world!\n"
			+ "\n"
			+ "- - dash\n"
			+ "-----BEGIN PGP SIGNATURE-----\n"
			+ "Version: GnuPG v1.4.2.1 (GNU/Linux)\n"
			+ "\n"
			+ "iQEVAwUBRCNS8+DPyHq93/cWAQi6SwgAj3ItmSLr/sd/ixAQLW7/12jzEjfNmFDt\n"
			+ "WOZpJFmXj0fnMzTrOILVnbxHv2Ru+U8Y1K6nhzFSR7d28n31/XGgFtdohDEaFJpx\n"
			+ "Fl+KvASKIonnpEDjFJsPIvT1/G/eCPalwO9IuxaIthmKj0z44SO1VQtmNKxdLAfK\n"
			+ "+xTnXGawXS1WUE4CQGPM45mIGSqXcYrLtJkAg3jtRa8YRUn2d7b2BtmWH+jVaVuC\n"
			+ "hNrXYv7iHFOu25yRWhUQJisvdC13D/gKIPRvARXPgPhAC2kovIy6VS8tDoyG6Hm5\n"
			+ "dMgLEGhmqsgaetVq1ZIuBZj5S4j2apBJCDpF6GBfpBOfwIZs0Tpmlw==\n"
			+ "=84Nd\n"
			+ "-----END PGP SIGNATURE-----\n";

		private static readonly string crNlSignedMessage =
			"-----BEGIN PGP SIGNED MESSAGE-----\r\n"
			+ "Hash: SHA256\r\n"
			+ "\r\n"
			+ "\r\n"
			+ " hello world!\r\n"
			+ "\r\n"
			+ "- - dash\r\n"
			+ "-----BEGIN PGP SIGNATURE-----\r\n"
			+ "Version: GnuPG v1.4.2.1 (GNU/Linux)\r\n"
			+ "\r\n"
			+ "iQEVAwUBRCNS8+DPyHq93/cWAQi6SwgAj3ItmSLr/sd/ixAQLW7/12jzEjfNmFDt\r\n"
			+ "WOZpJFmXj0fnMzTrOILVnbxHv2Ru+U8Y1K6nhzFSR7d28n31/XGgFtdohDEaFJpx\r\n"
			+ "Fl+KvASKIonnpEDjFJsPIvT1/G/eCPalwO9IuxaIthmKj0z44SO1VQtmNKxdLAfK\r\n"
			+ "+xTnXGawXS1WUE4CQGPM45mIGSqXcYrLtJkAg3jtRa8YRUn2d7b2BtmWH+jVaVuC\r\n"
			+ "hNrXYv7iHFOu25yRWhUQJisvdC13D/gKIPRvARXPgPhAC2kovIy6VS8tDoyG6Hm5\r\n"
			+ "dMgLEGhmqsgaetVq1ZIuBZj5S4j2apBJCDpF6GBfpBOfwIZs0Tpmlw==\r\n"
			+ "=84Nd\r"
			+ "-----END PGP SIGNATURE-----\r\n";

		private static readonly string crNlSignedMessageTrailingWhiteSpace =
			"-----BEGIN PGP SIGNED MESSAGE-----\r\n"
			+ "Hash: SHA256\r\n"
			+ "\r\n"
			+ "\r\n"
			+ " hello world! \t\r\n"
			+ "\r\n"
			+ "- - dash\r\n"
			+ "-----BEGIN PGP SIGNATURE-----\r\n"
			+ "Version: GnuPG v1.4.2.1 (GNU/Linux)\r\n"
			+ "\r\n"
			+ "iQEVAwUBRCNS8+DPyHq93/cWAQi6SwgAj3ItmSLr/sd/ixAQLW7/12jzEjfNmFDt\r\n"
			+ "WOZpJFmXj0fnMzTrOILVnbxHv2Ru+U8Y1K6nhzFSR7d28n31/XGgFtdohDEaFJpx\r\n"
			+ "Fl+KvASKIonnpEDjFJsPIvT1/G/eCPalwO9IuxaIthmKj0z44SO1VQtmNKxdLAfK\r\n"
			+ "+xTnXGawXS1WUE4CQGPM45mIGSqXcYrLtJkAg3jtRa8YRUn2d7b2BtmWH+jVaVuC\r\n"
			+ "hNrXYv7iHFOu25yRWhUQJisvdC13D/gKIPRvARXPgPhAC2kovIy6VS8tDoyG6Hm5\r\n"
			+ "dMgLEGhmqsgaetVq1ZIuBZj5S4j2apBJCDpF6GBfpBOfwIZs0Tpmlw==\r\n"
			+ "=84Nd\r"
			+ "-----END PGP SIGNATURE-----\r\n";

		private TextWriter _oldOut;
		private TextWriter _oldErr;

		private MemoryStream _currentOut;
		private MemoryStream _currentErr;

		[SetUp]
		public void SetUp()
		{
			_oldOut = Console.Out;
			_oldErr = Console.Error;
			_currentOut = new MemoryStream();
			_currentErr = new MemoryStream();

			Console.SetOut(new StreamWriter(_currentOut));
			Console.SetError(new StreamWriter(_currentErr));
		}

		[TearDown]
		public void TearDown()
		{
			Console.SetOut(_oldOut);
			Console.SetError(_oldErr);
		}

		[Test]
		public void TestRsaKeyGeneration() 
		{
			RsaKeyRingGenerator.Main(new string[]{ "test", "password" });

			CreateSmallTestInput();
			CreateLargeTestInput();

			CheckSigning("bpg");
			CheckKeyBasedEncryption("bpg");
			CheckLargeKeyBasedEncryption("bpg");

			RsaKeyRingGenerator.Main(new string[]{ "-a", "test", "password" });

			CheckSigning("asc");
			CheckKeyBasedEncryption("asc");
			CheckLargeKeyBasedEncryption("asc");
		}

		[Test]
		public void TestDsaElGamalKeyGeneration() 
		{
			DsaElGamalKeyRingGenerator.Main(new string[]{ "test", "password" });

			CreateSmallTestInput();
			CreateLargeTestInput();

			CheckSigning("bpg");
			CheckKeyBasedEncryption("bpg");
			CheckLargeKeyBasedEncryption("bpg");

			DsaElGamalKeyRingGenerator.Main(new string[]{ "-a", "test", "password" });

			CheckSigning("asc");
			CheckKeyBasedEncryption("asc");
			CheckLargeKeyBasedEncryption("asc");
		}

		[Test]
		public void TestPbeEncryption() 
		{
			Console.Error.Flush();
			_currentErr.SetLength(0);

			PbeFileProcessor.Main(new string[]{ "-e", "test.txt", "password" });

			PbeFileProcessor.Main(new string[]{ "-d", "test.txt.bpg", "password" });

			Console.Error.Flush();
			Assert.AreEqual("no message integrity check", GetLine(_currentErr));

			PbeFileProcessor.Main(new string[]{ "-e", "-i", "test.txt", "password" });

			PbeFileProcessor.Main(new string[]{ "-d", "test.txt.bpg", "password" });

			Console.Error.Flush();
			Assert.AreEqual("message integrity check passed", GetLine(_currentErr));

			PbeFileProcessor.Main(new string[]{ "-e", "-ai", "test.txt", "password" });

			PbeFileProcessor.Main(new string[]{ "-d", "test.txt.asc", "password" });

			Console.Error.Flush();
			Assert.AreEqual("message integrity check passed", GetLine(_currentErr));
		}

		[Test]
		public void TestClearSigned()
		{
			CreateTestFile(clearSignedPublicKey, "pub.bpg");

			CheckClearSignedVerify(nlOnlySignedMessage);
			CheckClearSignedVerify(crOnlySignedMessage);
			CheckClearSignedVerify(crNlSignedMessage);
			CheckClearSignedVerify(crNlSignedMessageTrailingWhiteSpace);

			ClearSignedFileProcessor.Main(new string[]{ "-v", "test.txt.asc", "pub.bpg" });

			RsaKeyRingGenerator.Main(new string[]{ "test", "password" });

			CheckClearSigned(crOnlyMessage);
			CheckClearSigned(nlOnlyMessage);
			CheckClearSigned(crNlMessage);
			CheckClearSigned(crNlMessageTrailingWhiteSpace);
		}

		[Test]
		public void TestClearSignedBogusInput()
		{
			CreateTestFile(clearSignedPublicKey, "test.txt");

            RsaKeyRingGenerator.Main(new string[]{ "test", "password" });

			ClearSignedFileProcessor.Main(new string[]{ "-s", "test.txt", "secret.bpg", "password" });
		}

        [Test]
        public void TestClearSignedSingleLine()
        {
            CreateTestData("This is a test payload!" + Environment.NewLine, "test.txt");
            CreateTestData("This is a test payload!" + Environment.NewLine, "test.bak");

            RsaKeyRingGenerator.Main(new string[]{ "test", "password" });

            ClearSignedFileProcessor.Main(new string[]{"-s", "test.txt", "secret.bpg", "password"});
            ClearSignedFileProcessor.Main(new string[]{"-v", "test.txt.asc", "pub.bpg"});

            CompareFile("test.bak", "test.txt");
        }

        private void CheckClearSignedVerify(
			string message)
		{
			CreateTestData(message, "test.txt.asc");

			ClearSignedFileProcessor.Main(new string[]{ "-v", "test.txt.asc", "pub.bpg" });
		}

        private void CompareFile(string file1, string file2)
        {
            byte[] data1 = GetFileContents(file1);
            byte[] data2 = GetFileContents(file2);

            Assert.IsTrue(Arrays.AreEqual(data1, data2));
        }

        private byte[] GetFileContents(string name)
        {
            FileStream fs = File.OpenRead(name);
            byte[] contents = Streams.ReadAll(fs);
            fs.Close();
            return contents;
        }

        private void CheckClearSigned(
			string message)
		{
			CreateTestData(message, "test.txt");

			ClearSignedFileProcessor.Main(new string[]{ "-s", "test.txt", "secret.bpg", "password" });
			ClearSignedFileProcessor.Main(new string[]{ "-v", "test.txt.asc", "pub.bpg" });
		}

		private void CheckSigning(
			string type) 
		{
			Console.Out.Flush();
			_currentOut.SetLength(0);

			SignedFileProcessor.Main(new string[]{ "-s", "test.txt", "secret." + type, "password" });
			SignedFileProcessor.Main(new string[]{ "-v", "test.txt.bpg", "pub." + type });

			Console.Out.Flush();
			Assert.AreEqual("signature verified.", GetLine(_currentOut));

			SignedFileProcessor.Main(new string[]{ "-s", "-a", "test.txt", "secret." + type, "password" });
			SignedFileProcessor.Main(new string[]{ "-v", "test.txt.asc", "pub." + type });

			Console.Out.Flush();
			Assert.AreEqual("signature verified.", GetLine(_currentOut));
		}

		private void CheckKeyBasedEncryption(
			string type) 
		{
			Console.Error.Flush();
			_currentErr.SetLength(0);

			KeyBasedFileProcessor.Main(new string[]{ "-e", "test.txt", "pub." + type });
			KeyBasedFileProcessor.Main(new string[]{ "-d", "test.txt.bpg", "secret." + type, "password" });

			Console.Error.Flush();
			Assert.AreEqual("no message integrity check", GetLine(_currentErr));

			KeyBasedFileProcessor.Main(new string[]{ "-e", "-i", "test.txt", "pub." + type });
			KeyBasedFileProcessor.Main(new string[]{ "-d", "test.txt.bpg", "secret." + type, "password" });

			Console.Error.Flush();
			Assert.AreEqual("message integrity check passed", GetLine(_currentErr));

			KeyBasedFileProcessor.Main(new string[]{ "-e", "-ai", "test.txt", "pub." + type });
			KeyBasedFileProcessor.Main(new string[]{ "-d", "test.txt.asc", "secret." + type, "password" });

			Console.Error.Flush();
			Assert.AreEqual("message integrity check passed", GetLine(_currentErr));
		}

		private void CheckLargeKeyBasedEncryption(
			string type) 
		{
			Console.Error.Flush();
			_currentErr.SetLength(0);

			KeyBasedLargeFileProcessor.Main(new string[]{ "-e", "large.txt", "pub." + type });
			KeyBasedLargeFileProcessor.Main(new string[]{ "-d", "large.txt.bpg", "secret." + type, "password" });

			Console.Error.Flush();
			Assert.AreEqual("no message integrity check", GetLine(_currentErr));

			KeyBasedLargeFileProcessor.Main(new string[]{ "-e", "-i", "large.txt", "pub." + type });
			KeyBasedLargeFileProcessor.Main(new string[]{ "-d", "large.txt.bpg", "secret." + type, "password" });

			Console.Error.Flush();
			Assert.AreEqual("message integrity check passed", GetLine(_currentErr));

			KeyBasedLargeFileProcessor.Main(new string[]{ "-e", "-ai", "large.txt", "pub." + type });
			KeyBasedLargeFileProcessor.Main(new string[]{ "-d", "large.txt.asc", "secret." + type, "password" });

			Console.Error.Flush();
			Assert.AreEqual("message integrity check passed", GetLine(_currentErr));
		}

		private void CreateSmallTestInput() 
		{
			TextWriter bfOut = new StreamWriter(File.Create("test.txt"));
			bfOut.WriteLine("hello world!");
			bfOut.Close();
		}

		private void CreateLargeTestInput() 
		{
			TextWriter bfOut = new StreamWriter(File.Create("large.txt"));

			for (int i = 1; i <= 2000; i++)
			{
				bfOut.WriteLine("hello to planet " + i + "!");
			}

			bfOut.Close();
		}

		private void CreateTestData(
			string	testData,
			string	name)
		{
            FileStream fOut = File.Create(name);
			TextWriter bfOut = new StreamWriter(fOut);
			bfOut.Write(testData);
			bfOut.Close();
		}

		private void CreateTestFile(
			byte[]	keyData,
			string	name)
		{
			FileStream fOut = File.Create(name);
			fOut.Write(keyData, 0, keyData.Length);
			fOut.Close();
		}

		private string GetLine(
			MemoryStream outStr) 
		{
			byte[] b = outStr.ToArray();
			TextReader bRd = new StreamReader(new MemoryStream(b, false));
			outStr.SetLength(0);
			return bRd.ReadLine();
		}

        public static void Main(string[] args)
        {
            Suite.Run(new NullListener(), NUnit.Core.TestFilter.Empty);
        }

        [Suite]
        public static TestSuite Suite
        {
            get
            {
                TestSuite suite = new TestSuite("OpenPGP Example Tests");
                suite.Add(new AllTests());
                return suite;
            }
        }
	}
}
#endif
