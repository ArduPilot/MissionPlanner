using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{

    /**
     * Tiger Digest Test
     */
    [TestFixture]
    public class TigerDigestTest
        : DigestTest
    {
        readonly static string[] messages =
        {
            "",
            "abc",
            "Tiger",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvw",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ=abcdefghijklmnopqrstuvwxyz+0123456789",
            "Tiger - A Fast New Hash Function, by Ross Anderson and Eli Biham, proceedings of Fast Software Encryption 3, Cambridge, 1996.",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-"
        };

        readonly static string[] digests = {
            "3293AC630C13F0245F92BBB1766E16167A4E58492DDE73F3",
            "2AAB1484E8C158F2BFB8C5FF41B57A525129131C957B5F93",
            "DD00230799F5009FEC6DEBC838BB6A27DF2B9D6F110C7937",
            "F71C8583902AFB879EDFE610F82C0D4786A3A534504486B5",
            "38F41D9D9A710A10C3727AC0DEEAA270727D9F926EC10139",
            "48CEEB6308B87D46E95D656112CDF18D97915F9765658957",
            "631ABDD103EB9A3D245B6DFD4D77B257FC7439501D1568DD",
            "C54034E5B43EB8005848A7E0AE6AAC76E4FF590AE715FD25",
            "C54034E5B43EB8005848A7E0AE6AAC76E4FF590AE715FD25"
        };

        readonly static string hash64k = "FDF4F5B35139F48E710E421BE5AF411DE1A8AAC333F26204";

        public TigerDigestTest()
            : base(new TigerDigest(), messages, digests)
        {
        }

        public override void PerformTest()
        {
            base.PerformTest();

            sixtyFourKTest(hash64k);
        }

        protected override IDigest CloneDigest(IDigest digest)
        {
            return new TigerDigest((TigerDigest)digest);
        }

        public static void Main(
            string[] args)
        {
            RunTest(new TigerDigestTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
