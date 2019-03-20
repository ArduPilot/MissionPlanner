using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/// <remarks> test for Pkcs12 key generation - vectors from
	/// <a href="http://www.drh-consultancy.demon.co.uk/test.txt">
	///	http://www.drh-consultancy.demon.co.uk/test.txt</a>
	/// </remarks>
	[TestFixture]
	public class Pkcs12Test
		: SimpleTest
	{
		public override string Name
		{
			get { return "Pkcs12Test"; }
		}

		internal char[] password1 = new char[]{'s', 'm', 'e', 'g'};
		internal char[] password2 = new char[]{'q', 'u', 'e', 'e', 'g'};

		private void Run1(int id, char[] password, byte[] salt, int iCount, byte[] result)
		{
			PbeParametersGenerator generator = new Pkcs12ParametersGenerator(new Sha1Digest());

			generator.Init(PbeParametersGenerator.Pkcs12PasswordToBytes(password), salt, iCount);

			ICipherParameters key = generator.GenerateDerivedParameters("DESEDE", 24 * 8);

			if (!Arrays.AreEqual(result, ((KeyParameter) key).GetKey()))
			{
				Fail("id " + id + " Failed");
			}
		}

		private void Run2(int id, char[] password, byte[] salt, int iCount, byte[] result)
		{
			PbeParametersGenerator generator = new Pkcs12ParametersGenerator(new Sha1Digest());

			generator.Init(PbeParametersGenerator.Pkcs12PasswordToBytes(password), salt, iCount);

			ParametersWithIV parameters = (ParametersWithIV)
				generator.GenerateDerivedParameters("DES", 64, 64);

			if (!Arrays.AreEqual(result, parameters.GetIV()))
			{
				Fail("id " + id + " Failed");
			}
		}

		private void Run3(int id, char[] password, byte[] salt, int iCount, byte[] result)
		{
			PbeParametersGenerator generator = new Pkcs12ParametersGenerator(new Sha1Digest());

			generator.Init(PbeParametersGenerator.Pkcs12PasswordToBytes(password), salt, iCount);

			ICipherParameters key = generator.GenerateDerivedMacParameters(160);

			if (!Arrays.AreEqual(result, ((KeyParameter) key).GetKey()))
			{
				Fail("id " + id + " Failed");
			}
		}

		public override void PerformTest()
		{
			Run1(1, password1, Hex.Decode("0A58CF64530D823F"), 1, Hex.Decode("8AAAE6297B6CB04642AB5B077851284EB7128F1A2A7FBCA3"));
			Run2(2, password1, Hex.Decode("0A58CF64530D823F"), 1, Hex.Decode("79993DFE048D3B76"));
			Run1(3, password1, Hex.Decode("642B99AB44FB4B1F"), 1, Hex.Decode("F3A95FEC48D7711E985CFE67908C5AB79FA3D7C5CAA5D966"));
			Run2(4, password1, Hex.Decode("642B99AB44FB4B1F"), 1, Hex.Decode("C0A38D64A79BEA1D"));
			Run3(5, password1, Hex.Decode("3D83C0E4546AC140"), 1, Hex.Decode("8D967D88F6CAA9D714800AB3D48051D63F73A312"));
			Run1(6, password2, Hex.Decode("05DEC959ACFF72F7"), 1000, Hex.Decode("ED2034E36328830FF09DF1E1A07DD357185DAC0D4F9EB3D4"));
			Run2(7, password2, Hex.Decode("05DEC959ACFF72F7"), 1000, Hex.Decode("11DEDAD7758D4860"));
			Run1(8, password2, Hex.Decode("1682C0FC5B3F7EC5"), 1000, Hex.Decode("483DD6E919D7DE2E8E648BA8F862F3FBFBDC2BCB2C02957F"));
			Run2(9, password2, Hex.Decode("1682C0FC5B3F7EC5"), 1000, Hex.Decode("9D461D1B00355C50"));
			Run3(10, password2, Hex.Decode("263216FCC2FAB31C"), 1000, Hex.Decode("5EC4C7A80DF652294C3925B6489A7AB857C83476"));
		}

		public static void Main(
			string[] args)
		{
			RunTest(new Pkcs12Test());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
