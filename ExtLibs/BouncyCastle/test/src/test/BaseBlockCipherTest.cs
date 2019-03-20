using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	public abstract class BaseBlockCipherTest
		: SimpleTest
	{
		string algorithm;

		internal BaseBlockCipherTest(
			string algorithm)
		{
			this.algorithm = algorithm;
		}

		public override string Name
		{
			get { return algorithm; }
		}

		protected void oidTest(
			string[]	oids,
			string[]	names,
			int			groupSize)
		{
			byte[] data = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};

			for (int i = 0; i != oids.Length; i++)
			{
				IBufferedCipher c1 = CipherUtilities.GetCipher(oids[i]);
				IBufferedCipher c2 = CipherUtilities.GetCipher(names[i]);
				CipherKeyGenerator kg = GeneratorUtilities.GetKeyGenerator(oids[i]);

				KeyParameter k = ParameterUtilities.CreateKeyParameter(oids[i], kg.GenerateKey());

				ICipherParameters cp = k;
				if (names[i].IndexOf("/ECB/") < 0)
				{
					cp = new ParametersWithIV(cp, new byte[16]);
				}

				c1.Init(true, cp);
				c2.Init(false, cp);

				byte[] result = c2.DoFinal(c1.DoFinal(data));

				if (!AreEqual(data, result))
				{
					Fail("failed OID test");
				}

				if (k.GetKey().Length != (16 + ((i / groupSize) * 8)))
				{
					Fail("failed key length test");
				}
			}
		}

		protected void wrapOidTest(
			string[]	oids,
			string		name)
		{
			byte[] data = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};

			for (int i = 0; i != oids.Length; i++)
			{
				IWrapper c1 = WrapperUtilities.GetWrapper(oids[i]);
				IWrapper c2 = WrapperUtilities.GetWrapper(name);
				CipherKeyGenerator kg = GeneratorUtilities.GetKeyGenerator(oids[i]);

				KeyParameter k = ParameterUtilities.CreateKeyParameter(oids[i], kg.GenerateKey());

				c1.Init(true, k);
				c2.Init(false, k);

				byte[] wrapped = c1.Wrap(data, 0, data.Length);
				byte[] wKeyBytes = c2.Unwrap(wrapped, 0, wrapped.Length);

				if (!AreEqual(data, wKeyBytes))
				{
					Fail("failed wrap OID test");
				}

				if (k.GetKey().Length != (16 + (i * 8)))
				{
					Fail("failed key length test");
				}
			}
		}

		protected void wrapTest(
			int		id,
			string	wrappingAlgorithm,
			byte[]	kek,
			byte[]	inBytes,
			byte[]	outBytes)
		{
			IWrapper wrapper = WrapperUtilities.GetWrapper(wrappingAlgorithm);

			wrapper.Init(true, ParameterUtilities.CreateKeyParameter(algorithm, kek));

			try
			{
				byte[] cText = wrapper.Wrap(inBytes, 0, inBytes.Length);
				if (!AreEqual(cText, outBytes))
				{
					Fail("failed wrap test " + id  + " expected "
						+ Hex.ToHexString(outBytes) + " got "
						+ Hex.ToHexString(cText));
				}
			}
			catch (TestFailedException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				Fail("failed wrap test exception " + e.ToString(), e);
			}

			wrapper.Init(false, ParameterUtilities.CreateKeyParameter(algorithm, kek));

			try
			{
				byte[] pTextBytes = wrapper.Unwrap(outBytes, 0, outBytes.Length);

				if (!AreEqual(pTextBytes, inBytes))
				{
					Fail("failed unwrap test " + id  + " expected "
						+ Hex.ToHexString(inBytes) + " got "
						+ Hex.ToHexString(pTextBytes));
				}
			}
			catch (Exception e)
			{
				Fail("failed unwrap test exception " + e.ToString(), e);
			}
		}
	}
}
