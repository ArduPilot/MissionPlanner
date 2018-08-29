using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class CommitmentTypeIndicationUnitTest
        : SimpleTest
    {
        public override string Name
        {
			get { return "CommitmentTypeIndication"; }
        }

		public override void PerformTest()
        {
            CommitmentTypeIndication cti = new CommitmentTypeIndication(CommitmentTypeIdentifier.ProofOfOrigin);

            CheckConstruction(cti, CommitmentTypeIdentifier.ProofOfOrigin, null);

            Asn1Sequence qualifier = new DerSequence(new DerObjectIdentifier("1.2"));

            cti = new CommitmentTypeIndication(CommitmentTypeIdentifier.ProofOfOrigin, qualifier);

            CheckConstruction(cti, CommitmentTypeIdentifier.ProofOfOrigin, qualifier);

            cti = CommitmentTypeIndication.GetInstance(null);

            if (cti != null)
            {
                Fail("null GetInstance() failed.");
            }

            try
            {
                CommitmentTypeIndication.GetInstance(new object());

                Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        private void CheckConstruction(
            CommitmentTypeIndication mv,
            DerObjectIdentifier commitmenttTypeId,
            Asn1Encodable qualifier)
        {
            CheckStatement(mv, commitmenttTypeId, qualifier);

			mv = CommitmentTypeIndication.GetInstance(mv);

			CheckStatement(mv, commitmenttTypeId, qualifier);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(
				mv.ToAsn1Object().GetEncoded());

			mv = CommitmentTypeIndication.GetInstance(seq);

			CheckStatement(mv, commitmenttTypeId, qualifier);
        }

		private void CheckStatement(
            CommitmentTypeIndication cti,
            DerObjectIdentifier     commitmentTypeId,
            Asn1Encodable           qualifier)
        {
            if (!cti.CommitmentTypeID.Equals(commitmentTypeId))
            {
                Fail("commitmentTypeIds don't match.");
            }

            if (qualifier != null)
            {
                if (!cti.CommitmentTypeQualifier.Equals(qualifier))
                {
                    Fail("qualifiers don't match.");
                }
            }
            else if (cti.CommitmentTypeQualifier != null)
            {
                Fail("qualifier found when none expected.");
            }
        }

        public static void Main(
            string[]    args)
        {
            RunTest(new CommitmentTypeIndicationUnitTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
