﻿using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Extension;

namespace Org.BouncyCastle.X509
{
    /// <summary>
    /// A class to Generate Version 3 X509Certificates.
    /// </summary>
    public class X509V3CertificateGenerator
    {
		private readonly X509ExtensionsGenerator extGenerator = new X509ExtensionsGenerator();

		private V3TbsCertificateGenerator	tbsGen;
        private DerObjectIdentifier			sigOid;
        private AlgorithmIdentifier			sigAlgId;
        private string						signatureAlgorithm;

		public X509V3CertificateGenerator()
        {
            tbsGen = new V3TbsCertificateGenerator();
        }

		/// <summary>
		/// Reset the Generator.
		/// </summary>
		public void Reset()
		{
			tbsGen = new V3TbsCertificateGenerator();
			extGenerator.Reset();
		}

		/// <summary>
        /// Set the certificate's serial number.
        /// </summary>
        /// <remarks>Make serial numbers long, if you have no serial number policy make sure the number is at least 16 bytes of secure random data.
        /// You will be surprised how ugly a serial number collision can Get.</remarks>
        /// <param name="serialNumber">The serial number.</param>
        public void SetSerialNumber(
			BigInteger serialNumber)
        {
			if (serialNumber.SignValue <= 0)
			{
				throw new ArgumentException("serial number must be a positive integer", "serialNumber");
			}

			tbsGen.SetSerialNumber(new DerInteger(serialNumber));
        }

		/// <summary>
        /// Set the distinguished name of the issuer.
        /// The issuer is the entity which is signing the certificate.
        /// </summary>
        /// <param name="issuer">The issuer's DN.</param>
        public void SetIssuerDN(
            X509Name issuer)
        {
            tbsGen.SetIssuer(issuer);
        }

		/// <summary>
        /// Set the date that this certificate is to be valid from.
        /// </summary>
        /// <param name="date"/>
        public void SetNotBefore(
            DateTime date)
        {
            tbsGen.SetStartDate(new Time(date));
        }

        /// <summary>
        /// Set the date after which this certificate will no longer be valid.
        /// </summary>
        /// <param name="date"/>
        public void SetNotAfter(
			DateTime date)
        {
            tbsGen.SetEndDate(new Time(date));
        }

		/// <summary>
		/// Set the DN of the entity that this certificate is about.
		/// </summary>
		/// <param name="subject"/>
        public void SetSubjectDN(
			X509Name subject)
        {
            tbsGen.SetSubject(subject);
        }

		/// <summary>
        /// Set the public key that this certificate identifies.
        /// </summary>
        /// <param name="publicKey"/>
        public void SetPublicKey(
			AsymmetricKeyParameter publicKey)
        {
            tbsGen.SetSubjectPublicKeyInfo(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey));
        }

		/// <summary>
        /// Set the signature algorithm that will be used to sign this certificate.
        /// </summary>
        /// <param name="signatureAlgorithm"/>
		[Obsolete("Not needed if Generate used with an ISignatureFactory")]
        public void SetSignatureAlgorithm(
			string signatureAlgorithm)
        {
			this.signatureAlgorithm = signatureAlgorithm;

			try
			{
				sigOid = X509Utilities.GetAlgorithmOid(signatureAlgorithm);
			}
			catch (Exception)
			{
				throw new ArgumentException("Unknown signature type requested: " + signatureAlgorithm);
			}

			sigAlgId = X509Utilities.GetSigAlgID(sigOid, signatureAlgorithm);

			tbsGen.SetSignature(sigAlgId);
		}

		/// <summary>
		/// Set the subject unique ID - note: it is very rare that it is correct to do this.
		/// </summary>
		/// <param name="uniqueID"/>
		public void SetSubjectUniqueID(
			bool[] uniqueID)
		{
			tbsGen.SetSubjectUniqueID(booleanToBitString(uniqueID));
		}

		/// <summary>
		/// Set the issuer unique ID - note: it is very rare that it is correct to do this.
		/// </summary>
		/// <param name="uniqueID"/>
		public void SetIssuerUniqueID(
			bool[] uniqueID)
		{
			tbsGen.SetIssuerUniqueID(booleanToBitString(uniqueID));
		}

		private DerBitString booleanToBitString(
			bool[] id)
		{
			byte[] bytes = new byte[(id.Length + 7) / 8];

			for (int i = 0; i != id.Length; i++)
			{
				if (id[i])
				{
					bytes[i / 8] |= (byte)(1 << ((7 - (i % 8))));
				}
			}

			int pad = id.Length % 8;

			if (pad == 0)
			{
				return new DerBitString(bytes);
			}

			return new DerBitString(bytes, 8 - pad);
		}

		/// <summary>
		/// Add a given extension field for the standard extensions tag (tag 3).
		/// </summary>
		/// <param name="oid">string containing a dotted decimal Object Identifier.</param>
		/// <param name="critical">Is it critical.</param>
		/// <param name="extensionValue">The value.</param>
		public void AddExtension(
			string			oid,
			bool			critical,
			Asn1Encodable	extensionValue)
		{
			extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, extensionValue);
		}

		/// <summary>
        /// Add an extension to this certificate.
        /// </summary>
        /// <param name="oid">Its Object Identifier.</param>
        /// <param name="critical">Is it critical.</param>
        /// <param name="extensionValue">The value.</param>
        public void AddExtension(
			DerObjectIdentifier	oid,
			bool				critical,
			Asn1Encodable		extensionValue)
        {
			extGenerator.AddExtension(oid, critical, extensionValue);
        }

		/// <summary>
		/// Add an extension using a string with a dotted decimal OID.
		/// </summary>
		/// <param name="oid">string containing a dotted decimal Object Identifier.</param>
		/// <param name="critical">Is it critical.</param>
		/// <param name="extensionValue">byte[] containing the value of this extension.</param>
		public void AddExtension(
			string	oid,
			bool	critical,
			byte[]	extensionValue)
		{
			extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, new DerOctetString(extensionValue));
		}

		/// <summary>
        /// Add an extension to this certificate.
        /// </summary>
        /// <param name="oid">Its Object Identifier.</param>
        /// <param name="critical">Is it critical.</param>
        /// <param name="extensionValue">byte[] containing the value of this extension.</param>
        public void AddExtension(
			DerObjectIdentifier	oid,
			bool				critical,
			byte[]				extensionValue)
        {
			extGenerator.AddExtension(oid, critical, new DerOctetString(extensionValue));
        }

		/// <summary>
		/// Add a given extension field for the standard extensions tag (tag 3),
		/// copying the extension value from another certificate.
		/// </summary>
		public void CopyAndAddExtension(
			string			oid,
			bool			critical,
			X509Certificate	cert)
		{
			CopyAndAddExtension(new DerObjectIdentifier(oid), critical, cert);
		}

		/**
		 * add a given extension field for the standard extensions tag (tag 3)
		 * copying the extension value from another certificate.
		 * @throws CertificateParsingException if the extension cannot be extracted.
		 */
		public void CopyAndAddExtension(
			DerObjectIdentifier	oid,
			bool				critical,
			X509Certificate		cert)
		{
			Asn1OctetString extValue = cert.GetExtensionValue(oid);

			if (extValue == null)
			{
				throw new CertificateParsingException("extension " + oid + " not present");
			}

			try
			{
				Asn1Encodable value = X509ExtensionUtilities.FromExtensionValue(extValue);

				this.AddExtension(oid, critical, value);
			}
			catch (Exception e)
			{
				throw new CertificateParsingException(e.Message, e);
			}
		}

		/// <summary>
        /// Generate an X509Certificate.
        /// </summary>
        /// <param name="privateKey">The private key of the issuer that is signing this certificate.</param>
        /// <returns>An X509Certificate.</returns>
		[Obsolete("Use Generate with an ISignatureFactory")]
		public X509Certificate Generate(
			AsymmetricKeyParameter privateKey)
        {
            return Generate(privateKey, null);
        }

		/// <summary>
		/// Generate an X509Certificate using your own SecureRandom.
		/// </summary>
		/// <param name="privateKey">The private key of the issuer that is signing this certificate.</param>
		/// <param name="random">You Secure Random instance.</param>
		/// <returns>An X509Certificate.</returns>
		[Obsolete("Use Generate with an ISignatureFactory")]
		public X509Certificate Generate(
			AsymmetricKeyParameter	privateKey,
			SecureRandom			random)
		{
			return Generate(new Asn1SignatureFactory(signatureAlgorithm, privateKey, random));
		}

		/// <summary>
		/// Generate a new X509Certificate using the passed in SignatureCalculator.
		/// </summary>
		/// <param name="signatureCalculatorFactory">A signature calculator factory with the necessary algorithm details.</param>
		/// <returns>An X509Certificate.</returns>
		public X509Certificate Generate(ISignatureFactory signatureCalculatorFactory)
		{
			tbsGen.SetSignature ((AlgorithmIdentifier)signatureCalculatorFactory.AlgorithmDetails);

            if (!extGenerator.IsEmpty)
            {
                tbsGen.SetExtensions(extGenerator.Generate());
            }

            TbsCertificateStructure tbsCert = tbsGen.GenerateTbsCertificate();

			IStreamCalculator streamCalculator = signatureCalculatorFactory.CreateCalculator();

			byte[] encoded = tbsCert.GetDerEncoded();

			streamCalculator.Stream.Write(encoded, 0, encoded.Length);

            Platform.Dispose(streamCalculator.Stream);

            return GenerateJcaObject(tbsCert, (AlgorithmIdentifier)signatureCalculatorFactory.AlgorithmDetails, ((IBlockResult)streamCalculator.GetResult()).Collect());
		}

		private X509Certificate GenerateJcaObject(
			TbsCertificateStructure	tbsCert,
			AlgorithmIdentifier     sigAlg,
			byte[]					signature)
		{
			return new X509Certificate(
				new X509CertificateStructure(tbsCert, sigAlg, new DerBitString(signature)));
		}

		/// <summary>
		/// Allows enumeration of the signature names supported by the generator.
		/// </summary>
		public IEnumerable SignatureAlgNames
		{
			get { return X509Utilities.GetAlgNames(); }
		}
	}
}
