using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * standard vector test for MD2
     * from RFC1319 by B.Kaliski of RSA Laboratories April 1992
     *
     */
    [TestFixture]
    public class MD2DigestTest
        : DigestTest
    {
        static string[] messages =
        {
            "",
            "a",
            "abc",
            "message digest",
            "abcdefghijklmnopqrstuvwxyz",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
            "12345678901234567890123456789012345678901234567890123456789012345678901234567890"
        };

        static string[] digests =
        { 
            "8350e5a3e24c153df2275c9f80692773",
            "32ec01ec4a6dac72c0ab96fb34c0b5d1",
            "da853b0d3f88d99b30283a69e6ded6bb",
            "ab4f496bfb2a530b219ff33031fe06b0",
            "4e8ddff3650292ab5a4108c3aa47940b",
            "da33def2a42df13975352846c30338cd",
            "d5976f79d83d3a0dc9806c3c66f3efd8" 
        };

        public MD2DigestTest()
            : base(new MD2Digest(), messages, digests)
        {
        }

        protected override IDigest CloneDigest(IDigest digest)
        {
            return new MD2Digest((MD2Digest)digest);
        }

        public static void Main(
            string[] args)
        {
            RunTest(new MD2DigestTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
