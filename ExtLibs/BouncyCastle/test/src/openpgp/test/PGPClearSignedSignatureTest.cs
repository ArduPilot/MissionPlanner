using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
	[TestFixture]
	public class PgpClearSignedSignatureTest
		: SimpleTest
	{
		private static readonly byte[] publicKey = Base64.Decode(
			  "mQELBEQh2+wBCAD26kte0hO6flr7Y2aetpPYutHY4qsmDPy+GwmmqVeCDkX+"
			+ "r1g7DuFbMhVeu0NkKDnVl7GsJ9VarYsFYyqu0NzLa9XS2qlTIkmJV+2/xKa1"
			+ "tzjn18fT/cnAWL88ZLCOWUr241aPVhLuIc6vpHnySpEMkCh4rvMaimnTrKwO"
			+ "42kgeDGd5cXfs4J4ovRcTbc4hmU2BRVsRjiYMZWWx0kkyL2zDVyaJSs4yVX7"
			+ "Jm4/LSR1uC/wDT0IJJuZT/gQPCMJNMEsVCziRgYkAxQK3OWojPSuv4rXpyd4"
			+ "Gvo6IbvyTgIskfpSkCnQtORNLIudQSuK7pW+LkL62N+ohuKdMvdxauOnAAYp"
			+ "tBNnZ2dnZ2dnZyA8Z2dnQGdnZ2c+iQE2BBMBAgAgBQJEIdvsAhsDBgsJCAcD"
			+ "AgQVAggDBBYCAwECHgECF4AACgkQ4M/Ier3f9xagdAf/fbKWBjLQM8xR7JkR"
			+ "P4ri8YKOQPhK+VrddGUD59/wzVnvaGyl9MZE7TXFUeniQq5iXKnm22EQbYch"
			+ "v2Jcxyt2H9yptpzyh4tP6tEHl1C887p2J4qe7F2ATua9CzVGwXQSUbKtj2fg"
			+ "UZP5SsNp25guhPiZdtkf2sHMeiotmykFErzqGMrvOAUThrO63GiYsRk4hF6r"
			+ "cQ01d+EUVpY/sBcCxgNyOiB7a84sDtrxnX5BTEZDTEj8LvuEyEV3TMUuAjx1"
			+ "7Eyd+9JtKzwV4v3hlTaWOvGro9nPS7YaPuG+RtufzXCUJPbPfTjTvtGOqvEz"
			+ "oztls8tuWA0OGHba9XfX9rfgorACAAM=");

		private static readonly byte[] secretKey = Base64.Decode(
			  "lQOWBEQh2+wBCAD26kte0hO6flr7Y2aetpPYutHY4qsmDPy+GwmmqVeCDkX+"
			+ "r1g7DuFbMhVeu0NkKDnVl7GsJ9VarYsFYyqu0NzLa9XS2qlTIkmJV+2/xKa1"
			+ "tzjn18fT/cnAWL88ZLCOWUr241aPVhLuIc6vpHnySpEMkCh4rvMaimnTrKwO"
			+ "42kgeDGd5cXfs4J4ovRcTbc4hmU2BRVsRjiYMZWWx0kkyL2zDVyaJSs4yVX7"
			+ "Jm4/LSR1uC/wDT0IJJuZT/gQPCMJNMEsVCziRgYkAxQK3OWojPSuv4rXpyd4"
			+ "Gvo6IbvyTgIskfpSkCnQtORNLIudQSuK7pW+LkL62N+ohuKdMvdxauOnAAYp"
			+ "AAf+JCJJeAXEcrTVHotsrRR5idzmg6RK/1MSQUijwPmP7ZGy1BmpAmYUfbxn"
			+ "B56GvXyFV3Pbj9PgyJZGS7cY+l0BF4ZqN9USiQtC9OEpCVT5LVMCFXC/lahC"
			+ "/O3EkjQy0CYK+GwyIXa+Flxcr460L/Hvw2ZEXJZ6/aPdiR+DU1l5h99Zw8V1"
			+ "Y625MpfwN6ufJfqE0HLoqIjlqCfi1iwcKAK2oVx2SwnT1W0NwUUXjagGhD2s"
			+ "VzJVpLqhlwmS0A+RE9Niqrf80/zwE7QNDF2DtHxmMHJ3RY/pfu5u1rrFg9YE"
			+ "lmS60mzOe31CaD8Li0k5YCJBPnmvM9mN3/DWWprSZZKtmQQA96C2/VJF5EWm"
			+ "+/Yxi5J06dG6Bkz311Ui4p2zHm9/4GvTPCIKNpGx9Zn47YFD3tIg3fIBVPOE"
			+ "ktG38pEPx++dSSFF9Ep5UgmYFNOKNUVq3yGpatBtCQBXb1LQLAMBJCJ5TQmk"
			+ "68hMOEaqjMHSOa18cS63INgA6okb/ueAKIHxYQcEAP9DaXu5n9dZQw7pshbN"
			+ "Nu/T5IP0/D/wqM+W5r+j4P1N7PgiAnfKA4JjKrUgl8PGnI2qM/Qu+g3qK++c"
			+ "F1ESHasnJPjvNvY+cfti06xnJVtCB/EBOA2UZkAr//Tqa76xEwYAWRBnO2Y+"
			+ "KIVOT+nMiBFkjPTrNAD6fSr1O4aOueBhBAC6aA35IfjC2h5MYk8+Z+S4io2o"
			+ "mRxUZ/dUuS+kITvWph2e4DT28Xpycpl2n1Pa5dCDO1lRqe/5JnaDYDKqxfmF"
			+ "5tTG8GR4d4nVawwLlifXH5Ll7t5NcukGNMCsGuQAHMy0QHuAaOvMdLs5kGHn"
			+ "8VxfKEVKhVrXsvJSwyXXSBtMtUcRtBNnZ2dnZ2dnZyA8Z2dnQGdnZ2c+iQE2"
			+ "BBMBAgAgBQJEIdvsAhsDBgsJCAcDAgQVAggDBBYCAwECHgECF4AACgkQ4M/I"
			+ "er3f9xagdAf/fbKWBjLQM8xR7JkRP4ri8YKOQPhK+VrddGUD59/wzVnvaGyl"
			+ "9MZE7TXFUeniQq5iXKnm22EQbYchv2Jcxyt2H9yptpzyh4tP6tEHl1C887p2"
			+ "J4qe7F2ATua9CzVGwXQSUbKtj2fgUZP5SsNp25guhPiZdtkf2sHMeiotmykF"
			+ "ErzqGMrvOAUThrO63GiYsRk4hF6rcQ01d+EUVpY/sBcCxgNyOiB7a84sDtrx"
			+ "nX5BTEZDTEj8LvuEyEV3TMUuAjx17Eyd+9JtKzwV4v3hlTaWOvGro9nPS7Ya"
			+ "PuG+RtufzXCUJPbPfTjTvtGOqvEzoztls8tuWA0OGHba9XfX9rfgorACAAA=");

		private static readonly string crOnlyMessage =
				"\r"
			+ " hello world!\r"
			+ "\r"
			+ "- dash\r";

		private static readonly string nlOnlyMessage =
			"\n"
			+ " hello world!\n"
			+ "\n"
			+ "- dash\n";

		private static readonly string crNlMessage =
			"\r\n"
			+ " hello world!\r\n"
			+ "\r\n"
			+ "- dash\r\n";

		private static readonly string crOnlySignedMessage =
				"-----BEGIN PGP SIGNED MESSAGE-----\r"
			+ "Hash: SHA256\r"
			+ "\r"
			+ "\r"
			+ " hello world!\r"
			+ "\r"
			+ "- - dash\r"
			+ "-----BEGIN PGP SIGNATURE-----\r"
			+ "Version: GnuPG v1.4.2.1 (GNU/Linux)\r"
			+ "\r"
			+ "iQEVAwUBRCNS8+DPyHq93/cWAQi6SwgAj3ItmSLr/sd/ixAQLW7/12jzEjfNmFDt\r"
			+ "WOZpJFmXj0fnMzTrOILVnbxHv2Ru+U8Y1K6nhzFSR7d28n31/XGgFtdohDEaFJpx\r"
			+ "Fl+KvASKIonnpEDjFJsPIvT1/G/eCPalwO9IuxaIthmKj0z44SO1VQtmNKxdLAfK\r"
			+ "+xTnXGawXS1WUE4CQGPM45mIGSqXcYrLtJkAg3jtRa8YRUn2d7b2BtmWH+jVaVuC\r"
			+ "hNrXYv7iHFOu25yRWhUQJisvdC13D/gKIPRvARXPgPhAC2kovIy6VS8tDoyG6Hm5\r"
			+ "dMgLEGhmqsgaetVq1ZIuBZj5S4j2apBJCDpF6GBfpBOfwIZs0Tpmlw==\r"
			+ "=84Nd\r"
			+ "-----END PGP SIGNATURE-----\r";

		private static readonly string nlOnlySignedMessage =
			"-----BEGIN PGP SIGNED MESSAGE-----\n"
			+ "Hash: SHA256\n"
			+ "\n"
			+ "\n"
			+ " hello world!\n"
			+ "\n"
			+ "- - dash\n"
			+ "-----BEGIN PGP SIGNATURE-----\n"
			+ "Version: GnuPG v1.4.2.1 (GNU/Linux)\n"
			+ "\n"
			+ "iQEVAwUBRCNS8+DPyHq93/cWAQi6SwgAj3ItmSLr/sd/ixAQLW7/12jzEjfNmFDt\n"
			+ "WOZpJFmXj0fnMzTrOILVnbxHv2Ru+U8Y1K6nhzFSR7d28n31/XGgFtdohDEaFJpx\n"
			+ "Fl+KvASKIonnpEDjFJsPIvT1/G/eCPalwO9IuxaIthmKj0z44SO1VQtmNKxdLAfK\n"
			+ "+xTnXGawXS1WUE4CQGPM45mIGSqXcYrLtJkAg3jtRa8YRUn2d7b2BtmWH+jVaVuC\n"
			+ "hNrXYv7iHFOu25yRWhUQJisvdC13D/gKIPRvARXPgPhAC2kovIy6VS8tDoyG6Hm5\n"
			+ "dMgLEGhmqsgaetVq1ZIuBZj5S4j2apBJCDpF6GBfpBOfwIZs0Tpmlw==\n"
			+ "=84Nd\n"
			+ "-----END PGP SIGNATURE-----\n";

		private static readonly string crNlSignedMessage =
				"-----BEGIN PGP SIGNED MESSAGE-----\r\n"
			+ "Hash: SHA256\r\n"
			+ "\r\n"
			+ "\r\n"
			+ " hello world!\r\n"
			+ "\r\n"
			+ "- - dash\r\n"
			+ "-----BEGIN PGP SIGNATURE-----\r\n"
			+ "Version: GnuPG v1.4.2.1 (GNU/Linux)\r\n"
			+ "\r\n"
			+ "iQEVAwUBRCNS8+DPyHq93/cWAQi6SwgAj3ItmSLr/sd/ixAQLW7/12jzEjfNmFDt\r\n"
			+ "WOZpJFmXj0fnMzTrOILVnbxHv2Ru+U8Y1K6nhzFSR7d28n31/XGgFtdohDEaFJpx\r\n"
			+ "Fl+KvASKIonnpEDjFJsPIvT1/G/eCPalwO9IuxaIthmKj0z44SO1VQtmNKxdLAfK\r\n"
			+ "+xTnXGawXS1WUE4CQGPM45mIGSqXcYrLtJkAg3jtRa8YRUn2d7b2BtmWH+jVaVuC\r\n"
			+ "hNrXYv7iHFOu25yRWhUQJisvdC13D/gKIPRvARXPgPhAC2kovIy6VS8tDoyG6Hm5\r\n"
			+ "dMgLEGhmqsgaetVq1ZIuBZj5S4j2apBJCDpF6GBfpBOfwIZs0Tpmlw==\r\n"
			+ "=84Nd\r"
			+ "-----END PGP SIGNATURE-----\r\n";

		private static readonly string crNlSignedMessageTrailingWhiteSpace =
			"-----BEGIN PGP SIGNED MESSAGE-----\r\n"
			+ "Hash: SHA256\r\n"
			+ "\r\n"
			+ "\r\n"
			+ " hello world! \t\r\n"
			+ "\r\n"
			+ "- - dash\r\n"
			+ "-----BEGIN PGP SIGNATURE-----\r\n"
			+ "Version: GnuPG v1.4.2.1 (GNU/Linux)\r\n"
			+ "\r\n"
			+ "iQEVAwUBRCNS8+DPyHq93/cWAQi6SwgAj3ItmSLr/sd/ixAQLW7/12jzEjfNmFDt\r\n"
			+ "WOZpJFmXj0fnMzTrOILVnbxHv2Ru+U8Y1K6nhzFSR7d28n31/XGgFtdohDEaFJpx\r\n"
			+ "Fl+KvASKIonnpEDjFJsPIvT1/G/eCPalwO9IuxaIthmKj0z44SO1VQtmNKxdLAfK\r\n"
			+ "+xTnXGawXS1WUE4CQGPM45mIGSqXcYrLtJkAg3jtRa8YRUn2d7b2BtmWH+jVaVuC\r\n"
			+ "hNrXYv7iHFOu25yRWhUQJisvdC13D/gKIPRvARXPgPhAC2kovIy6VS8tDoyG6Hm5\r\n"
			+ "dMgLEGhmqsgaetVq1ZIuBZj5S4j2apBJCDpF6GBfpBOfwIZs0Tpmlw==\r\n"
			+ "=84Nd\r"
			+ "-----END PGP SIGNATURE-----\r\n";

		public override string Name
		{
			get { return "PGPClearSignedSignature"; }
		}

		private void messageTest(
			string message,
			string type)
		{
			ArmoredInputStream aIn = new ArmoredInputStream(
				new MemoryStream(Encoding.ASCII.GetBytes(message)));

			string[] headers = aIn.GetArmorHeaders();

			if (headers == null || headers.Length != 1)
			{
				Fail("wrong number of headers found");
			}

			if (!"Hash: SHA256".Equals(headers[0]))
			{
				Fail("header value wrong: " + headers[0]);
			}

			//
			// read the input, making sure we ingore the last newline.
			//
			MemoryStream bOut = new MemoryStream();
			int ch;

			while ((ch = aIn.ReadByte()) >= 0 && aIn.IsClearText())
			{
				bOut.WriteByte((byte)ch);
			}

			PgpPublicKeyRingBundle pgpRings = new PgpPublicKeyRingBundle(publicKey);

			PgpObjectFactory	pgpFact = new PgpObjectFactory(aIn);
			PgpSignatureList	p3 = (PgpSignatureList)pgpFact.NextPgpObject();
			PgpSignature		sig = p3[0];

			sig.InitVerify(pgpRings.GetPublicKey(sig.KeyId));

			MemoryStream lineOut = new MemoryStream();
			Stream sigIn = new MemoryStream(bOut.ToArray(), false);
			int lookAhead = ReadInputLine(lineOut, sigIn);

			ProcessLine(sig, lineOut.ToArray());

			if (lookAhead != -1)
			{
				do
				{
					lookAhead = ReadInputLine(lineOut, lookAhead, sigIn);

					sig.Update((byte) '\r');
					sig.Update((byte) '\n');

					ProcessLine(sig, lineOut.ToArray());
				}
				while (lookAhead != -1);
			}

			if (!sig.Verify())
			{
				Fail("signature failed to verify m_in " + type);
			}
		}

		private PgpSecretKey ReadSecretKey(
			Stream    inputStream)
		{
			PgpSecretKeyRingBundle        pgpSec = new PgpSecretKeyRingBundle(inputStream);

			//
			// we just loop through the collection till we find a key suitable for encryption, in the real
			// world you would probably want to be a bit smarter about this.
			//
			//
			// iterate through the key rings.
			//
			foreach (PgpSecretKeyRing kRing in pgpSec.GetKeyRings())
			{
				foreach (PgpSecretKey k in kRing.GetSecretKeys())
				{
					if (k.IsSigningKey)
					{
						return k;
					}
				}
			}

			throw new ArgumentException("Can't find signing key in key ring.");
		}

		private void generateTest(
			string message,
			string type)
		{
			PgpSecretKey                    pgpSecKey = ReadSecretKey(new MemoryStream(secretKey));
			PgpPrivateKey                   pgpPrivKey = pgpSecKey.ExtractPrivateKey("".ToCharArray());
			PgpSignatureGenerator           sGen = new PgpSignatureGenerator(pgpSecKey.PublicKey.Algorithm, HashAlgorithmTag.Sha256);
			PgpSignatureSubpacketGenerator  spGen = new PgpSignatureSubpacketGenerator();

			sGen.InitSign(PgpSignature.CanonicalTextDocument, pgpPrivKey);

			IEnumerator    it = pgpSecKey.PublicKey.GetUserIds().GetEnumerator();
			if (it.MoveNext())
			{
				spGen.SetSignerUserId(false, (string)it.Current);
				sGen.SetHashedSubpackets(spGen.Generate());
			}

			MemoryStream bOut = new MemoryStream();
			ArmoredOutputStream aOut = new ArmoredOutputStream(bOut);
			MemoryStream bIn = new MemoryStream(Encoding.ASCII.GetBytes(message), false);

			aOut.BeginClearText(HashAlgorithmTag.Sha256);

			//
			// note the last \n m_in the file is ignored
			//
			MemoryStream lineOut = new MemoryStream();
			int lookAhead = ReadInputLine(lineOut, bIn);

			ProcessLine(aOut, sGen, lineOut.ToArray());

			if (lookAhead != -1)
			{
				do
				{
					lookAhead = ReadInputLine(lineOut, lookAhead, bIn);

					sGen.Update((byte) '\r');
					sGen.Update((byte) '\n');

					ProcessLine(aOut, sGen, lineOut.ToArray());
				}
				while (lookAhead != -1);
			}

			aOut.EndClearText();

			BcpgOutputStream bcpgOut = new BcpgOutputStream(aOut);

			sGen.Generate().Encode(bcpgOut);

			aOut.Close();

			byte[] bs = bOut.ToArray();
			messageTest(Encoding.ASCII.GetString(bs, 0, bs.Length), type);
		}

		private static int ReadInputLine(
			MemoryStream	bOut,
			Stream			fIn)
		{
			bOut.SetLength(0);

			int lookAhead = -1;
			int ch;

			while ((ch = fIn.ReadByte()) >= 0)
			{
				bOut.WriteByte((byte) ch);
				if (ch == '\r' || ch == '\n')
				{
					lookAhead = ReadPassedEol(bOut, ch, fIn);
					break;
				}
			}

			return lookAhead;
		}

		private static int ReadInputLine(
			MemoryStream	bOut,
			int				lookAhead,
			Stream			fIn)
		{
			bOut.SetLength(0);

			int ch = lookAhead;

			do
			{
				bOut.WriteByte((byte) ch);
				if (ch == '\r' || ch == '\n')
				{
					lookAhead = ReadPassedEol(bOut, ch, fIn);
					break;
				}
			}
			while ((ch = fIn.ReadByte()) >= 0);

			return lookAhead;
		}

		private static int ReadPassedEol(
			MemoryStream	bOut,
			int				lastCh,
			Stream			fIn)
		{
			int lookAhead = fIn.ReadByte();

			if (lastCh == '\r' && lookAhead == '\n')
			{
				bOut.WriteByte((byte) lookAhead);
				lookAhead = fIn.ReadByte();
			}

			return lookAhead;
		}

		private static void ProcessLine(
			PgpSignature	sig,
			byte[]			line)
		{
			int length = GetLengthWithoutWhiteSpace(line);
			if (length > 0)
			{
				sig.Update(line, 0, length);
			}
		}

		private static void ProcessLine(
			Stream					aOut,
			PgpSignatureGenerator	sGen,
			byte[]					line)
		{
			int length = GetLengthWithoutWhiteSpace(line);
			if (length > 0)
			{
				sGen.Update(line, 0, length);
			}

			aOut.Write(line, 0, line.Length);
		}

		private static int GetLengthWithoutWhiteSpace(
			byte[] line)
		{
			int end = line.Length - 1;

			while (end >= 0 && IsWhiteSpace(line[end]))
			{
				end--;
			}

			return end + 1;
		}

		private static bool IsWhiteSpace(
			byte b)
		{
			return b == '\r' || b == '\n' || b == '\t' || b == ' ';
		}

		public override void PerformTest()
		{
			messageTest(crOnlySignedMessage, "\\r");
			messageTest(nlOnlySignedMessage, "\\n");
			messageTest(crNlSignedMessage, "\\r\\n");
			messageTest(crNlSignedMessageTrailingWhiteSpace, "\\r\\n");

			generateTest(nlOnlyMessage, "\\r");
			generateTest(crOnlyMessage, "\\n");
			generateTest(crNlMessage, "\\r\\n");
		}

		public static void Main(
			string[] args)
		{
			RunTest(new PgpClearSignedSignatureTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
