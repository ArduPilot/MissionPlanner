
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

        public partial class com_himark_servo_ServoCmd : IDroneCANSerialize
        {
            public static void encode_com_himark_servo_ServoCmd(com_himark_servo_ServoCmd msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_himark_servo_ServoCmd(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_himark_servo_ServoCmd(CanardRxTransfer transfer, com_himark_servo_ServoCmd msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_himark_servo_ServoCmd(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_himark_servo_ServoCmd(uint8_t[] buffer, com_himark_servo_ServoCmd msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                if (!tao) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 5, msg.cmd_len);
                    chunk_cb(buffer, 5, ctx);
                }
                for (int i=0; i < msg.cmd_len; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 10, msg.cmd[i]);
                        chunk_cb(buffer, 10, ctx);
                }
            }

            internal static void _decode_com_himark_servo_ServoCmd(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_himark_servo_ServoCmd msg, bool tao) {

                if (!tao) {
                    canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.cmd_len);
                    bit_ofs += 5;
                } else {
                    msg.cmd_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/10);
                }

                msg.cmd = new uint16_t[msg.cmd_len];
                for (int i=0; i < msg.cmd_len; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 10, false, ref msg.cmd[i]);
                    bit_ofs += 10;
                }

            }
        }
    }
}