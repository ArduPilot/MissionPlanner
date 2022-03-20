
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
        static void encode_mppt_OutputEnable_res(mppt_OutputEnable_res msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_mppt_OutputEnable_res(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_mppt_OutputEnable_res(CanardRxTransfer transfer, mppt_OutputEnable_res msg) {
            uint32_t bit_ofs = 0;
            _decode_mppt_OutputEnable_res(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_mppt_OutputEnable_res(uint8_t[] buffer, mppt_OutputEnable_res msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            chunk_cb(null, 7, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 1, msg.enabled);
            chunk_cb(buffer, 1, ctx);
        }

        static void _decode_mppt_OutputEnable_res(CanardRxTransfer transfer,ref uint32_t bit_ofs, mppt_OutputEnable_res msg, bool tao) {

            bit_ofs += 7;

            canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.enabled);
            bit_ofs += 1;

        }
    }
}