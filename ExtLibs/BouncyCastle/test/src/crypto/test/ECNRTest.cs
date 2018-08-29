using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * ECNR tests.
     */
    [TestFixture]
    public class EcNrTest
        : SimpleTest
    {
        /**
            * a basic regression test with 239 bit prime
            */
        BigInteger r = new BigInteger("308636143175167811492623515537541734843573549327605293463169625072911693");
        BigInteger s = new BigInteger("852401710738814635664888632022555967400445256405412579597015412971797143");

        byte[] kData = BigIntegers.AsUnsignedByteArray(new BigInteger("700000017569056646655505781757157107570501575775705779575555657156756655"));

        private readonly SecureRandom k;

        public EcNrTest()
        {
            k = FixedSecureRandom.From(kData);
        }

        private void ecNR239bitPrime()
        {
            BigInteger n = new BigInteger("883423532389192164791648750360308884807550341691627752275345424702807307");

            FpCurve curve = new FpCurve(
                new BigInteger("883423532389192164791648750360308885314476597252960362792450860609699839"), // q
                new BigInteger("7fffffffffffffffffffffff7fffffffffff8000000000007ffffffffffc", 16), // a
                new BigInteger("6b016c3bdcf18941d0d654921475ca71a9db2fb27d1d37796185c2942c0a", 16), // b
                n, BigInteger.One);

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.DecodePoint(Hex.Decode("020ffa963cdca8816ccc33b8642bedf905c3d358573d3f27fbbd3b3cb9aaaf")), // G
                n, BigInteger.One);

            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                new BigInteger("876300101507107567501066130761671078357010671067781776716671676178726717"), // d
                parameters);

            ECNRSigner ecnr = new ECNRSigner();
            ParametersWithRandom param = new ParametersWithRandom(priKey, k);

            ecnr.Init(true, param);

            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();
            BigInteger[] sig = ecnr.GenerateSignature(message);

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong.", r, sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong.", s, sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                curve.DecodePoint(Hex.Decode("025b6dc53bc61a2548ffb0f671472de6c9521a9d2d2534e65abfcbd5fe0c70")), // Q
                parameters);

            ecnr.Init(false, pubKey);
            if (!ecnr.VerifySignature(message, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        public override string Name
        {
            get
            {
                return "ECNR";
            }
        }

        public override void PerformTest()
        {
            ecNR239bitPrime();
        }

        public static void Main(
            string[] args)
        {
            EcNrTest test = new EcNrTest();
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
