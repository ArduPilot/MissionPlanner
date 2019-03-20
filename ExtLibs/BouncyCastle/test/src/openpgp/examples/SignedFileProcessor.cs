using System;
using System.Collections;
using System.IO;


using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Examples
{
    /**
    * A simple utility class that signs and verifies files.
    * <p>
    * To sign a file: SignedFileProcessor -s [-a] fileName secretKey passPhrase.<br/>
    * If -a is specified the output file will be "ascii-armored".</p>
    * <p>
    * To decrypt: SignedFileProcessor -v fileName publicKeyFile.</p>
    * <p>
    * <b>Note</b>: this example will silently overwrite files, nor does it pay any attention to
    * the specification of "_CONSOLE" in the filename. It also expects that a single pass phrase
    * will have been used.</p>
    * <p>
    * <b>Note</b>: the example also makes use of PGP compression. If you are having difficulty Getting it
    * to interoperate with other PGP programs try removing the use of compression first.</p>
    */
    public sealed class SignedFileProcessor
    {
        private SignedFileProcessor() {}

		/**
        * verify the passed in file as being correctly signed.
        */
        private static void VerifyFile(
            Stream	inputStream,
            Stream	keyIn)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            PgpObjectFactory			pgpFact = new PgpObjectFactory(inputStream);
            PgpCompressedData			c1 = (PgpCompressedData) pgpFact.NextPgpObject();
            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            PgpOnePassSignatureList		p1 = (PgpOnePassSignatureList) pgpFact.NextPgpObject();
            PgpOnePassSignature			ops = p1[0];

            PgpLiteralData				p2 = (PgpLiteralData) pgpFact.NextPgpObject();
            Stream						dIn = p2.GetInputStream();
            PgpPublicKeyRingBundle		pgpRing = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(keyIn));
            PgpPublicKey				key = pgpRing.GetPublicKey(ops.KeyId);
            Stream						fos = File.Create(p2.FileName);

			ops.InitVerify(key);

			int ch;
			while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte)ch);
                fos.WriteByte((byte) ch);
            }
            fos.Close();

            PgpSignatureList	p3 = (PgpSignatureList)pgpFact.NextPgpObject();
			PgpSignature		firstSig = p3[0];
            if (ops.Verify(firstSig))
            {
                Console.Out.WriteLine("signature verified.");
            }
            else
            {
                Console.Out.WriteLine("signature verification failed.");
            }
        }

        /**
        * Generate an encapsulated signed file.
        *
        * @param fileName
        * @param keyIn
        * @param outputStream
        * @param pass
        * @param armor
        */
        private static void SignFile(
            string	fileName,
            Stream	keyIn,
            Stream	outputStream,
            char[]	pass,
            bool	armor,
			bool	compress)
        {
            if (armor)
            {
                outputStream = new ArmoredOutputStream(outputStream);
            }

            PgpSecretKey pgpSec = PgpExampleUtilities.ReadSecretKey(keyIn);
            PgpPrivateKey pgpPrivKey = pgpSec.ExtractPrivateKey(pass);
            PgpSignatureGenerator sGen = new PgpSignatureGenerator(pgpSec.PublicKey.Algorithm, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.BinaryDocument, pgpPrivKey);
            foreach (string userId in pgpSec.PublicKey.GetUserIds())
            {
                PgpSignatureSubpacketGenerator spGen = new PgpSignatureSubpacketGenerator();
                spGen.SetSignerUserId(false, userId);
                sGen.SetHashedSubpackets(spGen.Generate());
                // Just the first one!
                break;
            }

            Stream cOut = outputStream;
			PgpCompressedDataGenerator cGen = null;
			if (compress)
			{
				cGen = new PgpCompressedDataGenerator(CompressionAlgorithmTag.ZLib);

				cOut = cGen.Open(cOut);
			}

			BcpgOutputStream bOut = new BcpgOutputStream(cOut);

            sGen.GenerateOnePassVersion(false).Encode(bOut);

            FileInfo					file = new FileInfo(fileName);
            PgpLiteralDataGenerator     lGen = new PgpLiteralDataGenerator();
            Stream						lOut = lGen.Open(bOut, PgpLiteralData.Binary, file);
            FileStream					fIn = file.OpenRead();
            int                         ch = 0;

			while ((ch = fIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte) ch);
                sGen.Update((byte)ch);
            }

			fIn.Close();
			lGen.Close();

			sGen.Generate().Encode(bOut);

			if (cGen != null)
			{
				cGen.Close();
			}

			if (armor)
			{
				outputStream.Close();
			}
        }

		public static void Main(
            string[] args)
        {
			// TODO provide command-line option to determine whether to use compression in SignFile
            if (args[0].Equals("-s"))
            {
				Stream keyIn, fos;
                if (args[1].Equals("-a"))
                {
                    keyIn = File.OpenRead(args[3]);
                    fos = File.Create(args[2] + ".asc");

					SignFile(args[2], keyIn, fos, args[4].ToCharArray(), true, true);
                }
                else
                {
                    keyIn = File.OpenRead(args[2]);
                    fos = File.Create(args[1] + ".bpg");

					SignFile(args[1], keyIn, fos, args[3].ToCharArray(), false, true);
                }
				keyIn.Close();
				fos.Close();
            }
            else if (args[0].Equals("-v"))
            {
				using (Stream fis = File.OpenRead(args[1]),
                	keyIn = File.OpenRead(args[2]))
				{
					VerifyFile(fis, keyIn);
				}
			}
            else
            {
                Console.Error.WriteLine("usage: SignedFileProcessor -v|-s [-a] file keyfile [passPhrase]");
            }
        }
    }
}
