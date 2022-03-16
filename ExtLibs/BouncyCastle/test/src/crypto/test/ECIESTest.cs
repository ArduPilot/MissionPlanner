using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /// <remarks>Test for ECIES - Elliptic Curve Integrated Encryption Scheme</remarks>
    [TestFixture]
    public class EcIesTest
        : SimpleTest
    {
        public EcIesTest()
        {
        }

        public override string Name
        {
            get { return "ECIES"; }
        }

        private void StaticTest()
        {
            BigInteger n = new BigInteger("6277101735386680763835789423176059013767194773182842284081");

            FpCurve curve = new FpCurve(
                new BigInteger("6277101735386680763835789423207666416083908700390324961279"), // q
                new BigInteger("fffffffffffffffffffffffffffffffefffffffffffffffc", 16), // a
                new BigInteger("64210519e59c80e70fa7e9ab72243049feb8deecc146b9b1", 16), // b
                n, BigInteger.One);

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.DecodePoint(Hex.Decode("03188da80eb03090f67cbf20eb43a18800f4ff0afd82ff1012")), // G
                n, BigInteger.One);

            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                "ECDH",
                new BigInteger("651056770906015076056810763456358567190100156695615665659"), // d
                parameters);

            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                "ECDH",
                curve.DecodePoint(Hex.Decode("0262b12d60690cdcf330babab6e69763b471f994dd702d16a5")), // Q
                parameters);

            AsymmetricCipherKeyPair p1 = new AsymmetricCipherKeyPair(pubKey, priKey);
            AsymmetricCipherKeyPair p2 = new AsymmetricCipherKeyPair(pubKey, priKey);

            //
            // stream test
            //
            IesEngine i1 = new IesEngine(
                new ECDHBasicAgreement(),
                new Kdf2BytesGenerator(new Sha1Digest()),
                new HMac(new Sha1Digest()));
            IesEngine i2 = new IesEngine(
                new ECDHBasicAgreement(),
                new Kdf2BytesGenerator(new Sha1Digest()),
                new HMac(new Sha1Digest()));
            byte[] d = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] e = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            IesParameters p = new IesParameters(d, e, 64);

            i1.Init(true, p1.Private, p2.Public, p);
            i2.Init(false, p2.Private, p1.Public, p);

            byte[] message = Hex.Decode("1234567890abcdef");

            byte[] out1 = i1.ProcessBlock(message, 0, message.Length);

            if (!AreEqual(out1, Hex.Decode("468d89877e8238802403ec4cb6b329faeccfa6f3a730f2cdb3c0a8e8")))
            {
                Fail("stream cipher test failed on enc");
            }

            byte[] out2 = i2.ProcessBlock(out1, 0, out1.Length);

            if (!AreEqual(out2, message))
            {
                Fail("stream cipher test failed");
            }

            //
            // twofish with CBC
            //
            BufferedBlockCipher c1 = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(new TwofishEngine()));
            BufferedBlockCipher c2 = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(new TwofishEngine()));
            i1 = new IesEngine(
                new ECDHBasicAgreement(),
                new Kdf2BytesGenerator(new Sha1Digest()),
                new HMac(new Sha1Digest()),
                c1);
            i2 = new IesEngine(
                new ECDHBasicAgreement(),
                new Kdf2BytesGenerator(new Sha1Digest()),
                new HMac(new Sha1Digest()),
                c2);
            d = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            e = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            p = new IesWithCipherParameters(d, e, 64, 128);

            i1.Init(true, p1.Private, p2.Public, p);
            i2.Init(false, p2.Private, p1.Public, p);

            message = Hex.Decode("1234567890abcdef");

            out1 = i1.ProcessBlock(message, 0, message.Length);

            if (!AreEqual(out1, Hex.Decode("b8a06ea5c2b9df28b58a0a90a734cde8c9c02903e5c220021fe4417410d1e53a32a71696")))
            {
                Fail("twofish cipher test failed on enc");
            }

            out2 = i2.ProcessBlock(out1, 0, out1.Length);

            if (!AreEqual(out2, message))
            {
                Fail("twofish cipher test failed");
            }
        }

        private void DoTest(
            AsymmetricCipherKeyPair	p1,
            AsymmetricCipherKeyPair	p2)
        {
            //
            // stream test
            //
            IesEngine i1 = new IesEngine(
                new ECDHBasicAgreement(),
                new Kdf2BytesGenerator(new Sha1Digest()),
                new HMac(new Sha1Digest()));
            IesEngine i2 = new IesEngine(
                new ECDHBasicAgreement(),
                new Kdf2BytesGenerator(new Sha1Digest()),
                new HMac(new Sha1Digest()));
            byte[] d = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] e = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            IesParameters  p = new IesParameters(d, e, 64);

            i1.Init(true, p1.Private, p2.Public, p);
            i2.Init(false, p2.Private, p1.Public, p);

            byte[] message = Hex.Decode("1234567890abcdef");

            byte[] out1 = i1.ProcessBlock(message, 0, message.Length);

            byte[] out2 = i2.ProcessBlock(out1, 0, out1.Length);

            if (!AreEqual(out2, message))
            {
                Fail("stream cipher test failed");
            }

            //
            // twofish with CBC
            //
            BufferedBlockCipher c1 = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(new TwofishEngine()));
            BufferedBlockCipher c2 = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(new TwofishEngine()));
            i1 = new IesEngine(
                new ECDHBasicAgreement(),
                new Kdf2BytesGenerator(new Sha1Digest()),
                new HMac(new Sha1Digest()),
                c1);
            i2 = new IesEngine(
                new ECDHBasicAgreement(),
                new Kdf2BytesGenerator(new Sha1Digest()),
                new HMac(new Sha1Digest()),
                c2);
            d = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            e = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            p = new IesWithCipherParameters(d, e, 64, 128);

            i1.Init(true, p1.Private, p2.Public, p);
            i2.Init(false, p2.Private, p1.Public, p);

            message = Hex.Decode("1234567890abcdef");

            out1 = i1.ProcessBlock(message, 0, message.Length);

            out2 = i2.ProcessBlock(out1, 0, out1.Length);

            if (!AreEqual(out2, message))
            {
                Fail("twofish cipher test failed");
            }
        }

        public override void PerformTest()
        {
            StaticTest();

            BigInteger n = new BigInteger("6277101735386680763835789423176059013767194773182842284081");

            FpCurve curve = new FpCurve(
                new BigInteger("6277101735386680763835789423207666416083908700390324961279"), // q
                new BigInteger("fffffffffffffffffffffffffffffffefffffffffffffffc", 16), // a
                new BigInteger("64210519e59c80e70fa7e9ab72243049feb8deecc146b9b1", 16), // b
                n, BigInteger.One);

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.DecodePoint(Hex.Decode("03188da80eb03090f67cbf20eb43a18800f4ff0afd82ff1012")), // G
                n, BigInteger.One);

            ECKeyPairGenerator eGen = new ECKeyPairGenerator();
            KeyGenerationParameters gParam = new ECKeyGenerationParameters(parameters, new SecureRandom());

            eGen.Init(gParam);

            AsymmetricCipherKeyPair p1 = eGen.GenerateKeyPair();
            AsymmetricCipherKeyPair p2 = eGen.GenerateKeyPair();

            DoTest(p1, p2);
        }

        public static void Main(
            string[] args)
        {
            RunTest(new EcIesTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
