using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * standard vector test for MD5 from "Handbook of Applied Cryptography", page 345.
     */
    [TestFixture]
    public class MD5DigestTest
        : DigestTest
    {
        static string[] messages =
        {
            "",
            "a",
            "abc",
            "abcdefghijklmnopqrstuvwxyz"
        };

        static string[] digests =
        {
            "d41d8cd98f00b204e9800998ecf8427e",
            "0cc175b9c0f1b6a831c399e269772661",
            "900150983cd24fb0d6963f7d28e17f72",
            "c3fcd3d76192e4007dfb496cca67e13b"
        };

        public MD5DigestTest()
            : base(new MD5Digest(), messages, digests)
        {
        }

        protected override IDigest CloneDigest(IDigest digest)
        {
            return new MD5Digest((MD5Digest)digest);
        }

        public static void Main(
            string[] args)
        {
            RunTest(new MD5DigestTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
