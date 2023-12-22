
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

        public partial class com_hobbywing_esc_StatusMsg2 : IDroneCANSerialize
        {
            public static void encode_com_hobbywing_esc_StatusMsg2(com_hobbywing_esc_StatusMsg2 msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_hobbywing_esc_StatusMsg2(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_hobbywing_esc_StatusMsg2(CanardRxTransfer transfer, com_hobbywing_esc_StatusMsg2 msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_hobbywing_esc_StatusMsg2(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_hobbywing_esc_StatusMsg2(uint8_t[] buffer, com_hobbywing_esc_StatusMsg2 msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.input_voltage);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.current);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.temperature);
                chunk_cb(buffer, 8, ctx);
            }

            internal static void _decode_com_hobbywing_esc_StatusMsg2(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hobbywing_esc_StatusMsg2 msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.input_voltage);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.current);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.temperature);
                bit_ofs += 8;

            }
        }
    }
}