using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class Iso4217CurrencyCodeUnitTest
        : SimpleTest
    {
        private const string AlphabeticCurrencyCode = "AUD";
        private const int NUMERIC_CurrencyCode = 1;

		public override string Name
        {
			get { return "Iso4217CurrencyCode"; }
        }

		public override void PerformTest()
        {
            //
            // alphabetic
            //
            Iso4217CurrencyCode cc = new Iso4217CurrencyCode(AlphabeticCurrencyCode);

            CheckNumeric(cc, AlphabeticCurrencyCode);

            cc = Iso4217CurrencyCode.GetInstance(cc);

            CheckNumeric(cc, AlphabeticCurrencyCode);

            Asn1Object obj = cc.ToAsn1Object();

            cc = Iso4217CurrencyCode.GetInstance(obj);

            CheckNumeric(cc, AlphabeticCurrencyCode);

            //
            // numeric
            //
            cc = new Iso4217CurrencyCode(NUMERIC_CurrencyCode);

            CheckNumeric(cc, NUMERIC_CurrencyCode);

            cc = Iso4217CurrencyCode.GetInstance(cc);

            CheckNumeric(cc, NUMERIC_CurrencyCode);

            obj = cc.ToAsn1Object();

            cc = Iso4217CurrencyCode.GetInstance(obj);

            CheckNumeric(cc, NUMERIC_CurrencyCode);

            cc = Iso4217CurrencyCode.GetInstance(null);

            if (cc != null)
            {
                Fail("null GetInstance() failed.");
            }

            try
            {
                Iso4217CurrencyCode.GetInstance(new object());

                Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }

            try
            {
                new Iso4217CurrencyCode("ABCD");

                Fail("constructor failed to detect out of range currencycode.");
            }
            catch (ArgumentException)
            {
                // expected
            }

            try
            {
                new Iso4217CurrencyCode(0);

                Fail("constructor failed to detect out of range small numeric code.");
            }
            catch (ArgumentException)
            {
                // expected
            }

            try
            {
                new Iso4217CurrencyCode(1000);

                Fail("constructor failed to detect out of range large numeric code.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        private void CheckNumeric(
            Iso4217CurrencyCode cc,
            string              code)
        {
            if (!cc.IsAlphabetic)
            {
                Fail("non-alphabetic code found when one expected.");
            }

            if (!cc.Alphabetic.Equals(code))
            {
                Fail("string codes don't match.");
            }
        }

        private void CheckNumeric(
            Iso4217CurrencyCode cc,
            int                 code)
        {
            if (cc.IsAlphabetic)
            {
                Fail("alphabetic code found when one not expected.");
            }

            if (cc.Numeric != code)
            {
                Fail("numeric codes don't match.");
            }
        }

        public static void Main(
            string[]    args)
        {
            RunTest(new Iso4217CurrencyCodeUnitTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
