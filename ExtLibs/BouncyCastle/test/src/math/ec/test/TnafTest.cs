//using System;
//using System.Text;
//
//using NUnit.Framework;
//
//using Org.BouncyCastle.Asn1.Sec;
//using Org.BouncyCastle.Asn1.X9;
//using Org.BouncyCastle.Math.EC.Multiplier;
//using Org.BouncyCastle.Utilities.Date;
//
//namespace Org.BouncyCastle.Math.EC.Tests
//{
//	[TestFixture, Explicit]
//	public class TnafTest
//	{
//		private Random m_rand = new Random();
//
//		private string ecPointToString(
//			ECPoint p) 
//		{
//			StringBuilder sb = new StringBuilder("x = ");
//			sb.Append(p.X.ToBigInteger().ToString());
//			sb.Append("; y = ");
//			sb.Append(p.Y.ToBigInteger().ToString());
//			return sb.ToString();
//		}
//
//		private ECPoint repeatedMultiply(ECPoint p, BigInteger k)
//		{
//			ECPoint result = p.Multiply(k);
//			for (int i = 1; i < 10; ++i)
//			{
//				ECPoint check = p.Multiply(k);
//				Assert.AreEqual(result, check);
//			}
//			return result;
//		}
//
//		private void ImplTestMultiplyTnaf(string curveName) 
//		{
//			X9ECParameters x9ECParameters = SecNamedCurves.GetByName(curveName);
//
//			AbstractF2mCurve curve = (AbstractF2mCurve)x9ECParameters.Curve;
//			BigInteger n = curve.N;
//
//			// The generator is multiplied by random b to get random q
//			BigInteger b = new BigInteger(n.BitLength, m_rand);
//			ECPoint g = x9ECParameters.G;
//			F2mPoint p = (F2mPoint) g.Multiply(b);
//
//			BigInteger k = new BigInteger(n.BitLength, m_rand);
//			long now1 = DateTimeUtilities.CurrentUnixMs();
//			p.SetECMultiplier(new WTauNafMultiplier());
//			ECPoint refRWTnaf = repeatedMultiply(p, k);
//			long now2 = DateTimeUtilities.CurrentUnixMs();
//			p.SetECMultiplier(new WNafMultiplier());
//			ECPoint refWnaf = repeatedMultiply(p, k);
//			long now3 = DateTimeUtilities.CurrentUnixMs();
//			p.SetECMultiplier(new FpNafMultiplier());
//			ECPoint refFpNaf = repeatedMultiply(p, k);
//			long now4 = DateTimeUtilities.CurrentUnixMs();
//			p.SetECMultiplier(new ReferenceMultiplier());
//			ECPoint reference = repeatedMultiply(p, k);
//			long now5 = DateTimeUtilities.CurrentUnixMs();
//
//			Assert.AreEqual(reference, refRWTnaf, "WTNAF multiplication is incorrect");
//			Assert.AreEqual(reference, refFpNaf, "FPNAF multiplication is incorrect");
//			Assert.AreEqual(reference, refWnaf, "WNAF multiplication is incorrect");
//
//			Console.WriteLine(curveName + ": Multiply WTNAF took millis:  " + (now2 - now1));
//			Console.WriteLine(curveName + ": Multiply WNAF took millis:   " + (now3 - now2));
//			Console.WriteLine(curveName + ": Multiply FPNAF took millis:  " + (now4 - now3));
//			Console.WriteLine(curveName + ": Multiply REFE took millis:   " + (now5 - now4));
//
////			Console.WriteLine(curveName + ": refRWTnaf  = " + ecPointToString(refRWTnaf));
////			Console.WriteLine(curveName + ": refWnaf    = " + ecPointToString(refWnaf));
////			Console.WriteLine(curveName + ": refFpNaf   = " + ecPointToString(refFpNaf));
////			Console.WriteLine(curveName + ": reference  = " + ecPointToString(reference) + "\n");
//			Console.WriteLine();
//		}
//
//		[Test]
//		public void TestMultiplyTnaf() 
//		{
//			Console.WriteLine("\n\n\n*****  Start test multiplications on F2m (Koblitz) *****");
//			ImplTestMultiplyTnaf("sect163k1");
//			ImplTestMultiplyTnaf("sect233k1");
//			ImplTestMultiplyTnaf("sect239k1");
//			ImplTestMultiplyTnaf("sect283k1");
//			ImplTestMultiplyTnaf("sect409k1");
//			ImplTestMultiplyTnaf("sect571k1");
//		}
//
//		private void ImplTestMultiplyWnaf(String curveName) 
//		{
//			X9ECParameters x9ECParameters = SecNamedCurves.GetByName(curveName);
//
//			BigInteger r = x9ECParameters.N;
//
//			// The generator is multiplied by random b to get random q
//			BigInteger b = new BigInteger(r.BitLength, m_rand);
//			ECPoint g = x9ECParameters.G;
//			ECPoint p = g.Multiply(b);
//
//			BigInteger k = new BigInteger(r.BitLength, m_rand);
//			long now1 = DateTimeUtilities.CurrentUnixMs();
//			p.SetECMultiplier(new WNafMultiplier());
//			ECPoint refWnaf = repeatedMultiply(p, k);
//			long now2 = DateTimeUtilities.CurrentUnixMs();
//			p.SetECMultiplier(new FpNafMultiplier());
//			ECPoint refFpNaf = repeatedMultiply(p, k);
//			long now3 = DateTimeUtilities.CurrentUnixMs();
//			p.SetECMultiplier(new ReferenceMultiplier());
//			ECPoint reference = repeatedMultiply(p, k);
//			long now4 = DateTimeUtilities.CurrentUnixMs();
//
//			Assert.AreEqual(reference, refWnaf, "WNAF multiplication is incorrect");
//			Assert.AreEqual(reference, refFpNaf, "FPNAF multiplication is incorrect");
//
//			Console.WriteLine(curveName + ": Multiply WNAF took millis:   " + (now2 - now1));
//			Console.WriteLine(curveName + ": Multiply FPNAF took millis:  " + (now3 - now2));
//			Console.WriteLine(curveName + ": Multiply REFE took millis:   " + (now4 - now3));
//
////			Console.WriteLine(curveName + ": refWnaf    = " + ecPointToString(refWnaf));
////			Console.WriteLine(curveName + ": refFpNaf   = " + ecPointToString(refFpNaf));
////			Console.WriteLine(curveName + ": reference  = " + ecPointToString(reference));
//			Console.WriteLine();
//		}
//
//		[Test]
//		public void TestMultiplyWnaf() 
//		{
//			Console.WriteLine("\n\n\n*****  Start test multiplications on F2m *****");
//			ImplTestMultiplyWnaf("sect113r1");
//			ImplTestMultiplyWnaf("sect113r2");
//			ImplTestMultiplyWnaf("sect131r1");
//			ImplTestMultiplyWnaf("sect131r2");
//			ImplTestMultiplyWnaf("sect163r1");
//			ImplTestMultiplyWnaf("sect163r2");
//			ImplTestMultiplyWnaf("sect193r1");
//			ImplTestMultiplyWnaf("sect193r2");
//			ImplTestMultiplyWnaf("sect233r1");
//			ImplTestMultiplyWnaf("sect283r1");
//			ImplTestMultiplyWnaf("sect409r1");
//			ImplTestMultiplyWnaf("sect571r1");
//
//			Console.WriteLine("\n\n\n*****  Start test multiplications on Fp  *****");
//			ImplTestMultiplyWnaf("secp112r1");
//			ImplTestMultiplyWnaf("secp128r1");
//			ImplTestMultiplyWnaf("secp160r1");
//			ImplTestMultiplyWnaf("secp192r1");
//			ImplTestMultiplyWnaf("secp224r1");
//			ImplTestMultiplyWnaf("secp256r1");
//			ImplTestMultiplyWnaf("secp384r1");
//			ImplTestMultiplyWnaf("secp521r1");
//		}
//	}
//}
