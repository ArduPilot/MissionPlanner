using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Tests
{
	[TestFixture]
	public class ECEncodingTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "ECEncodingTest"; }
		}

		/** J.4.7 An Example with m = 304 */
		private int m = 304;

		/** f = 010000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000807 */
		private int k1 = 1;
		private int k2 = 2;
		private int k3 = 11;
		private static readonly byte[] hexa = {(byte)0xFD, 0x0D, 0x69, 0x31, 0x49, (byte)0xA1, 0x18, (byte)0xF6, 0x51
			, (byte)0xE6, (byte)0xDC, (byte)0xE6, (byte)0x80, 0x20, (byte)0x85, 0x37, 0x7E, 0x5F, (byte)0x88, 0x2D, 0x1B, 0x51
			, 0x0B, 0x44, 0x16, 0x00, 0x74, (byte)0xC1, 0x28, (byte)0x80, 0x78, 0x36, 0x5A, 0x03
			, (byte)0x96, (byte)0xC8, (byte)0xE6, (byte)0x81};
		private static readonly byte[] hexb = {(byte)0xBD, (byte)0xDB, (byte)0x97, (byte)0xE5, (byte)0x55
			, (byte)0xA5, (byte)0x0A, (byte)0x90, (byte)0x8E, (byte)0x43, (byte)0xB0
			, (byte)0x1C, (byte)0x79, (byte)0x8E, (byte)0xA5, (byte)0xDA, (byte)0xA6
			, (byte)0x78, (byte)0x8F, (byte)0x1E, (byte)0xA2, (byte)0x79
			, (byte)0x4E, (byte)0xFC, (byte)0xF5, (byte)0x71, (byte)0x66, (byte)0xB8
			, (byte)0xC1, (byte)0x40, (byte)0x39, (byte)0x60, (byte)0x1E
			, (byte)0x55, (byte)0x82, (byte)0x73, (byte)0x40, (byte)0xBE};
		private static readonly BigInteger a = new BigInteger(1, hexa);
		private static readonly BigInteger b = new BigInteger(1, hexb);

		/** Base point G (with point compression) */
		private byte[] enc = {0x02, 0x19, 0x7B, 0x07, (byte)0x84, 0x5E, (byte)0x9B, (byte)0xE2, (byte)0xD9, 0x6A, (byte)0xDB, 0x0F
				, 0x5F, 0x3C, 0x7F, 0x2C, (byte)0xFF, (byte)0xBD, 0x7A, 0x3E, (byte)0xB8, (byte)0xB6, (byte)0xFE, 
				(byte)0xC3, 0x5C, 0x7F, (byte)0xD6, 0x7F, 0x26, (byte)0xDD, (byte)0xF6
				, 0x28, 0x5A, 0x64, 0x4F, 0x74, 0x0A, 0x26, 0x14};

		private void doTestPointCompression() 
		{
			ECCurve curve = new F2mCurve(m, k1, k2, k3, a, b);
			curve.DecodePoint(enc);

			int[] ks = new int[3];
			ks[0] = k3;
			ks[1] = k2;
			ks[2] = k1;
		}
	    
		public override void PerformTest()
		{
			byte[] ecParams = Hex.Decode("3081C8020101302806072A8648CE3D0101021D00D7C134AA264366862A18302575D1D787B09F075797DA89F57EC8C0FF303C041C68A5E62CA9CE6C1C299803A6C1530B514E182AD8B0042A59CAD29F43041C2580F63CCFE44138870713B1A92369E33E2135D266DBB372386C400B0439040D9029AD2C7E5CF4340823B2A87DC68C9E4CE3174C1E6EFDEE12C07D58AA56F772C0726F24C6B89E4ECDAC24354B9E99CAA3F6D3761402CD021D00D7C134AA264366862A18302575D0FB98D116BC4B6DDEBCA3A5A7939F020101");
			doTestParams(ecParams, true);

			doTestParams(ecParams, false);

			ecParams = Hex.Decode("3081C8020101302806072A8648CE3D0101021D00D7C134AA264366862A18302575D1D787B09F075797DA89F57EC8C0FF303C041C56E6C7E4F11A7B4B961A4DCB5BD282EB22E42E9BCBE3E7B361F18012041C4BE3E7B361F18012F2353D22975E02D8D05D2C6F3342DD8F57D4C76F0439048D127A0C27E0DE207ED3B7FB98F83C8BD5A2A57C827F4B97874DEB2C1BAEB0C006958CE61BB1FC81F5389E288CB3E86E2ED91FB47B08FCCA021D00D7C134AA264366862A18302575D11A5F7AABFBA3D897FF5CA727AF53020101");
			doTestParams(ecParams, true);

			doTestParams(ecParams, false);

			ecParams = Hex.Decode("30820142020101303c06072a8648ce3d0101023100fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffeffffffff0000000000000000ffffffff3066043100fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffeffffffff0000000000000000fffffffc043100b3312fa7e23ee7e4988e056be3f82d19181d9c6efe8141120314088f5013875ac656398d8a2ed19d2a85c8edd3ec2aef046104aa87ca22be8b05378eb1c71ef320ad746e1d3b628ba79b9859f741e082542a385502f25dbf55296c3a545e3872760ab73617de4a96262c6f5d9e98bf9292dc29f8f41dbd289a147ce9da3113b5f0b8c00a60b1ce1d7e819d7a431d7c90ea0e5f023100ffffffffffffffffffffffffffffffffffffffffffffffffc7634d81f4372ddf581a0db248b0a77aecec196accc52973020101");
			doTestParams(ecParams, true);

			doTestParams(ecParams, false);

			doTestPointCompression();
		}

		private void doTestParams(
			byte[]	ecParameterEncoded,
			bool	compress)
		{
//			string keyStorePass = "myPass";
			Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(ecParameterEncoded);
			X9ECParameters x9 = new X9ECParameters(seq);
			AsymmetricCipherKeyPair kp = null;
			bool success = false;
			while (!success)
			{
				IAsymmetricCipherKeyPairGenerator kpg = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
//				kpg.Init(new ECParameterSpec(x9.Curve, x9.G, x9.N, x9.H, x9.GetSeed()));
				ECDomainParameters ecParams = new ECDomainParameters(
					x9.Curve, x9.G, x9.N, x9.H, x9.GetSeed());
				kpg.Init(new ECKeyGenerationParameters(ecParams, new SecureRandom()));
				kp = kpg.GenerateKeyPair();
				// The very old Problem... we need a certificate chain to
				// save a private key...
				ECPublicKeyParameters pubKey = (ECPublicKeyParameters) kp.Public;

				if (!compress)
				{
					//pubKey.setPointFormat("UNCOMPRESSED");
					pubKey = SetPublicUncompressed(pubKey);
				}

				byte[] x = pubKey.Q.AffineXCoord.ToBigInteger().ToByteArrayUnsigned();
				byte[] y = pubKey.Q.AffineYCoord.ToBigInteger().ToByteArrayUnsigned();
				if (x.Length == y.Length)
				{
					success = true;
				}
			}

			// The very old Problem... we need a certificate chain to
			// save a private key...

			X509CertificateEntry[] chain = new X509CertificateEntry[] {
				new X509CertificateEntry(GenerateSelfSignedSoftECCert(kp, compress))
			};

//			KeyStore keyStore = KeyStore.getInstance("BKS");
//			keyStore.load(null, keyStorePass.ToCharArray());
			Pkcs12Store keyStore = new Pkcs12StoreBuilder().Build();

			keyStore.SetCertificateEntry("ECCert", chain[0]);

			ECPrivateKeyParameters privateECKey = (ECPrivateKeyParameters) kp.Private;
			keyStore.SetKeyEntry("ECPrivKey", new AsymmetricKeyEntry(privateECKey), chain);

			// Test ec sign / verify
			ECPublicKeyParameters pub = (ECPublicKeyParameters) kp.Public;
//			string oldPrivateKey = new string(Hex.encode(privateECKey.getEncoded()));
			byte[] oldPrivateKeyBytes = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateECKey).GetDerEncoded();
			string oldPrivateKey = Hex.ToHexString(oldPrivateKeyBytes);
//			string oldPublicKey = new string(Hex.encode(pub.getEncoded()));
			byte[] oldPublicKeyBytes = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub).GetDerEncoded();
			string oldPublicKey = Hex.ToHexString(oldPublicKeyBytes);
			ECPrivateKeyParameters newKey = (ECPrivateKeyParameters)
				keyStore.GetKey("ECPrivKey").Key;
			ECPublicKeyParameters newPubKey = (ECPublicKeyParameters)
				keyStore.GetCertificate("ECCert").Certificate.GetPublicKey();

			if (!compress)
			{
				// TODO Private key compression?
				//newKey.setPointFormat("UNCOMPRESSED");
				//newPubKey.setPointFormat("UNCOMPRESSED");
				newPubKey = SetPublicUncompressed(newPubKey);
			}

//			string newPrivateKey = new string(Hex.encode(newKey.getEncoded()));
			byte[] newPrivateKeyBytes = PrivateKeyInfoFactory.CreatePrivateKeyInfo(newKey).GetDerEncoded();
			string newPrivateKey = Hex.ToHexString(newPrivateKeyBytes);
//			string newPublicKey = new string(Hex.encode(newPubKey.getEncoded()));
			byte[] newPublicKeyBytes = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(newPubKey).GetDerEncoded();
			string newPublicKey = Hex.ToHexString(newPublicKeyBytes);

			if (!oldPrivateKey.Equals(newPrivateKey))
//			if (!privateECKey.Equals(newKey))
			{
				Fail("failed private key comparison");
			}

			if (!oldPublicKey.Equals(newPublicKey))
//			if (!pub.Equals(newPubKey))
			{
				Fail("failed public key comparison");
			}
		}

		/**
		* Create a self signed cert for our software emulation
		* 
		* @param kp
		*            is the keypair for our certificate
		* @return a self signed cert for our software emulation
		* @throws InvalidKeyException
		*             on error
		* @throws SignatureException
		*             on error
		*/
		private X509Certificate GenerateSelfSignedSoftECCert(
			AsymmetricCipherKeyPair	kp,
			bool					compress)
		{
			X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();
			ECPrivateKeyParameters privECKey = (ECPrivateKeyParameters) kp.Private;
			ECPublicKeyParameters pubECKey = (ECPublicKeyParameters) kp.Public;

			if (!compress)
			{
				// TODO Private key compression?
				//privECKey.setPointFormat("UNCOMPRESSED");
				//pubECKey.setPointFormat("UNCOMPRESSED");
				pubECKey = SetPublicUncompressed(pubECKey);
			}

			certGen.SetSignatureAlgorithm("ECDSAwithSHA1");
			certGen.SetSerialNumber(BigInteger.One);
			certGen.SetIssuerDN(new X509Name("CN=Software emul (EC Cert)"));
			certGen.SetNotBefore(DateTime.UtcNow.AddSeconds(-50));
			certGen.SetNotAfter(DateTime.UtcNow.AddSeconds(50000));
			certGen.SetSubjectDN(new X509Name("CN=Software emul (EC Cert)"));
			certGen.SetPublicKey(pubECKey);

			return certGen.Generate(privECKey);
		}

		private ECPublicKeyParameters SetPublicUncompressed(
			ECPublicKeyParameters	key)
		{
			ECPoint p = key.Q.Normalize();
			return new ECPublicKeyParameters(
				key.AlgorithmName,
				p.Curve.CreatePoint(p.XCoord.ToBigInteger(), p.YCoord.ToBigInteger()),
				key.Parameters);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new ECEncodingTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
