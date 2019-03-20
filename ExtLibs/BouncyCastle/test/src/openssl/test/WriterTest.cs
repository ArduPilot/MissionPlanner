using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.OpenSsl.Tests
{
	[TestFixture]
	public class WriterTest
		: SimpleTest
	{
		private static readonly SecureRandom random = new SecureRandom();

		// TODO Replace with a randomly generated key each test run?
		private static readonly RsaPrivateCrtKeyParameters testRsaKey = new RsaPrivateCrtKeyParameters(
			new BigInteger("b4a7e46170574f16a97082b22be58b6a2a629798419be12872a4bdba626cfae9900f76abfb12139dce5de56564fab2b6543165a040c606887420e33d91ed7ed7", 16),
			new BigInteger("11", 16),
			new BigInteger("9f66f6b05410cd503b2709e88115d55daced94d1a34d4e32bf824d0dde6028ae79c5f07b580f5dce240d7111f7ddb130a7945cd7d957d1920994da389f490c89", 16),
			new BigInteger("c0a0758cdf14256f78d4708c86becdead1b50ad4ad6c5c703e2168fbf37884cb", 16),
			new BigInteger("f01734d7960ea60070f1b06f2bb81bfac48ff192ae18451d5e56c734a5aab8a5", 16),
			new BigInteger("b54bb9edff22051d9ee60f9351a48591b6500a319429c069a3e335a1d6171391", 16),
			new BigInteger("d3d83daf2a0cecd3367ae6f8ae1aeb82e9ac2f816c6fc483533d8297dd7884cd", 16),
			new BigInteger("b8f52fc6f38593dabb661d3f50f8897f8106eee68b1bce78a95b132b4e5b5d19", 16));

		private static readonly DsaParameters testDsaParams = new DsaParameters(
			new BigInteger("7434410770759874867539421675728577177024889699586189000788950934679315164676852047058354758883833299702695428196962057871264685291775577130504050839126673"),
			new BigInteger("1138656671590261728308283492178581223478058193247"),
			new BigInteger("4182906737723181805517018315469082619513954319976782448649747742951189003482834321192692620856488639629011570381138542789803819092529658402611668375788410"));

//		private static readonly PKCS8EncodedKeySpec testEcDsaKeySpec = new PKCS8EncodedKeySpec(
//			Base64.Decode("MIG/AgEAMBAGByqGSM49AgEGBSuBBAAiBIGnMIGkAgEBBDCSBU3vo7ieeKs0ABQamy/ynxlde7Ylr8HmyfLaNnMr" +
//				"jAwPp9R+KMUEhB7zxSAXv9KgBwYFK4EEACKhZANiAQQyyolMpg+TyB4o9kPWqafHIOe8o9K1glus+w2sY8OIPQQWGb5i5LdAyi" +
//				"/SscwU24rZM0yiL3BHodp9ccwyhLrFYgXJUOQcCN2dno1GMols5497in5gL5+zn0yMsRtyv5o=")
//		);
		private static readonly byte[] testEcDsaKeyBytes = Base64.Decode(
			  "MIG/AgEAMBAGByqGSM49AgEGBSuBBAAiBIGnMIGkAgEBBDCSBU3vo7ieeKs0ABQamy/ynxlde7Ylr8HmyfLaNnMr"
			+ "jAwPp9R+KMUEhB7zxSAXv9KgBwYFK4EEACKhZANiAQQyyolMpg+TyB4o9kPWqafHIOe8o9K1glus+w2sY8OIPQQWGb5i5LdAyi"
			+ "/SscwU24rZM0yiL3BHodp9ccwyhLrFYgXJUOQcCN2dno1GMols5497in5gL5+zn0yMsRtyv5o=");

		private static readonly char[] testPassword = "bouncy".ToCharArray();

		private static readonly string[] algorithms = new string[]
		{
			"AES-128-CBC", "AES-128-CFB", "AES-128-ECB", "AES-128-OFB",
			"AES-192-CBC", "AES-192-CFB", "AES-192-ECB", "AES-192-OFB",
			"AES-256-CBC", "AES-256-CFB", "AES-256-ECB", "AES-256-OFB",
			"BF-CBC", "BF-CFB", "BF-ECB", "BF-OFB",
			"DES-CBC", "DES-CFB", "DES-ECB", "DES-OFB",
			"DES-EDE", "DES-EDE-CBC", "DES-EDE-CFB", "DES-EDE-ECB", "DES-EDE-OFB",
			"DES-EDE3", "DES-EDE3-CBC", "DES-EDE3-CFB", "DES-EDE3-ECB", "DES-EDE3-OFB",
			"RC2-CBC", "RC2-CFB", "RC2-ECB", "RC2-OFB",
			"RC2-40-CBC",
			"RC2-64-CBC",
		};

		private class Password
			: IPasswordFinder
		{
			private readonly char[] password;

			public Password(
				char[] word)
			{
				this.password = (char[]) word.Clone();
			}

			public char[] GetPassword()
			{
				return (char[]) password.Clone();
			}
		}

		public override string Name
		{
			get { return "PEMWriterTest"; }
		}

		public override void PerformTest()
		{
			IAsymmetricCipherKeyPairGenerator dsaKpg = GeneratorUtilities.GetKeyPairGenerator("DSA");
			dsaKpg.Init(new DsaKeyGenerationParameters(random, testDsaParams));
			AsymmetricCipherKeyPair testDsaKp = dsaKpg.GenerateKeyPair();
			AsymmetricKeyParameter testDsaKey = testDsaKp.Private;

			DoWriteReadTest(testDsaKey);
			DoWriteReadTests(testDsaKey, algorithms);

			DoWriteReadTest(testRsaKey);
			DoWriteReadTests(testRsaKey, algorithms);

			AsymmetricKeyParameter ecPriv = PrivateKeyFactory.CreateKey(testEcDsaKeyBytes);
			DoWriteReadTest(ecPriv);
			DoWriteReadTests(ecPriv, algorithms);

			IAsymmetricCipherKeyPairGenerator ecKpg = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
			ecKpg.Init(new KeyGenerationParameters(random, 239));
			ecPriv = ecKpg.GenerateKeyPair().Private;
			DoWriteReadTest(ecPriv);
			DoWriteReadTests(ecPriv, algorithms);

			// override test
			PemWriter pWrt = new PemWriter(new StringWriter());

			object o = new PemObject("FRED", new byte[100]);
			pWrt.WriteObject(o);

			pWrt.Writer.Close();
		}

		private void DoWriteReadTests(
			AsymmetricKeyParameter	akp,
			string[]				algorithms)
		{
			foreach (string algorithm in algorithms)
			{
				DoWriteReadTest(akp, algorithm);
			}
		}

		private void DoWriteReadTest(
			AsymmetricKeyParameter	akp)
		{
			StringWriter sw = new StringWriter();
			PemWriter pw = new PemWriter(sw);

			pw.WriteObject(akp);
			pw.Writer.Close();

			string data = sw.ToString();

			PemReader pr = new PemReader(new StringReader(data));

			AsymmetricCipherKeyPair kp = pr.ReadObject() as AsymmetricCipherKeyPair;

			if (kp == null || !kp.Private.Equals(akp))
			{
				Fail("Failed to read back test key");
			}
		}

		private void DoWriteReadTest(
			AsymmetricKeyParameter	akp,
			string					algorithm)
		{
			StringWriter sw = new StringWriter();
			PemWriter pw = new PemWriter(sw);

			pw.WriteObject(akp, algorithm, testPassword, random);
			pw.Writer.Close();

			string data = sw.ToString();

			PemReader pr = new PemReader(new StringReader(data), new Password(testPassword));

			AsymmetricCipherKeyPair kp = pr.ReadObject() as AsymmetricCipherKeyPair;

			if (kp == null || !kp.Private.Equals(akp))
			{
				Fail("Failed to read back test key encoded with: " + algorithm);
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new WriterTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
