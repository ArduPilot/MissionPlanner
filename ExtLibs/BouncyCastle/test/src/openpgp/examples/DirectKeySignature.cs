using System;
using System.IO;

using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.Sig;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Examples
{
	/**
	* A simple utility class that directly signs a public key and writes the signed key to "SignedKey.asc" in 
	* the current working directory.
	* <p>
	* To sign a key: DirectKeySignature secretKeyFile secretKeyPass publicKeyFile(key to be signed) NotationName NotationValue.<br/>
	* </p><p>
	* To display a NotationData packet from a publicKey previously signed: DirectKeySignature signedPublicKeyFile.<br/>
	* </p><p>
	* <b>Note</b>: this example will silently overwrite files, nor does it pay any attention to
	* the specification of "_CONSOLE" in the filename. It also expects that a single pass phrase
	* will have been used.
	* </p>
	*/
	public class DirectKeySignature
	{
		public static void Main(
			string[] args)
		{
			if (args.Length == 1)
			{
				Stream fis = File.OpenRead(args[0]);

				PgpPublicKeyRing ring = new PgpPublicKeyRing(
					PgpUtilities.GetDecoderStream(fis));
				PgpPublicKey key = ring.GetPublicKey();

				// iterate through all direct key signautures and look for NotationData subpackets
				foreach (PgpSignature sig in key.GetSignaturesOfType(PgpSignature.DirectKey))
				{
					Console.WriteLine("Signature date is: "
						+ sig.GetHashedSubPackets().GetSignatureCreationTime());

					NotationData[] data = sig.GetHashedSubPackets().GetNotationDataOccurences();

					for (int i = 0; i < data.Length; i++)
					{
						Console.WriteLine("Found Notaion named '" + data[i].GetNotationName()
							+"' with content '" + data[i].GetNotationValue() + "'.");
					}
				}

				fis.Close();
			}
			else if (args.Length == 5)
			{
				Stream secFis = File.OpenRead(args[0]);
				Stream pubFis = File.OpenRead(args[2]);

				// gather command line arguments
				PgpSecretKeyRing secRing = new PgpSecretKeyRing(
					PgpUtilities.GetDecoderStream(secFis));
				String secretKeyPass = args[1];
				PgpPublicKeyRing ring = new PgpPublicKeyRing(
					PgpUtilities.GetDecoderStream(pubFis));
				String notationName = args[3];
				String notationValue = args[4];

				// create the signed keyRing
				PgpPublicKeyRing sRing = null;
				sRing = new PgpPublicKeyRing(
					new MemoryStream(
						SignPublicKey(secRing.GetSecretKey(), secretKeyPass,
							ring.GetPublicKey(), notationName, notationValue, true),
						false));
				ring = sRing;

				secFis.Close();
				pubFis.Close();

				Stream fos = File.Create("SignedKey.asc");

				// write the created keyRing to file
				ArmoredOutputStream aOut = new ArmoredOutputStream(fos);
				sRing.Encode(aOut);
				aOut.Close();

				// Note: ArmoredOutputStream.Close() leaves underlying stream open
				fos.Close();
			}
			else
			{
				Console.Error.WriteLine("usage: DirectKeySignature secretKeyFile secretKeyPass publicKeyFile(key to be signed) NotationName NotationValue");
				Console.Error.WriteLine("or: DirectKeySignature signedPublicKeyFile");
			}
		}

		private static byte[] SignPublicKey(
			PgpSecretKey	secretKey,
			string			secretKeyPass,
			PgpPublicKey	keyToBeSigned,
			string			notationName,
			string			notationValue,
			bool			armor)
		{
			Stream os = new MemoryStream();
			if (armor)
			{
				os = new ArmoredOutputStream(os);
			}

			PgpPrivateKey pgpPrivKey = secretKey.ExtractPrivateKey(
				secretKeyPass.ToCharArray());

			PgpSignatureGenerator sGen = new PgpSignatureGenerator(
				secretKey.PublicKey.Algorithm, HashAlgorithmTag.Sha1);

			sGen.InitSign(PgpSignature.DirectKey, pgpPrivKey);

			BcpgOutputStream bOut = new BcpgOutputStream(os);

			sGen.GenerateOnePassVersion(false).Encode(bOut);

			PgpSignatureSubpacketGenerator spGen = new PgpSignatureSubpacketGenerator();

			bool isHumanReadable = true;

			spGen.SetNotationData(true, isHumanReadable, notationName, notationValue);

			PgpSignatureSubpacketVector packetVector = spGen.Generate();
			sGen.SetHashedSubpackets(packetVector);

			bOut.Flush();

			if (armor)
			{
				os.Close();
			}

			return PgpPublicKey.AddCertification(keyToBeSigned, sGen.Generate()).GetEncoded();
		}
	}
}
