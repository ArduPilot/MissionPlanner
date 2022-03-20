
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
        static void encode_uavcan_equipment_actuator_ArrayCommand(uavcan_equipment_actuator_ArrayCommand msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_uavcan_equipment_actuator_ArrayCommand(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_uavcan_equipment_actuator_ArrayCommand(CanardRxTransfer transfer, uavcan_equipment_actuator_ArrayCommand msg) {
            uint32_t bit_ofs = 0;
            _decode_uavcan_equipment_actuator_ArrayCommand(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_uavcan_equipment_actuator_ArrayCommand(uint8_t[] buffer, uavcan_equipment_actuator_ArrayCommand msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            if (!tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 4, msg.commands_len);
                chunk_cb(buffer, 4, ctx);
            }
            for (int i=0; i < msg.commands_len; i++) {
                    _encode_uavcan_equipment_actuator_Command(buffer, msg.commands[i], chunk_cb, ctx, false);
            }
        }

        static void _decode_uavcan_equipment_actuator_ArrayCommand(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_actuator_ArrayCommand msg, bool tao) {

            if (!tao) {
                canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.commands_len);
                bit_ofs += 4;
            }


            if (tao) {
                msg.commands_len = 0;
                var temp = new List<uavcan_equipment_actuator_Command>();
                while (((transfer.payload_len*8)-bit_ofs) > 0) {
                    msg.commands_len++;
                    temp.Add(new uavcan_equipment_actuator_Command());
                    _decode_uavcan_equipment_actuator_Command(transfer, ref bit_ofs, temp[msg.commands_len - 1], false);
                }
                msg.commands = temp.ToArray();
            } else {
                msg.commands = new uavcan_equipment_actuator_Command[msg.commands_len];
                for (int i=0; i < msg.commands_len; i++) {
                    _decode_uavcan_equipment_actuator_Command(transfer, ref bit_ofs, msg.commands[i], false);
                }
            }

        }
    }
}