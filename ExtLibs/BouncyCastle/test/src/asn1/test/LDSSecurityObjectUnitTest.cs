using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Icao;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class LDSSecurityObjectUnitTest
        : SimpleTest
    {
        public override string Name
        {
			get { return "LDSSecurityObject"; }
        }

		private byte[] GenerateHash()
        {
            Random rand = new Random();
            byte[] bytes = new byte[20];
            rand.NextBytes(bytes);
            return bytes;
        }

        public override void PerformTest()
        {
            AlgorithmIdentifier  algoId = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1);
            DataGroupHash[] datas = new DataGroupHash[2];

            datas[0] = new DataGroupHash(1, new DerOctetString(GenerateHash()));
            datas[1] = new DataGroupHash(2, new DerOctetString(GenerateHash()));

            LdsSecurityObject so = new LdsSecurityObject(algoId, datas);

			CheckConstruction(so, algoId, datas);

			LdsVersionInfo versionInfo = new LdsVersionInfo("Hello", "world");

			so = new LdsSecurityObject(algoId, datas, versionInfo);

			CheckConstruction(so, algoId, datas, versionInfo);

			try
			{
				LdsSecurityObject.GetInstance(null);
			}
			catch (Exception)
			{
				Fail("GetInstance() failed to handle null.");
			}

			try
            {
                LdsSecurityObject.GetInstance(new object());

                Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }

			try
            {
				LdsSecurityObject.GetInstance(DerSequence.Empty);

				Fail("constructor failed to detect empty sequence.");
            }
            catch (ArgumentException)
            {
                // expected
            }

			try
            {
                new LdsSecurityObject(algoId, new DataGroupHash[1]);

				Fail("constructor failed to detect small DataGroupHash array.");
            }
            catch (ArgumentException)
            {
                // expected
            }

			try
            {
                new LdsSecurityObject(algoId, new DataGroupHash[LdsSecurityObject.UBDataGroups + 1]);

				Fail("constructor failed to out of bounds DataGroupHash array.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

		private void CheckConstruction(
            LdsSecurityObject	so,
            AlgorithmIdentifier	digestAlgorithmIdentifier,
            DataGroupHash[]		datagroupHash)
        {
            CheckStatement(so, digestAlgorithmIdentifier, datagroupHash, null);

			so = LdsSecurityObject.GetInstance(so);

			CheckStatement(so, digestAlgorithmIdentifier, datagroupHash, null);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(
				so.ToAsn1Object().GetEncoded());

			so = LdsSecurityObject.GetInstance(seq);

			CheckStatement(so, digestAlgorithmIdentifier, datagroupHash, null);
        }
		
		private void CheckConstruction(
			LdsSecurityObject	so,
			AlgorithmIdentifier	digestAlgorithmIdentifier,
			DataGroupHash[]		datagroupHash,
			LdsVersionInfo		versionInfo)
		{
			if (!so.Version.Equals(BigInteger.One))
			{
				Fail("version number not 1");
			}

			CheckStatement(so, digestAlgorithmIdentifier, datagroupHash, versionInfo);

			so = LdsSecurityObject.GetInstance(so);

			CheckStatement(so, digestAlgorithmIdentifier, datagroupHash, versionInfo);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(
				so.ToAsn1Object().GetEncoded());

			so = LdsSecurityObject.GetInstance(seq);

			CheckStatement(so, digestAlgorithmIdentifier, datagroupHash, versionInfo);
		}

		private void CheckStatement(
            LdsSecurityObject	so,
            AlgorithmIdentifier	digestAlgorithmIdentifier,
            DataGroupHash[]		datagroupHash,
			LdsVersionInfo		versionInfo)
        {
            if (digestAlgorithmIdentifier != null)
            {
                if (!so.DigestAlgorithmIdentifier.Equals(digestAlgorithmIdentifier))
                {
                    Fail("ids don't match.");
                }
            }
            else if (so.DigestAlgorithmIdentifier != null)
            {
                Fail("digest algorithm Id found when none expected.");
            }

			if (datagroupHash != null)
            {
                DataGroupHash[] datas = so.GetDatagroupHash();

                for (int i = 0; i != datas.Length; i++)
                {
                    if (!datagroupHash[i].Equals(datas[i]))
                    {
                        Fail("name registration authorities don't match.");
                    }
                }
            }
            else if (so.GetDatagroupHash() != null)
            {
                Fail("data hash groups found when none expected.");
            }

			if (versionInfo != null)
			{
				if (!versionInfo.Equals(so.VersionInfo))
				{
					Fail("versionInfo doesn't match");
				}
			}
			else if (so.VersionInfo != null)
			{
				Fail("version info found when none expected.");
			}
        }

		public static void Main(
            string[]    args)
        {
            RunTest(new LDSSecurityObjectUnitTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
