using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.X509
{
	public class X509CrlParser
	{
		private static readonly PemParser PemCrlParser = new PemParser("CRL");

		private readonly bool lazyAsn1;

		private Asn1Set	sCrlData;
		private int		sCrlDataObjectCount;
		private Stream	currentCrlStream;

		public X509CrlParser()
			: this(false)
		{
		}

		public X509CrlParser(
			bool lazyAsn1)
		{
			this.lazyAsn1 = lazyAsn1;
		}

		private X509Crl ReadPemCrl(
			Stream inStream)
		{
			Asn1Sequence seq = PemCrlParser.ReadPemObject(inStream);

			return seq == null
				?	null
				:	CreateX509Crl(CertificateList.GetInstance(seq));
		}

		private X509Crl ReadDerCrl(
			Asn1InputStream dIn)
		{
			Asn1Sequence seq = (Asn1Sequence)dIn.ReadObject();

			if (seq.Count > 1 && seq[0] is DerObjectIdentifier)
			{
				if (seq[0].Equals(PkcsObjectIdentifiers.SignedData))
				{
					sCrlData = SignedData.GetInstance(
						Asn1Sequence.GetInstance((Asn1TaggedObject) seq[1], true)).Crls;

					return GetCrl();
				}
			}

			return CreateX509Crl(CertificateList.GetInstance(seq));
		}

		private X509Crl GetCrl()
		{
			if (sCrlData == null || sCrlDataObjectCount >= sCrlData.Count)
			{
				return null;
			}

			return CreateX509Crl(
				CertificateList.GetInstance(
					sCrlData[sCrlDataObjectCount++]));
		}

		protected virtual X509Crl CreateX509Crl(
			CertificateList c)
		{
			return new X509Crl(c);
		}

		/// <summary>
		/// Create loading data from byte array.
		/// </summary>
		/// <param name="input"></param>
		public X509Crl ReadCrl(
			byte[] input)
		{
			return ReadCrl(new MemoryStream(input, false));
		}

		/// <summary>
		/// Create loading data from byte array.
		/// </summary>
		/// <param name="input"></param>
		public ICollection ReadCrls(
			byte[] input)
		{
			return ReadCrls(new MemoryStream(input, false));
		}

		/**
		 * Generates a certificate revocation list (CRL) object and initializes
		 * it with the data read from the input stream inStream.
		 */
		public X509Crl ReadCrl(
			Stream inStream)
		{
			if (inStream == null)
				throw new ArgumentNullException("inStream");
			if (!inStream.CanRead)
				throw new ArgumentException("inStream must be read-able", "inStream");

			if (currentCrlStream == null)
			{
				currentCrlStream = inStream;
				sCrlData = null;
				sCrlDataObjectCount = 0;
			}
			else if (currentCrlStream != inStream) // reset if input stream has changed
			{
				currentCrlStream = inStream;
				sCrlData = null;
				sCrlDataObjectCount = 0;
			}

			try
			{
				if (sCrlData != null)
				{
					if (sCrlDataObjectCount != sCrlData.Count)
					{
						return GetCrl();
					}

					sCrlData = null;
					sCrlDataObjectCount = 0;
					return null;
				}

				PushbackStream pis = new PushbackStream(inStream);
				int tag = pis.ReadByte();

				if (tag < 0)
					return null;

				pis.Unread(tag);

				if (tag != 0x30)	// assume ascii PEM encoded.
				{
					return ReadPemCrl(pis);
				}

				Asn1InputStream asn1 = lazyAsn1
					?	new LazyAsn1InputStream(pis)
					:	new Asn1InputStream(pis);

				return ReadDerCrl(asn1);
			}
			catch (CrlException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new CrlException(e.ToString());
			}
		}

		/**
		 * Returns a (possibly empty) collection view of the CRLs read from
		 * the given input stream inStream.
		 *
		 * The inStream may contain a sequence of DER-encoded CRLs, or
		 * a PKCS#7 CRL set.  This is a PKCS#7 SignedData object, with the
		 * only significant field being crls.  In particular the signature
		 * and the contents are ignored.
		 */
		public ICollection ReadCrls(
			Stream inStream)
		{
			X509Crl crl;
			IList crls = Platform.CreateArrayList();

			while ((crl = ReadCrl(inStream)) != null)
			{
				crls.Add(crl);
			}

			return crls;
		}
	}
}
