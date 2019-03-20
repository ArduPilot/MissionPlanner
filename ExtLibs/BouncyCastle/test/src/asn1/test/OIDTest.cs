using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	/**
	 * X.690 test example
	 */
	[TestFixture]
	public class OidTest
		: SimpleTest
	{
		byte[] req1 = Hex.Decode("0603813403");
		byte[] req2 = Hex.Decode("06082A36FFFFFFDD6311");

		public override string Name
		{
			get { return "OID"; }
		}

		private void recodeCheck(
			string	oid,
			byte[]	enc)
		{
			DerObjectIdentifier o = new DerObjectIdentifier(oid);
			DerObjectIdentifier encO = (DerObjectIdentifier) Asn1Object.FromByteArray(enc);

			if (!o.Equals(encO))
			{
				Fail("oid ID didn't match", o, encO);
			}

			byte[] bytes = o.GetDerEncoded();

			if (!Arrays.AreEqual(bytes, enc))
			{
				Fail("failed comparison test", Hex.ToHexString(enc), Hex.ToHexString(bytes));
			}
		}

		private void validOidCheck(
			string oid)
		{
			DerObjectIdentifier o = new DerObjectIdentifier(oid);
			o = (DerObjectIdentifier) Asn1Object.FromByteArray(o.GetEncoded());

			if (!o.Id.Equals(oid))
			{
				Fail("failed oid check: " + oid);
			}
		}

		private void invalidOidCheck(
			string oid)
		{
			try
			{
				new DerObjectIdentifier(oid);
				Fail("failed to catch bad oid: " + oid);
			}
			catch (FormatException)
			{
				// expected
			}
		}

		private void branchCheck(string stem, string branch)
		{
			string expected = stem + "." + branch;
			string actual = new DerObjectIdentifier(stem).Branch(branch).Id;

			if (expected != actual)
			{
				Fail("failed 'branch' check for " + stem + "/" + branch);
			}
		}

		private void onCheck(String stem, String test, bool expected)
		{
			if (expected != new DerObjectIdentifier(test).On(new DerObjectIdentifier(stem)))
			{
				Fail("failed 'on' check for " + stem + "/" + test);
			}
		}

		public override void PerformTest()
		{
			recodeCheck("2.100.3", req1);
			recodeCheck("1.2.54.34359733987.17", req2);

			validOidCheck(PkcsObjectIdentifiers.Pkcs9AtContentType.Id);
			validOidCheck("0.1");
			validOidCheck("1.1.127.32512.8323072.2130706432.545460846592.139637976727552.35747322042253312.9151314442816847872");
			validOidCheck("1.2.123.12345678901.1.1.1");
			validOidCheck("2.25.196556539987194312349856245628873852187.1");

			invalidOidCheck("0");
			invalidOidCheck("1");
			invalidOidCheck("2");
			invalidOidCheck("3.1");
			invalidOidCheck("..1");
			invalidOidCheck("192.168.1.1");
			invalidOidCheck(".123452");
			invalidOidCheck("1.");
			invalidOidCheck("1.345.23.34..234");
			invalidOidCheck("1.345.23.34.234.");
			invalidOidCheck(".12.345.77.234");
			invalidOidCheck(".12.345.77.234.");
			invalidOidCheck("1.2.3.4.A.5");
			invalidOidCheck("1,2");

			branchCheck("1.1", "2.2");

			onCheck("1.1", "1.1", false);
			onCheck("1.1", "1.2", false);
			onCheck("1.1", "1.2.1", false);
			onCheck("1.1", "2.1", false);
			onCheck("1.1", "1.11", false);
			onCheck("1.12", "1.1.2", false);
			onCheck("1.1", "1.1.1", true);
			onCheck("1.1", "1.1.2", true);
		}

		public static void Main(
			string[] args)
		{
			ITest test = new OidTest();
			ITestResult	result = test.Perform();

			Console.WriteLine(result);
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
