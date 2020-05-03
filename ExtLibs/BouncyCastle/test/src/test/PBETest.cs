using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	/**
	* test out the various PBE modes, making sure the JCE implementations
	* are compatible woth the light weight ones.
	*/
	[TestFixture]
	public class PbeTest
		: SimpleTest
	{
		private class OpenSslTest
			: SimpleTest
		{
			private char[] password;
			private string baseAlgorithm;
			private string algorithm;
			private int keySize;
			private int ivSize;

			public OpenSslTest(
				string	baseAlgorithm,
				string	algorithm,
				int		keySize,
				int		ivSize)
			{
				this.password = algorithm.ToCharArray();
				this.baseAlgorithm = baseAlgorithm;
				this.algorithm = algorithm;
				this.keySize = keySize;
				this.ivSize = ivSize;
			}

			public override string Name
			{
				get { return "OpenSSLPBE"; }
			}

			public override void PerformTest()
			{
				byte[] salt = new byte[16];
				int iCount = 100;

				for (int i = 0; i != salt.Length; i++)
				{
					salt[i] = (byte)i;
				}

				PbeParametersGenerator pGen = new OpenSslPbeParametersGenerator();

				pGen.Init(
					PbeParametersGenerator.Pkcs5PasswordToBytes(password),
					salt,
					iCount);

				ParametersWithIV parameters = (ParametersWithIV)
					pGen.GenerateDerivedParameters(baseAlgorithm, keySize, ivSize);

				KeyParameter encKey = (KeyParameter) parameters.Parameters;

				IBufferedCipher c;
				if (baseAlgorithm.Equals("RC4"))
				{
					c = CipherUtilities.GetCipher(baseAlgorithm);

					c.Init(true, encKey);
				}
				else
				{
					c = CipherUtilities.GetCipher(baseAlgorithm + "/CBC/PKCS7Padding");

					c.Init(true, parameters);
				}

				byte[] enc = c.DoFinal(salt);

				c = CipherUtilities.GetCipher(algorithm);

//					PBEKeySpec keySpec = new PBEKeySpec(password, salt, iCount);
//					SecretKeyFactory fact = SecretKeyFactory.getInstance(algorithm);
//
//					c.Init(false, fact.generateSecret(keySpec));

				Asn1Encodable algParams = PbeUtilities.GenerateAlgorithmParameters(
					algorithm, salt, iCount);
				ICipherParameters cipherParams = PbeUtilities.GenerateCipherParameters(
					algorithm, password, algParams);
				c.Init(false, cipherParams);

				byte[] dec = c.DoFinal(enc);

				if (!AreEqual(salt, dec))
				{
					Fail("" + algorithm + "failed encryption/decryption test");
				}
			}
		}

		private class Pkcs12Test
			: SimpleTest
		{
			private char[] password;
			private string baseAlgorithm;
			private string algorithm;
			private IDigest digest;
			private int keySize;
			private int ivSize;

			public Pkcs12Test(
				string		baseAlgorithm,
				string		algorithm,
				IDigest		digest,
				int			keySize,
				int			ivSize)
			{
				this.password = algorithm.ToCharArray();
				this.baseAlgorithm = baseAlgorithm;
				this.algorithm = algorithm;
				this.digest = digest;
				this.keySize = keySize;
				this.ivSize = ivSize;
			}

			public override string Name
			{
				get { return "PKCS12PBE"; }
			}

			public override void PerformTest()
			{
				int iCount = 100;
				byte[] salt = DigestUtilities.DoFinal(digest);

				PbeParametersGenerator pGen = new Pkcs12ParametersGenerator(digest);

				pGen.Init(
					PbeParametersGenerator.Pkcs12PasswordToBytes(password),
					salt,
					iCount);

				ParametersWithIV parameters = (ParametersWithIV)
					pGen.GenerateDerivedParameters(baseAlgorithm, keySize, ivSize);

				KeyParameter encKey = (KeyParameter) parameters.Parameters;

				IBufferedCipher c;
				if (baseAlgorithm.Equals("RC4"))
				{
					c = CipherUtilities.GetCipher(baseAlgorithm);
	                
					c.Init(true, encKey);
				}
				else
				{
					c = CipherUtilities.GetCipher(baseAlgorithm + "/CBC/PKCS7Padding");
	                
					c.Init(true, parameters);
				}

				byte[] enc = c.DoFinal(salt);

				c = CipherUtilities.GetCipher(algorithm);

//					PBEKeySpec keySpec = new PBEKeySpec(password, salt, iCount);
//					SecretKeyFactory fact = SecretKeyFactory.getInstance(algorithm);
//
//					c.Init(false, fact.generateSecret(keySpec));

				Asn1Encodable algParams = PbeUtilities.GenerateAlgorithmParameters(
					algorithm, salt, iCount);
				ICipherParameters cipherParams = PbeUtilities.GenerateCipherParameters(
					algorithm, password, algParams);
				c.Init(false, cipherParams);

				byte[] dec = c.DoFinal(enc);

				if (!AreEqual(salt, dec))
				{
					Fail("" + algorithm + "failed encryption/decryption test");
				}

				// NB: We don't support retrieving parameters from cipher
//				//
//				// get the parameters
//				//
//				AlgorithmParameters param = c.getParameters();
//				PBEParameterSpec spec = (PBEParameterSpec)param.getParameterSpec(PBEParameterSpec.class);
//
//				if (!AreEqual(salt, spec.getSalt()))
//				{
//					Fail("" + algorithm + "failed salt test");
//				}
//	            
//				if (iCount != spec.getIterationCount())
//				{
//					Fail("" + algorithm + "failed count test");
//				}

	            // NB: This section just repeats earlier test passing 'param' separately
//				//
//				// try using parameters
//				//
//				keySpec = new PBEKeySpec(password);
//	            
//				c.Init(false, fact.generateSecret(keySpec), param);
//	            
//				dec = c.DoFinal(enc);
//	            
//				if (!AreEqual(salt, dec))
//				{
//					Fail("" + algorithm + "failed encryption/decryption test");
//				}
			}
		}

		private Pkcs12Test[] pkcs12Tests = {
			new Pkcs12Test("DESede", "PBEWITHSHAAND3-KEYTRIPLEDES-CBC",  new Sha1Digest(),   192,  64),
			new Pkcs12Test("DESede", "PBEWITHSHAAND2-KEYTRIPLEDES-CBC",  new Sha1Digest(),   128,  64),
			new Pkcs12Test("RC4",    "PBEWITHSHAAND128BITRC4",           new Sha1Digest(),   128,   0),
			new Pkcs12Test("RC4",    "PBEWITHSHAAND40BITRC4",            new Sha1Digest(),    40,   0),
			new Pkcs12Test("RC2",    "PBEWITHSHAAND128BITRC2-CBC",       new Sha1Digest(),   128,  64),
			new Pkcs12Test("RC2",    "PBEWITHSHAAND40BITRC2-CBC",        new Sha1Digest(),    40,  64),
			new Pkcs12Test("AES",    "PBEWithSHA1And128BitAES-CBC-BC",   new Sha1Digest(),   128, 128),
			new Pkcs12Test("AES",    "PBEWithSHA1And192BitAES-CBC-BC",   new Sha1Digest(),   192, 128),
			new Pkcs12Test("AES",    "PBEWithSHA1And256BitAES-CBC-BC",   new Sha1Digest(),   256, 128),
			new Pkcs12Test("AES",    "PBEWithSHA256And128BitAES-CBC-BC", new Sha256Digest(), 128, 128),
			new Pkcs12Test("AES",    "PBEWithSHA256And192BitAES-CBC-BC", new Sha256Digest(), 192, 128),   
			new Pkcs12Test("AES",    "PBEWithSHA256And256BitAES-CBC-BC", new Sha256Digest(), 256, 128)
		};

		private OpenSslTest[] openSSLTests = {
			new OpenSslTest("AES", "PBEWITHMD5AND128BITAES-CBC-OPENSSL", 128, 128),
			new OpenSslTest("AES", "PBEWITHMD5AND192BITAES-CBC-OPENSSL", 192, 128),
			new OpenSslTest("AES", "PBEWITHMD5AND256BITAES-CBC-OPENSSL", 256, 128)
		};

		static byte[] message = Hex.Decode("4869205468657265");

		private byte[] hMac1 = Hex.Decode("bcc42174ccb04f425d9a5c8c4a95d6fd7c372911");
		private byte[] hMac2 = Hex.Decode("cb1d8bdb6aca9e3fa8980d6eb41ab28a7eb2cfd6");

		// NB: These two makePbeCipher... methods are same in .NET
		private IBufferedCipher makePbeCipherUsingParam(
			string  algorithm,
			bool	forEncryption,
			char[]  password,
			byte[]  salt,
			int     iterationCount)
		{
//			PBEKeySpec pbeSpec = new PBEKeySpec(password);
//			SecretKeyFactory keyFact = SecretKeyFactory.getInstance(algorithm);
//			PBEParameterSpec defParams = new PBEParameterSpec(salt, iterationCount);

			Asn1Encodable algParams = PbeUtilities.GenerateAlgorithmParameters(
				algorithm, salt, iterationCount);
			ICipherParameters cipherParams = PbeUtilities.GenerateCipherParameters(
				algorithm, password, algParams);
			
			IBufferedCipher cipher = CipherUtilities.GetCipher(algorithm);

//			cipher.Init(forEncryption, keyFact.generateSecret(pbeSpec), defParams);
			cipher.Init(forEncryption, cipherParams);

			return cipher;
		}

		// NB: These two makePbeCipher... methods are same in .NET
		private IBufferedCipher makePbeCipherWithoutParam(
			string  algorithm,
			bool	forEncryption,
			char[]  password,
			byte[]  salt,
			int     iterationCount)
		{
//			PBEKeySpec pbeSpec = new PBEKeySpec(password, salt, iterationCount);
//			SecretKeyFactory keyFact = SecretKeyFactory.getInstance(algorithm);

			Asn1Encodable algParams = PbeUtilities.GenerateAlgorithmParameters(
				algorithm, salt, iterationCount);
			ICipherParameters cipherParams = PbeUtilities.GenerateCipherParameters(
				algorithm, password, algParams);
			
			IBufferedCipher cipher = CipherUtilities.GetCipher(algorithm);

//			cipher.Init(forEncryption, keyFact.generateSecret(pbeSpec));
			cipher.Init(forEncryption, cipherParams);

			return cipher;
		}

		private void doTestPbeHMac(
			string	hmacName,
			byte[]	output)
		{
			ICipherParameters key = null;
			byte[] outBytes;
			IMac mac = null;

			try
			{
//				SecretKeyFactory fact = SecretKeyFactory.getInstance(hmacName);
//
//				key = fact.generateSecret(new PBEKeySpec("hello".ToCharArray()));

				Asn1Encodable algParams = PbeUtilities.GenerateAlgorithmParameters(
					hmacName, new byte[20], 100);
				key = PbeUtilities.GenerateCipherParameters(
					hmacName, "hello".ToCharArray(), algParams);
				mac = MacUtilities.GetMac(hmacName);
			}
			catch (Exception e)
			{
				Fail("Failed - exception " + e.ToString(), e);
			}

			try
			{
//				mac.Init(key, new PBEParameterSpec(new byte[20], 100));
				mac.Init(key);
			}
			catch (Exception e)
			{
				Fail("Failed - exception " + e.ToString(), e);
			}

			mac.Reset();

			mac.BlockUpdate(message, 0, message.Length);

//			outBytes = mac.DoFinal();
			outBytes = new byte[mac.GetMacSize()];
			mac.DoFinal(outBytes, 0);

			if (!AreEqual(outBytes, output))
			{
				Fail("Failed - expected "
					+ Hex.ToHexString(output) + " got "
					+ Hex.ToHexString(outBytes));
			}
		}

		public override void PerformTest()
		{
			byte[] input = Hex.Decode("1234567890abcdefabcdef1234567890fedbca098765");

			//
			// DES
			//
			IBufferedCipher cEnc = CipherUtilities.GetCipher("DES/CBC/PKCS7Padding");

			cEnc.Init(
				true,
				new ParametersWithIV(
					new DesParameters(Hex.Decode("30e69252758e5346")),
					Hex.Decode("7c1c1ab9c454a688")));

			byte[] outBytes = cEnc.DoFinal(input);
			char[] password = "password".ToCharArray();

			IBufferedCipher cDec = makePbeCipherUsingParam(
				"PBEWithSHA1AndDES",
				false,
				password,
				Hex.Decode("7d60435f02e9e0ae"),
				2048);

			byte[] inBytes = cDec.DoFinal(outBytes);

			if (!AreEqual(input, inBytes))
			{
				Fail("DES failed");
			}

			cDec = makePbeCipherWithoutParam(
				"PBEWithSHA1AndDES",
				false,
				password,
				Hex.Decode("7d60435f02e9e0ae"),
				2048);

			inBytes = cDec.DoFinal(outBytes);

			if (!AreEqual(input, inBytes))
			{
				Fail("DES failed without param");
			}

			//
			// DESede
			//
			cEnc = CipherUtilities.GetCipher("DESede/CBC/PKCS7Padding");

			cEnc.Init(
				true,
				new ParametersWithIV(
					new DesParameters(Hex.Decode("732f2d33c801732b7206756cbd44f9c1c103ddd97c7cbe8e")),
					Hex.Decode("b07bf522c8d608b8")));

			outBytes = cEnc.DoFinal(input);

			cDec = makePbeCipherUsingParam(
				"PBEWithSHAAnd3-KeyTripleDES-CBC",
				false,
				password,
				Hex.Decode("7d60435f02e9e0ae"),
				2048);

			inBytes = cDec.DoFinal(outBytes);

			if (!AreEqual(input, inBytes))
			{
				Fail("DESede failed");
			}

			//
			// 40Bit RC2
			//
			cEnc = CipherUtilities.GetCipher("RC2/CBC/PKCS7Padding");

			cEnc.Init(
				true,
				new ParametersWithIV(
					new RC2Parameters(Hex.Decode("732f2d33c8")),
					Hex.Decode("b07bf522c8d608b8")));

			outBytes = cEnc.DoFinal(input);

			cDec = makePbeCipherUsingParam(
				"PBEWithSHAAnd40BitRC2-CBC",
				false,
				password,
				Hex.Decode("7d60435f02e9e0ae"),
				2048);

			inBytes = cDec.DoFinal(outBytes);

			if (!AreEqual(input, inBytes))
			{
				Fail("RC2 failed");
			}

			//
			// 128bit RC4
			//
			cEnc = CipherUtilities.GetCipher("RC4");

			cEnc.Init(
				true,
				ParameterUtilities.CreateKeyParameter("RC4", Hex.Decode("732f2d33c801732b7206756cbd44f9c1")));

			outBytes = cEnc.DoFinal(input);

			cDec = makePbeCipherUsingParam(
				"PBEWithSHAAnd128BitRC4",
				false,
				password,
				Hex.Decode("7d60435f02e9e0ae"),
				2048);

			inBytes = cDec.DoFinal(outBytes);

			if (!AreEqual(input, inBytes))
			{
				Fail("RC4 failed");
			}

			cDec = makePbeCipherWithoutParam(
				"PBEWithSHAAnd128BitRC4",
				false,
				password,
				Hex.Decode("7d60435f02e9e0ae"),
				2048);

			inBytes = cDec.DoFinal(outBytes);
	        
			if (!AreEqual(input, inBytes))
			{
				Fail("RC4 failed without param");
			}

			for (int i = 0; i != pkcs12Tests.Length; i++)
			{
				pkcs12Tests[i].PerformTest();
			}

			for (int i = 0; i != openSSLTests.Length; i++)
			{
				openSSLTests[i].PerformTest();
			}

			doTestPbeHMac("PBEWithHMacSHA1", hMac1);
			doTestPbeHMac("PBEWithHMacRIPEMD160", hMac2);
		}

		public override string Name
		{
			get { return "PbeTest"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new PbeTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
