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
        public partial class dronecan_protocol_Stats: IDroneCANSerialize 
        {
            public const int DRONECAN_PROTOCOL_STATS_MAX_PACK_SIZE = 28;
            public const ulong DRONECAN_PROTOCOL_STATS_DT_SIG = 0x763AE3B8A986F8D1;
            public const int DRONECAN_PROTOCOL_STATS_DT_ID = 342;

            public uint32_t tx_frames = new uint32_t();
            public uint16_t tx_errors = new uint16_t();
            public uint32_t rx_frames = new uint32_t();
            public uint16_t rx_error_oom = new uint16_t();
            public uint16_t rx_error_internal = new uint16_t();
            public uint16_t rx_error_missed_start = new uint16_t();
            public uint16_t rx_error_wrong_toggle = new uint16_t();
            public uint16_t rx_error_short_frame = new uint16_t();
            public uint16_t rx_error_bad_crc = new uint16_t();
            public uint16_t rx_ignored_wrong_address = new uint16_t();
            public uint16_t rx_ignored_not_wanted = new uint16_t();
            public uint16_t rx_ignored_unexpected_tid = new uint16_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_dronecan_protocol_Stats(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_dronecan_protocol_Stats(transfer, this, fdcan);
            }

            public static dronecan_protocol_Stats ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new dronecan_protocol_Stats();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}