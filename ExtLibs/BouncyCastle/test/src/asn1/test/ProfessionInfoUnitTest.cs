using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Asn1.X500;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class ProfessionInfoUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "ProfessionInfo"; }
		}

		public override void PerformTest()
		{
			NamingAuthority auth =  new NamingAuthority(new DerObjectIdentifier("1.2.3"), "url", new DirectoryString("fred"));
			DirectoryString[] professionItems = { new DirectoryString("substitution") };
			DerObjectIdentifier[] professionOids = { new DerObjectIdentifier("1.2.3") };
			string registrationNumber = "12345";
			DerOctetString addProfInfo = new DerOctetString(new byte[20]);

			ProfessionInfo info = new ProfessionInfo(auth, professionItems, professionOids, registrationNumber, addProfInfo);

			checkConstruction(info, auth, professionItems, professionOids, registrationNumber, addProfInfo);

			info = new ProfessionInfo(null, professionItems, professionOids, registrationNumber, addProfInfo);

			checkConstruction(info, null, professionItems, professionOids, registrationNumber, addProfInfo);

			info = new ProfessionInfo(auth, professionItems, null, registrationNumber, addProfInfo);

			checkConstruction(info, auth, professionItems, null, registrationNumber, addProfInfo);

			info = new ProfessionInfo(auth, professionItems, professionOids, null, addProfInfo);

			checkConstruction(info, auth, professionItems, professionOids, null, addProfInfo);

			info = new ProfessionInfo(auth, professionItems, professionOids, registrationNumber, null);

			checkConstruction(info, auth, professionItems, professionOids, registrationNumber, null);

			info = ProfessionInfo.GetInstance(null);

			if (info != null)
			{
				Fail("null GetInstance() failed.");
			}

			try
			{
				ProcurationSyntax.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			ProfessionInfo			profInfo,
			NamingAuthority			auth,
			DirectoryString[]		professionItems,
			DerObjectIdentifier[]	professionOids,
			string					registrationNumber,
			DerOctetString			addProfInfo)
		{
			checkValues(profInfo, auth, professionItems, professionOids, registrationNumber, addProfInfo);

			profInfo = ProfessionInfo.GetInstance(profInfo);

			checkValues(profInfo, auth, professionItems, professionOids, registrationNumber, addProfInfo);

			Asn1InputStream aIn = new Asn1InputStream(profInfo.ToAsn1Object().GetEncoded());

			Asn1Sequence seq = (Asn1Sequence) aIn.ReadObject();

			profInfo = ProfessionInfo.GetInstance(seq);

			checkValues(profInfo, auth, professionItems, professionOids, registrationNumber, addProfInfo);
		}

		private void checkValues(
			ProfessionInfo			profInfo,
			NamingAuthority			auth,
			DirectoryString[]		professionItems,
			DerObjectIdentifier[]	professionOids,
			string					registrationNumber,
			DerOctetString			addProfInfo)
		{
			checkOptionalField("auth", auth, profInfo.NamingAuthority);
			checkMandatoryField("professionItems", professionItems[0], profInfo.GetProfessionItems()[0]);
			if (professionOids != null)
			{
				checkOptionalField("professionOids", professionOids[0], profInfo.GetProfessionOids()[0]);
			}
			checkOptionalField("registrationNumber", registrationNumber, profInfo.RegistrationNumber);
			checkOptionalField("addProfessionInfo", addProfInfo, profInfo.AddProfessionInfo);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new ProfessionInfoUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
