
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

        public partial class dronecan_protocol_Stats : IDroneCANSerialize
        {
            public static void encode_dronecan_protocol_Stats(dronecan_protocol_Stats msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_dronecan_protocol_Stats(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_dronecan_protocol_Stats(CanardRxTransfer transfer, dronecan_protocol_Stats msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_dronecan_protocol_Stats(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_dronecan_protocol_Stats(uint8_t[] buffer, dronecan_protocol_Stats msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.tx_frames);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.tx_errors);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.rx_frames);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_error_oom);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_error_internal);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_error_missed_start);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_error_wrong_toggle);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_error_short_frame);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_error_bad_crc);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_ignored_wrong_address);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_ignored_not_wanted);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.rx_ignored_unexpected_tid);
                chunk_cb(buffer, 16, ctx);
            }

            internal static void _decode_dronecan_protocol_Stats(CanardRxTransfer transfer,ref uint32_t bit_ofs, dronecan_protocol_Stats msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.tx_frames);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.tx_errors);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.rx_frames);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_error_oom);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_error_internal);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_error_missed_start);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_error_wrong_toggle);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_error_short_frame);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_error_bad_crc);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_ignored_wrong_address);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_ignored_not_wanted);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.rx_ignored_unexpected_tid);
                bit_ofs += 16;

            }
        }
    }
}