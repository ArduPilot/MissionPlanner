using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	/**
	* X.690 test example
	*/
	[TestFixture]
	public class StringTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "String"; }
		}

		public override void PerformTest()
		{
			DerBitString bs = new DerBitString(
				new byte[] { (byte)0x01,(byte)0x23,(byte)0x45,(byte)0x67,(byte)0x89,(byte)0xab,(byte)0xcd,(byte)0xef });

			if (!bs.GetString().Equals("#0309000123456789ABCDEF"))
			{
				Fail("DerBitString.GetString() result incorrect");
			}

			if (!bs.ToString().Equals("#0309000123456789ABCDEF"))
			{
				Fail("DerBitString.ToString() result incorrect");
			}

			bs = new DerBitString(
				new byte[] { (byte)0xfe,(byte)0xdc,(byte)0xba,(byte)0x98,(byte)0x76,(byte)0x54,(byte)0x32,(byte)0x10 });

			if (!bs.GetString().Equals("#030900FEDCBA9876543210"))
			{
				Fail("DerBitString.GetString() result incorrect");
			}

			if (!bs.ToString().Equals("#030900FEDCBA9876543210"))
			{
				Fail("DerBitString.ToString() result incorrect");
			}

			DerUniversalString us = new DerUniversalString(
				new byte[] { (byte)0x01,(byte)0x23,(byte)0x45,(byte)0x67,(byte)0x89,(byte)0xab,(byte)0xcd,(byte)0xef });

			if (!us.GetString().Equals("#1C080123456789ABCDEF"))
			{
				Fail("DerUniversalString.GetString() result incorrect");
			}

			if (!us.ToString().Equals("#1C080123456789ABCDEF"))
			{
				Fail("DerUniversalString.ToString() result incorrect");
			}

			us = new DerUniversalString(
				new byte[] { (byte)0xfe,(byte)0xdc,(byte)0xba,(byte)0x98,(byte)0x76,(byte)0x54,(byte)0x32,(byte)0x10 });

			if (!us.GetString().Equals("#1C08FEDCBA9876543210"))
			{
				Fail("DerUniversalString.GetString() result incorrect");
			}

			if (!us.ToString().Equals("#1C08FEDCBA9876543210"))
			{
				Fail("DerUniversalString.ToString() result incorrect");
			}

			byte[] t61Bytes = new byte[] { 0xff, 0xfe, 0xfd, 0xfc, 0xfb, 0xfa, 0xf9, 0xf8 };
			string t61String = Encoding.GetEncoding("iso-8859-1").GetString(t61Bytes, 0, t61Bytes.Length);
			DerT61String t61 = new DerT61String(t61Bytes);

			if (!t61.GetString().Equals(t61String))
			{
				Fail("DerT61String.GetString() result incorrect");
			}

			if (!t61.ToString().Equals(t61String))
			{
				Fail("DerT61String.ToString() result incorrect");
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new StringTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
