using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Ess;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class ContentHintsUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "ContentHints"; }
		}

		public override void PerformTest()
		{
			DerUtf8String contentDescription = new DerUtf8String("Description");
			DerObjectIdentifier contentType = new DerObjectIdentifier("1.2.2.3");

			ContentHints hints = new ContentHints(contentType);

			checkConstruction(hints, contentType, null);

			hints = new ContentHints(contentType, contentDescription);

			checkConstruction(hints, contentType, contentDescription);

			hints = ContentHints.GetInstance(null);

			if (hints != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				ContentHints.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			ContentHints		hints,
			DerObjectIdentifier	contentType,
			DerUtf8String		description)
		{
			checkValues(hints, contentType, description);

			hints = ContentHints.GetInstance(hints);

			checkValues(hints, contentType, description);

			Asn1InputStream aIn = new Asn1InputStream(hints.ToAsn1Object().GetEncoded());

			Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

			hints = ContentHints.GetInstance(seq);

			checkValues(hints, contentType, description);
		}

		private void checkValues(
			ContentHints		hints,
			DerObjectIdentifier	contentType,
			DerUtf8String		description)
		{
			checkMandatoryField("contentType", contentType, hints.ContentType);
			checkOptionalField("description", description, hints.ContentDescription);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new ContentHintsUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
