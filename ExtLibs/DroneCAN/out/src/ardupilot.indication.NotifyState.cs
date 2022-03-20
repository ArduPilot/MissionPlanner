
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
        static void encode_ardupilot_indication_NotifyState(ardupilot_indication_NotifyState msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_ardupilot_indication_NotifyState(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_ardupilot_indication_NotifyState(CanardRxTransfer transfer, ardupilot_indication_NotifyState msg) {
            uint32_t bit_ofs = 0;
            _decode_ardupilot_indication_NotifyState(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_ardupilot_indication_NotifyState(uint8_t[] buffer, ardupilot_indication_NotifyState msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.aux_data_type);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.aux_data_len);
            chunk_cb(buffer, 8, ctx);
            for (int i=0; i < msg.aux_data_len; i++) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 8, msg.aux_data[i]);
                    chunk_cb(buffer, 8, ctx);
            }
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 64, msg.vehicle_state);
            chunk_cb(buffer, 64, ctx);
        }

        static void _decode_ardupilot_indication_NotifyState(CanardRxTransfer transfer,ref uint32_t bit_ofs, ardupilot_indication_NotifyState msg, bool tao) {

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.aux_data_type);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.aux_data_len);
            bit_ofs += 8;
            msg.aux_data = new uint8_t[msg.aux_data_len];
            for (int i=0; i < msg.aux_data_len; i++) {
                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.aux_data[i]);
                bit_ofs += 8;
            }

            canardDecodeScalar(transfer, bit_ofs, 64, false, ref msg.vehicle_state);
            bit_ofs += 64;

        }
    }
}