
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
        public partial class uavcan_equipment_hardpoint_Command: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_HARDPOINT_COMMAND_MAX_PACK_SIZE = 3;
            public const ulong UAVCAN_EQUIPMENT_HARDPOINT_COMMAND_DT_SIG = 0xA1A036268B0C3455;
            public const int UAVCAN_EQUIPMENT_HARDPOINT_COMMAND_DT_ID = 1070;

            public uint8_t hardpoint_id = new uint8_t();
            public uint16_t command = new uint16_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_equipment_hardpoint_Command(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_equipment_hardpoint_Command(transfer, this);
            }

            public static uavcan_equipment_hardpoint_Command ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_equipment_hardpoint_Command();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}