
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
        public partial class uavcan_protocol_dynamic_node_id_server_Entry: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_ENTRY_MAX_PACK_SIZE = 21;
            public const ulong UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_ENTRY_DT_SIG = 0x7FAA779D64FA75C2;

            public uint32_t term = new uint32_t();
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)] public uint8_t[] unique_id = new uint8_t[16];
            public uint8_t node_id = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_protocol_dynamic_node_id_server_Entry(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_protocol_dynamic_node_id_server_Entry(transfer, this);
            }

            public static uavcan_protocol_dynamic_node_id_server_Entry ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_protocol_dynamic_node_id_server_Entry();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}