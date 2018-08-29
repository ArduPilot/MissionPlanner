using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class Gost3410Test
		: ITest
	{
		private static readonly byte[] hashmessage = Hex.Decode("3042453136414534424341374533364339313734453431443642453241453435");

		private static byte[] zeroTwo(int length)
		{
			byte[] data = new byte[length];
			data[data.Length - 1] = 0x02;
			return data;
		}

		private class Gost3410_TEST1_512
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-TEST1-512"; }
			}

			FixedSecureRandom init_random = FixedSecureRandom.From(Hex.Decode("00005EC900007341"), zeroTwo(64));
			FixedSecureRandom random = FixedSecureRandom.From(Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A"));
			FixedSecureRandom keyRandom = FixedSecureRandom.From(Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046"));

			BigInteger pValue = new BigInteger("EE8172AE8996608FB69359B89EB82A69854510E2977A4D63BC97322CE5DC3386EA0A12B343E9190F23177539845839786BB0C345D165976EF2195EC9B1C379E3", 16);
			BigInteger qValue = new BigInteger("98915E7EC8265EDFCDA31E88F24809DDB064BDC7285DD50D7289F0AC6F49DD2D", 16);

			public ITestResult Perform()
			{
				BigInteger r = new BigInteger("3e5f895e276d81d2d52c0763270a458157b784c57abdbd807bc44fd43a32ac06",16);
				BigInteger s = new BigInteger("3f0dd5d4400d47c08e4ce505ff7434b6dbf729592e37c74856dab85115a60955",16);
				Gost3410ParametersGenerator pGen = new Gost3410ParametersGenerator();

				pGen.Init(512, 1, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (parameters.ValidationParameters == null)
				{
					return new SimpleTestResult(false, Name + "validation parameters wrong");
				}

				if (parameters.ValidationParameters.C != 29505
					||  parameters.ValidationParameters.X0 != 24265)
				{
					return new SimpleTestResult(false, Name + "validation parameters values wrong");
				}

				if (!init_random.IsExhausted)
				{
					return new SimpleTestResult(false, Name
						+ ": unexpected number of bytes used from 'init_random'.");
				}

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator         Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters  genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair  pair = Gost3410KeyGen.GenerateKeyPair();

				if (!keyRandom.IsExhausted)
				{
					return new SimpleTestResult(false, Name
						+ ": unexpected number of bytes used from 'keyRandom'.");
				}

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer gost3410 = new Gost3410Signer();

				gost3410.Init(true, param);

				BigInteger[] sig = gost3410.GenerateSignature(hashmessage);

				if (!random.IsExhausted)
				{
					return new SimpleTestResult(false, Name
						+ ": unexpected number of bytes used from 'random'.");
				}

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				gost3410.Init(false, pair.Public);

				if (gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		private class Gost3410_TEST2_512
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-TEST2-512"; }
			}

			FixedSecureRandom init_random = FixedSecureRandom.From(Hex.Decode("000000003DFC46F1000000000000000D"), zeroTwo(64));
			FixedSecureRandom random = FixedSecureRandom.From(Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A"));
			FixedSecureRandom keyRandom = FixedSecureRandom.From(Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046"));

			BigInteger pValue = new BigInteger("8b08eb135af966aab39df294538580c7da26765d6d38d30cf1c06aae0d1228c3316a0e29198460fad2b19dc381c15c888c6dfd0fc2c565abb0bf1faff9518f85", 16);
			BigInteger qValue = new BigInteger("931a58fb6f0dcdf2fe7549bc3f19f4724b56898f7f921a076601edb18c93dc75", 16);

			public ITestResult Perform()
			{
				BigInteger r = new BigInteger("7c07c8cf035c2a1cb2b7fae5807ac7cd623dfca7a1a68f6d858317822f1ea00d",16);
				BigInteger s = new BigInteger("7e9e036a6ff87dbf9b004818252b1f6fc310bdd4d17cb8c37d9c36c7884de60c",16);
				Gost3410ParametersGenerator  pGen = new Gost3410ParametersGenerator();

				pGen.Init(512, 2, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (!init_random.IsExhausted)
				{
					return new SimpleTestResult(false, Name
						+ ": unexpected number of bytes used from 'init_random'.");
				}

				if (parameters.ValidationParameters == null)
				{
					return new SimpleTestResult(false, Name + ": validation parameters wrong");
				}

				if (parameters.ValidationParameters.CL != 13
					||  parameters.ValidationParameters.X0L != 1039943409)
				{
					return new SimpleTestResult(false, Name + ": validation parameters values wrong");
				}

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair pair = Gost3410KeyGen.GenerateKeyPair();

				if (!keyRandom.IsExhausted)
				{
					return new SimpleTestResult(false, Name
						+ ": unexpected number of bytes used from 'keyRandom'.");
				}

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!random.IsExhausted)
				{
					return new SimpleTestResult(false, Name
						+ ": unexpected number of bytes used from 'random'.");
				}

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (!Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}

				return new SimpleTestResult(true, Name + ": Okay");
			}
		}

		private class Gost3410_TEST1_1024
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-TEST1-1024"; }
			}

			private class SecureRandomImpl1 : SecureRandom
			{
				bool firstInt = true;

				public override int NextInt()
				{
					string x0 = "0xA565";
					string c =  "0x538B";

					if (firstInt)
					{
						firstInt = false;
						return NumberParsing.DecodeIntFromHex(x0);
					}
					return NumberParsing.DecodeIntFromHex(c);
				}

				public override void NextBytes(byte[] bytes)
				{

					byte[] d = Hex.Decode("02");

					Array.Copy(d, 0, bytes, bytes.Length-d.Length, d.Length);
				}
			};
			SecureRandom init_random = new SecureRandomImpl1();

			private class SecureRandomImpl2 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] k = Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A");

					int i;

					for (i = 0; i < (bytes.Length - k.Length); i += k.Length)
					{
						Array.Copy(k, 0, bytes, i, k.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(k, 0, bytes, i - k.Length, bytes.Length - (i - k.Length));
					}
					else
					{
						Array.Copy(k, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom random = new SecureRandomImpl2();

			private class SecureRandomImpl3 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] x = Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046");

					int i;

					for (i = 0; i < (bytes.Length - x.Length); i += x.Length)
					{
						Array.Copy(x, 0, bytes, i, x.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(x, 0, bytes, i - x.Length, bytes.Length - (i - x.Length));
					}
					else
					{
						Array.Copy(x, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom keyRandom = new SecureRandomImpl3();

			BigInteger  pValue = new BigInteger("ab8f37938356529e871514c1f48c5cbce77b2f4fc9a2673ac2c1653da8984090c0ac73775159a26bef59909d4c9846631270e16653a6234668f2a52a01a39b921490e694c0f104b58d2e14970fccb478f98d01e975a1028b9536d912de5236d2dd2fc396b77153594d4178780e5f16f718471e2111c8ce64a7d7e196fa57142d", 16);
			BigInteger  qValue = new BigInteger("bcc02ca0ce4f0753ec16105ee5d530aa00d39f3171842ab2c334a26b5f576e0f", 16);

			public ITestResult Perform()
			{
				BigInteger r = new BigInteger("a8790aabbd5a998ff524bad048ac69cd1faff2dab048265c8d60d1471c44a9ee",16);
				BigInteger s = new BigInteger("30df5ba32ac77170b9632559bef7d37620017756dff3fea1088b4267db0944b8",16);
				Gost3410ParametersGenerator pGen = new Gost3410ParametersGenerator();

				pGen.Init(1024, 1, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair pair = Gost3410KeyGen.GenerateKeyPair();

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		private class Gost3410_TEST2_1024
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-TEST2-1024"; }
			}

			private class SecureRandomImpl4 : SecureRandom
			{
				bool firstLong = true;

				public override long NextLong()
				{
					string x0 = "0x3DFC46F1";
					string c =  "0xD";

					if (firstLong)
					{
						firstLong = false;
						return NumberParsing.DecodeLongFromHex(x0);
					}
					return NumberParsing.DecodeLongFromHex(c);
				}

				public override void NextBytes(byte[] bytes)
				{

					byte[] d = Hex.Decode("02");

					Array.Copy(d, 0, bytes, bytes.Length-d.Length, d.Length);
				}
			};
			SecureRandom init_random = new SecureRandomImpl4();

			private class SecureRandomImpl5 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] k = Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A");

					int i;

					for (i = 0; i < (bytes.Length - k.Length); i += k.Length)
					{
						Array.Copy(k, 0, bytes, i, k.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(k, 0, bytes, i - k.Length, bytes.Length - (i - k.Length));
					}
					else
					{
						Array.Copy(k, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom random = new SecureRandomImpl5();

			private class SecureRandomImpl6 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] x = Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046");

					int i;

					for (i = 0; i < (bytes.Length - x.Length); i += x.Length)
					{
						Array.Copy(x, 0, bytes, i, x.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(x, 0, bytes, i - x.Length, bytes.Length - (i - x.Length));
					}
					else
					{
						Array.Copy(x, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom keyRandom = new SecureRandomImpl6();

			BigInteger  pValue = new BigInteger("e2c4191c4b5f222f9ac2732562f6d9b4f18e7fb67a290ea1e03d750f0b9806755fc730d975bf3faa606d05c218b35a6c3706919aab92e0c58b1de4531c8fa8e7af43c2bff016251e21b2870897f6a27ac4450bca235a5b748ad386e4a0e4dfcb09152435abcfe48bd0b126a8122c7382f285a9864615c66decddf6afd355dfb7", 16);
			BigInteger  qValue = new BigInteger("931a58fb6f0dcdf2fe7549bc3f19f4724b56898f7f921a076601edb18c93dc75", 16);

			public ITestResult Perform()
			{
				BigInteger              r = new BigInteger("81d69a192e9c7ac21fc07da41bd07e230ba6a94eb9f3c1fd104c7bd976733ca5",16);
				BigInteger              s = new BigInteger("315c879c8414f35feb4deb15e7cc0278c48e6ca1596325d6959338d860b0c47a",16);
				Gost3410ParametersGenerator  pGen = new Gost3410ParametersGenerator();

				pGen.Init(1024, 2, init_random);

				Gost3410Parameters           parameters = pGen.GenerateParameters();

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator         Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters  genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair  pair = Gost3410KeyGen.GenerateKeyPair();

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		private class Gost3410_AParam
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-AParam"; }
			}

			private class SecureRandomImpl7 : SecureRandom
			{
				bool firstLong = true;

				public override long NextLong()
				{
					string x0 = "0x520874F5";
					string c =  "0xEE39ADB3";

					if (firstLong)
					{
						firstLong = false;
						return NumberParsing.DecodeLongFromHex(x0);
					}
					return NumberParsing.DecodeLongFromHex(c);
				}

				public override void NextBytes(byte[] bytes)
				{

					byte[] d = Hex.Decode("02");

					Array.Copy(d, 0, bytes, bytes.Length-d.Length, d.Length);
				}
			};
			SecureRandom init_random = new SecureRandomImpl7();

			private class SecureRandomImpl8 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] k = Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A");

					int i;

					for (i = 0; i < (bytes.Length - k.Length); i += k.Length)
					{
						Array.Copy(k, 0, bytes, i, k.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(k, 0, bytes, i - k.Length, bytes.Length - (i - k.Length));
					}
					else
					{
						Array.Copy(k, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom random = new SecureRandomImpl8();

			private class SecureRandomImpl9 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] x = Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046");

					int i;

					for (i = 0; i < (bytes.Length - x.Length); i += x.Length)
					{
						Array.Copy(x, 0, bytes, i, x.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(x, 0, bytes, i - x.Length, bytes.Length - (i - x.Length));
					}
					else
					{
						Array.Copy(x, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom keyRandom = new SecureRandomImpl9();

			BigInteger  pValue = new BigInteger("b4e25efb018e3c8b87505e2a67553c5edc56c2914b7e4f89d23f03f03377e70a2903489dd60e78418d3d851edb5317c4871e40b04228c3b7902963c4b7d85d52b9aa88f2afdbeb28da8869d6df846a1d98924e925561bd69300b9ddd05d247b5922d967cbb02671881c57d10e5ef72d3e6dad4223dc82aa1f7d0294651a480df", 16);
			BigInteger  qValue = new BigInteger("972432a437178b30bd96195b773789ab2fff15594b176dd175b63256ee5af2cf", 16);

			public ITestResult Perform()
			{
				BigInteger r = new BigInteger("64a8856628e5669d85f62cd763dd4a99bc56d33dc0e1859122855d141e9e4774",16);
				BigInteger s = new BigInteger("319ebac97092b288d469a4b988248794f60c865bc97858d9a3135c6d1a1bf2dd",16);
				Gost3410ParametersGenerator pGen = new Gost3410ParametersGenerator();

				pGen.Init(1024, 2, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair pair = Gost3410KeyGen.GenerateKeyPair();

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		private class Gost3410_BParam
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-BParam"; }
			}

			private class SecureRandomImpl10 : SecureRandom
			{
				bool firstLong = true;

				public override long NextLong()
				{
					string x0 = "0x5B977CDB";
					string c =  "0x6E9692DD";

					if (firstLong)
					{
						firstLong = false;
						return NumberParsing.DecodeLongFromHex(x0);
					}
					return NumberParsing.DecodeLongFromHex(c);
				}

				public override void NextBytes(byte[] bytes)
				{
					byte[] d = Hex.Decode("bc3cbbdb7e6f848286e19ad9a27a8e297e5b71c53dd974cdf60f937356df69cbc97a300ccc71685c553046147f11568c4fddf363d9d886438345a62c3b75963d6546adfabf31b31290d12cae65ecb8309ef66782");

					Array.Copy(d, 0, bytes, bytes.Length-d.Length, d.Length);
				}
			};
			SecureRandom init_random = new SecureRandomImpl10();

			private class SecureRandomImpl11 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] k = Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A");

					int i;

					for (i = 0; i < (bytes.Length - k.Length); i += k.Length)
					{
						Array.Copy(k, 0, bytes, i, k.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(k, 0, bytes, i - k.Length, bytes.Length - (i - k.Length));
					}
					else
					{
						Array.Copy(k, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom random = new SecureRandomImpl11();

			private class SecureRandomImpl12 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] x = Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046");

					int i;

					for (i = 0; i < (bytes.Length - x.Length); i += x.Length)
					{
						Array.Copy(x, 0, bytes, i, x.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(x, 0, bytes, i - x.Length, bytes.Length - (i - x.Length));
					}
					else
					{
						Array.Copy(x, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom keyRandom = new SecureRandomImpl12();

			BigInteger  pValue = new BigInteger("c6971fc57524b30c9018c5e621de15499736854f56a6f8aee65a7a404632b3540f09020f67f04dc2e6783b141dceffd21a703035b7d0187c6e12cb4229922bafdb2225b73e6b23a0de36e20047065aea000c1a374283d0ad8dc1981e3995f0bb8c72526041fcb98ae6163e1e71a669d8364e9c4c3188f673c5f8ee6fadb41abf", 16);
			BigInteger  qValue = new BigInteger("b09d634c10899cd7d4c3a7657403e05810b07c61a688bab2c37f475e308b0607", 16);

			public ITestResult Perform()
			{
				BigInteger r = new BigInteger("860d82c60e9502cd00c0e9e1f6563feafec304801974d745c5e02079946f729e",16);
				BigInteger s = new BigInteger("7ef49264ef022801aaa03033cd97915235fbab4c823ed936b0f360c22114688a",16);
				Gost3410ParametersGenerator pGen = new Gost3410ParametersGenerator();

				pGen.Init(1024, 2, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair pair = Gost3410KeyGen.GenerateKeyPair();

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		private class Gost3410_CParam
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-CParam"; }
			}

			private class SecureRandomImpl13 : SecureRandom
			{
				bool firstLong = true;

				public override long NextLong()
				{
					string x0 = "0x43848744";
					string c =  "0xB50A826D";

					if (firstLong)
					{
						firstLong = false;
						return NumberParsing.DecodeLongFromHex(x0);
					}
					return NumberParsing.DecodeLongFromHex(c);
				}

				public override void NextBytes(byte[] bytes)
				{
					byte[] d = Hex.Decode("7F575E8194BC5BDF");

					Array.Copy(d, 0, bytes, bytes.Length-d.Length, d.Length);
				}
			};
			SecureRandom init_random = new SecureRandomImpl13();

			private class SecureRandomImpl14 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] k = Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A");

					int i;

					for (i = 0; i < (bytes.Length - k.Length); i += k.Length)
					{
						Array.Copy(k, 0, bytes, i, k.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(k, 0, bytes, i - k.Length, bytes.Length - (i - k.Length));
					}
					else
					{
						Array.Copy(k, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom random = new SecureRandomImpl14();

			private class SecureRandomImpl15 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] x = Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046");

					int i;

					for (i = 0; i < (bytes.Length - x.Length); i += x.Length)
					{
						Array.Copy(x, 0, bytes, i, x.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(x, 0, bytes, i - x.Length, bytes.Length - (i - x.Length));
					}
					else
					{
						Array.Copy(x, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom keyRandom = new SecureRandomImpl15();

			BigInteger  pValue = new BigInteger("9d88e6d7fe3313bd2e745c7cdd2ab9ee4af3c8899e847de74a33783ea68bc30588ba1f738c6aaf8ab350531f1854c3837cc3c860ffd7e2e106c3f63b3d8a4c034ce73942a6c3d585b599cf695ed7a3c4a93b2b947b7157bb1a1c043ab41ec8566c6145e938a611906de0d32e562494569d7e999a0dda5c879bdd91fe124df1e9", 16);
			BigInteger  qValue = new BigInteger("fadd197abd19a1b4653eecf7eca4d6a22b1f7f893b641f901641fbb555354faf", 16);

			public ITestResult Perform()
			{
				BigInteger r = new BigInteger("4deb95a0b35e7ed7edebe9bef5a0f93739e16b7ff27fe794d989d0c13159cfbc",16);
				BigInteger s = new BigInteger("e1d0d30345c24cfeb33efde3deee5fbbda78ddc822b719d860cd0ba1fb6bd43b",16);
				Gost3410ParametersGenerator pGen = new Gost3410ParametersGenerator();

				pGen.Init(1024, 2, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair pair = Gost3410KeyGen.GenerateKeyPair();

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		private class Gost3410_DParam
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-DParam"; }
			}

			private class SecureRandomImpl16 : SecureRandom
			{
				bool firstLong = true;

				public override long NextLong()
				{
					string x0 = "0x13DA8B9D";
					string c =  "0xA0E9DE4B";

					if (firstLong)
					{
						firstLong = false;
						return NumberParsing.DecodeLongFromHex(x0);
					}
					return NumberParsing.DecodeLongFromHex(c);
				}

				public override void NextBytes(byte[] bytes)
				{

					byte[] d = Hex.Decode("41ab97857f42614355d32db0b1069f109a4da283676c7c53a68185b4");

					Array.Copy(d, 0, bytes, bytes.Length-d.Length, d.Length);
				}
			};
			SecureRandom init_random = new SecureRandomImpl16();

			private class SecureRandomImpl17 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] k = Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A");

					int i;

					for (i = 0; i < (bytes.Length - k.Length); i += k.Length)
					{
						Array.Copy(k, 0, bytes, i, k.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(k, 0, bytes, i - k.Length, bytes.Length - (i - k.Length));
					}
					else
					{
						Array.Copy(k, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom random = new SecureRandomImpl17();

			private class SecureRandomImpl18 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] x = Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046");

					int i;

					for (i = 0; i < (bytes.Length - x.Length); i += x.Length)
					{
						Array.Copy(x, 0, bytes, i, x.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(x, 0, bytes, i - x.Length, bytes.Length - (i - x.Length));
					}
					else
					{
						Array.Copy(x, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom keyRandom = new SecureRandomImpl18();

			BigInteger  pValue = new BigInteger("80f102d32b0fd167d069c27a307adad2c466091904dbaa55d5b8cc7026f2f7a1919b890cb652c40e054e1e9306735b43d7b279eddf9102001cd9e1a831fe8a163eed89ab07cf2abe8242ac9dedddbf98d62cddd1ea4f5f15d3a42a6677bdd293b24260c0f27c0f1d15948614d567b66fa902baa11a69ae3bceadbb83e399c9b5", 16);
			BigInteger  qValue = new BigInteger("f0f544c418aac234f683f033511b65c21651a6078bda2d69bb9f732867502149", 16);

			public ITestResult Perform()
			{
				BigInteger              r = new BigInteger("712592d285b792e33b8a9a11e8e6c4f512ddf0042972bbfd1abb0a93e8fc6f54",16);
				BigInteger              s = new BigInteger("2cf26758321258b130d5612111339f09ceb8668241f3482e38baa56529963f07",16);
				Gost3410ParametersGenerator  pGen = new Gost3410ParametersGenerator();

				pGen.Init(1024, 2, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair pair = Gost3410KeyGen.GenerateKeyPair();

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		private class Gost3410_AExParam
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-AExParam"; }
			}

			private class SecureRandomImpl19 : SecureRandom
			{
				bool firstLong = true;

				public override long NextLong()
				{
					string x0 = "0xD05E9F14";
					string c =  "0x46304C5F";

					if (firstLong)
					{
						firstLong = false;
						return NumberParsing.DecodeLongFromHex(x0);
					}
					return NumberParsing.DecodeLongFromHex(c);
				}

				public override void NextBytes(byte[] bytes)
				{
					byte[] d = Hex.Decode("35ab875399cda33c146ca629660e5a5e5c07714ca326db032dd6751995cdb90a612b9228932d8302704ec24a5def7739c5813d83");

					Array.Copy(d, 0, bytes, bytes.Length-d.Length, d.Length);
				}
			};
			SecureRandom init_random = new SecureRandomImpl19();

			private class SecureRandomImpl20 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] k = Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A");

					int i;

					for (i = 0; i < (bytes.Length - k.Length); i += k.Length)
					{
						Array.Copy(k, 0, bytes, i, k.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(k, 0, bytes, i - k.Length, bytes.Length - (i - k.Length));
					}
					else
					{
						Array.Copy(k, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom random = new SecureRandomImpl20();

			private class SecureRandomImpl21 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] x = Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046");

					int i;

					for (i = 0; i < (bytes.Length - x.Length); i += x.Length)
					{
						Array.Copy(x, 0, bytes, i, x.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(x, 0, bytes, i - x.Length, bytes.Length - (i - x.Length));
					}
					else
					{
						Array.Copy(x, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom keyRandom = new SecureRandomImpl21();

			BigInteger  pValue = new BigInteger("ca3b3f2eee9fd46317d49595a9e7518e6c63d8f4eb4d22d10d28af0b8839f079f8289e603b03530784b9bb5a1e76859e4850c670c7b71c0df84ca3e0d6c177fe9f78a9d8433230a883cd82a2b2b5c7a3306980278570cdb79bf01074a69c9623348824b0c53791d53c6a78cab69e1cfb28368611a397f50f541e16db348dbe5f", 16);
			BigInteger  qValue = new BigInteger("cae4d85f80c147704b0ca48e85fb00a9057aa4acc44668e17f1996d7152690d9", 16);

			public ITestResult Perform()
			{
				BigInteger r = new BigInteger("90892707282f433398488f19d31ac48523a8e2ded68944e0da91c6895ee7045e",16);
				BigInteger s = new BigInteger("3be4620ee88f1ee8f9dd63c7d145b7e554839feeca125049118262ea4651e9de",16);
				Gost3410ParametersGenerator pGen = new Gost3410ParametersGenerator();

				pGen.Init(1024, 2, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair pair = Gost3410KeyGen.GenerateKeyPair();

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		private class Gost3410_BExParam
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-BExParam"; }
			}

			private class SecureRandomImpl22 : SecureRandom
			{
				bool firstLong = true;

				public override long NextLong()
				{
					string x0 = "0x7A007804";
					string c =  "0xD31A4FF7";

					if (firstLong)
					{
						firstLong = false;
						return NumberParsing.DecodeLongFromHex(x0);
					}
					return NumberParsing.DecodeLongFromHex(c);
				}

				public override void NextBytes(byte[] bytes)
				{
					byte[] d = Hex.Decode("7ec123d161477762838c2bea9dbdf33074af6d41d108a066a1e7a07ab3048de2");

					Array.Copy(d, 0, bytes, bytes.Length-d.Length, d.Length);
				}
			};
			SecureRandom init_random = new SecureRandomImpl22();

			private class SecureRandomImpl23 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] k = Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A");

					int i;

					for (i = 0; i < (bytes.Length - k.Length); i += k.Length)
					{
						Array.Copy(k, 0, bytes, i, k.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(k, 0, bytes, i - k.Length, bytes.Length - (i - k.Length));
					}
					else
					{
						Array.Copy(k, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom random = new SecureRandomImpl23();

			private class SecureRandomImpl24 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] x = Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046");

					int i;

					for (i = 0; i < (bytes.Length - x.Length); i += x.Length)
					{
						Array.Copy(x, 0, bytes, i, x.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(x, 0, bytes, i - x.Length, bytes.Length - (i - x.Length));
					}
					else
					{
						Array.Copy(x, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom keyRandom = new SecureRandomImpl24();

			BigInteger  pValue = new BigInteger("9286dbda91eccfc3060aa5598318e2a639f5ba90a4ca656157b2673fb191cd0589ee05f4cef1bd13508408271458c30851ce7a4ef534742bfb11f4743c8f787b11193ba304c0e6bca25701bf88af1cb9b8fd4711d89f88e32b37d95316541bf1e5dbb4989b3df13659b88c0f97a3c1087b9f2d5317d557dcd4afc6d0a754e279", 16);
			BigInteger  qValue = new BigInteger("c966e9b3b8b7cdd82ff0f83af87036c38f42238ec50a876cd390e43d67b6013f", 16);

			public ITestResult Perform()
			{
				BigInteger r = new BigInteger("8f79a582513df84dc247bcb624340cc0e5a34c4324a20ce7fe3ab8ff38a9db71",16);
				BigInteger s = new BigInteger("7508d22fd6cbb45efd438cb875e43f137247088d0f54b29a7c91f68a65b5fa85",16);
				Gost3410ParametersGenerator pGen = new Gost3410ParametersGenerator();

				pGen.Init(1024, 2, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair pair = Gost3410KeyGen.GenerateKeyPair();

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		private class Gost3410_CExParam
			: ITest
		{
			public string Name
			{
				get { return "Gost3410-CExParam"; }
			}

			private class SecureRandomImpl25 : SecureRandom
			{
				bool firstLong = true;

				public override long NextLong()
				{
					string x0 = "0x162AB910";
					string c =  "0x93F828D3";

					if (firstLong)
					{
						firstLong = false;
						return NumberParsing.DecodeLongFromHex(x0);
					}
					return NumberParsing.DecodeLongFromHex(c);
				}

				public override void NextBytes(byte[] bytes)
				{
					byte[] d = Hex.Decode("ca82cce78a738bc46f103d53b9bf809745ec845e4f6da462606c51f60ecf302e31204b81");

					Array.Copy(d, 0, bytes, bytes.Length-d.Length, d.Length);
				}
			};
			SecureRandom init_random = new SecureRandomImpl25();

			private class SecureRandomImpl26 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] k = Hex.Decode("90F3A564439242F5186EBB224C8E223811B7105C64E4F5390807E6362DF4C72A");

					int i;

					for (i = 0; i < (bytes.Length - k.Length); i += k.Length)
					{
						Array.Copy(k, 0, bytes, i, k.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(k, 0, bytes, i - k.Length, bytes.Length - (i - k.Length));
					}
					else
					{
						Array.Copy(k, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom random = new SecureRandomImpl26();

			private class SecureRandomImpl27 : SecureRandom
			{
				public override void NextBytes(byte[] bytes)
				{
					byte[] x = Hex.Decode("3036314538303830343630454235324435324234314132373832433138443046");

					int i;

					for (i = 0; i < (bytes.Length - x.Length); i += x.Length)
					{
						Array.Copy(x, 0, bytes, i, x.Length);
					}

					if (i > bytes.Length)
					{
						Array.Copy(x, 0, bytes, i - x.Length, bytes.Length - (i - x.Length));
					}
					else
					{
						Array.Copy(x, 0, bytes, i, bytes.Length - i);
					}
				}
			};
			SecureRandom keyRandom = new SecureRandomImpl27();

			BigInteger  pValue = new BigInteger("b194036ace14139d36d64295ae6c50fc4b7d65d8b340711366ca93f383653908ee637be428051d86612670ad7b402c09b820fa77d9da29c8111a8496da6c261a53ed252e4d8a69a20376e6addb3bdcd331749a491a184b8fda6d84c31cf05f9119b5ed35246ea4562d85928ba1136a8d0e5a7e5c764ba8902029a1336c631a1d", 16);
			BigInteger  qValue = new BigInteger("96120477df0f3896628e6f4a88d83c93204c210ff262bccb7dae450355125259", 16);

			public ITestResult Perform()
			{
				BigInteger r = new BigInteger("169fdb2dc09f690b71332432bfec806042e258fa9a21dafe73c6abfbc71407d9",16);
				BigInteger s = new BigInteger("9002551808ae40d19f6f31fb67e4563101243cf07cffd5f2f8ff4c537b0c9866",16);
				Gost3410ParametersGenerator pGen = new Gost3410ParametersGenerator();

				pGen.Init(1024, 2, init_random);

				Gost3410Parameters parameters = pGen.GenerateParameters();

				if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
				{
					return new SimpleTestResult(false, Name + ": p or q wrong");
				}

				Gost3410KeyPairGenerator Gost3410KeyGen = new Gost3410KeyPairGenerator();
				Gost3410KeyGenerationParameters genParam = new Gost3410KeyGenerationParameters(keyRandom, parameters);

				Gost3410KeyGen.Init(genParam);

				AsymmetricCipherKeyPair pair = Gost3410KeyGen.GenerateKeyPair();

				ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

				Gost3410Signer Gost3410 = new Gost3410Signer();

				Gost3410.Init(true, param);

				BigInteger[] sig = Gost3410.GenerateSignature(hashmessage);

				if (!r.Equals(sig[0]))
				{
					return new SimpleTestResult(false, Name
						+ ": r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[0].ToString(16));
				}

				if (!s.Equals(sig[1]))
				{
					return new SimpleTestResult(false, Name
						+ ": s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s.ToString(16) + SimpleTest.NewLine
						+ " got      : " + sig[1].ToString(16));
				}

				Gost3410.Init(false, pair.Public);

				if (Gost3410.VerifySignature(hashmessage, sig[0], sig[1]))
				{
					return new SimpleTestResult(true, Name + ": Okay");
				}
				else
				{
					return new SimpleTestResult(false, Name + ": verification fails");
				}
			}
		}

		ITest[] tests =
		{
			new Gost3410_TEST1_512(),
			new Gost3410_TEST2_512(),
//	        new Gost3410_TEST1_1024(),
//	        new Gost3410_TEST2_1024(),
//	        new Gost3410_AParam(),
//	        new Gost3410_BParam(),
//	        new Gost3410_CParam(),
//	        new Gost3410_DParam(),
//	        new Gost3410_AExParam(),
//	        new Gost3410_BExParam(),
//	        new Gost3410_CExParam()
		};

		public string Name
		{
			get { return "Gost3410"; }
		}

		public ITestResult Perform()
		{
			for (int i = 0; i != tests.Length; i++)
			{
				ITestResult result = tests[i].Perform();

				if (!result.IsSuccessful())
				{
					return result;
				}
			}

			return new SimpleTestResult(true, "Gost3410: Okay");
		}

		public static void Main(
			string[] args)
		{
			ITest test = new Gost3410Test();
			ITestResult result = test.Perform();

			Console.WriteLine(result);
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
