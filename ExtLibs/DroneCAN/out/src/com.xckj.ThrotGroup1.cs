
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using int8_t = System.SByte;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;

using float32 = System.Single;

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using static DroneCAN.DroneCAN;

namespace DroneCAN
{
    public partial class DroneCAN
    {

        public partial class com_xckj_esc_ThrotGroup2 : IDroneCANSerialize
        {
            public static void encode_com_xckj_esc_ThrotGroup2(com_xckj_esc_ThrotGroup2 msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan)
            {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_xckj_esc_ThrotGroup2(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_xckj_esc_ThrotGroup2(CanardRxTransfer transfer, com_xckj_esc_ThrotGroup2 msg, bool fdcan)
            {
                uint32_t bit_ofs = 0;
                _decode_com_xckj_esc_ThrotGroup2(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs + 7) / 8;
            }

            internal static void _encode_com_xckj_esc_ThrotGroup2(uint8_t[] buffer, com_xckj_esc_ThrotGroup2 msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao)
            {
                if (!tao)
                {
                    memset(buffer, 0, 8);
                    canardEncodeScalar(buffer, 0, 2, msg.command_len);
                    chunk_cb(buffer, 2, ctx);
                }
                for (int i = 0; i < msg.command_len; i++)
                {
                    memset(buffer, 0, 8);
                    canardEncodeScalar(buffer, 0, 8, msg.command[i]);
                    chunk_cb(buffer, 8, ctx);
                }
            }

            internal static void _decode_com_xckj_esc_ThrotGroup2(CanardRxTransfer transfer, ref uint32_t bit_ofs, com_xckj_esc_ThrotGroup2 msg, bool tao)
            {
                if (!tao)
                {
                    canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.command_len);
                    bit_ofs += 4;
                }
                else
                {
                    msg.command_len = (uint8_t)(((transfer.payload_len * 8) - bit_ofs) / 14);
                }

                msg.command = new uint16_t[msg.command_len];
                for (int i = 0; i < msg.command_len; i++)
                {
                    canardDecodeScalar(transfer, bit_ofs, 14, true, ref msg.command[i]);
                    bit_ofs += 14;
                }
            }
        }
    }

}
