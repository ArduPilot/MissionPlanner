// TODO Need a replacement for the Java properties class to finish this class

//using System;
//using System.IO;
//using System.Text;
//
//using Org.BouncyCastle.Asn1.Sec;
//using Org.BouncyCastle.Asn1.X9;
//using Org.BouncyCastle.Math.EC;
//using Org.BouncyCastle.Security;
//
//namespace Org.BouncyCastle.Math.EC.Tests
//{
//	public class F2mProofer
//	{
//		private const int NUM_SAMPLES = 1000;
//
//		private static readonly string PATH = "crypto/test/src/org/bouncycastle/math/ec/test/samples/";
//
//		private static readonly string INPUT_FILE_NAME_PREFIX = "Input_";
//
//		private static readonly string RESULT_FILE_NAME_PREFIX = "Output_";
//
//		/**
//		* The standard curves on which the tests are done
//		*/
//		public static readonly string[] Curves = { "sect163r2", "sect233r1",
//			"sect283r1", "sect409r1", "sect571r1" };
//
//		private string pointToString(F2mPoint p)
//		{
//			F2mFieldElement x = (F2mFieldElement) p.X;
//			F2mFieldElement y = (F2mFieldElement) p.Y;
//
//			int m = x.M;
//			int len = m / 2 + 5;
//
//			StringBuilder sb = new StringBuilder(len);
//			sb.Append('(');
//			sb.Append(x.ToBigInteger().ToString(16));
//			sb.Append(", ");
//			sb.Append(y.ToBigInteger().ToString(16));
//			sb.Append(')');
//
//			return sb.ToString();
//		}
//
//		private void generateRandomInput(X9ECParameters x9ECParameters)
//		{
//			F2mPoint g = (F2mPoint) x9ECParameters.G;
//			int m = ((F2mFieldElement) g.X).M;
//
//			SecureRandom secRand = new SecureRandom(); //SecureRandom.GetInstance("SHA1PRNG");
//			Properties inputProps = new Properties();
//			for (int i = 0; i < NUM_SAMPLES; i++)
//			{
//				BigInteger rand = new BigInteger(m, secRand);
//				inputProps.put(i.ToString(), rand.ToString(16));
//			}
//			string bits = m.ToString();
//			FileStream fos = File.Create(PATH
//				+ INPUT_FILE_NAME_PREFIX + bits + ".properties");
//			inputProps.store(fos, "Input Samples of length" + bits);
//			fos.Close();
//		}
//
//		private void multiplyPoints(X9ECParameters x9ECParameters,
//			string classPrefix)
//		{
//			F2mPoint g = (F2mPoint) x9ECParameters.G;
//			int m = ((F2mFieldElement) g.X).M;
//
//			string inputFileName = PATH + INPUT_FILE_NAME_PREFIX + m
//				+ ".properties";
//			Properties inputProps = new Properties();
//			FileStream fis = File.OpenRead(inputFileName); 
//			inputProps.load(fis);
//			fis.Close();
//
//			Properties outputProps = new Properties();
//
//			for (int i = 0; i < NUM_SAMPLES; i++)
//			{
//				BigInteger rand = new BigInteger(inputProps.getProperty(Integer
//					.ToString(i)), 16);
//				F2mPoint result = (F2mPoint) g.Multiply(rand).normalize();
//				string resultStr = pointToString(result);
//				outputProps.setProperty(i.ToString(), resultStr);
//			}
//
//			string outputFileName = PATH + RESULT_FILE_NAME_PREFIX + classPrefix
//				+ "_" + m + ".properties";
//			FileStream fos = File.Create(outputFileName);
//			outputProps.store(fos, "Output Samples of length" + m);
//			fos.Close();
//		}
//
//		private Properties loadResults(string classPrefix, int m)
//		{
//			FileStream fis = File.OpenRead(PATH
//				+ RESULT_FILE_NAME_PREFIX + classPrefix + "_" + m + ".properties");
//			Properties res = new Properties();
//			res.load(fis);
//			fis.Close();
//			return res;
//		}
//
//		private void compareResult(X9ECParameters x9ECParameters,
//			string classPrefix1, string classPrefix2)
//		{
//			F2mPoint g = (F2mPoint) x9ECParameters.G;
//			int m = ((F2mFieldElement) g.X).M;
//
//			Properties res1 = loadResults(classPrefix1, m);
//			Properties res2 = loadResults(classPrefix2, m);
//
//			Set keys = res1.keySet();
//			Iterator iter = keys.iterator();
//			while (iter.hasNext())
//			{
//				string key = (string) iter.next();
//				string result1 = res1.getProperty(key);
//				string result2 = res2.getProperty(key);
//				if (!(result1.Equals(result2)))
//				{
//					Console.Error.WriteLine("Difference found: m = " + m + ", "
//						+ result1 + " does not equal " + result2);
//				}
//			}
//
//		}
//
//		private static void usage()
//		{
//			Console.Error.WriteLine("Usage: F2mProofer [-init | -Multiply <className> "
//				+ "| -compare <className1> <className2>]");
//		}
//
//		public static void Main(string[] args)
//		{
//			if (args.Length == 0)
//			{
//				usage();
//				return;
//			}
//			F2mProofer proofer = new F2mProofer();
//			if (args[0].Equals("-init"))
//			{
//				Console.WriteLine("Generating random input...");
//				for (int i = 0; i < Curves.Length; i++)
//				{
//					X9ECParameters x9ECParameters = SecNamedCurves
//						.GetByName(Curves[i]);
//					proofer.generateRandomInput(x9ECParameters);
//				}
//				Console.WriteLine("Successfully generated random input in " + PATH);
//			}
//			else if (args[0].Equals("-compare"))
//			{
//				if (args.Length < 3)
//				{
//					usage();
//					return;
//				}
//				string classPrefix1 = args[1];
//				string classPrefix2 = args[2];
//				Console.WriteLine("Comparing results...");
//				for (int i = 0; i < Curves.Length; i++)
//				{
//					X9ECParameters x9ECParameters = SecNamedCurves
//						.GetByName(Curves[i]);
//					proofer.compareResult(x9ECParameters, classPrefix1,
//						classPrefix2);
//				}
//				Console.WriteLine("Successfully compared results in " + PATH);
//			}
//			else if (args[0].Equals("-Multiply"))
//			{
//				if (args.Length < 2)
//				{
//					usage();
//					return;
//				}
//				string classPrefix = args[1];
//				Console.WriteLine("Multiplying points...");
//				for (int i = 0; i < Curves.Length; i++)
//				{
//					X9ECParameters x9ECParameters = SecNamedCurves
//						.GetByName(Curves[i]);
//					proofer.multiplyPoints(x9ECParameters, classPrefix);
//				}
//				Console.WriteLine("Successfully generated multiplied points in "
//					+ PATH);
//			}
//			else
//			{
//				usage();
//			}
//		}
//	}
//}
