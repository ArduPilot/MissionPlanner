using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Agreement.JPake;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Agreement.Tests
{
    [TestFixture]
    public class JPakeUtilitiesTest
        : SimpleTest
    {
        private static readonly BigInteger Ten = BigInteger.ValueOf(10);

        public override void PerformTest()
        {
            TestValidateGx4();
            TestValidateGa();
            TestValidateParticipantIdsDiffer();
            TestValidateParticipantsIdsEqual();
            TestValidateMacTag();
            TestValidateNotNull();
            TestValidateZeroKnowledgeProof();
        }

        public override string Name
        {
            get { return "JPakeUtilities"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new JPakeUtilitiesTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }

        public void TestValidateGx4()
        {
            JPakeUtilities.ValidateGx4(Ten);

            try
            {
                JPakeUtilities.ValidateGx4(BigInteger.One);

                Fail("exception not thrown for g^x4 equal to 1");
            }
            catch (CryptoException)
            {
                // expected
            }
        }

        public void TestValidateGa()
        {
            JPakeUtilities.ValidateGa(Ten);

            try
            {
                JPakeUtilities.ValidateGa(BigInteger.One);

                Fail("exception not thrown for g^a equal to 1");
            }
            catch (CryptoException)
            {
                // expected
            }
        }

        public void TestValidateParticipantIdsDiffer()
        {
            JPakeUtilities.ValidateParticipantIdsDiffer("a", "b");
            JPakeUtilities.ValidateParticipantIdsDiffer("a", "A");

            try
            {
                JPakeUtilities.ValidateParticipantIdsDiffer("a", "a");

                Fail("validate participant ids differ not throwing exception for equal participant ids");
            }
            catch (CryptoException)
            {
                // expected
            }
        }

        public void TestValidateParticipantsIdsEqual()
        {
            JPakeUtilities.ValidateParticipantIdsEqual("a", "a");

            try
            {
                JPakeUtilities.ValidateParticipantIdsEqual("a", "b");

                Fail("validate participant ids equal not throwing exception for different participant ids");
            }
            catch (CryptoException)
            {
                // expected
            }
        }

        public void TestValidateMacTag()
        {
            JPakePrimeOrderGroup pg1 = JPakePrimeOrderGroups.SUN_JCE_1024;

            SecureRandom random = new SecureRandom();
            IDigest digest = new Sha256Digest();

            BigInteger x1 = JPakeUtilities.GenerateX1(pg1.Q, random);
            BigInteger x2 = JPakeUtilities.GenerateX2(pg1.Q, random);
            BigInteger x3 = JPakeUtilities.GenerateX1(pg1.Q, random);
            BigInteger x4 = JPakeUtilities.GenerateX2(pg1.Q, random);

            BigInteger gx1 = JPakeUtilities.CalculateGx(pg1.P, pg1.G, x1);
            BigInteger gx2 = JPakeUtilities.CalculateGx(pg1.P, pg1.G, x2);
            BigInteger gx3 = JPakeUtilities.CalculateGx(pg1.P, pg1.G, x3);
            BigInteger gx4 = JPakeUtilities.CalculateGx(pg1.P, pg1.G, x4);

            BigInteger gB = JPakeUtilities.CalculateGA(pg1.P, gx3, gx1, gx2);

            BigInteger s = JPakeUtilities.CalculateS("password".ToCharArray());

            BigInteger xs = JPakeUtilities.CalculateX2s(pg1.Q, x4, s);

            BigInteger B = JPakeUtilities.CalculateA(pg1.P, pg1.Q, gB, xs);

            BigInteger keyingMaterial = JPakeUtilities.CalculateKeyingMaterial(pg1.P, pg1.Q, gx4, x2, s, B);

            BigInteger macTag = JPakeUtilities.CalculateMacTag("participantId", "partnerParticipantId", gx1, gx2, gx3, gx4, keyingMaterial, digest);

            // should succeed
            JPakeUtilities.ValidateMacTag("partnerParticipantId", "participantId", gx3, gx4, gx1, gx2, keyingMaterial, digest, macTag);

            // validating own macTag (as opposed to the other party's mactag)
            try
            {
                JPakeUtilities.ValidateMacTag("participantId", "partnerParticipantId", gx1, gx2, gx3, gx4, keyingMaterial, digest, macTag);

                Fail("failed to throw exception on validating own macTag (calculated partner macTag)");
            }
            catch (CryptoException)
            {
                // expected
            }

            // participant ids switched
            try
            {
                JPakeUtilities.ValidateMacTag("participantId", "partnerParticipantId", gx3, gx4, gx1, gx2, keyingMaterial, digest, macTag);

                Fail("failed to throw exception on validating own macTag (calculated partner macTag");
            }
            catch (CryptoException)
            {
                // expected
            }
        }

        public void TestValidateNotNull()
        {
            JPakeUtilities.ValidateNotNull("a", "description");

            try
            {
                JPakeUtilities.ValidateNotNull(null, "description");

                Fail("failed to throw exception on null");
            }
            catch (ArgumentNullException)
            {
                // expected
            }
        }

        public void TestValidateZeroKnowledgeProof()
        {
            JPakePrimeOrderGroup pg1 = JPakePrimeOrderGroups.SUN_JCE_1024;

            SecureRandom random = new SecureRandom();
            IDigest digest1 = new Sha256Digest();

            BigInteger x1 = JPakeUtilities.GenerateX1(pg1.Q, random);
            BigInteger gx1 = JPakeUtilities.CalculateGx(pg1.P, pg1.G, x1);
            string participantId1 = "participant1";

            BigInteger[] zkp1 = JPakeUtilities.CalculateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, gx1, x1, participantId1, digest1, random);

            // should succeed
            JPakeUtilities.ValidateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, gx1, zkp1, participantId1, digest1);

            // wrong group
            JPakePrimeOrderGroup pg2 = JPakePrimeOrderGroups.NIST_3072;
            try
            {
                JPakeUtilities.ValidateZeroKnowledgeProof(pg2.P, pg2.Q, pg2.G, gx1, zkp1, participantId1, digest1);

                Fail("failed to throw exception on wrong prime order group");
            }
            catch (CryptoException)
            {
                // expected
            }

            // wrong digest
            IDigest digest2 = new Sha1Digest();
            try
            {
                JPakeUtilities.ValidateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, gx1, zkp1, participantId1, digest2);

                Fail("failed to throw exception on wrong digest");
            }
            catch (CryptoException)
            {
                // expected
            }

            // wrong participant
            string participantId2 = "participant2";
            try
            {
                JPakeUtilities.ValidateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, gx1, zkp1, participantId2, digest1);

                Fail("failed to throw exception on wrong participant");
            }
            catch (CryptoException)
            {
                // expected
            }

            // wrong gx
            BigInteger x2 = JPakeUtilities.GenerateX2(pg1.Q, random);
            BigInteger gx2 = JPakeUtilities.CalculateGx(pg1.P, pg1.G, x2);
            try
            {
                JPakeUtilities.ValidateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, gx2, zkp1, participantId1, digest1);

                Fail("failed to throw exception on wrong gx");
            }
            catch (CryptoException)
            {
                // expected
            }

            // wrong zkp
            BigInteger[] zkp2 = JPakeUtilities.CalculateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, gx2, x2, participantId1, digest1, random);
            try
            {
                JPakeUtilities.ValidateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, gx1, zkp2, participantId1, digest1);

                Fail("failed to throw exception on wrong zero knowledge proof");
            }
            catch (CryptoException)
            {
                // expected
            }

            // gx <= 0
            try
            {
                JPakeUtilities.ValidateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, BigInteger.Zero, zkp1, participantId1, digest1);

                Fail("failed to throw exception on g^x <= 0");
            }
            catch (CryptoException)
            {
                // expected
            }

            // gx >= p
            try
            {
                JPakeUtilities.ValidateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, pg1.P, zkp1, participantId1, digest1);

                Fail("failed to throw exception on g^x >= p");
            }
            catch (CryptoException)
            {
                // expected
            }

            // gx mod q == 1
            try
            {
                JPakeUtilities.ValidateZeroKnowledgeProof(pg1.P, pg1.Q, pg1.G, pg1.Q.Add(BigInteger.One), zkp1, participantId1, digest1);

                Fail("failed to throw exception on g^x mod q == 1");
            }
            catch (CryptoException)
            {
                // expected
            }
        }
    }
}
