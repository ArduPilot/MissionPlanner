using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
	[TestFixture]
	public class PgpCompressionTest
		: SimpleTest
	{
		private static readonly byte[] Data = Encoding.ASCII.GetBytes("hello world! !dlrow olleh");

		[Test]
		public void TestUncompressed()
		{
			doTestCompression(CompressionAlgorithmTag.Uncompressed);
		}

		[Test]
		public void TestZip()
		{
			doTestCompression(CompressionAlgorithmTag.Zip);
		}

		[Test]
		public void TestZLib()
		{
			doTestCompression(CompressionAlgorithmTag.ZLib);
		}

		[Test]
		public void TestBZip2()
		{
			doTestCompression(CompressionAlgorithmTag.BZip2);
		}

		public override void PerformTest()
		{
			doTestCompression(CompressionAlgorithmTag.Uncompressed);
			doTestCompression(CompressionAlgorithmTag.Zip);
			doTestCompression(CompressionAlgorithmTag.ZLib);
			doTestCompression(CompressionAlgorithmTag.BZip2);
		}

		private void doTestCompression(
			CompressionAlgorithmTag type)
		{
			doTestCompression(type, true);
			doTestCompression(type, false);
		}

		private void doTestCompression(
			CompressionAlgorithmTag	type,
			bool					streamClose)
		{
			MemoryStream bOut = new MemoryStream();
			PgpCompressedDataGenerator cPacket = new PgpCompressedDataGenerator(type);
			Stream os = cPacket.Open(new UncloseableStream(bOut), new byte[Data.Length - 1]);
			os.Write(Data, 0, Data.Length);

			if (streamClose)
			{
				os.Close();
			}
			else
			{
				cPacket.Close();
			}

			ValidateData(bOut.ToArray());

			try
			{
				os.Close();
				cPacket.Close();
			}
			catch (Exception)
			{
				Fail("Redundant Close() should be ignored");
			}
		}

		private void ValidateData(
			byte[] compressed)
		{
			PgpObjectFactory pgpFact = new PgpObjectFactory(compressed);
			PgpCompressedData c1 = (PgpCompressedData) pgpFact.NextPgpObject();

			Stream pIn = c1.GetDataStream();
			byte[] bytes = Streams.ReadAll(pIn);
			pIn.Close();

			if (!AreEqual(bytes, Data))
			{
				Fail("compression test failed");
			}
		}

		public override string Name
		{
			get { return "PGPCompressionTest"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new PgpCompressionTest());
		}
	}
}
