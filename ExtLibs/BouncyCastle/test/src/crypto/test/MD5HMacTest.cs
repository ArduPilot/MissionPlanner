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
    /// <remarks> MD5 HMac Test, test vectors from RFC 2202</remarks>
    [TestFixture]
    public class MD5HMacTest
		: ITest
    {
        public string Name
        {
			get { return "MD5HMac"; }
        }

		internal static readonly string[] keys = new string[]{"0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b", "4a656665", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "0102030405060708090a0b0c0d0e0f10111213141516171819", "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"};
        internal static readonly string[] digests = new string[]{"9294727a3638bb1c13f48ef8158bfc9d", "750c783e6ab0b503eaa86e310a5db738", "56be34521d144c88dbb8c733f0e8b3f6", "697eaf0aca3a3aea3a75164746ffaa79", "56461ef2342edc00f9bab995690efd4c", "6b1ab7fe4bd7bf8f0b62e6ce61b9d0cd", "6f630fad67cda0ee1fb1f562db3aa53e"};
        internal static readonly string[] messages = new string[]{"Hi There", "what do ya want for nothing?", "0xdddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", "0xcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd", "Test With Truncation", "Test Using Larger Than Block-Size Key - Hash Key First", "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data"};

		public virtual ITestResult Perform()
        {
            HMac hmac = new HMac(new MD5Digest());
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
                    return new SimpleTestResult(false, Name + "Vector " + i + " failed");
                }
            }

			// test reset
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
            ITest test = new MD5HMacTest();
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
