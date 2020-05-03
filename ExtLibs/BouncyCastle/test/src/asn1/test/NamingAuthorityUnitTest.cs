using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Asn1.X500;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class NamingAuthorityUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "NamingAuthority"; }
		}

		public override void PerformTest()
		{
			DerObjectIdentifier namingAuthorityID = new DerObjectIdentifier("1.2.3");
			string namingAuthorityURL = "url";
			DirectoryString namingAuthorityText = new DirectoryString("text");

			NamingAuthority auth =  new NamingAuthority(namingAuthorityID, namingAuthorityURL, namingAuthorityText);

			checkConstruction(auth, namingAuthorityID, namingAuthorityURL, namingAuthorityText);

			auth =  new NamingAuthority(null, namingAuthorityURL, namingAuthorityText);

			checkConstruction(auth, null, namingAuthorityURL, namingAuthorityText);

			auth =  new NamingAuthority(namingAuthorityID, null, namingAuthorityText);

			checkConstruction(auth, namingAuthorityID, null, namingAuthorityText);

			auth =  new NamingAuthority(namingAuthorityID, namingAuthorityURL, null);

			checkConstruction(auth, namingAuthorityID, namingAuthorityURL, null);

			auth = NamingAuthority.GetInstance(null);

			if (auth != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				NamingAuthority.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			NamingAuthority		auth,
			DerObjectIdentifier	namingAuthorityID,
			string				namingAuthorityURL,
			DirectoryString		namingAuthorityText)
		{
			checkValues(auth, namingAuthorityID, namingAuthorityURL, namingAuthorityText);

			auth = NamingAuthority.GetInstance(auth);

			checkValues(auth, namingAuthorityID, namingAuthorityURL, namingAuthorityText);

			Asn1InputStream aIn = new Asn1InputStream(auth.ToAsn1Object().GetEncoded());

			Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

			auth = NamingAuthority.GetInstance(seq);

			checkValues(auth, namingAuthorityID, namingAuthorityURL, namingAuthorityText);
		}

		private void checkValues(
			NamingAuthority		auth,
			DerObjectIdentifier	namingAuthorityId,
			string				namingAuthorityURL,
			DirectoryString		namingAuthorityText)
		{
			checkOptionalField("namingAuthorityId", namingAuthorityId, auth.NamingAuthorityID);
			checkOptionalField("namingAuthorityURL", namingAuthorityURL, auth.NamingAuthorityUrl);
			checkOptionalField("namingAuthorityText", namingAuthorityText, auth.NamingAuthorityText);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new NamingAuthorityUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
