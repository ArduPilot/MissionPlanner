using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class SignerLocationUnitTest
        : SimpleTest
    {
        public override string Name
        {
			get { return "SignerLocation"; }
        }

		public override void PerformTest()
        {
            DerUtf8String countryName = new DerUtf8String("Australia");

            SignerLocation sl = new SignerLocation(countryName, null, null);

            CheckConstruction(sl, DirectoryString.GetInstance(countryName), null, null);

            DerUtf8String localityName = new DerUtf8String("Melbourne");

            sl = new SignerLocation(null, localityName, null);

			CheckConstruction(sl, null, DirectoryString.GetInstance(localityName), null);

			sl = new SignerLocation(countryName, localityName, null);

			CheckConstruction(sl, DirectoryString.GetInstance(countryName), DirectoryString.GetInstance(localityName), null);

			Asn1Sequence postalAddress = new DerSequence(
				new DerUtf8String("line 1"),
				new DerUtf8String("line 2"));

            sl = new SignerLocation(null, null, postalAddress);

            CheckConstruction(sl, null, null, postalAddress);

            sl = new SignerLocation(countryName, null, postalAddress);

            CheckConstruction(sl, DirectoryString.GetInstance(countryName), null, postalAddress);

            sl = new SignerLocation(countryName, localityName, postalAddress);

            CheckConstruction(sl, DirectoryString.GetInstance(countryName), DirectoryString.GetInstance(localityName), postalAddress);

            sl = SignerLocation.GetInstance(null);

            if (sl != null)
            {
                Fail("null GetInstance() failed.");
            }

            try
            {
                SignerLocation.GetInstance(new object());

                Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }

            //
            // out of range postal address
            //
			postalAddress = new DerSequence(
				new DerUtf8String("line 1"),
				new DerUtf8String("line 2"),
				new DerUtf8String("line 3"),
				new DerUtf8String("line 4"),
				new DerUtf8String("line 5"),
				new DerUtf8String("line 6"),
				new DerUtf8String("line 7"));

			try
            {
                new SignerLocation(null, null, postalAddress);

                Fail("constructor failed to detect bad postalAddress.");
            }
            catch (ArgumentException)
            {
                // expected
            }

            try
            {
                new SignerLocation(new DerSequence(new DerTaggedObject(2, postalAddress)));

                Fail("sequence constructor failed to detect bad postalAddress.");
            }
            catch (ArgumentException)
            {
                // expected
            }

            try
            {
                new SignerLocation(new DerSequence(new DerTaggedObject(5, postalAddress)));

                Fail("sequence constructor failed to detect bad tag.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

		private void CheckConstruction(
            SignerLocation sl,
            DirectoryString countryName,
            DirectoryString localityName,
            Asn1Sequence postalAddress)
        {
            CheckValues(sl, countryName, localityName, postalAddress);

			sl = SignerLocation.GetInstance(sl);

			CheckValues(sl, countryName, localityName, postalAddress);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(
				sl.ToAsn1Object().GetEncoded());

			sl = SignerLocation.GetInstance(seq);

			CheckValues(sl, countryName, localityName, postalAddress);
        }

		private void CheckValues(
            SignerLocation sl,
            DirectoryString countryName,
            DirectoryString localityName,
            Asn1Sequence postalAddress)
        {
            if (countryName != null)
            {
                if (!countryName.Equals(sl.CountryName))
                {
                    Fail("countryNames don't match.");
                }
            }
            else if (sl.CountryName != null)
            {
                Fail("countryName found when none expected.");
            }

            if (localityName != null)
            {
                if (!localityName.Equals(sl.LocalityName))
                {
                    Fail("localityNames don't match.");
                }
            }
            else if (sl.LocalityName != null)
            {
                Fail("localityName found when none expected.");
            }

            if (postalAddress != null)
            {
                if (!postalAddress.Equals(sl.PostalAddress))
                {
                    Fail("postalAddresses don't match.");
                }
            }
            else if (sl.PostalAddress != null)
            {
                Fail("postalAddress found when none expected.");
            }
        }

		public static void Main(
			string[] args)
        {
            RunTest(new SignerLocationUnitTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
