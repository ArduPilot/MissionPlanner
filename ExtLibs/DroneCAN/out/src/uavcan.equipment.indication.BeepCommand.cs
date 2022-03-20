
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
        static void encode_uavcan_equipment_indication_BeepCommand(uavcan_equipment_indication_BeepCommand msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_uavcan_equipment_indication_BeepCommand(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_uavcan_equipment_indication_BeepCommand(CanardRxTransfer transfer, uavcan_equipment_indication_BeepCommand msg) {
            uint32_t bit_ofs = 0;
            _decode_uavcan_equipment_indication_BeepCommand(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_uavcan_equipment_indication_BeepCommand(uint8_t[] buffer, uavcan_equipment_indication_BeepCommand msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.frequency);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.duration);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
        }

        static void _decode_uavcan_equipment_indication_BeepCommand(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_indication_BeepCommand msg, bool tao) {

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.frequency = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.duration = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

        }
    }
}