
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

        public partial class dronecan_protocol_CanStats : IDroneCANSerialize
        {
            public static void encode_dronecan_protocol_CanStats(dronecan_protocol_CanStats msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_dronecan_protocol_CanStats(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_dronecan_protocol_CanStats(CanardRxTransfer transfer, dronecan_protocol_CanStats msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_dronecan_protocol_CanStats(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_dronecan_protocol_CanStats(uint8_t[] buffer, dronecan_protocol_CanStats msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.@interface);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.tx_requests);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.tx_rejected);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.tx_overflow);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.tx_success);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.tx_timedout);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.tx_abort);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.rx_received);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_overflow);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_errors);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.busoff_errors);
                chunk_cb(buffer, 16, ctx);
            }

            internal static void _decode_dronecan_protocol_CanStats(CanardRxTransfer transfer,ref uint32_t bit_ofs, dronecan_protocol_CanStats msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.@interface);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.tx_requests);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.tx_rejected);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.tx_overflow);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.tx_success);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.tx_timedout);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.tx_abort);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.rx_received);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_overflow);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_errors);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.busoff_errors);
                bit_ofs += 16;

            }
        }
    }
}