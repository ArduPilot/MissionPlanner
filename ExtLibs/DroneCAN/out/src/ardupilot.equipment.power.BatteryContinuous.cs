
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

        public partial class ardupilot_equipment_power_BatteryContinuous : IDroneCANSerialize
        {
            public static void encode_ardupilot_equipment_power_BatteryContinuous(ardupilot_equipment_power_BatteryContinuous msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_ardupilot_equipment_power_BatteryContinuous(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_ardupilot_equipment_power_BatteryContinuous(CanardRxTransfer transfer, ardupilot_equipment_power_BatteryContinuous msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_ardupilot_equipment_power_BatteryContinuous(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_ardupilot_equipment_power_BatteryContinuous(uint8_t[] buffer, ardupilot_equipment_power_BatteryContinuous msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                {
                    uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.temperature_cells);
                    canardEncodeScalar(buffer, 0, 16, float16_val);
                }
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                {
                    uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.temperature_pcb);
                    canardEncodeScalar(buffer, 0, 16, float16_val);
                }
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                {
                    uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.temperature_other);
                    canardEncodeScalar(buffer, 0, 16, float16_val);
                }
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.current);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.voltage);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                {
                    uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.state_of_charge);
                    canardEncodeScalar(buffer, 0, 16, float16_val);
                }
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.slot_id);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.capacity_consumed);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.status_flags);
                chunk_cb(buffer, 32, ctx);
            }

            internal static void _decode_ardupilot_equipment_power_BatteryContinuous(CanardRxTransfer transfer,ref uint32_t bit_ofs, ardupilot_equipment_power_BatteryContinuous msg, bool tao) {

                {
                    uint16_t float16_val = 0;
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                    msg.temperature_cells = canardConvertFloat16ToNativeFloat(float16_val);
                }
                bit_ofs += 16;

                {
                    uint16_t float16_val = 0;
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                    msg.temperature_pcb = canardConvertFloat16ToNativeFloat(float16_val);
                }
                bit_ofs += 16;

                {
                    uint16_t float16_val = 0;
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                    msg.temperature_other = canardConvertFloat16ToNativeFloat(float16_val);
                }
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.current);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.voltage);
                bit_ofs += 32;

                {
                    uint16_t float16_val = 0;
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                    msg.state_of_charge = canardConvertFloat16ToNativeFloat(float16_val);
                }
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.slot_id);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.capacity_consumed);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.status_flags);
                bit_ofs += 32;

            }
        }
    }
}