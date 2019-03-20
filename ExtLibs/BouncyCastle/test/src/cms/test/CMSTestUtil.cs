using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Cms.Tests
{
	public class CmsTestUtil
	{
		public static SecureRandom rand;
		private static IAsymmetricCipherKeyPairGenerator kpg;
		private static IAsymmetricCipherKeyPairGenerator gostKpg;
		private static IAsymmetricCipherKeyPairGenerator dsaKpg;
		private static IAsymmetricCipherKeyPairGenerator ecGostKpg;
		private static IAsymmetricCipherKeyPairGenerator ecDsaKpg;
		public static CipherKeyGenerator aes192kg;
		public static CipherKeyGenerator desede128kg;
		public static CipherKeyGenerator desede192kg;
		public static CipherKeyGenerator rc240kg;
		public static CipherKeyGenerator rc264kg;
		public static CipherKeyGenerator rc2128kg;
		public static CipherKeyGenerator aesKg;
		public static CipherKeyGenerator seedKg;
		public static CipherKeyGenerator camelliaKg;
		public static BigInteger serialNumber;

		private static readonly byte[] attrCert = Base64.Decode(
			  "MIIHQDCCBqkCAQEwgZChgY2kgYowgYcxHDAaBgkqhkiG9w0BCQEWDW1sb3JjaEB2"
			+ "dC5lZHUxHjAcBgNVBAMTFU1hcmt1cyBMb3JjaCAobWxvcmNoKTEbMBkGA1UECxMS"
			+ "VmlyZ2luaWEgVGVjaCBVc2VyMRAwDgYDVQQLEwdDbGFzcyAyMQswCQYDVQQKEwJ2"
			+ "dDELMAkGA1UEBhMCVVMwgYmkgYYwgYMxGzAZBgkqhkiG9w0BCQEWDHNzaGFoQHZ0"
			+ "LmVkdTEbMBkGA1UEAxMSU3VtaXQgU2hhaCAoc3NoYWgpMRswGQYDVQQLExJWaXJn"
			+ "aW5pYSBUZWNoIFVzZXIxEDAOBgNVBAsTB0NsYXNzIDExCzAJBgNVBAoTAnZ0MQsw"
			+ "CQYDVQQGEwJVUzANBgkqhkiG9w0BAQQFAAIBBTAiGA8yMDAzMDcxODE2MDgwMloY"
			+ "DzIwMDMwNzI1MTYwODAyWjCCBU0wggVJBgorBgEEAbRoCAEBMYIFORaCBTU8UnVs"
			+ "ZSBSdWxlSWQ9IkZpbGUtUHJpdmlsZWdlLVJ1bGUiIEVmZmVjdD0iUGVybWl0Ij4K"
			+ "IDxUYXJnZXQ+CiAgPFN1YmplY3RzPgogICA8U3ViamVjdD4KICAgIDxTdWJqZWN0"
			+ "TWF0Y2ggTWF0Y2hJZD0idXJuOm9hc2lzOm5hbWVzOnRjOnhhY21sOjEuMDpmdW5j"
			+ "dGlvbjpzdHJpbmctZXF1YWwiPgogICAgIDxBdHRyaWJ1dGVWYWx1ZSBEYXRhVHlw"
			+ "ZT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS9YTUxTY2hlbWEjc3RyaW5nIj4KICAg"
			+ "ICAgIENOPU1hcmt1cyBMb3JjaDwvQXR0cmlidXRlVmFsdWU+CiAgICAgPFN1Ympl"
			+ "Y3RBdHRyaWJ1dGVEZXNpZ25hdG9yIEF0dHJpYnV0ZUlkPSJ1cm46b2FzaXM6bmFt"
			+ "ZXM6dGM6eGFjbWw6MS4wOnN1YmplY3Q6c3ViamVjdC1pZCIgRGF0YVR5cGU9Imh0"
			+ "dHA6Ly93d3cudzMub3JnLzIwMDEvWE1MU2NoZW1hI3N0cmluZyIgLz4gCiAgICA8"
			+ "L1N1YmplY3RNYXRjaD4KICAgPC9TdWJqZWN0PgogIDwvU3ViamVjdHM+CiAgPFJl"
			+ "c291cmNlcz4KICAgPFJlc291cmNlPgogICAgPFJlc291cmNlTWF0Y2ggTWF0Y2hJ"
			+ "ZD0idXJuOm9hc2lzOm5hbWVzOnRjOnhhY21sOjEuMDpmdW5jdGlvbjpzdHJpbmct"
			+ "ZXF1YWwiPgogICAgIDxBdHRyaWJ1dGVWYWx1ZSBEYXRhVHlwZT0iaHR0cDovL3d3"
			+ "dy53My5vcmcvMjAwMS9YTUxTY2hlbWEjYW55VVJJIj4KICAgICAgaHR0cDovL3p1"
			+ "bmkuY3MudnQuZWR1PC9BdHRyaWJ1dGVWYWx1ZT4KICAgICA8UmVzb3VyY2VBdHRy"
			+ "aWJ1dGVEZXNpZ25hdG9yIEF0dHJpYnV0ZUlkPSJ1cm46b2FzaXM6bmFtZXM6dGM6"
			+ "eGFjbWw6MS4wOnJlc291cmNlOnJlc291cmNlLWlkIiBEYXRhVHlwZT0iaHR0cDov"
			+ "L3d3dy53My5vcmcvMjAwMS9YTUxTY2hlbWEjYW55VVJJIiAvPiAKICAgIDwvUmVz"
			+ "b3VyY2VNYXRjaD4KICAgPC9SZXNvdXJjZT4KICA8L1Jlc291cmNlcz4KICA8QWN0"
			+ "aW9ucz4KICAgPEFjdGlvbj4KICAgIDxBY3Rpb25NYXRjaCBNYXRjaElkPSJ1cm46"
			+ "b2FzaXM6bmFtZXM6dGM6eGFjbWw6MS4wOmZ1bmN0aW9uOnN0cmluZy1lcXVhbCI+"
			+ "CiAgICAgPEF0dHJpYnV0ZVZhbHVlIERhdGFUeXBlPSJodHRwOi8vd3d3LnczLm9y"
			+ "Zy8yMDAxL1hNTFNjaGVtYSNzdHJpbmciPgpEZWxlZ2F0ZSBBY2Nlc3MgICAgIDwv"
			+ "QXR0cmlidXRlVmFsdWU+CgkgIDxBY3Rpb25BdHRyaWJ1dGVEZXNpZ25hdG9yIEF0"
			+ "dHJpYnV0ZUlkPSJ1cm46b2FzaXM6bmFtZXM6dGM6eGFjbWw6MS4wOmFjdGlvbjph"
			+ "Y3Rpb24taWQiIERhdGFUeXBlPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxL1hNTFNj"
			+ "aGVtYSNzdHJpbmciIC8+IAogICAgPC9BY3Rpb25NYXRjaD4KICAgPC9BY3Rpb24+"
			+ "CiAgPC9BY3Rpb25zPgogPC9UYXJnZXQ+CjwvUnVsZT4KMA0GCSqGSIb3DQEBBAUA"
			+ "A4GBAGiJSM48XsY90HlYxGmGVSmNR6ZW2As+bot3KAfiCIkUIOAqhcphBS23egTr"
			+ "6asYwy151HshbPNYz+Cgeqs45KkVzh7bL/0e1r8sDVIaaGIkjHK3CqBABnfSayr3"
			+ "Rd1yBoDdEv8Qb+3eEPH6ab9021AsLEnJ6LWTmybbOpMNZ3tv");

		private static IAsymmetricCipherKeyPairGenerator Kpg
		{
			get
			{
				if (kpg == null)
				{
					kpg = GeneratorUtilities.GetKeyPairGenerator("RSA");
					kpg.Init(new RsaKeyGenerationParameters(
						BigInteger.ValueOf(17), rand, 1024, 25));
				}

				return kpg;
			}
		}

		private static IAsymmetricCipherKeyPairGenerator GostKpg
		{
			get
			{
				if (gostKpg == null)
				{
					gostKpg = GeneratorUtilities.GetKeyPairGenerator("GOST3410");
					gostKpg.Init(
						new Gost3410KeyGenerationParameters(
							rand,
							CryptoProObjectIdentifiers.GostR3410x94CryptoProA));
				}

				return gostKpg;
			}
		}

		private static IAsymmetricCipherKeyPairGenerator DsaKpg
		{
			get
			{
				if (dsaKpg == null)
				{
					DsaParameters dsaSpec = new DsaParameters(
						new BigInteger("7434410770759874867539421675728577177024889699586189000788950934679315164676852047058354758883833299702695428196962057871264685291775577130504050839126673"),
						new BigInteger("1138656671590261728308283492178581223478058193247"),
						new BigInteger("4182906737723181805517018315469082619513954319976782448649747742951189003482834321192692620856488639629011570381138542789803819092529658402611668375788410"));
					dsaKpg = GeneratorUtilities.GetKeyPairGenerator("DSA");
					dsaKpg.Init(new DsaKeyGenerationParameters(rand, dsaSpec));
				}

				return dsaKpg;
			}
		}

		private static IAsymmetricCipherKeyPairGenerator ECGostKpg
		{
			get
			{
				if (ecGostKpg == null)
				{
					ecGostKpg = GeneratorUtilities.GetKeyPairGenerator("ECGOST3410");
					ecGostKpg.Init(
						new ECKeyGenerationParameters(
							CryptoProObjectIdentifiers.GostR3410x2001CryptoProA,
							new SecureRandom()));
				}

				return ecGostKpg;
			}
		}

		private static IAsymmetricCipherKeyPairGenerator ECDsaKpg
		{
			get
			{
				if (ecDsaKpg == null)
				{
					ecDsaKpg = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
					ecDsaKpg.Init(new KeyGenerationParameters(rand, 239));
				}

				return ecDsaKpg;
			}
		}

		static CmsTestUtil()
		{
		    try
		    {
		        rand = new SecureRandom();

				aes192kg = GeneratorUtilities.GetKeyGenerator("AES");
				aes192kg.Init(new KeyGenerationParameters(rand, 192));

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

				aesKg = GeneratorUtilities.GetKeyGenerator("AES");

				seedKg = GeneratorUtilities.GetKeyGenerator("SEED");

				camelliaKg = GeneratorUtilities.GetKeyGenerator("Camellia");

				serialNumber = BigInteger.One;
		    }
		    catch (Exception ex)
		    {
		        throw new Exception(ex.ToString());
		    }
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
					buf.Append(Encoding.Default.GetString(data, i, 64));
				}
				else
				{
					buf.Append(Encoding.Default.GetString(data, i, data.Length - i));
				}
				buf.Append('\n');
			}

			return buf.ToString();
		}

		public static IX509AttributeCertificate GetAttributeCertificate()
		{
//			X509StreamParser parser = X509StreamParser.GetInstance("AttributeCertificate");
//			parser.Init(CmsTestUtil.attrCert);
//			return (X509AttributeCertificate) parser.Read();

			return new X509AttrCertParser().ReadAttrCert(attrCert);
		}

		public static AsymmetricCipherKeyPair MakeKeyPair()
		{
			return Kpg.GenerateKeyPair();
		}

		public static AsymmetricCipherKeyPair MakeGostKeyPair()
		{
			return GostKpg.GenerateKeyPair();
		}

		public static AsymmetricCipherKeyPair MakeDsaKeyPair()
		{
			return DsaKpg.GenerateKeyPair();
		}

		public static AsymmetricCipherKeyPair MakeECGostKeyPair()
		{
			return ECGostKpg.GenerateKeyPair();
		}

		public static AsymmetricCipherKeyPair MakeECDsaKeyPair()
		{
			return ECDsaKpg.GenerateKeyPair();
		}

		public static KeyParameter MakeDesEde128Key()
		{
			return ParameterUtilities.CreateKeyParameter("DESEDE", desede128kg.GenerateKey());
		}

		public static KeyParameter MakeAes192Key()
		{
			return ParameterUtilities.CreateKeyParameter("AES", aes192kg.GenerateKey());
		}

		public static KeyParameter MakeDesEde192Key()
		{
			return ParameterUtilities.CreateKeyParameter("DESEDE", desede192kg.GenerateKey());
		}

		public static KeyParameter MakeRC240Key()
		{
			return ParameterUtilities.CreateKeyParameter("RC2", rc240kg.GenerateKey());
		}

		public static KeyParameter MakeRC264Key()
		{
			return ParameterUtilities.CreateKeyParameter("RC2", rc264kg.GenerateKey());
		}

		public static KeyParameter MakeRC2128Key()
		{
			return ParameterUtilities.CreateKeyParameter("RC2", rc2128kg.GenerateKey());
		}

		public static KeyParameter MakeSeedKey()
		{
			return ParameterUtilities.CreateKeyParameter("SEED", seedKg.GenerateKey());
		}

		public static KeyParameter MakeAesKey(
			int keySize)
		{
			aesKg.Init(new KeyGenerationParameters(rand, keySize));

			return ParameterUtilities.CreateKeyParameter("AES", aesKg.GenerateKey());
		}

		public static KeyParameter MakeCamelliaKey(
			int keySize)
		{
			camelliaKg.Init(new KeyGenerationParameters(rand, keySize));

			return ParameterUtilities.CreateKeyParameter("CAMELLIA", camelliaKg.GenerateKey());
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

		public static X509Certificate MakeV1Certificate(AsymmetricCipherKeyPair subKP,
			string _subDN, AsymmetricCipherKeyPair issKP, string _issDN)
		{
			AsymmetricKeyParameter subPub = subKP.Public;
			AsymmetricKeyParameter issPriv = issKP.Private;
			AsymmetricKeyParameter issPub = issKP.Public;

			X509V1CertificateGenerator v1CertGen = new X509V1CertificateGenerator();

			v1CertGen.Reset();
			v1CertGen.SetSerialNumber(AllocateSerialNumber());
			v1CertGen.SetIssuerDN(new X509Name(_issDN));
			v1CertGen.SetNotBefore(DateTime.UtcNow);
			v1CertGen.SetNotAfter(DateTime.UtcNow.AddDays(100));
			v1CertGen.SetSubjectDN(new X509Name(_subDN));
			v1CertGen.SetPublicKey(subPub);

			if (issPub is RsaKeyParameters)
			{
				v1CertGen.SetSignatureAlgorithm("SHA1WithRSA");
			}
			else if (issPub is DsaPublicKeyParameters)
			{
				v1CertGen.SetSignatureAlgorithm("SHA1withDSA");
			}
			else if (issPub is ECPublicKeyParameters)
			{
				ECPublicKeyParameters ecPub = (ECPublicKeyParameters)issPub;
				if (ecPub.AlgorithmName == "ECGOST3410")
				{
					v1CertGen.SetSignatureAlgorithm("GOST3411withECGOST3410");
				}
				else
				{
					v1CertGen.SetSignatureAlgorithm("SHA1withECDSA");
				}	
			}
			else
			{
				v1CertGen.SetSignatureAlgorithm("GOST3411WithGOST3410");
			}

			X509Certificate _cert = v1CertGen.Generate(issPriv);

			_cert.CheckValidity(DateTime.UtcNow);
			_cert.Verify(issPub);

			return _cert;
		}

		public static X509Certificate MakeCertificate(
			AsymmetricCipherKeyPair subKP, string _subDN,
			AsymmetricCipherKeyPair issKP, string _issDN, bool _ca)
		{
			AsymmetricKeyParameter subPub = subKP.Public;
			AsymmetricKeyParameter issPriv = issKP.Private;
			AsymmetricKeyParameter issPub = issKP.Public;

			X509V3CertificateGenerator v3CertGen = new X509V3CertificateGenerator();

			v3CertGen.Reset();
			v3CertGen.SetSerialNumber(AllocateSerialNumber());
			v3CertGen.SetIssuerDN(new X509Name(_issDN));
			v3CertGen.SetNotBefore(DateTime.UtcNow);
			v3CertGen.SetNotAfter(DateTime.UtcNow.AddDays(100));
			v3CertGen.SetSubjectDN(new X509Name(_subDN));
			v3CertGen.SetPublicKey(subPub);

			if (issPub is RsaKeyParameters)
			{
				v3CertGen.SetSignatureAlgorithm("SHA1WithRSA");
			}
			else if (issPub is ECPublicKeyParameters)
			{
				ECPublicKeyParameters ecPub = (ECPublicKeyParameters) issPub;
				if (ecPub.AlgorithmName == "ECGOST3410")
				{
					v3CertGen.SetSignatureAlgorithm("GOST3411withECGOST3410");
				}
				else
				{
					v3CertGen.SetSignatureAlgorithm("SHA1withECDSA");
				}
			}
			else
			{
				v3CertGen.SetSignatureAlgorithm("GOST3411WithGOST3410");
			}

			v3CertGen.AddExtension(
				X509Extensions.SubjectKeyIdentifier,
				false,
				CreateSubjectKeyId(subPub));

			v3CertGen.AddExtension(
				X509Extensions.AuthorityKeyIdentifier,
				false,
				CreateAuthorityKeyId(issPub));

			v3CertGen.AddExtension(
				X509Extensions.BasicConstraints,
				false,
				new BasicConstraints(_ca));

			X509Certificate _cert = v3CertGen.Generate(issPriv);

			_cert.CheckValidity();
			_cert.Verify(issPub);

			return _cert;
		}

		public static X509Crl MakeCrl(
			AsymmetricCipherKeyPair pair)
		{
			X509V2CrlGenerator crlGen = new X509V2CrlGenerator();
			DateTime now = DateTime.UtcNow;

			crlGen.SetIssuerDN(new X509Name("CN=Test CA"));

			crlGen.SetThisUpdate(now);
			crlGen.SetNextUpdate(now.AddSeconds(100));
			crlGen.SetSignatureAlgorithm("SHA256WithRSAEncryption");

			crlGen.AddCrlEntry(BigInteger.One, now, CrlReason.PrivilegeWithdrawn);

			crlGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(pair.Public));

			return crlGen.Generate(pair.Private);
		}

		/*
		*
		*  INTERNAL METHODS
		*
		*/
        internal static IX509Store MakeAttrCertStore(params IX509AttributeCertificate[] attrCerts)
        {
            IList attrCertList = new ArrayList();
            foreach (IX509AttributeCertificate attrCert in attrCerts)
            {
                attrCertList.Add(attrCert);
            }

            return X509StoreFactory.Create("AttributeCertificate/Collection", new X509CollectionStoreParameters(attrCertList));
        }

        internal static IX509Store MakeCertStore(params X509Certificate[] certs)
        {
            IList certList = new ArrayList();
            foreach (X509Certificate cert in certs)
            {
                certList.Add(cert);
            }

            return X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(certList));
        }

        internal static IX509Store MakeCrlStore(params X509Crl[] crls)
        {
            IList crlList = new ArrayList();
            foreach (X509Crl crl in crls)
            {
                crlList.Add(crl);
            }

            return X509StoreFactory.Create("CRL/Collection", new X509CollectionStoreParameters(crlList));
        }

        private static AuthorityKeyIdentifier CreateAuthorityKeyId(
			AsymmetricKeyParameter _pubKey)
		{
			SubjectPublicKeyInfo _info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_pubKey);
			return new AuthorityKeyIdentifier(_info);
		}

		internal static SubjectKeyIdentifier CreateSubjectKeyId(
			AsymmetricKeyParameter _pubKey)
		{
			SubjectPublicKeyInfo _info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_pubKey);
			return new SubjectKeyIdentifier(_info);
		}

		private static BigInteger AllocateSerialNumber()
		{
			BigInteger _tmp = serialNumber;
			serialNumber = serialNumber.Add(BigInteger.One);
			return _tmp;
		}

		public static byte[] StreamToByteArray(
			Stream inStream)
		{
			return Streams.ReadAll(inStream);
		}
	}
}
