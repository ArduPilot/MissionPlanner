
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
//using uavcan.tunnel.Protocol.cs
        public partial class uavcan_tunnel_Broadcast: IDroneCANSerialize 
        {
            public const int UAVCAN_TUNNEL_BROADCAST_MAX_PACK_SIZE = 63;
            public const ulong UAVCAN_TUNNEL_BROADCAST_DT_SIG = 0x5AA2D4D9CF4B1E85;
            public const int UAVCAN_TUNNEL_BROADCAST_DT_ID = 2010;

            public uavcan_tunnel_Protocol protocol = new uavcan_tunnel_Protocol();
            public uint8_t channel_id = new uint8_t();
            public uint8_t buffer_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=60)] public uint8_t[] buffer = Enumerable.Range(1, 60).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_tunnel_Broadcast(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_tunnel_Broadcast(transfer, this);
            }

            public static uavcan_tunnel_Broadcast ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_tunnel_Broadcast();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}