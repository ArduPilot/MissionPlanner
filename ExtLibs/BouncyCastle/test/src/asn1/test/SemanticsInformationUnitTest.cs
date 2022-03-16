using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class SemanticsInformationUnitTest
        : SimpleTest
    {
        public override string Name
        {
			get { return "SemanticsInformation"; }
        }

		public override void PerformTest()
        {
            DerObjectIdentifier statementId = new DerObjectIdentifier("1.1");
            SemanticsInformation mv = new SemanticsInformation(statementId);

			CheckConstruction(mv, statementId, null);

            GeneralName[] names = new GeneralName[2];

            names[0] = new GeneralName(GeneralName.Rfc822Name, "test@test.org");
            names[1] = new GeneralName(new X509Name("cn=test"));

            mv = new SemanticsInformation(statementId, names);

			CheckConstruction(mv, statementId, names);

			mv = new SemanticsInformation(names);

			CheckConstruction(mv, null, names);

			mv = SemanticsInformation.GetInstance(null);

			if (mv != null)
            {
                Fail("null GetInstance() failed.");
            }

			try
            {
                SemanticsInformation.GetInstance(new object());

                Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }

			try
            {
                new SemanticsInformation(DerSequence.Empty);

				Fail("constructor failed to detect empty sequence.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

		private void CheckConstruction(
            SemanticsInformation	mv,
            DerObjectIdentifier		semanticsIdentifier,
            GeneralName[]			names)
        {
            CheckStatement(mv, semanticsIdentifier, names);

			mv = SemanticsInformation.GetInstance(mv);

			CheckStatement(mv, semanticsIdentifier, names);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(mv.ToAsn1Object().GetEncoded());

			mv = SemanticsInformation.GetInstance(seq);

			CheckStatement(mv, semanticsIdentifier, names);
        }

		private void CheckStatement(
            SemanticsInformation si,
            DerObjectIdentifier  id,
            GeneralName[]        names)
        {
            if (id != null)
            {
                if (!si.SemanticsIdentifier.Equals(id))
                {
                    Fail("ids don't match.");
                }
            }
            else if (si.SemanticsIdentifier != null)
            {
                Fail("statementId found when none expected.");
            }

            if (names != null)
            {
                GeneralName[] siNames = si.GetNameRegistrationAuthorities();

                for (int i = 0; i != siNames.Length; i++)
                {
                    if (!names[i].Equals(siNames[i]))
                    {
                        Fail("name registration authorities don't match.");
                    }
                }
            }
            else if (si.GetNameRegistrationAuthorities() != null)
            {
                Fail("name registration authorities found when none expected.");
            }
        }

		public static void Main(
            string[] args)
        {
            RunTest(new SemanticsInformationUnitTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
