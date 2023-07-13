
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

        public partial class com_xacti_GnssStatus : IDroneCANSerialize
        {
            public static void encode_com_xacti_GnssStatus(com_xacti_GnssStatus msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_xacti_GnssStatus(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_xacti_GnssStatus(CanardRxTransfer transfer, com_xacti_GnssStatus msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_xacti_GnssStatus(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_xacti_GnssStatus(uint8_t[] buffer, com_xacti_GnssStatus msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.gps_status);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.order);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.remain_buffer);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.utc_year);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.utc_month);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.utc_day);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.utc_hour);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.utc_minute);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.utc_seconds);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 64, msg.latitude);
                chunk_cb(buffer, 64, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 64, msg.longitude);
                chunk_cb(buffer, 64, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.altitude);
                chunk_cb(buffer, 32, ctx);
            }

            internal static void _decode_com_xacti_GnssStatus(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_xacti_GnssStatus msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.gps_status);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.order);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.remain_buffer);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.utc_year);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.utc_month);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.utc_day);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.utc_hour);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.utc_minute);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.utc_seconds);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 64, true, ref msg.latitude);
                bit_ofs += 64;

                canardDecodeScalar(transfer, bit_ofs, 64, true, ref msg.longitude);
                bit_ofs += 64;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.altitude);
                bit_ofs += 32;

            }
        }
    }
}