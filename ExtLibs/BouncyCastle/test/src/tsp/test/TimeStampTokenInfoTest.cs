using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Tsp.Tests
{
	[TestFixture]
	public class TimeStampTokenInfoUnitTest
	{
		private static readonly byte[] tstInfo1 = Hex.Decode(
			  "303e02010106022a033021300906052b0e03021a050004140000000000000000000000000000000000000000"
			+ "020118180f32303035313130313038313732315a");

		private static readonly byte[] tstInfo2 = Hex.Decode(
			  "304c02010106022a033021300906052b0e03021a05000414ffffffffffffffffffffffffffffffffffffffff"
			+ "020117180f32303035313130313038323934355a3009020103800101810102020164");

		private static readonly byte[] tstInfo3 = Hex.Decode(
			  "304f02010106022a033021300906052b0e03021a050004140000000000000000000000000000000000000000"
			+ "020117180f32303035313130313038343733355a30090201038001018101020101ff020164");

		private static readonly byte[] tstInfoDudDate = Hex.Decode(
			  "303e02010106022a033021300906052b0e03021a050004140000000000000000000000000000000000000000"
			+ "020118180f32303056313130313038313732315a");

		[Test]
		public void TestTstInfo1()
		{
			TimeStampTokenInfo tstInfo = getTimeStampTokenInfo(tstInfo1);

			//
			// verify
			//
			GenTimeAccuracy accuracy = tstInfo.GenTimeAccuracy;

			Assert.IsNull(accuracy);

			Assert.AreEqual(new BigInteger("24"), tstInfo.SerialNumber);

			Assert.AreEqual(1130833041000L, DateTimeUtilities.DateTimeToUnixMs(tstInfo.GenTime));

			Assert.AreEqual("1.2.3", tstInfo.Policy);

			Assert.AreEqual(false, tstInfo.IsOrdered);

			Assert.IsNull(tstInfo.Nonce);

			Assert.AreEqual(TspAlgorithms.Sha1, tstInfo.MessageImprintAlgOid);

			Assert.IsTrue(Arrays.AreEqual(new byte[20], tstInfo.GetMessageImprintDigest()));

			Assert.IsTrue(Arrays.AreEqual(tstInfo1, tstInfo.GetEncoded()));
		}

	    [Test]
		public void TestTstInfo2()
		{
			TimeStampTokenInfo tstInfo = getTimeStampTokenInfo(tstInfo2);

			//
			// verify
			//
			GenTimeAccuracy accuracy = tstInfo.GenTimeAccuracy;

			Assert.AreEqual(3, accuracy.Seconds);
			Assert.AreEqual(1, accuracy.Millis);
			Assert.AreEqual(2, accuracy.Micros);

			Assert.AreEqual(new BigInteger("23"), tstInfo.SerialNumber);

			Assert.AreEqual(1130833785000L, DateTimeUtilities.DateTimeToUnixMs(tstInfo.GenTime));

			Assert.AreEqual("1.2.3", tstInfo.Policy);

			Assert.AreEqual(false, tstInfo.IsOrdered);

			Assert.AreEqual(tstInfo.Nonce, BigInteger.ValueOf(100));

			Assert.IsTrue(Arrays.AreEqual(Hex.Decode("ffffffffffffffffffffffffffffffffffffffff"), tstInfo.GetMessageImprintDigest()));

			Assert.IsTrue(Arrays.AreEqual(tstInfo2, tstInfo.GetEncoded()));
		}

	    [Test]
		public void TestTstInfo3()
		{
			TimeStampTokenInfo tstInfo = getTimeStampTokenInfo(tstInfo3);

			//
			// verify
			//
			GenTimeAccuracy accuracy = tstInfo.GenTimeAccuracy;

			Assert.AreEqual(3, accuracy.Seconds);
			Assert.AreEqual(1, accuracy.Millis);
			Assert.AreEqual(2, accuracy.Micros);

			Assert.AreEqual(new BigInteger("23"), tstInfo.SerialNumber);

			Assert.AreEqual(1130834855000L, DateTimeUtilities.DateTimeToUnixMs(tstInfo.GenTime));

			Assert.AreEqual("1.2.3", tstInfo.Policy);

			Assert.AreEqual(true, tstInfo.IsOrdered);

			Assert.AreEqual(tstInfo.Nonce, BigInteger.ValueOf(100));

			Assert.AreEqual(TspAlgorithms.Sha1, tstInfo.MessageImprintAlgOid);

			Assert.IsTrue(Arrays.AreEqual(new byte[20], tstInfo.GetMessageImprintDigest()));

			Assert.IsTrue(Arrays.AreEqual(tstInfo3, tstInfo.GetEncoded()));
		}

		[Test]
		public void TestTstInfoDudDate()
		{
			try
			{
				getTimeStampTokenInfo(tstInfoDudDate);

				Assert.Fail("dud date not detected.");
			}
			catch (TspException)
			{
				// expected
			}
		}

		private TimeStampTokenInfo getTimeStampTokenInfo(
			byte[] tstInfo)
		{
			return new TimeStampTokenInfo(
				TstInfo.GetInstance(
					Asn1Object.FromByteArray(tstInfo)));
		}
	}
}
