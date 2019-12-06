using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Pkcs.Tests
{
    [TestFixture]
    public class EncryptedPrivateKeyInfoTest
        : SimpleTest
    {
        private readonly string alg = PkcsObjectIdentifiers.PbeWithShaAnd3KeyTripleDesCbc.Id; // 3 key triple DES with SHA-1

		public override string Name
        {
			get { return "EncryptedPrivateKeyInfoTest"; }
        }

		public override void PerformTest()
        {
            IAsymmetricCipherKeyPairGenerator pGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
            RsaKeyGenerationParameters genParam = new RsaKeyGenerationParameters(
				BigInteger.ValueOf(0x10001), new SecureRandom(), 512, 25);

			pGen.Init(genParam);

			AsymmetricCipherKeyPair pair = pGen.GenerateKeyPair();

            //
            // set up the parameters
            //
            byte[] salt = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int iterationCount = 100;

			//
            // set up the key
            //
            char[] password1 = { 'h', 'e', 'l', 'l', 'o' };

            EncryptedPrivateKeyInfo  encInfo = EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(alg, password1, salt, iterationCount, PrivateKeyInfoFactory.CreatePrivateKeyInfo(pair.Private));

            PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(password1, encInfo);

            AsymmetricKeyParameter key = PrivateKeyFactory.CreateKey(info);

            if (!key.Equals(pair.Private))
            {
                Fail("Key corrupted");
            }

			doOpensslTestKeys();
		}

        private void doOpensslTestKeys()
		{
			string[] names = GetTestDataEntries("keys");
			foreach (string name in names)
			{
                if (!name.EndsWith(".key"))
                    continue;

//				Console.Write(name + " => ");
				Stream data = GetTestDataAsStream(name);
				AsymmetricKeyParameter key = PrivateKeyFactory.DecryptKey("12345678a".ToCharArray(), data);
//				Console.WriteLine(key.GetType().Name);
				if (!(key is RsaPrivateCrtKeyParameters))
				{
					Fail("Sample key could not be decrypted: " + name);
				}
			}
		}

		public static void Main(
            string[] args)
        {
			RunTest(new EncryptedPrivateKeyInfoTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
