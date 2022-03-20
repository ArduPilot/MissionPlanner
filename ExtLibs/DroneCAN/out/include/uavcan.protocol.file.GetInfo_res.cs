
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
//using uavcan.protocol.file.EntryType.cs
//using uavcan.protocol.file.Error.cs
        public partial class uavcan_protocol_file_GetInfo_res: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_FILE_GETINFO_RES_MAX_PACK_SIZE = 8;
            public const ulong UAVCAN_PROTOCOL_FILE_GETINFO_RES_DT_SIG = 0x5004891EE8A27531;
            public const int UAVCAN_PROTOCOL_FILE_GETINFO_RES_DT_ID = 45;

            public uint64_t size = new uint64_t();
            public uavcan_protocol_file_Error error = new uavcan_protocol_file_Error();
            public uavcan_protocol_file_EntryType entry_type = new uavcan_protocol_file_EntryType();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_protocol_file_GetInfo_res(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_protocol_file_GetInfo_res(transfer, this);
            }

            public static uavcan_protocol_file_GetInfo_res ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_protocol_file_GetInfo_res();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}