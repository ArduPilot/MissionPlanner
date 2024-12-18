
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

        public partial class com_tmotor_esc_PUSHSCI : IDroneCANSerialize
        {
            public static void encode_com_tmotor_esc_PUSHSCI(com_tmotor_esc_PUSHSCI msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_tmotor_esc_PUSHSCI(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_tmotor_esc_PUSHSCI(CanardRxTransfer transfer, com_tmotor_esc_PUSHSCI msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_tmotor_esc_PUSHSCI(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_tmotor_esc_PUSHSCI(uint8_t[] buffer, com_tmotor_esc_PUSHSCI msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.data_sequence);
                chunk_cb(buffer, 32, ctx);
                if (!tao) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 8, msg.data_len);
                    chunk_cb(buffer, 8, ctx);
                }
                for (int i=0; i < msg.data_len; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 8, msg.data[i]);
                        chunk_cb(buffer, 8, ctx);
                }
            }

            internal static void _decode_com_tmotor_esc_PUSHSCI(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_tmotor_esc_PUSHSCI msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.data_sequence);
                bit_ofs += 32;

                if (!tao) {
                    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.data_len);
                    bit_ofs += 8;
                } else {
                    msg.data_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);
                }

                msg.data = new uint8_t[msg.data_len];
                for (int i=0; i < msg.data_len; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.data[i]);
                    bit_ofs += 8;
                }

            }
        }
    }
}