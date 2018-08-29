using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class MonetaryLimitUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "MonetaryLimit"; }
		}

		public override void PerformTest()
		{
			string currency = "AUD";
			int    amount = 1;
			int    exponent = 2;

			MonetaryLimit limit = new MonetaryLimit(currency, amount, exponent);

			checkConstruction(limit, currency, amount, exponent);

			limit = MonetaryLimit.GetInstance(null);

			if (limit != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				MonetaryLimit.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			MonetaryLimit	limit,
			string			currency,
			int				amount,
			int				exponent)
		{
			checkValues(limit, currency, amount, exponent);

			limit = MonetaryLimit.GetInstance(limit);

			checkValues(limit, currency, amount, exponent);

			Asn1InputStream aIn = new Asn1InputStream(limit.ToAsn1Object().GetEncoded());

			Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

			limit = MonetaryLimit.GetInstance(seq);

			checkValues(limit, currency, amount, exponent);
		}

		private void checkValues(
			MonetaryLimit	limit,
			string			currency,
			int				amount,
			int				exponent)
		{
			checkMandatoryField("currency", currency, limit.Currency);
			checkMandatoryField("amount", amount, limit.Amount.IntValue);
			checkMandatoryField("exponent", exponent, limit.Exponent.IntValue);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new MonetaryLimitUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
