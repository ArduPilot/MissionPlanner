
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

        public partial class com_tmotor_esc_FocCtrl : IDroneCANSerialize
        {
            public static void encode_com_tmotor_esc_FocCtrl(com_tmotor_esc_FocCtrl msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_tmotor_esc_FocCtrl(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_tmotor_esc_FocCtrl(CanardRxTransfer transfer, com_tmotor_esc_FocCtrl msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_tmotor_esc_FocCtrl(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_tmotor_esc_FocCtrl(uint8_t[] buffer, com_tmotor_esc_FocCtrl msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.esc_index);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.esc_mode);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.esc_fdb_rate);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.esc_cmd);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.esc_cmd_val);
                chunk_cb(buffer, 32, ctx);
            }

            internal static void _decode_com_tmotor_esc_FocCtrl(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_tmotor_esc_FocCtrl msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.esc_index);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.esc_mode);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.esc_fdb_rate);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.esc_cmd);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.esc_cmd_val);
                bit_ofs += 32;

            }
        }
    }
}