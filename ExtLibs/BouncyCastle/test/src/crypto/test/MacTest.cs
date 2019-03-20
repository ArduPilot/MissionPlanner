using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /// <remarks> MAC tester - vectors from
    /// <a href="http://www.itl.nist.gov/fipspubs/fip81.htm">FIP 81</a> and
    /// <a href="http://www.itl.nist.gov/fipspubs/fip113.htm">FIP 113</a>.
    /// </remarks>
    [TestFixture]
    public class MacTest
		: ITest
    {
        public string Name
        {
			get { return "IMac"; }
        }

		internal static byte[] keyBytes;
        internal static byte[] ivBytes;
        internal static byte[] input1;
        internal static byte[] output1;
        internal static byte[] output2;
        internal static byte[] output3;

        //
        // these aren't NIST vectors, just for regression testing.
        //
        internal static byte[] input2;
        internal static byte[] output4;
        internal static byte[] output5;
        internal static byte[] output6;

        public MacTest()
        {
        }

        public virtual ITestResult Perform()
        {
            KeyParameter key = new KeyParameter(keyBytes);
            IBlockCipher cipher = new DesEngine();
            IMac mac = new CbcBlockCipherMac(cipher);

            //
            // standard DAC - zero IV
            //
            mac.Init(key);

            mac.BlockUpdate(input1, 0, input1.Length);

            byte[] outBytes = new byte[4];

            mac.DoFinal(outBytes, 0);

            if (!Arrays.AreEqual(outBytes, output1))
            {
                return new SimpleTestResult(false, Name + ": Failed - expected "
					+ Hex.ToHexString(output1) + " got " + Hex.ToHexString(outBytes));
            }

            //
            // mac with IV.
            //
            ParametersWithIV param = new ParametersWithIV(key, ivBytes);

            mac.Init(param);

            mac.BlockUpdate(input1, 0, input1.Length);

            outBytes = new byte[4];

            mac.DoFinal(outBytes, 0);

            if (!Arrays.AreEqual(outBytes, output2))
            {
                return new SimpleTestResult(false, Name + ": Failed - expected "
					+ Hex.ToHexString(output2) + " got " + Hex.ToHexString(outBytes));
            }

            //
            // CFB mac with IV - 8 bit CFB mode
            //
            param = new ParametersWithIV(key, ivBytes);

            mac = new CfbBlockCipherMac(cipher);

            mac.Init(param);

            mac.BlockUpdate(input1, 0, input1.Length);

            outBytes = new byte[4];

            mac.DoFinal(outBytes, 0);

            if (!Arrays.AreEqual(outBytes, output3))
            {
                return new SimpleTestResult(false, Name + ": Failed - expected "
					+ Hex.ToHexString(output3) + " got " + Hex.ToHexString(outBytes));
            }

            //
            // word aligned data - zero IV
            //
            mac.Init(key);

            mac.BlockUpdate(input2, 0, input2.Length);

            outBytes = new byte[4];

            mac.DoFinal(outBytes, 0);

            if (!Arrays.AreEqual(outBytes, output4))
            {
                return new SimpleTestResult(false, Name + ": Failed - expected "
					+ Hex.ToHexString(output4) + " got " + Hex.ToHexString(outBytes));
            }

            //
            // word aligned data - zero IV - CBC padding
            //
            mac = new CbcBlockCipherMac(cipher, new Pkcs7Padding());

            mac.Init(key);

            mac.BlockUpdate(input2, 0, input2.Length);

            outBytes = new byte[4];

            mac.DoFinal(outBytes, 0);

            if (!Arrays.AreEqual(outBytes, output5))
            {
                return new SimpleTestResult(false, Name + ": Failed - expected "
					+ Hex.ToHexString(output5) + " got " + Hex.ToHexString(outBytes));
            }

            //
            // non-word aligned data - zero IV - CBC padding
            //
            mac.Reset();

            mac.BlockUpdate(input1, 0, input1.Length);

            outBytes = new byte[4];

            mac.DoFinal(outBytes, 0);

            if (!Arrays.AreEqual(outBytes, output6))
            {
                return new SimpleTestResult(false, Name + ": Failed - expected "
					+ Hex.ToHexString(output6) + " got " + Hex.ToHexString(outBytes));
            }

            //
            // non-word aligned data - zero IV - CBC padding
            //
            mac.Init(key);

            mac.BlockUpdate(input1, 0, input1.Length);

            outBytes = new byte[4];

            mac.DoFinal(outBytes, 0);

            if (!Arrays.AreEqual(outBytes, output6))
            {
                return new SimpleTestResult(false, Name + ": Failed - expected "
					+ Hex.ToHexString(output6) + " got " + Hex.ToHexString(outBytes));
            }

            return new SimpleTestResult(true, Name + ": Okay");
        }

		public static void Main(
            string[] args)
        {
            MacTest test = new MacTest();
            ITestResult result = test.Perform();

            Console.WriteLine(result);
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }

        static MacTest()
        {
            keyBytes = Hex.Decode("0123456789abcdef");
            ivBytes = Hex.Decode("1234567890abcdef");
            input1 = Hex.Decode("37363534333231204e6f77206973207468652074696d6520666f7220");
            output1 = Hex.Decode("f1d30f68");
            output2 = Hex.Decode("58d2e77e");
            output3 = Hex.Decode("cd647403");
            input2 = Hex.Decode("3736353433323120");
            output4 = Hex.Decode("3af549c9");
            output5 = Hex.Decode("188fbdd5");
            output6 = Hex.Decode("7045eecd");
        }
    }
}
