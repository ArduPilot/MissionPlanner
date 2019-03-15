using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace UAVCAN
{
    public partial class uavcan
    {
        private static int CANARD_ERROR_INTERNAL = -1;

        public delegate void uavcan_serializer_chunk_cb_ptr_t(byte[] buffer, int sizeinbits, object ctx);

        [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 1)]
        public struct union
        {
            [FieldOffset(0)] public bool boolean;

            ///< sizeof(bool) is implementation-defined, so it has to be handled separately
            [FieldOffset(0)] public Byte u8;

            ///< Also char
            [FieldOffset(0)] public SByte s8;

            [FieldOffset(0)] public UInt16 u16;
            [FieldOffset(0)] public Int16 s16;
            [FieldOffset(0)] public UInt32 u32;
            [FieldOffset(0)] public Int32 s32;

            ///< Also float, possibly double, possibly long double (depends on implementation)
            [FieldOffset(0)] public UInt64 u64;

            [FieldOffset(0)] public Int64 s64;

            [FieldOffset(0)] public float f32;
            [FieldOffset(0)] public double d64;

            public Byte this[int index]
            {
                get { return BitConverter.GetBytes(u64)[index]; }
                set
                {
                    var temp = BitConverter.GetBytes(u64);
                    temp[index] = value;
                    u64 = BitConverter.ToUInt64(temp, 0);
                }
            }

            ///< Also double, possibly float, possibly long double (depends on implementation)
            public IReadOnlyList<Byte> bytes
            {
                get { return BitConverter.GetBytes(u64); }
                /* set
                {
                    var temp = value.ToArray();
                    Array.Resize(ref temp, 8);
                    u64 = BitConverter.ToUInt64(temp, 0);
                }*/
            }

            public union(bool b1 = false)
            {
                boolean = false;
                u8 = 0;
                s8 = 0;
                u16 = 0;
                u32 = 0;
                u64 = 0;
                s8 = 0;
                s16 = 0;
                s32 = 0;
                s64 = 0;
                f32 = 0;
                d64 = 0;
            }
        }



        public class CanardRxTransfer : IEnumerable<byte>
        {
            public UInt32 payload_len
            {
                get { return (UInt32) data.Length; }
            }

            public byte[] data;

            public CanardRxTransfer(byte[] input)
            {
                data = input;
            }

            public byte this[int a]
            {
                get { return data[a]; }
            }

            public IEnumerator<byte> GetEnumerator()
            {
                return ((IEnumerable<byte>) data).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<byte>) data).GetEnumerator();
            }
        }

        public static void canardEncodeScalar<T>(byte[] destination,
            uint bit_offset,
            byte bit_length,
            T value)
        {
            union storage = new union(false);

            Byte std_byte_length = 0;

            // Extra most significant bits can be safely ignored here.
            if (bit_length == 1)
            {
                std_byte_length = sizeof(bool);
                storage.boolean = (bool) (dynamic) value;
            }
            else if (bit_length <= 8)
            {
                std_byte_length = 1;
                storage.u8 = ((Byte) (dynamic) value);
            }
            else if (bit_length <= 16)
            {
                std_byte_length = 2;
                storage.u16 = ((UInt16) (dynamic) value);
            }
            else if (bit_length <= 32)
            {
                std_byte_length = 4;
                storage.u32 = ((UInt32) (dynamic) value);
            }
            else if (bit_length <= 64)
            {
                std_byte_length = 8;
                storage.u64 = ((UInt64) (dynamic) value);
            }

            /*
 * The bit copy algorithm assumes that more significant bits have lower index, so we need to shift some.
 * Extra least significant bits will be filled with zeroes, which is fine.
 * Extra most significant bits will be discarded here.
 * Coverity Scan mistakenly believes that the array may be overrun if bit_length == 64; however, this branch will
 * not be taken if bit_length == 64, because 64 % 8 == 0.
 */
            if ((bit_length % 8) != 0)
            {
                // coverity[overrun-local]
                storage[bit_length / 8] = (byte)(storage.bytes[bit_length / 8] << ((8 - (bit_length % 8)) & 7));
            }

            copyBitArray(storage.bytes.ToArray(), 0, bit_length, destination, bit_offset);
        }

        private static void copyBitArray(


            byte[] src, UInt32 src_offset, UInt32 src_len, byte[] dstin, UInt32 dst_offset)
        {
            CANARD_ASSERT(src_len > 0U);

            var dst = dstin;

            // Normalizing inputs
            //src += src_offset / 8;
            //dst += dst_offset / 8;
            if (src_offset >= 8)
                src = src.Skip((int) src_offset / 8).ToArray();
            if (dst_offset >= 8)
                dst = dst.Skip((int) dst_offset / 8).ToArray();

            src_offset %= 8;
            dst_offset %= 8;

            uint last_bit = src_offset + src_len;
            while (last_bit - src_offset > 0)
            {
                Byte src_bit_offset = (Byte) (src_offset % 8U);
                Byte dst_bit_offset = (Byte) (dst_offset % 8U);

                Byte max_offset = (Byte) Math.Max(src_bit_offset, dst_bit_offset);
                UInt32 copy_bits = (UInt32) Math.Min(last_bit - src_offset, 8U - max_offset);

                Byte write_mask = (Byte) ((Byte) (0xFF00U >> (int) copy_bits) >> dst_bit_offset);
                Byte src_data = (Byte) ((src[src_offset / 8U] << src_bit_offset) >> dst_bit_offset);

                dst[dst_offset / 8U] = (Byte) ((dst[dst_offset / 8U] & ~write_mask) | (src_data & write_mask));

                src_offset += copy_bits;
                dst_offset += copy_bits;
            }

            if(dst != dstin)
                Array.ConstrainedCopy(dst, 0, dstin, dstin.Length - dst.Length, dst.Length);
        }

        public static void uavcan_transmit_chunk_handler(byte[] buffer, int bitlen, object ctx)
        {
            if (buffer == null)
            {
                ((uavcan.statetracking)ctx).bit += bitlen;
                return;
            }

            byte[] output = new byte[8];
            var frame_bit_ofs = ((uavcan.statetracking)ctx).bit;
            int chunk_bit_ofs = 0;

            while (chunk_bit_ofs<bitlen) {
                int frame_copy_bits = Math.Min(bitlen - chunk_bit_ofs, (7 * 8 - frame_bit_ofs));
                if (frame_copy_bits <= 0) {
                    frame_bit_ofs = 0;
                    continue;
                }
                copyBitArray(buffer, (uint)chunk_bit_ofs, (uint)frame_copy_bits, output, (uint)frame_bit_ofs);
                chunk_bit_ofs += frame_copy_bits;
                frame_bit_ofs += frame_copy_bits;
            }

            BigInteger input = new BigInteger(output.Reverse().ToArray());

            for (uint a = 0; a < (bitlen + (8-(bitlen%8))); a++)
            {
                if ((input & (1L << (int)a)) > 0)
                {
                    ((uavcan.statetracking)ctx).bi.setBit((uint)((uavcan.statetracking)ctx).bit + a);
                }
            }

            ((uavcan.statetracking) ctx).bit += chunk_bit_ofs;
        }

        public static void memset(byte[] buffer, int chartocopy, int size)
        {
            Array.Clear(buffer, 0, size);
        }

        public static int canardDecodeScalar<T>(CanardRxTransfer transfer,
            uint bit_offset,
            byte bit_length,
            bool value_is_signed,
            ref T out_value)
        {

            union storage = new union(false);

            memset(storage.bytes.ToArray(), 0, Marshal.SizeOf(storage)); // This is important

            int result = descatterTransferPayload(transfer, bit_offset, bit_length, ref storage);
            if (result <= 0)
            {
                return result;
            }

            CANARD_ASSERT((result > 0) && (result <= 64) && (result <= bit_length));

            /*
             * The bit copy algorithm assumes that more significant bits have lower index, so we need to shift some.
             * Extra most significant bits will be filled with zeroes, which is fine.
             * Coverity Scan mistakenly believes that the array may be overrun if bit_length == 64; however, this branch will
             * not be taken if bit_length == 64, because 64 % 8 == 0.
             */
            if ((bit_length % 8) != 0)
            {
                // coverity[overrun-local]
                storage[bit_length / 8] = (byte) (storage.bytes[bit_length / 8] >> ((8 - (bit_length % 8)) & 7));
            }

            /*
             * Determining the closest standard byte length - this will be needed for byte reordering and sign bit extension.
             */
            Byte std_byte_length = 0;
            if (bit_length == 1)
            {
                std_byte_length = sizeof(bool);
            }
            else if (bit_length <= 8)
            {
                std_byte_length = 1;
            }
            else if (bit_length <= 16)
            {
                std_byte_length = 2;
            }
            else if (bit_length <= 32)
            {
                std_byte_length = 4;
            }
            else if (bit_length <= 64)
            {
                std_byte_length = 8;
            }
            else
            {
                CANARD_ASSERT(false);
                return -CANARD_ERROR_INTERNAL;
            }

            CANARD_ASSERT((std_byte_length > 0) && (std_byte_length <= 8));

            /*
             * Flipping the byte order if needed.
             */
            /*if (isBigEndian())
            {
                swapByteOrder(&storage.bytes[0], std_byte_length);
            }*/

            /*
             * Extending the sign bit if needed. I miss templates.
             */
            if (value_is_signed && (std_byte_length * 8 != bit_length))
            {
                if (bit_length <= 8)
                {
                    if ((storage.s8 & (1U << (bit_length - 1))) != 0) // If the sign bit is set...
                    {
                        storage.u8 |=
                            (byte) ((Byte) 0xFFU & (Byte) ~((1 << bit_length) - 1U)); // ...set all bits above it.
                    }
                }
                else if (bit_length <= 16)
                {
                    if ((storage.s16 & (1U << (bit_length - 1))) != 0)
                    {
                        storage.u16 |= (UInt16) ((UInt16) 0xFFFFU & (UInt16) ~((1 << bit_length) - 1U));
                    }
                }
                else if (bit_length <= 32)
                {
                    if ((storage.s32 & (((UInt32) 1) << (bit_length - 1))) != 0)
                    {
                        storage.u32 |= (UInt32) 0xFFFFFFFFU & (UInt32) ~((((UInt32) 1U) << bit_length) - 1U);
                    }
                }
                else if (bit_length < 64) // Strictly less, this is not a typo
                {
                    if ((storage.u64 & (((UInt64) 1) << (bit_length - 1))) != 0)
                    {
                        storage.u64 |= (UInt64) 0xFFFFFFFFFFFFFFFFU & (UInt64) ~((((UInt64) 1) << bit_length) - 1U);
                    }
                }
                else
                {
                    CANARD_ASSERT(false);
                    return -CANARD_ERROR_INTERNAL;
                }
            }

            /*
             * Copying the result out.
             */
            if (value_is_signed)
            {
                if (bit_length <= 8)
                {
                    out_value = (T) (IConvertible) storage.s8;
                }
                else if (bit_length <= 16)
                {
                    out_value = (T) (dynamic) storage.s16;
                }
                else if (typeof(T) == typeof(float))
                {
                    out_value = (T) (IConvertible) storage.f32;
                }
                else if (bit_length <= 32)
                {
                    out_value = (T) (IConvertible) storage.s32;
                }
                else if (bit_length <= 64)
                {
                    out_value = (T) (IConvertible) storage.s64;
                }
                else
                {
                    CANARD_ASSERT(false);
                    return -CANARD_ERROR_INTERNAL;
                }
            }
            else
            {
                if (bit_length == 1)
                {
                    out_value = (T) (IConvertible) storage.boolean;
                }
                else if (bit_length <= 8)
                {
                    out_value = (T) (IConvertible) storage.u8;
                }
                else if (bit_length <= 16)
                {
                    out_value = (T) (IConvertible) storage.u16;
                }
                else if (bit_length <= 32)
                {
                    out_value = (T) (IConvertible) storage.u32;
                }
                else if (bit_length <= 64)
                {
                    out_value = (T) (IConvertible) storage.u64;
                }
                else
                {
                    CANARD_ASSERT(false);
                    return -CANARD_ERROR_INTERNAL;
                }
            }

            CANARD_ASSERT(result <= bit_length);
            CANARD_ASSERT(result > 0);
            return result;
        }

        private static int descatterTransferPayload(CanardRxTransfer transfer, uint bit_offset, byte bit_length
            , ref union output)
        {
            if (bit_offset >= (transfer.payload_len * 8))
            {
                return 0; // Out of range, reading zero bits
            }

            if (bit_offset + bit_length > (transfer.payload_len * 8))
                bit_length = (Byte) (transfer.payload_len * 8 - bit_offset);

            byte[] dest = new byte[8];

            copyBitArray(transfer.data, bit_offset, bit_length, dest, 0);

            Array.Resize(ref dest, 8);
            output.u64 = BitConverter.ToUInt64(dest, 0);
            return bit_length;
        }

        private static void CANARD_ASSERT(bool p)
        {
            if (p == false)
                throw new ArgumentException();
        }

        private static UInt16 canardConvertNativeFloatToFloat16(Single flo)
        {
            var bad = make_float16(flo);
            return bad;
        }

        private static Single canardConvertFloat16ToNativeFloat(ushort float16Val)
        {
            var bad = float16_to_float32(float16Val);
            return bad;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct fp32
        {
            [FieldOffset(0)] public uint u;
            [FieldOffset(0)] public float f;

            public static implicit operator fp32(uint v)
            {
                return new fp32() {u = v};
            }
        }

        private static float float16_to_float32(ushort h)
        {
            fp32 ans = 0;
            UInt16 h_exp, h_sig;
            UInt32 f_sgn, f_exp, f_sig;

            h_exp = (UInt16)(h & 0x7c00u);
            f_sgn = ((UInt32)h & 0x8000u) << 16;
            switch (h_exp)
            {
                case 0x0000: /* 0 or subnormal */
                    h_sig = (UInt16)(h & 0x03ffu);
                    /* Signed zero */
                    if (h_sig == 0)
                    {
                        ans= f_sgn;
                        return ans.f;
                    }
                    /* Subnormal */
                    h_sig <<= 1;
                    while ((h_sig & 0x0400u) == 0)
                    {
                        h_sig <<= 1;
                        h_exp++;
                    }
                    f_exp = ((UInt32)(127 - 15 - h_exp)) << 23;
                    f_sig = ((UInt32)(h_sig & 0x03ffu)) << 13;
                    ans= f_sgn + f_exp + f_sig;
                    return ans.f;
                case 0x7c00: /* inf or NaN */
                    /* All-ones exponent and a copy of the significand */
                    ans= f_sgn + 0x7f800000u + (((UInt32)(h & 0x03ffu)) << 13);
                    return ans.f;
                default: /* normalized */
                    /* Just need to adjust the exponent and shift */
                    ans= f_sgn + (((UInt32)(h & 0x7fffu) + 0x1c000u) << 13);
                    return ans.f;
            }
        }

        private static ushort make_float16(float value)
        {
            fp32 f32infty = 255U << 23;
            fp32 f16infty = 31U << 23;
            fp32 magic = 15U << 23;
            uint sign_mask = 0x80000000U;
            uint round_mask = ~0xFFFU;

            fp32 @in = 0;
            ushort @out = 0;

            @in.f = value;

            uint sign = @in.u & sign_mask;
            @in.u ^= sign;

            if (@in.u >= f32infty.u)
            {
                @out = (@in.u > f32infty.u) ? (ushort)0x7FFFU : (ushort)0x7C00U;
            }
            else
            {
                @in.u &= round_mask;
                @in.f *= magic.f;
                @in.u -= round_mask;
                if (@in.u > f16infty.u)
                {
                    @in.u = f16infty.u;
                }

                @out = (ushort) (@in.u >> 13);
            }

            @out |= (ushort) (sign >> 16);

            return @out;
        }
    }
}