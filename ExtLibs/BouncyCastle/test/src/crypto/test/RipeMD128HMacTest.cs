using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * RipeMD128 HMac Test, test vectors from RFC 2286
     */
    [TestFixture]
    public class RipeMD128HMacTest: ITest
    {
        readonly static string[] keys =
        {
            "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b",
            "4a656665",
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "0102030405060708090a0b0c0d0e0f10111213141516171819",
            "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c",
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        };

        readonly static string[] digests = {
            "fbf61f9492aa4bbf81c172e84e0734db",
            "875f828862b6b334b427c55f9f7ff09b",
            "09f0b2846d2f543da363cbec8d62a38d",
            "bdbbd7cf03e44b5aa60af815be4d2294",
            "e79808f24b25fd031c155f0d551d9a3a",
            "dc732928de98104a1f59d373c150acbb",
            "5c6bec96793e16d40690c237635f30c5"
        };

        readonly static string[] messages = {
            "Hi There",
            "what do ya want for nothing?",
            "0xdddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
            "0xcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd",
            "Test With Truncation",
            "Test Using Larger Than Block-Size Key - Hash Key First",
            "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data"
        };

		public string Name
        {
			get { return "RipeMD128HMac"; }
        }

		public ITestResult Perform()
        {
            HMac hmac = new HMac(new RipeMD128Digest());
            byte[] resBuf = new byte[hmac.GetMacSize()];

            for (int i = 0; i < messages.Length; i++)
            {
                byte[] m = Encoding.ASCII.GetBytes(messages[i]);
                if (messages[i].StartsWith("0x"))
                {
                    m = Hex.Decode(messages[i].Substring(2));
                }
                hmac.Init(new KeyParameter(Hex.Decode(keys[i])));
                hmac.BlockUpdate(m, 0, m.Length);
                hmac.DoFinal(resBuf, 0);

                if (!Arrays.AreEqual(resBuf, Hex.Decode(digests[i])))
                {
                    return new SimpleTestResult(false, Name + ": Vector " + i + " failed");
                }
            }

            return new SimpleTestResult(true, Name + ": Okay");
        }

        public static void Main(
            string[] args)
        {
            ITest test = new RipeMD128HMacTest();
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
