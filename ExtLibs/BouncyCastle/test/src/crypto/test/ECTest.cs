using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * ECDSA tests are taken from X9.62.
     */
    [TestFixture]
    public class ECTest
        : SimpleTest
    {
        /**
        * X9.62 - 1998,<br/>
        * J.3.1, Page 152, ECDSA over the field Fp<br/>
        * an example with 192 bit prime
        */
        [Test]
        public void TestECDsa192bitPrime()
        {
            BigInteger r = new BigInteger("3342403536405981729393488334694600415596881826869351677613");
            BigInteger s = new BigInteger("5735822328888155254683894997897571951568553642892029982342");

            byte[] kData = BigIntegers.AsUnsignedByteArray(new BigInteger("6140507067065001063065065565667405560006161556565665656654"));

            SecureRandom k = FixedSecureRandom.From(kData);

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
                "ECDSA",
                new BigInteger("651056770906015076056810763456358567190100156695615665659"), // d
                parameters);

            ParametersWithRandom param = new ParametersWithRandom(priKey, k);

            ECDsaSigner ecdsa = new ECDsaSigner();

            ecdsa.Init(true, param);

            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();
            BigInteger[] sig = ecdsa.GenerateSignature(message);

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                "ECDSA",
                curve.DecodePoint(Hex.Decode("0262b12d60690cdcf330babab6e69763b471f994dd702d16a5")), // Q
                parameters);

            ecdsa.Init(false, pubKey);
            if (!ecdsa.VerifySignature(message, sig[0], sig[1]))
            {
                Fail("verification fails");
            }
        }

        [Test]
        public void TestDecode()
        {
            X9ECParameters x9 = ECNamedCurveTable.GetByName("prime192v1");
            ECPoint p = x9.G;

            if (!p.AffineXCoord.ToBigInteger().Equals(new BigInteger("188da80eb03090f67cbf20eb43a18800f4ff0afd82ff1012", 16)))
            {
                Fail("x uncompressed incorrectly");
            }

            if (!p.AffineYCoord.ToBigInteger().Equals(new BigInteger("7192b95ffc8da78631011ed6b24cdd573f977a11e794811", 16)))
            {
                Fail("y uncompressed incorrectly");
            }

            byte[] encoding = p.GetEncoded(true);

            if (!AreEqual(encoding, Hex.Decode("03188da80eb03090f67cbf20eb43a18800f4ff0afd82ff1012")))
            {
                Fail("point compressed incorrectly");
            }
        }

        /**
         * X9.62 - 1998,<br/>
         * J.3.2, Page 155, ECDSA over the field Fp<br/>
         * an example with 239 bit prime
         */
        [Test]
        public void TestECDsa239bitPrime()
        {
            BigInteger r = new BigInteger("308636143175167811492622547300668018854959378758531778147462058306432176");
            BigInteger s = new BigInteger("323813553209797357708078776831250505931891051755007842781978505179448783");

            byte[] kData = BigIntegers.AsUnsignedByteArray(new BigInteger("700000017569056646655505781757157107570501575775705779575555657156756655"));

            SecureRandom k = FixedSecureRandom.From(kData);

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
                "ECDSA",
                new BigInteger("876300101507107567501066130761671078357010671067781776716671676178726717"), // d
                parameters);

            ECDsaSigner ecdsa = new ECDsaSigner();
            ParametersWithRandom param = new ParametersWithRandom(priKey, k);

            ecdsa.Init(true, param);

            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();
            BigInteger[] sig = ecdsa.GenerateSignature(message);

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                "ECDSA",
                curve.DecodePoint(Hex.Decode("025b6dc53bc61a2548ffb0f671472de6c9521a9d2d2534e65abfcbd5fe0c70")), // Q
                parameters);

            ecdsa.Init(false, pubKey);
            if (!ecdsa.VerifySignature(message, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }


        /**
         * X9.62 - 1998,<br/>
         * J.2.1, Page 100, ECDSA over the field F2m<br/>
         * an example with 191 bit binary field
         */
        [Test]
        public void TestECDsa191bitBinary()
        {
            BigInteger r = new BigInteger("87194383164871543355722284926904419997237591535066528048");
            BigInteger s = new BigInteger("308992691965804947361541664549085895292153777025772063598");

            byte[] kData = BigIntegers.AsUnsignedByteArray(new BigInteger("1542725565216523985789236956265265265235675811949404040041"));

            SecureRandom k = FixedSecureRandom.From(kData);

            F2mCurve curve = new F2mCurve(
                191, // m
                9, //k
                new BigInteger("2866537B676752636A68F56554E12640276B649EF7526267", 16), // a
                new BigInteger("2E45EF571F00786F67B0081B9495A3D95462F5DE0AA185EC", 16)); // b

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.DecodePoint(Hex.Decode("0436B3DAF8A23206F9C4F299D7B21A9C369137F2C84AE1AA0D765BE73433B3F95E332932E70EA245CA2418EA0EF98018FB")), // G
                new BigInteger("1569275433846670190958947355803350458831205595451630533029"), // n
                BigInteger.Two); // h

            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                "ECDSA",
                new BigInteger("1275552191113212300012030439187146164646146646466749494799"), // d
                parameters);

            ECDsaSigner ecdsa = new ECDsaSigner();
            ParametersWithRandom param = new ParametersWithRandom(priKey, k);

            ecdsa.Init(true, param);

            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();
            BigInteger[] sig = ecdsa.GenerateSignature(message);

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                "ECDSA",
                curve.DecodePoint(Hex.Decode("045DE37E756BD55D72E3768CB396FFEB962614DEA4CE28A2E755C0E0E02F5FB132CAF416EF85B229BBB8E1352003125BA1")), // Q
                parameters);

            ecdsa.Init(false, pubKey);
            if (!ecdsa.VerifySignature(message, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        /**
         * X9.62 - 1998,<br/>
         * J.2.1, Page 100, ECDSA over the field F2m<br/>
         * an example with 191 bit binary field
         */
        [Test]
        public void TestECDsa239bitBinary()
        {
            BigInteger r = new BigInteger("21596333210419611985018340039034612628818151486841789642455876922391552");
            BigInteger s = new BigInteger("197030374000731686738334997654997227052849804072198819102649413465737174");

            byte[] kData = BigIntegers.AsUnsignedByteArray(new BigInteger("171278725565216523967285789236956265265265235675811949404040041670216363"));

            SecureRandom k = FixedSecureRandom.From(kData);

            F2mCurve curve = new F2mCurve(
                239, // m
                36, //k
                new BigInteger("32010857077C5431123A46B808906756F543423E8D27877578125778AC76", 16), // a
                new BigInteger("790408F2EEDAF392B012EDEFB3392F30F4327C0CA3F31FC383C422AA8C16", 16)); // b

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.DecodePoint(Hex.Decode("0457927098FA932E7C0A96D3FD5B706EF7E5F5C156E16B7E7C86038552E91D61D8EE5077C33FECF6F1A16B268DE469C3C7744EA9A971649FC7A9616305")), // G
                new BigInteger("220855883097298041197912187592864814557886993776713230936715041207411783"), // n
                BigInteger.ValueOf(4)); // h

            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                "ECDSA",
                new BigInteger("145642755521911534651321230007534120304391871461646461466464667494947990"), // d
                parameters);

            ECDsaSigner ecdsa = new ECDsaSigner();
            ParametersWithRandom param = new ParametersWithRandom(priKey, k);

            ecdsa.Init(true, param);

            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();
            BigInteger[] sig = ecdsa.GenerateSignature(message);

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                "ECDSA",
                curve.DecodePoint(Hex.Decode("045894609CCECF9A92533F630DE713A958E96C97CCB8F5ABB5A688A238DEED6DC2D9D0C94EBFB7D526BA6A61764175B99CB6011E2047F9F067293F57F5")), // Q
                parameters);

            ecdsa.Init(false, pubKey);
            if (!ecdsa.VerifySignature(message, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        // L 4.1  X9.62 2005
        [Test]
        public void TestECDsaP224Sha224()
        {
            X9ECParameters p = NistNamedCurves.GetByName("P-224");
            ECDomainParameters parameters = new ECDomainParameters(p.Curve, p.G, p.N, p.H);
            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                new BigInteger("6081831502424510080126737029209236539191290354021104541805484120491"), // d
                parameters);
            SecureRandom k = FixedSecureRandom.From(BigIntegers.AsUnsignedByteArray(new BigInteger("15456715103636396133226117016818339719732885723579037388121116732601")));
        
            byte[] M = Hex.Decode("8797A3C693CC292441039A4E6BAB7387F3B4F2A63D00ED384B378C79");

            ECDsaSigner dsa = new ECDsaSigner();

            dsa.Init(true, new ParametersWithRandom(priKey, k));

            BigInteger[] sig = dsa.GenerateSignature(M);

            BigInteger r = new BigInteger("26477406756127720855365980332052585411804331993436302005017227573742");
            BigInteger s = new BigInteger("17694958233103667059888193972742186995283044672015112738919822429978");
        
            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                parameters.Curve.DecodePoint(Hex.Decode("03FD44EC11F9D43D9D23B1E1D1C9ED6519B40ECF0C79F48CF476CC43F1")), // Q
                parameters);

            dsa.Init(false, pubKey);
            if (!dsa.VerifySignature(M, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        [Test]
        public void TestECDsaSecP224k1Sha256()
        {
            X9ECParameters p = SecNamedCurves.GetByName("secp224k1");
            ECDomainParameters parameters = new ECDomainParameters(p.Curve, p.G, p.N, p.H);
            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                new BigInteger("BE6F6E91FE96840A6518B56F3FE21689903A64FA729057AB872A9F51", 16), // d
                parameters);
            SecureRandom k = FixedSecureRandom.From(Hex.Decode("00c39beac93db21c3266084429eb9b846b787c094f23a4de66447efbb3"));
       
            byte[] M = Hex.Decode("E5D5A7ADF73C5476FAEE93A2C76CE94DC0557DB04CDC189504779117920B896D");

            ECDsaSigner dsa = new ECDsaSigner();

            dsa.Init(true, new ParametersWithRandom(priKey, k));

            BigInteger[] sig = dsa.GenerateSignature(M);

            BigInteger r = new BigInteger("8163E5941BED41DA441B33E653C632A55A110893133351E20CE7CB75", 16);
            BigInteger s = new BigInteger("D12C3FC289DDD5F6890DCE26B65792C8C50E68BF551D617D47DF15A8", 16);

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                parameters.Curve.DecodePoint(Hex.Decode("04C5C9B38D3603FCCD6994CBB9594E152B658721E483669BB42728520F484B537647EC816E58A8284D3B89DFEDB173AFDC214ECA95A836FA7C")), // Q
                parameters);

            dsa.Init(false, pubKey);
            if (!dsa.VerifySignature(M, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        // L4.2  X9.62 2005
        [Test]
        public void TestECDsaP256Sha256()
        {
            X9ECParameters p = NistNamedCurves.GetByName("P-256");
            ECDomainParameters parameters = new ECDomainParameters(p.Curve, p.G, p.N, p.H);
            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                new BigInteger("20186677036482506117540275567393538695075300175221296989956723148347484984008"), // d
                parameters);
            SecureRandom k = FixedSecureRandom.From(BigIntegers.AsUnsignedByteArray(new BigInteger("72546832179840998877302529996971396893172522460793442785601695562409154906335")));

            byte[] M = Hex.Decode("1BD4ED430B0F384B4E8D458EFF1A8A553286D7AC21CB2F6806172EF5F94A06AD");

            ECDsaSigner dsa = new ECDsaSigner();

            dsa.Init(true, new ParametersWithRandom(priKey, k));

            BigInteger[] sig = dsa.GenerateSignature(M);

            BigInteger r = new BigInteger("97354732615802252173078420023658453040116611318111190383344590814578738210384");
            BigInteger s = new BigInteger("98506158880355671805367324764306888225238061309262649376965428126566081727535");

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                parameters.Curve.DecodePoint(Hex.Decode("03596375E6CE57E0F20294FC46BDFCFD19A39F8161B58695B3EC5B3D16427C274D")), // Q
                parameters);

            dsa.Init(false, pubKey);
            if (!dsa.VerifySignature(M, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        [Test]
        public void TestECDsaP224OneByteOver()
        {
            X9ECParameters p = NistNamedCurves.GetByName("P-224");
            ECDomainParameters parameters = new ECDomainParameters(p.Curve, p.G, p.N, p.H);
            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                new BigInteger("6081831502424510080126737029209236539191290354021104541805484120491"), // d
                parameters);
            SecureRandom    k = FixedSecureRandom.From(BigIntegers.AsUnsignedByteArray(new BigInteger("15456715103636396133226117016818339719732885723579037388121116732601")));

            byte[] M = Hex.Decode("8797A3C693CC292441039A4E6BAB7387F3B4F2A63D00ED384B378C79FF");

            ECDsaSigner dsa = new ECDsaSigner();

            dsa.Init(true, new ParametersWithRandom(priKey, k));

            BigInteger[] sig = dsa.GenerateSignature(M);

            BigInteger r = new BigInteger("26477406756127720855365980332052585411804331993436302005017227573742");
            BigInteger s = new BigInteger("17694958233103667059888193972742186995283044672015112738919822429978");

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                parameters.Curve.DecodePoint(Hex.Decode("03FD44EC11F9D43D9D23B1E1D1C9ED6519B40ECF0C79F48CF476CC43F1")), // Q
                parameters);

            dsa.Init(false, pubKey);
            if (!dsa.VerifySignature(M, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        // L4.3  X9.62 2005
        [Test]
        public void TestECDsaP521Sha512()
        {
            X9ECParameters p = NistNamedCurves.GetByName("P-521");
            ECDomainParameters parameters = new ECDomainParameters(p.Curve, p.G, p.N, p.H);
            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                new BigInteger("617573726813476282316253885608633222275541026607493641741273231656161177732180358888434629562647985511298272498852936680947729040673640492310550142822667389"), // d
                parameters);
            SecureRandom k = FixedSecureRandom.From(BigIntegers.AsUnsignedByteArray(new BigInteger("6806532878215503520845109818432174847616958675335397773700324097584974639728725689481598054743894544060040710846048585856076812050552869216017728862957612913")));

            byte[] M = Hex.Decode("6893B64BD3A9615C39C3E62DDD269C2BAAF1D85915526083183CE14C2E883B48B193607C1ED871852C9DF9C3147B574DC1526C55DE1FE263A676346A20028A66");

            ECDsaSigner dsa = new ECDsaSigner();

            dsa.Init(true, new ParametersWithRandom(priKey, k));

            BigInteger[] sig = dsa.GenerateSignature(M);

            BigInteger r = new BigInteger("1368926195812127407956140744722257403535864168182534321188553460365652865686040549247096155740756318290773648848859639978618869784291633651685766829574104630");
            BigInteger s = new BigInteger("1624754720348883715608122151214003032398685415003935734485445999065609979304811509538477657407457976246218976767156629169821116579317401249024208611945405790");

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                parameters.Curve.DecodePoint(Hex.Decode("020145E221AB9F71C5FE740D8D2B94939A09E2816E2167A7D058125A06A80C014F553E8D6764B048FB6F2B687CEC72F39738F223D4CE6AFCBFF2E34774AA5D3C342CB3")), // Q
                parameters);

            dsa.Init(false, pubKey);
            if (!dsa.VerifySignature(M, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        /**
         * General test for long digest.
         */
        [Test]
        public void TestECDsa239bitBinaryAndLargeDigest()
        {
            BigInteger r = new BigInteger("21596333210419611985018340039034612628818151486841789642455876922391552");
            BigInteger s = new BigInteger("144940322424411242416373536877786566515839911620497068645600824084578597");

            byte[] kData = BigIntegers.AsUnsignedByteArray(
                new BigInteger("171278725565216523967285789236956265265265235675811949404040041670216363"));

            SecureRandom k = FixedSecureRandom.From(kData);

            F2mCurve curve = new F2mCurve(
                239, // m
                36, //k
                new BigInteger("32010857077C5431123A46B808906756F543423E8D27877578125778AC76", 16), // a
                new BigInteger("790408F2EEDAF392B012EDEFB3392F30F4327C0CA3F31FC383C422AA8C16", 16)); // b

            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.DecodePoint(
                    Hex.Decode("0457927098FA932E7C0A96D3FD5B706EF7E5F5C156E16B7E7C86038552E91D61D8EE5077C33FECF6F1A16B268DE469C3C7744EA9A971649FC7A9616305")), // G
                new BigInteger("220855883097298041197912187592864814557886993776713230936715041207411783"), // n
                BigInteger.ValueOf(4)); // h

            ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
                "ECDSA",
                new BigInteger("145642755521911534651321230007534120304391871461646461466464667494947990"), // d
                parameters);

            ECDsaSigner ecdsa = new ECDsaSigner();
            ParametersWithRandom param = new ParametersWithRandom(priKey, k);

            ecdsa.Init(true, param);

            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517968236873715988614170569073515315707566766479517968236873715988614170569073515315707566766479517").ToByteArray();
            BigInteger[] sig = ecdsa.GenerateSignature(message);

            if (!r.Equals(sig[0]))
            {
                Fail("r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail("s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }

            // Verify the signature
            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
                "ECDSA",
                curve.DecodePoint(
                    Hex.Decode("045894609CCECF9A92533F630DE713A958E96C97CCB8F5ABB5A688A238DEED6DC2D9D0C94EBFB7D526BA6A61764175B99CB6011E2047F9F067293F57F5")), // Q
                parameters);

            ecdsa.Init(false, pubKey);
            if (!ecdsa.VerifySignature(message, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        /**
         * key generation test
         */
        [Test]
        public void TestECDsaKeyGenTest()
        {
            SecureRandom random = new SecureRandom();

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

            ECKeyPairGenerator pGen = new ECKeyPairGenerator();
            ECKeyGenerationParameters genParam = new ECKeyGenerationParameters(
                parameters,
                random);

            pGen.Init(genParam);

            AsymmetricCipherKeyPair pair = pGen.GenerateKeyPair();

            ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

            ECDsaSigner ecdsa = new ECDsaSigner();

            ecdsa.Init(true, param);

            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();
            BigInteger[] sig = ecdsa.GenerateSignature(message);

            ecdsa.Init(false, pair.Public);

            if (!ecdsa.VerifySignature(message, sig[0], sig[1]))
            {
                Fail("signature fails");
            }
        }

        /**
         * Basic Key Agreement Test
         */
        [Test]
        public void TestECDHBasicAgreement()
        {
            SecureRandom random = new SecureRandom();

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

            ECKeyPairGenerator pGen = new ECKeyPairGenerator();
            ECKeyGenerationParameters genParam = new ECKeyGenerationParameters(parameters, random);

            pGen.Init(genParam);

            AsymmetricCipherKeyPair p1 = pGen.GenerateKeyPair();
            AsymmetricCipherKeyPair p2 = pGen.GenerateKeyPair();

            //
            // two way
            //
            IBasicAgreement e1 = new ECDHBasicAgreement();
            IBasicAgreement e2 = new ECDHBasicAgreement();

            e1.Init(p1.Private);
            e2.Init(p2.Private);

            BigInteger   k1 = e1.CalculateAgreement(p2.Public);
            BigInteger   k2 = e2.CalculateAgreement(p1.Public);

            if (!k1.Equals(k2))
            {
                Fail("calculated agreement test failed");
            }

            //
            // two way
            //
            e1 = new ECDHCBasicAgreement();
            e2 = new ECDHCBasicAgreement();

            e1.Init(p1.Private);
            e2.Init(p2.Private);

            k1 = e1.CalculateAgreement(p2.Public);
            k2 = e2.CalculateAgreement(p1.Public);

            if (!k1.Equals(k2))
            {
                Fail("calculated agreement test failed");
            }
        }

        [Test]
        public void TestECDHBasicAgreementCofactor()
        {
            SecureRandom random = new SecureRandom();

            X9ECParameters x9 = CustomNamedCurves.GetByName("curve25519");
            ECDomainParameters ec = new ECDomainParameters(x9.Curve, x9.G, x9.N, x9.H, x9.GetSeed());

            ECKeyPairGenerator kpg = new ECKeyPairGenerator();
            kpg.Init(new ECKeyGenerationParameters(ec, random));

            AsymmetricCipherKeyPair p1 = kpg.GenerateKeyPair();
            AsymmetricCipherKeyPair p2 = kpg.GenerateKeyPair();

            IBasicAgreement e1 = new ECDHBasicAgreement();
            IBasicAgreement e2 = new ECDHBasicAgreement();

            e1.Init(p1.Private);
            e2.Init(p2.Private);

            BigInteger k1 = e1.CalculateAgreement(p2.Public);
            BigInteger k2 = e2.CalculateAgreement(p1.Public);

            if (!k1.Equals(k2))
            {
                Fail("calculated agreement test failed");
            }
        }

        [Test]
        public void TestECMqvTestVector1()
        {
            // Test Vector from GEC-2

            X9ECParameters x9 = SecNamedCurves.GetByName("secp160r1");
            ECDomainParameters p = new ECDomainParameters(
                x9.Curve, x9.G, x9.N, x9.H, x9.GetSeed());

            AsymmetricCipherKeyPair U1 = new AsymmetricCipherKeyPair(
                new ECPublicKeyParameters(
                    p.Curve.DecodePoint(Hex.Decode("0251B4496FECC406ED0E75A24A3C03206251419DC0")), p),
                new ECPrivateKeyParameters(
                    new BigInteger("AA374FFC3CE144E6B073307972CB6D57B2A4E982", 16), p));

            AsymmetricCipherKeyPair U2 = new AsymmetricCipherKeyPair(
                new ECPublicKeyParameters(
                    p.Curve.DecodePoint(Hex.Decode("03D99CE4D8BF52FA20BD21A962C6556B0F71F4CA1F")), p),
                new ECPrivateKeyParameters(
                    new BigInteger("149EC7EA3A220A887619B3F9E5B4CA51C7D1779C", 16), p));

            AsymmetricCipherKeyPair V1 = new AsymmetricCipherKeyPair(
                new ECPublicKeyParameters(
                    p.Curve.DecodePoint(Hex.Decode("0349B41E0E9C0369C2328739D90F63D56707C6E5BC")), p),
                new ECPrivateKeyParameters(
                    new BigInteger("45FB58A92A17AD4B15101C66E74F277E2B460866", 16), p));

            AsymmetricCipherKeyPair V2 = new AsymmetricCipherKeyPair(
                new ECPublicKeyParameters(
                    p.Curve.DecodePoint(Hex.Decode("02706E5D6E1F640C6E9C804E75DBC14521B1E5F3B5")), p),
                new ECPrivateKeyParameters(
                    new BigInteger("18C13FCED9EADF884F7C595C8CB565DEFD0CB41E", 16), p));

            BigInteger x = calculateAgreement(U1, U2, V1, V2);

            if (x == null
                || !x.Equals(new BigInteger("5A6955CEFDB4E43255FB7FCF718611E4DF8E05AC", 16)))
            {
                Fail("MQV Test Vector #1 agreement failed");
            }
        }

        [Test]
        public void TestECMqvTestVector2()
        {
            // Test Vector from GEC-2

            X9ECParameters x9 = SecNamedCurves.GetByName("sect163k1");
            ECDomainParameters p = new ECDomainParameters(
                x9.Curve, x9.G, x9.N, x9.H, x9.GetSeed());

            AsymmetricCipherKeyPair U1 = new AsymmetricCipherKeyPair(
                new ECPublicKeyParameters(
                    p.Curve.DecodePoint(Hex.Decode("03037D529FA37E42195F10111127FFB2BB38644806BC")), p),
                new ECPrivateKeyParameters(
                    new BigInteger("03A41434AA99C2EF40C8495B2ED9739CB2155A1E0D", 16), p));

            AsymmetricCipherKeyPair U2 = new AsymmetricCipherKeyPair(
                new ECPublicKeyParameters(
                    p.Curve.DecodePoint(Hex.Decode("02015198E74BC2F1E5C9A62B80248DF0D62B9ADF8429")), p),
                new ECPrivateKeyParameters(
                    new BigInteger("032FC4C61A8211E6A7C4B8B0C03CF35F7CF20DBD52", 16), p));

            AsymmetricCipherKeyPair V1 = new AsymmetricCipherKeyPair(
                new ECPublicKeyParameters(
                    p.Curve.DecodePoint(Hex.Decode("03072783FAAB9549002B4F13140B88132D1C75B3886C")), p),
                new ECPrivateKeyParameters(
                    new BigInteger("57E8A78E842BF4ACD5C315AA0569DB1703541D96", 16), p));

            AsymmetricCipherKeyPair V2 = new AsymmetricCipherKeyPair(
                new ECPublicKeyParameters(
                    p.Curve.DecodePoint(Hex.Decode("03067E3AEA3510D69E8EDD19CB2A703DDC6CF5E56E32")), p),
                new ECPrivateKeyParameters(
                    new BigInteger("02BD198B83A667A8D908EA1E6F90FD5C6D695DE94F", 16), p));

            BigInteger x = calculateAgreement(U1, U2, V1, V2);

            if (x == null
                || !x.Equals(new BigInteger("038359FFD30C0D5FC1E6154F483B73D43E5CF2B503", 16)))
            {
                Fail("MQV Test Vector #2 agreement failed");
            }
        }

        [Test]
        public void TestECMqvRandom()
        {
            SecureRandom random = new SecureRandom();

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

            ECKeyPairGenerator pGen = new ECKeyPairGenerator();

            pGen.Init(new ECKeyGenerationParameters(parameters, random));


            // Pre-established key pairs
            AsymmetricCipherKeyPair U1 = pGen.GenerateKeyPair();
            AsymmetricCipherKeyPair V1 = pGen.GenerateKeyPair();

            // Ephemeral key pairs
            AsymmetricCipherKeyPair U2 = pGen.GenerateKeyPair();
            AsymmetricCipherKeyPair V2 = pGen.GenerateKeyPair();

            BigInteger x = calculateAgreement(U1, U2, V1, V2);

            if (x == null)
            {
                Fail("MQV Test Vector (random) agreement failed");
            }
        }

        private static BigInteger calculateAgreement(
            AsymmetricCipherKeyPair U1,
            AsymmetricCipherKeyPair U2,
            AsymmetricCipherKeyPair V1,
            AsymmetricCipherKeyPair V2)
        {
            ECMqvBasicAgreement u = new ECMqvBasicAgreement();
            u.Init(new MqvPrivateParameters(
                (ECPrivateKeyParameters)U1.Private,
                (ECPrivateKeyParameters)U2.Private,
                (ECPublicKeyParameters)U2.Public));
            BigInteger ux = u.CalculateAgreement(new MqvPublicParameters(
                (ECPublicKeyParameters)V1.Public,
                (ECPublicKeyParameters)V2.Public));

            ECMqvBasicAgreement v = new ECMqvBasicAgreement();
            v.Init(new MqvPrivateParameters(
                (ECPrivateKeyParameters)V1.Private,
                (ECPrivateKeyParameters)V2.Private,
                (ECPublicKeyParameters)V2.Public));
            BigInteger vx = v.CalculateAgreement(new MqvPublicParameters(
                (ECPublicKeyParameters)U1.Public,
                (ECPublicKeyParameters)U2.Public));

            if (ux.Equals(vx))
                return ux;

            return null;
        }

        public override string Name
        {
            get { return "EC"; }
        }

        public override void PerformTest()
        {
            TestDecode();
            TestECDsa192bitPrime();
            TestECDsa239bitPrime();
            TestECDsa191bitBinary();
            TestECDsa239bitBinary();
            TestECDsaKeyGenTest();
            TestECDHBasicAgreement();
            TestECDHBasicAgreementCofactor();

            TestECDsaP224Sha224();
            TestECDsaP224OneByteOver();
            TestECDsaP256Sha256();
            TestECDsaP521Sha512();
            TestECDsaSecP224k1Sha256();
            TestECDsa239bitBinaryAndLargeDigest();

            TestECMqvTestVector1();
            TestECMqvTestVector2();
            TestECMqvRandom();
        }

        public static void Main(
            string[] args)
        {
            RunTest(new ECTest());
        }
    }
}
