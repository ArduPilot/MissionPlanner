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
        public partial class uavcan_Timestamp: IDroneCANSerialize 
        {
            public const int UAVCAN_TIMESTAMP_MAX_PACK_SIZE = 7;
            public const ulong UAVCAN_TIMESTAMP_DT_SIG = 0x5BD0B5C81087E0D;

            public const double UAVCAN_TIMESTAMP_UNKNOWN = 0; // saturated uint56

            public uint64_t usec = new uint64_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_Timestamp(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_Timestamp(transfer, this, fdcan);
            }

            public static uavcan_Timestamp ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_Timestamp();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}