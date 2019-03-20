using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Iana;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Rosstandart;
using Org.BouncyCastle.Asn1.UA;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
    /// <remarks>HMAC tester</remarks>
    [TestFixture]
    public class HMacTest
        : SimpleTest
    {
        private static byte[] keyBytes = Hex.Decode("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b");
        private static byte[] message = Encoding.ASCII.GetBytes("Hi There");
        private static byte[] output1 = Hex.Decode("b617318655057264e28bc0b6fb378c8ef146be00");
        private static byte[] outputMD5 = Hex.Decode("5ccec34ea9656392457fa1ac27f08fbc");
        private static byte[] outputMD2 = Hex.Decode("dc1923ef5f161d35bef839ca8c807808");
        private static byte[] outputMD4 = Hex.Decode("5570ce964ba8c11756cdc3970278ff5a");
        private static byte[] output224 = Hex.Decode("896fb1128abbdf196832107cd49df33f47b4b1169912ba4f53684b22");
        private static byte[] output256 = Hex.Decode("b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7");
        private static byte[] output384 = Hex.Decode("afd03944d84895626b0825f4ab46907f15f9dadbe4101ec682aa034c7cebc59cfaea9ea9076ede7f4af152e8b2fa9cb6");
        private static byte[] output512 = Hex.Decode("87aa7cdea5ef619d4ff0b4241a1d6cb02379f4e2ce4ec2787ad0b30545e17cdedaa833b7d6b8a702038b274eaea3f4e4be9d914eeb61f1702e696c203a126854");
        private static byte[] output512_224 = Hex.Decode("b244ba01307c0e7a8ccaad13b1067a4cf6b961fe0c6a20bda3d92039");
        private static byte[] output512_256 = Hex.Decode("9f9126c3d9c3c330d760425ca8a217e31feae31bfe70196ff81642b868402eab");
        private static byte[] outputRipeMD128 = Hex.Decode("fda5717fb7e20cf05d30bb286a44b05d");
        private static byte[] outputRipeMD160 = Hex.Decode("24cb4bd67d20fc1a5d2ed7732dcc39377f0a5668");
        private static byte[] outputTiger = Hex.Decode("1d7a658c75f8f004916e7b07e2a2e10aec7de2ae124d3647");
        private static byte[] outputOld384 = Hex.Decode("0a046aaa0255e432912228f8ccda437c8a8363fb160afb0570ab5b1fd5ddc20eb1888b9ed4e5b6cb5bc034cd9ef70e40");
        private static byte[] outputOld512 = Hex.Decode("9656975ee5de55e75f2976ecce9a04501060b9dc22a6eda2eaef638966280182477fe09f080b2bf564649cad42af8607a2bd8d02979df3a980f15e2326a0a22a");

        private static byte[] outputKck224 = Hex.Decode("b73d595a2ba9af815e9f2b4e53e78581ebd34a80b3bbaac4e702c4cc");
        private static byte[] outputKck256 = Hex.Decode("9663d10c73ee294054dc9faf95647cb99731d12210ff7075fb3d3395abfb9821");
        private static byte[] outputKck288 = Hex.Decode("36145df8742160a1811139494d708f9a12757c30dedc622a98aa6ecb69da32a34ea55441");
        private static byte[] outputKck384 = Hex.Decode("892dfdf5d51e4679bf320cd16d4c9dc6f749744608e003add7fba894acff87361efa4e5799be06b6461f43b60ae97048");
        private static byte[] outputKck512 = Hex.Decode("8852c63be8cfc21541a4ee5e5a9a852fc2f7a9adec2ff3a13718ab4ed81aaea0b87b7eb397323548e261a64e7fc75198f6663a11b22cd957f7c8ec858a1c7755");

        private static byte[] outputSha3_224 = Hex.Decode("3b16546bbc7be2706a031dcafd56373d9884367641d8c59af3c860f7");
        private static byte[] outputSha3_256 = Hex.Decode("ba85192310dffa96e2a3a40e69774351140bb7185e1202cdcc917589f95e16bb");
        private static byte[] outputSha3_384 = Hex.Decode("68d2dcf7fd4ddd0a2240c8a437305f61fb7334cfb5d0226e1bc27dc10a2e723a20d370b47743130e26ac7e3d532886bd");
        private static byte[] outputSha3_512 = Hex.Decode("eb3fbd4b2eaab8f5c504bd3a41465aacec15770a7cabac531e482f860b5ec7ba47ccb2c6f2afce8f88d22b6dc61380f23a668fd3888bb80537c0a0b86407689e");

        private static byte[] outputGost2012_256 = Hex.Decode("f03422dfa37a507ca126ce01b8eba6b7fdda8f8a60dd8f2703e3a372120b8294");
        private static byte[] outputGost2012_512 = Hex.Decode("86b6a06bfa9f1974aff6ccd7fa3f835f0bd850395d6084efc47b9dda861a2cdf0dcaf959160733d5269f6567966dd7a9f932a77cd6f080012cd476f1c2cc31bb");

        private static byte[] outputDSTU7564_256 = Hex.Decode("98ac67aa21eaf6e8666fb748d66cfc15d5d66f5194c87fffa647e406d3375cdb");
        private static byte[] outputDSTU7564_384 = Hex.Decode("4e46a87e70fcd2ccfb4433a8eaec68991a96b11085c5d5484db71af51bac469c03f76e1f721843c8e8667708fe41a48d");
        private static byte[] outputDSTU7564_512 = Hex.Decode("5b7acf633a7551b8410fa66a60c74a494e46a87e70fcd2ccfb4433a8eaec68991a96b11085c5d5484db71af51bac469c03f76e1f721843c8e8667708fe41a48d");

        private void DoTestHMac(string hmacName, byte[] output)
        {
            KeyParameter key = new KeyParameter(keyBytes); //, hmacName);

            IMac mac = MacUtilities.GetMac(hmacName);
            mac.Init(key);
            mac.Reset();
            mac.BlockUpdate(message, 0, message.Length);
            byte[] outBytes = MacUtilities.DoFinal(mac);

            if (!AreEqual(outBytes, output))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output) + " got "
                    + Hex.ToHexString(outBytes));
            }

            // no key generator for the old algorithms
            if (hmacName.StartsWith("Old"))
                return;

            CipherKeyGenerator kGen = GeneratorUtilities.GetKeyGenerator(hmacName);
            key = new KeyParameter(kGen.GenerateKey());
            mac.Init(key); // hmacName
            mac.BlockUpdate(message, 0, message.Length);
            outBytes = MacUtilities.DoFinal(mac);
        }

        private void DoTestHMac(string hmacName, int defKeySize, byte[] output)
        {
            KeyParameter key = new KeyParameter(keyBytes); //, hmacName);

            IMac mac = MacUtilities.GetMac(hmacName);
            mac.Init(key);
            mac.Reset();
            mac.BlockUpdate(message, 0, message.Length);
            byte[] outBytes = MacUtilities.DoFinal(mac);

            if (!AreEqual(outBytes, output))
            {
                Fail("Failed - expected "
                    + Hex.ToHexString(output) + " got "
                    + Hex.ToHexString(outBytes));
            }

            CipherKeyGenerator kGen = GeneratorUtilities.GetKeyGenerator(hmacName);
            key = new KeyParameter(kGen.GenerateKey());
            mac.Init(key); // hmacName
            mac.BlockUpdate(message, 0, message.Length);
            outBytes = MacUtilities.DoFinal(mac);

            IsTrue("default key wrong length", key.GetKey().Length == (defKeySize / 8));
        }

        private void DoTestExceptions()
        {
            IMac mac = MacUtilities.GetMac("HmacSHA1");

            byte [] b = {(byte)1, (byte)2, (byte)3, (byte)4, (byte)5};
//			KeyParameter sks = new KeyParameter(b); //, "HmacSHA1");
//			RC5ParameterSpec algPS = new RC5ParameterSpec(100, 100, 100);
            RC5Parameters rc5Parameters = new RC5Parameters(b, 100);

            try
            {
//				mac.Init(sks, algPS);
                mac.Init(rc5Parameters);
            }
//			catch (InvalidAlgorithmParameterException e)
            catch (Exception)
            {
                // ignore okay
            }

            try
            {
                mac.Init(null); //, null);
            }
//			catch (InvalidKeyException)
//			{
//				// ignore okay
//			}
//			catch (InvalidAlgorithmParameterException e)
            catch (Exception)
            {
                // ignore okay
            }

//			try
//			{
//				mac.Init(null);
//			}
//			catch (InvalidKeyException)
//			{
//				// ignore okay
//			}
        }

        public override void PerformTest()
        {
            DoTestHMac("HMac-SHA1", output1);
            DoTestHMac("HMac-MD5", outputMD5);
            DoTestHMac("HMac-MD4", outputMD4);
            DoTestHMac("HMac-MD2", outputMD2);
            DoTestHMac("HMac-SHA224", output224);
            DoTestHMac("HMac-SHA256", output256);
            DoTestHMac("HMac-SHA384", output384);
            DoTestHMac("HMac-SHA512", output512);
            DoTestHMac("HMac-SHA512/224", output512_224);
            DoTestHMac("HMac-SHA512/256", output512_256);
            DoTestHMac("HMac-RIPEMD128", outputRipeMD128);
            DoTestHMac("HMac-RIPEMD160", outputRipeMD160);
            DoTestHMac("HMac-TIGER", outputTiger);
            DoTestHMac("HMac-KECCAK224", 224, outputKck224);
            DoTestHMac("HMac-KECCAK256", 256, outputKck256);
            DoTestHMac("HMac-KECCAK288", 288, outputKck288);
            DoTestHMac("HMac-KECCAK384", 384, outputKck384);
            DoTestHMac("HMac-KECCAK512", 512, outputKck512);
            DoTestHMac("HMac-SHA3-224", 224, outputSha3_224);
            DoTestHMac("HMac-SHA3-256", 256, outputSha3_256);
            DoTestHMac("HMac-SHA3-384", 384, outputSha3_384);
            DoTestHMac("HMac-SHA3-512", 512, outputSha3_512);

            DoTestHMac("HMac-GOST3411-2012-256", 256, outputGost2012_256);
            DoTestHMac("HMac-GOST3411-2012-512", 512, outputGost2012_512);

            //DoTestHMac("HMac-DSTU7564-256", 256, outputDSTU7564_256);
            //DoTestHMac("HMac-DSTU7564-384", 384, outputDSTU7564_384);
            //DoTestHMac("HMac-DSTU7564-512", 512, outputDSTU7564_512);

            DoTestHMac("HMac/SHA1", output1);
            DoTestHMac("HMac/MD5", outputMD5);
            DoTestHMac("HMac/MD4", outputMD4);
            DoTestHMac("HMac/MD2", outputMD2);
            DoTestHMac("HMac/SHA224", output224);
            DoTestHMac("HMac/SHA256", output256);
            DoTestHMac("HMac/SHA384", output384);
            DoTestHMac("HMac/SHA512", output512);
            DoTestHMac("HMac/RIPEMD128", outputRipeMD128);
            DoTestHMac("HMac/RIPEMD160", outputRipeMD160);
            DoTestHMac("HMac/TIGER", outputTiger);
            DoTestHMac("HMac/KECCAK224", 224, outputKck224);
            DoTestHMac("HMac/KECCAK256", 256, outputKck256);
            DoTestHMac("HMac/KECCAK288", 288, outputKck288);
            DoTestHMac("HMac/KECCAK384", 384, outputKck384);
            DoTestHMac("HMac/KECCAK512", 512, outputKck512);
            DoTestHMac("HMac/SHA3-224", 224, outputSha3_224);
            DoTestHMac("HMac/SHA3-256", 256, outputSha3_256);
            DoTestHMac("HMac/SHA3-384", 384, outputSha3_384);
            DoTestHMac("HMac/SHA3-512", 512, outputSha3_512);
            DoTestHMac("HMac/GOST3411-2012-256", 256, outputGost2012_256);
            DoTestHMac("HMac/GOST3411-2012-512", 512, outputGost2012_512);

            DoTestHMac(PkcsObjectIdentifiers.IdHmacWithSha1.Id, output1);
            DoTestHMac(PkcsObjectIdentifiers.IdHmacWithSha224.Id, output224);
            DoTestHMac(PkcsObjectIdentifiers.IdHmacWithSha256.Id, output256);
            DoTestHMac(PkcsObjectIdentifiers.IdHmacWithSha384.Id, output384);
            DoTestHMac(PkcsObjectIdentifiers.IdHmacWithSha512.Id, output512);
            DoTestHMac(IanaObjectIdentifiers.HmacSha1.Id, output1);
            DoTestHMac(IanaObjectIdentifiers.HmacMD5.Id, outputMD5);
            DoTestHMac(IanaObjectIdentifiers.HmacRipeMD160.Id, outputRipeMD160);
            DoTestHMac(IanaObjectIdentifiers.HmacTiger.Id, outputTiger);

            DoTestHMac(NistObjectIdentifiers.IdHMacWithSha3_224.Id, 224, outputSha3_224);
            DoTestHMac(NistObjectIdentifiers.IdHMacWithSha3_256.Id, 256, outputSha3_256);
            DoTestHMac(NistObjectIdentifiers.IdHMacWithSha3_384.Id, 384, outputSha3_384);
            DoTestHMac(NistObjectIdentifiers.IdHMacWithSha3_512.Id, 512, outputSha3_512);

            DoTestHMac(RosstandartObjectIdentifiers.id_tc26_hmac_gost_3411_12_256.Id, 256, outputGost2012_256);
            DoTestHMac(RosstandartObjectIdentifiers.id_tc26_hmac_gost_3411_12_512.Id, 512, outputGost2012_512);

            //DoTestHMac(UAObjectIdentifiers.dstu7564mac_256.Id, 256, outputDSTU7564_256);
            //DoTestHMac(UAObjectIdentifiers.dstu7564mac_384.Id, 384, outputDSTU7564_384);
            //DoTestHMac(UAObjectIdentifiers.dstu7564mac_512.Id, 512, outputDSTU7564_512);

//			// test for compatibility with broken HMac.
//			DoTestHMac("OldHMacSHA384", outputOld384);
//			DoTestHMac("OldHMacSHA512", outputOld512);

            DoTestExceptions();
        }

        public override string Name
        {
            get { return "HMac"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new HMacTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
