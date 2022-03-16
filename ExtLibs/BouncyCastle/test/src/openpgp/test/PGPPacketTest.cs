using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
    [TestFixture]
    public class PgpPacketTest
        : SimpleTest
    {
        private static int MAX = 32000;

        private void ReadBackTest(
            PgpLiteralDataGenerator generator)
        {
            Random rand = new Random();
            byte[] buf = new byte[MAX];

            rand.NextBytes(buf);

            for (int i = 1; i != MAX; i++)
            {
                MemoryStream bOut = new MemoryStream();

                Stream outputStream = generator.Open(
					new UncloseableStream(bOut),
					PgpLiteralData.Binary,
					PgpLiteralData.Console,
					i,
					DateTime.UtcNow);

				outputStream.Write(buf, 0, i);

                generator.Close();

                PgpObjectFactory fact = new PgpObjectFactory(bOut.ToArray());

                PgpLiteralData data = (PgpLiteralData)fact.NextPgpObject();

                Stream inputStream = data.GetInputStream();

                for (int count = 0; count != i; count++)
                {
                    if (inputStream.ReadByte() != (buf[count] & 0xff))
                    {
                        Fail("failed readback test - length = " + i);
                    }
                }
            }
        }

        public override void PerformTest()
        {
            ReadBackTest(new PgpLiteralDataGenerator(true));
			ReadBackTest(new PgpLiteralDataGenerator(false));
        }

		public override string Name
        {
			get { return "PGPPacketTest"; }
        }

		public static void Main(
            string[] args)
        {
			RunTest(new PgpPacketTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
