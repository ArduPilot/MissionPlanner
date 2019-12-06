using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * standard vector test for SHA-256 from FIPS Draft 180-2.
     *
     * Note, the first two vectors are _not_ from the draft, the last three are.
     */
    [TestFixture]
    public class Sha256DigestTest
        : DigestTest
    {
        private static string[] messages =
        {
            "",
            "a",
            "abc",
            "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq"
        };

        private static string[] digests =
        {
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            "ca978112ca1bbdcafac231b39a23dc4da786eff8147c4e72b9807785afee48bb",
            "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad",
            "248d6a61d20638b8e5c026930c3e6039a33ce45964ff2167f6ecedd419db06c1"
        };

        // 1 million 'a'
        static private string  million_a_digest = "cdc76e5c9914fb9281a1c7e284d73e67f1809a48a497200e046d39ccc7112cd0";

        public Sha256DigestTest()
            : base(new Sha256Digest(), messages, digests)
        {
        }

        public override void PerformTest()
        {
            base.PerformTest();

            millionATest(million_a_digest);
        }

        protected override IDigest CloneDigest(IDigest digest)
        {
            return new Sha256Digest((Sha256Digest)digest);
        }

        public static void Main(
            string[] args)
        {
            RunTest(new Sha256DigestTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
