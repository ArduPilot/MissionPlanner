
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

        public partial class uavcan_equipment_esc_StatusExtended : IDroneCANSerialize
        {
            public static void encode_uavcan_equipment_esc_StatusExtended(uavcan_equipment_esc_StatusExtended msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_uavcan_equipment_esc_StatusExtended(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_uavcan_equipment_esc_StatusExtended(CanardRxTransfer transfer, uavcan_equipment_esc_StatusExtended msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_uavcan_equipment_esc_StatusExtended(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_uavcan_equipment_esc_StatusExtended(uint8_t[] buffer, uavcan_equipment_esc_StatusExtended msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 7, msg.input_pct);
                chunk_cb(buffer, 7, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 7, msg.output_pct);
                chunk_cb(buffer, 7, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 9, msg.motor_temperature_degC);
                chunk_cb(buffer, 9, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 9, msg.motor_angle);
                chunk_cb(buffer, 9, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 19, msg.status_flags);
                chunk_cb(buffer, 19, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 5, msg.esc_index);
                chunk_cb(buffer, 5, ctx);
            }

            internal static void _decode_uavcan_equipment_esc_StatusExtended(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_esc_StatusExtended msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.input_pct);
                bit_ofs += 7;

                canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.output_pct);
                bit_ofs += 7;

                canardDecodeScalar(transfer, bit_ofs, 9, true, ref msg.motor_temperature_degC);
                bit_ofs += 9;

                canardDecodeScalar(transfer, bit_ofs, 9, false, ref msg.motor_angle);
                bit_ofs += 9;

                canardDecodeScalar(transfer, bit_ofs, 19, false, ref msg.status_flags);
                bit_ofs += 19;

                canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.esc_index);
                bit_ofs += 5;

            }
        }
    }
}