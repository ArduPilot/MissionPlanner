
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
        static void encode_ardupilot_equipment_power_BatteryInfoAux(ardupilot_equipment_power_BatteryInfoAux msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_ardupilot_equipment_power_BatteryInfoAux(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_ardupilot_equipment_power_BatteryInfoAux(CanardRxTransfer transfer, ardupilot_equipment_power_BatteryInfoAux msg) {
            uint32_t bit_ofs = 0;
            _decode_ardupilot_equipment_power_BatteryInfoAux(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_ardupilot_equipment_power_BatteryInfoAux(uint8_t[] buffer, ardupilot_equipment_power_BatteryInfoAux msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.voltage_cell_len);
            chunk_cb(buffer, 8, ctx);
            for (int i=0; i < msg.voltage_cell_len; i++) {
                    memset(buffer,0,8);
                    {
                        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.voltage_cell[i]);
                        canardEncodeScalar(buffer, 0, 16, float16_val);
                    }
                    chunk_cb(buffer, 16, ctx);
            }
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.cycle_count);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.over_discharge_count);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.max_current);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.nominal_voltage);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 1, msg.is_powering_off);
            chunk_cb(buffer, 1, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.battery_id);
            chunk_cb(buffer, 8, ctx);
        }

        static void _decode_ardupilot_equipment_power_BatteryInfoAux(CanardRxTransfer transfer,ref uint32_t bit_ofs, ardupilot_equipment_power_BatteryInfoAux msg, bool tao) {

            _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.voltage_cell_len);
            bit_ofs += 8;
            msg.voltage_cell = new Single[msg.voltage_cell_len];
            for (int i=0; i < msg.voltage_cell_len; i++) {
                {
                    uint16_t float16_val = 0;
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                    msg.voltage_cell[i] = canardConvertFloat16ToNativeFloat(float16_val);
                }
                bit_ofs += 16;
            }

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.cycle_count);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.over_discharge_count);
            bit_ofs += 16;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.max_current = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.nominal_voltage = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.is_powering_off);
            bit_ofs += 1;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.battery_id);
            bit_ofs += 8;

        }
    }
}