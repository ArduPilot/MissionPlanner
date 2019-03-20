using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	 * Test case for NaccacheStern cipher. For details on this cipher, please see
	 *
	 * http://www.gemplus.com/smart/rd/publications/pdf/NS98pkcs.pdf
	 *
	 * Performs the following tests:
	 *  <ul>
	 *  <li> Toy example from the NaccacheSternPaper </li>
	 *  <li> 768 bit test with text "Now is the time for all good men." (ripped from RSA test) and
	 *     the same test with the first byte replaced by 0xFF </li>
	 *  <li> 1024 bit test analog to 768 bit test </li>
	 *  </ul>
	 */
	[TestFixture, Explicit]
	public class NaccacheSternTest
		: SimpleTest
	{
		static bool debug = false;

		static readonly NaccacheSternEngine cryptEng = new NaccacheSternEngine();
		static readonly NaccacheSternEngine decryptEng = new NaccacheSternEngine();

		// Values from NaccacheStern paper
		static readonly BigInteger a = BigInteger.ValueOf(101);
		static readonly BigInteger u1 = BigInteger.ValueOf(3);
		static readonly BigInteger u2 = BigInteger.ValueOf(5);
		static readonly BigInteger u3 = BigInteger.ValueOf(7);
		static readonly BigInteger b = BigInteger.ValueOf(191);
		static readonly BigInteger v1 = BigInteger.ValueOf(11);
		static readonly BigInteger v2 = BigInteger.ValueOf(13);
		static readonly BigInteger v3 = BigInteger.ValueOf(17);

		static readonly BigInteger sigma
			= u1.Multiply(u2).Multiply(u3).Multiply(v1).Multiply(v2).Multiply(v3);

		static readonly BigInteger p
			= BigInteger.Two.Multiply(a).Multiply(u1).Multiply(u2).Multiply(u3).Add(BigInteger.One);

		static readonly BigInteger q
			= BigInteger.Two.Multiply(b).Multiply(v1).Multiply(v2).Multiply(v3).Add(BigInteger.One);

		static readonly BigInteger n = p.Multiply(q);

		static readonly BigInteger phi_n
			= p.Subtract(BigInteger.One).Multiply(q.Subtract(BigInteger.One));

		static readonly BigInteger g = BigInteger.ValueOf(131);

		static readonly IList smallPrimes = new ArrayList();

		// static final BigInteger paperTest = BigInteger.ValueOf(202);

		static readonly string input = "4e6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e";

		static readonly BigInteger paperTest = BigInteger.ValueOf(202);

		//
		// to check that we handling byte extension by big number correctly.
		//
		static readonly string edgeInput = "ff6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e";

        static NaccacheSternTest()
        {
            // First the Parameters from the NaccacheStern Paper
            // (see http://www.gemplus.com/smart/rd/publications/pdf/NS98pkcs.pdf )

            smallPrimes.Add(u1);
            smallPrimes.Add(u2);
            smallPrimes.Add(u3);
            smallPrimes.Add(v1);
            smallPrimes.Add(v2);
            smallPrimes.Add(v3);
        }

		public override string Name
		{
			get { return "NaccacheStern"; }
		}

		public override void PerformTest()
		{
			// Test with given key from NaccacheSternPaper (totally insecure)

			NaccacheSternKeyParameters pubParameters = new NaccacheSternKeyParameters(false, g, n, sigma.BitLength);

			NaccacheSternPrivateKeyParameters privParameters = new NaccacheSternPrivateKeyParameters(g, n, sigma
				.BitLength, smallPrimes, phi_n);

			AsymmetricCipherKeyPair pair = new AsymmetricCipherKeyPair(pubParameters, privParameters);

			// Initialize Engines with KeyPair

			if (debug)
			{
				Console.WriteLine("initializing encryption engine");
			}
			cryptEng.Init(true, pair.Public);

			if (debug)
			{
				Console.WriteLine("initializing decryption engine");
			}
			decryptEng.Init(false, pair.Private);

			byte[] data = paperTest.ToByteArray();

			if (!new BigInteger(data).Equals(new BigInteger(enDeCrypt(data))))
			{
				Fail("failed NaccacheStern paper test");
			}

			//
			// key generation test
			//

			//
			// 768 Bit test
			//

			if (debug)
			{
				Console.WriteLine();
				Console.WriteLine("768 Bit TEST");
			}

			// specify key generation parameters
			NaccacheSternKeyGenerationParameters genParam
				= new NaccacheSternKeyGenerationParameters(new SecureRandom(), 768, 8, 30);

			// Initialize Key generator and generate key pair
			NaccacheSternKeyPairGenerator pGen = new NaccacheSternKeyPairGenerator();
			pGen.Init(genParam);

			pair = pGen.GenerateKeyPair();

			if (((NaccacheSternKeyParameters)pair.Public).Modulus.BitLength < 768)
			{
				Console.WriteLine("FAILED: key size is <786 bit, exactly "
						   + ((NaccacheSternKeyParameters)pair.Public).Modulus.BitLength + " bit");
				Fail("failed key generation (768) length test");
			}

			// Initialize Engines with KeyPair

			if (debug)
			{
				Console.WriteLine("initializing " + genParam.Strength + " bit encryption engine");
			}
			cryptEng.Init(true, pair.Public);

			if (debug)
			{
				Console.WriteLine("initializing " + genParam.Strength + " bit decryption engine");
			}
			decryptEng.Init(false, pair.Private);

			// Basic data input
			data = Hex.Decode(input);

			if (!new BigInteger(1, data).Equals(new BigInteger(1, enDeCrypt(data))))
			{
				Fail("failed encryption decryption (" + genParam.Strength + ") basic test");
			}

			// Data starting with FF byte (would be interpreted as negative
			// BigInteger)

			data = Hex.Decode(edgeInput);

			if (!new BigInteger(1, data).Equals(new BigInteger(1, enDeCrypt(data))))
			{
				Fail("failed encryption decryption (" + genParam.Strength + ") edgeInput test");
			}

			//
			// 1024 Bit Test
			//
			/*
					if (debug)
					{
						Console.WriteLine();
						Console.WriteLine("1024 Bit TEST");
					}

					// specify key generation parameters
					genParam = new NaccacheSternKeyGenerationParameters(new SecureRandom(), 1024, 8, 40, debug);

					pGen.Init(genParam);
					pair = pGen.generateKeyPair();

					if (((NaccacheSternKeyParameters)pair.Public).getModulus().bitLength() < 1024)
					{
						if (debug)
						{
							Console.WriteLine("FAILED: key size is <1024 bit, exactly "
											+ ((NaccacheSternKeyParameters)pair.Public).getModulus().bitLength() + " bit");
						}
						Fail("failed key generation (1024) length test");
					}

					// Initialize Engines with KeyPair

					if (debug)
					{
						Console.WriteLine("initializing " + genParam.getStrength() + " bit encryption engine");
					}
					cryptEng.Init(true, pair.Public);

					if (debug)
					{
						Console.WriteLine("initializing " + genParam.getStrength() + " bit decryption engine");
					}
					decryptEng.Init(false, pair.Private);

					if (debug)
					{
						Console.WriteLine("Data is           " + new BigInteger(1, data));
					}

					// Basic data input
					data = Hex.Decode(input);

					if (!new BigInteger(1, data).Equals(new BigInteger(1, enDeCrypt(data))))
					{
						Fail("failed encryption decryption (" + genParam.getStrength() + ") basic test");
					}

					// Data starting with FF byte (would be interpreted as negative
					// BigInteger)

					data = Hex.Decode(edgeInput);

					if (!new BigInteger(1, data).Equals(new BigInteger(1, enDeCrypt(data))))
					{
						Fail("failed encryption decryption (" + genParam.getStrength() + ") edgeInput test");
					}
			*/
			// END OF TEST CASE

			try
			{
				new NaccacheSternEngine().ProcessBlock(new byte[]{ 1 }, 0, 1);
				Fail("failed initialisation check");
			}
			catch (InvalidOperationException)
			{
				// expected
			}

			if (debug)
			{
				Console.WriteLine("All tests successful");
			}
		}

		private byte[] enDeCrypt(
			byte[] input)
		{
			// create work array
			byte[] data = new byte[input.Length];
			Array.Copy(input, 0, data, 0, data.Length);

			// Perform encryption like in the paper from Naccache-Stern
			if (debug)
			{
				Console.WriteLine("encrypting data. Data representation\n"
					//                    + "As string:.... " + new string(data) + "\n"
					+ "As BigInteger: " + new BigInteger(1, data));
				Console.WriteLine("data length is " + data.Length);
			}

			try
			{
				data = cryptEng.ProcessData(data);
			}
			catch (InvalidCipherTextException e)
			{
				if (debug)
				{
					Console.WriteLine("failed - exception " + e + "\n" + e.Message);
				}
				Fail("failed - exception " + e + "\n" + e.Message);
			}

			if (debug)
			{
				Console.WriteLine("enrypted data representation\n"
						   //                    + "As string:.... " + new string(data) + "\n"
						   + "As BigInteger: " + new BigInteger(1, data));
				Console.WriteLine("data length is " + data.Length);
			}

			try
			{
				data = decryptEng.ProcessData(data);
			}
			catch (InvalidCipherTextException e)
			{
				if (debug)
				{
					Console.WriteLine("failed - exception " + e + "\n" + e.Message);
				}
				Fail("failed - exception " + e + "\n" + e.Message);
			}

			if (debug)
			{
				Console.WriteLine("decrypted data representation\n"
					//                    + "As string:.... " + new string(data) + "\n"
					+ "As BigInteger: " + new BigInteger(1, data));
				Console.WriteLine("data length is " + data.Length);
			}

			return data;
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}

		public static void Main(
			string[] args)
		{
			ITest test = new NaccacheSternTest();
			ITestResult result = test.Perform();

			Console.WriteLine(result);
		}
	}
}
