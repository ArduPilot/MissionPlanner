
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

namespace DroneCAN
{
    public partial class DroneCAN {
        static void encode_com_hobbywing_esc_SelfTest_res(com_hobbywing_esc_SelfTest_res msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_com_hobbywing_esc_SelfTest_res(buffer, msg, chunk_cb, ctx, !fdcan);
        }

        static uint32_t decode_com_hobbywing_esc_SelfTest_res(CanardRxTransfer transfer, com_hobbywing_esc_SelfTest_res msg, bool fdcan) {
            uint32_t bit_ofs = 0;
            _decode_com_hobbywing_esc_SelfTest_res(transfer, ref bit_ofs, msg, !fdcan);
            return (bit_ofs+7)/8;
        }

        static void _encode_com_hobbywing_esc_SelfTest_res(uint8_t[] buffer, com_hobbywing_esc_SelfTest_res msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.status);
            chunk_cb(buffer, 8, ctx);
        }

        static void _decode_com_hobbywing_esc_SelfTest_res(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hobbywing_esc_SelfTest_res msg, bool tao) {

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.status);
            bit_ofs += 8;

        }
    }
}