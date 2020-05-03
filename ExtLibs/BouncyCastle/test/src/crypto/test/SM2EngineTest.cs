using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    [TestFixture]
    public class SM2EngineTest
        : SimpleTest
    {
        public override string Name
        {
            get { return "SM2Engine"; }
        }

        private void DoEngineTestFp()
        {
            BigInteger SM2_ECC_P = new BigInteger("8542D69E4C044F18E8B92435BF6FF7DE457283915C45517D722EDB8B08F1DFC3", 16);
            BigInteger SM2_ECC_A = new BigInteger("787968B4FA32C3FD2417842E73BBFEFF2F3C848B6831D7E0EC65228B3937E498", 16);
            BigInteger SM2_ECC_B = new BigInteger("63E4C6D3B23B0C849CF84241484BFE48F61D59A5B16BA06E6E12D1DA27C5249A", 16);
            BigInteger SM2_ECC_N = new BigInteger("8542D69E4C044F18E8B92435BF6FF7DD297720630485628D5AE74EE7C32E79B7", 16);
            BigInteger SM2_ECC_H = BigInteger.One;
            BigInteger SM2_ECC_GX = new BigInteger("421DEBD61B62EAB6746434EBC3CC315E32220B3BADD50BDC4C4E6C147FEDD43D", 16);
            BigInteger SM2_ECC_GY = new BigInteger("0680512BCBB42C07D47349D2153B70C4E5D7FDFCBFA36EA1A85841B9E46E09A2", 16);

            ECCurve curve = new FpCurve(SM2_ECC_P, SM2_ECC_A, SM2_ECC_B, SM2_ECC_N, SM2_ECC_H);

            ECPoint g = curve.CreatePoint(SM2_ECC_GX, SM2_ECC_GY);
            ECDomainParameters domainParams = new ECDomainParameters(curve, g, SM2_ECC_N);

            ECKeyPairGenerator keyPairGenerator = new ECKeyPairGenerator();

            ECKeyGenerationParameters aKeyGenParams = new ECKeyGenerationParameters(domainParams, new TestRandomBigInteger("1649AB77A00637BD5E2EFE283FBF353534AA7F7CB89463F208DDBC2920BB0DA0", 16));

            keyPairGenerator.Init(aKeyGenParams);

            AsymmetricCipherKeyPair aKp = keyPairGenerator.GenerateKeyPair();

            ECPublicKeyParameters aPub = (ECPublicKeyParameters)aKp.Public;
            ECPrivateKeyParameters aPriv = (ECPrivateKeyParameters)aKp.Private;

            SM2Engine sm2Engine = new SM2Engine();

            byte[] m = Strings.ToByteArray("encryption standard");

            sm2Engine.Init(true, new ParametersWithRandom(aPub, new TestRandomBigInteger("4C62EEFD6ECFC2B95B92FD6C3D9575148AFA17425546D49018E5388D49DD7B4F", 16)));

            byte[] enc = sm2Engine.ProcessBlock(m, 0, m.Length);

            IsTrue("enc wrong", Arrays.AreEqual(Hex.Decode(
                "04245C26 FB68B1DD DDB12C4B 6BF9F2B6 D5FE60A3 83B0D18D 1C4144AB F17F6252" +
                "E776CB92 64C2A7E8 8E52B199 03FDC473 78F605E3 6811F5C0 7423A24B 84400F01" +
                "B8650053 A89B41C4 18B0C3AA D00D886C 00286467 9C3D7360 C30156FA B7C80A02" +
                "76712DA9 D8094A63 4B766D3A 285E0748 0653426D"), enc));

            sm2Engine.Init(false, aPriv);

            byte[] dec = sm2Engine.ProcessBlock(enc, 0, enc.Length);

            IsTrue("dec wrong", Arrays.AreEqual(m, dec));

            enc[80] = (byte)(enc[80] + 1);

            try
            {
                sm2Engine.ProcessBlock(enc, 0, enc.Length);
                Fail("no exception");
            }
            catch (InvalidCipherTextException e)
            {
                IsTrue("wrong exception", "invalid cipher text".Equals(e.Message));
            }

            // long message
            sm2Engine = new SM2Engine();

            m = new byte[4097];
            for (int i = 0; i != m.Length; i++)
            {
                m[i] = (byte)i;
            }

            sm2Engine.Init(true, new ParametersWithRandom(aPub, new TestRandomBigInteger("4C62EEFD6ECFC2B95B92FD6C3D9575148AFA17425546D49018E5388D49DD7B4F", 16)));

            enc = sm2Engine.ProcessBlock(m, 0, m.Length);

            sm2Engine.Init(false, aPriv);

            dec = sm2Engine.ProcessBlock(enc, 0, enc.Length);

            IsTrue("dec wrong", Arrays.AreEqual(m, dec));
        }

        private void DoEngineTestF2m()
        {
            BigInteger SM2_ECC_A = new BigInteger("00", 16);
            BigInteger SM2_ECC_B = new BigInteger("E78BCD09746C202378A7E72B12BCE00266B9627ECB0B5A25367AD1AD4CC6242B", 16);
            BigInteger SM2_ECC_N = new BigInteger("7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBC972CF7E6B6F900945B3C6A0CF6161D", 16);
            BigInteger SM2_ECC_H = BigInteger.ValueOf(4);
            BigInteger SM2_ECC_GX = new BigInteger("00CDB9CA7F1E6B0441F658343F4B10297C0EF9B6491082400A62E7A7485735FADD", 16);
            BigInteger SM2_ECC_GY = new BigInteger("013DE74DA65951C4D76DC89220D5F7777A611B1C38BAE260B175951DC8060C2B3E", 16);

            ECCurve curve = new F2mCurve(257, 12, SM2_ECC_A, SM2_ECC_B, SM2_ECC_N, SM2_ECC_H);

            ECPoint g = curve.CreatePoint(SM2_ECC_GX, SM2_ECC_GY);
            ECDomainParameters domainParams = new ECDomainParameters(curve, g, SM2_ECC_N, SM2_ECC_H);

            ECKeyPairGenerator keyPairGenerator = new ECKeyPairGenerator();

            ECKeyGenerationParameters aKeyGenParams = new ECKeyGenerationParameters(domainParams, new TestRandomBigInteger("56A270D17377AA9A367CFA82E46FA5267713A9B91101D0777B07FCE018C757EB", 16));

            keyPairGenerator.Init(aKeyGenParams);

            AsymmetricCipherKeyPair aKp = keyPairGenerator.GenerateKeyPair();

            ECPublicKeyParameters aPub = (ECPublicKeyParameters)aKp.Public;
            ECPrivateKeyParameters aPriv = (ECPrivateKeyParameters)aKp.Private;

            SM2Engine sm2Engine = new SM2Engine();

            byte[] m = Strings.ToByteArray("encryption standard");

            sm2Engine.Init(true, new ParametersWithRandom(aPub, new TestRandomBigInteger("6D3B497153E3E92524E5C122682DBDC8705062E20B917A5F8FCDB8EE4C66663D", 16)));

            byte[] enc = sm2Engine.ProcessBlock(m, 0, m.Length);

            IsTrue("f2m enc wrong", Arrays.AreEqual(Hex.Decode(
                "04019D23 6DDB3050 09AD52C5 1BB93270 9BD534D4 76FBB7B0 DF9542A8 A4D890A3" +
                    "F2E100B2 3B938DC0 A94D1DF8 F42CF45D 2D6601BF 638C3D7D E75A29F0 2AFB7E45" +
                    "E91771FD 55AC6213 C2A8A040 E4CAB5B2 6A9CFCDA 737373A4 8625D375 8FA37B3E" +
                    "AB80E9CF CABA665E 3199EA15 A1FA8189 D96F5791 25E4"), enc));

            sm2Engine.Init(false, aPriv);

            byte[] dec = sm2Engine.ProcessBlock(enc, 0, enc.Length);

            IsTrue("f2m dec wrong", Arrays.AreEqual(m, dec));
        }

        public override void PerformTest()
        {
            DoEngineTestFp();
            DoEngineTestF2m();
        }

        public static void Main(string[] args)
        {
            RunTest(new SM2EngineTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
