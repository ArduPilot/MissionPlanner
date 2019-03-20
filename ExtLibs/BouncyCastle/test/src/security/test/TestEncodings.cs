using System;
using System.Collections;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Security.Tests
{
    [TestFixture]
    public class TestEncodings
    {
		[Test]
		public void TestEC()
		{
			BigInteger ECParraGX = new BigInteger(Base64.Decode("D/qWPNyogWzMM7hkK+35BcPTWFc9Pyf7vTs8uaqv"));
			BigInteger ECParraGY = new BigInteger(Base64.Decode("AhQXGxb1olGRv6s1LPRfuatMF+cx3ZTGgzSE/Q5R"));
			BigInteger ECParraH = new BigInteger(Base64.Decode("AQ=="));
			BigInteger ECParraN = new BigInteger(Base64.Decode("f///////////////f///nl6an12QcfvRUiaIkJ0L"));
			BigInteger ECPubQX = new BigInteger(Base64.Decode("HWWi17Yb+Bm3PYr/DMjLOYNFhyOwX1QY7ZvqqM+l"));
			BigInteger ECPubQY = new BigInteger(Base64.Decode("JrlJfxu3WGhqwtL/55BOs/wsUeiDFsvXcGhB8DGx"));
			BigInteger ECPrivD = new BigInteger(Base64.Decode("GYQmd/NF1B+He1iMkWt3by2Az6Eu07t0ynJ4YCAo"));

            FpCurve curve = new FpCurve(
                new BigInteger("883423532389192164791648750360308885314476597252960362792450860609699839"), // q
                new BigInteger("7fffffffffffffffffffffff7fffffffffff8000000000007ffffffffffc", 16), // a
                new BigInteger("6b016c3bdcf18941d0d654921475ca71a9db2fb27d1d37796185c2942c0a", 16), // b
                ECParraN, ECParraH);

			ECDomainParameters ecDomain = new ECDomainParameters(
				curve,
                curve.ValidatePoint(ECParraGX, ECParraGY),
                ECParraN, ECParraH);

            ECPublicKeyParameters ecPub = new ECPublicKeyParameters(
                curve.ValidatePoint(ECPubQX, ECPubQY),
				ecDomain);

			ECPrivateKeyParameters ecPriv = new ECPrivateKeyParameters(ECPrivD, ecDomain);

			SubjectPublicKeyInfo subinfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(ecPub);
			PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(ecPriv);

			ECPublicKeyParameters tecPub = (ECPublicKeyParameters)PublicKeyFactory.CreateKey(subinfo);
			ECPrivateKeyParameters tecPriv = (ECPrivateKeyParameters)PrivateKeyFactory.CreateKey(privInfo);

			Assert.IsTrue(tecPub.Equals(ecPub), "EC: public key to info back to public key");
			Assert.IsTrue(tecPriv.Equals(ecPriv), "EC: private key to info back to private key");
		}

        [Test]
        public void TestDH()
        {
            BigInteger DHParraP = new BigInteger(Base64.Decode("ALJCm1CUL6mOnyVqWTSV6Z2+DVSGOvgboOhmbyyxCrym59uVnXMmPjIgQTrmniFg7PvdcN7NNFwFmcZleULso1s="));
            BigInteger DHParraQ = new BigInteger(Base64.Decode("WSFNqEoX1MdPkrUsmkr0zt8GqkMdfA3QdDM3lliFXlNz7crOuZMfGRAgnXNPELB2fe64b2aaLgLM4zK8oXZRrQ=="));
            BigInteger DHPubY = new BigInteger(Base64.Decode("AIki+8/zggCS2e488AsTNULI4LujdUeQQsZI949Dc9lKXZRmrPIC1h8NRoneHQEhpAe4Rhe0nhUOGZJekT5++SA="));
            BigInteger DHPrivX = new BigInteger(Base64.Decode("Apo67noMRO5eDWo/TtpRiBmKGw7ywh25shIu0Rs03krQmWKRbDPvdygWdJ5IpW6ZbKlCTAMhSxpz03YSeSEDmw=="));


            DHParameters dhPara = new DHParameters(DHParraP, DHParraQ);
            DHPublicKeyParameters dhPublic = new DHPublicKeyParameters(DHPubY, dhPara);
            DHPrivateKeyParameters dhPrivate = new DHPrivateKeyParameters(DHPrivX, dhPara);

            SubjectPublicKeyInfo pubInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(dhPublic);
            PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(dhPrivate);

            DHPublicKeyParameters testPubKey = (DHPublicKeyParameters) PublicKeyFactory.CreateKey(pubInfo);
            DHPrivateKeyParameters testPrivKey = (DHPrivateKeyParameters) PrivateKeyFactory.CreateKey(privInfo);

            Assert.IsFalse(!testPrivKey.Equals(dhPrivate), "DH: Private key to info back to key");
            Assert.IsFalse(!testPubKey.Equals(dhPublic), "DH: Public key to info back to key");

            Assert.IsTrue(true, "Diffe Helman Test worked.");

        }

        [Test]
        public void TestElGamal()
        {

                BigInteger ELGamalParaG = new BigInteger(Base64.Decode("QAZPRcsH8kHVKS5065R1Xy6QtsPvDkmDZtPuq18EJkvLrCIZivE/m5puQp3/VKJrG7dKgz4NBGpONp3HT+Cn/g=="));
                BigInteger ELGamalParaP = new BigInteger(Base64.Decode("AKXmAwgkudDLI/Yxk6wk3APn+mSjX5QSyDwpchmegSIi1ZNC0Jb+IbxjroKNhRTBKjtv4/JTXtJS6IqaZv9uKes="));
                BigInteger ELGamalPubY = new BigInteger(Base64.Decode("AJ/gXuZuCA2X044otNkzs8FI36XuFu1L/YHg5cEmDvICTigycRN2E1DnhP+CTqxEqgEqX8rBe5tuGDlkTLwgNqM="));
                BigInteger ELGamalPriv = new BigInteger(Base64.Decode("CqVr+K0TpuJKQnc76MjKhxrJzGr93jnuE3mTpth486Meymt8uWEVAQj1tGc9DTt14F9aV9WIT2oYYbvLJRcwow=="));



                ElGamalParameters elPara = new ElGamalParameters(ELGamalParaP, ELGamalParaG);

                ElGamalPrivateKeyParameters elPriv = new ElGamalPrivateKeyParameters(ELGamalPriv, elPara);
                ElGamalPublicKeyParameters elPub = new ElGamalPublicKeyParameters(ELGamalPubY, elPara);

                SubjectPublicKeyInfo subInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(elPub);
                PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(elPriv);

                ElGamalPrivateKeyParameters telPriv = (ElGamalPrivateKeyParameters)PrivateKeyFactory.CreateKey(privInfo);
                ElGamalPublicKeyParameters telPub = (ElGamalPublicKeyParameters)PublicKeyFactory.CreateKey(subInfo);


                             // Console.WriteLine(telPriv.Equals(elPriv));



                Assert.IsTrue(telPriv.Equals(elPriv), "ELGamal Private key to into back to private key.");
                Assert.IsTrue(telPub.Equals(elPub), "ELGamal Public key to into back to private key.");

                Assert.IsTrue(true, "ELGamal Test worked.");

            }

        [Test]
        public void TestRsa()
        {

            BigInteger rsaPubMod = new BigInteger(Base64.Decode("AIASoe2PQb1IP7bTyC9usjHP7FvnUMVpKW49iuFtrw/dMpYlsMMoIU2jupfifDpdFxIktSB4P+6Ymg5WjvHKTIrvQ7SR4zV4jaPTu56Ys0pZ9EDA6gb3HLjtU+8Bb1mfWM+yjKxcPDuFjwEtjGlPHg1Vq+CA9HNcMSKNn2+tW6qt"));
            BigInteger rsaPubExp = new BigInteger(Base64.Decode("EQ=="));
            BigInteger rsaPrivMod = new BigInteger(Base64.Decode("AIASoe2PQb1IP7bTyC9usjHP7FvnUMVpKW49iuFtrw/dMpYlsMMoIU2jupfifDpdFxIktSB4P+6Ymg5WjvHKTIrvQ7SR4zV4jaPTu56Ys0pZ9EDA6gb3HLjtU+8Bb1mfWM+yjKxcPDuFjwEtjGlPHg1Vq+CA9HNcMSKNn2+tW6qt"));
            BigInteger rsaPrivDP = new BigInteger(Base64.Decode("JXzfzG5v+HtLJIZqYMUefJfFLu8DPuJGaLD6lI3cZ0babWZ/oPGoJa5iHpX4Ul/7l3s1PFsuy1GhzCdOdlfRcQ=="));
            BigInteger rsaPrivDQ = new BigInteger(Base64.Decode("YNdJhw3cn0gBoVmMIFRZzflPDNthBiWy/dUMSRfJCxoZjSnr1gysZHK01HteV1YYNGcwPdr3j4FbOfri5c6DUQ=="));
            BigInteger rsaPrivExp = new BigInteger(Base64.Decode("DxFAOhDajr00rBjqX+7nyZ/9sHWRCCp9WEN5wCsFiWVRPtdB+NeLcou7mWXwf1Y+8xNgmmh//fPV45G2dsyBeZbXeJwB7bzx9NMEAfedchyOwjR8PYdjK3NpTLKtZlEJ6Jkh4QihrXpZMO4fKZWUm9bid3+lmiq43FwW+Hof8/E="));
            BigInteger rsaPrivP = new BigInteger(Base64.Decode("AJ9StyTVW+AL/1s7RBtFwZGFBgd3zctBqzzwKPda6LbtIFDznmwDCqAlIQH9X14X7UPLokCDhuAa76OnDXb1OiE="));
            BigInteger rsaPrivQ = new BigInteger(Base64.Decode("AM3JfD79dNJ5A3beScSzPtWxx/tSLi0QHFtkuhtSizeXdkv5FSba7lVzwEOGKHmW829bRoNxThDy4ds1IihW1w0="));
            BigInteger rsaPrivQinv = new BigInteger(Base64.Decode("Lt0g7wrsNsQxuDdB8q/rH8fSFeBXMGLtCIqfOec1j7FEIuYA/ACiRDgXkHa0WgN7nLXSjHoy630wC5Toq8vvUg=="));
            RsaKeyParameters rsaPublic = new RsaKeyParameters(false, rsaPubMod, rsaPubExp);
			RsaPrivateCrtKeyParameters rsaPrivate = new RsaPrivateCrtKeyParameters(rsaPrivMod, rsaPubExp, rsaPrivExp, rsaPrivP, rsaPrivQ, rsaPrivDP, rsaPrivDQ, rsaPrivQinv);

			SubjectPublicKeyInfo subInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(rsaPublic);
            RsaKeyParameters testResult = (RsaKeyParameters)PublicKeyFactory.CreateKey(subInfo);

			// check RSA public key.

            Assert.IsFalse(!testResult.Equals(rsaPublic), "RSA: test failed on public key to info and back to public key.");

            PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(rsaPrivate);
            testResult = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(privInfo);

            Assert.IsFalse(!testResult.Equals(rsaPrivate), "RSA: private key to info back to private key.");

            Assert.IsTrue(true, "RSATest worked.");

        }

        [Test]
        public void TestDSA()
        {
            BigInteger DSAParaG = new BigInteger(Base64.Decode("AL0fxOTq10OHFbCf8YldyGembqEu08EDVzxyLL29Zn/t4It661YNol1rnhPIs+cirw+yf9zeCe+KL1IbZ/qIMZM="));
            BigInteger DSAParaP = new BigInteger(Base64.Decode("AM2b/UeQA+ovv3dL05wlDHEKJ+qhnJBsRT5OB9WuyRC830G79y0R8wuq8jyIYWCYcTn1TeqVPWqiTv6oAoiEeOs="));
            BigInteger DSAParaQ = new BigInteger(Base64.Decode("AIlJT7mcKL6SUBMmvm24zX1EvjNx"));
            BigInteger DSAPublicY = new BigInteger(Base64.Decode("TtWy2GuT9yGBWOHi1/EpCDa/bWJCk2+yAdr56rAcqP0eHGkMnA9s9GJD2nGU8sFjNHm55swpn6JQb8q0agrCfw=="));
            BigInteger DsaPrivateX = new BigInteger(Base64.Decode("MMpBAxNlv7eYfxLTZ2BItJeD31A="));

            DsaParameters para = new DsaParameters(DSAParaP, DSAParaQ, DSAParaG);
            DsaPrivateKeyParameters dsaPriv = new DsaPrivateKeyParameters(DsaPrivateX, para);
            DsaPublicKeyParameters dsaPub = new DsaPublicKeyParameters(DSAPublicY, para);

            SubjectPublicKeyInfo subInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(dsaPub);
            DsaKeyParameters testResult = (DsaKeyParameters)PublicKeyFactory.CreateKey(subInfo);

            // check RSA public key.

            Assert.IsFalse(!testResult.Equals(dsaPub), "DSA: test failed on public key to info and back to public key.");

            PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(dsaPriv);
            testResult = (DsaPrivateKeyParameters)PrivateKeyFactory.CreateKey(privInfo);

            Assert.IsFalse(!testResult.Equals(dsaPriv), "DSA: private key to info back to private key.");

            Assert.IsTrue(true, "DSATest worked.");

        }
    }
}
