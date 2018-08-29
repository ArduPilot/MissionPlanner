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
    * A simple utility class that Generates a public/secret keyring containing a DSA signing
    * key and an El Gamal key for encryption.
    * <p>
    * usage: DSAElGamalKeyRingGenerator [-a] identity passPhrase</p>
    * <p>
    * Where identity is the name to be associated with the public key. The keys are placed
    * in the files pub.[asc|bpg] and secret.[asc|bpg].</p>
    * <p>
    * <b>Note</b>: this example encrypts the secret key using AES_256, many PGP products still
    * do not support this, if you are having problems importing keys try changing the algorithm
    * id to PgpEncryptedData.Cast5. CAST5 is more widelysupported.</p>
    */
    public sealed class DsaElGamalKeyRingGenerator
    {
        private DsaElGamalKeyRingGenerator()
        {
        }

		private static void ExportKeyPair(
            Stream					secretOut,
            Stream					publicOut,
            AsymmetricCipherKeyPair	dsaKp,
            AsymmetricCipherKeyPair	elgKp,
            string					identity,
            char[]					passPhrase,
            bool					armor)
        {
            if (armor)
            {
                secretOut = new ArmoredOutputStream(secretOut);
            }

			PgpKeyPair dsaKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.Dsa, dsaKp, DateTime.UtcNow);
            PgpKeyPair elgKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.ElGamalEncrypt, elgKp, DateTime.UtcNow);

			PgpKeyRingGenerator keyRingGen = new PgpKeyRingGenerator(PgpSignature.PositiveCertification, dsaKeyPair,
				identity, SymmetricKeyAlgorithmTag.Aes256, passPhrase, true, null, null, new SecureRandom());

			keyRingGen.AddSubKey(elgKeyPair);

			keyRingGen.GenerateSecretKeyRing().Encode(secretOut);

			if (armor)
            {
				secretOut.Close();
				publicOut = new ArmoredOutputStream(publicOut);
            }

			keyRingGen.GeneratePublicKeyRing().Encode(publicOut);

			if (armor)
			{
				publicOut.Close();
			}
        }

		public static int Main(
            string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("DsaElGamalKeyRingGenerator [-a] identity passPhrase");
                return 0;
            }

			IAsymmetricCipherKeyPairGenerator dsaKpg = GeneratorUtilities.GetKeyPairGenerator("DSA");
            DsaParametersGenerator pGen = new DsaParametersGenerator();
            pGen.Init(1024, 80, new SecureRandom());
            DsaParameters dsaParams = pGen.GenerateParameters();
            DsaKeyGenerationParameters kgp = new DsaKeyGenerationParameters(new SecureRandom(), dsaParams);
            dsaKpg.Init(kgp);


			//
            // this takes a while as the key generator has to Generate some DSA parameters
            // before it Generates the key.
            //
            AsymmetricCipherKeyPair dsaKp = dsaKpg.GenerateKeyPair();


			IAsymmetricCipherKeyPairGenerator elgKpg = GeneratorUtilities.GetKeyPairGenerator("ELGAMAL");

			BigInteger g = new BigInteger("153d5d6172adb43045b68ae8e1de1070b6137005686d29d3d73a7749199681ee5b212c9b96bfdcfa5b20cd5e3fd2044895d609cf9b410b7a0f12ca1cb9a428cc", 16);
            BigInteger p = new BigInteger("9494fec095f3b85ee286542b3836fc81a5dd0a0349b4c239dd38744d488cf8e31db8bcb7d33b41abb9e5a33cca9144b1cef332c94bf0573bf047a3aca98cdf3b", 16);

			ElGamalParameters elParams = new ElGamalParameters(p, g);
            ElGamalKeyGenerationParameters elKgp = new ElGamalKeyGenerationParameters(new SecureRandom(), elParams);
            elgKpg.Init(elKgp);

			//
            // this is quicker because we are using preGenerated parameters.
            //
            AsymmetricCipherKeyPair elgKp = elgKpg.GenerateKeyPair();

			Stream out1, out2;
			if (args[0].Equals("-a"))
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("DSAElGamalKeyRingGenerator [-a] identity passPhrase");
                    return 0;
                }

				out1 = File.Create("secret.asc");
                out2 = File.Create("pub.asc");

				ExportKeyPair(out1, out2, dsaKp, elgKp, args[1], args[2].ToCharArray(), true);
            }
            else
            {
                out1 = File.Create("secret.bpg");
                out2 = File.Create("pub.bpg");

				ExportKeyPair(out1, out2, dsaKp, elgKp, args[0], args[1].ToCharArray(), false);
            }
			out1.Close();
			out2.Close();
			return 0;
        }
    }
}
