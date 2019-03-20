using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	/**
	* X.690 test example
	*/
	[TestFixture]
	public class TagTest
		: SimpleTest
	{
		private static readonly byte[] longTagged = Base64.Decode(
			  "ZSRzIp8gEEZFRENCQTk4NzY1NDMyMTCfIQwyMDA2MDQwMTEyMzSUCCAFERVz"
			+ "A4kCAHEXGBkalAggBRcYGRqUCCAFZS6QAkRFkQlURUNITklLRVKSBQECAwQF"
			+ "kxAREhMUFRYXGBkalAggBREVcwOJAgBxFxgZGpQIIAUXGBkalAggBWUukAJE"
			+ "RZEJVEVDSE5JS0VSkgUBAgMEBZMQERITFBUWFxgZGpQIIAURFXMDiQIAcRcY"
			+ "GRqUCCAFFxgZGpQIIAVlLpACREWRCVRFQ0hOSUtFUpIFAQIDBAWTEBESExQV"
			+ "FhcYGRqUCCAFERVzA4kCAHEXGBkalAggBRcYGRqUCCAFFxgZGpQIIAUXGBka"
			+ "lAg=");

		private static readonly byte[] longAppSpecificTag = Hex.Decode("5F610101");

		public override string Name
		{
			get { return "Tag"; }
		}

		public override void PerformTest()
		{
            Asn1InputStream aIn = new Asn1InputStream(longTagged);

            DerApplicationSpecific app = (DerApplicationSpecific)aIn.ReadObject();

            aIn = new Asn1InputStream(app.GetContents());

            app = (DerApplicationSpecific)aIn.ReadObject();

            aIn = new Asn1InputStream(app.GetContents());

            Asn1TaggedObject tagged = (Asn1TaggedObject)aIn.ReadObject();

			if (tagged.TagNo != 32)
			{
				Fail("unexpected tag value found - not 32");
			}

			tagged = (Asn1TaggedObject) Asn1Object.FromByteArray(tagged.GetEncoded());

			if (tagged.TagNo != 32)
			{
				Fail("unexpected tag value found on recode - not 32");
			}

			tagged = (Asn1TaggedObject) aIn.ReadObject();

			if (tagged.TagNo != 33)
			{
				Fail("unexpected tag value found - not 33");
			}

			tagged = (Asn1TaggedObject) Asn1Object.FromByteArray(tagged.GetEncoded());

			if (tagged.TagNo != 33)
			{
				Fail("unexpected tag value found on recode - not 33");
			}

			aIn = new Asn1InputStream(longAppSpecificTag);

			app = (DerApplicationSpecific)aIn.ReadObject();

			if (app.ApplicationTag != 97)
			{
				Fail("incorrect tag number read");
			}

			app = (DerApplicationSpecific)Asn1Object.FromByteArray(app.GetEncoded());

			if (app.ApplicationTag != 97)
			{
				Fail("incorrect tag number read on recode");
			}

			SecureRandom sr = new SecureRandom();
			for (int i = 0; i < 100; ++i)
			{
				int testTag = (sr.NextInt() & int.MaxValue) >> sr.Next(26);
				app = new DerApplicationSpecific(testTag, new byte[]{ 1 });
				app = (DerApplicationSpecific)Asn1Object.FromByteArray(app.GetEncoded());

				if (app.ApplicationTag != testTag)
				{
					Fail("incorrect tag number read on recode (random test value: " + testTag + ")");
				}
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new TagTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
