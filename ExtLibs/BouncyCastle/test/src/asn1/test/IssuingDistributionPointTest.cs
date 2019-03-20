using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class IssuingDistributionPointUnitTest 
		: SimpleTest
	{
		public override string Name
		{
			get { return "IssuingDistributionPoint"; }
		}

		public override void PerformTest()
		{
			DistributionPointName name = new DistributionPointName(
				new GeneralNames(new GeneralName(new X509Name("cn=test"))));
			ReasonFlags reasonFlags = new ReasonFlags(ReasonFlags.CACompromise);

			checkPoint(6, name, true, true, reasonFlags, true, true);

			checkPoint(2, name, false, false, reasonFlags, false, false);

			checkPoint(0, null, false, false, null, false, false);

			try
			{
				IssuingDistributionPoint.GetInstance(new object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkPoint(
			int						size,
			DistributionPointName	distributionPoint,
			bool					onlyContainsUserCerts,
			bool					onlyContainsCACerts,
			ReasonFlags				onlySomeReasons,
			bool					indirectCRL,
			bool					onlyContainsAttributeCerts)
		{
			IssuingDistributionPoint point = new IssuingDistributionPoint(distributionPoint, onlyContainsUserCerts, onlyContainsCACerts, onlySomeReasons, indirectCRL, onlyContainsAttributeCerts);

			checkValues(point, distributionPoint, onlyContainsUserCerts, onlyContainsCACerts, onlySomeReasons, indirectCRL, onlyContainsAttributeCerts);

			Asn1Sequence seq = Asn1Sequence.GetInstance(Asn1Object.FromByteArray(point.GetEncoded()));

			if (seq.Count != size)
			{
				Fail("size mismatch");
			}

			point = IssuingDistributionPoint.GetInstance(seq);

			checkValues(point, distributionPoint, onlyContainsUserCerts, onlyContainsCACerts, onlySomeReasons, indirectCRL, onlyContainsAttributeCerts);
		}

		private void checkValues(
			IssuingDistributionPoint	point,
			DistributionPointName		distributionPoint,
			bool						onlyContainsUserCerts,
			bool						onlyContainsCACerts,
			ReasonFlags					onlySomeReasons,
			bool						indirectCRL,
			bool						onlyContainsAttributeCerts)
		{
			if (point.OnlyContainsUserCerts != onlyContainsUserCerts)
			{
				Fail("mismatch on onlyContainsUserCerts");
			}

			if (point.OnlyContainsCACerts != onlyContainsCACerts)
			{
				Fail("mismatch on onlyContainsCACerts");
			}

			if (point.IsIndirectCrl != indirectCRL)
			{
				Fail("mismatch on indirectCRL");
			}

			if (point.OnlyContainsAttributeCerts != onlyContainsAttributeCerts)
			{
				Fail("mismatch on onlyContainsAttributeCerts");
			}

			if (!isEquiv(onlySomeReasons, point.OnlySomeReasons))
			{
				Fail("mismatch on onlySomeReasons");
			}

			if (!isEquiv(distributionPoint, point.DistributionPoint))
			{
				Fail("mismatch on distributionPoint");
			}
		}

		private bool isEquiv(object o1, object o2)
		{
			if (o1 == null)
			{
				return o2 == null;
			}

			return o1.Equals(o2);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new IssuingDistributionPointUnitTest());
		}

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
	}
}
