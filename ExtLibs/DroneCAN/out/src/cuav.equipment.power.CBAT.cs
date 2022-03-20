
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
        static void encode_cuav_equipment_power_CBAT(cuav_equipment_power_CBAT msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_cuav_equipment_power_CBAT(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_cuav_equipment_power_CBAT(CanardRxTransfer transfer, cuav_equipment_power_CBAT msg) {
            uint32_t bit_ofs = 0;
            _decode_cuav_equipment_power_CBAT(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_cuav_equipment_power_CBAT(uint8_t[] buffer, cuav_equipment_power_CBAT msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.temperature);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.voltage);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 4, msg.voltage_cell_len);
            chunk_cb(buffer, 4, ctx);
            for (int i=0; i < msg.voltage_cell_len; i++) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 32, msg.voltage_cell[i]);
                    chunk_cb(buffer, 32, ctx);
            }
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.cell_count);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.current);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.average_current);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.average_power);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.available_energy);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.remaining_capacity);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.full_charge_capacity);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.design_capacity);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.average_time_to_empty);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.average_time_to_full);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 7, msg.state_of_health);
            chunk_cb(buffer, 7, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 7, msg.state_of_charge);
            chunk_cb(buffer, 7, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 7, msg.max_error);
            chunk_cb(buffer, 7, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.serial_number);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.manufacture_date);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.cycle_count);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.over_discharge_count);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.passed_charge);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.nominal_voltage);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 1, msg.is_powering_off);
            chunk_cb(buffer, 1, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.interface_error);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 11, msg.status_flags);
            chunk_cb(buffer, 11, ctx);
        }

        static void _decode_cuav_equipment_power_CBAT(CanardRxTransfer transfer,ref uint32_t bit_ofs, cuav_equipment_power_CBAT msg, bool tao) {

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.temperature);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.voltage);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.voltage_cell_len);
            bit_ofs += 4;
            msg.voltage_cell = new Single[msg.voltage_cell_len];
            for (int i=0; i < msg.voltage_cell_len; i++) {
                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.voltage_cell[i]);
                bit_ofs += 32;
            }

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.cell_count);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.current);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.average_current);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.average_power);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.available_energy);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.remaining_capacity);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.full_charge_capacity);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.design_capacity);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.average_time_to_empty);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.average_time_to_full);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.state_of_health);
            bit_ofs += 7;

            canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.state_of_charge);
            bit_ofs += 7;

            canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.max_error);
            bit_ofs += 7;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.serial_number);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.manufacture_date);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.cycle_count);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.over_discharge_count);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.passed_charge);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.nominal_voltage);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.is_powering_off);
            bit_ofs += 1;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.interface_error);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 11, false, ref msg.status_flags);
            bit_ofs += 11;

        }
    }
}