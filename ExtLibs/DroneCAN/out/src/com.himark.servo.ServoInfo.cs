
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

        public partial class com_himark_servo_ServoInfo : IDroneCANSerialize
        {
            public static void encode_com_himark_servo_ServoInfo(com_himark_servo_ServoInfo msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_himark_servo_ServoInfo(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_himark_servo_ServoInfo(CanardRxTransfer transfer, com_himark_servo_ServoInfo msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_himark_servo_ServoInfo(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_himark_servo_ServoInfo(uint8_t[] buffer, com_himark_servo_ServoInfo msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 5, msg.servo_id);
                chunk_cb(buffer, 5, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 12, msg.pwm_input);
                chunk_cb(buffer, 12, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.pos_cmd);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.pos_sensor);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 12, msg.voltage);
                chunk_cb(buffer, 12, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 10, msg.current);
                chunk_cb(buffer, 10, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 10, msg.pcb_temp);
                chunk_cb(buffer, 10, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 10, msg.motor_temp);
                chunk_cb(buffer, 10, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 5, msg.error_status);
                chunk_cb(buffer, 5, ctx);
            }

            internal static void _decode_com_himark_servo_ServoInfo(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_himark_servo_ServoInfo msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.servo_id);
                bit_ofs += 5;

                canardDecodeScalar(transfer, bit_ofs, 12, false, ref msg.pwm_input);
                bit_ofs += 12;

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.pos_cmd);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.pos_sensor);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 12, false, ref msg.voltage);
                bit_ofs += 12;

                canardDecodeScalar(transfer, bit_ofs, 10, false, ref msg.current);
                bit_ofs += 10;

                canardDecodeScalar(transfer, bit_ofs, 10, false, ref msg.pcb_temp);
                bit_ofs += 10;

                canardDecodeScalar(transfer, bit_ofs, 10, false, ref msg.motor_temp);
                bit_ofs += 10;

                canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.error_status);
                bit_ofs += 5;

            }
        }
    }
}