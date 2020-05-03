using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Icao;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class DataGroupHashUnitTest
        : SimpleTest
    {
        public override string Name
        {
			get { return "DataGroupHash"; }
        }

		private byte[] GenerateHash()
        {
            Random rand = new Random();
            byte[] bytes = new byte[20];
			rand.NextBytes(bytes);
			return bytes;
        }

		public override void PerformTest()
        {
            int dataGroupNumber = 1;
            Asn1OctetString dataHash = new DerOctetString(GenerateHash());
            DataGroupHash dg = new DataGroupHash(dataGroupNumber, dataHash);

            CheckConstruction(dg, dataGroupNumber, dataHash);

			try
			{
				DataGroupHash.GetInstance(null);
			}
			catch (Exception)
			{
				Fail("GetInstance() failed to handle null.");
			}

			try
            {
                DataGroupHash.GetInstance(new object());

				Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

		private void CheckConstruction(
            DataGroupHash dg,
            int dataGroupNumber,
            Asn1OctetString     dataGroupHashValue)
        {
            CheckValues(dg, dataGroupNumber, dataGroupHashValue);

			dg = DataGroupHash.GetInstance(dg);

			CheckValues(dg, dataGroupNumber, dataGroupHashValue);

			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(
				dg.ToAsn1Object().GetEncoded());

			dg = DataGroupHash.GetInstance(seq);

			CheckValues(dg, dataGroupNumber, dataGroupHashValue);
        }

		private void CheckValues(
            DataGroupHash	dg,
            int				dataGroupNumber,
            Asn1OctetString	dataGroupHashValue)
        {
            if (dg.DataGroupNumber != dataGroupNumber)
            {
                Fail("group number don't match.");
            }

			if (!dg.DataGroupHashValue.Equals(dataGroupHashValue))
            {
                Fail("hash value don't match.");
            }
        }

		public static void Main(
            string[] args)
        {
            RunTest(new DataGroupHashUnitTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
