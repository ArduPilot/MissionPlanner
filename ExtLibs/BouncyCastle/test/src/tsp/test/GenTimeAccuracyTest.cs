using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Tsp;

namespace Org.BouncyCastle.Tsp.Tests
{
	[TestFixture]
	public class GenTimeAccuracyUnitTest
	{
		private static readonly DerInteger ZERO_VALUE = new DerInteger(0);
		private static readonly DerInteger ONE_VALUE = new DerInteger(1);
		private static readonly DerInteger TWO_VALUE = new DerInteger(2);
		private static readonly DerInteger THREE_VALUE = new DerInteger(3);

		[Test]
		public void TestOneTwoThree()
		{
			GenTimeAccuracy accuracy = new GenTimeAccuracy(new Accuracy(ONE_VALUE, TWO_VALUE, THREE_VALUE));

			checkValues(accuracy, ONE_VALUE, TWO_VALUE, THREE_VALUE);

			checkTostring(accuracy, "1.002003");
		}

		[Test]
		public void TestThreeTwoOne()
		{
			GenTimeAccuracy accuracy = new GenTimeAccuracy(new Accuracy(THREE_VALUE, TWO_VALUE, ONE_VALUE));

			checkValues(accuracy, THREE_VALUE, TWO_VALUE, ONE_VALUE);

			checkTostring(accuracy, "3.002001");
		}

		[Test]
		public void TestTwoThreeTwo()
		{
			GenTimeAccuracy accuracy = new GenTimeAccuracy(new Accuracy(TWO_VALUE, THREE_VALUE, TWO_VALUE));

			checkValues(accuracy, TWO_VALUE, THREE_VALUE, TWO_VALUE);

			checkTostring(accuracy, "2.003002");
		}

		[Test]
		public void TestZeroTwoThree()
		{
			GenTimeAccuracy accuracy = new GenTimeAccuracy(new Accuracy(ZERO_VALUE, TWO_VALUE, THREE_VALUE));

			checkValues(accuracy, ZERO_VALUE, TWO_VALUE, THREE_VALUE);

			checkTostring(accuracy, "0.002003");
		}

		[Test]
		public void TestThreeTwoNull()
		{
			GenTimeAccuracy accuracy = new GenTimeAccuracy(new Accuracy(THREE_VALUE, TWO_VALUE, null));

			checkValues(accuracy, THREE_VALUE, TWO_VALUE, ZERO_VALUE);

			checkTostring(accuracy, "3.002000");
		}

		[Test]
		public void TestOneNullOne()
		{
			GenTimeAccuracy accuracy = new GenTimeAccuracy(new Accuracy(ONE_VALUE, null, ONE_VALUE));

			checkValues(accuracy, ONE_VALUE, ZERO_VALUE, ONE_VALUE);

			checkTostring(accuracy, "1.000001");
		}

		[Test]
		public void TestZeroNullNull()
		{
			GenTimeAccuracy accuracy = new GenTimeAccuracy(new Accuracy(ZERO_VALUE, null, null));

			checkValues(accuracy, ZERO_VALUE, ZERO_VALUE, ZERO_VALUE);

			checkTostring(accuracy, "0.000000");
		}

		[Test]
		public void TestNullNullNull()
		{
			GenTimeAccuracy accuracy = new GenTimeAccuracy(new Accuracy(null, null, null));

			checkValues(accuracy, ZERO_VALUE, ZERO_VALUE, ZERO_VALUE);

			checkTostring(accuracy, "0.000000");
		}

		private void checkValues(
			GenTimeAccuracy accuracy,
			DerInteger      secs,
			DerInteger      millis,
			DerInteger      micros)
		{
			Assert.AreEqual(secs.Value.IntValue, accuracy.Seconds);
			Assert.AreEqual(millis.Value.IntValue, accuracy.Millis);
			Assert.AreEqual(micros.Value.IntValue, accuracy.Micros);
		}

		private void checkTostring(
			GenTimeAccuracy	accuracy,
			string			expected)
		{
			Assert.AreEqual(expected, accuracy.ToString());
		}
	}
}
