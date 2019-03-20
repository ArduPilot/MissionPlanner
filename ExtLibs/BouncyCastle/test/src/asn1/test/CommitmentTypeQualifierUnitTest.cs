using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class CommitmentTypeQualifierUnitTest
        : SimpleTest
    {
        public override string Name
        {
			get { return "CommitmentTypeQualifier"; }
        }

		public override void PerformTest()
        {
            CommitmentTypeQualifier ctq = new CommitmentTypeQualifier(CommitmentTypeIdentifier.ProofOfOrigin);

            CheckConstruction(ctq, CommitmentTypeIdentifier.ProofOfOrigin, null);

            Asn1Encodable info = new DerObjectIdentifier("1.2");

            ctq = new CommitmentTypeQualifier(CommitmentTypeIdentifier.ProofOfOrigin, info);

            CheckConstruction(ctq, CommitmentTypeIdentifier.ProofOfOrigin, info);

            ctq = CommitmentTypeQualifier.GetInstance(null);

            if (ctq != null)
            {
                Fail("null GetInstance() failed.");
            }

            try
            {
                CommitmentTypeQualifier.GetInstance(new object());

                Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        private void CheckConstruction(
            CommitmentTypeQualifier mv,
            DerObjectIdentifier commitmenttTypeId,
            Asn1Encodable qualifier)
        {
            CheckStatement(mv, commitmenttTypeId, qualifier);

			mv = CommitmentTypeQualifier.GetInstance(mv);

			CheckStatement(mv, commitmenttTypeId, qualifier);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(
				mv.ToAsn1Object().GetEncoded());

			mv = CommitmentTypeQualifier.GetInstance(seq);

			CheckStatement(mv, commitmenttTypeId, qualifier);
        }

		private void CheckStatement(
            CommitmentTypeQualifier ctq,
            DerObjectIdentifier     commitmentTypeId,
            Asn1Encodable           qualifier)
        {
            if (!ctq.CommitmentTypeIdentifier.Equals(commitmentTypeId))
            {
                Fail("commitmentTypeIds don't match.");
            }

            if (qualifier != null)
            {
                if (!ctq.Qualifier.Equals(qualifier))
                {
                    Fail("qualifiers don't match.");
                }
            }
            else if (ctq.Qualifier != null)
            {
                Fail("qualifier found when none expected.");
            }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new CommitmentTypeQualifierUnitTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
