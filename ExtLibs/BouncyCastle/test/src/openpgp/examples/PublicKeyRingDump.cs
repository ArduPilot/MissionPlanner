using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.Utilities;

using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Examples
{
    /**
    * Basic class which just lists the contents of the public key file passed
    * as an argument. If the file contains more than one "key ring" they are
    * listed in the order found.
    */
    public sealed class PublicKeyRingDump
    {
        private PublicKeyRingDump()
        {
        }

        public static string GetAlgorithm(
            PublicKeyAlgorithmTag algId)
        {
            switch (algId)
            {
                case PublicKeyAlgorithmTag.RsaGeneral:
                    return "RsaGeneral";
                case PublicKeyAlgorithmTag.RsaEncrypt:
                    return "RsaEncrypt";
                case PublicKeyAlgorithmTag.RsaSign:
                    return "RsaSign";
                case PublicKeyAlgorithmTag.ElGamalEncrypt:
                    return "ElGamalEncrypt";
                case PublicKeyAlgorithmTag.Dsa:
                    return "DSA";
                case PublicKeyAlgorithmTag.ECDH:
                    return "ECDH";
                case PublicKeyAlgorithmTag.ECDsa:
                    return "ECDSA";
                case PublicKeyAlgorithmTag.ElGamalGeneral:
                    return "ElGamalGeneral";
                case PublicKeyAlgorithmTag.DiffieHellman:
                    return "DiffieHellman";
            }

            return "unknown";
        }

		public static void Main(
			string[] args)
        {
			Stream fs = File.OpenRead(args[0]);

			//
            // Read the public key rings
            //
            PgpPublicKeyRingBundle pubRings = new PgpPublicKeyRingBundle(
                PgpUtilities.GetDecoderStream(fs));

			fs.Close();

			foreach (PgpPublicKeyRing pgpPub in pubRings.GetKeyRings())
            {
                try
                {
					//PgpPublicKey pubKey =
					pgpPub.GetPublicKey();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    Console.Error.WriteLine(e.StackTrace);
                    continue;
                }

				bool first = true;

				foreach (PgpPublicKey pgpKey in pgpPub.GetPublicKeys())
                {
                    if (first)
                    {
                        Console.WriteLine("Key ID: " +  pgpKey.KeyId.ToString("X"));
                        first = false;
                    }
                    else
                    {
                        Console.WriteLine("Key ID: " + pgpKey.KeyId.ToString("X") + " (subkey)");
                    }

					Console.WriteLine("            Algorithm: " + GetAlgorithm(pgpKey.Algorithm));
                    Console.WriteLine("            Fingerprint: " + Hex.ToHexString(pgpKey.GetFingerprint()));
                }
            }
        }
    }
}
