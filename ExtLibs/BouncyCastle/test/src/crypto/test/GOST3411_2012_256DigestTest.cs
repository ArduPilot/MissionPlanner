using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tests;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Crypto.Tests
{
    [TestFixture]
    public class GOST3411_2012_256DigestTest
        : DigestTest
    {
        private static readonly String[] messages;

        private static char[] M1 =
            {
            (char)0x30, (char)0x31, (char)0x32, (char)0x33, (char)0x34, (char)0x35,
            (char)0x36, (char)0x37, (char)0x38, (char)0x39, (char)0x30, (char)0x31,
            (char)0x32, (char)0x33, (char)0x34, (char)0x35, (char)0x36, (char)0x37,
            (char)0x38, (char)0x39,(char)0x30, (char)0x31, (char)0x32, (char)0x33,
            (char)0x34, (char)0x35, (char)0x36, (char)0x37, (char)0x38, (char)0x39,
            (char)0x30, (char)0x31, (char)0x32, (char)0x33, (char)0x34, (char)0x35,
            (char)0x36, (char)0x37, (char)0x38, (char)0x39, (char)0x30, (char)0x31,
            (char)0x32, (char)0x33, (char)0x34, (char)0x35, (char)0x36, (char)0x37,
            (char)0x38, (char)0x39, (char)0x30, (char)0x31, (char)0x32, (char)0x33,
            (char)0x34, (char)0x35, (char)0x36, (char)0x37, (char)0x38, (char)0x39,
            (char)0x30, (char)0x31, (char)0x32
        };

        private static char[] M2 =
            {
            (char)0xd1,(char)0xe5,(char)0x20,(char)0xe2,(char)0xe5,(char)0xf2,
            (char)0xf0,(char)0xe8,(char)0x2c,(char)0x20,(char)0xd1,(char)0xf2,
            (char)0xf0,(char)0xe8,(char)0xe1,(char)0xee,(char)0xe6,(char)0xe8,
            (char)0x20,(char)0xe2,(char)0xed,(char)0xf3,(char)0xf6,(char)0xe8,
            (char)0x2c,(char)0x20,(char)0xe2,(char)0xe5,(char)0xfe,(char)0xf2,
            (char)0xfa,(char)0x20,(char)0xf1,(char)0x20,(char)0xec,(char)0xee,
            (char)0xf0,(char)0xff,(char)0x20,(char)0xf1,(char)0xf2,(char)0xf0,
            (char)0xe5,(char)0xeb,(char)0xe0,(char)0xec,(char)0xe8,(char)0x20,
            (char)0xed,(char)0xe0,(char)0x20,(char)0xf5,(char)0xf0,(char)0xe0,
            (char)0xe1,(char)0xf0,(char)0xfb,(char)0xff,(char)0x20,(char)0xef,
            (char)0xeb,(char)0xfa,(char)0xea,(char)0xfb,(char)0x20,(char)0xc8,
            (char)0xe3,(char)0xee,(char)0xf0,(char)0xe5,(char)0xe2,(char)0xfb
        };

        static GOST3411_2012_256DigestTest()
        {
            messages = new string[] { new string(M1), new string(M2) };
        }

        private static readonly String[] digests = {
            "9d151eefd8590b89daa6ba6cb74af9275dd051026bb149a452fd84e5e57b5500",
            "9dd2fe4e90409e5da87f53976d7405b0c0cac628fc669a741d50063c557e8f50"
        };

        public GOST3411_2012_256DigestTest()
            : base(new GOST3411_2012_256Digest(), messages, digests)
        {
        }

        public override void PerformTest()
        {
			base.PerformTest();

            HMac gMac = new HMac(new GOST3411_2012_256Digest());

            gMac.Init(new KeyParameter(Hex.Decode("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f")));

            byte[] data = Hex.Decode("0126bdb87800af214341456563780100");

            gMac.BlockUpdate(data, 0, data.Length);
            byte[] mac = new byte[gMac.GetMacSize()];

			gMac.DoFinal(mac, 0);

			if (!Arrays.AreEqual(Hex.Decode("a1aa5f7de402d7b3d323f2991c8d4534013137010a83754fd0af6d7cd4922ed9"), mac))
			{
				Fail("mac calculation failed.");
			}
        }

        protected override IDigest CloneDigest(IDigest digest)
        {
			return new GOST3411_2012_256Digest((GOST3411_2012_256Digest)digest);
        }

        [Test]
        public void GOST3422_2012_256_TestFunction()
        {
            string resultText = Perform().ToString();
  
            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
