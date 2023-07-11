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
        public partial class dronecan_protocol_CanStats: IDroneCANSerialize 
        {
            public const int DRONECAN_PROTOCOL_CANSTATS_MAX_PACK_SIZE = 25;
            public const ulong DRONECAN_PROTOCOL_CANSTATS_DT_SIG = 0xCE080CAE3CA33C75;
            public const int DRONECAN_PROTOCOL_CANSTATS_DT_ID = 343;

            public uint8_t @interface = new uint8_t();
            public uint32_t tx_requests = new uint32_t();
            public uint16_t tx_rejected = new uint16_t();
            public uint16_t tx_overflow = new uint16_t();
            public uint16_t tx_success = new uint16_t();
            public uint16_t tx_timedout = new uint16_t();
            public uint16_t tx_abort = new uint16_t();
            public uint32_t rx_received = new uint32_t();
            public uint16_t rx_overflow = new uint16_t();
            public uint16_t rx_errors = new uint16_t();
            public uint16_t busoff_errors = new uint16_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_dronecan_protocol_CanStats(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_dronecan_protocol_CanStats(transfer, this, fdcan);
            }

            public static dronecan_protocol_CanStats ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new dronecan_protocol_CanStats();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}