using System;

using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	/**
	* X.690 test example
	*/
	[TestFixture]
	public class UtcTimeTest
		: SimpleTest
	{
		private static readonly string[] input =
		{
			"020122122220Z",
			"020122122220-1000",
			"020122122220+1000",
			"020122122220+00",
			"0201221222Z",
			"0201221222-1000",
			"0201221222+1000",
			"0201221222+00",
			"550122122220Z",
			"5501221222Z"
		};

		private static readonly string[] output =
		{
			"20020122122220GMT+00:00",
			"20020122122220GMT-10:00",
			"20020122122220GMT+10:00",
			"20020122122220GMT+00:00",
			"20020122122200GMT+00:00",
			"20020122122200GMT-10:00",
			"20020122122200GMT+10:00",
			"20020122122200GMT+00:00",
			"19550122122220GMT+00:00",
			"19550122122200GMT+00:00"
		};

		private static readonly string[] zOutput1 =
		{
			"20020122122220Z",
			"20020122222220Z",
			"20020122022220Z",
			"20020122122220Z",
			"20020122122200Z",
			"20020122222200Z",
			"20020122022200Z",
			"20020122122200Z",
			"19550122122220Z",
			"19550122122200Z"
		};

		private static readonly string[] zOutput2 =
		{
			"20020122122220Z",
			"20020122222220Z",
			"20020122022220Z",
			"20020122122220Z",
			"20020122122200Z",
			"20020122222200Z",
			"20020122022200Z",
			"20020122122200Z",
			"19550122122220Z",
			"19550122122200Z"
		};

		public override string Name
		{
			get { return "UTCTime"; }
		}

		public override void PerformTest()
		{
//			SimpleDateFormat yyyyF = new SimpleDateFormat("yyyyMMddHHmmss'Z'");
//			SimpleDateFormat yyF = new SimpleDateFormat("yyyyMMddHHmmss'Z'");

//			yyyyF.setTimeZone(new SimpleTimeZone(0,"Z"));
//			yyF.setTimeZone(new SimpleTimeZone(0,"Z"));

			for (int i = 0; i != input.Length; i++)
			{
				DerUtcTime t = new DerUtcTime(input[i]);

				if (!t.AdjustedTimeString.Equals(output[i]))
				{
					Fail("failed conversion test " + i);
				}

//				if (!yyyyF.format(t.getAdjustedDate()).Equals(zOutput1[i]))
				if (!t.ToAdjustedDateTime().ToString(@"yyyyMMddHHmmss\Z").Equals(zOutput1[i]))
				{
					Fail("failed date conversion test " + i);
				}

//				if (!yyF.format(t.getDate()).Equals(zOutput2[i]))
				if (!t.ToDateTime().ToString(@"yyyyMMddHHmmss\Z").Equals(zOutput2[i]))
				{
					Fail("failed date shortened conversion test " + i);
				}
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new UtcTimeTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
