using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Math.EC.Rfc8032
{
    public abstract class Ed448
    {
        private const ulong M26UL = 0x03FFFFFFUL;
        private const ulong M28UL = 0x0FFFFFFFUL;

        private const int PointBytes = 57;
        private const int ScalarUints = 14;
        private const int ScalarBytes = ScalarUints * 4 + 1;

        public static readonly int PublicKeySize = PointBytes;
        public static readonly int SecretKeySize = 57;
        public static readonly int SignatureSize = PointBytes + ScalarBytes;

        private static readonly byte[] Dom4Prefix = Strings.ToByteArray("SigEd448");

        private static readonly uint[] P = { 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU,
            0xFFFFFFFEU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU };
        private static readonly uint[] L = { 0xAB5844F3U, 0x2378C292U, 0x8DC58F55U, 0x216CC272U, 0xAED63690U, 0xC44EDB49U, 0x7CCA23E9U,
            0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0xFFFFFFFFU, 0x3FFFFFFFU };
        private static readonly BigInteger N = Nat.ToBigInteger(L.Length, L);

        private const int L_0 = 0x04A7BB0D;     // L_0:26/24
        private const int L_1 = 0x0873D6D5;     // L_1:27/23
        private const int L_2 = 0x0A70AADC;     // L_2:27/26
        private const int L_3 = 0x03D8D723;     // L_3:26/--
        private const int L_4 = 0x096FDE93;     // L_4:27/25
        private const int L_5 = 0x0B65129C;     // L_5:27/26
        private const int L_6 = 0x063BB124;     // L_6:27/--
        private const int L_7 = 0x08335DC1;     // L_7:27/22

        private const int L4_0 = 0x029EEC34;    // L4_0:25/24
        private const int L4_1 = 0x01CF5B55;    // L4_1:25/--
        private const int L4_2 = 0x09C2AB72;    // L4_2:27/25
        private const int L4_3 = 0x0F635C8E;    // L4_3:28/--
        private const int L4_4 = 0x05BF7A4C;    // L4_4:26/25
        private const int L4_5 = 0x0D944A72;    // L4_5:28/--
        private const int L4_6 = 0x08EEC492;    // L4_6:27/24
        private const int L4_7 = 0x20CD7705;    // L4_7:29/24

        private static readonly uint[] B_x = { 0x070CC05EU, 0x026A82BCU, 0x00938E26U, 0x080E18B0U, 0x0511433BU, 0x0F72AB66U, 0x0412AE1AU,
            0x0A3D3A46U, 0x0A6DE324U, 0x00F1767EU, 0x04657047U, 0x036DA9E1U, 0x05A622BFU, 0x0ED221D1U, 0x066BED0DU, 0x04F1970CU };
        private static readonly uint[] B_y = { 0x0230FA14U, 0x008795BFU, 0x07C8AD98U, 0x0132C4EDU, 0x09C4FDBDU, 0x01CE67C3U, 0x073AD3FFU,
            0x005A0C2DU, 0x07789C1EU, 0x0A398408U, 0x0A73736CU, 0x0C7624BEU, 0x003756C9U, 0x02488762U, 0x016EB6BCU, 0x0693F467U };
        private const int C_d = -39081;

        private const int WnafWidthBase = 7;

        private const int PrecompBlocks = 5;
        private const int PrecompTeeth = 5;
        private const int PrecompSpacing = 18;
        private const int PrecompPoints = 1 << (PrecompTeeth - 1);
        private const int PrecompMask = PrecompPoints - 1;

        // TODO[ed448] Convert to PointPrecomp
        private static PointExt[] precompBaseTable = null;
        private static uint[] precompBase = null;

        private class PointExt
        {
            internal uint[] x = X448Field.Create();
            internal uint[] y = X448Field.Create();
            internal uint[] z = X448Field.Create();
        }

        private class PointPrecomp
        {
            internal uint[] x = X448Field.Create();
            internal uint[] y = X448Field.Create();
        }

        private static byte[] CalculateS(byte[] r, byte[] k, byte[] s)
        {
            uint[] t = new uint[ScalarUints * 2];   DecodeScalar(r, 0, t);
            uint[] u = new uint[ScalarUints];       DecodeScalar(k, 0, u);
            uint[] v = new uint[ScalarUints];       DecodeScalar(s, 0, v);

            Nat.MulAddTo(14, u, v, t);

            byte[] result = new byte[ScalarBytes * 2];
            for (int i = 0; i < t.Length; ++i)
            {
                Encode32(t[i], result, i * 4);
            }
            return ReduceScalar(result);
        }

        private static bool CheckContextVar(byte[] ctx)
        {
            return ctx != null && ctx.Length < 256;
        }

        private static bool CheckPointVar(byte[] p)
        {
            if ((p[PointBytes - 1] & 0x7F) != 0x00)
            {
                return false;
            }

            uint[] t = new uint[14];
            Decode32(p, 0, t, 0, 14);
            return !Nat.Gte(14, t, P);
        }

        private static bool CheckScalarVar(byte[] s)
        {
            if (s[ScalarBytes - 1] != 0x00)
            {
                return false;
            }

            uint[] n = new uint[ScalarUints];
            DecodeScalar(s, 0, n);
            return !Nat.Gte(ScalarUints, n, L);
        }

        private static uint Decode16(byte[] bs, int off)
        {
            uint n = bs[off];
            n |= (uint)bs[++off] << 8;
            return n;
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

            X448Field.Decode(py, 0, r.y);

            uint[] u = X448Field.Create();
            uint[] v = X448Field.Create();

            X448Field.Sqr(r.y, u);
            X448Field.Mul(u, (uint)-C_d, v);
            X448Field.Negate(u, u);
            X448Field.AddOne(u);
            X448Field.AddOne(v);

            if (!X448Field.SqrtRatioVar(u, v, r.x))
            {
                return false;
            }

            X448Field.Normalize(r.x);
            if (x_0 == 1 && X448Field.IsZeroVar(r.x))
            {
                return false;
            }

            if (negate ^ (x_0 != (r.x[0] & 1)))
            {
                X448Field.Negate(r.x, r.x);
            }

            PointExtendXY(r);
            return true;
        }

        private static void DecodeScalar(byte[] k, int kOff, uint[] n)
        {
            Debug.Assert(k[kOff + ScalarBytes - 1] == 0x00);

            Decode32(k, kOff, n, 0, ScalarUints);
        }

        private static void Dom4(ShakeDigest d, byte x, byte[] y)
        {
            d.BlockUpdate(Dom4Prefix, 0, Dom4Prefix.Length);
            d.Update(x);
            d.Update((byte)y.Length);
            d.BlockUpdate(y, 0, y.Length);
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

        private static void EncodePoint(PointExt p, byte[] r, int rOff)
        {
            uint[] x = X448Field.Create();
            uint[] y = X448Field.Create();

            X448Field.Inv(p.z, y);
            X448Field.Mul(p.x, y, x);
            X448Field.Mul(p.y, y, y);
            X448Field.Normalize(x);
            X448Field.Normalize(y);

            X448Field.Encode(y, r, rOff);
            r[rOff + PointBytes - 1] = (byte)((x[0] & 1) << 7);
        }

        public static void GeneratePublicKey(byte[] sk, int skOff, byte[] pk, int pkOff)
        {
            ShakeDigest d = new ShakeDigest(256);
            byte[] h = new byte[ScalarBytes * 2];

            d.BlockUpdate(sk, skOff, SecretKeySize);
            d.DoFinal(h, 0, h.Length);

            byte[] s = new byte[ScalarBytes];
            PruneScalar(h, 0, s);

            ScalarMultBaseEncoded(s, pk, pkOff);
        }

        private static sbyte[] GetWnaf(uint[] n, int width)
        {
            Debug.Assert(n[ScalarUints - 1] >> 31 == 0U);

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

            sbyte[] ws = new sbyte[448];

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

        private static void ImplSign(ShakeDigest d, byte[] h, byte[] s, byte[] pk, int pkOff, byte[] ctx, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
        {
            byte phflag = 0x00;

            Dom4(d, phflag, ctx);
            d.BlockUpdate(h, ScalarBytes, ScalarBytes);
            d.BlockUpdate(m, mOff, mLen);
            d.DoFinal(h, 0, h.Length);

            byte[] r = ReduceScalar(h);
            byte[] R = new byte[PointBytes];
            ScalarMultBaseEncoded(r, R, 0);

            Dom4(d, phflag, ctx);
            d.BlockUpdate(R, 0, PointBytes);
            d.BlockUpdate(pk, pkOff, PointBytes);
            d.BlockUpdate(m, mOff, mLen);
            d.DoFinal(h, 0, h.Length);

            byte[] k = ReduceScalar(h);
            byte[] S = CalculateS(r, k, s);

            Array.Copy(R, 0, sig, sigOff, PointBytes);
            Array.Copy(S, 0, sig, sigOff + PointBytes, ScalarBytes);
        }

        private static void PointAddVar(bool negate, PointExt p, PointExt r)
        {
            uint[] A = X448Field.Create();
            uint[] B = X448Field.Create();
            uint[] C = X448Field.Create();
            uint[] D = X448Field.Create();
            uint[] E = X448Field.Create();
            uint[] F = X448Field.Create();
            uint[] G = X448Field.Create();
            uint[] H = X448Field.Create();

            uint[] b, e, f, g;
            if (negate)
            {
                b = E; e = B; f = G; g = F;
                X448Field.Sub(p.y, p.x, H);
            }
            else
            {
                b = B; e = E; f = F; g = G;
                X448Field.Add(p.y, p.x, H);
            }

            X448Field.Mul(p.z, r.z, A);
            X448Field.Sqr(A, B);
            X448Field.Mul(p.x, r.x, C);
            X448Field.Mul(p.y, r.y, D);
            X448Field.Mul(C, D, E);
            X448Field.Mul(E, -C_d, E);
    //        X448Field.Apm(B, E, F, G);
            X448Field.Add(B, E, f);
            X448Field.Sub(B, E, g);
            X448Field.Add(r.x, r.y, E);
            X448Field.Mul(H, E, H);
    //        X448Field.Apm(D, C, B, E);
            X448Field.Add(D, C, b);
            X448Field.Sub(D, C, e);
            X448Field.Carry(b);
            X448Field.Sub(H, B, H);
            X448Field.Mul(H, A, H);
            X448Field.Mul(E, A, E);
            X448Field.Mul(F, H, r.x);
            X448Field.Mul(E, G, r.y);
            X448Field.Mul(F, G, r.z);
        }

        private static void PointAddPrecomp(PointPrecomp p, PointExt r)
        {
            uint[] B = X448Field.Create();
            uint[] C = X448Field.Create();
            uint[] D = X448Field.Create();
            uint[] E = X448Field.Create();
            uint[] F = X448Field.Create();
            uint[] G = X448Field.Create();
            uint[] H = X448Field.Create();

            X448Field.Sqr(r.z, B);
            X448Field.Mul(p.x, r.x, C);
            X448Field.Mul(p.y, r.y, D);
            X448Field.Mul(C, D, E);
            X448Field.Mul(E, -C_d, E);
    //        X448Field.Apm(B, E, F, G);
            X448Field.Add(B, E, F);
            X448Field.Sub(B, E, G);
            X448Field.Add(p.x, p.y, B);
            X448Field.Add(r.x, r.y, E);
            X448Field.Mul(B, E, H);
    //        X448Field.Apm(D, C, B, E);
            X448Field.Add(D, C, B);
            X448Field.Sub(D, C, E);
            X448Field.Carry(B);
            X448Field.Sub(H, B, H);
            X448Field.Mul(H, r.z, H);
            X448Field.Mul(E, r.z, E);
            X448Field.Mul(F, H, r.x);
            X448Field.Mul(E, G, r.y);
            X448Field.Mul(F, G, r.z);
        }

        private static PointExt PointCopy(PointExt p)
        {
            PointExt r = new PointExt();
            X448Field.Copy(p.x, 0, r.x, 0);
            X448Field.Copy(p.y, 0, r.y, 0);
            X448Field.Copy(p.z, 0, r.z, 0);
            return r;
        }

        private static void PointDouble(PointExt r)
        {
            uint[] B = X448Field.Create();
            uint[] C = X448Field.Create();
            uint[] D = X448Field.Create();
            uint[] E = X448Field.Create();
            uint[] H = X448Field.Create();
            uint[] J = X448Field.Create();

            X448Field.Add(r.x, r.y, B);
            X448Field.Sqr(B, B);
            X448Field.Sqr(r.x, C);
            X448Field.Sqr(r.y, D);
            X448Field.Add(C, D, E);
            X448Field.Carry(E);
            X448Field.Sqr(r.z, H);
            X448Field.Add(H, H, H);
            X448Field.Carry(H);
            X448Field.Sub(E, H, J);
            X448Field.Sub(B, E, B);
            X448Field.Sub(C, D, C);
            X448Field.Mul(B, J, r.x);
            X448Field.Mul(E, C, r.y);
            X448Field.Mul(E, J, r.z);
        }

        private static void PointExtendXY(PointExt p)
        {
            X448Field.One(p.z);
        }

        private static void PointLookup(int block, int index, PointPrecomp p)
        {
            Debug.Assert(0 <= block && block < PrecompBlocks);
            Debug.Assert(0 <= index && index < PrecompPoints);

            int off = block * PrecompPoints * 2 * X448Field.Size;

            for (int i = 0; i < PrecompPoints; ++i)
            {
                int mask = ((i ^ index) - 1) >> 31;
                Nat.CMov(X448Field.Size, mask, precompBase, off, p.x, 0);   off += X448Field.Size;
                Nat.CMov(X448Field.Size, mask, precompBase, off, p.y, 0);   off += X448Field.Size;
            }
        }

        private static PointExt[] PointPrecompVar(PointExt p, int count)
        {
            Debug.Assert(count > 0);

            PointExt d = PointCopy(p);
            PointDouble(d);

            PointExt[] table = new PointExt[count];
            table[0] = PointCopy(p);
            for (int i = 1; i < count; ++i)
            {
                table[i] = PointCopy(table[i - 1]);
                PointAddVar(false, d, table[i]);
            }
            return table;
        }

        private static void PointSetNeutral(PointExt p)
        {
            X448Field.Zero(p.x);
            X448Field.One(p.y);
            X448Field.One(p.z);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Precompute()
        {
            if (precompBase != null)
            {
                return;
            }

            PointExt p = new PointExt();
            X448Field.Copy(B_x, 0, p.x, 0);
            X448Field.Copy(B_y, 0, p.y, 0);
            PointExtendXY(p);

            precompBaseTable = PointPrecompVar(p, 1 << (WnafWidthBase - 2));

            precompBase = new uint[PrecompBlocks * PrecompPoints * 2 * X448Field.Size];

            int off = 0;
            for (int b = 0; b < PrecompBlocks; ++b)
            {
                PointExt[] ds = new PointExt[PrecompTeeth];

                PointExt sum = new PointExt();
                PointSetNeutral(sum);

                for (int t = 0; t < PrecompTeeth; ++t)
                {
                    PointAddVar(true, p, sum);
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
                        points[k] = PointCopy(points[k - size]);
                        PointAddVar(false, ds[t], points[k]);
                    }
                }

                Debug.Assert(k == PrecompPoints);

                for (int i = 0; i < PrecompPoints; ++i)
                {
                    PointExt q = points[i];
                    // TODO[ed448] Batch inversion
                    X448Field.Inv(q.z, q.z);
                    X448Field.Mul(q.x, q.z, q.x);
                    X448Field.Mul(q.y, q.z, q.y);

    //                X448Field.Normalize(q.x);
    //                X448Field.Normalize(q.y);

                    X448Field.Copy(q.x, 0, precompBase, off);   off += X448Field.Size;
                    X448Field.Copy(q.y, 0, precompBase, off);   off += X448Field.Size;
                }
            }

            Debug.Assert(off == precompBase.Length);
        }

        private static void PruneScalar(byte[] n, int nOff, byte[] r)
        {
            Array.Copy(n, nOff, r, 0, ScalarBytes);

            r[0] &= 0xFC;
            r[ScalarBytes - 2] |= 0x80;
            r[ScalarBytes - 1] &= 0x00;
        }

        private static byte[] ReduceScalar(byte[] n)
        {
            ulong x00 =  Decode32(n,   0);          // x00:32/--
            ulong x01 = (Decode24(n,   4) << 4);    // x01:28/--
            ulong x02 =  Decode32(n,   7);          // x02:32/--
            ulong x03 = (Decode24(n,  11) << 4);    // x03:28/--
            ulong x04 =  Decode32(n,  14);          // x04:32/--
            ulong x05 = (Decode24(n,  18) << 4);    // x05:28/--
            ulong x06 =  Decode32(n,  21);          // x06:32/--
            ulong x07 = (Decode24(n,  25) << 4);    // x07:28/--
            ulong x08 =  Decode32(n,  28);          // x08:32/--
            ulong x09 = (Decode24(n,  32) << 4);    // x09:28/--
            ulong x10 =  Decode32(n,  35);          // x10:32/--
            ulong x11 = (Decode24(n,  39) << 4);    // x11:28/--
            ulong x12 =  Decode32(n,  42);          // x12:32/--
            ulong x13 = (Decode24(n,  46) << 4);    // x13:28/--
            ulong x14 =  Decode32(n,  49);          // x14:32/--
            ulong x15 = (Decode24(n,  53) << 4);    // x15:28/--
            ulong x16 =  Decode32(n,  56);          // x16:32/--
            ulong x17 = (Decode24(n,  60) << 4);    // x17:28/--
            ulong x18 =  Decode32(n,  63);          // x18:32/--
            ulong x19 = (Decode24(n,  67) << 4);    // x19:28/--
            ulong x20 =  Decode32(n,  70);          // x20:32/--
            ulong x21 = (Decode24(n,  74) << 4);    // x21:28/--
            ulong x22 =  Decode32(n,  77);          // x22:32/--
            ulong x23 = (Decode24(n,  81) << 4);    // x23:28/--
            ulong x24 =  Decode32(n,  84);          // x24:32/--
            ulong x25 = (Decode24(n,  88) << 4);    // x25:28/--
            ulong x26 =  Decode32(n,  91);          // x26:32/--
            ulong x27 = (Decode24(n,  95) << 4);    // x27:28/--
            ulong x28 =  Decode32(n,  98);          // x28:32/--
            ulong x29 = (Decode24(n, 102) << 4);    // x29:28/--
            ulong x30 =  Decode32(n, 105);          // x30:32/--
            ulong x31 = (Decode24(n, 109) << 4);    // x31:28/--
            ulong x32 =  Decode16(n, 112);          // x32:16/--

    //        x32 += (x31 >> 28); x31 &= M28UL;
            x16 += x32 * L4_0;                          // x16:42/--
            x17 += x32 * L4_1;                          // x17:41/28
            x18 += x32 * L4_2;                          // x18:43/42
            x19 += x32 * L4_3;                          // x19:44/28
            x20 += x32 * L4_4;                          // x20:43/--
            x21 += x32 * L4_5;                          // x21:44/28
            x22 += x32 * L4_6;                          // x22:43/41
            x23 += x32 * L4_7;                          // x23:45/41

            x31 += (x30 >> 28); x30 &= M28UL;           // x31:28/--, x30:28/--
            x15 += x31 * L4_0;                          // x15:54/--
            x16 += x31 * L4_1;                          // x16:53/42
            x17 += x31 * L4_2;                          // x17:55/54
            x18 += x31 * L4_3;                          // x18:56/44
            x19 += x31 * L4_4;                          // x19:55/--
            x20 += x31 * L4_5;                          // x20:56/43
            x21 += x31 * L4_6;                          // x21:55/53
            x22 += x31 * L4_7;                          // x22:57/53

    //        x30 += (x29 >> 28); x29 &= M28UL;
            x14 += x30 * L4_0;                          // x14:54/--
            x15 += x30 * L4_1;                          // x15:54/53
            x16 += x30 * L4_2;                          // x16:56/--
            x17 += x30 * L4_3;                          // x17:57/--
            x18 += x30 * L4_4;                          // x18:56/55
            x19 += x30 * L4_5;                          // x19:56/55
            x20 += x30 * L4_6;                          // x20:57/--
            x21 += x30 * L4_7;                          // x21:57/56

            x29 += (x28 >> 28); x28 &= M28UL;           // x29:28/--, x28:28/--
            x13 += x29 * L4_0;                          // x13:54/--
            x14 += x29 * L4_1;                          // x14:54/53
            x15 += x29 * L4_2;                          // x15:56/--
            x16 += x29 * L4_3;                          // x16:57/--
            x17 += x29 * L4_4;                          // x17:57/55
            x18 += x29 * L4_5;                          // x18:57/55
            x19 += x29 * L4_6;                          // x19:57/52
            x20 += x29 * L4_7;                          // x20:58/52

    //        x28 += (x27 >> 28); x27 &= M28UL;
            x12 += x28 * L4_0;                          // x12:54/--
            x13 += x28 * L4_1;                          // x13:54/53
            x14 += x28 * L4_2;                          // x14:56/--
            x15 += x28 * L4_3;                          // x15:57/--
            x16 += x28 * L4_4;                          // x16:57/55
            x17 += x28 * L4_5;                          // x17:58/--
            x18 += x28 * L4_6;                          // x18:58/--
            x19 += x28 * L4_7;                          // x19:58/53

            x27 += (x26 >> 28); x26 &= M28UL;           // x27:28/--, x26:28/--
            x11 += x27 * L4_0;                          // x11:54/--
            x12 += x27 * L4_1;                          // x12:54/53
            x13 += x27 * L4_2;                          // x13:56/--
            x14 += x27 * L4_3;                          // x14:57/--
            x15 += x27 * L4_4;                          // x15:57/55
            x16 += x27 * L4_5;                          // x16:58/--
            x17 += x27 * L4_6;                          // x17:58/56
            x18 += x27 * L4_7;                          // x18:59/--

    //        x26 += (x25 >> 28); x25 &= M28UL;
            x10 += x26 * L4_0;                          // x10:54/--
            x11 += x26 * L4_1;                          // x11:54/53
            x12 += x26 * L4_2;                          // x12:56/--
            x13 += x26 * L4_3;                          // x13:57/--
            x14 += x26 * L4_4;                          // x14:57/55
            x15 += x26 * L4_5;                          // x15:58/--
            x16 += x26 * L4_6;                          // x16:58/56
            x17 += x26 * L4_7;                          // x17:59/--

            x25 += (x24 >> 28); x24 &= M28UL;           // x25:28/--, x24:28/--
            x09 += x25 * L4_0;                          // x09:54/--
            x10 += x25 * L4_1;                          // x10:54/53
            x11 += x25 * L4_2;                          // x11:56/--
            x12 += x25 * L4_3;                          // x12:57/--
            x13 += x25 * L4_4;                          // x13:57/55
            x14 += x25 * L4_5;                          // x14:58/--
            x15 += x25 * L4_6;                          // x15:58/56
            x16 += x25 * L4_7;                          // x16:59/--

            x21 += (x20 >> 28); x20 &= M28UL;           // x21:58/--, x20:28/--
            x22 += (x21 >> 28); x21 &= M28UL;           // x22:57/54, x21:28/--
            x23 += (x22 >> 28); x22 &= M28UL;           // x23:45/42, x22:28/--
            x24 += (x23 >> 28); x23 &= M28UL;           // x24:28/18, x23:28/--

            x08 += x24 * L4_0;                          // x08:54/--
            x09 += x24 * L4_1;                          // x09:55/--
            x10 += x24 * L4_2;                          // x10:56/46
            x11 += x24 * L4_3;                          // x11:57/46
            x12 += x24 * L4_4;                          // x12:57/55
            x13 += x24 * L4_5;                          // x13:58/--
            x14 += x24 * L4_6;                          // x14:58/56
            x15 += x24 * L4_7;                          // x15:59/--

            x07 += x23 * L4_0;                          // x07:54/--
            x08 += x23 * L4_1;                          // x08:54/53
            x09 += x23 * L4_2;                          // x09:56/53
            x10 += x23 * L4_3;                          // x10:57/46
            x11 += x23 * L4_4;                          // x11:57/55
            x12 += x23 * L4_5;                          // x12:58/--
            x13 += x23 * L4_6;                          // x13:58/56
            x14 += x23 * L4_7;                          // x14:59/--

            x06 += x22 * L4_0;                          // x06:54/--
            x07 += x22 * L4_1;                          // x07:54/53
            x08 += x22 * L4_2;                          // x08:56/--
            x09 += x22 * L4_3;                          // x09:57/53
            x10 += x22 * L4_4;                          // x10:57/55
            x11 += x22 * L4_5;                          // x11:58/--
            x12 += x22 * L4_6;                          // x12:58/56
            x13 += x22 * L4_7;                          // x13:59/--

            x18 += (x17 >> 28); x17 &= M28UL;           // x18:59/31, x17:28/--
            x19 += (x18 >> 28); x18 &= M28UL;           // x19:58/54, x18:28/--
            x20 += (x19 >> 28); x19 &= M28UL;           // x20:30/29, x19:28/--
            x21 += (x20 >> 28); x20 &= M28UL;           // x21:28/03, x20:28/--

            x05 += x21 * L4_0;                          // x05:54/--
            x06 += x21 * L4_1;                          // x06:55/--
            x07 += x21 * L4_2;                          // x07:56/31
            x08 += x21 * L4_3;                          // x08:57/31
            x09 += x21 * L4_4;                          // x09:57/56
            x10 += x21 * L4_5;                          // x10:58/--
            x11 += x21 * L4_6;                          // x11:58/56
            x12 += x21 * L4_7;                          // x12:59/--

            x04 += x20 * L4_0;                          // x04:54/--
            x05 += x20 * L4_1;                          // x05:54/53
            x06 += x20 * L4_2;                          // x06:56/53
            x07 += x20 * L4_3;                          // x07:57/31
            x08 += x20 * L4_4;                          // x08:57/55
            x09 += x20 * L4_5;                          // x09:58/--
            x10 += x20 * L4_6;                          // x10:58/56
            x11 += x20 * L4_7;                          // x11:59/--

            x03 += x19 * L4_0;                          // x03:54/--
            x04 += x19 * L4_1;                          // x04:54/53
            x05 += x19 * L4_2;                          // x05:56/--
            x06 += x19 * L4_3;                          // x06:57/53
            x07 += x19 * L4_4;                          // x07:57/55
            x08 += x19 * L4_5;                          // x08:58/--
            x09 += x19 * L4_6;                          // x09:58/56
            x10 += x19 * L4_7;                          // x10:59/--

            x15 += (x14 >> 28); x14 &= M28UL;           // x15:59/31, x14:28/--
            x16 += (x15 >> 28); x15 &= M28UL;           // x16:59/32, x15:28/--
            x17 += (x16 >> 28); x16 &= M28UL;           // x17:31/29, x16:28/--
            x18 += (x17 >> 28); x17 &= M28UL;           // x18:28/04, x17:28/--

            x02 += x18 * L4_0;                          // x02:54/--
            x03 += x18 * L4_1;                          // x03:55/--
            x04 += x18 * L4_2;                          // x04:56/32
            x05 += x18 * L4_3;                          // x05:57/32
            x06 += x18 * L4_4;                          // x06:57/56
            x07 += x18 * L4_5;                          // x07:58/--
            x08 += x18 * L4_6;                          // x08:58/56
            x09 += x18 * L4_7;                          // x09:59/--

            x01 += x17 * L4_0;                          // x01:54/--
            x02 += x17 * L4_1;                          // x02:54/53
            x03 += x17 * L4_2;                          // x03:56/53
            x04 += x17 * L4_3;                          // x04:57/32
            x05 += x17 * L4_4;                          // x05:57/55
            x06 += x17 * L4_5;                          // x06:58/--
            x07 += x17 * L4_6;                          // x07:58/56
            x08 += x17 * L4_7;                          // x08:59/--

            x16 *= 4;
            x16 += (x15 >> 26); x15 &= M26UL;
            x16 += 1;                                   // x16:30/01

            x00 += x16 * L_0;
            x01 += x16 * L_1;
            x02 += x16 * L_2;
            x03 += x16 * L_3;
            x04 += x16 * L_4;
            x05 += x16 * L_5;
            x06 += x16 * L_6;
            x07 += x16 * L_7;

            x01 += (x00 >> 28); x00 &= M28UL;
            x02 += (x01 >> 28); x01 &= M28UL;
            x03 += (x02 >> 28); x02 &= M28UL;
            x04 += (x03 >> 28); x03 &= M28UL;
            x05 += (x04 >> 28); x04 &= M28UL;
            x06 += (x05 >> 28); x05 &= M28UL;
            x07 += (x06 >> 28); x06 &= M28UL;
            x08 += (x07 >> 28); x07 &= M28UL;
            x09 += (x08 >> 28); x08 &= M28UL;
            x10 += (x09 >> 28); x09 &= M28UL;
            x11 += (x10 >> 28); x10 &= M28UL;
            x12 += (x11 >> 28); x11 &= M28UL;
            x13 += (x12 >> 28); x12 &= M28UL;
            x14 += (x13 >> 28); x13 &= M28UL;
            x15 += (x14 >> 28); x14 &= M28UL;
            x16  = (x15 >> 26); x15 &= M26UL;

            x16 -= 1;

            Debug.Assert(x16 == 0UL || x16 == ulong.MaxValue);

            x00 -= x16 & L_0;
            x01 -= x16 & L_1;
            x02 -= x16 & L_2;
            x03 -= x16 & L_3;
            x04 -= x16 & L_4;
            x05 -= x16 & L_5;
            x06 -= x16 & L_6;
            x07 -= x16 & L_7;

            x01 += (ulong)((long)x00 >> 28); x00 &= M28UL;
            x02 += (ulong)((long)x01 >> 28); x01 &= M28UL;
            x03 += (ulong)((long)x02 >> 28); x02 &= M28UL;
            x04 += (ulong)((long)x03 >> 28); x03 &= M28UL;
            x05 += (ulong)((long)x04 >> 28); x04 &= M28UL;
            x06 += (ulong)((long)x05 >> 28); x05 &= M28UL;
            x07 += (ulong)((long)x06 >> 28); x06 &= M28UL;
            x08 += (ulong)((long)x07 >> 28); x07 &= M28UL;
            x09 += (ulong)((long)x08 >> 28); x08 &= M28UL;
            x10 += (ulong)((long)x09 >> 28); x09 &= M28UL;
            x11 += (ulong)((long)x10 >> 28); x10 &= M28UL;
            x12 += (ulong)((long)x11 >> 28); x11 &= M28UL;
            x13 += (ulong)((long)x12 >> 28); x12 &= M28UL;
            x14 += (ulong)((long)x13 >> 28); x13 &= M28UL;
            x15 += (ulong)((long)x14 >> 28); x14 &= M28UL;

            Debug.Assert(x15 >> 26 == 0UL);

            byte[] r = new byte[ScalarBytes];
            Encode56(x00 | (x01 << 28), r,  0);
            Encode56(x02 | (x03 << 28), r,  7);
            Encode56(x04 | (x05 << 28), r, 14);
            Encode56(x06 | (x07 << 28), r, 21);
            Encode56(x08 | (x09 << 28), r, 28);
            Encode56(x10 | (x11 << 28), r, 35);
            Encode56(x12 | (x13 << 28), r, 42);
            Encode56(x14 | (x15 << 28), r, 49);
    //        r[ScalarBytes - 1] = 0;
            return r;
        }

        private static void ScalarMultBase(byte[] k, PointExt r)
        {
            Precompute();

            PointSetNeutral(r);

            uint[] n = new uint[ScalarUints + 1];
            DecodeScalar(k, 0, n);

            // Recode the scalar into signed-digit form
            {
                n[ScalarUints] = 4U + Nat.CAdd(ScalarUints, ~(int)n[0] & 1, n, L, n);
                uint c = Nat.ShiftDownBit(n.Length, n, 0);
                Debug.Assert(c == (1U << 31));
            }

            PointPrecomp p = new PointPrecomp();

            int cOff = PrecompSpacing - 1;
            for (;;)
            {
                int tPos = cOff;

                for (int b = 0; b < PrecompBlocks; ++b)
                {
                    uint w = 0;
                    for (int t = 0; t < PrecompTeeth; ++t)
                    {
                        uint tBit = (n[tPos >> 5] >> (tPos & 0x1F)) & 1U;
                        w |= tBit << t;
                        tPos += PrecompSpacing;
                    }

                    int sign = (int)(w >> (PrecompTeeth - 1)) & 1;
                    int abs = ((int)w ^ -sign) & PrecompMask;

                    Debug.Assert(sign == 0 || sign == 1);
                    Debug.Assert(0 <= abs && abs < PrecompPoints);

                    PointLookup(b, abs, p);

                    X448Field.CNegate(sign, p.x);

                    PointAddPrecomp(p, r);
                }

                if (--cOff < 0)
                {
                    break;
                }

                PointDouble(r);
            }
        }

        private static void ScalarMultBaseEncoded(byte[] k, byte[] r, int rOff)
        {
            PointExt p = new PointExt();
            ScalarMultBase(k, p);
            EncodePoint(p, r, rOff);
        }

        private static void ScalarMultStraussVar(uint[] nb, uint[] np, PointExt p, PointExt r)
        {
            Precompute();

            int width = 5;

            sbyte[] ws_b = GetWnaf(nb, WnafWidthBase);
            sbyte[] ws_p = GetWnaf(np, width);

            PointExt[] tp = PointPrecompVar(p, 1 << (width - 2));

            PointSetNeutral(r);

            int bit = 447;
            while (bit > 0 && (ws_b[bit] | ws_p[bit]) == 0)
            {
                --bit;
            }

            for (;;)
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

        public static void Sign(byte[] sk, int skOff, byte[] ctx, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
        {
            if (!CheckContextVar(ctx))
            {
                throw new ArgumentException("ctx");
            }

            ShakeDigest d = new ShakeDigest(256);
            byte[] h = new byte[ScalarBytes * 2];

            d.BlockUpdate(sk, skOff, SecretKeySize);
            d.DoFinal(h, 0, h.Length);

            byte[] s = new byte[ScalarBytes];
            PruneScalar(h, 0, s);

            byte[] pk = new byte[PointBytes];
            ScalarMultBaseEncoded(s, pk, 0);

            ImplSign(d, h, s, pk, 0, ctx, m, mOff, mLen, sig, sigOff);
        }

        public static void Sign(byte[] sk, int skOff, byte[] pk, int pkOff, byte[] ctx, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
        {
            if (!CheckContextVar(ctx))
            {
                throw new ArgumentException("ctx");
            }

            ShakeDigest d = new ShakeDigest(256);
            byte[] h = new byte[ScalarBytes * 2];

            d.BlockUpdate(sk, skOff, SecretKeySize);
            d.DoFinal(h, 0, h.Length);

            byte[] s = new byte[ScalarBytes];
            PruneScalar(h, 0, s);

            ImplSign(d, h, s, pk, pkOff, ctx, m, mOff, mLen, sig, sigOff);
        }

        public static bool Verify(byte[] sig, int sigOff, byte[] pk, int pkOff, byte[] ctx, byte[] m, int mOff, int mLen)
        {
            if (!CheckContextVar(ctx))
            {
                throw new ArgumentException("ctx");
            }

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

            byte phflag = 0x00;

            ShakeDigest d = new ShakeDigest(256);
            byte[] h = new byte[ScalarBytes * 2];

            Dom4(d, phflag, ctx);
            d.BlockUpdate(R, 0, PointBytes);
            d.BlockUpdate(pk, pkOff, PointBytes);
            d.BlockUpdate(m, mOff, mLen);
            d.DoFinal(h, 0, h.Length);

            byte[] k = ReduceScalar(h);

            uint[] nS = new uint[ScalarUints];
            DecodeScalar(S, 0, nS);

            uint[] nA = new uint[ScalarUints];
            DecodeScalar(k, 0, nA);

            PointExt pR = new PointExt();
            ScalarMultStraussVar(nS, nA, pA, pR);

            byte[] check = new byte[PointBytes];
            EncodePoint(pR, check, 0);

            return Arrays.AreEqual(check, R);
        }
    }
}
