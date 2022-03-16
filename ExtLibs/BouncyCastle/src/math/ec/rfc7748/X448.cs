using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Org.BouncyCastle.Math.EC.Rfc7748
{
    public abstract class X448
    {
        private const uint C_A = 156326;
        private const uint C_A24 = (C_A + 2)/4;

        // 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE
        private static readonly uint[] S_x = { 0x0FFFFFFEU, 0x0FFFFFFFU, 0x0FFFFFFFU, 0x0FFFFFFFU, 0x0FFFFFFFU, 0x0FFFFFFFU,
            0x0FFFFFFFU, 0x0FFFFFFFU, 0x0FFFFFFEU, 0x0FFFFFFFU, 0x0FFFFFFFU, 0x0FFFFFFFU, 0x0FFFFFFFU, 0x0FFFFFFFU, 0x0FFFFFFFU,
            0x0FFFFFFFU };

        // 0xF0FAB725013244423ACF03881AFFEB7BDACDD1031C81B9672954459D84C1F823F1BD65643ACE1B5123AC33FF1C69BAF8ACB1197DC99D2720
        private static readonly uint[] PsubS_x = { 0x099D2720U, 0x0B1197DCU, 0x09BAF8ACU, 0x033FF1C6U, 0x0B5123ACU,
            0x0643ACE1U, 0x03F1BD65U, 0x084C1F82U, 0x0954459DU, 0x081B9672U, 0x0DD1031CU, 0x0EB7BDACU, 0x03881AFFU, 0x0423ACF0U,
            0x05013244U, 0x0F0FAB72U };

        private static uint[] precompBase = null;

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
            for (int i = 0; i < 14; ++i)
            {
                n[i] = Decode32(k, kOff + i * 4);
            }

            n[ 0] &= 0xFFFFFFFCU;
            n[13] |= 0x80000000U;
        }

        private static void PointDouble(uint[] x, uint[] z)
        {
            uint[] A = X448Field.Create();
            uint[] B = X448Field.Create();

            //X448Field.Apm(x, z, A, B);
            X448Field.Add(x, z, A);
            X448Field.Sub(x, z, B);
            X448Field.Sqr(A, A);
            X448Field.Sqr(B, B);
            X448Field.Mul(A, B, x);
            X448Field.Sub(A, B, A);
            X448Field.Mul(A, C_A24, z);
            X448Field.Add(z, B, z);
            X448Field.Mul(z, A, z);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Precompute()
        {
            if (precompBase != null)
                return;

            precompBase = new uint[X448Field.Size * 446];

            uint[] xs = precompBase;
            uint[] zs = new uint[X448Field.Size * 445];

            uint[] x = X448Field.Create();     x[0] = 5;          
            uint[] z = X448Field.Create();     z[0] = 1;

            uint[] n = X448Field.Create();
            uint[] d = X448Field.Create();

            //X448Field.Apm(x, z, n, d);
            X448Field.Add(x, z, n);
            X448Field.Sub(x, z, d);

            uint[] c = X448Field.Create();     X448Field.Copy(d, 0, c, 0);

            int off = 0;
            for (;;)
            {
                X448Field.Copy(n, 0, xs, off);

                if (off == (X448Field.Size * 445))
                    break;

                PointDouble(x, z);

                //X448Field.Apm(x, z, n, d);
                X448Field.Add(x, z, n);
                X448Field.Sub(x, z, d);
                X448Field.Mul(n, c, n);
                X448Field.Mul(c, d, c);

                X448Field.Copy(d, 0, zs, off);

                off += X448Field.Size;
            }

            uint[] u = X448Field.Create();
            X448Field.Inv(c, u);

            for (;;)
            {
                X448Field.Copy(xs, off, x, 0);

                X448Field.Mul(x, u, x);
                //X448Field.Normalize(x);
                X448Field.Copy(x, 0, precompBase, off);

                if (off == 0)
                    break;

                off -= X448Field.Size;
                X448Field.Copy(zs, off, z, 0);
                X448Field.Mul(u, z, u);
            }
        }

        public static void ScalarMult(byte[] k, int kOff, byte[] u, int uOff, byte[] r, int rOff)
        {
            uint[] n = new uint[14];    DecodeScalar(k, kOff, n);

            uint[] x1 = X448Field.Create();     X448Field.Decode(u, uOff, x1);
            uint[] x2 = X448Field.Create();     X448Field.Copy(x1, 0, x2, 0);
            uint[] z2 = X448Field.Create();     z2[0] = 1;
            uint[] x3 = X448Field.Create();     x3[0] = 1;
            uint[] z3 = X448Field.Create();

            uint[] t1 = X448Field.Create();
            uint[] t2 = X448Field.Create();

            Debug.Assert(n[13] >> 31 == 1U);

            int bit = 447, swap = 1;
            do
            {
                //X448Field.Apm(x3, z3, t1, x3);
                X448Field.Add(x3, z3, t1);
                X448Field.Sub(x3, z3, x3);
                //X448Field.Apm(x2, z2, z3, x2);
                X448Field.Add(x2, z2, z3);
                X448Field.Sub(x2, z2, x2);

                X448Field.Mul(t1, x2, t1);
                X448Field.Mul(x3, z3, x3);
                X448Field.Sqr(z3, z3);
                X448Field.Sqr(x2, x2);

                X448Field.Sub(z3, x2, t2);
                X448Field.Mul(t2, C_A24, z2);
                X448Field.Add(z2, x2, z2);
                X448Field.Mul(z2, t2, z2);
                X448Field.Mul(x2, z3, x2);

                //X448Field.Apm(t1, x3, x3, z3);
                X448Field.Sub(t1, x3, z3);
                X448Field.Add(t1, x3, x3);
                X448Field.Sqr(x3, x3);
                X448Field.Sqr(z3, z3);
                X448Field.Mul(z3, x1, z3);

                --bit;

                int word = bit >> 5, shift = bit & 0x1F;
                int kt = (int)(n[word] >> shift) & 1;
                swap ^= kt;
                X448Field.CSwap(swap, x2, x3);
                X448Field.CSwap(swap, z2, z3);
                swap = kt;
            }
            while (bit >= 2);

            Debug.Assert(swap == 0);

            for (int i = 0; i < 2; ++i)
            {
                PointDouble(x2, z2);
            }

            X448Field.Inv(z2, z2);
            X448Field.Mul(x2, z2, x2);

            X448Field.Normalize(x2);
            X448Field.Encode(x2, r, rOff);
        }

        public static void ScalarMultBase(byte[] k, int kOff, byte[] r, int rOff)
        {
            Precompute();

            uint[] n = new uint[14];    DecodeScalar(k, kOff, n);

            uint[] x0 = X448Field.Create();
            uint[] x1 = X448Field.Create();     X448Field.Copy(S_x, 0, x1, 0);
            uint[] z1 = X448Field.Create();     z1[0] = 1;        
            uint[] x2 = X448Field.Create();     X448Field.Copy(PsubS_x, 0, x2, 0);
            uint[] z2 = X448Field.Create();     z2[0] = 1;

            uint[] A = X448Field.Create();
            uint[] B = z1;
            uint[] C = x0;
            uint[] D = x1;
            uint[] E = B;

            Debug.Assert(n[13] >> 31 == 1U);

            int off = 0, bit = 2, swap = 1;
            do
            {
                X448Field.Copy(precompBase, off, x0, 0);
                off += X448Field.Size;

                int word = bit >> 5, shift = bit & 0x1F;
                int kt = (int)(n[word] >> shift) & 1;
                swap ^= kt;
                X448Field.CSwap(swap, x1, x2);
                X448Field.CSwap(swap, z1, z2);
                swap = kt;

                //X448Field.Apm(x1, z1, A, B);
                X448Field.Add(x1, z1, A);
                X448Field.Sub(x1, z1, B);
                X448Field.Mul(x0, B, C);
                X448Field.Carry(A);
                //X448Field.Apm(A, C, D, E);
                X448Field.Add(A, C, D);
                X448Field.Sub(A, C, E);
                X448Field.Sqr(D, D);
                X448Field.Sqr(E, E);
                X448Field.Mul(z2, D, x1);
                X448Field.Mul(x2, E, z1);
            }
            while (++bit < 448);

            Debug.Assert(swap == 1);

            for (int i = 0; i < 2; ++i)
            {
                PointDouble(x1, z1);
            }

            X448Field.Inv(z1, z1);
            X448Field.Mul(x1, z1, x1);

            X448Field.Normalize(x1);
            X448Field.Encode(x1, r, rOff);
        }
    }
}
