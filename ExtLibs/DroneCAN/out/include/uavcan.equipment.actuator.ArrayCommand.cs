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
//using uavcan.equipment.actuator.Command.cs
        public partial class uavcan_equipment_actuator_ArrayCommand: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_ACTUATOR_ARRAYCOMMAND_MAX_PACK_SIZE = 61;
            public const ulong UAVCAN_EQUIPMENT_ACTUATOR_ARRAYCOMMAND_DT_SIG = 0xD8A7486238EC3AF3;
            public const int UAVCAN_EQUIPMENT_ACTUATOR_ARRAYCOMMAND_DT_ID = 1010;

            public uint8_t commands_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=15)] public uavcan_equipment_actuator_Command[] commands = Enumerable.Range(1, 15).Select(i => new uavcan_equipment_actuator_Command()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_equipment_actuator_ArrayCommand(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_equipment_actuator_ArrayCommand(transfer, this, fdcan);
            }

            public static uavcan_equipment_actuator_ArrayCommand ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_equipment_actuator_ArrayCommand();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}