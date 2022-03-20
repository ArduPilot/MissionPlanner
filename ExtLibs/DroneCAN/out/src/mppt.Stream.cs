
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
        static void encode_mppt_Stream(mppt_Stream msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_mppt_Stream(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_mppt_Stream(CanardRxTransfer transfer, mppt_Stream msg) {
            uint32_t bit_ofs = 0;
            _decode_mppt_Stream(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_mppt_Stream(uint8_t[] buffer, mppt_Stream msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.fault_flags);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.temperature);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.input_voltage);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.input_current);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.input_power);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.output_voltage);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.output_current);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.output_power);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
        }

        static void _decode_mppt_Stream(CanardRxTransfer transfer,ref uint32_t bit_ofs, mppt_Stream msg, bool tao) {

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.fault_flags);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 8, true, ref msg.temperature);
            bit_ofs += 8;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.input_voltage = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.input_current = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.input_power = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.output_voltage = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.output_current = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.output_power = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

        }
    }
}