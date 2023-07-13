
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

        public partial class com_hobbywing_esc_GetMajorConfig_res : IDroneCANSerialize
        {
            public static void encode_com_hobbywing_esc_GetMajorConfig_res(com_hobbywing_esc_GetMajorConfig_res msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_hobbywing_esc_GetMajorConfig_res(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_hobbywing_esc_GetMajorConfig_res(CanardRxTransfer transfer, com_hobbywing_esc_GetMajorConfig_res msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_hobbywing_esc_GetMajorConfig_res(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_hobbywing_esc_GetMajorConfig_res(uint8_t[] buffer, com_hobbywing_esc_GetMajorConfig_res msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 1, msg.direction);
                chunk_cb(buffer, 1, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 1, msg.throttle_source);
                chunk_cb(buffer, 1, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 6, msg.throttle_channel);
                chunk_cb(buffer, 6, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 5, msg.led_status);
                chunk_cb(buffer, 5, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 3, msg.led_color);
                chunk_cb(buffer, 3, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 4, msg.MSG2_rate);
                chunk_cb(buffer, 4, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 4, msg.MSG1_rate);
                chunk_cb(buffer, 4, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.positioning_angle);
                chunk_cb(buffer, 16, ctx);
                for (int i=0; i < 2; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 8, msg.reserved[i]);
                        chunk_cb(buffer, 8, ctx);
                }
            }

            internal static void _decode_com_hobbywing_esc_GetMajorConfig_res(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hobbywing_esc_GetMajorConfig_res msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.direction);
                bit_ofs += 1;

                canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.throttle_source);
                bit_ofs += 1;

                canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.throttle_channel);
                bit_ofs += 6;

                canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.led_status);
                bit_ofs += 5;

                canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.led_color);
                bit_ofs += 3;

                canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.MSG2_rate);
                bit_ofs += 4;

                canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.MSG1_rate);
                bit_ofs += 4;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.positioning_angle);
                bit_ofs += 16;

                for (int i=0; i < 2; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.reserved[i]);
                    bit_ofs += 8;
                }

            }
        }
    }
}