using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class MonetaryValueUnitTest
        : SimpleTest
    {
        private const int TestAmount = 100;
        private const int ZeroExponent = 0;

        private const string CurrencyCode = "AUD";

        public override string Name
        {
			get { return "MonetaryValue"; }
        }

		public override void PerformTest()
        {
            MonetaryValue mv = new MonetaryValue(new Iso4217CurrencyCode(CurrencyCode), TestAmount, ZeroExponent);

			CheckValues(mv, TestAmount, ZeroExponent);

			mv = MonetaryValue.GetInstance(mv);

			CheckValues(mv, TestAmount, ZeroExponent);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(
				mv.ToAsn1Object().GetEncoded());

			mv = MonetaryValue.GetInstance(seq);

			CheckValues(mv, TestAmount, ZeroExponent);

			mv = MonetaryValue.GetInstance(null);

			if (mv != null)
            {
                Fail("null GetInstance() failed.");
            }

			try
            {
                MonetaryValue.GetInstance(new object());

				Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

		private void CheckValues(
            MonetaryValue mv,
            int           amount,
            int           exponent)
        {
            if (mv.Amount.IntValue != amount)
            {
                Fail("amounts don't match.");
            }

            if (mv.Exponent.IntValue != exponent)
            {
                Fail("exponents don't match.");
            }

            Iso4217CurrencyCode cc = mv.Currency;

            if (!cc.Alphabetic.Equals(CurrencyCode))
            {
                Fail("currency code wrong");
            }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new MonetaryValueUnitTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
