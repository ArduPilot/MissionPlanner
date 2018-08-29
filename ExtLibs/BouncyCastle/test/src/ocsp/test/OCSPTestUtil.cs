using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Ocsp.Tests
{
	public class OcspTestUtil
	{
		public static SecureRandom rand;
		public static IAsymmetricCipherKeyPairGenerator kpg, ecKpg;
		public static CipherKeyGenerator desede128kg;
		public static CipherKeyGenerator desede192kg;
		public static CipherKeyGenerator rc240kg;
		public static CipherKeyGenerator rc264kg;
		public static CipherKeyGenerator rc2128kg;
		public static BigInteger serialNumber;

		public static readonly bool Debug = true;

		static OcspTestUtil()
		{
			rand = new SecureRandom();

//			kpg  = KeyPairGenerator.GetInstance("RSA");
//			kpg.initialize(1024, rand);
			kpg = GeneratorUtilities.GetKeyPairGenerator("RSA");
			kpg.Init(new RsaKeyGenerationParameters(
				BigInteger.ValueOf(0x10001), rand, 1024, 25));

			serialNumber = BigInteger.One;

			ecKpg = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
			ecKpg.Init(new KeyGenerationParameters(rand, 192));
		}

		public static AsymmetricCipherKeyPair MakeKeyPair()
		{
			return kpg.GenerateKeyPair();
		}

		public static AsymmetricCipherKeyPair MakeECKeyPair()
		{
			return ecKpg.GenerateKeyPair();
		}

		public static X509Certificate MakeCertificate(AsymmetricCipherKeyPair _subKP,
			string _subDN, AsymmetricCipherKeyPair _issKP, string _issDN)
		{
			return MakeCertificate(_subKP, _subDN, _issKP, _issDN, false);
		}

		public static X509Certificate MakeECDsaCertificate(AsymmetricCipherKeyPair _subKP,
			string _subDN, AsymmetricCipherKeyPair _issKP, string _issDN)
		{
			return MakeECDsaCertificate(_subKP, _subDN, _issKP, _issDN, false);
		}

		public static X509Certificate MakeCACertificate(AsymmetricCipherKeyPair _subKP,
			string _subDN, AsymmetricCipherKeyPair _issKP, string _issDN)
		{

			return MakeCertificate(_subKP, _subDN, _issKP, _issDN, true);
		}

		public static X509Certificate MakeCertificate(AsymmetricCipherKeyPair _subKP,
			string _subDN, AsymmetricCipherKeyPair _issKP, string _issDN, bool _ca)
		{
			return MakeCertificate(_subKP,_subDN, _issKP, _issDN, "MD5withRSA", _ca);
		}

		public static X509Certificate MakeECDsaCertificate(AsymmetricCipherKeyPair _subKP,
			string _subDN, AsymmetricCipherKeyPair _issKP, string _issDN, bool _ca)
		{
			return MakeCertificate(_subKP,_subDN, _issKP, _issDN, "SHA1WithECDSA", _ca);
		}

		public static X509Certificate MakeCertificate(AsymmetricCipherKeyPair _subKP,
			string _subDN, AsymmetricCipherKeyPair _issKP, string _issDN, string algorithm, bool _ca)
		{
			AsymmetricKeyParameter _subPub = _subKP.Public;
			AsymmetricKeyParameter _issPriv = _issKP.Private;
			AsymmetricKeyParameter _issPub = _issKP.Public;

			X509V3CertificateGenerator _v3CertGen = new X509V3CertificateGenerator();

			_v3CertGen.Reset();
			_v3CertGen.SetSerialNumber(allocateSerialNumber());
			_v3CertGen.SetIssuerDN(new X509Name(_issDN));
			_v3CertGen.SetNotBefore(DateTime.UtcNow);
			_v3CertGen.SetNotAfter(DateTime.UtcNow.AddDays(100));
			_v3CertGen.SetSubjectDN(new X509Name(_subDN));
			_v3CertGen.SetPublicKey(_subPub);
			_v3CertGen.SetSignatureAlgorithm(algorithm);

			_v3CertGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false,
				createSubjectKeyId(_subPub));

			_v3CertGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false,
				createAuthorityKeyId(_issPub));

			_v3CertGen.AddExtension(X509Extensions.BasicConstraints, false,
				new BasicConstraints(_ca));

			X509Certificate _cert = _v3CertGen.Generate(_issPriv);

			_cert.CheckValidity(DateTime.UtcNow);
			_cert.Verify(_issPub);

			return _cert;
		}

		/*
		 *
		 * INTERNAL METHODS
		 *
		 */

		private static AuthorityKeyIdentifier createAuthorityKeyId(
			AsymmetricKeyParameter _pubKey)
		{
			SubjectPublicKeyInfo _info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_pubKey);
			return new AuthorityKeyIdentifier(_info);
		}

		private static SubjectKeyIdentifier createSubjectKeyId(
			AsymmetricKeyParameter _pubKey)
		{
			SubjectPublicKeyInfo _info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_pubKey);
			return new SubjectKeyIdentifier(_info);
		}

		private static BigInteger allocateSerialNumber()
		{
			BigInteger _tmp = serialNumber;
			serialNumber = serialNumber.Add(BigInteger.One);
			return _tmp;
		}
	}
}
