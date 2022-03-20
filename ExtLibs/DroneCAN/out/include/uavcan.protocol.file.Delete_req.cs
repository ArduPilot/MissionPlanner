
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
//using uavcan.protocol.file.Path.cs
        public partial class uavcan_protocol_file_Delete_req: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_FILE_DELETE_REQ_MAX_PACK_SIZE = 201;
            public const ulong UAVCAN_PROTOCOL_FILE_DELETE_REQ_DT_SIG = 0x78648C99170B47AA;
            public const int UAVCAN_PROTOCOL_FILE_DELETE_REQ_DT_ID = 47;

            public uavcan_protocol_file_Path path = new uavcan_protocol_file_Path();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_protocol_file_Delete_req(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_protocol_file_Delete_req(transfer, this);
            }

            public static uavcan_protocol_file_Delete_req ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_protocol_file_Delete_req();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}