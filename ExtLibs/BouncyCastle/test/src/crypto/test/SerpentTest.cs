using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /**
     * Test vectors based on the NESSIE submission
     */
    [TestFixture]
    public class SerpentTest
        :   CipherTest
    {
        static SimpleTest[] tests = 
        {
            new BlockCipherVectorTest(0, new SerpentEngine(),
                new KeyParameter(Hex.Decode("00000000000000000000000000000000")),
                "00000000000000000000000000000000", "3620b17ae6a993d09618b8768266bae9"),
            new BlockCipherVectorTest(1, new SerpentEngine(),
                new KeyParameter(Hex.Decode("80000000000000000000000000000000")),
                "00000000000000000000000000000000", "264E5481EFF42A4606ABDA06C0BFDA3D"),
            new BlockCipherVectorTest(2, new SerpentEngine(),
                new KeyParameter(Hex.Decode("D9D9D9D9D9D9D9D9D9D9D9D9D9D9D9D9")),
                "D9D9D9D9D9D9D9D9D9D9D9D9D9D9D9D9", "20EA07F19C8E93FDA30F6B822AD5D486"),
            new BlockCipherVectorTest(3, new SerpentEngine(),
                new KeyParameter(Hex.Decode("000000000000000000000000000000000000000000008000")),
                "00000000000000000000000000000000", "40520018C4AC2BBA285AEEB9BCB58755"),
            new BlockCipherVectorTest(4, new SerpentEngine(),
                new KeyParameter(Hex.Decode("0000000000000000000000000000000000000000000000000000000000000000")),
                "00000000000000000000000000000001", "AD86DE83231C3203A86AE33B721EAA9F"),
            new BlockCipherVectorTest(5, new SerpentEngine(),
                new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F")),
                "3DA46FFA6F4D6F30CD258333E5A61369", "00112233445566778899AABBCCDDEEFF"),
            new BlockCipherVectorTest(6, new SerpentEngine(),
                new KeyParameter(Hex.Decode("2BD6459F82C5B300952C49104881FF482BD6459F82C5B300952C49104881FF48")),
                "677C8DFAA08071743FD2B415D1B28AF2", "EA024714AD5C4D84EA024714AD5C4D84"),
            new BlockCipherVectorTest(7, new SerpentEngine(),
                new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F1011121314151617")),
                "4528CACCB954D450655E8CFD71CBFAC7", "00112233445566778899AABBCCDDEEFF"),
            new BlockCipherVectorTest(8, new SerpentEngine(),
                new KeyParameter(Hex.Decode("2BD6459F82C5B300952C49104881FF482BD6459F82C5B300")),
                "E0208BE278E21420C4B1B9747788A954", "EA024714AD5C4D84EA024714AD5C4D84"),
            new BlockCipherVectorTest(9, new SerpentEngine(),
                new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F")),
                "33B3DC87EDDD9B0F6A1F407D14919365", "00112233445566778899AABBCCDDEEFF"),
            new BlockCipherVectorTest(10, new SerpentEngine(),
                new KeyParameter(Hex.Decode("2BD6459F82C5B300952C49104881FF48")),
                "BEB6C069393822D3BE73FF30525EC43E", "EA024714AD5C4D84EA024714AD5C4D84"),
            new BlockCipherMonteCarloTest(20, 100, new SerpentEngine(),
                new KeyParameter(Hex.Decode("F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3")),
                "F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3", "8FD0E58DB7A54B929FCA6A12F96F20AF"),
            new BlockCipherMonteCarloTest(21, 100, new SerpentEngine(),
                new KeyParameter(Hex.Decode("0004000000000000000000000000000000000000000000000000000000000000")),
                "00000000000000000000000000000000", "E7B681E8871FD05FEAE5FB64DA891EA2"),
            new BlockCipherMonteCarloTest(22, 100, new SerpentEngine(),
                new KeyParameter(Hex.Decode("0000000020000000000000000000000000000000000000000000000000000000")),
                "00000000000000000000000000000000", "C5545D516EEC73BFA3622A8194F95620"),
            new BlockCipherMonteCarloTest(23, 100, new SerpentEngine(),
                new KeyParameter(Hex.Decode("0000000000000000000000000000000000000000000000000000000002000000")),
                "00000000000000000000000000000000", "11FF5C9BE006F82C98BD4FAC1A19920E"),
            new BlockCipherMonteCarloTest(24, 100, new SerpentEngine(),
                new KeyParameter(Hex.Decode("0000000000000000000000000000000000000000000000000000000000000000")),
                "00000000000000000000000000010000", "47CA1CA404B6481CAD4C21C8A0415A0E"),
            new BlockCipherMonteCarloTest(25, 100, new SerpentEngine(),
                new KeyParameter(Hex.Decode("0000000000000000000000000000000000000000000000000000000000000000")),
                "00000000000000008000000000000000", "A0A2D5B07E27D539CA5BEE9DE1EAB3E6")
        };

        public SerpentTest()
            : base(tests, new SerpentEngine(), new KeyParameter(new byte[32]))
        {
        }

        public override void PerformTest()
        {
            base.PerformTest();

            //DoCbcMonte(new byte[16], new byte[16], new byte[16], Hex.Decode("9ea101ecebaa41c712bcb0d9bab3e2e4"));
            //DoCbcMonte(Hex.Decode("9ea101ecebaa41c712bcb0d9bab3e2e4"), Hex.Decode("9ea101ecebaa41c712bcb0d9bab3e2e4"), Hex.Decode("b4813d8a66244188b9e92c75913fa2f4"), Hex.Decode("f86b2c265b9c75869f31e2c684c13e9f"));

            DoCbc(Hex.Decode("BE4295539F6BD1752FD0A80229EF8847"), Hex.Decode("00963F59224794D5AD4252094358FBC3"), Strings.ToByteArray("CBC Mode Test"), Hex.Decode("CF2CF2547E02F6D34D97246E8042ED89"));


            DoEax(Hex.Decode("7494A57648FB420043BFBFC5639EB82D"), Hex.Decode("6DF94638B83E01458F3E30C9A1D6AF1C"), Strings.ToByteArray("EAX Mode Test"), new byte[0], 128, Hex.Decode("96C521F32DC5E9BBC369DDE4914CB13B710EEBBAB7D706D3ABE06A99DC"));
        }

        private void DoEax(byte[] key, byte[] iv, byte[] pt, byte[] aad, int tagLength, byte[] expected)
        {
            EaxBlockCipher c = new EaxBlockCipher(new SerpentEngine());

            c.Init(true, new AeadParameters(new KeyParameter(key), tagLength, iv, aad));

            byte[] output = new byte[expected.Length];

            int len = c.ProcessBytes(pt, 0, pt.Length, output, 0);

            c.DoFinal(output, len);

            if (!Arrays.AreEqual(expected, output))
            {
                Fail("EAX test failed");
            }
        }

        private void DoCbc(byte[] key, byte[] iv, byte[] pt, byte[] expected)
        {
            PaddedBufferedBlockCipher c = new PaddedBufferedBlockCipher(new CbcBlockCipher(new SerpentEngine()), new Pkcs7Padding());

            byte[] ct = new byte[expected.Length];

            c.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

            int l = c.ProcessBytes(pt, 0, pt.Length, ct, 0);

            c.DoFinal(ct, l);

            if (!Arrays.AreEqual(expected, ct))
            {
                Fail("CBC test failed");
            }
        }

        public override string Name
        {
			get { return "Serpent"; }
        }

        public static void Main(string[] args)
        {
            RunTest(new SerpentTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
