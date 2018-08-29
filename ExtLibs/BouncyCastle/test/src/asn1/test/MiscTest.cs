using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class MiscTest
        : SimpleTest
    {
        private void DoShouldFailOnExtraData()
        {
            // basic construction
            DerBitString s1 = new DerBitString(new byte[0], 0);

            Asn1Object.FromByteArray(s1.GetEncoded());

            Asn1Object.FromByteArray(new BerSequence(s1).GetEncoded());

            try
            {
                Asn1Object obj = Asn1Object.FromByteArray(Arrays.Concatenate(s1.GetEncoded(), new byte[1]));
                Fail("no exception");
            }
            catch (IOException e)
            {
                //if (!"Extra data detected in stream".Equals(e.Message))
                if (!"extra data found after object".Equals(e.Message))
                {
                        Fail("wrong exception");
                }
            }
        }

        private void DoDerIntegerTest()
        {
            try
            {
                new DerInteger(new byte[] { 0, 0, 0, 1});
            }
            catch (ArgumentException e)
            {
                IsTrue("wrong exc", "malformed integer".Equals(e.Message));
            }

            try
            {
                new DerInteger(new byte[] {(byte)0xff, (byte)0x80, 0, 1});
            }
            catch (ArgumentException e)
            {
                IsTrue("wrong exc", "malformed integer".Equals(e.Message));
            }

            try
            {
                new DerEnumerated(new byte[] { 0, 0, 0, 1});
            }
            catch (ArgumentException e)
            {
                IsTrue("wrong exc", "malformed enumerated".Equals(e.Message));
            }

            try
            {
                new DerEnumerated(new byte[] {(byte)0xff, (byte)0x80, 0, 1});
            }
            catch (ArgumentException e)
            {
                IsTrue("wrong exc", "malformed enumerated".Equals(e.Message));
            }
        }

        public override void PerformTest()
        {
            byte[] testIv = { 1, 2, 3, 4, 5, 6, 7, 8 };

            Asn1Encodable[] values =
            {
                new Cast5CbcParameters(testIv, 128),
                new NetscapeCertType(NetscapeCertType.Smime),
                new VerisignCzagExtension(new DerIA5String("hello")),
                new IdeaCbcPar(testIv),
                new NetscapeRevocationUrl(new DerIA5String("http://test"))
            };

            byte[] data = Base64.Decode("MA4ECAECAwQFBgcIAgIAgAMCBSAWBWhlbGxvMAoECAECAwQFBgcIFgtodHRwOi8vdGVzdA==");

            MemoryStream bOut = new MemoryStream();
            Asn1OutputStream aOut = new Asn1OutputStream(bOut);

            for (int i = 0; i != values.Length; i++)
            {
                aOut.WriteObject(values[i]);
            }

            if (!Arrays.AreEqual(bOut.ToArray(), data))
            {
                Fail("Failed data check");
            }

            Asn1InputStream aIn = new Asn1InputStream(bOut.ToArray());

            for (int i = 0; i != values.Length; i++)
            {
                Asn1Object o = aIn.ReadObject();

                if (!values[i].Equals(o))
                {
                    Fail("Failed equality test for " + o);
                }

                if (o.GetHashCode() != values[i].GetHashCode())
                {
                    Fail("Failed hashCode test for " + o);
                }
            }

            DoShouldFailOnExtraData();
            DoDerIntegerTest();
        }

        public override string Name
        {
            get { return "Misc"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new MiscTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
