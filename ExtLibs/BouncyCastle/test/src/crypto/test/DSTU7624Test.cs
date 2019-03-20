using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tests
{
    [TestFixture]
    public class Dstu7624Test : CipherTest
    {
        private static SecureRandom Random = new SecureRandom();

        public Dstu7624Test()
            : base(tests, new Dstu7624Engine(256), new KeyParameter(new byte[32]))
        {
        }

        internal static SimpleTest[] tests = new SimpleTest[]
        {
            //ECB mode
            new BlockCipherVectorTest(0, new Dstu7624Engine(128), new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F")), "101112131415161718191A1B1C1D1E1F", "81BF1C7D779BAC20E1C9EA39B4D2AD06"),
            new BlockCipherVectorTest(1, new Dstu7624Engine(128), new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F")), "202122232425262728292A2B2C2D2E2F", "58EC3E091000158A1148F7166F334F14"),
            new BlockCipherVectorTest(2, new Dstu7624Engine(256), new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F")), "202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F", "F66E3D570EC92135AEDAE323DCBD2A8CA03963EC206A0D5A88385C24617FD92C"),
            new BlockCipherVectorTest(3, new Dstu7624Engine(256), new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F")), "404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F", "606990E9E6B7B67A4BD6D893D72268B78E02C83C3CD7E102FD2E74A8FDFE5DD9"),
            new BlockCipherVectorTest(4, new Dstu7624Engine(512), new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F")), "404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F", "4A26E31B811C356AA61DD6CA0596231A67BA8354AA47F3A13E1DEEC320EB56B895D0F417175BAB662FD6F134BB15C86CCB906A26856EFEB7C5BC6472940DD9D9"),

            //CBC mode (PADDING NOT SUPPORTED)
            new BlockCipherVectorTest(14, new CbcBlockCipher(new Dstu7624Engine(128)), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F")), Hex.Decode("101112131415161718191A1B1C1D1E1F")), "202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F", "A73625D7BE994E85469A9FAABCEDAAB6DBC5F65DD77BB35E06BD7D1D8EAFC8624D6CB31CE189C82B8979F2936DE9BF14"),
            new BlockCipherVectorTest(15, new CbcBlockCipher(new Dstu7624Engine(128)), new ParametersWithIV(new KeyParameter(Hex.Decode("0F0E0D0C0B0A09080706050403020100")), Hex.Decode("1F1E1D1C1B1A19181716151413121110")), "88F2F048BA696170E3818915E0DBC0AFA6F141FEBC2F817138DA4AAB2DBF9CE490A488C9C82AC83FB0A6C0EEB64CFD22", "4F4E4D4C4B4A494847464544434241403F3E3D3C3B3A393837363534333231302F2E2D2C2B2A29282726252423222120"),
            new BlockCipherVectorTest(16, new CbcBlockCipher(new Dstu7624Engine(128)), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F")), Hex.Decode("202122232425262728292A2B2C2D2E2F")), "303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D8000", "13EA15843AD14C50BC03ECEF1F43E398E4217752D3EB046AC393DACC5CA1D6FA0EB9FCEB229362B4F1565527EE3D8433"),
            new BlockCipherVectorTest(17, new CbcBlockCipher(new Dstu7624Engine(128)), new ParametersWithIV(new KeyParameter(Hex.Decode("1F1E1D1C1B1A191817161514131211100F0E0D0C0B0A09080706050403020100")), Hex.Decode("2F2E2D2C2B2A29282726252423222120")), "BC8F026FC603ECE05C24FDE87542730999B381870882AC0535D4368C4BABD81B884E96E853EE7E055262D9D204FBE212", "5F5E5D5C5B5A595857565554535251504F4E4D4C4B4A494847464544434241403F3E3D3C3B3A39383736353433323130"),
            new BlockCipherVectorTest(18, new CbcBlockCipher(new Dstu7624Engine(256)), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F")), Hex.Decode("202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F")), "404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9F", "9CDFDAA75929E7C2A5CFC1BF16B42C5AE3886D0258E8C577DC01DAF62D185FB999B9867736B87110F5F1BC7481912C593F48FF79E2AFDFAB9F704A277EC3E557B1B0A9F223DAE6ED5AF591C4F2D6FB22E48334F5E9B96B1A2EA5200F30A406CE"),
            new BlockCipherVectorTest(19, new CbcBlockCipher(new Dstu7624Engine(256)), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F")), Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F")), "606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9FA0A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBF", "B8A2474578C2FEBF3F94703587BD5FDC3F4A4D2F43575B6144A1E1031FB3D1452B7FD52F5E3411461DAC506869FF8D2FAEF4FEE60379AE00B33AA3EAF911645AF8091CD8A45D141D1FB150E5A01C1F26FF3DBD26AC4225EC7577B2CE57A5B0FF"),
            new BlockCipherVectorTest(20, new CbcBlockCipher(new Dstu7624Engine(256)), new ParametersWithIV(new KeyParameter(Hex.Decode("3F3E3D3C3B3A393837363534333231302F2E2D2C2B2A292827262524232221201F1E1D1C1B1A191817161514131211100F0E0D0C0B0A09080706050403020100")), Hex.Decode("5F5E5D5C5B5A595857565554535251504F4E4D4C4B4A49484746454443424140")), "C69A59E10D00F087319B62288A57417C074EAD07C732A87055F0A5AD2BB288105705C45E091A9A6726E9672DC7D8C76FC45C782BCFEF7C39D94DEB84B17035BC8651255A0D34373451B6E1A2C827DB97566C9FF5506C5579F982A0EFC5BA7C28", "BFBEBDBCBBBAB9B8B7B6B5B4B3B2B1B0AFAeadACABAAA9A8A7A6A5A4A3A2A1A09F9E9D9C9B9A999897969594939291908F8E8D8C8B8A898887868584838281807F7E7D7C7B7A797877767574737271706F6E6D6C6B6A69686766656463626160"),
            new BlockCipherVectorTest(21, new CbcBlockCipher(new Dstu7624Engine(512)), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F")), Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F")), "808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9FA0A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBFC0C1C2C3C4C5C6C7C8C9CACBCCCDCECFD0D1D2D3D4D5D6D7D8D9DADBDCDDDEDFE0E1E2E3E4E5E6E7E8E9EAEBECEDEEEFF0F1F2F3F4F5F6F7F8F9FAFBFCFDFEFF", "D4739B829EF901B24C1162AE4FDEF897EDA41FAC7F5770CDC90E1D1CDF124E8D7831E06B4498A4B6F6EC815DF2461DC99BB0449B0F09FCAA2C84090534BCC9329626FD74EF8F0A0BCB5765184629C3CBF53B0FB134F6D0421174B1C4E884D1CD1069A7AD19752DCEBF655842E79B7858BDE01390A760D85E88925BFE38B0FA57"),
            new BlockCipherVectorTest(22, new CbcBlockCipher(new Dstu7624Engine(512)), new ParametersWithIV(new KeyParameter(Hex.Decode("3F3E3D3C3B3A393837363534333231302F2E2D2C2B2A292827262524232221201F1E1D1C1B1A191817161514131211100F0E0D0C0B0A09080706050403020100")), Hex.Decode("7F7E7D7C7B7A797877767574737271706F6E6D6C6B6A696867666564636261605F5E5D5C5B5A595857565554535251504F4E4D4C4B4A49484746454443424140")), "5D5B3E3DE5BAA70E0A0684D458856CE759C6018D0B3F087FC1DAC101D380236DD934F2880B02D56A575BCA35A0CE4B0D9BA1F4A39C16CA7D80D59956630F09E54EC91E32B6830FE08323ED393F8028D150BF03CAD0629A5AFEEFF6E44257980618DB2F32B7B2B65B96E8451F1090829D2FFFC615CC1581E9221438DCEAD1FD12", "FFFEFDFCFBFAF9F8F7F6F5F4F3F2F1F0EFEEEDECEBEAE9E8E7E6E5E4E3E2E1E0DFDEDDDCDBDAD9D8D7D6D5D4D3D2D1D0CFCECDCCCBCAC9C8C7C6C5C4C3C2C1C0BFBEBDBCBBBAB9B8B7B6B5B4B3B2B1B0AFAeadACABAAA9A8A7A6A5A4A3A2A1A09F9E9D9C9B9A999897969594939291908F8E8D8C8B8A89888786858483828180"),

            //CFB mode
            new BlockCipherVectorTest(14, new CfbBlockCipher(new Dstu7624Engine(128), 128), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F")), Hex.Decode("101112131415161718191A1B1C1D1E1F")), "202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F", "A19E3E5E53BE8A07C9E0C01298FF83291F8EE6212110BE3FA5C72C88A082520B265570FE28680719D9B4465E169BC37A"),

            //OFB mode
            new BlockCipherVectorTest(23, new OfbBlockCipher(new Dstu7624Engine(128), 128), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F")), Hex.Decode("101112131415161718191A1B1C1D1E1F")), "202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F", "A19E3E5E53BE8A07C9E0C01298FF832953205C661BD85A51F3A94113BC785CAB634B36E89A8FDD16A12E4467F5CC5A26"),
            new BlockCipherVectorTest(24, new OfbBlockCipher(new Dstu7624Engine(128), 128), new ParametersWithIV(new KeyParameter(Hex.Decode("0F0E0D0C0B0A09080706050403020100")), Hex.Decode("1F1E1D1C1B1A19181716151413121110")), "649A1EAAE160AF20F5B3EF2F58D66C1178B82E00D26F30689C8EC22E8E86E9CBB0BD4FFEE39EB13C2311276A906DD636", "4F4E4D4C4B4A494847464544434241403F3E3D3C3B3A393837363534333231302F2E2D2C2B2A29282726252423222120"),
            new BlockCipherVectorTest(25, new OfbBlockCipher(new Dstu7624Engine(128), 128), new ParametersWithIV(new KeyParameter(Hex.Decode("1F1E1D1C1B1A191817161514131211100F0E0D0C0B0A09080706050403020100")), Hex.Decode("2F2E2D2C2B2A29282726252423222120")), "1A66CFBFEC00C6D52E39923E858DD64B214AB787798D3D5059A6B498AD66B34EAC48C4074BEC0D98C6", "5F5E5D5C5B5A595857565554535251504F4E4D4C4B4A494847464544434241403F3E3D3C3B3A393837"),
            new BlockCipherVectorTest(26, new OfbBlockCipher(new Dstu7624Engine(256), 256), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F")), Hex.Decode("202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F")), "404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F90", "B62F7F144A8C6772E693A96890F064C3F06831BF743F5B0DD061067F3D22877331AA6A99D939F05B7550E9402BD1615CC7B2D4A167E83EC0D8A894F92C72E176F3880B61C311D69CE1210C59184E818E19"),
            new BlockCipherVectorTest(27, new OfbBlockCipher(new Dstu7624Engine(256), 256), new ParametersWithIV(new KeyParameter(Hex.Decode("1F1E1D1C1B1A191817161514131211100F0E0D0C0B0A09080706050403020100")), Hex.Decode("3F3E3D3C3B3A393837363534333231302F2E2D2C2B2A29282726252423222120")), "7758A939DD6BD00CAF9153E5A5D5A66129105CA1EA54A97C06FA4A40960A068F55E34F9339A14436216948F92FA2FB5286D3AB1E81543FC0018A0C4E8C493475F4D35DCFB0A7A5377F6669B857CDC978E4", "9F9E9D9C9B9A999897969594939291908F8E8D8C8B8A898887868584838281807F7E7D7C7B7A797877767574737271706F6E6D6C6B6A696867666564636261605F5E5D5C5B5A595857565554535251504F"),
            new BlockCipherVectorTest(28, new OfbBlockCipher(new Dstu7624Engine(256), 256), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F")), Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F")), "606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9FA0A1A2A3A4A5A6A7A8A9AAABACADAEAFB0", "0008F28A82D2D01D23BFB2F8BB4F06D8FE73BA4F48A2977585570ED3818323A668883C9DCFF610CC7E3EA5C025FBBC5CA6520F8F11CA35CEB9B07031E6DBFABE39001E9A3CC0A24BBC565939592B4DEDBD"),
            new BlockCipherVectorTest(29, new OfbBlockCipher(new Dstu7624Engine(256), 256), new ParametersWithIV(new KeyParameter(Hex.Decode("3F3E3D3C3B3A393837363534333231302F2E2D2C2B2A292827262524232221201F1E1D1C1B1A191817161514131211100F0E0D0C0B0A09080706050403020100")), Hex.Decode("5F5E5D5C5B5A595857565554535251504F4E4D4C4B4A49484746454443424140")), "98E122708FDABB1B1A5765C396DC79D7573221EC486ADDABD1770B147A6DD00B5FBC4F1EC68C59775B7AAA4D43C4CCE4F396D982DF64D30B03EF6C3B997BA0ED940BBC590BD30D64B5AE207147D71086B5", "BFBEBDBCBBBAB9B8B7B6B5B4B3B2B1B0AFAeadACABAAA9A8A7A6A5A4A3A2A1A09F9E9D9C9B9A999897969594939291908F8E8D8C8B8A898887868584838281807F7E7D7C7B7A797877767574737271706F"),
            new BlockCipherVectorTest(30, new OfbBlockCipher(new Dstu7624Engine(512), 512), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F")), Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F")), "808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9FA0A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBFC0C1C2C3C4C5C6C7C8C9CACBCCCDCECFD0D1D2D3D4D5D6D7D8D9DADBDCDDDEDFE0", "CAA761980599B3ED2E945C41891BAD95F72B11C73ED26536A6847458BC76C827357156B4B3FE0DC1877F5B9F17B866C37B21D89531DB48007D05DEC928B06766C014BB9080385EDF0677E48A0A39B5E7489E28E82FFFD1F84694F17296CB701656"),
            new BlockCipherVectorTest(31, new OfbBlockCipher(new Dstu7624Engine(512), 512), new ParametersWithIV(new KeyParameter(Hex.Decode("3F3E3D3C3B3A393837363534333231302F2E2D2C2B2A292827262524232221201F1E1D1C1B1A191817161514131211100F0E0D0C0B0A09080706050403020100")), Hex.Decode("7F7E7D7C7B7A797877767574737271706F6E6D6C6B6A696867666564636261605F5E5D5C5B5A595857565554535251504F4E4D4C4B4A49484746454443424140")), "06C061A4A66DFC0910034B3CFBDC4206D8908241C56BF41C4103CFD6DF322210B87F57EAE9F9AD815E606A7D1E8E6BD7CB1EBFBDBCB085C2D06BF3CC1586CB2EE1D81D38437F425131321647E42F5DE309D33F25B89DE37124683E4B44824FC56D", "EFEEEDECEBEAE9E8E7E6E5E4E3E2E1E0DFDEDDDCDBDAD9D8D7D6D5D4D3D2D1D0CFCECDCCCBCAC9C8C7C6C5C4C3C2C1C0BFBEBDBCBBBAB9B8B7B6B5B4B3B2B1B0AFAeadACABAAA9A8A7A6A5A4A3A2A1A09F9E9D9C9B9A999897969594939291908F"),

            //CTR mode
            new BlockCipherVectorTest(24, new KCtrBlockCipher(new Dstu7624Engine(128)), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F")), Hex.Decode("101112131415161718191A1B1C1D1E1F")), "202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748", "A90A6B9780ABDFDFF64D14F5439E88F266DC50EDD341528DD5E698E2F000CE21F872DAF9FE1811844A"),
            new BlockCipherVectorTest(25, new KCtrBlockCipher(new Dstu7624Engine(128)), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F")), Hex.Decode("101112131415161718191A1B1C1D1E1F")), "303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F", "B91A7B8790BBCFCFE65D04E5538E98E216AC209DA33122FDA596E8928070BE51"),
            new StreamCipherVectorTest(26, new KCtrBlockCipher(new Dstu7624Engine(128)), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F")), Hex.Decode("101112131415161718191A1B1C1D1E1F")), "202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748", "A90A6B9780ABDFDFF64D14F5439E88F266DC50EDD341528DD5E698E2F000CE21F872DAF9FE1811844A"),
            new StreamCipherVectorTest(27, new KCtrBlockCipher(new Dstu7624Engine(128)), new ParametersWithIV(new KeyParameter(Hex.Decode("000102030405060708090A0B0C0D0E0F")), Hex.Decode("101112131415161718191A1B1C1D1E1F")), "303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F", "B91A7B8790BBCFCFE65D04E5538E98E216AC209DA33122FDA596E8928070BE51")
        };

        public override ITestResult Perform()
        {
            ITestResult result = base.Perform();

            if (!result.IsSuccessful())
            {
                return result;
            }

            result = MacTests(); //Mac tests

            if (!result.IsSuccessful())
            {
                return result;
            }

            CCMModeTests();

            result = KeyWrapTests(); //Key wrapping tests

            if (!result.IsSuccessful())
            {
                return result;
            }

            return new SimpleTestResult(true, Name + ": Okay");
        }

        private ITestResult MacTests()
        {
            //MAC mode (PADDING NOT SUPPORTED)
            //test 1
            byte[] key = Hex.Decode("000102030405060708090A0B0C0D0E0F");

            byte[] authtext = Hex.Decode("202122232425262728292A2B2C2D2E2F" +
                                         "303132333435363738393A3B3C3D3E3F" +
                                         "404142434445464748494A4B4C4D4E4F");

            byte[] expectedMac = Hex.Decode("123B4EAB8E63ECF3E645A99C1115E241");

            byte[] mac = new byte[128 / 8];

            Dstu7624Mac dstu7624Mac = new Dstu7624Mac(128, 128);
            dstu7624Mac.Init(new KeyParameter(key));
            dstu7624Mac.BlockUpdate(authtext, 0, authtext.Length);
            dstu7624Mac.DoFinal(mac, 0);

            if (!Arrays.AreEqual(mac, expectedMac))
            {
                return new SimpleTestResult(false, Name + ": Failed MAC test 1 - expected "
                     + Hex.ToHexString(expectedMac)
                     + " got " + Hex.ToHexString(mac));
            }


            //test 2
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F" +
                             "101112131415161718191A1B1C1D1E1F" +
                             "202122232425262728292A2B2C2D2E2F" +
                             "303132333435363738393A3B3C3D3E3F");

            authtext = Hex.Decode("404142434445464748494A4B4C4D4E4F" +
                                  "505152535455565758595A5B5C5D5E5F" +
                                  "606162636465666768696A6B6C6D6E6F" +
                                  "707172737475767778797A7B7C7D7E7F" +
                                  "808182838485868788898A8B8C8D8E8F" +
                                  "909192939495969798999A9B9C9D9E9F" +
                                  "A0A1A2A3A4A5A6A7A8A9AAABACADAEAF" +
                                  "B0B1B2B3B4B5B6B7B8B9BABBBCBDBEBF");

            expectedMac = Hex.Decode("7279FA6BC8EF7525B2B35260D00A1743");

            dstu7624Mac = new Dstu7624Mac(512, 128);
            dstu7624Mac.Init(new KeyParameter(key));
            dstu7624Mac.BlockUpdate(authtext, 0, authtext.Length);
            dstu7624Mac.DoFinal(mac, 0);

            if (!Arrays.AreEqual(mac, expectedMac))
            {
                return new SimpleTestResult(false, Name + ": Failed MAC test 2 - expected "
                     + Hex.ToHexString(expectedMac)
                     + " got " + Hex.ToHexString(mac));
            }

            return new SimpleTestResult(true, Name + ": Okay");
        }

        private ITestResult KeyWrapTests()
        {
            //KW mode (PADDING NOT SUPPORTED)
            //test 1
            /*
             * Initial implementation had bugs handling offset and length correctly, so for
             * this first test case we embed the input inside a larger buffer.
             */
            byte[] textA = SecureRandom.GetNextBytes(Random, Random.Next(1, 64));
            byte[] textB = SecureRandom.GetNextBytes(Random, Random.Next(1, 64));
            byte[] textToWrap = Arrays.ConcatenateAll(textA, Hex.Decode("101112131415161718191A1B1C1D1E1F"), textB);

            byte[] key = Hex.Decode("000102030405060708090A0B0C0D0E0F");
            byte[] expectedWrappedText = Hex.Decode("1DC91DC6E52575F6DBED25ADDA95A1B6AD3E15056E489738972C199FB9EE2913");
            byte[] output = new byte[expectedWrappedText.Length];

            Dstu7624WrapEngine wrapper = new Dstu7624WrapEngine(128);
            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, textA.Length, textToWrap.Length - textA.Length - textB.Length);

            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 1 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }

            output = Arrays.ConcatenateAll(textB, output, textA);

            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(output, textB.Length, output.Length - textB.Length - textA.Length);

            byte[] expected = Arrays.CopyOfRange(textToWrap, textA.Length, textToWrap.Length - textB.Length);
            if (!Arrays.AreEqual(output, expected))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 1 - expected "
                     + Hex.ToHexString(expected)
                     + " got " + Hex.ToHexString(output));
            }

            //test 2
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F");
            textToWrap = Hex.Decode("101112131415161718191A1B1C1D1E1F20219000000000000000800000000000");
            expectedWrappedText = Hex.Decode("0EA983D6CE48484D51462C32CC61672210FCC44196ABE635BAF878FDB83E1A63114128585D49DB355C5819FD38039169");

            output = new byte[expectedWrappedText.Length];

            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, 0, textToWrap.Length);


            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 2 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }


            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(expectedWrappedText, 0, expectedWrappedText.Length);

            if (!Arrays.AreEqual(output, textToWrap))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 2 - expected "
                     + Hex.ToHexString(textToWrap)
                     + " got " + Hex.ToHexString(output));
            }


            //test 3
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F");
            textToWrap = Hex.Decode("202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F");
            expectedWrappedText = Hex.Decode("2D09A7C18E6A5A0816331EC27CEA596903F77EC8D63F3BDB73299DE7FD9F4558E05992B0B24B39E02EA496368E0841CC1E3FA44556A3048C5A6E9E335717D17D");

            output = new byte[expectedWrappedText.Length];

            wrapper = new Dstu7624WrapEngine(128);
            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, 0, textToWrap.Length);


            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 3 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }

            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(expectedWrappedText, 0, expectedWrappedText.Length);

            if (!Arrays.AreEqual(output, textToWrap))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 3 - expected "
                     + Hex.ToHexString(textToWrap)
                     + " got " + Hex.ToHexString(output));
            }

            //test 4
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F");
            textToWrap = Hex.Decode("202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464E8040000000000020");
            expectedWrappedText = Hex.Decode("37E3EECB91150C6FA04CFD19D6FC57B7168C9FA5C5ED18601C68EE4AFD7301F8C8C51D7A0A5CD34F6FAB0D8AF11845CC1E4B16E0489FDA1D76BA4EFCFD161F76");

            output = new byte[expectedWrappedText.Length];

            wrapper = new Dstu7624WrapEngine(128);
            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, 0, textToWrap.Length);


            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 4 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }

            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(expectedWrappedText, 0, expectedWrappedText.Length);

            if (!Arrays.AreEqual(output, textToWrap))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 4 - expected "
                     + Hex.ToHexString(textToWrap)
                     + " got " + Hex.ToHexString(output));
            }

            //test 5
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F");
            textToWrap = Hex.Decode("202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F");
            expectedWrappedText = Hex.Decode("BE59D3C3C31B2685A8FA57CD000727F16AF303F0D87BC2D7ABD80DC2796BBC4CDBC4E0408943AF4DAF7DE9084DC81BFEF15FDCDD0DF399983DF69BF730D7AE2A199CA4F878E4723B7171DD4D1E8DF59C0F25FA0C20946BA64F9037D724BB1D50B6C2BD9788B2AF83EF6163087CD2D4488BC19F3A858D813E3A8947A529B6D65D");

            output = new byte[expectedWrappedText.Length];

            wrapper = new Dstu7624WrapEngine(256);
            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, 0, textToWrap.Length);


            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 5 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }

            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(expectedWrappedText, 0, expectedWrappedText.Length);

            if (!Arrays.AreEqual(output, textToWrap))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 5 - expected "
                     + Hex.ToHexString(textToWrap)
                     + " got " + Hex.ToHexString(output));
            }


            //test 6
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F");
            textToWrap = Hex.Decode("202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F708802000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000");
            expectedWrappedText = Hex.Decode("CC41D643B08592F509432E3C6F4B73156907A53B9FFB99B157DEC708F917AEA1E41D76475EDFB138A8B0220A152B673E9713DE7A2791E3573FE257C3FF3C0DAA9AD13477E52770F54CBF94D1603AED7CA876FB7913BC359D2B89562299FA92D32A9C17DBE4CC21CCE097089B9FBC245580D6DB59F8731D864B604E654397E5F5E7A79A6A777C75856039C8C86140D0CB359CA3923D902D08269F8D48E7F0F085");

            output = new byte[expectedWrappedText.Length];

            wrapper = new Dstu7624WrapEngine(256);
            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, 0, textToWrap.Length);


            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 6 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }

            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(expectedWrappedText, 0, expectedWrappedText.Length);

            if (!Arrays.AreEqual(output, textToWrap))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 6 - expected "
                     + Hex.ToHexString(textToWrap)
                     + " got " + Hex.ToHexString(output));
            }


            //test 7
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F");
            textToWrap = Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9F");
            expectedWrappedText = Hex.Decode("599217EB2B5270ECEF0BB716D70E251234A2451CE04FCFBAEEA92022C581F19B7C9386BB7476B4AD721D40778F49062C3605F1E8FAC9F3F3AC04E46E89E1844DBF4F18FA9303B288741ABD71013CF208F31B4C76FBE342F89B1ABFD97E830457555651B74D3CCDBF94CC5E5EEC22821536A96F44C8BC4346B0271303E67FD313");

            output = new byte[expectedWrappedText.Length];

            wrapper = new Dstu7624WrapEngine(256);
            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, 0, textToWrap.Length);


            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 7 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }

            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(expectedWrappedText, 0, expectedWrappedText.Length);

            if (!Arrays.AreEqual(output, textToWrap))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 7 - expected "
                     + Hex.ToHexString(textToWrap)
                     + " got " + Hex.ToHexString(output));
            }

            //test 8
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F");
            textToWrap = Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F908802000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000");
            expectedWrappedText = Hex.Decode("B92E58F53C38F7D23F1068FA98B921AC800AD0D1947BD620700D0B6088F87D03D6A516F54198154D0C71169C2BCF520F3DF3DF527FC23E800E9A65158D45BB253A3BD0493E4822DF0DB5A366BC2F47551C5D477DDDE724A0B869F562223CEDB9D4AA36C750FA864ADF938273FBC859F7D4930F6B70C6474304AB670BA32CB0C41023769338A29EA1555F526CDFEB75C72212CD2D29F4BA49C2A62ACBE4F3272B");

            output = new byte[expectedWrappedText.Length];

            wrapper = new Dstu7624WrapEngine(256);
            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, 0, textToWrap.Length);


            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 8 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }

            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(expectedWrappedText, 0, expectedWrappedText.Length);

            if (!Arrays.AreEqual(output, textToWrap))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 8 - expected "
                     + Hex.ToHexString(textToWrap)
                     + " got " + Hex.ToHexString(output));
            }

            //test 9
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F");
            textToWrap = Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9FA0A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBF");
            expectedWrappedText = Hex.Decode("9618AE6065069D5054464040F17337D58BEB51AE92391D740BDF7ABB239709C46270832039FF045BCF7878E7DA9C3B4CF89326CA8B4D29DB8680EEAE1B5A18463284713A323A69AEBF33CFC4B11283C7C8041FFC97668EDF727823411C9559816C108C11EC401643765527860D8DA0ED7254792C21DB775DEB1D6971C924CC83EB626173D894694943B1828ABDE8F9495BCEBA9AC3A4A03592C085AA29CC9A0C65786E631A702D589B819C89E79EEFF29C4EC312C8860BB68F02272EA770FB8D");

            output = new byte[expectedWrappedText.Length];

            wrapper = new Dstu7624WrapEngine(512);
            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, 0, textToWrap.Length);


            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 9 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }

            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(expectedWrappedText, 0, expectedWrappedText.Length);

            if (!Arrays.AreEqual(output, textToWrap))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 9 - expected "
                     + Hex.ToHexString(textToWrap)
                     + " got " + Hex.ToHexString(output));
            }


            //test 10
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F");
            textToWrap = Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9FA0A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBFC0C1C2C3C4C5C6C7C8C9CACBCCCDCECFD0D1D2D3D4D5D6D7D8D9DADBDCDDDEDFE00805000000000000000000000000000000000000000000000000000000000000800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            expectedWrappedText = Hex.Decode("3A05BB41513555F171E9234D4834EDAD16C0BAA6136197650138219C5DA406A703C39259E9DCCF6F2691EC691CE7414B5D3CDA006DE6D6C62142FAAA742C5F8AF64FCE95BE7ABA7FE5E06C3C33EE67BAEAB196E3A71132CAE78CD605A22E34D53CD159217E7B692CC79FAC66BF5E08DBC4FE274299474E176DDDF9F462AC63F4872E9B7F16B98AA56707EE5F2F94616CFC6A9548ADBD7DCB73664C331213964593F712ECCDFA7A94E3ABA7995176EA4B7E77096A3A3FF4E4087F430B62D5DEE64999F235FA9EAC79896A1C2258BF1DFC8A6AD0E5E7E06EAEEA0CCC2DEF62F67ECE8D12EFF432277C40A7BF1A23440B3533AF1E2F7AE1BBC076D12628BB4BC7B2E4D4B4353BCEAF9A67276B3FA23CADCA80062B95EBB2D51510AFA16F97249DF98E7B845C9A410F24B3C8B3E838E58D22BC2D14F46190FC1BFDB60C9691404F99");

            output = new byte[expectedWrappedText.Length];

            wrapper = new Dstu7624WrapEngine(512);
            wrapper.Init(true, new KeyParameter(key));
            output = wrapper.Wrap(textToWrap, 0, textToWrap.Length);


            if (!Arrays.AreEqual(output, expectedWrappedText))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (wrapping) test 10 - expected "
                     + Hex.ToHexString(expectedWrappedText)
                     + " got " + Hex.ToHexString(output));
            }

            wrapper.Init(false, new KeyParameter(key));
            output = wrapper.Unwrap(expectedWrappedText, 0, expectedWrappedText.Length);

            if (!Arrays.AreEqual(output, textToWrap))
            {
                return new SimpleTestResult(false, Name + ": Failed KW (unwrapping) test 10 - expected "
                     + Hex.ToHexString(textToWrap)
                     + " got " + Hex.ToHexString(output));
            }

            return new SimpleTestResult(true, Name + ": Okay");
        }

        private void CCMModeTests()
        {
            //test 1
            byte[] key = Hex.Decode("000102030405060708090a0b0c0d0e0f");
            byte[] iv = Hex.Decode("101112131415161718191a1b1c1d1e1f");
            byte[] input = Hex.Decode("303132333435363738393a3b3c3d3e3f");
            byte[] authText = Hex.Decode("202122232425262728292a2b2c2d2e2f");

            byte[] expectedMac = Hex.Decode("26a936173a4dc9160d6e3fda3a974060");
            byte[] expectedEncrypted = Hex.Decode("b91a7b8790bbcfcfe65d04e5538e98e2704454c9dd39adace0b19d03f6aab07e");

            byte[] mac;
            byte[] encrypted = new byte[expectedEncrypted.Length];

            byte[] decrypted = new byte[encrypted.Length];
            byte[] expectedDecrypted = new byte[input.Length + expectedMac.Length];
            Array.Copy(input, 0, expectedDecrypted, 0, input.Length);
            Array.Copy(expectedMac, 0, expectedDecrypted, input.Length, expectedMac.Length);
            int len;


            AeadParameters param = new AeadParameters(new KeyParameter(key), 128, iv);

            KCcmBlockCipher dstu7624ccm = new KCcmBlockCipher(new Dstu7624Engine(128));

            dstu7624ccm.Init(true, param);

            dstu7624ccm.ProcessAadBytes(authText, 0, authText.Length);

            len = dstu7624ccm.ProcessBytes(input, 0, input.Length, encrypted, 0);


            dstu7624ccm.DoFinal(encrypted, len);

            mac = dstu7624ccm.GetMac();

            if (!Arrays.AreEqual(mac, expectedMac))
            {
                Fail("Failed CCM mac test 1 - expected "
                    + Hex.ToHexString(expectedMac)
                    + " got " + Hex.ToHexString(mac));
            }

            if (!Arrays.AreEqual(encrypted, expectedEncrypted))
            {
                Fail("Failed CCM encrypt test 1 - expected "
                    + Hex.ToHexString(expectedEncrypted)
                    + " got " + Hex.ToHexString(encrypted));
            }

            dstu7624ccm.Init(false, param);

            dstu7624ccm.ProcessAadBytes(authText, 0, authText.Length);

            len = dstu7624ccm.ProcessBytes(expectedEncrypted, 0, expectedEncrypted.Length, decrypted, 0);

            dstu7624ccm.DoFinal(decrypted, len);

            if (!Arrays.AreEqual(decrypted, expectedDecrypted))
            {
                Fail("Failed CCM decrypt/verify mac test 1 - expected "
                    + Hex.ToHexString(expectedDecrypted)
                    + " got " + Hex.ToHexString(decrypted));
            }

            //test 2
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F");
            iv = Hex.Decode("202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F");
            input = Hex.Decode("606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9F");
            authText = Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F");

            expectedMac = Hex.Decode("9AB831B4B0BF0FDBC36E4B4FD58F0F00");
            expectedEncrypted = Hex.Decode("7EC15C54BB553CB1437BE0EFDD2E810F6058497EBCE4408A08A73FADF3F459D56B0103702D13AB73ACD2EB33A8B5E9CFFF5EB21865A6B499C10C810C4BAEBE809C48AD90A9E12A68380EF1C1B7C83EE1");

            mac = new byte[expectedMac.Length];
            encrypted = new byte[expectedEncrypted.Length];

            decrypted = new byte[encrypted.Length];
            expectedDecrypted = new byte[input.Length + expectedMac.Length];
            Array.Copy(input, 0, expectedDecrypted, 0, input.Length);
            Array.Copy(expectedMac, 0, expectedDecrypted, input.Length, expectedMac.Length);


            param = new AeadParameters(new KeyParameter(key), 128, iv);

            dstu7624ccm = new KCcmBlockCipher(new Dstu7624Engine(256));

            dstu7624ccm.Init(true, param);

            dstu7624ccm.ProcessAadBytes(authText, 0, authText.Length);

            len = dstu7624ccm.ProcessBytes(input, 0, input.Length, encrypted, 0);

            dstu7624ccm.DoFinal(encrypted, len);

            mac = dstu7624ccm.GetMac();

            if (!Arrays.AreEqual(mac, expectedMac))
            {
                Fail("Failed CCM mac test 2 - expected "
                    + Hex.ToHexString(expectedMac)
                    + " got " + Hex.ToHexString(mac));
            }

            if (!Arrays.AreEqual(encrypted, expectedEncrypted))
            {
                Fail("Failed CCM encrypt test 2 - expected "
                    + Hex.ToHexString(expectedEncrypted)
                    + " got " + Hex.ToHexString(encrypted));
            }

            dstu7624ccm.Init(false, param);

            dstu7624ccm.ProcessAadBytes(authText, 0, authText.Length);

            len = dstu7624ccm.ProcessBytes(expectedEncrypted, 0, expectedEncrypted.Length, decrypted, 0);

            dstu7624ccm.DoFinal(decrypted, len);

            if (!Arrays.AreEqual(decrypted, expectedDecrypted))
            {
                Fail("Failed CCM decrypt/verify mac test 2 - expected "
                    + Hex.ToHexString(expectedDecrypted)
                    + " got " + Hex.ToHexString(decrypted));
            }

            //test 3
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F");
            iv = Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F");
            input = Hex.Decode("808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9FA0A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBF");
            authText = Hex.Decode("606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F");

            expectedMac = Hex.Decode("924FA0326824355595C98028E84D86279CEA9135FAB35F22054AE3203E68AE46");
            expectedEncrypted = Hex.Decode("3EBDB4584B5169A26FBEBA0295B4223F58D5D8A031F2950A1D7764FAB97BA058E9E2DAB90FF0C519AA88435155A71B7B53BB100F5D20AFFAC0552F5F2813DEE8DD3653491737B9615A5CCD83DB32F1E479BF227C050325BBBFF60BCA9558D7FE");

            mac = new byte[expectedMac.Length];
            encrypted = new byte[expectedEncrypted.Length];

            decrypted = new byte[encrypted.Length];
            expectedDecrypted = new byte[input.Length + expectedMac.Length];
            Array.Copy(input, 0, expectedDecrypted, 0, input.Length);
            Array.Copy(expectedMac, 0, expectedDecrypted, input.Length, expectedMac.Length);


            param = new AeadParameters(new KeyParameter(key), 256, iv);

            dstu7624ccm = new KCcmBlockCipher(new Dstu7624Engine(256), 6);

            dstu7624ccm.Init(true, param);

            dstu7624ccm.ProcessAadBytes(authText, 0, authText.Length);

            len = dstu7624ccm.ProcessBytes(input, 0, input.Length, encrypted, 0);

            dstu7624ccm.DoFinal(encrypted, len);

            mac = dstu7624ccm.GetMac();

            if (!Arrays.AreEqual(mac, expectedMac))
            {
                Fail("Failed CCM mac test 3 - expected "
                    + Hex.ToHexString(expectedMac)
                    + " got " + Hex.ToHexString(mac));
            }

            if (!Arrays.AreEqual(encrypted, expectedEncrypted))
            {
                Fail("Failed CCM encrypt test 3 - expected "
                    + Hex.ToHexString(expectedEncrypted)
                    + " got " + Hex.ToHexString(encrypted));
            }

            dstu7624ccm.Init(false, param);

            dstu7624ccm.ProcessAadBytes(authText, 0, authText.Length);

            len = dstu7624ccm.ProcessBytes(expectedEncrypted, 0, expectedEncrypted.Length, decrypted, 0);

            dstu7624ccm.DoFinal(decrypted, len);

            if (!Arrays.AreEqual(decrypted, expectedDecrypted))
            {
                Fail("Failed CCM decrypt/verify mac test 3 - expected "
                    + Hex.ToHexString(expectedDecrypted)
                    + " got " + Hex.ToHexString(decrypted));
            }

            //test 4
            key = Hex.Decode("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F");
            iv = Hex.Decode("404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F");
            input = Hex.Decode("C0C1C2C3C4C5C6C7C8C9CACBCCCDCECFD0D1D2D3D4D5D6D7D8D9DADBDCDDDEDFE0E1E2E3E4E5E6E7E8E9EAEBECEDEEEFF0F1F2F3F4F5F6F7F8F9FAFBFCFDFEFF");
            authText = Hex.Decode("808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9FA0A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBF");

            expectedMac = Hex.Decode("D4155EC3D888C8D32FE184AC260FD60F567705E1DF362A6F1F9C287156AA96D91BC4C56F9709E72F3D79CF0A9AC8BDC2BA836BE50E823AB50FB1B39080390923");
            expectedEncrypted = Hex.Decode("220642D7277D104788CF97B10210984F506435512F7BF153C5CDABFECC10AFB4A2E2FC51F616AF80FFDD0607FAD4F542B8EF0667717CE3EAAA8FBC303CE76C99BD8F80CE149143C04FC2490272A31B029DDADA82F055FE4ABEF452A7D438B21E59C1D8B3DD4606BAD66A6F36300EF3CE0E5F3BB59F11416E80B7FC5A8E8B057A");

            mac = new byte[expectedMac.Length];
            encrypted = new byte[expectedEncrypted.Length];

            decrypted = new byte[encrypted.Length];
            expectedDecrypted = new byte[input.Length + expectedMac.Length];
            Array.Copy(input, 0, expectedDecrypted, 0, input.Length);
            Array.Copy(expectedMac, 0, expectedDecrypted, input.Length, expectedMac.Length);


            param = new AeadParameters(new KeyParameter(key), 512, iv);

            dstu7624ccm = new KCcmBlockCipher(new Dstu7624Engine(512), 8);

            dstu7624ccm.Init(true, param);

            dstu7624ccm.ProcessAadBytes(authText, 0, authText.Length);

            len = dstu7624ccm.ProcessBytes(input, 0, input.Length, encrypted, 0);

            dstu7624ccm.DoFinal(encrypted, len);

            mac = dstu7624ccm.GetMac();

            if (!Arrays.AreEqual(mac, expectedMac))
            {
                Fail("Failed CCM mac test 4 - expected "
                    + Hex.ToHexString(expectedMac)
                    + " got " + Hex.ToHexString(mac));
            }

            if (!Arrays.AreEqual(encrypted, expectedEncrypted))
            {
                Fail("Failed CCM encrypt test 4 - expected "
                    + Hex.ToHexString(expectedEncrypted)
                    + " got " + Hex.ToHexString(encrypted));
            }

            dstu7624ccm.Init(false, param);

            dstu7624ccm.ProcessAadBytes(authText, 0, authText.Length);

            len = dstu7624ccm.ProcessBytes(expectedEncrypted, 0, expectedEncrypted.Length, decrypted, 0);

            dstu7624ccm.DoFinal(decrypted, len);

            if (!Arrays.AreEqual(decrypted, expectedDecrypted))
            {
                Fail("Failed CCM decrypt/verify mac test 4 - expected "
                    + Hex.ToHexString(expectedDecrypted)
                    + " got " + Hex.ToHexString(decrypted));
            }
        }

        [Test]
        public void Dstu7624TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }

        public override string Name
        {
            get { return "Dstu7624"; }
        }

        public static void Main(
           string[] args)
        {
            Dstu7624Test test = new Dstu7624Test();
            ITestResult result = test.Perform();

            Console.WriteLine(result.ToString());
        }
    }
}
