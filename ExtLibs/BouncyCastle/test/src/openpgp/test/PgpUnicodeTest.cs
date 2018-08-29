using System;
using System.IO;
using System.Text;

using NUnit.Core;
using NUnit.Framework;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
    [TestFixture]
    public class PgpUnicodeTest
    {
        private void DoTestKey(BigInteger keyId, string passphrase, bool utf8)
        {
            PgpSecretKeyRingBundle secretKeyRing = LoadSecretKeyCollection("secring.gpg");

            PgpSecretKeyRing secretKey = secretKeyRing.GetSecretKeyRing(keyId.LongValue);
            Assert.NotNull(secretKey, "Could not locate secret keyring with Id=" + keyId.ToString(16));

            PgpSecretKey key = secretKey.GetSecretKey();
            Assert.NotNull(key, "Could not locate secret key!");

            try
            {
                char[] pass = passphrase.ToCharArray();

                PgpPrivateKey privateKey = utf8
                    ?   key.ExtractPrivateKeyUtf8(pass)
                    :   key.ExtractPrivateKey(pass);

                Assert.IsTrue(privateKey.KeyId == keyId.LongValue);
            }
            catch (PgpException e)
            {
                throw new PgpException("Password incorrect!", e);
            }

            // all fine!
        }

        [Test]
        public void TestUmlautPassphrase()
        {

            try
            {
                BigInteger keyId = new BigInteger("362961283C48132B9F14C5C3EC87272EFCB986D2", 16);

                string passphrase = Encoding.Unicode.GetString(Encoding.Unicode.GetBytes("Händle"));

 //             FileInputStream passwordFile = new FileInputStream("testdata/passphrase_for_test.txt");
 //             byte[] password = new byte[passwordFile.available()];
 //             passwordFile.read(password);
 //             passwordFile.close();
 //             String passphrase = new String(password);            

                DoTestKey(keyId, passphrase, true);

                // all fine!

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void TestAsciiPassphrase()
        {

            try
            {
                BigInteger keyId = new BigInteger("A392B7310C64026022405257AA2AAAC7CB417459", 16);

                string passphrase = "Admin123";

                DoTestKey(keyId, passphrase, false);
                DoTestKey(keyId, passphrase, true);

                // all fine!
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void TestCyrillicPassphrase()
        {

            try
            {
                BigInteger keyId = new BigInteger("B7773AF32BE4EC1806B1BACC4680E7F3960C44E7", 16);

                // XXX The password text file must not have the UTF-8 BOM !
                // Ref: http://stackoverflow.com/questions/2223882/whats-different-between-utf-8-and-utf-8-without-bom

                Stream passwordFile = SimpleTest.GetTestDataAsStream("openpgp.unicode.passphrase_cyr.txt");
                TextReader reader = new StreamReader(passwordFile, Encoding.UTF8);
                string passphrase = reader.ReadLine();
                passwordFile.Close();

                DoTestKey(keyId, passphrase, true);

                // all fine!
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Assert.Fail(e.Message);
            }
        }

        private PgpSecretKeyRingBundle LoadSecretKeyCollection(string keyName)
        {
            return new PgpSecretKeyRingBundle(SimpleTest.GetTestDataAsStream("openpgp.unicode." + keyName));
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
                TestSuite suite = new TestSuite("Unicode Password Tests");
                suite.Add(new PgpUnicodeTest());
                return suite;
            }
        }
    }
}
