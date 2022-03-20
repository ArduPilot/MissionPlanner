
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
        static void encode_mppt_OutputEnable_req(mppt_OutputEnable_req msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_mppt_OutputEnable_req(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_mppt_OutputEnable_req(CanardRxTransfer transfer, mppt_OutputEnable_req msg) {
            uint32_t bit_ofs = 0;
            _decode_mppt_OutputEnable_req(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_mppt_OutputEnable_req(uint8_t[] buffer, mppt_OutputEnable_req msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            chunk_cb(null, 6, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 1, msg.enable);
            chunk_cb(buffer, 1, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 1, msg.disable);
            chunk_cb(buffer, 1, ctx);
        }

        static void _decode_mppt_OutputEnable_req(CanardRxTransfer transfer,ref uint32_t bit_ofs, mppt_OutputEnable_req msg, bool tao) {

            bit_ofs += 6;

            canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.enable);
            bit_ofs += 1;

            canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.disable);
            bit_ofs += 1;

        }
    }
}