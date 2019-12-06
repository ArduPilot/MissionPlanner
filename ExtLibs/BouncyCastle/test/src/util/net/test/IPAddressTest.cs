using System;

using NUnit.Framework;

namespace Org.BouncyCastle.Utilities.Net.Tests
{
	[TestFixture]
	public class IPTest
	{
		private static readonly string[] validIP4v = new string[]
		{ "0.0.0.0", "255.255.255.255", "192.168.0.0" };

		private static readonly string[] invalidIP4v = new string[]
		{ "0.0.0.0.1", "256.255.255.255", "1", "A.B.C", "1:.4.6.5" };

		private static readonly string[] validIP6v = new string[]
		{ "0:0:0:0:0:0:0:0", "FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFF",
				"0:1:2:3:FFFF:5:FFFF:1" };

		private static readonly string[] invalidIP6v = new string[]
		{ "0.0.0.0:1", "FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFFF" };

		private void doTestIP(
			string[]	valid,
			string[]	invalid)
		{
			for (int i = 0; i < valid.Length; i++)
			{
				if (!IPAddress.IsValid(valid[i]))
				{
					Assert.Fail("Valid input string not accepted: " + valid[i] + ".");
				}
			}

			for (int i = 0; i < invalid.Length; i++)
			{
				if (IPAddress.IsValid(invalid[i]))
				{
					Assert.Fail("Invalid input string accepted: " + invalid[i] + ".");
				}
			}
		}

		public string Name
		{
			get { return "IPTest"; }
		}

		[Test]
		public void TestIPv4()
		{
			doTestIP(validIP4v, invalidIP4v);
		}

		[Test]
		public void TestIPv6()
		{
			doTestIP(validIP6v, invalidIP6v);
		}
	}
}
