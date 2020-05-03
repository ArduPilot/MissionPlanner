using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
    [TestFixture]
    public class PgpPbeTest
		: SimpleTest
    {
		private static readonly DateTime TestDateTime = new DateTime(2003, 8, 29, 23, 35, 11, 0);

		private static readonly byte[] enc1 = Base64.Decode(
            "jA0EAwMC5M5wWBP2HBZgySvUwWFAmMRLn7dWiZN6AkQMvpE3b6qwN3SSun7zInw2"
            + "hxxdgFzVGfbjuB8w");
//        private static readonly byte[] enc1crc = Base64.Decode("H66L");
		private static readonly char[] pass = "hello world".ToCharArray();

		/**
		 * Message with both PBE and symmetric
		 */
		private static readonly byte[] testPBEAsym = Base64.Decode(
			"hQIOA/ZlQEFWB5vuEAf/covEUaBve7NlWWdiO5NZubdtTHGElEXzG9hyBycp9At8" +
			"nZGi27xOZtEGFQo7pfz4JySRc3O0s6w7PpjJSonFJyNSxuze2LuqRwFWBYYcbS8/" +
			"7YcjB6PqutrT939OWsozfNqivI9/QyZCjBvFU89pp7dtUngiZ6MVv81ds2I+vcvk" +
			"GlIFcxcE1XoCIB3EvbqWNaoOotgEPT60unnB2BeDV1KD3lDRouMIYHfZ3SzBwOOI" +
			"6aK39sWnY5sAK7JjFvnDAMBdueOiI0Fy+gxbFD/zFDt4cWAVSAGTC4w371iqppmT" +
			"25TM7zAtCgpiq5IsELPlUZZnXKmnYQ7OCeysF0eeVwf+OFB9fyvCEv/zVQocJCg8" +
			"fWxfCBlIVFNeNQpeGygn/ZmRaILvB7IXDWP0oOw7/F2Ym66IdYYIp2HeEZv+jFwa" +
			"l41w5W4BH/gtbwGjFQ6CvF/m+lfUv6ZZdzsMIeEOwhP5g7rXBxrbcnGBaU+PXbho" +
			"gjDqaYzAWGlrmAd6aPSj51AGeYXkb2T1T/yoJ++M3GvhH4C4hvitamDkksh/qRnM" +
			"M/s8Nku6z1+RXO3M6p5QC1nlAVqieU8esT43945eSoC77K8WyujDNbysDyUCUTzt" +
			"p/aoQwe/HgkeOTJNelKR9y2W3xinZLFzep0SqpNI/e468yB/2/LGsykIyQa7JX6r" +
			"BYwuBAIDAkOKfv5rK8v0YDfnN+eFqwhTcrfBj5rDH7hER6nW3lNWcMataUiHEaMg" +
			"o6Q0OO1vptIGxW8jClTD4N1sCNwNu9vKny8dKYDDHbCjE06DNTv7XYVW3+JqTL5E" +
			"BnidvGgOmA==");

		/**
        * decrypt the passed in message stream
        */
        private byte[] DecryptMessage(
            byte[] message)
        {
            PgpObjectFactory		pgpF = new PgpObjectFactory(message);
            PgpEncryptedDataList	enc = (PgpEncryptedDataList) pgpF.NextPgpObject();
            PgpPbeEncryptedData		pbe = (PgpPbeEncryptedData) enc[0];
            Stream					clear = pbe.GetDataStream(pass);

			PgpObjectFactory		pgpFact = new PgpObjectFactory(clear);
            PgpCompressedData		cData = (PgpCompressedData) pgpFact.NextPgpObject();
            pgpFact = new PgpObjectFactory(cData.GetDataStream());

			PgpLiteralData ld = (PgpLiteralData) pgpFact.NextPgpObject();

            if (!ld.FileName.Equals("test.txt")
                && !ld.FileName.Equals("_CONSOLE"))
            {
                Fail("wrong filename in packet");
            }

			if (!ld.ModificationTime.Equals(TestDateTime))
			{
				Fail("wrong modification time in packet: " + ld.ModificationTime + " vs " + TestDateTime);
			}

			Stream unc = ld.GetInputStream();
			byte[] bytes = Streams.ReadAll(unc);

			if (pbe.IsIntegrityProtected() && !pbe.Verify())
			{
				Fail("integrity check failed");
			}

			return bytes;
        }

		private byte[] DecryptMessageBuffered(
			byte[] message)
		{
			PgpObjectFactory		pgpF = new PgpObjectFactory(message);
			PgpEncryptedDataList	enc = (PgpEncryptedDataList) pgpF.NextPgpObject();
			PgpPbeEncryptedData		pbe = (PgpPbeEncryptedData) enc[0];

			Stream clear = pbe.GetDataStream(pass);

			PgpObjectFactory	pgpFact = new PgpObjectFactory(clear);
			PgpCompressedData	cData = (PgpCompressedData) pgpFact.NextPgpObject();

			pgpFact = new PgpObjectFactory(cData.GetDataStream());

			PgpLiteralData ld = (PgpLiteralData) pgpFact.NextPgpObject();

			MemoryStream bOut = new MemoryStream();
			if (!ld.FileName.Equals("test.txt")
				&& !ld.FileName.Equals("_CONSOLE"))
			{
				Fail("wrong filename in packet");
			}
			if (!ld.ModificationTime.Equals(TestDateTime))
			{
				Fail("wrong modification time in packet: " + ld.ModificationTime.Ticks + " " + TestDateTime.Ticks);
			}

			Stream unc = ld.GetInputStream();
			byte[] buf = new byte[1024];

			int len;
			while ((len = unc.Read(buf, 0, buf.Length)) > 0)
			{
				bOut.Write(buf, 0, len);
			}

			if (pbe.IsIntegrityProtected() && !pbe.Verify())
			{
				Fail("integrity check failed");
			}

			return bOut.ToArray();
		}

		public override void PerformTest()
        {
            byte[] data = DecryptMessage(enc1);
            if (data[0] != 'h' || data[1] != 'e' || data[2] != 'l')
            {
                Fail("wrong plain text in packet");
            }

			//
            // create a PBE encrypted message and read it back.
            //
			byte[] text = Encoding.ASCII.GetBytes("hello world!\n");

			//
            // encryption step - convert to literal data, compress, encode.
            //
            MemoryStream bOut = new UncloseableMemoryStream();

            PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(
                CompressionAlgorithmTag.Zip);

            PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
			Stream comOut = comData.Open(new UncloseableStream(bOut));
            Stream ldOut = lData.Open(
				new UncloseableStream(comOut),
                PgpLiteralData.Binary,
                PgpLiteralData.Console,
                text.Length,
                TestDateTime);

			ldOut.Write(text, 0, text.Length);
			ldOut.Close();

			comOut.Close();

			//
            // encrypt - with stream close
            //
            MemoryStream cbOut = new UncloseableMemoryStream();
            PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(
				SymmetricKeyAlgorithmTag.Cast5, new SecureRandom());

            cPk.AddMethod(pass, HashAlgorithmTag.Sha1);

			byte[] bOutData = bOut.ToArray();
			Stream cOut = cPk.Open(new UncloseableStream(cbOut), bOutData.Length);
            cOut.Write(bOutData, 0, bOutData.Length);
            cOut.Close();

			data = DecryptMessage(cbOut.ToArray());
            if (!Arrays.AreEqual(data, text))
            {
                Fail("wrong plain text in generated packet");
            }

			//
			// encrypt - with generator close
			//
			cbOut = new UncloseableMemoryStream();
			cPk = new PgpEncryptedDataGenerator(
				SymmetricKeyAlgorithmTag.Cast5, new SecureRandom());

            cPk.AddMethod(pass, HashAlgorithmTag.Sha1);

			bOutData = bOut.ToArray();
			cOut = cPk.Open(new UncloseableStream(cbOut), bOutData.Length);
			cOut.Write(bOutData, 0, bOutData.Length);

			cPk.Close();

			data = DecryptMessage(cbOut.ToArray());

			if (!AreEqual(data, text))
			{
				Fail("wrong plain text in generated packet");
			}

			//
            // encrypt - partial packet style.
            //
            SecureRandom rand = new SecureRandom();
            byte[] test = new byte[1233];

            rand.NextBytes(test);

			bOut = new UncloseableMemoryStream();

			comData = new PgpCompressedDataGenerator(
				CompressionAlgorithmTag.Zip);
			comOut = comData.Open(new UncloseableStream(bOut));

			lData = new PgpLiteralDataGenerator();
            ldOut = lData.Open(
				new UncloseableStream(comOut),
                PgpLiteralData.Binary,
                PgpLiteralData.Console,
                TestDateTime,
                new byte[16]);

            ldOut.Write(test, 0, test.Length);
            lData.Close();

			comData.Close();
            cbOut = new UncloseableMemoryStream();
            cPk = new PgpEncryptedDataGenerator(
				SymmetricKeyAlgorithmTag.Cast5, rand);

            cPk.AddMethod(pass, HashAlgorithmTag.Sha1);

			cOut = cPk.Open(new UncloseableStream(cbOut), new byte[16]);
            {
                byte[] tmp = bOut.ToArray();
                cOut.Write(tmp, 0, tmp.Length);
            }

			cPk.Close();

			data = DecryptMessage(cbOut.ToArray());
            if (!Arrays.AreEqual(data, test))
            {
                Fail("wrong plain text in generated packet");
            }

            //
            // with integrity packet
            //
            cbOut = new UncloseableMemoryStream();
            cPk = new PgpEncryptedDataGenerator(
				SymmetricKeyAlgorithmTag.Cast5, true, rand);

            cPk.AddMethod(pass, HashAlgorithmTag.Sha1);

            cOut = cPk.Open(new UncloseableStream(cbOut), new byte[16]);
            bOutData = bOut.ToArray();
            cOut.Write(bOutData, 0, bOutData.Length);
            cPk.Close();

			data = DecryptMessage(cbOut.ToArray());
            if (!Arrays.AreEqual(data, test))
            {
                Fail("wrong plain text in generated packet");
            }

			//
			// decrypt with buffering
			//
			data = DecryptMessageBuffered(cbOut.ToArray());
			if (!AreEqual(data, test))
			{
				Fail("wrong plain text in buffer generated packet");
			}

			//
			// sample message
			//
			PgpObjectFactory pgpFact = new PgpObjectFactory(testPBEAsym);

			PgpEncryptedDataList enc = (PgpEncryptedDataList)pgpFact.NextPgpObject();

			PgpPbeEncryptedData pbe = (PgpPbeEncryptedData) enc[1];

			Stream clear = pbe.GetDataStream("password".ToCharArray());

			pgpFact = new PgpObjectFactory(clear);

			PgpLiteralData ld = (PgpLiteralData) pgpFact.NextPgpObject();

			Stream unc = ld.GetInputStream();
			byte[] bytes = Streams.ReadAll(unc);

			if (!AreEqual(bytes, Hex.Decode("5361742031302e30322e30370d0a")))
			{
				Fail("data mismatch on combined PBE");
			}

			//
			// with integrity packet - one byte message
			//
			byte[] msg = new byte[1];
			bOut = new MemoryStream();

			comData = new PgpCompressedDataGenerator(
				CompressionAlgorithmTag.Zip);

			lData = new PgpLiteralDataGenerator();
			comOut = comData.Open(new UncloseableStream(bOut));
			ldOut = lData.Open(
				new UncloseableStream(comOut),
				PgpLiteralData.Binary,
				PgpLiteralData.Console,
				msg.Length,
				TestDateTime);

			ldOut.Write(msg, 0, msg.Length);

			ldOut.Close();

			comOut.Close();
        
			cbOut = new MemoryStream();
			cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, true, rand);

            cPk.AddMethod(pass, HashAlgorithmTag.Sha1);

			cOut = cPk.Open(new UncloseableStream(cbOut), new byte[16]);

			data = bOut.ToArray();
			cOut.Write(data, 0, data.Length);

			cOut.Close();

			data = DecryptMessage(cbOut.ToArray());
			if (!AreEqual(data, msg))
			{
				Fail("wrong plain text in generated packet");
			}

			//
			// decrypt with buffering
			//
			data = DecryptMessageBuffered(cbOut.ToArray());
			if (!AreEqual(data, msg))
			{
				Fail("wrong plain text in buffer generated packet");
			}
		}

		private class UncloseableMemoryStream
			: MemoryStream
		{
			public override void Close()
			{
				throw new Exception("Close() called on underlying stream");
			}
		}

		public override string Name
        {
			get { return "PGPPBETest"; }
        }

		public static void Main(
			string[] args)
        {
			RunTest(new PgpPbeTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
