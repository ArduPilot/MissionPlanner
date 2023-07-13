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
        public partial class uavcan_equipment_indication_BeepCommand: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_INDICATION_BEEPCOMMAND_MAX_PACK_SIZE = 4;
            public const ulong UAVCAN_EQUIPMENT_INDICATION_BEEPCOMMAND_DT_SIG = 0xBE9EA9FEC2B15D52;
            public const int UAVCAN_EQUIPMENT_INDICATION_BEEPCOMMAND_DT_ID = 1080;

            public Single frequency = new Single();
            public Single duration = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_equipment_indication_BeepCommand(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_equipment_indication_BeepCommand(transfer, this, fdcan);
            }

            public static uavcan_equipment_indication_BeepCommand ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_equipment_indication_BeepCommand();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}