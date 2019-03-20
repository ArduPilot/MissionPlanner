using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Org.BouncyCastle.Math.EC.Rfc7748
{
    public abstract class X25519
    {
        private const int C_A = 486662;
        private const int C_A24 = (C_A + 2)/4;

        // 0x1
        //private static readonly int[] S_x = { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        // 0x215132111D8354CB52385F46DCA2B71D440F6A51EB4D1207816B1E0137D48290
        private static readonly int[] PsubS_x = { 0x03D48290, 0x02C7804D, 0x01207816, 0x028F5A68, 0x00881ED4, 0x00A2B71D,
            0x0217D1B7, 0x014CB523, 0x0088EC1A, 0x0042A264 };

        private static int[] precompBase = null;

        private static uint Decode32(byte[] bs, int off)
        {
            uint n = bs[off];
            n |= (uint)bs[++off] << 8;
            n |= (uint)bs[++off] << 16;
            n |= (uint)bs[++off] << 24;
            return n;
        }

        private static void DecodeScalar(byte[] k, int kOff, uint[] n)
        {
            for (int i = 0; i < 8; ++i)
            {
                n[i] = Decode32(k, kOff + i * 4);
            }

            n[0] &= 0xFFFFFFF8U;
            n[7] &= 0x7FFFFFFFU;
            n[7] |= 0x40000000U;
        }

        private static void PointDouble(int[] x, int[] z)
        {
            int[] A = X25519Field.Create();
            int[] B = X25519Field.Create();

            X25519Field.Apm(x, z, A, B);
            X25519Field.Sqr(A, A);
            X25519Field.Sqr(B, B);
            X25519Field.Mul(A, B, x);
            X25519Field.Sub(A, B, A);
            X25519Field.Mul(A, C_A24, z);
            X25519Field.Add(z, B, z);
            X25519Field.Mul(z, A, z);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Precompute()
        {
            if (precompBase != null)
                return;

            precompBase = new int[X25519Field.Size * 252];

            int[] xs = precompBase;
            int[] zs = new int[X25519Field.Size * 251];

            int[] x = X25519Field.Create();     x[0] = 9;          
            int[] z = X25519Field.Create();     z[0] = 1;

            int[] n = X25519Field.Create();
            int[] d = X25519Field.Create();

            X25519Field.Apm(x, z, n, d);

            int[] c = X25519Field.Create();     X25519Field.Copy(d, 0, c, 0);

            int off = 0;
            for (;;)
            {
                X25519Field.Copy(n, 0, xs, off);

                if (off == (X25519Field.Size * 251))
                    break;

                PointDouble(x, z);

                X25519Field.Apm(x, z, n, d);
                X25519Field.Mul(n, c, n);
                X25519Field.Mul(c, d, c);

                X25519Field.Copy(d, 0, zs, off);

                off += X25519Field.Size;
            }

            int[] u = X25519Field.Create();
            X25519Field.Inv(c, u);

            for (;;)
            {
                X25519Field.Copy(xs, off, x, 0);

                X25519Field.Mul(x, u, x);
                //X25519Field.Normalize(x);
                X25519Field.Copy(x, 0, precompBase, off);

                if (off == 0)
                    break;

                off -= X25519Field.Size;
                X25519Field.Copy(zs, off, z, 0);
                X25519Field.Mul(u, z, u);
            }
        }

        public static void ScalarMult(byte[] k, int kOff, byte[] u, int uOff, byte[] r, int rOff)
        {
            uint[] n = new uint[8];     DecodeScalar(k, kOff, n);

            int[] x1 = X25519Field.Create();        X25519Field.Decode(u, uOff, x1);
            int[] x2 = X25519Field.Create();        X25519Field.Copy(x1, 0, x2, 0);
            int[] z2 = X25519Field.Create();        z2[0] = 1;
            int[] x3 = X25519Field.Create();        x3[0] = 1;
            int[] z3 = X25519Field.Create();

            int[] t1 = X25519Field.Create();
            int[] t2 = X25519Field.Create();

            Debug.Assert(n[7] >> 30 == 1U);

            int bit = 254, swap = 1;
            do
            {
                X25519Field.Apm(x3, z3, t1, x3);
                X25519Field.Apm(x2, z2, z3, x2);
                X25519Field.Mul(t1, x2, t1);
                X25519Field.Mul(x3, z3, x3);
                X25519Field.Sqr(z3, z3);
                X25519Field.Sqr(x2, x2);

                X25519Field.Sub(z3, x2, t2);
                X25519Field.Mul(t2, C_A24, z2);
                X25519Field.Add(z2, x2, z2);
                X25519Field.Mul(z2, t2, z2);
                X25519Field.Mul(x2, z3, x2);

                X25519Field.Apm(t1, x3, x3, z3);
                X25519Field.Sqr(x3, x3);
                X25519Field.Sqr(z3, z3);
                X25519Field.Mul(z3, x1, z3);

                --bit;

                int word = bit >> 5, shift = bit & 0x1F;
                int kt = (int)(n[word] >> shift) & 1;
                swap ^= kt;
                X25519Field.CSwap(swap, x2, x3);
                X25519Field.CSwap(swap, z2, z3);
                swap = kt;
            }
            while (bit >= 3);

            Debug.Assert(swap == 0);

            for (int i = 0; i < 3; ++i)
            {
                PointDouble(x2, z2);
            }

            X25519Field.Inv(z2, z2);
            X25519Field.Mul(x2, z2, x2);

            X25519Field.Normalize(x2);
            X25519Field.Encode(x2, r, rOff);
        }

        public static void ScalarMultBase(byte[] k, int kOff, byte[] r, int rOff)
        {
            Precompute();

            uint[] n = new uint[8];     DecodeScalar(k, kOff, n);

            int[] x0 = X25519Field.Create();
            //int[] x1 = X25519Field.Create();        X25519Field.Copy(S_x, 0, x1, 0);
            int[] x1 = X25519Field.Create();        x1[0] = 1;
            int[] z1 = X25519Field.Create();        z1[0] = 1;        
            int[] x2 = X25519Field.Create();        X25519Field.Copy(PsubS_x, 0, x2, 0);
            int[] z2 = X25519Field.Create();        z2[0] = 1;        

            int[] A = x1;
            int[] B = z1;
            int[] C = x0;
            int[] D = A;
            int[] E = B;

            Debug.Assert(n[7] >> 30 == 1U);

            int off = 0, bit = 3, swap = 1;
            do
            {
                X25519Field.Copy(precompBase, off, x0, 0);
                off += X25519Field.Size;

                int word = bit >> 5, shift = bit & 0x1F;
                int kt = (int)(n[word] >> shift) & 1;
                swap ^= kt;
                X25519Field.CSwap(swap, x1, x2);
                X25519Field.CSwap(swap, z1, z2);
                swap = kt;

                X25519Field.Apm(x1, z1, A, B);
                X25519Field.Mul(x0, B, C);
                X25519Field.Carry(A);
                X25519Field.Apm(A, C, D, E);
                X25519Field.Sqr(D, D);
                X25519Field.Sqr(E, E);
                X25519Field.Mul(z2, D, x1);
                X25519Field.Mul(x2, E, z1);
            }
            while (++bit < 255);

            Debug.Assert(swap == 1);

            for (int i = 0; i < 3; ++i)
            {
                PointDouble(x1, z1);
            }

            X25519Field.Inv(z1, z1);
            X25519Field.Mul(x1, z1, x1);

            X25519Field.Normalize(x1);
            X25519Field.Encode(x1, r, rOff);
        }
    }
}
