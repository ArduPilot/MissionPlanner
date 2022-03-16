using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class BiometricDataUnitTest
        : SimpleTest
    {
        public override string Name
        {
			get { return "BiometricData"; }
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
            TypeOfBiometricData dataType = new TypeOfBiometricData(TypeOfBiometricData.HandwrittenSignature);
            AlgorithmIdentifier hashAlgorithm = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance);
            Asn1OctetString     dataHash = new DerOctetString(GenerateHash());
            BiometricData       bd = new BiometricData(dataType, hashAlgorithm, dataHash);

            CheckConstruction(bd, dataType, hashAlgorithm, dataHash, null);

            DerIA5String dataUri = new DerIA5String("http://test");

            bd = new BiometricData(dataType, hashAlgorithm, dataHash, dataUri);

            CheckConstruction(bd, dataType, hashAlgorithm, dataHash, dataUri);

            bd = BiometricData.GetInstance(null);

            if (bd != null)
            {
                Fail("null GetInstance() failed.");
            }

            try
            {
                BiometricData.GetInstance(new object());

                Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        private void CheckConstruction(
            BiometricData bd,
            TypeOfBiometricData dataType,
            AlgorithmIdentifier hashAlgorithm,
            Asn1OctetString dataHash,
            DerIA5String dataUri)
        {
            CheckValues(bd, dataType, hashAlgorithm, dataHash, dataUri);

            bd = BiometricData.GetInstance(bd);

            CheckValues(bd, dataType, hashAlgorithm, dataHash, dataUri);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(bd.ToAsn1Object().GetEncoded());

			bd = BiometricData.GetInstance(seq);

			CheckValues(bd, dataType, hashAlgorithm, dataHash, dataUri);
        }

        private void CheckValues(
            BiometricData       bd,
            TypeOfBiometricData dataType,
            AlgorithmIdentifier algID,
            Asn1OctetString     dataHash,
            DerIA5String        sourceDataURI)
        {
            if (!bd.TypeOfBiometricData.Equals(dataType))
            {
                Fail("types don't match.");
            }

            if (!bd.HashAlgorithm.Equals(algID))
            {
                Fail("hash algorithms don't match.");
            }

            if (!bd.BiometricDataHash.Equals(dataHash))
            {
                Fail("hash algorithms don't match.");
            }

            if (sourceDataURI != null)
            {
                if (!bd.SourceDataUri.Equals(sourceDataURI))
                {
                    Fail("data uris don't match.");
                }
            }
            else if (bd.SourceDataUri != null)
            {
                Fail("data uri found when none expected.");
            }
        }

		public static void Main(
            string[] args)
        {
            RunTest(new BiometricDataUnitTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
