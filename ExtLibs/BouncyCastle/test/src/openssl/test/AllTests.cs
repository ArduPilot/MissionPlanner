using System;
using System.IO;
using System.Text;

#if !LIB
using NUnit.Core;
#endif
using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.OpenSsl.Tests
{
	[TestFixture]
	public class AllTests
	{
		private class Password
			: IPasswordFinder
		{
			private readonly char[] password;

			public Password(
				char[] word)
			{
				this.password = (char[]) word.Clone();
			}

			public char[] GetPassword()
			{
				return (char[]) password.Clone();
			}
		}

#if !LIB
        public static void Main(string[] args)
        {
            Suite.Run(new NullListener(), NUnit.Core.TestFilter.Empty);
        }

        [Suite]
        public static TestSuite Suite
        {
            get
            {
                TestSuite suite = new TestSuite("OpenSSL Tests");
                suite.Add(new AllTests());
                return suite;
            }
        }
#endif

        [Test]
		public void TestOpenSsl()
		{
			Org.BouncyCastle.Utilities.Test.ITest[] tests = new Org.BouncyCastle.Utilities.Test.ITest[]{
				new ReaderTest(),
				new WriterTest()
			};

			foreach (Org.BouncyCastle.Utilities.Test.ITest test in tests)
			{
				SimpleTestResult result = (SimpleTestResult)test.Perform();

				if (!result.IsSuccessful())
				{
					Assert.Fail(result.ToString());
				}
			}
		}

		[Test]
		public void TestPkcs8Encrypted()
		{
			IAsymmetricCipherKeyPairGenerator kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
			kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 1024));

			AsymmetricKeyParameter privKey = kpGen.GenerateKeyPair().Private;

			// FIXME see PbeUtilities and Pkcs8Generator
//			EncryptedTest(privKey, Pkcs8Generator.Aes256Cbc);
//			EncryptedTest(privKey, Pkcs8Generator.Des3Cbc);
			EncryptedTest(privKey, Pkcs8Generator.PbeSha1_3DES);
		}

		private void EncryptedTest(AsymmetricKeyParameter privKey, string algorithm)
		{
			StringWriter sw = new StringWriter();
			PemWriter pWrt = new PemWriter(sw);
			Pkcs8Generator pkcs8 = new Pkcs8Generator(privKey, algorithm);
			pkcs8.Password = "hello".ToCharArray();

			pWrt.WriteObject(pkcs8);
			pWrt.Writer.Close();

			String result = sw.ToString();

			PemReader pRd = new PemReader(new StringReader(result), new Password("hello".ToCharArray()));

			AsymmetricKeyParameter rdKey = (AsymmetricKeyParameter)pRd.ReadObject();
			pRd.Reader.Close();

			Assert.AreEqual(privKey, rdKey);
		}

		[Test]
		public void TestPkcs8Plain()
		{
			IAsymmetricCipherKeyPairGenerator kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
			kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 1024));

			AsymmetricKeyParameter privKey = kpGen.GenerateKeyPair().Private;

			StringWriter sw = new StringWriter();
			PemWriter pWrt = new PemWriter(sw);

			Pkcs8Generator pkcs8 = new Pkcs8Generator(privKey);
			pWrt.WriteObject(pkcs8);
			pWrt.Writer.Close();

			string result = sw.ToString();

			PemReader pRd = new PemReader(new StringReader(result), new Password("hello".ToCharArray()));

			AsymmetricKeyParameter rdKey = (AsymmetricKeyParameter)pRd.ReadObject();
			pRd.Reader.Close();

			Assert.AreEqual(privKey, rdKey);
		}
	}
}
