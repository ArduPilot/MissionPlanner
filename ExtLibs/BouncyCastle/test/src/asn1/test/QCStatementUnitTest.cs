using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class QCStatementUnitTest
        : SimpleTest
    {
        public override string Name
        {
			get { return "QCStatement"; }
        }

		public override void PerformTest()
        {
            QCStatement mv = new QCStatement(Rfc3739QCObjectIdentifiers.IdQcsPkixQCSyntaxV1);

            CheckConstruction(mv, Rfc3739QCObjectIdentifiers.IdQcsPkixQCSyntaxV1, null);

			Asn1Encodable info = new SemanticsInformation(new DerObjectIdentifier("1.2"));

            mv = new QCStatement(Rfc3739QCObjectIdentifiers.IdQcsPkixQCSyntaxV1, info);

            CheckConstruction(mv, Rfc3739QCObjectIdentifiers.IdQcsPkixQCSyntaxV1, info);

            mv = QCStatement.GetInstance(null);

			if (mv != null)
            {
                Fail("null GetInstance() failed.");
            }

			try
            {
                QCStatement.GetInstance(new object());

				Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

		private void CheckConstruction(
            QCStatement mv,
            DerObjectIdentifier statementId,
            Asn1Encodable statementInfo)
        {
            CheckStatement(mv, statementId, statementInfo);

			mv = QCStatement.GetInstance(mv);

			CheckStatement(mv, statementId, statementInfo);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(
				mv.ToAsn1Object().GetEncoded());

			mv = QCStatement.GetInstance(seq);

			CheckStatement(mv, statementId, statementInfo);
        }

		private void CheckStatement(
            QCStatement         qcs,
            DerObjectIdentifier statementId,
            Asn1Encodable       statementInfo)
        {
            if (!qcs.StatementId.Equals(statementId))
            {
                Fail("statementIds don't match.");
            }

			if (statementInfo != null)
            {
                if (!qcs.StatementInfo.Equals(statementInfo))
                {
                    Fail("statementInfos don't match.");
                }
            }
            else if (qcs.StatementInfo != null)
            {
                Fail("statementInfo found when none expected.");
            }
        }

		public static void Main(
            string[] args)
        {
            RunTest(new QCStatementUnitTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
