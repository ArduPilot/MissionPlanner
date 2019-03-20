using System;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Tsp.Tests
{
	public class TspTestUtil
	{
		public static SecureRandom rand = new SecureRandom();
		public static IAsymmetricCipherKeyPairGenerator kpg;
		public static CipherKeyGenerator desede128kg;
		public static CipherKeyGenerator desede192kg;
		public static CipherKeyGenerator rc240kg;
		public static CipherKeyGenerator rc264kg;
		public static CipherKeyGenerator rc2128kg;
		public static BigInteger serialNumber = BigInteger.One;
		public static readonly bool Debug = true;
		public static DerObjectIdentifier EuroPkiTsaTestPolicy = new DerObjectIdentifier("1.3.6.1.4.1.5255.5.1");

		static TspTestUtil()
		{
			rand = new SecureRandom();

			kpg = GeneratorUtilities.GetKeyPairGenerator("RSA");
			kpg.Init(new RsaKeyGenerationParameters(
				BigInteger.ValueOf(0x10001), rand, 1024, 25));

			desede128kg = GeneratorUtilities.GetKeyGenerator("DESEDE");
			desede128kg.Init(new KeyGenerationParameters(rand, 112));

			desede192kg = GeneratorUtilities.GetKeyGenerator("DESEDE");
			desede192kg.Init(new KeyGenerationParameters(rand, 168));

			rc240kg = GeneratorUtilities.GetKeyGenerator("RC2");
			rc240kg.Init(new KeyGenerationParameters(rand, 40));

			rc264kg = GeneratorUtilities.GetKeyGenerator("RC2");
			rc264kg.Init(new KeyGenerationParameters(rand, 64));

			rc2128kg = GeneratorUtilities.GetKeyGenerator("RC2");
			rc2128kg.Init(new KeyGenerationParameters(rand, 128));

			serialNumber = BigInteger.One;
		}

		public static string DumpBase64(
			byte[] data)
		{
			StringBuilder buf = new StringBuilder();

			data = Base64.Encode(data);

			for (int i = 0; i < data.Length; i += 64)
			{
				if (i + 64 < data.Length)
				{
					buf.Append(Encoding.ASCII.GetString(data, i, 64));
				}
				else
				{
					buf.Append(Encoding.ASCII.GetString(data, i, data.Length - i));
				}
				buf.Append('\n');
			}

			return buf.ToString();
		}

		public static AsymmetricCipherKeyPair MakeKeyPair()
		{
			return kpg.GenerateKeyPair();
		}

		public static KeyParameter MakeDesede128Key()
		{
			return new DesEdeParameters(desede128kg.GenerateKey());
		}

		public static KeyParameter MakeDesede192Key()
		{
			return new DesEdeParameters(desede192kg.GenerateKey());
		}

		public static KeyParameter MakeRC240Key()
		{
			return new RC2Parameters(rc240kg.GenerateKey());
		}

		public static KeyParameter MakeRC264Key()
		{
			return new RC2Parameters(rc264kg.GenerateKey());
		}

		public static KeyParameter MakeRC2128Key()
		{
			return new RC2Parameters(rc2128kg.GenerateKey());
		}

		public static X509Certificate MakeCertificate(AsymmetricCipherKeyPair _subKP,
			string _subDN, AsymmetricCipherKeyPair _issKP, string _issDN)
		{
			return MakeCertificate(_subKP, _subDN, _issKP, _issDN, false);
		}

		public static X509Certificate MakeCACertificate(AsymmetricCipherKeyPair _subKP,
			string _subDN, AsymmetricCipherKeyPair _issKP, string _issDN)
		{
			return MakeCertificate(_subKP, _subDN, _issKP, _issDN, true);
		}

		public static X509Certificate MakeCertificate(AsymmetricCipherKeyPair _subKP,
			string _subDN, AsymmetricCipherKeyPair _issKP, string _issDN, bool _ca)
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
			_v3CertGen.SetSignatureAlgorithm("MD5WithRSAEncryption");

			_v3CertGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false,
					createSubjectKeyId(_subPub));

			_v3CertGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false,
					createAuthorityKeyId(_issPub));

			if (_ca)
			{
				_v3CertGen.AddExtension(X509Extensions.BasicConstraints, false,
						new BasicConstraints(_ca));
			}
			else
			{
				_v3CertGen.AddExtension(X509Extensions.ExtendedKeyUsage, true,
					ExtendedKeyUsage.GetInstance(new DerSequence(KeyPurposeID.IdKPTimeStamping)));
			}

			X509Certificate _cert = _v3CertGen.Generate(_issPriv);

			_cert.CheckValidity(DateTime.UtcNow);
			_cert.Verify(_issPub);

			return _cert;
		}

		/*
		*
		*  INTERNAL METHODS
		*
		*/
		private static AuthorityKeyIdentifier createAuthorityKeyId(
			AsymmetricKeyParameter _pubKey)
		{
			return new AuthorityKeyIdentifier(
				SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_pubKey));
		}

//		private static AuthorityKeyIdentifier createAuthorityKeyId(
//			AsymmetricKeyParameter _pubKey, X509Name _name, int _sNumber)
//		{
//			SubjectPublicKeyInfo _info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_pubKey);
//
//			GeneralName _genName = new GeneralName(_name);
//
//			return new AuthorityKeyIdentifier(_info, GeneralNames.GetInstance(
//				new DerSequence(_genName)), BigInteger.ValueOf(_sNumber));
//		}

		private static SubjectKeyIdentifier createSubjectKeyId(
			AsymmetricKeyParameter _pubKey)
		{
			return new SubjectKeyIdentifier(
				SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_pubKey));
		}

		private static BigInteger allocateSerialNumber()
		{
			BigInteger _tmp = serialNumber;
			serialNumber = serialNumber.Add(BigInteger.One);
			return _tmp;
		}
	}
}