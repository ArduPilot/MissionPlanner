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
//using uavcan.Timestamp.cs
        public partial class dronecan_protocol_GlobalTime: IDroneCANSerialize 
        {
            public const int DRONECAN_PROTOCOL_GLOBALTIME_MAX_PACK_SIZE = 7;
            public const ulong DRONECAN_PROTOCOL_GLOBALTIME_DT_SIG = 0xA55177448A490F33;
            public const int DRONECAN_PROTOCOL_GLOBALTIME_DT_ID = 344;

            public uavcan_Timestamp timestamp = new uavcan_Timestamp();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_dronecan_protocol_GlobalTime(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_dronecan_protocol_GlobalTime(transfer, this, fdcan);
            }

            public static dronecan_protocol_GlobalTime ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new dronecan_protocol_GlobalTime();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}