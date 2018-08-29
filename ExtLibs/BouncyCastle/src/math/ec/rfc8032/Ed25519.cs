using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Math.EC.Rfc8032
{
    public abstract class Ed25519
    {
        private const long M28L = 0x0FFFFFFFL;
        private const long M32L = 0xFFFFFFFFL;

        private const int PointBytes = 32;
        private const int ScalarUints = 8;
        private const int ScalarBytes = ScalarUints * 4;

        public static readonly int PublicKeySize = PointBytes;
        public static readonly int SecretKeySize = 32;
        public static readonly int SignatureSize = PointBytes + ScalarBytes;

        //private static readonly byte[] Dom2Prefix = Strings.ToByteArray("SigEd25519 no Ed25519 collisions");

        private static readonly uint[] P = { 0xFFFFFFEDU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0x7FFFFFFFU };
        private static readonly uint[] L = { 0x5CF5D3EDU, 0x5812631AU, 0xA2F79CD6U, 0x14DEF9DEU, 0x00000000U, 0x00000000U, 0x00000000U, 0x10000000U };

        private const int L0 = unchecked((int)0xFCF5D3ED);  // L0:26/--
        private const int L1 =                0x012631A6;   // L1:24/22
        private const int L2 =                0x079CD658;   // L2:27/--
        private const int L3 = unchecked((int)0xFF9DEA2F);  // L3:23/--
        private const int L4 =                0x000014DF;   // L4:12/11

        private static readonly int[] B_x = { 0x0325D51A, 0x018B5823, 0x007B2C95, 0x0304A92D, 0x00D2598E, 0x01D6DC5C,
            0x01388C7F, 0x013FEC0A, 0x029E6B72, 0x0042D26D };
        private static readonly int[] B_y = { 0x02666658, 0x01999999, 0x00666666, 0x03333333, 0x00CCCCCC, 0x02666666,
            0x01999999, 0x00666666, 0x03333333, 0x00CCCCCC, };
        private static readonly int[] C_d = { 0x035978A3, 0x02D37284, 0x018AB75E, 0x026A0A0E, 0x0000E014, 0x0379E898,
            0x01D01E5D, 0x01E738CC, 0x03715B7F, 0x00A406D9 };
        private static readonly int[] C_d2 = { 0x02B2F159, 0x01A6E509, 0x01156EBD, 0x00D4141D, 0x0001C029, 0x02F3D130,
            0x03A03CBB, 0x01CE7198, 0x02E2B6FF, 0x00480DB3 };
        private static readonly int[] C_d4 = { 0x0165E2B2, 0x034DCA13, 0x002ADD7A, 0x01A8283B, 0x00038052, 0x01E7A260,
            0x03407977, 0x019CE331, 0x01C56DFF, 0x00901B67 };

        private const int WnafWidthBase = 7;

        private const int PrecompBlocks = 8;
        private const int PrecompTeeth = 4;
        private const int PrecompSpacing = 8;
        private const int PrecompPoints = 1 << (PrecompTeeth - 1);
        private const int PrecompMask = PrecompPoints - 1;

        // TODO[ed25519] Convert to PointPrecomp
        private static PointExt[] precompBaseTable = null;
        private static int[] precompBase = null;

        private class PointAccum
        {
            internal int[] x = X25519Field.Create();
            internal int[] y = X25519Field.Create();
            internal int[] z = X25519Field.Create();
            internal int[] u = X25519Field.Create();
            internal int[] v = X25519Field.Create();
        }

        private class PointExt
        {
            internal int[] x = X25519Field.Create();
            internal int[] y = X25519Field.Create();
            internal int[] z = X25519Field.Create();
            internal int[] t = X25519Field.Create();
        }

        private class PointPrecomp
        {
            internal int[] ypx_h = X25519Field.Create();
            internal int[] ymx_h = X25519Field.Create();
            internal int[] xyd = X25519Field.Create();
        }

        private static byte[] CalculateS(byte[] r, byte[] k, byte[] s)
        {
            uint[] t = new uint[ScalarUints * 2];   DecodeScalar(r, 0, t);
            uint[] u = new uint[ScalarUints];       DecodeScalar(k, 0, u);
            uint[] v = new uint[ScalarUints];       DecodeScalar(s, 0, v);

            Nat256.MulAddTo(u, v, t);

            byte[] result = new byte[ScalarBytes * 2];
            for (int i = 0; i < t.Length; ++i)
            {
                Encode32(t[i], result, i * 4);
            }
            return ReduceScalar(result);
        }

        private static bool CheckPointVar(byte[] p)
        {
            uint[] t = new uint[8];
            Decode32(p, 0, t, 0, 8);
            t[7] &= 0x7FFFFFFFU;
            return !Nat256.Gte(t, P);
        }

        private static bool CheckScalarVar(byte[] s)
        {
            uint[] n = new uint[ScalarUints];
            DecodeScalar(s, 0, n);
            return !Nat256.Gte(n, L);
        }

        private static uint Decode24(byte[] bs, int off)
        {
            uint n = bs[off];
            n |= (uint)bs[++off] << 8;
            n |= (uint)bs[++off] << 16;
            return n;
        }

        private static uint Decode32(byte[] bs, int off)
        {
            uint n = bs[off];
            n |= (uint)bs[++off] << 8;
            n |= (uint)bs[++off] << 16;
            n |= (uint)bs[++off] << 24;
            return n;
        }

        private static void Decode32(byte[] bs, int bsOff, uint[] n, int nOff, int nLen)
        {
            for (int i = 0; i < nLen; ++i)
            {
                n[nOff + i] = Decode32(bs, bsOff + i * 4);
            }
        }

        private static bool DecodePointVar(byte[] p, int pOff, bool negate, PointExt r)
        {
            byte[] py = Arrays.CopyOfRange(p, pOff, pOff + PointBytes);
            if (!CheckPointVar(py))
            {
                return false;
            }

            int x_0 = (py[PointBytes - 1] & 0x80) >> 7;
            py[PointBytes - 1] &= 0x7F;

            X25519Field.Decode(py, 0, r.y);

            int[] u = X25519Field.Create();
            int[] v = X25519Field.Create();

            X25519Field.Sqr(r.y, u);
            X25519Field.Mul(C_d, u, v);
            X25519Field.SubOne(u);
            X25519Field.AddOne(v);

            if (!X25519Field.SqrtRatioVar(u, v, r.x))
            {
                return false;
            }

            X25519Field.Normalize(r.x);
            if (x_0 == 1 && X25519Field.IsZeroVar(r.x))
            {
                return false;
            }

            if (negate ^ (x_0 != (r.x[0] & 1)))
            {
                X25519Field.Negate(r.x, r.x);
            }

            PointExtendXY(r);
            return true;
        }

        private static void DecodeScalar(byte[] k, int kOff, uint[] n)
        {
            Decode32(k, kOff, n, 0, ScalarUints);
        }

        private static void Encode24(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)(n);
            bs[++off] = (byte)(n >> 8);
            bs[++off] = (byte)(n >> 16);
        }

        private static void Encode32(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)(n);
            bs[++off] = (byte)(n >> 8);
            bs[++off] = (byte)(n >> 16);
            bs[++off] = (byte)(n >> 24);
        }

        private static void Encode56(ulong n, byte[] bs, int off)
        {
            Encode32((uint)n, bs, off);
            Encode24((uint)(n >> 32), bs, off + 4);
        }

        private static void EncodePoint(PointAccum p, byte[] r, int rOff)
        {
            int[] x = X25519Field.Create();
            int[] y = X25519Field.Create();

            X25519Field.Inv(p.z, y);
            X25519Field.Mul(p.x, y, x);
            X25519Field.Mul(p.y, y, y);
            X25519Field.Normalize(x);
            X25519Field.Normalize(y);

            X25519Field.Encode(y, r, rOff);
            r[rOff + PointBytes - 1] |= (byte)((x[0] & 1) << 7);
        }

        public static void GeneratePublicKey(byte[] sk, int skOff, byte[] pk, int pkOff)
        {
            Sha512Digest d = new Sha512Digest();
            byte[] h = new byte[d.GetDigestSize()];

            d.BlockUpdate(sk, skOff, SecretKeySize);
            d.DoFinal(h, 0);

            byte[] s = new byte[ScalarBytes];
            PruneScalar(h, 0, s);

            ScalarMultBaseEncoded(s, pk, pkOff);
        }

        private static sbyte[] GetWnaf(uint[] n, int width)
        {
            Debug.Assert(n[ScalarUints - 1] >> 31 == 0);

            uint[] t = new uint[ScalarUints * 2];
            {
                uint c = 0;
                int tPos = t.Length, i = ScalarUints;
                while (--i >= 0)
                {
                    uint next = n[i];
                    t[--tPos] = (next >> 16) | (c << 16);
                    t[--tPos] = c = next;
                }
            }

            sbyte[] ws = new sbyte[256];

            uint pow2 = 1U << width;
            uint mask = pow2 - 1U;
            uint sign = pow2 >> 1;

            uint carry = 0U;
            int j = 0;
            for (int i = 0; i < t.Length; ++i, j -= 16)
            {
                uint word = t[i];
                while (j < 16)
                {
                    uint word16 = word >> j;
                    uint bit = word16 & 1U;

                    if (bit == carry)
                    {
                        ++j;
                        continue;
                    }

                    uint digit = (word16 & mask) + carry;
                    carry = digit & sign;
                    digit -= (carry << 1);
                    carry >>= (width - 1);

                    ws[(i << 4) + j] = (sbyte)digit;

                    j += width;
                }
            }

            Debug.Assert(carry == 0);

            return ws;
        }

        private static void ImplSign(Sha512Digest d, byte[] h, byte[] s, byte[] pk, int pkOff, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
        {
            d.BlockUpdate(h, ScalarBytes, ScalarBytes);
            d.BlockUpdate(m, mOff, mLen);
            d.DoFinal(h, 0);

            byte[] r = ReduceScalar(h);
            byte[] R = new byte[PointBytes];
            ScalarMultBaseEncoded(r, R, 0);

            d.BlockUpdate(R, 0, PointBytes);
            d.BlockUpdate(pk, 0, PointBytes);
            d.BlockUpdate(m, mOff, mLen);
            d.DoFinal(h, 0);

            byte[] k = ReduceScalar(h);
            byte[] S = CalculateS(r, k, s);

            Array.Copy(R, 0, sig, sigOff, PointBytes);
            Array.Copy(S, 0, sig, sigOff + PointBytes, ScalarBytes);
        }

        private static void PointAddVar(bool negate, PointExt p, PointAccum r)
        {
            int[] A = X25519Field.Create();
            int[] B = X25519Field.Create();
            int[] C = X25519Field.Create();
            int[] D = X25519Field.Create();
            int[] E = r.u;
            int[] F = X25519Field.Create();
            int[] G = X25519Field.Create();
            int[] H = r.v;

            int[] c, d, f, g;
            if (negate)
            {
                c = D; d = C; f = G; g = F;
            }
            else
            {
                c = C; d = D; f = F; g = G;
            }

            X25519Field.Apm(r.y, r.x, B, A);
            X25519Field.Apm(p.y, p.x, d, c);
            X25519Field.Mul(A, C, A);
            X25519Field.Mul(B, D, B);
            X25519Field.Mul(r.u, r.v, C);
            X25519Field.Mul(C, p.t, C);
            X25519Field.Mul(C, C_d2, C);
            X25519Field.Mul(r.z, p.z, D);
            X25519Field.Add(D, D, D);
            X25519Field.Apm(B, A, H, E);
            X25519Field.Apm(D, C, g, f);
            X25519Field.Carry(g);
            X25519Field.Mul(E, F, r.x);
            X25519Field.Mul(G, H, r.y);
            X25519Field.Mul(F, G, r.z);
        }

        private static void PointAddVar(bool negate, PointExt p, PointExt q, PointExt r)
        {
            int[] A = X25519Field.Create();
            int[] B = X25519Field.Create();
            int[] C = X25519Field.Create();
            int[] D = X25519Field.Create();
            int[] E = X25519Field.Create();
            int[] F = X25519Field.Create();
            int[] G = X25519Field.Create();
            int[] H = X25519Field.Create();

            int[] c, d, f, g;
            if (negate)
            {
                c = D; d = C; f = G; g = F;
            }
            else
            {
                c = C; d = D; f = F; g = G;
            }

            X25519Field.Apm(p.y, p.x, B, A);
            X25519Field.Apm(q.y, q.x, d, c);
            X25519Field.Mul(A, C, A);
            X25519Field.Mul(B, D, B);
            X25519Field.Mul(p.t, q.t, C);
            X25519Field.Mul(C, C_d2, C);
            X25519Field.Mul(p.z, q.z, D);
            X25519Field.Add(D, D, D);
            X25519Field.Apm(B, A, H, E);
            X25519Field.Apm(D, C, g, f);
            X25519Field.Carry(g);
            X25519Field.Mul(E, F, r.x);
            X25519Field.Mul(G, H, r.y);
            X25519Field.Mul(F, G, r.z);
            X25519Field.Mul(E, H, r.t);
        }

        private static void PointAddPrecomp(PointPrecomp p, PointAccum r)
        {
            int[] A = X25519Field.Create();
            int[] B = X25519Field.Create();
            int[] C = X25519Field.Create();
            int[] E = r.u;
            int[] F = X25519Field.Create();
            int[] G = X25519Field.Create();
            int[] H = r.v;

            X25519Field.Apm(r.y, r.x, B, A);
            X25519Field.Mul(A, p.ymx_h, A);
            X25519Field.Mul(B, p.ypx_h, B);
            X25519Field.Mul(r.u, r.v, C);
            X25519Field.Mul(C, p.xyd, C);
            X25519Field.Apm(B, A, H, E);
            X25519Field.Apm(r.z, C, G, F);
            X25519Field.Carry(G);
            X25519Field.Mul(E, F, r.x);
            X25519Field.Mul(G, H, r.y);
            X25519Field.Mul(F, G, r.z);
        }

        private static PointExt PointCopy(PointAccum p)
        {
            PointExt r = new PointExt();
            X25519Field.Copy(p.x, 0, r.x, 0);
            X25519Field.Copy(p.y, 0, r.y, 0);
            X25519Field.Copy(p.z, 0, r.z, 0);
            X25519Field.Mul(p.u, p.v, r.t);
            return r;
        }

        private static PointExt PointCopy(PointExt p)
        {
            PointExt r = new PointExt();
            X25519Field.Copy(p.x, 0, r.x, 0);
            X25519Field.Copy(p.y, 0, r.y, 0);
            X25519Field.Copy(p.z, 0, r.z, 0);
            X25519Field.Copy(p.t, 0, r.t, 0);
            return r;
        }

        private static void PointDouble(PointAccum r)
        {
            int[] A = X25519Field.Create();
            int[] B = X25519Field.Create();
            int[] C = X25519Field.Create();
            int[] E = r.u;
            int[] F = X25519Field.Create();
            int[] G = X25519Field.Create();
            int[] H = r.v;

            X25519Field.Sqr(r.x, A);
            X25519Field.Sqr(r.y, B);
            X25519Field.Sqr(r.z, C);
            X25519Field.Add(C, C, C);
            X25519Field.Apm(A, B, H, G);
            X25519Field.Add(r.x, r.y, E);
            X25519Field.Sqr(E, E);
            X25519Field.Sub(H, E, E);
            X25519Field.Add(C, G, F);
            X25519Field.Carry(F);
            X25519Field.Mul(E, F, r.x);
            X25519Field.Mul(G, H, r.y);
            X25519Field.Mul(F, G, r.z);
        }

        private static void PointExtendXY(PointAccum p)
        {
            X25519Field.One(p.z);
            X25519Field.Copy(p.x, 0, p.u, 0);
            X25519Field.Copy(p.y, 0, p.v, 0);
        }

        private static void PointExtendXY(PointExt p)
        {
            X25519Field.One(p.z);
            X25519Field.Mul(p.x, p.y, p.t);
        }

        private static void PointLookup(int block, int index, PointPrecomp p)
        {
            Debug.Assert(0 <= block && block < PrecompBlocks);
            Debug.Assert(0 <= index && index < PrecompPoints);

            int off = block * PrecompPoints * 3 * X25519Field.Size;

            for (int i = 0; i < PrecompPoints; ++i)
            {
                int mask = ((i ^ index) - 1) >> 31;
                Nat.CMov(X25519Field.Size, mask, precompBase, off, p.ypx_h, 0); off += X25519Field.Size;
                Nat.CMov(X25519Field.Size, mask, precompBase, off, p.ymx_h, 0); off += X25519Field.Size;
                Nat.CMov(X25519Field.Size, mask, precompBase, off, p.xyd, 0);   off += X25519Field.Size;
            }
        }

        private static PointExt[] PointPrecompVar(PointExt p, int count)
        {
            Debug.Assert(count > 0);

            PointExt d = new PointExt();
            PointAddVar(false, p, p, d);

            PointExt[] table = new PointExt[count];
            table[0] = PointCopy(p);
            for (int i = 1; i < count; ++i)
            {
                PointAddVar(false, table[i - 1], d, table[i] = new PointExt());
            }
            return table;
        }

        private static void PointSetNeutral(PointAccum p)
        {
            X25519Field.Zero(p.x);
            X25519Field.One(p.y);
            X25519Field.One(p.z);
            X25519Field.Zero(p.u);
            X25519Field.One(p.v);
        }

        private static void PointSetNeutral(PointExt p)
        {
            X25519Field.Zero(p.x);
            X25519Field.One(p.y);
            X25519Field.One(p.z);
            X25519Field.Zero(p.t);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Precompute()
        {
            if (precompBase != null)
            {
                return;
            }

            // Precomputed table for the base point in verification ladder
            {
                PointExt b = new PointExt();
                X25519Field.Copy(B_x, 0, b.x, 0);
                X25519Field.Copy(B_y, 0, b.y, 0);
                PointExtendXY(b);

                precompBaseTable = PointPrecompVar(b, 1 << (WnafWidthBase - 2));
            }

            PointAccum p = new PointAccum();
            X25519Field.Copy(B_x, 0, p.x, 0);
            X25519Field.Copy(B_y, 0, p.y, 0);
            PointExtendXY(p);

            precompBase = new int[PrecompBlocks * PrecompPoints * 3 * X25519Field.Size];

            int off = 0;
            for (int b = 0; b < PrecompBlocks; ++b)
            {
                PointExt[] ds = new PointExt[PrecompTeeth];

                PointExt sum = new PointExt();
                PointSetNeutral(sum);

                for (int t = 0; t < PrecompTeeth; ++t)
                {
                    PointExt q = PointCopy(p);
                    PointAddVar(true, sum, q, sum);
                    PointDouble(p);

                    ds[t] = PointCopy(p);

                    for (int s = 1; s < PrecompSpacing; ++s)
                    {
                        PointDouble(p);
                    }
                }

                PointExt[] points = new PointExt[PrecompPoints];
                int k = 0;
                points[k++] = sum;

                for (int t = 0; t < (PrecompTeeth - 1); ++t)
                {
                    int size = 1 << t;
                    for (int j = 0; j < size; ++j, ++k)
                    {
                        PointAddVar(false, points[k - size], ds[t], points[k] = new PointExt());
                    }
                }

                Debug.Assert(k == PrecompPoints);

                for (int i = 0; i < PrecompPoints; ++i)
                {
                    PointExt q = points[i];

                    int[] x = X25519Field.Create();
                    int[] y = X25519Field.Create();

                    X25519Field.Add(q.z, q.z, x);
                    // TODO[ed25519] Batch inversion
                    X25519Field.Inv(x, y);
                    X25519Field.Mul(q.x, y, x);
                    X25519Field.Mul(q.y, y, y);

                    PointPrecomp r = new PointPrecomp();
                    X25519Field.Apm(y, x, r.ypx_h, r.ymx_h);
                    X25519Field.Mul(x, y, r.xyd);
                    X25519Field.Mul(r.xyd, C_d4, r.xyd);

                    X25519Field.Normalize(r.ypx_h);
                    X25519Field.Normalize(r.ymx_h);
                    //X25519Field.Normalize(r.xyd);

                    X25519Field.Copy(r.ypx_h, 0, precompBase, off); off += X25519Field.Size;
                    X25519Field.Copy(r.ymx_h, 0, precompBase, off); off += X25519Field.Size;
                    X25519Field.Copy(r.xyd, 0, precompBase, off);   off += X25519Field.Size;
                }
            }

            Debug.Assert(off == precompBase.Length);
        }

        private static void PruneScalar(byte[] n, int nOff, byte[] r)
        {
            Array.Copy(n, nOff, r, 0, ScalarBytes);

            r[0] &= 0xF8;
            r[ScalarBytes - 1] &= 0x7F;
            r[ScalarBytes - 1] |= 0x40;
        }

        private static byte[] ReduceScalar(byte[] n)
        {
            long x00 = Decode32(n,  0) & M32L;          // x00:32/--
            long x01 = (Decode24(n, 4) << 4) & M32L;    // x01:28/--
            long x02 = Decode32(n,  7) & M32L;          // x02:32/--
            long x03 = (Decode24(n, 11) << 4) & M32L;   // x03:28/--
            long x04 = Decode32(n, 14) & M32L;          // x04:32/--
            long x05 = (Decode24(n, 18) << 4) & M32L;   // x05:28/--
            long x06 = Decode32(n, 21) & M32L;          // x06:32/--
            long x07 = (Decode24(n, 25) << 4) & M32L;   // x07:28/--
            long x08 = Decode32(n, 28) & M32L;          // x08:32/--
            long x09 = (Decode24(n, 32) << 4) & M32L;   // x09:28/--
            long x10 = Decode32(n, 35) & M32L;          // x10:32/--
            long x11 = (Decode24(n, 39) << 4) & M32L;   // x11:28/--
            long x12 = Decode32(n, 42) & M32L;          // x12:32/--
            long x13 = (Decode24(n, 46) << 4) & M32L;   // x13:28/--
            long x14 = Decode32(n, 49) & M32L;          // x14:32/--
            long x15 = (Decode24(n, 53) << 4) & M32L;   // x15:28/--
            long x16 = Decode32(n, 56) & M32L;          // x16:32/--
            long x17 = (Decode24(n, 60) << 4) & M32L;   // x17:28/--
            long x18 = n[63] & 0xFFL;                   // x18:08/--
            long t;

            //x18 += (x17 >> 28); x17 &= M28L;
            x09 -= x18 * L0;                            // x09:34/28
            x10 -= x18 * L1;                            // x10:33/30
            x11 -= x18 * L2;                            // x11:35/28
            x12 -= x18 * L3;                            // x12:32/31
            x13 -= x18 * L4;                            // x13:28/21

            x17 += (x16 >> 28); x16 &= M28L;            // x17:28/--, x16:28/--
            x08 -= x17 * L0;                            // x08:54/32
            x09 -= x17 * L1;                            // x09:52/51
            x10 -= x17 * L2;                            // x10:55/34
            x11 -= x17 * L3;                            // x11:51/36
            x12 -= x17 * L4;                            // x12:41/--

            //x16 += (x15 >> 28); x15 &= M28L;
            x07 -= x16 * L0;                            // x07:54/28
            x08 -= x16 * L1;                            // x08:54/53
            x09 -= x16 * L2;                            // x09:55/53
            x10 -= x16 * L3;                            // x10:55/52
            x11 -= x16 * L4;                            // x11:51/41

            x15 += (x14 >> 28); x14 &= M28L;            // x15:28/--, x14:28/--
            x06 -= x15 * L0;                            // x06:54/32
            x07 -= x15 * L1;                            // x07:54/53
            x08 -= x15 * L2;                            // x08:56/--
            x09 -= x15 * L3;                            // x09:55/54
            x10 -= x15 * L4;                            // x10:55/53

            //x14 += (x13 >> 28); x13 &= M28L;
            x05 -= x14 * L0;                            // x05:54/28
            x06 -= x14 * L1;                            // x06:54/53
            x07 -= x14 * L2;                            // x07:56/--
            x08 -= x14 * L3;                            // x08:56/51
            x09 -= x14 * L4;                            // x09:56/--

            x13 += (x12 >> 28); x12 &= M28L;            // x13:28/22, x12:28/--
            x04 -= x13 * L0;                            // x04:54/49
            x05 -= x13 * L1;                            // x05:54/53
            x06 -= x13 * L2;                            // x06:56/--
            x07 -= x13 * L3;                            // x07:56/52
            x08 -= x13 * L4;                            // x08:56/52

            x12 += (x11 >> 28); x11 &= M28L;            // x12:28/24, x11:28/--
            x03 -= x12 * L0;                            // x03:54/49
            x04 -= x12 * L1;                            // x04:54/51
            x05 -= x12 * L2;                            // x05:56/--
            x06 -= x12 * L3;                            // x06:56/52
            x07 -= x12 * L4;                            // x07:56/53

            x11 += (x10 >> 28); x10 &= M28L;            // x11:29/--, x10:28/--
            x02 -= x11 * L0;                            // x02:55/32
            x03 -= x11 * L1;                            // x03:55/--
            x04 -= x11 * L2;                            // x04:56/55
            x05 -= x11 * L3;                            // x05:56/52
            x06 -= x11 * L4;                            // x06:56/53

            x10 += (x09 >> 28); x09 &= M28L;            // x10:29/--, x09:28/--
            x01 -= x10 * L0;                            // x01:55/28
            x02 -= x10 * L1;                            // x02:55/54
            x03 -= x10 * L2;                            // x03:56/55
            x04 -= x10 * L3;                            // x04:57/--
            x05 -= x10 * L4;                            // x05:56/53

            x08 += (x07 >> 28); x07 &= M28L;            // x08:56/53, x07:28/--
            x09 += (x08 >> 28); x08 &= M28L;            // x09:29/25, x08:28/--

            t    = (x08 >> 27) & 1L;
            x09 += t;                                   // x09:29/26

            x00 -= x09 * L0;                            // x00:55/53
            x01 -= x09 * L1;                            // x01:55/54
            x02 -= x09 * L2;                            // x02:57/--
            x03 -= x09 * L3;                            // x03:57/--
            x04 -= x09 * L4;                            // x04:57/42

            x01 += (x00 >> 28); x00 &= M28L;
            x02 += (x01 >> 28); x01 &= M28L;
            x03 += (x02 >> 28); x02 &= M28L;
            x04 += (x03 >> 28); x03 &= M28L;
            x05 += (x04 >> 28); x04 &= M28L;
            x06 += (x05 >> 28); x05 &= M28L;
            x07 += (x06 >> 28); x06 &= M28L;
            x08 += (x07 >> 28); x07 &= M28L;
            x09  = (x08 >> 28); x08 &= M28L;

            x09 -= t;

            Debug.Assert(x09 == 0L || x09 == -1L);

            x00 += x09 & L0;
            x01 += x09 & L1;
            x02 += x09 & L2;
            x03 += x09 & L3;
            x04 += x09 & L4;

            x01 += (x00 >> 28); x00 &= M28L;
            x02 += (x01 >> 28); x01 &= M28L;
            x03 += (x02 >> 28); x02 &= M28L;
            x04 += (x03 >> 28); x03 &= M28L;
            x05 += (x04 >> 28); x04 &= M28L;
            x06 += (x05 >> 28); x05 &= M28L;
            x07 += (x06 >> 28); x06 &= M28L;
            x08 += (x07 >> 28); x07 &= M28L;

            byte[] r = new byte[ScalarBytes];
            Encode56((ulong)(x00 | (x01 << 28)), r, 0);
            Encode56((ulong)(x02 | (x03 << 28)), r, 7);
            Encode56((ulong)(x04 | (x05 << 28)), r, 14);
            Encode56((ulong)(x06 | (x07 << 28)), r, 21);
            Encode32((uint)x08, r, 28);
            return r;
        }

        private static void ScalarMultBase(byte[] k, PointAccum r)
        {
            Precompute();

            PointSetNeutral(r);

            uint[] n = new uint[ScalarUints];
            DecodeScalar(k, 0, n);

            // Recode the scalar into signed-digit form, then group comb bits in each block
            {
                uint c1 = Nat.CAdd(ScalarUints, ~(int)n[0] & 1, n, L, n);   Debug.Assert(c1 == 0);
                uint c2 = Nat.ShiftDownBit(ScalarUints, n, 1U);             Debug.Assert(c2 == (1U << 31));

                for (int i = 0; i < ScalarUints; ++i)
                {
                    n[i] = Interleave.Shuffle2(n[i]);
                }
            }

            PointPrecomp p = new PointPrecomp();

            int cOff = (PrecompSpacing - 1) * PrecompTeeth;
            for (; ; )
            {
                for (int b = 0; b < PrecompBlocks; ++b)
                {
                    uint w = n[b] >> cOff;
                    int sign = (int)(w >> (PrecompTeeth - 1)) & 1;
                    int abs = ((int)w ^ -sign) & PrecompMask;

                    Debug.Assert(sign == 0 || sign == 1);
                    Debug.Assert(0 <= abs && abs < PrecompPoints);

                    PointLookup(b, abs, p);

                    X25519Field.CSwap(sign, p.ypx_h, p.ymx_h);
                    X25519Field.CNegate(sign, p.xyd);

                    PointAddPrecomp(p, r);
                }

                if ((cOff -= PrecompTeeth) < 0)
                {
                    break;
                }

                PointDouble(r);
            }
        }

        private static void ScalarMultBaseEncoded(byte[] k, byte[] r, int rOff)
        {
            PointAccum p = new PointAccum();
            ScalarMultBase(k, p);
            EncodePoint(p, r, rOff);
        }

        private static void ScalarMultStraussVar(uint[] nb, uint[] np, PointExt p, PointAccum r)
        {
            Precompute();

            int width = 5;

            sbyte[] ws_b = GetWnaf(nb, WnafWidthBase);
            sbyte[] ws_p = GetWnaf(np, width);

            PointExt[] tp = PointPrecompVar(p, 1 << (width - 2));

            PointSetNeutral(r);

            int bit = 255;
            while (bit > 0 && (ws_b[bit] | ws_p[bit]) == 0)
            {
                --bit;
            }

            for (; ; )
            {
                int wb = ws_b[bit];
                if (wb != 0)
                {
                    int sign = wb >> 31;
                    int index = (wb ^ sign) >> 1;

                    PointAddVar((sign != 0), precompBaseTable[index], r);
                }

                int wp = ws_p[bit];
                if (wp != 0)
                {
                    int sign = wp >> 31;
                    int index = (wp ^ sign) >> 1;

                    PointAddVar((sign != 0), tp[index], r);
                }

                if (--bit < 0)
                {
                    break;
                }

                PointDouble(r);
            }
        }

        public static void Sign(byte[] sk, int skOff, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
        {
            Sha512Digest d = new Sha512Digest();
            byte[] h = new byte[d.GetDigestSize()];

            d.BlockUpdate(sk, skOff, SecretKeySize);
            d.DoFinal(h, 0);

            byte[] s = new byte[ScalarBytes];
            PruneScalar(h, 0, s);

            byte[] pk = new byte[PointBytes];
            ScalarMultBaseEncoded(s, pk, 0);

            ImplSign(d, h, s, pk, 0, m, mOff, mLen, sig, sigOff);
        }

        public static void Sign(byte[] sk, int skOff, byte[] pk, int pkOff, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
        {
            Sha512Digest d = new Sha512Digest();
            byte[] h = new byte[d.GetDigestSize()];

            d.BlockUpdate(sk, skOff, SecretKeySize);
            d.DoFinal(h, 0);

            byte[] s = new byte[ScalarBytes];
            PruneScalar(h, 0, s);

            ImplSign(d, h, s, pk, pkOff, m, mOff, mLen, sig, sigOff);
        }

        public static bool Verify(byte[] sig, int sigOff, byte[] pk, int pkOff, byte[] m, int mOff, int mLen)
        {
            byte[] R = Arrays.CopyOfRange(sig, sigOff, sigOff + PointBytes);
            byte[] S = Arrays.CopyOfRange(sig, sigOff + PointBytes, sigOff + SignatureSize);

            if (!CheckPointVar(R))
            {
                return false;
            }
            if (!CheckScalarVar(S))
            {
                return false;
            }

            PointExt pA = new PointExt();
            if (!DecodePointVar(pk, pkOff, true, pA))
            {
                return false;
            }

            Sha512Digest d = new Sha512Digest();
            byte[] h = new byte[d.GetDigestSize()];

            d.BlockUpdate(R, 0, PointBytes);
            d.BlockUpdate(pk, pkOff, PointBytes);
            d.BlockUpdate(m, mOff, mLen);
            d.DoFinal(h, 0);

            byte[] k = ReduceScalar(h);

            uint[] nS = new uint[ScalarUints];
            DecodeScalar(S, 0, nS);

            uint[] nA = new uint[ScalarUints];
            DecodeScalar(k, 0, nA);

            PointAccum pR = new PointAccum();
            ScalarMultStraussVar(nS, nA, pA, pR);

            byte[] check = new byte[PointBytes];
            EncodePoint(pR, check, 0);

            return Arrays.AreEqual(check, R);
        }
    }
}
