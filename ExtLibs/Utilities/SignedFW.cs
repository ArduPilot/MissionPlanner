using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zlib;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;

namespace MissionPlanner.Utilities
{
    public  class SignedFW
    {

        public static AsymmetricCipherKeyPair GenerateKey()
        {
            //Creating Random
            var secureRandom = new SecureRandom();

            //Parameters creation using the random and keysize
            var keyGenParam = new KeyGenerationParameters(secureRandom, 256);

            var generator = new Ed25519KeyPairGenerator();
            generator.Init(keyGenParam);
            AsymmetricCipherKeyPair keyPairg = generator.GenerateKeyPair();
            return keyPairg;
        }

        public static AsymmetricCipherKeyPair GenerateKey(byte[] knownseed)
        {
            //Creating Random
            var secureRandom = new SecureRandom(new preseedrandom(knownseed));

            //Parameters creation using the random and keysize
            var keyGenParam = new KeyGenerationParameters(secureRandom, 256);

            var generator = new Ed25519KeyPairGenerator();
            generator.Init(keyGenParam);
            AsymmetricCipherKeyPair keyPairg = generator.GenerateKeyPair();
            return keyPairg;
        }

        public static byte[] CreateSignedBL(AsymmetricCipherKeyPair keyPair, string filename)
        {
            var descriptor = new byte[] { 0x4e, 0xcf, 0x4e, 0xa5, 0xa6, 0xb6, 0xf7, 0x29 };
            var max_keys = 10;
            var key_len = 32;

            var bl = File.ReadAllBytes(filename);
            var ms = new MemoryStream(bl);

            var offset = bl.Search(descriptor, 0);

            if (offset == -1)
                throw new Exception("Invalid bin, descriptor not found");

            offset += 8;

            // ap keys
            var ap1 = "PUBLIC_KEYV1:WJbbpbjOz/yMB3JxnvqyTUInCQdZcStkA0qhn2ldhPI=";
            var ap2 = "PUBLIC_KEYV1:X8jdVqxIIUmCuMSi8IhTZ40VkXW0gbRczzMtdSghqCI=";
            var ap3 = "PUBLIC_KEYV1:snNHkX96F9A+/ISppHZrc1jjPo3jMNN+g2PToKhWSgA=";

            var header = "PUBLIC_KEYV1:";

            ms.Position = offset;
            var key1 = Convert.FromBase64String(ap1.Substring(header.Length));
            ms.Write(key1, 0, key1.Length);

            var key2 = Convert.FromBase64String(ap2.Substring(header.Length));
            ms.Write(key2, 0, key2.Length);

            var key3 = Convert.FromBase64String(ap3.Substring(header.Length));
            ms.Write(key3, 0, key3.Length);

            var key4 = ((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded();
            ms.Write(key4, 0, key4.Length);

            return ms.ToArray();
        }
        public static byte[] CreateSignedAPJ(AsymmetricCipherKeyPair keyPair, string filename)
        {
            var key_len = 32;
            var sig_len = 64;
            var sig_version = 30437;
            var descriptor = new byte[] { 0x41, 0xa3, 0xe5, 0xf2, 0x65, 0x69, 0x92, 0x07 };

            var d = JsonConvert.DeserializeObject<dynamic>(new StreamReader(filename).ReadToEnd());

            byte[] img = new byte[(int)d["image_size"]];
            var b64 = (byte[])Convert.FromBase64String(d["image"].ToString());
            ZlibStream decompressionStream = new ZlibStream(new MemoryStream(b64), CompressionMode.Decompress);
            var read = decompressionStream.Read(img, 0, img.Length);

            var offset = img.Search(descriptor, 0);

            if (offset == -1)
                throw new Exception("Invalid APJ, descriptor not found");

            offset += 8;
            var desc_len = 92;

            var signer = SignerUtilities.GetSigner("ED25519");
            signer.Init(true, keyPair.Private);
            signer.BlockUpdate(img, 0, offset);
            signer.BlockUpdate(img, offset + desc_len, img.Length - (offset + desc_len));
            var sig = signer.GenerateSignature();
            
            var sha = new Sha512Digest();
            byte[] h = new byte[sha.GetDigestSize()];
            sha.BlockUpdate(img, 0, offset);
            sha.BlockUpdate(img, offset + desc_len, img.Length - (offset + desc_len));
            sha.DoFinal(h, 0);

            //desc = struct.pack("<IQ64s", sig_len+8, sig_version, signature)
            //img = img[:(offset + 16)] + desc + img[(offset + desc_len):]

            BitConverter.GetBytes((uint)sig_len + 8).CopyTo(img, offset + 16);
            BitConverter.GetBytes((ulong)sig_version).CopyTo(img, offset + 16 + 4);
            sig.CopyTo(img, offset + 16 + 4 + 8);

            d["image"] = Convert.ToBase64String(ZlibStream.CompressBuffer(img));
            d["image_size"] = img.Length;
            try
            {
                d["flash_free"] = d["flash_total"] - d["image_size"];
            }
            catch
            {
            }

            d["signed_firmware"] = true;
            d["sha"] = Convert.ToBase64String(h);
            Console.WriteLine("FW SHA: " + Convert.ToBase64String(h));

            return System.Text.ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(d, Formatting.Indented));
        }

        private class preseedrandom : IRandomGenerator
        {
            private byte[] knownseed;

            public preseedrandom(byte[] knownseed)
            {
                this.knownseed = knownseed;
            }

            public void AddSeedMaterial(byte[] seed)
            {
                throw new NotImplementedException();
            }

            public void AddSeedMaterial(long seed)
            {
                throw new NotImplementedException();
            }

            public void NextBytes(byte[] bytes)
            {
                Array.Copy(knownseed, bytes, bytes.Length);
            }

            public void NextBytes(byte[] bytes, int start, int len)
            {
                throw new NotImplementedException();
            }
        }
    }
}
