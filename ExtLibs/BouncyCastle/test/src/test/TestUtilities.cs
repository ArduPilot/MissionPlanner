using System;
using System.Diagnostics;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;

namespace Org.BouncyCastle.Tests
{
	/**
	 * Test Utils
	 */
	internal class TestUtilities
	{
		/**
		 * Create a random 1024 bit RSA key pair
		 */
		public static AsymmetricCipherKeyPair GenerateRsaKeyPair()
		{
			IAsymmetricCipherKeyPairGenerator kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");

			kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 1024));

			return kpGen.GenerateKeyPair();
		}

		public static X509Certificate GenerateRootCert(
			AsymmetricCipherKeyPair pair)
		{
			X509V1CertificateGenerator  certGen = new X509V1CertificateGenerator();

			certGen.SetSerialNumber(BigInteger.One);
			certGen.SetIssuerDN(new X509Name("CN=Test CA Certificate"));
			certGen.SetNotBefore(DateTime.UtcNow.AddSeconds(-50));
			certGen.SetNotAfter(DateTime.UtcNow.AddSeconds(50));
			certGen.SetSubjectDN(new X509Name("CN=Test CA Certificate"));
			certGen.SetPublicKey(pair.Public);
			certGen.SetSignatureAlgorithm("SHA256WithRSAEncryption");

			return certGen.Generate(pair.Private);
		}

		public static X509Certificate GenerateIntermediateCert(
			AsymmetricKeyParameter	intKey,
			AsymmetricKeyParameter	caKey,
			X509Certificate			caCert)
		{
			X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();

			certGen.SetSerialNumber(BigInteger.One);
			certGen.SetIssuerDN(PrincipalUtilities.GetSubjectX509Principal(caCert));
			certGen.SetNotBefore(DateTime.UtcNow.AddSeconds(-50));
			certGen.SetNotAfter(DateTime.UtcNow.AddSeconds(50));
			certGen.SetSubjectDN(new X509Name("CN=Test Intermediate Certificate"));
			certGen.SetPublicKey(intKey);
			certGen.SetSignatureAlgorithm("SHA256WithRSAEncryption");

			certGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(caCert));
			certGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(intKey));
			certGen.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(0));
			certGen.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyCertSign | KeyUsage.CrlSign));

			return certGen.Generate(caKey);
		}

		public static X509Certificate GenerateEndEntityCert(
			AsymmetricKeyParameter entityKey,
			AsymmetricKeyParameter caKey,
			X509Certificate caCert)
		{
			X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();

			certGen.SetSerialNumber(BigInteger.One);
			certGen.SetIssuerDN(PrincipalUtilities.GetSubjectX509Principal(caCert));
			certGen.SetNotBefore(DateTime.UtcNow.AddSeconds(-50));
			certGen.SetNotAfter(DateTime.UtcNow.AddSeconds(50));
			certGen.SetSubjectDN(new X509Name("CN=Test End Certificate"));
			certGen.SetPublicKey(entityKey);
			certGen.SetSignatureAlgorithm("SHA256WithRSAEncryption");

			certGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(caCert));
			certGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(entityKey));
			certGen.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(false));
			certGen.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment));

			return certGen.Generate(caKey);
		}

		public static X509Crl CreateCrl(
			X509Certificate			caCert, 
			AsymmetricKeyParameter	caKey, 
			BigInteger				serialNumber)
		{
			X509V2CrlGenerator	crlGen = new X509V2CrlGenerator();
			DateTime			now = DateTime.UtcNow;
//			BigInteger			revokedSerialNumber = BigInteger.Two;

			crlGen.SetIssuerDN(PrincipalUtilities.GetSubjectX509Principal(caCert));

			crlGen.SetThisUpdate(now);
			crlGen.SetNextUpdate(now.AddSeconds(100));
			crlGen.SetSignatureAlgorithm("SHA256WithRSAEncryption");

			crlGen.AddCrlEntry(serialNumber, now, CrlReason.PrivilegeWithdrawn);

			crlGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(caCert));
			crlGen.AddExtension(X509Extensions.CrlNumber, false, new CrlNumber(BigInteger.One));

			return crlGen.Generate(caKey);
		}

		public static X509Certificate CreateExceptionCertificate(
			bool exceptionOnEncode)
		{
			return new ExceptionCertificate(exceptionOnEncode);
		}

		private class ExceptionCertificate
			: X509Certificate
		{
			private bool _exceptionOnEncode;

			public ExceptionCertificate(
				bool exceptionOnEncode)
			{
				_exceptionOnEncode = exceptionOnEncode;
			}

			public override void CheckValidity()
			{
				throw new CertificateNotYetValidException();
			}

			public override void CheckValidity(
				DateTime date)
			{
				throw new CertificateExpiredException();
			}

			public override int Version
			{
				get { return 0; }
			}

			public override BigInteger SerialNumber
			{
				get { return null; }
			}

			public override X509Name IssuerDN
			{
				get { return null; }
			}

			public override X509Name SubjectDN
			{
				get { return null; }
			}

			public override DateTime NotBefore
			{
				get { return DateTime.MaxValue; }
			}

			public override DateTime NotAfter
			{
				get { return DateTime.MinValue; }
			}

			public override byte[] GetTbsCertificate()
			{
				throw new CertificateEncodingException();
			}

			public override byte[] GetSignature()
			{
				return new byte[0];
			}

			public override string SigAlgName
			{
				get { return null; }
			}

			public override string SigAlgOid
			{
				get { return null; }
			}

			public override byte[] GetSigAlgParams()
			{
				return new byte[0];
			}

			public override DerBitString IssuerUniqueID
			{
				get { return null; }
			}

			public override DerBitString SubjectUniqueID
			{
				get { return null; }
			}

			public override bool[] GetKeyUsage()
			{
				return new bool[0];
			}

			public override int GetBasicConstraints()
			{
				return 0;
			}

			public override byte[] GetEncoded()
			{
				if (_exceptionOnEncode)
				{
					throw new CertificateEncodingException();
				}

				return new byte[0];
			}

			public override void Verify(
				AsymmetricKeyParameter key)
			{
				throw new CertificateException();
			}

			public override string ToString()
			{
				return null;
			}

			public override AsymmetricKeyParameter GetPublicKey()
			{
				return null;
			}

			public override ISet GetCriticalExtensionOids()
			{
				return null;
			}

			public override ISet GetNonCriticalExtensionOids()
			{
				return null;
			}

			public override Asn1OctetString GetExtensionValue(
				DerObjectIdentifier oid)
			{
				return null;
			}
		}
	}
}
