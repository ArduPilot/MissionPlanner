using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Cms;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class OctetStringTest
	{
		[Test]
		public void TestReadingWriting()
		{
			MemoryStream bOut = new MemoryStream();
			BerOctetStringGenerator octGen = new BerOctetStringGenerator(bOut);

			Stream outStream = octGen.GetOctetOutputStream();

			outStream.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);
			outStream.Write(new byte[4], 0, 4);

			outStream.Close();

			Asn1StreamParser aIn = new Asn1StreamParser(bOut.ToArray());

			BerOctetStringParser s = (BerOctetStringParser)aIn.ReadObject();

			Stream inStream = s.GetOctetStream();
			int count = 0;

			while (inStream.ReadByte() >= 0)
			{
				count++;
			}

			Assert.AreEqual(8, count);
		}

		[Test]
		public void TestReadingWritingZeroInLength()
		{
			MemoryStream bOut = new MemoryStream();
			BerOctetStringGenerator octGen = new BerOctetStringGenerator(bOut);

			Stream outStream = octGen.GetOctetOutputStream();

			outStream.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);
			outStream.Write(new byte[512], 0, 512);  // forces a zero to appear in length

			outStream.Close();

			Asn1StreamParser aIn = new Asn1StreamParser(bOut.ToArray());

			BerOctetStringParser s = (BerOctetStringParser)aIn.ReadObject();

			Stream inStream = s.GetOctetStream();
			int         count = 0;

			while (inStream.ReadByte() >= 0)
			{
				count++;
			}

			Assert.AreEqual(516, count);
		}

		[Test]
		public void TestReadingWritingNested()
		{
			MemoryStream bOut = new MemoryStream();
			BerSequenceGenerator sGen = new BerSequenceGenerator(bOut);
			BerOctetStringGenerator octGen = new BerOctetStringGenerator(sGen.GetRawOutputStream());

			Stream outStream = octGen.GetOctetOutputStream();

			BerSequenceGenerator inSGen = new BerSequenceGenerator(outStream);

			BerOctetStringGenerator inOctGen = new BerOctetStringGenerator(inSGen.GetRawOutputStream());

			Stream inOut = inOctGen.GetOctetOutputStream();

			inOut.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);
			inOut.Write(new byte[10], 0, 10);

			inOut.Close();

			inSGen.Close();

			outStream.Close();

			sGen.Close();

			Asn1StreamParser aIn = new Asn1StreamParser(bOut.ToArray());

			BerSequenceParser sq = (BerSequenceParser)aIn.ReadObject();

			BerOctetStringParser s = (BerOctetStringParser)sq.ReadObject();

			Asn1StreamParser aIn2 = new Asn1StreamParser(s.GetOctetStream());

			BerSequenceParser sq2 = (BerSequenceParser)aIn2.ReadObject();

			BerOctetStringParser inS = (BerOctetStringParser)sq2.ReadObject();

			Stream inStream = inS.GetOctetStream();
			int         count = 0;

			while (inStream.ReadByte() >= 0)
			{
				count++;
			}

			Assert.AreEqual(14, count);
		}

		[Test]
		public void TestNestedStructure()
		{
			MemoryStream bOut = new MemoryStream();

			BerSequenceGenerator sGen = new BerSequenceGenerator(bOut);

			sGen.AddObject(new DerObjectIdentifier(CmsObjectIdentifiers.CompressedData.Id));

			BerSequenceGenerator cGen = new BerSequenceGenerator(sGen.GetRawOutputStream(), 0, true);

			cGen.AddObject(new DerInteger(0));

			//
			// AlgorithmIdentifier
			//
			DerSequenceGenerator algGen = new DerSequenceGenerator(cGen.GetRawOutputStream());

			algGen.AddObject(new DerObjectIdentifier("1.2"));

			algGen.Close();

			//
			// Encapsulated ContentInfo
			//
			BerSequenceGenerator eiGen = new BerSequenceGenerator(cGen.GetRawOutputStream());

			eiGen.AddObject(new DerObjectIdentifier("1.1"));

			BerOctetStringGenerator octGen = new BerOctetStringGenerator(eiGen.GetRawOutputStream(), 0, true);

			//
			// output containing zeroes
			//
			Stream outStream = octGen.GetOctetOutputStream();

			outStream.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);
			outStream.Write(new byte[4], 0, 4);
			outStream.Write(new byte[20], 0, 20);

			outStream.Close();
			eiGen.Close();
			cGen.Close();
			sGen.Close();

			//
			// reading back
			//
			Asn1StreamParser aIn = new Asn1StreamParser(bOut.ToArray());

			ContentInfoParser cp = new ContentInfoParser((Asn1SequenceParser)aIn.ReadObject());

			CompressedDataParser comData = new CompressedDataParser((Asn1SequenceParser)cp.GetContent(Asn1Tags.Sequence));
			ContentInfoParser content = comData.GetEncapContentInfo();

			Asn1OctetStringParser bytes = (Asn1OctetStringParser)content.GetContent(Asn1Tags.OctetString);

			Stream inStream = bytes.GetOctetStream();
			int count = 0;

			while (inStream.ReadByte() >= 0)
			{
				count++;
			}

			Assert.AreEqual(28, count);
		}
	}
}
