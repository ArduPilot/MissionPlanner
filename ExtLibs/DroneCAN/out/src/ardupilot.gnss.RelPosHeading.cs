
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
        static void encode_ardupilot_gnss_RelPosHeading(ardupilot_gnss_RelPosHeading msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_ardupilot_gnss_RelPosHeading(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_ardupilot_gnss_RelPosHeading(CanardRxTransfer transfer, ardupilot_gnss_RelPosHeading msg) {
            uint32_t bit_ofs = 0;
            _decode_ardupilot_gnss_RelPosHeading(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_ardupilot_gnss_RelPosHeading(uint8_t[] buffer, ardupilot_gnss_RelPosHeading msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 1, msg.reported_heading_acc_available);
            chunk_cb(buffer, 1, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.reported_heading_deg);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.reported_heading_acc_deg);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.relative_distance_m);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.relative_down_pos_m);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
        }

        static void _decode_ardupilot_gnss_RelPosHeading(CanardRxTransfer transfer,ref uint32_t bit_ofs, ardupilot_gnss_RelPosHeading msg, bool tao) {

            _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);

            canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.reported_heading_acc_available);
            bit_ofs += 1;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.reported_heading_deg);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.reported_heading_acc_deg);
            bit_ofs += 32;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.relative_distance_m = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.relative_down_pos_m = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

        }
    }
}