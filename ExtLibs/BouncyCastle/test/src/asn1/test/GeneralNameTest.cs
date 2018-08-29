using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class GeneralNameTest
		: SimpleTest
	{
		private static readonly byte[] ipv4 = Hex.Decode("87040a090800");
		private static readonly byte[] ipv4WithMask = Hex.Decode("87080a090800ffffff00");

		private static readonly byte[] ipv6a = Hex.Decode("871020010db885a308d313198a2e03707334");
		private static readonly byte[] ipv6b = Hex.Decode("871020010db885a3000013198a2e03707334");
		private static readonly byte[] ipv6c = Hex.Decode("871000000000000000000000000000000001");
		private static readonly byte[] ipv6d = Hex.Decode("871020010db885a3000000008a2e03707334");
		private static readonly byte[] ipv6e = Hex.Decode("871020010db885a3000000008a2e0a090800");
		private static readonly byte[] ipv6f = Hex.Decode("872020010db885a3000000008a2e0a090800ffffffffffff00000000000000000000");
		private static readonly byte[] ipv6g = Hex.Decode("872020010db885a3000000008a2e0a090800ffffffffffffffffffffffffffffffff");
		private static readonly byte[] ipv6h = Hex.Decode("872020010db885a300000000000000000000ffffffffffff00000000000000000000");
	    
		public override string Name
		{
			get { return "GeneralName"; }
		}

		public override void PerformTest()
		{
			GeneralName nm = new GeneralName(GeneralName.IPAddress, "10.9.8.0");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv4))
			{
				Fail("ipv4 encoding failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "10.9.8.0/255.255.255.0");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv4WithMask))
			{
				Fail("ipv4 with netmask 1 encoding failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "10.9.8.0/24");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv4WithMask))
			{
				Fail("ipv4 with netmask 2 encoding failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "2001:0db8:85a3:08d3:1319:8a2e:0370:7334");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv6a))
			{
				Fail("ipv6 with netmask encoding failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "2001:0db8:85a3::1319:8a2e:0370:7334");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv6b))
			{
				Fail("ipv6b encoding failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "::1");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv6c))
			{
				Fail("ipv6c failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "2001:0db8:85a3::8a2e:0370:7334");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv6d))
			{
				Fail("ipv6d failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "2001:0db8:85a3::8a2e:10.9.8.0");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv6e))
			{
				Fail("ipv6e failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "2001:0db8:85a3::8a2e:10.9.8.0/ffff:ffff:ffff::0000");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv6f))
			{
				Fail("ipv6f failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "2001:0db8:85a3::8a2e:10.9.8.0/128");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv6g))
			{
				Fail("ipv6g failed");
			}

			nm = new GeneralName(GeneralName.IPAddress, "2001:0db8:85a3::/48");
			if (!Arrays.AreEqual(nm.GetEncoded(), ipv6h))
			{
				Fail("ipv6h failed");
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new GeneralNameTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(resultText, Name + ": Okay", resultText);
		}
	}
}
