
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

        public partial class ardupilot_equipment_power_BatteryPeriodic : IDroneCANSerialize
        {
            public static void encode_ardupilot_equipment_power_BatteryPeriodic(ardupilot_equipment_power_BatteryPeriodic msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_ardupilot_equipment_power_BatteryPeriodic(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_ardupilot_equipment_power_BatteryPeriodic(CanardRxTransfer transfer, ardupilot_equipment_power_BatteryPeriodic msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_ardupilot_equipment_power_BatteryPeriodic(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_ardupilot_equipment_power_BatteryPeriodic(uint8_t[] buffer, ardupilot_equipment_power_BatteryPeriodic msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 6, msg.name_len);
                chunk_cb(buffer, 6, ctx);
                for (int i=0; i < msg.name_len; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 8, msg.name[i]);
                        chunk_cb(buffer, 8, ctx);
                }
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 6, msg.serial_number_len);
                chunk_cb(buffer, 6, ctx);
                for (int i=0; i < msg.serial_number_len; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 8, msg.serial_number[i]);
                        chunk_cb(buffer, 8, ctx);
                }
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 4, msg.manufacture_date_len);
                chunk_cb(buffer, 4, ctx);
                for (int i=0; i < msg.manufacture_date_len; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 8, msg.manufacture_date[i]);
                        chunk_cb(buffer, 8, ctx);
                }
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.design_capacity);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.cells_in_series);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                {
                    uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.nominal_voltage);
                    canardEncodeScalar(buffer, 0, 16, float16_val);
                }
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                {
                    uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.discharge_minimum_voltage);
                    canardEncodeScalar(buffer, 0, 16, float16_val);
                }
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                {
                    uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.charging_minimum_voltage);
                    canardEncodeScalar(buffer, 0, 16, float16_val);
                }
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                {
                    uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.charging_maximum_voltage);
                    canardEncodeScalar(buffer, 0, 16, float16_val);
                }
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.charging_maximum_current);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.discharge_maximum_current);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.discharge_maximum_burst_current);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.full_charge_capacity);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.cycle_count);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.state_of_health);
                chunk_cb(buffer, 8, ctx);
            }

            internal static void _decode_ardupilot_equipment_power_BatteryPeriodic(CanardRxTransfer transfer,ref uint32_t bit_ofs, ardupilot_equipment_power_BatteryPeriodic msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.name_len);
                bit_ofs += 6;
                msg.name = new uint8_t[msg.name_len];
                for (int i=0; i < msg.name_len; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.name[i]);
                    bit_ofs += 8;
                }

                canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.serial_number_len);
                bit_ofs += 6;
                msg.serial_number = new uint8_t[msg.serial_number_len];
                for (int i=0; i < msg.serial_number_len; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.serial_number[i]);
                    bit_ofs += 8;
                }

                canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.manufacture_date_len);
                bit_ofs += 4;
                msg.manufacture_date = new uint8_t[msg.manufacture_date_len];
                for (int i=0; i < msg.manufacture_date_len; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.manufacture_date[i]);
                    bit_ofs += 8;
                }

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.design_capacity);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.cells_in_series);
                bit_ofs += 8;

                {
                    uint16_t float16_val = 0;
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                    msg.nominal_voltage = canardConvertFloat16ToNativeFloat(float16_val);
                }
                bit_ofs += 16;

                {
                    uint16_t float16_val = 0;
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                    msg.discharge_minimum_voltage = canardConvertFloat16ToNativeFloat(float16_val);
                }
                bit_ofs += 16;

                {
                    uint16_t float16_val = 0;
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                    msg.charging_minimum_voltage = canardConvertFloat16ToNativeFloat(float16_val);
                }
                bit_ofs += 16;

                {
                    uint16_t float16_val = 0;
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                    msg.charging_maximum_voltage = canardConvertFloat16ToNativeFloat(float16_val);
                }
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.charging_maximum_current);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.discharge_maximum_current);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.discharge_maximum_burst_current);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.full_charge_capacity);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.cycle_count);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.state_of_health);
                bit_ofs += 8;

            }
        }
    }
}