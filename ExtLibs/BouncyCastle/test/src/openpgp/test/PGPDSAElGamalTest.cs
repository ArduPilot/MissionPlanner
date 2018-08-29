using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
	[TestFixture]
	public class PgpDsaElGamalTest
		: SimpleTest
	{
		private static readonly byte[] testPubKeyRing = Base64.Decode(
			  "mQGiBEAR8jYRBADNifuSopd20JOQ5x30ljIaY0M6927+vo09NeNxS3KqItba"
			+ "nz9o5e2aqdT0W1xgdHYZmdElOHTTsugZxdXTEhghyxoo3KhVcNnTABQyrrvX"
			+ "qouvmP2fEDEw0Vpyk+90BpyY9YlgeX/dEA8OfooRLCJde/iDTl7r9FT+mts8"
			+ "g3azjwCgx+pOLD9LPBF5E4FhUOdXISJ0f4EEAKXSOi9nZzajpdhe8W2ZL9gc"
			+ "BpzZi6AcrRZBHOEMqd69gtUxA4eD8xycUQ42yH89imEcwLz8XdJ98uHUxGJi"
			+ "qp6hq4oakmw8GQfiL7yQIFgaM0dOAI9Afe3m84cEYZsoAFYpB4/s9pVMpPRH"
			+ "NsVspU0qd3NHnSZ0QXs8L8DXGO1uBACjDUj+8GsfDCIP2QF3JC+nPUNa0Y5t"
			+ "wKPKl+T8hX/0FBD7fnNeC6c9j5Ir/Fp/QtdaDAOoBKiyNLh1JaB1NY6US5zc"
			+ "qFks2seZPjXEiE6OIDXYra494mjNKGUobA4hqT2peKWXt/uBcuL1mjKOy8Qf"
			+ "JxgEd0MOcGJO+1PFFZWGzLQ3RXJpYyBILiBFY2hpZG5hICh0ZXN0IGtleSBv"
			+ "bmx5KSA8ZXJpY0Bib3VuY3ljYXN0bGUub3JnPohZBBMRAgAZBQJAEfI2BAsH"
			+ "AwIDFQIDAxYCAQIeAQIXgAAKCRAOtk6iUOgnkDdnAKC/CfLWikSBdbngY6OK"
			+ "5UN3+o7q1ACcDRqjT3yjBU3WmRUNlxBg3tSuljmwAgAAuQENBEAR8jgQBAC2"
			+ "kr57iuOaV7Ga1xcU14MNbKcA0PVembRCjcVjei/3yVfT/fuCVtGHOmYLEBqH"
			+ "bn5aaJ0P/6vMbLCHKuN61NZlts+LEctfwoya43RtcubqMc7eKw4k0JnnoYgB"
			+ "ocLXOtloCb7jfubOsnfORvrUkK0+Ne6anRhFBYfaBmGU75cQgwADBQP/XxR2"
			+ "qGHiwn+0YiMioRDRiIAxp6UiC/JQIri2AKSqAi0zeAMdrRsBN7kyzYVVpWwN"
			+ "5u13gPdQ2HnJ7d4wLWAuizUdKIQxBG8VoCxkbipnwh2RR4xCXFDhJrJFQUm+"
			+ "4nKx9JvAmZTBIlI5Wsi5qxst/9p5MgP3flXsNi1tRbTmRhqIRgQYEQIABgUC"
			+ "QBHyOAAKCRAOtk6iUOgnkBStAJoCZBVM61B1LG2xip294MZecMtCwQCbBbsk"
			+ "JVCXP0/Szm05GB+WN+MOCT2wAgAA");

		private static readonly byte[] testPrivKeyRing = Base64.Decode(
			  "lQHhBEAR8jYRBADNifuSopd20JOQ5x30ljIaY0M6927+vo09NeNxS3KqItba"
			+ "nz9o5e2aqdT0W1xgdHYZmdElOHTTsugZxdXTEhghyxoo3KhVcNnTABQyrrvX"
			+ "qouvmP2fEDEw0Vpyk+90BpyY9YlgeX/dEA8OfooRLCJde/iDTl7r9FT+mts8"
			+ "g3azjwCgx+pOLD9LPBF5E4FhUOdXISJ0f4EEAKXSOi9nZzajpdhe8W2ZL9gc"
			+ "BpzZi6AcrRZBHOEMqd69gtUxA4eD8xycUQ42yH89imEcwLz8XdJ98uHUxGJi"
			+ "qp6hq4oakmw8GQfiL7yQIFgaM0dOAI9Afe3m84cEYZsoAFYpB4/s9pVMpPRH"
			+ "NsVspU0qd3NHnSZ0QXs8L8DXGO1uBACjDUj+8GsfDCIP2QF3JC+nPUNa0Y5t"
			+ "wKPKl+T8hX/0FBD7fnNeC6c9j5Ir/Fp/QtdaDAOoBKiyNLh1JaB1NY6US5zc"
			+ "qFks2seZPjXEiE6OIDXYra494mjNKGUobA4hqT2peKWXt/uBcuL1mjKOy8Qf"
			+ "JxgEd0MOcGJO+1PFFZWGzP4DAwLeUcsVxIC2s2Bb9ab2XD860TQ2BI2rMD/r"
			+ "7/psx9WQ+Vz/aFAT3rXkEJ97nFeqEACgKmUCAEk9939EwLQ3RXJpYyBILiBF"
			+ "Y2hpZG5hICh0ZXN0IGtleSBvbmx5KSA8ZXJpY0Bib3VuY3ljYXN0bGUub3Jn"
			+ "PohZBBMRAgAZBQJAEfI2BAsHAwIDFQIDAxYCAQIeAQIXgAAKCRAOtk6iUOgn"
			+ "kDdnAJ9Ala3OcwEV1DbK906CheYWo4zIQwCfUqUOLMp/zj6QAk02bbJAhV1r"
			+ "sAewAgAAnQFYBEAR8jgQBAC2kr57iuOaV7Ga1xcU14MNbKcA0PVembRCjcVj"
			+ "ei/3yVfT/fuCVtGHOmYLEBqHbn5aaJ0P/6vMbLCHKuN61NZlts+LEctfwoya"
			+ "43RtcubqMc7eKw4k0JnnoYgBocLXOtloCb7jfubOsnfORvrUkK0+Ne6anRhF"
			+ "BYfaBmGU75cQgwADBQP/XxR2qGHiwn+0YiMioRDRiIAxp6UiC/JQIri2AKSq"
			+ "Ai0zeAMdrRsBN7kyzYVVpWwN5u13gPdQ2HnJ7d4wLWAuizUdKIQxBG8VoCxk"
			+ "bipnwh2RR4xCXFDhJrJFQUm+4nKx9JvAmZTBIlI5Wsi5qxst/9p5MgP3flXs"
			+ "Ni1tRbTmRhr+AwMC3lHLFcSAtrNg/EiWFLAnKNXH27zjwuhje8u2r+9iMTYs"
			+ "GjbRxaxRY0GKRhttCwqe2BC0lHhzifdlEcc9yjIjuKfepG2fnnSIRgQYEQIA"
			+ "BgUCQBHyOAAKCRAOtk6iUOgnkBStAJ9HFejVtVJ/A9LM/mDPe0ExhEXt/QCg"
			+ "m/KM7hJ/JrfnLQl7IaZsdg1F6vCwAgAA");

		private static readonly byte[] encMessage = Base64.Decode(
			  "hQEOAynbo4lhNjcHEAP/dgCkMtPB6mIgjFvNiotjaoh4sAXf4vFNkSeehQ2c"
			+ "r+IMt9CgIYodJI3FoJXxOuTcwesqTp5hRzgUBJS0adLDJwcNubFMy0M2tp5o"
			+ "KTWpXulIiqyO6f5jI/oEDHPzFoYgBmR4x72l/YpMy8UoYGtNxNvR7LVOfqJv"
			+ "uDY/71KMtPQEAIadOWpf1P5Td+61Zqn2VH2UV7H8eI6hGa6Lsy4sb9iZNE7f"
			+ "c+spGJlgkiOt8TrQoq3iOK9UN9nHZLiCSIEGCzsEn3uNuorD++Qs065ij+Oy"
			+ "36TKeuJ+38CfT7u47dEshHCPqWhBKEYrxZWHUJU/izw2Q1Yxd2XRxN+nafTL"
			+ "X1fQ0lABQUASa18s0BkkEERIdcKQXVLEswWcGqWNv1ZghC7xO2VDBX4HrPjp"
			+ "drjL63p2UHzJ7/4gPWGGtnqq1Xita/1mrImn7pzLThDWiT55vjw6Hw==");

		private static readonly byte[] signedAndEncMessage = Base64.Decode(
			  "hQEOAynbo4lhNjcHEAP+K20MVhzdX57hf/cU8TH0prP0VePr9mmeBedzqqMn"
			+ "fp2p8Zb68zmcMlI/WiL5XMNLYRmCgEcXyWbKdP/XV9m9LDBe1CMAGrkCeGBy"
			+ "je69IQQ5LS9vDPyEMF4iAAv/EqACjqHkizdY/a/FRx/t2ioXYdEC2jA6kS9C"
			+ "McpsNz16DE8EAIk3uKn4bGo/+15TXkyFYzW5Cf71SfRoHNmU2zAI93zhjN+T"
			+ "B7mGJwWXzsMkIO6FkMU5TCSrwZS3DBWCIaJ6SYoaawE/C/2j9D7bX1Jv8kum"
			+ "4cq+eZM7z6JYs6xend+WAwittpUxbEiyC2AJb3fBSXPAbLqWd6J6xbZZ7GDK"
			+ "r2Ca0pwBxwGhbMDyi2zpHLzw95H7Ah2wMcGU6kMLB+hzBSZ6mSTGFehqFQE3"
			+ "2BnAj7MtnbghiefogacJ891jj8Y2ggJeKDuRz8j2iICaTOy+Y2rXnnJwfYzm"
			+ "BMWcd2h1C5+UeBJ9CrrLniCCI8s5u8z36Rno3sfhBnXdRmWSxExXtocbg1Ht"
			+ "dyiThf6TK3W29Yy/T6x45Ws5zOasaJdsFKM=");

		private static readonly char[] pass = "hello world".ToCharArray();

		private static readonly SecureRandom random = new SecureRandom();

		public override void PerformTest()
		{
			PgpPublicKey pubKey = null;

			//
			// Read the public key
			//
			PgpObjectFactory pgpFact = new PgpObjectFactory(testPubKeyRing);
			PgpPublicKeyRing pgpPub = (PgpPublicKeyRing)pgpFact.NextPgpObject();

			pubKey = pgpPub.GetPublicKey();

			if (pubKey.BitStrength != 1024)
			{
				Fail("failed - key strength reported incorrectly.");
			}

			//
			// Read the private key
			//
			PgpSecretKeyRing	sKey = new PgpSecretKeyRing(testPrivKeyRing);
			PgpSecretKey		secretKey = sKey.GetSecretKey();
			PgpPrivateKey		pgpPrivKey = secretKey.ExtractPrivateKey(pass);

			//
			// signature generation
			//
			const string data = "hello world!";
			byte[] dataBytes = Encoding.ASCII.GetBytes(data);
			MemoryStream bOut = new MemoryStream();
			MemoryStream testIn = new MemoryStream(dataBytes, false);
			PgpSignatureGenerator sGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.Dsa,
				HashAlgorithmTag.Sha1);

			sGen.InitSign(PgpSignature.BinaryDocument, pgpPrivKey);

			PgpCompressedDataGenerator cGen = new PgpCompressedDataGenerator(
				CompressionAlgorithmTag.Zip);

			BcpgOutputStream bcOut = new BcpgOutputStream(
				cGen.Open(new UncloseableStream(bOut)));

			sGen.GenerateOnePassVersion(false).Encode(bcOut);

			PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();

			DateTime testDateTime = new DateTime(1973, 7, 27);
			Stream lOut = lGen.Open(
				new UncloseableStream(bcOut),
				PgpLiteralData.Binary,
				"_CONSOLE",
				dataBytes.Length,
				testDateTime);

			int ch;
			while ((ch = testIn.ReadByte()) >= 0)
			{
				lOut.WriteByte((byte) ch);
				sGen.Update((byte) ch);
			}

			lGen.Close();

			sGen.Generate().Encode(bcOut);

			cGen.Close();

			//
			// verify Generated signature
			//
			pgpFact = new PgpObjectFactory(bOut.ToArray());

			PgpCompressedData c1 = (PgpCompressedData)pgpFact.NextPgpObject();

			pgpFact = new PgpObjectFactory(c1.GetDataStream());

			PgpOnePassSignatureList p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();

			PgpOnePassSignature ops = p1[0];

			PgpLiteralData p2 = (PgpLiteralData)pgpFact.NextPgpObject();
			if (!p2.ModificationTime.Equals(testDateTime))
			{
				Fail("Modification time not preserved");
			}

			Stream    dIn = p2.GetInputStream();

			ops.InitVerify(pubKey);

			while ((ch = dIn.ReadByte()) >= 0)
			{
				ops.Update((byte)ch);
			}

			PgpSignatureList p3 = (PgpSignatureList)pgpFact.NextPgpObject();

			if (!ops.Verify(p3[0]))
			{
				Fail("Failed Generated signature check");
			}

			//
			// test encryption
			//

			//
			// find a key sutiable for encryption
			//
			long pgpKeyID = 0;
			AsymmetricKeyParameter pKey = null;

			foreach (PgpPublicKey pgpKey in pgpPub.GetPublicKeys())
			{
				if (pgpKey.Algorithm == PublicKeyAlgorithmTag.ElGamalEncrypt
					|| pgpKey.Algorithm == PublicKeyAlgorithmTag.ElGamalGeneral)
				{
					pKey = pgpKey.GetKey();
					pgpKeyID = pgpKey.KeyId;
					if (pgpKey.BitStrength != 1024)
					{
						Fail("failed - key strength reported incorrectly.");
					}

					//
					// verify the key
					//

				}
			}

			IBufferedCipher c = CipherUtilities.GetCipher("ElGamal/None/PKCS1Padding");

			c.Init(true, pKey);

			byte[] inBytes = Encoding.ASCII.GetBytes("hello world");
			byte[] outBytes = c.DoFinal(inBytes);

			pgpPrivKey = sKey.GetSecretKey(pgpKeyID).ExtractPrivateKey(pass);

			c.Init(false, pgpPrivKey.Key);

			outBytes = c.DoFinal(outBytes);

			if (!Arrays.AreEqual(inBytes, outBytes))
			{
				Fail("decryption failed.");
			}

			//
			// encrypted message
			//
			byte[] text = { (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o',
								(byte)' ', (byte)'w', (byte)'o', (byte)'r', (byte)'l', (byte)'d', (byte)'!', (byte)'\n' };

			PgpObjectFactory pgpF = new PgpObjectFactory(encMessage);

			PgpEncryptedDataList encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

			PgpPublicKeyEncryptedData encP = (PgpPublicKeyEncryptedData)encList[0];

			Stream clear = encP.GetDataStream(pgpPrivKey);

			pgpFact = new PgpObjectFactory(clear);

			c1 = (PgpCompressedData)pgpFact.NextPgpObject();

			pgpFact = new PgpObjectFactory(c1.GetDataStream());

			PgpLiteralData ld = (PgpLiteralData)pgpFact.NextPgpObject();

			if (!ld.FileName.Equals("test.txt"))
			{
				throw new Exception("wrong filename in packet");
			}

			Stream inLd = ld.GetDataStream();
			byte[] bytes = Streams.ReadAll(inLd);

			if (!Arrays.AreEqual(bytes, text))
			{
				Fail("wrong plain text in decrypted packet");
			}

			//
			// signed and encrypted message
			//
			pgpF = new PgpObjectFactory(signedAndEncMessage);

			encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

			encP = (PgpPublicKeyEncryptedData)encList[0];

			clear = encP.GetDataStream(pgpPrivKey);

			pgpFact = new PgpObjectFactory(clear);

			c1 = (PgpCompressedData)pgpFact.NextPgpObject();

			pgpFact = new PgpObjectFactory(c1.GetDataStream());

			p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();

			ops = p1[0];

			ld = (PgpLiteralData)pgpFact.NextPgpObject();

			bOut = new MemoryStream();

			if (!ld.FileName.Equals("test.txt"))
			{
				throw new Exception("wrong filename in packet");
			}

			inLd = ld.GetDataStream();

			//
			// note: we use the DSA public key here.
			//
			ops.InitVerify(pgpPub.GetPublicKey());

			while ((ch = inLd.ReadByte()) >= 0)
			{
				ops.Update((byte) ch);
				bOut.WriteByte((byte) ch);
			}

			p3 = (PgpSignatureList)pgpFact.NextPgpObject();

			if (!ops.Verify(p3[0]))
			{
				Fail("Failed signature check");
			}

			if (!Arrays.AreEqual(bOut.ToArray(), text))
			{
				Fail("wrong plain text in decrypted packet");
			}

			//
			// encrypt
			//
			MemoryStream cbOut = new MemoryStream();
			PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(
				SymmetricKeyAlgorithmTag.TripleDes, random);
			PgpPublicKey puK = sKey.GetSecretKey(pgpKeyID).PublicKey;

			cPk.AddMethod(puK);

			Stream cOut = cPk.Open(new UncloseableStream(cbOut), bOut.ToArray().Length);

			cOut.Write(text, 0, text.Length);

			cOut.Close();

			pgpF = new PgpObjectFactory(cbOut.ToArray());

			encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

			encP = (PgpPublicKeyEncryptedData)encList[0];

			pgpPrivKey = sKey.GetSecretKey(pgpKeyID).ExtractPrivateKey(pass);

			clear = encP.GetDataStream(pgpPrivKey);
			outBytes = Streams.ReadAll(clear);

			if (!Arrays.AreEqual(outBytes, text))
			{
				Fail("wrong plain text in Generated packet");
			}

			//
			// use of PgpKeyPair
			//
			BigInteger g = new BigInteger("153d5d6172adb43045b68ae8e1de1070b6137005686d29d3d73a7749199681ee5b212c9b96bfdcfa5b20cd5e3fd2044895d609cf9b410b7a0f12ca1cb9a428cc", 16);
			BigInteger p = new BigInteger("9494fec095f3b85ee286542b3836fc81a5dd0a0349b4c239dd38744d488cf8e31db8bcb7d33b41abb9e5a33cca9144b1cef332c94bf0573bf047a3aca98cdf3b", 16);

			ElGamalParameters elParams = new ElGamalParameters(p, g);

			IAsymmetricCipherKeyPairGenerator kpg = GeneratorUtilities.GetKeyPairGenerator("ELGAMAL");
			kpg.Init(new ElGamalKeyGenerationParameters(random, elParams));

			AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

			PgpKeyPair pgpKp = new PgpKeyPair(PublicKeyAlgorithmTag.ElGamalGeneral ,
				kp.Public, kp.Private, DateTime.UtcNow);

			PgpPublicKey k1 = pgpKp.PublicKey;
			PgpPrivateKey k2 = pgpKp.PrivateKey;





			// Test bug with ElGamal P size != 0 mod 8 (don't use these sizes at home!)
			for (int pSize = 257; pSize < 264; ++pSize)
			{
				// Generate some parameters of the given size
				ElGamalParametersGenerator epg = new ElGamalParametersGenerator();
				epg.Init(pSize, 2, random);

				elParams = epg.GenerateParameters();

				kpg = GeneratorUtilities.GetKeyPairGenerator("ELGAMAL");
				kpg.Init(new ElGamalKeyGenerationParameters(random, elParams));


				// Run a short encrypt/decrypt test with random key for the given parameters
				kp = kpg.GenerateKeyPair();

				PgpKeyPair elGamalKeyPair = new PgpKeyPair(
					PublicKeyAlgorithmTag.ElGamalGeneral, kp, DateTime.UtcNow);

				cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, random);

				puK = elGamalKeyPair.PublicKey;

				cPk.AddMethod(puK);

				cbOut = new MemoryStream();

				cOut = cPk.Open(new UncloseableStream(cbOut), text.Length);

				cOut.Write(text, 0, text.Length);

				cOut.Close();

				pgpF = new PgpObjectFactory(cbOut.ToArray());

				encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

				encP = (PgpPublicKeyEncryptedData)encList[0];

				pgpPrivKey = elGamalKeyPair.PrivateKey;

				// Note: This is where an exception would be expected if the P size causes problems
				clear = encP.GetDataStream(pgpPrivKey);
				byte[] decText = Streams.ReadAll(clear);

				if (!Arrays.AreEqual(text, decText))
				{
					Fail("decrypted message incorrect");
				}
			}


			// check sub key encoding

			foreach (PgpPublicKey pgpKey in pgpPub.GetPublicKeys())
			{
				if (!pgpKey.IsMasterKey)
				{
					byte[] kEnc = pgpKey.GetEncoded();

					PgpObjectFactory objF = new PgpObjectFactory(kEnc);

					// TODO Make PgpPublicKey a PgpObject or return a PgpPublicKeyRing
//					PgpPublicKey k = (PgpPublicKey)objF.NextPgpObject();
//
//					pKey = k.GetKey();
//					pgpKeyID = k.KeyId;
//					if (k.BitStrength != 1024)
//					{
//						Fail("failed - key strength reported incorrectly.");
//					}
//
//					if (objF.NextPgpObject() != null)
//					{
//						Fail("failed - stream not fully parsed.");
//					}
                }
            }
		}

		public override string Name
		{
			get { return "PGPDSAElGamalTest"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new PgpDsaElGamalTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
