using System;
using System.IO;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Examples
{
    /**
    * A simple utility class that Generates a RSA PgpPublicKey/PgpSecretKey pair.
    * <p>
    * usage: RsaKeyRingGenerator [-a] identity passPhrase</p>
    * <p>
    * Where identity is the name to be associated with the public key. The keys are placed
    * in the files pub.[asc|bpg] and secret.[asc|bpg].</p>
    */
    public sealed class RsaKeyRingGenerator
    {
        private RsaKeyRingGenerator()
        {
        }

		private static void ExportKeyPair(
            Stream                   secretOut,
            Stream                   publicOut,
            AsymmetricKeyParameter   publicKey,
            AsymmetricKeyParameter   privateKey,
            string                   identity,
            char[]                   passPhrase,
            bool                     armor)
        {
			if (armor)
			{
				secretOut = new ArmoredOutputStream(secretOut);
			}

			PgpSecretKey secretKey = new PgpSecretKey(
                PgpSignature.DefaultCertification,
                PublicKeyAlgorithmTag.RsaGeneral,
                publicKey,
                privateKey,
                DateTime.UtcNow,
                identity,
                SymmetricKeyAlgorithmTag.Cast5,
                passPhrase,
                null,
                null,
                new SecureRandom()
                );

            secretKey.Encode(secretOut);

			if (armor)
            {
				secretOut.Close();
				publicOut = new ArmoredOutputStream(publicOut);
            }

            PgpPublicKey key = secretKey.PublicKey;

            key.Encode(publicOut);

			if (armor)
			{
				publicOut.Close();
			}
        }

		public static int Main(
			string[] args)
        {
            IAsymmetricCipherKeyPairGenerator kpg = GeneratorUtilities.GetKeyPairGenerator("RSA");

            kpg.Init(new RsaKeyGenerationParameters(
				BigInteger.ValueOf(0x10001), new SecureRandom(), 1024, 25));

            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            if (args.Length < 2)
            {
                Console.WriteLine("RsaKeyPairGenerator [-a] identity passPhrase");
                return 0;
            }

			Stream out1, out2;
            if (args[0].Equals("-a"))
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("RsaKeyPairGenerator [-a] identity passPhrase");
					return 0;
				}

				out1 = File.Create("secret.asc");
                out2 = File.Create("pub.asc");

                ExportKeyPair(out1, out2, kp.Public, kp.Private, args[1], args[2].ToCharArray(), true);
            }
            else
            {
                out1 = File.Create("secret.bpg");
                out2 = File.Create("pub.bpg");

                ExportKeyPair(out1, out2, kp.Public, kp.Private, args[0], args[1].ToCharArray(), false);
            }
			out1.Close();
			out2.Close();
			return 0;
		}
    }
}
