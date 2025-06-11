
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

        public partial class ardupilot_equipment_power_BatteryTag : IDroneCANSerialize
        {
            public static void encode_ardupilot_equipment_power_BatteryTag(ardupilot_equipment_power_BatteryTag msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_ardupilot_equipment_power_BatteryTag(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_ardupilot_equipment_power_BatteryTag(CanardRxTransfer transfer, ardupilot_equipment_power_BatteryTag msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_ardupilot_equipment_power_BatteryTag(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_ardupilot_equipment_power_BatteryTag(uint8_t[] buffer, ardupilot_equipment_power_BatteryTag msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.serial_number);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.num_cycles);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.armed_hours);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.battery_capacity_mAh);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.first_use_mins);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.last_arm_time_mins);
                chunk_cb(buffer, 32, ctx);
            }

            internal static void _decode_ardupilot_equipment_power_BatteryTag(CanardRxTransfer transfer,ref uint32_t bit_ofs, ardupilot_equipment_power_BatteryTag msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.serial_number);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.num_cycles);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.armed_hours);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.battery_capacity_mAh);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.first_use_mins);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.last_arm_time_mins);
                bit_ofs += 32;

            }
        }
    }
}