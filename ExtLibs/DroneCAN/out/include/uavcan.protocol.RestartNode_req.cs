
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
        public partial class uavcan_protocol_RestartNode_req: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_RESTARTNODE_REQ_MAX_PACK_SIZE = 5;
            public const ulong UAVCAN_PROTOCOL_RESTARTNODE_REQ_DT_SIG = 0x569E05394A3017F0;
            public const int UAVCAN_PROTOCOL_RESTARTNODE_REQ_DT_ID = 5;

            public const double UAVCAN_PROTOCOL_RESTARTNODE_REQ_MAGIC_NUMBER = 742196058910; // saturated uint40

            public uint64_t magic_number = new uint64_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_protocol_RestartNode_req(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_protocol_RestartNode_req(transfer, this);
            }

            public static uavcan_protocol_RestartNode_req ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_protocol_RestartNode_req();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}