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
        public partial class dronecan_protocol_FlexDebug: IDroneCANSerialize 
        {
            public const int DRONECAN_PROTOCOL_FLEXDEBUG_MAX_PACK_SIZE = 258;
            public const ulong DRONECAN_PROTOCOL_FLEXDEBUG_DT_SIG = 0xECA60382FF038F39;
            public const int DRONECAN_PROTOCOL_FLEXDEBUG_DT_ID = 16371;

            public const double DRONECAN_PROTOCOL_FLEXDEBUG_RESERVATION_SIZE = 10; // saturated uint16
            public const double DRONECAN_PROTOCOL_FLEXDEBUG_AM32_RESERVE_START = 100; // saturated uint16
            public const double DRONECAN_PROTOCOL_FLEXDEBUG_MLRS_RESERVE_START = 110; // saturated uint16

            public uint16_t id = new uint16_t();
            public uint8_t u8_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=255)] public uint8_t[] u8 = Enumerable.Range(1, 255).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_dronecan_protocol_FlexDebug(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_dronecan_protocol_FlexDebug(transfer, this, fdcan);
            }

            public static dronecan_protocol_FlexDebug ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new dronecan_protocol_FlexDebug();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}