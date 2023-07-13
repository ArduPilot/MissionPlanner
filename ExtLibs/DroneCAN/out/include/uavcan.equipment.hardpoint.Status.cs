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
        public partial class uavcan_equipment_hardpoint_Status: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_HARDPOINT_STATUS_MAX_PACK_SIZE = 7;
            public const ulong UAVCAN_EQUIPMENT_HARDPOINT_STATUS_DT_SIG = 0x624A519D42553D82;
            public const int UAVCAN_EQUIPMENT_HARDPOINT_STATUS_DT_ID = 1071;

            public uint8_t hardpoint_id = new uint8_t();
            public Single payload_weight = new Single();
            public Single payload_weight_variance = new Single();
            public uint16_t status = new uint16_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_equipment_hardpoint_Status(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_equipment_hardpoint_Status(transfer, this, fdcan);
            }

            public static uavcan_equipment_hardpoint_Status ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_equipment_hardpoint_Status();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}