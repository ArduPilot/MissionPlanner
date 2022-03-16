using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /*
     * bcrypt test vectors
     */
    [TestFixture]
    public class BCryptTest
        :   SimpleTest
    {
        // Raw test vectors based on crypt style test vectors
        // Cross checked with JBCrypt
        private static readonly object[][] TestVectorData = {
            new object[]{"", "144b3d691a7b4ecf39cf735c7fa7a79c", 6, "557e94f34bf286e8719a26be94ac1e16d95ef9f819dee092"},
            new object[]{"00", "144b3d691a7b4ecf39cf735c7fa7a79c", 6, "557e94f34bf286e8719a26be94ac1e16d95ef9f819dee092"},
            new object[]{"00", "26c63033c04f8bcba2fe24b574db6274", 8, "56701b26164d8f1bc15225f46234ac8ac79bf5bc16bf48ba"},
            new object[]{"00", "9b7c9d2ada0fd07091c915d1517701d6", 10, "7b2e03106a43c9753821db688b5cc7590b18fdf9ba544632"},
            new object[]{"6100", "a3612d8c9a37dac2f99d94da03bd4521", 6, "e6d53831f82060dc08a2e8489ce850ce48fbf976978738f3"},
            new object[]{"6100", "7a17b15dfe1c4be10ec6a3ab47818386", 8, "a9f3469a61cbff0a0f1a1445dfe023587f38b2c9c40570e1"},
            new object[]{"6100", "9bef4d04e1f8f92f3de57323f8179190", 10, "5169fd39606d630524285147734b4c981def0ee512c3ace1"},
            new object[]{"61626300", "2a1f1dc70a3d147956a46febe3016017", 6, "d9a275b493bcbe1024b0ff80d330253cfdca34687d8f69e5"},
            new object[]{"61626300", "4ead845a142c9bc79918c8797f470ef5", 8, "8d4131a723bfbbac8a67f2e035cae08cc33b69f37331ea91"},
            new object[]{"61626300", "631c554493327c32f9c26d9be7d18e4c", 10, "8cd0b863c3ff0860e31a2b42427974e0283b3af7142969a6"},
            new object[]{"6162636465666768696a6b6c6d6e6f707172737475767778797a00", "02d1176d74158ee29cffdac6150cf123", 6, "4d38b523ce9dc6f2f6ff9fb3c2cd71dfe7f96eb4a3baf19f"},
            new object[]{"6162636465666768696a6b6c6d6e6f707172737475767778797a00", "715b96caed2ac92c354ed16c1e19e38a", 8, "98bf9ffc1f5be485f959e8b1d526392fbd4ed2d5719f506b"},
            new object[]{"6162636465666768696a6b6c6d6e6f707172737475767778797a00", "85727e838f9049397fbec90566ede0df", 10, "cebba53f67bd28af5a44c6707383c231ac4ef244a6f5fb2b"},
            new object[]{"7e21402324255e262a28292020202020207e21402324255e262a2829504e4246524400", "8512ae0d0fac4ec9a5978f79b6171028", 6, "26f517fe5345ad575ba7dfb8144f01bfdb15f3d47c1e146a"},
            new object[]{"7e21402324255e262a28292020202020207e21402324255e262a2829504e4246524400", "1ace2de8807df18c79fced54678f388f", 8, "d51d7cdf839b91a25758b80141e42c9f896ae80fd6cd561f"},
            new object[]{"7e21402324255e262a28292020202020207e21402324255e262a2829504e4246524400", "36285a6267751b14ba2dc989f6d43126", 10, "db4fab24c1ff41c1e2c966f8b3d6381c76e86f52da9e15a9"},
            new object[]{"c2a300", "144b3d691a7b4ecf39cf735c7fa7a79c", 6, "5a6c4fedb23980a7da9217e0442565ac6145b687c7313339"},
        };

        public override string Name
        {
            get { return "BCrypt"; }
        }

        public override void PerformTest()
        {
            DoTestParameters();
            DoTestShortKeys();
            DoTestVectors();
        }

        private void DoTestShortKeys()
        {
            byte[] salt = new byte[16];

            // Check BCrypt with empty key pads to zero byte key
            byte[] hashEmpty = BCrypt.Generate(new byte[0], salt, 4);
            byte[] hashZero1 = BCrypt.Generate(new byte[1], salt, 4);

            if (!Arrays.AreEqual(hashEmpty, hashZero1))
            {
                Fail("Hash for empty password should equal zeroed key", Hex.ToHexString(hashEmpty),
                    Hex.ToHexString(hashZero1));
            }

            // Check zeroed byte key of min Blowfish length is equivalent
            byte[] hashZero4 = BCrypt.Generate(new byte[4], salt, 4);
            if (!Arrays.AreEqual(hashEmpty, hashZero4))
            {
                Fail("Hash for empty password should equal zeroed key[4]", Hex.ToHexString(hashEmpty),
                    Hex.ToHexString(hashZero4));
            }

            // Check BCrypt isn't padding too small (32 bit) keys
            byte[] hashA = BCrypt.Generate(new byte[]{(byte)'a'}, salt, 4);
            byte[] hashA0 = BCrypt.Generate(new byte[]{(byte)'a', (byte)0}, salt, 4);
            if (Arrays.AreEqual(hashA, hashA0))
            {
                Fail("Small keys should not be 0 padded.");
            }
        }

        public void DoTestParameters()
        {
            CheckOK("Empty key", new byte[0], new byte[16], 4);
            CheckOK("Minimal values", new byte[1], new byte[16], 4);
            //CheckOK("Max cost", new byte[1], new byte[16], 31);
            CheckOK("Max passcode", new byte[72], new byte[16], 4);
            CheckIllegal("Null password", null, new byte[16], 4);
            CheckIllegal("Null salt", new byte[1], null, 4);
            CheckIllegal("Salt too small", new byte[1], new byte[15], 4);
            CheckIllegal("Salt too big", new byte[1], new byte[17], 4);
            CheckIllegal("Cost too low", new byte[16], new byte[16], 3);
            CheckIllegal("Cost too high", new byte[16], new byte[16], 32);
            CheckIllegal("Passcode too long", new byte[73], new byte[16], 32);
        }

        private void CheckOK(string msg, byte[] pass, byte[] salt, int cost)
        {
            try
            {
                BCrypt.Generate(pass, salt, cost);
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Fail(msg);
            }
        }

        private void CheckIllegal(String msg, byte[] pass, byte[] salt, int cost)
        {
            try
            {
                BCrypt.Generate(pass, salt, cost);
                Fail(msg);
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        public void DoTestVectors()
        {
            foreach (object[] v in TestVectorData)
            {
                byte[] password = Hex.Decode((string)v[0]);
                byte[] salt = Hex.Decode((string)v[1]);
                int cost = (int)v[2];
                byte[] expected = Hex.Decode((string)v[3]);

                DoTest(password, salt, cost, expected);
            }

            IsTrue(AreEqual(BCrypt.Generate(BCrypt.PasswordToByteArray("12341234".ToCharArray()), Hex.Decode("01020304050607080102030405060708"), 5), Hex.Decode("cdd19088721c50e5cb49a7b743d93b5a6e67bef0f700cd78")));
            IsTrue(AreEqual(BCrypt.Generate(BCrypt.PasswordToByteArray("1234".ToCharArray()), Hex.Decode("01020304050607080102030405060708"), 5), Hex.Decode("02a3269aca2732484057b40c614204814cbfc2becd8e093e")));
        }

        private void DoTest(byte[] password, byte[] salt, int cost, byte[] expected)
        {
            byte[] hash = BCrypt.Generate(password, salt, cost);
            if (!Arrays.AreEqual(hash, expected))
            {
                Fail("Hash for " + Hex.ToHexString(password), Hex.ToHexString(expected), Hex.ToHexString(hash));
            }
        }

        public static void Main(string[] args)
		{
            RunTest(new BCryptTest());
		}

        [Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
		}
    }
}
