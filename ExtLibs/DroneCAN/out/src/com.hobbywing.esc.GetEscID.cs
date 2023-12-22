
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

        public partial class com_hobbywing_esc_GetEscID : IDroneCANSerialize
        {
            public static void encode_com_hobbywing_esc_GetEscID(com_hobbywing_esc_GetEscID msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_hobbywing_esc_GetEscID(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_hobbywing_esc_GetEscID(CanardRxTransfer transfer, com_hobbywing_esc_GetEscID msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_hobbywing_esc_GetEscID(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_hobbywing_esc_GetEscID(uint8_t[] buffer, com_hobbywing_esc_GetEscID msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                if (!tao) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 2, msg.payload_len);
                    chunk_cb(buffer, 2, ctx);
                }
                for (int i=0; i < msg.payload_len; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 8, msg.payload[i]);
                        chunk_cb(buffer, 8, ctx);
                }
            }

            internal static void _decode_com_hobbywing_esc_GetEscID(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hobbywing_esc_GetEscID msg, bool tao) {

                if (!tao) {
                    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.payload_len);
                    bit_ofs += 2;
                } else {
                    msg.payload_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);
                }

                msg.payload = new uint8_t[msg.payload_len];
                for (int i=0; i < msg.payload_len; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.payload[i]);
                    bit_ofs += 8;
                }

            }
        }
    }
}