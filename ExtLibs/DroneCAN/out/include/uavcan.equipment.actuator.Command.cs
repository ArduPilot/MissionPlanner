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

namespace DroneCAN
{
    public partial class DroneCAN 
    {
        public partial class uavcan_equipment_actuator_Command: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_ACTUATOR_COMMAND_MAX_PACK_SIZE = 4;
            public const ulong UAVCAN_EQUIPMENT_ACTUATOR_COMMAND_DT_SIG = 0x8D9A6A920C1D616C;

            public const double UAVCAN_EQUIPMENT_ACTUATOR_COMMAND_COMMAND_TYPE_UNITLESS = 0; // saturated uint8
            public const double UAVCAN_EQUIPMENT_ACTUATOR_COMMAND_COMMAND_TYPE_POSITION = 1; // saturated uint8
            public const double UAVCAN_EQUIPMENT_ACTUATOR_COMMAND_COMMAND_TYPE_FORCE = 2; // saturated uint8
            public const double UAVCAN_EQUIPMENT_ACTUATOR_COMMAND_COMMAND_TYPE_SPEED = 3; // saturated uint8
            public const double UAVCAN_EQUIPMENT_ACTUATOR_COMMAND_COMMAND_TYPE_PWM = 4; // saturated uint8

            public uint8_t actuator_id = new uint8_t();
            public uint8_t command_type = new uint8_t();
            public Single command_value = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_equipment_actuator_Command(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_equipment_actuator_Command(transfer, this, fdcan);
            }

            public static uavcan_equipment_actuator_Command ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_equipment_actuator_Command();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}