using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Extension;

namespace Org.BouncyCastle.X509
{
	/**
	 * The following extensions are listed in RFC 2459 as relevant to CRL Entries
	 *
	 * ReasonCode Hode Instruction Code Invalidity Date Certificate Issuer
	 * (critical)
	 */
	public class X509CrlEntry
		: X509ExtensionBase
	{
		private CrlEntry	c;
		private bool		isIndirect;
		private X509Name	previousCertificateIssuer;
		private X509Name	certificateIssuer;

		public X509CrlEntry(
			CrlEntry c)
		{
			this.c = c;
			this.certificateIssuer = loadCertificateIssuer();
		}

		/**
		* Constructor for CRLEntries of indirect CRLs. If <code>isIndirect</code>
		* is <code>false</code> {@link #getCertificateIssuer()} will always
		* return <code>null</code>, <code>previousCertificateIssuer</code> is
		* ignored. If this <code>isIndirect</code> is specified and this CrlEntry
		* has no certificate issuer CRL entry extension
		* <code>previousCertificateIssuer</code> is returned by
		* {@link #getCertificateIssuer()}.
		*
		* @param c
		*            TbsCertificateList.CrlEntry object.
		* @param isIndirect
		*            <code>true</code> if the corresponding CRL is a indirect
		*            CRL.
		* @param previousCertificateIssuer
		*            Certificate issuer of the previous CrlEntry.
		*/
		public X509CrlEntry(
			CrlEntry		c,
			bool			isIndirect,
			X509Name		previousCertificateIssuer)
		{
			this.c = c;
			this.isIndirect = isIndirect;
			this.previousCertificateIssuer = previousCertificateIssuer;
			this.certificateIssuer = loadCertificateIssuer();
		}

		private X509Name loadCertificateIssuer()
		{
			if (!isIndirect)
			{
				return null;
			}

			Asn1OctetString ext = GetExtensionValue(X509Extensions.CertificateIssuer);
			if (ext == null)
			{
				return previousCertificateIssuer;
			}

			try
			{
				GeneralName[] names = GeneralNames.GetInstance(
					X509ExtensionUtilities.FromExtensionValue(ext)).GetNames();

				for (int i = 0; i < names.Length; i++)
				{
					if (names[i].TagNo == GeneralName.DirectoryName)
					{
						return X509Name.GetInstance(names[i].Name);
					}
				}
			}
			catch (Exception)
			{
			}

			return null;
		}

		public X509Name GetCertificateIssuer()
		{
			return certificateIssuer;
		}

		protected override X509Extensions GetX509Extensions()
		{
			return c.Extensions;
		}

		public byte[] GetEncoded()
		{
			try
			{
				return c.GetDerEncoded();
			}
			catch (Exception e)
			{
				throw new CrlException(e.ToString());
			}
		}

		public BigInteger SerialNumber
		{
			get { return c.UserCertificate.Value; }
		}

		public DateTime RevocationDate
		{
			get { return c.RevocationDate.ToDateTime(); }
		}

		public bool HasExtensions
		{
			get { return c.Extensions != null; }
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			string nl = Platform.NewLine;

			buf.Append("        userCertificate: ").Append(this.SerialNumber).Append(nl);
			buf.Append("         revocationDate: ").Append(this.RevocationDate).Append(nl);
			buf.Append("      certificateIssuer: ").Append(this.GetCertificateIssuer()).Append(nl);

			X509Extensions extensions = c.Extensions;

			if (extensions != null)
			{
				IEnumerator e = extensions.ExtensionOids.GetEnumerator();
				if (e.MoveNext())
				{
					buf.Append("   crlEntryExtensions:").Append(nl);

					do
					{
						DerObjectIdentifier oid = (DerObjectIdentifier)e.Current;
						X509Extension ext = extensions.GetExtension(oid);

						if (ext.Value != null)
						{
							Asn1Object obj = Asn1Object.FromByteArray(ext.Value.GetOctets());

							buf.Append("                       critical(")
								.Append(ext.IsCritical)
								.Append(") ");
							try
							{
								if (oid.Equals(X509Extensions.ReasonCode))
								{
									buf.Append(new CrlReason(DerEnumerated.GetInstance(obj)));
								}
								else if (oid.Equals(X509Extensions.CertificateIssuer))
								{
									buf.Append("Certificate issuer: ").Append(
										GeneralNames.GetInstance((Asn1Sequence)obj));
								}
								else 
								{
									buf.Append(oid.Id);
									buf.Append(" value = ").Append(Asn1Dump.DumpAsString(obj));
								}
								buf.Append(nl);
							}
							catch (Exception)
							{
								buf.Append(oid.Id);
								buf.Append(" value = ").Append("*****").Append(nl);
							}
						}
						else
						{
							buf.Append(nl);
						}
					}
					while (e.MoveNext());
				}
			}

			return buf.ToString();
		}
	}
}
