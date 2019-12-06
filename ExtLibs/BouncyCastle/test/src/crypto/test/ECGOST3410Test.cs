using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     *  ECGOST3410 tests are taken from GOST R 34.10-2001.
     */
    [TestFixture]
    public class ECGost3410Test
        : SimpleTest
    {
        private static readonly byte[] hashmessage = Hex.Decode("3042453136414534424341374533364339313734453431443642453241453435");

        /**
        * ECGOST3410 over the field Fp<br/>
        */
        BigInteger r = new BigInteger("29700980915817952874371204983938256990422752107994319651632687982059210933395");
        BigInteger s = new BigInteger("574973400270084654178925310019147038455227042649098563933718999175515839552");

        byte[] kData = new BigInteger("53854137677348463731403841147996619241504003434302020712960838528893196233395").ToByteArray();

        private readonly SecureRandom k;

        public ECGost3410Test()
        {
            k = FixedSecureRandom.From(kData);
        }

        private void ecGOST3410_TEST()
        {
            BigInteger mod_p = new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564821041"); //p
            BigInteger mod_q = new BigInteger("57896044618658097711785492504343953927082934583725450622380973592137631069619");

            FpCurve curve = new FpCurve(
                mod_p, // p
                new BigInteger("7"), // a
                new BigInteger("43308876546767276905765904595650931995942111794451039583252968842033849580414"), // b
                mod_q, BigInteger.One);

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("2"), // x
                    new BigInteger("4018974056539037503335449422937059775635739389905545080690979365213431566280")), // y
                mod_q, BigInteger.One);

            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                "ECGOST3410",
                new BigInteger("55441196065363246126355624130324183196576709222340016572108097750006097525544"), // d
                parameters);

            ParametersWithRandom param = new ParametersWithRandom(priKey, k);

            ECGost3410Signer ecgost3410 = new ECGost3410Signer();

            ecgost3410.Init(true, param);

            byte[] mVal = new BigInteger("20798893674476452017134061561508270130637142515379653289952617252661468872421").ToByteArray();
            byte[] message = new byte[mVal.Length];

            for (int i = 0; i != mVal.Length; i++)
            {
                message[i] = mVal[mVal.Length - 1 - i];
            }

            BigInteger[] sig = ecgost3410.GenerateSignature(message);

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
                "ECGOST3410",
                curve.CreatePoint(
                    new BigInteger("57520216126176808443631405023338071176630104906313632182896741342206604859403"), // x
                    new BigInteger("17614944419213781543809391949654080031942662045363639260709847859438286763994")), // y
                parameters);

            ecgost3410.Init(false, pubKey);
            if (!ecgost3410.VerifySignature(message, sig[0], sig[1]))
            {
                Fail("verification fails");
            }
        }

        /**
         * Test Sign and Verify with test parameters
         * see: http://www.ietf.org/internet-drafts/draft-popov-cryptopro-cpalgs-01.txt
         * gostR3410-2001-TestParamSet  P.46
         */
        private void ecGOST3410_TestParam()
        {
            SecureRandom    random = new SecureRandom();

            BigInteger mod_p = new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564821041"); //p
            BigInteger mod_q = new BigInteger("57896044618658097711785492504343953927082934583725450622380973592137631069619");

            FpCurve curve = new FpCurve(
                mod_p, // p
                new BigInteger("7"), // a
                new BigInteger("43308876546767276905765904595650931995942111794451039583252968842033849580414"), // b
                mod_q, BigInteger.One);

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("2"), // x
                    new BigInteger("4018974056539037503335449422937059775635739389905545080690979365213431566280")), // y
                mod_q, BigInteger.One);

            ECKeyPairGenerator          pGen = new ECKeyPairGenerator();
            ECKeyGenerationParameters   genParam = new ECKeyGenerationParameters(
                parameters,
                random);

            pGen.Init(genParam);

            AsymmetricCipherKeyPair  pair = pGen.GenerateKeyPair();

            ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

            ECGost3410Signer ecgost3410 = new ECGost3410Signer();

            ecgost3410.Init(true, param);

            //get hash message using the digest GOST3411.
            byte[] message = Encoding.ASCII.GetBytes("Message for sign");
            Gost3411Digest gost3411 = new Gost3411Digest();
            gost3411.BlockUpdate(message, 0, message.Length);
            byte[] hashmessage = new byte[gost3411.GetDigestSize()];
            gost3411.DoFinal(hashmessage, 0);

            BigInteger[] sig = ecgost3410.GenerateSignature(hashmessage);

            ecgost3410.Init(false, pair.Public);

            if (!ecgost3410.VerifySignature(hashmessage, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        /**
         * Test Sign and Verify with A parameters
         * see: http://www.ietf.org/internet-drafts/draft-popov-cryptopro-cpalgs-01.txt
         * gostR3410-2001-CryptoPro-A-ParamSet  P.47
         */
        public void ecGOST3410_AParam()
        {
            SecureRandom    random = new SecureRandom();

            BigInteger mod_p = new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639319"); //p
            BigInteger mod_q = new BigInteger("115792089237316195423570985008687907853073762908499243225378155805079068850323");

            FpCurve curve = new FpCurve(
                mod_p, // p
                new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639316"), // a
                new BigInteger("166"), // b
                mod_q, BigInteger.One);

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("1"), // x
                    new BigInteger("64033881142927202683649881450433473985931760268884941288852745803908878638612")), // y
                mod_q, BigInteger.One);

            ECKeyPairGenerator pGen = new ECKeyPairGenerator("ECGOST3410");
            ECKeyGenerationParameters genParam = new ECKeyGenerationParameters(
                parameters,
                random);

            pGen.Init(genParam);

            AsymmetricCipherKeyPair  pair = pGen.GenerateKeyPair();

            ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

            ECGost3410Signer ecgost3410 = new ECGost3410Signer();

            ecgost3410.Init(true, param);

            BigInteger[] sig = ecgost3410.GenerateSignature(hashmessage);

            ecgost3410.Init(false, pair.Public);

            if (!ecgost3410.VerifySignature(hashmessage, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        /**
         * Test Sign and Verify with B parameters
         * see: http://www.ietf.org/internet-drafts/draft-popov-cryptopro-cpalgs-01.txt
         * gostR3410-2001-CryptoPro-B-ParamSet  P.47-48
         */
        private void ecGOST3410_BParam()
        {
            SecureRandom random = new SecureRandom();

            BigInteger mod_p = new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564823193"); //p
            BigInteger mod_q = new BigInteger("57896044618658097711785492504343953927102133160255826820068844496087732066703");

            FpCurve curve = new FpCurve(
                mod_p, // p
                new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564823190"), // a
                new BigInteger("28091019353058090096996979000309560759124368558014865957655842872397301267595"), // b
                mod_q, BigInteger.One);

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("1"), // x
                    new BigInteger("28792665814854611296992347458380284135028636778229113005756334730996303888124")), // y
                mod_q, BigInteger.One);

            ECKeyPairGenerator pGen = new ECKeyPairGenerator("ECGOST3410");
            ECKeyGenerationParameters genParam = new ECKeyGenerationParameters(
                parameters,
                random);

            pGen.Init(genParam);

            AsymmetricCipherKeyPair pair = pGen.GenerateKeyPair();

            ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

            ECGost3410Signer ecgost3410 = new ECGost3410Signer();

            ecgost3410.Init(true, param);

            BigInteger[] sig = ecgost3410.GenerateSignature(hashmessage);

            ecgost3410.Init(false, pair.Public);

            if (!ecgost3410.VerifySignature(hashmessage, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        /**
         * Test Sign and Verify with C parameters
         * see: http://www.ietf.org/internet-drafts/draft-popov-cryptopro-cpalgs-01.txt
         * gostR3410-2001-CryptoPro-C-ParamSet  P.48
         */
        private void ecGOST3410_CParam()
        {
            SecureRandom random = new SecureRandom();

            BigInteger mod_p = new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502619"); //p
            BigInteger mod_q = new BigInteger("70390085352083305199547718019018437840920882647164081035322601458352298396601");

            FpCurve curve = new FpCurve(
                mod_p, // p
                new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502616"), // a
                new BigInteger("32858"), // b
                mod_q, BigInteger.One);

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("0"), // x
                    new BigInteger("29818893917731240733471273240314769927240550812383695689146495261604565990247")), // y
                mod_q, BigInteger.One);

            ECKeyPairGenerator pGen = new ECKeyPairGenerator("ECGOST3410");
            ECKeyGenerationParameters genParam = new ECKeyGenerationParameters(
                parameters,
                random);

            pGen.Init(genParam);

            AsymmetricCipherKeyPair pair = pGen.GenerateKeyPair();

            ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

            ECGost3410Signer ecgost3410 = new ECGost3410Signer();

            ecgost3410.Init(true, param);

            BigInteger[] sig = ecgost3410.GenerateSignature(hashmessage);

            ecgost3410.Init(false, pair.Public);

            if (!ecgost3410.VerifySignature(hashmessage, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        public override string Name
        {
            get { return "ECGOST3410"; }
        }

        public override void PerformTest()
        {
            ecGOST3410_TEST();
            ecGOST3410_TestParam();
            ecGOST3410_AParam();
            ecGOST3410_BParam();
            ecGOST3410_CParam();
        }

        public static void Main(
            string[] args)
        {
            ECGost3410Test test = new ECGost3410Test();
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
