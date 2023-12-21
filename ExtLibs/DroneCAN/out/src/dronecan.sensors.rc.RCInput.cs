
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

        public partial class dronecan_sensors_rc_RCInput : IDroneCANSerialize
        {
            public static void encode_dronecan_sensors_rc_RCInput(dronecan_sensors_rc_RCInput msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_dronecan_sensors_rc_RCInput(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_dronecan_sensors_rc_RCInput(CanardRxTransfer transfer, dronecan_sensors_rc_RCInput msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_dronecan_sensors_rc_RCInput(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_dronecan_sensors_rc_RCInput(uint8_t[] buffer, dronecan_sensors_rc_RCInput msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.status);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.quality);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 4, msg.id);
                chunk_cb(buffer, 4, ctx);
                if (!tao) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 6, msg.rcin_len);
                    chunk_cb(buffer, 6, ctx);
                }
                for (int i=0; i < msg.rcin_len; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 12, msg.rcin[i]);
                        chunk_cb(buffer, 12, ctx);
                }
            }

            internal static void _decode_dronecan_sensors_rc_RCInput(CanardRxTransfer transfer,ref uint32_t bit_ofs, dronecan_sensors_rc_RCInput msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.status);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.quality);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.id);
                bit_ofs += 4;

                if (!tao) {
                    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.rcin_len);
                    bit_ofs += 6;
                } else {
                    msg.rcin_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/12);
                }

                msg.rcin = new uint16_t[msg.rcin_len];
                for (int i=0; i < msg.rcin_len; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 12, false, ref msg.rcin[i]);
                    bit_ofs += 12;
                }

            }
        }
    }
}