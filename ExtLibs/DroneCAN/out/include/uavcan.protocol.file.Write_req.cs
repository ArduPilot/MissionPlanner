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
        public partial class uavcan_protocol_file_Write_req: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_FILE_WRITE_REQ_MAX_PACK_SIZE = 399;
            public const ulong UAVCAN_PROTOCOL_FILE_WRITE_REQ_DT_SIG = 0x515AA1DC77E58429;
            public const int UAVCAN_PROTOCOL_FILE_WRITE_REQ_DT_ID = 49;

            public uint64_t offset = new uint64_t();
            public uavcan_protocol_file_Path path = new uavcan_protocol_file_Path();
            public uint8_t data_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=192)] public uint8_t[] data = Enumerable.Range(1, 192).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_protocol_file_Write_req(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_protocol_file_Write_req(transfer, this, fdcan);
            }

            public static uavcan_protocol_file_Write_req ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_protocol_file_Write_req();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}