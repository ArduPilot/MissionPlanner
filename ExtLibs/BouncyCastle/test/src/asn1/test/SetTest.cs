using System;

using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	/// <remarks>Set sorting test example.</remarks>
	[TestFixture]
	public class SetTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "Set"; }
		}

		private void checkSortedSet(
			int		attempt,
			Asn1Set	s)
		{
			if (s[0] is DerBoolean
				&& s[1] is DerInteger
				&& s[2] is DerBitString
				&& s[3] is DerOctetString)
			{
				return;
			}

			Fail("sorting failed on attempt: " + attempt);
		}

		public override void PerformTest()
		{
			Asn1EncodableVector v = new Asn1EncodableVector();
			byte[] data = new byte[10];

			v.Add(new DerOctetString(data));
			v.Add(new DerBitString(data));
			v.Add(new DerInteger(100));
			v.Add(DerBoolean.True);

			checkSortedSet(0, new DerSet(v));

			v = new Asn1EncodableVector();
			v.Add(new DerInteger(100));
			v.Add(DerBoolean.True);
			v.Add(new DerOctetString(data));
			v.Add(new DerBitString(data));

			checkSortedSet(1, new DerSet(v));

			v = new Asn1EncodableVector();
			v.Add(DerBoolean.True);
			v.Add(new DerOctetString(data));
			v.Add(new DerBitString(data));
			v.Add(new DerInteger(100));


			checkSortedSet(2, new DerSet(v));

			v = new Asn1EncodableVector();
			v.Add(new DerBitString(data));
			v.Add(new DerOctetString(data));
			v.Add(new DerInteger(100));
			v.Add(DerBoolean.True);

			checkSortedSet(3, new DerSet(v));

			v = new Asn1EncodableVector();
			v.Add(new DerOctetString(data));
			v.Add(new DerBitString(data));
			v.Add(new DerInteger(100));
			v.Add(DerBoolean.True);

			Asn1Set s = new BerSet(v);

			if (!(s[0] is DerOctetString))
			{
				Fail("BER set sort order changed.");
			}

			// create an implicitly tagged "set" without sorting
			Asn1TaggedObject tag = new DerTaggedObject(false, 1, new DerSequence(v));
			s = Asn1Set.GetInstance(tag, false);

			if (s[0] is DerBoolean)
			{
				Fail("sorted when shouldn't be.");
			}

			// equality test
			v = new Asn1EncodableVector();

			v.Add(DerBoolean.True);
			v.Add(DerBoolean.True);
			v.Add(DerBoolean.True);

			s = new DerSet(v);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new SetTest());
		}

		[Test]
		public void TestFunction()
		{
		    string resultText = Perform().ToString();

		    Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
