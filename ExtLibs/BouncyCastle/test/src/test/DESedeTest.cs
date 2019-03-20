using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
    /// <remarks>
    /// Basic test class for key generation for a DES-EDE block cipher, basically
    /// this just exercises the provider, and makes sure we are behaving sensibly,
    /// correctness of the implementation is shown in the lightweight test classes.
    /// </remarks>
    [TestFixture]
    public class DesEdeTest
        : SimpleTest
    {
        private static string[] cipherTests1 =
    {
        "112",
        "2f4bc6b30c893fa549d82c560d61cf3eb088aed020603de249d82c560d61cf3e529e95ecd8e05394",
        "128",
        "2f4bc6b30c893fa549d82c560d61cf3eb088aed020603de249d82c560d61cf3e529e95ecd8e05394",
        "168",
        "50ddb583a25c21e6c9233f8e57a86d40bb034af421c03096c9233f8e57a86d402fce91e8eb639f89",
        "192",
        "50ddb583a25c21e6c9233f8e57a86d40bb034af421c03096c9233f8e57a86d402fce91e8eb639f89",
        };

        private static byte[] input1 = Hex.Decode("000102030405060708090a0b0c0d0e0fff0102030405060708090a0b0c0d0e0f");

        /**
         * a fake random number generator - we just want to make sure the random numbers
         * aren't random so that we get the same output, while still getting to test the
         * key generation facilities.
         */
        private class FixedSecureRandom
            : SecureRandom
        {
            private byte[] seed =
        {
            (byte)0xaa, (byte)0xfd, (byte)0x12, (byte)0xf6, (byte)0x59,
            (byte)0xca, (byte)0xe6, (byte)0x34, (byte)0x89, (byte)0xb4,
            (byte)0x79, (byte)0xe5, (byte)0x07, (byte)0x6d, (byte)0xde,
            (byte)0xc2, (byte)0xf0, (byte)0x6c, (byte)0xb5, (byte)0x8f
        };

            public override void NextBytes(
                byte[] bytes)
            {
                int offset = 0;

                while ((offset + seed.Length) < bytes.Length)
                {
                    Array.Copy(seed, 0, bytes, offset, seed.Length);
                    offset += seed.Length;
                }

                Array.Copy(seed, 0, bytes, offset, bytes.Length - offset);
            }
        }

        public override string Name
        {
            get { return "DESEDE"; }
        }

        private void wrapTest(
            string  alg,
            int     id,
            byte[]  kek,
            byte[]  iv,
            byte[]  input,
            byte[]  output)
        {
            try
            {
                IWrapper wrapper = WrapperUtilities.GetWrapper(alg + "Wrap");

                KeyParameter desEdeKey = new DesEdeParameters(kek);
                wrapper.Init(true, new ParametersWithIV(desEdeKey, iv));

                try
                {
//					byte[] cText = wrapper.Wrap(new SecretKeySpec(input, alg));
                    byte[] cText = wrapper.Wrap(input, 0, input.Length);

                    if (!Arrays.AreEqual(cText, output))
                    {
                        Fail("failed wrap test " + id  + " expected "
                            + Hex.ToHexString(output) + " got " + Hex.ToHexString(cText));
                    }
                }
                catch (Exception e)
                {
                    Fail("failed wrap test exception " + e.ToString());
                }

                wrapper.Init(false, desEdeKey);

                try
                {
//					Key pText = wrapper.unwrap(output, alg, IBufferedCipher.SECRET_KEY);
                    byte[] pText = wrapper.Unwrap(output, 0, output.Length);
//					if (!Arrays.AreEqual(pText.getEncoded(), input))
                    if (!Arrays.AreEqual(pText, input))
                    {
                        Fail("failed unwrap test " + id  + " expected "
                            + Hex.ToHexString(input) + " got "
                            + Hex.ToHexString(pText));
                    }
                }
                catch (Exception e)
                {
                    Fail("failed unwrap test exception " + e.ToString());
                }
            }
            catch (Exception ex)
            {
                Fail("failed exception " + ex.ToString());
            }
        }

        private void doTest(
            string  alg,
            int     strength,
            byte[]  input,
            byte[]  output)
        {
            KeyParameter		key = null;
            CipherKeyGenerator	keyGen;
            SecureRandom		rand;
            IBufferedCipher		inCipher = null;
            IBufferedCipher		outCipher = null;
            CipherStream		cIn;
            CipherStream		cOut;
            MemoryStream		bIn;
            MemoryStream		bOut;

            rand = new FixedSecureRandom();

            try
            {
                keyGen = GeneratorUtilities.GetKeyGenerator(alg);
                keyGen.Init(new KeyGenerationParameters(rand, strength));

                key = new DesEdeParameters(keyGen.GenerateKey());

                inCipher = CipherUtilities.GetCipher(alg + "/ECB/PKCS7Padding");
                outCipher = CipherUtilities.GetCipher(alg + "/ECB/PKCS7Padding");

                outCipher.Init(true, new ParametersWithRandom(key, rand));
            }
            catch (Exception e)
            {
                Fail(alg + " failed initialisation - " + e.ToString());
            }

            try
            {
                inCipher.Init(false, key);
            }
            catch (Exception e)
            {
                Fail(alg + " failed initialisation - " + e.ToString());
            }

            //
            // encryption pass
            //
            bOut = new MemoryStream();

            cOut = new CipherStream(bOut, null, outCipher);

            try
            {
                for (int i = 0; i != input.Length / 2; i++)
                {
                    cOut.WriteByte(input[i]);
                }
                cOut.Write(input, input.Length / 2, input.Length - input.Length / 2);
                cOut.Close();
            }
            catch (IOException e)
            {
                Fail(alg + " failed encryption - " + e.ToString());
            }

            byte[] bytes = bOut.ToArray();

            if (!Arrays.AreEqual(bytes, output))
            {
                Fail(alg + " failed encryption - expected "
                    + Hex.ToHexString(output) + " got "
                    + Hex.ToHexString(bytes));
            }

            //
            // decryption pass
            //
            bIn = new MemoryStream(bytes, false);

            cIn = new CipherStream(bIn, inCipher, null);

            try
            {
//				DataInputStream dIn = new DataInputStream(cIn);
                BinaryReader dIn = new BinaryReader(cIn);

                bytes = new byte[input.Length];

                for (int i = 0; i != input.Length / 2; i++)
                {
                    bytes[i] = (byte)dIn.ReadByte();
                }
//				dIn.readFully(bytes, input.Length / 2, bytes.Length - input.Length / 2);
                int remaining = bytes.Length - input.Length / 2;
                byte[] rest = dIn.ReadBytes(remaining);
                if (rest.Length != remaining)
                    throw new Exception("IO problem with BinaryReader");
                rest.CopyTo(bytes, input.Length / 2);
            }
            catch (Exception e)
            {
                Fail(alg + " failed encryption - " + e.ToString());
            }

            if (!Arrays.AreEqual(bytes, input))
            {
                Fail(alg + " failed decryption - expected "
                    + Hex.ToHexString(input) + " got "
                    + Hex.ToHexString(bytes));
            }

            // TODO Put back in
//			//
//			// keyspec test
//			//
//			try
//			{
//				SecretKeyFactory keyFactory = SecretKeyFactory.getInstance(alg);
//				DESedeKeySpec keySpec = (DESedeKeySpec)keyFactory.getKeySpec((SecretKey)key, DESedeKeySpec.class);
//
//				if (!equalArray(key.getEncoded(), keySpec.getKey(), 16))
//				{
//					Fail(alg + " KeySpec does not match key.");
//				}
//			}
//			catch (Exception e)
//			{
//				Fail(alg + " failed keyspec - " + e.ToString());
//			}
        }

        public override void PerformTest()
        {
            for (int i = 0; i != cipherTests1.Length; i += 2)
            {
                doTest("DESEDE", int.Parse(cipherTests1[i]), input1, Hex.Decode(cipherTests1[i + 1]));
            }

            for (int i = 0; i != cipherTests1.Length; i += 2)
            {
                doTest("TDEA", int.Parse(cipherTests1[i]), input1, Hex.Decode(cipherTests1[i + 1]));
            }

            byte[] kek1 = Hex.Decode("255e0d1c07b646dfb3134cc843ba8aa71f025b7c0838251f");
            byte[] iv1 = Hex.Decode("5dd4cbfc96f5453b");
            byte[] in1 = Hex.Decode("2923bf85e06dd6ae529149f1f1bae9eab3a7da3d860d3e98");
            byte[] out1 = Hex.Decode("690107618ef092b3b48ca1796b234ae9fa33ebb4159604037db5d6a84eb3aac2768c632775a467d4");

            wrapTest("DESEDE", 1, kek1, iv1, in1, out1);
            wrapTest("TDEA", 1, kek1, iv1, in1, out1);
        }

        public static void Main(
            string[] args)
        {
            RunTest(new DesEdeTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
