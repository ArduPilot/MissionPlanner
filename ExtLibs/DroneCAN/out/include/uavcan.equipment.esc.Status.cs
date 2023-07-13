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
        public partial class uavcan_equipment_esc_Status: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_ESC_STATUS_MAX_PACK_SIZE = 14;
            public const ulong UAVCAN_EQUIPMENT_ESC_STATUS_DT_SIG = 0xA9AF28AEA2FBB254;
            public const int UAVCAN_EQUIPMENT_ESC_STATUS_DT_ID = 1034;

            public uint32_t error_count = new uint32_t();
            public Single voltage = new Single();
            public Single current = new Single();
            public Single temperature = new Single();
            public int32_t rpm = new int32_t();
            public uint8_t power_rating_pct = new uint8_t();
            public uint8_t esc_index = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_equipment_esc_Status(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_equipment_esc_Status(transfer, this, fdcan);
            }

            public static uavcan_equipment_esc_Status ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_equipment_esc_Status();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}