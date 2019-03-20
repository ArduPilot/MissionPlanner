using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Examples
{
    /**
    * A simple utility class that creates clear signed files and verifies them.
    * <p>
    * To sign a file: ClearSignedFileProcessor -s fileName secretKey passPhrase.
	* </p>
    * <p>
    * To decrypt: ClearSignedFileProcessor -v fileName signatureFile publicKeyFile.
	* </p>
    */
    public sealed class ClearSignedFileProcessor
    {
        private ClearSignedFileProcessor()
        {
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

			if (ch < 0)
			{
				lookAhead = -1;
			}

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

		/*
		* verify a clear text signed file
		*/
        private static void VerifyFile(
            Stream	inputStream,
            Stream	keyIn,
			string	resultName)
        {
			ArmoredInputStream aIn = new ArmoredInputStream(inputStream);
			Stream outStr = File.Create(resultName);

			//
			// write out signed section using the local line separator.
			// note: trailing white space needs to be removed from the end of
			// each line RFC 4880 Section 7.1
			//
			MemoryStream lineOut = new MemoryStream();
			int lookAhead = ReadInputLine(lineOut, aIn);
			byte[] lineSep = LineSeparator;

			if (lookAhead != -1 && aIn.IsClearText())
			{
				byte[] line = lineOut.ToArray();
				outStr.Write(line, 0, GetLengthWithoutSeparatorOrTrailingWhitespace(line));
				outStr.Write(lineSep, 0, lineSep.Length);

				while (lookAhead != -1 && aIn.IsClearText())
				{
					lookAhead = ReadInputLine(lineOut, lookAhead, aIn);
                
					line = lineOut.ToArray();
					outStr.Write(line, 0, GetLengthWithoutSeparatorOrTrailingWhitespace(line));
					outStr.Write(lineSep, 0, lineSep.Length);
				}
			}
            else
            {
                // a single line file
                if (lookAhead != -1)
                {
                    byte[] line = lineOut.ToArray();
                    outStr.Write(line, 0, GetLengthWithoutSeparatorOrTrailingWhitespace(line));
                    outStr.Write(lineSep, 0, lineSep.Length);
                }
            }

			outStr.Close();

			PgpPublicKeyRingBundle pgpRings = new PgpPublicKeyRingBundle(keyIn);

			PgpObjectFactory pgpFact = new PgpObjectFactory(aIn);
			PgpSignatureList p3 = (PgpSignatureList) pgpFact.NextPgpObject();
			PgpSignature sig = p3[0];

			sig.InitVerify(pgpRings.GetPublicKey(sig.KeyId));

			//
			// read the input, making sure we ignore the last newline.
			//
			Stream sigIn = File.OpenRead(resultName);

			lookAhead = ReadInputLine(lineOut, sigIn);

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

			sigIn.Close();

			if (sig.Verify())
            {
                Console.WriteLine("signature verified.");
            }
            else
            {
                Console.WriteLine("signature verification failed.");
            }
        }

		private static byte[] LineSeparator
		{
			get { return Encoding.ASCII.GetBytes(SimpleTest.NewLine); }
		}

        /*
        * create a clear text signed file.
        */
        private static void SignFile(
            string	fileName,
            Stream	keyIn,
            Stream	outputStream,
            char[]	pass,
			string	digestName)
        {
			HashAlgorithmTag digest;

			if (digestName.Equals("SHA256"))
			{
				digest = HashAlgorithmTag.Sha256;
			}
			else if (digestName.Equals("SHA384"))
			{
				digest = HashAlgorithmTag.Sha384;
			}
			else if (digestName.Equals("SHA512"))
			{
				digest = HashAlgorithmTag.Sha512;
			}
			else if (digestName.Equals("MD5"))
			{
				digest = HashAlgorithmTag.MD5;
			}
			else if (digestName.Equals("RIPEMD160"))
			{
				digest = HashAlgorithmTag.RipeMD160;
			}
			else
			{
				digest = HashAlgorithmTag.Sha1;
			}

			PgpSecretKey                    pgpSecKey = PgpExampleUtilities.ReadSecretKey(keyIn);
            PgpPrivateKey                   pgpPrivKey = pgpSecKey.ExtractPrivateKey(pass);
            PgpSignatureGenerator           sGen = new PgpSignatureGenerator(pgpSecKey.PublicKey.Algorithm, digest);
            PgpSignatureSubpacketGenerator  spGen = new PgpSignatureSubpacketGenerator();

			sGen.InitSign(PgpSignature.CanonicalTextDocument, pgpPrivKey);

			IEnumerator enumerator = pgpSecKey.PublicKey.GetUserIds().GetEnumerator();
            if (enumerator.MoveNext())
            {
                spGen.SetSignerUserId(false, (string) enumerator.Current);
                sGen.SetHashedSubpackets(spGen.Generate());
            }

            Stream fIn = File.OpenRead(fileName);
			ArmoredOutputStream aOut = new ArmoredOutputStream(outputStream);

			aOut.BeginClearText(digest);

			//
			// note the last \n/\r/\r\n in the file is ignored
			//
			MemoryStream lineOut = new MemoryStream();
			int lookAhead = ReadInputLine(lineOut, fIn);

			ProcessLine(aOut, sGen, lineOut.ToArray());

			if (lookAhead != -1)
			{
				do
				{
					lookAhead = ReadInputLine(lineOut, lookAhead, fIn);

					sGen.Update((byte) '\r');
					sGen.Update((byte) '\n');

					ProcessLine(aOut, sGen, lineOut.ToArray());
				}
				while (lookAhead != -1);
			}

			fIn.Close();

			aOut.EndClearText();

			BcpgOutputStream bOut = new BcpgOutputStream(aOut);

            sGen.Generate().Encode(bOut);

            aOut.Close();
        }

		private static void ProcessLine(
			PgpSignature	sig,
			byte[]			line)
		{
			// note: trailing white space needs to be removed from the end of
			// each line for signature calculation RFC 4880 Section 7.1
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

		private static int GetLengthWithoutSeparatorOrTrailingWhitespace(byte[] line)
		{
			int end = line.Length - 1;

			while (end >= 0 && IsWhiteSpace(line[end]))
			{
				end--;
			}

			return end + 1;
		}

		private static bool IsLineEnding(
			byte b)
		{
			return b == '\r' || b == '\n';
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
			return IsLineEnding(b) || b == '\t' || b == ' ';
		}

		public static int Main(
            string[] args)
        {
            if (args[0].Equals("-s"))
            {
                Stream fis = File.OpenRead(args[2]);
                Stream fos = File.Create(args[1] + ".asc");

                Stream keyIn = PgpUtilities.GetDecoderStream(fis);

                string digestName = (args.Length == 4)
                    ?	"SHA1"
                    :	args[4];

                SignFile(args[1], keyIn, fos, args[3].ToCharArray(), digestName);

                fis.Close();
                fos.Close();
            }
            else if (args[0].Equals("-v"))
            {
				if (args[1].IndexOf(".asc") < 0)
				{
					Console.Error.WriteLine("file needs to end in \".asc\"");
					return 1;
				}

				Stream fin = File.OpenRead(args[1]);
				Stream fis = File.OpenRead(args[2]);

				Stream keyIn = PgpUtilities.GetDecoderStream(fis);

				VerifyFile(fin, keyIn, args[1].Substring(0, args[1].Length - 4));

				fin.Close();
				fis.Close();
            }
            else
            {
                Console.Error.WriteLine("usage: ClearSignedFileProcessor [-s file keyfile passPhrase]|[-v sigFile keyFile]");
            }
			return 0;
        }
    }
}
