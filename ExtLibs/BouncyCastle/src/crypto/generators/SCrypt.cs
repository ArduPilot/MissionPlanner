﻿using System;
using System.Diagnostics;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Crypto.Generators
{
    /// <summary>Implementation of the scrypt a password-based key derivation function.</summary>
    /// <remarks>
    /// Scrypt was created by Colin Percival and is specified in
    /// <a href="http://tools.ietf.org/html/draft-josefsson-scrypt-kdf-01">draft-josefsson-scrypt-kd</a>.
    /// </remarks>
    public class SCrypt
	{
        /// <summary>Generate a key using the scrypt key derivation function.</summary>
        /// <param name="P">the bytes of the pass phrase.</param>
        /// <param name="S">the salt to use for this invocation.</param>
        /// <param name="N">CPU/Memory cost parameter. Must be larger than 1, a power of 2 and less than
        ///     <code>2^(128 * r / 8)</code>.</param>
        /// <param name="r">the block size, must be >= 1.</param>
        /// <param name="p">Parallelization parameter. Must be a positive integer less than or equal to
        ///     <code>Int32.MaxValue / (128 * r * 8)</code>.</param>
        /// <param name="dkLen">the length of the key to generate.</param>
        /// <returns>the generated key.</returns>
        public static byte[] Generate(byte[] P, byte[] S, int N, int r, int p, int dkLen)
		{
            if (P == null)
                throw new ArgumentNullException("Passphrase P must be provided.");
            if (S == null)
                throw new ArgumentNullException("Salt S must be provided.");
            if (N <= 1 || !IsPowerOf2(N))
                throw new ArgumentException("Cost parameter N must be > 1 and a power of 2.");
            // Only value of r that cost (as an int) could be exceeded for is 1
            if (r == 1 && N >= 65536)
                throw new ArgumentException("Cost parameter N must be > 1 and < 65536.");
            if (r < 1)
                throw new ArgumentException("Block size r must be >= 1.");
            int maxParallel = Int32.MaxValue / (128 * r * 8);
            if (p < 1 || p > maxParallel)
            {
                throw new ArgumentException("Parallelisation parameter p must be >= 1 and <= " + maxParallel
                    + " (based on block size r of " + r + ")");
            }
            if (dkLen < 1)
                throw new ArgumentException("Generated key length dkLen must be >= 1.");

            return MFcrypt(P, S, N, r, p, dkLen);
		}

		private static byte[] MFcrypt(byte[] P, byte[] S, int N, int r, int p, int dkLen)
		{
			int MFLenBytes = r * 128;
			byte[] bytes = SingleIterationPBKDF2(P, S, p * MFLenBytes);

			uint[] B = null;

			try
			{
				int BLen = bytes.Length >> 2;
				B = new uint[BLen];

				Pack.LE_To_UInt32(bytes, 0, B);

				int MFLenWords = MFLenBytes >> 2;
				for (int BOff = 0; BOff < BLen; BOff += MFLenWords)
				{
					// TODO These can be done in parallel threads
					SMix(B, BOff, N, r);
				}

				Pack.UInt32_To_LE(B, bytes, 0);

				return SingleIterationPBKDF2(P, bytes, dkLen);
			}
			finally
			{
				ClearAll(bytes, B);
			}
		}

		private static byte[] SingleIterationPBKDF2(byte[] P, byte[] S, int dkLen)
		{
			PbeParametersGenerator pGen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
			pGen.Init(P, S, 1);
			KeyParameter key = (KeyParameter)pGen.GenerateDerivedMacParameters(dkLen * 8);
			return key.GetKey();
		}

		private static void SMix(uint[] B, int BOff, int N, int r)
		{
			int BCount = r * 32;

			uint[] blockX1 = new uint[16];
			uint[] blockX2 = new uint[16];
			uint[] blockY = new uint[BCount];

			uint[] X = new uint[BCount];
			uint[][] V = new uint[N][];

			try
			{
				Array.Copy(B, BOff, X, 0, BCount);

				for (int i = 0; i < N; ++i)
				{
					V[i] = (uint[])X.Clone();
					BlockMix(X, blockX1, blockX2, blockY, r);
				}

				uint mask = (uint)N - 1;
				for (int i = 0; i < N; ++i)
				{
					uint j = X[BCount - 16] & mask;
					Xor(X, V[j], 0, X);
					BlockMix(X, blockX1, blockX2, blockY, r);
				}

				Array.Copy(X, 0, B, BOff, BCount);
			}
			finally
			{
				ClearAll(V);
				ClearAll(X, blockX1, blockX2, blockY);
			}
		}

		private static void BlockMix(uint[] B, uint[] X1, uint[] X2, uint[] Y, int r)
		{
			Array.Copy(B, B.Length - 16, X1, 0, 16);

			int BOff = 0, YOff = 0, halfLen = B.Length >> 1;

			for (int i = 2 * r; i > 0; --i)
			{
				Xor(X1, B, BOff, X2);

				Salsa20Engine.SalsaCore(8, X2, X1);
				Array.Copy(X1, 0, Y, YOff, 16);

				YOff = halfLen + BOff - YOff;
				BOff += 16;
			}

			Array.Copy(Y, 0, B, 0, Y.Length);
		}

		private static void Xor(uint[] a, uint[] b, int bOff, uint[] output)
		{
			for (int i = output.Length - 1; i >= 0; --i)
			{
				output[i] = a[i] ^ b[bOff + i];
			}
		}

		private static void Clear(Array array)
		{
			if (array != null)
			{
				Array.Clear(array, 0, array.Length);
			}
		}

		private static void ClearAll(params Array[] arrays)
		{
			foreach (Array array in arrays)
			{
				Clear(array);
			}
		}

        // note: we know X is non-zero
        private static bool IsPowerOf2(int x)
        {
            Debug.Assert(x != 0);

            return (x & (x - 1)) == 0;
        }
    }
}
