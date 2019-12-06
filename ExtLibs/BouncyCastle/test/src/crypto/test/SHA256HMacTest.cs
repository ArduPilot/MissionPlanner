using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /// <summary> SHA256 HMac Test, test vectors from RFC</summary>
    [TestFixture]
    public class Sha256HMacTest
		: ITest
    {
        public string Name
        {
			get { return "SHA256HMac"; }
        }

		public static readonly string[] keys =
		{
            "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b",
            "4a656665",
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "0102030405060708090a0b0c0d0e0f10111213141516171819",
            "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c",
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        };

		public static readonly string[] digests =
		{
            "b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7",
            "5bdcc146bf60754e6a042426089575c75a003f089d2739839dec58b964ec3843",
            "773ea91e36800e46854db8ebd09181a72959098b3ef8c122d9635514ced565fe",
            "82558a389a443c0ea4cc819899f2083a85f0faa3e578f8077a2e3ff46729665b",
            "a3b6167473100ee06e0c796c2955552bfa6f7c0a6a8aef8b93f860aab0cd20c5",
            "60e431591ee0b67f0d8a26aacbf5b77f8e0bc6213728c5140546040f0ee37f54",
            "9b09ffa71b942fcb27635fbcd5b0e944bfdc63644f0713938a7f51535c3a35e2"
        };

		public static readonly string[] messages =
		{
            "Hi There",
            "what do ya want for nothing?",
            "0xdddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
            "0xcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd",
            "Test With Truncation",
            "Test Using Larger Than Block-Size Key - Hash Key First",
            "This is a test using a larger than block-size key and a larger than block-size data. The key needs to be hashed before being used by the HMAC algorithm."
        };

		public virtual ITestResult Perform()
        {
            HMac hmac = new HMac(new Sha256Digest());
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

            //
            // test reset
            //
            int vector = 0; // vector used for test
            byte[] m2 = Encoding.ASCII.GetBytes(messages[vector]);
            if (messages[vector].StartsWith("0x"))
            {
                m2 = Hex.Decode(messages[vector].Substring(2));
            }
            hmac.Init(new KeyParameter(Hex.Decode(keys[vector])));
            hmac.BlockUpdate(m2, 0, m2.Length);
            hmac.DoFinal(resBuf, 0);
            hmac.Reset();
            hmac.BlockUpdate(m2, 0, m2.Length);
            hmac.DoFinal(resBuf, 0);

            if (!Arrays.AreEqual(resBuf, Hex.Decode(digests[vector])))
            {
                return new SimpleTestResult(false, Name + "Reset with vector " + vector + " failed");
            }

            return new SimpleTestResult(true, Name + ": Okay");
        }

        public static void Main(
            string[] args)
        {
            ITest test = new Sha256HMacTest();
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
