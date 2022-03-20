
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
//using uavcan.protocol.CANIfaceStats.cs
        public partial class uavcan_protocol_GetTransportStats_res: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_GETTRANSPORTSTATS_RES_MAX_PACK_SIZE = 73;
            public const ulong UAVCAN_PROTOCOL_GETTRANSPORTSTATS_RES_DT_SIG = 0xBE6F76A7EC312B04;
            public const int UAVCAN_PROTOCOL_GETTRANSPORTSTATS_RES_DT_ID = 4;

            public uint64_t transfers_tx = new uint64_t();
            public uint64_t transfers_rx = new uint64_t();
            public uint64_t transfer_errors = new uint64_t();
            public uint8_t can_iface_stats_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public uavcan_protocol_CANIfaceStats[] can_iface_stats = Enumerable.Range(1, 3).Select(i => new uavcan_protocol_CANIfaceStats()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_protocol_GetTransportStats_res(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_protocol_GetTransportStats_res(transfer, this);
            }

            public static uavcan_protocol_GetTransportStats_res ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_protocol_GetTransportStats_res();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}