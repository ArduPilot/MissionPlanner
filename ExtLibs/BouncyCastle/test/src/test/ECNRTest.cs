using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
    [TestFixture]
    public class ECNRTest
        : SimpleTest
    {
        private static readonly byte[] k1 = Hex.Decode("d5014e4b60ef2ba8b6211b4062ba3224e0427dd3");
        private static readonly byte[] k2 = Hex.Decode("345e8d05c075c3a508df729a1685690e68fcfb8c8117847e89063bca1f85d968fd281540b6e13bd1af989a1fbf17e06462bf511f9d0b140fb48ac1b1baa5bded");

        private readonly SecureRandom random = FixedSecureRandom.From(k1, k2);

        /**
         * X9.62 - 1998,<br/>
         * J.3.2, Page 155, ECDSA over the field Fp<br/>
         * an example with 239 bit prime
         */
        [Test]
        public void TestECNR239bitPrime()
        {
            BigInteger r = new BigInteger("308636143175167811492623515537541734843573549327605293463169625072911693");
            BigInteger s = new BigInteger("852401710738814635664888632022555967400445256405412579597015412971797143");

            byte[] kData = new BigInteger("700000017569056646655505781757157107570501575775705779575555657156756655").ToByteArrayUnsigned();

            SecureRandom k = FixedSecureRandom.From(kData);

            X9ECParameters p = X962NamedCurves.GetByOid(X9ObjectIdentifiers.Prime239v1);
            ECDomainParameters spec = new ECDomainParameters(p.Curve, p.G, p.N, p.H);
            ECCurve curve = spec.Curve;

            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                new BigInteger("876300101507107567501066130761671078357010671067781776716671676178726717"), // d
                spec);

            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                curve.DecodePoint(Hex.Decode("025b6dc53bc61a2548ffb0f671472de6c9521a9d2d2534e65abfcbd5fe0c70")), // Q
                spec);

            ISigner sgr = SignerUtilities.GetSigner("SHA1withECNR");
            byte[] message = new byte[] { (byte)'a', (byte)'b', (byte)'c' };

            checkSignature(239, priKey, pubKey, sgr, k, message, r, s);
        }

        // -------------------------------------------------------------------------

        /**
         * X9.62 - 1998,<br/>
         * Page 104-105, ECDSA over the field Fp<br/>
         * an example with 192 bit prime
         */
        [Test]
        public void TestECNR192bitPrime()
        {
            BigInteger r  = new BigInteger("2474388605162950674935076940284692598330235697454145648371");
            BigInteger s  = new BigInteger("2997192822503471356158280167065034437828486078932532073836");

            byte[] kData = new BigInteger("dcc5d1f1020906df2782360d36b2de7a17ece37d503784af", 16).ToByteArrayUnsigned();

            SecureRandom k = FixedSecureRandom.From(kData);

            X9ECParameters p = X962NamedCurves.GetByOid(X9ObjectIdentifiers.Prime192v1);
            ECDomainParameters spec = new ECDomainParameters(p.Curve, p.G, p.N, p.H);
            ECCurve curve = spec.Curve;

            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                new BigInteger("651056770906015076056810763456358567190100156695615665659"), // d
                spec);

            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                curve.DecodePoint(Hex.Decode("0262B12D60690CDCF330BABAB6E69763B471F994DD702D16A5")), // Q
                spec);

            ISigner sgr = SignerUtilities.GetSigner("SHA1withECNR");
            byte[] message = new byte[] { (byte)'a', (byte)'b', (byte)'c' };

            checkSignature(192, priKey, pubKey, sgr, k, message, r, s);
        }

        // -------------------------------------------------------------------------

        /**
         * SEC 2: Recommended Elliptic Curve Domain Parameters - September 2000,<br/>
         * Page 17-19, Recommended 521-bit Elliptic Curve Domain Parameters over Fp<br/>
         * an ECC example with a 521 bit prime and a 512 bit hash
         */
        [Test]
        public void TestECNR521bitPrime()
        {
            BigInteger r  = new BigInteger("1820641608112320695747745915744708800944302281118541146383656165330049339564439316345159057453301092391897040509935100825960342573871340486684575368150970954");
            BigInteger s  = new BigInteger("6358277176448326821136601602749690343031826490505780896013143436153111780706227024847359990383467115737705919410755190867632280059161174165591324242446800763");

            byte[] kData = new BigInteger("cdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", 16).ToByteArrayUnsigned();

            SecureRandom k = FixedSecureRandom.From(kData);

            X9ECParameters p = SecNamedCurves.GetByOid(SecObjectIdentifiers.SecP521r1);
            ECDomainParameters spec = new ECDomainParameters(p.Curve, p.G, p.N, p.H);
            ECCurve curve = spec.Curve;

            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                new BigInteger("5769183828869504557786041598510887460263120754767955773309066354712783118202294874205844512909370791582896372147797293913785865682804434049019366394746072023"), // d
                spec);

            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                curve.DecodePoint(Hex.Decode("02006BFDD2C9278B63C92D6624F151C9D7A822CC75BD983B17D25D74C26740380022D3D8FAF304781E416175EADF4ED6E2B47142D2454A7AC7801DD803CF44A4D1F0AC")), // Q
                spec);

            ISigner sgr = SignerUtilities.GetSigner("SHA512withECNR");
            byte[] message = new byte[] { (byte)'a', (byte)'b', (byte)'c' };

            checkSignature(521, priKey, pubKey, sgr, k, message, r, s);
        }

        private void checkSignature(
            int						size,
            ECPrivateKeyParameters	sKey,
            ECPublicKeyParameters	vKey,
            ISigner					sgr,
            SecureRandom			k,
            byte[]					message,
            BigInteger				r,
            BigInteger				s)
        {
            sgr.Init(true, new ParametersWithRandom(sKey, k));

            sgr.BlockUpdate(message, 0, message.Length);

            byte[] sigBytes = sgr.GenerateSignature();

            sgr.Init(false, vKey);

            sgr.BlockUpdate(message, 0, message.Length);

            if (!sgr.VerifySignature(sigBytes))
            {
                Fail(size + " bit EC verification failed");
            }

            BigInteger[] sig = derDecode(sigBytes);

            if (!r.Equals(sig[0]))
            {
                Fail(size + "bit"
                    + ": r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail(size + "bit"
                    + ": s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }
        }

        protected BigInteger[] derDecode(
            byte[] encoding)
        {
            Asn1Sequence s = (Asn1Sequence) Asn1Object.FromByteArray(encoding);

            return new BigInteger[]
            {
                ((DerInteger)s[0]).Value,
                ((DerInteger)s[1]).Value
            };
        }

        public override string Name
        {
            get { return "ECNR"; }
        }

        public override void PerformTest()
        {
            TestECNR192bitPrime();
            TestECNR239bitPrime();
            TestECNR521bitPrime();
        }

        public static void Main(
            string[] args)
        {
            RunTest(new ECNRTest());
        }
    }
}
