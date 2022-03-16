using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Security.Tests
{
	[TestFixture]
	public class TestParameterUtilities
	{
		[Test]
		public void TestCreateKeyParameter()
		{
			SecureRandom random = new SecureRandom();

			doTestCreateKeyParameter("AES", NistObjectIdentifiers.IdAes128Cbc,
				128, typeof(KeyParameter), random);
			doTestCreateKeyParameter("DES", OiwObjectIdentifiers.DesCbc,
				64, typeof(DesParameters), random);
			doTestCreateKeyParameter("DESEDE", PkcsObjectIdentifiers.DesEde3Cbc,
				192, typeof(DesEdeParameters), random);
			doTestCreateKeyParameter("RC2", PkcsObjectIdentifiers.RC2Cbc,
				128, typeof(RC2Parameters), random);
		}

		private void doTestCreateKeyParameter(
			string				algorithm,
			DerObjectIdentifier	oid,
			int					keyBits,
			Type				expectedType,
			SecureRandom		random)
		{
			int keyLength = keyBits / 8;
			byte[] bytes = new byte[keyLength];
			random.NextBytes(bytes);

			KeyParameter key;

			key = ParameterUtilities.CreateKeyParameter(algorithm, bytes);
			checkKeyParameter(key, expectedType, bytes);

			key = ParameterUtilities.CreateKeyParameter(oid, bytes);
			checkKeyParameter(key, expectedType, bytes);

			bytes = new byte[keyLength * 2];
			random.NextBytes(bytes);

			int offset = random.Next(1, keyLength);
			byte[] expected = new byte[keyLength];
			Array.Copy(bytes, offset, expected, 0, keyLength);

			key = ParameterUtilities.CreateKeyParameter(algorithm, bytes, offset, keyLength);
			checkKeyParameter(key, expectedType, expected);

			key = ParameterUtilities.CreateKeyParameter(oid, bytes, offset, keyLength);
			checkKeyParameter(key, expectedType, expected);
		}

		private void checkKeyParameter(
			KeyParameter	key,
			Type			expectedType,
			byte[]			expectedBytes)
		{
			Assert.IsTrue(expectedType.IsInstanceOfType(key));
			Assert.IsTrue(Arrays.AreEqual(expectedBytes, key.GetKey()));
		}
	}
}
