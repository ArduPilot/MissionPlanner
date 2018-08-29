using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	public abstract class Asn1UnitTest
		: SimpleTest
	{
		protected void checkMandatoryField(string name, Asn1Encodable expected, Asn1Encodable present)
		{
			if (!expected.Equals(present))
			{
				Fail(name + " field doesn't match.");
			}
		}

		protected void checkMandatoryField(string name, string expected, string present)
		{
			if (!expected.Equals(present))
			{
				Fail(name + " field doesn't match.");
			}
		}

		protected void checkMandatoryField(string name, byte[] expected, byte[] present)
		{
			if (!AreEqual(expected, present))
			{
				Fail(name + " field doesn't match.");
			}
		}

		protected void checkMandatoryField(string name, int expected, int present)
		{
			if (expected != present)
			{
				Fail(name + " field doesn't match.");
			}
		}

		protected void checkOptionalField(string name, Asn1Encodable expected, Asn1Encodable present)
		{
			if (expected != null)
			{
				if (!expected.Equals(present))
				{
					Fail(name + " field doesn't match.");
				}
			}
			else if (present != null)
			{
				Fail(name + " field found when none expected.");
			}
		}

		protected void checkOptionalField(string name, string expected, string present)
		{
			if (expected != null)
			{
				if (!expected.Equals(present))
				{
					Fail(name + " field doesn't match.");
				}
			}
			else if (present != null)
			{
				Fail(name + " field found when none expected.");
			}
		}

		protected void checkOptionalField(string name, BigInteger expected, BigInteger present)
		{
			if (expected != null)
			{
				if (!expected.Equals(present))
				{
					Fail(name + " field doesn't match.");
				}
			}
			else if (present != null)
			{
				Fail(name + " field found when none expected.");
			}
		}
	}
}
