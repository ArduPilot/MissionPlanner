using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class Asn1SequenceParserTest
    {
        private static readonly byte[] seqData = Hex.Decode("3006020100060129");
        private static readonly byte[] nestedSeqData = Hex.Decode("300b0201000601293003020101");
        private static readonly byte[] expTagSeqData = Hex.Decode("a1083006020100060129");
        private static readonly byte[] implTagSeqData = Hex.Decode("a106020100060129");
        private static readonly byte[] nestedSeqExpTagData = Hex.Decode("300d020100060129a1053003020101");
        private static readonly byte[] nestedSeqImpTagData = Hex.Decode("300b020100060129a103020101");

		private static readonly byte[] berSeqData = Hex.Decode("30800201000601290000");
        private static readonly byte[] berDerNestedSeqData = Hex.Decode("308002010006012930030201010000");
        private static readonly byte[] berNestedSeqData = Hex.Decode("3080020100060129308002010100000000");
        private static readonly byte[] berExpTagSeqData = Hex.Decode("a180308002010006012900000000");
		private static readonly byte[] berSeqWithDERNullData = Hex.Decode("308005000201000601290000");

		[Test]
		public void TestDerWriting()
        {
            MemoryStream bOut = new MemoryStream();
            DerSequenceGenerator seqGen = new DerSequenceGenerator(bOut);

			seqGen.AddObject(new DerInteger(BigInteger.Zero));

			seqGen.AddObject(new DerObjectIdentifier("1.1"));

			seqGen.Close();

			Assert.IsTrue(Arrays.AreEqual(seqData, bOut.ToArray()), "basic DER writing test failed.");
        }

		[Test]
		public void TestNestedDerWriting()
        {
            MemoryStream bOut = new MemoryStream();
            DerSequenceGenerator seqGen1 = new DerSequenceGenerator(bOut);

			seqGen1.AddObject(new DerInteger(BigInteger.Zero));

			seqGen1.AddObject(new DerObjectIdentifier("1.1"));

			DerSequenceGenerator seqGen2 = new DerSequenceGenerator(seqGen1.GetRawOutputStream());

			seqGen2.AddObject(new DerInteger(BigInteger.One));

			seqGen2.Close();

			seqGen1.Close();

			Assert.IsTrue(Arrays.AreEqual(nestedSeqData, bOut.ToArray()), "nested DER writing test failed.");
        }

		[Test]
		public void TestDerExplicitTaggedSequenceWriting()
        {
            MemoryStream bOut = new MemoryStream();
            DerSequenceGenerator seqGen = new DerSequenceGenerator(bOut, 1, true);

            seqGen.AddObject(new DerInteger(BigInteger.Zero));

            seqGen.AddObject(new DerObjectIdentifier("1.1"));

            seqGen.Close();

            Assert.IsTrue(Arrays.AreEqual(expTagSeqData, bOut.ToArray()), "explicit tag writing test failed.");
        }

		[Test]
		public void TestDerImplicitTaggedSequenceWriting()
        {
            MemoryStream bOut = new MemoryStream();
            DerSequenceGenerator seqGen = new DerSequenceGenerator(bOut, 1, false);

			seqGen.AddObject(new DerInteger(BigInteger.Zero));

			seqGen.AddObject(new DerObjectIdentifier("1.1"));

			seqGen.Close();

			Assert.IsTrue(Arrays.AreEqual(implTagSeqData, bOut.ToArray()), "implicit tag writing test failed.");
        }

		[Test]
		public void TestNestedExplicitTagDerWriting()
        {
            MemoryStream bOut = new MemoryStream();
            DerSequenceGenerator  seqGen1 = new DerSequenceGenerator(bOut);

            seqGen1.AddObject(new DerInteger(BigInteger.Zero));

            seqGen1.AddObject(new DerObjectIdentifier("1.1"));

            DerSequenceGenerator seqGen2 = new DerSequenceGenerator(seqGen1.GetRawOutputStream(), 1, true);

            seqGen2.AddObject(new DerInteger(BigInteger.ValueOf(1)));

            seqGen2.Close();

            seqGen1.Close();

            Assert.IsTrue(Arrays.AreEqual(nestedSeqExpTagData, bOut.ToArray()), "nested explicit tagged DER writing test failed.");
        }

		[Test]
		public void TestNestedImplicitTagDerWriting()
        {
            MemoryStream bOut = new MemoryStream();
            DerSequenceGenerator seqGen1 = new DerSequenceGenerator(bOut);

			seqGen1.AddObject(new DerInteger(BigInteger.Zero));

			seqGen1.AddObject(new DerObjectIdentifier("1.1"));

			DerSequenceGenerator seqGen2 = new DerSequenceGenerator(seqGen1.GetRawOutputStream(), 1, false);

			seqGen2.AddObject(new DerInteger(BigInteger.ValueOf(1)));

			seqGen2.Close();

			seqGen1.Close();

			Assert.IsTrue(Arrays.AreEqual(nestedSeqImpTagData, bOut.ToArray()), "nested implicit tagged DER writing test failed.");
        }

		[Test]
		public void TestBerWriting()
        {
            MemoryStream bOut = new MemoryStream();
            BerSequenceGenerator seqGen = new BerSequenceGenerator(bOut);

			seqGen.AddObject(new DerInteger(BigInteger.Zero));

			seqGen.AddObject(new DerObjectIdentifier("1.1"));

			seqGen.Close();

			Assert.IsTrue(Arrays.AreEqual(berSeqData, bOut.ToArray()), "basic BER writing test failed.");
        }

		[Test]
		public void TestNestedBerDerWriting()
        {
            MemoryStream bOut = new MemoryStream();
            BerSequenceGenerator seqGen1 = new BerSequenceGenerator(bOut);

			seqGen1.AddObject(new DerInteger(BigInteger.Zero));

			seqGen1.AddObject(new DerObjectIdentifier("1.1"));

			DerSequenceGenerator seqGen2 = new DerSequenceGenerator(seqGen1.GetRawOutputStream());

			seqGen2.AddObject(new DerInteger(BigInteger.ValueOf(1)));

			seqGen2.Close();

			seqGen1.Close();

			Assert.IsTrue(Arrays.AreEqual(berDerNestedSeqData, bOut.ToArray()), "nested BER/DER writing test failed.");
        }

		[Test]
		public void TestNestedBerWriting()
        {
            MemoryStream bOut = new MemoryStream();
            BerSequenceGenerator seqGen1 = new BerSequenceGenerator(bOut);

			seqGen1.AddObject(new DerInteger(BigInteger.Zero));

			seqGen1.AddObject(new DerObjectIdentifier("1.1"));

			BerSequenceGenerator seqGen2 = new BerSequenceGenerator(seqGen1.GetRawOutputStream());

			seqGen2.AddObject(new DerInteger(BigInteger.ValueOf(1)));

			seqGen2.Close();

			seqGen1.Close();

			Assert.IsTrue(Arrays.AreEqual(berNestedSeqData, bOut.ToArray()), "nested BER writing test failed.");
        }

		[Test]
		public void TestDerReading()
        {
            Asn1StreamParser aIn = new Asn1StreamParser(seqData);

			Asn1SequenceParser seq = (Asn1SequenceParser)aIn.ReadObject();
            int count = 0;

			Assert.IsNotNull(seq, "null sequence returned");

			object o;
			while ((o = seq.ReadObject()) != null)
            {
                switch (count)
                {
                    case 0:
                        Assert.IsTrue(o is DerInteger);
                        break;
                    case 1:
                        Assert.IsTrue(o is DerObjectIdentifier);
                        break;
                }
                count++;
            }

			Assert.AreEqual(2, count, "wrong number of objects in sequence");
        }

		private void doTestNestedReading(
            byte[] data)
        {
            Asn1StreamParser aIn = new Asn1StreamParser(data);

			Asn1SequenceParser seq = (Asn1SequenceParser) aIn.ReadObject();
            object o = null;
            int count = 0;

			Assert.IsNotNull(seq, "null sequence returned");

			while ((o = seq.ReadObject()) != null)
            {
                switch (count)
                {
                    case 0:
                        Assert.IsTrue(o is DerInteger);
                        break;
                    case 1:
                        Assert.IsTrue(o is DerObjectIdentifier);
                        break;
                    case 2:
                        Assert.IsTrue(o is Asn1SequenceParser);

						Asn1SequenceParser s = (Asn1SequenceParser)o;

						// NB: Must exhaust the nested parser
						while (s.ReadObject() != null)
						{
							// Ignore
						}

						break;
                }
                count++;
            }

			Assert.AreEqual(3, count, "wrong number of objects in sequence");
        }

		[Test]
		public void TestNestedDerReading()
        {
            doTestNestedReading(nestedSeqData);
        }

		[Test]
		public void TestBerReading()
        {
            Asn1StreamParser aIn = new Asn1StreamParser(berSeqData);

			Asn1SequenceParser seq = (Asn1SequenceParser) aIn.ReadObject();
            object o = null;
            int count = 0;

			Assert.IsNotNull(seq, "null sequence returned");

			while ((o = seq.ReadObject()) != null)
            {
                switch (count)
                {
                    case 0:
                        Assert.IsTrue(o is DerInteger);
                        break;
                    case 1:
                        Assert.IsTrue(o is DerObjectIdentifier);
                        break;
                }
                count++;
            }

			Assert.AreEqual(2, count, "wrong number of objects in sequence");
        }

		[Test]
		public void TestNestedBerDerReading()
        {
            doTestNestedReading(berDerNestedSeqData);
        }

		[Test]
		public void TestNestedBerReading()
        {
            doTestNestedReading(berNestedSeqData);
        }

		[Test]
		public void TestBerExplicitTaggedSequenceWriting()
        {
            MemoryStream bOut = new MemoryStream();
            BerSequenceGenerator seqGen = new BerSequenceGenerator(bOut, 1, true);

			seqGen.AddObject(new DerInteger(BigInteger.Zero));

			seqGen.AddObject(new DerObjectIdentifier("1.1"));

			seqGen.Close();

			Assert.IsTrue(Arrays.AreEqual(berExpTagSeqData, bOut.ToArray()), "explicit BER tag writing test failed.");
        }

		[Test]
		public void TestSequenceWithDerNullReading()
		{
			doTestParseWithNull(berSeqWithDERNullData);
		}

		private void doTestParseWithNull(
			byte[] data)
		{
			Asn1StreamParser aIn = new Asn1StreamParser(data);
			Asn1SequenceParser seq = (Asn1SequenceParser) aIn.ReadObject();
			object o;
			int count = 0;

			Assert.IsNotNull(seq, "null sequence returned");

			while ((o = seq.ReadObject()) != null)
			{
				switch (count)
				{
					case 0:
						Assert.IsTrue(o is Asn1Null);
						break;
					case 1:
						Assert.IsTrue(o is DerInteger);
						break;
					case 2:
						Assert.IsTrue(o is DerObjectIdentifier);
						break;
				}
				count++;
			}

			Assert.AreEqual(3, count, "wrong number of objects in sequence");
		}
	}
}
